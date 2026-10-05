# RPG Online: Huyền Lộ

## Tóm tắt

GDD quyết định luật game và phạm vi TARGET. Mỗi phái có ba kỹ năng chủ động; chọn kỹ năng và thực thi là hai thao tác riêng. Game Server quyết định kết quả chiến đấu. Bản thử (prototype) chỉ cung cấp bằng chứng cho revision đã chạy, không nghiệm thu bản triển khai thật (production).

## Tìm gì ở đâu

- [CombatFocus / đổi mục tiêu](#focus-input), [pending / buffer / arrival](#pending-cast), [Esc](#escape-priority).
- [Quái / bãi](#world-farm), [crowd melee](#melee-crowd), [Q1–Q12](#quests-story).
- [Phím / dùng đồ nhanh](#ux-art), [Quick Potion / Food](#quick-items), [nghiệm thu](#acceptance-routing).
## Tài liệu thiết kế game

**Trạng thái:** thiết kế và kiểm chứng bằng bản thử; chưa bắt đầu codebase production
**Ngày đối chiếu:** 2026-10-06

<a id="gdd-0"></a>

# 0. Thẩm quyền và luật đã khóa

GDD giữ luật game (WHAT); [Technical](2_HUYEN_LO_TECHNICAL.md) giữ hợp đồng triển khai (HOW); [Analysis](3_HUYEN_LO_DESIGN_ANALYSIS.md) giữ evidence/quyết định (WHY); [Art](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md) giữ chi tiết hình ảnh/production; [Roadmap](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md) giữ thứ tự và CURRENT/DEFERRED (WHEN).

VS-1 hiện có là **bản thử tích hợp dùng để tham khảo, có thể bỏ đi**. Thư mục và class trong bản thử không quyết định kiến trúc hay nền code production. Bằng chứng của bản cũ không nghiệm thu điều khiển/nhiệm vụ mới.

GDD hiện hành ghi các quyết định người dùng đã khóa và kết luận review đã thống nhất. Analysis/Technical không tự đổi gameplay. Lỗi xác định đã kiểm chứng phải được sửa đồng bộ; đề xuất và nguồn tham khảo không tự thành luật.

| Phạm vi khóa | Luật |
| --- | --- |
| Nhân vật | Cap Lv 20; Tân Lữ trước chuyển class (kể cả Lv 5+ để làm Q5); từ Lv 5 được chọn một trong hai class; một nhân vật nam |
| Build | Bốn thuộc tính, 95 điểm ở Lv 20, không branch cap, không Skill Rank |
| Combat | Ba skill tích lũy / class ở Lv 5/10/17; hai nội tại / class; Băng Hàn chọn Đóng Băng hoặc Làm Chậm theo loại mục tiêu |
| Nội dung | 7 loại quái có level cố định trên 6 base rigs; Linh Biến modifier P0, 1 World Boss; 5 farm maps + 3 support zones |
| RPG | Food hồi phục chính, không tự hồi khi thiếu Food; Death có hậu quả; 6 ô × 3 bậc trang bị; I tối đa +4, II +6, III +8 |
| Online | Client–Server; Unity Dedicated Game Server quyết định gameplay; PvP 1v1 cược Vàng qua backend escrow; shared loot, Map Chat, MapId P0 |
| Asset / save | Một male modular rig, 64 × 64, PPU 32, 26 frames; Spring Boot + PostgreSQL lưu tiến trình và recovery checkpoint P0 |

**LOCKED** là quyết định đã khóa; **STRONG DIRECTION** là hướng thiết kế rõ nhưng cách thực hiện còn phải kiểm; **BASELINE / TUNABLE** là mốc số dùng để thử và có thể tinh chỉnh; **OPEN** là quyết định chưa chốt; **LEGACY / SUPERSEDED** là dữ liệu lịch sử đã bị thay thế. P0 bắt buộc trong TARGET, P1 sau phần cốt lõi, P2 hoàn thiện thêm; DROP nằm ngoài MVP. Nguồn tham khảo và đề xuất không tự thành luật.

> **Đọc sâu:** [Design Analysis — quyết định mở](3_HUYEN_LO_DESIGN_ANALYSIS.md#open-decisions)

---

<a id="vision"></a>
<a id="gdd-1"></a>
<a id="gdd-12"></a>

# 1. Tầm nhìn và phạm vi

**Giới thiệu ngắn:** RPG hành động 2D online ngang trên PC / Unity. Tân Lữ khám phá linh mạch, chọn Kiếm / Cung, tự phân bốn thuộc tính, gom quái đánh lan, nâng gear, săn Linh Biến / Boss và tỷ thí.

| Trụ cột | Người chơi cảm nhận | Dấu hiệu đạt |
| --- | --- | --- |
| Farm có nhịp | Gom quái rồi cleave / pierce / spread / explosion | Lv 5 single; Lv 10 học tiến cảnh max 3; Lv 17 đại chiêu |
| Build tự do | Đổi phân phối điểm để thử cách chơi | All-in không bị khóa progression; Tẩy Mạch sửa build |
| Progression hữu hình | Gear mới đổi cả stat và hình | Weapon / Armor / Pants đổi sprite |
| Hai class khác nhau | Kiếm áp sát; Cung giữ khoảng cách | Range, hit shape và control khác rõ |
| Online có ý nghĩa | Co-op farm / Boss, chat, challenge | Client kết nối tham gia gameplay thật |
| Scope hoàn chỉnh | Ít nội dung nhưng nối thành hành trình | Lv 1 → 20, ba chương, chính tuyến và vòng chơi sau truyện |

**Vòng chơi chính:** nhiệm vụ chỉ đường → chuẩn bị Food/Bình → vùng đã mở → đánh đơn rồi gom cụm/đánh lan → EXP/đồ rơi → phân điểm/nâng đồ → mốc cấp → NPC. Linh Biến/Boss/PvP xen giữa các chặng farm; Food và túi đồ tạo nhịp về làng.

Mục tiêu tới Lv 20: 2,5–4 giờ gồm đi đường, nhiệm vụ, mua bán và chạy lại sau tử vong; chưa được kiểm bằng người chơi thật. **P0 acceptance: tối thiểu 2 concurrent players** (hai Client kết nối cùng Game Server); game online nhiều người, không đặt MaxPlayers = 2. Capacity 3–4+ concurrent players phải benchmark performance / network trước khi công bố.

## Phạm vi P0 / P1 / P2

| Hệ thống | P0 | P1 khi core ổn | P2 |
| --- | --- | --- | --- |
| Player / combat | Novice, hai class, bốn attributes / Tẩy Mạch, 3 active tích lũy + 2 nội tại / class, nhập môn / tiến cảnh qua bí kíp, đánh lan / Bỏng / Băng Hàn | Buff đề xuất: Chiến Ý / Ưng Nhãn Cường Hóa (P1; chưa khóa phím); DPS Meter | Cosmetic polish |
| World | Năm bãi, cụm quái, bảy loại quái / sáu rigs, Linh Biến modifier, Boss basic + ba pattern / Cuồng Mạch | Linh Giáp / Vỡ Thế | Hazard, Boss polish |
| RPG | 18 dòng trang bị / 6 ô / 3 bậc, phẩm chất, I +4 / II +6 / III +8, shop / đồ rơi / túi / kho, Food / Death, chuyển giao cường hóa | Sắp túi; khóa đồ; Bùa Hồi Thành | Mua lại, mở rộng túi |
| Story / UI | Q1–Q12 với Q9 nhánh optional; ba Stage Summary; Journey; controls / HUD | Quest arrow, chat history | Extra cosmetics |
| Online / data | Unity Dedicated Game Server + Spring Boot + PostgreSQL; acceptance tối thiểu 2 concurrent players, MapId, co-farm không Party, chat / banner, shared loot ownership, lưu tiến trình an toàn | Observer hide / show; triển khai Internet công khai | Performance polish |

P1 chỉ triển khai sau core và quyết định scope; thông số proposal giữ tại Analysis.

**DROP:** Guild / Trade / Pet / Mount / Crafting / Auction / FreePK; nhiều tiền tệ; Skill Rank; cường hóa vượt giới hạn từng bậc (III không vượt +8); Channel / Zone; world chat / hạ tầng MMO nhiều cụm máy chủ; nợ EXP; hút HP / MP; Decoy; ghép đá; Hương EXP; hệ kháng / yếu nguyên tố. Không thêm thuộc tính hoặc hiệu ứng ngẫu nhiên ngoài luật hiện hành.

**TARGET / CURRENT / DEFERRED:** P0 trong GDD là game đích đầy đủ, gồm cả Kiếm/Cung và online Dedicated + Spring/PostgreSQL. Hiện tổng hợp phản hồi bản thử, sửa tài liệu và thử art/cảm giác trước khi thiết kế nền production Kiếm; Cung và các hệ online được hoãn theo thứ tự, vẫn thuộc TARGET P0. Đây là chiến lược triển khai, không đổi genre hoặc final acceptance. Phạm vi bản thử và gate mạng sớm nằm ở [Roadmap](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#target-current-deferred).

> **Thứ tự triển khai / ngân sách:** [Roadmap — VS-1 và mục tiêu quản lý](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#vs-1)

---

<a id="character-power"></a>
<a id="gdd-3"></a>

# 2. Sức mạnh nhân vật và tiến trình

Lv 20 cap; 5 điểm / level-up = **95 điểm**. Lv 1 chưa có điểm; Lv 2–4 auto +2 STR / +2 VIT / +1 INT, tổng 15. Lv 5 thêm 5 và hoàn 15 thành **20 unspent** một lần; chọn class Q6 rồi tự cộng. Chưa class không dùng skill / vũ khí class; kể cả Lv 5+ vẫn dùng basic Tân Lữ / Mộc Kiếm cho Q4/Q5 hoặc quest muộn. Reset nhập môn xảy ra một lần khi đạt Lv 5; nếu trì hoãn Q6 tới Lv 6+, giữ mọi điểm level-up thêm, tổng spent + unspent = 5 × (L−1), không set lại 20. Q3 trao Mộc Kiếm; Lv 1–2 học NPC / movement, chưa bị yêu cầu combat.

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

`Nền theo level + Thuộc tính + Trang bị → stat trước nội tại → stat cuối`; không hệ số class ẩn. **HP nền Tân Lữ/Kiếm = 120 + 10 × (L−1); MP nền mọi phái = 60 + 4 × (L−1).** Với Cung, ghi `ClassChosenLevel = C` tại giao dịch nhập phái: **HP nền = 120 + 10 × (C−1) + 8 × (L−C)**, L ≥ C ≥ 5. Đây là BASELINE/TUNABLE, không khóa hệ số 8. Cung tăng HP chậm hơn sau nhập phái, không mất HP nền ngay lúc chọn phái, kể cả nhập phái muộn; VIT vẫn +8 HP/điểm cho cả hai phái. Khi C=5, HP nền Cung tại Lv 5/10/17/20 là 160/200/256/280. Công thức này hiển thị công khai trong bảng chỉ số, không phải hệ số ẩn. VIT/INT và gear tiếp tục tăng MaxHP/MaxMP. Rarity/enhance tính ở §6 trước khi cộng; nội tại nền tảng Lv 5 nhân stat sau tổng này đúng một lần (§3). Giữ fractional values, UI mới làm tròn.

| Stat | Nền | Mỗi điểm thuộc tính |
| --- | --- | --- |
| HP | Tân Lữ/Kiếm: +10 mỗi cấp; Cung: +8 mỗi cấp từ `ClassChosenLevel` theo công thức trên (BASELINE/TUNABLE) | VIT +8 cho mọi phái |
| MP | 60 + 4 × (L−1) | INT +5 |
| ATK | 12 + 1,2 × (L−1) | STR +0,70 |
| DEF | 5 + 0,6 × (L−1) | VIT +0,10 |
| ACC | 60 + 4 × (L−1) | AGI +6 |
| EVA | 20 + 2 × (L−1) | AGI +6 |
| SkillDamageBonus | 1 | INT +0,35%, chỉ direct skill, không basic Tân Lữ / DoT |
| MoveSpeedMultiplier | 1 | AGI +0,05%; cộng thêm tốc chạy cố định từ Giày (§6) |

Bốn thuộc tính hiển thị: **Công Lực (STR), Sinh Lực (VIT), Linh Lực (INT), Thân Pháp (AGI)**. STR tăng sát thương trực tiếp; VIT tăng khả năng sống sót; INT tăng MP và sát thương kỹ năng; AGI tăng chính xác/né tránh, kèm một phần tốc chạy. CritChance nền **5% cho mọi class/Tân Lữ + gear**, CritMultiplier 1,5; nội tại không cộng Crit ngầm.

`EvadeChance = 0.02 + 0.43*EVADefender/(EVADefender+2.5*ACCAttacker)`, tiệm cận 45%. All-in không bị khóa progression, không cam kết DPS ngang nhau hoặc đứng chịu ba quái. Chưa cộng điểm tại Lv 5 vẫn làm được Q5 bằng Mộc Kiếm; sau chọn class học S1; không còn class Normal Attack. Các mốc: nội tại nền tảng ở 5 → tiến cảnh ở 10 → nội tại tinh thông ở 13 → đại chiêu ở 17. Bảng gear và ATK quái đã đối chiếu lại với HP/MP theo cấp; playable test vẫn quyết định balance.

**Khi MaxHP/MaxMP thay đổi** do gear/cộng hoặc tẩy điểm: giữ HP/MP hiện có rồi clamp không vượt Max mới; không tự hồi theo tỷ lệ, không revive qua equip/reset. Hồi đầy vẫn qua nghỉ/hồi sinh đã quy định.

Tẩy Mạch Phù: 1.200 Vàng tại Yên Thảo, stock vô hạn; hoàn điểm về unspent, giữ class / level / gear / quest / learned skills. Reset Lv 5 một lần miễn phí. Evidence profiles và sustain ở [Analysis](3_HUYEN_LO_DESIGN_ANALYSIS.md#character-evidence).

---

<a id="class-combat"></a>
<a id="gdd-4"></a>
<a id="combat-status"></a>

# 3. Phái, kỹ năng và chiến đấu

| Giai đoạn | Combat action | Identity |
| --- | --- | --- |
| Tân Lữ, chưa chuyển class | Basic Mộc Kiếm 1,00 × / CD 0,70 s / cận chiến 1,2 u; MP 0 | Onboarding Q3–Q5; giữ được ở Lv 5+ trước Q6 |
| Kiếm, sau chuyển class | S1 single → thêm S2 arc → thêm S3 line; ba slot tích lũy | Áp sát, Bỏng; không Normal Attack thứ tư |
| Cung, sau chuyển class | S1 single → thêm S2 spread → thêm S3 primary/explosion | Tầm xa, Băng Hàn; không Normal Attack thứ tư |

**Nhập phái:** Phong Du hướng dẫn Kiếm, Diệp Lam hướng dẫn Cung tại Học Viện; không gộp thành NPC chọn cả hai. Trước khi xác nhận Q6, ô Vũ khí phải trống: người chơi tháo Mộc Kiếm vào túi, không auto-remove/consume hoặc tự thay bằng weapon thưởng. Kiểm alive/idle, đúng step/NPC/range, weapon slot và capacity trước commit class + grant; reject giữ nguyên class/đồ/receipt. Full bag khi tháo thì giải phóng ô/cất đồ rồi retry.

Class transition bỏ quyền dùng basic Tân Lữ, giữ asset Mộc Kiếm cho onboarding. Chưa học S1 sau chọn class thì học sách/equip theo Q6, không cấp fallback miễn MP. S1 mặc định được chọn sau learn; mở S2/S3 không tự cast hoặc đổi selection. Nhịp và MP mới dưới đây là **PROBE BASELINE / TUNABLE**; chưa có bằng chứng chơi thật. Hướng thiết kế là S1 nhanh, S2 cũng nhanh và có thể dùng thường xuyên để farm, S3 là đòn đặc trưng/burst. Chọn S2 rồi bấm Execute nhiều lần là cách chơi hợp lệ; không mặc định S1 là Attack duy nhất. [Analysis](3_HUYEN_LO_DESIGN_ANALYSIS.md#current-balance-probe) giữ giả định, phép tính và mô phỏng mới.

Hỏa của Kiếm và Băng của Cung là phong vị kỹ năng qua VFX / Bỏng / Băng Hàn; không có hệ khắc nguyên tố hay bảng kháng riêng.

**Ba active tích lũy + hai nội tại/class.** Mỗi active có SkillId và cooldown riêng; S2 không thay S1, S3 không xóa S1/S2. Sáu class SkillIds cho hai class, sáu manual definitions, bốn passive IDs; basic Tân Lữ là definition onboarding riêng. Active cần level **và** learned manual flag; không Skill Rank/điểm skill. Nội tại derive đúng class + level, kể cả chọn class muộn; tên/icon/tooltip/khóa-mở, không manual/hotkey/persist thêm cờ.

| Class / mốc | Nội tại và tooltip — BASELINE / TUNABLE | Cách thể hiện |
| --- | --- | --- |
| Kiếm / Lv 5 | **Kiếm Tâm:** sau cộng nền, điểm và trang bị, MaxHP ×1,10 và DEF ×1,08. | Icon khiên/kiếm; tooltip nêu rõ +10% Máu tối đa, +8% Phòng thủ |
| Kiếm / Lv 13 | **Kiếm Thế:** kỹ năng Kiếm gây **+12% sát thương trực tiếp** lên mục tiêu có tâm vùng trúng đòn cách vị trí ra đòn đã chụp ≤1,2 u. | Icon kiếm áp sát; từng mục tiêu xét riêng, không tăng đánh thường/Bỏng |
| Cung / Lv 5 | **Ưng Nhãn:** sau cộng nền, điểm và trang bị, MaxMP ×1,10 và ACC ×1,08. **Không tăng tầm bắn.** | Icon mắt/tên; tooltip nêu rõ +10% Linh lực tối đa, +8% Chính xác |
| Cung / Lv 13 | **Xạ Tâm:** kỹ năng Cung gây **+12% sát thương trực tiếp** lên mục tiêu có tâm vùng trúng đòn cách điểm phóng đã chụp ≥4 u tại lúc trúng. | Icon tên xa; từng mục tiêu nổ xét riêng, không tăng đánh thường |

Nội tại hiện tên, icon, tooltip, trạng thái khóa/mở trong bảng kỹ năng; tự mở theo class + level, không sách/phím/rank/điểm. Kiếm Tâm/Ưng Nhãn là bonus stat **hiển thị**, không class multiplier ẩn; tính lại sau equip/reset/level rồi clamp HP/MP hiện có theo §2. Kiếm Thế/Xạ Tâm mirror cự ly gần/xa; chỉ áp trên hit trực tiếp của kỹ năng, một lần mỗi target, không thêm proc/combo/resource. Bốn icon tái dùng motif của phái. **Chiến Ý / Ưng Nhãn Cường Hóa** vẫn là buff đề xuất P1, chưa khóa phím, xem [Analysis — candidates](3_HUYEN_LO_DESIGN_ANALYSIS.md#research-ideas).

## Bí kíp — nhận và học

| Milestone | Kiếm / Cung | Cách nhận và học |
| --- | --- | --- |
| Q6 / Lv 5 | Phong Trảm Kiếm Phổ / Linh Tiễn Cung Pháp | Staged grant sau class choice, trước cast objective; use unlock active nhập môn |
| Q8 reward | Phong Trảm Tiến Cảnh / Linh Tiễn Tiến Cảnh | Completion reward Lv 8; use cần Lv 10 + đã học nhập môn, unlock S2 độc lập, giữ S1 |
| Q11 reward / Lv 17 | Kiếm Khí Chân Quyết / Hàn Tiễn Chân Quyết | Completion reward + Rare weapon; use cần Lv 17 + nhập môn, unlock đại chiêu; không bắt buộc đã học tiến cảnh |

Sáu manual IDs, guaranteed one-time / class-specific, stack 1 / quest-bound, không sell / drop / trade; có thể cất rương cùng character, learn từ bag; consume-on-learn. Chỉ học khi alive / idle, không pending cast; kiểm class / level / prerequisite, rồi atomically consume book + set learned flag theo SkillId; Replay / duplicate learned reject không consume. **Bí kíp không có cooldown**; chống học lặp bằng learned flag và grant / learn receipt. Cooldown dưới đây thuộc kỹ năng được mở. Cooldown riêng mỗi SkillId; học skill mới/chuyển slot không reset deadline skill cũ; action đang chạy giữ snapshot. Chưa đủ Lv 10 giữ book, tooltip “Cần Lv 10”, không mất sách; không random book farm. Full bag theo reward preflight; Q6 staged pending không lock quest vĩnh viễn. Nội tại Lv 5/Lv 13 tự học theo bảng trên, không manual. NPC không thêm menu học riêng: dùng trong bag. HUD locked nói rõ level / manual / quest; Lv 10 có book chưa học vẫn nhập môn, tiến cảnh feedback ngắn khi use. Manual là power item thật, evidence chính tuyến vẫn virtual.

## Bộ kỹ năng — BASELINE / TUNABLE

| Class / profile | Lv / manual | Executor / shape | Power | Max | MP / CD | Status |
| --- | --- | --- | --- | ---: | --- | --- |
| Phong Trảm nhập môn | 5 / Q6 | Cận chiến, một mục tiêu, 1,7 u | 1,20 × | 1 | 2 / 0,60 s | Không |
| Phong Trảm tiến cảnh | 10 / Q8 | Arc 120°, 1,7 u | 1,35 × / mục tiêu | 3 | 3 / 0,90 s | Bỏng 4% |
| Kiếm Khí | 17 / Q11 | Line 5,5 u, rộng 0,6 u | 2,80 / 2,60 / 2,40 / 2,20 / 2,00 × | 5 | 16 / 6 s | Bỏng 70% |
| Linh Tiễn nhập môn | 5 / Q6 | Một mục tiêu logic, 6,5 u; một tên hình ảnh | 1,15 × | 1 | 2 / 0,60 s | Không |
| Linh Tiễn tiến cảnh | 10 / Q8 | Spread logic 6,5 u, ba hit | 0,70 / 0,60 / 0,50 × / tên | 3 khác nhau | 3 / 0,90 s | Băng Hàn: 2% Normal / 1% Linh; 2% Boss / PvP |
| Hàn Tiễn | 17 / Q11 | Mục tiêu chính 6,5 u + nổ bán kính 2 u | 2,80 × chính; 1,60 × phụ | 1 + 4 | 16 / 6 s | Băng Hàn: 45% Normal / 30% Linh; 100% Boss / PvP |

Tầm đánh trong skill profiles là tầm thực, không cộng thêm nội tại. Phong Trảm có cùng Bỏng 4% ở mọi level từ Lv 10; Kiếm Khí dùng cùng effect với 70% chance. Nội tại Lv 13 chỉ nhân sát thương trực tiếp khi đúng cự ly.

<a id="focus-input"></a>

## CombatFocus, chọn kỹ năng và thực thi

`CombatFocus` là mục tiêu chiến đấu đang theo dõi, có ba chế độ `NONE / AUTO / EXPLICIT`, độc lập với kỹ năng đang chọn. Tách ba vùng: `search envelope` (vùng tìm mục tiêu), `retention range` (vùng giữ mục tiêu) và `execution range` (tầm thực thi kỹ năng). Rời tầm thực thi không tự xóa focus còn hợp lệ trong vùng giữ. Bán kính, giới hạn dọc và chi tiết thứ tự là TUNABLE.

| Thao tác / trạng thái | Luật chọn và giữ mục tiêu |
| --- | --- |
| NONE + `ExecuteSelected` | Xét kỹ năng đang chọn; ưu tiên mục tiêu cục bộ đánh được ngay, rồi mục tiêu tới được bằng tiếp cận ngang có giới hạn. Không quét toàn map hoặc chọn melee khác tầng không tới được. |
| AUTO | Giữ mục tiêu đang hợp lệ; quái khác gần hơn không tự chiếm focus. Khi mục tiêu bị xóa, lần Execute mới mới được tìm lại. |
| Click quái | Ghim EXPLICIT đúng ID/đời quái đã chọn; click không đánh. Xa, khác tầng hoặc bị địa hình chắn vẫn giữ focus trong retention range; Execute có thể từ chối nhưng không đổi đích. |
| Tab / Shift+Tab ngoài UI | Đổi EXPLICIT trong tập mục tiêu chiến đấu cục bộ (`local combat set`), không cycle toàn map. Ưu tiên cùng SpawnGroup → cùng mặt/vùng đi được → cụm lân cận có liên quan, nhìn thấy/cục bộ → ứng viên cục bộ khác. Thứ tự ổn định theo nhóm/mặt và ID, không sort lại theo nearest mỗi frame; chi tiết còn TUNABLE. |
| RETAIN | Giữ đúng ID, life/generation (đời instance) và MapId; range reject không đổi target để cứu cast. Returning target có thể còn focus; Execute có thể từ chối `TargetReturning` nếu policy bản thử không cho đánh trong Return. Quyền đánh chính xác còn OPEN/TUNABLE. |
| Player chết | **Không tự clear CombatFocus.** Hủy lệnh chờ, buffer và tiếp cận; khóa combat input. Nếu target còn sống, đúng đời/MapId và trong retention range, marker/HUD vẫn hiện current/max HP và cập nhật khi người khác đánh. |
| Target chết/despawn/đổi đời, MapId không hợp lệ, vượt retention, Esc clear hoặc chọn đích khác | Xóa hoặc thay focus theo thao tác. Quái hồi sinh tại cùng SpawnSlot không kế thừa focus đời cũ. Rời/chuyển map làm MapId cũ không hợp lệ. |

**LOCKED cấu trúc input:** `1 / 2 / 3` chỉ chọn S1/S2/S3 đã mở; không cast, không tiếp cận, không tiêu MP, không bắt đầu CD. Tân Lữ chọn basic Mộc Kiếm ở slot 1, slot 2/3 khóa. `ExecuteSelected` là action riêng; phím vật lý còn OPEN. Ví dụ `2 → Execute → Execute → Execute` là ba lần chủ động dùng S2. Giữ Execute không RepeatOnHold; mỗi lần bấm vật lý tạo tối đa một execution intent. Chọn slot đã khóa không đổi selection hay action đã nhận. Chọn slot hợp lệ khác không sửa SkillId trong pending/buffer/action đang chạy; phải bấm Execute mới để thay intent chờ.

Execute chụp `SkillId` đang chọn và identity/life/MapId của target. Nếu không có mục tiêu hoặc victim hợp lệ thì không action, MP, CD hay movement. Với Arc/Line, kiểm có victim trong hình dự kiến tại vị trí tiếp cận hợp lệ và tại vị trí thực trước commit; không buộc mọi victim là focus. RunningAction giữ snapshot riêng, không đọc selection thay đổi sau đó.

Hơi ngoài tầm và cùng đường ngang đi được: một lần Execute có thể tạo **PendingCast + bounded approach** (lệnh chờ và tự tiếp cận trong khoảng giới hạn). Đến tầm phải kiểm lại, thành công mới bắt đầu action/MP/CD và cast đúng một lần. Không tự nhảy, drop, dash, tìm đường nhiều tầng, chạy qua bãi xa hoặc đổi đích. Thiếu MP, skill khóa hoặc CD còn lâu bị từ chối trước tiếp cận; không chờ vô hạn để tự đánh.

<a id="pending-cast"></a>

| Điều kiện | Lệnh chờ / buffer / thực thi |
| --- | --- |
| Execute khi đang giữ phím ngang | Chụp các binding ngang đang giữ. Trong pending/buffer tạm bỏ trục cũ, kể cả ngược hướng; khi intent kết thúc trả quyền cho trục hiện còn giữ. |
| Thả hoặc giữ Execute | Thả không hủy pending một lần; giữ không sinh intent thứ hai. |
| KeyDown di chuyển mới, Jump, DropThrough, đổi/clear focus, mở UI/chat, Esc, chết hoặc chuyển map | Hủy pending/buffer/approach; không thay action đã bắt đầu. Action chưa resolve bị hủy bởi death/map/hard CC theo luật timeline. |
| Action lock hoặc CD sắp sẵn | Chỉ một `BufferedIntent` mới nhất; cửa sổ **0,18 s BASELINE/TUNABLE**. Cả lock và CD phải sẵn trong cửa sổ mới nhận; không tiếp cận khi lock chưa hết. Không FIFO hay hàng đợi dài. |
| Execute mới | Thay pending/buffer cũ bằng snapshot kỹ năng/target tại lần bấm mới; không chồng nhiều đường chạy. Select-only không tạo intent mới. |
| Blocked/không tiến triển/timeout/quá xa/đường cắt EdgeExit/target invalid | Hủy + lý do; không MP/CD, không retry vô hạn. Approach không kích hoạt EdgeExit. |
| Đến tầm | Kiểm target còn sống/đúng đời/cùng MapId và đáp ứng policy Return đang thử; player sống/không CC; skill đã học, vũ khí hợp lệ, đủ MP, CD sẵn, hết lock và range/shape ở vị trí thực. Fail trả reason, không cost. |
| Bắt đầu cast hợp lệ | Chụp stat nguồn/vị trí ra đòn/facing và commit MP/CD đúng một lần. Approach không miễn sát thương hoặc bảo đảm hit. Đổi slot không reset CD. |

<a id="escape-priority"></a>

**Esc, mỗi lần chỉ xử lý một tầng:** đóng/lùi modal hoặc chat → nếu đang pending/buffer thì hủy → nếu có focus thì clear → nếu không có gì thì no-op. Không rơi tiếp xuống thao tác gameplay trong cùng lần bấm.

Trọng lực và quán tính ngang tiếp tục khi Tân Lữ/S1 ra đòn trên không; quyền dùng S2/S3 trên không còn OPEN, phải thử trước khi chốt. Sát thương thường chỉ đổi HP và phản hồi bằng flash/chữ/impact, không tạo trạng thái Hurt, giật đòn, đẩy lùi hay ngắt action. Thứ tự ưu tiên là trạng thái kết thúc/tử vong → khống chế cứng đúng loại mục tiêu → action chưa giải quyết. P0 không cho nhảy/dash để hủy đoạn hồi động tác.

Ứng viên nhặt đồ/tương tác tách khỏi CombatFocus: làm nổi món ở gần, đủ quyền và tầm; `Interact` nhặt/tương tác ngay, không cần bấm lần đầu để chọn rồi lần thứ hai mới dùng. Nhặt xong chuyển sang ứng viên gần tiếp theo; rời tầm thì xóa hoặc đổi ứng viên. Quyền nhận đồ tutorial riêng và quyền nhặt đồ chung vẫn theo §5–6; phím combat không tự nhặt, dùng bình hay chạy tuyến.

**Ba hit Cung:** snapshot A/B/C, A/B/A hoặc A/A/A tại start; resolve cả ba cùng clock +0,12 s, giữ power theo index. Invalid target làm mất index đó, không chuyển target/chia power. Mỗi logical hit có Evade/Crit roll, status tối đa một application/unique landed target/cast kể cả proc fail cache. Hàn primary hợp lệ tại resolve tạo tâm nổ ở primary position kể cả primary Evade; invalid primary không nổ; secondary roll riêng, primary không nhận explosion lần hai. Line intersections gần→xa quyết falloff, focus không đổi thứ tự. Secondary theo geometry, không PlatformID damage gate.

**Mô hình canonical: target-based authoritative combat + logical geometry validation.** Target xác định entity action nhắm tới; Game Server là authority (phía có quyền quyết định kết quả). Tại HitMoment, cận chiến kiểm target sống, đúng life/generation/MapId, phía trước/facing, chồng lấp dọc, range và hình đòn (`hitbox`) với vùng nhận đòn (`hurtbox`), rồi mới xét né/chí mạng/damage. Sprite kiếm không quyết định sát thương. Với đánh xa, tên hình ảnh bay qua quái khác không đổi victim; AoE do Arc/Line/Spread/Explosion chọn secondary, không dùng collider VFX.

Đạn/tên của người chơi và quái đánh xa chỉ trình diễn; Game Server giải quyết mục tiêu/hình đòn theo đồng hồ gameplay. Không tính sát thương từ đường bay, va chạm, chặn đạn hoặc lúc hình đạn tới đích. Cận chiến và vùng báo đòn trên đất của Boss vẫn kiểm lại vị trí/vùng nhận đòn tại HitMoment để người chơi né. Hai cách kiểm đường nhìn (LoS) còn là bản thử: A không lọc địa hình, B lọc SolidWall, so sánh trước production Cung. Kiểm LoS toàn bộ hình học ngoài P0; chưa khóa A/B.

## Sát thương, nhịp đòn và trạng thái

Chụp ATK, bonus kỹ năng từ INT, ACC, Crit và nội tại của nguồn tại lúc bắt đầu cast; xét DEF/EVA và vị trí/vùng nhận đòn của mục tiêu tại lúc trúng. Đổi đồ hoặc lên cấp giữa action không sửa sát thương của action đã phát.

Basic Tân Lữ raw = FinalATK × power; skill raw = FinalATK × power × SkillDamageBonus và bonus nội tại có điều kiện. Trong match PvP, nhân raw theo §8 trước DEF/Crit/random; PvE không dùng hệ số đó. Game Server validate → Evade → Crit → DEF / random → HP: `Damage=max(1,round(Raw*100/(100+TargetDEF)*Random(0.95,1.05)*CritMultiplier))`; miss 0 / NÉ. `round(x)=floor(x+0.5)` cho x ≥ 0. ActualHpLost = min(calculatedDamage, remainingHP) vào threat / contribution, không overkill.

Một action lock chung; CD / MP commit tại cast start, basic Tân Lữ cũng có CD riêng. Các mốc tính từ cast start — BASELINE / TUNABLE:

| Action | Logical resolve | Action lock |
| --- | --- | --- |
| Basic Tân Lữ | +0,10 s | 0,32 s |
| Nhập môn single (hai class) | +0,12 s | 0,30 s |
| Phong Trảm tiến cảnh | +0,14 s | 0,30 s |
| Linh Tiễn tiến cảnh | Ba logical hits cùng +0,12 s | 0,34 s |
| Kiếm Khí | +0,16 s | 0,40 s |
| Hàn Tiễn | +0,18 s | 0,40 s |

Chỉ số/nội tại nguồn và điểm ra đòn được chụp lúc bắt đầu; DEF/EVA/vị trí mục tiêu kiểm tại resolve. Người ra đòn di chuyển sau đó không dời gốc hoặc kéo dài tầm. Chết, chuyển map hoặc khống chế cứng đúng loại hủy action chưa resolve, không hoàn chi phí; kết quả đã resolve không bị hình ảnh sửa. AnimationEvent chỉ trình diễn, không lên lịch sát thương. ART-01 kiểm visual timing; các mô hình rotation cũ cần chạy lại theo [Analysis](3_HUYEN_LO_DESIGN_ANALYSIS.md#design-lock-rationale).

**Status application:** Bỏng và Băng Hàn mỗi loại có tối đa một roll theo `(actionId,actualTargetId,effectId)` tại landed hit đầu tiên; cache cả fail, không reroll trên A/A/A. **Băng Hàn là một application**, chọn kết quả theo loại target trước roll; không roll Làm Chậm và Đóng Băng độc lập.

| Effect / target | Lifecycle — BASELINE / TUNABLE |
| --- | --- |
| Bỏng / quái thường, Linh Biến, Boss | 6 s, tick mỗi 1 s; raw/tick = 0,06 × ATK nguồn đã chụp, qua DEF hiện tại; không Crit / random / INT. Một Bỏng/target. Proc lại thay source/ATK snapshot, refresh **về đủ 6 s từ lúc proc**, giữ nhịp tick hiện có; không cộng dồn sát thương hoặc kéo duration bằng phép cộng. PvP immune. |
| Băng Hàn / quái thường | Chỉ **Đóng Băng 1,5 s**: Linh Tiễn 2%, Hàn Tiễn 45%. Không Làm Chậm. |
| Băng Hàn / Linh Biến | Chỉ **Đóng Băng 1,5 s**: Linh Tiễn 1%, Hàn Tiễn 30%. Khác quái thường ở chance, không ở duration. |
| Băng Hàn / Boss | Chỉ **Làm Chậm 3 s**: tốc chạy khi đổi vị trí ×0,85; **đồng hồ chờ action kế tiếp chạy ở 75% tốc độ**. Linh Tiễn 2%, Hàn Tiễn 100%. Không Đóng Băng. |
| Băng Hàn / người chơi PvP | Chỉ **Làm Chậm 1,5 s**: MoveSpeed ×0,75. Linh Tiễn 2%, Hàn Tiễn 100%; không ảnh hưởng tốc đánh, hồi chiêu, animation hoặc action đang cast. Không Đóng Băng. |

Đang Đóng Băng không roll/apply Băng Hàn; tan băng được miễn Đóng Băng **3 s trên cùng target**, N Cung cùng dùng một deadline, không chain-freeze. Đóng Băng khóa di chuyển/AI đánh và hủy windup/hit/spawn chưa giải quyết; visual đã phát không sinh hit sau cancel, target không nhận thêm damage. Boss/người chơi không Đóng Băng. Làm Chậm chỉ refresh deadline về `now + duration`, không stack magnitude. Boss còn 1,5 s chờ action thì debuff **không reset về full CD**: chỉ giảm tốc đếm phần thời gian còn lại; hết debuff đếm lại 100%. Action/telegraph/projectile **đã bắt đầu** giữ nguyên mốc và tốc độ. Cuồng Mạch đổi future base cadence, Băng Hàn chỉ tác động đồng hồ chờ sau khi đã chọn cadence đó; nhiều Cung không nhân nhiều lớp slow. Một target chỉ dùng đúng effect của category.

Nhiều Kiếm cùng đánh chỉ refresh một Bỏng trên target: tick đầu không bị đẩy lùi mãi, source/ATK snapshot/expiry thay khi proc, tick đúng expiry trước remove. Đòn trực tiếp của Kiếm Thế chỉ nhận +12% nếu đúng khoảng cách từ action origin; Xạ Tâm dùng immutable action origin và actual target position tại logical resolve. Hai nội tại tinh thông không đổi status chance/magnitude. DoT credit source thực; source chết không xóa Bỏng đã áp. Eligibility tại death theo §6; không active Buff P0.

Player–Monster không gây sát thương khi chạm hoặc chặn thân; Monster–Monster không xô đẩy. Chỉ dùng điều chỉnh tách nhẹ để hình dễ đọc khi cần, không đội hình runtime hay thêm stun/đẩy lùi. Gốc đòn cận chiến ở originY + 0,8 u; vùng trúng theo chiều dọc kiểm tại PHY-01.

---

<a id="world-farm"></a>
<a id="gdd-5"></a>

# 4. Thế giới và bãi farm

**World route:** Vân Khê ↔ Đồng Sương ↔ Trúc Ảnh ↔ Bạch Vân ↔ Xích Nham ↔ Huyền Tích. Vân Khê nối Học Viện và Lôi Đài. Q11 mở Huyền Môn từ Xích Nham. Không fast travel ra bãi; Bùa Hồi Thành P1 chỉ đưa về làng.

Mỗi map farm/combat có **một SafeAnchor cố định** để khôi phục khi phiên chơi đã mất; người chơi không xuất hiện lại giữa bãi quái. Vân Khê và Học Viện là khu an toàn: có thể khôi phục tọa độ đã lưu nếu còn hợp lệ, nếu không thì dùng SafeAnchor. Lôi Đài không có điểm khôi phục phiên; trận PvP theo lifecycle riêng (§8).

| Vùng | Hình thái và mục đích |
| --- | --- |
| Vân Khê | Hub yên bình: NPC, mua thuốc, rèn đồ, rương và nghỉ; đường về đọc được từ mũi tên mép map. |
| Học Viện | Khu nhập môn với ledge / drop-through và bãi Bù Nhìn; thao tác lớp học dùng NPC / terrain hiện có. |
| Lôi Đài | Không gian tỷ thí 1v1, tách khỏi farm world bằng match membership. |
| Đồng Sương | Đồng thoáng, 1–2 tầng; thấy quái và lối thoát sớm, chưa ép gom đông. |
| Trúc Ảnh | Rừng trúc, cầu và 2–3 tuyến cao độ; tập xoay bãi, nhảy / kite, lần theo vết ấn dưới cầu. |
| Bạch Vân | Các bậc địa hình bên thác, tuyến vòng; nhóm ba bắt đầu làm tiến cảnh có giá trị. Ba bậc là ví dụ bố cục, không khóa số tầng. |
| Xích Nham | Hẻm núi rộng nhất, nhánh tách rồi nhập lại; ngoại vi luyện công rồi đi sâu tới ba Mạch Ấn/Huyền Môn. Hai nhánh là ví dụ bố cục, không khóa tổng nhánh. |
| Huyền Tích | Ngoại vi phế tích dẫn tới landmark Cự Thú; khoảng trống Boss tách normal spawn để đọc telegraph. |

## Mở bản đồ, mật độ và cụm quái cố định — TEST / TUNABLE

Mỗi điểm sinh quái chọn một **mob identity có level cố định**, vị trí và cụm. Level thuộc identity, `SpawnSlot.level` chỉ cache/validation bằng level đó; không author cùng loài lên nhiều level. Respawn giữ nguyên identity/level. Linh Biến chỉ thêm modifier; cụm là bố trí bãi, không formation hoặc tổ đội runtime.

| Map / gate | Ý đồ bãi và topology nội bộ — STRONG DIRECTION | Traversal tham khảo — TUNABLE |
| --- | --- | --- |
| Đồng Sương / onboarding | Đồi bậc thấp và tuyến dưới; cụm hai quái vẫn phù hợp, thêm các bãi độc lập thay vì một blob lớn | 25–35 s |
| Trúc Ảnh / Q6 Completed + Lv 5 | Nhánh trên cầu/nhánh dưới trấn ấn và vòng về; giữ `TA4.slot1` cho Q8 | 35–55 s |
| Bạch Vân / Q8 Completed + Lv 8 | Các terrace solid quanh thác, tuyến vòng và mỏm cụt; cụm trên/dưới cùng xuất hiện trên camera | 35–55 s |
| Xích Nham / Q8 Completed + Lv 12 | Ngoại vi tách nhánh sâu, hốc/khe đá và ba khu trấn ấn; lối Huyền Môn ở nhánh phù hợp | 35–55 s |
| Huyền Tích / Q11 Completed | Cấu trúc phế tích có tuyến cao/thấp và ngách, khoảng Boss tách normal spawn | 45–60 s |

**STRONG DIRECTION mật độ:** Tăng số lượng bãi/cụm độc lập trên các tuyến và độ cao để thế giới trực tuyến đông đúc, khắc nghiệt và có nhịp độ hợp lý hơn; tuyệt đối **không tăng quy mô một cụm thành khối dồn cục (blob) 8–10 quái**. Mục tiêu là trong một khung hình camera (khung nhìn tiêu chuẩn), người chơi có thể quan sát thấy nhiều cụm nằm ở các thềm đá/tầng cao/tuyến nhánh khác nhau (khoảng 5–8+ quái cùng xuất hiện trong tầm mắt), nhưng mỗi cụm là một `SpawnGroup` độc lập với vùng đi lại (`WalkRegion`) và giới hạn đuổi (`LeashRegion`) riêng biệt. Khi chiến đấu, người chơi chỉ kích hoạt từng nhóm nhỏ, không gây báo động dây chuyền sang các cụm lân cận.

### Đặc tả thiết kế không gian và topology nội bộ 8 bản đồ logic

Đồ thị thế giới (`World Graph`) quyết định cách các bản đồ liên kết với nhau, trong khi đồ thị nội bộ (`Internal Map Graph`) định hình trải nghiệm điều khiển, tầm nhìn và nhịp độ chiến đấu. **Tuyệt đối không thiết kế các bản đồ theo kiểu hành lang phẳng một chiều đơn điệu (vào bên trái → chạy thẳng một mạch → thoát bên phải)**. Mỗi bản đồ phải sở hữu nhận diện không gian riêng biệt thông qua sự kết hợp của: các nhánh rẽ (branches), đường vòng lặp quay về (loops), thềm đá cao thấp (terraces), gờ nhảy (ledges), hốc hang khoét sâu (hollows), đoạn rơi xuyên sàn (drops) và các ngõ cụt đặt bãi tài nguyên/quái tinh anh (dead-ends).

Thế giới gồm **8 logical map roots** (Vân Khê, Học Viện, Lôi Đài và 5 map farm dã ngoại):

#### 1. Làng Vân Khê — Hub bình yên & Điểm tựa sơn cước
- **Vai trò:** Khu vực an toàn tuyệt đối, không có quái vật. Nơi tập trung toàn bộ dịch vụ cốt lõi, tiếp nhận nhiệm vụ và là điểm trở về sau các chuyến thám hiểm.
- **Phân khu chức năng không gian (Spatial Layout):**
  + *Khu trung tâm công cộng:* Nơi già làng Lâm Bá đứng bên gốc đa cổ thụ và bảng chỉ dẫn, đón tiếp người chơi mới (Q1/Q2), dẫn dắt cốt truyện chính và phong ấn Huyền Môn (Q8, Q10–Q12).
  + *Khu Dược thảo (phía Đông):* Nhà thuốc mộc mạc của Yên Thảo, bày các sọt thảo mộc phơi khô, phục vụ mua bán bình Máu/Linh lực, thức ăn, bùa Hồi Sinh và dịch vụ Tẩy Mạch Phù.
  + *Khu Lò rèn (phía Tây tựa vách đá):* Xưởng rèn rực lửa than của Bách Luyện với đe thép và bễ thổi, phụ trách rèn trang bị, cường hóa, chuyển giao và nhiệm vụ Mộc Kiếm (Q3/Q4/Q7).
  + *Khu Kho lương & Nhà nghỉ (phía Bắc):* Gian nhà gỗ yên tĩnh của Mộc An, cung cấp dịch vụ cất giữ đồ đạc (`Storage`) và nghỉ ngơi hồi phục toàn bộ sinh lực/linh lực.
  + *Các lối thông map:* Nhánh Tây nối sang Học Viện (`EdgeExit`); Nhánh Đông nối sang Đồng Sương (`EdgeExit`); Nhánh Nam dẫn xuống Lôi Đài của Hạo Vũ (`SpecialGate`).
- **Triết lý Onboarding Q1:** Người chơi không đứng một chỗ bấm hội thoại menu mà phải thực sự di chuyển bộ qua từng khu vực chức năng, nhận diện vị trí các NPC để hình thành bản đồ nhận thức không gian (mental map) vững chắc.

#### 2. Thiên Môn Học Viện — Huấn luyện nhập môn & Điện Nhập Phái
- **Vai trò:** Khu vực bán an toàn dành riêng cho tập luyện kỹ năng cơ bản, thử nghiệm di chuyển và nghi thức chọn phái (Q2, Q3, Q6).
- **Phân khu 3 khu vực cốt lõi:**
  + *Tuyến vượt chướng ngại vật Q2 (`HV_ObstacleCourse`):* Bắt đầu từ cửa vào (`HV_Entrance`), người chơi phải nhảy qua gờ đá cao (`HV_JumpLedge`), tiếp cận sàn gỗ mỏng trên cao rồi bấm `↓` để rơi xuyên sàn (`DropThrough`) đáp xuống thềm dưới (`HV_DropLanding`), sau đó men theo đường vòng quay lại lối ra làng. Tuyến này kiểm tra trực quan toàn bộ năng lực di chuyển cơ bản (Move, Jump, DropThrough) trước khi cho phép cầm vũ khí.
  + *Sân tập Bù Nhìn (`HV_DummyYard`):* Bãi đất bằng phẳng bố trí **tối thiểu 3 cọc Bù Nhìn rơm độc lập** (HP 60, hồi sinh 25 s). Việc đặt nhiều cọc ngăn chặn tình trạng người chơi chen lấn tranh giành mục tiêu khi làm Q3 và Q6.
  + *Điện Nhập Phái (`HV_ClassHall`):* Gian điện uy nghiêm đặt ở tầng cao phía Tây, thiết kế đối xứng hoàn hảo hai cánh tả hữu: Mentor Phong Du (Kiếm Sĩ) đứng bên cánh tả cùng giá gươm thép; Mentor Diệp Lam (Xạ Thủ) đứng bên cánh hữu cùng giá cung tên. Cách bố trí này khẳng định tính bình đẳng tuyệt đối giữa hai phái, không đặt Kiếm Sĩ làm lựa chọn mặc định trước Xạ Thủ.

#### 3. Lôi Đài Vân Khê — Đấu trường 1v1 PvP
- **Vai trò:** Không gian thi đấu đối kháng trực tiếp giữa hai người chơi theo giao kèo cược (Q9 và hệ thống PvP tự do).
- **Thiết kế không gian:** Sàn đấu đá tảng nguyên khối hoàn toàn phẳng lặng, sạch chướng ngại vật, không có bậc địa hình nhấp nhô hay sàn one-way ngẫu nhiên để đảm bảo tính công bằng và thuần túy kỹ năng. Phía sau là hàng rào gỗ mộc, cờ hiệu truyền thống và cảnh núi xa mờ ảo; tuyệt đối không dựng khán đài huyên náo hay màn hình kỹ thuật số lạc lõng.

#### 4. Đồng Sương (Lv 1–5) — Đồi nương bậc thấp & Bờ suối sương mai
- **Ý đồ không gian & Topology:** Môi trường mở, dốc thoải, thoáng đãng với đồi cỏ bậc thấp và nương rẫy ven suối cạn. Giúp người chơi làm quen với nhịp độ chiến đấu, di chuyển vượt bậc nhỏ và gom nhặt chiến lợi phẩm.
- **Phân bố 2 tuyến đường:**
  + *Tuyến dưới (Lower Lane):* Men theo bờ suối cạn nước nông và vạt nương thấp, nền đất bằng phẳng, bố trí các bãi Nấm Linh Lv 2 di chuyển chậm (cụm `DS1`, `DS2` cho Q4, cùng `DS7`, `DS8` mở rộng).
  + *Tuyến đồi giữa và trên (Middle/Upper Terrace):* Các thềm đồi cỏ bậc solid vững chãi, liên kết nhau bằng các bước nhảy ngắn (cao độ 1–1,5 u), nơi bầy Sói Sương Lv 4 nhanh nhẹn tuần tra (cụm `DS3–DS6` cho Q5, cùng `DS9`, `DS10` mở rộng).
- **Ranh giới an toàn & Lối thoát:** Dải vào an toàn 6–8 u tại cửa ngõ phía Tây giáp Vân Khê; lối thoát sang Trúc Ảnh (`EdgeExit`) nằm ở thềm đồi phía Đông.

#### 5. Trúc Ảnh (Lv 5–10) — Rừng trúc u tịch & Cầu gỗ đa tầng
- **Ý đồ không gian & Topology:** Chênh lệch cao độ bắt đầu rõ rệt với rừng trúc dày đặc, vách đá phủ rêu và hệ thống cầu giàn ván bắc qua đèo. Đây là nơi kiểm tra khả năng phối hợp kỹ năng mới nhận sau khi nhập phái (Lv 5+).
- **Phân bố 3 tuyến đường & Vòng lặp (Loops):**
  + *Tuyến cầu trên cao (Upper Bridge Route):* Kết cấu giàn ván mỏng (sàn one-way) vắt ngang giữa hai mỏm đá, nơi Ong Giáp Lv 10 bay lơ lửng, tạo áp lực tấn công tầm cao (cụm `TA5`, `TA9`, `TA10`).
  + *Tuyến rừng trúc trung tâm (Mid Bamboo Forest):* Thềm đất ẩm ướt dưới tán trúc quanh trụ Trấn Ấn cổ bị nứt (`TA4_BrokenSeal`), nơi bầy Sói Trúc Ảnh Lv 8 hung hãn mai phục (cụm `TA4` với `slot 1` cố định cho Q8 Linh Biến, cụm `TA6`, `TA7`, `TA8`).
  + *Tuyến ven suối trũng (Lower Stream Trail):* Ranh giới phía Tây còn sót lại các cụm Sói Sương Lv 4 (`TA1–TA3`).
- **Vòng lặp cơ động:** Người chơi có thể đứng trên cầu gỗ bấm `↓` để nhảy xuyên sàn rơi xuống bãi trúc dưới chân, hoặc đi vòng qua bậc đá trực giao phía sau để leo ngược lên cầu, tạo nhịp cơ động tự nhiên khi thả diều quái.

#### 6. Bạch Vân (Lv 8–13) — Vách đá thác nước & Đèo mây ba tầng
- **Ý đồ không gian & Topology:** Bản đồ thẳng đứng và hiểm trở nhất, chia thành **3 tầng thềm đá vững chắc (terrace solid)** ôm quanh ngọn thác nước trắng xóa cuồn cuộn đổ xuống vực mây.
- **Phân bố cao độ & Lợi thế class:**
  + *Thềm trên cao quanh đỉnh thác (Upper Falls Terrace):* Không gian mở lộng gió trên vách đá vôi xám lạnh, nơi Ong Giáp Lv 10 bay lượn trên cao (cụm `BV1`, `BV2`, `BV6`).
  + *Tuyến terrace bậc giữa và hốc hang (Middle Cliff Terraces):* Các thềm đá bậc nối tiếp và các hốc đá khoét sâu vào lòng vách núi, nơi các toán Đoạt Mạch Đạo Tặc Lv 13 đóng trại khai thác khoáng (cụm `BV3–BV5` cho Q10/farm, cùng `BV7`, `BV8`).
  + *Mỏm đá cụt nhìn ra vực (Dead-end Overlook):* Điểm ngắm cảnh mây mù và bãi farm phụ với góc nhìn bao quát toàn bộ thác nước.
- **Tương tác chiến đấu:** Cung thủ tận dụng tầm bắn xa 6,5 u đứng từ thềm trên tỉa xuống các toán đạo tặc bên dưới; Kiếm Sĩ tận dụng góc hang hẹp của hốc đá để gom cụm 3 quái tung Phong Trảm tiến cảnh diện rộng.

#### 7. Xích Nham (Lv 12–17) — Hẻm sa thạch đỏ & Mạch ngầm phong ấn
- **Ý đồ không gian & Topology:** Bản đồ có diện tích rộng lớn nhất thế giới, đặc trưng bởi sa thạch đỏ cằn cỗi, khe nứt địa chất sâu hoắm và các mạch khoáng ngầm rực lửa. Cấu trúc không gian dựa trên **2 nhánh lớn hội tụ (Two Branches Merge)**.
- **Phân bố 2 nhánh chiến lược:**
  + *Nhánh hẻm núi ngoại vi (Canyon Branch):* Tuyến đèo dốc đá đỏ khô cằn dẫn từ Bạch Vân vào, nơi các toán Đoạt Mạch Đạo Tặc Lv 13 rải rác đào trộm cổ vật (cụm `XN1–XN3` cho Q10, cụm `XN7`).
  + *Nhánh khe nứt khoáng mạch ngầm (Deep Rift Branch):* Tuyến đường ăn sâu vào lòng núi đá đỏ rực, nơi bố trí **3 trụ phong ấn cổ xưa** (`XN4_SealA`, `XN5_SealB`, `XN6_SealC`) được canh gác nghiêm ngặt bởi quái đá khổng lồ Xích Thạch Linh Lv 16 trâu bò (cụm `XN4–XN6` cho Q11, cùng `XN8`, `XN9`).
- **Điểm kết nối tối thượng:** Cuối nhánh sâu là đại môn Huyền Môn (`XN_HuyenMon_Outer`) sừng sững tựa vào vách núi nguyên khối — một `SpecialGate` phong tỏa lối vào cấm địa, chỉ mở ra khi hoàn thành nghi thức thu thập đủ 3 Mảnh Cổ Ấn trong Q11.

#### 8. Huyền Tích (Lv 17–20) — Phế tích cấm địa & World Boss Huyền Nham Cự Thú
- **Ý đồ không gian & Topology:** Di tích cấm địa cổ đại chìm trong u tối, rêu phong và tàn tích cấm thuật. Kiến trúc đá nguyên khối đồ sộ với các hàng cột gãy, bậc thang đá khổng lồ và hoa văn Mạch Ấn phát sáng tím mờ.
- **Phân khu chức năng nghiêm ngặt:**
  + *Tiền môn và hành lang ngoài (Outer Gate & Corridors):* Các thềm đá bậc dẫn vào phế tích, do Xích Thạch Linh Lv 16 trấn giữ lối vào (cụm `HT1`, `HT6`).
  + *Trung sảnh và hai cánh tả/hữu (Great Hall & Wings):* Dãy hành lang đá cổ uy nghiêm với các bậc thang cao, nơi Cổ Môn Vệ Binh Lv 20 giáp nặng đứng gác (cụm `HT2–HT5` cho Q12, cùng `HT7`, `HT8`).
  + *Khu vực cấm điện trung tâm — BossCombatArea:* Một đại sàn đấu đá tảng cổ xưa rộng lớn, bằng phẳng, hoàn toàn sạch sẽ chướng ngại vật; **tách biệt tuyệt đối khỏi quái thường (normal-spawn exclusion)**. Đây là đấu trường dành riêng cho World Boss Huyền Nham Cự Thú trong Q12, đảm bảo telegraph đòn đánh của Boss luôn rõ ràng, không bị quái thường quấy nhiễu hay gây nhiễu loạn mục tiêu.

---

### Quy chuẩn địa hình trực giao và cơ chế di chuyển thế giới

Để đảm bảo tính nhất quán tuyệt đối giữa mỹ thuật, vật lý và trí tuệ nhân tạo (AI), toàn bộ thế giới tuân thủ các quy tắc bất biến sau:

1. **Khối đặc tự nhiên (Natural Terrain = SOLID MASS):** Đất, đá, gờ núi, thềm đồi tự nhiên luôn là khối chắn đặc có độ dày thực tế: mặt trên nằm ngang, khối vật liệu lấp kín bên trong, vách đứng thẳng góc, đáy và bóng đổ khép kín. Tuyệt đối không vẽ các dải đất tự nhiên mỏng manh lơ lửng giả làm đồi núi.
2. **Không dốc chơi được (`no playable slope/ramp/triangle`):** Toàn bộ bề mặt di chuyển trong gameplay đều là mặt phẳng ngang hoặc vách đứng trực giao. Mái nhà, cành cây, núi xa ở phông nền có thể vẽ chéo cho mềm mại thẩm mỹ, nhưng mặt va chạm gameplay tiếp xúc với chân nhân vật vẫn phải là các bậc ngang/đứng.
3. **Đất đá tự nhiên không bao giờ là sàn xuyên thấu (`no natural one-way`):** Nền đất đá tự nhiên luôn cản trở hai chiều.
4. **Sàn One-way là kết cấu mỏng nhân tạo đặc biệt:** Chỉ các cấu trúc mỏng nhẹ hợp lý như ván gỗ, giàn tre/catwalk, ban công, sàn treo tựa vách có dầm đỡ/dây treo rõ ràng mới được dùng làm sàn one-way. Khi đứng trên sàn one-way, người chơi có thể bấm `↓` để rơi xuyên sàn (`DropThrough`). Một lần bấm chỉ xuyên qua một tầng sàn, không xuyên liên tiếp nhiều tầng khi giữ nút.
5. **Tuyệt đối không có cơ chế leo trèo (`no ladder/rope/vine/climb`):** Không có thang dây, dây leo, cột đu hay bám tường leo trèo; toàn bộ di chuyển dọc dựa vào Nhảy (`Jump` - `↑`), Rơi tự do (`Fall`) và Xuyên sàn (`DropThrough` - `↓`). Mọi cầu thang trong game đều là khối bậc trực giao hoặc chi tiết trang trí.
6. **Vùng nước nông (Shallow Water):** Lòng nước nông có đáy đất thật, người chơi lội qua thì chân tiếp xúc mặt nước sẽ giảm nhẹ tốc độ chạy (hệ số TUNABLE); khi đi trên cầu gỗ hoặc nhảy trên không qua mặt nước thì không bị giảm tốc. Tuyệt đối không có cơ chế bơi lội, chết đuối hay vật lý thủy động lực học.
7. **Cơ chế chuyển tiếp bản đồ:**
   - `EdgeExit`: Vùng mép bản đồ thông thường có mũi tên chỉ hướng và tên vùng đích; nhân vật đi chạm vào vùng này bằng di chuyển thủ công sẽ tự động chuyển map, không cần bấm phím tương tác và không dựng vòm cổng dịch chuyển. Điểm xuất hiện ở map đích luôn nằm phía trong mép, bên ngoài vùng trigger trả về để chống hiện tượng giật chuyển map liên tục (ping-pong transition).
   - `SpecialGate`: Cổng đặc biệt đòi hỏi tương tác xác thực bằng phím (Huyền Môn Q11 cần đủ 3 Mảnh Ấn; Lôi Đài cần giao kèo thách đấu).
   - `SafeAnchor`: Mỗi map có một tọa độ an toàn cố định. Khi mất kết nối hoặc máy chủ khởi động lại, người chơi sẽ xuất hiện tại SafeAnchor của map đó.

---

**Fixed monster ladder — canonical:** 7 identities, 6 base sprite/AI rigs; Sói Trúc Ảnh reuse Sói Sương bằng palette. Hai Sói xám lạnh/lục tối có nameplate riêng; Linh dùng aura tím/ấn sáng chung, không dùng màu sói làm dấu Linh.

| Mob identity | Lv | HP | ATK | EXP | Gold | Visual / AI reuse |
| --- | ---: | ---: | ---: | ---: | ---: | --- |
| Nấm Linh | 2 | 48 | 9 | 15 | 7–12 | Base rig |
| Sói Sương | 4 | 107 | 13 | 22 | 11–18 | Base rig |
| Sói Trúc Ảnh | 8 | 339 | 22 | 38 | 19–30 | Sói rig; palette lục tối |
| Ong Giáp | 10 | 473 | 27 | 47 | 23–36 | Base rig |
| Đoạt Mạch Đạo Tặc | 13 | 704 | 36 | 63 | 29–45 | Base rig |
| Xích Thạch Linh | 16 | 974 | 45 | 81 | 35–54 | Base rig |
| Cổ Môn Vệ Binh | 20 | 1393 | 60 | 108 | 43–66 | Base rig |

Level là nhận diện nội dung; không tạo thêm variant chỉ để mỗi level có một quái. Số bảng derive từ formula dưới; `MobDefinition.fixedLevel` và manifest phải khớp.

### Kế hoạch mật độ và phân bổ bãi quái hiện hành — CURRENT / TUNABLE AUTHORING PLAN

Dưới đây là kế hoạch phân bổ bãi quái (pockets) cho 5 bản đồ farm nhằm phục vụ việc authoring màn chơi (level design blockout) và kiểm thử hiệu năng. Các mốc số là **PROBE BASELINE / TUNABLE RANGE**, không phải trần cố định.

| Map farm | Ý đồ bãi và phân bố không gian | Số cụm dự kiến (Pockets) | Số quái hoạt động (Active Mob Budget) | Quy mô mỗi cụm (Group Size) | Phân bố tầng / nhánh | Quest Anchors bắt buộc bảo toàn | Điểm an toàn & Ranh giới cách ly |
| --- | --- | ---: | ---: | ---: | --- | --- | --- |
| **Đồng Sương** (Lv 1–5) | Onboarding, đồi thấp, nương dốc thoải và bìa rừng. Nhiều bãi nhỏ, không gian mở, tránh áp lực dồn dập. | 8–10 cụm | 14–20 quái | 1–2 quái / cụm | Tuyến dưới/ven suối: Nấm Linh (Lv 2). Đồi bậc giữa và thềm đông: Sói Sương (Lv 4). | `DS1` (Nấm Lv2), `DS2` (Q4 Nấm Linh), `DS3–DS6` (Q5 Da Sói) | Dải vào an toàn 6–8 u từ Vân Khê; các bãi Sói cách biệt đường về làng. |
| **Trúc Ảnh** (Lv 5–10) | Rừng trúc rậm rạp, cầu gỗ, thềm đá cao thấp, kiểm tra di chuyển bậc và đánh quái theo nhóm. | 9–11 cụm | 20–28 quái | 2–3 quái / cụm | Tuyến dưới ven suối: Sói Sương (Lv 4). Bãi trúc trung tâm & quanh ấn: Sói Trúc Ảnh (Lv 8). Tuyến cầu trên cao & vách đá: Ong Giáp (Lv 10). | `TA1–TA3` (Sói Sương), `TA4` (với `TA4.slot1` giữ cho Q8 Linh Biến), `TA5` (Ong Giáp), `TA6` (Sói Trúc Ảnh) | Vùng an toàn 6–8 u tại cửa Đồng Sương và cầu nối sang Bạch Vân. |
| **Bạch Vân** (Lv 8–13) | Vách đá dựng đứng, thác nước, thềm đá bậc liên tục, mỏm cụt và đường vòng. Cung phát huy tầm xa, Kiếm gom góc hẹp. | 8–10 cụm | 20–28 quái | 2–3 quái / cụm | Thềm trên cao quanh thác: Ong Giáp (Lv 10). Các terrace đá bậc giữa, hốc hang và lối đèo: Đoạt Mạch Đạo Tặc (Lv 13). | `BV1`, `BV2` (Ong Giáp Lv 10), `BV3–BV5` (Q10/farm Đoạt Mạch Đạo Tặc Lv 13) | Thềm nghỉ an toàn 6–8 u đầu đèo và trước cửa sang Xích Nham. |
| **Xích Nham** (Lv 12–17) | Mỏ khoáng cằn cỗi, đất đá đỏ, khe nứt sâu và ba khu vực trấn ấn. Quái trâu, áp lực chiến đấu tăng cao. | 9–11 cụm | 24–32 quái | 2–4 quái / cụm | Vành đai ngoại vi và lối vào: Đạo Tặc (Lv 13). Hốc nứt mạch sâu và 3 khu trấn ấn: Xích Thạch Linh (Lv 16). | `XN1–XN3` (Q10 Đạo Tặc Lv 13), `XN4–XN6` (Q11 Xích Thạch Linh Lv 16 tại 3 phong ấn) | Vùng an toàn 8 u cửa ngõ vào và hành lang dẫn đến cổng Huyền Môn. |
| **Huyền Tích** (Lv 17–20) | Phế tích cổ, đền thờ phong ấn, hành lang đá nguyên khối. Tách bạch hoàn toàn quái thường và khu vực Boss. | 8–10 cụm thường + 1 Boss | 20–26 quái thường + 1 Boss | 2–3 quái / cụm | Tiền môn / ngoài cổng: Xích Thạch Linh (Lv 16). Hành lang / nội điện: Cổ Môn Vệ Binh (Lv 20). Trung điện (BossCombatArea): Boss độc lập. | `HT1` (Thạch Linh), `HT2–HT5` (Q12 Cổ Môn Vệ Binh Lv 20), `HT_BossLandmark` (Q12 Boss) | **Boss Exclusion:** Tuyệt đối cấm quái thường trong BossCombatArea. Vùng vào Huyền Môn an toàn 8 u. |

**Bảng danh mục bãi quái hiện hành (Candidate Manifest — PROBE / TUNABLE):**
Các ID cụm mới (`DS7+`, `TA7+`, `BV6+`, `XN7+`, `HT6+`) là các mã định danh authoring ổn định, không dùng chỉ số thực thể sống (live instance index). Level cố định theo loài quái vật (`fixedLevel`), không ngẫu nhiên hóa cấp độ trong cùng loài.

| Nhóm cụm / Map | Cụm ID | Mob identity | Level | Slots dự kiến | Vai trò / Ghi chú bố cục |
| --- | --- | --- | ---: | ---: | --- |
| **Đồng Sương** | `DS1` | Nấm Linh | 2 | 1 | Quest anchor: Nấm khởi đầu ven đường |
| | `DS2` | Nấm Linh | 2 | 1 | Quest anchor: Q4 Nấm Sương tutorial |
| | `DS3`–`DS6` | Sói Sương | 4 | 2 mỗi cụm (8) | Quest anchors: Q5 Da Sói (4 cụm đồi cỏ bậc giữa) |
| | `DS7`, `DS8` | Nấm Linh | 2 | 2 mỗi cụm (4) | Bổ sung: Dải nương thấp và bờ suối phía nam |
| | `DS9`, `DS10` | Sói Sương | 4 | 2 mỗi cụm (4) | Bổ sung: Gờ đồi phía đông và lối rẽ lên Trúc Ảnh |
| **Trúc Ảnh** | `TA1`–`TA3` | Sói Sương | 4 | 2 mỗi cụm (6) | Quest anchors: Bìa rừng trúc giáp ranh Đồng Sương |
| | `TA4` | Sói Trúc Ảnh | 8 | 2 | Quest anchor: `TA4.slot1` cố định cho Q8 Linh Biến |
| | `TA5` | Ong Giáp | 10 | 3 | Quest anchor: Nhịp cầu gỗ trên cao |
| | `TA6` | Sói Trúc Ảnh | 8 | 2 | Quest anchor: Bãi trúc quanh trụ trấn ấn nứt |
| | `TA7`, `TA8` | Sói Trúc Ảnh | 8 | 2 mỗi cụm (4) | Bổ sung: Tuyến rừng trúc trũng và khe đá phụ |
| | `TA9`, `TA10` | Ong Giáp | 10 | 2–3 mỗi cụm (5) | Bổ sung: Mỏm đá cao nhìn ra vực và giàn ván bắc qua đèo |
| **Bạch Vân** | `BV1`, `BV2` | Ong Giáp | 10 | 2 mỗi cụm (4) | Quest anchors: Vùng trời thềm thác nước phía tây |
| | `BV3`–`BV5` | Đoạt Mạch Đạo Tặc | 13 | 3 mỗi cụm (9) | Quest anchors: Ba thềm đá bậc giữa đường đèo |
| | `BV6` | Ong Giáp | 10 | 2 | Bổ sung: Thềm đá gần đỉnh thác đổ |
| | `BV7`, `BV8` | Đoạt Mạch Đạo Tặc | 13 | 2–3 mỗi cụm (5) | Bổ sung: Hốc đá cụt phía bắc và đường vòng chân vách |
| **Xích Nham** | `XN1`–`XN3` | Đoạt Mạch Đạo Tặc | 13 | 2/3/2 (7) | Quest anchors: Q10 Vật Chứng (khu mỏ ngoại vi) |
| | `XN4`–`XN6` | Xích Thạch Linh | 16 | 3/3/4 (10) | Quest anchors: Q11 Mảnh Ấn (ba cụm trấn ấn A/B/C) |
| | `XN7` | Đoạt Mạch Đạo Tặc | 13 | 2 | Bổ sung: Ngách đá hẹp phía tây |
| | `XN8`, `XN9` | Xích Thạch Linh | 16 | 3 mỗi cụm (6) | Bổ sung: Thềm đá nứt mạch ngầm và lối dốc vào phế tích |
| **Huyền Tích** | `HT1` | Xích Thạch Linh | 16 | 2 | Quest anchor: Tiền môn phế tích |
| | `HT2`, `HT3` | Cổ Môn Vệ Binh | 20 | 3/2 (5) | Quest anchors: Hành lang ngoài và cầu thang đá dẫn vào cấm điện |
| | `HT4`, `HT5` | Cổ Môn Vệ Binh | 20 | 3 mỗi cụm (6) | Quest anchors: Q12 Vệ Binh trước cửa Boss |
| | `HT6` | Xích Thạch Linh | 16 | 2 | Bổ sung: Ngách phế tích phía đông |
| | `HT7`, `HT8` | Cổ Môn Vệ Binh | 20 | 2–3 mỗi cụm (5) | Bổ sung: Cánh tả và cánh hữu sảnh tế lễ |
| | `HT_Boss` | Huyền Nham Cự Thú | 20 | 1 | Khu vực Boss độc lập (`BossCombatArea`), cấm quái thường |

---

### Bảng dữ liệu gốc lịch sử (LEGACY seed manifest — 28 cụm / 66 điểm sinh quái)

> [!NOTE]
> Bảng manifest 28 cụm dưới đây là **dữ liệu lịch sử (LEGACY seed)** được giữ lại nhằm phục vụ việc đối chiếu với bản prototype cũ và bảo toàn các `SpawnGroup` ID dùng làm mốc neo cho nhiệm vụ (Quest Anchors ở §5). Đây **không phải là trần mật độ hay giới hạn số cụm của bản hoàn chỉnh**. Khi mở rộng bản đồ, các ID nguồn quest (`DS2`, `DS3–DS6`, `TA4.slot1`, `TA5`, `TA6`, `XN1–XN6`, `HT4–HT5`) phải được giữ nguyên vị trí và vai trò logic.

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

Lối vào an toàn khoảng 6–8 u là BASELINE/TUNABLE; từ đó các bãi tách nhau bằng vùng đi được, khối địa hình hoặc tuyến cao/thấp, có đường quay về dễ đọc. Không dùng khoảng cách tâm 18–20 u cũ làm luật mật độ mới: isolation xét SpawnGroup, WalkRegion và khả năng tới nhau. Aggro 5 u/leash 8 u chỉ là BASELINE/TUNABLE; leash neo vào home, không chạy theo vị trí chase mới. Attack offset 0–0,35 s cũng còn cần kiểm.

<a id="terrain-rules"></a>

**LOCKED địa hình:** đất/đá tự nhiên là **khối solid có độ dày**, fill, mặt trên ngang, mặt đứng, đáy/bóng và góc khép. Đồi/núi xây bằng khối bậc/terrace liên tục; có thể có hốc, hang, khe dọc và mỏm nhưng không giả núi bằng dải tự nhiên mỏng nổi. Mặt chơi chỉ ngang hoặc đứng: không slope/ramp/triangle, collider đứng được xoay hay mặt chéo. Mái/cành/núi nền có thể vẽ chéo; collision chơi vẫn trực giao.

**One-way hiếm, chỉ cho cấu trúc hợp lý:** ván mỏng, giàn/catwalk, ban công nhẹ, sàn tạm hoặc sàn treo/tựa vách, có dây/dầm/cột/bracket đỡ rõ. Đất/đá tự nhiên không one-way. Solid phải dày/khép; one-way mỏng/có khoảng trống dưới; background/decor tương phản thấp và không giả mặt đứng được. Vật trông như cầu thang/sàn phải chơi được đúng hình hoặc đổi hình đủ rõ. DropThrough chỉ qua one-way đang đứng, không xuyên solid và không chain sàn khi giữ nút.

Kit công trình tái dùng cột, dầm, sàn, cầu, ban công, tường, mái, vòm/cổng và block step; landmark chỉ cần vài mặt collision sạch. Không procedural building, không polygon collider bám toàn silhouette. Mái làm route phải có mặt ngang/bậc riêng. **Không ladder, rope/vine/pole/wall climb, Climb action/state/animation.** Di chuyển vẫn Move/Jump/Fall/DropThrough; cầu thang là khối bậc trực giao hoặc decor rõ.

**HomeRegion / WalkRegion:** HomeRegion là vùng hoạt động gốc của cụm; WalkRegion/SurfaceId là địa hình quái được phép đi, có HomeSpan khi cần. Ground mob tuần tra trong vùng đã author, gặp mép không có nền nối thì quay đầu, không tự rơi/nhảy/drop hoặc tìm đường nhiều tầng. Hai bậc chỉ nối được với AI nếu có đường đất trực giao liên tục thật; route player phải jump/drop không tự là route quái. Passive aggro ưu tiên mục tiêu local có thể tới được. Bị đánh ngoài passive aggro vẫn tạo threat/wake; báo động chỉ cùng SpawnGroup, không lan recursively sang group bên cạnh.

**Return:** mục tiêu unreachable hoặc ra khỏi leash sau grace thì quái về home theo policy; ưu tiên xét threat khác có đường hợp lệ trước. Cung kite bằng liên tục đổi vị trí trên route hợp lệ có thể no-hit pure melee, đó là lợi thế class. Safe perch không tới được mà đứng spam mãi là lỗi geometry; ưu tiên sửa authoring và Return đơn giản, không cho Sói ranged fallback/teleport/jump tầng để cân Cung. Exact grace, tốc Return, regen/invulnerability/targetability còn OPEN/TUNABLE; không miễn damage tức thì chỉ vì player nhảy. Focus có thể giữ Returning target để quan sát. Return/reset không loot, reroll hay tạo life mới; clear ledger/status theo policy encounter hiện hành, chưa khóa cách hồi HP theo thời gian.

**Nước nông:** lòng nước nông có đáy đất thật, người chơi vẫn đi được và giảm nhẹ tốc chạy khi chân chạm nước. Qua cầu hoặc ở trên không không nhận giảm tốc nước. Nước rộng có cầu và đường đi hợp lệ; không thêm swimming, drowning hoặc fluid physics P0. Hình nước/thác không tự quyết collision hay sát thương. Hệ số/depth cần playtest trước production; thông số bản mẫu thuộc [Roadmap](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#prototype-visual-review).


Farm bãi gần cấp trong khoảng thưởng §6; quái cao cấp hơn vẫn có thưởng nếu trong khoảng, nhưng không bảo đảm an toàn. Identity/cấp quái quyết định bậc đồ rơi; map quyết định nguyên liệu và sắc thái. **[Farm Matrix Lv 1–20](3_HUYEN_LO_DESIGN_ANALYSIS.md#farm-progression)** giữ derived HP / EXP / Gold, không copy stats table vào GDD. Measure solo / 2 / 3 / 4 players, wait / crowd / CPU / network và run-back trước capacity claim; low-level pockets ít slot có thể cần rotate, không thêm Party / Channel.

## Chiến đấu và vòng đời quái

| Normal archetype | AI | Move u / s | Melee / ranged u | Interval | Projectile |
| --- | --- | ---: | --- | ---: | --- |
| Nấm Linh | Melee only | 1,2 | 0,8 / — | 1,8 s | — |
| Sói Sương / Sói Trúc Ảnh | Melee only; chase nhanh | 2,4 | 1,0 / — | 1,3 s | — |
| Ong Giáp | Ranged / Flying | 2,0 | — / 5 | 1,6 s | Visual generic, speed 5,5 u / s |
| Đoạt Mạch Đạo Tặc | Nền GroundMelee; cấu hình Hybrid còn OPEN | 2,2 | 1,2 / 5 | 1,5 s | Visual generic, speed 5 u / s |
| Xích Thạch Linh | Nền GroundMelee; cấu hình Hybrid còn OPEN | 1,4 | 1,1 / 4,5 | 2,0 s | Visual generic, speed 4 u / s |
| Cổ Môn Vệ Binh | Nền GroundMelee; cấu hình Hybrid còn OPEN | 1,8 | 1,4 / 6 | 1,6 s | Visual generic, speed 6 u / s |

**Số lượng/identity Hybrid còn OPEN**, không khóa ba Hybrid hoặc 3/1/0. Ba dòng đánh xa trên chỉ giữ tham số candidate của baseline cũ, không yêu cầu triển khai đánh xa cho cả ba. Nếu chọn Hybrid, dùng capability GroundRanged/Hybrid tái sử dụng và đặt điều kiện cận chiến/đánh xa rõ; không cho mọi GroundMelee một ranged fallback. AI dùng profile/config chung như GroundMelee, FlyingRanged và capability Boss, không behavior riêng theo tên loài. FlyingBox của Ong ~6 × 3 u là phạm vi bay, không phải độ cao lơ lửng cố định. Ong phải tiếp cận vào vùng cận chiến theo chiều dọc khi giao tranh, không treo mãi ngoài tầm Kiếm; thời gian bay/tiếp cận thử tại PHY-01/ART-01. Một Linh Đạn dùng chung đổi scale/tint/speed/trail; không họ projectile hoặc animation projectile riêng theo loài. Pose ra đòn của actor chỉ bổ sung khi capability được chọn, theo accounting có điều kiện tại Art.

Stat normal tại level L (`round(x) = floor(x + 0.5)` cho x ≥ 0, kể cả EXP / final Damage): `HP = round(40 + 1.04*L*L*L)` khi L ≤ 5; L>5 đặt `x = L - 5`, `HP = round(170 + 50*x + 2.1*x*x)`; `DEF = round(2 + 0.8*L)`; `ATK = round(6 + 1.5*L + 0.06*L*L)`; `ACC = 60 + 4*L`; `EVA = 20 + 2*L`; `NormalEXP = round(10 + 2.5*L + 0.12*L*L)`; `GoldMin = 3 + 2*L`; `GoldMax = 6 + 3*L` (integer uniform inclusive). ATK mid/late tăng để bù một phần HP player tăng theo cấp; HP mob giữ theo TTK probe. Một curve chung, chỉ evaluate ở bảy identity-levels đã author; hp / atk multipliers mặc định 1.0, chỉ tune có evidence sau playtest. Normal / variant slot respawn **25 s BASELINE / TUNABLE**, test 20 / 25 / 30 s, tính từ death; không timer variant riêng. Mục tiêu vòng bãi: clear A → nhặt → B / C / D → quay lại, không đứng nguyên một pocket đợi respawn.

TTK target cùng level + Common + 0: early 2–4 s, mid 3–6 s, late 4–8 s TEST, không guarantee mọi build. Đánh Thạch Lv 16 khi player15 trước đại chiêu còn là probe chậm; giữ HP curve, không tăng mọi HP chỉ để kéo giờ chơi. Các phép thử solo, 2–4 mục tiêu, Q8 Linh và đồ chậm hơn mốc cấp được quản lý tại Analysis.

**Mob attack contract:** đi qua aggro radius vẫn bị acquire / chase, body overlap không gây damage. Melee: Acquire → Chase → attack range → Face → Windup / lock facing → HitMoment / front hitbox → Recovery / reposition ngắn khi có chỗ hợp lệ → tiếp cận lại. Target chạy xuyên ra sau / nhảy ra khỏi vertical range / rời hitbox trước HitMoment thì MISS; không guaranteed damage vì animation đã start, không quay 180° giữa swing. Ranged / Hybrid: Acquire → Aim / Windup → resolveMoment → authority logical target resolve → result → visual projectile; AnimationEvent chỉ visual. Mob normal attack power 1.0, CritChance 0 P0; formula Damage chung, exact hitbox / windup tại PHY-01 / ART-01.

<a id="melee-crowd"></a>

**Melee crowd — flow tái sử dụng:** `Approach → Contact hoặc Staging → Attack → Recovery/Reposition`. `Staging` là vị trí chờ gần tầm đánh: quái sau phải chờ/chỉnh bước có lý do đọc được, không trông như bị đồng đội chắn tường. **Occupied != blocked:** chỗ đã có quái không đồng nghĩa terrain wall. Không body blocking Player–Mob/Mob–Mob; chỉ separation nhẹ cho hình dễ đọc.

- Quái trước có thể nhường điểm tiếp xúc trong recovery; quái sau tiến/chỉnh vị trí hợp lệ. Không teleport, rear melee không tự thành ranged.
- Ưu tiên chỗ trái/phải trên WalkRegion, hòa dùng ID ổn định; deadband và thời gian hạn chế đổi phía 1,5 s là BASELINE/TUNABLE. Không đảo phía liên tục sau mỗi hit.
- Không formation cứng, bốn slot cố định, vòng tròn hoặc group attack token. Recovery/reposition dùng interval hiện có, không thêm cửa miễn đòn hoặc tăng HP/range/tốc độ để ép metric.
- Origin/facing lúc windup/hit không bị steering thay đổi; phase lệch nhau theo life. N-player probe phải kiểm nhiều target và cụm trong cùng camera.

**Mob / Linh Biến threat:** một `Threat[playerId]` và `Contribution[playerId]` riêng mỗi mob. Initial acquire nearest valid player hoặc attacker đầu tiên; direct / DoT cộng ActualHpLost (cap overkill, dedup), không raw damage. Sticky target: challenger có threat>0 và ≥ 1,25 × current mới đổi; current invalid / dead / disconnect / khác MapId / out-of-leash thì chọn highest valid threat, tie playerId; nếu không có threat chọn nearest valid trong aggro. Báo động cụm chỉ wake, từng mob tự acquire / resolve. Return kết thúc encounter cũ, clear threat/contribution/status và hủy action chưa resolve; không giữ damage từ lượt kéo trước. Đích reset là HP đầy ở home; exact cách hồi/regen, grace và invulnerability khi về còn OPEN/TUNABLE, không coi “đầy ở home” là khóa hồi tức thì lúc bắt đầu Return. Linh Biến dùng cùng resolver, không nearest-only sau acquire.

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

**STRONG DIRECTION bối cảnh:** vùng sơn cước Việt Nam tiền hiện đại giả tưởng, không khóa triều đại/năm lịch sử hoặc tái dựng lịch sử. Sắc thái đi từ dân dã → hiểm trở → huyền bí. Làng gỗ, mái ngói giản lược, tre, cầu gỗ, dược thảo, đèo đá và bia/trấn ấn tận dụng ba environment families, không thêm mechanic hoặc asset family.

Vân Khê nằm trên những Mạch Ấn ngầm, nơi linh khí nuôi rừng núi và giữ phế tích yên giấc. Gần đây, gió núi mang mùi tanh, nấm mọc khác thường, sói bỏ bãi cũ. Tân Lữ là người dự tuyển, lần theo những dấu nhỏ ấy trong lúc học cách tự giữ mình; không có lời tiên tri hay danh phận cứu thế. Lễ Nhập Lộ tại Lv 5 gắn lựa chọn kiếm / cung với việc chính thức bước vào đường tu luyện, bằng lời NPC và thao tác Q6, không cinematic hay quest mới.

| Trụ cột thế giới | Điều được hé lộ |
| --- | --- |
| Mạch Ấn và linh khí | Mạch Ấn bị can thiệp làm dòng linh khí lệch hướng, sinh trọc khí khiến sinh vật hung dữ. Ghi chú quest, tên vật phẩm và mô tả quái nối từng dấu vết. |
| Đoạt Mạch Đạo Tặc | Chúng đục phá Mạch Ấn để lấy linh thạch: lợi trước mắt của con người làm rối trật tự tự nhiên. Biết có đạo tặc chưa đủ kết luận nguồn gây nhiễu. |
| Cự Thú và Dư Ảnh | Huyền Nham Cự Thú là sinh linh thủ hộ cổ xưa bị trọc khí ăn mòn. Hạ nó giúp giải thoát Thủ Vệ; Dư Ảnh là tàn niệm linh lực còn đọng nơi cấm địa. |

Lời NPC ngắn, mộc mạc; không diễn giải hết bí ẩn. Lâm Bá kiệm lời, ấm áp, nhắc đường về; Bách Luyện cộc nhưng trọng người bền chí; Yên Thảo nghiêm về khí huyết và giữ mạng; Mộc An điềm đạm, nhắc nghỉ và giữ đồ; Phong Du gọn lời về thế kiếm, Diệp Lam rõ ràng về khoảng cách; Hạo Vũ sảng khoái, lấy tỷ thí làm lời chào. Những sắc thái này dùng text và nội dung hiện có, không thêm quest/NPC/asset chỉ để kể chuyện.

| Chương | Dải cấp | Sắc thái và diễn tiến | Checkpoint tổng kết |
| --- | --- | --- | --- |
| I — Dấu Nứt Vân Khê | 1–7 | Nhập môn & sinh tồn: làng còn yên, điềm lạ thoáng qua; tự cầm kiếm, dùng thuốc, rèn món đầu tiên | Q7 completed và Lv ≥ 7 |
| II — Theo Dấu Huyền Lộ | 8–17 | Dấn thân & khám phá: lần theo trọc khí; Lv 12 tu luyện ngoại vi Xích Nham, Lv 15 điều tra sâu, Lv 17 phục hồi Huyền Môn | Q11 completed và Lv ≥ 17 |
| III — Huyền Tích Thức Tỉnh | 18–20 khuyến nghị | Thanh tẩy & vấn đạo: phế tích trang nghiêm, Thủ Vệ bị cuồng hóa; phong ấn ổn định sau trận chiến | Q12 completed và Lv 20 |

**Main Story Complete — HOÀN THÀNH CHÍNH TUYẾN:** Q12 khép lại chính tuyến / Chương III. Summary: **CHƯƠNG III HOÀN THÀNH / CHÍNH TUYẾN ĐÃ HOÀN THÀNH** — Lâm Bá: “Tai ương tạm lắng. Đường phía trước còn dài.” Tiếp tục farm Huyền Tích, săn đồ Rare / Epic, nâng đồ III lên +8, săn Linh Biến / Dư Ảnh, tỷ thí và thử build; P1 chỉ có khi được triển khai.

Q12 **per character**: chưa complete hiển thị **Huyền Nham Cự Thú**, đã complete **Dư Ảnh Huyền Nham**, kể cả tracker / banner. Hai tên dùng một entity / sprite / AI / drop, không world story flag.

> **Đọc sâu:** [Design Analysis — review narrative](3_HUYEN_LO_DESIGN_ANALYSIS.md#review-decisions)

## Q1–Q12 và nhiệm vụ

Q1 catch-up Lv 2, Q2 Lv 3, Q3 không EXP (giữ Lv 3), Q4 Nấm/loot catch-up Lv 4, Q5 Sói catch-up Lv 5, `CatchUp = max(0, TargetCumulativeEXP - CharacterTotalEXP)`; BaseEXP = 0 khi đã qua mốc. Q7–Q11 EXP hiện dùng 10% thanh cấp (half-up), Q3/Q6 = 0, Q12 = 0 ở cap; các số là BASELINE / TUNABLE cho QUEST-03. **Giữ 12 QuestId**: các bước ngắn đầu game phục vụ onboarding/recovery, Q9 là nhánh tùy chọn và không chặn Q10. Không thêm ID chỉ để lấp khoảng farm.

**State:** LOCKED → AVAILABLE khi đủ prerequisite + level → nhận → IN_PROGRESS → đủ active objectives → READY_TO_TURN_IN → đúng NPC/range → COMPLETED + reward/unlock. Ready không auto-turn-in/chuyển map; thiếu cấp thì NPC/HUD chỉ rõ mốc và bãi phù hợp. Lối cũ vẫn quay lại được.

**Objective theo hành động:** quest data lưu Jump/DropThrough/UseFood/UseHpPotion/UseMpPotion/Interact/Pickup, không lưu phím. Tutorial/HUD lấy glyph từ binding hiện hành ở §9; key press đơn thuần không credit action thất bại.

**Marker/vùng nhiệm vụ:** mỗi ID dưới là khóa của trigger hoặc điểm tương tác trên map/NPC hiện có; Game Server kiểm MapId/vị trí. Visit radius 1,5 u TEST, Interact trong 2 u TEST (binding tại §9). Waypoint Huyền Môn đang khóa vẫn interact được từ phía ngoài Xích Nham ở Q11. Dấu `+` trong objectives chỉ các mục tiêu cùng active group, không phải thêm QuestId.

| Quest / Lv | Story + người giao → trả | Objectives theo thứ tự | Reward | Unlock / next |
| --- | --- | --- | --- | --- |
| Q1 — Người mới đến Vân Khê / 1 | Lâm Bá: “Nhớ chỗ thuốc, lò rèn và đường về. Ra núi rồi, chẳng ai giữ hộ mạng mình.” Lâm Bá → Lâm Bá. | Đi tới khu dược nói chuyện Yên Thảo → lò rèn gặp Bách Luyện → nhà kho/nghỉ gặp Mộc An → nhìn lối đi ra/về làng rồi báo Lâm Bá. Các NPC ở khu chức năng riêng, không xếp cạnh nhau thành menu. | Catch-up Lv 2 + 50 Vàng | Q2 sau Q1 + Lv 2 |
| Q2 — Bước chân đầu tiên / 2 | Lâm Bá: “Đi thử một vòng. Chân vững rồi hãy cầm kiếm.” Lâm Bá → Lâm Bá. | `HV_Entrance` → nhảy tới `HV_JumpLedge` → đi xuống xuyên sàn tới `HV_DropLanding` → đi qua MapExit về Vân Khê → báo Lâm Bá. | Catch-up Lv 3 + 75 Vàng | Q3 sau Q2 + Lv 3 |
| Q3 — Vũ khí trong tay / 3 | Bách Luyện: “Cầm thử cây kiếm gỗ này. Ra sân tập cho quen tay, rồi trở lại đây.” Bách Luyện → Bách Luyện. | Nhận/mặc Mộc Kiếm → `HV_DummyYard` → hạ 3 Bù Nhìn, mỗi life đóng góp ≥20% → báo Bách Luyện. Yard có ít nhất 3 Dummy cùng lúc; HP 60, không đánh/trả thưởng; respawn 25 s TEST. | Mộc Kiếm cấp trước một lần; turn-in Quần Thanh Mộc I, 0 EXP | Q4 sau Q3 + Lv 3 |
| Q4 — Chiến lợi phẩm đầu tiên / 3 | Bách Luyện: “Thứ mặc được thì giữ. Thứ thừa đem bán, lấy đồng lộ phí.” Bách Luyện → Bách Luyện. | `DS2_MushroomPatch`: hạ 1 Nấm Lv 2 khi đúng step, đóng góp ≥20% → nhặt Áo Thanh Mộc + Nấm Sương tutorial → mặc áo → bán sample cho Bách Luyện → báo Bách Luyện. | Áo I + sample cấp theo bước; turn-in catch-up Lv 4 | Q5 sau Q4 + Lv 4 |
| Q5 — Sinh tồn ngoài làng / 4 | Yên Thảo: “Ăn trước khi đi. Thuốc để dành lúc cần.” Yên Thảo → Yên Thảo. | Ứng 320 Vàng một lần → chuẩn bị Food I + HP Potion I (mua nếu chưa có; món hợp lệ đang có cũng tính) → dùng Food → `DS4_ExitTrail`, hạ **5 Sói Sương Lv 4 trong DS3–DS6** → báo Yên Thảo. Bình Máu được giới thiệu, chỉ dùng khi thiếu HP, không objective tiêu lúc đầy; Bình MP dạy ở Q6. | Turn-in catch-up Lv 5 | Q6 sau Q5 + Lv 5 |
| Q6 — Lễ Nhập Lộ / 5 | Lâm Bá: “Qua Học Viện gặp hai người hướng dẫn. Kiếm hay cung, con tự chọn.” Lâm Bá → mentor đã chọn (Phong Du hoặc Diệp Lam). | `HV_ClassHall`: tháo Mộc Kiếm vào túi → chọn Kiếm tại Phong Du hoặc Cung tại Diệp Lam (điểm đã được trả lại một lần khi lên Lv 5) → nhận weapon I + bí kíp nhập môn; mặc weapon + xác nhận đã cộng ≥1 điểm + học sách → cast S1 ở `HV_DummyYard` → nhận MP Potion I dự trữ, dùng khi thiếu MP → báo đúng mentor đã chọn tại Học Viện. Không quay qua NPC trung gian. Bảng skill hiện nội tại Lv 5 mở/Lv 13 khóa. | Class, weapon + bí kíp theo bước, MP Potion I; 0 EXP | Q7 sau Q6 + Lv 7; mở Trúc Ảnh |
| Q7 — Tinh Thạch đầu tiên / 7 | Bách Luyện: “Một nhát búa không thành đồ tốt. Cứ làm cho đều tay.” Bách Luyện → Bách Luyện. | Chọn nhẫn I đang sở hữu (thiếu mới cấp) → preview → nâng +0→+1 hoặc xác nhận nhẫn đã ≥+1 → mặc/xác nhận đúng nhẫn → báo Bách Luyện. | Nhẫn I nếu thiếu; 1 đá + 100 Vàng dự trữ chỉ khi cần nâng; turn-in 83 EXP | Q8 sau Q7 + Lv 8; hết Chương I |
| Q8 — Bóng sói trong Trúc Ảnh / 8 | Lâm Bá: “Vết cào này lạ. Xem dưới chân cầu có gì.” Mảnh ấn mang vết đục của người. Lâm Bá → Lâm Bá. | Tương tác `TA4_BrokenSeal` → hạ **4 Sói Trúc Ảnh Lv 8 tại TA4 + TA6**, nhận 2 Dấu Trọc Khí virtual ở qualifying kill #1/#2 → tương tác dấu ấn → hạ 1 Sói Trúc Ảnh **Linh Biến tại `TA4.slot1`** với ≥20% → báo Lâm Bá. | 108 EXP + bí kíp tiến cảnh đúng class (dùng từ Lv 10) + 2 đá | Mở Bạch Vân; Xích Nham thêm Lv 12; Q9 tùy chọn Lv 12 / Q10 Lv 15 cần Q8 |
| Q9 — Khảo Chiến Đồng Môn / 12 | Hạo Vũ: “Có đồng môn thì thử vài đường. Chưa gặp ai, cứ đi tiếp.” Hạo Vũ → Hạo Vũ. | Nói chuyện Hạo Vũ → mời/chấp nhận cược 1v1 → hoàn thành một trận đấu thật có kết quả thắng/thua/hòa → báo Hạo Vũ. FORFEIT do ngắt kết nối và SYSTEM_ABORT không tính mục tiêu. | 270 EXP + 200 Vàng sau PvP thật | Optional; không chặn Q10, bỏ qua không thưởng |
| Q10 — Dấu chân Xích Nham / 15 | Lâm Bá: “Dấu đục trên đá không do thú rừng. Mang chứng cứ về, ta sẽ cùng xem.” Lâm Bá → Lâm Bá. | Tương tác `XN1_ChiselMarks` → hạ **6 Đoạt Mạch Lv 13 tại XN1–XN3**, nhận 3 Vật Chứng virtual ở qualifying kill #2/#4/#6 → Tương tác `XN2_SealScar` → báo Lâm Bá. | 480 EXP + 3 đá | Q11 sau Q10 + Lv 17 |
| Q11 — Mở Lối Huyền Môn / 17 | Lâm Bá: “Nối lại từng dấu ấn. Chỉ chạm Huyền Môn khi mạch đất đã yên.” Lâm Bá → Lâm Bá. | Tương tác `XN4_SealA` + hạ **3 Thạch Lv 16 tại XN4** → Mảnh 1; Tương tác `XN5_SealB` + **3 tại XN5** → Mảnh 2; Tương tác `XN6_SealC` + **4 tại XN6** → Mảnh 3; Tương tác `XN_HuyenMon_Outer` kiểm đủ mảnh/kích hoạt → báo Lâm Bá. | 670 EXP + Rare III weapon + bí kíp đại chiêu đúng class | Chỉ **Completed** mới mở Huyền Tích; Q12 thêm Lv 20; hết Chương II |
| Q12 — Tiếng gọi từ Huyền Tích / 20 | Lâm Bá: “Giúp Thủ Vệ buông gánh cũ. Trở về rồi kể ta nghe.” Lâm Bá → Lâm Bá. | `HT4_GuardRoute`: hạ **6 Cổ Vệ Lv 20 tại HT4 + HT5** → tới `HT_BossLandmark` → đóng góp ≥10% HP trong **một life Boss** ở death khi đúng step → báo Lâm Bá. | 1.000 Vàng một lần; 0 EXP, không thêm Boss pile | Main Story/Chương III complete; Dư Ảnh và farm tiếp tục |

**Đồ tutorial chỉ cấp ở bước đang làm:** kill/grant chỉ tạo đồ hướng dẫn/ràng buộc nhiệm vụ và quyền nhận khi đúng QuestId, InProgress, bước đang active và nguồn. Ngoài bước đó, quái chỉ roll đồ thường; không sinh áo/sample tutorial hoặc quyền nhận cho nhiệm vụ tương lai. Quyền đã tạo hợp lệ giữ cùng instance để thử nhận lại khi đồ trên đất hết hạn hoặc túi đầy; không dùng recovery này để hồi tố kill cũ.

**Recovery chung:** talk/visit/kill chỉ credit sau Game Server event hợp lệ; lưu step/counter sau commit, death/reconnect không xóa. Ngã ở Q2 thì thử lại, không giả credit chỉ vì bấm phím. Mộc Kiếm Q3 không bán/vứt, grant pending nếu túi đầy. Q5 không cấp lại 320 Vàng khi replay; dùng bình lúc đầy HP bị từ chối mà không tiêu thuốc. Q4 áo giữ binding tới lúc mặc, sample chỉ bán đúng step; hết hạn ground/full bag/reconnect giữ entitlement chưa claim với cùng itemInstanceId. Quest reward chỉ trao tại đúng NPC sau capacity preflight.

| Quest cần xử lý riêng | Contract không lặp lại trong bảng objectives |
| --- | --- |
| Q6 | Reset Lv 5 đúng một lần kể cả chọn class muộn; giữ điểm lên cấp sau đó. Equip/learn/xác nhận điểm cùng active group; đã cộng ≥1 điểm chấp nhận, không ép cộng mới khi pool 0. Manual grant/learn và thuốc dự trữ có receipt; nếu Food hồi đầy MP, cast lại để dùng Bình Linh Lực thật, không fake consume. |
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

Điểm Journey theo §6; đủ điều kiện chương/truyện không tự hoàn thành nhiệm vụ khi Boss chết.

> **Đọc sâu:** [Design Analysis — progression và quyết định](3_HUYEN_LO_DESIGN_ANALYSIS.md#quest-progression)

---

<a id="gear-economy"></a>
<a id="gdd-7"></a>

# 6. Trang bị, đồ rơi và kinh tế

## Danh mục — 18 dòng trang bị thường

Sáu ô × ba bậc tiến trình = **18 family trang bị**. Vũ khí mỗi bậc tách Kiếm/Cung: **21 mẫu thường cụ thể** (15 mẫu không phải vũ khí + 6 vũ khí); phẩm chất/cấp cường hóa thuộc trạng thái instance. **22 ItemDefinition trang bị = 21 mẫu thường + Mộc Kiếm Q3**. Tổng này chưa tính Food/Bình/bí kíp/nguyên liệu/phù. Mộc Kiếm là 1 tutorial exception ngoài 18 families; ATK 10 để giữ Q4 TTK khi basic Tân Lữ là 0,70 s (mô hình 1,00 s cũ là LEGACY), không sell / drop. Weapon class lock giữ, Family I Sword / Bow cần Lv 5 dù non-weapon mặc từ Lv 1.

**Mốc mặc đồ dễ nhớ:** Band I non-weapon Lv 1, Band I Weapon Lv 5 sau chọn class, **mọi món Band II Lv 11**, **mọi món Band III Lv 17**. Nguồn gear theo mob/content, không suy từ tên map; bảng source bên dưới là authority chung cho loot/potion/material. Shop chỉ Common I/II; III không bán, Q11 Rare weapon đúng class là guaranteed exception. Mộc Kiếm quest-only, ATK 10, không bán/cường hóa/chuyển giao. Off-class weapon bán được, không equip. Weapon shop chỉ mua khi đã chọn đúng class; đồ II có thể mua/nhặt từ Ong Lv 10 trước Lv 11 nhưng chưa mặc.

**STRONG DIRECTION:** Armor/Pants/Boots luôn có HP; Weapon/Ring/Necklace luôn có MP. Armor HP mạnh + DEF, Pants HP vừa + DEF, Boots HP nhẹ + EVA/tốc chạy; Weapon ATK + MP + secondary theo phái, Ring MP + ACC/Crit, Necklace MP mạnh + EVA/utility. Stat do `ItemDefinition/GearSlot` quyết định, không suy từ vị trí trái/phải trên UI.

**Catalog — Common +0, PROBE BASELINE/TUNABLE 2026-10-06.** HP/MP được tăng vừa phải để kiểm cadence mới; ATK/DEF/ACC/EVA/Crit/tốc chạy, giá, nguồn và trần enhance giữ nguyên. [Analysis](3_HUYEN_LO_DESIGN_ANALYSIS.md#current-balance-probe) giữ so trước/sau, sustain và biên rarity/enhance. Rarity drop theo channel table; shop không bán Uncommon/Rare/Epic. Cột source I/II/III dẫn bảng mob/content; ngoại lệ quest ghi riêng. Mỗi bậc có hai mẫu vũ khí mang tên riêng theo phái, cùng ATK/giá nhưng Kiếm thêm Chí mạng và Cung thêm Chính xác; **18 family = 21 mẫu thường** vì ba family vũ khí tách Kiếm/Cung. Mộc Kiếm là mẫu ngoại lệ ngoài 18 family.

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

Thứ tự tính từng món: ATK/HP/MP/DEF gốc × hệ số phẩm chất × hệ số cường hóa; ACC/EVA gốc (gồm ACC Cung) thuộc cột chỉ số chịu phẩm chất chỉ × hệ số phẩm chất, **không** × hệ số cường hóa; Chí mạng gốc của Kiếm/Nhẫn và tốc chạy Giày giữ nguyên. Sau đó cộng riêng theo cấp: Giày +2 EVA/cấp; Nhẫn +2 ACC và +0,2 điểm % Chí mạng/cấp; Dây chuyền +2 EVA/cấp. Cộng Tinh Hoa I/II vào chỉ số tương ứng sau phép nhân, không nhân chúng lại với phẩm chất/cường hóa. Cộng các món với nền nhân vật và điểm thuộc tính; cuối cùng mới áp nội tại nền tảng Kiếm/Cung một lần. Tốc chạy cộng tuyến tính: `MoveSpeedMultiplier = 1 + 0,0005 × AGI + Giày%/100`; không nhân lại theo phẩm chất/cường hóa. Giữ số lẻ nội bộ, chỉ làm tròn khi UI/sát thương cần. Tinh Hoa tính từ cấp item, không cộng dồn lại khi mặc hoặc tải save.

Bảng tra đầy đủ chỉ số **Common từ +0 đến trần từng bậc**, gồm cả Mộc Kiếm không thể nâng, nằm tại [Analysis §4](3_HUYEN_LO_DESIGN_ANALYSIS.md#gear-upgrade-values). Công thức và giới hạn trong GDD này là luật nếu cần tính phẩm chất khác Common.

**Ví dụ kiểm chứng:** Áo giáp Rare III +8 có HP = 138 × 1,16 × 1,59 + 10 (Tinh Hoa I) + 0 (Tinh Hoa II không cho HP) = **264,5272**; DEF = 15 × 1,16 × 1,59 + 2 (Tinh Hoa II) = **29,666**. Khi mặc, cộng hai giá trị này vào tổng HP/DEF trước nội tại Kiếm Tâm; Kiếm Tâm mới nhân MaxHP ×1,10 và DEF ×1,08. Preview, stat panel và save load phải dùng đúng cùng phép tính.

## Chuyển giao cường hóa — P0 tại Bách Luyện

Người chơi chọn **đồ nguồn + đồ đích đã sở hữu**, khác instance, cùng ô, đều ở trong túi (tháo món đang mặc trước). Vũ khí phải cùng loại Kiếm/Cung, đúng phái; nhân vật phải đủ cấp mặc đồ đích. Mộc Kiếm, bí kíp, vật phẩm nhiệm vụ và đồ đang ràng buộc hướng dẫn không dùng được. Đích giữ nguyên template, phẩm chất và instanceID; nguồn bị tiêu, không sao chép trang bị hay phẩm chất.

| Kiểu chuyển | Điều kiện bậc | Cấp đích sau chuyển | Chi phí |
| --- | --- | --- | --- |
| Đổi món trong cùng bậc, thường để lên phẩm chất | I→I, II→II hoặc III→III | max(cấp đích, cấp nguồn) | 800 Vàng; không Tinh Thạch |
| Sang bậc kế tiếp | I→II hoặc II→III; không nhảy I→III | max(cấp đích, cấp nguồn) | 500 Vàng + 2 Tinh Thạch |

Nếu cấp đích không tăng, từ chối trước khi tiêu gì; cấp chuyển không được vượt trần bậc đích. Đường nâng dễ nhớ: **I +4 → II +4 → II +6 → III +6 → III +8**. Không tạo vật phẩm chuyển giao. Preview nêu nguồn sẽ mất, Vàng/đá, chỉ số đích trước/sau và trạng thái hai Tinh Hoa. Tiêu nguồn, trả chi phí, cập nhật đúng instance đích và lưu cùng một giao dịch; replay/reconnect trả kết quả cũ, không trừ hai lần. Tinh Hoa luôn tính từ cấp đích sau commit, không copy giá trị cộng từ nguồn. Không bán nguồn trong cùng giao dịch. Evidence chi phí tại [Analysis §4](3_HUYEN_LO_DESIGN_ANALYSIS.md#economy-analysis).

**Luồng đồ rơi:** Vàng/nguyên liệu/Bình/Tinh Thạch là các lượt roll độc lập; trang bị dùng **một roll loại trừ giữa các phẩm chất**. Một kill có thể cho Vàng + nguyên liệu + đá + đồ, nhưng không roll hai phẩm chất trang bị. Tỷ lệ/số lượng là TEST/TUNABLE; ngữ nghĩa dưới đây là baseline triển khai, LOOT-01 còn kiểm tuning/kinh tế.

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

Áp riêng từng recipient, snapshot trước level-up, đối xứng cả player quá cao lẫn quá thấp; không có tầng nửa thưởng hay gate RNG theo level. Lv 20 EXP luôn0, Gold/loot/Journey vẫn hợp lệ. Quest/supply/evidence xét riêng §5, vẫn làm quest muộn. Reward eligibility hiển thị tooltip/reject feedback khi cần, không spam trên mọi nameplate; level/quest/Linh/Boss cues giữ vai trò riêng.

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
| Q3 / Q4 / Q6 / Q7 / Q11 | Quest exceptions trong catalog | Tutorial supplies theo quest | Evidence virtual riêng, không farm material | Guaranteed, không loot roll |

Quest evidence không dùng Trade Material đã farm; tooltip material “Vật liệu giao dịch — có thể bán”. Tinh Thạch không có tier riêng. Source Nấm / Sói Lv 4 loại Weapon ở **mọi map**, không dùng MapId để thay slot pool.

**Vendor economy — BASELINE / TUNABLE:** ItemDefinition buyPrice optional / sellValue explicit. Rarity sell Common 1 / Uncommon 1,5 / Rare 2 / Epic 3 trên Common sell, floor một lần; enhance không tăng sell / hoàn resource. Các supply khác sell = floor(buy × 0,25); drop-only không fake buyPrice. Shop stock vô hạn.


**Boss contributor / quest / Journey:** ≥ 10% RuntimeBossMaxHP (release 3.200; demo 12.800 → 1.280), connected / sameMap / insideBossCombatArea tại death; corpse trong area hợp lệ, về làng / disconnect thì không. Q12 dùng active-step damage, không hồi tố; tối đa 10 recipients có thể đạt threshold trên full-health life. Contributor không tạo personal pile.

Normal quest 20% còn phải alive / inAssistRadius 8 u / lastDamage ≤ 10 s, độc lập EXP / Gold>0. Normal contributor pickup 5% theo cùng eligibility. Journey normal 5 / Linh 100 cho regular recipient ≥ 20%, nhân own level factor / floor; Boss 500 / deathID cho qualifying 10%; quest 150 / chapter 300 một lần, PvPwin 200 / matchID. Không claim points bằng pickup.

Bag 30 / storage 40; stack 99, gear 1. Pickup stackable ưu tiên fill compatible stack có sẵn, overflow sang stack mới; transaction không fit toàn bộ thì không consume ground item. Bag đầy nhưng stack còn chỗ vẫn nhặt được. Ground Normal / Linh Biến 60 s / Boss 90 s, chỉ physical items; unequip cần ô trống, không bán equipped.

Turn-in tính X ô trống thực cần sau merge stack: thiếu thì báo “Cần X ô trống trong hành trang”, giữ READY_TO_TURN_IN; không consume evidence / trao một phần reward / set Completed. Vàng / EXP / story / Journey không cần slot; retry không nhận lặp.

Yên Thảo bán Food / Potion / phù; Bách Luyện bán Common I / II, Tinh Thạch **800 Vàng**, upgrade / sell; Yên Thảo bán Tẩy Mạch. Q9 không gear-exclusive, skip không mất nâng slot. Một tiền tệ Vàng. Bag Sort / protection gear P1; validation inventory P0.

> **Đọc sâu:** [Design Analysis — kinh tế và enhance](3_HUYEN_LO_DESIGN_ANALYSIS.md#economy-analysis)

---

<a id="consumables-death"></a>
<a id="gdd-8"></a>

# 7. Vật phẩm tiêu hao và tử vong

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

Mọi loại Bình Sinh Lực dùng chung **hồi chiêu HP 8 s**; mọi loại Bình Linh Lực dùng chung **hồi chiêu MP 8 s**. Hai nhóm **tách nhau**: dùng HP không khóa MP và ngược lại. Không có hồi chiêu riêng từng bậc bình.

HP ≤ 0: Death tint / khóa movement, attack, skill, pickup; camera / corpse tại MapId, không auto-respawn. Về Vân Khê miễn phí full HP / MP hoặc tiêu phù hồi tại chỗ. Consequence phù / run-back; không EXP debt / mất gear / Vàng. Death là mốc checkpoint HP = 0: nếu phiên mất, khôi phục tại SafeAnchor của MapId ở trạng thái chết, vẫn phải chọn về làng hoặc dùng phù; relog không tự hồi sinh.

Corpse đủ 10% còn trong BossCombatArea tại Huyền Tích được Boss quest / contributor snapshot, nhưng chỉ nhặt đồ sau hồi sinh; Về Làng trước khi Boss chết thì mất quyền contributor/quest credit. Pile chung vẫn theo các cửa nhặt. Người chết không giữ Boss khỏi reset khi không còn ai sống trong vùng đánh. Food **không hồi máu/Linh lực khi đã chết**. Khi phiên chơi mất, Food/status/cooldown hết hiệu lực; lần vào sau dùng **MapId + HP + MP đã checkpoint**, spawn tại SafeAnchor của map farm/combat, hoặc tọa độ hợp lệ ở khu an toàn (§8). HP/MP được clamp theo Max hiện hành; không restore vị trí chính xác giữa bãi, không tự hồi đầy. Reconnect ngắn khi Game Server vẫn giữ phiên có thể tiếp tục state realtime đó.

> **Đọc sâu:** [Design Analysis — Food / MP sustain](3_HUYEN_LO_DESIGN_ANALYSIS.md#combat-analysis)

---

<a id="online-social"></a>
<a id="gdd-9"></a>

# 8. Online, PvP và giao tiếp

Online multiplayer theo mô hình Client–Server. Player đăng nhập bằng tài khoản do admin cấp, chọn nhân vật rồi kết nối Unity Dedicated Game Server; không có Register cho player. Game Server quyết định combat, quái, Boss và kết quả trận PvP; Spring Boot/PostgreSQL lưu tiến trình, tạm giữ và quyết toán Vàng cược. Client không gửi level / Vàng / EXP / gear / thuộc tính như dữ liệu đáng tin. **P0 acceptance: tối thiểu 2 concurrent players** — hai Client kết nối Game Server; đây là mức nghiệm thu, không phải giới hạn world. P0 chạy local/LAN; triển khai Internet công khai ngoài phạm vi P0.

MapId P0 kiểm combat / mob / map transition / loot / chat; khác map không tương tác, chỉ render khu hiện tại. Hide / show P1 không gate slice. Boss timer / banner global khi map rỗng; tên Cự Thú / Dư Ảnh theo character, không tạo entity khác.

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

**LOCKED:** Move dùng ←/→, Jump dùng ↑, DropThrough dùng ↓ trên one-way đang đứng. Letter keys được giải phóng khỏi movement; A/D/Space/S không còn primary gameplay bindings. Một action Move không cộng đôi tốc độ. Jump và DropThrough cùng frame trên one-way thì Drop ưu tiên; trên solid Drop no-op, Jump vẫn hợp lệ. Text input/modal giữ input, không để movement/combat lọt qua. Toàn bộ vòng chơi/menu P0 dùng được bằng bàn phím, chuột là cách bổ sung.

| Binding / trạng thái | Semantic action | Hành vi |
| --- | --- | --- |
| ← / → — LOCKED | Move | Đi ngang thủ công |
| ↑ — LOCKED | Jump | Nhảy theo movement state |
| ↓ — LOCKED | DropThrough | Một lần xuyên one-way hợp lệ đang đứng |
| 1 / 2 / 3 — LOCKED | SelectSkillSlot1 / 2 / 3 | Chỉ chọn kỹ năng; không action/MP/CD/approach |
| Phím còn OPEN | ExecuteSelected | Thực thi kỹ năng đã chọn theo §3 |
| Tab / Shift+Tab | CycleTarget | World đổi target cục bộ; trong modal thuộc UI navigation |
| Phím còn OPEN | QuickHP / QuickMP / Food / Interact | Gọi validator dùng đồ/tương tác, độc lập CombatFocus |
| Navigate / Confirm / Back | UI actions | Điều hướng, xác nhận và lùi/đóng theo context |
| Enter / Esc | Chat / Back hoặc clear theo context | Chat theo §8; Esc chỉ xử lý một tầng theo §3 |

**PROPOSAL / TUNABLE DEFAULT để thử usability, không LOCK:** E→ExecuteSelected, F→Interact, 4/5→QuickHP/QuickMP, R→Food, I→RPG menu. C/Q và các letter khác chưa được gán mechanic mới. Không thêm full remapping P0. Nếu dùng bảng này trong probe, HUD/tutorial phải lấy glyph từ binding thực, không hard-code phím vào quest data.

**Hướng UX đề xuất — PROPOSAL:** hợp nhất các view RPG trong một giao diện chung mở bằng một action menu; cấu trúc giao diện/phím I/C/Q còn OPEN. Inventory, Equipment, Attributes, Derived Stats/Thông số, Skills và Quest vẫn đủ chức năng. Menu action, Navigate/Confirm/Back tách khỏi gameplay, không cần key riêng cho từng view.

**View inventory/NPC:** NPC hiện hội thoại ngắn và marker `!` khi Available, `?` khi Ready; chọn chức năng rồi mở submenu riêng (mua, bán, gửi/lấy rương), không trải mọi item/action trên một menu NPC. Hành trang 30 ô dùng lưới icon + stack count, một bảng chi tiết cho ô đang chọn; Enter/Interact mở thao tác của đúng instance. Trang bị nằm ở view Nhân vật với hình người và sáu slot quanh hình, tách khỏi bag grid. Navigate/Tab navigation, mouse click, Confirm/Interact và Esc/back dùng cùng commands; không bắt click. Chi tiết bố cục ở [Art — map/UI blockout](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#icons-ui).

**Menu bằng bàn phím:** Interact mở NPC với action phù hợp được chọn sẵn (nhận/trả quest trước, rồi service). Trong modal: Navigate chọn ô/action, Tab/Shift+Tab đổi lựa chọn/view; Confirm hoặc Interact xác nhận; Esc đóng. Move/Jump/DropThrough/SelectSkillSlot/ExecuteSelected không lọt thành gameplay khi UI giữ focus. Enter chỉ mở/submit chat khi không có modal khác; Tab ở modal là UI navigation; ở world dùng [CycleTarget](#focus-input). Inventory/equip/learn, shop buy/sell, character/skill tab, rương và revive đều có focus rõ, text/action disabled reason và cùng command validation cho chuột/bàn phím. Không yêu cầu click để hoàn tất quest. Đổi mục tiêu và vòng đời focus theo [§3](#focus-input); menu không nhận world CycleTarget.

**Gate cảm giác di chuyển:** thử khoảng cho nhảy ngay sau khi rời mép (coyote time), nhớ input nhảy ngay trước khi chạm đất (jump buffer), độ cao theo thời điểm thả phím, tăng/giảm tốc trên đất và tốc rơi. Các số là PROTOTYPE, chưa khóa trước khi có collider/tỷ lệ/map/mẫu art. Không thêm double-jump/dash. Drop không hưởng coyote để nhảy bật ngược lên sàn; authority và client phải dùng cùng semantics. Xem contract/probe Technical.

<a id="quick-items"></a>

QuickHP và QuickMP chọn bình **bậc thấp nhất hiện có, đủ cấp dùng và đủ hồi phần HP/MP đang thiếu**; nếu không bình nào đủ bù, dùng bậc cao nhất hợp lệ. Game Server kiểm túi, cấp, số lượng và hồi chiêu; đầy HP/MP hoặc đã chết thì từ chối, không tiêu bình. Q6 dùng Bình Linh Lực I đã phát trước bình khác để không kẹt hướng dẫn. Food dùng bậc cao nhất hợp lệ; Food mới thay hiệu ứng cũ và đặt lại thời hạn 10 phút, không cộng dồn. QuickHP/QuickMP chỉ dùng bình, không chọn skill. Q6 hiển thị glyph QuickMP theo binding probe. Interact tác động ngay ứng viên NPC/loot riêng, không thay CombatFocus. [Approach không kích hoạt EdgeExit](#pending-cast).

Target HUD tối giản: marker + mini HP trong world; tên/level/current-max HP/bar trên màn hình, bind đúng focus ID/generation/MapId. Player chết vẫn quan sát HP target hợp lệ, kể cả HP đổi do người khác đánh; death không tự ẩn/xóa focus. Không thêm portrait/element/rarity/generic buff panel.

HUD: HP / MP / EXP / level, skill CD, Food / Potion, quest, Boss timer. Trong PvP hiện cược/pot, đồng hồ 120 s và số lần dùng HP/MP Potion còn lại (ban đầu 3/3). Bảng skill hiện ba active tích lũy, selected/locked/CD/MP riêng và hai nội tại/class: icon, tooltip, level/điều kiện khóa, auto-open Lv 5/Lv 13; không thêm hotkey nội tại. Bag-full rõ; tooltip enhance trước / sau. MapExit arrow + tên vùng đích, NPC marker cơ bản P0; SpecialGate có cue riêng. Quest navigation arrow xuyên map vẫn P1. Route automation không chứng minh inventory/shop/quest dễ dùng; cần manual usability review ở tốc độ thường, xem Art/Roadmap.

| Tracker state | Người chơi thấy |
| --- | --- |
| IN_PROGRESS | “Hạ Sói Trúc Ảnh: 2 / 4” (Q8; counts tại §5) |
| READY_TO_TURN_IN | “Quay về gặp Lâm Bá để báo cáo” |
| LEVEL GATE | “Tu luyện đến Lv.12”; NPC giải thích vùng phù hợp |
| AVAILABLE | NPC / quest panel cho nhận, chưa có progress trước khi nhận |
| OPTIONAL Q9 | Nhãn “Tùy chọn — Tỷ thí”, tách quest được pin; không chặn chính tuyến |

| NPC / khu chức năng | Quest / service hiện hành |
| --- | --- |
| Lâm Bá — khu công cộng Vân Khê, dễ thấy | Q1/Q2, giới thiệu Q6; Q8/Q10/Q11/Q12 và truyện/Huyền Môn |
| Yên Thảo — khu dược/thảo mộc | Q5; Food/Potion/Hồi Sinh và Tẩy Mạch/reset stat |
| Bách Luyện — lò rèn/đe | Q3/Q4/Q7; vũ khí/gear/đá/bán/cường hóa/chuyển giao |
| Mộc An — nhà kho/nghỉ | Storage và nghỉ hồi đầy |
| Phong Du — khu Kiếm Học Viện | Mentor Kiếm; giao dịch nhập phái và trả Q6 nếu đã chọn Kiếm |
| Diệp Lam — khu Cung Học Viện | Mentor Cung; giao dịch nhập phái và trả Q6 nếu đã chọn Cung |
| Hạo Vũ — gần biển/lối Lôi Đài | Q9, PvP Challenge |

**Bảy NPC hiện hành.** Tạ Minh là LEGACY đã merge/remove khỏi roster player-facing: truyện/Huyền Môn sang Lâm Bá, Tẩy Mạch sang Yên Thảo, nhập phái/Q6 sang hai mentor. Không tạo NPC thay thế. Học Viện có hai khu mentor và Dummy Yard dễ tìm; không đặt Kiếm như lựa chọn mặc định trước Cung. Exact tọa độ còn OPEN cho blockout. Q1 phải dẫn qua các khu chức năng thật, giữ đường ra/về làng; dialogue/tracker nêu việc → khu vực/đường đi → NPC tiếp theo.

> **Implementation:** [Technical — UI](2_HUYEN_LO_TECHNICAL.md#ui-notes)

## Hợp đồng hình ảnh

Một rig nam với các phần ghép đồng bộ (modular rig), chưa có rig nữ trong MVP: **64 × 64 px / PPU 32**, body **44–48 px**, pivot Bottom-Center(0.5, 0.0), hướng phải / flipX trái. Collider~0,60–0,65 u × 1,45 u TUNABLE, không toàn canvas.

| Animation | Frames | FPS baseline | Animation | Frames | FPS baseline |
| --- | ---: | ---: | --- | ---: | ---: |
| Idle | 4 | 6 | Attack | 3 | 12 |
| Run | 6 | 10 | Skill | 4 | 12 |
| Jump | 2 | 8 | Hit | 2 | 10 |
| Fall | 2 | 8 | Death | 3 | 8 |

**26 frames**, parts đồng bộ index / pivot. Ý nghĩa giới hạn tổng hình raster hay ô/profile đang **OPEN A01**; chưa duyệt thay user-lock bằng 33 pose. [Art §1](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#player-visual) phân biệt ô timeline/hình reuse/pose class; [Analysis](3_HUYEN_LO_DESIGN_ANALYSIS.md#art-open-decisions) giữ trạng thái quyết định. Ba visual families trùng ba gear bands: Thanh Mộc / Vân Nham / Huyền Ấn. Weapon / Armor / Pants modular; Boots / Ring / Necklace icon / stat only. Cùng family reuse silhouette / frame nhưng khác tier có palette / tint hoặc accent rẻ: Thanh Mộc vải / lục; Vân Nham đá / đồng; Huyền Ấn cổ văn. Không tự thêm animation set khi A01/A02 chưa được duyệt; gear vẫn dùng male rig chung, khả năng thêm pose/profile class phải giải quyết OPEN trước. Head / Hair là base visual, không Helmet slot.

Art P0: male rig, ba families, Sword / Bow visuals, sáu normal sprite sets + một palette Sói Trúc Ảnh (bảy identities), không sprite set riêng Linh Biến, một Boss, ba environment families, UI kit, sáu skill bindings, reuse motif/preset hình theo class; số unique VFX còn cần kiểm, Linh Đạn generic, aura Linh Biến, heal / upgrade / death feedback. Ba họ môi trường: rừng ẩm/tre/đồng cho Đồng Sương–Trúc Ảnh; núi đá/vách/thác cho Bạch Vân–Xích Nham; phế tích/trấn ấn cho Huyền Tích. Hub dùng lại đạo cụ kiến trúc phù hợp.

**Yêu cầu hình ảnh người chơi thấy:** default outfit khi chưa mặc/unequip; Mộc Kiếm và sáu vũ khí phái phải nhận diện đúng món; Weapon/Armor/Pants thay hình theo band, phụ kiện stat-only như §6. Cue mặt đứng/one-way/prop, trạng thái Bỏng/Đóng Băng/Làm Chậm và hình đòn không được gây hiểu nhầm về hit/CC/loot. Pose/hybrid/socket, import/layer và số ảnh sản xuất do [Art](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#art-integration) sở hữu; cách tích hợp runtime/physics do [Technical §8](2_HUYEN_LO_TECHNICAL.md#art-contract) sở hữu. Pipeline cũ đã chuyển nguyên sang [Art §22.1](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#legacy-visual-flow).

Flash / damage / heal / NÉ, trail, loot beam và sound; không hard CC mới. Layer / import tại Technical.

| Trạng thái nhìn thấy | Cách đọc trên cùng rig / VFX pool |
| --- | --- |
| Bỏng | Tia lửa/viền ấm nhỏ theo target, tick feedback gọn; không che telegraph hoặc aura Linh Biến. |
| Đóng Băng quái thường / Linh Biến | Phủ băng xanh rõ trong 1,5 s; nứt/tan băng ngắn lúc hết. Cửa miễn 3 s chỉ cần icon nhỏ nếu cần debug, không phủ băng khi đã tan. |
| Băng Hàn Làm Chậm Boss / người chơi | Phủ lam mờ + viền/hạt lạnh nhẹ, **không dùng lớp băng cứng**; Boss có thể hiện icon cạnh HP bar. PvP vẫn đọc được animation đang cast. |

Các overlay chỉ là presentation của status Game Server đã resolve; không thêm sprite/animation rig riêng hoặc đổi collider.

> **Implementation:** [Technical — art / animation](2_HUYEN_LO_TECHNICAL.md#art-contract)
Production accounting thuộc [Art §23](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#production-accounting); phép tính 12 modules trước review được giữ nguyên ở [trace](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#legacy-art-accounting), không là tổng asset đã duyệt.

---


**RPG navigation — chức năng giữ, shell còn PROPOSAL:** Hành trang, Trang bị (sáu slot + preview), Thuộc tính (STR/VIT/INT/AGI/unspent), Thông số (HP/MP/ATK/DEF/ACC/EVA/Crit/MoveSpeed/Class/Lv/EXP), Kỹ năng và Nhiệm vụ đều phải dùng được bằng keyboard. Nếu dùng shell chung, Tab/Shift+Tab đổi view; Navigate chọn ô/action, Confirm xác nhận, Back lùi. Mouse gọi cùng commands/validation. Slot weapon trống mở bag lọc Vũ khí; Store/Take chỉ tại kho, Buy/Sell chỉ tại shop. Exact layout và physical menu key còn OPEN, không khóa I/C/Q.

**Đóng giao diện theo thao tác:** nhận/trả quest, nhập phái và nghỉ thành công đóng hội thoại để tiếp tục đi; câu xác nhận vẫn hiện trên NPC/tracker. Lỗi/reject giữ view và reason. Buy/Sell/Store/Take giữ view để làm nhiều lần; Esc lùi một submenu, ở root thì đóng. Equip/Unequip/Learn thành công trở về view chứa item/slot; tab switch đi trực tiếp tới view mới, không giữ submenu cũ; [Esc ưu tiên theo context](#escape-priority). Intro hiện trước khi nhận quest; không bỏ narrative chỉ vì auto-close.


**Thoại hướng dẫn:** Q1–Q6 dùng 1–3 câu khi nhận nhiệm vụ, phản hồi của NPC trung gian và một câu khi trả; tracker nêu việc → khu vực/đường đi → NPC tiếp theo. Không thêm QuestId, số kill, EXP hay reward từ việc mở rộng hội thoại. Q4 tutorial supply vẫn chỉ đúng active step; Q6 giữ gate tự tháo Mộc Kiếm trước nhập phái.

<a id="acceptance-routing"></a>
<a id="gdd-13"></a>
<a id="gdd-14"></a>

# 10. Nghiệm thu và hướng dẫn tra cứu

**Dev Mode chỉ là tooling:** cho test nhanh bằng preset/lệnh dev không thay canonical Q1–Q12, không tạo tiến trình hợp lệ trong production. Fresh-run acceptance phải chạy route thật từ đầu; [Technical](2_HUYEN_LO_TECHNICAL.md#dev-mode) giữ cách cô lập tooling, [Roadmap](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#dev-speed-acceptance) phân biệt DEV SPEED và ACCEPTANCE EVIDENCE.

**VS-1 hiện có là prototype tham khảo, không phải production codebase hoặc nghiệm thu TARGET.** G-L theo revision mới cần evidence mới; test/video cũ chỉ ghi behavior của revision cũ. Chi tiết CURRENT/DEFERRED nằm ở Roadmap; không ký hoàn thành P0 khi chỉ Kiếm/offline chạy được.

**Chưa nghiệm thu**: cần playable build / evidence hai Client kết nối Dedicated Game Server (tối thiểu 2 concurrent players), không thay bằng simulation hoặc diễn giải thành capacity tối đa.

| Nhóm | Tiêu chuẩn |
| --- | --- |
| Player | Movement ←/→, Jump ↑, DropThrough ↓; Lv 1–20; reset 20 điểm Lv 5; hai class; 95 điểm Lv 20; Tẩy Mạch không mất dữ liệu |
| Combat | Basic Tân Lữ trước class; ba active tích lũy + hai nội tại / class, manual Lv 5 / 10 / 17, CD riêng/common lock; 1–3 chỉ select, ExecuteSelected tạo one-shot approach/cast, giữ không RepeatOnHold; chết giữ valid focus/HUD; shape / maxTargets / falloff; snapshot / pierce / explosion không double-hit; Evade / Crit; Bỏng / Băng Hàn đúng target branch, post-thaw protection target-wide |
| World | Năm farm maps, ba support zones; SpawnGroup độc lập, HomeRegion/WalkRegion/Return/respawn; mật độ re-author, solid trực giao/one-way cấu trúc, không slope/climb; đúng bảy fixed-level identities / sáu rigs, Linh Biến max 1 / MapId và Q8 deterministic, một Boss với telegraph / target / reset / Cuồng Mạch |
| Story | Q1–Q12 có setup / objectives / turn-in; thiếu level không auto-chain; READY_TO_TURN_IN không auto trả; Q9 không chặn Q10; Q11 complete mới mở vùng; Q12 per character / Main Story Complete; vòng chơi tiếp tục |
| RPG / art | Food / Potion / Death; túi / kho / shop; 6 ô / 18 dòng / 21 mẫu thường / phẩm chất / giới hạn I+4, II+6, III+8 / chuyển giao cùng bậc hoặc lên bậc kế; modular 64 × 64 / PPU 32 / 26 frames |
| Online | N-player collections; P0 acceptance tối thiểu 2 concurrent players qua LAN; combat / MapExit/SpecialGate / chat / loot MapId validation; co-op; PvP cược 1v1, escrow trước trận, timeout 120 s hòa; Boss contribution 10% và shared pile 90 s |
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
