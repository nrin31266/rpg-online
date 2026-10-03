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
        [UnityTest] public IEnumerator KeyboardAliasesModalCancellationAndAirCastFixture() {
            var go=new GameObject("Input + air physics fixture");var host=go.AddComponent<SliceHost>();
            var oldBackground=InputSystem.settings.backgroundBehavior;
            var oldEditorBehavior=InputSystem.settings.editorInputBehaviorInPlayMode;
            InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
            InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            var keyboard=InputSystem.AddDevice<Keyboard>();keyboard.MakeCurrent();
            try {
                // Isolated fixture, never used by the continuous Q1-Q6 acceptance route.
                host.Session.Player.Position=new Point(25,.8);Assert.That(host.Session.Portal("toAcademy"),Is.True);
                yield return new WaitForFixedUpdate();yield return null;
                var s=host.Session;s.Player.School=School.Sword;s.Player.Level=5;
                s.Player.Inventory.Equipment[GearSlot.Weapon]=s.NewItem("sword1");s.Combat.Unlocked[1]=Rules.Sword1;
                host.Body.position=new Vector2(21,.72f);host.Body.linearVelocity=Vector2.zero;
                yield return new WaitForFixedUpdate();yield return new WaitForFixedUpdate();
                InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.A,Key.LeftArrow));yield return null;yield return null;
                Assert.That(keyboard.aKey.isPressed,Is.True,"Queued device state must reach input adapter");yield return new WaitForFixedUpdate();
                Assert.That(host.Body.linearVelocity.x,Is.EqualTo(-SliceHost.RunSpeed).Within(.01));
                InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;yield return new WaitForFixedUpdate();
                host.Hud.Toggle("bag");InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.J));yield return null;yield return new WaitForFixedUpdate();
                Assert.That(s.Combat.StartedCount,Is.Zero);host.Hud.Close();yield return null;yield return new WaitForFixedUpdate();
                Assert.That(s.Combat.StartedCount,Is.Zero,"Closing modal must not resume held J");
                InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;
                InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Space,Key.D));yield return null;yield return new WaitForFixedUpdate();yield return new WaitForFixedUpdate();
                InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Digit1,Key.D));yield return null;yield return new WaitForFixedUpdate();
                Assert.That(s.Combat.StartedCount,Is.EqualTo(1));Assert.That(s.Combat.Running.Skill.Id,Is.EqualTo("sword.s1"));
                Assert.That(s.Combat.Running.Origin.Y,Is.GreaterThan(.9),"Skill must actually start airborne in this fixture");
                Assert.That(host.Body.linearVelocity.y,Is.GreaterThan(0),"Action must not freeze gravity/jump momentum");
                Assert.That(host.Body.linearVelocity.x,Is.EqualTo(SliceHost.RunSpeed).Within(.01),"Cast must not freeze horizontal movement");
                InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;yield return new WaitForFixedUpdate();
                Assert.That(host.Body.linearVelocity.y,Is.GreaterThan(0));Assert.That(Resources.Load<Font>("Fonts/DejaVuSans"),Is.Not.Null);
                Assert.That(host.Hud.IsOverUi(new Vector2(Screen.width-100,Screen.height/2)),Is.False,"Quest HUD must not block world clicks down the whole right column");
            }
            finally {InputSystem.RemoveDevice(keyboard);InputSystem.settings.backgroundBehavior=oldBackground;InputSystem.settings.editorInputBehaviorInPlayMode=oldEditorBehavior;Object.Destroy(go);}
            yield return null;
        }
    }
}
