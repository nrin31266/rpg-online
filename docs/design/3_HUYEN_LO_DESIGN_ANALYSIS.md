# Huyền Lộ — Phân tích thiết kế

**Đối chiếu:** [GDD hiện hành](1_HUYEN_LO_GDD.md) · **Ngày:** 2026-10-03 · **Vai trò:** evidence, simulation và quyết định mở.

GDD là design authority; Technical là implementation contract; Art sở hữu visual/production; [Roadmap](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md) giữ CURRENT/DEFERRED. Local Kiếm là bước triển khai đầu, mọi bảng Cung/online vẫn là evidence TARGET P0. **DERIVED** là phép tính từ GDD; **SIMULATION** phụ thuộc giả định; **PROPOSAL** chưa thành luật. Bảng chỉ số trang bị và hồ sơ xác định đã tính lại với Chí mạng Kiếm, Chính xác Cung và tốc chạy Giày. Các mô phỏng giao tranh, hành trình và Boss còn dùng hồ sơ trước thay đổi này; xem là mốc đối chiếu, cần chạy lại khi có harness/runtime. Chưa có Unity/runtime acceptance.

<a id="balance-baselines"></a>

# 1. Phương pháp, đối chiếu và giới hạn

Bộ số combat/gear trong GDD V6.1.0 giữ nguyên; các bảng dưới đây là phép kiểm, không thay thế GDD. **TÍNH TỪ LUẬT** là phép tính xác định; **MÔ PHỎNG** phụ thuộc giả định; **ĐỀ XUẤT** chưa là luật. PvP có Food/Potion và recovery checkpoint mới nên mô hình PvP cũ chỉ là đối chiếu sát thương trực tiếp, không dự báo thắng/hòa; các mô phỏng farm không đổi. Chưa chạy Unity hoặc nghiệm thu game thật.

**Nguồn đầu vào:** [nhân vật](1_HUYEN_LO_GDD.md#character-power), [kỹ năng/trạng thái](1_HUYEN_LO_GDD.md#class-combat), [quái và bãi](1_HUYEN_LO_GDD.md#world-farm), [trang bị/thưởng](1_HUYEN_LO_GDD.md#gear-economy), [Food/Bình](1_HUYEN_LO_GDD.md#consumables-death). Analysis không giữ catalog thứ hai.

**Phép tính chiến đấu:** Python 3 tạm trong `/tmp`, 24 hạt giống 0–23; HP/sát thương/EXP làm tròn half-up, thưởng từng người làm tròn xuống. Phân điểm cân bằng có bảng số thật tại §2. Các đòn chọn theo thời gian khóa động tác, hồi chiêu và Linh lực; ưu tiên đại chiêu → tiến cảnh/nhập môn → đòn thường. Mỗi hit kiểm né/chí mạng/biến thiên sát thương; Food hồi mỗi 2 s, bình khi cần, hai loại hồi chiêu riêng 8 s. Thời gian đánh chia cho tỷ lệ ra đòn hữu hiệu giả định 0,85; không tính thêm Linh lực lần hai. Kiếm gom mục tiêu trong 1,2 u, Cung ở ≥4 u. Đây là mô hình giao tranh thuận lợi, chưa tính vị trí/hitbox/latency/death.

**Mô hình bãi:** 16 hạt giống, 90 phút/lượt bỏ 15 phút đầu; mỗi điểm sinh quái hồi 25 s sau chết, 1–4 người chơi luân phiên Kiếm/Cung chọn cụm gần cấp rồi cụm còn nhiều quái. Không cho hai người claim cùng một lượt đánh trong mô hình; chưa mô phỏng cùng đánh/nhặt, địa hình hay năng lực mạng. Số EXP/Vàng/đá là ngân sách cả bản đồ, không nhân cho mỗi người. Linh Biến roll 5% từ quái Lv 8+, tối đa một con/MapId.

**Mô hình hành trình:** 16 hạt giống/phái/kịch bản, Q1–Q3 chiếm 6 phút, nhiệm vụ/đường đi theo GDD; Q9 tùy chọn bỏ qua. Tiến cảnh chỉ sau Q8 và Lv 10, vũ khí Rare III/đại chiêu chỉ sau Q11. Tính Vàng, đá, Food và Bình Linh lực; không giả định +8 trước Q12. Không tính HP Potion/death, túi đầy, người chơi do dự, đi vòng vì địa hình hoặc mạng. Thời gian Lv 20 tách khỏi Q12/Boss. Có bốn mức đầu tư rõ tại §4.

**Mô hình trạng thái:** 32 hạt giống trong hai giờ, 1–4 người đánh lệch pha, giả định đòn trúng và đủ Linh lực; đây là trần thuận lợi, không là tỷ lệ hiệu ứng thực khi chơi. Boss projection cộng hai dòng sát thương độc lập, giả định 50/65/75% thời gian ra đòn hữu hiệu. PvP chỉ mô hình một chiều, không là kết quả trận đấu.

**Bất biến kiểm được:** tổng NeedEXP tới Lv 20 là 53.100, tổng 95 điểm thuộc tính. 18 dòng / 21 mẫu trang bị thường + Mộc Kiếm. Mốc art cũ “12 module hình ảnh trên một rig 26 khung” là phép đếm ba band × bốn loại, không chứng minh tổng asset hoặc diễn giải 26-frame; [Art accounting](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#production-accounting) giữ kịch bản đầy đủ, A01/A17 OPEN. Bảy loại quái và bản đồ hiện hành cho đường farm hợp lệ tới Lv 20. Các phép tính này không thay bài kiểm thử khi triển khai.

<a id="character-evidence"></a>
<a id="combat-analysis"></a>

# 2. Nhân vật, chiến đấu và hiệu ứng

## 95 điểm thuộc tính: các cách phân minh bạch

Mỗi cấp sau Lv 1 cho 5 điểm; Lv 20 có 95 điểm. Trước khi chọn phái ở Lv 5, Q5 dùng 20 điểm **chưa phân** sau lần hoàn điểm nhập môn. Bản cân bằng sau chọn phái chia gần đều: điểm dư lần lượt vào Sinh Lực, Linh Lực, Công Lực, rồi Thân Pháp; không mặc định dồn phần dư cho Thân Pháp.

**Cách tính:** nền theo cấp + điểm + trang bị đã tính phẩm chất/cường hóa/Tinh Hoa, sau đó mới áp nội tại nền tảng một lần. Các hàng sau chọn phái dùng sáu món Common +0 theo mốc mặc, gồm Chí mạng trên Kiếm, Chính xác trên Cung và tốc chạy trên Giày (Lv 5/10 bậc I, Lv 13 bậc II, Lv 17/20 bậc III); đây là bộ kiểm chỉ số, không giả định người chơi đã mua đủ set. Trong các ô ghi hai số, thứ tự luôn là **Kiếm / Cung**. Q5 trước chọn phái chỉ có Tân Lữ với Mộc Kiếm + Quần I, chưa hưởng nội tại. Chính xác/Né tránh là chỉ số thô dùng trong công thức né, không phải phần trăm trúng/né; Cung được nhân Chính xác ×1,08 sau khi cộng điểm và trang bị. Tốc chạy tăng so với nền gồm +0,05% mỗi điểm Thân Pháp và bonus Giày cố định 1/2/3% theo bậc.

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

## Thời gian hạ quái và duy trì Linh lực

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

Sói Sương Lv 4 tại Lv 5 là quái thấp cấp sau cú nhảy chọn phái, nên hạ trong ~1,4 s; Tân Lữ Lv 4 với Mộc Kiếm hạ trong ~3,9 s. Cụm ba/bốn con ở Lv 8+ còn sống qua nhiều action, cho kỹ năng đánh lan giá trị. Q8 Sói Trúc Linh Biến có **1.695 HP**, với đồ kỳ vọng mất **22,7/23,7 s** (Kiếm/Cung) solo. Q10 ở Lv 15 đánh ba Đạo Tặc mất **10,0/14,0 s**; Đạo Tặc Lv 13 còn thưởng đủ. Lv 15 đánh Thạch Lv 16 trước đại chiêu hơi chậm. Trước Q11, Lv 17 đánh ba Thạch bằng đồ II mất **8,8/13,3 s**; cần kiểm né và dùng thuốc trong cảnh thật để tránh kẹt khi ba quái cùng áp sát.

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

## Nội tại tinh thông ở Lv 13

Kiếm Thế chỉ tăng 12% sát thương trực tiếp của kỹ năng khi mục tiêu ≤1,2 u; Xạ Tâm chỉ tăng khi impact ≥4 u. Phép thử cũ cho Kiếm 6,21→5,82 s solo và Cung 5,16→4,60 s; thay phân điểm cân bằng có thể dịch số nhỏ, nhưng điều kiện và xu hướng không đổi. Không cộng vào đòn thường, Bỏng hoặc xác suất trạng thái. Cảm giác thực cần combat slice.
## Bỏng và Băng Hàn khi nhiều người cùng đánh

Status probe 32 seeds, hai giờ, nhịp cast cố định và giả định **mọi hit trúng, đủ MP**. Cột trái chỉ core, phải core + đại chiêu. Bỏng: core 4%, đại chiêu 70%, 6 s, tick 1 s × 0,06 source ATK; một effect/target, refresh expiry/source **giữ next tick**. Normal/Linh cùng Đóng Băng 1,5 s; proc core 2/1%, đại chiêu 45/30%; sau tan băng miễn 3 s target-wide. Boss Làm Chậm proc core 2%/big 100%, MoveSpeed ×0,85 và ActionClockSpeed ×0,75 trong 3 s, refresh-only. Không đặt Đóng Băng và Làm Chậm cùng một target. PvP chỉ MoveSpeed ×0,75 trong 1,5 s, không Đóng Băng/Bỏng.

| Số người cùng đánh | Bỏng: cơ bản / có đại chiêu, % thời gian | Quái thường Đóng Băng: cơ bản / có đại chiêu | Linh Biến Đóng Băng: cơ bản / có đại chiêu | Boss Làm Chậm: cơ bản / có đại chiêu |
| --- | --- | --- | --- | --- |
| 1 | 14,77 / 65,64% | 1,67 / 10,40% | 0,83 / 6,98% | 3,43 / 44,80% |
| 2 | 27,77 / 89,03% | 3,22 / 14,51% | 1,66 / 10,69% | 6,92 / 86,70% |
| 3 | 38,90 / 96,56% | 4,70 / 20,73% | 2,47 / 15,62% | 10,34 / 100,00% |
| 4 | 48,04 / 99,03% | 5,97 / 21,29% | 3,21 / 16,99% | 13,52 / 100,00% |

Bốn Cung cho quái thường Đóng Băng khoảng **21,3%**, Linh **17,0%** trong phép thử thuận lợi; giới hạn lý thuyết do 1,5 s + 3 s miễn hiệu ứng là **33,3%**. Bỏng từ bốn Kiếm gần như liên tục khi dùng đại chiêu đúng hồi chiêu, nhưng **vẫn một lần sát thương mỗi giây, không nhân bốn lần sát thương**. Số nhịp Bỏng mỗi giây nếu đặt lại nhịp đầu so với giữ lịch cũ: 1 Kiếm **0,633/0,651**, 2 **0,780/0,877**, 4 **0,687/0,987**; vì vậy luật giữ `nextTickAt`. Tick Lv 20 chỉ khoảng 5–6 HP trước DEF cho Cổ Vệ 1.393 HP, Bỏng chỉ bổ sung sát thương, không phải đòn bùng nổ. Với ba Cung cast đại chiêu lệch pha, Boss Làm Chậm có thể đạt 100% thời gian trong phép thử; đồng hồ chờ action tương lai vẫn chỉ kéo dài tối đa **33,3%** (tốc đếm 75%), mức làm chậm không chồng và vùng báo đòn/action đã bắt đầu không đổi. Không cần guard thứ hai trước kiểm bằng game chạy được, nhưng CC-01 phải đo Boss không mất đe dọa. Đóng Băng và Làm Chậm cần VFX khác nhau theo GDD/Technical.

## Hệ số sát thương PvP — mô hình một chiều, chưa phải đấu thật

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

## Evidence timing từ Art — giữ baseline, chưa retune

Phần này chuyển đầy đủ từ Art §10/§10.1/§10.2 ngày 2026-10-03. Chủ sở hữu phép tính/probe là Analysis; [Art §10](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#art-timing) giữ cách diễn pose/VFX. Đây là **DERIVED / PROPOSAL**, không kết quả Unity. Các mốc test không thay luật [GDD §3](1_HUYEN_LO_GDD.md#class-combat).

**Đọc từ:** GDD §3, Analysis §2. Giữ số hiện hành trước test; mô phỏng TTK cũ chưa tính đủ Chí mạng/Chính xác vũ khí và Giày, không bằng chứng chốt nhịp.

| Đại lượng | Hiện hành / suy ra | Probe recommendation, chưa đổi luật |
| --- | --- | --- |
| Normal interval | Tân Lữ1,00 s; Kiếm0,80 s; Cung0,90 s | Giữ làm control; nếu feel sai, thử Kiếm0,70–0,90 và Cung0,85–1,05 s theo cặp, gap khoảng0,05–0,20 s. Không đổi range/damage cùng lượt thử |
| Cung chậm hơn Kiếm | Interval +0,10 s =12,5% dài hơn; tần suất đòn1,111 so1,25/s =11,1% ít hơn | 0,10 s hợp lý để probe draw, chưa chứng minh tối ưu. Với power0,95, raw normal/s khoảng15,6% thấp hơn Kiếm trước hit/crit, bù bằng range/accuracy chứ không riêng art |
| Normal hit/spawn và lock | +0,10 s /0,26 s cho mọi class | Kiếm Attack3 ở12 FPS dài0,25 s gần khớp lock; hit0,10 cần duration riêng. Cung draw0,10 s có nguy cơ quá ngắn. Thử release0,12–0,18 s, lock0,28–0,36 s **chỉ nếu cần**, rerun scheduler/TTK |
| Lv5 core | 2MP/1,0 s, spawn/hit0,12 s, lock0,30 s | Pose prepare/draw cần hoàn trước0,12; dùng hold/time map. Nếu spam làm normal không có vai trò, đo usage rồi thử CD1,2/1,4 s riêng, không mặc định nerf |
| Lv10 core | Kiếm4MP/1,5 s/lock0,30; Cung4MP/1,7 s/lock0,34 | Giữ control; khi cần probe CD ±0,2 s, giữ cost/power trước để tách ảnh hưởng |
| Lv17 signature | 16MP/7 s/lock0,40; Kiếm hit0,16, Hàn spawn0,18 | Cast chuẩn bị có thể giữ bốn pose khác duration. CD6/7/8 s là sensitivity test sau có scene, không tự tăng để VFX dài hơn |

Timing contract là **anticipation → release/hit mốc server → recovery**, tách interval/CD/action lock/flight/VFX decay. Skill4 ở12 FPS dài0,333 s >lock nhập môn0,30 s: nếu giữ duration đều sẽ trễ pose return hoặc khóa người chơi ngoài luật. Đề xuất sampling theo normalized action phase, đặt release khớp mốc server, cắt/chuyển recovery khi action mới hợp lệ; main VFX có thể tan sau actor về Idle. Không queue thêm hit do frame skipped.

Normal còn vai trò: không MP, lấp quãng CD/MP thiếu và gây damage ổn định. Trong giả định skill luôn sẵn đúng CD, Lv10 core chiếm lock khoảng0,30/1,5 hoặc0,34/1,7 =**20% thời gian**; Lv17 thêm0,40/7≈5,7%. Đây chỉ là occupancy suy ra, không bảo đảm normal đạt full attack rate vì mốc CD có thể đụng nhau. Cần log số normal/skill/idle do MP và action lock; không kết luận normal vô dụng từ CD1 s của Lv5.

<a id="art-sustain-evidence"></a>

### Sustain Lv 5/10/20 — giả định thuận lợi

Phép tính dưới dùng bảng **chỉ số đã cập nhật** trong Analysis, hồ sơ cân bằng/full Common+0 ở đúng band, Food đúng cấp. Upper demand = cost/CD cho core + big khi đã học; không tính travel/evade/lock làm bỏ cast, không dự báo TTK thực.

| Mốc / Kiếm–Cung | MaxMP | Food MP/s | Upper skill MP/s | Thiếu MP/s |
| --- | --- | --- | --- | --- |
| Lv5 / FoodI / core | 121 /133,1 | 0,9075 /0,9983 | 2 /2 | 1,0925 /1,0018 |
| Lv10 / FoodII / tiến cảnh | 171 /188,1 | 1,71 /1,881 | 4/1,5=2,6667 /4/1,7=2,3529 | 0,9567 /0,4719 |
| Lv20 / FoodIII / core+big | 316 /347,6 | 3,95 /4,345 | 4/1,5+16/7=4,9524 /4/1,7+16/7=4,6387 | 1,0024 /0,2937 |

FoodI/II/III hồi MP tương ứng0,75%/1%/1,25% MaxMP mỗi giây trung bình. Không tính regen khi chết/không Food. Thay CD, action lock, Boots hoặc target travel sẽ đổi thời gian hữu hiệu/cost bình; thử có Food, thiếu Food, cân bằng và zero-INT, không dùng full+8 làm chuẩn mọi người.

TTK target cùng cấp/Common+0 hiện early2–4 s, mid3–6, late4–8 s; các loài chỉ có bảy level cố định nên đo actual pair gần cấp, không dựng thêm mob giả cùng mỗi level. Đo trước/sau Q11, solo và nhóm2/3/5 target, hit/miss ở hai tầng, run-back và 2/4 player. Giữ HP curve trước khi có evidence, không tăng máu để bù VFX signature quá dài.

<a id="art-proc-evidence"></a>

### Proc và nhịp feedback — tần suất, không phải uptime

Normal không roll Bỏng/Băng Hàn. Upper successful chance cadence trên một target trúng mọi cast: Kiếm core4%/1,5 s → một proc kỳ vọng mỗi37,5 s; Cung core2%/1,7 s →85 s, Linh1%→170 s. Đại chiêu Kiếm70%/7 s →10 s; Hàn normal45%→15,6 s, Linh30%→23,3 s. Đây là tần suất roll thuận lợi, không uptime vì refresh/protection/miss/đổi target. Không tăng core proc chỉ để art status xuất hiện nhiều.

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
| 3 | Học Viện | Dummy Lv 3 | 60 / 0 / 0 | Mộc + Quần I / I | Q3 equip / 3 dummy |
| 4 | Đồng DS3–DS6 | Sói Sương Lv 4 | 107 / 22 / 11–18 | I, không Weapon / I | Food / H; Q4 lên Lv 5 |
| 5 | Đồng DS2(Q5) → Trúc TA1–3 | Sói Sương Lv 4 | 107 / 22 / 11–18 | I, không Weapon / I | Q5 Nấm Lv 2 là ngoại lệ; Q6 class / manual |
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

## Mô hình lịch sinh quái theo từng điểm — MÔ PHỎNG

N = 1–4 cùng MapId nhưng chia pull trong scheduler; chưa mô phỏng cùng đánh/tranh loot, geometry hoặc network. Throughput là budget cả map, không full reward mỗi người. Linh counts thấp hơn 5% kills do cap/lifetime. Q8 force nằm trong journey, không steady-state world rows.

| Map / player Lv | N = 1 normal / Linh deaths / h | N = 1 EXP / Gold / h | N = 1 Stone / gear generated / h | Idle wait % N = 1 / 2 / 3 / 4 |
| --- | --- | --- | --- | --- |
| Đồng Sương (DS) / 4 | 532,0 / 0,0 | 11.704 / 7.714 | 42,6 / 27,1 | 0,0 / 0,0 / 15,0 / 35,9 |
| Trúc Ảnh (TA) / 5 | 609,6 / 0,0 | 13.411 / 8.839 | 48,8 / 31,1 | 0,0 / 0,0 / 2,9 / 20,6 |
| Trúc Ảnh (TA) / 8 | 342,3 / 14,8 | 14.700 / 9.478 | 42,2 / 22,9 | 0,0 / 0,6 / 13,6 / 32,8 |
| Bạch Vân (BV) / 10 | 304,8 / 8,4 | 15.515 / 9.738 | 32,8 / 18,6 | 0,0 / 0,0 / 0,1 / 0,5 |
| Xích Nham (XN) / 15 | 404,8 / 12,2 | 35.762 / 19.647 | 44,6 / 25,1 | 0,0 / 0,0 / 0,0 / 0,0 |
| Huyền Tích (HT) / 20 | 438,7 / 17,3 | 0 / 26.746 | 52,4 / 28,7 | 0,0 / 0,0 / 0,5 / 1,1 |

Early N = 4 vẫn bottleneck: Đồng khoảng 36%, Trúc Lv 8 khoảng 33% thời gian chờ; N = 2 tại Trúc Lv 8 dưới 1%. Trúc có ba cụm Sói Lv 4 thay hai cụm, giữ budget 13 slots; Lv 8 có bốn Sói Trúc + ba Ong eligible. Cần benchmark rotate/crowd/CC, không tự thêm Channel/Party hay công bố capacity bốn người. Khi cụm gần level bị chiếm, scheduler chọn quái khó hơn; idle thấp ở Bạch/Xích/Huyền không chứng minh người chơi sẽ nhận risk đó. Chỉ chọn ngang cấp có thể chờ lâu hơn bảng.

| BV / player Lv 10 / N = 4 respawn | Normal / Linh deaths / h | Wait% |
| --- | --- | ---: |
| 20 s | 1078,2 / 32,5 | 0,20 |
| 25 s | 1012,6 / 32,1 | 0,38 |
| 30 s | 951,2 / 31,7 | 1,13 |

Giữ 25 s baseline; không giảm respawn chỉ từ toy model. Roster giữ 28 cụm/66 slots. Density/aggro/root deadlines và Q8 cap phải test ở scene thật. Q8 có thể chờ variant khác chết rồi slot respawn; không demote để demo nhanh. Threshold 20% trên một full-health life cho tối đa năm requesters đủ credit; nhiều hơn dùng waiting set/life tiếp theo, không spawn mỗi player. Boot/Return/disconnect không reroll Linh.

<a id="economy-analysis"></a>
<a id="loot-consumable-analysis"></a>
<a id="quest-progression"></a>
<a id="boss-availability"></a>

# 4. Trang bị, kinh tế, nhiệm vụ và hành trình

## Trang bị, cường hóa và chuyển giao

Giữ **18 dòng / 21 mẫu thường + Mộc Kiếm**; mốc mặc I không phải vũ khí Lv 1, vũ khí I Lv 5, tất cả II Lv 11, tất cả III Lv 17. Vũ khí giữ ATK 15/28/40; Kiếm thêm 0,5/1/1,5 điểm % Chí mạng, Cung thêm 10/20/30 Chính xác. Giày I/II/III thêm 1/2/3% tốc chạy cố định. Giữ Áo 40/90/125 HP, Quần 25/55/80 HP, Dây chuyền 20/40/60 MP, các chỉ số còn lại, giá mua/bán và nguồn rơi GDD §6. Ong Lv 10 rơi II trước khi người chơi mặc ở Lv 11. Tinh Hoa I/II cố định đã chốt tại GDD; **GEAR-01 đóng ở mức data baseline**, còn sức mạnh khi chơi cần kiểm.

**Kiểm biên Common +0:** Với hồ sơ cân bằng tại Lv 5/13/20 và quái thường cùng cấp (EVA theo GDD), Chí mạng Kiếm mới tăng kỳ vọng sát thương trực tiếp khoảng 0,24/0,48/0,73%; Chính xác Cung giảm tỷ lệ bị né khoảng 0,29/0,27/0,27 điểm %. Đây là khác biệt nhỏ trước mô phỏng kỹ năng/di chuyển; Giày tăng tốc chạy tuyệt đối 1/2/3 điểm % nhưng không đổi nhịp đòn hoặc hồi chiêu. Không dùng phép kiểm này để khẳng định TTK mới.

<a id="gear-upgrade-values"></a>

## Bảng tra đủ chỉ số 21 mẫu trang bị thường + Mộc Kiếm

Bảng này **tính từ luật GDD §6**, chỉ cho phẩm chất **Common**; không phải catalog hay luật thứ hai. Mỗi ô là chỉ số **của riêng món đồ**, trước khi cộng nền nhân vật, điểm thuộc tính và nội tại phái. Kiếm cùng bậc thêm Chí mạng cố định, Cung thêm Chính xác chịu phẩm chất nhưng không chịu hệ số cường hóa; Giày thêm tốc chạy cố định. Tinh Hoa cộng sau các chỉ số này; Chí mạng và tốc chạy hiển thị theo điểm phần trăm. Số lẻ là giá trị nội bộ; UI mới làm tròn. Tinh Hoa I đã cộng tại +4; Tinh Hoa II chỉ có ở bậc III +8. Dấu **—** là cấp vượt trần hoặc không được nâng; các phẩm chất khác dùng đúng công thức GDD.

| Bậc | Món | +0 | +1 | +2 | +3 | +4 | +5 | +6 | +7 | +8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Ngoại lệ | Mộc Kiếm Q3 | ATK 10 | — | — | — | — | — | — | — | — |
| I — Thanh Mộc | Thanh Mộc Kiếm | ATK 15 · Crit 0,5% | ATK 15,75 · Crit 0,5% | ATK 16,5 · Crit 0,5% | ATK 17,4 · Crit 0,5% | ATK 18,45 · Crit 1% | — | — | — | — |
| I — Thanh Mộc | Thanh Mộc Cung | ATK 15 · ACC 10 | ATK 15,75 · ACC 10 | ATK 16,5 · ACC 10 | ATK 17,4 · ACC 10 | ATK 18,45 · ACC 10 · Crit 0,5% | — | — | — | — |
| I — Thanh Mộc | Áo Thanh Mộc | HP 40 · DEF 4 | HP 42 · DEF 4,2 | HP 44 · DEF 4,4 | HP 46,4 · DEF 4,64 | HP 59,2 · DEF 4,92 | — | — | — | — |
| I — Thanh Mộc | Quần Thanh Mộc | HP 25 · DEF 3 | HP 26,25 · DEF 3,15 | HP 27,5 · DEF 3,3 | HP 29 · DEF 3,48 | HP 30,75 · DEF 4,69 | — | — | — | — |
| I — Thanh Mộc | Giày Thanh Mộc | DEF 2 · EVA 4 · Tốc chạy +1% | DEF 2,1 · EVA 6 · Tốc chạy +1% | DEF 2,2 · EVA 8 · Tốc chạy +1% | DEF 2,32 · EVA 10 · Tốc chạy +1% | DEF 2,46 · EVA 16 · Tốc chạy +1% | — | — | — | — |
| I — Thanh Mộc | Nhẫn Thanh Mộc | ACC 6 · Crit 1% | ACC 8 · Crit 1,2% | ACC 10 · Crit 1,4% | ACC 12 · Crit 1,6% | ACC 18 · Crit 1,8% | — | — | — | — |
| I — Thanh Mộc | Dây chuyền Thanh Mộc | MP 20 · EVA 5 | MP 21 · EVA 7 | MP 22 · EVA 9 | MP 23,2 · EVA 11 | MP 34,6 · EVA 13 | — | — | — | — |
| II — Vân Nham | Vân Nham Kiếm | ATK 28 · Crit 1% | ATK 29,4 · Crit 1% | ATK 30,8 · Crit 1% | ATK 32,48 · Crit 1% | ATK 34,44 · Crit 1,5% | ATK 36,96 · Crit 1,5% | ATK 39,2 · Crit 1,5% | — | — |
| II — Vân Nham | Vân Nham Cung | ATK 28 · ACC 20 | ATK 29,4 · ACC 20 | ATK 30,8 · ACC 20 | ATK 32,48 · ACC 20 | ATK 34,44 · ACC 20 · Crit 0,5% | ATK 36,96 · ACC 20 · Crit 0,5% | ATK 39,2 · ACC 20 · Crit 0,5% | — | — |
| II — Vân Nham | Áo Vân Nham | HP 90 · DEF 9 | HP 94,5 · DEF 9,45 | HP 99 · DEF 9,9 | HP 104,4 · DEF 10,44 | HP 120,7 · DEF 11,07 | HP 128,8 · DEF 11,88 | HP 136 · DEF 12,6 | — | — |
| II — Vân Nham | Quần Vân Nham | HP 55 · DEF 6 | HP 57,75 · DEF 6,3 | HP 60,5 · DEF 6,6 | HP 63,8 · DEF 6,96 | HP 67,65 · DEF 8,38 | HP 72,6 · DEF 8,92 | HP 77 · DEF 9,4 | — | — |
| II — Vân Nham | Giày Vân Nham | DEF 4 · EVA 8 · Tốc chạy +2% | DEF 4,2 · EVA 10 · Tốc chạy +2% | DEF 4,4 · EVA 12 · Tốc chạy +2% | DEF 4,64 · EVA 14 · Tốc chạy +2% | DEF 4,92 · EVA 20 · Tốc chạy +2% | DEF 5,28 · EVA 22 · Tốc chạy +2% | DEF 5,6 · EVA 24 · Tốc chạy +2% | — | — |
| II — Vân Nham | Nhẫn Vân Nham | ACC 10 · Crit 1,5% | ACC 12 · Crit 1,7% | ACC 14 · Crit 1,9% | ACC 16 · Crit 2,1% | ACC 22 · Crit 2,3% | ACC 24 · Crit 2,5% | ACC 26 · Crit 2,7% | — | — |
| II — Vân Nham | Dây chuyền Vân Nham | MP 40 · EVA 8 | MP 42 · EVA 10 | MP 44 · EVA 12 | MP 46,4 · EVA 14 | MP 59,2 · EVA 16 | MP 62,8 · EVA 18 | MP 66 · EVA 20 | — | — |
| III — Huyền Ấn | Huyền Ấn Kiếm | ATK 40 · Crit 1,5% | ATK 42 · Crit 1,5% | ATK 44 · Crit 1,5% | ATK 46,4 · Crit 1,5% | ATK 49,2 · Crit 2% | ATK 52,8 · Crit 2% | ATK 56 · Crit 2% | ATK 59,6 · Crit 2% | ATK 63,6 · Crit 2% · ACC 6 |
| III — Huyền Ấn | Huyền Ấn Cung | ATK 40 · ACC 30 | ATK 42 · ACC 30 | ATK 44 · ACC 30 | ATK 46,4 · ACC 30 | ATK 49,2 · ACC 30 · Crit 0,5% | ATK 52,8 · ACC 30 · Crit 0,5% | ATK 56 · ACC 30 · Crit 0,5% | ATK 59,6 · ACC 30 · Crit 0,5% | ATK 63,6 · ACC 36 · Crit 0,5% |
| III — Huyền Ấn | Áo Huyền Ấn | HP 125 · DEF 15 | HP 131,25 · DEF 15,75 | HP 137,5 · DEF 16,5 | HP 145 · DEF 17,4 | HP 163,75 · DEF 18,45 | HP 175 · DEF 19,8 | HP 185 · DEF 21 | HP 196,25 · DEF 22,35 | HP 208,75 · DEF 25,85 |
| III — Huyền Ấn | Quần Huyền Ấn | HP 80 · DEF 9 | HP 84 · DEF 9,45 | HP 88 · DEF 9,9 | HP 92,8 · DEF 10,44 | HP 98,4 · DEF 12,07 | HP 105,6 · DEF 12,88 | HP 112 · DEF 13,6 | HP 119,2 · DEF 14,41 | HP 142,2 · DEF 15,31 |
| III — Huyền Ấn | Giày Huyền Ấn | DEF 6 · EVA 12 · Tốc chạy +3% | DEF 6,3 · EVA 14 · Tốc chạy +3% | DEF 6,6 · EVA 16 · Tốc chạy +3% | DEF 6,96 · EVA 18 · Tốc chạy +3% | DEF 7,38 · EVA 24 · Tốc chạy +3% | DEF 7,92 · EVA 26 · Tốc chạy +3% | DEF 8,4 · EVA 28 · Tốc chạy +3% | DEF 8,94 · EVA 30 · Tốc chạy +3% | DEF 10,54 · EVA 32 · Tốc chạy +3% |
| III — Huyền Ấn | Nhẫn Huyền Ấn | ACC 15 · Crit 2% | ACC 17 · Crit 2,2% | ACC 19 · Crit 2,4% | ACC 21 · Crit 2,6% | ACC 27 · Crit 2,8% | ACC 29 · Crit 3% | ACC 31 · Crit 3,2% | ACC 33 · Crit 3,4% | ACC 35 · Crit 4,1% |
| III — Huyền Ấn | Dây chuyền Huyền Ấn | MP 60 · EVA 12 | MP 63 · EVA 14 | MP 66 · EVA 16 | MP 69,6 · EVA 18 | MP 83,8 · EVA 20 | MP 89,2 · EVA 22 | MP 94 · EVA 24 | MP 99,4 · EVA 26 | MP 105,4 · EVA 32 |

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

Rare II +6 Áo là **156,16 HP / 14,62 DEF**, so Common III +0 **125 / 15** và Common III +4 **163,75 / 18,45**. Nhẫn Rare II +6 có **27,6 ACC / 2,7% chí mạng**, còn Dây chuyền Rare II +6 **74,96 MP / 21,28 EVA**; vì thế không tự động thay toàn bộ II bằng Common III +0. Trang bị phòng thủ/phụ kiện vẫn có giá trị: bớt bình máu, tăng ổn định trúng/né và Linh lực, dù vũ khí quyết định phần lớn tốc độ hạ quái.

Ở Lv 20 cân bằng, toàn bộ Rare III +6 → +8 đổi ATK nhân vật **116,6 → 125,4** (+7,5%) và Máu Kiếm **929 → 996** (+7,2%) sau nội tại; thời gian solo Cổ Vệ **5,28 → 4,62 s** là mô phỏng trước khi phân hóa Chí mạng/Chính xác vũ khí, cần chạy lại. Toàn bộ Epic III +8 so Common III +0 đổi ATK **91,6 → 131,1**; đó là biên sức mạnh sau truyện. Tinh Hoa II và flat của Giày/Nhẫn/Dây chuyền đã tính trong các mốc +8, không nhân lại theo phẩm chất.

**Chuyển giao giữ nguyên cấp, không nhân đồ:** cùng bậc tốn 800 Vàng, không đá; lên đúng một bậc tốn 500 Vàng +2 đá. Đồ nguồn mất, đồ đích giữ template/phẩm chất/instance; từ chối nếu đích không tăng. Ví dụ Rare II +6 → Epic II +0 thành Epic II +6, tiết kiệm việc đập lại +0→+6 nhưng mất giá bán Rare II (vũ khí Rare II 324 Vàng); opportunity tối thiểu **1.124 Vàng**. II +6 → III +0 tốn 500+2 đá và mất giá bán nguồn, opportunity **2.424 Vàng** nếu đá mua 800; rẻ hơn tự đập III +0→+6 ước 39.479 Vàng gồm đá mua. So sánh chỉ hợp khi đã sở hữu món nguồn; sunk cost cường hóa nguồn không được bỏ qua để gọi chuyển giao là nguồn tạo cấp miễn phí. I→III trực tiếp bị cấm. Không có vòng lặp bán/transfer: mỗi lệnh tiêu một source, target không sinh bản sao, giá bán không cộng tiền cường hóa, no-gain bị chặn; receipt chống replay.

**Đường đầu tư kỳ vọng:** Lv 5 vũ khí I +0; Lv 8 +2; Lv 10 +3; Lv 11 chọn vũ khí II hoặc giữ I +4 tới lúc đủ đá; Lv 13 vũ khí II +4 và nâng Áo nếu chịu đòn nhiều; Lv 15 II +4; Lv 17 trước Q11 II +4/+5, sau Q11 chuyển sang vũ khí Rare III rồi học đại chiêu; Lv 20 chính tuyến thường chỉ vũ khí III khoảng +4..+6, các món khác +0..+2. Đó là hồ sơ kiểm, không là requirement. Band I +4 đáng làm nếu chơi lâu trước II vì Tinh Hoa I và chuyển nguyên +4; II +6 đáng làm nếu sở hữu món tốt và muốn sang III +6. Đập toàn set +8 trước Q12 không hợp supply.

## Ngân sách farm và ví dụ kiểm chứng khi nhiều người đánh

Cận trên bán toàn bộ đồ rơi giả định bán trang bị/nguyên liệu/bình/đá trước khi dùng hoặc nhặt hụt; không đồng thời bán và dùng. Food I/II/III tốn **900/2.400/4.200 Vàng/h** nếu hiệu lực liên tục. Theo quái cố định Lv 2/4/8/10/13/16/20, Vàng trực tiếp + cận trên bán đồ khoảng **29,24/34,54/45,16/55,38/63,78/73,88/95,53 Vàng/quái thường**. Ngoài abs level gap 3, regular EXP/Gold/loot/Journey bằng 0. Quest/supply/evidence active step vẫn hoạt động ở level thấp.

Necklace III sell tăng **200 → 225 Vàng**, ngang Boots/Ring III; tổng sell sáu Common III tăng **1.725 → 1.750**. Giá mua II, sell II, rates và stat không đổi, nên transfer Rare II→III không đổi chi phí; normal Lv 20 vendor-all upper tăng khoảng **0,24 Vàng/kill**.

Boss expected sell-all pile: **1.000** Thỏi + **303,33** gear + **1.300** Stone + **150** Potion + **75** phù = **2.828,33 Vàng/world death** trước pickup/use; direct Gold/EXP = 0. Respawn 15 phút + fight 90–150 s tạo Thỏi khoảng 3.429–3.636 Vàng/h/world. Q12 turn-in 1.000 riêng từng character, không nhân vật phẩm world theo N người.

**Ví dụ kiểm chứng đóng góp:** mob Lv 4 HP107/EXP22/Gold15; Lv20 gây 90, Lv5 gây17. Người cao lệch level ≥4 nhận 0; người thấp nhận floor(22×17/107)=3 EXP, floor(15×17/107)=2 Gold, không nhận cả pool. TopDamage quá gap không tạo regular loot set. Người thấp 15,9% chưa đạt quest credit threshold 20%. Cùng level 70/37 damage nhận 14/7 EXP và 9/5 Gold; lẻ không redistribute. AFK damage0 không credit; cùng MapId / bán kính 8u / còn sống / đã gây sát thương trong 10s vẫn kiểm từng recipient. Quest chỉ active-step; Boss ≥10% và corpse area là predicate riêng. Không có Party. FFA chỉ nhặt, không sinh EXP/Gold/quest. Level lúc mob chết là bản chụp cho người có đóng góp, nên lên cấp ngay sau kill không làm mất quyền pickup; người tới sau xét level hiện tại.

## Nhiệm vụ, hành trình và Boss

Giữ **Q1–Q12**, Q9 tùy chọn; không thêm nhiệm vụ để lấp khoảng farm. Đối chiếu đúng SpawnSlot: Q4 năm Sói DS3–DS6; Q8 bốn Sói TA4+TA6 rồi Linh Biến `TA4.slot1`; Q10 sáu Đạo Tặc XN1–XN3, vật chứng ở kill thứ 2/4/6; Q11 3/3/4 Thạch XN4/5/6; Q12 sáu Cổ Vệ HT4+HT5. Vật chứng nhiệm vụ theo từng người, không RNG vô hạn. Q11 hoàn thành mới cho vũ khí Rare III và đại chiêu; không dùng chúng để tính độ khó Q11.

Mô hình hành trình 16 hạt giống/phái/kịch bản, Q9 bỏ qua, Boss/wait Q12 tính riêng. Tính thời gian đánh, travel cơ bản, turn-in, Food, Linh lực và chi phí cường hóa; chưa mô phỏng sát thương nhận, death, bag đầy, do dự UI, latency và nhặt hụt. Kết quả chỉ để phát hiện mâu thuẫn lớn, không nghiệm thu mục tiêu **150–240 phút**.

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

**Boss projection:** HP 32.000, DEF 25, EVA 60 (baseline GDD); hai người cân bằng Kiếm/Cung cùng đánh, cộng sát thương trung bình độc lập, chưa mô phỏng đồng thời né đòn/chết/threat. Tỷ lệ thời gian ra đòn hữu hiệu là giả định 50/65/75%; không auto-scale theo số người.

| Trang bị / người | Hai người, 50% / 65% / 75% ra đòn | Ba người, 65% | Bốn người, 65% |
| --- | ---: | ---: | ---: |
| Common III +0 | 151 / 116 / 101 s | 78 s | 58 s |
| Chính tuyến: Rare III vũ khí +6, đồ hỗn hợp | 119 / 91 / 79 s | 61 s | 46 s |
| Rare III +6 toàn bộ (đầu tư vừa) | 118 / 90 / 78 s | 60 s | 45 s |
| Rare III +8 toàn bộ (săn sau truyện) | 109 / 84 / 72 s | 56 s | 42 s |
| Epic III +8 toàn bộ (biên trần) | 104 / 80 / 69 s | 53 s | 40 s |

Hai người chính tuyến ở khoảng **50–65% thời gian ra đòn hữu hiệu** đạt 91–119 s, đúng mục tiêu 90–150 s. 75% cho 79 s là biên thuận lợi, cần cảnh né/telegraph thật để xem có quá nhanh. Bốn người endgame ~40 s ở 65% là hệ quả tự nhiên của cộng DPS, không lý do thêm auto-scale Boss; capacity/độ khó nhiều người phải đo. +8 làm nhanh hơn nhưng không vài giây và không cần cho Q12. Boss ATK 160, ba vùng Nham Thạch Rơi, lịch một action/các cooldown phải kiểm scene; mô hình không chứng minh đòn luôn né được. Demo HP 12.800/respawn 60 s chỉ là override nghiệm thu, không dùng làm cân bằng release.

<a id="review-decisions"></a>
<a id="open-decisions"></a>

# 5. Quyết định đã chốt và cổng kiểm khi triển khai

**Gameplay đã chốt:** GEAR-01 (trần +4/+6/+8, hai Tinh Hoa, chuyển giao); CONS-01 (hai hồi chiêu bình riêng); BOSS-02 (DEF25/ACC140/EVA60, ba vùng đá, một action/lịch ưu tiên). COOP-01, QUEST-02, BOSS-03, BOSS-01 và NAR-01 cũng đã chốt. **PVP-01 V6.1:** cược 1.000–10.000 bước 1.000, escrow cả hai trước MatchId, Food/Bình hợp lệ với quota 3+3, 120 s cả hai sống DRAW, fee/refund qua Spring receipt. **SAVE-01 V6.1:** PostgreSQL giữ tiến trình và MapId/HP/MP checkpoint; resume phiên còn trong RAM hoặc spawn SafeAnchor từ checkpoint. Các phép tính stat/loot/TTK PvE không đổi; mô hình PvP chưa tính hồi phục mới.

NAR-01: Đã tinh chỉnh Narrative theo hướng mở, gợi cảm giác tò mò và gỡ mâu thuẫn bối cảnh Xích Nham mà không tăng scope. Q9 tùy chọn, trang bị 18 dòng, bí kíp, không Party P0 và vòng Boss chung cũng đã có luật; không mở lại vì chưa chơi thử.

| ID / trạng thái | Điều cần đo hoặc làm | Baseline hiện tại | Điều kiện xem lại |
| --- | --- | --- | --- |
| BAL-01 — PLAYTEST | Giá trị bốn thuộc tính và build cực đoan | Bảng phân điểm/chỉ số §2, 95 điểm | Một build làm Q10/Q11 không thể qua dù dùng cơ chế bình thường, hoặc PvP có kết quả lệch quá xa |
| BAL-02 — PLAYTEST | Hồi phục và chi phí Food/Bình | Food III mỗi 2 s +4% HP/+2,5% MP; hai bình riêng 8 s | Trận farm phải đứng chờ Linh lực liên tục hoặc Vàng âm sau route hợp lệ |
| PHY-01 — PLAYTEST | Collider, platform, nhiều người | Bounds GDD, drop-through theo từng actor | Trúng đòn/đi xuyên sàn sai hoặc người khác làm đổi collision |
| CC-01 — PLAYTEST | Đóng Băng/Làm Chậm/Bỏng khi nhiều người | Cửa miễn Đóng Băng 3 s; Boss clock ×0,75, không stack | Boss mất khả năng ra đòn, người chơi không đọc được hiệu ứng, hoặc overlap gây unfair hit |
| SCOPE-01 — PLAYTEST | Mật độ, respawn, contention | 28 cụm/66 điểm, hồi 25 s, Linh 5%/cap1 | 2 người thiếu quái rõ hoặc 3–4 người chờ nhiều; benchmark trước claim capacity |
| GEAR-01 — BASELINE ĐÃ CHỐT | Chạy lại TTK, hit/crit, di chuyển giữa cụm, hành trình và Boss với Chí mạng Kiếm / Chính xác Cung / tốc chạy Giày; đo cảm giác +4/+6/+8 và preview | GDD §6, bảng kinh tế §4, hai Client + Dedicated Server playtest | Gear II/III, sức mạnh hai phái hoặc Tinh Hoa làm Boss/PvP/kinh tế lệch khi chơi thật |
| CONS-01 — BASELINE ĐÃ CHỐT | Thử nhầm phím/bình/refresh Food | HP và MP hồi chiêu riêng; tier đủ bù nhỏ nhất, Food thay cũ | Người chơi thường xuyên phí bình hoặc tutorial Q6 kẹt |
| SAVE-01 — V6.1 ĐÃ CHỐT | Checkpoint 30 s + portal/logout/death/revive/PvP; resume 15 s nếu phiên còn; mất phiên dùng SafeAnchor và HP/MP đã commit | Technical §5–7, sequence/generation, safe-zone coordinate validation | Checkpoint cũ đè mới, HP=0 hồi sinh miễn phí, load Arena hoặc mất Potion nhưng hồi máu không được checkpoint |
| PVP-01 — V6.1 ĐÃ CHỐT | Escrow hai người trước MatchId; 10 mức cược; Food tick/Bình quota; outcome và payout idempotent | GDD §8, Technical §4/§6, bảng arithmetic ở trên | HELD/ACTIVE mắc kẹt tiền; hết 120 s vẫn so HP; FORFEIT sai thời điểm; result retry trả Vàng hai lần |
| BOSS-02 — BASELINE ĐÃ CHỐT | Độ rộng vùng/nhịp báo trước đòn | 32.000 HP, DEF25/ACC140/EVA60, ba vùng đá, một action | Không thể né bằng kỹ năng di chuyển thường, overlap khó đọc, thời gian 2 người lệch xa 90–150 s |
| ART-01 — PLAYTEST | Đường đạn, aim, thời điểm hit, hình nhân vật | GDD/Technical art contract | Collider/hình lệch hoặc cảm giác chém/bắn khó đọc |
| LOOT-01 — PLAYTEST | Đá/trang bị sinh ra so lượng nhặt và sink | 8% đá thường, giá shop 800, §3–4 | Người chơi hợp level không đủ nguồn cho +4, hoặc +8 quá dễ trước story |
| QUEST-03 — PLAYTEST | Thời gian Lv1–20 và hồi phục khi lỗi | Mô hình ~115–131 phút, mục tiêu 150–240 phút | Playable journey vẫn quá nhanh/chậm sau tính đi lại, chết, UI và multiplayer |
| TECH-01 — SPIKE | Login → Character Select → one-time ticket → dedicated join; lease/duplicate session, internal service credential và backend outage | Technical §5–6: Spring xác minh account/character; Game Server xác minh ticket, một writer/character | Spike hai Client + backend/DB thật, thử duplicate join, consumed ticket, timeout trước/sau commit và server crash |

TECH-01/SAVE-01 và A15 còn cần chốt thứ tự Potion–hit–checkpoint, definite reject khác timeout, deathUtc/cap release ở terminal-pending và neo clock phiên↔UTC. [Các chuỗi probe cụ thể](2_HUYEN_LO_TECHNICAL.md#pending-ordering-probes) bổ sung acceptance của spike, chưa bằng chứng runtime hoặc approval schema. Các gap này không chặn local Kiếm bằng RAM fixture, nhưng phải giải trước G-D/reliability.

**Rủi ro kiến trúc V6.1:** auth/ticket/lease, checkpoint ordering/validation, reconnect grace, escrow orphan recovery, transaction N recipients, backend downtime và dedicated build/headless physics cần integration tests. Mốc giờ player-host/JSON cũ không còn dùng để cam kết lịch; spike đầu đo integration/latency, sau đó Technical mới đặt lại ngân sách. Quest, gear, combat PvE và world balance giữ số hiện tại; các luật PvP/recovery trên là user lock, không Open Decision.

[Review Art](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#art-review) đã tách nguyên tắc đủ dùng và con số chưa duyệt. CURRENT kiểm Kiếm/Q1–Q6 local rồi gate Dedicated hai Client trước production rộng; gate backend thật và toàn TARGET giữ trong [Roadmap](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#phase-gates). Cung/Boss/PvP/SAVE giữ TARGET dù DEFER implementation khỏi slice.

Các dòng đã có baseline không chặn việc bắt đầu code; ca PLAYTEST/SPIKE ghi rõ phải đo gì. Không tăng NeedEXP, HP quái hoặc tạo hệ thống mới từ mô hình thiếu cảnh thật.

<a id="art-open-decisions"></a>

## Sổ quyết định Art và các dependency — alias A01–A17

Đây là **nơi duy nhất giữ trạng thái quyết định** cho A01–A17, chuyển nguyên options/recommendation/cách chốt từ Art §25. Axx là alias lịch sử, gắn vào gate sẵn có dưới đây; không tạo một hệ luật cạnh tranh với BAL/ART/PHY/TECH. Trong bảng options cũ, `§10` chỉ [evidence timing đã chuyển](#art-combat-timing-evidence). P01–P15 là **ca thử** trong [Art §24](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#art-validation), không decision ID hay phase roadmap. Tất cả ca Unity vẫn **CHƯA CHẠY**.

| ID / vấn đề | Options và trade-off | Recommendation hiện tại | Cách chốt |
| --- | --- | --- | --- |
| A01 Ý nghĩa 26 frame | 26 ảnh tổng: chặt budget nhưng có thể thiếu Bow; 26 ô/profile dùng lại ảnh: tăng vài upper pose; hai bộ đầy đủ: dễ author nhưng dư locomotion | 26 ô baseline/profile, pose map chung, upper body riêng theo class; **cần duyệt lại diễn giải user-lock** | Duyệt design + P01/P03, không tự tuyên bố lock đã đổi |
| A02 Technique/weapon poses | Raster toàn bộ: sạch nhưng tốn; socket/skeletal thuần: ít ảnh nhưng rủi ro outline/khớp; hybrid: setup vừa và pose chính sạch | Hybrid body raster/weapon socket, góc sửa khi cần; 13–25 weapon outcomes chỉ là kịch bản | P01–P03 và chi phí P15 |
| A03 Boots | Giữ stat-only: không cost world; bỏ: giảm gear nhưng đổi economy/balance; footwear slot có visual thật: thêm layer | Giữ stat-only P0, LowerBody chứa footwear mỹ thuật | Có thể quyết bằng design; nếu bỏ phải tính lại P13/loot/traversal |
| A04 Skill/VFX concept | Nét Mạch Ấn, kiếm khí/băng hoặc linh ảnh: khác sắc thái/cost; linh ảnh lớn dễ bị hiểu là entity đánh thêm | Motif linh mạch ấm/lạnh, 4 active presets/6 profiles; signature bằng hình và nhịp | P03/P04/P14; concept cụ thể chưa khóa |
| A05 Multi-target Cung | Flight hiện hành: giữ collision nhưng impact lệch; đổi speed/spawn: giảm lệch, có thể đổi né; resolve đồng thời: đổi gameplay lớn | Giữ một fan release và impact server. Nếu bắt damage cùng nhịp, duyệt contract mới riêng | Design + P04/P12/P13, art không tự sửa |
| A06 Normal/timing/CD/proc | Giữ baseline; đổi duration pose nhưng giữ clock; tune hit/lock/CD nếu cảm giác sai | Giữ 0,80/0,90 s và MP/CD/proc; thử release Bow/timeline trước đổi damage/HP | P03/P13; các test ranges §10 không là acceptance mới |
| A07 Dummy | HP60/25 s cũ: rẻ nhưng chờ; HP60 + pool nhanh: giữ Q3 nhưng vẫn one-shot; HP lớn/scaling: test dài hơn nhưng phức tạp hoặc hại Q3 | Một prefab/yard, HP60; thử 3 điểm đứng/respawn 3–5 s; không DPS Meter/scaling P0 | P07; timer/count thay đổi cần duyệt GDD |
| A08 Corpse/flying loot | Fade nhanh: sạch; hold lâu: thấy kill nhưng clutter; rơi visual: tự nhiên; tan tại chỗ: rẻ; chiếu loot xuống nền: reachable nhưng cần luật server | Ground hold 0,5–1,5 s/fade 0,3–0,5 s; Ong rơi visual/fallback tan; loot server chọn điểm hợp lệ | P06/P09; corpse art và loot gameplay quyết riêng |
| A09 Terrain/building | Cell 16/32/64 px; tile-only hoặc mini kit/full asset; ít variant giảm cost nhưng lặp texture | Thử 32 px, 17 semantic + 4 cosmetic/họ; kit cho motif lặp, full asset cho landmark độc nhất | P08/P09/P15; blockout quyết số module thật |
| A10 Tầng/AI/projectile mask | Ground cùng lane reachable: rẻ; navigation đổi tầng: linh hoạt nhưng tăng logic; platform chặn đạn/cho xuyên: đều cần cue và balance | Author mob trên lane AI hiện có hỗ trợ, hybrid fallback; solid chặn nhất quán; one-way mask chưa khóa | Technical/Design + P09; không mặc định mob Jump |
| A11 Anticipation online | Chỉ local pose: rẻ nhưng chờ projectile; projectile tạm: mượt hơn, cần xử lý ghost/dedup; rollback combat: tăng scope | Pose/âm/cast cosmetic ngay, projectile chính/result authoritative; projectile tạm chỉ khi đo thấy cần | P12; không thêm prediction/rollback physics P0 |
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
| A03 | GEAR-01 / BAL-01 / LOOT-01 | BASELINE giữ sáu slots/Boots stat-only theo GDD. Option bỏ/visual Boots vẫn OPEN, chưa duyệt thay gameplay. |
| A04 | ART-01 / CC-01 | APPROVED nguyên tắc phân biệt cast/main VFX/impact/status; OPEN concept cụ thể, số frame và motif. Kiếm CURRENT, Cung/Boss thử ở phase sau. |
| A05 | ART-01 / PHY-01 / BAL-01 | BASELINE giữ SnapshotSpread/flight/fallback/status dedup hiện hành. OPEN normal Cung executor/speed còn thiếu, tốc độ flight player projectile chưa có số và option đổi spread đồng thời; không tự chốt hitscan. |
| A06 | ART-01 / BAL-02 / GEAR-01 | BASELINE giữ interval/MP/CD/proc; OPEN các sensitivity ranges. Timing pose bám clock đã được chấp nhận, retune gameplay cần evidence. |
| A07 | ART-01 / QUEST-03 / SCOPE-01 | BASELINE HP60/25 s Q3; OPEN DEF/EVA, số điểm đứng và timer nhanh. Q3/Q6 solo CURRENT; contention thử ở gate mạng. |
| A08 | ART-01 / PHY-01 / LOOT-01 | APPROVED corpse tách gameplay terminal/respawn. OPEN hold/fade exact, flying fall và điểm loot server chọn. |
| A09 | ART-01 / PHY-01 | APPROVED cue solid/one-way/prop và surface đọc được; OPEN tile cell/module count/cách dựng kit. Room CURRENT, full families DEFERRED. |
| A10 | PHY-01 / ART-01 | OPEN ground route đổi tầng, projectile mask one-way. Author lane reachable làm baseline thử, không thêm mob Jump/navigation tự động. |
| A11 | TECH-01 / ART-01 | BASELINE presentation chỉ đọc authority; OPEN immediate local anticipation/tentative projectile và schema. Local Authority CURRENT; Dedicated/2-client gate kế tiếp. |
| A12 | ART-01 / PHY-01 | OPEN camera/reference resolution/UI scale, pixel snapping và icon size; PPU32 giữ nguyên. |
| A13 | TECH-01 / ART-01 | OPEN preview default hoặc exact gear/read-only summary; Login/Select TARGET P0, DEFERRED khỏi VS-1. |
| A14 | ART-01 / PHY-01 / BAL-01 | OPEN quyền chạy/nhảy lúc cast và cách phối upper/lower pose. Không thêm movement lock để giữ số ảnh. |
| A15 | TECH-01 / SAVE-01 / ART-01 | BASELINE terminal-pending không reward/respawn trước commit; OPEN event presentation trước ACK và deathUtc t0 contract chi tiết. |
| A16 | BOSS-02 / CC-01 / ART-01 | DEFERRED art prototype Boss; OPEN canvas/28 pose/roar. Behavior, telegraph và Cuồng đã có baseline GDD, không bị bỏ. |
| A17 | ART-01 / SCOPE-01 / TECH-01 | OPEN tổng count/giờ thực; CURRENT đo Kiếm/room/kit và % art dùng được. S0 chỉ kịch bản, thay estimate sau evidence. |

Không có proposal gameplay nào bị chuyển sang LOCKED trong lượt này. **REJECTED cho CURRENT**: production hàng loạt trước gate, dùng AnimationEvent/VFX để sinh damage, dùng mô phỏng làm acceptance, hoặc gọi Cung là P1. Các lựa chọn thẩm mỹ chưa có prototype tiếp tục OPEN; việc chưa làm là DEFERRED, không phải loại bỏ khỏi TARGET.

<a id="research-ideas"></a>
<a id="legacy-provenance"></a>

# 6. Nguồn tham khảo và đề xuất P1

Các đề xuất dưới đây được giữ để không mất thiết kế đang cân nhắc. **P1 / PROPOSAL, chưa duyệt triển khai**, không cộng vào balance / acceptance P0. Chọn hoặc bỏ sau core gate theo [GDD scope](1_HUYEN_LO_GDD.md#vision); không phải Open Decision chặn code P0.

| ID | Candidate | Input proposal được giữ lại |
| --- | --- | --- |
| SCOPE-03 | **Chiến Ý — Kiếm / Ưng Nhãn Cường Hóa — Cung** | Buff Lv 10, phím R; duration 10 s, skill CD 40 s, MP 15. Chiến Ý: +15% ATK, +5% speed. Ưng Nhãn Cường Hóa: +10 điểm % CritChance, +15% range. Không phải nội tại Ưng Nhãn Lv 5; không thêm active P0. Acquisition / stacking và exact values cần chốt nếu chọn P1. |
| SCOPE-04 | Linh Giáp / Vỡ Thế | Shield 1.000 và Groggy là proposal cũ; chưa có shield-break / CC contract P0. Cần kiểm lại Boss / CC / workload nếu chọn; không tự lấy số này làm Boss data. |

**Legacy provenance compact:** NSO reference đã khai thác, không current authority. Trace từ `research/SRC NSOACE FIX/`: `Char.initMenu/finishTask` (NPC turn-in / bag checks), equip / use callbacks và `AbilityFromEquip` (onboarding / +4), `Mob.dead` (quest assist / loot input), Part / TileMap (modular / one-way). Availability instances là historical input; Boss hiện hành shared world. Không dùng reference chứng minh crash atomicity, rates hoặc balance. Lịch sử chi tiết nằm trong Git; pattern đã nhận là design Huyền Lộ.
