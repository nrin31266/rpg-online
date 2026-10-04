# Huyền Lộ — lịch sử prototype và chỉ mục bằng chứng

Prototype là disposable/reference; findings chưa là luật production hoặc feel acceptance. Các archive refs chỉ local, **không push `archive/*`**.

## Git safety — Phase A

Baseline HEAD `50f05ed`; 15 commit chưa push. Working tree có 12 file chưa commit, đã backup riêng cả patch và nội dung/hash, không đưa thay đổi của phiên khác vào commit phase này. Thư mục lồng 151 file / 89.268.405 bytes không track, archive ngoài repo đã kiểm từng payload/hash; **chưa xóa**, theo mục 0.6. Bundle lịch sử đã verify. Backup: `/home/nguyenvanrin/PersonalProjects/huyenlo_backup/2026-10-04-input-probes`. Tag `v6.2.7-full-checkpoint` trỏ HEAD trước task (có cả hai commit hình sau V6.2.7), không phải tag nghiệm thu runtime. `46006c4`/`13035d3` là hash lịch sử ngoài nhánh main hiện tại; vẫn được giữ trong bundle, chưa tự đổi docs thành tag thiếu kiểm chứng.

Hook chia sẻ: `git config core.hooksPath .githooks`. Kiểm index blobs trước commit; cấm MP4/DOCX/log, size >500.000 bytes trừ allowlist theo thư mục Assets của từng Unity project. XML nhỏ không bị cấm toàn repo (test XML prototype đã ignore). Khi có CI, chạy `python3 .githooks/check_staged_files.py --all`; repo hiện không có CI, không tự thêm workflow. README giữ nguyên theo mục 0; hướng dẫn đặt tại đây.

`python3 tools/check_doc_links.py`: baseline 326 local links, 0 anchor gãy, 0 file thiếu. Task A5 media ignore đã có; chỉ thêm ignore hẹp cho DOCX gốc. Không untrack ảnh/evidence trước Phase H đã duyệt.

## Chỉ mục evidence nhị phân

Agent chỉ tính hash/dung lượng; chưa upload. Cột nơi lưu dành cho chủ dự án. File TXT/JSON nhỏ tiếp tục giữ trong repo. Trước untrack/xóa, cần bản sao ngoài repo và kiểm hash.

Media hiện tại đã không được track ở HEAD; các link cũ vẫn dùng file local. Chỉ mục lấy từ file thực, không từ `git ls-files`; 48 payload đã backup/hash-verify trong `evidence-media.tar.gz` ngoài repo. Chưa upload hoặc xóa local media.

| Phiên bản | Tên file | SHA256 | Bytes | Nơi lưu ngoài repo |
| --- | --- | --- | ---: | --- |
| <a id="evidence-v6-2-2-91acfb7e59b3"></a>V6_2_2 | combat.png | `91acfb7e59b3b0fe9cc87e438881497a77d2e47a08ee9a9ff98a9c3a9236ced6` | 62323 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-2-832bb2fafd76"></a>V6_2_2 | continuous-route.log | `832bb2fafd768028ae554217e1fc32ab63249c6a307aee1eac2bcfe40f5124af` | 6164 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-2-7455f7efa025"></a>V6_2_2 | continuous-route.mp4 | `7455f7efa025f497627ad503e0fa10ebe8acb441cda045ccc1ae77943cf35326` | 4916196 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-2-a140a0b67d72"></a>V6_2_2 | editmode.xml | `a140a0b67d72479c853dee15cd5eede83db76915c462221ece193cac306c349e` | 30601 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-2-66887d1a4d93"></a>V6_2_2 | keyboard-npc.png | `66887d1a4d93aa767e13ba88c1cf0994ed452dd8fdb36ef0fe15bf3c62fb6920` | 99958 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-2-56b435d683a8"></a>V6_2_2 | playmode.xml | `56b435d683a8814cc9297d788d782bb497b59c2ece43b967420679d6309fc3fd` | 14291 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-3-6912d9c1dfc2"></a>V6_2_3 | map-layout.png | `6912d9c1dfc2a577a393c2d03fdd1b11ba768eb4dcc2c1e9025db0a73bdef67f` | 244625 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-3-f09d45a8e321"></a>V6_2_3 | ui-layout.png | `f09d45a8e321075d4ac08952155263afc26e3e080a4023e4efe4892ca28abe2c` | 176781 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-4-cfe49aacb8b9"></a>V6_2_4 | basin-runtime.png | `cfe49aacb8b990e955e0e170a28e305e27b248d95d832607659c3c292cdf7c5a` | 57166 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-4-ad21fb1d41ca"></a>V6_2_4 | equipment-runtime.png | `ad21fb1d41ca6fc32df04b22ba4f411a05a65aa99aa40100845fb27912927968` | 87301 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-4-9c498826ae58"></a>V6_2_4 | map-layout.png | `9c498826ae58888cd8d01996185678cbb7b7e1591778935f123560d5619ddb57` | 291957 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-4-e5a97a7ee355"></a>V6_2_4 | ui-layout.png | `e5a97a7ee3556dc81c91d8e439775b576b70eb3d46c5d9eecea98fcae554a60d` | 341618 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-5-1187d6f9f22b"></a>V6_2_5 | academy-floor.png | `1187d6f9f22b5bf96af813a6c4d159e96d2cdd90134e174e674537f984ba5f12` | 63004 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-5-cac531e206a4"></a>V6_2_5 | dry-pack.png | `cac531e206a407dbcdbf97526b6ea1497fb13faebc0278523cb9bf89c8bb49d9` | 63401 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-5-b3255faf4826"></a>V6_2_5 | mist-stream.png | `b3255faf4826ead41a063503911a40bbd2bf816980adb308d59e0c890a2279ad` | 55314 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-5-63769d2e524c"></a>V6_2_5 | mist-wading.png | `63769d2e524cfc374131722705b41f335bd095d29dc9dc3b63d35f0cdece73cf` | 56747 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-5-e1eaa0d076fc"></a>V6_2_5 | village.png | `e1eaa0d076fcb0c6a592f3cc5b2e6528dbd79c7fa028223853047077fa9de1bf` | 60409 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-6-be3216664c3c"></a>V6_2_6 | academy-high-ground.png | `be3216664c3c853bc56836cfe48278e190cee53388d77a7d14c4d0ab0109d9fd` | 63437 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-6-502002a1cacd"></a>V6_2_6 | academy-high-npc.png | `502002a1cacdd19002a6304b484cafc19dbf2eb7eef42ad2a59551899d668227` | 80866 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-6-3bfed78f4223"></a>V6_2_6 | mist-lower-passage.png | `3bfed78f42234ada35dd54f81074827dfe96e8a19b6d48aa218eef2d00390ed7` | 60501 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-6-6b6fe15b1f42"></a>V6_2_6 | mist-solid-bridge-after-s.png | `6b6fe15b1f42fd0b374d34594caf9d2cb92150ef865ac58e72e7005d28ae07ff` | 52748 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-6-04ba9aaa398c"></a>V6_2_6 | mist-upper-earth.png | `04ba9aaa398cd71925baf41de61cc89453f9973c2e2e5b519b4c1c6c45223a42` | 61003 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-6-4b8dfc4f604f"></a>V6_2_6 | mist-upper-pack.png | `4b8dfc4f604fc06ac66d6a72bab33a0550e3c89a6c4041db6b83a3619e6cee8d` | 59792 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-6-575d48207cfb"></a>V6_2_6 | village-real-hill.png | `575d48207cfb4ffda4b7c26d330925cbaad72ed458bb90220eb20dc20b47eb24` | 63745 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-7-c6417a2faf7a"></a>V6_2_7 | academy-npc.png | `c6417a2faf7a0af2624ad3f60dae6f2ec46b0334d49e58df592399765569d19d` | 80822 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-7-509cf70747f8"></a>V6_2_7 | academy-rear-earth.png | `509cf70747f83b71e943f985492486c48e22c8af10be8cef4058a7046377a97c` | 63451 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-7-66a61248c284"></a>V6_2_7 | bridge-after-drop.png | `66a61248c284803014ada560378c68a777b6b50ce5fce1e53786d9afbc24aa90` | 65404 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-7-7309e033940e"></a>V6_2_7 | bridge-clearance.png | `7309e033940ee065401687a767a04b0d9c98e2cbd8a02c8a872ec37221f1900f` | 65369 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-7-40e393301647"></a>V6_2_7 | bridge-east-ramp.png | `40e3933016475bd36b7bec464d6e705d7d4aa14f20bf53bafbf367e2b4d3015c` | 65298 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-7-2fc7ef218b42"></a>V6_2_7 | bridge-west-return.png | `2fc7ef218b42848630592215e6d6cbfb36c011925d57ecb20f9c92033e3ca83f` | 55768 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-7-c2a1915f388a"></a>V6_2_7 | mist-bridge.png | `c2a1915f388a5f53b956b5ed6834d79af6e9f6e5cb00c308fff5ceea611e356f` | 66208 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-7-17491543efd7"></a>V6_2_7 | mist-riverbed.png | `17491543efd717cf83ba97e688529df9c852fee9f9e1c2d62050b20aaf82fc6a` | 65956 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-7-06ecc12dedf4"></a>V6_2_7 | mist-waterfall.png | `06ecc12dedf4f0a8c1044a7b8322ed06fe93426e64312edea5eaa2d996d91ee0` | 69245 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-7-e5aa9e1ae645"></a>V6_2_7 | rear-earth-after-drop.png | `e5aa9e1ae64547d09b9905cb6d76232e5e0316ee0711e9e4b86ee8b17adc3a54` | 62903 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-7-4cee9f818a65"></a>V6_2_7 | rear-earth-lower-lane.png | `4cee9f818a6595ef88fb4c872ceaeb2dac039df3dee38197983165ffbddb08d4` | 65319 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-7-7731401d7b12"></a>V6_2_7 | rear-earth-upper.png | `7731401d7b12701df1e0bce91e88ba33510aa0c90bfed32994a4e794693288b5` | 63596 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-v6-2-7-b4deca4c65d7"></a>V6_2_7 | village-real-hill.png | `b4deca4c65d70fc35ab8ddc0f3bed451f6225262eae76168eb71bbef6d4d4c88` | 58384 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-vs1-endtoend-894c3c221671"></a>VS1_EndToEnd | combat-focus.png | `894c3c221671ccc4dc57575a88feaab1947b5e8f2c681726b4131ad90bdaec61` | 60343 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-vs1-endtoend-38fa473e7a72"></a>VS1_EndToEnd | continuous-route.log | `38fa473e7a7213dab8376145ea513e0ceb46f035e79f15476a1a0b670ebb9c42` | 6035 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-vs1-endtoend-82d1046a1cb8"></a>VS1_EndToEnd | continuous-route.mp4 | `82d1046a1cb87e75624a88e61164c330f0a1090d505541ae47889af57f0f8e32` | 2198375 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-vs1-endtoend-1e893ec3b408"></a>VS1_EndToEnd | editmode.xml | `1e893ec3b408e050fceb8f17e35096ea4100cf3e1d94b0b005bbe1d63ca86e51` | 24323 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-vs1-endtoend-266ebeb49a62"></a>VS1_EndToEnd | input-physics.xml | `266ebeb49a6214408e99b4732efbca6be65664de714c407bdbe61cc6e10a2837` | 3409 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-vs1-endtoend-53c701c94c0d"></a>VS1_EndToEnd | linux-build.log | `53c701c94c0d3d0b5bd025a2e77d0b232b0df50a528bf851c5b831451ade04b7` | 258613 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-vs1-endtoend-914ba0417a50"></a>VS1_EndToEnd | migration-editmode.xml | `914ba0417a50338c53d058e9008534cac72e5a0cc1ac75fdfc4c56730348c443` | 24309 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-vs1-endtoend-ec83e949e86b"></a>VS1_EndToEnd | onboarding.png | `ec83e949e86b3756e04a532603af624d4042ff8d58807fd5c0874c0d32aa35c2` | 68041 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-vs1-endtoend-ee0eba494e11"></a>VS1_EndToEnd | playmode.xml | `ee0eba494e1113a5b1748a1d27fa4920525a0b605725c562e7ec8892a3564901` | 11820 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-vs1-endtoend-d525b9622d22"></a>VS1_EndToEnd | standalone.log | `d525b9622d22db79c0fd1a59dfb5e198cbf9372b9ce7129dd466c205963ca2fd` | 18385 | CHỜ CHỦ DỰ ÁN ĐIỀN |
| <a id="evidence-vs1-endtoend-83ff9361d6e4"></a>VS1_EndToEnd | sword-q6.png | `83ff9361d6e44529a112326e1935e6a79785d2da776168ddd884d051cdb42d36` | 79796 | CHỜ CHỦ DỰ ÁN ĐIỀN |

## Phase B — baseline trước sửa hành vi

[Metrics](PrototypeEvidence/Baseline/metrics.json) và [source/test hashes](PrototypeEvidence/Baseline/validation.json): seed731, dt0,02. EditMode59/59 gồm recorder mới (58 test cũ), PlayMode10/10. Mã baseline có thay đổi chưa commit từ phiên trước; exact source được giữ trong backup ngoài repo và hash manifest, không gọi HEAD là toàn bộ source baseline.

| Chỉ số | Baseline đo |
| --- | ---: |
| Đổi Facing hoặc dấu vận tốc, mỗi quái / 5 s | 3,3333 |
| Press bị reject / 60 s | 119/171 = 69,5906% |
| Dead time basic Tân Lữ | 74% |
| HP mất / 10 s, 3 Sói, player đứng đánh | 131 |
| TTK Tân Lữ Lv4 / 1 Sói | 4,96 s |
| TTK Tân Lữ Lv3 / 3 Dummy HP60 | 9,78 s |
| Approach bị hủy bởi stale held axis | 20/20 |
| Auto-approach lọt EdgeExit | 1/1 |

Reject recorder dùng Dummy HP60, tạo life lab kế ngay sau chết để luôn có mục tiêu; không đổi HP catalog hoặc respawn world. Crowd dùng ba PROBE7 Sói với HP107, Lv5 cân bằng/Common I; player sống hết10s, có kill thực. TTK dùng cùng lịch press seed và đi giữa mục tiêu, không unlimited approach. Held/exit là replay thứ tự của Host trên domain, chưa là bằng chứng keyboard hardware.

**CHƯA ĐO bởi người thật:** rollover D+Space+1(+2) trên desktop/laptop; WASD/mũi tên với4/5; feel0,70s normal speed. Rủi ro: cadence press0,35±0,1s nhanh hơn CD, reject≤10% có thể không khả thi; damage intake còn phụ thuộc thời điểm chết của quái, không riêng AI safe window. Không có luật/số GDD được thay.

## Phase D — prototype sau flag, mặc định OFF

Commit `cbd73f9`: ProbeConfig/domain/input adapter/HUD và presets. Buffer 0,18s chỉ nhận latest intent gần lúc skill sẵn; stale held axis bị bỏ qua trong pending rồi hoạt động lại; arrival có enum reason, không commit khi fail; approach chặn exit; Esc theo ngữ cảnh; 4/5 và Tab/Shift+Tab sau flag. Basic Tân Lữ thử0,70/0,32/hit0,10, không đổi S1/S2/S3 hoặc stat/EXP/speed/range quái. StableMelee thử free-space0,9×range, side hysteresis1,5s, phase SpawnSlot+life, wolf retreat0,8u trong recovery, Nấm không retreat và quadratic separation. Không token/slot/formation/shoving; windup không steering. Ranged giả đi cùng pipeline; chưa implement Cung hoặc PvP.

F8 → mục cuối “A/B probes” → bật riêng từng flag hoặc tất cả; Enter/chuột cùng command. Fresh app mặc định mọi flag OFF; reset debug giữ lựa chọn A/B của phiên, vẫn có nhãn fixture. Mở panel debug tạm dừng mô phỏng local. Test adapter dùng thiết bị Input System ảo; không tự gọi đó là keyboard hardware.

Các sửa cảnh quan có sẵn trong working tree được backup và giữ nguyên; phần code chồng file được stage riêng bằng diff với snapshot đầu task, kiểm residual khớp nguyên thay đổi trước task. Không đưa đại tu cảnh quan vào commit probe hoặc gọi nó là kết quả của task này. Sửa HUD Tân Lữ ở bước kiểm thử để highlight/CD đọc đúng profile A/B thay vì luôn trỏ profile baseline.

## Phase E — kết quả A/B và giới hạn

[So ngưỡng](PrototypeEvidence/InputProbes/comparison.json) · [All flags ON](PrototypeEvidence/InputProbes/metrics.json) · [Chỉ AI](PrototypeEvidence/InputProbes/metrics-melee-only.json) · [Chỉ input](PrototypeEvidence/InputProbes/metrics-input-only.json) · [Validation/source hashes](PrototypeEvidence/InputProbes/validation.json). Seed731/dt0,02 và kịch bản Phase B giữ nguyên; OFF replay trùng baseline. Test correctness và metric threshold là hai kết quả riêng: test PASS không làm các FAIL dưới thành PASS.

| Chỉ số | Baseline OFF | All ON | Ngưỡng / kết quả |
| --- | ---: | ---: | --- |
| Facing hoặc velocity sign flip / quái /5s | 3,3333 | 6,1667 | ≤2: **FAIL, tệ hơn** |
| Reject / press60s | 119/171 (69,5906%) | 88/171 (51,4620%) | ≤10%: **FAIL** |
| Dead time basic Tân Lữ | 74% | 54,2857% | ≤55%: PASS arithmetic |
| HP mất /10s, 3 Sói | 131 | 166 (+26,7176%) | ±10%: **FAIL** |
| TTK Lv4/1 Sói | 4,96s | 3,98s (−19,7581%) | Báo cáo, không ngưỡng |
| TTK Lv3/3 Dummy HP60 | 9,78s | 6,42s (−34,3558%) | Báo cáo, không ngưỡng |
| Hủy do stale held axis | 20/20 | 0/20 | PASS domain replay + virtual adapter |
| Auto-approach lọt exit | 1/1 | 0/1 | PASS domain fixture + virtual adapter |

**Tách nguyên nhân:** chỉ StableMelee ON cho5,8333 flips/quái/5s và142HP (+8,397%); chỉ nhóm input ON cho3,3333 flips và122HP (−6,870%). AI candidate vẫn không ổn định về đổi dấu vận tốc dù không lật cánh ngay sau cắn. Retreat và separation correction có thể đổi dấu liên tục; deadband facing không bảo đảm velocity ổn định. Đây là hypothesis cần trajectory review, chưa causal proof. Với cả hai nhóm ON, damage lệch26,7% nên không duyệt package hoặc đổi baseline. Không chỉnh interval/speed/range/HP để ép đạt.

**Reject theo cadence:** lịch171 press/60s nhanh hơn CD0,70s: kể cả target luôn hợp lệ, không thể thực thi tất cả press. Trần khoảng86 cast/60s nghĩa là reject hoặc không thực thi khoảng50% nếu không giữ queue dài; ngưỡng10% không phù hợp kịch bản spam này. Latest buffer có thể giảm hụt press nhưng không tạo cooldown bypass. Không đổi định nghĩa reject để tô đẹp kết quả.

**Q3–Q5:** nhịp Tân Lữ mới rút ngắn mẫu3 Dummy khoảng3,36s; mẫu1 Sói nhanh hơn0,98s khi all ON. AI riêng lại kéo mẫu1 Sói4,96→7,38s; input riêng4,96→2,96s, cho thấy interaction không thể suy bằng cộng delta. HP60 Dummy, objective count, EXP0 Dummy/15 Nấm/22 Sói, level gap và quest top-up không đổi trong code. Chưa đo lại fresh toàn Q3–Q5 ở1× cho mỗi nhóm; không nhân TTK một Sói thành dự báo journey/EXP mỗi giờ hoặc cập nhật bảng balance cũ. Điều đó còn nằm trong CMB-01.

**Giới hạn damage:** Sói có HP hữu hạn và chết thật; movement ảnh hưởng target/evade và thứ tự RNG. ±10% là kiểm cần thiết của probe, không đủ chứng minh “không thêm safe window”. Các deadline interval không bị kéo dài trong code nhưng áp lực thực và loop retreat cần playtest/trace bổ sung. Cung/PvP, backend/Dedicated và geometry production chưa được nghiệm thu.

### Checklist cần người thật — CHƯA ĐO

- Desktop và laptop: giữ D+Space+1, rồi thêm2; kiểm mỗi physical press có đúng một intent, không double/repeat hoặc mất phím. Phân biệt giới hạn keyboard rollover với cancellation đúng vì Jump/new KeyDown.
- WASD và mũi tên: dùng4/5 lúc đang chạy/cần hồi; đo bấm nhầm, rời tay, có đọc được glyph4/H và5/M không.
- 1× OFF/ON basic0,70s: thử3 Dummy, Q4 Nấm và Q5 Sói; đánh giá windup/recovery/di chuyển, không chỉ cảm giác spam nhanh hơn.
- Crowd1/2/4, mép lane và player đi xuyên: xem silhouette, retreat/velocity jitter, pressure, side lock và target pinned bằng Tab.
- Esc: NPC submenu/menu, pending, focus, no-op; mở/đóng UI không resume approach cũ. Thử blocked exit rồi manual chuyển map.

Chưa có người thật thực hiện checklist; **G-L PARTIAL, không PASS**. Findings chuyển thành yêu cầu kiểm cho G-B, không mang throwaway classes vào production. Câu hỏi chủ dự án: giữ probe OFF và review AI/cadence trước mọi sync luật; Phase H còn cần duyệt riêng. Không sửa GDD/Technical/Art/README.

## Chỉ mục evidence TXT/JSON lịch sử

Các file nhỏ tiếp tục ở Git; link lịch sử đi qua chỉ mục để phân biệt hồ sơ với acceptance hiện tại.

| File | SHA256 | Bytes | Vị trí trong repo |
| --- | --- | ---: | --- |
| <a id="evidence-small-ae3ddbad7ec4"></a>PrototypeEvidence/V6_2_5/document-audit.json | `ae3ddbad7ec4277f15e9033a888412fefa6ad4ae68d3d76246d2f7e3c525945c` | 5102 | [File](PrototypeEvidence/V6_2_5/document-audit.json) |
| <a id="evidence-small-392f4b2e387b"></a>PrototypeEvidence/V6_2_5/source-destination-sample.json | `392f4b2e387bd74577ab54a5a454e8b60b073588beb898d30c0ade251cbd85c8` | 3181 | [File](PrototypeEvidence/V6_2_5/source-destination-sample.json) |
| <a id="evidence-small-23d16d500d0e"></a>PrototypeEvidence/V6_2_5/validation.json | `23d16d500d0ede38dd229a94df46d26761657bff64eb80b89051b263eed66008` | 4985 | [File](PrototypeEvidence/V6_2_5/validation.json) |
| <a id="evidence-small-080c1b72b86f"></a>PrototypeEvidence/V6_2_6/build-summary.txt | `080c1b72b86f094ef88b904cd100e2c9d23bd7d090532fd7c65756d1db8c49e0` | 808 | [File](PrototypeEvidence/V6_2_6/build-summary.txt) |
| <a id="evidence-small-4f633c9d68cb"></a>PrototypeEvidence/V6_2_6/document-audit.json | `4f633c9d68cb0bacd63ee905c69f92106b8b7c1c9248b566d3fe992b72ca6d14` | 4341 | [File](PrototypeEvidence/V6_2_6/document-audit.json) |
| <a id="evidence-small-ff856246be01"></a>PrototypeEvidence/V6_2_6/validation.json | `ff856246be01b9cbfa5b0dd383ca1ea1111cb891c4e468d977c9734e89def019` | 9721 | [File](PrototypeEvidence/V6_2_6/validation.json) |
| <a id="evidence-small-257a28f6fc1a"></a>PrototypeEvidence/V6_2_7/build-summary.txt | `257a28f6fc1a11d942ecf68c7ea4c3b7cf9f71402fd519cba2929c106c231793` | 1029 | [File](PrototypeEvidence/V6_2_7/build-summary.txt) |
| <a id="evidence-small-114942163ccf"></a>PrototypeEvidence/V6_2_7/document-audit.json | `114942163ccff28c392983d199784798089ac9cb7b3075513009592a7abb1793` | 4341 | [File](PrototypeEvidence/V6_2_7/document-audit.json) |
| <a id="evidence-small-1c3b52fbfcf1"></a>PrototypeEvidence/V6_2_7/route-result.txt | `1c3b52fbfcf1050ba54932357a73e25281af66de991eb7c1087759c5cce33622` | 250 | [File](PrototypeEvidence/V6_2_7/route-result.txt) |
| <a id="evidence-small-8a5466319cec"></a>PrototypeEvidence/V6_2_7/validation.json | `8a5466319cec56c3e29378e9f132b21fbf43f10f848e71be233c1eb86bbf391b` | 10435 | [File](PrototypeEvidence/V6_2_7/validation.json) |

## Archive checkpoints — chỉ local

| Hash lịch sử | Tag local | Trạng thái |
| --- | --- | --- |
| `46006c4` | `archive/checkpoint-46006c4` | Local-only archive, không có trên origin; chưa push |
| `13035d3` | `archive/checkpoint-13035d3` | Local-only archive, không có trên origin; chưa push |
| `52d7a4a` | `archive/checkpoint-52d7a4a` | Local-only archive, không có trên origin; chưa push |

## Phase F — hồ sơ prototype chuyển từ Roadmap §10

Giữ nguyên toàn bộ văn bản nguồn, chỉ đổi đường dẫn/hash sang routing archive. §10 có chỉnh sửa cảnh quan chưa commit từ trước task; chúng được bảo toàn tại đây cùng nhãn lịch sử, không được xem như kết quả triển khai/benchmark của task này. Mục có tiêu đề V6.2.8 là bản nháp chưa xác minh nằm trong nguồn V6.2.7; không nâng CURRENT hoặc công nhận các claim “zero allocation/triệt tiêu lag”. V6.2.4–V6.2.6 đã bị từ chối hình; các PASS cũ chỉ có nghĩa cho revision/phương pháp ghi kèm.

<a id="prototype-visual-review"></a>

# 10. Hồ sơ bản mẫu — không phải luật production

Mục này sở hữu layout/tọa độ mock, lỗi runtime, fixtures, capture và thông số thử. Docs 1–4 giữ luật, contract, reasoning balance và production visual requirements; không lấy một lần sửa mock để khóa design mới. V6.2.4, V6.2.5 và V6.2.6 bị user từ chối về địa hình dù tests/route pass; các hình và mô tả cũ dưới đây chỉ là lịch sử, **không mô tả bản hiện tại và không phải visual acceptance**.

## V6.2.7 — CURRENT: đất cùng màu tầng, mặt cỏ mượt, cầu mặt vuông nối liền hai bờ, đại tu cảnh quan 3 map và mô hình quái/nhân vật

Feedback hoàn thiện: các phần đất dùng chung màu cho toàn bộ các tầng; mặt cỏ để đứng ở trên được làm mượt và liền mạch; cầu không dùng khối chéo (bỏ dốc nghiêng và thanh chống chéo xoay độ), sử dụng toàn bộ mặt vuông/chữ nhật phẳng và nối liền mạch hai bờ cùng cao độ để đi qua không bị chắn phải nhảy. Đại tu toàn diện phong cảnh, kiến trúc 3 map theo đúng lore GDD và Art spec, nâng cấp rig nhân vật và mô hình quái vật đa phần.

- **Màu đất đồng nhất:** Tất cả các khối đất (tiền cảnh solid và đất tầng sau) dùng chung palette màu đất (`#706145`); thân đất tầng sau vẽ đầy xuống nền và nằm sau actor (sortingOrder −14); không collider ở thân/sườn, chỉ mặt trên đỡ nhân vật.
- **Mặt cỏ mượt:** Bỏ mép đứt đoạn; toàn bộ các mặt đỡ nhân vật đều có thanh cỏ xanh mượt (`#6E9C5C`) kèm dải highlight (`#85B86B`) chạy liền suốt bề rộng, tạo cảm giác phẳng mịn và nhất quán giữa các tầng.
- **Cầu mặt vuông nối liền hai bờ:** x66…82, sàn solid top1,2. Hai đầu cầu nối trực tiếp với đường hai bờ ở cùng cao độ (bờ tây `DS5 lower passage` top1,2 và bờ đông `DS6 clearing` top1,2), đi qua hai chiều hoàn toàn phẳng, không bị chắn, không cần nhảy. Trụ cầu, mũ trụ và xà ngang đều là các khối hình chữ nhật vuông vức, không góc chéo, không thanh chống xoay độ. Nước suối trang trí nằm sâu bên dưới (level−1,2), cách sàn cầu 2,4 u.
- **Cổ và dáng nhân vật:** Bổ sung phần cổ (`BodyBase/neck`) cùng viền cổ áo (`BodyBase/collar`) nối liền đầu và thân, khép kín hoàn toàn khoảng hở cổ. Khi di chuyển, thay thế dịch chuyển tịnh tiến thẳng đứng `+gait` bằng cơ chế vung chân (swing angle $\pm 14^\circ$) và sải bước (stride); mép trên của chân luôn được thắt lưng che chắn hoàn toàn, không còn hiện tượng chân chọc lên trên thắt lưng đâm vào bụng. Thêm băng trán thiên thanh (`Head/band`), mắt có con ngươi biểu cảm (`Head/eye`), đôi ủng da sẫm (`Feet/left`, `Feet/right`) vung theo bước chạy.
- **Bố cục địa hình nhất quán:** Mở rộng mỏm đất Cung đường (`Bow hill`) ở Học Viện thành $x = -18\dots-8$ (trước là $-18\dots-10$), giúp toàn bộ công trình Cung đường ($x = -15\dots-9$) nằm trọn trên mặt đất, mép nhà không còn chìa ra ngoài vực và tiếp nối liền mạch với các bậc thang dẫn lên từ sân chính ($x = -8\dots-5$). Sử dụng `GroundTop` tự động tính cao độ nền đất cho mọi công trình.
- Cung đường x−18…−8/top6,4, DS5 x54…64/top4,6, vách thác sau x64…66/top3,4, PROBE8 x109…119/top4,6 và bậc sau x119…122/top3,2 dùng rear earth cùng màu và mặt cỏ mượt. Nhảy xuyên từ dưới lên mặt trên, S/↓ chủ động rơi xuống lại đường thấp.

### Chi tiết kiến trúc & cảnh quan 3 Map:
- **Thôn Vân Khê (Village):**
  - Cổng thôn phía Tây đồ sộ ("CỔNG THÔN VÂN KHÊ") với cột gỗ lim, mái ngói dốc và chân tảng đá vững chãi.
  - Nhà Trưởng Lão (Lâm Bá): Mái đao uốn cong nhiều tầng, cửa đỏ sơn then cài, ô cửa sổ hoa văn phát sáng ấm áp, lồng đèn đỏ treo hiên và chậu bonsai trang nghiêm trước thềm.
  - Ao Sen làng: Bờ đá uốn lượn, lá sen xanh nổi trên mặt nước và hoa sen hồng hé nở.
  - Tiệm Thuốc (Yên Thảo): Mái ngói xanh lục, giàn phơi thảo dược 3 tầng với các khay thuốc ngũ sắc, dãy bình hồ lô và hũ gốm ngọc chứa dược liệu.
  - Lò Rèn (Bách Luyện): Lò gạch chịu lửa với than hồng rực sáng, ống khói cao tỏa khói, đe thép nặng trên gốc cây cổ thụ, máng nước tôi thép và giá vũ khí rèn thô.
  - Quán Nghỉ & Rương đồ (Mộc An): Mái hiên kẻ sọc đỏ-kem, cờ rượu "TỬU", bàn trà ngoại cảnh với ấm chén sứ thanh nhã, rương chứa đồ nẹp đồng kiên cố trên thềm quán.
  - Đài Nhập Môn: Bệ đá granite trang nghiêm, lư hương tế tổ nghi ngút khói đỏ linh thiêng, đôi cột trụ khắc phù văn cổ và đại kỳ phái tung bay.
  - Cổng Thôn Đông: Biển chỉ đường đi Đồng Sương kèm đèn lồng treo dẫn lối.
- **Thanh Vân Học Viện (Academy):**
  - Cổng Paifang Đông ("THANH VÂN HỌC VIỆN") ngói thanh lam uy nghi, trụ đỏ son bề thế.
  - Đại Sảnh Kiếm (Phong Du): Mái cung điện 2 tầng rực rỡ, 4 cột đại trụ uy nghi, hoành phi "ĐẠI SẢNH KIẾM", giá trưng bày kiếm thép sáng loáng hai bên.
  - Cung Đường (Diệp Lam): Gian nhà mái dốc, 3 bia rơm tập bắn cung cắm mũi tên với các vòng tròn hồng tâm đỏ/trắng/xanh, giá treo trường cung và ống tên tập kích.
  - Võ Đường Tây: Sàn tập gỗ thao trường, giá chiêng đồng huấn luyện võ sinh có khung gỗ chạm khắc tinh xảo.
  - Bậc Tập Nhảy: Giàn giáo gỗ tập luyện với thanh giằng chéo chịu lực, thang leo và biển hướng dẫn thân công.
  - Sân Bù Nhìn: Hàng rào gỗ bao quanh thao trường, các thùng gỗ chứa gậy luyện võ và khối tạ đá tập thể lực.
- **Đồng Sương (Mist):**
  - Các dải sương mù huyền ảo nhiều tầng lượn sóng dọc theo thung lũng.
  - Rừng Nấm Phát Sáng (DS1 & DS2): Các cây nấm khổng lồ phát quang huyền ảo làm phông nền, các thân cây mục rỗng phủ rêu phong.
  - Bãi Sói Sương (DS3 & DS4): Rừng thông vách đá hiểm trở, cột totem cảnh báo "⚠ BÃI SÓI SƯƠNG - CẨN TRỌNG" cùng tiêu bản đầu thú cảnh báo lữ khách.
  - Hẻm Thác & Vực Nước (DS5): Thác nước nhiều tầng đổ bọt trắng xóa, ghềnh đá ngập dòng suối trong vắt.
  - Cầu Thung Lũng: Hệ trụ giàn giáo gỗ đồ sộ, xà ngang kiên cố, thành cầu và đèn lồng cổ trấn giữ hai đầu cầu.
  - Trận Pháp Cổ (DS6): 5 cột cự thạch khổng lồ khắc chữ phù văn phát sáng xanh lam, thạch trụ gãy đổ cổ xưa, tế đàn đá linh thiêng với ngọn linh hỏa thanh lam bất diệt.
  - Tháp Canh PROBE8: Giàn giáo cao với thang leo gỗ dẫn lên cao nguyên bãi sói trên cao.
  - Tiền Đồn Trúc Ảnh: Hàng rào cọc nhọn dã chiến trấn ải và biển báo chỉ hướng "TRÚC ẢNH → (Chưa mở)".

### Chi tiết Mô hình Quái vật & Trang phục NPC:
- **Bù Nhìn / Mộc Nhân (Lv1):** Chân đế chữ thập gỗ với 4 thanh giằng chéo, thân trụ gỗ đứng, lớp rơm quấn ngang lưng siết bằng dây thừng gai, tay đòn ngang ngực có chốt giữ, tay đòn chéo hông, nón rơm chóp nhọn trên đầu thắt dải khăn đỏ võ đạo. Rung lắc khi bị trúng đòn (`Mathf.Sin(Time.time * 45f) * 0.06f`).
- **Nấm Linh (Lv2):** Cuống nấm ngà voi có đôi mắt to biểu cảm, tròng đen và gò má hồng dễ thương; gốc cuống xòe bám đất, lớp phiến sẫm màu; mũ nấm vòm đôi xanh ngọc/tím có 4 đốm bào tử phát sáng màu bạc hà. Hoạt ảnh nhấp nhô hít thở (`Mathf.Sin(Session.Now * 4.5f + m.Id) * 0.035f`).
- **Sói Sương (Lv4):** Mô hình dã thú 4 chân hoàn chỉnh: Lưng và bờm màu xanh xám đá phiến cơ bắp, ngực và bụng bạc sáng, bờm lông dựng dọc gáy; mõm nhọn với mũi đen, tai nhọn lót hồng dựng đứng, mắt phát quang xanh băng; đuôi xù dài chóp bạc; 4 chân có khớp chân trước/sau và bóng râm chân xa. Lật hướng mượt mà, chân chạy nhịp nhàng ($\pm 18^\circ$). Đổi màu đồng bộ toàn bộ bộ phận khi trúng đòn (chớp trắng), tụ lực cắn (cam đỏ) và hồi về bãi (xám).
- **Trang phục NPC:**
  - `Lâm Bá` (Trưởng Lão): Áo Nho sinh nâu, râu dài trắng muốt, tóc bạc búi gọn, gậy trúc chống tay.
  - `Yên Thảo` (Dược Sư): Áo lam ngọc y gia, trâm cài tóc thanh nhã, túi da chứa dược thảo bên hông.
  - `Bách Luyện` (Thợ Rèn): Tạp dề da thợ rèn, cơ bắp lực lưỡng, khăn đỏ buộc đầu, búa rèn thép nặng.
  - `Mộc An` (Chủ Quán): Áo choàng xanh lữ hành, đai vàng hoàng kim, mũ thương nhân lữ quán.
  - `Tạ Minh` (Sứ Giả Tiên Môn): Phẩm phục tím triều đình tiên phái, mũ quan cao vút, cuộn phù chú ngọc.
  - `Phong Du` (Kiếm Sư): Áo võ sư thiên thanh, tóc kiếm khách, thanh kiếm dài giắt ngang hông.
  - `Diệp Lam` (Cung Thủ): Trang phục xạ thủ xanh rừng, cung tên dài đeo chéo sau lưng.
  - Cột mốc / Biển chỉ đường: Trụ gỗ chạm khắc tinh xảo có viền gờ nổi và chữ chỉ hướng sắc nét.

Các màu/tọa độ/mặt vuông là CURRENT PROBE, không khóa production. Docs 1–4 giữ nguyên 100%. G-L vẫn PARTIAL; cần người chơi xem hình thực tế để đánh giá.

### Tái cấu trúc Địa hình 3 Map, Kết hợp Cao - Thấp & Bổ sung Vật liệu Đá (V6.2.7):
- **Bổ sung Vật liệu Đá kết hợp Đất:**
  - Thêm trường `bool Stone` vào struct `Surface` của `BlockoutLayout` và helper method `Stone(name, left, right, top)`.
  - Phân tách rõ ràng giữa các khối đất tự nhiên (lớp cỏ xanh `#6E9C5C` trên thổ đài sẫm màu `#706145`, hạt đất lấm chấm) và các bề mặt bằng đá kiến tạo (sân đình, thềm thợ rèn, võ đường, thềm sảnh Kiếm, đài tế, bậc thang leo núi, bờ kè dốc sông) với chất liệu đá khối xanh xám (`#586470`), đường mạch vữa lát gạch ngang, vát gờ đá vát góc (`bevel`), và dải phản quang ánh sáng (`#758288`).
- **Làng Vân Khê (Village):**
  - Ao sen mở rộng dài 3m ($x \in [2.0, 5.0]$) với bờ thoải giật cấp chênh lệch chỉ $0.2\text{u}$ (bước đi hoàn toàn tự nhiên không cần nhảy). Máng trúc dẫn nước suối từ vách núi rót vào lòng ao, các phiến đá bước dạo bắc ngang mặt nước giữa hoa sen.
  - Lò rèn Bách Luyện được đặt trên thềm đá kiên cố ($y = 0.6$).
  - Giãn cách bố cục công trình và NPC rộng rãi, không còn tình trạng chồng lấn.
- **Thanh Vân Học Viện (Academy):**
  - Đại Sảnh Kiếm uy nghiêm lát sân đá phẳng phiu, hai hàng đại trụ uy nghi và bậc thang đá leo núi phía tây.
  - Phân tách cao độ các phân khu: Thao trường bù nhìn ($y = 0$), Bậc tập nhảy giàn giáo gỗ ($y = 0.8 / 2.8$), Võ đường Tây ($y = 4.0$), Đỉnh đồi cung xạ ($y = 6.4$).
- **Đồng Sương (Mist):**
  - **Hẻm núi dòng sông dài và sâu:** Lòng sông trải dài 24m ($x \in [62, 86]$, đáy sông $y = -1.5$, mặt nước $y = -0.9$, độ sâu lội nước $0.6\text{u}$). Nước sông là vùng tương tác thực tế (`speed factor = 0.85×`, bọt nước sóng sánh), các tảng đá nhô trên sông có mặt khô ($y = -0.55$) cho phép nhảy dạo qua.
  - **Cầu gỗ trên cao bắc qua sông:** Cầu gỗ kiên cố tại $y = 1.2$ (cách mặt nước $2.1\text{u} - 2.7\text{u}$ tĩnh không), có trụ giàn giáo chống sâu xuống đáy sông, xà ngang, thành cầu và đèn lồng dẫn lối. Cầu khô ráo, giữ tốc độ di chuyển bình thường ($1.0\times$).
  - **Tuyệt đối không kẹt vĩnh viễn (Zero Softlocks):** Cả hai bờ sông đều bố trí dốc đá thoải bậc thang (bờ tây $x \in [58, 62]$, bờ đông $x \in [86, 88]$ với độ chênh mỗi bậc $\le 0.7\text{u}$), đảm bảo người chơi khi rơi hoặc nhảy xuống lòng sông đều có thể dễ dàng đi bộ hoặc nhảy nhẹ bước lên bờ từ cả hai hướng.
  - **Thác nước nhiều tầng tự nhiên (Multi-tiered Cascading Waterfall):** Tái tạo lại thác nước 3 tầng hoành tráng ($x \in [42, 54]$):
    - Tầng 1: Khe núi cao trên nền đá sẫm màu đổ dòng nước trắng xóa có gờ bọt nước trắng tinh ở đỉnh.
    - Tầng 2: Tảng đá cản giữa vách núi tách dòng thác thành đôi dải lụa nước song song đổ dốc.
    - Tầng 3: Vực xoáy chân thác với bọt nước cuồn cuộn nhiều lớp và làn sương mù nước mờ ảo bốc lên quanh chân cầu.
    - Thác suối rừng nhỏ (Forest Cascade) tại $x = 18.5$ với bậc đá phủ rêu dẫn nước từ đồi sói róc rách đổ xuống đồng nấm.
- **Khớp nối nhân vật & chuyển động:**
  - Cổ nhân vật gắn kín khít vào thân áo tại $y = 0.53\text{u}$ (khoảng đè $0.06\text{u}$, tuyệt đối không hở cổ).
  - Khớp háng xoay tự nhiên tại $y = -0.16\text{u}$ với hành trình góc $\pm 18^\circ$, chân không đâm xiên lên bụng khi chạy.

### Tinh chỉnh Cảnh quan & Tối ưu Hiệu năng (V6.2.8):
- **Sửa triệt để các vật thể lơ lửng & sai lệch vị trí:**
  - *Cây thông Võ đường Tây (Học Viện):* Bỏ hoàn toàn cây thông lơ lửng bị kẹt tại $y = 5.4$ cấn vào mái hiên và bậc thang đá; chuyển thành hai cây thông cắm rễ vững chãi trên mặt đất thực `GroundTop`: một cây ở thềm Tây ($x = -27.2$) và một cây ở sườn Cung đường ($x = -16.5$).
  - *Cột đèn lồng đá thềm tập võ (Học Viện):* Dọn sạch các cột đèn đặt lệch cọc, lệch bậc cắt ngang khóm trúc trên thềm tập võ; bố trí lại các cặp đèn lồng đá đối xứng trang nghiêm đặt trên mặt đất phẳng $y = 0$: đôi đèn chầu Đại Sảnh Kiếm ($x = \pm 5.2$), đèn dẫn lối vào thềm luyện võ ($x = 11.5$), và đèn trước cổng rào sân bù nhìn ($x = 19.2$).
- **Phân tầng hiển thị & Lớp đất đồi thác nước (Đồng Sương):**
  - Khắc phục xung đột sorting order khiến mảng đất sau bị vách núi cắt xén: tách biệt rõ các lớp render (Vách núi xa `sortingOrder = -16`, Khe vực thác `-15`, Dòng thác chảy `-14/-13`, Đất đồi sau `RearEarth` `-12`, Lớp cỏ `-10`, Nền solid `0..2`, Nhân vật & Quái vật `7..15`).
  - Khối đất đồi sau (`RearEarth`) phủ kín toàn bộ phông nền sau lưng nhân vật khi đi dọc đường thấp đến tận $-7.5\text{m}$, cho phép nhân vật bước đi xuyên suốt qua đường hầm dưới đồi trước phông nền đất tự nhiên liền lạc, không còn vệt cắt cụt.
- **Triệt tiêu hiện tượng lag giật & Tối ưu mượt mà:**
  - *V-Sync & Framerate:* Tắt VSync (`QualitySettings.vSyncCount = 0`) và cố định `Application.targetFrameRate = 60`, loại bỏ hiện tượng driver đồ họa Mesa OpenGL trên Linux tự động drop FPS xuống 30 khi có biến thiên fillrate.
  - *Camera lag:* Giảm hệ số trễ `SmoothDamp` của camera từ $0.13\text{s}$ xuống $0.045\text{s}$, loại bỏ triệt để hiện tượng giật cục / trễ dây thun (rubber-banding) khi nhân vật di chuyển và nhảy.
  - *Zero-allocation per frame:* Chuyển hàm dựng khung xương `GeometricRig.Pose` sang dạng truyền danh sách tái sử dụng (`List<RigPart>`), loại bỏ toàn bộ các truy vấn LINQ `.Where()` / `.ToArray()` trong vòng lặp `FixedUpdate`, `LateUpdate`, và `OnGUI`, triệt tiêu rác bộ nhớ (GC spike) gây đứng hình định kỳ.
  - *Build Tooling:* Thêm `EditorApplication.Exit()` vào `SliceBuild.cs` giúp quá trình build tự động trong batchmode thoát sạch sẽ ngay khi hoàn tất mà không bị treo tiến trình Editor.

**Kiểm V6.2.7:** EditMode **58/58**, PlayMode **10/10**; Linux development build đã build lại và hoàn tất (`HuyenLo.Editor.SliceBuild.Linux()`). [Fresh Q1–Q6](#evidence-small-1c3b52fbfcf1) PASS bằng Rigidbody2D và keyboard menu adapter, timeScale4; không preset hoặc inject position/quest/EXP/HP để bỏ bước. PlayMode kiểm riêng đi qua cầu hai chiều không jump, S không xuyên cầu, jump xuyên rear earth/landing/drop, leo tới NPC tầng cao, và kiểm tra tốc độ lội nước dưới đáy sông/trên cầu.

Ảnh **standalone 1×, input bàn phím thật**: [đường thấp trước đất sau](#evidence-v6-2-7-4cee9f818a65) → [nhảy lên mặt cao](#evidence-v6-2-7-7731401d7b12) → [S xuống lại](#evidence-v6-2-7-e5aa9e1ae645); [cầu và khoảng hở trên nước](#evidence-v6-2-7-7309e033940e), [dốc đầu đông](#evidence-v6-2-7-40e393301647), [Cung đường trên đất fill đầy](#evidence-v6-2-7-509cf70747f8); [Cầu thung lũng trên cao và thác nhiều tầng](#evidence-v6-2-7-c2a1915f388a), [Lội nước hẻm sông dưới chân cầu](#evidence-v6-2-7-17491543efd7).

[Build/source hashes và phương pháp kiểm](#evidence-small-8a5466319cec) · [Build summary](#evidence-small-257a28f6fc1a) · [Audit số dòng/bảng/heading/giá trị trọng yếu](#evidence-small-114942163ccf). Script so với checkpoint `archive/checkpoint-13035d3` (local-only archive, không có trên origin), kiểm docs1–4 giữ byte, links/anchors và 17 nội dung di chuyển lịch sử; lấy mẫu ngẫu nhiên8 mục SOURCE→DESTINATION. Revision này không chuyển/xóa nội dung ở docs1–4.

## V6.2.6 — lịch sử bị từ chối: slab đất solid và cầu sát nước

User làm rõ: “núi” là các khối đất cao có mặt đứng được, có đoạn phải nhảy qua và đoạn có thể đi dưới rồi leo lên; ngoại cảnh sau cùng là lớp khác. Nước rộng có cầu là hình trang trí cao gần cầu vì người chơi không xuống đó. V6.2.5 giải sai vấn đề bằng lớp đất sau không collision; không kế thừa cách đó.

- Mọi khối đất có grass cap nhìn như mặt đi đều lấy từ `Surface` và có solid collider. Bỏ toàn bộ rear earth terraces giả; ngoại cảnh chỉ nền xa/vegetation, không vẽ một tuyến đứng giả.
- Vân Khê: đài nhập môn là đồi thật với bậc1,2/2,4 và đỉnh3,6 u; Tạ Minh đứng trên đỉnh. Gác nhà vẫn one-way gỗ, đất không drop-through. Đường xuống trở về cổng x30/top0, không đổi chiều liên kết map.
- Học Viện: sống đất phía tây lên4,4/5,4/6,4 u; mỏm đất x−18…−10/top6,4/bottom5,2 nối sống đất. Cung đường/Diệp Lam ở mặt trên; gác nhà top9,2. Đường thấp bên dưới vẫn tồn tại; phía đông có bậc1,4/2,8/4,2 và khoảng nhảy2 u để lên mỏm. Phong Du và Q2/Q3 giữ tuyến chính.
- Đồng Sương: DS3 là đồi3,6 u có bậc lên/xuống. DS4 đứng ở plateau2,4; nhảy từ plateau lên mỏm DS5 x54…64/top4,6/bottom2,2 nối khối đất bên trái, có hai Sói ở trên. Có thể đi dưới mỏm trên nền0 rồi ra khe bên phải. PROBE7 bốn Sói ở tuyến0,8; PROBE8 ba Sói ở mỏm thật x109…119/top4,6, từ đường dưới nhảy lên bậc gỗ x107/top2,5 rồi lên mỏm; bậc đất x119…122 nối vai đồi để xuống đường phía đông. Tổng vẫn17 slots mock, không tăng budget release10 hoặc thay stat/reward/quest credit.
- Mob home/lane chọn đúng mặt nền được author, không dùng highest-Y để đẩy quái từ đường dưới lên roof. Không thêm AI nhảy/đa tầng; mob tuần tra và đánh trong mặt nền của đàn.
- Suối rộng x66…82 có cầu **solid** top0,4, nước trang trí level0,08 nối nhánh thác bờ. S không drop qua cầu, không có route đi đáy suối hoặc slow ở nước rộng. Không swimming/drowning/hazard. Vũng nhỏ Vân Khê vẫn có đáy thật, đi qua và thử speed×0,85; đây là hai loại khác nhau.

Test-driver của mock giữ jump đủ cao khi vượt cliff và dùng takeoff point để lên DS5; chỉ là điều khiển probe, không player auto-navigation. Q5 entry marker derive đúng cao độ bãi, không giữ waypoint Y cũ làm softlock. Spawn khi portal/về làng derive mặt nền + body offset; không đặt actor trong đồi rồi rơi khỏi map. Các tọa độ/tuning/capture ở mục này chỉ CURRENT PROBE; docs1–4 không đổi trong lượt sửa.

**Kiểm revision:** EditMode57/57, PlayMode9/9; Linux development build V6.2.6 thành công. Fresh standalone Q1–Q6 PASS (Rigidbody2D, keyboard menu adapter, timeScale4, không preset hoặc set quest/EXP/HP/position). Physics fixtures kiểm leo đồi tới Diệp Lam bằng E, đi dưới/nhảy lên cùng mỏm đất, đất/cầu không drop-through và leo lên PROBE8 rồi xuống đường đông. Native1× dùng bàn phím, preset chỉ cho view riêng, không là route acceptance. [Kết quả và source/build hashes](#evidence-small-ff856246be01) · [Build](#evidence-small-080c1b72b86f) · [Audit docs](#evidence-small-4f633c9d68cb).

Ảnh native1× đã xem: [đồi Tạ Minh](#evidence-v6-2-6-575d48207cfb), [Cung đường trên mỏm đất thật](#evidence-v6-2-6-be3216664c3c), [hội thoại Diệp Lam sau khi leo](#evidence-v6-2-6-502002a1cacd), [đàn trên/dưới PROBE8](#evidence-v6-2-6-4b8dfc4f604f), [mặt trên DS5](#evidence-v6-2-6-04ba9aaa398c), [đường dưới cùng mỏm đất](#evidence-v6-2-6-3bfed78f4223), [nước trang trí sát cầu sau S](#evidence-v6-2-6-6b6fe15b1f42). Art vẫn khối primitive; **G-L PARTIAL**, chờ user xem hình/cảm giác. Không suy từ test pass rằng hình đã đúng ý user hoặc G-N/G-D đã mở.

## V6.2.5 — lịch sử bị từ chối: hiểu sai núi thành ngoại cảnh

- Thay diamond silhouettes/khối cây rải đều bằng hai lớp đất bậc liền, cao dần phía sau tuyến chính. Cây tre đặt theo mặt đất của từng lớp; hậu cảnh không collision, không dùng để đánh lừa có sàn đứng.
- DS5 chuyển lên bờ khô x58/60, activity nằm trong support x56…64. Giữ ID/slot/stat, DS1–DS6/10 slots và quest credit; PROBE7/8 vẫn riêng mock; PROBE8 centreY4,05 theo mặt sàn3,4 + offset0,65 để chân không bị sàn che. Không đặt quái hay props khô dưới lòng nước.
- Lòng suối x66…80 có một vùng nước liên tục, đáy −2 u/mặt −1,45 u. Nguồn thác ở mép lớp đất sau, x66,325/top3,5, nối xuống cùng mặt nước. Cầu x64…82/top0,1 đi khô; tốc độ chân chạm nước thử ×0,85, không stack. Thác/ripple cosmetic, không water physics.
- Cung đường có sàn gỗ thật top2,8 u; Diệp Lam đứng trên gác (centreY3,6), tiếp cận bằng jump và E/chuột đúng range. Phong Du/nhiệm vụ Kiếm giữ tầng chính. Nhà trưởng lão (sàn3 u)/lò rèn (sàn3,125 u), Cung đường/võ đường tây có gác thật, cửa/cột/dầm/mái đọc được; nhà không có deck chỉ vẽ một tầng. Không vẽ dầm tầng giả hoặc mái trang trí như sàn đứng.

Đây là authoring probe, không cập nhật manifest map production. Không đổi stats/balance hoặc khóa spacing/grace chỉ từ hình tham khảo. Kiểm route tự động, chân chạm nước/cầu, quái trên nền khô, gác NPC và render thực; người chơi vẫn cần đánh giá chất lượng/feel ở tốc độ thường.

**Kết quả kiểm V6.2.5:** 57 EditMode + 7 PlayMode pass; Linux Development build thành công, fresh Q1–Q6 route PASS bằng Rigidbody2D/menu keyboard adapter ở timeScale4. Capture bên dưới là input bàn phím thực ở 1×: Vân Khê fresh start, Học Viện dùng SwordTraining, Đồng Sương dùng Crowd có nhãn DEBUG. Đã kiểm nối nguồn/thác/suối, Sói trên nền khô, cầu khô/lội nước và E/Esc trên gác Diệp Lam. Không phải fresh journey playtest 1×, user visual approval, online capacity hoặc production art acceptance; **G-L vẫn PARTIAL**.

[Evidence/build/hash](#evidence-small-23d16d500d0e) · [Audit số dòng/bảng/heading/chỉ số](#evidence-small-ae3ddbad7ec4) · [8 mục SOURCE → DESTINATION lấy mẫu bằng script](#evidence-small-392f4b2e387b).

Ảnh build hiện tại: [Vân Khê](#evidence-v6-2-5-e1eaa0d076fc) · [Diệp Lam trên gác thật](#evidence-v6-2-5-1187d6f9f22b) · [Thác, suối và cầu](#evidence-v6-2-5-b3255faf4826) · [Lội nước dưới cầu](#evidence-v6-2-5-63769d2e524c) · [Sói trên bờ khô và sàn cao](#evidence-v6-2-5-cac531e206a4).

<a id="prototype-technical-history"></a>

## Technical — lịch sử probe V6.2.4

**Melee lane probe V6.2.4:** trace target → desired X → occupied/blocked → velocity → actual range/windup. V6.2.3 rank offset + peer clipping đã làm Sói sau kẹt ngoài range; không giữ hành vi chờ con đầu chết. Probe mới dùng recovery/cross goal có chủ đích + bounded neighbor correction trong một lần tích phân velocity; không shove sau chase hoặc hard body blocker. Cắn giữ definition range/interval; windup origin/facing khóa. Recovery goals nằm trong support lane/home bounds, có phase lệch mỗi life và chọn phía đủ chỗ tại mép. Brief crossings hợp lệ vì no body blocking; không persistent blob, không attack token/formation. Test 1/2/4, tường/mép/khác cao độ và player đổi hướng; local pass không suy capacity online.

**Blockout V6.2.4:** dữ liệu `BlockoutLayout` của mock tách solid terrain profile/wooden one-way/bounds khỏi drawing; GroundTop và support span cấp home/lane cho mob, không đặt mob lơ lửng trên cao độ cũ; reciprocal exit links + destination spawn nằm đúng mép, camera snap chỉ khi transition/reset. Physics body tiếp tục FixedUpdate, camera bám interpolated visual Transform tại LateUpdate với dead zone/damping/clamp; mob logical positions giữ previous/current cho visual interpolation, không snap simulation để cứu render. [Unity — Rigidbody2D interpolation](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Rigidbody2D-interpolation.html) mô tả smoothing giữa physics updates; đây là căn cứ probe, không bảo đảm cảm giác mượt trên mọi máy.

**Pack activity / patrol probe:** cả pocket dùng ActivityMin/Max chung, nằm trong support span; không từng con bị kẹt ở home clamp khác nhau. Chỉ start bite khi player trong vùng. Ngoài vùng/không reachable áp grace hiện có rồi Return; reset HP/contribution, không tạo life/reward/reroll. Sau return, goal patrol gần home có pause/phase riêng; cùng một bounded velocity integration. Windup đã bắt đầu vẫn resolve/cancel theo clock, không trì hoãn tới khi player quay lại. Exact bounds/speed/policy chỉ dữ liệu mock.

**Water contact:** `BlockoutLayout.Waters` giữ bounds/bottom/level/factor; locomotion kiểm feet contact, nhân speed factor tối thiểu một lần. Collider là đáy đất/cầu hiện hành, nước không có fluid physics. Bridge/air phải dry; front overlay chỉ che phần thấp, ripple cosmetic dùng cùng contact predicate, không là authority của slow.

**UI event boundary:** command có thể đổi panel và rebuild actions; mouse callback phải kết thúc IMGUI event ngay sau mutation (`GUIUtility.ExitGUI`), không tiếp tục index sáu slot cũ. Selection ID, breadcrumb và validation vẫn dùng chung keyboard/mouse; không catch/swallow lỗi list để che bug. Auto-close chỉ sau domain command thành công, batch services giữ context; Esc Back thống nhất cả input adapter và route driver.

**Mock/dev boundary:** Unity project tham khảo nằm tại `prototypes/VS1_EndToEnd/`; production `game/` dự kiến dùng chung rules/content cho Client và Dedicated build, `backend/` dành Spring/PostgreSQL. Chưa scaffold hai phần production. Debug menu chỉ Editor/Development build, tạo session fixture mới từ mốc Q1–Q6/sân tập và reset đủ inventory/receipts/clock/food/cooldown/loot/focus. Preset có nhãn DEBUG, không dùng trong route acceptance. Chọn menu mốc thay save JSON; JSON hiện chỉ config/fixture/evidence, production persistence vẫn PostgreSQL.

**Offset fixture cũ — lịch sử, chưa là production lock:** proposal 3–4 offsets được giữ để đối chiếu; response hiện tại đọc phần V6.2.5.

**Melee separation/reposition prototype:** production phải đạt nguyên tắc cluster đọc được mà không pile; exact spacing/response vẫn PHY-01 TUNABLE. Disable body collisions Player–Monster và Monster–Monster; hurtbox/query riêng. Ground AI chọn desired X offsets trái/phải quanh reachable target trên cùng lane; thử một danh sách 3–4 offsets chỉ là fixture, không lock slot count hoặc formation system. Điểm bị tường/mép/actor gần chiếm thì chọn điểm hợp lệ khác hoặc hold có hạn; không teleport sang phía kia player, không group attack mutex. Threat target không đổi vì offset bị chiếm. N-player probe theo từng mob/target, không assume hai người.

<a id="prototype-feedback-history"></a>

## Analysis — lịch sử feedback V6.2.4

**Feedback V6.2.4 — CURRENT PROBE:** mock trước gộp người hướng dẫn phái và đảo mép Học Viện, hai sai lệch đã được đưa về đúng contract. User yêu cầu manual unequip trước chọn class; reject không grant/đổi class. V6.2.3 có lỗi rank/peer clipping làm Sói sau kẹt; V6.2.4 thay bằng recovery/cross goals + bounded neighbor correction, trace eligibility và kiểm cả bốn có windup mà không phải giết con trước; không tăng stat/giảm interval. Đồng Sương mock giữ DS1–DS6/10 slots, thêm PROBE7/4 và PROBE8/3 ở hai lane riêng để kiểm crowd/vertical; 17 slots là workload mock, không thay budget release hoặc capacity promise. PROBE không credit Q5, vẫn level-gap loot thường. Terrain dùng khối solid bậc lớn, ít sàn gỗ one-way, nước nông giảm tốc nhẹ theo direction mới của user, thác/ripple cosmetic. RPG shell năm tab và geometric rig là UX/visual probe, không nghiệm thu art production. Bảng TTK/farm/hành trình trước chưa tính topology/crowd mới; vẫn là lịch sử đối chiếu, cần rerun khi gate tương ứng.

**Feedback nối tiếp:** hai ảnh tham khảo cung cấp terrain grammar (khối đất liền, terraces, basin, cầu/thác), không scale pixel hoặc yêu cầu sao chép asset. Mock tăng depth basin và thử ×0,85 water contact; chưa đo ảnh hưởng travel/journey/endgame và không retune EXP/TTK từ đó. Shared activity bounds + return→patrol giải quyết đứng mãi ở mép; giữ combat hiện tại. Lỗi mouse equipment do action list đổi giữa render, khác với keyboard logic PASS; cần kiểm click thật trên build bên cạnh tests. Menu close policy tách terminal single action khỏi batch services. Review 1× phát hiện tracker Q1 chỉ sai sang cổng Đồng Sương; route hint nay theo step destination/current map, kiểm cả bước quay về NPC.

<a id="prototype-map-history"></a>

## Art — Tái cấu trúc hoàn toàn layout 3 map VS-1 V6.2.7 — CURRENT PROBE

### 13.1. Thiết kế bố cục địa hình và cảnh quan 3 map mới

Theo phản hồi từ người dùng, sơ đồ bản đồ cũ đã được gỡ bỏ hoàn toàn. Ba bản đồ được tái cấu trúc toàn diện theo các nguyên tắc:
1. **Cổng chuyển vùng ở sát biên bản đồ (Edge Boundaries):** Cổng sang Học Viện (tây Vân Khê x = -10, biên -12), cổng sang Đồng Sương (đông Vân Khê x = 34, biên 36), cổng đông Học Viện (x = 34, biên 36), cổng tây Đồng Sương (x = -4, biên -6) và ranh giới Trúc Ảnh (x = 128, biên 130) được đặt tại sát biên của thế giới playable, loại bỏ tình trạng đi qua cổng rồi vẫn còn vùng đất trống thừa.
2. **Loại bỏ hoàn toàn hố kẹt dưới Diệp Lam (Học Viện):** Xóa bỏ lòng sân trũng `Class courtyard` y = 0 dưới Diệp Lam. Toàn bộ sườn núi phía tây từ x = -18 đến -8 tại y = 6.4 là khối đất/đá solid nguyên vẹn chạy liền xuống nền móng, loại bỏ mọi hố sâu gây kẹt vĩnh viễn (softlock) cho người chơi.
3. **Đài Tạ Minh vững chãi, bậc tam cấp (Village):** Móng đá được mở rộng thành khối tam cấp vững chắc x ∈ [21, 31], đỉnh đài x ∈ [23, 29] tại y = 3.6 rộng 6m; bệ thờ dài 4.8m và các cột tế đàn nằm hoàn toàn bên trong đế đá với gờ an toàn, không còn cảm giác chìa ra ngoài không trung sắp sập.
4. **Khoảng cách thoáng giữa cổng và công trình:** Cổng tây cách Nhà trưởng lão > 5.6m, cổng đông cách Đài Tạ Minh > 4.8m; các công trình và dịch vụ (Lâm Bá, Hồ Sen, Nhà thuốc, Lò rèn, Quán nghỉ, Đài Nhập Môn) có vị trí mặt tiền riêng biệt, không chồng lấn.
5. **Thác nước trung tâm và vách núi hùng vĩ (Đồng Sương):** Dời thác nước từ góc cầu về chính giữa hẻm vực (x = 74), dựng vách núi cao phía sau (y = 5.5 → 10.2) với dòng thác lớn đổ từ đỉnh núi cao y = 8.8 xuống thẳng xoáy nước hẻm sông y = -1.5, nhìn thấy phía sau cầu gỗ.
6. **Toàn bộ trang trí cắm đất thực tế (Grounded Props):** Tất cả biển báo, nấm khổng lồ, cột cảnh báo, bia đầu lâu, đèn lồng đá, cây cối đều có thân/cột cắm xuống mặt đất thực `GroundTop(x)`, xóa sạch chữ bay lơ lửng giữa trời.
7. **Hẻm sông có dốc thoải 2 bờ:** Lòng hẻm suối dưới cầu gỗ (y = -1.5) có nước nông, đá tảng và có dốc thoai thoải ở cả bờ tây (x = 58..62) và bờ đông (x = 86..88), đảm bảo người chơi có thể xuống khám phá và tự đi lên cả hai phía mà không bị kẹt.

Ca kiểm: reciprocal exits giữ hướng/outside reverse trigger; Q2 thực sự đứng ledge rồi drop; Q4/5 không bị optional route chặn; Kiếm nhảy tới PROBE8 qua bậc; crowd2/3/4 giữ silhouette riêng, không steering đổi windup; camera không bám từng fixed tick hoặc rung theo bước landing. Chạy cả normal speed và route automation; chưa pass manual thì ghi G-L PARTIAL.

<a id="prototype-ui-history"></a>

## Art — hồ sơ UI/rig mock

### 19.1. NPC, bag grid và equipment view — CURRENT PROBE

NPC root hiện lời nói mộc mạc, quest action đúng state và các chức năng của chính NPC; chọn Mua/Bán/Gửi/Lấy mới mở danh sách đó. `!` là quest Available hoặc nhập phái đúng step; `?` là Ready, `…` là đang làm. Không đánh dấu Diệp Lam “sẵn để chọn Cung” nếu nhánh mock chưa mở. Khi Q6 đang có Mộc Kiếm, Phong Du nói rõ C → Trang bị → Vũ khí → Tháo; action class khóa đến khi ô trống. Không auto-unequip thưởng.

Bag 30 ô dựng 6 cột ×5 hàng, icon + count + selected border; một bảng bên phải hiện tên/type/band/quality/stat/binding. Enter/Interact vào thao tác món; arrow chọn ô, Tab tuyến tính, Esc về grid giữ focus theo instance nếu còn. View equipment ở C → Trang bị có hình nam tạm ở giữa và sáu slot quanh; slot trống vẫn đọc được, slot có đồ mở detail/tháo. Không lặp toàn bộ action trang bị ở bag. Mock icons dùng khối/motif + nhãn; art final vẫn ItemDefinition visual/icon refs, rarity/stack là overlay.

**RPG UX probe V6.2.4:** năm tab đổi trực tiếp bằng Tab/Shift+Tab hoặc click, arrows giữ navigation trong grid/action list. I/C/Q mở đúng view; selected action ID được giữ khi đổi tab, không callback vào item cũ. Bag có icon/stack/detail + dùng/trang bị nhanh + so món đang mặc; slot equipment mở bag lọc đúng slot, cả slot trống. Phân điểm riêng Thuộc tính, derived stats riêng Thông số; không bắt đóng menu để chuyển view.

**Geometric rig:** BodyBase/Hair/Armor/LowerBody/Weapon dùng một module/pose/socket description cho world và equipment preview. Áo, Quần, Wood/Sword đổi silhouette/màu thật, grip và weapon angle theo action clock; walking sway cosmetic. Đây là layering probe, chưa art production hoặc chốt nghĩa 26 frame.

**Normal HUD:** ba slot bottom-center có key/icon/selected/locked/CD/MP, H/M/F bên cạnh. DEV/F8 nhỏ riêng, preset vẫn có nhãn; không để menu debug định layout player HUD. Q1–Q6 có nhận/trả và phản hồi trung gian, marker/name/speech ở các cao độ tách nhau.

**Water probe — feedback mới:** hub có basin nông, Mist có đáy suối thấp hơn main route 2 u, shallow pool và vùng nước rộng dưới cầu. WaterBase + FrontOverlay theo volume thật, chỉ che chân/phần thấp; waterfall nhỏ sau lane + mặt nước/splash readable. Feet chạm nước thử speed ×0,85, bridge/air dry; ripple cosmetic tối đa một lần/0,25 s. Không water collider/swimming/hazard; full Bạch waterfall production vẫn deferred. Đây thay water cosmetic-only của mock trước, theo user; depth/factor/tọa độ không production lock.

[Capture blockout chạy thật](#evidence-v6-2-4-cfe49aacb8b9) · [Trang bị sau mouse unequip](#evidence-v6-2-4-ad21fb1d41ca). Đây là fixture 1×, không evidence fresh journey hoặc production art.


<div style="max-width:860px;background:#ffffff;border-left:4px solid #0043ce;padding:18px;color:#1e1e1e"><p>RPG shell chung, năm view riêng; thao tác keyboard/mouse gọi cùng command.</p><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 820 300" role="img" aria-label="RPG tab shell">
<rect x="10" y="10" width="800" height="275" fill="#e8e8e8" stroke="#787878"/>
<rect x="20" y="20" width="152" height="36" fill="#c8d4ed" stroke="#003bb5"/>
<text x="32" y="44" fill="#1e1e1e">I · Hành trang</text><text x="185" y="44" fill="#1e1e1e">C · Trang bị</text><text x="340" y="44" fill="#1e1e1e">Thuộc tính</text><text x="495" y="44" fill="#1e1e1e">Thông số</text><text x="650" y="44" fill="#1e1e1e">Kỹ năng</text>
<rect x="24" y="75" width="330" height="162" fill="#ffffff" stroke="#787878"/><path d="M79,75v162M134,75v162M189,75v162M244,75v162M299,75v162M24,129h330M24,183h330" stroke="#787878"/>
<text x="30" y="264" fill="#4a4a4a">Grid icon / stack · selected ID ổn định</text>
<rect x="380" y="75" width="400" height="162" fill="#ffffff" stroke="#787878"/>
<text x="398" y="101" fill="#0043ce">Tên / chỉ số / đang mặc</text><text x="398" y="133" fill="#1e1e1e">Trang bị / Dùng / Học + reason</text><text x="398" y="165" fill="#1e1e1e">Slot → bag lọc đúng GearSlot</text><text x="398" y="197" fill="#1e1e1e">Điểm chưa dùng → Thuộc tính</text><text x="380" y="264" fill="#4a4a4a">Tab / Shift+Tab · arrows · Enter/E · Esc</text></svg><p>Equipment: rig giữa sáu slot. Attributes: STR/VIT/INT/AGI. Stats: derived evaluator. Q mở Quest riêng; NPC mở từng dịch vụ.</p></div>

[PNG bố cục UI đã render](#evidence-v6-2-4-e5a97a7ee355).

DoD của view: hoàn tất Q1–Q6 không click, submenu không lọt movement/skill, disabled reason đọc được, consume/equip không giữ callback item cũ, reset xóa breadcrumb. Kiểm text Việt không crop ở720p và resize; 30/40 ô inventory/storage không tự biến thành hàng trăm actions trong NPC root. Icon/rig final và khả năng dùng nhanh bằng người thật còn qua art/UI gate.

<a id="prototype-water-history"></a>

## GDD — lịch sử tuning nước, đã tách khỏi luật

**Nước nông — direction mới của user:** basin có đáy solid đi được, mặt nước cao hơn đáy để nhân vật chìm phần chân khi lội; giảm nhẹ tốc di chuyển khi chân trong nước. Mock thử hệ số ×0,85, không stack nhiều water volumes; exact depth/factor là TUNABLE. Đi trên cầu hoặc nhảy ra khỏi mặt nước không giảm tốc. Vùng nước rộng có route cầu rõ, không buộc bơi; thác/flow/ripple chỉ presentation. Không swimming, breath, buoyancy hoặc water damage. Luật này thay baseline water visual-only trước feedback; không tự coi water slowdown là trạng thái Băng Hàn.

<a id="prototype-tooling-history"></a>

## Technical — tooling của prototype

**Unity / tooling:** prototype hiện có dùng Editor thực cài 6000.5.9f1; production base chưa được scaffold. Pin ProjectVersion/manifest/lock khi dựng base bằng Editor, không gọi bản này là LTS nếu chưa xác minh. Input System đã dùng trong prototype; NGO + Unity Transport là lựa chọn TARGET realtime, không claim đang có implementation trong prototype; Multiplayer Play Mode, Multiplayer Tools/Network Simulator và Unity Test Framework cho dev/QA; ObjectPool chỉ cho VFX/presentation; Cinemachine 3 cho camera client. Production dev test đầu dùng Local Session boundary ở §1.1, không mặc định kế thừa prototype; gate mạng và final online acceptance kết nối Dedicated Server. MPPM chỉ giúp mở nhiều Client khi lặp nhanh. Tránh DOTS/ECS, Addressables, Relay, prediction/rollback, cloud services và service framework nếu slice chưa chứng minh lợi ích.

## SOURCE → DESTINATION của lượt này

| SOURCE | DESTINATION | Bảo toàn |
| --- | --- | --- |
| 2_HUYEN_LO_TECHNICAL.md: `**Melee lane probe V6.2.4:**` | [Roadmap](#prototype-technical-history) | Chuyển nguyên nội dung lịch sử; không biến thành luật hiện tại |
| 2_HUYEN_LO_TECHNICAL.md: `**Blockout V6.2.4:**` | [Roadmap](#prototype-technical-history) | Chuyển nguyên nội dung lịch sử; không biến thành luật hiện tại |
| 2_HUYEN_LO_TECHNICAL.md: `**Pack activity / patrol probe:**` | [Roadmap](#prototype-technical-history) | Chuyển nguyên nội dung lịch sử; không biến thành luật hiện tại |
| 2_HUYEN_LO_TECHNICAL.md: `**Water contact:**` | [Roadmap](#prototype-technical-history) | Chuyển nguyên nội dung lịch sử; không biến thành luật hiện tại |
| 2_HUYEN_LO_TECHNICAL.md: `**UI event boundary:**` | [Roadmap](#prototype-technical-history) | Chuyển nguyên nội dung lịch sử; không biến thành luật hiện tại |
| 2_HUYEN_LO_TECHNICAL.md: `**Mock/dev boundary:**` | [Roadmap](#prototype-technical-history) | Chuyển nguyên nội dung lịch sử; không biến thành luật hiện tại |
| 3_HUYEN_LO_DESIGN_ANALYSIS.md: `**Feedback V6.2.4 — CURRENT PROBE:**` | [Roadmap](#prototype-feedback-history) | Chuyển nguyên nội dung lịch sử; không biến thành luật hiện tại |
| 3_HUYEN_LO_DESIGN_ANALYSIS.md: `**Feedback nối tiếp:**` | [Roadmap](#prototype-feedback-history) | Chuyển nguyên nội dung lịch sử; không biến thành luật hiện tại |
| 4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md: `### 13.1.` | [Roadmap](#prototype-map-history) | Chuyển nguyên nội dung lịch sử; không biến thành luật hiện tại |
| 4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md: `### 19.1.` | [Roadmap](#prototype-ui-history) | Chuyển nguyên nội dung lịch sử; không biến thành luật hiện tại |
| 1_HUYEN_LO_GDD.md: `**Nước nông` | [Roadmap](#prototype-water-history) | Chuyển nguyên nội dung lịch sử; không biến thành luật hiện tại |
| 2_HUYEN_LO_TECHNICAL.md: `**Unity / tooling:**` | [Roadmap](#prototype-tooling-history) | Chuyển nguyên nội dung lịch sử; không biến thành luật hiện tại |
| 3_HUYEN_LO_DESIGN_ANALYSIS.md: `Bản mẫu mới kiểm one-press` | [Roadmap](#prototype-runtime-history) | Lịch sử runtime; phương pháp/decision design vẫn ở Analysis |
| 3_HUYEN_LO_DESIGN_ANALYSIS.md: `User yêu cầu sửa theo feedback và tự xử lý inconsistency;` | [Roadmap](#prototype-runtime-history) | Lịch sử runtime; phương pháp/decision design vẫn ở Analysis |
| 3_HUYEN_LO_DESIGN_ANALYSIS.md: `[Review Art]` | [Roadmap](#prototype-runtime-history) | Lịch sử runtime; phương pháp/decision design vẫn ở Analysis |
| 3_HUYEN_LO_DESIGN_ANALYSIS.md: `Bộ power/MP/CD/gear làm control` | [Roadmap](#prototype-runtime-history) | Lịch sử runtime; phương pháp/decision design vẫn ở Analysis |
| 2_HUYEN_LO_TECHNICAL.md: `**Melee separation/reposition prototype:**` | [Roadmap](#prototype-technical-history) | Giữ offset fixture cũ, production chỉ giữ nguyên tắc |


<a id="prototype-runtime-history"></a>

## Analysis — lịch sử trạng thái runtime và sequencing

Các snapshot sau mô tả thời điểm review trước; trạng thái mới đọc ở đầu §10. Không dùng chúng làm nghiệm thu layout hiện tại.

Bản mẫu mới kiểm one-press approach/cast, Nấm→Sói, EdgeExit và menu bằng bàn phím. Debug preset giúp tái hiện một đoạn mà không replay cả hành trình; đây là fixture có nhãn, không save authority hoặc evidence route thật. G-L vẫn PARTIAL tới khi có rig/import và manual usability/feel review ở tốc độ thường. Production base và Dedicated/backend chưa bắt đầu.

User yêu cầu sửa theo feedback và tự xử lý inconsistency; đây là nguồn của lượt sửa, không NSO recommendation tự thành luật. V6.2.0 prototype tại checkpoint `archive/checkpoint-46006c4` (local-only archive, không có trên origin) được giữ làm reference; build/tests/video cũ vẫn đúng cho revision đó, **không chứng minh V6.2.1 đạt G-L**. Production codebase chưa bắt đầu, không lấy các class prototype làm architecture authority.

[Review Art](../../docs/design/4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#art-review) đã tách nguyên tắc đủ dùng và con số chưa duyệt. CURRENT thu hoạch mock, probe feel/art/UI rồi review/dựng production base; G-L revision mới trước gate Dedicated hai Client và production rộng; gate backend thật và toàn TARGET giữ trong [Roadmap](../../docs/design/5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#phase-gates). Cung/Boss/PvP/SAVE giữ TARGET dù DEFER implementation khỏi slice.

Bộ power/MP/CD/gear làm control V6.2.4 giữ nguyên, nhưng rotation/resolve đã đổi; các bảng dưới đây là phép kiểm, không thay thế GDD. **TÍNH TỪ LUẬT** là phép tính xác định; **MÔ PHỎNG** phụ thuộc giả định; **ĐỀ XUẤT** chưa là luật. PvP có Food/Potion và recovery checkpoint mới nên mô hình PvP cũ chỉ là đối chiếu sát thương trực tiếp, không dự báo thắng/hòa; farm scheduler cũ cũng cần rerun vì combat/resource đổi. VS-1 disposable/reference prototype đã chạy Unity với route/tests **revision V6.2.0**; [evidence cũ](README.md) không nghiệm thu V6.2.1, production architecture, UX hoặc các mô hình balance/TARGET. Production codebase chưa bắt đầu.

## Phase F — báo cáo bảo toàn và SOURCE → DESTINATION

262 dòng/73 khối từ Roadmap §10 đã chuyển đầy đủ; 8 khối lấy mẫu seed1004 kiểm lại nội dung tại đích và so từng chữ sau khi bỏ routing/tag. §7–9 không di chuyển/xóa; chỉ hash routing sang tag archive đã xác minh. Read-only GDD/Technical/Art và ba README giữ byte. Một occurrence `26 frame` rời Roadmap cùng đoạn geometric rig và còn nguyên trong CHANGELOG; 53.100/95 điểm/32.000 và arithmetic1.800/200 không mất. [Audit độc lập](PrototypeEvidence/InputProbes/document-audit.json) và [toàn bộ mapping/hash](PrototypeEvidence/InputProbes/source-destination.json).

| Mẫu | SOURCE (Roadmap trước di chuyển) | DESTINATION kiểm thực |
| --- | --- | --- |
| 32 | §10, dòng484: **UI event boundary:** command có thể đổi panel và rebuild actions; mouse c… | [CHANGELOG](#prototype-technical-history) — kiểm actual content PASS |
| 8 | §10, dòng395: Các màu/tọa độ/mặt vuông là CURRENT PROBE, không khóa production. Docs 1–4 … | [CHANGELOG](#prototype-visual-review) — kiểm actual content PASS |
| 68 | §10, dòng590: Các snapshot sau mô tả thời điểm review trước; trạng thái mới đọc ở đầu §10… | [CHANGELOG](#prototype-runtime-history) — kiểm actual content PASS |
| 53 | §10, dòng533: **Water probe — feedback mới:** hub có basin nông, Mist có đáy suối thấp hơ… | [CHANGELOG](#prototype-ui-history) — kiểm actual content PASS |
| 48 | §10, dòng523: NPC root hiện lời nói mộc mạc, quest action đúng state và các chức năng của… | [CHANGELOG](#prototype-ui-history) — kiểm actual content PASS |
| 63 | §10, dòng561: **Unity / tooling:** prototype hiện có dùng Editor thực cài 6000.5.9f1; pro… | [CHANGELOG](#prototype-tooling-history) — kiểm actual content PASS |
| 57 | §10, dòng549: DoD của view: hoàn tất Q1–Q6 không click, submenu không lọt movement/skill,… | [CHANGELOG](#prototype-ui-history) — kiểm actual content PASS |
| 43 | §10, dòng506: Theo phản hồi từ người dùng, sơ đồ bản đồ cũ đã được gỡ bỏ hoàn toàn. Ba bả… | [CHANGELOG](#prototype-map-history) — kiểm actual content PASS |

`git diff --stat` đã kiểm: Analysis không bị cắt dòng; Art giữ nguyên. Link checker design284 local links,0 anchor gãy/0 file thiếu; các link trong CHANGELOG cũng được kiểm. Media48 payload và10 evidence TXT/JSON có chỉ mục, hash và vị trí rõ. Không upload/xóa media; thư mục lồng còn nguyên dù archive đã verify, chờ xác nhận mục0.6. Hook instruction ở CHANGELOG vì mục0 cấm sửa README; repo không có CI.

Bản Linux mới đã build thành công, all9 flags OFF được kiểm trong scene đã serialize; [build hash/summary](PrototypeEvidence/InputProbes/build-summary.json). Scene/settings trước build được khôi phục đúng bytes; binary build vẫn là reference/probe. Các sửa visual đang dở trước task vẫn ngoài commit code probe; §10 lưu bản nháp V6.2.8 chứ không nâng CURRENT V6.2.7.

Fresh Q1–Q6 trên binary mới, flags OFF: [route-result](PrototypeEvidence/InputProbes/route-result.txt) PASS bằng Rigidbody2D/menu keyboard adapter ở timeScale4, headless. Không preset hoặc inject quest/EXP/HP/position; không human feel/visual acceptance.

| File | Dòng trước → sau | Dòng bắt đầu `|` | Heading |
| --- | ---: | ---: | ---: |
| 1_HUYEN_LO_GDD.md | 806 → 806 | 336 → 336 | 26 → 26 |
| 2_HUYEN_LO_TECHNICAL.md | 437 → 437 | 124 → 124 | 18 → 18 |
| 3_HUYEN_LO_DESIGN_ANALYSIS.md | 545 → 558 | 273 → 276 | 25 → 25 |
| 4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md | 917 → 917 | 401 → 401 | 50 → 50 |
| 5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md | 598 → 380 | 166 → 153 | 35 → 18 |
| README.md | 90 → 90 | 45 → 45 | 6 → 6 |

Kiểm cuối: EditMode87/87, PlayMode16/16 (gồm58 EditMode và10 PlayMode cũ); all-OFF replay trùng baseline. XML/log của lượt kiểm lại giữ ngoài repo trong backup bền vững thay vì `/tmp`; hash/path ở validation. Chỉ10 TXT/JSON lịch sử được re-route qua chỉ mục; evidence nhỏ và bảng đo giữ Git. Source hash hiện tại trùng source đã kiểm, gồm thay đổi cảnh quan trước task.


<a id="single-behavior-history-1"></a>

## Historical source fragments — 1_HUYEN_LO_GDD.md

Các đoạn dưới đây giữ nguyên văn trước đồng bộ luật được chủ dự án duyệt. Chỉ là lịch sử; không áp dụng làm luật hiện hành. Source: `docs/design/1_HUYEN_LO_GDD.md`.

### Source fragment 1.1

````text
**Current Design Version:** V6.2.4
````

### Source fragment 1.2

````text
**Status:** DESIGN + PROTOTYPE VALIDATION; production codebase chưa bắt đầu
**Last Reviewed:** 2026-10-04 (feedback prototype; onboarding/control/navigation baseline đã sửa)
````

### Source fragment 1.3

````text
| Giai đoạn | Combat action | Identity |
| --- | --- | --- |
| Tân Lữ, chưa chuyển class | Basic Mộc Kiếm 1,00 × / CD 1,00 s / cận chiến 1,2 u; MP 0 | Onboarding Q3–Q5; giữ được ở Lv 5+ trước Q6 |
| Kiếm, sau chuyển class | S1 single → thêm S2 arc → thêm S3 line; ba slot tích lũy | Áp sát, Bỏng; không Normal Attack thứ tư |
| Cung, sau chuyển class | S1 single → thêm S2 spread → thêm S3 primary/explosion | Tầm xa, Băng Hàn; không Normal Attack thứ tư |
````

### Source fragment 1.4

````text
CombatFocus = NONE / AUTO / EXPLICIT, độc lập selectedSlot. ACQUIRE → RETAIN → REACQUIRE: chọn nearest eligible trong search bounds, tie stable entity ID; selection độc lập facing rồi auto-face khi start action. Search/retention khác execution range; reward level-gap không cấm combat/quest.
````

### Source fragment 1.5

````text
AUTO giữ target alive/eligible/cùng MapId/life còn đúng và còn relevant trong context; quái hơi gần hơn không cướp focus. Chết/invalid có thể acquire con gần hợp lệ tiếp theo; người chơi chủ động chạy/nhảy sang combat context khác có thể mất relevance. Hysteresis/search/retention/vertical bounds là TUNABLE; không sort nearest mỗi frame hoặc chain bãi xa. EXPLICIT do click là pinned: nearest và đổi slot không thay nó; chỉ explicit replacement/clear hoặc lifecycle invalid (death/despawn/generation/map) mới clear. Focus tồn tại không bảo đảm skill đánh tới; ngoài range không âm thầm đổi target để cast.
````

### Source fragment 1.6

````text
Thả phím không hủy pending một lần bấm; manual move/jump/drop, click đổi/clear focus, UI/Esc/death/chuyển map hủy nó ngay. Input skill hợp lệ mới thay pending/buffer cũ, không chồng nhiều đường chạy; không sửa action đã start. Bị blocked/quá xa/hết deadline/target invalid thì clear + feedback, không retry vô hạn. Một latest input buffer thử 150 ms chỉ quanh recovery ngắn khi skill sẽ sẵn; không FIFO hoặc queue dài qua CD/MP reject. Common action lock chặn action mới, đổi slot không reset CD. Cast start mới commit MP/CD và action origin; approach chỉ là movement, không đảm bảo hit hoặc miễn sát thương.
````

### Source fragment 1.7

````text
| Action | Logical resolve | Action lock |
| --- | --- | --- |
| Basic Tân Lữ | +0,10 s | 0,26 s |
| Nhập môn single (hai class) | +0,12 s | 0,30 s |
| Phong Trảm tiến cảnh | +0,14 s | 0,30 s |
| Linh Tiễn tiến cảnh | Ba logical hits cùng +0,12 s | 0,34 s |
| Kiếm Khí | +0,16 s | 0,40 s |
| Hàn Tiễn | +0,18 s | 0,40 s |
````

### Source fragment 1.8

````text
**Melee crowd feel — baseline nguyên tắc, exact behavior TUNABLE:** soft separation và mục tiêu đứng lệch nhau trên lane giúp 2–4 con còn trong tầm AoE nhưng không trùng một điểm. Sau đòn, có bước chỉnh vị trí/lùi ngắn khi hợp lệ; không bắt mọi loài lùi mỗi hit hoặc đồng bộ cả đàn. Reposition dùng recovery/interval hiện có, không tự giảm attack interval hoặc thêm guaranteed safe window. Windup/hit origin đã start không bị steering sửa; không knockback/body shove. Thử offset attack positions theo trái/phải cùng lane, ưu tiên khoảng trống + stable ID; **không khóa 3–4 slot**, vòng tròn bao player, group attack token hay formation subsystem. Cố định phase lệch nhau theo life để tránh đồng loạt cắn; spacing/time/offset kiểm PHY-01, không suy DPS mới từ mô hình cũ.
````

### Source fragment 1.9

````text
| Phím | Action | Phím | Action |
| --- | --- | --- | --- |
| A / ←, D / → | Move trái / phải | H | Quick HP Potion |
| Space / ↑ | Jump | M | Quick MP Potion |
| S / ↓ | Drop-through trên one-way đang đứng | F | Food |
| — | — | E | Interact / Pickup (không dùng cho MapExit thường) |
| 1 | Basic Tân Lữ / Select S1 + one-shot approach/cast | I | Inventory |
| 2 | Select S2 + one-shot approach/cast | C | Character + skill tab |
| 3 | Select S3 + one-shot approach/cast | Q | Quest |
| R | Buff P1 | Enter | Chat |
| Esc | Cancel | — | — |
````

### Source fragment 1.10

````text
**Binding baseline cho prototype tiếp theo:** I Inventory, C Character (gồm skill tab), Q Quest; không B/K/L panel bindings song song. Đây là bộ mặc định để kiểm usability, chưa cam kết tối ưu hoặc thêm key-remapping P0. Space/↑ và S/↓ là OR action; drop chỉ trên one-way đang đứng, không crouch/đi xuyên solid. Nếu Jump + Drop cùng frame trên one-way thì Drop ưu tiên; trên solid Jump vẫn hợp lệ.
````

### Source fragment 1.11

````text
**Menu bằng bàn phím:** Interact mở NPC với action phù hợp được chọn sẵn (nhận/trả quest trước, rồi service). Trong modal: ↑/↓ hoặc W/S, Tab/Shift+Tab đổi lựa chọn; Enter hoặc Interact xác nhận; Esc đóng. Arrow/Space/1–3 không lọt thành movement/cast khi UI giữ focus. Enter chỉ mở/submit chat khi không có modal khác; Tab ở đây là UI navigation, không thêm combat target cycling. Inventory/equip/learn, shop buy/sell, character/skill tab, rương và revive đều có focus rõ, text/action disabled reason và cùng command validation cho chuột/bàn phím. Không yêu cầu click để hoàn tất quest. Click explicit focus vẫn tùy chọn; auto-acquire và clear focus đủ cho route keyboard.
````

### Source fragment 1.12

````text
Phím H/M chọn bình **bậc thấp nhất hiện có, đủ cấp dùng và đủ hồi phần HP/MP đang thiếu**; nếu không bình nào đủ bù, dùng bậc cao nhất hợp lệ. Game Server kiểm túi, cấp, số lượng và hồi chiêu; đầy HP/MP hoặc đã chết thì từ chối, không tiêu bình. Q6 dùng Bình Linh Lực I đã phát trước bình khác để không kẹt hướng dẫn. Phím F dùng Food bậc cao nhất hợp lệ; Food mới thay hiệu ứng cũ và đặt lại thời hạn 10 phút, không cộng dồn. E tác động ngay candidate NPC/loot riêng, không thay CombatFocus.
````

### Source fragment 1.13

````text
**Đóng giao diện theo thao tác:** nhận/trả quest, nhập phái và nghỉ thành công đóng hội thoại để tiếp tục đi; câu xác nhận vẫn hiện trên NPC/tracker. Lỗi/reject giữ view và reason. Buy/Sell/Store/Take giữ view để làm nhiều lần; Esc lùi một submenu, ở root thì đóng. Equip/Unequip/Learn thành công trở về view chứa item/slot; tab switch đi trực tiếp tới view mới, không giữ submenu cũ. Intro hiện trước khi nhận quest; không bỏ narrative chỉ vì auto-close.
````


<a id="single-behavior-history-2"></a>

## Historical source fragments — 2_HUYEN_LO_TECHNICAL.md

Các đoạn dưới đây giữ nguyên văn trước đồng bộ luật được chủ dự án duyệt. Chỉ là lịch sử; không áp dụng làm luật hiện hành. Source: `docs/design/2_HUYEN_LO_TECHNICAL.md`.

### Source fragment 2.1

````text
**UI focus/submenu:** modal giữ breadcrumb + selected action/itemInstanceId, không giữ closure tới item đã bị consume/reset. NPC root chỉ hiện quest/service; service view lấy đúng danh sách; Esc quay một cấp rồi đóng, I/C/Q mở root mới. Bag grid điều hướng theo hàng/cột và ô rỗng, detail dùng cùng domain validators; equipment slots có selected/empty/locked cues. Markers derive quest state, dialogue không tự grant objective/reward. Class admission kiểm slot Vũ khí trống trước staged grant; tháo/cất là commands riêng.
````

### Source fragment 2.2

````text
**MapTransition contract:** `EdgeExit` là trigger mép map + targetMap/targetExit/spawn anchor; authority kiểm actor alive, MapId/generation, overlap đúng exit, connected destination và unlock, không nhận arbitrary destination từ client. `SpecialGate` có activation/interaction riêng theo GDD (Huyền Môn/Arena), không chung E cho mọi exit. Hai loại đi qua cùng pipeline MapId/checkpoint/cancel. Một pending transition/actor, dedup request/trigger; vào vùng đích ngoài return trigger, chỉ re-arm sau khi rời exit. Reject hiện reason một lần và không spam retry khi đứng tại biên; cần rời rồi vào lại. Actor khác không bị khóa exit. Backend commit destination checkpoint trước publish theo §6; fail giữ nguồn và không cho destination snapshot giả. Collider/safe-strip/camera và pending ACK phải kiểm riêng, không lấy trigger local prototype chứng minh online.
````

### Source fragment 2.3

````text
**ActionTimeline:** authority kiểm requested SkillId, class/learned/level, MP/per-skill CD/common action lock và target/geometry; accepted start tạo actionId/life/startClock, snapshot SkillId/profile/source stats/passive/origin/facing rồi commit MP/CD đúng một lần. Một latest intent buffer thử 150 ms, không FIFO; running action không đọc mutable selectedSlot. Một timeline cho Novice/slot requests; không J hoặc held-repeat executor. Logical ranged resolve tại clock trong GDD, không gameplay projectile object. Spread assignments ABC/ABA/AAA snapshot ở start, cùng +0,12 s stable hitIndex; invalid index mất hit, không reacquire. Line sort intersections; Hàn invalid primary không nổ, primary Evade vẫn nổ/secondary roll riêng/no primary double-hit. Dedup actionId+hitIndex+targetId+generation; revalidate target lifecycle/MapId/shape và DEF/EVA tại resolve. Death/map transition/CC hủy unresolved action, không refund; result đã resolve bất biến. Presentation projectile không giữ callback mutate HP.
````

### Source fragment 2.4

````text
Input System actions: Move (A/←, D/→ OR/clamp không cộng speed), Jump (Space/↑), DropThrough (S/↓), Slot1/2/3, Interact và panels theo GDD §9. Không ExecuteSelected/J hoặc RepeatOnHold. Press Slot1 chọn basic khi Novice hoặc S1 sau class; Slot2/3 chỉ khi learned/unlocked. Một physical press → một requested SkillId/intentId; giữ input không emit performed requests liên tục. Slot hợp lệ vẫn selected khi reject, locked slot không đổi selection hoặc tạo queue. Authority không trust selectedSlot/secondary list client.
````

### Source fragment 2.5

````text
CombatFocus NONE/AUTO/EXPLICIT giữ target ID+generation+MapId riêng selection. Retain trước acquire; AUTO context hysteresis không nearest sort mỗi frame. NONE acquire tại press trong local eligibility/envelope; không có target thì reject, không tạo pending chờ spawn. EXPLICIT/range reject không thay target để cast. Loot candidate riêng; E act ngay, pickup next candidate không mutate focus.
````

### Source fragment 2.6

````text
`PendingCast` nhỏ cho một press: requested SkillId, target/life/map, intentId, expiry, start/progress position; không source-stat/action snapshot hoặc commit cost lúc approach. Validate learned/state/MP/CD trước movement, trừ một latest buffer recovery ngắn nếu skill sẵn tại recovery end. In-range start ngay; hơi ngoài range và cùng reachable lane thì bounded run. Tới tầm revalidate target/profile/MP/CD/state/geometry rồi accepted start mới snapshot/commit. Target invalid/changed, blocked, hết budget/timeout hoặc manual movement/jump/drop/UI/Esc/death/map transition → clear + reason, không reacquire/cast đích mới cùng press. Press unlocked mới replace pending/buffer cũ; running action immutable. Release không cancel one-shot pending, hold không tạo cast thứ hai. Một latest buffer thử 150 ms, không FIFO/queue chờ CD dài hoặc thiếu MP. Reject không giữ intent để bất ngờ cast sau này.
````

### Source fragment 2.7

````text
**UI input boundary:** modal giữ một selected action ID ổn định; build cùng danh sách action/điều kiện cho keyboard và mouse. Menu coordinator nhận Navigate/Confirm/Back; renderer chỉ vẽ focus, disabled reason và dispatch command, không mutate progression. NPC mới mở ưu tiên quest action; mở modal consume input vừa dùng, không xác nhận thêm trong cùng frame. Khi action làm list đổi, giữ selected ID còn tồn tại hoặc clamp index; không giữ callback vào item/session cũ. Close/transition/reset clear pending gameplay, UI capture và buffer; giữ phím không tự resume. Enter thuộc Confirm khi modal, Chat khi world; Tab/Shift+Tab đổi RPG tab, arrows giữ grid navigation. Tab ngoài RPG chọn action, không combat cycling. Tab coordinator lưu selected action ID theo view; context bag filter theo GearSlot, không copy inventory. Equipment/Attributes/Derived Stats là ba view riêng đọc cùng evaluator; preview và world đọc cùng module/pose/socket contract; technique cụ thể phải qua art gate. NPC acceptance/completion text chỉ presentation, không mutate reward ngoài domain command.
````

### Source fragment 2.8

````text
| Bộ nghiệm thu | Ca bắt buộc | Evidence |
| --- | --- | --- |
| Progression | Cumulative 53.100, carry EXP, catch-up remaining, reset Lv 5 một lần, trì hoãn Q6 tới Lv 6+ giữ total points / normal Mộc Kiếm và M reserved potion, 95 điểm, không branch cap / rank | State trước / sau và reload |
| Combat / status | BaseHP/MP theo cấp; nội tại Kiếm Tâm/Ưng Nhãn nhân final stat một lần, Kiếm Thế/Xạ Tâm +12% direct skill đúng cự ly; basic Tân Lữ CD1,00 s; sau class chỉ S1/S2/S3 tích lũy/CD riêng/common lock, Lv 5 active vẫn single, Lv 10 mới đánh lan, Lv 17 big; A/B/C fallback/no reacquire, per-unique-target status roll cached kể cả fail; S2 độc lập giữ CD; Bỏng 4%/70%, 6 s/1 s/6% ATK, refresh source + expiry nhưng không reset tick; Freeze 1,5 s Normal/Linh + miễn 3 s, test 1/2/4 Cung; Boss Slow 3 s/75% action wait **không reset countdown/action đã start**; PvP Slow 25% move 1,5 s, không attack/CD slow; không nhân status khi nhiều caster | Game Server clock / stat evaluator / status logs, không AnimationEvent |
| CC / AI | Sticky per-mob threat 1,25 ×, group wake no copied threat; dead / disconnect / map transition / leash retarget, Return clears status / ledger; front / vertical miss, no contact / shoving, projectile snapshot, Ong melee-accessible | N-player target / status tests |
| World / timers | 2 / 3 / 4 players same-map farm, no whole-map starvation, safe exit strip, no chain aggro cả map, traversal / run-back logs; N-player root occupancy; drop-through một actor không ảnh hưởng actor khác; density theo GDD matrix TEST / TUNABLE, Return reset HP, deadlines map rỗng / re-entry, respawn idempotent | Clock / MapId logs |
| Boss | New world/Alive boot đúng một con; HP 32.000, ATK 160, DEF 25, ACC 140, EVA 60; chết hồi sau 15 phút / demo 60 s, không spawn khi nhận Q12; một action theo thứ tự GDD, ba vùng đá không double-hit; Cuồng Mạch chỉ đổi nhịp về sau, Làm Chậm chỉ giảm tốc phần chờ còn lại; ngưỡng 10% = 3.200/demo 1.280, corpse đủ điều kiện, không EXP/Vàng trực tiếp, một pile/Thỏi/cửa nhặt 12–30–90 s | Lifecycle/clock/credit/claim logs, 2/4 người |
| RPG — thưởng/nhặt | Mỗi người nhận phần EXP/Vàng theo đóng góp, làm tròn xuống, không chia lại; lệch cấp ≤3 nhận đủ, ≥4 không có thưởng farm. TopDamage không đủ level thì không roll set thường, không fallback. Shared pickup 8/20/60 s; một item chỉ một claim. Level tăng từ kill không tước quyền nhặt đã chụp; người tới sau dùng level hiện tại. | Race claim, túi đầy, deadline và crash logs |
| RPG — catalog | 18 dòng / 21 mẫu thường có tên riêng; Kiếm thêm Chí mạng 0,5/1/1,5 điểm %, Cung thêm ACC 10/20/30, Giày thêm tốc chạy 1/2/3%. Quái Lv 2/4 chỉ rơi năm ô không vũ khí ở mọi map. Mặc I không vũ khí Lv 1, vũ khí I Lv 5, II Lv 11, III Lv 17. Ong Lv 10 rơi II nhưng chưa mặc/chuyển vào đích II trước Lv 11. Dây chuyền III bán 225, Thỏi bán 250; cường hóa không tăng giá bán. | 21 templateID/tên duy nhất; sai phái không mặc/chuyển; Chí mạng/ACC/tốc chạy tại +0/+4/+8, preview/load khớp; nguồn rơi, shop và cấp mặc |
| RPG — cường hóa/chuyển giao | Trần I+4/II+6/III+8; Tinh Hoa I +4, II +8; thất bại giữ cấp nhưng tiêu chi phí. Chuyển cùng bậc 800 Vàng hoặc lên đúng bậc kế 500 Vàng +2 đá; cùng ô/đúng loại vũ khí, đủ cấp, hai instance khác nhau trong túi. Từ chối nếu đích không tăng; nguồn bị tiêu, đích giữ ID/phẩm chất; receipt replay không trừ/cấp lại. | +0→+8, vượt trần, khác phái, no-gain, hai lệnh tranh nguồn, save failure |
| Linh Biến / economy | Chance 5% only Lv 8+, HP × 5 / EXP-Gold × 3, cap 1 under concurrent respawn, fixed mobIdentity / fixedLevel validate slot cache, respawn25 s; Q8 reservation waits live variant / no demote, same Linh rewards, no force replay; Stone / gear / Gold-hour and crowd contention | Seed / slot / generation logs and measured throughput |
| Quest | Một bảng Q1–Q12 GDD là authority; Q4 Nấm DS2/loot/equip/sell; Q5 5 Sói DS3–DS6, Q8 4 Sói TA4+TA6 rồi force `TA4.slot1`, Q10 6 Đoạt XN1–XN3/evidence #2/#4/#6, Q11 3/3/4 Thạch theo XN4/5/6, Q12 6 Cổ HT4+HT5; active virtual evidence, no pre-farm/overcount; damage trước accept/sai step không hồi tố, late quest không bị level penalty softlock; Q4–Q7 supply/manual/full bag/replay, Q9 optional, Q11 activation ngoài portal | Solo/late/online/reconnect/full-bag journey |
| PvP / chat | Wager 1.000–10.000 bước 1.000; accept đúng stake, cả hai đủ Vàng/escrow một transaction trước MatchId; thiếu tiền/đồng thời accept hai lời mời không trừ một phía. Food tick; HP/MP Potion riêng CD 8 s, mỗi loại tối đa 3/người/MatchId, mỗi use tiêu đúng một bình, reject không tiêu; cấm Hồi Sinh Phù. Active sau countdown/ACK; HP=0 hoặc disconnect sau Active thắng/FORFEIT; disconnect trước Active hủy; 120 s cả hai sống hòa **không so HP**; hệ số PvP 0,20 và Slow đúng luật. Chat theo MapId, 80 ký tự/rate. | Test cả 10 stake; W=1.000: WIN trả 1.800/fee200, DRAW trả 900 mỗi người, cancel/SYSTEM_ABORT trả 1.000 mỗi người; WIN/FORFEIT +200 Journey một lần, Q9 không credit FORFEIT/abort; match logs hai Clients |
| Recovery / backend / network | N-player registry/recipient sets; ticket/lease, resume token một lần trong 15 s giữ exact runtime, actor vẫn chịu hit; mất phiên load checkpoint MapId/HP/MP ở một SafeAnchor farm/combat, safe-zone coordinate hợp lệ/fallback, Arena không restore, HP=0 vẫn chết. Periodic 30 s, map-transition/logout/death/revive/PvP critical; PvE potion consume+HP checkpoint cùng transaction, PvP potion consume chỉ ghi item/receipt và giữ pre-Arena checkpoint; stale seq không overwrite. Escrow HELD/BIND/ACTIVE/settled crash hoặc mất ACK: refund 100% nếu SYSTEM_ABORT/pre-Active, không refund sau settled; death/claim/enhance receipts vẫn sống qua crash; schema migration fail chặn startup. | PostgreSQL transaction tests, checkpoint race/failure, orphan reconciler, 2/3/4 Client network/crash logs |
| Art / UI | 64 × 64 / PPU 32 / 26 frames, pivot/frame alignment, gear flip, map anchors; phân biệt Bỏng/tia lửa, quái Đóng Băng/băng vỡ, Boss/PvP Làm Chậm/phủ lam mờ; input context | Import audit / video |
````


<a id="single-behavior-history-3"></a>

## Historical source fragments — 3_HUYEN_LO_DESIGN_ANALYSIS.md

Các đoạn dưới đây giữ nguyên văn trước đồng bộ luật được chủ dự án duyệt. Chỉ là lịch sử; không áp dụng làm luật hiện hành. Source: `docs/design/3_HUYEN_LO_DESIGN_ANALYSIS.md`.

### Source fragment 3.1

````text
Bộ power/MP/CD/gear làm control V6.2.4 giữ nguyên, nhưng rotation/resolve đã đổi; các bảng dưới đây là phép kiểm, không thay thế GDD. **TÍNH TỪ LUẬT** là phép tính xác định; **MÔ PHỎNG** phụ thuộc giả định; **ĐỀ XUẤT** chưa là luật. PvP có Food/Potion và recovery checkpoint mới nên mô hình PvP cũ chỉ là đối chiếu sát thương trực tiếp, không dự báo thắng/hòa; farm scheduler cũ cũng cần rerun vì combat/resource đổi. Các mô hình dưới đây không nghiệm thu runtime; kết quả bản mẫu thuộc [Roadmap](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#prototype-runtime-history).
````

### Source fragment 3.2

````text
**Cách tính:** nền theo cấp + điểm + trang bị đã tính phẩm chất/cường hóa/Tinh Hoa, sau đó mới áp nội tại nền tảng một lần. Các hàng sau chọn phái dùng sáu món Common +0 theo mốc mặc, gồm Chí mạng trên Kiếm, Chính xác trên Cung và tốc chạy trên Giày (Lv 5/10 bậc I, Lv 13 bậc II, Lv 17/20 bậc III); đây là bộ kiểm chỉ số, không giả định người chơi đã mua đủ set. Trong các ô ghi hai số, thứ tự luôn là **Kiếm / Cung**. Hồ sơ Lv 5 trước chọn phái trong bảng là **stat fixture V6.2.0** với Mộc Kiếm + Quần I, chưa hưởng nội tại; không mô tả inventory route V6.2.1 đã nhận Áo từ Q4. Giữ số để đối chiếu, không tự cộng lại bảng này thành acceptance mới. Chính xác/Né tránh là chỉ số thô dùng trong công thức né, không phải phần trăm trúng/né; Cung được nhân Chính xác ×1,08 sau khi cộng điểm và trang bị. Tốc chạy tăng so với nền gồm +0,05% mỗi điểm Thân Pháp và bonus Giày cố định 1/2/3% theo bậc.
````

### Source fragment 3.3

````text
Sói Sương Lv 4 tại Lv 5 là quái thấp cấp sau cú nhảy chọn phái, nên hạ trong ~1,4 s; Tân Lữ Lv 4 với Mộc Kiếm hạ trong ~3,9 s. Cụm ba/bốn con ở Lv 8+ còn sống qua nhiều action, cho kỹ năng đánh lan giá trị. Q8 Sói Trúc Linh Biến có **1.695 HP**, với đồ kỳ vọng mất **22,7/23,7 s** (Kiếm/Cung) solo. Q10 ở Lv 15 đánh ba Đạo Tặc mất **10,0/14,0 s**; Đạo Tặc Lv 13 còn thưởng đủ. Lv 15 đánh Thạch Lv 16 trước đại chiêu hơi chậm. Trước Q11, Lv 17 đánh ba Thạch bằng đồ II mất **8,8/13,3 s**; cần kiểm né và dùng thuốc trong cảnh thật để tránh kẹt khi ba quái cùng áp sát.
````

### Source fragment 3.4

````text
**LEGACY TIMING/ROTATION EVIDENCE V6.1:** toàn bộ các bảng/phép tính trong nhóm timing/sustain/proc sau đây giả định class Normal + evolution overwrite. Giữ nguyên để đối chiếu; các recommendation cũ không còn luật. GDD §3 hiện dùng basic chỉ Tân Lữ, ba class SkillIds độc lập, logical ranged batch.
````

### Source fragment 3.5

````text
**LEGACY / SUPERSEDED — class Normal V6.1, không phải reasoning hiện hành:** Normal từng không MP, lấp quãng CD/MP thiếu và gây damage ổn định. Trong giả định skill luôn sẵn đúng CD, Lv10 core chiếm lock khoảng0,30/1,5 hoặc0,34/1,7 =**20% thời gian**; Lv17 thêm0,40/7≈5,7%. Đây chỉ là occupancy suy ra, không bảo đảm normal đạt full attack rate vì mốc CD có thể đụng nhau. Cần log số normal/skill/idle do MP và action lock; không kết luận normal vô dụng từ CD1 s của Lv5.
````

### Source fragment 3.6

````text
**Gameplay đã chốt:** GEAR-01 (trần +4/+6/+8, hai Tinh Hoa, chuyển giao); CONS-01 (hai hồi chiêu bình riêng); BOSS-02 (DEF25/ACC140/EVA60, ba vùng đá, một action/lịch ưu tiên). COOP-01, QUEST-02, BOSS-03, BOSS-01 và NAR-01 cũng đã chốt. **PVP-01 V6.1:** cược 1.000–10.000 bước 1.000, escrow cả hai trước MatchId, Food/Bình hợp lệ với quota 3+3, 120 s cả hai sống DRAW, fee/refund qua Spring receipt. **SAVE-01 V6.1:** PostgreSQL giữ tiến trình và MapId/HP/MP checkpoint; resume phiên còn trong RAM hoặc spawn SafeAnchor từ checkpoint. Stat/loot arithmetic giữ; TTK PvE và sustain/journey cần rerun; mô hình PvP chưa tính hồi phục mới.
````

### Source fragment 3.7

````text
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
| INPUT-01 — PLAYTEST / PROPOSAL | So A/B stale held axis, Esc, approach × EdgeExit/arrival enum, latest buffer0,18s, aliases4/5 và keyboard target cycle A | GDD150ms, H/M; Tab chỉ UI. Recorder seed731: reject119/171, held cancel20/20, exit leak1/1; [phương pháp và giới hạn](../../prototypes/VS1_EndToEnd/CHANGELOG.md#phase-b--baseline-trước-sửa-hành-vi) | Mặc định giữ baseline; chỉ đề nghị sync sau review normal-speed/rollover và giải thích reject theo cadence. Không sửa AUTO hysteresis/ID tie từ probe |
| CMB-01 — PLAYTEST / PROPOSAL | Chỉ basic Tân Lữ: CD1,00→0,70s, Lock0,26→0,32s, hit0,10s giữ; dead time74%→54,3% | GDD1,00/0,26; recorder TTK Lv4/1Sói4,96s, Lv3/3Dummy9,78s; S1/S2/S3 Kiếm, mob HP/EXP/speed/range giữ | Chạy lại TTK, ngân sách EXP và Q3–Q5; feel0,70s còn CHƯA ĐO. Không sửa bảng TTK cũ hoặc GDD chỉ vì test pass |
| MOBAI-01 — PLAYTEST / PROPOSAL | Bỏ flip sau cắn; khoảng trống trái/phải0,9×range/tie SpawnSlot, hysteresis1,5s, reposition tùy loài0,8–1,0u/phase theo life, deadband facing và quadratic separation | GDD melee crowd không formation/slot/token/shoving; recorder3Sói:3,3333 flips/quái/5s, mất131HP/10s. Runtime baseline cũ giữ để đối chiếu, không dùng làm production contract | Đo áp lực ±10%, flips≤2/5s, crowd1/2/4 và mép. Không thêm interval/safe window hoặc sửa windup origin để ép metric; sai ngưỡng thì báo cáo |
````

### Source fragment 3.8

````text
<a id="input-combat-mob-proposals"></a>
````

### Source fragment 3.9

````text
**Phạm vi thử INPUT-01:** mỗi physical press vẫn một intent. Snapshot các phím ngang giữ lúc press; trong pending, bỏ qua chúng **kể cả đang giữ ngược hướng**, rồi trả quyền cho trục đang giữ khi cast/fail/cancel. KeyDown mới, Jump/Drop/Esc, đổi/clear focus, UI/chat/death/map hủy intent. Đây là thử nghiệm quyền điều khiển, không luật đã khóa. Không approach trong action lock; latest buffer0,18s chỉ nhận khi cooldown/lock sẽ sẵn trong cửa sổ, không FIFO hoặc long-CD queue. Approach cắt exit trigger thì blocked trước cost; chỉ manual input chuyển map. Arrival kiểm ID/generation (life), MapId, alive/notReturning, capability, weapon/learned, MP/CD/lock và geometry tại origin thực; thất bại có enum reason và không commit. Hard-CC chỉ fixture kiểm cancellation, không cấp Freeze cho player/Boss. Esc: modal/chat trước → pending/buffer → focus → no-op.
````

### Source fragment 3.10

````text
**Keyboard target options:** **A thử sau flag:** Tab/Shift+Tab trong world cycle tiến/lùi, thứ tự khoảng cách ngang rồi SpawnSlot ID, EXPLICIT pinned và lifecycle clear; modal vẫn UI navigation. **B chỉ đề xuất:** action CycleTarget gắn phím riêng (không R, đã giữ cho Buff P1). **C chỉ ý tưởng, không code:** ưu tiên Threat/HP thấp thay AUTO; mâu thuẫn với hysteresis/tie-ID hiện tại và cần review riêng. Aliases4/5 nối HP/MP với H/M; cùng consumable command, không đổi quota/cost/CD. Tutorial/glyph đọc bindings của probe. GDD §9 chưa đổi.
````

### Source fragment 3.11

````text
**Câu hỏi giữ mở trong INPUT-01/CMB-01/MOBAI-01:** Dummy là training actor ngoài bảy combat identities/sáu mob rig, không tự cộng roster; cần kiểm wording/manifest. Học Viện an toàn có sân Bù Nhìn không gây damage, cần kiểm consistency với save/safe-zone. Không fast travel P0: đo chi phí đi bộ/turn-in, không thêm tiện ích tự động. Ngưỡng “sắp sẵn” của buffer chưa có số canonical;0,18s là giá trị probe. Rollover D+Space+1(+2), desktop/laptop, WASD/mũi tên với4/5 và feel0,70s: **CHƯA ĐO bởi người thật**. Các hành vi mới chỉ nằm sau ProbeConfig mặc định tắt; findings dùng để review Production Base G-B, không pass G-L.
````

### Source fragment 3.12

````text
**Findings của probe, chưa duyệt luật:** [báo cáo A/B và giới hạn](../../prototypes/VS1_EndToEnd/CHANGELOG.md#phase-e--kết-quả-ab-và-giới-hạn) giữ seed/kịch bản baseline. All ON cho flips3,3333→6,1667/quái/5s và damage131→166HP/10s (+26,7%): MOBAI-01 **chưa đạt**, không đổi baseline. Chỉ AI ON cho5,8333 flips/142HP; chỉ input ON cho3,3333 flips/122HP. Damage còn phụ thuộc life hữu hạn, target và RNG; không dùng một mẫu ±10% để chứng minh không có safe window. INPUT-01 giảm reject69,59→51,46%, chưa đạt10%; lịch press nhanh hơn cooldown cần được tách khỏi lỗi cancellation. CMB-01 đạt phép tính dead time54,29%, TTK mẫu3Dummy9,78→6,42s; đó chưa là kết quả fresh journey Q3–Q5 hoặc feel acceptance. Không cập nhật bảng TTK cũ từ các fixture này.
````

### Source fragment 3.13

````text
**Rủi ro kiến trúc V6.1:** auth/ticket/lease, checkpoint ordering/validation, reconnect grace, escrow orphan recovery, transaction N recipients, backend downtime và dedicated build/headless physics cần integration tests. Mốc giờ player-host/JSON cũ không còn dùng để cam kết lịch; spike đầu đo integration/latency, sau đó Technical mới đặt lại ngân sách. Quest, gear, combat PvE và world balance giữ số hiện tại; các luật PvP/recovery trên là user lock, không Open Decision.
````

### Source fragment 3.14

````text
## DESIGN LOCK 2026-10-03 — rationale, delta và gates
````

### Source fragment 3.15

````text
User approval lần này là nguồn của các thay đổi; NSO research chỉ evidence. Basic Tân Lữ giữ theo trạng thái trước class, kể cả Q5 Lv5; không hard cutoff làm onboarding kẹt. Sau class chỉ S1/S2/S3 tích lũy, sáu class SkillIds/CD riêng; sáu books/bốn passives giữ. Control J/hold của lượt sync V6.2.0 đã **SUPERSEDED bởi feedback V6.2.1**: 1/2/3 one-press, bounded approach + cast một lần dùng một resolver/lock/clock; focus độc lập slot, AUTO sticky theo context, EXPLICIT pinned; loot candidate riêng E act ngay. Logical ranged result bỏ flight damage/interception, mất né visual tên sau resolve nhưng giữ melee/Boss geometry dodge. Movement/momentum không bị normal damage interrupt.
````

### Source fragment 3.16

````text
| Review decision | Kết luận / trade-off | Gate còn phải đo |
| --- | --- | --- |
| D01/D11/D12/D19 | Logical hit + shape identity + visual-only projectile; source snapshot/target resolve, dedup hit/status | Geometry/lifecycle/visual timing G-L rồi G-N; rerun simulation |
| D02–D05/D09/D22/D28 | AUTO acquire-retain-reacquire/EXPLICIT pinned; selection khác execution range; one-press pending/short latest buffer; minimal target HUD | Context retention, range, pending replace/cancel/modal/spam; release không cancel pending, hold không repeat mọi skill |
| D06–D08/D10 | Damage feedback không Hurt/hit-stun/knockback; gravity/momentum liên tục; không recovery cancel P0 | Novice/S1 air/control, S2/S3 permissions còn prototype |
| D13/D16 | Lightweight logical lane/flying bounds, Ong giữ flying/ranged và band reachable | PHY-01, G-N positions/headless; không full Rigidbody/nav graph |
| D14 | Giữ ba hybrid hiện hành; so 3/1/0, thử Cổ Vệ làm một đại diện; cắt ranged là đổi content identity | PROTOTYPE + user content approval; không tiết kiệm pose count trước duyệt |
| D15/D15R | LOCK chống free-farm melee unreachable; exact grace/Return/regen/reset/untargetable/immune chưa chốt | Jump ngắn/perch/kite/2 players/reachable retarget; reset không reward/reroll/life mới |
| D17/D18/D20 | LoS A/B PROTOTYPE; full geometry LoS DROP P0; 25 s/28 pockets/66 slots và rates giữ control | Room/playtest/benchmark workload thật; không CPU%/số dòng recipe proof |
| D21/D23/D24 | Per-profile visuals; Tab/Auto/gamepad/hitstop/crit shake/material audio DEFER; 26 frame/A01/A02 giữ OPEN | Art imported rig/weapon/readability/hour/% usable; không 33 pose lock |
| D25–D27/D29/D30 | Backend integrity giữ; VS-1 trước; ba accumulated slots/CD riêng, không zero-MP class fallback | Harvest/probe → production base → G-L mới → G-N/G-D; independent-CD weave đổi burst/resource |
````

### Source fragment 3.17

````text
Research claims về CPU/GPRS/T9 motive là HISTORICAL INFERENCE; “1000 mobs <3% CPU”, deterministic/anti-cheat tuyệt đối hoặc số dòng recipe là UNSUPPORTED, không justification. Client/server splash thresholds khác nhánh, map parse chưa xác minh không khóa geometry. Ba báo cáo giữ ở [research notes](../../research/README.md), không design authority. Không triển khai navigation/DropLink/universal ranged fallback/Auto/Tab/rich UI/rollback để kế thừa NSO.
````

### Source fragment 3.18

````text
## Keyboard UX và editorial review — 2026-10-04
````

### Source fragment 3.19

````text
## Feedback prototype 2026-10-03 — V6.2.1, quyết định tạm và giới hạn evidence
````

### Source fragment 3.20

````text
User yêu cầu sửa theo feedback và tự xử lý inconsistency là nguồn của lượt sửa design; NSO recommendation không tự thành luật. Lịch sử code/checkpoint được giữ tại [Roadmap](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#prototype-runtime-history).
````

### Source fragment 3.21

````text
| ID / xử lý | Kết luận và lý do | Gate / chưa chốt |
| --- | --- | --- |
| F01 — BASELINE | Q3 Dummy → Q4 Nấm/loot/equip/sell → Q5 Food/HP supply/Sói → Q6 class. Giữ QuestIds, đổi nội dung Q4/Q5; Q3 0 EXP, Q4 catch-up Lv4, Q5 Lv5; EXP floor không downlevel người đã farm vượt mốc. Áo có trước Sói; 45 EXP Q5 cố định cũ được thay bằng catch-up, không cộng hai reward; MP tutorial ở Q6 tránh mua rồi chưa dùng | QUEST-03: normal-speed route/UI mock, độ khó Tân Lữ và shop load; không tăng NeedEXP/mob HP |
| F02 — BASELINE | MapExit/EdgeExit thường auto-transition với edge arrow/name; SpecialGate cho Huyền Môn/Arena. Quest objectives lưu hành động, glyph theo bindings; validation/checkpoint giữ cho mọi transition | PHY-01: no ping-pong/stale trigger/locked-exit spam, destination safe spawn |
| F03 — BASELINE | Bỏ J/combat repeat. Mỗi press 1–3 acquire/retain → bounded horizontal approach nếu hơi xa → cast một lần. Release không hủy intent một lần bấm; manual/focus/UI/map/expiry hủy, invalid target không reacquire cùng press | ART-01/PHY-01: pending phải đọc được; short buffer không queue dài; reject chưa ready/blocked/quá xa không cost/movement |
| F04 — PROTOTYPE | Space/↑ jump, S/↓ drop trên one-way; I/C/Q panel default thống nhất. Coyote/buffer/variable height/acceleration/fall feel cần sample thật | Chưa khóa forces/duration hoặc hotkey tối ưu; không key-remapping/double-jump/dash P0 |
| F05 — PRINCIPLE + TUNABLE | Melee cluster không pile; soft separation, lệch phase và recovery reposition hợp lệ trên lane. Không khóa 3–4 ring slots/group mutex; không kéo interval thêm hoặc sửa facing/hit origin lúc windup | PHY-01: 1/2/4 mobs, near wall/edge, nhiều player, AoE vẫn giữ group value; không dùng toy DPS để duyệt spacing |
| F06 — CURRENT | Prototype folder/evidence tách khỏi production; harvest → docs/feel/art/UI probe → design/build base → G-L mới → network gate sớm | Không attach network vào throwaway base; reuse phải review/test theo contract, không dựng framework để thay prototype |
| F07 — BASELINE | Yard ≥3 Dummy đồng thời, cùng pool/life/HP60/25s; tránh chờ có thể giải bằng placement trước đổi timer | A07: multiplayer contention vẫn OPEN; chưa đổi respawn farm hoặc thêm dummy identity |
| F08 — PROTOTYPE | Edge arrow/name + NPC marker P0; quest navigation xuyên map vẫn P1. Text-heavy functional prototype không final UI | P11/P15: manual inventory/equip/shop/quest/pending review; automation route không UX pass |
````

### Source fragment 3.22

````text
**Không tự chốt trong lượt này:** 26-frame interpretation/socket/front-back/camera/tile scale; LoS A/B/hybrid reduction/unreachable exact response; S2/S3 air permissions. Hành vi mới chưa có runtime evidence, không thay nhãn video/test cũ thành PASS mới. Những số power/MP/CD/gear/EXP/Boss/payout không liên quan được giữ nguyên.
````

### Source fragment 3.23

````text
<a id="research-ideas"></a>
<a id="legacy-provenance"></a>
````

### Source fragment 3.24

````text
# 6. Nguồn tham khảo và đề xuất P1
````

### Source fragment 3.25

````text
Các đề xuất dưới đây được giữ để không mất thiết kế đang cân nhắc. **P1 / PROPOSAL, chưa duyệt triển khai**, không cộng vào balance / acceptance P0. Chọn hoặc bỏ sau core gate theo [GDD scope](1_HUYEN_LO_GDD.md#vision); không phải Open Decision chặn code P0.
````

### Source fragment 3.26

````text
| ID | Candidate | Input proposal được giữ lại |
| --- | --- | --- |
| SCOPE-03 | **Chiến Ý — Kiếm / Ưng Nhãn Cường Hóa — Cung** | Buff Lv 10, phím R; duration 10 s, skill CD 40 s, MP 15. Chiến Ý: +15% ATK, +5% speed. Ưng Nhãn Cường Hóa: +10 điểm % CritChance, +15% range. Không phải nội tại Ưng Nhãn Lv 5; không thêm active P0. Acquisition / stacking và exact values cần chốt nếu chọn P1. |
| SCOPE-04 | Linh Giáp / Vỡ Thế | Shield 1.000 và Groggy là proposal cũ; chưa có shield-break / CC contract P0. Cần kiểm lại Boss / CC / workload nếu chọn; không tự lấy số này làm Boss data. |
````

### Source fragment 3.27

````text
**Legacy provenance compact:** NSO reference đã khai thác, không current authority. Trace từ `research/SRC NSOACE FIX/`: `Char.initMenu/finishTask` (NPC turn-in / bag checks), equip / use callbacks và `AbilityFromEquip` (onboarding / +4), `Mob.dead` (quest assist / loot input), Part / TileMap (modular / one-way). Availability instances là historical input; Boss hiện hành shared world. Không dùng reference chứng minh crash atomicity, rates hoặc balance. Lịch sử chi tiết nằm trong Git; pattern đã nhận là design Huyền Lộ.

````


<a id="single-behavior-history-4"></a>

## Historical source fragments — 4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md

Các đoạn dưới đây giữ nguyên văn trước đồng bộ luật được chủ dự án duyệt. Chỉ là lịch sử; không áp dụng làm luật hiện hành. Source: `docs/design/4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md`.

### Source fragment 4.1

````text
**Player khác mob:** death camera/corpse phải còn tới revive/về làng; không áp auto-fade/despawn normal lên player. Corpse Boss contributor ≥10% còn trong area vẫn hợp lệ theo GDD, không pickup tới khi sống. Phiên mất HP0 khôi phục corpse ở SafeAnchor; trạng thái corpse art không quyết định quest eligibility. PvPDefeated dùng kết thúc trận, không mở lựa chọn Hồi Sinh Phù/PvE death.
````

### Source fragment 4.2

````text
Ba skill entries/class dùng selected/locked/CD/MP cues, ghi rõ chỉ là default prototype UX; Tân Lữ entry 1 Mộc Kiếm, 2/3 khóa; 1/2/3 one-press bounded approach/cast, không J/RepeatOnHold; pending tiếp cận có cue “Đang tiếp cận”, reject nêu blocked/quá xa/chưa sẵn, manual/Esc hủy. Release không cancel pending; không hiện accepted cast animation trước validation/commit. Accepted action presentation bind snapshotted SkillId, không selection mới.
````


<a id="single-behavior-history-5"></a>

## Historical source fragments — 5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md

Các đoạn dưới đây giữ nguyên văn trước đồng bộ luật được chủ dự án duyệt. Chỉ là lịch sử; không áp dụng làm luật hiện hành. Source: `docs/design/5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md`.

### Source fragment 5.1

````text
**Evidence lịch sử 2026-10-03:** [VS-1 disposable/reference](../../prototypes/VS1_EndToEnd/README.md) tại checkpoint `archive/checkpoint-46006c4` (local-only archive, không có trên origin) có route/tests/build Linux revision V6.2.0. Feedback V6.2.1 đổi control/Q4–Q5/transition; evidence cũ giữ để trace, **không pass G-L revision mới hoặc production base**. G-L PARTIAL: chưa có normal-speed feel/usability review, rig/pose/pivot/socket, giờ art/QA/rework/% asset dùng được. Prototype code/evidence tách dưới `_Prototype`/`PrototypeEvidence`; production chưa bắt đầu.
````

### Source fragment 5.2

````text
**Prototype update 2026-10-04:** V6.2.4 có route/tests nhưng bị user từ chối về hình địa hình: Sói trong lòng nước, dòng nước đứt, background chưa đúng lớp đất. V6.2.5 bị user từ chối vì hiểu sai núi thành backdrop giả. V6.2.6 cũng bị user từ chối: slab đất lơ lửng, cầu sát nước và bờ chắn. V6.2.7 sửa rear soil/pass-through và cầu; kết quả kiểm ở [hồ sơ prototype](#prototype-visual-review), không tự pass G-L hoặc art production.
````

### Source fragment 5.3

````text
**Probe map/UI hiện tại:** thông số, sơ đồ và capture riêng của mock nằm ở [§10](#prototype-visual-review). Budget release, quest và các gate production không đổi. G-L vẫn PARTIAL; không dùng tests để kết luận layout/feel đã được người chơi duyệt.
````

### Source fragment 5.4

````text
Một đường đi revision mới: NPC → nhảy/drop-through/EdgeExit auto-transition → nhận/mặc Mộc Kiếm, hạ ba Dummy đồng thời → Nấm/nhặt supply/mặc Áo/bán sample → chuẩn bị Food/Bình Máu, đánh Sói → catch-up Lv5 → chọn Kiếm/cộng điểm/học bí kíp/S1 + dùng Bình MP → trả Q6. Quest objectives là hành động, tutorial glyph từ bindings. Cung giới thiệu nhưng chưa playable; Quần/Áo/Mộc/Kiếm phải đổi hình đọc được. Đồ II/III chỉ fixture hẹp, không route mới.
````


<a id="single-behavior-history-readme"></a>

## Historical source fragments — README.md

Các đoạn dưới đây giữ nguyên văn trước đồng bộ luật được chủ dự án duyệt. Chỉ là lịch sử; không áp dụng làm luật hiện hành. Source: `docs/design/README.md`.

### Source fragment readme.1

````text
Feedback V6.2.4 đã sync: ba skill tích lũy/class, keyboard-complete menus, 1–3 one-press bounded approach/cast, logical ranged và focus/interaction riêng; luật chi tiết ở owner. Prototype cũ không production codebase hoặc evidence pass cho revision mới; harvest/probe → production base → G-L mới → G-N sớm theo Roadmap. Chi tiết luật ở GDD, contract ở Technical, rationale/gates ở Analysis; review artifact tạm được dọn sau audit. User lock và review đã thống nhất được ghi vào GDD hiện hành. Nguồn tham khảo không tự đổi design.
````

### Source fragment readme.2

````text
| Tôi đang làm | Authority / evidence / contract |
| --- | --- |
| Character / attributes / EXP | [GDD §2](1_HUYEN_LO_GDD.md#character-power) + [Analysis §2](3_HUYEN_LO_DESIGN_ANALYSIS.md#character-evidence) |
| Skills / nội tại / bí kíp / status / cast | [GDD §3](1_HUYEN_LO_GDD.md#class-combat) + [Technical §3](2_HUYEN_LO_TECHNICAL.md#combat-data) |
| World / mob / Linh / Boss | [GDD §4](1_HUYEN_LO_GDD.md#world-farm) + [Technical §7](2_HUYEN_LO_TECHNICAL.md#timers) |
| Farm Lv 1 → 20 / density | [Analysis §3 matrix / model](3_HUYEN_LO_DESIGN_ANALYSIS.md#farm-progression) từ GDD §4 |
| Story / Q1–Q12 | [GDD §5](1_HUYEN_LO_GDD.md#quests-story) + [Technical §3](2_HUYEN_LO_TECHNICAL.md#combat-data) |
| Gear / cường hóa / chuyển giao / loot / economy | [GDD §6](1_HUYEN_LO_GDD.md#gear-economy) + [Analysis §4](3_HUYEN_LO_DESIGN_ANALYSIS.md#economy-analysis) |
| Food / Potion / death | [GDD §7](1_HUYEN_LO_GDD.md#consumables-death) + [Analysis gates](3_HUYEN_LO_DESIGN_ANALYSIS.md#open-decisions) |
| Online / PvP cược Vàng / chat | [GDD §8](1_HUYEN_LO_GDD.md#online-social) + [Technical §4–6](2_HUYEN_LO_TECHNICAL.md#network-authority) |
| Save / SafeAnchor / reconnect | [GDD §7–8](1_HUYEN_LO_GDD.md#consumables-death) + [Technical §5–6](2_HUYEN_LO_TECHNICAL.md#profile-authority) |
| UX / controls / art | [GDD §9](1_HUYEN_LO_GDD.md#ux-art) + [Technical §8–9](2_HUYEN_LO_TECHNICAL.md#art-contract) |
| Acceptance / demo / QA | [GDD §10](1_HUYEN_LO_GDD.md#acceptance-routing) + [Technical §12](2_HUYEN_LO_TECHNICAL.md#qa) |
| Quyết định và phần cần kiểm khi chơi thử | [Analysis §5](3_HUYEN_LO_DESIGN_ANALYSIS.md#open-decisions) |
````

### Source fragment readme.3

````text
Giữ năm file thiết kế active (1–5) và README làm điểm vào; tiền tố biểu thị thứ tự đọc. GDD là nguồn xác định version hiện hành; không tạo GDD versioned, audit, archive hay Open Questions riêng.
````

### Source fragment readme.4

````text
Trạng thái / version và DoD hiện hành xem GDD; bằng chứng nghiệm thu xem Technical.
````

### Source fragment readme.5

````text
| Cách gọi | Identifier | Nghĩa trong dự án |
| --- | --- | --- |
| Máu / Linh lực | HP / MP | Tài nguyên sống sót / dùng kỹ năng. |
| Tấn công / Phòng thủ | ATK / DEF | Chỉ số gây sát thương / giảm sát thương nhận. |
| Chính xác / Né tránh | ACC / EVA | Hai chỉ số dùng tính xác suất đánh trượt. |
| Đấu người / đánh quái | PvP / PvE | Người chơi đấu người chơi / chiến đấu với quái và thế giới. |
| Thời gian hạ mục tiêu | TTK | Từ lúc bắt đầu đánh tới khi mục tiêu chết. |
| Hiệu ứng hình ảnh / giao diện chơi | VFX / HUD | Hình chém, trạng thái… / thanh máu, kỹ năng, nhiệm vụ khi chơi. |
| Pixel trên một đơn vị Unity | PPU | Tỷ lệ nhập sprite để kích thước trong cảnh nhất quán. |
| Lời gọi qua mạng | RPC | Lệnh realtime giữa Client và Unity Game Server. |
| Đối tượng dữ liệu lưu/truyền | DTO | Bản dữ liệu không chứa tham chiếu Unity runtime. |
| Ưu tiên triển khai | P0 / P1 / P2 | P0 bắt buộc TARGET hiện tại; VS-1 là một phần thử trước. P1 sau core; P2 hoàn thiện thêm. |
| Mốc dùng để triển khai / số cần đo lại | BASELINE / TUNABLE | BASELINE là số hiện hành; TUNABLE chỉ đổi sau kiểm chứng. |
| Cụm quái | SpawnGroup | Bố trí một bãi quái do designer author; **không phải Party**. |
| Điểm sinh quái | SpawnSlot | Vị trí có ID cố định, tham chiếu mob identity; hồi sinh đúng loài / level đó. |
| Loại quái cố định | Mob identity | Tên, palette và level nhận diện nội dung; có thể dùng chung rig / AI. |
| Mức đóng góp sát thương | Contribution | HP thực lấy đi / MaxHP của life quái; không tính overkill. |
| Điều kiện nhận thưởng | Eligibility | Kiểm từng người: level, map, vị trí, sống / kết nối và tham gia; quest / Boss có predicate riêng. |
| Người gây sát thương cao nhất | TopDamage | Chọn từ ledger toàn life, tie theo characterId; dùng cho shared loot priority / suppression. |
| Mức đe dọa | Threat | Điểm quái dùng chọn mục tiêu; mỗi quái / mỗi player riêng, không phải contribution reward. |
| Bản chụp trạng thái | Snapshot | Giá trị được giữ tại cast, death hoặc lúc lập mutation bền vững. |
| Dấu xác nhận | Receipt | Bằng chứng event / transaction đã commit, chặn grant / claim / reward lặp khi retry / reconnect. |
| Backend / Game Server | Spring Boot / Unity Dedicated Server | Backend xác thực, escrow/settle Vàng và lưu tiến trình/checkpoint; Game Server chạy combat, world, PvP và quyết định kết quả realtime. |
| Cơ sở dữ liệu | PostgreSQL | Nguồn lưu tiến trình nhân vật và giao dịch bền vững P0. |
| Phiên / vé vào game | Session / game ticket | Phiên chơi hiện tại; vé dùng một lần, ngắn hạn, ràng tài khoản và nhân vật khi vào Game Server. |
| Điểm khôi phục | Recovery checkpoint / SafeAnchor | DB giữ MapId/HP/MP; phiên mất thì spawn tại một SafeAnchor của map farm/combat, khu an toàn có thể giữ tọa độ hợp lệ. |
| Tạm giữ cược / quyết toán | Escrow / settlement | Backend trừ cược của cả hai trước trận và trả thưởng/hoàn cược đúng một lần theo kết quả. |
| Trạng thái đang chạy | Runtime | State của phiên trên Game Server, phân biệt static definition và dữ liệu lưu trong PostgreSQL. |
| Cửa bảo vệ sau tan băng | Refractory | Deadline chung của target chặn Freeze mới; không kháng nhiều tầng. |
| Đồng hồ chờ hành động của Boss | ActionClockSpeed | Tốc độ đếm phần thời gian còn lại trước action tiếp theo; Băng Hàn giảm còn 75%, không đặt lại timer. |
| Bậc trang bị | Gear band | Ba chặng I / II / III; khác rarity và enhancement của từng instance. |
````
