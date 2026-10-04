using System;
using System.Collections.Generic;
using System.Linq;

namespace HuyenLo.Domain
{
    public sealed class Anchor
    {
        public readonly string Id,Name;
        public readonly Map Map;
        public readonly Point Position;
        public readonly bool EdgeExit;
        public Anchor(string id,string name,Map map,double x,double y=.8,bool exit=false)
        {Id=id;Name=name;Map=map;Position=new Point(x,y);EdgeExit=exit;}
    }
    public sealed class SliceSession
    {
        public readonly Player Player=new Player();
        public readonly List<Mob> Mobs=new List<Mob>();
        public readonly List<Loot> Loot=new List<Loot>();
        public readonly List<string> Events=new List<string>();
        public readonly HashSet<string> Receipts=new HashSet<string>();
        public readonly CombatController Combat;
        public readonly ProbeConfig Probes;
        public readonly Random Random;
        public double Now {get;private set;}
        public double FoodUntil,NextFood,HpPotionUntil,MpPotionUntil;
        public int WorldRevision {get;private set;}
        public int Quest=1,Stage,Kills;
        public QuestState QuestState=QuestState.Available;
        public bool Complete => Quest>6;
        public string Feedback="Vân Khê — hãy nói chuyện với Lâm Bá (E).";
        public Action<string> OnEvent;
        public string DebugPreset {get;internal set;}
        private string exitLatch;
        public readonly HashSet<string> Purchased=new HashSet<string>();
        private long nextItem=1000;
        private readonly Dictionary<string,Item[]> pendingGrants=new Dictionary<string,Item[]>();
        public double LastHurtAt {get;private set;}=-100;
        private long tutorialArmor,tutorialSample;
        public static readonly Anchor[] Anchors={
            new Anchor("Lam","Lâm Bá",Map.Village,0), new Anchor("Yen","Yên Thảo",Map.Village,6),
            new Anchor("Bach","Bách Luyện",Map.Village,13,1.4),new Anchor("Moc","Mộc An",Map.Village,20),
            new Anchor("Ta","Tạ Minh",Map.Village,26,4.4),
            new Anchor("toAcademy","← Học Viện",Map.Village,-4,exit:true),new Anchor("toMist","Đồng Sương →",Map.Village,30,exit:true),
            new Anchor("toVillageA","Vân Khê →",Map.Academy,34,exit:true),new Anchor("Phong","Phong Du",Map.Academy,0),
            new Anchor("Diep","Diệp Lam",Map.Academy,-12,7.2),
            new Anchor("toVillageM","← Vân Khê",Map.Mist,-4,exit:true),
            new Anchor("outOfSlice","Trúc Ảnh →",Map.Mist,126,exit:true)
        };
        public SliceSession(int seed=731,ProbeConfig probes=null) {
            Probes=probes??new ProbeConfig();
            Random=new Random(seed);Combat=new CombatController(this);
            // Dummy DEF/EVA=0 are explicit VS-1 probe values: GDD only fixes its HP.
            for(int i=0;i<3;i++)Mobs.Add(new Mob(10+i,"HV_Dummy.slot"+(i+1),"Bù Nhìn",Map.Academy,new Point(22+i*2,.8),3,60,0,0,0,0,0,0,0,true));
            Mobs.Add(new Mob(20,"DS1.slot1","Nấm Linh",Map.Mist,new Point(5,.65),2,48,9,4,68,24,1.2,.8,1.8));
            Mobs.Add(new Mob(21,"DS2.slot1","Nấm Linh",Map.Mist,new Point(15,.65),2,48,9,4,68,24,1.2,.8,1.8));
            int id=30;
            for(int pocket=3;pocket<=6;pocket++)for(int slot=1;slot<=2;slot++)
                Mobs.Add(new Mob(id++,$"DS{pocket}.slot{slot}","Sói Sương",Map.Mist,new Point((pocket==5?58:30+(pocket-3)*20)+(slot-1)*2,.65),4,107,13,5,76,28,2.4,1,1.3));
            // Two extra mock pockets are explicitly probes, outside the release DS1–DS6 budget/quest credit.
            for(int i=0;i<4;i++)Mobs.Add(new Mob(60+i,"PROBE7.slot"+(i+1),"Sói Sương",Map.Mist,new Point(106+i*1.5,.65),4,107,13,5,76,28,2.4,1,1.3){Lane=7,LaneMin=101,LaneMax=121});
            for(int i=0;i<3;i++)Mobs.Add(new Mob(70+i,"PROBE8.slot"+(i+1),"Sói Sương",Map.Mist,new Point(111+i*2,5.25),4,107,13,5,76,28,2.4,1,1.3){Lane=8,LaneMin=49.5,LaneMax=58.5});
            foreach(var mob in Mobs.Where(x=>x.Map==Map.Mist)){
                var lane=BlockoutLayout.MobSupport(mob.Slot,mob.Home.X);
                mob.Home=mob.Position=mob.PreviousPosition=new Point(mob.Home.X,lane.Y+lane.Height/2+.65);
                mob.LaneMin=lane.X-lane.Width/2+.45;mob.LaneMax=lane.X+lane.Width/2-.45;
            }
            foreach(var group in Mobs.Where(x=>!x.Dummy).GroupBy(x=>x.Slot.Split('.')[0])){
                double low=group.Max(x=>x.LaneMin),high=group.Min(x=>x.LaneMax);
                if(!group.Key.StartsWith("PROBE")){low=Math.Max(low,group.Min(x=>x.Home.X)-6);high=Math.Min(high,group.Max(x=>x.Home.X)+6);}
                foreach(var mob in group){mob.ActivityMin=low;mob.ActivityMax=high;mob.PatrolDirection=mob.Id%2==0?1:-1;mob.PatrolGoal=mob.Home.X;mob.PatrolPauseUntil=(mob.Id%5)*.35;}
            }
        }
        public void Emit(string value){Feedback=value;Events.Add($"{Now:F2} {value}");OnEvent?.Invoke(Events[Events.Count-1]);}
        public Item NewItem(string id,int count=1,string binding=null,double quality=1) => new Item(++nextItem,id,count,binding,quality);
        public bool Grant(string receipt,params Item[] items) {
            if(Receipts.Contains(receipt))return true;
            if(!pendingGrants.TryGetValue(receipt,out var pending)){pending=items;pendingGrants.Add(receipt,pending);}
            if(!Player.Inventory.Add(pending)){Emit("Cần ô trống trong hành trang; có thể thử lại.");return false;}
            Receipts.Add(receipt);pendingGrants.Remove(receipt);Emit("Grant "+receipt);return true;
        }
        public Mob Find(int id,int generation) => Mobs.FirstOrDefault(x=>x.Id==id && x.Generation==generation && x.Map==Player.Map && x.Alive);
        public Anchor NearAnchor() => Anchors.Where(x=>!x.EdgeExit&&x.Map==Player.Map && Player.Position.Distance(x.Position)<=2)
            .OrderBy(x=>Player.Position.Distance(x.Position)).ThenBy(x=>x.Id).FirstOrDefault();
        public Loot LootCandidate() => Loot.Where(x=>x.Eligible(Player,Now)).OrderBy(x=>Player.Position.Distance(x.Position)).ThenBy(x=>x.Id).FirstOrDefault();
        public bool Near(string id) => Anchors.Any(x=>x.Id==id && x.Map==Player.Map && Player.Position.Distance(x.Position)<=2);
        public string QuestNpc => Quest==1||Quest==2?"Lam":Quest==3?"Phong":Quest==4?"Bach":Quest==5?"Yen":"Ta";
        private string RouteTo(Map destination){
            string name=destination==Map.Village?"Vân Khê":destination==Map.Academy?"Học Viện":"Đồng Sương";
            if(Player.Map==destination)return name+" · ở map hiện tại";
            if(Player.Map==Map.Village)return destination==Map.Academy?"← Cổng tây Vân Khê → Học Viện":"Cổng đông Vân Khê → Đồng Sương";
            return (Player.Map==Map.Academy?"Cổng đông Học Viện → Vân Khê":"← Cổng tây Đồng Sương → Vân Khê")+(destination==Map.Village?"":destination==Map.Academy?" → Học Viện":" → Đồng Sương");
        }
        public string Objective {
            get {
                if(Complete)return "Q1–Q6 hoàn tất. Trúc Ảnh thuộc gate sau. Phiên local chỉ giữ RAM.";
                var owner=Anchors.First(x=>x.Id==QuestNpc);
                if(QuestState==QuestState.Available)return $"Q{Quest}: Nhận nhiệm vụ tại {owner.Name}\n"+RouteTo(owner.Map);
                if(QuestState==QuestState.Ready)return $"Q{Quest}: Báo lại {owner.Name}\n"+RouteTo(owner.Map);
                string[][] steps={
                    new[]{"Nói chuyện Yên Thảo","Nói chuyện Bách Luyện","Nói chuyện Mộc An","Báo Lâm Bá"},
                    new[]{"Tới HV_Entrance ở mép đông","Nhảy lên HV_JumpLedge (x8)","Đi xuống xuyên sàn tới HV_DropLanding","Đi qua mép phải về Vân Khê","Báo Lâm Bá"},
                    new[]{"Mặc Mộc Kiếm trong hành trang","Tới HV_DummyYard (x22)",$"Hạ Bù Nhìn {Kills}/3","Báo Phong Du"},
                    new[]{"Hạ Nấm tại DS2_MushroomPatch (x15)","Nhặt Áo + Nấm Sương tutorial","Mặc Áo Thanh Mộc","Bán Nấm Sương tutorial tại Bách Luyện","Báo Bách Luyện"},
                    new[]{"Chuẩn bị Food I + Bình Máu I tại Yên Thảo","Dùng Food","Tới DS4_ExitTrail (x50)",$"Hạ Sói DS3–DS6 {Kills}/5","Báo Yên Thảo"},
                    new[]{"Tới HV_ClassHall (x3)","Tháo Mộc Kiếm (C → Trang bị), nói chuyện Phong Du để chọn Kiếm","Mặc Kiếm + cộng ≥1 điểm + dùng bí kíp","Cast S1 tại HV_DummyYard","Dùng Bình Linh lực "+ProbeBindings.MpGlyph(Probes)+" khi thiếu MP (đã cấp dự trữ)","Báo Tạ Minh"}
                };
                Map destination=Quest==1?Map.Village:Quest==2?(Stage<3?Map.Academy:Map.Village):Quest==3?Map.Academy:Quest==4?(Stage<3?Map.Mist:Map.Village):Quest==5?(Stage<2||Stage>=4?Map.Village:Map.Mist):(Stage<5?Map.Academy:Map.Village);
                string next=Quest==1?" · gặp: "+Anchors.First(x=>x.Id==new[]{"Yen","Bach","Moc","Lam"}[Math.Min(Stage,3)]).Name:Quest==6&&Stage==1?" · gặp: Phong Du":"";
                return $"Q{Quest}: "+steps[Quest-1][Math.Min(Stage,steps[Quest-1].Length-1)]+"\n"+RouteTo(destination)+next;
            }
        }
        public bool AcceptQuest() {
            if(Complete||QuestState!=QuestState.Available||!Player.Alive||!Near(QuestNpc))return false;
            if(Quest==3 && !Grant("Q3.wood",NewItem("wood",binding:"Q3")))return false;
            if(Quest==5 && Receipts.Add("Q5.supply-gold"))Player.Gold+=320;
            QuestState=QuestState.InProgress;Stage=Kills=0;CheckSupply();Emit($"Đã nhận Q{Quest}");return true;
        }
        public bool TurnIn() {
            if(Complete||QuestState!=QuestState.Ready||!Player.Alive||!Near(QuestNpc))return false;
            if(Quest==3 && !Grant("Q3.reward",NewItem("pants1")))return false;
            if(!Receipts.Add($"Q{Quest}.completed"))return false;
            int target=Quest==1?100:Quest==2?250:Quest==4?470:Quest==5?790:0;
            Player.AddExp(Math.Max(0,target-Player.TotalExp));
            if(Quest==1)Player.Gold+=50;if(Quest==2)Player.Gold+=75;
            Emit($"Completed Q{Quest}; Lv{Player.Level}");Quest++;Stage=Kills=0;
            QuestState=Complete?QuestState.Completed:QuestState.Available;return true;
        }
        public bool Interact(string id) {
            if(!Player.Alive || !Near(id))return false;
            var a=Anchors.First(x=>x.Id==id);
            if(a.EdgeExit)return false;
            if(Quest==1 && QuestState==QuestState.InProgress && new[]{"Yen","Bach","Moc"}[Math.Min(Stage,2)]==id){Stage++;if(Stage==3)QuestState=QuestState.Ready;}
            Emit("Nói chuyện "+a.Name);return true;
        }
        public bool ChooseSword() {
            if(!Player.Alive||!Near("Phong")||Quest!=6||Stage!=1||QuestState!=QuestState.InProgress||Player.School!=School.Novice)return false;
            if(Player.Inventory.Equipment.ContainsKey(GearSlot.Weapon)){Emit("Hãy tháo Mộc Kiếm ở Nhân vật → Trang bị trước khi nhập phái.");return false;}
            if(Combat.Running!=null||Combat.HasPendingCast){Emit("Chờ action kết thúc trước khi nhập phái.");return false;}
            if(!Grant("Q6.class",NewItem("sword1"),NewItem("manual1",binding:"Q6")))return false;
            Combat.Cancel();Player.School=School.Sword;Player.Clamp();Stage=2;Emit("Đã chọn Kiếm; nội tại MaxHP ×1,10 / DEF ×1,08.");return true;
        }
        public bool Equip(long instance) {
            if(!Player.Alive||Combat.Running!=null)return false;
            var item=Player.Inventory.Bag.FirstOrDefault(x=>x.Instance==instance);if(item==null)return false;
            var d=Catalog.Get(item.Id);
            if(d.Kind!=ItemKind.Gear||Player.Level<d.MinLevel||d.Id=="sword1"&&Player.School!=School.Sword||d.Id=="wood"&&Player.School!=School.Novice)return false;
            Player.Inventory.Bag.Remove(item);
            if(Player.Inventory.Equipment.TryGetValue(d.Slot,out var old))Player.Inventory.Bag.Add(old);
            Player.Inventory.Equipment[d.Slot]=item;Player.Clamp();
            if(Quest==3 && Stage==0 && item.Id=="wood")Stage=1;
            if(Quest==4 && Stage==2 && item.Instance==tutorialArmor){item.Binding=null;Stage=3;}
            CheckClassGroup();Emit("Mặc "+d.Name);return true;
        }
        public bool Unequip(GearSlot slot) {
            if(!Player.Alive||Combat.Running!=null||!Player.Inventory.Equipment.TryGetValue(slot,out var item)||!Player.Inventory.Fits(new[]{item}))return false;
            Player.Inventory.Equipment.Remove(slot);Player.Inventory.Add(new[]{item});Player.Clamp();return true;
        }
        public bool Allocate(string stat) {
            if(!Player.Alive||Player.Unspent==0)return false;
            if(stat=="STR")Player.Str++;else if(stat=="VIT")Player.Vit++;else if(stat=="INT")Player.Int++;else if(stat=="AGI")Player.Agi++;else return false;
            Player.Unspent--;Player.Allocated++;Player.Clamp();CheckClassGroup();Emit("+1 "+stat);return true;
        }
        public bool Learn(long instance) {
            if(!Player.Alive||Combat.Running!=null||Player.School!=School.Sword||Player.Level<5||Combat.Unlocked.ContainsKey(1))return false;
            var book=Player.Inventory.Bag.FirstOrDefault(x=>x.Instance==instance&&x.Id=="manual1");if(book==null)return false;
            if(!Receipts.Add("learn.sword.s1"))return false;Player.Inventory.Consume(book);Combat.Unlocked[1]=Rules.Sword1;Combat.SelectedSlot=1;
            CheckClassGroup();Emit("Đã học Phong Trảm; bấm 1 để tiếp cận và cast một lần.");return true;
        }
        private void CheckClassGroup() {
            if(Quest==6&&Stage==2&&Player.Allocated>0&&Combat.Unlocked.ContainsKey(1)&&
                Player.Inventory.Equipment.TryGetValue(GearSlot.Weapon,out var weapon)&&weapon.Id=="sword1")Stage=3;
        }
        public bool Buy(string id,string npc) {
            if(!Player.Alive||!Near(npc)||!Catalog.Items.TryGetValue(id,out var d)||d.Buy==0||Player.Gold<d.Buy)return false;
            bool vendor=(npc=="Yen"&&(d.Kind==ItemKind.Food||d.Kind==ItemKind.HpPotion||d.Kind==ItemKind.MpPotion||d.Kind==ItemKind.Scroll)) ||
                (npc=="Bach"&&(d.Kind==ItemKind.Gear||d.Kind==ItemKind.Stone));
            if(!vendor||id=="sword1"&&Player.School!=School.Sword)return false;
            if(!Player.Inventory.Add(new[]{NewItem(id)})){Emit("Túi đầy — giao dịch chưa thực hiện.");return false;}
            Player.Gold-=d.Buy;
            Purchased.Add(id);CheckSupply();
            Emit("Mua "+d.Name);return true;
        }
        public bool Sell(long instance) {
            if(!Player.Alive||!Near("Bach"))return false;
            var item=Player.Inventory.Bag.FirstOrDefault(x=>x.Instance==instance);if(item==null||Catalog.Get(item.Id).Sell<=0)return false;
            bool tutorial=item.Instance==tutorialSample && Quest==4 && Stage==3 && QuestState==QuestState.InProgress;
            if(item.Binding!=null&&!tutorial)return false;
            double sellMultiplier=item.Quality>=1.25?3:item.Quality>=1.16?2:item.Quality>=1.08?1.5:1;
            Player.Gold+=(int)Math.Floor(Catalog.Get(item.Id).Sell*sellMultiplier);Player.Inventory.Consume(item);
            if(tutorial){Stage=4;QuestState=QuestState.Ready;}Emit("Bán "+Catalog.Get(item.Id).Name);return true;
        }
        public bool Store(long instance,bool deposit) {
            if(!Player.Alive||Combat.Running!=null||!Near("Moc"))return false;
            var from=deposit?Player.Inventory:Player.Storage;var to=deposit?Player.Storage:Player.Inventory;
            var item=from.Bag.FirstOrDefault(x=>x.Instance==instance);if(item==null||!to.Fits(new[]{item}))return false;
            from.Bag.Remove(item);to.Add(new[]{item});Emit((deposit?"Cất ":"Lấy ")+Catalog.Get(item.Id).Name);return true;
        }
        public bool UseFood() {
            if(!Player.Alive)return false;var item=Player.Inventory.Bag.FirstOrDefault(x=>x.Id=="food1");if(item==null)return false;
            Player.Inventory.Consume(item);FoodUntil=Now+600;NextFood=Now+2;
            if(Quest==5&&Stage==1&&QuestState==QuestState.InProgress)Stage=2;Emit("Food I: 2% HP / 1,5% MP mỗi 2 giây, 10 phút.");return true;
        }
        public bool Potion(bool hp) {
            if(!Player.Alive||Now<(hp?HpPotionUntil:MpPotionUntil))return false;
            var s=Player.Stats;if((hp?Player.Hp:Player.Mp)>=(hp?s.Hp:s.Mp)-.00001)return false;
            var item=Player.Inventory.Bag.Where(x=>x.Id==(hp?"hp1":"mp1")).OrderBy(x=>x.Binding=="Q6"?0:1).FirstOrDefault();if(item==null)return false;
            bool reserved=item.Binding=="Q6";Player.Inventory.Consume(item);
            if(hp){Player.Hp=Math.Min(s.Hp,Player.Hp+s.Hp*.3);HpPotionUntil=Now+8;}
            else {Player.Mp=Math.Min(s.Mp,Player.Mp+s.Mp*.3);MpPotionUntil=Now+8;}
            if(!hp&&reserved&&Quest==6&&Stage==4){Stage=5;QuestState=QuestState.Ready;}
            Emit(hp?"Dùng Bình Máu I":"Dùng Bình Linh Lực I");return true;
        }
        public bool PickUp(long lootId) {
            var value=Loot.FirstOrDefault(x=>x.Id==lootId);if(value==null||!value.Eligible(Player,Now))return false;
            if(!Player.Inventory.Add(new[]{value.Item})){Emit("Túi đầy — đồ vẫn trên đất.");return false;}
            value.Claimed=true;Receipts.Add("claim."+value.Id);
            if(Quest==4&&Stage==1&&Player.Inventory.Bag.Any(x=>x.Instance==tutorialArmor)&&Player.Inventory.Bag.Any(x=>x.Instance==tutorialSample))Stage=2;
            Emit("Nhặt "+Catalog.Get(value.Item.Id).Name);return true;
        }
        private void TutorialSupply(Point position) {
            if(Quest!=4||QuestState!=QuestState.InProgress||Stage!=0||!Receipts.Add("Q4.supply"))return;
            var armor=NewItem("armor1",binding:"Q4");var sample=NewItem("mushroom",binding:"Q4");tutorialArmor=armor.Instance;tutorialSample=sample.Instance;
            foreach(var item in new[]{armor,sample})Loot.Add(new Loot{Id=item.Instance,Item=item,Map=Map.Mist,Position=position,Created=Now,Tutorial=true});
        }
        public void ObservePosition(Point position,bool grounded,bool jumped=false,bool dropped=false,bool manualExit=true) {
            Player.Position=position;
            if(!Player.Alive)return;
            if(!Probes.ApproachExitSafety||manualExit)ObserveExits();
            if(QuestState!=QuestState.InProgress)return;
            if(Quest==2 && Player.Map==Map.Academy){
                if(Stage==0 && position.Distance(new Point(31,.8))<=1.5){Stage=1;Emit("HV_Entrance");}
                if(Stage==1 && grounded && position.Distance(new Point(8,3.8))<=1.5){Stage=2;Emit("HV_JumpLedge — đạt cao độ thật");}
                if(Stage==2 && dropped)Receipts.Add("Q2.dropped");
                if(Stage==2 && Receipts.Contains("Q2.dropped") && grounded && position.Distance(new Point(8,.8))<=1.5){Stage=3;Emit("HV_DropLanding — tiếp đất thật");}
            }
            if(Quest==3&&Stage==1&&Player.Map==Map.Academy&&position.Distance(new Point(22,.8))<=1.5)Stage=2;
            if(Quest==5&&Stage==2&&Player.Map==Map.Mist&&position.Distance(new Point(50,BlockoutLayout.BaseGroundTop(Map.Mist,50)+.8))<=1.5)Stage=3;
            if(Quest==6&&Stage==0&&Player.Map==Map.Academy&&position.Distance(new Point(3,.8))<=1.5)Stage=1;
        }
        private void CheckSupply() {
            if(Quest==5&&QuestState==QuestState.InProgress&&Stage==0&&Player.Inventory.Count("food1")>0&&Player.Inventory.Count("hp1")>0)Stage=1;
        }
        private void ObserveExits() {
            var exit=Anchors.FirstOrDefault(x=>x.EdgeExit&&x.Map==Player.Map&&Math.Abs(Player.Position.X-x.Position.X)<=.65&&Math.Abs(Player.Position.Y-x.Position.Y)<=1.6);
            if(exit==null){exitLatch=null;return;}
            if(exitLatch==exit.Id)return;
            exitLatch=exit.Id;TryExit(exit.Id);
        }
        public bool TryExit(string id) {
            var exit=Anchors.FirstOrDefault(x=>x.Id==id&&x.EdgeExit&&x.Map==Player.Map);
            if(!Player.Alive||exit==null||Math.Abs(Player.Position.X-exit.Position.X)>.65||Math.Abs(Player.Position.Y-exit.Position.Y)>1.6)return false;
            if(id=="outOfSlice"){Emit(Complete?"Trúc Ảnh đã mở trong tiến trình, chưa có trong VS-1.":"Cần hoàn thành Q6.");return false;}
            Map destination=id=="toAcademy"?Map.Academy:id=="toMist"?Map.Mist:Map.Village;
            if(Quest==2&&Stage==3&&Player.Map==Map.Academy&&destination==Map.Village){Stage=4;QuestState=QuestState.Ready;}
            double spawnX=destination==Map.Village?(id=="toVillageA"?-1:27):(destination==Map.Academy?31:-1);
            ChangeMap(destination,new Point(spawnX,BlockoutLayout.BaseGroundTop(destination,spawnX)+.72));return true;
        }
        private void ChangeMap(Map map,Point spawn){
            foreach(var m in Mobs.Where(x=>x.Map==Player.Map&&x.Alive&&!x.Dummy)){m.Windup=false;m.ReturnSince=Now;}
            Combat.Cancel();Combat.ClearFocus();Player.Map=map;Player.Position=spawn;WorldRevision++;Emit("Chuyển vùng → "+map);
        }
        public bool Rest() {if(!Player.Alive||!Near("Moc"))return false;Player.Hp=Player.Stats.Hp;Player.Mp=Player.Stats.Mp;Emit("Mộc An hồi đầy HP/MP.");return true;}
        public bool Revive(bool village) {
            if(Player.Alive)return false;
            if(!village){var scroll=Player.Inventory.Bag.FirstOrDefault(x=>x.Id=="scroll");if(scroll==null)return false;Player.Inventory.Consume(scroll);}
            Combat.Cancel();Combat.ClearFocus();
            if(village)ChangeMap(Map.Village,new Point(15,BlockoutLayout.BaseGroundTop(Map.Village,15)+.72));
            Player.Hp=Player.Stats.Hp*(village?1:.5);Player.Mp=Player.Stats.Mp*(village?1:.5);Player.InvulnerableUntil=Now+(village?0:2);Emit("Hồi sinh "+(village?"Vân Khê":"tại chỗ"));return true;
        }
        public void HurtPlayer(double damage) {
            if(!Player.Alive||Now<Player.InvulnerableUntil)return;LastHurtAt=Now;Player.Hp=Math.Max(0,Player.Hp-damage);Emit($"Nhận {damage:F0} damage; HP {Player.Hp:F0}");
            if(!Player.Alive){Combat.Cancel();Combat.ClearFocus();Emit("Đã chết — chọn Về Làng hoặc dùng phù.");}
        }
        internal void Landed(Mob mob,int damage) {
            if(!mob.Alive||!Player.Alive||mob.Map!=Player.Map)return;
            double lost=Math.Min(mob.Hp,damage);mob.Hp-=lost;mob.LastDamage=Now;
            int tag=Quest*100+Stage;
            if(mob.QuestTag!=tag){mob.QuestTag=tag;mob.QuestDamage=0;}mob.QuestDamage+=lost;
            if(!mob.Alive)Die(mob);
        }
        private void Die(Mob m) {
            if(!Receipts.Add($"death.{m.Id}.{m.Generation}"))return;
            m.Windup=false;m.RespawnAt=Now+25;
            bool qualified=Player.Alive&&Player.Map==m.Map&&Player.Position.Distance(m.Position)<=8 && Now-m.LastDamage<=10 && m.QuestDamage>=m.MaxHp*.2;
            int tag=Quest*100+Stage;
            if(qualified&&m.QuestTag==tag&&QuestState==QuestState.InProgress){
                if(Quest==3&&Stage==2&&m.Dummy){Kills++;if(Kills==3){Stage=3;QuestState=QuestState.Ready;}}
                if(Quest==5&&Stage==3&&new[]{"DS3","DS4","DS5","DS6"}.Any(x=>m.Slot.StartsWith(x+"."))&&m.Level==4){Kills++;if(Kills==5){Stage=4;QuestState=QuestState.Ready;}}
                if(Quest==4&&Stage==0&&m.Slot=="DS2.slot1"){TutorialSupply(m.Position);Stage=1;}
            }
            int atDeath=Player.Level;
            if(!m.Dummy&&Math.Abs(atDeath-m.Level)<=3){
                Player.AddExp(m.Level==2?15:22);Player.Gold+=Random.Next(m.Level==2?7:11,m.Level==2?13:19);
                if(Random.NextDouble()<.3)Drop(m,NewItem(m.Level==2?"mushroom":"fang"),atDeath);
                if(Random.NextDouble()<.04)Drop(m,NewItem(Random.Next(2)==0?"hp1":"mp1"),atDeath);
                if(Random.NextDouble()<.08)Drop(m,NewItem("stone"),atDeath);
                double gear=Random.NextDouble();if(gear<.051){double quality=gear<.001?1.16:gear<.011?1.08:1;
                    var ids=new[]{"armor1","pants1","boots1","ring1","neck1"};Drop(m,NewItem(ids[Random.Next(5)],quality:quality),atDeath);}
            }
            Emit($"Death {m.Slot}@{m.Generation}; quest {Quest}/{Stage}; respawn +25s");
        }
        private void Drop(Mob m,Item item,int level) => Loot.Add(new Loot{Id=item.Instance,Item=item,Map=m.Map,Position=m.Position,Created=Now,MobLevel=m.Level,LevelAtDeath=level});
        public void ActionStarted(Skill skill) {
            if(Quest==6&&Stage==3&&skill.Id==Rules.Sword1.Id&&Player.Map==Map.Academy&&Player.Position.Distance(new Point(22,.8))<=3){
                // Pending grant can be retried by another valid cast after freeing a slot.
                if(Grant("Q6.potion",NewItem("mp1",binding:"Q6")))Stage=4;
            }
        }
        public void Tick(double dt,bool manualContext=false) {
            if(dt<=0||dt>1)throw new ArgumentOutOfRangeException(nameof(dt));Now+=dt;
            if(Player.Alive&&NextFood>0&&Now<FoodUntil&&Now>=NextFood){
                while(Now>=NextFood&&NextFood<FoodUntil){Player.Hp=Math.Min(Player.Stats.Hp,Player.Hp+.02*Player.Stats.Hp);Player.Mp=Math.Min(Player.Stats.Mp,Player.Mp+.015*Player.Stats.Mp);NextFood+=2;}
            }
            Combat.Tick(manualContext);
            foreach(var m in Mobs){
                if(!m.Alive){if(Now>=m.RespawnAt){m.Generation++;m.Hp=m.MaxHp;m.Position=m.PreviousPosition=m.Home;m.QuestDamage=0;m.Engaged=false;m.ApproachSide=0;m.Returning=false;m.ReturnSince=-1;m.RepositionUntil=0;m.SideLockedUntil=0;m.SideSwitchPending=false;m.NextAttack=Now+(Probes.StableMelee?StablePhase(m):(m.Id%5)*.07);Emit("Respawn "+m.Slot+"@"+m.Generation);}continue;}
                if(m.Dummy)continue;
                if(m.Map!=Player.Map){
                    m.Windup=false;
                    if(m.Hp<m.MaxHp||m.Position.Distance(m.Home)>.1){
                        if(m.ReturnSince<0)m.ReturnSince=Now;
                        if(Now-m.ReturnSince>2)m.Returning=true;
                        if(m.Returning)TickMob(m,dt); // only Return branch; never use another map's player as a target
                    }
                    continue;
                }
                m.PreviousPosition=m.Position;TickMob(m,dt);
            }
            // Tutorial entitlement keeps the same instance; regular world loot expires.
            foreach(var l in Loot.Where(x=>!x.Claimed&&Now-x.Created>=60)){
                if(l.Tutorial){l.Position=Mobs.First(x=>x.Slot=="DS2.slot1").Home;l.Created=Now;Emit("Tutorial supply re-offer same instance "+l.Id);}
                else l.Claimed=true;
            }
        }
        private static void MoveMob(Mob m,double goal,double dt,Mob[] peers,double speedFactor=1,bool quadratic=false) {
            double speed=m.Speed*speedFactor;
            m.DesiredX=Math.Max(Math.Max(m.ActivityMin,m.LaneMin),Math.Min(Math.Min(m.ActivityMax,m.LaneMax),goal));
            double dx=m.DesiredX-m.Position.X,velocity=Math.Sign(dx)*Math.Min(Math.Abs(dx)/dt,speed);
            double separation=0;
            foreach(var p in peers){double gap=m.Position.X-p.Position.X;if(Math.Abs(gap)<.85){double weight=(.85-Math.Abs(gap))/.85;int side=Math.Abs(gap)<.001?(quadratic?StringComparer.Ordinal.Compare(m.Slot,p.Slot):m.Id-p.Id):Math.Sign(gap);separation+=Math.Sign(side)*(quadratic?weight*weight:weight)*speed*.8;}}
            // One velocity integration, bounded correction; no post-chase displacement or hard blocker.
            velocity=Math.Max(-speed,Math.Min(speed,velocity+Math.Max(-speed*.8,Math.Min(speed*.8,separation))));
            double step=velocity*dt;
            step=Math.Max(Math.Max(m.ActivityMin,m.LaneMin)-m.Position.X,Math.Min(Math.Min(m.ActivityMax,m.LaneMax)-m.Position.X,step));
            // Player/mob and mob/mob have no physical blocking. Brief crossings are allowed;
            // occupancy changes destination rather than permanently clipping rear velocity to zero.
            m.VelocityX=step/dt;m.Position=new Point(m.Position.X+step,m.Home.Y);
        }
        private void Patrol(Mob m,double dt){
            m.Engaged=false;m.Windup=false;m.Occupancy="clear";m.ReturnSince=-1;
            if(Now<m.PatrolPauseUntil){m.Motion="idle";m.VelocityX=0;return;}
            if(Math.Abs(m.Position.X-m.PatrolGoal)<.08){
                m.PatrolDirection=-m.PatrolDirection;
                double radius=1.5+(m.Id%3)*.4;
                m.PatrolGoal=Math.Max(m.ActivityMin+.3,Math.Min(m.ActivityMax-.3,m.Home.X+m.PatrolDirection*radius));
                m.PatrolPauseUntil=Now+.6+(m.Id%5)*.17;m.Motion="idle";m.VelocityX=0;return;
            }
            m.Motion="patrol";
            var peers=Mobs.Where(x=>x!=m&&x.Alive&&!x.Dummy&&x.Map==m.Map&&Math.Abs(x.Home.Y-m.Home.Y)<.3&&Math.Abs(x.Home.X-m.Home.X)<8).ToArray();
            MoveMob(m,m.PatrolGoal,dt,peers,.35,Probes.StableMelee);if(Math.Abs(m.VelocityX)>.001)m.Facing=m.VelocityX<0?-1:1;
        }
        private void TickMob(Mob m,double dt) {
            double dist=m.Position.Distance(Player.Position);
            bool reachable=Math.Abs(m.Position.Y-Player.Position.Y)<1.6;
            bool inActivity=Player.Position.X>=m.ActivityMin&&Player.Position.X<=m.ActivityMax;
            if(m.Returning){
                m.Motion="return";double dx=m.Home.X-m.Position.X;if(Math.Abs(dx)>.01)m.Facing=dx<0?-1:1;m.VelocityX=Math.Sign(dx)*Math.Min(Math.Abs(dx)/dt,m.Speed);m.Position=new Point(m.Position.X+Math.Sign(dx)*Math.Min(Math.Abs(dx),m.Speed*dt),m.Home.Y);
                if(Math.Abs(dx)<.05){m.Returning=false;m.Engaged=false;m.Hp=m.MaxHp;m.QuestDamage=0;m.ReturnSince=-1;m.Windup=false;m.PatrolGoal=m.Home.X;m.PatrolPauseUntil=Now+.4;m.Motion="idle";m.VelocityX=0;}return;
            }
            // VS-1 probe: 2s no reachable progress -> Return + reset. This is NOT a production lock.
            // Resolve on the original hit clock, even when the player has left reach.
            // Reachability grace must never postpone an old hit until the player returns.
            if(m.Windup&&Now>=m.HitAt){
                m.Windup=false;
                if(Probes.StableMelee){
                    if(m.RepositionAfterHit){m.RepositionGoal=Bound(m,m.Position.X+m.ApproachSide*.8);m.RepositionUntil=m.NextAttack;}
                    else m.RepositionUntil=0;
                }else {
                    m.ApproachSide=-m.ApproachSide;
                    double retreat=1.8+(m.Id%4)*.8;
                    double low=Math.Max(m.ActivityMin,m.LaneMin),high=Math.Min(m.ActivityMax,m.LaneMax);
                    if(Player.Position.X+m.ApproachSide*retreat<low||Player.Position.X+m.ApproachSide*retreat>high)m.ApproachSide=-m.ApproachSide;
                    m.RepositionUntil=m.NextAttack;
                }
                if(Player.Alive&&Math.Abs(Player.Position.X-m.Position.X)<=m.Range && Math.Abs(Player.Position.Y-m.Position.Y)<1.3 && Math.Sign(Player.Position.X-m.Position.X)==m.Facing){
                    var s=Player.Stats;if(Random.NextDouble()>=Rules.Evade(m.Acc,s.Eva))HurtPlayer(Rules.Damage(m.Atk,1,1,s.Def,false,.95+Random.NextDouble()*.1));else Emit("NÉ đòn "+m.Name);
                }
            }
            if(!Player.Alive||dist>8||!reachable||!inActivity){
                if(m.Engaged||m.Hp<m.MaxHp){
                    if(m.ReturnSince<0)m.ReturnSince=Now;
                    m.Motion="disengage";m.VelocityX=0;
                    if(Now-m.ReturnSince>2){m.Returning=true;m.Windup=false;}
                }else Patrol(m,dt);
                return;
            }
            m.ReturnSince=-1;
            if(m.Windup){m.Motion="windup";m.VelocityX=0;return;}
            if(dist>5&&m.Hp==m.MaxHp&&!m.Engaged){Patrol(m,dt);return;}
            double delta=Player.Position.X-m.Position.X;
            if(Probes.StableMelee){TickStableMelee(m,dt);return;}
            if(!m.Engaged){m.Engaged=true;m.ApproachSide=delta>0?-1:1;m.NextAttack=Math.Max(m.NextAttack,Now+((m.Id+m.Generation-1)%6)*.07);}
            if(Now>=m.RepositionUntil&&Math.Abs(delta)>m.Range+1.2&&Math.Sign(delta)==m.ApproachSide)m.ApproachSide=-m.ApproachSide;
            var peers=Mobs.Where(x=>x!=m&&x.Alive&&!x.Dummy&&!x.Returning&&x.Map==m.Map&&Math.Abs(x.Home.Y-m.Home.Y)<.3&&Math.Abs(x.Home.X-m.Home.X)<8).ToArray();
            double goal=Player.Position.X+m.ApproachSide*m.Range*.82;
            bool reposition=Now<m.RepositionUntil;
            m.Occupancy="clear";
            if(reposition){goal=Player.Position.X+m.ApproachSide*(1.8+(m.Id%4)*.8);m.Motion="reposition";}
            else {
                // A peer occupying the intended bite point makes us cross to free space. Never
                // reserve an offset/rank outside range, nor use another wolf as a permanent wall.
                if(peers.Any(x=>x.Windup&&Math.Abs(x.Position.X-goal)<.85)){
                    m.ApproachSide=-m.ApproachSide;goal=Player.Position.X+m.ApproachSide*1.65;
                    m.RepositionUntil=Now+.38;m.Occupancy="bite point occupied: cross";reposition=true;
                }
                m.Motion=reposition?"cross":"approach";
            }
            MoveMob(m,goal,dt,peers);
            delta=Player.Position.X-m.Position.X;
            if(!reposition&&inActivity&&Math.Abs(delta)<=m.Range&&Now>=m.NextAttack){
                m.Facing=delta<0?-1:1;m.Windup=true;m.HitAt=Now+.35;m.NextAttack=Now+m.Interval;m.BiteAttempts++;m.Motion="windup";m.VelocityX=0;
            } else if(!m.Windup&&Math.Abs(m.VelocityX)>.001)m.Facing=m.VelocityX<0?-1:1;

        }
        private static double Bound(Mob m,double x)=>Math.Max(Math.Max(m.ActivityMin,m.LaneMin),Math.Min(Math.Min(m.ActivityMax,m.LaneMax),x));
        public static uint StableLifeHash(string slot,int life) {
            unchecked {uint h=2166136261;foreach(char c in slot)h=(h^c)*16777619;return (h^(uint)life)*16777619;}
        }
        public static double StablePhase(Mob m)=>StableLifeHash(m.Slot,m.Generation)%6*.07;
        private Mob[] Neighbors(Mob m)=>Mobs.Where(x=>x!=m&&x.Alive&&!x.Dummy&&!x.Returning&&x.Map==m.Map&&Math.Abs(x.Home.Y-m.Home.Y)<.3&&Math.Abs(x.Home.X-m.Home.X)<8).ToArray();
        public int ProbeFreeSide(Mob m) {
            var peers=Neighbors(m);double left=Player.Position.X-m.Range*.9,right=Player.Position.X+m.Range*.9;
            double l=Math.Abs(Bound(m,left)-left)>.001?double.NegativeInfinity:peers.Length==0?double.PositiveInfinity:peers.Min(x=>Math.Abs(x.Position.X-left));
            double r=Math.Abs(Bound(m,right)-right)>.001?double.NegativeInfinity:peers.Length==0?double.PositiveInfinity:peers.Min(x=>Math.Abs(x.Position.X-right));
            if(l==r)return (StableLifeHash(m.Slot,1)&1)==0?-1:1;
            return l>r?-1:1;
        }
        private void ProbeFacing(Mob m,double dx) {
            int facing=dx<0?-1:1;
            if(facing!=m.FacingCandidate){m.FacingCandidate=facing;m.FacingCandidateSince=Now;}
            if(Math.Abs(dx)>.15||Now-m.FacingCandidateSince>=.2)m.Facing=facing;
        }
        private void TickStableMelee(Mob m,double dt) {
            if(!m.Engaged){
                m.Engaged=true;m.ApproachSide=ProbeFreeSide(m);m.LastTargetX=Player.Position.X;m.SideLockedUntil=Now+1.5;
                m.NextAttack=Math.Max(m.NextAttack,Now+StablePhase(m));
            }
            double playerX=Player.Position.X,dx=playerX-m.Position.X;
            if((m.LastTargetX-m.Position.X)*dx<0&&Math.Abs(playerX-m.LastTargetX)>.001)m.SideSwitchPending=true;
            m.LastTargetX=playerX;
            double attackGoal=playerX+m.ApproachSide*m.Range*.9;
            bool blocked=Math.Abs(Bound(m,attackGoal)-attackGoal)>.001;
            if((m.SideSwitchPending||blocked)&&Now>=m.SideLockedUntil){
                int side=blocked?ProbeFreeSide(m):Math.Sign(m.Position.X-playerX);
                if(side!=0&&side!=m.ApproachSide){m.ApproachSide=side;m.SideLockedUntil=Now+1.5;}
                m.SideSwitchPending=false;attackGoal=playerX+m.ApproachSide*m.Range*.9;
            }
            // Return travel consumes existing recovery, never extends the attack deadline.
            double returnTravel=Math.Abs(m.RepositionGoal-attackGoal)/m.Speed;
            bool reposition=m.RepositionAfterHit&&Now<m.RepositionUntil&&Now<m.NextAttack-returnTravel;
            double goal=reposition?m.RepositionGoal:attackGoal;
            m.Motion=reposition?"reposition":"approach";m.Occupancy="probe free-space; stable side";
            MoveMob(m,goal,dt,Neighbors(m),1,true);
            dx=playerX-m.Position.X;
            if(Math.Abs(dx)<=m.Range&&Now>=m.NextAttack){
                // Windup starts from the actual position/facing; subsequent ticks freeze it.
                m.Facing=dx<0?-1:1;m.Windup=true;m.HitAt=Now+.35;m.NextAttack=Now+m.Interval;m.BiteAttempts++;m.Motion="windup";m.VelocityX=0;
            }else ProbeFacing(m,dx);
        }

    }
}
