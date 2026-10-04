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
