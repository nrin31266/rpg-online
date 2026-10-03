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
            foreach(var m in s.Mobs){m.Dummy=true;m.Hp=m.MaxHp=10000;}
            return s;
        }
        private void Advance(SliceSession s,double duration){for(int i=0;i<(int)Math.Ceiling(duration/.02);i++)s.Tick(.02);}
        [TestCase(true,true,false,false,-1)] [TestCase(false,false,true,true,1)] [TestCase(true,false,true,false,0)]
        public void AlternateBindingsDoNotDoubleSpeed(bool a,bool left,bool d,bool right,int expected)=>Assert.That(Rules.Axis(a,left,d,right),Is.EqualTo(expected));
        [Test] public void NoTargetCommitsNothing() {
            var s=Fixture(Rules.Sword1);s.Player.Position=new Point(100,.8);double mp=s.Player.Mp;
            s.Combat.Press("1",1);Advance(s,1);Assert.That(s.Combat.StartedCount,Is.Zero);Assert.That(s.Player.Mp,Is.EqualTo(mp));Assert.That(s.Combat.Cooldowns,Is.Empty);Assert.That(s.Combat.AssistAxis,Is.Zero);
        }
        [Test] public void SlotPressExecutesImmediatelyAndJUsesSelection() {
            var s=Fixture(Rules.Sword1);s.Combat.Press("1",1);Assert.That(s.Combat.Running.Skill,Is.SameAs(Rules.Sword1));s.Combat.Release("1");Advance(s,1.1);
            s.Combat.Press("J");Assert.That(s.Combat.StartedCount,Is.EqualTo(2));Assert.That(s.Combat.Running.Skill.Id,Is.EqualTo("sword.s1"));
        }
        [Test] public void LockedSlotDoesNotSelectOrStart() {
            var s=Fixture(Rules.Sword1);s.Combat.Press("2",2);Assert.That(s.Combat.SelectedSlot,Is.EqualTo(1));Assert.That(s.Combat.StartedCount,Is.Zero);
        }
        [Test] public void SpamKeepsImmutableActionAndIndependentCooldowns() {
            var s=Fixture(Rules.Sword1,Rules.Sword2,Rules.Sword3);s.Combat.Press("1",1);var action=s.Combat.Running;
            s.Combat.Press("2",2);s.Combat.Press("3",3);s.Combat.Press("J");
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
        [Test] public void NewHoldOwnerReleaseDoesNotResurrectOlderHold() {
            var s=Fixture(Rules.Sword1,Rules.Sword2);s.Combat.Press("1",1);s.Combat.Press("2",2);s.Combat.Release("2");Advance(s,3);Assert.That(s.Combat.StartedCount,Is.EqualTo(1));
        }
        [Test] public void AutoFocusStickyThenDeathReacquires() {
            var s=Fixture(Rules.Sword1);var a=s.Mobs[0];a.Position=new Point(22,.8);var b=s.Mobs[1];b.Map=Map.Academy;b.Position=new Point(23,.8);
            s.Combat.Press("1",1);s.Combat.Release("1");Assert.That(s.Combat.FocusId,Is.EqualTo(a.Id));b.Position=new Point(21.1,.8);s.Tick(.02);Assert.That(s.Combat.FocusId,Is.EqualTo(a.Id));
            a.Hp=0;a.RespawnAt=100;s.Tick(.02);Assert.That(s.Combat.FocusId,Is.EqualTo(b.Id));
        }
        [Test] public void ManualContextChangesAutoButExplicitIsPinned() {
            var s=Fixture(Rules.Sword1);var a=s.Mobs[0];s.Combat.Press("1",1);s.Combat.Release("1");
            var b=s.Mobs[1];b.Map=Map.Academy;b.Position=new Point(30,.8);s.Player.Position=new Point(29,.8);s.Tick(.02,true);Assert.That(s.Combat.FocusId,Is.EqualTo(b.Id));
            s.Combat.Explicit(a);s.Tick(.02,true);Assert.That(s.Combat.FocusId,Is.EqualTo(a.Id));Assert.That(s.Combat.FocusKind,Is.EqualTo(FocusKind.Explicit));
        }
        [Test] public void FacingIndependentAcquireAndExplicitOutOfRangeDoesNotSubstitute() {
            var s=Fixture(Rules.Sword1);s.Player.Position=new Point(23,.8);s.Combat.Facing=1;s.Combat.Press("1",1);Assert.That(s.Combat.Running.Facing,Is.EqualTo(-1));s.Combat.Cancel();
            var a=s.Mobs[0];a.Position=new Point(30,.8);var b=s.Mobs[1];b.Map=Map.Academy;b.Position=new Point(23.5,.8);s.Combat.Explicit(a);double mp=s.Player.Mp;
            s.Combat.Press("J");Assert.That(s.Combat.Running,Is.Null);Assert.That(s.Combat.FocusId,Is.EqualTo(a.Id));Assert.That(s.Player.Mp,Is.EqualTo(mp));
        }
        [Test] public void ApproachIsBoundedHorizontalAndManualCancels() {
            var s=Fixture(Rules.Sword1);s.Player.Position=new Point(19,.8);s.Combat.Press("J");Advance(s,.22);Assert.That(s.Combat.AssistAxis,Is.EqualTo(1));
            s.Combat.ManualOverride();Assert.That(s.Combat.AssistAxis,Is.Zero);Advance(s,2);Assert.That(s.Combat.StartedCount,Is.Zero);
            s.Combat.Press("J");Advance(s,1);Assert.That(s.Combat.AssistAxis,Is.Zero); // blocked progress times out
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
            var s=new SliceSession();s.Player.Position=new Point(15,.8);var item=s.NewItem("fang",2);s.Player.Inventory.Add(new[]{item});s.Player.Storage.Capacity=0;
            Assert.That(s.Store(item.Instance,true),Is.False);Assert.That(s.Player.Inventory.Count("fang"),Is.EqualTo(2));
            s.Player.Storage.Capacity=40;Assert.That(s.Store(item.Instance,true),Is.True);Assert.That(s.Player.Inventory.Count("fang"),Is.Zero);
            Assert.That(s.Store(item.Instance,true),Is.False);Assert.That(s.Store(item.Instance,false),Is.True);Assert.That(s.Player.Storage.Bag,Is.Empty);
            Assert.That(s.Player.Inventory.Bag.Single().Instance,Is.EqualTo(item.Instance));
        }
        [Test] public void RarityVendorMultiplierUsesFloorAndEquippedCannotBeSold() {
            var s=new SliceSession();s.Player.Position=new Point(10,.8);var item=s.NewItem("armor1",quality:1.08);s.Player.Inventory.Add(new[]{item});
            Assert.That(s.Sell(item.Instance),Is.True);Assert.That(s.Player.Gold,Is.EqualTo(82));
            item=s.NewItem("armor1");s.Player.Inventory.Add(new[]{item});s.Equip(item.Instance);Assert.That(s.Sell(item.Instance),Is.False);
        }
        [Test] public void MobWindupMissResolvesOnClockInsteadOfHittingAfterPlayerReturns() {
            var s=new SliceSession();var mob=s.Mobs.First(x=>x.Slot=="DS1.slot1");s.Player.Map=Map.Mist;
            s.Player.Position=new Point(mob.Home.X+.5,mob.Home.Y);s.Tick(.02);Assert.That(mob.Windup,Is.True);
            double hp=s.Player.Hp;s.Player.Position=new Point(mob.Home.X+.5,mob.Home.Y+3);Advance(s,1);
            Assert.That(mob.Windup,Is.False);s.Player.Position=new Point(mob.Home.X+.5,mob.Home.Y);s.Tick(.02);
            Assert.That(s.Player.Hp,Is.EqualTo(hp),"Jump dodge must not turn into a delayed hit");
        }
        [Test] public void PortalCancelsMobWindupAndEmptyMapReturnsWithoutNewLifeOrLoot() {
            var s=new SliceSession();var mob=s.Mobs.First(x=>x.Slot=="DS1.slot1");s.Player.Map=Map.Mist;s.Player.Position=new Point(mob.Home.X+.5,mob.Home.Y);
            s.Tick(.02);Assert.That(mob.Windup,Is.True);mob.Hp=mob.MaxHp-1;int generation=mob.Generation;
            s.Player.Position=new Point(-4,.8);Assert.That(s.Portal("toVillageM"),Is.True);Assert.That(mob.Windup,Is.False);
            Advance(s,3);Assert.That(mob.Hp,Is.EqualTo(mob.MaxHp));Assert.That(mob.Generation,Is.EqualTo(generation));Assert.That(s.Loot,Is.Empty);
            s.Player.Position=new Point(30,.8);Assert.That(s.Portal("toMist"),Is.True);s.Player.Position=new Point(mob.Home.X+.5,mob.Home.Y);
            double hp=s.Player.Hp;s.Tick(.02);Assert.That(s.Player.Hp,Is.EqualTo(hp),"Portal must not retain a deferred old hit");
        }
        [Test] public void ExpInvariantAndHalfUpDamage() {
            Assert.That(Rules.Exp.Last(),Is.EqualTo(53100));Assert.That(19*5,Is.EqualTo(95));Assert.That(Rules.Round(2.5),Is.EqualTo(3));
            Assert.That(Rules.Damage(100,1,1,100,false,1),Is.EqualTo(50));
        }
    }
}
