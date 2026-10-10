# Huyền Lộ — Items & Economy

Các số BASELINE/TUNABLE chưa phải nghiệm thu runtime.

**RULE:** Food là nguồn sustain MP chính khi farm dài; người chơi chủ động mua/dự trữ. Nếu sustain yếu, ưu tiên kiểm/tune Food trước. Không thêm free MP regen, zero-MP class Normal, giảm cost S1/S2 hoặc auto-consume Food.

## Document owns

Catalog, sáu ô đồ, enhancement/transfer, Food/Potion, inventory/shop, shared loot, Gold/Journey và cược PvP.

## Document does not own

Luật ngoài domain thuộc owner trong [documentation map](../README.md); evidence thuộc [Playtest & Balance](../04-production/playtest-and-balance.md), thứ tự triển khai thuộc [Roadmap](../04-production/roadmap.md).

<a id="gear-economy"></a>
<a id="gdd-7"></a>

<a id="6-trang-bị-đồ-rơi-và-kinh-tế"></a>

## Trang bị, đồ rơi và kinh tế

## Danh mục — 18 dòng trang bị thường

Sáu ô × ba bậc tiến trình = **18 family trang bị**. Vũ khí mỗi bậc tách Kiếm/Cung: **21 mẫu thường cụ thể** (15 mẫu không phải vũ khí + 6 vũ khí); phẩm chất/cấp cường hóa thuộc trạng thái instance. **22 ItemDefinition trang bị = 21 mẫu thường + Mộc Kiếm Q3**. Tổng này chưa tính Food/Bình/bí kíp/nguyên liệu/phù. Mộc Kiếm là 1 tutorial exception ngoài 18 families; ATK 10 để giữ Q4 TTK khi basic Tân Lữ là 0,70 s (mô hình 1,00 s cũ là LEGACY), không sell / drop. Weapon class lock giữ, Family I Sword / Bow cần Lv 5 dù non-weapon mặc từ Lv 1.

**Mốc mặc đồ dễ nhớ:** Band I non-weapon Lv 1, Band I Weapon Lv 5 sau chọn class, **mọi món Band II Lv 11**, **mọi món Band III Lv 17**. Nguồn gear theo mob/content, không suy từ tên map; bảng source bên dưới là authority chung cho loot/potion/material. Shop chỉ Common I/II; III không bán, Q11 Rare weapon đúng class là guaranteed exception. Mộc Kiếm quest-only, ATK 10, không bán/cường hóa/chuyển giao. Off-class weapon bán được, không equip.

Weapon shop chỉ mua khi đã chọn đúng class; đồ II có thể mua/nhặt từ Ong Lv 10 trước Lv 11 nhưng chưa mặc.

**STRONG DIRECTION:** Armor/Pants/Boots luôn có HP; Weapon/Ring/Necklace luôn có MP. Armor HP mạnh + DEF, Pants HP vừa + DEF, Boots HP nhẹ + EVA/tốc chạy; Weapon ATK + MP + secondary theo phái, Ring MP + ACC/Crit, Necklace MP mạnh + EVA/utility. Stat do `ItemDefinition/GearSlot` quyết định, không suy từ vị trí trái/phải trên UI.

**Catalog — Common +0, PROBE BASELINE/TUNABLE 2026-10-06.** HP/MP được tăng vừa phải để kiểm cadence mới; ATK/DEF/ACC/EVA/Crit/tốc chạy, giá, nguồn và trần enhance giữ nguyên. [Playtest & Balance](../04-production/playtest-and-balance.md#current-balance-probe) giữ so trước/sau, sustain và biên rarity/enhance. Rarity drop theo channel table; shop không bán Uncommon/Rare/Epic. Cột source I/II/III dẫn bảng mob/content; ngoại lệ quest ghi riêng.

Mỗi bậc có hai mẫu vũ khí mang tên riêng theo phái, cùng ATK/giá nhưng Kiếm thêm Chí mạng và Cung thêm Chính xác; **18 family = 21 mẫu thường** vì ba family vũ khí tách Kiếm/Cung. Mộc Kiếm là mẫu ngoại lệ ngoài 18 family.

| Bậc / mẫu | Ô | Chỉ số Common +0 | Cấp mặc / phái | Nguồn | Mua / bán Common, Vàng |
| --- | --- | --- | --- | --- | --- |
| Ngoại lệ — Mộc Kiếm | Vũ khí nhập môn | ATK 10 | Q3 / Tân Lữ | Q3 cấp một lần; không rơi | Không mua / không bán |
| I — Thanh Mộc Kiếm | Vũ khí Kiếm | ATK 15 / MP 10 / Chí mạng +0,5 điểm % | 5 / Kiếm | Shop + I drop; Q6 nếu chọn Kiếm | 300 / 75 |
| I — Thanh Mộc Cung | Vũ khí Cung | ATK 15 / MP 10 / ACC 10 | 5 / Cung | Shop + I drop; Q6 nếu chọn Cung | 300 / 75 |
| I — Áo Thanh Mộc | Áo giáp | HP 44 / DEF 4 | 1 / mọi phái | Shop + I drop; Q4 guaranteed | 220 / 55 |
| I — Quần Thanh Mộc | Quần | HP 28 / DEF 3 | 1 / mọi phái | Shop + I drop; Q3 guaranteed | 160 / 40 |
| I — Giày Thanh Mộc | Giày | HP 12 / DEF 2 / EVA 4 / tốc chạy +1% | 1 / mọi phái | Shop + I drop | 140 / 35 |
| I — Nhẫn Thanh Mộc | Nhẫn | MP 8 / ACC 6 / Crit 1% | 1 / mọi phái | Shop + I drop; Q7 nếu thiếu | 140 / 35 |
| I — Dây chuyền Thanh Mộc | Dây chuyền | MP 24 / EVA 5 | 1 / mọi phái | Shop + I drop | 160 / 40 |
| II — Vân Nham Kiếm | Vũ khí Kiếm | ATK 28 / MP 18 / Chí mạng +1 điểm % | 11 / Kiếm | Shop + II drop | 650 / 162 |
| II — Vân Nham Cung | Vũ khí Cung | ATK 28 / MP 18 / ACC 20 | 11 / Cung | Shop + II drop | 650 / 162 |
| II — Áo Vân Nham | Áo giáp | HP 99 / DEF 9 | 11 / mọi phái | Shop + II drop | 500 / 125 |
| II — Quần Vân Nham | Quần | HP 61 / DEF 6 | 11 / mọi phái | Shop + II drop | 380 / 95 |
| II — Giày Vân Nham | Giày | HP 28 / DEF 4 / EVA 8 / tốc chạy +2% | 11 / mọi phái | Shop + II drop | 300 / 75 |
| II — Nhẫn Vân Nham | Nhẫn | MP 14 / ACC 10 / Crit 1,5% | 11 / mọi phái | Shop + II drop | 320 / 80 |
| II — Dây chuyền Vân Nham | Dây chuyền | MP 44 / EVA 8 | 11 / mọi phái | Shop + II drop | 330 / 82 |
| III — Huyền Ấn Kiếm | Vũ khí Kiếm | ATK 40 / MP 26 / Chí mạng +1,5 điểm % | 17 / Kiếm | III drop / Boss; Q11 Rare nếu chọn Kiếm | Không mua / 450 |
| III — Huyền Ấn Cung | Vũ khí Cung | ATK 40 / MP 26 / ACC 30 | 17 / Cung | III drop / Boss; Q11 Rare nếu chọn Cung | Không mua / 450 |
| III — Áo Huyền Ấn | Áo giáp | HP 138 / DEF 15 | 17 / mọi phái | III drop / Boss | Không mua / 350 |
| III — Quần Huyền Ấn | Quần | HP 88 / DEF 9 | 17 / mọi phái | III drop / Boss | Không mua / 275 |
| III — Giày Huyền Ấn | Giày | HP 40 / DEF 6 / EVA 12 / tốc chạy +3% | 17 / mọi phái | III drop / Boss | Không mua / 225 |
| III — Nhẫn Huyền Ấn | Nhẫn | MP 22 / ACC 15 / Crit 2% | 17 / mọi phái | III drop / Boss | Không mua / 225 |
| III — Dây chuyền Huyền Ấn | Dây chuyền | MP 66 / EVA 12 | 17 / mọi phái | III drop / Boss | Không mua / 225 |

18 dòng trang bị không buộc thay cả bộ. Vũ khí Rare II +6 đạt **45,47 ATK** và Tinh Hoa I, so Common III +0 là **40 ATK**; đồ II đã đầu tư có thể đáng giữ để chuyển sang III. Nhẫn/dây chuyền đã nâng cũng có thể hơn món III mới chưa nâng. Dây chuyền III bán 225 Vàng, ngang Giày/Nhẫn III. Khi rơi trang bị, chọn đều giữa các ô hợp lệ (Nấm Lv 2 / Sói Lv 4 chỉ năm ô không vũ khí; nguồn khác đủ sáu); vũ khí chia đều Kiếm/Cung, không tự ưu tiên phái người nhặt.

| Ô trang bị | Chỉ số chịu phẩm chất | Tinh Hoa I tại +4 | Tinh Hoa II tại +8 (chỉ bậc III) |
| --- | --- | --- | --- |
| Vũ khí | ATK / MP; thêm ACC nếu là Cung, không nhân Chí mạng Kiếm | +0,5 điểm % chí mạng | +6 chính xác |
| Áo giáp | HP / DEF | +10 máu | +2 phòng thủ |
| Quần | HP / DEF | +1 phòng thủ | +15 máu |
| Giày | HP / DEF / EVA; không nhân tốc chạy | +4 né tránh | +1 phòng thủ |
| Nhẫn | MP / ACC, không nhân Crit | +4 chính xác | +0,5 điểm % chí mạng |
| Dây chuyền | MP / EVA | +10 linh lực | +4 né tránh |

Rarity Common 1 / Uncommon 1,08 / Rare 1,16 / Epic 1,25 nhân primary list trước enhance; Chí mạng gốc của Kiếm/Nhẫn và tốc chạy Giày không nhân. Weapon / Armor / Pants visual, Boots / Ring / Necklace stat / icon. Full 3 sets chỉ benchmark, không yêu cầu player đổi đồng loạt.

**Một bảng cường hóa chung cho sáu ô.** Bậc I dùng +0..+4, bậc II +0..+6, bậc III +0..+8; không thể thử bước vượt trần của bậc. Xác suất/chi phí dưới đây là BASELINE để kiểm trong game. Thất bại giữ cấp, vẫn tiêu Vàng và Tinh Thạch; không tụt cấp. Tinh Hoa là lợi ích cố định, thấy trước trong tooltip kể cả khi còn khóa; mở tại +4 và +8, không quay ngẫu nhiên. Cùng cấp + dùng cùng hệ số ở mọi bậc; bậc chỉ đổi chỉ số gốc và trần.

| Cấp đạt | Tỉ lệ thành công | Vàng / lần | Tinh Thạch / lần | Hệ số ATK / HP / MP / DEF | Mốc mở |
| --- | ---: | ---: | ---: | ---: | --- |
| +0 | — | — | — | 1,00 | — |
| +1 | 100% | 100 | 1 | 1,05 | — |
| +2 | 90% | 200 | 1 | 1,10 | — |
| +3 | 80% | 350 | 2 | 1,16 | — |
| +4 | 65% | 550 | 3 | 1,23 | Tinh Hoa I, mọi bậc |
| +5 | 45% | 850 | 5 | 1,32 | Bậc II/III |
| +6 | 35% | 1.300 | 7 | 1,40 | Bậc II/III |
| +7 | 25% | 2.000 | 10 | 1,49 | Chỉ bậc III |
| +8 | 15% | 3.200 | 14 | 1,59 | Tinh Hoa II, chỉ bậc III |

Thứ tự tính từng món: ATK/HP/MP/DEF gốc × hệ số phẩm chất × hệ số cường hóa; ACC/EVA gốc (gồm ACC Cung) thuộc cột chỉ số chịu phẩm chất chỉ × hệ số phẩm chất, **không** × hệ số cường hóa; Chí mạng gốc của Kiếm/Nhẫn và tốc chạy Giày giữ nguyên. Sau đó cộng riêng theo cấp: Giày +2 EVA/cấp; Nhẫn +2 ACC và +0,2 điểm % Chí mạng/cấp; Dây chuyền +2 EVA/cấp. Cộng Tinh Hoa I/II vào chỉ số tương ứng sau phép nhân, không nhân chúng lại với phẩm chất/cường hóa.

Cộng các món với nền nhân vật và điểm thuộc tính; cuối cùng mới áp nội tại nền tảng Kiếm/Cung một lần. Tốc chạy cộng tuyến tính: `MoveSpeedMultiplier = 1 + 0,0005 × AGI + Giày%/100`; không nhân lại theo phẩm chất/cường hóa. Giữ số lẻ nội bộ, chỉ làm tròn khi UI/sát thương cần. Tinh Hoa tính từ cấp item, không cộng dồn lại khi mặc hoặc tải save.

Bảng tra đầy đủ chỉ số **Common từ +0 đến trần từng bậc**, gồm cả Mộc Kiếm không thể nâng, nằm tại [Playtest & Balance](../04-production/playtest-and-balance.md#gear-upgrade-values). Công thức và giới hạn trong design owner này là luật nếu cần tính phẩm chất khác Common.

**Ví dụ kiểm chứng:** Áo giáp Rare III +8 có HP = 138 × 1,16 × 1,59 + 10 (Tinh Hoa I) + 0 (Tinh Hoa II không cho HP) = **264,5272**; DEF = 15 × 1,16 × 1,59 + 2 (Tinh Hoa II) = **29,666**. Khi mặc, cộng hai giá trị này vào tổng HP/DEF trước nội tại Kiếm Tâm; Kiếm Tâm mới nhân MaxHP ×1,10 và DEF ×1,08. Preview, stat panel và save load phải dùng đúng cùng phép tính.

## Chuyển giao cường hóa — P0 tại Bách Luyện

Người chơi chọn **đồ nguồn + đồ đích đã sở hữu**, khác instance, cùng ô, đều ở trong túi (tháo món đang mặc trước). Vũ khí phải cùng loại Kiếm/Cung, đúng phái; nhân vật phải đủ cấp mặc đồ đích. Mộc Kiếm, bí kíp, vật phẩm nhiệm vụ và đồ đang ràng buộc hướng dẫn không dùng được. Đích giữ nguyên template, phẩm chất và instanceID; nguồn bị tiêu, không sao chép trang bị hay phẩm chất.

| Kiểu chuyển | Điều kiện bậc | Cấp đích sau chuyển | Chi phí |
| --- | --- | --- | --- |
| Đổi món trong cùng bậc, thường để lên phẩm chất | I→I, II→II hoặc III→III | max(cấp đích, cấp nguồn) | 800 Vàng; không Tinh Thạch |
| Sang bậc kế tiếp | I→II hoặc II→III; không nhảy I→III | max(cấp đích, cấp nguồn) | 500 Vàng + 2 Tinh Thạch |

Nếu cấp đích không tăng, từ chối trước khi tiêu gì; cấp chuyển không được vượt trần bậc đích. Đường nâng dễ nhớ: **I +4 → II +4 → II +6 → III +6 → III +8**. Không tạo vật phẩm chuyển giao. Preview nêu nguồn sẽ mất, Vàng/đá, chỉ số đích trước/sau và trạng thái hai Tinh Hoa. Tiêu nguồn, trả chi phí, cập nhật đúng instance đích và lưu cùng một giao dịch; replay/reconnect trả kết quả cũ, không trừ hai lần.

Tinh Hoa luôn tính từ cấp đích sau commit, không copy giá trị cộng từ nguồn. Không bán nguồn trong cùng giao dịch. Evidence chi phí tại [Playtest & Balance](../04-production/playtest-and-balance.md#economy-analysis).

**Luồng đồ rơi:** Vàng/nguyên liệu/Bình/Tinh Thạch là các lượt roll độc lập; trang bị dùng **một roll loại trừ giữa các phẩm chất**. Một kill có thể cho Vàng + nguyên liệu + đá + đồ, nhưng không roll hai phẩm chất trang bị. Tỷ lệ/số lượng là TEST/TUNABLE; ngữ nghĩa dưới đây là baseline triển khai, LOOT-01 còn kiểm tuning/kinh tế.

| Channel | Normal | Linh Biến | Boss (một shared pile) |
| --- | --- | --- | --- |
| Gold / EXP | 100% budget theo mobL | Base × 3 | **0 direct Gold / 0 EXP** |
| Trade Material | 30%, 1 | 100%, 1–2 | — |
| Potion | 4%, 1; tier theo mobL | — | 50%, III 2–3; HP / MP 50–50 |
| Tinh Thạch | 8%, 1 | 100%, 1 | Guaranteed 5–8 |
| Gear exclusive | Common 4 / Uncommon 1 / Rare 0,1 / None 94,9% | Uncommon 30 / Rare 6 / Epic 0,5 / None 63,5% | Rare 40 / Epic 8 / None 52%, tối đa 1 |
| Hồi Sinh Phù | — | — | 30%, 1 |
| Thỏi Vàng | — | — | Guaranteed 3–5, stack theo [Inventory contract](#inventory-contract), sell 250 / thỏi |

Boss Thỏi chỉ thành currency khi nhặt + bán NPC, không auto-credit / không shop buy. Boss không dùng normal level-penalty. Counts uniform inclusive; channels independent, gear exclusive. Q12 Gold turn-in riêng một lần / character, không thêm pile. Linh gear band theo base mob level ở source table; cùng một mob dùng một Linh reward profile.

<a id="farm-rewards"></a>

**Normal / Linh Biến EXP / Gold — one budget / death:** snapshot Contribution = ActualHpLost / RuntimeMaxHP theo characterId, level trước reward, MapId / position / alive / connected và lastDamageAt. Recipient cần damage>0, sameMap, alive / connected, distance ≤ AssistRadius 8 u và đã gây direct / DoT damage trong 10 s trước death (BASELINE / TUNABLE). Không Party requirement. `Reward = floor(BaseReward * Contribution * LevelRewardFactor)` cho EXP và Gold riêng;

BaseReward đã có variant multiplier. **Không normalize / redistribute** phần người bị loại; sum reward không vượt budget. Không dùng threshold 20% quest cho EXP / Gold. Gold auto-credit / coin burst local; Boss không dùng pipeline này.

| Chênh lệch tuyệt đối | Hệ số thưởng — TEST / TUNABLE | Regular physical set / Journey |
| --- | ---: | --- |
| `abs(PlayerLevel−MobLevel) ≤ 3` | 1,00 | Đủ điều kiện thưởng; roll profile thường, Journey khi đủ participation |
| `abs(PlayerLevel−MobLevel) ≥ 4` | 0 | EXP / Gold / regular loot / Journey kill = 0 |

Áp riêng từng recipient, snapshot trước level-up, đối xứng cả player quá cao lẫn quá thấp; không có tầng nửa thưởng hay gate RNG theo level. Lv 20 EXP luôn0, Gold/loot/Journey vẫn hợp lệ. Quest/supply/evidence xét riêng các mục liên quan, vẫn làm quest muộn. Reward eligibility hiển thị tooltip/reject feedback khi cần, không spam trên mọi nameplate; level/quest/Linh/Boss cues giữ vai trò riêng.

**Physical loot Normal / Linh Biến:** roll một shared set / death, không personal sets. TopDamage là highest ActualHpLost trên entire life ledger, tie characterId; nếu TopDamage factor = 0 thì **không roll regular physical set**, không fallback người thứ hai. TopDamage đủ điều kiện thì roll profile bình thường nguyên quantity; không gate RNG theo level. TopDamage absent / dead không chuyển priority; owner window có thể không ai nhặt, deadline vẫn chạy. Quest tutorial supply tách khỏi regular set.

| Từ deathUtc | Normal / Linh Biến 60 s | Boss 90 s |
| --- | --- | --- |
| Owner window | 0–8 s TopDamage | 0–12 s TopDamage |
| Contributor window | 8–20 s: snapshot contributors ≥ 5% và farm factor>0 | 12–30 s: snapshot Boss contributors ≥ 10% |
| Shared pickup | 20–60 s eligible-FFA, không cần damage nhưng own level factor>0 | 30–90 s FFA, không normal level penalty |
| Expiry | 60 s despawn | 90 s despawn |

Mọi pickup verify sameMap / alive / distance ≤ 1,5 u, instance / capacity và quyền theo window; normal contributor window còn cần snapshot alive/connected/radius/participation như EXP/Gold. **Level eligibility khi pickup:** người có trong death ledger dùng level đã chụp trước reward cho cả ba windows; người đến sau không có ledger dùng level hiện tại. Lên cấp từ chính kill không làm mất quyền nhặt món đã roll; các kiểm tra alive/MapId/distance/capacity tại pickup vẫn bắt buộc.

Eligible-FFA cố ý cho người đến sau nhặt đồ level-hợp lệ, không cho họ EXP / Gold / quest credit. Timers không reset khi owner chết / reconnect / rời map. Một item chỉ một claim; bag đầy không consume item. Boss pile / thỏi theo channel table trên.

**Source table — canonical mob/content → gear/potion/material.** Gear success +0; Linh kế thừa band/slot pool/material của base identity, nhưng dùng Linh rarity profile. Potion column là tier có thể rơi ở channel Potion; Food là tier chuẩn bị phù hợp, chỉ mua chứ không drop. Use/equip gates vẫn theo item (Potion/Food I1, II10, III15), không theo map mở sớm. Material chỉ bán, không crafting; mỗi source/map chọn đúng một material ID, không roll nhiều loại.

| Mob / content | Gear band / slot pool | Potion / Food tier | Material theo map (sell Vàng) | Rarity profile |
| --- | --- | --- | --- | --- |
| Nấm Linh Lv 2 | I / 5 non-weapon | I | Đồng: Nấm Sương (2) | Normal / Linh theo variant; Nấm không Linh |
| Sói Sương Lv 4 | I / 5 non-weapon | I | Đồng: Nanh Sói (3); Trúc: Trúc Tâm (4) | Normal; không Linh |
| Sói Trúc Ảnh Lv 8 | I / 6 slots | I | Trúc: Trúc Tâm (4) | Normal hoặc Linh |
| Ong Giáp Lv 10 | II / 6 slots | II | Trúc: Cánh Ong (4); Bạch: Vân Thạch (6) | Normal hoặc Linh |
| Đoạt Mạch Lv 13 | II / 6 slots | II | Bạch: Huy Hiệu Đoạt Mạch (6); Xích: Khoáng Xích Nham (9) | Normal hoặc Linh |
| Xích Thạch Linh Lv 16 | II / 6 slots | III | Xích: Khoáng Xích Nham (9); Huyền: Mảnh Cổ Ấn (12) | Normal hoặc Linh |
| Cổ Môn Vệ Binh Lv 20 | III / 6 slots | III | Huyền: Mảnh Cổ Ấn (12) | Normal hoặc Linh |
| World Boss Lv 20 | III / 6 slots | III | Thỏi Vàng theo channel table | Boss |
| Q3 / Q4 / Q6 / Q7 / Q8 / Q10 / Q11 | Quest exceptions trong catalog | Tutorial supplies theo quest | Collection quest-bound riêng theo Quest owner, không nhập chung farm material | Guaranteed, không loot roll |

Quest evidence không dùng Trade Material đã farm; tooltip material “Vật liệu giao dịch — có thể bán”. Tinh Thạch không có tier riêng. Source Nấm / Sói Lv 4 loại Weapon ở **mọi map**, không dùng MapId để thay slot pool.

**Vendor economy — BASELINE / TUNABLE:** ItemDefinition buyPrice optional / sellValue explicit. Rarity sell Common 1 / Uncommon 1,5 / Rare 2 / Epic 3 trên Common sell, floor một lần; enhance không tăng sell / hoàn resource. Các supply khác sell = floor(buy × 0,25); drop-only không fake buyPrice. Shop stock vô hạn.


**Boss contributor / quest / Journey:** ≥ 10% RuntimeBossMaxHP (release 3.200; demo 12.800 → 1.280), connected / sameMap / insideBossCombatArea tại death; corpse trong area hợp lệ, về làng / disconnect thì không. Q12 dùng active-step damage, không hồi tố; tối đa 10 recipients có thể đạt threshold trên full-health life. Contributor không tạo personal pile.

Quest credit/predicates theo [Quest owner](quests-and-narrative.md#quests-story), độc lập EXP / Gold>0. Normal contributor pickup 5% theo cùng eligibility. Journey normal 5 / Linh 100 cho regular recipient ≥ 20%, nhân own level factor / floor; Boss 500 / deathID cho qualifying 10%; quest 150 / chapter 300 một lần, PvPwin 200 / matchID. Không claim points bằng pickup.

Inventory ban đầu **60 slots — LOCKED**, Storage giữ 40. Không expansion/VIP/mua ô/nâng túi P0. [Inventory contract](#inventory-contract) sở hữu stack và item classification. Pickup phải fit toàn amount sau merge; thất bại giữ nguyên ground/entitlement. Bag 60/60 vẫn nhận được item vào compatible stack còn trong technical bound. Ground thường/Linh 60 s, Boss 90 s giữ nguyên; personal quest representation có TTL hữu hạn riêng còn TUNABLE. Unequip cần capacity, không bán equipped.

Turn-in mô phỏng consume đúng collection items rồi merge reward, tính X ô còn thiếu trên net inventory. Thiếu thì báo “Cần X ô trống trong hành trang”, giữ READY_TO_TURN_IN; không consume vật phẩm / trao một phần reward / set Completed. Vàng / EXP / story / Journey không cần slot; retry không nhận lặp.

Yên Thảo bán Food/HP/MP Potion; Bách Luyện bán Common I/II, Tinh Thạch **800 Vàng**, General Sell/Enhance/Transfer và Q4 sample; Mộc An sở hữu Utility Shop Hồi Sinh Phù/Tẩy Mạch Phù cùng Storage40/Rest. Giá/effect hiện hành giữ nguyên. Q9 không gear-exclusive, skip không mất nâng slot. Một tiền tệ Vàng. Bag Sort / protection gear P1; validation inventory P0.

> **Đọc sâu:** [Playtest & Balance — kinh tế và enhance](../04-production/playtest-and-balance.md#economy-analysis)

---


<a id="consumables-death"></a>
<a id="gdd-8"></a>

<a id="7-vật-phẩm-tiêu-hao-và-tử-vong"></a>

## Vật phẩm tiêu hao và tử vong

Không tự hồi khi thiếu Food. Food là nguồn hồi chính: tick mỗi 2 s theo MaxHP/MaxMP, không vượt đầy, hiệu lực 10 phút; món mới thay món cũ, trúng đòn không tạm dừng hồi. Bình dùng cứu nguy. Nghỉ tại Mộc An ở Vân Khê hồi đầy HP/MP.

| Item | Lv | Hiệu quả | Giá Vàng |
| --- | ---: | --- | ---: |
| Bánh Lúa Mạch | 1 | Mỗi 2 s +2% HP, +1,5% MP | 150 |
| Thịt Nướng Thảo Mộc | 10 | Mỗi 2 s +3% HP, +2% MP | 400 |
| Cơm Hầm Linh Thảo | 15 | Mỗi 2 s +4% HP, +2,5% MP **BASELINE / TUNABLE** | 700 |
| Bình Sinh Lực I / II / III | 1 / 10 / 15 | Instant 30 / 45 / 60% MaxHP | 80 / 220 / 480 |
| Bình Linh Lực I / II / III | 1 / 10 / 15 | Instant 30 / 45 / 60% MaxMP | 80 / 220 / 480 |
| Hồi Sinh Phù | 1 | Tại chỗ 50% HP / MP + 2 s invulnerable | 1.000 |
| Bùa Hồi Thành | P1 | Cast 3 s về làng; damage hủy cast | 150 |


**PvP 1v1 có cược Vàng — USER LOCK:** Hạo Vũ → chọn đối thủ → người mời chọn **1.000–10.000 Vàng**, chỉ theo bước **1.000 Vàng** → người được mời thấy và chấp nhận **đúng mức cược đó** → backend kiểm cả hai đủ tiền và tạm giữ cược của **cả hai cùng một giao dịch** → Game Server mới tạo MatchId/đếm ngược → Active → kết quả → Vân Khê. Chênh cấp ≤5, một hiệp. Không có trận nếu escrow thất bại; không trừ tiền một phía. Lời mời hết hạn 20 s. UI hiển thị cược mỗi người, tổng pot, luật phí và trạng thái tạm giữ trước khi xác nhận.

**Kết quả và tiền:** Hạ đối thủ HP = 0 hoặc đối thủ disconnect **sau khi Active** thì thắng; người disconnect trước bị FORFEIT. Nếu cả hai mất kết nối trong cùng một tick mà không xác định được ai trước, hoặc Game Server/backend/system lỗi làm trận bị abort, xử **SYSTEM_ABORT**, hoàn **100%** cho cả hai, không phí và không xử thua. Hết 120 s khi cả hai còn sống là **Hòa**, không so HP hay %HP.

Disconnect trước Active/khi còn đếm ngược, hết lời mời hoặc không thể bắt đầu thì hủy và hoàn **100% cược cho mỗi người**. Trận có người thắng (kể cả FORFEIT): hệ thống giữ **10% tổng pot**, người thắng nhận **90% tổng pot**. Hòa: mỗi người nhận lại **90% cược của chính mình**, mỗi người mất 10%. Chỉ Spring Boot quyết định số Vàng escrow/settlement và commit một lần theo MatchId; Game Server chỉ quyết định kết quả realtime.

Với cược 1.000 mỗi người: pot 2.000; thắng nhận 1.800, phí 200; hòa mỗi người nhận 900; SYSTEM_ABORT mỗi người nhận 1.000.

<a id="quick-items"></a>

QuickHP và QuickMP chọn bình **bậc thấp nhất hiện có, đủ cấp dùng và đủ hồi phần HP/MP đang thiếu**; nếu không bình nào đủ bù, dùng bậc cao nhất hợp lệ. Game Server kiểm túi, cấp, số lượng và hồi chiêu; đầy HP/MP hoặc đã chết thì từ chối, không tiêu bình. Q6 dùng Bình Linh Lực I đã phát trước bình khác để không kẹt hướng dẫn. Food dùng bậc cao nhất hợp lệ; Food mới thay hiệu ứng cũ và đặt lại thời hạn 10 phút, không cộng dồn. QuickHP/QuickMP chỉ dùng bình, không chọn skill.

Q6 hiển thị glyph QuickMP theo binding probe. Interact tác động ngay ứng viên NPC/loot riêng, không thay CombatFocus. [Approach không kích hoạt EdgeExit](combat-and-character.md#pending-cast).

## Food state và Potion feedback

Food icon cho biết active/absent, remaining duration, cảnh báo gần hết và thông báo expiry. Thiếu MP nêu reason rõ và gợi chuẩn bị Food/bình phù hợp; không tự mua hay dùng Food. Warning threshold còn TUNABLE qua usability review. Hồi phục Potion đã accepted do Game Server phát ngay; reject đầy HP/MP/dead/quota/CD không tiêu item. Persistence chi tiết thuộc [Online & Persistence](../02-technical/online-and-persistence.md#potion-durability).

## Ngoại lệ kinh tế reservation Q8

[World Q8 policy](world-and-content.md#q8-bounded-path) khóa một shared economic budget/receipt, không roll Linh mới mỗi credit retry. Regular random Linh vẫn dùng profile trong bảng trên. Forced Q8 retry không sinh thêm Rare/EXP/Gold/Journey budget, không nhân loot theo số requester. Entitlement/recovery cụ thể phải qua Q8-01/G-D; mọi thay đổi profile ngoài ngoại lệ này cần review design.

<a id="a03"></a>

## A03 — quyết định liên quan

**A03 Boots** — Giữ stat-only: không cost world; bỏ: phương án lịch sử đã SUPERSEDED, đổi economy/balance; footwear slot có visual thật: thêm layer · Giữ stat-only P0, LowerBody chứa footwear mỹ thuật · Kiểm đủ sáu ô, icon và evaluator HP/MP; không mở lại phương án bỏ Boots

**A03** — GEAR-01 / BAL-01 / LOOT-01 · BASELINE giữ sáu slots/Boots stat-only theo design owner. Đề xuất bỏ Boots là LEGACY/SUPERSEDED bởi yêu cầu giữ sáu ô. Thêm world visual riêng không thuộc P0; giữ stat-only, không mở lại số slot.



Mọi loại Bình Sinh Lực dùng chung **hồi chiêu HP 8 s**; mọi loại Bình Linh Lực dùng chung **hồi chiêu MP 8 s**. Hai nhóm **tách nhau**: dùng HP không khóa MP và ngược lại. Không có hồi chiêu riêng từng bậc bình.

Tẩy Mạch Phù: 1.200 Vàng tại Mộc An, stock vô hạn. Gameplay reset giữ class/level/gear/quest/learned skills theo Combat owner.

Food không hồi HP/MP khi actor đã chết. Mất phiên xóa Food runtime; resume phiên còn sống giữ state theo Online owner.

<a id="inventory-contract"></a>

## Inventory: capacity, stack và item classification

**LOCKED:** 60 ô ban đầu, kể cả vật phẩm thu thập nhiệm vụ; không reserve quest slots. **STRONG DIRECTION:** cùng identity và stack-compatible dùng chung ô, không low gameplay cap 20/99/999. Không gọi stack vô hạn. Storage vẫn 40 ô, dùng cùng stack rules; không thay capacity Storage hoặc mở Trade P0.

**Technical bound — BASELINE kỹ thuật đề nghị, cần parity test trước wire/DB schema:** quantity của một stack là integer `1..2_147_483_647` (int32 signed dương); request quantity cũng trong miền này, không float/âm/0. Tính tổng/merge và `unitPrice × quantity` bằng checked int64; reject overflow/currency out-of-bound trước mutation. Chọn wire/DB type cụ thể ở G-D, không tạo SQL schema bằng quyết định này. Gold bound riêng phải được kiểm ở G-B/G-D, không suy Gold là int32 từ prototype. Tại giới hạn kỹ thuật, phần dư có thể sang stack tương thích mới nếu đủ ô; không fit toàn amount thì reject toàn lệnh. Không silently clamp, wrap hoặc mất phần dư. UI formatter rút gọn có tooltip số chính xác; không áp cap vì text bị dài.

Compatibility key gồm template identity, character/QuestId/objective binding và mọi state có nghĩa gameplay (rarity/options/enhancement nếu applicable). Khác binding không merge. Unique equipment luôn là instance riêng, quantity 1; không thêm durability nếu chưa có. Stack identity còn sống ổn định; khi merge, receipt ánh xạ entitlement/source tới stack đích + delta quantity, không hứa giữ tất cả incoming instance IDs như nhiều physical instances cùng ô.

| Classification | Chiếm ô? | Stack | Sell | Trade | Manual Drop | Death loss | Persistence | Server owner |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Stackable consumable — Food/Potion/phù | Có | Compatible, technical bound | Theo sellValue/policy | P0 không có Trade | Không thêm manual-drop feature P0 | Theo recovery hiện hành; không thêm loss | Quantity/location/binding | Inventory mutation; Use handler riêng |
| Stackable material/Tinh Thạch/Thỏi | Có | Compatible, technical bound | Catalog hiện hành | P0 không có Trade | Chưa có handler production; không tự mở | Không thêm loss | Stack + quantity | Inventory; Sell/Storage riêng |
| Quest-bound collection stack | Có, ô thường | Cùng quest/objective/identity mới merge | Cấm, reason nhiệm vụ | Cấm kể cả khi Trade được thêm | Cấm | Không mất | Item/binding + entitlement claim receipt | Inventory + Quest validation |
| Normal item referenced by quest — Nấm Sương Q4 | Có | Compatible theo binding của sample; không merge nhầm bản thường trước sell | **Có** ở đúng Bách Luyện/step; gỡ temporary restriction sau action theo quest | P0 không có Trade | Tutorial restriction tới bước hợp lệ; không feature drop mới | Không mất quyền tutorial | Item và receipt `ItemSold` | Sell handler; quest chỉ observe commit |
| Unique equipment instance | Có khi bag; equipped nằm ngoài bag | Không; enhancement/rarity/options riêng | Chỉ unequipped và policy cho phép; Mộc/bound cấm | P0 không có Trade | Bound cấm; không tự thêm normal-drop | Không đổi item-loss rule | Instance/location/slot/state | Equipment + Inventory |
| Bound manual/power item | Có tới khi học | Receipt grant tối đa một/quyền học; giữ quantity 1 mỗi bound learning instance, không phải cap consumable 99 | Cấm | Cấm | Cấm | Không mất | Instance/grant/learn receipt | Learn handler consume + learned SkillId atomic |
| Non-world UI-only feedback | Không | Không inventory quantity | Không | Không | Không | Không applicable | UI selection không bền; objective action receipts nếu cần | UI chỉ đọc; **không dùng nhóm này thay collection item** |
| Currency — Vàng | Không | Numeric balance bound riêng | Không item để bán | Không thêm Trade | Không ground coin item | Theo contract hiện hành | Balance + transaction receipt | Shop/reward mutation; Spring commit |

Personal quest ground drop là **delivery/ownership**, không một ItemKind hay một boolean `questItem` dùng cho tất cả. Tách item policy/binding khỏi entitlement/representation; Q4 sample được bán không mâu thuẫn với collection quest-bound cấm bán. **BASELINE implementation đề nghị:** quest-bound collection nằm trong bag, không gửi Storage P0 để tránh ambiguity về "đang mang"; supply gear/storage theo policy riêng. Pending entitlement không chiếm ô cho tới claim, nhưng cũng không được tính là đã có item. Normal quest-referenced nghĩa item có khả năng giao dịch theo domain; không khôi phục hệ Trade đã DROP P0.

<a id="service-terminology"></a>

## Enhancement, Chuyển giao và Tẩy Mạch

Enhancement tăng cấp theo cost/RNG/trần hiện hành. **Chuyển giao (Enhancement Transfer)** tiêu source, giữ target instance/template/phẩm chất và chuyển cấp cùng bậc/đúng một bậc tiếp theo theo contract đã chốt; Bách Luyện sở hữu. **Tẩy Mạch** trả điểm thuộc tính, không đổi phái/ClassChosenLevel; Mộc An giữ ownership và giá 1200 theo chỉ đạo recovery, cùng Hồi Sinh Phù1000/Storage40/Rest; Yên Thảo giữ Food/HP/MP. Utility P0 chỉ hai phù đã có; nhóm bùa tương lai chỉ thêm nếu có requirement/catalog được duyệt, không seasonal/Crafting hoặc mở rộng túi. **Hoán Chuyển — PROPOSAL bị khuyến nghị bỏ như concept mới:** chưa thấy input/output/use-case khác Transfer; dùng tên Chuyển giao hiện hành, không thêm handler/item/cost mới.


<a id="vendor-catalogs"></a>

## Vendor catalogs và Sell policy — ownership đã duyệt, engineering policy BASELINE

Một Shop UI Buy/Sell đọc catalog và capabilities của NPC, không mỗi NPC một layout. Root menu chọn Shop/Storage/Rest/Enhance/Transfer theo [NPC owner](quests-and-narrative.md#npc-service-review); các dịch vụ đó không thành Shop tabs. ItemDefinition giữ giá/effect/explicit sellValue hiện hành; catalog là refs, không copy stats.

| NPC | Buy catalog P0 | Sell capability BASELINE | Other services |
| --- | --- | --- | --- |
| Yên Thảo | Ba Food + HP I/II/III + MP I/II/III; không Hồi Sinh/Tẩy Mạch | Chính các consumables Food/HP/MP sellable theo hiện hành; bag khác vẫn hiện nhưng disabled reason “Bán tại Bách Luyện” | Q5/Talk và tư vấn hồi phục thông thường; không thêm paid heal mới |
| Bách Luyện | Common I/II theo class/equip gates + Tinh Thạch800; III/Mộc/manual không shop buy | **General Sell** mọi unequipped bag item sellable theo policy, cả materials/drop-only/consumables; quest-bound cấm. Q4 exact sample/step riêng | Enhance/Transfer/Q3/Q4/Q7; không Hoán Chuyển duplicate |
| Mộc An | **Hồi Sinh Phù1000 + Tẩy Mạch Phù1200**; stock vô hạn/effect/cooldown giữ hiện hành | Chỉ hai utility sellable theo explicit sellValue/current quarter-price rule; mọi item khác disabled reason ở Sell | Storage40/Rest full; dùng Tẩy Mạch vẫn inventory Use handler theo Combat, không hai reset services |
| Lâm Bá/hai mentors/Hạo Vũ | Không item vendor mới; manuals chỉ quest grants | Không General Sell | Theo NPC owner |

Sell subsets ở Yên/Mộc là engineering recommendation để giữ nghề và reuse Buy/Sell tabs; không nerf giá hay đổi item policy. Bách vẫn bán được toàn bộ đồ hợp lệ, không buộc người chơi quay từng vendor. Catalog revisions/vendor capabilities server-owned; wrong vendor/template/step/quantity fail trước mutation. Hồi Sinh/Tẩy Mạch chỉ có một canonical Buy owner Mộc; Boss vẫn có thể drop Hồi Sinh như channel cũ. Chuyển catalog không regrant/migrate giá hoặc tạo utility item mới.
