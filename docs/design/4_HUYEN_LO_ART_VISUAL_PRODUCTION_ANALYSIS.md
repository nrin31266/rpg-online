# Huyền Lộ — Phân tích art, hình ảnh và production

**Đối chiếu:** toàn bộ GDD V6.1.0, Technical và Design Analysis hiện tại.

**Ngày:** 2026-10-03 · **Trạng thái:** PHÂN TÍCH / ĐỀ XUẤT, chưa duyệt, chưa có prototype Unity.

Tài liệu này không sửa luật trong [GDD](1_HUYEN_LO_GDD.md), không thay [Technical](2_HUYEN_LO_TECHNICAL.md) và không biến mô phỏng trong [Analysis](3_HUYEN_LO_DESIGN_ANALYSIS.md) thành bằng chứng chơi thật. Yêu cầu tạo file 4 của lượt này là ngoại lệ có chủ đích với quy ước bộ tài liệu cũ; chưa cập nhật README hoặc ba file nguồn.

**HIỆN HÀNH** = requirement/số đang dùng; **ĐỀ XUẤT** = phương án để duyệt; **SUY RA** = phép tính kèm giả định; **OPEN / NEED VALIDATION** = chưa khóa. Các số art dưới đây là kịch bản authoring để kiểm scope, không phải asset đã tồn tại hay cam kết giờ sản xuất. Mọi thay đổi gameplay được nêu riêng và chỉ sync sau khi duyệt.

## 0. Những phát hiện quyết định scope

| Requirement hiện tại | Vấn đề khi production | Hướng đề xuất |
| --- | --- | --- |
| Một rig nam, 26 frame đồng bộ cho mọi part | Đúng tổng ô một profile, chưa chứng minh đủ hình cho cả chém kiếm và kéo cung; index đồng bộ không đồng nghĩa mỗi part cần một PNG mới ở mỗi ô | Giữ tám state và nhịp mẫu; tách profile Kiếm/Cung, bảng ánh xạ pose và hình thực. Việc diễn giải lại user-lock 26 frame phải được duyệt |
| 12 module = ba band × bốn loại | Chưa tính visual Mộc Kiếm và outfit trước khi nhận Áo/Quần; cũng chưa tính redraw tay/áo theo class | 13 module gear world, thêm hai fallback outfit; không nhân item × 26 |
| Bốn active VFX presets | Sáu profile Lv5/10/17, normal, impact, status và telegraph có nhiệm vụ đọc gameplay khác nhau | Bốn preset active theo SkillId, sáu cấu hình profile; thêm các primitive dùng chung, không sáu bộ VFX độc lập |
| Attack/Skill 12 FPS, action lock 0,26–0,40 s | Ba frame mất 0,25 s, bốn frame mất 0,333 s ở tốc đều; hit 0,10–0,18 s không luôn nằm đúng biên frame. Bow cần draw/release thật | Author thời lượng từng pose theo server timeline; FPS chỉ mốc preview, không clock damage |
| Multi-target và projectile Cung | Arc/line/nổ có thể cùng mốc; ba tên hiện spawn lệch 30 ms và có thời gian bay. Không thể tuyên bố mọi skill resolve đồng thời mà vẫn giữ luật này | Một cast đọc được như một nhịp; impact theo thời điểm server. OPEN riêng nếu muốn đổi ba tên thành resolve đồng thời |
| Mob Hit/Death chưa có bảng production | Damage thường dễ bị diễn thành stun; corpse dễ kéo dài hurtbox hoặc respawn | Hit feedback chồng lên action, Death terminal tách khỏi corpse và reward |
| MapRoot thiên về bốn Tilemap | Chưa đủ mô tả công trình đứng được, nước có lớp trước/sau, route của mob | Giữ physics đơn giản; thêm nhóm structural/overlay vào quy ước authoring, không xây hệ building lớn |
| Login/Character Select đã là P0 | UI kit, trạng thái chờ/lỗi và nguồn preview chưa nằm rõ trong scope art | Reuse rig và kit; không thêm account feature |

Ưu tiên chi phí: silhouette gear, tư thế Kiếm/Cung, thời điểm release/hit, telegraph và status là core. Cắt particle thừa, idle phụ và cảnh tổng kết cầu kỳ trước khi cắt các dấu hiệu đó. Không thêm class, slot, CC, companion, swimming, hệ ánh sáng hay active R P0.

## 1. Kiến trúc hình ảnh player

**Đọc từ:** GDD §0/§9, Technical §8. Giữ canvas 64×64, PPU 32, một cơ thể nam; không tạo full rig cho từng bộ đồ.

| Part | Sở hữu hình gì | Reuse và giới hạn |
| --- | --- | --- |
| BodyBase | Da, tay, cổ, phần cơ thể nhìn thấy; pose tay cầm/ra đòn | Locomotion chung nếu tay trung tính; Attack/Skill cần pose Kiếm/Cung riêng. Giáp không được che tay sai để giả vờ reuse |
| Hair/Head | Một mặt/tóc nền, không Helmet slot | Dùng lại hình đầu ở nhiều frame nếu góc mặt không đổi; offset theo pose. Frame đổi hướng nhìn/cúi đầu cần redraw |
| Armor | Thân áo, vai, tay áo thuộc outfit | Tay áo đi theo tay và xoay thân; một ảnh áo đứng yên không đủ cho swing/draw. Có thể tách hình tay áo trong source nhưng không thêm equipment slot |
| LowerBody | Quần **và footwear về presentation** | Chân chạy/nhảy/ngã/death cần thay pose; hai class có thể dùng cùng chân khi stance khớp. Giày stat-only không thay pixel footwear này |
| Weapon | Cây đang dùng, grip/string và vị trí trước/sau thân | Tra theo visual ID của action, không hard-code Sword_Common/Bow_Common trong Attack/Skill |

**Default outfit bắt buộc:** áo vải và quần/footwear đơn giản khi Lv1–2 hoặc unequip. Đây là fallback khi slot trống, không item mới, không stat và không tính là Band I. Q3 Quần I, Q5 Áo I phải tạo khác biệt nhìn thấy; có thể sửa màu/vạt áo trên cùng pose template, nhưng không mặc sẵn nguyên bộ I rồi claim progression đã hữu hình. Không equip weapon thì hai tay trung tính; không phát đòn giả với cây kiếm không tồn tại. Điều kiện attack khi chưa có vũ khí vẫn thuộc gameplay hiện hành, không tự thêm combat tay không.

### 1.1. Contract 26 frame thực sự đếm gì

Tám **logical state** không phải 26 state. `4+6+2+2+3+4+2+3=26` là 26 ô lấy mẫu của **một profile**; một ô có thể tham chiếu lại sprite, giữ pose lâu hơn, hoặc chọn sprite khác theo weapon class. Timeline không bắt buộc mỗi ô dài bằng nhau.

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

| Cách | Combat/pixel art 64×64 | Chi phí thực và rủi ro |
| --- | --- | --- |
| Modular frame-by-frame | Outline và foreshortening kiểm soát tốt; mọi part chọn theo pose key | Redraw áo/tay ở pose khác; tốn kiểm mọi outfit. Bảng pose dùng lại giảm sprite nhưng không xóa công QA |
| Skeletal/socket toàn thân | Reuse ảnh chi và tween được nhiều action | Đầu tư rig/weights/khớp; rotation/deformation có thể làm pixel/outline biến dạng. Tay áo dài và occlusion khó; không mặc định rẻ hơn |
| **Hybrid đề xuất** | Cơ thể/áo/chân chủ yếu raster theo pose; đầu reuse có offset; vũ khí theo socket và vài pose sửa raster | Giữ chất pixel tại các pose chính, giảm redraw weapon và đầu. Cần kiểm đường xoay vũ khí, tay nắm và front/back; không cần hệ skeletal tổng quát |

**OPEN A01/A02:** prototype hybrid trước. Nếu socket rotation gây nhấp nháy hoặc mất hình kiếm, dùng góc raster đã vẽ ở pose chính; nếu áo/chân lệch, thêm đúng pose thiếu thay vì đổi toàn dự án sang skeletal. Reuse lower-body giữa hai class chỉ sau khi stance và trọng tâm được kiểm.

**GAP A14 — ra đòn khi chạy/nhảy/rơi:** action lock hiện ngăn chồng action nhưng chưa mô tả đủ ranh giới movement/jump lúc cast. Cần chốt hành vi được phép rồi thử upper-body Attack/Skill kết hợp lower-body locomotion: cùng phase/root/socket, không hai clock độc lập. Nếu full-body Attack đặt chân xuống đất khi actor còn bay, cần pose/track sửa; không tự khóa movement/jump để giữ phép đếm 26/33. Player nhận damage thường cũng không tự restart Attack/Skill qua Hit; áp nguyên tắc overlay/action ở §7 khi không có CC gameplay.

### 1.3. Pivot, flip và overlap

Canvas căn một mốc chân `(32,0)` theo pivot hiện hành; artwork có thể chừa 1–2 px transparent và foot anchor offset chung, không auto-crop làm đổi điểm chân. Head/hand/grip author tọa độ pixel **theo pose**, không theo bounds ảnh. Grip lệch 1 px đã dễ thấy ở cây cung mảnh.

Giữ một SortingGroup/actor để part của hai player không chen nhau. Unity xác nhận SortingGroup phù hợp nhân vật gồm nhiều SpriteRenderer chồng lấp. [Nguồn Unity — SortingGroup](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Rendering.SortingGroup.html).

Order hiện tại là baseline, không áp một thứ tự cứng cho mọi pose: Shadow 0, WeaponBack 5, Body 10, LowerBody 14, Hair/Head 20, Armor 22, WeaponFront 30. Tay cầm cung phải có phần trước cây cung; tóc/vai/vạt áo cần mask/split đúng pose. Split trước/sau là render slice của một module, không một món gear mới. Ưu tiên cutout đã author; chỉ thêm HandFront nếu kiểm grip chứng minh cần, tính thêm việc slice/QA.

**Flip có bẫy kỹ thuật:** `SpriteRenderer.flipX` chỉ đổi render, không tự mirror child socket. Đề xuất mirror VisualRoot chứa sprite/socket, giữ physics root ngoài nó, không đồng thời flipX lần hai; hoặc flipX tất cả part và mirror socket/rotation rõ ràng. Gameplay origin do server/data riêng. [Nguồn Unity — flipX](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/SpriteRenderer-flipX.html).

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
| Mũi tên | 1 silhouette chung; tint/tip/trail theo normal/skill | Không bảy projectile theo gear. Tên gắn trên dây có thể dùng cùng ảnh; draw hand/arrow offset là track chung |

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

| Dependency | Tác động nếu duyệt bỏ | Việc bắt buộc đo/sync |
| --- | --- | --- |
| Catalog / scope | 6→5 slots; 18→15 family; 21→18 mẫu thường, cộng Mộc Kiếm thành 19 gear definitions | Bỏ ba icon nhưng **không giảm world animation**, vì Boots vốn stat-only |
| Chỉ số | Mất DEF 2/4/6, EVA 4/8/12, flat +2 EVA/cấp, Tinh Hoa Giày; mất tốc chạy +1/2/3% | Build survival/accuracy, kite/chase/run-back và PvP phải tính lại; không tự chuyển tất cả stat sang Quần |
| Loot | Các nguồn 6 slots thành 5, nguồn Lv2/4 từ 5 thành 4 | Nếu giữ chọn đều, mỗi non-weapon ở nguồn đủ slot tăng 1/6→1/5; xác suất một loại weapon khi gear roll thành công tăng 1/12→1/10. Đây là thay economy dù tổng gear roll giữ nguyên |
| Shop / sinks | Mất dòng mua/bán và đường enhance/transfer một slot | Tính lại vendor-all, nhu cầu Gold/Stone và giá trị loot; không giả sink thực giảm đúng 1/6 vì player không đầu tư đều |
| UI / progression | Character/equipment filters/tooltip/preview/QA đổi; Q7 Nhẫn vẫn giữ | Cập nhật item refs, evaluator, fixtures, bảng Analysis và acceptance; không rename slot rồi bỏ qua stat |

**OPEN A03:** bỏ Boots chỉ hợp nếu mục tiêu là đơn giản hóa gear, không phải tiết kiệm art. Recommendation hiện tại giữ. Nếu muốn footwear world thực sự theo Boots, đó là option thứ ba tăng một visual layer và kiểm overlap, không tự đưa vào P0.

## 4. Ngôn ngữ hình ảnh combat — CORE

**Đọc từ:** GDD §3/§4/§9. Người chơi cần biết ai ra đòn, cây gì, phạm vi nào, lúc nào trúng, có trạng thái gì. VFX đẹp mà sai thông tin này là lỗi core.

| Lớp presentation | Sở hữu | Điều không được suy ra từ lớp này |
| --- | --- | --- |
| Actor/cast animation | Anticipation, tụ lực, release, recovery; tư thế class | Không tự sinh hit/damage bằng AnimationEvent |
| Weapon visual | Item đang dùng, grip, draw/swing | Silhouette dài không tăng range |
| Normal presentation | Kiếm quét ngắn; Cung nock/draw/release và tên đọc được | Trail rộng không biến normal thành cleave |
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
| Kiếm normal | Swing gọn khoảng melee 1,2 u, một target có impact | Không lấy arc skill 1,7 u phủ lên normal |
| Cung normal | Pose kéo/release riêng, một tên nhìn thấy, impact hoặc NÉ | **GAP:** GDD có range nhưng chưa ghi đủ executor/speed normal Cung; đề xuất projectile server như identity Cung, cần duyệt contract trước chốt thời gian bay |
| Lv5 Phong Trảm | Cùng cây kiếm, dấu linh ấm ở release; nét chém rõ hơn normal nhưng chỉ một target 1,7 u | Một nhịp cast/shape riêng; không vẽ quét rộng ám chỉ ba con đều nhận damage |
| Lv5 Linh Tiễn | Cùng cây cung draw/release; tên có tip/trail linh khí mảnh, khác tên thường | Projectile thật về gameplay hiện hành; một target/arrow, không nhiều bóng tên giả |
| Lv10 Phong Trảm | Dấu quét 120° liên tục, accent Mạch Ấn; tối đa ba impact ở hit moment | Sweep và multi-target feedback core; không ba explosion lớn |
| Lv10 Linh Tiễn | Một lần kéo/phóng, ba nhánh tên đồng motif; cadence spawn hiện hành 0,12/0,15/0,18 s | Ba projectile thật; đừng diễn thành ba cast kéo cung. A/A/A vẫn ba tên, status một roll/unique target |
| Lv13 nội tại | Icon/tooltip mở, hit đủ điều kiện có accent nhỏ tùy chọn | Không thêm aura liên tục hay skill slot; không làm accent thành proc gameplay mới |
| Lv17 Kiếm Khí | Chuẩn bị/tụ rõ hơn, đường ấn 5,5 u, width 0,6 u mở trong một release; tối đa năm impact | Signature bằng silhouette/nhịp/âm sắc; không cần projectile server mới cho Line |
| Lv17 Hàn Tiễn | Một tên mạnh, arrival rõ, nổ lạnh radius 2 u; primary tâm và secondary cùng nhịp nổ | Projectile primary thật + explosion server; flash tâm ngắn, vành nổ gọn. Không thêm tên phụ gây damage |
| Lv18–20 | Gear III và signature giữ nhận diện, status theo loại target | Không tự thêm evolution Lv20 hoặc tăng effect vô hạn theo enhancement |

Lv5 khác normal bằng pose tụ ngắn, release accent, tip/shape và impact cùng motif; không chỉ đổi màu. Lv10 tăng độ rộng/số nhánh cùng thông tin đánh lan. Lv17 tạo “wow” bằng **anticipation rõ → một hình signature lớn nhưng rỗng tâm → kết thúc sạch**, thay vì giữ màn hình trắng. Không kéo action lock để lấy chỗ diễn nếu chưa duyệt timing.

Khởi điểm authoring: slash/wave **4 hình** (mở–active–co–tan), burst **5 hình** (arrival–mở–vành–vỡ–tan), impact **3 hình** (bật–tách–mất). Ít hơn mất hướng/nhịp hoặc thành nhấp nháy; thêm frame chỉ giúp decay mượt, không thêm hit. Dùng lại strip bằng scale/tint/rotation và duration theo profile; không nhân bảy weapon visuals. Đây là dải probe chất lượng, không bộ sprite đã khóa.

## 5. Multi-target: cùng action, đúng thời điểm

**Authority:** combat system trên Game Server chọn/resolve actual targets, collision, evade/crit/status. Client nhận action và các result; không cho SpriteRenderer/ParticleSystem tìm mục tiêu đáng tin.

| Trường hợp | Presentation đề xuất | Mốc damage phải giữ |
| --- | --- | --- |
| Single | Một cast, một đường/impact đúng target; NÉ không impact gây thương | Server hit hoặc projectile impact; không target vẫn cast và tốn cost theo GDD |
| Arc 3 targets | Một sweep; ba impact gọn bắt đầu cùng mốc result. Primary chỉ khác độ nhấn | HitMoment của action, không lan từ A→B→C |
| Line tối đa 5 | Một đường mở toàn chiều dài; impact cùng mốc resolve ở các giao điểm thực | Thứ tự đường đánh dùng power 2,8/2,6/2,4/2,2/2,0; **không suy ra delay theo thứ tự** |
| Hàn Tiễn 1+4 | Arrival primary + một vòng nổ; impact primary/secondary cùng tick nổ. Primary không nổ damage lần hai | Projectile tới trước rồi explosion; secondary không chờ effect bò từ tâm tới từng con |
| Ba tên snapshot | Một fan release gần cùng nhịp, từng tên bay theo aim snapshot; đánh dấu actual impact từng tên | Spawn cách nhau tổng 60 ms; flight/collision có thể khác đáng kể. Không giả các target trúng cùng tick |
| Hai/một target của spread | Hai target A/B/A hoặc một A/A/A nhìn vẫn ba tên; mỗi tên một landed hit nếu thực sự collision | Không thay fallback, không vẽ damage phụ ngoài hit results, không ba status overlays cùng target |

Ví dụ arc resolve tại `t`: thân cast và sweep dẫn tới `t`; cả ba impact tại phase `t`, độ lớn primary lớn hơn nhưng không tới trước secondary. Ví dụ spread: target cách 2 u và 6 u có chênh flight `4/speed` giây dù cùng cast; speed hiện chưa khóa nên không được tự ghi “gần như đồng thời” cho impact.

**OPEN A05:** nếu yêu cầu gameplay mọi target cùng resolve áp cả spread, phải chọn lại executor/speed/spawn contract và chạy balance/PvP/collision test. Recommendation giữ luật projectile V6.1, làm release đọc như một action; không lén dùng tia hitscan để giả tên bay.

Arc/sweep là tự nhiên với Kiếm; fan projectile tự nhiên với Cung; AoE vòng dùng cho explosion. Link cực mảnh có thể thử như **nét Mạch Ấn** ở cùng tick landed results, không chạy chuyền qua từng target. Mặc định bỏ link: thêm đường dễ bị đọc thành chain skill, trong khi impact và sweep đã giải thích đủ. Nếu giữ, không bám target chưa hit/đã invalid, không phủ kín đội hình.

Giảm spam theo camera: một main VFX/cast, một impact nhỏ/landed target, một status instance/type/target. Giữ projectile nguy hiểm và Boss telegraph; giảm hạt/trail/shake của remote trước. Không che kết quả bằng dồn mọi impact vào primary; không dùng thời gian decay effect làm trì hoãn HP/damage feedback. Chạy 2/4+ player và trường hợp 5 target; cap cosmetic phải đo, không giới hạn world ở bốn người.

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

Hybrid mobs cần **pose ranged**, dù dùng cùng Linh Đạn generic: Đạo Tặc phóng/ném; Thạch tụ/phóng mảnh linh lực; Cổ Vệ đưa vũ khí/ấn phát. Đó là khác gesture, không ba projectile families mới. Đạn generic chỉ đổi tint/scale/trail/speed 5/4/6 u/s theo definition; Ong 5,5 u/s. Idle 2 thay 4 tiết kiệm vì thở không phải core; thêm work flourish không phục vụ mob AI P0.

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

Flash nên ngắn, probe 60–100 ms; recoil 1–2 px/60–120 ms chỉ trên visual và khi không làm sai hướng đòn. Ít hơn khó thấy ở tốc chơi, nhiều hơn giống stun và làm nhấp nháy nhiều người; các số này **NEED VALIDATION**, không hit-stop gameplay. Local hit-stop nếu thử chỉ ở presentation local, không dừng timeline server hoặc làm trễ remote hit/telegraph. Với packet gộp nhiều hit, debounce flash cường độ nhưng vẫn giữ từng result đúng; không reset action clock theo số hit.

## 8. Mob death lifecycle và corpse

**Đọc từ:** GDD §4/§6, Technical §6/§7. Respawn normal/Linh hiện tính **deathUtc+25 s**, không tính từ lúc corpse tan hoặc DB ACK. Body không gây contact damage/blocking hiện hành; corpse càng không được tạo chướng ngại mới.

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

**Player khác mob:** death camera/corpse phải còn tới revive/về làng; không áp auto-fade/despawn normal lên player. Corpse Boss contributor ≥10% còn trong area vẫn hợp lệ theo GDD, không pickup tới khi sống. Phiên mất HP0 khôi phục corpse ở SafeAnchor; trạng thái corpse art không quyết định quest eligibility. PvPDefeated dùng kết thúc trận, không mở lựa chọn Hồi Sinh Phù/PvE death.

## 9. Bù Nhìn dùng chung tutorial và training

**Đọc từ:** Q3/Q6 và Analysis farm matrix. Hiện Dummy HP60, không đánh/không thưởng, respawn25 s TEST; Q3 hạ ba life với ≥20% đóng góp/life, Q6 cast active tại yard. Không tạo một dummy tutorial và một dummy training có logic khác trên cùng map.

| Nhu cầu | Kiểm baseline | Recommendation / trade-off |
| --- | --- | --- |
| Q3 Lv3/Mộc Kiếm | ATK suy ra: 12+1,2×2+0,7×4+10=27,2; HP60 khoảng ba landed normal nếu DEF thấp. DEF/EVA dummy chưa ghi đủ | Giữ HP60 cho probe Q3; author rõ DEF/EVA, không lấy ngầm từ mob bất kỳ. TTK tùy miss/DEF, chưa claim chính xác |
| Quest ba kills | Một slot hồi25 s gây ít nhất hai quãng chờ sau các kill, dù combat ngắn | Thử **một prefab/loại**, 3 điểm đứng trong cùng yard và respawn3–5 s; số ba để ba objective kills có sẵn. Đây là thay authoring/timer cần duyệt, không baseline mới |
| Player level cao quay lại | Lv20 Common III cân bằng ATK91,6 có thể one-shot60; không đủ test nhiều action/status liên tục | Đủ xem per-hit damage và đúng weapon/cast; không hứa DPS Meter hay test rotation dài P0. Không tự tăng HP làm Q3 dài |
| N player cùng đánh | Một shared HP/life và ledger; one-shot của người mạnh có thể chiếm toàn life; Q3 không dựa last hit | Server serialize damage/death/respawn, credit từng requester ≥20%; HUD không báo đã hạ nếu chỉ nhìn người khác đánh. Nhiều slot/respawn nhanh giảm chờ, không đảm bảo mọi requester credit cùng life |

**Visual tối thiểu:** Idle1 (dáng tĩnh), Hit2 (cong→bật lại), Break3 (nứt→gãy→đống rơm), dùng frame cuối làm corpse = **6 hình nếu đều khác**. Ít hơn Break dễ giống despawn, hơn hai Hit chỉ làm lắc mượt không dạy mechanic mới. Dummy không Run/Attack; Hit không dừng server action của player. Repeated hit reset/cộng lắc phải clamp, không lắc tới che body/HP bar. Có HP/name, damage/Crit/NÉ cùng pipeline; Death/Break terminal rồi respawn như life mới, không reward vật phẩm/EXP.

**OPEN A07:** (a) giữ60/25 s nguyên trạng rẻ nhất nhưng contention; (b) giữ60, một pool nhiều standpoints và timer nhanh — recommendation để test; (c) adaptive HP theo player hiện diện/life hoặc cá nhân hóa — phức tạp, thay ledger/counter, không nên P0; (d) HP lớn cố định — test mạnh dễ hơn nhưng hại Q3. Prototype hành trình Q3/Q6, late quest và một người mạnh cùng yard trước đổi timer. Muốn rotation test sâu dùng fixture dev HP cao trong validation, không thêm loại dummy player-facing.

## 10. Timing combat, MP và vai trò normal

**Đọc từ:** GDD §3, Analysis §2. Giữ số hiện hành trước test; mô phỏng TTK cũ chưa tính đủ Chí mạng/Chính xác vũ khí và Giày, không bằng chứng chốt nhịp.

| Đại lượng | Hiện hành / suy ra | Probe recommendation, chưa đổi luật |
| --- | --- | --- |
| Normal interval | Tân Lữ1,00 s; Kiếm0,80 s; Cung0,90 s | Giữ làm control; nếu feel sai, thử Kiếm0,70–0,90 và Cung0,85–1,05 s theo cặp, gap khoảng0,05–0,20 s. Không đổi range/damage cùng lượt thử |
| Cung chậm hơn Kiếm | Interval +0,10 s =12,5% dài hơn; tần suất đòn1,111 so1,25/s =11,1% ít hơn | 0,10 s hợp lý để probe draw, chưa chứng minh tối ưu. Với power0,95, raw normal/s khoảng15,6% thấp hơn Kiếm trước hit/crit, bù bằng range/accuracy chứ không riêng art |
| Normal hit/spawn và lock | +0,10 s /0,26 s cho mọi class | Kiếm Attack3 ở12 FPS dài0,25 s gần khớp lock; hit0,10 cần duration riêng. Cung draw0,10 s có nguy cơ quá ngắn. Thử release0,12–0,18 s, lock0,28–0,36 s **chỉ nếu cần**, rerun scheduler/TTK |
| Lv5 core | 2MP/1,0 s, spawn/hit0,12 s, lock0,30 s | Pose prepare/draw cần hoàn trước0,12; dùng hold/time map. Nếu spam làm normal không có vai trò, đo usage rồi thử CD1,2/1,4 s riêng, không mặc định nerf |
| Lv10 core | Kiếm4MP/1,5 s/lock0,30; Cung4MP/1,7 s/lock0,34 | Giữ control; khi cần probe CD ±0,2 s, giữ cost/power trước để tách ảnh hưởng |
| Lv17 signature | 16MP/7 s/lock0,40; Kiếm hit0,16, Hàn spawn0,18 | Cast chuẩn bị có thể giữ bốn pose khác duration. CD6/7/8 s là sensitivity test sau có scene, không tự tăng để VFX dài hơn |

Timing contract là **anticipation → release/hit mốc server → recovery**, tách interval/CD/action lock/flight/VFX decay. Skill4 ở12 FPS dài0,333 s >lock nhập môn0,30 s: nếu giữ duration đều sẽ trễ pose return hoặc khóa người chơi ngoài luật. Đề xuất sampling theo normalized action phase, đặt release khớp mốc server, cắt/chuyển recovery khi action mới hợp lệ; main VFX có thể tan sau actor về Idle. Không queue thêm hit do frame skipped.

Normal còn vai trò: không MP, lấp quãng CD/MP thiếu và gây damage ổn định. Trong giả định skill luôn sẵn đúng CD, Lv10 core chiếm lock khoảng0,30/1,5 hoặc0,34/1,7 =**20% thời gian**; Lv17 thêm0,40/7≈5,7%. Đây chỉ là occupancy suy ra, không bảo đảm normal đạt full attack rate vì mốc CD có thể đụng nhau. Cần log số normal/skill/idle do MP và action lock; không kết luận normal vô dụng từ CD1 s của Lv5.

### 10.1. Kiểm sustain có cơ sở

Phép tính dưới dùng bảng **chỉ số đã cập nhật** trong Analysis, hồ sơ cân bằng/full Common+0 ở đúng band, Food đúng cấp. Upper demand = cost/CD cho core + big khi đã học; không tính travel/evade/lock làm bỏ cast, không dự báo TTK thực.

| Mốc / Kiếm–Cung | MaxMP | Food MP/s | Upper skill MP/s | Thiếu MP/s |
| --- | --- | --- | --- | --- |
| Lv5 / FoodI / core | 121 /133,1 | 0,9075 /0,9983 | 2 /2 | 1,0925 /1,0018 |
| Lv10 / FoodII / tiến cảnh | 171 /188,1 | 1,71 /1,881 | 4/1,5=2,6667 /4/1,7=2,3529 | 0,9567 /0,4719 |
| Lv20 / FoodIII / core+big | 316 /347,6 | 3,95 /4,345 | 4/1,5+16/7=4,9524 /4/1,7+16/7=4,6387 | 1,0024 /0,2937 |

FoodI/II/III hồi MP tương ứng0,75%/1%/1,25% MaxMP mỗi giây trung bình. Không tính regen khi chết/không Food. Thay CD, action lock, Boots hoặc target travel sẽ đổi thời gian hữu hiệu/cost bình; thử có Food, thiếu Food, cân bằng và zero-INT, không dùng full+8 làm chuẩn mọi người.

TTK target cùng cấp/Common+0 hiện early2–4 s, mid3–6, late4–8 s; các loài chỉ có bảy level cố định nên đo actual pair gần cấp, không dựng thêm mob giả cùng mỗi level. Đo trước/sau Q11, solo và nhóm2/3/5 target, hit/miss ở hai tầng, run-back và 2/4 player. Giữ HP curve trước khi có evidence, không tăng máu để bù VFX signature quá dài.

### 10.2. Proc và nhịp feedback

Normal không roll Bỏng/Băng Hàn. Upper successful chance cadence trên một target trúng mọi cast: Kiếm core4%/1,5 s → một proc kỳ vọng mỗi37,5 s; Cung core2%/1,7 s →85 s, Linh1%→170 s. Đại chiêu Kiếm70%/7 s →10 s; Hàn normal45%→15,6 s, Linh30%→23,3 s. Đây là tần suất roll thuận lợi, không uptime vì refresh/protection/miss/đổi target. Không tăng core proc chỉ để art status xuất hiện nhiều.

Giữ Bỏng một overlay và tick1 s, refresh không restart animation entry liên tục; Freeze1,5 s rồi protection3 s target-wide; Boss/PvP chỉ Slow. Ba tên A/A/A chỉ một roll/effect/unique actual target/cast, kể cả fail. Test 1/2/4 Kiếm/Cung lệch pha vì overlay/proc spam có thể che action dù damage không stack. Giữ số proc hiện tại, dùng fixture status có chủ đích để kiểm art thay vì buff gameplay.

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

SafeAnchor, entrance6–8 u và portal approach phải có mặt đứng/đường rõ; không decorate thành bãi nguy hiểm. Kích thước root theo actual layout, không nhân background bằng offset200 u. Một palette environment không đủ thay gameplay room/placement/QA.

## 12. Terrain readability và tile variants

| Ngữ nghĩa | Cue khi nhìn hình | Collider / kiểm |
| --- | --- | --- |
| Solid ground | Mặt trên cap liên tục, khối fill có trọng lượng, edge đậm hơn background | Solid, không đổi một tile giống hệt thành pass-through tùy chỗ |
| Cliff/step | Mép/corner khép, mặt đứng tách mặt trên; cao độ theo block rõ | Step thật, không trang trí gờ nhỏ khiến tưởng đứng được |
| One-way/platform | Mặt sàn mỏng, chân trống hoặc dầm mảnh; edge top đọc rõ | One-way từ dưới, S+Space xuống riêng actor; hint tutorial có thể dùng UI |
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

Navigation đơn giản theo các surface/route authored, không suy walkable từ outline ảnh. **GAP:** Technical chưa chốt thuật toán mob ground đổi tầng; phương án P0 là spawn/leash ground mob trên lane liên thông có thể chase/Return bằng hành vi đã có, hybrid bắn khi khác tầng/blocked. Không mặc định mob có Jump vì player có Jump. Nếu quest/pocket bắt buộc ground mob leo nhiều tầng, cần spike navigation và duyệt trước vẽ toàn công trình; không thêm AI parkour ngầm.

Mini kit giả định gỗ gồm bridge-floor3 +roof3 +support2 +wall/arch2 =**10 logical modules**, **8 hình** nếu hai cặp cap trái/phải thật sự mirror được. Nếu motif không đối xứng thì10 hình. Đây là ví dụ derivation, không bắt phải có đủ roof trên mọi map; stone kit có thể dùng terrain hoặc một bộ cap/mid/support riêng sau route test. Landmark độc nhất không ép thành kit mười loại chỉ để “modular”.

Test một công trình: lower lane và upper standable surface, một spawn đúng platform, đường tiếp cận Kiếm, Bow bắn không xuyên solid roof, mob Return không mắc cột, người drop-through không làm người khác rơi. Khoảng trống Boss15 u TEST và telegraph tránh bị mặt sàn foreground che.

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

Contact ripple chỉ cosmetic, không thêm Slow/Burn/HP hazard; pooling reset phase/scale/owner. Actor đi trên cầu phía trên nước không splash vì chỉ chồng hình2D; cần surface/contact tag presentation đúng lane. P0 không deep-water, breath, buoyancy/swimming. Nếu thác là background, không vẽ ledge giả hoặc trigger tương tác không có luật.

## 15. Map ↔ mob ↔ vũ khí

| Ca gameplay hiện có | Failure mode visual/route | Hợp đồng authoring và validation |
| --- | --- | --- |
| Spawn platform/công trình | Chân ở giữa không khí, slot trên sàn không tới được | Spawn anchor trên surface hợp lệ, clearance đủ body/HP bar, hurtbox không xuyên sàn; identity/level giữ manifest |
| Chase/Return | Mob rơi tầng dưới rồi teleport lên, hoặc mắc dầm/cột | Route cùng lane/linked surfaces có hành vi AI hỗ trợ; leash8 u và spawn reachable. Return không dùng đường ảnh tưởng có mà collider chặn |
| Melee Kiếm | Cao độ khiến range1,2/1,7 u không chạm, ngay cả nhìn gần | Kiểm vertical hit band/originY+0,8 u với geometry; không dời hitbox theo trail để “sửa art” |
| Cung/ranged hybrid | Ảnh tên xuyên roof rồi server báo MISS; sàn one-way chưa rõ cản đạn | Author và chốt collision mask projectile với ground/platform; solid phải nhất quán. **OPEN A10** one-way chặn đạn hay không, không quyết qua sprite alpha |
| Flying Ong | Hover box6×3 u khiến ở cao ngoài tầm Kiếm vô hạn | Engage approach vào melee-accessible band như GDD; room có route/jump thật, không yêu cầu Kiếm có skill mới để tới |
| Multi-floor | Target distance gần nhưng khác tầng, target lock/hit xuyên đá | Server MapId/line/arc/flight/vertical validation; foreground và platforms đọc được khoảng cách thực |
| Linh scale1,20–1,30 | Sprite overlap roof, người hiểu hurtbox lớn hơn trong khi physics không scale | Aura/name/HP bar chính, scale vừa phải theo clearance. Nếu scale mờ pixel, thử tint/accent trước đổi physics |
| Pocket spacing | Cầu nối khiến aggro5 u kéo cả map hoặc leash cắt route | Kiểm tâm18–20 u/terrain tách, safe strip, line/arc grouping; không ép mọi pocket thành blob0,8–1,5 u |
| Loot sau flying death | Món nằm trên ledge kín hoặc trên không | Server-owned điểm pickup đứng được; collider route/1,5 u pickup test cùng lifecycle §8 |

Không phải mọi mob đều đi mọi tầng của công trình: author phạm vi hợp lệ và thể hiện bằng vị trí/spawn/route, hybrid dùng ranged fallback hiện có. Nhưng mọi encounter bắt buộc quest phải có cách Kiếm và Cung tiếp cận/đánh/nhặt bằng bộ kỹ năng hiện hành; không dùng effect đẹp che softlock. Không thêm body blocking, knockback, hazard, moving platforms hoặc navigation framework trước khi room prototype chứng minh cần.

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
| Active profiles | 6=Kiếm/Cung×Lv5/10/17 | Hai motif class với độ mở/rune khác theo milestone; Lv5/10 cùng slot/SkillId nhưng icon/profile tooltip phân biệt được |
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
| Skill | Hai active, hai passive; evolved slot và manual/level lock lý do | Mười profile/passive icons §18, rows/tooltips; không talent tree |
| Quest | State/group/count/return NPC/level gate; Q9 optional tách pin | Rows/markers; evidence virtual, không drag-drop quest item |
| Journey | Score và category/record tổng hợp đúng hệ thống hiện có | List/progress/title; optional emblem dùng motif Mạch Ấn |
| PvP | Invite/opponent picker, exact wager/pot/fee, pending escrow, countdown/120 s/quota, result/pending settle | Two columns +badges/countdown numbers; không portrait/arena splash bắt buộc |
| Login/Character Select | Logo/title giản dị, list và selected preview, feedback kết nối/lỗi | Kit và rig; dependency §21 |
| Stage Summary | Ba title/chương, reward/progression/story completion text | Một modal template, ba accent/thumb crop map sẵn; không ba tranh full-screen bắt buộc |
| Inventory/Storage/Shop/Sell | Grid/list, capacity30/40, stack count/ownership, price/stock state | Cùng kit; Storage không item background riêng |
| Class choice/manual use/attributes reset | Choice cards +reason/preview/confirm | Hai class motifs/kit; không thêm tạo nhân vật hoặc menu học riêng NPC |
| Dialogue/Rest/Death | NPC text/action, confirm heal/death choices khác PvP | Panel/buttons; Hồi Sinh icon, không tám portraits bắt buộc |
| HUD/world UI/chat/portal | Bars, quest tracker, food/bình, nameplate/reward eligibility, bubble hai dòng, signpost, Boss timer/banner | Kit/typography/marker glyphs; không HP bars Party, chat history/quest arrow P0 |

Pending là core online: khóa thao tác lặp cần thiết, giữ dữ liệu committed, cho biết đang xử lý; timeout/network error không dùng success/fail RNG animation. Equip preview có thể xem trước visual, nhưng HUD/world nhận canonical result đúng revision. Success enhance kể cả fail cost đã tiêu chỉ sau ACK, không kéo animation giả để che một request chưa có kết quả.

**Dependency bổ sung:** font có đủ dấu tiếng Việt, số/+/% dễ đọc; test tên dài “Đóng Băng”, “Huyền Nham Cự Thú”, trạng thái thiếu X ô, wager10.000 và chat80 ký tự/hai dòng. Text dynamic không bake thành sprite. Chọn world reference resolution/zoom và UI scale **OPEN A12** trước export: thử480×270 hoặc640×360 cho16:9 (1920×1080 scale4 hoặc3); world view rộng15 hoặc20 u ở PPU32, ảnh hưởng đọc Bow6,5 u/telegraph. Đây là probe camera, không requirement render resolution đã khóa hay scene mới.

## 20. Presentation online server-authoritative

**Đọc từ:** GDD §8, Technical §1/§3/§4/§6. Local anticipation cosmetic không mở thêm prediction/rollback physics/combat P0. Backend không nằm trên mỗi hit, nhưng reward/consume/enhance/PvP settle cần durable ACK.

| Bước | Local player | Game Server / remote |
| --- | --- | --- |
| Bấm Attack/Skill | Ngay frame input: đổi pose chuẩn bị/draw, weapon đúng visual, âm/tụ lực cosmetic; ghi action request đang chờ | Intent có binding/sequence/aim; client không gửi trusted targets/damage |
| Validate/start | Giữ UI resource/cooldown canonical, có pending cue nhẹ khi cần; không hiện target impact/HP trừ giả | Server kiểm alive/MapId/class/learned/profile/MP/CD/action lock, chụp source/action origin/weapon visual và startClock, commit cost realtime |
| Accept/reject | Ghép request với actionId, căn phase theo server clock; reject trả pose phù hợp và reason, clear tentative effect | Remote bắt đầu từ phase còn hiệu lực, không chạy lại windup từ đầu ở packet trễ |
| Release/projectile | Local slash/cast accent có thể anticipate cosmetic; khuyến nghị chờ authoritative spawn để tạo projectile chính trong slice | Server schedule hit/spawn; replicated projectile/event có ID/index/life, origin/aim/speed/expiry để render flight |
| Hit/multi-target | Chỉ HitResult mới damage/Crit/NÉ/landed impact; giữ cùng timestamp đối với batch arc/line/nổ | Server collision/target validation resolve actual IDs, per-target result/status; remote thấy cùng outcome, không VFX overlap damage |
| CC/Death | Present cancel/freeze/terminal đúng state, bỏ pending hit visual của action bị hủy | Server generation/cancel reason/expiry; projectile đã spawn tiếp tục MapId gốc như GDD |
| Loot/consume/progression | UI pending, success/pile/reward/HP potion effect theo ACK quy định | Game Server tính result, backend commit bền vững; remote/pickup đúng phase/receipt |

Nếu latency làm projectile xuất hiện muộn, render ở phase flight hiện tại và nối muzzle trail ngắn để đọc nguồn; không phát một tên thứ hai từ pose release. Option projectile local tentative có thể giảm delay hơn nhưng tăng ghost shot/dedup complexity; **OPEN A11**, chỉ thử sau baseline, không dùng collision của nó gây damage. Cung cũng không cần biến mọi arrow thành NetworkObject riêng nếu event/state mô tả flight đủ; cách transport là Technical spike, art không chốt implementation.

**Thông tin presentation cần đề xuất thêm vào contract**, không code/khóa schema: actionId +client request correlation, actor/life generation, MapId, profileId, startClock và timeline revision; visualId/equipment revision chụp cast; projectileId/index/origin/aim/speed/spawnClock; hitIndex +actualTarget/life +resolveClock +evade/crit/damage/remainingHP; status kind/source/expiry; terminal/cancel/lifecycle phase. Fields chỉ là dữ liệu server để diễn đúng, không trust client echoes. HitResult hiện có chưa nêu đủ timestamp/generation cho reuse/late packet.

Network20 Hz có khoảng snapshot50 ms, physics50 Hz tick20 ms, render60 FPS frame≈16,7 ms hiện là baseline. Không nên stream từng sprite frame: clock/phase profile deterministic chọn pose local, correction ở action/state/result. Chênh server tick không được thành delay truyền targetA→B→C. Remote interpolation và local anticipation phải cùng semantics, không giả cam kết mọi effect zero-lag.

Dedup VFX theo actionId/projectileIndex/hitIndex/targetGeneration thích hợp; cache status theo state revision, không replay entry trên mọi snapshot. Death terminal của life cũ thắng Hit/Status đến trễ; mob respawn không nhận effect life trước. Join/reconnect snapshot dựng status còn hiệu lực, không replay damage/reward; portal clear visuals của map cũ, projectile đã spawn vẫn thuộc map cũ cho viewers ở đó. Một actor disconnected trong grace15 s vẫn bị đánh; không biến ghost render thành invulnerable. Map filter chỉ presentation, không disable server root/player khác.

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

Một rig preview reuse Body/Hair/outfit/weapon assets, không sprite-sheet riêng màn chọn nhân vật. Không claim cây gear thật nếu chỉ nhận class. Khi data thiếu, default preview có nhãn cấp/phái đủ, tránh placeholder fake Rare III làm người chơi tưởng được cấp đồ. Login dependency là UI states/font/input/focus/background và preview source, không thiết kế lại ticket/lease/account schema.

## 22. Technical Art Contract đề xuất

**Đọc từ:** Technical §8; giữ thông số hợp lý, tách thông số còn cần validation. Bảng sau là proposal để sync, không importer/code được tạo trong lượt này.

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

Unity hướng dẫn cùng PPU, Point filter và Compression None cho sprite pixel; atlas có padding mặc định4 và setting rotation/texture riêng. Đây là cơ sở kiểm import, **không tự pin phiên bản Unity/URP**; spike chọn supported LTS/package như Technical. [Nguồn Unity — chuẩn bị sprite pixel](https://docs.unity.com/en-us/engine/6000.6/manual/unity2d/2d-urp/2d-pixelperfect/prep-sprites), [Sprite Atlas reference](https://docs.unity3d.com/6000.3/Documentation/Manual/sprite/atlas/sprite-atlas-reference.html).

ObjectPool cung cấp cơ chế reuse object, còn reset/dedup/generation ở trên là contract đề xuất của Huyền Lộ, không engine tự bảo đảm. [Nguồn Unity — ObjectPool](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Pool.ObjectPool_1.html). Không thêm lighting stack, Addressables hay skeletal package chỉ để đạt contract này.

## 23. Production accounting: đếm công cần làm, không đếm item×animation

**Đơn vị:** pose key là hình dáng cần đọc; frame/ô logical là mẫu trên timeline; sprite outcome là ảnh raster khác được xuất; variant là sửa từ template; module là một family part; preset là config VFX; placement là instance trong map. Một ảnh dùng ở năm ô vẫn một ảnh, nhưng năm chỗ sync phải được kiểm. Một recolor xuất PNG khác là variant outcome, không công vẽ silhouette mới.

### 23.1. Kịch bản player S0 có thể kiểm lại

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

### 23.2. Tổng quan các family còn lại

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
| Map content placement | 8 roots,5 farm maps/28 pockets/66 slots hiện hành | Reuse art không reuse toàn route | Colliders/spawns/portal/safe strips/Boss exclusion/traversal/combat QA là cost editor thật |

Mười VFX family để không giấu scope trong “4 active”: **(1)** sword slash, **(2)** arrow/flight kể cả generic mob config, **(3)** signature line/wave, **(4)** explosion/burst, **(5)** landed impact, **(6)** telegraph cone/ground/landing, **(7)** statuses Burn/Freeze/Slow, **(8)** aura Linh/Cuồng, **(9)** beneficial heal/MP/Food/revive, **(10)** kết quả reward/loot/enhance/death cosmetic. Family là nhóm reuse/QA, không buộc một prefab đa năng. Status family cần ba ngôn ngữ khác nhau, không gọi cùng một tint là đủ cả ba.

Frame probe slash4/wave4/burst5/impact3/Freeze shell2+thaw3/flame4 ở §4/§14/§17 có lý do theo phase. Particle/glint/sigil thường chỉ một primitive; không cộng mỗi lifetime tick thành frame. Main active vẫn bốn SkillId presets với sáu cấu hình profile; normal/telegraph/status ngoài active được tính độc lập. Tổng sprite VFX chỉ chốt sau manifest xác định shared textures và technique, tránh double-count flame/aura/arrow.

**Cách ra production cost thực:** đo giờ riêng cho tạo pose template mới, sửa silhouette variant, recolor/cleanup, export/slice, socket/sorting setup, integration, QA và rework. Ước lượng `Σ(số pose mới×giờ/pose mới + số sửa variant×giờ/sửa + công setup/QA)` từ slice đạt chất lượng, không từ credit hoặc phép nhân item count. Ghi cả lần sửa Bow và Death thất bại; không dùng lần recolor nhanh nhất làm tốc độ vẽ toàn bộ Boss. Chưa có asset/slice nên **không có căn cứ chốt tổng giờ**; mốc160–240 h cũ đã bị Technical loại là estimate hiện hành.

Ma trận QA tối thiểu: mỗi Armor với Sword Attack và Bow draw/Skill; mỗi LowerBody với Run/Jump/Fall/Death; cả 7 weapon với Attack, 6 weapon class với Skill, ở hai hướng; một số mix-band mặc chéo và default/unequip. Nếu kiểm toàn bộ: 4 Armor×4 LowerBody×7 weapon=112 outfit/weapon combinations trước hướng/pose, nhưng không vẽ 112 rigs. Kiểm theo part/contact và ca mix-band rủi ro, không giả bỏ integration nhờ reuse.

## 24. Prototype/validation trước production hàng loạt

Đây là **khuyến nghị validation**, không kế hoạch implementation, PixelLab, free trial hay generate asset. Các ca dưới **CHƯA CHẠY**; dùng placeholder/fixture phù hợp khi làm Unity slice, chưa nghiệm thu bằng tài liệu.

| Ca | Nội dung phải thử | Dấu hiệu đủ để ra quyết định |
| --- | --- | --- |
| P01 Modular player | Default +I/II/III Armor/Lower, mix-band, unequip, cả state và hai hướng | Không hở thân/grip sai/nhảy pivot; xác nhận pose refs nào thật sự reuse; quyết26 logical hay physical và delta class/airborne |
| P02 Hai Sword cùng action | Mộc và Huyền Ấn dùng cùng track Attack; hai Kiếm class khác band dùng cùng Skill; thử swap giữa action hợp lệ | Đúng cây ở cả Idle/release, không sprite Common baked; socket/góc raster đủ; visual snapshot không morph sai; không cấp skill class cho Tân Lữ chỉ để test Mộc |
| P03 Bow | Ít nhất hai band rest/draw/release/Skill, normal và triple | Tay/string/arrow nock khớp; spawn đúng timeline, ba tên một cast; quyết có partial-draw thêm và normal projectile contract |
| P04 Multi-target | Arc3, Line5, Hàn1+4, spreadA/B/C–A/B/A–A/A/A, no-target/invalid | Arc/line/nổ impact cùng resolve phase; spread đúng flight, không chain false; không double status/impact/projectile |
| P05 Hit không stun | Mỗi rig bị hit Idle/Move/Windup/Attack/Ranged; Freeze trước/sau release; lethal | Main action không restart/hit trễ vì flash; CC server cancel đúng pending, đã bay vẫn sống, Death thắng stale Hit |
| P06 Death/corpse | Ground/Ong/Boss; chết ở ledge, late viewer, ACK chậm/retry, normal respawn 25 s | Corpse không target/collider/AI, loot đứng được, timer từ death không từ clip/ACK, đời mới không nhận effect cũ |
| P07 Dummy online | Q3/Q6 solo, hai ngườiLv3, Lv3+Lv20,3–4 requesters | HP/Break/respawn thật, credit≥20% đúng từng life, không hai loại dummy; đo chờ để chọn timer/standpoints |
| P08 Terrain room | Solid/step/one-way/pass-through/overpass và background cùng chất liệu | Người chưa biết collider đoán đúng trước khi nhảy; drop một actor không làm người kia rơi; Kiếm/Cung đọc được target |
| P09 Công trình | Hai surfaces/routes +spawn/hybrid/Ong/chase/leash/Return/melee/Bow | Không stuck/unreachable/spawn trên không; projectile mask đúng solid/platform; không ngầm cần mob Jump/navigation mới |
| P10 Nước | Puddle base/front, thác seam, actor dưới nước/đi trên cầu, cả facing | Front chỉ che chân, không telegraph; bridge không splash, không thay physics; quyết sprite/particle reuse |
| P11 UI kit | Inventory/Storage/Shop cùng kit, Character/Enhance/Transfer, Vietnamese text/tooltip | Không bitmap screens riêng, rarity/band/+level rõ; pending/error/RNG fail khác, focus/input không phát attack |
| P12 Online presentation | 2 clients Dedicated baseline; thêm4+, RTT0/100/200 ms, jitter/loss probe | Immediate local pose, no trusted client damage, one effect per result, late death/status/action đúng generation, map/reconnect không replay reward |
| P13 Combat/balance | Intervals/timing control; Lv5/10/17, MP zero-INT/cân bằng, TTK solo/cụm và PvP Food/quota | Ghi số normal/cast, thời gian chờ vì MP, proc/TTK/flight; đổi một nhóm số mỗi lượt, chạy lại model sau khi timeline thay |
| P14 Boss/camera | Telegraph0,5/1,0/1,2 s, ba landing zones, Cuồng dưới30%, Slow, nhiều VFX/người | Telegraph không bị che/cắt hoặc time-stretch, đọc/né bằng movement hiện có; camera/zoom đọc được range |
| P15 Chi phí slice | Một outfit family, Sword/Bow, mob hybrid và room/UI primitive nhập hoàn chỉnh | Manifest unique/dedup/variant/setup và giờ thật; derive lại accounting, không production hàng loạt với frame count chưa kiểm |

Ghi build/Editor/packages, camera scale, machine và seed/fixture; không claim benchmark capacity từ một ca chạy mượt. Nếu vấn đề là pose không đọc, sửa art/timing presentation trước; nếu cần đổi hit/spawn/CD/collider/loot position/timer, trình như gameplay/Technical proposal, chạy ca phụ thuộc. Validation không tự khóa mọi số đề xuất trong file này.

## 25. OPEN decisions tập trung

| ID / vấn đề | Options và trade-off | Recommendation hiện tại | Cách chốt |
| --- | --- | --- | --- |
| A01 Ý nghĩa 26 frame | 26 ảnh tổng: chặt budget nhưng có thể thiếu Bow; 26 ô/profile dùng lại ảnh: tăng vài upper pose; hai bộ đầy đủ: dễ author nhưng dư locomotion | 26 ô baseline/profile, pose map chung, upper body riêng theo class; **cần duyệt lại diễn giải user-lock** | Duyệt design + P01/P03, không tự tuyên bố lock đã đổi |
| A02 Technique/weapon poses | Raster toàn bộ: sạch nhưng tốn; socket/skeletal thuần: ít ảnh nhưng rủi ro outline/khớp; hybrid: setup vừa và pose chính sạch | Hybrid body raster/weapon socket, góc sửa khi cần; 13–25 weapon outcomes chỉ là kịch bản | P01–P03 và chi phí P15 |
| A03 Boots | Giữ stat-only: không cost world; bỏ: giảm gear nhưng đổi economy/balance; footwear slot có visual thật: thêm layer | Giữ stat-only P0, LowerBody chứa footwear mỹ thuật | Có thể quyết bằng design; nếu bỏ phải tính lại P13/loot/traversal |
| A04 Skill/VFX concept | Nét Mạch Ấn, kiếm khí/băng hoặc linh ảnh: khác sắc thái/cost; linh ảnh lớn dễ bị hiểu là entity đánh thêm | Motif linh mạch ấm/lạnh, 4 active presets/6 profiles; signature bằng hình và nhịp | P03/P04/P14; concept cụ thể chưa khóa |
| A05 Multi-target Cung | Flight hiện hành: giữ collision nhưng impact lệch; đổi speed/spawn: giảm lệch, có thể đổi né; resolve đồng thời: đổi gameplay lớn | Giữ một fan release và impact server. Nếu bắt damage cùng nhịp, duyệt contract mới riêng | Design + P04/P12/P13, art không tự sửa |
| A06 Normal/timing/CD/proc | Giữ baseline; đổi duration pose nhưng giữ clock; tune hit/lock/CD nếu cảm giác sai | Giữ 0,80/0,90 s và MP/CD/proc; thử release Bow/timeline trước đổi damage/HP | P03/P13; các test ranges §10 không là acceptance mới |
| A07 Dummy | HP60/25 s cũ: rẻ nhưng chờ; HP60 + pool nhanh: giữ Q3 nhưng vẫn one-shot; HP lớn/scaling: test dài hơn nhưng phức tạp hoặc hại Q3 | Một prefab/yard, HP60; thử 3 điểm đứng/respawn 3–5 s; không DPS Meter/scaling P0 | P07; timer/count thay đổi cần duyệt GDD |
| A08 Corpse/flying loot | Fade nhanh: sạch; hold lâu: thấy kill nhưng clutter; rơi visual: tự nhiên; tan tại chỗ: rẻ; chiếu loot xuống nền: reachable nhưng cần luật server | Ground hold 0,5–1,5 s/fade 0,3–0,5 s; Ong rơi visual/fallback tan; loot server chọn điểm hợp lệ | P06/P09; corpse art và loot gameplay quyết riêng |
| A09 Terrain/building | Cell 16/32/64 px; tile-only hoặc mini kit/full asset; ít variant giảm cost nhưng lặp texture | Thử 32 px, 17 semantic + 4 cosmetic/họ; kit cho motif lặp, full asset cho landmark độc nhất | P08/P09/P15; blockout quyết số module thật |
| A10 Tầng/AI/projectile mask | Ground cùng lane reachable: rẻ; navigation đổi tầng: linh hoạt nhưng tăng logic; platform chặn đạn/cho xuyên: đều cần cue và balance | Author mob trên lane AI hiện có hỗ trợ, hybrid fallback; solid chặn nhất quán; one-way mask chưa khóa | Technical/Design + P09; không mặc định mob Jump |
| A11 Anticipation online | Chỉ local pose: rẻ nhưng chờ projectile; projectile tạm: mượt hơn, cần xử lý ghost/dedup; rollback combat: tăng scope | Pose/âm/cast cosmetic ngay, projectile chính/result authoritative; projectile tạm chỉ khi đo thấy cần | P12; không thêm prediction/rollback physics P0 |
| A12 Camera/UI/pixel scale | 480×270 hoặc 640×360 và policy màn hình khác; icon 32 px/lớn hơn; snap/rotation | Giữ PPU32, thử world view 15/20 u, UI font đủ dấu; chọn readability trước export | P01/P08/P11/P14; pin pipeline ở Technical spike |
| A13 Select preview | Default/class preview: không thêm data; exact gear: reuse rig nhưng cần canonical visual summary | Default preview/name/level/class; exact gear chỉ nếu duyệt dependency read-only | Design/Technical + P11/P12; không account redesign |
| A14 Action khi nhảy/chạy | Full pose attack trên không: ít setup nhưng chân lệch; upper action + lower locomotion: reuse tốt nhưng overlap khó; pose riêng: đẹp nhưng tăng cost | Không thêm movement lock để cứu art; thử hành vi được cho phép, thêm đúng pose/track thiếu | P01/P03/P09; khóa movement/jump là gameplay proposal |
| A15 Death terminal-pending | Chờ ACK mới Death: nhất quán nhưng trễ; terminal state → Death ngay: đọc tốt nhưng cần event/state riêng | Diễn theo server terminal, success/reward sau ACK; deathUtc/deadline không tự đổi | Technical + P06/P12; không local đoán chết |
| A16 Boss scope | 28 pose core hoặc thêm 4 roar; canvas 128/192 px; roar pose riêng hoặc overlay | Giữ behavior/telegraph, Cuồng không chen action; canvas theo room, roar optional | P14/P15; ngưỡng 30% không tự tạo action mới |
| A17 Accounting thực | Xuất PNG variants: nhiều ảnh; palette reuse: ít ảnh nhưng có setup; thêm shape: silhouette tốt và tăng cost | Thay S0 bằng manifest/giờ slice; không chốt tổng giờ khi chưa có evidence | P15 và review scope trước production hàng loạt |

## 26. Nội dung cần sync về V6.1 NẾU recommendation được duyệt

Không file nguồn nào được sửa trong lượt này. Sync phải giữ một domain authority cho mỗi luật; file 4 là phân tích tham chiếu, không catalog thứ hai. Nhóm presentation có thể chốt độc lập; nhóm gameplay phải review riêng trước chỉnh V6.1.

| File / vị trí hiện có | Nội dung cụ thể cần cập nhật sau duyệt | Điều kiện/dependency |
| --- | --- | --- |
| GDD §0/§9/§10 | Làm rõ lock 26 là ô/profile hay tổng hình; class pose/dedup/default outfit; 13 gear world modules thay 12 | A01/A02/P01–P03; đồng bộ acceptance modular, không âm thầm bỏ user-lock hoặc thêm item |
| GDD §1/§6/§9 | Bảy weapon visuals kể cả Mộc; silhouette ba band; Armor/LowerBody+footwear; Ring/Necklace/Boots stat-only | Item/stat/rarity tách visual; không đổi catalog/range nếu chỉ duyệt art |
| GDD §2/§6/§10 nếu bỏ Boots | 5 slots/15 families/18 regular+Mộc; DEF/EVA/MoveSpeed/enhance/Tinh Hoa; slot pool/shop/sell/transfer/fixtures/DoD | Chỉ khi option bỏ A03 được duyệt; tính lại balance/economy, không tự chuyển stat |
| GDD §3 | Executor/speed/flight normal Cung còn thiếu; timeline riêng theo class nếu retune; quyền movement/jump lúc cast; sáu profile/bốn SkillIds; semantics multi-target | A05/A06/A14; giữ fallback SnapshotSpread/status dedup/Line falloff/Hàn không double-hit trừ khi luật mới được duyệt |
| GDD §3/§4/§9 | Damage reaction không stun, CC/Death cancel, normal/flying death/corpse, status/Boss telegraph dễ đọc | Presentation không đổi hit/CC; loot ground position hoặc mốc respawn đụng gameplay phải review riêng |
| GDD §4/§9 | Structural standables/surfaces, AI reachable/leash/flying approach, cue solid/one-way, nước visual-only, camera readability | Giữ 8 roots/28 pockets/66 slots/gates; không thêm AI Jump/hazard/swimming; mask projectile cần chốt |
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

**Tự review tài liệu:** đã kiểm ranh giới art ↔ combat ↔ map ↔ online và phép tính S0/icons/mob/timing. Con số production có tập giả định, hoặc được ghi rõ chưa derive được; chưa có runtime evidence. Các đề xuất có thể đổi gameplay đã tách: ý nghĩa lock 26, executor/timing normal Bow, spread đồng thời, Boots, dummy timer/HP, navigation/projectile mask/loot point. Corpse/status/VFX không quyết định damage, respawn hay credit; preview và pending không được giả success đã commit. OPEN §25 và prototype §24 là điều kiện trước chốt production; các nội dung sync trên chưa được áp dụng.
