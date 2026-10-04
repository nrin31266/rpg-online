using System;
using System.Linq;
using HuyenLo.Domain;
using NUnit.Framework;

namespace HuyenLo.Tests
{
    public sealed class DomainTests
    {
        private SliceSession Fixture(params Skill[] skills) {
            var s=new SliceSession(123);s.Player.School=School.Sword;s.Player.Level=5;s.Player.Map=Map.Academy;s.Player.Position=new Point(21,.8);
            s.Player.Inventory.Equipment[GearSlot.Weapon]=s.NewItem("sword1");s.Player.Mp=100;
            foreach(var skill in skills)s.Combat.Unlocked[skill.Slot]=skill;
            foreach(var m in s.Mobs){m.Dummy=true;m.Hp=m.MaxHp=10000;if(m.Id!=10)m.Map=Map.Mist;}
            return s;
        }
        private void Advance(SliceSession s,double duration){for(int i=0;i<(int)Math.Ceiling(duration/.02);i++)s.Tick(.02);}
        [TestCase(true,true,false,false,-1)] [TestCase(false,false,true,true,1)] [TestCase(true,false,true,false,0)]
        public void AlternateBindingsDoNotDoubleSpeed(bool a,bool left,bool d,bool right,int expected)=>Assert.That(Rules.Axis(a,left,d,right),Is.EqualTo(expected));
        [Test] public void NoTargetCommitsNothing() {
            var s=Fixture(Rules.Sword1);s.Player.Position=new Point(100,.8);double mp=s.Player.Mp;
            s.Combat.Press("1",1);Advance(s,1);Assert.That(s.Combat.StartedCount,Is.Zero);Assert.That(s.Player.Mp,Is.EqualTo(mp));Assert.That(s.Combat.Cooldowns,Is.Empty);Assert.That(s.Combat.AssistAxis,Is.Zero);
        }
        [Test] public void SlotPressExecutesImmediatelyAndSecondPhysicalPressStartsAgain() {
            var s=Fixture(Rules.Sword1);s.Combat.Press("1",1);Assert.That(s.Combat.Running.Skill,Is.SameAs(Rules.Sword1));s.Combat.Release("1");Advance(s,1.1);
            s.Combat.Press("1",1);Assert.That(s.Combat.StartedCount,Is.EqualTo(2));Assert.That(s.Combat.Running.Skill.Id,Is.EqualTo("sword.s1"));
        }
        [Test] public void LockedSlotDoesNotSelectOrStart() {
            var s=Fixture(Rules.Sword1);s.Combat.Press("2",2);Assert.That(s.Combat.SelectedSlot,Is.EqualTo(1));Assert.That(s.Combat.StartedCount,Is.Zero);
        }
        [Test] public void SpamKeepsImmutableActionAndIndependentCooldowns() {
            var s=Fixture(Rules.Sword1,Rules.Sword2,Rules.Sword3);s.Combat.Press("1",1);var action=s.Combat.Running;
            s.Combat.Press("2",2);s.Combat.Press("3",3);
            Assert.That(s.Combat.Running,Is.SameAs(action));Assert.That(action.Skill.Id,Is.EqualTo("sword.s1"));Assert.That(s.Combat.StartedCount,Is.EqualTo(1));
            Assert.That(s.Combat.SelectedSlot,Is.EqualTo(3));Advance(s,.34);Assert.That(s.Combat.StartedCount,Is.LessThanOrEqualTo(2));Assert.That(s.Combat.Cooldowns["sword.s1"],Is.EqualTo(1));
        }
        [Test] public void ShortRecoveryTapBuffersLatestSkillButCooldownTapDoesNotQueue() {
            var s=Fixture(Rules.Sword1,Rules.Sword2);s.Combat.Press("1",1);s.Combat.Release("1");Advance(s,.22);
            s.Combat.Press("2",2);s.Combat.Release("2");Assert.That(s.Combat.StartedCount,Is.EqualTo(1));Advance(s,.12);
            Assert.That(s.Combat.StartedCount,Is.EqualTo(2));Assert.That(s.Combat.Running.Skill.Id,Is.EqualTo("sword.s2"));
            s.Combat.Cancel();s.Combat.Press("1",1);s.Combat.Release("1");Advance(s,1);
            Assert.That(s.Combat.StartedCount,Is.EqualTo(2),"A tap rejected by cooldown must not wait as a long queue");
        }
        [Test] public void UltimateHoldConsumesOnePhysicalPress() {
            var s=Fixture(Rules.Sword3);s.Combat.Press("3",3);Advance(s,15);Assert.That(s.Combat.StartedCount,Is.EqualTo(1));
            s.Combat.Release("3");s.Combat.Press("3",3);Assert.That(s.Combat.StartedCount,Is.EqualTo(2));
        }
        [Test] public void RejectedPressDoesNotCreateLongQueue() {
            var s=Fixture(Rules.Sword1,Rules.Sword2);s.Combat.Press("1",1);s.Combat.Press("2",2);s.Combat.Release("2");Advance(s,3);Assert.That(s.Combat.StartedCount,Is.EqualTo(1));
        }
        [Test] public void AutoFocusStickyThenDeathReacquires() {
            var s=Fixture(Rules.Sword1);var a=s.Mobs[0];a.Position=new Point(22,.8);var b=s.Mobs[1];b.Map=Map.Academy;b.Position=new Point(23,.8);
            s.Combat.Press("1",1);s.Combat.Release("1");Assert.That(s.Combat.FocusId,Is.EqualTo(a.Id));b.Position=new Point(21.1,.8);s.Tick(.02);Assert.That(s.Combat.FocusId,Is.EqualTo(a.Id));
            a.Hp=0;a.RespawnAt=100;s.Tick(.02);Assert.That(s.Combat.FocusId,Is.EqualTo(b.Id));
        }
        [Test] public void ExplicitFocusRetainsWithinEnvelopeThenReturnsToAuto() {
            var s=Fixture(Rules.Sword1);var a=s.Mobs[0];s.Combat.Press("1",1);s.Combat.Release("1");
            var b=s.Mobs[1];b.Map=Map.Academy;b.Position=new Point(30,.8);s.Player.Position=new Point(29,.8);s.Tick(.02,true);Assert.That(s.Combat.FocusId,Is.EqualTo(b.Id));
            a.Position=new Point(27,.8);s.Combat.Explicit(a);s.Tick(.02,true);Assert.That(s.Combat.FocusId,Is.EqualTo(a.Id));Assert.That(s.Combat.FocusKind,Is.EqualTo(FocusKind.Explicit));
            a.Position=new Point(22,.8);s.Tick(.02);Assert.That(s.Combat.FocusId,Is.EqualTo(b.Id));Assert.That(s.Combat.FocusKind,Is.EqualTo(FocusKind.Auto));
        }
        [Test] public void FacingIndependentAcquireAndExplicitOutOfRangeDoesNotSubstitute() {
            var s=Fixture(Rules.Sword1);s.Player.Position=new Point(23,.8);s.Combat.Facing=1;s.Combat.Press("1",1);Assert.That(s.Combat.Running.Facing,Is.EqualTo(-1));s.Combat.Cancel();
            var a=s.Mobs[0];a.Position=new Point(26,.8);var b=s.Mobs[1];b.Map=Map.Academy;b.Position=new Point(23.5,.8);s.Combat.Explicit(a);double mp=s.Player.Mp;
            s.Combat.Press("1",1);Assert.That(s.Combat.Running,Is.Null);Assert.That(s.Combat.FocusId,Is.EqualTo(a.Id));Assert.That(s.Player.Mp,Is.EqualTo(mp));
        }
        [Test] public void ApproachIsBoundedHorizontalAndManualCancels() {
            var s=Fixture(Rules.Sword1);s.Player.Position=new Point(19,.8);s.Combat.Press("1",1);Advance(s,.22);Assert.That(s.Combat.AssistAxis,Is.EqualTo(1));
            s.Combat.ManualOverride();Assert.That(s.Combat.AssistAxis,Is.Zero);Advance(s,2);Assert.That(s.Combat.StartedCount,Is.Zero);
            s.Combat.Press("1",1);Advance(s,1);Assert.That(s.Combat.AssistAxis,Is.Zero); // blocked progress times out
        }
        [Test] public void SourceSnapshotAndGenerationAreImmutable() {
            var s=Fixture(Rules.Sword1);s.Combat.Press("1",1);var action=s.Combat.Running;double atk=action.Atk;s.Player.Str=500;
            s.Mobs[0].Generation++;Advance(s,.2);Assert.That(action.Atk,Is.EqualTo(atk));Assert.That(s.Combat.Results,Is.Empty);
        }
        [Test] public void DeathCancelsUnresolvedWithoutRefund() {
            var s=Fixture(Rules.Sword1);double mp=s.Player.Mp;s.Combat.Press("1",1);s.HurtPlayer(9999);Advance(s,.4);s.Combat.Explicit(s.Mobs[0]);
            Assert.That(s.Combat.FocusKind,Is.EqualTo(FocusKind.None));
            Assert.That(s.Combat.Results,Is.Empty);Assert.That(s.Combat.Running,Is.Null);Assert.That(s.Player.Mp,Is.EqualTo(mp-2));Assert.That(s.Combat.Cooldowns,Is.Not.Empty);
        }
        [Test] public void NormalDamageDoesNotInterruptAction() {
            var s=Fixture(Rules.Sword1);s.Combat.Press("1",1);var action=s.Combat.Running;s.HurtPlayer(1);Assert.That(s.Combat.Running,Is.SameAs(action));Advance(s,.2);Assert.That(s.Combat.Results.Count,Is.EqualTo(1));
        }
        [Test] public void RangedBatchResolvesExactlyOnceNoPresentationCallback() {
            var s=Fixture(Rules.Bow2);s.Combat.Press("2",2);s.Combat.Release("2");Advance(s,.5);
            Assert.That(s.Combat.Results.Count,Is.EqualTo(3));Assert.That(s.Combat.Results.Select(x=>x.Clock).Distinct().Count(),Is.EqualTo(1));Advance(s,2);Assert.That(s.Combat.Results.Count,Is.EqualTo(3));
            Assert.That(s.Combat.Results.Select(x=>x.Index),Is.EqualTo(new[]{0,1,2}));
        }
        [Test] public void MultiHitLethalFirstIndexCannotCreditSameDeathAgain() {
            var s=Fixture(Rules.Bow2);s.Mobs[0].Hp=s.Mobs[0].MaxHp=1;
            s.Combat.Press("2",2);s.Combat.Release("2");Advance(s,.5);
            Assert.That(s.Combat.Results.Count,Is.EqualTo(1));Assert.That(s.Combat.Results.Single().Damage,Is.EqualTo(1));
            Assert.That(s.Receipts.Count(x=>x.StartsWith("death.")),Is.EqualTo(1));
        }
        [Test] public void PlayerMovingAfterCastDoesNotDragOrigin() {
            var s=Fixture(Rules.Sword1);s.Combat.Press("1",1);s.Player.Position=new Point(50,.8);Advance(s,.2);Assert.That(s.Combat.Results.Count,Is.EqualTo(1));
        }
        [Test] public void LootImmediateIndependentAndNextCandidate() {
            var s=Fixture(Rules.Sword1);s.Combat.Explicit(s.Mobs[0]);int id=s.Combat.FocusId;
            foreach(var item in new[]{s.NewItem("fang"),s.NewItem("stone")})s.Loot.Add(new Loot{Id=item.Instance,Item=item,Map=s.Player.Map,Position=s.Player.Position,Created=s.Now,MobLevel=4,LevelAtDeath=5});
            var first=s.LootCandidate();Assert.That(s.PickUp(first.Id),Is.True);Assert.That(s.Combat.FocusId,Is.EqualTo(id));Assert.That(s.LootCandidate().Id,Is.Not.EqualTo(first.Id));Assert.That(s.PickUp(first.Id),Is.False);
        }
        [Test] public void FullBagRejectDoesNotConsumeGroundAndStackMergeStillFits() {
            var s=Fixture(Rules.Sword1);s.Player.Inventory.Capacity=1;s.Player.Inventory.Add(new[]{s.NewItem("fang",98)});
            var item=s.NewItem("stone");var l=new Loot{Id=item.Instance,Item=item,Map=s.Player.Map,Position=s.Player.Position,LevelAtDeath=5,MobLevel=4};s.Loot.Add(l);
            Assert.That(s.PickUp(l.Id),Is.False);Assert.That(l.Claimed,Is.False);Assert.That(s.Player.Inventory.Add(new[]{s.NewItem("fang")}),Is.True);Assert.That(s.Player.Inventory.Count("fang"),Is.EqualTo(99));
            Assert.That(s.Player.Inventory.Add(new[]{s.NewItem("fang")}),Is.False);
        }
        [Test] public void LevelAtDeathSnapshotSurvivesLevelUpAndGapRejects() {
            var s=Fixture(Rules.Sword1);s.Player.Level=10;var item=s.NewItem("fang");var l=new Loot{Id=item.Instance,Item=item,Map=s.Player.Map,Position=s.Player.Position,MobLevel=4,LevelAtDeath=5};
            Assert.That(l.Eligible(s.Player,s.Now),Is.True);l.LevelAtDeath=8;Assert.That(l.Eligible(s.Player,s.Now),Is.False);l.LevelAtDeath=5;l.Created=-61;Assert.That(l.Eligible(s.Player,s.Now),Is.False);
        }
        [Test] public void ConsumablesRejectFullAndSeparateCooldownsFoodDoesNotResurrect() {
            var s=new SliceSession();s.Player.Inventory.Add(new[]{s.NewItem("hp1",2),s.NewItem("mp1",2),s.NewItem("food1")});Assert.That(s.Potion(true),Is.False);Assert.That(s.Player.Inventory.Count("hp1"),Is.EqualTo(2));
            s.Player.Hp=10;s.Player.Mp=10;Assert.That(s.Potion(true),Is.True);Assert.That(s.Potion(false),Is.True);Assert.That(s.Potion(true),Is.False);
            s.UseFood();s.HurtPlayer(9999);Advance(s,4);Assert.That(s.Player.Hp,Is.Zero);Assert.That(s.Potion(false),Is.False);Assert.That(s.Revive(true),Is.True);
        }
        [Test] public void LevelFiveResetsExactlyOnceAndLaterPointsRemain() {
            var p=new Player();p.AddExp(790);Assert.That(p.Level,Is.EqualTo(5));Assert.That(p.Unspent,Is.EqualTo(20));Assert.That(p.Str+p.Vit+p.Int+p.Agi,Is.Zero);
            p.AddExp(450);Assert.That(p.Level,Is.EqualTo(6));Assert.That(p.Unspent,Is.EqualTo(25));p.AddExp(1);Assert.That(p.Unspent,Is.EqualTo(25));
        }
        [Test] public void QuestStagedGrantRetryAndTurnInCapacityAreAtomic() {
            var s=new SliceSession();s.Quest=3;s.QuestState=QuestState.Available;s.Player.Map=Map.Academy;s.Player.Position=new Point(0,.8);s.Player.Inventory.Capacity=0;
            Assert.That(s.AcceptQuest(),Is.False);Assert.That(s.Receipts.Contains("Q3.wood"),Is.False);s.Player.Inventory.Capacity=1;Assert.That(s.AcceptQuest(),Is.True);
            Assert.That(s.Player.Inventory.Count("wood"),Is.EqualTo(1));Assert.That(s.AcceptQuest(),Is.False);
            s.QuestState=QuestState.Ready;Assert.That(s.TurnIn(),Is.False);Assert.That(s.Quest,Is.EqualTo(3));s.Player.Inventory.Capacity=2;Assert.That(s.TurnIn(),Is.True);Assert.That(s.Quest,Is.EqualTo(4));Assert.That(s.Player.Inventory.Count("pants1"),Is.EqualTo(1));
        }
        [Test] public void ShopAndTurnInRequireLiveNearCorrectNpc() {
            var s=new SliceSession();s.Player.Gold=9999;s.Player.Position=new Point(100,.8);Assert.That(s.Buy("food1","Yen"),Is.False);Assert.That(s.AcceptQuest(),Is.False);
            s.Player.Position=new Point(5,.8);Assert.That(s.Buy("food1","Yen"),Is.True);s.Player.Hp=0;Assert.That(s.Buy("food1","Yen"),Is.False);
        }
        [Test] public void ClassSkillsRequireClassWeaponAndUnequipCannotBypass() {
            var s=Fixture(Rules.Sword1);s.Player.Inventory.Equipment[GearSlot.Weapon]=s.NewItem("wood");
            s.Combat.Press("1",1);Assert.That(s.Combat.StartedCount,Is.Zero);Assert.That(s.Combat.Cooldowns,Is.Empty);
        }
        [Test] public void PendingGrantKeepsInstanceAndReceiptAfterCapacityRetry() {
            var s=new SliceSession();var first=s.NewItem("wood");s.Player.Inventory.Capacity=0;
            Assert.That(s.Grant("pending",first),Is.False);s.Player.Inventory.Capacity=1;
            Assert.That(s.Grant("pending",s.NewItem("wood")),Is.True);Assert.That(s.Player.Inventory.Bag.Single().Instance,Is.EqualTo(first.Instance));
            Assert.That(s.Grant("pending",s.NewItem("wood")),Is.True);Assert.That(s.Player.Inventory.Bag.Count,Is.EqualTo(1));
        }
        [Test] public void StorageTransferIsAtomicAndDoesNotCopyInstances() {
            var s=new SliceSession();s.Player.Position=SliceSession.Anchors.First(x=>x.Id=="Moc").Position;var item=s.NewItem("fang",2);s.Player.Inventory.Add(new[]{item});s.Player.Storage.Capacity=0;
            Assert.That(s.Store(item.Instance,true),Is.False);Assert.That(s.Player.Inventory.Count("fang"),Is.EqualTo(2));
            s.Player.Storage.Capacity=40;Assert.That(s.Store(item.Instance,true),Is.True);Assert.That(s.Player.Inventory.Count("fang"),Is.Zero);
            Assert.That(s.Store(item.Instance,true),Is.False);Assert.That(s.Store(item.Instance,false),Is.True);Assert.That(s.Player.Storage.Bag,Is.Empty);
            Assert.That(s.Player.Inventory.Bag.Single().Instance,Is.EqualTo(item.Instance));
        }
        [Test] public void RarityVendorMultiplierUsesFloorAndEquippedCannotBeSold() {
            var s=new SliceSession();s.Player.Position=SliceSession.Anchors.First(x=>x.Id=="Bach").Position;var item=s.NewItem("armor1",quality:1.08);s.Player.Inventory.Add(new[]{item});
            Assert.That(s.Sell(item.Instance),Is.True);Assert.That(s.Player.Gold,Is.EqualTo(82));
            item=s.NewItem("armor1");s.Player.Inventory.Add(new[]{item});s.Equip(item.Instance);Assert.That(s.Sell(item.Instance),Is.False);
        }
        [Test] public void MobWindupMissResolvesOnClockInsteadOfHittingAfterPlayerReturns() {
            var s=new SliceSession();var mob=s.Mobs.First(x=>x.Slot=="DS1.slot1");s.Player.Map=Map.Mist;
            s.Player.Position=new Point(mob.Home.X+.5,mob.Home.Y);for(int i=0;i<100&&!mob.Windup;i++)s.Tick(.02);Assert.That(mob.Windup,Is.True);
            double hp=s.Player.Hp;s.Player.Position=new Point(mob.Home.X+.5,mob.Home.Y+3);Advance(s,1);
            Assert.That(mob.Windup,Is.False);s.Player.Position=new Point(mob.Home.X+.5,mob.Home.Y);s.Tick(.02);
            Assert.That(s.Player.Hp,Is.EqualTo(hp),"Jump dodge must not turn into a delayed hit");
        }
        [Test] public void PortalCancelsMobWindupAndEmptyMapReturnsWithoutNewLifeOrLoot() {
            var s=new SliceSession();var mob=s.Mobs.First(x=>x.Slot=="DS1.slot1");s.Player.Map=Map.Mist;s.Player.Position=new Point(mob.Home.X+.5,mob.Home.Y);
            for(int i=0;i<100&&!mob.Windup;i++)s.Tick(.02);Assert.That(mob.Windup,Is.True);mob.Hp=mob.MaxHp-1;int generation=mob.Generation;
            s.Player.Position=new Point(-4,.8);Assert.That(s.TryExit("toVillageM"),Is.True);Assert.That(mob.Windup,Is.False);
            Advance(s,3);Assert.That(mob.Hp,Is.EqualTo(mob.MaxHp));Assert.That(mob.Generation,Is.EqualTo(generation));Assert.That(s.Loot,Is.Empty);
            s.Player.Position=new Point(34,.8);Assert.That(s.TryExit("toMist"),Is.True);s.Player.Position=new Point(mob.Home.X+.5,mob.Home.Y);
            double hp=s.Player.Hp;s.Tick(.02);Assert.That(s.Player.Hp,Is.EqualTo(hp),"Portal must not retain a deferred old hit");
        }
        [TestCase(1)] [TestCase(2)] [TestCase(3)] public void EverySkillNeedsAnotherPhysicalPress(int slot) {
            var skill=slot==1?Rules.Sword1:slot==2?Rules.Sword2:Rules.Sword3;var s=Fixture(skill);
            s.Combat.Press(slot.ToString(),slot);Advance(s,16);Assert.That(s.Combat.StartedCount,Is.EqualTo(1));
            s.Combat.Press("J");Assert.That(s.Combat.StartedCount,Is.EqualTo(1),"J has no gameplay binding");
        }
        [Test] public void ReleaseKeepsBoundedApproachAndCommitsExactlyOnceAtActualOrigin() {
            var s=Fixture(Rules.Sword1);s.Player.Position=new Point(19,.8);double mp=s.Player.Mp;
            s.Combat.Press("1",1);s.Combat.Release("1");Assert.That(s.Combat.HasPendingCast,Is.True);Assert.That(s.Player.Mp,Is.EqualTo(mp));
            for(int i=0;i<20;i++){s.Player.Position=new Point(s.Player.Position.X+s.Combat.AssistAxis*.10,.8);s.Tick(.02);}
            Assert.That(s.Combat.StartedCount,Is.EqualTo(1));Assert.That(s.Combat.HasPendingCast,Is.False);Assert.That(s.Player.Mp,Is.EqualTo(mp-2));
            Advance(s,3);Assert.That(s.Combat.StartedCount,Is.EqualTo(1));
        }
        [Test] public void PendingTargetDeathNeverCastsAtReplacementAndLowMpNeverRuns() {
            var s=Fixture(Rules.Sword1);s.Player.Position=new Point(19,.8);s.Combat.Press("1",1);s.Mobs[0].Hp=0;s.Mobs[0].RespawnAt=100;
            s.Mobs[1].Map=Map.Academy;s.Mobs[1].Position=new Point(20,.8);Advance(s,.2);Assert.That(s.Combat.StartedCount,Is.Zero);Assert.That(s.Combat.HasPendingCast,Is.False);
            s.Player.Mp=0;s.Combat.Press("1",1);Assert.That(s.Combat.AssistAxis,Is.Zero);Assert.That(s.Combat.Cooldowns,Is.Empty);
        }
        [Test] public void DummyAndCatchupRouteMatchesRevisionWithoutDowngradingLateCharacters() {
            var s=new SliceSession();Assert.That(s.Mobs.Count(x=>x.Dummy),Is.GreaterThanOrEqualTo(3));
            foreach(var pair in new[]{(3,3),(4,4),(5,5)}){
                s.Quest=pair.Item1;s.QuestState=QuestState.Ready;s.Player.Map=pair.Item1==3?Map.Academy:Map.Village;s.Player.Position=SliceSession.Anchors.First(x=>x.Id==s.QuestNpc).Position;
                if(pair.Item1==3)s.Player.AddExp(250);Assert.That(s.TurnIn(),Is.True);Assert.That(s.Player.Level,Is.EqualTo(pair.Item2));
            }
            Assert.That(s.Player.Unspent,Is.EqualTo(20));s.Player.AddExp(450);s.Quest=4;s.Receipts.Remove("Q4.completed");s.QuestState=QuestState.Ready;s.Player.Position=SliceSession.Anchors.First(x=>x.Id==s.QuestNpc).Position;Assert.That(s.TurnIn(),Is.True);Assert.That(s.Player.Level,Is.EqualTo(6));Assert.That(s.Player.Unspent,Is.EqualTo(25));
        }
        [Test] public void TutorialSupplyOnlyExistsAtRelevantActiveStep() {
            foreach(bool active in new[]{false,true}){
                var s=PrototypePresets.Create(PrototypeStart.FirstLoot);s.Player.Map=Map.Mist;s.Player.Position=new Point(15,.8);s.QuestState=active?QuestState.InProgress:QuestState.Available;
                var mob=s.Mobs.First(x=>x.Slot=="DS2.slot1");mob.Dummy=true;
                for(int i=0;i<6&&mob.Alive;i++){s.Combat.Press("1",1);Advance(s,1.1);}
                Assert.That(mob.Alive,Is.False);Assert.That(s.Loot.Count(x=>x.Tutorial),Is.EqualTo(active?2:0));Assert.That(s.Receipts.Contains("Q4.supply"),Is.EqualTo(active));
            }
        }
        [Test] public void EdgeOverlapTransitionsOnceWithoutInteractAndResetIsFresh() {
            var s=new SliceSession();s.ObservePosition(new Point(-10,.8),true);Assert.That(s.Player.Map,Is.EqualTo(Map.Academy));int revision=s.WorldRevision;
            for(int i=0;i<10;i++)s.ObservePosition(s.Player.Position,true);Assert.That(s.WorldRevision,Is.EqualTo(revision));
            var debug=PrototypePresets.Create(PrototypeStart.SwordTraining);Assert.That(debug.Player.Level,Is.EqualTo(5));Assert.That(debug.Combat.Unlocked.ContainsKey(1),Is.True);Assert.That(debug.DebugPreset,Is.Not.Null);
            var fresh=PrototypePresets.Create(PrototypeStart.Fresh);Assert.That(fresh.Quest,Is.EqualTo(1));Assert.That(fresh.Player.Level,Is.EqualTo(1));Assert.That(fresh.Receipts,Is.Empty);Assert.That(fresh.Combat.Cooldowns,Is.Empty);Assert.That(fresh.DebugPreset,Is.Null);
        }
        [Test] public void MeleeWindupAndRecoveryKeepConfiguredAttackInterval(){
            var s=new SliceSession();s.Player.Map=Map.Mist;s.Player.Position=new Point(30,4.25);s.Player.InvulnerableUntil=100;
            var pack=s.Mobs.Where(x=>x.Slot.StartsWith("DS3.")).ToArray();
            foreach(var mob in pack)mob.Position=new Point(30+(mob==pack[0]?-.5:.5),4.25);
            for(int i=0;i<300;i++){
                s.Tick(.02);
                foreach(var mob in pack)if(mob.Windup)Assert.That(mob.NextAttack-(mob.HitAt-.35),Is.EqualTo(mob.Interval).Within(.001),"Steering/recovery must not extend the configured attack clock");
            }
            Assert.That(pack.All(x=>x.BiteAttempts>0),Is.True);
        }
        [Test] public void ClassAdmissionRequiresManualUnequipAndHasTwoGuides(){
            var s=PrototypePresets.Create(PrototypeStart.Class);s.QuestState=QuestState.InProgress;s.Stage=1;s.Player.Map=Map.Academy;s.Player.Position=SliceSession.Anchors.First(x=>x.Id=="Phong").Position;
            long wood=s.Player.Inventory.Equipment[GearSlot.Weapon].Instance;
            Assert.That(SliceSession.Anchors.Any(x=>x.Id=="Diep"&&x.Name=="Diệp Lam"),Is.True);
            Assert.That(s.ChooseSword(),Is.False);Assert.That(s.Player.School,Is.EqualTo(School.Novice));Assert.That(s.Receipts.Contains("Q6.class"),Is.False);
            Assert.That(s.Unequip(GearSlot.Weapon),Is.True);Assert.That(s.Player.Inventory.Bag.Any(x=>x.Instance==wood),Is.True);
            Assert.That(s.ChooseSword(),Is.True);Assert.That(s.Player.Inventory.Equipment.ContainsKey(GearSlot.Weapon),Is.False);
            Assert.That(s.Player.Inventory.Bag.Count(x=>x.Id=="sword1"),Is.EqualTo(1));Assert.That(s.ChooseSword(),Is.False);
        }
        [Test] public void ReciprocalExitPreservesTravelDirection(){
            var s=new SliceSession();s.ObservePosition(new Point(-10,.8),true);Assert.That(s.Player.Map,Is.EqualTo(Map.Academy));Assert.That(s.Player.Position.X,Is.EqualTo(31));
            s.ObservePosition(new Point(34,.8),true);Assert.That(s.Player.Map,Is.EqualTo(Map.Village));Assert.That(s.Player.Position.X,Is.EqualTo(-8));
            s.ObservePosition(new Point(34,.8),true);Assert.That(s.Player.Map,Is.EqualTo(Map.Mist));Assert.That(s.Player.Position.X,Is.EqualTo(-2));
            s.ObservePosition(new Point(-4,.8),true);Assert.That(s.Player.Map,Is.EqualTo(Map.Village));Assert.That(s.Player.Position.X,Is.EqualTo(32));
        }
        [TestCase(1,107)] [TestCase(2,107)] [TestCase(4,107)] [TestCase(4,104.6)] [TestCase(4,120.8)]
        public void EveryWolfCanReengageWithoutFrontDeathOrTeleport(int count,double playerX){
            var s=new SliceSession();s.Player.Map=Map.Mist;s.Player.Position=new Point(playerX,1.45);s.Player.InvulnerableUntil=100;
            var pack=s.Mobs.Where(x=>x.Slot.StartsWith("PROBE7.")).OrderBy(x=>x.Id).ToArray();
            for(int j=0;j<4;j++){pack[j].Home=pack[j].Position=new Point(Math.Max(104.45,Math.Min(121.55,playerX+(playerX>115?-1:1)*(1+j))),1.45);if(j>=count){pack[j].Hp=0;pack[j].RespawnAt=100;}}
            for(int i=0;i<800;i++){
                var before=pack.Select(x=>x.Position.X).ToArray();var windup=pack.Select(x=>x.Windup).ToArray();var facing=pack.Select(x=>x.Facing).ToArray();s.Tick(.02);
                for(int j=0;j<count;j++){
                    Assert.That(Math.Abs(pack[j].Position.X-before[j]),Is.LessThanOrEqualTo(pack[j].Speed*.02+.0001),"No teleport or separation shove");
                    if(windup[j]&&pack[j].Windup){Assert.That(pack[j].Facing,Is.EqualTo(facing[j]));Assert.That(pack[j].Position.X,Is.EqualTo(before[j]));}
                    Assert.That(pack[j].Position.X,Is.InRange(pack[j].LaneMin,pack[j].LaneMax));
                }
            }
            Assert.That(pack.Take(count).All(x=>x.BiteAttempts>=2),Is.True,"Every wolf must get actual in-range attempts without removing the front mob: "+string.Join(",",pack.Take(count).Select(x=>x.Id+":"+x.BiteAttempts+" "+x.Motion+" goal="+x.DesiredX)));
        }
        [Test] public void PackLeavesBoundaryThenResumesPatrolWithoutNewLifeOrCredit(){
            var s=new SliceSession();s.Player.Map=Map.Mist;s.Player.Position=new Point(107,1.45);s.Player.InvulnerableUntil=100;
            var pack=s.Mobs.Where(x=>x.Slot.StartsWith("PROBE7.")).ToArray();Advance(s,3);
            Assert.That(pack.All(x=>x.Engaged),Is.True);pack[0].Hp-=10;pack[0].QuestDamage=10;
            s.Player.Position=new Point(125,1.45);Advance(s,12);
            Assert.That(pack.All(x=>!x.Engaged&&!x.Windup&&!x.Returning),Is.True);
            var previous=pack.Select(x=>x.Position.X).ToArray();Advance(s,2.5);
            Assert.That(pack.Where((m,i)=>Math.Abs(m.Position.X-previous[i])>.2).Any(),Is.True,"They must patrol, not stare forever at the leash edge");
            Assert.That(pack.All(x=>x.Position.X>=x.ActivityMin&&x.Position.X<=x.ActivityMax&&x.Generation==1),Is.True);
            Assert.That(pack[0].Hp,Is.EqualTo(pack[0].MaxHp));Assert.That(pack[0].QuestDamage,Is.Zero);Assert.That(s.Loot,Is.Empty);
            Assert.That(pack.Select(x=>x.ActivityMin).Distinct().Count(),Is.EqualTo(1));
        }
        [Test] public void WaterContactSlowsFeetButNotBridgeAirOrDryGround(){
            Assert.That(BlockoutLayout.WaterSpeed(Map.Village,new Point(3,-.35)),Is.EqualTo(.85));
            Assert.That(BlockoutLayout.WaterSpeed(Map.Village,new Point(3,.2)),Is.EqualTo(1));
            Assert.That(BlockoutLayout.WaterSpeed(Map.Mist,new Point(73,-2)),Is.EqualTo(1));
            Assert.That(BlockoutLayout.WaterSpeed(Map.Mist,new Point(73,.1)),Is.EqualTo(1));
            Assert.That(BlockoutLayout.WaterSpeed(Map.Academy,new Point(3,0)),Is.EqualTo(1));
        }
        [Test] public void DryPacksStayOnShoreWhilePlayerCrossesWater(){
            var s=PrototypePresets.Create(PrototypeStart.Crowd);
            foreach(var m in s.Mobs.Where(x=>x.Map==Map.Mist)){
                foreach(double x in new[]{m.Home.X,m.ActivityMin,m.ActivityMax})
                    Assert.That(BlockoutLayout.WaterSpeed(Map.Mist,new Point(x,m.Home.Y-.65)),Is.EqualTo(1),m.Slot+" must have a dry home and patrol/chase bounds");
            }
            s.Player.InvulnerableUntil=200;s.Player.Position=new Point(58,5.25);Advance(s,8);
            s.Player.Position=new Point(73,1.12);
            for(int tick=0;tick<600;tick++){
                s.Tick(.02);
                foreach(var m in s.Mobs.Where(x=>x.Map==Map.Mist))
                    Assert.That(BlockoutLayout.WaterSpeed(Map.Mist,new Point(m.Position.X,m.Position.Y-.65)),Is.EqualTo(1),m.Slot+" entered water during chase/return/patrol");
            }
            Assert.That(s.Mobs.Where(x=>x.Slot.StartsWith("DS5")).All(x=>!x.Engaged&&!x.Returning),Is.True);
        }
        [Test] public void WideWaterIsSceneryBehindContinuousSolidBridge(){
            var bridge=BlockoutLayout.Surfaces(Map.Mist).Single(x=>x.Name=="Valley solid wooden bridge");
            var water=BlockoutLayout.Waters(Map.Mist).Single();
            Assert.That(water.DecorativeOnly,Is.False);Assert.That(bridge.OneWay,Is.False);Assert.That(bridge.Wood,Is.True);
            Assert.That(BlockoutLayout.WaterSpeed(Map.Mist,new Point(73,-1.3)),Is.EqualTo(.85));
            Assert.That(bridge.TopAt(74)-water.Level,Is.GreaterThanOrEqualTo(2),"The bridge needs visible air above water");
            Assert.That(BlockoutLayout.BaseGroundTop(Map.Mist,65.9),Is.EqualTo(bridge.TopAt(74)).Within(.0001));
            Assert.That(BlockoutLayout.BaseGroundTop(Map.Mist,82.01),Is.EqualTo(bridge.TopAt(74)).Within(.01));
            Assert.That(bridge.X-bridge.Width/2,Is.LessThanOrEqualTo(water.Left));Assert.That(bridge.X+bridge.Width/2,Is.GreaterThanOrEqualTo(water.Right));
            for(double x=66.1;x<82;x+=.2)Assert.That(BlockoutLayout.BaseGroundTop(Map.Mist,x),Is.GreaterThan(water.Level));
        }
        [Test] public void TrackerDirectionsFollowCurrentQuestStepAndMap(){
            var s=new SliceSession();s.QuestState=QuestState.InProgress;
            Assert.That(s.Objective,Does.Contain("Vân Khê · ở map hiện tại"));Assert.That(s.Objective,Does.Not.Contain("Đồng Sương"));
            s.Quest=4;s.Stage=3;s.Player.Map=Map.Mist;Assert.That(s.Objective,Does.Contain("Cổng tây Đồng Sương → Vân Khê"));
            s.Quest=5;s.Stage=0;s.Player.Map=Map.Village;Assert.That(s.Objective,Does.Contain("Vân Khê · ở map hiện tại"));
            s.Quest=6;s.Stage=3;s.Player.Map=Map.Academy;Assert.That(s.Objective,Does.Contain("Học Viện · ở map hiện tại"));
            s.Stage=5;Assert.That(s.Objective,Does.Contain("Cổng đông Học Viện → Vân Khê"));
        }
        [Test] public void RearEarthIsFilledBehindActorsWithAnExplicitOneWayTop(){
            foreach(var map in new[]{Map.Academy,Map.Mist})foreach(var rear in BlockoutLayout.Surfaces(map).Where(x=>x.RearEarth)){
                Assert.That(rear.OneWay,Is.True);Assert.That(rear.Wood,Is.False);Assert.That(rear.Overhead,Is.True);
                Assert.That(rear.Y-rear.Height/2,Is.EqualTo(-8),"Rear soil must be filled to ground depth, not a floating slab");
            }
            Assert.That(BlockoutLayout.BaseGroundTop(Map.Mist,60),Is.EqualTo(1.2).Within(.0001));Assert.That(BlockoutLayout.GroundTop(Map.Mist,60),Is.EqualTo(4.6).Within(.0001));
        }
        [Test] public void TerrainHasSolidElevationAndFewIntentionalOneWayDecks(){
            foreach(Map map in Enum.GetValues(typeof(Map))){var surfaces=BlockoutLayout.Surfaces(map).ToArray();Assert.That(surfaces.Where(x=>!x.OneWay).Select(x=>x.Y+x.Height/2).Distinct().Count(),Is.GreaterThan(1));Assert.That(surfaces.Count(x=>x.OneWay&&x.Wood),Is.InRange(1,3));}
            var s=new SliceSession();foreach(var m in s.Mobs.Where(x=>x.Map==Map.Mist)){
                var support=BlockoutLayout.MobSupport(m.Slot,m.Home.X);
                Assert.That(m.Home.Y,Is.EqualTo(support.Y+support.Height/2+.65).Within(.0001),m.Slot+" must stand above its physical support");
            }

        }
        [Test] public void ProbePocketsDoNotGrantQ5KillCredit(){
            var s=PrototypePresets.Create(PrototypeStart.Crowd);s.Quest=5;s.Stage=3;s.QuestState=QuestState.InProgress;
            var m=s.Mobs.First(x=>x.Slot.StartsWith("PROBE7."));m.Hp=1;m.Dummy=true;s.Player.Position=m.Position;s.Combat.Press("1",1);Advance(s,1);
            Assert.That(m.Alive,Is.False);Assert.That(s.Kills,Is.Zero);
        }
        [Test] public void ExpInvariantAndHalfUpDamage() {
            Assert.That(Rules.Exp.Last(),Is.EqualTo(53100));Assert.That(19*5,Is.EqualTo(95));Assert.That(Rules.Round(2.5),Is.EqualTo(3));
            Assert.That(Rules.Damage(100,1,1,100,false,1),Is.EqualTo(50));
        }
    }
}
