# RPG Online: Huyền Lộ
## Game Design Document

**Current Design Version:** V5.2.0

**Status:** PRE-IMPLEMENTATION DESIGN  
**Last Reviewed:** 2026-09-29

<a id="gdd-0"></a>

# 0. Trạng thái tài liệu và Design Lock

GDD giữ luật game; [Analysis](3_HUYEN_LO_DESIGN_ANALYSIS.md) giữ evidence/quyết định mở; [Technical](2_HUYEN_LO_TECHNICAL.md) giữ implementation contract.

User lock và review đã thống nhất được ghi vào GDD hiện hành; Analysis/Technical không tự đổi gameplay. Deterministic defect đã xác minh phải sửa đồng bộ; proposal và reference không tự thành luật.

| Phạm vi khóa | Luật |
| --- | --- |
| Nhân vật | Cap Lv20; Tân Lữ nhập môn Lv1–4; từ Lv5 được chọn một trong hai class; một nhân vật nam |
| Build | Bốn thuộc tính, 95 điểm ở Lv20, không branch cap, không Skill Rank |
| Combat | AoE/Multi-target và SpawnGroup core; Freeze hard CC chỉ normal/Linh Biến PvE |
| Nội dung | 6 normal archetype, Linh Biến modifier P0, 1 World Boss; 5 farm maps + 3 support zones |
| RPG | Food regen chính, không passive regen; Death có consequence; 6 gear slots; +0→+5 |
| Online | Host-first; Personal Boss Loot; overhead Map Chat; MapId P0 |
| Asset/save | Một male modular rig, 64×64, PPU 32, 26 frames; JSON P0, MariaDB P1 |

**LOCKED:** luật khóa; **BASELINE:** số hiện dùng; **TUNABLE:** cần playtest. P0 bắt buộc; P1 sau core; P2 polish; DROP ngoài MVP. Research chưa chốt không thành requirement.

> **Đọc sâu:** [Design Analysis — quyết định mở](3_HUYEN_LO_DESIGN_ANALYSIS.md#open-decisions)

---

<a id="gdd-1"></a>

# 1. Tầm nhìn game và Core Loop

**Elevator Pitch:** RPG hành động 2D online ngang trên PC/Unity. Tân Lữ khám phá linh mạch, chọn Kiếm/Cung, tự phân bốn thuộc tính, gom quái đánh lan, nâng gear, săn Linh Biến/Boss và tỷ thí.

| Pillar | Người chơi cảm nhận | Dấu hiệu đạt |
| --- | --- | --- |
| Farm có nhịp | Gom quái rồi cleave/pierce/spread/explosion | Skill Lv5 tác động tối đa 3 mục tiêu |
| Build tự do | Đổi phân phối điểm để thử cách chơi | All-in không bị khóa progression; Tẩy Mạch sửa build |
| Progression hữu hình | Gear mới đổi cả stat và hình | Weapon/Armor/Pants đổi sprite |
| Hai class khác nhau | Kiếm áp sát; Cung giữ khoảng cách | Range, hit shape và control khác rõ |
| Online có ý nghĩa | Co-op farm/Boss, chat, challenge | Client kết nối tham gia gameplay thật |
| Scope hoàn chỉnh | Ít nội dung nhưng nối thành hành trình | Lv1→20, ba chương, chính tuyến và vòng chơi sau truyện |

**Core Loop:** Quest chỉ đường → Food/Potion → vùng mở → gom cụm/đánh lan → EXP/loot → build/gear → level gate → NPC. Linh Biến/Boss/PvP xen giữa chặng farm; Food/túi đồ tạo nhịp về làng.

Target Lv20: 2,5–4h gồm travel/quest/shop/run-back, chưa được playtest. **P0 acceptance: tối thiểu 2 concurrent players** (Host + ít nhất 1 Client); game online nhiều người, không đặt MaxPlayers=2. Capacity 3–4+ concurrent players phải benchmark performance/network trước khi công bố.

---

<a id="gdd-2"></a>

# 2. Thế giới và cốt truyện

Vân Khê nằm trên những Mạch Ấn ngầm, nơi linh khí nuôi rừng núi và giữ phế tích yên giấc. Gần đây, gió núi mang mùi tanh, nấm mọc khác thường, sói bỏ bãi cũ. Tân Lữ là người dự tuyển, lần theo những dấu nhỏ ấy trong lúc học cách tự giữ mình; không có lời tiên tri hay danh phận cứu thế. Lễ Nhập Lộ tại Lv5 gắn lựa chọn kiếm/cung với việc chính thức bước vào đường tu luyện, bằng lời NPC và thao tác Q6, không cinematic hay quest mới.

| Trụ cột thế giới | Điều được hé lộ |
| --- | --- |
| Mạch Ấn và linh khí | Mạch Ấn bị can thiệp làm dòng linh khí lệch hướng, sinh trọc khí khiến sinh vật hung dữ. Ghi chú quest, tên vật phẩm và mô tả quái nối từng dấu vết. |
| Đoạt Mạch Đạo Tặc | Chúng đục phá Mạch Ấn để lấy linh thạch: lợi trước mắt của con người làm rối trật tự tự nhiên. Biết có đạo tặc chưa đủ kết luận nguồn gây nhiễu. |
| Cự Thú và Dư Ảnh | Huyền Nham Cự Thú là sinh linh thủ hộ cổ xưa bị trọc khí ăn mòn. Hạ nó giúp giải thoát Thủ Vệ; Dư Ảnh là tàn niệm linh lực còn đọng nơi cấm địa. |

Lời NPC ngắn, mộc mạc; không diễn giải hết bí ẩn. Lâm Bá kiệm lời, ấm áp, nhắc đường về; Bách Luyện cộc nhưng trọng người bền chí; Yên Thảo nghiêm về khí huyết và giữ mạng; Tạ Minh thâm trầm, chỉ dẫn đại cục; Hạo Vũ sảng khoái, lấy tỷ thí làm lời chào. Những sắc thái này dùng text và nội dung hiện có, không thêm quest/NPC/quái/Art.

| Chương | Dải cấp | Sắc thái và diễn tiến | Checkpoint tổng kết |
| --- | --- | --- | --- |
| I — Dấu Nứt Vân Khê | 1–7 | Nhập môn & sinh tồn: làng còn yên, điềm lạ thoáng qua; tự cầm kiếm, dùng thuốc, rèn món đầu tiên | Q7 completed và Lv≥7 |
| II — Theo Dấu Huyền Lộ | 8–17 | Dấn thân & khám phá: lần theo trọc khí; Lv12 tu luyện ngoại vi Xích Nham, Lv15 điều tra sâu, Lv17 phục hồi Huyền Môn | Q11 completed và Lv≥17 |
| III — Huyền Tích Thức Tỉnh | 18–20 khuyến nghị | Thanh tẩy & vấn đạo: phế tích trang nghiêm, Thủ Vệ bị cuồng hóa; phong ấn ổn định sau trận chiến | Q12 completed và Lv20 |

**Main Story Complete — HOÀN THÀNH CHÍNH TUYẾN:** Q12 khép lại chính tuyến/Chương III. Summary: **CHƯƠNG III HOÀN THÀNH / CHÍNH TUYẾN ĐÃ HOÀN THÀNH** — Tạ Minh: “Tai ương tạm lắng. Đường tu luyện còn dài.” Tiếp tục farm Huyền Tích, săn Rare/Epic/+5/Linh Biến/Dư Ảnh, PvP Challenge, Tẩy Mạch/thử build, Journey Score và P1 nếu được triển khai.

Q12 **per character**: chưa complete hiển thị **Huyền Nham Cự Thú**, đã complete **Dư Ảnh Huyền Nham**, kể cả tracker/banner. Hai tên dùng một entity/sprite/AI/drop, không world story flag.

> **Đọc sâu:** [Design Analysis — review narrative](3_HUYEN_LO_DESIGN_ANALYSIS.md#review-decisions)

---

<a id="gdd-3"></a>

# 3. Progression và thuộc tính

Lv20 cap; 5 điểm/level-up = **95 điểm**. Lv1 chưa có điểm; Lv2–4 auto +2STR/+2VIT/+1INT, tổng 15. Lv5 thêm 5 và hoàn 15 thành **20 unspent** một lần; chọn class Q6 rồi tự cộng. Chưa class không dùng skill/vũ khí class; kể cả Lv5+ vẫn dùng normal Tân Lữ/Mộc Kiếm để làm Q5. Reset nhập môn xảy ra một lần khi đạt Lv5; nếu trì hoãn Q6 tới Lv6+, giữ mọi điểm level-up thêm, tổng spent+unspent=5×(L−1), không set lại20. Q3 trao Mộc Kiếm; Lv1–2 học NPC/movement, chưa bị yêu cầu combat.

| Lv | EXP lên cấp | Tích lũy | Lv | EXP lên cấp | Tích lũy |
| ---: | ---: | ---: | ---: | ---: | ---: |
| 1 | 100 | 0 | 11 | 2.200 | 6.900 |
| 2 | 150 | 100 | 12 | 2.700 | 9.100 |
| 3 | 220 | 250 | 13 | 3.300 | 11.800 |
| 4 | 320 | 470 | 14 | 4.000 | 15.100 |
| 5 | 450 | 790 | 15 | 4.800 | 19.100 |
| 6 | 620 | 1.240 | 16 | 5.700 | 23.900 |
| 7 | 830 | 1.860 | 17 | 6.700 | 29.600 |
| 8 | 1.080 | 2.690 | 18 | 7.800 | 36.300 |
| 9 | 1.380 | 3.770 | 19 | 9.000 | 44.100 |
| 10 | 1.750 | 5.150 | 20 | MAX | **53.100** |

`MobBaseEXP(L) = round(10 + 2.5*L + 0.12*L*L)`. Sau onboarding farm là nguồn EXP/Vàng/đá/gear/material chính; quest dẫn đường/dạy mechanic/kể chuyện/mở nội dung. Target Q5–Q11 **8–15% bar tại RequiredLevel**, retune QUEST-03; số kill cuối cần PLAYTEST/TUNABLE; mô hình normal-equivalent hiện tại ~702–726 chưa gồm dynamic Linh Biến. `DemoExpMultiplier = 10` chỉ cho demo.

| Nhánh | Mỗi điểm | Baseline |
| --- | --- | --- |
| Sinh Lực — VIT | +8 HP, +0,10 DEF | Chịu đòn |
| Linh Lực — INT | +5 MP, +0,25% Skill Damage | Giữ 0,25%; test 0,30/0,35 ở Analysis |
| Công Lực — STR | +0,70 ATK | Tăng normal và skill |
| Thân Pháp — AGI | +6 ACC, +6 EVA, +0,05% MoveSpeed | MoveSpeed TUNABLE, lợi ích chính là ACC/EVA |

Base theo L: `HP = 120 + 10*(L-1)`; `MP = 60 + 4*(L-1)`; `ATK = 12 + 1.2*(L-1)`; `DEF = 5 + 0.6*(L-1)`. Cộng gear và điểm trước khi nhân modifier class. `ACC = 60 + 4*(L-1) + 6*AGI + GearACC`; `EVA = 20 + 2*(L-1) + 6*AGI + GearEVA`.

`EvadeChance = 0.02 + 0.43 * EVADefender / (EVADefender + 2.5 * ACCAttacker)`. Hàm tiệm cận 45%, không hard cap điểm AGI. `SkillDamageBonus = 1 + INT*0.0025`; chỉ skill dùng bonus này. `MoveSpeedMultiplier = 1 + AGI*0.0005`.

All-in hợp lệ nghĩa là có thể chơi và clear normal content mà không softlock, có tradeoff damage/sustain/mobility; không cam kết mọi build DPS ngang nhau. BAL-01/02/03 kiểm bằng playable rotation, reset luôn có qua Tẩy Mạch.

**Tẩy Mạch Phù:** 1.200 Vàng tại Tạ Minh, stock vô hạn; trả mọi điểm đã cộng về unspent, giữ class/level/gear/quest/skill. Reset nhập môn Lv5 miễn phí, chỉ một lần; Q6 xác nhận kết quả, không reset/cấp điểm thêm.

> **Đọc sâu:** [Design Analysis — balance](3_HUYEN_LO_DESIGN_ANALYSIS.md#balance-baselines)

---

<a id="gdd-4"></a>

# 4. Class, combat và kỹ năng

| Tiêu chí | Tân Lữ trước chọn class | Kiếm Sĩ Lv5+ | Xạ Thủ Lv5+ |
| --- | --- | --- | --- |
| Normal | 1,00× / **0,80s** / 1 target | 1,00× / **0,72s** / 1 target | 0,95× / **0,80s** / 1 target |
| Vũ khí/range | Mộc Kiếm +8 ATK; melee ~1,2u | Sword; melee ~1,2u | Bow; normal 6,5u |
| Class modifier | Chưa áp class | HP×1,10; DEF×1,10; ATK×1,05; Crit 5% | MP×1,10; ATK×1,05; Crit 8% |
| Identity | Học movement/normal | Áp sát, cleave, direct pressure, Hỏa VFX | Range, pierce, Slow/Freeze PvE |

Hỏa/Băng là class identity, không Element System lớn. Kiếm không có Burn P0 là chủ ý; Burn nằm P1. Mỗi class có 3 active và 1 passive, mở theo level, không Skill Rank.

| Class | Skill | Lv | Shape/range | Power theo target | Max | MP | CD |
| --- | --- | ---: | --- | --- | ---: | ---: | ---: |
| Kiếm | Phong Trảm | 5 | Arc 120°, 1,7u | **1,25× mỗi target** | 3 | 8 | 1,4s |
| Kiếm | Kiếm Tâm | 8 | Passive | Crit +5 điểm % | — | 0 | — |
| Kiếm | Liên Kích | 13 | Box ~1,5×1,5u, snapshot | 3×0,50× mỗi target | 3 | 14 | 4,0s |
| Kiếm | Kiếm Khí | 17 | Line 5,5u, width ~0,6u | **1,80/1,65/1,50/1,35/1,20×** | 5 | 22 | 7,0s |
| Cung | Liên Tiễn | 5 | SmartSpread, 6,5u | 0,50/0,45/0,40× theo tên | 3 | 10 | 2,0s |
| Cung | Ưng Nhãn | 8 | Passive | Crit +5 điểm %; range +10% | — | 0 | — |
| Cung | Xuyên Tiễn | 13 | Line 7u | **1,60/1,40/1,20/1,00×** | 4 | 16 | 4,5s |
| Cung | Hàn Tiễn | 17 | Primary 6,5u + explosion radius 2u | 1,40× chính; 0,80× phụ | 1+4 | 20 | 7,0s |

Liên Tiễn≥3 target ưu tiên khác nhau; một target tổng 1,35×. Liên Kích snapshot, không reacquire; bỏ target mất hợp lệ. Pierce theo đường đạn. Explosion loại primary, không double-hit.

**Damage:** normal `Raw = FinalATK×NormalPower`; skill `Raw = FinalATK×SkillPower×SkillDamageBonus`. Host validate → roll Evade → Crit → DEF → random → HP/result. Nếu né, damage 0 và hiện NÉ. Nếu trúng: `Damage = max(1, round(Raw×100/(100+TargetDEF)×Random(0.95, 1.05)×CritMultiplier))`. CritMultiplier cố định **1,5**; CritChance cộng class, passive và gear.

| Target Hàn Tiễn | Slow | Freeze trên hit hợp lệ |
| --- | --- | --- |
| Normal | 30% / 2s | 25% chance / 1,2s |
| Linh Biến | 30% / 2s | 25% chance / 0,6s |
| Boss | 10% / 1,5s | Miễn nhiễm |
| PvP player | 20% / 1,5s | Không áp dụng |

Freeze 25% cần playtest; khóa movement/attack AI, hủy pending windup/hit/projectile spawn của mob, không thu hồi projectile đã bay hoặc hoàn cooldown; dùng overlay chung. Player xuyên quái, không contact damage. Monster–Monster không physical push collider hoặc rigidbody shoving; chỉ light separation steering có giới hạn để tránh stack một điểm, không vượt leash hay làm pathing rung. Farm không knockback; flash/hit-stop không hard stun.

Kiếm ưu tiên phía trước; trống thì auto-facing target sau lưng≤1,2u. Cung cone±25° phía trước, không auto quay sau. Không target vẫn cast theo facing. Melee originY+0,8u; vertical reach TUNABLE.

> **Đọc sâu:** [Design Analysis — combat](3_HUYEN_LO_DESIGN_ANALYSIS.md#combat-analysis)

---

<a id="gdd-5"></a>

# 5. World, mob, Linh Biến và Boss

**World route:** Vân Khê ↔ Đồng Sương ↔ Trúc Ảnh ↔ Bạch Vân ↔ Xích Nham ↔ Huyền Tích. Vân Khê nối Học Viện và Lôi Đài. Q11 mở Huyền Môn từ Xích Nham. Không fast travel ra bãi; Bùa Hồi Thành P1 chỉ đưa về làng.

| Khu | Cấp | SpawnGroups / slots — TEST/TUNABLE | Layout / traversal không combat — TEST TARGET |
| --- | ---: | --- | --- |
| Đồng Sương | 1–4 | 4 / 12–16 | Thoáng, tutorial 1–2 tầng; Nấm Lv1–2, Sói Lv3–4; 25–35s |
| Trúc Ảnh Lâm | 5–8 | 5 / 15–20 | Cầu tre, 2–3 lanes/vertical pockets; Sói Lv5–8 (một slot Lv8 cho Q8), Ong Lv7–8; 35–55s |
| Bạch Vân Thác | 8–12 | 5 / 15–20 | Ba terraces, waterfall landmark, loop/alternate climb dễ; Ong Lv8–9, Đạo Tặc Lv10–12; 35–55s |
| Xích Nham Cốc | 12–17 | 6 / 18–24 | Canyon rộng nhất mid/late, hai nhánh rồi hội lại; Đạo Tặc Lv13, Thạch Linh Lv14–17; 35–55s |
| Huyền Tích outer | 18–20 khuyến nghị; gate Lv17 | 5 / 15–20 | Outer ruins, ancient landmark hướng tới Boss; Thạch Linh Lv17, Cổ Vệ Lv18–20; 45–60s; arena access BOSS-03 |
| Vân Khê / Học Viện / Lôi Đài | Support | Không bãi farm | NPC, class/dummy, challenge |

| Vùng | Gate vào vùng — BASELINE | Ý nghĩa |
| --- | --- | --- |
| Đồng Sương | Onboarding mở từ đầu | Q4 dạy sinh tồn; chưa có story gate |
| Trúc Ảnh | Q6 COMPLETED + Lv5 | Chọn class rồi thử đánh cụm |
| Bạch Vân | Q8 COMPLETED + Lv8 | Bằng chứng Sói Linh Biến mở tuyến điều tra |
| Xích Nham | Q8 COMPLETED + Lv12 | Farm ngoại vi trước Q10; chưa xác nhận nguồn trọc khí, không phụ thuộc Q9 |
| Huyền Tích | Q11 COMPLETED | Vào farm ngoại vi từ Lv17, khuyến nghị Lv18; Q12 Lv20. Arena/spoiler còn BOSS-03 |

Level là power gate, quest flag là story gate; mở bãi farm không tự nhận quest. Village/Academy mở onboarding; Arena qua Challenge. Gate levels BASELINE, playtest cùng QUEST-03.

Tổng **5 farm maps + 3 support zones = 8 logical map roots**; **25 SpawnGroups / 75–100 normal slots là TEST/TUNABLE**, không release capacity. Mỗi cụm 3–4 slots; Linh Biến thay một slot, không tăng mob count. Within-group spacing 0,8–1,5u; aggro 5u, leash 8u. Hit alert chỉ cùng group, không truyền qua group khác; attack offset random 0–0,35s tránh cùng frame. Vượt leash → Return, tạm miễn damage, về spawn rồi full HP.

**Layout contract:** Safe Entrance → Pocket A → main route → vertical/alternate path → Pocket B/C → landmark/story point → later pockets → Exit. Main route dễ đọc, không maze/puzzle khó; portal có safe strip ~6–8u và không spawn mob sát cửa. Group centers ~18–20u hoặc terrain/platform tách aggro tương đương, không kéo hai group thành blob. Không moving platform/hazard P0. Freeze dimensions sau movement/jump/camera spike, không chốt width bằng world units trước đo traversal.

Farm acceptance: 2/3/4 concurrent players cùng level-map; đo group sống, wait/respawn, clear speed, contention, Linh Biến visibility, CPU/network và aggro chaining. Tune groups/spacing/rate theo kết quả, không hard-code bốn players hoặc thêm Party/Channel/instance farm.

| Normal archetype | AI | Move u/s | Melee/ranged u | Interval | Projectile |
| --- | --- | ---: | --- | ---: | --- |
| Nấm Linh | Melee only | 1,2 | 0,8 / — | 1,8s | — |
| Sói Sương | Melee only; chase nhanh | 2,4 | 1,0 / — | 1,3s | — |
| Ong Giáp | Ranged/Flying | 2,0 | — / 5 | 1,6s | Generic, speed 5,5u/s |
| Đoạt Mạch Đạo Tặc | Hybrid | 2,2 | 1,2 / 5 | 1,5s | Generic, speed 5u/s |
| Xích Thạch Linh | Hybrid | 1,4 | 1,1 / 4,5 | 2,0s | Generic, speed 4u/s |
| Cổ Môn Vệ Binh | Hybrid | 1,8 | 1,4 / 6 | 1,6s | Generic, speed 6u/s |

Hybrid reachable→Chase/Melee; ranged khi platform không reach/vertical/chase blocked-timeout, không chỉ vì distance>melee. Ong FlyingBox ~6×3u là phạm vi roam, không hover height cố định. Ong phải approach vào vertical melee band khi engage, không treo mãi ngoài tầm Kiếm; hover/approach timings PHY-01/ART-01. Một Linh Đạn generic đổi scale/tint/speed/trail; không projectile/animation riêng từng loài.

Stat normal tại level L (`round(x) = floor(x + 0.5)` cho x≥0, kể cả EXP/final Damage): `HP = round(80 + 12*L + 1.2*L*L)`; `DEF = round(2 + 0.8*L)`; `ATK = round(6 + 1.6*L + 0.08*L*L)`; `ACC = 60 + 4*L`; `EVA = 20 + 2*L`; `NormalEXP = MobBaseEXP(L)`; `GoldMin = 3 + 2*L`; `GoldMax = 6 + 3*L` (integer uniform inclusive). Một formula chung cho sáu archetypes; definition multipliers mặc định 1.0, không sáu hệ stat riêng. Normal/variant slot respawn **12s BASELINE/TUNABLE**, test 8/12/15s; không timer variant riêng.

**Mob attack contract:** đi qua aggro radius vẫn bị acquire/chase, body overlap không gây damage. Melee: Acquire → Chase → attack range → Face → Windup/lock facing → HitMoment/front hitbox → Recovery. Target chạy xuyên ra sau/nhảy ra khỏi vertical range/rời hitbox trước HitMoment thì MISS; không guaranteed damage vì animation đã start, không quay 180° giữa swing. Ranged/Hybrid: Acquire → Aim/Windup → projectileSpawnMoment → Host projectile → collision → damage; AnimationEvent chỉ visual. Mob normal attack power 1.0, CritChance0 P0; formula Damage chung, exact hitbox/windup tại PHY-01/ART-01.

**Linh Biến P0 — modifier trên normal slot:** lúc slot respawn, nếu map chưa có active Linh Biến/reservation Q8 thì roll `LinhBienChance = 0.10` (**10% TEST/TUNABLE**); `MaxActiveLinhBienPerMapId = 1`. Max1 áp trên toàn MapId, không per group/player. Chết xong dùng normal slot lifecycle; chỉ lần spawn sau mới roll, không reroll do Return/root hide/reconnect; initial population dùng cùng spawn arbitration, không bulk reroll map rỗng.

| Modifier | TEST/TUNABLE baseline |
| --- | --- |
| Stat/reward | HP×3; ATK×1,3; EXP×3; Gold×3; DEF/ACC/EVA/MoveSpeed giữ base |
| Visual | Exact base sprite/animation/AI/projectile/attack set; scale ~1,20–1,30, aura/name color/HP bar lớn, tint nhẹ optional |
| Count | Chiếm đúng một normal slot, không archetype thứ bảy/anchor/respawn riêng |

**Q8 deterministic promotion:** đúng active step, Host giữ pending entitlement theo character và dùng một Sói Sương slot Lv8 đã có ở Trúc Ảnh. Reservation ngừng random promotion mới và giải quyết theo trạng thái sau, không đợi trúng 10%:

| Trạng thái tại Trúc Ảnh | Host xử lý |
| --- | --- |
| Đã có Sói Sương Linh Biến Lv8 | Bind encounter hiện tại; giữ nguồn Dynamic/ForcedQ8 và reward profile |
| Linh Biến khác idle tại spawn, full HP, không pending action | Demote an toàn rồi promote Sói Lv8 idle/full HP hoặc chờ next respawn |
| Linh Biến khác đang combat, chưa về spawn/full HP hoặc còn pending action | Chờ death hoặc Return hoàn tất rồi nhường cap; không reset/demote quái giữa fight |
| Cap trống nhưng Sói Lv8 chưa idle tại spawn/full HP hoặc còn pending action | Chờ Return hoàn tất hoặc next respawn của slot; giữ deadline respawn 12s |
| Cap trống và Sói Lv8 idle tại spawn, full HP, không pending action | Promote chính slot đó, không thêm entity |

HUD chỉ target đang giữ reservation rồi Sói Q8. Các requests dùng chung encounter/slot và waiting playerId set; không spawn hai variant. Death/reconnect giữ request/progress, không nhân entity; nếu kill chưa cho quest credit theo COOP-01 thì retry deterministic. Hết pending Q8 ở map thì random rule tiếp tục; variant đang sống đi hết lifecycle, không despawn giữa fight.

Forced promotion không thành farm boost vô hạn: normal/Linh Biến loot eligibility/distribution vẫn theo COOP-01; **bonus forced Q8 chỉ một lần cho mỗi character có entitlement và eligible**, bằng receipt cùng reward transaction. Effective reward profile bao gồm EXP, Vàng, các loot channel và Journey (+100 Linh Biến hoặc +5 Normal). Các recipient khác hoặc entitlement đã nhận bonus dùng Normal reward profile; không roll cả hai profile. Quest assist và bonus receipt tách nhau: retry lấy quest credit không reset bonus. Dynamic Linh Biến không có giới hạn one-time này; bind một dynamic Sói Lv8 vào Q8 không đổi nguồn/reward profile của nó. Đây là guard cho quest-trigger replay, không thêm Party hoặc private encounter.

**Một World Boss:** Lv20 Huyền Nham Cự Thú, HP **32.000 BASELINE**, ATK **140 BASELINE**; **DEF/ACC/EVA TUNABLE — BOSS-02**, chưa khóa; không EXP ở cap. Arena radius 15u; respawn 15 phút sau death, demo 60s; tracker luôn có, cảnh báo T−60s và banner spawn/death. Release target 90–150s trong benchmark đầu với 2 endgame players; không giới hạn số người tham gia, không tự scale Boss HP theo player count. Balance khi đông hơn cần đo, chưa được chứng minh bằng build.

| Pattern | Power | Telegraph | CD/interval | Shape |
| --- | ---: | ---: | ---: | --- |
| Basic | 1,00× | — | 1,8s | Melee |
| Nham Trảo | 1,20× | 0,5s | 2,5s | Frontal cone |
| Địa Chấn | 1,50× | 1,0s | 6s | Ground AoE, nhảy né |
| Nham Thạch Rơi | 1,80× | 1,2s | 8s | Nhiều vùng cảnh báo trên mặt đất; count/overlap/scheduler tại BOSS-02 |

Threat P0: `Threat[playerId] += ActualHpLost`; target alive có threat cao nhất, retarget khi chết/rời arena hoặc khoảng 1s. Nếu `AlivePlayersInArena == 0` liên tục 10s: reset HP/position, clear Threat **và Contribution**. Dead player không ngăn reset. **Cuồng Mạch P0:** khi HP≤30%, một lần/encounter: tint/glow, roar và local camera shake; baseline `CadenceMultiplier = 0.8` (CD/interval ngắn hơn 20%), không nhân ATK. Giữ windup/telegraph tối thiểu; Nham Thạch Rơi tăng áp lực qua cadence, count/overlap exact còn BOSS-02/TUNABLE. Phase change không reschedule/double-hit action đang chạy; reset encounter xóa phase. Linh Giáp/Vỡ Thế vẫn P1, không thêm skill Boss.

Contribution chỉ cộng HP thật đã trừ, cap overkill và dedup hit; không inflate threshold bằng raw damage khi mục tiêu còn1 HP. Reward: `DamageContribution >= RuntimeBossMaxHP * 0.05`, connected cùng MapId khi Boss chết. Release **1.600**; demoHP**12.800**→threshold **640**. Corpse/phù tại chỗ eligible; về làng/disconnect không. Join muộn/AFK vẫn đủ nếu đạt threshold.

Mỗi eligible player có reward riêng; một exclusive gear roll Rare 40% / Epic 8% / None 52%, giữ marginals; slot/weapon policy theo §7. Gold auto-credit, physical items owner-only. Table tỷ lệ hiện tại: 1.000 Vàng; Tinh Thạch 5–8 (100%); Rare Huyền Tích gear 40%; Epic 8%; Hồi Sinh Phù 30%; Potion III 2–3 (50%); Journey +500. PersonalLootDrop owner-only cả visibility và pickup, tồn tại 90s, không chuyển FFA. Bag full để item trên đất và báo đầy; không có shared chest/Top DPS bonus.

> **Implementation:** [Technical — timer và Boss lifecycle](2_HUYEN_LO_TECHNICAL.md#timers)

---

<a id="gdd-6"></a>

# 6. Quest và tiến trình chương

Q1–Q4 catch-up onboarding, target không grind mười phút đầu. Q5+ EXP hỗ trợ farm, reward chính là gear/utility/story. **EXP Q5–Q11: TUNABLE — QUEST-03**; legacy numeric chỉ tại Analysis S11, không phải reward để triển khai.

**Quest flow:** LOCKED → AVAILABLE (story flag + RequiredLevel) → nhận → IN_PROGRESS → đủ sub-objectives → READY_TO_TURN_IN → chủ động gặp đúng NPC → COMPLETED + reward/story flag/portal unlock → kiểm eligibility quest kế.

READY_TO_TURN_IN không auto-turn-in/teleport; được farm/loot/heal/khám phá tiếp. Đủ story thiếu level vẫn LOCKED: NPC: “Đường phía trước cần thực lực cấp 12; hãy tu luyện rồi quay lại.” HUD: “Nhiệm vụ chính tiếp theo mở tại Lv.12”, không marker AVAILABLE giả.

| Quest / Lv | Setup → Story beat | Sub-objectives | Turn-in | Reward hiện tại | Unlock / Next condition |
| --- | --- | --- | --- | --- | --- |
| Q1 — Người mới đến Vân Khê / 1 | Lâm Bá chỉ những nơi cần nhớ trước khi ra núi: thuốc, lò rèn, chỗ gửi đồ. Gió qua làng thoảng mùi lạ, ông chỉ dặn đừng đi quá xa. | Gặp Lâm Bá, Yên Thảo, Bách Luyện, Mộc An; trở lại báo | Lâm Bá | Catch-up Lv2 + 50 Vàng | Q2: Q1 complete + Lv2 |
| Q2 — Bước chân đầu tiên / 2 | Đường tới Học Viện có bậc đá và lối xuống thấp. Tự đi được cũng phải biết đường trở về. | A/D tới marker; Space vượt bục; S+Space drop-through; portal Vân Khê↔Học Viện; báo cáo | Lâm Bá | Catch-up Lv3 + 75 Vàng | Q3: Q2 complete + Lv3 |
| Q3 — Vũ khí trong tay / 3 | Phong Du trao kiếm gỗ: cầm cho chắc rồi hãy nghĩ đến chuyện hơn thua. Bù nhìn không đuổi theo, đủ để người mới hiểu một nhát kiếm. | Nhận kiếm; Inventory/equip; tới sân luyện; hạ 3 Bù Nhìn Tập Luyện; trở lại | Phong Du | Kiếm cấp ở bước nhận; turn-in catch-up Lv4 + Quần Vải Tân Lữ | Q4: Q3 complete + Lv4 |
| Q4 — Sinh tồn ngoài làng / 4 | Yên Thảo dặn giữ khí huyết trước khi vào rừng. Nấm mọc khác thường; mạng sống quý hơn một chuyến săn. | Mua Food/Potion; F dùng Food; ra bãi/hạ quái tutorial; khi bị thương thử H; về báo (H không chặn turn-in nếu chưa bị thương) | Yên Thảo | Ứng 320 Vàng một lần khi nhận; turn-in catch-up Lv5 | Q5: Q4 complete + Lv5 |
| Q5 — Chiến lợi phẩm đầu tiên / 5 | Bách Luyện không chê món đồ cũ: thứ dùng được thì mặc, thứ thừa thì bán. Có đồng lộ phí mới đi được chuyến sau. | Kill hợp lệ đầu tiên khi objective active bảo đảm Áo Thanh Mộc + Nấm Sương ×1 riêng tutorial; E nhặt; equip áo; bán Nấm Sương; báo lại | Bách Luyện | EXP TUNABLE — QUEST-03; Áo Thanh Mộc chuyển sang tutorial drop một lần, không cấp lại khi trả | Q6: Q5 complete + Lv5 |
| Q6 — Con đường của ta / 5 | Trong Lễ Nhập Lộ, Tạ Minh để người dự tuyển tự chọn kiếm hay cung. Bước vào đường tu luyện cũng là học cách ra đòn và điều hòa linh lực. | Xác nhận reset Lv5 một lần (20 điểm ở Lv5, giữ điểm thêm nếu cấp cao hơn); chọn class; nhận/equip class weapon; cộng điểm; dùng Skill1; cấp Bình Linh Lực I ×1 dành cho bước M; khi MP thiếu sau Skill1, nhấn M và thực sự consume | Tạ Minh | Class/weapon/skill + Potion cấp một lần theo bước; EXP TUNABLE — QUEST-03 (0 là input legacy) | Q7: Q6 complete + Lv7; mở Trúc Ảnh |
| Q7 — Tinh Thạch đầu tiên / 7 | Bách Luyện đặt viên đá cạnh lò: muốn đồ bền phải biết mình bỏ vào bao nhiêu. Món rèn đầu tiên là công sức của chính người cầm nó. | Nhận Nhẫn Thanh Mộc +0; nguồn tutorial dành riêng 1 Tinh Thạch + 100 Vàng; preview; nâng nhẫn +1 (100%); equip/confirm; báo | Bách Luyện | EXP TUNABLE — QUEST-03; nhẫn/đá/100 Vàng cấp theo bước một lần, không cấp lại lúc trả | Q8: Q7 complete + Lv8 |
| Q8 — Bóng sói trong Trúc Ảnh / 8 | Một con Sói trong bầy bị trọc khí làm Linh Biến; dấu Mạch Ấn lệch dòng nối qua Bạch Vân về Xích Nham. Lâm Bá dặn Lv12 chỉ nên luyện công ngoài rìa, chưa đủ bằng chứng để đi sâu. | Điều tra Trúc Ảnh; hạ Sói/tìm dấu bất thường; hạ một Sói Sương Linh Biến Lv8 do Host bảo đảm ở đúng step; báo cáo | Lâm Bá | EXP TUNABLE — QUEST-03 + Giày Thanh Mộc + 2 đá | Mở Bạch Vân; Q9 optional Lv12, Q10 Lv15; cả hai cần Q8 complete |
| Q9 — Khảo Chiến Đồng Môn / 12 | Hạo Vũ cười, mời lấy vài đường võ làm lời chào. Tỷ thí là dịp hiểu đồng môn, không phải điều kiện để bước tiếp. | Invite/accept player khác; hoàn thành tutorial PvP hợp lệ; báo | Hạo Vũ | EXP TUNABLE — QUEST-03 + Dây Bạch Vân, chỉ khi làm PvP | Optional; không có đối thủ PvP vẫn tới Q10; không bot/reward khi skip |
| Q10 — Dấu chân Xích Nham / 15 | Tạ Minh giao truy dấu sâu vào Xích Nham, nơi đạo tặc đã quen mặt từ những chuyến luyện công. Vật chứng mới xác nhận chúng đục Mạch Ấn lấy linh thạch; đem về để tìm cách nối lại dòng khí. | Tới Xích Nham; hạ Đoạt Mạch Đạo Tặc; ghi nhận vật chứng quest riêng khi objective active; mang kết quả về | Tạ Minh | EXP TUNABLE — QUEST-03 + Quần Xích Nham | Q11: Q10 complete + Lv17, tuyệt đối không cần Q9 |
| Q11 — Mở Lối Huyền Môn / 17 | Ba Mảnh Huyền Ấn là phần ấn đã vỡ, đặt đúng chỗ sẽ nối lại dòng linh lực qua Huyền Môn. Dẹp Thạch Linh, kích hoạt cổng rồi trở về để Tạ Minh xác nhận lối vào cấm địa. | Hạ 12 Xích Thạch Linh; thu 3 Mảnh Huyền Ấn tại Xích Nham; đặt/kích hoạt Ấn ở cổng; về xác nhận | Tạ Minh | EXP TUNABLE — QUEST-03 + Rare Xích Nham weapon đúng class | **Q11 COMPLETED mới mở Huyền Tích**; Q12 thêm Lv20 |
| Q12 — Tiếng gọi từ Huyền Tích / 20 | Qua Cổ Vệ, Tân Lữ đối mặt Thủ Vệ cổ xưa bị trọc khí ăn mòn. Giải thoát Cự Thú rồi báo Tạ Minh: linh khí hồi phục, tai ương tạm lắng; tàn niệm Dư Ảnh và đường tu luyện vẫn còn. | Vào Huyền Tích; hạ 6 Cổ Vệ; tới trung tâm; tham gia Boss đủ điều kiện; về báo | Tạ Minh | Không EXP ở cap; 1.000 Vàng + story completion; Boss reward theo contribution, không cấp lần hai khi turn-in | **Main Story Complete**, Chương III complete; tiếp tục vòng chơi Dư Ảnh |

Bù Nhìn HP hữu hạn/hạ được/reset nhanh TUNABLE, 0 EXP/Vàng/loot; không mob farm/animation mới. Q3 kiếm/Q4 tiền/Q6 class-weapon-skill cấp trước một lần; completion reward chỉ khi turn-in. Reset 20 điểm không cấp lại khi reconnect.

Q4 tracker: “Khi bị thương, nhấn H”; HP đầy thì không consume/không tăng progress, H là hướng dẫn có điều kiện và không chặn toàn quest. Không fake damage. Q6 M là thao tác thật, có Potion một lần dành riêng cho bước này; nếu Food đã hồi đầy, dùng lại Skill1 rồi M khi MP thiếu.

Q5/Q7 supply tách khỏi normal loot: Host one-time, lưu pending grant/drop qua reconnect/despawn đến khi nhận, không spawn thêm bản sao. Áo/nhẫn dùng reward sẵn có, chuyển thời điểm trao; không tăng rate normal. Áo Q5 giữ cho bước equip, Nấm mẫu chỉ được bán khi bước sell active; nhẫn Q7 giữ đến enhance/equip. Hạn chế tutorial tự gỡ khi bước tương ứng hoàn tất, không mở hệ thống khóa gear P0. Q7 resource dành riêng cho preview/nâng nhẫn tutorial, không thể bán hoặc tiêu vào việc khác trước bước đó. Q6 Potion cũng giữ cho bước M; khi bước này active, M ưu tiên Bình Linh Lực I tutorial, xong bước quay về tier selection thường. Bag đầy báo chỗ cần dọn và cho retry; không mất staged reward. Counts Q4/Sói Q8/Đạo Tặc Q10 và BaseEXP catch-up còn QUEST-02.

Q8 dấu bất thường/Q10 vật chứng/Q11 ba Mảnh Huyền Ấn là **quest-bound evidence**, tách ID khỏi Trade Material. Hướng ưu tiên là counter/vật chứng ảo có tên/icon tracker, không chiếm bag, chỉ nhận lúc objective active; nếu chọn virtual, Q11 tương tác cổng kiểm counter và lưu activation flag, không đòi mở bag. Representation cuối và nguồn/count còn QUEST-02; đồ farm trước không tính, turn-in thành công dọn evidence, failed turn-in giữ nguyên.

**Farm beats:** Q6→7 gom cụm/gear; Q7→8 chuẩn bị Sói Linh Biến Q8; Q8→12 Bạch Vân; Lv12→15 Xích Nham dù bỏ Q9; Q10→17 đá/nâng đồ; Q11→20 chuẩn bị Boss. Q12 availability BOSS-03, không story instance tự thêm.

Catch-up `RemainingToNextLevel = RequiredExp − CurrentExpInLevel`: chỉ bù thiếu tới mốc onboarding; đã đạt mốc chỉ BaseEXP (QUEST-02). Q11 objectives đều ngoài Huyền Tích, portal vẫn khóa ở READY_TO_TURN_IN; turn-in mới mở nên không unlock deadlock.

| Journey event | Điểm | Journey event | Điểm |
| --- | ---: | --- | ---: |
| Normal kill | +5 | Linh Biến kill | +100 |
| Main quest complete | +150 | PvP win | +200 |
| Boss reward | +500 | Chapter complete | +300 |

Journey phục vụ summary/rubric, không stat/tiền; checkpoint §2. Forced Q8 lấy điểm theo effective reward profile tại §5, không +100 mỗi lần retry. Quest/chapter reward một lần; Journey tiếp tục tích lũy qua hoạt động lặp hợp lệ, không qua replay claim.

> **Đọc sâu:** [Design Analysis — progression và quyết định](3_HUYEN_LO_DESIGN_ANALYSIS.md#quest-progression)

---

<a id="gdd-7"></a>

# 7. Trang bị, cường hóa và kinh tế

Chỉ có **Weapon, Armor, Pants, Boots, Ring, Necklace**. Weapon/Armor/Pants đổi sprite; ba slot còn lại stat/icon only. Sword chỉ Kiếm Sĩ, Bow chỉ Xạ Thủ; Tân Lữ dùng Mộc Kiếm. Mộc Kiếm cấp ở Q3 có tutorial binding, không bán/vứt; gear Tân Lữ loot/shop không có binding này và bán được. Sword/Bow loot giữ class restriction; riêng kiếm Q3 dùng được khi chưa chọn class.

| Tier | Cấp Mặc | Vũ Khí (ATK) | Áo Giáp (Armor) | Quần (Pants) | Giày (Boots) | Nhẫn (Ring) | Dây Chuyền (Necklace) |
| --- | ---: | ---: | --- | --- | --- | --- | --- |
| **Tân Lữ** | Lv 1 | +8 ATK | +20 HP, +2 DEF | +12 HP, +1 DEF | +1 DEF | +0,5% Crit, +3 ACC | +8 MP, +3 EVA |
| **Thanh Mộc** | Lv 5 | +15 ATK | +40 HP, +4 DEF | +25 HP, +2 DEF | +2 DEF, +4 EVA | +1,0% Crit, +5 ACC | +15 MP, +5 EVA |
| **Bạch Vân** | Lv 10 | +24 ATK | +70 HP, +7 DEF | +45 HP, +4 DEF | +3 DEF, +8 EVA | +1,5% Crit, +8 ACC | +30 MP, +8 EVA |
| **Xích Nham** | Lv 15 | +34 ATK | +105 HP, +10 DEF | +70 HP, +6 DEF | +5 DEF, +12 EVA | +2,0% Crit, +12 ACC | +45 MP, +12 EVA |
| **Huyền Tích** | Lv 20 | +44 ATK | +145 HP, +13 DEF | +95 HP, +8 DEF | +7 DEF, +16 EVA | +2,5% Crit, +16 ACC | +60 MP, +16 EVA |

Rarity: Common×1,00; Uncommon×1,08; Rare×1,16; Epic×1,25 áp đúng **primary list theo slot** trước enhance; CritChance template không nhân rarity.

| Slot | Primary nhân rarity | Tinh Hoa +4 — bonus cố định, existing stat |
| --- | --- | --- |
| Weapon | ATK | CritChance |
| Armor | HP + DEF | HP |
| Pants | HP + DEF | DEF |
| Boots | DEF + EVA | EVA |
| Ring | ACC | ACC |
| Necklace | MP + EVA | MP |

**Tinh Hoa BASELINE/TUNABLE:** +0→+3 tăng stat; +4 mở fixed-slot bonus, giữ ở+5; +5 thêm stat/visual mạnh hơn. Không random affix/stat mới. Amounts/rarity interaction tại GEAR-01, chưa cộng vào baseline simulation.

Cường hóa +0→+5 theo bảng dưới; fail giữ cấp hiện tại, vẫn mất Vàng/đá.

| Bước | Success | Vàng/lần | Tinh Thạch/lần |
| --- | ---: | ---: | ---: |
| +0→+1 | 100% | 100 | 1 |
| +1→+2 | 90% | 200 | 1 |
| +2→+3 | 80% | 350 | 2 |
| +3→+4 | 65% | 550 | 3 |
| +4→+5 | 45% | 850 | 5 |

Enhance ATK/HP/MP/DEF theo +0..+5: **1,00 / 1,05 / 1,10 / 1,16 / 1,23 / 1,32**. Flat gains sau rarity — BASELINE/TUNABLE: Boots +2 EVA/cấp; Ring +2 ACC và +0,2 điểm % CritChance/cấp; Necklace +2 EVA/cấp. Không nhân lại flat gains bằng rarity/enhance multiplier. **DO NOT FREEZE DATA BEFORE GEAR-01** cho Tinh Hoa amounts/rarity interaction; flat rules hiện hành dùng để spike, không tìm proposal trong Analysis. Giữ fractional stat để mỗi bước tăng thật; UI làm tròn không mất gain. Bonus Tinh Hoa chỉ kích hoạt một lần theo cấp item, không stack khi equip/reload.

**Loot pipeline:** Gold/Material/Potion/Stone là các channel separate; Gear là **ONE EXCLUSIVE GEAR ROLL**. Một kill có thể Gold + Material + Stone + Gear, nhưng không hai rarity gear. Rates/counts TEST/TUNABLE; semantics dưới đây là baseline triển khai, LOOT-01 chỉ còn tuning/economy.

| Channel | Normal | Linh Biến |
| --- | --- | --- |
| A — Gold | 100%, integer uniform range theo L | 100%, roll base range rồi ×3 |
| B — Trade Material | 30%, 1 item | 100%, 1–2 uniform inclusive |
| C — Potion | 4%, 1 item | Không separate Potion roll/guarantee |
| D — Tinh Thạch | 8%, 1 viên | 100%, 1 viên |
| E — Gear exclusive | Common 4%; Uncommon 1%; Rare 0,1%; None 94,9% | Uncommon 30%; Rare 6%; Epic 0,5%; None 63,5%; không Common |

Gold Host auto-credit sau reward eligibility/distribution, coin burst/text chỉ presentation; không ground gold/pickup RPC. **COOP-01** vẫn phải quyết định EXP/Gold/normal-Linh Biến loot recipients/share; không mặc định killer-only hoặc phát full reward cho mọi player trong map. Q8 forced bonus exception tại §5. Boss Gold cũng auto-credit theo contribution receipt, không cần bag slot.

Gear success: **map → tier**, rarity từ roll, sáu slots uniform 1/6; Weapon 50% Sword / 50% Bow, không smart-loot; class khác không equip nhưng bán được. Mọi gear drop +0. Đồng Sương→Tân Lữ; Trúc Ảnh→Thanh Mộc; Bạch Vân→Bạch Vân; Xích Nham→Xích Nham; Huyền Tích→Huyền Tích. Linh Biến dùng tier map, không exact mob level; rơi tier trước Cấp Mặc là preparation, không bypass equip gate. Quest weapon đúng class là reward exception đã ghi Q6/Q11.

Normal Potion: Lv1–9→I, Lv10–14→II, Lv15+→III; sau đó 50% HP/50% MP, count 1. Boss Potion III roll 50%, count 2–3, chọn HP/MP50/50 cho stack đó. Boss Stone count 5–8 uniform; Hồi Sinh Phù count 1 nếu roll thành công. Không riêng loot table từng mob.

Trade Material theo map/archetype flavor, stack/sell cho Bách Luyện, tooltip “Vật liệu giao dịch — có thể bán”; không crafting/hidden recipe P0. **sellValue Vàng — BASELINE/TUNABLE:** Đồng Sương Nấm→Nấm Sương 2 / Sói→Nanh Sói 3; Trúc Ảnh Sói→Trúc Tâm 4 / Ong→Cánh Ong 4; Bạch Vân Ong→Vân Thạch 6 / Đạo Tặc→Huy Hiệu Đoạt Mạch 6; Xích Nham Đạo Tặc/Thạch Linh→Khoáng Xích Nham 9; Huyền Tích Thạch Linh/Cổ Vệ→Mảnh Cổ Ấn 12. Mỗi archetype ở map chọn một material definition tương ứng, không roll thêm loại; quest Mảnh Huyền Ấn/evidence có ID/channel riêng, không complete bằng Trade Material farm trước.

**Item economy:** ItemDefinition có `buyPrice` optional và `sellValue` explicit. Shop gear bán Common ba tier đầu; sellValue template có thể author từ `floor(buyPrice * 0.25)`, không fake buyPrice cho drop-only. Enhancement không tăng giá bán/hoàn nguyên liệu. Vendor rarity value cho gear: Common×1; Uncommon×1,5; Rare×2; Epic×3 trên Common sellValue, floor một lần; đây là value economy, không stat multiplier. Food/Potion/Stone/phù/Tẩy Mạch dùng sellValue=floor(giá shop×0,25); quest-bound/supply reserved không bán trước bước cho phép.

| Common gear tier | Weapon buy/sell | Armor buy/sell | Pants buy/sell | Boots buy/sell | Ring buy/sell | Necklace buy/sell |
| --- | --- | --- | --- | --- | --- | --- |
| Tân Lữ | 110/27 | 90/22 | 70/17 | 50/12 | 50/12 | 50/12 |
| Thanh Mộc | 300/75 | 250/62 | 200/50 | 150/37 | 150/37 | 150/37 |
| Bạch Vân | 710/177 | 590/147 | 470/117 | 350/87 | 350/87 | 350/87 |
| Xích Nham — drop-only | —/378 | —/315 | —/252 | —/189 | —/189 | —/189 |
| Huyền Tích — drop-only | —/654 | —/545 | —/436 | —/327 | —/327 | —/327 |

Giá là BASELINE/TUNABLE từ kill budgets tại [Analysis S16](3_HUYEN_LO_DESIGN_ANALYSIS.md#world-economy-analysis), không final-release economy. Mỗi ItemDefinition lưu số explicit; không cần runtime pricing service. Quest gear dùng value của template/rarity khi được phép bán.

Bag 30 / storage 40; stack 99, gear 1. Pickup stackable ưu tiên fill compatible stack có sẵn, overflow sang stack mới; transaction không fit toàn bộ thì không consume ground item. Bag đầy nhưng stack còn chỗ vẫn nhặt được. Ground Normal/Linh Biến 60s / Boss 90s, chỉ physical items; unequip cần ô trống, không bán equipped.

Turn-in tính X ô trống thực cần sau merge stack: thiếu thì báo “Cần X ô trống trong hành trang”, giữ READY_TO_TURN_IN; không consume evidence/trao một phần reward/set Completed. Vàng/EXP/story/Journey không cần slot; retry không nhận lặp.

Yên Thảo bán Food/Potion/phù; Bách Luyện bán Common Tân Lữ/Thanh Mộc/Bạch Vân, Tinh Thạch **800 Vàng**, upgrade/sell; Tạ Minh bán Tẩy Mạch. Stock NPC vô hạn, buyPrice/sellValue theo bảng trên. Q9 Dây Bạch Vân là bonus không exclusive: Common mua ở shop, gear drop Bạch Vân cũng có Necklace; bỏ PvP không mất đường nâng slot. Một tiền tệ Vàng. Bag Sort/protection gear P1; validation inventory P0.

> **Đọc sâu:** [Design Analysis — kinh tế và enhance](3_HUYEN_LO_DESIGN_ANALYSIS.md#economy-analysis)

---

<a id="gdd-8"></a>

# 8. Food, Death và consumable

Không passive regen. Food chính: tick 2s theo MaxHP/MP, cap đầy, 10 phút, mới thay cũ, không pause khi trúng. Potion cứu nguy. Nghỉ tại Mộc An ở Vân Khê hồi đầy HP/MP.

| Item | Lv | Hiệu quả | Giá Vàng |
| --- | ---: | --- | ---: |
| Bánh Lúa Mạch | 1 | Mỗi 2s +2% HP, +3% MP | 150 |
| Thịt Nướng Thảo Mộc | 10 | Mỗi 2s +3% HP, +4% MP | 400 |
| Cơm Hầm Linh Thảo | 15 | Mỗi 2s +4% HP, +5% MP | 700 |
| Bình Sinh Lực I/II/III | 1/10/15 | Instant 30/45/60% MaxHP | 80/220/480 |
| Bình Linh Lực I/II/III | 1/10/15 | Instant 30/45/60% MaxMP | 80/220/480 |
| Hồi Sinh Phù | 1 | Tại chỗ 50% HP/MP + 2s invulnerable | 1.000 |
| Bùa Hồi Thành | P1 | Cast 3s về làng; damage hủy cast | 150 |

Potion CD 8s chung tier cùng loại; HP/MP chung hay tách nhóm chờ CONS-01.

HP≤0: Death tint/khóa movement, attack, skill, pickup; camera/corpse tại MapId, không auto-respawn. Về Vân Khê miễn phí full HP/MP hoặc tiêu phù hồi tại chỗ. Consequence phù/run-back; không EXP debt/mất gear/Vàng.

Dead character ở Huyền Tích vẫn có thể eligible loot nếu đủ contribution; Về Làng trước Boss chết mất eligibility. Death không giữ Boss khỏi reset khi không còn ai sống trong arena. Hành vi Food lúc dead/logout và restore HP/MP cần chốt trước persistence hoàn chỉnh.

> **Đọc sâu:** [Design Analysis — Food/MP sustain](3_HUYEN_LO_DESIGN_ANALYSIS.md#combat-analysis)

---

<a id="gdd-9"></a>

# 9. Online, PvP và social

Online multiplayer Host-authoritative, localhost/LAN/DirectIP cùng mạng. **P0 acceptance: tối thiểu 2 concurrent players** — Host + ít nhất 1 Client; đây là mức nghiệm thu, không phải giới hạn world. Internet Relay/VPN/tunnel P1. Host giữ profiles; client chọn IDs, không gửi trusted level/gold/EXP/gear/attributes.

MapId P0 kiểm combat/mob/portal/loot/chat; khác map không tương tác, chỉ render khu hiện tại. Hide/show P1 không gate slice. Boss timer/banner global khi map rỗng.

World support N players; không hard-code hai slot/Player1–Player2. Threat/contribution, MapId/chat, quest assist và loot eligibility dùng collection theo playerId. PvP **1v1** là mode riêng, không giới hạn multiplayer world. Co-op xét mọi player hợp lệ; EXP/normal loot P0 thiếu formula: chốt COOP-01 trước online milestone; audit bonus/radius không tự lock. Không full Party system.

**Map Chat:** Enter mở input/gửi, tối đa 80 ký tự; rate 1 message/2s mỗi playerId do Host kiểm; bubble trên đầu tối đa hai dòng, 4s rồi fade 0,5s, cùng MapId. P0 không history lớn. System banner do Host phát cho Boss T−60, spawn/death và chapter completion; P1 có thể thêm history nhỏ.

**PvP:** Hạo Vũ → chọn player → Invite → Accept/Reject → Arena → countdown → fight → result → Village. Chênh level≤5; Best of 1; 120s. Vào trận full HP/MP; Food pause; Potion/Hồi Sinh disable. HP 0 dùng PvPDefeated, không Death PvE; không mất item/Vàng/EXP. Timeout so %HP còn lại; xử lý hòa/disconnect chờ PVP-01. Winner +200 Journey. Boss/PvP miễn Freeze, player chỉ Slow theo §4. Q9 optional không làm PvP optional trong DoD P0: tối thiểu 2 concurrent players.

> **Implementation:** [Technical — network authority](2_HUYEN_LO_TECHNICAL.md#network-authority)

---

<a id="gdd-10"></a>

# 10. Điều khiển và UX

Một mapping, không secondary. S+Space ưu tiên drop-through. Text input chặn gameplay; Esc cancel/đóng cửa sổ.

| Phím | Action | Phím | Action |
| --- | --- | --- | --- |
| A/D | Move trái/phải | H | Quick HP Potion |
| Space | Jump | M | Quick MP Potion |
| S+Space | Drop-through | F | Food |
| J | Normal Attack | E | Interact/Pickup |
| 1 | Skill 1 | I | Inventory |
| 2 | Skill 2 | C | Character |
| 3 | Skill 3 | Q | Quest |
| R | Buff P1 | Enter | Chat |
| Esc | Cancel | — | — |

Quick Potion baseline chọn tier cao nhất có trong bag/đủ level; reject HP/MP đầy hoặc dead, Host kiểm count/CD/state. Tier selection/F refresh-overwrite còn CONS-01; luật Food 10 phút/mới thay cũ giữ nguyên trước quyết định. E NPC/loot theo target.

HUD: HP/MP/EXP/level, skillCD, Food/Potion, quest, Boss timer. Bag-full rõ; tooltip enhance trước/sau. Portal/signpost tên vùng/hướng; quest arrow P1.

| Tracker state | Người chơi thấy |
| --- | --- |
| IN_PROGRESS | “Hạ Sói Sương: 4/8” (ví dụ UI, count final tại QUEST-02) |
| READY_TO_TURN_IN | “Quay về gặp Lâm Bá để báo cáo” |
| LEVEL GATE | “Tu luyện đến Lv.12”; NPC giải thích vùng phù hợp |
| AVAILABLE | NPC/quest panel cho nhận, chưa có progress trước khi nhận |
| OPTIONAL Q9 | Nhãn “Tùy chọn — Tỷ thí”, tách quest được pin; không chặn chính tuyến |

| NPC | Menu/action |
| --- | --- |
| Lâm Bá | Main quest và dẫn truyện |
| Yên Thảo | Food/Potion/Hồi Sinh |
| Bách Luyện | Gear, đá, sell, upgrade |
| Mộc An | Storage, nghỉ hồi đầy tại hub |
| Tạ Minh | Class, Huyền Môn, Tẩy Mạch |
| Phong Du / Diệp Lam | Hướng dẫn Kiếm / Cung |
| Hạo Vũ | PvP Challenge |

> **Implementation:** [Technical — UI](2_HUYEN_LO_TECHNICAL.md#ui-notes)

---

<a id="gdd-11"></a>

# 11. Art và hợp đồng hình ảnh

Một male modular rig, không female MVP: **64×64px/PPU 32**, body **44–48px**, pivot Bottom-Center(0.5, 0.0), hướng phải/flipX trái. Collider~0,60–0,65u×1,45u TUNABLE, không toàn canvas.

| Animation | Frames | FPS baseline | Animation | Frames | FPS baseline |
| --- | ---: | ---: | --- | ---: | ---: |
| Idle | 4 | 6 | Attack | 3 | 12 |
| Run | 6 | 10 | Skill | 4 | 12 |
| Jump | 2 | 8 | Hit | 2 | 10 |
| Fall | 2 | 8 | Death | 3 | 8 |

**26 frames**, parts đồng bộ index/pivot. Ba families: Tân Lữ/Thanh Mộc; Bạch Vân/Xích Nham; Huyền Tích. Weapon/Armor/Pants modular; Boots/Ring/Necklace icon/stat only. Cùng family reuse silhouette/frame nhưng khác tier có palette/tint hoặc accent rẻ: Tân Lữ vải trầm→Thanh Mộc lục; Bạch Vân sáng lạnh→Xích Nham đỏ/đồng; Huyền Tích family cuối. Không thêm animation set; Head/Hair là base visual, không Helmet slot.

Art P0: male rig, ba families, Sword/Bow visuals, sáu normal sprite sets, không sprite set riêng Linh Biến, một Boss, ba environment families, UI kit, sáu active VFX, Linh Đạn generic, aura Linh Biến, heal/upgrade/death feedback. Forest dùng Đồng Sương/Trúc Ảnh; Mountain dùng Bạch Vân/Xích Nham; Ancient dùng Huyền Tích; hub tái dùng architectural props phù hợp.

Flash/damage/heal/NÉ, local hit-stop, trail, loot beam và sound; không hard CC mới. Layer/import tại Technical.

> **Implementation:** [Technical — art/animation](2_HUYEN_LO_TECHNICAL.md#art-contract)

---

<a id="gdd-12"></a>

# 12. Scope và mức ưu tiên

| Hệ thống | P0 | P1 khi core ổn | P2 |
| --- | --- | --- | --- |
| Player/combat | Novice, hai class, bốn attributes/Tẩy Mạch, normal+3 active+passive, AoE/Freeze PvE | Lv10 buff; Burn Kiếm; DPS Meter | Cosmetic polish |
| World | Năm bãi, SpawnGroup, sáu mob, Linh Biến modifier, Boss basic+ba pattern/Cuồng Mạch | Linh Giáp/Vỡ Thế | Hazard, Boss polish |
| RPG | Sáu gear slots, năm tiers, rarity, +0→+5, shop/drop/bag/storage, Food/Death | Upgrade Transfer; Bag Sort; protection gear; Bùa Hồi Thành | Buy-back, bag expansion |
| Story/UI | Q1–Q12 với Q9 nhánh optional; ba Stage Summary; Journey; controls/HUD | Quest arrow, chat history | Extra cosmetics |
| Online/data | Multiplayer LAN; acceptance tối thiểu 2 concurrent players, MapId, co-op, chat/banner, personal loot, safe JSON | Observer hide/show; Relay/tunnel; MariaDB | Performance polish |

P1 chỉ triển khai sau core và quyết định scope; thông số proposal giữ tại Analysis.

**DROP:** Guild/Trade/Pet/Mount/Crafting/Auction/FreePK; multiple currency; Skill Rank; enhance vượt +5; full Party/Channel/Zone; world chat/dedicated production server; EXP debt; hút HP/MP; Decoy; ghép đá; Hương EXP; elemental weakness/resistance system. Không thêm stat/proc ngoài existing contract.

Roadmap tám tuần: target **160–200h**, risk **160–240h**; gate và rủi ro ở Technical.

> **Implementation:** [Technical — spike/roadmap](2_HUYEN_LO_TECHNICAL.md#roadmap)

---

<a id="gdd-13"></a>

# 13. Definition of Done

**Chưa nghiệm thu**: cần playable build/evidence Host + ít nhất 1 Client (tối thiểu 2 concurrent players), không thay bằng simulation hoặc diễn giải thành capacity tối đa.

| Nhóm | Tiêu chuẩn |
| --- | --- |
| Player | Movement/jump/drop-through; Lv1–20; reset 20 điểm Lv5; hai class; 95 điểm Lv20; Tẩy Mạch không mất dữ liệu |
| Combat | Normal đúng interval; ba active+passive/class; shape/maxTargets/falloff; snapshot/pierce/explosion không double-hit; Evade/Crit; Freeze chỉ normal/Linh Biến |
| World | Năm farm maps, ba support zones; SpawnGroup/return/respawn; đúng sáu archetypes, Linh Biến max1/MapId và Q8 deterministic, một Boss với telegraph/target/reset/Cuồng Mạch |
| Story | Q1–Q12 có setup/objectives/turn-in; thiếu level không auto-chain; READY_TO_TURN_IN không auto trả; Q9 không chặn Q10; Q11 complete mới mở vùng; Q12 per character/Main Story Complete; vòng chơi tiếp tục |
| RPG/art | Food/Potion/Death; bag/storage/shops; six slots/tiers/rarity/+5 tăng giá trị; modular 64×64/PPU 32/26 frames |
| Online | N-player collections; P0 acceptance tối thiểu 2 concurrent players qua LAN; combat/portal/chat/loot MapId validation; co-op; PvP 120s; Boss contribution runtime và personal loot 90s |
| Reliability | Host profiles; JSON temp write/flush/atomic replacement/backup/recovery; không duplication/reward replay; năm demo profiles, scripts/build/video fallback |

Autosave 120s và critical events, **enhance fail cũng save**. Persistence contract, demo profiles và acceptance đầy đủ tại Technical.

> **Implementation:** [Technical — demo/QA](2_HUYEN_LO_TECHNICAL.md#qa)

---

<a id="gdd-14"></a>

# 14. Full Routing Index

| Chủ đề | GDD | Analysis | Technical |
| --- | --- | --- | --- |
| Combat / Balance | [§4](#gdd-4) | [Combat](3_HUYEN_LO_DESIGN_ANALYSIS.md#combat-analysis) | [Data](2_HUYEN_LO_TECHNICAL.md#combat-data) |
| EXP / Attributes | [§3](#gdd-3) | [Baselines](3_HUYEN_LO_DESIGN_ANALYSIS.md#balance-baselines) | [Profile](2_HUYEN_LO_TECHNICAL.md#profile-authority) |
| World / Mob / Boss | [§5](#gdd-5) | [World/flow](3_HUYEN_LO_DESIGN_ANALYSIS.md#world-economy-analysis), [Boss access](3_HUYEN_LO_DESIGN_ANALYSIS.md#boss-availability) | [Timers](2_HUYEN_LO_TECHNICAL.md#timers) |
| Quest / Chapter | [§6](#gdd-6) | [Progression](3_HUYEN_LO_DESIGN_ANALYSIS.md#quest-progression) | [Quest state](2_HUYEN_LO_TECHNICAL.md#combat-data) |
| Gear / Economy | [§7](#gdd-7) | [Economy](3_HUYEN_LO_DESIGN_ANALYSIS.md#economy-analysis) | [Persistence](2_HUYEN_LO_TECHNICAL.md#persistence) |
| Food / Death | [§8](#gdd-8) | [Sustain](3_HUYEN_LO_DESIGN_ANALYSIS.md#combat-analysis) | [Timers](2_HUYEN_LO_TECHNICAL.md#timers) |
| Network / Save | [§9](#gdd-9), [§13](#gdd-13) | [Review](3_HUYEN_LO_DESIGN_ANALYSIS.md#review-decisions) | [Authority](2_HUYEN_LO_TECHNICAL.md#network-authority), [Save](2_HUYEN_LO_TECHNICAL.md#persistence) |
| Art / Controls | [§10](#gdd-10), [§11](#gdd-11) | [Decisions](3_HUYEN_LO_DESIGN_ANALYSIS.md#open-decisions) | [Art](2_HUYEN_LO_TECHNICAL.md#art-contract), [UI](2_HUYEN_LO_TECHNICAL.md#ui-notes) |
| Scope / Roadmap / Demo | [§12](#gdd-12), [§13](#gdd-13) | [Research](3_HUYEN_LO_DESIGN_ANALYSIS.md#research-ideas) | [Roadmap](2_HUYEN_LO_TECHNICAL.md#roadmap), [QA](2_HUYEN_LO_TECHNICAL.md#qa) |
