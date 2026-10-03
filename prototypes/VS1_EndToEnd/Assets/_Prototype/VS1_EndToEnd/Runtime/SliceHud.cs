using System;
using System.Collections.Generic;
using System.Linq;
using HuyenLo.Domain;
using UnityEngine;

namespace HuyenLo.Runtime
{
    public sealed class MenuAction
    {
        public string Id,Label,Reason;
        public bool Enabled;
        public Func<bool> Execute;
    }
    // Both keyboard and mouse dispatch these same domain commands.
    public sealed class SliceHud : MonoBehaviour
    {
        public SliceHost Host;
        public bool Modal => panel!=null||Host!=null&&!Host.Session.Player.Alive;
        public string Panel => panel;
        public string SelectedActionId => actions.Count==0?null:actions[Mathf.Clamp(selected,0,actions.Count-1)].Id;
        public IReadOnlyList<MenuAction> Actions {get{Refresh();return actions;}}
        private string panel,npc;
        private readonly List<MenuAction> actions=new List<MenuAction>();
        private int selected;
        private Vector2 scroll;
        private Font font;
        private PrototypeStart pendingStart;
        private SliceSession S=>Host.Session;
        private Player P=>S.Player;
        private void Awake(){font=Resources.Load<Font>("Fonts/DejaVuSans");}
        public void Toggle(string name){if(panel==name)Close();else Open(name);}
        private void Open(string name){panel=name;npc=null;selected=0;actions.Clear();scroll=Vector2.zero;S.Combat.CancelIntent();Refresh();}
        public void OpenNpc(string id){Open("npc");npc=id;actions.Clear();selected=0;Refresh();}
        public void Close(){panel=npc=null;selected=0;actions.Clear();S.Combat.CancelIntent();}
        public bool IsOverUi(Vector2 screen){float y=Screen.height-screen.y;return Modal||y>Screen.height-78||(y<190&&screen.x<333)||(y<174&&screen.x>Screen.width-345)||(S.Combat.Focus!=null&&y<100&&screen.x>=345&&screen.x<=595);}
        public void Navigate(int delta){Refresh();if(actions.Count==0)return;selected=(selected+delta+actions.Count)%actions.Count;scroll.y=Mathf.Max(0,(selected-6)*36);}
        public bool SelectAction(string id){Refresh();int index=actions.FindIndex(x=>x.Id==id);if(index<0)return false;selected=index;return true;}
        public bool ActivateSelected(){Refresh();return actions.Count>0&&Activate(actions[selected]);}
        private bool Activate(MenuAction action){
            if(!action.Enabled){S.Feedback=action.Reason??"Thao tác chưa hợp lệ.";return false;}
            bool result=action.Execute();if(!result)S.Feedback=action.Reason??"Không thể thực hiện. Kiểm tra vị trí, túi và điều kiện.";
            Refresh();return result;
        }
        private void Add(string id,string label,Func<bool> execute,bool enabled=true,string reason=null)=>actions.Add(new MenuAction{Id=id,Label=label,Execute=execute,Enabled=enabled,Reason=reason});
        private void Refresh(){
            if(Host==null||S==null)return;
            string previous=SelectedActionId;actions.Clear();
            if(!P.Alive&&panel!="debug"&&panel!="confirm"){Add("revive.village","Về Vân Khê — miễn phí",()=>{Close();return S.Revive(true);});Add("revive.scroll","Dùng phù — 50% HP/MP",()=>{bool ok=S.Revive(false);if(ok)Close();return ok;},P.Inventory.Count("scroll")>0,"Không có Hồi Sinh Phù.");}
            else if(panel=="bag")Bag();else if(panel=="stats")Stats();else if(panel=="skills")Add("tab.stats","← Thuộc tính",()=>{Open("stats");return true;});
            else if(panel=="npc")Npc();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            else if(panel=="debug"){
                Add("debug.resume","Tiếp tục phiên hiện tại",()=>{Close();return true;});
                foreach(PrototypeStart start in Enum.GetValues(typeof(PrototypeStart))){var value=start;string name=new[]{"Bắt đầu mới / reset","Q2 — movement","Q3 — Dummy","Q4 — Nấm / loot","Q5 — Sói / Food","Q6 — nhập môn","Sân tập Kiếm Lv5"}[(int)value-1];Add("debug."+value,name,()=>{pendingStart=value;Open("confirm");return true;});}
            }else if(panel=="confirm"){
                Add("debug.cancel","Hủy — giữ phiên hiện tại",()=>{Open("debug");return true;});
                Add("debug.apply","Reset toàn phiên và mở mốc "+pendingStart,()=>{Host.ResetPrototype(pendingStart);Close();return true;});
            }
#endif
            if(panel!=null)Add("close","Đóng (Esc)",()=>{Close();return true;});
            int same=actions.FindIndex(x=>x.Id==previous);selected=same>=0?same:Mathf.Clamp(selected,0,Math.Max(0,actions.Count-1));
        }
        private void Bag(){
            foreach(var e in P.Inventory.Equipment.ToArray()){var slot=e.Key;Add("unequip."+slot,"Tháo "+Catalog.Get(e.Value.Id).Name,()=>S.Unequip(slot));}
            foreach(var item in P.Inventory.Bag.ToArray()){
                var i=item;var d=Catalog.Get(i.Id);string quality=d.Kind==ItemKind.Gear?(i.Quality>=1.16?" · Rare":i.Quality>=1.08?" · Uncommon":" · Common"):"";
                string text=$"{d.Name} ×{i.Count}{quality}{(i.Binding==null?"":" [hướng dẫn]")}";
                if(d.Kind==ItemKind.Gear)Add("equip."+i.Instance,"Mặc "+text,()=>S.Equip(i.Instance));
                else if(d.Kind==ItemKind.Manual)Add("learn."+i.Instance,"Học "+text,()=>S.Learn(i.Instance));
                else if(d.Kind==ItemKind.Food)Add("use."+i.Instance,"Dùng "+text,()=>S.UseFood());
                else if(d.Kind==ItemKind.HpPotion||d.Kind==ItemKind.MpPotion)Add("use."+i.Instance,"Dùng "+text,()=>S.Potion(d.Kind==ItemKind.HpPotion));
                else Add("info."+i.Instance,text,()=>{S.Feedback="Vật phẩm trong túi; bán tại Bách Luyện hoặc cất tại Mộc An.";return true;});
            }
        }
        private void Stats(){
            foreach(string stat in new[]{"STR","VIT","INT","AGI"}){string value=stat;Add("allocate."+value,"+1 "+new Dictionary<string,string>{{"STR","Công Lực"},{"VIT","Sinh Lực"},{"INT","Linh Lực"},{"AGI","Thân Pháp"}}[value],()=>S.Allocate(value),P.Unspent>0,"Không còn điểm chưa dùng.");}
            Add("tab.skills","Kỹ năng và nội tại →",()=>{Open("skills");return true;});
        }
        private void Npc(){
            if(npc==null)return;
            if(npc==S.QuestNpc&&!S.Complete){
                if(S.QuestState==QuestState.Available)Add("quest.accept","Nhận Q"+S.Quest,()=>S.AcceptQuest());
                if(S.QuestState==QuestState.Ready)Add("quest.turnin","Trả Q"+S.Quest,()=>S.TurnIn());
            }
            if(npc=="ClassHall")Add("class.sword","Chọn Kiếm — Cung chưa mở trong mock",()=>S.ChooseSword(),S.Quest==6&&S.Stage==1,"Cần đúng bước nhập môn Q6.");
            if(npc=="Moc"){
                Add("rest","Nghỉ — hồi đầy HP/MP",()=>S.Rest());
                foreach(var item in P.Inventory.Bag.ToArray()){var i=item;Add("store."+i.Instance,"Cất "+Catalog.Get(i.Id).Name+" ×"+i.Count,()=>S.Store(i.Instance,true));}
                foreach(var item in P.Storage.Bag.ToArray()){var i=item;Add("retrieve."+i.Instance,"Lấy "+Catalog.Get(i.Id).Name+" ×"+i.Count,()=>S.Store(i.Instance,false));}
            }
            if(npc=="Yen"||npc=="Bach"){
                // Quest sample first, so accepting and selling need no long keyboard traversal.
                if(npc=="Bach")foreach(var item in P.Inventory.Bag.Where(x=>Catalog.Get(x.Id).Sell>0).OrderByDescending(x=>x.Binding=="Q4").ToArray()){
                    var i=item;Add("sell."+i.Instance,"Bán 1 "+Catalog.Get(i.Id).Name,()=>S.Sell(i.Instance),i.Binding==null||S.Quest==4&&S.Stage==3&&i.Id=="mushroom","Đồ hướng dẫn chưa được phép bán.");
                }
                foreach(string id in npc=="Yen"?new[]{"food1","hp1","mp1","scroll"}:new[]{"sword1","armor1","pants1","boots1","ring1","neck1","stone"}){
                    var key=id;var d=Catalog.Get(id);Add("buy."+id,$"Mua {d.Name} — {d.Buy} Vàng",()=>S.Buy(key,npc),P.Gold>=d.Buy,"Không đủ Vàng hoặc túi đầy.");
                }
            }
        }
        private void OnGUI(){
            if(Host==null||S==null)return;Refresh();GUI.skin.font=font;GUI.skin.label.fontSize=15;GUI.skin.button.fontSize=14;
            var st=P.Stats;
            GUI.Box(new Rect(8,8,325,180),"");GUILayout.BeginArea(new Rect(18,15,305,170));
            GUILayout.Label("Huyền Lộ · mẫu V6.2.2 · "+SliceHost.MapName(P.Map));
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if(GUILayout.Button("Debug / mốc / reset (F8)",GUILayout.Height(22)))Toggle("debug");
#endif
            GUILayout.Label($"Lv{P.Level} · {(P.School==School.Novice?"Tân Lữ":"Kiếm")} · Vàng {P.Gold}");GUILayout.Label($"HP {P.Hp:F0}/{st.Hp:F0}   MP {P.Mp:F0}/{st.Mp:F0}");GUILayout.Label($"EXP {P.TotalExp}/{Rules.Exp[Math.Min(P.Level,19)]} · Túi {P.Inventory.Bag.Count}/30");GUILayout.Label(S.FoodUntil>S.Now?$"Food {S.FoodUntil-S.Now:F0}s":"Không Food · không hồi tự nhiên");GUILayout.EndArea();
            GUI.Box(new Rect(Screen.width-345,8,337,166),"");GUILayout.BeginArea(new Rect(Screen.width-334,16,315,151));GUILayout.Label(S.Objective);GUILayout.Label("I túi · C nhân vật · Q quest · F8 debug");GUILayout.Label("Space/↑ nhảy · S/↓ xuống sàn");GUILayout.Label("E tương tác · F Food · H máu · M linh lực");GUILayout.EndArea();
            var focus=S.Combat.Focus;if(focus!=null){GUI.Box(new Rect(345,8,250,90),"");GUI.Label(new Rect(355,14,230,27),$"{focus.Name} Lv{focus.Level} ({S.Combat.FocusKind})");GUI.Label(new Rect(355,42,230,24),$"{focus.Hp:F0} / {focus.MaxHp:F0}");GUI.color=Color.green;GUI.DrawTexture(new Rect(355,70,230*(float)(focus.Hp/focus.MaxHp),10),Texture2D.whiteTexture);GUI.color=Color.white;var pt=Host.ScreenPoint(new Point(focus.Position.X,focus.Position.Y+1));GUI.color=Color.red;GUI.DrawTexture(new Rect(pt.x-25,Screen.height-pt.y,50*(float)(focus.Hp/focus.MaxHp),5),Texture2D.whiteTexture);GUI.color=Color.white;}
            GUI.Box(new Rect(8,Screen.height-76,Screen.width-16,68),"");GUI.Label(new Rect(18,Screen.height-69,Screen.width-36,25),S.Feedback);
            string skills=P.School==School.Novice?"1: Mộc Kiếm · 2/3 khóa":string.Join("   ",Enumerable.Range(1,3).Select(i=>S.Combat.Unlocked.TryGetValue(i,out var skill)?$"{i}: {skill.Name} CD{S.Combat.Remaining(skill):F1}":$"{i}: khóa"));
            GUI.Label(new Rect(18,Screen.height-43,Screen.width-36,27),skills+" · Mỗi lần bấm = một đòn · "+(S.DebugPreset==null?"Phiên mới, RAM":"DEBUG "+S.DebugPreset));
            if(!Modal)return;
            float height=Mathf.Max(180,Screen.height-285);GUI.Box(new Rect(28,182,530,height),"");GUILayout.BeginArea(new Rect(42,190,502,height-16));
            GUILayout.Label(!P.Alive&&panel!="debug"&&panel!="confirm"?"Đã chết":panel=="npc"?SliceSession.Anchors.First(x=>x.Id==npc).Name:panel=="debug"?"DEBUG — chọn mốc hoặc reset":panel=="confirm"?"Reset sẽ xóa toàn bộ trạng thái RAM":panel=="bag"?"Hành trang":panel=="stats"?"Nhân vật":panel=="skills"?"Kỹ năng":"Nhiệm vụ");
            GUILayout.Label("↑/↓ hoặc W/S, Tab: chọn · Enter/E: thực hiện · Esc: đóng");
            if(panel=="quest"||panel=="npc")GUILayout.Label(S.Objective);
            if(panel=="stats"){GUILayout.Label($"Điểm: {P.Unspent} chưa dùng · STR {P.Str} VIT {P.Vit} INT {P.Int} AGI {P.Agi}");GUILayout.Label($"ATK {st.Atk:F1} · DEF {st.Def:F1} · ACC {st.Acc:F0} · EVA {st.Eva:F0}\nChí mạng {st.Crit:P1} · tốc chạy {st.Speed:P1}");}
            if(panel=="skills"){GUILayout.Label(skills);GUILayout.Label("Nội tại Lv5: "+(P.School==School.Sword?"MaxHP ×1,10 / DEF ×1,08":"Chọn phái trước")+"; Lv13 chưa mở.");}
            scroll=GUILayout.BeginScrollView(scroll);
            foreach(var action in actions.ToArray()){
                int index=actions.IndexOf(action);var old=GUI.backgroundColor;GUI.backgroundColor=index==selected?new Color(.65f,.8f,.25f):action.Enabled?new Color(.35f,.55f,.6f):Color.gray;
                if(GUILayout.Button((index==selected?"▶ ":"  ")+action.Label+(action.Enabled?"":" [chưa sẵn]"),GUILayout.Height(32))){selected=index;Activate(action);}
                GUI.backgroundColor=old;
            }
            GUILayout.EndScrollView();GUILayout.EndArea();
        }
    }
}
