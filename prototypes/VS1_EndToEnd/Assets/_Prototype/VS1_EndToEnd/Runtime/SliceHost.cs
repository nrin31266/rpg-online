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
        private readonly List<(string key,int slot,bool pressed,double time,long sequence)> combatEvents=new List<(string,int,bool,double,long)>();
        private InputAction[] combatBindings;
        private long inputSequence;
        private Camera cameraView;
        private GameObject world,playerVisual;
        private BoxCollider2D playerCollider;
        private SpriteRenderer torso,pants,weapon;
        private readonly Dictionary<int,SpriteRenderer> mobViews=new Dictionary<int,SpriteRenderer>();
        private readonly Dictionary<long,SpriteRenderer> lootViews=new Dictionary<long,SpriteRenderer>();
        private readonly List<BoxCollider2D> platforms=new List<BoxCollider2D>();
        private readonly Dictionary<BoxCollider2D,float> ignored=new Dictionary<BoxCollider2D,float>();
        private Map renderedMap;
        private int renderedRevision;
        private Sprite square;
        private SpriteRenderer marker;
        private int feedbackResultCount;
        private readonly Dictionary<int,double> flashUntil=new Dictionary<int,double>();
        private readonly List<(TextMesh mesh,float expiry)> floating=new List<(TextMesh,float)>();
        private readonly RaycastHit2D[] contacts=new RaycastHit2D[8];
        public const float RunSpeed=5,JumpSpeed=12;
        public static string MapName(Map map) => map==Map.Village?"Vân Khê":map==Map.Academy?"Học Viện":"Đồng Sương";
        private void Awake() {
            Application.targetFrameRate=60;Time.fixedDeltaTime=.02f;Application.SetStackTraceLogType(LogType.Log,StackTraceLogType.None);
            Session=new SliceSession();Session.OnEvent=x=>Debug.Log("[VS1] "+x);
            combatBindings=new InputAction[4];
            var keys=new[]{"j","1","2","3"};
            for(int i=0;i<keys.Length;i++){
                int slot=i;string key=i==0?"J":keys[i];
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
            torso=RectVisual("Torso",playerVisual.transform,new Vector2(0,.15f),new Vector2(.5f,.75f),new Color(.55f,.55f,.6f),10);
            pants=RectVisual("Pants",playerVisual.transform,new Vector2(0,-.45f),new Vector2(.5f,.45f),new Color(.25f,.3f,.4f),10);
            RectVisual("Head",playerVisual.transform,new Vector2(0,.65f),new Vector2(.4f,.4f),new Color(.8f,.67f,.5f),10);
            weapon=RectVisual("Weapon",playerVisual.transform,new Vector2(.4f,0),new Vector2(.12f,.9f),new Color(.6f,.4f,.2f),11);
            marker=RectVisual("CombatFocus",null,Vector2.zero,new Vector2(.9f,.08f),Color.yellow,12);
            Hud=gameObject.AddComponent<SliceHud>();Hud.Host=this;
            BuildMap();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (System.Environment.GetCommandLineArgs().Contains("--verify-route")) gameObject.AddComponent<SliceRouteProbe>();
#endif
        }
        private SpriteRenderer RectVisual(string name,Transform parent,Vector2 position,Vector2 size,Color color,int order=0) {
            var go=new GameObject(name);if(parent!=null)go.transform.SetParent(parent,false);go.transform.localPosition=position;
            go.transform.localScale=size;var sr=go.AddComponent<SpriteRenderer>();sr.sprite=square;sr.color=color;sr.sortingOrder=order;return sr;
        }
        private void Ground(string name,float x,float y,float width,float height,bool oneWay=false) {
            var sr=RectVisual(name,world.transform,new Vector2(x,y),new Vector2(width,height),oneWay?new Color(.36f,.55f,.58f):new Color(.21f,.3f,.34f));
            sr.gameObject.layer=6;var box=sr.gameObject.AddComponent<BoxCollider2D>();
            if(oneWay){var eff=sr.gameObject.AddComponent<PlatformEffector2D>();eff.useOneWay=true;eff.useOneWayGrouping=true;eff.surfaceArc=160;box.usedByEffector=true;platforms.Add(box);}
        }
        private TextMesh Label(string text,Vector3 position,Transform parent=null,float scale=.12f) {
            var go=new GameObject("Label "+text);if(parent!=null)go.transform.SetParent(parent);go.transform.position=position;
            var mesh=go.AddComponent<TextMesh>();mesh.font=Resources.Load<Font>("Fonts/DejaVuSans");go.GetComponent<MeshRenderer>().sharedMaterial=mesh.font.material;mesh.text=text;mesh.fontSize=32;mesh.characterSize=scale;mesh.anchor=TextAnchor.MiddleCenter;mesh.color=Color.white;go.GetComponent<MeshRenderer>().sortingOrder=20;return mesh;
        }
        private void BuildMap() {
            foreach(var v in floating)if(v.mesh!=null)Destroy(v.mesh.gameObject);floating.Clear();
            if(world!=null)Destroy(world);world=new GameObject("Map "+Session.Player.Map);mobViews.Clear();lootViews.Clear();platforms.Clear();ignored.Clear();renderedMap=Session.Player.Map;renderedRevision=Session.WorldRevision;
            float width=renderedMap==Map.Mist?130:60;
            Ground("Ground",width/2-10,-.3f,width,.6f);Ground("LeftBoundary",-9,3,1,7);Ground("RightBoundary",width-11,3,1,7);
            if(renderedMap==Map.Academy){Ground("HV_JumpLedge one-way",8,2.8f,4,.4f,true);Label("HV_JumpLedge\nS + Space để xuống",new Vector3(8,5,0),world.transform);Label("HV_DummyYard",new Vector3(22,2.6f,0),world.transform);}
            if(renderedMap==Map.Mist)Ground("DS optional upper lane",50,2.8f,12,.4f,true);
            foreach(var a in SliceSession.Anchors.Where(x=>x.Map==renderedMap)){
                RectVisual(a.Id,world.transform,new Vector2((float)a.Position.X,(float)a.Position.Y),a.Portal?new Vector2(.5f,2.4f):new Vector2(.6f,1.6f),a.Portal?new Color(.45f,.35f,.75f):new Color(.5f,.65f,.52f));
                Label(a.Name,new Vector3((float)a.Position.X,2.1f,0),world.transform,.09f);
            }
            if(renderedMap==Map.Mist)for(int i=1;i<=6;i++)Label("DS"+i,new Vector3(i<=2?5+(i-1)*10:30+(i-3)*20,3,0),world.transform);
            foreach(var m in Session.Mobs.Where(x=>x.Map==renderedMap))mobViews[m.Id]=RectVisual(m.Slot,world.transform,new Vector2((float)m.Position.X,(float)m.Position.Y),m.Dummy?new Vector2(.6f,1.4f):new Vector2(.8f,.7f),m.Dummy?new Color(.65f,.5f,.25f):m.Level==2?new Color(.7f,.35f,.45f):new Color(.55f,.65f,.78f),5);
            Body.position=new Vector2((float)Session.Player.Position.X,(float)Session.Player.Position.Y);Body.linearVelocity=Vector2.zero;
            Label(MapName(renderedMap)+" — blockout VS-1",new Vector3(8,6,0),world.transform,.13f);
        }
        public void CombatPress(string key,int slot=0){if(!Modal)Session.Combat.Press(key,slot);}
        public void CombatRelease(string key)=>Session.Combat.Release(key);
        public void Interact() {
            if(Modal||!Session.Player.Alive)return;
            var loot=Session.LootCandidate();if(loot!=null){Session.PickUp(loot.Id);return;}
            var anchor=Session.NearAnchor();if(anchor==null){Session.Feedback="Không có NPC/portal/đồ trong tầm E.";return;}
            if(Session.Interact(anchor.Id)&&!anchor.Portal)Hud.OpenNpc(anchor.Id);
        }
        private void Update() {
            if(ExternalInput){combatEvents.Clear();return;}var k=Keyboard.current;if(k==null){combatEvents.Clear();return;}
            if(k.escapeKey.wasPressedThisFrame){Hud.Close();Session.Combat.CancelIntent();Session.Combat.ClearFocus();}
            if(k.bKey.wasPressedThisFrame)Hud.Toggle("bag");if(k.cKey.wasPressedThisFrame)Hud.Toggle("stats");if(k.kKey.wasPressedThisFrame)Hud.Toggle("skills");if(k.lKey.wasPressedThisFrame)Hud.Toggle("quest");
            if(Modal){axis=0;Session.Combat.CancelIntent();combatEvents.Clear();return;}
            axis=Rules.Axis(k.aKey.isPressed,k.leftArrowKey.isPressed,k.dKey.isPressed,k.rightArrowKey.isPressed);
            if(axis!=0)Session.Combat.ManualOverride();
            if(k.spaceKey.wasPressedThisFrame){ExternalJump=!k.sKey.isPressed;ExternalDrop=k.sKey.isPressed;Session.Combat.ManualOverride();}
            // Preserve event timestamps instead of inventing a J/slot polling order.
            // Truly simultaneous presses use slots before J, so J sees the chosen slot.
            foreach(var input in combatEvents.OrderBy(x=>x.time).ThenBy(x=>x.slot==0?4:x.slot).ThenBy(x=>x.sequence)){
                if(input.pressed)CombatPress(input.key,input.slot);else CombatRelease(input.key);
            }
            combatEvents.Clear();
            if(k.eKey.wasPressedThisFrame)Interact();if(k.fKey.wasPressedThisFrame)Session.UseFood();if(k.hKey.wasPressedThisFrame)Session.Potion(true);if(k.mKey.wasPressedThisFrame)Session.Potion(false);
            if(Mouse.current!=null&&Mouse.current.leftButton.wasPressedThisFrame&&!Hud.IsOverUi(Mouse.current.position.ReadValue())){
                var worldPos=cameraView.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                var target=Session.Mobs.Where(x=>x.Alive&&x.Map==renderedMap&&new Point(worldPos.x,worldPos.y).Distance(x.Position)<1)
                    .OrderBy(x=>new Point(worldPos.x,worldPos.y).Distance(x.Position)).ThenBy(x=>x.Id).FirstOrDefault();if(target!=null)Session.Combat.Explicit(target);
            }
        }
        private bool IsGrounded() {
            if(Body.linearVelocity.y>.1f)return false;
            var filter=new ContactFilter2D();filter.SetLayerMask(1<<6);filter.useLayerMask=true;
            int n=playerCollider.Cast(Vector2.down,filter,contacts,.08f);
            for(int i=0;i<n;i++)if(contacts[i].normal.y>.5f && !ignored.ContainsKey(contacts[i].collider as BoxCollider2D))return true;return false;
        }
        private void FixedUpdate() {
            if(Session.WorldRevision!=renderedRevision)BuildMap();
            foreach(var p in ignored.Keys.ToArray())if(p==null||Time.time>=ignored[p]){if(p!=null)Physics2D.IgnoreCollision(playerCollider,p,false);ignored.Remove(p);}
            Grounded=IsGrounded();int manual=ExternalInput?ExternalAxis:axis;
            bool jump=ExternalJump,drop=ExternalDrop;ExternalJump=ExternalDrop=false;
            if(Modal||!Session.Player.Alive){manual=0;jump=drop=false;Session.Combat.CancelIntent();}
            if(manual!=0||jump||drop)Session.Combat.ManualOverride();
            bool dropped=false;
            if(drop&&Grounded){foreach(var p in platforms){
                if(Math.Abs(Body.position.x-p.transform.position.x)<=p.bounds.extents.x+.3f && Math.Abs(playerCollider.bounds.min.y-p.bounds.max.y)<.2f){
                    Physics2D.IgnoreCollision(playerCollider,p,true);ignored[p]=Time.time+.65f;dropped=true;}}
                if(dropped){Body.linearVelocity=new Vector2(Body.linearVelocity.x,-2);Session.Emit("Physics drop-through pair: "+Body.position);}
            }
            if(jump&&Grounded)Body.linearVelocity=new Vector2(Body.linearVelocity.x,JumpSpeed);
            Session.ObservePosition(new Point(Body.position.x,Body.position.y),Grounded,jump&&Grounded,dropped);
            Session.Tick(Time.fixedDeltaTime,manual!=0||jump||drop);
            int move=manual!=0?manual:Session.Combat.AssistAxis;
            if(move!=0 && manual==0){
                var foot=new Vector2(Body.position.x+move*.6f,Body.position.y-.5f);
                if(!Grounded||Physics2D.Raycast(foot,Vector2.down,.5f,1<<6).collider==null || Physics2D.Raycast(Body.position,new Vector2(move,0),.65f,1<<6).collider!=null)move=0;
            }
            if(!Session.Player.Alive)move=0;
            Body.linearVelocity=new Vector2(move*RunSpeed*(float)Session.Player.Stats.Speed,Body.linearVelocity.y);
            if(move!=0&&Session.Combat.Running==null)Session.Combat.Facing=move;
            if(Body.position.y<-8){Session.HurtPlayer(Session.Player.Stats.Hp);}
        }
        private void LateUpdate() {
            if(Session.WorldRevision!=renderedRevision)BuildMap();
            if(!Session.Player.Alive && Session.Player.Map==Map.Village && Body.position.y<-8)Body.position=new Vector2((float)Session.Player.Position.X,.8f);
            cameraView.transform.position=new Vector3(Mathf.Lerp(cameraView.transform.position.x,Mathf.Max(5,Body.position.x),Time.deltaTime*8),3,-10);
            var inv=Session.Player.Inventory;torso.color=inv.Equipment.ContainsKey(GearSlot.Armor)?new Color(.3f,.7f,.5f):new Color(.55f,.55f,.6f);
            pants.color=inv.Equipment.ContainsKey(GearSlot.Pants)?new Color(.25f,.55f,.5f):new Color(.25f,.3f,.4f);
            weapon.enabled=inv.Equipment.TryGetValue(GearSlot.Weapon,out var visibleWeapon);weapon.color=visibleWeapon!=null&&visibleWeapon.Id=="sword1"?new Color(.75f,.87f,.83f):new Color(.6f,.4f,.2f);
            weapon.transform.localPosition=new Vector3(Session.Combat.Facing*.4f,0,0);
            var action=Session.Combat.Running;
            float phase=action==null?0:Mathf.Clamp01((float)((Session.Now-action.Started)/action.Skill.Lock));
            weapon.transform.localEulerAngles=new Vector3(0,0,action==null?0:Session.Combat.Facing*Mathf.Sin(phase*Mathf.PI)*-75);
            torso.transform.localPosition=new Vector3(action==null?0:Session.Combat.Facing*.07f,.15f,0);
            if(!Session.Player.Alive){torso.color=Color.gray;pants.color=Color.gray;}else if(Session.Now-Session.LastHurtAt<.12)torso.color=Color.white;
            foreach(var m in Session.Mobs.Where(x=>x.Map==renderedMap)){
                var view=mobViews[m.Id];view.enabled=m.Alive;view.transform.localPosition=new Vector3((float)m.Position.X,(float)m.Position.Y,0);
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
