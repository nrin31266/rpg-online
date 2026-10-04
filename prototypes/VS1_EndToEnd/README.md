# Huyền Lộ — game mẫu V6.2.6

Đây là **disposable/reference prototype**, không production base. Unity project mẫu nằm riêng tại `prototypes/VS1_EndToEnd/`; tương lai `game/` dùng cho Unity Client/Dedicated và `backend/` cho Spring/PostgreSQL, chưa dựng. Gameplay authority là [GDD hiện hành](../../docs/design/1_HUYEN_LO_GDD.md), integration contract ở [Technical](../../docs/design/2_HUYEN_LO_TECHNICAL.md), thứ tự ở [Roadmap](../../docs/design/5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md).

## Chạy và thử nhanh

Mở folder này bằng Unity Hub **6000.5.9f1**, scene [VS1.unity](Assets/_Prototype/VS1_EndToEnd/Scenes/VS1.unity), Play. Linux development build:

```sh
./prototypes/VS1_EndToEnd/Builds/Prototype/VS1_EndToEnd/HuyenLo.x86_64
```

**F8 hoặc nút DEV nhỏ** mở menu mốc Q1–Q6/sân tập Kiếm Lv5/bãi 4 Sói/reset. Chọn mốc, rồi xác nhận reset; menu mặc định chọn Hủy để tránh mất phiên. Chọn Bắt đầu mới xóa toàn bộ RAM và về Q1. Menu debug tạm dừng local simulation; các panel gameplay khác không tạo miễn sát thương. Không save JSON hoặc auto-resume; dùng mốc để thử nhanh. Preset có nhãn DEBUG, không là evidence hành trình.

| Phím | Thao tác |
| --- | --- |
| A/←, D/→ | Đi, aliases không cộng tốc |
| Space/↑ | Jump; thả sớm nhảy thấp hơn |
| S/↓ | Drop trên one-way; không cần Space |
| 1/2/3 | Chọn skill + yêu cầu đúng một lần; Novice dùng 1 Mộc Kiếm, slot chưa mở báo lý do |
| E | Nhặt đồ hoặc mở NPC; không dùng cho MapExit |
| ↑↓←→ / WASD trong menu | Chọn action/ô; Tab/Shift+Tab đổi RPG tab |
| Enter/E trong menu | Nhận/trả quest, equip/learn, mua/bán, cộng điểm, rương, revive; không cần chuột |
| I / C / Q | Hành trang / Trang bị / Quest |
| F / H / M | Food / Bình Máu / Bình Linh lực |
| Esc | Đóng modal, hủy intent/clear focus trong world |
| Click | Focus quái hoặc mở NPC/nhặt loot trong range; các menu hỗ trợ mouse cùng command |

**Không J, không giữ để repeat.** Một press acquire/focus; hơi xa thì tiếp cận ngang có giới hạn rồi cast một lần. Release giữ pending; manual movement/jump/drop/focus/UI/Esc/map hủy. No target/blocked/quá xa/MP/CD chưa sẵn không commit cost. Normal damage không interrupt hoặc knockback. Chạm exit có arrow/tên vùng tự chuyển map; exit đích đặt ngoài trigger ngược.

## Map mới

Học Viện nằm tây Vân Khê, Đồng Sương phía đông; reciprocal exits giữ nguyên. “Núi” là khối đất solid cao, có bậc nhảy và mặt đứng thật; bỏ các lớp núi giả trong V6.2.5. Đài Tạ Minh ở đồi3,6 u; Cung đường/Diệp Lam ở sống đất6,4 u, lên qua bậc phía đông. Mist có DS3/DS4 hill, mỏm DS5 có Sói trên và đường đi dưới; PROBE8 nằm trên mỏm cao phía đông. Home/lane của từng đàn theo đúng mặt nền, không AI tự nhảy giữa tầng. DS1–DS6 giữ10 slots + PROBE7/4 + PROBE8/3 riêng mock, tổng17; PROBE không credit Q5.

Suối rộng là hình trang trí sát cầu solid: không thể dùng S rơi xuống, không đi đáy/nước rộng slow. Vũng nhỏ Vân Khê vẫn đi qua và giảm tốc nhẹ ×0,85. Đất/cliff không drop-through; chỉ gác gỗ one-way có S/↓. Các kích thước/layout thuộc [Roadmap §10](../../docs/design/5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#prototype-visual-review), không là manifest production.

F9 bật/tắt trace dev live: target → desired position → occupancy → velocity → range/windup/bite count. Không đổi range hoặc cho bite xa để cứu đàn. V6.2.3 dùng rank + peer clipping làm rear wolf kẹt; V6.2.4 recovery/cross + bounded neighbor correction giữ tất cả có cơ hội re-engage. Crossings ngắn là hợp lệ, không body-block/formation/token.

## Route và probe

Q1 NPC → Q2 movement/ledge/drop/EdgeExit → Q3 ba Dummy đồng thời (Lv3) → Q4 Nấm DS2/loot/equip/sell (Lv4) → Q5 Food/HP + năm Sói (Lv5) → Q6 tháo Mộc Kiếm trước/chọn Kiếm tại Phong Du/equip/điểm/bí kíp/S1/MP. Diệp Lam là NPC Cung riêng, nhánh Cung chưa mở trong mock. NPC mở với quest action ưu tiên; dùng Enter/E xác nhận và Esc quay lại world. NPC có hội thoại + !/?; chọn Mua/Bán/Gửi/Lấy mới mở submenu đó. I là bag grid30 ô, detail bên phải, Enter mở thao tác món. C mở thẳng Trang bị: hình người/sáu slot → chọn slot → Tháo. Esc quay một cấp, rồi đóng root; I/C/Q đổi root. Học/use/equip trong I; Tab đổi Trang bị / Thuộc tính / Thông số / Kỹ năng, không phải đóng root. Chọn slot (kể cả trống) mở hành trang lọc đúng loại. Q4 tutorial supply chỉ tạo đúng active kill step; ngoài bước đó không future entitlement. Khi full bag, claim/grant giữ instance để retry.

Coyote `.10s`, jump buffer `.12s`, release cutoff, ground/air acceleration, fall cap và melee phase/separation/recovery movement là **giá trị probe**, chưa lock production. Geometry/cast vẫn authority clock; variable jump không cấp double jump. Class không có permanent Normal. S2/S3/Cung chỉ fixture, không content route; backend/network/chat/PvP/Boss chưa làm. Art vẫn primitive; icon bag đã có motif khối/count và paper doll tạm, geometric rig dùng chung BodyBase/Hair/Armor/LowerBody/Weapon và grip/pose description cho world/preview, chưa production rig26/socket hoặc Common UI Kit production; tests không thay manual feel/usability review.

## Kiểm chứng

Từ root repo:

```sh
HUYEN_LO_EDITOR=/home/nguyenvanrin/Unity/Hub/Editor/6000.5.9f1/Editor/Unity
"$HUYEN_LO_EDITOR" -batchmode -nographics -projectPath "$PWD/prototypes/VS1_EndToEnd" -runTests -testPlatform EditMode -testResults /tmp/huyenlo-edit.xml -logFile /tmp/huyenlo-edit.log
"$HUYEN_LO_EDITOR" -batchmode -nographics -projectPath "$PWD/prototypes/VS1_EndToEnd" -runTests -testPlatform PlayMode -testResults /tmp/huyenlo-play.xml -logFile /tmp/huyenlo-play.log
"$HUYEN_LO_EDITOR" -batchmode -nographics -quit -projectPath "$PWD/prototypes/VS1_EndToEnd" -executeMethod HuyenLo.Editor.SliceBuild.Linux -logFile /tmp/huyenlo-build.log
python3 prototypes/VS1_EndToEnd/PrototypeEvidence/V6_2_6/verify_revision.py
```

[Evidence revision mới](PrototypeEvidence/V6_2_6/validation.json) và [audit docs](PrototypeEvidence/V6_2_6/document-audit.json). V6.2.4/V6.2.5 pass tests/route nhưng user từ chối hình địa hình; không là visual acceptance. Các thông số, hình và lỗi riêng mock nằm trong [Roadmap §10](../../docs/design/5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#prototype-visual-review), không chen vào production docs 1–4. Acceptance driver `--verify-route --evidence-path /tmp/huyenlo-route` đi fresh Q1–Q6 bằng Rigidbody2D, auto exits và keyboard menu adapter; không dùng debug preset hoặc set quest/EXP/HP/position để skip. TimeScale4 chỉ cho automation; đây không manual playtest. Real Input System tests kiểm NPC/shop/inventory/debug và air cast; movement fixture kiểm tap/hold/coyote/no-double/drop.

Evidence [V6.2.0 và migration cũ](PrototypeEvidence/VS1_EndToEnd/validation.json) giữ nguyên byte và đường dẫn lịch sử. Scripts/audit cũ thuộc checkpoint trước, không chạy lên revision mới để ghi đè. Folder Assets/_Prototype vẫn giữ meta/GUID qua relocation cả project. Font [DejaVu Sans](Assets/_Prototype/VS1_EndToEnd/Resources/Fonts/DejaVuSans.ttf) có [license](Assets/_Prototype/VS1_EndToEnd/Resources/Fonts/LICENSE.txt).

Raw test XML/log, video/capture dư và render HTML/cache để ngoài Git. Chỉ giữ kết quả có source/build hash, audit, hình sơ đồ được docs link và ghi chú kiểm 1×. Historical evidence giữ nguyên vì audit/checkpoint cũ vẫn dùng; không thêm raw dump mới.

**G-L PARTIAL.** Probe mới hỗ trợ chơi thử sửa docs; production base chưa bắt đầu, G-N/G-D chưa mở. Rig/art/normal-speed UX và quyền art OPEN theo canonical owners.

V6.2.4 feedback nối tiếp: solid basin/terraces + cầu qua nước rộng; nước nông đi được ở đáy và thử giảm tốc 15% khi chân chạm nước (cầu/nhảy ra nước không slow). Đàn quái có đoạn hoạt động chung, return rồi patrol thay vì đứng mãi ở mép. NPC nhận/trả quest/nhập phái/nghỉ thành công đóng menu; shop/kho giữ mở, Esc lùi một tầng. Mouse chuyển slot/tab kết thúc IMGUI event để không vẽ tiếp danh sách cũ. Tọa độ/tuning vẫn là mock probe.
