# Huyền Lộ — Roadmap

Các số BASELINE/TUNABLE chưa phải nghiệm thu runtime.

**CURRENT:** Pha R phải có minimal Kiếm **và Cung** trước production base/content rộng. Sword-first production vẫn giữ; full Bow production được hoãn sau gates phù hợp. [Early probe protocol](playtest-and-balance.md#early-two-class-probe) là điều kiện review, chưa có evidence pass.

## Document owns

TARGET/CURRENT/DEFERRED, thứ tự gate, phụ thuộc production, phân việc và cửa sổ quản lý.

## Document does not own

Luật ngoài domain thuộc owner trong [documentation map](../README.md); evidence thuộc [Playtest & Balance](../04-production/playtest-and-balance.md), thứ tự triển khai thuộc [Roadmap](../04-production/roadmap.md).

<a id="target-current-deferred"></a>

<a id="1-target-current-và-deferred"></a>

## TARGET, CURRENT và DEFERRED

**TARGET** là toàn bộ P0 hiện hành: Tân Lữ, Kiếm và Cung; Lv 1–20 và Q1–Q12, trong đó Q9 tùy chọn; trang bị/kinh tế; năm map farm và ba khu hỗ trợ; Linh Biến, Boss, co-farm, chat và PvP. Kiến trúc đích là Unity Client + Dedicated Game Server + Spring Boot + PostgreSQL, có Login/Character Select, lưu tiến trình và khôi phục phiên. Nghiệm thu cuối theo [Playtest & Balance](playtest-and-balance.md#acceptance-routing) cần tối thiểu hai client đồng thời với backend/DB thật.

World và các danh sách nhận thưởng, nhặt đồ, threat phải an toàn với N người; hai client là mức kiểm tối thiểu, không phải trần người chơi hoặc tuyên bố capacity.

**CURRENT** là đồng bộ tài liệu sau khi thu bài học từ VS-1. Đường triển khai tiếp theo là **thu bài học bản mẫu → đồng bộ docs → probe cảm giác điều khiển/UI/art/rig → review production base → dựng local production slice → Dedicated với ít nhất hai client sớm → mở rộng production**. Local-first giúp kiểm luật và tương tác sớm; TARGET vẫn là game online. CURRENT biểu thị việc đang ưu tiên, không tự có nghĩa đã code hoặc đã pass.

Bản mẫu cũ đã có các lượt test và video theo revision riêng. Trạng thái G-L PARTIAL trong hồ sơ cũ không chứng minh G-L production theo thiết kế mới. Chưa có bằng chứng runtime mới cho controls, cadence, mật độ, địa hình hay NPC/quest đã sửa.

**DEFERRED** là hạng mục vẫn thuộc TARGET nhưng chưa nằm trên đường phụ thuộc đầu tiên. Cung là P0. Giữ toàn bộ skill, projectile, gear, pose/VFX và các phép kiểm Kiếm/Cung. Slice sớm triển khai Kiếm trước; Cung vẫn được giới thiệu trong Q6 và có thể ghi “Chưa mở trong bản thử nghiệm” khi chưa playable. Nhãn giới hạn này không dùng trong sản phẩm cuối.

| Nhóm | Trạng thái hiện hành | Điều kiện quay lại / mở rộng |
| --- | --- | --- |
| Tân Lữ → Kiếm, movement, chọn skill và ExecuteSelected | CURRENT docs và kế hoạch probe; chưa triển khai revision mới | Review production base, rồi kiểm G-L theo controls mới ở tốc độ thường |
| Vân Khê, Học Viện, Đồng Sương; NPC/Nấm/Sói/Dummy/UI | CURRENT phạm vi blockout và local slice; layout VS-1 cũ chỉ là reference | Kiểm tuyến Q1–Q6, ≥3 Dummy, địa hình mới, mật độ và khu chức năng NPC |
| Kiếm Lv 10/13/17, Q7–Q12 và các map sau Đồng Sương | DEFERRED khỏi slice đầu, vẫn P0 | G-N và G-D; fixture hẹp chỉ kiểm kỹ thuật, không thay hành trình thật |
| Cung playable, skill/gear/projectile/pose/balance | DEFERRED IMPLEMENTATION, vẫn P0 | Minimal Cung và so Kiếm/Cung ở Pha R; production Cung sau G-N, trước nhân toàn bộ family; giải OPEN liên quan |
| Dedicated với ≥2 client | Gate sớm sau G-B và G-L revision mới; chưa bắt đầu | G-N trước mở production content/art rộng, trên production base |
| Login/Character Select, Spring/PostgreSQL, ticket/lease/checkpoint/reconnect | DEFERRED khỏi slice local, vẫn P0 | G-D sau gate mạng; adapter RAM không thay DB thật |
| Co-farm/chat/shared claim, Linh Biến và Q8 | DEFERRED khỏi slice đầu | G-N kiểm N recipients/MapId; G-D kiểm commit; G-C mở route/content |
| Boss và tải hình ảnh online đầy đủ | DEFERRED khỏi slice đầu | G-C → G-F; giữ luật Boss và kiểm telegraph/camera/TTK thật |
| PvP/Q9/escrow/settlement | DEFERRED khỏi slice đầu, vẫn P0 | G-D trước G-P; Q9 optional không cho phép bỏ hệ PvP |
| Buff mới, shield/groggy, QoL/P1/P2 | Đề xuất chưa duyệt | Tra [Game Design — đề xuất P1](../01-design/game-design.md#research-ideas); phím R trong proposal controls hiện dành cho Food, không suy ra Buff R đã duyệt |

<a id="vs-1"></a>

<a id="2-lát-cắt-local-đầu-tiên-và-reference-vs-1"></a>

## Lát cắt local đầu tiên và reference VS-1

**BASELINE phạm vi G-L:** Q1–Q6, Tân Lữ → Kiếm, ba map **Vân Khê / Học Viện / Đồng Sương**. Hướng triển khai vẫn Kiếm trước → local trước → gate mạng sớm. Trúc Ảnh mở trong quest state sau Q6 nhưng nằm ngoài build slice đầu; MapExit phải báo giới hạn bản thử. G-L không thêm Q7/enhance để thay mục tiêu đang kiểm. Phạm vi có thể được review sau blockout nếu bằng chứng cho thấy cần đổi; khi đó cập nhật mục này, G-L và kế hoạch kiểm liên quan.

**Phạm vi slice không phải phạm vi định nghĩa quest.** Production phải có đầy đủ 12 `QuestDefinition` cho Q1–Q12, điều kiện, bước hành động, thưởng, NPC nhận/trả và mở khóa theo [Quests & Narrative](../01-design/quests-and-narrative.md#quests-story). G-B review cách biểu diễn cả tuyến; G-L chỉ chạy đoạn Q1–Q6. Những quest sau phải nằm trong tuyến production thật ở G-C/G-T, không được thay bằng vài con số tracker hoặc preset debug.

VS-1 hiện có là reference của luật/layout/input cũ, không phải production architecture hay art acceptance. Không mang nguyên các bờ dốc, đất one-way hoặc phím alias cũ vào slice revision mới. Probe art/rig mới dùng một sandbox standalone disposable theo [Art](../03-art/art-and-visual-production.md#first-art-probe). Sandbox art chỉ kiểm hình ảnh/pipeline, không thay production base hoặc route G-L.

G-L chạy onboarding Tân Lữ → Kiếm theo [canonical quest route](../01-design/quests-and-narrative.md#quests-story). Minimal Cung probe Pha R dùng fixture riêng, không thay fresh-run Kiếm hoặc full-route Cung ở G-C/G-T.

Slice Kiếm chạy Phong Du; production Cung chạy Diệp Lam. Không khôi phục Tạ Minh làm bước trung gian.

| Cần có trong slice revision mới | Bằng chứng cần giữ cho G-L |
| --- | --- |
| Local Session authoritative; intent/result/UI tách biệt | Một input đi qua resolver một lần; UI/VFX không tự sửa HP/túi/quest; log clock/life/action IDs |
| Movement/jump/drop-through, ba map và camera | Trái/phải di chuyển, lên nhảy, xuống drop-through; mặt solid trực giao đúng collider. EdgeExit/MapId/gate/refused-transition không ping-pong; kiểm coyote/buffer/variable-height ở tốc độ thường |
| Chọn skill và thực thi riêng | 1/2/3 chỉ chọn slot. Chỉ ExecuteSelected tạo intent; giữ phím không lặp. Đổi slot không sửa lệnh pending/buffer/action đã chụp; kiểm hủy/thay thế và revalidate khi tới nơi |
| Focus và combat Tân Lữ → Kiếm | AUTO/EXPLICIT, tìm/giữ/thực thi có vùng riêng; range/geometry/clock đúng. Chết hủy lệnh combat nhưng giữ focus hợp lệ và HUD HP, kể cả cập nhật HP người chơi khác |
| Quái và mật độ mới | Cụm melee có Approach/Contact–Staging/Attack/Recovery–Reposition; peer đã chiếm chỗ không bị coi là terrain bị chặn. Quái giữ HomeRegion/WalkRegion/SurfaceId; Return và kiting theo luật chung |
| EXP/điểm/class, item/equip/loot/sell, Food/Potion/death | Đúng luật cho phần đã có; reject/full bag/đầy HP không tiêu sai item. Dùng điểm chọn phái thực tế để giữ HP liên tục; không dùng fixture reset thay lưu bền |
| Q1–Q6, NPC và UI | Route thật, late/retry/grant/full-bag; inventory/equipment preview, shop, Skills/Quest/HUD dùng được bằng bàn phím và chuột; automation không chứng minh UX đã pass |
| Mẫu art nhỏ đã nhập Unity | Default + outfit I, Mộc/Kiếm I và một visual khác qua fixture; Dummy/Nấm/Sói, impact/Death, solid/one-way đúng loại, font có dấu; đo grip/pivot/phase/độ đọc và giờ sửa |

**Chưa bắt buộc cho G-L:** login/backend/DB, network/reconnect, full production Cung, PvP, Boss và các map sau Đồng Sương. Local dùng profile/fixture và RAM; phải báo dữ liệu có thể mất khi reset/đóng phiên. JSON chỉ phục vụ config/fixture/import-export dev. Thành công trong RAM chưa chứng minh crash atomicity hay persistence.

Dummy placements/HP/timer theo quest owner; G-L kiểm route và contention, không tự đổi farm respawn. Gravity/momentum giữ; air S2/S3 vẫn OPEN, không khóa movement để giảm công vẽ.

<a id="phase-gates"></a>

<a id="3-các-phase-và-gate-mở-rộng"></a>

## Các phase và gate mở rộng

Gate chỉ pass khi có bằng chứng đúng revision. [Playtest & Balance](playtest-and-balance.md#technical-gates) giữ setup và contract kiểm; trạng thái quyết định gameplay nằm ở semantic owner, index tại docs/README.md. G-x dưới đây là gate triển khai. **Hiện chưa có gate production theo revision mới được xác nhận pass.**

| Phase / gate | Điều kiện vào và công việc | Điều kiện ra / cho phép tiếp theo |
| --- | --- | --- |
| Pha R — thu bài học và probe | Thu findings VS-1, đồng bộ docs; minimal Kiếm/Cung controls/UI/art/rig trong sandbox disposable riêng | Có contract hiện hành, danh sách giả định cần đo và mẫu nhỏ đọc được; không biến mock thành production base |
| Pha B / **G-B: production base review** | Review input/intent/pending/clock/IDs/definitions/physics/commit/presentation; review toàn bộ Q1–Q12 và class/NPC dependencies rồi dựng base nhỏ | Ownership/dependency rõ; kiểm core theo revision mới; room production sơ bộ; quyết phần reuse/rewrite. Không dựng framework chỉ vì đối xứng |
| Pha L / **G-L: local slice revision mới** | G-B pass; ghép Q1–Q6/Kiếm từ base, blockout ba map và kit đủ đọc | Route fresh Q1–Q6 cùng log/video, review feel/UX ở tốc độ thường; các phép kiểm áp dụng trong bảng dưới đạt. Art/QA/rework có số đo; mở G-N |
| Pha N / **G-N: Dedicated + ≥2 client sớm** | G-B/G-L mới pass; rules/resolver/timeline chạy headless; adapter RAM ghi rõ fixture dev | Hai client độc lập kiểm movement/MapId/kill/quest/shared claim, stale life/replay/late result, dead-focus và HP người chơi khác; N-safe recipients/claim/threat. Đo correction/latency/headless; mở rộng có chọn lọc |
| Pha D / **G-D: backend/persistence thật** | G-N pass; thay fixtures bằng Spring/PostgreSQL, giữ một writer và domain result chung | Login → Select → one-time ticket → join; lease/duplicate, checkpoint/SafeAnchor/HP0; N recipients, claim/quest commit idempotent; crash/outage/retry trước/sau ACK. ClassChosenLevel và class transaction bền vững |
| Pha C / **G-C: content và hai phái** | G-N trước art rộng; G-D trước nghiệm thu route dài có dữ liệu bền | Kiếm và Cung, skill/passive, gear HP/MP mới; Q7–Q12, bảy mob identities/sáu rigs, các map/Linh/Q8 đúng design owner. Chạy đủ định nghĩa quest trong tuyến thật; kiểm balance/pose trước nhân variants |
| Pha F / **G-F: Boss và tải online** | Content/skill/world đã ổn; schema/lifecycle không đổi lớn | Lịch/telegraph/Slow/Cuồng, corpse/loot/credit/camera đúng; chơi thật ≥2 người và probe 3–4+ để tìm giả định fixed-pair. Ghi máy/build/CPU/bytes/latency, TTK và journey; không suy capacity từ ca pass |
| Pha P / **G-P: PvP/chat và ghép online** | G-D trước escrow; movement/combat/recovery đủ ổn | 10 stakes, escrow hai bên, Food/quota, timeout DRAW/forfeit/abort, settlement/crash retry; Map Chat/reconnect/standalone package. Q9 vẫn optional trong route |
| **G-T: nghiệm thu TARGET** | Hai phái và toàn P0 đã ghép; các gate phụ thuộc có bằng chứng | Đối chiếu owner design tương ứng/owner kỹ thuật tương ứng trên Client + Dedicated + backend/DB thật, ≥2 người đồng thời; full fresh-run Q1–Q12 từng phái theo mục dưới. Không gọi TARGET done khi thiếu Cung, Q12/Boss/PvP/recovery |

G-N dùng phạm vi local đã kiểm lại và diễn ra trước khi làm rộng art/content. Không chờ xong toàn bộ Cung/gear/Boss mới kiểm authority trên Dedicated. Spike không backend chỉ là fixture dev; G-D vẫn bắt buộc trước khi nhận persistence/reliability là hoàn thành.


<a id="management-window"></a>

<a id="4-khung-quản-lý-hai-tháng-nhóm-bốn-người"></a>

## Khung quản lý hai tháng, nhóm bốn người

**Hạn nguồn lực thực tế: hai tháng/bốn người.** Dùng khoảng tám tuần quản lý kể từ lúc bắt đầu thực hiện. Chưa có ngày bắt đầu hoặc số giờ khả dụng từng người nên chưa thể suy deadline lịch/tổng person-hours. Bảng dưới là mục tiêu quản lý, không cam kết toàn TARGET chắc chắn hoàn thành trong hai tháng.

| Tuần mục tiêu | Đầu ra để review | Điều kiện / xử lý nếu chưa đạt |
| --- | --- | --- |
| 1 | Thu findings bản mẫu, sync docs; minimal Kiếm/Cung controls/UI/art/rig và review base nhỏ | Ghi assumptions, OPEN và công sửa/% dùng được thật; VS-1 giữ vai trò reference |
| 2 | Core production base/local room → G-B; definitions Q1–Q12, input/clock/IDs/physics/AI/kit | Review select-only/ExecuteSelected, class/NPC dependencies, terrain/mật độ mới; chưa nhân toàn bộ family |
| 3 | Ghép Q1–Q6 Nấm→Sói/Kiếm, blockout ba map, art/UI nhỏ → G-L revision mới | Fresh route, review tốc độ thường và logic retry/death; thiếu UX/art thì gate còn PARTIAL |
| 4 | Dedicated + ≥2 client trên slice → G-N | Checkpoint mạng trước production rộng; nếu boundary/headless/recipients sai, sửa trước mở rộng |
| 5 | Spring/PostgreSQL/auth/ticket/lease/commit/recovery → G-D mục tiêu; tích hợp production Cung sau minimal probe Pha R và nếu G-N pass | Chưa pass DB thật chưa nhận persistence done; Cung vẫn DEFERRED P0 nếu chưa triển khai |
| 6 | Kiếm skill/gear, Q7–Q8/Linh rồi mở tuyến theo G-C; Cung probe và tích hợp | Đo cadence/sustain/HP/gear mới và mật độ; ưu tiên đoạn hành trình liên tục đã kiểm |
| 7 | Boss/recovery/chat; PvP khi G-D và combat đủ ổn; tiếp tục route Cung | Ghi từng hệ đã đạt/chưa đạt, giữ đủ definitions và full-route backlog |
| 8 | Regression, package standalone và evidence fresh-route; G-T chỉ khi đủ P0 | Báo phạm vi thực đã chạy. Thiếu thời gian thì review lịch/phạm vi bản thử với chủ dự án, giữ nguyên TARGET |

Hạng mục độc lập trong G-C/F/P có thể làm song song sau gate phụ thuộc. Schema/fixture và art probe được chuẩn bị sớm, nhưng không tự mở production rộng hoặc chứng minh gate đã pass.

| Vai trò chính trong nhóm bốn người | Trách nhiệm và phối hợp |
| --- | --- |
| 1 — Gameplay/authority | Local Session, combat/stat/timeline/lifecycle; production base rồi Dedicated với người 4; giữ resolver/clock chung |
| 2 — World/quest/AI | Blockout/physics/MapExit/SpecialGate, bảy NPC và definitions Q1–Q12; route slice/mob/loot rồi mở phần sau theo gate |
| 3 — Art/UI | Mẫu nhỏ, clean/slice/socket, rig/weapon/terrain/common kit; đo với người 2/4 trước nhân catalog |
| 4 — Integration/QA/backend | Fixtures/log/regression route; Dedicated ≥2 client rồi Spring/DB/recovery; hỗ trợ local từ tuần 1 |

Đây là phân trách nhiệm, không giả mọi người cùng chuyên môn hoặc làm full-time. Review bằng giờ khả dụng còn lại và giờ thực đã tiêu cho gameplay/art/editor/backend/integration/QA/rework. Không nhân bốn người với estimate 160–240 h lịch sử để gọi thành ngân sách mới.

<a id="art-workflow"></a>

<a id="5-art-cho-người-chưa-thạo-vẽ-và-kỷ-luật-giao-việc"></a>

## Art cho người chưa thạo vẽ và kỷ luật giao việc

Dùng quy trình nhỏ ở [Art & Visual Production](../03-art/art-and-visual-production.md#art-tool-workflow): chọn mẫu, sửa palette/outline/pivot/grip bằng editor pixel, nhập Unity rồi đo. Probe chạy trong sandbox standalone disposable riêng; VS-1 giữ vai trò reference.

**TOOL CANDIDATE:** có thể thử PixelLab bằng Free/free trial rồi đánh giá tool/workflow. Contract visual/import và architecture không phụ thuộc PixelLab. Giới hạn dịch vụ cần kiểm lại từ [FAQ chính thức](https://www.pixellab.ai/docs/faq) tại thời điểm dùng; không giả tool animation/outfit đều miễn phí hoặc hứa số credits. Nếu tính năng cần thiết không có trong Free, ghi “chưa kiểm được với Free”, dùng placeholder kiểm pipeline và đo chi phí trước quyết định mua.

Đếm cả output thất bại. `% dùng trực tiếp = output pass không sửa / toàn bộ output tạo`; `% dùng sau sửa = output pass sau sửa / toàn bộ output tạo` là hai nhóm riêng. `% dùng được tổng = tổng hai nhóm pass / toàn bộ output tạo`, không đếm một output hai lần. Chưa tạo mẫu ghi **CHƯA ĐO**. Ghi giờ sửa/import/QA, lỗi pose/alignment và cỡ mẫu cho từng loại. Gate cần mẫu rig/weapon/outfit chạy đúng timing trong room thật, không chỉ một PNG đẹp.

Kỷ luật cho mọi người và coding agent nằm ở [Architecture](../02-technical/architecture.md#architecture-discipline): gameplay tách UI, ID ổn định, một clock authority/session; AnimationEvent chỉ presentation; Local và Dedicated dùng cùng rules. Task ghi rõ TARGET/CURRENT, section canonical, input/result, gate cần kiểm và OPEN dependency. Không tự đổi range/timer/26-frame/slot để làm task pass. Giá trị BASELINE/TUNABLE phải có setup và bằng chứng khi đề xuất chỉnh; quyết định tại semantic owner, index ở docs/README.md.

<a id="production-release"></a>

<a id="6-khi-nào-mở-production-và-điều-gì-được-hoãn"></a>

## Khi nào mở production và điều gì được hoãn

Trước G-N chỉ làm mẫu nhỏ đủ kiểm: room, default/outfit I, Mộc/Kiếm và minimal Cung S1/S2 + sockets cùng vài mob/kit primitives. G-L vẫn cần blockout chơi liên tục đủ ba map; room art không thay hành trình. Trước nhân mỗi family sau G-N cần evidence pose/socket/readability và xử lý các OPEN liên quan trong phạm vi đó. G-N không tự duyệt Hybrid count, 33 pose, 13–25 weapon images, camera hoặc tổng terrain/pocket mới.

Cắt P1/P2 và polish trước: giảm cosmetic variations/shake/sound/phần trình bày cầu kỳ, giữ thông tin gameplay/pending/status/telegraph, Storage core và Journey scores. Cung/Boss/backend/PvP hoãn khỏi slice đầu theo thứ tự hiện hành, không xóa khỏi TARGET. Review cuối tuần 2/4 và sau mỗi gate bằng công còn lại. Nếu TARGET vượt hai tháng, trình phạm vi bản thử và lịch tiếp theo cụ thể; không hạ nghĩa final DoD. Rationale, bảng số và workload cũ tiếp tục được giữ trong phần trace.
