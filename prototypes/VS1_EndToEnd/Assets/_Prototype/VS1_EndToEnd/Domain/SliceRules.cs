using System;
using System.Collections.Generic;
using System.Linq;

namespace HuyenLo.Domain
{
    public readonly struct Point
    {
        public readonly double X, Y;
        public Point(double x, double y) { X = x; Y = y; }
        public double Distance(Point p) => Math.Sqrt((X-p.X)*(X-p.X)+(Y-p.Y)*(Y-p.Y));
    }
    public enum Map { Village, Academy, Mist }
    public enum School { Novice, Sword }
    public enum FocusKind { None, Auto, Explicit }
    public enum Shape { Single, Arc, Line, Spread, Explosion }
    public enum QuestState { Locked, Available, InProgress, Ready, Completed }
    public enum GearSlot { None, Weapon, Armor, Pants, Boots, Ring, Necklace }
    public enum ItemKind { Gear, Food, HpPotion, MpPotion, Material, Manual, Scroll, Stone }

    public sealed class Skill
    {
        public readonly string Id, Name;
        public readonly int Slot, Mp, MaxTargets;
        public readonly double Range, Power, Cooldown, HitDelay, Lock, Vertical;
                public readonly Shape Shape;
        public Skill(string id, string name, int slot, int mp, double range, double power, double cd,
            double hit, double actionLock, Shape shape = Shape.Single, int maxTargets = 1, double vertical = 1.6)
        { Id=id; Name=name; Slot=slot; Mp=mp; Range=range; Power=power; Cooldown=cd; HitDelay=hit;
          Lock=actionLock; Shape=shape; MaxTargets=maxTargets; Vertical=vertical; }
    }
    public static class Rules
    {
        public static readonly int[] Exp = {0,100,250,470,790,1240,1860,2690,3770,5150,6900,9100,11800,15100,19100,23900,29600,36300,44100,53100};
        public static readonly Skill Novice = new Skill("novice", "Mộc Kiếm", 1, 0, 1.2, 1, 1, .10, .26);
        public static readonly Skill Sword1 = new Skill("sword.s1", "Phong Trảm nhập môn", 1, 2, 1.7, 1.2, 1, .12, .30);
        // Fixture definitions, not unlockable in VS-1. Same evaluator supports later gates.
        public static readonly Skill Sword2 = new Skill("sword.s2", "Phong Trảm tiến cảnh", 2, 4, 1.7, 1.35, 1.5, .14, .30, Shape.Arc, 3);
        public static readonly Skill Sword3 = new Skill("sword.s3", "Kiếm Khí", 3, 16, 5.5, 2.8, 7, .16, .40, Shape.Line, 5, .3);
        public static readonly Skill Bow1 = new Skill("bow.s1", "Linh Tiễn", 1, 2, 6.5, 1.15, 1, .12, .30);
        public static readonly Skill Bow2 = new Skill("bow.s2", "Ba Linh Tiễn", 2, 4, 6.5, .9, 1.7, .12, .34, Shape.Spread, 3);
        public static readonly Skill Bow3 = new Skill("bow.s3", "Hàn Tiễn", 3, 16, 6.5, 2.8, 7, .18, .40, Shape.Explosion, 5);
        public static int Round(double x) => (int)Math.Floor(x+.5);
        public static int Axis(bool leftA, bool leftArrow, bool rightD, bool rightArrow) =>
            ((rightD || rightArrow) ? 1 : 0) - ((leftA || leftArrow) ? 1 : 0);
        public static int Damage(double atk, double power, double bonus, double def, bool critical, double variation) =>
            Math.Max(1, Round(atk*power*bonus*100/(100+def)*variation*(critical?1.5:1)));
        public static double Evade(double acc, double eva) => .02+.43*eva/(eva+2.5*acc);
    }
    public sealed class ItemDef
    {
        public readonly string Id, Name;
        public readonly ItemKind Kind;
        public readonly GearSlot Slot;
        public readonly int Buy, Sell, MinLevel, Stack;
        public readonly double Hp, Mp, Atk, Def, Acc, Eva, Crit, Speed;
        public ItemDef(string id, string name, ItemKind kind, int buy, int sell, GearSlot slot=GearSlot.None,
            int level=1, double hp=0, double mp=0, double atk=0, double def=0, double acc=0, double eva=0, double crit=0, double speed=0)
        { Id=id; Name=name; Kind=kind; Buy=buy; Sell=sell; Slot=slot; MinLevel=level;
          Hp=hp; Mp=mp; Atk=atk; Def=def; Acc=acc; Eva=eva; Crit=crit; Speed=speed;
          Stack=kind==ItemKind.Gear || kind==ItemKind.Manual ? 1 : 99; }
    }
    public static class Catalog
    {
        public static readonly Dictionary<string,ItemDef> Items = new[] {
            new ItemDef("wood", "Mộc Kiếm", ItemKind.Gear, 0, 0, GearSlot.Weapon, atk:10),
            new ItemDef("sword1", "Thanh Mộc Kiếm", ItemKind.Gear, 300,75,GearSlot.Weapon,5,atk:15,crit:.005),
            new ItemDef("armor1", "Áo Thanh Mộc", ItemKind.Gear,220,55,GearSlot.Armor,hp:40,def:4),
            new ItemDef("pants1", "Quần Thanh Mộc", ItemKind.Gear,160,40,GearSlot.Pants,hp:25,def:3),
            new ItemDef("boots1", "Giày Thanh Mộc", ItemKind.Gear,140,35,GearSlot.Boots,def:2,eva:4,speed:.01),
            new ItemDef("ring1", "Nhẫn Thanh Mộc", ItemKind.Gear,140,35,GearSlot.Ring,acc:6,crit:.01),
            new ItemDef("neck1", "Dây chuyền Thanh Mộc", ItemKind.Gear,160,40,GearSlot.Necklace,mp:20,eva:5),
            new ItemDef("food1", "Food I", ItemKind.Food,150,37),
            new ItemDef("hp1", "Bình Máu I", ItemKind.HpPotion,80,20),
            new ItemDef("mp1", "Bình Linh Lực I", ItemKind.MpPotion,80,20),
            new ItemDef("manual1", "Phong Trảm Kiếm Phổ", ItemKind.Manual,0,0),
            new ItemDef("mushroom", "Nấm Sương", ItemKind.Material,0,2),
            new ItemDef("fang", "Nanh Sói", ItemKind.Material,0,3),
            new ItemDef("scroll", "Hồi Sinh Phù", ItemKind.Scroll,1000,250),
            new ItemDef("stone", "Tinh Thạch", ItemKind.Stone,800,200)
        }.ToDictionary(x=>x.Id);
        public static ItemDef Get(string id) => Items[id];
    }
    public sealed class Item
    {
        public long Instance;
        public string Id;
        public int Count;
        public string Binding;
        public double Quality=1;
        public Item(long instance,string id,int count=1,string binding=null,double quality=1)
        { Instance=instance; Id=id; Count=count; Binding=binding; Quality=quality; }
        public Item Copy() => new Item(Instance,Id,Count,Binding,Quality);
    }
    public sealed class Inventory
    {
        public readonly List<Item> Bag = new List<Item>();
        public readonly Dictionary<GearSlot,Item> Equipment = new Dictionary<GearSlot,Item>();
        public int Capacity=30;
        public int Count(string id) => Bag.Where(x=>x.Id==id).Sum(x=>x.Count);
        private static List<Item> Merge(List<Item> original,IEnumerable<Item> incoming)
        {
            var copy=original.Select(x=>x.Copy()).ToList();
            foreach(var value in incoming) {
                int left=value.Count, stack=Catalog.Get(value.Id).Stack;
                foreach(var target in copy.Where(x=>x.Id==value.Id && x.Binding==value.Binding && x.Quality==value.Quality && x.Count<stack)) {
                    int add=Math.Min(left,stack-target.Count); target.Count+=add;left-=add;if(left==0)break;
                }
                while(left>0){var next=value.Copy();next.Count=Math.Min(left,stack);copy.Add(next);left-=next.Count;}
            }
            return copy;
        }
        public bool Fits(IEnumerable<Item> items) => Merge(Bag,items).Count<=Capacity;
        public bool Add(IEnumerable<Item> items)
        { var merged=Merge(Bag,items);if(merged.Count>Capacity)return false;Bag.Clear();Bag.AddRange(merged);return true; }
        public bool Consume(Item item)
        {if(!Bag.Contains(item)||item.Count<1)return false;item.Count--;if(item.Count==0)Bag.Remove(item);return true;}
    }
    public sealed class Stats
    {
        public double Hp,Mp,Atk,Def,Acc,Eva,Crit,Bonus,Speed;
    }
    public sealed class Player
    {
        public readonly long Id=1;
        public Map Map=Map.Village;
        public Point Position = new Point(0,.8);
        public School School;
        public int Level=1, TotalExp, Gold, Unspent, Str, Vit, Int, Agi, Allocated;
        public bool ResetAtFive;
        public double Hp=120,Mp=60,InvulnerableUntil;
        public readonly Inventory Inventory=new Inventory();
        public readonly Inventory Storage=new Inventory {Capacity=40};
        public bool Alive => Hp>0;
        public Stats Stats {
            get {
                var s=new Stats{Hp=120+10*(Level-1)+8*Vit,Mp=60+4*(Level-1)+5*Int,
                    Atk=12+1.2*(Level-1)+.7*Str,Def=5+.6*(Level-1)+.1*Vit,
                    Acc=60+4*(Level-1)+6*Agi,Eva=20+2*(Level-1)+6*Agi,Crit=.05,Bonus=1+.0035*Int,Speed=1+.0005*Agi};
                foreach(var i in Inventory.Equipment.Values) {
                    var d=Catalog.Get(i.Id);s.Hp+=d.Hp*i.Quality;s.Mp+=d.Mp*i.Quality;s.Atk+=d.Atk*i.Quality;
                    s.Def+=d.Def*i.Quality;s.Acc+=d.Acc*i.Quality;s.Eva+=d.Eva*i.Quality;s.Crit+=d.Crit;s.Speed+=d.Speed;
                }
                if(School==School.Sword){s.Hp*=1.1;s.Def*=1.08;}return s;
            }
        }
        public void Clamp(){Hp=Math.Min(Hp,Stats.Hp);Mp=Math.Min(Mp,Stats.Mp);}
        public void AddExp(int value) {
            TotalExp=Math.Min(Rules.Exp[19],TotalExp+Math.Max(0,value));
            while(Level<20 && TotalExp>=Rules.Exp[Level]) {
                Level++;
                if(Level<5){Str+=2;Vit+=2;Int++;}
                else if(Level==5 && !ResetAtFive){Unspent+=20;Str=Vit=Int=Agi=0;Allocated=0;ResetAtFive=true;}
                else Unspent+=5;
                // No undocumented full-heal on level up.
                Clamp();
            }
        }
    }
    public sealed class Mob
    {
        public int Id,Generation=1,Level;
        public string Name,Slot;
        public Map Map;
        public Point Position,Home,PreviousPosition;
        public int Lane,ApproachSide;
        public double LaneMin=-10000,LaneMax=10000;
        public double ActivityMin=-10000,ActivityMax=10000,PatrolGoal,PatrolPauseUntil;
        public int PatrolDirection;
        public double Hp,MaxHp,Atk,Def,Acc,Eva,Speed,Range,Interval,NextAttack,HitAt,RespawnAt,ReturnSince=-1,QuestDamage,LastDamage;
        // Probe telemetry: no gameplay decisions depend on the view or trace consumer.
        public double DesiredX,VelocityX,RepositionUntil;
        public int BiteAttempts;
        public string Motion="idle",Occupancy="clear";
        public int Facing=1,QuestTag;
        public bool Dummy,Returning,Windup,Engaged;
        public bool Alive => Hp>0;
        public Mob(int id,string slot,string name,Map map,Point home,int level,double hp,double atk,double def,double acc,double eva,double speed,double range,double interval,bool dummy=false)
        {Id=id;Slot=slot;Name=name;Map=map;Home=Position=PreviousPosition=home;Level=level;Hp=MaxHp=hp;Atk=atk;Def=def;Acc=acc;Eva=eva;Speed=speed;Range=range;Interval=interval;Dummy=dummy;}
    }
    public sealed class Loot
    {
        public long Id,Owner=1;
        public int MobLevel,LevelAtDeath;
        public Map Map;
        public Point Position;
        public double Created;
        public bool Claimed,Tutorial;
        public Item Item;
        public bool Eligible(Player p,double now) => !Claimed && p.Alive && p.Map==Map && p.Position.Distance(Position)<=1.5 &&
            (Tutorial || (now-Created<60 && Math.Abs((p.Id==Owner?LevelAtDeath:p.Level)-MobLevel)<=3 && (now-Created>=20 || p.Id==Owner)));
    }
}
