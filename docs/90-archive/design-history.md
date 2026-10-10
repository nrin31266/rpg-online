# Huyền Lộ — Design History

> LEGACY / SUPERSEDED
> Không dùng làm nguồn triển khai hiện hành.
> Chỉ giữ để truy vết lịch sử thiết kế.

Nguồn hiện hành: [documentation map](../README.md). Các con số, claim và đường triển khai bên dưới mô tả revision lịch sử.

> **SUPERSEDED bởi phase 2026-10-10:** two-button Execute/Interact, Q8 force/reservation, Q8/Q10 ordinal evidence, Q11 old quotas/mob fragments/three Xích seals và multi-independent regular non-boss loot trong trace cũ không còn authority. Đọc [PrimaryAction](../02-technical/gameplay-runtime.md#active-focus), [Quest RNG/Q11](../01-design/quests-and-narrative.md#quest-collection), [natural Linh](../01-design/world-and-content.md#linh-bien), [loot](../01-design/items-and-economy.md#regular-loot-outcome). Bảng/số lịch sử giữ nguyên để truy vết.

### Bảng dữ liệu gốc lịch sử (LEGACY seed manifest — 28 cụm / 66 điểm sinh quái)

> [!NOTE]
> Bảng manifest 28 cụm dưới đây là **dữ liệu lịch sử (LEGACY seed)** được giữ lại nhằm phục vụ việc đối chiếu với bản prototype cũ và bảo toàn các `SpawnGroup` ID. Đây **không phải là trần mật độ hay giới hạn số cụm của bản hoàn chỉnh**. Khi mở rộng bản đồ, các **Quest Anchor IDs** (`DS2`, `DS3–DS6`, `TA4`, `TA4.slot1`, `TA6`, `XN1–XN3`, `XN4–XN6`, `HT4–HT5`, `HT_BossLandmark`) phải được giữ nguyên vị trí và vai trò logic vì quest trực tiếp tham chiếu chúng. Các **Stable Seed IDs** khác (`DS1`, `TA1–TA3`, `TA5`, `BV1–BV5`, `HT1–HT3`) nên giữ ổn định nếu có thể nhưng không bị khóa bởi quest.

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


**LEGACY — phương pháp mô phỏng chiến đấu trước migration:** Python 3 tạm trong `/tmp`, 24 hạt giống 0–23; HP/sát thương/EXP làm tròn half-up, thưởng từng người làm tròn xuống. Phân điểm cân bằng có bảng số thật tại §2. Các đòn chọn theo thời gian khóa động tác, hồi chiêu và Linh lực; ưu tiên đại chiêu → tiến cảnh/nhập môn → đòn thường. Mỗi hit kiểm né/chí mạng/biến thiên sát thương; Food hồi mỗi 2 s, bình khi cần, hai loại hồi chiêu riêng 8 s. Thời gian đánh chia cho tỷ lệ ra đòn hữu hiệu giả định 0,85; không tính thêm Linh lực lần hai. Kiếm gom mục tiêu trong 1,2 u, Cung ở ≥4 u. Đây là mô hình giao tranh thuận lợi, chưa tính vị trí/hitbox/latency/death.

**LEGACY — mô hình bãi:** 16 hạt giống, 90 phút/lượt bỏ 15 phút đầu; mỗi điểm sinh quái hồi 25 s sau chết, 1–4 người chơi luân phiên Kiếm/Cung chọn cụm gần cấp rồi cụm còn nhiều quái. Không cho hai người claim cùng một lượt đánh trong mô hình; chưa mô phỏng cùng đánh/nhặt, địa hình hay năng lực mạng. Số EXP/Vàng/đá là ngân sách cả bản đồ, không nhân cho mỗi người. Linh Biến roll 5% từ quái Lv 8+, tối đa một con/MapId.

**LEGACY — mô hình hành trình:** 16 hạt giống/phái/kịch bản, Q1–Q3 chiếm 6 phút, nhiệm vụ/đường đi theo GDD; Q9 tùy chọn bỏ qua. Tiến cảnh chỉ sau Q8 và Lv 10, vũ khí Rare III/đại chiêu chỉ sau Q11. Tính Vàng, đá, Food và Bình Linh lực; không giả định +8 trước Q12. Không tính HP Potion/death, túi đầy, người chơi do dự, đi vòng vì địa hình hoặc mạng. Thời gian Lv 20 tách khỏi Q12/Boss. Có bốn mức đầu tư rõ tại §4.

**LEGACY — mô hình trạng thái:** 32 hạt giống trong hai giờ, 1–4 người đánh lệch pha, giả định đòn trúng và đủ Linh lực; đây là trần thuận lợi, không là tỷ lệ hiệu ứng thực khi chơi. Boss projection cộng hai dòng sát thương độc lập, giả định 50/65/75% thời gian ra đòn hữu hiệu. PvP chỉ mô hình một chiều, không là kết quả trận đấu.

**Bất biến kiểm được:** tổng NeedEXP tới Lv 20 là 53.100, tổng 95 điểm thuộc tính. 18 dòng / 21 mẫu trang bị thường + Mộc Kiếm. Mốc art cũ “12 module hình ảnh trên một rig 26 khung” là phép đếm ba band × bốn loại, không chứng minh tổng asset hoặc diễn giải 26-frame; [Art accounting](../03-art/art-and-visual-production.md#production-accounting) giữ kịch bản đầy đủ, A01/A17 OPEN. Bảy loại quái và bản đồ hiện hành cho đường farm hợp lệ tới Lv 20. Các phép tính này không thay bài kiểm thử khi triển khai.


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

Sói Sương Lv 4 tại Lv 5 là quái thấp cấp sau cú nhảy chọn phái, nên hạ trong ~1,4 s; Tân Lữ Lv 4 với Mộc Kiếm ~3,9 s là mốc mô hình cũ, không phải TTK của nhịp đã duyệt; xem [kiểm chứng onboarding](../04-production/playtest-and-balance.md#novice-onboarding-evidence). Cụm ba/bốn con ở Lv 8+ còn sống qua nhiều action, cho kỹ năng đánh lan giá trị. Q8 Sói Trúc Linh Biến có **1.695 HP**, với đồ kỳ vọng mất **22,7/23,7 s** (Kiếm/Cung) solo. Q10 ở Lv 15 đánh ba Đạo Tặc mất **10,0/14,0 s**; Đạo Tặc Lv 13 còn thưởng đủ. Lv 15 đánh Thạch Lv 16 trước đại chiêu hơi chậm. Trước Q11, Lv 17 đánh ba Thạch bằng đồ II mất **8,8/13,3 s**; cần kiểm né và dùng thuốc trong cảnh thật để tránh kẹt khi ba quái cùng áp sát.

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

Phần này chuyển đầy đủ từ Art §10/§10.1/§10.2 ngày 2026-10-03. Chủ sở hữu phép tính/probe là Analysis; [Art §10](../03-art/art-and-visual-production.md#art-timing) giữ cách diễn pose/VFX. Đây là **DERIVED / PROPOSAL**, không kết quả Unity. Các mốc test không thay luật [GDD §3](../01-design/combat-and-character.md#class-combat).

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


**LEGACY phần HP/TTK của đoạn này:** ở Lv 20 cân bằng trước migration, toàn bộ Rare III +6 → +8 đổi ATK nhân vật **116,6 → 125,4** (+7,5%) và Máu Kiếm **929 → 996** (+7,2%) sau nội tại; thời gian solo Cổ Vệ **5,28 → 4,62 s** là mô phỏng trước khi phân hóa Chí mạng/Chính xác vũ khí, cần chạy lại. Toàn bộ Epic III +8 so Common III +0 đổi ATK **91,6 → 131,1**; đó là biên sức mạnh sau truyện. Tinh Hoa II và flat của Giày/Nhẫn/Dây chuyền đã tính trong các mốc +8, không nhân lại theo phẩm chất.


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




Hồ sơ feedback và lỗi của mock được giữ tại [Roadmap](production-history.md#prototype-feedback-history); các mô phỏng balance dưới đây không nghiệm thu layout của bản mẫu.


**Legacy provenance compact:** NSO reference đã khai thác, không current authority. Trace từ `research/SRC NSOACE FIX/`: `Char.initMenu/finishTask` (NPC turn-in / bag checks), equip / use callbacks và `AbilityFromEquip` (onboarding / +4), `Mob.dead` (quest assist / loot input), Part / TileMap (modular / one-way). Availability instances là historical input; Boss hiện hành shared world. Không dùng reference chứng minh crash atomicity, rates hoặc balance. Lịch sử chi tiết nằm trong Git; pattern đã nhận là design Huyền Lộ.


> **HISTORICAL / SUPERSEDED một phần:** Decision Trace 2026-10-09 bên dưới giữ nguyên evidence lúc recovery, không current input/quest authority. Q8/Q10 normal kill quotas, Q11 quota/mob fragments và two-button input đã bị thay tại [Quest](../01-design/quests-and-narrative.md#quest-gameplay-review) / [Runtime](../02-technical/gameplay-runtime.md#active-focus). Git không chứng minh approval.

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



### TTK và group clear — mô phỏng lịch sử 24 seeds, SUPERSEDED cho targeting/pacing

**HISTORICAL ASSUMPTIONS, không executor requirement:** chưa rerun sau sửa primary-proximity/counts; không dùng TTK dưới để nghiệm thu revision mới. Phép tính stats/gear và cost/CD vẫn DERIVED từ values không đổi; thuận lợi ideal target-set không chứng minh distribution thực. Python 3 tạm trong `/tmp`, 24 hạt giống 0–23; bước đồng hồ 0,01 s. Sát thương làm tròn half-up; né/chí mạng/random theo design owner; một Bỏng/mục tiêu, refresh giữ nhịp tick. Mọi mục tiêu đứng trong hình đòn hợp lệ; Kiếm ở ≤1,2 u, Cung ≥4 u để có nội tại Lv 13. Chụp nguồn lúc cast; lên lịch hit +0,12/0,14/0,16/0,18 s, index mất hiệu lực không chuyển đích. Spread ABC/ABA/AAA theo số mục tiêu sống lúc bắt đầu; giới hạn Line/nổ theo owner tại revision cũ. Đủ bộ Common +0, Food đúng bậc;

Bình MP chỉ khi thiếu chi phí, hồi chiêu 8 s. Chưa tính phản công, Bình HP, chết, đi đường, túi/UI/mạng hoặc hụt hình đòn; chưa mô phỏng Đóng Băng/Làm Chậm vì không có AI phản công. Không nhân hệ số 0,85 của mô hình cũ. “Luân phiên” giả định người chơi chọn rồi Execute theo ưu tiên S3→S2→S1 khi sẵn, **không phải auto-combat hay lặp khi giữ phím của game**. Hàng Lv 17 giả định đã hoàn Q11/học S3, không dùng tính độ khó Q11.

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

**Boss sensitivity mới, chưa trận Boss:** 120 s bấm thuận lợi cùng mô hình, target DEF25/EVA60 và HP rất lớn để đo dòng sát thương; Common III+0 cân bằng. Kiếm284,16 DPS, Cung318,30 DPS, MP dùng8,0417/8,025 mỗi giây; 129/128 S1,129 S2,20 S3 trong cửa sổ (cast cuối có thể resolve ngoài120 s), Kiếm dùng1 Bình MP, Cung0 do pool đầu. Burn giữ, target không phản công. Tổng602,46 DPS; nhân tỷ lệ ra đòn hữu hiệu50/65/75% → Boss32.000 khoảng106/82/71 s.

Đây là sensitivity tính từ hai dòng độc lập, **không mô phỏng vừa né vừa dùng tài nguyên**; food/pool đầu khiến nhu cầu bình dài hạn khác. Biên65–75% có thể nhanh hơn target90–150 s: ghi rủi ro BOSS-02/BAL-02, giữ BossHP/ATK và telegraph hiện hành tới playtest, không gọi đã đạt target.

**PvP retune audit — arithmetic, chưa duel:** MaxHP Cung mới738 so Kiếm844,8 ở fixtureLv20; Bình III hồi442,8/506,88 HP. Ba lầnHP có trần tổng1.328,4/1.520,64 hồi (thực tế clamp/HP đầy/quota làm thấp hơn), Food III trung bình14,76/16,896 HP/s. Cadence mới tăng pressure nhưng gear/HP/Food/Potion thay cả sống sót lẫn hồi phục; bảng PvP TTK cũ không còn đủ. Hệ số0,20, quota3+3, CD8 s, Băng Hàn move-only, Bỏng immune, cược/payout và 120 s DRAW giữ nguyên.

Cần duel cả chiều, VIT/INT/cực đoan/gear-lag và tần suất hòa trước retune hệ số hoặc economic stake.
