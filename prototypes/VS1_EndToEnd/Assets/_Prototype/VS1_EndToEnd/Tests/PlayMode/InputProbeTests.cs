using System.Collections;
using System.Linq;
using HuyenLo.Domain;
using HuyenLo.Runtime;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

namespace HuyenLo.Tests
{
    // Virtual InputSystem devices verify the adapter, NOT physical keyboard rollover or feel.
    public sealed class InputProbeTests
    {
        private GameObject root;
        private SliceHost host;
        private Keyboard keyboard;
        private InputSettings.BackgroundBehavior background;
        private InputSettings.EditorInputBehaviorInPlayMode editor;
        [UnitySetUp] public IEnumerator Setup(){
            background=InputSystem.settings.backgroundBehavior;editor=InputSystem.settings.editorInputBehaviorInPlayMode;
            InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
            InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            keyboard=InputSystem.AddDevice<Keyboard>();keyboard.MakeCurrent();
            root=new GameObject("Disposable InputProbes fixture");host=root.AddComponent<SliceHost>();
            foreach(var f in typeof(ProbeConfig).GetFields())if(f.FieldType==typeof(bool))f.SetValue(host.Session.Probes,true);
            host.ResetPrototype(PrototypeStart.SwordTraining);host.Session.Player.InvulnerableUntil=100;
            host.Body.position=new Vector2(19.3f,.72f);host.Body.transform.position=host.Body.position;
            host.Body.linearVelocity=Vector2.zero;Physics2D.SyncTransforms();
            for(int i=0;i<4;i++)yield return new WaitForFixedUpdate();
        }
        [UnityTearDown] public IEnumerator Cleanup(){
            InputSystem.RemoveDevice(keyboard);InputSystem.settings.backgroundBehavior=background;
            InputSystem.settings.editorInputBehaviorInPlayMode=editor;Object.Destroy(root);yield return null;
        }
        private IEnumerator State(params Key[] keys){InputSystem.QueueStateEvent(keyboard,new KeyboardState(keys));yield return null;yield return new WaitForFixedUpdate();}
        private IEnumerator Tap(params Key[] keys){yield return State();yield return State(keys);yield return State();}
        private IEnumerator Pending(params Key[] held){
            yield return State(held);yield return State(held.Concat(new[]{Key.Digit1}).ToArray());
            Assert.That(host.Session.Combat.HasPendingCast,Is.True,"Fixture must begin outside ExecutionRange");
        }
        [UnityTest] public IEnumerator HeldAxisBothDirectionsPermitArrivalThenResume(){
            foreach(var key in new[]{Key.D,Key.A}){
                host.ResetPrototype(PrototypeStart.SwordTraining);host.Session.Player.InvulnerableUntil=100;
                host.Body.position=new Vector2(19.3f,.72f);host.Body.transform.position=host.Body.position;host.Body.linearVelocity=Vector2.zero;Physics2D.SyncTransforms();
                for(int i=0;i<3;i++)yield return new WaitForFixedUpdate();
                yield return Pending(key);yield return State(key);
                int guard=0;while(host.Session.Combat.HasPendingCast){Assert.That(++guard,Is.LessThan(90));yield return new WaitForFixedUpdate();}
                Assert.That(host.Session.Combat.StartedCount,Is.EqualTo(1));
                for(int i=0;i<15;i++)yield return new WaitForFixedUpdate();
                Assert.That(Mathf.Sign(host.Body.linearVelocity.x),Is.EqualTo(key==Key.D?1:-1),"Held axis resumes after pending ends");
                yield return State();
            }
        }
        [UnityTest] public IEnumerator NewDirectionJumpDropAndUiCancelPending(){
            foreach(var key in new[]{Key.LeftArrow,Key.Space,Key.S,Key.I}){
                host.ResetPrototype(PrototypeStart.SwordTraining);host.Session.Player.InvulnerableUntil=100;
                host.Body.position=new Vector2(19.3f,.72f);host.Body.transform.position=host.Body.position;host.Body.linearVelocity=Vector2.zero;Physics2D.SyncTransforms();
                for(int i=0;i<3;i++)yield return new WaitForFixedUpdate();
                yield return Pending(Key.D);yield return State(Key.D,key);
                Assert.That(host.Session.Combat.HasPendingCast,Is.False,key+" must cancel");
                Assert.That(host.Session.Combat.StartedCount,Is.Zero);
                host.Hud.Close();yield return State();
            }
        }
        [UnityTest] public IEnumerator EscapeClosesModalBeforeClearingFocusThenPendingBeforeFocus(){
            var target=host.Session.Mobs.First(x=>x.Dummy);host.Session.Combat.Explicit(target);
            host.Hud.Toggle("bag");yield return Tap(Key.Escape);
            Assert.That(host.Hud.Panel,Is.Null.Or.Empty);Assert.That(host.Session.Combat.FocusId,Is.EqualTo(target.Id));
            yield return Pending(Key.D);yield return State(Key.D,Key.Escape);
            Assert.That(host.Session.Combat.HasPendingCast,Is.False);Assert.That(host.Session.Combat.FocusId,Is.EqualTo(target.Id));
            yield return Tap(Key.Escape);Assert.That(host.Session.Combat.FocusKind,Is.EqualTo(FocusKind.None));
            yield return Tap(Key.Escape);Assert.That(host.Hud.Panel,Is.Null.Or.Empty);
        }
        [UnityTest] public IEnumerator TabCyclesExplicitWorldFocusAndModalTabOnlyNavigatesUi(){
            host.Session.Player.Position=new Point(23,.8);host.Body.position=new Vector2(23,.72f);
            yield return Tap(Key.Tab);long first=host.Session.Combat.FocusId;
            Assert.That(first,Is.Not.Zero);Assert.That(host.Session.Combat.FocusKind,Is.EqualTo(FocusKind.Explicit));
            yield return Tap(Key.Tab);Assert.That(host.Session.Combat.FocusId,Is.Not.EqualTo(first));
            yield return Tap(Key.LeftShift,Key.Tab);Assert.That(host.Session.Combat.FocusId,Is.EqualTo(first));
            host.Hud.Toggle("equipment");yield return Tap(Key.Tab);
            Assert.That(host.Hud.Panel,Is.EqualTo("attributes"));Assert.That(host.Session.Combat.FocusId,Is.EqualTo(first));
            yield return Tap(Key.LeftShift,Key.Tab);Assert.That(host.Hud.Panel,Is.EqualTo("equipment"));
        }
        [UnityTest] public IEnumerator FourFiveArePotionAliasesWithModalAndFullResourceGuards(){
            var s=host.Session;s.Player.Inventory.Add(new[]{s.NewItem("hp1",3),s.NewItem("mp1",3)});
            s.Player.Hp-=40;s.Player.Mp-=30;double hp=s.Player.Hp,mp=s.Player.Mp;int slot=s.Combat.SelectedSlot;
            yield return Tap(Key.Digit4);Assert.That(s.Player.Hp,Is.GreaterThan(hp));
            yield return Tap(Key.Digit5);Assert.That(s.Player.Mp,Is.GreaterThan(mp));
            Assert.That(s.Combat.SelectedSlot,Is.EqualTo(slot));Assert.That(s.Combat.StartedCount,Is.Zero);
            Assert.That(host.QuickKey("H"),Is.False,"Same cooldown as4");Assert.That(host.QuickKey("M"),Is.False,"Same cooldown as5");
            s.HpPotionUntil=s.MpPotionUntil=0;s.Player.Hp=s.Player.Stats.Hp;s.Player.Mp=s.Player.Stats.Mp;
            Assert.That(host.QuickKey("4"),Is.False);Assert.That(host.QuickKey("5"),Is.False);
            s.Player.Hp-=40;s.Player.Mp-=30;host.Hud.Toggle("bag");hp=s.Player.Hp;mp=s.Player.Mp;
            yield return Tap(Key.Digit4);yield return Tap(Key.Digit5);Assert.That(s.Player.Hp,Is.EqualTo(hp));Assert.That(s.Player.Mp,Is.EqualTo(mp));
            host.Hud.Close();Assert.That(host.QuickKey("H"),Is.True);Assert.That(host.QuickKey("M"),Is.True);
            Assert.That(ProbeBindings.HpGlyph(s.Probes),Is.EqualTo("4/H"));Assert.That(ProbeBindings.MpGlyph(s.Probes),Is.EqualTo("5/M"));
        }
        [UnityTest] public IEnumerator StaleHeldKeyCannotCarryApproachThroughEdgeExit(){
            // Capture held D in a safe lane first, then author the exit fixture without a new KeyDown.
            yield return State(Key.D);
            host.ResetPrototype(PrototypeStart.Crowd);var s=host.Session;s.Player.InvulnerableUntil=100;
            foreach(var m in s.Mobs)m.Map=Map.Academy;
            var target=s.Mobs.First(x=>x.Dummy);target.Map=Map.Mist;target.Position=new Point(-5.3,.8);
            host.Body.position=new Vector2(-2.9f,.72f);host.Body.transform.position=host.Body.position;host.Body.linearVelocity=Vector2.zero;Physics2D.SyncTransforms();
            s.Player.Position=new Point(-2.9,.8);s.Combat.Explicit(target);
            yield return State(Key.D,Key.Digit1);
            Assert.That(s.Combat.LastReject,Is.EqualTo(ActionRejectReason.Blocked));Assert.That(s.Combat.HasPendingCast,Is.False);
            for(int i=0;i<15;i++)yield return new WaitForFixedUpdate();
            Assert.That(s.Player.Map,Is.EqualTo(Map.Mist));Assert.That(s.Combat.StartedCount,Is.Zero);Assert.That(s.Combat.Cooldowns,Is.Empty);
        }
    }
}
