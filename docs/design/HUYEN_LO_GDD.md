# RPG Online: Huyền Lộ
## Game Design Document

**Current Design Version:** V5.1.2  
**Status:** PRE-IMPLEMENTATION DESIGN  
**Last Reviewed:** 2026-09-29

| Version | Thay đổi |
| --- | --- |
| 5.1.2 | Quest/player journey refinement + consistency fixes |
| 5.1.1 | Hợp nhất docs; sửa deterministic defects; loại scope chưa được chốt |
| 5.1 | Hợp nhất deep audit và review |
| 5.0 | Baseline thiết kế chính |


<a id="gdd-0"></a>

# 0. Trạng thái tài liệu và Design Lock

GDD giữ luật game; [Analysis](HUYEN_LO_DESIGN_ANALYSIS.md) giữ evidence/quyết định mở; [Technical](HUYEN_LO_TECHNICAL.md) giữ implementation contract.

Authority: user lock → review đã thống nhất → deterministic fix → V5 → audit → reference. Analysis/Technical không tự đổi gameplay.

| Phạm vi khóa | Luật |
| --- | --- |
| Nhân vật | Cap Lv20; Tân Lữ Lv1–4; Lv5 chọn một trong hai class; một nhân vật nam |
| Build | Bốn thuộc tính, 95 điểm ở Lv20, không branch cap, không Skill Rank |
| Combat | AoE/Multi-target và SpawnGroup core; Freeze hard CC chỉ normal/Elite PvE |
| Nội dung | 6 normal archetype, 2 named Elite reuse, 1 World Boss; 5 farm maps + 3 support zones |
| RPG | Food regen chính, không passive regen; Death có consequence; 6 gear slots; +0→+5 |
| Online | Host-first; Personal Boss Loot; overhead Map Chat; MapId P0 |
| Asset/save | Một male modular rig, 64×64, PPU 32, 26 frames; JSON P0, MariaDB P1 |

**LOCKED:** luật khóa; **BASELINE:** số hiện dùng; **TUNABLE:** cần playtest. P0 bắt buộc; P1 sau core; P2 polish; DROP ngoài MVP. Research chưa chốt không thành requirement.

> **Đọc sâu:** [Design Analysis — quyết định mở](HUYEN_LO_DESIGN_ANALYSIS.md#open-decisions)

---

<a id="gdd-1"></a>

# 1. Tầm nhìn game và Core Loop

**Elevator Pitch:** RPG hành động 2D online ngang trên PC/Unity. Tân Lữ khám phá linh mạch, chọn Kiếm/Cung, tự phân bốn thuộc tính, gom quái đánh lan, nâng gear, săn Elite/Boss và tỷ thí.

| Pillar | Người chơi cảm nhận | Dấu hiệu đạt |
| --- | --- | --- |
| Farm có nhịp | Gom quái rồi cleave/pierce/spread/explosion | Skill Lv5 tác động tối đa 3 mục tiêu |
| Build tự do | Đổi phân phối điểm để thử cách chơi | All-in hợp lệ; Tẩy Mạch sửa build |
| Progression hữu hình | Gear mới đổi cả stat và hình | Weapon/Armor/Pants đổi sprite |
| Hai class khác nhau | Kiếm áp sát; Cung giữ khoảng cách | Range, hit shape và control khác rõ |
| Online có ý nghĩa | Co-op farm/Boss, chat, challenge | Client thứ hai tham gia gameplay thật |
| Scope hoàn chỉnh | Ít nội dung nhưng nối thành hành trình | Lv1→20, ba chương, chính tuyến và vòng chơi sau truyện |

**Core Loop:** Quest chỉ đường → Food/Potion → vùng mở → gom cụm/đánh lan → EXP/loot → build/gear → level gate → NPC. Elite/Boss/PvP xen giữa chặng farm; Food/túi đồ tạo nhịp về làng.

Target Lv20: 2,5–4h gồm travel/quest/shop/run-back, chưa được playtest. Nghiệm thu hai player; 3–4 player cần benchmark riêng.

---

<a id="gdd-2"></a>

# 2. Thế giới và cốt truyện

Mạch Ấn nối Vân Khê tới Huyền Tích, điều hòa linh lực. Tân Lữ nhập Học Viện Thiên Môn khi Đạo Tặc khai thác Mảnh Huyền Ấn tại Xích Nham: Nấm/Sói hung dữ, Ong tụ linh khí, Thạch Linh thức tỉnh, Cổ Vệ coi mọi sinh vật là kẻ xâm nhập.

Cự Thú là Thủ Vệ bị cuồng hóa. Khôi phục Huyền Môn/thanh tẩy Thủ Vệ khép lại cốt truyện chính hiện tại; nhân vật không là “người được chọn”.

| Chương | Dải cấp | Diễn tiến | Checkpoint tổng kết |
| --- | --- | --- | --- |
| I — Dấu Nứt Vân Khê | 1–7 | Sinh tồn, nhập học, lần đầu nâng gear | Q7 completed và Lv≥7 |
| II — Theo Dấu Huyền Lộ | 8–17 | Trúc Ảnh→Bạch Vân→Xích Nham; phục hồi Huyền Môn | Q11 completed và Lv≥17 |
| III — Huyền Tích Thức Tỉnh | 18–20 | Vượt Cổ Vệ, thanh tẩy Cự Thú | Q12 completed và Lv20 |

**Main Story Complete — HOÀN THÀNH CHÍNH TUYẾN:** Q12 khép lại chính tuyến/Chương III. Summary: **CHƯƠNG III HOÀN THÀNH / CHÍNH TUYẾN ĐÃ HOÀN THÀNH** — “Hành trình tại Huyền Lộ vẫn tiếp tục.” Tiếp tục farm Huyền Tích, săn Rare/Epic/+5/Elite/Dư Ảnh, PvP Challenge, Tẩy Mạch/thử build, Journey Score và P1 nếu được triển khai.

Q12 **per character**: chưa complete hiển thị **Huyền Nham Cự Thú**, đã complete **Dư Ảnh Huyền Nham**, kể cả tracker/banner. Hai tên dùng một entity/sprite/AI/drop, không world story flag.

> **Đọc sâu:** [Design Analysis — review narrative](HUYEN_LO_DESIGN_ANALYSIS.md#review-decisions)

---

<a id="gdd-3"></a>

# 3. Progression và thuộc tính

Lv20 cap; 5 điểm/level-up = **95 điểm**. Lv1 chưa có điểm; Lv2–4 auto +2STR/+2VIT/+1INT, tổng 15. Lv5 thêm 5 và hoàn 15 thành **20 unspent** một lần; chọn class Q6 rồi tự cộng. Chưa class không dùng skill/vũ khí class. Q3 trao Mộc Kiếm; Lv1–2 học NPC/movement, chưa bị yêu cầu combat.

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

`MobBaseEXP(L) = round(10 + 2,5L + 0,12L²)`. Sau onboarding farm là nguồn EXP/Vàng/đá/gear/material chính; quest dẫn đường/dạy mechanic/kể chuyện/mở nội dung. Target Q5–Q11 **8–15% bar tại RequiredLevel**, retune QUEST-03; target 600–720 kills cần đo lại. `DemoExpMultiplier = 10` chỉ cho demo.

| Nhánh | Mỗi điểm | Baseline |
| --- | --- | --- |
| Sinh Lực — VIT | +8 HP, +0,10 DEF | Chịu đòn |
| Linh Lực — INT | +5 MP, +0,25% Skill Damage | Giữ 0,25%; test 0,30/0,35 ở Analysis |
| Công Lực — STR | +0,70 ATK | Tăng normal và skill |
| Thân Pháp — AGI | +6 ACC, +6 EVA, +0,05% MoveSpeed | MoveSpeed TUNABLE, lợi ích chính là ACC/EVA |

Base theo L: HP = 120+10(L−1); MP = 60+4(L−1); ATK = 12+1,2(L−1); DEF = 5+0,6(L−1). Cộng gear và điểm trước khi nhân modifier class. ACC = 60+4(L−1)+6AGI+GearACC; EVA = 20+2(L−1)+6AGI+GearEVA.

`EvadeChance = 0,02 + 0,43 × EVADefender / (EVADefender + 2,5 × ACCAttacker)`. Hàm tiệm cận 45%, không hard cap điểm AGI. `SkillDamageBonus = 1 + INT×0,0025`; chỉ skill dùng bonus này. `MoveSpeedMultiplier = 1 + AGI×0,0005`.

**Tẩy Mạch Phù:** 1.200 Vàng tại Tạ Minh, stock vô hạn; trả mọi điểm đã cộng về unspent, giữ class/level/gear/quest/skill. Reset lúc chọn class miễn phí, chỉ thực hiện một lần.

> **Đọc sâu:** [Design Analysis — balance](HUYEN_LO_DESIGN_ANALYSIS.md#balance-baselines)

---

<a id="gdd-4"></a>

# 4. Class, combat và kỹ năng

| Tiêu chí | Tân Lữ Lv1–4 | Kiếm Sĩ Lv5+ | Xạ Thủ Lv5+ |
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

Liên Tiễn≥3 target ưu tiên khác nhau; một target tổng1,35×. Liên Kích snapshot, không reacquire; bỏ target mất hợp lệ. Pierce theo đường đạn. Explosion loại primary, không double-hit.

**Damage:** normal `Raw = FinalATK×NormalPower`; skill `Raw = FinalATK×SkillPower×SkillDamageBonus`. Host validate → roll Evade → Crit → DEF → random → HP/result. Nếu né, damage 0 và hiện NÉ. Nếu trúng: `Damage = max(1, round(Raw×100/(100+TargetDEF)×Random(0,95; 1,05)×CritMultiplier))`. CritMultiplier cố định **1,5**; CritChance cộng class, passive và gear.

| Target Hàn Tiễn | Slow | Freeze trên hit hợp lệ |
| --- | --- | --- |
| Normal | 30% / 2s | 25% chance / 1,2s |
| Elite | 30% / 2s | 25% chance / 0,6s |
| Boss | 10% / 1,5s | Miễn nhiễm |
| PvP player | 20% / 1,5s | Không áp dụng |

Freeze 25% cần playtest; khóa movement/attack AI, dùng overlay chung. Player xuyên quái, không contact damage. Farm không knockback; flash/hit-stop không hard stun.

Kiếm ưu tiên phía trước; trống thì auto-facing target sau lưng≤1,2u. Cung cone±25° phía trước, không auto quay sau. Không target vẫn cast theo facing. Melee originY+0,8u; vertical reach TUNABLE.

> **Đọc sâu:** [Design Analysis — combat](HUYEN_LO_DESIGN_ANALYSIS.md#combat-analysis)

---

<a id="gdd-5"></a>

# 5. World, mob, Elite và Boss

**World route:** Vân Khê ↔ Đồng Sương ↔ Trúc Ảnh ↔ Bạch Vân ↔ Xích Nham ↔ Huyền Tích. Vân Khê nối Học Viện và Lôi Đài. Q11 mở Huyền Môn từ Xích Nham. Không fast travel ra bãi; Bùa Hồi Thành P1 chỉ đưa về làng.

| Khu | Cấp | Cụm / số normal dự kiến | Nội dung/địa hình |
| --- | ---: | --- | --- |
| Đồng Sương | 1–4 | 3 / 9 | Nấm Lv1–2, Sói Lv3–4; tutorial 1–2 tầng |
| Trúc Ảnh Lâm | 5–8 | 4 / 12–14 | Sói Lv5–6, Ong Lv7–8; cầu tre, Elite Sói |
| Bạch Vân Thác | 8–12 | 4 / 14–16 | Ong Lv8–9, Đạo Tặc Lv10–12; ba tầng |
| Xích Nham Cốc | 12–17 | 4 / 14–16 | Đạo Tặc Lv13, Thạch Linh Lv14–17; Elite Đạo Tặc |
| Huyền Tích | 18–20 | 3 / 12 | Thạch Linh Lv17, Cổ Vệ Lv18–20; Boss landmark |
| Vân Khê / Học Viện / Lôi Đài | Support | Không bãi farm | NPC, class/dummy, challenge |

| Vùng | Gate vào vùng — BASELINE | Ý nghĩa |
| --- | --- | --- |
| Đồng Sương | Onboarding mở từ đầu | Q4 dạy sinh tồn; chưa có story gate |
| Trúc Ảnh | Q6 COMPLETED + Lv5 | Chọn class rồi thử đánh cụm |
| Bạch Vân | Q8 COMPLETED + Lv8 | Bằng chứng Elite mở tuyến điều tra |
| Xích Nham | Q8 COMPLETED + Lv12 | Power gate để farm trước Q10; không phụ thuộc Q9 |
| Huyền Tích | Q11 COMPLETED | Q11 đã cần Lv17; Q12 riêng cần Lv20 |

Level là power gate, quest flag là story gate; mở bãi farm không tự nhận quest. Village/Academy mở onboarding; Arena qua Challenge. Gate levels BASELINE, playtest cùng QUEST-03.

Tổng **5 farm maps + 3 support zones = 8 logical map roots**, 18 SpawnGroups là target placement. Mỗi cụm 3–4 normal, spacing 0,8–1,5u; aggro 5u, leash 8u. Hit một con alert group; attack offset random 0–0,35s tránh cùng frame. Vượt leash → Return, tạm miễn damage, về spawn rồi full HP; không kéo nhiều bãi thành 20 quái.

| Normal archetype | AI | Move u/s | Melee/ranged u | Interval | Projectile |
| --- | --- | ---: | --- | ---: | --- |
| Nấm Linh | Melee only | 1,2 | 0,8 / — | 1,8s | — |
| Sói Sương | Melee only; chase nhanh | 2,4 | 1,0 / — | 1,3s | — |
| Ong Giáp | Ranged/Flying | 2,0 | — / 5 | 1,6s | Generic, speed 5,5u/s |
| Đoạt Mạch Đạo Tặc | Hybrid | 2,2 | 1,2 / 5 | 1,5s | Generic, speed 5u/s |
| Xích Thạch Linh | Hybrid | 1,4 | 1,1 / 4,5 | 2,0s | Generic, speed 4u/s |
| Cổ Môn Vệ Binh | Hybrid | 1,8 | 1,4 / 6 | 1,6s | Generic, speed 6u/s |

Hybrid reachable→Chase/Melee; ranged khi platform không reach/vertical/chase blocked-timeout, không chỉ vì distance>melee. Ong FlyingBox~6×3u. Một Linh Đạn generic đổi scale/tint/speed/trail; không projectile/animation riêng từng loài.

Stat normal tại L: HP = round(80+12L+1,2L²); DEF = round(2+0,8L); ATK = round(6+1,6L+0,08L²); ACC = 60+4L; EVA = 20+2L; EXP theo §3. Vàng từ 3+2L tới 6+3L. Normal respawn **12s — BASELINE/TUNABLE**, test 8/12/15s; timer độc lập AI.

| Named Elite | Base / nơi | Stat/drop | Visual |
| --- | --- | --- | --- |
| Sói Đầu Đàn | Sói Sương Lv8 / Trúc Ảnh | HP×4, ATK×1,5, EXP×4, Gold×3, Move×1,05 | Reuse sprite; scale ~1,25–1,30; aura/name/HP bar lớn |
| Thiết Giáp Đoạt Mạch | Đạo Tặc Lv15 / Xích Nham | Cùng multiplier; Elite drop table | Cùng AI/animation/projectile gốc |

Elite không skill riêng; respawn release 3–5 phút, demo 20s. Linh Biến chỉ là P1 candidate, không cộng vào roster P0.

**Một World Boss:** Lv20 Huyền Nham Cự Thú, HP **32.000 BASELINE**, ATK **140 BASELINE**; **DEF/ACC/EVA TUNABLE — BOSS-02**, chưa khóa; không EXP ở cap. Arena radius 15u; respawn 15 phút sau death, demo 60s; tracker luôn có, cảnh báo T−60s và banner spawn/death. Release target hai endgame player hạ trong 90–150s, chưa được chứng minh bằng build.

| Pattern | Power | Telegraph | CD/interval | Shape |
| --- | ---: | ---: | ---: | --- |
| Basic | 1,00× | — | 1,8s | Melee |
| Nham Trảo | 1,20× | 0,5s | 2,5s | Frontal cone |
| Địa Chấn | 1,50× | 1,0s | 6s | Ground AoE, nhảy né |
| Nham Thạch Rơi | 1,80× | 1,2s | 8s | Nhiều vùng cảnh báo trên mặt đất; count/overlap/scheduler tại BOSS-02 |

Threat P0: `Threat[player] += DamageDealt`; target alive có threat cao nhất, retarget khi chết/rời arena hoặc khoảng 1s. Nếu `AlivePlayersInArena == 0` liên tục 10s: reset HP/position, clear Threat **và Contribution**. Dead player không ngăn reset. Enrage/Cuồng Mạch và Linh Giáp chưa nằm P0.

Reward: `DamageContribution >= RuntimeBossMaxHP×5%`, connected cùng MapId khi Boss chết. Release **1.600**; demoHP**12.800**→threshold **640**. Corpse/phù tại chỗ eligible; về làng/disconnect không. Join muộn/AFK vẫn đủ nếu đạt threshold.

Mỗi eligible player có reward riêng; quan hệ roll Rare/Epic chờ LOOT-01. Table tỷ lệ hiện tại: 1.000 Vàng; Tinh Thạch 5–8 (100%); Rare Huyền Tích gear 40%; Epic 8%; Hồi Sinh Phù 30%; Potion III 2–3 (50%); Journey +500. PersonalLootDrop owner-only cả visibility và pickup, tồn tại 90s, không chuyển FFA. Bag full để item trên đất và báo đầy; không có shared chest/Top DPS bonus.

> **Implementation:** [Technical — timer và Boss lifecycle](HUYEN_LO_TECHNICAL.md#timers)

---

<a id="gdd-6"></a>

# 6. Quest và tiến trình chương

Q1–Q4 catch-up onboarding, target không grind mười phút đầu. Q5+ EXP hỗ trợ farm, reward chính là gear/utility/story. **EXP cũ dưới đây BASELINE tạm, chưa retune/không LOCKED**, đang vượt target 8–15%; proposal/final tại QUEST-03.

**Quest flow:** LOCKED → AVAILABLE (story flag + RequiredLevel) → nhận → IN_PROGRESS → đủ sub-objectives → READY_TO_TURN_IN → chủ động gặp đúng NPC → COMPLETED + reward/story flag/portal unlock → kiểm eligibility quest kế.

READY_TO_TURN_IN không auto-turn-in/teleport; được farm/loot/heal/khám phá tiếp. Đủ story thiếu level vẫn LOCKED: NPC: “Đường phía trước cần thực lực cấp12; hãy tu luyện rồi quay lại.” HUD: “Nhiệm vụ chính tiếp theo mở tại Lv.12”, không marker AVAILABLE giả.

| Quest / Lv | Setup → Story beat | Sub-objectives | Turn-in | Reward hiện tại | Unlock / Next condition |
| --- | --- | --- | --- | --- | --- |
| Q1 — Người mới đến Vân Khê / 1 | Lâm Bá giới thiệu làng → biết thuốc/rèn/storage | Gặp Lâm Bá, Yên Thảo, Bách Luyện, Mộc An; trở lại báo | Lâm Bá | Catch-up Lv2 + 50 Vàng | Q2: Q1 complete + Lv2 |
| Q2 — Bước chân đầu tiên / 2 | Đường Học Viện có bục/cổng → tự đi/về | A/D tới marker; Space vượt bục; S+Space drop-through; portal Vân Khê↔Học Viện; báo cáo | Lâm Bá | Catch-up Lv3 + 75 Vàng | Q3: Q2 complete + Lv3 |
| Q3 — Vũ khí trong tay / 3 | Phong Du trao Mộc Kiếm Tân Lữ → equip/hit feedback | Nhận kiếm; Inventory/equip; tới sân luyện; hạ 3 Bù Nhìn Tập Luyện; trở lại | Phong Du | Kiếm cấp ở bước nhận; turn-in catch-up Lv4 + Quần Vải Tân Lữ | Q4: Q3 complete + Lv4 |
| Q4 — Sinh tồn ngoài làng / 4 | Yên Thảo lo kiệt sức ngoài bãi → hồi phục chủ động | Mua Food/Potion; F dùng Food; ra bãi/hạ quái tutorial; H dùng Bình Sinh Lực khi HP chưa đầy; về báo | Yên Thảo | Ứng 320 Vàng một lần khi nhận; turn-in catch-up Lv5 | Q5: Q4 complete + Lv5 |
| Q5 — Chiến lợi phẩm đầu tiên / 5 | Bách Luyện dạy tận dụng loot → đồ thừa nuôi chuyến sau | Hạ quái; nhặt gear; equip món mới; bán món thừa; báo lại | Bách Luyện | 180 EXP tạm + Áo Thanh Mộc | Q6: Q5 complete + Lv5 |
| Q6 — Con đường của ta / 5 | Tạ Minh dẫn chọn phái → khác cách đánh/dùng MP | Xác nhận reset 20 điểm một lần; chọn class; nhận/equip class weapon; cộng điểm; dùng Skill1; M Quick MP Potion hoặc instruction khi thiếu Potion | Tạ Minh | Class/weapon/skill mở trong tutorial; chưa có EXP baseline | Q7: Q6 complete + Lv7; mở Trúc Ảnh |
| Q7 — Tinh Thạch đầu tiên / 7 | Bách Luyện hướng dẫn rèn → hiểu chi phí/gain | Nhận/kiếm đá; chọn item; xem preview trước/sau; nâng +1; equip/confirm; báo | Bách Luyện | 210 EXP tạm + Nhẫn Thanh Mộc | Q8: Q7 complete + Lv8 |
| Q8 — Bóng sói trong Trúc Ảnh / 8 | Sói kích động → phát hiện Mạch Ấn lệch dòng | Điều tra Trúc Ảnh; hạ Sói/tìm dấu bất thường; hạ Sói Đầu Đàn; báo cáo | Lâm Bá | 320 EXP tạm + Giày Thanh Mộc + 2 đá | Mở Bạch Vân; Q9 optional Lv12, Q10 Lv15; cả hai cần Q8 complete |
| Q9 — Khảo Chiến Đồng Môn / 12 | Hạo Vũ mời tỷ thí → học challenge/result | Invite/accept player khác; hoàn thành tutorial PvP hợp lệ; báo | Hạo Vũ | 700 EXP tạm + Dây Bạch Vân, chỉ khi làm PvP | Optional; không có Client2 vẫn tới Q10; không bot/reward khi skip |
| Q10 — Dấu chân Xích Nham / 15 | Tạ Minh nghe Mạch Ấn suy yếu → phát hiện Đạo Tặc khai thác Mảnh Huyền Ấn | Tới Xích Nham; hạ Đoạt Mạch Đạo Tặc; thu khoáng/vật chứng; mang về | Tạ Minh | 1.300 EXP tạm + Quần Xích Nham | Q11: Q10 complete + Lv17, tuyệt đối không cần Q9 |
| Q11 — Mở Lối Huyền Môn / 17 | Cổng thiếu Ấn → phục hồi linh lực/mở đường | Hạ 12 Xích Thạch Linh; thu 3 Mảnh Huyền Ấn tại Xích Nham; đặt/kích hoạt Ấn ở cổng; về xác nhận | Tạ Minh | 1.800 EXP tạm + Rare Xích Nham weapon đúng class | **Q11 COMPLETED mới mở Huyền Tích**; Q12 thêm Lv20 |
| Q12 — Tiếng gọi từ Huyền Tích / 20 | Cổng ổn định, Thủ Vệ vẫn cuồng → thanh tẩy/chính tuyến khép lại | Vào Huyền Tích; hạ 6 Cổ Vệ; tới trung tâm; tham gia Boss đủ điều kiện; về báo | Tạ Minh | Không EXP ở cap; 1.000 Vàng + story completion; Boss reward theo contribution, không cấp lần hai khi turn-in | **Main Story Complete**, Chương III complete; tiếp tục vòng chơi Dư Ảnh |

Bù Nhìn HP hữu hạn/hạ được/reset nhanh TUNABLE, 0 EXP/Vàng/loot; không mob farm/animation mới. Q3 kiếm/Q4 tiền/Q6 class-weapon-skill cấp trước một lần; completion reward chỉ khi turn-in. Reset 20 điểm không cấp lại khi reconnect.

Q4 H kiểm HP chưa đầy, không bắt damage giả. Q6 instruction M khi thiếu Potion, không grind để mua. Q4 count/supply, Q5 gear, Q7 đá, vật chứng Q8/Q10/full-bag tại QUEST-02; dùng nguồn/material sẵn có.

**Farm beats:** Q6→7 gom cụm/gear; Q7→8 chuẩn bị Elite; Q8→12 Bạch Vân; Lv12→15 Xích Nham dù bỏ Q9; Q10→17 đá/nâng đồ; Q11→20 chuẩn bị Boss. Q12 availability BOSS-03, không story instance tự thêm.

Catch-up `RemainingToNextLevel = RequiredExp − CurrentExpInLevel`: chỉ bù thiếu tới mốc onboarding; đã đạt mốc chỉ BaseEXP (QUEST-02). Q11 objectives đều ngoài Huyền Tích, portal vẫn khóa ở READY_TO_TURN_IN; turn-in mới mở nên không unlock deadlock.

| Journey event | Điểm | Journey event | Điểm |
| --- | ---: | --- | ---: |
| Normal kill | +5 | Elite kill | +100 |
| Main quest complete | +150 | PvP win | +200 |
| Boss reward | +500 | Chapter complete | +300 |

Journey phục vụ summary/rubric, không stat/tiền; checkpoint §2. Quest/chapter reward một lần; Journey tiếp tục tích lũy qua hoạt động lặp hợp lệ, không qua replay claim.

> **Đọc sâu:** [Design Analysis — progression và quyết định](HUYEN_LO_DESIGN_ANALYSIS.md#quest-progression)

---

<a id="gdd-7"></a>

# 7. Trang bị, cường hóa và kinh tế

Chỉ có **Weapon, Armor, Pants, Boots, Ring, Necklace**. Weapon/Armor/Pants đổi sprite; ba slot còn lại stat/icon only. Sword chỉ Kiếm Sĩ, Bow chỉ Xạ Thủ; Tân Lữ dùng Mộc Kiếm. Mộc Kiếm không bán/vứt.

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

Mốc +0→+5 giữ chi phí và success V5; fail giữ cấp hiện tại, vẫn mất Vàng/đá.

| Bước | Success | Vàng/lần | Tinh Thạch/lần |
| --- | ---: | ---: | ---: |
| +0→+1 | 100% | 100 | 1 |
| +1→+2 | 90% | 200 | 1 |
| +2→+3 | 80% | 350 | 2 |
| +3→+4 | 65% | 550 | 3 |
| +4→+5 | 45% | 850 | 5 |

Enhance ATK/HP/MP/DEF theo +0..+5: **1,00 / 1,05 / 1,10 / 1,16 / 1,23 / 1,32**. ACC/EVA/CritChance tăng flat TUNABLE tại Analysis S10 sau rarity; không nhân lại multiplier. Giữ fractional stat để mỗi bước tăng thật; UI làm tròn không mất gain. Bonus Tinh Hoa chỉ kích hoạt một lần theo cấp item, không stack khi equip/reload.

| Drop | Normal | Elite |
| --- | --- | --- |
| Vàng | 100%, range theo L | 100%, Gold×3 |
| Material | 30% | 100%, 2–4 |
| Potion tier | 4% | Không có drop riêng trong V5 |
| Tinh Thạch | 8%, một viên | 100%, 2–4 |
| Gear | Common 4%; Uncommon 1%; Rare 0,1% | Uncommon 50%; Rare 12%; Epic 1% |

Các tỷ lệ gear giữ hiện tại; independent roll hay tối đa một món, Boss Rare/Epic độc lập hay supersede: **LOOT-01**, chưa khóa.

Material map: Đồng Sương Nấm Sương/Nanh Sói; Trúc Ảnh Trúc Tâm/Cánh Ong; Bạch Vân Vân Thạch/Huy Hiệu Đoạt Mạch; Xích Nham Khoáng Xích Nham/Mảnh Huyền Ấn quest; Huyền Tích Mảnh Cổ Ấn. Material phục vụ quest/economy, không crafting.

Bag 30 / storage 40; stack 99, gear 1. Ground normal 60s / Boss 90s. Full bag từ chối pickup/unequip; không bán equipped. Quest reward phải bảo toàn, full-bag policy mở.

Yên Thảo bán Food/Potion/phù; Bách Luyện bán Common Tân Lữ/Thanh Mộc/Bạch Vân, Tinh Thạch **800 Vàng**, upgrade/sell; Tạ Minh bán Tẩy Mạch. `SellPrice = floor(BuyPrice×0,25)`, stock NPC vô hạn. Một tiền tệ Vàng. Bag Sort/protection gear P1; validation inventory P0.

> **Đọc sâu:** [Design Analysis — kinh tế và enhance](HUYEN_LO_DESIGN_ANALYSIS.md#economy-analysis)

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

> **Đọc sâu:** [Design Analysis — Food/MP sustain](HUYEN_LO_DESIGN_ANALYSIS.md#combat-analysis)

---

<a id="gdd-9"></a>

# 9. Online, PvP và social

P0 Host authority+Player 1, Client Player 2, localhost/LAN/DirectIP cùng mạng. Internet Relay/VPN/tunnel P1. Host giữ profiles; client chọn IDs, không gửi trusted level/gold/EXP/gear/attributes.

MapId P0 kiểm combat/mob/portal/loot/chat; khác map không tương tác, chỉ render khu hiện tại. Hide/show P1 không gate slice. Boss timer/banner global khi map rỗng.

Co-op EXP/normal loot P0 thiếu formula: chốt COOP-01 trước online milestone; audit bonus/radius không tự lock. Không full Party system.

**Map Chat:** Enter mở input/gửi, tối đa 80 ký tự; rate 1 message/2s do Host kiểm; bubble trên đầu tối đa hai dòng, 4s rồi fade 0,5s, cùng MapId. P0 không history lớn. System banner do Host phát cho Boss T−60, spawn/death và chapter completion; P1 có thể thêm history nhỏ.

**PvP:** Hạo Vũ → chọn player → Invite → Accept/Reject → Arena → countdown → fight → result → Village. Chênh level≤5; Best of 1; 120s. Vào trận full HP/MP; Food pause; Potion/Hồi Sinh disable. HP 0 dùng PvPDefeated, không Death PvE; không mất item/Vàng/EXP. Timeout so %HP còn lại; xử lý hòa/disconnect chờ PVP-01. Winner +200 Journey. Boss/PvP miễn Freeze, player chỉ Slow theo §4. Q9 optional không làm PvP optional trong DoD hai client.

> **Implementation:** [Technical — network authority](HUYEN_LO_TECHNICAL.md#network-authority)

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

Quick Potion chọn tier cao nhất có trong bag/đủ level; Host kiểm count/CD/state. F Food hợp lệ, ưu tiên tier chờ test. E NPC/loot theo target.

HUD: HP/MP/EXP/level, skillCD, Food/Potion, quest, Boss timer. Bag-full rõ; tooltip enhance trước/sau. Portal/signpost tên vùng/hướng; quest arrow P1.

| NPC | Menu/action |
| --- | --- |
| Lâm Bá | Main quest và dẫn truyện |
| Yên Thảo | Food/Potion/Hồi Sinh |
| Bách Luyện | Gear, đá, sell, upgrade |
| Mộc An | Storage, nghỉ hồi đầy tại hub |
| Tạ Minh | Class, Huyền Môn, Tẩy Mạch |
| Phong Du / Diệp Lam | Hướng dẫn Kiếm / Cung |
| Hạo Vũ | PvP Challenge |

> **Implementation:** [Technical — UI](HUYEN_LO_TECHNICAL.md#ui-notes)

---

<a id="gdd-11"></a>

# 11. Art và hợp đồng hình ảnh

Một male modular rig, không female MVP: **64×64px/PPU 32**, body **44–48px**, pivot Bottom-Center(0,5;0,0), hướng phải/flipX trái. Collider~0,60–0,65u×1,45u TUNABLE, không toàn canvas.

| Animation | Frames | FPS baseline | Animation | Frames | FPS baseline |
| --- | ---: | ---: | --- | ---: | ---: |
| Idle | 4 | 6 | Attack | 3 | 12 |
| Run | 6 | 10 | Skill | 4 | 12 |
| Jump | 2 | 8 | Hit | 2 | 10 |
| Fall | 2 | 8 | Death | 3 | 8 |

**26 frames**, parts đồng bộ index/pivot. Ba families: Tân Lữ/Thanh Mộc; Bạch Vân/Xích Nham; Huyền Tích. Weapon/Armor/Pants modular; Boots/Ring/Necklace icon/stat only.

Art P0: male rig, ba families, Sword/Bow visuals, sáu normal sprite sets, zero set riêng Elite, một Boss, ba environment families, UI kit, sáu active VFX, Linh Đạn generic, aura Elite, heal/upgrade/death feedback. Forest dùng Đồng Sương/Trúc Ảnh; Mountain dùng Bạch Vân/Xích Nham; Ancient dùng Huyền Tích; hub tái dùng architectural props phù hợp.

Flash/damage/heal/NÉ, local hit-stop, trail, loot beam và sound; không hard CC mới. Layer/import tại Technical.

> **Implementation:** [Technical — art/animation](HUYEN_LO_TECHNICAL.md#art-contract)

---

<a id="gdd-12"></a>

# 12. Scope và mức ưu tiên

| Hệ thống | P0 | P1 khi core ổn | P2 |
| --- | --- | --- | --- |
| Player/combat | Novice, hai class, bốn attributes/Tẩy Mạch, normal+3 active+passive, AoE/Freeze PvE | Lv10 buff; Burn Kiếm; DPS Meter | Cosmetic polish |
| World | Năm bãi, SpawnGroup, sáu mob, hai Elite reuse, Boss basic+ba pattern | Linh Biến; Linh Giáp; Enrage candidate | Hazard, Boss polish |
| RPG | Sáu gear slots, năm tiers, rarity, +0→+5, shop/drop/bag/storage, Food/Death | Upgrade Transfer; Bag Sort; protection gear; Bùa Hồi Thành | Buy-back, bag expansion |
| Story/UI | Q1–Q12 với Q9 nhánh optional; ba Stage Summary; Journey; controls/HUD | Quest arrow, chat history | Extra cosmetics |
| Online/data | Hai player LAN, MapId, co-op, chat/banner, personal loot, safe JSON | Observer hide/show; Relay/tunnel; MariaDB | Performance polish |

P1 chỉ triển khai sau core và quyết định scope; thông số proposal giữ tại Analysis.

**DROP:** Guild/Trade/Pet/Mount/Crafting/Auction/FreePK; multiple currency; Skill Rank; enhance vượt +5; full Party/Channel/Zone; world chat/dedicated production server; EXP debt; hút HP/MP; Decoy; ghép đá; Hương EXP; elemental weakness/resistance system. Không thêm stat/proc ngoài existing contract.

Roadmap tám tuần: target **160–200h**, risk **160–240h**; gate và rủi ro ở Technical.

> **Implementation:** [Technical — spike/roadmap](HUYEN_LO_TECHNICAL.md#roadmap)

---

<a id="gdd-13"></a>

# 13. Definition of Done

**Chưa nghiệm thu**: cần playable build/evidence hai client, không thay bằng simulation.

| Nhóm | Tiêu chuẩn |
| --- | --- |
| Player | Movement/jump/drop-through; Lv1–20; reset 20 điểm Lv5; hai class; 95 điểm Lv20; Tẩy Mạch không mất dữ liệu |
| Combat | Normal đúng interval; ba active+passive/class; shape/maxTargets/falloff; snapshot/pierce/explosion không double-hit; Evade/Crit; Freeze chỉ normal/Elite |
| World | Năm farm maps, ba support zones; SpawnGroup/return/respawn; đúng sáu archetypes, hai Elite reuse, một Boss với telegraph/target/reset |
| Story | Q1–Q12 có setup/objectives/turn-in; thiếu level không auto-chain; READY_TO_TURN_IN không auto trả; Q9 không chặn Q10; Q11 complete mới mở vùng; Q12 per character/Main Story Complete; vòng chơi tiếp tục |
| RPG/art | Food/Potion/Death; bag/storage/shops; six slots/tiers/rarity/+5 tăng giá trị; modular 64×64/PPU 32/26 frames |
| Online | Host+Client LAN; combat/portal/chat/loot MapId validation; co-op; PvP 120s; Boss contribution runtime và personal loot 90s |
| Reliability | Host profiles; JSON temp write/flush/atomic replacement/backup/recovery; không duplication/reward replay; năm demo profiles, scripts/build/video fallback |

Save: temp/flush/atomic replacement/backup/recovery; autosave 120s và critical events, **enhance fail cũng save**. API, profile demo và acceptance đầy đủ tại Technical.

> **Implementation:** [Technical — demo/QA](HUYEN_LO_TECHNICAL.md#qa)

---

<a id="gdd-14"></a>

# 14. Full Routing Index

| Chủ đề | GDD | Analysis | Technical |
| --- | --- | --- | --- |
| Combat / Balance | [§4](#gdd-4) | [Combat](HUYEN_LO_DESIGN_ANALYSIS.md#combat-analysis) | [Data](HUYEN_LO_TECHNICAL.md#combat-data) |
| EXP / Attributes | [§3](#gdd-3) | [Baselines](HUYEN_LO_DESIGN_ANALYSIS.md#balance-baselines) | [Profile](HUYEN_LO_TECHNICAL.md#profile-authority) |
| World / Mob / Boss | [§5](#gdd-5) | [Review](HUYEN_LO_DESIGN_ANALYSIS.md#review-decisions) | [Timers](HUYEN_LO_TECHNICAL.md#timers) |
| Quest / Chapter | [§6](#gdd-6) | [Progression](HUYEN_LO_DESIGN_ANALYSIS.md#quest-progression) | [Quest state](HUYEN_LO_TECHNICAL.md#combat-data) |
| Gear / Economy | [§7](#gdd-7) | [Economy](HUYEN_LO_DESIGN_ANALYSIS.md#economy-analysis) | [Persistence](HUYEN_LO_TECHNICAL.md#persistence) |
| Food / Death | [§8](#gdd-8) | [Sustain](HUYEN_LO_DESIGN_ANALYSIS.md#combat-analysis) | [Timers](HUYEN_LO_TECHNICAL.md#timers) |
| Network / Save | [§9](#gdd-9), [§13](#gdd-13) | [Review](HUYEN_LO_DESIGN_ANALYSIS.md#review-decisions) | [Authority](HUYEN_LO_TECHNICAL.md#network-authority), [Save](HUYEN_LO_TECHNICAL.md#persistence) |
| Art / Controls | [§10](#gdd-10), [§11](#gdd-11) | [Decisions](HUYEN_LO_DESIGN_ANALYSIS.md#open-decisions) | [Art](HUYEN_LO_TECHNICAL.md#art-contract), [UI](HUYEN_LO_TECHNICAL.md#ui-notes) |
| Scope / Roadmap / Demo | [§12](#gdd-12), [§13](#gdd-13) | [Research](HUYEN_LO_DESIGN_ANALYSIS.md#research-ideas) | [Roadmap](HUYEN_LO_TECHNICAL.md#roadmap), [QA](HUYEN_LO_TECHNICAL.md#qa) |
