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
        public ActionRejectReason LastReject {get;private set;}
        public bool HasBufferedCast=>bufferSkill!=null;
        public CombatAction Running {get;private set;}
        public FocusKind FocusKind {get;private set;}
        public int FocusId {get;private set;}
        public int FocusGeneration {get;private set;}
        public int SelectedSlot=1,Facing=1;
        public int AssistAxis {get;private set;}
        public double BufferUntil {get;private set;}
        public long StartedCount {get;private set;}
        private Skill pendingSkill, bufferSkill;
        private TargetKey pendingTarget, bufferTarget;
        private double approachStarted, approachX, lastProgressAt, lastX;
        public bool HasPendingCast => pendingSkill != null;
        public CombatController(SliceSession value){session=value;}
        public Mob Focus => session.Find(FocusId,FocusGeneration);
        public Skill Selected => P.School==School.Novice?Rules.Novice:Unlocked.TryGetValue(SelectedSlot,out var s)?s:null;
        private bool Available(Skill s) => s!=null && (P.School==School.Novice ? s.Id=="novice" && P.Inventory.Equipment.TryGetValue(GearSlot.Weapon,out var item) && item.Id=="wood":P.Inventory.Equipment.TryGetValue(GearSlot.Weapon,out var weapon) && weapon.Id=="sword1" && Unlocked.Values.Any(x=>x.Id==s.Id));
        public double Remaining(Skill skill) => skill!=null&&Cooldowns.TryGetValue(skill.Id,out var end)?Math.Max(0,end-session.Now):0;
        public void Explicit(Mob target) {
            CancelIntent();
            if(!P.Alive||!InSearch(target))return;
            FocusId=target.Id;FocusGeneration=target.Generation;FocusKind=FocusKind.Explicit;session.Emit("Focus EXPLICIT "+target.Slot);
        }
        public void ClearFocus(){CancelIntent();FocusId=FocusGeneration=0;FocusKind=FocusKind.None;}
        // Selection/retention are local usability bounds, independent of execution geometry.
        private bool Relevant(Mob m) => m!=null&&m.Alive&&!m.Returning&&m.Map==P.Map;
        private bool InSearch(Mob m) => Relevant(m)&&Math.Abs(m.Position.X-P.Position.X)<=12&&Math.Abs(m.Position.Y-P.Position.Y)<=6;
        private bool InRetention(Mob m) => Relevant(m)&&Math.Abs(m.Position.X-P.Position.X)<=20&&Math.Abs(m.Position.Y-P.Position.Y)<=10;
        private bool InEnvelope(Mob m,Skill skill) => m!=null && m.Alive && !m.Returning && m.Map==P.Map && m.Position.Distance(P.Position)<=skill.Range+ApproachBudget(skill) && Math.Abs(m.Position.Y-P.Position.Y)<=skill.Vertical;
        private void Acquire(Skill skill) {
            skill=skill??Rules.Novice;
            Mob target=null;double nearest=double.PositiveInfinity;
            foreach(var m in session.Mobs){if(!InEnvelope(m,skill))continue;double d=m.Position.Distance(P.Position);if(d<nearest||d==nearest&&(target==null||m.Id<target.Id)){target=m;nearest=d;}}
            if(target==null){FocusId=FocusGeneration=0;FocusKind=FocusKind.None;return;}
            FocusId=target.Id;FocusGeneration=target.Generation;FocusKind=FocusKind.Auto;
        }
        // Probe values only; the production gate measures distance/progress at the real scale.
        private static double ApproachBudget(Skill s) => s==null?1:Math.Max(1,Math.Min(3,s.Range*1.5));
        private void Reject(string reason,ActionRejectReason code=ActionRejectReason.None){
            CancelIntent();session.Feedback=reason;LastReject=code;
        }
        public void RejectBlocked(){Reject("Tiếp cận bị chặn bởi cổng/địa hình — không tiêu MP/CD.",ActionRejectReason.Blocked);}
        public bool ApproachCrossesExit(Point from,Point to) {
            return SliceSession.Anchors.Any(x=>x.EdgeExit&&x.Map==P.Map&&Math.Abs(from.Y-x.Position.Y)<=1.6&&
                Math.Min(from.X,to.X)<=x.Position.X+.65&&Math.Max(from.X,to.X)>=x.Position.X-.65);
        }
        public void Escape() {
            if(HasPendingCast||HasBufferedCast){Reject("Đã hủy thao tác.",ActionRejectReason.UserCancelled);return;}
            if(FocusKind!=FocusKind.None)ClearFocus();
        }
        public bool CycleTarget(int direction) {
            if(!P.Alive)return false;
            var candidates=session.Mobs.Where(x=>InSearch(x))
                .OrderBy(x=>Math.Abs(x.Position.X-P.Position.X)).ThenBy(x=>x.Slot,StringComparer.Ordinal).ToArray();
            if(candidates.Length==0)return false;
            int current=Array.FindIndex(candidates,x=>x.Id==FocusId&&x.Generation==FocusGeneration);
            int next=current<0?(direction<0?candidates.Length-1:0):(current+(direction<0?-1:1)+candidates.Length)%candidates.Length;
            Explicit(candidates[next]);return true;
        }
        private ActionRejectReason Validate(Skill skill,TargetKey key,bool requireRange) {
            if(!P.Alive)return ActionRejectReason.PlayerDead;
            if(P.HardCc)return ActionRejectReason.HardCc;
            var m=session.Mobs.FirstOrDefault(x=>x.Id==key.Id);
            if(m==null)return ActionRejectReason.TargetMissing;
            if(m.Generation!=key.Generation)return ActionRejectReason.TargetGeneration;
            if(!m.Alive)return ActionRejectReason.TargetDead;
            if(m.Map!=P.Map)return ActionRejectReason.WrongMap;
            if(m.Returning)return ActionRejectReason.TargetReturning;
            if(P.School==School.Novice?skill.Id!="novice":!Unlocked.Values.Any(x=>x.Id==skill.Id))return ActionRejectReason.NotLearned;
            if(!Available(skill))return ActionRejectReason.WeaponRequired;
            if(P.Mp<skill.Mp)return ActionRejectReason.InsufficientMp;
            if(Remaining(skill)>0)return ActionRejectReason.Cooldown;
            if(Running!=null)return ActionRejectReason.ActionLocked;
            if(requireRange&&!Witness(skill,m,P.Position,m.Position.X<P.Position.X?-1:1))return ActionRejectReason.OutOfRange;
            return ActionRejectReason.None;
        }
        private void ValidationReject(ActionRejectReason reason)=>Reject("Không thể thực hiện: "+reason+" — không tiêu MP/CD.",reason);
        public void Press(string key,int slot=0) {
            LastReject=ActionRejectReason.None;
            if(!int.TryParse(key,out var digit)||digit<1||digit>3)return;
            if(slot==0)slot=digit;
            if(P.School==School.Novice?slot!=1:!Unlocked.ContainsKey(slot)) {ValidationReject(ActionRejectReason.NotLearned);return;}
            SelectedSlot=slot;
            var skill=Selected;
            CancelIntent();
            if(!P.Alive||P.HardCc){ValidationReject(!P.Alive?ActionRejectReason.PlayerDead:ActionRejectReason.HardCc);return;}
            if(!P.Alive||!Available(skill)){Reject("Cần mặc vũ khí và học kỹ năng.",!P.Alive?ActionRejectReason.PlayerDead:ActionRejectReason.WeaponRequired);return;}
            if(P.Mp<skill.Mp){Reject("Không đủ Linh lực. Dùng bình hoặc Food.",ActionRejectReason.InsufficientMp);return;}
            if(Focus==null)Acquire(skill);
            if(Focus==null||Focus.Returning){Reject("Không có mục tiêu hợp lệ — không tiêu MP/CD.",ActionRejectReason.NoTarget);return;}
            if(Running!=null||Remaining(skill)>0){
                double ready=Math.Max(Remaining(skill),Running==null?0:Running.EndAt-session.Now);
                if(ready>0&&ready<=.18){bufferSkill=skill;bufferTarget=new TargetKey(Focus);BufferUntil=session.Now+.18;return;}
                Reject("Chưa sắp sẵn — cần bấm mới.",Running!=null?ActionRejectReason.ActionLocked:ActionRejectReason.Cooldown);return;
            }
            BeginOrApproach(skill,Focus);
        }
        // A physical release never repeats or cancels the one-shot pending intent.
        public void Release(string key) { }
        public void ManualOverride(){CancelIntent();}
        public void CancelIntent(){pendingSkill=bufferSkill=null;BufferUntil=0;AssistAxis=0;}
        public void Cancel(){Running=null;CancelIntent();}
        private bool InRange(Skill skill,Mob m,Point origin,int facing) {
            double dx=m.Position.X-origin.X,dy=m.Position.Y-origin.Y;
            if(Math.Abs(dy)>skill.Vertical)return false;
            if(skill.Shape==Shape.Line)return dx*facing>=0 && dx*facing<=skill.Range && Math.Abs(dy)<=.3;
            if(skill.Shape==Shape.Arc)return m.Position.Distance(origin)<=skill.Range && dx*facing>=Math.Abs(dy)/Math.Sqrt(3);
            return m.Position.Distance(origin)<=skill.Range;
        }
        private IEnumerable<Mob> Eligible(Skill skill,Point origin,int facing) => session.Mobs.Where(x=>x.Alive&&x.Map==P.Map&&!x.Returning&&InRange(skill,x,origin,facing));
        private bool Witness(Skill skill,Mob focus,Point origin,int facing) => skill.Shape==Shape.Line||skill.Shape==Shape.Arc?Eligible(skill,origin,facing).Any():InRange(skill,focus,origin,facing);
        private void BeginOrApproach(Skill skill,Mob focus) {
            int facing=focus.Position.X<P.Position.X?-1:1;
            if(Running!=null){ValidationReject(ActionRejectReason.ActionLocked);return;}
            if(Witness(skill,focus,P.Position,facing)){TryStart(skill);return;}
            double dy=focus.Position.Y-P.Position.Y;
            double horizontal=Math.Sqrt(Math.Max(0,skill.Range*skill.Range-dy*dy))-.12;
            double missing=Math.Abs(focus.Position.X-P.Position.X)-horizontal;
            var prospective=new Point(P.Position.X+facing*Math.Max(0,missing),P.Position.Y);
            if(Math.Abs(dy)>Math.Min(1.3,skill.Vertical)||missing<=0||missing>ApproachBudget(skill)||!Witness(skill,focus,prospective,facing)){
                Reject("Quá xa hoặc khác tầng — hãy tự di chuyển.",ActionRejectReason.OutOfRange);return;
            }
            if(ApproachCrossesExit(P.Position,prospective)){RejectBlocked();return;}
            pendingSkill=skill;pendingTarget=new TargetKey(focus);approachStarted=lastProgressAt=session.Now;approachX=lastX=P.Position.X;
            AssistAxis=facing;session.Feedback="Đang tiếp cận — hướng/nhảy/Esc để hủy.";
        }
        private bool TryStart(Skill skill) {
            {
                var raw=session.Mobs.FirstOrDefault(x=>x.Id==FocusId);
                if(raw==null){ValidationReject(ActionRejectReason.TargetMissing);return false;}
                var key=HasPendingCast?pendingTarget:new TargetKey(raw);
                var reason=Validate(skill,key,false);
                if(reason!=ActionRejectReason.None){ValidationReject(reason);return false;}
            }
            if(!P.Alive||!Available(skill)||Running!=null||Remaining(skill)>0||P.Mp<skill.Mp)return false;
            var focus=Focus;
            if(focus==null||focus.Returning)return false;
            var stats=P.Stats;int facing=focus.Position.X<P.Position.X?-1:1;
            if(!Witness(skill,focus,P.Position,facing))return false;
            var eligible=Eligible(skill,P.Position,facing);
            TargetKey[] targets;
            if(skill.Shape==Shape.Line)targets=eligible.OrderBy(x=>(x.Position.X-P.Position.X)*facing).ThenBy(x=>x.Id).Take(5).Select(x=>new TargetKey(x)).ToArray();
            else if(skill.Shape==Shape.Arc)targets=eligible.OrderBy(x=>x.Id==focus.Id?0:1).ThenBy(x=>x.Position.Distance(P.Position)).ThenBy(x=>x.Id).Take(3).Select(x=>new TargetKey(x)).ToArray();
            else if(skill.Shape==Shape.Spread){var rest=eligible.Where(x=>x.Id!=focus.Id).OrderBy(x=>x.Position.Distance(P.Position)).ThenBy(x=>x.Id).Take(2).ToArray();
                targets=new[]{new TargetKey(focus),new TargetKey(rest.Length>0?rest[0]:focus),new TargetKey(rest.Length>1?rest[1]:focus)};}
            else targets=new[]{new TargetKey(focus)};
            if(targets.Length==0)return false;
            Facing=facing;P.Mp-=skill.Mp;Cooldowns[skill.Id]=session.Now+skill.Cooldown;
            Running=new CombatAction(++StartedCount,skill,P.Position,facing,stats,session.Now,targets);
            CancelIntent();
            session.Emit($"Action {Running.Id} {skill.Id}; target {focus.Id}@{focus.Generation}; MP {P.Mp:F2}");session.ActionStarted(skill);return true;
        }
        public void Tick(bool manualContext) {
            if(pendingSkill!=null){
                var reason=Validate(pendingSkill,pendingTarget,false);
                if(reason!=ActionRejectReason.None){ValidationReject(reason);return;}
                if(!InEnvelope(session.Find(pendingTarget.Id,pendingTarget.Generation),pendingSkill)){ValidationReject(ActionRejectReason.OutOfRange);return;}
            }
            var previous=FocusKind;
            if(previous!=FocusKind.None&&!InRetention(Focus)){
                bool hadIntent=HasPendingCast||HasBufferedCast;ClearFocus();
                if(hadIntent)session.Feedback="Mục tiêu đã mất hoặc ra khỏi tầm tìm — cần bấm mới.";
                Acquire(Selected);
            }
            if(Running!=null){
                if(!P.Alive){Cancel();return;}
                if(!Running.Resolved&&session.Now>=Running.ResolveAt){Resolve(Running);Running.Resolved=true;}
                if(session.Now>=Running.EndAt)Running=null;
            }
            if(bufferSkill!=null){
                var skill=bufferSkill;var target=session.Find(bufferTarget.Id,bufferTarget.Generation);
                if(session.Now>BufferUntil||target==null||target.Returning||FocusId!=target.Id||FocusGeneration!=target.Generation){
                    {var reason=Validate(skill,bufferTarget,false);ValidationReject(reason==ActionRejectReason.None?ActionRejectReason.BufferExpired:reason);}
                }
                else if(Running==null&&Remaining(skill)<=0){bufferSkill=null;BufferUntil=0;BeginOrApproach(skill,target);}
            }
            if(pendingSkill==null)return;
            var mob=session.Find(pendingTarget.Id,pendingTarget.Generation);
            if(!P.Alive||mob==null||mob.Returning||mob.Id!=FocusId||mob.Generation!=FocusGeneration){Reject("Mục tiêu không còn hợp lệ — cần bấm mới.");return;}
            if(P.Mp<pendingSkill.Mp||Remaining(pendingSkill)>0||!Available(pendingSkill)){Reject("Kỹ năng chưa sẵn — tiếp cận đã hủy.");return;}
            var active=pendingSkill;
            if(TryStart(active)||pendingSkill==null)return;
            {
                int nextAxis=mob.Position.X<P.Position.X?-1:1;
                if(ApproachCrossesExit(P.Position,new Point(P.Position.X+nextAxis*.1,P.Position.Y))){RejectBlocked();return;}
            }
            if(Math.Abs(P.Position.X-lastX)>.04){lastX=P.Position.X;lastProgressAt=session.Now;}
            if(session.Now-approachStarted>ApproachBudget(pendingSkill)/5+1||session.Now-lastProgressAt>.45||Math.Abs(P.Position.X-approachX)>ApproachBudget(pendingSkill)||Math.Abs(mob.Position.Y-P.Position.Y)>1.3){Reject("Tiếp cận dừng: blocked/hết giới hạn. Bấm mới để thử lại.",Math.Abs(mob.Position.Y-P.Position.Y)>1.3?ActionRejectReason.OutOfRange:ActionRejectReason.Blocked);return;}
            AssistAxis=mob.Position.X<P.Position.X?-1:1;
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
