using System;
namespace HuyenLo.Domain
{
    // Disposable local fixtures only. Never called by the continuous acceptance route.
    public enum PrototypeStart { Fresh=1, Movement=2, Dummy=3, FirstLoot=4, Wolves=5, Class=6, SwordTraining=7, Crowd=8 }
    public static class PrototypePresets
    {
        public static SliceSession Create(PrototypeStart start)
        {
            if(!Enum.IsDefined(typeof(PrototypeStart),start))throw new ArgumentOutOfRangeException(nameof(start));
            var s=new SliceSession();if(start==PrototypeStart.Fresh)return s;
            s.DebugPreset=start.ToString();s.Quest=(int)start;
            s.QuestState=start>=PrototypeStart.SwordTraining?QuestState.Completed:QuestState.Available;
            for(int i=1;i<s.Quest;i++)s.Receipts.Add($"Q{i}.completed");
            int exp=start<=PrototypeStart.Dummy?start==PrototypeStart.Movement?100:250:start==PrototypeStart.FirstLoot?250:start==PrototypeStart.Wolves?470:790;
            s.Player.AddExp(exp);s.Player.Gold=125;
            if(start>=PrototypeStart.FirstLoot){
                s.Player.Inventory.Equipment[GearSlot.Weapon]=s.NewItem("wood",binding:"Q3");
                s.Player.Inventory.Equipment[GearSlot.Pants]=s.NewItem("pants1");
                s.Receipts.Add("Q3.wood");s.Receipts.Add("Q3.reward");
            }
            if(start>=PrototypeStart.Wolves){s.Player.Inventory.Equipment[GearSlot.Armor]=s.NewItem("armor1");s.Receipts.Add("Q4.supply");}
            if(start>=PrototypeStart.Class){s.Player.Inventory.Add(new[]{s.NewItem("food1",2),s.NewItem("hp1",3)});s.Receipts.Add("Q5.supply-gold");s.Player.Gold=200;}
            s.Player.Map=start==PrototypeStart.Dummy||start==PrototypeStart.SwordTraining?Map.Academy:Map.Village;
            s.Player.Position=new Point(start==PrototypeStart.FirstLoot?10:start==PrototypeStart.Wolves?5:start==PrototypeStart.Class?20:0,.8);
            if(start==PrototypeStart.FirstLoot||start==PrototypeStart.Wolves||start==PrototypeStart.Class)s.Player.Position=Array.Find(SliceSession.Anchors,x=>x.Id==s.QuestNpc).Position;
            if(start>=PrototypeStart.SwordTraining){
                s.Player.School=School.Sword;s.Player.Inventory.Equipment[GearSlot.Weapon]=s.NewItem("sword1");
                s.Combat.Unlocked[1]=Rules.Sword1;s.Player.Inventory.Add(new[]{s.NewItem("mp1",3)});
                s.Player.Position=new Point(20,.8);
            }
            if(start==PrototypeStart.Crowd){s.Player.Map=Map.Mist;s.Player.Position=new Point(85,1.92);s.Player.Inventory.Add(new[]{s.NewItem("hp1",20),s.NewItem("food1",4)});}
            s.Player.Hp=s.Player.Stats.Hp;s.Player.Mp=s.Player.Stats.Mp;
            s.Emit("DEBUG preset "+start+" — không là evidence hành trình.");return s;
        }
    }
}
