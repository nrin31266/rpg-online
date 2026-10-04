# Huyền Lộ — Art, hình ảnh và production

## Tóm tắt

Owner của visual/pose/weapon/map/UI/import và production accounting. Giữ nguyên quy định art, 26-frame contract, các bảng và scenarios. Gameplay/input dẫn về GDD; blockout tối giản của bản chạy thử không thay art production.

## Tìm gì ở đâu

- [Player / 26 frame](#player-visual), [weapon](#weapon-visual), [combat / mob](#combat-visual).
- [Map / terrain / building](#map-visual), [icons / UI](#icons-ui).
- [Import / First Art Probe](#art-integration), [accounting](#production-accounting), [validation](#art-validation).

**PROJECT STATUS:** DESIGN + PROTOTYPE VALIDATION. VS-1 hiện có là disposable/reference integration prototype, production codebase chưa bắt đầu. Primitive/text UI và video revision cũ không production art/UX acceptance; prototype folders/classes không quyết architecture. CURRENT là harvest/feel/art/UI probe trước base theo Roadmap.

<a id="huyền-lộ--phân-tích-art-hình-ảnh-và-production"></a>

**Đối chiếu:** [GDD hiện hành](1_HUYEN_LO_GDD.md), Technical và Design Analysis.

**Ngày:** 2026-10-04 · **Vai trò:** tài liệu sống chính thức ART / VISUAL / PRODUCTION. Nguyên tắc được chấp nhận, baseline và proposal OPEN được phân biệt dưới đây; đã có [VS-1 primitive blockout](../../prototypes/VS1_EndToEnd/README.md), chưa có rig/asset production đã nghiệm thu.

File này sở hữu chi tiết visual/pose/weapon/animation/map/UI/import và production accounting. [GDD](1_HUYEN_LO_GDD.md) sở hữu gameplay; [Technical](2_HUYEN_LO_TECHNICAL.md) sở hữu runtime/data/physics; [Analysis](3_HUYEN_LO_DESIGN_ANALYSIS.md) sở hữu evidence/quyết định; [Roadmap](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md) sở hữu thứ tự làm. Các kết luận đã sync có phạm vi rõ; proposal gameplay và các con số production chưa kiểm vẫn chưa được duyệt.

**HIỆN HÀNH** = requirement/số đang dùng; **ĐỀ XUẤT** = phương án để duyệt; **SUY RA** = phép tính kèm giả định; **OPEN / NEED VALIDATION** = chưa khóa. Các số art dưới đây là kịch bản authoring để kiểm scope, không phải asset đã tồn tại hay cam kết giờ sản xuất. Mọi thay đổi gameplay được nêu riêng và chỉ sync sau khi duyệt.

### Đọc nhanh theo việc đang làm

- [Phạm vi/trạng thái](#art-review), [player/layer/pose](#player-visual), [weapon/equipment](#weapon-visual).
- [Combat/VFX](#combat-visual), [mob/death/Dummy](#mob-visual), [NPC](#npc-visual).
- [Map/terrain/building/environment](#map-visual), [icon/UI](#icons-ui), [online/Select](#online-presentation).
- [First Art Probe](#first-art-probe), [Technical art contract](#art-integration), [accounting/workflow](#production-accounting), [CURRENT validation](#art-validation).
- Tra sâu: [rationale](#art-rationale), [derived scenarios](#derived-production-scenarios), [historical trace](#art-historical-trace), [bản đồ MOVE](#cleanup-source-destination).

<a id="art-review"></a>

## 0. Review và phạm vi được chấp nhận

**APPROVED** là nguyên tắc đủ cơ sở để dùng trong specification, không đồng nghĩa mọi con số trong nhóm đã khóa. **BASELINE** là luật/số GDD hiện dùng; **CURRENT PRIORITY** là phần thử Kiếm/VS-1 trước; **DEFERRED** là implementation/art probe làm sau nhưng vẫn TARGET P0; **OPEN** cần quyết định/evidence. Trạng thái quyết định duy nhất ở [Analysis — A01–A17](3_HUYEN_LO_DESIGN_ANALYSIS.md#art-open-decisions); phần này phân loại phạm vi Art, không sổ quyết định thứ hai.

| Nhóm được review | Kết luận sử dụng hiện tại | Phần chưa duyệt / lúc validate |
| --- | --- | --- |
| Modular player/Body-Hair-Armor-Lower-Weapon | APPROVED tách part và đồng bộ state/phase; CURRENT Kiếm | A01/A02 exact raster/socket/hybrid và reuse class OPEN |
| Logical frame / unique sprite | APPROVED phân biệt đơn vị đếm và khai báo giả định | 26 ô tổng có phép cộng thật; ý nghĩa user-lock, 33 pose/S0 OPEN |
| Default outfit/Mộc và visual progression | APPROVED default khi unequip, đúng bảy weapon visuals theo catalog, band đọc bằng silhouette/accent | Số góc/frame/cách dựng cây Kiếm/Cung OPEN; fixtures không cấp skill sai class |
| LowerBody/Boots/phụ kiện | BASELINE sáu slots, Boots/Ring/Necklace stat-only; footwear presentation thuộc LowerBody | Bỏ Boots hoặc thêm visual layer là A03 OPEN, chưa sửa catalog/stat/drop |
| Normal/skill/main VFX/impact/reaction/status | APPROVED phân trách nhiệm và đọc result đúng; CURRENT Novice/Kiếm S1 | Motif/frame exact A04/A06 OPEN; Lv10/17 dùng fixture hẹp rồi route sau |
| Multi-target/weapon snapshot | BASELINE giữ GDD Arc/Line/Spread/Explosion, đúng cây lúc cast | Cung DEFERRED prototype; logical batch đã duyệt; visual travel/AAA balance A05 cần đo |
| Mob animation / hit | APPROVED feedback không tự stun, action/death/CC ưu tiên đúng | CURRENT Nấm/Sói; 105 pose và windup exact là probe, rigs khác DEFERRED |
| Mob death/corpse | APPROVED terminal gameplay tách corpse visual, timer không theo clip | Hold/fade/flight/loot ground points và terminal event A08/A15 OPEN |
| Training Dummy | BASELINE HP60/25 s Q3/Q6 và ≥3 placements đồng thời; prototype solo | DEF/EVA/timer alternatives và online contention A07 OPEN; không giảm timer chỉ để giải chờ solo |
| Terrain/readability | APPROVED cue solid/one-way/prop/back/front và surface rõ | CURRENT room ba map; cell32/module17+4/camera A09/A12 OPEN |
| Building/structural/maps↔mob | APPROVED surfaces/route/spawn/leash/loot reachable được kiểm cùng art | Exact kit/logical lane/LoS A/B/hybrid/unreachable response A09/A10 OPEN |
| Animated environment | BASELINE flow/ripple cosmetic; shallow slowdown theo GDD, không swimming/hazard | Puddle nhỏ nếu có trong slice; thác/Bạch/full environment DEFERRED, strip counts OPEN |
| NPC | APPROVED reuse source/props và chức năng đọc bằng silhouette/text | CURRENT NPC Q1–Q6; full sprites/16–22 frames và work gestures là probe |
| VFX ngoài skill | APPROVED Freeze khác Slow, một status/target, ưu tiên telegraph | Burn/Freeze/Boss load phần sau; texture/frame count/concept OPEN |
| Items/icons | APPROVED binding tách motif/rarity/+n overlays, UI đọc được band | CURRENT items VS-1; 49/64 bindings là derived scope, 63 bitmap/size OPEN |
| Common UI Kit | APPROVED compose panel/button/slot, text Việt và pending/error đúng result | CURRENT HUD/NPC/bag/shop/quest/class; 21 functions không khóa 21 textures |
| Local/online presentation | APPROVED visual đọc session authority, không AnimationEvent damage | Harvest/feel/art/UI probe → production base → G-L mới → G-N sớm; pose/âm anticipation A11 OPEN; tentative gameplay projectile/rollback DROP P0 |
| Login/Character Select preview | TARGET P0 giữ đủ flow; DEFERRED khỏi local slice | A13 default/exact gear data dependency OPEN |
| Technical art pipeline | BASELINE canvas64/PPU32/pivot; APPROVED pose/physics tách | Exact atlas/padding/flip mechanism/package/camera cần Unity test, A01/A02/A12 |
| Accounting/production cost | APPROVED tách pose/variant/export/editor/QA/rework, đo % dùng được | CURRENT Free sample + Kiếm; S0/exact totals/hours A17 OPEN |
| Prototype matrix | Giữ đầy đủ P01–P15, CHƯA CHẠY; CURRENT chọn phần VS-1 | Bow/Boss/full online load DEFERRED, quay lại trước production branch tương ứng |

Lượt consolidation trước không chọn option gameplay; DESIGN LOCK 2026-10-03 đã sync target resolution/ba skill/control, vẫn chưa duyệt technique/count. REJECTED cho production hiện tại: vẽ hàng loạt trước G-N, lấy animation/VFX làm damage authority hoặc dùng số giả định làm nghiệm thu. Các phân tích Cung/Boss còn đủ dưới đây; DEFERRED không đổi chúng thành P1.

Reasoning từ các phát hiện ban đầu được giữ tại [Phụ lục A](#art-initial-findings).

Ưu tiên chi phí: silhouette gear, tư thế Kiếm/Cung, thời điểm release/hit, telegraph và status là core. Cắt particle thừa, idle phụ và cảnh tổng kết cầu kỳ trước khi cắt các dấu hiệu đó. Không thêm class, slot ngoài ba combat slots đã duyệt, CC, companion, swimming, hệ ánh sáng hay active R P0.

<a id="player-visual"></a>

## 1. Kiến trúc hình ảnh player

**Đọc từ:** GDD §0/§9, Technical §8. Giữ canvas 64×64, PPU 32, một cơ thể nam; không tạo full rig cho từng bộ đồ.

| Part | Sở hữu hình gì | Reuse và giới hạn |
| --- | --- | --- |
| BodyBase | Da, tay, cổ, phần cơ thể nhìn thấy; pose tay cầm/ra đòn | Locomotion chung nếu tay trung tính; Attack/Skill cần pose Kiếm/Cung riêng. Giáp không được che tay sai để giả vờ reuse |
| Hair/Head | Một mặt/tóc nền, không Helmet slot | Dùng lại hình đầu ở nhiều frame nếu góc mặt không đổi; offset theo pose. Frame đổi hướng nhìn/cúi đầu cần redraw |
| Armor | Thân áo, vai, tay áo thuộc outfit | Tay áo đi theo tay và xoay thân; một ảnh áo đứng yên không đủ cho swing/draw. Có thể tách hình tay áo trong source nhưng không thêm equipment slot |
| LowerBody | Quần **và footwear về presentation** | Chân chạy/nhảy/ngã/death cần thay pose; hai class có thể dùng cùng chân khi stance khớp. Giày stat-only không thay pixel footwear này |
| Weapon | Cây đang dùng, grip/string và vị trí trước/sau thân | Tra theo visual ID của action, không hard-code Sword_Common/Bow_Common trong Attack/Skill |

**Default outfit bắt buộc:** áo vải và quần/footwear đơn giản khi Lv1–2 hoặc unequip. Đây là fallback khi slot trống, không item mới, không stat và không tính là Band I. Q3 Quần I, Q4 Áo I phải tạo khác biệt nhìn thấy; có thể sửa màu/vạt áo trên cùng pose template, nhưng không mặc sẵn nguyên bộ I rồi claim progression đã hữu hình. Không equip weapon thì hai tay trung tính; không phát đòn giả với cây kiếm không tồn tại. Điều kiện attack khi chưa có vũ khí vẫn thuộc gameplay hiện hành, không tự thêm combat tay không.

### 1.1. Contract 26 frame thực sự đếm gì

Tám state và phép cộng `4+6+2+2+3+4+2+3=26` là dữ liệu hiện hành. **ĐỀ XUẤT A01, chưa đổi lock:** xem 26 là ô lấy mẫu của một profile; một ô có thể reuse sprite/hold hoặc chọn pose class khác. Logical sample không đồng nghĩa hình raster khác nhau. Việc duyệt nghĩa profile thay giới hạn tổng ảnh còn OPEN; timeline presentation bám clock gameplay, không bắt buộc mọi pose dài bằng nhau.

| State / ô hiện hành | Lý do đủ cho baseline | Ít hơn mất gì / nhiều hơn được gì |
| --- | --- | --- |
| Idle 4 / 6 FPS | Hai nhịp thở lên/xuống có chuyển tiếp; nhiều part có thể chỉ cần 1–2 hình | Hai hình vẫn dùng được nhưng thở dễ giật; thêm hình ít lợi ích ở camera game |
| Run 6 / 10 FPS | Hai chân × contact/passing/lift = sáu pose có khả năng đọc cadence | Bốn hình bớt chuyển trọng lượng; tám hình mượt hơn nhưng tăng chân/áo QA. Đo trượt chân theo MoveSpeed, không đổi tốc gameplay để khớp sprite |
| Jump 2 / 8 FPS | Rời đất và tư thế đi lên; hold hình thứ hai khi còn đi lên | Một hình mất dấu takeoff; thêm landing không được tự thêm action lock mới |
| Fall 2 / 8 FPS | Chuyển từ apex sang tư thế rơi; hold, không loop rung chân vô hạn | Một hình có thể đủ nhưng mất chuyển apex; landing riêng chỉ thêm nếu tiếp đất khó đọc |
| Attack 3 / 12 FPS | Kiếm: chuẩn bị → quét/hit → trả thế; Cung: kéo → release → trả thế | Hai hình làm hit/release khó đọc; bốn–sáu hình cho draw dài/arc đẹp hơn, cần kiểm timing trước tăng cost |
| Skill 4 / 12 FPS | Chuẩn bị → tụ lực → phát → trả thế; reuse cho nhập môn/tiến cảnh/đại chiêu bằng hold/VFX | Ba hình mất nhịp tụ; hơn bốn chỉ đáng làm nếu Lv17 vẫn không đọc signature sau đổi VFX |
| Hit 2 / 10 FPS | Recoil → hồi pose khi rảnh; còn flash/impact cho lúc đang action | Một hình vẫn đủ overlay; hơn hai dễ tạo cảm giác bị khóa lâu. Không mặc định Hit interrupt |
| Death 3 / 8 FPS | Mất thăng bằng → đổ → nằm; hold hình cuối | Hai hình dễ giống biến mất; thêm hình làm fall đẹp hơn, không tăng hậu quả gameplay |

**SUY RA có điều kiện:** 19 ô chung (Idle/Run/Jump/Fall/Hit/Death) + 7 ô Attack/Skill Kiếm + 7 ô Attack/Skill Cung = **33 pose cơ thể nếu tất cả khác nhau**, không phải hai rig 26×2. Tay idle cầm cung, nock và chuẩn bị Lv17 có thể cần thêm pose; 33 chưa phải trần. Ngược lại Hair/LowerBody có thể dùng ít hình hơn 33. Nếu 26 là giới hạn **hình raster toàn bộ hai class** chứ không phải profile baseline, phải trình lại trade-off; không gọi bộ 33 hình là tuân thủ nguyên văn lock cũ.

### 1.2. Ba cách làm và recommendation

Ba phương án cần đối chiếu là raster từng pose, skeletal/socket toàn thân và hybrid. Hybrid đang là phương án prototype A01/A02, chưa phải technique đã duyệt. Bảng trade-off và hướng xử lý lỗi socket/stance được giữ tại [Phụ lục A — technique](#art-technique-rationale).

**A14 — ra đòn khi chạy/nhảy/rơi:** giữ gravity và horizontal momentum đã duyệt; CURRENT thử Novice/S1, permissions skill S2/S3 còn gate. Upper action/lower locomotion cần chung phase/root/socket; không thêm movement lock hoặc animation set để cứu 26/33. Hit flash không restart action; pose đọc accepted SkillId snapshot, không mutable selectedSlot.

### 1.3. Pivot, flip và overlap

Canvas căn một mốc chân `(32,0)` theo pivot hiện hành; artwork có thể chừa 1–2 px transparent và foot anchor offset chung, không auto-crop làm đổi điểm chân. Head/hand/grip author tọa độ pixel **theo pose**, không theo bounds ảnh. Grip lệch 1 px đã dễ thấy ở cây cung mảnh.

Giữ một SortingGroup/actor để part của hai player không chen nhau. Unity xác nhận SortingGroup phù hợp nhân vật gồm nhiều SpriteRenderer chồng lấp. [Nguồn Unity — SortingGroup](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Rendering.SortingGroup.html).

Order hiện tại là baseline, không áp một thứ tự cứng cho mọi pose: Shadow 0, WeaponBack 5, Body 10, LowerBody 14, Hair/Head 20, Armor 22, WeaponFront 30. Tay cầm cung phải có phần trước cây cung; tóc/vai/vạt áo cần mask/split đúng pose. Split trước/sau là render slice của một module, không một món gear mới. Ưu tiên cutout đã author; chỉ thêm HandFront nếu kiểm grip chứng minh cần, tính thêm việc slice/QA.

**Flip có bẫy kỹ thuật:** `SpriteRenderer.flipX` chỉ đổi render, không tự mirror child socket. Đề xuất mirror VisualRoot chứa sprite/socket, giữ physics root ngoài nó, không đồng thời flipX lần hai; hoặc flipX tất cả part và mirror socket/rotation rõ ràng. Gameplay origin do server/data riêng. [Nguồn Unity — flipX](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/SpriteRenderer-flipX.html).

<a id="weapon-visual"></a>

## 2. Tiến trình hình ảnh vũ khí

**Đọc từ:** GDD §1/§6/§9. Có **bảy weapon visual**: Mộc Kiếm + ba Kiếm + ba Cung. Rarity/enhancement không nhân số bộ animation.

| Visual | Khác biệt nên thấy ở camera chơi | Phần reuse |
| --- | --- | --- |
| Mộc Kiếm | Lưỡi gỗ thô, chuôi buộc đơn giản, không sáng linh khí | Cùng grip/đường swing Kiếm, khác ảnh cây kiếm |
| Thanh Mộc Kiếm / Cung | Đường nét gọn, vật liệu gỗ/vải, lục nhạt | Motif Band I, pose class chung |
| Vân Nham Kiếm / Cung | Guard/limb chắc, accent đá/đồng, silhouette khác I | Cùng socket, pose, effect class |
| Huyền Ấn Kiếm / Cung | Dấu cổ văn, hình lưỡi/đầu cung có đặc trưng; sáng tiết chế | Cùng hit timing, pool VFX; không làm dài hitbox vì ảnh lớn |

Palette đơn thuần rẻ nhưng dễ bỏ lỡ progression. Khuyến nghị mỗi band đổi **ít nhất một dấu silhouette** (guard/lưỡi hoặc đầu limb cung) và accent; Common/Rare/Epic cùng template vẫn cùng hình, rarity đọc qua UI. Không giả mọi món cùng band là cùng stat/rarity/item; `ItemDefinition → visualId`, instance giữ stat state riêng.

| Vũ khí / phương án | Hình cần author cho mỗi variant | Vì sao / trade-off |
| --- | --- | --- |
| Kiếm socket cứng | 1 ảnh canonical có grip + transform track theo mọi pose | Nhẹ nhất; góc nghiêng có thể stair-step, không diễn foreshortening. Trail tách ảnh kiếm |
| **Kiếm hybrid đề xuất** | 1 canonical + tối đa 3 góc sửa cho anticipation/active/recovery = **1–4 hình/visual** | Ba sửa là nhu cầu góc của Attack 3 ô, không phải 26. Skill 4 ô reuse góc gần nhất; chỉ thêm nếu hand/foreshortening không đạt |
| Kiếm raster toàn bộ | Hình theo tập góc thực sự khác của Attack/Skill/locomotion | Tốt outline, đắt variant; không tự yêu cầu 26 hình nếu nhiều ô cùng góc |
| **Cung hybrid đề xuất** | Rest/nocked shape, bent draw, released recoil = **3 hình/visual** | Hai hình rest/draw bỏ recoil nhưng vẫn test được; thêm partial draw thứ tư chỉ nếu nhịp kéo không đọc. String phải theo draw, không dùng ảnh cung đứng yên |
| Mũi tên | 1 silhouette chung; tint/tip/trail theo Novice/skill | Không bảy projectile theo gear. Tên gắn trên dây có thể dùng cùng ảnh; draw hand/arrow offset là track chung |

Kịch bản hybrid đầy đủ: 4 Kiếm×4 + 3 Cung×3 = **25 ô hình vũ khí**, trong đó 7 canonical/shape nền và 18 góc/deformation thêm; 1 tên chung tính riêng. Nếu cả bốn Kiếm dùng xoay canonical tốt, chỉ cần 4+9=13; đây là khoảng **13–25** có cơ sở, không asset budget đã khóa. Nhìn đúng cây đang equip cần kiểm **mỗi visual trên Attack và các Skill hợp lệ với nó**, không chỉ Idle; không cấp skill class cho Tân Lữ chỉ để thử Mộc Kiếm.

Cast snapshot giữ visualId vũ khí lúc bắt đầu action, cùng profile/pose/timeline; equip đổi giữa action không được morph thành cây khác khi hit. Đề xuất áp visual gear mới ở biên pose/action gần nhất, còn damage action cũ giữ snapshot hiện hành. Việc có chặn equip khi cast hay không là **OPEN**, không tự thêm equip lock. Remote nhận visual snapshot/revision đủ để diễn cùng cây; không suy ngược từ damage hoặc gear mới nhất.

Skill VFX là sức mạnh phái, không baked vào ảnh cây kiếm/cung. Anchor ở grip/tip do pose track; nhập môn/tiến cảnh/đại chiêu đổi preset theo skill profile, dùng được với mọi visual hợp lệ. Vũ khí mạnh hơn vẫn đọc bằng silhouette khi effect sáng lên.

## 3. Trang bị, LowerBody và Boots

**Đọc từ:** GDD §2/§6. World visual, inventory icon và gameplay item là ba lớp riêng: một áo Rare +4 vẫn dùng visual band của template; icon thêm rarity/+4 ở UI; stat evaluator xử lý sức mạnh.

| Loại | World | Icon / gameplay | Recommendation |
| --- | --- | --- | --- |
| Armor I/II/III | Ba thiết kế áo trên pose template; redraw khi tay/thân đổi | Ba template icon, rarity và enhance ngoài ảnh | Vải → viền/miếng giáp → cổ văn; tránh áo dài che chân và bow grip |
| LowerBody I/II/III | Ba thiết kế quần **kèm footwear mỹ thuật** | Ba item Quần, không thêm slot footwear | Đổi viền/gối/cạp và footwear theo band; chân chạy/nhảy phải theo pose |
| Ring / Necklace | Không world sprite | Sáu template icons, stat và tooltip | Giữ stat-only; vẽ trên body 44–48 px khó đọc, ít lợi ích so cost |
| Boots | Không world sprite; footwear do LowerBody trình bày | Ba template icons, DEF/EVA/tốc chạy và enhance/Tinh Hoa | **Giữ slot stat-only P0**, tooltip mô tả đây là slot chỉ số; đừng hứa footwear world khớp item Boots |

Armor có thể giữ cùng silhouette gốc nhưng cần accent band đủ nhìn; LowerBody phải khác ở vùng không bị áo che. Đổi outfit không thay collider, shadow footprint hay range. Không sản xuất 18 full rigs, không sản xuất hình cho mỗi mức +.

### 3.1. Nếu bỏ Boots thì chuyện gì thay đổi

Baseline vẫn là sáu slot; Boots stat-only và footwear mỹ thuật thuộc LowerBody. Bỏ Boots hoặc cho Boots đổi world visual là hai option A03 còn OPEN. Xem [Phụ lục A — Boots](#art-boots-rationale) để tra đủ tác động lên catalog, chỉ số, loot, shop và UI trước khi đề xuất thay đổi.

<a id="combat-visual"></a>

## 4. Ngôn ngữ hình ảnh combat — CORE

**Đọc từ:** GDD §3/§4/§9. Người chơi cần biết ai ra đòn, cây gì, phạm vi nào, lúc nào trúng, có trạng thái gì. VFX đẹp mà sai thông tin này là lỗi core.

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

### 4.1. Framework Lv1 → Lv20

Concept đề xuất dựa lore **linh mạch/Mạch Ấn**: Kiếm là nét khắc ấn ấm, gọn và dứt; Cung là đường linh khí lạnh căng qua dây. Có thể thay thành linh ảnh/phong/hỏa/băng khác sau test, nhưng phải giữ shape và timing hiện hành. Chưa khóa phải có thú, băng cầu hoặc kiếm khí dạng projectile.

| Mốc / action | Hình tối thiểu và progression | Gameplay-critical / phần có thể cắt |
| --- | --- | --- |
| Lv1–2 | Movement/outfit mặc định; không fake skill trước Q6 | Đọc player/NPC/platform; bụi chân phụ có thể bỏ |
| Q3/Tân Lữ normal | Mộc Kiếm thật, swing ba pose, một dấu quét ngắn và impact tại target | Hit moment/weapon là core; trail dài/sparks là polish |
| Kiếm S1 sau class | Single Phong Trảm giữ cue riêng, reuse Attack/Skill pose hợp lệ | Không dựng class Normal executor hoặc zero-MP fallback |
| Cung S1 sau class | Draw/release + logical single/visual arrow | Không gap normal Cung executor: action class là S1; visual không damage |
| Lv5 Phong Trảm | Cùng cây kiếm, dấu linh ấm ở release; nét chém rõ hơn normal nhưng chỉ một target 1,7 u | Một nhịp cast/shape riêng; không vẽ quét rộng ám chỉ ba con đều nhận damage |
| Lv5 Linh Tiễn | Cùng cây cung draw/release; tên có tip/trail linh khí mảnh, khác tên thường | Logical single result; một visual arrow, không nhiều bóng tên giả |
| Lv10 Phong Trảm | Dấu quét 120° liên tục, accent Mạch Ấn; tối đa ba impact ở hit moment | Sweep và multi-target feedback core; không ba explosion lớn |
| Lv10 Linh Tiễn | Một lần kéo/phóng, ba nhánh tên đồng motif; cùng logical resolve +0,12 s | Ba visual arrows; đừng diễn thành ba cast kéo cung. A/A/A vẫn ba tên, status một roll/unique target |
| Lv13 nội tại | Icon/tooltip mở, hit đủ điều kiện có accent nhỏ tùy chọn | Không thêm aura liên tục hay skill slot; không làm accent thành proc gameplay mới |
| Lv17 Kiếm Khí | Chuẩn bị/tụ rõ hơn, đường ấn 5,5 u, width 0,6 u mở trong một release; tối đa năm impact | Signature bằng silhouette/nhịp/âm sắc; không cần projectile server mới cho Line |
| Lv17 Hàn Tiễn | Một tên mạnh, arrival rõ, nổ lạnh radius 2 u; primary tâm và secondary cùng nhịp nổ | Logical primary + explosion authority; flash tâm ngắn, vành nổ gọn. Không thêm tên phụ gây damage |
| Lv18–20 | Gear III và signature giữ nhận diện, status theo loại target | Không tự thêm evolution Lv20 hoặc tăng effect vô hạn theo enhancement |

Lv5 khác normal bằng pose tụ ngắn, release accent, tip/shape và impact cùng motif; không chỉ đổi màu. Lv10 tăng độ rộng/số nhánh cùng thông tin đánh lan. Lv17 tạo “wow” bằng **anticipation rõ → một hình signature lớn nhưng rỗng tâm → kết thúc sạch**, thay vì giữ màn hình trắng. Không kéo action lock để lấy chỗ diễn nếu chưa duyệt timing.

Khởi điểm authoring: slash/wave **4 hình** (mở–active–co–tan), burst **5 hình** (arrival–mở–vành–vỡ–tan), impact **3 hình** (bật–tách–mất). Ít hơn mất hướng/nhịp hoặc thành nhấp nháy; thêm frame chỉ giúp decay mượt, không thêm hit. Dùng lại strip bằng scale/tint/rotation và duration theo profile; không nhân bảy weapon visuals. Đây là dải probe chất lượng, không bộ sprite đã khóa.

## 5. Multi-target: cùng action, đúng thời điểm

**Authority:** session gameplay chọn/resolve actual targets, geometry và Evade/Crit/status; SpriteRenderer/ParticleSystem đọc result. Không gameplay projectile collision hoặc arrival callback sửa HP.

| Trường hợp | Presentation | Mốc result |
| --- | --- | --- |
| Single | Một cast/đường/impact đúng target; NÉ không wound impact | +0,12 s S1; no target reject không pose/cost giả |
| Arc3 | Một sweep + tối đa ba landed impacts cùng phase | Authority hit moment, không truyền A→B→C |
| Line5 | Đường mở theo tuyến, impacts đúng intersections | Cùng resolve; falloff gần→xa, không focus priority làm sai power |
| Hàn1+4 | Primary cue + vòng lạnh radius2; primary không second impact damage | Cùng +0,18 s; invalid primary không explosion, primary Evade vẫn secondary rolls |
| Spread ABC/ABA/AAA | Một fan release, ba visual hit indices; A/A/A vẫn ba tia | Cùng +0,12 s; invalid index mất hit, no reacquire; status một roll/unique landed target |

Travel/streak duration/aim là presentation probe, dẫn tới result clock hoặc dùng streak ngắn; không trừ HP rồi đợi tên bay dài mới hiện feedback. Source/visual/SkillId snapshot cố định suốt action; đổi selectedSlot/gear không morph action đang chạy. A05 không còn option giữ gameplay flight; còn visual truthfulness/vertical/Bow AAA balance gate. Secondary geometry giữ Arc/Line/Spread/Explosion, không nearest AoE hóa.

Arc/sweep là tự nhiên với Kiếm; fan projectile tự nhiên với Cung; AoE vòng dùng cho explosion. Link cực mảnh có thể thử như **nét Mạch Ấn** ở cùng tick landed results, không chạy chuyền qua từng target. Mặc định bỏ link: thêm đường dễ bị đọc thành chain skill, trong khi impact và sweep đã giải thích đủ. Nếu giữ, không bám target chưa hit/đã invalid, không phủ kín đội hình.

Giảm spam theo camera: một main VFX/cast, một impact nhỏ/landed target, một status instance/type/target. Giữ projectile nguy hiểm và Boss telegraph; giảm hạt/trail/shake của remote trước. Không che kết quả bằng dồn mọi impact vào primary; không dùng thời gian decay effect làm trì hoãn HP/damage feedback. Chạy 2/4+ player và trường hợp 5 target; cap cosmetic phải đo, không giới hạn world ở bốn người.

<a id="mob-visual"></a>

## 6. Mob: animation theo hành vi

**Đọc từ:** GDD §4; bảy identities/sáu rigs giữ nguyên, Sói Trúc reuse Sói Sương bằng palette/name. Không ép 26 frame player lên mob. Bảng là **kịch bản pose khởi điểm**, chưa assets và chưa lock frame count.

| Base rig / hành vi | Idle | Move / Flying | Melee | Ranged | Hit rảnh | Death | Tổng hình nếu các ô mới đều khác |
| --- | ---: | --- | ---: | ---: | ---: | ---: | ---: |
| Nấm Linh, melee chậm 1,2 u/s | 2 | 4 co/nẩy/thân đi/lấy lại thế | 4 | — | 1 | 3 | 14 |
| Sói Sương, chase 2,4 u/s | 2 | 6, hai chân × contact/passing/lift | 4 | — | 1 | 4 | 17 |
| Ong Giáp, ranged/flying 2,0 u/s | 4 hover/wing | Reuse 4 hover, offset/tilt lúc move; **0 hình thêm** | — | 4 | 1 | 4 | 13 |
| Đoạt Mạch Đạo Tặc, hybrid nhanh 2,2 u/s | 2 | 6 bước hai chân | 4 | 4 | 1 | 4 | 21 |
| Xích Thạch Linh, hybrid nặng 1,4 u/s | 2 | 4 đặt chân/chuyển trọng lượng | 4 | 4 | 1 | 4 | 19 |
| Cổ Môn Vệ Binh, hybrid 1,8 u/s | 2 | 6 bước và áo/giáp chuyển | 4 | 4 | 1 | 4 | 21 |
| Sói Trúc Ảnh | Reuse | Reuse | Reuse | — | Reuse | Reuse | **0 pose mới**, một palette identity |

**SUY RA:** 14+17+13+21+19+21 = **105 hình rig trong kịch bản này**, không bảy lần cùng một animation count. Nếu ranged recovery reuse idle hoặc Hit chỉ flash, số hình thực giảm; cần ghi pose map, không tự giảm bảng mà vẫn claim tất cả ô unique.

Lý do bốn ô attack: nhận thế/aim → windup silhouette → hit/release → recovery. Ba ô vẫn được nếu aim đọc từ facing/hold; hai ô dễ mất báo trước, nhất là ranged. Thêm frame chỉ làm chuyển động mượt, không tăng attack rate. Wolf cần stride/lunge đọc hơn Nấm; Stone bốn move đủ tạo sức nặng; Ong không cần bộ đi bộ hoặc melee không có gameplay. Wing bốn pose có thể loop nhanh hơn thân (probe 12–16 FPS), nhưng release đọc bằng thân/dấu phát đạn, không theo nhịp cánh.

Nấm Death ba ô: xẹp→đổ→bẹp. Sói bốn: gục đầu→khuỵu→đổ→nằm; bớt một chuyển vẫn có thể pass. Đạo Tặc/Cổ Vệ bốn cho trọng lượng thân người/giáp; Thạch bốn cho nứt→rụng→sụp→tàn, không debris physics. Ong bốn cho mất wing→rơi presentation→chạm/rụng→tàn; không bắt chước corpse Sói nằm giữa không khí. Hit một hình chỉ dùng lúc rảnh; flash/impact cho mọi action ở §7.

Hybrid mobs cần **pose ranged**, dù dùng cùng Linh Đạn generic: Đạo Tặc phóng/ném; Thạch tụ/phóng mảnh linh lực; Cổ Vệ đưa vũ khí/ấn phát. Đó là khác gesture, không ba projectile families mới. Đạn generic visual reuse tint/scale/trail; tốc 5/4/6 và Ong 5,5 u/s chỉ là control travel visual từ baseline cũ, không clock damage. Roster giữ ba hybrid đến prototype/content approval 3/1/0, không trừ poses trước duyệt. Idle 2 thay 4 tiết kiệm vì thở không phải core; thêm work flourish không phục vụ mob AI P0.

### 6.1. Boss là dependency art bắt buộc ngoài bảng sáu rig

GDD có một Cự Thú, basic + Nham Trảo + Địa Chấn + ba vùng Nham Thạch Rơi + Cuồng Mạch. Không được bỏ Boss khỏi scope chỉ vì checklist nhấn normal mob. Kịch bản: Idle4 + Move6 + Basic/Claw4 dùng chung motion + Slam4 + Cast4 + Death6 = **28 hình**, thêm Roar4 **tùy chọn** =32. Move6 chỉ cần nếu reposition có đi; bốn frame death sẽ rẻ hơn nhưng thân lớn sụp dễ thiếu trọng lượng, sáu là probe có lý do. Claw khác basic bằng telegraph/config, chỉ redraw nếu silhouette không phân biệt được.

Canvas Boss **OPEN** theo kích thước world cần đánh/né; giữ PPU32, thử 128/192 px thay vì phóng ảnh 64px thành khối thô. 28 frame Boss không ngang cost 28 frame player: diện tích, cleanup và telegraph QA lớn hơn. Ba telegraph shape: cone, ground AoE, landing zones; vùng đá dùng cùng shape ba lần, không ba asset riêng. Địa Chấn phải báo vùng nhảy né, landing zones không ám chỉ double-hit overlap.

Cuồng Mạch tint/glow/roar feedback theo ngưỡng server; không chen Roar animation làm ngắt action/telegraph hiện tại hoặc thêm stun/lock. Nếu không có khoảng rảnh để roar, dùng accent/âm thanh và giữ pose chính. Boss Slow giữ telegraph/action đã start nguyên tốc độ; Dư Ảnh chỉ đổi tên per viewer, không rig/entity/phase mới.

## 7. Mob nhận damage khi đang action

**Đọc từ:** GDD §3/§4; damage không đồng nghĩa stun. Reaction là lớp presentation có ưu tiên: **Death > CC authoritative > action đang chạy > locomotion**, impact/flash có thể chồng lên lớp chính.

| State lúc bị hit | Main animation | Feedback đề xuất | Không được làm |
| --- | --- | --- | --- |
| Idle | Có thể dùng Hit1 rồi về Idle phase hợp lệ | Impact/flash; recoil visual nhỏ nếu không che facing | Tự thêm cooldown/immobility |
| Move/Chase | Tiếp tục move và Run phase | Flash + impact; offset sprite nhẹ, nhanh về 0 | Reset stride mỗi hit, dừng movement hoặc đẩy physics root |
| Melee windup/Attack | Giữ windup/hit/recovery và facing khóa | Impact + flash; mặc định bỏ recoil nếu làm lệch telegraph | Attack→Hit→Attack restart, trễ hit hoặc bỏ hit vì client đổi animation |
| Ranged aim/release | Giữ aim pose, hand/socket và projectile spawn phase | Flash/impact không dời muzzle | Spawn thêm đạn khi animation quay lại release; tự retarget theo attacker |
| Recovery | Giữ recovery đến mốc authoritative | Recoil visual chỉ nếu không che return pose | Cấp đòn kế sớm vì Hit clip ngắn |
| Frozen thật | Cancel pending action theo server; giữ pose ngắt dưới overlay băng | Freeze entry/tan; hết CC về state mới của server | Resume windup cũ rồi phát pending hit đã bị hủy |
| Boss/PvP Slow | Main action giữ nguyên thời gian | Lam mờ/status icon theo expiry | Dùng Hit/ice-block hoặc giảm animation tốc độ action |
| HP0/Death | Terminal death pose, clear status theo server | Hit cuối gọn dẫn vào Death, bỏ Hit dài | Cố phát nốt hit/spawn chưa giải quyết hoặc đợi Hit clip xong mới chết |

Flash nên ngắn, probe 60–100 ms; recoil 1–2 px/60–120 ms chỉ trên visual và khi không làm sai hướng đòn. Ít hơn khó thấy ở tốc chơi, nhiều hơn giống stun và làm nhấp nháy nhiều người; các số này **NEED VALIDATION**, không hit-stop gameplay. Hitstop/crit shake/material audio DEFERRED, không requirement CURRENT. Với packet gộp nhiều hit, debounce flash cường độ nhưng vẫn giữ từng result đúng; không reset action clock theo số hit.

## 8. Mob death lifecycle và corpse

**Đọc từ:** GDD §4/§6, Technical §6/§7. Respawn normal/Linh hiện tính **deathUtc+25 s**, không tính từ lúc corpse tan hoặc DB ACK. Body không gây contact damage/blocking hiện hành; corpse càng không được tạo chướng ngại mới.

**Giả định review A15 cho bảng/ví dụ dưới:** đặt `t0` ở lethal HP0 và giả định `deathUtc=t0` để thử ACK trễ; chưa khóa API/event/timestamp capture hoặc thời điểm release Linh cap ở terminal-pending. Invariant đã có là không reward/respawn trước finalize, và corpse không reset deadline. [Technical gate](2_HUYEN_LO_TECHNICAL.md#pending-ordering-probes) phải kiểm/chốt các chi tiết này trước production lifecycle; presentation terminal trước ACK vẫn là proposal.

| Mốc | Gameplay / persistence | Presentation đề xuất |
| --- | --- | --- |
| Hit cuối/HP0 tại t0 server | Terminal-pending; loại khỏi target/hurtbox queries, dừng AI/threat action, cancel pending hit/spawn; chụp ledger/deathID/generation | Last impact ngắn dẫn thẳng Death; bỏ Hit clip dài, clear status đang phủ thân. Phải có terminal state server, không client tự đoán chết |
| Chờ commit | Không nhận hit, không reward/loot/quest success và không respawn; payload giữ bất biến | Có thể chạy Death theo terminal state; đây là **đề xuất bổ sung event presentation** cho Technical, không publish reward sớm. Lỗi kéo dài dùng thông báo gián đoạn chung |
| ACK death transaction | Finalize reward/loot/credit một lần; deadlines vẫn từ t0 | Loot hiện ở điểm death hợp lệ, không chờ Death animation. Banner Boss/quest success chỉ sau ACK |
| Death clip xong | Entity gameplay terminal; corpse chỉ là hình có lifetime riêng | Ground giữ frame cuối; flying dùng nhánh rơi/tàn phía dưới. Không hurtbox/AI ở frame corpse |
| Hold/fade | Không đổi deadline, contribution, pickup ownership | Probe ground: clip khoảng 0,35–0,60 s, hold 0,5–1,5 s, fade 0,3–0,5 s; rồi trả visual pool |
| Slot tới hạn | Spawn life mới nếu death đã finalize và map lifecycle cho phép; ID/generation mới | Không reuse corpse như mob sống bằng bật hurtbox trước reset. Old callback không gắn vào life mới |

Ví dụ kiểm: Death4 ở 8 FPS =0,5 s; hold1 s + fade0,35 s → corpse biến mất **t0+1,85 s**, respawn vẫn t0+25 s. Hold ngắn hơn giúp bãi sạch nhưng mất dấu đã hạ mob; lâu hơn giúp nhìn kết quả nhưng tăng clutter khi nhiều người farm. Boss có thể hold 2–4 s để thấy kết thúc, **OPEN**, không đổi respawn 15 phút/pile90 s. Không bắt số này bằng người chơi phải chờ animation.

ACK tới t0+30 s thì deadline 25 s đã qua: chỉ được spawn sau finalize theo rule due-slot, không đặt lại 25 s từ ACK và cũng không spawn trước commit. Nếu loot đã mất phần lifetime khi ACK trễ, không tự reset owner/expiry windows bằng animation; đó là dependency reliability cần QA, không art tự bù. Nếu technical muốn định nghĩa deathUtc khác t0, phải chốt/sync rõ trước implementation.

**Flying death:** phương án rẻ là mất wing→trượt/rơi **visual-only** tới nền gần hợp lệ rồi tàn; fallback tan tại chỗ nếu dưới là hố/khác tầng. Không thêm Rigidbody corpse, collision loot hoặc hurtbox rơi qua player. Vị trí loot do server author/resolve điểm pickup đứng được; nếu hiện chưa có ground projection, ghi **OPEN A08**, không client raycast tự đổi vị trí loot đáng tin. Test Ong chết trên cầu, mép vực, giữa các tầng; loot không nằm ở điểm Kiếm không thể tới.

**Player khác mob:** death camera/corpse phải còn tới revive/về làng; không áp auto-fade/despawn normal lên player. Corpse Boss contributor ≥10% còn trong area vẫn hợp lệ theo GDD, không pickup tới khi sống. [Recovery gameplay](1_HUYEN_LO_GDD.md#consumables-death) quyết vị trí corpse/SafeAnchor; trạng thái corpse art không quyết định quest eligibility. PvPDefeated dùng kết thúc trận, không mở lựa chọn Hồi Sinh Phù/PvE death.

## 9. Bù Nhìn dùng chung tutorial và training

**Đọc từ:** Q3/Q6 và Analysis farm matrix. Hiện Dummy HP60, không đánh/không thưởng, ≥3 placements cùng lúc, respawn25 s **BASELINE / TEST-TUNABLE** (số hiện dùng cần đo, chưa đổi thành timer nhanh); Q3 hạ ba life với ≥20% đóng góp/life, Q6 cast active tại yard. Ba placements cùng pool/rig/credit/lifecycle, không ba identity hoặc dummy tutorial/training logic khác. Q3 không bắt chờ respawn nếu solo yard còn đủ ba life; online contention vẫn cần probe.

| Nhu cầu | Kiểm baseline | Recommendation / trade-off |
| --- | --- | --- |
| Q3 Lv3/Mộc Kiếm | ATK suy ra: 12+1,2×2+0,7×4+10=27,2; HP60 khoảng ba landed normal nếu DEF thấp. DEF/EVA dummy chưa ghi đủ | Giữ HP60 cho probe Q3; author rõ DEF/EVA, không lấy ngầm từ mob bất kỳ. TTK tùy miss/DEF, chưa claim chính xác |
| Quest ba kills | Một slot hồi25 s gây ít nhất hai quãng chờ sau các kill, dù combat ngắn | Thử **một prefab/loại**, 3 điểm đứng trong cùng yard và respawn3–5 s; số ba để ba objective kills có sẵn. Đây là thay authoring/timer cần duyệt, không baseline mới |
| Player level cao quay lại | Lv20 Common III cân bằng ATK91,6 có thể one-shot60; không đủ test nhiều action/status liên tục | Đủ xem per-hit damage và đúng weapon/cast; không hứa DPS Meter hay test rotation dài P0. Không tự tăng HP làm Q3 dài |
| N player cùng đánh | Một shared HP/life và ledger; one-shot của người mạnh có thể chiếm toàn life; Q3 không dựa last hit | Server serialize damage/death/respawn, credit từng requester ≥20%; HUD không báo đã hạ nếu chỉ nhìn người khác đánh. Nhiều slot/respawn nhanh giảm chờ, không đảm bảo mọi requester credit cùng life |

**Visual tối thiểu:** Idle1 (dáng tĩnh), Hit2 (cong→bật lại), Break3 (nứt→gãy→đống rơm), dùng frame cuối làm corpse = **6 hình nếu đều khác**. Ít hơn Break dễ giống despawn, hơn hai Hit chỉ làm lắc mượt không dạy mechanic mới. Dummy không Run/Attack; Hit không dừng server action của player. Repeated hit reset/cộng lắc phải clamp, không lắc tới che body/HP bar. Có HP/name, damage/Crit/NÉ cùng pipeline; Death/Break terminal rồi respawn như life mới, không reward vật phẩm/EXP.

**OPEN A07:** (a) giữ60/25 s nguyên trạng rẻ nhất nhưng contention; (b) giữ60, một pool nhiều standpoints và timer nhanh — recommendation để test; (c) adaptive HP theo player hiện diện/life hoặc cá nhân hóa — phức tạp, thay ledger/counter, không nên P0; (d) HP lớn cố định — test mạnh dễ hơn nhưng hại Q3. Prototype hành trình Q3/Q6, late quest và một người mạnh cùng yard trước đổi timer. Muốn rotation test sâu dùng fixture dev HP cao trong validation, không thêm loại dummy player-facing.

<a id="art-timing"></a>

## 10. Timing presentation và vai trò skill

Bảng timing/probe, phép so tốc độ Kiếm/Cung, occupancy Novice/skill, sustain Lv5/10/20 và proc cadence đã chuyển **đầy đủ** sang [Analysis — timing](3_HUYEN_LO_DESIGN_ANALYSIS.md#art-combat-timing-evidence), [sustain](3_HUYEN_LO_DESIGN_ANALYSIS.md#art-sustain-evidence) và [proc](3_HUYEN_LO_DESIGN_ANALYSIS.md#art-proc-evidence). Nhãn DERIVED/PROPOSAL, giả định và mọi số giữ nguyên; GDD §3 vẫn là luật. Không dùng các ranges đó làm acceptance mới.

Art author **anticipation → release/hit mốc authority → recovery**, phân biệt interval/CD/action lock/flight/VFX decay. Attack3/Skill4 không chạy FPS đều rồi ép timeline gameplay chạy theo: ánh xạ duration/hold từng pose để release khớp clock; VFX được tan sau khi actor về Idle. Phần recovery được chuyển ở action boundary hợp lệ; bỏ frame không queue thêm hit. CURRENT kiểm Tân Lữ/Kiếm/Lv5, phần S2/S3 dùng fixture hẹp khi cần xác minh reuse; Cung draw/release và visual travel DEFERRED prototype, không xóa nhu cầu test.

Basic chỉ Tân Lữ; sau class không có zero-MP Normal để lấp CD. S1 single efficiency, S2 group farm, S3 burst; artwork/icon có thể reuse motif/pose nhưng ba entries và six SkillIds độc lập. Legacy occupancy/sustain tables ở Analysis giữ nhãn lịch sử. Proc overlay thử bằng fixture, không đổi chance cho dễ thấy. Chạy lại rotation/resource/TTK trước retune.

<a id="map-visual"></a>

## 11. Kiến trúc visual map

**Đọc từ:** GDD §4/§9, Technical §2/§8. Năm farm + ba support roots là **tám layout**, không tám tileset mới. Forest/Mountain/Ancient là ba họ chất liệu, vẫn phải có landmark/route phân biệt từng map.

| Nhóm | Nội dung/đơn vị production | Unity/presentation và reuse |
| --- | --- | --- |
| Background | Silhouette xa, trời/núi/rừng/phế tích; mảng nền theo family | Sprite lớn hoặc BackgroundTilemap không collider; giảm contrast, reuse palette/crop. Parallax phụ nếu camera cần, không framework riêng |
| Terrain Kit | Ground cap/fill/edge/corner/one-way theo ngữ nghĩa | Tilemap collider và platform tách; topology dùng chung, texture ba họ khác |
| Decoration | Cỏ/trúc/đá vụn/cột đổ/biển đường | Sprite/prefab cụm; đặt sparse trong combat lanes, không collider vô cớ |
| Structural/Full Assets | Cầu/mái/cổng/tầng phế tích/lò rèn/ấn | Một visual nhiều mảng nhưng chỉ vài collider surfaces; §13 |
| Animated Environment | Thác/nước/lửa/khói/lá/bụi | Sprite loop/animated tile/ParticleSystem/static overlay theo §14 |
| Foreground | Cành, mỏm đá viền camera, lớp nước trước chân | Riêng layer/order và vùng occlusion; không che telegraph/name/loot |

| Map | Dấu hình ảnh phải phục vụ layout hiện có | Reuse tiết kiệm |
| --- | --- | --- |
| Đồng Sương | Đồng thoáng, safe strip, ledge1–2 tầng, bãi Sói thấy đường về | Forest grass/soil, background thưa |
| Trúc Ảnh | Cầu2–3 tuyến cao độ, broken seal dưới cầu; lối xuống thật nhìn được | Forest đổi trúc/palette, mini bridge kit |
| Bạch Vân | Ba terrace, waterfall landmark, route vòng nhìn nối nhau | Mountain terrain lạnh, thác một họ effect |
| Xích Nham | Hai nhánh nhập, ngoại vi/deep route, ba seal và Huyền Môn ngoài cổng | Mountain ấm; cùng seal base ba anchors, không ba hệ puzzle |
| Huyền Tích | Phế tích nhiều surface, Boss clearing và normal spawn exclusion rõ | Ancient + stone structure kit; landmark không xây Boss scene mới |
| Vân Khê/Học Viện/Lôi Đài | NPC services, ledge/drop tutorial, yard, sàn đấu và lối ra | Hub architectural props dùng lại; độ sáng/props phân khu, không ba bộ art độc lập |

SafeAnchor, entrance6–8 u và MapExit approach phải có mặt đứng/đường rõ; không decorate thành bãi nguy hiểm. Kích thước root theo actual layout, không nhân background bằng offset200 u. Một palette environment không đủ thay gameplay room/placement/QA.

## 12. Terrain readability và tile variants

| Ngữ nghĩa | Cue khi nhìn hình | Collider / kiểm |
| --- | --- | --- |
| Solid ground | Mặt trên cap liên tục, khối fill có trọng lượng, edge đậm hơn background | Solid, không đổi một tile giống hệt thành pass-through tùy chỗ |
| Cliff/step | Mép/corner khép, mặt đứng tách mặt trên; cao độ theo block rõ | Step thật, không trang trí gờ nhỏ khiến tưởng đứng được |
| One-way/platform | Mặt sàn mỏng, chân trống hoặc dầm mảnh; edge top đọc rõ | One-way từ dưới, DropThrough xuống riêng actor; hint tutorial lấy glyph binding hiện hành |
| Pass-through prop | Ít contrast/outline khép, chân không nối solid cap; texture nền | Không collider; tránh rock foreground đậm cùng hình rock solid |
| Overpass/cầu | Sàn trên rõ, khoảng đi dưới có chiều cao và route hai đầu; cột không giả blocked tunnel | Có surface trên + đường dưới nếu author; không cần hệ depth lane mới |
| Background | Saturation/contrast thấp, silhouette mềm, không cap standable | Không physics; rìa background không trùng rìa ground |
| Foreground | Crop/occlusion viền, ít chi tiết vùng action; không làm giả platform | Không collider; hở silhouette actor/telegraph/loot |

Một vật liệu có thể xuất hiện ở solid và background **nếu hình cue khác rõ**: cap/edge/contrast và chân nối nền khác, không chỉ opacity5% mà mắt thường không thấy. Kiểm grayscale, nền sáng/tối và khi Freeze/Burn/Linh aura bật; không dựa chỉ màu đỏ/xanh để phân loại.

**Kịch bản Terrain Kit khởi điểm, OPEN A09:** ô32×32 ở PPU32 =1 u; platform có thể sprite trong ô nhưng mặt ván chỉ8–12 px. Player64 px không suy ra tile64 px. 32 là probe để người cao44–48 px đứng trên ledge dễ đọc; 16px chi tiết hơn nhưng tăng placement/corners, 64px coarse với các bước nhỏ. Chọn sau room test, không đổi PPU theo family.

| Tập module topology | Số hình trong kịch bản | Vì sao cần / nếu cắt |
| --- | ---: | --- |
| Solid3×3: fill1, edges4, outer corners4 | 9 | Đủ khối chữ nhật liên tục; thiếu corner làm mép không khép |
| Inner corners | 4 | Hốc/routes concave; nếu map không có hốc có thể hoãn, không vẽ rồi không dùng |
| Isolated block | 1 | Block độc lập đọc được; nếu tất cả nền ≥2 ô dày có thể không cần |
| One-way left/mid/right | 3 | Hai mép + lặp sàn; mirror có thể giảm hình mới nếu vật liệu đối xứng |
| Cosmetic alternatives: top2, fill2 | 4 | Bớt pattern lặp trên đường dài; không đổi collision meaning |
| **Tổng/họ chất liệu** | **17 semantic +4 cosmetic=21** | Ba family tối đa63 hình terrain nếu vẽ từng họ; cùng topology/data, chưa cam kết mỗi họ cần đủ variant |

Không gọi đây là autotile47 đầy đủ: staircase dùng block, chưa yêu cầu slopes/moving platform/hazard. Cột mỏng, cap đặc biệt hoặc asset lớn có thể cần module riêng nếu room test chứng minh; ghi tăng scope thay vì nhét collider lệch vào hình cũ. Tile variants cosmetic không tự tạo collider mới.

## 13. Building/structural là không gian gameplay

**Đọc từ:** bridge/vertical route Trúc, terraces Bạch, ruin/Boss Huyền, AI hybrid/flying. Công trình đứng được là dependency physics/combat, không chỉ decoration.

| Cách dựng | Dùng khi | Trade-off / recommendation |
| --- | --- | --- |
| Terrain Kit | Bậc đá, nền/ruin tường đơn giản dùng cùng chất liệu | Rẻ và collider nhất quán; motif kiến trúc bị phẳng nếu ép mọi mái/cổng thành tile |
| Mini Building Kit | Cầu/mái/dầm/cột hoặc vòm lặp ở nhiều vị trí | Một bộ nhỏ cap/mid/support theo grid; đủ reuse, không hệ building procedural/placement của player |
| Full Asset | Landmark độc nhất, lò rèn, Huyền Môn, phế tích silhouette lớn | Vẽ đẹp theo layout; vẫn chia back/front và surfaces, không collider theo từng chi tiết ảnh |

Một mái nhà nhỏ có thể có mặt đứng và exploration route nếu nối được bằng jump hiện hành; phần dưới vẫn là prop/pass-through có cue. Một ruin lớn có nhiều sàn, cầu/mái/cổng, mob spawn và combat: art author **surface map** riêng gồm ID/mặt trên/cao độ/solid hay one-way/route nối/spawn/leash/Bow line-of-sight. Full asset không có nghĩa một polygon collider toàn bộ cửa sổ/gạch/vines.

Collider đề xuất: vài rectangle/edge được làm sạch trên physics root, solid nền + one-way sàn riêng; trang trí khung cửa/cột/vòm sau/trước không collider nếu route không cần. Giữ landing lip khớp mặt pixel, loại khe collider làm chân mắc. Ground tile tiếp tục Composite Operation Merge; one-way dùng PlatformEffector2D và drop-through theo actor hiện hành. [Nguồn Unity — TilemapCollider2D](https://docs.unity3d.com/6000.3/Documentation/Manual/tilemaps/work-with-tilemaps/tilemap-collider-2d-reference.html), [PlatformEffector2D](https://docs.unity3d.com/6000.3/Documentation/Manual/2d-physics/effectors/platform-effector-2d-reference.html).

Ground mob logical movement trên lane/home bounds authored; không navigation graph/DropLink/jump/drop AI. Surfaces vẫn phục vụ player collision/route/spawns. Mọi quest encounter phải có đường player tiếp cận thật; không thiết kế layout cần mob parkour rồi thêm universal ranged fallback. Hybrid roster/LoS A-B/unreachable response là prototype riêng, không art khóa gameplay mask/immune.

Mini kit giả định gỗ gồm bridge-floor3 +roof3 +support2 +wall/arch2 =**10 logical modules**, **8 hình** nếu hai cặp cap trái/phải thật sự mirror được. Nếu motif không đối xứng thì10 hình. Đây là ví dụ derivation, không bắt phải có đủ roof trên mọi map; stone kit có thể dùng terrain hoặc một bộ cap/mid/support riêng sau route test. Landmark độc nhất không ép thành kit mười loại chỉ để “modular”.

Test một công trình: lower lane và upper standable surface, một spawn đúng platform, đường tiếp cận Kiếm, so LoS A/B qua solid roof với cue đọc được, mob Return không mắc cột, người drop-through không làm người khác rơi. Khoảng trống Boss15 u TEST và telegraph tránh bị mặt sàn foreground che.

<a id="mock-map-ui"></a>

Các sơ đồ, tọa độ và capture của bản mẫu được quản lý riêng tại [Roadmap — hồ sơ prototype](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#prototype-map-history).

## 14. Animated environment

**Đọc từ:** Bạch waterfall, map family/foreground và scope không swimming/hazard. Nước P0 là presentation trên mặt đi được khi không có gameplay requirement riêng.

| Thành phần | Cách đề xuất | Frame/primitive khởi điểm và reasoning |
| --- | --- | --- |
| Waterfall top | Sprite loop riêng ở mép nguồn | 4 hình crest đổi hướng/nhịp; hai hình dễ nhấp nháy, hơn bốn chỉ thêm mượt |
| Waterfall fall | Animated tile/strip repeat dọc, phase ổn định | 4 hình flow streak; không vẽ toàn thác cao mỗi frame. Test seam và không làm pattern như climbable ladder |
| Waterfall splash | Sprite loop ở chân + hạt thưa nếu cần | 4 hình mở/đổi/tan/trở lại; reuse ripple motif, không emitter dày che terrain |
| Stream/water | Static base + flow overlay hoặc animated tile4 | Base giữ palette/mặt nền; loop4 chỉ cho phần có dòng thấy rõ, puddle tĩnh không cần cùng loop |
| Puddle | **WaterBase + FrontOverlay**, base static1, front static1 | Base sau chân, front chỉ che phần chân thấp. Hai lớp không hai vật lý nước; ít hơn một lớp mất cảm giác chân trong nước |
| Actor ripple/splash | Một strip3 hoặc4 hình spawn theo bước/contact presentation | Một prefab chung; ba hình mở/lan/tan, thêm một cho decay. Có throttle local/remote theo camera, không mọi network transform tick |
| Lá rơi | ParticleSystem với1–2 leaf sprites | Random vị trí/lifetime, tránh giả từng tile có animation; hai silhouette bớt lặp, nhiều lá không thêm gameplay |
| Khói | ParticleSystem1 soft/pixel puff, hoặc sprite loop4 cho nguồn rõ | Forge/fire reuse puff, ít emitter. Không lighting framework |
| Bụi | ParticleSystem1 puff hoặc ripple/impact strip reuse | Chân/mob nặng/đổ dùng chung texture; cắt ở crowd trước core feedback |
| Lửa/ánh sáng | Sprite flame loop4 + static emissive-looking accent | Reuse flame với Burn có chỉnh scale/tint; không buộc Dynamic Light/URP2D lighting nếu chưa cần |

Thác ba strip×4 =**12 ô chuyển động** theo kịch bản, không chiều cao thác×12. Top/fall/splash không nhất thiết cùng hình dù dùng cùng clock; fall đặt sau actor nếu route phía trước, không làm lớp nước opaque ngang mặt. Stream4 và ripple3–4 chỉ thêm nếu layout có chức năng visual đó. Foreground nước đứng yên vẫn có thể đẹp bằng base/accent, không phải mọi puddle đều loop.

Contact ripple chỉ cosmetic; tốc độ lội nước theo feet contact của GDD, không thêm CC/Burn/HP hazard; pooling reset phase/scale/owner. Actor đi trên cầu phía trên nước không splash vì chỉ chồng hình2D; cần surface/contact tag presentation đúng lane. P0 không deep-water, breath, buoyancy/swimming. Nếu thác là background, không vẽ ledge giả hoặc trigger tương tác không có luật.

## 15. Map ↔ mob ↔ vũ khí

| Ca gameplay hiện có | Failure mode visual/route | Hợp đồng authoring và validation |
| --- | --- | --- |
| Spawn platform/công trình | Chân ở giữa không khí, slot trên sàn không tới được | Spawn anchor trên surface hợp lệ, clearance đủ body/HP bar, hurtbox không xuyên sàn; identity/level giữ manifest |
| Chase/Return | Mob rơi tầng dưới rồi teleport lên, hoặc mắc dầm/cột | Route cùng lane/linked surfaces có hành vi AI hỗ trợ; leash8 u và spawn reachable. Return không dùng đường ảnh tưởng có mà collider chặn |
| Melee Kiếm | Cao độ khiến range1,2/1,7 u không chạm, ngay cả nhìn gần | Kiểm vertical hit band/originY+0,8 u với geometry; không dời hitbox theo trail để “sửa art” |
| Cung/ranged hybrid | Visual xuyên roof có thể trái LoS mode của prototype | A không LoS so B SolidWall, one-way không chặn trong B; chưa production lock. Visual chỉ đọc logical result, không collision damage |
| Flying Ong | Hover box6×3 u khiến ở cao ngoài tầm Kiếm vô hạn | Engage approach vào melee-accessible band như GDD; room có route/jump thật, không yêu cầu Kiếm có skill mới để tới |
| Multi-floor | Target distance gần nhưng khác tầng, target lock/hit xuyên đá | Authority MapId/line/arc/logical range/vertical validation; foreground và platforms đọc được khoảng cách thực |
| Linh scale1,20–1,30 | Sprite overlap roof, người hiểu hurtbox lớn hơn trong khi physics không scale | Aura/name/HP bar chính, scale vừa phải theo clearance. Nếu scale mờ pixel, thử tint/accent trước đổi physics |
| Pocket spacing | Cầu nối khiến aggro5 u kéo cả map hoặc leash cắt route | Kiểm tâm18–20 u/terrain tách, safe strip, line/arc grouping; không ép mọi pocket thành blob0,8–1,5 u |
| Loot sau flying death | Món nằm trên ledge kín hoặc trên không | Server-owned điểm pickup đứng được; collider route/1,5 u pickup test cùng lifecycle §8 |

Không phải mọi mob đều đi mọi tầng của công trình: author phạm vi hợp lệ và thể hiện bằng vị trí/spawn/route, ba hybrid giữ tới content approval, không tự bỏ ranged hoặc thêm ranged cho mọi loài. Nhưng mọi encounter bắt buộc quest phải có cách Kiếm và Cung tiếp cận/đánh/nhặt bằng bộ kỹ năng hiện hành; không dùng effect đẹp che softlock. Không thêm body blocking, knockback, hazard, moving platforms hoặc navigation framework trước khi room prototype chứng minh cần.

<a id="npc-visual"></a>

## 16. Tám NPC

**Đọc từ:** GDD §5/§9. Menu/quest làm NPC có chức năng; không cần tám animation set như player. Đề xuất **full sprite NPC** chia sẻ pose template/palette/props trong source; không dùng module gear runtime nếu không có thay outfit. Full sprite dễ giữ silhouette nghề nghiệp; modular runtime tiết kiệm ít nhưng tăng controller/sorting QA.

| NPC | Idle khởi điểm | Work/gesture và props | Recommendation |
| --- | --- | --- | --- |
| Lâm Bá | 2 hình thở nhẹ | Gậy/ghế hoặc áo nghề; nói bằng text/marker | Không gesture riêng P0; dáng ấm và khác thầy class |
| Yên Thảo | 2 | Bàn thuốc/giỏ, tùy chọn2 hình đưa/chỉ thuốc | Prop tĩnh đủ; chỉ vẽ gesture nếu tương tác khó đọc |
| Bách Luyện | 2 | Lò/đe/búa; tùy chọn4 hình nâng→đập→recoil→nghỉ | Work loop đáng ưu tiên nhất nếu forge nhìn rõ; đừng play đập liên tục trong lúc modal enhance chờ ACK |
| Mộc An | 2 | Rương/ghế nghỉ dùng prop chung | Không sleeping/heal rig riêng; heal feedback player dùng pool |
| Tạ Minh | 2 | Bàn/ấn Huyền Môn motif dùng lại | Silhouette trang nghiêm + UI class choice; không cinematic/ritual set |
| Phong Du | 2 | Kiếm/giá binh khí, stance Kiếm | Không attack-demo loop vì Q3 dạy bằng player/dummy; nếu cần pose minh họa reuse template player |
| Diệp Lam | 2 | Cung/giá binh khí, stance Cung | Không draw-release set đầy đủ khi không có objective biểu diễn |
| Hạo Vũ | 2 | Cờ/sàn tỷ thí motif Arena | Menu/invite/countdown là tín hiệu chính; không bộ đánh nhau NPC |

**SUY RA:** tám NPC×2 idle =**16 hình** nếu mỗi NPC khác silhouette. Work Bách+4, gesture Yên+2 tùy chọn →**22**, không 8×26. Idle2 đủ một nhịp thở nhẹ ở2–4 FPS/hold; một hình tiết kiệm nhưng hub tĩnh, bốn hình mượt hơn nhưng không đổi service readability. Props tĩnh tính riêng theo motif thực; không tám portrait bắt buộc, dialogue có thể dùng tên + sprite trong world. NPC không combat/death rig nếu gameplay không cho đánh.

## 17. VFX ngoài active skill

**Impact** = ngắn tại hit; **Status** = tồn tại theo gameplay state/expiry. Một hit băng không đồng nghĩa target Đóng Băng. Reuse tint không được xóa khác biệt silhouette/lifetime giữa Freeze và Slow.

| Feedback | Presentation tối thiểu | Reuse / giới hạn |
| --- | --- | --- |
| Bỏng | Flame/viền ấm nhỏ dưới thân hoặc cạnh target; tick feedback nhỏ mỗi1 s | Một instance/target; loop4 dùng lại flame môi trường khi hợp palette. Refresh không phát lại explosion |
| Freeze normal/Linh | Ice silhouette ôm target, trạng thái đứng yên rõ; crack/thaw khi hết | Probe2 static shapes shell/crack +strip3 thaw; scale theo visual bounds nhưng không collider. Không che aura/name. Shell ít hơn vẫn test được; nhiều shard chủ yếu polish |
| Slow Boss/PvP | Lam mờ/1 cold particle motif, icon/expiry khi cần | Không ice-block; không slow current cast animation; một overlay refresh deadline |
| Linh Biến aura | Vòng/ấn tím bên chân/nameplate, nhẹ và bền | Một sigil sprite +rotation/pulse presentation; cùng mọi rig, khác palette Sói. Non-integer scale phải test pixel fidelity |
| Boss Cuồng Mạch | Accent nóng/viền, âm/roar tùy chọn; giữ telegraph | Reuse aura primitive có palette/shape cue phù hợp, không overlay tím làm nhầm Linh |
| Heal | Hạt/nhịp ấm xanh, HP number/bar đổi sau xác nhận | Một puff/spark primitive, không animation cơ thể riêng; Food ticks không emit full burst2 s/lần |
| MP restore | Hạt lạnh/lam, MP feedback riêng | Reuse heal motion/texture, khác glyph/palette/vị trí; chỉ sau consume ACK |
| Food/buff | Icon item Food đang active +duration; entry feedback ngắn khi use thành công | Không aura liên tục, không thêm buff R P0. Three foods reuse HUD binding |
| Enhance success | Sigil/glint quanh item preview, kết quả cấp/stat rõ | ACK rồi play; cùng mọi item/+level. Một glint primitive, không scene riêng |
| Enhance fail | Crack/dim ngắn trên panel, thông báo giữ cấp và chi phí đã tiêu | Không vỡ item sprite/giảm cấp; network error khác RNG fail, pending không giả kết quả |
| Transfer | Arrow/flow trong UI, success cùng glint sau commit | Không item chuyển giao mới; source mất thể hiện bằng preview/result, không effect tự consume |
| Death/Revive | Death tint/pose; hồi sinh tại chỗ nhịp sáng/viền miễn thương2 s đúng server | Particle tan chung với mob; invulnerability cue reuse Hồi Sinh motif, không tự kéo dài. PvPDefeated riêng flow |
| Loot/Gold | Shared item icon/pile +prompt quyền nhặt; Gold coin burst chỉ cho recipient đã ACK | Một glint/beam rỗng tiết chế; không coin vật lý pickup khi Gold auto-credit, không beam giả personal loot |
| Crit | Font weight/scale/glyph khác normal, impact accent nhỏ | Reuse damage renderer; không tăng hit-stop mọi remote crit |
| NÉ | Text/glyph rõ, không wound impact hoặc proc overlay | Có thể vẫn thấy đường đòn đi qua; damage0, không flash target như đã landed |

Priority khi overlap: Boss telegraph/projectile nguy hiểm → state/action silhouette → local hit/status → loot/UI → remote cosmetic → môi trường. Beneficial và status không bật mảng opaque che boss markers. Freeze trước Death phải bị clear khi server terminal; aura Linh còn corpse chỉ nếu decay cosmetic không khiến tưởng mob còn sống. Pool reset color/material/owner/expiry/action/life IDs để Burn cũ không gắn vào mob respawn.

<a id="icons-ui"></a>

## 18. Items và icons

**Đọc từ:** catalog/source/manual GDD §3/§6/§7. Đếm **binding/icon outcome**, rồi tách motif vẽ mới; không nhân rarity×enhancement×template.

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
| Active profiles | 6=Kiếm/Cung×Lv5/10/17 | Hai motif class với độ mở/rune khác theo milestone; Lv5/10/17 là ba SkillIds/slot riêng, reuse motif icon nếu vẫn phân biệt được |
| Passives | 4 | Kiếm Tâm/Kiếm Thế/Ưng Nhãn/Xạ Tâm cần cue khiên/gần/mắt/xa; derive hai motif class, không thêm tree/rank |
| Status HUD | 4 loại presentation: Burn/Freeze/Slow/Food | Ba glyph status mới; Food dùng lại icon Food đang active. Freeze/Slow không chung một hình ice-block |
| Virtual quest feedback | Dấu Trọc Khí, Vật Chứng, Mảnh1/2/3:5 binding nếu dùng hình | Reuse sigil/ấn/glyph +ordinal/text; không năm inventory items. Có thể dùng text/glyph chung thay unique bitmap |

**SUY RA:** gear22 +Food3 +Potion6 +phù2 +stone1 +materials8 +ingot1 +manual6 =**49 physical ItemDefinition icon bindings**. Thêm Gold1 →50; active6 +passive4 →60; status4 →**64 nhóm binding cơ sở**, trong đó Food status trỏ ảnh item đã có nên tối đa **63 ảnh phân biệt** ở kịch bản xuất đầy đủ, chưa tính glyph kit/UI/virtual feedback. Đây không phải63 motif vẽ từ đầu; rarity/frame/+n là overlay reuse. Revive invulnerability/Linh/Boss có thể dùng icon item/sigil sẵn, nếu quyết định thêm badge riêng phải ghi delta, không âm thầm tăng con số64.

Nguồn icon dự kiến32×32 transparent, hiển thị scale nguyên và tooltip lớn khi cần, **OPEN** theo HUD/reference resolution. Không lấy PPU world để quyết pixel UI. Inventory30/storage40 dùng cùng slot primitive; stack count/+level/rarity/locked/quest-bound hiển thị text/glyph, không bake vào từng PNG. Icon màu band không được giống rarity border đến mức nhầm band III=Epic.

## 19. Common UI Kit và các view

**Đọc từ:** GDD §6/§8/§9, Technical §9. Inventory/Storage/Shop là bố cục và binding dùng chung, không ba bitmap screen. Chọn một style panel/outline/spacing/text; kit không đòi mỗi trạng thái một ảnh mới.

| Primitive / số thứ tự để đếm kit | Công dụng | States cần có; reuse |
| --- | --- | --- |
| 1 Panel/frame | HUD/popup/đối thoại | Nine-slice một style nền/frame; kích thước do layout |
| 2 Button | Confirm/use/cancel | Normal/hover/pressed/disabled; tint/border/offset, không bốn ảnh bắt buộc |
| 3 Item slot | Bag/storage/shop | Empty/filled/selected/unavailable/pending; icon +overlays |
| 4 Equipment slot | Character/equip picker | Reuse3 +slot glyph; empty/filled/off-class/level-lock |
| 5 Skill slot | HUD/panel | Reuse3 +locked/cooldown/manual-missing/pending |
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
| Character | Layout sáu slots, silhouette/preview rig hiện tại, nhóm attributes/stat | Kit/equipment glyph; không portrait bitmap cho mỗi outfit |
| Enhance | Vùng preview trước/sau, hai mốc Tinh Hoa khóa/mở, success/fail accent | Item icons/cost/result kit; không lò rèn fullscreen |
| Transfer | Hai item/source-consumed/target-before-after và flow arrow | Kit/arrow glyph; không art mới từng pair item |
| Skill | Ba active, hai passive; selected slot/CD riêng và manual/level lock lý do | Mười profile/passive icons §18, rows/tooltips; không talent tree |
| Quest | State/group/count/return NPC/level gate; Q9 optional tách pin | Rows/markers; evidence virtual, không drag-drop quest item |
| Journey | Score và category/record tổng hợp đúng hệ thống hiện có | List/progress/title; optional emblem dùng motif Mạch Ấn |
| PvP | Invite/opponent picker, exact wager/pot/fee, pending escrow, countdown/120 s/quota, result/pending settle | Two columns +badges/countdown numbers; không portrait/arena splash bắt buộc |
| Login/Character Select | Logo/title giản dị, list và selected preview, feedback kết nối/lỗi | Kit và rig; dependency §21 |
| Stage Summary | Ba title/chương, reward/progression/story completion text | Một modal template, ba accent/thumb crop map sẵn; không ba tranh full-screen bắt buộc |
| Inventory/Storage/Shop/Sell | Grid/list, capacity30/40, stack count/ownership, price/stock state | Cùng kit; Storage không item background riêng |
| Class choice/manual use/attributes reset | Choice cards +reason/preview/confirm | Hai class motifs/kit; không thêm tạo nhân vật hoặc menu học riêng NPC |
| Dialogue/Rest/Death | NPC text/action, confirm heal/death choices khác PvP | Panel/buttons; Hồi Sinh icon, không tám portraits bắt buộc |
| HUD/world UI/chat/map exits | Bars, quest tracker, food/bình, focus marker/mini HP + screen name/level/current-max HP, reward tooltip, bubble hai dòng, signpost, Boss timer/banner | Kit/typography/marker glyphs; edge arrow + destination name/NPC marker P0; không HP bars Party hoặc quest navigation xuyên map P0 |

Ba skill entries/class dùng selected/locked/CD/MP cues; Tân Lữ entry 1 Mộc Kiếm, 2/3 khóa. Input/pending/cancel theo [GDD §3](1_HUYEN_LO_GDD.md#pending-cast); Art chỉ thể hiện cue “Đang tiếp cận” và reason blocked/quá xa/chưa sẵn. Không hiện accepted cast animation trước validation/commit. Accepted action presentation bind snapshotted SkillId, không selection mới.

**Navigation/controls/readability probe:** edge exit thường dùng arrow ở mép + tên đích khi tới gần, không vẽ teleport arch hoặc E prompt. Huyền Môn/Arena có SpecialGate cue khác; NPC marker cơ bản đủ thấy nơi nhận/trả. Quest arrow xuyên map vẫn P1. Tutorial glyph lấy Jump/DropThrough/UseFood/UsePotion/Interact binding, không bake S+Space/F/M vào dialogue/art. Default panels I/C/Q theo GDD; skill view trong C, không B/K/L overrides từ prototype.

**UI usability gate P11:** dùng kit panel/button/slot/tooltip/list hiện có để review ở tốc độ thường: tìm món/equip/bán sample không nhầm, nhận/trả quest và next action dễ hiểu, dismiss modal không lọt attack, pending approach có thể hủy rõ, locked skill/exit có reason; thử cả lỗi full bag/đầy HP/sai NPC. Log do dự/misclick/số bước/giờ sửa và nhận xét người chơi; không khóa ngưỡng thời gian hoặc gọi text-only automation là UX PASS.

**Movement/mob feel presentation:** probe jump tap/hold/apex/landing/coyote/drop với rig và camera thật, pose không lái gravity; soft separation/reposition phải đọc vị trí Sói, không telegraph lệch do visual steering. Không tự thêm animation set, knockback, ring slots hoặc pose budget; exact movement/AI thuộc GDD/Technical/PHY-01.
 World focus marker+mini HP và screen name/level/HP current-max/bar dùng cùng target life; không portrait/element/rarity/generic buff panel. Loot candidate highlight/E prompt độc lập, nhặt ngay không đổi CombatFocus.

Pending là core online: khóa thao tác lặp cần thiết, giữ dữ liệu committed, cho biết đang xử lý; timeout/network error không dùng success/fail RNG animation. Equip preview có thể xem trước visual, nhưng HUD/world nhận canonical result đúng revision. Success enhance kể cả fail cost đã tiêu chỉ sau ACK, không kéo animation giả để che một request chưa có kết quả.

**Dependency bổ sung:** font có đủ dấu tiếng Việt, số/+/% dễ đọc; test tên dài “Đóng Băng”, “Huyền Nham Cự Thú”, trạng thái thiếu X ô, wager10.000 và chat80 ký tự/hai dòng. Text dynamic không bake thành sprite. Chọn world reference resolution/zoom và UI scale **OPEN A12** trước export: thử480×270 hoặc640×360 cho16:9 (1920×1080 scale4 hoặc3); world view rộng15 hoặc20 u ở PPU32, ảnh hưởng đọc Bow6,5 u/telegraph. Đây là probe camera, không requirement render resolution đã khóa hay scene mới.

<a id="online-presentation"></a>

## 20. Presentation online server-authoritative

**Đọc từ:** GDD §8, Technical §1/§3/§4/§6. Local anticipation cosmetic không mở thêm prediction/rollback physics/combat P0. Backend không nằm trên mỗi hit, nhưng reward/consume/enhance/PvP settle cần durable ACK.

Bảng flow dưới là **phương án presentation để thử A11**, không toàn bộ specification đã duyệt. “Ngay frame input” là mục tiêu probe anticipation cosmetic; dùng state/result authoritative là invariant đã chấp nhận. Exact immediate pose/âm anticipation cần đo ở G-N/P12, không điều kiện pass cứng; không thêm tentative projectile branch. Local slice và Dedicated đều đọc result từ authority; flow server/remote này dành cho TARGET online.

| Bước | Local player | Game Server / remote |
| --- | --- | --- |
| Bấm Attack/Skill | Ngay frame input: đổi pose chuẩn bị/draw, weapon đúng visual, âm/tụ lực cosmetic; ghi action request đang chờ | Intent có binding/sequence/aim; client không gửi trusted targets/damage |
| Validate/start | Giữ UI resource/cooldown canonical, có pending cue nhẹ khi cần; không hiện target impact/HP trừ giả | Server kiểm alive/MapId/class/learned/profile/MP/CD/action lock, chụp source/action origin/weapon visual và startClock, commit cost realtime |
| Accept/reject | Ghép request với actionId, căn phase theo server clock; reject trả pose phù hợp và reason, clear cosmetic anticipation | Remote bắt đầu từ phase còn hiệu lực, không chạy lại windup từ đầu ở packet trễ |
| Release/projectile | Local slash/cast accent có thể anticipate cosmetic; khuyến nghị chờ authoritative spawn để tạo projectile chính trong slice | Authority schedule logical hit; visual event có action/hitIndex/life/origin/aim/travel phase để render |
| Hit/multi-target | Chỉ HitResult mới damage/Crit/NÉ/landed impact; giữ cùng timestamp đối với batch arc/line/nổ | Authority target/geometry validation resolve actual IDs, per-target result/status; remote thấy cùng outcome, không VFX overlap damage |
| CC/Death | Present cancel/freeze/terminal đúng state, bỏ pending hit visual của action bị hủy | Server generation/cancel reason/expiry; unresolved action cancel; visual không gameplay callback |
| Loot/consume/progression | UI pending, success/pile/reward/HP potion effect theo ACK quy định | Game Server tính result, backend commit bền vững; remote/pickup đúng phase/receipt |

Nếu latency làm visual projectile xuất hiện muộn, render ở phase travel hiện tại và nối muzzle trail ngắn để đọc nguồn; không phát một tên thứ hai từ pose release. A11 chỉ probe pose/âm anticipation cosmetic; tentative gameplay projectile và rollback **DROP P0**, không tạo projectile prediction branch. Cung không cần biến mọi visual arrow thành NetworkObject riêng nếu event/state mô tả travel đủ; cách transport là Technical spike, art không chốt implementation.

**Data/clock proposal** về correlation/action/life/MapId/profile/visual revision, projectile flight, hit resolveTime và status/terminal fields đã chuyển đầy đủ sang [Technical — presentation data](2_HUYEN_LO_TECHNICAL.md#presentation-data). Schema vẫn OPEN A11/A15. Art cần state/phase/result để diễn và dedup, không sở hữu message schema hay quyết server tick/transport.

Dedup VFX theo actionId/projectileIndex/hitIndex/targetGeneration thích hợp; cache status theo state revision, không replay entry trên mọi snapshot. Death terminal của life cũ thắng Hit/Status đến trễ; mob respawn không nhận effect life trước. Join/reconnect snapshot dựng status còn hiệu lực, không replay damage/reward; map transition clear visuals của map cũ, visual map cũ được dọn, không giữ callback gameplay. Một actor disconnected trong grace15 s vẫn bị đánh; không biến ghost render thành invulnerable. Map filter chỉ presentation, không disable server root/player khác.

Ở2–4+ player: giữ unique target/arrow/status rules, tránh tốn VFX theo N×mọi khả năng thay vì events thực. WorldUI priority local/selected/Boss, status một instance/target; giảm remote cosmetic ở crowd. Đo frame time/overdraw/allocations/bytes và latency; bốn player là probe, acceptance hiện tối thiểu hai, không capacity claim.

## 21. Login và Character Select: dependency visual

**HIỆN HÀNH:** Boot/Main Menu → Login → Character Select → connecting overlay → World. Không Register/forgot password/server browser/Loading Scene riêng/Character Create bắt buộc.

| View/state | Art/UI cần | Dependency cần tránh scope thừa |
| --- | --- | --- |
| Boot/Main Menu | Title/logo chữ, background crop environment, Start/Exit khi flow dùng | Không cinematic hoặc key art bắt buộc; reuse kit |
| Login | Username/password masked, focus/submit/disabled/pending, lỗi dễ hiểu | Admin provisioning/account rules ở GDD/Technical; không UI admin trong file art |
| Character Select | List name/level/class; selected highlight, confirm/back, lỗi list/lease/connect | List hiện chỉ đảm bảo name/level/class. Preview gear **không được giả định backend đã trả equipment** |
| Selected preview | Option rẻ: rig Idle default và class motif; option đủ gear: canonical equipment visual summary read-only | Recommendation default preview +text cho P0 baseline; nếu muốn exact gear, bổ sung dependency read-only summary Technical, không client-state authority |
| Connecting/reconnect | Overlay/progress indeterminate, message và retry/back phù hợp state | Không báo “đã vào World” trước join; không hiện token/ticket/backend thuật ngữ cho player |
| Failure/return | Không có nhân vật/đang trong phiên/không thể kết nối, reason đúng luồng | Không tự thêm nút tạo nhân vật, cấp account hay recovery gameplay mới |

Cụm “Recommendation default preview +text cho P0 baseline” trong bảng trên là **option đề xuất A13**, chưa chọn giữa default và exact gear. Baseline chắc chắn hiện có là list name/level/class; không suy backend đã trả equipment.

Một rig preview reuse Body/Hair/outfit/weapon assets, không sprite-sheet riêng màn chọn nhân vật. Không claim cây gear thật nếu chỉ nhận class. Khi data thiếu, default preview có nhãn cấp/phái đủ, tránh placeholder fake Rare III làm người chơi tưởng được cấp đồ. Login dependency là UI states/font/input/focus/background và preview source, không thiết kế lại ticket/lease/account schema.

<a id="art-integration"></a>

## 22. Technical Art Contract — baseline và phần cần kiểm

**Owner:** Art sở hữu contract visual/import; Technical §8 giữ cách tích hợp runtime/physics. Bảng sau giữ đầy đủ baseline/proposals trước review; đọc cùng phân loại §0. Canvas/PPU/pivot theo GDD, các lựa chọn exact technique/import/camera chưa qua prototype vẫn NEED VALIDATION; importer production chưa được dựng.

| Contract | Giữ / đề xuất | Căn cứ và cách kiểm |
| --- | --- | --- |
| Player canvas/body | Giữ64×64, body44–48 px, hướng phải | Vừa silhouette nhân vật; weapon/VFX có thể renderer riêng vượt canvas, không scale body để nhét |
| PPU | Giữ32 toàn world sprite; icon/UI theo UI scale riêng | Body≈1,375–1,5 u hợp collider cao1,45 TUNABLE;64px canvas=2 u không phải hurtbox |
| Pivot/alignment | GiữBottom-Center(0,5;0); mốc chân/offset chung trên grid pixel | Không crop mỗi pose; part cùng pose key, đường chân không nhảy khi equip |
| Padding/transparency | Alpha thật; probe margin transparent1–2 px nếu không làm đổi anchor; sheet gutter2–4 px ngoài cell; atlas padding4 px khởi điểm | Margin/gutter/atlas padding là ba việc khác nhau. Canvas64 không bị cộng thêm gutter vào sprite rect |
| Palette | Swatch chung da/outline, ba gear/environment families; skill ấm/lạnh/Linh tím có cue hình riêng | Không hard-lock số màu trước test; outline/contrast và band/rarity phân biệt. Không shader palette system bắt buộc |
| Sheet | Mỗi state/profile có grid cell cố định; manifest `poseKey → spriteRef/socket/order/duration` | Shared spriteRef hợp lệ; duplicate sheet cells không tính thành hình vẽ mới. Sheet layout không quyết định damage |
| FPS/duration | Giữ preview baseline6/10/8/8/12/12/10/8 theo player state; timeline duration riêng | Attack release/hit đúng GDD; mob loops chọn theo cadence/hành vi, không ép cùng FPS/frame count |
| Naming | Ví dụ `Player_Male_Body_Sword_Attack_p02`, `Weapon_Sword_BandII_angle01`, `Mob_Wolf_Move_p03`, `FX_Freeze_Thaw_p01` | Identifier kỹ thuật ASCII/stable, tên display tiếng Việt; index pose khác hitIndex/animation state |
| Folders đề xuất | `Art/Characters/Player/{Base,Outfits,Weapons}`, `Mobs`, `NPCs`, `Art/Environment/{Forest,Mountain,Ancient,Shared}`, `Art/FX`, `Art/UI/{Kit,Icons}` | Source editable/sheet/manifest và imported sprite refs phân biệt; không folder riêng mỗi rarity/+level. Chưa tạo các folder |
| World sorting | Background →back props/terrain →actors →WaterFront/occluding trim →CombatReadable →WorldUI | Full structure chia Back/Front/surfaces; opaque front tránh lanes. Telegraph/critical VFX nằm ngoài actor SortingGroup khi cần phủ world |
| Actor order | Shadow0/WeaponBack5/Body10/Lower14/Head20/Armor22/WeaponFront30 là baseline; per-pose override/split | Một actor SortingGroup; body/hand/weapon overlap test hai hướng. FX con trong group không tự vượt group khác/foreground |
| Sockets | Foot/Head/Grip/Muzzle/Tip theo pose, tọa độ pixel; mirror vị trí/góc nhất quán | Socket visual không gameplay hit origin; shape/hurtbox vẫn server data |
| Frame sync | Một state/phase clock/actor, class profile và pose mapping chọn đồng bộ mọi part | Không Animator clock độc lập; swap gear đọc pose hiện tại, không restart Idle/Attack |
| Flip/scale | Mirror VisualRoot **hoặc** flipX +explicit socket mirror, không cả hai; physics root giữscale1 | Linh scale chỉ visual; non-integer scale/rotation cần test pixel. Không mirror text/worldUI |
| Pooling | Presentation-only pools cho impact/projectile renderer/status/telegraph/feedback/environment thưa | Reset timers/listeners/color/material/phase/parent/action/life/MapId; cancel callbacks cũ; không pool quyết gameplay lifetime |
| Sprite import | Point, Compression None cho nguồn pixel baseline, no mipmaps, alpha, full rect/modular cell cố định; disable read/write nếu không cần | Không max-size downscale sheet; atlas settings/platform overrides phải kiểm riêng, không chỉ texture nguồn |
| Atlas | Khởi điểm padding 4 px; UI tắt packing rotation; pixel/module QA cân nhắc tắt tight packing để dễ kiểm | Padding chống bleed; packing không đổi pivot/pose semantics |
| Camera | Asset PPU32, integer output scaling/render snapping nếu pipeline đã chọn hỗ trợ | Không snap physics/server positions; test camera/interpolation/Cinemachine và screen ratios trước khóa resolution |

Unity hướng dẫn cùng PPU, Point filter và Compression None cho sprite pixel; atlas có padding mặc định4 và setting rotation/texture riêng. Đây là cơ sở kiểm import, **không tự pin phiên bản Unity/URP**; spike pin Editor/package thực dùng theo Technical. [Nguồn Unity — chuẩn bị sprite pixel](https://docs.unity.com/en-us/engine/6000.6/manual/unity2d/2d-urp/2d-pixelperfect/prep-sprites), [Sprite Atlas reference](https://docs.unity3d.com/6000.3/Documentation/Manual/sprite/atlas/sprite-atlas-reference.html).

ObjectPool cung cấp cơ chế reuse object, còn reset/dedup/generation ở trên là contract đề xuất của Huyền Lộ, không engine tự bảo đảm. [Nguồn Unity — ObjectPool](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Pool.ObjectPool_1.html). Không thêm lighting stack, Addressables hay skeletal package chỉ để đạt contract này.

Pipeline và phép đếm trước review nằm tại [Phụ lục C — pipeline](#legacy-visual-flow) / [accounting cũ](#legacy-art-accounting). Contract đang dùng ở §22; các anchor lịch sử vẫn được giữ.

<a id="first-art-probe"></a>

### 22.1. First Art Probe — Sword01

**Mục tiêu:** mở Unity và kiểm một nhân vật Kiếm với thay đồ/hai hướng trước khi vẽ cả catalog. Đây là subset để thử rig; không tự đổi contract 26 frame hoặc count outcomes trong accounting.

1. Chuẩn bị tám module: BodyBase, Hair, DefaultArmor, DefaultLower, ArmorI, LowerI, Mộc Kiếm, Kiếm I. Dùng cùng canvas/palette/mốc chân; module chưa có dùng placeholder có nhãn.
2. Vẽ theo thứ tự `Idle_p00`, `Run_p00`, `Jump_p00`, `Attack_p00` (chuẩn bị), `Attack_p01` (release), `Attack_p02` (trả thế), `Skill_p00` (tụ), `Skill_p01` (phát). Giữ spriteRef dùng lại nếu hợp lệ; đây là tám pose probe, không full animation set mới.
3. Xuất PNG alpha thật, cell cố định 64×64 cho body/parts, weapon renderer riêng khi cần. Đặt tên theo §22; ghi rõ module/pose thiếu, không dùng `final2/fix_final`.
4. Import Sprite, PPU32/Point/Compression None/no mipmap; slice theo grid cell, Bottom-Center chung, kiểm max-size/platform override không làm nhỏ sheet. Tạo test rig/prefab nhỏ từ refs thật; physics root tách visual.
5. Gắn pose/socket hai phương án ở mục dưới; chạy Idle/Run/Jump/Attack/S1, flip, unequip/mix outfit và gear swap khi action đang chạy. Dùng cùng authority clock hiện có, không AnimationEvent damage.
6. Ghi manifest refs dùng lại/ảnh mới, thời gian source→export→socket→import→QA và tỷ lệ output dùng được. Pass khi DoD §24 đạt; nếu fail, sửa đúng grip/pivot/pose/import trước mass production. A01/A02/A12 phải có kết quả/quyết định trước nhân rig.

### 22.2. File lifecycle và pose/socket data

**Đường đi của file:** editable source ở `ArtSource/Probes/Sword01/` → PNG export vào `prototypes/VS1_EndToEnd/Assets/_Prototype/VS1_EndToEnd/ArtProbe/Sword01/` → Unity import tạo sprite refs + `.meta` → definition/prefab tham chiếu refs. PNG export chính là imported asset; không chép thêm một bản PNG “runtime”. Cache Library không version. Khi production base đã duyệt, chuyển asset được chọn sang `game/Assets/GameArt/`, giữ GUID khi move và đưa refs vào presentation definition; prototype source không tự thành production architecture.

Mỗi export có stable asset ID, source path và revision. Sửa source rồi export đè đúng file; retain `.meta` khi rename/move. Sheet layout đổi thì kiểm lại slice refs, không chỉ tên PNG. Font/license và file gốc của asset ngoài được giữ cùng nguồn.

**Minimum pose manifest — presentation only:** `PoseKey`, `SpriteRefsByLayer`, `Duration/phase mapping`, `Grip`, `Muzzle`, `WeaponOrder`; thêm `Foot/Head/Tip` hoặc `HeadOffset` khi pose cần. Missing optional socket có fallback được ghi rõ; poseKey và spriteRef có thể reuse. Không có damage, MP, range, cooldown, gameplay hitbox hoặc authority hit moment trong manifest.

**Socket probe A02:** so hai phương án trên cùng tám pose/hai hướng: (A) pose data lưu điểm/góc pixel rồi runtime đặt weapon; (B) author Transform socket trong rig/prefab rồi export bảng theo poseKey. Chọn một nguồn cuối, không sửa cả Transform và data độc lập. Trong mẫu A, tọa độ canvas gốc trái-dưới: điểm `(u,v)` đổi local thành `((u−32)/32,v/32)` theo Bottom-Center/PPU32; exporter từ gốc trái-trên phải đổi Y một lần. Weapon grip offset/góc và front/back per pose ghi cùng entry; mirror cả điểm/góc đúng một lần ở VisualRoot. P01/P02 kiểm tay không trượt, nock/release đọc đúng, equip không restart clip. Chưa pass/chọn phương án thì chưa nhân catalog.

### 22.3. Visual language — mẫu đọc trong 30 giây

Status: **style sample để kiểm**, dùng các baseline canvas/palette/readability hiện hành; chưa khóa technique/camera mới.

- Character: body 44–48 px trong cell64, silhouette/hand-grip ưu tiên. Thử outline tối 1 px, ít mức sáng/tối và một hướng sáng thống nhất; không để palette gear che tay/vũ khí.
- Environment: lane/edge đứng được tương phản rõ; background giảm contrast, foreground không che silhouette/telegraph. Cùng vật liệu vẫn phân biệt solid và one-way bằng cạnh/shape.
- Kiếm/Cung: chém ấm, cung lạnh, Linh tím kèm hình/nhịp riêng; cue release/impact gọn và ăn clock. Không chỉ dùng màu để phân Freeze/Slow.
- UI: panel dùng kit chung, text Việt đọc ở camera/output thực; selected focus và disabled/error khác nhau. Icon/slot/tooltip hỗ trợ thông tin, không thay bằng màn bitmap hoặc bắt click để chơi.

### 22.4. Provenance tối thiểu

Một record cho mỗi nguồn/pack với asset IDs liên quan: URL hoặc source hash, author/tool, license text/file, phạm vi sử dụng đã kiểm, modified/from-source revision, ngày lấy và người kiểm. Asset tự vẽ ghi tác giả; AI ghi tool/model nếu biết và reference nguồn. Thiếu thông tin thì đánh dấu chưa kiểm và dùng placeholder, không tính vào số asset sẵn sàng phát hành. Kiểm quyền tại lúc dùng/mua; không suy toàn pack có cùng quyền từ một ảnh mẫu.

<a id="production-accounting"></a>

## 23. Production accounting: đếm công cần làm, không đếm item×animation

**Đơn vị:** pose key là hình dáng cần đọc; frame/ô logical là mẫu trên timeline; sprite outcome là ảnh raster khác được xuất; variant là sửa từ template; module là một family part; preset là config VFX; placement là instance trong map. Một ảnh dùng ở năm ô vẫn một ảnh, nhưng năm chỗ sync phải được kiểm. Một recolor xuất PNG khác là variant outcome, không công vẽ silhouette mới.

**Cách dùng accounting hiện tại:** tách công tạo pose, sửa variant, export/slice, socket/import, placement, QA và rework. CURRENT đo mẫu Kiếm/outfit I/room/UI đã nhập chạy được; tổng ảnh và tổng giờ vẫn OPEN A17.

Tra đầy đủ [S0 player](#player-s0) và [các family/công sản xuất](#family-scenarios) tại Phụ lục B. Các bảng giữ toàn bộ giả định/dedup, phép cộng, workload và ma trận QA; số suy ra chưa phải số asset đã sản xuất. [Accounting cũ](#legacy-art-accounting) nằm riêng trong trace để tránh dùng làm budget hiện tại.

<a id="art-tool-workflow"></a>

### 23.1. Quy trình art nhỏ cho người chưa thạo vẽ

**TOOL CANDIDATE / CURRENT PROBE:** PixelLab là công cụ ứng viên để thử chất lượng đầu vào bằng Free/free trial nếu phù hợp. Nếu mẫu không đạt hoặc thiếu tool, có thể đổi công cụ/workflow; art contract và architecture không phụ thuộc PixelLab. Không mua/generate full catalog theo S0.

Trước probe, kiểm [FAQ chính thức](https://www.pixellab.ai/docs/faq) và quyền tool thực trên tài khoản Free/trial; không giả animation/outfit/size/features đều miễn phí. Chưa có mẫu nào được tạo trong vòng này.

Chọn mẫu nhỏ phục vụ CURRENT: cùng character/default outfit, Mộc/Kiếm/outfit I, pose Attack/Skill và một surface/icon/impact. Giữ reference/palette/canvas/pivot; xuất ảnh rồi sửa outline/transparent/palette/hand-grip/alignment trong editor pixel (ví dụ Pixelorama), slice/import và chạy room Unity ở camera thực. Nếu Free không hỗ trợ công đoạn cần kiểm, ghi thiếu evidence, dùng placeholder cho pipeline; không gọi thiếu tool là đã chứng minh hybrid đạt.

Ghi mỗi output vào nhóm **dùng trực tiếp / dùng sau sửa / loại bỏ**; một output chỉ một nhóm sau QA. `Dùng trực tiếp% = pass không sửa / tổng output`; `dùng sau sửa% = pass đã sửa / tổng output`; `dùng được tổng% = (hai nhóm pass) / tổng output`. Đếm cả lần thất bại và sample size, cùng giờ clean/slice/socket/import/QA/rework theo loại. Khi chưa chạy ghi **CHƯA ĐO**, không đoán 70/80/90%. Nếu PNG đẹp nhưng chuỗi pose/tay áo/socket lệch, mẫu animation chưa pass; không tính mọi frame trong strip là đạt chỉ vì một frame đạt.

Dùng số đo để chọn tiếp tục tool, sửa tay nhiều hơn, giảm polish hoặc lấy asset tham khảo có nguồn rõ; đánh giá chi phí **mẫu đã nhập chạy được**, không theo credits/ảnh đẹp nhất. Quyết định chi tiền là bước sau evidence, không tự mua gói. Đây là quy trình art của tài liệu sống, không PixelLab/trial plan, prompts hoặc asset manifest riêng. Gate/nguồn lực xem [Roadmap §4–6](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#art-workflow).

<a id="art-validation"></a>

## 24. Prototype/validation trước production hàng loạt

Đây là **ma trận validation visual**, thứ tự làm nằm ở Roadmap; quy trình mẫu/công cụ ứng viên ở §23.1, không kế hoạch tool hoặc generate asset riêng. Các ca dưới **CHƯA CHẠY**; dùng placeholder/fixture phù hợp khi làm Unity slice, chưa nghiệm thu bằng tài liệu. Các “dấu hiệu đủ” là câu hỏi để validate proposal: riêng P12 “Immediate local pose” kiểm phương án A11, không tự khóa anticipation. G-N cần result/life/phase đúng với chế độ đã thử, không buộc triển khai anticipation chưa duyệt hoặc thêm tentative projectile branch.

**Ưu tiên kiểm:**

- Trước base: P01/P02 Kiếm/default/I, P05 Nấm/Sói, P06 ground, P07 solo Q3/Q6, P08 room, P09 geometry nhỏ, P11 kit và P15 công thực tế. P04 Arc/Line chỉ fixture hẹp khi cần.
- Sau base và G-L mới: P07 contention, P12 Dedicated. RAM pass không thay backend ACK/terminal-pending ở P06/P12.
- Sau core gate: Cung/P03, Boss/P14, thác/full gear và stress full content. Bảng dưới giữ toàn TARGET, không buộc chạy hết trước slice.

**Asset Definition of Done — áp cho module nhập vào probe:**

- [ ] Canvas/PPU/pivot/mốc chân đúng; alpha/palette sạch, không crop lệch cell hoặc bleed.
- [ ] PoseKey/ref/duration có manifest; các layer khớp, không hở Body/Armor/Lower.
- [ ] Hai hướng đúng grip/socket/front-back; không mirror text/physics hoặc mirror hai lần.
- [ ] Import/slice/platform/atlas refs đúng; gear swap runtime giữ phase, Run không trượt chân.
- [ ] Camera thực đọc silhouette/cue/UI; AnimationEvent/visual collision không gây damage.
- [ ] Source/export/meta/revision/provenance đầy đủ; ghi pass/fail, công sửa và output bị loại.

DoD là chất lượng asset probe. “Có PNG” hoặc “build chạy” chưa nghiệm thu art/UX toàn game.

| Ca | Nội dung phải thử | Dấu hiệu đủ để ra quyết định |
| --- | --- | --- |
| P01 Modular player | Default +I/II/III Armor/Lower, mix-band, unequip, cả state và hai hướng | Không hở thân/grip sai/nhảy pivot; xác nhận pose refs nào thật sự reuse; quyết26 logical hay physical và delta class/airborne |
| P02 Hai Sword cùng action | Mộc và Huyền Ấn dùng cùng track Attack; hai Kiếm class khác band dùng cùng Skill; thử swap giữa action hợp lệ | Đúng cây ở cả Idle/release, không sprite Common baked; socket/góc raster đủ; visual snapshot không morph sai; không cấp skill class cho Tân Lữ chỉ để test Mộc |
| P03 Bow | Ít nhất hai band rest/draw/release/Skill, S1 và triple | Tay/string/arrow nock khớp; spawn đúng timeline, ba tên một cast; quyết có partial-draw thêm và logical hit/visual timing contract |
| P04 Multi-target | Arc3, Line5, Hàn1+4, spreadA/B/C–A/B/A–A/A/A, no-target/invalid | Arc/line/nổ impact cùng resolve phase; spread đúng flight, không chain false; không double status/impact/projectile |
| P05 Hit không stun | Mỗi rig bị hit Idle/Move/Windup/Attack/Ranged; Freeze trước/sau release; lethal | Main action không restart/hit trễ vì flash; CC server cancel unresolved đúng life; resolved result bất biến, visual travel không gameplay callback, Death thắng stale Hit |
| P06 Death/corpse | Ground/Ong/Boss; chết ở ledge, late viewer, ACK chậm/retry, normal respawn 25 s | Corpse không target/collider/AI, loot đứng được, timer từ death không từ clip/ACK, đời mới không nhận effect cũ |
| P07 Dummy online | Q3/Q6 solo, hai ngườiLv3, Lv3+Lv20,3–4 requesters | HP/Break/respawn thật, credit≥20% đúng từng life, không hai loại dummy; đo chờ để chọn timer/standpoints |
| P08 Terrain room | Solid/step/one-way/pass-through/overpass và background cùng chất liệu | Người chưa biết collider đoán đúng trước khi nhảy; drop một actor không làm người kia rơi; Kiếm/Cung đọc được target |
| P09 Công trình | Hai surfaces/routes +spawn/hybrid/Ong/chase/leash/Return/melee/Bow | Không stuck/unreachable/spawn trên không; projectile mask đúng solid/platform; không ngầm cần mob Jump/navigation mới |
| P10 Nước | Puddle base/front, thác seam, actor dưới nước/đi trên cầu, cả facing | Front chỉ che chân, không telegraph; bridge không splash, không thay physics; quyết sprite/particle reuse |
| P11 UI kit | Inventory/Storage/Shop cùng kit, Character/Enhance/Transfer, Vietnamese text/tooltip | Không bitmap screens riêng, rarity/band/+level rõ; pending/error/RNG fail khác, focus/input không phát attack |
| P12 Online presentation | 2 clients Dedicated baseline; thêm4+, RTT0/100/200 ms, jitter/loss probe | Immediate local pose, no trusted client damage, one effect per result, late death/status/action đúng generation, map/reconnect không replay reward |
| P13 Combat/balance | Intervals/timing control; Lv5/10/17, MP zero-INT/cân bằng, TTK solo/cụm và PvP Food/quota | Ghi số Novice/S1/S2/S3 casts, thời gian chờ MP, proc/TTK/visual travel; đổi một nhóm số mỗi lượt, chạy lại model sau khi timeline thay |
| P14 Boss/camera | Telegraph0,5/1,0/1,2 s, ba landing zones, Cuồng dưới30%, Slow, nhiều VFX/người | Telegraph không bị che/cắt hoặc time-stretch, đọc/né bằng movement hiện có; camera/zoom đọc được range |
| P15 Chi phí slice | Một outfit family, Sword/Bow, mob hybrid và room/UI primitive nhập hoàn chỉnh | Manifest unique/dedup/variant/setup và giờ thật; derive lại accounting, không production hàng loạt với frame count chưa kiểm |

Ghi build/Editor/packages, camera scale, machine và seed/fixture; không claim benchmark capacity từ một ca chạy mượt. Nếu vấn đề là pose không đọc, sửa art/timing presentation trước; nếu cần đổi hit/spawn/CD/collider/loot position/timer, trình như gameplay/Technical proposal, chạy ca phụ thuộc. Validation không tự khóa mọi số đề xuất trong file này.

## 25. Tra cứu OPEN decisions

Options, trade-off, recommendation và cách chốt của **A01–A17 đã chuyển nguyên** sang [Analysis — sổ quyết định Art](3_HUYEN_LO_DESIGN_ANALYSIS.md#art-open-decisions), cùng trạng thái và gate BAL/ART/PHY/TECH liên quan. File này giữ reasoning chi tiết/ma trận thử; không giữ một bảng trạng thái thứ hai.

A01/A02/A14 chi phối pose/modular và chi phí; A03 liên quan Boots; A04/A05/A06 liên quan concept/timing/Cung; A07 Dummy; A08/A15 death/corpse/loot; A09/A10 terrain/route/projectile; A11/A13 online/Select; A12 camera; A16 Boss; A17 accounting. CURRENT/DEFERRED do Roadmap sở hữu. Chưa prototype không biến recommendation thành LOCKED; giữ nguyên GDD baseline đến khi quyết định thay được duyệt.

---

<a id="working-spec-end"></a>

**CURRENT WORKING SPEC ENDS HERE.** Phần dưới là reasoning, accounting scenarios và lịch sử để tra khi cần; người làm probe bắt đầu ở §22.1, không phải đọc hết phụ lục. Các bảng được giữ nguyên để bảo toàn evidence.

<a id="art-rationale"></a>

## Phụ lục A. Rationale và ghi chú thiết kế

Phần này giữ các phát hiện và trade-off đã dẫn tới spec ở trên. Recommendation là phương án để thử; trạng thái quyết định vẫn ở Analysis A01–A17.

<a id="art-initial-findings"></a>

### Những phát hiện ban đầu — reasoning được giữ

| Requirement hiện tại | Vấn đề khi production | Hướng đề xuất |
| --- | --- | --- |
| Một rig nam, 26 frame đồng bộ cho mọi part | Đúng tổng ô một profile, chưa chứng minh đủ hình cho cả chém kiếm và kéo cung; index đồng bộ không đồng nghĩa mỗi part cần một PNG mới ở mỗi ô | Giữ tám state và nhịp mẫu; tách profile Kiếm/Cung, bảng ánh xạ pose và hình thực. Việc diễn giải lại user-lock 26 frame phải được duyệt |
| 12 module = ba band × bốn loại | Chưa tính visual Mộc Kiếm và outfit trước khi nhận Áo/Quần; cũng chưa tính redraw tay/áo theo class | 13 module gear world, thêm hai fallback outfit; không nhân item × 26 |
| VFX accounting cũ: bốn active presets | Sáu profile Lv5/10/17, normal, impact, status và telegraph có nhiệm vụ đọc gameplay khác nhau | Lịch sử bốn SkillIds; hiện sáu bindings độc lập, reuse preset family; thêm các primitive dùng chung, không sáu bộ VFX độc lập |
| Attack/Skill 12 FPS, action lock 0,26–0,40 s | Ba frame mất 0,25 s, bốn frame mất 0,333 s ở tốc đều; hit 0,10–0,18 s không luôn nằm đúng biên frame. Bow cần draw/release thật | Author thời lượng từng pose theo server timeline; FPS chỉ mốc preview, không clock damage |
| Multi-target và projectile Cung | Arc/line/nổ có thể cùng mốc; ba tên hiện spawn lệch 30 ms và có thời gian bay. Không thể tuyên bố mọi skill resolve đồng thời mà vẫn giữ luật này | Một cast đọc được như một nhịp; impact theo thời điểm server. OPEN riêng nếu muốn đổi ba tên thành resolve đồng thời |
| Mob Hit/Death chưa có bảng production | Damage thường dễ bị diễn thành stun; corpse dễ kéo dài hurtbox hoặc respawn | Hit feedback chồng lên action, Death terminal tách khỏi corpse và reward |
| MapRoot thiên về bốn Tilemap | Chưa đủ mô tả công trình đứng được, nước có lớp trước/sau, route của mob | Giữ physics đơn giản; thêm nhóm structural/overlay vào quy ước authoring, không xây hệ building lớn |
| Login/Character Select đã là P0 | UI kit, trạng thái chờ/lỗi và nguồn preview chưa nằm rõ trong scope art | Reuse rig và kit; không thêm account feature |

<a id="art-technique-rationale"></a>

### So sánh raster, skeletal/socket và hybrid

| Cách | Combat/pixel art 64×64 | Chi phí thực và rủi ro |
| --- | --- | --- |
| Modular frame-by-frame | Outline và foreshortening kiểm soát tốt; mọi part chọn theo pose key | Redraw áo/tay ở pose khác; tốn kiểm mọi outfit. Bảng pose dùng lại giảm sprite nhưng không xóa công QA |
| Skeletal/socket toàn thân | Reuse ảnh chi và tween được nhiều action | Đầu tư rig/weights/khớp; rotation/deformation có thể làm pixel/outline biến dạng. Tay áo dài và occlusion khó; không mặc định rẻ hơn |
| **Hybrid đề xuất** | Cơ thể/áo/chân chủ yếu raster theo pose; đầu reuse có offset; vũ khí theo socket và vài pose sửa raster | Giữ chất pixel tại các pose chính, giảm redraw weapon và đầu. Cần kiểm đường xoay vũ khí, tay nắm và front/back; không cần hệ skeletal tổng quát |

**OPEN A01/A02:** prototype hybrid trước. Nếu socket rotation gây nhấp nháy hoặc mất hình kiếm, dùng góc raster đã vẽ ở pose chính; nếu áo/chân lệch, thêm đúng pose thiếu thay vì đổi toàn dự án sang skeletal. Reuse lower-body giữa hai class chỉ sau khi stance và trọng tâm được kiểm.

<a id="art-boots-rationale"></a>

### Tác động nếu bỏ Boots hoặc thêm footwear visual

| Dependency | Tác động nếu duyệt bỏ | Việc bắt buộc đo/sync |
| --- | --- | --- |
| Catalog / scope | 6→5 slots; 18→15 family; 21→18 mẫu thường, cộng Mộc Kiếm thành 19 gear definitions | Bỏ ba icon nhưng **không giảm world animation**, vì Boots vốn stat-only |
| Chỉ số | Mất DEF 2/4/6, EVA 4/8/12, flat +2 EVA/cấp, Tinh Hoa Giày; mất tốc chạy +1/2/3% | Build survival/accuracy, kite/chase/run-back và PvP phải tính lại; không tự chuyển tất cả stat sang Quần |
| Loot | Các nguồn 6 slots thành 5, nguồn Lv2/4 từ 5 thành 4 | Nếu giữ chọn đều, mỗi non-weapon ở nguồn đủ slot tăng 1/6→1/5; xác suất một loại weapon khi gear roll thành công tăng 1/12→1/10. Đây là thay economy dù tổng gear roll giữ nguyên |
| Shop / sinks | Mất dòng mua/bán và đường enhance/transfer một slot | Tính lại vendor-all, nhu cầu Gold/Stone và giá trị loot; không giả sink thực giảm đúng 1/6 vì player không đầu tư đều |
| UI / progression | Character/equipment filters/tooltip/preview/QA đổi; Q7 Nhẫn vẫn giữ | Cập nhật item refs, evaluator, fixtures, bảng Analysis và acceptance; không rename slot rồi bỏ qua stat |

**OPEN A03:** bỏ Boots chỉ hợp nếu mục tiêu là đơn giản hóa gear, không phải tiết kiệm art. Recommendation hiện tại giữ. Nếu muốn footwear world thực sự theo Boots, đó là option thứ ba tăng một visual layer và kiểm overlap, không tự đưa vào P0.

<a id="derived-production-scenarios"></a>

## Phụ lục B. Kịch bản production suy ra

Các giả định, phép cộng và giới hạn dưới đây được giữ nguyên. Đây là dữ liệu tính thử, chưa phải manifest hoặc budget đã được duyệt. [§23](#production-accounting) là điểm vào accounting hiện tại.

<a id="player-s0"></a>

### B.1. Kịch bản player S0 có thể kiểm lại

S0 giả định hybrid, 19 ô chung đều có body riêng; bảy ô action/class đều riêng upper body; locomotion dùng tay trung tính; các active trong cùng class reuse Skill4. Chưa cộng pose airborne bổ sung, hand-front slice hoặc idle cầm cung khác. Các tập Hair/LowerBody dưới đây là **giả định dedup công khai**, phải thay bằng manifest sau prototype; không claim đã chứng minh từ artwork chưa có.

| Family | Tập pose/hình trong S0 | Variants / sprite outcomes | Reuse thực sự |
| --- | --- | --- | --- |
| BodyBase | 19 common +7 Kiếm +7 Cung=33 | 1 body /33 | Hai profile có52 logical ô nhưng chỉ33 body pose; không52 full redraw |
| Hair/Head | Idle1+Run2+Jump1+Fall1+Hit1+Death2=8; action7 dùng chung giữa class=15 | 1 family /tối đa15 theo tập giả định | Offset dùng chung; nếu đầu giữ cùng góc phải trỏ lại ảnh và giảm số, không vẽ15 chỉ để đúng bảng |
| Armor | 19 common +14 action=33 /thiết kế | Default+3 bands=4 thiết kế →132 outcomes | 33 pose template; ba variant sửa chất liệu/silhouette/tay áo, không132 body drawings |
| LowerBody | Idle2+Run6+Jump1+Fall1+Hit1+Death3=14; Attack3+Skill4 chân chung=7 →21 /thiết kế | Default+3 bands=4 →84 outcomes | Chân chung class; chỉ hợp nếu upper/lower pose khớp. Thêm bow stance thì delta được ghi, không âm thầm nhân33 |
| Sword | 4 canonical +tối đa4×3 góc sửa=16 | 4 visual /4–16 outcomes | Grip/transform/Skill góc reuse;4 là option socket thuần,16 là option hybrid nhiều sửa |
| Bow | 3 shapes×3 bands=9 | 3 visual /9 | Track draw/string/arrow dùng chung; mỗi cây vẫn có silhouette riêng |
| Arrow | 1 | 1 outcome | Normal/skills dùng tip/trail/palette config, không một ảnh mỗi item |

**SUY RA S0:** body33 +hair15 +armor132 +lower84 =**264 raster outcomes** nếu toàn bộ tập giả định được xuất thành hình khác. Weapon13–25 +arrow1 →**278–290 outcomes player/weapon/arrow**. Đây là kiểm tổng từ tập pose, **không 278–290 tranh vẽ độc lập**, không trần cuối dự án. Nếu recolor runtime thay PNG hoặc nhiều pose reuse, số ảnh giảm; nếu stance/airborne cần sửa, tăng đúng delta.

13 **gear world modules** =4 Sword +3 Bow +3 Armor +3 LowerBody. Hai fallback outfit →15 outfit/weapon modules kể cả default; Body/Hair là hai base families riêng. 26-frame contract cũ phải được làm rõ trước duyệt S0. Không xem con số33,264 hoặc290 là lock thay thế26 trong lượt này.

Công vẽ mới tách khỏi export: Body33 pose; Armor33 template +3×33 lần sửa variant; Lower21 template +3×21 sửa; Hair theo số góc thực, tối đa15 của S0;7 weapon canonical +tối đa18 sửa góc/deformation. Công sạch/slice/manifest/socket/order/import/QA tính riêng. Một shape crop dùng hai render slice không thành hai thiết kế nhưng vẫn có setup/QA.

<a id="family-scenarios"></a>

### B.2. Tổng quan các family còn lại

| Asset family | Unique frame/sprite trong kịch bản | Variants/reuse | Workload chưa thể quy thành số ảnh/giờ |
| --- | --- | --- | --- |
| Normal mobs | 6 rigs /105 pose §6 | Wolf palette identity thứ7; nếu xuất PNG palette, thêm 17 recolor outcomes nhưng 0 pose mới; không rig Linh mới | Clean và windup/hurtbox/flight/death QA theo từng behavior |
| Boss | 1 rig /28 core pose, optional+4 roar | Claw reuse Basic, Dư Ảnh chỉ tên; Cuồng/Slow overlays | Canvas/area lớn, telegraph/scheduler/3-zone QA; không lấy cost/frame player áp Boss |
| Dummy | 1 prefab visual /6 pose | Một pool dùng Q3/Q6/training, số placement theo validation | Shared HP/life/credit/respawn, không dummy system/DPS Meter |
| NPC | 8 full sprites /16 idle outcomes; optional+6 work/gesture | Props/source templates chung | Setup menus/anchors/occlusion; không8×26, không8 portraits bắt buộc |
| VFX | **10 family chức năng** bên dưới, không10 PNG | Profile/weapon/target reuse config | Frame count sau chọn sprite/particle/mask; overdraw/pool/reset/online QA |
| Terrain | 3 chất liệu, topology17 +cosmetic4/họ →tối đa63 hình §12 | Collider semantics/template chung, mirror/dedup có thể giảm | Room authoring và edge/route QA, không8 maps×63 |
| Mini building | Ví dụ gỗ10 logical modules /8–10 hình §13 | Cầu/mái/cột lặp; stone có thể dùng terrain | Chưa có layout hoàn tất để chốt stone kit và full-asset dimensions |
| Full structural assets | Danh mục cần review: forge station, broken/quest seals, Huyền Môn, Boss landmark | Ba seal anchors dùng một motif, back/front slice; waterfall ở nhóm riêng | Chốt mỗi landmark sau blockout; không tự áp một asset độc nhất mỗi map |
| Background/decoration | 3 environment families; hub reuse | Palette/crop/clusters, props dùng lại nhiều roots | Số silhouette/prop phải từ blockout+camera; chưa derive được tổng nên không đoán |
| Animated environment | Thác12 ô; nếu dùng: flow4, ripple3–4; puddle2 static, leaf1–2, puff1, flame4 | Flame Burn dùng chung khi phù hợp; puff/dust dùng lại | Không cộng flame/texture hai lần vào tổng; cần manifest shared refs |
| Items/icons | 49 physical item bindings +Gold1 +skills10 +status4 =64 binding cơ sở | Food status reuse, rarity/+n overlay; tối đa63 ảnh outcome trước glyph/virtual | Motif mới/clean crop/tier accent phải ghi riêng; không64 tranh hoàn toàn mới |
| Common UI | 21 primitives/chức năng §19 | Panel/button/slot compose; nhiều states tint/mask/text | Layout/bindings/font/focus/pending/error; screen-specific art chỉ phần thật cần |
| Map content placement | 8 roots,5 farm maps/28 pockets/66 slots hiện hành | Reuse art không reuse toàn route | Colliders/spawns/MapExit/SpecialGate/safe strips/Boss exclusion/traversal/combat QA là cost editor thật |

Mười VFX family để không giấu scope trong “4 active”: **(1)** sword slash, **(2)** arrow/flight kể cả generic mob config, **(3)** signature line/wave, **(4)** explosion/burst, **(5)** landed impact, **(6)** telegraph cone/ground/landing, **(7)** statuses Burn/Freeze/Slow, **(8)** aura Linh/Cuồng, **(9)** beneficial heal/MP/Food/revive, **(10)** kết quả reward/loot/enhance/death cosmetic. Family là nhóm reuse/QA, không buộc một prefab đa năng. Status family cần ba ngôn ngữ khác nhau, không gọi cùng một tint là đủ cả ba.

Frame probe slash4/wave4/burst5/impact3/Freeze shell2+thaw3/flame4 ở §4/§14/§17 có lý do theo phase. Particle/glint/sigil thường chỉ một primitive; không cộng mỗi lifetime tick thành frame. Main active có sáu SkillId bindings; unique preset/bitmap theo reuse manifest, không ép sáu families; normal/telegraph/status ngoài active được tính độc lập. Tổng sprite VFX chỉ chốt sau manifest xác định shared textures và technique, tránh double-count flame/aura/arrow.

**Cách ra production cost thực:** đo giờ riêng cho tạo pose template mới, sửa silhouette variant, recolor/cleanup, export/slice, socket/sorting setup, integration, QA và rework. Ước lượng `Σ(số pose mới×giờ/pose mới + số sửa variant×giờ/sửa + công setup/QA)` từ slice đạt chất lượng, không từ credit hoặc phép nhân item count. Ghi cả lần sửa Bow và Death thất bại; không dùng lần recolor nhanh nhất làm tốc độ vẽ toàn bộ Boss. Chưa có asset/slice nên **không có căn cứ chốt tổng giờ**; mốc160–240 h cũ đã bị Technical loại là estimate hiện hành.

Ma trận QA tối thiểu: mỗi Armor với Sword Attack và Bow draw/Skill; mỗi LowerBody với Run/Jump/Fall/Death; cả 7 weapon với Attack, 6 weapon class với Skill, ở hai hướng; một số mix-band mặc chéo và default/unequip. Nếu kiểm toàn bộ: 4 Armor×4 LowerBody×7 weapon=112 outfit/weapon combinations trước hướng/pose, nhưng không vẽ 112 rigs. Kiểm theo part/contact và ca mix-band rủi ro, không giả bỏ integration nhờ reuse.

<a id="art-historical-trace"></a>

## Phụ lục C. Historical trace và các giả định trước đây

**HISTORICAL / SUPERSEDED về gameplay:** pipeline/accounting và đề xuất sync sau đây là trace trước lock. Các mô tả flight/class Normal/four SkillIds/evolution không là contract hiện hành; giữ số và dependency để đối chiếu. Các nhãn §22.1/§26 và anchor cũ được giữ để tra cứu; thứ tự làm hiện tại thuộc Roadmap.

<a id="legacy-visual-flow"></a>

### C.1. Pipeline trước consolidation — trace được giữ

Bảng sau chuyển nguyên từ GDD §9 để bảo toàn trình tự production cũ. Chi tiết art do file này sở hữu; cách gắn runtime/physics do Technical sở hữu. Cụm `slice 26 frames` là shorthand lịch sử của contract; **không giải quyết A01 hoặc buộc mỗi part có 26 PNG**. Dùng §1/§22 và gate A01/A02 trước production.

| Pipeline | Thứ tự và ranh giới |
| --- | --- |
| Nhân vật | Source sprite → canvas64×64 / PPU 32 → slice 26 frames → chung pivot chân → male BodyBase / HairHead / Pants / Armor / Weapon → đồng bộ state / frame → palette / accent ba gear bands → actor SortingGroup → status / VFX overlays. Hurtbox / collider độc lập visual, đổi gear / scale Linh không đổi physics. |
| Map | Forest / Mountain / Ancient → tileset / palette → background → Ground / one-way Platform → back props → landmark → foreground → anchors quái / NPC / MapExit/SpecialGate → colliders → camera bounds → kiểm contrast / telegraph / loot / chat. Không thêm lighting framework P0; dùng màu / VFX hiện có. |
| Gắn layout với art | Đồng thoáng / sparse; Trúc nhiều tầng / cầu; Bạch bậc thác; Xích hẻm núi / ba dấu ấn; Huyền phế tích / landmark Boss. Hub reuse props; safe strips và đường về phải đọc được. |

<a id="legacy-art-accounting"></a>

**Accounting trước review (giữ để đối chiếu, không budget đã duyệt):**

Art accounting: 3 bands × (Sword + Bow + Armor + Pants) = 12 visual modules trên chung 26-frame rig, không 18 full rigs; 21 regular template icons có thể reuse motif / palette, sáu manual icons dùng hai motif class + ba accents. Một base body / hair, aura / status overlays chung; actual slicing / pose reuse cần ART-01 đo, không nhân template count thành rig count.

Phép tính 12 đúng cho ba band × bốn loại, nhưng chưa bao gồm Mộc Kiếm/fallback outfit/pose theo class. Kịch bản bổ sung và mọi giả định nằm ở §23; số 13/15/33/264/278–290 chưa thay thế user-lock hay thành manifest sản xuất.

<a id="sync-history"></a>

### C.2. Trace đề xuất sync V6.1 và điều kiện còn lại

Bảng đề xuất sync trước consolidation được giữ nguyên dưới đây để không mất dependency/history. **Không phải mọi dòng đã áp dụng.** Các nguyên tắc/owner/routing đã sync theo §0; phần đổi gameplay/26-frame/hybrid/camera/Dummy/Boots vẫn theo trạng thái Analysis Axx. Di chuyển nội dung thực tế có bảng [SOURCE → DESTINATION](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#source-destination) và audit ở file 5. Các tham chiếu §10/§25 trong bảng lịch sử chỉ evidence/decision cũ, nay đã chuyển Analysis; GDD/Technical vẫn giữ baseline nếu option chưa duyệt.

| File / vị trí hiện có | Nội dung cụ thể cần cập nhật sau duyệt | Điều kiện/dependency |
| --- | --- | --- |
| GDD §0/§9/§10 | Làm rõ lock 26 là ô/profile hay tổng hình; class pose/dedup/default outfit; 13 gear world modules thay 12 | A01/A02/P01–P03; đồng bộ acceptance modular, không âm thầm bỏ user-lock hoặc thêm item |
| GDD §1/§6/§9 | Bảy weapon visuals kể cả Mộc; silhouette ba band; Armor/LowerBody+footwear; Ring/Necklace/Boots stat-only | Item/stat/rarity tách visual; không đổi catalog/range nếu chỉ duyệt art |
| GDD §2/§6/§10 nếu bỏ Boots | 5 slots/15 families/18 regular+Mộc; DEF/EVA/MoveSpeed/enhance/Tinh Hoa; slot pool/shop/sell/transfer/fixtures/DoD | Chỉ khi option bỏ A03 được duyệt; tính lại balance/economy, không tự chuyển stat |
| GDD §3 | Executor/speed/flight normal Cung còn thiếu; timeline riêng theo class nếu retune; quyền movement/jump lúc cast; sáu profile/bốn SkillIds; semantics multi-target | A05/A06/A14; giữ fallback SnapshotSpread/status dedup/Line falloff/Hàn không double-hit trừ khi luật mới được duyệt |
| GDD §3/§4/§9 | Damage reaction không stun, CC/Death cancel, normal/flying death/corpse, status/Boss telegraph dễ đọc | Presentation không đổi hit/CC; loot ground position hoặc mốc respawn đụng gameplay phải review riêng |
| GDD §4/§9 | Structural standables/surfaces, AI reachable/leash/flying approach, cue solid/one-way, nước nông giảm tốc theo GDD, camera readability | Giữ 8 roots/28 pockets/66 slots/gates; không thêm AI Jump/hazard/swimming; mask projectile cần chốt |
| GDD §5 Q3/Q6, §4 nếu cần | Bù Nhìn chung prefab/HP/DEF/EVA/life/Break; respawn/điểm đứng nếu đổi | A07/P07; giữ ba kills/20% quest credit và training yard; không QuestId/reward mới |
| GDD §8/§9 | Local anticipation/remote phase, UI pending/Common Kit, dependency Login/Select preview | Giữ authority và screen flow; không Create/Register/Party hoặc networking feature mới |
| Technical §2 | MapRoot có structural back/front/surface colliders/water overlay; route/spawn/loot ground points/occlusion | One-way theo actor như hiện hành; spike thuật toán navigation nếu phải đổi tầng |
| Technical §3/§4 | Visual snapshot weapon/profile/revision/clock; HitResult resolveTime/life generation; projectile phase/status snapshot/cancel/dedup | A11/A15; art/socket không authority; normal Bow contract sync GDD, không thêm client trust |
| Technical §5/§9 | Nguồn Select preview; read-only visual summary nếu chọn exact gear; views pending/error/connecting | Không dùng inventory/Gold client payload làm nguồn tin cậy, không thêm admin UI |
| Technical §6/§7 | Presentation terminal-pending; deathUtc t0/finalize/respawn deadline, corpse lifetime, loot publish sau ACK | Transaction/retry giữ semantics; corpse không chặn finalize/respawn, deadline không reset theo animation |
| Technical §8 | Pose manifest thay suy luận mỗi part cần 26 PNG; default/class profiles, weapon sockets/string/front-back; import/padding/atlas/sorting/flip/pool reset | A01/A02/P15; kiểm camera/import trên pipeline được pin, không thêm package tùy tiện |
| Technical §9/§10/§12 | Kit 21 primitives khác số ảnh; layout/icons từng view; thay estimate 12 modules; đo giờ variant/import/QA, thêm ca §24 | Không budget credit/PixelLab plan; QA 3–4+ là benchmark, không capacity claim |
| Analysis §1/§2/§4 | Cập nhật derived art counts; chạy lại TTK/sustain/Boss/PvP nếu đổi timeline/CD/projectile/Boots; ghi giả định và giờ slice | TTK cũ trước phụ stat gear là mốc tham khảo, không duyệt cảm giác; formula GDD giữ authority |
| Analysis §3/§4 | Dummy contention, layout mob-platform/loot access/run-back nếu timer/route/Boots đổi; vendor/slot pool/enhance sinks nếu bỏ Boots | Không suy economy giảm đúng 1/6 hoặc spawn rate chia theo N; quest credit và reward eligibility không gộp |
| Analysis §5 ART-01/PHY-01/BAL-02/CC-01/SCOPE-01/BOSS-02/TECH-01 | Gắn OPEN A01–A17 vào đúng gate, ghi option đã duyệt và evidence còn chưa chạy | Không mở lại PvP/SAVE user locks nếu chỉ đổi presentation; không biến mọi proposal thành baseline |

**Tự review sau consolidation:** bảo toàn S0/icons/mob/timing/options/dependencies; evidence timing và sổ Axx nay ở Analysis, data proposal ở Technical, lịch ở Roadmap. Art vẫn đủ detail cho pose/weapon/map/mob/NPC/UI/import/production. Các proposal đổi gameplay và counts chưa kiểm vẫn OPEN; prototype §24 chưa chạy. Corpse/status/VFX không quyết định damage/respawn/credit; local fixture không giả success bền vững. Số liệu đếm và kiểm destination nằm ở [audit](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#no-loss-audit).

<a id="cleanup-source-destination"></a>

### SOURCE → DESTINATION của lượt cleanup

Các khối dưới được MOVE đầy đủ trong cùng file; phần chính giữ summary/link. Bảng này chỉ ghi vị trí, không đổi trạng thái approval.

| Mã | SOURCE trước cleanup | DESTINATION hiện tại |
| --- | --- | --- |
| C01 | §0 — Những phát hiện ban đầu | [Phụ lục A — phát hiện](#art-initial-findings) |
| C02 | §1.2 — Ba technique và reasoning A01/A02 | [Phụ lục A — technique](#art-technique-rationale) |
| C03 | §3.1 — Dependency/option Boots | [Phụ lục A — Boots](#art-boots-rationale) |
| C04 | §23.1 — S0/pose/export/module calculations | [Phụ lục B — S0](#player-s0) |
| C05 | §23.2 — Family totals/VFX/cost/QA combinations | [Phụ lục B — accounting](#family-scenarios) |
| C06 | §22.1 — Pipeline và accounting cũ | [Phụ lục C — pipeline](#legacy-visual-flow), [accounting](#legacy-art-accounting) |
| C07 | §26 — Sync proposals/dependencies/self-review | [Phụ lục C — sync trace](#sync-history) |
