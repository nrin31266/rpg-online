using System;
namespace HuyenLo.Domain
{
    // All defaults OFF. Disposable A/B controls, never a production rules authority.
    [Serializable] public sealed class ProbeConfig
    {
        public bool Buffer180,StaleHeldAxis,StrictArrival,ApproachExitSafety,EscapePriority;
        public bool NoviceCadence,PotionAliases,TabCycleTarget,StableMelee;
        public double BufferWindow=>Buffer180?.18:.15;
        public bool Any=>Buffer180||StaleHeldAxis||StrictArrival||ApproachExitSafety||EscapePriority||NoviceCadence||PotionAliases||TabCycleTarget||StableMelee;
        public static ProbeConfig Experimental()=>new ProbeConfig{Buffer180=true,StaleHeldAxis=true,StrictArrival=true,ApproachExitSafety=true,EscapePriority=true,NoviceCadence=true,PotionAliases=true,TabCycleTarget=true,StableMelee=true};
    }
    public enum ActionRejectReason
    {
        None,PlayerDead,HardCc,NotLearned,WeaponRequired,InsufficientMp,Cooldown,ActionLocked,
        NoTarget,TargetMissing,TargetDead,TargetGeneration,TargetReturning,WrongMap,
        OutOfRange,Blocked,BufferExpired,UserCancelled
    }
    public readonly struct ActionRejection
    {
        public readonly double Clock;public readonly ActionRejectReason Reason;
        public ActionRejection(double clock,ActionRejectReason reason){Clock=clock;Reason=reason;}
    }
    // Shared by the input adapter and deterministic probes; no keyboard device dependency.
    public sealed class HeldAxisProbe
    {
        public int HeldAtPress {get;private set;}
        public void Snapshot(int mask)=>HeldAtPress=mask;
        public int Axis(int held,bool pending)=>ToAxis(pending?held&~HeldAtPress:held);
        public void Clear()=>HeldAtPress=0;
        public static int ToAxis(int mask)=>((mask&12)!=0?1:0)-((mask&3)!=0?1:0);
    }
    public static class ProbeBindings
    {
        public const string CycleTarget="<Keyboard>/tab",HpMain="H",MpMain="M",HpAlias="4",MpAlias="5";
        public static string HpGlyph(ProbeConfig c)=>c.PotionAliases?HpAlias+"/"+HpMain:HpMain;
        public static string MpGlyph(ProbeConfig c)=>c.PotionAliases?MpAlias+"/"+MpMain:MpMain;
        public static bool IsHp(string key,ProbeConfig c)=>key==HpMain||(c.PotionAliases&&key==HpAlias);
        public static bool IsMp(string key,ProbeConfig c)=>key==MpMain||(c.PotionAliases&&key==MpAlias);
    }
}
