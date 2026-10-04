using System;
namespace HuyenLo.Domain
{

    public enum ActionRejectReason
    {
        None,PlayerDead,HardCc,NotLearned,WeaponRequired,InsufficientMp,Cooldown,ActionLocked,
        NoTarget,TargetMissing,TargetDead,TargetGeneration,TargetReturning,WrongMap,
        OutOfRange,Blocked,BufferExpired,UserCancelled
    }

    // Per-binding snapshot used by the input adapter; no keyboard device dependency.
    public sealed class HeldMovementState
    {
        public int HeldAtPress {get;private set;}
        public void Snapshot(int mask)=>HeldAtPress=mask;
        public int Axis(int held,bool pending)=>ToAxis(pending?held&~HeldAtPress:held);
        public void Clear()=>HeldAtPress=0;
        public static int ToAxis(int mask)=>((mask&12)!=0?1:0)-((mask&3)!=0?1:0);
    }
    public static class InputBindings
    {
        public const string CycleTarget="<Keyboard>/tab",HpMain="H",MpMain="M",HpAlias="4",MpAlias="5";
        public static string HpGlyph()=>HpAlias+"/"+HpMain;
        public static string MpGlyph()=>MpAlias+"/"+MpMain;
        public static bool IsHp(string key)=>key==HpMain||key==HpAlias;
        public static bool IsMp(string key)=>key==MpMain||key==MpAlias;
    }
}
