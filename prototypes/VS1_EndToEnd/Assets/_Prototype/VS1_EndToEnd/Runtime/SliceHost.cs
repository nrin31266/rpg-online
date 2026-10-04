using System;
using System.Collections.Generic;
using System.Linq;
using HuyenLo.Domain;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HuyenLo.Runtime
{
    public sealed class SliceHost : MonoBehaviour
    {
        public SliceSession Session {get;private set;}
        private readonly HeldMovementState heldMovement=new HeldMovementState();
        private int heldMask;
        private InputAction cycleBinding;
        private int cycleRequest;
        public Rigidbody2D Body {get;private set;}
        public bool Grounded {get;private set;}
        public bool Modal => Hud!=null && Hud.Modal;
        public SliceHud Hud {get;private set;}
        public bool ExternalInput;
        public int ExternalAxis;
        public bool ExternalJump,ExternalDrop;
        private int axis;
        public bool ExternalJumpHeld=true;
        private bool jumpHeld=true;
        private double lastGroundAt=-100,jumpBufferedUntil=-100;
        // Local feel probe only: durations/forces are not production locks.
        private const float GroundAcceleration=60,AirAcceleration=35,GroundDeceleration=80,AirDeceleration=20;
        private readonly List<(string key,int slot,bool pressed,double time,long sequence,int held)> combatEvents=new List<(string,int,bool,double,long,int)>();
        private InputAction[] combatBindings;
        private long inputSequence;
        private Camera cameraView;
        private Vector3 cameraVelocity;
        private readonly Dictionary<string,TextMesh> npcMarkers=new Dictionary<string,TextMesh>();
        private TextMesh speech;private double speechUntil;
        private GameObject world,playerVisual;
        private BoxCollider2D playerCollider;
        private class MobPresenter {
            public GameObject Root;
            public Transform VisualRoot;
            public readonly List<(SpriteRenderer sr, Color baseColor)> Parts=new List<(SpriteRenderer, Color)>();
            public Transform[] Legs;
            public bool IsDummy;
            public bool IsWolf;
        }
        private readonly Dictionary<int,MobPresenter> mobViews=new Dictionary<int,MobPresenter>();
        private readonly Dictionary<long,SpriteRenderer> lootViews=new Dictionary<long,SpriteRenderer>();
        private readonly List<BoxCollider2D> platforms=new List<BoxCollider2D>();
        private readonly Dictionary<BoxCollider2D,float> ignored=new Dictionary<BoxCollider2D,float>();
        private Map renderedMap;
        private int renderedRevision;
        private Sprite square;
        public string LastDialogue {get;private set;}

        private readonly List<BoxCollider2D> tempColliderKeys=new List<BoxCollider2D>();
        private SpriteRenderer marker,playerBox;
        private int feedbackResultCount;
        private readonly Dictionary<int,double> flashUntil=new Dictionary<int,double>();
        private readonly List<(TextMesh mesh,float expiry)> floating=new List<(TextMesh,float)>();
        private readonly RaycastHit2D[] contacts=new RaycastHit2D[8];
        public const float RunSpeed=5,JumpSpeed=12;
        public static string MapName(Map map) => map==Map.Village?"Vân Khê":map==Map.Academy?"Học Viện":"Đồng Sương";
        private void Awake() {
            QualitySettings.vSyncCount=0;Application.targetFrameRate=60;Time.fixedDeltaTime=.02f;Application.SetStackTraceLogType(LogType.Log,StackTraceLogType.None);
            Session=new SliceSession();BindSession();
            combatBindings=new InputAction[3];
            var keys=new[]{"1","2","3"};
            for(int i=0;i<keys.Length;i++){
                int slot=i+1;string key=keys[i];
                var binding=new InputAction("Combat "+key,InputActionType.Button,"<Keyboard>/"+keys[i]);
                binding.started+=ctx=>combatEvents.Add((key,slot,true,ctx.time,++inputSequence,KeyboardMask()));
                binding.canceled+=ctx=>combatEvents.Add((key,slot,false,ctx.time,++inputSequence,KeyboardMask()));
                binding.Enable();combatBindings[i]=binding;
            }
            cycleBinding=new InputAction("CycleTarget",InputActionType.Button,InputBindings.CycleTarget);
            cycleBinding.started+=ctx=>cycleRequest=Keyboard.current!=null&&Keyboard.current.shiftKey.isPressed?-1:1;
            cycleBinding.Enable();
            var texture=new Texture2D(2,2);texture.SetPixels(new[]{Color.white,Color.white,Color.white,Color.white});texture.Apply();
            square=Sprite.Create(texture,new Rect(0,0,2,2),new Vector2(.5f,.5f),2);
            cameraView=new GameObject("Camera").AddComponent<Camera>();cameraView.tag="MainCamera";cameraView.orthographic=true;cameraView.clearFlags=CameraClearFlags.SolidColor;cameraView.orthographicSize=6.5f;cameraView.backgroundColor=new Color(.055f,.09f,.13f);
            cameraView.transform.position=new Vector3(9,3,-10);
            playerVisual=new GameObject("PlayerPhysics");Body=playerVisual.AddComponent<Rigidbody2D>();Body.gravityScale=2;Body.freezeRotation=true;Body.interpolation=RigidbodyInterpolation2D.Interpolate;Body.collisionDetectionMode=CollisionDetectionMode2D.Continuous;
            playerBox=RectVisual("Player",playerVisual.transform,Vector2.zero,new Vector2(.55f,1.4f),new Color(.3f,.8f,.55f),10);
            playerVisual.layer=7;playerCollider=playerVisual.AddComponent<BoxCollider2D>();playerCollider.size=new Vector2(.55f,1.4f);
            var mat=new PhysicsMaterial2D("NoFriction"){friction=0,bounciness=0};playerCollider.sharedMaterial=mat;
            marker=RectVisual("CombatFocus",null,Vector2.zero,new Vector2(.9f,.08f),Color.yellow,12);
            Hud=gameObject.AddComponent<SliceHud>();Hud.Host=this;
            BuildMap();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (System.Environment.GetCommandLineArgs().Contains("--verify-route")) gameObject.AddComponent<SliceRouteProbe>();
#endif
        }
        private readonly List<Sprite> terrainSprites=new List<Sprite>();
        private SpriteRenderer RectVisual(string name,Transform parent,Vector2 position,Vector2 size,Color color,int order=0) {
            var go=new GameObject(name);if(parent!=null)go.transform.SetParent(parent,false);go.transform.localPosition=position;
            go.transform.localScale=size;var sr=go.AddComponent<SpriteRenderer>();sr.sprite=square;sr.color=color;sr.sortingOrder=order;return sr;
        }
        private void Ground(string name,float x,float y,float width,float height,bool oneWay=false,bool wood=false,bool rearEarth=false,float rise=0,bool stone=false) {
            Color col=wood||(oneWay&&!rearEarth)?new Color(.55f,.39f,.23f):stone?new Color(.35f,.40f,.44f):new Color(.44f,.38f,.27f);
            var sr=RectVisual(name,world.transform,new Vector2(x,y),new Vector2(width,height),col);
            if(rearEarth)sr.sortingOrder=-12;else if(oneWay||wood)sr.sortingOrder=6;else if(stone)sr.sortingOrder=0;
            sr.gameObject.layer=6;var box=sr.gameObject.AddComponent<BoxCollider2D>();
            // Full rear soil is visual; only a thin top participates in collision.
            if(rearEarth){box.size=new Vector2(1,.16f/height);box.offset=new Vector2(0,.5f-.08f/height);}
            if(rise!=0){
                box.enabled=false;
                var vertices=new[]{new Vector2(-.5f,-.5f),new Vector2(.5f,-.5f),new Vector2(.5f,.5f+Mathf.Min(0,rise)/height),new Vector2(-.5f,.5f-Mathf.Max(0,rise)/height)};
                var polygon=sr.gameObject.AddComponent<PolygonCollider2D>();polygon.points=vertices;
                var shape=Sprite.Create(square.texture,square.rect,new Vector2(.5f,.5f),square.pixelsPerUnit);shape.OverrideGeometry(vertices.Select(v=>new Vector2((v.x+.5f)*square.rect.width,(v.y+.5f)*square.rect.height)).ToArray(),new ushort[]{0,1,2,0,2,3});sr.sprite=shape;terrainSprites.Add(shape);
            }
            if(oneWay){var eff=sr.gameObject.AddComponent<PlatformEffector2D>();eff.useOneWay=true;eff.useOneWayGrouping=true;eff.surfaceArc=160;box.usedByEffector=true;platforms.Add(box);}
        }
        private TextMesh Label(string text,Vector3 position,Transform parent=null,float scale=.12f) {
            var go=new GameObject("Label "+text);if(parent!=null)go.transform.SetParent(parent);go.transform.position=position;
            var mesh=go.AddComponent<TextMesh>();mesh.font=Resources.Load<Font>("Fonts/DejaVuSans");go.GetComponent<MeshRenderer>().sharedMaterial=mesh.font.material;mesh.text=text;mesh.fontSize=32;mesh.characterSize=scale;mesh.anchor=TextAnchor.MiddleCenter;mesh.color=Color.white;go.GetComponent<MeshRenderer>().sortingOrder=20;return mesh;
        }
        private void BuildMap() {
            foreach(var sprite in terrainSprites)Destroy(sprite);terrainSprites.Clear();
            foreach(var v in floating)if(v.mesh!=null)Destroy(v.mesh.gameObject);floating.Clear();
            if(world!=null){world.SetActive(false);Destroy(world);}world=new GameObject("Map "+Session.Player.Map);mobViews.Clear();lootViews.Clear();platforms.Clear();ignored.Clear();renderedMap=Session.Player.Map;renderedRevision=Session.WorldRevision;
            npcMarkers.Clear();speech=null;
            float min=(float)BlockoutLayout.MinX(renderedMap),max=(float)BlockoutLayout.MaxX(renderedMap);
            foreach(var surface in BlockoutLayout.Surfaces(renderedMap))
                Ground(surface.Name,(float)surface.X,(float)surface.Y,(float)surface.Width,(float)surface.Height,surface.OneWay,surface.Wood,surface.RearEarth,(float)surface.Rise,surface.Stone);
            Ground("LeftBoundary",min,5,1,14);Ground("RightBoundary",max,5,1,14);
            foreach(var a in SliceSession.Anchors.Where(x=>x.Map==renderedMap))RenderNpc(a);
            foreach(var m in Session.Mobs.Where(x=>x.Map==renderedMap))SpawnMob(m);
            var spawn=new Vector2((float)Session.Player.Position.X,(float)Session.Player.Position.Y);
            spawn.y=Mathf.Max(spawn.y,.72f);playerVisual.transform.position=spawn;Body.position=spawn;Body.linearVelocity=Vector2.zero;Physics2D.SyncTransforms();lastGroundAt=jumpBufferedUntil=-100;cameraVelocity=Vector3.zero;cameraView.transform.position=CameraTarget();
        }
        private static int KeyboardMask(){var k=Keyboard.current;if(k==null)return 0;return (k.aKey.isPressed?1:0)|(k.leftArrowKey.isPressed?2:0)|(k.dKey.isPressed?4:0)|(k.rightArrowKey.isPressed?8:0);}
        public void CombatPress(string key,int slot=0,int? atPress=null){
            if(Modal)return;
            heldMovement.Snapshot(atPress??(ExternalInput?(ExternalAxis<0?1:ExternalAxis>0?4:0):KeyboardMask()));
            Session.Combat.Press(key,slot);
        }
        public bool QuickKey(string key){if(Modal)return false;return InputBindings.IsHp(key)?Session.Potion(true):InputBindings.IsMp(key)&&Session.Potion(false);}
        public void EscapeWorld(){if(Modal){Hud.Back();return;}Session.Combat.Escape();heldMovement.Clear();}
        private int EffectiveManual()=>heldMovement.Axis(ExternalInput?(ExternalAxis<0?1:ExternalAxis>0?4:0):heldMask,Session.Combat.HasPendingCast||Session.Combat.HasBufferedCast);
        public void CombatRelease(string key)=>Session.Combat.Release(key);
        public void Interact() {
            if(Modal||!Session.Player.Alive)return;
            var loot=Session.LootCandidate();if(loot!=null){Session.PickUp(loot.Id);return;}
            var anchor=Session.NearAnchor();if(anchor==null){Session.Feedback="Không có NPC hoặc đồ trong tầm tương tác.";return;}
            if(Session.Interact(anchor.Id)){Hud.OpenNpc(anchor.Id);Speak(anchor.Id);}
        }
        private Vector3 CameraTarget(){
            var visual=playerVisual.transform.position;
            float half=cameraView.orthographicSize*cameraView.aspect;
            float min=(float)BlockoutLayout.MinX(renderedMap)+half,max=(float)BlockoutLayout.MaxX(renderedMap)-half;
            return new Vector3(Mathf.Clamp(visual.x,Mathf.Min(min,max),Mathf.Max(min,max)),Mathf.Max(2.4f,visual.y+1.6f),-10);
        }
        private void RenderNpc(Anchor a){
            var root=new GameObject(a.Name);root.transform.SetParent(world.transform,false);root.transform.localPosition=new Vector3((float)a.Position.X,(float)a.Position.Y,0);
            RectVisual(a.EdgeExit?"Map edge":"NPC",root.transform,Vector2.zero,a.EdgeExit?new Vector2(.2f,1.5f):new Vector2(.55f,1.4f),a.EdgeExit?Color.cyan:new Color(.9f,.7f,.3f),9);
            Label(a.EdgeExit?(a.Id.StartsWith("toVillage")?"← ":"→ ")+a.Name:a.Name,root.transform.position+Vector3.up*1.3f,root.transform,.05f);
            if(!a.EdgeExit)npcMarkers[a.Id]=Label("",root.transform.position+Vector3.up*1.8f,root.transform,.09f);
        }
        private void SpawnMob(Mob m){
            var root=new GameObject(m.Slot);root.transform.SetParent(world.transform,false);root.transform.localPosition=new Vector3((float)m.Position.X,(float)m.Position.Y,0);
            var color=m.Dummy?new Color(.75f,.6f,.4f):m.Name=="Nấm Linh"?new Color(.7f,.3f,.7f):new Color(.55f,.65f,.8f);
            var view=new MobPresenter{Root=root};view.Parts.Add((RectVisual(m.Name,root.transform,Vector2.zero,new Vector2(.7f,m.Dummy?1.2f:.8f),color,10),color));
            Label(m.Name+" Lv"+m.Level,root.transform.position+Vector3.up*.9f,root.transform,.045f);mobViews[m.Id]=view;
        }
        public void Speak(string id)=>SpeakText(id,Hud.Dialogue(id));
        public void SpeakText(string id,string text){
            LastDialogue=text;if(speech!=null)Destroy(speech.gameObject);var a=SliceSession.Anchors.First(x=>x.Id==id);
            var words=text.Split(' ');string wrapped="",line="";
            foreach(var word in words){if(line.Length+word.Length>32){wrapped+=line+"\n";line="";}line+=word+" ";}wrapped+=line;
            speech=Label(wrapped,new Vector3((float)a.Position.X,(float)a.Position.Y+3.3f,0),world.transform,.05f);speechUntil=Session.Now+7;
        }
        private void BindSession(){Session.OnEvent=x=>Debug.Log("[VS1] "+x);}
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        public void ResetPrototype(PrototypeStart start){
            Session.OnEvent=null;Session=PrototypePresets.Create(start);BindSession();heldMovement.Clear();heldMask=0;cycleRequest=0;
            combatEvents.Clear();feedbackResultCount=0;flashUntil.Clear();axis=ExternalAxis=0;
            ExternalJump=ExternalDrop=false;jumpBufferedUntil=lastGroundAt=-100;
            Hud.Close();BuildMap();Session.Emit(start==PrototypeStart.Fresh?"Phiên mới — bắt đầu Q1.":"DEBUG "+start+" — preset, không nghiệm thu route.");
        }
#endif
        public void HandleMenuKey(string key){
            if(key=="up")Hud.Navigate(-1);else if(key=="down")Hud.Navigate(1);else if(key=="tab")Hud.ChangeTab(1);else if(key=="left")Hud.Navigate(-1);else if(key=="right")Hud.Navigate(1);else if(key=="activate")Hud.ActivateSelected();else if(key=="escape")Hud.Back();
        }
        private void Update() {
            if(ExternalInput){combatEvents.Clear();return;}var k=Keyboard.current;if(k==null){combatEvents.Clear();return;}
            if(k.escapeKey.wasPressedThisFrame){EscapeWorld();axis=0;combatEvents.Clear();cycleRequest=0;return;}
            if(k.iKey.wasPressedThisFrame){Hud.Toggle("bag");axis=0;combatEvents.Clear();return;}
            if(k.cKey.wasPressedThisFrame){Hud.Toggle("equipment");axis=0;combatEvents.Clear();return;}
            if(k.qKey.wasPressedThisFrame){Hud.Toggle("quest");axis=0;combatEvents.Clear();return;}
            if(Modal){
                cycleRequest=0;heldMovement.Clear();axis=0;Session.Combat.CancelIntent();combatEvents.Clear();
                if(k.upArrowKey.wasPressedThisFrame||k.wKey.wasPressedThisFrame)Hud.NavigateGrid(0,-1);
                if(k.leftArrowKey.wasPressedThisFrame||k.aKey.wasPressedThisFrame)Hud.NavigateGrid(-1,0);
                if(k.rightArrowKey.wasPressedThisFrame||k.dKey.wasPressedThisFrame)Hud.NavigateGrid(1,0);
                if(k.downArrowKey.wasPressedThisFrame||k.sKey.wasPressedThisFrame)Hud.NavigateGrid(0,1);
                if(k.tabKey.wasPressedThisFrame)Hud.ChangeTab(k.shiftKey.isPressed?-1:1);
                if(k.enterKey.wasPressedThisFrame||k.numpadEnterKey.wasPressedThisFrame||k.eKey.wasPressedThisFrame){if(Hud.Panel=="chat")Hud.SubmitChat();else HandleMenuKey("activate");}
                return;
            }
            if(k.enterKey.wasPressedThisFrame||k.numpadEnterKey.wasPressedThisFrame){Hud.OpenChat();axis=0;combatEvents.Clear();cycleRequest=0;return;}
            axis=Rules.Axis(k.aKey.isPressed,k.leftArrowKey.isPressed,k.dKey.isPressed,k.rightArrowKey.isPressed);
            heldMask=KeyboardMask();
            bool newDirection=k.aKey.wasPressedThisFrame||k.leftArrowKey.wasPressedThisFrame||k.dKey.wasPressedThisFrame||k.rightArrowKey.wasPressedThisFrame;
            if(newDirection){Session.Combat.ManualOverride();heldMovement.Clear();}
            jumpHeld=k.spaceKey.isPressed||k.upArrowKey.isPressed;
            if(k.spaceKey.wasPressedThisFrame||k.upArrowKey.wasPressedThisFrame){ExternalJump=true;Session.Combat.ManualOverride();}
            if(k.sKey.wasPressedThisFrame||k.downArrowKey.wasPressedThisFrame){ExternalDrop=true;Session.Combat.ManualOverride();}
            foreach(var input in combatEvents.OrderBy(x=>x.time).ThenBy(x=>x.slot).ThenBy(x=>x.sequence)){
                if(input.pressed)CombatPress(input.key,input.slot,input.held);else CombatRelease(input.key);
            }
            combatEvents.Clear();
            if(cycleRequest!=0){Session.Combat.CycleTarget(cycleRequest);cycleRequest=0;heldMovement.Clear();}
            if(k.eKey.wasPressedThisFrame)Interact();if(k.fKey.wasPressedThisFrame)Session.UseFood();
            if(k.hKey.wasPressedThisFrame)QuickKey(InputBindings.HpMain);if(k.mKey.wasPressedThisFrame)QuickKey(InputBindings.MpMain);
            if(k.digit4Key.wasPressedThisFrame)QuickKey(InputBindings.HpAlias);if(k.digit5Key.wasPressedThisFrame)QuickKey(InputBindings.MpAlias);
            if(Mouse.current!=null&&Mouse.current.leftButton.wasPressedThisFrame&&!Hud.IsOverUi(Mouse.current.position.ReadValue())){
                var worldPos=cameraView.ScreenToWorldPoint(Mouse.current.position.ReadValue());var point=new Point(worldPos.x,worldPos.y);
                var target=Session.Mobs.Where(x=>x.Alive&&x.Map==renderedMap&&point.Distance(x.Position)<1)
                    .OrderBy(x=>point.Distance(x.Position)).ThenBy(x=>x.Id).FirstOrDefault();
                if(target!=null){Session.Combat.Explicit(target);heldMovement.Clear();}
                else {var loot=Session.LootCandidate();if(loot!=null&&point.Distance(loot.Position)<.8){Session.PickUp(loot.Id);return;}var anchor=Session.NearAnchor();if(anchor!=null&&point.Distance(anchor.Position)<1.2&&Session.Interact(anchor.Id)){Hud.OpenNpc(anchor.Id);Speak(anchor.Id);}}
            }
        }
        private bool IsGrounded() {
            if(Body.linearVelocity.y>.1f)return false;
            var filter=new ContactFilter2D();filter.SetLayerMask(1<<6);filter.useLayerMask=true;
            int n=playerCollider.Cast(Vector2.down,filter,contacts,.08f);
            for(int i=0;i<n;i++)if(contacts[i].normal.y>.5f && (!(contacts[i].collider is BoxCollider2D support)||!ignored.ContainsKey(support)))return true;return false;
        }
        private void FixedUpdate() {
            if(!Body.simulated)Body.simulated=true;
            if(Session.WorldRevision!=renderedRevision)BuildMap();
            if(ignored.Count>0){
                tempColliderKeys.Clear();
                foreach(var kv in ignored)if(kv.Key==null||Session.Now>=kv.Value||playerCollider.bounds.max.y<kv.Key.bounds.min.y-.02f)tempColliderKeys.Add(kv.Key);
                for(int i=0;i<tempColliderKeys.Count;i++){var p=tempColliderKeys[i];if(p!=null)Physics2D.IgnoreCollision(playerCollider,p,false);ignored.Remove(p);}
            }
            Grounded=IsGrounded();int manual=EffectiveManual();
            bool jump=ExternalJump,drop=ExternalDrop;ExternalJump=ExternalDrop=false;
            if(Modal||!Session.Player.Alive){manual=0;jump=drop=false;jumpBufferedUntil=lastGroundAt=-100;Session.Combat.CancelIntent();}
            if(jump||drop){Session.Combat.ManualOverride();heldMovement.Clear();}
            if(Grounded&&!Modal)lastGroundAt=Session.Now;
            if(jump)jumpBufferedUntil=Session.Now+.12;
            bool dropped=false,jumped=false;
            if(drop&&Grounded){foreach(var p in platforms){
                if(Math.Abs(Body.position.x-p.transform.position.x)<=p.bounds.extents.x+.3f&&Math.Abs(playerCollider.bounds.min.y-p.bounds.max.y)<.2f){
                    Physics2D.IgnoreCollision(playerCollider,p,true);ignored[p]=(float)(Session.Now+.65);dropped=true;}}
                if(dropped){Body.linearVelocity=new Vector2(Body.linearVelocity.x,-2);jumpBufferedUntil=lastGroundAt=-100;Session.Emit("Physics drop-through pair: "+Body.position);}
            }
            if(!dropped&&Session.Now<=jumpBufferedUntil&&Session.Now-lastGroundAt<=.10&&!Modal){Body.linearVelocity=new Vector2(Body.linearVelocity.x,JumpSpeed);jumpBufferedUntil=lastGroundAt=-100;jumped=true;}
            if(!(ExternalInput?ExternalJumpHeld:jumpHeld)&&Body.linearVelocity.y>4)Body.linearVelocity=new Vector2(Body.linearVelocity.x,4);
            if(Body.linearVelocity.y<0)Body.linearVelocity=new Vector2(Body.linearVelocity.x,Mathf.Max(-18,Body.linearVelocity.y+Physics2D.gravity.y*Body.gravityScale*.35f*Time.fixedDeltaTime));
            if(!Modal)Session.ObservePosition(new Point(Body.position.x,Body.position.y),Grounded,jumped,dropped,manual!=0);
            else Session.Player.Position=new Point(Body.position.x,Body.position.y);
            Session.Tick(Time.fixedDeltaTime,manual!=0||jump||drop);
            manual=EffectiveManual();
            int move=manual!=0?manual:Session.Combat.AssistAxis;
            if(manual==0&&move!=0&&Session.Combat.ApproachCrossesExit(Session.Player.Position,new Point(Session.Player.Position.X+move*RunSpeed*Time.fixedDeltaTime,Session.Player.Position.Y))){Session.Combat.RejectBlocked();move=0;}
            if(move!=0&&manual==0){
                var foot=new Vector2(Body.position.x+move*.6f,Body.position.y-.5f);
                if(!Grounded||Physics2D.Raycast(foot,Vector2.down,.5f,1<<6).collider==null||Physics2D.Raycast(Body.position,new Vector2(move,0),.65f,1<<6).collider!=null){move=0;Session.Combat.RejectBlocked();}
            }
            if(!Session.Player.Alive||Modal)move=0;
            float targetSpeed=move*RunSpeed*(float)Session.Player.Stats.Speed*(float)BlockoutLayout.WaterSpeed(renderedMap,new Point(Body.position.x,playerCollider.bounds.min.y));
            float acceleration=Grounded?(move==0?GroundDeceleration:GroundAcceleration):(move==0?AirDeceleration:AirAcceleration);
            Body.linearVelocity=new Vector2(Mathf.MoveTowards(Body.linearVelocity.x,targetSpeed,acceleration*Time.fixedDeltaTime),Body.linearVelocity.y);
            if(move!=0&&Session.Combat.Running==null)Session.Combat.Facing=move;
            if(Body.position.y<-8)Session.HurtPlayer(Session.Player.Stats.Hp);
        }
        private void LateUpdate() {
            if(Session.WorldRevision!=renderedRevision)BuildMap();
            if(!Session.Player.Alive&&Session.Player.Map==Map.Village&&Body.position.y<-8)Body.position=new Vector2((float)Session.Player.Position.X,.8f);
            cameraView.transform.position=Vector3.SmoothDamp(cameraView.transform.position,CameraTarget(),ref cameraVelocity,.045f,100,Time.deltaTime);
            playerBox.color=!Session.Player.Alive?Color.gray:Session.Now-Session.LastHurtAt<.12?Color.white:Session.Player.School==School.Novice?new Color(.3f,.8f,.55f):new Color(.3f,.6f,1);
            foreach(var pair in npcMarkers){pair.Value.text=Hud.Marker(pair.Key);pair.Value.color=pair.Value.text=="?"?Color.yellow:Color.white;}
            if(speech!=null&&Session.Now>speechUntil){Destroy(speech.gameObject);speech=null;}
            foreach(var m in Session.Mobs.Where(x=>x.Map==renderedMap)){
                if(!mobViews.TryGetValue(m.Id,out var view))continue;view.Root.SetActive(m.Alive);if(!m.Alive)continue;
                view.Root.transform.localPosition=Vector2.Lerp(new Vector2((float)m.PreviousPosition.X,(float)m.PreviousPosition.Y),new Vector2((float)m.Position.X,(float)m.Position.Y),Mathf.Clamp01((Time.time-Time.fixedTime)/Time.fixedDeltaTime));
                bool hurt=flashUntil.TryGetValue(m.Id,out var until)&&Session.Now<until;
                foreach(var part in view.Parts)part.sr.color=hurt?Color.white:m.Returning?Color.gray:m.Windup?new Color(1,.55f,.3f):part.baseColor;
            }
            foreach(var l in Session.Loot.Where(x=>x.Map==renderedMap&&!x.Claimed)){
                if(!lootViews.TryGetValue(l.Id,out var view)){view=RectVisual("Loot "+l.Id,world.transform,new Vector2((float)l.Position.X,(float)l.Position.Y-.3f),new Vector2(.25f,.25f),Color.cyan,6);lootViews[l.Id]=view;}
                view.color=Session.LootCandidate()?.Id==l.Id?Color.yellow:Color.cyan;
            }
            foreach(var pair in lootViews)if(Session.Loot.First(x=>x.Id==pair.Key).Claimed)pair.Value.enabled=false;
            var focus=Session.Combat.Focus;marker.enabled=focus!=null;if(focus!=null)marker.transform.position=new Vector3((float)focus.Position.X,(float)focus.Position.Y-.7f,0);
            for(;feedbackResultCount<Session.Combat.Results.Count;feedbackResultCount++){
                var result=Session.Combat.Results[feedbackResultCount];var mob=Session.Mobs.First(x=>x.Id==result.Target);if(mob.Map!=renderedMap||mob.Generation!=result.Generation)continue;
                flashUntil[mob.Id]=Session.Now+.12;var txt=Label(result.Evaded?"NÉ":result.Damage.ToString(),new Vector3((float)mob.Position.X,(float)mob.Position.Y+1.1f,0));txt.color=result.Crit?Color.yellow:Color.white;floating.Add((txt,Time.time+1));
            }
            for(int i=floating.Count-1;i>=0;i--)if(Time.time>=floating[i].expiry){Destroy(floating[i].mesh.gameObject);floating.RemoveAt(i);}else floating[i].mesh.transform.Translate(0,Time.deltaTime*.4f,0);
        }
        public Vector2 ScreenPoint(Point point)=>cameraView.WorldToScreenPoint(new Vector3((float)point.X,(float)point.Y,0));
        private void OnDestroy(){cycleBinding?.Dispose();foreach(var sprite in terrainSprites)Destroy(sprite);if(combatBindings!=null)foreach(var binding in combatBindings)binding.Dispose();if(world!=null)Destroy(world);if(playerVisual!=null)Destroy(playerVisual);if(cameraView!=null)Destroy(cameraView.gameObject);if(marker!=null)Destroy(marker.gameObject);foreach(var v in floating)if(v.mesh!=null)Destroy(v.mesh.gameObject);}
    }
}
