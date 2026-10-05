# Huyền Lộ — Phân tích thiết kế

## Tóm tắt

Analysis giữ lý do thiết kế, phép tính suy ra, mô phỏng và quyết định còn mở. Migration 2026-10-06 khóa cấu trúc select/execute; các số cadence, HP/MP và gear mới là PROBE BASELINE/TUNABLE. Mô phỏng mới và dữ liệu lịch sử được phân biệt ngay tại từng nhóm; không mô phỏng nào thay nghiệm thu game chạy thật.

## Tìm gì ở đâu

- [Chỉ số / TTK](#character-evidence), [farm](#farm-progression), [kinh tế](#economy-analysis).
- [Onboarding / áp lực nhận damage / câu hỏi còn mở](#novice-onboarding-evidence).
- [Gate / quyết định](#open-decisions), [A01–A17](#art-open-decisions), [rationale combat](#design-lock-rationale).

**Đối chiếu:** [GDD hiện hành](1_HUYEN_LO_GDD.md) · **Ngày:** 2026-10-06 · **Vai trò:** evidence, simulation và quyết định mở.

GDD sở hữu luật gameplay; Technical sở hữu hợp đồng triển khai. **DERIVED** là số tính từ GDD; **SIMULATION** là mô hình có giả định; **PROPOSAL** chưa thành luật. Migration 2026-10-06 thay input, cadence, HP nền Cung, các dòng HP/MP gear, mật độ và NPC. [Probe hiện hành](#current-balance-probe) tính lại các dependency đó; các bảng TTK/sustain/proc/farm/journey/PvP/Boss trước migration đều **LEGACY / SUPERSEDED**, giữ để đối chiếu và không pass revision mới. EXP, giá/drop/chi phí nâng, payout và luật contribution không đổi. Không có Unity/runtime acceptance mới trong lượt tài liệu.

<a id="balance-baselines"></a>

# 1. Phương pháp, đối chiếu và giới hạn

Power/MP/CD và HP-MP trang bị đã đổi sang baseline để thử trong GDD. Các bảng lịch sử bên dưới dùng cấu hình cũ; chỉ [probe hiện hành](#current-balance-probe) và bảng cường hóa được tính lại cho revision này, không thay thế GDD. **TÍNH TỪ LUẬT** là phép tính xác định; **MÔ PHỎNG** phụ thuộc giả định; **ĐỀ XUẤT** chưa là luật. PvP có Food/Potion và recovery checkpoint mới nên mô hình PvP cũ chỉ là đối chiếu sát thương trực tiếp, không dự báo thắng/hòa; farm scheduler cũ cũng cần rerun vì combat/resource đổi. Các mô hình dưới đây không nghiệm thu runtime; kết quả bản mẫu thuộc [Roadmap](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#prototype-runtime-history).

**Nguồn đầu vào:** [nhân vật](1_HUYEN_LO_GDD.md#character-power), [kỹ năng/trạng thái](1_HUYEN_LO_GDD.md#class-combat), [quái và bãi](1_HUYEN_LO_GDD.md#world-farm), [trang bị/thưởng](1_HUYEN_LO_GDD.md#gear-economy), [Food/Bình](1_HUYEN_LO_GDD.md#consumables-death). Analysis không giữ catalog thứ hai.

**LEGACY — phương pháp mô phỏng chiến đấu trước migration:** Python 3 tạm trong `/tmp`, 24 hạt giống 0–23; HP/sát thương/EXP làm tròn half-up, thưởng từng người làm tròn xuống. Phân điểm cân bằng có bảng số thật tại §2. Các đòn chọn theo thời gian khóa động tác, hồi chiêu và Linh lực; ưu tiên đại chiêu → tiến cảnh/nhập môn → đòn thường. Mỗi hit kiểm né/chí mạng/biến thiên sát thương; Food hồi mỗi 2 s, bình khi cần, hai loại hồi chiêu riêng 8 s. Thời gian đánh chia cho tỷ lệ ra đòn hữu hiệu giả định 0,85; không tính thêm Linh lực lần hai. Kiếm gom mục tiêu trong 1,2 u, Cung ở ≥4 u. Đây là mô hình giao tranh thuận lợi, chưa tính vị trí/hitbox/latency/death.

**LEGACY — mô hình bãi:** 16 hạt giống, 90 phút/lượt bỏ 15 phút đầu; mỗi điểm sinh quái hồi 25 s sau chết, 1–4 người chơi luân phiên Kiếm/Cung chọn cụm gần cấp rồi cụm còn nhiều quái. Không cho hai người claim cùng một lượt đánh trong mô hình; chưa mô phỏng cùng đánh/nhặt, địa hình hay năng lực mạng. Số EXP/Vàng/đá là ngân sách cả bản đồ, không nhân cho mỗi người. Linh Biến roll 5% từ quái Lv 8+, tối đa một con/MapId.

**LEGACY — mô hình hành trình:** 16 hạt giống/phái/kịch bản, Q1–Q3 chiếm 6 phút, nhiệm vụ/đường đi theo GDD; Q9 tùy chọn bỏ qua. Tiến cảnh chỉ sau Q8 và Lv 10, vũ khí Rare III/đại chiêu chỉ sau Q11. Tính Vàng, đá, Food và Bình Linh lực; không giả định +8 trước Q12. Không tính HP Potion/death, túi đầy, người chơi do dự, đi vòng vì địa hình hoặc mạng. Thời gian Lv 20 tách khỏi Q12/Boss. Có bốn mức đầu tư rõ tại §4.

**LEGACY — mô hình trạng thái:** 32 hạt giống trong hai giờ, 1–4 người đánh lệch pha, giả định đòn trúng và đủ Linh lực; đây là trần thuận lợi, không là tỷ lệ hiệu ứng thực khi chơi. Boss projection cộng hai dòng sát thương độc lập, giả định 50/65/75% thời gian ra đòn hữu hiệu. PvP chỉ mô hình một chiều, không là kết quả trận đấu.

**Bất biến kiểm được:** tổng NeedEXP tới Lv 20 là 53.100, tổng 95 điểm thuộc tính. 18 dòng / 21 mẫu trang bị thường + Mộc Kiếm. Mốc art cũ “12 module hình ảnh trên một rig 26 khung” là phép đếm ba band × bốn loại, không chứng minh tổng asset hoặc diễn giải 26-frame; [Art accounting](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#production-accounting) giữ kịch bản đầy đủ, A01/A17 OPEN. Bảy loại quái và bản đồ hiện hành cho đường farm hợp lệ tới Lv 20. Các phép tính này không thay bài kiểm thử khi triển khai.

<a id="character-evidence"></a>
<a id="combat-analysis"></a>

# 2. Nhân vật, chiến đấu và hiệu ứng

<a id="current-balance-probe"></a>

## Probe hiện hành 2026-10-06 — DERIVED / SIMULATION, chưa nghiệm thu

**Giả định → phép tính → kết luận:** dùng baseline GDD §2/§3/§6; nhập phái ở Lv 5, trừ ca chọn muộn riêng. Phân điểm cân bằng như bảng dưới, đủ bộ Common +0 theo cấp (I ở Lv 5/10, II ở Lv 13, III ở Lv 17/20). Bộ đồ chỉ là fixture kiểm chỉ số, không phải inventory được cấp cho người chơi. S1/S2/S3 có CD độc lập, chi phí mới và chung thời gian khóa action; không có đòn Normal sau nhập phái. Giá/nguồn, Food/Bình, HP quái và thưởng không đổi. Các số chỉnh lại đều **BASELINE/TUNABLE**, chưa LOCKED.

### HP Cung và HP/MP của gear

Cung tăng HP chậm hơn **sau cấp nhập phái thực `ClassChosenLevel`**, giữ HP nền đã tích lũy; Kiếm/Tân Lữ tiếp tục tăng +10. Chọn Cung Lv 5 có hệ số probe +8/cấp về sau, không khóa đề xuất +7. Chọn muộn Lv 10 giữ HP nền 210 tại giao dịch, Lv 11 mới lên 218; nếu cứ trừ theo Lv 5 sẽ mất 10 HP ngay lúc chọn, nên không dùng công thức đó. VIT +8/điểm không đổi, HP gear vẫn là lựa chọn build; Technical lưu cấp nhập phái cùng class receipt. Cộng/reset/equip chỉ clamp current HP/MP, không hồi theo tỷ lệ.

| Lv / điểm STR–VIT–INT–AGI | HP nền Kiếm / Cung (chọn Lv 5) | MaxHP Kiếm / Cung | MaxMP Kiếm / Cung |
| --- | --- | --- | --- |
| 5 / 5–5–5–5 | 160 / 160 | 312,4 / 284 | 143 / 157,3 |
| 10 / 11–12–11–11 | 210 / 200 | 429 / 380 | 193 / 212,3 |
| 17 / 20–20–20–20 | 280 / 256 | 776,6 / 682 | 338 / 371,8 |
| 20 / 24–24–24–23 | 310 / 280 | 844,8 / 738 | 370 / 407 |

Lv 5 không có chênh HP nền ở class choice. Lv 20 cân bằng Cung thấp hơn Kiếm 106,8 HP, gồm tăng HP chậm hơn và nội tại Kiếm Tâm; Cung vẫn có range/kite và Ưng Nhãn. Không cố cân từng encounter 50/50 hoặc giảm hệ số VIT riêng của Cung.

| Gear contribution Common +0 | I trước → mới | II trước → mới | III trước → mới |
| --- | --- | --- | --- |
| HP từ Armor/Pants/Boots | 65 → 84 | 145 → 188 | 205 → 266 |
| MP từ Weapon/Ring/Necklace | 20 → 42 | 40 → 76 | 60 → 114 |

HP tăng khoảng 29–30% **trên phần gear**; Lv 20 MaxHP Kiếm cân bằng tăng 777,7→844,8 (+8,6%). MP gear tăng mạnh theo tỷ lệ vì có hai dòng mới từ nền nhỏ; MaxMP Kiếm Lv 20 316→370 (+17,1%), Cung 347,6→407 (+17,1%), không tăng gấp đôi tài nguyên nhân vật. Armor giữ HP mạnh, Pants vừa, Boots nhẹ; Necklace MP mạnh hơn Weapon/Ring. Không suy stat từ bên trái/phải layout.

**Cực đoan Lv 20 Common III+0 — cùng 95 điểm:**

| Build | MaxHP Kiếm / Cung | MaxMP Kiếm / Cung | Ý nghĩa |
| --- | --- | --- | --- |
| Dồn STR | 633,6 / 546 | 250 / 275 | Sát thương cao, vẫn phải quản lý MP/đòn nhận |
| Dồn VIT | 1469,6 / 1306 | 250 / 275 | VIT giữ hiệu quả bằng nhau, không khóa progression |
| Dồn INT | 633,6 / 546 | 725 / 797,5 | MP và skill bonus cao, không tự tăng HP |
| Dồn AGI | 633,6 / 546 | 250 / 275 | Tăng hit/evasion, không tăng HP/MP qua AGI |
| Không VIT | 633,6 / 546 | 410 / 451 | HP gear giúp nhưng không cam kết chịu overlap |
| Không INT | 915,2 / 802 | 250 / 275 | Gear MP/Food hỗ trợ S2; weave vẫn tốn bình |

### Rarity / enhancement / Tinh Hoa — tính lại đủ biên

HP/MP mới thuộc primary list, cùng thứ tự nhân rarity → enhance → cộng flat/Tinh Hoa → cộng nền/điểm → nội tại một lần. Không nhân Crit/tốc chạy; ACC/EVA chỉ chịu rarity và flat hiện hành. Trần I+4/II+6/III+8, giá, tỷ lệ và milestone không đổi. Ví dụ Rare III Áo +8: HP 138×1,16×1,59+10 = 264,5272; DEF 15×1,16×1,59+2 = 29,666. Weapon/Ring có MP chịu cùng hệ số; Boots có HP chịu cùng hệ số, không sinh Tinh Hoa mới.

| Full III, Lv 20 cân bằng | MaxHP Kiếm / Cung | MaxMP Kiếm / Cung | ATK |
| --- | --- | --- | ---: |
| Common +0 | 844,8 / 738 | 370 / 407 | 91,6 |
| Common +4 | 923,1 / 809,18 | 406,22 / 446,84 | 100,8 |
| Common +8 | 1044,93 / 919,94 | 447,26 / 491,99 | 115,2 |
| Rare +4 | 980,68 / 861,53 | 428,66 / 471,52 | 108,67 |
| Rare +6 | 1038,38 / 913,98 | 451,14 / 496,25 | 116,56 |
| Rare +8 | 1119,37 / 987,61 | 476,26 / 523,89 | 125,38 |
| Epic +8 | 1161,24 / 1025,68 | 492,58 / 541,83 | 131,1 |

Đủ bộ Rare/Epic +8 là biên sau truyện, không phải điều kiện Q12. [Bảng tra Common từng món](#gear-upgrade-values) được tính lại toàn bộ; không dùng số HP/MP cũ của các mô hình lịch sử để nghiệm thu evaluator mới. ATK không đổi nên không có lý do tăng giá, HP quái hoặc drop chỉ vì thêm MP/HP.

### Cadence, animation và vai trò S1/S2/S3

CD probe 0,60/0,90/6 s nằm trong hướng S1 0,5–0,7; S2 0,7–1,0; S3 5–7, **không khóa những khoảng brainstorm**. Cost S1 giữ 2; S2 giảm 4→3 để kiểm farm thường xuyên; S3 giữ 16. Cung S2 giảm tổng power 2,4→1,8 qua ba indices 0,70/0,60/0,50, bù một phần tăng tần suất, tránh dùng nguyên burst cũ ở CD ngắn. Kiếm S2 giữ 1,35 mỗi victim, max3 để giữ lợi thế cluster.

| Profile trên một victim | Power/cast | Power/MP | Power/CD (trước INT/DEF/crit) | Action lock/CD |
| --- | ---: | ---: | ---: | ---: |
| Kiếm S1 | 1,20 | 0,60 | 2,00 | 0,30/0,60 = 50% |
| Kiếm S2 | 1,35 | 0,45 | 1,50 | 0,30/0,90 = 33,33% |
| Cung S1 | 1,15 | 0,575 | 1,9167 | 0,30/0,60 = 50% |
| Cung S2 AAA | 1,80 | 0,60 | 2,00 | 0,34/0,90 = 37,78% |
| S3 hai phái | Theo shape GDD | Theo số victim | CD 6 s; không full DPS độc lập | 0,40/6 = 6,67% |

Cung AAA hơn S1 khoảng 4,35% power/MP và power/CD; S1 vẫn commit 2 MP, CD ngắn hơn và ít overkill. Kiếm S1 tốt cho đơn, S2 tốt cho cụm (ba victim: tổng power 4,05/0,90 = 4,50/s trước giảm trừ). Đây là role direction, chưa chứng minh không có nút bị lép vế. S2 phải được đầu tư pose/release/impact và VFX sạch vì có thể bấm rất thường xuyên, không chỉ dành art cho S3.

Cộng thời gian chiếm lock lý tưởng của ba kỹ năng được 90% với Kiếm/94,44% với Cung, nhưng CD có thể trùng lúc và chỉ một action chạy. Không cộng power/CD để gọi là DPS luân phiên thực. HitMoment/resolve/action lock giữ baseline trước; Attack 3 hình ở 12 FPS dài 0,25 s, Skill 4 hình ở 12 FPS dài 0,333 s. Art phải đặt pose, đoạn giữ và hồi động tác theo đồng hồ/action đã nhận, không tăng lock chỉ để chạy hết clip; lock S2 Cung 0,34 s không phù hợp động tác kéo cung dài 1 s. Giữ trọng lực/quán tính; quyền S2/S3 trên không còn OPEN.

### MP/s, Food/Potion và sustain

Food hồi trung bình MaxMP × 0,75%/1%/1,25% mỗi giây ở bậc I/II/III; tick thực mỗi 2 s. Demand = cost/CD là trần nhu cầu khi bấm đủ nhịp thuận lợi, chưa trừ đi đường, hụt hình đòn, chỉnh vị trí hoặc CD trùng lúc. Dấu âm là Food hồi dư; dương cần MP có sẵn/bình/nghỉ. Chỉ S1 ở Lv 5, S2 từ Lv 10, S3 sau Q11.

| Lv / kiểu bấm | MaxMP Kiếm / Cung | Food MP/s Kiếm / Cung | Demand MP/s | Thiếu MP/s Kiếm / Cung |
| --- | --- | --- | ---: | --- |
| 5 / S1 | 143 / 157,3 | 1,0725 / 1,1798 | 3,3333 | 2,2608 / 2,1536 |
| 10 / S1 | 193 / 212,3 | 1,93 / 2,123 | 3,3333 | 1,4033 / 1,2103 |
| 10 / S2 | 193 / 212,3 | 1,93 / 2,123 | 3,3333 | 1,4033 / 1,2103 |
| 17 / S1 | 338 / 371,8 | 4,225 / 4,6475 | 3,3333 | -0,8917 / -1,3142 |
| 17 / S2 | 338 / 371,8 | 4,225 / 4,6475 | 3,3333 | -0,8917 / -1,3142 |
| 17 / S2+S3 | 338 / 371,8 | 4,225 / 4,6475 | 6 | 1,775 / 1,3525 |
| 17 / luân phiên cả ba | 338 / 371,8 | 4,225 / 4,6475 | 9,3333 | 5,1083 / 4,6858 |
| 20 / S1 | 370 / 407 | 4,625 / 5,0875 | 3,3333 | -1,2917 / -1,7542 |
| 20 / S2 | 370 / 407 | 4,625 / 5,0875 | 3,3333 | -1,2917 / -1,7542 |
| 20 / S2+S3 | 370 / 407 | 4,625 / 5,0875 | 6 | 1,375 / 0,9125 |
| 20 / luân phiên cả ba | 370 / 407 | 4,625 / 5,0875 | 9,3333 | 4,7083 / 4,2458 |

Lv 20 không INT: MP 250/275; Food III hồi 3,125/3,4375 MP/s. Chỉ S2 cần 3,3333 MP/s: Kiếm thiếu 0,2083, Cung dư 0,1042; thêm S3 đưa trần lên 6,0, thiếu 2,875/2,5625. Trang bị hỗ trợ farm; INT vẫn có ích khi dùng đòn mạnh/luân phiên. Không Food thì không tự hồi; hồ sơ cân bằng chỉ S2 tiêu pool 370/407 MP trong khoảng 111/122 s nếu bỏ mọi hồi phục.

Bình III hồi 222/244,2 MP; hồi chiêu MP 8 s tách hồi chiêu HP 8 s. Hồ sơ cân bằng Lv 20 bấm S2+S3 đủ nhịp thiếu 1,375/0,9125 MP/s → một bình khoảng mỗi 161/268 s khi đã ổn định, chưa tính pool đầu. Luân phiên cả ba ở trần thiếu 4,7083/4,2458 MP/s → khoảng 47/58 s/bình, tốn xấp xỉ 36.649/30.044 Vàng/h **nếu dùng hết lượng hồi**, cộng Food III 4.200 Vàng/h. Đây không phải cam kết đủ Vàng: mô hình Vàng/h cũ mất hiệu lực khi mật độ/nhịp đòn đổi. Ít thời gian đánh thực hơn sẽ giảm nhu cầu; không tự tăng Vàng/drop để bảo đảm luân phiên liên tục.

### TTK và group clear — mô phỏng mới 24 seeds

Python 3 tạm trong `/tmp`, 24 hạt giống 0–23; bước đồng hồ 0,01 s. Sát thương làm tròn half-up; né/chí mạng/random theo GDD; một Bỏng/mục tiêu, refresh giữ nhịp tick. Mọi mục tiêu đứng trong hình đòn hợp lệ; Kiếm ở ≤1,2 u, Cung ≥4 u để có nội tại Lv 13. Chụp nguồn lúc cast; lên lịch hit +0,12/0,14/0,16/0,18 s, index mất hiệu lực không chuyển đích. Spread ABC/ABA/AAA theo số mục tiêu sống lúc bắt đầu; giới hạn Line/nổ theo GDD. Đủ bộ Common +0, Food đúng bậc; Bình MP chỉ khi thiếu chi phí, hồi chiêu 8 s. Chưa tính phản công, Bình HP, chết, đi đường, túi/UI/mạng hoặc hụt hình đòn; chưa mô phỏng Đóng Băng/Làm Chậm vì không có AI phản công. Không nhân hệ số 0,85 của mô hình cũ. “Luân phiên” giả định người chơi chọn rồi Execute theo ưu tiên S3→S2→S1 khi sẵn, **không phải auto-combat hay lặp khi giữ phím của game**. Hàng Lv 17 giả định đã hoàn Q11/học S3, không dùng tính độ khó Q11.

| Lv / mob / số victim | Chỉ S2 Kiếm / Cung, s | Luân phiên Kiếm / Cung, s |
| --- | --- | --- |
| 5 / mob Lv 4 / 1 | Khóa | 1,4 / 1,37 (chỉ S1) |
| 5 / mob Lv 4 / 3 | Khóa | 5,22 / 5,22 (chỉ S1) |
| 5 / mob Lv 4 / 4 | Khóa | 7,09 / 7,09 (chỉ S1) |
| 10 / mob Lv 10 / 1 | 7,42 / 5,26 | 3,8 / 3,24 |
| 10 / mob Lv 10 / 3 | 7,94 / 17,11 | 6,66 / 10,46 |
| 10 / mob Lv 10 / 4 | 14,92 / 22,81 | 9,32 / 14,12 |
| 17 / mob Lv 16 / 1 | 7,24 / 5,26 | 3,16 / 2,7 |
| 17 / mob Lv 16 / 3 | 7,83 / 16,96 | 5,4 / 7,86 |
| 17 / mob Lv 16 / 4 | 14,39 / 22,58 | 6,69 / 10,41 |
| 20 / mob Lv 20 / 1 | 9,74 / 7,25 | 4,43 / 3,86 |
| 20 / mob Lv 20 / 3 | 10,27 / 22,92 | 6,78 / 11,22 |
| 20 / mob Lv 20 / 4 | 19,45 / 30,38 | 8,98 / 14,33 |

Các lượt ngắn trên không dùng Bình MP vì pool đầu còn đủ; **không suy khả năng đánh lâu dài từ số bình 0**. Kiếm S2 rõ lợi thế ở ba mục tiêu; bốn con vượt cap 3 cần lượt thêm. Cung AAA nhanh hơn Kiếm S2 trên một mục tiêu nhưng dọn cụm chậm hơn, bù bằng tầm/kite. Đánh đơn cuối game khi luân phiên thuận lợi có thể dưới mục tiêu 4–8 s; phải kiểm đồ chậm hơn mốc cấp, trước Q11, thời gian đổi bãi và người chơi thật trước chỉnh HP quái.

**Boss sensitivity mới, chưa trận Boss:** 120 s bấm thuận lợi cùng mô hình, target DEF25/EVA60 và HP rất lớn để đo dòng sát thương; Common III+0 cân bằng. Kiếm284,16 DPS, Cung318,30 DPS, MP dùng8,0417/8,025 mỗi giây; 129/128 S1,129 S2,20 S3 trong cửa sổ (cast cuối có thể resolve ngoài120 s), Kiếm dùng1 Bình MP, Cung0 do pool đầu. Burn giữ, target không phản công. Tổng602,46 DPS; nhân tỷ lệ ra đòn hữu hiệu50/65/75% → Boss32.000 khoảng106/82/71 s. Đây là sensitivity tính từ hai dòng độc lập, **không mô phỏng vừa né vừa dùng tài nguyên**; food/pool đầu khiến nhu cầu bình dài hạn khác. Biên65–75% có thể nhanh hơn target90–150 s: ghi rủi ro BOSS-02/BAL-02, giữ BossHP/ATK và telegraph hiện hành tới playtest, không gọi đã đạt target.

**PvP retune audit — arithmetic, chưa duel:** MaxHP Cung mới738 so Kiếm844,8 ở fixtureLv20; Bình III hồi442,8/506,88 HP. Ba lầnHP có trần tổng1.328,4/1.520,64 hồi (thực tế clamp/HP đầy/quota làm thấp hơn), Food III trung bình14,76/16,896 HP/s. Cadence mới tăng pressure nhưng gear/HP/Food/Potion thay cả sống sót lẫn hồi phục; bảng PvP TTK cũ không còn đủ. Hệ số0,20, quota3+3, CD8 s, Băng Hàn move-only, Bỏng immune, cược/payout và 120 s DRAW giữ nguyên. Cần duel cả chiều, VIT/INT/cực đoan/gear-lag và tần suất hòa trước retune hệ số hoặc economic stake.

### Dependency thiết kế: input, density, terrain và quest

Tách chọn/thực thi cho phép chọn S2 trước rồi Execute theo ý định rõ, một intent mỗi lần bấm; không bắt chọn lại S2 mỗi đòn. Bỏ letter keys khỏi movement để thử Execute/Interact/dùng đồ/menu RPG; **phím chính xác vẫn OPEN**. AUTO tìm cục bộ theo kỹ năng/khả năng tới đích, click ghim đích, Tab đổi cục bộ theo thứ tự ổn định; vùng tìm/giữ/thực thi tách nhau. Chết hủy lệnh chờ nhưng giữ focus hợp lệ để xem trận cùng farm và HP thay đổi; focus đời cũ không tự gắn quái respawn.

**Phân tích mật độ — Tăng số pocket thay vì gom blob:**
- *Gameplay & Combat flow:* Tăng số bãi nhỏ độc lập (1–3 quái/cụm) trên các tuyến/thềm địa hình thay vì gom 8–10 quái thành một khối dồn cục (`blob`). Gom blob sẽ phá vỡ cơ chế xếp hàng tiếp cận (`crowd staging`), khiến quái chồng lấn khó đọc, đồng thời tạo ra lượng sát thương dồn tức thời quá lớn khiến người chơi không thể phản ứng. Bố trí nhiều cụm nhỏ giúp duy trì nhịp độ ra đòn nhanh của S1/S2 và tạo điều kiện cho Kiếm tận dụng sát thương quét 3 mục tiêu hoặc Cung tận dụng tầm xa/Spread để xử lý nhiều mục tiêu.
- *Online co-farm & Contention:* Trong môi trường trực tuyến N-người chơi, nhiều cụm độc lập (ví dụ Đồng Sương 8–10 cụm, Trúc Ảnh 9–11 cụm, Bạch Vân 8–10 cụm, Xích Nham 9–11 cụm, Huyền Tích 8–10 cụm) cho phép các nhóm người chơi chia nhau các khu vực farm mà không bị nghẽn (bottleneck). Đồng thời giảm nguy cơ quái bị hạ gục quá nhanh trước khi người chơi kịp đạt ngưỡng đóng góp 20% máu để nhận tín chỉ nhiệm vụ.
- *Thị giác camera tiêu chuẩn:* Bố trí theo tầng (dưới 1–2 quái, giữa 1–3 quái, trên 1–2 quái, nhánh phụ 1–2 quái) giúp một khung hình camera có thể bao quát 5–8+ quái cùng lúc, tạo cảm giác thế giới hoang sơ, nguy hiểm và đông đúc, nhưng aggro vẫn được giữ độc lập nhờ ranh giới `HomeRegion` và `WalkRegion` riêng biệt.
- *Tải CPU / Physics / Network:* Phân tán quái theo các `SpawnGroup` nhỏ độc lập giúp AI server dễ dàng đưa các cụm không có người chơi vào trạng thái ngủ (dormant). Do địa hình hoàn toàn trực giao, không có dốc (`no slope`) và không có leo trèo (`no climb/ladder`), quái chỉ tuần tra trên mặt phẳng ngang của vùng đi lại được chỉ định, triệt tiêu hoàn toàn chi phí tìm đường nhiều tầng (multi-floor pathfinding).
- *Boss Exclusion Rule:* Tại Huyền Tích, việc loại trừ tuyệt đối quái thường khỏi `BossCombatArea` bảo vệ tính toàn vẹn của cuộc chiến với Boss Huyền Nham Cự Thú, ngăn chặn các trường hợp quái thường quấy rối telegraph hoặc bị lợi dụng để farm hồi phục/tích nộ ngoài ý muốn.
- *Dữ liệu lịch sử vs Kế hoạch hiện hành:* Mốc 28 cụm / 66 slots và khoảng cách tâm 18–20 u là seed lịch sử để đối chiếu prototype; kế hoạch authoring hiện hành đưa ra các khoảng ngân sách mục tiêu (14–20 ở Đồng Sương, 20–28 ở Trúc Ảnh/Bạch Vân, 24–32 ở Xích Nham, 20–26 ở Huyền Tích). Các mã authored ID mới (`DS7+`, `TA7+`, `BV6+`, `XN7+`, `HT6+`) mở rộng số cụm mà không làm xáo trộn các Quest Anchor IDs gốc (`DS2`, `DS3–DS6`, `TA4`, `TA6`, `TA4.slot1`, `XN1–XN6`, `HT4–HT5`, `HT_BossLandmark`) cũng như các stable authored seed IDs (`TA5`, `BV1–BV5`, v.v.).

**Phân tích thiết kế địa hình trực giao (Orthogonal Terrain Rationale):**
- *Tính toán hình học chiến đấu tất định:* Việc khóa địa hình tự nhiên thành khối đặc dày (`Solid Mass`), chỉ gồm mặt phẳng ngang và mặt đứng trực giao (loại bỏ hoàn toàn dốc nghiêng `slope/ramp/triangle`) là yêu cầu cốt lõi để đảm bảo sự chuẩn xác của hệ thống chiến đấu 2D authoritative. Mọi hình dạng kiểm tra sát thương (Melee single 1,7 u, Arc 120°, Line 5,5 u rộng 0,6 u, Logical single/Spread 6,5 u, Explosion bán kính 2 u) đều tính toán theo trục tọa độ trực giao. Dốc nghiêng sẽ làm lệch góc xoay hitbox, dẫn đến việc đòn đánh bị trượt hoặc xuyên thấu kỳ dị giữa Client và Dedicated Server. Mặt phẳng ngang đảm bảo việc kiểm tra chồng lấp dọc (`vertical overlap`) và va chạm hitbox–hurtbox tại `HitMoment` luôn mang tính tất định (deterministic).
- *Bảo vệ tính toàn vẹn của AI quái vật:* Quái vật mặt đất (Sói, Nấm, Đạo Tặc, Thạch Linh, Cổ Vệ) không có logic leo trèo phức tạp. Nếu map có thang dây hay cơ chế leo (`ladder/climb`), người chơi chỉ cần đu trên thang hoặc đứng trên vách hẹp bắn tỉa quái bên dưới mà quái không thể phản ứng, biến toàn bộ bãi quái thành bia tập bắn vô dụng. Việc triệt tiêu thang leo và thay bằng các khối bậc nhảy trực giao (`stepped blocks`) bảo đảm mọi cao độ đều được quy về các phép kiểm tra tiếp cận (`reachability evaluation`) rõ ràng.
- *Quy tắc sàn One-way khắt khe:* Đất đá tự nhiên không bao giờ là one-way platform để tránh cảm giác phi lý (đất đá không thể nhảy xuyên từ dưới lên). Sàn one-way chỉ dành cho kết cấu mỏng nhân tạo có trụ/dầm/dây treo (ván gỗ, giàn catwalk, ban công, sàn treo). Người chơi bấm `↓` (`DropThrough`) chỉ xuyên qua một tầng sàn, không xâu chuỗi nhiều sàn khi giữ nút.
- *Vùng nước nông (Shallow Water):* Lòng suối cạn có đáy đất thật, người chơi lội qua bị giảm tốc nhẹ khi chân chạm nước, nhưng khi đi trên cầu gỗ hoặc nhảy trên không thì giữ nguyên tốc độ. Không bổ sung cơ chế bơi lội hay đuối nước để tránh phình to phạm vi animation, trạng thái và vật lý.

**Kiting hợp lệ vs Lỗi góc chết (Legitimate Kiting vs Safe Perch Exploit):**
- *Bản sắc class của Cung thủ:* Cung thủ sở hữu tầm đánh xa 6,5 u và độ cơ động cao. Việc liên tục di chuyển lùi bước, vừa chạy vừa bắn và nhảy qua lại giữa các thềm đá trên cùng một tuyến đường đi lại được (`reachable path`) để tránh né đòn đánh cận chiến của quái là kỹ năng thả diều hoàn toàn hợp lệ và là lợi thế tự nhiên của phái đánh xa (`class advantage`).
- *Xử lý lỗi góc chết bằng cơ chế Return đơn giản:* Nếu người chơi nhảy lên một mỏm đá cụt hoặc thềm cao mà quái vật không có đường tiếp cận hợp lệ (unreachable), hoặc người chơi chạy vượt quá giới hạn truy đuổi (`LeashRegion`), quái vật sẽ kích hoạt trạng thái `Return` rút về vị trí xuất phát (`HomeRegion`), đích reset tại home là full HP (cách hồi trong Return còn OPEN), kết thúc giao tranh. Cách xử lý này giải quyết triệt để vấn đề người chơi lợi dụng lỗi địa hình để farm quái an toàn mà không cần phải gượng ép bổ sung đòn đánh xa vô lý cho quái cận chiến (như cho Sói bắn đạn) hay teleport gian lận.
- *Trạng thái Hybrid & Return:* Số lượng quái lai (Hybrid) và tham số cụ thể của Return (grace period, tốc độ rút lui, trạng thái invulnerability/targetability trong khi Return) vẫn là quyết định **OPEN / TUNABLE**.

Q3 chuyển Bách Luyện để vũ khí tutorial không khiến người chơi hiểu Phong Du là lựa chọn class mặc định. Q6 Lâm Bá giới thiệu hai mentor, mentor đã chọn nhập phái/giao supply/trả quest; giảm vòng đi qua NPC trung gian. Lâm Bá giữ mạch truyện Q8/Q10–Q12; Yên Thảo nhận Tẩy Mạch; Mộc An/Hạo Vũ giữ service. Bảy NPC ở khu chức năng giúp Q1 vẫn là tìm đường thật. Dev Mode cho test nhanh nhưng không cắt Q1–Q12 hay thay fresh-run evidence.

### Worldbuilding và naming audit — giữ nội dung mạnh, không nổ art scope

| Nhóm đã đối chiếu | Kết luận / lý do |
| --- | --- |
| Game title | Giữ Huyền Lộ, gợi hành trình và bí ẩn; không thêm thuật ngữ tôn/xianxia để phô trương. |
| Map display names | Giữ Vân Khê/Đồng Sương/Trúc Ảnh/Bạch Vân/Xích Nham/Huyền Tích, Học Viện/Lôi Đài; phù hợp dân dã→hiểm trở→huyền bí. |
| NPC | Giữ tên 7 NPC hiện hành (Lâm Bá, Yên Thảo, Bách Luyện, Mộc An, Phong Du, Diệp Lam, Hạo Vũ); Tạ Minh merge/remove là thay ownership, không chỉ display rename. |
| Quest titles | Giữ 12 tên trong GDD; thoại theo khu chức năng/người trả mới, Q12 khép Chương III chứ không Game Complete. |
| Skill / passive / manual | Giữ Phong Trảm/Linh Tiễn/Kiếm Khí/Hàn Tiễn, Kiếm Tâm/Ưng Nhãn/tiến cảnh/chân quyết; không thêm skill rank hay hệ tu luyện mới. |
| Gear families | Thanh Mộc/Vân Nham/Huyền Ấn giữ đường vật liệu và ba bậc; HP/MP retune không rename ItemId. |
| Materials/resources | Giữ Nấm Sương/Trúc Tâm/Vân Thạch/Khoáng Xích Nham/Mảnh Cổ Ấn/Tinh Thạch và Vàng; không crafting/gacha/currency mới. |
| Mob / Boss | Giữ Nấm Linh/Sói Sương/Sói Trúc Ảnh/Ong Giáp/Đoạt Mạch Đạo Tặc/Xích Thạch Linh/Cổ Môn Vệ Binh/Huyền Nham Cự Thú. Không đổi thành tên tầm thường chỉ để “Việt hơn”. |
| Status / landmarks | Giữ Linh Biến/Bỏng/Băng Hàn/Đóng Băng/Làm Chậm, trấn ấn/Linh Mạch/Huyền Môn/Dư Ảnh; dùng lại cơ chế đang có. |
| Dialogue / motifs | Lời mộc mạc, 1–3 câu, mỗi NPC giọng vừa đủ; núi rừng, gỗ/ngói/tre/cầu/dược/đèo/bia đá thay generic xianxia, không nhồi “ngươi/bổn tọa/linh căn”. |

Bối cảnh Việt Nam huyền sử tiền hiện đại không khóa triều đại hay tái dựng trang phục/công trình lịch sử. Bí ẩn đi từ trấn ấn nứt → mạch đất bất thường → Linh Biến → dấu người can thiệp → Huyền Môn/phế tích; không thêm danh phận cứu thế hoặc mechanic mới. Nếu đổi tên hiển thị sau này, giữ ID nội bộ ổn định; Technical/Art/thoại nhiệm vụ cùng theo GDD.

---

**Phần lịch sử bên dưới:** giữ số HP/MP/TTK/duy trì MP/PvP cũ để đối chiếu; chúng không phải kết quả probe 2026-10-06. Bảng EXP/nguồn farm/chi phí nâng/payout chưa đổi vẫn dùng được trong phạm vi ghi rõ.

## LEGACY — 95 điểm thuộc tính và fixture HP/MP trước migration

Mỗi cấp sau Lv 1 cho 5 điểm; Lv 20 có 95 điểm. Sau Q5 catch-up Lv 5, trước Q6 chọn phái có 20 điểm **chưa phân** sau lần hoàn điểm nhập môn; Q4 Nấm và Q5 Sói theo baseline route không yêu cầu đánh sau reset này; farm thêm có thể đạt Lv 5 sớm, vẫn giữ basic Tân Lữ/quest supply và không reset lần hai. Bản cân bằng sau chọn phái chia gần đều: điểm dư lần lượt vào Sinh Lực, Linh Lực, Công Lực, rồi Thân Pháp; không mặc định dồn phần dư cho Thân Pháp.

**Cách tính:** nền theo cấp + điểm + trang bị đã tính phẩm chất/cường hóa/Tinh Hoa, sau đó mới áp nội tại nền tảng một lần. Các hàng sau chọn phái dùng sáu món Common +0 theo mốc mặc, gồm Chí mạng trên Kiếm, Chính xác trên Cung và tốc chạy trên Giày (Lv 5/10 bậc I, Lv 13 bậc II, Lv 17/20 bậc III); đây là bộ kiểm chỉ số, không giả định người chơi đã mua đủ set. Trong các ô ghi hai số, thứ tự luôn là **Kiếm / Cung**. Hồ sơ Lv 5 trước chọn phái trong bảng là **fixture chỉ số** với Mộc Kiếm + Quần I, chưa hưởng nội tại; không mô tả inventory route đã nhận Áo từ Q4. Giữ số để đối chiếu, không tự cộng lại bảng này thành acceptance mới. Chính xác/Né tránh là chỉ số thô dùng trong công thức né, không phải phần trăm trúng/né; Cung được nhân Chính xác ×1,08 sau khi cộng điểm và trang bị. Tốc chạy tăng so với nền gồm +0,05% mỗi điểm Thân Pháp và bonus Giày cố định 1/2/3% theo bậc.

| Mốc / hồ sơ | Tổng điểm | Công Lực | Sinh Lực | Linh Lực | Thân Pháp | Chưa dùng | Máu Kiếm / Cung | Linh lực Kiếm / Cung | ATK | DEF Kiếm / Cung | Chính xác Kiếm / Cung | Né tránh | Chí mạng Kiếm / Cung | Tốc chạy tăng |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Lv 5 trước chọn phái | 20 | 0 | 0 | 0 | 0 | 20 | 185 (Tân Lữ) | 76 (Tân Lữ) | 26,8 | 10,4 (Tân Lữ) | 76 (Tân Lữ) | 28 | 5% (Tân Lữ) | 0% |
| Lv 5 cân bằng | 20 | 5 | 5 | 5 | 5 | 0 | 291,5 / 265,0 | 121,0 / 133,1 | 35,3 | 18,3 / 16,9 | 112 / 131,76 | 67 | 6,5% / 6% | 1,25% |
| Lv 10 cân bằng | 45 | 11 | 12 | 11 | 11 | 0 | 408,1 / 371,0 | 171,0 / 188,1 | 45,5 | 22,2 / 20,6 | 168 / 192,24 | 113 | 6,5% / 6% | 1,55% |
| Lv 13 cân bằng | 60 | 15 | 15 | 15 | 15 | 0 | 555,5 / 505,0 | 223,0 / 245,3 | 64,9 | 35,3 / 32,7 | 208 / 246,24 | 150 | 7,5% / 6,5% | 2,75% |
| Lv 17 cân bằng | 80 | 20 | 20 | 20 | 20 | 0 | 709,5 / 645,0 | 284,0 / 312,4 | 85,2 | 50,3 / 46,6 | 259 / 312,12 | 196 | 8,5% / 7% | 4% |
| Lv 20 cân bằng | 95 | 24 | 24 | 24 | 23 | 0 | 777,7 / 707,0 | 316,0 / 347,6 | 91,6 | 52,7 / 48,8 | 289 / 344,52 | 220 | 8,5% / 7% | 4,15% |
| Lv 20 Dồn Công Lực | 95 | 95 | 0 | 0 | 0 | 0 | 566,5 / 515,0 | 196,0 / 215,6 | 141,3 | 50,1 / 46,4 | 151 / 195,48 | 82 | 8,5% / 7% | 3% |
| Lv 20 Dồn Sinh Lực | 95 | 0 | 95 | 0 | 0 | 0 | 1402,5 / 1275,0 | 196,0 / 215,6 | 74,8 | 60,4 / 55,9 | 151 / 195,48 | 82 | 8,5% / 7% | 3% |
| Lv 20 Dồn Linh Lực | 95 | 0 | 0 | 95 | 0 | 0 | 566,5 / 515,0 | 671,0 / 738,1 | 74,8 | 50,1 / 46,4 | 151 / 195,48 | 82 | 8,5% / 7% | 3% |
| Lv 20 Dồn Thân Pháp | 95 | 0 | 0 | 0 | 95 | 0 | 566,5 / 515,0 | 196,0 / 215,6 | 74,8 | 50,1 / 46,4 | 721 / 811,08 | 652 | 8,5% / 7% | 7,75% |
| Lv 20 Không Sinh Lực | 95 | 32 | 0 | 32 | 31 | 0 | 566,5 / 515,0 | 356,0 / 391,6 | 97,2 | 50,1 / 46,4 | 337 / 396,36 | 268 | 8,5% / 7% | 4,55% |
| Lv 20 Không Linh Lực | 95 | 32 | 32 | 0 | 31 | 0 | 848,1 / 771,0 | 196,0 / 215,6 | 97,2 | 53,6 / 49,6 | 337 / 396,36 | 268 | 8,5% / 7% | 4,55% |

## LEGACY SIMULATION — TTK và duy trì Linh lực trước migration

Mô hình 24 hạt giống, timeline giả lập; một mục tiêu/ba mục tiêu trong cùng cụm. Cột Kiếm/Cung viết rõ từng phái. **Các số thời gian dưới đây là mốc trước khi vũ khí có Chí mạng/Chính xác phụ và Giày có tốc chạy**: tỷ lệ hit/crit và thời gian di chuyển giữa cụm có thể đổi. Cần chạy lại trước khi dùng làm mốc nghiệm thu. Không dùng kết quả bộ +8 để tăng HP quái cơ bản.

| Mốc / quái | HP quái | Một con Kiếm / Cung, giây | Ba con Kiếm / Cung, giây |
| --- | ---: | ---: | ---: |
| Lv 5 / Sói Sương (đã chọn phái) | 107 | 1,39 / 1,46 | 5,36 / 6,04 |
| Lv 8 / Sói Trúc | 339 | 4,16 / 4,39 | 13,91 / 14,39 |
| Lv 10 / Ong | 473 | 5,75 / 4,55 | 11,38 / 16,02 |
| Lv 11 / Ong | 473 | 3,99 / 3,39 | 8,33 / 11,33 |
| Lv 13 / Đạo Tặc | 704 | 5,55 / 4,29 | 10,74 / 15,1 |
| Lv 15 / Thạch Lv 16 | 974 | 7,55 / 6,2 | 14,34 / 20,11 |
| Lv 17 trước Q11 / Thạch | 974 | 5,18 / 4,55 | 8,85 / 13,35 |
| Lv 17 sau Q11 / Thạch | 974 | 3,71 / 2,86 | 7,38 / 9,4 |
| Lv 20 Common III +0 / Cổ Vệ | 1393 | 7,28 / 6,35 | 11,81 / 17,05 |
| Lv 20 chính tuyến / Cổ Vệ | 1393 | 5,28 / 4,62 | 9,0 / 13,73 |
| Lv 20 Epic III +8 / Cổ Vệ | 1393 | 4,45 / 3,89 | 8,44 / 11,4 |

Đối chiếu cụm hai/bốn con trên cùng hồ sơ; Kiếm/Cung đều phải ra nhiều action, không xóa cả cụm bằng một nút:

| Hồ sơ | Hai con Kiếm / Cung, giây | Bốn con Kiếm / Cung, giây |
| --- | ---: | ---: |
| Lv 8 / Sói Trúc | 9,07 / 9,42 | 18,76 / 19,54 |
| Lv 10 / Ong | 9,06 / 10,42 | 15,61 / 21,32 |
| Lv 15 / Thạch | 11,59 / 13,14 | 19,78 / 27,34 |
| Lv 17 trước Q11 / Thạch | 8,31 / 8,87 | 11,51 / 17,08 |
| Lv 20 chính tuyến / Cổ Vệ | 8,40 / 9,01 | 11,87 / 17,64 |
| Lv 20 Epic III +8 / Cổ Vệ | 7,59 / 8,50 | 10,26 / 15,54 |

Sói Sương Lv 4 tại Lv 5 là quái thấp cấp sau cú nhảy chọn phái, nên hạ trong ~1,4 s; Tân Lữ Lv 4 với Mộc Kiếm ~3,9 s là mốc mô hình cũ, không phải TTK của nhịp đã duyệt; xem [kiểm chứng onboarding](#novice-onboarding-evidence). Cụm ba/bốn con ở Lv 8+ còn sống qua nhiều action, cho kỹ năng đánh lan giá trị. Q8 Sói Trúc Linh Biến có **1.695 HP**, với đồ kỳ vọng mất **22,7/23,7 s** (Kiếm/Cung) solo. Q10 ở Lv 15 đánh ba Đạo Tặc mất **10,0/14,0 s**; Đạo Tặc Lv 13 còn thưởng đủ. Lv 15 đánh Thạch Lv 16 trước đại chiêu hơi chậm. Trước Q11, Lv 17 đánh ba Thạch bằng đồ II mất **8,8/13,3 s**; cần kiểm né và dùng thuốc trong cảnh thật để tránh kẹt khi ba quái cùng áp sát.

| Hồ sơ Lv 20, Common III +0 | Linh lực Kiếm / Cung | Thiếu Linh lực mỗi giây Kiếm / Cung |
| --- | ---: | ---: |
| Cân bằng | 316,00 / 347,60 | 1,00 / 0,29 |
| Dồn Công Lực | 196,00 / 215,60 | 2,50 / 1,94 |
| Dồn Sinh Lực | 196,00 / 215,60 | 2,50 / 1,94 |
| Dồn Linh Lực | 671,00 / 738,10 | -3,44 / -4,59 |
| Dồn Thân Pháp | 196,00 / 215,60 | 2,50 / 1,94 |
| Không Sinh Lực | 356,00 / 391,60 | 0,50 / -0,26 |
| Không Linh Lực | 196,00 / 215,60 | 2,50 / 1,94 |

Giá trị dương: cần thêm bình/nghỉ nếu dùng kỹ năng liên tục ngay khi hết hồi chiêu; âm: Food hồi dư. Tính Food III +2,5% MaxMP mỗi 2 s, không tính hai lần. Bình III hồi 60% MaxMP; mọi bậc bình Linh lực chung hồi chiêu 8 s, riêng bình Máu có đồng hồ khác. Khi di chuyển/nhặt đồ giữa các trận, nhu cầu bình thấp hơn.

**Sát thương nhận ở các mốc:** giả định quái đánh mỗi 1,6 s, không né chủ động; Food hồi đúng bậc mỗi 2 s. Đây là máu mất ròng mỗi giây sau Food; Chí mạng/Chính xác vũ khí không đổi Máu tối đa hoặc hồi Food. Nhịp đánh 1,6 s chỉ là giả định mô hình, cần đo trên cảnh thật.

| Hồ sơ kỳ vọng / quái | Một quái Kiếm / Cung | Hai quái Kiếm / Cung | Ba quái Kiếm / Cung |
| --- | ---: | ---: | ---: |
| Lv 8 / Sói Trúc | 6,5 / 6,9 | 16,5 / 17,1 | 26,6 / 27,3 |
| Lv 13 / Đạo Tặc | 7,0 / 7,9 | 21,8 / 23,0 | 36,6 / 38,0 |
| Lv 17 trước Q11 / Thạch | 4,5 / 6,0 | 21,8 / 23,6 | 39,1 / 41,3 |
| Lv 20 chính tuyến / Cổ Vệ | 4,8 / 6,8 | 25,2 / 27,6 | 45,6 / 48,5 |

**So build Lv 20:** Common III +0, Cổ Vệ ATK 60, cùng giả định. Bảng này dùng vũ khí III +0 với chỉ số Chí mạng/Chính xác mới; Máu tối đa và hồi Food không đổi. Dấu âm nghĩa là Food hồi nhiều hơn sát thương kỳ vọng. Worst Boss hit là Nham Thạch Rơi 1,8 × 160 × 1,05 qua DEF, không né/chí mạng/hồi giữa đòn.

| Hồ sơ | Máu Kiếm / Cung | Mất ròng với 1 / 2 / 3 Cổ Vệ (Kiếm) | Worst Boss hit Kiếm / Cung |
| --- | ---: | ---: | ---: |
| Cân bằng | 778 / 707 | 4,4 / 24,4 / 44,4 | 198 / 203 |
| Không Sinh Lực | 567 / 515 | 8,5 / 28,3 / 48,1 | 201 / 207 |
| Dồn Sinh Lực | 1403 / 1275 | -7,0 / 14,0 / 35,0 | 189 / 194 |

Dồn Sinh Lực sống dai nhưng đánh chậm; không Sinh Lực vẫn chịu được hai worst hit riêng lẻ trong mô hình, không chịu được overlap bất cẩn. Ba quái cần di chuyển và dùng bình đúng lúc. Đường đạn, hitbox, recovery, target switching và latency chưa có trong mô hình.

## LEGACY SIMULATION — nội tại tinh thông ở Lv 13

Kiếm Thế chỉ tăng 12% sát thương trực tiếp của kỹ năng khi mục tiêu ≤1,2 u; Xạ Tâm chỉ tăng khi impact ≥4 u. Phép thử cũ cho Kiếm 6,21→5,82 s solo và Cung 5,16→4,60 s; thay phân điểm cân bằng có thể dịch số nhỏ, nhưng điều kiện và xu hướng không đổi. Không cộng vào đòn thường, Bỏng hoặc xác suất trạng thái. Cảm giác thực cần combat slice.
## LEGACY SIMULATION — Bỏng/Băng Hàn ở cadence cũ

Status probe 32 seeds, hai giờ, nhịp cast cố định và giả định **mọi hit trúng, đủ MP**. Cột trái chỉ core, phải core + đại chiêu. Bỏng: core 4%, đại chiêu 70%, 6 s, tick 1 s × 0,06 source ATK; một effect/target, refresh expiry/source **giữ next tick**. Normal/Linh cùng Đóng Băng 1,5 s; proc core 2/1%, đại chiêu 45/30%; sau tan băng miễn 3 s target-wide. Boss Làm Chậm proc core 2%/big 100%, MoveSpeed ×0,85 và ActionClockSpeed ×0,75 trong 3 s, refresh-only. Không đặt Đóng Băng và Làm Chậm cùng một target. PvP chỉ MoveSpeed ×0,75 trong 1,5 s, không Đóng Băng/Bỏng.

| Số người cùng đánh | Bỏng: cơ bản / có đại chiêu, % thời gian | Quái thường Đóng Băng: cơ bản / có đại chiêu | Linh Biến Đóng Băng: cơ bản / có đại chiêu | Boss Làm Chậm: cơ bản / có đại chiêu |
| --- | --- | --- | --- | --- |
| 1 | 14,77 / 65,64% | 1,67 / 10,40% | 0,83 / 6,98% | 3,43 / 44,80% |
| 2 | 27,77 / 89,03% | 3,22 / 14,51% | 1,66 / 10,69% | 6,92 / 86,70% |
| 3 | 38,90 / 96,56% | 4,70 / 20,73% | 2,47 / 15,62% | 10,34 / 100,00% |
| 4 | 48,04 / 99,03% | 5,97 / 21,29% | 3,21 / 16,99% | 13,52 / 100,00% |

Bốn Cung cho quái thường Đóng Băng khoảng **21,3%**, Linh **17,0%** trong phép thử thuận lợi; giới hạn lý thuyết do 1,5 s + 3 s miễn hiệu ứng là **33,3%**. Bỏng từ bốn Kiếm gần như liên tục khi dùng đại chiêu đúng hồi chiêu, nhưng **vẫn một lần sát thương mỗi giây, không nhân bốn lần sát thương**. Số nhịp Bỏng mỗi giây nếu đặt lại nhịp đầu so với giữ lịch cũ: 1 Kiếm **0,633/0,651**, 2 **0,780/0,877**, 4 **0,687/0,987**; vì vậy luật giữ `nextTickAt`. Tick Lv 20 chỉ khoảng 5–6 HP trước DEF cho Cổ Vệ 1.393 HP, Bỏng chỉ bổ sung sát thương, không phải đòn bùng nổ. Với ba Cung cast đại chiêu lệch pha, Boss Làm Chậm có thể đạt 100% thời gian trong phép thử; đồng hồ chờ action tương lai vẫn chỉ kéo dài tối đa **33,3%** (tốc đếm 75%), mức làm chậm không chồng và vùng báo đòn/action đã bắt đầu không đổi. Không cần guard thứ hai trước kiểm bằng game chạy được, nhưng CC-01 phải đo Boss không mất đe dọa. Đóng Băng và Làm Chậm cần VFX khác nhau theo GDD/Technical.

## LEGACY SIMULATION — PvP một chiều với HP/MP/cadence cũ

Đòn Kiếm vào Cung cùng hồ sơ điểm/trang bị, mục tiêu đứng yên; không phản công, né bằng di chuyển, Food/Bình **đang được phép dùng trong PvP**, mạng hoặc tác dụng Làm Chậm. Dùng hệ số trước DEF; số là thời gian hạ mục tiêu nếu không hồi phục, giây. Dòng Epic +8 là biên cuối game, không dùng để cân toàn bộ hành trình hay suy rằng trận sẽ có người thắng trước 120 s.

| Trang bị cân bằng Lv 20 | Hệ số 0,15 | Hệ số 0,20 | Hệ số 0,25 |
| --- | ---: | ---: | ---: |
| Common III +0 | 35,35 | 26,32 | 20,87 |
| Rare III +6 | 37,47 | 28,01 | 22,53 |
| Epic III +8 | 39,98 | 30,06 | 24,06 |

Với hệ số **0,20 BASELINE**, so hồ sơ cực đoan trên Common III +0 và Epic III +8:

| Phân điểm | Common III +0, giây | Epic III +8, giây |
| --- | ---: | ---: |
| Cân bằng | 26,32 | 30,06 |
| Dồn Công Lực | 11,92 | 17,3 |
| Dồn Sinh Lực | 81,41 | 60,03 |
| Dồn Linh Lực | 19,66 | 23,62 |
| Dồn Thân Pháp | 24,6 | 28,54 |
| Không Sinh Lực | 17,44 | 22,05 |
| Không Linh Lực | 28,73 | 32,5 |

Dồn Sinh Lực có trận dài, dồn Công Lực có trận ngắn; điều này là khác biệt build, chưa chứng minh mất cân bằng trong đấu thực. Đảo chiều Cung→Kiếm cân bằng ở hệ số 0,20 cho **25,9 s** với Common III +0, **29,7 s** với Epic III +8; gần với Kiếm→Cung nhưng chưa tính lợi thế khoảng cách. Hệ số 0,20 giữ phản ứng cho build cân bằng; 0,25 làm nhiều ca ngắn, 0,15 kéo dài. PvP Băng Hàn chỉ giảm 25% tốc chạy trong 1,5 s; mô hình tĩnh chưa đo truy đuổi/giữ khoảng cách, Food tick và tối đa 3 bình HP + 3 bình MP/người. **Hết 120 s cả hai còn sống là DRAW bất kể tỷ lệ HP**; cần duel thật để đo tần suất hòa, không dùng bảng TTK để chọn thắng bằng HP.

**Đối chiếu tiền cược xác định** (hai người cùng cược `W`, pot `2W`; Spring escrow trước khi MatchId bắt đầu):

| W mỗi người | WIN/FORFEIT: winner nhận / phí hệ thống | DRAW: mỗi người nhận lại | SYSTEM_ABORT/cancel trước Active: mỗi người nhận lại |
| ---: | ---: | ---: | ---: |
| 1.000 | 1.800 / 200 | 900 | 1.000 |
| 5.000 | 9.000 / 1.000 | 4.500 | 5.000 |
| 10.000 | 18.000 / 2.000 | 9.000 | 10.000 |

Winner đã nộp `W` nên lãi ròng `0,8W`; loser mất `W`. DRAW mỗi người mất `0,1W`; system abort không thu phí. Mọi mức cược 1.000–10.000 bước 1.000 cho kết quả Vàng nguyên; không cần làm tròn. Bảng này kiểm arithmetic, chưa đánh giá đủ Vàng ở Q9 hay hành vi người chơi; Q9 vẫn optional.

<a id="art-combat-timing-evidence"></a>

**LEGACY TIMING/ROTATION EVIDENCE :** toàn bộ các bảng/phép tính trong nhóm timing/sustain/proc sau đây giả định class Normal + evolution overwrite. Giữ nguyên để đối chiếu; các recommendation cũ không còn luật. GDD §3 hiện dùng basic chỉ Tân Lữ, ba class SkillIds độc lập, logical ranged batch.


## LEGACY / SUPERSEDED — timing từ Art trước retune

Phần này chuyển đầy đủ từ Art §10/§10.1/§10.2 ngày 2026-10-03. Chủ sở hữu phép tính/probe là Analysis; [Art §10](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#art-timing) giữ cách diễn pose/VFX. Đây là **DERIVED / PROPOSAL**, không kết quả Unity. Các mốc test không thay luật [GDD §3](1_HUYEN_LO_GDD.md#class-combat).

**Đọc từ:** GDD §3, Analysis §2. Giữ số hiện hành trước test; mô phỏng TTK cũ chưa tính đủ Chí mạng/Chính xác vũ khí và Giày, không bằng chứng chốt nhịp.

| Đại lượng | Hiện hành / suy ra | Probe recommendation, chưa đổi luật |
| --- | --- | --- |
| Normal interval | Tân Lữ1,00 s; Kiếm0,80 s; Cung0,90 s | Giữ làm control; nếu feel sai, thử Kiếm0,70–0,90 và Cung0,85–1,05 s theo cặp, gap khoảng0,05–0,20 s. Không đổi range/damage cùng lượt thử |
| Cung chậm hơn Kiếm | Interval +0,10 s =12,5% dài hơn; tần suất đòn1,111 so1,25/s =11,1% ít hơn | 0,10 s hợp lý để probe draw, chưa chứng minh tối ưu. Với power0,95, raw normal/s khoảng15,6% thấp hơn Kiếm trước hit/crit, bù bằng range/accuracy chứ không riêng art |
| Normal hit/spawn và lock | +0,10 s /0,26 s cho mọi class | Kiếm Attack3 ở12 FPS dài0,25 s gần khớp lock; hit0,10 cần duration riêng. Cung draw0,10 s có nguy cơ quá ngắn. Thử release0,12–0,18 s, lock0,28–0,36 s **chỉ nếu cần**, rerun scheduler/TTK |
| Lv 5 core | 2MP/1,0 s, spawn/hit0,12 s, lock0,30 s | Pose prepare/draw cần hoàn trước0,12; dùng hold/time map. Nếu spam làm normal không có vai trò, đo usage rồi thử CD1,2/1,4 s riêng, không mặc định nerf |
| Lv 10 core | Kiếm4MP/1,5 s/lock0,30; Cung4MP/1,7 s/lock0,34 | Giữ control; khi cần probe CD ±0,2 s, giữ cost/power trước để tách ảnh hưởng |
| Lv 17 signature | 16MP/7 s/lock0,40; Kiếm hit0,16, Hàn spawn0,18 | Cast chuẩn bị có thể giữ bốn pose khác duration. CD6/7/8 s là sensitivity test sau có scene, không tự tăng để VFX dài hơn |

Timing contract là **anticipation → release/hit mốc server → recovery**, tách interval/CD/action lock/flight/VFX decay. Skill4 ở12 FPS dài0,333 s >lock nhập môn0,30 s: nếu giữ duration đều sẽ trễ pose return hoặc khóa người chơi ngoài luật. Đề xuất sampling theo normalized action phase, đặt release khớp mốc server, cắt/chuyển recovery khi action mới hợp lệ; main VFX có thể tan sau actor về Idle. Không queue thêm hit do frame skipped.

**LEGACY / SUPERSEDED — class Normal , không phải reasoning hiện hành:** Normal từng không MP, lấp quãng CD/MP thiếu và gây damage ổn định. Trong giả định skill luôn sẵn đúng CD, Lv 10 core chiếm lock khoảng0,30/1,5 hoặc0,34/1,7 =**20% thời gian**; Lv 17 thêm0,40/7≈5,7%. Đây chỉ là occupancy suy ra, không bảo đảm normal đạt full attack rate vì mốc CD có thể đụng nhau. Cần log số normal/skill/idle do MP và action lock; không kết luận normal vô dụng từ CD1 s của Lv 5.

<a id="art-sustain-evidence"></a>

### LEGACY SIMULATION — sustain Lv 5/10/20 trước retune

Phép tính lịch sử dưới dùng bảng chỉ số của revision cũ trong Analysis, hồ sơ cân bằng/đủ bộ Common+0 ở đúng band, Food đúng cấp. Upper demand = cost/CD cho core + big khi đã học; không tính travel/evade/lock làm bỏ cast, không dự báo TTK thực.

| Mốc / Kiếm–Cung | MaxMP | Food MP/s | Upper skill MP/s | Thiếu MP/s |
| --- | --- | --- | --- | --- |
| Lv 5 / Food I / core | 121 /133,1 | 0,9075 /0,9983 | 2 /2 | 1,0925 /1,0018 |
| Lv 10 / Food II / tiến cảnh | 171 /188,1 | 1,71 /1,881 | 4/1,5=2,6667 /4/1,7=2,3529 | 0,9567 /0,4719 |
| Lv 20 / Food III / core+big | 316 /347,6 | 3,95 /4,345 | 4/1,5+16/7=4,9524 /4/1,7+16/7=4,6387 | 1,0024 /0,2937 |

Food I/II/III hồi MP tương ứng0,75%/1%/1,25% MaxMP mỗi giây trung bình. Không tính regen khi chết/không Food. Thay CD, action lock, Boots hoặc target travel sẽ đổi thời gian hữu hiệu/cost bình; thử có Food, thiếu Food, cân bằng và zero-INT, không dùng full+8 làm chuẩn mọi người.

TTK target cùng cấp/Common+0 hiện early2–4 s, mid3–6, late4–8 s; các loài chỉ có bảy level cố định nên đo actual pair gần cấp, không dựng thêm mob giả cùng mỗi level. Đo trước/sau Q11, solo và nhóm2/3/5 target, hit/miss ở hai tầng, run-back và 2/4 player. Giữ HP curve trước khi có evidence, không tăng máu để bù VFX signature quá dài.

<a id="art-proc-evidence"></a>

### LEGACY SIMULATION — proc/feedback với cadence cũ

**LEGACY cadence:** class Normal cũ không roll Bỏng/Băng Hàn. Upper successful chance cadence trên một target trúng mọi cast: Kiếm core4%/1,5 s → một proc kỳ vọng mỗi37,5 s; Cung core2%/1,7 s →85 s, Linh1%→170 s. Đại chiêu Kiếm70%/7 s →10 s; Hàn normal45%→15,6 s, Linh30%→23,3 s. Đây là tần suất roll thuận lợi, không uptime vì refresh/protection/miss/đổi target. Không tăng core proc chỉ để art status xuất hiện nhiều.

Giữ Bỏng một overlay và tick1 s, refresh không restart animation entry liên tục; Freeze1,5 s rồi protection3 s target-wide; Boss/PvP chỉ Slow. Ba tên A/A/A chỉ một roll/effect/unique actual target/cast, kể cả fail. Test 1/2/4 Kiếm/Cung lệch pha vì overlay/proc spam có thể che action dù damage không stack. Giữ số proc hiện tại, dùng fixture status có chủ đích để kiểm art thay vì buff gameplay.

<a id="farm-progression"></a>
<a id="world-economy-analysis"></a>

# 3. Đường farm và tranh chấp bãi

## Lv 1 → 20 — lộ trình tính từ luật

Mỗi loại quái có một level cố định. HP/EXP/Gold derive từ GDD; band dưới là **loot source**, không đòi full set. Tier Food/Potion dùng theo player level có thể khác Potion rơi từ source. Ở Lv 20, EXP = 0 dù Cổ Vệ có base EXP 108. Map gates/quest markers tại GDD.

| Player Lv | Map / cụm nên farm | Mob Lv | HP / EXP / Gold | Gear drop / Potion-Food dùng | Lý do chuyển bãi |
| --- | --- | --- | --- | --- | --- |
| 1 | Vân Khê | Talk | — | — / I | Q1 nhớ NPC / đường về |
| 2 | Học Viện | Movement | — | — / I | Q2 platform |
| 3 | Học Viện → Đồng DS2 | Dummy → Nấm Lv 2 | Dummy 60 / 0 / 0; Nấm 48 / 15 / 7–12 | Mộc + Quần I; supply Áo I / I | Q3 không EXP; Q4 Nấm/loot/equip/sell catch-up Lv 4 |
| 4 | Đồng DS3–DS6 | Sói Sương Lv 4 | 107 / 22 / 11–18 | I, không Weapon / I | Q5 Food/thuốc + 5 Sói, turn-in catch-up Lv 5 |
| 5 | Học Viện Q6 → Trúc TA1–3 | Sói Sương Lv 4 | 107 / 22 / 11–18 | I, không Weapon / I | Q6 class/manual; không quay lại Nấm làm tutorial mới |
| 6 | Trúc TA1–3 hoặc TA4 / 6 | Sói Sương Lv 4 | 107 / 22 / 11–18 | I, không Weapon / I | Sói Lv 4 còn thưởng; Sói Lv 8 khó hơn nếu chọn |
| 7 | Trúc TA4 / TA6 | Sói Trúc Lv 8 | 339 / 38 / 19–30 | I / I | Q7 nhẫn +1; chuyển Sói Lv 8 trước mốc Lv 8 |
| 8 | Trúc TA4 / 6 → Bạch BV1 / 2 | Sói Trúc Lv 8 | 339 / 38 / 19–30 | I / I | Q8 forced Linh / book / gates, chưa auto tiến cảnh |
| 9 | Bạch BV1 / 2 | Ong Lv 10 | 473 / 47 / 23–36 | II chưa mặc / I | Luyện trước tiến cảnh, Sói Lv 8 vẫn hợp lệ |
| 10 | Bạch BV1 / 2 → BV3–5 | Ong Lv 10 | 473 / 47 / 23–36 | II chưa mặc / II | Học tiến cảnh; Đoạt Lv 13 là lựa chọn khó hơn |
| 11 | Bạch BV1 / 2 / BV3–5 | Ong Lv 10 | 473 / 47 / 23–36 | II mặc được / II | Mốc gear riêng sau tiến cảnh Lv 10; chọn nâng I hay thay II |
| 12 | Bạch BV3–5 / Xích XN1–3 | Đoạt Lv 13 | 704 / 63 / 29–45 | II / II | Giữ/mua thêm II; ngoại vi Xích, Q9 optional |
| 13 | Xích XN1–3 | Đoạt Lv 13 | 704 / 63 / 29–45 | II / II | Nội tại II tự mở; không active Lv 13 |
| 14 | Xích XN1–3 hoặc XN4–6 | Đoạt Lv 13 | 704 / 63 / 29–45 | II / II | Đoạt Lv 13 gần cấp; Thạch Lv 16 nếu đủ sức |
| 15 | Xích XN1–XN3(Q10) → XN4–6 | Thạch Lv 16 | 974 / 81 / 35–54 | II / III | Q10 6 Đoạt Lv 13 / evidence; Food III |
| 16 | Xích XN4–6 | Thạch Lv 16 | 974 / 81 / 35–54 | II / III | Core / Bỏng / position, chưa big |
| 17 | Xích XN4–6(Q11) → Huyền HT1 | Thạch Lv 16 | 974 / 81 / 35–54 | II; Q11 Rare III Weapon / III | Ba khu cùng Thạch Lv 16; big sau turn-in |
| 18 | Huyền HT1 hoặc HT2–5 | Cổ Lv 20 | 1393 / 108 / 43–66 | III / III | Cổ Lv 20 khó hơn; HT1 Thạch Lv 16 vẫn full reward |
| 19 | Huyền HT2–5 | Cổ Lv 20 | 1393 / 108 / 43–66 | III / III | Big farm; III từ Cổ, Thạch Lv 16 vẫn trong 3 cấp |
| 20 | Huyền HT2–5 / Boss | Cổ Lv 20 | 1393 / 0 / 43–66 | III / III | Cap: 0 EXP; Thạch Lv 16 không thưởng farm; Q12 / endgame |

Lv 5–7 có thể chọn Sói Lv 8 trong gap 3, nhưng không đảm bảo survival; Lv 8 không còn thưởng từ Sói Lv 4. Ong Lv 10 bắt đầu rơi Band II: Lv 10 có thể giữ trong bag, Lv 11 mặc được; mốc mở ngoại vi Xích Nham vẫn là Lv 12. Lv 17 đánh Thạch Lv 16 vẫn gear II; weapon Q11 và Cổ Vệ dẫn sang III. Quest muộn quay về quái thấp vẫn lấy objective/evidence, không lấy regular budget ngoài khoảng.

## LEGACY SIMULATION — lịch sinh quái với seed 28/66

N = 1–4 cùng MapId nhưng chia pull trong scheduler; chưa mô phỏng cùng đánh/tranh loot, geometry hoặc network. Throughput là budget cả map, không full reward mỗi người. Linh counts thấp hơn 5% kills do cap/lifetime. Q8 force nằm trong journey, không steady-state world rows.

| Map / player Lv | N = 1 normal / Linh deaths / h | N = 1 EXP / Gold / h | N = 1 Stone / gear generated / h | Idle wait % N = 1 / 2 / 3 / 4 |
| --- | --- | --- | --- | --- |
| Đồng Sương (DS) / 4 | 532,0 / 0,0 | 11.704 / 7.714 | 42,6 / 27,1 | 0,0 / 0,0 / 15,0 / 35,9 |
| Trúc Ảnh (TA) / 5 | 609,6 / 0,0 | 13.411 / 8.839 | 48,8 / 31,1 | 0,0 / 0,0 / 2,9 / 20,6 |
| Trúc Ảnh (TA) / 8 | 342,3 / 14,8 | 14.700 / 9.478 | 42,2 / 22,9 | 0,0 / 0,6 / 13,6 / 32,8 |
| Bạch Vân (BV) / 10 | 304,8 / 8,4 | 15.515 / 9.738 | 32,8 / 18,6 | 0,0 / 0,0 / 0,1 / 0,5 |
| Xích Nham (XN) / 15 | 404,8 / 12,2 | 35.762 / 19.647 | 44,6 / 25,1 | 0,0 / 0,0 / 0,0 / 0,0 |
| Huyền Tích (HT) / 20 | 438,7 / 17,3 | 0 / 26.746 | 52,4 / 28,7 | 0,0 / 0,0 / 0,5 / 1,1 |

Trong model cũ, early N=4 có bottleneck: Đồng khoảng 36%, Trúc Lv 8 khoảng 33% thời gian chờ; N = 2 tại Trúc Lv 8 dưới 1%. Trúc có ba cụm Sói Lv 4 thay hai cụm, giữ budget 13 slots; Lv 8 có bốn Sói Trúc + ba Ong eligible. Cần benchmark rotate/crowd/CC, không tự thêm Channel/Party hay công bố capacity bốn người. Khi cụm gần level bị chiếm, scheduler chọn quái khó hơn; idle thấp ở Bạch/Xích/Huyền không chứng minh người chơi sẽ nhận risk đó. Chỉ chọn ngang cấp có thể chờ lâu hơn bảng.

| BV / player Lv 10 / N = 4 respawn | Normal / Linh deaths / h | Wait% |
| --- | --- | ---: |
| 20 s | 1078,2 / 32,5 | 0,20 |
| 25 s | 1012,6 / 32,1 | 0,38 |
| 30 s | 951,2 / 31,7 | 1,13 |

25 s respawn vẫn là BASELINE/TUNABLE; không giảm chỉ từ toy model. 28 cụm/66 slots đã SUPERSEDED về mật độ final. Population mới còn OPEN; density/aggro/root deadlines/Q8 cap phải đo lại trên bố cục mới, không dùng throughput cũ để bảo đảm Vàng/h hoặc capacity. Q8 có thể chờ variant khác chết rồi slot respawn; không demote để demo nhanh. Threshold 20% trên một full-health life cho tối đa năm requesters đủ credit; nhiều hơn dùng waiting set/life tiếp theo, không spawn mỗi player. Boot/Return/disconnect không reroll Linh.

<a id="economy-analysis"></a>
<a id="loot-consumable-analysis"></a>
<a id="quest-progression"></a>
<a id="boss-availability"></a>

# 4. Trang bị, kinh tế, nhiệm vụ và hành trình

## Trang bị, cường hóa và chuyển giao

Giữ **18 dòng / 21 mẫu thường + Mộc Kiếm**; mốc mặc I không phải vũ khí Lv 1, vũ khí I Lv 5, tất cả II Lv 11, tất cả III Lv 17. Vũ khí giữ ATK 15/28/40; Kiếm thêm 0,5/1/1,5 điểm % Chí mạng, Cung thêm 10/20/30 Chính xác. Giày I/II/III thêm 1/2/3% tốc chạy cố định. HP/MP mới theo [GDD §6](1_HUYEN_LO_GDD.md#gear-economy) và probe hiện hành phía trên; ATK/DEF/ACC/EVA/Crit/tốc chạy, giá/nguồn và trần enhance giữ nguyên. HP/MP family là STRONG DIRECTION, exact values TUNABLE. Ong Lv 10 rơi II trước khi người chơi mặc ở Lv 11. Tinh Hoa I/II cố định đã chốt tại GDD; **GEAR-01 giữ trần/milestone/transfer đã chốt; phần retune HP/MP còn BASELINE/TUNABLE**, phải kiểm sức mạnh khi chơi.

**Kiểm biên Common +0:** Với hồ sơ cân bằng tại Lv 5/13/20 và quái thường cùng cấp (EVA theo GDD), Chí mạng Kiếm mới tăng kỳ vọng sát thương trực tiếp khoảng 0,24/0,48/0,73%; Chính xác Cung giảm tỷ lệ bị né khoảng 0,29/0,27/0,27 điểm %. Đây là khác biệt nhỏ trước mô phỏng kỹ năng/di chuyển; Giày tăng tốc chạy tuyệt đối 1/2/3 điểm % nhưng không đổi nhịp đòn hoặc hồi chiêu. Không dùng phép kiểm này để khẳng định TTK mới.

<a id="gear-upgrade-values"></a>

## Bảng tra đủ chỉ số 21 mẫu trang bị thường + Mộc Kiếm

Bảng này **tính từ luật GDD §6**, chỉ cho phẩm chất **Common**; không phải catalog hay luật thứ hai. Mỗi ô là chỉ số **của riêng món đồ**, trước khi cộng nền nhân vật, điểm thuộc tính và nội tại phái. Kiếm cùng bậc thêm Chí mạng cố định, Cung thêm Chính xác chịu phẩm chất nhưng không chịu hệ số cường hóa; Giày thêm tốc chạy cố định. Tinh Hoa cộng sau các chỉ số này; Chí mạng và tốc chạy hiển thị theo điểm phần trăm. Số lẻ là giá trị nội bộ; UI mới làm tròn. Tinh Hoa I đã cộng tại +4; Tinh Hoa II chỉ có ở bậc III +8. Dấu **—** là cấp vượt trần hoặc không được nâng; các phẩm chất khác dùng đúng công thức GDD.

| Bậc | Món | +0 | +1 | +2 | +3 | +4 | +5 | +6 | +7 | +8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Ngoại lệ | Mộc Kiếm Q3 | ATK 10 | — | — | — | — | — | — | — | — |
| I — Thanh Mộc | Thanh Mộc Kiếm | ATK 15 · MP 10 · Crit 0,5% | ATK 15,75 · MP 10,5 · Crit 0,5% | ATK 16,5 · MP 11 · Crit 0,5% | ATK 17,4 · MP 11,6 · Crit 0,5% | ATK 18,45 · MP 12,3 · Crit 1% | — | — | — | — |
| I — Thanh Mộc | Thanh Mộc Cung | ATK 15 · MP 10 · ACC 10 | ATK 15,75 · MP 10,5 · ACC 10 | ATK 16,5 · MP 11 · ACC 10 | ATK 17,4 · MP 11,6 · ACC 10 | ATK 18,45 · MP 12,3 · ACC 10 · Crit 0,5% | — | — | — | — |
| I — Thanh Mộc | Áo Thanh Mộc | HP 44 · DEF 4 | HP 46,2 · DEF 4,2 | HP 48,4 · DEF 4,4 | HP 51,04 · DEF 4,64 | HP 64,12 · DEF 4,92 | — | — | — | — |
| I — Thanh Mộc | Quần Thanh Mộc | HP 28 · DEF 3 | HP 29,4 · DEF 3,15 | HP 30,8 · DEF 3,3 | HP 32,48 · DEF 3,48 | HP 34,44 · DEF 4,69 | — | — | — | — |
| I — Thanh Mộc | Giày Thanh Mộc | HP 12 · DEF 2 · EVA 4 · Tốc chạy +1% | HP 12,6 · DEF 2,1 · EVA 6 · Tốc chạy +1% | HP 13,2 · DEF 2,2 · EVA 8 · Tốc chạy +1% | HP 13,92 · DEF 2,32 · EVA 10 · Tốc chạy +1% | HP 14,76 · DEF 2,46 · EVA 16 · Tốc chạy +1% | — | — | — | — |
| I — Thanh Mộc | Nhẫn Thanh Mộc | MP 8 · ACC 6 · Crit 1% | MP 8,4 · ACC 8 · Crit 1,2% | MP 8,8 · ACC 10 · Crit 1,4% | MP 9,28 · ACC 12 · Crit 1,6% | MP 9,84 · ACC 18 · Crit 1,8% | — | — | — | — |
| I — Thanh Mộc | Dây chuyền Thanh Mộc | MP 24 · EVA 5 | MP 25,2 · EVA 7 | MP 26,4 · EVA 9 | MP 27,84 · EVA 11 | MP 39,52 · EVA 13 | — | — | — | — |
| II — Vân Nham | Vân Nham Kiếm | ATK 28 · MP 18 · Crit 1% | ATK 29,4 · MP 18,9 · Crit 1% | ATK 30,8 · MP 19,8 · Crit 1% | ATK 32,48 · MP 20,88 · Crit 1% | ATK 34,44 · MP 22,14 · Crit 1,5% | ATK 36,96 · MP 23,76 · Crit 1,5% | ATK 39,2 · MP 25,2 · Crit 1,5% | — | — |
| II — Vân Nham | Vân Nham Cung | ATK 28 · MP 18 · ACC 20 | ATK 29,4 · MP 18,9 · ACC 20 | ATK 30,8 · MP 19,8 · ACC 20 | ATK 32,48 · MP 20,88 · ACC 20 | ATK 34,44 · MP 22,14 · ACC 20 · Crit 0,5% | ATK 36,96 · MP 23,76 · ACC 20 · Crit 0,5% | ATK 39,2 · MP 25,2 · ACC 20 · Crit 0,5% | — | — |
| II — Vân Nham | Áo Vân Nham | HP 99 · DEF 9 | HP 103,95 · DEF 9,45 | HP 108,9 · DEF 9,9 | HP 114,84 · DEF 10,44 | HP 131,77 · DEF 11,07 | HP 140,68 · DEF 11,88 | HP 148,6 · DEF 12,6 | — | — |
| II — Vân Nham | Quần Vân Nham | HP 61 · DEF 6 | HP 64,05 · DEF 6,3 | HP 67,1 · DEF 6,6 | HP 70,76 · DEF 6,96 | HP 75,03 · DEF 8,38 | HP 80,52 · DEF 8,92 | HP 85,4 · DEF 9,4 | — | — |
| II — Vân Nham | Giày Vân Nham | HP 28 · DEF 4 · EVA 8 · Tốc chạy +2% | HP 29,4 · DEF 4,2 · EVA 10 · Tốc chạy +2% | HP 30,8 · DEF 4,4 · EVA 12 · Tốc chạy +2% | HP 32,48 · DEF 4,64 · EVA 14 · Tốc chạy +2% | HP 34,44 · DEF 4,92 · EVA 20 · Tốc chạy +2% | HP 36,96 · DEF 5,28 · EVA 22 · Tốc chạy +2% | HP 39,2 · DEF 5,6 · EVA 24 · Tốc chạy +2% | — | — |
| II — Vân Nham | Nhẫn Vân Nham | MP 14 · ACC 10 · Crit 1,5% | MP 14,7 · ACC 12 · Crit 1,7% | MP 15,4 · ACC 14 · Crit 1,9% | MP 16,24 · ACC 16 · Crit 2,1% | MP 17,22 · ACC 22 · Crit 2,3% | MP 18,48 · ACC 24 · Crit 2,5% | MP 19,6 · ACC 26 · Crit 2,7% | — | — |
| II — Vân Nham | Dây chuyền Vân Nham | MP 44 · EVA 8 | MP 46,2 · EVA 10 | MP 48,4 · EVA 12 | MP 51,04 · EVA 14 | MP 64,12 · EVA 16 | MP 68,08 · EVA 18 | MP 71,6 · EVA 20 | — | — |
| III — Huyền Ấn | Huyền Ấn Kiếm | ATK 40 · MP 26 · Crit 1,5% | ATK 42 · MP 27,3 · Crit 1,5% | ATK 44 · MP 28,6 · Crit 1,5% | ATK 46,4 · MP 30,16 · Crit 1,5% | ATK 49,2 · MP 31,98 · Crit 2% | ATK 52,8 · MP 34,32 · Crit 2% | ATK 56 · MP 36,4 · Crit 2% | ATK 59,6 · MP 38,74 · Crit 2% | ATK 63,6 · MP 41,34 · Crit 2% · ACC 6 |
| III — Huyền Ấn | Huyền Ấn Cung | ATK 40 · MP 26 · ACC 30 | ATK 42 · MP 27,3 · ACC 30 | ATK 44 · MP 28,6 · ACC 30 | ATK 46,4 · MP 30,16 · ACC 30 | ATK 49,2 · MP 31,98 · ACC 30 · Crit 0,5% | ATK 52,8 · MP 34,32 · ACC 30 · Crit 0,5% | ATK 56 · MP 36,4 · ACC 30 · Crit 0,5% | ATK 59,6 · MP 38,74 · ACC 30 · Crit 0,5% | ATK 63,6 · MP 41,34 · ACC 36 · Crit 0,5% |
| III — Huyền Ấn | Áo Huyền Ấn | HP 138 · DEF 15 | HP 144,9 · DEF 15,75 | HP 151,8 · DEF 16,5 | HP 160,08 · DEF 17,4 | HP 179,74 · DEF 18,45 | HP 192,16 · DEF 19,8 | HP 203,2 · DEF 21 | HP 215,62 · DEF 22,35 | HP 229,42 · DEF 25,85 |
| III — Huyền Ấn | Quần Huyền Ấn | HP 88 · DEF 9 | HP 92,4 · DEF 9,45 | HP 96,8 · DEF 9,9 | HP 102,08 · DEF 10,44 | HP 108,24 · DEF 12,07 | HP 116,16 · DEF 12,88 | HP 123,2 · DEF 13,6 | HP 131,12 · DEF 14,41 | HP 154,92 · DEF 15,31 |
| III — Huyền Ấn | Giày Huyền Ấn | HP 40 · DEF 6 · EVA 12 · Tốc chạy +3% | HP 42 · DEF 6,3 · EVA 14 · Tốc chạy +3% | HP 44 · DEF 6,6 · EVA 16 · Tốc chạy +3% | HP 46,4 · DEF 6,96 · EVA 18 · Tốc chạy +3% | HP 49,2 · DEF 7,38 · EVA 24 · Tốc chạy +3% | HP 52,8 · DEF 7,92 · EVA 26 · Tốc chạy +3% | HP 56 · DEF 8,4 · EVA 28 · Tốc chạy +3% | HP 59,6 · DEF 8,94 · EVA 30 · Tốc chạy +3% | HP 63,6 · DEF 10,54 · EVA 32 · Tốc chạy +3% |
| III — Huyền Ấn | Nhẫn Huyền Ấn | MP 22 · ACC 15 · Crit 2% | MP 23,1 · ACC 17 · Crit 2,2% | MP 24,2 · ACC 19 · Crit 2,4% | MP 25,52 · ACC 21 · Crit 2,6% | MP 27,06 · ACC 27 · Crit 2,8% | MP 29,04 · ACC 29 · Crit 3% | MP 30,8 · ACC 31 · Crit 3,2% | MP 32,78 · ACC 33 · Crit 3,4% | MP 34,98 · ACC 35 · Crit 4,1% |
| III — Huyền Ấn | Dây chuyền Huyền Ấn | MP 66 · EVA 12 | MP 69,3 · EVA 14 | MP 72,6 · EVA 16 | MP 76,56 · EVA 18 | MP 91,18 · EVA 20 | MP 97,12 · EVA 22 | MP 102,4 · EVA 24 | MP 108,34 · EVA 26 | MP 114,94 · EVA 32 |

**Kỳ vọng lũy kế từ +0** (mỗi bước hình học độc lập; thất bại tiêu chi phí nhưng giữ cấp). Cột Vàng quy đổi dùng giả định mua toàn bộ đá thiếu ở shop 800 Vàng/viên; không cộng đồng thời với Vàng thuần. Số thập phân là kỳ vọng, không bảo đảm số lần cụ thể.

| Đạt cấp | Tổng lần thử kỳ vọng | Vàng thuần kỳ vọng | Tinh Thạch kỳ vọng | Vàng nếu mua toàn bộ đá |
| --- | ---: | ---: | ---: | ---: |
| +1 | 1,00 | 100 | 1,00 | 900 |
| +2 | 2,11 | 322 | 2,11 | 2.011 |
| +3 | 3,36 | 760 | 4,61 | 4.449 |
| +4 — Tinh Hoa I | 4,90 | 1.606 | 9,23 | 8.987 |
| +5 | 7,12 | 3.495 | 20,34 | 19.765 |
| +6 — trần II | 9,98 | 7.209 | 40,34 | 39.479 |
| +7 | 13,98 | 15.209 | 80,34 | 79.479 |
| +8 — Tinh Hoa II | 20,65 | 36.542 | 173,67 | 175.479 |

Một vũ khí I +4 cần bình quân 9,23 đá; II +6 từ +0 cần 40,34 đá, nhưng **I +4 → II +4** chỉ cần kỳ vọng thêm 31,11 đá cho hai bước +5/+6 và chi phí chuyển 2 đá. III +8 cần thêm 133,33 đá từ +6: mục tiêu sau truyện, không bắt để đánh Q12. Drop đá thường 8% = 12,5 kill/đá nếu chỉ tính quái thường; 640 kill cho kỳ vọng 51,2 đá, thêm khoảng 23 Linh Biến cho ~74 đá sinh ra trước nhặt/tiêu. Mua đá 800 Vàng cho phép bù thiếu nhưng +8 một món từ +0 vượt ngân sách Gold/đá chính tuyến. Boss có 5–8 đá trong một pile chung, **không** nhân theo số người tham gia. Không cần tăng drop hay sửa giá đá chỉ để biến +8 thành điều kiện story.

**Một phép so cùng ô vũ khí** (chỉ số của món, làm tròn hai chữ số; chưa cộng nền nhân vật/nội tại):

| Món | ATK | Chí mạng Kiếm / Chính xác Cung (riêng vũ khí) | Tinh Hoa / ý nghĩa |
| --- | ---: | ---: | --- |
| Rare II +6 | 45,47 | 1,5% / 23,2 | Tinh Hoa I +0,5 điểm % chí mạng; đáng giữ tới III |
| Common III +0 | 40,00 | 1,5% / 30 | Món mới chưa vượt món II đã đầu tư |
| Rare III +0 | 46,40 | 1,5% / 34,8 | Nhỉnh hơn Rare II +6 về ATK, chưa có Tinh Hoa |
| Common III +4 | 49,20 | 2% / 30 | Vượt Rare II +6 sau đầu tư +4 |
| Rare III +4 | 57,07 | 2% / 34,8 | Giữ phẩm chất Rare và mở Tinh Hoa I; chuyển từ Rare II +6 giữ +6 còn mạnh hơn |
| Rare III +8 | 73,78 | 2% / 40,8 | Thêm Tinh Hoa II +6 ACC; trần săn sau truyện |
| Epic III +8 | 79,50 | 2% / 43,5 | Cao hơn Rare III +8 khoảng 7,8% ATK món, không nhân cả nhân vật |

Rare II +6 Áo mới là **170,776 HP / 14,616 DEF**, so Common III +0 **138 / 15** và Common III +4 **179,74 / 18,45**. Nhẫn Rare II +6 có **27,6 ACC / 2,7% chí mạng**, còn Dây chuyền Rare II +6 **81,456 MP / 21,28 EVA**; vì thế không tự động thay toàn bộ II bằng Common III +0. Trang bị phòng thủ/phụ kiện vẫn có giá trị: bớt bình máu, tăng ổn định trúng/né và Linh lực, dù vũ khí quyết định phần lớn tốc độ hạ quái.

**LEGACY phần HP/TTK của đoạn này:** ở Lv 20 cân bằng trước migration, toàn bộ Rare III +6 → +8 đổi ATK nhân vật **116,6 → 125,4** (+7,5%) và Máu Kiếm **929 → 996** (+7,2%) sau nội tại; thời gian solo Cổ Vệ **5,28 → 4,62 s** là mô phỏng trước khi phân hóa Chí mạng/Chính xác vũ khí, cần chạy lại. Toàn bộ Epic III +8 so Common III +0 đổi ATK **91,6 → 131,1**; đó là biên sức mạnh sau truyện. Tinh Hoa II và flat của Giày/Nhẫn/Dây chuyền đã tính trong các mốc +8, không nhân lại theo phẩm chất.

**Chuyển giao giữ nguyên cấp, không nhân đồ:** cùng bậc tốn 800 Vàng, không đá; lên đúng một bậc tốn 500 Vàng +2 đá. Đồ nguồn mất, đồ đích giữ template/phẩm chất/instance; từ chối nếu đích không tăng. Ví dụ Rare II +6 → Epic II +0 thành Epic II +6, tiết kiệm việc đập lại +0→+6 nhưng mất giá bán Rare II (vũ khí Rare II 324 Vàng); opportunity tối thiểu **1.124 Vàng**. II +6 → III +0 tốn 500+2 đá và mất giá bán nguồn, opportunity **2.424 Vàng** nếu đá mua 800; rẻ hơn tự đập III +0→+6 ước 39.479 Vàng gồm đá mua. So sánh chỉ hợp khi đã sở hữu món nguồn; sunk cost cường hóa nguồn không được bỏ qua để gọi chuyển giao là nguồn tạo cấp miễn phí. I→III trực tiếp bị cấm. Không có vòng lặp bán/transfer: mỗi lệnh tiêu một source, target không sinh bản sao, giá bán không cộng tiền cường hóa, no-gain bị chặn; receipt chống replay.

**Đường đầu tư kỳ vọng:** Lv 5 vũ khí I +0; Lv 8 +2; Lv 10 +3; Lv 11 chọn vũ khí II hoặc giữ I +4 tới lúc đủ đá; Lv 13 vũ khí II +4 và nâng Áo nếu chịu đòn nhiều; Lv 15 II +4; Lv 17 trước Q11 II +4/+5, sau Q11 chuyển sang vũ khí Rare III rồi học đại chiêu; Lv 20 chính tuyến thường chỉ vũ khí III khoảng +4..+6, các món khác +0..+2. Đó là hồ sơ kiểm, không là requirement. Band I +4 đáng làm nếu chơi lâu trước II vì Tinh Hoa I và chuyển nguyên +4; II +6 đáng làm nếu sở hữu món tốt và muốn sang III +6. Đập toàn set +8 trước Q12 không hợp supply.

## Ngân sách farm và ví dụ kiểm chứng khi nhiều người đánh

Cận trên bán toàn bộ đồ rơi giả định bán trang bị/nguyên liệu/bình/đá trước khi dùng hoặc nhặt hụt; không đồng thời bán và dùng. Food I/II/III tốn **900/2.400/4.200 Vàng/h** nếu hiệu lực liên tục. Theo quái cố định Lv 2/4/8/10/13/16/20, Vàng trực tiếp + cận trên bán đồ khoảng **29,24/34,54/45,16/55,38/63,78/73,88/95,53 Vàng/quái thường**. Ngoài abs level gap 3, regular EXP/Gold/loot/Journey bằng 0. Quest/supply/evidence active step vẫn hoạt động ở level thấp.

Necklace III sell tăng **200 → 225 Vàng**, ngang Boots/Ring III; tổng sell sáu Common III tăng **1.725 → 1.750**. Giá mua II, sell II, rates và ATK không đổi; HP/MP đã retune, nên transfer Rare II→III không đổi chi phí; normal Lv 20 vendor-all upper tăng khoảng **0,24 Vàng/kill**.

**DERIVED — giá/drop không đổi:** Boss expected sell-all pile: **1.000** Thỏi + **303,33** gear + **1.300** Stone + **150** Potion + **75** phù = **2.828,33 Vàng/world death** trước pickup/use; direct Gold/EXP = 0. Respawn 15 phút + fight 90–150 s tạo Thỏi khoảng 3.429–3.636 Vàng/h/world. Q12 turn-in 1.000 riêng từng character, không nhân vật phẩm world theo N người.

**Ví dụ kiểm chứng đóng góp:** mob Lv 4 HP107/EXP22/Gold15; Lv 20 gây 90, Lv 5 gây17. Người cao lệch level ≥4 nhận 0; người thấp nhận floor(22×17/107)=3 EXP, floor(15×17/107)=2 Gold, không nhận cả pool. TopDamage quá gap không tạo regular loot set. Người thấp 15,9% chưa đạt quest credit threshold 20%. Cùng level 70/37 damage nhận 14/7 EXP và 9/5 Gold; lẻ không redistribute. AFK damage0 không credit; cùng MapId / bán kính 8u / còn sống / đã gây sát thương trong 10s vẫn kiểm từng recipient. Quest chỉ active-step; Boss ≥10% và corpse area là predicate riêng. Không có Party. FFA chỉ nhặt, không sinh EXP/Gold/quest. Level lúc mob chết là bản chụp cho người có đóng góp, nên lên cấp ngay sau kill không làm mất quyền pickup; người tới sau xét level hiện tại.

## Nhiệm vụ, hành trình và Boss

Giữ **Q1–Q12**, Q9 tùy chọn; không thêm nhiệm vụ để lấp khoảng farm. Đối chiếu đúng SpawnSlot: Q4 một Nấm DS2/loot/equip/sell; Q5 năm Sói DS3–DS6; Q8 bốn Sói TA4+TA6 rồi Linh Biến `TA4.slot1`; Q10 sáu Đạo Tặc XN1–XN3, vật chứng ở kill thứ 2/4/6; Q11 3/3/4 Thạch XN4/5/6; Q12 sáu Cổ Vệ HT4+HT5. Vật chứng nhiệm vụ theo từng người, không RNG vô hạn. Q11 hoàn thành mới cho vũ khí Rare III và đại chiêu; không dùng chúng để tính độ khó Q11.

**LEGACY SIMULATION — cadence/density/HP-MP/NPC route cũ:** mô hình hành trình 16 hạt giống/phái/kịch bản, Q9 bỏ qua, Boss/wait Q12 tính riêng. Tính thời gian đánh, travel cơ bản, turn-in, Food, Linh lực và chi phí cường hóa; chưa mô phỏng sát thương nhận, death, bag đầy, do dự UI, latency và nhặt hụt. Kết quả chỉ để phát hiện mâu thuẫn lớn, không nghiệm thu mục tiêu **150–240 phút**.

| Phái | Đầu tư | Phút tới Lv 20 | Vàng còn | Đá còn | Lần thử cường hóa | Chuyển giao |
| --- | --- | ---: | ---: | ---: | ---: | ---: |
| Kiếm | Ít cường hóa | 119,0 | 21.025 | 72,9 | 0,0 | 0,0 |
| Kiếm | Hợp lý: vũ khí +2→+4, áo tới +2 | 115,5 | 17.050 | 48,3 | 16,4 | 0,0 |
| Kiếm | Ưu tiên vũ khí tới +6, không chuyển | 114,5 | 9.646 | 10,9 | 20,5 | 0,0 |
| Kiếm | Ưu tiên vũ khí, chuyển I→II→III | 114,7 | 12.393 | 30,5 | 11,1 | 1,8 |
| Cung | Ít cường hóa | 130,7 | 20.337 | 76,4 | 0,0 | 0,0 |
| Cung | Hợp lý: vũ khí +2→+4, áo tới +2 | 126,7 | 16.452 | 51,8 | 16,1 | 0,0 |
| Cung | Ưu tiên vũ khí tới +6, không chuyển | 125,3 | 9.352 | 9,2 | 20,4 | 0,0 |
| Cung | Ưu tiên vũ khí, chuyển I→II→III | 124,5 | 13.433 | 30,9 | 10,2 | 1,8 |

Chuyển giao bảo toàn cấp giảm trung bình khoảng mười lần thử và 20 đá ở kịch bản ưu tiên vũ khí; thời gian tới Lv 20 gần bằng nhau trong mô hình vì source/shop/roll khác nhau. Không suy rằng mọi người phải chuyển hay nâng +6. Vàng cuối dương trong tất cả runs; chưa trừ HP Potion/death và không cộng tự bán toàn bộ đồ rơi. Không tăng NeedEXP từ mô hình thiếu geometry. QUEST-03 vẫn cần hành trình chơi thật; nếu đoạn Lv 17–20 quá nhanh, thử riêng +10/+15/+20% NeedEXP cuối game.

**LEGACY SIMULATION — Boss projection trước migration:** HP 32.000, DEF 25, EVA 60 (baseline GDD); hai người cân bằng Kiếm/Cung cùng đánh, cộng sát thương trung bình độc lập, chưa mô phỏng đồng thời né đòn/chết/threat. Tỷ lệ thời gian ra đòn hữu hiệu là giả định 50/65/75%; không auto-scale theo số người.

| Trang bị / người | Hai người, 50% / 65% / 75% ra đòn | Ba người, 65% | Bốn người, 65% |
| --- | ---: | ---: | ---: |
| Common III +0 | 151 / 116 / 101 s | 78 s | 58 s |
| Chính tuyến: Rare III vũ khí +6, đồ hỗn hợp | 119 / 91 / 79 s | 61 s | 46 s |
| Rare III +6 toàn bộ (đầu tư vừa) | 118 / 90 / 78 s | 60 s | 45 s |
| Rare III +8 toàn bộ (săn sau truyện) | 109 / 84 / 72 s | 56 s | 42 s |
| Epic III +8 toàn bộ (biên trần) | 104 / 80 / 69 s | 53 s | 40 s |

Hai người chính tuyến ở khoảng **50–65% thời gian ra đòn hữu hiệu** đạt 91–119 s, đúng mục tiêu 90–150 s trong mô hình cũ, không chứng minh baseline 2026-10-06 đạt. 75% cho 79 s là biên thuận lợi, cần cảnh né/telegraph thật để xem có quá nhanh. Bốn người endgame ~40 s ở 65% là hệ quả tự nhiên của cộng DPS, không lý do thêm auto-scale Boss; capacity/độ khó nhiều người phải đo. +8 làm nhanh hơn nhưng không vài giây và không cần cho Q12. Boss ATK 160, ba vùng Nham Thạch Rơi, lịch một action/các cooldown phải kiểm scene; mô hình không chứng minh đòn luôn né được. Demo HP 12.800/respawn 60 s chỉ là override nghiệm thu, không dùng làm cân bằng release.




Hồ sơ feedback và lỗi của mock được giữ tại [Roadmap](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#prototype-feedback-history); các mô phỏng balance dưới đây không nghiệm thu layout của bản mẫu.

<a id="review-decisions"></a>
<a id="open-decisions"></a>

# 5. Quyết định đã chốt và cổng kiểm khi triển khai

**Gameplay đã chốt:** GEAR-01 (trần +4/+6/+8, hai Tinh Hoa, chuyển giao); CONS-01 (hai hồi chiêu bình riêng); BOSS-02 (DEF25/ACC140/EVA60, ba vùng đá, một action/lịch ưu tiên). COOP-01, QUEST-02, BOSS-03, BOSS-01 và NAR-01 cũng đã chốt. **PVP-01 :** cược 1.000–10.000 bước 1.000, escrow cả hai trước MatchId, Food/Bình hợp lệ với quota 3+3, 120 s cả hai sống DRAW, fee/refund qua Spring receipt. **SAVE-01 :** PostgreSQL giữ tiến trình và MapId/HP/MP checkpoint; resume phiên còn trong RAM hoặc spawn SafeAnchor từ checkpoint. Luật loot/payout/EXP giữ; chỉ số/trang bị/nhịp đòn mới được tính lại trong [probe hiện hành](#current-balance-probe). Mô hình density/journey/status/PvP thực tế cần chạy lại; TTK thuận lợi mới chưa có geometry/AI/network.

NAR-01: Đã tinh chỉnh Narrative theo hướng mở, gợi cảm giác tò mò và gỡ mâu thuẫn bối cảnh Xích Nham mà không tăng scope. Q9 tùy chọn, trang bị 18 dòng, bí kíp, không Party P0 và vòng Boss chung cũng đã có luật; không mở lại vì chưa chơi thử.

| ID / trạng thái | Điều cần đo hoặc làm | Baseline hiện tại | Điều kiện xem lại |
| --- | --- | --- | --- |
| BAL-01 — PLAYTEST | Giá trị bốn thuộc tính và build cực đoan | Bảng phân điểm/chỉ số §2, 95 điểm | Một build làm Q10/Q11 không thể qua dù dùng cơ chế bình thường, hoặc PvP có kết quả lệch quá xa |
| BAL-02 — PLAYTEST | Hồi phục và chi phí Food/Bình | Food III mỗi 2 s +4% HP/+2,5% MP; hai bình riêng 8 s | Trận farm phải đứng chờ Linh lực liên tục hoặc Vàng âm sau route hợp lệ |
| PHY-01 — PLAYTEST | Collider, platform, nhiều người | Bounds GDD, drop-through theo từng actor | Trúng đòn/đi xuyên sàn sai hoặc người khác làm đổi collision |
| CC-01 — PLAYTEST | Đóng Băng/Làm Chậm/Bỏng khi nhiều người | Cửa miễn Đóng Băng 3 s; Boss clock ×0,75, không stack | Boss mất khả năng ra đòn, người chơi không đọc được hiệu ứng, hoặc overlap gây unfair hit |
| SCOPE-01 — STRONG DIRECTION / TUNABLE | Re-author nhiều pocket độc lập, đường nhánh/cao độ và contention | 28/66 là LEGACY seed; totals OPEN, respawn 25 s/Linh 5%/cap 1 còn baseline | 2 người thiếu quái rõ hoặc 3–4 người chờ nhiều; benchmark trước claim capacity |
| GEAR-01 — LOCKED SEMANTICS / TUNABLE STATS | Chạy lại TTK, hit/crit, di chuyển giữa cụm, hành trình và Boss với Chí mạng Kiếm / Chính xác Cung / tốc chạy Giày; đo cảm giác +4/+6/+8 và preview | GDD §6, bảng kinh tế §4, hai Client + Dedicated Server playtest | Gear II/III, sức mạnh hai phái hoặc Tinh Hoa làm Boss/PvP/kinh tế lệch khi chơi thật |
| CONS-01 — BASELINE ĐÃ CHỐT | Thử nhầm phím/bình/refresh Food | HP và MP hồi chiêu riêng; tier đủ bù nhỏ nhất, Food thay cũ | Người chơi thường xuyên phí bình hoặc tutorial Q6 kẹt |
| SAVE-01 — ĐÃ CHỐT | Checkpoint 30 s + portal/logout/death/revive/PvP; resume 15 s nếu phiên còn; mất phiên dùng SafeAnchor và HP/MP đã commit | Technical §5–7, sequence/generation, safe-zone coordinate validation | Checkpoint cũ đè mới, HP=0 hồi sinh miễn phí, load Arena hoặc mất Potion nhưng hồi máu không được checkpoint |
| PVP-01 — ĐÃ CHỐT | Escrow hai người trước MatchId; 10 mức cược; Food tick/Bình quota; outcome và payout idempotent | GDD §8, Technical §4/§6, bảng arithmetic ở trên | HELD/ACTIVE mắc kẹt tiền; hết 120 s vẫn so HP; FORFEIT sai thời điểm; result retry trả Vàng hai lần |
| BOSS-02 — BASELINE ĐÃ CHỐT | Độ rộng vùng/nhịp báo trước đòn | 32.000 HP, DEF25/ACC140/EVA60, ba vùng đá, một action | Không thể né bằng kỹ năng di chuyển thường, overlap khó đọc, thời gian 2 người lệch xa 90–150 s |
| ART-01 — PLAYTEST | Đường đạn, aim, thời điểm hit, hình nhân vật | GDD/Technical art contract | Collider/hình lệch hoặc cảm giác chém/bắn khó đọc |
| LOOT-01 — PLAYTEST | Đá/trang bị sinh ra so lượng nhặt và sink | 8% đá thường, giá shop 800, §3–4 | Người chơi hợp level không đủ nguồn cho +4, hoặc +8 quá dễ trước story |
| QUEST-03 — PLAYTEST | Full Q1–Q12, NPC mới, hồi phục và Dev Mode cách ly acceptance | Mô hình 115–131 phút là LEGACY; mục tiêu 150–240 phút chưa nghiệm thu | Playable journey vẫn quá nhanh/chậm sau tính đi lại, chết, UI và multiplayer |
| TECH-01 — SPIKE | Login → Character Select → one-time ticket → dedicated join; lease/duplicate session, internal service credential và backend outage | Technical §5–6: Spring xác minh account/character; Game Server xác minh ticket, một writer/character | Spike hai Client + backend/DB thật, thử duplicate join, consumed ticket, timeout trước/sau commit và server crash |
| INPUT-01 — LUẬT ĐÃ DUYỆT / FEEL CÒN MỞ | Input/focus/pending được chủ dự án duyệt; [GDD §3](1_HUYEN_LO_GDD.md#focus-input) là owner | Contract [Technical — input](2_HUYEN_LO_TECHNICAL.md#input-contract); kiểm hành vi một luồng, không flags/A/B | Rollover bàn phím thật, cảm giác ở tốc độ thường và envelope còn cần review |
| CMB-01 — STRONG DIRECTION / TUNABLE | Basic Tân Lữ giữ; S1/S2 nhanh, S3 signature theo probe mới | [Onboarding evidence](#novice-onboarding-evidence), số fixture cũ không thay fresh route | TTK Q3–Q5 và feel còn giới hạn; không tune EXP/HP để ép số |
| MOBAI-01 — NGUYÊN TẮC ĐÃ DUYỆT | [GDD — melee crowd](1_HUYEN_LO_GDD.md#melee-crowd) | [Balance finding](#novice-onboarding-evidence); không formation/token/shoving hoặc range/speed/interval/HP mới | Readability/deadband/offset cần xem bằng người thật; không chỉnh damage để khớp một ngưỡng metric |

<a id="input-combat-mob-proposals"></a>
<a id="novice-onboarding-evidence"></a>

## Kiểm chứng onboarding và rủi ro cân bằng

**DERIVED:** nhịp basic Tân Lữ đã duyệt cho occupancy lock 0,32 / 0,70 và phần còn lại 54,29% chu kỳ, so với 74% ở control cũ 1,00 / 0,26. Phần còn lại không đồng nghĩa mất điều khiển: locomotion/gravity vẫn hoạt động. Đây là phép tính, không bằng chứng cảm giác tay.

**EXISTING FIXTURE, không đo lại:** mẫu trước đã ghi Lv 3/ba Dummy 9,78 → 6,42 s và Lv 4/một Sói 4,96 → 3,98 s khi bật nhóm thay đổi. Nguồn và điều kiện nằm trong [CHANGELOG](../../prototypes/VS1_EndToEnd/CHANGELOG.md#phase-e--kết-quả-ab-và-giới-hạn). Những số này giúp đánh giá Q3 và Q5, nhưng gộp input/AI/cadence nên không chứng minh riêng tác động CD, không là TTK của build một-hành-vi mới. Q4 Nấm/loot/equip/sell chưa có thời gian fresh tương ứng; Q3–Q5 cần route chức năng mới. Không lấy mốc Q4 Sói của mô hình cũ thay Q4 Nấm hiện hành; reward/count đọc [GDD §5](1_HUYEN_LO_GDD.md#quests-story).

**BALANCE FINDING:** cùng mẫu cũ ba Sói, mất 131 → 166 HP/10 s (+26,7%) khi bật toàn bộ nhóm; riêng AI là 142 HP (+8,4%), riêng input 122 HP (−6,9%). Phụ thuộc target/life/RNG và nhiều thay đổi gộp, chưa thể gán chênh lệch cho một luật chọn cánh. Ghi rủi ro áp lực nhận damage; không đổi HP/range/speed/interval, không thêm safe window để ép kết quả. Không chạy lại harness hoặc bản thử Unity trong lượt tài liệu này; mô phỏng Python hiện hành ở đầu §2 là loại bằng chứng riêng.

**Các câu hỏi thật còn mở:**

- A07 / SCOPE-01: xác nhận wording và authoring của Dummy training actor ngoài “bảy identity/sáu rig”; không tự cộng một combat identity hay rig mới.
- A07 / SAVE-01: Học Viện là khu an toàn nhưng có sân Dummy không gây damage; cần định nghĩa nhất quán safe-zone/checkpoint với training yard trước production.
- QUEST-03 / BAL-02: chi phí chạy về làng/turn-in/mua đồ khi chưa có fast travel; không thêm teleport tiện ích từ ước lượng hành trình.
- INPUT-01: latest buffer giữ 0,18 s BASELINE/TUNABLE; readiness phải nằm trong cửa sổ đang dùng. Tick/latency, duration và giới hạn approach phải kiểm trước production, không tự gọi 0,18 là exact lock.
- Feel/rollover thực trên desktop/laptop, mũi tên + Select/Execute và đề xuất phím dùng đồ chưa được người thật nghiệm thu. Functional route/test không thay kết luận này.

Luật hiện hành ở GDD; các phương án keyboard B/C và số A/B cũ chỉ giữ trong [lịch sử](../../prototypes/VS1_EndToEnd/CHANGELOG.md#single-behavior-history-3). Cấu trúc input đã LOCKED; physical Execute/Interact/items/menu, shell, envelopes và baseline cadence vẫn OPEN/TUNABLE, không giữ điều khiển cũ làm requirement.

TECH-01/SAVE-01 và A15 còn cần chốt thứ tự Potion–hit–checkpoint, definite reject khác timeout, deathUtc/cap release ở terminal-pending và neo clock phiên↔UTC. [Các chuỗi probe cụ thể](2_HUYEN_LO_TECHNICAL.md#pending-ordering-probes) bổ sung acceptance của spike, chưa bằng chứng runtime hoặc approval schema. Các gap này không chặn local Kiếm bằng RAM fixture, nhưng phải giải trước G-D/reliability.

**Rủi ro kiến trúc :** auth/ticket/lease, checkpoint ordering/validation, reconnect grace, escrow orphan recovery, transaction N recipients, backend downtime và dedicated build/headless physics cần integration tests. Mốc giờ player-host/JSON cũ không còn dùng để cam kết lịch; spike đầu đo integration/latency, sau đó Technical mới đặt lại ngân sách. Quest rewards/nguồn, EXP, giá/drop và Boss giữ; HP/MP trang bị/cadence/HP Cung dùng probe mới, world density chưa chốt totals; các luật PvP/recovery trên là user lock, không Open Decision.

[Review Art](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#art-review) tách nguyên tắc đủ dùng và con số chưa duyệt; thứ tự probe/mở production được quản lý tại [Roadmap](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#phase-gates).

Các dòng đã có baseline không chặn việc bắt đầu code; ca PLAYTEST/SPIKE ghi rõ phải đo gì. Không tăng NeedEXP, HP quái hoặc tạo hệ thống mới từ mô hình thiếu cảnh thật.


### Quyết định migration còn OPEN / TUNABLE

| Owner gate | Trạng thái và việc phải chốt |
| --- | --- |
| INPUT-01 | Execute/Interact/QuickHP/QuickMP/Food/menu physical keys và RPG shell còn OPEN; E/F/4/5/R/I chỉ proposal. Search/retention/vertical/cycle ordering/approach budget và buffer duration TUNABLE. |
| CMB-01 / BAL-02 | CD0,60/0,90/6, MP2/3/16, power S2 Cung và animation timing là PROBE BASELINE/TUNABLE; test S2 repeated và manual weave, MP thiếu/no Food/gear-lag. |
| BAL-01 / GEAR-01 | Cung+8 sau ClassChosenLevel, exact HP/MP trang bị còn TUNABLE; VIT chung/6slot đã giữ. Kiểm late class/evaluator/rarity/+4/+8/PvP/Boss. |
| MOBAI-01 / SCOPE-01 | Hybrid count/identity OPEN; grace/Return regen/invuln/speed/targetability, crowd offsets và Home/Walk bounds còn TUNABLE/OPEN. Không khóa 3/1/0 hoặc dùng quái anti-Bow. |
| SCOPE-01 / PHY-01 | Population/pocket totals từng map còn OPEN; 28/66 là lịch sử; validate nhiều tầng/nhiều group độc lập và quest anchors. Solid trực giao/no natural one-way/no ladder đã LOCKED. |
| ART-01 / QUEST-03 | NPC tọa độ, camera/tile/module/VFX counts và ý nghĩa 26 frame interpretation OPEN; roster bảy NPC/quest ownership mới đã xác định. Dev Mode không thay fresh-run Q1–Q12 hai phái. |

<a id="art-open-decisions"></a>

## Sổ quyết định Art và các dependency — alias A01–A17

Đây là **nơi duy nhất giữ trạng thái quyết định** cho A01–A17, giữ options/trade-off lịch sử từ Art §25; recommendation/trạng thái đã reconcile theo lock mới. Axx là alias lịch sử, gắn vào gate sẵn có dưới đây; không tạo một hệ luật cạnh tranh với BAL/ART/PHY/TECH. Trong bảng options cũ, `§10` chỉ [evidence timing đã chuyển](#art-combat-timing-evidence). P01–P15 là **ca thử** trong [Art §24](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#art-validation), không decision ID hay phase roadmap. Tất cả ca Unity vẫn **CHƯA CHẠY**.

| ID / vấn đề | Options và trade-off | Recommendation hiện tại | Cách chốt |
| --- | --- | --- | --- |
| A01 Ý nghĩa 26 frame | 26 ảnh tổng: chặt budget nhưng có thể thiếu Bow; 26 ô/profile dùng lại ảnh: tăng vài upper pose; hai bộ đầy đủ: dễ author nhưng dư locomotion | 26 ô baseline/profile, pose map chung, upper body riêng theo class; **cần duyệt lại diễn giải user-lock** | Duyệt design + P01/P03, không tự tuyên bố lock đã đổi |
| A02 Technique/weapon poses | Raster toàn bộ: sạch nhưng tốn; socket/skeletal thuần: ít ảnh nhưng rủi ro outline/khớp; hybrid: setup vừa và pose chính sạch | Hybrid body raster/weapon socket, góc sửa khi cần; 13–25 weapon outcomes chỉ là kịch bản | P01–P03 và chi phí P15 |
| A03 Boots | Giữ stat-only: không cost world; bỏ: phương án lịch sử đã SUPERSEDED, đổi economy/balance; footwear slot có visual thật: thêm layer | Giữ stat-only P0, LowerBody chứa footwear mỹ thuật | Kiểm đủ sáu ô, icon và evaluator HP/MP; không mở lại phương án bỏ Boots |
| A04 Skill/VFX concept | Nét Mạch Ấn, kiếm khí/băng hoặc linh ảnh: khác sắc thái/cost; linh ảnh lớn dễ nhầm entity | Motif ấm/lạnh, sáu skill bindings, reuse visual primitives; không tự nhân sáu family | P03/P04/P14; concept cụ thể chưa khóa |
| A05 Multi-target Cung | Lịch sử flight/spawn lệch được thay; mất projectile dodge sau resolve | APPROVED logical Spread batch +0,12 s ABC/ABA/AAA; Hàn +0,18 s, visual không damage | P04/P12/P13; Bow AAA role/sustain và visual timing cần đo |
| A06 Normal/timing/CD/proc | Lịch sử 0,80/0,90 s class Normal không còn; action/CD khác pose duration | Dùng power/MP/CD probe mới sáu skills TUNABLE; CD độc lập/common lock, Novice basic trước class | P03/P13; rerun rotation/resource, không dùng legacy occupancy |
| A07 Dummy | HP 60/25 s: rẻ nhưng chờ; pool thêm/respawn nhanh: ít chờ; HP lớn/scaling: test dài hơn nhưng phức tạp hoặc hại Q3 | Một prefab/yard, tối thiểu 3 Dummy cùng lúc theo Q3; số thêm và respawn 3–5 s chỉ đề xuất, không DPS Meter/scaling P0 | P07; đổi HP/timer hoặc số thêm phải theo owner GDD |
| A08 Corpse/flying loot | Fade nhanh: sạch; hold lâu: thấy kill nhưng clutter; rơi visual: tự nhiên; tan tại chỗ: rẻ; chiếu loot xuống nền: reachable nhưng cần luật server | Ground hold 0,5–1,5 s/fade 0,3–0,5 s; Ong rơi visual/fallback tan; loot server chọn điểm hợp lệ | P06/P09; corpse art và loot gameplay quyết riêng |
| A09 Terrain/building | Cell 16/32/64 px; tile-only hoặc mini kit/full asset; ít variant giảm cost nhưng lặp texture | Thử 32 px, 17 semantic + 4 cosmetic/họ; kit cho motif lặp, full asset cho landmark độc nhất | P08/P09/P15; blockout quyết số module thật |
| A10 Tầng/AI/projectile mask | Vùng đi được/bounds logic; LoS A không lọc vs B SolidWall; Hybrid 3/1/0 là ví dụ lịch sử | Không navigation nhiều tầng/full geometry LoS; exact A/B PROTOTYPE, số/identity Hybrid OPEN | P09; duyệt nội dung trước chọn roster Hybrid, không khóa 3/1/0 |
| A11 Anticipation online | Chỉ local pose: rẻ nhưng chờ projectile; projectile tạm: mượt hơn, cần xử lý ghost/dedup; rollback combat: tăng scope | Pose/âm/cast cosmetic là probe; projectile chính/result authoritative; tentative gameplay projectile và rollback DROP P0 | P12; không thêm prediction/rollback physics P0 |
| A12 Camera/UI/pixel scale | 480×270 hoặc 640×360 và policy màn hình khác; icon 32 px/lớn hơn; snap/rotation | Giữ PPU32, thử world view 15/20 u, UI font đủ dấu; chọn readability trước export | P01/P08/P11/P14; pin pipeline ở Technical spike |
| A13 Select preview | Default/class preview: không thêm data; exact gear: reuse rig nhưng cần canonical visual summary | Default preview/name/level/class; exact gear chỉ nếu duyệt dependency read-only | Design/Technical + P11/P12; không account redesign |
| A14 Action khi nhảy/chạy | Full pose attack trên không: ít setup nhưng chân lệch; upper action + lower locomotion: reuse tốt nhưng overlap khó; pose riêng: đẹp nhưng tăng cost | Không thêm movement lock để cứu art; thử hành vi được cho phép, thêm đúng pose/track thiếu | P01/P03/P09; khóa movement/jump là gameplay proposal |
| A15 Death terminal-pending | Chờ ACK mới Death: nhất quán nhưng trễ; terminal state → Death ngay: đọc tốt nhưng cần event/state riêng | Diễn theo server terminal, success/reward sau ACK; deathUtc/deadline không tự đổi | Technical + P06/P12; không local đoán chết |
| A16 Boss scope | 28 pose core hoặc thêm 4 roar; canvas 128/192 px; roar pose riêng hoặc overlay | Giữ behavior/telegraph, Cuồng không chen action; canvas theo room, roar optional | P14/P15; ngưỡng 30% không tự tạo action mới |
| A17 Accounting thực | Xuất PNG variants: nhiều ảnh; palette reuse: ít ảnh nhưng có setup; thêm shape: silhouette tốt và tăng cost | Thay S0 bằng manifest/giờ slice; không chốt tổng giờ khi chưa có evidence | P15 và review scope trước production hàng loạt |

| Alias lịch sử | Gate liên quan | Trạng thái sau consolidation / phạm vi được chấp nhận |
| --- | --- | --- |
| A01 | ART-01 | OPEN: phép cộng 26 ô là xác định; ý nghĩa giới hạn tổng raster hay ô/profile chưa được duyệt lại. Giữ nguyên user-lock trong GDD. |
| A02 | ART-01 | OPEN: hybrid và số góc/pose là phương án thử, chưa specification kỹ thuật cuối. Kiếm CURRENT; phần Cung DEFERRED. |
| A03 | GEAR-01 / BAL-01 / LOOT-01 | BASELINE giữ sáu slots/Boots stat-only theo GDD. Đề xuất bỏ Boots là LEGACY/SUPERSEDED bởi yêu cầu giữ sáu ô. Thêm world visual riêng không thuộc P0; giữ stat-only, không mở lại số slot. |
| A04 | ART-01 / CC-01 | APPROVED nguyên tắc phân biệt cast/main VFX/impact/status; OPEN concept cụ thể, số frame và motif. Kiếm CURRENT, Cung/Boss thử ở phase sau. |
| A05 | ART-01 / PHY-01 / BAL-01 | APPROVED logical target resolution/batch, fallback/status dedup; OPEN presentation travel/vertical bounds/Bow AAA balance. Class Normal executor gap đã superseded. |
| A06 | ART-01 / BAL-02 / GEAR-01 | PROBE BASELINE mới giữ hit/lock/proc, retune CD/MP/powerS2Cung theo GDD; exact số vẫn TUNABLE. Timing pose bám clock đã được chấp nhận, retune gameplay cần evidence. |
| A07 | ART-01 / QUEST-03 / SCOPE-01 | BASELINE HP60/25 s Q3; OPEN DEF/EVA, số điểm đứng thêm ngoài tối thiểu3 và timer nhanh. Q3/Q6 solo CURRENT; contention thử ở gate mạng. |
| A08 | ART-01 / PHY-01 / LOOT-01 | APPROVED corpse tách gameplay terminal/respawn. OPEN hold/fade exact, flying fall và điểm loot server chọn. |
| A09 | ART-01 / PHY-01 | APPROVED cue solid/one-way/prop và surface đọc được; OPEN tile cell/module count/cách dựng kit. Room CURRENT, full families DEFERRED. |
| A10 | PHY-01 / ART-01 | APPROVED logical lane/flying bounds; PROTOTYPE LoS A/B; Hybrid count/identity và exact Return/grace/regen/invuln/speed/targetability OPEN. Không tự khóa immune hoặc navigation; terrain/no-slope/no-climb đã LOCKED, không nằm trong options này. |
| A11 | TECH-01 / ART-01 | BASELINE presentation chỉ đọc authority; OPEN immediate local pose/âm anticipation và schema. Tentative gameplay projectile/rollback DROP P0; không giữ branch projectile prediction trong backlog. Prototype chỉ reference; production base sau docs/feel/art probe, Dedicated/2-client gate sớm sau G-L revision mới. |
| A12 | ART-01 / PHY-01 | OPEN camera/reference resolution/UI scale, pixel snapping và icon size; PPU32 giữ nguyên. |
| A13 | TECH-01 / ART-01 | OPEN preview default hoặc exact gear/read-only summary; Login/Select TARGET P0, DEFERRED khỏi VS-1. |
| A14 | ART-01 / PHY-01 / BAL-01 | APPROVED giữ gravity/momentum khi cast; CURRENT Novice/S1. OPEN S2/S3 air permissions và pose phối upper/lower; không lock movement để cứu art. |
| A15 | TECH-01 / SAVE-01 / ART-01 | BASELINE terminal-pending không reward/respawn trước commit; OPEN event presentation trước ACK và deathUtc t0 contract chi tiết. |
| A16 | BOSS-02 / CC-01 / ART-01 | DEFERRED art prototype Boss; OPEN canvas/28 pose/roar. Behavior, telegraph và Cuồng đã có baseline GDD, không bị bỏ. |
| A17 | ART-01 / SCOPE-01 / TECH-01 | OPEN tổng count/giờ thực; CURRENT đo Kiếm/room/kit và % art dùng được. S0 chỉ kịch bản, thay estimate sau evidence. |

DESIGN LOCK đã duyệt progression/controls/authority trong mục dưới; Art technique/count/camera và exact response chưa được promote thành production lock. **REJECTED cho CURRENT**: production hàng loạt trước gate, dùng AnimationEvent/VFX để sinh damage, dùng mô phỏng làm acceptance, hoặc gọi Cung là P1. Các lựa chọn thẩm mỹ chưa có prototype tiếp tục OPEN; việc chưa làm là DEFERRED, không phải loại bỏ khỏi TARGET.

<a id="design-lock-rationale"></a>

## Combat rationale, delta và gates

Luật progression/input hiện hành đọc [GDD §3](1_HUYEN_LO_GDD.md#focus-input); NSO research chỉ evidence. Basic Tân Lữ trước class tránh hard cutoff làm Q5 Lv 5 kẹt. Ba active tích lũy giữ lựa chọn skill/MP/CD riêng, thay vì thêm class Normal miễn MP; interaction riêng giúp thao tác loot không đổi combat target. Logical ranged result bỏ flight damage/interception, mất né visual tên sau resolve nhưng giữ melee/Boss geometry dodge. Movement/momentum không bị normal damage interrupt.

| Review decision | Kết luận / trade-off | Gate còn phải đo |
| --- | --- | --- |
| D01/D11/D12/D19 | Logical hit + shape identity + visual-only projectile; source snapshot/target resolve, dedup hit/status | Geometry/lifecycle/visual timing G-L rồi G-N; rerun simulation |
| D02–D05/D09/D22/D28 | AUTO acquire-retain-reacquire/EXPLICIT pinned; search/retention khác execution range; Select chỉ chọn, Execute riêng tạo one-shot pending/latest buffer; minimal target HUD | Context retention, range, pending replace/cancel/modal/spam; release không cancel pending, hold không repeat mọi skill |
| D06–D08/D10 | Damage feedback không Hurt/hit-stun/knockback; gravity/momentum liên tục; không recovery cancel P0 | Novice/S1 air/control, S2/S3 permissions còn prototype |
| D13/D16 | Lightweight logical lane/flying bounds, Ong giữ flying/ranged và band reachable | PHY-01, G-N positions/headless; không full Rigidbody/nav graph |
| D14 | Hybrid count/identity OPEN; 3/1/0 chỉ ví dụ lịch sử, không preferred/final roster; giữ các mob identity/fixedlevel | PROTOTYPE + user content approval; không tiết kiệm pose count trước duyệt |
| D15/D15R | Legit Cung kite là lợi thế; perch unreachable xử authoring+Return đơn giản, không anti-Bow AI; exact grace/Return/regen/invuln/targetability chưa chốt | Jump ngắn/perch/kite/2 players/reachable retarget; reset không reward/reroll/life mới |
| D17/D18/D20 | LoS A/B PROTOTYPE; full geometry LoS DROP P0; respawn 25 s/rates giữ TUNABLE; 28/66 là LEGACY seed, mật độ mới re-author chưa totals | Room/playtest/benchmark workload thật; không CPU%/số dòng recipe proof |
| D21/D23/D24 | Per-profile visuals; Auto/gamepad/hitstop/crit shake/material audio DEFER; Tab theo input đã duyệt; 26 frame/A01/A02 giữ OPEN | Art imported rig/weapon/readability/hour/% usable; không 33 pose lock |
| D25–D27/D29/D30 | Giữ tính toàn vẹn backend; tổng hợp findings VS-1 rồi thử trong sandbox riêng; ba ô tích lũy/CD riêng, không fallback class miễn MP | Tổng hợp/thử → nền production → G-L mới → G-N/G-D; luân phiên CD độc lập đổi burst/tài nguyên |

**ROLE AUDIT hiện hành:** power/MP, power/CD, occupancy, TTK đơn/cụm và rủi ro Cung AAA/S1 nằm tại [probe 2026-10-06](#current-balance-probe). Bảng input1–3one-press/cadence1,0/1,5/1,7/7 cũ đã SUPERSEDED; không restore class Normal để lấp MP/CD. Independent cooldown và chọn/Execute thủ công giữ quyền luân phiên skill, không cho hold-repeat hoặc auto-combat.

Research claims về CPU/GPRS/T9 motive là HISTORICAL INFERENCE; “1000 mobs <3% CPU”, deterministic/anti-cheat tuyệt đối hoặc số dòng recipe là UNSUPPORTED, không justification. Client/server splash thresholds khác nhánh, map parse chưa xác minh không khóa geometry. Ba báo cáo giữ ở [research notes](../../research/README.md), không design authority. Không triển khai navigation/DropLink/universal ranged fallback/Auto/rich UI/rollback để kế thừa NSO.

<a id="keyboard-prototype-review"></a>

## Keyboard UX và giới hạn art validation

Trạng thái build, debug fixtures và kết quả kiểm VS-1 thuộc [Roadmap](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#prototype-runtime-history); chúng không thay thế validation balance hoặc G-L production.

Editorial bổ sung First Art Probe, file lifecycle, pose/socket minimum schema, asset DoD, visual style sample và provenance ngay tại Art §22–24. Không chốt nghĩa 26 frame, technique/socket/camera hoặc sản xuất full catalog. Các bảng mô phỏng/accounting giữ để đối chiếu, không chuyển thành acceptance mới.

<a id="prototype-feedback-review"></a>

## Onboarding, control và giới hạn kiểm chứng

Luật và counts Q1–Q12 ở [GDD §5](1_HUYEN_LO_GDD.md#quests-story); input/pending ở [GDD §3](1_HUYEN_LO_GDD.md#focus-input). Các quyết định F01–F08 và review theo phiên đã chuyển nguyên văn sang [CHANGELOG](../../prototypes/VS1_EndToEnd/CHANGELOG.md#single-behavior-history-3). Giữ các alias trên để tra evidence cũ, không giữ luật gameplay trùng tại Analysis.

26-frame và technique/camera/count chưa được khóa lại; LoS/Hybrid/Return/airborneS2-S3 giữ OPEN. Migration này thay input/NPC/terrain/cadence/HP-MP theo owner GDD; bảng lịch sử còn đủ số và nhãn, không nghiệm thu runtime.

<a id="research-ideas"></a>
<a id="legacy-provenance"></a>

# 6. Nguồn tham khảo và đề xuất P1

Các đề xuất dưới đây được giữ để không mất thiết kế đang cân nhắc. **P1 / PROPOSAL, chưa duyệt triển khai**, không cộng vào balance / acceptance P0. Chọn hoặc bỏ sau core gate theo [GDD scope](1_HUYEN_LO_GDD.md#vision); không phải Open Decision chặn code P0.

| ID | Candidate | Input proposal được giữ lại |
| --- | --- | --- |
| SCOPE-03 | **Chiến Ý — Kiếm / Ưng Nhãn Cường Hóa — Cung** | Buff Lv 10, phím đề xuất còn OPEN; duration 10 s, skill CD 40 s, MP 15. Chiến Ý: +15% ATK, +5% speed. Ưng Nhãn Cường Hóa: +10 điểm % CritChance, +15% range. Không phải nội tại Ưng Nhãn Lv 5; không thêm active P0. Acquisition / stacking và exact values cần chốt nếu chọn P1. |
| SCOPE-04 | Linh Giáp / Vỡ Thế | Shield 1.000 và Groggy là proposal cũ; chưa có shield-break / CC contract P0. Cần kiểm lại Boss / CC / workload nếu chọn; không tự lấy số này làm Boss data. |

**Legacy provenance compact:** NSO reference đã khai thác, không current authority. Trace từ `research/SRC NSOACE FIX/`: `Char.initMenu/finishTask` (NPC turn-in / bag checks), equip / use callbacks và `AbilityFromEquip` (onboarding / +4), `Mob.dead` (quest assist / loot input), Part / TileMap (modular / one-way). Availability instances là historical input; Boss hiện hành shared world. Không dùng reference chứng minh crash atomicity, rates hoặc balance. Lịch sử chi tiết nằm trong Git; pattern đã nhận là design Huyền Lộ.
