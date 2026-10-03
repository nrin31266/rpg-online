# Huyền Lộ Design Documentation

## Đọc trước

1. [GDD](1_HUYEN_LO_GDD.md): game, luật, scope và DoD hiện hành.
2. [Technical](2_HUYEN_LO_TECHNICAL.md): hợp đồng triển khai, local → Dedicated và QA.
3. [Design Analysis](3_HUYEN_LO_DESIGN_ANALYSIS.md): balance, evidence và quyết định mở; tra theo feature đang code.
4. [Art / Visual / Production](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md): tài liệu sống cho pose/weapon/map/UI/import/accounting và validation.
5. [Implementation Roadmap](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md): CURRENT/DEFERRED, VS-1, thứ tự/gates và mục tiêu quản lý hai tháng/nhóm bốn người.

## Authority

GDD = WHAT/gameplay authority; Technical = HOW/implementation; Analysis = WHY/evidence/quyết định; Art = detail ART/VISUAL/PRODUCTION; Roadmap = WHEN/CURRENT/DEFERRED.

TARGET P0 giữ Kiếm + Cung và Dedicated + Spring/PostgreSQL online. CURRENT là harvest disposable/reference VS-1, docs/feel/art/UI probe và review production base; local slice revision mới vẫn Kiếm Q1–Q6 ở Vân Khê/Học Viện/Đồng Sương; Cung/online đầy đủ DEFER theo [Roadmap](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#target-current-deferred), không bị bỏ hoặc đổi thành P1. Gate Dedicated + hai clients diễn ra trước production rộng; local pass không thay final acceptance.

Nếu Analysis / Technical mâu thuẫn GDD, GDD thắng. Deterministic defect đã xác minh phải sửa đồng bộ; proposal không tự đổi luật.

Feedback V6.2.3 đã sync: ba skill tích lũy/class, keyboard-complete menus, 1–3 one-press bounded approach/cast, logical ranged và focus/interaction riêng; luật chi tiết ở owner. Prototype cũ không production codebase hoặc evidence pass cho revision mới; harvest/probe → production base → G-L mới → G-N sớm theo Roadmap. Chi tiết luật ở GDD, contract ở Technical, rationale/gates ở Analysis; review artifact tạm được dọn sau audit. User lock và review đã thống nhất được ghi vào GDD hiện hành. Nguồn tham khảo không tự đổi design.

## Quick Routing

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

## Quy ước cập nhật

Giữ năm file thiết kế active (1–5) và README làm điểm vào; tiền tố biểu thị thứ tự đọc. GDD là nguồn xác định version hiện hành; không tạo GDD versioned, audit, archive hay Open Questions riêng.

Trạng thái / version và DoD hiện hành xem GDD; bằng chứng nghiệm thu xem Technical.

BASELINE là số đang dùng để triển khai; TUNABLE chỉ chỉnh sau khi đo; P1 / P2 không thành yêu cầu P0. Trạng thái quyết định chỉ giữ ở Analysis §5: bảng gate sẵn có và alias A01–A17 gắn vào gate đó; Art giữ reasoning/ma trận thử, Roadmap giữ thứ tự, không mở sổ quyết định cạnh tranh.

Khi gộp nội dung, giữ mỗi luật tại một domain authority và chuyển evidence/proposal sang đúng nơi; không xóa chi tiết chỉ để giảm độ dài. P1 chưa duyệt vẫn được giữ rõ trạng thái tại [Analysis — candidates](3_HUYEN_LO_DESIGN_ANALYSIS.md#research-ideas).

Bảng số ở Analysis phục vụ tra cứu combat, density và economy; không nhúng JSON/chart dài. Persistence P0 là Spring Boot + PostgreSQL theo Technical §5–6; JSON chỉ dùng config/fixture/import-export dev. Objectives/reward/unlock Q1–Q12 nằm trong **một bảng** GDD §5, recovery đặc biệt ngay sau bảng; luật bí kíp và nội tại tập trung ở GDD §3.

Lịch sử thay đổi nằm trong Git.

Tra cứu chi tiết hơn qua Routing ở cuối GDD.

## Thuật ngữ dùng chung

GDD ưu tiên tiếng Việt; Technical giữ identifier tiếng Anh khi cần code/API. Đây là bảng tra từ thường dùng, không thêm hệ thống gameplay.

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

Art đọc [GDD §9](1_HUYEN_LO_GDD.md#ux-art) → [Art review/detail](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#art-review) → [Technical §8](2_HUYEN_LO_TECHNICAL.md#art-contract); layout/anchors đọc GDD §4. Thứ tự làm và [SOURCE → DESTINATION/audit](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#source-destination) ở Roadmap. Kỷ luật coding agent ở [Technical §1.1](2_HUYEN_LO_TECHNICAL.md#architecture-discipline). Co-farm không cần Party; world support N players, **P0 acceptance: tối thiểu 2 concurrent players** kết nối Dedicated Game Server, chưa công bố capacity.
