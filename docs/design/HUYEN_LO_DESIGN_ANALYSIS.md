# Huyền Lộ — Phân tích thiết kế

**Đối chiếu:** GDD V5.1.2 · **Ngày:** 2026-09-29 · **Vai trò:** evidence, simulation, review và quyết định mở.

[GDD](HUYEN_LO_GDD.md) là nguồn design authority; [Technical](HUYEN_LO_TECHNICAL.md) là implementation contract. FACT ở đây là dữ kiện tài liệu hoặc phép tính kiểm lại; SIMULATION là kết quả phụ thuộc giả định; PROPOSAL không tự thành luật game. Các nhận xét source NSO dưới đây là ghi nhận từ audit đã đọc, không tuyên bố đã chạy lại game/reference trong vòng này.

<a id="balance-baselines"></a>

# 1. Balance Baselines

## S01 — EXP và onboarding

**Question:** cumulative và 791 kills có thực sự suy ra từ bảng? **Evidence:** cộng 19 giá trị GDD cho 53.100; Lv20 bắt đầu tại 53.100, không có requirement lên Lv21.

Bảng Need/cumulative do [GDD §3](HUYEN_LO_GDD.md#gdd-3) sở hữu. Kiểm toàn bộ20 mốc: tổng19 requirement = **53.100**; Lv5=790, Lv12=9.100, Lv17=29.600, Lv20=53.100.

**Calculation:** `round = floor(x+0,5)` cho số dương; kill mỗi level = ceil(Need/MobBaseEXP). Tổng bỏ carry là **799**, không mặc định 791. Với EXP dư được carry qua level, kết quả còn khác; con số lịch sử 791 thiếu mô hình tái lập nên bỏ khỏi GDD. 600–720 normal-equivalent kills và 2,5–4h vẫn là target phải đo bằng hành trình thật.

Catch-up: Lv4 Need 320, đã có 200 → Remaining 120; reward bù 120 đưa lên Lv5, không 320. Khi đã đạt mốc, chỉ BaseEXP. Q1–Q4 tổng tối đa 790 EXP nếu chưa có nguồn khác; không cộng thêm 790 lần nữa khi dùng mô hình normal farming đã đi qua Lv1–4. EXP quest cũ được giữ như input **legacy**, không dùng tổng cũ làm FACT cho journey mới. Reward Q5–Q11 retune ở S11/QUEST-03; GDD vẫn ghi rõ số tạm chưa khóa.


## S02 — Enhancement expected cost

**Question:** fail giữ cấp thì expected cost bao nhiêu? **Calculation:** mỗi bước expected attempts=1/p; Vàng và đá = cost/p, cộng năm bước. Không cần Monte Carlo để kiểm kỳ vọng.

| Bước | p | Attempts | Vàng kỳ vọng | Đá kỳ vọng |
| --- | --- | --- | --- | --- |
| +0→+1 | 100% | 1,00 | 100,00 | 1,00 |
| +1→+2 | 90% | 1,11 | 222,22 | 1,11 |
| +2→+3 | 80% | 1,25 | 437,50 | 2,50 |
| +3→+4 | 65% | 1,54 | 846,15 | 4,62 |
| +4→+5 | 45% | 2,22 | 1.888,89 | 11,11 |
| Tổng | — | — | 3.494,76 | 20,34 |

**Baseline:** ~3.494,76 Vàng và 20,34 đá/món +5. Sáu món ~20.968,59 Vàng và 122,03 đá chưa gồm mua gear/Food/Potion. Đây là trung bình, không bảo đảm RNG. Từ +0→+4:1.605,88 Vàng / 9,23 đá. Nếu Transfer P1 giá 500/2 thay nâng mới tới+4, tiết kiệm ròng **1.105,88 Vàng/7,23 đá**, không phải toàn bộ 1.605,88/9,23. Transfer consume món cũ và mất một cấp, không phải chuyển miễn phí.

<a id="combat-analysis"></a>

# 2. Combat Analysis

## S03 — Sword/Bow DPS và bảy extreme builds

**Question:** parity và build value khi dùng đúng V5 skill? **Model:** Lv20, sáu Common Huyền Tích +0, không hidden option/buff P1. Gear: ATK 44, HP 240, DEF 28, MP 60, ACC 16, EVA 32, Crit 2,5%. Chỉ class/passive V5; random trung bình 1, chưa round mỗi hit. Mob Lv20: HP 800/DEF 18/ATK 70/ACC 140/EVA 60; incoming lấy Cổ Vệ interval1,6s, không Crit mob vì source chưa định nghĩa.

`ExpectedHit = ATK×100/(100+DEFtarget)×(1−EvadeTarget)×(1+0,5×CritChance)`.

`NormalDPS = ExpectedHit×NormalPower/Interval`.

`SkillDPS = ExpectedHit×(1+INT×0,0025)×Σ(primaryPower/CD)`.

`RotationUpper = NormalDPS+SkillDPS`: upper bound **không trừ cast lock**, không claim là DPS playable. TTK=800/RotationUpper; không mô phỏng burst, travel, target acquisition, MP exhaustion.

| Build | Class | VIT/INT/STR/AGI | HP | MP | ATK | DEF | Né mob | Normal DPS | Rotation upper | TTK s |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| STR95 | Kiếm Sĩ | 0/0/95/0 | 605,0 | 196,0 | 152,6 | 48,8 | 10,8% | 175,8 | 368,8 | 2,2 |
| VIT95 | Kiếm Sĩ | 95/0/0/0 | 1.441,0 | 196,0 | 82,7 | 59,3 | 10,8% | 95,3 | 200,0 | 4,0 |
| INT 95 | Kiếm Sĩ | 0/95/0/0 | 605,0 | 671,0 | 82,7 | 48,8 | 10,8% | 95,3 | 224,9 | 3,6 |
| AGI 95 | Kiếm Sĩ | 0/0/0/95 | 605,0 | 196,0 | 82,7 | 48,8 | 30,1% | 100,0 | 209,7 | 3,8 |
| Cân bằng | Kiếm Sĩ | 24/24/24/23 | 816,2 | 316,0 | 100,4 | 51,5 | 19,0% | 118,9 | 257,3 | 3,1 |
| Kiếm mẫu | Kiếm Sĩ | 30/0/50/15 | 869,0 | 196,0 | 119,5 | 52,1 | 16,6% | 140,6 | 295,1 | 2,7 |
| Cung mẫu | Xạ Thủ | 15/15/45/20 | 670,0 | 298,1 | 115,8 | 45,9 | 18,1% | 118,7 | 246,3 | 3,2 |

**Conclusion:** không tái dùng bảng cũ có class Lv1, 95 INT/719 MP hay AGI hard cap 40%. Với setup trên, AGI 95 vẫn được thêm ACC/EVA nhưng không bảo đảm all-in tối ưu; +4,75% speed chỉ phần thưởng phụ. INT 95 tăng skill 23,75%, không tăng normal; raw STR có thể vẫn thắng damage. Không kết luận INT đã viable chỉ từ một cột skill bonus.

Cùng ATK/ACC/gear, ratio Bow/Sword normal = `(0,95/0,80)/(1/0,72) × (1+0,5CritBow)/(1+0,5CritSword)`. Crit 12,5%/15,5% cho Bow/Sword≈0,867, Bow thấp hơn≈13,3% (hoặc Sword cao hơn≈15,3% nếu lấy Bow làm mẫu số). Không trộn hai cách ghi %.

## S04 — MP sustain và INT sensitivity

**Question:** Food bù spam skill tới mức nào? **Model:** cùng Common Huyền Tích +0 như S03; INT 0/20/40/60/95, Food III, full skill on-cooldown P0. Sword MP/s = 8/1,4+14/4+22/7; Bow = 10/2+16/4,5+20/7. Không tính buff Lv10. Average FoodMP/s = MaxMP×0,025; drift=spend−regen; OOM=MaxMP/drift nếu drift>0. Đây là xấp xỉ liên tục; tick 2s/cast discrete có thể làm thiếu MP trước kết quả này.

| Class | INT | MaxMP | Food MP/s | Skill MP/s | Thiếu MP/s | OOM s | Potion dependency |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Kiếm | 0 | 196,0 | 4,90 | 12,36 | 7,46 | 26,3 | Có nếu spam dài |
| Cung | 0 | 215,6 | 5,39 | 11,41 | 6,02 | 35,8 | Có nếu spam dài |
| Kiếm | 20 | 296,0 | 7,40 | 12,36 | 4,96 | 59,7 | Có nếu spam dài |
| Cung | 20 | 325,6 | 8,14 | 11,41 | 3,27 | 99,5 | Có nếu spam dài |
| Kiếm | 40 | 396,0 | 9,90 | 12,36 | 2,46 | 161,2 | Có nếu spam dài |
| Cung | 40 | 435,6 | 10,89 | 11,41 | 0,52 | 833,4 | Có nếu spam dài |
| Kiếm | 60 | 496,0 | 12,40 | 12,36 | -0,04 | Không cạn trung bình | Không bắt buộc theo model |
| Cung | 60 | 545,6 | 13,64 | 11,41 | -2,23 | Không cạn trung bình | Không bắt buộc theo model |
| Kiếm | 95 | 671,0 | 16,78 | 12,36 | -4,42 | Không cạn trung bình | Không bắt buộc theo model |
| Cung | 95 | 738,1 | 18,45 | 11,41 | -7,04 | Không cạn trung bình | Không bắt buộc theo model |

| INT | Gain skill 0,25% | Gain skill 0,30% | Gain skill 0,35% |
| --- | --- | --- | --- |
| 0 | 0,00% | 0,00% | 0,00% |
| 20 | 5,00% | 6,00% | 7,00% |
| 40 | 10,00% | 12,00% | 14,00% |
| 60 | 15,00% | 18,00% | 21,00% |
| 95 | 23,75% | 28,50% | 33,25% |

Đổi coefficient không đổi MaxMP/Food/spend/OOM, chỉ đổi damage gain. Threshold sustain trung bình Sword≈59,7 INT; Bow≈43,8 INT trong setup này. Không Food: OOM=MaxMP/spend, tại INT 0 Sword≈15,9s và Bow≈18,9s. Potion III phục hồi 60%MP; tại thiếu hụt dương, khoảng cách uống lý thuyết `0,6MaxMP/drift`, nhưng còn CD 8s và discrete burst. Không đổi master coefficient hoặc Food chỉ từ phép tính; BAL-01/BAL-02 cần xác nhận ý đồ và test actual cast lock.

## S05 — Food HP sustain

**Question:** Food có thực sự bù 100% một mob? **Model:** S03, Cổ Vệ 70 ATK/1,6s, ExpectedIncoming=(70/1,6)×100/(100+DEF)×(1−PlayerEvade). Food III HPS=0,02MaxHP, Potion III upper HPS=0,6MaxHP/8; không over-heal/latency/chi phí Potion.

| Build | Incoming/mob DPS | Food HPS | Mob equiv Food | Food+Potion upper |
| --- | --- | --- | --- | --- |
| VIT95 | 24,50 | 28,82 | 1,18 | 5,59 |
| Kiếm mẫu | 23,98 | 17,38 | 0,72 | 3,44 |
| Cung mẫu | 24,55 | 13,40 | 0,55 | 2,59 |

Q3 mới trao Mộc Kiếm nên không benchmark combat Lv1 với vũ khí sẵn có. Ví dụ **Tân Lữ Lv3 sau equip**, auto4VIT/2INT/4STR: HP 172/MP 78/ATK 25,2/DEF 6,6; Food I 1,72 HP/s và 1,17 MP/s. NấmLv3 ATK 12/1,8s, mobACC72/playerEVA24; incoming≈5,81DPS → Food≈0,30mob-equivalent. Bảng source có “Kiếm/Cung Lv1” cùng endgame gear không hợp progression, đã bỏ. Kiting/clear nhanh mới giảm incoming thực tế; Food không được quảng bá đủ mọi build.

## S06 — Multi-target và Freeze

**Calculation:** Phong Trảm ba target tổng 3,75×; Liên Kích ba target tổng 4,50×; Kiếm Khí năm target tổng 7,50×; Xuyên Tiễn bốn target tổng 5,20×; Hàn Tiễn 1+4 tổng 4,60×, primary không nhận 0,80× lần hai; Liên Tiễn tổng 1,35× phân phối theo số target. Không nhân thêm falloff proposal lên power V5 đã liệt kê.

Freeze 25%/cast CD 7s có uptime đơn giản Normal ≈0,25×1,2/7=4,29%, Elite ≈2,14% nếu một target luôn trúng. Uptime thực thấp hơn khi né và cao hơn khi nhiều nguồn cast; không chứng minh CC balance từ kỳ vọng. Boss/PvP immunity giữ nguyên. Check stacking/refresh tại CC-01; không thêm stun 0,1s vào mọi hit vì biến feedback thành gameplay.

## S07 — Boss TTK sensitivity

**Question:** target 90–150s có còn đúng? **Model:** build Kiếm/Cung mẫu S03, **ASSUMPTION FOR SIMULATION: Boss DEF 25/EVA 60**, cả DEF/ACC/EVA đều TUNABLE tại BOSS-02, không phải GDD lock; ACC không cần cho mô hình outgoing TTK này. Rotation upper chuyển DEF 18→25 rồi nhân uptime; không Enrage/Linh Giáp/Burn/buff. Gear+0 cố ý minh bạch; không gọi là endgame+4 benchmark.

| Combat uptime | Effective DPS upper | Release 32.000 TTK | Demo 12.800 TTK |
| --- | --- | --- | --- |
| 100% | 511,1 | 62,6s | 25,0s |
| 75% | 383,3 | 83,5s | 33,4s |
| 50% | 255,5 | 125,2s | 50,1s |

**Conclusion:** đây là sensitivity, không kết quả Boss playable. Uptime đã gom cast-lock/né chiêu/di chuyển; phải đo riêng trước khi chỉnh HP. Benchmark 114s cũ trộn skill/gear không tái lập, và 385,2×0,75=288,9 chứ không 280. Boss DEF/ACC/EVA và count/overlap/scheduling vùng đá rơi thiếu specification, BOSS-02 cần chốt trước milestone 6. Demo HP multiplier phải áp cả threshold contribution; không chỉ sửa thanh HP.

<a id="economy-analysis"></a>

# 3. Economy Analysis

## S08 — Food affordability và income/sink

**Calculation:** AverageGold=4,5+2,5L. Bỏ gear sale/quest/Elite, số mob bù một Food = Price/AverageGold; không coi drop money đã nhặt khi bag/player chưa pickup.

| Lv | Gold/mob | Food giá | Mob/Food | Food cost/phút |
| --- | --- | --- | --- | --- |
| 1 | 7,0 | 150 | 21,4 | 15 |
| 10 | 29,5 | 400 | 13,6 | 40 |
| 20 | 54,5 | 700 | 12,8 | 70 |

Nguồn: normal/Elite/Boss, quest đã chốt, sell 25%BuyPrice. Sink: Food/Potion, nâng gear, gear shop, Tẩy Mạch 1.200, Hồi Sinh 1.000, đá shop 800. Tại Lv20: reset≈22 mob; revive≈18,35 mob; đá≈14,68 mob. Q4 ứng 320 đủ Food 150+hai Potion 80=310, dư 10; không thưởng lại 320 khi reconnect/accept replay.

Mô hình tài sản 25.983/20.003/12.185 Vàng của audit thiếu lịch mua, gear price, drop-sale realization và dùng EXP quest sai; không giữ như FACT. Gold milestone hiện chưa đủ data để tái lập. Before balance pass phải ghi kill count, travel/death time, tỷ lệ Food uptime, Potion counts, enhance attempts và transaction log theo ba phong cách; không tự lock quest ứng trước mới.

## S09 — Stone flow và variance

Normal 8%: expected 12,5 kills/stone. Giả định 672 kills: E = 53,76 đá, σ=sqrt(672×0,08×0,92)=7,03; khoảng E±2σ≈39,7–67,8 là xấp xỉ, không bảo đảm. “35–50 đá miễn phí” cũ không phải kỳ vọng của model này. Normal-only để trả expected 20,34 đá cho một món +5 cần≈254 kills; sáu món≈1.526 kills, chưa tính quest/Elite/Boss.

Ở Lv20, vàng kỳ vọng theo 12,5 kills=681,25; shop 800 đắt hơn≈17,43% theo gross gold. Không kết luận farm luôn rẻ hơn khi thêm thời gian/Potion/death. Linh Biến nếu làm sẽ tăng flow và cần chạy lại toàn bộ sink; không dùng 3% hay drop 50–100% như số đã khóa.

## S10 — Sáu slot đều tăng giá trị +0→+5

**Decision boundary:** giữ enhance multiplier V5 cho ATK/HP/MP/DEF 1/1,05/1,10/1,16/1,23/1,32; không tự lấy 8%/cấp (+40%) của proposal. Sửa mất giá trị slot bằng existing stats. **Working baseline TUNABLE:** Boots thêm 2 EVA/cấp; Ring thêm 2 ACC và 0,2 điểm % CritChance/cấp; Necklace thêm 2 EVA/cấp. Rarity áp primary list mới ở GDD; Common trong bảng nên giữ kết quả số cũ. Enhance multiplier cho ATK/HP/MP/DEF; ACC/EVA tăng flat hiện dùng, không nhân lặp. Giữ fractional stat nội bộ. Đây là phương án sửa từ nguồn V5.1, chưa gọi mọi giá trị là user-locked; flat gains được playtest cùng GEAR-01 trước freeze data.

| Bạch Vân Common | +0 | +1 | +2 | +3 | +4 | +5 |
| --- | --- | --- | --- | --- | --- | --- |
| Weapon | 24,00 ATK | 25,20 ATK | 26,40 ATK | 27,84 ATK | 29,52 ATK | 31,68 ATK |
| Armor | 70,00/7,00 HP/DEF | 73,50/7,35 HP/DEF | 77,00/7,70 HP/DEF | 81,20/8,12 HP/DEF | 86,10/8,61 HP/DEF | 92,40/9,24 HP/DEF |
| Pants | 45,00/4,00 HP/DEF | 47,25/4,20 HP/DEF | 49,50/4,40 HP/DEF | 52,20/4,64 HP/DEF | 55,35/4,92 HP/DEF | 59,40/5,28 HP/DEF |
| Boots | 3,00/8 DEF/EVA | 3,15/10 DEF/EVA | 3,30/12 DEF/EVA | 3,48/14 DEF/EVA | 3,69/16 DEF/EVA | 3,96/18 DEF/EVA |
| Ring | 8 ACC/1,5% Crit | 10 ACC/1,7% Crit | 12 ACC/1,9% Crit | 14 ACC/2,1% Crit | 16 ACC/2,3% Crit | 18 ACC/2,5% Crit |
| Necklace | 30,00/8 MP/EVA | 31,50/10 MP/EVA | 33,00/12 MP/EVA | 34,80/14 MP/EVA | 36,90/16 MP/EVA | 39,60/18 MP/EVA |

**Validation:** primary tăng strict mỗi bước; boots EVA/ring ACC/necklace EVA tăng strict ở mọi tier. Tân Lữ boots không có EVA base vẫn thêm 2/cấp; fractional DEF không bị round mất. Tinh Hoa+4 đã có concept fixed-slot bonus, exact amounts/rarity interaction chưa chốt nên bảng chưa cộng. Probe tại S13, không gán bonus vào baseline damage. Không tự thêm +10 DEF quần/+5%HP/+5%MP/+2%Crit weapon từ audit; chúng có thể đổi balance lớn. Rarity × enhance không đổi CritMultiplier 1,5.

<a id="quest-progression"></a>

## S11 — Quest EXP và nhịp farm (SIMULATION / PROPOSAL)

**Question:** EXP8–15% giữ farm core, gap nào dài? **Model:** bar tại RequiredLevel; reward=floor(Need×p+0,5). Q5/Q6 Lv5, Q7 Lv7, Q8 Lv8, Q9 Lv12, Q10 Lv15, Q11 Lv17. Q6 baseline 0 được thử% trong model, **không thêm EXP Q6 vào GDD**; Q12 cap không EXP. Legacy chỉ input tạm, không reward final.

| Quest | Legacy EXP / bar% | A8% | B10% | C12,5% | D15% |
| --- | --- | --- | --- | --- | --- |
| Q5 | 180 / 40,0% | 36 | 45 | 56 | 68 |
| Q6 | 0 / 0,0% | 36 | 45 | 56 | 68 |
| Q7 | 210 / 25,3% | 66 | 83 | 104 | 125 |
| Q8 | 320 / 29,6% | 86 | 108 | 135 | 162 |
| Q9 | 700 / 25,9% | 216 | 270 | 338 | 405 |
| Q10 | 1.300 / 27,1% | 384 | 480 | 600 | 720 |
| Q11 | 1.800 / 26,9% | 536 | 670 | 838 | 1.005 |

Model khởi đầu Lv5/0 EXP sau catch-up790; normal cùng level player, carry EXP qua gate/map GDD. Q8 EliteLv8=152 EXP;12 normal Lv17 Q11=1.044 EXP **đã trong kill budget**. Counts chưa chốt Q4/Q5/Sói Q8/Đạo Tặc Q10 không invent: đây là normal-equivalent budget, chưa đủ script journey. Không thêm Elite khác/co-op/demo/Tinh Hoa. Q9 thử làm/bỏ, không chặn Q10.

| Profile | EXP Q5–Q11 có/bỏQ9 | Tổng quest kể cả onboarding có/bỏQ9 | Normal kills có/bỏQ9 | Kill budget×9s cóQ9 |
| --- | --- | --- | --- | --- |
| A8% | 1.360 / 1.144 | 2.150 / 1.934 | 721 / 725 | 108,2 phút |
| B10% | 1.701 / 1.431 | 2.491 / 2.221 | 715 / 720 | 107,3 phút |
| C12,5% | 2.127 / 1.789 | 2.917 / 2.579 | 708 / 714 | 106,2 phút |
| D15% | 2.553 / 2.148 | 3.343 / 2.938 | 701 / 709 | 105,2 phút |

Quest kể onboarding/cóQ9≈4,0–6,3% cumulative53.100; phần lớn từ farm/Elite. A→D giảm20 normal, tổng701–725: sát/vượt target 600–720, không ép model khớp bằng reward tự chốt. BỏQ9 thêm 4–8 kills, vẫn thông main story.

| Segment sau turn-in | A kills | B kills | C kills | D kills | Dải phút ở9s/kill | Nhịp / risk |
| --- | --- | --- | --- | --- | --- | --- |
| Q6→Q7/Lv5→7 | 36 | 36 | 35 | 34 | 5,1–5,4 | Gom cụm/equip, gap ngắn |
| Q7→Q8/Lv7→8 | 24 | 22 | 22 | 21 | 3,2–3,6 | Chuẩn bị Elite |
| Q8→Lv12 | 134 | 135 | 133 | 133 | 20,0–20,3 | Khám phá Bạch Vân, gap dài |
| Q9→Q10/Lv12→15 | 154 | 153 | 152 | 151 | 22,7–23,1 | Xích Nham; bỏQ9 vẫn farm |
| Q10→Q11/Lv15→17 | 130 | 128 | 127 | 125 | 18,8–19,5 | Gear/đá và điều tra |
| Q11→Q12/Lv17→20 | 231 | 229 | 227 | 225 | 33,8–34,7 | Gap dài nhất, cần mục tiêu gear/Elite |

Gap là farm thiếu **sau turn-in**, chưa gồm 12 objective Q11; cộng12 vào tổng. Carry/round có thể làm kill count không giảm đơn điệu. Lv12 là checkpoint phân tích; solo Q8→Q10 khi Lv15.

Cycle giả định6/9/12s/normal-equivalent kill →70,1–72,5 /105,2–108,8 /140,2–145,0 phút farm (**ASSUMPTION FOR SIMULATION**). Chưa cộng travel/NPC/inventory/death/Elite-Boss wait/PvP/tìm vật chứng: không phải tổng journey2,5–4h đã đo. Player farm thêm khi READY rồi trả gần đầy vẫn có thể lên level; % nhỏ không bảo đảm không bao giờ ding.

**Recommendation:** thử B10%, A/C/D làm sensitivity; Q6 có thể giữ 0 vì class/weapon/skill đủ reward. Đo gap8→15/17→20 với mục tiêu gear/đá/Elite sẵn có. Model trả ở minimum gate không reward-skip gate tiếp, trừ Q5→Q6 cùngLv5 chủ ý. Legacy tạm giữ đến khi duyệt QUEST-03, không tự lock.

## S12 — Respawn và Q12 availability

**Respawn sensitivity:** test8/12/15s. Toy model group3 normal clear 9s, chờ từ last death →cycles 17/21/24s →635,3/514,3/450 kills/h; so12s: +23,5%/0/−12,5%. Gold/EXP/stone cùng level tỷ lệ tương ứng. Giả định một bãi, chưa18 groups/travel/stagger. Đo empty-bay/income; không cộng wait lần hai vào S11 cycle.

**BOSS-03:** timer15 phút có thể khiến Q12 đợi gần15 phút. Arrival uniform giả định cho wait trung bình7,5 phút; không FACT về player arrivals. Contribution không thay encounter availability.

| Hướng | Ưu / cost | Recommendation, chưa lock |
| --- | --- | --- |
| A — Giữ world wait | Không thêm lifecycle; wait có thể dài/gãy finale | Baseline hiện hành; UI timer rõ, farm gear/Cổ Vệ trong lúc chờ |
| B — Q12 spawn window sớm | Giảm wait; ảnh hưởng cadence/reward/người đang fight | Chỉ xem sau gate abuse/repeated accept và một global Boss |
| C — Availability first-story qua cùng hệ Boss | Finale dễ tiếp cận; thêm routing/concurrency/state | Chưa duyệt; không tự dựng private instance/entity |
| D — Accept Q12 sớm, tracker dẫn về Boss sắp spawn | Không đổi15 phút, chủ động chọn thời điểm vào | Thử UX/routing trước cơ chế spawn mới, giữ RequiredLevel20 |

Thử A+D trước bằng timer/farm sẵn có; nếu wait phá journey mới review B. Không auto-complete Q12/thay kill/trao reward khi thiếu contribution.

## S13 — Rarity sáu slot và Tinh Hoa+4

**Evidence:** primary rarity list GDD gồm ACC/EVA, fixedCrit không scale. Ví dụ Bạch Vân+0; rarity nhân base primary → enhance ATK/HP/MP/DEF → flat ACC/EVA, không nhân lặp.

| Slot / primary | Common1,00 | Uncommon1,08 | Rare1,16 | Epic1,25 |
| --- | --- | --- | --- | --- |
| Weapon ATK | 24 | 25,92 | 27,84 | 30 |
| Armor HP/DEF | 70/7 | 75,6/7,56 | 81,2/8,12 | 87,5/8,75 |
| Pants HP/DEF | 45/4 | 48,6/4,32 | 52,2/4,64 | 56,25/5 |
| Boots DEF/EVA | 3/8 | 3,24/8,64 | 3,48/9,28 | 3,75/10 |
| Ring ACC | 8 | 8,64 | 9,28 | 10 |
| Necklace MP/EVA | 30/8 | 32,4/8,64 | 34,8/9,28 | 37,5/10 |

Mỗi tier/slot có primary dương, rarity multiplier tăng strict nên gain thật; Tân Lữ BootsEVA0 vẫn có DEF 1 tăng. Giữ fractional stat; RingCrit1,5% không nhân rarity, CritMultiplier1,5 cố định.

**Tinh Hoa BASELINE/TUNABLE; probe chỉ ASSUMPTION FOR SIMULATION**, chưa stat data/S03/S07, fixed bonus không random affix.

| Slot, Bạch Vân Common+4 | Chưa bonus (S10) | Probe chưa duyệt | Sau probe |
| --- | --- | --- | --- |
| Weapon | 29,52 ATK | +0,5 điểm%CritChance | 29,52 ATK và+0,5 điểm%Crit |
| Armor | 86,1 HP/8,61 DEF | +10 HP | 96,1 HP/8,61 DEF |
| Pants | 55,35 HP/4,92 DEF | +1 DEF | 55,35 HP/5,92 DEF |
| Boots | 3,69 DEF/16 EVA | +4 EVA | 3,69 DEF/20 EVA |
| Ring | 16 ACC/2,3%Crit | +4 ACC | 20 ACC/2,3%Crit |
| Necklace | 36,9 MP/16 EVA | +10 MP | 46,9 MP/16 EVA |

Probe Weapon+0,5 điểm% tăng expected damage≈0,235% tại Crit 12,5%;+1 DEF giảm incoming;MP+10 tăng Food III0,25 MP/s trước class modifier, chưa chữa deficitS04. Fixed bonus so rarity-scaled: EpicHP/MP 12,5 thay 10. **GEAR-01 chưa chọn amounts/interaction**. Derive bonus theo item+enhance, giữ+5, không stack equip/load; test cùng flat gainsS10.

<a id="review-decisions"></a>

# 4. Design Review Decisions

| Topic | Finding | Decision | Reason |
| --- | --- | --- | --- |
| Journey/quest | State/level gate và turn-in thiếu | GDD §6 owns content; Host chỉ claim khi đúng NPC | READY không kết thúc khám phá/farm |
| Rarity | ACC/EVA bị bỏ khỏi rarity list | List theo slot; S13 kiểm gain | Ring rarity không còn tương đương |
| Tinh Hoa | Hidden+4 chưa có concept | Fixed slot bonus existing stats, amounts còn GEAR-01 | Không random affix/system mới |
| Source audit | Deep audit chủ yếu review V4 dù title V5 | V5 là baseline; proposal phải phân nhãn | Tránh lấy draft cũ override |
| Phong Trảm/pierce | Power V5 trộn falloff proposal | Khôi phục power từng target V5 | Deterministic contradiction |
| Normal damage | V5.1 ghi skill bonus cho mọi damage | Normal không nhân INT bonus | Baseline phân rõ normal/skill |
| Roster/AI | Research thêm archetype, Sói ranged, riêng projectile | Giữ 6 roster user và một Linh Đạn; Hybrid reachable→Chase | Scope/art đã khóa |
| Combat physics | Thiếu collision/vertical/facing | Giữ no contact, auto-facing, AttackOrigin baseline | Fix rõ cách core combat chạy |
| Hit feedback | Review gọi stun 0,1s | Chỉ visual; chưa thêm stun | Tránh tự mở CC |
| Elite | Table lẫn quest reward và drop | HP×4/ATK×1,5; reward quest tách loot kill | Tránh thưởng quest mỗi respawn |
| Q9 | Solo thiếu Client 2 | Optional branch; Q10 không require Q9 | User lock; không auto reward/bot |
| Q10/Q12 | Agent thêm Elite kill bắt buộc/Hộp Epic | Trả objectives/reward V5 | Proposal không phải decision |
| Narrative | Lễ Nhập Lộ tự mang LOCKED | Giữ synopsis V5, tên roster user | Chưa có chứng cứ duyệt rewrite |
| Boss phase | Enrage tự thành P0 | P1 candidate, không DoD | Không có trong V5/user lock |
| Boss story | Global progression ảnh hưởng mọi client | Per-character Q12/display | World entity vẫn một |
| Boss reward | Hard threshold/demo lỗi | RuntimeMaxHP×5%; owner-only 90s | Giữ đúng threshold/runtime |
| Boss reset | Dead body giữ arena | Alive=0 trong 10s clear HP/position/threat/contribution | User deterministic fix |
| Network | Hide/show được gate P0 | MapId/filter local P0; observers P1 | Vertical slice không chờ optimization |
| Profile/save | Client state trusted/save fail bị bỏ | Host load ID; persist fail resource; transactions idempotent | Ngăn duplication/state loss |
| QA readiness | Research đánh [x] dù chưa game | Tất cả acceptance chưa chạy | Evidence không bằng proposal |
| Economy/art | Giờ art≈164–210 vượt total budget | Gate reuse/import sớm; 160–200h target/160–240h envelope | Không cam kết schedule giả |

Nguồn đã đọc đầy đủ: V5 baseline, deep audit, V5.1 audit, NSO research plan, V5.1 current, Open Questions, bảy file design-research và GDD có sẵn ở design. Các nguồn cũ migrate theo chủ đề, không giữ bản dump. NSO runtime setup, packet opcodes/account test chỉ phục vụ reference ngoài design; không chuyển thành Huyền Lộ requirement hoặc chạy lại database trong vòng này.

<a id="open-decisions"></a>

# 5. Open Decisions

Một bảng duy nhất. Deadline là gate tương lai, không ngày đã hoàn tất. OPEN/PLAYTEST/P1/DEFERRED không đổi baseline GDD; CRITICAL là gap phải chốt trước milestone phụ thuộc, không hỏi lại design lock.

| ID | Chủ đề | Cần chốt | Baseline hiện tại | Priority | Deadline | Status |
| --- | --- | --- | --- | --- | --- | --- |
| BAL-01 | INT | 0,25/0,30/0,35% đủ value? | 0,25%; S03/S04 | HIGH | Balance pass trước data freeze | PLAYTEST |
| BAL-02 | MP sustain | Intended deficit và Potion dependency? | Giữ Food/MP costs; S04 | CRITICAL | Combat slice sustain gate | PLAYTEST |
| QUEST-01 | Q9 UX | Hiện/skip/revisit branch ra sao? | Optional; Q10 không chặn, reward chỉ PvP | HIGH | Quest framework | OPEN |
| PHY-01 | Collider | Kích thước final và vertical hitbox? | 0,60–0,65×1,45u; originY+0,8 | HIGH | Movement/combat slice | PLAYTEST |
| CC-01 | Freeze | 25% và refresh/stack nhiều caster? | Normal 1,2s / Elite 0,6s; Boss/PvP immune | MEDIUM | CC balance pass | PLAYTEST |
| SCOPE-01 | Linh Biến | Có làm P1, chance/drop/cap nào? | Không P0; 3%/HP×3 chỉ proposal | MEDIUM | Sau world P0 | P1 |
| SCOPE-02 | Upgrade Transfer | Có làm P1? | Same slot/higher tier, Old−1,500/2 | HIGH | Sau economy P0 | P1 |
| SCOPE-03 | BuffLv10 | Có làm P1? | R, CD40s/MP 15; không simulation P0 | MEDIUM | Sau combat P0 | P1 |
| SCOPE-04 | Linh Giáp | Có làm P1 và CC workload? | Không P0; 1.000/Groggy chỉ proposal | MEDIUM | Sau Boss P0 | P1 |
| SCOPE-05 | Thứ tự P1 | DPS meter/QoL/transfer/network xếp sao? | Gate P0 trước, chưa thứ tự cuối | HIGH | Review tuần 4 | OPEN |
| BAL-03 | AGI speed | Giữ 0,05%/điểm? | ACC/EVA chính; speed TUNABLE | MEDIUM | Movement/balance pass | PLAYTEST |
| GEAR-01 | Tinh Hoa/rarity | Exact bonus amounts và rarity interaction? | Concept slot GDD; S10/S13 probe chưa khóa | HIGH | Gear data freeze | PLAYTEST |
| COOP-01 | Co-op reward | EXP split/bonus/radius, normal loot ownership? | Co-op P0; không party system | CRITICAL | Online milestone 6 | OPEN |
| CONS-01 | Potion/Food | HP/MP CD chung hay tách; Food death/logout? | CD 8s/tier group, tick 2s | HIGH | Consumable/persistence gate | OPEN |
| SAVE-01 | Restore state | HP/MP/dead/Food và restart map restore? | Host JSON; không tự heal/regen offline | HIGH | Persistence acceptance | OPEN |
| PVP-01 | Match edge | Timeout hòa, disconnect và restore combat state? | Best1/120s/%HP thắng | HIGH | PvP gate tuần 7 | OPEN |
| QUEST-02 | Tutorial/reward | Counts/supply Q4/Q5/Q7, vật chứng Q8/Q10, catch-up BaseEXP/full bag? | Q3 hữu hạn/reset nhanh, cấp trước một lần; không grind tutorial | HIGH | Quest/economy framework | OPEN |
| BOSS-01 | Enrage | Có giữ Cuồng Mạch P1? | Basic+3 pattern P0; không tự -20%CD | MEDIUM | Sau Boss P0 | DEFERRED |
| BOSS-02 | Boss stat/scheduler | DEF/ACC/EVA, số vùng/overlap/scheduler đá rơi? | HP 32k/ATK 140 baseline; S07 giả định DEF 25/EVA 60 | HIGH | Boss framework tuần 6 | OPEN |
| ART-01 | Projectile feel | Arrow speed, telegraph/aim và hybrid timeout? | Generic mob speed V5; không homing mới | MEDIUM | Combat slice | PLAYTEST |
| NAR-01 | Lore polish | Có duyệt Lễ Nhập Lộ/quest rename? | Synopsis V5, roster user | LOW | Trước quest text final | DEFERRED |
| LOOT-01 | Gear/Boss rarity roll | Normal/Elite independent hay một gear roll; Boss independent hay Epic supersede Rare? | Giữ tỷ lệ GDD; chưa có final semantics | HIGH | Trước LootService data freeze | OPEN |
| BOSS-03 | Story Boss availability | Q12 chờ world respawn hay availability tối giản? | World Boss15 phút; không story instance tự thêm | HIGH | Trước Q12 acceptance | OPEN |
| QUEST-03 | Quest EXP final | Profile8/10/12,5/15%, Q6 có EXP hay giữ 0? | Target8–15%; S11 ưu tiên thử10%, chưa đổi reward data | HIGH | Journey/balance pass | PLAYTEST |
| TECH-01 | Profile binding | ID→connection và duplicate selection policy? | Host load ID; một active writer/character | HIGH | Network spike/profile gate | OPEN |

<a id="research-ideas"></a>

# 6. Accepted / Rejected Research Ideas

| Idea | Status | Note |
| --- | --- | --- |
| Data-driven shapes, Host validate | ACCEPT | Pattern hữu ích, luật theo GDD |
| SpawnGroup, modular sprite, absolute timers | ACCEPT | Core đã được khóa/giữ V5 |
| Bag Sort / protection gear | P1 | Merge stack compatible, confirm đồ quý; không thêm P0 |
| Dynamic Linh Biến | P1 | Chỉ nếu core ổn, chạy lại economy |
| Upgrade Transfer / Lv10 buff / Burn | P1 | Không thay P0 rotation |
| Boss Linh Giáp / Enrage | DEFERRED | Chưa design-lock, không gate P0 |
| Hộp Epic Q12 / Top DPS Boss Loot | REJECT | V5 reward và PersonalLoot giữ nguyên |
| Chỉ số sát thương chí mạng riêng | REJECT | CritMultiplier 1,5 cố định |
| Kháng nguyên tố, hút máu, giảm hồi chiêu, damage Boss, proc gear | REJECT | Thêm stat/system ngoài scope |
| Roster ứng viên mở rộng và projectile riêng | REJECT | Art scope user đã khóa |
| FFA sau reservation/loot Top1 | REJECT | Owner-only trọn 90s |
| Female rig MVP, SkillRank, nhiều tiền tệ | REJECT | Scope lock |
| EXP debt, Decoy, stone fusion, Hương EXP | REJECT | V5 DROP |
| Hazard / Buy-back | P2 | Không yêu cầu core mới |
| NSO TCP/XOR, SQL 59 tables, naming/assets | REFERENCE | Không port vào Huyền Lộ; không override design |


**Thông số proposal P1, chưa khóa:** Buff Lv10 đề xuất: Chiến Ý +15% ATK/+5% speed hoặc Ưng Nhãn Cường Hóa +10 điểm % Crit/+15% range, 10s/CD40s/MP 15, R; không đưa vào P0 simulation. Transfer đề xuất cùng slot, higher tier, consume đồ cũ, 500 Vàng+2 đá, `New=max(0, Old−1)`; chưa triển khai trước quyết định scope.
