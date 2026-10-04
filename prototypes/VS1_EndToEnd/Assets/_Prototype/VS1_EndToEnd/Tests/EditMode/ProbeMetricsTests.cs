using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HuyenLo.Domain;
using NUnit.Framework;
using UnityEngine;

namespace HuyenLo.Tests
{
    // Controlled probes only. No runtime balance data is changed by this recorder.
    public sealed class ProbeMetricsTests
    {
        private const int Seed=731;
        private const double Dt=.02;
        private static SliceSession Novice(Map map,int level) {
            var s=new SliceSession(Seed);s.Player.Map=map;s.Player.Level=level;
            s.Player.Inventory.Equipment[GearSlot.Weapon]=s.NewItem("wood");
            s.Player.Hp=s.Player.Stats.Hp;s.Player.Mp=s.Player.Stats.Mp;return s;
        }
        private static void AdvanceMovement(SliceSession s,int manual=0) {
            var p=s.Player.Position;int move=manual!=0?manual:s.Combat.AssistAxis;
            s.ObservePosition(new Point(p.X+move*5*Dt,p.Y),true);
            s.Tick(Dt,manual!=0);
        }
        private static IEnumerable<double> PressTimes(double seconds) {
            var random=new System.Random(Seed);double time=0;
            while(time<seconds){yield return time;time+=.35+(random.NextDouble()*2-1)*.1;}
        }
        private static (int presses,int rejected) Cadence() {
            var s=Novice(Map.Academy,3);s.Player.Position=new Point(21,.8);
            var dummy=s.Mobs.First(x=>x.Dummy);foreach(var m in s.Mobs)if(m!=dummy)m.Map=Map.Mist;
            var times=PressTimes(60).ToArray();int next=0,rejected=0;
            while(s.Now<60){
                // Continuous-target laboratory: new Dummy life as soon as previous dies.
                // HP stays 60; this deliberately bypasses world respawn for cadence isolation.
                if(!dummy.Alive){dummy.Generation++;dummy.Hp=dummy.MaxHp;dummy.RespawnAt=double.MaxValue;}
                if(next<times.Length&&s.Now>=times[next]){
                    long starts=s.Combat.StartedCount;s.Combat.Press("1");
                    if(starts==s.Combat.StartedCount&&!s.Combat.HasPendingCast&&s.Combat.BufferUntil<=s.Now)rejected++;
                    next++;
                }
                s.Tick(Dt);
            }
            return(next,rejected);
        }
        private static (double flipRate,double damage,double ttk,int[] flips,bool alive) Crowd() {
            var s=new SliceSession(Seed);s.Player.Map=Map.Mist;s.Player.School=School.Sword;s.Player.Level=5;
            s.Player.Str=s.Player.Vit=s.Player.Int=s.Player.Agi=5;
            foreach(var id in new[]{"sword1","armor1","pants1","boots1","ring1","neck1"})s.Player.Inventory.Equipment[Catalog.Get(id).Slot]=s.NewItem(id);
            s.Player.Hp=s.Player.Stats.Hp;s.Player.Mp=s.Player.Stats.Mp;s.Combat.Unlocked[1]=Rules.Sword1;
            var wolves=s.Mobs.Where(x=>x.Slot.StartsWith("PROBE7.")).Take(3).ToArray();
            foreach(var m in s.Mobs)if(!wolves.Contains(m))m.Map=Map.Academy;
            s.Player.Position=new Point(107,wolves[0].Home.Y+.07);double hp=s.Player.Hp,ttk=-1;
            int[] flips=new int[3],facing=wolves.Select(x=>x.Facing).ToArray(),sign=new int[3];
            var times=PressTimes(10).ToArray();int next=0;
            while(s.Now<10){
                if(next<times.Length&&s.Now>=times[next]){s.Combat.Press("1");next++;}
                s.Tick(Dt);
                for(int i=0;i<3;i++){
                    var m=wolves[i];if(!m.Alive){if(ttk<0)ttk=s.Now;continue;}
                    int velocity=Math.Abs(m.VelocityX)>.001?Math.Sign(m.VelocityX):0;
                    bool changed=facing[i]!=m.Facing||(velocity!=0&&sign[i]!=0&&velocity!=sign[i]);
                    if(changed)flips[i]++;facing[i]=m.Facing;if(velocity!=0)sign[i]=velocity;
                }
            }
            return(flips.Sum()/3.0/2,hp-s.Player.Hp,ttk,flips,s.Player.Alive);
        }
        private static double Ttk(bool wolf) {
            var s=Novice(wolf?Map.Mist:Map.Academy,wolf?4:3);
            var targets=wolf?s.Mobs.Where(x=>x.Name=="Sói Sương").Take(1).ToArray():s.Mobs.Where(x=>x.Dummy).ToArray();
            foreach(var m in s.Mobs)if(!targets.Contains(m))m.Map=Map.Village;
            if(wolf)s.Player.Inventory.Equipment[GearSlot.Pants]=s.NewItem("pants1");
            s.Player.Hp=s.Player.Stats.Hp;s.Player.Position=new Point(targets[0].Position.X-1,targets[0].Position.Y+.07);
            var killed=new HashSet<string>();var times=PressTimes(120).ToArray();int next=0;
            while(s.Now<120&&killed.Count<targets.Length&&s.Player.Alive){
                if(next<times.Length&&s.Now>=times[next]){s.Combat.Press("1");next++;}
                var nearest=targets.Where(x=>x.Alive).OrderBy(x=>x.Position.Distance(s.Player.Position)).FirstOrDefault();
                // A new Dummy may be outside the bounded acquisition envelope. Walk manually
                // between targets, rather than turning this fixture into unlimited auto combat.
                int manual=!s.Combat.HasPendingCast&&s.Combat.Running==null&&nearest!=null&&nearest.Position.Distance(s.Player.Position)>Rules.Novice.Range?Math.Sign(nearest.Position.X-s.Player.Position.X):0;
                if(manual!=0)s.Combat.ManualOverride();
                AdvanceMovement(s,manual);foreach(var m in targets)if(!m.Alive)killed.Add(m.Slot);
            }
            return killed.Count==targets.Length?s.Now:-1;
        }
        private static (int attempts,int cancels) HeldApproach() {
            int cancelled=0;
            for(int i=0;i<20;i++){
                var s=Novice(Map.Academy,3);s.Player.Position=new Point(19.8,.8);
                s.Combat.Press("1");Assert.That(s.Combat.HasPendingCast,Is.True);
                // Mirror baseline Host: a held movement axis cancels in every FixedUpdate.
                s.Combat.ManualOverride();if(!s.Combat.HasPendingCast)cancelled++;
            }
            return(20,cancelled);
        }
        private static int ExitLeak() {
            var s=Novice(Map.Mist,4);foreach(var m in s.Mobs)m.Map=Map.Academy;
            var target=s.Mobs.First(x=>x.Dummy);target.Map=Map.Mist;target.Position=new Point(-5.3,.8);s.Player.Position=new Point(-2.9,.8);
            s.Combat.Explicit(target);s.Combat.Press("1");Assert.That(s.Combat.HasPendingCast,Is.True);
            for(int i=0;i<100&&s.Player.Map==Map.Mist&&s.Combat.HasPendingCast;i++)AdvanceMovement(s);
            return s.Player.Map==Map.Mist?0:1;
        }
        [Test] public void RecordBaselineBeforeBehaviorChanges() {
            var c=Cadence();var crowd=Crowd();var held=HeldApproach();
            var report=new BaselineMetrics {seed=Seed,dt=Dt,utc=DateTime.UtcNow.ToString("O"),presses=c.presses,rejected=c.rejected,rejectPercent=100.0*c.rejected/c.presses,
                flipsPerWolfPer5s=crowd.flipRate,flipsPerWolf10s=crowd.flips,damageTaken10s=crowd.damage,playerSurvivedCrowd10s=crowd.alive,
                crowdFirstKillSeconds=crowd.ttk,noviceCooldown=Rules.Novice.Cooldown,noviceActionLock=Rules.Novice.Lock,deadTimePercent=100*(Rules.Novice.Cooldown-Rules.Novice.Lock)/Rules.Novice.Cooldown,
                noviceOneWolfTtk=Ttk(true),noviceThreeDummyTtk=Ttk(false),heldApproachAttempts=held.attempts,heldApproachCancelled=held.cancels,edgeExitLeaks=ExitLeak()};
            string dir=Path.GetFullPath(Path.Combine(Application.dataPath,"../PrototypeEvidence/Baseline"));Directory.CreateDirectory(dir);
            File.WriteAllText(Path.Combine(dir,"metrics.json"),JsonUtility.ToJson(report,true)+"\n");Debug.Log(JsonUtility.ToJson(report));
        }
    }
    [Serializable] public sealed class BaselineMetrics {
        public string utc;
        public int seed,presses,rejected,heldApproachAttempts,heldApproachCancelled,edgeExitLeaks;
        public int[] flipsPerWolf10s;
        public bool playerSurvivedCrowd10s;
        public double dt,rejectPercent,flipsPerWolfPer5s,damageTaken10s,crowdFirstKillSeconds,noviceCooldown,noviceActionLock,deadTimePercent,noviceOneWolfTtk,noviceThreeDummyTtk;
        public string method="Deterministic domain recorder, seed731/dt0.02; native keyboard/physics rollover remains CHƯA ĐO. Cadence: 60s, 0.35±0.1s seeded presses, Dummy HP60 with immediate new laboratory life after death (not world respawn). Crowd: three PROBE7 wolves, stationary balanced Lv5/Common I Sword, same seeded presses for10s, finite HP. Direction rate counts Facing OR nonzero horizontal sign change once per tick, averaged over three wolves and two5s windows. Damage is actual HP loss, no healing. Novice TTK: Lv4+wood+pants vs one wolf; Lv3+wood vs three HP60 Dummy, bounded logical movement5u/s. Held/exit scenarios mirror Host order in domain, not native physical keyboard evidence.";
    }
}
