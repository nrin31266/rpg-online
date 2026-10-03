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
        public readonly bool Portal;
        public Anchor(string id,string name,Map map,double x,double y=.8,bool portal=false)
        {Id=id;Name=name;Map=map;Position=new Point(x,y);Portal=portal;}
    }
    public sealed class SliceSession
    {
        public readonly Player Player=new Player();
        public readonly List<Mob> Mobs=new List<Mob>();
        public readonly List<Loot> Loot=new List<Loot>();
        public readonly List<string> Events=new List<string>();
        public readonly HashSet<string> Receipts=new HashSet<string>();
        public readonly CombatController Combat;
        public readonly Random Random;
        public double Now {get;private set;}
        public double FoodUntil,NextFood,HpPotionUntil,MpPotionUntil;
        public int WorldRevision {get;private set;}
        public int Quest=1,Stage,Kills;
        public QuestState QuestState=QuestState.Available;
        public bool Complete => Quest>6;
        public string Feedback="Vân Khê — hãy nói chuyện với Lâm Bá (E).";
        public Action<string> OnEvent;
        public readonly HashSet<string> Purchased=new HashSet<string>();
        private long nextItem=1000;
        private readonly Dictionary<string,Item[]> pendingGrants=new Dictionary<string,Item[]>();
        public double LastHurtAt {get;private set;}=-100;
        private long tutorialArmor,tutorialSample;
        public static readonly Anchor[] Anchors={
            new Anchor("Lam","Lâm Bá",Map.Village,0), new Anchor("Yen","Yên Thảo",Map.Village,5),
            new Anchor("Bach","Bách Luyện",Map.Village,10),new Anchor("Moc","Mộc An",Map.Village,15),
            new Anchor("Ta","Tạ Minh",Map.Village,20),
            new Anchor("toAcademy","Học Viện →",Map.Village,25,portal:true),new Anchor("toMist","Đồng Sương →",Map.Village,30,portal:true),
            new Anchor("toVillageA","← Vân Khê",Map.Academy,-4,portal:true),new Anchor("Phong","Phong Du",Map.Academy,0),
            new Anchor("ClassHall","Chọn Kiếm / Cung",Map.Academy,3),
            new Anchor("toVillageM","← Vân Khê",Map.Mist,-4,portal:true),
            new Anchor("outOfSlice","Trúc Ảnh →",Map.Mist,110,portal:true)
        };
        public SliceSession(int seed=731) {
            Random=new Random(seed);Combat=new CombatController(this);
            // Dummy DEF/EVA=0 are explicit VS-1 probe values: GDD only fixes its HP.
            Mobs.Add(new Mob(10,"HV_Dummy","Bù Nhìn",Map.Academy,new Point(22,.8),3,60,0,0,0,0,0,0,0,true));
            Mobs.Add(new Mob(20,"DS1.slot1","Nấm Linh",Map.Mist,new Point(5,.65),2,48,9,4,68,24,1.2,.8,1.8));
            Mobs.Add(new Mob(21,"DS2.slot1","Nấm Linh",Map.Mist,new Point(15,.65),2,48,9,4,68,24,1.2,.8,1.8));
            int id=30;
            for(int pocket=3;pocket<=6;pocket++)for(int slot=1;slot<=2;slot++)
                Mobs.Add(new Mob(id++,$"DS{pocket}.slot{slot}","Sói Sương",Map.Mist,new Point(30+(pocket-3)*20+(slot-1)*2,.65),4,107,13,5,76,28,2.4,1,1.3));
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
        public Anchor NearAnchor() => Anchors.Where(x=>x.Map==Player.Map && Player.Position.Distance(x.Position)<=2)
            .OrderBy(x=>Player.Position.Distance(x.Position)).ThenBy(x=>x.Id).FirstOrDefault();
        public Loot LootCandidate() => Loot.Where(x=>x.Eligible(Player,Now)).OrderBy(x=>Player.Position.Distance(x.Position)).ThenBy(x=>x.Id).FirstOrDefault();
        public bool Near(string id) => Anchors.Any(x=>x.Id==id && x.Map==Player.Map && Player.Position.Distance(x.Position)<=2);
        public string QuestNpc => Quest==1||Quest==2?"Lam":Quest==3?"Phong":Quest==4?"Yen":Quest==5?"Bach":"Ta";
        public string Objective {
            get {
                if(Complete)return "Q1–Q6 hoàn tất. Trúc Ảnh thuộc gate sau. Phiên local chỉ giữ RAM.";
                if(QuestState==QuestState.Available)return $"Q{Quest}: Nhận nhiệm vụ tại {Anchors.First(x=>x.Id==QuestNpc).Name}";
                if(QuestState==QuestState.Ready)return $"Q{Quest}: Báo lại {Anchors.First(x=>x.Id==QuestNpc).Name}";
                string[][] steps={
                    new[]{"Nói chuyện Yên Thảo","Nói chuyện Bách Luyện","Nói chuyện Mộc An","Báo Lâm Bá"},
                    new[]{"Tới HV_Entrance","Nhảy lên HV_JumpLedge (x8)","S + Space xuống HV_DropLanding","Qua portal thật về Vân Khê","Báo Lâm Bá"},
                    new[]{"Mặc Mộc Kiếm trong túi (B)","Tới HV_DummyYard (x22)",$"Hạ Bù Nhìn {Kills}/3","Báo Phong Du"},
                    new[]{"Mua Food I + Bình Máu I + Bình Linh Lực I","Dùng Food bằng F","Tới DS4_ExitTrail (x50)",$"Hạ Sói DS3–DS6 {Kills}/5","Báo Yên Thảo"},
                    new[]{"Hạ Nấm tại DS2_MushroomPatch (x15)","E nhặt Áo + Nấm Sương tutorial","Mặc Áo Thanh Mộc","Bán Nấm Sương tutorial tại Bách Luyện","Báo Bách Luyện"},
                    new[]{"Tới HV_ClassHall (x3)","E tại ClassHall, chọn Kiếm","Mặc Kiếm + cộng ≥1 điểm + dùng bí kíp","Cast S1 tại HV_DummyYard","Bấm M khi thiếu MP (đã cấp bình dự trữ)","Báo Tạ Minh"}
                };
                return $"Q{Quest}: "+steps[Quest-1][Math.Min(Stage,steps[Quest-1].Length-1)];
            }
        }
        public bool AcceptQuest() {
            if(Complete||QuestState!=QuestState.Available||!Player.Alive||!Near(QuestNpc))return false;
            if(Quest==3 && !Grant("Q3.wood",NewItem("wood",binding:"Q3")))return false;
            if(Quest==4 && Receipts.Add("Q4.supply"))Player.Gold+=320;
            QuestState=QuestState.InProgress;Stage=Kills=0;Emit($"Accepted Q{Quest}");return true;
        }
        public bool TurnIn() {
            if(Complete||QuestState!=QuestState.Ready||!Player.Alive||!Near(QuestNpc))return false;
            if(Quest==3 && !Grant("Q3.reward",NewItem("pants1")))return false;
            if(!Receipts.Add($"Q{Quest}.completed"))return false;
            if(Quest<=4)Player.AddExp(Math.Max(0,Rules.Exp[Quest]-Player.TotalExp));
            if(Quest==1)Player.Gold+=50;if(Quest==2)Player.Gold+=75;if(Quest==5)Player.AddExp(45);
            Emit($"Completed Q{Quest}; Lv{Player.Level}");Quest++;Stage=Kills=0;
            QuestState=Complete?QuestState.Completed:QuestState.Available;return true;
        }
        public bool Interact(string id) {
            if(!Player.Alive || !Near(id))return false;
            var a=Anchors.First(x=>x.Id==id);
            if(a.Portal)return Portal(id);
            if(Quest==1 && QuestState==QuestState.InProgress && new[]{"Yen","Bach","Moc"}[Math.Min(Stage,2)]==id){Stage++;if(Stage==3)QuestState=QuestState.Ready;}
            Emit("Nói chuyện "+a.Name);return true;
        }
        public bool ChooseSword() {
            if(!Player.Alive||!Near("ClassHall")||Quest!=6||Stage!=1||QuestState!=QuestState.InProgress||Player.School!=School.Novice)return false;
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
            if(Quest==5 && Stage==2 && item.Instance==tutorialArmor){item.Binding=null;Stage=3;}
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
            CheckClassGroup();Emit("Đã học Phong Trảm; 1 chọn và cast, J dùng slot đang chọn.");return true;
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
            if(Quest==4&&Stage==0){Purchased.Add(id);if(new[]{"food1","hp1","mp1"}.All(x=>Purchased.Contains(x)))Stage=1;}
            Emit("Mua "+d.Name);return true;
        }
        public bool Sell(long instance) {
            if(!Player.Alive||!Near("Bach"))return false;
            var item=Player.Inventory.Bag.FirstOrDefault(x=>x.Instance==instance);if(item==null||Catalog.Get(item.Id).Sell<=0)return false;
            bool tutorial=item.Instance==tutorialSample && Quest==5 && Stage==3;
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
            if(Quest==4&&Stage==1)Stage=2;Emit("Food I: 2% HP / 1,5% MP mỗi 2 giây, 10 phút.");return true;
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
            if(Quest==5&&Stage==1&&Player.Inventory.Bag.Any(x=>x.Instance==tutorialArmor)&&Player.Inventory.Bag.Any(x=>x.Instance==tutorialSample))Stage=2;
            Emit("Nhặt "+Catalog.Get(value.Item.Id).Name);return true;
        }
        private void TutorialSupply(Point position) {
            if(!Receipts.Add("Q5.supply"))return;
            var armor=NewItem("armor1",binding:"Q5");var sample=NewItem("mushroom",binding:"Q5");tutorialArmor=armor.Instance;tutorialSample=sample.Instance;
            foreach(var item in new[]{armor,sample})Loot.Add(new Loot{Id=item.Instance,Item=item,Map=Map.Mist,Position=position,Created=Now,Tutorial=true});
        }
        public void ObservePosition(Point position,bool grounded,bool jumped=false,bool dropped=false) {
            Player.Position=position;
            if(!Player.Alive||QuestState!=QuestState.InProgress)return;
            if(Quest==2 && Player.Map==Map.Academy){
                if(Stage==0 && position.Distance(new Point(0,.8))<=1.5){Stage=1;Emit("HV_Entrance");}
                if(Stage==1 && grounded && position.Distance(new Point(8,3.8))<=1.5){Stage=2;Emit("HV_JumpLedge — đạt cao độ thật");}
                if(Stage==2 && dropped)Receipts.Add("Q2.dropped");
                if(Stage==2 && Receipts.Contains("Q2.dropped") && grounded && position.Distance(new Point(8,.8))<=1.5){Stage=3;Emit("HV_DropLanding — tiếp đất thật");}
            }
            if(Quest==3&&Stage==1&&Player.Map==Map.Academy&&position.Distance(new Point(22,.8))<=1.5)Stage=2;
            if(Quest==4&&Stage==2&&Player.Map==Map.Mist&&position.Distance(new Point(50,.8))<=1.5)Stage=3;
            if(Quest==6&&Stage==0&&Player.Map==Map.Academy&&position.Distance(new Point(3,.8))<=1.5)Stage=1;
        }
        public bool Portal(string id) {
            if(!Player.Alive||!Near(id))return false;
            if(id=="outOfSlice"){Emit(Complete?"Trúc Ảnh đã mở trong tiến trình, chưa có trong VS-1.":"Cần hoàn thành Q6.");return false;}
            Map destination=id=="toAcademy"?Map.Academy:id=="toMist"?Map.Mist:Map.Village;
            if(Quest==2&&Stage==3&&Player.Map==Map.Academy&&destination==Map.Village){Stage=4;QuestState=QuestState.Ready;}
            ChangeMap(destination,destination==Map.Village?new Point(id=="toVillageA"?25:30,.8):new Point(-3,.8));return true;
        }
        private void ChangeMap(Map map,Point spawn){
            foreach(var m in Mobs.Where(x=>x.Map==Player.Map&&x.Alive&&!x.Dummy)){m.Windup=false;m.ReturnSince=Now;}
            Combat.Cancel();Combat.ClearFocus();Player.Map=map;Player.Position=spawn;WorldRevision++;Emit("Portal → "+map);
        }
        public bool Rest() {if(!Player.Alive||!Near("Moc"))return false;Player.Hp=Player.Stats.Hp;Player.Mp=Player.Stats.Mp;Emit("Mộc An hồi đầy HP/MP.");return true;}
        public bool Revive(bool village) {
            if(Player.Alive)return false;
            if(!village){var scroll=Player.Inventory.Bag.FirstOrDefault(x=>x.Id=="scroll");if(scroll==null)return false;Player.Inventory.Consume(scroll);}
            Combat.Cancel();Combat.ClearFocus();
            if(village)ChangeMap(Map.Village,new Point(15,.8));
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
                if(Quest==4&&Stage==3&&m.Slot.StartsWith("DS")&&m.Level==4){Kills++;if(Kills==5){Stage=4;QuestState=QuestState.Ready;}}
                if(Quest==5&&Stage==0&&m.Slot=="DS2.slot1"){Stage=1;TutorialSupply(m.Position);}
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
                if(!m.Alive){if(Now>=m.RespawnAt){m.Generation++;m.Hp=m.MaxHp;m.Position=m.Home;m.QuestDamage=0;m.Returning=false;m.ReturnSince=-1;m.NextAttack=Now;Emit("Respawn "+m.Slot+"@"+m.Generation);}continue;}
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
                TickMob(m,dt);
            }
            // Tutorial entitlement keeps the same instance; regular world loot expires.
            foreach(var l in Loot.Where(x=>!x.Claimed&&Now-x.Created>=60)){
                if(l.Tutorial){l.Position=Mobs.First(x=>x.Slot=="DS2.slot1").Home;l.Created=Now;Emit("Tutorial supply re-offer same instance "+l.Id);}
                else l.Claimed=true;
            }
        }
        private void TickMob(Mob m,double dt) {
            double dist=m.Position.Distance(Player.Position);
            bool reachable=Math.Abs(m.Position.Y-Player.Position.Y)<1.6;
            if(m.Returning){
                double dx=m.Home.X-m.Position.X;m.Position=new Point(m.Position.X+Math.Sign(dx)*Math.Min(Math.Abs(dx),m.Speed*dt),m.Home.Y);
                if(Math.Abs(dx)<.05){m.Returning=false;m.Hp=m.MaxHp;m.QuestDamage=0;m.ReturnSince=-1;m.Windup=false;}return;
            }
            // VS-1 probe: 2s no reachable progress -> Return + reset. This is NOT a production lock.
            // Resolve on the original hit clock, even when the player has left reach.
            // Reachability grace must never postpone an old hit until the player returns.
            if(m.Windup&&Now>=m.HitAt){
                m.Windup=false;
                if(Player.Alive&&Math.Abs(Player.Position.X-m.Position.X)<=m.Range && Math.Abs(Player.Position.Y-m.Position.Y)<1.3 && Math.Sign(Player.Position.X-m.Position.X)==m.Facing){
                    var s=Player.Stats;if(Random.NextDouble()>=Rules.Evade(m.Acc,s.Eva))HurtPlayer(Rules.Damage(m.Atk,1,1,s.Def,false,.95+Random.NextDouble()*.1));else Emit("NÉ đòn "+m.Name);
                }
            }
            if(!Player.Alive||dist>8||!reachable){
                if(m.Hp<m.MaxHp||m.Position.Distance(m.Home)>.1){if(m.ReturnSince<0)m.ReturnSince=Now;if(Now-m.ReturnSince>2){m.Returning=true;m.Windup=false;}}
                return;
            }
            m.ReturnSince=-1;
            if(m.Windup)return;
            if(dist>5 && m.Hp==m.MaxHp)return;
            double delta=Player.Position.X-m.Position.X;
            if(Math.Abs(delta)>m.Range*.9){m.Position=new Point(m.Position.X+Math.Sign(delta)*Math.Min(Math.Abs(delta)-m.Range*.8,m.Speed*dt),m.Home.Y);}
            else if(Now>=m.NextAttack){m.Facing=delta<0?-1:1;m.Windup=true;m.HitAt=Now+.35;m.NextAttack=Now+m.Interval;}
            if(Math.Abs(m.Position.X-m.Home.X)>8){m.Returning=true;m.Windup=false;}
        }
    }
}
