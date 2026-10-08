# Huyền Lộ — Game Design

Các số BASELINE/TUNABLE chưa phải nghiệm thu runtime.

Game đích gồm cả Kiếm/Cung và online Dedicated + Spring/PostgreSQL. Tài liệu này sở hữu mục tiêu trải nghiệm, chương và scope; lore/objectives chi tiết tại [Quests & Narrative](quests-and-narrative.md). Trạng thái triển khai xem tại [Roadmap](../04-production/roadmap.md).

## Document owns

Identity, fantasy, pillars, core loop, phạm vi TARGET, mục tiêu trải nghiệm và tổng kết chương.

## Document does not own

Luật ngoài domain thuộc owner trong [documentation map](../README.md); evidence thuộc [Playtest & Balance](../04-production/playtest-and-balance.md), thứ tự triển khai thuộc [Roadmap](../04-production/roadmap.md).

<a id="vision"></a>
<a id="gdd-1"></a>
<a id="gdd-12"></a>

<a id="1-tầm-nhìn-và-phạm-vi"></a>

## Tầm nhìn và phạm vi

**Giới thiệu ngắn:** RPG hành động 2D online ngang trên PC / Unity. Tân Lữ khám phá linh mạch, chọn Kiếm / Cung, tự phân bốn thuộc tính, gom quái đánh lan, nâng gear, săn Linh Biến / Boss và tỷ thí.

**STRONG DIRECTION hình ảnh:** cổ phong võ hiệp/huyền huyễn Á Đông, lấy cảm hứng cảnh quan/văn hóa Việt Nam và tiên hiệp nhẹ, không khóa triều đại cụ thể hay thuần làng quê. Camera gameplay pure 2D orthographic side-view LOCKED; kiến trúc/motif và sprite-facing thuộc [Art owner](../03-art/art-and-visual-production.md#visual-perspective), không rewrite map/lore/quest.

| Trụ cột | Người chơi cảm nhận | Dấu hiệu đạt |
| --- | --- | --- |
| Farm có nhịp | Gom quái rồi cleave / pierce / spread / explosion | Lv 5 single; Lv 10 học tiến cảnh max 3; Lv 17 đại chiêu |
| Build tự do | Đổi phân phối điểm để thử cách chơi | All-in không bị khóa progression; Tẩy Mạch sửa build |
| Progression hữu hình | Gear mới đổi cả stat và hình | Weapon / Armor / Pants đổi sprite |
| Hai class khác nhau | Kiếm áp sát; Cung giữ khoảng cách | Range, hit shape và control khác rõ |
| Online có ý nghĩa | Co-op farm / Boss, chat, challenge | Client kết nối tham gia gameplay thật |
| Scope hoàn chỉnh | Ít nội dung nhưng nối thành hành trình | Lv 1 → 20, ba chương, chính tuyến và vòng chơi sau truyện |

**Vòng chơi chính:** nhiệm vụ chỉ đường → chuẩn bị Food/Bình → vùng đã mở → đánh đơn rồi gom cụm/đánh lan → EXP/đồ rơi → phân điểm/nâng đồ → mốc cấp → NPC. Linh Biến/Boss/PvP xen giữa các chặng farm; Food và túi đồ tạo nhịp về làng.

Mục tiêu tới Lv 20: 2,5–4 giờ gồm đi đường, nhiệm vụ, mua bán và chạy lại sau tử vong; chưa được kiểm bằng người chơi thật. **P0 acceptance: tối thiểu 2 concurrent players** (hai Client kết nối cùng Game Server); game online nhiều người, không đặt MaxPlayers = 2. Capacity 3–4+ concurrent players phải benchmark performance / network trước khi công bố.

## Phạm vi P0 / P1 / P2

| Hệ thống | P0 | P1 khi core ổn | P2 |
| --- | --- | --- | --- |
| Player / combat | Novice, hai class, bốn attributes / Tẩy Mạch, 3 active tích lũy + 2 nội tại / class, nhập môn / tiến cảnh qua bí kíp, đánh lan / Bỏng / Băng Hàn | Buff đề xuất: Chiến Ý / Ưng Nhãn Cường Hóa (P1; chưa khóa phím); DPS Meter | Cosmetic polish |
| World | Năm bãi, cụm quái, bảy loại quái / sáu rigs, Linh Biến modifier, Boss basic + ba pattern / Cuồng Mạch | Linh Giáp / Vỡ Thế | Hazard, Boss polish |
| RPG | 18 dòng trang bị / 6 ô / 3 bậc, phẩm chất, I +4 / II +6 / III +8, shop / đồ rơi / túi / kho, Food / Death, chuyển giao cường hóa | Sắp túi; khóa đồ; Bùa Hồi Thành | Mua lại, mở rộng túi |
| Story / UI | Q1–Q12 với Q9 nhánh optional; ba Stage Summary; Journey; controls / HUD | Quest arrow, chat history | Extra cosmetics |
| Online / data | Unity Dedicated Game Server + Spring Boot + PostgreSQL; acceptance tối thiểu 2 concurrent players, MapId, co-farm không Party, chat / banner, shared loot ownership, lưu tiến trình an toàn | Observer hide / show; triển khai Internet công khai | Performance polish |

P1 chỉ triển khai sau core và quyết định scope; proposal ở cuối tài liệu này chưa thuộc gameplay P0.

**DROP:** Guild / Trade / Pet / Mount / Crafting / Auction / FreePK; nhiều tiền tệ; Skill Rank; cường hóa vượt giới hạn từng bậc (III không vượt +8); Channel / Zone; world chat / hạ tầng MMO nhiều cụm máy chủ; nợ EXP; hút HP / MP; Decoy; ghép đá; Hương EXP; hệ kháng / yếu nguyên tố. Không thêm thuộc tính hoặc hiệu ứng ngẫu nhiên ngoài luật hiện hành.

**Phạm vi game đích:** P0 là game đầy đủ, gồm cả Kiếm/Cung và online Dedicated + Spring/PostgreSQL. Thứ tự triển khai không đổi genre, scope hoặc final acceptance. CURRENT/DEFERRED, phạm vi bản thử và gate mạng sớm nằm ở [Roadmap](../04-production/roadmap.md#target-current-deferred).

> **Thứ tự triển khai / ngân sách:** [Roadmap — VS-1 và mục tiêu quản lý](../04-production/roadmap.md#vs-1)

---


## Chương và completion

| Chương | Dải cấp | Sắc thái và diễn tiến | Checkpoint tổng kết |
| --- | --- | --- | --- |
| I — Dấu Nứt Vân Khê | 1–7 | Nhập môn & sinh tồn: làng còn yên, điềm lạ thoáng qua; tự cầm kiếm, dùng thuốc, rèn món đầu tiên | Q7 completed và Lv ≥ 7 |
| II — Theo Dấu Huyền Lộ | 8–17 | Dấn thân & khám phá: lần theo trọc khí; Lv 12 tu luyện ngoại vi Xích Nham, Lv 15 điều tra sâu, Lv 17 phục hồi Huyền Môn | Q11 completed và Lv ≥ 17 |
| III — Huyền Tích Thức Tỉnh | 18–20 khuyến nghị | Thanh tẩy & vấn đạo: phế tích trang nghiêm, Thủ Vệ bị cuồng hóa; phong ấn ổn định sau trận chiến | Q12 completed và Lv 20 |

**Main Story Complete — HOÀN THÀNH CHÍNH TUYẾN:** Q12 khép lại chính tuyến / Chương III. Summary: **CHƯƠNG III HOÀN THÀNH / CHÍNH TUYẾN ĐÃ HOÀN THÀNH** — Lâm Bá: “Tai ương tạm lắng. Đường phía trước còn dài.” Tiếp tục farm Huyền Tích, săn đồ Rare / Epic, nâng đồ III lên +8, săn Linh Biến / Dư Ảnh, tỷ thí và thử build; P1 chỉ có khi được triển khai.

<a id="online-social"></a>
<a id="gdd-9"></a>

<a id="8-online-pvp-và-giao-tiếp"></a>

## Online, PvP và giao tiếp

Online multiplayer theo mô hình Client–Server. Player đăng nhập bằng tài khoản do admin cấp, chọn nhân vật rồi kết nối Unity Dedicated Game Server; không có Register cho player. Game Server quyết định combat, quái, Boss và kết quả trận PvP; Spring Boot/PostgreSQL lưu tiến trình, tạm giữ và quyết toán Vàng cược. Client không gửi level / Vàng / EXP / gear / thuộc tính như dữ liệu đáng tin. **P0 acceptance: tối thiểu 2 concurrent players** — hai Client kết nối Game Server; đây là mức nghiệm thu, không phải giới hạn world. P0 chạy local/LAN; triển khai Internet công khai ngoài phạm vi P0.

MapId P0 kiểm combat / mob / map transition / loot / chat; khác map không tương tác, chỉ render khu hiện tại. Hide / show P1 không gate slice. Boss timer / banner global khi map rỗng; tên Cự Thú / Dư Ảnh theo character, không tạo entity khác.

World support N players; không hard-code hai slot / Player 1–Player 2. Threat / contribution, MapId / chat, quest assist và loot eligibility dùng collection theo playerId. PvP **1v1** là mode riêng, không giới hạn multiplayer world. Co-op xét N recipients theo contribution / eligibility và level factor các mục liên quan; kill / evidence credit các mục liên quan, shared loot windows tách khỏi credit.

**Không Party P0:** không create/invite/accept/leader/leave/kick, UI/HP bars/chat nhóm, EXP bonus, loot/quest sharing dành riêng nhóm hoặc raid. N người cùng farm chỉ cần tham gia đánh: contribution, threat, credit và quyền loot xét từng character; không membership container ở giữa. “Cụm quái” là bãi author, không tổ đội. Party chỉ xem xét sau P0, chưa có thiết kế cần code.

<a id="research-ideas"></a>
<a id="legacy-provenance"></a>

<a id="6-nguồn-tham-khảo-và-đề-xuất-p1"></a>

## Nguồn tham khảo và đề xuất P1

Các đề xuất dưới đây được giữ để không mất thiết kế đang cân nhắc. **P1 / PROPOSAL, chưa duyệt triển khai**, không cộng vào balance / acceptance P0. Chọn hoặc bỏ sau core gate theo [Game Design scope](#vision); không phải Open Decision chặn code P0.

| ID | Candidate | Input proposal được giữ lại |
| --- | --- | --- |
| SCOPE-03 | **Chiến Ý — Kiếm / Ưng Nhãn Cường Hóa — Cung** | Buff Lv 10, phím đề xuất còn OPEN; duration 10 s, skill CD 40 s, MP 15. Chiến Ý: +15% ATK, +5% speed. Ưng Nhãn Cường Hóa: +10 điểm % CritChance, +15% range. Không phải nội tại Ưng Nhãn Lv 5; không thêm active P0. Acquisition / stacking và exact values cần chốt nếu chọn P1. |
| SCOPE-04 | Linh Giáp / Vỡ Thế | Shield 1.000 và Groggy là proposal cũ; chưa có shield-break / CC contract P0. Cần kiểm lại Boss / CC / workload nếu chọn; không tự lấy số này làm Boss data. |



<a id="gdd-0"></a>
