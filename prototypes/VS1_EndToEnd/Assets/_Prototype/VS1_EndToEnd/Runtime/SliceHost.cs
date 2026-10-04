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
        private readonly List<(string key,int slot,bool pressed,double time,long sequence)> combatEvents=new List<(string,int,bool,double,long)>();
        private InputAction[] combatBindings;
        private long inputSequence;
        private Camera cameraView;
        private Vector3 cameraVelocity;
        private readonly List<(Transform root,Vector3 home,float factor)> scenery=new List<(Transform,Vector3,float)>();
        private readonly Dictionary<string,TextMesh> npcMarkers=new Dictionary<string,TextMesh>();
        private TextMesh speech;private double speechUntil;
        private GameObject world,playerVisual;
        private BoxCollider2D playerCollider;
        private readonly Dictionary<int,SpriteRenderer> mobViews=new Dictionary<int,SpriteRenderer>();
        private readonly Dictionary<long,SpriteRenderer> lootViews=new Dictionary<long,SpriteRenderer>();
        private readonly List<BoxCollider2D> platforms=new List<BoxCollider2D>();
        private readonly Dictionary<BoxCollider2D,float> ignored=new Dictionary<BoxCollider2D,float>();
        private Map renderedMap;
        private int renderedRevision;
        private Sprite square;
        private readonly Dictionary<string,SpriteRenderer> rig=new Dictionary<string,SpriteRenderer>();
        private readonly List<(float left,float right,float top)> water=new List<(float,float,float)>();
        private double nextRipple;
        public string LastDialogue {get;private set;}

        private SpriteRenderer marker;
        private int feedbackResultCount;
        private readonly Dictionary<int,double> flashUntil=new Dictionary<int,double>();
        private readonly List<(TextMesh mesh,float expiry)> floating=new List<(TextMesh,float)>();
        private readonly RaycastHit2D[] contacts=new RaycastHit2D[8];
        public const float RunSpeed=5,JumpSpeed=12;
        public static string MapName(Map map) => map==Map.Village?"Vân Khê":map==Map.Academy?"Học Viện":"Đồng Sương";
        private void Awake() {
            Application.targetFrameRate=60;Time.fixedDeltaTime=.02f;Application.SetStackTraceLogType(LogType.Log,StackTraceLogType.None);
            Session=new SliceSession();BindSession();
            combatBindings=new InputAction[3];
            var keys=new[]{"1","2","3"};
            for(int i=0;i<keys.Length;i++){
                int slot=i+1;string key=keys[i];
                var binding=new InputAction("Combat "+key,InputActionType.Button,"<Keyboard>/"+keys[i]);
                binding.started+=ctx=>combatEvents.Add((key,slot,true,ctx.time,++inputSequence));
                binding.canceled+=ctx=>combatEvents.Add((key,slot,false,ctx.time,++inputSequence));
                binding.Enable();combatBindings[i]=binding;
            }
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
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (System.Environment.GetCommandLineArgs().Contains("--verify-route")) gameObject.AddComponent<SliceRouteProbe>();
#endif
        }
        private readonly List<(Transform sprite,float bottom,float top,float phase)> flowStreaks=new List<(Transform,float,float,float)>();
        private SpriteRenderer RectVisual(string name,Transform parent,Vector2 position,Vector2 size,Color color,int order=0) {
            var go=new GameObject(name);if(parent!=null)go.transform.SetParent(parent,false);go.transform.localPosition=position;
            go.transform.localScale=size;var sr=go.AddComponent<SpriteRenderer>();sr.sprite=square;sr.color=color;sr.sortingOrder=order;return sr;
        }
        private void Ground(string name,float x,float y,float width,float height,bool oneWay=false) {
            var sr=RectVisual(name,world.transform,new Vector2(x,y),new Vector2(width,height),oneWay?new Color(.55f,.39f,.23f):new Color(.44f,.38f,.27f));
            if(oneWay)sr.sortingOrder=6;
            sr.gameObject.layer=6;var box=sr.gameObject.AddComponent<BoxCollider2D>();
            if(oneWay){var eff=sr.gameObject.AddComponent<PlatformEffector2D>();eff.useOneWay=true;eff.useOneWayGrouping=true;eff.surfaceArc=160;box.usedByEffector=true;platforms.Add(box);}
        }
        private TextMesh Label(string text,Vector3 position,Transform parent=null,float scale=.12f) {
            var go=new GameObject("Label "+text);if(parent!=null)go.transform.SetParent(parent);go.transform.position=position;
            var mesh=go.AddComponent<TextMesh>();mesh.font=Resources.Load<Font>("Fonts/DejaVuSans");go.GetComponent<MeshRenderer>().sharedMaterial=mesh.font.material;mesh.text=text;mesh.fontSize=32;mesh.characterSize=scale;mesh.anchor=TextAnchor.MiddleCenter;mesh.color=Color.white;go.GetComponent<MeshRenderer>().sortingOrder=20;return mesh;
        }
        private void BuildMap() {
            foreach(var v in floating)if(v.mesh!=null)Destroy(v.mesh.gameObject);floating.Clear();
            if(world!=null){world.SetActive(false);Destroy(world);}world=new GameObject("Map "+Session.Player.Map);mobViews.Clear();lootViews.Clear();platforms.Clear();ignored.Clear();renderedMap=Session.Player.Map;renderedRevision=Session.WorldRevision;
            scenery.Clear();npcMarkers.Clear();water.Clear();flowStreaks.Clear();speech=null;
            float min=(float)BlockoutLayout.MinX(renderedMap),max=(float)BlockoutLayout.MaxX(renderedMap);
            BuildBackdrop(min,max);
            foreach(var surface in BlockoutLayout.Surfaces(renderedMap)){
                Ground(surface.Name,(float)surface.X,(float)surface.Y,(float)surface.Width,(float)surface.Height,surface.OneWay);
                bool riverbed=!surface.OneWay&&BlockoutLayout.Waters(renderedMap).Any(w=>surface.X>w.Left&&surface.X<w.Right&&surface.Y+surface.Height/2<w.Level);
                var cap=surface.OneWay?new Color(.72f,.53f,.31f):riverbed?new Color(.55f,.48f,.33f):new Color(.43f,.6f,.36f);
                RectVisual("Walkable cap",world.transform,new Vector2((float)surface.X,(float)(surface.Y+surface.Height/2)),new Vector2((float)surface.Width,.08f),cap,surface.OneWay?7:1);
                if(!surface.OneWay){
                    float top=(float)(surface.Y+surface.Height/2);
                    RectVisual("Soil upper band",world.transform,new Vector2((float)surface.X,top-.25f),new Vector2((float)surface.Width,.45f),new Color(.51f,.44f,.31f),0);
                    for(float px=(float)(surface.X-surface.Width/2)+.6f;px<surface.X+surface.Width/2;px+=1.8f)
                        for(float py=-6.5f;py<top-.5f;py+=1.4f)RectVisual("Soil grain",world.transform,new Vector2(px,py),new Vector2(.09f,.06f),new Color(.36f,.31f,.23f),1);
                }
                if(surface.OneWay)for(float peg=(float)(surface.X-surface.Width/2);peg<surface.X+surface.Width/2;peg+=.7f)RectVisual("Wood grain",world.transform,new Vector2(peg,(float)surface.Y),new Vector2(.035f,(float)surface.Height),new Color(.3f,.22f,.16f),7);
            }
            Ground("LeftBoundary",min,5,1,14);Ground("RightBoundary",max,5,1,14);
            if(renderedMap==Map.Academy){
                Label("CỔNG ĐÔNG → VÂN KHÊ",new Vector3(31,3,0),world.transform,.07f);
                Label("BẬC TẬP NHẢY\nS / ↓ để xuống",new Vector3(8,4.3f,0),world.transform,.07f);
                Label("SÂN BÙ NHÌN",new Vector3(23,2.6f,0),world.transform,.08f);
                Building("Đại sảnh Kiếm",0,7,5);Building("Cung đường",-12,6,4.5f);Building("Võ đường phía tây",-27,6,4);Building("Sân tập",24,9,3);
            }else if(renderedMap==Map.Village){
                Building("Nhà trưởng lão",0,5,5);Building("Nhà thuốc",6,3,3.5f);Building("Lò rèn",13,5,4.8f);Building("Quán nghỉ / rương",20,5,4);Building("Đài nhập môn",26,3,5);

                Label("← HỌC VIỆN     ·     ĐỒNG SƯƠNG →",new Vector3(24,5.6f,0),world.transform,.065f);
            }else {
                Label("DS1 · NẤM",new Vector3(5,2.4f,0),world.transform,.06f);Label("DS2 · NẤM SƯƠNG",new Vector3(15,3,0),world.transform,.06f);
                foreach(float x in new[]{30f,50f,59f,90f})Label("BÃI SÓI",new Vector3(x,(float)BlockoutLayout.GroundTop(renderedMap,x)+2.2f,0),world.transform,.06f);
                Label("ĐƯỜNG CAO · PROBE8",new Vector3(54,5.5f,0),world.transform,.06f);
                Label("BÃI PHỤ · PROBE7 · 4 SÓI",new Vector3(110,2.7f,0),world.transform,.06f);
                // Rear stream ends at the same edge as the fall; the fall meets the continuous basin.
                RectVisual("Rear stream source",world.transform,new Vector2(64.7f,3.5f),new Vector2(3.1f,.15f),new Color(.39f,.7f,.79f),2);
                Waterfall(66.325f,3.5f,-1.45f);
                RectVisual("Ancient standing stone",world.transform,new Vector2(99,2.1f),new Vector2(1.5f,4.2f),new Color(.28f,.36f,.35f),-1);
            }
            foreach(var region in BlockoutLayout.Waters(renderedMap))Puddle(region);
            foreach(var a in SliceSession.Anchors.Where(x=>x.Map==renderedMap)){
                if(!a.EdgeExit){
                    RectVisual(a.Id+" robe",world.transform,new Vector2((float)a.Position.X,(float)a.Position.Y-.15f),new Vector2(.55f,1.1f),a.Id=="Diep"?new Color(.34f,.58f,.72f):new Color(.56f,.57f,.35f),7);
                    RectVisual(a.Id+" head",world.transform,new Vector2((float)a.Position.X,(float)a.Position.Y+.55f),new Vector2(.35f,.38f),new Color(.8f,.67f,.5f),8);
                    npcMarkers[a.Id]=Label("",new Vector3((float)a.Position.X,(float)a.Position.Y+1.85f,0),world.transform,.17f);
                    Label(a.Name,new Vector3((float)a.Position.X,(float)a.Position.Y+1.3f,0),world.transform,.075f);
                }else{
                    RectVisual("Road sign "+a.Id,world.transform,new Vector2((float)a.Position.X,1.1f),new Vector2(.12f,2.2f),new Color(.45f,.38f,.28f),2);
                    Label(a.Name,new Vector3((float)a.Position.X,3.1f,0),world.transform,.08f);
                }
            }
            foreach(var m in Session.Mobs.Where(x=>x.Map==renderedMap)){
                var view=RectVisual(m.Slot,world.transform,new Vector2((float)m.Position.X,(float)m.Position.Y),m.Dummy?new Vector2(.6f,1.4f):new Vector2(.8f,.7f),m.Dummy?new Color(.65f,.5f,.25f):m.Level==2?new Color(.7f,.35f,.45f):new Color(.55f,.65f,.78f),5);mobViews[m.Id]=view;
                if(!m.Dummy&&m.Level==4){
                    RectVisual("Wolf head",view.transform,new Vector2(-.44f,.14f),new Vector2(.26f,.38f),new Color(.65f,.73f,.81f),6);
                    RectVisual("Wolf ear",view.transform,new Vector2(-.42f,.42f),new Vector2(.13f,.3f),new Color(.53f,.63f,.73f),6);
                    RectVisual("Wolf legs A",view.transform,new Vector2(-.3f,-.58f),new Vector2(.15f,.35f),new Color(.45f,.55f,.66f),5);
                    RectVisual("Wolf legs B",view.transform,new Vector2(.32f,-.58f),new Vector2(.15f,.35f),new Color(.45f,.55f,.66f),5);
                }
            }
            var spawn=new Vector2((float)Session.Player.Position.X,(float)Session.Player.Position.Y);
            spawn.y=Mathf.Max(spawn.y,.72f);playerVisual.transform.position=spawn;Body.position=spawn;Body.linearVelocity=Vector2.zero;Physics2D.SyncTransforms();lastGroundAt=jumpBufferedUntil=-100;cameraVelocity=Vector3.zero;cameraView.transform.position=CameraTarget();

        }
        public void CombatPress(string key,int slot=0){if(!Modal)Session.Combat.Press(key,slot);}
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
        // Authored rear terraces: earth rises behind the route, rather than floating blocks.
        // They are presentation only. Fixed world positions keep the stream/fall join aligned.
        private void BuildBackdrop(float min,float max){
            cameraView.backgroundColor=new Color(.16f,.23f,.26f);
            float[] far,mid;
            if(renderedMap==Map.Village){
                far=new[]{-10f,2,6.6f, 2,14,8, 14,25,6.8f, 25,40,8.8f};
                mid=new[]{-10f,-3,3.6f, -3,10,3, 10,18,4.1f, 18,30,2.7f, 30,40,4.7f};
            }else if(renderedMap==Map.Academy){
                far=new[]{-30f,-16,10, -16,-5,8, -5,14,7.2f, 14,40,6.4f};
                mid=new[]{-30f,-22,6.3f, -22,-16,4.8f, -16,-5,4.2f, -5,10,3.8f, 10,20,3, 20,40,3};
            }else{
                far=new[]{-10f,18,6.5f, 18,36,8.3f, 36,56,7, 56,66,7.8f, 66,88,6, 88,104,8.1f, 104,132,9};
                mid=new[]{-10f,18,3, 18,36,4.8f, 36,56,3.6f, 56,66,3.5f, 66,82,1.8f, 82,100,4, 100,132,5};
            }
            RearTerraces(far,new Color(.29f,.34f,.31f),new Color(.3f,.42f,.33f),-20);
            RearTerraces(mid,new Color(.38f,.39f,.3f),new Color(.37f,.49f,.33f),-14);
            if(renderedMap==Map.Mist){
                foreach(float x in new[]{2f,13,26,34,45,54,60,85,96,108,118,128}){
                    float y=0;for(int i=0;i<mid.Length;i+=3)if(x>=mid[i]&&x<mid[i+1])y=mid[i+2];
                    Bamboo(x,y,3+(Mathf.Abs(x)%3)*.35f);
                }
            }else foreach(float x in renderedMap==Map.Village?new[]{-7f,33,37}:new[]{-24f,17,36}){
                float y=0;for(int i=0;i<mid.Length;i+=3)if(x>=mid[i]&&x<mid[i+1])y=mid[i+2];Bamboo(x,y,3);
            }
        }
        private void RearTerraces(float[] profile,Color soil,Color grass,int order){
            for(int i=0;i<profile.Length;i+=3){
                float left=profile[i],right=profile[i+1],top=profile[i+2];
                RectVisual("Rear earth terrace",world.transform,new Vector2((left+right)/2,(top-8)/2),new Vector2(right-left,top+8),soil,order);
                RectVisual("Rear terrace lip",world.transform,new Vector2((left+right)/2,top-.16f),new Vector2(right-left,.32f),grass,order+1);
                for(float x=left+.8f;x<right;x+=2.4f)for(float y=top-.8f;y>-6;y-=1.8f)
                    RectVisual("Rear earth grain",world.transform,new Vector2(x,y),new Vector2(.1f,.05f),soil*.87f,order+1);
            }
        }
        private void Bamboo(float x,float baseY,float height){
            for(int stem=0;stem<3;stem++){
                float px=x+stem*.28f,h=height-stem*.25f;
                RectVisual("Rear bamboo stem",world.transform,new Vector2(px,baseY+h/2),new Vector2(.09f,h),new Color(.3f,.48f,.32f),-11);
                for(float y=baseY+.5f;y<baseY+h;y+=.7f){
                    RectVisual("Bamboo joint",world.transform,new Vector2(px,y),new Vector2(.14f,.06f),new Color(.48f,.57f,.34f),-10);
                    var leaf=RectVisual("Bamboo leaves",world.transform,new Vector2(px+(stem%2==0?.3f:-.3f),y+.2f),new Vector2(.65f,.1f),new Color(.27f,.44f,.3f),-10);
                    leaf.transform.localEulerAngles=new Vector3(0,0,stem%2==0?30:-30);
                }
            }
        }
        private void Building(string name,float x,float width,float height){
            float baseY=(float)BlockoutLayout.GroundTop(renderedMap,x);
            RectVisual(name+" plaster",world.transform,new Vector2(x,baseY+height/2),new Vector2(width,height),new Color(.56f,.49f,.36f),-7);
            // Openings and horizontal timbers read as two floors; only authored wood decks collide.
            float floor=0;bool upstairs=false;
            foreach(var s in BlockoutLayout.Surfaces(renderedMap))if(s.OneWay&&Mathf.Abs((float)s.X-x)<.1f){floor=(float)(s.Y+s.Height/2);upstairs=true;}
            foreach(float y in upstairs?new[]{baseY+.45f,floor+.35f}:new[]{baseY+.45f})for(float col=x-width/2+.65f;col<x+width/2-.4f;col+=1.5f){
                RectVisual("Room opening",world.transform,new Vector2(col,y+.65f),new Vector2(1.1f,1.3f),new Color(.22f,.28f,.27f),-6);
                RectVisual("Window mullion",world.transform,new Vector2(col,y+.65f),new Vector2(.07f,1.3f),new Color(.52f,.39f,.25f),-5);
            }
            if(upstairs)RectVisual("Upper floor beam",world.transform,new Vector2(x,floor-.15f),new Vector2(width+.4f,.3f),new Color(.47f,.32f,.21f),-3);
            for(float col=x-width/2+.25f;col<=x+width/2;col+=width/2-.25f)
                RectVisual("Timber column",world.transform,new Vector2(col,baseY+height/2),new Vector2(.18f,height),new Color(.44f,.32f,.22f),-4);
            if(upstairs)RectVisual("Lower eave",world.transform,new Vector2(x,floor+.17f),new Vector2(width+.7f,.22f),new Color(.24f,.36f,.4f),-3);
            RectVisual("Roof",world.transform,new Vector2(x,baseY+height+.2f),new Vector2(width+.8f,.45f),new Color(.24f,.36f,.4f),-3);
            RectVisual("Roof ridge",world.transform,new Vector2(x,baseY+height+.48f),new Vector2(width-.4f,.12f),new Color(.38f,.48f,.48f),-2);
            Label(name,new Vector3(x-width*.2f,baseY+height+.75f,0),world.transform,.055f);
        }
        private void Puddle(WaterRegion region){
            float x=(float)((region.Left+region.Right)/2),width=(float)(region.Right-region.Left),depth=(float)(region.Level-region.Bottom);
            water.Add(((float)region.Left,(float)region.Right,(float)region.Level));
            RectVisual("WaterBase cosmetic",world.transform,new Vector2(x,(float)region.Bottom+depth/2),new Vector2(width,depth),new Color(.22f,.48f,.61f,.75f),3);
            RectVisual("WaterFrontOverlay cosmetic",world.transform,new Vector2(x,(float)region.Bottom+depth/2),new Vector2(width,depth),new Color(.38f,.68f,.76f,.22f),17);
            RectVisual("Water surface",world.transform,new Vector2(x,(float)region.Level),new Vector2(width,.06f),new Color(.64f,.85f,.88f),18);
        }
        private void Waterfall(float x,float top,float bottom){
            RectVisual("Waterfall behind lane",world.transform,new Vector2(x,(top+bottom)/2),new Vector2(.65f,top-bottom),new Color(.26f,.55f,.65f,.85f),3);
            for(float y=bottom+.2f;y<top;y+=.55f){var streak=RectVisual("Flow streak",world.transform,new Vector2(x+.13f,y),new Vector2(.12f,.18f),new Color(.5f,.76f,.8f,.7f),4);flowStreaks.Add((streak.transform,bottom+.1f,top-.1f,y-bottom));}
            RectVisual("Fall splash",world.transform,new Vector2(x+.3f,bottom+.05f),new Vector2(1.3f,.12f),new Color(.64f,.85f,.88f,.8f),4);
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
            Session.OnEvent=null;Session=PrototypePresets.Create(start);BindSession();
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
            if(k.escapeKey.wasPressedThisFrame){if(Modal){Hud.Back();axis=0;combatEvents.Clear();return;}Hud.Close();Session.Combat.ClearFocus();axis=0;combatEvents.Clear();return;}
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if(k.f9Key.wasPressedThisFrame)Hud.ToggleTrace();
            if(k.f8Key.wasPressedThisFrame){Hud.Toggle("debug");axis=0;combatEvents.Clear();return;}
#endif
            if(k.iKey.wasPressedThisFrame){Hud.Toggle("bag");axis=0;combatEvents.Clear();return;}
            if(k.cKey.wasPressedThisFrame){Hud.Toggle("equipment");axis=0;combatEvents.Clear();return;}
            if(k.qKey.wasPressedThisFrame){Hud.Toggle("quest");axis=0;combatEvents.Clear();return;}
            if(Modal){
                axis=0;Session.Combat.CancelIntent();combatEvents.Clear();
                if(k.upArrowKey.wasPressedThisFrame||k.wKey.wasPressedThisFrame)Hud.NavigateGrid(0,-1);
                if(k.leftArrowKey.wasPressedThisFrame||k.aKey.wasPressedThisFrame)Hud.NavigateGrid(-1,0);
                if(k.rightArrowKey.wasPressedThisFrame||k.dKey.wasPressedThisFrame)Hud.NavigateGrid(1,0);
                if(k.downArrowKey.wasPressedThisFrame||k.sKey.wasPressedThisFrame)Hud.NavigateGrid(0,1);
                if(k.tabKey.wasPressedThisFrame)Hud.ChangeTab(k.shiftKey.isPressed?-1:1);
                if(k.enterKey.wasPressedThisFrame||k.numpadEnterKey.wasPressedThisFrame||k.eKey.wasPressedThisFrame)HandleMenuKey("activate");
                return;
            }
            axis=Rules.Axis(k.aKey.isPressed,k.leftArrowKey.isPressed,k.dKey.isPressed,k.rightArrowKey.isPressed);
            if(axis!=0)Session.Combat.ManualOverride();
            jumpHeld=k.spaceKey.isPressed||k.upArrowKey.isPressed;
            if(k.spaceKey.wasPressedThisFrame||k.upArrowKey.wasPressedThisFrame){ExternalJump=true;Session.Combat.ManualOverride();}
            if(k.sKey.wasPressedThisFrame||k.downArrowKey.wasPressedThisFrame){ExternalDrop=true;Session.Combat.ManualOverride();}
            foreach(var input in combatEvents.OrderBy(x=>x.time).ThenBy(x=>x.slot).ThenBy(x=>x.sequence)){
                if(input.pressed)CombatPress(input.key,input.slot);else CombatRelease(input.key);
            }
            combatEvents.Clear();
            if(k.eKey.wasPressedThisFrame)Interact();if(k.fKey.wasPressedThisFrame)Session.UseFood();if(k.hKey.wasPressedThisFrame)Session.Potion(true);if(k.mKey.wasPressedThisFrame)Session.Potion(false);
            if(Mouse.current!=null&&Mouse.current.leftButton.wasPressedThisFrame&&!Hud.IsOverUi(Mouse.current.position.ReadValue())){
                var worldPos=cameraView.ScreenToWorldPoint(Mouse.current.position.ReadValue());var point=new Point(worldPos.x,worldPos.y);
                var target=Session.Mobs.Where(x=>x.Alive&&x.Map==renderedMap&&point.Distance(x.Position)<1)
                    .OrderBy(x=>point.Distance(x.Position)).ThenBy(x=>x.Id).FirstOrDefault();
                if(target!=null)Session.Combat.Explicit(target);
                else {var loot=Session.LootCandidate();if(loot!=null&&point.Distance(loot.Position)<.8){Session.PickUp(loot.Id);return;}var anchor=Session.NearAnchor();if(anchor!=null&&point.Distance(anchor.Position)<1.2&&Session.Interact(anchor.Id)){Hud.OpenNpc(anchor.Id);Speak(anchor.Id);}}
            }
        }
        private bool IsGrounded() {
            if(Body.linearVelocity.y>.1f)return false;
            var filter=new ContactFilter2D();filter.SetLayerMask(1<<6);filter.useLayerMask=true;
            int n=playerCollider.Cast(Vector2.down,filter,contacts,.08f);
            for(int i=0;i<n;i++)if(contacts[i].normal.y>.5f && !ignored.ContainsKey(contacts[i].collider as BoxCollider2D))return true;return false;
        }
        private void FixedUpdate() {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if(Hud.Panel=="debug"||Hud.Panel=="confirm"){if(Body.simulated)Body.simulated=false;Session.Combat.CancelIntent();return;}
#endif
            if(!Body.simulated)Body.simulated=true;
            if(Session.WorldRevision!=renderedRevision)BuildMap();
            foreach(var p in ignored.Keys.ToArray())if(p==null||Session.Now>=ignored[p]||playerCollider.bounds.max.y<p.bounds.min.y-.02f){if(p!=null)Physics2D.IgnoreCollision(playerCollider,p,false);ignored.Remove(p);}
            Grounded=IsGrounded();int manual=ExternalInput?ExternalAxis:axis;
            bool jump=ExternalJump,drop=ExternalDrop;ExternalJump=ExternalDrop=false;
            if(Modal||!Session.Player.Alive){manual=0;jump=drop=false;jumpBufferedUntil=lastGroundAt=-100;Session.Combat.CancelIntent();}
            if(manual!=0||jump||drop)Session.Combat.ManualOverride();
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
            if(!Modal)Session.ObservePosition(new Point(Body.position.x,Body.position.y),Grounded,jumped,dropped);
            else Session.Player.Position=new Point(Body.position.x,Body.position.y);
            Session.Tick(Time.fixedDeltaTime,manual!=0||jump||drop);
            int move=manual!=0?manual:Session.Combat.AssistAxis;
            if(move!=0&&manual==0){
                var foot=new Vector2(Body.position.x+move*.6f,Body.position.y-.5f);
                if(!Grounded||Physics2D.Raycast(foot,Vector2.down,.5f,1<<6).collider==null||Physics2D.Raycast(Body.position,new Vector2(move,0),.65f,1<<6).collider!=null)move=0;
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
            if(!Session.Player.Alive && Session.Player.Map==Map.Village && Body.position.y<-8)Body.position=new Vector2((float)Session.Player.Position.X,.8f);
            cameraView.transform.position=Vector3.SmoothDamp(cameraView.transform.position,CameraTarget(),ref cameraVelocity,.13f,100,Time.deltaTime);
            foreach(var layer in scenery)layer.root.position=layer.home+new Vector3(cameraView.transform.position.x*layer.factor,0,0);
            foreach(var flow in flowStreaks){var pos=flow.sprite.localPosition;pos.y=flow.top-Mathf.Repeat((float)Session.Now*.9f+flow.phase,flow.top-flow.bottom);flow.sprite.localPosition=pos;}
            foreach(var pair in npcMarkers){pair.Value.text=Hud.Marker(pair.Key);pair.Value.color=pair.Value.text=="?"?Color.yellow:pair.Value.text=="!"?new Color(.7f,.9f,.55f):Color.white;}
            if(speech!=null&&Session.Now>speechUntil){Destroy(speech.gameObject);speech=null;}
            var action=Session.Combat.Running;
            float phase=action==null?0:Mathf.Clamp01((float)((Session.Now-action.Started)/action.Skill.Lock));
            var parts=GeometricRig.Pose(Session.Player,Session.Combat.Facing,Session.Now,phase,action!=null,Body.linearVelocity.x).ToArray();
            foreach(var oldPart in rig.Values)oldPart.enabled=false;
            foreach(var part in parts){
                if(!rig.TryGetValue(part.Name,out var sprite)){sprite=RectVisual(part.Name,playerVisual.transform,part.Center,part.Size,part.Color,part.Layer);rig.Add(part.Name,sprite);}
                sprite.enabled=true;sprite.transform.localPosition=part.Center;sprite.transform.localScale=part.Size;sprite.transform.localEulerAngles=new Vector3(0,0,part.Angle);
                sprite.color=!Session.Player.Alive?Color.gray:Session.Now-Session.LastHurtAt<.12?Color.white:part.Color;
            }
            if(Session.Now>=nextRipple&&Mathf.Abs(Body.linearVelocity.x)>.5f){
                foreach(var puddle in water)if(Body.position.x>puddle.left&&Body.position.x<puddle.right&&BlockoutLayout.WaterSpeed(renderedMap,new Point(Body.position.x,playerCollider.bounds.min.y))<1){
                    var ripple=Label("≈",new Vector3(Body.position.x,puddle.top+.12f,0),null,.07f);ripple.color=new Color(.6f,.84f,.86f);floating.Add((ripple,Time.time+.35f));nextRipple=Session.Now+.25;break;
                }
            }
            foreach(var m in Session.Mobs.Where(x=>x.Map==renderedMap)){
                var view=mobViews[m.Id];view.gameObject.SetActive(m.Alive);view.enabled=m.Alive;if(!m.Dummy&&m.Level==4)view.transform.localScale=new Vector3(-m.Facing*.8f,.7f,1);var rendered=Vector2.Lerp(new Vector2((float)m.PreviousPosition.X,(float)m.PreviousPosition.Y),new Vector2((float)m.Position.X,(float)m.Position.Y),Mathf.Clamp01((Time.time-Time.fixedTime)/Time.fixedDeltaTime));view.transform.localPosition=rendered;
                view.color=flashUntil.TryGetValue(m.Id,out var until)&&Session.Now<until?Color.white:m.Returning?Color.gray:m.Windup?new Color(1,.5f,.3f):m.Dummy?new Color(.65f,.5f,.25f):m.Level==2?new Color(.7f,.35f,.45f):new Color(.55f,.65f,.78f);
            }
            foreach(var l in Session.Loot.Where(x=>x.Map==renderedMap&&!x.Claimed)){
                if(!lootViews.TryGetValue(l.Id,out var view)){view=RectVisual("Loot "+l.Id,world.transform,new Vector2((float)l.Position.X,(float)l.Position.Y-.3f),new Vector2(.25f,.25f),Color.cyan,6);lootViews[l.Id]=view;}
                view.transform.localPosition=new Vector3((float)l.Position.X,(float)l.Position.Y-.3f,0);
                view.color=Session.LootCandidate()?.Id==l.Id?Color.yellow:Color.cyan;
            }
            foreach(var pair in lootViews)if(Session.Loot.First(x=>x.Id==pair.Key).Claimed)pair.Value.enabled=false;
            var focus=Session.Combat.Focus;marker.enabled=focus!=null;
            if(focus!=null)marker.transform.position=new Vector3((float)focus.Position.X,(float)focus.Position.Y-1,0);
            for(;feedbackResultCount<Session.Combat.Results.Count;feedbackResultCount++){
                var result=Session.Combat.Results[feedbackResultCount];var mob=Session.Mobs.First(x=>x.Id==result.Target);
                if(mob.Map!=renderedMap||mob.Generation!=result.Generation)continue;
                flashUntil[mob.Id]=Session.Now+.12;
                var txt=Label(result.Evaded?"NÉ":result.Damage.ToString(),new Vector3((float)mob.Position.X,(float)mob.Position.Y+1.1f,0));txt.color=result.Crit?Color.yellow:Color.white;floating.Add((txt,Time.time+1));
            }
            for(int i=floating.Count-1;i>=0;i--)if(Time.time>=floating[i].expiry){Destroy(floating[i].mesh.gameObject);floating.RemoveAt(i);}else floating[i].mesh.transform.Translate(0,Time.deltaTime*.4f,0);
        }
        public Vector2 ScreenPoint(Point point)=>cameraView.WorldToScreenPoint(new Vector3((float)point.X,(float)point.Y,0));
        private void OnDestroy(){if(combatBindings!=null)foreach(var binding in combatBindings)binding.Dispose();if(world!=null)Destroy(world);if(playerVisual!=null)Destroy(playerVisual);if(cameraView!=null)Destroy(cameraView.gameObject);if(marker!=null)Destroy(marker.gameObject);foreach(var v in floating)if(v.mesh!=null)Destroy(v.mesh.gameObject);}
    }
}
