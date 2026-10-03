# Huyền Lộ — VS-1 local

Unity project thực, tạo bằng Editor **6000.5.9f1** đang cài. Mở thư mục `game/` bằng Unity Hub, mở [VS1.unity](Assets/HuyenLo/Scenes/VS1.unity) rồi Play. Bản Linux được build tại `Builds/Linux/HuyenLo.x86_64` (output không đưa vào Git).

Scope hiện có là **Q1–Q6, Tân Lữ → Kiếm, Vân Khê / Học Viện / Đồng Sương**. Phiên local giữ trạng thái trong RAM; đóng game mất tiến trình. Quyết định gameplay thuộc [GDD](../docs/design/1_HUYEN_LO_GDD.md), contract thuộc [Technical](../docs/design/2_HUYEN_LO_TECHNICAL.md), gate thuộc [Roadmap](../docs/design/5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#vs-1).

| Phím | Thao tác |
| --- | --- |
| A hoặc ← / D hoặc → | Đi trái / phải; hai binding không cộng đôi tốc độ |
| Space / S + Space | Nhảy / xuống sàn one-way |
| E | Tương tác ngay NPC/portal/đồ gần; loot không thay CombatFocus |
| J | Mộc Kiếm khi còn Tân Lữ; sau chọn Kiếm dùng slot đã chọn |
| 1 | Chọn S1 + thử cast ngay; giữ để lặp/tiếp cận ngang có giới hạn |
| 2 / 3 | Feedback khóa trong VS-1; không mở content Lv10/Lv17 |
| Click quái / Esc | Focus EXPLICIT / đóng modal, clear focus và held intent |
| F / H / M | Food / Bình Máu / Bình Linh Lực |
| B / C / K / L | Túi/trang bị / cộng điểm / bảng skill / quest |

Nhận và trả quest bằng nút tại đúng NPC. Q3 mở túi để mặc Mộc Kiếm, dùng ba life Bù Nhìn với respawn thật. Q4 mua đủ ba supply, dùng F rồi đi Đồng Sương. Q5 nhặt hai supply bằng E, mặc Áo, bán sample tại Bách Luyện. Q6 nhận tại Tạ Minh ở làng, chọn Kiếm tại ClassHall ở Học Viện, mặc Kiếm/cộng điểm/học sách, dùng S1 ở DummyYard rồi M và về trả Tạ Minh. Mộc An cho nghỉ/rương; hồi sinh miễn phí về làng hoặc tiêu phù tại chỗ. Trúc Ảnh chỉ mở trong quest state, portal báo ngoài phạm vi build.

Code chính:

- [SliceRules.cs](Assets/HuyenLo/Domain/SliceRules.cs): definitions CURRENT, stat/equip/EXP/inventory arithmetic và các profile fixture.
- [CombatController.cs](Assets/HuyenLo/Domain/CombatController.cs): pipeline J/slot chung, AUTO/EXPLICIT focus, press token/buffer, bounded assist, snapshot và logical resolve. Assembly domain không reference Unity.
- [SliceSession.cs](Assets/HuyenLo/Domain/SliceSession.cs): local authority, mobs/clock/lifetimes, inventory/shop/claim/receipts, staged quest/retry, Food/Bình/death. UI gọi command, không sở hữu mutation.
- [SliceHost.cs](Assets/HuyenLo/Runtime/SliceHost.cs): Input System, Rigidbody2D player/world collision, per-actor drop-through, camera/maps và presentation.
- [SliceHud.cs](Assets/HuyenLo/Runtime/SliceHud.cs): HUD/modal và dispatch command. Target name/level/HP cùng ID/generation; AnimationEvent/VFX không có API gây hit.
- [SliceBuild.cs](Assets/HuyenLo/Editor/SliceBuild.cs): tạo scene qua Editor API và build Linux; menu `Huyền Lộ`.

Các điểm **probe**, chưa thành luật production: layout/toạ độ blockout, speed 5 u/s/jump velocity 12/gravity scale 2, Dummy DEF/EVA=0, vertical combat bound, input buffer 150 ms, grace unreachable 2 s → Return untargetable + reset tại home. Giữ roster hybrid/LoS/art decisions mở ở canonical docs; VS-1 chưa có các loài hybrid hoặc Cung. S2/S3/Bow definitions chỉ dùng fixture để kiểm action/shape/batch, không unlock qua route; status của các skill muộn chưa triển khai. Local receipt chỉ chứng minh idempotency trong RAM, không chứng minh crash atomicity, UTC recovery hay multiplayer correctness.

Art là các primitive đọc được trạng thái, thay màu Quần/Áo và vũ khí Mộc/Kiếm, hit flash/text và pose theo action clock. Chưa có rig raster 26 frame/socket production, Common UI Kit production hoặc tỷ lệ asset Free dùng được. Font Việt [DejaVu Sans](Assets/HuyenLo/Resources/Fonts/DejaVuSans.ttf) được import và include trong build; [license](Assets/HuyenLo/Resources/Fonts/LICENSE.txt) đi kèm.

## Chạy kiểm tra

Từ root repo, đặt biến đường dẫn Editor thực:

```sh
HUYEN_LO_EDITOR=/home/nguyenvanrin/Unity/Hub/Editor/6000.5.9f1/Editor/Unity
"$HUYEN_LO_EDITOR" -batchmode -nographics -projectPath "$PWD/game" -runTests -testPlatform EditMode -testResults /tmp/huyenlo-edit.xml -logFile /tmp/huyenlo-edit.log
"$HUYEN_LO_EDITOR" -batchmode -nographics -projectPath "$PWD/game" -runTests -testPlatform PlayMode -testResults /tmp/huyenlo-play.xml -logFile /tmp/huyenlo-play.log
"$HUYEN_LO_EDITOR" -batchmode -nographics -quit -projectPath "$PWD/game" -executeMethod HuyenLo.Editor.SliceBuild.Linux -logFile /tmp/huyenlo-build.log
python3 game/Validation/verify_documents.py
```

Development build có acceptance driver riêng (không khởi động trong chế độ chơi thường):

```sh
game/Builds/Linux/HuyenLo.x86_64 -screen-fullscreen 0 -screen-width 1280 -screen-height 720 -logFile /tmp/huyenlo-player.log --verify-route --capture-route --evidence-path /tmp/huyenlo-route
```

[SliceRouteProbe.cs](Assets/HuyenLo/Runtime/SliceRouteProbe.cs) gửi movement/interaction/domain command qua cùng boundary với UI. Route không set quest/EXP/HP/vị trí để skip; đi bằng Rigidbody2D, nhảy/drop thật và portal thật. Tốc độ thời gian ×4 phục vụ kiểm tự động; đây không phải video playtest game feel ở tốc độ thường. Các test fixture riêng có arrange state rõ, không được dùng thay hành trình này. Input fixture dùng keyboard events của Input System, bật IgnoreFocus **chỉ trong test batch** rồi khôi phục settings.

Kết quả và giới hạn gate: [validation.json](Validation/validation.json), [audit tài liệu](Validation/document-audit.json), [log route](Validation/continuous-route.log), [video route](Validation/continuous-route.mp4). **G-L PARTIAL**: route/runtime có evidence, còn cần rig/pose/pivot/socket probe, giờ art/QA/rework và % asset dùng được, cùng review thao tác ở tốc độ thường. Dừng ở G-L; chưa mở Dedicated/backend/content sau slice.
