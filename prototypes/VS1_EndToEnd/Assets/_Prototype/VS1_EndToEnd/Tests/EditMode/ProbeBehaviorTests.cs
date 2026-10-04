using System;
using System.Linq;
using HuyenLo.Domain;
using NUnit.Framework;

namespace HuyenLo.Tests
{
    public sealed class ProbeBehaviorTests
    {
        private static SliceSession Fixture(Skill skill=null) {
            var s=new SliceSession(731,ProbeConfig.Experimental());s.Player.Map=Map.Academy;s.Player.School=School.Sword;s.Player.Level=5;
            s.Player.Position=new Point(18.9,.8);s.Player.Inventory.Equipment[GearSlot.Weapon]=s.NewItem("sword1");s.Player.Mp=100;
            foreach(var m in s.Mobs)m.Map=Map.Mist;
            var target=s.Mobs.First(x=>x.Dummy);target.Map=Map.Academy;target.Position=new Point(22,.8);
            s.Combat.Unlocked[1]=skill??Rules.Sword1;s.Combat.Unlocked[2]=Rules.Sword2;s.Combat.Unlocked[3]=Rules.Sword3;return s;
        }
        private static void Advance(SliceSession s,double seconds,bool move=false) {
            for(int i=0;i<(int)Math.Ceiling(seconds/.02);i++){
                if(move)s.ObservePosition(new Point(s.Player.Position.X+s.Combat.AssistAxis*.1,s.Player.Position.Y),true,false,false,false);
                s.Tick(.02);
            }
        }
        [Test] public void DefaultsAndClassSkillsKeepCanonicalNumbers() {
            var s=new SliceSession();Assert.That(s.Probes.Any,Is.False);Assert.That(s.Combat.Selected,Is.SameAs(Rules.Novice));
            var p=new SliceSession(probes:ProbeConfig.Experimental());Assert.That(p.Combat.Selected,Is.SameAs(Rules.NoviceProbe));
            Assert.That(Rules.Novice.Cooldown,Is.EqualTo(1));Assert.That(Rules.NoviceProbe.HitDelay,Is.EqualTo(.10));
            Assert.That(Rules.Sword1.Cooldown,Is.EqualTo(1));Assert.That(Rules.Sword2.Cooldown,Is.EqualTo(1.5));Assert.That(Rules.Sword3.Cooldown,Is.EqualTo(7));
            Assert.That(s.Mobs.Where(m=>m.Name=="Sói Sương").All(m=>m.Speed==2.4&&m.Range==1&&m.MaxHp==107),Is.True);
        }
        [TestCase(.18,true)] [TestCase(.181,false)] [TestCase(2,false)]
        public void BufferAcceptsOnlySoonReadySkill(double remaining,bool accepted) {
            var s=Fixture();s.Player.Position=new Point(21,.8);s.Combat.Cooldowns[Rules.Sword1.Id]=remaining;
            s.Combat.Press("1");Assert.That(s.Combat.HasBufferedCast,Is.EqualTo(accepted));Assert.That(s.Combat.HasPendingCast,Is.False);
            Assert.That(s.Combat.AssistAxis,Is.Zero);Assert.That(s.Combat.StartedCount,Is.Zero);
            Advance(s,.2);Assert.That(s.Combat.StartedCount,Is.EqualTo(accepted?1:0));
        }
        [Test] public void BufferKeepsLatestAndNeverApproachesDuringLock() {
            var s=Fixture();s.Player.Position=new Point(21,.8);s.Combat.Press("1");var running=s.Combat.Running;Advance(s,.16);
            s.Combat.Press("2");s.Combat.Press("3");Assert.That(s.Combat.HasPendingCast,Is.False);Assert.That(s.Combat.AssistAxis,Is.Zero);
            Assert.That(s.Combat.Running,Is.SameAs(running));Advance(s,.18);Assert.That(s.Combat.StartedCount,Is.EqualTo(2));Assert.That(s.Combat.Running.Skill,Is.SameAs(Rules.Sword3));
        }
        [TestCase(ActionRejectReason.PlayerDead)] [TestCase(ActionRejectReason.HardCc)]
        [TestCase(ActionRejectReason.TargetMissing)] [TestCase(ActionRejectReason.TargetDead)]
        [TestCase(ActionRejectReason.TargetGeneration)] [TestCase(ActionRejectReason.TargetReturning)]
        [TestCase(ActionRejectReason.WrongMap)] [TestCase(ActionRejectReason.NotLearned)]
        [TestCase(ActionRejectReason.WeaponRequired)] [TestCase(ActionRejectReason.InsufficientMp)]
        [TestCase(ActionRejectReason.Cooldown)] [TestCase(ActionRejectReason.OutOfRange)]
        public void ArrivalFailureHasEnumAndCommitsNoCost(ActionRejectReason expected) {
            var s=Fixture();s.Combat.Press("1");Assert.That(s.Combat.HasPendingCast,Is.True);double mp=s.Player.Mp;var target=s.Combat.Focus;
            switch(expected){
                case ActionRejectReason.PlayerDead:s.Player.Hp=0;break;
                case ActionRejectReason.HardCc:s.Player.HardCc=true;break;
                case ActionRejectReason.TargetMissing:s.Mobs.Remove(target);break;
                case ActionRejectReason.TargetDead:target.Hp=0;target.RespawnAt=1000;break;
                case ActionRejectReason.TargetGeneration:target.Generation++;break;
                case ActionRejectReason.TargetReturning:target.Returning=true;break;
                case ActionRejectReason.WrongMap:target.Map=Map.Mist;break;
                case ActionRejectReason.NotLearned:s.Combat.Unlocked.Remove(1);break;
                case ActionRejectReason.WeaponRequired:s.Player.Inventory.Equipment.Remove(GearSlot.Weapon);break;
                case ActionRejectReason.InsufficientMp:s.Player.Mp=1;mp=1;break;
                case ActionRejectReason.Cooldown:s.Combat.Cooldowns[Rules.Sword1.Id]=100;break;
                case ActionRejectReason.OutOfRange:target.Position=new Point(22,4);break;
            }
            Advance(s,.02);Assert.That(s.Combat.LastReject,Is.EqualTo(expected));Assert.That(s.Combat.StartedCount,Is.Zero);Assert.That(s.Combat.HasPendingCast,Is.False);Assert.That(s.Player.Mp,Is.EqualTo(mp));
            Assert.That(s.Combat.Cooldowns.Count,Is.EqualTo(expected==ActionRejectReason.Cooldown?1:0));
        }
        [Test] public void PendingArrivalUsesActualOriginAndGeneration() {
            var s=Fixture();s.Combat.Press("1");Advance(s,.4,true);
            Assert.That(s.Combat.StartedCount,Is.EqualTo(1));Assert.That(s.Combat.Running.Origin.X,Is.GreaterThan(18.9));Assert.That(s.Combat.Running.Targets[0].Generation,Is.EqualTo(s.Combat.Focus.Generation));
        }
        [Test] public void ApproachExitIsBlockedBeforeCostAndOnlyManualPositionCanExit() {
            var s=Fixture();s.Player.Map=Map.Mist;foreach(var m in s.Mobs)m.Map=Map.Academy;
            var target=s.Mobs[0];target.Map=Map.Mist;target.Position=new Point(-5.3,.8);s.Player.Position=new Point(-2.9,.8);
            s.Combat.Explicit(target);s.Combat.Press("1");Assert.That(s.Combat.LastReject,Is.EqualTo(ActionRejectReason.Blocked));Assert.That(s.Combat.HasPendingCast,Is.False);Assert.That(s.Combat.Cooldowns,Is.Empty);
            s.ObservePosition(new Point(-4,.8),true,false,false,false);Assert.That(s.Player.Map,Is.EqualTo(Map.Mist));
            s.ObservePosition(new Point(-4,.8),true,false,false,true);Assert.That(s.Player.Map,Is.EqualTo(Map.Village));
        }
        [Test] public void StaleHeldAxisIsIgnoredIncludingOppositeDirectionThenReturns() {
            var s=Fixture();var held=new HeldAxisProbe();held.Snapshot(1);s.Combat.Press("1");
            Assert.That(s.Combat.AssistAxis,Is.EqualTo(1));Assert.That(held.Axis(1,s.Combat.HasPendingCast),Is.Zero);
            Advance(s,.4,true);Assert.That(s.Combat.StartedCount,Is.EqualTo(1));Assert.That(held.Axis(1,s.Combat.HasPendingCast),Is.EqualTo(-1));
        }
        [Test] public void EscCancelsPendingThenFocusThenDoesNothing() {
            var s=Fixture();s.Combat.Press("1");int focus=s.Combat.FocusId;s.Combat.Escape();Assert.That(s.Combat.HasPendingCast,Is.False);Assert.That(s.Combat.FocusId,Is.EqualTo(focus));
            s.Combat.Escape();Assert.That(s.Combat.FocusKind,Is.EqualTo(FocusKind.None));s.Combat.Escape();Assert.That(s.Combat.StartedCount,Is.Zero);
        }
        [Test] public void CycleIsStableWrappedReversibleAndExplicit() {
            var s=Fixture();s.Player.Position=new Point(22,.8);
            for(int i=0;i<3;i++){var m=s.Mobs[i];m.Map=Map.Academy;m.Position=new Point(i==0?21:23,.8);}
            var order=s.Mobs.Take(3).OrderBy(x=>Math.Abs(x.Position.X-22)).ThenBy(x=>x.Slot,StringComparer.Ordinal).ToArray();
            for(int i=0;i<4;i++){Assert.That(s.Combat.CycleTarget(1),Is.True);Assert.That(s.Combat.FocusId,Is.EqualTo(order[i%3].Id));Assert.That(s.Combat.FocusKind,Is.EqualTo(FocusKind.Explicit));}
            s.Combat.CycleTarget(-1);Assert.That(s.Combat.FocusId,Is.EqualTo(order[2].Id));
            var focus=s.Combat.Focus;s.Mobs[0].Position=new Point(22,.8);s.Tick(.02,true);Assert.That(s.Combat.FocusId,Is.EqualTo(focus.Id));
            focus.Generation++;s.Tick(.02);Assert.That(s.Combat.FocusKind,Is.EqualTo(FocusKind.None));
        }
        [Test] public void RangedFixtureUsesSameApproachSnapshotAndBatchPipeline() {
            var ranged=new Skill("fixture.ranged", "Fixture only",1,2,6.5,.9,1.7,.12,.34,Shape.Spread,3);
            var s=Fixture(ranged);s.Player.Position=new Point(14,.8);s.Combat.Press("1");Assert.That(s.Combat.HasPendingCast,Is.True);
            Advance(s,.34,true);var action=s.Combat.Running;Assert.That(action,Is.Not.Null);Assert.That(action.Skill.Range,Is.EqualTo(6.5));Assert.That(action.Targets.Length,Is.EqualTo(3));
            var key=action.Targets[0];s.Combat.Focus.Generation++;Advance(s,.2);Assert.That(action.Targets[0].Generation,Is.EqualTo(key.Generation));Assert.That(s.Combat.Results,Is.Empty);
        }
        private static (SliceSession session,Mob wolf) Wolf() {
            var s=new SliceSession(731,ProbeConfig.Experimental());s.Player.Map=Map.Mist;s.Player.Level=5;
            var wolf=s.Mobs.First(x=>x.Name=="Sói Sương");foreach(var m in s.Mobs)if(m!=wolf)m.Map=Map.Academy;
            s.Player.Position=new Point(wolf.Home.X+.8,wolf.Home.Y+.07);s.Player.Hp=s.Player.Stats.Hp;return(s,wolf);
        }
        [Test] public void MobBiteDoesNotFlipSideAndWindupIsImmutable() {
            var f=Wolf();var s=f.session;var wolf=f.wolf;
            for(int i=0;i<100&&!wolf.Windup;i++)s.Tick(.02);Assert.That(wolf.Windup,Is.True);
            int side=wolf.ApproachSide,facing=wolf.Facing;var origin=wolf.Position;double hit=wolf.HitAt,next=wolf.NextAttack;
            s.Player.Position=new Point(s.Player.Position.X+.1,s.Player.Position.Y);s.Tick(.02);
            Assert.That(wolf.Position.X,Is.EqualTo(origin.X));Assert.That(wolf.Facing,Is.EqualTo(facing));Assert.That(wolf.HitAt,Is.EqualTo(hit));
            while(s.Now<hit)s.Tick(.02);Assert.That(wolf.ApproachSide,Is.EqualTo(side));Assert.That(wolf.NextAttack,Is.EqualTo(next));
        }
        [Test] public void FreeSpaceUsesSpawnSlotTieAndLifePhaseIgnoresRuntimeIds() {
            var f=Wolf();int side=f.session.ProbeFreeSide(f.wolf);double phase=SliceSession.StablePhase(f.wolf);f.wolf.Id+=1001;
            Assert.That(f.session.ProbeFreeSide(f.wolf),Is.EqualTo(side));Assert.That(SliceSession.StablePhase(f.wolf),Is.EqualTo(phase));
            var phases=f.session.Mobs.Where(x=>x.Name=="Sói Sương").Take(3).Select(SliceSession.StablePhase).Distinct().Count();Assert.That(phases,Is.GreaterThan(1));
            var peer=f.session.Mobs.First(x=>x!=f.wolf);peer.Map=Map.Mist;peer.Dummy=false;peer.Home=new Point(f.wolf.Home.X,f.wolf.Home.Y);peer.Position=new Point(f.session.Player.Position.X-f.wolf.Range*.9,f.wolf.Home.Y);
            Assert.That(f.session.ProbeFreeSide(f.wolf),Is.EqualTo(1));
        }
        [Test] public void SideSwitchIsLockedForOnePointFiveSeconds() {
            var f=Wolf();var s=f.session;var m=f.wolf;s.Tick(.02);int side=m.ApproachSide;double unlock=m.SideLockedUntil;
            m.Windup=false;m.NextAttack=100; s.Player.Position=new Point(m.Position.X-side*.6,m.Position.Y);
            s.Tick(.02);Assert.That(m.ApproachSide,Is.EqualTo(side));Assert.That(m.SideLockedUntil,Is.EqualTo(unlock));
            m.SideSwitchPending=true;while(s.Now<unlock-.02){m.Position=new Point(m.Home.X,m.Home.Y);s.Tick(.02);Assert.That(m.ApproachSide,Is.EqualTo(side));}
        }
        [Test] public void MushroomsDoNotInheritWolfRepositionPolicy() {
            var s=new SliceSession(731,ProbeConfig.Experimental());var m=s.Mobs.First(x=>x.Name=="Nấm Linh");Assert.That(m.RepositionAfterHit,Is.False);s.Player.Map=Map.Mist;s.Player.Position=new Point(m.Home.X+.5,m.Home.Y);
            foreach(var other in s.Mobs)if(other!=m)other.Map=Map.Academy;
            for(int i=0;i<120;i++)s.Tick(.02);Assert.That(m.BiteAttempts,Is.GreaterThan(0));Assert.That(m.RepositionUntil,Is.Zero);
        }
    }
}
