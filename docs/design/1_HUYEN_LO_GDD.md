# RPG Online: Huyền Lộ
## Tài liệu thiết kế game

**Current Design Version:** V6.1.0

**Status:** PRE-IMPLEMENTATION DESIGN  
**Last Reviewed:** 2026-09-30

<a id="gdd-0"></a>

# 0. Thẩm quyền và luật đã khóa

GDD giữ luật game; [Analysis](3_HUYEN_LO_DESIGN_ANALYSIS.md) giữ evidence / quyết định mở; [Technical](2_HUYEN_LO_TECHNICAL.md) giữ implementation contract.

User lock và review đã thống nhất được ghi vào GDD hiện hành; Analysis / Technical không tự đổi gameplay. Deterministic defect đã xác minh phải sửa đồng bộ; proposal và reference không tự thành luật.

| Phạm vi khóa | Luật |
| --- | --- |
| Nhân vật | Cap Lv 20; Tân Lữ nhập môn Lv 1–4; từ Lv 5 được chọn một trong hai class; một nhân vật nam |
| Build | Bốn thuộc tính, 95 điểm ở Lv 20, không branch cap, không Skill Rank |
| Combat | Active nhập môn → tiến cảnh đánh lan; hai nội tại / class; Băng Hàn chọn Đóng Băng hoặc Làm Chậm theo loại mục tiêu |
| Nội dung | 7 loại quái có level cố định trên 6 base rigs; Linh Biến modifier P0, 1 World Boss; 5 farm maps + 3 support zones |
| RPG | Food hồi phục chính, không tự hồi khi thiếu Food; Death có hậu quả; 6 ô × 3 bậc trang bị; I tối đa +4, II +6, III +8 |
| Online | Client–Server; Unity Dedicated Game Server quyết định gameplay; PvP 1v1 cược Vàng qua backend escrow; shared loot, Map Chat, MapId P0 |
| Asset / save | Một male modular rig, 64 × 64, PPU 32, 26 frames; Spring Boot + PostgreSQL lưu tiến trình và recovery checkpoint P0 |

**LOCKED:** luật khóa; **BASELINE:** số hiện dùng; **TUNABLE:** cần playtest. P0 bắt buộc; P1 sau core; P2 polish; DROP ngoài MVP. Research chưa chốt không thành requirement.

> **Đọc sâu:** [Design Analysis — quyết định mở](3_HUYEN_LO_DESIGN_ANALYSIS.md#open-decisions)

---

<a id="vision"></a>
<a id="gdd-1"></a>
<a id="gdd-12"></a>

# 1. Tầm nhìn và phạm vi

**Elevator Pitch:** RPG hành động 2D online ngang trên PC / Unity. Tân Lữ khám phá linh mạch, chọn Kiếm / Cung, tự phân bốn thuộc tính, gom quái đánh lan, nâng gear, săn Linh Biến / Boss và tỷ thí.

| Pillar | Người chơi cảm nhận | Dấu hiệu đạt |
| --- | --- | --- |
| Farm có nhịp | Gom quái rồi cleave / pierce / spread / explosion | Lv 5 single; Lv 10 học tiến cảnh max 3; Lv 17 đại chiêu |
| Build tự do | Đổi phân phối điểm để thử cách chơi | All-in không bị khóa progression; Tẩy Mạch sửa build |
| Progression hữu hình | Gear mới đổi cả stat và hình | Weapon / Armor / Pants đổi sprite |
| Hai class khác nhau | Kiếm áp sát; Cung giữ khoảng cách | Range, hit shape và control khác rõ |
| Online có ý nghĩa | Co-op farm / Boss, chat, challenge | Client kết nối tham gia gameplay thật |
| Scope hoàn chỉnh | Ít nội dung nhưng nối thành hành trình | Lv 1 → 20, ba chương, chính tuyến và vòng chơi sau truyện |

**Core Loop:** Quest chỉ đường → Food / Potion → vùng mở → đánh đơn rồi gom cụm / đánh lan → EXP / loot → build / gear → level gate → NPC. Linh Biến / Boss / PvP xen giữa chặng farm; Food / túi đồ tạo nhịp về làng.

Target Lv 20: 2,5–4 h gồm travel / quest / shop / run-back, chưa được playtest. **P0 acceptance: tối thiểu 2 concurrent players** (hai Client kết nối cùng Game Server); game online nhiều người, không đặt MaxPlayers = 2. Capacity 3–4+ concurrent players phải benchmark performance / network trước khi công bố.

## Phạm vi P0 / P1 / P2

| Hệ thống | P0 | P1 khi core ổn | P2 |
| --- | --- | --- | --- |
| Player / combat | Novice, hai class, bốn attributes / Tẩy Mạch, normal + 2 active + 2 nội tại / class, nhập môn / tiến cảnh qua bí kíp, đánh lan / Bỏng / Băng Hàn | Buff R: Chiến Ý / Ưng Nhãn Cường Hóa (P1); DPS Meter | Cosmetic polish |
| World | Năm bãi, cụm quái, bảy loại quái / sáu rigs, Linh Biến modifier, Boss basic + ba pattern / Cuồng Mạch | Linh Giáp / Vỡ Thế | Hazard, Boss polish |
| RPG | 18 dòng trang bị / 6 ô / 3 bậc, phẩm chất, I +4 / II +6 / III +8, shop / đồ rơi / túi / kho, Food / Death, chuyển giao cường hóa | Sắp túi; khóa đồ; Bùa Hồi Thành | Mua lại, mở rộng túi |
| Story / UI | Q1–Q12 với Q9 nhánh optional; ba Stage Summary; Journey; controls / HUD | Quest arrow, chat history | Extra cosmetics |
| Online / data | Unity Dedicated Game Server + Spring Boot + PostgreSQL; acceptance tối thiểu 2 concurrent players, MapId, co-farm không Party, chat / banner, shared loot ownership, lưu tiến trình an toàn | Observer hide / show; triển khai Internet công khai | Performance polish |

P1 chỉ triển khai sau core và quyết định scope; thông số proposal giữ tại Analysis.

**DROP:** Guild / Trade / Pet / Mount / Crafting / Auction / FreePK; nhiều tiền tệ; Skill Rank; cường hóa vượt giới hạn từng bậc (III không vượt +8); Channel / Zone; world chat / hạ tầng MMO nhiều cụm máy chủ; nợ EXP; hút HP / MP; Decoy; ghép đá; Hương EXP; hệ kháng / yếu nguyên tố. Không thêm thuộc tính hoặc hiệu ứng ngẫu nhiên ngoài luật hiện hành.

Roadmap / budget là giả định cần đo slice, không acceptance; xem Technical.

> **Implementation:** [Technical — spike / roadmap](2_HUYEN_LO_TECHNICAL.md#roadmap)

---

<a id="character-power"></a>
<a id="gdd-3"></a>

# 2. Sức mạnh nhân vật và tiến trình

Lv 20 cap; 5 điểm / level-up = **95 điểm**. Lv 1 chưa có điểm; Lv 2–4 auto +2 STR / +2 VIT / +1 INT, tổng 15. Lv 5 thêm 5 và hoàn 15 thành **20 unspent** một lần; chọn class Q6 rồi tự cộng. Chưa class không dùng skill / vũ khí class; kể cả Lv 5+ vẫn dùng normal Tân Lữ / Mộc Kiếm để làm Q5. Reset nhập môn xảy ra một lần khi đạt Lv 5; nếu trì hoãn Q6 tới Lv 6+, giữ mọi điểm level-up thêm, tổng spent + unspent = 5 × (L−1), không set lại 20. Q3 trao Mộc Kiếm; Lv 1–2 học NPC / movement, chưa bị yêu cầu combat.

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

Mob EXP theo §4. Sau onboarding farm là nguồn EXP / Vàng / đá / gear / material chính; quest dẫn đường / dạy mechanic / kể chuyện / mở nội dung. Reward / catch-up theo §5; QUEST-03 chỉ retune sau journey test. Giữ NeedEXP hiện tại trước khi đo HP / respawn mới. Lv 20 cap thật: CurrentEXP = 0, không tích hidden EXP / Lv 21; farm reward non-EXP vẫn hợp lệ. Demo trình diễn bằng profiles; `DebugExpMultiplier = 10` chỉ dev toggle, mặc định 1 ở release / demo.

## Công thức chỉ số nhân vật — BASELINE / TUNABLE

`Nền theo level + Thuộc tính + Trang bị → stat trước nội tại → stat cuối`; không hệ số class ẩn. **HP nền = 120 + 10 × (L−1), MP nền = 60 + 4 × (L−1).** VIT/INT và gear tiếp tục tăng MaxHP/MaxMP. Rarity/enhance tính ở §6 trước khi cộng; nội tại nền tảng Lv 5 nhân stat sau tổng này đúng một lần (§3). Giữ fractional values, UI mới làm tròn.

| Stat | Nền | Mỗi điểm thuộc tính |
| --- | --- | --- |
| HP | 120 + 10 × (L−1) | VIT +8 |
| MP | 60 + 4 × (L−1) | INT +5 |
| ATK | 12 + 1,2 × (L−1) | STR +0,70 |
| DEF | 5 + 0,6 × (L−1) | VIT +0,10 |
| ACC | 60 + 4 × (L−1) | AGI +6 |
| EVA | 20 + 2 × (L−1) | AGI +6 |
| SkillDamageBonus | 1 | INT +0,35%, chỉ direct skill, không normal / DoT |
| MoveSpeedMultiplier | 1 | AGI +0,05%; cộng thêm tốc chạy cố định từ Giày (§6) |

Bốn thuộc tính hiển thị: **Công Lực (STR), Sinh Lực (VIT), Linh Lực (INT), Thân Pháp (AGI)**. STR tăng direct damage; VIT giữ mạng; INT tăng MP và skill damage; AGI accuracy/evasion, mobility phụ. CritChance nền **5% cho mọi class/Tân Lữ + gear**, CritMultiplier 1,5; nội tại không cộng Crit ngầm.

`EvadeChance = 0.02 + 0.43*EVADefender/(EVADefender+2.5*ACCAttacker)`, tiệm cận 45%. All-in không bị khóa progression, không cam kết DPS ngang nhau hoặc đứng chịu ba quái. Chưa cộng điểm tại Lv 5 vẫn làm được Q5 bằng Mộc Kiếm; sau chọn class có normal và active nhập môn. Các mốc: nội tại nền tảng ở 5 → tiến cảnh ở 10 → nội tại tinh thông ở 13 → đại chiêu ở 17. Bảng gear và ATK quái đã đối chiếu lại với HP/MP theo cấp; playable test vẫn quyết định balance.

**Khi MaxHP/MaxMP thay đổi** do gear/cộng hoặc tẩy điểm: giữ HP/MP hiện có rồi clamp không vượt Max mới; không tự hồi theo tỷ lệ, không revive qua equip/reset. Hồi đầy vẫn qua nghỉ/hồi sinh đã quy định.

Tẩy Mạch Phù: 1.200 Vàng tại Tạ Minh, stock vô hạn; hoàn điểm về unspent, giữ class / level / gear / quest / learned skills. Reset Lv 5 một lần miễn phí. Evidence profiles và sustain ở [Analysis](3_HUYEN_LO_DESIGN_ANALYSIS.md#character-evidence).

---

<a id="class-combat"></a>
<a id="gdd-4"></a>
<a id="combat-status"></a>

# 3. Phái, kỹ năng và chiến đấu

| Class | Normal / interval / range | Identity |
| --- | --- | --- |
| Tân Lữ | 1,00 × / 1,00 s / cận chiến 1,2 u | Mộc Kiếm, chưa kỹ năng class |
| Kiếm | 1,00 × / 0,80 s / cận chiến 1,2 u | Áp sát, cung quét / đường kiếm, Bỏng |
| Cung | 0,95 × / 0,90 s / tầm bắn nền 6,5 u | Tầm xa mặc định, ba tên, Băng Hàn |

Nhịp đòn thường là **BASELINE / TUNABLE**: Tân Lữ chậm rõ để học ra đòn, còn 0,80/0,90 s của Kiếm/Cung cần kiểm cảm giác xen giữa các kỹ năng trên scene thật.

Hỏa của Kiếm và Băng của Cung là phong vị kỹ năng qua VFX / Bỏng / Băng Hàn; không có hệ khắc nguyên tố hay bảng kháng riêng.

**Hai active + hai nội tại/class.** Active đầu giữ một SkillId qua tiến cảnh; hotkey 1 thay profile sau học sách, không thêm slot/rank/điểm kỹ năng. Active cần level **và** learned manual flag. Nội tại tự học khi đủ **class + level**, kể cả chọn class muộn; có tên, icon, tooltip và mốc khóa/mở trong bảng skill. Không manual/hotkey/point/rank, không persist thêm cờ unlock nội tại.

| Class / mốc | Nội tại và tooltip — BASELINE / TUNABLE | Cách thể hiện |
| --- | --- | --- |
| Kiếm / Lv 5 | **Kiếm Tâm:** sau cộng nền, điểm và trang bị, MaxHP ×1,10 và DEF ×1,08. | Icon khiên/kiếm; tooltip nêu rõ +10% Máu tối đa, +8% Phòng thủ |
| Kiếm / Lv 13 | **Kiếm Thế:** kỹ năng Kiếm gây **+12% sát thương trực tiếp** lên mục tiêu có tâm vùng trúng đòn cách vị trí ra đòn đã chụp ≤1,2 u. | Icon kiếm áp sát; từng mục tiêu xét riêng, không tăng đánh thường/Bỏng |
| Cung / Lv 5 | **Ưng Nhãn:** sau cộng nền, điểm và trang bị, MaxMP ×1,10 và ACC ×1,08. **Không tăng tầm bắn.** | Icon mắt/tên; tooltip nêu rõ +10% Linh lực tối đa, +8% Chính xác |
| Cung / Lv 13 | **Xạ Tâm:** kỹ năng Cung gây **+12% sát thương trực tiếp** lên mục tiêu có tâm vùng trúng đòn cách điểm phóng đã chụp ≥4 u tại lúc trúng. | Icon tên xa; từng mục tiêu nổ xét riêng, không tăng đánh thường |

Nội tại hiện tên, icon, tooltip, trạng thái khóa/mở trong bảng kỹ năng; tự mở theo class + level, không sách/phím/rank/điểm. Kiếm Tâm/Ưng Nhãn là bonus stat **hiển thị**, không class multiplier ẩn; tính lại sau equip/reset/level rồi clamp HP/MP hiện có theo §2. Kiếm Thế/Xạ Tâm mirror cự ly gần/xa; chỉ áp trên hit trực tiếp của kỹ năng, một lần mỗi target, không thêm proc/combo/resource. Bốn icon tái dùng motif class. **Chiến Ý / Ưng Nhãn Cường Hóa** vẫn là buff R P1, xem [Analysis — candidates](3_HUYEN_LO_DESIGN_ANALYSIS.md#research-ideas).

## Bí kíp — nhận và học

| Milestone | Kiếm / Cung | Cách nhận và học |
| --- | --- | --- |
| Q6 / Lv 5 | Phong Trảm Kiếm Phổ / Linh Tiễn Cung Pháp | Staged grant sau class choice, trước cast objective; use unlock active nhập môn |
| Q8 reward | Phong Trảm Tiến Cảnh / Linh Tiễn Tiến Cảnh | Completion reward Lv 8; use cần Lv 10 + đã học nhập môn, thay profile trên cùng active slot |
| Q11 reward / Lv 17 | Kiếm Khí Chân Quyết / Hàn Tiễn Chân Quyết | Completion reward + Rare weapon; use cần Lv 17 + nhập môn, unlock đại chiêu; không bắt buộc đã học tiến cảnh |

Sáu manual IDs, guaranteed one-time / class-specific, stack 1 / quest-bound, không sell / drop / trade; có thể cất rương cùng character, learn từ bag; consume-on-learn. Chỉ học khi alive / idle, không pending cast; kiểm class / level / prerequisite, rồi atomically consume book + set learned flag theo SkillId / profile; Replay / duplicate learned reject không consume. **Bí kíp không có cooldown**; chống học lặp bằng learned flag và grant / learn receipt. Cooldown dưới đây thuộc kỹ năng được mở. Evolution giữ CD key: deadline mới = max(deadline cũ, lastCast + CD profile mới), không cấp lượt cast miễn phí. Chưa đủ Lv 10 giữ book, tooltip “Cần Lv 10”, không mất sách; không random book farm. Full bag theo reward preflight; Q6 staged pending không lock quest vĩnh viễn. Nội tại Lv 5/Lv 13 tự học theo bảng trên, không manual. NPC không thêm menu học riêng: dùng trong bag. HUD locked nói rõ level / manual / quest; Lv 10 có book chưa học vẫn nhập môn, tiến cảnh feedback ngắn khi use. Manual là power item thật, evidence chính tuyến vẫn virtual.

## Bộ kỹ năng — BASELINE / TUNABLE

| Class / profile | Lv / manual | Executor / shape | Power | Max | MP / CD | Status |
| --- | --- | --- | --- | ---: | --- | --- |
| Phong Trảm nhập môn | 5 / Q6 | Melee single 1,7 u | 1,20 × | 1 | 2 / 1,0 s | None |
| Phong Trảm tiến cảnh | 10 / Q8 | Arc 120°, 1,7 u | 1,35 × / target | 3 | 4 / 1,5 s | Bỏng 4% |
| Kiếm Khí | 17 / Q11 | Line 5,5 u, width 0,6 u | 2,80 / 2,60 / 2,40 / 2,20 / 2,00 × | 5 | 16 / 7 s | Bỏng 70% |
| Linh Tiễn nhập môn | 5 / Q6 | Single projectile 6,5 u, 1 arrow | 1,15 × | 1 | 2 / 1,0 s | None |
| Linh Tiễn tiến cảnh | 10 / Q8 | SnapshotSpread 6,5 u, 3 arrows | 0,90 / 0,80 / 0,70 × / arrow | 3 unique | 4 / 1,7 s | Băng Hàn: 2% Normal / 1% Linh; 2% Boss / PvP |
| Hàn Tiễn | 17 / Q11 | Primary 6,5 u + explosion radius 2 u | 2,80 × primary; 1,60 × secondary | 1 + 4 | 16 / 7 s | Băng Hàn: 45% Normal / 30% Linh; 100% Boss / PvP |

Tầm đánh trong skill profiles là tầm thực, không cộng thêm nội tại. Phong Trảm có cùng Bỏng 4% ở mọi level từ Lv 10; Kiếm Khí dùng cùng effect với 70% chance. Nội tại Lv 13 chỉ nhân sát thương trực tiếp khi đúng cự ly.

Target rank deterministic: primary theo cone / aim, rồi distance và stable instanceID. Cung cone ± 25° phía trước, không auto quay sau. Kiếm ưu tiên phía trước; trống thì auto-face target sau ≤ 1,2 u. Không target vẫn cast theo facing / aim, charge MP / CD, không tìm ngoài cone.

**Ba tên:** snapshot A / B / C khi ≥ 3 targets, A / B / A khi 2, A / A / A khi 1. Giữ arrow power theo index; không chia lại power. Target đã chết / invalid trước arrow spawn → tên đó bay theo hướng aim snapshot, không chuyển sang target khác; projectile đã bay không homing. Không target → cả ba theo cùng facing / aim, vẫn đúng 3 arrow. Game Server collision có thể hit mob khác đầu tiên trong allowed flight; mỗi arrow tối đa 1 hit. Snapshot không bảo đảm trúng. Status chỉ xét một lần cho mỗi unique target **thực nhận landed hit / cast**, không 3 rolls vào A / A / A. Primary Hàn không nhận explosion lần hai; line theo thứ tự đường đánh.

## Sát thương, nhịp đòn và trạng thái

ATK/INT skill bonus/ACC/Crit và nội tại nguồn chụp tại cast start; target DEF/EVA và vị trí/hurtbox xét tại hit/impact. Đổi gear/level giữa action không sửa damage của action đã phát.

Normal raw = FinalATK × power; skill raw = FinalATK × power × SkillDamageBonus và bonus nội tại có điều kiện. Trong match PvP, nhân raw theo §8 trước DEF/Crit/random; PvE không dùng hệ số đó. Game Server validate → Evade → Crit → DEF / random → HP: `Damage=max(1,round(Raw*100/(100+TargetDEF)*Random(0.95,1.05)*CritMultiplier))`; miss 0 / NÉ. `round(x)=floor(x+0.5)` cho x ≥ 0. ActualHpLost = min(calculatedDamage, remainingHP) vào threat / contribution, không overkill.

Một action lock chung; CD / MP commit tại cast start, normal cũng có CD riêng. Các mốc tính từ cast start — BASELINE / TUNABLE:

| Action | Hit / projectile spawn | Action lock |
| --- | --- | --- |
| Normal | +0,10 s | 0,26 s |
| Nhập môn single (hai class) | +0,12 s | 0,30 s |
| Phong Trảm tiến cảnh | +0,14 s | 0,30 s |
| Linh Tiễn tiến cảnh | Ba tên: +0,12 / +0,15 / +0,18 s | 0,34 s |
| Kiếm Khí | +0,16 s | 0,40 s |
| Hàn Tiễn | Spawn +0,18 s | 0,40 s |

Projectile damage tại Game Server impact. Death / portal / invalid generation hủy pending hit / spawn, không hoàn cost; projectile đã spawn tiếp tục trong MapId gốc tới hit / expiry, không chuyển theo caster. AnimationEvent chỉ visual. ART-01 tune timings, chạy lại model khi đổi.

**Status application:** Bỏng và Băng Hàn mỗi loại có tối đa một roll theo `(actionId,actualTargetId,effectId)` tại landed hit đầu tiên; cache cả fail, không reroll trên A/A/A. **Băng Hàn là một application**, chọn kết quả theo loại target trước roll; không roll Làm Chậm và Đóng Băng độc lập.

| Effect / target | Lifecycle — BASELINE / TUNABLE |
| --- | --- |
| Bỏng / quái thường, Linh Biến, Boss | 6 s, tick mỗi 1 s; raw/tick = 0,06 × ATK nguồn đã chụp, qua DEF hiện tại; không Crit / random / INT. Một Bỏng/target. Proc lại thay source/ATK snapshot, refresh **về đủ 6 s từ lúc proc**, giữ nhịp tick hiện có; không cộng dồn sát thương hoặc kéo duration bằng phép cộng. PvP immune. |
| Băng Hàn / quái thường | Chỉ **Đóng Băng 1,5 s**: Linh Tiễn 2%, Hàn Tiễn 45%. Không Làm Chậm. |
| Băng Hàn / Linh Biến | Chỉ **Đóng Băng 1,5 s**: Linh Tiễn 1%, Hàn Tiễn 30%. Khác quái thường ở chance, không ở duration. |
| Băng Hàn / Boss | Chỉ **Làm Chậm 3 s**: tốc chạy khi đổi vị trí ×0,85; **đồng hồ chờ action kế tiếp chạy ở 75% tốc độ**. Linh Tiễn 2%, Hàn Tiễn 100%. Không Đóng Băng. |
| Băng Hàn / người chơi PvP | Chỉ **Làm Chậm 1,5 s**: MoveSpeed ×0,75. Linh Tiễn 2%, Hàn Tiễn 100%; không ảnh hưởng tốc đánh, hồi chiêu, animation hoặc action đang cast. Không Đóng Băng. |

Đang Đóng Băng không roll/apply Băng Hàn; tan băng được miễn Đóng Băng **3 s trên cùng target**, N Cung cùng dùng một deadline, không chain-freeze. Đóng Băng khóa di chuyển/AI đánh và hủy windup/hit/spawn chưa giải quyết; projectile đã bay vẫn sống, target không nhận thêm damage. Boss/người chơi không Đóng Băng. Làm Chậm chỉ refresh deadline về `now + duration`, không stack magnitude. Boss còn 1,5 s chờ action thì debuff **không reset về full CD**: chỉ giảm tốc đếm phần thời gian còn lại; hết debuff đếm lại 100%. Action/telegraph/projectile **đã bắt đầu** giữ nguyên mốc và tốc độ. Cuồng Mạch đổi future base cadence, Băng Hàn chỉ tác động đồng hồ chờ sau khi đã chọn cadence đó; nhiều Cung không nhân nhiều lớp slow. Một target chỉ dùng đúng effect của category.

Nhiều Kiếm cùng đánh chỉ refresh một Bỏng trên target: tick đầu không bị đẩy lùi mãi, source/ATK snapshot/expiry thay khi proc, tick đúng expiry trước remove. Đòn trực tiếp của Kiếm Thế chỉ nhận +12% nếu đúng khoảng cách từ action origin; Xạ Tâm dùng projectile origin và vị trí actual target tại impact. Hai nội tại tinh thông không đổi status chance/magnitude. DoT credit source thực; source chết không xóa Bỏng đã áp. Eligibility tại death theo §6; không active Buff P0.

Player–Monster không contact damage / body blocking; Monster–Monster không shoving. Chỉ light separation steering nếu cần readability, không formation runtime hoặc extra stun / knockback. Melee originY + 0,8 u, vertical hitbox PHY-01.

---

<a id="world-farm"></a>
<a id="gdd-5"></a>

# 4. Thế giới và bãi farm

**World route:** Vân Khê ↔ Đồng Sương ↔ Trúc Ảnh ↔ Bạch Vân ↔ Xích Nham ↔ Huyền Tích. Vân Khê nối Học Viện và Lôi Đài. Q11 mở Huyền Môn từ Xích Nham. Không fast travel ra bãi; Bùa Hồi Thành P1 chỉ đưa về làng.

Mỗi map farm/combat có **một SafeAnchor cố định** để khôi phục khi phiên chơi đã mất; người chơi không xuất hiện lại giữa bãi quái. Vân Khê và Học Viện là khu an toàn: có thể khôi phục tọa độ đã lưu nếu còn hợp lệ, nếu không thì dùng SafeAnchor. Lôi Đài không có điểm khôi phục phiên; trận PvP theo lifecycle riêng (§8).

| Vùng | Hình thái và mục đích |
| --- | --- |
| Vân Khê | Hub yên bình: NPC, mua thuốc, rèn đồ, rương và nghỉ; đường về đọc được từ portal. |
| Học Viện | Khu nhập môn với ledge / drop-through và bãi Bù Nhìn; thao tác lớp học dùng NPC / terrain hiện có. |
| Lôi Đài | Không gian tỷ thí 1v1, tách khỏi farm world bằng match membership. |
| Đồng Sương | Đồng thoáng, 1–2 tầng; thấy quái và lối thoát sớm, chưa ép gom đông. |
| Trúc Ảnh | Rừng trúc, cầu và 2–3 tuyến cao độ; tập xoay bãi, nhảy / kite, lần theo vết ấn dưới cầu. |
| Bạch Vân | Ba bậc địa hình bên thác, route vòng; nhóm ba bắt đầu làm tiến cảnh có giá trị. |
| Xích Nham | Hẻm núi rộng nhất, hai nhánh nhập lại; ngoại vi luyện công rồi đi sâu tới ba Mạch Ấn / Huyền Môn. |
| Huyền Tích | Ngoại vi phế tích dẫn tới landmark Cự Thú; khoảng trống Boss tách normal spawn để đọc telegraph. |

## Mở bản đồ, mật độ và cụm quái cố định — TEST / TUNABLE

Mỗi điểm sinh quái chọn một **mob identity có level cố định**, vị trí và cụm. Level thuộc identity, `SpawnSlot.level` chỉ cache/validation bằng level đó; không author cùng loài lên nhiều level. Respawn giữ nguyên identity/level. Linh Biến chỉ thêm modifier; cụm là bố trí bãi, không formation hoặc tổ đội runtime.

| Map / gate | Pocket budget | Layout | Traversal không combat |
| --- | --- | --- | --- |
| Đồng Sương / onboarding | DS1–DS6: 6 cụm, 10 slots | Sparse 1–2, hai lanes dễ nhìn | 25–35 s |
| Trúc Ảnh / Q6 Completed + Lv 5 | TA1–TA6: 6 cụm, 13 slots | Chủ yếu 2, một pocket 3; bridge / vertical spacing. `TA4.slot1` cho Q8 | 35–55 s |
| Bạch Vân / Q8 Completed + Lv 8 | BV1–BV5: 5 cụm, 13 slots | Từ 2 → 3; tiến cảnh Lv 10 có ích ở BV3 | 35–55 s |
| Xích Nham / Q8 Completed + Lv 12 | XN1–XN6: 6 cụm, 17 slots | Mixed 2–4, line / vertical split; không tất cả fit arc | 35–55 s |
| Huyền Tích / Q11 Completed | HT1–HT5: 5 cụm, 13 slots | Chủ yếu 2–3; Boss landmark tách normal spawn | 45–60 s |

**28 cụm / 66 điểm sinh quái — TEST/TUNABLE.** Giữ budget hiện tại, phân lại identity để có đường farm liên tục; Trúc có 3 cụm Sói Sương Lv 4 làm bước đệm trước Sói Trúc Lv 8. Quest anchors giữ vị trí. Đây là budget authoring, không capacity promise; contention theo [Analysis §3](3_HUYEN_LO_DESIGN_ANALYSIS.md#farm-progression).

5 farm + Village / Academy / Arena = 8 logical roots. Huyền Tích vào từ Lv 17, khuyến nghị 18; Q12 Lv 20 là quest gate, Boss không gate 20. Village / Academy mở onboarding; Arena qua Challenge. Old maps luôn quay lại được; farm map không tự nhận quest.

**Fixed monster ladder — canonical:** 7 identities, 6 base sprite/AI rigs; Sói Trúc Ảnh reuse Sói Sương bằng palette. Hai Sói xám lạnh/lục tối có nameplate riêng; Linh dùng aura tím/ấn sáng chung, không dùng màu sói làm dấu Linh.

| Mob identity | Lv | HP | ATK | EXP | Gold | Visual / AI reuse |
| --- | ---: | ---: | ---: | ---: | --- | --- |
| Nấm Linh | 2 | 48 | 9 | 15 | 7–12 | Base rig |
| Sói Sương | 4 | 107 | 13 | 22 | 11–18 | Base rig |
| Sói Trúc Ảnh | 8 | 339 | 22 | 38 | 19–30 | Sói rig; palette lục tối |
| Ong Giáp | 10 | 473 | 27 | 47 | 23–36 | Base rig |
| Đoạt Mạch Đạo Tặc | 13 | 704 | 36 | 63 | 29–45 | Base rig |
| Xích Thạch Linh | 16 | 974 | 45 | 81 | 35–54 | Base rig |
| Cổ Môn Vệ Binh | 20 | 1393 | 60 | 108 | 43–66 | Base rig |

Level là nhận diện nội dung; không tạo thêm variant chỉ để mỗi level có một quái. Số bảng derive từ formula dưới; `MobDefinition.fixedLevel` và manifest phải khớp.

**Spawn manifest — canonical authoring data:** mỗi dòng là một cụm; level cố định trên từng slot, không random trong khoảng. `TA4.slot1` là slot đầu của TA4. ID cụm là authored ID ổn định, không index player hoặc live instance.

| Cụm | Mob identity | Level | Slots |
| --- | --- | ---: | ---: |
| DS1 | Nấm Linh | 2 | 1 |
| DS2 | Nấm Linh | 2 | 1 |
| DS3 | Sói Sương | 4 | 2 |
| DS4 | Sói Sương | 4 | 2 |
| DS5 | Sói Sương | 4 | 2 |
| DS6 | Sói Sương | 4 | 2 |
| TA1 | Sói Sương | 4 | 2 |
| TA2 | Sói Sương | 4 | 2 |
| TA3 | Sói Sương | 4 | 2 |
| TA4 | Sói Trúc Ảnh | 8 | 2 |
| TA5 | Ong Giáp | 10 | 3 |
| TA6 | Sói Trúc Ảnh | 8 | 2 |
| BV1 | Ong Giáp | 10 | 2 |
| BV2 | Ong Giáp | 10 | 2 |
| BV3 | Đoạt Mạch Đạo Tặc | 13 | 3 |
| BV4 | Đoạt Mạch Đạo Tặc | 13 | 3 |
| BV5 | Đoạt Mạch Đạo Tặc | 13 | 3 |
| XN1 | Đoạt Mạch Đạo Tặc | 13 | 2 |
| XN2 | Đoạt Mạch Đạo Tặc | 13 | 3 |
| XN3 | Đoạt Mạch Đạo Tặc | 13 | 2 |
| XN4 | Xích Thạch Linh | 16 | 3 |
| XN5 | Xích Thạch Linh | 16 | 3 |
| XN6 | Xích Thạch Linh | 16 | 4 |
| HT1 | Xích Thạch Linh | 16 | 2 |
| HT2 | Cổ Môn Vệ Binh | 20 | 3 |
| HT3 | Cổ Môn Vệ Binh | 20 | 2 |
| HT4 | Cổ Môn Vệ Binh | 20 | 3 |
| HT5 | Cổ Môn Vệ Binh | 20 | 3 |

Safe Entrance 6–8 u → pockets → alternate path / vertical route → landmark → exit; không maze / moving platform / hazard P0. Aggro 5 u / leash 8 u, tâm cụm khoảng 18–20 u hoặc terrain tách tương đương. Trong pocket author sparse / line / split phù hợp shape; không áp spacing 0,8–1,5 u cho mọi cụm. Melee Sói tự converge khi chase; Ong giữ hover band reachable bằng Kiếm, không blob cố định. Hit alert chỉ đánh thức cụm, không truyền sang cụm khác; attack offset 0–0,35 s. Vượt leash Return tạm immune, về spawn full HP.

Farm theo bãi gần level trong khoảng thưởng §6; mob cao hơn có reward nếu trong khoảng nhưng không đảm bảo an toàn. Mob identity/level quyết định gear band; map quyết định material/flavor. **[Farm Matrix Lv 1–20](3_HUYEN_LO_DESIGN_ANALYSIS.md#farm-progression)** giữ derived HP / EXP / Gold, không copy stats table vào GDD. Measure solo / 2 / 3 / 4 players, wait / crowd / CPU / network và run-back trước capacity claim; low-level pockets ít slot có thể cần rotate, không thêm Party / Channel.

## Chiến đấu và vòng đời quái

| Normal archetype | AI | Move u / s | Melee / ranged u | Interval | Projectile |
| --- | --- | ---: | --- | ---: | --- |
| Nấm Linh | Melee only | 1,2 | 0,8 / — | 1,8 s | — |
| Sói Sương / Sói Trúc Ảnh | Melee only; chase nhanh | 2,4 | 1,0 / — | 1,3 s | — |
| Ong Giáp | Ranged / Flying | 2,0 | — / 5 | 1,6 s | Generic, speed 5,5 u / s |
| Đoạt Mạch Đạo Tặc | Hybrid | 2,2 | 1,2 / 5 | 1,5 s | Generic, speed 5 u / s |
| Xích Thạch Linh | Hybrid | 1,4 | 1,1 / 4,5 | 2,0 s | Generic, speed 4 u / s |
| Cổ Môn Vệ Binh | Hybrid | 1,8 | 1,4 / 6 | 1,6 s | Generic, speed 6 u / s |

Hybrid reachable → Chase / Melee; ranged khi platform không reach / vertical / chase blocked-timeout, không chỉ vì distance>melee. Ong FlyingBox ~6 × 3 u là phạm vi roam, không hover height cố định. Ong phải approach vào vertical melee band khi engage, không treo mãi ngoài tầm Kiếm; hover / approach timings PHY-01 / ART-01. Một Linh Đạn generic đổi scale / tint / speed / trail; không projectile / animation riêng từng loài.

Stat normal tại level L (`round(x) = floor(x + 0.5)` cho x ≥ 0, kể cả EXP / final Damage): `HP = round(40 + 1.04*L*L*L)` khi L ≤ 5; L>5 đặt `x = L - 5`, `HP = round(170 + 50*x + 2.1*x*x)`; `DEF = round(2 + 0.8*L)`; `ATK = round(6 + 1.5*L + 0.06*L*L)`; `ACC = 60 + 4*L`; `EVA = 20 + 2*L`; `NormalEXP = round(10 + 2.5*L + 0.12*L*L)`; `GoldMin = 3 + 2*L`; `GoldMax = 6 + 3*L` (integer uniform inclusive). ATK mid/late tăng để bù một phần HP player tăng theo cấp; HP mob giữ theo TTK probe. Một curve chung, chỉ evaluate ở bảy identity-levels đã author; hp / atk multipliers mặc định 1.0, chỉ tune có evidence sau playtest. Normal / variant slot respawn **25 s BASELINE / TUNABLE**, test 20 / 25 / 30 s, tính từ death; không timer variant riêng. Mục tiêu vòng bãi: clear A → nhặt → B / C / D → quay lại, không đứng nguyên một pocket đợi respawn.

TTK target cùng level + Common + 0: early 2–4 s, mid 3–6 s, late 4–8 s TEST, không guarantee mọi build. Đánh Thạch Lv 16 khi player15 trước đại chiêu còn là probe chậm; giữ HP curve, không tăng mọi HP chỉ để kéo giờ chơi. Normal solo / 2–4-target / Q8 Linh và gear-lag probes ở Analysis.

**Mob attack contract:** đi qua aggro radius vẫn bị acquire / chase, body overlap không gây damage. Melee: Acquire → Chase → attack range → Face → Windup / lock facing → HitMoment / front hitbox → Recovery. Target chạy xuyên ra sau / nhảy ra khỏi vertical range / rời hitbox trước HitMoment thì MISS; không guaranteed damage vì animation đã start, không quay 180° giữa swing. Ranged / Hybrid: Acquire → Aim / Windup → projectileSpawnMoment → Game Server projectile → collision → damage; AnimationEvent chỉ visual. Mob normal attack power 1.0, CritChance 0 P0; formula Damage chung, exact hitbox / windup tại PHY-01 / ART-01.

**Mob / Linh Biến threat:** một `Threat[playerId]` và `Contribution[playerId]` riêng mỗi mob. Initial acquire nearest valid player hoặc attacker đầu tiên; direct / DoT cộng ActualHpLost (cap overkill, dedup), không raw damage. Sticky target: challenger có threat>0 và ≥ 1,25 × current mới đổi; current invalid / dead / disconnect / khác MapId / out-of-leash thì chọn highest valid threat, tie playerId; nếu không có threat chọn nearest valid trong aggro. Báo động cụm chỉ wake, từng mob tự acquire / resolve. Return về spawn full HP, clear threat / contribution / status, hủy pending action; không giữ damage từ lượt kéo trước. Linh Biến dùng cùng resolver, không nearest-only sau acquire.

**Linh Biến P0 — modifier trên normal slot:** dynamic roll `LinhBienChance = 0.05` (**5% TEST / TUNABLE**) chỉ tại spawn / respawn của mob **Lv 8+**, cap `MaxActiveLinhBienPerMapId = 1`. Lv 1–7 không roll, không tiêu RNG rồi upgrade level; initial population dùng cùng arbitration. Return / root hide / reconnect không reroll. Khi chết, slot dùng deadline 25 s như normal; lần spawn sau mới xét variant.

| Modifier | BASELINE / TUNABLE |
| --- | --- |
| Stat | HP × 5; ATK × 1,3; DEF / ACC / EVA / MoveSpeed giữ base |
| Reward | EXP × 3; Gold × 3; physical loot theo Linh Biến profile §6, không nhân EXP / Gold × 5 |
| Visual / count | Same sprite / animation / AI / projectile / threat; scale ~1,20–1,30, aura / name / HP bar; chiếm một normal slot, không thêm mob identity / rig |

**Q8 force encounter:** requester đang đúng quest step, dùng `TA4.slot1` Sói Trúc Ảnh Lv 8 hiện có. Existing Sói Lv 8 Linh Biến thì bind. Nếu map đã có Linh Biến khác, chờ nó chết / cap trống; **không demote / despawn** kể cả nó đang idle. Cap trống thì promote Sói idle / full HP tại spawn, hoặc chờ Return / next respawn 25 s; không reset mob giữa fight. Reservation suppress random promotion mới khi có requester đúng step trong map. HUD hiện variant đang giữ cap / slot chờ; không phụ thuộc roll 5% nhưng không hứa zero wait khi world đang combat.

N requesters dùng chung encounter, không entity per player. Quest credit 20% mỗi requester; người không đủ credit còn pending, retry tại slot lifecycle, request replay không nhân encounter. Rời map / disconnect giữ quest progress, không force cho absent player. Có credit thì bỏ request; hết requester hiện diện thì random rule tiếp tục, variant sống vẫn đi hết lifecycle. **Mọi Sói Linh Biến Q8 dùng cùng reward profile Linh Biến**, phân phối contribution / loot như §6; entitlement chỉ điều khiển force / quest credit.

**Một World Boss chung:** Lv 20 Huyền Nham Cự Thú, HP **32.000**, ATK **160**, DEF **25**, ACC **140**, EVA **60** — BASELINE. Ngay trong Huyền Tích, không scene / story instance / cổng arena / gate Lv 20 riêng. Q11 hoàn thành mở toàn map; người đi ngang thấy trận đánh. Landmark có khoảng trống đọc vùng báo trước đòn, BossCombatArea radius 15 u TEST; không đặt quái thường trong vùng đánh Boss.

World mới Boss có sẵn; sống thì không spawn thêm; chết đặt nextSpawn = deathUtc + 15 phút; tới deadline chỉ spawn nếu không có entity. Reset khi wipe không coi là death, không roll loot / đổi deadline. Demo Boss có sẵn, respawn 60 s override rõ; release target 90–150 s trong benchmark đầu với hai endgame players, không auto-scale HP theo số người hoặc giới hạn world ở hai người.

| Pattern | Power | Telegraph | CD / interval | Shape |
| --- | ---: | ---: | ---: | --- |
| Basic | 1,00 × | — | 1,8 s | Melee |
| Nham Trảo | 1,20 × | 0,5 s | 2,5 s | Frontal cone |
| Địa Chấn | 1,50 × | 1,0 s | 6 s | Ground AoE, nhảy né |
| Nham Thạch Rơi | 1,80 × | 1,2 s | 8 s | Ba vùng đất, đặt quanh tối đa ba vị trí người chơi hợp lệ; mỗi người chỉ chịu tối đa một vùng trong một lần tung |

Boss chỉ bắt đầu **một action tại một thời điểm**: chọn kỹ năng đã hết hồi chiêu theo thứ tự Địa Chấn → Nham Thạch Rơi → Nham Trảo, nếu chưa có thì đánh thường. Hồi chiêu tính từ lúc bắt đầu action; vùng báo đòn đã phát không bị action mới chen vào. Ba vùng Nham Thạch Rơi không chồng hitbox gây double-hit; khi chỉ có một/hai mục tiêu, vẫn ba vùng tách nhau theo anchor quanh vị trí hợp lệ, không nhắm một người ba lần. Cuồng Mạch đổi nhịp action **sau** action hiện tại; Băng Hàn chỉ giảm tốc đồng hồ chờ action tiếp theo. Độ rộng vùng, khoảng cách né và nhịp telegraph cần kiểm trên scene thật, không đổi quy tắc này khi chưa có evidence.

Boss threat = ActualHpLost, highest alive valid trong BossCombatArea; retarget invalid hoặc khoảng 1 s, không dùng sticky normal multiplier. Không ai alive trong area liên tục 10 s → reset HP / position / threat / contribution / status / phase. Corpse không ngăn reset. **Cuồng Mạch P0** một lần khi HP ≤ 30%: tint / glow / roar / local shake, future cadence × 0,8 TEST (CD / interval giảm 20%), không tăng ATK / cắt telegraph / reschedule action đang chạy. Linh Giáp / Vỡ Thế P1.

Boss eligibility / corpse / EXP / Gold / pile / Journey theo **§6 reward contract**; Q12 dùng active-step damage (§5), không personal set. Damage clear khi reset. Sau chính tuyến: Huyền Tích / Linh → Rare / Epic → enhance → shared Boss / Dư Ảnh → build / PvP; không daily / dungeon mới.

> **Implementation:** [Technical — timer và Boss lifecycle](2_HUYEN_LO_TECHNICAL.md#timers)

---

<a id="quests-story"></a>
<a id="gdd-2"></a>
<a id="gdd-6"></a>

# 5. Cốt truyện và nhiệm vụ

Vân Khê nằm trên những Mạch Ấn ngầm, nơi linh khí nuôi rừng núi và giữ phế tích yên giấc. Gần đây, gió núi mang mùi tanh, nấm mọc khác thường, sói bỏ bãi cũ. Tân Lữ là người dự tuyển, lần theo những dấu nhỏ ấy trong lúc học cách tự giữ mình; không có lời tiên tri hay danh phận cứu thế. Lễ Nhập Lộ tại Lv 5 gắn lựa chọn kiếm / cung với việc chính thức bước vào đường tu luyện, bằng lời NPC và thao tác Q6, không cinematic hay quest mới.

| Trụ cột thế giới | Điều được hé lộ |
| --- | --- |
| Mạch Ấn và linh khí | Mạch Ấn bị can thiệp làm dòng linh khí lệch hướng, sinh trọc khí khiến sinh vật hung dữ. Ghi chú quest, tên vật phẩm và mô tả quái nối từng dấu vết. |
| Đoạt Mạch Đạo Tặc | Chúng đục phá Mạch Ấn để lấy linh thạch: lợi trước mắt của con người làm rối trật tự tự nhiên. Biết có đạo tặc chưa đủ kết luận nguồn gây nhiễu. |
| Cự Thú và Dư Ảnh | Huyền Nham Cự Thú là sinh linh thủ hộ cổ xưa bị trọc khí ăn mòn. Hạ nó giúp giải thoát Thủ Vệ; Dư Ảnh là tàn niệm linh lực còn đọng nơi cấm địa. |

Lời NPC ngắn, mộc mạc; không diễn giải hết bí ẩn. Lâm Bá kiệm lời, ấm áp, nhắc đường về; Bách Luyện cộc nhưng trọng người bền chí; Yên Thảo nghiêm về khí huyết và giữ mạng; Tạ Minh thâm trầm, chỉ dẫn đại cục; Hạo Vũ sảng khoái, lấy tỷ thí làm lời chào. Những sắc thái này dùng text và nội dung hiện có, không thêm quest/NPC/asset chỉ để kể chuyện.

| Chương | Dải cấp | Sắc thái và diễn tiến | Checkpoint tổng kết |
| --- | --- | --- | --- |
| I — Dấu Nứt Vân Khê | 1–7 | Nhập môn & sinh tồn: làng còn yên, điềm lạ thoáng qua; tự cầm kiếm, dùng thuốc, rèn món đầu tiên | Q7 completed và Lv ≥ 7 |
| II — Theo Dấu Huyền Lộ | 8–17 | Dấn thân & khám phá: lần theo trọc khí; Lv 12 tu luyện ngoại vi Xích Nham, Lv 15 điều tra sâu, Lv 17 phục hồi Huyền Môn | Q11 completed và Lv ≥ 17 |
| III — Huyền Tích Thức Tỉnh | 18–20 khuyến nghị | Thanh tẩy & vấn đạo: phế tích trang nghiêm, Thủ Vệ bị cuồng hóa; phong ấn ổn định sau trận chiến | Q12 completed và Lv 20 |

**Main Story Complete — HOÀN THÀNH CHÍNH TUYẾN:** Q12 khép lại chính tuyến / Chương III. Summary: **CHƯƠNG III HOÀN THÀNH / CHÍNH TUYẾN ĐÃ HOÀN THÀNH** — Tạ Minh: “Tai ương tạm lắng. Đường tu luyện còn dài.” Tiếp tục farm Huyền Tích, săn đồ Rare / Epic, nâng đồ III lên +8, săn Linh Biến / Dư Ảnh, tỷ thí và thử build; P1 chỉ có khi được triển khai.

Q12 **per character**: chưa complete hiển thị **Huyền Nham Cự Thú**, đã complete **Dư Ảnh Huyền Nham**, kể cả tracker / banner. Hai tên dùng một entity / sprite / AI / drop, không world story flag.

> **Đọc sâu:** [Design Analysis — review narrative](3_HUYEN_LO_DESIGN_ANALYSIS.md#review-decisions)

## Q1–Q12 và nhiệm vụ

Q1–Q4 catch-up tới Lv 2 / 3 / 4 / 5, `CatchUp = max(0, TargetCumulativeEXP - CharacterTotalEXP)`; BaseEXP = 0 khi đã qua mốc. Q5 / Q7–Q11 EXP hiện dùng 10% thanh cấp (half-up), Q6 = 0, Q12 = 0 ở cap; các số là BASELINE / TUNABLE cho QUEST-03. **Giữ 12 QuestId**: các bước ngắn đầu game phục vụ onboarding/recovery, Q9 là nhánh tùy chọn và không chặn Q10. Không thêm ID chỉ để lấp khoảng farm.

**State:** LOCKED → AVAILABLE khi đủ prerequisite + level → nhận → IN_PROGRESS → đủ active objectives → READY_TO_TURN_IN → đúng NPC/range → COMPLETED + reward/unlock. Ready không auto-turn-in/teleport; thiếu cấp thì NPC/HUD chỉ rõ mốc và bãi phù hợp. Cổng cũ vẫn quay lại được.

**Markers/regions:** mỗi ID dưới là key của trigger/interact anchor trên map/NPC hiện có; Game Server kiểm MapId/vị trí. Visit radius 1,5 u TEST, E trong 2 u TEST. Waypoint Huyền Môn đang khóa vẫn interact được từ phía ngoài Xích Nham ở Q11. Dấu `+` trong objectives chỉ các mục tiêu cùng active group, không phải thêm QuestId.

| Quest / Lv | Story + người giao → trả | Objectives theo thứ tự | Reward | Unlock / next |
| --- | --- | --- | --- | --- |
| Q1 — Người mới đến Vân Khê / 1 | Lâm Bá: “Nhớ chỗ thuốc, lò rèn và đường về. Ra núi rồi, chẳng ai giữ hộ mạng mình.” Lâm Bá → Lâm Bá. | Nói chuyện Yên Thảo → Bách Luyện → Mộc An → báo Lâm Bá. | Catch-up Lv 2 + 50 Vàng | Q2 sau Q1 + Lv 2 |
| Q2 — Bước chân đầu tiên / 2 | Lâm Bá: “Đi thử một vòng. Chân vững rồi hãy cầm kiếm.” Lâm Bá → Lâm Bá. | `HV_Entrance` → nhảy tới `HV_JumpLedge` → S + Space xuống `HV_DropLanding` → qua portal thật về Vân Khê → báo Lâm Bá. | Catch-up Lv 3 + 75 Vàng | Q3 sau Q2 + Lv 3 |
| Q3 — Vũ khí trong tay / 3 | Phong Du: “Đừng vội. Giữ thế cho chắc, rồi ra đòn.” Phong Du → Phong Du. | Nhận/mặc Mộc Kiếm → `HV_DummyYard` → hạ 3 Bù Nhìn, mỗi con đóng góp ≥20% → báo Phong Du. Dummy HP 60, không đánh/trả thưởng; hồi sinh 25 s TEST. | Mộc Kiếm cấp trước một lần; turn-in catch-up Lv 4 + Quần Thanh Mộc I | Q4 sau Q3 + Lv 4 |
| Q4 — Sinh tồn ngoài làng / 4 | Yên Thảo: “Ăn trước khi đi. Thuốc để dành lúc cần.” Yên Thảo → Yên Thảo. | Ứng 320 Vàng một lần → mua Food I, HP Potion I, MP Potion I mỗi loại 1 → dùng Food bằng F → `DS4_ExitTrail`, hạ **5 Sói Sương Lv 4 trong DS3–DS6** → báo Yên Thảo. H dùng khi thiếu HP, không bắt dùng lúc đầy. | Turn-in catch-up Lv 5 | Q5 sau Q4 + Lv 5 |
| Q5 — Chiến lợi phẩm đầu tiên / 5 | Bách Luyện: “Thứ mặc được thì giữ. Thứ thừa đem bán, lấy đồng lộ phí.” Bách Luyện → Bách Luyện. | `DS2_MushroomPatch`: hạ 1 Nấm Lv 2 khi đúng step, đóng góp ≥20% → E nhận Áo Thanh Mộc + Nấm Sương tutorial → mặc áo → bán sample cho Bách Luyện → báo Bách Luyện. | Áo I + sample cấp theo bước; turn-in 45 EXP | Q6 sau Q5 + Lv 5 |
| Q6 — Lễ Nhập Lộ / 5 | Tạ Minh: “Kiếm hay cung, tự ngươi chọn. Đã chọn thì phải học giữ hơi thở.” Tạ Minh → Tạ Minh. | `HV_ClassHall`: chọn class (điểm đã được trả lại một lần khi lên Lv 5) → nhận weapon I + bí kíp nhập môn; mặc weapon + xác nhận đã cộng ≥1 điểm + học sách → cast active ở `HV_DummyYard` → nhận MP Potion I dự trữ, bấm M khi thiếu MP → báo Tạ Minh. Bảng skill hiện nội tại Lv 5 mở/Lv 13 khóa. | Class, weapon + bí kíp theo bước, MP Potion I; 0 EXP | Q7 sau Q6 + Lv 7; mở Trúc Ảnh |
| Q7 — Tinh Thạch đầu tiên / 7 | Bách Luyện: “Một nhát búa không thành đồ tốt. Cứ làm cho đều tay.” Bách Luyện → Bách Luyện. | Chọn nhẫn I đang sở hữu (thiếu mới cấp) → preview → nâng +0→+1 hoặc xác nhận nhẫn đã ≥+1 → mặc/xác nhận đúng nhẫn → báo Bách Luyện. | Nhẫn I nếu thiếu; 1 đá + 100 Vàng dự trữ chỉ khi cần nâng; turn-in 83 EXP | Q8 sau Q7 + Lv 8; hết Chương I |
| Q8 — Bóng sói trong Trúc Ảnh / 8 | Lâm Bá: “Vết cào này lạ. Xem dưới chân cầu có gì.” Mảnh ấn mang vết đục của người. Lâm Bá → Lâm Bá. | E `TA4_BrokenSeal` → hạ **4 Sói Trúc Ảnh Lv 8 tại TA4 + TA6**, nhận 2 Dấu Trọc Khí virtual ở qualifying kill #1/#2 → E dấu ấn → hạ 1 Sói Trúc Ảnh **Linh Biến tại `TA4.slot1`** với ≥20% → báo Lâm Bá. | 108 EXP + bí kíp tiến cảnh đúng class (dùng từ Lv 10) + 2 đá | Mở Bạch Vân; Xích Nham thêm Lv 12; Q9 tùy chọn Lv 12 / Q10 Lv 15 cần Q8 |
| Q9 — Khảo Chiến Đồng Môn / 12 | Hạo Vũ: “Có đồng môn thì thử vài đường. Chưa gặp ai, cứ đi tiếp.” Hạo Vũ → Hạo Vũ. | Nói chuyện Hạo Vũ → mời/chấp nhận cược 1v1 → hoàn thành một trận đấu thật có kết quả thắng/thua/hòa → báo Hạo Vũ. FORFEIT do ngắt kết nối và SYSTEM_ABORT không tính mục tiêu. | 270 EXP + 200 Vàng sau PvP thật | Optional; không chặn Q10, bỏ qua không thưởng |
| Q10 — Dấu chân Xích Nham / 15 | Tạ Minh: “Chúng lấy đá, còn dòng khí thì mặc kệ. Đem dấu vết về.” Tạ Minh → Tạ Minh. | E `XN1_ChiselMarks` → hạ **6 Đoạt Mạch Lv 13 tại XN1–XN3**, nhận 3 Vật Chứng virtual ở qualifying kill #2/#4/#6 → E `XN2_SealScar` → báo Tạ Minh. | 480 EXP + 3 đá | Q11 sau Q10 + Lv 17 |
| Q11 — Mở Lối Huyền Môn / 17 | Tạ Minh: “Nối lại từng chỗ. Đừng chạm cổng khi dòng khí chưa yên.” Tạ Minh → Tạ Minh. | E `XN4_SealA` + hạ **3 Thạch Lv 16 tại XN4** → Mảnh 1; E `XN5_SealB` + **3 tại XN5** → Mảnh 2; E `XN6_SealC` + **4 tại XN6** → Mảnh 3; E `XN_HuyenMon_Outer` kiểm đủ mảnh/kích hoạt → báo Tạ Minh. | 670 EXP + Rare III weapon + bí kíp đại chiêu đúng class | Chỉ **Completed** mới mở Huyền Tích; Q12 thêm Lv 20; hết Chương II |
| Q12 — Tiếng gọi từ Huyền Tích / 20 | Tạ Minh: “Giúp nó buông gánh cũ. Đường tu luyện của ngươi còn dài.” Tạ Minh → Tạ Minh. | `HT4_GuardRoute`: hạ **6 Cổ Vệ Lv 20 tại HT4 + HT5** → tới `HT_BossLandmark` → đóng góp ≥10% HP trong **một life Boss** ở death khi đúng step → báo Tạ Minh. | 1.000 Vàng một lần; 0 EXP, không thêm Boss pile | Main Story/Chương III complete; Dư Ảnh và farm tiếp tục |

**Recovery chung:** talk/visit/kill chỉ credit sau Game Server event hợp lệ; lưu step/counter sau commit, death/reconnect không xóa. Ngã ở Q2 thì thử lại, không giả credit chỉ vì bấm phím. Mộc Kiếm Q3 không bán/vứt, grant pending nếu túi đầy. Q4 không cấp lại 320 Vàng khi replay; H đầy HP bị từ chối mà không tiêu thuốc. Q5 áo giữ binding tới lúc mặc, sample chỉ bán đúng step; hết hạn ground/full bag/reconnect giữ entitlement chưa claim với cùng itemInstanceId. Quest reward chỉ trao tại đúng NPC sau capacity preflight.

| Quest cần xử lý riêng | Contract không lặp lại trong bảng objectives |
| --- | --- |
| Q6 | Reset Lv 5 đúng một lần kể cả chọn class muộn; giữ điểm lên cấp sau đó. Equip/learn/xác nhận điểm cùng active group; đã cộng ≥1 điểm chấp nhận, không ép cộng mới khi pool 0. Manual grant/learn và thuốc dự trữ có receipt; nếu Food hồi đầy MP, cast lại để dùng M thật, không fake consume. |
| Q7 | Selected ring instance được giữ binding không bán/vứt tới khi xác nhận đã mặc; đang mặc +1 không phải tháo/mặc lại. Chỉ cấp đá/Vàng nếu thật cần +0→+1, không duplicate resource sau reconnect. |
| Q8 | Chỉ step Linh Biến giữ force request; variant khác đang sống chiếm cap thì chờ lifecycle, không demote/despawn. N requesters chia cùng `TA4.slot1`; quest credit theo từng recipient, reward Linh Biến như mọi con Linh khác. |
| Q9 | Không có đối thủ thì giữ optional và đi Q10; không bot/skip reward. Disconnect/abort không tự hoàn thành, kết quả theo PVP-01. |
| Q10 | Evidence #2/#4/#6 tính theo qualifying kills **của từng recipient**; 6 kills đủ 3, không RNG hay farm lại vô hạn. Sai step/source không hồi tố. |
| Q11 | Mảnh 1/2/3 cấp tại kill đủ credit cuối mỗi khu 3/3/4; portal từ ngoài chỉ activation khi đủ ba counters; không consume mảnh trước turn-in, không mở map trước Completed. |
| Q12 | Boss sống thì đánh, dead thì chờ shared deadline 15 phút; nhận quest không spawn Boss. Damage trước active step/reset không hồi tố; corpse còn trong BossCombatArea có thể credit, về làng/disconnect trước death thì không. Boss death credit và quest turn-in tách receipt; một shared pile. |

**Virtual evidence — luật gameplay:** chỉ `QuestId + IN_PROGRESS + active step/group + đúng source + count còn thiếu` mới generate / credit, sau Game Server death / interaction đã xác thực. Không physical quest pickup, không bag slot, không hiển thị cho player không có objective. Item / icon “Vật Chứng +1” chỉ feedback cho recipient hợp lệ; farm trước / sai step / đủ count không sinh. Guaranteed ordinal dùng qualifying kills **của chính recipient**, không raw world kills; progress / evidence cùng event receipt, cap required count. Turn-in thành công mới cleanup; failed turn-in giữ counters / activation.

Normal / Linh Biến kill / evidence credit cần **≥ 20% RuntimeMobMaxHP ActualHpLost trong lúc objective đang active**, connected / alive / sameMap / trong AssistRadius 8 u và participation ≤ 10 s tại death; Boss Q12 dùng 10% active-step damage và corpse exception §6. Level penalty không chặn quest credit / supply / evidence; EXP / Gold / loot là pipeline riêng. N players có counter / step riêng: không tự share Talk / Equip / Use / Enhance / Region / PvP, không last-hit dependency. Cùng một target không thể credit hơn 5 recipients ở 20% trên full-health life.

Áo / thuốc / vũ khí tutorial và Nấm Sương dùng dạy bán hàng là supply sử dụng được, không phải quest evidence; giữ delivery / entitlement của vật phẩm thật. Dấu Trọc Khí / Vật Chứng / Mảnh Ấn chính tuyến chỉ là virtual counters.

**Failure chung:** step / counter / staged grant / class / activation đã commit giữ qua death, rời map / reconnect; chưa commit event được retry cùng ID, không nhân credit. Tutorial supply giữ pending / cùng instance nếu full bag / ground expiry; không cấp lại lúc turn-in. Completion reward kiểm capacity sau merge stack, thiếu thì giữ Ready và báo X ô; không thưởng một phần / cleanup / unlock trước commit. Quest-specific item bindings chỉ phục vụ tutorial rồi gỡ đúng bước; không gear-lock UI mới.

Journey points theo §6; chapter / story eligibility không tự complete khi Boss chết.

> **Đọc sâu:** [Design Analysis — progression và quyết định](3_HUYEN_LO_DESIGN_ANALYSIS.md#quest-progression)

---

<a id="gear-economy"></a>
<a id="gdd-7"></a>

# 6. Trang bị, đồ rơi và kinh tế

## Danh mục — 18 dòng trang bị thường

6 slots × 3 progression bands = **18 base families**. Weapon có Sword / Bow class variants: **21 concrete regular templates** (15 non-weapon + 6 weapon), rarity / enhancement là instance state. **22 concrete gear ItemDefinitions = 21 regular templates + Mộc Kiếm Q3**. Đây chỉ là tổng gear definitions, chưa tính Food / Potion / manual / material / phù. Mộc Kiếm là 1 tutorial exception ngoài 18 families; ATK 10 để giữ Q4 TTK khi nhịp thường là 1,00 s, không sell / drop. Weapon class lock giữ, Family I Sword / Bow cần Lv 5 dù non-weapon mặc từ Lv 1.

**Mốc mặc đồ dễ nhớ:** Band I non-weapon Lv 1, Band I Weapon Lv 5 sau chọn class, **mọi món Band II Lv 11**, **mọi món Band III Lv 17**. Nguồn gear theo mob/content, không suy từ tên map; bảng source bên dưới là authority chung cho loot/potion/material. Shop chỉ Common I/II; III không bán, Q11 Rare weapon đúng class là guaranteed exception. Mộc Kiếm quest-only, ATK 10, không bán/cường hóa/chuyển giao. Off-class weapon bán được, không equip. Weapon shop chỉ mua khi đã chọn đúng class; đồ II có thể mua/nhặt từ Ong Lv 10 trước Lv 11 nhưng chưa mặc.

**Catalog — Common +0, BASELINE/TUNABLE.** Rarity drop theo channel table; shop không bán Uncommon/Rare/Epic. Cột source I/II/III dẫn bảng mob/content; ngoại lệ quest ghi riêng. Mỗi bậc có hai mẫu vũ khí mang tên riêng theo phái, cùng ATK/giá nhưng Kiếm thêm Chí mạng và Cung thêm Chính xác; **18 family = 21 mẫu thường** vì ba family vũ khí tách Kiếm/Cung. Mộc Kiếm là mẫu ngoại lệ ngoài 18 family.

| Bậc / mẫu | Ô | Chỉ số Common +0 | Cấp mặc / phái | Nguồn | Mua / bán Common, Vàng |
| --- | --- | --- | --- | --- | --- |
| Ngoại lệ — Mộc Kiếm | Vũ khí nhập môn | ATK 10 | Q3 / Tân Lữ | Q3 cấp một lần; không rơi | Không mua / không bán |
| I — Thanh Mộc Kiếm | Vũ khí Kiếm | ATK 15 / Chí mạng +0,5 điểm % | 5 / Kiếm | Shop + I drop; Q6 nếu chọn Kiếm | 300 / 75 |
| I — Thanh Mộc Cung | Vũ khí Cung | ATK 15 / ACC 10 | 5 / Cung | Shop + I drop; Q6 nếu chọn Cung | 300 / 75 |
| I — Áo Thanh Mộc | Áo giáp | HP 40 / DEF 4 | 1 / mọi phái | Shop + I drop; Q5 guaranteed | 220 / 55 |
| I — Quần Thanh Mộc | Quần | HP 25 / DEF 3 | 1 / mọi phái | Shop + I drop; Q3 guaranteed | 160 / 40 |
| I — Giày Thanh Mộc | Giày | DEF 2 / EVA 4 / tốc chạy +1% | 1 / mọi phái | Shop + I drop | 140 / 35 |
| I — Nhẫn Thanh Mộc | Nhẫn | ACC 6 / Crit 1% | 1 / mọi phái | Shop + I drop; Q7 nếu thiếu | 140 / 35 |
| I — Dây chuyền Thanh Mộc | Dây chuyền | MP 20 / EVA 5 | 1 / mọi phái | Shop + I drop | 160 / 40 |
| II — Vân Nham Kiếm | Vũ khí Kiếm | ATK 28 / Chí mạng +1 điểm % | 11 / Kiếm | Shop + II drop | 650 / 162 |
| II — Vân Nham Cung | Vũ khí Cung | ATK 28 / ACC 20 | 11 / Cung | Shop + II drop | 650 / 162 |
| II — Áo Vân Nham | Áo giáp | HP 90 / DEF 9 | 11 / mọi phái | Shop + II drop | 500 / 125 |
| II — Quần Vân Nham | Quần | HP 55 / DEF 6 | 11 / mọi phái | Shop + II drop | 380 / 95 |
| II — Giày Vân Nham | Giày | DEF 4 / EVA 8 / tốc chạy +2% | 11 / mọi phái | Shop + II drop | 300 / 75 |
| II — Nhẫn Vân Nham | Nhẫn | ACC 10 / Crit 1,5% | 11 / mọi phái | Shop + II drop | 320 / 80 |
| II — Dây chuyền Vân Nham | Dây chuyền | MP 40 / EVA 8 | 11 / mọi phái | Shop + II drop | 330 / 82 |
| III — Huyền Ấn Kiếm | Vũ khí Kiếm | ATK 40 / Chí mạng +1,5 điểm % | 17 / Kiếm | III drop / Boss; Q11 Rare nếu chọn Kiếm | Không mua / 450 |
| III — Huyền Ấn Cung | Vũ khí Cung | ATK 40 / ACC 30 | 17 / Cung | III drop / Boss; Q11 Rare nếu chọn Cung | Không mua / 450 |
| III — Áo Huyền Ấn | Áo giáp | HP 125 / DEF 15 | 17 / mọi phái | III drop / Boss | Không mua / 350 |
| III — Quần Huyền Ấn | Quần | HP 80 / DEF 9 | 17 / mọi phái | III drop / Boss | Không mua / 275 |
| III — Giày Huyền Ấn | Giày | DEF 6 / EVA 12 / tốc chạy +3% | 17 / mọi phái | III drop / Boss | Không mua / 225 |
| III — Nhẫn Huyền Ấn | Nhẫn | ACC 15 / Crit 2% | 17 / mọi phái | III drop / Boss | Không mua / 225 |
| III — Dây chuyền Huyền Ấn | Dây chuyền | MP 60 / EVA 12 | 17 / mọi phái | III drop / Boss | Không mua / 225 |

18 dòng trang bị không buộc thay cả bộ. Vũ khí Rare II +6 đạt **45,47 ATK** và Tinh Hoa I, so Common III +0 là **40 ATK**; đồ II đã đầu tư có thể đáng giữ để chuyển sang III. Nhẫn/dây chuyền đã nâng cũng có thể hơn món III mới chưa nâng. Dây chuyền III bán 225 Vàng, ngang Giày/Nhẫn III. Khi rơi trang bị, chọn đều giữa các ô hợp lệ (Nấm Lv 2 / Sói Lv 4 chỉ năm ô không vũ khí; nguồn khác đủ sáu); vũ khí chia đều Kiếm/Cung, không tự ưu tiên phái người nhặt.

| Ô trang bị | Chỉ số chịu phẩm chất | Tinh Hoa I tại +4 | Tinh Hoa II tại +8 (chỉ bậc III) |
| --- | --- | --- | --- |
| Vũ khí | ATK; thêm ACC nếu là Cung, không nhân Chí mạng Kiếm | +0,5 điểm % chí mạng | +6 chính xác |
| Áo giáp | HP / DEF | +10 máu | +2 phòng thủ |
| Quần | HP / DEF | +1 phòng thủ | +15 máu |
| Giày | DEF / EVA; không nhân tốc chạy | +4 né tránh | +1 phòng thủ |
| Nhẫn | ACC, không nhân Crit | +4 chính xác | +0,5 điểm % chí mạng |
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

Thứ tự tính từng món: ATK/HP/MP/DEF gốc × hệ số phẩm chất × hệ số cường hóa; ACC/EVA gốc (gồm ACC Cung) thuộc cột chỉ số chịu phẩm chất chỉ × hệ số phẩm chất, **không** × hệ số cường hóa; Chí mạng gốc của Kiếm/Nhẫn và tốc chạy Giày giữ nguyên. Sau đó cộng riêng theo cấp: Giày +2 EVA/cấp; Nhẫn +2 ACC và +0,2 điểm % Chí mạng/cấp; Dây chuyền +2 EVA/cấp. Cộng Tinh Hoa I/II vào chỉ số tương ứng sau phép nhân, không nhân chúng lại với phẩm chất/cường hóa. Cộng các món với nền nhân vật và điểm thuộc tính; cuối cùng mới áp nội tại nền tảng Kiếm/Cung một lần. Tốc chạy cộng tuyến tính: `MoveSpeedMultiplier = 1 + 0,0005 × AGI + Giày%/100`; không nhân lại theo phẩm chất/cường hóa. Giữ số lẻ nội bộ, chỉ làm tròn khi UI/sát thương cần. Tinh Hoa tính từ cấp item, không cộng dồn lại khi mặc hoặc tải save.

Bảng tra đầy đủ chỉ số **Common từ +0 đến trần từng bậc**, gồm cả Mộc Kiếm không thể nâng, nằm tại [Analysis §4](3_HUYEN_LO_DESIGN_ANALYSIS.md#gear-upgrade-values). Công thức và giới hạn trong GDD này là luật nếu cần tính phẩm chất khác Common.

**Ví dụ kiểm chứng:** Áo giáp Rare III +8 có HP = 125 × 1,16 × 1,59 + 10 (Tinh Hoa I) + 0 (Tinh Hoa II không cho HP) = **240,55**; DEF = 15 × 1,16 × 1,59 + 2 (Tinh Hoa II) = **29,666**. Khi mặc, cộng hai giá trị này vào tổng HP/DEF trước nội tại Kiếm Tâm; Kiếm Tâm mới nhân MaxHP ×1,10 và DEF ×1,08. Preview, stat panel và save load phải dùng đúng cùng phép tính.

## Chuyển giao cường hóa — P0 tại Bách Luyện

Người chơi chọn **đồ nguồn + đồ đích đã sở hữu**, khác instance, cùng ô, đều ở trong túi (tháo món đang mặc trước). Vũ khí phải cùng loại Kiếm/Cung, đúng phái; nhân vật phải đủ cấp mặc đồ đích. Mộc Kiếm, bí kíp, vật phẩm nhiệm vụ và đồ đang ràng buộc hướng dẫn không dùng được. Đích giữ nguyên template, phẩm chất và instanceID; nguồn bị tiêu, không sao chép trang bị hay phẩm chất.

| Kiểu chuyển | Điều kiện bậc | Cấp đích sau chuyển | Chi phí |
| --- | --- | --- | --- |
| Đổi món trong cùng bậc, thường để lên phẩm chất | I→I, II→II hoặc III→III | max(cấp đích, cấp nguồn) | 800 Vàng; không Tinh Thạch |
| Sang bậc kế tiếp | I→II hoặc II→III; không nhảy I→III | max(cấp đích, cấp nguồn) | 500 Vàng + 2 Tinh Thạch |

Nếu cấp đích không tăng, từ chối trước khi tiêu gì; cấp chuyển không được vượt trần bậc đích. Đường nâng dễ nhớ: **I +4 → II +4 → II +6 → III +6 → III +8**. Không tạo vật phẩm chuyển giao. Preview nêu nguồn sẽ mất, Vàng/đá, chỉ số đích trước/sau và trạng thái hai Tinh Hoa. Tiêu nguồn, trả chi phí, cập nhật đúng instance đích và lưu cùng một giao dịch; replay/reconnect trả kết quả cũ, không trừ hai lần. Tinh Hoa luôn tính từ cấp đích sau commit, không copy giá trị cộng từ nguồn. Không bán nguồn trong cùng giao dịch. Evidence chi phí tại [Analysis §4](3_HUYEN_LO_DESIGN_ANALYSIS.md#economy-analysis).

**Loot pipeline:** Gold / Material / Potion / Stone là các channel separate; Gear là **ONE EXCLUSIVE GEAR ROLL**. Một kill có thể Gold + Material + Stone + Gear, nhưng không hai rarity gear. Rates / counts TEST / TUNABLE; semantics dưới đây là baseline triển khai, LOOT-01 chỉ còn tuning / economy.

| Channel | Normal | Linh Biến | Boss (một shared pile) |
| --- | --- | --- | --- |
| Gold / EXP | 100% budget theo mobL | Base × 3 | **0 direct Gold / 0 EXP** |
| Trade Material | 30%, 1 | 100%, 1–2 | — |
| Potion | 4%, 1; tier theo mobL | — | 50%, III 2–3; HP / MP 50–50 |
| Tinh Thạch | 8%, 1 | 100%, 1 | Guaranteed 5–8 |
| Gear exclusive | Common 4 / Uncommon 1 / Rare 0,1 / None 94,9% | Uncommon 30 / Rare 6 / Epic 0,5 / None 63,5% | Rare 40 / Epic 8 / None 52%, tối đa 1 |
| Hồi Sinh Phù | — | — | 30%, 1 |
| Thỏi Vàng | — | — | Guaranteed 3–5, stack 99, sell 250 / thỏi |

Boss Thỏi chỉ thành currency khi nhặt + bán NPC, không auto-credit / không shop buy. Boss không dùng normal level-penalty. Counts uniform inclusive; channels independent, gear exclusive. Q12 Gold turn-in riêng một lần / character, không thêm pile. Linh gear band theo base mob level ở source table; cùng một mob dùng một Linh reward profile.

<a id="farm-rewards"></a>

**Normal / Linh Biến EXP / Gold — one budget / death:** snapshot Contribution = ActualHpLost / RuntimeMaxHP theo characterId, level trước reward, MapId / position / alive / connected và lastDamageAt. Recipient cần damage>0, sameMap, alive / connected, distance ≤ AssistRadius 8 u và đã gây direct / DoT damage trong 10 s trước death (BASELINE / TUNABLE). Không Party requirement. `Reward = floor(BaseReward * Contribution * LevelRewardFactor)` cho EXP và Gold riêng; BaseReward đã có variant multiplier. **Không normalize / redistribute** phần người bị loại; sum reward không vượt budget. Không dùng threshold 20% quest cho EXP / Gold. Gold auto-credit / coin burst local; Boss không dùng pipeline này.

| Chênh lệch tuyệt đối | Hệ số thưởng — TEST / TUNABLE | Regular physical set / Journey |
| --- | ---: | --- |
| `abs(PlayerLevel−MobLevel) ≤ 3` | 1,00 | Đủ điều kiện thưởng; roll profile thường, Journey khi đủ participation |
| `abs(PlayerLevel−MobLevel) ≥ 4` | 0 | EXP / Gold / regular loot / Journey kill = 0 |

Áp riêng từng recipient, snapshot trước level-up, đối xứng cả player quá cao lẫn quá thấp; không có tầng nửa thưởng hay gate RNG theo level. Lv 20 EXP luôn0, Gold/loot/Journey vẫn hợp lệ. Quest/supply/evidence xét riêng §5, vẫn làm quest muộn. Nameplate hai trạng thái theo người xem: bình thường “Đủ điều kiện nhận thưởng”, xám “Chênh lệch quá 3 cấp — không có thưởng farm”; level và marker quest vẫn hiện.

**Physical loot Normal / Linh Biến:** roll một shared set / death, không personal sets. TopDamage là highest ActualHpLost trên entire life ledger, tie characterId; nếu TopDamage factor = 0 thì **không roll regular physical set**, không fallback người thứ hai. TopDamage đủ điều kiện thì roll profile bình thường nguyên quantity; không gate RNG theo level. TopDamage absent / dead không chuyển priority; owner window có thể không ai nhặt, deadline vẫn chạy. Quest tutorial supply tách khỏi regular set.

| Từ deathUtc | Normal / Linh Biến 60 s | Boss 90 s |
| --- | --- | --- |
| Owner window | 0–8 s TopDamage | 0–12 s TopDamage |
| Contributor window | 8–20 s: snapshot contributors ≥ 5% và farm factor>0 | 12–30 s: snapshot Boss contributors ≥ 10% |
| Shared pickup | 20–60 s eligible-FFA, không cần damage nhưng own level factor>0 | 30–90 s FFA, không normal level penalty |
| Expiry | 60 s despawn | 90 s despawn |

Mọi pickup verify sameMap / alive / distance ≤ 1,5 u, instance / capacity và quyền theo window; normal contributor window còn cần snapshot alive/connected/radius/participation như EXP/Gold. **Level eligibility khi pickup:** người có trong death ledger dùng level đã chụp trước reward cho cả ba windows; người đến sau không có ledger dùng level hiện tại. Lên cấp từ chính kill không làm mất quyền nhặt món đã roll; các kiểm tra alive/MapId/distance/capacity tại pickup vẫn bắt buộc. Eligible-FFA cố ý cho người đến sau nhặt đồ level-hợp lệ, không cho họ EXP / Gold / quest credit. Timers không reset khi owner chết / reconnect / rời map. Một item chỉ một claim; bag đầy không consume item. Boss pile / thỏi theo channel table trên.

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
| Q3 / Q5 / Q6 / Q7 / Q11 | Quest exceptions trong catalog | Tutorial supplies theo quest | Evidence virtual riêng, không farm material | Guaranteed, không loot roll |

Quest evidence không dùng Trade Material đã farm; tooltip material “Vật liệu giao dịch — có thể bán”. Tinh Thạch không có tier riêng. Source Nấm / Sói Lv 4 loại Weapon ở **mọi map**, không dùng MapId để thay slot pool.

**Vendor economy — BASELINE / TUNABLE:** ItemDefinition buyPrice optional / sellValue explicit. Rarity sell Common 1 / Uncommon 1,5 / Rare 2 / Epic 3 trên Common sell, floor một lần; enhance không tăng sell / hoàn resource. Các supply khác sell = floor(buy × 0,25); drop-only không fake buyPrice. Shop stock vô hạn.


**Boss contributor / quest / Journey:** ≥ 10% RuntimeBossMaxHP (release 3.200; demo 12.800 → 1.280), connected / sameMap / insideBossCombatArea tại death; corpse trong area hợp lệ, về làng / disconnect thì không. Q12 dùng active-step damage, không hồi tố; tối đa 10 recipients có thể đạt threshold trên full-health life. Contributor không tạo personal pile.

Normal quest 20% còn phải alive / inAssistRadius 8 u / lastDamage ≤ 10 s, độc lập EXP / Gold>0. Normal contributor pickup 5% theo cùng eligibility. Journey normal 5 / Linh 100 cho regular recipient ≥ 20%, nhân own level factor / floor; Boss 500 / deathID cho qualifying 10%; quest 150 / chapter 300 một lần, PvPwin 200 / matchID. Không claim points bằng pickup.

Bag 30 / storage 40; stack 99, gear 1. Pickup stackable ưu tiên fill compatible stack có sẵn, overflow sang stack mới; transaction không fit toàn bộ thì không consume ground item. Bag đầy nhưng stack còn chỗ vẫn nhặt được. Ground Normal / Linh Biến 60 s / Boss 90 s, chỉ physical items; unequip cần ô trống, không bán equipped.

Turn-in tính X ô trống thực cần sau merge stack: thiếu thì báo “Cần X ô trống trong hành trang”, giữ READY_TO_TURN_IN; không consume evidence / trao một phần reward / set Completed. Vàng / EXP / story / Journey không cần slot; retry không nhận lặp.

Yên Thảo bán Food / Potion / phù; Bách Luyện bán Common I / II, Tinh Thạch **800 Vàng**, upgrade / sell; Tạ Minh bán Tẩy Mạch. Q9 không gear-exclusive, skip không mất nâng slot. Một tiền tệ Vàng. Bag Sort / protection gear P1; validation inventory P0.

> **Đọc sâu:** [Design Analysis — kinh tế và enhance](3_HUYEN_LO_DESIGN_ANALYSIS.md#economy-analysis)

---

<a id="consumables-death"></a>
<a id="gdd-8"></a>

# 7. Vật phẩm tiêu hao và tử vong

Không passive regen. Food chính: tick 2 s theo MaxHP / MP, cap đầy, 10 phút, mới thay cũ, không pause khi trúng. Potion cứu nguy. Nghỉ tại Mộc An ở Vân Khê hồi đầy HP / MP.

| Item | Lv | Hiệu quả | Giá Vàng |
| --- | ---: | --- | ---: |
| Bánh Lúa Mạch | 1 | Mỗi 2 s +2% HP, +1,5% MP | 150 |
| Thịt Nướng Thảo Mộc | 10 | Mỗi 2 s +3% HP, +2% MP | 400 |
| Cơm Hầm Linh Thảo | 15 | Mỗi 2 s +4% HP, +2,5% MP **BASELINE / TUNABLE** | 700 |
| Bình Sinh Lực I / II / III | 1 / 10 / 15 | Instant 30 / 45 / 60% MaxHP | 80 / 220 / 480 |
| Bình Linh Lực I / II / III | 1 / 10 / 15 | Instant 30 / 45 / 60% MaxMP | 80 / 220 / 480 |
| Hồi Sinh Phù | 1 | Tại chỗ 50% HP / MP + 2 s invulnerable | 1.000 |
| Bùa Hồi Thành | P1 | Cast 3 s về làng; damage hủy cast | 150 |

Mọi loại Bình Sinh Lực dùng chung **hồi chiêu HP 8 s**; mọi loại Bình Linh Lực dùng chung **hồi chiêu MP 8 s**. Hai nhóm **tách nhau**: dùng HP không khóa MP và ngược lại. Không có hồi chiêu riêng từng bậc bình.

HP ≤ 0: Death tint / khóa movement, attack, skill, pickup; camera / corpse tại MapId, không auto-respawn. Về Vân Khê miễn phí full HP / MP hoặc tiêu phù hồi tại chỗ. Consequence phù / run-back; không EXP debt / mất gear / Vàng. Death là mốc checkpoint HP = 0: nếu phiên mất, khôi phục tại SafeAnchor của MapId ở trạng thái chết, vẫn phải chọn về làng hoặc dùng phù; relog không tự hồi sinh.

Corpse đủ 10% còn trong BossCombatArea tại Huyền Tích được Boss quest / contributor snapshot, nhưng chỉ nhặt đồ sau hồi sinh; Về Làng trước khi Boss chết thì mất quyền contributor/quest credit. Pile chung vẫn theo các cửa nhặt. Người chết không giữ Boss khỏi reset khi không còn ai sống trong vùng đánh. Food **không hồi máu/Linh lực khi đã chết**. Khi phiên chơi mất, Food/status/cooldown hết hiệu lực; lần vào sau dùng **MapId + HP + MP đã checkpoint**, spawn tại SafeAnchor của map farm/combat, hoặc tọa độ hợp lệ ở khu an toàn (§8). HP/MP được clamp theo Max hiện hành; không restore vị trí chính xác giữa bãi, không tự hồi đầy. Reconnect ngắn khi Game Server vẫn giữ phiên có thể tiếp tục state realtime đó.

> **Đọc sâu:** [Design Analysis — Food / MP sustain](3_HUYEN_LO_DESIGN_ANALYSIS.md#combat-analysis)

---

<a id="online-social"></a>
<a id="gdd-9"></a>

# 8. Online, PvP và giao tiếp

Online multiplayer theo mô hình Client–Server. Player đăng nhập bằng tài khoản do admin cấp, chọn nhân vật rồi kết nối Unity Dedicated Game Server; không có Register cho player. Game Server quyết định combat, quái, Boss và kết quả trận PvP; Spring Boot/PostgreSQL lưu tiến trình, tạm giữ và quyết toán Vàng cược. Client không gửi level / Vàng / EXP / gear / thuộc tính như dữ liệu đáng tin. **P0 acceptance: tối thiểu 2 concurrent players** — hai Client kết nối Game Server; đây là mức nghiệm thu, không phải giới hạn world. P0 chạy local/LAN; triển khai Internet công khai ngoài phạm vi P0.

MapId P0 kiểm combat / mob / portal / loot / chat; khác map không tương tác, chỉ render khu hiện tại. Hide / show P1 không gate slice. Boss timer / banner global khi map rỗng; tên Cự Thú / Dư Ảnh theo character, không tạo entity khác.

World support N players; không hard-code hai slot / Player 1–Player 2. Threat / contribution, MapId / chat, quest assist và loot eligibility dùng collection theo playerId. PvP **1v1** là mode riêng, không giới hạn multiplayer world. Co-op xét N recipients theo contribution / eligibility và level factor §6; kill / evidence credit §5, shared loot windows tách khỏi credit. **Không Party P0:** không create/invite/accept/leader/leave/kick, UI/HP bars/chat nhóm, EXP bonus, loot/quest sharing dành riêng nhóm hoặc raid. N người cùng farm chỉ cần tham gia đánh: contribution, threat, credit và quyền loot xét từng character; không membership container ở giữa. “Cụm quái” là bãi author, không tổ đội. Party chỉ xem xét sau P0, chưa có thiết kế cần code.

**Map Chat:** Enter mở input / gửi, tối đa 80 ký tự; rate 1 message / 2 s mỗi playerId do Game Server kiểm; bubble trên đầu tối đa hai dòng, 4 s rồi fade 0,5 s, cùng MapId. P0 không history lớn. System banner do Game Server phát cho Boss T−60, spawn / death và chapter completion; P1 có thể thêm history nhỏ.

**PvP 1v1 có cược Vàng — USER LOCK:** Hạo Vũ → chọn đối thủ → người mời chọn **1.000–10.000 Vàng**, chỉ theo bước **1.000 Vàng** → người được mời thấy và chấp nhận **đúng mức cược đó** → backend kiểm cả hai đủ tiền và tạm giữ cược của **cả hai cùng một giao dịch** → Game Server mới tạo MatchId/đếm ngược → Active → kết quả → Vân Khê. Chênh cấp ≤5, một hiệp. Không có trận nếu escrow thất bại; không trừ tiền một phía. Lời mời hết hạn 20 s. UI hiển thị cược mỗi người, tổng pot, luật phí và trạng thái tạm giữ trước khi xác nhận.

**Trong trận:** Bắt đầu Active sau khi đếm ngược hoàn tất; đồng hồ **120 s** tính từ đó. Vào Lôi Đài đầy HP/MP; Food đang có hiệu lực **tiếp tục tick và hết hạn bình thường**, không tạm dừng. HP Potion và MP Potion dùng được theo đúng cooldown riêng hiện hành và phải có bình thật trong túi; mỗi người tối đa **3 lần HP + 3 lần MP / MatchId**. Dùng quá quota hoặc không đủ điều kiện bị từ chối **không tiêu item**. Mỗi lần hợp lệ tiêu đúng một bình. **Cấm Hồi Sinh Phù**. `PvPDamageMultiplier = 0,20` BASELINE/TUNABLE áp lên raw trước DEF/Crit/random, không tác động PvE; Bỏng miễn nhiễm, Băng Hàn chỉ Làm Chậm. HP = 0 là PvPDefeated, không phải chết PvE; không mất gear/EXP ngoài tiền cược đã tạm giữ.

**Kết quả và tiền:** Hạ đối thủ HP = 0 hoặc đối thủ disconnect **sau khi Active** thì thắng; người disconnect trước bị FORFEIT. Nếu cả hai mất kết nối trong cùng một tick mà không xác định được ai trước, hoặc Game Server/backend/system lỗi làm trận bị abort, xử **SYSTEM_ABORT**, hoàn **100%** cho cả hai, không phí và không xử thua. Hết 120 s khi cả hai còn sống là **Hòa**, không so HP hay %HP. Disconnect trước Active/khi còn đếm ngược, hết lời mời hoặc không thể bắt đầu thì hủy và hoàn **100% cược cho mỗi người**. Trận có người thắng (kể cả FORFEIT): hệ thống giữ **10% tổng pot**, người thắng nhận **90% tổng pot**. Hòa: mỗi người nhận lại **90% cược của chính mình**, mỗi người mất 10%. Chỉ Spring Boot quyết định số Vàng escrow/settlement và commit một lần theo MatchId; Game Server chỉ quyết định kết quả realtime. Với cược 1.000 mỗi người: pot 2.000; thắng nhận 1.800, phí 200; hòa mỗi người nhận 900; SYSTEM_ABORT mỗi người nhận 1.000.

Sau kết quả, người còn phiên trở về Vân Khê với HP/MP trước trận được chụp lại (clamp theo Max), không mang lượng hồi từ lúc vào trận ra world. Food **không được hoàn thời gian** đã trôi; bình đã dùng vẫn mất. Người mất phiên khôi phục từ checkpoint **trước Lôi Đài**, không bao giờ load tọa độ Arena; kết quả/hoàn cược đã commit vẫn giữ. Người thắng nhận +200 Journey một lần theo MatchId; hòa/abort không nhận. Q9 chỉ tính trận đã giao đấu thật theo §5, không tính disconnect/abort. PvP 1v1 không đặt giới hạn số người trong world.

> **Implementation:** [Technical — network authority](2_HUYEN_LO_TECHNICAL.md#network-authority)

---

<a id="ux-art"></a>
<a id="gdd-10"></a>
<a id="gdd-11"></a>

# 9. Trải nghiệm, điều khiển và hình ảnh

Luồng màn hình P0: Boot/Main Menu → Login tài khoản được cấp → chọn nhân vật → overlay kết nối → map/điểm khôi phục hợp lệ. Nhân vật mới bắt đầu ở Vân Khê. Không có Register cho player; không cần Loading Scene riêng.

Một mapping, không secondary. S + Space ưu tiên drop-through. Text input chặn gameplay; Esc cancel / đóng cửa sổ.

| Phím | Action | Phím | Action |
| --- | --- | --- | --- |
| A / D | Move trái / phải | H | Quick HP Potion |
| Space | Jump | M | Quick MP Potion |
| S + Space | Drop-through | F | Food |
| J | Normal Attack | E | Interact / Pickup |
| 1 | Nhập môn / tiến cảnh | I | Inventory |
| 2 | Đại chiêu | C | Character |
| 3 | Reserve, không dùng P0 | Q | Quest |
| R | Buff P1 | Enter | Chat |
| Esc | Cancel | — | — |

Phím H/M chọn bình **bậc thấp nhất hiện có, đủ cấp dùng và đủ hồi phần HP/MP đang thiếu**; nếu không bình nào đủ bù, dùng bậc cao nhất hợp lệ. Game Server kiểm túi, cấp, số lượng và hồi chiêu; đầy HP/MP hoặc đã chết thì từ chối, không tiêu bình. Q6 dùng Bình Linh Lực I đã phát trước bình khác để không kẹt hướng dẫn. Phím F dùng Food bậc cao nhất hợp lệ; Food mới thay hiệu ứng cũ và đặt lại thời hạn 10 phút, không cộng dồn. E tương tác NPC/nhặt đồ theo mục tiêu.

HUD: HP / MP / EXP / level, skill CD, Food / Potion, quest, Boss timer. Trong PvP hiện cược/pot, đồng hồ 120 s và số lần dùng HP/MP Potion còn lại (ban đầu 3/3). Bảng skill hiện hai active (nhập môn/tiến cảnh chung slot) và hai nội tại/class: icon, tooltip, level/điều kiện khóa, auto-open Lv 5/Lv 13; không thêm hotkey nội tại. Bag-full rõ; tooltip enhance trước / sau. Portal / signpost tên vùng / hướng; quest arrow P1.

| Tracker state | Người chơi thấy |
| --- | --- |
| IN_PROGRESS | “Hạ Sói Trúc Ảnh: 2 / 4” (Q8; counts tại §5) |
| READY_TO_TURN_IN | “Quay về gặp Lâm Bá để báo cáo” |
| LEVEL GATE | “Tu luyện đến Lv.12”; NPC giải thích vùng phù hợp |
| AVAILABLE | NPC / quest panel cho nhận, chưa có progress trước khi nhận |
| OPTIONAL Q9 | Nhãn “Tùy chọn — Tỷ thí”, tách quest được pin; không chặn chính tuyến |

| NPC | Menu / action |
| --- | --- |
| Lâm Bá | Main quest và dẫn truyện |
| Yên Thảo | Food / Potion / Hồi Sinh |
| Bách Luyện | Gear, đá, bán, cường hóa, chuyển giao P0 |
| Mộc An | Storage, nghỉ hồi đầy tại hub |
| Tạ Minh | Class, Huyền Môn, Tẩy Mạch |
| Phong Du / Diệp Lam | Hướng dẫn Kiếm / Cung |
| Hạo Vũ | PvP Challenge |

> **Implementation:** [Technical — UI](2_HUYEN_LO_TECHNICAL.md#ui-notes)

## Hợp đồng hình ảnh

Một male modular rig, không female MVP: **64 × 64 px / PPU 32**, body **44–48 px**, pivot Bottom-Center(0.5, 0.0), hướng phải / flipX trái. Collider~0,60–0,65 u × 1,45 u TUNABLE, không toàn canvas.

| Animation | Frames | FPS baseline | Animation | Frames | FPS baseline |
| --- | ---: | ---: | --- | ---: | ---: |
| Idle | 4 | 6 | Attack | 3 | 12 |
| Run | 6 | 10 | Skill | 4 | 12 |
| Jump | 2 | 8 | Hit | 2 | 10 |
| Fall | 2 | 8 | Death | 3 | 8 |

**26 frames**, parts đồng bộ index / pivot. Ba visual families trùng ba gear bands: Thanh Mộc / Vân Nham / Huyền Ấn. Weapon / Armor / Pants modular; Boots / Ring / Necklace icon / stat only. Cùng family reuse silhouette / frame nhưng khác tier có palette / tint hoặc accent rẻ: Thanh Mộc vải / lục; Vân Nham đá / đồng; Huyền Ấn cổ văn. Không thêm animation set; Head / Hair là base visual, không Helmet slot.

Art P0: male rig, ba families, Sword / Bow visuals, sáu normal sprite sets + một palette Sói Trúc Ảnh (bảy identities), không sprite set riêng Linh Biến, một Boss, ba environment families, UI kit, bốn active VFX presets, nhập môn / evolution reuse cùng class motif, Linh Đạn generic, aura Linh Biến, heal / upgrade / death feedback. Forest dùng Đồng Sương / Trúc Ảnh; Mountain dùng Bạch Vân / Xích Nham; Ancient dùng Huyền Tích; hub tái dùng architectural props phù hợp.

**Visual production flow** (import/layer/collider chi tiết tại Technical §8):

| Pipeline | Thứ tự và ranh giới |
| --- | --- |
| Nhân vật | Source sprite → canvas64×64 / PPU 32 → slice 26 frames → chung pivot chân → male BodyBase / HairHead / Pants / Armor / Weapon → đồng bộ state / frame → palette / accent ba gear bands → actor SortingGroup → status / VFX overlays. Hurtbox / collider độc lập visual, đổi gear / scale Linh không đổi physics. |
| Map | Forest / Mountain / Ancient → tileset / palette → background → Ground / one-way Platform → back props → landmark → foreground → anchors quái / NPC / portal → colliders → camera bounds → kiểm contrast / telegraph / loot / chat. Không thêm lighting framework P0; dùng màu / VFX hiện có. |
| Gắn layout với art | Đồng thoáng / sparse; Trúc nhiều tầng / cầu; Bạch bậc thác; Xích hẻm núi / ba dấu ấn; Huyền phế tích / landmark Boss. Hub reuse props; safe strips và đường về phải đọc được. |

Flash / damage / heal / NÉ, local hit-stop, trail, loot beam và sound; không hard CC mới. Layer / import tại Technical.

| Trạng thái nhìn thấy | Cách đọc trên cùng rig / VFX pool |
| --- | --- |
| Bỏng | Tia lửa/viền ấm nhỏ theo target, tick feedback gọn; không che telegraph hoặc aura Linh Biến. |
| Đóng Băng quái thường / Linh Biến | Phủ băng xanh rõ trong 1,5 s; nứt/tan băng ngắn lúc hết. Cửa miễn 3 s chỉ cần icon nhỏ nếu cần debug, không phủ băng khi đã tan. |
| Băng Hàn Làm Chậm Boss / người chơi | Phủ lam mờ + viền/hạt lạnh nhẹ, **không dùng lớp băng cứng**; Boss có thể hiện icon cạnh HP bar. PvP vẫn đọc được animation đang cast. |

Các overlay chỉ là presentation của status Game Server đã resolve; không thêm sprite/animation rig riêng hoặc đổi collider.

> **Implementation:** [Technical — art / animation](2_HUYEN_LO_TECHNICAL.md#art-contract)
Art accounting: 3 bands × (Sword + Bow + Armor + Pants) = 12 visual modules trên chung 26-frame rig, không 18 full rigs; 21 regular template icons có thể reuse motif / palette, sáu manual icons dùng hai motif class + ba accents. Một base body / hair, aura / status overlays chung; actual slicing / pose reuse cần ART-01 đo, không nhân template count thành rig count.

---

<a id="acceptance-routing"></a>
<a id="gdd-13"></a>
<a id="gdd-14"></a>

# 10. Nghiệm thu và hướng dẫn tra cứu

**Chưa nghiệm thu**: cần playable build / evidence hai Client kết nối Dedicated Game Server (tối thiểu 2 concurrent players), không thay bằng simulation hoặc diễn giải thành capacity tối đa.

| Nhóm | Tiêu chuẩn |
| --- | --- |
| Player | Movement / jump / drop-through; Lv 1–20; reset 20 điểm Lv 5; hai class; 95 điểm Lv 20; Tẩy Mạch không mất dữ liệu |
| Combat | Normal đúng interval; hai active + hai nội tại / class Lv 5 / Lv 13, manual / evolution Lv 5 / 10 / 17; shape / maxTargets / falloff; snapshot / pierce / explosion không double-hit; Evade / Crit; Bỏng / Băng Hàn đúng target branch, post-thaw protection target-wide |
| World | Năm farm maps, ba support zones; SpawnGroup / return / respawn; đúng bảy fixed-level identities / sáu rigs, Linh Biến max 1 / MapId và Q8 deterministic, một Boss với telegraph / target / reset / Cuồng Mạch |
| Story | Q1–Q12 có setup / objectives / turn-in; thiếu level không auto-chain; READY_TO_TURN_IN không auto trả; Q9 không chặn Q10; Q11 complete mới mở vùng; Q12 per character / Main Story Complete; vòng chơi tiếp tục |
| RPG / art | Food / Potion / Death; túi / kho / shop; 6 ô / 18 dòng / 21 mẫu thường / phẩm chất / giới hạn I+4, II+6, III+8 / chuyển giao cùng bậc hoặc lên bậc kế; modular 64 × 64 / PPU 32 / 26 frames |
| Online | N-player collections; P0 acceptance tối thiểu 2 concurrent players qua LAN; combat / portal / chat / loot MapId validation; co-op; PvP cược 1v1, escrow trước trận, timeout 120 s hòa; Boss contribution 10% và shared pile 90 s |
| Reliability | Login/character binding qua Spring Boot; PostgreSQL lưu tiến trình, recovery checkpoint và escrow/settlement receipt; reconnect ngắn resume phiên còn sống, phiên đã mất dùng SafeAnchor; hai Client + Game Server + backend + database, demo fixtures và script/build/video fallback |

Mọi thay đổi tiến trình lưu ngay qua backend, **enhance fail cũng lưu chi phí**; checkpoint MapId/HP/MP theo chu kỳ và chuyển trạng thái quan trọng, không save từng hit hay toàn world. Persistence contract, demo fixtures và acceptance đầy đủ tại Technical.

> **Implementation:** [Technical — demo / QA](2_HUYEN_LO_TECHNICAL.md#qa)

## Tra cứu theo hệ thống

| Domain | Authority | Evidence / implementation |
| --- | --- | --- |
| Power | [§2](#character-power) | [Analysis](3_HUYEN_LO_DESIGN_ANALYSIS.md#character-evidence) |
| Skill / status | [§3](#class-combat) | [Technical](2_HUYEN_LO_TECHNICAL.md#combat-data) |
| World / farm / Boss | [§4](#world-farm) | [Matrix / model](3_HUYEN_LO_DESIGN_ANALYSIS.md#world-economy-analysis) |
| Story / quest; manual ở §3 | [§5](#quests-story) / [§3](#class-combat) | [Technical](2_HUYEN_LO_TECHNICAL.md#combat-data) |
| Gear / reward | [§6](#gear-economy) | [Economy](3_HUYEN_LO_DESIGN_ANALYSIS.md#economy-analysis) |
| Consumables / online | [§7](#consumables-death) / [§8](#online-social) | [Technical](2_HUYEN_LO_TECHNICAL.md#network-authority) |
| UX / art | [§9](#ux-art) | [Technical](2_HUYEN_LO_TECHNICAL.md#art-contract) |
| Gate / open decisions | [§10](#acceptance-routing) | [Analysis](3_HUYEN_LO_DESIGN_ANALYSIS.md#open-decisions) |

---
