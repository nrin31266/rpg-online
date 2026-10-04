using System.Collections;
using HuyenLo.Domain;
using HuyenLo.Runtime;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

namespace HuyenLo.Tests
{
    public sealed class InputPhysicsTests
    {
        private IEnumerator Tap(Keyboard keyboard, params Key[] keys){
            InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(keys));yield return null;yield return new WaitForFixedUpdate();
            InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;
        }
        [UnityTest] public IEnumerator MovementProbeVariableHeightCoyoteAndStandaloneDrop(){
            var go=new GameObject("Movement feel fixture");var host=go.AddComponent<SliceHost>();host.ExternalInput=true;host.ResetPrototype(PrototypeStart.SwordTraining);
            try {
                yield return new WaitForFixedUpdate();yield return new WaitForFixedUpdate();
                host.ExternalJumpHeld=true;host.ExternalJump=true;float high=0;
                for(int i=0;i<45;i++){yield return new WaitForFixedUpdate();high=Mathf.Max(high,host.Body.position.y);}
                for(int i=0;i<30;i++)yield return new WaitForFixedUpdate();
                host.ExternalJump=true;host.ExternalJumpHeld=false;float low=0;
                for(int i=0;i<45;i++){yield return new WaitForFixedUpdate();low=Mathf.Max(low,host.Body.position.y);}
                Assert.That(high,Is.GreaterThan(low+.5f),"Holding jump should make a taller jump, not a forced fixed height");
                host.ExternalJumpHeld=true;host.Body.position=new Vector2(8,3.72f);host.Body.linearVelocity=Vector2.zero;
                yield return new WaitForFixedUpdate();yield return new WaitForFixedUpdate();Assert.That(host.Grounded,Is.True);
                host.Body.position=new Vector2(10.5f,host.Body.position.y);host.Body.linearVelocity=Vector2.zero;host.ExternalJump=true;yield return new WaitForFixedUpdate();
                Assert.That(host.Body.linearVelocity.y,Is.GreaterThan(0),"Coyote jump must work just after leaving support");
                for(int i=0;i<8;i++)yield return new WaitForFixedUpdate();float rising=host.Body.linearVelocity.y;
                host.ExternalJump=true;yield return new WaitForFixedUpdate();Assert.That(host.Body.linearVelocity.y,Is.LessThan(rising),"A second press in air must not grant a double jump");
                // Isolate drop from the earlier, legitimately buffered jump intent.
                host.ResetPrototype(PrototypeStart.SwordTraining);host.Body.position=new Vector2(8,3.72f);host.Body.transform.position=host.Body.position;host.Body.linearVelocity=Vector2.zero;Physics2D.SyncTransforms();
                yield return new WaitForFixedUpdate();yield return new WaitForFixedUpdate();Assert.That(host.Grounded,Is.True);
                host.ExternalDrop=true;yield return new WaitForFixedUpdate();for(int i=0;i<18;i++)yield return new WaitForFixedUpdate();
                Assert.That(host.Body.position.y,Is.LessThan(3.0f),"Drop action alone must pass through the support");
                host.Session.Emit($"Movement probe: hold apex {high:F2}, tap apex {low:F2}; coyote/no-double/drop PASS.");
            }finally {Object.Destroy(go);}
            yield return null;
        }
        private IEnumerator WalkLeftTo(SliceHost host,float x){
            int guard=0;while(host.Body.position.x>x+.08f){
                Assert.That(++guard,Is.LessThan(350),"Failed to reach authored stair "+x);
                host.ExternalAxis=-1;
                if(host.Grounded){var hit=Physics2D.Raycast(host.Body.position+new Vector2(0,-.6f),Vector2.left,.7f,1<<6);
                    if(hit.collider!=null&&hit.normal.y<.5f){host.ExternalJump=true;host.ExternalJumpHeld=true;}}
                yield return new WaitForFixedUpdate();
            }
            host.ExternalAxis=0;for(int i=0;i<70;i++)yield return new WaitForFixedUpdate();
        }
        [UnityTest] public IEnumerator ClimbRealHillThenTalkToNpcWithKeyboard(){
            var go=new GameObject("Climb NPC hill fixture");var host=go.AddComponent<SliceHost>();host.ExternalInput=true;host.ResetPrototype(PrototypeStart.SwordTraining);
            var oldBackground=InputSystem.settings.backgroundBehavior;var oldEditor=InputSystem.settings.editorInputBehaviorInPlayMode;
            InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
            InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            var keyboard=InputSystem.AddDevice<Keyboard>();keyboard.MakeCurrent();
            try{
                host.Body.position=new Vector2(-4.6f,.72f);host.Body.transform.position=host.Body.position;host.Body.linearVelocity=Vector2.zero;Physics2D.SyncTransforms();
                for(int i=0;i<5;i++)yield return new WaitForFixedUpdate();Assert.That(host.Session.NearAnchor(),Is.Null);
                yield return WalkLeftTo(host,-6.6f);Assert.That(host.Body.position.y,Is.EqualTo(3.52f).Within(.1));
                yield return WalkLeftTo(host,-7.45f);Assert.That(host.Body.position.y,Is.EqualTo(4.92f).Within(.1));
                host.ExternalJump=true;host.ExternalJumpHeld=true;host.ExternalAxis=-1;
                int guard=0;while(host.Body.position.x>-12){Assert.That(++guard,Is.LessThan(350));yield return new WaitForFixedUpdate();}host.ExternalAxis=0;
                for(int i=0;i<75;i++)yield return new WaitForFixedUpdate();
                Assert.That(host.Grounded,Is.True);Assert.That(host.Body.position.y,Is.EqualTo(7.12f).Within(.1));
                Assert.That(host.Session.NearAnchor().Id,Is.EqualTo("Diep"));
                host.ExternalInput=false;yield return Tap(keyboard,Key.E);Assert.That(host.Hud.Panel,Is.EqualTo("npc"));
                yield return Tap(keyboard,Key.Escape);Assert.That(host.Hud.Panel,Is.Null.Or.Empty);
            }finally{InputSystem.settings.backgroundBehavior=oldBackground;InputSystem.settings.editorInputBehaviorInPlayMode=oldEditor;InputSystem.RemoveDevice(keyboard);Object.Destroy(go);}yield return null;
        }
        [UnityTest] public IEnumerator ClimbUpperPackFromLowerRoute(){
            var go=new GameObject("Reach upper eastern pack");var host=go.AddComponent<SliceHost>();host.ExternalInput=true;host.ResetPrototype(PrototypeStart.Crowd);host.Session.Player.InvulnerableUntil=100;
            try{
                for(int i=0;i<5;i++)yield return new WaitForFixedUpdate();host.ExternalJump=true;host.ExternalJumpHeld=true;host.ExternalAxis=1;
                while(host.Body.position.x<107)yield return new WaitForFixedUpdate();host.ExternalAxis=0;
                for(int i=0;i<70;i++)yield return new WaitForFixedUpdate();Assert.That(host.Grounded,Is.True);Assert.That(host.Body.position.y,Is.EqualTo(3.22f).Within(.1));
                host.ExternalJump=true;host.ExternalJumpHeld=true;host.ExternalAxis=1;int guard=0;
                while(host.Body.position.x<112){Assert.That(++guard,Is.LessThan(150));yield return new WaitForFixedUpdate();}host.ExternalAxis=0;
                for(int i=0;i<70;i++)yield return new WaitForFixedUpdate();Assert.That(host.Grounded,Is.True);Assert.That(host.Body.position.y,Is.EqualTo(5.32f).Within(.1));
                host.ExternalAxis=1;for(int i=0;i<140;i++)yield return new WaitForFixedUpdate();host.ExternalAxis=0;
                Assert.That(host.Body.position.x,Is.GreaterThan(124),"Upper route must descend to eastern road without a dead end");
            }finally{Object.Destroy(go);}yield return null;
        }
        [UnityTest] public IEnumerator RearEarthAllowsBodyTraversalUpwardJumpLandingAndDeliberateDrop(){
            var go=new GameObject("Physical earth overhang fixture");var host=go.AddComponent<SliceHost>();host.ExternalInput=true;host.ResetPrototype(PrototypeStart.Crowd);host.Session.Player.InvulnerableUntil=100;
            try{
                host.Body.position=new Vector2(57.2f,1.92f);host.Body.transform.position=host.Body.position;host.Body.linearVelocity=Vector2.zero;Physics2D.SyncTransforms();
                host.ExternalAxis=1;for(int i=0;i<65;i++)yield return new WaitForFixedUpdate();host.ExternalAxis=0;
                Assert.That(host.Body.position.x,Is.GreaterThan(61));Assert.That(host.Body.position.y,Is.EqualTo(1.92f).Within(.08),"Rear visual fill must not push the actor onto its top");
                host.Body.position=new Vector2(60,1.92f);host.Body.transform.position=host.Body.position;host.Body.linearVelocity=Vector2.zero;Physics2D.SyncTransforms();
                for(int i=0;i<5;i++)yield return new WaitForFixedUpdate();host.ExternalJump=true;host.ExternalJumpHeld=true;host.ExternalAxis=0;
                for(int i=0;i<70;i++)yield return new WaitForFixedUpdate();host.ExternalAxis=0;
                for(int i=0;i<8;i++)yield return new WaitForFixedUpdate();Assert.That(host.Grounded,Is.True);
                Assert.That(host.Body.position.y,Is.EqualTo(5.32f).Within(.1),"Jump upward through rear fill and land on its top");
                host.ExternalJumpHeld=false;host.ExternalDrop=true;for(int i=0;i<65;i++)yield return new WaitForFixedUpdate();Assert.That(host.Grounded,Is.True);Assert.That(host.Body.position.y,Is.EqualTo(1.92f).Within(.1),"S drops through only the rear support and lands safely on the solid lower lane");
            }finally{Object.Destroy(go);}yield return null;
        }
        [UnityTest] public IEnumerator WalkableBasinAndBridgeHaveDistinctMovementSpeeds(){
            var go=new GameObject("Water feet fixture");var host=go.AddComponent<SliceHost>();host.ExternalInput=true;
            try {
                host.Body.position=new Vector2(3,.37f);host.Body.transform.position=host.Body.position;host.Body.linearVelocity=Vector2.zero;Physics2D.SyncTransforms();
                for(int i=0;i<5;i++)yield return new WaitForFixedUpdate();Assert.That(host.Grounded,Is.True);Assert.That(host.Body.position.y,Is.LessThan(.5));
                host.ExternalAxis=1;for(int i=0;i<4;i++)yield return new WaitForFixedUpdate();
                Assert.That(host.Body.linearVelocity.x,Is.EqualTo(SliceHost.RunSpeed*.85f).Within(.05));
                host.ResetPrototype(PrototypeStart.Crowd);host.ExternalAxis=1;
                host.Body.position=new Vector2(73,1.92f);host.Body.transform.position=host.Body.position;host.Body.linearVelocity=Vector2.zero;Physics2D.SyncTransforms();
                for(int i=0;i<8;i++)yield return new WaitForFixedUpdate();Assert.That(host.Grounded,Is.True);
                host.ExternalDrop=true;for(int i=0;i<12;i++)yield return new WaitForFixedUpdate();Assert.That(host.Body.position.y,Is.GreaterThan(1.8),"Wide-water bridge is solid; S must not reach the water");
                Assert.That(host.Body.linearVelocity.x,Is.EqualTo(SliceHost.RunSpeed*(float)host.Session.Player.Stats.Speed).Within(.05),"Bridge over water remains dry");
            }finally{Object.Destroy(go);}yield return null;
        }
        [UnityTest] public IEnumerator BridgeAndBanksCanBeWalkedBothWaysWithoutJump(){
            var go=new GameObject("Continuous bridge route fixture");var host=go.AddComponent<SliceHost>();host.ExternalInput=true;host.ResetPrototype(PrototypeStart.Crowd);host.Session.Player.InvulnerableUntil=100;
            try{
                host.Body.position=new Vector2(60,1.92f);host.Body.transform.position=host.Body.position;host.Body.linearVelocity=Vector2.zero;Physics2D.SyncTransforms();
                for(int i=0;i<5;i++)yield return new WaitForFixedUpdate();int guard=0;host.ExternalAxis=1;
                while(host.Body.position.x<88){Assert.That(++guard,Is.LessThan(500),"Eastbound crossing blocked");Assert.That(host.Body.position.y,Is.LessThan(2.15f),"Should not hop over a bank");yield return new WaitForFixedUpdate();}
                host.ExternalAxis=-1;guard=0;while(host.Body.position.x>60){Assert.That(++guard,Is.LessThan(500),"Westbound crossing blocked");Assert.That(host.Body.position.y,Is.LessThan(2.15f));yield return new WaitForFixedUpdate();}
                host.ExternalAxis=0;for(int i=0;i<10;i++)yield return new WaitForFixedUpdate();Assert.That(host.Grounded,Is.True);Assert.That(host.Body.position.y,Is.EqualTo(1.92f).Within(.1));
            }finally{Object.Destroy(go);}yield return null;
        }
        [UnityTest] public IEnumerator SolidTerraceBlocksWalkAndDropButJumpCanTraverse(){
            var go=new GameObject("Solid terrace fixture");var host=go.AddComponent<SliceHost>();host.ExternalInput=true;
            try {
                host.Body.position=new Vector2(6,.72f);host.Body.transform.position=host.Body.position;host.Body.linearVelocity=Vector2.zero;Physics2D.SyncTransforms();
                for(int i=0;i<5;i++)yield return new WaitForFixedUpdate();host.ExternalAxis=1;
                for(int i=0;i<35;i++)yield return new WaitForFixedUpdate();Assert.That(host.Body.position.x,Is.LessThan(8),"Solid step must block, not act as a one-way");
                host.ExternalDrop=true;for(int i=0;i<12;i++)yield return new WaitForFixedUpdate();Assert.That(host.Body.position.x,Is.LessThan(8));
                host.ExternalJumpHeld=true;host.ExternalJump=true;for(int i=0;i<35;i++)yield return new WaitForFixedUpdate();
                Assert.That(host.Body.position.x,Is.GreaterThan(9),"Ordinary jump must cross the hub step");
            }finally {Object.Destroy(go);}yield return null;
        }
        [UnityTest] public IEnumerator KeyboardNpcShopInventoryAndDebugResetNeedNoMouse(){
            var go=new GameObject("Keyboard UI fixture");var host=go.AddComponent<SliceHost>();
            var oldBackground=InputSystem.settings.backgroundBehavior;var oldEditor=InputSystem.settings.editorInputBehaviorInPlayMode;
            InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
            InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            var keyboard=InputSystem.AddDevice<Keyboard>();keyboard.MakeCurrent();
            try {
                yield return new WaitForFixedUpdate();yield return Tap(keyboard,Key.E);
                Assert.That(host.Hud.Panel,Is.EqualTo("npc"));Assert.That(host.Hud.SelectedActionId,Is.EqualTo("quest.accept"));
                yield return Tap(keyboard,Key.Enter);Assert.That(host.Session.QuestState,Is.EqualTo(QuestState.InProgress));yield return Tap(keyboard,Key.Escape);
                host.ResetPrototype(PrototypeStart.Wolves);yield return new WaitForFixedUpdate();yield return Tap(keyboard,Key.E);yield return Tap(keyboard,Key.Enter);
                Assert.That(host.Session.Quest,Is.EqualTo(5));Assert.That(host.Hud.Panel,Is.Null);yield return Tap(keyboard,Key.E);Assert.That(host.Hud.SelectedActionId,Is.EqualTo("service.buy"));
                yield return Tap(keyboard,Key.Enter);Assert.That(host.Hud.Panel,Is.EqualTo("buy"));Assert.That(host.Hud.SelectedActionId,Is.EqualTo("buy.food1"));
                yield return Tap(keyboard,Key.Enter);yield return Tap(keyboard,Key.DownArrow);Assert.That(host.Hud.SelectedActionId,Is.EqualTo("buy.hp1"));
                yield return Tap(keyboard,Key.Enter);Assert.That(host.Session.Stage,Is.EqualTo(1));yield return Tap(keyboard,Key.Escape);Assert.That(host.Hud.Panel,Is.EqualTo("npc"));yield return Tap(keyboard,Key.Escape);yield return Tap(keyboard,Key.F);
                Assert.That(host.Session.Stage,Is.EqualTo(2));
                host.ResetPrototype(PrototypeStart.FirstLoot);var armor=host.Session.NewItem("armor1");host.Session.Player.Inventory.Add(new[]{armor});yield return new WaitForFixedUpdate();
                yield return Tap(keyboard,Key.I);Assert.That(host.Hud.SelectedActionId,Is.EqualTo("item."+armor.Instance));yield return Tap(keyboard,Key.Enter);Assert.That(host.Hud.Panel,Is.EqualTo("item"));
                Assert.That(host.Hud.SelectedActionId,Is.EqualTo("equip."+armor.Instance));yield return Tap(keyboard,Key.Enter);
                Assert.That(host.Session.Player.Inventory.Equipment.ContainsKey(GearSlot.Armor),Is.True);Assert.That(host.Hud.Panel,Is.EqualTo("bag"));yield return Tap(keyboard,Key.Escape);
                yield return Tap(keyboard,Key.C);Assert.That(host.Hud.Panel,Is.EqualTo("equipment"));
                yield return Tap(keyboard,Key.Tab);Assert.That(host.Hud.Panel,Is.EqualTo("attributes"));
                yield return Tap(keyboard,Key.Tab);Assert.That(host.Hud.Panel,Is.EqualTo("stats"));
                yield return Tap(keyboard,Key.Tab);Assert.That(host.Hud.Panel,Is.EqualTo("skills"));
                yield return Tap(keyboard,Key.Tab);Assert.That(host.Hud.Panel,Is.EqualTo("bag"));
                yield return Tap(keyboard,Key.Tab);Assert.That(host.Hud.Panel,Is.EqualTo("equipment"));Assert.That(host.Hud.SelectedActionId,Is.EqualTo("slot.Weapon"));
                yield return Tap(keyboard,Key.Enter);Assert.That(host.Hud.SelectedActionId,Is.EqualTo("unequip.Weapon"));yield return Tap(keyboard,Key.Enter);
                Assert.That(host.Session.Player.Inventory.Equipment.ContainsKey(GearSlot.Weapon),Is.False);yield return Tap(keyboard,Key.Escape);yield return Tap(keyboard,Key.Escape);
                host.Session.HurtPlayer(9999);yield return Tap(keyboard,Key.F8);yield return Tap(keyboard,Key.DownArrow);yield return Tap(keyboard,Key.Enter);
                Assert.That(host.Hud.Panel,Is.EqualTo("confirm"));Assert.That(host.Hud.SelectedActionId,Is.EqualTo("debug.cancel"));
                yield return Tap(keyboard,Key.DownArrow);yield return Tap(keyboard,Key.Enter);Assert.That(host.Session.Quest,Is.EqualTo(1));Assert.That(host.Session.DebugPreset,Is.Null);Assert.That(host.Session.Player.Inventory.Equipment,Is.Empty);
            }
            finally {InputSystem.RemoveDevice(keyboard);InputSystem.settings.backgroundBehavior=oldBackground;InputSystem.settings.editorInputBehaviorInPlayMode=oldEditor;Object.Destroy(go);}
            yield return null;
        }
        [UnityTest] public IEnumerator KeyboardAliasesModalCancellationAndAirCastFixture() {
            var go=new GameObject("Input + air physics fixture");var host=go.AddComponent<SliceHost>();
            var oldBackground=InputSystem.settings.backgroundBehavior;
            var oldEditorBehavior=InputSystem.settings.editorInputBehaviorInPlayMode;
            InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
            InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            var keyboard=InputSystem.AddDevice<Keyboard>();keyboard.MakeCurrent();
            try {
                // Isolated fixture, never used by the continuous Q1-Q6 acceptance route.
                host.Session.Player.Position=new Point(-4,.8);Assert.That(host.Session.TryExit("toAcademy"),Is.True);
                yield return new WaitForFixedUpdate();yield return null;
                var s=host.Session;s.Player.School=School.Sword;s.Player.Level=5;
                s.Player.Inventory.Equipment[GearSlot.Weapon]=s.NewItem("sword1");s.Combat.Unlocked[1]=Rules.Sword1;
                host.Body.position=new Vector2(21,.72f);host.Body.linearVelocity=Vector2.zero;
                yield return new WaitForFixedUpdate();yield return new WaitForFixedUpdate();
                InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.A,Key.LeftArrow));yield return null;yield return null;
                Assert.That(keyboard.aKey.isPressed,Is.True,"Queued device state must reach input adapter");for(int step=0;step<8;step++)yield return new WaitForFixedUpdate();
                Assert.That(host.Body.linearVelocity.x,Is.EqualTo(-SliceHost.RunSpeed).Within(.01));
                InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;yield return new WaitForFixedUpdate();
                host.Hud.Toggle("bag");InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Digit1));yield return null;yield return new WaitForFixedUpdate();
                Assert.That(s.Combat.StartedCount,Is.Zero);host.Hud.Close();yield return null;yield return new WaitForFixedUpdate();
                Assert.That(s.Combat.StartedCount,Is.Zero,"Closing modal must not resume held skill");
                InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;
                InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.UpArrow,Key.D));yield return null;yield return new WaitForFixedUpdate();yield return new WaitForFixedUpdate();
                // Place a reachable airborne target in this isolated fixture, not beyond vertical bounds.
                var airborne=s.Mobs.Find(x=>x.Dummy);airborne.Position=new Point(host.Body.position.x+.7,host.Body.position.y);
                InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Digit1,Key.D));yield return null;yield return new WaitForFixedUpdate();
                Assert.That(s.Combat.StartedCount,Is.EqualTo(1),s.Feedback+"; body="+host.Body.position);Assert.That(s.Combat.Running.Skill.Id,Is.EqualTo("sword.s1"));
                Assert.That(s.Combat.Running.Origin.Y,Is.GreaterThan(.9),"Skill must actually start airborne in this fixture");
                Assert.That(host.Body.linearVelocity.y,Is.GreaterThan(0),"Action must not freeze gravity/jump momentum");
                Assert.That(host.Body.linearVelocity.x,Is.GreaterThan(0).And.LessThanOrEqualTo(SliceHost.RunSpeed+.01),"Cast preserves movement while acceleration is tuned");
                InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;yield return new WaitForFixedUpdate();
                Assert.That(host.Body.linearVelocity.y,Is.GreaterThan(0));Assert.That(Resources.Load<Font>("Fonts/DejaVuSans"),Is.Not.Null);
                Assert.That(host.Hud.IsOverUi(new Vector2(Screen.width-100,Screen.height/2)),Is.False,"Quest HUD must not block world clicks down the whole right column");
            }
            finally {InputSystem.RemoveDevice(keyboard);InputSystem.settings.backgroundBehavior=oldBackground;InputSystem.settings.editorInputBehaviorInPlayMode=oldEditorBehavior;Object.Destroy(go);}
            yield return null;
        }
    }
}
