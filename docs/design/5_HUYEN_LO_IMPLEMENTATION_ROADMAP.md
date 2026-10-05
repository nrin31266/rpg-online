# Huyền Lộ — Roadmap triển khai và trạng thái hiện tại

## Tóm tắt

Roadmap sở hữu thứ tự triển khai, điều kiện qua từng giai đoạn và trạng thái CURRENT/DEFERRED. §1–6 hướng dẫn công việc hiện hành; §7–10 giữ lịch sử và bằng chứng của các revision trước, với routing lượt migration mới tại §8. TARGET vẫn có Kiếm, Cung và online đầy đủ, trong khung nguồn lực hai tháng/bốn người. VS-1 là bản mẫu disposable; production cần thiết kế lại.

## Tìm gì ở đâu

- [TARGET / CURRENT / DEFERRED](#target-current-deferred), [lát cắt local](#vs-1), [phase gates](#phase-gates).
- [Các phép kiểm revision mới](#revision-validation), [DEV SPEED và fresh-run acceptance](#dev-speed-acceptance).
- [Khung quản lý](#management-window), [art workflow](#art-workflow), [điều kiện production](#production-release).
- [Phụ lục lịch sử §7–10](#roadmap-history-appendix), [SOURCE → DESTINATION](#source-destination).

**Ngày đồng bộ:** 2026-10-06 · **Trạng thái:** DESIGN + PROTOTYPE VALIDATION. Công việc hiện hành là di chuyển và đồng bộ tài liệu theo thiết kế mới; production codebase chưa bắt đầu. Các gate của revision mới chưa chạy. VS-1 hiện có giữ nguyên trong lượt migration này.

[GDD](1_HUYEN_LO_GDD.md) sở hữu luật game; [Technical](2_HUYEN_LO_TECHNICAL.md) sở hữu hợp đồng triển khai; [Analysis](3_HUYEN_LO_DESIGN_ANALYSIS.md) sở hữu phân tích, bằng chứng và sổ quyết định; [Art](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md) sở hữu hình ảnh và quy trình sản xuất. Roadmap giữ điều kiện mở rộng và đường dẫn tới [hồ sơ bản mẫu](#prototype-visual-review). Không dùng lịch để tự sửa luật, duyệt đề xuất Art hoặc giảm nghiệm thu TARGET.

<a id="target-current-deferred"></a>

# 1. TARGET, CURRENT và DEFERRED

**TARGET** là toàn bộ P0 hiện hành: Tân Lữ, Kiếm và Cung; Lv 1–20 và Q1–Q12, trong đó Q9 tùy chọn; trang bị/kinh tế; năm map farm và ba khu hỗ trợ; Linh Biến, Boss, co-farm, chat và PvP. Kiến trúc đích là Unity Client + Dedicated Game Server + Spring Boot + PostgreSQL, có Login/Character Select, lưu tiến trình và khôi phục phiên. Nghiệm thu cuối theo [GDD §10](1_HUYEN_LO_GDD.md#acceptance-routing) cần tối thiểu hai client đồng thời với backend/DB thật. World và các danh sách nhận thưởng, nhặt đồ, threat phải an toàn với N người; hai client là mức kiểm tối thiểu, không phải trần người chơi hoặc tuyên bố capacity.

**CURRENT** là đồng bộ tài liệu sau khi thu bài học từ VS-1. Đường triển khai tiếp theo là **thu bài học bản mẫu → đồng bộ docs → probe cảm giác điều khiển/UI/art/rig → review production base → dựng local production slice → Dedicated với ít nhất hai client sớm → mở rộng production**. Local-first giúp kiểm luật và tương tác sớm; TARGET vẫn là game online. CURRENT biểu thị việc đang ưu tiên, không tự có nghĩa đã code hoặc đã pass.

Bản mẫu cũ đã có các lượt test và video theo revision riêng. Trạng thái G-L PARTIAL trong hồ sơ cũ không chứng minh G-L production theo thiết kế mới. Chưa có bằng chứng runtime mới cho controls, cadence, mật độ, địa hình hay NPC/quest đã sửa.

**DEFERRED** là hạng mục vẫn thuộc TARGET nhưng chưa nằm trên đường phụ thuộc đầu tiên. Cung là P0. Giữ toàn bộ skill, projectile, gear, pose/VFX và các phép kiểm Kiếm/Cung. Slice sớm triển khai Kiếm trước; Cung vẫn được giới thiệu trong Q6 và có thể ghi “Chưa mở trong bản thử nghiệm” khi chưa playable. Nhãn giới hạn này không dùng trong sản phẩm cuối.

| Nhóm | Trạng thái hiện hành | Điều kiện quay lại / mở rộng |
| --- | --- | --- |
| Tân Lữ → Kiếm, movement, chọn skill và ExecuteSelected | CURRENT docs và kế hoạch probe; chưa triển khai revision mới | Review production base, rồi kiểm G-L theo controls mới ở tốc độ thường |
| Vân Khê, Học Viện, Đồng Sương; NPC/Nấm/Sói/Dummy/UI | CURRENT phạm vi blockout và local slice; layout VS-1 cũ chỉ là reference | Kiểm tuyến Q1–Q6, ≥3 Dummy, địa hình mới, mật độ và khu chức năng NPC |
| Kiếm Lv 10/13/17, Q7–Q12 và các map sau Đồng Sương | DEFERRED khỏi slice đầu, vẫn P0 | G-N và G-D; fixture hẹp chỉ kiểm kỹ thuật, không thay hành trình thật |
| Cung playable, skill/gear/projectile/pose/balance | DEFERRED IMPLEMENTATION, vẫn P0 | Probe Cung và so Kiếm/Cung sau G-N, trước nhân toàn bộ family; giải các OPEN liên quan |
| Dedicated với ≥2 client | Gate sớm sau G-B và G-L revision mới; chưa bắt đầu | G-N trước mở production content/art rộng, trên production base |
| Login/Character Select, Spring/PostgreSQL, ticket/lease/checkpoint/reconnect | DEFERRED khỏi slice local, vẫn P0 | G-D sau gate mạng; adapter RAM không thay DB thật |
| Co-farm/chat/shared claim, Linh Biến và Q8 | DEFERRED khỏi slice đầu | G-N kiểm N recipients/MapId; G-D kiểm commit; G-C mở route/content |
| Boss và tải hình ảnh online đầy đủ | DEFERRED khỏi slice đầu | G-C → G-F; giữ luật Boss và kiểm telegraph/camera/TTK thật |
| PvP/Q9/escrow/settlement | DEFERRED khỏi slice đầu, vẫn P0 | G-D trước G-P; Q9 optional không cho phép bỏ hệ PvP |
| Buff mới, shield/groggy, QoL/P1/P2 | Đề xuất chưa duyệt | Tra Analysis; phím R trong proposal controls hiện dành cho Food, không suy ra Buff R đã duyệt |

Lượt migration này chỉ sửa tài liệu canonical. Không sửa bản mẫu, sinh asset, nối mạng hoặc chạy lại build/test để gọi thiết kế mới đã pass. Các chỉ thị cũ về dừng A/B, evidence work hay Phase H ở §8–10 là lịch sử của phiên trước, không phải trạng thái các gate mới.

<a id="vs-1"></a>

# 2. Lát cắt local đầu tiên và reference VS-1

**BASELINE phạm vi G-L:** Q1–Q6, Tân Lữ → Kiếm, ba map **Vân Khê / Học Viện / Đồng Sương**. Hướng triển khai vẫn Kiếm trước → local trước → gate mạng sớm. Trúc Ảnh mở trong quest state sau Q6 nhưng nằm ngoài build slice đầu; MapExit phải báo giới hạn bản thử. G-L không thêm Q7/enhance để thay mục tiêu đang kiểm. Phạm vi có thể được review sau blockout nếu bằng chứng cho thấy cần đổi; khi đó cập nhật mục này, G-L và kế hoạch kiểm liên quan.

**Phạm vi slice không phải phạm vi định nghĩa quest.** Production phải có đầy đủ 12 `QuestDefinition` cho Q1–Q12, điều kiện, bước hành động, thưởng, NPC nhận/trả và mở khóa theo [GDD §5](1_HUYEN_LO_GDD.md#quests-story). G-B review cách biểu diễn cả tuyến; G-L chỉ chạy đoạn Q1–Q6. Những quest sau phải nằm trong tuyến production thật ở G-C/G-T, không được thay bằng vài con số tracker hoặc preset debug.

VS-1 hiện có là reference của luật/layout/input cũ, không phải production architecture hay art acceptance. Không mang nguyên các bờ dốc, đất one-way hoặc phím alias cũ vào slice revision mới. Probe art/rig mới dùng một sandbox standalone disposable theo [Art](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#first-art-probe); bản VS-1 hiện có giữ nguyên trong migration này. Sandbox art chỉ kiểm hình ảnh/pipeline, không thay production base hoặc route G-L.

Tuyến Q1–Q6 mới: Lâm Bá hướng dẫn qua khu Yên Thảo → Bách Luyện → Mộc An rồi nhận bài nhảy/drop-through ở Học Viện. Q3 nhận và trả tại Bách Luyện, dùng Mộc Kiếm hạ ba Dummy. Q4 đánh Nấm, nhận/mặc Áo và bán sample; Q5 gặp Yên Thảo, chuẩn bị Food/Potion và đánh Sói tới mốc Lv5. Q6 do Lâm Bá giới thiệu; người chơi tự tháo vũ khí cũ, tới Phong Du hoặc Diệp Lam để chọn phái, nhận vũ khí/bí kíp, trang bị/cộng điểm/học, dùng S1 và Bình MP rồi trả tại mentor phái đã chọn. Slice Kiếm chạy Phong Du; production Cung chạy Diệp Lam. Không khôi phục Tạ Minh làm bước trung gian.

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

**Chưa bắt buộc cho G-L:** login/backend/DB, network/reconnect, Cung playable, PvP, Boss và các map sau Đồng Sương. Local dùng profile/fixture và RAM; phải báo dữ liệu có thể mất khi reset/đóng phiên. JSON chỉ phục vụ config/fixture/import-export dev. Thành công trong RAM chưa chứng minh crash atomicity hay persistence.

Dummy giữ HP 60/respawn 25 s và ≥3 placements đồng thời để Q3 không buộc solo chờ. Online contention và các phương án DEF/EVA/timer khác vẫn cần kiểm, không đổi normal farm respawn. Gravity/momentum liên tục vẫn giữ. Pose và quyền cast S2/S3 khi nhân vật đang trên không vẫn là OPEN; không ép movement lock để giảm công vẽ.

<a id="phase-gates"></a>

# 3. Các phase và gate mở rộng

Gate chỉ pass khi có bằng chứng đúng revision. [Technical §10](2_HUYEN_LO_TECHNICAL.md#technical-gates) giữ setup và contract kiểm; trạng thái quyết định gameplay nằm ở Analysis. G-x dưới đây là gate triển khai. **Hiện chưa có gate production theo revision mới được xác nhận pass.**

| Phase / gate | Điều kiện vào và công việc | Điều kiện ra / cho phép tiếp theo |
| --- | --- | --- |
| Pha R — thu bài học và probe | Thu findings VS-1, đồng bộ docs; probe controls/UI/art/rig trong sandbox disposable riêng | Có contract hiện hành, danh sách giả định cần đo và mẫu nhỏ đọc được; không biến mock thành production base |
| Pha B / **G-B: production base review** | Review input/intent/pending/clock/IDs/definitions/physics/commit/presentation; review toàn bộ Q1–Q12 và class/NPC dependencies rồi dựng base nhỏ | Ownership/dependency rõ; kiểm core theo revision mới; room production sơ bộ; quyết phần reuse/rewrite. Không dựng framework chỉ vì đối xứng |
| Pha L / **G-L: local slice revision mới** | G-B pass; ghép Q1–Q6/Kiếm từ base, blockout ba map và kit đủ đọc | Route fresh Q1–Q6 cùng log/video, review feel/UX ở tốc độ thường; các phép kiểm áp dụng trong bảng dưới đạt. Art/QA/rework có số đo; mở G-N |
| Pha N / **G-N: Dedicated + ≥2 client sớm** | G-B/G-L mới pass; rules/resolver/timeline chạy headless; adapter RAM ghi rõ fixture dev | Hai client độc lập kiểm movement/MapId/kill/quest/shared claim, stale life/replay/late result, dead-focus và HP người chơi khác; N-safe recipients/claim/threat. Đo correction/latency/headless; mở rộng có chọn lọc |
| Pha D / **G-D: backend/persistence thật** | G-N pass; thay fixtures bằng Spring/PostgreSQL, giữ một writer và domain result chung | Login → Select → one-time ticket → join; lease/duplicate, checkpoint/SafeAnchor/HP0; N recipients, claim/quest commit idempotent; crash/outage/retry trước/sau ACK. ClassChosenLevel và class transaction bền vững |
| Pha C / **G-C: content và hai phái** | G-N trước art rộng; G-D trước nghiệm thu route dài có dữ liệu bền | Kiếm và Cung, skill/passive, gear HP/MP mới; Q7–Q12, bảy mob identities/sáu rigs, các map/Linh/Q8 đúng GDD. Chạy đủ định nghĩa quest trong tuyến thật; kiểm balance/pose trước nhân variants |
| Pha F / **G-F: Boss và tải online** | Content/skill/world đã ổn; schema/lifecycle không đổi lớn | Lịch/telegraph/Slow/Cuồng, corpse/loot/credit/camera đúng; chơi thật ≥2 người và probe 3–4+ để tìm giả định fixed-pair. Ghi máy/build/CPU/bytes/latency, TTK và journey; không suy capacity từ ca pass |
| Pha P / **G-P: PvP/chat và ghép online** | G-D trước escrow; movement/combat/recovery đủ ổn | 10 stakes, escrow hai bên, Food/quota, timeout DRAW/forfeit/abort, settlement/crash retry; Map Chat/reconnect/standalone package. Q9 vẫn optional trong route |
| **G-T: nghiệm thu TARGET** | Hai phái và toàn P0 đã ghép; các gate phụ thuộc có bằng chứng | Đối chiếu GDD §10/Technical §12 trên Client + Dedicated + backend/DB thật, ≥2 người đồng thời; full fresh-run Q1–Q12 từng phái theo mục dưới. Không gọi TARGET done khi thiếu Cung, Q12/Boss/PvP/recovery |

G-N dùng phạm vi local đã kiểm lại và diễn ra trước khi làm rộng art/content. Không chờ xong toàn bộ Cung/gear/Boss mới kiểm authority trên Dedicated. Spike không backend chỉ là fixture dev; G-D vẫn bắt buộc trước khi nhận persistence/reliability là hoàn thành.

<a id="revision-validation"></a>

## 3.1. Các phép kiểm bắt buộc sau migration

Đây là những dependency để đóng gate triển khai, không phải sổ quyết định mới. Kết quả cũ chỉ chứng minh revision cũ; tất cả dòng dưới **chưa có bằng chứng runtime theo revision mới**. Các số TUNABLE và phần OPEN phải được ghi cùng setup để người review biết đang thử giả định nào.

| Nội dung cần kiểm | Kết quả đủ để review | Gate phụ thuộc |
| --- | --- | --- |
| Controls và ExecuteSelected | Phím mũi tên điều khiển movement. 1/2/3 chỉ chọn, không tiếp cận/cast/tiêu MP/đặt CD. Execute riêng tạo lệnh một lần; đổi slot không sửa snapshot đã có. Kiểm giữ phím, pending/buffer, hủy lệnh và lúc tới tầm; log/video ghi bindings cùng review bàn phím/chuột. Đề xuất E Execute/F Interact/4–5 Potion/R Food/I menu còn TUNABLE, exact keys OPEN | G-B/G-L; G-N kiểm intent qua mạng |
| Focus khi chết và HP observer | Người chơi chết hủy lệnh combat nhưng giữ focus hợp lệ, marker và HP hiện tại/tối đa; HP vẫn cập nhật khi người khác đánh target. Target chết, despawn, sai life/map hoặc hết điều kiện giữ phải xóa focus đúng. Respawn cùng slot không kế thừa focus đời cũ | G-L; G-N với ≥2 client |
| Cadence và vai trò S1/S2/S3 | Baseline TUNABLE: S1 0,60 s/2 MP, S2 0,90 s/3 MP, S3 6 s/16 MP; Cung S2 0,70/0,60/0,50 power. Kiểm S2 dùng thường xuyên để farm, thời gian khóa hành động, lúc resolve, đánh nhóm/trạng thái, mức tiêu và hồi MP, TTK. Không thêm đòn thường 0 MP sau chọn phái | G-L cho phần Kiếm đã có; G-C/F cho skill và hai phái đầy đủ |
| HP và gear mới | So hai phái ở cùng cấp/trang bị/điểm, chọn phái đúng Lv5 và chọn muộn. Cung tăng +8 HP/level sau ClassChosenLevel thực là baseline TUNABLE; HP không tụt khi chọn phái, VIT vẫn +8. Hướng ba slot HP/ba slot MP cần tính lại qua rarity/enhance/chuyển giao với các giá trị TUNABLE. Kết quả sustain/Boss cũ không pass giá trị mới | G-B/D kiểm dữ liệu và transaction; G-C/F kiểm balance |
| Mật độ và hành vi cụm | Tác giả nhiều cụm nhỏ độc lập theo [Kế hoạch mật độ GDD §4](1_HUYEN_LO_GDD.md#world-farm) (Đồng Sương 8–10, Trúc Ảnh 9–11, Bạch Vân 8–10, Xích Nham 9–11, Huyền Tích 8–10 cụm thường + 1 Boss riêng; authored IDs mới `DS7+`, `TA7+`, `BV6+`, `XN7+`, `HT6+`). Bảo toàn các Quest Anchor IDs (`DS2`, `DS3–DS6`, `TA4`, `TA6`, `TA4.slot1`, `XN1–XN6`, `HT4–HT5`, `HT_BossLandmark`) cùng các stable seed IDs (`TA5`, `BV1–BV5`, v.v.); thực thi Boss Exclusion cấm quái thường trong `BossCombatArea`. Kiểm camera bao quát 5–8+ quái trên nhiều thềm, nhịp tiếp xúc, độ đọc và tranh chấp bãi co-farm. 28 cụm / 66 slots là seed manifest lịch sử; tổng số cụm và Hybrid count/identity còn OPEN/TUNABLE. Chỗ có quái (occupied) khác terrain bị chặn (blocked); kiểm staging/Return, chọn threat tới được và kiting hợp lệ. Không thêm AI riêng chống Cung | G-L trên phần có mặt; G-N co-farm; G-C trên năm map |
| Địa hình và kiểm thử 8 map | Đất/đá tự nhiên là khối solid dày, mặt đi ngang/mặt đứng trực giao; không mặt đi dốc/ramp/tam giác, đất one-way hay cơ chế leo. One-way hiếm chỉ ở kết cấu mỏng nhân tạo có chống đỡ rõ ràng. Kiểm thử nhảy/drop-through/EdgeExit chống giật lặp chuyển cảnh (anti-pingpong transition); kiểm thử nước nông làm chậm nhẹ khi chân tiếp xúc và không làm chậm khi đi trên cầu gỗ/trên không; kiểm tra 8 MapRoots không chồng lấn collider; kiểm tra BossCombatArea cấm tuyệt đối quái thường; GroundMelee không rơi/nhảy/drop giữa tầng | G-L trước G-N; G-C kiểm phần map mở thêm |
| NPC và tuyến chọn phái | Bảy NPC ở khu chức năng (Lâm Bá, Yên Thảo, Bách Luyện, Mộc An, Phong Du, Diệp Lam, Hạo Vũ). Q3 tại Bách Luyện; Q6 Lâm Bá giới thiệu, nhập phái và trả tại mentor phái đã chọn; Q10–Q12 Lâm Bá, Tẩy Mạch tại Yên Thảo. Người chơi tự tháo/mặc/học/dùng đồ; kiểm từ chối, retry và túi đầy, không dùng preset để bỏ bước | G-B review định nghĩa; G-L Q1–Q6; G-C/G-T hai route |
| Full quest definitions và fresh-run | Đủ Q1–Q12 definitions và tuyến production thật; giữ IDs/anchors/nguồn credit ổn định. Q9 tùy chọn có ca online riêng. Mỗi phái có route fresh hợp lệ với hành động, thưởng và mở khóa thật; không chỉ đặt tracker hoặc dùng dev tool hoàn thành prerequisite | G-B về schema/definition; G-C/G-T về tuyến đầy đủ |
| Rig/UI và độ đọc theo luật mới | Slot được chọn đọc rõ và độc lập hành động đang chạy; S2 có feedback đủ đọc ở nhịp mới; HUD vẫn hiện focus khi chết. Kiểm pose/socket/terrain/crowd trong room thật; chưa giải A01/A02/A12/A14 thì chưa nhân family dựa trên giả định | G-L với mẫu nhỏ; G-C/F trước mở rộng hình ảnh |

<a id="dev-speed-acceptance"></a>

## 3.2. DEV SPEED và ACCEPTANCE EVIDENCE

[Dev Mode trong Technical](2_HUYEN_LO_TECHNICAL.md#dev-mode) là tooling dev-only để rút thời gian thử: đặt level/quest/class, hoàn thành prerequisite, cấp item/skill, teleport tới marker đã author, reset encounter/group/Boss hoặc force Linh Biến. Nó không thuộc UI production cho người chơi, không lưu như tiến trình hợp lệ và không bypass authority release. Kết quả có preset phải ghi là fixture/probe.

**DEV SPEED** cho phép đi thẳng tới tình huống để tìm lỗi và so phương án. **ACCEPTANCE EVIDENCE** chứng minh người chơi thực hiện được hành trình bằng các hành động hợp lệ. Ca dùng preset có thể pass kiểm kỹ thuật hẹp, nhưng không pass fresh-run route.

G-L cần fresh-run Q1–Q6 của Kiếm từ trạng thái đầu hợp lệ. G-C/G-T cần fresh-run **toàn Q1–Q12 cho từng phái Kiếm và Cung**, từ tạo/chọn nhân vật tới chọn phái, nhận/trả quest, gear/skill/supply và Boss/credit đúng tuyến. Q9 optional không chặn truyện chính; phải có ca PvP/Q9 riêng để nghiệm thu hệ đó. Không dùng SetQuestState/GiveItem/CompletePrerequisite/teleport dev để thay bước trong bằng chứng route. Kiểm retry/recovery có setup riêng và phải phân biệt với video hành trình fresh.

Mỗi hồ sơ cần ghi revision/build, phái, trạng thái khởi đầu, bindings/setup, log kết quả authority và video/phần review ở tốc độ thường. Ghi rõ bước nào đã quan sát và gate nào còn thiếu. Automation bổ trợ kiểm logic; cảm giác điều khiển, layout và usability vẫn cần review của người chơi.

<a id="management-window"></a>

# 4. Khung quản lý hai tháng, nhóm bốn người

**Hạn nguồn lực thực tế: hai tháng/bốn người.** Dùng khoảng tám tuần quản lý kể từ lúc bắt đầu thực hiện. Chưa có ngày bắt đầu hoặc số giờ khả dụng từng người nên chưa thể suy deadline lịch/tổng person-hours. Bảng dưới là mục tiêu quản lý, không cam kết toàn TARGET chắc chắn hoàn thành trong hai tháng.

| Tuần mục tiêu | Đầu ra để review | Điều kiện / xử lý nếu chưa đạt |
| --- | --- | --- |
| 1 | Thu findings bản mẫu, sync docs; probe controls/UI/art/rig và review base nhỏ | Ghi assumptions, OPEN và công sửa/% dùng được thật; VS-1 giữ vai trò reference |
| 2 | Core production base/local room → G-B; definitions Q1–Q12, input/clock/IDs/physics/AI/kit | Review select-only/ExecuteSelected, class/NPC dependencies, terrain/mật độ mới; chưa nhân toàn bộ family |
| 3 | Ghép Q1–Q6 Nấm→Sói/Kiếm, blockout ba map, art/UI nhỏ → G-L revision mới | Fresh route, review tốc độ thường và logic retry/death; thiếu UX/art thì gate còn PARTIAL |
| 4 | Dedicated + ≥2 client trên slice → G-N | Checkpoint mạng trước production rộng; nếu boundary/headless/recipients sai, sửa trước mở rộng |
| 5 | Spring/PostgreSQL/auth/ticket/lease/commit/recovery → G-D mục tiêu; probe Cung hẹp nếu G-N pass | Chưa pass DB thật chưa nhận persistence done; Cung vẫn DEFERRED P0 nếu chưa triển khai |
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

# 5. Art cho người chưa thạo vẽ và kỷ luật giao việc

Dùng quy trình nhỏ ở [Art §23.1](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#art-tool-workflow): chọn mẫu, sửa palette/outline/pivot/grip bằng editor pixel, nhập Unity rồi đo. Probe mới chạy trong sandbox standalone disposable riêng; đây là công việc sau migration, không sửa VS-1 hiện có hoặc sinh asset trong lượt tài liệu này.

**TOOL CANDIDATE:** có thể thử PixelLab bằng Free/free trial rồi đánh giá tool/workflow. Contract visual/import và architecture không phụ thuộc PixelLab. Giới hạn dịch vụ cần kiểm lại từ [FAQ chính thức](https://www.pixellab.ai/docs/faq) tại thời điểm dùng; không giả tool animation/outfit đều miễn phí hoặc hứa số credits. Nếu tính năng cần thiết không có trong Free, ghi “chưa kiểm được với Free”, dùng placeholder kiểm pipeline và đo chi phí trước quyết định mua.

Đếm cả output thất bại. `% dùng trực tiếp = output pass không sửa / toàn bộ output tạo`; `% dùng sau sửa = output pass sau sửa / toàn bộ output tạo` là hai nhóm riêng. `% dùng được tổng = tổng hai nhóm pass / toàn bộ output tạo`, không đếm một output hai lần. Chưa tạo mẫu ghi **CHƯA ĐO**. Ghi giờ sửa/import/QA, lỗi pose/alignment và cỡ mẫu cho từng loại. Gate cần mẫu rig/weapon/outfit chạy đúng timing trong room thật, không chỉ một PNG đẹp. Không tạo tool plan, prompts, assets hoặc manifest riêng trong vòng docs này.

Kỷ luật cho mọi người và coding agent nằm ở [Technical §1.1](2_HUYEN_LO_TECHNICAL.md#architecture-discipline): gameplay tách UI, ID ổn định, một clock authority/session; AnimationEvent chỉ presentation; Local và Dedicated dùng cùng rules. Task ghi rõ TARGET/CURRENT, section canonical, input/result, gate cần kiểm và OPEN dependency. Không tự đổi range/timer/26-frame/slot để làm task pass. Giá trị BASELINE/TUNABLE phải có setup và bằng chứng khi đề xuất chỉnh; sổ quyết định ở Analysis, luật đã duyệt sync GDD.

<a id="production-release"></a>

# 6. Khi nào mở production và điều gì được hoãn

Trước G-N chỉ làm mẫu art nhỏ đủ kiểm: room art, default/outfit I, Mộc/Kiếm và vài mob/kit primitives. G-L vẫn cần blockout chơi liên tục đủ ba map; room art không thay hành trình. Trước nhân mỗi family sau G-N cần evidence pose/socket/readability và xử lý các OPEN liên quan trong phạm vi đó. G-N không tự duyệt Hybrid count, 33 pose, 13–25 weapon images, camera hoặc tổng terrain/pocket mới.

Cắt P1/P2 và polish trước: giảm cosmetic variations/shake/sound/phần trình bày cầu kỳ, giữ thông tin gameplay/pending/status/telegraph, Storage core và Journey scores. Cung/Boss/backend/PvP hoãn khỏi slice đầu theo thứ tự hiện hành, không xóa khỏi TARGET. Review cuối tuần 2/4 và sau mỗi gate bằng công còn lại. Nếu TARGET vượt hai tháng, trình phạm vi bản thử và lịch tiếp theo cụ thể; không hạ nghĩa final DoD. Rationale, bảng số và workload cũ tiếp tục được giữ trong phần trace.

<a id="legacy-roadmap"></a>

<a id="roadmap-history-appendix"></a>

# Phụ lục — lịch sử roadmap và hồ sơ prototype

§7–10 giữ các bảng, số đo và routing lịch sử, kèm nhãn để phân biệt với thiết kế hiện hành. Riêng §8 có thêm bảng routing migration 2026-10-06. Các chữ CURRENT, mặc định OFF, flags/A/B và version trong phần lịch sử mô tả thời điểm cũ. Controls, cadence, NPC/quest, mật độ và terrain đã được thay ở revision tài liệu 2026-10-06; bằng chứng cũ không pass gate mới. Luật đọc GDD; thứ tự và trạng thái hiện hành đọc §1–6.

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

<a id="canonical-migration-2026-10-06"></a>

## Migration canonical 2026-10-06

Đây là routing của lượt migration hiện hành từ master prompt và các quyết định owner đã chốt. Nội dung luật, số tính lại và contract nằm tại các đích canonical; bảng này chỉ ghi cách xử lý để review, không tự chứng minh gate đã pass. Những bảng consolidation cũ phía sau tiếp tục giữ làm lịch sử.

| SOURCE cần thay / đồng bộ | DESTINATION sở hữu nội dung hiện hành | Cách giữ và trạng thái kiểm |
| --- | --- | --- |
| Controls chọn+cast, aliases và focus cũ | [GDD input](1_HUYEN_LO_GDD.md#focus-input), [Technical contract](2_HUYEN_LO_TECHNICAL.md#input-contract), [gate revision](#revision-validation) | Thay bằng chọn/thực thi riêng, giữ focus hợp lệ khi chết; controls cũ ở §8/§10 có nhãn lịch sử |
| Cadence S1/S2/S3, HP Cung và gear HP/MP cũ | [GDD combat](1_HUYEN_LO_GDD.md#class-combat), [gear](1_HUYEN_LO_GDD.md#gear-economy), [Analysis probe mới](3_HUYEN_LO_DESIGN_ANALYSIS.md#current-balance-probe) | Baseline mới TUNABLE; số cũ giữ tại Analysis với nhãn LEGACY. Tính lại không phải runtime acceptance |
| Density 28/66, crowd và capability quái | [GDD world/crowd](1_HUYEN_LO_GDD.md#world-farm), [Analysis farm](3_HUYEN_LO_DESIGN_ANALYSIS.md#farm-progression), [Technical combat](2_HUYEN_LO_TECHNICAL.md#combat-data) | Giữ seed IDs/quest sources; re-author mật độ, tổng và Hybrid count OPEN; kiểm Home/Walk/Return dùng chung |
| Dốc, đất one-way và nhu cầu climb cũ | [GDD terrain](1_HUYEN_LO_GDD.md#terrain-rules), [Art map](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#map-visual), [Technical maps](2_HUYEN_LO_TECHNICAL.md#maps) | Solid tự nhiên trực giao, one-way kết cấu hiếm, không climb; hình/layout VS-1 cũ chỉ là reference |
| Tám NPC, Q3/Q6 và NPC cuối truyện cũ | [GDD quest](1_HUYEN_LO_GDD.md#quests-story), [Art NPC](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#npc-visual), [slice/gates](#vs-1) | Bảy NPC theo khu chức năng; mentor phái giữ class transaction/turn-in Q6. Quest IDs, thưởng/credit/anchors được giữ |
| Scope slice bị hiểu thành scope quest / preset debug | [Technical Dev Mode](2_HUYEN_LO_TECHNICAL.md#dev-mode), [DEV SPEED / acceptance](#dev-speed-acceptance), [phase gates](#phase-gates) | Đủ 12 definitions/tuyến production; Q1–Q6 chỉ là slice đầu. Full fresh-run từng phái không dùng preset |
| CURRENT/evidence/prototype bị hiểu thành production | [TARGET/CURRENT](#target-current-deferred), [hồ sơ lịch sử](#prototype-visual-review), [README routing](README.md) | Docs sync 2026-10-06, không bump prototype; old evidence không pass revision mới. Local-first → G-N ≥2 sớm, giữ TARGET N người |

## Trace consolidation trước cleanup

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

Các replacement sau ghi review được user duyệt 2026-10-03. Đây là lịch sử sync; các mô tả select+cast, aliases, E interact và Hybrid 3/1/0 dưới đây không còn là contract hiện hành. Không giữ artifact tạm như authority thứ sáu. Lịch sử simulation/art/accounting trong Analysis/Art/§7–9 vẫn đủ số, chưa nghiệm thu runtime.

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

**LEGACY / SUPERSEDED — control acceptance V6.2.1:** A/D+arrows OR; Space/↑ jump và S/↓ drop; 1–3 select/one-press approach+cast, no J/no-repeat mọi skill, locked slots2/3 trong route. One-action/snapshot/latest buffer, AUTO/EXPLICIT, pending không đổi target; release giữ one-shot, manual/focus/UI/Esc/map cancel. E candidate độc lập; EdgeExit auto không E. Manual feel/usability và revision-matched evidence bắt buộc. S2/S3/Cung production DEFERRED. Không tăng số balance để làm button đẹp; giữ role/sustain gate.

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

**Trace feedback 2026-10-03; những mapping/input trong bảng có thể đã bị thay bởi thiết kế 2026-10-06.** Mốc trước sửa `archive/checkpoint-46006c4` (local-only archive, không có trên origin); request feedback 2026-10-03 cho phép sửa tạm và tự xử lý inconsistency. Các đích dưới là nội dung thực, không copy feedback thành authority thứ sáu. Các quyết định numeric/feel chưa có evidence giữ TUNABLE/PROTOTYPE ở Analysis.

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

Mốc Git trước sửa: `archive/checkpoint-52d7a4a` (local-only archive, không có trên origin) (`docs: snapshot art analysis before consolidation`). Bản gốc và script/snapshots/metrics/migration proofs lưu tại `/tmp/huyenlo-consolidation/`; thư mục tạm không thuộc deliverable repo và có thể mất sau phiên. Các kết quả cần review được giữ ngay trong mục này, không tạo audit/plan/manifest riêng trong active docs.

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

**Snapshot lịch sử V6.2.7; G-L của bản mẫu cũ PARTIAL.** Layout, tọa độ mock, lịch sử V6.2.4–V6.2.7 và bằng chứng đã chuyển sang [CHANGELOG prototype](../../prototypes/VS1_EndToEnd/CHANGELOG.md#prototype-visual-review). Bản nháp V6.2.8 trong hồ sơ chưa được xác minh. Metadata docs 2026-10-06 không phải prototype version bump; không xác nhận bản nháp hoặc gate mới đã pass. Toàn bộ chi tiết được bảo toàn, gồm chỉnh sửa cảnh quan chưa commit có trước task này; tests cũ không là visual/feel acceptance.

**Snapshot vận hành trước migration:** bản chạy thử cũ dùng một hành vi đã duyệt, không ProbeConfig/flag/metric harness; F8/debug chọn giai đoạn và reset đã được khôi phục, chỉ cho prototype local. Chỉ thị khi đó là dừng A/B, evidence work, hook/Git cleanup và Phase H. Đây là lịch sử, không mở hoặc đóng gate revision mới; budget release và TARGET vẫn theo tài liệu canonical hiện hành.

| Revision | Tóm tắt lịch sử |
| --- | --- |
| V6.2.4 | User từ chối layout khối/nước; giữ làm trace. |
| V6.2.5 | User từ chối hiểu “núi” thành ngoại cảnh. |
| V6.2.6 | User từ chối slab solid, bridge clearance và cách đọc tầng đất. |
| V6.2.7 | Snapshot bản mẫu được ghi trong hồ sơ cũ; visual/feel chưa nghiệm thu, không production base. |

**Trace INPUT-01/CMB-01/MOBAI-01 của revision cũ:** A/B lúc đó mặc định OFF, [báo cáo probe](../../prototypes/VS1_EndToEnd/CHANGELOG.md#phase-e--kết-quả-ab-và-giới-hạn). Cung gameplay vẫn DEFERRED thuộc TARGET P0; chỉ có ranged fixture. Không nối network/backend vào lớp throwaway. G-B nhận findings sau review, không lấy test pass để mở G-L.

Các anchor dưới đây giữ routing lịch sử cho tài liệu read-only; nội dung thực ở CHANGELOG:

<a id="prototype-technical-history"></a>

[Technical — lịch sử probe](../../prototypes/VS1_EndToEnd/CHANGELOG.md#prototype-technical-history).

<a id="prototype-feedback-history"></a>

[Analysis — lịch sử feedback](../../prototypes/VS1_EndToEnd/CHANGELOG.md#prototype-feedback-history).

<a id="prototype-map-history"></a>

[Art — layout mock](../../prototypes/VS1_EndToEnd/CHANGELOG.md#prototype-map-history).

<a id="prototype-ui-history"></a>

[Art — UI/rig mock](../../prototypes/VS1_EndToEnd/CHANGELOG.md#prototype-ui-history).

<a id="prototype-water-history"></a>

[GDD — lịch sử tuning nước](../../prototypes/VS1_EndToEnd/CHANGELOG.md#prototype-water-history).

<a id="prototype-tooling-history"></a>

[Technical — tooling mock](../../prototypes/VS1_EndToEnd/CHANGELOG.md#prototype-tooling-history).

<a id="prototype-runtime-history"></a>

[Analysis — runtime và sequencing](../../prototypes/VS1_EndToEnd/CHANGELOG.md#prototype-runtime-history).

**LEGACY / SUPERSEDED — feedback prototype 2026-10-05 về địa hình/focus/actor:** vũng Vân Khê chữ nhật dài hơn, có đường đất one-way phía trên để chọn đi khô/lội nước; cầu giữ đường đi solid và nâng mặt nước hình ảnh, không thêm collision dưới nước. Thân đất liền, mặt cỏ/đá lát nông khác nhau; bỏ bờ tam giác và bậc vụn ở quảng trường. Dùng lại rig hình học từ `50f05ed`, cache renderer thay vì dựng mỗi frame; NPC/Sói/Dummy đặt chân đúng support. Hướng dẫn phím world chỉ một panel. Tab/click search thử ±12 u ngang/±6 u dọc; retention ±20/±10, độc lập range/vertical cast nên nhảy không mất focus. Quái có thể crossing ngắn khi recovery, nhưng không bắt đầu windup khi peer quá sát; không slot/token/formation. Đây là mô tả disposable probe của revision cũ, chưa nghiệm thu feel hoặc G-L production. Đất one-way và các vùng số thử ±12/±6, ±20/±10 không phải luật hiện hành; VS-1 giữ nguyên trong migration. Chi tiết/tọa độ lịch sử không chuyển sang docs 1–4.
