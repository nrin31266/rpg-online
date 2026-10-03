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
        private readonly List<MenuAction> actions=new List<MenuAction>();
        private readonly Stack<(string panel,string action)> trail=new Stack<(string,string)>();
        private int selected;
        private Vector2 scroll;
        private Font font;
        private PrototypeStart pendingStart;
        private SliceSession S=>Host.Session;
        private Player P=>S.Player;
        private Item SelectedItem=>P.Inventory.Bag.FirstOrDefault(x=>x.Instance==itemInstance);
        private void Awake(){font=Resources.Load<Font>("Fonts/DejaVuSans");}
        public void Toggle(string name){if(panel==name)Close();else {Close();Open(name);}}
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
        public string Dialogue(string id){
            if(id=="Lam")return "Ra núi rồi, nhớ đường về. Chân vững hãy cầm kiếm.";
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
                    if(index<P.Inventory.Bag.Count){var i=P.Inventory.Bag[index];Add("item."+i.Instance,Catalog.Get(i.Id).Name,()=>{itemInstance=i.Instance;return Sub("item");});}
                    else Add("empty."+index,"Ô trống",()=>false,false,"Ô này chưa có vật phẩm.");
                }
            }else if(panel=="item")ItemActions();
            else if(panel=="equipment"){
                foreach(var slot in new[]{GearSlot.Weapon,GearSlot.Necklace,GearSlot.Armor,GearSlot.Ring,GearSlot.Pants,GearSlot.Boots}){
                    var value=slot;bool has=P.Inventory.Equipment.TryGetValue(value,out var i);Add("slot."+value,SlotName(value)+": "+(has?Catalog.Get(i.Id).Name:"Trống"),()=>{chosenSlot=value;return Sub("equipped");},has,"Slot trống; mặc đồ từ hành trang.");
                }
            }else if(panel=="equipped"){
                Add("unequip."+chosenSlot,"Tháo "+SlotName(chosenSlot)+" vào túi",()=>{bool ok=S.Unequip(chosenSlot);if(ok)Back();return ok;},P.Inventory.Equipment.ContainsKey(chosenSlot),"Không còn món trong slot hoặc túi đầy.");
                Add("open.bag","Mở hành trang",()=>Sub("bag"));
            }else if(panel=="stats"){
                Add("tab.equipment","Trang bị — hình nhân vật / sáu slot",()=>Sub("equipment"));
                foreach(string stat in new[]{"STR","VIT","INT","AGI"}){string value=stat;Add("allocate."+value,"+1 "+new Dictionary<string,string>{{"STR","Công Lực"},{"VIT","Sinh Lực"},{"INT","Linh Lực"},{"AGI","Thân Pháp"}}[value],()=>S.Allocate(value),P.Unspent>0,"Không còn điểm chưa dùng.");}
                Add("tab.skills","Kỹ năng và nội tại",()=>Sub("skills"));
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
        private void ItemActions(){
            var i=SelectedItem;if(i==null){Add("missing","Món đã dùng hoặc chuyển khỏi túi",()=>{Back();return true;});return;}
            var d=Catalog.Get(i.Id);
            if(d.Kind==ItemKind.Gear)Add("equip."+i.Instance,"Mặc "+d.Name,()=>{bool ok=S.Equip(i.Instance);if(ok)Back();return ok;},P.Level>=d.MinLevel&&(i.Id!="wood"||P.School==School.Novice)&&(i.Id!="sword1"||P.School==School.Sword),"Cần đúng cấp/phái; Mộc Kiếm chỉ dùng trước nhập phái.");
            else if(d.Kind==ItemKind.Manual)Add("learn."+i.Instance,"Học "+d.Name,()=>{bool ok=S.Learn(i.Instance);if(ok)Back();return ok;});
            else if(d.Kind==ItemKind.Food)Add("use."+i.Instance,"Dùng Food",()=>S.UseFood());
            else if(d.Kind==ItemKind.HpPotion||d.Kind==ItemKind.MpPotion)Add("use."+i.Instance,"Dùng bình",()=>S.Potion(d.Kind==ItemKind.HpPotion));
            else Add("info."+i.Instance,"Vật liệu — bán tại Bách Luyện / cất tại Mộc An",()=>true);
        }
        private void Npc(){
            if(npc==null)return;
            if(npc==S.QuestNpc&&!S.Complete){
                if(S.QuestState==QuestState.Available)Add("quest.accept","Nhận Q"+S.Quest,()=>S.AcceptQuest());
                if(S.QuestState==QuestState.Ready)Add("quest.turnin","Trả Q"+S.Quest,()=>S.TurnIn());

            }
            if(npc=="Phong")Add("class.sword","Nhập phái Kiếm",()=>S.ChooseSword(),S.Quest==6&&S.Stage==1&&P.School==School.Novice&&!P.Inventory.Equipment.ContainsKey(GearSlot.Weapon),P.School!=School.Novice?"Bạn đã nhập phái; không đổi phái trong mock.":S.Quest!=6||S.Stage!=1?"Cần đúng bước nhập môn Q6.":"Tháo Mộc Kiếm tại C → Trang bị trước.");
            if(npc=="Diep")Add("class.bow","Nhập phái Cung — chưa mở trong VS-1",()=>false,false,"Diệp Lam thuộc nhánh Cung; prototype này chỉ mở Kiếm.");
            if(npc=="Moc"){
                Add("rest","Nghỉ — hồi đầy HP/MP",()=>S.Rest());Add("service.store","Rương → Gửi đồ",()=>Sub("store"));Add("service.retrieve","Rương → Lấy đồ",()=>Sub("retrieve"));
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
        private void DrawButton(MenuAction action,Rect r){
            int index=actions.IndexOf(action);var old=GUI.backgroundColor;GUI.backgroundColor=index==selected?new Color(.64f,.8f,.42f):action.Enabled?new Color(.35f,.5f,.57f):Color.gray;
            if(GUI.Button(r,(index==selected?"▶ ":"")+action.Label+(action.Enabled?"":" [khóa]"))){selected=index;Activate(action);}GUI.backgroundColor=old;
        }
        private void DrawInventory(Rect area){
            GUI.Label(new Rect(area.x+20,area.y+12,500,25),"Hành trang · "+P.Inventory.Bag.Count+" / "+P.Inventory.Capacity);
            GUI.Label(new Rect(area.x+20,area.y+42,760,25),"↑↓←→ chọn ô · Enter/E mở thao tác · Esc quay lại · icon khối tạm");
            for(int i=0;i<P.Inventory.Capacity;i++){
                Rect cell=new Rect(area.x+20+(i%6)*62,area.y+85+(i/6)*62,56,56);var old=GUI.backgroundColor;GUI.backgroundColor=i==selected?new Color(.64f,.8f,.42f):new Color(.28f,.4f,.48f);
                if(GUI.Button(cell,"")){selected=i;Activate(actions[i]);}GUI.backgroundColor=old;
                Icon(cell,i<P.Inventory.Bag.Count?P.Inventory.Bag[i]:null);
            }
            Item item=selected<P.Inventory.Bag.Count?P.Inventory.Bag[selected]:null;
            GUI.Box(new Rect(area.x+435,area.y+85,area.width-455,335),"");GUI.Label(new Rect(area.x+450,area.y+100,area.width-485,310),Description(item));
            DrawButton(actions.Last(),new Rect(area.x+20,area.yMax-50,200,32));
        }
        private void DrawEquipment(Rect area){
            GUI.Label(new Rect(area.x+20,area.y+15,700,25),"Trang bị · Enter/E chọn slot để tháo · I: hành trang · Esc: quay lại");
            float cx=area.x+area.width/2,cy=area.y+220;
            Fill(new Rect(cx-19,cy-58,38,38),new Color(.8f,.67f,.5f));Fill(new Rect(cx-25,cy-15,50,65),P.Inventory.Equipment.ContainsKey(GearSlot.Armor)?new Color(.3f,.7f,.5f):new Color(.5f,.55f,.6f));
            Fill(new Rect(cx-24,cy+55,19,63),new Color(.25f,.45f,.48f));Fill(new Rect(cx+5,cy+55,19,63),new Color(.25f,.45f,.48f));
            if(P.Inventory.Equipment.ContainsKey(GearSlot.Weapon))Fill(new Rect(cx+33,cy-20,7,100),new Color(.75f,.87f,.83f));
            for(int i=0;i<6;i++){
                Rect r=new Rect(cx+(i%2==0?-340:155),area.y+90+(i/2)*95,185,85);var a=actions[i];DrawButton(a,r);
            }
            GUI.Label(new Rect(cx-55,area.y+370,120,26),P.School==School.Novice?"Tân Lữ":"Kiếm · Lv"+P.Level);DrawButton(actions.Last(),new Rect(area.x+20,area.yMax-50,200,32));
        }
        private void OnGUI(){
            if(Host==null||S==null)return;Refresh();GUI.skin.font=font;GUI.skin.label.fontSize=15;GUI.skin.label.wordWrap=true;GUI.skin.button.fontSize=14;GUI.skin.button.wordWrap=true;
            var st=P.Stats;
            GUI.Box(new Rect(8,8,325,180),"");GUILayout.BeginArea(new Rect(18,15,305,170));GUILayout.Label("Huyền Lộ · V6.2.3 · "+SliceHost.MapName(P.Map));
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if(GUILayout.Button("Debug / mốc / reset (F8)",GUILayout.Height(22)))Toggle("debug");
#endif
            GUILayout.Label($"Lv{P.Level} · {(P.School==School.Novice?"Tân Lữ":"Kiếm")} · Vàng {P.Gold}");GUILayout.Label($"HP {P.Hp:F0}/{st.Hp:F0} · MP {P.Mp:F0}/{st.Mp:F0}");GUILayout.Label($"EXP {P.TotalExp} · điểm {P.Unspent}");GUILayout.Label($"Food {(S.FoodUntil>S.Now?$"{S.FoodUntil-S.Now:F0}s":"chưa dùng")} · H:HP {Math.Max(0,S.HpPotionUntil-S.Now):F1}s · M:MP {Math.Max(0,S.MpPotionUntil-S.Now):F1}s");GUILayout.EndArea();
            GUI.Box(new Rect(Screen.width-340,8,332,165),"");GUI.Label(new Rect(Screen.width-330,16,310,105),S.Objective);GUI.Label(new Rect(Screen.width-330,125,310,43),"E: tương tác · I: túi · C: nhân vật · Q: quest");
            var focus=S.Combat.Focus;if(focus!=null){GUI.Box(new Rect(345,8,250,90),"");GUI.Label(new Rect(355,14,230,27),$"{focus.Name} Lv{focus.Level}");GUI.Label(new Rect(355,42,230,24),$"{focus.Hp:F0} / {focus.MaxHp:F0}");Fill(new Rect(355,70,230*(float)(focus.Hp/focus.MaxHp),10),Color.green);var pt=Host.ScreenPoint(new Point(focus.Position.X,focus.Position.Y+1));Fill(new Rect(pt.x-25,Screen.height-pt.y,50*(float)(focus.Hp/focus.MaxHp),5),Color.red);}
            GUI.Box(new Rect(8,Screen.height-76,Screen.width-16,68),"");GUI.Label(new Rect(18,Screen.height-69,Screen.width-36,25),S.Feedback);
            string skills=P.School==School.Novice?"1: Mộc Kiếm · 2/3 khóa":string.Join("   ",Enumerable.Range(1,3).Select(i=>S.Combat.Unlocked.TryGetValue(i,out var skill)?$"{i}: {skill.Name} CD{S.Combat.Remaining(skill):F1}":$"{i}: khóa"));GUI.Label(new Rect(18,Screen.height-43,Screen.width-36,27),skills+" · "+(S.DebugPreset==null?"Phiên RAM":"DEBUG "+S.DebugPreset));
            if(!Modal)return;float width=Mathf.Min(820,Screen.width-40),height=Mathf.Min(480,Screen.height-270);Rect area=new Rect((Screen.width-width)/2,190,width,height);Fill(area,new Color(.08f,.14f,.18f,1));GUI.Box(area,"");
            if(panel=="bag"){DrawInventory(area);return;}if(panel=="equipment"){DrawEquipment(area);return;}
            GUILayout.BeginArea(new Rect(area.x+20,area.y+12,area.width-40,area.height-24));
            GUILayout.Label(!P.Alive&&panel!="debug"&&panel!="confirm"?"Đã chết":npc!=null?SliceSession.Anchors.First(x=>x.Id==npc).Name+" · "+(panel=="npc"?"Nói chuyện":panel=="buy"?"Mua hàng":panel=="sell"?"Bán đồ":panel=="store"?"Gửi rương":panel=="retrieve"?"Lấy rương":"Nhiệm vụ"):panel=="debug"?"DEBUG — mốc / reset":panel=="confirm"?"Reset sẽ xóa toàn phiên RAM":panel=="stats"?"Nhân vật":panel=="item"?"Vật phẩm":"Nhiệm vụ / "+panel);
            GUILayout.Label("↑/↓, Tab: chọn · Enter/E: thực hiện · Esc: quay lại");
            if(panel=="npc")GUILayout.Label("“"+Dialogue(npc)+"”",GUILayout.Height(50));
            if(panel=="quest"||panel=="npc")GUILayout.Label(S.Objective,GUILayout.Height(48));
            if(panel=="item")GUILayout.Label(Description(SelectedItem),GUILayout.Height(200));
            if(panel=="equipped")GUILayout.Label(Description(P.Inventory.Equipment.TryGetValue(chosenSlot,out var equipped)?equipped:null),GUILayout.Height(190));
            if(panel=="stats"){GUILayout.Label($"Điểm {P.Unspent} · STR {P.Str} VIT {P.Vit} INT {P.Int} AGI {P.Agi}");GUILayout.Label($"ATK {st.Atk:F1} · DEF {st.Def:F1} · ACC {st.Acc:F0} · EVA {st.Eva:F0}\nChí mạng {st.Crit:P1} · tốc chạy {st.Speed:P1}");}
            if(panel=="skills"){GUILayout.Label(skills);GUILayout.Label("Nội tại Lv5: "+(P.School==School.Sword?"MaxHP ×1,10 / DEF ×1,08":"Chọn phái trước")+"; Lv13 khóa.");}
            scroll=GUILayout.BeginScrollView(scroll);
            foreach(var action in actions.ToArray()){
                int index=actions.IndexOf(action);var old=GUI.backgroundColor;GUI.backgroundColor=index==selected?new Color(.64f,.8f,.42f):action.Enabled?new Color(.35f,.5f,.57f):Color.gray;
                if(GUILayout.Button((index==selected?"▶ ":"")+action.Label+(action.Enabled?"":" [khóa]"),GUILayout.Height(34))){selected=index;Activate(action);}GUI.backgroundColor=old;
                if(index==selected&&!action.Enabled)GUILayout.Label(action.Reason);
            }
            GUILayout.EndScrollView();GUILayout.EndArea();
        }
    }
}
