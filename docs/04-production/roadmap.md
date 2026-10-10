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

**CURRENT** là đồng bộ tài liệu sau khi thu bài học từ VS-1. Prototype cũ đã được xóa khỏi checkout; [trace và source đã commit](../90-archive/production-history.md#prototype-source-retired) chỉ dùng để tra lịch sử. Đường triển khai tiếp theo là **thu bài học bản mẫu → đồng bộ docs → probe cảm giác điều khiển/UI/art/rig → review production base → dựng local production slice → Dedicated với ít nhất hai client sớm → mở rộng production**. Local-first giúp kiểm luật và tương tác sớm; TARGET vẫn là game online. CURRENT biểu thị việc đang ưu tiên, không tự có nghĩa đã code hoặc đã pass.

Bản mẫu cũ đã có các lượt test và video theo revision riêng. Trạng thái G-L PARTIAL trong hồ sơ cũ không chứng minh G-L production theo thiết kế mới. Chưa có bằng chứng runtime mới cho controls, cadence, mật độ, địa hình hay NPC/quest đã sửa.

**DEFERRED** là hạng mục vẫn thuộc TARGET nhưng chưa nằm trên đường phụ thuộc đầu tiên. Cung là P0. Giữ toàn bộ skill, projectile, gear, pose/VFX và các phép kiểm Kiếm/Cung. Slice sớm triển khai Kiếm trước; Cung vẫn được giới thiệu trong Q6 và có thể ghi “Chưa mở trong bản thử nghiệm” khi chưa playable. Nhãn giới hạn này không dùng trong sản phẩm cuối.

| Nhóm | Trạng thái hiện hành | Điều kiện quay lại / mở rộng |
| --- | --- | --- |
| Tân Lữ → Kiếm, movement, chọn skill và PrimaryAction (combat branch) | CURRENT docs và kế hoạch probe; chưa triển khai revision mới | Review production base, rồi kiểm G-L theo controls mới ở tốc độ thường |
| Vân Khê, Học Viện, Đồng Sương; NPC/Nấm/Sói/Dummy/UI | CURRENT phạm vi blockout và local slice; layout VS-1 cũ chỉ là reference lịch sử trong Git | Kiểm tuyến Q1–Q6, năm Dummy3+2/bốn Q3 kills, địa hình mới, mật độ và khu chức năng NPC |
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

## Lát cắt local đầu tiên và lịch sử VS-1

**BASELINE phạm vi G-L:** Q1–Q6, Tân Lữ → Kiếm, ba map **Vân Khê / Học Viện / Đồng Sương**. Hướng triển khai vẫn Kiếm trước → local trước → gate mạng sớm. Trúc Ảnh mở trong quest state sau Q6 nhưng nằm ngoài build slice đầu; MapExit phải báo giới hạn bản thử. G-L không thêm Q7/enhance để thay mục tiêu đang kiểm. Phạm vi có thể được review sau blockout nếu bằng chứng cho thấy cần đổi; khi đó cập nhật mục này, G-L và kế hoạch kiểm liên quan.

**Phạm vi slice không phải phạm vi định nghĩa quest.** Production phải có đầy đủ 12 `QuestDefinition` cho Q1–Q12, điều kiện, bước hành động, thưởng, NPC nhận/trả và mở khóa theo [Quests & Narrative](../01-design/quests-and-narrative.md#quests-story). G-B review cách biểu diễn cả tuyến; G-L chỉ chạy đoạn Q1–Q6. Những quest sau phải nằm trong tuyến production thật ở G-C/G-T, không được thay bằng vài con số tracker hoặc preset debug.

VS-1 đã được xóa khỏi checkout; luật/layout/input cũ chỉ còn là reference lịch sử trong Git, không phải production architecture hay art acceptance. Không mang nguyên các bờ dốc, đất one-way hoặc phím alias cũ vào slice revision mới. Probe art/rig mới dùng một sandbox standalone disposable theo [Art](../03-art/art-and-visual-production.md#first-art-probe). Sandbox art chỉ kiểm hình ảnh/pipeline, không thay production base hoặc route G-L.

G-L chạy onboarding Tân Lữ → Kiếm theo [canonical quest route](../01-design/quests-and-narrative.md#quests-story). Minimal Cung probe Pha R dùng fixture riêng, không thay fresh-run Kiếm hoặc full-route Cung ở G-C/G-T.

Q6 discovery nói chuyện cả Phong Du và Diệp Lam trong cả slice Kiếm và route Cung; mentor đã chọn giữ admission/turn-in, mentor kia vẫn có contextual Talk. Không khôi phục Tạ Minh làm bước trung gian.

G-L acceptance chi tiết dùng [gate protocol](playtest-and-balance.md#technical-gates) và [QA cases](playtest-and-balance.md#qa); [Runtime discipline](../02-technical/gameplay-runtime.md#architecture-discipline) giữ implementation boundaries, không bảng requirement thứ hai tại Roadmap.

**Chưa bắt buộc cho G-L:** login/backend/DB, network/reconnect, full production Cung, PvP, Boss và các map sau Đồng Sương. Local dùng profile/fixture và RAM; phải báo dữ liệu có thể mất khi reset/đóng phiên. JSON chỉ phục vụ config/fixture/import-export dev. Thành công trong RAM chưa chứng minh crash atomicity hay persistence.

Dummy placements/HP/timer theo quest owner; G-L kiểm route và contention, không tự đổi farm respawn. Gravity/momentum giữ; air S2/S3 vẫn OPEN, không khóa movement để giảm công vẽ.

<a id="phase-gates"></a>

<a id="3-các-phase-và-gate-mở-rộng"></a>

## Các phase và gate mở rộng

Gate chỉ pass khi có bằng chứng đúng revision. [Playtest & Balance](playtest-and-balance.md#technical-gates) giữ setup và contract kiểm; trạng thái quyết định gameplay nằm ở semantic owner, index tại docs/README.md. G-x dưới đây là gate triển khai. **Hiện chưa có gate production theo revision mới được xác nhận pass.**

| Phase / gate | Điều kiện vào và công việc | Điều kiện ra / cho phép tiếp theo |
| --- | --- | --- |
| Pha R — thu bài học và probe | Thu findings VS-1, đồng bộ docs; minimal Kiếm/Cung controls/UI/art/rig trong sandbox disposable riêng | Có contract hiện hành, danh sách giả định cần đo và mẫu nhỏ đọc được; không biến mock thành production base |
| Pha B / **G-B: production base review** | Review input/intent/pending/clock/IDs/definitions/physics/commit/presentation; review typed ObjectiveGroup/QuestDefinition toàn bộ Q1–Q12 (MobIdentity, special predicates, RNG/staged grants/restoration OPEN, physical bindings, NPC/rewards/unlocks) và class/NPC dependencies rồi dựng base nhỏ; review C0 PrimaryAction/ActiveFocus/acquisition/propagation/presentation trước actual quest credit | Ownership/dependency rõ; kiểm core theo revision mới; room production sơ bộ; quyết phần reuse/rewrite. Không dựng framework chỉ vì đối xứng |
| Pha L / **G-L: local slice revision mới** | G-B và C0 pass cho combat/real kill flow; ghép Q1–Q6/Kiếm từ base, blockout ba map và kit đủ đọc | Route fresh Q1–Q6 cùng log/video, review feel/UX ở tốc độ thường; các phép kiểm áp dụng trong bảng dưới đạt. Art/QA/rework có số đo; mở G-N |
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
| 1 | Thu findings bản mẫu, sync docs; minimal Kiếm/Cung controls/UI/art/rig và review base nhỏ | Ghi assumptions, OPEN và công sửa/% dùng được thật; VS-1 chỉ là reference lịch sử trong Git |
| 2 | Core production base/local room → G-B; definitions Q1–Q12, input/clock/IDs/physics/AI/kit | Review select-only/PrimaryAction (combat branch), class/NPC dependencies, terrain/mật độ mới; chưa nhân toàn bộ family |
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

## Giao việc cho art probe

Pha R phải review [Master Pose modular probe](../03-art/art-and-visual-production.md#first-art-probe) và [P01/P02/P03 acceptance](playtest-and-balance.md#modular-character-acceptance) trước chốt raster budget; P15 đo công thực, không chỉ review một outfit. Pha R/P15 dùng [Art workflow và phép đo output/giờ](../03-art/art-and-visual-production.md#art-tool-workflow); task tuân [Runtime discipline](../02-technical/gameplay-runtime.md#architecture-discipline). Chỉ mở production theo gate bên dưới.

<a id="production-release"></a>

<a id="6-khi-nào-mở-production-và-điều-gì-được-hoãn"></a>

## Khi nào mở production và điều gì được hoãn

Trước G-N chỉ làm mẫu nhỏ đủ kiểm: room, default/outfit I, Mộc/Kiếm và minimal Cung S1/S2 + sockets cùng vài mob/kit primitives. G-L vẫn cần blockout chơi liên tục đủ ba map; room art không thay hành trình. Trước nhân mỗi family sau G-N cần evidence pose/socket/readability và xử lý các OPEN liên quan trong phạm vi đó. G-N không tự duyệt Hybrid count, 33 pose, 13–25 weapon images, camera hoặc tổng terrain/pocket mới.

Cắt P1/P2 và polish trước: giảm cosmetic variations/shake/sound/phần trình bày cầu kỳ, giữ thông tin gameplay/pending/status/telegraph, Storage core và Journey scores. Cung/Boss/backend/PvP hoãn khỏi slice đầu theo thứ tự hiện hành, không xóa khỏi TARGET. Review cuối tuần 2/4 và sau mỗi gate bằng công còn lại. Nếu TARGET vượt hai tháng, trình phạm vi bản thử và lịch tiếp theo cụ thể; không hạ nghĩa final DoD. Rationale, bảng số và workload cũ tiếp tục được giữ trong phần trace.


<a id="consolidation-slices"></a>

## Implementation readiness — vertical slices sau consolidation 2026-10-09

**PLAN/PROPOSAL; chưa code hoặc pass gate.** [Source audit](../90-archive/production-history.md#source-audit-20261009) ghi actual classes/files tại snapshot 2026-10-09; [matrix](../02-technical/gameplay-runtime.md#responsibility-matrix) ghi trách nhiệm đề nghị. Source VS-1 đã bị xóa khỏi checkout; tra commit lưu trữ khi cần đối chiếu, dựng production theo contract mới. Production `game/`/`backend/` chưa tồn tại. Module mới dưới đây là đề xuất boundary, chưa phải filename/class có sẵn; G-B quyết layout nhỏ nhất. Không port nguyên `SliceSession`/`SliceHud`.

**Thứ tự đề nghị:** Inventory → fixed Jump; C0 combat contract sau base/movement và trước real combat death credit; shared UI → Shop → regular ground → personal entitlement → collection objectives → Q2/mentor → presentation integration. Fixed Jump đi trước Q2 để blockout không dựa hold apex. Minimal Kiếm/Cung art probe vẫn làm sớm ở **Pha R**; Slice 9 chỉ tích hợp vào production. Review schema/data cho đủ 12 QuestDefinitions tại G-B; Q2 content cần duyệt riêng, tuyến Q8+ nghiệm thu ở G-C. Fixture local của Slice 6–7 phục vụ Q2/Q4; durability chỉ hoàn thành sau G-N/G-D. Dedicated receiver/replication và Spring commit adapter là các PR nhỏ tại gate tương ứng, không gộp vào PR Inventory.

Mỗi slice phải có demo hẹp chạy độc lập, logs và rollback theo definition revision. Chưa có save production để migrate SQL; mục Migration nói cách xử lý fixture/import nếu được tạo. Không đổi lịch TARGET, không dùng preset thay fresh acceptance.

<a id="combat-micro-slice"></a>

### C0 — Combat contract gate / micro-slice trước real quest credit và full VFX

- **Why now:** source VS-1 tại snapshot lịch sử dùng Arc/Line và caster-order, thiếu status runtime; sửa docs không chứng minh primary-proximity chạy đúng. C0 cô lập tác động targeting trước ghép loot/collection hoặc sản xuất art rộng.
- **Dependencies:** Pha R minimal hai phái/S2 A-B và G-B boundaries/typed QuestDefinitions; Slice2 cần cho pending/arrival/vertical physics thật. Slice1/3/4 có thể đi độc lập bằng command fixtures; C0 không buộc Inventory PR đầu tích hợp combat.
- **Actual reference:** `Domain/CombatController.cs` InRange/TryStart/Resolve/Spread/Hit/Tick, `SliceRules.Skills`, target life/facing; old fixtures chỉ reference, chưa production implementation.
- **New contracts:** PrimaryAction/one ActiveFocus/modal→concrete typed intent; combat acquisition/focus/pending; primary validator; SinglePrimary/PrimaryProximity/SnapshotSpread/PrimaryExplosion; immutable action snapshot/result; unique status cache; presentation đọc ordered actual results. Đủ sáu SkillIds bằng data fixture đúng unlock, không cấp skill thật sai level.
- **Minimal implementation:** room disposable/production harness nhỏ, một caster/primary + configurable secondary/solid/one-way/life; C1/C2, vertical và LoS A/B explicit profiles. Minimal SwordS2/BowS2 pose/impact probe, không full assets hoặc quest route.
- **Migration:** dựng/test trên production base mới, đối chiếu VS-1 qua Git khi cần; ghi revision cũ/mới và expected differences, giữ power/MP/CD/range primary/caps/timing/cancel. New stats hoặc kỹ năng không thuộc slice.
- **Server:** pin source/primary/start clock/MP-CD; validate tại start/HitMoment; Kiếm query primary-near set trước apply; Spread immutable indices, Hàn valid center; results/lives/status/damage không VFX collision, không SpawnGroup gate.
- **Client:** select-only/PrimaryAction/pending reasons/one ActiveFocus; actor/weapon riêng, một main/action, actual landed impacts/status; late/replay/life mới không effect cũ. Auto-face chỉ accepted start là baseline probe, không focus-only.
- **Persistence:** không DB mới hoặc claim durable; RAM ledger/death fixtures kiểm one terminal event/recipient, actual transactions phải Slice5–7/G-D. Không save từng hit.
- **Dev Mode:** target positions/lives/MapId/identity/group/variant/Evade seed; death trước resolve, movement/focus/slot/replay, ≥2 caster/>5 recipients; log profile/start/resolve/indices/cache/action/result IDs.
- **Unit tests:** six-policy target sets/caps/power indices, ties/boundaries/invalid primary-secondary/status fail cache; source snapshot; wrong life/map/identity special predicates; no refill/double primary.
- **Integration tests:** [15 acceptance cases](playtest-and-balance.md#recovery-combat-acceptance) cùng timing/MP/CD, Rigidbody pending/arrival/vertical, result trước FX retire; one death không credit group kế. C1/C2/LoS A/B so cùng setup.
- **Multiplayer tests:** local multi-actor fixture trước, rerun ≥2 Dedicated Clients ở G-N; one life/death/recipient ledger, delayed/stale/replayed hit/status, player death giữ valid focus HP; RAM không pass network/durability.
- **Manual acceptance:** KiếmS2 và CungS2 lặp cùng camera, sau primary/sau player/terrace/tường, AAA readability và 2/4+ clutter; chọn profiles dựa evidence, không tự LOCK số chưa duyệt.
- **DoD:** sáu skills/15 cases có logs expected/actual; direction và executor recommendations rõ, policy OPEN có A/B evidence hoặc explicitly unresolved/no balance pass; không conflict Combat/Runtime/Art. C0 correctness cần đạt trước nối actual MobKilled→normal loot/personal rights/quest credit; G-N/D/C vẫn phải rerun gate thích hợp.
- **Rollback risk:** propagation đổi target distribution/aggro/falloff nên TTK/pacing lịch sử không dùng nghiệm thu; rollback profile revision không sửa nguồn stats hay đã committed quest rights.

### Slice 1 — Inventory 60 và compatible stack

- **Why now:** capacity/identity/quantity là nền cho mọi Buy/Pickup/TurnIn; cần chứng minh invariants trước UI/content.
- **Dependencies:** Pha R và G-B boundary review; chưa cần combat, quest hoặc DB chạy thật.
- **Actual reference:** `Domain/SliceRules.cs` (`Inventory`, `ItemDef`, `Item`, `Catalog`); `SliceSession.NewItem/Grant/Store/Equip/Unequip`; EditMode `DomainTests.FullBagRejectDoesNotConsumeGroundAndStackMergeStillFits`, `PendingGrantKeepsInstanceAndReceiptAfterCapacityRetry`, `StorageTransferIsAtomicAndDoesNotCopyInstances`. Full paths ở audit; không sửa frozen files.
- **New contracts:** capacity 60/Storage 40, stack key/explicit canDiscard/protection policy, unique instance IDs, checked quantity, expected character/inventory revision, immutable command payload và receipt; versioned definitions không copy gear stats cũ.
- **Minimal implementation:** Add/Remove/Merge/Split/Discard/Capacity planner trên state copy, một commit function, RAM receipt/revision adapter và room/harness thấy Add/Merge/Reject/Replay; chưa Shop/Quest logic.
- **Migration:** reset fixture 30/99 hoặc import bằng mapping được review; split tại technical bound tạo IDs mới, không dùng một ID cho nhiều stacks.
- **Server:** validate ownership, positive quantity, checked arithmetic và whole-payload capacity trước mutation; local receiver và dedicated adapter sau G-N dùng cùng path.
- **Client:** inspect panel cho occupancy/quantity/pending/reject, đủ demo; chưa full UI kit/art production.
- **Persistence:** RAM fixture ghi rõ mất khi đóng; chuẩn bị result/revision seam cho G-D, không gọi JSON/RAM là authority bền vững. Gold/cost kiểm tại Shop khi cần.
- **Dev Mode:** FillBag60Distinct, GiveCompatible, NearNumericBound, InjectCommitFailure/Replay trong test profile; không release commands.
- **Unit tests:** reject món distinct thứ 61; merge khi 60/60; sai binding; 0/âm/overflow; user split k/empty slot/new stackID/no source receipt clone; manual merge/sort selection survivor; protected discard reject; capacity reject không mất quantity; unique gear không merge; duplicate incoming ID reject.
- **Integration tests:** receipt lookup trước revision check; cùng ID khác payload reject; failure giữ toàn state; Store/Take nối fixture phải giữ identity/quantity.
- **Multiplayer tests:** hai actor states độc lập và competing mutations serialize trong fixture; ghi chưa G-N PASS. Tại G-N chạy wrong sender/two writers/stale revision thật.
- **Manual acceptance:** 60/60 vẫn thêm được compatible item; món khác báo thiếu ô; UI hiển thị quantity chính xác, không cần kill/grant để demo.
- **DoD:** invariants, receiver/RAM seam, demo độc lập và test logs đạt; chưa tuyên bố Inventory durable.
- **Rollback risk:** data semantics ảnh hưởng mọi handler; version fixture/definition trước tích hợp Shop/Quest để rollback nhỏ.

### Slice 2 — Fixed normal Jump và per-actor DropThrough

- **Why now:** Q2/traversal phải được author theo jump profile mới; làm feel sandbox trước production geometry.
- **Dependencies:** physics/input clock review G-B; không phụ thuộc Shop/entitlement.
- **Actual reference:** `Runtime/SliceHost.cs` `Update/FixedUpdate/IsGrounded/BuildMap`, `jumpHeld/ExternalJumpHeld`, release clamp vy>4→4 và ignored pairs; `BlockoutLayout.Surfaces`; `InputPhysicsTests.MovementProbeVariableHeightCoyoteAndStandaloneDrop`, `ContinuousRouteTests`, `SliceRouteProbe.releaseJumpAt/JumpObstacle` còn hold-dependent.
- **New contracts:** một press/profile, coyote/buffer consume một lần; upward gravity không phụ thuộc hold; semantic Jump/Drop riêng, lifecycle của từng ignored support pair.
- **Minimal implementation:** room có solid step và one-way gỗ có support; kiểm Jump/Drop trước tám map.
- **Migration:** automation mới bỏ hold/release apex và Q2.dropped như completion predicate; không port natural RearEarth one-way. Retune ledges trước fresh route.
- **Server:** bỏ release cutoff; forces/fall vẫn TUNABLE. Drop chỉ current standing support pair, restore khi crossing/timeout/death/map/disconnect/destroy; không disable platform toàn cục. Không suy Unity cross-machine lockstep.
- **Client:** ↑/↓ locked actions dùng cùng intent local/dedicated; velocity/phase chỉ phục vụ visual, không để presenter set gravity; Space/S không thành primary binding mới.
- **Persistence:** không lưu jump buffer/support pairs/facing DB; clear state khi session/map mới. G-N kiểm sequence/life/correction.
- **Dev Mode:** standing/falling/coyote/buffer fixtures, log press/tick/grounded/vy/apex/support pairs; không fake quest credit.
- **Unit tests:** buffer/coyote boundaries, consume một lần, không air double jump; Jump+Drop trên one-way ưu tiên Drop, trên solid ưu tiên Jump.
- **Integration tests:** Rigidbody thật tap/hold cùng apex/time trong cùng setup; ceiling collision, terminal fall, render FPS; restore ignored pairs khi reset/map/death.
- **Multiplayer tests:** hai actor trên một sàn, một người Drop không ảnh hưởng người kia; stale/replay không thêm jump; RTT/jitter correction tại G-N.
- **Manual acceptance:** jump dễ dự đoán, không precision landing; solid không Drop, một press không xuyên sàn kế; exact forces ghi PROBE.
- **DoD:** fixed profile hoạt động, có physics logs và feel review ở tốc độ chơi bình thường.
- **Rollback risk:** geometry phụ thuộc launch profile; version room trước Q2, không tune mọi map ngầm.

### Slice 3 — Shared ItemGrid/ItemDetails và selection theo context

- **Why now:** xây reusable rendering/navigation trước các business handlers.
- **Dependencies:** Slice 1 snapshots/results, Pha R font/scale/UI kit; chưa cần DB.
- **Actual reference:** `SliceHud.DrawInventory/Description/Icon/NavigateGrid/Refresh/SelectAction/DrawEquipment`, `MenuAction`; `SliceHost.HandleMenuKey/Update`; `InputPhysicsTests.KeyboardNpcShopInventoryAndDebugResetNeedNoMouse`.
- **New contracts:** stable entry ID, snapshot revision, formatted display data, allowed actions/reasons; widget Grid/Slot/Details/formatter và controllers theo context.
- **Minimal implementation:** Bag và một Storage/picker fixture dùng chung widgets; 60 slots scroll/page responsive, chưa khóa pixel/columns.
- **Migration:** không port giant IMGUI Refresh hoặc fixed 6 columns×30; bỏ legacy callbacks trong code mới sau session reset.
- **Server:** snapshots/reasons dựa canonical state; client prevalidation không trusted; UI callback không mutation trực tiếp.
- **Client:** keyboard/mouse chung command router; selection stableID; merged source map tới survivor, deleted entry clear có cue; quantity Split/Sort-Merge/Discardconfirm+disabled reasons; pending/empty/unsellable details, modal consume opening input.
- **Persistence:** selection/viewport/breadcrumb không DB; chỉ đọc committed snapshot.
- **Dev Mode:** tên tiếng Việt dài, full/empty bag, gear bound/mixed equipment, slow/fail result fixtures.
- **Unit tests:** quantity/rarity/+n/binding formatters; entry bị xóa không gửi stale command.
- **Integration tests:** cùng action qua keyboard/mouse; focus/breadcrumb/modal capture; selected entry vẫn trong viewport sau snapshot.
- **Multiplayer tests:** G-N cập nhật khi view mở không gửi old instance/pending double command; mỗi client có UI state riêng.
- **Manual acceptance:** tìm món, đọc detail, action/back ở 60 ô dễ dùng; tooltip exact quantity/font Việt/reason rõ. Automation không chứng minh usability.
- **DoD:** Bag và Storage/picker dùng chung widgets với controllers riêng, không branch Shop business logic trong view.
- **Rollback risk:** version viewmodel; rollback UI không khôi phục mutation state.

### Slice 4 — Shop Buy/Sell local end-to-end

- **Why now:** nối currency/bag qua đúng vendor để kiểm transaction boundary.
- **Dependencies:** Slice 1+3; catalog/service/binding review.
- **Actual reference:** `SliceHud.ShopBuy/ShopSell/Npc`, `SliceSession.Buy/Sell`; `DomainTests.ShopAndTurnInRequireLiveNearCorrectNpc`, `RarityVendorMultiplierUsesFloorAndEquippedCannotBeSold`; prototype chỉ Buy 1 và lọc Sell list.
- **New contracts:** NPC catalog revision/entry/quantity/cost preview, Sell instance+quantity; Buy/Sell handlers riêng dùng planner/receipt seam.
- **Minimal implementation:** một NPC room với tabs Buy/Sell trên, grid trái/details phải; Buy catalog riêng, Sell toàn bag và disabled reasons.
- **Migration:** giữ giá/effects; chuyển utility Hồi Sinh/Tẩy Mạch sang Mộc theo owner, Yên chỉ Food/HP/MP, Bách GeneralSell/gear/stone. Không thêm Crafting/off-class sales; version catalog/service refs. Q4 fixture mapping đúng sample binding, không cho bán mọi item Q4.
- **Server:** catalog/vendor/alive/range/map/class/quantity/Gold/capacity checks; bound/equipped fail, sample Q4 exact instance/step; currency+bag+qualifying event một plan atomic.
- **Client:** quantity picker/pending/result, giữ view sau Buy/Sell; không auto-sort, không bắt filter SellableOnly.
- **Persistence:** RAM receipt ở Local; G-D commit bag+Gold+ItemSold progress/receipt cùng transaction, success sau ACK; client price không trusted.
- **Dev Mode:** thiếu Vàng/full bag/bound/price change/stale revision/commit delay fixtures, chỉ test profile.
- **Unit tests:** cost overflow, quantity bounds, compatible buy khi full, noncatalog/off-class/equipped/bound/wrong-step sample rejects.
- **Integration tests:** Buy→Bag→Sell đúng quantity/Gold; ItemSold chỉ sau commit; duplicate/lost ACK/definite reject; rerun với backend G-D.
- **Multiplayer tests:** G-N hai wallets độc lập/wrong sender; G-D Sell/Use cùng stack chỉ một thắng; không global stock mới.
- **Manual acceptance:** keyboard/mouse NPC→Buy/Sell/back; total/reasons rõ, tìm được Q4 sample không cần Sort.
- **DoD:** local demo và logs nhất quán; local done khác durable done.
- **Rollback risk:** price/definition revisions làm request cũ invalid; rollback UI không được hoàn tiền từ snapshot cũ.

### Slice 5 — Regular ground loot foundation

- **Why now:** chứng minh shared pile/claim trước personal rights, đo model0..1 regular outcome, Gold/EXPbudget riêng.
- **Dependencies:** Slice1/receiver/valid-ground physics và Slice3 selection widgets; **C0 pass trước actual combat death→loot/quest credit**. Synthetic committed-death fixture có thể kiểm planner sớm, không pass combat.
- **Actual reference:** `Loot.Eligible` trong `SliceRules.cs`, `SliceSession.Die/Drop/LootCandidate/PickUp`, `SliceHost.Interact/LateUpdate` lootViews; `DomainTests.LootImmediateIndependentAndNextCandidate`, `LevelAtDeathSnapshotSurvivesLevelUpAndGapRejects`.
- **New contracts:** death/life IDs, immutable owner/contribution/level snapshot, original deadlines, weighted one-result payload Normal/Linh (weightsTUNABLE), valid-ground resolver, claim command/revision.
- **Minimal implementation:** một normal mob death→pile→pickup demo; chưa toàn Boss content.
- **Migration:** không port owner=1/Tutorial flag thành ACL; chuyển single-player fixture thành N recipients, reset expired pile không reroll.
- **Server:** valid snap/offset/bounds; giữ Normal/Linh 8/20/60, Boss 12/30/90 và top-damage/level/no-fallback; validate alive/range/map/ACL/deadline/whole capacity.
- **Client:** icon/prompt/selection và retirement sau claim commit; loot ActiveFocus thay combat selection; exact item/gen Pickup only, no samepress fallback.
- **Persistence:** G-D atomic death N rewards+pile+receipt và claim+bag+receipt; restore UTC deadline. A15 timestamp/cap release cần spike trước durable acceptance.
- **Dev Mode:** drop trên ledge/trong solid/Ong ở cao, windows owned/contributor/FFA, expired pile và delayed death ACK.
- **Unit tests:** threshold/window edges/level snapshot, capacity/ownership/generation, không fixed array 2 người.
- **Integration tests:** kill→roll một lần→reachable ground→commit pickup; late ACK/replay, không respawn trước finalize; claim giữ identity.
- **Multiplayer tests:** ≥2 clients tranh một pile chỉ một winner; outsider theo window, khác MapId, visibility không bypass ACL.
- **Manual acceptance:** icon/pile reachable, không trong tường; footprint/keyboard selection đọc được cùng personal placeholder.
- **DoD:** parity regular rules và demo nhỏ; network/durability chỉ nghiệm thu tại gate tương ứng.
- **Rollback risk:** clocks OPEN không thể đóng bằng visual demo; giữ versioned rolled payload, không reroll khi rollback.

### Slice 6 — Personal quest entitlement và bounded recovery

- **Why now:** local placed/mob fixtures mở Q2/Q4 và collection; không cần full Q8 trước.
- **Dependencies:** Slice 1+5; per-Quest state/schema review G-B. Sau local, chạy Dedicated G-N rồi Spring G-D.
- **Actual reference:** `SliceSession.Grant/pendingGrants/TutorialSupply/PickUp/Tick`, `Loot.Tutorial`; `DomainTests.PendingGrantKeepsInstanceAndReceiptAfterCapacityRetry`, `TutorialSupplyOnlyExistsAtRelevantActiveStep`; chưa backend durable.
- **New contracts:** bounded entitlement/epoch/source outcome success+failure/sourceMap/item payload/binding/staged grant/claim receipt; representation generation và TTL tách quyền nhận.
- **Minimal implementation:** một placed source và một mob collection fixture theo [phương án A](../02-technical/online-and-persistence.md#personal-quest-recovery); owner tới ground tự nhặt.
- **Migration:** version exact ItemIds/binding mapping; virtual fixture không tự thành picked up. Reset hoặc approved mapping một lần sang Pending, không reroll reward cũ.
- **Server:** đúng active objective group + MobIdentity/special predicate + threshold → one RNG outcome/eligible death/recipient/objective; success mới tạo quyền bounded, failure cũng persist; SpawnGroup/sourceMap là provenance/recovery, không standard kill whitelist; expose sau death commit. Pickup owner/lease/generation/map/range/alive/capacity; add+claim atomic; re-offer Pending tại valid sourceMap anchor.
- **Client:** owner-only visual và pickup intent; tracker pending/full-bag reason, không client grant/security bằng hide.
- **Persistence:** local fault fixture rồi PR adapter G-D riêng: transaction source/death→RNG outcome+entitlement và transaction claim→bag. Versioned progress payload/relation còn OPEN, spike chốt atomicity/queries/lease.
- **Dev Mode:** FillBag60, ExpireRepresentation, InspectEntitlement, duplicate/old-generation/ACK loss; DropTransport/Restart chỉ khi test infrastructure có thật.
- **Unit tests:** bag+pending bounds/RNG fail-success dedup/Q4 tutorialordinal/wrong step/owner/binding, double claim/capacity reject; TTL không xóa quyền; completed không respawn item.
- **Integration tests:** full bag→TTL→dọn→re-offer→pickup; death/map/new session; crash trước/sau source/claim commit với DB ở G-D, không dùng RAM để chứng minh.
- **Multiplayer tests:** N eligible có rights độc lập; outsider không read/claim; threshold 20% giới hạn tối đa 5 recipients/normal life; một claim/owner, stale lease/rejoin.
- **Manual acceptance:** lấy lại ở sourceMap rõ, không giết lại/NPC mới; regular/personal icons riêng và clickable/keyboard reachable, không auto-add vào bag.
- **DoD:** demo local, G-N ACL và G-D failure logs; chỉ durable done khi cả gate thực sự đạt.
- **Rollback risk:** binding revision phải giữ Pending rights/receipts hoặc quarantine test import; không downgrade sang virtual âm thầm.

### Slice 7 — Inventory-derived objectives và atomic turn-in

- **Why now:** nối collection possession vào quest, dùng fixture trước Q2; actual Q8/Q10/Q11 ở G-C.
- **Dependencies:** Slice1+6, C0 cho real MobKilled và typed full QuestDefinition review G-B (đủ stable counts/RNG/staged bindings/restoration OPEN/special predicates, Q9 parallel).
- **Actual reference:** `SliceSession.Objective/Stage/Kills/ObservePosition/TurnIn/Grant/Die`; `DomainTests.QuestStagedGrantRetryAndTurnInCapacityAreAtomic`; source chưa Q7–Q12/per-Quest definitions.
- **New contracts:** per-Quest state/groups, ItemRequirement(binding/identity/quantity/consumeAction); action receipts/Q4 tutorial ordinal/RNG outcomes/placed flags riêng, không collectionCounter authority thứ hai.
- **Minimal implementation:** một objective→bag→net-capacity turn-in demo; full 12 definition validation không có nghĩa một mega-PR chạy tất cả content.
- **Migration:** virtual fixtures reset hoặc approved Pending mapping; giữ QuestIds, counts Q3/Q4/Q5/Q12, thresholds và rewards; thay Q8/Q10 normal quota và Q11 flow cũ; Q9 parallel state không bị một currentQuestId ghi đè.
- **Server:** evaluate committed bag; death snapshot pin active group trước progress, không future-step credit; single-axis Kill hoặc CollectRNG cùng nguồn; Q8 Collect→Linh; Q10 Collect; Q11 material→Lâm→Bách/gate/grant→ba placements trên nhiều maps (OPEN), consume từng fragment+placed flag atomic; Q4 ItemSold history khác possession; Ready revalidate/resolve đúng NPC.
- **Client:** tracker phân biệt pending/ground/owned/action count; TurnIn pending/missing-item/capacity reasons; internal IDs chỉ debug.
- **Persistence:** consume+reward+Completed+unlocks+entitlement cleanup+receipt atomic; local reject fixture rồi DB fault thật G-D, matched definition revisions.
- **Dev Mode:** SetQuestGroup, GrantBoundItem, RemoveItem, FillNetCapacity, FailTurnIn, InspectReceipt; reset epoch invalidates stale claims.
- **Unit tests:** entitlement không là possession; wrong quest/mảnh; rights đủ nhưng chưa item không hoàn; Q4 sold vẫn hoàn; không duplicate collection authority.
- **Integration tests:** full bag consume tạo một slot rồi reward fit; reward cần hai slots reject toàn consume; lost ACK hoàn/thưởng một lần; Q11 placement consume+flag durable; gate đọc flags/approved endpoint, không possession ba mảnh; staged grant không duplicate.
- **Multiplayer tests:** cofarm vẫn riêng quest state; Pickup/TurnIn serialize revisions; Talk không share; G-D transaction faults thật.
- **Manual acceptance:** item thấy trong bag, không Sell/mất khi chết; turn-in thiếu item có reason, Q4 sample vẫn Sell đúng policy.
- **DoD:** fixture end-to-end và full definition validation, Q2 draft gated pending approval; Q8+ nghiệm thu G-C/G-T.
- **Rollback risk:** revision migration cao; rollback view không được xóa committed rights/history.

### Slice 8 — Q2 journey và Q6/NPC contextual route

- **Why now:** gameplay/content dựa trên physics, UI và recovery đã kiểm; mỗi quest là PR nhỏ.
- **Dependencies:** Slice 2+3+6+7; exact Q2 item/name/route cần duyệt trước production definition. Q6 both-talk direction đã xác nhận, không hỏi lại.
- **Actual reference:** `SliceSession.Anchors/QuestNpc/Interact/ChooseSword/ObservePosition`, `SliceHud.QuestIntro/QuestComplete/Dialogue/Npc/Marker`, `BlockoutLayout.Surfaces`, `PrototypePresets.Create`, `SliceRouteProbe.Route`, `ContinuousRouteTests`; Q3 Phong/Q6 Tạ Minh chỉ legacy reference.
- **New contracts:** approved Q2 placed source/destination/return (Vải Bọc Chuôi là draft chưa duyệt); Q6 two NpcTalked receipts/resolved mentor và contextual data table. Q6 dời mandatory Used sang lúc thiếu MP tự nhiên vẫn PROPOSAL riêng, không tự đổi definition.
- **Minimal implementation:** Q2 route và Q6 admission nối riêng vào fresh journey, không rewrite mọi NPC cùng lúc.
- **Migration:** giữ anchor/QuestIds; bỏ mandatory Drop không rename MapId. Migrate Tạ Minh references theo current roster; G-L Sword-only không là final Cung product.
- **Server:** Q2 một placed entitlement, pickup thật/atomic consume, không credit từ Jump/Drop presses. Q6 both-talk→manual unequip→class/grant/mentor commit; không trust selected mentor ID/off-class manual.
- **Client:** tracker mục đích/đường về; PrimaryAction NPC mởrootcontext, Talkoption explicit/noopeningcredit/modalconsume; service/context rõ, unselected mentor vẫn Talk và biết class sau Q6; không auto-face/relation meter.
- **Persistence:** discovery/class/mentor/learn receipts durable G-D; ambient readonly; Q2 epoch/claim/turn-in dùng flow chung.
- **Dev Mode:** FreshQ2/full bag placed item, Q6 one/both talked, Sword/Bow selected/unselected, late class/reserved MP; preset không thay fresh G-L video.
- **Unit tests:** chọn class trước both-talk fail, duplicate Talk không thêm resource; Q2 presses không credit; wrong-NPC/replay reject; context selectors.
- **Integration tests:** fresh Q1–Q6 Sword bằng Rigidbody/menu commands; Q2 fall retry/TTL; Q6 learn/cast/actual MP use/unselected Talk; Cung fixture rồi full route G-C.
- **Multiplayer tests:** một placed source tạo rights độc lập; class choice không share, NPC response đúng từng class; reconnect discovery/claim.
- **Manual acceptance:** journey có lý do, không precision platformer; 2–4 jumps là geometry proposal, Drop optional; hai mentor dễ tìm/ngang hàng, không off-class sale.
- **DoD:** approved Q2 definition + fresh G-L logs/video và keyboard/mouse usability; full Cung route G-C/G-T.
- **Rollback risk:** content revision giữ completed Q2/correct mentor receipts; không buộc chọn lại class.

### Slice 9 — Two-facing modular presentation integration

- **Why now:** đưa art probe đã làm sớm vào production rig, rồi mới mở rộng families.
- **Dependencies:** Pha R minimal KiếmS2 A/B và CungS2 ngang ưu tiên, Slice2 movement/shared rig + **C0 actual target/result contract trước full VFX**; G-N và A01/A02/A12 trước nhân families.
- **Actual reference:** `CombatController.Facing/TryStart`, `SliceHost.FixedUpdate/RenderPlayer`, `GeometricRig.Pose`, `SliceHud.DrawEquipment`; dead-gray outfit/Idle torso+weapon rotation là gap prototype.
- **New contracts:** Left/Right, Idle3/4, class upper poses/clock/keys/parts/sockets/carry; Right default/immutable action facing/life; SkillPresentationProfile main/impact/status/SuppressDefaultMainWeaponVfx/local-remote LOD theo Art, không per-skill system mới.
- **Minimal implementation:** tích hợp Master Pose và default/Áo I/Quần I + fixture variants nhỏ đã qua [Art probe](../03-art/art-and-visual-production.md#first-art-probe); Kiếm/Cung cùng pose system, không full catalog.
- **Migration:** Front Idle priority SUPERSEDED, giữ tám states/count 26; A01 timeline/raster OPEN, không dùng estimate Body/Armor lịch sử làm budget hoặc mass rename assets.
- **Server:** expose authoritative action/facing/terminal phase theo presentation spike; không gameplay damage/AnimationEvent/facing DB mới.
- **Client:** Idle giữ hướng, Run side hơn, Jump/Fall riêng 2+2, swing/draw upper thật; Back/Hand một representation, player pop/fall/shadow hide parts; NPC authored độc lập.
- **Persistence:** dùng existing gear/class/terminal/checkpoint contracts; lost session Right baseline khi chưa saved-facing requirement.
- **Dev Mode:** Left/Right stop, air/opposite action, swap/unequip/S2, duplicate death/late viewer/revive/life reset, fixture labels rõ.
- **Unit tests:** phase/facing/carry dedup/life-reset/pose mapping invariants; không test chỉ mirror PNG implementation.
- **Integration tests:** Unity import/pivot/socket/timing/pose-indexed part mix và visual revision; late/stale shadow đúng, Hit không restart clock; sprite không là damage collision.
- **Multiplayer tests:** remote clients, old-life action/late join/MapId/dead focus HP updates; PNG local không chứng minh network truth.
- **Manual acceptance:** Idle 3/4/action side dễ đọc; hand grip/hip/armor/weapon align hai hướng, không chỉ xoay weapon; camera/font ở scale thật.
- **DoD:** [modular acceptance](playtest-and-balance.md#modular-character-acceptance) và minimal Kiếm/Cung evidence, manifest, effort hours/% usable; A01 mapping/reuse trong26 logical frames được kiểm, không fake asset PASS; physics/mob death giữ.
- **Rollback risk:** presentation revision reset callbacks cũ, không rollback Inventory/class.

<a id="first-coding-slice"></a>

## Một coding slice nên làm đầu tiên

**Chọn Slice 1: production Inventory domain nhỏ + RAM command/receipt seam, 60 slots và compatible stack.** Đây không chỉ đổi 30→60 trong prototype. Nó mở net capacity/quantity/ownership/revision cho Shop/Pickup/TurnIn và test full bag độc lập, chưa cần chốt Q2 name/schema/26-raster mapping. G-B boundary review cùng Pha R prerequisites đi trước; deliverable đầu là harness chạy Add/Merge/Reject/Replay với logs/invariants và label RAM fixture. Fixed Jump tiếp theo trước Q2 authoring; C0 sau movement/base review và trước Slice5–7 actual kills hoặc full VFX. Không để Inventory slice thành chốt ngầm combat bounds. Đối chiếu VS-1 qua lịch sử Git khi cần; không gộp UI/DB/quests/network trong PR đầu.

<a id="map-info-work-package"></a>

## Map Info work package / revised UX scope — PLAN, chưa code

Gắn **Slice5 + Slice8** và PhaC/world integration, không slice/network framework mới: Slice5 có server population observer gắn SpawnManager/newlife/terminaldeath/despawn/natural cap; session-map snapshot+revision/delta/reconcile nối G-N; Slice8/HUD reuse small panel/unknown/loading state. Dependencies: C0 input/focus cho hunting UX, world lifecycle/due ordering, map/session generation replication; widget có thể fixture sớm nhưng fixture không pass authoritative count. G-D không persist client counts. DoD MI01/MI02 spawn/death/despawn/reconnect/mapchange/outoforder, no coordinates; original16fields/slice giữ.

Split/Sort-Merge/Discard planners trong Slice1, player UX Slice3, quantities Slice4 là **CURRENT DIRECTION planned integration**, chưa implementation. Exact canDiscard ordinary defaults và P0/P1 shipment priority **OPEN scope confirmation**, không vague “SortP1” rồi bỏ acceptance. Engineering recommendation kiểm invariants sớm/UX cùng inventory slice để không retrofit receipts; trade-off tăng Slice3 scope, không cắt baseline60/40 hoặc deviate task đầu Inventory domain. MapInfo cũng cần population integration thật, không chỉ Art budget. Q11 exact graph chờ user lock các OPEN ở Quest; không production content author bằng placeholder.

[Acceptance revision](playtest-and-balance.md#primary-action-acceptance) và [lootEV](playtest-and-balance.md#loot-model-transition) không runtime pass; drop weights/rates/quest pacing tune sau evidence. Natural Q8 không reservation task/retry queue, old source/test names trong Actualreference chỉ frozen archaeology.
