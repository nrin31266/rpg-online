# Huyền Lộ — Combat & Character

Các số BASELINE/TUNABLE chưa phải nghiệm thu runtime.

**CURRENT:** Select và Execute riêng, mỗi Execute một intent; sau nhập phái không class Normal miễn MP. [Items & Economy](items-and-economy.md) sở hữu Food/Potion; [World & Content](world-and-content.md) sở hữu mob/Boss, địa hình và nước.

## Document owns

Chỉ số nhân vật, tiến trình class, skill/passive, input/focus/pending, damage/status và combat PvP.

## Document does not own

Luật ngoài domain thuộc owner trong [documentation map](../README.md); evidence thuộc [Playtest & Balance](../04-production/playtest-and-balance.md), thứ tự triển khai thuộc [Roadmap](../04-production/roadmap.md).

<a id="character-power"></a>
<a id="gdd-3"></a>

<a id="2-sức-mạnh-nhân-vật-và-tiến-trình"></a>

## Sức mạnh nhân vật và tiến trình

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

Mob EXP theo các mục liên quan. Sau onboarding farm là nguồn EXP / Vàng / đá / gear / material chính; quest dẫn đường / dạy mechanic / kể chuyện / mở nội dung. Reward / catch-up theo các mục liên quan; QUEST-03 chỉ retune sau journey test. Giữ NeedEXP hiện tại trước khi đo HP / respawn mới. Lv 20 cap thật: CurrentEXP = 0, không tích hidden EXP / Lv 21; farm reward non-EXP vẫn hợp lệ. Demo trình diễn bằng profiles; `DebugExpMultiplier = 10` chỉ dev toggle, mặc định 1 ở release / demo.

## Công thức chỉ số nhân vật — BASELINE / TUNABLE

`Nền theo level + Thuộc tính + Trang bị → stat trước nội tại → stat cuối`; không hệ số class ẩn. **HP nền Tân Lữ/Kiếm = 120 + 10 × (L−1); MP nền mọi phái = 60 + 4 × (L−1).** Với Cung, ghi `ClassChosenLevel = C` tại giao dịch nhập phái: **HP nền = 120 + 10 × (C−1) + 8 × (L−C)**, L ≥ C ≥ 5. Đây là BASELINE/TUNABLE, không khóa hệ số 8. Cung tăng HP chậm hơn sau nhập phái, không mất HP nền ngay lúc chọn phái, kể cả nhập phái muộn; VIT vẫn +8 HP/điểm cho cả hai phái.

Khi C=5, HP nền Cung tại Lv 5/10/17/20 là 160/200/256/280. Công thức này hiển thị công khai trong bảng chỉ số, không phải hệ số ẩn. VIT/INT và gear tiếp tục tăng MaxHP/MaxMP. Rarity/enhance tính ở các mục liên quan trước khi cộng; nội tại nền tảng Lv 5 nhân stat sau tổng này đúng một lần (các mục liên quan). Giữ fractional values, UI mới làm tròn.

| Stat | Nền | Mỗi điểm thuộc tính |
| --- | --- | --- |
| HP | Tân Lữ/Kiếm: +10 mỗi cấp; Cung: +8 mỗi cấp từ `ClassChosenLevel` theo công thức trên (BASELINE/TUNABLE) | VIT +8 cho mọi phái |
| MP | 60 + 4 × (L−1) | INT +5 |
| ATK | 12 + 1,2 × (L−1) | STR +0,70 |
| DEF | 5 + 0,6 × (L−1) | VIT +0,10 |
| ACC | 60 + 4 × (L−1) | AGI +6 |
| EVA | 20 + 2 × (L−1) | AGI +6 |
| SkillDamageBonus | 1 | INT +0,35%, chỉ direct skill, không basic Tân Lữ / DoT |
| MoveSpeedMultiplier | 1 | AGI +0,05%; cộng thêm tốc chạy cố định từ Giày (các mục liên quan) |

Bốn thuộc tính hiển thị: **Công Lực (STR), Sinh Lực (VIT), Linh Lực (INT), Thân Pháp (AGI)**. STR tăng sát thương trực tiếp; VIT tăng khả năng sống sót; INT tăng MP và sát thương kỹ năng; AGI tăng chính xác/né tránh, kèm một phần tốc chạy. CritChance nền **5% cho mọi class/Tân Lữ + gear**, CritMultiplier 1,5; nội tại không cộng Crit ngầm.

`EvadeChance = 0.02 + 0.43*EVADefender/(EVADefender+2.5*ACCAttacker)`, tiệm cận 45%. All-in không bị khóa progression, không cam kết DPS ngang nhau hoặc đứng chịu ba quái. Chưa cộng điểm tại Lv 5 vẫn làm được Q5 bằng Mộc Kiếm; sau chọn class học S1; không còn class Normal Attack. Các mốc: nội tại nền tảng ở 5 → tiến cảnh ở 10 → nội tại tinh thông ở 13 → đại chiêu ở 17. Bảng gear và ATK quái đã đối chiếu lại với HP/MP theo cấp; playable test vẫn quyết định balance.

**Khi MaxHP/MaxMP thay đổi** do gear/cộng hoặc tẩy điểm: giữ HP/MP hiện có rồi clamp không vượt Max mới; không tự hồi theo tỷ lệ, không revive qua equip/reset. Hồi đầy vẫn qua nghỉ/hồi sinh đã quy định.

Tẩy Mạch Phù mua tại Mộc An theo [giá/stock Items owner](items-and-economy.md#gear-economy); hoàn điểm về unspent, giữ class / level / gear / quest / learned skills. Reset Lv 5 một lần miễn phí. Evidence profiles và sustain ở [Playtest & Balance](../04-production/playtest-and-balance.md#character-evidence).

---

<a id="class-combat"></a>
<a id="gdd-4"></a>
<a id="combat-status"></a>

<a id="3-phái-kỹ-năng-và-chiến-đấu"></a>

## Phái, kỹ năng và chiến đấu

| Giai đoạn | Combat action | Identity |
| --- | --- | --- |
| Tân Lữ, chưa chuyển class | Basic Mộc Kiếm 1,00 × / CD 0,70 s / cận chiến 1,2 u; MP 0 | Onboarding Q3–Q5; giữ được ở Lv 5+ trước Q6 |
| Kiếm, sau chuyển class | S1 single → thêm S2 primary + lan → thêm S3 primary + lan/falloff; ba slot tích lũy | Áp sát, Bỏng; không Normal Attack thứ tư |
| Cung, sau chuyển class | S1 single → thêm S2 spread → thêm S3 primary/explosion | Tầm xa, Băng Hàn; không Normal Attack thứ tư |

**Nhập phái:** Phong Du hướng dẫn Kiếm, Diệp Lam hướng dẫn Cung tại Học Viện; không gộp thành NPC chọn cả hai. Trước khi xác nhận Q6, ô Vũ khí phải trống: người chơi tháo Mộc Kiếm vào túi, không auto-remove/consume hoặc tự thay bằng weapon thưởng. Kiểm alive/idle, đúng step/NPC/range, weapon slot và capacity trước commit class + grant; reject giữ nguyên class/đồ/receipt. Full bag khi tháo thì giải phóng ô/cất đồ rồi retry.

Class transition bỏ quyền dùng basic Tân Lữ, giữ asset Mộc Kiếm cho onboarding. Chưa học S1 sau chọn class thì học sách/equip theo Q6, không cấp fallback miễn MP. S1 mặc định được chọn sau learn; mở S2/S3 không tự cast hoặc đổi selection. Nhịp và MP mới dưới đây là **PROBE BASELINE / TUNABLE**; chưa có bằng chứng chơi thật. Hướng thiết kế là S1 nhanh, S2 cũng nhanh và có thể dùng thường xuyên để farm, S3 là đòn đặc trưng/burst.

Chọn S2 rồi bấm Execute nhiều lần là cách chơi hợp lệ; không mặc định S1 là Attack duy nhất. [Playtest & Balance](../04-production/playtest-and-balance.md#current-balance-probe) giữ giả định, phép tính và mô phỏng mới.

Hỏa của Kiếm và Băng của Cung là phong vị kỹ năng qua VFX / Bỏng / Băng Hàn; không có hệ khắc nguyên tố hay bảng kháng riêng.

**Ba active tích lũy + hai nội tại/class.** Mỗi active có SkillId và cooldown riêng; S2 không thay S1, S3 không xóa S1/S2. Sáu class SkillIds cho hai class, sáu manual definitions, bốn passive IDs; basic Tân Lữ là definition onboarding riêng. Active cần level **và** learned manual flag; không Skill Rank/điểm skill. Nội tại derive đúng class + level, kể cả chọn class muộn; tên/icon/tooltip/khóa-mở, không manual/hotkey/persist thêm cờ.

| Class / mốc | Nội tại và tooltip — BASELINE / TUNABLE | Cách thể hiện |
| --- | --- | --- |
| Kiếm / Lv 5 | **Kiếm Tâm:** sau cộng nền, điểm và trang bị, MaxHP ×1,10 và DEF ×1,08. | Icon khiên/kiếm; tooltip nêu rõ +10% Máu tối đa, +8% Phòng thủ |
| Kiếm / Lv 13 | **Kiếm Thế:** kỹ năng Kiếm gây **+12% sát thương trực tiếp** lên mục tiêu có tâm vùng trúng đòn cách vị trí ra đòn đã chụp ≤1,2 u. | Icon kiếm áp sát; từng mục tiêu xét riêng, không tăng đánh thường/Bỏng |
| Cung / Lv 5 | **Ưng Nhãn:** sau cộng nền, điểm và trang bị, MaxMP ×1,10 và ACC ×1,08. **Không tăng tầm bắn.** | Icon mắt/tên; tooltip nêu rõ +10% Linh lực tối đa, +8% Chính xác |
| Cung / Lv 13 | **Xạ Tâm:** kỹ năng Cung gây **+12% sát thương trực tiếp** lên mục tiêu có tâm vùng trúng đòn cách điểm phóng đã chụp ≥4 u tại lúc trúng. | Icon tên xa; từng mục tiêu nổ xét riêng, không tăng đánh thường |

Nội tại hiện tên, icon, tooltip, trạng thái khóa/mở trong bảng kỹ năng; tự mở theo class + level, không sách/phím/rank/điểm. Kiếm Tâm/Ưng Nhãn là bonus stat **hiển thị**, không class multiplier ẩn; tính lại sau equip/reset/level rồi clamp HP/MP hiện có theo các mục liên quan. Kiếm Thế/Xạ Tâm mirror cự ly gần/xa; chỉ áp trên hit trực tiếp của kỹ năng, một lần mỗi target, không thêm proc/combo/resource. Bốn icon tái dùng motif của phái.

**Chiến Ý / Ưng Nhãn Cường Hóa** vẫn là buff đề xuất P1, chưa khóa phím, xem [Game Design — candidates](game-design.md#research-ideas).

## Bí kíp — nhận và học

| Milestone | Kiếm / Cung | Cách nhận và học |
| --- | --- | --- |
| Q6 / Lv 5 | Phong Trảm Kiếm Phổ / Linh Tiễn Cung Pháp | Staged grant sau class choice, trước cast objective; use unlock active nhập môn |
| Q8 reward | Phong Trảm Tiến Cảnh / Linh Tiễn Tiến Cảnh | Completion reward Lv 8; use cần Lv 10 + đã học nhập môn, unlock S2 độc lập, giữ S1 |
| Q11 reward / Lv 17 | Kiếm Khí Chân Quyết / Hàn Tiễn Chân Quyết | Completion reward + Rare weapon; use cần Lv 17 + nhập môn, unlock đại chiêu; không bắt buộc đã học tiến cảnh |

Sáu manual IDs, guaranteed one-time / class-specific, stack 1 / quest-bound, không sell / drop / trade; có thể cất rương cùng character, learn từ bag; consume-on-learn. Chỉ học khi alive / idle, không pending cast; kiểm class / level / prerequisite, rồi atomically consume book + set learned flag theo SkillId; Replay / duplicate learned reject không consume. **Bí kíp không có cooldown**; chống học lặp bằng learned flag và grant / learn receipt.

Cooldown dưới đây thuộc kỹ năng được mở. Cooldown riêng mỗi SkillId; học skill mới/chuyển slot không reset deadline skill cũ; action đang chạy giữ snapshot. Chưa đủ Lv 10 giữ book, tooltip “Cần Lv 10”, không mất sách; không random book farm. Full bag theo reward preflight; Q6 staged pending không lock quest vĩnh viễn. Nội tại Lv 5/Lv 13 tự học theo bảng trên, không manual. NPC không thêm menu học riêng: dùng trong bag. HUD locked nói rõ level / manual / quest;

Lv 10 có book chưa học vẫn nhập môn, tiến cảnh feedback ngắn khi use. Manual là power item thật; collection chính tuyến cũng là item Inventory thật theo [Quest owner](quests-and-narrative.md#quest-collection).

## Bộ kỹ năng — BASELINE / TUNABLE

| Class / profile | Lv / manual | Primary / propagation | Power | Max | MP / CD | Status |
| --- | --- | --- | --- | ---: | --- | --- |
| Phong Trảm nhập môn | 5 / Q6 | Cận chiến, một mục tiêu, 1,7 u | 1,20 × | 1 | 2 / 0,60 s | Không |
| Phong Trảm tiến cảnh | 10 / Q8 | Primary cận chiến 1,7 u + secondary gần primary; bounds ở contract dưới | 1,35 × / mục tiêu | 3 | 3 / 0,90 s | Bỏng 4% |
| Kiếm Khí | 17 / Q11 | Primary cast range 5,5 u + secondary gần primary; primary-first falloff | 2,80 / 2,60 / 2,40 / 2,20 / 2,00 × | 5 | 16 / 6 s | Bỏng 70% |
| Linh Tiễn nhập môn | 5 / Q6 | Một mục tiêu logic, 6,5 u; một tên hình ảnh | 1,15 × | 1 | 2 / 0,60 s | Không |
| Linh Tiễn tiến cảnh | 10 / Q8 | Spread logic 6,5 u, ba hit | 0,70 / 0,60 / 0,50 × / tên | 3 hit indices; tối đa 3 target, cho phép trùng target | 3 / 0,90 s | Băng Hàn: 2% Normal / 1% Linh; 2% Boss / PvP |
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

**LOCKED cấu trúc input:** `1 / 2 / 3` chỉ chọn S1/S2/S3 đã mở; không cast, không tiếp cận, không tiêu MP, không bắt đầu CD. Tân Lữ chọn basic Mộc Kiếm ở slot 1, slot 2/3 khóa. `ExecuteSelected` là action riêng; phím vật lý còn OPEN. Ví dụ `2 → Execute → Execute → Execute` là ba lần chủ động dùng S2. Giữ Execute không RepeatOnHold; mỗi lần bấm vật lý tạo tối đa một execution intent. Chọn slot đã khóa không đổi selection hay action đã nhận.

Chọn slot hợp lệ khác không sửa SkillId trong pending/buffer/action đang chạy; phải bấm Execute mới để thay intent chờ.

Execute chụp `SkillId` đang chọn và identity/life/MapId của target. Nếu không có primary hợp lệ thì không action, MP, CD hay movement. Kiểm primary từ vị trí tiếp cận dự kiến và origin thực trước commit; không thay focus bằng một victim tình cờ nằm dưới nét chém. Secondary không cứu một cast có primary invalid. RunningAction giữ snapshot riêng, không đọc selection thay đổi sau đó.

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
| Đến tầm | Kiểm target còn sống/đúng đời/cùng MapId và đáp ứng policy Return đang thử; player sống/không CC; skill đã học, vũ khí hợp lệ, đủ MP, CD sẵn, hết lock và primary range/eligibility ở vị trí thực. Fail trả reason, không cost. |
| Bắt đầu cast hợp lệ | Chụp stat nguồn/vị trí ra đòn/facing và commit MP/CD đúng một lần. Approach không miễn sát thương hoặc bảo đảm hit. Đổi slot không reset CD. |

<a id="escape-priority"></a>

**Esc, mỗi lần chỉ xử lý một tầng:** đóng/lùi modal hoặc chat → nếu đang pending/buffer thì hủy → nếu có focus thì clear → nếu không có gì thì no-op. Không rơi tiếp xuống thao tác gameplay trong cùng lần bấm.

Trọng lực và quán tính ngang tiếp tục khi Tân Lữ/S1 ra đòn trên không; quyền dùng S2/S3 trên không còn OPEN, phải thử trước khi chốt. Sát thương thường chỉ đổi HP và phản hồi bằng flash/chữ/impact, không tạo trạng thái Hurt, giật đòn, đẩy lùi hay ngắt action. Thứ tự ưu tiên là trạng thái kết thúc/tử vong → khống chế cứng đúng loại mục tiêu → action chưa giải quyết. P0 không cho nhảy/dash để hủy đoạn hồi động tác.

Ứng viên nhặt đồ/tương tác tách khỏi CombatFocus: làm nổi món ở gần, đủ quyền và tầm; `Interact` nhặt/tương tác ngay, không cần bấm lần đầu để chọn rồi lần thứ hai mới dùng. Nhặt xong chuyển sang ứng viên gần tiếp theo; rời tầm thì xóa hoặc đổi ứng viên. Quyền nhận đồ tutorial riêng và quyền nhặt đồ chung vẫn theo các mục liên quan; phím combat không tự nhặt, dùng bình hay chạy tuyến.

**Ba hit Cung:** snapshot A/B/C, A/B/A hoặc A/A/A tại start; resolve cả ba cùng clock +0,12 s, giữ power theo index. Invalid target làm mất index đó, không chuyển target/chia power. Mỗi logical hit có Evade/Crit roll, status tối đa một application/unique landed target/cast kể cả proc fail cache. Hàn primary hợp lệ tại resolve tạo tâm nổ ở primary position kể cả primary Evade; invalid primary không nổ; secondary roll riêng, primary không nhận explosion lần hai.

Primary/secondary ordering và eligibility theo contract propagation dưới đây; hình chém không chọn victim.

<a id="target-propagation"></a>

## Target-based combat: acquisition, propagation và presentation

**LOCKED direction theo chỉ đạo recovery hiện tại:** player chọn primary, Game Server validate và giải quyết kết quả theo clock; skill có thể lan tới secondary gần primary. Secondary sau primary hoặc khác cao độ được xét nếu đáp ứng bounds; không cần nằm dưới nét chém. Không collider vũ khí/VFX, PlatformID hoặc SpawnGroup gate damage. Ba việc tách nhau: **A acquisition** giữ CombatFocus/PendingCast; **B propagation** chọn actual targets bằng policy của skill; **C presentation** vẽ pose/main VFX/impact/status theo kết quả. Target-based không đồng nghĩa chỉ single-target.

Primary phải sống, attackable, đúng ID/life/generation và cùng action MapId ở start. Tại resolve, kiểm lại identity, range từ immutable action origin và eligibility; chuyển động caster không kéo dài range. Auto-face hướng primary tại accepted start là **engineering recommendation BASELINE**, không focus-only auto-face, không xoay action giữa release. Primary front/vertical và occlusion policy chính xác chưa có approval lịch sử riêng: thử profiles dưới; không ép player canh collider của nét chém. Author combat origin và hurtbox center riêng khỏi sprite bounds; originY+0,8 u và passive distances đang dùng giữ nguyên để đối chiếu, không scale range theo Linh/Boss/VFX.

**Kiếm S2/S3 — engineering recommendation BASELINE cho executor:** pin primary identity lúc start; tại HitMoment primary còn eligible thì chụp tập secondary một lần quanh **vị trí primary tại resolve**, loại primary, dùng target IDs/lives duy nhất, sameMap/alive/attackable/bounds/occlusion. Không lọc loại mob, group hoặc tầng. Primary index0; secondary sort theo khoảng cách bình phương tới primary rồi stable instanceId/generation. S3 power giảm theo indices 0..4 trong bảng skill; primary Evade vẫn giữ index0, secondary roll riêng và không dồn power lên index khác. S2 mọi target dùng cùng power. Resolve set trước khi apply damage, không refill cap sau một hit giết target.

Primary chết/despawn/đổi life/map hoặc ra khỏi eligibility trước HitMoment: S1 mất hit; Kiếm S2/S3 không lan; Hàn không nổ. Không retarget, không refund MP/CD đã commit. Primary **Evade vẫn là target hợp lệ**, Kiếm S2/S3 vẫn lan, Hàn vẫn nổ; status chỉ roll ở landed targets. Secondary invalid không nhận hit/status, không replacement giữa lượt resolve. Đây là lựa chọn executor đề nghị mới cho Kiếm, không claim Git chứng minh đã duyệt trước đây.

**Cung S2 giữ batch policy riêng:** start snapshot A=primary; chọn tối đa hai distinct eligible targets trong execution envelope của Cung, recommendation ưu tiên gần A rồi stable ID. Có ≥3 targets → ABC; hai → ABA; một → AAA. Ba indices giữ power và cùng logical +0,12 s. Mỗi index revalidate identity/life/MapId/range tại resolve; invalid chỉ mất index đó, không reacquire/redivide power. A chết không tự hủy B/C hợp lệ; các indices tham chiếu A bị bỏ. Status cache cả fail trên unique landed target, không reroll A/A/A. Không ép Spread dùng Sword splash bounds hoặc một fan collider.

**Hàn Tiễn giữ primary/explosion:** primary valid tại +0,18 s xác định tâm nổ ở primary position, kể cả primary Evade; primary không nhận secondary hit lần hai. Query radius **2 u BASELINE** theo logical centers tại resolve, tối đa bốn distinct secondary; recommendation distance tới primary rồi stable ID. Không snapshot secondary trước tâm nổ; mỗi target có Evade/Crit/status riêng. Target chết sau một kết quả đã resolve không làm kết quả ấy biến mất.

<a id="propagation-probes"></a>

### Bounds và policy còn OPEN/PROBE — có cấu hình thử cụ thể

Các range primary trong bảng skill **giữ nguyên**. Bảng này chỉ là cấu hình thử cần log revision; chưa số gameplay đã LOCKED. Không dựng AoE toàn map hoặc nhập nhằng primary range với splash extent. Chưa có bằng chứng runtime để chọn mức dọc/rộng cuối; canonical cho phép coding micro-slice với profiles test, chưa cho nghiệm thu balance.

| Policy | Historical basis | Recommended probe | Acceptance để chọn |
| --- | --- | --- | --- |
| Primary range/vertical | Current ranges; NSO research đề nghị melee Y1 u, source/prototype khác nhau | Logical distance từ action combat origin tới authored hurtbox center; melee vertical1 u thử trước; ranged/SwordS3 vertical1,3 so 3 u. Tất cả vẫn phải trong range primary hiện hành | Biên range/vertical, cùng vị trí đổi sprite size, target trên tầng; không hidden target-size buff. Nếu closest hurtbox-point tốt hơn center, trình riêng thay vì silently đổi metric |
| Sword secondary proximity | Research đề nghị X3–3,5/Y2,2 u, chưa approval Huyền Lộ | **C1 khuyến nghị khởi điểm bảo thủ:** ellipse quanh primary, S2 halfX1,7/halfY1 u; S3 halfX2/halfY1 u. **C2 đối chiếu farm đa tầng:** halfX3/halfY2,2 u cho cả hai. `dx²/X²+dy²/Y²≤1`, cộng caster-distance bound `≤ primaryRange + max(X,Y)`; không cone/line | Sau primary, cao độ ngay trong/ngoài ellipse và tổng envelope, nhóm khác; measure target distribution/TTK/pressure. C1 cho lệch tầng gần, C2 cho terrace cao hơn nhưng tăng reach; không đổi power để che chênh DPS |
| Behind-player secondary | Không tìm approval riêng; research gần primary không front-filter | Recommendation không front-filter secondary trong C1/C2; primary auto-face start. Query bị giới hạn proximity + caster envelope | Đặt secondary trước/sau player sát biên; đo readability/aggro. Quyết policy cuối sau probe, không giả đã khóa 360° |
| SolidWall/one-way | Research B là lock candidate; GDD sau vẫn A/B PROTOTYPE | Recommendation B: SolidWall cản origin→primary và primary→secondary; Spread kiểm origin→từng batch target. A bỏ LoS là control. One-way mỏng không chặn B; full geometry LoS DROP P0 | Cùng centers, cùng skill qua tường dày/sàn mỏng/hốc; record mode. Không tự dùng SurfaceId để thay ray/query |
| Ordering/ties | Old Line caster intersections gần→xa; không primary-first approval | Primary index0; distance-to-primary/ID cho Kiếm/Hàn; Bow snapshot order giữ immutable. Metric/ties deterministic trong session | Focus giữa cụm, targets đổi vị trí trước resolve, ties/invalid/Evade; peer clients thấy cùng powers/indices |

Bounds là logical eligibility, không collider damage chạy theo animation. Melee **của mob** và telegraph đất **của Boss** vẫn kiểm front/vùng nhận đòn/vị trí tại HitMoment để player né; correction player target propagation không biến các đòn đó thành guaranteed hit. Ranged mob visual-only giữ nguyên. Return targetability và S2/S3 air permissions còn OPEN tại owners, không mặc định thêm immune hoặc movement lock.

## Sát thương, nhịp đòn và trạng thái

Chụp ATK, bonus kỹ năng từ INT, ACC, Crit và nội tại của nguồn tại lúc bắt đầu cast; xét DEF/EVA và vị trí/vùng nhận đòn của mục tiêu tại lúc trúng. Đổi đồ hoặc lên cấp giữa action không sửa sát thương của action đã phát.

Basic Tân Lữ raw = FinalATK × power; skill raw = FinalATK × power × SkillDamageBonus và bonus nội tại có điều kiện. Trong match PvP, nhân raw theo các mục liên quan trước DEF/Crit/random; PvE không dùng hệ số đó. Game Server validate → Evade → Crit → DEF / random → HP: `Damage=max(1,round(Raw*100/(100+TargetDEF)*Random(0.95,1.05)*CritMultiplier))`; miss 0 / NÉ. `round(x)=floor(x+0.5)` cho x ≥ 0. ActualHpLost = min(calculatedDamage, remainingHP) vào threat / contribution, không overkill.

Một action lock chung; CD / MP commit tại cast start, basic Tân Lữ cũng có CD riêng. Các mốc tính từ cast start — BASELINE / TUNABLE:

| Action | Logical resolve | Action lock |
| --- | --- | --- |
| Basic Tân Lữ | +0,10 s | 0,32 s |
| Nhập môn single (hai class) | +0,12 s | 0,30 s |
| Phong Trảm tiến cảnh | +0,14 s | 0,30 s |
| Linh Tiễn tiến cảnh | Ba logical hits cùng +0,12 s | 0,34 s |
| Kiếm Khí | +0,16 s | 0,40 s |
| Hàn Tiễn | +0,18 s | 0,40 s |

Chỉ số/nội tại nguồn và điểm ra đòn được chụp lúc bắt đầu; DEF/EVA/vị trí mục tiêu kiểm tại resolve. Người ra đòn di chuyển sau đó không dời gốc hoặc kéo dài tầm. Chết, chuyển map hoặc khống chế cứng đúng loại hủy action chưa resolve, không hoàn chi phí; kết quả đã resolve không bị hình ảnh sửa. AnimationEvent chỉ trình diễn, không lên lịch sát thương. ART-01 kiểm visual timing; các mô hình rotation cũ cần chạy lại theo [Combat & Character](#design-lock-rationale).

**Status application:** Bỏng và Băng Hàn mỗi loại có tối đa một roll theo `(actionId,actualTargetId,effectId)` tại landed hit đầu tiên; cache cả fail, không reroll trên A/A/A. **Băng Hàn là một application**, chọn kết quả theo loại target trước roll; không roll Làm Chậm và Đóng Băng độc lập.

| Effect / target | Lifecycle — BASELINE / TUNABLE |
| --- | --- |
| Bỏng / quái thường, Linh Biến, Boss | 6 s, tick mỗi 1 s; raw/tick = 0,06 × ATK nguồn đã chụp, qua DEF hiện tại; không Crit / random / INT. Một Bỏng/target. Proc lại thay source/ATK snapshot, refresh **về đủ 6 s từ lúc proc**, giữ nhịp tick hiện có; không cộng dồn sát thương hoặc kéo duration bằng phép cộng. PvP immune. |
| Băng Hàn / quái thường | Chỉ **Đóng Băng 1,5 s**: Linh Tiễn 2%, Hàn Tiễn 45%. Không Làm Chậm. |
| Băng Hàn / Linh Biến | Chỉ **Đóng Băng 1,5 s**: Linh Tiễn 1%, Hàn Tiễn 30%. Khác quái thường ở chance, không ở duration. |
| Băng Hàn / Boss | Chỉ **Làm Chậm 3 s**: tốc chạy khi đổi vị trí ×0,85; **đồng hồ chờ action kế tiếp chạy ở 75% tốc độ**. Linh Tiễn 2%, Hàn Tiễn 100%. Không Đóng Băng. |
| Băng Hàn / người chơi PvP | Chỉ **Làm Chậm 1,5 s**: MoveSpeed ×0,75. Linh Tiễn 2%, Hàn Tiễn 100%; không ảnh hưởng tốc đánh, hồi chiêu, animation hoặc action đang cast. Không Đóng Băng. |

Đang Đóng Băng không roll/apply Băng Hàn; tan băng được miễn Đóng Băng **3 s trên cùng target**, N Cung cùng dùng một deadline, không chain-freeze. Đóng Băng khóa di chuyển/AI đánh và hủy windup/hit/spawn chưa giải quyết; visual đã phát không sinh hit sau cancel, target không nhận thêm damage. Boss/người chơi không Đóng Băng. Làm Chậm chỉ refresh deadline về `now + duration`, không stack magnitude.

Boss còn 1,5 s chờ action thì debuff **không reset về full CD**: chỉ giảm tốc đếm phần thời gian còn lại; hết debuff đếm lại 100%. Action/telegraph/projectile **đã bắt đầu** giữ nguyên mốc và tốc độ. Cuồng Mạch đổi future base cadence, Băng Hàn chỉ tác động đồng hồ chờ sau khi đã chọn cadence đó; nhiều Cung không nhân nhiều lớp slow. Một target chỉ dùng đúng effect của category.

Nhiều Kiếm cùng đánh chỉ refresh một Bỏng trên target: tick đầu không bị đẩy lùi mãi, source/ATK snapshot/expiry thay khi proc, tick đúng expiry trước remove. Đòn trực tiếp của Kiếm Thế chỉ nhận +12% nếu đúng khoảng cách từ action origin; Xạ Tâm dùng immutable action origin và actual target position tại logical resolve. Hai nội tại tinh thông không đổi status chance/magnitude. DoT credit source thực; source chết không xóa Bỏng đã áp. Eligibility tại death theo các mục liên quan; không active Buff P0.

Player–Monster không gây sát thương khi chạm hoặc chặn thân; Monster–Monster không xô đẩy. Chỉ dùng điều chỉnh tách nhẹ để hình dễ đọc khi cần, không đội hình runtime hay thêm stun/đẩy lùi. Gốc đòn cận chiến ở originY + 0,8 u; vùng trúng theo chiều dọc kiểm tại PHY-01.

---



HP ≤ 0: khóa movement, attack, skill, pickup; camera / death anchor tại MapId, không auto-respawn. [Player pop/fall → shadow](../03-art/art-and-visual-production.md#player-shadow-death) chỉ là presentation theo authority, không knockback gameplay; mob death giữ nguyên. Về Vân Khê miễn phí full HP / MP hoặc tiêu phù hồi tại chỗ. Consequence phù / run-back; không EXP debt / mất gear / Vàng. Relog không được hồi sinh miễn phí; checkpoint/recovery thuộc [Online & Persistence](../02-technical/online-and-persistence.md#profile-authority).

Corpse eligibility/loot thuộc [Items & Economy](items-and-economy.md#farm-rewards); khôi phục mất phiên, Food/status/CD và SafeAnchor thuộc [Online & Persistence](../02-technical/online-and-persistence.md#profile-authority). Death không tự hủy focus còn hợp lệ.

> **Đọc sâu:** [Playtest & Balance — Food / MP sustain](../04-production/playtest-and-balance.md#combat-analysis)


**Trong trận:** Bắt đầu Active sau khi đếm ngược hoàn tất; đồng hồ **120 s** tính từ đó. Vào Lôi Đài đầy HP/MP; Food đang có hiệu lực **tiếp tục tick và hết hạn bình thường**, không tạm dừng. HP Potion và MP Potion dùng được theo đúng cooldown riêng hiện hành và phải có bình thật trong túi; mỗi người tối đa **3 lần HP + 3 lần MP / MatchId**. Dùng quá quota hoặc không đủ điều kiện bị từ chối **không tiêu item**. Mỗi lần hợp lệ tiêu đúng một bình. **Cấm Hồi Sinh Phù**. `PvPDamageMultiplier = 0,20` BASELINE/TUNABLE áp lên raw trước DEF/Crit/random, không tác động PvE; Bỏng miễn nhiễm, Băng Hàn chỉ Làm Chậm. HP = 0 là PvPDefeated, không phải chết PvE; không mất gear/EXP ngoài tiền cược đã tạm giữ.

<a id="ux-art"></a>
<a id="gdd-10"></a>
<a id="gdd-11"></a>

<a id="9-trải-nghiệm-điều-khiển-và-hình-ảnh"></a>

## Trải nghiệm, điều khiển và hình ảnh

Luồng màn hình P0: Boot/Main Menu → Login tài khoản được cấp → chọn nhân vật → overlay kết nối → map/điểm khôi phục hợp lệ. Nhân vật mới bắt đầu ở Vân Khê. Không có Register cho player; không cần Loading Scene riêng.

**LOCKED:** Move dùng ←/→, Jump dùng ↑, DropThrough dùng ↓ trên one-way đang đứng. Letter keys được giải phóng khỏi movement; A/D/Space/S không còn primary gameplay bindings. Một action Move không cộng đôi tốc độ. Jump và DropThrough cùng frame trên one-way thì Drop ưu tiên; trên solid Drop no-op, Jump vẫn hợp lệ. Text input/modal giữ input, không để movement/combat lọt qua. Toàn bộ vòng chơi/menu P0 dùng được bằng bàn phím, chuột là cách bổ sung.

| Binding / trạng thái | Semantic action | Hành vi |
| --- | --- | --- |
| ← / → — LOCKED | Move | Đi ngang thủ công |
| ↑ — LOCKED | Jump | Nhảy theo movement state |
| ↓ — LOCKED | DropThrough | Một lần xuyên one-way hợp lệ đang đứng |
| 1 / 2 / 3 — LOCKED | SelectSkillSlot1 / 2 / 3 | Chỉ chọn kỹ năng; không action/MP/CD/approach |
| Phím còn OPEN | ExecuteSelected | Thực thi kỹ năng đã chọn theo các mục liên quan |
| Tab / Shift+Tab | CycleTarget | World đổi target cục bộ; trong modal thuộc UI navigation |
| Phím còn OPEN | QuickHP / QuickMP / Food / Interact | Gọi validator dùng đồ/tương tác, độc lập CombatFocus |
| Navigate / Confirm / Back | UI actions | Điều hướng, xác nhận và lùi/đóng theo context |
| Enter / Esc | Chat / Back hoặc clear theo context | Chat theo các mục liên quan; Esc chỉ xử lý một tầng theo các mục liên quan |

**PROPOSAL / TUNABLE DEFAULT để thử usability, không LOCK:** E→ExecuteSelected, F→Interact, 4/5→QuickHP/QuickMP, R→Food, I→RPG menu. C/Q và các letter khác chưa được gán mechanic mới. Không thêm full remapping P0. Nếu dùng bảng này trong probe, HUD/tutorial phải lấy glyph từ binding thực, không hard-code phím vào quest data.


**Menu bằng bàn phím:** Interact mở NPC với action phù hợp được chọn sẵn (nhận/trả quest trước, rồi service). Trong modal: Navigate chọn ô/action, Tab/Shift+Tab đổi lựa chọn/view; Confirm hoặc Interact xác nhận; Esc đóng. Move/Jump/DropThrough/SelectSkillSlot/ExecuteSelected không lọt thành gameplay khi UI giữ focus. Enter chỉ mở/submit chat khi không có modal khác; Tab ở modal là UI navigation; ở world dùng [CycleTarget](#focus-input). Inventory/equip/learn, shop buy/sell, character/skill tab, rương và revive đều có focus rõ, text/action disabled reason và cùng command validation cho chuột/bàn phím. Không yêu cầu click để hoàn tất quest. Đổi mục tiêu và vòng đời focus theo [các mục liên quan](#focus-input); menu không nhận world CycleTarget.

**Gate cảm giác di chuyển:** thử khoảng cho nhảy ngay sau khi rời mép (coyote time), nhớ input nhảy ngay trước khi chạm đất (jump buffer), normal Jump độ cao cố định cho mỗi accepted press, không đổi apex bằng hold/release, tăng/giảm tốc trên đất và tốc rơi. Các số là PROTOTYPE, chưa khóa trước khi có collider/tỷ lệ/map/mẫu art. Không thêm double-jump/dash. Drop không hưởng coyote để nhảy bật ngược lên sàn; authority và client phải dùng cùng semantics. Xem contract/probe Technical.

Target HUD tối giản: marker + mini HP trong world; tên/level/current-max HP/bar trên màn hình, bind đúng focus ID/generation/MapId. Player chết vẫn quan sát HP target hợp lệ, kể cả HP đổi do người khác đánh; death không tự ẩn/xóa focus. Không thêm portrait/element/rarity/generic buff panel.

<a id="design-lock-rationale"></a>

## Combat rationale, delta và gates

Luật progression/input hiện hành đọc [Combat & Character](#focus-input); NSO research chỉ evidence. Basic Tân Lữ trước class tránh hard cutoff làm Q5 Lv 5 kẹt. Ba active tích lũy giữ lựa chọn skill/MP/CD riêng, thay vì thêm class Normal miễn MP; interaction riêng giúp thao tác loot không đổi combat target. Logical ranged result bỏ flight damage/interception; player primary/propagation kiểm eligibility, còn mob melee/Boss telegraph giữ positional dodge. Movement/momentum không bị normal damage interrupt.

| Review decision | Kết luận / trade-off | Gate còn phải đo |
| --- | --- | --- |
| D01/D11/D12/D19 | Logical primary/propagation + visual-only projectile; source snapshot/target resolve, dedup hit/status | C0 eligibility/lifecycle/visual timing rồi G-L/G-N; rerun simulation |
| D02–D05/D09/D22/D28 | AUTO acquire-retain-reacquire/EXPLICIT pinned; search/retention khác execution range; Select chỉ chọn, Execute riêng tạo one-shot pending/latest buffer; minimal target HUD | Context retention, range, pending replace/cancel/modal/spam; release không cancel pending, hold không repeat mọi skill |
| D06–D08/D10 | Damage feedback không Hurt/hit-stun/knockback; gravity/momentum liên tục; không recovery cancel P0 | Novice/S1 air/control, S2/S3 permissions còn prototype |
| D13/D16 | Lightweight logical lane/flying bounds, Ong giữ flying/ranged và band reachable | PHY-01, G-N positions/headless; không full Rigidbody/nav graph |
| D14 | Hybrid count/identity OPEN; 3/1/0 chỉ ví dụ lịch sử, không preferred/final roster; giữ các mob identity/fixedlevel | PROTOTYPE + user content approval; không tiết kiệm pose count trước duyệt |
| D15/D15R | Legit Cung kite là lợi thế; perch unreachable xử authoring+Return đơn giản, không anti-Bow AI; exact grace/Return/regen/invuln/targetability chưa chốt | Jump ngắn/perch/kite/2 players/reachable retarget; reset không reward/reroll/life mới |
| D17/D18/D20 | LoS A/B PROTOTYPE; full geometry LoS DROP P0; respawn 25 s/rates giữ TUNABLE; 28/66 là LEGACY seed, mật độ mới re-author chưa totals | Room/playtest/benchmark workload thật; không CPU%/số dòng recipe proof |
| D21/D23/D24 | Per-profile visuals; Auto/gamepad/hitstop/crit shake/material audio DEFER; Tab theo input đã duyệt; 26 logical frames giữ LOCKED, A01 mapping/A02 technique OPEN | Art imported rig/weapon/readability/hour/% usable; không 33 pose lock |
| D25–D27/D29/D30 | Giữ tính toàn vẹn backend; tổng hợp findings VS-1 rồi thử trong sandbox riêng; ba ô tích lũy/CD riêng, không fallback class miễn MP | Tổng hợp/thử → nền production → G-L mới → G-N/G-D; luân phiên CD độc lập đổi burst/tài nguyên |

**ROLE AUDIT hiện hành:** power/MP, power/CD, occupancy, TTK đơn/cụm và rủi ro Cung AAA/S1 nằm tại [probe 2026-10-06](../04-production/playtest-and-balance.md#current-balance-probe). Bảng input1–3one-press/cadence1,0/1,5/1,7/7 cũ đã SUPERSEDED; không restore class Normal để lấp MP/CD. Independent cooldown và chọn/Execute thủ công giữ quyền luân phiên skill, không cho hold-repeat hoặc auto-combat.

Research claims về CPU/GPRS/T9 motive là HISTORICAL INFERENCE; “1000 mobs <3% CPU”, deterministic/anti-cheat tuyệt đối hoặc số dòng recipe là UNSUPPORTED, không justification. Client/server splash thresholds khác nhánh, map parse chưa xác minh không khóa geometry. Ba báo cáo giữ ở [research notes](../../research/README.md), không design authority. Không triển khai navigation/DropLink/universal ranged fallback/Auto/rich UI/rollback để kế thừa NSO.


<a id="a05"></a>

## A05 — quyết định liên quan

**A05 Multi-target Cung** — Lịch sử flight/spawn lệch được thay; mất projectile dodge sau resolve · APPROVED logical Spread batch +0,12 s ABC/ABA/AAA; Hàn +0,18 s, visual không damage · P04/P12/P13; Bow AAA role/sustain và visual timing cần đo

**A05** — ART-01 / PHY-01 / BAL-01 · APPROVED logical target resolution/batch, fallback/status dedup; OPEN presentation travel/vertical bounds/Bow AAA balance. Class Normal executor gap đã superseded.


<a id="a06"></a>

## A06 — quyết định liên quan

**A06 Normal/timing/CD/proc** — Lịch sử 0,80/0,90 s class Normal không còn; action/CD khác pose duration · Dùng power/MP/CD probe mới sáu skills TUNABLE; CD độc lập/common lock, Novice basic trước class · P03/P13; rerun rotation/resource, không dùng legacy occupancy

**A06** — ART-01 / BAL-02 / GEAR-01 · PROBE BASELINE mới giữ hit/lock/proc, retune CD/MP/powerS2Cung theo design owner; exact số vẫn TUNABLE. Timing pose bám clock đã được chấp nhận, retune gameplay cần evidence.


<a id="a14"></a>

## A14 — quyết định liên quan

**Weapon carry/facing:** Idle 3/4 Left/Right và Back↔Hand thuộc [Art probe](../03-art/art-and-visual-production.md#weapon-carry), không đổi CombatFocus/PendingCast, MP/CD, input hoặc action clock; rút/cất không trì hoãn ExecuteSelected.

**A14 Action khi nhảy/chạy** — Full pose attack trên không: ít setup nhưng chân lệch; upper action + lower locomotion: reuse tốt nhưng overlap khó; pose riêng: đẹp nhưng tăng cost · Không thêm movement lock để cứu art; thử hành vi được cho phép, thêm đúng pose/track thiếu · P01/P03/P09; khóa movement/jump là gameplay proposal

**A14** — ART-01 / PHY-01 / BAL-01 · APPROVED giữ gravity/momentum khi cast; CURRENT Novice/S1. OPEN S2/S3 air permissions và pose phối upper/lower; không lock movement để cứu art.

Build không branch cap; không Skill Rank. Movement semantics giữ no ladder/climb, no playable slope và shallow-water effect theo [World terrain](world-and-content.md#terrain-rules).

<a id="movement-direction"></a>

## Movement direction đã xác nhận

**LOCKED direction:** Normal Jump dùng một accepted press và một jump profile độ cao cố định; hold/release không đổi upward velocity/gravity/apex. Coyote/buffer, launch velocity, gravity/fall gravity/terminal speed và landing feel là TUNABLE; trần/tường có thể chặn quỹ đạo, không hứa mọi jump đạt apex qua collider. Không double/charged jump/dash. [Runtime](../02-technical/gameplay-runtime.md#movement-feel) sở hữu triển khai và lifecycle.

Player có **hai logical facing Left/Right**; dừng sau movement giữ hướng vừa đi. Idle 3/4 nhẹ, Run nghiêng side hơn, Jump/Fall có silhouette side khác nhau, action gần full side với upper pose đúng Kiếm/Cung. Action facing đã snapshot không bị input mới sửa giữa release. [Art](../03-art/art-and-visual-production.md#player-facing) sở hữu mapping/raster; không thêm bốn hướng hoặc camera depth. Default Right deterministic khi chưa có saved-facing requirement là BASELINE, không thêm field DB.

DropThrough giữ cho one-way kết cấu có topology cần (ví dụ cầu/sàn Trúc); không ép Q2 dùng. **Binding ↓ đã LOCKED từ canonical input**, S chỉ alias prototype cũ, không mở lại ↓ hoặc mới khóa S. Jump và Drop là actions riêng, solid vẫn no-op Drop.


<a id="combat-decision-trace"></a>

## Decision Trace — lập trước khi sửa Combat, 2026-10-09

Git chứng minh nội dung và thời điểm ghi, không chứng minh user đã duyệt từng câu. `APPROVED` trong draft là phân loại do tài liệu ghi; không có transcript quyết định tương ứng trong Git. Chỉ đạo trực tiếp ở prompt recovery hiện tại là approval mới cho target-based propagation và quest/service corrections. Không reset về `229cc61`.

| Decision | Historical evidence | Current canonical trước sửa | Conflict | Recommendation |
| --- | --- | --- | --- | --- |
| Kiếm Arc/Line | `2080dd8`, `docs/design/HUYEN_LO_GDD.md`, bảng kỹ năng Lv5/Lv17 đã có Arc120°/1,7 và Line5,5/rộng≈0,6; `ed73c73` chuyển đường dẫn | Combat, Bộ kỹ năng giữ hình học đó với stats mới | Có từ import đầu tiên; không tìm được bản thảo trước import hoặc approval riêng | Không gọi là Codex mới tự thêm trong migration. Chỉ đạo hiện tại thay damage geometry Kiếm bằng primary + proximity; giữ stats hiện hành, không phục hồi stats import |
| Primary với geometry và falloff | `46006c4` / nhánh `63bbd22`, `docs/design/1_HUYEN_LO_GDD.md`, CombatFocus: Arc/Line chỉ cần victim, Line intersections gần→xa; `af7e23c` giữ câu này | Focus có thể không bị hit; primary chưa giữ vị trí power đầu | Target intent và geometry không có một primary bắt buộc thống nhất | Primary hợp lệ bắt buộc cho Kiếm; primary index0, secondary gần primary theo thứ tự ổn định là engineering recommendation, không recovered lock |
| Từ khóa target-based | Pickaxe `target-based`: `af7e23c`, GDD CombatFocus, thêm “target-based authoritative combat + logical geometry validation”; `229cc61` chuyển sang owner mới | Combat/Runtime kiểm hitbox–hurtbox/front/Arc/Line | Tên target-based vẫn ghép shape gate cũ | Tách acquisition, propagation và presentation. Không VFX collider hoặc PlatformID/SpawnGroup damage gate |
| AUTO/EXPLICIT/Pending | `00e9d9c`, GDD CombatFocus; `00d2369`, Technical input; `34829d5`, Analysis ghi approved input/cadence. `af7e23c` sửa select-only/Execute riêng | Select riêng, Execute one-shot; pending/buffer/dead focus | Rule hold-repeat/123-execute trong checkpoint đã superseded | Giữ current input/snapshot/lifecycle; không phục hồi hold-repeat vì nằm ở commit cũ |
| Cung batch/Hàn/status | `46006c4`, GDD Ba hit Cung/timing/status; parent của `af7e23c` và `af7e23c` đều giữ ABC/ABA/AAA, invalid index drop, primary Evade vẫn nổ, cache fail | Cùng logical resolve, no retarget/status dedup | Không có căn cứ bỏ các invariants này khi sửa Kiếm | Giữ; làm rõ batch khác primary-gated AoE, mỗi index revalidate riêng |
| Secondary proximity khác tầng | `46006c4`, `research/notes/NSO_WORLD_FARM_TARGETING_RESEARCH.md` §3B/§5; Modernization §5 đề xuất X3–3,5/Y2,2 u | Geometry quanh caster; research chưa là authority | Research phỏng NSO không chứng minh Huyền Lộ duyệt exact units | Current user xác nhận gần primary/cross-height nếu eligible; numeric bounds chỉ OPEN/PROBE. So profile hẹp và rộng, không đổi power để che phạm vi |
| NSO source bounds | Local `research/SRC NSOACE FIX/src/main/java/com/nsoz/model/Char.java` khoảng6347 dùng X±100/Y±100; khoảng7656/7861 dùng X±100/Y±50. Client decompile path trong notes nằm ngoài repo, chưa xác minh trực tiếp | Rationale đã cảnh báo khác nhánh | Không có một ngưỡng dọc duy nhất có thể copy hoặc suy PPU=32 là conversion đã duyệt | Ghi branch-specific research. Không khóa 2,2 u như recovered rule; không port client target lists/auto-train/physics cũ |
| LoS/facing/behind-player | Research Modernization §8 gọi B là LOCK CANDIDATE; `46006c4`/`af7e23c` ghi A/B prototype, không PlatformID gate | Front gate player melee; A/B OPEN | Không chứng minh wall hoặc behind-player secondary đã approved | Recommendation auto-face primary lúc start, không front-filter secondary; compare A/B với SolidWall only, one-way bỏ qua. Final policies phải có probe |
| Ownership authority/visual | `46006c4` GDD timeline và Technical; `229cc61` migration | Dedicated realtime, Spring durable; visual-only ranged/result per target | Không có conflict cần rollback network | Giữ origin/source snapshot, resolve clock, cost/cancel, damage/status; main VFX/impact/status đọc result |
| Quest counts/source | `229cc61` và staged consolidation giữ 3/1/5/4+Linh/6/3-3-4/6+Boss; whitelist còn trong Quest/Runtime/Playtest | Counts và source proposals chưa duyệt ở lượt trước | Prompt hiện tại duyệt counts mới và MobIdentity; staged canonical không còn đúng | Áp trực tiếp Q3=4,Q4=5,Q5=8,Q8=8+Linh,Q10=10,Q11=3/3/4,Q12=10+Boss; groups chỉ placement. Special actor/landmark explicit |
| NPC utility | Staged Quest/Items ghi Yên giữ HồiSinh/TẩyMạch, Mộc Storage/Rest | Ownership cũ và recommendation cũ | Prompt hiện tại chuyển utility sang Mộc | Yên Food/HP/MP; Bách gear/stone/Sell/Enhance/Transfer; Mộc Storage/Rest/utility hiện có. Giá/effect không đổi |

Không tìm được bằng chứng user approval riêng cho Arc120°, Line width0,6, X/Y proximity, behind-player hoặc SolidWall mode. Lịch sử chỉ đủ truy nguyên drift; quyết định mới không được gắn nhãn “đã khôi phục user lock” cho các số chưa xác nhận.

<a id="combat-completeness"></a>

## Six-skill completeness — authority và mức sẵn sàng

**L**=LOCKED direction/invariant; **B**=BASELINE/TUNABLE đang dùng; **O**=OPEN/PROBE, chưa production acceptance. Số skill/unlocks/caps giữ theo yêu cầu hiện tại; số power/MP/CD/timing/status đã có vẫn B, không được sửa trong correction này. “Primary” dùng A; “secondary” dùng B; visual dùng C ở contract trên. Mỗi cell tham chiếu luật trong owner này hoặc Art, không tạo implementation khác.

| Skill | Primary acquisition | Secondary selection | Target cap | Hit timing | Range | Power / MP / CD | Status | VFX | Per-target impact | Multiplayer | Failure/cancellation |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Kiếm S1 | L: Focus + eligible primary | L: không | L:1 | B:+0,12/lock0,30 | B:1,7; vertical/metric O | B:bảng skill | L:không | [Art contract](../03-art/art-and-visual-production.md#six-skill-visual): S1/default | L:landed result, NÉ riêng | L:server/target life; N ledger | L:invalid primary mất hit; death/map/CC cancel unresolved, không refund |
| Kiếm S2 | L:primary; auto-face B | B:query quanh primary tại resolve; bounds O | L:1+2 | B:+0,14/0,30 | B:primary1,7; splash O | B:bảng skill | B:Bỏng4%, cache unique | Art:S2 hot-qi; concept PROBE | L:mỗi landed target; không default double | L:distinct lives/actual loss | B:invalid primary không lan, Evade vẫn lan; cancel như trên |
| Kiếm S3 | L:primary; order B | B:primary index0 + gần-primary; bounds O | L:1+4 | B:+0,16/0,40 | B:primary5,5; splash O | B:bảng skill, index0..4 | B:Bỏng70%, cache unique | Art:signature qi-wave PROBE | L:actual targets kể cả ngoài nét wave | L:immutable resolve set/indices | B:invalid không lan, no retarget/refill; Evade không đổi indices |
| Cung S1 | L:Focus + eligible primary | L:không | L:1 | B:+0,12/0,30 | B:6,5; vertical O | B:bảng skill | L:không | Art:single spirit arrow | L:landed chỉ một, visual-only travel | L:server result/MapId/life | L:invalid primary mất hit; timeline cancel |
| Cung S2 | L:A primary snapshot start | L:ABC/ABA/AAA; order B; vertical/LoS O | L:3 indices,≤3 unique | B:batch+0,12/0,34 | B:6,5 execution envelope | B:bảng skill theo index | L:unique target/fail cache; B:category chances | Art:one draw/release,3 trails | L:mỗi landed index; scale clutter cosmetic | L:per-index rolls/N ledger | L:invalid index drop, không reacquire, A invalid không xóa B/C |
| Cung S3 | L:primary snapshot | L:radius2 quanh valid primary; order B/LoS O | L:1+4 | B:+0,18/0,40 | B:primary6,5 + radius2 | B:bảng skill | L:category branch; B:chances/durations | Art:cold explosion honest radius | L:primary không double-hit; secondary riêng | L:server/unique target/status | L:invalid primary không nổ, Evade vẫn nổ; timeline cancel |

Tất cả event/transport/presentation fields còn proposal ở Runtime. Six-skill definition đủ ngữ nghĩa để dựng C0; bounds/LoS/air còn gate trước production combat/content acceptance, không blocker Inventory domain độc lập.
