// Disposable fixtures evaluated by Unity Pipeline's eval_file in Play Mode.
// Run from the repository root with an absolute --file path. No scene is saved.
// This probes the real sandbox input resolver/Physics2D/camera; it does not
// replace physical-key, representative-art, network, or human feel acceptance.
var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
var source = UnityEngine.GameObject.Find("PlayerTest").GetComponent<PlayerTestMovement>();
var sourceCamera = UnityEngine.Camera.main.GetComponent<CameraFollowTest>();
if (!UnityEditor.EditorApplication.isPlaying) throw new System.Exception("Enter Play Mode first.");
object Read(object component, string field) => component.GetType().GetField(field, flags).GetValue(component);
void Set(object component, string field, object value) => component.GetType().GetField(field, flags).SetValue(component, value);
void Call(object component, string method, params object[] args) => component.GetType().GetMethod(method, flags).Invoke(component, args);
var results = new System.Collections.Generic.List<object>();
var jumps = new System.Collections.Generic.Dictionary<string, float>();
int failures = 0;
void Check(string id, bool ok, object evidence) {
    if (!ok) failures++;
    results.Add(new { id, numerical = ok ? "PASS" : "FAIL", evidence });
}
string[] movementFields = { "moveSpeed", "jumpForce", "fallGravityMultiplier", "holdThreshold", "minimumStage2UpwardVelocity", "stage2VelocityFraction" };
string[] cameraFields = { "verticalOffset", "verticalSmoothTime", "upperSoftZone", "lowerSoftZone", "upwardLookAhead", "downwardLookAhead", "maxLookAhead", "upwardSmoothTime", "downwardSmoothTime", "safeViewMargin", "pixelsPerUnit", "quantizeToWorldPixel" };
float dt = UnityEngine.Time.fixedDeltaTime;
int matchingHoldTick = (int)System.Math.Ceiling((float)Read(source, "holdThreshold") / dt);

// Tuple: isolated scene, root, player, body, movement, camera follow, camera.
(UnityEngine.SceneManagement.Scene, UnityEngine.GameObject, UnityEngine.GameObject, UnityEngine.Rigidbody2D, PlayerTestMovement, CameraFollowTest, UnityEngine.Camera) Make(float height = 0f) {
    var scene = UnityEngine.SceneManagement.SceneManager.CreateScene("MovementCameraProbe_" + System.Guid.NewGuid().ToString("N"), new UnityEngine.SceneManagement.CreateSceneParameters(UnityEngine.SceneManagement.LocalPhysicsMode.Physics2D));
    var root = new UnityEngine.GameObject("DisposableMovementCameraProbe");
    UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(root, scene);
    var floor = new UnityEngine.GameObject("GroundTilemap");
    floor.transform.SetParent(root.transform);
    floor.transform.position = new UnityEngine.Vector3(0, height - 0.5f, 0);
    floor.AddComponent<UnityEngine.BoxCollider2D>().size = new UnityEngine.Vector2(200, 1);
    var player = new UnityEngine.GameObject("ProbePlayer");
    player.transform.SetParent(root.transform);
    player.transform.position = new UnityEngine.Vector3(0, height + 0.51f, 0);
    var rb = player.AddComponent<UnityEngine.Rigidbody2D>();
    rb.gravityScale = (float)Read(source, "defaultGravityScale");
    rb.constraints = UnityEngine.RigidbodyConstraints2D.FreezeRotation;
    player.AddComponent<UnityEngine.BoxCollider2D>().size = new UnityEngine.Vector2(0.5f, 1);
    var movement = player.AddComponent<PlayerTestMovement>();
    movement.enabled = false; // drive its input resolver explicitly; collision callbacks still run.
    foreach (var field in movementFields) Set(movement, field, Read(source, field));
    var view = new UnityEngine.GameObject("ProbeCamera");
    view.transform.SetParent(root.transform);
    view.transform.position = new UnityEngine.Vector3(0, 0, -10);
    var camera = view.AddComponent<UnityEngine.Camera>();
    camera.enabled = false;
    camera.orthographic = true;
    camera.orthographicSize = UnityEngine.Camera.main.orthographicSize;
    var follow = view.AddComponent<CameraFollowTest>();
    follow.enabled = false;
    foreach (var field in cameraFields) Set(follow, field, Read(sourceCamera, field));
    Set(follow, "target", player.transform);
    Set(follow, "movement", movement);
    for (int i = 0; i < 15; i++) scene.GetPhysicsScene2D().Simulate(dt);
    Call(follow, "Start");
    if (!movement.IsGrounded) throw new System.Exception("Ground contact fixture did not establish support.");
    return (scene, root, player, rb, movement, follow, camera);
}
void Clean(UnityEngine.SceneManagement.Scene scene, UnityEngine.GameObject root) {
    UnityEngine.Object.DestroyImmediate(root);
    UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);
}

foreach (string route in new[] { "tap", "hold", "double", "relaxed", "near-apex", "falling", "hold+tap", "reset", "short-hold" }) {
    var f = Make();
    try {
        float baselineY = f.Item4.position.y;
        float cameraY = f.Item7.transform.position.y;
        float apex = baselineY;
        float extensionTime = -1;
        float extensionContribution = 0;
        float cameraDrift = 0;
        bool revealedWhileAscending = false;
        bool wasConsumed = false;
        bool fallingPressRejected = false;
        bool immediateExtension = false;
        int extensions = 0;
        int landings = 0;
        int launchIndex = 0;
        int nextLaunch = -1;
        bool airborne = false;
        bool heldRoute = route == "hold" || route == "hold+tap" || route == "reset";
        for (int i = 0; i < 160; i++) {
            if (i == nextLaunch) { launchIndex = i; nextLaunch = -1; airborne = false; }
            int local = i - launchIndex;
            // Compare hold vs second press at the SAME trigger tick. Additive
            // boosts at different ascent times can legitimately change apex.
            bool pressed = local == 0 || (route == "double" && local == matchingHoldTick) ||
                (route == "relaxed" && local == 25) || (route == "near-apex" && local == 34) || (route == "falling" && local == 45) ||
                (route == "hold+tap" && local == matchingHoldTick + 3);
            bool held = heldRoute ? (route != "hold+tap" || local <= matchingHoldTick + 1 || local == matchingHoldTick + 3) : route == "short-hold" ? local < 13 : pressed;
            float before = f.Item4.linearVelocity.y;
            Call(f.Item5, "ProcessJumpInput", pressed, held, i * dt);
            bool consumed = (bool)Read(f.Item5, "stage2Consumed");
            if (local == 0 && consumed) immediateExtension = true;
            if (consumed && !wasConsumed) {
                extensions++;
                extensionTime = local * dt;
                extensionContribution = f.Item4.linearVelocity.y - before;
            }
            wasConsumed = consumed;
            if (route == "falling" && local == 45)
                fallingPressRejected = before < 0f && f.Item4.linearVelocity.y == before;
            Call(f.Item5, "UpdateFallGravity");
            f.Item1.GetPhysicsScene2D().Simulate(dt);
            Call(f.Item6, "Follow", dt);
            apex = UnityEngine.Mathf.Max(apex, f.Item4.position.y);
            cameraDrift = UnityEngine.Mathf.Max(cameraDrift, UnityEngine.Mathf.Abs(f.Item7.transform.position.y - cameraY));
            if (!f.Item5.IsGrounded && f.Item5.VerticalVelocity > 0f && f.Item7.transform.position.y > cameraY + 0.01f)
                revealedWhileAscending = true;
            if (!f.Item5.IsGrounded) airborne = true;
            if (airborne && f.Item5.IsGrounded) {
                landings++;
                airborne = false;
                if (route == "reset" && landings == 1) nextLaunch = i + 10;
            }
        }
        float height = apex - baselineY;
        jumps[route] = height;
        string id = route == "tap" ? "JUMP-01" : route == "hold" ? "JUMP-02" : route == "double" ? "JUMP-03-same-tick" : route == "relaxed" ? "JUMP-03-relaxed" : route == "near-apex" ? "JUMP-04" : route == "falling" ? "JUMP-05" : route == "hold+tap" ? "JUMP-06" : route == "short-hold" ? "JUMP-08" : "JUMP-07";
        int wanted = route == "reset" ? 2 : heldRoute || route == "double" || route == "relaxed" ? 1 : 0;
        bool ok = extensions == wanted && !immediateExtension && landings == (route == "reset" ? 2 : 1);
        if (route == "hold") ok &= extensionTime >= (float)Read(source, "holdThreshold") && height > jumps["tap"] + 0.1f && height < jumps["tap"] * 1.4f;
        if (route == "double" || route == "hold+tap") ok &= UnityEngine.Mathf.Abs(height - jumps["hold"]) < 0.02f;
        if (route == "near-apex" || route == "falling" || route == "short-hold") ok &= UnityEngine.Mathf.Abs(height - jumps["tap"]) < 0.02f;
        if (route == "falling") ok &= fallingPressRejected;
        if (route == "relaxed") ok &= extensionTime > 0.26f && height > jumps["tap"] && extensionContribution > 0f;
        Check(id, ok, new { route, height, extensions, landings, extensionTime, extensionContribution, immediateExtension, fallingPressRejected });
        if (route == "tap") Check("CAM-01", cameraDrift < 0.05f, new { cameraDrift });
        if (route == "hold") Check("CAM-02", revealedWhileAscending, new { revealedWhileAscending, cameraDrift, fixture = "same-level landing; pre-apex reveal only, destination readability needs manual staircase" });
    } finally { Clean(f.Item1, f.Item2); }
}


{
    var f = Make();
    try {
        Call(f.Item5, "ProcessJumpInput", true, true, 0f);
        float cutoff = (float)Read(source, "minimumStage2UpwardVelocity");
        f.Item4.linearVelocity = new UnityEngine.Vector2(0, cutoff);
        Call(f.Item5, "ProcessJumpInput", true, false, 0.6f);
        bool rejectedAtCutoff = !(bool)Read(f.Item5, "stage2Consumed") && f.Item4.linearVelocity.y == cutoff;
        f.Item4.linearVelocity = new UnityEngine.Vector2(0, 3f);
        Call(f.Item5, "ProcessJumpInput", true, true, 0.62f);
        bool stayedClosed = !(bool)Read(f.Item5, "stage2Consumed") && f.Item4.linearVelocity.y == 3f;
        Check("JUMP-09", rejectedAtCutoff && stayedClosed, new { cutoff, rejectedAtCutoff, stayedClosed, fixture = "resolver boundary and synthetic positive velocity after window closure" });
    } finally { Clean(f.Item1, f.Item2); }
}

{
    var f = Make(12f);
    try {
        var low = new UnityEngine.GameObject("GroundTilemap"); low.transform.SetParent(f.Item2.transform);
        low.transform.position = new UnityEngine.Vector3(0, -0.5f, 0);
        low.AddComponent<UnityEngine.BoxCollider2D>().size = new UnityEngine.Vector2(200, 1);
        f.Item2.transform.GetChild(0).GetComponent<UnityEngine.BoxCollider2D>().enabled = false;
        float startCameraY = f.Item7.transform.position.y;
        bool preLandingFollow = false;
        bool landed = false;
        float largestDistance = 0;
        for (int i = 0; i < 180; i++) {
            Call(f.Item5, "UpdateFallGravity"); f.Item1.GetPhysicsScene2D().Simulate(dt); Call(f.Item6, "Follow", dt);
            largestDistance = UnityEngine.Mathf.Max(largestDistance, UnityEngine.Mathf.Abs(f.Item4.position.y - f.Item7.transform.position.y));
            if (!f.Item5.IsGrounded && f.Item7.transform.position.y < startCameraY - 0.01f) preLandingFollow = true;
            if (i > 5 && f.Item5.IsGrounded) { landed = true; break; }
        }
        float safe = f.Item7.orthographicSize - (float)Read(sourceCamera, "safeViewMargin");
        Check("CAM-03", preLandingFollow && landed && largestDistance <= safe + 0.001f, new { preLandingFollow, landed, largestDistance, safe });
    } finally { Clean(f.Item1, f.Item2); }
}
{
    var f = Make();
    try {
        bool allTransitionsBeforeLanding = true;
        float y = f.Item4.position.y;
        foreach (float delta in new[] { 2.7f, 2.7f, -2.7f, -2.7f }) {
            float before = f.Item7.transform.position.y;
            Set(f.Item5, "isGrounded", false);
            for (int i = 1; i <= 30; i++) {
                f.Item3.transform.position = new UnityEngine.Vector3(i * 0.1f, y + delta * i / 30f, 0);
                f.Item4.linearVelocity = new UnityEngine.Vector2(0, delta / (30f * dt));
                Call(f.Item6, "Follow", dt);
            }
            allTransitionsBeforeLanding &= delta > 0 ? f.Item7.transform.position.y > before + 0.01f : f.Item7.transform.position.y < before - 0.01f;
            y += delta; Set(f.Item5, "isGrounded", true); f.Item4.linearVelocity = UnityEngine.Vector2.zero;
            for (int i = 0; i < 30; i++) Call(f.Item6, "Follow", dt);
        }
        Check("CAM-04", allTransitionsBeforeLanding, new { allTransitionsBeforeLanding, fixture = "synthetic four-leg staircase trajectory; manual collision/route/feel remains pending" });
        Set(f.Item6, "quantizeToWorldPixel", true);
        bool gridAligned = true;
        foreach (float x in new[] { -1.013f, 0.017f, 2.129f }) {
            f.Item3.transform.position = new UnityEngine.Vector3(x, y, 0); Call(f.Item6, "Follow", dt);
            var pos = f.Item7.transform.position;
            gridAligned &= UnityEngine.Mathf.Abs(pos.x * 32f - UnityEngine.Mathf.Round(pos.x * 32f)) < 0.001f;
            gridAligned &= UnityEngine.Mathf.Abs(pos.y * 32f - UnityEngine.Mathf.Round(pos.y * 32f)) < 0.001f;
        }
        Check("CAM-05-grid-only", gridAligned, new { gridAligned, acceptance = "MANUAL_PENDING: no animated-art shimmer review; default quantization remains off" });
    } finally { Clean(f.Item1, f.Item2); }
}
var movementValues = new System.Collections.Generic.Dictionary<string, object>();
foreach (string field in movementFields) movementValues[field] = Read(source, field);
var cameraValues = new System.Collections.Generic.Dictionary<string, object>();
foreach (string field in cameraFields) cameraValues[field] = Read(sourceCamera, field);
var report = new { scope = "Numerical probe only, not manual/G-L/G-N acceptance", editor = UnityEngine.Application.unityVersion, dt, movementValues, cameraValues, failures, results };
System.IO.File.WriteAllText("/tmp/huyenlo-movement-camera-results.json", Newtonsoft.Json.JsonConvert.SerializeObject(report, Newtonsoft.Json.Formatting.Indented));
return new { failures, numericalChecks = results.Count, report = "/tmp/huyenlo-movement-camera-results.json" };
