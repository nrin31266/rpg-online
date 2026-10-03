# Huyền Lộ — Roadmap triển khai và trạng thái hiện tại

**Ngày đồng bộ:** 2026-10-03 · **Trạng thái:** trước triển khai; chưa có playable build hoặc gate runtime đã pass.

File này sở hữu **thứ tự làm, CURRENT/DEFERRED và điều kiện mở production**. [GDD](1_HUYEN_LO_GDD.md) giữ game đích; [Technical](2_HUYEN_LO_TECHNICAL.md) giữ cách chạy/tích hợp; [Analysis](3_HUYEN_LO_DESIGN_ANALYSIS.md) giữ evidence/quyết định; [Art](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md) giữ chi tiết hình ảnh và production. Không dùng roadmap để sửa luật, duyệt proposal Art hoặc thu nhỏ nghiệm thu cuối.

<a id="target-current-deferred"></a>

# 1. TARGET, CURRENT và DEFERRED

**TARGET** là toàn bộ P0 hiện hành: Tân Lữ, Kiếm và Cung, hành trình Lv 1–20/Q1–Q12 (Q9 optional), gear/kinh tế, năm farm maps và ba support zones, Linh Biến/Boss, co-farm/chat/PvP; Unity Client + Dedicated Game Server + Spring Boot + PostgreSQL, Login/Character Select/persistence/recovery. Nghiệm thu cuối vẫn tối thiểu hai client đồng thời với backend/DB thật theo [GDD §10](1_HUYEN_LO_GDD.md#acceptance-routing).

**CURRENT** là chuẩn bị và validate Kiếm bằng VS-1 local, sau đó kiểm chuyển authority sang Dedicated với hai client. `Offline/local-first` là cách triển khai để kiểm sớm, không đổi game đích thành single-player. `CURRENT` không có nghĩa đã code hoặc đã pass.

**DEFERRED** là phần đã thiết kế và vẫn phải làm trong TARGET, nhưng chưa nằm trên đường phụ thuộc đầu tiên. Cung thuộc TARGET P0, **không phải feature P1**. Mọi bảng Cung, projectile, gear, pose/VFX và balance được giữ. Q6/lore vẫn giới thiệu hai phái; trong bản thử sớm, Kiếm chơi được, Cung ghi “Chưa mở trong bản thử nghiệm”. Không dùng nhãn này trong sản phẩm cuối.

| Nhóm | Trạng thái triển khai hiện tại | Điểm quay lại / điều kiện |
| --- | --- | --- |
| Tân Lữ → Kiếm, movement, normal/Lv 5 active, nội tại nền tảng | CURRENT, trong VS-1 | G-L: hành trình Q1–Q6 chơi được |
| Vân Khê, Học Viện, Đồng Sương; Nấm/Sói/Dummy; UI liên quan Q1–Q6 | CURRENT | Art/room nhỏ đủ kiểm gameplay; chưa sản xuất cả ba families |
| Kiếm Lv 10/13/17 và các bãi/Q7–Q12 | DEFERRED khỏi VS-1, TARGET P0 giữ nguyên | G-N + G-D; có thể dùng fixture hẹp để kiểm kỹ thuật trước hành trình đầy đủ |
| Cung: chọn/chơi, mọi skill/gear/projectile/pose/balance | DEFERRED IMPLEMENTATION, TARGET P0 | Sau G-N; giải A01/A02/A05/A14 bằng prototype Cung trước nhân toàn bộ gear |
| Dedicated Server và ít nhất hai clients | Gate kế tiếp ngay sau G-L | G-N, bắt buộc trước production rộng |
| Login/Character Select, Spring/PostgreSQL, ticket/lease/checkpoint/reconnect | DEFERRED khỏi local slice, TARGET P0 | G-D sau spike mạng; chưa có DB thật không được gọi final online acceptance |
| Chat/co-farm/claim shared, Linh Biến/Q8 | DEFERRED khỏi VS-1 | G-N chứng minh recipient/MapId; G-D chứng minh commit; G-C mở content |
| Boss/full online visual load | DEFERRED prototype và production | G-C; giữ các luật Boss đã có, giải A16/camera/telegraph ở G-F |
| PvP/Q9/escrow/settlement | DEFERRED khỏi VS-1, TARGET P0 | G-D trước G-P; Q9 optional cho người chơi không có nghĩa được bỏ hệ PvP |
| Buff R, shield/groggy, QoL/P1/P2 | Chưa duyệt triển khai | Chỉ xét sau core; proposal vẫn ở Analysis, không chen vào VS-1 |

<a id="vs-1"></a>

# 2. VS-1 — lát cắt đầu tiên phải chơi được

**CURRENT ROADMAP BASELINE:** Q1–Q6, Tân Lữ → Kiếm, ba map **Vân Khê / Học Viện / Đồng Sương** là phạm vi đang dùng để bắt đầu triển khai. Dùng layout/marker/spawn của GDD cho phần có mặt; Trúc Ảnh mở trong quest state sau Q6 nhưng nằm ngoài build thử, portal cần báo phạm vi bản thử thay vì giả đã có map. Theo baseline này, VS-1 không thêm Q7/enhance để thay mục tiêu đang kiểm.

Hướng triển khai đã xác nhận là **Kiếm trước → local/offline trước → gate mạng sớm**. Q1–Q6/ba map là baseline roadmap được dùng hiện tại, có thể thu/phình sau blockout nếu evidence cho thấy scope chưa hợp lý.

Khi review thay phạm vi slice, cập nhật mục này và phần G-L/validation liên quan; không âm thầm đổi quest/map trong GDD, dependency gate mạng hoặc final acceptance.

Một đường đi liên tục: nói chuyện NPC → nhảy/drop-through/portal thật → nhận/mặc Mộc Kiếm và hạ Dummy → mua/dùng Food/Bình, đánh Sói → nhặt supply/mặc Áo/bán sample → chọn Kiếm, cộng điểm, nhận/học bí kíp, cast Lv 5 và dùng M → trả Q6. Hiển thị Cung trong lời giới thiệu/lựa chọn nhưng chưa cho chọn vào branch chưa chạy được. Mộc/default outfit, Quần/Áo I và Kiếm I phải đọc được thay đổi; đồ II/III dùng fixture hẹp nếu cần kiểm reuse, không thành route mới.

| Cần có trong slice | Evidence G-L cần giữ |
| --- | --- |
| Local Session authoritative, intent/result/UI tách biệt | Một input chỉ qua resolver một lần; UI/VFX không tự sửa HP/túi/quest. Clock/life/action IDs có log |
| Movement/jump/drop-through, ba map và camera | Chân/collider/surface khớp; portal/MapId kiểm thật; ngã Q2 có thể làm lại; hướng trái/phải và cao độ |
| Normal Tân Lữ/Kiếm, active nhập môn, quái melee | Hit đúng clock/front/vertical band; miss khi ra khỏi vùng; nhận damage không tự stun; lethal/cancel/death đúng life |
| EXP/điểm/reset Lv 5, item/equip/loot/sell, Food/HP/MP Potion/death | Runtime áp luật GDD cho phần đã hiện diện; full bag/reject/đầy HP không tiêu bình; restart fixture không giả lưu bền |
| Q1–Q6, NPC/modal/skill/quest/HUD | Đi qua toàn hành trình và thử late/retry/staged grant/full-bag; đóng text/modal không lọt attack. Quest state và reward có result/receipt trong adapter local |
| Art/combat room nhỏ đã nhập Unity | Default + outfit I, Mộc/Kiếm I và một visual khác bằng fixture; Dummy/Nấm/Sói, impact/Death, terrain solid/one-way, font có dấu; đo grip/pivot/phase/độ đọc và công sửa |

**Chưa bắt buộc cho G-L:** login/backend/DB, network/reconnect, Cung chơi được, PvP, Boss, các bãi sau Đồng Sương. Các phép thử local dùng profile/fixture và kho trạng thái trong RAM; reset/đóng phiên có thể mất dữ liệu, phải ghi rõ trong bản thử. Không tạo save JSON như authority production. Adapter local trả thành công trong RAM chỉ chứng minh flow, chưa chứng minh crash atomicity hay persistence.

A07 vẫn giữ Dummy HP60/respawn25 s baseline; điểm đứng/timer nhanh/DEF/EVA phải ghi là fixture hoặc proposal đang kiểm, chưa sửa GDD. A14 chưa khóa quyền movement/jump lúc cast: probe hành vi rõ và ghi evidence trước chốt; không ép movement lock để cứu pose count.

<a id="phase-gates"></a>

# 3. Các phase và gate mở rộng

Gate là điều kiện kiểm có evidence, không tự pass vì tới tuần dự kiến. [Technical §10](2_HUYEN_LO_TECHNICAL.md#technical-gates) mô tả phép thử và setup. Decision IDs ở Analysis; G-x dưới đây chỉ là gate triển khai, không hệ quyết định gameplay mới.

| Phase / gate | Điều kiện vào và công việc | Đủ để ra khỏi gate / cho phép tiếp theo |
| --- | --- | --- |
| Pha L / **G-L: VS-1 local** | Chuẩn bị boundary đơn giản, blockout ba map, art probe nhỏ; ghép đường Q1–Q6/Kiếm | Video và log hành trình liên tục; lỗi hit/pivot/quest/death lớn đã xử lý, giờ art/QA/rework và % dùng được có số thật. Mở spike G-N |
| Pha N / **G-N: Dedicated + ≥2 clients sớm** | G-L ổn; chuyển cùng luật/resolver/timeline sang headless Dedicated. Có thể dùng admission/commit RAM fixtures dev, ghi rõ không backend thật | Hai client độc lập vào cùng server, movement/portal/MapId, một action/kill/quest credit/shared claim đúng từng recipient; replay/duplicate/stale life/late result không nhân. Ghi correction/latency, headless physics và presentation remote. Chỉ sau pass mới mở production content/art rộng có chọn lọc |
| Pha D / **G-D: backend/persistence thật** | G-N pass; thay admission/commit fixtures bằng Spring/PostgreSQL, giữ một writer và cùng domain result | Login → Select → one-time ticket → join; lease/duplicate, checkpoint/SafeAnchor/HP0, một death N recipients và claim/quest commit idempotent; crash/outage/retry trước/sau ACK. Mở gameplay có dữ liệu bền và integration mở rộng |
| Pha C / **G-C: mở content theo dependency** | G-N pass trước art rộng; hệ reward/quest có DB thật G-D trước nghiệm thu route dài | Kiếm evolution/ult/passives; Q7–Q12, bảy identities/sáu rigs, maps/route/Linh/Q8/gear đúng GDD. Cung prototype rồi full branch dùng cùng architecture; kiểm art/balance trước nhân variant. Không thêm content cho gap level |
| Pha F / **G-F: Boss và full visual load** | Content/skill/world ổn, schema/lifecycle không đổi lớn; mở Boss/Cung load tests | Lịch/telegraph/Slow/Cuồng, corpse/loot/credit/camera đúng; chơi thật hai người và probe 3–4+ với số máy/build/CPU/bytes/latency. Ghi lại TTK/journey, không coi mô phỏng cũ là acceptance |
| Pha P / **G-P: PvP/chat và ghép hệ online** | G-D trước escrow; movement/combat/recovery đủ ổn | 10 stakes, hai bên escrow, Food/quota, timeout DRAW/forfeit/abort, settlement/crash retry; Map Chat, reconnect, standalone package theo Technical. Online subset pass vẫn chưa có nghĩa full P0 đã xong |
| **G-T: nghiệm thu TARGET** | Các branch Kiếm/Cung và toàn P0 đã ghép; G-L/N/D/C/F/P có evidence liên quan | Đối chiếu toàn GDD §10 và Technical §12 trên hai Client + Dedicated + backend/DB thật. Không ký done khi thiếu Cung, Q12/Boss/PvP hoặc recovery; không dùng video fallback thay kiểm chạy thật |

G-N là gate kiến trúc mạng **tương đối sớm**, dùng chính ba map và combat của VS-1; không đợi vẽ hết gear/Bow/Boss rồi mới thử authority. Spike không backend phải mang nhãn fixture dev, không phân phối như final online flow. G-D giữ nội dung architecture spike cũ và là gate bắt buộc trước nhận persistence/reliability là hoàn thành.

<a id="management-window"></a>

# 4. Khung quản lý hai tháng, nhóm bốn người

**Hạn nguồn lực thực tế: hai tháng / bốn người.** Chia thành khoảng tám tuần quản lý từ ngày bắt đầu thực hiện; chưa có ngày bắt đầu hoặc số giờ khả dụng từng người để suy deadline lịch hay tổng person-hours. Các tuần dưới là **mục tiêu quản lý**, không cam kết kỹ thuật. Đầu ra ưu tiên là VS-1 + G-N sớm, sau đó tích hợp G-D và mở TARGET theo evidence. Không tuyên bố toàn bộ TARGET chắc chắn xong trong hai tháng.

| Tuần mục tiêu | Trọng tâm / đầu ra reviewable | Điều kiện và cách xử lý nếu chưa đạt |
| --- | --- | --- |
| 1 | Boundary Local Session/rules/result, clock/IDs; movement/blockout và mẫu art nhỏ | Chạy được intent→state→presentation; ghi lỗi A01/A02/A12/A14, giờ sửa và giới hạn Free thực dùng |
| 2 | Combat feel/rig/weapon/Dummy/Nấm/Sói; Q1–Q3 và room collider/one-way | Review art/combat slice đầu; chưa nhân full families hoặc map ngoài ba map |
| 3 | Ghép Q1–Q6/Kiếm, Food/Bình/equip/loot/sell/death/UI → **G-L** | Nếu hành trình kẹt hoặc feel/pose chưa đọc, sửa slice và cập nhật tuần; không lấy fixture skip quest làm pass |
| 4 | Dedicated build + ít nhất hai clients trên slice → **G-N** | Đây là checkpoint mạng trước production rộng. Nếu phải sửa boundary/headless/recipient, dừng mở rộng content và art để sửa |
| 5 | Spring/PostgreSQL/auth/ticket/lease/commit/recovery → **G-D mục tiêu**; prototype Cung hẹp nếu G-N đã pass | G-D sai thì chưa nhận persistence done. Art Cung chỉ probe A01/A02/A05; phần chưa làm vẫn DEFERRED TARGET P0 |
| 6 | Mở Kiếm Lv10/17, gear/Q7–Q8/Linh và route tiếp theo theo G-C; Cung tiếp tục prototype rồi tích hợp | Review throughput/quest/pose và nguồn lực; ưu tiên ghép một đoạn end-to-end thay nhân toàn bộ catalog chưa kiểm |
| 7 | Mục tiêu tích hợp Boss/recovery/chat; bắt đầu PvP nếu G-D đủ và combat ổn, mở branch Cung đã validate | Phạm vi hoàn thành phụ thuộc gate; ghi từng hệ done/chưa done, giữ TARGET trong backlog, không đổi Cung thành P1 |
| 8 | Regression, standalone package/evidence và review phạm vi đã chạy; **G-T chỉ nếu đủ toàn P0** | Báo rõ slice/online subset đạt tới đâu. Nếu thiếu thời gian, chọn kéo lịch hoặc trade-off cụ thể với chủ dự án; không đổi nghĩa final acceptance |

G-C/F/P có thể ghép hạng mục độc lập sau gate phụ thuộc, không đòi mọi người chờ hết phase. Một người có thể chuẩn bị schema fixture/tests hoặc artwork probe sớm; điều đó không mở production rộng trước G-N và không chứng minh gate đã pass.

| Vai trò chính trong nhóm bốn người | Trách nhiệm và phối hợp |
| --- | --- |
| 1 — Gameplay/authority | Local Session, combat/stat/timeline/lifecycle; ghép Dedicated cùng người 4; giữ một resolver và clock |
| 2 — World/quest/AI | Blockout/physics/portals, NPC/Q1–Q6, mob/loot/route; thử với người 1, sau gate mới mở maps/quest tiếp |
| 3 — Art/UI presentation | Mẫu art/clean/slice/socket, rig/weapon/terrain và common kit; đo với người 2/4; không cần vẽ toàn bộ catalog trước playable |
| 4 — Integration/QA/backend | Build/log/fixtures, regression hành trình, Dedicated/2-client spike rồi Spring/DB/recovery; hỗ trợ local slice từ tuần 1 |

Đây là phân trách nhiệm để giảm phụ thuộc, không giả mỗi người đã có năng lực tương đương hoặc làm full-time. Mỗi review dùng giờ **khả dụng còn lại** và giờ thật đã tiêu cho gameplay/art/editor/backend/integration/QA/rework. Không nhân bốn người với estimate 160–240 h cũ để gọi thành ngân sách mới.

<a id="art-workflow"></a>

# 5. Art cho người chưa thạo vẽ và kỷ luật giao việc

Dùng quy trình nhỏ trong [Art §23.3](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#art-tool-workflow): chọn mẫu, sửa palette/outline/pivot/grip bằng editor pixel, nhập Unity rồi đo.

**TOOL CANDIDATE / CURRENT PROBE:** có thể thử PixelLab bằng Free/free trial trước; nếu không đạt, đổi tool hoặc workflow. Contract visual/import và architecture không phụ thuộc PixelLab.

Free có giới hạn generation/tool theo [FAQ chính thức](https://www.pixellab.ai/docs/faq); không giả toàn bộ animation/outfit tools có sẵn miễn phí hay hứa số credits. Nếu tool cần không có, ghi “chưa kiểm được với Free”; dùng placeholder để kiểm pipeline và đánh giá chi phí phần còn thiếu trước quyết định mua.

Đếm cả output thất bại. `% dùng trực tiếp = output pass không sửa / toàn bộ output tạo`; `% dùng sau sửa = output pass sau sửa / toàn bộ output tạo` là nhóm riêng, không cộng một output hai lần; `% dùng được tổng = (hai nhóm pass) / toàn bộ output tạo`. Khi chưa tạo mẫu ghi **CHƯA ĐO**, không đoán tỷ lệ. Ghi giờ sửa/import/QA, sai pose/alignment và số mẫu cho từng loại. Gate không dựa vào một PNG đẹp: ít nhất mẫu rig/weapon/outfit chạy cùng timing trong room thật phải đọc được. Không tạo PixelLab plan, trial plan, prompts, assets hoặc manifest riêng trong vòng docs này.

Kỷ luật áp dụng cho mọi người và coding agent ở [Technical §1.1](2_HUYEN_LO_TECHNICAL.md#architecture-discipline): gameplay tách UI, ID ổn định, một clock gameplay authority/session, AnimationEvent chỉ presentation; Local và Dedicated dùng cùng rules. Task phải chỉ rõ TARGET/CURRENT, canonical sections, input/result cần thay, gate/test relevant và OPEN dependency. Agent không tự đổi số/range/timer/26-frame/Boots để làm task pass; đề xuất thay đổi phải ghi Analysis rồi sync authority khi đã duyệt.

<a id="production-release"></a>

# 6. Khi nào mở production và điều gì được hoãn

Trước G-N chỉ sản xuất mẫu art nhỏ đủ kiểm VS-1: một room mẫu để kiểm art, default/outfit I, Mộc/Kiếm và vài mob/kit primitives. **Với baseline hiện tại, VS-1 cần blockout chơi liên tục qua đủ ba map**; một room mẫu chỉ kiểm art, không thay hành trình. Nếu evidence cần đổi kích thước slice, review baseline theo §2 trước. Trước nhân mỗi family sau G-N phải có pose/socket/readability evidence tương ứng, các OPEN liên quan đã giải hoặc có phạm vi probe rõ. G-N cho phép mở rộng có chọn lọc, **không tự duyệt hybrid/33 pose/13–25 weapon images/camera/terrain counts**.

Giữ cut ladder: cắt P1/P2 và polish trước; giảm cosmetic variations/shake/sound/summary cầu kỳ, giữ thông tin gameplay/pending/status/telegraph, Storage core và Journey scores. Cung/Boss/backend/PvP DEFER khỏi VS-1 theo thứ tự hiện hành, không xóa khỏi TARGET. Review cuối tuần 2/4 và sau mỗi gate bằng công còn lại; nếu TARGET vượt hai tháng, trình phạm vi bản thử cụ thể và lịch tiếp theo, không âm thầm hạ final DoD. Các rationale và workload từ lịch cũ được giữ đầy đủ ở phần trace dưới đây.

<a id="legacy-roadmap"></a>

# 7. Trace roadmap cũ — dữ liệu giữ để đối chiếu

Chuyển nguyên từ Technical §10/asset gate trước consolidation. **Không phải lịch đang điều hành.** Thứ tự backend-first và Sword/Bow art cùng lúc đã được thay bằng VS-1 → G-N → G-D. Gates/giờ cần đo/cut rationale bên dưới vẫn có giá trị; mốc Week/160–240 h/12 modules là giả định lịch sử, không thông số mới được duyệt. Chi tiết đầy đủ art accounting hiện dùng để probe nằm ở Art §23, trạng thái ở Analysis A01/A17.

<a id="legacy-tech-spike"></a>

## Gate backend/dedicated cũ

**Gate đầu: architecture spike** — PostgreSQL + Spring Boot login/character/ticket + Unity Dedicated build + hai Clients; kiểm ticket một lần, duplicate connection/lease, SafeAnchor/checkpoint + resume ngắn, movement/portal/MapId, một kill reward và một loot claim commit idempotent. Chạy thêm 3–4 Clients để tìm fixed-pair assumptions, đo backend latency, không công bố capacity từ ca pass. MPPM dùng khi iterate, acceptance vẫn cần standalone server/clients. Đo giờ build/integration thật trước cam kết lịch; nếu ticket, DB commit, checkpoint hoặc headless physics sai thì sửa trước mở content.

<a id="legacy-calendar"></a>

## Bảng lịch tám tuần cũ và giới hạn estimate

| Tuần | Milestone | Gate |
| --- | --- | --- |
| 1 | Architecture spike: DB/backend/login/ticket/dedicated build | Hai Clients, MapId, một character load và một reward/claim commit idempotent |
| 2 | Combat slice / rig pipeline | Normal, Lv 5 single +Lv 10 evolution fixtures, target fallback, separation / melee miss / projectile, một modular family |
| 3 | Progression / persistent items | EXP / attributes, class reset, Food / Potion / Death, PostgreSQL inventory/loot/sell; contribution và cửa nhặt đã rõ |
| 4 | Gear / shop / quest framework | Enhance / chuyển giao sáu slots, backend transactions, Q1–Q8 / manual grants / forced variant, recipient policy / quest assist; scope checkpoint |
| 5 | Cụm quái / remaining skills | Năm farm roots / density matrix TEST, bảy fixed identities / sáu rigs / Linh Biến cap1, hai active +hai nội tại / class / evolution / manuals |
| 6 | Boss / co-op / recovery | Shared pile / claim / reset / Cuồng Mạch, Q11 / Q12; backend outage/retry và restart semantics, lịch Boss trên scene |
| 7 | PvP / chat / story / demo | Q9 optional, escrow/settlement idempotent, potion quota, match 120 s, banners/summaries, account/character fixtures |
| 8 | Integration / QA / package | Regression, evidence tối thiểu 2 concurrent players, build / scripts / video fallback |

Lịch tám tuần phía trên là **thứ tự gate, chưa là cam kết thời gian**. Mốc 160–240 h cũ được tính cho player-host/JSON nên không còn là estimate hợp lệ sau khi thêm Spring Boot, PostgreSQL, auth và dedicated build. Đo riêng giờ backend/schema/migration, Unity server build, integration/recovery và editor/art tại slice đầu rồi lập lại ngân sách; báo trượt milestone hoặc scope P1 minh bạch. Không giảm sáu base rigs / hai class / 26 frames user-lock để ép lịch.

<a id="legacy-workload"></a>

## Workload, cut ladder và reuse rationale cũ

| Workload cần đo tại slice | Unit / reuse | Evidence phải ghi |
| --- | --- | --- |
| Character rig | Một male rig, 26 frames; chung pivots / frame controller | Giờ clean / slice / import, lỗi flip / socket / pose |
| Gear visuals | 12 modules = 3 bands × Sword / Bow / Armor / Pants | Giờ trên module đầu, phần reuse so redraw; không nhân 21 templates thành rigs |
| Icons / data | 21 regular gear + Mộc Kiếm; 6 manuals dùng 2 motifs × 3 accents; consumable / material riêng | Template count, icon mới / reuse (thêm bốn passive icons từ hai motifs), validation bindings / stat evaluator |
| World | 5 farm rooms + 3 support roots; pocket / slot budget theo GDD | Giờ placement / route / aggro / vertical / portal QA trên room đầu rồi extrapolate |
| Mobs / Boss | 6 base sets +1 wolf palette, 7 identities, shared Linh modifier, 1 Boss | AI / config reuse, telegraph / collision / scheduler integration |
| UI / network / backend | HUD/modal, ticket/lease, ownership claims, DB transactions, dedicated build | Editor/prefab/backend integration hours, latency/crash/standalone test |

**Contingency / cut ladder:** review sau Art Vertical Slice, cuối Week 2 và Week 4 scope checkpoint, bằng actual spent / remaining hours. Cắt P1 / P2 trước; sau đó giảm cosmetic polish / additional sound / VFX variations, elaborate Journey summary presentation (giữ scores / summary core), Storage presentation depth (giữ 40 slots / basic deposit-withdraw), extra UI polish / QoL. Không tự cut hai classes, sáu base rigs/bảy identities, 26-frame rig, Lv 1 → 20 hoặc multiplayer core; không demote PvP / Q9 / MapChat nếu chưa user approve. Nếu vẫn vượt 160–240 h, đổi schedule hoặc trình concrete scope tradeoff để user quyết, không gọi nghiệm thu phần thiếu là done.

Linh Biến tái dùng normal sprites nhưng vẫn cần shared aura và integration / QA; không suy ra giảm giờ vẽ hai bộ Elite riêng vì baseline vốn không có các bộ đó. Đo full rig / family / farm-room ở Week 2 và actual placement / route / QA cho layout mới trước cam kết budget; bỏ anchors / timers cũ không chứng minh tổng giờ giảm.

**Chú thích trace:** câu “Nếu vẫn vượt 160–240 h” và các Week ở đoạn cũ trên chỉ phản ánh điều kiện lịch sử. Ngân sách hiện hành phải lập từ giờ khả dụng của nhóm bốn người trong hai tháng và số đo slice; không dùng 160–240 h làm trần hay tổng person-hours.

<a id="legacy-art-gate"></a>

## Asset gate cũ — trace của yêu cầu thử cả Kiếm/Cung

Asset gate: làm một full rig 26 frames, một Sword / Bow / Armor / Pants family và một farm-room trước sản xuất đủ ba gear bands. Kiểm outline / palette / pivot / frame sync khi chuyển gear. AI-generated bitmap nếu dùng vẫn cần slice / clean / import / QA; chưa tạo asset trong vòng docs này.

Gate art CURRENT đã tách theo class ở §2–3: Kiếm thử trước, Cung thử trước production branch Cung. Câu gate cũ ở trên được giữ để không mất history, không dùng làm điều kiện chặn VS-1. Tương tự, Week 2/4 cũ được map sang các checkpoint quản lý §4.

<a id="source-destination"></a>

# 8. SOURCE → DESTINATION — bản đồ bảo toàn nội dung

Bảng dưới ghi lượt consolidation trước cleanup. Một số detail Art nay nằm trong phụ lục; anchor đích cũ vẫn giữ. Vị trí cleanup cụ thể ở [Art — bản đồ MOVE](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#cleanup-source-destination).

Mọi MOVE đã ghi đích đầy đủ trước khi rút nguồn; nguồn còn summary/link. GIỮ nghĩa là không xóa khi không có đích tốt hơn. Các bảng luật/evidence được giữ nguyên; khác biệt authority/context được ghi để tránh hai nơi cùng sửa số. Bảng lịch cũ/sync proposal cũ vẫn có nhãn trace; không thành lịch hoặc quyết định mới.

| Mã | SOURCE | DESTINATION | Cách giữ / merge / summary + reference |
| --- | --- | --- | --- |
| T01 | Technical §10: gate backend-first cũ | [5 — legacy-tech-spike](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#legacy-tech-spike) | MOVE đầy đủ, đánh dấu trace; G-D hiện hành giữ kiểm thật |
| T02 | Technical §10: bảng tám tuần + estimate | [5 — legacy-calendar](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#legacy-calendar) | MOVE nguyên bảng/giới hạn; lịch CURRENT mới ở §4 |
| T03 | Technical §10: workload/cut ladder/reuse | [5 — legacy-workload](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#legacy-workload) | MOVE đầy đủ; 12 modules/giờ cũ giữ nhãn lịch sử |
| T04 | Technical §8: asset gate Sword/Bow cũ | [5 — legacy-art-gate](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#legacy-art-gate) | MOVE nguyên gate; CURRENT thử Kiếm trước |
| T05 | Art §10: timing/probes/occupancy/normal | [3 — art-combat-timing-evidence](3_HUYEN_LO_DESIGN_ANALYSIS.md#art-combat-timing-evidence) | MOVE đầy đủ bảng/phép tính/giả định; Art giữ kết luận+link |
| T06 | Art §10.1: sustain/TTK probe assumptions | [3 — art-sustain-evidence](3_HUYEN_LO_DESIGN_ANALYSIS.md#art-sustain-evidence) | MOVE đầy đủ bảng và giới hạn, không retune |
| T07 | Art §10.2: proc cadence/feedback | [3 — art-proc-evidence](3_HUYEN_LO_DESIGN_ANALYSIS.md#art-proc-evidence) | MOVE đầy đủ phép tính/reasoning; tần suất không thành uptime |
| T08 | Art §25: A01–A17 options/trade-off | [3 — art-open-decisions](3_HUYEN_LO_DESIGN_ANALYSIS.md#art-open-decisions) | MOVE nguyên bảng; thêm trạng thái/alias gate, Art chỉ link |
| T09 | Art §20: field/timebase proposal | [2 — presentation-data](2_HUYEN_LO_TECHNICAL.md#presentation-data) | MOVE nguyên nhu cầu/schema đề xuất; còn OPEN |
| T10 | GDD §9: Visual production flow table | [4 — legacy-visual-flow](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#legacy-visual-flow) | MOVE nguyên bảng; GDD giữ yêu cầu nhìn thấy+link |
| T11 | GDD §9: art accounting 12 modules | [4 — legacy-art-accounting](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#legacy-art-accounting) | MOVE nguyên phép đếm cũ; phân biệt thiếu Mộc/fallback với S0 |
| K01 | GDD §2/§6: EXP/stat/catalog/enhance/loot tables | [1 — character-power](1_HUYEN_LO_GDD.md#character-power) | GIỮ nguyên mọi bảng số; không đổi gameplay |
| K02 | Analysis §2: stat/TTK/MP/damage/status/PvP tables | [3 — character-evidence](3_HUYEN_LO_DESIGN_ANALYSIS.md#character-evidence) | GIỮ nguyên mọi bảng; ghi rõ simulation cũ/giả định |
| K03 | Analysis §3: farm ladder/density/respawn tables | [3 — farm-progression](3_HUYEN_LO_DESIGN_ANALYSIS.md#farm-progression) | GIỮ nguyên mọi bảng/giới hạn map budget |
| K04 | Analysis §4: gear/upgrade/economy/journey/Boss tables | [3 — economy-analysis](3_HUYEN_LO_DESIGN_ANALYSIS.md#economy-analysis) | GIỮ nguyên mọi bảng và reasoning, chưa chạy runtime |
| K05 | Art §1–3: pose/weapon/Lower/Boots reasoning | [4 — player-visual](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#player-visual) | GIỮ đủ bảng/trade-off; chỉ làm rõ A01 là proposal |
| K06 | Art §6–9: mob/Hit/Death/Dummy tables | [4 — mob-visual](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#mob-visual) | GIỮ nguyên bảng và phép đếm/timing probe |
| K07 | Art §11–19: map/terrain/structure/environment/NPC/VFX/icons/UI | [4 — map-visual](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#map-visual) | GIỮ đầy đủ các matrix/counts/reasoning |
| K08 | Art §22–24: contract/S0/family cost/prototype matrix | [4 — production-accounting](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#production-accounting) | GIỮ nguyên các bảng/kịch bản; thêm Free workflow và thứ tự probe |
| K09 | Art §26: sync proposal history/dependencies | [4 — sync-history](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#sync-history) | GIỮ nguyên bảng cũ, đánh dấu trace chưa duyệt toàn bộ |
| K10 | Technical §11–12: risk/fixtures/acceptance tables | [2 — qa](2_HUYEN_LO_TECHNICAL.md#qa) | GIỮ nguyên bảng; final acceptance vẫn Dedicated/backend/DB |

**Các chỉnh sửa routing/context:** GDD §0/§1/§10 thêm vai trò năm file và TARGET/CURRENT; Technical §1.1 thêm Local→Dedicated/kỷ luật kiến trúc và §10 giữ gate/setup thay calendar; Art §0 phân loại nhóm và §24 ghi thứ tự probe; README mở đường đọc 1–5. Đây là phần bổ sung/diễn đạt lại, không xóa catalog/simulation/decision history.

<a id="no-loss-audit"></a>

# 9. No-loss audit của lượt consolidation

**Snapshot lịch sử:** các số trước/sau và mẫu kiểm dưới đây ghi riêng lượt consolidation, không phải số đo sau cleanup. Giữ nguyên để đối chiếu; audit cleanup dùng snapshot riêng và kiểm các đích MOVE hiện tại.

Mốc Git trước sửa: `52d7a4a` (`docs: snapshot art analysis before consolidation`). Bản gốc và script/snapshots/metrics/migration proofs lưu tại `/tmp/huyenlo-consolidation/`; thư mục tạm không thuộc deliverable repo và có thể mất sau phiên. Các kết quả cần review được giữ ngay trong mục này, không tạo audit/plan/manifest riêng trong active docs.

Đếm bằng Python 3: dòng dùng `splitlines()`, bảng là **số dòng bắt đầu bằng `|`** (gồm header/separator), heading dùng `^#{1,6}\s`. Theo dõi literal `53.100`, `95 điểm`, `32.000`; frame dùng regex `26` + khoảng trắng/gạch nối + `frame/frames/khung`; payout là dòng có `1.800` và số nguyên `200` (nhận cả bảng hai cột). Đây là occurrences, không số lượng gameplay. File mới có baseline 0.

<!-- AUDIT_COUNTS_START -->
Số **trước → sau** (giá trị sau bao gồm chính bảng báo cáo này):

| File | Dòng | Dòng bảng | Heading |
| --- | ---: | ---: | ---: |
| 1 | 752 → 750 | 341 → 336 | 25 → 25 |
| 2 | 337 → 385 | 127 → 124 | 13 → 16 |
| 3 | 382 → 479 | 195 → 246 | 18 → 22 |
| 4 | 725 → 766 | 397 → 393 | 38 → 39 |
| 5 | 0 → 256 | 0 → 112 | 0 → 14 |
| README | 86 → 90 | 45 → 45 | 6 → 6 |

Occurrences số quan trọng **trước → sau**; thay đổi vị trí do MOVE hoặc thêm câu giải thích/audit, không đổi giá trị gameplay:

| File | 53.100 | 95 điểm | 32.000 | 26 frame/khung | 1.800 cùng 200 |
| --- | ---: | ---: | ---: | ---: | ---: |
| 1 | 1 → 1 | 3 → 3 | 1 → 1 | 5 → 3 | 1 → 1 |
| 2 | 1 → 1 | 1 → 1 | 2 → 2 | 5 → 3 | 1 → 1 |
| 3 | 1 → 1 | 4 → 4 | 2 → 2 | 1 → 3 | 1 → 1 |
| 4 | 0 → 0 | 0 → 0 | 0 → 0 | 6 → 9 | 0 → 0 |
| 5 | 0 → 2 | 0 → 2 | 0 → 2 | 0 → 7 | 0 → 2 |
| README | 0 → 0 | 0 → 0 | 0 → 0 | 0 → 0 | 0 → 0 |

EXP/điểm/Boss/payout của các file gameplay/evidence gốc giữ nguyên. Occurrences frame giảm ở GDD/Technical vì pipeline/accounting/calendar/asset gate đã chuyển sang Art/Roadmap; các khối này đã được kiểm nguyên văn. Giá trị 26 chưa được diễn giải lại thành lock mới.
<!-- AUDIT_COUNTS_END -->

Ngoài đếm, script đã kiểm **110 khối bảng gốc của file 1–4** còn nguyên trong bộ đích và 11 block MOVE đầy đủ ở file chỉ định. README giữ toàn bộ row cũ trừ ô nghĩa P0 được cập nhật từ “bản đầu” sang TARGET để VS-1 không bị hiểu là full P0; đây là thay thuật ngữ theo feedback, không mất bảng số. Chỉ sửa ô ấy trong bảng thuật ngữ. Script đếm không giao agent. Analysis mất từ 100 dòng trở lên phải dừng và hỏi; lượt này Analysis tăng vì nhận evidence/decision từ Art. Link nội bộ/anchor và `git diff --stat` được kiểm sau edit.

<!-- AUDIT_SAMPLE_START -->
Lấy mẫu bằng `secrets.SystemRandom().sample(..., 8)` từ 21 mục, lưu selection trước khi đọc: **T04, T05, T02, K02, K04, T09, T08, K05**. Root đã tự mở/đọc nội dung và ngữ cảnh tại đích, ngoài kiểm full-block tự động; không giao agent xác nhận mẫu thay mình.

| Mục ngẫu nhiên | Đích đã tự kiểm | Nội dung thực có / kết quả |
| --- | --- | --- |
| T04 | [5 — legacy-art-gate](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#legacy-art-gate) | PASS — Gate 26 frames/Sword–Bow/farm-room còn nguyên, nằm trong trace và có chú thích CURRENT Kiếm trước. |
| T05 | [3 — art-combat-timing-evidence](3_HUYEN_LO_DESIGN_ANALYSIS.md#art-combat-timing-evidence) | PASS — Bảng normal/core/big, occupancy 20%/5,7% và timing reasoning đủ ở Analysis; không retune. |
| T02 | [5 — legacy-calendar](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#legacy-calendar) | PASS — Bảng lịch tám tuần và đoạn estimate 160–240 h được giữ nguyên dưới nhãn lịch cũ. |
| K02 | [3 — character-evidence](3_HUYEN_LO_DESIGN_ANALYSIS.md#character-evidence) | PASS — Hàng Common III +0 35,35/26,32/20,87 và giả định PvP một chiều vẫn ở Analysis §2. |
| K04 | [3 — economy-analysis](3_HUYEN_LO_DESIGN_ANALYSIS.md#economy-analysis) | PASS — Bảng +8 20,65 lần thử/36.542 Vàng/173,67 đá/175.479 quy đổi cùng giả định còn ở §4. |
| T09 | [2 — presentation-data](2_HUYEN_LO_TECHNICAL.md#presentation-data) | PASS — Có đủ correlation/generation/resolveClock/flight/status fields và 20Hz–50Hz–60FPS, nhãn PROPOSAL. |
| T08 | [3 — art-open-decisions](3_HUYEN_LO_DESIGN_ANALYSIS.md#art-open-decisions) | PASS — Bảng A01–A17 options/recommendations giữ nguyên; trạng thái mới gắn gate, không LOCKED proposal. |
| K05 | [4 — player-visual](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#player-visual) | PASS — Bảng Kiếm hybrid 1–4 hình và Cung 3 shapes giữ nguyên; 13–25 còn kịch bản OPEN. |

Reader Testing độc lập theo skill doc-coauthoring kiểm scope/gates, authority/ACK và Art OPEN; các ambiguity immediate pose, t0 death, speed Cung, request/action IDs, room/ba map và legacy budget đã được sửa. Reader không đếm file. Chi tiết Potion ordering/cap release/time capture được giữ làm ca SPIKE/OPEN, không tự duyệt cơ chế mới.
<!-- AUDIT_SAMPLE_END -->

**Giới hạn:** kiểm đếm/khối bảng/migration/link là audit tài liệu, không nghiệm thu Unity, art đã xuất, performance, balance hoặc backend transactions. Tất cả gate runtime và mẫu art/% dùng được vẫn CHƯA CHẠY/CHƯA ĐO.
