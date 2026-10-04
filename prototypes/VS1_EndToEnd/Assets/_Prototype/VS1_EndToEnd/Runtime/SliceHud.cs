using System;
using System.Collections.Generic;
using System.Linq;
using HuyenLo.Domain;
using UnityEngine;
namespace HuyenLo.Runtime
{
    public sealed class MenuAction {public string Id,Label,Reason;public bool Enabled;public Func<bool> Execute;}
    // Presentation holds focus/breadcrumbs only. All item, quest and currency commands stay in domain.
    public sealed class SliceHud : MonoBehaviour
    {
        public SliceHost Host;
        public bool Modal=>panel!=null||Host!=null&&!Host.Session.Player.Alive;
        public string Panel=>panel;
        public string SelectedActionId=>actions.Count==0?null:actions[Mathf.Clamp(selected,0,actions.Count-1)].Id;
        public IReadOnlyList<MenuAction> Actions {get{Refresh();return actions;}}
        private string panel,npc;
        private long itemInstance;
        private GearSlot chosenSlot;
        private GearSlot? bagFilter;
        private readonly Dictionary<string,string> tabFocus=new Dictionary<string,string>();
        private readonly string[] tabs={"bag","equipment","attributes","stats","skills"};
        private readonly string[] tabNames={"Hành trang","Trang bị","Thuộc tính","Thông số","Kỹ năng"};
        private Item[] BagItems=>P.Inventory.Bag.Where(x=>!bagFilter.HasValue||Catalog.Get(x.Id).Kind==ItemKind.Gear&&Catalog.Get(x.Id).Slot==bagFilter.Value).ToArray();
        private bool Rpg=>panel!=null&&(tabs.Contains(panel)||panel=="item"||panel=="equipped");
        public void ChangeTab(int direction){if(!Rpg){Navigate(direction);return;}int i=Array.IndexOf(tabs,panel);SwitchTab(tabs[(Math.Max(0,i)+direction+tabs.Length)%tabs.Length]);}
        public void SwitchTab(string name){if(panel!=null)tabFocus[panel]=SelectedActionId;trail.Clear();npc=null;if(name=="bag")bagFilter=null;Open(name);if(tabFocus.TryGetValue(name,out var focus))SelectAction(focus);}
        private bool FilterBag(){bagFilter=chosenSlot;trail.Clear();Open("bag");return true;}

        private readonly List<MenuAction> actions=new List<MenuAction>();
        private readonly Stack<(string panel,string action)> trail=new Stack<(string,string)>();
        private int selected;
        private Vector2 scroll;
        private Font font;
        private PrototypeStart pendingStart;
        private bool showTrace;
        public void ToggleTrace()=>showTrace=!showTrace;
        private SliceSession S=>Host.Session;
        private Player P=>S.Player;
        private Item SelectedItem=>P.Inventory.Bag.FirstOrDefault(x=>x.Instance==itemInstance);
        private void Awake(){font=Resources.Load<Font>("Fonts/DejaVuSans");}
        public void Toggle(string name){if(panel==name)Close();else {Close();if(name=="bag")bagFilter=null;Open(name);}}
        private void Open(string name){panel=name;selected=0;actions.Clear();scroll=Vector2.zero;S.Combat.CancelIntent();Refresh();}
        private bool Sub(string name){trail.Push((panel,SelectedActionId));Open(name);return true;}
        public void OpenNpc(string id){Close();npc=id;Open("npc");}
        public void Close(){panel=npc=null;selected=0;actions.Clear();trail.Clear();S.Combat.CancelIntent();}
        public void Back(){if(trail.Count==0){Close();return;}var previous=trail.Pop();Open(previous.panel);SelectAction(previous.action);}
        public bool IsOverUi(Vector2 screen){float y=Screen.height-screen.y;return Modal||y>Screen.height-78||(y<190&&screen.x<333)||(y<174&&screen.x>Screen.width-345)||(S.Combat.Focus!=null&&y<100&&screen.x>=345&&screen.x<=595);}
        public void Navigate(int delta){Refresh();if(actions.Count==0)return;selected=(selected+delta+actions.Count)%actions.Count;scroll.y=Mathf.Max(0,(selected-6)*36);}
        public void NavigateGrid(int x,int y){Navigate((panel=="bag"?6:panel=="equipment"?2:1)*y+x);}
        public bool SelectAction(string id){Refresh();int index=actions.FindIndex(x=>x.Id==id);if(index<0)return false;selected=index;return true;}
        public bool ActivateSelected(){Refresh();return actions.Count>0&&Activate(actions[selected]);}
        private bool Activate(MenuAction action){
            if(!action.Enabled){S.Feedback=action.Reason??"Thao tác chưa hợp lệ.";return false;}
            bool result=action.Execute();if(!result)S.Feedback=action.Reason??"Không thể thực hiện. Kiểm tra vị trí, túi và điều kiện.";
            Refresh();return result;
        }
        private void Add(string id,string label,Func<bool> execute,bool enabled=true,string reason=null)=>actions.Add(new MenuAction{Id=id,Label=label,Execute=execute,Enabled=enabled,Reason=reason});
        public string Marker(string id){
            if(!S.Complete&&id==S.QuestNpc)return S.QuestState==QuestState.Ready?"?":S.QuestState==QuestState.Available?"!":"…";
            if(id=="Phong")return S.Quest==6&&S.Stage==1&&P.School==School.Novice?"!":"";
            return "";
        }
        public static string QuestIntro(int q)=>new[]{
            "Trước khi ra núi, hãy biết nơi tìm thuốc, rèn đồ và nghỉ chân. Gặp Yên Thảo → Bách Luyện → Mộc An rồi về đây.",
            "Đường núi có nhiều bậc. Sang Học Viện, nhảy lên sàn gỗ rồi dùng S/↓ đi xuống; trở lại báo ta.",
            "Mộc Kiếm giúp ngươi luyện thế. Mặc kiếm trong túi, tới sân phía đông đánh ba Bù Nhìn rồi gặp ta.",
            "Nấm ở bãi DS2 có thứ dùng được. Nhặt áo và mẫu Nấm, mặc áo rồi mang mẫu về bán tại lò rèn.",
            "Ăn trước khi ra bãi Sói, mang theo bình máu. Tới lối DS4 rồi luyện với năm Sói DS3–DS6; quay về báo ta.",
            "Đã đủ căn cơ. Sang đại sảnh Học Viện, tháo Mộc Kiếm rồi gặp Phong Du. Học kiếm, phân điểm và thử hồi Linh lực."}[Mathf.Clamp(q-1,0,5)];
        public static string QuestComplete(int q)=>new[]{"Ngươi đã biết đường về và nơi tìm trợ giúp.","Chân đã vững; Phong Du sẽ dạy thế kiếm.","Giữ chiếc quần này. Bách Luyện còn việc cho ngươi.","Áo vừa người rồi. Ghé Yên Thảo trước khi thử sức với Sói.","Ngươi đã sẵn sàng nhập môn; gặp Tạ Minh.","Một thế kiếm, một hơi thở. Hành trình tiếp theo đợi ngoài lát cắt này."}[Mathf.Clamp(q-1,0,5)];
        public string Dialogue(string id){
            if(id==S.QuestNpc&&S.QuestState==QuestState.Available&&!S.Complete)return QuestIntro(S.Quest);
            if(id=="Lam")return "Ra núi rồi, nhớ đường về. Chân vững hãy cầm kiếm.";
            if(S.Quest==1&&S.QuestState==QuestState.InProgress&&id=="Yen")return "Tiệm thuốc ở đây. Khi ra bãi, nhớ Food và Bình Máu; tiếp theo gặp Bách Luyện ở lò rèn.";
            if(S.Quest==1&&S.QuestState==QuestState.InProgress&&id=="Bach")return "Mặc đồ trước, bán đồ thừa sau. Mộc An ở quán nghỉ sẽ chỉ chỗ gửi đồ.";
            if(id=="Yen")return "Ăn trước khi đi. Thuốc để dành lúc cần.";
            if(id=="Bach")return "Thứ mặc được thì giữ. Thứ thừa đem bán, lấy đồng lộ phí.";
            if(id=="Moc")return "Nghỉ một lát. Đồ chưa dùng, cứ gửi ở đây.";
            if(id=="Ta")return "Kiếm hay cung, tự ngươi chọn. Đã chọn, phải học giữ hơi thở.";
            if(id=="Diep")return "Ta hướng dẫn Cung. Lát cắt hiện tại chỉ mở nhánh Kiếm.";
            return S.Quest==6?"Đặt Mộc Kiếm vào túi trước khi nhập phái. Rồi học giữ thế." : "Đừng vội. Giữ thế cho chắc, rồi ra đòn.";
        }
        private void Refresh(){
            if(Host==null||S==null)return;string previous=SelectedActionId;actions.Clear();
            if(!P.Alive&&panel!="debug"&&panel!="confirm"){
                Add("revive.village","Về Vân Khê — miễn phí",()=>{Close();return S.Revive(true);});
                Add("revive.scroll","Dùng phù — 50% HP/MP",()=>{bool ok=S.Revive(false);if(ok)Close();return ok;},P.Inventory.Count("scroll")>0,"Không có Hồi Sinh Phù.");
            }else if(panel=="bag"){
                for(int index=0;index<P.Inventory.Capacity;index++){
                    if(index<BagItems.Length){var i=BagItems[index];Add("item."+i.Instance,Catalog.Get(i.Id).Name,()=>{itemInstance=i.Instance;return Sub("item");});}
                    else Add("empty."+index,"Ô trống",()=>false,false,"Ô này chưa có vật phẩm.");
                }
            }else if(panel=="item")ItemActions();
            else if(panel=="equipment"){
                foreach(var slot in new[]{GearSlot.Weapon,GearSlot.Necklace,GearSlot.Armor,GearSlot.Ring,GearSlot.Pants,GearSlot.Boots}){
                    var value=slot;bool has=P.Inventory.Equipment.TryGetValue(value,out var i);Add("slot."+value,SlotName(value)+": "+(has?Catalog.Get(i.Id).Name:"Trống"),()=>{chosenSlot=value;return Sub("equipped");},true);
                }
            }else if(panel=="equipped"){
                Add("unequip."+chosenSlot,"Tháo "+SlotName(chosenSlot)+" vào túi",()=>{bool ok=S.Unequip(chosenSlot);if(ok)Back();return ok;},P.Inventory.Equipment.ContainsKey(chosenSlot),P.Inventory.Equipment.ContainsKey(chosenSlot)?"Cần ô trống trong túi để tháo món.":"Slot đang trống.");
                Add("open.bag","Hành trang lọc "+SlotName(chosenSlot),FilterBag);
            }else if(panel=="attributes"){
                foreach(string stat in new[]{"STR","VIT","INT","AGI"}){string value=stat;Add("allocate."+value,"+1 "+new Dictionary<string,string>{{"STR","Công Lực"},{"VIT","Sinh Lực"},{"INT","Linh Lực"},{"AGI","Thân Pháp"}}[value],()=>S.Allocate(value),P.Unspent>0,"Không còn điểm chưa dùng.");}

            }else if(panel=="npc")Npc();
            else if(panel=="buy")ShopBuy();else if(panel=="sell")ShopSell();else if(panel=="store")Storage(true);else if(panel=="retrieve")Storage(false);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            else if(panel=="debug"){
                Add("debug.resume","Tiếp tục phiên hiện tại",()=>{Close();return true;});
                foreach(PrototypeStart start in Enum.GetValues(typeof(PrototypeStart))){var value=start;string name=new[]{"Bắt đầu mới / reset","Q2 — movement","Q3 — Dummy","Q4 — Nấm / loot","Q5 — Sói / Food","Q6 — nhập môn","Sân tập Kiếm Lv5","Bãi phụ — đàn 4 Sói / đường cao"}[(int)value-1];Add("debug."+value,name,()=>{pendingStart=value;return Sub("confirm");});}
            }else if(panel=="confirm"){
                Add("debug.cancel","Hủy — giữ phiên hiện tại",()=>{Back();return true;});
                Add("debug.apply","Reset toàn phiên và mở mốc "+pendingStart,()=>{Host.ResetPrototype(pendingStart);Close();return true;});
            }
#endif
            if(panel!=null)Add("close",trail.Count>0?"← Quay lại (Esc)":"Đóng (Esc)",()=>{Back();return true;});
            int same=actions.FindIndex(x=>x.Id==previous);selected=same>=0?same:Mathf.Clamp(selected,0,Math.Max(0,actions.Count-1));
        }
        private bool CanUse(Item i,out string reason){
            reason="";if(i==null){reason="Ô trống";return false;}var d=Catalog.Get(i.Id);
            if(d.Kind==ItemKind.Gear&&(P.Level<d.MinLevel||i.Id=="wood"&&P.School!=School.Novice||i.Id=="sword1"&&P.School!=School.Sword)){reason="Cần đúng cấp / phái";return false;}
            if(d.Kind==ItemKind.Gear&&S.Combat.Running!=null){reason="Chờ action kết thúc";return false;}
            if(d.Kind==ItemKind.HpPotion&&(P.Hp>=P.Stats.Hp||S.Now<S.HpPotionUntil)||d.Kind==ItemKind.MpPotion&&(P.Mp>=P.Stats.Mp||S.Now<S.MpPotionUntil)){reason="Đã đầy hoặc bình đang hồi chiêu";return false;}
            if(d.Kind==ItemKind.Manual&&(P.School!=School.Sword||P.Level<5)){reason="Cần nhập phái Kiếm";return false;}return true;
        }
        private void ItemActions(){
            var i=SelectedItem;if(i==null){Add("missing","Món đã dùng hoặc chuyển khỏi túi",()=>{Back();return true;});return;}
            var d=Catalog.Get(i.Id);bool allowed=CanUse(i,out var reason);
            if(d.Kind==ItemKind.Gear)Add("equip."+i.Instance,"Mặc "+d.Name,()=>{bool ok=S.Equip(i.Instance);if(ok)Back();return ok;},allowed,reason);
            else if(d.Kind==ItemKind.Manual)Add("learn."+i.Instance,"Học "+d.Name,()=>{bool ok=S.Learn(i.Instance);if(ok)Back();return ok;},allowed,reason);
            else if(d.Kind==ItemKind.Food)Add("use."+i.Instance,"Dùng Food",()=>S.UseFood());
            else if(d.Kind==ItemKind.HpPotion||d.Kind==ItemKind.MpPotion)Add("use."+i.Instance,"Dùng bình",()=>S.Potion(d.Kind==ItemKind.HpPotion),allowed,reason);
            else Add("info."+i.Instance,"Vật liệu — bán tại Bách Luyện / cất tại Mộc An",()=>true);
        }
        private void Npc(){
            if(npc==null)return;
            if(npc==S.QuestNpc&&!S.Complete){
                if(S.QuestState==QuestState.Available)Add("quest.accept","Nhận Q"+S.Quest,()=>{int q=S.Quest;bool ok=S.AcceptQuest();if(ok){Host.SpeakText(npc,QuestIntro(q));Close();}return ok;});
                if(S.QuestState==QuestState.Ready)Add("quest.turnin","Trả Q"+S.Quest,()=>{int q=S.Quest;bool ok=S.TurnIn();if(ok){Host.SpeakText(npc,QuestComplete(q));Close();}return ok;});

            }
            if(npc=="Phong")Add("class.sword","Nhập phái Kiếm",()=>{bool ok=S.ChooseSword();if(ok){Host.SpeakText(npc,"Đã nhập phái Kiếm. Mặc kiếm, học bí kíp trong túi rồi phân điểm.");Close();}return ok;},S.Quest==6&&S.Stage==1&&P.School==School.Novice&&!P.Inventory.Equipment.ContainsKey(GearSlot.Weapon),P.School!=School.Novice?"Bạn đã nhập phái; không đổi phái trong mock.":S.Quest!=6||S.Stage!=1?"Cần đúng bước nhập môn Q6.":"Tháo Mộc Kiếm tại C → Trang bị trước.");
            if(npc=="Diep")Add("class.bow","Nhập phái Cung — chưa mở trong VS-1",()=>false,false,"Diệp Lam thuộc nhánh Cung; prototype này chỉ mở Kiếm.");
            if(npc=="Moc"){
                Add("rest","Nghỉ — hồi đầy HP/MP",()=>{bool ok=S.Rest();if(ok)Close();return ok;});Add("service.store","Rương → Gửi đồ",()=>Sub("store"));Add("service.retrieve","Rương → Lấy đồ",()=>Sub("retrieve"));
            }
            if(npc=="Yen"||npc=="Bach"){
                if(npc=="Yen"||S.Quest!=4||S.Stage!=3)Add("service.buy","Mua hàng",()=>Sub("buy"));
                if(npc=="Bach")Add("service.sell","Bán đồ trong túi",()=>Sub("sell"));
                if(npc=="Bach"&&S.Quest==4&&S.Stage==3)Add("service.buy","Mua hàng",()=>Sub("buy"));
            }
            if(npc==S.QuestNpc&&!S.Complete&&S.QuestState==QuestState.InProgress)Add("quest.details","Xem nhiệm vụ đang làm",()=>Sub("quest"));
        }
        private void ShopBuy(){
            foreach(string id in npc=="Yen"?new[]{"food1","hp1","mp1","scroll"}:new[]{"sword1","armor1","pants1","boots1","ring1","neck1","stone"}){
                var key=id;var d=Catalog.Get(id);Add("buy."+id,$"{d.Name} — {d.Buy} Vàng",()=>S.Buy(key,npc),P.Gold>=d.Buy&&(id!="sword1"||P.School==School.Sword),"Không đủ Vàng, sai phái hoặc túi đầy.");
            }
        }
        private void ShopSell(){
            foreach(var item in P.Inventory.Bag.Where(x=>Catalog.Get(x.Id).Sell>0).OrderByDescending(x=>x.Binding=="Q4").ToArray()){
                var i=item;Add("sell."+i.Instance,"Bán 1 "+Catalog.Get(i.Id).Name+" ×"+i.Count,()=>S.Sell(i.Instance),i.Binding==null||S.Quest==4&&S.Stage==3&&i.Id=="mushroom","Đồ hướng dẫn chưa được phép bán.");
            }
        }
        private void Storage(bool deposit){
            foreach(var item in (deposit?P.Inventory:P.Storage).Bag.ToArray()){var i=item;Add((deposit?"store.":"retrieve.")+i.Instance,(deposit?"Cất ":"Lấy ")+Catalog.Get(i.Id).Name+" ×"+i.Count,()=>S.Store(i.Instance,deposit));}
        }
        private static string SlotName(GearSlot slot)=>new Dictionary<GearSlot,string>{{GearSlot.Weapon,"Vũ khí"},{GearSlot.Armor,"Áo"},{GearSlot.Pants,"Quần"},{GearSlot.Boots,"Giày"},{GearSlot.Ring,"Nhẫn"},{GearSlot.Necklace,"Dây chuyền"}}[slot];
        private static string Quality(Item i)=>i.Quality>=1.16?"Rare":i.Quality>=1.08?"Uncommon":"Common";
        private string Description(Item i){
            if(i==null)return "Ô trống\nChọn món bằng phím mũi tên hoặc chuột.";var d=Catalog.Get(i.Id);
            string stats="";foreach(var pair in new[]{("HP",d.Hp),("MP",d.Mp),("ATK",d.Atk),("DEF",d.Def),("ACC",d.Acc),("EVA",d.Eva)})if(pair.Item2!=0)stats+=pair.Item1+" +"+(pair.Item2*i.Quality).ToString("0.##")+"\n";
            if(d.Crit>0)stats+="Chí mạng +"+d.Crit.ToString("P1")+"\n";if(d.Speed>0)stats+="Tốc chạy +"+d.Speed.ToString("P0")+"\n";
            return d.Name+" ×"+i.Count+"\n"+(d.Kind==ItemKind.Gear?Quality(i)+" · "+SlotName(d.Slot)+" · Lv"+d.MinLevel+"\n":"")+stats+(i.Binding!=null?"Đồ hướng dẫn ["+i.Binding+"]\n":"")+(d.Sell>0?"Giá bán cơ bản: "+d.Sell+" Vàng":"Không bán")+"\n"+(d.Kind==ItemKind.Manual?"Dùng để học kỹ năng; đúng phái và cấp.":d.Kind==ItemKind.Food?"Hồi HP/MP mỗi 2s trong 10 phút.":d.Kind==ItemKind.HpPotion||d.Kind==ItemKind.MpPotion?"Hồi 30%; HP và MP có cooldown riêng 8s.":"");
        }
        private static void Fill(Rect r,Color color){var old=GUI.color;GUI.color=color;GUI.DrawTexture(r,Texture2D.whiteTexture);GUI.color=old;}
        private void Icon(Rect r,Item item){
            if(item==null)return;var d=Catalog.Get(item.Id);float x=r.center.x,y=r.center.y;
            Color c=new Color(.78f,.83f,.68f);
            if(d.Slot==GearSlot.Weapon){Fill(new Rect(x-3,y-20,6,35),c);Fill(new Rect(x-13,y+6,26,4),new Color(.65f,.45f,.25f));}
            else if(d.Slot==GearSlot.Armor){Fill(new Rect(x-12,y-13,24,27),c);Fill(new Rect(x-22,y-13,44,8),c);}
            else if(d.Slot==GearSlot.Pants){Fill(new Rect(x-15,y-14,30,7),c);Fill(new Rect(x-15,y-7,11,25),c);Fill(new Rect(x+4,y-7,11,25),c);}
            else if(d.Slot==GearSlot.Boots){Fill(new Rect(x-9,y-17,12,28),c);Fill(new Rect(x-9,y+8,24,9),c);}
            else if(d.Slot==GearSlot.Ring||d.Slot==GearSlot.Necklace){GUI.Box(new Rect(x-13,y-13,26,26),d.Slot==GearSlot.Ring?"○":"◇");}
            else if(d.Kind==ItemKind.Manual){Fill(new Rect(x-14,y-18,28,36),new Color(.56f,.63f,.39f));GUI.Label(new Rect(x-12,y-12,28,27),"BK");}
            else if(d.Kind==ItemKind.HpPotion||d.Kind==ItemKind.MpPotion){Fill(new Rect(x-6,y-18,12,9),c);Fill(new Rect(x-12,y-9,24,27),d.Kind==ItemKind.HpPotion?new Color(.7f,.29f,.25f):new Color(.28f,.55f,.8f));}
            else {Fill(new Rect(x-14,y-12,28,24),c);GUI.Label(new Rect(x-13,y-10,34,24),d.Kind==ItemKind.Food?"F":d.Kind==ItemKind.Stone?"◆":"M");}
            GUI.Label(new Rect(r.x+3,r.y+2,r.width-6,18),d.Kind==ItemKind.Gear?SlotName(d.Slot):d.Kind==ItemKind.Manual?"Bí kíp":d.Kind==ItemKind.Food?"Food":d.Kind==ItemKind.HpPotion?"HP":d.Kind==ItemKind.MpPotion?"MP":"Vật liệu");
            GUI.Label(new Rect(r.xMax-27,r.yMax-21,25,18),item.Count.ToString());
        }
        // A command can rebuild actions/panel. End this IMGUI event before drawing old controls.
        private void DrawButton(MenuAction action,Rect r){
            int index=actions.IndexOf(action);var old=GUI.backgroundColor;GUI.backgroundColor=index==selected?new Color(.64f,.8f,.42f):action.Enabled?new Color(.35f,.5f,.57f):Color.gray;
            if(GUI.Button(r,(index==selected?"▶ ":"")+action.Label+(action.Enabled?"":" [khóa]"))){selected=index;GUI.backgroundColor=old;Activate(action);GUIUtility.ExitGUI();}GUI.backgroundColor=old;
        }
        private void DrawInventory(Rect area){
            GUI.Label(new Rect(area.x+20,area.y+12,500,25),"Hành trang "+(bagFilter.HasValue?"[lọc "+SlotName(bagFilter.Value)+": "+BagItems.Length+"]":"")+" · "+P.Inventory.Bag.Count+" / "+P.Inventory.Capacity);
            GUI.Label(new Rect(area.x+20,area.y+42,760,25),"↑↓←→ chọn ô · Enter/E mở thao tác · Esc quay lại · icon khối tạm");
            for(int i=0;i<P.Inventory.Capacity;i++){
                Rect cell=new Rect(area.x+20+(i%6)*62,area.y+85+(i/6)*62,56,56);var old=GUI.backgroundColor;GUI.backgroundColor=i==selected?new Color(.64f,.8f,.42f):new Color(.28f,.4f,.48f);
                if(GUI.Button(cell,"")){selected=i;}GUI.backgroundColor=old;
                Icon(cell,i<BagItems.Length?BagItems[i]:null);
            }
            Item item=selected<BagItems.Length?BagItems[selected]:null;
            GUI.Box(new Rect(area.x+435,area.y+85,area.width-455,335),"");GUI.Label(new Rect(area.x+450,area.y+100,area.width-485,210),Description(item));
            if(item!=null){
                var d=Catalog.Get(item.Id);string label=d.Kind==ItemKind.Gear?"Trang bị ngay":d.Kind==ItemKind.Manual?"Học bí kíp":d.Kind==ItemKind.Food||d.Kind==ItemKind.HpPotion||d.Kind==ItemKind.MpPotion?"Dùng":"Chi tiết";
                bool allowed=CanUse(item,out var reason);GUI.enabled=allowed;
                if(GUI.Button(new Rect(area.x+450,area.y+320,area.width-485,30),label)){GUI.enabled=true;itemInstance=item.Instance;Sub("item");ActivateSelected();GUIUtility.ExitGUI();}
                GUI.enabled=true;if(!allowed)GUI.Label(new Rect(area.x+450,area.y+352,area.width-485,24),reason);
                if(d.Kind==ItemKind.Gear){P.Inventory.Equipment.TryGetValue(d.Slot,out var current);GUI.Label(new Rect(area.x+450,area.y+360,area.width-485,52),"Đang mặc: "+(current==null?"Trống":Catalog.Get(current.Id).Name));}
                if(d.Kind==ItemKind.Gear&&GUI.Button(new Rect(area.x+450,area.y+415,area.width-485,28),"Xem Trang bị")){SwitchTab("equipment");GUIUtility.ExitGUI();}
            }
            DrawButton(actions.Last(),new Rect(area.x+20,area.yMax-50,200,32));
        }
        private void DrawEquipment(Rect area){
            GUI.Label(new Rect(area.x+20,area.y+15,700,25),"Trang bị · Enter/E chọn slot để tháo · I: hành trang · Esc: quay lại");
            float cx=area.x+area.width/2,cy=area.y+220;
            foreach(var part in GeometricRig.Pose(P,1,S.Now,0,false).OrderBy(x=>x.Layer)){
                var r=new Rect(cx+part.Center.x*100-part.Size.x*50,cy-part.Center.y*100-part.Size.y*50,part.Size.x*100,part.Size.y*100);
                var matrix=GUI.matrix;GUIUtility.RotateAroundPivot(-part.Angle,r.center);Fill(r,part.Color);GUI.matrix=matrix;
            }
            for(int i=0;i<6;i++){
                Rect r=new Rect(cx+(i%2==0?-340:155),area.y+90+(i/2)*95,185,85);var a=actions[i];DrawButton(a,r);
            }
            if(GUI.Button(new Rect(cx-85,area.y+365,170,26),"Điểm chưa dùng: "+P.Unspent)){SwitchTab("attributes");GUIUtility.ExitGUI();}
            GUI.Label(new Rect(cx-55,area.y+397,120,26),P.School==School.Novice?"Tân Lữ":"Kiếm · Lv"+P.Level);DrawButton(actions.Last(),new Rect(area.x+20,area.yMax-50,200,32));
        }
        private string ViewTitle=>panel=="equipped"?"Trang bị · "+SlotName(chosenSlot):panel=="item"?"Vật phẩm":panel=="quest"?"Nhiệm vụ":"Thao tác";
        private void OnGUI(){
            if(Host==null||S==null)return;Refresh();GUI.skin.font=font;GUI.skin.label.fontSize=15;GUI.skin.label.wordWrap=true;GUI.skin.button.fontSize=14;GUI.skin.button.wordWrap=true;
            var st=P.Stats;
            GUI.Box(new Rect(8,8,325,180),"");GUILayout.BeginArea(new Rect(18,15,305,170));GUILayout.Label("Huyền Lộ · V6.2.7 · "+SliceHost.MapName(P.Map));
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if(GUI.Button(new Rect(8,Screen.height-27,95,22),"DEV · F8")){Toggle("debug");GUIUtility.ExitGUI();}
#endif
            GUILayout.Label($"Lv{P.Level} · {(P.School==School.Novice?"Tân Lữ":"Kiếm")} · Vàng {P.Gold}");GUILayout.Label($"HP {P.Hp:F0}/{st.Hp:F0} · MP {P.Mp:F0}/{st.Mp:F0}");GUILayout.Label($"EXP {P.TotalExp} · điểm {P.Unspent}");GUILayout.Label($"Food {(S.FoodUntil>S.Now?$"{S.FoodUntil-S.Now:F0}s":"chưa dùng")} · H:HP {Math.Max(0,S.HpPotionUntil-S.Now):F1}s · M:MP {Math.Max(0,S.MpPotionUntil-S.Now):F1}s");GUILayout.EndArea();
            GUI.Box(new Rect(Screen.width-340,8,332,165),"");GUI.Label(new Rect(Screen.width-330,16,310,105),S.Objective);GUI.Label(new Rect(Screen.width-330,125,310,43),"E: tương tác · I: túi · C: nhân vật · Q: quest");
            var focus=S.Combat.Focus;if(focus!=null){GUI.Box(new Rect(345,8,250,90),"");GUI.Label(new Rect(355,14,230,27),$"{focus.Name} Lv{focus.Level}");GUI.Label(new Rect(355,42,230,24),$"{focus.Hp:F0} / {focus.MaxHp:F0}");Fill(new Rect(355,70,230*(float)(focus.Hp/focus.MaxHp),10),Color.green);var pt=Host.ScreenPoint(new Point(focus.Position.X,focus.Position.Y+1));Fill(new Rect(pt.x-25,Screen.height-pt.y,50*(float)(focus.Hp/focus.MaxHp),5),Color.red);}
            GUI.Label(new Rect(112,Screen.height-31,Screen.width-230,25),S.Feedback);
            string skills=P.School==School.Novice?"Mộc Kiếm":string.Join(" · ",S.Combat.Unlocked.Values.Select(x=>x.Name));
            float barX=Screen.width/2-132;
            var slotText=new GUIStyle(GUI.skin.label){fontSize=12};
            for(int i=1;i<=3;i++){
                bool unlocked=S.Combat.Unlocked.TryGetValue(i,out var skill);if(P.School==School.Novice&&i==1){skill=Rules.Novice;unlocked=true;}bool active=unlocked&&S.Combat.Selected==skill;
                var r=new Rect(barX+(i-1)*90,Screen.height-110,82,70);Fill(r,active?new Color(.22f,.4f,.35f):new Color(.12f,.2f,.25f));GUI.Box(r,"");
                GUI.Label(new Rect(r.x+5,r.y+3,72,22),i+"   "+(unlocked?"◆":"×"));
                GUI.Label(new Rect(r.x+5,r.y+26,72,23),unlocked?skill.Name.Replace(" nhập môn","").Replace(" tiến cảnh",""):"Chưa học",slotText);
                if(unlocked)GUI.Label(new Rect(r.x+5,r.y+49,72,22),S.Combat.Remaining(skill)>0?"CD "+S.Combat.Remaining(skill).ToString("0.0"):"MP "+skill.Mp);
            }
            GUI.Label(new Rect(barX+282,Screen.height-91,225,48),"H: HP · M: MP · F: Food\n"+(S.DebugPreset==null?"":"DEV preset: "+S.DebugPreset));
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if(showTrace){
                float ty=110;foreach(var m in S.Mobs.Where(x=>x.Alive&&x.Map==P.Map&&x.Engaged).Take(7)){
                    var r=new Rect(345,ty,Screen.width-700,45);Fill(r,new Color(.07f,.1f,.13f,.95f));GUI.Label(r,$"DEV F9 · {m.Id} → player {P.Position.X:F1}/{P.Position.Y:F1} · {m.Motion}\ngoal {m.DesiredX:F2} · {m.Occupancy} · v {m.VelocityX:F2} · Δx {Math.Abs(m.Position.X-P.Position.X):F2}/{m.Range} · bites {m.BiteAttempts}");ty+=46;
                }
            }
#endif
            if(!Modal)return;float width=Mathf.Min(820,Screen.width-40),height=Mathf.Min(550,Screen.height-150);Rect area=new Rect((Screen.width-width)/2,115,width,height);Fill(area,new Color(.08f,.14f,.18f,1));GUI.Box(area,"");
            if(Rpg){
                for(int i=0;i<tabs.Length;i++){var r=new Rect(area.x+i*area.width/5,area.y,area.width/5,32);var old=GUI.backgroundColor;GUI.backgroundColor=panel==tabs[i]?new Color(.5f,.7f,.45f):Color.gray;if(GUI.Button(r,tabNames[i])){GUI.backgroundColor=old;SwitchTab(tabs[i]);GUIUtility.ExitGUI();}GUI.backgroundColor=old;}
                area.y+=38;area.height-=38;
            }
            if(panel=="bag"){DrawInventory(area);return;}if(panel=="equipment"){DrawEquipment(area);return;}
            GUILayout.BeginArea(new Rect(area.x+20,area.y+12,area.width-40,area.height-24));
            GUILayout.Label(!P.Alive&&panel!="debug"&&panel!="confirm"?"Đã chết":npc!=null?SliceSession.Anchors.First(x=>x.Id==npc).Name+" · "+(panel=="npc"?"Nói chuyện":panel=="buy"?"Mua hàng":panel=="sell"?"Bán đồ":panel=="store"?"Gửi rương":panel=="retrieve"?"Lấy rương":"Nhiệm vụ"):panel=="debug"?"DEBUG — mốc / reset":panel=="confirm"?"Reset sẽ xóa toàn phiên RAM":tabs.Contains(panel)?tabNames[Array.IndexOf(tabs,panel)]:ViewTitle);
            GUILayout.Label(Rpg?"Tab / Shift+Tab đổi tab · mũi tên chọn · Enter/E dùng · Esc quay lại":"↑/↓ chọn · Enter/E thực hiện · Esc quay lại");
            if(panel=="npc")GUILayout.Label("“"+(Host.LastDialogue??Dialogue(npc))+"”",GUILayout.Height(85));
            if(panel=="quest"||panel=="npc")GUILayout.Label(S.Objective,GUILayout.Height(48));
            if(panel=="item")GUILayout.Label(Description(SelectedItem),GUILayout.Height(200));
            if(panel=="equipped")GUILayout.Label(Description(P.Inventory.Equipment.TryGetValue(chosenSlot,out var equipped)?equipped:null),GUILayout.Height(190));
            if(panel=="attributes")GUILayout.Label($"Điểm chưa dùng: {P.Unspent}\nCông Lực STR {P.Str} · Sinh Lực VIT {P.Vit}\nLinh Lực INT {P.Int} · Thân Pháp AGI {P.Agi}",GUILayout.Height(75));
            if(panel=="stats")GUILayout.Label($"{(P.School==School.Novice?"Tân Lữ":"Kiếm")} · Lv {P.Level} · EXP {P.TotalExp}\nHP {P.Hp:F0}/{st.Hp:F0} · MP {P.Mp:F0}/{st.Mp:F0}\nATK {st.Atk:F1} · DEF {st.Def:F1}\nACC {st.Acc:F0} · EVA {st.Eva:F0}\nChí mạng {st.Crit:P1} · Tốc chạy nền {SliceHost.RunSpeed*st.Speed:F2} u/s ({st.Speed:P1})",GUILayout.Height(155));
            if(panel=="skills"){GUILayout.Label(skills);GUILayout.Label("Nội tại Lv5: "+(P.School==School.Sword?"MaxHP ×1,10 / DEF ×1,08":"Chọn phái trước")+"; Lv13 khóa.");}
            scroll=GUILayout.BeginScrollView(scroll);
            foreach(var action in actions.ToArray()){
                int index=actions.IndexOf(action);var old=GUI.backgroundColor;GUI.backgroundColor=index==selected?new Color(.64f,.8f,.42f):action.Enabled?new Color(.35f,.5f,.57f):Color.gray;
                if(GUILayout.Button((index==selected?"▶ ":"")+action.Label+(action.Enabled?"":" [khóa]"),GUILayout.Height(34))){selected=index;GUI.backgroundColor=old;Activate(action);GUIUtility.ExitGUI();}GUI.backgroundColor=old;
                if(index==selected&&!action.Enabled)GUILayout.Label(action.Reason);
            }
            GUILayout.EndScrollView();GUILayout.EndArea();
        }
    }
}
