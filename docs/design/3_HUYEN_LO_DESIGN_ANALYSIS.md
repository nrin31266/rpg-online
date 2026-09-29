# Huyền Lộ — Phân tích thiết kế

**Đối chiếu:** [GDD hiện hành](1_HUYEN_LO_GDD.md) · **Ngày:** 2026-09-29 · **Vai trò:** evidence, simulation, review và quyết định mở.

[GDD](1_HUYEN_LO_GDD.md) là nguồn design authority; [Technical](2_HUYEN_LO_TECHNICAL.md) là implementation contract. FACT ở đây là dữ kiện tài liệu hoặc phép tính kiểm lại; SIMULATION là kết quả phụ thuộc giả định; PROPOSAL không tự thành luật game.

<a id="balance-baselines"></a>

# 1. Balance Baselines

## S01 — EXP và onboarding

**Question:** cumulative và kill budget có thực sự suy ra từ bảng? **Evidence:** cộng 19 giá trị GDD cho 53.100; Lv20 bắt đầu tại 53.100, không có requirement lên Lv21.

Bảng Need/cumulative do [GDD §3](1_HUYEN_LO_GDD.md#gdd-3) sở hữu. Kiểm toàn bộ 20 mốc: tổng 19 requirement = **53.100**; Lv5=790, Lv12=9.100, Lv17=29.600, Lv20=53.100.

**Calculation:** `round = floor(x+0.5)` cho số dương; kill mỗi level = ceil(Need/MobBaseEXP). Tổng bỏ carry là **799**. Với EXP dư được carry qua level, kết quả khác; chỉ dùng kill count có mô hình tái lập. Final kill target PLAYTEST/TUNABLE; range hiện tại ở S11 là model, không chứng minh lower bound 600 hoặc journey 2,5–4h.

Catch-up: Lv4 Need 320, đã có 200 → Remaining 120; reward bù 120 đưa lên Lv5, không 320. Khi đã đạt mốc, chỉ BaseEXP. Q1–Q4 tổng tối đa 790 EXP nếu chưa có nguồn khác; không cộng thêm 790 lần nữa khi dùng mô hình normal farming đã đi qua Lv1–4. EXP quest cũ được giữ như input **legacy**, không dùng tổng cũ làm FACT cho journey mới. Reward Q5–Q11 retune ở S11/QUEST-03; GDD chỉ dẫn TUNABLE — QUEST-03; legacy numeric chỉ ở S11.


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

**Question:** parity và build value theo skill hiện hành? **Model:** Lv20, sáu Common Huyền Tích +0, không hidden option/buff P1. Gear: ATK 44, HP 240, DEF 28, MP 60, ACC 16, EVA 32, Crit 2,5%. Chỉ class/passive hiện hành; random trung bình 1, chưa round mỗi hit. Mob Lv20: HP 800/DEF 18/ATK 70/ACC 140/EVA 60; incoming lấy Cổ Vệ interval 1,6s, mob CritChance0 P0 theo GDD.

`ExpectedHit = ATK×100/(100+DEFtarget)×(1−EvadeTarget)×(1+0.5×CritChance)`.

`NormalDPS = ExpectedHit×NormalPower/Interval`.

`SkillDPS = ExpectedHit×(1+INT×0.0025)×Σ(primaryPower/CD)`.

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

**Conclusion:** Với setup trên, AGI 95 vẫn được thêm ACC/EVA nhưng không bảo đảm all-in tối ưu; +4,75% speed chỉ phần thưởng phụ. INT 95 tăng skill 23,75%, không tăng normal; raw STR có thể vẫn thắng damage. Không kết luận INT đã viable chỉ từ một cột skill bonus.

Cùng ATK/ACC/gear, ratio Bow/Sword normal = `(0.95/0.80)/(1/0.72) × (1+0.5CritBow)/(1+0.5CritSword)`. Crit Sword/Bow = 12,5%/15,5% (Bow/Sword = 15,5%/12,5%) cho Bow/Sword≈0,867, Bow thấp hơn≈13,3% (hoặc Sword cao hơn≈15,3% nếu lấy Bow làm mẫu số). Không trộn hai cách ghi %.

**Mirror comparison — SIMULATION, cùng VIT/INT/STR/AGI = 24/24/24/23:** cùng sáu Common Huyền Tích +0/S03, khác class modifiers/passive/rotation đúng GDD. Ba targets đứng trong shape, full falloff đúng từng target; normal vẫn một target. Không cast-lock/projectile travel/overkill/reduced target count sau kill nên clear time chỉ lower bound.

| Class | HP / MP | Normal DPS | 1-target upper DPS / TTK s | 3-target aggregate upper / clear s | Incoming một Cổ Vệ DPS | MP spend / FoodIII regen / deficit mỗi s | OOM / khoảng PotionIII lý thuyết s |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Kiếm | 816,2 / 316,0 | 118,9 | 257,3 / 3,1 | 528,2 / 4,5 | 23,41 | 12,36 / 7,90 / 4,46 | 70,9 / 42,5 |
| Cung | 742,0 / 347,6 | 103,1 | 216,3 / 3,7 | 290,5 / 8,3 | 24,15 | 11,41 / 8,69 / 2,72 | 127,7 / 76,6 |

Combat uptime 100/75/50% là sensitivity chung: DPS nhân uptime, TTK chia uptime; không tự cho Cung 100% vì có range. Incoming trên giả định không kite và một mob cùng layer, không dùng cho ba mobs luôn hit. Runtime mirror test phải đo melee exposure, kiting, uptime, 1/3-target clear, cast MP và số Potion thật; BAL-01/02/03 chưa được đóng bằng upper-bound DPS.

## S04 — MP sustain và INT sensitivity

**Question:** Food bù spam skill tới mức nào? **Model:** cùng Common Huyền Tích +0 như S03; INT 0/20/40/60/95, Food III, full skill on-cooldown P0. `SwordMPPerSecond = 8/1.4 + 14/4 + 22/7`; `BowMPPerSecond = 10/2 + 16/4.5 + 20/7`. Không tính buff Lv10. Average FoodMP/s = MaxMP×0,025; drift=spend−regen; OOM=MaxMP/drift nếu drift>0. Đây là xấp xỉ liên tục; tick 2s/cast discrete có thể làm thiếu MP trước kết quả này.

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

Đổi coefficient không đổi MaxMP/Food/spend/OOM, chỉ đổi damage gain. Threshold sustain trung bình Sword≈59,7 INT; Bow≈43,8 INT trong setup này. Không Food: OOM=MaxMP/spend, tại INT 0 Sword≈15,9s và Bow≈18,9s. Potion III phục hồi 60%MP; tại thiếu hụt dương, khoảng cách uống lý thuyết `0.6MaxMP/drift`, nhưng còn CD 8s và discrete burst. Không đổi master coefficient hoặc Food chỉ từ phép tính; BAL-01/BAL-02 cần xác nhận ý đồ và test actual cast lock.

## S05 — Food HP sustain

**Question:** Food có thực sự bù 100% một mob? **Model:** S03, Cổ Vệ 70 ATK/1,6s, ExpectedIncoming=(70/1,6)×100/(100+DEF)×(1−PlayerEvade). Food III HPS=0,02MaxHP, Potion III upper HPS=0,6MaxHP/8; không over-heal/latency/chi phí Potion.

| Build | Incoming/mob DPS | Food HPS | Mob equiv Food | Food+Potion upper |
| --- | --- | --- | --- | --- |
| VIT95 | 24,50 | 28,82 | 1,18 | 5,59 |
| Kiếm mẫu | 23,98 | 17,38 | 0,72 | 3,44 |
| Cung mẫu | 24,55 | 13,40 | 0,55 | 2,59 |

Q3 mới trao Mộc Kiếm nên không benchmark combat Lv1 với vũ khí sẵn có. Ví dụ **Tân Lữ Lv3 sau equip**, auto4VIT/2INT/4STR: HP 172/MP 78/ATK 25,2/DEF 6,6; Food I 1,72 HP/s và 1,17 MP/s. Target giả định Nấm Lv3 ATK 12/1,8s, mobACC72/playerEVA24; incoming≈5,81DPS → Food≈0,30mob-equivalent. Kiting/clear nhanh mới giảm incoming thực tế; Food không được quảng bá đủ mọi build.

## S06 — Multi-target và Freeze

**Calculation:** Phong Trảm ba target tổng 3,75×; Liên Kích ba target tổng 4,50×; Kiếm Khí năm target tổng 7,50×; Xuyên Tiễn bốn target tổng 5,20×; Hàn Tiễn 1+4 tổng 4,60×, primary không nhận 0,80× lần hai; Liên Tiễn tổng 1,35× phân phối theo số target. Power GDD đã bao gồm falloff, không nhân lần hai.

Freeze 25%/cast CD 7s có uptime đơn giản Normal ≈0,25×1,2/7=4,29%, Linh Biến ≈2,14% nếu một target luôn trúng. Uptime thực thấp hơn khi né và cao hơn khi nhiều nguồn cast; không chứng minh CC balance từ kỳ vọng. Boss/PvP immunity giữ nguyên. Check stacking/refresh tại CC-01; không thêm stun 0,1s vào mọi hit vì biến feedback thành gameplay.

## S07 — Boss TTK sensitivity

**Question:** target 90–150s có còn đúng? **Model:** build Kiếm/Cung mẫu S03, **ASSUMPTION FOR SIMULATION: Boss DEF 25/EVA 60**, cả DEF/ACC/EVA đều TUNABLE tại BOSS-02, không phải GDD lock; ACC không cần cho mô hình outgoing TTK này. **2-player benchmark = SwordSample + BowSample**, không single-player. `CombinedDPS = (SwordSampleDPS + BowSampleDPS) * 118/125 * uptime`. Rotation upper chuyển DEF 18→25 rồi nhân uptime; chưa mô hình đổi exposure do Cuồng Mạch; không Linh Giáp/Burn/buff. Gear+0 cố ý minh bạch; không gọi là endgame+4 benchmark.

| Combat uptime | Effective DPS upper | Release 32.000 TTK | Demo 12.800 TTK |
| --- | --- | --- | --- |
| 100% | 511,1 | 62,6s | 25,0s |
| 75% | 383,3 | 83,5s | 33,4s |
| 50% | 255,5 | 125,2s | 50,1s |

**Conclusion:** đây là sensitivity, không kết quả Boss playable. Uptime đã gom cast-lock/né chiêu/di chuyển; phải đo riêng trước khi chỉnh HP. Không tăng HP vì đọc nhầm benchmark thành một người. Khi playable, đo 2/4 players, 8 nếu performance cho phép: TTK, telegraph readability/Cuồng Mạch, contribution/threat switching/eligibility; không HP auto-scaling. Boss DEF/ACC/EVA và count/overlap/scheduling vùng đá rơi thiếu specification, BOSS-02 cần chốt trước milestone 6. Demo HP multiplier phải áp cả threshold contribution; không chỉ sửa thanh HP.

<a id="economy-analysis"></a>

# 3. Economy Analysis

## S08 — Food affordability và income/sink

**Calculation:** `AverageGold = 4.5 + 2.5*L`. Bỏ gear sale/quest/Linh Biến, số mob bù một Food = Price/AverageGold; Gold auto-credit đã chốt; đây là award budget trước COOP distribution, không N bản reward đầy đủ.

| Lv | Gold/mob | Food giá | Mob/Food | Food cost/phút |
| --- | --- | --- | --- | --- |
| 1 | 7,0 | 150 | 21,4 | 15 |
| 10 | 29,5 | 400 | 13,6 | 40 |
| 20 | 54,5 | 700 | 12,8 | 70 |

Nguồn: normal/Linh Biến/Boss, quest đã chốt, sellValue explicit (shop Common author từ 25% buyPrice). Sink: Food/Potion, nâng gear, gear shop, Tẩy Mạch 1.200, Hồi Sinh 1.000, đá shop 800. Tại Lv20: reset≈22 mob; revive≈18,35 mob; đá≈14,68 mob. Q4 ứng 320 đủ Food 150+hai Potion 80=310, dư 10; không thưởng lại 320 khi reconnect/accept replay.

Gold milestone cần lịch mua và drop-sale realization để tái lập, chưa đủ data để coi là FACT. Before balance pass phải ghi kill count, travel/death time, tỷ lệ Food uptime, Potion counts, enhance attempts và transaction log theo ba phong cách; không tự lock quest ứng trước mới.

## S09 — Stone flow và variance

Normal 8%: expected 12,5 kills/stone. Giả định 672 kills: E = 53,76 đá, σ=sqrt(672×0,08×0,92)=7,03; khoảng E±2σ≈39,7–67,8 là xấp xỉ, không bảo đảm. “35–50 đá miễn phí” cũ không phải kỳ vọng của model này. Normal-only để trả expected 20,34 đá cho một món +5 cần≈254 kills; sáu món≈1.526 kills, chưa tính quest/Linh Biến/Boss.

Ở Lv20, vàng kỳ vọng theo 12,5 kills=681,25; shop 800 đắt hơn≈17,43% theo gross gold. Không kết luận farm luôn rẻ hơn khi thêm thời gian/Potion/death. Linh Biến P0 làm tăng flow; S16 mô hình cap1/rate10% test với stone1, không giữ legacy drop2–4 đá.

## S10 — Sáu slot đều tăng giá trị +0→+5

**Decision boundary:** giữ enhance multiplier GDD cho ATK/HP/MP/DEF 1/1,05/1,10/1,16/1,23/1,32; không tự lấy 8%/cấp (+40%) của proposal. Sửa mất giá trị slot bằng existing stats. **Working baseline TUNABLE tại GDD §7:** Boots thêm 2 EVA/cấp; Ring thêm 2 ACC và 0,2 điểm % CritChance/cấp; Necklace thêm 2 EVA/cấp. Rarity áp primary list mới ở GDD; Common trong bảng nên giữ kết quả số cũ. Enhance multiplier cho ATK/HP/MP/DEF; ACC/EVA tăng flat hiện dùng, không nhân lặp. Giữ fractional stat nội bộ. Flat gains được playtest cùng GEAR-01 trước freeze data.

| Bạch Vân Common | +0 | +1 | +2 | +3 | +4 | +5 |
| --- | --- | --- | --- | --- | --- | --- |
| Weapon | 24,00 ATK | 25,20 ATK | 26,40 ATK | 27,84 ATK | 29,52 ATK | 31,68 ATK |
| Armor | 70,00/7,00 HP/DEF | 73,50/7,35 HP/DEF | 77,00/7,70 HP/DEF | 81,20/8,12 HP/DEF | 86,10/8,61 HP/DEF | 92,40/9,24 HP/DEF |
| Pants | 45,00/4,00 HP/DEF | 47,25/4,20 HP/DEF | 49,50/4,40 HP/DEF | 52,20/4,64 HP/DEF | 55,35/4,92 HP/DEF | 59,40/5,28 HP/DEF |
| Boots | 3,00/8 DEF/EVA | 3,15/10 DEF/EVA | 3,30/12 DEF/EVA | 3,48/14 DEF/EVA | 3,69/16 DEF/EVA | 3,96/18 DEF/EVA |
| Ring | 8 ACC/1,5% Crit | 10 ACC/1,7% Crit | 12 ACC/1,9% Crit | 14 ACC/2,1% Crit | 16 ACC/2,3% Crit | 18 ACC/2,5% Crit |
| Necklace | 30,00/8 MP/EVA | 31,50/10 MP/EVA | 33,00/12 MP/EVA | 34,80/14 MP/EVA | 36,90/16 MP/EVA | 39,60/18 MP/EVA |

**Validation:** primary tăng strict mỗi bước; boots EVA/ring ACC/necklace EVA tăng strict ở mọi tier. Tân Lữ boots không có EVA base vẫn thêm 2/cấp; fractional DEF không bị round mất. Tinh Hoa+4 đã có concept fixed-slot bonus, exact amounts/rarity interaction chưa chốt nên bảng chưa cộng. Probe tại S13, không gán bonus vào baseline damage. Rarity × enhance không đổi CritMultiplier 1,5.

<a id="quest-progression"></a>

## S11 — Quest EXP và nhịp farm (SIMULATION / PROPOSAL)

**Question:** EXP8–15% giữ farm core, gap nào dài? **Model:** bar tại RequiredLevel; `reward = floor(Need*p + 0.5)`. Q5/Q6 Lv5, Q7 Lv7, Q8 Lv8, Q9 Lv12, Q10 Lv15, Q11 Lv17. Q6 baseline 0 được thử% trong model, **không thêm EXP Q6 vào GDD**; Q12 cap không EXP. Legacy chỉ input tạm, không reward final.

| Quest | Legacy EXP / bar% | A8% | B10% | C12,5% | D15% |
| --- | --- | --- | --- | --- | --- |
| Q5 | 180 / 40,0% | 36 | 45 | 56 | 68 |
| Q6 | 0 / 0,0% | 36 | 45 | 56 | 68 |
| Q7 | 210 / 25,3% | 66 | 83 | 104 | 125 |
| Q8 | 320 / 29,6% | 86 | 108 | 135 | 162 |
| Q9 | 700 / 25,9% | 216 | 270 | 338 | 405 |
| Q10 | 1.300 / 27,1% | 384 | 480 | 600 | 720 |
| Q11 | 1.800 / 26,9% | 536 | 670 | 838 | 1.005 |

Model khởi đầu Lv5/0 EXP sau catch-up 790; normal cùng level player, carry EXP qua gate/map GDD. Q8 forced Sói Linh Biến Lv8=38×3=114 EXP;12 normal Lv17 Q11=1.044 EXP **đã trong kill budget**. Counts chưa chốt Q4/Sói Q8/Đạo Tặc Q10 không invent: đây là normal-equivalent budget, chưa đủ script journey. Không thêm dynamic Linh Biến khác/co-op/demo/Tinh Hoa; Q8 bonus one-time giả định nhận hợp lệ. Q9 thử làm/bỏ, không chặn Q10.

| Profile | EXP Q5–Q11 có/bỏ Q9 | Tổng quest kể cả onboarding có/bỏ Q9 | Normal kills có/bỏ Q9 | Kill budget×9s có Q9 |
| --- | --- | --- | --- | --- |
| A8% | 1.360 / 1.144 | 2.150 / 1.934 | 722 / 726 | 108,3 phút |
| B10% | 1.701 / 1.431 | 2.491 / 2.221 | 716 / 721 | 107,4 phút |
| C12,5% | 2.127 / 1.789 | 2.917 / 2.579 | 709 / 715 | 106,4 phút |
| D15% | 2.553 / 2.148 | 3.343 / 2.938 | 702 / 710 | 105,3 phút |

Quest kể onboarding/có Q9≈4,0–6,3% cumulative 53.100; phần lớn từ farm. Current model **~702–726 normal kills + một forced Linh Biến**, thường gọi normal-equivalent budget; không phải target final. Bỏ Q9 thêm 4–8 kills, không softlock. Dynamic Linh Biến/COOP sẽ đổi số kills thực tế nên lower bound 600 chưa được chứng minh.

| Segment sau turn-in | A kills | B kills | C kills | D kills | Dải phút ở 9s/kill | Nhịp / risk |
| --- | --- | --- | --- | --- | --- | --- |
| Q6→Q7/Lv5→7 | 36 | 36 | 35 | 34 | 5,1–5,4 | Gom cụm/equip, gap ngắn |
| Q7→Q8/Lv7→8 | 24 | 22 | 22 | 21 | 3,2–3,6 | Chuẩn bị Sói Linh Biến |
| Q8→Lv12 | 135 | 136 | 134 | 134 | 20,1–20,4 | Khám phá Bạch Vân, gap dài |
| Q9→Q10/Lv12→15 | 154 | 153 | 152 | 151 | 22,7–23,1 | Xích Nham; bỏ Q9 vẫn farm |
| Q10→Q11/Lv15→17 | 130 | 128 | 127 | 125 | 18,8–19,5 | Gear/đá và điều tra |
| Q11→Q12/Lv17→20 | 231 | 229 | 227 | 225 | 33,8–34,7 | Gap dài nhất, cần mục tiêu gear/Linh Biến |

Gap là farm thiếu **sau turn-in**, chưa gồm 12 objective Q11; cộng 12 vào tổng. Carry/round có thể làm kill count không giảm đơn điệu. Lv12 là checkpoint phân tích; solo Q8→Q10 khi Lv15.

Cycle giả định 6/9/12s/normal-equivalent kill →70,2–72,6 /105,3–108,9 /140,4–145,2 phút farm (**ASSUMPTION FOR SIMULATION**). Chưa cộng travel/NPC/inventory/death/Linh Biến-Boss wait/PvP/tìm vật chứng: không phải tổng journey2,5–4h đã đo. Player farm thêm khi READY rồi trả gần đầy vẫn có thể lên level; % nhỏ không bảo đảm không bao giờ ding.

**Recommendation:** thử B10%, A/C/D làm sensitivity; Q6 có thể giữ 0 vì class/weapon/skill đủ reward. Đo gap 8→15/17→20 với mục tiêu gear/đá/Linh Biến sẵn có. Model trả ở minimum gate không reward-skip gate tiếp, trừ Q5→Q6 cùng Lv5 chủ ý. Legacy chỉ giữ trong bảng S11 để so sánh, không reward authority.

**Farm goals — gợi ý từ hệ thống sẵn có, không objective bắt buộc:**

| Segment | Level Goal | Gear Goal | Enhance Goal | Linh Biến/World Goal | Story Guidance | Risk |
| --- | --- | --- | --- | --- | --- | --- |
| Lv5→7 | Mở Q7 | Thanh Mộc/class weapon | Gom đá; +1 tutorial ở Q7 | Thử gom cụm Trúc Ảnh | Tự đứng vững sau chọn class | Chưa biết đòn lan/MP |
| Lv7→8 | Mở Q8 | Bổ sung slot Thanh Mộc | Hiểu preview/cost, tiết kiệm đá | Chuẩn bị Sói Sương Linh Biến Q8 | Sói bỏ bãi cũ | Khoảng ngắn dễ bỏ qua gear |
| Lv8→12 | Mở Q9 optional/Xích ngoại vi | Bạch Vân Lv10, săn Rare | Đá và nâng gear đang dùng | Sói Linh Biến, khám phá thác | Theo dòng khí qua Bạch Vân | ~20m thiếu mục tiêu; Rare không guarantee |
| Lv12→15 | Mở Q10 | Chuẩn bị Xích Nham Lv15; Necklace shop/drop không cần Q9 | Dành đá/Vàng cho tier tới | Linh Biến Đạo Tặc khi đủ sức | Chỉ ngoại vi; đợi thực lực để điều tra sâu | ~23m; vượt sức Linh Biến trước 15 |
| Lv15→17 | Mở Q11 | Xích Nham slots còn thiếu | +2/+3 tùy resource | Linh Biến tại Xích Nham | Vật chứng cho thấy nguồn phá ấn | ~19m, enhance RNG |
| Lv17→20 | Mở Q12 | Rare class weapon Q11; Huyền Tích gear Lv20 | +3/+4 preparation, không ép đủ bộ | Outer Huyền Tích/Linh Biến, chuẩn bị Boss | Cổng đã mở; recommended 18, tu luyện tới20 | ~34m dài nhất, Boss access BOSS-03 |

Supply theo [GDD §6](1_HUYEN_LO_GDD.md#gdd-6) tách khỏi regular drop và không tính là tăng rate trong S09/S11. S11 giữ nguyên normal-equivalent model/range 702–726; chưa mô phỏng supply grant mới hoặc thời gian thao tác. Shop tier/loot acquisition và enhancement goals là gợi ý, không guarantee Rare hay +4 trong gap.

<a id="boss-availability"></a>

## S12 — Respawn và Q12 availability

**Respawn sensitivity:** test 8/12/15s. Toy model group 3 normal clear 9s, chờ từ last death →cycles 17/21/24s →635,3/514,3/450 kills/h; so 12s: +23,5%/0/−12,5%. Gold/EXP/stone cùng level tỷ lệ tương ứng. Giả định một bãi, chưa 25 groups/travel/stagger. Đo empty-bay/income; không cộng wait lần hai vào S11 cycle.

**BOSS-03:** timer 15 phút có thể khiến Q12 đợi gần15 phút. Arrival uniform giả định cho wait trung bình7,5 phút; không FACT về player arrivals. Contribution không thay encounter availability.

Tách story encounter khỏi world respawn là bằng chứng availability hữu ích ([provenance](#legacy-provenance)); implementation phải phù hợp world hiện tại. A+D chưa giới hạn wait, nên B/C vẫn cần so sánh trước Q12 acceptance.

| BOSS-03 option | Wait worst / mean | Concurrency và đóng góp | Loot/reconnect/abuse | Cost và continuity |
| --- | --- | --- | --- | --- |
| A — Shared World Boss 15m | Gần15m / 7,5m chỉ khi arrival uniform | N players khác state Q12 dùng chung entity; per-character5% | Existing death event dedup; reconnect theo connected-at-death; không first-clear trigger | LOW; world farm giữ nguyên nhưng finale dễ đứt nhịp |
| B — Shared Boss + guaranteed/accelerated story spawn | Cần chọn bound W; mean chưa mô hình, không gọi 0 khi đang fight | Không reset/spawn đè active Boss; serialize requests, player đã clear vẫn cùng fight | Persist one-time entitlement theo character; spawn request replay không làm thêm loot/cắt timer liên tục | MEDIUM; ít routing hơn C nhưng đổi world cadence, cần guard abuse |
| C — Same BossDefinition + first-story lifecycle riêng | 0 timer wait nếu idle/ready; queue/concurrency bound chưa chốt | Player chưa clear vào story encounter; đã clear farm world; share eligibility/range5%, không share contribution giữa encounters | encounterID/rewardKind/deathID, first-clear flag và pending reconnect phải atomic; quyết định story encounter có full farm loot hay không để tránh double reward | HIGH; reuse sprite/AI/pattern không art/map mới nếu sub-region routing đủ, nhưng thêm save/state/filter; world15m vẫn độc lập |
| D — Arena gate + chung Boss, route Q12 theo timer | Vẫn gần15m / uniform7,5m nếu không đổi spawn rule | Khác Q12 state cần per-character gate nhưng cùng fight; không tự trục xuất player đang săn | Existing personal reward; gate/accept replay không đổi timer; reconnect restore gate | LOW–MEDIUM; giảm spoil/hướng dẫn, **không bảo đảm availability** |

**Recommendation OPEN:** thử feasibility B trước vì giữ một global Boss/8 roots; so C nếu B không bảo đảm finale và chống first-clear abuse. D hỗ trợ guidance/spoiler cho B hoặc C, A là control baseline. Không lock spawn bound, private scene hay story loot policy. Nếu chọn C phải sửa GDD “một entity” thành cùng BossDefinition/sprite/AI/patterns nhưng runtime encounter có thể khác; hiện GDD vẫn một entity. N players ở different Q12 states, late join, reconnect, contribution reset và world reward cadence là gate của mọi proposal. Không complete Q12 khi thiếu Boss eligibility.

| Huyền Tích access option (thuộc BOSS-03) | Farm Lv17→20 / guidance | Spoiler / multiplayer | Code cost |
| --- | --- | --- | --- |
| A — Q11 Completed + Lv18 outer; arena tới Q12 | Lv17→18 phải farm Xích Nham; signpost rõ level | Per-character arena gate tránh vào sớm; khác state vẫn cần encounter filtering | MEDIUM; đổi gate, không thêm map |
| B — Q11 Completed outer Lv17, recommended 18; arena tới Q12 Available/Accepted | Giữ farm gap hiện tại, warning Lv18 | Có thể filter landmark/banner trước Q12; gate state Available hay Accepted còn phải chọn; khác state kiểm Host/visibility | LOW–MEDIUM; ưu tiên khảo sát vì giữ map route |
| C — Toàn map mở, lifecycle tại BOSS-03 | Không đổi farm access | Hiện có thể thấy/fight Boss trước Q12; lifecycle riêng chỉ giải quyết khi thực sự tách access/display | Phụ thuộc BOSS option, không mặc định rẻ |

**Current design gap:** range18–20 là **khuyến nghị**, gate Q11 hiện cho vào17, không numeric contradiction sau clarify. Boss access/spoiler trước Q12 **chưa giải quyết**; ưu tiên option B cùng BOSS-03, không tự lock arena barrier/scene trong vòng này. NAR-01 đóng văn phong không đóng Boss lifecycle. Không cần thêm MAP-01.

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

<a id="loot-consumable-analysis"></a>

## S14 — Loot semantics đã chốt / consumable UX còn mở

**Decision:** [GDD §7](1_HUYEN_LO_GDD.md#gdd-7) owns rates, map-tier, slot/weapon, potion-tier, sellValue và Gold delivery. Các channel A/B/C/D independent; E một exclusive gear roll. Không roll rarity độc lập rồi supersede, không weighted category chung làm mất guaranteed channels. Boss cũng exclusive Rare40/Epic8/None52: probability≥1 gear48%, đúng cả hai marginals và không multi-gear. Boss personal eligibility/owner-only90s giữ nguyên.

**Why Gold auto-credit:** bỏ physical coins/pickup traffic trên hành trình ~700 kills; cosmetic burst local không state. Không chốt COOP recipients bằng quyết định delivery. Uniform slots và Sword/Bow50/50 làm off-class weapon là 1/12 gear successes; với Normal5,1%, khoảng một off-class weapon/235,3 kills. Không smart-loot nhưng vendor value cho chúng; class reward Q6/Q11 vẫn đúng class. Shop Common là route xác định để lấp slot, không làm Rare/Epic vô dụng. LOOT-01 giờ chỉ tuning prices/rates/reward flow; evidence representation vẫn QUEST-02, distribution COOP-01.

| Quick Potion option — CONS-01 | Economy/UX | Cost / recommendation |
| --- | --- | --- |
| A Highest available | Baseline đơn giản; thiếu 10% vẫn dùng III 60%, waste | LOW; giữ trước decision |
| B Lowest tier đủ bù missing% | <=30% dùng I, <=45% II nếu I không đủ,  >45% III; khi không tier nào đủ dùng highest eligible | LOW; ưu tiên playtest, deterministic tiết kiệm, không full hotbar |
| C Preferred tier | Người chơi kiểm soát nhưng cần setting/fallback UI | MEDIUM; sau B nếu cần |

F Food CONS-01: khi chưa active chọn suitable tier (ưu tiên highest hay lowest chưa chốt); so same-tier refresh còn nhiều thời gian (consume vs reject/prompt), lower-overwrite-higher (baseline mới thay cũ vs guard), higher-overwrite-lower, gần hết timer, dead/logout/reconnect. Recommendation reject same-tier sớm và lower overwrite để tránh waste, **chưa đổi GDD**, timer vẫn 10 phút. CD HP/MP chung/tách và SAVE-01 offline/death còn mở.

COOP-01 tách **quest kill assist / EXP / normal loot ownership**. Candidate assist cùng MapId/QuestId/active objective, alive, AssistRadius, participation; snapshot eligible trước cập nhật step để tránh mất credit khi killer chuyển bước. Không share Equip/Use/Enhance/Talk/PvP/BossEligibility; Q12 vẫn contribution5% per character. Chính sách phải áp dụng cho N eligible players, không mặc định chia đôi hoặc chỉ chọn một đồng đội.

<a id="world-economy-analysis"></a>

## S15 — Representative mob stats (DERIVED)

GDD giữ formula và rounding half-up; bảng này chỉ projection, không stat authority thứ hai. Normal EXP=MobBaseEXP(Level), dynamic Linh Biến EXP=NormalEXP×3, Boss Lv20=0 EXP; forced Q8 dùng effective reward profile/receipt theo GDD, không full bonus cho mọi recipient. DEF/ACC/EVA không tự scale khi Linh Biến; ATK×1,3 giữ fractional runtime trước final damage rounding.

| Lv | HP | ATK | DEF | Normal EXP | Linh Biến EXP | Gold range |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | 93 | 8 | 3 | 13 | 39 | 5–9 |
| 5 | 170 | 16 | 6 | 26 | 78 | 13–21 |
| 8 | 253 | 24 | 8 | 38 | 114 | 19–30 |
| 10 | 320 | 30 | 10 | 47 | 141 | 23–36 |
| 15 | 530 | 48 | 14 | 75 | 225 | 33–51 |
| 20 | 800 | 70 | 18 | 108 | 324 | 43–66 |

## S16 — Linh Biến flow / prices (SIMULATION / TEST)

**Event model:** một map 15 slots (5×3); mỗi slot chết respawn 12s, Host cap1 khi roll10%. Normal kill opportunities Poisson 400/player-hour, random available normal slot; Linh Biến sống 45s rồi bị hạ, không bị normal scheduler giết thêm. 80 seeds 0–79, mỗi run 6h/burn-in 1h; invariant cap1 kiểm mỗi event. Đây là giả định throughput và lifetime, không claim clear speed/encounters thật. Một reward budget/death, **không nhân reward theo số player** khi COOP chưa chốt; không Boss/quest/supply, stats tại Lv15 (Gold mean 42). Variant 3×HP được biểu diễn bằng lifetime 45s; model không phân DPS/action/pathing giữa players.

| Players | Normal kills/h | Linh Biến/h | Gold/h | Stone/h | Gear/h | Material/h | Gold / Stone / Gear tăng so normal-only cùng tổng kills |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 1 | 402,1 | 28,3 | 20.452 | 60,5 | 30,8 | 163,1 | +13,2% / +75,6% / +40,5% |
| 2 | 800,1 | 41,5 | 38.836 | 105,5 | 56,0 | 302,3 | +9,9% / +56,7% / +30,4% |
| 4 | 1.599,2 | 54,5 | 74.028 | 182,4 | 101,4 | 561,4 | +6,6% / +37,9% / +20,3% |

Cap1 khiến variant share chỉ≈6,6/4,9/3,3% kills ở các rows, không bằng10% respawn probability. Solo lifetime20/45/90s cho≈35,2/28,3/21,0 encounters/h; không phát hành encounter rate như số chắc chắn. Nếu share kill thật10%, Gold/EXP +20%, Stones +115%, gear +61,6%, material +40% ở cùng total kills; cap không tự chứng minh economy an toàn. Stone boost≈38–76% trong model là risk cần tune/test SCOPE-01/LOOT-01; giữ test10%+stone1, so lower rate/reward nếu flow vượt sink, không bỏ mechanic. Aggregate 4-player throughput không performance acceptance.

**Price rationale:** shop base Pants≈AverageGold tại Lv1/5/10 × kill budget10/12/16, round về10 →70/200/470. Slot weights Weapon1,5 / Armor1,25 / Pants1 / Boots-Ring-Necklace0,75; buy round về10, sell floor25%. Vì thế Common một slot≈7,1–15,7 /8,8–17,6 /11,9–24,1 normal kills theo tier; full set420/1.200/2.820≈60,0/70,6/95,6 kills. Common gear drop marginal4% (một exact slot≈150 kills) làm shop là phương án lấp thiếu chắc chắn, không thay săn rarity. Purchase+sell không lời; enhancement fail/success không tăng sellValue nên không có upgrade-vendor refund loop.

Drop-only Common sellValue Xích Nham/Huyền Tích từ AverageGold tại Lv15/20 × sell kill budget6/8 × slot weights; không fake buyPrice. Vendor rarity×1/1,5/2/3 cho economy, khác stat rarity×1/1,08/1,16/1,25. Mean Common sellValue Lv15=252, Lv20=436; kỳ vọng gear cash-out lấy mean sau floor giá từng slot/rarity: Normal≈14,36/24,85 Vàng/kill. Không nhân rarity lên mean rồi mới floor. Material adds≈2,70/3,60; Potion/Stone nếu bán hết adds4,80/16. Tổng Gold+sell-all≈79,86/103,75 vs base 42/54,50: **sell-all upper**, không inventory realization/gear giữ/stone tiêu; không cộng vừa dùng vừa bán cùng item. Linh Biến Lv15 sell-all≈486,82/kill, nhưng3×HP/lifetime và cap1 hạn chế flow. Trong solo event model sell-all tăng≈33,5% ở cùng kills; rate10% chưa final. Material normal tạo≈6–11% extra gold ở representative map levels, meaningful nhưng không là sink chính.

**Run-back:** đo Village→từng farm map và Boss death→rejoin, không chỉ traversal từng map. Theo traversal target cộng tuyến liên tiếp (chưa Village/portal/né quái), bounds tham khảo Đồng25–35s; Trúc60–90s; Bạch95–145s; Xích130–200s; Huyền Tích175–260s. Late run-back có thể dài hơn Boss TTK90–150s; không giả định người về làng sẽ kịp reward. Đo ở Week 1/5/6; nếu quá punitive ưu tiên tune main-route/shortcut terrain và traversal trước, không tự thêm fast travel/checkpoint P0. Không xem summed target là playable travel time.

<a id="review-decisions"></a>

# 4. Quyết định đã chốt

Luật đã chốt nằm trong GDD, không giữ bảng audit lịch sử song song. Tra [design lock](1_HUYEN_LO_GDD.md#gdd-0), [scope P0/P1/P2 và DROP](1_HUYEN_LO_GDD.md#gdd-12); implementation contract nằm trong Technical. Lịch sử sửa và lý do thay đổi tra Git.

**BOSS-01 — CLOSED (scope correction):** Cuồng Mạch HP≤30% thuộc P0, cadence×0,8 TEST/TUNABLE; count/overlap/scheduler vẫn BOSS-02. Không đồng nhất Linh Giáp/Vỡ Thế P1. SCOPE-01 chuyển từ candidate P1 sang P0 rate/economy PLAYTEST.

**NAR-01 — CLOSED:** Đã tinh chỉnh Narrative theo hướng mở, gợi cảm giác tò mò và gỡ mâu thuẫn bối cảnh Xích Nham mà không tăng scope. Nội dung hiện hành tại [GDD §2](1_HUYEN_LO_GDD.md#gdd-2) và [§6](1_HUYEN_LO_GDD.md#gdd-6); quyết định này không đóng BOSS-03 về availability/lifecycle.

<a id="open-decisions"></a>

# 5. Open Decisions

Bảng dưới chỉ giữ việc cần chốt hoặc playtest cho P0 và gate chọn scope P1. Quyết định đã đóng ở §4; candidate P1 ở §6. Deadline là gate tương lai, không ngày đã hoàn tất. OPEN/PLAYTEST không đổi baseline GDD; CRITICAL phải chốt trước milestone phụ thuộc, không chặn spike nền tảng nếu chưa phụ thuộc vào policy đó.

| ID | Chủ đề | Cần chốt | Baseline hiện tại | Priority | Deadline | Status |
| --- | --- | --- | --- | --- | --- | --- |
| BAL-01 | INT | 0,25/0,30/0,35% đủ value? | 0,25%; S03/S04 | HIGH | During Week 2–3; retest Week 5 | PLAYTEST |
| BAL-02 | MP sustain | Intended deficit và Potion dependency? | Giữ Food/MP costs; S04 | CRITICAL | During Week 2–3 trước sustain acceptance | PLAYTEST |
| QUEST-01 | Q9 UX | Hiện/skip/revisit branch ra sao? | Optional; Q10 không chặn, reward chỉ PvP | HIGH | Before Week 4 quest framework; UX Week 7 | OPEN |
| PHY-01 | Collider | Kích thước final và vertical hitbox? | 0,60–0,65×1,45u; originY+0,8 | HIGH | During Week 1–2; layout retest Week 5 | PLAYTEST |
| CC-01 | Freeze | 25% và refresh/stack nhiều caster? | Normal 1,2s / Linh Biến 0,6s; Boss/PvP immune | MEDIUM | During Week 5 CC/multi-caster pass | PLAYTEST |
| SCOPE-01 | Linh Biến flow | Chance/reward/cap1 có phù hợp farm nhiều người và sinks? | 10% TEST; P0 modifier và Q8 deterministic; S16 | HIGH | During Week 5 world/economy; Q8 verify Week 4 | PLAYTEST |
| SCOPE-05 | Thứ tự P1 | DPS meter/QoL/transfer/network xếp sao? | Gate P0 trước, chưa thứ tự cuối | HIGH | During Week 4 scope checkpoint; chọn sau P0 gate | OPEN |
| BAL-03 | AGI speed | Giữ 0,05%/điểm? | ACC/EVA chính; speed TUNABLE | MEDIUM | During Week 1–2; balance retest Week 5 | PLAYTEST |
| GEAR-01 | Tinh Hoa/rarity | Exact bonus amounts và rarity interaction? | Concept slot GDD; S10/S13 probe chưa khóa | HIGH | Before Week 4 gear data authoring | PLAYTEST |
| COOP-01 | Co-op policies | Quest kill assist/radius/participation; EXP split/bonus; normal loot ownership? | S14 tách ba policy; Boss5% per character, không party system | CRITICAL | Before Week 3 loot recipients/EXP; assist before Week 4 quest; verify Week 6 | OPEN |
| CONS-01 | Potion/Food | Tier selection H/M, F refresh/overwrite, CD groups, death/logout? | S14; highest baseline, Food10m/tick2s mới thay cũ | HIGH | Before Week 3 consumable implementation; restore Week 6 | OPEN |
| SAVE-01 | Restore state | HP/MP/dead/Food và restart map restore? | Host JSON; không tự heal/regen offline | HIGH | During Week 1 save spike; freeze restore before Week 6 acceptance | OPEN |
| PVP-01 | Match edge | Timeout hòa, disconnect và restore combat state? | Best1/120s/%HP thắng | HIGH | Before Week 7 PvP implementation | OPEN |
| QUEST-02 | Counts/evidence | Counts Q4/Q8/Q10, virtual/physical evidence policy/rates, catch-up BaseEXP? | Supply Q5/Q6/Q7 guaranteed và full-bag safety đã rõ GDD; representation còn mở | HIGH | Before Week 4 Q1–Q8; Q10/Q11 counts before Week 6 | OPEN |
| BOSS-02 | Boss stat/scheduler | DEF/ACC/EVA, số vùng/overlap/scheduler đá rơi? | HP 32k/ATK 140 baseline; S07 giả định DEF 25/EVA 60 | HIGH | Before Week 6 Boss implementation | OPEN |
| ART-01 | Projectile feel | Arrow speed, telegraph/aim và hybrid timeout? | Generic mob speed V5; không homing mới | MEDIUM | During Week 2 combat slice; Ong/Hybrid retest Week 5 | PLAYTEST |
| LOOT-01 | Loot/economy tuning | Normal/Linh Biến/Boss rates, vendor budgets và sinks phù hợp? | Semantics/Gold/map-tier/exclusive roll đã chốt GDD; S08/S16 là test | HIGH | Before Week 3 item prices; retune Week 5 flow / Week 6 Boss | PLAYTEST |
| BOSS-03 | Story access/lifecycle | S12 A/B/C/D, Huyền Tích17/18 và arena/spoiler/concurrency/reconnect/first-clear loot? | Một global Boss15m; B/C lifecycle recommendation chưa lock | HIGH | Before Week 6 Boss/story lifecycle implementation | OPEN |
| QUEST-03 | Quest EXP final | Profile8/10/12,5/15%, Q6 có EXP hay giữ 0? | Target8–15%; S11 ưu tiên thử10%, chưa đổi reward data | HIGH | During Week 4 provisional reward authoring; final Week 6 journey | PLAYTEST |
| TECH-01 | Profile binding | ID→connection và duplicate selection policy? | Host load ID; một active writer/character | HIGH | During Week 1 network/profile spike | OPEN |

<a id="research-ideas"></a>

# 6. Candidate P1 và provenance

Các ID dưới đây được đưa ra khỏi bảng việc cần chốt P0, không phải đã duyệt triển khai. Chọn hoặc bỏ tại SCOPE-05 sau gate P0; scope hiện hành vẫn theo [GDD §12](1_HUYEN_LO_GDD.md#gdd-12). Không lặp danh sách ACCEPT/REJECT đã trở thành design lock hoặc DROP.

| ID | Candidate | Input proposal chưa khóa |
| --- | --- | --- |
| SCOPE-02 | Upgrade Transfer | Same slot/higher tier, consume đồ cũ, New=max(0, Old−1), 500 Vàng + 2 đá |
| SCOPE-03 | Buff Lv10 | R, 10s/CD40s/MP15; Chiến Ý +15% ATK/+5% speed hoặc Ưng Nhãn Cường Hóa +10 điểm % Crit/+15% range |
| SCOPE-04 | Linh Giáp | 1.000/Groggy chỉ proposal; cần kiểm CC/workload nếu chọn |

<a id="legacy-provenance"></a>

**Legacy provenance:** NSO đã được khai thác như reference, không là authority hiện hành. Trace còn lại ở bảng dưới từ source server trong `research/SRC NSOACE FIX/` và Git, không phụ thuộc tài liệu research tạm. Audit trước đã kiểm các method dưới đây, không suy ra reference có crash-atomic save hoặc balance phù hợp Huyền Lộ. Pattern đã chấp nhận được mô tả trực tiếp trong GDD/Technical.

| Evidence trace | Giá trị giữ lại |
| --- | --- |
| `Char.initMenu/finishTask`, level tracker | NPC turn-in, bag preflight, level feedback; contract hiện hành độc lập |
| Equip/use/allocate callbacks; `AbilityFromEquip` | Tutorial theo hành động thật, +4 bonus; không chứng minh stat amounts |
| InoshishiCave/Gymnasium creation | Story encounter lifecycle riêng: evidence BOSS-03, chưa lock implementation |
| `Mob.dead` same quest/step; weighted regular/separate quest roll | COOP-01 assist và LOOT-01 evidence; EXP/ownership/rates chưa copy |
| Part/map/cleanup records | Modular visual/one-way/cleanup; scope và asset contract theo GDD |
