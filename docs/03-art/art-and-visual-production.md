# Huyền Lộ — Art & Visual Production

Các số BASELINE/TUNABLE chưa phải nghiệm thu runtime.

<a id="visual-perspective"></a>

**LOCKED — pure 2D orthographic side-view:** camera nhìn ngang trực giao, công trình đọc theo elevation; movement trái/phải/nhảy/rơi, không trục chiều sâu. Facing sprite player được thử riêng ở [two-facing Idle/Side poses](#player-facing); không đổi camera hoặc profile mob hiện hành. Không isometric, oblique ground plane hoặc camera tilt. Chiều sâu dùng rim/cap sáng, mặt đứng tối hơn, contact/edge shadow, silhouette, overlap foreground/midground/background và parallax. Mặt đứng được phải ngang, contiguous solid; mái/cành/núi nền có thể chéo nhưng không tạo mặt collision chéo.

**STRONG DIRECTION — bản sắc:** RPG võ hiệp huyền huyễn Á Đông, lấy cảm hứng cảnh quan và văn hóa Việt Nam, pha cổ phong và tiên hiệp nhẹ; không mô phỏng một triều đại cụ thể. Sơn môn, học viện/võ đường, mái ngói cong vừa phải, cầu gỗ/cổ đạo, cổng đá/bia/pháp ấn tạo vẻ cổ kính và có võ học. Giữ ba environment families và map/lore hiện hành; không đẩy toàn thế giới thành thiên giới, cung điện bay hoặc high-xianxia.

## Document owns

Pure side-view; Master Pose, visual-part authoring/composition, pose/raster reuse, sockets và art probe; import/layer, visual language/map Bible, UI, workflow và accounting có điều kiện.

## Document does not own

Luật ngoài domain thuộc owner trong [documentation map](../README.md); evidence thuộc [Playtest & Balance](../04-production/playtest-and-balance.md), thứ tự triển khai thuộc [Roadmap](../04-production/roadmap.md).

**Hướng UX đề xuất — PROPOSAL:** hợp nhất các view RPG trong một giao diện chung mở bằng một action menu; cấu trúc giao diện/phím I/C/Q còn OPEN. Inventory, Equipment, Attributes, Derived Stats/Thông số, Skills và Quest vẫn đủ chức năng. Menu action, Navigate/Confirm/Back tách khỏi gameplay, không cần key riêng cho từng view.

**View inventory/NPC:** NPC hiện hội thoại ngắn và marker `!` khi Available, `?` khi Ready; chọn chức năng rồi mở submenu riêng (mua, bán, gửi/lấy rương), không trải mọi item/action trên một menu NPC. Hành trang 60 ô dùng lưới icon + stack count, một bảng chi tiết cho ô đang chọn; Confirm mở thao tác của đúng instance. Trang bị nằm ở view Nhân vật với hình người và sáu slot quanh hình, tách khỏi bag grid.

Navigate/Tab navigation, mouse click, Confirm và Esc/back dùng cùng commands; không bắt click. Chi tiết bố cục ở [Art — map/UI blockout](#icons-ui).

HUD: HP / MP / EXP / level, skill CD, Food / Potion, quest, Boss timer. Trong PvP hiện cược/pot, đồng hồ 120 s và số lần dùng HP/MP Potion còn lại (ban đầu 3/3). Bảng skill hiện ba active tích lũy, selected/locked/CD/MP riêng và hai nội tại/class: icon, tooltip, level/điều kiện khóa, auto-open Lv 5/Lv 13; không thêm hotkey nội tại. Bag-full rõ; tooltip enhance trước / sau. MapExit arrow + tên vùng đích, NPC marker cơ bản P0; SpecialGate có cue riêng.

Quest navigation arrow xuyên map vẫn P1. Route automation không chứng minh inventory/shop/quest dễ dùng; cần manual usability review ở tốc độ thường, xem Art/Roadmap.



<a id="art-open-decisions"></a>

## Sổ quyết định Art và các dependency — alias A01–A17

Các quyết định visual dưới đây do Art sở hữu; quyết định gameplay/online/runtime đã chuyển về domain tương ứng theo [Open Decision Index](../README.md#open-decision-index). Giữ giữ options/trade-off lịch sử từ Art & Visual Production; recommendation/trạng thái đã reconcile theo lock mới. Axx là alias lịch sử, gắn vào gate sẵn có dưới đây; không tạo một hệ luật cạnh tranh với BAL/ART/PHY/TECH.

Trong bảng options cũ, `các mục liên quan` chỉ [evidence timing đã chuyển](../90-archive/design-history.md#art-combat-timing-evidence). P01–P15 là **ca thử** trong [Art & Visual Production](../04-production/playtest-and-balance.md#art-validation), không decision ID hay phase roadmap. Tất cả ca Unity vẫn **CHƯA CHẠY**.

| ID / vấn đề | Options và trade-off | Recommendation hiện tại | Cách chốt |
| --- | --- | --- | --- |
| A01 Mapping 26 logical frames | Prompt recovery xác nhận 26 logical frames; số raster mới/reuse còn cần manifest, không hai bộ locomotion đầy đủ | Giữ tám state/26 samples, pose map chung, upper theo class; mapping sprite/socket/hold là PROBE, không budget26 ảnh mới | P01/P03 kiểm mapping/reuse và công thật, không hỏi lại logical-frame lock |
| A02 Technique/weapon poses | Raster toàn bộ: sạch nhưng tốn; socket/skeletal thuần: ít ảnh nhưng rủi ro outline/khớp; hybrid: setup vừa và pose chính sạch | Hybrid pose-indexed parts + Back/Hand socket để probe; 13–25 hình hand weapon chưa gồm carry, count chỉ là kịch bản | P01–P03 và chi phí P15 |
| A04 Skill/VFX concept | Nét Mạch Ấn, kiếm khí/băng hoặc linh ảnh: khác sắc thái/cost; linh ảnh lớn dễ nhầm entity | Mạch Ấn/linh khí hoặc linh vật trang trí, sáu skill bindings giữ nguyên; concept cụ thể PROBE, không thêm entity/hitbox | P03/P04/P14; concept cụ thể chưa khóa |
| A08 Mob corpse / player shadow / flying loot | Fade nhanh: sạch; hold lâu: thấy kill nhưng clutter; rơi visual: tự nhiên; tan tại chỗ: rẻ; chiếu loot xuống nền: reachable nhưng cần luật server | Ground hold 0,5–1,5 s/fade 0,3–0,5 s; Ong rơi visual/fallback tan; loot server chọn điểm hợp lệ; player dùng shadow chung, timing OPEN | P01/P06/P09; corpse art và loot gameplay quyết riêng |
| A09 Terrain/building | Cell 16/32/64 px; tile-only hoặc mini kit/full asset; ít variant giảm cost nhưng lặp texture | Thử 32 px, 17 semantic + 4 cosmetic/họ; kit cho motif lặp, full asset cho landmark độc nhất | P08/P09/P15; blockout quyết số module thật |
| A12 Camera/UI/pixel scale | 480×270 hoặc 640×360 và policy màn hình khác; icon 32 px/lớn hơn; snap/rotation | Giữ PPU32, thử world view 15/20 u, UI font đủ dấu; chọn readability trước export | P01/P08/P11/P14; pin pipeline ở Technical spike |
| A16 Boss scope | 28 pose core hoặc thêm 4 roar; canvas 128/192 px; roar pose riêng hoặc overlay | Giữ behavior/telegraph, Cuồng không chen action; canvas theo room, roar optional | P14/P15; ngưỡng 30% không tự tạo action mới |
| A17 Accounting thực | Xuất PNG variants: nhiều ảnh; palette reuse: ít ảnh nhưng có setup; thêm shape: silhouette tốt và tăng cost | Thay estimate bằng manifest/giờ slice; không chốt tổng giờ khi chưa có evidence | P15 và review scope trước production hàng loạt |

| Alias lịch sử | Gate liên quan | Trạng thái / phạm vi được chấp nhận |
| --- | --- | --- |
| A01 | ART-01 | LOCKED 26 logical frames theo prompt recovery; OPEN exact raster/pose/socket mapping và số ảnh mới/reuse. Không tăng logical count. |
| A02 | ART-01 | OPEN: hybrid và số góc/pose là phương án thử, chưa specification kỹ thuật cuối. Minimal Kiếm/Cung probe CURRENT; full Cung production DEFERRED. |
| A04 | ART-01 / CC-01 | APPROVED nguyên tắc phân biệt cast/main VFX/impact/status; OPEN concept cụ thể, số frame và motif. Minimal Kiếm/Cung CURRENT; Boss/full catalog thử ở phase sau. |
| A08 | ART-01 / PHY-01 / LOOT-01 | APPROVED corpse tách gameplay terminal/respawn. Player shadow là STRONG DIRECTION riêng; OPEN timing/hình mẫu, mob hold/fade/flying và điểm loot server chọn. |
| A09 | ART-01 / PHY-01 | APPROVED cue solid/one-way/prop và surface đọc được; OPEN tile cell/module count/cách dựng kit. Room CURRENT, full families DEFERRED. |
| A12 | ART-01 / PHY-01 | OPEN camera/reference resolution/UI scale, pixel snapping và icon size; PPU32 giữ nguyên. |
| A16 | BOSS-02 / CC-01 / ART-01 | DEFERRED art prototype Boss; OPEN canvas/28 pose/roar. Behavior, telegraph và Cuồng đã có baseline design owner, không bị bỏ. |
| A17 | ART-01 / SCOPE-01 / TECH-01 | OPEN tổng count/giờ thực; CURRENT đo Kiếm/room/kit và % art dùng được. Kịch bản player mới chỉ PROPOSAL, thay estimate sau evidence. |

DESIGN LOCK đã duyệt progression/controls/authority trong mục dưới; Art technique/count, camera scale/reference resolution và exact response chưa được promote thành production lock; góc camera side-view đã LOCKED. **REJECTED cho CURRENT**: production hàng loạt trước gate, dùng AnimationEvent/VFX để sinh damage, dùng mô phỏng làm acceptance, hoặc gọi Cung là P1. Các lựa chọn thẩm mỹ chưa có prototype tiếp tục OPEN; việc chưa làm là DEFERRED, không phải loại bỏ khỏi TARGET.


<a id="art-review"></a>

<a id="0-review-và-phạm-vi-được-chấp-nhận"></a>

## Review và phạm vi được chấp nhận

**APPROVED** là nguyên tắc đủ cơ sở để dùng trong specification, không đồng nghĩa mọi con số trong nhóm đã khóa. **BASELINE** là luật/số design owner hiện dùng; **CURRENT PRIORITY** gồm minimal Kiếm/Cung Pha R và route Q1–Q6/Kiếm trước production rộng; **DEFERRED** là phần triển khai/thử art làm sau nhưng vẫn TARGET P0; **OPEN** cần quyết định/evidence. Quyết định visual ở [Art decisions](#art-open-decisions), các domain khác theo [Open Decision Index](../README.md#open-decision-index); phần này phân loại phạm vi Art, không sổ quyết định thứ hai.

| Nhóm được review | Kết luận sử dụng hiện tại | Phần chưa duyệt / lúc validate |
| --- | --- | --- |
| Modular player/Master Pose–HeadHair–Upper–Lower–Weapon | APPROVED semantic composition và đồng bộ pose/state/phase; CURRENT minimal Kiếm/Cung | A01/A02 exact raster/socket/hybrid và reuse class OPEN |
| Logical frame / unique sprite | APPROVED phân biệt đơn vị đếm và khai báo giả định | 26 logical samples đã xác nhận; manifest raster/3/4/side/shadow và asset accounting còn PROBE |
| Default outfit/Mộc và visual progression | APPROVED default khi unequip, đúng bảy weapon visuals theo catalog, band đọc bằng silhouette/accent | Số góc/frame/cách dựng cây Kiếm/Cung OPEN; fixtures không cấp skill sai class |
| LowerBody/Boots/phụ kiện | Giữ sáu ô; Boots/Ring/Necklace chỉ đổi chỉ số/icon, footwear hình ảnh thuộc LowerBody | Đề xuất bỏ Boots cũ SUPERSEDED; thêm layer footwear chưa được duyệt |
| Normal/skill/main VFX/impact/reaction/status | APPROVED phân trách nhiệm và đọc result đúng; CURRENT Novice/Kiếm S1 | Motif/frame exact A04/A06 OPEN; Lv 10/17 dùng fixture hẹp rồi route sau |
| Multi-target/weapon snapshot | DIRECTION giữ primary + propagation theo design owner; VFX không chọn victim, đúng cây lúc cast | Minimal Cung probe CURRENT; full production Cung DEFERRED; logical batch đã duyệt; visual travel/AAA balance A05 cần đo |
| Mob animation / hit | APPROVED feedback không tự stun, action/death/CC ưu tiên đúng | CURRENT Nấm/Sói; 105 pose là kịch bản lịch sử có điều kiện ba Hybrid, capability/count hiện OPEN; rig khác DEFERRED |
| Mob death/corpse | APPROVED terminal gameplay tách corpse visual, timer không theo clip | Hold/fade/flight/loot ground points và terminal event A08/A15 OPEN |
| Training Dummy | BASELINE HP 60/25 s; DIRECTION Q3 bốn kills, năm placements (ba chính + hai phụ); prototype solo là lịch sử | DEF/EVA/timer alternatives và online contention A07 OPEN; không giảm timer chỉ để giải chờ solo |
| Terrain/readability | LOCKED đất/đá solid trực giao, one-way kết cấu hiếm và có đỡ; không slope/climb | Bốn grammar ở các mục liên quan; cell 32/module 17+4/camera A09/A12 OPEN |
| Building/structural/maps↔mob | Mặt đứng, tuyến đi, spawn/leash và loot tới được phải kiểm cùng art; kit tái dùng | Exact kit/LoS A/B/Hybrid/Return còn OPEN; mật độ 28/66 cũ SUPERSEDED |
| Animated environment | BASELINE flow/ripple cosmetic; shallow slowdown theo design owner, không swimming/hazard | Puddle nhỏ nếu có trong slice; thác/Bạch/full environment DEFERRED, strip counts OPEN |
| NPC | Bảy NPC hiện hành, khu chức năng riêng và dấu nhận/trả quest đúng design owner | 14–20 hình idle/gesture là kịch bản thử; tọa độ OPEN, 16–22 cũ lịch sử |
| VFX ngoài skill | APPROVED Freeze khác Slow, một status/target, ưu tiên telegraph | Burn/Freeze/Boss load phần sau; texture/frame count/concept OPEN |
| Items/icons | APPROVED binding tách motif/rarity/+n overlays, UI đọc được band | CURRENT items Q1–Q6; 49/64 bindings là số suy ra có giả định, 63 bitmap/size OPEN |
| Common UI Kit | APPROVED compose panel/button/slot, text Việt và pending/error đúng result | CURRENT HUD/NPC/bag/shop/quest/class; 21 functions không khóa 21 textures |
| Local/online presentation | APPROVED visual đọc session authority, không AnimationEvent damage | Harvest/feel/art/UI probe → production base → G-L mới → G-N sớm; pose/âm anticipation A11 OPEN; tentative gameplay projectile/rollback DROP P0 |
| Login/Character Select preview | TARGET P0 giữ đủ flow; DEFERRED khỏi local slice | A13 default/exact gear data dependency OPEN |
| Technical art pipeline | BASELINE canvas 64/PPU32/pivot; APPROVED pose/physics tách | Exact atlas/padding/flip mechanism/package/camera cần Unity test, A01/A02/A12 |
| Accounting/production cost | APPROVED tách pose/variant/export/editor/QA/rework, đo % dùng được | CURRENT sample player/3/4-side/carry/shadow; exact totals/hours A17 OPEN |
| Prototype matrix | Giữ đầy đủ P01–P15, CHƯA CHẠY; CURRENT minimal Kiếm/Cung trong sandbox riêng, route Kiếm Q1–Q6 | Full Bow/Boss/online load DEFERRED, quay lại trước production branch tương ứng |

Luật input/terrain/NPC/gear đọc theo design owner; technique/count vẫn chưa duyệt. REJECTED cho production hiện tại: vẽ hàng loạt trước G-N, lấy animation/VFX làm damage authority hoặc dùng số giả định làm nghiệm thu. Các phân tích Cung/Boss còn đủ dưới đây; DEFERRED không đổi chúng thành P1.

Reasoning từ các phát hiện ban đầu được giữ tại [Phụ lục A](../90-archive/art-history.md#art-initial-findings).

Ưu tiên công cho dáng trang bị, tư thế Kiếm/Cung, thời điểm phát/trúng đòn, telegraph và trạng thái. S2 là ứng viên farm dùng thường xuyên, phải được đầu tư pose/impact dễ đọc cùng S1 và S3. Giảm hạt thừa, idle phụ và cảnh tổng kết cầu kỳ trước khi giảm thông tin này. Không thêm class, combat slot, CC, companion, swimming, hệ ánh sáng hay buff chủ động P0.

<a id="player-visual"></a>

<a id="1-kiến-trúc-hình-ảnh-player"></a>

## Kiến trúc hình ảnh player

**Đọc từ:** owner design tương ứng, owner kỹ thuật tương ứng. Giữ canvas 64×64 (khung ảnh nguồn), PPU 32 (32 pixel trên một world unit) và một cơ thể nam; không tạo rig đầy đủ cho từng bộ đồ.

<a id="master-pose"></a>

### Master Pose Template / Master Pose Schema

**Semantic composition đã được duyệt:** Master Pose là **authoring reference + runtime pose data contract**. Template dùng để artist/AI thống nhất tư thế, tỷ lệ và điểm ghép; schema mô tả pose để runtime chọn/căn part. Không xuất template thành một naked-body render layer, không vẽ nhân vật trần hoàn chỉnh rồi chồng áo/quần tĩnh. Phần da lộ được author trong fragment tương ứng.

| Ngữ nghĩa Master Pose | Quy ước dùng chung |
| --- | --- |
| Pose identity / phase | Logical pose ID gắn state/profile và timing; các part lấy cùng mẫu phase, không tự chạy clip riêng |
| Root/origin và feet/ground | Chung gốc character, mốc chân, scale/PPU và tỷ lệ; pose offset không dời physics root |
| Head anchor | Điểm ghép đầu/cổ, cùng góc nhìn và quy ước offset |
| Upper-body anchor | Vị trí thân trên/vai, căn torso và tay theo pose class/action |
| Hip/lower-body anchor | Điểm ghép eo/hông và chân; tiếp xúc upper/lower được kiểm ở locomotion và action |
| Hand/grip và back sockets | Điểm nắm, rút/cất; vũ khí bám current pose + weapon state |
| Optional VFX/socket anchors | Tip/Muzzle/nock hoặc điểm FX khi presentation cần; không là gameplay origin/hitbox |
| Per-part local transform | Offset từng part; rotation/flip/correction metadata khi cần, cùng convention và revision |

Exact field/component names và data-asset schema còn là lựa chọn implementation. Tỷ lệ cụ thể được review trên sample; sau khi chọn template, mọi artist/AI và visual set phải dùng chung tỷ lệ/root/anchor convention đó, không tự đổi pivot hay proportion cho từng món.

Unity 2D có thể tổ chức một GameObject visual root và child SpriteRenderer, với SortingGroup khi phù hợp. Cấu trúc dưới đây chỉ minh họa trách nhiệm, không khóa tên GameObject, số renderer hoặc hierarchy:

```text
Player VisualRoot
├── Shadow
├── LowerBodyVisual
├── UpperBodyVisual
├── HeadHairVisual
├── WeaponVisual
└── VFX / presentation layers

Master Pose → Head/Hair + UpperBody/Armor + LowerBody/Pants + Weapon
```

### Visual parts và thay trang bị

| Part | Sở hữu hình gì | Chọn/căn theo current logical pose |
| --- | --- | --- |
| Head/Hair | Đầu, mặt và tóc; visual module, không Helmet equipment slot | Reuse khi góc phù hợp; offset/correction theo head anchor, không ép một hình đầu vào mọi góc |
| UpperBody/Armor | Silhouette thân trên hoàn chỉnh: torso, tay áo, arms/hands, da cổ/tay lộ; belt/phụ kiện thuộc thiết kế đó khi cần | Equip Armor thay UpperBody visual set; từng fragment theo Master Pose, không một PNG áo tĩnh đặt lên cơ thể khác pose |
| LowerBody/Pants | Quần, legs, da lộ, xà cạp và footwear silhouette theo art direction | Equip Pants thay LowerBody visual set; root/hip/foot contact thống nhất, không chồng icon quần lên chân set khác |
| Weapon | Vũ khí đang dùng, grip/string/nock và Back/Hand presentation | Chọn visual ID và trạng thái/socket theo action/pose; đổi cây không tạo upper/lower set mới |
| Shadow / presentation layers | Ground shadow hoặc death shadow/eyes, flash, trail và status/FX theo vai trò | Tách khỏi equipment; đọc phase/result, không damage/hitbox hoặc gear slot mới |

**Default outfit bắt buộc:** DefaultUpperVisual và DefaultLowerVisual là áo/quần võ sinh hoàn chỉnh theo cùng Master Pose khi Armor/Pants slot trống. Đây là fallback visual, không item/stat mới, không Band I và không cơ thể trần. Upper/lower thay độc lập: equip một món vẫn ghép với default của phần còn lại. Q3 Quần I, Q4 Áo I phải khác biệt nhìn thấy. Boots/Ring/Necklace tiếp tục chỉ đổi stat/icon; footwear world thuộc LowerBody. Không equip weapon thì hai tay trung tính; điều kiện attack vẫn thuộc gameplay hiện hành.

**Pose-indexed parts:** ở một sample như `Run_02`, Head/Hair, Upper, Lower và weapon state/socket cùng lấy root, scale, proportions, anchors, pose identity và frame timing từ Master Pose. SpriteRef có thể reuse, còn local offset/order/correction đi theo pose; không đồng nghĩa mỗi item cần 26 unique rasters. Một mapping đã kiểm có thể ghép Upper A + Lower B + Weapon C + Head/Hair D. Không author full-character sprite theo `Class × Armor × Pants × Weapon × Frame`.

Inventory/UI representation dùng trong Inventory, Shop, Equipment panel và Loot UI; character visual representation dùng ghép actor trong world. ItemDefinition có thể tham chiếu hai asset roles riêng; icon có thể được dựng/crop từ art rồi clean cho UI, nhưng không dùng trực tiếp làm fragment trên actor.

<a id="player-facing"></a>

### Two logical facings, Idle 3/4 nhẹ — direction đã xác nhận

Player chỉ có **Left/Right**. Idle là 3/4 nhẹ theo facing hiện tại để thấy mặt/thân áo/gear; Run nghiêng side hơn, Jump/Fall có silhouette riêng. Attack/Skill gần full side với upper pose Kiếm/Cung đúng swing/draw. **Front Idle bắt buộc và proposal front→side cũ SUPERSEDED.** Không đổi camera, thêm depth movement/bốn hướng hoặc xoay toàn actor 90°.

Chạy Right rồi dừng → Idle Right; Left → Idle Left. Focus-only không ép quay theo target/thêm combat mode. Action giữ facing/origin snapshot, input mới không sửa giữa release; recovery trở về facing hợp lệ. Execute S2 liên tiếp không chèn Idle. Back/Hand cosmetic, settle/hold duration TUNABLE, không reset về front/random. Default Right deterministic khi spawn/reconnect là BASELINE nếu chưa saved-facing requirement; không thêm DB field.

| Mapping probe | Vì sao cần | Điều còn OPEN |
| --- | --- | --- |
| Idle 4: giả thuyết timeline một facing và mirror | Nhịp thở 3/4, giữ logical count | Bốn samples dùng 1–2 raster hay 4; asymmetry/corrections Left |
| Run 6 side hơn Idle | Tay/vai/chân/trọng lượng đổi thật | Pose transition/sockets, không mặc định thêm turning strip |
| Jump 2/Fall 2 distinct | Rising/apex/falling đọc khác nhau | Raster reuse/hold/landing, không tăng counts |
| Attack 3/Skill 4 Kiếm/Cung | Thấy swing/draw, không chỉ xoay weapon trên Idle torso | Shared 26 logical indices; exact upper raster/socket reuse OPEN |

Tách movement direction khỏi action facing; lower locomotion ghép đúng hip/neck/shoulder với upper kể cả chạy ngược hướng action, không đổi momentum/air policy để cứu hình. **PROBE:** author Right rồi mirror VisualRoot/socket đúng một lần; tóc/vạt áo/chuôi/linh văn bất đối xứng chỉ correction Left khi sample cần. Physics root/text không mirror. UpperBody/Armor theo Master Pose và class profile; Lower reuse khi stance/foot contact hợp, không nhân theo weapon variant/outfit combinations. [Accounting](#player-s0) là ví dụ công, không lock raster mới.

NPC facing **authored độc lập** front-ish/3/4 Left/3/4 Right/side theo composition: shopkeeper hướng ra player space, mentor về yard. Không last-movement rule hoặc auto-face khi Talk; không hệ NPC aim/turning mới.

<a id="weapon-carry"></a>

### Back / Hand — STRONG DIRECTION, cơ chế OPEN/PROBE

Chỉ Sword/Bow, Mộc Kiếm dùng Sword track. **PROPOSAL tối thiểu:** hai anchor Back và Hand/Grip; Tip/Muzzle/nock hiện có derive từ pose track khi cần, chưa khóa rig/socket phức tạp. Ngoài action ưu tiên Back; action bắt đầu chuyển Hand đúng phase, không chờ draw clip để PrimaryAction (combat branch). Hold ngắn sau recovery rồi cất là cosmetic timer, không đọc MP/CD để rút/cất, không nút/skill mới và không đổi CombatFocus/PendingCast. Khi chạy/nhảy/rơi: Back nếu đã cất, Hand nếu action/hold còn hiệu lực; gear trống không hiện cây giả.

Một cây equip có **một representation nhìn thấy tại mỗi phase**: đổi Back ↔ Hand và render order cùng lúc, không hai bản sao/ghost weapon. Probe ưu tiên carry silhouette gộp bao/chuôi với Sword, ẩn representation đó khi Hand hiện. Bao kiếm rỗng riêng có thể đẹp hơn nhưng thêm prop, sorting và tối đa một hình/visual Sword; optional, chưa cộng mặc định. Back có thể reuse canonical bằng offset/góc nếu đọc tốt, nếu không thêm đúng hình carry cần sửa.

Idle 3/4 theo facing cần chuôi/lưỡi hoặc cánh cung lộ trên vai/bên thân, không bị áo che hết; không mở rộng collider. Cung đeo chéo có thể vượt cell 64 × 64 qua weapon renderer riêng; kiểm silhouette/crop/camera bounds ở scale thật, không scale body để nhét. Bow draw/release đổi rest/bent/recoil shape, dây/nock/arrow và hai tay cùng pose phase; tên nocked có thể reuse projectile art, không là vũ khí equip thứ hai. Cùng family dùng chuẩn grip chung, từng visual vẫn phải kiểm chiều dài/guard/cánh cung và occlusion; đổi item không phát sinh áo/quần mới.

<a id="11-contract-26-frame-thực-sự-đếm-gì"></a>

### Contract 26 frame thực sự đếm gì

Tám state và phép cộng `4+6+2+2+3+4+2+3=26` giữ nguyên. **LOCKED 26 logical frames theo prompt recovery hiện tại:** các ô lấy mẫu timeline có thể reuse sprite/hold hoặc chọn upper pose class phù hợp; không đồng nghĩa26 raster mới. A01 còn OPEN về exact slot→pose→sprite/socket mapping và asset accounting, không về logical count. Timeline bám clock gameplay, không buộc mọi pose dài bằng nhau.

| State / ô hiện hành | Lý do đủ cho baseline | Ít hơn mất gì / nhiều hơn được gì |
| --- | --- | --- |
| Idle 4 / 6 FPS | Nhịp thở có thể chỉ cần 1–2 hình; Idle 3/4 Left/Right là direction; raster/timeline mapping đang PROBE, không tự nhân đôi bốn ô | Hai hình vẫn dùng được nhưng thở dễ giật; thêm hình ít lợi ích ở camera game |
| Run 6 / 10 FPS | Hai chân × contact/passing/lift = sáu pose có khả năng đọc cadence | Bốn hình bớt chuyển trọng lượng; tám hình mượt hơn nhưng tăng chân/áo QA. Đo trượt chân theo MoveSpeed, không đổi tốc gameplay để khớp sprite |
| Jump 2 / 8 FPS | Rời đất và tư thế đi lên; hold hình thứ hai khi còn đi lên | Một hình mất dấu takeoff; thêm landing không được tự thêm action lock mới |
| Fall 2 / 8 FPS | Chuyển từ apex sang tư thế rơi; hold, không loop rung chân vô hạn | Một hình có thể đủ nhưng mất chuyển apex; landing riêng chỉ thêm nếu tiếp đất khó đọc |
| Attack 3 / 12 FPS | Kiếm: chuẩn bị → quét/hit → trả thế; Cung: kéo → release → trả thế | Hai hình làm hit/release khó đọc; bốn–sáu hình cho draw dài/arc đẹp hơn, cần kiểm timing trước tăng cost |
| Skill 4 / 12 FPS | Chuẩn bị → tụ lực → phát → trả thế; reuse cho nhập môn/tiến cảnh/đại chiêu bằng hold/VFX | Ba hình mất nhịp tụ; hơn bốn chỉ đáng làm nếu Lv 17 vẫn không đọc signature sau đổi VFX |
| Hit 2 / 10 FPS | Recoil → hồi pose khi rảnh; còn flash/impact cho lúc đang action | Một hình vẫn đủ overlay; hơn hai dễ tạo cảm giác bị khóa lâu. Không mặc định Hit interrupt |
| Death 3 / 8 FPS | Player: pop → fall → shadow chung; có thể reuse Hit/Jump/Fall, không outfit corpse | Ba ô giữ nguyên; FPS là preview baseline, không khóa pop/fall/hold duration. Blink là cosmetic, không thêm state gameplay |

Phép suy ra **33** cũ (19 common +7 Kiếm +7 Cung) giả định side-only idle và death mặc đủ đồ, nay chỉ là tham chiếu kịch bản cũ; không dùng làm budget hướng 3/4/carry/shadow. Giữ nguyên 26 và tám state; IdleLeft/IdleRight dùng cùng Idle state, Land reuse Jump/Run nếu hợp, shadow/blink là visual chung. Manifest phải chỉ rõ slot→pose→spriteRef, ảnh mới/reuse và variant. Prompt recovery đã xác nhận logical frames; probe vẫn phải chứng minh manifest/reuse thực, không tuyên bố26 ảnh raster đủ mọi outfit/class/facing.

<a id="12-ba-cách-làm-và-recommendation"></a>

### Ba cách làm và recommendation

Ba phương án cần đối chiếu: vẽ ảnh riêng cho từng pose (`raster`), ghép các part trên xương/điểm gắn rồi xoay (`skeletal/socket`) và kết hợp ảnh sửa theo góc với điểm gắn (`hybrid`). Hybrid đang là phương án thử A01/A02, chưa phải kỹ thuật đã duyệt. Bảng lợi/hại và hướng sửa lỗi điểm gắn/tư thế đứng (`stance`) được giữ tại [Phụ lục A — technique](#art-technique-rationale).

**A14 — ra đòn khi chạy/nhảy/rơi:** giữ gravity và đà ngang; CURRENT thử Tân Lữ/S1, quyền dùng S2/S3 trên không còn OPEN. Ghép phần thân trên ra đòn với chân chạy/nhảy phải cùng phase/root/socket; không khóa movement hoặc thêm animation set để cứu 26/33. Flash khi bị đánh không bắt đầu lại action; pose đọc SkillId đã chụp của action được nhận, không selection mới.

<a id="13-pivot-flip-và-overlap"></a>

### Pivot, flip và overlap

Canvas căn mốc chân `(32,0)` theo pivot hiện hành. Có thể chừa 1–2 px trong suốt và offset chân chung, không crop tự động làm đổi điểm chân. Điểm đầu/tay/nắm (`grip`) được author bằng tọa độ pixel **theo pose**, không theo khung bao ảnh. Grip lệch 1 px dễ thấy trên cây cung mảnh.

Cần giữ các part cùng actor được sort nhất quán để hai player không chen mảnh; SortingGroup là hướng Unity phù hợp để probe, không khóa số component/hierarchy. Unity xác nhận SortingGroup phù hợp nhân vật gồm nhiều SpriteRenderer chồng lấp. [Nguồn Unity — SortingGroup](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Rendering.SortingGroup.html).

Sorting theo quan hệ che khuất của current pose: weapon Back sau thân, weapon Hand và tay/đầu/tóc/vạt áo trước hoặc sau đúng silhouette. Exact SortingOrder numbers và renderer count còn OPEN; không có Body layer nằm dưới Armor. Tay cầm cung phải có phần trước cây cung; tóc/vai/vạt áo cần mask/split đúng pose. Split trước/sau là render slice của một module, không một món gear mới. Ưu tiên cutout đã author; chỉ thêm HandFront nếu kiểm grip chứng minh cần, tính thêm việc slice/QA.

**Flip có bẫy kỹ thuật:** `SpriteRenderer.flipX` chỉ đổi render, không tự mirror child socket. Preferred production direction / ART PROBE: author một canonical facing khi phù hợp, mirror VisualRoot chứa parts/anchors/sockets cùng actor đúng một lần, giữ physics root và text/worldUI ngoài nó. Asymmetry của tóc, áo, bao/phụ kiện hoặc weapon có correction sprite/pose khi cần. Exact mirror/correction policy TUNABLE; phương án flipX riêng chỉ dùng nếu adapter chứng minh socket/rotation đồng bộ, không mirror lần hai. Gameplay origin do server/data riêng. [Nguồn Unity — flipX](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/SpriteRenderer-flipX.html).

<a id="modular-compatibility-probes"></a>

### Compatibility và occlusion — ENGINEERING RECOMMENDATIONS / ART PROBE

- Author neck/waist/hip seam ownership rõ: Upper sở hữu da cổ/tay, Lower sở hữu legs/da lộ; tránh thiếu da, hai cặp tay hoặc đường viền kép khi mix. Test wide sleeves/hem/hair với Sword grip và Bow nock; foreground hand slice nếu cần thuộc Upper visual set, không hand/body base hoặc equipment slot mới.
- Action upper + locomotion lower cần một resolved composition/sample chung cho head/hip/hand/feet. Probe chạy ngược action facing, Run→Jump/Fall khi action đang chạy và chuyển 3/4→side; không thêm full sprites theo mọi cặp pose hoặc khóa movement/air policy để cứu seam. Metadata/correction cần thật được kê sau probe, không tăng tám state/26 logical samples.
- Cùng pixel-coordinate convention, PPU, pivot và schema revision qua source→export→slice→atlas. Trim/crop, top-left↔bottom-left Y và parent scale/rotation không được đổi anchor âm thầm. Kiểm alpha edge/padding ở neckline/waist, camera snapping và interpolation; local rotation/mirror phải được áp đúng một lần cho socket và grip offset.
- Sort actor parts thành cùng nhóm khi phù hợp; thử occlusion hand/weapon/head/hem theo pose ở hai actor chồng nhau. Render slices trước/sau có thể cần nhiều renderer cho một semantic module; exact counts/orders OPEN. Ground shadow, death shadow và telegraph có vai trò/lifetime khác nhau; group membership được probe để không che telegraph/worldUI.
- Test equipment swap giữ phase, cả callback/load sprite refs đến trễ và unequip một phần; required pose coverage có diagnostics. Swap/sorting/carry và part refs phải nhất quán trong cùng presentation sample, không một frame Upper cũ/anchors mới. Missing art không đổi item/class/stats hoặc combat result. Runtime giữ revision/lifecycle contract tại [art integration](../02-technical/gameplay-runtime.md#art-contract).

<a id="weapon-visual"></a>

<a id="2-tiến-trình-hình-ảnh-vũ-khí"></a>

## Tiến trình hình ảnh vũ khí

**Đọc từ:** owner design tương ứng. Có **bảy weapon visual**: Mộc Kiếm + ba Kiếm + ba Cung. Rarity/enhancement không nhân số bộ animation.

**ART PROBE / OPEN:** một canonical Sword sprite với socket transform, các góc raster sửa, hoặc các Bow rest/draw/recoil variants là phương án chất lượng để so. Các dải số trong bảng/kịch bản dưới chỉ minh họa phương án, không khóa Sword1/Sword4/Bow3, góc xoay hoặc số ảnh carry; raster budget chỉ chốt sau probe.

| Visual | Khác biệt nên thấy ở camera chơi | Phần reuse |
| --- | --- | --- |
| Mộc Kiếm | Lưỡi gỗ thô, chuôi buộc đơn giản, không sáng linh khí | Cùng grip/đường swing Kiếm, khác ảnh cây kiếm |
| Thanh Mộc Kiếm / Cung | Đường nét gọn, vật liệu gỗ/vải, lục nhạt | Motif Band I, pose class chung |
| Vân Nham Kiếm / Cung | Guard/limb chắc, accent đá/đồng, silhouette khác I | Cùng socket, pose, effect class |
| Huyền Ấn Kiếm / Cung | Dấu cổ văn, hình lưỡi/đầu cung có đặc trưng; sáng tiết chế | Cùng hit timing, pool VFX; không làm dài hitbox vì ảnh lớn |

Chỉ đổi palette giảm công nhưng progression dễ khó thấy. Khuyến nghị mỗi band đổi **ít nhất một dấu dáng bao** ở chuôi/lưỡi kiếm hoặc đầu cánh cung và một accent. Common/Rare/Epic cùng template giữ cùng hình; phẩm chất đọc qua UI. Các món cùng band không đồng nghĩa cùng stat/rarity/item: `ItemDefinition → visualId`, instance giữ chỉ số riêng.

| Vũ khí / phương án | Hình cần author cho mỗi variant | Vì sao / trade-off |
| --- | --- | --- |
| Kiếm socket cứng | 1 ảnh canonical có grip + transform track theo mọi pose | Nhẹ nhất; góc nghiêng có thể stair-step, không diễn foreshortening. Trail tách ảnh kiếm |
| **Kiếm hybrid đề xuất** | 1 canonical + tối đa 3 góc sửa cho anticipation/active/recovery = **1–4 hình/visual** | Ba sửa là nhu cầu góc của Attack 3 ô, không phải 26. Skill 4 ô reuse góc gần nhất; chỉ thêm nếu hand/foreshortening không đạt |
| Kiếm raster toàn bộ | Hình theo tập góc thực sự khác của Attack/Skill/locomotion | Tốt outline, đắt variant; không tự yêu cầu 26 hình nếu nhiều ô cùng góc |
| **Cung hybrid đề xuất** | Rest/nocked shape, bent draw, released recoil = **3 hình/visual** | Hai hình rest/draw bỏ recoil nhưng vẫn test được; thêm partial draw thứ tư chỉ nếu nhịp kéo không đọc. String phải theo draw, không dùng ảnh cung đứng yên |
| Mũi tên | 1 silhouette chung; tint/tip/trail theo Novice/skill | Không bảy projectile theo gear. Tên gắn trên dây có thể dùng cùng ảnh; draw hand/arrow offset là track chung |

Kịch bản hybrid **hand weapon, chưa gồm carry/bao riêng**: 4 Kiếm×4 + 3 Cung×3 = **25 ô hình vũ khí**, trong đó 7 canonical/shape nền và 18 góc/deformation thêm; 1 tên chung tính riêng. Nếu cả bốn Kiếm dùng xoay canonical tốt, chỉ cần 4+9=13; đây là khoảng **13–25** có cơ sở, không asset budget đã khóa. Nhìn đúng cây đang equip cần kiểm **mỗi visual trên Attack và các Skill hợp lệ với nó**, không chỉ Idle; không cấp skill class cho Tân Lữ chỉ để thử Mộc Kiếm.

Action giữ visualId vũ khí, profile/pose/timeline đã chụp lúc bắt đầu; đổi equip giữa action không làm cây vũ khí đang đánh đổi hình. Đề xuất áp hình gear mới ở biên pose/action hợp lệ, damage action cũ giữ snapshot. Có chặn equip khi cast hay không vẫn **OPEN**. Remote cần visual snapshot/revision để diễn đúng cây, không đoán từ damage hay gear mới nhất.

Skill VFX là sức mạnh phái, không baked vào ảnh cây kiếm/cung. Anchor ở grip/tip do pose track; nhập môn/tiến cảnh/đại chiêu đổi preset theo skill profile, dùng được với mọi visual hợp lệ. Vũ khí mạnh hơn vẫn đọc bằng silhouette khi effect sáng lên.

<a id="3-trang-bị-lowerbody-và-boots"></a>

## Trang bị, LowerBody và Boots

**Đọc từ:** owner design tương ứng. World visual, inventory icon và gameplay item là ba lớp riêng: một áo Rare +4 vẫn dùng visual band của template; icon thêm rarity/+4 ở UI; stat evaluator xử lý sức mạnh.

| Loại | World | Icon / gameplay | Recommendation |
| --- | --- | --- | --- |
| Armor I/II/III | Ba UpperBody visual sets hoàn chỉnh theo Master Pose, gồm tay/da lộ; redraw khi silhouette pose cần | Ba template icon, rarity và enhance ngoài ảnh | Vải → viền/miếng giáp → cổ văn; tránh áo dài che chân và bow grip |
| LowerBody I/II/III | Ba LowerBody visual sets hoàn chỉnh theo Master Pose, **kèm legs/footwear mỹ thuật** | Ba item Quần, không thêm slot footwear | Đổi viền/gối/cạp và footwear theo band; chân chạy/nhảy phải theo pose |
| Ring / Necklace | Không world sprite | Sáu template icons, stat và tooltip | Giữ stat-only; vẽ trên body 44–48 px khó đọc, ít lợi ích so cost |
| Boots | Không sprite trong world; footwear do LowerBody trình bày | Ba icon; HP/DEF/EVA/tốc chạy và enhance/Tinh Hoa đọc [Items & Economy](../01-design/items-and-economy.md#gear-economy) | **Giữ ô chỉ số P0**, tooltip không hứa hình footwear đổi theo item Boots |

Armor có thể giữ cùng silhouette gốc nhưng cần accent band đủ nhìn; LowerBody phải khác ở vùng không bị áo che. Đổi outfit không thay collider, shadow footprint hay range. Không sản xuất full sprites theo tổ hợp outfit/class/weapon, không sản xuất hình cho mỗi mức +.

<a id="31-giữ-boots-và-tra-đề-xuất-cũ"></a>

### Giữ Boots và tra đề xuất cũ

Hiện hành giữ sáu ô; Boots chỉ đổi chỉ số/icon, footwear hình ảnh thuộc LowerBody. Armor/Pants/Boots là nhóm HP; Weapon/Ring/Necklace là nhóm MP theo design owner, không suy chỉ số từ vị trí slot trên UI. Đề xuất bỏ Boots đã SUPERSEDED; [Phụ lục A — Boots](../90-archive/art-history.md#art-boots-rationale) giữ phép tính/trade-off lịch sử để tra, không mở lại số ô. Thêm layer footwear chưa được duyệt và không thuộc công sản xuất hiện hành.

<a id="combat-visual"></a>

<a id="4-ngôn-ngữ-hình-ảnh-combat--core"></a>

## Ngôn ngữ hình ảnh combat — CORE

**Đọc từ:** owner design tương ứng. Người chơi cần biết ai ra đòn, cây gì, phạm vi nào, lúc nào trúng, có trạng thái gì. VFX đẹp mà sai thông tin này là lỗi core.

| Lớp presentation | Sở hữu | Điều không được suy ra từ lớp này |
| --- | --- | --- |
| Actor/cast animation | Anticipation, tụ lực, release, recovery; tư thế class | Không tự sinh hit/damage bằng AnimationEvent |
| Weapon visual | Item đang dùng, grip, draw/swing | Silhouette dài không tăng range |
| Novice basic presentation | Mộc Kiếm swing onboarding trước class | Không class Normal thứ tư; Attack pose có thể reuse cho S1 |
| Main skill VFX | Motif/phạm vi/nhịp/mốc progression của cast | Không quyết định tập target |
| Projectile/wave/slash/linh ảnh/object | Hình chuyển động có nhiệm vụ đọc executor | Linh ảnh trang trí không là pet/entity đánh thêm |
| Per-target impact | Điểm landed hit, gọn và đúng timestamp | Không biểu diễn mục tiêu chỉ vì nằm dưới sprite effect |
| Target reaction | Flash/recoil/CC/death theo state | Damage không tự stun/knockback |
| Damage/Crit/NÉ | Kết quả server và nguồn đọc ưu tiên local | Không số damage dự đoán |
| Status | Bỏng/Đóng Băng/Làm Chậm theo lifetime thực | Màu lạnh lúc cast không chứng minh đã proc Freeze |
| Beneficial | Heal/MP/Food/miễn thương hồi sinh được xác nhận | Hạt hồi máu không thêm regen ngoài luật |

<a id="41-framework-lv-1--lv-20"></a>

### Framework Lv 1 → Lv 20

**PROPOSAL concept:** linh mạch/Mạch Ấn làm liên kết; có thể thử Kiếm nét ấm, Cung linh khí lạnh hoặc motif khác hợp skill. Long/Giao Long, Lân/Kỳ Lân, Phượng, Hổ/Bạch Hổ, sư tử/linh thú Á Đông có thể làm họa tiết kiến trúc/giáp/vũ khí, aura/telegraph hoặc linh ảnh VFX. Hình cụ thể chưa khóa; giữ silhouette đòn, số target/logical eligibility/timing, không thêm pet/summon/mount/mob/Boss/class/damage type. Nguồn cảm hứng cho tên skill/item về sau, không đổi tên/cơ chế SkillId hay catalog hiện hành. Dùng motif/crop trên kit sẵn có, không mỗi linh vật một asset family bắt buộc.

| Mốc / action | Hình tối thiểu và progression | Gameplay-critical / phần có thể cắt |
| --- | --- | --- |
| Lv 1–2 | Movement/outfit mặc định; không fake skill trước Q6 | Đọc player/NPC/platform; bụi chân phụ có thể bỏ |
| Q3/Tân Lữ normal | Mộc Kiếm thật, swing ba pose, một dấu quét ngắn và impact tại target | Hit moment/weapon là core; trail dài/sparks là polish |
| Kiếm S1 sau class | Single Phong Trảm giữ cue riêng, reuse Attack/Skill pose hợp lệ | Không dựng class Normal executor hoặc zero-MP fallback |
| Cung S1 sau class | Draw/release + logical single/visual arrow | Không gap normal Cung executor: action class là S1; visual không damage |
| Lv 5 Phong Trảm | Cùng cây kiếm, dấu linh ấm ở release; nét chém rõ hơn normal nhưng chỉ một target 1,7 u | Một nhịp cast/shape riêng; không vẽ quét rộng ám chỉ ba con đều nhận damage |
| Lv 5 Linh Tiễn | Cùng cây cung draw/release; tên có tip/trail linh khí mảnh, khác tên thường | Logical single result; một visual arrow, không nhiều bóng tên giả |
| Lv 10 Phong Trảm | Một nhịp kiếm khí nóng, motif linh thú gắn trong đường khí; tối đa ba actual landed impacts quanh primary | Không cone 120° chọn victim; concept A/B ở bảng dưới, không ba linh thú độc lập |
| Lv 10 Linh Tiễn | Một draw/release, ba spirit arrow trails có glyph ngắn tùy chọn; cùng logical resolve +0,12 s | ABC/ABA/AAA cho phép hội tụ; không bắt fan, ba cast hoặc ba chim riêng. Status một roll/unique target |
| Lv 13 nội tại | Icon/tooltip mở, hit đủ điều kiện có accent nhỏ tùy chọn | Không thêm aura liên tục hay skill slot; không làm accent thành proc gameplay mới |
| Lv 17 Kiếm Khí | Chuẩn bị/tụ rõ hơn, một đường kiếm khí mạnh hướng primary; tối đa năm actual impacts quanh primary | Primary range 5,5 u không là rectangle collider; secondary ngoài nét khí vẫn có impact nếu authority cho hit |
| Lv 17 Hàn Tiễn | Một tên mạnh, arrival rõ, nổ lạnh radius 2 u; primary tâm và secondary cùng nhịp nổ | Logical primary + explosion authority; flash tâm ngắn, vành nổ gọn. Không thêm tên phụ gây damage |
| Lv 18–20 | Gear III và signature giữ nhận diện, status theo loại target | Không tự thêm evolution Lv 20 hoặc tăng effect vô hạn theo enhancement |

Lv 5 khác basic bằng tư thế tụ ngắn, dấu phát đòn, đầu tên/nét chém và impact cùng motif. Lv 10 tăng độ rộng/số nhánh để đọc đánh lan. Lv 17 tạo cảm giác mạnh bằng **chuẩn bị rõ → hình đòn lớn có khoảng trống → kết thúc sạch**. Không kéo action lock để thêm thời gian diễn.

**STRONG DIRECTION — S2 thường xuyên:** S1 và S2 đều phản hồi nhanh; S2 có thể là kỹ năng player chọn để farm qua nhiều lần Execute. Ưu tiên thế chuẩn bị gọn, nét quét/ba nhánh rõ, impact thỏa mãn và kết thúc sạch khi lặp liên tục. Không để S3 là đòn duy nhất có art tốt. Thử S2 trên camera đông nhiều bãi và nhiều player: tái dùng strip/preset, giảm trail/hạt thừa, giữ silhouette actor và telegraph. Exact frame/VFX count vẫn OPEN; không kéo thời gian khóa hành động để chứa hiệu ứng.

Khởi điểm authoring: slash/wave **4 hình** (mở–active–co–tan), burst **5 hình** (arrival–mở–vành–vỡ–tan), impact **3 hình** (bật–tách–mất). Ít hơn mất hướng/nhịp hoặc thành nhấp nháy; thêm frame chỉ giúp decay mượt, không thêm hit. Dùng lại strip bằng scale/tint/rotation và duration theo profile; không nhân bảy weapon visuals. Đây là dải probe chất lượng, không bộ sprite đã khóa.

<a id="5-multi-target-cùng-action-đúng-thời-điểm"></a>

## Multi-target: cùng action, đúng thời điểm

**Authority:** [Combat — propagation](../01-design/combat-and-character.md#target-propagation) chọn primary/secondary và resolve Evade/Crit/status. VFX chỉ trình diễn tập kết quả; trail không collider, không AnimationEvent/arrival callback sửa HP. Acquisition, propagation và presentation là ba trách nhiệm riêng.

| Trường hợp | Presentation | Mốc result |
| --- | --- | --- |
| Single | Một cast và một impact nếu landed; NÉ không wound | S1 +0,12 s; reject trước start không cost/pose thành công giả |
| Kiếm S2 primary + tối đa 2 | Một main qi slash và impacts tại actual victims quanh primary | +0,14 s; primary pinned, secondary snapshot tại HitMoment; không chạy chuyền |
| Kiếm S3 primary + tối đa 4 | Một main qi wave; impacts đúng IDs, không intersections | +0,16 s; primary index0, secondary order theo Combat; không hứa damage ở mọi điểm dưới nét khí |
| Hàn1+4 | Một tên chính và nổ lạnh đúng radius2; primary không nhận damage lần hai | +0,18 s; invalid primary không nổ, primary Evade vẫn nổ/secondary rolls |
| Spread ABC/ABA/AAA | Một draw/release, ba trail theo index, có thể hội tụ cùng A | +0,12 s; invalid index mất hit, no refill/reacquire; một status roll/unique target |

Source/visual/SkillId snapshot cố định suốt action; đổi selectedSlot/gear không morph action đang chạy. Travel/streak phải ăn result clock, không trì hoãn HP cho tên bay hoặc VFX tan. Primary chết/invalid xử lý theo từng executor, không âm thầm retarget. Secondary sau lưng primary/cùng identity khác SpawnGroup vẫn cần impact nếu actual eligible; khác tầng/qua SolidWall theo profile probe, không suy từ overlay.

<a id="six-skill-visual"></a>

### Sáu skill — visual contract đề nghị

**STRONG DIRECTION** về ưu tiên S2 và một main effect/execution; hình cụ thể **PROPOSAL/PROBE**, chưa có asset nghiệm thu. Timing là contract Combat hiện hành. Màu Kiếm ấm/Cung lạnh là direction; tỷ lệ màu/HEX OPEN. Linh thú là nét khí/ornament, không entity. Character Pose + Weapon State + Skill VFX + Target Impact / Projectile Presentation khi cần là các vai trò riêng. Skill VFX không bake vào Upper/Armor hoặc Lower/Pants fragments; giữ one-main-action-per-cast và one-main-VFX baseline, không bắt mọi skill có nhiều VFX layers. Suppress chỉ default main weapon VFX.

| Skill | Character pose | Main VFX | Color / motif | Per-target impact | Status visual | SuppressDefault? | Timing | Asset reuse |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Kiếm S1 / Phong Trảm đơn | Windup ngắn → swing → recovery | Default slash/trail nhỏ + qi accent đủ khác novice | Ấm, nét Mạch Ấn | Spark nhỏ đúng một landed result; NÉ chỉ glyph | Không status S1; qi ấm chỉ cosmetic | Không bắt buộc; cho phép default trail làm main | +0,12 s; không kéo action lock | Swing/Grip/Tip, slash4/impact3; remote giảm sparks |
| Kiếm S2 / Phong Trảm tiến cảnh | Một windup gọn, swing dứt khoát, pose farm lặp sạch | A: kiếm khí nóng gắn nét bờm/sừng/vuốt; B: qi-sword slash rắn | Ấm; A gợi Lân/Kỳ Lân, không con thú chạy riêng | Tối đa3 actual landed impacts cùng phase | Bỏng thật; không biến motif nóng thành proc mặc định | Có; main của skill thay default slash | +0,14 s; release gọn dù CD ngắn | Swing/Grip/Tip + qi strip4 + impact3; LOD giản motif |
| Kiếm S3 / Kiếm Khí | Tụ rõ hơn S2 → release → hồi thế | Một đường qi lớn hướng primary, không collider rectangle | Ấm, Mạch Ấn/qi đậm; không thêm summon | Tối đa5 actual impacts theo ordered set | Bỏng thật, không tăng chance | Có; một main wave, không thêm DefaultSlash | +0,16 s; tan sau hit được | Reuse qi strip/impact bằng profile scale-duration; remote giữ hướng |
| Cung S1 / Linh Tiễn đơn | Một draw → release; nock/muzzle đúng pose | Một spirit arrow trail | Lạnh, tip/glyph nhỏ | Một actual landed impact | Không status S1; trail lạnh chỉ cosmetic | Có nếu default arrow effect có sẵn; một main arrow | +0,12 s | Bow draw/Muzzle/nock, trail/impact; remote giảm hạt |
| Cung S2 / Linh Tiễn tiến cảnh | Một draw/release mạnh hơn, lặp farm gọn | Ba spirit trails; rune ngắn/wing accent tùy chọn | Lạnh; không ba chim/thực thể, không fan bắt buộc | Ba indices ABC/ABA/AAA; A trúng lặp vẫn ba results, không ba status | Một roll/unique landed target; status lifetime thật | Có; thay default arrow, không cộng thêm ba default arrows | Cùng +0,12 s; không A→B→C delay | Reuse S1 trail + glyph, profile index aim; remote đủ ba nhịp/index cue |
| Cung S3 / Hàn Tiễn | Draw tụ → một release → recovery | Một tên lạnh + một burst radius2 tại valid primary | Lạnh, băng/Mạch Ấn; vành nổ đọc đúng2u | Primary một lần + tối đa4 secondary; Evade không wound | Freeze/Slow thật; nổ lạnh không chứng minh mọi target frozen | Có; arrow/burst thuộc một skill execution, bỏ default arrow | +0,18 s; invalid primary không burst | Trail S1 + burst5 + impact3 + status chung; remote giữ vành/telegraph |

**Kiếm S2 A/B:** A tốn thêm một qi motif strip/cleanup nhưng signature Á Đông mạnh, vẫn reuse swing/impact và đọc nhanh nếu linh ảnh nằm trong nét khí; rủi ro motif quá lớn che primary/telegraph. B rẻ hơn, silhouette kiếm rõ và dễ lặp nhưng ít nhận diện riêng. **Recommendation vertical slice: thử A trước**, giữ B làm control/fallback nếu A không đọc được ở camera thật. Không mặc định ghép A+B hoặc DefaultSlash + SkillSlash + Creature. So cùng timing, target-set và 2/4+ player, đo giờ/reuse/overdraw; hình cụ thể chưa LOCKED.

### Art completeness — sáu skill, chín trường visual

| Skill | Pose | Socket | Main FX | Default trail policy | Motif | On-hit | Status | Remote LOD | Reuse |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Kiếm S1 | Swing | Grip/Tip | Slash nhỏ | Cho phép làm main | Ấm/ấn | Single landed | Không S1 status | Giảm sparks | Swing/slash/impact |
| Kiếm S2 | Một swing mạnh | Grip/Tip | A qi motif, B control | Suppress default | Lân embedded | ≤3 actual | Burn thật | Giản motif, giữ release | S1 pose/qi/impact |
| Kiếm S3 | Tụ/release | Grip/Tip | Qi wave | Suppress default | Ấm/ấn | ≤5 actual | Burn thật | Giữ hướng, giảm debris | Qi profile/impact |
| Cung S1 | Draw/release | Grip/Muzzle/nock | Arrow | Một arrow main | Lạnh | Single landed | Không S1 status | Giữ source/result | Draw/trail/impact |
| Cung S2 | Một draw mạnh | Grip/Muzzle/nock | 3 index trails | Suppress default | Rune/wing optional | ABC/ABA/AAA | Unique proc | Giữ index cues, bỏ rune phụ | S1 trail/glyph |
| Cung S3 | Tụ/draw/release | Grip/Muzzle/nock | Arrow+burst2u | Suppress default | Băng/ấn | Primary+≤4 | Freeze/Slow thật | Giữ radius, giảm shards | Trail/burst/status |

**Profile proposal tối thiểu:** SkillId → actor pose/phase mapping, sockets, main pool key, `SuppressDefaultMainWeaponVfx`, impact/status pool keys, tint/motif/scale/duration và local/remote LOD. Không per-skill controller/service mới. Main theo actionId, trail theo hitIndex, impact theo actual resultId/target life, status theo type/revision; dedup và reset pool cả phase/parent/sorting/material/life/map. Reconnect dựng status/phase còn hiệu lực, không replay cast/impact cũ. Sorting giữ actor/weapon riêng, main FX không che Boss telegraph; một impact nhỏ/landed result, một status/type/target. Cắt remote particles/shake trước hướng đòn/actual impacts.

C0 kiểm targeting độc lập trước full VFX; minimal Kiếm S2 **và** Cung S2 có ưu tiên ngang nhau với S1/S3 tại Pha R. Khởi điểm strip4/burst5/impact3 là dải probe, không tăng 26 frame player hay khóa tổng asset. VFX có thể retire sau damage, không giữ target/action sống để chờ fade.

<a id="mob-visual"></a>

<a id="6-mob-animation-theo-hành-vi"></a>

## Mob: animation theo hành vi

**Đọc từ:** owner design tương ứng; bảy loại quái/sáu rig giữ nguyên, Sói Trúc dùng lại Sói Sương bằng palette/name. Không ép 26 frame player lên mob. Hybrid count/identity còn OPEN; pose ranged chỉ sản xuất khi capability được chọn. [Bảng lịch sử](../90-archive/art-history.md) không là budget hiện hành.

Mob pose budget theo capability/pose map được chọn; bảng 105 hình cũ ở [Art history](../90-archive/art-history.md).

Lý do bốn ô attack: nhận thế/aim → windup silhouette → hit/release → recovery. Ba ô vẫn được nếu aim đọc từ facing/hold; hai ô dễ mất báo trước, nhất là ranged. Thêm frame chỉ làm chuyển động mượt, không tăng attack rate. Wolf cần stride/lunge đọc hơn Nấm; Stone bốn move đủ tạo sức nặng; Ong không cần bộ đi bộ hoặc melee không có gameplay. Wing bốn pose có thể loop nhanh hơn thân (probe 12–16 FPS), nhưng release đọc bằng thân/dấu phát đạn, không theo nhịp cánh.

Nấm Death ba ô: xẹp→đổ→bẹp. Sói bốn: gục đầu→khuỵu→đổ→nằm; bớt một chuyển vẫn có thể pass. Đạo Tặc/Cổ Vệ bốn cho trọng lượng thân người/giáp; Thạch bốn cho nứt→rụng→sụp→tàn, không debris physics. Ong bốn cho mất wing→rơi presentation→chạm/rụng→tàn; không bắt chước corpse Sói nằm giữa không khí. Hit một hình chỉ dùng lúc rảnh; flash/impact cho mọi action ở các mục liên quan.

Nếu chọn Hybrid thì cần **tư thế đánh xa**, dù dùng cùng Linh Đạn: Đạo Tặc phóng/ném, Thạch tụ/phóng mảnh linh lực, Cổ Vệ đưa vũ khí/ấn phát. Đây là gesture candidate, không ba họ projectile mới. Đạn dùng chung, chỉnh tint/scale/trail; tốc 5/4/6 và Ong 5,5 u/s là tham số hình ảnh giữ từ mốc cũ, không quyết clock damage. Chốt capability và pose cần thật trước production; không tự thêm ranged cho Sói hoặc quái melee phía sau để cân Cung.

Idle 2 thay4 giảm công vì nhịp thở ít quan trọng hơn windup/release; không thêm flourish không phục vụ hành vi P0.

<a id="61-boss-là-dependency-art-bắt-buộc-ngoài-bảng-sáu-rig"></a>

### Boss là dependency art bắt buộc ngoài bảng sáu rig

design owner có một Cự Thú, basic + Nham Trảo + Địa Chấn + ba vùng Nham Thạch Rơi + Cuồng Mạch. Không được bỏ Boss khỏi scope chỉ vì checklist nhấn normal mob. Kịch bản: Idle 4 + Move6 + Basic/Claw4 dùng chung motion + Slam4 + Cast4 + Death6 = **28 hình**, thêm Roar4 **tùy chọn** =32. Move6 chỉ cần nếu reposition có đi; bốn frame death sẽ rẻ hơn nhưng thân lớn sụp dễ thiếu trọng lượng, sáu là probe có lý do. Claw khác basic bằng telegraph/config, chỉ redraw nếu silhouette không phân biệt được.

Canvas Boss **OPEN** theo kích thước world cần đánh/né; giữ PPU32, thử 128/192 px thay vì phóng ảnh 64 px thành khối thô. 28 frame Boss không ngang cost 28 frame player: diện tích, cleanup và telegraph QA lớn hơn. Ba telegraph shape: cone, ground AoE, landing zones; vùng đá dùng cùng shape ba lần, không ba asset riêng. Địa Chấn phải báo vùng nhảy né, landing zones không ám chỉ double-hit overlap.

Cuồng Mạch tint/glow/roar feedback theo ngưỡng server; không chen Roar animation làm ngắt action/telegraph hiện tại hoặc thêm stun/lock. Nếu không có khoảng rảnh để roar, dùng accent/âm thanh và giữ pose chính. Boss Slow giữ telegraph/action đã start nguyên tốc độ; Dư Ảnh chỉ đổi tên per viewer, không rig/entity/phase mới.

<a id="7-mob-nhận-damage-khi-đang-action"></a>

## Mob nhận damage khi đang action

**Đọc từ:** owner design tương ứng; damage không đồng nghĩa stun. Reaction là lớp presentation có ưu tiên: **Death > CC authoritative > action đang chạy > locomotion**, impact/flash có thể chồng lên lớp chính.

| State lúc bị hit | Main animation | Feedback đề xuất | Không được làm |
| --- | --- | --- | --- |
| Idle | Có thể dùng Hit1 rồi về Idle phase hợp lệ | Impact/flash; recoil visual nhỏ nếu không che facing | Tự thêm cooldown/immobility |
| Move/Chase | Tiếp tục move và Run phase | Flash + impact; offset sprite nhẹ, nhanh về 0 | Reset stride mỗi hit, dừng movement hoặc đẩy physics root |
| Melee windup/Attack | Giữ windup/hit/recovery và facing khóa | Impact + flash; mặc định bỏ recoil nếu làm lệch telegraph | Attack→Hit→Attack restart, trễ hit hoặc bỏ hit vì client đổi animation |
| Ranged aim/release | Giữ aim pose, hand/socket và projectile spawn phase | Flash/impact không dời muzzle | Spawn thêm đạn khi animation quay lại release; tự retarget theo attacker |
| Recovery | Giữ recovery đến mốc authoritative | Recoil visual chỉ nếu không che return pose | Cấp đòn kế sớm vì Hit clip ngắn |
| Frozen thật | Cancel pending action theo server; giữ pose ngắt dưới overlay băng | Freeze entry/tan; hết CC về state mới của server | Resume windup cũ rồi phát pending hit đã bị hủy |
| Boss/PvP Slow | Main action giữ nguyên thời gian | Lam mờ/status icon theo expiry | Dùng Hit/ice-block hoặc giảm animation tốc độ action |
| HP 0/Death | Terminal death pose, clear status theo server | Hit cuối gọn dẫn vào Death, bỏ Hit dài | Cố phát nốt hit/spawn chưa giải quyết hoặc đợi Hit clip xong mới chết |

Flash thử 60–100 ms; giật hình nhẹ 1–2 px/60–120 ms chỉ trên visual khi không sai hướng đòn. Quá ngắn khó thấy, quá dài giống stun và tăng nhấp nháy; số này **TUNABLE**, không dừng clock gameplay. Hitstop/crit shake/âm theo vật liệu DEFERRED. Packet nhiều hit có thể giảm cường độ flash lặp nhưng giữ từng result; không reset action clock.

<a id="8-mob-death-lifecycle-và-corpse"></a>

## Mob death lifecycle và corpse

**Đọc từ:** owner design tương ứng, owner kỹ thuật tương ứng. Respawn normal/Linh hiện tính **deathUtc+25 s**, không tính từ lúc corpse tan hoặc DB ACK. Body không gây contact damage/blocking hiện hành; corpse càng không được tạo chướng ngại mới.

**Giả định review A15 cho bảng/ví dụ dưới:** đặt `t0` ở lethal HP 0 và giả định `deathUtc=t0` để thử ACK trễ; chưa khóa API/event/timestamp capture hoặc thời điểm release Linh cap ở terminal-pending. Invariant đã có là không reward/respawn trước finalize, và corpse không reset deadline. [Technical gate](../04-production/playtest-and-balance.md#pending-ordering-probes) phải kiểm/chốt các chi tiết này trước production lifecycle; presentation terminal trước ACK vẫn là proposal.

| Mốc | Gameplay / persistence | Presentation đề xuất |
| --- | --- | --- |
| Hit cuối/HP 0 tại t0 server | Terminal-pending; loại khỏi target/hurtbox queries, dừng AI/threat action, cancel pending hit/spawn; chụp ledger/deathID/generation | Last impact ngắn dẫn thẳng Death; bỏ Hit clip dài, clear status đang phủ thân. Phải có terminal state server, không client tự đoán chết |
| Chờ commit | Không nhận hit, không reward/loot/quest success và không respawn; payload giữ bất biến | Có thể chạy Death theo terminal state; đây là **đề xuất bổ sung event presentation** cho Technical, không publish reward sớm. Lỗi kéo dài dùng thông báo gián đoạn chung |
| ACK death transaction | Finalize reward/loot/credit một lần; deadlines vẫn từ t0 | Loot hiện ở điểm death hợp lệ, không chờ Death animation. Banner Boss/quest success chỉ sau ACK |
| Death clip xong | Entity gameplay terminal; corpse chỉ là hình có lifetime riêng | Ground giữ frame cuối; flying dùng nhánh rơi/tàn phía dưới. Không hurtbox/AI ở frame corpse |
| Hold/fade | Không đổi deadline, contribution, pickup ownership | Probe ground: clip khoảng 0,35–0,60 s, hold 0,5–1,5 s, fade 0,3–0,5 s; rồi trả visual pool |
| Slot tới hạn | Spawn life mới nếu death đã finalize và map lifecycle cho phép; ID/generation mới | Không reuse corpse như mob sống bằng bật hurtbox trước reset. Old callback không gắn vào life mới |

Ví dụ kiểm: Death4 ở 8 FPS =0,5 s; hold1 s + fade0,35 s → corpse biến mất **t0+1,85 s**, respawn vẫn t0+25 s. Hold ngắn hơn giúp bãi sạch nhưng mất dấu đã hạ mob; lâu hơn giúp nhìn kết quả nhưng tăng clutter khi nhiều người farm. Boss có thể hold 2–4 s để thấy kết thúc, **OPEN**, không đổi respawn 15 phút/pile90 s. Không bắt số này bằng người chơi phải chờ animation.

ACK tới t0+30 s thì deadline 25 s đã qua: chỉ được spawn sau finalize theo rule due-slot, không đặt lại 25 s từ ACK và cũng không spawn trước commit. Nếu loot đã mất phần lifetime khi ACK trễ, không tự reset owner/expiry windows bằng animation; đó là dependency reliability cần QA, không art tự bù. Nếu technical muốn định nghĩa deathUtc khác t0, phải chốt/sync rõ trước implementation.

**Flying death:** phương án rẻ là mất wing→trượt/rơi **visual-only** tới nền gần hợp lệ rồi tàn; fallback tan tại chỗ nếu dưới là hố/khác tầng. Không thêm Rigidbody corpse, collision loot hoặc hurtbox rơi qua player. Vị trí loot do server author/resolve điểm pickup đứng được; nếu hiện chưa có ground projection, ghi **OPEN A08**, không client raycast tự đổi vị trí loot đáng tin. Test Ong chết trên cầu, mép vực, giữa các tầng; loot không nằm ở điểm Kiếm không thể tới.

<a id="player-shadow-death"></a>

## Player death — shared shadow form

**STRONG DIRECTION, chỉ player:** authoritative lethal/dead state → bật/giật nhẹ visual → rơi → bóng đen có đôi mắt → mắt/bóng nhấp nháy → chờ revive/về làng hoặc transition tương ứng. Pop/fall reuse Hit/Jump/Fall hợp lệ; không death sheet riêng Kiếm/Cung, không corpse sprite đủ đồ. Khi vào shadow, ẩn Head/Hair, UpperBody, LowerBody và Weapon, chỉ giữ visual chung; một shadow + eyes overlay hoặc composite blink là lựa chọn PROBE. Hình cụ thể, biên độ/thời lượng, blink, thời điểm shadow biến mất và respawn visual đều OPEN/TUNABLE.

**PROPOSAL đồng bộ tối thiểu:** renderer đọc player terminal/dead state có identity/life-generation, MapId, vị trí và mốc clock authority; offset pop/fall chỉ trên VisualRoot, không Rigidbody/knockback/collider hay dịch gameplay root. Giữ death anchor/camera tại vị trí authority theo recovery policy, không follow offset bật lên. Một visual phase track đủ, không thêm gameplay state machine. Duplicate/stale event không replay pop; late viewer/reconnect đã dead đi thẳng phase hiện tại/shadow. Revive/map/life mới reset offset/visibility/timer và bỏ callback đời cũ. Schema/event cụ thể theo [Runtime presentation spike](../02-technical/gameplay-runtime.md#presentation-data), không tự mở transaction hoặc timer hồi sinh mới.

**Gameplay giữ nguyên:** death anchor/quan sát phải còn tới revive/về làng; không áp auto-fade/despawn normal lên player. Shadow có thể chỉnh visibility nhưng không xóa actor/dead state, HUD hay lựa chọn hồi sinh. Corpse Boss contributor ≥10% còn trong area vẫn hợp lệ theo design owner, không pickup tới khi sống. [Recovery policy](../02-technical/online-and-persistence.md#profile-authority) quyết vị trí corpse/SafeAnchor; trạng thái corpse art không quyết định quest eligibility. PvPDefeated dùng kết thúc trận, không mở lựa chọn Hồi Sinh Phù/PvE death.

**Player chết vẫn quan sát mục tiêu:** theo [Combat & Character — CombatFocus](../01-design/combat-and-character.md#focus-input), death hủy PendingCast/buffer/approach và khóa combat input, nhưng không tự xóa focus. Marker, mini HP và HUD tên/cấp/current-max HP vẫn cập nhật khi người khác đánh target còn sống, đúng life/generation/MapId và trong retention range. Target chết/despawn/đổi đời, MapId không hợp lệ, vượt vùng giữ hoặc Esc/chọn đích khác mới xóa/thay focus.

Mob hồi sinh cùng SpawnSlot không kế thừa HUD life cũ. Hiển thị này dùng state authority, không camera corpse tự giữ GameObject đã tái dùng.

<a id="9-bù-nhìn-dùng-chung-tutorial-và-training"></a>

## Bù Nhìn dùng chung tutorial và training

**Đọc từ:** [Q3/Q6 owner](../01-design/quests-and-narrative.md#quests-story). Dummy dùng cùng prefab/pool/lifecycle; HP, placements, respawn và credit theo quest owner, không tạo identity tutorial/training riêng. Art kiểm silhouette, impact/death và contention trước thêm pose.

| Nhu cầu | Kiểm baseline | Recommendation / trade-off |
| --- | --- | --- |
| Q3 Lv 3/Mộc Kiếm | ATK suy ra: 12+1,2×2+0,7×4+10=27,2; HP 60 khoảng ba landed normal nếu DEF thấp. DEF/EVA dummy chưa ghi đủ | Giữ HP 60 cho probe Q3; author rõ DEF/EVA, không lấy ngầm từ mob bất kỳ. TTK tùy miss/DEF, chưa claim chính xác |
| Q3 bốn kills / năm placements | Ba điểm chính + hai phụ cùng Bù Nhìn; bốn valid life kills đủ, giết lại slot respawned vẫn tính | Không bắt kill bốn IDs khác nhau hoặc chờ respawn; giữ HP60/timer25s baseline, đo online contention trước đề nghị đổi timer |
| Player level cao quay lại | Lv 20 Common III cân bằng ATK 91,6 có thể one-shot60; không đủ test nhiều action/status liên tục | Đủ xem per-hit damage và đúng weapon/cast; không hứa DPS Meter hay test rotation dài P0. Không tự tăng HP làm Q3 dài |
| N player cùng đánh | Một shared HP/life và ledger; one-shot của người mạnh có thể chiếm toàn life; Q3 không dựa last hit | Server serialize damage/death/respawn, credit từng requester ≥20%; HUD không báo đã hạ nếu chỉ nhìn người khác đánh. Nhiều slot/respawn nhanh giảm chờ, không đảm bảo mọi requester credit cùng life |

**Visual tối thiểu:** Idle 1 (dáng tĩnh), Hit2 (cong→bật lại), Break3 (nứt→gãy→đống rơm), dùng frame cuối làm corpse = **6 hình nếu đều khác**. Ít hơn Break dễ giống despawn, hơn hai Hit chỉ làm lắc mượt không dạy mechanic mới. Dummy không Run/Attack; Hit không dừng server action của player. Repeated hit reset/cộng lắc phải clamp, không lắc tới che body/HP bar. Có HP/name, damage/Crit/NÉ cùng pipeline; Death/Break terminal rồi respawn như life mới, không reward vật phẩm/EXP.

**A07:** năm placements đã là direction hiện hành, không còn quyết định ba hay năm. HP60/respawn25s giữ baseline; DEF/EVA, khoảng cách và contention còn cần đo. Chạy bốn kills không mandatory wait, Q6 thiếu MP tự nhiên/full-MP reject và người mạnh cùng yard trước đề nghị đổi timer. Adaptive HP/personalized Dummy không được thêm P0; rotation test sâu dùng fixture dev có nhãn, không thêm loại Dummy player-facing.

<a id="art-timing"></a>

<a id="10-timing-presentation-và-vai-trò-skill"></a>

## Timing presentation và vai trò skill

Bảng timing/probe, phép so Kiếm/Cung, tỷ lệ thời gian khóa hành động, sustain Lv 5/10/20 và nhịp proc đã chuyển **đầy đủ** sang [Design history — timing](../90-archive/design-history.md#art-combat-timing-evidence), [sustain](../90-archive/design-history.md#art-sustain-evidence) và [proc](../90-archive/design-history.md#art-proc-evidence).

Các số cũ là **LEGACY / SUPERSEDED** khi dùng class Normal, cadence/gear/HP cũ. [Combat & Character](../01-design/combat-and-character.md#class-combat) giữ mốc mới BASELINE/TUNABLE; [Playtest & Balance — phép thử hiện hành](../04-production/playtest-and-balance.md#current-balance-probe) giữ giả định và tính lại. Không dùng bảng cũ nghiệm thu revision mới.

Art author **chuẩn bị → phát/trúng đòn đúng mốc authority → hồi thế**. Tách interval/CD/action lock khỏi thời gian tên hình ảnh bay và VFX tan. Attack3/Skill4 dùng duration/hold từng pose để khớp clock; không ép gameplay chạy theo FPS sprite đều. VFX có thể tan sau actor về Idle; recovery chuyển ở biên action hợp lệ, bỏ frame không tạo hit thêm. CURRENT minimal Kiếm/Cung ở Pha R; Cung S1/S2 draw/release/travel phải thử sớm. S3 dùng fixture hẹp ở gate mở rộng.

Role và MP/CD ở [Combat owner](../01-design/combat-and-character.md#class-combat). Art cần làm S1/S2 đọc được khi dùng thường xuyên và S3 có signature; không kéo dài lock để chạy hết clip.

**S2 là ưu tiên hình ảnh tần suất cao (High-Frequency Visual Priority):**
Trong vòng lặp chơi thực tế, chọn S2 rồi bấm Execute liên tục là cách đánh phổ biến nhất để dọn bãi. Do đó, **animation của S2 (Phong Trảm tiến cảnh / Linh Tiễn tiến cảnh) phải được đầu tư tư thế chuẩn bị (windup/draw), thời điểm phát đòn (release) và cảm giác trúng đích (impact) cực kỳ rõ ràng, dứt khoát và sướng mắt**, không được coi S2 là kỹ năng phụ mà dồn hết công sức sang S3. Animation của S2 phải gọn ghẽ, không vẽ động tác giật/khựng dài làm kẹt cảm giác ra đòn.

Art/icon tái dùng motif/pose nhưng ba entry và sáu SkillIds độc lập. Pose Attack/Skill là ngôn ngữ hình ảnh, không đồng nghĩa S1 là Attack mặc định. Status được thử bằng fixture có chủ đích, không đổi chance để dễ thấy. Bow range/kite là lợi thế hợp lệ; thay đổi HP sau nhập phái và chỉ số gear đọc design owner, Art không tự cân HP hoặc giảm VIT của Cung.

<a id="map-visual"></a>

<a id="11-kiến-trúc-visual-map"></a>

## Kiến trúc visual map

**Đọc từ:** owner design tương ứng, owner kỹ thuật tương ứng. Năm farm + ba support roots là **tám bố cục**, không tám tileset mới. Giữ ba họ chất liệu `Forest / Mountain / Ancient` và các ID/folder kỹ thuật ổn định; từng map có dấu mốc và tuyến đi riêng.

**STRONG DIRECTION hình ảnh:** cổ phong võ hiệp/huyền huyễn Á Đông, Việt-inspired, không khóa triều đại/trang phục lịch sử. Sắc thái **gần gũi có võ học → hiểm trở → huyền bí**: gỗ/tre, mái ngói cong vừa phải, học viện/võ đường/sơn môn, cầu/cổ đạo, lò rèn, khe/thác, vách núi, cổng/bia/linh văn và trấn ấn. Vân Khê vẫn có chất làng bản, không biến toàn game thành thuần nông thôn. Dùng lại địa hình, kit công trình nhỏ, mob rigs, palette Sói, male modular rig và motif VFX; không thêm environment family, tileset riêng từng map hoặc hàng chục công trình độc nhất.

| Environment Family | Sắc thái văn hóa và cảnh quan sơn cước | Bản đồ áp dụng & Kế hoạch tái sử dụng |
| --- | --- | --- |
| **Family 1 — Làng / nương / tre / rừng ẩm** | Gỗ mộc, mái lá/ngói giản lược, hàng rào tre, đồi nương đất đỏ pha cỏ xanh, suối mát; gợi không khí làng bản vùng cao Việt Nam. | **Vân Khê, Đồng Sương, Trúc Ảnh:** Dùng chung structural kit gỗ/tre, chỉ đổi palette và biến thể cây cối. |
| **Family 2 — Núi / vách đá / thác / xích nham** | Vách đá vôi dựng đứng, đèo bậc hiểm trở, thác nước trắng xóa, chuyển tiếp sang các phiến sa thạch đỏ khô cằn. | **Bạch Vân (đá xám lạnh, thác nước) và Xích Nham (đá đỏ ấm, khe nứt):** Cùng chung bộ kết cấu đá khối bậc solid, khác biệt về màu sắc và ánh sáng. |
| **Family 3 — Phế tích cổ / trấn ấn / Huyền Môn** | Phế tích đá tảng nguyên khối rêu phong, bia đá mang hoa văn Mạch Ấn cổ xưa, tàn tích cổng phong ấn thâm u. | **Huyền Tích và các khu cấm địa phong ấn:** Dùng chung bộ đá cổ (`ancient ruin kit`) và hoa văn ấn khắc; không vẽ riêng mỗi khu vực. |

**Tên và chữ trong hình:** giữ Huyền Lộ, Linh Biến, Huyền Môn, Mạch Ấn/Trấn Ấn, Nấm Linh, Sói Sương, Sói Trúc Ảnh, Ong Giáp, Đoạt Mạch Đạo Tặc, Xích Thạch Linh và Cổ Môn Vệ Binh. Thanh Mộc/Vân Nham/Huyền Ấn vẫn là ba family gear. Hán-Việt của quái/cổ vật/địa danh cổ hợp thế giới; signage dịch vụ dùng lời gần gũi. Không đổi display name tốt thành tên tầm thường hoặc thêm Thiên/Thần/Đế/Tôn để gây vẻ lớn lao. Nếu sau này đổi display name, giữ stable internal ID; không đổi folder/definition chỉ vì tên hiển thị.

| Nhóm | Nội dung/đơn vị production | Unity/presentation và reuse |
| --- | --- | --- |
| Background | Silhouette xa, trời/núi/rừng/phế tích; mảng nền theo family | Sprite lớn hoặc BackgroundTilemap không collider; giảm contrast, reuse palette/crop. Parallax phụ nếu camera cần, không framework riêng |
| Terrain Kit | Mặt trên/fill/cạnh/góc của khối solid; one-way thuộc kết cấu riêng | Collision solid/platform tách; cấu trúc nối dùng chung, texture ba họ khác |
| Decoration | Cỏ/trúc/đá vụn/cột đổ/biển đường | Sprite/prefab cụm; đặt sparse trong combat lanes, không collider vô cớ |
| Structural/Full Assets | Cầu/mái/cổng/tầng phế tích/lò rèn/ấn | Một visual nhiều mảng nhưng chỉ vài collider surfaces; các mục liên quan |
| Animated Environment | Thác/nước/lửa/khói/lá/bụi | Sprite loop/animated tile/ParticleSystem/static overlay theo các mục liên quan |
| Foreground | Cành, mỏm đá viền camera, lớp nước trước chân | Riêng layer/order và vùng occlusion; không che telegraph/name/loot |

### Visual Bible chi tiết cho 8 bản đồ logic

Mỗi bản đồ sở hữu một bộ nhận diện hình ảnh (Visual Identity) rõ rệt, gắn liền với nhịp độ gameplay và tiến trình cảm xúc của người chơi từ **gần gũi có võ học → hiểm trở cheo leo → huyền bí cổ xưa**:

<a id="1-làng-vân-khê--hub-bình-yên--bờ-cõi-sơn-cước"></a>

#### Làng Vân Khê — Hub bình yên & Bờ cõi sơn cước
- **Environment Family:** Family 1 — Làng / nương / tre / rừng ẩm (`Forest`).
- **Tone & Mood:** Thanh bình, mộc mạc, gần gũi, khơi gợi cảm giác thân thuộc của một bản làng vùng cao Việt Nam.
- **Palette gợi ý — PROBE:** Gỗ mộc ấm áp (`warm timber`), xanh xám lá xô thơm (`sage green`), đá xám tự nhiên (`gray stone`), vải gai lanh màu ngà (`cream linen`), ngói xám xanh nhạt (`muted teal roofs`).
- **Ánh sáng & Thời gian:** Ban mai hoặc xế chiều ấm áp, nắng xiên nhẹ qua rặng tre và tán cây rừng; không khí trong lành, sương mỏng tan dần.
- **Landmarks & Hình khối:** Nhà gỗ mái lá/ngói mộc, hàng rào tre, bảng gỗ chỉ đường, sọt dược thảo phơi khô của Yên Thảo, lò rèn rực lửa than của Bách Luyện, hòm gỗ nhà kho của Mộc An. Tuyệt đối không vẽ thành kinh thành lộng lẫy hay phố xá đồ sộ.

<a id="2-học-viện--huấn-luyện-nhập-môn--điện-nhập-phái"></a>

#### Học Viện — Huấn luyện nhập môn & Điện Nhập Phái
- **Environment Family:** Family 1 — Làng / nương / tre / rừng ẩm (`Forest`).
- **Tone & Mood:** Trang nghiêm, chuẩn mực, khơi dậy tinh thần rèn giũa võ học sơn cước.
- **Palette gợi ý — PROBE:** Gỗ sáng thanh nhã (`pale wood`), đá thanh xám nhạt (`pale stone`), cờ ngọc bích mờ (`muted jade banners`), vải lanh trắng ngà.
- **Ánh sáng & Không khí:** Ánh sáng rọi đều, rõ ràng, không có góc tối mập mờ, tối ưu cho việc quan sát thao tác nhân vật.
- **Landmarks & Hình khối:** Tuyến nhảy gờ đá (`HV_JumpLedge`), giàn ván gỗ mỏng (`one-way platform`), sân tập Bù Nhìn rơm (`HV_DummyYard`), và khu vực Điện Nhập Phái (`HV_ClassHall`) với hai giá vũ khí Kiếm/Cung đại diện cho hai phái được bố trí bình đẳng, dễ thấy (bố cục tả/hữu là đề xuất blockout).

<a id="3-lôi-đài--đấu-trường-1v1-pvp"></a>

#### Lôi Đài — Đấu trường 1v1 PvP
- **Environment Family:** Family 1 — Làng / nương / tre / rừng ẩm (`Forest`).
- **Tone & Mood:** Căng thẳng, tập trung cao độ, mang tinh thần thượng võ thuần khiết.
- **Palette gợi ý — PROBE:** Nền đá xanh xám phẳng lặng (`slate gray`), gỗ sẫm màu, cờ hiệu truyền thống đỏ thẫm pha vàng mờ.
- **Ánh sáng:** Nắng rọi trực tiếp vuông góc xuống sàn đấu, tạo bóng đổ ngắn sắc nét, giúp đọc silhouette, mục tiêu, hướng ra đòn và telegraph; ánh sáng không biểu diễn collider damage.
- **Cấu trúc:** Sàn đấu đá tảng trực giao phẳng phiu, sạch bóng chướng ngại vật; phông nền là hàng rào gỗ mộc và rặng núi xa; không có khán đài ồn ào hay màn hình công nghệ.

<a id="4-đồng-sương-lv-15--đồi-nương-bậc-thấp--suối-cạn-sương-mai"></a>

#### Đồng Sương (Lv 1–5) — Đồi nương bậc thấp & Suối cạn sương mai
- **Environment Family:** Family 1 — Làng / nương / tre / rừng ẩm (`Forest`).
- **Tone & Mood:** Thoáng đãng, hoang sơ nhẹ nhàng, bước chân mở đầu đầy hiếu kỳ.
- **Palette gợi ý — PROBE:** Cỏ xanh non mát mắt (`cool grass`), đất nương nâu đỏ ấm (`warm dirt`), đá xám viền rêu, sương mù lam nhạt (`fog blue`).
- **Ánh sáng & Không khí:** Sương mai bảng lảng trôi trên mặt suối cạn, ánh mặt trời le lói qua tầng sương.
- **Landmarks & Hình khối:** Đồi đất cỏ với chuỗi bậc thấp tạo cảm giác thoải khi nhìn tổng thể, dòng suối cạn nước nông, vạt nương hoang; bãi Nấm Linh lúp búp ven bờ suối và bầy Sói Sương xám tro rải rác trên đồi cỏ. Dải vào an toàn 6–8 u tại cửa làng thoáng đãng.

<a id="5-trúc-ảnh-lv-510--rừng-trúc-u-tịch--cầu-gỗ-đa-tầng"></a>

#### Trúc Ảnh (Lv 5–10) — Rừng trúc u tịch & Cầu gỗ đa tầng
- **Environment Family:** Family 1 — Làng / nương / tre / rừng ẩm (`Forest`).
- **Tone & Mood:** U huyền, tĩnh mịch, bắt đầu cảm nhận rõ mối đe dọa từ tà khí Linh Biến.
- **Palette gợi ý — PROBE:** Rừng trúc xanh ngọc bích sẫm (`jade green`), rêu ẩm xanh đen (`dark moss`), sương rừng lam biếc (`blue mist`), gỗ cầu đẫm nước (`damp timber`).
- **Ánh sáng & Không khí:** Ánh sáng lốm đốm tán xạ qua kẽ lá trúc dày đặc; hơi ẩm bốc lên từ lòng thung lũng rêu phong.
- **Landmarks & Hình khối:** Cầu gỗ giàn ván mỏng vắt ngang vực đá, trụ Trấn Ấn cổ bị nứt rỉ vệt trọc khí tím dưới chân cầu (`TA4_BrokenSeal`), bầy Sói Trúc Ảnh lục tối ẩn hiện dưới bóng trúc và đàn Ong Giáp bay lượn trên cao.

<a id="6-bạch-vân-lv-813--vách-đá-thác-nước--đèo-mây-nhiều-bậc-cao-độ"></a>

#### Bạch Vân (Lv 8–13) — Vách đá thác nước & Đèo mây nhiều bậc cao độ
- **Environment Family:** Family 2 — Núi / vách đá / thác / xích nham (`Mountain`).
- **Tone & Mood:** Hùng vĩ, hiểm trở, gió núi lồng lộng, lạnh lẽo và choáng ngợp.
- **Palette gợi ý — PROBE:** Vách đá vôi xám lạnh (`pale gray-blue rocks`), bọt thác nước trắng xóa (`white water foam`), bụi cây lá kim cằn cỗi (`muted green shrubs`), biển mây trắng bồng bềnh.
- **Ánh sáng & Không khí:** Hơi nước mịt mù bắn ra từ chân thác, ánh sáng núi cao trong vắt nhưng lạnh lùng.
- **Landmarks & Hình khối:** Bố cục blockout tham khảo khoảng 3 tầng thềm đá solid (TUNABLE, không khóa số tầng) ôm sát vách núi; thác nước đổ ầm vang ở trung tâm; hốc hang đá nơi Đoạt Mạch Đạo Tặc dựng lều trại cướp bóc; mỏm đá cụt nhìn ra biển mây bao la.

<a id="7-xích-nham-lv-1217--hẻm-sa-thạch-đỏ--mạch-ngầm-phong-ấn"></a>

#### Xích Nham (Lv 12–17) — Hẻm sa thạch đỏ & Mạch ngầm phong ấn
- **Environment Family:** Family 2 — Núi / vách đá / thác / xích nham (`Mountain`).
- **Tone & Mood:** Khô cằn, khắc nghiệt, nóng bức, báo hiệu trung tâm của sự biến động mạch đất.
- **Palette gợi ý — PROBE:** Sa thạch đỏ sắt (`iron red`), đất hoàng thổ (`ochre`), đá phiến tối màu (`dark slate`), đồng rỉ mờ (`muted copper`), bóng đổ lam bụi (`dusty cyan shadow`).
- **Ánh sáng & Không khí:** Không khí oi ả, bụi đá đỏ cuốn theo gió rít; khe nứt khoáng mạch ngầm phát ra ánh sáng ấm nóng kỳ dị (tuyệt đối không vẽ dung nham núi lửa).
- **Landmarks & Hình khối:** Hẻm núi sâu với bố cục blockout tham khảo khoảng 2 nhánh lớn hội tụ (TUNABLE, không khóa tổng nhánh); 3 trụ phong ấn đá cổ khắc hoa văn Mạch Ấn (`XN4_SealA`, `XN5_SealB`, `XN6_SealC`); đại môn Huyền Môn (`XN_HuyenMon_Outer`) sừng sững tựa vào vách núi nguyên khối ở cuối hẻm sâu.

<a id="8-huyền-tích-lv-1720--phế-tích-cấm-địa--world-boss-huyền-nham-cự-thú"></a>

#### Huyền Tích (Lv 17–20) — Phế tích cấm địa & World Boss Huyền Nham Cự Thú
- **Environment Family:** Family 3 — Phế tích cổ / trấn ấn / Huyền Môn (`Ancient`).
- **Tone & Mood:** Tối tăm, uy nghiêm, ngột ngạt, bí ẩn cổ sơ, tràn ngập cảm giác trận chiến cuối cùng của Chương III.
- **Palette gợi ý — PROBE:** Đá than đen (`charcoal stone`), xanh mực xám (`ink blue-gray`), ngọc bích cổ rêu phong (`muted jade`), đồng cổ phong hóa (`aged bronze`), tia sáng trọc khí tím ma mị được tiết chế (`restrained violet aura`).
- **Ánh sáng & Không khí:** Ánh sáng u tối, sương lạnh mờ ảo bao phủ các phiến đá nguyên khối; bầu không khí nặng trĩu áp lực tâm linh.
- **Landmarks & Hình khối:** Cổng đá đổ nát, dãy cột gãy khổng lồ phủ rêu, hành lang đá có Cổ Môn Vệ Binh canh gác; và **đại sảnh cấm điện trung tâm — BossCombatArea:** sàn đấu đá tảng nguyên khối khổng lồ, rộng rãi, phẳng phiu, sạch bóng quái thường, nơi World Boss Huyền Nham Cự Thú thức tỉnh uy dũng.

SafeAnchor và lối vào an toàn theo World owner và đường tới exit phải có mặt đứng/đường đọc được; không đặt quái/props che chỗ hồi phục. Kích thước root theo bố cục thật, không nhân background bằng offset 200 u. World graph giữ kết nối design owner; đường bên trong có nhánh trên/dưới, loop, ngách cụt, ledge/hollow và jump/drop vừa đủ. Exit có thể ở một nhánh, không buộc cuối bên phải. EdgeExit dùng vùng thoát có hướng/tên đích và reason khóa, không vòm portal/Interact cho mọi lối;

SpecialGate chỉ Huyền Môn/Arena hoặc cửa gameplay đặc biệt. Map asset tồn tại không tự mở quyền vào: gate dùng level/unlock/quest **Completed** theo design owner/Technical.

**Mật độ visual và kế hoạch authoring hiện hành — STRONG DIRECTION:**
Tăng số bãi/cụm độc lập (`pockets`), giữ dải vào an toàn (safe strip theo World owner) tại cửa map, tuyệt đối tránh dồn một group thành blob 8–10 quái.
- **Thị giác trên một camera:** Khung hình camera tiêu chuẩn bao quát được 5–8+ quái trải trên nhiều thềm/tầng (ví dụ: tầng dưới 1–2 quái, tầng giữa 1–3 quái, tầng trên 1–2 quái, nhánh phụ 1–2 quái), mang lại cảm giác thế giới online hoang dã, đông đúc.
- **Cách ly hành vi:** Mỗi cụm là một `SpawnGroup` độc lập với `HomeRegion` và `WalkRegion` riêng; các cụm nằm cạnh nhau trên màn hình nhưng không chain aggro (không kích hoạt dây chuyền khi đánh một cụm).
- **Authoring source:** [World candidate manifest](../01-design/world-and-content.md) sở hữu ranges, stable IDs và quest anchors.
- **Boss Exclusion Rule:** Khu vực giao chiến Boss (`BossCombatArea`) tại trung tâm Huyền Tích cấm tuyệt đối việc sinh hoặc tuần tra của quái thường, tạo sàn đấu tập trung, sạch sẽ cho trận đánh đỉnh cao.
- **Dữ liệu lịch sử:** Bảng 28 cụm / 66 slots cũ là **LEGACY seed** để giữ các mốc neo nhiệm vụ (`DS2`, `DS3–DS6`, `TA4`, `TA6`, `XN1–XN6`, `HT4–HT5`, `HT_BossLandmark`) cùng các stable seed IDs (`TA5`, `BV1–BV5`, v.v.) và trace prototype; tổng số quái và số cụm cuối cùng còn **OPEN / TUNABLE**.

<a id="12-terrain-readability-và-tile-variants"></a>

## Terrain readability và tile variants

**Luật gameplay ở [World & Content — terrain](../01-design/world-and-content.md#terrain-rules); Art làm rõ hình đọc được.**
- **Natural terrain = SOLID MASS (Khối đặc dày):** Đất/đá tự nhiên luôn luôn là khối chắn đặc có độ dày thực tế: mặt trên nằm ngang, khối vật liệu lấp đầy bên trong, mặt đứng thẳng góc, đáy/bóng đổ và các mép/góc khép kín. Các bậc ghép thành một khối địa chất liền mạch; hốc/hang/khe nứt/mỏm đá không bao giờ biến núi đồi thành dải đất tự nhiên mỏng manh lơ lửng.
- **Không dốc chơi được (`no playable slope/ramp`):** Tuyệt đối không có mặt dốc nghiêng để nhân vật chạy lên/xuống, không có địa hình tam giác, không có collider xoay góc. Mái nhà, cành cây hoặc núi xa ở lớp nền có thể vẽ chéo cho mềm mại, nhưng toàn bộ mặt tiếp xúc gameplay vẫn phải là các bậc ngang/đứng trực giao.
- **Đất đá tự nhiên KHÔNG BAO GIỜ là one-way platform:** Nền đất, đá tảng, gờ núi không bao giờ cho phép nhảy xuyên từ dưới lên hoặc xuyên xuống (`no natural one-way`).
- **One-way CHỈ dành cho kết cấu nhân tạo đặc biệt:** Chỉ các cấu trúc mỏng nhẹ hợp lý như ván gỗ, giàn tre/catwalk, ban công, sàn treo tựa vách có dầm đỡ/dây treo rõ ràng mới được làm sàn one-way.
- **Không leo trèo (`no ladder/rope/vine/climb`):** Không có thang dây, dây leo, cột đu hay cơ chế bám tường leo trèo; toàn bộ di chuyển dọc dựa vào nhảy (`Jump`) và rơi (`Fall/DropThrough`).

### Bốn grammar địa hình/cảnh — cách ghép khối và tuyến chơi, có thể trộn trong một map

| Grammar | Hình khối và tuyến chơi | Giới hạn sản xuất/đọc hình |
| --- | --- | --- |
| Đồi tự nhiên có bậc | Khối đất/đá solid nối liền, thềm và jump giữa bậc; nhiều bãi độc lập | Mặt đứng/vật liệu khép; không dải đất mỏng nổi giả đồi |
| Khối đá/hốc/khe dọc | Một khối lớn có hốc/khe/mỏm/ngách; tuyến đi nhìn thấy trong khối | Không tam giác/ramp để giả núi; nền và mặt đứng tách rõ |
| Công trình/cầu | Sàn/cột/dầm/tường/mái tái dùng, tuyến trên/dưới và surface được author | Collision là vài mặt sạch trực giao, không polygon theo mọi chi tiết |
| One-way đặc biệt | Ván/giàn/lối ván cao/ban công nhẹ/sàn tạm treo hoặc tựa vách | Hiếm, mỏng, khoảng trống dưới và dây/dầm/cột/giá đỡ rõ; không đất/đá tự nhiên |

Ảnh AI hoặc concept reference chỉ giúp trao đổi hình, **không là authority** cho collider, route hay grammar. Text spec và design owner quyết định.

| Ngữ nghĩa | Cue khi nhìn hình | Collider / kiểm |
| --- | --- | --- |
| Solid ground | Mặt trên cap liên tục, khối fill có trọng lượng, edge đậm hơn background | Solid, không đổi một tile giống hệt thành pass-through tùy chỗ |
| Cliff/step | Mép/corner khép, mặt đứng tách mặt trên; cao độ theo block rõ | Step thật, không trang trí gờ nhỏ khiến tưởng đứng được |
| One-way/platform | Kết cấu mỏng có khoảng trống dưới và support rõ; khác đất/đá solid | Chỉ structural one-way, DropThrough riêng actor; hint lấy glyph binding đang thử |
| Pass-through prop | Ít contrast/outline khép, chân không nối solid cap; texture nền | Không collider; tránh rock foreground đậm cùng hình rock solid |
| Overpass/cầu | Sàn trên rõ, khoảng đi dưới có chiều cao và route hai đầu; cột không giả blocked tunnel | Có surface trên + đường dưới nếu author; không cần hệ depth lane mới |
| Background | Saturation/contrast thấp, silhouette mềm, không cap standable | Không physics; rìa background không trùng rìa ground |
| Foreground | Crop/occlusion viền, ít chi tiết vùng action; không làm giả platform | Không collider; hở silhouette actor/telegraph/loot |

Một vật liệu có thể xuất hiện ở solid và background **nếu dấu mặt trên/cạnh/contrast và chân nối nền khác rõ**; giảm opacity 5% thường chưa đủ. Vật trông như cầu thang/sàn phải chơi đúng hình hoặc đổi hình đủ rõ để không lừa player. Kiểm grayscale, nền sáng/tối và lúc Freeze/Burn/Linh aura bật; không phân collider bằng màu đơn thuần. **Không ladder, rope/vine/pole/wall climb, Climb action/state/animation**; movement vẫn Move/Jump/Fall/DropThrough. Cầu thang dùng block steps trực giao hoặc decor nhìn rõ, không animation leo riêng.

**Kịch bản Terrain Kit khởi điểm, OPEN A09:** ô 32×32 ở PPU32 =1 u; platform có thể sprite trong ô nhưng mặt ván chỉ 8–12 px. Player 64 px không suy ra tile 64 px. 32 là probe để người cao 44–48 px đứng trên ledge dễ đọc; 16 px chi tiết hơn nhưng tăng placement/corners, 64 px coarse với các bước nhỏ. Chọn sau room test, không đổi PPU theo family.

| Tập module topology | Số hình trong kịch bản | Vì sao cần / nếu cắt |
| --- | ---: | --- |
| Solid3×3: fill 1, edges4, outer corners4 | 9 | Đủ khối chữ nhật liên tục; thiếu corner làm mép không khép |
| Inner corners | 4 | Hốc/routes concave; nếu map không có hốc có thể hoãn, không vẽ rồi không dùng |
| Isolated block | 1 | Block độc lập đọc được; nếu tất cả nền ≥2 ô dày có thể không cần |
| One-way kết cấu left/mid/right | 3 | Hai mép + lặp ván/sàn có support; không ba tile đất one-way. Mirror có thể giảm ảnh |
| Cosmetic alternatives: top 2, fill 2 | 4 | Bớt pattern lặp trên đường dài; không đổi collision meaning |
| **Tổng/họ chất liệu** | **17 semantic +4 cosmetic=21** | Ba family tối đa 63 hình terrain nếu vẽ từng họ; cùng topology/data, chưa cam kết mỗi họ cần đủ variant |

17+4 là kịch bản tối đa của topology và kết cấu được chọn, không yêu cầu mỗi material có đủ ba one-way tiles; chúng có thể dùng mini kit chung. Không gọi đây là autotile47: cầu thang dùng block, slope/climb bị loại, moving platform/hazard ngoài P0. Cột mỏng/mặt trên đặc biệt hoặc asset lớn chỉ thêm module khi room test chứng minh cần; ghi delta công thật. Variant trang trí không tạo ngữ nghĩa collider mới.

<a id="13-buildingstructural-là-không-gian-gameplay"></a>

## Building/structural là không gian gameplay

**Đọc từ:** bridge/vertical route Trúc, terraces Bạch, ruin/Boss Huyền, AI hybrid/flying. Công trình đứng được là dependency physics/combat, không chỉ decoration.

| Cách dựng | Dùng khi | Trade-off / recommendation |
| --- | --- | --- |
| Terrain Kit | Bậc đá, nền/ruin tường đơn giản dùng cùng chất liệu | Rẻ và collider nhất quán; motif kiến trúc bị phẳng nếu ép mọi mái/cổng thành tile |
| Mini Building Kit | Cầu/mái/dầm/cột hoặc vòm lặp ở nhiều vị trí | Một bộ nhỏ cap/mid/support theo grid; đủ reuse, không hệ building procedural/placement của player |
| Full Asset | Landmark độc nhất, lò rèn, Huyền Môn, phế tích silhouette lớn | Vẽ đẹp theo layout; vẫn chia back/front và surfaces, không collider theo từng chi tiết ảnh |

Kit tái dùng cột/support, dầm, sàn, cầu, ban công, tường, mái, vòm/cổng và block step; không procedural building hoặc công trình độc nhất cho mọi map. Mái nhà có route chỉ khi đã author mặt ngang/bậc nối bằng jump hiện hành; không mái dốc đứng được. Phần dưới là prop/pass-through có cue. Phế tích lớn có nhiều sàn/cầu/cổng, spawn và combat cần **bảng mặt đứng (`surface map`)** với ID/cao độ/solid hay one-way/route nối/spawn/HomeRegion/WalkRegion/leash và đường ngắm Cung. Full asset không phải một polygon collider bám toàn cửa sổ/gạch/cây leo.

Collider đề xuất: vài rectangle/edge được làm sạch trên physics root, solid nền + one-way sàn riêng; trang trí khung cửa/cột/vòm sau/trước không collider nếu route không cần. Giữ landing lip khớp mặt pixel, loại khe collider làm chân mắc.

Ground tile tiếp tục Composite Operation Merge; one-way dùng PlatformEffector2D và drop-through theo actor hiện hành. [Nguồn Unity — TilemapCollider2D](https://docs.unity3d.com/6000.3/Documentation/Manual/tilemaps/work-with-tilemaps/tilemap-collider-2d-reference.html), [PlatformEffector2D](https://docs.unity3d.com/6000.3/Documentation/Manual/2d-physics/effectors/platform-effector-2d-reference.html).

Ground mob chỉ đi trong HomeRegion/WalkRegion/SurfaceId đã author. Mép không có nền nối thì quay đầu, không tự rơi/jump/drop; hai terrace chỉ nối AI nếu có đường đất trực giao liên tục thật. Route player phải jump/drop không tự là route mob. Passive aggro ưu tiên ngữ cảnh local tới được; hostile hit ngoài aggro vẫn wake/threat, chỉ báo động cùng SpawnGroup. Không navigation graph/DropLink hoặc ranged fallback chung để cứu bố cục.

Hybrid roster, LoS A/B, grace/Return/regen/invulnerability/targetability vẫn OPEN/TUNABLE; Art phải cho người chơi đọc Returning reason, không tự khóa immune.

Mini kit giả định gỗ gồm bridge-floor3 +roof3 +support2 +wall/arch2 =**10 logical modules**, **8 hình** nếu hai cặp cap trái/phải thật sự mirror được. Nếu motif không đối xứng thì 10 hình. Đây là ví dụ derivation, không bắt phải có đủ roof trên mọi map; stone kit có thể dùng terrain hoặc một bộ cap/mid/support riêng sau route test. Landmark độc nhất không ép thành kit mười loại chỉ để “modular”.

Test một công trình: lower lane và upper standable surface, một spawn đúng platform, đường tiếp cận Kiếm, so LoS A/B qua solid roof với cue đọc được, mob Return không mắc cột, người drop-through không làm người khác rơi. Khoảng trống Boss15 u TEST và telegraph tránh bị mặt sàn foreground che.

<a id="mock-map-ui"></a>

Các sơ đồ, tọa độ và capture của bản mẫu được quản lý riêng tại [Roadmap — hồ sơ prototype](../90-archive/production-history.md#prototype-map-history).

<a id="14-animated-environment"></a>

## Animated environment

**Đọc từ:** Bạch waterfall, map family/foreground và scope không swimming/hazard. Nước P0 là presentation trên mặt đi được khi không có gameplay requirement riêng.

| Thành phần | Cách đề xuất | Frame/primitive khởi điểm và reasoning |
| --- | --- | --- |
| Waterfall top | Sprite loop riêng ở mép nguồn | 4 hình crest đổi hướng/nhịp; hai hình dễ nhấp nháy, hơn bốn chỉ thêm mượt |
| Waterfall fall | Animated tile/strip repeat dọc, phase ổn định | 4 hình flow streak; không vẽ toàn thác cao mỗi frame. Test seam và không làm pattern như climbable ladder |
| Waterfall splash | Sprite loop ở chân + hạt thưa nếu cần | 4 hình mở/đổi/tan/trở lại; reuse ripple motif, không emitter dày che terrain |
| Stream/water | Static base + flow overlay hoặc animated tile 4 | Base giữ palette/mặt nền; loop 4 chỉ cho phần có dòng thấy rõ, puddle tĩnh không cần cùng loop |
| Puddle | **WaterBase + FrontOverlay**, base static 1, front static 1 | Base sau chân, front chỉ che phần chân thấp. Hai lớp không hai vật lý nước; ít hơn một lớp mất cảm giác chân trong nước |
| Actor ripple/splash | Một strip3 hoặc 4 hình spawn theo bước/contact presentation | Một prefab chung; ba hình mở/lan/tan, thêm một cho decay. Có throttle local/remote theo camera, không mọi network transform tick |
| Lá rơi | ParticleSystem với 1–2 leaf sprites | Random vị trí/lifetime, tránh giả từng tile có animation; hai silhouette bớt lặp, nhiều lá không thêm gameplay |
| Khói | ParticleSystem 1 soft/pixel puff, hoặc sprite loop 4 cho nguồn rõ | Forge/fire reuse puff, ít emitter. Không lighting framework |
| Bụi | ParticleSystem 1 puff hoặc ripple/impact strip reuse | Chân/mob nặng/đổ dùng chung texture; cắt ở crowd trước core feedback |
| Lửa/ánh sáng | Sprite flame loop 4 + static emissive-looking accent | Reuse flame với Burn có chỉnh scale/tint; không buộc Dynamic Light/URP2D lighting nếu chưa cần |

Thác ba strip×4 =**12 ô chuyển động** theo kịch bản, không chiều cao thác×12. Top/fall/splash không nhất thiết cùng hình dù dùng cùng clock; fall đặt sau actor nếu route phía trước, không làm lớp nước opaque ngang mặt. Stream 4 và ripple 3–4 chỉ thêm nếu layout có chức năng visual đó. Foreground nước đứng yên vẫn có thể đẹp bằng base/accent, không phải mọi puddle đều loop.

Contact ripple chỉ cosmetic; tốc độ lội nước theo feet contact của design owner, không thêm CC/Burn/HP hazard; pooling reset phase/scale/owner. Actor đi trên cầu phía trên nước không splash vì chỉ chồng hình 2D; cần surface/contact tag presentation đúng lane. P0 không deep-water, breath, buoyancy/swimming. Nếu thác là background, không vẽ ledge giả hoặc trigger tương tác không có luật.

<a id="15-map--mob--vũ-khí"></a>

## Map ↔ mob ↔ vũ khí

| Ca gameplay hiện có | Failure mode visual/route | Hợp đồng authoring và validation |
| --- | --- | --- |
| Spawn platform/công trình | Chân ở giữa không khí, slot trên sàn không tới được | Spawn anchor trên surface hợp lệ, clearance đủ body/HP bar, hurtbox không xuyên sàn; identity/level giữ manifest |
| Chase/Return | Mob rơi tầng dưới rồi teleport lên, hoặc mắc dầm/cột | Chân giữ WalkRegion, nền nối thật và home/leash đã author; leash 8 u là TUNABLE. Return đọc được, không dùng đường ảnh giả |
| Melee Kiếm | Cao độ khiến range 1,2/1,7 u không chạm, ngay cả nhìn gần | Kiểm primary eligibility/vertical profile theo Combat, originY+0,8 u giữ baseline; trail không sửa eligibility |
| Cung/ranged hybrid | Visual xuyên roof có thể trái LoS mode của prototype | A không LoS so B SolidWall, one-way không chặn trong B; chưa production lock. Visual chỉ đọc logical result, không collision damage |
| Flying Ong | Hover box6×3 u khiến ở cao ngoài tầm Kiếm vô hạn | Engage approach vào melee-accessible band như design owner; room có route/jump thật, không yêu cầu Kiếm có skill mới để tới |
| Multi-floor | Target distance gần nhưng khác tầng, target lock/hit xuyên đá | Authority MapId/primary/secondary range/vertical profile validation; foreground và platforms đọc được khoảng cách thực |
| Linh scale 1,20–1,30 | Sprite overlap roof, người hiểu hurtbox lớn hơn trong khi physics không scale | Aura/name/HP bar chính, scale vừa phải theo clearance. Nếu scale mờ pixel, thử tint/accent trước đổi physics |
| Nhiều pocket cùng camera | Cầu nối làm báo động lan cả map hoặc leash cắt route | Kiểm SpawnGroup/HomeRegion/WalkRegion độc lập, route cao/thấp và lối an toàn; 18–20 u cũ không là luật mật độ mới |
| Loot sau flying death | Món nằm trên ledge kín hoặc trên không | Server-owned điểm pickup đứng được; collider route/1,5 u pickup test cùng lifecycle các mục liên quan |

Không phải mọi mob đi mọi tầng công trình: author phạm vi và thể hiện bằng chân/spawn/route. Hybrid count/identity còn OPEN, không tự thêm ranged cho mọi loài. Mọi encounter bắt buộc quest phải có cách Kiếm/Cung tiếp cận, đánh và nhặt bằng mechanic hiện hành. Cung liên tục kite trên route hợp lệ có thể no-hit pure melee; safe perch đứng spam mãi là lỗi geometry cần sửa map và Return đơn giản, không thêm đòn chống Cung cho Sói. Không dùng effect đẹp che softlock hoặc thêm body blocking/knockback/hazard/moving platform/navigation framework.

<a id="npc-visual"></a>

<a id="16-bảy-npc-và-các-khu-chức-năng"></a>

## Bảy NPC và các khu chức năng

**Owner quest/service:** [Quests & Narrative](../01-design/quests-and-narrative.md#quests-story) và [NPC roster](../01-design/quests-and-narrative.md#npc-roster). Art làm rõ nghề, khu vực và đường tìm NPC; không giữ bảng luật quest thứ hai. Dùng sprite toàn thân NPC với pose template/palette/props chung trong source; NPC không thay outfit thì không cần module gear runtime. Chức năng phải nhận ra bằng dáng bao, props, biển và text; không xếp NPC thành một hàng như menu.

| NPC / khu đặt — ý đồ, tọa độ OPEN | Idle khởi điểm | Props/gesture và chức năng cần đọc |
| --- | --- | --- |
| Lâm Bá — khu công cộng dễ thấy ở Vân Khê | 2 hình thở nhẹ | Ghế/gậy hoặc bia/biển nhỏ dùng motif sẵn; hub truyện, giới thiệu Q6 và diễn tiến trấn ấn/Huyền Môn |
| Yên Thảo — khu dược/thảo mộc/thuốc | 2 | Bàn thuốc/giỏ/bình; Food/HP Potion/MP Potion và healing theo luật thường. Gesture đưa/chỉ thuốc +2 hình chỉ nếu giúp đọc tương tác |
| Bách Luyện — lò rèn/đe | 2 | Đe/búa/lò; Q3 vũ khí, Q4 bán đồ, Q7 enhance và gear/transfer. Work loop +4 nâng→đập→hồi→nghỉ là option ưu tiên nếu lò dễ thấy |
| Mộc An — nhà kho/nghỉ | 2 | Rương/ghế/nhà trọ dùng kit chung; Storage40/nghỉ và utility Hồi Sinh Phù/Tẩy Mạch; không sleeping/heal rig riêng |
| Phong Du — khu Kiếm ở Học Viện | 2 | Kiếm/giá binh khí, stance Kiếm; mentor/giao dịch nhập phái và trả Q6 Kiếm, không ôm Q3 |
| Diệp Lam — khu Cung ở Học Viện | 2 | Cung/giá binh khí, stance Cung; mentor/giao dịch nhập phái và trả Q6 Cung, đọc ngang hàng với Phong Du |
| Hạo Vũ — gần biển/lối sang Lôi Đài | 2 | Cờ/biển tỷ thí và motif Arena; Q9/PvP, không bộ đánh nhau NPC |

**SUY RA hiện hành có điều kiện:** 7 NPC×2 idle = **14 hình** nếu mỗi silhouette khác; work Bách+4 và gesture Yên+2 tùy chọn → **20**. Đây là kịch bản **14–20**, không 7×26, không cam kết frame count. Idle 2 ở 2–4 FPS/hold đủ nhịp thở; một hình rẻ nhưng hub tĩnh, bốn hình tăng công mà không thêm dịch vụ. Props tính riêng theo motif thật; không cần bảy portraits hoặc combat/death rig. Khi modal enhance đang chờ ACK, work loop không được giả kết quả thành công.

**Tạ Minh — LEGACY/SUPERSEDED:** roster cũ có 8 NPC, 16 idle và optional+6 →22; giữ số này để đối chiếu lịch sử. Không sản xuất NPC thứ tám hoặc NPC thay thế. Motif bàn/ấn cũ có thể chuyển thành prop của khu Lâm Bá/Huyền Môn; class cue thuộc hai mentor, Tẩy Mạch thuộc Mộc An. Stable internal IDs hiện có chỉ migrate references theo Technical; không mass-rename identifier vì display roster thay đổi.

Q1 phải dẫn player qua khu dược → lò rèn → kho/nghỉ và đọc lối ra/về, không chỉ click ba người cạnh nhau. Q2 theo [mini journey proposal](../01-design/quests-and-narrative.md#q2-journey), destination/return rõ, không ép Drop; Q3 nhận Mộc Kiếm tại Bách Luyện, tới Dummy Yard rồi trả Bách Luyện. Q6 từ Lâm Bá tới Học Viện, player nói chuyện cả hai mentor, tự tháo Mộc Kiếm, chọn một mentor rồi nhận/learn/equip/cast/dùng MP Potion và trả đúng mentor đó. Hai khu mentor và yard phải nhìn ra route, không dàn Kiếm như default trước Cung. Q10/Q12 về Lâm Bá; Q11 Lâm Bá→Bách Luyện restoration theo direction, endpoint OPEN; không thêm NPC mới.

Hội thoại nhận 1–3 câu, phản hồi ngắn ở bước giữa và một câu trả; tracker nêu **việc → khu vực/đường đi → NPC tiếp theo**. Text có giọng riêng vừa đủ, giữ fantasy Việt nhưng không lặp “ngươi/bổn tọa/linh căn” mọi câu. Player không thấy QuestId/counter nội bộ hay reward dạng debug. Marker/quest/service menu đọc committed state; UI không tự hoàn quest vì đã phát thoại. Exact NPC coordinates và số props chốt sau blockout, không bịa pixel tọa độ.

<a id="17-vfx-ngoài-active-skill"></a>

## VFX ngoài active skill

**Impact** = ngắn tại hit; **Status** = tồn tại theo gameplay state/expiry. Một hit băng không đồng nghĩa target Đóng Băng. Reuse tint không được xóa khác biệt silhouette/lifetime giữa Freeze và Slow.

| Feedback | Presentation tối thiểu | Reuse / giới hạn |
| --- | --- | --- |
| Bỏng | Flame/viền ấm nhỏ dưới thân hoặc cạnh target; tick feedback nhỏ mỗi 1 s | Một instance/target; loop 4 dùng lại flame môi trường khi hợp palette. Refresh không phát lại explosion |
| Freeze normal/Linh | Ice silhouette ôm target, trạng thái đứng yên rõ; crack/thaw khi hết | Probe2 static shapes shell/crack +strip3 thaw; scale theo visual bounds nhưng không collider. Không che aura/name. Shell ít hơn vẫn test được; nhiều shard chủ yếu polish |
| Slow Boss/PvP | Lam mờ/1 cold particle motif, icon/expiry khi cần | Không ice-block; không slow current cast animation; một overlay refresh deadline |
| Linh Biến aura | Vòng/ấn tím bên chân/nameplate, nhẹ và bền | Một sigil sprite +rotation/pulse presentation; cùng mọi rig, khác palette Sói. Non-integer scale phải test pixel fidelity |
| Boss Cuồng Mạch | Accent nóng/viền, âm/roar tùy chọn; giữ telegraph | Reuse aura primitive có palette/shape cue phù hợp, không overlay tím làm nhầm Linh |
| Heal | Hạt/nhịp ấm xanh, HP number/bar đổi sau xác nhận | Một puff/spark primitive, không animation cơ thể riêng; Food ticks không emit full burst2 s/lần |
| MP restore | Hạt lạnh/lam, MP feedback riêng | Reuse heal motion/texture, khác glyph/palette/vị trí; chỉ sau server realtime acceptance của Potion |
| Food/buff | Icon item Food đang active +duration; entry feedback ngắn khi use thành công | Không aura liên tục, không thêm buff R P0. Three foods reuse HUD binding |
| Enhance success | Sigil/glint quanh item preview, kết quả cấp/stat rõ | ACK rồi play; cùng mọi item/+level. Một glint primitive, không scene riêng |
| Enhance fail | Crack/dim ngắn trên panel, thông báo giữ cấp và chi phí đã tiêu | Không vỡ item sprite/giảm cấp; network error khác RNG fail, pending không giả kết quả |
| Transfer | Arrow/flow trong UI, success cùng glint sau commit | Không item chuyển giao mới; source mất thể hiện bằng preview/result, không effect tự consume |
| Death/Revive | Player [pop/fall→shadow](#player-shadow-death), mob Death/corpse riêng; hồi sinh tại chỗ nhịp sáng/viền miễn thương2 s đúng server | Shadow/eyes chung player, không áp fade mob; invulnerability cue reuse Hồi Sinh motif, không tự kéo dài. PvPDefeated riêng flow |
| Loot/Gold | Shared item icon/pile +prompt quyền nhặt; Gold coin burst chỉ cho recipient đã ACK | Một glint/beam rỗng tiết chế; không coin vật lý pickup khi Gold auto-credit, không beam giả personal loot |
| Crit | Font weight/scale/glyph khác normal, impact accent nhỏ | Reuse damage renderer; không tăng hit-stop mọi remote crit |
| NÉ | Text/glyph rõ, không wound impact hoặc proc overlay | Có thể vẫn thấy đường đòn đi qua; damage0, không flash target như đã landed |

Ưu tiên khi hình chồng nhau: Boss telegraph/đường đòn nguy hiểm → dáng actor/action → hit/status local → loot/UI → cosmetic remote → môi trường. Hồi phục/status không phủ mảng đặc che Boss marker. Server terminal thì clear Freeze; aura Linh chỉ còn tan cosmetic nếu không làm corpse giống mob sống. Pool reset màu/material/owner/expiry/action/life IDs để Bỏng cũ không gắn vào respawn.

<a id="icons-ui"></a>

<a id="18-items-và-icons"></a>

## Items và icons

**Đọc từ:** catalog/source/manual owner design tương ứng. Đếm **binding/icon outcome**, rồi tách motif vẽ mới; không nhân rarity×enhancement×template.

| Nhóm P0 | Nhu cầu binding / căn cứ | Cách author |
| --- | --- | --- |
| Weapon | 7: Mộc Kiếm +3 Kiếm +3 Cung | Bảy silhouette đúng visual world, không dùng icon kiếm Common cho tất cả. Có thể dựng từ crop canonical rồi clean để đọc ở nhỏ |
| Armor / LowerBody | 3+3 | Ba band mỗi loại, crop/motif +accent; icon pants có footwear ngụ ý, không phải icon Boots |
| Boots/Ring/Necklace | 3+3+3 | Ba motif slot, palette/accent band; vẫn cần nhận biết bậc khi không đọc tên |
| Food | 3 | Bánh/thịt/cơm khác silhouette, không chỉ màu |
| HP/MP Potion | 6=2 loại×3 tier | Hai hình bình/motif, nhãn/tier I/II/III và palette; tránh màu là cue duy nhất |
| Phù P0 | 2: Hồi Sinh, Tẩy Mạch | Hai motif/ấn khác công dụng; Bùa Hồi Thành P1 không tính |
| Tinh Thạch | 1, không tier | Một đá rõ, không ba đá giả tạo vì ba bands |
| Trade materials | 8: Nấm Sương, Nanh Sói, Trúc Tâm, Cánh Ong, Vân Thạch, Huy Hiệu Đoạt Mạch, Khoáng Xích Nham, Mảnh Cổ Ấn | Tám icon đọc công dụng/name, reuse đá/ấn motif cho nhóm gần nhau; không random thêm loại |
| Thỏi Vàng | 1 | Khác coin currency vì phải pickup+bán, không auto-credit |
| Manuals | 6=2 class×3 milestones | Hai sách motif +ba accent/ordinal, cần output nhận biết nhập môn/tiến cảnh/chân quyết |
| Currency | 1 Vàng | Một coin glyph dùng HUD/shop/cost, không thêm tiền linh lực |
| Active profiles | 6=Kiếm/Cung×Lv 5/10/17 | Hai motif class với độ mở/rune khác theo milestone; Lv 5/10/17 là ba SkillIds/slot riêng, reuse motif icon nếu vẫn phân biệt được |
| Passives | 4 | Kiếm Tâm/Kiếm Thế/Ưng Nhãn/Xạ Tâm cần cue khiên/gần/mắt/xa; derive hai motif class, không thêm tree/rank |
| Status HUD | 4 loại presentation: Burn/Freeze/Slow/Food | Ba glyph status mới; Food dùng lại icon Food đang active. Freeze/Slow không chung một hình ice-block |
| Physical quest collection | Dấu Trọc Khí, Vật Chứng, quest material Q11 tên/ID OPEN, restored fragments1/2/3: candidate6 ItemDefinition bindings; Q2 placed item thêm1 chỉ nếu duyệt | Icon Inventory/ground thật, reuse sigil/ấn/glyph+ordinal/binding label; exact IDs/mapping PROPOSAL, không virtual feedback thay item |

**SUY RA có điều kiện:** base catalog trước collection có 49 physical bindings; Gold 1/active 6/passive 4/status 4 cho 64 base bindings, Food status reuse. Physical collection candidate6 definitions→55 physical/70 base bindings; Q2 nếu duyệt thêm1→56/71. Exact ItemIds/icon mapping/schema còn PROPOSAL, không khóa tổng bitmap mới. Tái dùng motif với label/ordinal/binding overlay có thể giảm ảnh vẽ; không dùng 49/64/63 cũ như full current catalog. Rarity/+n không nhân bitmap. Revive/Linh/Boss badges chỉ thêm khi sample cần và ghi delta.

Nguồn icon dự kiến32 × 32 transparent, hiển thị scale nguyên và tooltip lớn khi cần, **OPEN** theo HUD/reference resolution. Không lấy PPU world để quyết pixel UI. Inventory 60/Storage 40 dùng cùng slot primitive; stack count/+level/rarity/locked/quest-bound hiển thị text/glyph, không bake vào từng PNG. Icon màu band không được giống rarity border đến mức nhầm band III=Epic.

<a id="19-common-ui-kit-và-các-view"></a>

## Common UI Kit và các view

**Đọc từ:** owner design tương ứng, owner kỹ thuật tương ứng. Inventory/Storage/Shop là bố cục và binding dùng chung, không ba bitmap screen. Chọn một style panel/outline/spacing/text; kit không đòi mỗi trạng thái một ảnh mới.

| Primitive / số thứ tự để đếm kit | Công dụng | States cần có; reuse |
| --- | --- | --- |
| 1 Panel/frame | HUD/popup/đối thoại | Nine-slice một style nền/frame; kích thước do layout |
| 2 Button | Confirm/use/cancel | Normal/hover/pressed/disabled; tint/border/offset, không bốn ảnh bắt buộc |
| 3 Item slot | Bag/storage/shop | Empty/filled/selected/unavailable/pending; icon +overlays |
| 4 Equipment slot | Character/equip picker | Reuse3 +slot glyph; empty/filled/off-class/level-lock |
| 5 Skill slot | HUD/panel | Thể hiện rõ: Slot 1/2/3 được chọn (`SelectedSkillSlot`), hồi chiêu (`Cooldown`), chưa học (`ManualMissing`), và phân biệt rõ với thao tác thực thi (`PrimaryAction` (combat branch)) |
| 6 Tabs | Bag categories/NPC menus | Normal/hover/selected/disabled; button derivative |
| 7 Tooltip | Item/skill/reason | Panel1 +title/stat rows/cost/rarity/lock; compare layout reused |
| 8 List row | Quest/Journey/character/opponents | Normal/hover/selected/disabled; text/icon/progress |
| 9 Scrollbar | Lists/storage | Track/thumb/button optional, states drag/disabled |
| 10 Divider | Sections | Một repeatable stroke, không vẽ mỗi panel |
| 11 Input | Login/chat/wager selector khi hợp flow | Normal/focused/disabled/error; caret/text/masking native |
| 12 Modal | Confirm/death/invite | Panel+buttons+focus blocker; pending/success/fail message |
| 13 Toast/banner | Bag full/reject/system | Panel style nhỏ +result glyph; queue/read duration |
| 14 Lock | Skill/level/equip restrictions | Một glyph, reason text; locked khác disabled |
| 15 Selection/highlight | Focus/cursor slot | Một outline/glow nhẹ; không opacity che icon |
| 16 Rarity frame | Common/Uncommon/Rare/Epic | Một border shape +palette/text, không item ảnh mới |
| 17 Progress bar | HP/MP/EXP/quest/timer | Shared track/fill/mask, label; HP/MP khác cả label/palette |
| 18 Cost/currency row | Shop/enhance/transfer/PvP | Gold icon/stone icon +amount, sufficient/insufficient/pending |
| 19 Icon button | Close/interact/page/back | Button2 +glyph, normal/hover/pressed/disabled |
| 20 Cooldown/pending mask | Skill/Potion và mutation chờ | Fill/radial/text dùng lại; cooldown deadline khác request đang chờ |
| 21 State/result badge | Success/fail/error/available/ready | Glyph/text +palette; không chỉ xanh/đỏ |

**21 primitive/chức năng** không bằng21 unique textures: equip/skill/tab/modal/tooltip/toast compose từ panel/button/slot; glyph/typography/layout là workload riêng. Nine-slicing giữ góc khi resize, phù hợp panel reuse; đề xuất viền pixel scale nguyên/tile phần lặp, cần kiểm ở UI pipeline thực chọn. [Nguồn Unity — 9-slicing](https://docs.unity3d.com/6000.3/Documentation/Manual/sprite/9-slice/9-slicing.html).

| View P0 | Visual riêng thực sự cần | Phần compose/reuse |
| --- | --- | --- |
| Character | Sáu ô + preview, Thuộc tính và Thông số tách chức năng | Kit/glyph trang bị, dùng rig hiện tại; không portrait bitmap mỗi outfit |
| Enhance | Vùng preview trước/sau, hai mốc Tinh Hoa khóa/mở, success/fail accent | Item icons/cost/result kit; không lò rèn fullscreen |
| Transfer | Hai item/source-consumed/target-before-after và flow arrow | Kit/arrow glyph; không art mới từng pair item |
| Skill | Ba active, hai passive; selected slot/CD riêng và manual/level lock lý do | Mười profile/passive icons các mục liên quan, rows/tooltips; không talent tree |
| Quest | State/group/count/return NPC/level gate; Q9 optional tách pin | Rows/markers; collection đọc item possession/pending ground, không UIcounter giả; không bắt drag-drop để turn-in |
| Journey | Score và category/record tổng hợp đúng hệ thống hiện có | List/progress/title; optional emblem dùng motif Mạch Ấn |
| PvP | Invite/opponent picker, exact wager/pot/fee, pending escrow, countdown/120 s/quota, result/pending settle | Two columns +badges/countdown numbers; không portrait/arena splash bắt buộc |
| Login/Character Select | Logo/title giản dị, list và selected preview, feedback kết nối/lỗi | Kit và rig; dependency các mục liên quan |
| Stage Summary | Ba title/chương, reward/progression/story completion text | Một modal template, ba accent/thumb crop map sẵn; không ba tranh full-screen bắt buộc |
| Inventory/Storage/Shop/Sell | Grid/list, capacity60/40, stack count/ownership, price/stock state | Cùng kit; Storage không item background riêng |
| Class choice/manual use/attributes reset | Hai mentor/choice cues ngang hàng, reason/preview/confirm | Hai motif class/kit; học sách từ bag, Tẩy Mạch tại Mộc An |
| Dialogue/Rest/Death | Text/action NPC, confirm nghỉ/death choices khác PvP | Panel/buttons; icon Hồi Sinh, không bảy portraits bắt buộc |
| HUD/world UI/chat/map exits | Small current-map counts panel theo World; bars, quest tracker, food/bình, focus marker/mini HP + screen name/level/current-max HP, reward tooltip, bubble hai dòng, signpost, Boss timer/banner | Kit/typography/marker glyphs; edge arrow + destination name/NPC marker P0; không HP bars Party hoặc quest navigation xuyên map P0 |

Ba skill entry/class hiện **selected, đã học/chưa học, locked, CD và MP** riêng. Tân Lữ slot 1 Mộc Kiếm, 2/3 khóa. `1/2/3` chỉ đổi selection: highlight mới, không pose ra đòn/approach/cost/CD. HUD có cue/glyph `PrimaryAction` (combat branch) riêng và tên kỹ năng sẽ dùng; không giả S1 luôn là Attack. Chọn S2 rồi Execute nhiều lần phải đọc rõ. Select khi pending/buffer/action đã chạy không đổi SkillId intent cũ; accepted presentation lấy snapshot, không slot highlight mới.

Input/cancel/revalidate theo [Combat & Character](../01-design/combat-and-character.md#pending-cast); Art hiện “Đang tiếp cận” hoặc lý do blocked/quá xa/chưa sẵn, không giả cast đã nhận trước validation/commit.

**Điều hướng/input/readability:** ←/→ Move, ↑ Jump, ↓ DropThrough và 1/2/3 Select đã khóa theo design owner. PrimaryAction/QuickHP/QuickMP/Food/menu dùng semantic actions và glyph binding đang thử. E PrimaryAction,4/5 Potion,R Food,I menu chỉ là **PROPOSAL / TUNABLE DEFAULT**, không khóa phím; C/Q chưa được gán mechanic mới. EdgeExit thường dùng mũi tên vùng thoát + tên đích, không arch dịch chuyển hay Interact prompt.

Huyền Môn/Arena có SpecialGate cue khác; marker NPC đủ thấy nơi nhận/trả, quest arrow xuyên map vẫn P1. Không bake key vào thoại/quest/ảnh; lời “nhảy/xuyên sàn/dùng Bình Linh lực” đi với glyph action hiện tại.

**RPG shell — preferred UX direction / PROPOSAL:** có thể hợp nhất Hành trang, Trang bị, Thuộc tính, Thông số, Kỹ năng và Nhiệm vụ trong một shell/menu action. Giữ đủ sáu gear slots/preview, grid inventory, attributes/unspent, derived stats và keyboard access; exact shell/layout/menu binding I/C/Q còn OPEN. Navigate/Confirm/Back và Tab/Shift+Tab theo context UI, mouse gọi cùng command. Modal/chat giữ input: Move/Jump/DropThrough/Select/Execute không lọt gameplay; input mở modal không xác nhận tiếp trong cùng frame.

**UI usability gate P11:** dùng kit panel/button/slot/tooltip/list hiện có để review ở tốc độ thường: tìm món/equip/bán sample không nhầm, nhận/trả quest và next action dễ hiểu, dismiss modal không lọt attack, pending approach có thể hủy rõ, locked skill/exit có reason; thử cả lỗi full bag/đầy HP/sai NPC. Log do dự/misclick/số bước/giờ sửa và nhận xét người chơi; không khóa ngưỡng thời gian hoặc gọi text-only automation là UX PASS.

**Movement/mob feel presentation:** probe jump tap/hold có cùngapex khi cùng physics setup; landing/coyote/buffer/drop với rig và camera thật, pose không lái gravity; soft separation/reposition phải đọc vị trí Sói, không telegraph lệch do visual steering. Không tự thêm animation set, knockback, ring slots hoặc pose budget; exact movement/AI thuộc design owner/Technical/PHY-01.
World marker+mini HP và tên/level/current-max HP/bar trên màn hình cùng bind focus life/generation/MapId; không portrait/element/rarity/generic buff panel. Player chết vẫn thấy/cập nhật HP target hợp lệ khi người khác đánh; target đổi đời/xóa hoặc rời retention thì dọn đúng HUD cũ. Một ActiveFocus: item selected có marker/tam giác nhỏ và PrimaryAction glyph pickup, không active enemy marker đồng thời; click item thay combat selection và cancel pending theo Runtime. Pickup fail không cast fallback cùng press. EXPLICIT xa/khác tầng/blocked vẫn có marker trong vùng giữ, Execute hiện reject reason và không tự swap target.

UI đang xử lý là thông tin core online: chặn thao tác lặp cần thiết, giữ dữ liệu đã commit và báo chờ. Timeout/network error khác RNG enhance fail. Preview có thể xem trước hình gear, HUD/world chỉ cập nhật canonical result đúng revision. Enhance success hoặc fail đã tiêu cost chỉ diễn sau ACK; không dùng animation giả làm người chơi tưởng request đã xong.

**Dependency bổ sung:** font có đủ dấu tiếng Việt, số/+/% dễ đọc; test tên dài “Đóng Băng”, “Huyền Nham Cự Thú”, trạng thái thiếu X ô, wager 10.000 và chat 80 ký tự/hai dòng. Text dynamic không bake thành sprite. Chọn world reference resolution/zoom và UI scale **OPEN A12** trước export: thử 480×270 hoặc 640×360 cho 16:9 (1920×1080 scale 4 hoặc 3); world view rộng 15 hoặc 20 u ở PPU32, ảnh hưởng đọc Bow6,5 u/telegraph. Đây là probe camera, không requirement render resolution đã khóa hay scene mới.

<a id="online-presentation"></a>

<a id="20-presentation-online-server-authoritative"></a>

## Presentation online server-authoritative

**Đọc từ:** owner design tương ứng, owner kỹ thuật tương ứng. Local anticipation cosmetic không mở thêm prediction/rollback physics/combat P0. Backend không nằm trên mỗi hit. Reward/enhance/PvP settlement cần durable ACK; Potion đã accepted trình diễn ngay, consume durability xử lý riêng.

Bảng flow dưới là **phương án presentation để thử A11**, không toàn bộ specification đã duyệt. “Ngay frame input” là mục tiêu probe anticipation cosmetic; dùng state/result authoritative là invariant đã chấp nhận. Exact immediate pose/âm anticipation cần đo ở G-N/P12, không điều kiện pass cứng; không thêm tentative projectile branch. Local slice và Dedicated đều đọc result từ authority; flow server/remote này dành cho TARGET online.

| Bước | Local player | Game Server / remote |
| --- | --- | --- |
| SelectSkillSlot1/2/3 | Chỉ đổi highlight kỹ năng, không chuẩn bị/cast/approach/cost/CD | Selection không là lệnh damage; action đang chạy giữ SkillId snapshot |
| Bấm PrimaryAction (combat branch) | Có thể thử pose/âm chuẩn bị cosmetic khi intent đủ điều kiện; ghi request đang chờ, không giả accepted cast | Intent chụp requested SkillId/target life/MapId; client không gửi trusted targets/damage |
| Validate/start | Giữ UI resource/cooldown canonical, có pending cue nhẹ khi cần; không hiện target impact/HP trừ giả | Server kiểm alive/MapId/class/learned/profile/MP/CD/action lock, chụp source/action origin/weapon visual và startClock, commit cost realtime |
| Accept/reject | Ghép request với actionId, căn phase theo server clock; reject trả pose phù hợp và reason, clear cosmetic anticipation | Remote bắt đầu từ phase còn hiệu lực, không chạy lại windup từ đầu ở packet trễ |
| Release/projectile | Local slash/cast accent có thể anticipate cosmetic; khuyến nghị chờ authoritative spawn để tạo projectile chính trong slice | Authority schedule logical hit; visual event có action/hitIndex/life/origin/aim/travel phase để render |
| Hit/multi-target | Chỉ HitResult mới damage/Crit/NÉ/landed impact; giữ cùng timestamp đối với ordered target sets/spread/nổ | Authority primary/propagation eligibility resolve actual IDs, per-target result/status; remote thấy cùng outcome, không VFX overlap damage |
| CC/Death | Present cancel/freeze/terminal đúng state, bỏ pending hit visual của action bị hủy | Server generation/cancel reason/expiry; unresolved action cancel; visual không gameplay callback |
| Loot/progression | UI pending; pile/reward/durable success theo ACK | Game Server result + backend receipt |
| Potion | HP/MP effect và HUD theo server realtime acceptance, không chờ DB ACK | Persistence pending không replay effect |

Nếu latency làm visual projectile xuất hiện muộn, render ở phase travel hiện tại và nối muzzle trail ngắn để đọc nguồn; không phát một tên thứ hai từ pose release. A11 chỉ probe pose/âm anticipation cosmetic; tentative gameplay projectile và rollback **DROP P0**, không tạo projectile prediction branch. Cung không cần biến mọi visual arrow thành NetworkObject riêng nếu event/state mô tả travel đủ; cách transport là Technical spike, art không chốt implementation.

**Data/clock proposal** về correlation/action/life/MapId/profile/visual revision, projectile flight, hit resolveTime và status/terminal fields đã chuyển đầy đủ sang [Technical — presentation data](../02-technical/gameplay-runtime.md#presentation-data). Schema vẫn OPEN A11/A15. Art cần state/phase/result để diễn và dedup, không sở hữu message schema hay quyết server tick/transport.

Loại VFX lặp theo actionId/projectileIndex/hitIndex/targetGeneration; cache status theo revision, không phát entry lại ở mọi snapshot. Target death đời cũ thắng Hit/Status đến trễ, respawn không nhận effect cũ. Player death là trạng thái riêng: giữ marker/HUD của target còn hợp lệ, hủy pending và khóa combat. Join/reconnect dựng status còn hiệu lực, không phát lại damage/reward; map transition dọn hình map cũ và target MapId cũ.

Actor disconnect trong grace15 s vẫn bị đánh; hình ghost không thành invulnerable. Map filter chỉ trình diễn, không disable server root/player khác.

Ở2–4+ player: giữ unique target/arrow/status rules, tránh tốn VFX theo N×mọi khả năng thay vì events thực. WorldUI priority local/selected/Boss, status một instance/target; giảm remote cosmetic ở crowd. Đo frame time/overdraw/allocations/bytes và latency; bốn player là probe, acceptance hiện tối thiểu hai, không capacity claim.

<a id="21-login-và-character-select-dependency-visual"></a>

## Login và Character Select: dependency visual

**HIỆN HÀNH:** Boot/Main Menu → Login → Character Select → connecting overlay → World. Không Register/forgot password/server browser/Loading Scene riêng/Character Create bắt buộc.

| View/state | Art/UI cần | Dependency cần tránh scope thừa |
| --- | --- | --- |
| Boot/Main Menu | Title/logo chữ, background crop environment, Start/Exit khi flow dùng | Không cinematic hoặc key art bắt buộc; reuse kit |
| Login | Username/password masked, focus/submit/disabled/pending, lỗi dễ hiểu | Admin provisioning/account rules ở design owner/Technical; không UI admin trong file art |
| Character Select | List name/level/class; selected highlight, confirm/back, lỗi list/lease/connect | List hiện chỉ đảm bảo name/level/class. Preview gear **không được giả định backend đã trả equipment** |
| Selected preview | Option rẻ: rig Idle default và class motif; option đủ gear: canonical equipment visual summary read-only | Recommendation default preview +text cho P0 baseline; nếu muốn exact gear, bổ sung dependency read-only summary Technical, không client-state authority |
| Connecting/reconnect | Overlay/progress indeterminate, message và retry/back phù hợp state | Không báo “đã vào World” trước join; không hiện token/ticket/backend thuật ngữ cho player |
| Failure/return | Không có nhân vật/đang trong phiên/không thể kết nối, reason đúng luồng | Không tự thêm nút tạo nhân vật, cấp account hay recovery gameplay mới |

Cụm “Recommendation default preview +text cho P0 baseline” trong bảng trên là **option đề xuất A13**, chưa chọn giữa default và exact gear. Baseline chắc chắn hiện có là list name/level/class; không suy backend đã trả equipment.

Một rig preview reuse Master Pose và Head/Hair, Upper/Lower outfit, weapon assets, không sprite-sheet riêng màn chọn nhân vật. Không claim cây gear thật nếu chỉ nhận class. Khi data thiếu, default preview có nhãn cấp/phái đủ, tránh placeholder fake Rare III làm người chơi tưởng được cấp đồ. Login dependency là UI states/font/input/focus/background và preview source, không thiết kế lại ticket/lease/account schema.

<a id="art-integration"></a>

<a id="22-technical-art-contract--baseline-và-phần-cần-kiểm"></a>

## Technical Art Contract — baseline và phần cần kiểm

**Owner:** Art sở hữu contract visual/import; owner kỹ thuật tương ứng giữ cách tích hợp runtime/physics. Bảng sau giữ đầy đủ baseline/proposals trước review; đọc cùng phân loại các mục liên quan. Canvas/PPU/pivot theo design owner, các lựa chọn exact technique/import/camera chưa qua prototype vẫn NEED VALIDATION; importer production chưa được dựng.

| Contract | Giữ / đề xuất | Căn cứ và cách kiểm |
| --- | --- | --- |
| Player canvas/body | Giữ 64 × 64/PPU32; body 44–48 px là sample BASELINE, side/3/4 author hướng phải + mirror/correction probe | Vừa silhouette nhân vật; weapon/VFX có thể renderer riêng vượt canvas, không scale body để nhét |
| PPU | Giữ 32 toàn world sprite; icon/UI theo UI scale riêng | Body≈1,375–1,5 u hợp collider cao 1,45 TUNABLE;64 px canvas=2 u không phải hurtbox |
| Pivot/alignment | GiữBottom-Center(0,5;0); mốc chân/offset chung trên grid pixel | Không crop mỗi pose; part cùng pose key, đường chân không nhảy khi equip |
| Padding/transparency | Alpha thật; probe margin transparent1–2 px nếu không làm đổi anchor; sheet gutter 2–4 px ngoài cell; atlas padding 4 px khởi điểm | Margin/gutter/atlas padding là ba việc khác nhau. Canvas64 không bị cộng thêm gutter vào sprite rect |
| Palette | Swatch chung da/outline, ba gear/environment families; skill ấm/lạnh/Linh tím có cue hình riêng | Không hard-lock số màu trước test; outline/contrast và band/rarity phân biệt. Không shader palette system bắt buộc |
| Sheet | Mỗi state/profile có grid cell cố định; manifest `poseKey → spriteRef/socket/order/duration` | Shared spriteRef hợp lệ; duplicate sheet cells không tính thành hình vẽ mới. Sheet layout không quyết định damage |
| FPS/duration | Giữ preview baseline6/10/8/8/12/12/10/8 theo player state; timeline duration riêng | Attack release/hit đúng design owner; mob loops chọn theo cadence/hành vi, không ép cùng FPS/frame count |
| Naming | Ví dụ `Player_Male_Upper_Default_Sword_Attack_p02`, `Weapon_Sword_BandII_angle01`, `Mob_Wolf_Move_p03`, `FX_Freeze_Thaw_p01` | Identifier kỹ thuật ASCII/stable, tên display tiếng Việt; index pose khác hitIndex/animation state |
| Folders đề xuất | `Art/Characters/Player/{PoseTemplates,HeadHair,Upper,Lower,Weapons}`, `Mobs`, `NPCs`, `Art/Environment/{Forest,Mountain,Ancient,Shared}`, `Art/FX`, `Art/UI/{Kit,Icons}` | Source editable/sheet/manifest và imported sprite refs phân biệt; không folder riêng mỗi rarity/+level. Chưa tạo các folder |
| World sorting | Background →back props/terrain →actors →WaterFront/occluding trim →CombatReadable →WorldUI | Full structure chia Back/Front/surfaces; opaque front tránh lanes. Telegraph/critical VFX nằm ngoài actor SortingGroup khi cần phủ world |
| Actor order | Theo quan hệ occlusion của Master Pose; per-pose override/render slice khi cần; exact numbers/count OPEN | Probe SortingGroup để tránh chen actor; upper/hand/weapon overlap hai hướng. FX trong group không tự vượt foreground/group khác |
| Anchors/sockets | Root/Feet/Head/Upper/Hip/Grip/Back theo Master Pose; Muzzle/nock/Tip/VFX khi cần; local offsets và mirror nhất quán | Socket visual không gameplay hit origin; shape/hurtbox vẫn server data |
| Frame sync | Một state/phase clock/actor, class profile và logical pose mapping chọn đồng bộ mọi part | Cùng sprite refs/transforms/order của sample; swap gear đọc pose hiện tại, không restart Idle/Attack |
| Flip/scale | Mirror VisualRoot **hoặc** flipX +explicit socket mirror, không cả hai; physics root giữscale1 | Linh scale chỉ visual; non-integer scale/rotation cần test pixel. Không mirror text/worldUI |
| Pooling | Presentation-only pools cho impact/projectile renderer/status/telegraph/feedback/environment thưa | Reset timers/listeners/color/material/phase/parent/action/life/MapId; cancel callbacks cũ; không pool quyết gameplay lifetime |
| Sprite import | Point, Compression None cho nguồn pixel baseline, no mipmaps, alpha, full rect/modular cell cố định; disable read/write nếu không cần | Không max-size downscale sheet; atlas settings/platform overrides phải kiểm riêng, không chỉ texture nguồn |
| Atlas | Khi profiling cho thấy có lợi: thử padding 4 px; UI tắt packing rotation; cân nhắc tight packing theo module QA | Packing không đổi pivot/pose semantics; không khóa một atlas, một draw call hoặc hứa zero allocations |
| Camera | Asset PPU32, integer output scaling/render snapping nếu pipeline đã chọn hỗ trợ | Không snap physics/server positions; test camera/interpolation/Cinemachine và screen ratios trước khóa resolution |

Unity hướng dẫn cùng PPU, Point filter và Compression None cho sprite pixel; atlas có padding mặc định4 và setting rotation/texture riêng. Đây là cơ sở kiểm import, **không tự pin phiên bản Unity/URP**; spike pin Editor/package thực dùng theo Technical. [Nguồn Unity — chuẩn bị sprite pixel](https://docs.unity.com/en-us/engine/6000.6/manual/unity2d/2d-urp/2d-pixelperfect/prep-sprites), [Sprite Atlas reference](https://docs.unity3d.com/6000.3/Documentation/Manual/sprite/atlas/sprite-atlas-reference.html).

ObjectPool cung cấp cơ chế reuse object, còn reset/dedup/generation ở trên là contract đề xuất của Huyền Lộ, không engine tự bảo đảm. [Nguồn Unity — ObjectPool](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Pool.ObjectPool_1.html). Không thêm lighting stack, Addressables hay package skeletal chỉ để đạt contract này.

Pipeline và phép đếm trước review nằm tại [Phụ lục C — pipeline](../90-archive/art-history.md#legacy-visual-flow) / [accounting cũ](../90-archive/art-history.md#legacy-art-accounting). Contract đang dùng ở các mục liên quan; các anchor lịch sử vẫn được giữ.

<a id="first-art-probe"></a>

<a id="221-first-art-probe--sword01"></a>

### First Art Probe — minimal Kiếm và Cung

**CHƯA CHẠY; đề xuất nghiệm thu, không triển khai trong lượt docs.** Thử cùng camera gameplay scale/movement/rig 64×64/PPU32 trước cả catalog; chưa đổi 26-frame lock.

1. Một Master Pose sample với Default Head/Hair, DefaultUpper, DefaultLower, Áo I, Quần I, Mộc Kiếm và Thanh Mộc Cung + arrow. Mẫu Cung đầu tiên dùng Thanh Mộc Cung hiện hành trong catalog, không tạo item mới. Mộc Kiếm giữ basic Tân Lữ; Thanh Mộc Cung dùng profile Cung hợp lệ. Thêm upper B/lower B nhỏ hoặc placeholder có nhãn để kiểm mix chéo với Áo/Quần A; không full bands/catalog hoặc class-switch gameplay.
2. Coverage: Idle 3/4 Left/Right, side action/Run, Jump/Fall, Sword S1/S2 (A/B) và S3 fixture, Bow draw/release S1/S2 ABC/ABA/AAA và S3 fixture, Back↔Hand, Hit/reuse và pop/fall→shadow. Land dùng lại trước; turn/draw-sheathe strip và stance chân riêng chỉ nếu thiếu readability. Giữ cùng action clock/hit contract, không AnimationEvent damage.
3. So hybrid với raster/socket trên cùng mẫu: 3/4 Idle→Run, chạy→Idle cùngfacing ngay/delay, focus-only idle, recovery→idle và Execute S2 liên tiếp; quay đầu/chạy ngược action/air theo policy hiện hành. Đổi Sword/Bow, áo/quần, gear swap giữa action nếu gameplay cho phép; ghi delta pose thật chứ không chỉ xem một outfit vừa khít. Tối thiểu kiểm DefaultUpper+DefaultLower, ArmorA+DefaultLower, DefaultUpper+PantsA, ArmorA+PantsA, ArmorA+PantsB và ArmorB+PantsA; thiếu B thì ghi coverage chưa đủ, chưa khóa reuse/budget.
4. Xuất alpha cell cố định; import/slice/pivot và socket theo [pipeline](#art-integration). Kiểm hai hướng và transitions Idle→Run, Run→Jump, Jump→Fall, Idle/Run→Attack: cổ/head, eo/hip, tay/string/nock, vạt/tay áo không gap hoặc trượt. Back→Hand→Back cùng pose schema cho Sword/Bow, không đổi body proportions theo weapon. Kiểm carry/canvas, clipping/ghost, mirror/correction không đảo sai tay/bao, pixel jitter/foot pivot và mốc hit/release ở camera thật; equipment vẫn đúng khi tắt skill VFX.
5. Player death: authority→pop/fall→shadow, hide gear, duplicate/late/stale event, revive/map reset; camera/dead state/focus không theo offset visual. Online timing xác minh sau G-N, không local fixture pass thay evidence mạng; P06 mob giữ nguyên.
6. Manifest dự kiến đủ tám state/26 logical samples theo direction hiện hành, ghi cả ô chưa dựng và unique/reused sprite, socket-only/VFX-only/optional pose; subset/placeholder không chứng minh reuse của ô chưa thử. Đo giờ tạo/sửa/export/socket/import/QA/rework và ảnh loại. Kết luận technique, reuse, facing/carry, effort/outfit, visual consistency và review mapping/reuse trong26 logical samples (A01/A02/A12/A17) trước nhân catalog. Chỉ đánh dấu pass khi có asset/runtime evidence.

<a id="222-file-lifecycle-và-posesocket-data"></a>

### File lifecycle và pose/socket data

**Đường đi asset:** editable source → PNG trong một sandbox art/rig disposable độc lập → Unity import tạo sprite refs + `.meta` → definition/prefab của probe tham chiếu refs. Exact sandbox path còn là lựa chọn tooling. PNG export chính là imported asset, không thêm bản PNG “runtime”; Library cache không version. Sau production base review, asset đã chọn mới chuyển sang `game/Assets/GameArt/`, giữ GUID khi move và refs trong presentation definition. Probe trước base không tự là production architecture.

Probe revision mới dùng sandbox riêng theo Roadmap; [export path cũ](../90-archive/art-history.md#export-path-của-probe-cũ) chỉ để truy vết.

Mỗi export có stable asset ID, source path và revision. Sửa source rồi export đè đúng file; retain `.meta` khi rename/move. Sheet layout đổi thì kiểm lại slice refs, không chỉ tên PNG. Font/license và file gốc của asset ngoài được giữ cùng nguồn.

**Master Pose manifest — presentation only, exact schema OPEN:** logical `PoseKey`/profile/revision và duration/phase mapping; character root/origin, feet/ground, head, upper-body, hip/lower-body anchors; hand/grip, back weapon socket; sprite refs theo visual set/part, per-part local offsets và optional rotation/flip/correction/order metadata. Muzzle/nock/Tip/VFX anchors thêm khi cần. Artist và runtime dùng cùng convention; cùng spriteRef dùng cho nhiều logical poses là hợp lệ. Không có damage, MP, range, cooldown, gameplay hitbox hoặc quyền đổi hit moment trong manifest.

**Engineering recommendations / ART PROBE:** schema revision và compatibility check của visual set phải chỉ ra missing pose/anchor/part. Required grip/back/hip/head mapping cần validation, không âm thầm dùng offset0; optional socket có fallback được khai báo. Khi thiếu visual resource, dùng fallback đã kiểm đúng pose hoặc giữ hình compatible trước đó kèm diagnostic dev; không che lỗi bằng body layer, tự unequip hay đổi gameplay. UI preview và world dùng cùng mapping contract nhưng instance phase/material riêng.

**Socket probe A02:** so hai phương án trên cùng coverage probe/3/4 Idle và hai hướng side: (A) pose data lưu điểm/góc pixel rồi runtime đặt weapon; (B) author Transform socket trong rig/prefab rồi export bảng theo poseKey. Chọn một nguồn cuối, không sửa cả Transform và data độc lập. Trong mẫu A, tọa độ canvas gốc trái-dưới: điểm `(u,v)` đổi local thành `((u−32)/32,v/32)` theo Bottom-Center/PPU32; exporter từ gốc trái-trên phải đổi Y một lần.

Weapon grip offset/góc và front/back per pose ghi cùng entry; mirror cả điểm/góc đúng một lần ở VisualRoot. P01/P02 kiểm tay không trượt, nock/release đọc đúng, equip không restart clip. Chưa pass/chọn phương án thì chưa nhân catalog.

<a id="223-visual-language--mẫu-đọc-trong-30-giây"></a>

### Visual language — mẫu đọc trong 30 giây

Status: **style sample để kiểm**, giữ canvas/PPU và camera side-view; technique/reference resolution còn OPEN. Không khóa tỷ lệ đầu/thân, HEX, outline 1 px, số mức shading, mặt/tóc/chi tiết áo, hình shadow hay linh vật trước sample được duyệt.

- Character: body 44–48 px trong cell 64, silhouette/hand-grip ưu tiên. Thử outline tối 1 px, ít mức sáng/tối và một hướng sáng thống nhất; không để palette gear che tay/vũ khí.
- Environment: lane/edge đứng được tương phản rõ; background giảm contrast, foreground không che silhouette/telegraph. Cùng vật liệu vẫn phân biệt solid và one-way bằng cạnh/shape.
- Kiếm/Cung: chém ấm, cung lạnh, Linh tím kèm hình/nhịp riêng; cue release/impact gọn và ăn clock. Không chỉ dùng màu để phân Freeze/Slow.
- UI: panel dùng kit chung, text Việt đọc ở camera/output thực; selected focus và disabled/error khác nhau. Icon/slot/tooltip hỗ trợ thông tin, không thay bằng màn bitmap hoặc bắt click để chơi.

<a id="224-provenance-tối-thiểu"></a>

### Provenance tối thiểu

Một record cho mỗi nguồn/pack với asset IDs liên quan: URL hoặc source hash, author/tool, license text/file, phạm vi sử dụng đã kiểm, modified/from-source revision, ngày lấy và người kiểm. Asset tự vẽ ghi tác giả; AI ghi tool/model nếu biết và reference nguồn. Thiếu thông tin thì đánh dấu chưa kiểm và dùng placeholder, không tính vào số asset sẵn sàng phát hành. Kiểm quyền tại lúc dùng/mua; không suy toàn pack có cùng quyền từ một ảnh mẫu.

<a id="production-accounting"></a>

<a id="23-production-accounting-đếm-công-cần-làm-không-đếm-itemanimation"></a>

## Production accounting: đếm công cần làm, không đếm item×animation

**Đơn vị đếm:** pose key là tư thế cần đọc; frame/ô logical là mẫu trên timeline; sprite outcome là ảnh raster khác được xuất; variant là sửa từ mẫu; module là họ part; preset là cấu hình VFX; placement là instance đặt trong map. Một ảnh dùng ở năm ô vẫn một ảnh, nhưng cả năm chỗ ghép/timing phải kiểm. Recolor xuất PNG khác là ảnh variant, không công vẽ dáng bao mới.

**Cách tính công hiện tại:** tách vẽ pose mới, sửa variant, xuất/cắt ảnh, gắn socket/nhập Unity, đặt map, QA và sửa lại. CURRENT phải đo mẫu player/3/4-side/outfit swap/Sword-Bow/carry/shadow và room/UI nhập chạy được; hiện CHƯA ĐO, tổng ảnh/giờ vẫn OPEN A17.

Tra đầy đủ [pose reuse và estimate player](#player-s0) và [các family/công sản xuất](#family-scenarios) tại Phụ lục B. Các bảng giữ toàn bộ giả định/dedup, phép cộng, workload và ma trận QA; số suy ra chưa phải số asset đã sản xuất. [Accounting cũ](../90-archive/art-history.md#legacy-art-accounting) nằm riêng trong trace để tránh dùng làm budget hiện tại.

<a id="art-tool-workflow"></a>

<a id="231-quy-trình-art-nhỏ-cho-người-chưa-thạo-vẽ"></a>

### Quy trình art nhỏ cho người chưa thạo vẽ

**TOOL CANDIDATE / CURRENT PROBE:** PixelLab là công cụ ứng viên để thử mẫu đầu vào bằng Free/free trial nếu phù hợp. Mẫu không đạt hoặc thiếu công cụ thì đổi cách làm; contract Art và kiến trúc không phụ thuộc PixelLab. Không mua/tạo full catalog theo estimate chưa đo.

Trước khi làm probe, kiểm [FAQ chính thức](https://www.pixellab.ai/docs/faq) và quyền thực của tài khoản Free/trial; không giả mọi tính năng animation/outfit/kích thước đều miễn phí.

Chọn mẫu nhỏ theo [minimal player probe](#first-art-probe): 3/4 Idle/side, outfit swap, Sword/Bow/carry và shadow; một mặt địa hình/icon/impact thử riêng khi cần. Giữ reference/palette/canvas/pivot; xuất ảnh, sửa outline/alpha/màu/tay nắm/căn lớp trong editor pixel (ví dụ Pixelorama), rồi cắt/nhập Unity ở camera thật. Dùng sandbox disposable độc lập trước base. Free thiếu công đoạn cần kiểm thì ghi thiếu evidence và dùng placeholder cho pipeline; không kết luận hybrid đạt khi chưa thử.

Mỗi output sau QA thuộc đúng một nhóm **dùng trực tiếp / dùng sau sửa / loại bỏ**. `Dùng trực tiếp% = pass không sửa / tổng output`; `dùng sau sửa% = pass đã sửa / tổng output`; `dùng được tổng% = (hai nhóm pass) / tổng output`. Đếm cả output thất bại và cỡ mẫu, giờ sửa/cắt/gắn socket/nhập/QA/làm lại theo loại. Chưa chạy ghi **CHƯA ĐO**, không đoán 70/80/90%. PNG đẹp nhưng chuỗi pose/tay áo/socket lệch thì animation chưa pass; một frame đạt không chứng minh cả strip đạt.

Dùng số đo để quyết tiếp tục công cụ, sửa tay thêm, giảm polish hoặc dùng asset có nguồn rõ. Đánh giá công của **mẫu đã nhập chạy được**, không theo credits/ảnh đẹp nhất. Chi tiền là bước sau evidence; không tự mua gói. Gate/nguồn lực xem [Roadmap](../04-production/roadmap.md#art-workflow).


<a id="25-tra-cứu-open-decisions"></a>

## Tra cứu OPEN decisions

Options/trade-off visual tại [Art decisions](#art-open-decisions); gameplay/runtime/persistence ở semantic owner theo [index](../README.md#open-decision-index). Ma trận thử thuộc [Playtest & Balance](../04-production/playtest-and-balance.md#art-validation).

A01/A02/A14 chi phối pose/modular và công; A03 giữ sáu ô/stat-only, đề xuất bỏ Boots cũ SUPERSEDED; A04/A05/A06 liên quan concept/timing/Cung; A07 Dummy; A08/A15 death/corpse/loot; A09/A10 terrain/route/LoS và Hybrid còn OPEN; A11/A13 online/Character Select; A12 camera; A16 Boss; A17 accounting. CURRENT/DEFERRED do Roadmap sở hữu. Chưa probe không biến đề xuất technique/count thành LOCKED; mốc gameplay hiện hành đọc design owner.

---


<a id="art-technique-rationale"></a>

### So sánh raster, skeletal/socket và hybrid

| Cách | Combat/pixel art 64×64 | Chi phí thực và rủi ro |
| --- | --- | --- |
| Modular frame-by-frame | Outline và foreshortening kiểm soát tốt; mọi part chọn theo pose key | Redraw áo/tay ở pose khác; tốn kiểm mọi outfit. Bảng pose dùng lại giảm sprite nhưng không xóa công QA |
| Skeletal/socket toàn thân | Reuse ảnh chi và tween được nhiều action | Đầu tư rig/weights/khớp; rotation/deformation có thể làm pixel/outline biến dạng. Tay áo dài và occlusion khó; không mặc định rẻ hơn |
| **Hybrid đề xuất** | Cơ thể/áo/chân chủ yếu raster theo pose; đầu reuse có offset; vũ khí theo socket và vài pose sửa raster | Giữ chất pixel tại các pose chính, giảm redraw weapon và đầu. Cần kiểm đường xoay vũ khí, tay nắm và front/back; không cần hệ skeletal tổng quát |

**OPEN A01/A02:** prototype hybrid trước. Nếu socket rotation gây nhấp nháy hoặc mất hình kiếm, dùng góc raster đã vẽ ở pose chính; nếu áo/chân lệch, thêm đúng pose thiếu thay vì đổi toàn dự án sang skeletal. Reuse lower-body giữa hai class chỉ sau khi stance và trọng tâm được kiểm.


<a id="derived-production-scenarios"></a>

## Phụ lục B. Kịch bản production suy ra

Các giả định, phép cộng và giới hạn dưới đây là kịch bản có thể thay bằng evidence; player đã cập nhật cho hướng 3/4/carry/shadow. Đây là dữ liệu tính thử, chưa phải manifest hoặc budget đã được duyệt. [các mục liên quan](#production-accounting) là điểm vào accounting hiện tại.

<a id="player-s0"></a>

<a id="b1-kịch-bản-player-s0-có-thể-kiểm-lại"></a>

### Player: pose reuse matrix và effort — PROPOSAL

Anchor S0 giữ routing tới accounting hiện hành. [Snapshot cũ](../90-archive/art-history.md#legacy-player-body-overlay-accounting) giữ các phép cộng Body/Armor trước correction; không dùng để đặt budget mới.

| Nhóm pose / kiểm | Head/Hair | UpperBody/Armor hoặc default | LowerBody/Pants hoặc default | Weapon |
| --- | --- | --- | --- | --- |
| Idle Left/Right 3/4 | Reuse theo góc + head offset/correction | Silhouette 3/4 đúng neckline/waist | Stance/feet khớp hip | Back/Hand silhouette theo phase |
| Run / Jump / Fall | Reuse khi góc khớp | Tay/vai/torso theo Master Pose, reuse hoặc redraw khi cần | Chân/contact/apex theo cùng sample | Socket/offset theo pose, không ảnh mới cho từng ô |
| Sword / Bow action | Đầu/neck đúng action facing | Swing/draw fragment gồm arms/hands, class profile chung giữa outfits | Reuse locomotion/stance nếu compatible; correction khi probe cần | Hand/Back; angle/shape variant chỉ khi chất lượng cần |
| Hit / recovery | Offset/correction đúng phase | Không restart action khi flash; dùng pose hợp lệ | Giữ locomotion/contact phù hợp | Theo action/weapon snapshot |
| Player death transition / shadow | Reuse transition rồi hide | Reuse transition rồi hide; không outfit corpse | Reuse transition rồi hide | Hide khi vào shared shadow |

**Raster budget OPEN:** sau P01/P02/P03 và effort P15 mới quyết số upper rasters mỗi Armor/default, lower rasters mỗi Pants/default, head/hair rasters, weapon angle/shape/carry variants và pose nào reuse/mirror/correction. **26 logical poses LOCKED** là coverage timeline; có thể reuse cùng sprite, đổi offset/hold, reuse head/lower qua action hoặc socket transform. Không suy ra mỗi item cần 26 unique sprites và không đặt count từ theoretical matrix.

**Công =** author Master Pose reference/schema + tạo/sửa fragment từng visual set + export/slice + anchors/socket/order/import + integration/QA/rework. Template không phải một bộ body rasters runtime cần cộng thêm. Manifest kê ảnh thực mới/reuse/render slices và correction; một slice tăng setup/QA dù dùng cùng nguồn. Đổi Kiếm/Cung band không redraw Upper/Lower; không author `Class × Armor × Pants × Weapon × Frame` full sprites. Exact part proportions/raster counts và technique được chốt sau sample, không claim reuse 100% hoặc tiết kiệm một tỷ lệ cố định.

13 gear world modules (4 Sword +3 Bow +3 Armor +3 Pants) và hai default outfit modules mô tả visual bindings theo catalog; Head/Hair và shared shadow là visual roles riêng, Master Pose là reference/data. Những bindings này không là renderer count, atlas count hay unique raster budget.

<a id="family-scenarios"></a>

<a id="b2-tổng-quan-các-family-còn-lại"></a>

### Tổng quan các family còn lại

| Asset family | Unique frame/sprite trong kịch bản | Variants/reuse | Workload chưa thể quy thành số ảnh/giờ |
| --- | --- | --- | --- |
| Normal mobs | 6 rigs; 105 pose là kịch bản lịch sử có điều kiện ba Hybrid các mục liên quan | Wolf palette identity thứ 7; xuất PNG palette thêm 17 recolor outcomes nhưng0 pose mới; không rig Linh mới | Chốt capability OPEN rồi kê pose thật; clean/windup/hurtbox/death QA |
| Boss | 1 rig /28 core pose, optional+4 roar | Claw reuse Basic, Dư Ảnh chỉ tên; Cuồng/Slow overlays | Canvas/area lớn, telegraph/scheduler/3-zone QA; không lấy cost/frame player áp Boss |
| Dummy | 1 prefab visual /6 pose | Một pool dùng Q3/Q6/training, số placement theo validation | Shared HP/life/credit/respawn, không dummy system/DPS Meter |
| NPC | 7 sprite toàn thân /14 idle; optional+6 →20 hình | Props/source templates chung; 8/16–22 là lịch sử đã bỏ Tạ Minh | Setup menu/khu chức năng/anchors/occlusion; không 7×26 hoặc 7 portraits bắt buộc |
| VFX | **10 family chức năng** bên dưới, không 10 PNG | Profile/weapon/target reuse config | Frame count sau chọn sprite/particle/mask; overdraw/pool/reset/online QA |
| Terrain | 3 chất liệu, topology 17 +cosmetic4/họ →tối đa 63 hình các mục liên quan | Collider semantics/template chung, mirror/dedup có thể giảm | Room authoring và edge/route QA, không 8 maps×63 |
| Mini building | Ví dụ gỗ 10 logical modules /8–10 hình các mục liên quan | Cầu/mái/cột lặp; stone có thể dùng terrain | Chưa có layout hoàn tất để chốt stone kit và full-asset dimensions |
| Full structural assets | Danh mục cần review: forge station, broken/quest seals, Huyền Môn, Boss landmark | Ba seal anchors dùng một motif, back/front slice; waterfall ở nhóm riêng | Chốt mỗi landmark sau blockout; không tự áp một asset độc nhất mỗi map |
| Background/decoration | 3 environment families; hub reuse | Palette/crop/clusters, props dùng lại nhiều roots | Số silhouette/prop phải từ blockout+camera; chưa derive được tổng nên không đoán |
| Animated environment | Thác12 ô; nếu dùng: flow4, ripple 3–4; puddle2 static, leaf 1–2, puff 1, flame 4 | Flame Burn dùng chung khi phù hợp; puff/dust dùng lại | Không cộng flame/texture hai lần vào tổng; cần manifest shared refs |
| Items/icons | Base 49 physical trước collection; candidate +6 collection (+Q2: 1 nếu duyệt), không final catalog count | Food status reuse, rarity/+n overlay; không dùng 63 outcomes cũ làm current asset budget; collection icons reuse sigil | Motif mới/clean crop/tier accent phải ghi riêng; không 64 tranh hoàn toàn mới |
| Common UI | 21 primitives/chức năng các mục liên quan | Panel/button/slot compose; nhiều states tint/mask/text | Layout/bindings/font/focus/pending/error; screen-specific art chỉ phần thật cần |
| Map content placement | 8 roots/5 farm maps; số pocket/slot mới OPEN/TUNABLE | 28 pockets/66 slots là LEGACY seed, ID nguồn quest vẫn giữ | Re-author nhánh/nhiều tầng/cụm độc lập, colliders/exit/safe strips/Boss exclusion; reuse art vẫn tốn công editor/QA |

Mười VFX family để không giấu scope trong “4 active”: **(1)** sword slash, **(2)** arrow/flight kể cả generic mob config, **(3)** signature line/wave, **(4)** explosion/burst, **(5)** landed impact, **(6)** telegraph cone/ground/landing, **(7)** statuses Burn/Freeze/Slow, **(8)** aura Linh/Cuồng, **(9)** beneficial heal/MP/Food/revive, **(10)** kết quả reward/loot/enhance/death cosmetic. Family là nhóm reuse/QA, không buộc một prefab đa năng. Status family cần ba ngôn ngữ khác nhau, không gọi cùng một tint là đủ cả ba.

Frame probe slash4/wave4/burst5/impact 3/Freeze shell 2+thaw 3/flame 4 ở các mục liên quan có lý do theo phase. Particle/glint/sigil thường chỉ một primitive; không cộng mỗi lifetime tick thành frame. Main active có sáu SkillId bindings; unique preset/bitmap theo reuse manifest, không ép sáu families; normal/telegraph/status ngoài active được tính độc lập. Tổng sprite VFX chỉ chốt sau manifest xác định shared textures và technique, tránh double-count flame/aura/arrow.

**Cách ra production cost thực:** đo giờ riêng cho tạo pose template mới, sửa silhouette variant, recolor/cleanup, export/slice, socket/sorting setup, integration, QA và rework. Ước lượng `Σ(số pose mới×giờ/pose mới + số sửa variant×giờ/sửa + công setup/QA)` từ slice đạt chất lượng, không từ credit hoặc phép nhân item count. Ghi cả lần sửa Bow và Death thất bại; không dùng lần recolor nhanh nhất làm tốc độ vẽ toàn bộ Boss. Chưa có asset/slice nên **không có căn cứ chốt tổng giờ**; mốc160–240 h cũ đã bị Technical loại là estimate hiện hành.

Ma trận QA tối thiểu: mỗi Armor với Sword Attack và Bow draw/Skill; mỗi LowerBody với Idle 3/4 Left/Right, Run/Jump/Fall và player death reuse→hide; cả 7 weapon với Attack, 6 weapon class với Skill, ở hai hướng; một số mix-band mặc chéo và default/unequip. Nếu kiểm toàn bộ: 4 Armor× 4 LowerBody× 7 weapon=112 outfit/weapon combinations trước hướng/pose, nhưng không vẽ 112 rigs. Kiểm theo part/contact và ca mix-band rủi ro, không giả bỏ integration nhờ reuse.



<a id="huyền-lộ--phân-tích-art-hình-ảnh-và-production"></a>
<a id="working-spec-end"></a>

## Early Bow art minimum

Kiếm kiểm S1 và S2 A/B primary-proximity ngang ưu tiên Cung S2, S3 fixture, weapon grip/socket và timing. Cung kiểm S1 cùng S2 Spread hiện hành, draw → release → recovery, bow/arrow socket, focus/range và kite trên cùng room/movement setup. Placeholder chỉ được dùng nếu có silhouette và timing đủ đọc. Chưa làm full Bow outfit/family/VFX catalog; không thay S2 bằng Xuyên Tiễn hoặc thêm hold-repeat. Protocol chung ở [early probes](../04-production/playtest-and-balance.md#early-two-class-probe).

Food HUD đọc được icon/state/duration, gần hết/expiry và reason thiếu MP; presentation đọc authority, không auto-consume. Tham số gameplay thuộc [Items & Economy](../01-design/items-and-economy.md).

## Potion presentation boundary

Accepted Potion result của Game Server cập nhật HP/MP/HUD và heal/MP VFX ngay trên server timeline, không đợi PostgreSQL ACK. Reject không hiện heal. Durable pending chỉ là trạng thái đồng bộ; ACK không phát effect lần hai. Loot/reward/upgrade/escrow success vẫn theo durable receipt. Xem [Potion ordering](../02-technical/gameplay-runtime.md#potion-ordering).


**RPG navigation — chức năng giữ, shell còn PROPOSAL:** Hành trang, Trang bị (sáu slot + preview), Thuộc tính (STR/VIT/INT/AGI/unspent), Thông số (HP/MP/ATK/DEF/ACC/EVA/Crit/MoveSpeed/Class/Lv/EXP), Kỹ năng và Nhiệm vụ đều phải dùng được bằng keyboard. Nếu dùng shell chung, Tab/Shift+Tab đổi view; Navigate chọn ô/action, Confirm xác nhận, Back lùi. Mouse gọi cùng commands/validation. Slot weapon trống mở bag lọc Vũ khí; Store/Take chỉ tại kho, Buy/Sell chỉ tại shop. Exact layout và physical menu key còn OPEN, không khóa I/C/Q.

`Frame` là ô lấy mẫu timeline; sprite/raster là ảnh pixel xuất. Rig là bộ ghép actor, pose là tư thế và socket là điểm gắn theo pose; các đơn vị này không đồng nghĩa nhau.


<a id="mach-an-motif"></a>

## Mạch Ấn xuyên world — PROPOSAL mở rộng motif, exact concept PROBE

Một motif nhỏ tái dùng: nét nối/đường mạch và dấu ấn đứt/gắn lại, không một asset family lớn. Vân Khê/Đồng Sương chỉ crop nhỏ trên bia/gear/props; Trúc Ảnh rõ ở BrokenSeal/aura; Xích Nham dấu đục/mạch bị phá trên ba seal; Huyền Tích lớn hơn ở cổng/cột/Boss landmark, vẫn cùng ancient kit/sigil. Skill/VFX có cùng cue nét nhưng khác shape/nhịp theo action, không thêm entity/pet/CC. Intensity/size/palette cụ thể là PROBE, không thay lore/biome/skill counts.

Readability của Huyền Lộ dùng top rim sáng, mặt đứng tối, mái/tầng có support, biome edge material, props tre sparse, silhouette và hiệu ứng gọn. Dựng bố cục theo geography/collider và motif riêng; provenance/sample QA trước production rộng.

<a id="primary-focus-visual"></a>

## ActiveFocus / Map Info / Inventory visual sync — CURRENT DIRECTION

[Runtime](../02-technical/gameplay-runtime.md#active-focus) sở hữu input/transitions, Art chỉ presentation. Một marker actionable cho Enemy/PvP/GroundItem/NPC/Landmark; combat miniHP/HUD chỉ combat branch, noncombat đổi cue theo type. Explicit GroundItem marker nhỏ nhìn được giữa piles; không icon tương tác song song active enemy. Quest/service marker !/? là trạng thái khả dụng, không actionable focus thứ hai. Hover/candidate cue nếu cần phải yếu và khác selected (**OPEN exact styling**).

NPC PrimaryAction mở contextual root, quest option preselected khi hợp lệ; Talk là option. Opening press không Confirm; modal header/breadcrumb/pending/reason đọc kết quả server. BrokenSeal/ChiselMarks/SealScar/trụ Xích cũ tiếp tục environmental lore, không mặc định checkbox; Q11 restored placements reuse Ancient motif trên nhiều maps chưa chọn, không author toàn bộ ba seals ở Xích theo graph cũ.

Map Info panel nhỏ tên map + ordinary total/Linh subset/NPC/Boss hiện hành theo [World](../01-design/world-and-content.md#map-population-info). Exact layout/text/colors OPEN; pending/reconnect/mapchange cue tránh stale truth. Không coordinates/pockets/ping/arrow Linh; ordinary total không cộng Linh hai lần. Reuse text/panel kit, chưa tạo assets hoặc bitmap budget mới.

Inventory60/60, quantity picker và total Buy/Sell; Split quantity+empty-slot, Sort/Merge explicit, Discard confirmation nêu permanent destroy/no-ground. Stable-ID selection/survivor mapping theo Runtime. Quest item subtle background/overlay, unsellable/undiscardable disabled reason; palette OPEN, nền trắng không permission. Equipment details gọn ReqLv/Class với unmet đỏ nhẹ, stats dễ đọc/modest colors; comparison compact optional, không mặc định delta kép mọi stat.

Composition theo [Master Pose](#master-pose): Head/Hair + UpperBody/Armor + LowerBody/Pants + Weapon; Q3 Quần/Q4 Áo visible, Boots/Ring/Necklace stat/icon; LeftRight/Idle3/4/RunJumpFallSide/AttackSide, Back/Hand, **26 logical LOCKED**. Không đổi one-main-action/VFX, SwordS2 embedded Lân hoặc qi slash alternative, BowS2 one release/ABC-ABA-AAA. Updated icon accounting là conditional bindings6, không6 bitmap mới và material Q11 exact vẫn OPEN.
