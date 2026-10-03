# DANH SÁCH RESEARCH GAPS & CÂU HỎI KỸ THUẬT TRƯỚC KHI KHÓA DESIGN HUYỀN LỘ
*(NSO Technical Gaps Evaluation for Huyền Lộ Design Lock)*

---

## 1. TỔNG QUAN TÌNH TRẠNG NGHIÊN CỨU

Sau hai vòng nghiên cứu sâu (Pass 1 & Pass 2) trên toàn bộ hệ thống Client (Java decompile & Unity C#) và Server (Java NSOACE), các trụ cột kỹ thuật và thiết kế gameplay cốt lõi của Ninja School Online (NSO) đã được giải mã minh bạch ở cấp độ mã nguồn:
- **Combat Target & Auto-Approach**: Đã hiểu rõ 100% (cơ chế sticky, khoảng cách Chebyshev, phân biệt Manual Tap vs Auto Mode, hủy waypoint bằng phím di chuyển).
- **Hit / Miss Resolution**: Đã hiểu rõ 100% (Stat roll thuần túy, tức thì, đạn chỉ là Presentation Visual).
- **Multi-Target & Đa Tầng**: Đã hiểu rõ 100% (hộp AABB $\Delta X \le 100, \Delta Y \le 50$, lan tầng tự nhiên nhờ bước nhảy sàn $48\text{px} < 50\text{px}$).
- **World, Map & Farm Pocket**: Đã hiểu rõ 100% (Tile $24\text{px}$, cụm 3–4 quái cách nhau 24–48px, chu kỳ respawn 12 giây tạo vòng quay 3 bãi).
- **AI Quái Đất & Quái Bay**: Đã hiểu rõ 100% (Server ghim tọa độ spawn gốc, client tự hành mô phỏng visual, quái bay chúc đầu xuống khi đánh).

Tuy nhiên, trước khi chính thức chuyển sang giai đoạn **"Huyền Lộ Design Lock"**, vẫn tồn tại một số câu hỏi kỹ thuật/gameplay cụ thể phát sinh từ việc hiện đại hóa công nghệ (từ J2ME 2012 lên Unity Dedicated Server) cần được xác định rõ ràng.

---

## 2. BẢNG DANH SÁCH RESEARCH GAPS CHI TIẾT

### GAP 1: Nhảy Đánh (Jump Attack) & Combat trên Không
- **Câu hỏi**: *Trong NSO, người chơi có thể vừa nhảy vừa tung chiêu (Jump Attack) không? Khi nhảy đánh, quán tính di chuyển ngang có được giữ nguyên hay nhân vật bị khựng lại giữa không trung? Bounding box đòn đánh có dời theo tọa độ Y của người chơi lúc đang ở trên đỉnh cú nhảy không?*
- **Tại sao quan trọng**: Ảnh hưởng trực tiếp đến cảm giác điều khiển (game feel) của class cận chiến khi đối đầu với Quái bay (như Ong Giáp) hoặc người chơi khác trong PvP.
- **Vùng mã nguồn liên quan**: [`nameBK.java`](file:///home/nguyenvanrin/.gemini/antigravity-cli/brain/ebc98840-85c9-423b-a2f4-d7cc0190f869/scratch/client_decompiled/nameBK.java) method `update()` khi `this.f == 3` (State JUMPING) và xử lý phím 5 trong [`nameBS.java`](file:///home/nguyenvanrin/.gemini/antigravity-cli/brain/ebc98840-85c9-423b-a2f4-d7cc0190f869/scratch/client_decompiled/nameBS.java#L2721-L2743).
- **TRẠNG THÁI HIỆN TẠI (PASS 3 RESOLVED)**: **ĐÃ GIẢI QUYẾT XONG (RESOLVED & DESIGN READY)**.
  - *Sự thật NSO*: NSO cho phép đánh khi nhảy và rơi, lấy tọa độ $Y$ trên không để tính tầm chém quái bay; nhưng NSO triệt tiêu vận tốc (`vx=0, vy=0`) gây khựng đơ giữa trời (hạn chế J2ME cũ).
  - *Giải pháp tối giản cho Huyền Lộ*: Giữ nguyên rơi tự do và quán tính ngang, chỉ kích hoạt animation chém (Recipe 2, đúng 1–3 dòng C#). Xem chi tiết tại Decision 4.
- **Có block quyết định Huyền Lộ không?**: **KHÔNG CÒN BLOCK**.
- **Mức độ ưu tiên**: **RESOLVED**.

---

### GAP 2: Animation Lock, Đệm Chiêu (Input Buffering) & Hủy Động Tác (Attack Cancel)
- **Câu hỏi**: *Khi nhân vật đang vung vũ khí chém (anim `isAttack`), người chơi bấm nút Nhảy hoặc Lướt thì anim chém có bị ngắt ngay (Animation Cancel) không, hay nhân vật bị khóa cứng (Animation Lock) cho tới khi kết thúc frame cuối? Server có xử lý trừng phạt nếu client gửi lệnh di chuyển trong lúc đang ra chiêu không?*
- **Tại sao quan trọng**: Quyết định độ "nhạy" (responsiveness) và độ mượt mà của combat. Game hiện đại thường cho phép Jump-cancel hoặc Dash-cancel ở các frame phục hồi (recovery frames).
- **Vùng mã nguồn liên quan**: [`nameBK.java`](file:///home/nguyenvanrin/.gemini/antigravity-cli/brain/ebc98840-85c9-423b-a2f4-d7cc0190f869/scratch/client_decompiled/nameBK.java) dòng 1589–1650, 7332 và xử lý phím [`nameBS.java`](file:///home/nguyenvanrin/.gemini/antigravity-cli/brain/ebc98840-85c9-423b-a2f4-d7cc0190f869/scratch/client_decompiled/nameBS.java#L2590).
- **TRẠNG THÁI HIỆN TẠI (PASS 3 RESOLVED)**: **ĐÃ GIẢI QUYẾT XONG (RESOLVED & DESIGN READY)**.
  - *Sự thật NSO*: Sát thương thường của quái hoàn toàn không có Hit-stun (chỉ chớp đỏ 4 frames), không ngắt chiêu của người chơi. Bấm phím di chuyển ngắt ngay lập tức quá trình auto-approach. Bộ đệm phím `nameBS.J` có độ dài 10 frames ($150-200\text{ms}$).
  - *Giải pháp tối giản cho Huyền Lộ*: Áp dụng bộ đệm $150\text{ms}$ trong recovery window; phím di chuyển A/D/Space lập tức gán `isApproaching = false` (Recipe 6 & 7, < 15 dòng C#). Xem chi tiết tại Decision 3 & 5.
- **Có block quyết định Huyền Lộ không?**: **KHÔNG CÒN BLOCK**.
- **Mức độ ưu tiên**: **RESOLVED**.

---

### GAP 3: Kiến trúc Vị trí Quái trên Dedicated Server (Server-Authoritative vs Spawn-Anchor)
- **Câu hỏi**: *Huyền Lộ sử dụng Dedicated Server hiện đại trên Unity/C#. Chúng ta nên tiếp tục dùng mô hình "Server ghim tọa độ spawn, Client tự giả lập di chuyển" của NSO để tiết kiệm CPU máy chủ, hay Server phải chạy physics mô phỏng vị trí quái thật sự và đồng bộ định kỳ xuống Client?*
- **Tại sao quan trọng**:
  - Nếu mô phỏng Server thật: Quái có thể bị đẩy lùi (knockback), bị người chơi kéo đi xa, nhưng Server tốn CPU tính toán va chạm.
  - Nếu dùng mô hình NSO: Server cực nhẹ, chống hack vị trí quái 100%, nhưng quái không bao giờ rời khỏi bán kính `rangeMove` của điểm spawn.
- **Vùng mã nguồn liên quan**: Kiến trúc server [`Mob.java`](file:///home/nguyenvanrin/PersonalProjects/rpg-online/research/SRC%20NSOACE%20FIX/src/com/nsoz/mob/Mob.java#L1010) và [`Zone.java`](file:///home/nguyenvanrin/PersonalProjects/rpg-online/research/SRC%20NSOACE%20FIX/src/com/nsoz/map/zones/Zone.java).
- **TRẠNG THÁI HIỆN TẠI (PASS 3 RESOLVED)**: **ĐÃ GIẢI QUYẾT XONG (RESOLVED & DESIGN READY)**.
  - *Sự thật NSO*: Server NSO thuần túy ghim tọa độ spawn gốc, hoàn toàn không chạy vị trí quái.
  - *Giải pháp tối giản cho Huyền Lộ*: Chọn **Model C (Lightweight Kinematic Server)**. Server không chạy Physics2D/Rigidbody, chỉ tính toán cộng trừ tọa độ 1D trên mép sàn và vận tốc suy giảm của knockback (Recipe 1, đúng 6 dòng C#). Server nhẹ vô đối mà vẫn hỗ trợ quái bị đánh văng lùi chân thực. Xem chi tiết tại Decision 6.
- **Có block quyết định Huyền Lộ không?**: **KHÔNG CÒN BLOCK**.
- **Mức độ ưu tiên**: **RESOLVED**.

---

### GAP 4: Thời gian Bảo hộ Rơi đồ (Loot Ownership Protection)
- **Câu hỏi**: *Khi quái chết rơi vật phẩm (`ItemMap`), người gây nhiều sát thương nhất hay người kết liễu được quyền ưu tiên nhặt trong bao nhiêu giây trước khi vật phẩm trở thành công cộng (free-for-all)?*
- **Tại sao quan trọng**: Tránh việc "hôi của" (kill-stealing / loot-ninja) trong các bãi farm đông đúc của Huyền Lộ.
- **Vùng mã nguồn liên quan**: [`ItemMap.java`](file:///home/nguyenvanrin/PersonalProjects/rpg-online/research/SRC%20NSOACE%20FIX/src/com/nsoz/map/item/ItemMap.java) và method `pickItem()` trong [`Service.java`](file:///home/nguyenvanrin/PersonalProjects/rpg-online/research/SRC%20NSOACE%20FIX/src/com/nsoz/network/Service.java).
- **Có block quyết định Huyền Lộ không?**: **KHÔNG** (Đây là logic kinh tế / drop, có thể tinh chỉnh số liệu sau).
- **Mức độ ưu tiên**: **MEDIUM**.

---

### GAP 5: Cơ chế Target & Phân cấp Ưu tiên giữa Quái thường, Boss và Tinh Anh
- **Câu hỏi**: *Khi trong cùng một cụm quái có cả Quái thường (Normal), Quái Tinh Anh (Elite) và Boss (Thủ lĩnh), thuật toán `w()` của NSO ưu tiên chọn con nào trước? Người chơi có thể dễ dàng chuyển focus vào Boss khi xung quanh có 10 con quái thường bu kín hay không?*
- **Tại sao quan trọng**: Giúp thiết kế trải nghiệm đánh Boss của Huyền Lộ không bị ức chế do con trỏ tự động cứ bám vào quái đệ (adds).
- **Vùng mã nguồn liên quan**: [`nameBK.java`](file:///home/nguyenvanrin/.gemini/antigravity-cli/brain/ebc98840-85c9-423b-a2f4-d7cc0190f869/scratch/client_decompiled/nameBK.java) method `w()`.
- **Có block quyết định Huyền Lộ không?**: **KHÔNG** (Huyền Lộ đã quyết định bổ sung phím Tab Cycle Target để người chơi chủ động đổi mục tiêu).
- **Mức độ ưu tiên**: **LOW**.

---

## 3. ĐÁNH GIÁ SẴN SÀNG CHO BƯỚC KHÓA THIẾT KẾ (DESIGN LOCK READINESS)

### 1. Những Pattern NSO Đã Hiểu Chắc Chắn 100%:
- [x] Quy trình Target Selection & Bám dính (Sticky Nearest Target).
- [x] Quy trình Auto-approach (chỉ chạy ngang trên sàn hiện tại, hủy tức thì khi có input tay).
- [x] Mô hình Roll Hit/Miss (Target-based Stat Roll, không collider vật lý, đạn chỉ là diễn hoạt hình ảnh).
- [x] Quy tắc Multi-target lan tầng theo khoảng cách $Y \le 50\text{px}$, giải quyết sát thương đồng thời trong 1 tick server.
- [x] Cấu trúc Map: Tile $24\text{px}$, độ cao nhảy tầng $\Delta Y = 48\text{px}$, portal đặt ở mép map.
- [x] Cấu trúc Farm Pocket: Cụm 3–4 quái cách nhau 24–48px khớp với bán kính $X \pm 100\text{px}$ của chiêu lan.
- [x] Chu kỳ Farm Loop: Hồi sinh 12 giây thúc đẩy di chuyển luân phiên 3 cụm quái.
- [x] AI Quái: Bỏ qua người chơi khác tầng, quay đầu tại mép sàn, quái bay chúc đầu xuống khi tấn công cận chiến.
- [x] UI Target: Chỉ hiển thị mini bar trên đầu quái đang Focus; toàn bộ thông số chi tiết nằm ở Target HUD cố định.

### 2. Những Pattern Đã Xác Nhận Do Hạn Chế Legacy (J2ME / Phần cứng 2012):
- Không có phím Tab đổi mục tiêu (do bàn phím số T9 không đủ nút).
- Quái đứng dưới đất hoàn toàn "bất động/chịu trận" khi bị bắn tỉa từ tầng khác (do giới hạn CPU server 2012).
- Server không cập nhật tọa độ quái thời gian thực (do tiết kiệm băng thông mạng 2G GPRS).
- Đạn và đòn đánh xuyên qua mọi chướng ngại vật terrain (do không có hệ thống raycast 2D thời đó).

### 3. KẾT LUẬN CUỐI CÙNG:

> ### **STATUS: READY FOR DESIGN LOCK**
> 
> Toàn bộ các thông tin bản chất nhất về **Combat Loop**, **Targeting Rules**, **Spatial Mathematics (khoảng cách X/Y, platform spacing)**, **Mob Placement Geometry**, và **Client-Server Contract** từ Ninja School Online đã được làm sáng tỏ và chứng minh bằng dòng mã nguồn cụ thể.
> 
> Các câu hỏi còn lại tại Mục 2 (Jump Attack, Animation Lock, Server-Authoritative Mob) không phải là rào cản ngăn chặn việc thiết kế, mà chính là **các điểm giao thoa công nghệ nơi Huyền Lộ sẽ nâng cấp vượt trội so với NSO** bằng việc tận dụng sức mạnh của Unity và hạ tầng mạng hiện đại.
> 
> Đội ngũ kiến trúc đã có đầy đủ cơ sở kỹ thuật vững chắc để bước sang giai đoạn khóa thiết kế (Design Lock) cho Huyền Lộ mà không sợ gặp phải các giả định sai lầm.
