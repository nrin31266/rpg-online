# Huyền Lộ — game mẫu V6.2.2

Đây là **disposable/reference prototype**, không production base. Unity project mẫu nằm riêng tại `prototypes/VS1_EndToEnd/`; tương lai `game/` dùng cho Unity Client/Dedicated và `backend/` cho Spring/PostgreSQL, chưa dựng. Gameplay authority là [GDD hiện hành](../../docs/design/1_HUYEN_LO_GDD.md), integration contract ở [Technical](../../docs/design/2_HUYEN_LO_TECHNICAL.md), thứ tự ở [Roadmap](../../docs/design/5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md).

## Chạy và thử nhanh

Mở folder này bằng Unity Hub **6000.5.9f1**, scene [VS1.unity](Assets/_Prototype/VS1_EndToEnd/Scenes/VS1.unity), Play. Linux development build:

```sh
./prototypes/VS1_EndToEnd/Builds/Prototype/VS1_EndToEnd/HuyenLo.x86_64
```

**F8 hoặc nút Debug** mở menu mốc Q1–Q6/sân tập Kiếm Lv5/reset. Chọn mốc, rồi xác nhận reset; menu mặc định chọn Hủy để tránh mất phiên. Chọn Bắt đầu mới xóa toàn bộ RAM và về Q1. Menu debug tạm dừng local simulation; các panel gameplay khác không tạo miễn sát thương. Không save JSON hoặc auto-resume; dùng mốc để thử nhanh. Preset có nhãn DEBUG, không là evidence hành trình.

| Phím | Thao tác |
| --- | --- |
| A/←, D/→ | Đi, aliases không cộng tốc |
| Space/↑ | Jump; thả sớm nhảy thấp hơn |
| S/↓ | Drop trên one-way; không cần Space |
| 1/2/3 | Chọn skill + yêu cầu đúng một lần; Novice dùng 1 Mộc Kiếm, slot chưa mở báo lý do |
| E | Nhặt đồ hoặc mở NPC; không dùng cho MapExit |
| ↑/↓ hoặc W/S, Tab/Shift+Tab trong menu | Đổi action được highlight |
| Enter/E trong menu | Nhận/trả quest, equip/learn, mua/bán, cộng điểm, rương, revive; không cần chuột |
| I / C / Q | Túi / Nhân vật với skill tab / Quest |
| F / H / M | Food / Bình Máu / Bình Linh lực |
| Esc | Đóng modal, hủy intent/clear focus trong world |
| Click | Focus quái hoặc mở NPC/nhặt loot trong range; các menu hỗ trợ mouse cùng command |

**Không J, không giữ để repeat.** Một press acquire/focus; hơi xa thì tiếp cận ngang có giới hạn rồi cast một lần. Release giữ pending; manual movement/jump/drop/focus/UI/Esc/map hủy. No target/blocked/quá xa/MP/CD chưa sẵn không commit cost. Normal damage không interrupt hoặc knockback. Chạm exit có arrow/tên vùng tự chuyển map; exit đích đặt ngoài trigger ngược.

## Route và probe

Q1 NPC → Q2 movement/ledge/drop/EdgeExit → Q3 ba Dummy đồng thời (Lv3) → Q4 Nấm DS2/loot/equip/sell (Lv4) → Q5 Food/HP + năm Sói (Lv5) → Q6 chọn Kiếm/equip/điểm/bí kíp/S1/MP. NPC mở với quest action ưu tiên; dùng Enter/E xác nhận và Esc quay lại world. Học/use/equip trong I, cộng điểm/skill tab trong C. Q4 tutorial supply chỉ tạo đúng active kill step; ngoài bước đó không future entitlement. Khi full bag, claim/grant giữ instance để retry.

Coyote `.10s`, jump buffer `.12s`, release cutoff, ground/air acceleration, fall cap và melee phase/separation/recovery movement là **giá trị probe**, chưa lock production. Geometry/cast vẫn authority clock; variable jump không cấp double jump. Class không có permanent Normal. S2/S3/Cung chỉ fixture, không content route; backend/network/chat/PvP/Boss chưa làm. Art vẫn primitive, chưa full rig26/socket hoặc Common UI Kit production; tests không thay manual feel/usability review.

## Kiểm chứng

Từ root repo:

```sh
HUYEN_LO_EDITOR=/home/nguyenvanrin/Unity/Hub/Editor/6000.5.9f1/Editor/Unity
"$HUYEN_LO_EDITOR" -batchmode -nographics -projectPath "$PWD/prototypes/VS1_EndToEnd" -runTests -testPlatform EditMode -testResults /tmp/huyenlo-edit.xml -logFile /tmp/huyenlo-edit.log
"$HUYEN_LO_EDITOR" -batchmode -nographics -projectPath "$PWD/prototypes/VS1_EndToEnd" -runTests -testPlatform PlayMode -testResults /tmp/huyenlo-play.xml -logFile /tmp/huyenlo-play.log
"$HUYEN_LO_EDITOR" -batchmode -nographics -quit -projectPath "$PWD/prototypes/VS1_EndToEnd" -executeMethod HuyenLo.Editor.SliceBuild.Linux -logFile /tmp/huyenlo-build.log
python3 prototypes/VS1_EndToEnd/PrototypeEvidence/V6_2_2/verify_revision.py
```

[Evidence revision mới](PrototypeEvidence/V6_2_2/validation.json) và [audit docs](PrototypeEvidence/V6_2_2/document-audit.json). Acceptance driver `--verify-route --evidence-path /tmp/huyenlo-route` đi fresh Q1–Q6 bằng Rigidbody2D, auto exits và keyboard menu adapter; không dùng debug preset hoặc set quest/EXP/HP/position để skip. TimeScale4 chỉ cho automation; đây không manual playtest. Real Input System tests kiểm NPC/shop/inventory/debug và air cast; movement fixture kiểm tap/hold/coyote/no-double/drop.

Evidence [V6.2.0 và migration cũ](PrototypeEvidence/VS1_EndToEnd/validation.json) giữ nguyên byte và đường dẫn lịch sử. Scripts/audit cũ thuộc checkpoint trước, không chạy lên revision mới để ghi đè. Folder Assets/_Prototype vẫn giữ meta/GUID qua relocation cả project. Font [DejaVu Sans](Assets/_Prototype/VS1_EndToEnd/Resources/Fonts/DejaVuSans.ttf) có [license](Assets/_Prototype/VS1_EndToEnd/Resources/Fonts/LICENSE.txt).

**G-L PARTIAL.** Probe mới hỗ trợ chơi thử sửa docs; production base chưa bắt đầu, G-N/G-D chưa mở. Rig/art/normal-speed UX và quyền art OPEN theo canonical owners.
