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
        private SpriteRenderer marker;
        private readonly Dictionary<string,SpriteRenderer> playerParts=new Dictionary<string,SpriteRenderer>();
        private int feedbackResultCount;
        private readonly Dictionary<int,double> flashUntil=new Dictionary<int,double>();
        private readonly Stack<TextMesh> damagePool=new Stack<TextMesh>();
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
            playerVisual.layer=7;playerCollider=playerVisual.AddComponent<BoxCollider2D>();playerCollider.size=new Vector2(.55f,1.4f);
            var mat=new PhysicsMaterial2D("NoFriction"){friction=0,bounciness=0};playerCollider.sharedMaterial=mat;
            marker=RectVisual("CombatFocus",null,Vector2.zero,new Vector2(.9f,.08f),Color.yellow,12);
            Hud=gameObject.AddComponent<SliceHud>();Hud.Host=this;
            BuildMap();
            if (System.Environment.GetCommandLineArgs().Contains("--verify-route")) gameObject.AddComponent<SliceRouteProbe>();
        }
        private readonly List<Sprite> terrainSprites=new List<Sprite>();
        private SpriteRenderer RectVisual(string name,Transform parent,Vector2 position,Vector2 size,Color color,int order=0) {
            var go=new GameObject(name);if(parent!=null)go.transform.SetParent(parent,false);go.transform.localPosition=position;
            go.transform.localScale=size;var sr=go.AddComponent<SpriteRenderer>();sr.sprite=square;sr.color=color;sr.sortingOrder=order;return sr;
        }
        private void Ground(string name,float x,float y,float width,float height,bool oneWay=false,bool wood=false,bool rearEarth=false,float rise=0,bool stone=false) {
            float top=height/2;
            float worldTop=y+top;
            float worldBottom=y-top;
            bool submerged=false;
            foreach(var w in BlockoutLayout.Waters(renderedMap)){
                if(worldTop<=(float)w.Level&&x>=(float)w.Left&&x<=(float)w.Right){submerged=true;break;}
            }

            if(rearEarth){
                // Background layered hills: one-way traversable mountain ridges in cross-section
                var sr=RectVisual(name,world.transform,new Vector2(x,y),new Vector2(width,height),new Color(.30f,.25f,.18f),-12);
                sr.gameObject.layer=6;
                var box=sr.gameObject.AddComponent<BoxCollider2D>();
                box.size=new Vector2(1,.16f/height);
                box.offset=new Vector2(0,.5f-.08f/height);
                var eff=sr.gameObject.AddComponent<PlatformEffector2D>();
                eff.useOneWay=true;eff.useOneWayGrouping=true;eff.surfaceArc=160;
                box.usedByEffector=true;platforms.Add(box);

                // Top surface: lush green grass for mountain ridges, seamless without vertical borders
                if(!submerged){
                    RectVisual(name+" Turf root",world.transform,new Vector2(x,worldTop-.065f),new Vector2(width,.06f),new Color(.24f,.42f,.16f),-11);
                    RectVisual(name+" Grass blades",world.transform,new Vector2(x,worldTop-.02f),new Vector2(width,.04f),new Color(.38f,.62f,.23f),-10);
                }
                return;
            }

            if(wood||(oneWay&&!rearEarth)){
                // Wooden decks, bridges, upper floors
                var sr=RectVisual(name,world.transform,new Vector2(x,y),new Vector2(width,height),new Color(.55f,.39f,.23f),6);
                sr.gameObject.layer=6;
                var box=sr.gameObject.AddComponent<BoxCollider2D>();
                if(oneWay){
                    var eff=sr.gameObject.AddComponent<PlatformEffector2D>();
                    eff.useOneWay=true;eff.useOneWayGrouping=true;eff.surfaceArc=160;
                    box.usedByEffector=true;platforms.Add(box);
                }
                RectVisual(name+" Timber deck",world.transform,new Vector2(x,worldTop-.035f),new Vector2(width,.07f),new Color(.8f,.59f,.3f),7);
                for(float offset=-width/2+.5f;offset<width/2;offset+=1){
                    RectVisual(name+" Plank seam",world.transform,new Vector2(x+offset,y),new Vector2(.025f,height),new Color(.3f,.2f,.12f),7);
                }
                return;
            }

            // Foreground solid ground: Geological stratification (bedrock below, earth above) + Architectural masonry
            // 1. Bedrock stratum underneath across the lower terrain (dãy ngang dưới là đá):
            float bedrockTop=Mathf.Min(worldTop-0.7f,worldTop<0?worldTop-0.7f:-0.4f);
            var srBase=RectVisual(name,world.transform,new Vector2(x,y),new Vector2(width,height),new Color(.36f,.38f,.38f),0);
            srBase.gameObject.layer=6;
            var baseBox=srBase.gameObject.AddComponent<BoxCollider2D>();

            // 2. Earth / soil layer rising above bedrock (rồi đất lên):
            if(worldTop>bedrockTop){
                float earthH=worldTop-bedrockTop;
                float earthY=bedrockTop+earthH/2;
                Color earthCol=submerged?new Color(.30f,.25f,.18f):new Color(.44f,.36f,.25f);
                RectVisual(name+" Earth layer",world.transform,new Vector2(x,earthY),new Vector2(width,earthH),earthCol,0);
            }

            // 3. Surface cap:
            if(submerged){
                // Submerged under water: clean riverbed/pond bed, NO lines, NO sediment strips!
            }else if(stone){
                // Architectural stone masonry on top (xây bằng đá ở trên như công trình: bậc thềm, sân tập võ, lối đi, tế đàn)
                RectVisual(name+" Stone paving",world.transform,new Vector2(x,worldTop-.14f),new Vector2(width,.28f),new Color(.60f,.62f,.60f),1);
                RectVisual(name+" Stone edge",world.transform,new Vector2(x,worldTop-.025f),new Vector2(width,.05f),new Color(.72f,.74f,.72f),2);
                for(float offset=-width/2+1.5f;offset<width/2;offset+=1.5f){
                    RectVisual(name+" Stone joint",world.transform,new Vector2(x+offset,worldTop-.14f),new Vector2(.03f,.28f),new Color(.45f,.47f,.45f),2);
                }
            }else{
                // Natural outdoor earth: lush green grass on top (mặt trên có cỏ xanh)
                RectVisual(name+" Turf root bed",world.transform,new Vector2(x,worldTop-.065f),new Vector2(width,.06f),new Color(.25f,.46f,.17f),1);
                RectVisual(name+" Grass blades",world.transform,new Vector2(x,worldTop-.02f),new Vector2(width,.04f),new Color(.40f,.68f,.25f),2);
            }
        }
        private void DecorativeStructures() {
            if(renderedMap==Map.Academy){
                // Bow hall pavilion behind Diệp Lam (x = -12, upper deck at y = 9.075):
                float bowBase=6.4f,bowRoof=11.2f,bowH=bowRoof-bowBase;
                RectVisual("Bow hall wall",world.transform,new Vector2(-12,bowBase+bowH/2),new Vector2(6.8f,bowH),new Color(.34f,.30f,.24f),-8);
                RectVisual("Bow hall left pillar",world.transform,new Vector2(-14.7f,bowBase+bowH/2),new Vector2(.24f,bowH),new Color(.44f,.30f,.18f),-4);
                RectVisual("Bow hall right pillar",world.transform,new Vector2(-9.3f,bowBase+bowH/2),new Vector2(.24f,bowH),new Color(.44f,.30f,.18f),-4);
                RectVisual("Bow hall upper beam",world.transform,new Vector2(-12,8.92f),new Vector2(6.6f,.22f),new Color(.38f,.26f,.16f),-3);
                RectVisual("Bow hall roof eave",world.transform,new Vector2(-12,bowRoof+.2f),new Vector2(7.8f,.42f),new Color(.24f,.35f,.38f),-3);
                RectVisual("Bow hall roof ridge",world.transform,new Vector2(-12,bowRoof+.48f),new Vector2(5.8f,.16f),new Color(.36f,.46f,.48f),-2);
                Label("Cung đường",new Vector3(-12,bowRoof+.82f,0),world.transform,.06f);

                // West dojo pavilion (x = -25, upper deck at y = 6.1):
                float dojoBase=4.0f,dojoRoof=8.2f,dojoH=dojoRoof-dojoBase;
                RectVisual("West dojo wall",world.transform,new Vector2(-25,dojoBase+dojoH/2),new Vector2(5.8f,dojoH),new Color(.34f,.30f,.24f),-8);
                RectVisual("West dojo left pillar",world.transform,new Vector2(-27.2f,dojoBase+dojoH/2),new Vector2(.22f,dojoH),new Color(.44f,.30f,.18f),-4);
                RectVisual("West dojo right pillar",world.transform,new Vector2(-22.8f,dojoBase+dojoH/2),new Vector2(.22f,dojoH),new Color(.44f,.30f,.18f),-4);
                RectVisual("West dojo upper beam",world.transform,new Vector2(-25,5.95f),new Vector2(5.6f,.2f),new Color(.38f,.26f,.16f),-3);
                RectVisual("West dojo roof eave",world.transform,new Vector2(-25,dojoRoof+.2f),new Vector2(6.8f,.42f),new Color(.24f,.35f,.38f),-3);
                RectVisual("West dojo roof ridge",world.transform,new Vector2(-25,dojoRoof+.48f),new Vector2(4.8f,.14f),new Color(.36f,.46f,.48f),-2);
                Label("Võ đường phía tây",new Vector3(-25,dojoRoof+.82f,0),world.transform,.06f);

                // Jump training platform (x = 8, deck at y = 2.8, ground at y = 0): sturdy timber scaffold structure
                RectVisual("Jump ledge timber stilt left",world.transform,new Vector2(6.6f,1.35f),new Vector2(.20f,2.7f),new Color(.42f,.29f,.18f),5);
                RectVisual("Jump ledge timber stilt right",world.transform,new Vector2(9.4f,1.35f),new Vector2(.20f,2.7f),new Color(.42f,.29f,.18f),5);
                RectVisual("Jump ledge upper beam",world.transform,new Vector2(8f,2.65f),new Vector2(3.6f,.16f),new Color(.35f,.23f,.14f),5);
                RectVisual("Jump ledge mid brace",world.transform,new Vector2(8f,1.35f),new Vector2(2.8f,.12f),new Color(.42f,.29f,.18f),5);
            }else if(renderedMap==Map.Village){
                // Elder house pavilion (x = -2, upper deck at y = 2.875):
                float elderBase=0,elderRoof=5.2f,elderH=elderRoof-elderBase;
                RectVisual("Elder house wall",world.transform,new Vector2(-2,elderBase+elderH/2),new Vector2(5.8f,elderH),new Color(.36f,.32f,.25f),-8);
                RectVisual("Elder house left pillar",world.transform,new Vector2(-4.2f,elderBase+elderH/2),new Vector2(.22f,elderH),new Color(.44f,.30f,.18f),-4);
                RectVisual("Elder house right pillar",world.transform,new Vector2(0.2f,elderBase+elderH/2),new Vector2(.22f,elderH),new Color(.44f,.30f,.18f),-4);
                RectVisual("Elder house upper beam",world.transform,new Vector2(-2,2.72f),new Vector2(5.6f,.2f),new Color(.38f,.26f,.16f),-3);
                RectVisual("Elder house roof eave",world.transform,new Vector2(-2,elderRoof+.2f),new Vector2(6.8f,.42f),new Color(.24f,.35f,.38f),-3);
                RectVisual("Elder house roof ridge",world.transform,new Vector2(-2,elderRoof+.48f),new Vector2(4.8f,.14f),new Color(.36f,.46f,.48f),-2);
                Label("Nhà trưởng lão",new Vector3(-2,elderRoof+.82f,0),world.transform,.06f);

                // Wooden forge workshop (x = 13, upper deck at y = 3):
                float forgeBase=0.6f,forgeRoof=5.6f,forgeH=forgeRoof-forgeBase;
                RectVisual("Forge wall",world.transform,new Vector2(13,forgeBase+forgeH/2),new Vector2(5.8f,forgeH),new Color(.36f,.32f,.25f),-8);
                RectVisual("Forge left pillar",world.transform,new Vector2(10.8f,forgeBase+forgeH/2),new Vector2(.22f,forgeH),new Color(.44f,.30f,.18f),-4);
                RectVisual("Forge right pillar",world.transform,new Vector2(15.2f,forgeBase+forgeH/2),new Vector2(.22f,forgeH),new Color(.44f,.30f,.18f),-4);
                RectVisual("Forge upper beam",world.transform,new Vector2(13,2.85f),new Vector2(5.6f,.2f),new Color(.38f,.26f,.16f),-3);
                RectVisual("Forge roof eave",world.transform,new Vector2(13,forgeRoof+.2f),new Vector2(6.8f,.42f),new Color(.24f,.35f,.38f),-3);
                RectVisual("Forge roof ridge",world.transform,new Vector2(13,forgeRoof+.48f),new Vector2(4.8f,.14f),new Color(.36f,.46f,.48f),-2);
                Label("Lò rèn",new Vector3(13,forgeRoof+.82f,0),world.transform,.06f);
            }else if(renderedMap==Map.Mist){
                // PROBE8 climbing timber step (x = 107, deck at y = 2.4, ground at y = 0.8): sturdy mountain lookout stilt
                RectVisual("Climbing step stilt left",world.transform,new Vector2(106.3f,1.55f),new Vector2(.16f,1.5f),new Color(.42f,.29f,.18f),5);
                RectVisual("Climbing step stilt right",world.transform,new Vector2(107.7f,1.55f),new Vector2(.16f,1.5f),new Color(.42f,.29f,.18f),5);
                RectVisual("Climbing step upper beam",world.transform,new Vector2(107f,2.28f),new Vector2(1.8f,.14f),new Color(.35f,.23f,.14f),5);

                // Valley wooden bridge piers:
                foreach(float px in new[]{69f,74f,79f}){
                    RectVisual("Bridge pier",world.transform,new Vector2(px,-.2f),new Vector2(.35f,2.2f),new Color(.35f,.25f,.17f),2);
                }
            }
        }
        private TextMesh Label(string text,Vector3 position,Transform parent=null,float scale=.12f) {
            var go=new GameObject("Label "+text);if(parent!=null)go.transform.SetParent(parent);go.transform.position=position;
            var mesh=go.AddComponent<TextMesh>();mesh.font=Resources.Load<Font>("Fonts/DejaVuSans");go.GetComponent<MeshRenderer>().sharedMaterial=mesh.font.material;mesh.text=text;mesh.fontSize=32;mesh.characterSize=scale;mesh.anchor=TextAnchor.MiddleCenter;mesh.color=Color.white;go.GetComponent<MeshRenderer>().sortingOrder=20;return mesh;
        }
        private void BuildMap() {
            foreach(var sprite in terrainSprites)Destroy(sprite);terrainSprites.Clear();
            foreach(var v in floating)if(v.mesh!=null){v.mesh.gameObject.SetActive(false);damagePool.Push(v.mesh);}floating.Clear();
            if(world!=null){world.SetActive(false);Destroy(world);}world=new GameObject("Map "+Session.Player.Map);mobViews.Clear();lootViews.Clear();platforms.Clear();ignored.Clear();renderedMap=Session.Player.Map;renderedRevision=Session.WorldRevision;
            npcMarkers.Clear();speech=null;
            float min=(float)BlockoutLayout.MinX(renderedMap),max=(float)BlockoutLayout.MaxX(renderedMap);
            foreach(var surface in BlockoutLayout.Surfaces(renderedMap))
                Ground(surface.Name,(float)surface.X,(float)surface.Y,(float)surface.Width,(float)surface.Height,surface.OneWay,surface.Wood,surface.RearEarth,(float)surface.Rise,surface.Stone);
            Ground("LeftBoundary",min,5,1,14);Ground("RightBoundary",max,5,1,14);
            DecorativeStructures();
            foreach(var water in BlockoutLayout.Waters(renderedMap)){
                // Village is a real shallow basin; inaccessible bridge water uses a higher scenic surface.
                float left=(float)water.Left,right=(float)water.Right,level=(float)water.Level;
                float bottom=renderedMap==Map.Mist?-8:(float)water.Bottom;
                if(renderedMap==Map.Mist)level=.75f;
                RectVisual("Shallow water · no collider",world.transform,new Vector2((left+right)/2,(level+bottom)/2),new Vector2(right-left,level-bottom),new Color(.18f,.43f,.58f,.85f),renderedMap==Map.Village?12:3);
                RectVisual("Water surface",world.transform,new Vector2((left+right)/2,level),new Vector2(right-left,.045f),new Color(.46f,.77f,.85f),renderedMap==Map.Village?13:4);
            }
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
            var root=new GameObject(a.Name);root.transform.SetParent(world.transform,false);
            float ground=(float)BlockoutLayout.GroundTop(renderedMap,a.Position.X);
            root.transform.localPosition=new Vector3((float)a.Position.X,ground+.7f,0);
            if(a.EdgeExit)RectVisual("Map edge",root.transform,Vector2.zero,new Vector2(.2f,1.4f),new Color(.85f,.6f,.18f),9);
            else {
                RectVisual("NPC robe",root.transform,new Vector2(0,-.15f),new Vector2(.5f,1.1f),new Color(.7f,.49f,.25f),9);
                RectVisual("NPC head",root.transform,new Vector2(0,.55f),new Vector2(.36f,.36f),new Color(.9f,.74f,.53f),10);
                RectVisual("NPC hair",root.transform,new Vector2(0,.75f),new Vector2(.4f,.12f),new Color(.2f,.22f,.23f),11);
                RectVisual("NPC belt",root.transform,new Vector2(0,-.1f),new Vector2(.54f,.08f),new Color(.3f,.24f,.2f),10);
            }
            Label(a.EdgeExit?(a.Id.StartsWith("toVillage")?"← ":"→ ")+a.Name:a.Name,root.transform.position+Vector3.up*1.3f,root.transform,.05f);
            if(!a.EdgeExit)npcMarkers[a.Id]=Label("",root.transform.position+Vector3.up*1.8f,root.transform,.09f);
        }
        private void SpawnMob(Mob m){
            var root=new GameObject(m.Slot);root.transform.SetParent(world.transform,false);root.transform.localPosition=new Vector3((float)m.Position.X,(float)m.Position.Y,0);
            var visual=new GameObject("Silhouette");visual.transform.SetParent(root.transform,false);
            var view=new MobPresenter{Root=root,VisualRoot=visual.transform,IsDummy=m.Dummy,IsWolf=!m.Dummy&&m.Name!="Nấm Linh"};
            void Part(string name,Vector2 at,Vector2 size,Color color,int layer=10){view.Parts.Add((RectVisual(name,visual.transform,at,size,color,layer),color));}
            if(m.Dummy){
                Part("Dummy post",new Vector2(0,-.25f),new Vector2(.18f,1.1f),new Color(.5f,.34f,.2f));
                Part("Dummy arms",new Vector2(0,.22f),new Vector2(.95f,.16f),new Color(.6f,.42f,.25f));
                Part("Dummy straw head",new Vector2(0,.48f),new Vector2(.42f,.4f),new Color(.8f,.68f,.38f),11);
            }else if(!view.IsWolf){
                Part("Mushroom stem",new Vector2(0,-.4f),new Vector2(.25f,.5f),new Color(.8f,.71f,.5f));
                Part("Mushroom cap",new Vector2(0,-.04f),new Vector2(.75f,.35f),new Color(.7f,.3f,.7f),11);
            }else {
                var fur=new Color(.55f,.65f,.8f);
                Part("Wolf torso",new Vector2(0,-.03f),new Vector2(.8f,.45f),fur);
                Part("Wolf chest",new Vector2(.27f,-.13f),new Vector2(.28f,.45f),fur*.85f,11);
                Part("Wolf muzzle",new Vector2(.48f,.12f),new Vector2(.4f,.25f),fur,12);
                Part("Wolf ear",new Vector2(.31f,.34f),new Vector2(.13f,.22f),fur*.7f,12);
                Part("Wolf tail",new Vector2(-.49f,.04f),new Vector2(.28f,.14f),fur*.75f);
                Part("Wolf eye",new Vector2(.4f,.18f),new Vector2(.055f,.045f),new Color(.08f,.1f,.15f),13);
                Part("Wolf rear leg",new Vector2(-.27f,-.45f),new Vector2(.14f,.4f),fur*.65f,11);
                Part("Wolf front leg",new Vector2(.27f,-.45f),new Vector2(.14f,.4f),fur*.75f,11);
            }
            Label(m.Name+" Lv"+m.Level,root.transform.position+Vector3.up*.9f,root.transform,.045f);mobViews[m.Id]=view;
        }
        private void RenderPlayer(){
            var action=Session.Combat.Running;
            float phase=action==null?0:(float)((Session.Now-action.Started)/(action.EndAt-action.Started));
            foreach(var sr in playerParts.Values)sr.enabled=false;
            foreach(var part in GeometricRig.Pose(Session.Player,Session.Combat.Facing,Session.Now,phase,action!=null,Body.linearVelocity.x)){
                if(!playerParts.TryGetValue(part.Name,out var sr)){sr=RectVisual(part.Name,playerVisual.transform,part.Center,part.Size,part.Color,part.Layer);playerParts.Add(part.Name,sr);}
                sr.enabled=true;sr.transform.localPosition=part.Center;sr.transform.localScale=part.Size;sr.transform.localRotation=Quaternion.Euler(0,0,part.Angle);sr.sortingOrder=part.Layer;
                sr.color=!Session.Player.Alive?Color.gray:Session.Now-Session.LastHurtAt<.12?Color.white:part.Color;
            }
        }
        public void Speak(string id)=>SpeakText(id,Hud.Dialogue(id));
        public void SpeakText(string id,string text){
            LastDialogue=text;if(speech!=null)Destroy(speech.gameObject);var a=SliceSession.Anchors.First(x=>x.Id==id);
            var words=text.Split(' ');string wrapped="",line="";
            foreach(var word in words){if(line.Length+word.Length>32){wrapped+=line+"\n";line="";}line+=word+" ";}wrapped+=line;
            speech=Label(wrapped,new Vector3((float)a.Position.X,(float)a.Position.Y+3.3f,0),world.transform,.05f);speechUntil=Session.Now+7;
        }
        private void BindSession(){Session.OnEvent=ExternalInput||System.Environment.GetCommandLineArgs().Contains("--verify-route")?x=>Debug.Log("[VS1] "+x):null;}
        public void ResetPrototype(PrototypeStart start){
            Session.OnEvent=null;Session=PrototypePresets.Create(start);BindSession();heldMovement.Clear();heldMask=0;cycleRequest=0;
            combatEvents.Clear();feedbackResultCount=0;flashUntil.Clear();axis=ExternalAxis=0;
            ExternalJump=ExternalDrop=false;jumpBufferedUntil=lastGroundAt=-100;
            Hud.Close();BuildMap();Session.Emit(start==PrototypeStart.Fresh?"Phiên mới — bắt đầu Q1.":"DEBUG "+start+" — preset, không nghiệm thu route.");
        }
        public void HandleMenuKey(string key){
            if(key=="up")Hud.Navigate(-1);else if(key=="down")Hud.Navigate(1);else if(key=="tab")Hud.ChangeTab(1);else if(key=="left")Hud.Navigate(-1);else if(key=="right")Hud.Navigate(1);else if(key=="activate")Hud.ActivateSelected();else if(key=="escape")Hud.Back();
        }
        private void Update() {
            if(ExternalInput){combatEvents.Clear();return;}var k=Keyboard.current;if(k==null){combatEvents.Clear();return;}
            if(k.f8Key.wasPressedThisFrame){Hud.Toggle("debug");axis=0;combatEvents.Clear();cycleRequest=0;return;}
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
            if(Hud.Panel=="debug"){Body.simulated=false;return;}
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
            RenderPlayer();
            foreach(var pair in npcMarkers){var text=Hud.Marker(pair.Key);if(pair.Value.text!=text)pair.Value.text=text;pair.Value.color=text=="?"?Color.yellow:Color.white;}
            if(speech!=null&&Session.Now>speechUntil){Destroy(speech.gameObject);speech=null;}
            foreach(var m in Session.Mobs.Where(x=>x.Map==renderedMap)){
                if(!mobViews.TryGetValue(m.Id,out var view))continue;view.Root.SetActive(m.Alive);if(!m.Alive)continue;
                view.Root.transform.localPosition=Vector2.Lerp(new Vector2((float)m.PreviousPosition.X,(float)m.PreviousPosition.Y),new Vector2((float)m.Position.X,(float)m.Position.Y),Mathf.Clamp01((Time.time-Time.fixedTime)/Time.fixedDeltaTime));
                view.VisualRoot.localScale=new Vector3(m.Facing,1,1);
                bool hurt=flashUntil.TryGetValue(m.Id,out var until)&&Session.Now<until;
                foreach(var part in view.Parts)part.sr.color=hurt?Color.white:m.Returning?Color.gray:m.Windup?new Color(1,.55f,.3f):part.baseColor;
            }
            var lootCandidate=Session.LootCandidate();
            foreach(var l in Session.Loot){
                if(l.Map!=renderedMap||l.Claimed)continue;
                if(!lootViews.TryGetValue(l.Id,out var view)){view=RectVisual("Loot "+l.Id,world.transform,new Vector2((float)l.Position.X,(float)l.Position.Y-.3f),new Vector2(.25f,.25f),Color.yellow,6);lootViews[l.Id]=view;}
                view.color=lootCandidate?.Id==l.Id?Color.white:Color.yellow;
            }
            // Retire collected/expired objects once, rather than searching the full history per view every frame.
            for(int i=Session.Loot.Count-1;i>=0;i--){var l=Session.Loot[i];if(!l.Claimed)continue;if(lootViews.TryGetValue(l.Id,out var view)){Destroy(view.gameObject);lootViews.Remove(l.Id);}Session.Loot.RemoveAt(i);}
            var focus=Session.Combat.Focus;marker.enabled=focus!=null;if(focus!=null)marker.transform.position=new Vector3((float)focus.Position.X,(float)focus.Position.Y-.7f,0);
            for(;feedbackResultCount<Session.Combat.Results.Count;feedbackResultCount++){
                var result=Session.Combat.Results[feedbackResultCount];var mob=Session.Mobs.First(x=>x.Id==result.Target);if(mob.Map!=renderedMap||mob.Generation!=result.Generation)continue;
                flashUntil[mob.Id]=Session.Now+.12;var txt=damagePool.Count>0?damagePool.Pop():Label("",Vector3.zero,transform);txt.transform.position=new Vector3((float)mob.Position.X,(float)mob.Position.Y+1.1f,0);txt.text=result.Evaded?"NÉ":result.Damage.ToString();txt.gameObject.SetActive(true);txt.color=result.Crit?Color.yellow:Color.white;floating.Add((txt,Time.time+1));
            }
            // Disposable UI histories are not durable logs. Keep long sessions bounded after consuming feedback.
            if(feedbackResultCount>256){Session.Combat.Results.Clear();feedbackResultCount=0;}
            if(Session.Events.Count>512)Session.Events.RemoveRange(0,Session.Events.Count-256);
            for(int i=floating.Count-1;i>=0;i--)if(Time.time>=floating[i].expiry){floating[i].mesh.gameObject.SetActive(false);damagePool.Push(floating[i].mesh);floating.RemoveAt(i);}else floating[i].mesh.transform.Translate(0,Time.deltaTime*.4f,0);
        }
        public Vector2 ScreenPoint(Point point)=>cameraView.WorldToScreenPoint(new Vector3((float)point.X,(float)point.Y,0));
        private void OnDestroy(){cycleBinding?.Dispose();foreach(var sprite in terrainSprites)Destroy(sprite);if(combatBindings!=null)foreach(var binding in combatBindings)binding.Dispose();if(world!=null)Destroy(world);if(playerVisual!=null)Destroy(playerVisual);if(cameraView!=null)Destroy(cameraView.gameObject);if(marker!=null)Destroy(marker.gameObject);foreach(var v in floating)if(v.mesh!=null)Destroy(v.mesh.gameObject);}
    }
}
