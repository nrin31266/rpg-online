# Huyền Lộ — Roadmap triển khai và trạng thái hiện tại

**Ngày đồng bộ:** 2026-10-04 · **Trạng thái:** DESIGN + PROTOTYPE VALIDATION; VS-1 disposable/reference cũ, production codebase chưa bắt đầu, chưa mở G-N.

File này sở hữu **thứ tự làm, CURRENT/DEFERRED, điều kiện mở production và hồ sơ riêng của bản mẫu** (§10). [GDD](1_HUYEN_LO_GDD.md) giữ game đích; [Technical](2_HUYEN_LO_TECHNICAL.md) giữ cách chạy/tích hợp; [Analysis](3_HUYEN_LO_DESIGN_ANALYSIS.md) giữ evidence/quyết định; [Art](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md) giữ chi tiết hình ảnh và production. Không dùng roadmap để sửa luật, duyệt proposal Art hoặc thu nhỏ nghiệm thu cuối.

<a id="target-current-deferred"></a>

# 1. TARGET, CURRENT và DEFERRED

**TARGET** là toàn bộ P0 hiện hành: Tân Lữ, Kiếm và Cung, hành trình Lv 1–20/Q1–Q12 (Q9 optional), gear/kinh tế, năm farm maps và ba support zones, Linh Biến/Boss, co-farm/chat/PvP; Unity Client + Dedicated Game Server + Spring Boot + PostgreSQL, Login/Character Select/persistence/recovery. Nghiệm thu cuối vẫn tối thiểu hai client đồng thời với backend/DB thật theo [GDD §10](1_HUYEN_LO_GDD.md#acceptance-routing).

**CURRENT** là harvest VS-1 mock đã chạy → sửa docs → movement/UI/art/rig probe → thiết kế production base → implement core base/local slice revision mới → G-N Dedicated với hai client sớm. Không nối network thẳng vào throwaway classes hoặc xem cấu trúc prototype là base đã duyệt. `Offline/local-first` là cách triển khai để kiểm sớm, không đổi game đích thành single-player. `CURRENT` không có nghĩa đã code hoặc đã pass.

**Evidence lịch sử 2026-10-03:** [VS-1 disposable/reference](../../prototypes/VS1_EndToEnd/README.md) tại checkpoint `46006c4` có route/tests/build Linux revision V6.2.0. Feedback V6.2.1 đổi control/Q4–Q5/transition; evidence cũ giữ để trace, **không pass G-L revision mới hoặc production base**. G-L PARTIAL: chưa có normal-speed feel/usability review, rig/pose/pivot/socket, giờ art/QA/rework/% asset dùng được. Prototype code/evidence tách dưới `_Prototype`/`PrototypeEvidence`; production chưa bắt đầu.

**Prototype update 2026-10-04:** V6.2.4 có route/tests nhưng bị user từ chối về hình địa hình: Sói trong lòng nước, dòng nước đứt, background chưa đúng lớp đất. V6.2.5 sửa blockout; kết quả kiểm ở [hồ sơ prototype](#prototype-visual-review), không tự pass G-L hoặc art production.

**DEFERRED** là phần đã thiết kế và vẫn phải làm trong TARGET, nhưng chưa nằm trên đường phụ thuộc đầu tiên. Cung thuộc TARGET P0, **không phải feature P1**. Mọi bảng Cung, projectile, gear, pose/VFX và balance được giữ. Q6/lore vẫn giới thiệu hai phái; trong bản thử sớm, Kiếm chơi được, Cung ghi “Chưa mở trong bản thử nghiệm”. Không dùng nhãn này trong sản phẩm cuối.

| Nhóm | Trạng thái triển khai hiện tại | Điểm quay lại / điều kiện |
| --- | --- | --- |
| Tân Lữ → Kiếm, movement, basic/S1, nội tại nền tảng | CURRENT docs/feel/art probe, prototype cũ reference | Base review rồi G-L revision mới Q1–Q6 |
| Vân Khê, Học Viện, Đồng Sương; Nấm/Sói/Dummy/UI | CURRENT blockout/UX probe, chưa production | Validate EdgeExit, Nấm→Sói, ≥3 Dummy, one-shot input ở tốc độ thường |
| Kiếm Lv 10/13/17 và các bãi/Q7–Q12 | DEFERRED khỏi VS-1, TARGET P0 giữ nguyên | G-N + G-D; có thể dùng fixture hẹp để kiểm kỹ thuật trước hành trình đầy đủ |
| Cung: chọn/chơi, mọi skill/gear/projectile/pose/balance | DEFERRED IMPLEMENTATION, TARGET P0 | Sau G-N; giải A01/A02/A05/A14 bằng prototype Cung trước nhân toàn bộ gear |
| Dedicated Server và ít nhất hai clients | Gate sớm **sau base review và G-L revision mới**, chưa bắt đầu | G-N trước mở production content/art rộng; không attach vào throwaway prototype |
| Login/Character Select, Spring/PostgreSQL, ticket/lease/checkpoint/reconnect | DEFERRED khỏi local slice, TARGET P0 | G-D sau spike mạng; chưa có DB thật không được gọi final online acceptance |
| Chat/co-farm/claim shared, Linh Biến/Q8 | DEFERRED khỏi VS-1 | G-N chứng minh recipient/MapId; G-D chứng minh commit; G-C mở content |
| Boss/full online visual load | DEFERRED prototype và production | G-C; giữ các luật Boss đã có, giải A16/camera/telegraph ở G-F |
| PvP/Q9/escrow/settlement | DEFERRED khỏi VS-1, TARGET P0 | G-D trước G-P; Q9 optional cho người chơi không có nghĩa được bỏ hệ PvP |
| Buff R, shield/groggy, QoL/P1/P2 | Chưa duyệt triển khai | Chỉ xét sau core; proposal vẫn ở Analysis, không chen vào VS-1 |

**Probe map/UI hiện tại:** thông số, sơ đồ và capture riêng của mock nằm ở [§10](#prototype-visual-review). Budget release, quest và các gate production không đổi. G-L vẫn PARTIAL; không dùng tests để kết luận layout/feel đã được người chơi duyệt.

<a id="vs-1"></a>

# 2. VS-1 — lát cắt đầu tiên phải chơi được

**CURRENT ROADMAP BASELINE:** Q1–Q6, Tân Lữ → Kiếm, ba map **Vân Khê / Học Viện / Đồng Sương** là phạm vi reference/probe và production local slice sau base review. Dùng layout/marker/spawn của GDD cho phần có mặt; Trúc Ảnh mở trong quest state sau Q6 nhưng nằm ngoài build thử, MapExit cần báo phạm vi bản thử thay vì giả đã có map. Theo baseline này, VS-1 không thêm Q7/enhance để thay mục tiêu đang kiểm.

Hướng triển khai đã xác nhận là **Kiếm trước → local/offline trước → gate mạng sớm**. Q1–Q6/ba map là baseline roadmap được dùng hiện tại, có thể thu/phình sau blockout nếu evidence cho thấy scope chưa hợp lý.

Khi review thay phạm vi slice, cập nhật mục này và phần G-L/validation liên quan; không âm thầm đổi quest/map trong GDD, dependency gate mạng hoặc final acceptance.

Một đường đi revision mới: NPC → nhảy/drop-through/EdgeExit auto-transition → nhận/mặc Mộc Kiếm, hạ ba Dummy đồng thời → Nấm/nhặt supply/mặc Áo/bán sample → chuẩn bị Food/Bình Máu, đánh Sói → catch-up Lv5 → chọn Kiếm/cộng điểm/học bí kíp/S1 + dùng Bình MP → trả Q6. Quest objectives là hành động, tutorial glyph từ bindings. Cung giới thiệu nhưng chưa playable; Quần/Áo/Mộc/Kiếm phải đổi hình đọc được. Đồ II/III chỉ fixture hẹp, không route mới.

| Cần có trong slice | Evidence G-L cần giữ |
| --- | --- |
| Local Session authoritative, intent/result/UI tách biệt | Một input chỉ qua resolver một lần; UI/VFX không tự sửa HP/túi/quest. Clock/life/action IDs có log |
| Movement/jump/drop-through, ba map/camera | Collider/surface khớp; Space/↑ và S/↓; EdgeExit auto/MapId/gate/refused-transition không ping-pong; jump feel/coyote/buffer variable-height manual probe |
| Basic Tân Lữ đến class transition, Kiếm S1 qua 1, quái melee | One-press approach/cast/no-repeat, cancel/replace pending, range/geometry/clock; melee separation/reposition không pile; action/death đúng life |
| EXP/điểm/reset Lv 5, item/equip/loot/sell, Food/HP/MP Potion/death | Runtime áp luật GDD cho phần đã hiện diện; full bag/reject/đầy HP không tiêu bình; restart fixture không giả lưu bền |
| Q1–Q6, NPC/modal/skill/quest/HUD | Route mới + late/retry/grant/full-bag; **manual usability** inventory/equip/shop/quest/pending tại tốc độ thường; automation không UX pass |
| Art/combat room nhỏ đã nhập Unity | Default + outfit I, Mộc/Kiếm I và một visual khác bằng fixture; Dummy/Nấm/Sói, impact/Death, terrain solid/one-way, font có dấu; đo grip/pivot/phase/độ đọc và công sửa |

**Chưa bắt buộc cho G-L:** login/backend/DB, network/reconnect, Cung chơi được, PvP, Boss, các bãi sau Đồng Sương. Các phép thử local dùng profile/fixture và kho trạng thái trong RAM; reset/đóng phiên có thể mất dữ liệu, phải ghi rõ trong bản thử. Không tạo save JSON như authority production. Adapter local trả thành công trong RAM chỉ chứng minh flow, chưa chứng minh crash atomicity hay persistence.

A07 giữ HP60/respawn25 s; GDD author ≥3 Dummy placements đồng thời để solo Q3 không phải chờ. DEF/EVA/timer thay thế/online contention vẫn fixture/prototype, không đổi normal farm respawn. A14 đã duyệt gravity/momentum liên tục; CURRENT probe Novice/S1, S2/S3 air permissions/pose còn OPEN; không ép movement lock để cứu pose count.

<a id="phase-gates"></a>

# 3. Các phase và gate mở rộng

Gate là điều kiện kiểm có evidence, không tự pass vì tới tuần dự kiến. [Technical §10](2_HUYEN_LO_TECHNICAL.md#technical-gates) mô tả phép thử và setup. Decision IDs ở Analysis; G-x dưới đây chỉ là gate triển khai, không hệ quyết định gameplay mới.

| Phase / gate | Điều kiện vào và công việc | Đủ để ra khỏi gate / cho phép tiếp theo |
| --- | --- | --- |
| Pha R / **Harvest + feel/art/UI probe** | Checkpoint prototype; thu findings, sync docs, mẫu rig/weapon/one-way/kit nhỏ | Chốt lesson learned, assumptions thử và contract; không cần production toàn bộ art hoặc biến mock thành base |
| Pha B / **G-B: production base review** | Pha R đủ evidence core: review input/intent/pending/clock/IDs/definitions/physics/commit/presentation boundary; thiết kế nhỏ rồi dựng base | Có module/dependency ownership, tests core theo revision mới, playable room production sơ bộ; quyết rõ phần reuse vs rewrite. Không tạo framework cho symmetry. Mở ghép G-L |
| Pha L / **G-L: local slice revision mới** | G-B pass; ghép Q1–Q6/Kiếm từ production base nhỏ, có art probe/kit đủ đọc | Video/log route mới, normal-speed feel/usability review, rig/pivot/quest/death ổn, giờ art/QA/rework/% dùng được thật. Cho phép G-N; prototype V6.2.0 không substitute |
| Pha N / **G-N: Dedicated + ≥2 clients sớm** | G-B và G-L revision mới pass; production rules/resolver/timeline chạy headless, adapter RAM dev có nhãn fixture | Hai client độc lập, movement/EdgeExit/MapId/kill/quest/shared claim, replay/stale life/late result đúng; đo correction/latency/headless. Sau pass mở production rộng có chọn lọc |
| Pha D / **G-D: backend/persistence thật** | G-N pass; thay admission/commit fixtures bằng Spring/PostgreSQL, giữ một writer và cùng domain result | Login → Select → one-time ticket → join; lease/duplicate, checkpoint/SafeAnchor/HP0, một death N recipients và claim/quest commit idempotent; crash/outage/retry trước/sau ACK. Mở gameplay có dữ liệu bền và integration mở rộng |
| Pha C / **G-C: mở content theo dependency** | G-N pass trước art rộng; hệ reward/quest có DB thật G-D trước nghiệm thu route dài | Kiếm S1/S2/S3 tích lũy/passives; Q7–Q12, bảy identities/sáu rigs, maps/route/Linh/Q8/gear đúng GDD. Cung prototype rồi full branch dùng cùng architecture; kiểm art/balance trước nhân variant. Không thêm content cho gap level |
| Pha F / **G-F: Boss và full visual load** | Content/skill/world ổn, schema/lifecycle không đổi lớn; mở Boss/Cung load tests | Lịch/telegraph/Slow/Cuồng, corpse/loot/credit/camera đúng; chơi thật hai người và probe 3–4+ với số máy/build/CPU/bytes/latency. Ghi lại TTK/journey, không coi mô phỏng cũ là acceptance |
| Pha P / **G-P: PvP/chat và ghép hệ online** | G-D trước escrow; movement/combat/recovery đủ ổn | 10 stakes, hai bên escrow, Food/quota, timeout DRAW/forfeit/abort, settlement/crash retry; Map Chat, reconnect, standalone package theo Technical. Online subset pass vẫn chưa có nghĩa full P0 đã xong |
| **G-T: nghiệm thu TARGET** | Các branch Kiếm/Cung và toàn P0 đã ghép; G-L/N/D/C/F/P có evidence liên quan | Đối chiếu toàn GDD §10 và Technical §12 trên hai Client + Dedicated + backend/DB thật. Không ký done khi thiếu Cung, Q12/Boss/PvP hoặc recovery; không dùng video fallback thay kiểm chạy thật |

G-N là gate mạng **tương đối sớm trên production base**, dùng phạm vi ba map/combat đã validate lại; không đợi vẽ hết gear/Bow/Boss rồi mới thử authority. Spike không backend phải mang nhãn fixture dev, không phân phối như final online flow. G-D giữ nội dung architecture spike cũ và là gate bắt buộc trước nhận persistence/reliability là hoàn thành.

<a id="management-window"></a>

# 4. Khung quản lý hai tháng, nhóm bốn người

**Hạn nguồn lực thực tế: hai tháng / bốn người.** Chia thành khoảng tám tuần quản lý từ ngày bắt đầu thực hiện; chưa có ngày bắt đầu hoặc số giờ khả dụng từng người để suy deadline lịch hay tổng person-hours. Các tuần dưới là **mục tiêu quản lý**, không cam kết kỹ thuật. Đầu ra ưu tiên là findings/feel/art/UI + production base/local slice đúng contract rồi G-N sớm, sau đó tích hợp G-D và mở TARGET theo evidence. Không tuyên bố toàn bộ TARGET chắc chắn xong trong hai tháng.

| Tuần mục tiêu | Trọng tâm / đầu ra reviewable | Điều kiện và cách xử lý nếu chưa đạt |
| --- | --- | --- |
| 1 | Harvest prototype, sync feedback; movement/UI/art rig probe và design review base nhỏ | Ghi findings/A01/A02/A12/A14, công sửa/% Free thật; prototype không tự trở thành production |
| 2 | Implement core production base/local room → G-B; movement/input/clock/IDs/definitions/physics/AI/kit | Review one-shot control/transition/cluster, dependency ownership/core tests; chưa nhân full families |
| 3 | Ghép Q1–Q6 Nấm→Sói/Kiếm, art nhỏ và UI → **G-L revision mới** | Manual normal-speed route/feel/usability + automation/retry/death; thiếu art/UX thì PARTIAL, không nối network vào mock |
| 4 | Dedicated build + ít nhất hai clients trên slice → **G-N** | Đây là checkpoint mạng trước production rộng. Nếu phải sửa boundary/headless/recipient, dừng mở rộng content và art để sửa |
| 5 | Spring/PostgreSQL/auth/ticket/lease/commit/recovery → **G-D mục tiêu**; prototype Cung hẹp nếu G-N đã pass | G-D sai thì chưa nhận persistence done. Art Cung chỉ probe A01/A02/A05; phần chưa làm vẫn DEFERRED TARGET P0 |
| 6 | Mở Kiếm Lv10/17, gear/Q7–Q8/Linh và route tiếp theo theo G-C; Cung tiếp tục prototype rồi tích hợp | Review throughput/quest/pose và nguồn lực; ưu tiên ghép một đoạn end-to-end thay nhân toàn bộ catalog chưa kiểm |
| 7 | Mục tiêu tích hợp Boss/recovery/chat; bắt đầu PvP nếu G-D đủ và combat ổn, mở branch Cung đã validate | Phạm vi hoàn thành phụ thuộc gate; ghi từng hệ done/chưa done, giữ TARGET trong backlog, không đổi Cung thành P1 |
| 8 | Regression, standalone package/evidence và review phạm vi đã chạy; **G-T chỉ nếu đủ toàn P0** | Báo rõ slice/online subset đạt tới đâu. Nếu thiếu thời gian, chọn kéo lịch hoặc trade-off cụ thể với chủ dự án; không đổi nghĩa final acceptance |

G-C/F/P có thể ghép hạng mục độc lập sau gate phụ thuộc, không đòi mọi người chờ hết phase. Một người có thể chuẩn bị schema fixture/tests hoặc artwork probe sớm; điều đó không mở production rộng trước G-N và không chứng minh gate đã pass.

| Vai trò chính trong nhóm bốn người | Trách nhiệm và phối hợp |
| --- | --- |
| 1 — Gameplay/authority | Local Session, combat/stat/timeline/lifecycle; thiết kế production base rồi ghép Dedicated cùng người 4; giữ một resolver và clock |
| 2 — World/quest/AI | Blockout/physics/MapExit/SpecialGate, NPC/Q1–Q6, mob/loot/route; thử với người 1, sau gate mới mở maps/quest tiếp |
| 3 — Art/UI presentation | Mẫu art/clean/slice/socket, rig/weapon/terrain và common kit; đo với người 2/4; không cần vẽ toàn bộ catalog trước playable |
| 4 — Integration/QA/backend | Build/log/fixtures, regression hành trình, Dedicated/2-client spike rồi Spring/DB/recovery; hỗ trợ local slice từ tuần 1 |

Đây là phân trách nhiệm để giảm phụ thuộc, không giả mỗi người đã có năng lực tương đương hoặc làm full-time. Mỗi review dùng giờ **khả dụng còn lại** và giờ thật đã tiêu cho gameplay/art/editor/backend/integration/QA/rework. Không nhân bốn người với estimate 160–240 h cũ để gọi thành ngân sách mới.

<a id="art-workflow"></a>

# 5. Art cho người chưa thạo vẽ và kỷ luật giao việc

Dùng quy trình nhỏ trong [Art §23.1](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#art-tool-workflow): chọn mẫu, sửa palette/outline/pivot/grip bằng editor pixel, nhập Unity rồi đo.

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

Lịch tám tuần phía trên là **thứ tự gate lịch sử, không phải lịch hiện hành hoặc cam kết thời gian**. Mốc 160–240 h cũ được tính cho player-host/JSON nên không còn là estimate hợp lệ sau khi thêm Spring Boot, PostgreSQL, auth và dedicated build. Đo riêng giờ backend/schema/migration, Unity server build, integration/recovery và editor/art tại slice đầu rồi lập lại ngân sách; báo trượt milestone hoặc scope P1 minh bạch. Không giảm sáu base rigs / hai class / 26 frames user-lock để ép lịch.

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

<a id="design-lock-sync-audit"></a>

## DESIGN LOCK SYNC — SOURCE → DESTINATION và lịch sử sync

Các replacement sau theo review được user duyệt 2026-10-03; không giữ artifact tạm như authority thứ sáu. Lịch sử simulation/art/accounting trong Analysis/Art/§7–9 vẫn đủ số, chưa nghiệm thu runtime.

| Mã | SOURCE review | DESTINATION canonical | Cách xử lý |
| --- | --- | --- | --- |
| S01 | D01/D12, logical ranged batch | [GDD §3](1_HUYEN_LO_GDD.md#class-combat), [Technical timeline](2_HUYEN_LO_TECHNICAL.md#combat-data) | REPLACE flight/collision damage; giữ geometry/power |
| S02 | D02/D03, focus independent facing | [GDD focus](1_HUYEN_LO_GDD.md#focus-input), [Technical shared input](2_HUYEN_LO_TECHNICAL.md#shared-combat-input) | AUTO context-sticky; EXPLICIT pinned; range khác acquire |
| S03 | D04/D09/D28, control V6.2.0 superseded bằng F03 one-press | [Technical shared input](2_HUYEN_LO_TECHNICAL.md#shared-combat-input) | Một pipeline/lock/buffer; pending one-shot, mọi skill không repeat |
| S04 | D27/D29, accumulated slots/IDs | [GDD §3](1_HUYEN_LO_GDD.md#class-combat), [Technical §3](2_HUYEN_LO_TECHNICAL.md#combat-data) | Sáu IDs/CD riêng, sáu books/bốn passives giữ |
| S05 | D05, Q5 Novice compatibility | [GDD §2](1_HUYEN_LO_GDD.md#character-power) / [§3](1_HUYEN_LO_GDD.md#class-combat) | Novice trước class kể cả Lv5; không class fourth action |
| S06 | D06/D08/D10, reaction/air | [GDD focus/input](1_HUYEN_LO_GDD.md#focus-input), [Art §1](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#player-visual) | Gravity/momentum; no normal Hurt/knockback/recovery cancel |
| S07 | D14/D15R/D17, prototype boundaries | [Analysis rationale](3_HUYEN_LO_DESIGN_ANALYSIS.md#design-lock-rationale) | Hybrid 3/1/0, exact unreachable/LoS A-B chưa khóa |
| S08 | D22, minimal target and slot UI | [Art §19](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#icons-ui), [GDD §9](1_HUYEN_LO_GDD.md#ux-art) | World marker/mini HP; screen name/level/current-max; ba slot |
| S09 | D30, Bow AAA/resource audit | [Analysis role audit](3_HUYEN_LO_DESIGN_ANALYSIS.md#design-lock-rationale) | Giữ TUNABLE numbers, rerun legacy models |
| S10 | D24, art frame/accounting | [Art §1](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#player-visual) / [§23](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#production-accounting) | Giữ 26 frame OPEN/count scenarios, không duyệt 33 pose |
| S11 | Interaction consistency mới | [GDD focus](1_HUYEN_LO_GDD.md#focus-input), [Technical input](2_HUYEN_LO_TECHNICAL.md#shared-combat-input) | E act candidate ngay, loot không steal focus |
| S12 | D26, CURRENT stop gate | [VS-1](#vs-1), [phase gates](#phase-gates) | Chỉ Q1–Q6/ba map/Tân Lữ→Kiếm local, không tự mở G-N |

CURRENT control acceptance V6.2.1: A/D+arrows OR; Space/↑ jump và S/↓ drop; 1–3 select/one-press approach+cast, no J/no-repeat mọi skill, locked slots2/3 trong route. One-action/snapshot/latest buffer, AUTO/EXPLICIT, pending không đổi target; release giữ one-shot, manual/focus/UI/Esc/map cancel. E candidate độc lập; EdgeExit auto không E. Manual feel/usability và revision-matched evidence bắt buộc. S2/S3/Cung production DEFERRED. Không tăng số balance để làm button đẹp; giữ role/sustain gate.

<a id="editorial-source-destination"></a>

## Keyboard prototype và operational art — 2026-10-04

Mock project ở `prototypes/VS1_EndToEnd/`, tương lai `game/` là Unity production Client/Dedicated, `backend/` là Spring/PostgreSQL; chưa dựng các codebase đó. Prototype tiếp tục dùng để thử nhanh, không mở G-B/G-N chỉ vì build mới chạy. Debug mốc/reset chỉ cho dev, route acceptance bắt đầu fresh và không dùng preset. Manual keyboard usability/feel và rig/art import vẫn cần người chơi review ở tốc độ thường.

| SOURCE | DESTINATION | Nội dung thực |
| --- | --- | --- |
| Review §2/§4 | [First Art Probe](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#first-art-probe) | Tám module/pose probe, source→export→import→runtime, pass/fail |
| Review §3/§5/§6 | [Art setup](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#first-art-probe), [DoD](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#art-validation) | Minimum manifest, socket A/B, asset QA |
| Review §7/§8 | [Art setup](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#first-art-probe) | Mini style sample và provenance |
| Review §9/§10/§15/§16 | [Art](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#working-spec-end), [Analysis](3_HUYEN_LO_DESIGN_ANALYSIS.md#keyboard-prototype-review) | Version hiện hành, base trước mạng, numbering và history marker |
| Review §12 | [GDD quest](1_HUYEN_LO_GDD.md#quests-story), [Technical quest](2_HUYEN_LO_TECHNICAL.md#combat-data) | Active-step-only tutorial supply, không future entitlement |
| User keyboard/debug/mock | [GDD UX](1_HUYEN_LO_GDD.md#ux-art), [Technical input](2_HUYEN_LO_TECHNICAL.md#shared-combat-input), [prototype](../../prototypes/VS1_EndToEnd/README.md) | Menu keyboard/mouse cùng command, debug mốc/reset, project tách biệt |

<a id="feedback-source-destination"></a>

## Feedback → canonical và migration prototype

Mốc trước sửa `46006c4`; request feedback 2026-10-03 cho phép sửa tạm và tự xử lý inconsistency. Các đích dưới là nội dung thực, không copy feedback thành authority thứ sáu. Các quyết định numeric/feel chưa có evidence giữ TUNABLE/PROTOTYPE ở Analysis.

| Mã | SOURCE feedback | DESTINATION | Xử lý |
| --- | --- | --- | --- |
| F01 | Nấm trước Sói/catch-up | [GDD quest](1_HUYEN_LO_GDD.md#quests-story), [Analysis matrix](3_HUYEN_LO_DESIGN_ANALYSIS.md#farm-progression) | Q3 Lv3, Q4 Nấm→Lv4, Q5 Sói→Lv5; QIds/supply/recovery đồng bộ |
| F02 | Portal thường bị dùng rộng | [GDD world](1_HUYEN_LO_GDD.md#world-farm), [Technical world](2_HUYEN_LO_TECHNICAL.md#maps) | EdgeExit auto + SpecialGate, validation/checkpoint/dedup/ping-pong |
| F03 | Bỏ J/hold, tap thông minh | [GDD input](1_HUYEN_LO_GDD.md#focus-input), [Technical input](2_HUYEN_LO_TECHNICAL.md#shared-combat-input) | One-shot pending/approach, no-repeat/no-cost/cancel/expiry/revalidate |
| F04 | Jump/drop mapping/feel | [GDD UX](1_HUYEN_LO_GDD.md#ux-art), [Technical movement](2_HUYEN_LO_TECHNICAL.md#movement-feel) | OR aliases; coyote/buffer/variable-height/acceleration chưa khóa số |
| F05 | Melee pile/reposition | [GDD mob](1_HUYEN_LO_GDD.md#world-farm), [Technical AI](2_HUYEN_LO_TECHNICAL.md#combat-data) | Cluster đọc được, soft separation/recovery offsets; không lock ring slots |
| F06 | Prototype không codebase | [Technical discipline](2_HUYEN_LO_TECHNICAL.md#architecture-discipline), [Roadmap gates](#phase-gates), [prototype README](../../prototypes/VS1_EndToEnd/README.md) | Tách folder giữ meta; evidence cũ không pass revision mới; G-B trước G-N |
| F07 | Q3 waiting | [GDD quest](1_HUYEN_LO_GDD.md#quests-story), [Art Dummy](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#mob-visual) | ≥3 placements cùng pool; HP60/25s giữ, contention OPEN |
| F08 | UI keys/usability/navigation | [Art UI](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#icons-ui), [GDD UX](1_HUYEN_LO_GDD.md#ux-art) | I/C/Q default, edge arrow/name + NPC marker P0, manual UX review |
| F09 | Class Normal reasoning stale | [Analysis feedback](3_HUYEN_LO_DESIGN_ANALYSIS.md#prototype-feedback-review), [legacy timing](3_HUYEN_LO_DESIGN_ANALYSIS.md#balance-baselines) | Giữ bảng số, gắn LEGACY/SUPERSEDED trực tiếp reasoning; không restore Normal |

Script đếm trước/sau lines/pipe rows/headings/critical values và hash ở `prototypes/VS1_EndToEnd/PrototypeEvidence/VS1_EndToEnd/verify_feedback.py`; baseline từ commit checkpoint, audit là documentation evidence của lượt này, tách build/test/video cũ. Lấy mẫu ngẫu nhiên 8 mục và kiểm snippet đích thực; không dùng agent đếm. Không xóa bảng combat/gear/journey/Boss/art scenarios để “cleanup”.

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


<a id="prototype-visual-review"></a>

# 10. Hồ sơ bản mẫu — không phải luật production

Mục này sở hữu layout/tọa độ mock, lỗi runtime, fixtures, capture và thông số thử. Docs 1–4 giữ luật, contract, reasoning balance và production visual requirements; không lấy một lần sửa mock để khóa design mới. V6.2.4 bị user từ chối về địa hình dù tests/route pass; các hình và mô tả cũ dưới đây chỉ là lịch sử, **không mô tả bản hiện tại và không phải visual acceptance**.

## V6.2.5 — sửa theo feedback địa hình

- Thay diamond silhouettes/khối cây rải đều bằng hai lớp đất bậc liền, cao dần phía sau tuyến chính. Cây tre đặt theo mặt đất của từng lớp; hậu cảnh không collision, không dùng để đánh lừa có sàn đứng.
- DS5 chuyển lên bờ khô x58/60, activity nằm trong support x56…64. Giữ ID/slot/stat, DS1–DS6/10 slots và quest credit; PROBE7/8 vẫn riêng mock; PROBE8 centreY4,05 theo mặt sàn3,4 + offset0,65 để chân không bị sàn che. Không đặt quái hay props khô dưới lòng nước.
- Lòng suối x66…80 có một vùng nước liên tục, đáy −2 u/mặt −1,45 u. Nguồn thác ở mép lớp đất sau, x66,325/top3,5, nối xuống cùng mặt nước. Cầu x64…82/top0,1 đi khô; tốc độ chân chạm nước thử ×0,85, không stack. Thác/ripple cosmetic, không water physics.
- Cung đường có sàn gỗ thật top2,8 u; Diệp Lam đứng trên gác (centreY3,6), tiếp cận bằng jump và E/chuột đúng range. Phong Du/nhiệm vụ Kiếm giữ tầng chính. Nhà trưởng lão (sàn3 u)/lò rèn (sàn3,125 u), Cung đường/võ đường tây có gác thật, cửa/cột/dầm/mái đọc được; nhà không có deck chỉ vẽ một tầng. Không vẽ dầm tầng giả hoặc mái trang trí như sàn đứng.

Đây là authoring probe, không cập nhật manifest map production. Không đổi stats/balance hoặc khóa spacing/grace chỉ từ hình tham khảo. Kiểm route tự động, chân chạm nước/cầu, quái trên nền khô, gác NPC và render thực; người chơi vẫn cần đánh giá chất lượng/feel ở tốc độ thường.

**Kết quả kiểm V6.2.5:** 57 EditMode + 7 PlayMode pass; Linux Development build thành công, fresh Q1–Q6 route PASS bằng Rigidbody2D/menu keyboard adapter ở timeScale4. Capture bên dưới là input bàn phím thực ở 1×: Vân Khê fresh start, Học Viện dùng SwordTraining, Đồng Sương dùng Crowd có nhãn DEBUG. Đã kiểm nối nguồn/thác/suối, Sói trên nền khô, cầu khô/lội nước và E/Esc trên gác Diệp Lam. Không phải fresh journey playtest 1×, user visual approval, online capacity hoặc production art acceptance; **G-L vẫn PARTIAL**.

[Evidence/build/hash](../../prototypes/VS1_EndToEnd/PrototypeEvidence/V6_2_5/validation.json) · [Audit số dòng/bảng/heading/chỉ số](../../prototypes/VS1_EndToEnd/PrototypeEvidence/V6_2_5/document-audit.json) · [8 mục SOURCE → DESTINATION lấy mẫu bằng script](../../prototypes/VS1_EndToEnd/PrototypeEvidence/V6_2_5/source-destination-sample.json).

Ảnh build hiện tại: [Vân Khê](../../prototypes/VS1_EndToEnd/PrototypeEvidence/V6_2_5/village.png) · [Diệp Lam trên gác thật](../../prototypes/VS1_EndToEnd/PrototypeEvidence/V6_2_5/academy-floor.png) · [Thác, suối và cầu](../../prototypes/VS1_EndToEnd/PrototypeEvidence/V6_2_5/mist-stream.png) · [Lội nước dưới cầu](../../prototypes/VS1_EndToEnd/PrototypeEvidence/V6_2_5/mist-wading.png) · [Sói trên bờ khô và sàn cao](../../prototypes/VS1_EndToEnd/PrototypeEvidence/V6_2_5/dry-pack.png).

<a id="prototype-technical-history"></a>

## Technical — lịch sử probe V6.2.4

**Melee lane probe V6.2.4:** trace target → desired X → occupied/blocked → velocity → actual range/windup. V6.2.3 rank offset + peer clipping đã làm Sói sau kẹt ngoài range; không giữ hành vi chờ con đầu chết. Probe mới dùng recovery/cross goal có chủ đích + bounded neighbor correction trong một lần tích phân velocity; không shove sau chase hoặc hard body blocker. Cắn giữ definition range/interval; windup origin/facing khóa. Recovery goals nằm trong support lane/home bounds, có phase lệch mỗi life và chọn phía đủ chỗ tại mép. Brief crossings hợp lệ vì no body blocking; không persistent blob, không attack token/formation. Test 1/2/4, tường/mép/khác cao độ và player đổi hướng; local pass không suy capacity online.

**Blockout V6.2.4:** dữ liệu `BlockoutLayout` của mock tách solid terrain profile/wooden one-way/bounds khỏi drawing; GroundTop và support span cấp home/lane cho mob, không đặt mob lơ lửng trên cao độ cũ; reciprocal exit links + destination spawn nằm đúng mép, camera snap chỉ khi transition/reset. Physics body tiếp tục FixedUpdate, camera bám interpolated visual Transform tại LateUpdate với dead zone/damping/clamp; mob logical positions giữ previous/current cho visual interpolation, không snap simulation để cứu render. [Unity — Rigidbody2D interpolation](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Rigidbody2D-interpolation.html) mô tả smoothing giữa physics updates; đây là căn cứ probe, không bảo đảm cảm giác mượt trên mọi máy.

**Pack activity / patrol probe:** cả pocket dùng ActivityMin/Max chung, nằm trong support span; không từng con bị kẹt ở home clamp khác nhau. Chỉ start bite khi player trong vùng. Ngoài vùng/không reachable áp grace hiện có rồi Return; reset HP/contribution, không tạo life/reward/reroll. Sau return, goal patrol gần home có pause/phase riêng; cùng một bounded velocity integration. Windup đã bắt đầu vẫn resolve/cancel theo clock, không trì hoãn tới khi player quay lại. Exact bounds/speed/policy chỉ dữ liệu mock.

**Water contact:** `BlockoutLayout.Waters` giữ bounds/bottom/level/factor; locomotion kiểm feet contact, nhân speed factor tối thiểu một lần. Collider là đáy đất/cầu hiện hành, nước không có fluid physics. Bridge/air phải dry; front overlay chỉ che phần thấp, ripple cosmetic dùng cùng contact predicate, không là authority của slow.

**UI event boundary:** command có thể đổi panel và rebuild actions; mouse callback phải kết thúc IMGUI event ngay sau mutation (`GUIUtility.ExitGUI`), không tiếp tục index sáu slot cũ. Selection ID, breadcrumb và validation vẫn dùng chung keyboard/mouse; không catch/swallow lỗi list để che bug. Auto-close chỉ sau domain command thành công, batch services giữ context; Esc Back thống nhất cả input adapter và route driver.

**Mock/dev boundary:** Unity project tham khảo nằm tại `prototypes/VS1_EndToEnd/`; production `game/` dự kiến dùng chung rules/content cho Client và Dedicated build, `backend/` dành Spring/PostgreSQL. Chưa scaffold hai phần production. Debug menu chỉ Editor/Development build, tạo session fixture mới từ mốc Q1–Q6/sân tập và reset đủ inventory/receipts/clock/food/cooldown/loot/focus. Preset có nhãn DEBUG, không dùng trong route acceptance. Chọn menu mốc thay save JSON; JSON hiện chỉ config/fixture/evidence, production persistence vẫn PostgreSQL.

<a id="prototype-feedback-history"></a>

## Analysis — lịch sử feedback V6.2.4

**Feedback V6.2.4 — CURRENT PROBE:** mock trước gộp người hướng dẫn phái và đảo mép Học Viện, hai sai lệch đã được đưa về đúng contract. User yêu cầu manual unequip trước chọn class; reject không grant/đổi class. V6.2.3 có lỗi rank/peer clipping làm Sói sau kẹt; V6.2.4 thay bằng recovery/cross goals + bounded neighbor correction, trace eligibility và kiểm cả bốn có windup mà không phải giết con trước; không tăng stat/giảm interval. Đồng Sương mock giữ DS1–DS6/10 slots, thêm PROBE7/4 và PROBE8/3 ở hai lane riêng để kiểm crowd/vertical; 17 slots là workload mock, không thay budget release hoặc capacity promise. PROBE không credit Q5, vẫn level-gap loot thường. Terrain dùng khối solid bậc lớn, ít sàn gỗ one-way, nước nông giảm tốc nhẹ theo direction mới của user, thác/ripple cosmetic. RPG shell năm tab và geometric rig là UX/visual probe, không nghiệm thu art production. Bảng TTK/farm/hành trình trước chưa tính topology/crowd mới; vẫn là lịch sử đối chiếu, cần rerun khi gate tương ứng.

**Feedback nối tiếp:** hai ảnh tham khảo cung cấp terrain grammar (khối đất liền, terraces, basin, cầu/thác), không scale pixel hoặc yêu cầu sao chép asset. Mock tăng depth basin và thử ×0,85 water contact; chưa đo ảnh hưởng travel/journey/endgame và không retune EXP/TTK từ đó. Shared activity bounds + return→patrol giải quyết đứng mãi ở mép; giữ combat hiện tại. Lỗi mouse equipment do action list đổi giữa render, khác với keyboard logic PASS; cần kiểm click thật trên build bên cạnh tests. Menu close policy tách terminal single action khỏi batch services. Review 1× phát hiện tracker Q1 chỉ sai sang cổng Đồng Sương; route hint nay theo step destination/current map, kiểm cả bước quay về NPC.

<a id="prototype-map-history"></a>

## Art — sơ đồ map cũ bị từ chối

### 13.1. Map blockout VS-1 V6.2.4 — CURRENT PROBE

Học Viện ở phía tây Vân Khê, Đồng Sương ở phía đông. Đi qua mép trái hub xuất hiện ở mép phải Academy (x31, trigger trả về x34); quay phải thì vào mép trái Village. Village x30 → Mist x−1, Mist x−4 → Village x27. Spawn ngoài trigger ngược, velocity reset và camera snap một lần; không tween body rơi từ giữa không trung. Hướng địa lý là contract; tọa độ dưới chỉ dữ liệu mock, không release lock.

Blockout có ba lớp đọc được: silhouette trời/núi xa không collider/parallax nhẹ; props/công trình phía giữa xuyên được có màu chìm; solid đất/đá có mặt cỏ sáng và khối chân liền, sàn gỗ one-way mỏng có vân/nẹp riêng. Main elevation đến từ terrace/step/hố nông liên tục, không platform spam. Solid bắt buộc nhảy/đi vòng, S chỉ xuyên đúng sàn gỗ; không tắt collision terrain để cứu route.

Vân Khê có quảng trường Lâm Bá/spawn, nhà thuốc x6, lò rèn trên terrace x13, quán nghỉ/rương x20, đài Tạ Minh x26; đường chính dễ đọc, gác chợ là optional route. Academy Phong Du x0/Diệp Lam x−12 tách hai khu, sân Dummy x22–26 cách cổng; dojo tây có bậc solid + gác phụ, Q2 giữ deck gỗ x8 để dạy jump/drop. Mist có DS3 terrace, DS4 valley, DS5 depression, DS6 clearing và ridge PROBE7; PROBE8 là cầu gỗ upper route. Bãi quest chính không parkour bắt buộc; cliff/step thấp vượt bằng jump thường. Essential services có đường mặt đất, không cần trèo gác.

Hai ảnh user được áp dụng ở **silhouette và traversal**: earth mass chạy xuống dưới viewport, grass cap và bờ bậc nối liên tục; west dojo terrace cao 4 u, DS3 plateau 1,8 u, đáy DS5 −2 u. Bridge là route khô trên nước rộng, đường đáy vẫn đi được; không thay Q1–Q6 hoặc thêm hazard. Waterfall nhỏ bám bờ suối, không floating ledge giả. Profile và NPC services giữ đường chính rõ; số liệu là mock authoring.

Đồng Sương vẫn DS1–DS6/10 slots canonical + PROBE7/4 và PROBE8/3 trong mock, 17 slots không tăng release budget. Home Y/lane derive từ support thật; không cho wolf logical movement đi xuyên cliff. Các tọa độ/kích thước chỉ CURRENT PROBE, không production lock.

Sáu pocket DS1–DS6 giữ 10 slots và respawn baseline. **Riêng mock** thêm PROBE7: bốn Sói dưới x106…110,5; PROBE8: ba Sói trên x51…54/centreY3,85, lane đi được x49,5…58,5, mặt sàn3,4. Cộng 17 slots; không thay manifest release và không credit Q5. Level/stat/loot/25 s hiện hành, không per-player spawn. Extra pocket tách main route bằng spacing/cao độ; validate reachable melee/jump/drop và Return trước nhân map. Local nhiều quái hơn không chứng minh online capacity.

<div style="max-width:860px;background:#ffffff;border-left:4px solid #0043ce;padding:18px;color:#1e1e1e"><p>Khối xám là solid terrain; xanh đứt là deck/cầu one-way; xanh nhạt là nước trên đáy đi được.</p><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 820 450" role="img" aria-label="Actual mock terrain profiles with walkable basins and bridges"><text x="20" y="22" fill="#0043ce" font-size="14">VÂN KHÊ · BASIN NÔNG / GÁC CHỢ</text><rect x="20.0" y="75.0" width="31.2" height="50.0" fill="#787878"/><path d="M20.0,75.0h31.2" stroke="#1e1e1e" stroke-width="2"/><rect x="51.2" y="85.0" width="15.6" height="40.0" fill="#787878"/><path d="M51.2,85.0h15.6" stroke="#1e1e1e" stroke-width="2"/><rect x="66.8" y="90.0" width="15.6" height="35.0" fill="#787878"/><path d="M66.8,90.0h15.6" stroke="#1e1e1e" stroke-width="2"/><rect x="82.4" y="95.0" width="131.0" height="30.0" fill="#787878"/><path d="M82.4,95.0h131.0" stroke="#1e1e1e" stroke-width="2"/><rect x="213.4" y="98.5" width="25.0" height="26.5" fill="#787878"/><path d="M213.4,98.5h25.0" stroke="#1e1e1e" stroke-width="2"/><rect x="238.4" y="95.0" width="62.4" height="30.0" fill="#787878"/><path d="M238.4,95.0h62.4" stroke="#1e1e1e" stroke-width="2"/><rect x="300.8" y="92.0" width="15.6" height="33.0" fill="#787878"/><path d="M300.8,92.0h15.6" stroke="#1e1e1e" stroke-width="2"/><rect x="316.4" y="89.0" width="124.8" height="36.0" fill="#787878"/><path d="M316.4,89.0h124.8" stroke="#1e1e1e" stroke-width="2"/><rect x="441.2" y="92.0" width="15.6" height="33.0" fill="#787878"/><path d="M441.2,92.0h15.6" stroke="#1e1e1e" stroke-width="2"/><rect x="456.8" y="95.0" width="218.4" height="30.0" fill="#787878"/><path d="M456.8,95.0h218.4" stroke="#1e1e1e" stroke-width="2"/><rect x="675.2" y="85.0" width="124.8" height="40.0" fill="#787878"/><path d="M675.2,85.0h124.8" stroke="#1e1e1e" stroke-width="2"/><path d="M300.8,63.8h93.6" stroke="#0043ce" stroke-width="4" stroke-dasharray="7 3"/><rect x="213.4" y="95.0" width="25.0" height="3.5" fill="#c8d4ed"/><path d="M213.4,95.0h25.0" stroke="#0043ce"/><text x="20" y="138" fill="#4a4a4a" font-size="11">Hub: services ở đường chính; nước nằm trong đáy đất thật.</text><text x="20" y="162" fill="#0043ce" font-size="14">HỌC VIỆN · ĐÀI VÕ / SÂN TẬP</text><rect x="20.0" y="195.0" width="66.9" height="70.0" fill="#787878"/><path d="M20.0,195.0h66.9" stroke="#1e1e1e" stroke-width="2"/><rect x="86.9" y="205.0" width="22.3" height="60.0" fill="#787878"/><path d="M86.9,205.0h22.3" stroke="#1e1e1e" stroke-width="2"/><rect x="109.1" y="215.0" width="22.3" height="50.0" fill="#787878"/><path d="M109.1,215.0h22.3" stroke="#1e1e1e" stroke-width="2"/><rect x="131.4" y="225.0" width="22.3" height="40.0" fill="#787878"/><path d="M131.4,225.0h22.3" stroke="#1e1e1e" stroke-width="2"/><rect x="153.7" y="235.0" width="334.3" height="30.0" fill="#787878"/><path d="M153.7,235.0h334.3" stroke="#1e1e1e" stroke-width="2"/><rect x="488.0" y="231.0" width="22.3" height="34.0" fill="#787878"/><path d="M488.0,231.0h22.3" stroke="#1e1e1e" stroke-width="2"/><rect x="510.3" y="227.0" width="33.4" height="38.0" fill="#787878"/><path d="M510.3,227.0h33.4" stroke="#1e1e1e" stroke-width="2"/><rect x="543.7" y="231.0" width="22.3" height="34.0" fill="#787878"/><path d="M543.7,231.0h22.3" stroke="#1e1e1e" stroke-width="2"/><rect x="566.0" y="235.0" width="234.0" height="30.0" fill="#787878"/><path d="M566.0,235.0h234.0" stroke="#1e1e1e" stroke-width="2"/><path d="M421.1,205.0h44.6" stroke="#0043ce" stroke-width="4" stroke-dasharray="7 3"/><path d="M25.6,172.8h55.7" stroke="#0043ce" stroke-width="4" stroke-dasharray="7 3"/><text x="20" y="278" fill="#4a4a4a" font-size="11">Dojo cao 4 u phía tây; Q2 deck x8 và sân Dummy phía đông.</text><text x="20" y="302" fill="#0043ce" font-size="14">ĐỒNG SƯƠNG · TERRACE / LÒNG SUỐI / CẦU</text><rect x="20.0" y="375.0" width="153.8" height="30.0" fill="#787878"/><path d="M20.0,375.0h153.8" stroke="#1e1e1e" stroke-width="2"/><rect x="173.8" y="369.0" width="11.0" height="36.0" fill="#787878"/><path d="M173.8,369.0h11.0" stroke="#1e1e1e" stroke-width="2"/><rect x="184.8" y="363.0" width="11.0" height="42.0" fill="#787878"/><path d="M184.8,363.0h11.0" stroke="#1e1e1e" stroke-width="2"/><rect x="195.8" y="357.0" width="87.9" height="48.0" fill="#787878"/><path d="M195.8,357.0h87.9" stroke="#1e1e1e" stroke-width="2"/><rect x="283.7" y="363.0" width="16.5" height="42.0" fill="#787878"/><path d="M283.7,363.0h16.5" stroke="#1e1e1e" stroke-width="2"/><rect x="300.1" y="369.0" width="16.5" height="36.0" fill="#787878"/><path d="M300.1,369.0h16.5" stroke="#1e1e1e" stroke-width="2"/><rect x="316.6" y="375.0" width="87.9" height="30.0" fill="#787878"/><path d="M316.6,375.0h87.9" stroke="#1e1e1e" stroke-width="2"/><rect x="404.5" y="380.0" width="11.0" height="25.0" fill="#787878"/><path d="M404.5,380.0h11.0" stroke="#1e1e1e" stroke-width="2"/><rect x="415.5" y="387.5" width="11.0" height="17.5" fill="#787878"/><path d="M415.5,387.5h11.0" stroke="#1e1e1e" stroke-width="2"/><rect x="426.5" y="395.0" width="87.9" height="10.0" fill="#787878"/><path d="M426.5,395.0h87.9" stroke="#1e1e1e" stroke-width="2"/><rect x="514.4" y="387.5" width="11.0" height="17.5" fill="#787878"/><path d="M514.4,387.5h11.0" stroke="#1e1e1e" stroke-width="2"/><rect x="525.4" y="380.0" width="22.0" height="25.0" fill="#787878"/><path d="M525.4,380.0h22.0" stroke="#1e1e1e" stroke-width="2"/><rect x="547.3" y="375.0" width="76.9" height="30.0" fill="#787878"/><path d="M547.3,375.0h76.9" stroke="#1e1e1e" stroke-width="2"/><rect x="624.2" y="371.0" width="22.0" height="34.0" fill="#787878"/><path d="M624.2,371.0h22.0" stroke="#1e1e1e" stroke-width="2"/><rect x="646.2" y="367.0" width="98.9" height="38.0" fill="#787878"/><path d="M646.2,367.0h98.9" stroke="#1e1e1e" stroke-width="2"/><rect x="745.1" y="375.0" width="54.9" height="30.0" fill="#787878"/><path d="M745.1,375.0h54.9" stroke="#1e1e1e" stroke-width="2"/><path d="M344.1,341.0h54.9" stroke="#0043ce" stroke-width="4" stroke-dasharray="7 3"/><path d="M415.5,374.0h120.8" stroke="#0043ce" stroke-width="4" stroke-dasharray="7 3"/><rect x="426.5" y="390.5" width="27.5" height="4.5" fill="#c8d4ed"/><path d="M426.5,390.5h27.5" stroke="#0043ce"/><rect x="464.9" y="389.5" width="49.4" height="5.5" fill="#c8d4ed"/><path d="M464.9,389.5h49.4" stroke="#0043ce"/><text x="20" y="418" fill="#4a4a4a" font-size="11">DS3 +1,8 u → DS4 0 → DS5 −2 u → DS6 0; cầu qua nước rộng.</text></svg><p>Profile từ BlockoutLayout của mock. Scale X khác mỗi map; tọa độ/tuning không khóa production.</p></div>

[PNG sơ đồ map đã render](../../prototypes/VS1_EndToEnd/PrototypeEvidence/V6_2_4/map-layout.png).

Ca kiểm: reciprocal exits giữ hướng/outside reverse trigger; Q2 thực sự đứng ledge rồi drop; Q4/5 không bị optional route chặn; Kiếm nhảy tới PROBE8 qua bậc; crowd2/3/4 giữ silhouette riêng, không steering đổi windup; camera không bám từng fixed tick hoặc rung theo bước landing. Chạy cả normal speed và route automation; chưa pass manual thì ghi G-L PARTIAL.

<a id="prototype-ui-history"></a>

## Art — hồ sơ UI/rig mock

### 19.1. NPC, bag grid và equipment view — CURRENT PROBE

NPC root hiện lời nói mộc mạc, quest action đúng state và các chức năng của chính NPC; chọn Mua/Bán/Gửi/Lấy mới mở danh sách đó. `!` là quest Available hoặc nhập phái đúng step; `?` là Ready, `…` là đang làm. Không đánh dấu Diệp Lam “sẵn để chọn Cung” nếu nhánh mock chưa mở. Khi Q6 đang có Mộc Kiếm, Phong Du nói rõ C → Trang bị → Vũ khí → Tháo; action class khóa đến khi ô trống. Không auto-unequip thưởng.

Bag 30 ô dựng 6 cột ×5 hàng, icon + count + selected border; một bảng bên phải hiện tên/type/band/quality/stat/binding. Enter/Interact vào thao tác món; arrow chọn ô, Tab tuyến tính, Esc về grid giữ focus theo instance nếu còn. View equipment ở C → Trang bị có hình nam tạm ở giữa và sáu slot quanh; slot trống vẫn đọc được, slot có đồ mở detail/tháo. Không lặp toàn bộ action trang bị ở bag. Mock icons dùng khối/motif + nhãn; art final vẫn ItemDefinition visual/icon refs, rarity/stack là overlay.

**RPG UX probe V6.2.4:** năm tab đổi trực tiếp bằng Tab/Shift+Tab hoặc click, arrows giữ navigation trong grid/action list. I/C/Q mở đúng view; selected action ID được giữ khi đổi tab, không callback vào item cũ. Bag có icon/stack/detail + dùng/trang bị nhanh + so món đang mặc; slot equipment mở bag lọc đúng slot, cả slot trống. Phân điểm riêng Thuộc tính, derived stats riêng Thông số; không bắt đóng menu để chuyển view.

**Geometric rig:** BodyBase/Hair/Armor/LowerBody/Weapon dùng một module/pose/socket description cho world và equipment preview. Áo, Quần, Wood/Sword đổi silhouette/màu thật, grip và weapon angle theo action clock; walking sway cosmetic. Đây là layering probe, chưa art production hoặc chốt nghĩa 26 frame.

**Normal HUD:** ba slot bottom-center có key/icon/selected/locked/CD/MP, H/M/F bên cạnh. DEV/F8 nhỏ riêng, preset vẫn có nhãn; không để menu debug định layout player HUD. Q1–Q6 có nhận/trả và phản hồi trung gian, marker/name/speech ở các cao độ tách nhau.

**Water probe — feedback mới:** hub có basin nông, Mist có đáy suối thấp hơn main route 2 u, shallow pool và vùng nước rộng dưới cầu. WaterBase + FrontOverlay theo volume thật, chỉ che chân/phần thấp; waterfall nhỏ sau lane + mặt nước/splash readable. Feet chạm nước thử speed ×0,85, bridge/air dry; ripple cosmetic tối đa một lần/0,25 s. Không water collider/swimming/hazard; full Bạch waterfall production vẫn deferred. Đây thay water cosmetic-only của mock trước, theo user; depth/factor/tọa độ không production lock.

[Capture blockout chạy thật](../../prototypes/VS1_EndToEnd/PrototypeEvidence/V6_2_4/basin-runtime.png) · [Trang bị sau mouse unequip](../../prototypes/VS1_EndToEnd/PrototypeEvidence/V6_2_4/equipment-runtime.png). Đây là fixture 1×, không evidence fresh journey hoặc production art.


<div style="max-width:860px;background:#ffffff;border-left:4px solid #0043ce;padding:18px;color:#1e1e1e"><p>RPG shell chung, năm view riêng; thao tác keyboard/mouse gọi cùng command.</p><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 820 300" role="img" aria-label="RPG tab shell">
<rect x="10" y="10" width="800" height="275" fill="#e8e8e8" stroke="#787878"/>
<rect x="20" y="20" width="152" height="36" fill="#c8d4ed" stroke="#003bb5"/>
<text x="32" y="44" fill="#1e1e1e">I · Hành trang</text><text x="185" y="44" fill="#1e1e1e">C · Trang bị</text><text x="340" y="44" fill="#1e1e1e">Thuộc tính</text><text x="495" y="44" fill="#1e1e1e">Thông số</text><text x="650" y="44" fill="#1e1e1e">Kỹ năng</text>
<rect x="24" y="75" width="330" height="162" fill="#ffffff" stroke="#787878"/><path d="M79,75v162M134,75v162M189,75v162M244,75v162M299,75v162M24,129h330M24,183h330" stroke="#787878"/>
<text x="30" y="264" fill="#4a4a4a">Grid icon / stack · selected ID ổn định</text>
<rect x="380" y="75" width="400" height="162" fill="#ffffff" stroke="#787878"/>
<text x="398" y="101" fill="#0043ce">Tên / chỉ số / đang mặc</text><text x="398" y="133" fill="#1e1e1e">Trang bị / Dùng / Học + reason</text><text x="398" y="165" fill="#1e1e1e">Slot → bag lọc đúng GearSlot</text><text x="398" y="197" fill="#1e1e1e">Điểm chưa dùng → Thuộc tính</text><text x="380" y="264" fill="#4a4a4a">Tab / Shift+Tab · arrows · Enter/E · Esc</text></svg><p>Equipment: rig giữa sáu slot. Attributes: STR/VIT/INT/AGI. Stats: derived evaluator. Q mở Quest riêng; NPC mở từng dịch vụ.</p></div>

[PNG bố cục UI đã render](../../prototypes/VS1_EndToEnd/PrototypeEvidence/V6_2_4/ui-layout.png).

DoD của view: hoàn tất Q1–Q6 không click, submenu không lọt movement/skill, disabled reason đọc được, consume/equip không giữ callback item cũ, reset xóa breadcrumb. Kiểm text Việt không crop ở720p và resize; 30/40 ô inventory/storage không tự biến thành hàng trăm actions trong NPC root. Icon/rig final và khả năng dùng nhanh bằng người thật còn qua art/UI gate.

<a id="prototype-water-history"></a>

## GDD — lịch sử tuning nước, đã tách khỏi luật

**Nước nông — direction mới của user:** basin có đáy solid đi được, mặt nước cao hơn đáy để nhân vật chìm phần chân khi lội; giảm nhẹ tốc di chuyển khi chân trong nước. Mock thử hệ số ×0,85, không stack nhiều water volumes; exact depth/factor là TUNABLE. Đi trên cầu hoặc nhảy ra khỏi mặt nước không giảm tốc. Vùng nước rộng có route cầu rõ, không buộc bơi; thác/flow/ripple chỉ presentation. Không swimming, breath, buoyancy hoặc water damage. Luật này thay baseline water visual-only trước feedback; không tự coi water slowdown là trạng thái Băng Hàn.

<a id="prototype-tooling-history"></a>

## Technical — tooling của prototype

**Unity / tooling:** prototype hiện có dùng Editor thực cài 6000.5.9f1; production base chưa được scaffold. Pin ProjectVersion/manifest/lock khi dựng base bằng Editor, không gọi bản này là LTS nếu chưa xác minh. Input System đã dùng trong prototype; NGO + Unity Transport là lựa chọn TARGET realtime, không claim đang có implementation trong prototype; Multiplayer Play Mode, Multiplayer Tools/Network Simulator và Unity Test Framework cho dev/QA; ObjectPool chỉ cho VFX/presentation; Cinemachine 3 cho camera client. Production dev test đầu dùng Local Session boundary ở §1.1, không mặc định kế thừa prototype; gate mạng và final online acceptance kết nối Dedicated Server. MPPM chỉ giúp mở nhiều Client khi lặp nhanh. Tránh DOTS/ECS, Addressables, Relay, prediction/rollback, cloud services và service framework nếu slice chưa chứng minh lợi ích.

## SOURCE → DESTINATION của lượt này

| SOURCE | DESTINATION | Bảo toàn |
| --- | --- | --- |
| 2_HUYEN_LO_TECHNICAL.md: `**Melee lane probe V6.2.4:**` | [Roadmap](#prototype-technical-history) | Chuyển nguyên nội dung lịch sử; không biến thành luật hiện tại |
| 2_HUYEN_LO_TECHNICAL.md: `**Blockout V6.2.4:**` | [Roadmap](#prototype-technical-history) | Chuyển nguyên nội dung lịch sử; không biến thành luật hiện tại |
| 2_HUYEN_LO_TECHNICAL.md: `**Pack activity / patrol probe:**` | [Roadmap](#prototype-technical-history) | Chuyển nguyên nội dung lịch sử; không biến thành luật hiện tại |
| 2_HUYEN_LO_TECHNICAL.md: `**Water contact:**` | [Roadmap](#prototype-technical-history) | Chuyển nguyên nội dung lịch sử; không biến thành luật hiện tại |
| 2_HUYEN_LO_TECHNICAL.md: `**UI event boundary:**` | [Roadmap](#prototype-technical-history) | Chuyển nguyên nội dung lịch sử; không biến thành luật hiện tại |
| 2_HUYEN_LO_TECHNICAL.md: `**Mock/dev boundary:**` | [Roadmap](#prototype-technical-history) | Chuyển nguyên nội dung lịch sử; không biến thành luật hiện tại |
| 3_HUYEN_LO_DESIGN_ANALYSIS.md: `**Feedback V6.2.4 — CURRENT PROBE:**` | [Roadmap](#prototype-feedback-history) | Chuyển nguyên nội dung lịch sử; không biến thành luật hiện tại |
| 3_HUYEN_LO_DESIGN_ANALYSIS.md: `**Feedback nối tiếp:**` | [Roadmap](#prototype-feedback-history) | Chuyển nguyên nội dung lịch sử; không biến thành luật hiện tại |
| 4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md: `### 13.1.` | [Roadmap](#prototype-map-history) | Chuyển nguyên nội dung lịch sử; không biến thành luật hiện tại |
| 4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md: `### 19.1.` | [Roadmap](#prototype-ui-history) | Chuyển nguyên nội dung lịch sử; không biến thành luật hiện tại |
| 1_HUYEN_LO_GDD.md: `**Nước nông` | [Roadmap](#prototype-water-history) | Chuyển nguyên nội dung lịch sử; không biến thành luật hiện tại |
| 2_HUYEN_LO_TECHNICAL.md: `**Unity / tooling:**` | [Roadmap](#prototype-tooling-history) | Chuyển nguyên nội dung lịch sử; không biến thành luật hiện tại |
| 3_HUYEN_LO_DESIGN_ANALYSIS.md: `Bản mẫu mới kiểm one-press` | [Roadmap](#prototype-runtime-history) | Lịch sử runtime; phương pháp/decision design vẫn ở Analysis |
| 3_HUYEN_LO_DESIGN_ANALYSIS.md: `User yêu cầu sửa theo feedback và tự xử lý inconsistency;` | [Roadmap](#prototype-runtime-history) | Lịch sử runtime; phương pháp/decision design vẫn ở Analysis |
| 3_HUYEN_LO_DESIGN_ANALYSIS.md: `[Review Art]` | [Roadmap](#prototype-runtime-history) | Lịch sử runtime; phương pháp/decision design vẫn ở Analysis |
| 3_HUYEN_LO_DESIGN_ANALYSIS.md: `Bộ power/MP/CD/gear làm control` | [Roadmap](#prototype-runtime-history) | Lịch sử runtime; phương pháp/decision design vẫn ở Analysis |

<a id="prototype-runtime-history"></a>

## Analysis — lịch sử trạng thái runtime và sequencing

Các snapshot sau mô tả thời điểm review trước; trạng thái mới đọc ở đầu §10. Không dùng chúng làm nghiệm thu layout hiện tại.

Bản mẫu mới kiểm one-press approach/cast, Nấm→Sói, EdgeExit và menu bằng bàn phím. Debug preset giúp tái hiện một đoạn mà không replay cả hành trình; đây là fixture có nhãn, không save authority hoặc evidence route thật. G-L vẫn PARTIAL tới khi có rig/import và manual usability/feel review ở tốc độ thường. Production base và Dedicated/backend chưa bắt đầu.

User yêu cầu sửa theo feedback và tự xử lý inconsistency; đây là nguồn của lượt sửa, không NSO recommendation tự thành luật. V6.2.0 prototype tại checkpoint `46006c4` được giữ làm reference; build/tests/video cũ vẫn đúng cho revision đó, **không chứng minh V6.2.1 đạt G-L**. Production codebase chưa bắt đầu, không lấy các class prototype làm architecture authority.

[Review Art](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#art-review) đã tách nguyên tắc đủ dùng và con số chưa duyệt. CURRENT thu hoạch mock, probe feel/art/UI rồi review/dựng production base; G-L revision mới trước gate Dedicated hai Client và production rộng; gate backend thật và toàn TARGET giữ trong [Roadmap](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#phase-gates). Cung/Boss/PvP/SAVE giữ TARGET dù DEFER implementation khỏi slice.

Bộ power/MP/CD/gear làm control V6.2.4 giữ nguyên, nhưng rotation/resolve đã đổi; các bảng dưới đây là phép kiểm, không thay thế GDD. **TÍNH TỪ LUẬT** là phép tính xác định; **MÔ PHỎNG** phụ thuộc giả định; **ĐỀ XUẤT** chưa là luật. PvP có Food/Potion và recovery checkpoint mới nên mô hình PvP cũ chỉ là đối chiếu sát thương trực tiếp, không dự báo thắng/hòa; farm scheduler cũ cũng cần rerun vì combat/resource đổi. VS-1 disposable/reference prototype đã chạy Unity với route/tests **revision V6.2.0**; [evidence cũ](../../prototypes/VS1_EndToEnd/README.md) không nghiệm thu V6.2.1, production architecture, UX hoặc các mô hình balance/TARGET. Production codebase chưa bắt đầu.
