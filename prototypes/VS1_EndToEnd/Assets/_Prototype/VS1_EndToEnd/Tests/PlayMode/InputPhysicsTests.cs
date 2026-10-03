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
                Assert.That(host.Session.Quest,Is.EqualTo(5));Assert.That(host.Hud.SelectedActionId,Is.EqualTo("service.buy"));
                yield return Tap(keyboard,Key.Enter);Assert.That(host.Hud.Panel,Is.EqualTo("buy"));Assert.That(host.Hud.SelectedActionId,Is.EqualTo("buy.food1"));
                yield return Tap(keyboard,Key.Enter);yield return Tap(keyboard,Key.DownArrow);Assert.That(host.Hud.SelectedActionId,Is.EqualTo("buy.hp1"));
                yield return Tap(keyboard,Key.Enter);Assert.That(host.Session.Stage,Is.EqualTo(1));yield return Tap(keyboard,Key.Escape);Assert.That(host.Hud.Panel,Is.EqualTo("npc"));yield return Tap(keyboard,Key.Escape);yield return Tap(keyboard,Key.F);
                Assert.That(host.Session.Stage,Is.EqualTo(2));
                host.ResetPrototype(PrototypeStart.FirstLoot);var armor=host.Session.NewItem("armor1");host.Session.Player.Inventory.Add(new[]{armor});yield return new WaitForFixedUpdate();
                yield return Tap(keyboard,Key.I);Assert.That(host.Hud.SelectedActionId,Is.EqualTo("item."+armor.Instance));yield return Tap(keyboard,Key.Enter);Assert.That(host.Hud.Panel,Is.EqualTo("item"));
                Assert.That(host.Hud.SelectedActionId,Is.EqualTo("equip."+armor.Instance));yield return Tap(keyboard,Key.Enter);
                Assert.That(host.Session.Player.Inventory.Equipment.ContainsKey(GearSlot.Armor),Is.True);Assert.That(host.Hud.Panel,Is.EqualTo("bag"));yield return Tap(keyboard,Key.Escape);
                yield return Tap(keyboard,Key.C);yield return Tap(keyboard,Key.Enter);Assert.That(host.Hud.Panel,Is.EqualTo("equipment"));
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
