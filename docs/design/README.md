# Huyền Lộ Design Documentation

## Tóm tắt

Điểm vào năm tài liệu canonical: GDD giữ luật game, Technical giữ hợp đồng triển khai, Analysis giữ phân tích và quyết định, Art giữ hình ảnh/quy trình sản xuất, Roadmap giữ thứ tự và trạng thái. GDD thắng khi có mâu thuẫn. README chỉ dẫn tới đúng owner và giải thích thuật ngữ, không tạo luật thứ sáu.

**Ngày đồng bộ:** 2026-10-06 · **Trạng thái:** DESIGN + PROTOTYPE VALIDATION; production chưa bắt đầu. Lượt migration này chỉ cập nhật tài liệu; VS-1 và source giữ nguyên.

## Tìm gì ở đâu

- [Thứ tự đọc](#đọc-trước), [authority](#authority), [Quick Routing](#quick-routing).
- [Quy ước cập nhật](#quy-ước-cập-nhật), [thuật ngữ](#thuật-ngữ-dùng-chung).

## Đọc trước

1. [GDD](1_HUYEN_LO_GDD.md): game, luật, scope và DoD hiện hành.
2. [Technical](2_HUYEN_LO_TECHNICAL.md): hợp đồng triển khai, local → Dedicated và QA.
3. [Design Analysis](3_HUYEN_LO_DESIGN_ANALYSIS.md): balance, evidence và quyết định mở; tra theo feature đang code.
4. [Art / Visual / Production](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md): tài liệu sống cho pose/weapon/map/UI/import/accounting và validation.
5. [Implementation Roadmap](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md): CURRENT/DEFERRED, VS-1, thứ tự/gates và mục tiêu quản lý hai tháng/nhóm bốn người.

## Authority

GDD = WHAT/luật game; Technical = HOW/cách triển khai; Analysis = WHY/phân tích, bằng chứng và sổ quyết định; Art = VISUAL/PRODUCTION; Roadmap = WHEN/CURRENT/DEFERRED.

TARGET P0 giữ Kiếm + Cung, đầy đủ Q1–Q12 và online Dedicated + Spring/PostgreSQL. CURRENT là đồng bộ docs từ bài học VS-1, sau đó probe controls/UI/art/rig → review production base → local production slice Kiếm Q1–Q6 ở Vân Khê/Học Viện/Đồng Sương → gate Dedicated sớm với ≥2 client → mở rộng. Cung và online đầy đủ vẫn là P0 theo [Roadmap](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#target-current-deferred). World/hệ shared phải an toàn với N người; hai client là mức nghiệm thu tối thiểu, không phải player cap. Local-first và local pass không thay nghiệm thu online cuối.

Nếu Analysis / Technical mâu thuẫn GDD, GDD thắng. Deterministic defect đã xác minh phải sửa đồng bộ; proposal không tự đổi luật.

Luật controls/focus/pending/dùng đồ nhanh đọc GDD; contract đọc Technical. Thiết kế hiện hành tách chọn skill khỏi ExecuteSelected. VS-1 là reference disposable theo input/layout cũ; test/video chỉ chứng minh revision tương ứng. Các gate mới chưa có bằng chứng pass. Probe art mới dùng sandbox disposable riêng theo Art; không sửa prototype hiện có trong lượt migration này.

## Quick Routing

| Tôi đang làm | Authority / evidence / contract |
| --- | --- |
| Character / attributes / EXP | [GDD §2](1_HUYEN_LO_GDD.md#character-power) + [Analysis §2](3_HUYEN_LO_DESIGN_ANALYSIS.md#character-evidence) |
| Skills / nội tại / bí kíp / status / cadence | [GDD §3](1_HUYEN_LO_GDD.md#class-combat), [Analysis baseline mới](3_HUYEN_LO_DESIGN_ANALYSIS.md#current-balance-probe) + [Technical §3](2_HUYEN_LO_TECHNICAL.md#combat-data) |
| 1/2/3 chọn skill / ExecuteSelected / bindings | [GDD focus/input](1_HUYEN_LO_GDD.md#focus-input), [GDD §9 UX/phím đề xuất](1_HUYEN_LO_GDD.md#ux-art) + [Technical input](2_HUYEN_LO_TECHNICAL.md#input-contract) |
| Đổi mục tiêu / AUTO / click / world Tab / focus khi chết | [GDD focus](1_HUYEN_LO_GDD.md#focus-input) + [Technical CycleTarget](2_HUYEN_LO_TECHNICAL.md#input-contract) |
| Pending / giữ phím / buffer / arrival / Esc | [GDD pending](1_HUYEN_LO_GDD.md#pending-cast), [Esc](1_HUYEN_LO_GDD.md#escape-priority) + [Technical contract](2_HUYEN_LO_TECHNICAL.md#input-contract) |
| Semantic actions: QuickHP / QuickMP / Food / Interact | [GDD Quick Potion / Food](1_HUYEN_LO_GDD.md#quick-items) + [GDD §9 bảng phím đề xuất](1_HUYEN_LO_GDD.md#ux-art) |
| Quái / crowd / staging / Home–Walk–Return | [GDD crowd](1_HUYEN_LO_GDD.md#melee-crowd) + [Technical §3](2_HUYEN_LO_TECHNICAL.md#combat-data) |
| World / mob / Linh / Boss | [GDD §4](1_HUYEN_LO_GDD.md#world-farm) + [Technical §7](2_HUYEN_LO_TECHNICAL.md#timers) |
| Farm Lv 1 → 20 / density và seed manifest lịch sử | [GDD world](1_HUYEN_LO_GDD.md#world-farm) + [Analysis §3](3_HUYEN_LO_DESIGN_ANALYSIS.md#farm-progression) |
| Terrain solid / one-way kết cấu / nước / không slope–climb | [GDD terrain](1_HUYEN_LO_GDD.md#terrain-rules) + [Art map](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#map-visual) + [Technical maps](2_HUYEN_LO_TECHNICAL.md#maps) |
| Story / bảy NPC / Q1–Q12 / class mentor | [GDD §5](1_HUYEN_LO_GDD.md#quests-story) + [Technical §3](2_HUYEN_LO_TECHNICAL.md#combat-data) |
| Gear HP/MP mới / cường hóa / chuyển giao / loot / economy | [GDD §6](1_HUYEN_LO_GDD.md#gear-economy), [Analysis baseline mới](3_HUYEN_LO_DESIGN_ANALYSIS.md#current-balance-probe) + [economy](3_HUYEN_LO_DESIGN_ANALYSIS.md#economy-analysis) |
| Food / Potion / death | [GDD §7](1_HUYEN_LO_GDD.md#consumables-death) + [Analysis gates](3_HUYEN_LO_DESIGN_ANALYSIS.md#open-decisions) |
| Online / PvP cược Vàng / chat | [GDD §8](1_HUYEN_LO_GDD.md#online-social) + [Technical §4–6](2_HUYEN_LO_TECHNICAL.md#network-authority) |
| Save / SafeAnchor / reconnect | [GDD §7–8](1_HUYEN_LO_GDD.md#consumables-death) + [Technical §5–6](2_HUYEN_LO_TECHNICAL.md#profile-authority) |
| UX / controls / art | [GDD §9](1_HUYEN_LO_GDD.md#ux-art) + [Technical §8–9](2_HUYEN_LO_TECHNICAL.md#art-contract) |
| Acceptance / demo / QA / gate revision mới | [GDD §10](1_HUYEN_LO_GDD.md#acceptance-routing), [Technical §12](2_HUYEN_LO_TECHNICAL.md#qa) + [Roadmap](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#revision-validation) |
| Dev Mode / preset / full fresh-run từng phái | [Technical tooling](2_HUYEN_LO_TECHNICAL.md#dev-mode) + [Roadmap DEV SPEED / acceptance](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#dev-speed-acceptance) |
| Quyết định và phần cần kiểm khi chơi thử | [Analysis §5](3_HUYEN_LO_DESIGN_ANALYSIS.md#open-decisions) |

## Quy ước cập nhật

Giữ năm file thiết kế active (1–5) và README làm điểm vào; tiền tố biểu thị thứ tự đọc. GDD là nguồn xác định luật hiện hành; không tạo GDD versioned, audit, archive hay Open Questions riêng.

Trạng thái và DoD hiện hành xem GDD; bằng chứng nghiệm thu xem Technical.

Đọc đúng nhãn **LOCKED / STRONG DIRECTION / BASELINE–TUNABLE / OPEN / LEGACY–SUPERSEDED** theo GDD; không biến mọi feedback thành luật khóa cứng. BASELINE–TUNABLE là giả định cụ thể để thử và đo, không bảo đảm balance đã đạt. P1/P2 không thành P0. Analysis §5 giữ sổ quyết định và trạng thái, gồm các alias A01–A17; Art giữ reasoning/ma trận thử, Roadmap giữ thứ tự và gate triển khai. Không mở sổ quyết định cạnh tranh.

Khi gộp nội dung, giữ mỗi luật tại một domain authority và chuyển evidence/proposal sang đúng nơi; không xóa chi tiết chỉ để giảm độ dài. P1 chưa duyệt vẫn được giữ rõ trạng thái tại [Analysis — candidates](3_HUYEN_LO_DESIGN_ANALYSIS.md#research-ideas).

Bảng số ở Analysis phục vụ tra cứu combat, density và economy; phải đọc nhãn current baseline hay historical evidence đi kèm. 28 groups/66 slots là seed manifest lịch sử, không phải tổng density hiện hành; các mô hình cũ chưa pass cadence/HP/gear mới. Không nhúng JSON/chart dài. Persistence P0 là Spring Boot + PostgreSQL theo Technical §5–6; JSON chỉ dùng config/fixture/import-export dev. Objectives/reward/unlock Q1–Q12 nằm trong **một bảng** GDD §5, recovery đặc biệt ngay sau bảng; production cần đủ 12 QuestDefinition và tuyến thật dù G-L chỉ chạy Q1–Q6. Dev presets không thay fresh-run acceptance. Luật bí kíp và nội tại tập trung ở GDD §3.

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
| Mốc để thử / số có thể chỉnh | BASELINE / TUNABLE | Giả định hiện hành cho probe/triển khai; phải đo và ghi setup, chưa tự là balance pass. |
| Luật đã chốt / hướng cần kiểm | LOCKED / STRONG DIRECTION | LOCKED là quyết định rõ; STRONG DIRECTION giữ hướng nhưng exact numbers/implementation còn cần kiểm. |
| Chưa quyết / đã bị thay | OPEN / LEGACY–SUPERSEDED | OPEN cần quyết định hoặc bằng chứng; LEGACY–SUPERSEDED giữ lịch sử, không dùng làm luật hiện hành. |
| Chọn kỹ năng / thực thi kỹ năng đã chọn | SelectSlot / ExecuteSelected | 1/2/3 chỉ chọn slot; ExecuteSelected là action riêng (phím vật lý còn OPEN, đề xuất E, xem [GDD §9](1_HUYEN_LO_GDD.md#ux-art)). Đổi slot không sửa pending/buffer/action đã chụp. |
| Mục tiêu chiến đấu | CombatFocus | NONE/AUTO/EXPLICIT; giữ đúng identity/life, độc lập selected skill và interaction candidate. |
| Đổi mục tiêu | CycleTarget | World Tab/Shift+Tab; context modal vẫn UI navigation. |
| Vùng tìm / giữ / thực thi | Search / Retention / Execution | Ba điều kiện riêng; mục tiêu giữ được chưa chắc nằm trong range/geometry hợp lệ để cast. |
| Lệnh chờ tiếp cận | PendingCast / Approach | Một intent chưa commit tài nguyên, đi ngang có giới hạn theo profile; không tạo map transition. |
| Đệm lệnh mới nhất | Latest buffer | Một intent có expiry/readiness theo GDD; không hàng đợi cast dài. |
| Dùng đồ nhanh / tương tác | QuickHP / QuickMP / Food / Interact | Các action ngữ nghĩa (semantic actions) độc lập với selected skill; phím vật lý cụ thể (đề xuất E/F/4/5/R/I) vẫn OPEN/TUNABLE, xem binding hiện hành tại [GDD §9](1_HUYEN_LO_GDD.md#ux-art). H/M là alias lịch sử (LEGACY), không dùng trong core hiện hành. |
| Cụm quái | SpawnGroup | Bố trí một bãi quái do designer author; **không phải Party**. |
| Điểm sinh quái | SpawnSlot | Vị trí có ID cố định, tham chiếu mob identity; hồi sinh đúng loài / level đó. |
| Vùng nhà / vùng đi / mặt đứng | HomeRegion / WalkRegion / SurfaceId | Ràng buộc map cho AI dùng chung; không suy ra quái GroundMelee được nhảy/drop hoặc leo tầng. |
| Đợi tiếp xúc / trở về | Staging / Return | Trạng thái dùng chung để xử lý crowd hoặc rời truy đuổi; chi tiết timer/regen còn theo OPEN, không AI riêng chống Cung. |
| Mốc đã chọn phái | ClassChosenLevel | Level tại transaction chọn phái thực tế; dùng để tính HP liên tục khi chọn Cung muộn. |
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

Art đọc [GDD §9](1_HUYEN_LO_GDD.md#ux-art) → [Art review/detail](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#art-review) → [Technical §8](2_HUYEN_LO_TECHNICAL.md#art-contract); layout/anchors đọc GDD §4. Thứ tự làm và [SOURCE → DESTINATION/audit](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#source-destination) ở Roadmap. Kỷ luật coding agent ở [Technical §1.1](2_HUYEN_LO_TECHNICAL.md#architecture-discipline). Co-farm không cần Party; world hỗ trợ N người theo contract dùng chung. **P0 acceptance: tối thiểu 2 concurrent players** kết nối Dedicated Game Server; đây là mức kiểm tối thiểu, chưa công bố capacity.
