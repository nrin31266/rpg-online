using System.Linq;
using HuyenLo.Domain;
using UnityEngine;

namespace HuyenLo.Runtime
{
    // Probe UI dispatches domain commands; it owns no HP, quest, inventory or cooldown mutations.
    public sealed class SliceHud : MonoBehaviour
    {
        public SliceHost Host;
        public bool Modal => panel!=null;
        private string panel,npc;
        private Vector2 scroll;
        private Font font;
        private SliceSession S=>Host.Session;
        private Player P=>S.Player;
        private void Awake(){font=Resources.Load<Font>("Fonts/DejaVuSans");}
        public void Toggle(string name){if(panel==name)Close();else {panel=name;npc=null;S.Combat.CancelIntent();}}
        public void OpenNpc(string id){panel="npc";npc=id;scroll=Vector2.zero;S.Combat.CancelIntent();}
        public void Close(){panel=npc=null;S.Combat.CancelIntent();}
        public bool IsOverUi(Vector2 screen){
            float y=Screen.height-screen.y;
            return Modal||y>Screen.height-70||(y<165&&screen.x<333)||(y<153&&screen.x>Screen.width-330)||
                (S.Combat.Focus!=null&&y<100&&screen.x>=345&&screen.x<=595);
        }
        private bool Button(string text)=>GUILayout.Button(text,GUILayout.Height(28));
        private void OnGUI() {
            if(Host==null||S==null)return;GUI.skin.font=font;GUI.skin.label.fontSize=15;GUI.skin.button.fontSize=14;
            var st=P.Stats;
            GUI.Box(new Rect(8,8,325,155),"");GUILayout.BeginArea(new Rect(18,15,305,145));
            GUILayout.Label($"Huyền Lộ · VS-1 · {SliceHost.MapName(P.Map)}");
            GUILayout.Label($"Lv {P.Level} · {(P.School==School.Novice?"Tân Lữ":"Kiếm")} · Vàng {P.Gold}");
            GUILayout.Label($"HP {P.Hp:F0}/{st.Hp:F0}   MP {P.Mp:F0}/{st.Mp:F0}");
            GUILayout.Label($"EXP {P.TotalExp}/{Rules.Exp[System.Math.Min(P.Level,19)]}   Túi {P.Inventory.Bag.Count}/30");
            GUILayout.Label(S.FoodUntil>S.Now?$"Food còn {S.FoodUntil-S.Now:F0}s":"Không Food · không hồi tự nhiên");GUILayout.EndArea();
            GUI.Box(new Rect(Screen.width-330,8,322,145),"");GUILayout.BeginArea(new Rect(Screen.width-318,16,300,130));GUILayout.Label(S.Objective);GUILayout.Label("B túi · C điểm · K skill · L quest · Esc đóng");GUILayout.Label("A/← D/→ · Space nhảy · S+Space xuống");GUILayout.Label("E tương tác · F Food · H máu · M linh lực");GUILayout.EndArea();
            var focus=S.Combat.Focus;
            if(focus!=null){GUI.Box(new Rect(345,8,250,90),"");GUI.Label(new Rect(355,14,230,27),$"{focus.Name} Lv{focus.Level} ({S.Combat.FocusKind})");GUI.Label(new Rect(355,42,230,24),$"{focus.Hp:F0} / {focus.MaxHp:F0}");GUI.color=new Color(.2f,.25f,.3f);GUI.DrawTexture(new Rect(355,70,230,10),Texture2D.whiteTexture);GUI.color=Color.green;GUI.DrawTexture(new Rect(355,70,230*(float)(focus.Hp/focus.MaxHp),10),Texture2D.whiteTexture);GUI.color=Color.white;}
            foreach(var m in S.Mobs.Where(x=>x.Map==P.Map&&x.Alive))if(m==focus){var pt=Host.ScreenPoint(new Point(m.Position.X,m.Position.Y+1));GUI.color=Color.red;GUI.DrawTexture(new Rect(pt.x-25,Screen.height-pt.y,50*(float)(m.Hp/m.MaxHp),5),Texture2D.whiteTexture);GUI.color=Color.white;}
            GUI.Box(new Rect(8,Screen.height-68,Screen.width-16,60),"");GUI.Label(new Rect(18,Screen.height-61,Screen.width-36,24),S.Feedback);
            string skills=P.School==School.Novice?"J: Mộc Kiếm (cần mặc)":string.Join("   ",Enumerable.Range(1,3).Select(i=>S.Combat.Unlocked.TryGetValue(i,out var skill)?$"{i}{(S.Combat.SelectedSlot==i?"*":"")}: {skill.Name} MP{skill.Mp} CD{S.Combat.Remaining(skill):F1}":$"{i}: Khóa — gate sau"));
            GUI.Label(new Rect(18,Screen.height-35,Screen.width-36,24),skills+"   · Local RAM, thoát sẽ mất tiến trình.");
            if(!P.Alive){GUI.Box(new Rect(350,180,340,160),"Đã chết");GUILayout.BeginArea(new Rect(365,210,310,120));if(Button("Về Vân Khê — miễn phí")){Close();S.Revive(true);}if(Button("Dùng Hồi Sinh Phù — 50% HP/MP")){Close();S.Revive(false);}GUILayout.EndArea();return;}
            if(panel==null)return;
            GUI.Box(new Rect(25,165,510,Screen.height-245),"VS-1 / "+panel);
            GUILayout.BeginArea(new Rect(40,193,480,Screen.height-286));scroll=GUILayout.BeginScrollView(scroll);
            if(Button("Đóng (Esc)")){Close();GUILayout.EndScrollView();GUILayout.EndArea();return;}
            if(panel=="bag")Bag();else if(panel=="stats")Stats();else if(panel=="skills")Skills();else if(panel=="quest")Quest();else Npc();
            GUILayout.EndScrollView();GUILayout.EndArea();
        }
        private void Bag() {
            GUILayout.Label("Trang bị đang mặc:");
            foreach(var e in P.Inventory.Equipment.ToArray()){GUILayout.BeginHorizontal();GUILayout.Label(e.Key+": "+Catalog.Get(e.Value.Id).Name);if(Button("Tháo"))S.Unequip(e.Key);GUILayout.EndHorizontal();}
            GUILayout.Label("Hành trang (gear 1 / stack 99):");
            foreach(var i in P.Inventory.Bag.ToArray()){
                var d=Catalog.Get(i.Id);GUILayout.BeginHorizontal();GUILayout.Label($"{d.Name} x{i.Count}{(i.Binding!=null?" ["+i.Binding+"]":"")} q{i.Quality:F2}");
                if(d.Kind==ItemKind.Gear&&Button("Mặc"))S.Equip(i.Instance);
                if(d.Kind==ItemKind.Manual&&Button("Học"))S.Learn(i.Instance);
                GUILayout.EndHorizontal();
            }
        }
        private void Stats() {
            GUILayout.Label($"Chưa dùng {P.Unspent} / Đã cộng {P.Allocated}");
            foreach(var pair in new[]{("STR",P.Str,"Công Lực"),("VIT",P.Vit,"Sinh Lực"),("INT",P.Int,"Linh Lực"),("AGI",P.Agi,"Thân Pháp")}){
                GUILayout.BeginHorizontal();GUILayout.Label(pair.Item3+": "+pair.Item2);if(Button("+1"))S.Allocate(pair.Item1);GUILayout.EndHorizontal();}
            var s=P.Stats;GUILayout.Label($"ATK {s.Atk:F1} / DEF {s.Def:F1} / ACC {s.Acc:F0} / EVA {s.Eva:F0}\nCrit {s.Crit:P1} / tốc chạy {s.Speed:P1}");
        }
        private void Skills() {
            GUILayout.Label("1/2/3 chọn + thử cast ngay. J dùng slot đã chọn.");
            foreach(int slot in Enumerable.Range(1,3))GUILayout.Label(S.Combat.Unlocked.TryGetValue(slot,out var skill)?$"{slot} — {skill.Name}; MP{skill.Mp}, CD{skill.Cooldown}s, giữ lặp: {skill.Repeat}":$"{slot} — khóa (Lv{(slot==1?5:slot==2?10:17)} / bí kíp / quest)");
            GUILayout.Label(P.School==School.Sword?"Nội tại Lv5: MaxHP ×1,10 / DEF ×1,08 (đang mở)":"Nội tại Lv5: cần chọn phái");GUILayout.Label("Nội tại Lv13: khóa, không thuộc VS-1.");
        }
        private void Quest(){GUILayout.Label(S.Objective);GUILayout.Label("Nhận và trả tại đúng NPC. Các stage/receipt chỉ giữ trong RAM. Q7–Q12 nằm ngoài bản thử này.");}
        private void Npc() {
            if(npc==null)return;
            GUILayout.Label(SliceSession.Anchors.First(x=>x.Id==npc).Name);
            if(npc==S.QuestNpc && !S.Complete){
                GUILayout.Label(S.Objective);
                if(S.QuestState==QuestState.Available&&Button("Nhận Q"+S.Quest))S.AcceptQuest();
                if(S.QuestState==QuestState.Ready&&Button("Trả Q"+S.Quest))S.TurnIn();
            }
            if(npc=="ClassHall"){
                if(Button("Kiếm — chơi được trong VS-1"))S.ChooseSword();GUILayout.Label("Cung — Chưa mở trong bản thử nghiệm.");
            }
            if(npc=="Moc"){
                if(Button("Nghỉ — hồi đầy HP/MP"))S.Rest();
                GUILayout.Label($"Rương {P.Storage.Bag.Count}/40 · chuyển cả stack · cùng character RAM");
                foreach(var i in P.Inventory.Bag.ToArray())if(Button("Cất "+Catalog.Get(i.Id).Name))S.Store(i.Instance,true);
                foreach(var i in P.Storage.Bag.ToArray())if(Button("Lấy "+Catalog.Get(i.Id).Name))S.Store(i.Instance,false);
            }
            if(npc=="Yen"||npc=="Bach"){
                var ids=npc=="Yen"?new[]{"food1","hp1","mp1","scroll"}:new[]{"sword1","armor1","pants1","boots1","ring1","neck1","stone"};
                foreach(string id in ids){var d=Catalog.Get(id);if(Button($"Mua {d.Name} ({d.Buy} Vàng)"))S.Buy(id,npc);}
                if(npc=="Bach")foreach(var i in P.Inventory.Bag.Where(x=>Catalog.Get(x.Id).Sell>0).ToArray())if(Button($"Bán 1 {Catalog.Get(i.Id).Name}{(i.Binding!=null?" ["+i.Binding+"]":"")}"))S.Sell(i.Instance);
            }
        }
    }
}
