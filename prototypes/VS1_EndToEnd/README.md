# Huyền Lộ — bản chạy thử tối giản

Đây là disposable prototype, sẽ thiết kế lại production từ đầu. Luật đọc [GDD](../../docs/01-design/game-design.md), contract đọc [Technical](../../docs/02-technical/architecture.md), phạm vi đọc [Roadmap](../../docs/04-production/roadmap.md#vs-1).

## Chạy

Unity Hub **6000.5.9f1**, mở project này và [VS1.unity](Assets/_Prototype/VS1_EndToEnd/Scenes/VS1.unity). Linux build:

```sh
./prototypes/VS1_EndToEnd/Builds/Prototype/VS1_EndToEnd/HuyenLo.x86_64
```

Một phiên RAM mới bắt đầu Q1; không save JSON, không resume, F8 mở menu debug chọn giai đoạn/reset; không cấu hình A/B. Preset debug dùng helpers tạo state riêng; không dùng để bỏ qua route nghiệm thu. Fresh reset về Q1, không save JSON. Khi mở debug, phiên local tạm dừng để chọn giai đoạn an toàn; Esc đóng và tiếp tục.

| Phím | Thao tác |
| --- | --- |
| A/←, D/→ | Di chuyển; aliases không cộng tốc |
| Space/↑, S/↓ | Jump / drop trên one-way |
| 1/2/3 | Một press: chọn skill → bounded approach nếu cần → cast một lần; Novice chỉ 1 |
| Tab / Shift+Tab ở world | Chọn mục tiêu kế/trước trong search envelope rộng hơn tầm đánh |
| E | Nhặt hoặc NPC; chỉ manual movement kích hoạt EdgeExit |
| I / C / Q | Hành trang / Nhân vật / Quest |
| Mũi tên/WASD, Tab/Shift+Tab ở menu, Enter/E | Chọn ô/action/tab và xác nhận bằng bàn phím |
| 4/H, 5/M, F | Bình Máu / Bình Linh lực / Food; 4/5 không chọn skill |
| F8 hoặc nút Debug | Chọn Fresh/Q2/Q3/Q4/Q5/Q6/Kiếm/đàn Sói và reset phiên RAM |
| Enter ở world | Mở ô chat local để kiểm UI capture; chưa gửi Map Chat online |
| Esc | Modal/chat → pending/buffer → focus → no-op; mỗi press xử một tầng |
| Click | Focus, NPC/loot hoặc menu; cùng command/validation với bàn phím |

Luật giữ phím/arrival/buffer/focus ở [GDD §3](../../docs/01-design/combat-and-character.md#pending-cast). UI nhận/trả quest, nhập phái/nghỉ thành công đóng hội thoại; shop/kho giữ mở. Nhập phái phải tự tháo Mộc Kiếm, có Phong Du và Diệp Lam riêng; nhánh Cung chưa playable.

## Phạm vi

Q1–Q6: Vân Khê ↔ Học Viện, Vân Khê ↔ Đồng Sương; giữ layout/collision/spawn/exits hiện có. Player dùng lại rig hình học từ commit `50f05ed`, cache các bộ phận; NPC có đầu/tóc/áo, Sói có thân/mõm/tai/chân, Dummy có cọc và tay. Chân đặt theo mặt đỡ. Đất liền bên dưới, cỏ xanh và đá lát nông phân biệt bề mặt; không cột đá/đất xen kẽ toàn chiều cao. Vũng Vân Khê là hình chữ nhật dài 6 u, đáy -0,8 và đường đất one-way phía trên 1,2; S để xuống lội, nhảy lên đi khô. Nước dưới cầu được nâng mặt hình ảnh lên 0,75, không đổi collider hoặc slow của đường cầu. Không vẽ viền đứng dưới nước. Không mở rộng roster.

Giữ marker NPC, đối thoại, HP/MP, target HUD/mini HP, skill text/CD, quest tracker, bag grid/equipment slots. S1/S2/S3 Kiếm giữ số; basic Tân Lữ dùng nhịp được duyệt. Code pending/selection/geometry lấy range từ profile; ranged chỉ fixture, không gameplay Cung. Dedicated/backend/PvP/Boss chưa làm; chat chỉ ô nhập local có nhãn.

## Kiểm chức năng

EditMode kiểm domain/cost/arrival/life/clock; PlayMode kiểm Input System, physics/UI và fresh route. Build có `--verify-route` để đi Q1–Q6 bằng Rigidbody2D và menu commands, không preset/inject quest/EXP/HP/position. Driver chỉ log PASS/FAIL, không capture/showcase/evidence directory hoặc đo metric. Automation không thay review feel/usability của người thật.

Lịch sử và giới hạn kết quả trước ở [CHANGELOG](CHANGELOG.md). Font [DejaVu Sans](Assets/_Prototype/VS1_EndToEnd/Resources/Fonts/DejaVuSans.ttf) giữ [license](Assets/_Prototype/VS1_EndToEnd/Resources/Fonts/LICENSE.txt). Production base chưa bắt đầu; G-L production/G-N/G-D chưa nghiệm thu.

Focus prototype: click/Tab tìm trong ±12 u ngang / ±6 u dọc; giữ focus đến ±20 u ngang / ±10 u dọc hoặc lifecycle invalid. Nhảy/hủy pending không clear focus. Attack/assist vẫn theo skill profile, không cast xa theo HUD. Các số này là tuning disposable. Hướng dẫn phím ở một panel bên trái; context menu vẫn có chỉ dẫn thao tác riêng.
