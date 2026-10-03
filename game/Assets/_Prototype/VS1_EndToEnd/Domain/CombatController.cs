using System;
using System.Collections.Generic;
using System.Linq;

namespace HuyenLo.Domain
{
    public readonly struct TargetKey
    {
        public readonly int Id,Generation;
        public TargetKey(Mob mob){Id=mob.Id;Generation=mob.Generation;}
    }
    public sealed class CombatAction
    {
        public readonly long Id;
        public readonly Skill Skill;
        public readonly Point Origin;
        public readonly int Facing;
        public readonly double Atk,Acc,Crit,Bonus,Started,ResolveAt,EndAt;
        public readonly TargetKey[] Targets;
        public bool Resolved {get;internal set;}
        public CombatAction(long id,Skill skill,Point origin,int facing,Stats stats,double now,TargetKey[] targets) {
            Id=id;Skill=skill;Origin=origin;Facing=facing;Atk=stats.Atk;Acc=stats.Acc;Crit=stats.Crit;Bonus=stats.Bonus;
            Started=now;ResolveAt=now+skill.HitDelay;EndAt=now+skill.Lock;Targets=targets;
        }
    }
    public sealed class HitResult
    {
        public long ActionId;
        public int Target,Generation,Index,Damage;
        public bool Evaded,Crit;
        public double Clock;
    }
    public sealed class CombatController
    {
        private readonly SliceSession session;
        private Player P => session.Player;
        public readonly Dictionary<int,Skill> Unlocked=new Dictionary<int,Skill>();
        public readonly Dictionary<string,double> Cooldowns=new Dictionary<string,double>();
        public readonly List<HitResult> Results=new List<HitResult>();
        public CombatAction Running {get;private set;}
        public FocusKind FocusKind {get;private set;}
        public int FocusId {get;private set;}
        public int FocusGeneration {get;private set;}
        public int SelectedSlot=1,Facing=1;
        public int AssistAxis {get;private set;}
        public double BufferUntil {get;private set;}
        public long StartedCount {get;private set;}
        private string owner;
        private long pressToken,consumedToken=-1;
        private double pressedAt,approachStarted=-1,approachX,lastProgressAt,lastX;
        private Skill heldSkill,bufferSkill;
        private double nextAcquireAt;
        public CombatController(SliceSession value){session=value;}
        public Mob Focus => session.Find(FocusId,FocusGeneration);
        public Skill Selected => P.School==School.Novice?Rules.Novice:Unlocked.TryGetValue(SelectedSlot,out var s)?s:null;
        private bool Available(Skill s) => s!=null && (P.School==School.Novice ? s.Id=="novice" && P.Inventory.Equipment.TryGetValue(GearSlot.Weapon,out var item) && item.Id=="wood":P.Inventory.Equipment.TryGetValue(GearSlot.Weapon,out var weapon) && weapon.Id=="sword1" && Unlocked.Values.Any(x=>x.Id==s.Id));
        public double Remaining(Skill skill) => skill!=null&&Cooldowns.TryGetValue(skill.Id,out var end)?Math.Max(0,end-session.Now):0;
        public void Explicit(Mob target) {
            CancelIntent();
            if(!P.Alive||target==null||!target.Alive||target.Map!=P.Map)return;
            FocusId=target.Id;FocusGeneration=target.Generation;FocusKind=FocusKind.Explicit;session.Emit("Focus EXPLICIT "+target.Slot);
        }
        public void ClearFocus(){FocusId=FocusGeneration=0;FocusKind=FocusKind.None;}
        private void Acquire(Skill skill) {
            if(session.Now<nextAcquireAt)return;
            double search=(skill?.Range??1.7)+ApproachBudget(skill);
            var target=session.Mobs.Where(x=>x.Alive&&x.Map==P.Map&&!x.Returning&&x.Position.Distance(P.Position)<=search&&Math.Abs(x.Position.Y-P.Position.Y)<=1.6)
                .OrderBy(x=>x.Position.Distance(P.Position)).ThenBy(x=>x.Id).FirstOrDefault();
            if(target==null){ClearFocus();nextAcquireAt=session.Now+.1;return;}nextAcquireAt=0;FocusId=target.Id;FocusGeneration=target.Generation;FocusKind=FocusKind.Auto;
        }
        private static double ApproachBudget(Skill s) => s==null?1:Math.Max(1,Math.Min(3,s.Range*1.5));
        public void Press(string key,int slot=0) {
            CancelIntent();owner=key;pressToken++;pressedAt=session.Now;
            if(slot>0){
                if(!Unlocked.ContainsKey(slot)){session.Emit("Slot "+slot+" chưa mở trong VS-1.");owner=null;return;}
                SelectedSlot=slot;
            }
            heldSkill=Selected;
            if(!P.Alive||!Available(heldSkill)){session.Emit("Cần mặc vũ khí và học kỹ năng.");owner=null;return;}
            if(Focus==null)Acquire(heldSkill);
            if(Running!=null){
                double recoveryLeft=Running.EndAt-session.Now;
                if(recoveryLeft<=.15 && Remaining(heldSkill)<=recoveryLeft && P.Mp>=heldSkill.Mp){bufferSkill=heldSkill;BufferUntil=session.Now+.15;}
                return;
            }
            if(Remaining(heldSkill)>0)return;
            TryStart(heldSkill);
        }
        public void Release(string key){
            if(owner!=key)return;
            owner=null;heldSkill=null;AssistAxis=0;approachStarted=-1;
            // A valid short recovery tap may survive release; held approach/repeat never does.
        }
        public void ManualOverride(){CancelIntent();}
        public void CancelIntent(){owner=null;heldSkill=bufferSkill=null;BufferUntil=0;AssistAxis=0;approachStarted=-1;}
        public void Cancel(){Running=null;CancelIntent();}
        private bool InRange(Skill skill,Mob m,Point origin,int facing) {
            double dx=m.Position.X-origin.X,dy=m.Position.Y-origin.Y;
            if(Math.Abs(dy)>skill.Vertical)return false;
            if(skill.Shape==Shape.Line)return dx*facing>=0 && dx*facing<=skill.Range && Math.Abs(dy)<=.3;
            if(skill.Shape==Shape.Arc)return m.Position.Distance(origin)<=skill.Range && dx*facing>=Math.Abs(dy)/Math.Sqrt(3);
            return m.Position.Distance(origin)<=skill.Range;
        }
        private bool TryStart(Skill skill) {
            if(!P.Alive||!Available(skill)||Running!=null||Remaining(skill)>0||P.Mp<skill.Mp)return false;
            var focus=Focus;
            if(focus==null||focus.Returning){session.Feedback="Không có mục tiêu hợp lệ — không tiêu MP/CD.";return false;}
            var stats=P.Stats;int facing=focus.Position.X<P.Position.X?-1:1;
            if(!InRange(skill,focus,P.Position,facing)){session.Feedback="Mục tiêu ngoài tầm. Tap không tự đi; giữ phím để tiếp cận ngang.";return false;}
            var eligible=session.Mobs.Where(x=>x.Alive&&x.Map==P.Map&&!x.Returning&&InRange(skill,x,P.Position,facing));
            TargetKey[] targets;
            if(skill.Shape==Shape.Line)targets=eligible.OrderBy(x=>(x.Position.X-P.Position.X)*facing).ThenBy(x=>x.Id).Take(5).Select(x=>new TargetKey(x)).ToArray();
            else if(skill.Shape==Shape.Arc)targets=eligible.OrderBy(x=>x.Id==focus.Id?0:1).ThenBy(x=>x.Position.Distance(P.Position)).ThenBy(x=>x.Id).Take(3).Select(x=>new TargetKey(x)).ToArray();
            else if(skill.Shape==Shape.Spread){var rest=eligible.Where(x=>x.Id!=focus.Id).OrderBy(x=>x.Position.Distance(P.Position)).ThenBy(x=>x.Id).Take(2).ToArray();
                targets=new[]{new TargetKey(focus),new TargetKey(rest.Length>0?rest[0]:focus),new TargetKey(rest.Length>1?rest[1]:focus)};}
            else targets=new[]{new TargetKey(focus)};
            if(targets.Length==0)return false;
            Facing=facing;P.Mp-=skill.Mp;Cooldowns[skill.Id]=session.Now+skill.Cooldown;
            Running=new CombatAction(++StartedCount,skill,P.Position,facing,stats,session.Now,targets);
            consumedToken=pressToken;AssistAxis=0;approachStarted=-1;bufferSkill=null;BufferUntil=0;
            session.Emit($"Action {Running.Id} {skill.Id}; target {focus.Id}@{focus.Generation}; MP {P.Mp:F2}");session.ActionStarted(skill);return true;
        }
        public void Tick(bool manualContext) {
            var previous=FocusKind;
            if(Focus==null&&previous!=FocusKind.None){ClearFocus();if(previous==FocusKind.Auto)Acquire(Selected);}
            if(FocusKind==FocusKind.Auto&&Focus!=null&&manualContext&&
                (Focus.Position.Distance(P.Position)>(Selected?.Range??1.7)+ApproachBudget(Selected)||Math.Abs(Focus.Position.Y-P.Position.Y)>2)){ClearFocus();Acquire(Selected);}
            if(Running!=null){
                if(!P.Alive){Cancel();return;}
                if(!Running.Resolved&&session.Now>=Running.ResolveAt){Resolve(Running);Running.Resolved=true;}
                if(session.Now>=Running.EndAt)Running=null;
            }
            if(bufferSkill!=null){if(session.Now>BufferUntil){bufferSkill=null;BufferUntil=0;}
                else if(Running==null&&TryStart(bufferSkill))bufferSkill=null;}
            if(owner==null||heldSkill==null||!P.Alive||session.Now-pressedAt<.18||(!heldSkill.Repeat&&consumedToken==pressToken))return;
            if(Running!=null||Remaining(heldSkill)>0||P.Mp<heldSkill.Mp){AssistAxis=0;return;}
            if(Focus==null)Acquire(heldSkill);
            if(TryStart(heldSkill))return;
            var target=Focus;if(target==null||target.Returning){AssistAxis=0;return;}
            double missing=Math.Abs(target.Position.X-P.Position.X)-heldSkill.Range;
            if(Math.Abs(target.Position.Y-P.Position.Y)>1.3||missing>ApproachBudget(heldSkill)||missing<=0){AssistAxis=0;return;}
            if(approachStarted<0){approachStarted=session.Now;approachX=lastX=P.Position.X;lastProgressAt=session.Now;}
            if(Math.Abs(P.Position.X-lastX)>.04){lastX=P.Position.X;lastProgressAt=session.Now;}
            if(session.Now-approachStarted>ApproachBudget(heldSkill)/5+1 ||session.Now-lastProgressAt>.4||Math.Abs(P.Position.X-approachX)>ApproachBudget(heldSkill)){
                session.Emit("Tiếp cận dừng: hết budget/blocked. Cần input mới.");CancelIntent();return;}
            AssistAxis=target.Position.X<P.Position.X?-1:1;
        }
        private void Resolve(CombatAction a) {
            if(a.Skill.Shape==Shape.Explosion){
                var primary=session.Find(a.Targets[0].Id,a.Targets[0].Generation);
                if(primary==null||primary.Returning||!InRange(a.Skill,primary,a.Origin,a.Facing))return;
                Hit(a,primary,0,a.Skill.Power);
                foreach(var m in session.Mobs.Where(x=>x.Alive&&!x.Returning&&x.Map==P.Map&&x.Id!=primary.Id&&x.Position.Distance(primary.Position)<=2).OrderBy(x=>x.Position.Distance(primary.Position)).ThenBy(x=>x.Id).Take(4).ToArray())Hit(a,m,1,1.6);
                return;
            }
            for(int i=0;i<a.Targets.Length;i++){
                var key=a.Targets[i];var mob=session.Find(key.Id,key.Generation);if(mob==null||mob.Returning||!InRange(a.Skill,mob,a.Origin,a.Facing))continue;
                double power=a.Skill.Shape==Shape.Spread?new[]{.9,.8,.7}[i]:a.Skill.Shape==Shape.Line?2.8-.2*i:a.Skill.Power;
                Hit(a,mob,i,power);
            }
        }
        private void Hit(CombatAction a,Mob m,int index,double power) {
            bool evade=session.Random.NextDouble()<Rules.Evade(a.Acc,m.Eva),crit=!evade&&session.Random.NextDouble()<a.Crit;
            int damage=evade?0:Math.Min((int)Math.Ceiling(m.Hp),Rules.Damage(a.Atk,power,a.Bonus,m.Def,crit,.95+session.Random.NextDouble()*.1));
            if(!evade)session.Landed(m,damage);
            Results.Add(new HitResult{ActionId=a.Id,Target=m.Id,Generation=m.Generation,Index=index,Damage=damage,Evaded=evade,Crit=crit,Clock=session.Now});
            session.Emit(evade?"NÉ":$"Hit {a.Id}.{index} {m.Id}@{m.Generation}: {damage}" );
        }
    }
}
