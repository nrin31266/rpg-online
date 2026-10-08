# Huyền Lộ — World & Content

Các số BASELINE/TUNABLE chưa phải nghiệm thu runtime.

**CURRENT:** topology và candidate density đã có; tổng placement cuối vẫn TUNABLE. Linh ngẫu nhiên và reservation Q8 là hai quyền spawn phân biệt. [Q8 reservation](#q8-bounded-path) không được phụ thuộc cái chết của Linh ở nơi khác.

## Document owns

Map/gate/topology, terrain intent, mob/AI gameplay, density/SpawnGroup, Linh Biến và Boss.

## Document does not own

Luật ngoài domain thuộc owner trong [documentation map](../README.md); evidence thuộc [Playtest & Balance](../04-production/playtest-and-balance.md), thứ tự triển khai thuộc [Roadmap](../04-production/roadmap.md).

<a id="world-farm"></a>
<a id="gdd-5"></a>

<a id="4-thế-giới-và-bãi-farm"></a>

## Thế giới và bãi farm

**World route:** Vân Khê ↔ Đồng Sương ↔ Trúc Ảnh ↔ Bạch Vân ↔ Xích Nham ↔ Huyền Tích. Vân Khê nối Học Viện và Lôi Đài. Q11 mở Huyền Môn từ Xích Nham. Không fast travel ra bãi; Bùa Hồi Thành P1 chỉ đưa về làng.

Mỗi map farm/combat có **một SafeAnchor cố định** để khôi phục khi phiên chơi đã mất; người chơi không xuất hiện lại giữa bãi quái. Vân Khê và Học Viện là khu an toàn: có thể khôi phục tọa độ đã lưu nếu còn hợp lệ, nếu không thì dùng SafeAnchor. Lôi Đài không có điểm khôi phục phiên; trận PvP theo lifecycle riêng (các mục liên quan).

| Vùng | Hình thái và mục đích |
| --- | --- |
| Vân Khê | Hub yên bình: NPC, mua thuốc, rèn đồ, rương và nghỉ; đường về đọc được từ mũi tên mép map. |
| Học Viện | Khu nhập môn với ledge / drop-through và bãi Bù Nhìn; thao tác lớp học dùng NPC / terrain hiện có. |
| Lôi Đài | Không gian tỷ thí 1v1, tách khỏi farm world bằng match membership. |
| Đồng Sương | Đồng thoáng, 1–2 tầng; thấy quái và lối thoát sớm, chưa ép gom đông. |
| Trúc Ảnh | Rừng trúc, cầu và 2–3 tuyến cao độ; tập xoay bãi, nhảy / kite, lần theo vết ấn dưới cầu. |
| Bạch Vân | Các bậc địa hình bên thác, tuyến vòng; nhóm ba bắt đầu làm tiến cảnh có giá trị. Ba bậc là ví dụ bố cục, không khóa số tầng. |
| Xích Nham | Hẻm núi rộng nhất, nhánh tách rồi nhập lại; ngoại vi luyện công rồi đi sâu tới ba Mạch Ấn/Huyền Môn. Hai nhánh là ví dụ bố cục, không khóa tổng nhánh. |
| Huyền Tích | Ngoại vi phế tích dẫn tới landmark Cự Thú; khoảng trống Boss tách normal spawn để đọc telegraph. |

## Mở bản đồ, mật độ và cụm quái cố định — TEST / TUNABLE

Mỗi điểm sinh quái chọn một **mob identity có level cố định**, vị trí và cụm. Level thuộc identity, `SpawnSlot.level` chỉ cache/validation bằng level đó; không author cùng loài lên nhiều level. Respawn giữ nguyên identity/level. Linh Biến chỉ thêm modifier; cụm là bố trí bãi, không formation hoặc tổ đội runtime.

| Map / gate | Ý đồ bãi và topology nội bộ — STRONG DIRECTION | Traversal tham khảo — TUNABLE |
| --- | --- | --- |
| Đồng Sương / onboarding | Đồi bậc thấp và tuyến dưới; cụm hai quái vẫn phù hợp, thêm các bãi độc lập thay vì một blob lớn | 25–35 s |
| Trúc Ảnh / Q6 Completed + Lv 5 | Nhánh trên cầu/nhánh dưới trấn ấn và vòng về; giữ `TA4.slot1` cho Q8 | 35–55 s |
| Bạch Vân / Q8 Completed + Lv 8 | Các terrace solid quanh thác, tuyến vòng và mỏm cụt; cụm trên/dưới cùng xuất hiện trên camera | 35–55 s |
| Xích Nham / Q8 Completed + Lv 12 | Ngoại vi tách nhánh sâu, hốc/khe đá và ba khu trấn ấn; lối Huyền Môn ở nhánh phù hợp | 35–55 s |
| Huyền Tích / Q11 Completed | Cấu trúc phế tích có tuyến cao/thấp và ngách, khoảng Boss tách normal spawn | 45–60 s |

**STRONG DIRECTION mật độ:** Tăng số lượng bãi/cụm độc lập trên các tuyến và độ cao để thế giới trực tuyến đông đúc, khắc nghiệt và có nhịp độ hợp lý hơn; tuyệt đối **không tăng quy mô một cụm thành khối dồn cục (blob) 8–10 quái**.

Mục tiêu là trong một khung hình camera (khung nhìn tiêu chuẩn), người chơi có thể quan sát thấy nhiều cụm nằm ở các thềm đá/tầng cao/tuyến nhánh khác nhau (khoảng 5–8+ quái cùng xuất hiện trong tầm mắt), nhưng mỗi cụm là một `SpawnGroup` độc lập với vùng đi lại (`WalkRegion`) và giới hạn đuổi (`LeashRegion`) riêng biệt. Khi chiến đấu, người chơi chỉ kích hoạt từng nhóm nhỏ, không gây báo động dây chuyền sang các cụm lân cận.

### Đặc tả thiết kế không gian và topology nội bộ 8 bản đồ logic

Đồ thị thế giới (`World Graph`) quyết định cách các bản đồ liên kết với nhau, trong khi đồ thị nội bộ (`Internal Map Graph`) định hình trải nghiệm điều khiển, tầm nhìn và nhịp độ chiến đấu. **Tuyệt đối không thiết kế các bản đồ theo kiểu hành lang phẳng một chiều đơn điệu (vào bên trái → chạy thẳng một mạch → thoát bên phải)**.

Mỗi bản đồ phải sở hữu nhận diện không gian riêng biệt thông qua sự kết hợp của: các nhánh rẽ (branches), đường vòng lặp quay về (loops), thềm đá cao thấp (terraces), gờ nhảy (ledges), hốc hang khoét sâu (hollows), đoạn rơi xuyên sàn (drops) và các ngõ cụt đặt bãi tài nguyên/quái tinh anh (dead-ends).

Thế giới gồm **8 logical map roots** (Vân Khê, Học Viện, Lôi Đài và 5 map farm dã ngoại):

<a id="1-làng-vân-khê--hub-bình-yên--điểm-tựa-sơn-cước"></a>

#### Làng Vân Khê — Hub bình yên & Điểm tựa sơn cước
- **Vai trò:** Khu vực an toàn tuyệt đối, không có quái vật. Nơi tập trung toàn bộ dịch vụ cốt lõi, tiếp nhận nhiệm vụ và là điểm trở về sau các chuyến thám hiểm.
- **Phân khu chức năng không gian (Spatial Layout):**
  + *Khu trung tâm công cộng:* Nơi già làng Lâm Bá đứng bên gốc đa cổ thụ và bảng chỉ dẫn, đón tiếp người chơi mới (Q1/Q2), dẫn dắt cốt truyện chính và phong ấn Huyền Môn (Q8, Q10–Q12).
  + *Khu Dược thảo (phía Đông):* Nhà thuốc mộc mạc của Yên Thảo, bày các sọt thảo mộc phơi khô, phục vụ mua bán bình Máu/Linh lực, thức ăn, bùa Hồi Sinh và dịch vụ Tẩy Mạch Phù.
  + *Khu Lò rèn (phía Tây tựa vách đá):* Xưởng rèn rực lửa than của Bách Luyện với đe thép và bễ thổi, phụ trách rèn trang bị, cường hóa, chuyển giao và nhiệm vụ Mộc Kiếm (Q3/Q4/Q7).
  + *Khu Kho lương & Nhà nghỉ (phía Bắc):* Gian nhà gỗ yên tĩnh của Mộc An, cung cấp dịch vụ cất giữ đồ đạc (`Storage`) và nghỉ ngơi hồi phục toàn bộ sinh lực/linh lực.
  + *Các lối thông map:* Nhánh Tây nối sang Học Viện (`EdgeExit`); Nhánh Đông nối sang Đồng Sương (`EdgeExit`); Nhánh Nam dẫn xuống Lôi Đài của Hạo Vũ (`SpecialGate`).
- **Triết lý Onboarding Q1:** Người chơi không đứng một chỗ bấm hội thoại menu mà phải thực sự di chuyển bộ qua từng khu vực chức năng, nhận diện vị trí các NPC để hình thành bản đồ nhận thức không gian (mental map) vững chắc.

<a id="2-học-viện--huấn-luyện-nhập-môn--điện-nhập-phái"></a>

#### Học Viện — Huấn luyện nhập môn & Điện Nhập Phái
- **Vai trò:** Khu vực bán an toàn dành riêng cho tập luyện kỹ năng cơ bản, thử nghiệm di chuyển và nghi thức chọn phái (Q2, Q3, Q6).
- **Phân khu 3 khu vực cốt lõi:**
  + *Tuyến vượt chướng ngại vật Q2 (`HV_ObstacleCourse`):* Bắt đầu từ cửa vào (`HV_Entrance`), người chơi phải nhảy qua gờ đá cao (`HV_JumpLedge`), tiếp cận sàn gỗ mỏng trên cao rồi bấm `↓` để rơi xuyên sàn (`DropThrough`) đáp xuống thềm dưới (`HV_DropLanding`), sau đó men theo đường vòng quay lại lối ra làng. Tuyến này kiểm tra trực quan toàn bộ năng lực di chuyển cơ bản (Move, Jump, DropThrough) trước khi cho phép cầm vũ khí.
  + *Sân tập Bù Nhìn (`HV_DummyYard`):* Bãi đất bằng phẳng bố trí các cọc Bù Nhìn rơm độc lập theo [Q3 owner](quests-and-narrative.md#quests-story). Việc đặt nhiều cọc ngăn chặn tình trạng người chơi chen lấn tranh giành mục tiêu khi làm Q3 và Q6.
  + *Khu vực Nhập Phái (`HV_ClassHall` / Điện Nhập Phái):* Khu vực riêng biệt cho hai phái với functional zoning rõ ràng; vị trí tả/hữu/Tây là đề xuất blockout tham khảo (tọa độ chính xác OPEN). Mentor Phong Du (Kiếm) và Mentor Diệp Lam (Cung) được bố trí bình đẳng, dễ thấy như nhau, khẳng định không đặt Kiếm Sĩ làm lựa chọn mặc định trước Cung Thủ.

<a id="3-lôi-đài--đấu-trường-1v1-pvp"></a>

#### Lôi Đài — Đấu trường 1v1 PvP
- **Vai trò:** Không gian thi đấu đối kháng trực tiếp giữa hai người chơi theo giao kèo cược (Q9 và hệ thống PvP tự do).
- **Thiết kế không gian:** Sàn đấu đá tảng nguyên khối hoàn toàn phẳng, sạch chướng ngại vật, không có bậc địa hình nhấp nhô hay sàn one-way để đảm bảo tính công bằng. Bao quanh là rào/tường kín; tuyệt đối không dựng khán đài hay công trình phức tạp. Chi tiết trang trí (rào, cờ, background) do [Art](../03-art/art-and-visual-production.md) quyết định.

<a id="4-đồng-sương-lv-15--đồi-nương-bậc-thấp--bờ-suối-sương-mai"></a>

#### Đồng Sương (Lv 1–5) — Đồi nương bậc thấp & Bờ suối sương mai
- **Ý đồ không gian & Topology:** Môi trường mở, chuỗi bậc thấp tạo cảm giác địa hình thoải khi nhìn tổng thể, thoáng đãng với đồi cỏ và nương rẫy ven suối cạn. Giúp người chơi làm quen với nhịp độ chiến đấu, di chuyển vượt bậc nhỏ và gom nhặt chiến lợi phẩm.
- **Phân bố 2 tuyến đường:**
  + *Tuyến dưới (Lower Lane):* Men theo bờ suối cạn nước nông và vạt nương thấp, nền đất bằng phẳng, bố trí các bãi Nấm Linh Lv 2 di chuyển chậm (cụm `DS1`, `DS2` cho Q4, cùng `DS7`, `DS8` mở rộng).
  + *Tuyến đồi giữa và trên (Middle/Upper Terrace):* Các thềm đồi cỏ bậc solid vững chãi, liên kết nhau bằng các bước nhảy ngắn (cao độ 1–1,5 u), nơi bầy Sói Sương Lv 4 nhanh nhẹn tuần tra (cụm `DS3–DS6` cho Q5, cùng `DS9`, `DS10` mở rộng).
- **Ranh giới an toàn & Lối thoát:** Dải vào an toàn 6–8 u tại cửa ngõ phía Tây giáp Vân Khê; lối thoát sang Trúc Ảnh (`EdgeExit`) nằm ở thềm đồi phía Đông.

<a id="5-trúc-ảnh-lv-510--rừng-trúc-u-tịch--cầu-gỗ-đa-tầng"></a>

#### Trúc Ảnh (Lv 5–10) — Rừng trúc u tịch & Cầu gỗ đa tầng
- **Ý đồ không gian & Topology:** Chênh lệch cao độ bắt đầu rõ rệt với rừng trúc dày đặc, vách đá phủ rêu và hệ thống cầu giàn ván bắc qua đèo. Đây là nơi kiểm tra khả năng phối hợp kỹ năng mới nhận sau khi nhập phái (Lv 5+).
- **Phân bố 3 tuyến đường & Vòng lặp (Loops):**
  + *Tuyến cầu trên cao (Upper Bridge Route):* Kết cấu giàn ván mỏng (sàn one-way) vắt ngang giữa hai mỏm đá, nơi Ong Giáp Lv 10 bay lơ lửng, tạo áp lực tấn công tầm cao (cụm `TA5`, `TA9`, `TA10`).
  + *Tuyến rừng trúc trung tâm (Mid Bamboo Forest):* Thềm đất ẩm ướt dưới tán trúc quanh trụ Trấn Ấn cổ bị nứt (`TA4_BrokenSeal`), nơi bầy Sói Trúc Ảnh Lv 8 hung hãn mai phục (cụm `TA4` với `slot 1` cố định cho Q8 Linh Biến, cụm `TA6`, `TA7`, `TA8`).
  + *Tuyến ven suối trũng (Lower Stream Trail):* Ranh giới phía Tây còn sót lại các cụm Sói Sương Lv 4 (`TA1–TA3`).
- **Vòng lặp cơ động:** Người chơi có thể đứng trên cầu gỗ bấm `↓` để nhảy xuyên sàn rơi xuống bãi trúc dưới chân, hoặc đi vòng qua bậc đá trực giao phía sau để leo ngược lên cầu, tạo nhịp cơ động tự nhiên khi thả diều quái.

<a id="6-bạch-vân-lv-813--vách-đá-thác-nước--đèo-mây-nhiều-bậc-cao-độ"></a>

#### Bạch Vân (Lv 8–13) — Vách đá thác nước & Đèo mây nhiều bậc cao độ
- **Ý đồ không gian & Topology:** Bản đồ thẳng đứng và hiểm trở nhất; bố cục blockout tham khảo dùng khoảng **3 tầng thềm đá vững chắc (terrace solid, TUNABLE, không khóa số tầng)** ôm quanh ngọn thác nước đổ xuống vực.
- **Phân bố cao độ & Lợi thế class:**
  + *Thềm trên cao quanh đỉnh thác (Upper Falls Terrace):* Không gian mở trên vách đá, nơi Ong Giáp Lv 10 bay lượn trên cao (cụm `BV1`, `BV2`, `BV6`).
  + *Tuyến terrace bậc giữa và hốc hang (Middle Cliff Terraces):* Các thềm đá bậc nối tiếp và các hốc đá khoét sâu vào lòng vách núi, nơi các toán Đoạt Mạch Đạo Tặc Lv 13 đóng trại khai thác khoáng (cụm `BV3–BV5` farm/progression Đoạt Mạch Lv 13, cùng `BV7`, `BV8`).
  + *Mỏm đá cụt nhìn ra vực (Dead-end Overlook):* Điểm ngắm cảnh mây mù và bãi farm phụ với góc nhìn bao quát toàn bộ thác nước.
- **Tương tác chiến đấu:** Cung thủ tận dụng tầm bắn xa 6,5 u đứng từ thềm trên tỉa xuống các toán đạo tặc bên dưới; Kiếm Sĩ tận dụng góc hang hẹp của hốc đá để gom cụm 3 quái tung Phong Trảm tiến cảnh diện rộng.

<a id="7-xích-nham-lv-1217--hẻm-đá-phân-nhánh--mạch-ngầm-phong-ấn"></a>

#### Xích Nham (Lv 12–17) — Hẻm đá phân nhánh & Mạch ngầm phong ấn
- **Ý đồ không gian & Topology:** Bản đồ có diện tích rộng, đặc trưng bởi hệ thống khe nứt địa chất và các thềm đá bậc cao độ; bố cục blockout tham khảo dùng khoảng **2 nhánh lớn hội tụ (Two Branches Merge, TUNABLE, không khóa tổng nhánh)**. Chi tiết chất liệu đá, ánh sáng sa thạch do [Art](../03-art/art-and-visual-production.md) sở hữu.
- **Phân bố 2 nhánh chiến lược:**
  + *Nhánh hẻm núi ngoại vi (Canyon Branch):* Tuyến đèo đá đỏ nhiều bậc cao độ dẫn từ Bạch Vân vào, nơi các toán Đoạt Mạch Đạo Tặc Lv 13 rải rác đào trộm cổ vật (cụm `XN1–XN3` cho Q10, cụm `XN7`).
  + *Nhánh khe nứt khoáng mạch ngầm (Deep Rift Branch):* Tuyến đường ăn sâu vào lòng núi, nơi bố trí **3 trụ phong ấn cổ xưa** (`XN4_SealA`, `XN5_SealB`, `XN6_SealC`) do Xích Thạch Linh Lv 16 canh gác (cụm `XN4–XN6` cho Q11, cùng `XN8`, `XN9`).
- **Điểm kết nối tối thượng:** Cuối nhánh sâu là đại môn Huyền Môn (`XN_HuyenMon_Outer`) tựa vào vách núi — một `SpecialGate` phong tỏa lối vào cấm địa, chỉ mở ra khi hoàn thành nghi thức thu thập đủ 3 Mảnh Cổ Ấn trong Q11.

<a id="8-huyền-tích-lv-1720--phế-tích-cấm-địa--world-boss-huyền-nham-cự-thú"></a>

#### Huyền Tích (Lv 17–20) — Phế tích cấm địa & World Boss Huyền Nham Cự Thú
- **Ý đồ không gian & Topology:** Phế tích đá cổ đại khép kín với các bậc thang lớn và cấu trúc phòng sảnh phân cấp. Chi tiết mỹ thuật (rêu phong, hoa văn ấn mờ, ánh sáng tím) do [Art](../03-art/art-and-visual-production.md) sở hữu; design owner định nghĩa functional topology, encounter zones và ranh giới Boss.
- **Phân khu chức năng nghiêm ngặt:**
  + *Tiền môn và hành lang ngoài (Outer Gate & Corridors):* Các thềm đá bậc dẫn vào phế tích, do Xích Thạch Linh Lv 16 trấn giữ lối vào (cụm `HT1`, `HT6`).
  + *Trung sảnh và hai cánh tả/hữu (Great Hall & Wings):* Dãy hành lang đá với các bậc thang cao kết nối các phòng phụ, nơi Cổ Môn Vệ Binh Lv 20 đứng gác (cụm `HT2–HT5` cho Q12, cùng `HT7`, `HT8`).
  + *Khu vực cấm điện trung tâm — BossCombatArea:* Đại sàn đấu phẳng, sạch chướng ngại vật; **tách biệt tuyệt đối khỏi quái thường (normal-spawn exclusion)**. Đây là đấu trường dành riêng cho World Boss Huyền Nham Cự Thú trong Q12, đảm bảo telegraph đòn đánh của Boss luôn rõ ràng, không bị quái thường quấy nhiễu hay gây nhiễu loạn mục tiêu.

---

### Quy chuẩn địa hình trực giao và cơ chế di chuyển thế giới

Để đảm bảo tính nhất quán tuyệt đối giữa mỹ thuật, vật lý và trí tuệ nhân tạo (AI), toàn bộ thế giới tuân thủ các quy tắc bất biến sau:

1. **Khối đặc tự nhiên (Natural Terrain = SOLID MASS):** Đất, đá, gờ núi, thềm đồi tự nhiên luôn là khối chắn đặc có độ dày thực tế: mặt trên nằm ngang, khối vật liệu lấp kín bên trong, vách đứng thẳng góc, đáy và bóng đổ khép kín. Tuyệt đối không vẽ các dải đất tự nhiên mỏng manh lơ lửng giả làm đồi núi.
2. **Không dốc chơi được (`no playable slope/ramp/triangle`):** Toàn bộ bề mặt di chuyển trong gameplay đều là mặt phẳng ngang hoặc vách đứng trực giao. Mái nhà, cành cây, núi xa ở phông nền có thể vẽ chéo cho mềm mại thẩm mỹ, nhưng mặt va chạm gameplay tiếp xúc với chân nhân vật vẫn phải là các bậc ngang/đứng.
3. **Đất đá tự nhiên không bao giờ là sàn xuyên thấu (`no natural one-way`):** Nền đất đá tự nhiên luôn cản trở hai chiều.
4. **Sàn One-way là kết cấu mỏng nhân tạo đặc biệt:** Chỉ các cấu trúc mỏng nhẹ hợp lý như ván gỗ, giàn tre/catwalk, ban công, sàn treo tựa vách có dầm đỡ/dây treo rõ ràng mới được dùng làm sàn one-way. Khi đứng trên sàn one-way, người chơi có thể bấm `↓` để rơi xuyên sàn (`DropThrough`). Một lần bấm chỉ xuyên qua một tầng sàn, không xuyên liên tiếp nhiều tầng khi giữ nút.
5. **Tuyệt đối không có cơ chế leo trèo (`no ladder/rope/vine/climb`):** Không có thang dây, dây leo, cột đu hay bám tường leo trèo; toàn bộ di chuyển dọc dựa vào Nhảy (`Jump` - `↑`), Rơi tự do (`Fall`) và Xuyên sàn (`DropThrough` - `↓`). Mọi cầu thang trong game đều là khối bậc trực giao hoặc chi tiết trang trí.
6. **Vùng nước nông (Shallow Water):** Lòng nước nông có đáy đất thật, người chơi lội qua thì chân tiếp xúc mặt nước sẽ giảm nhẹ tốc độ chạy (hệ số TUNABLE); khi đi trên cầu gỗ hoặc nhảy trên không qua mặt nước thì không bị giảm tốc. Tuyệt đối không có cơ chế bơi lội, chết đuối hay vật lý thủy động lực học.
7. **Cơ chế chuyển tiếp bản đồ:**
   - `EdgeExit`: Vùng mép bản đồ thông thường có mũi tên chỉ hướng và tên vùng đích; nhân vật đi chạm vào vùng này bằng di chuyển thủ công sẽ tự động chuyển map, không cần bấm phím tương tác và không dựng vòm cổng dịch chuyển. Điểm xuất hiện ở map đích luôn nằm phía trong mép, bên ngoài vùng trigger trả về để chống hiện tượng giật chuyển map liên tục (ping-pong transition).
   - `SpecialGate`: Cổng đặc biệt đòi hỏi tương tác xác thực bằng phím (Huyền Môn Q11 cần đủ 3 Mảnh Ấn; Lôi Đài cần giao kèo thách đấu).
   - `SafeAnchor`: Mỗi map có một tọa độ an toàn cố định. Khi mất kết nối hoặc máy chủ khởi động lại, người chơi sẽ xuất hiện tại SafeAnchor của map đó.

---

**Fixed monster ladder — canonical:** 7 identities, 6 base sprite/AI rigs; Sói Trúc Ảnh reuse Sói Sương bằng palette. Hai Sói xám lạnh/lục tối có nameplate riêng; Linh dùng aura tím/ấn sáng chung, không dùng màu sói làm dấu Linh.

| Mob identity | Lv | HP | ATK | EXP | Gold | Visual / AI reuse |
| --- | ---: | ---: | ---: | ---: | ---: | --- |
| Nấm Linh | 2 | 48 | 9 | 15 | 7–12 | Base rig |
| Sói Sương | 4 | 107 | 13 | 22 | 11–18 | Base rig |
| Sói Trúc Ảnh | 8 | 339 | 22 | 38 | 19–30 | Sói rig; palette lục tối |
| Ong Giáp | 10 | 473 | 27 | 47 | 23–36 | Base rig |
| Đoạt Mạch Đạo Tặc | 13 | 704 | 36 | 63 | 29–45 | Base rig |
| Xích Thạch Linh | 16 | 974 | 45 | 81 | 35–54 | Base rig |
| Cổ Môn Vệ Binh | 20 | 1393 | 60 | 108 | 43–66 | Base rig |

Level là nhận diện nội dung; không tạo thêm variant chỉ để mỗi level có một quái. Số bảng derive từ formula dưới; `MobDefinition.fixedLevel` và manifest phải khớp.

### Kế hoạch mật độ và phân bổ bãi quái hiện hành — CURRENT / TUNABLE AUTHORING PLAN

Dưới đây là kế hoạch phân bổ bãi quái (pockets) cho 5 bản đồ farm nhằm phục vụ việc authoring màn chơi (level design blockout) và kiểm thử hiệu năng. Các mốc số là **PROBE BASELINE / TUNABLE RANGE**, không phải trần cố định.

| Map farm | Ý đồ bãi và phân bố không gian | Số cụm dự kiến (Pockets) | Số quái hoạt động (Active Mob Budget) | Quy mô mỗi cụm (Group Size) | Phân bố tầng / nhánh | Stable authored IDs / Quest anchors cần bảo toàn | Điểm an toàn & Ranh giới cách ly |
| --- | --- | ---: | ---: | ---: | --- | --- | --- |
| **Đồng Sương** (Lv 1–5) | Onboarding, đồi thấp, nương bậc và bìa rừng. Nhiều bãi nhỏ, không gian mở, tránh áp lực dồn dập. | 8–10 cụm | 14–20 quái | 1–2 quái / cụm | Tuyến dưới/ven suối: Nấm Linh (Lv 2). Đồi bậc giữa và thềm đông: Sói Sương (Lv 4). | `DS1` (Nấm Lv2), `DS2` (Q4 Nấm Linh), `DS3–DS6` (Q5 Da Sói) | Dải vào an toàn 6–8 u từ Vân Khê; các bãi Sói cách biệt đường về làng. |
| **Trúc Ảnh** (Lv 5–10) | Rừng trúc rậm rạp, cầu gỗ, thềm đá cao thấp, kiểm tra di chuyển bậc và đánh quái theo nhóm. | 9–11 cụm | 20–28 quái | 2–3 quái / cụm | Tuyến dưới ven suối: Sói Sương (Lv 4). Bãi trúc trung tâm & quanh ấn: Sói Trúc Ảnh (Lv 8). Tuyến cầu trên cao & vách đá: Ong Giáp (Lv 10). | `TA1–TA3` (Sói Sương), `TA4` (với `TA4.slot1` giữ cho Q8 Linh Biến), `TA5` (Ong Giáp), `TA6` (Sói Trúc Ảnh) | Vùng an toàn 6–8 u tại cửa Đồng Sương và cầu nối sang Bạch Vân. |
| **Bạch Vân** (Lv 8–13) | Vách đá dựng đứng, thác nước, thềm đá bậc liên tục, mỏm cụt và đường vòng. Cung phát huy tầm xa, Kiếm gom góc hẹp. | 8–10 cụm | 20–28 quái | 2–3 quái / cụm | Thềm trên cao quanh thác: Ong Giáp (Lv 10). Các terrace đá bậc giữa, hốc hang và lối đèo: Đoạt Mạch Đạo Tặc (Lv 13). | `BV1`, `BV2` (Ong Giáp Lv 10), `BV3–BV5` (farm/progression Đoạt Mạch Đạo Tặc Lv 13) | Thềm nghỉ an toàn 6–8 u đầu đèo và trước cửa sang Xích Nham. |
| **Xích Nham** (Lv 12–17) | Mỏ khoáng cằn cỗi, đất đá đỏ, khe nứt sâu và ba khu vực trấn ấn. Quái trâu, áp lực chiến đấu tăng cao. | 9–11 cụm | 24–32 quái | 2–4 quái / cụm | Vành đai ngoại vi và lối vào: Đạo Tặc (Lv 13). Hốc nứt mạch sâu và 3 khu trấn ấn: Xích Thạch Linh (Lv 16). | `XN1–XN3` (Q10 Đạo Tặc Lv 13), `XN4–XN6` (Q11 Xích Thạch Linh Lv 16 tại 3 phong ấn) | Vùng an toàn 8 u cửa ngõ vào và hành lang dẫn đến cổng Huyền Môn. |
| **Huyền Tích** (Lv 17–20) | Phế tích cổ, đền thờ phong ấn, hành lang đá nguyên khối. Tách bạch hoàn toàn quái thường và khu vực Boss. | 8–10 cụm thường + 1 Boss | 20–26 quái thường + 1 Boss | 2–3 quái / cụm | Tiền môn / ngoài cổng: Xích Thạch Linh (Lv 16). Hành lang / nội điện: Cổ Môn Vệ Binh (Lv 20). Trung điện (BossCombatArea): Boss độc lập. | `HT1` (Thạch Linh), `HT2`, `HT3` (Vệ Binh), `HT4`, `HT5` (Q12 Cổ Môn Vệ Binh Lv 20), `HT_BossLandmark` (Q12 Boss) | **Boss Exclusion:** Tuyệt đối cấm quái thường trong BossCombatArea. Vùng vào Huyền Môn an toàn 8 u. |

**Bảng danh mục bãi quái hiện hành (Candidate Manifest — PROBE / TUNABLE):**
Các ID cụm mới (`DS7+`, `TA7+`, `BV6+`, `XN7+`, `HT6+`) là các mã định danh authoring ổn định, không dùng chỉ số thực thể sống (live instance index). Level cố định theo loài quái vật (`fixedLevel`), không ngẫu nhiên hóa cấp độ trong cùng loài.

| Nhóm cụm / Map | Cụm ID | Mob identity | Level | Slots dự kiến | Vai trò / Ghi chú bố cục |
| --- | --- | --- | ---: | ---: | --- |
| **Đồng Sương** | `DS1` | Nấm Linh | 2 | 1 | Stable seed: Nấm khởi đầu ven đường |
| | `DS2` | Nấm Linh | 2 | 1 | Quest anchor: Q4 Nấm Sương tutorial |
| | `DS3`–`DS6` | Sói Sương | 4 | 2 mỗi cụm (8) | Quest anchors: Q5 Da Sói (4 cụm đồi cỏ bậc giữa) |
| | `DS7`, `DS8` | Nấm Linh | 2 | 2 mỗi cụm (4) | Bổ sung: Dải nương thấp và bờ suối phía nam |
| | `DS9`, `DS10` | Sói Sương | 4 | 2 mỗi cụm (4) | Bổ sung: Gờ đồi phía đông và lối rẽ lên Trúc Ảnh |
| **Trúc Ảnh** | `TA1`–`TA3` | Sói Sương | 4 | 2 mỗi cụm (6) | Stable seeds: Bìa rừng trúc giáp ranh Đồng Sương |
| | `TA4` | Sói Trúc Ảnh | 8 | 2 | Quest anchor: `TA4.slot1` cố định cho Q8 Linh Biến |
| | `TA5` | Ong Giáp | 10 | 3 | Stable seed: Nhịp cầu gỗ trên cao |
| | `TA6` | Sói Trúc Ảnh | 8 | 2 | Quest anchor: Bãi trúc quanh trụ trấn ấn nứt |
| | `TA7`, `TA8` | Sói Trúc Ảnh | 8 | 2 mỗi cụm (4) | Bổ sung: Tuyến rừng trúc trũng và khe đá phụ |
| | `TA9`, `TA10` | Ong Giáp | 10 | 2–3 mỗi cụm (5) | Bổ sung: Mỏm đá cao nhìn ra vực và giàn ván bắc qua đèo |
| **Bạch Vân** | `BV1`, `BV2` | Ong Giáp | 10 | 2 mỗi cụm (4) | Stable seeds: Vùng trời thềm thác nước phía tây |
| | `BV3`–`BV5` | Đoạt Mạch Đạo Tặc | 13 | 3 mỗi cụm (9) | Stable seeds: Ba thềm đá bậc giữa đường đèo (farm/progression, không phải Q10 anchor) |
| | `BV6` | Ong Giáp | 10 | 2 | Bổ sung: Thềm đá gần đỉnh thác đổ |
| | `BV7`, `BV8` | Đoạt Mạch Đạo Tặc | 13 | 2–3 mỗi cụm (5) | Bổ sung: Hốc đá cụt phía bắc và đường vòng chân vách |
| **Xích Nham** | `XN1`–`XN3` | Đoạt Mạch Đạo Tặc | 13 | 2/3/2 (7) | Quest anchors: Q10 Vật Chứng (khu mỏ ngoại vi) |
| | `XN4`–`XN6` | Xích Thạch Linh | 16 | 3/3/4 (10) | Quest anchors: Q11 Mảnh Ấn (ba cụm trấn ấn A/B/C) |
| | `XN7` | Đoạt Mạch Đạo Tặc | 13 | 2 | Bổ sung: Ngách đá hẹp phía tây |
| | `XN8`, `XN9` | Xích Thạch Linh | 16 | 3 mỗi cụm (6) | Bổ sung: Thềm đá nứt mạch ngầm và lối bậc đá dẫn vào phế tích |
| **Huyền Tích** | `HT1` | Xích Thạch Linh | 16 | 2 | Stable seed: Tiền môn phế tích |
| | `HT2`, `HT3` | Cổ Môn Vệ Binh | 20 | 3/2 (5) | Stable seeds: Hành lang ngoài và cầu thang đá dẫn vào cấm điện |
| | `HT4`, `HT5` | Cổ Môn Vệ Binh | 20 | 3 mỗi cụm (6) | Quest anchors: Q12 Vệ Binh trước cửa Boss |
| | `HT6` | Xích Thạch Linh | 16 | 2 | Bổ sung: Ngách phế tích phía đông |
| | `HT7`, `HT8` | Cổ Môn Vệ Binh | 20 | 2–3 mỗi cụm (5) | Bổ sung: Cánh tả và cánh hữu sảnh tế lễ |
| | `HT_Boss` | Huyền Nham Cự Thú | 20 | 1 | Khu vực Boss độc lập (`BossCombatArea`), cấm quái thường |

> Candidate manifest trên là **initial blockout candidate**, không phải danh sách đóng kín. Có thể thêm `BV9+`, `XN10+`, `HT9+` nếu playtest cần đạt phần trên của range (ví dụ 10 cụm thay vì 8). Tổng số cụm cuối cùng và Hybrid count/identity vẫn OPEN/TUNABLE.

---


<a id="terrain-rules"></a>

**LOCKED địa hình:** đất/đá tự nhiên là **khối solid có độ dày**, fill, mặt trên ngang, mặt đứng, đáy/bóng và góc khép. Đồi/núi xây bằng khối bậc/terrace liên tục; có thể có hốc, hang, khe dọc và mỏm nhưng không giả núi bằng dải tự nhiên mỏng nổi. Mặt chơi chỉ ngang hoặc đứng: không slope/ramp/triangle, collider đứng được xoay hay mặt chéo. Mái/cành/núi nền có thể vẽ chéo; collision chơi vẫn trực giao.

**One-way hiếm, chỉ cho cấu trúc hợp lý:** ván mỏng, giàn/catwalk, ban công nhẹ, sàn tạm hoặc sàn treo/tựa vách, có dây/dầm/cột/bracket đỡ rõ. Đất/đá tự nhiên không one-way. Solid phải dày/khép; one-way mỏng/có khoảng trống dưới; background/decor tương phản thấp và không giả mặt đứng được. Vật trông như cầu thang/sàn phải chơi được đúng hình hoặc đổi hình đủ rõ. DropThrough chỉ qua one-way đang đứng, không xuyên solid và không chain sàn khi giữ nút.

Kit công trình tái dùng cột, dầm, sàn, cầu, ban công, tường, mái, vòm/cổng và block step; landmark chỉ cần vài mặt collision sạch. Không procedural building, không polygon collider bám toàn silhouette. Mái làm route phải có mặt ngang/bậc riêng. **Không ladder, rope/vine/pole/wall climb, Climb action/state/animation.** Di chuyển vẫn Move/Jump/Fall/DropThrough; cầu thang là khối bậc trực giao hoặc decor rõ.

**HomeRegion / WalkRegion:** HomeRegion là vùng hoạt động gốc của cụm; WalkRegion/SurfaceId là địa hình quái được phép đi, có HomeSpan khi cần. Ground mob tuần tra trong vùng đã author, gặp mép không có nền nối thì quay đầu, không tự rơi/nhảy/drop hoặc tìm đường nhiều tầng. Hai bậc chỉ nối được với AI nếu có đường đất trực giao liên tục thật; route player phải jump/drop không tự là route quái. Passive aggro ưu tiên mục tiêu local có thể tới được.

Bị đánh ngoài passive aggro vẫn tạo threat/wake; báo động chỉ cùng SpawnGroup, không lan recursively sang group bên cạnh.

**Return:** mục tiêu unreachable hoặc ra khỏi leash sau grace thì quái về home theo policy; ưu tiên xét threat khác có đường hợp lệ trước. Cung kite bằng liên tục đổi vị trí trên route hợp lệ có thể no-hit pure melee, đó là lợi thế class. Safe perch không tới được mà đứng spam mãi là lỗi geometry; ưu tiên sửa authoring và Return đơn giản, không cho Sói ranged fallback/teleport/jump tầng để cân Cung.

Exact grace, tốc Return, regen/invulnerability/targetability còn OPEN/TUNABLE; không miễn damage tức thì chỉ vì player nhảy. Focus có thể giữ Returning target để quan sát. Return/reset không loot, reroll hay tạo life mới; clear ledger/status theo policy encounter hiện hành, chưa khóa cách hồi HP theo thời gian.

**Nước nông:** lòng nước nông có đáy đất thật, người chơi vẫn đi được và giảm nhẹ tốc chạy khi chân chạm nước. Qua cầu hoặc ở trên không không nhận giảm tốc nước. Nước rộng có cầu và đường đi hợp lệ; không thêm swimming, drowning hoặc fluid physics P0. Hình nước/thác không tự quyết collision hay sát thương. Hệ số/depth cần playtest trước production; thông số bản mẫu thuộc [Roadmap](../90-archive/production-history.md#prototype-visual-review).


Farm bãi gần cấp trong khoảng thưởng các mục liên quan; quái cao cấp hơn vẫn có thưởng nếu trong khoảng, nhưng không bảo đảm an toàn. Identity/cấp quái quyết định bậc đồ rơi; map quyết định nguyên liệu và sắc thái. **[Farm Matrix Lv 1–20](../04-production/playtest-and-balance.md#farm-progression)** giữ derived HP / EXP / Gold, không copy stats table vào design owner. Measure solo / 2 / 3 / 4 players, wait / crowd / CPU / network và run-back trước capacity claim; low-level pockets ít slot có thể cần rotate, không thêm Party / Channel.

## Chiến đấu và vòng đời quái

| Normal archetype | AI | Move u / s | Melee / ranged u | Interval | Projectile |
| --- | --- | ---: | --- | ---: | --- |
| Nấm Linh | Melee only | 1,2 | 0,8 / — | 1,8 s | — |
| Sói Sương / Sói Trúc Ảnh | Melee only; chase nhanh | 2,4 | 1,0 / — | 1,3 s | — |
| Ong Giáp | Ranged / Flying | 2,0 | — / 5 | 1,6 s | Visual generic, speed 5,5 u / s |
| Đoạt Mạch Đạo Tặc | Nền GroundMelee; cấu hình Hybrid còn OPEN | 2,2 | 1,2 / 5 | 1,5 s | Visual generic, speed 5 u / s |
| Xích Thạch Linh | Nền GroundMelee; cấu hình Hybrid còn OPEN | 1,4 | 1,1 / 4,5 | 2,0 s | Visual generic, speed 4 u / s |
| Cổ Môn Vệ Binh | Nền GroundMelee; cấu hình Hybrid còn OPEN | 1,8 | 1,4 / 6 | 1,6 s | Visual generic, speed 6 u / s |

**Số lượng/identity Hybrid còn OPEN**, không khóa ba Hybrid hoặc 3/1/0. Ba dòng đánh xa trên chỉ giữ tham số candidate của baseline cũ, không yêu cầu triển khai đánh xa cho cả ba. Nếu chọn Hybrid, dùng capability GroundRanged/Hybrid tái sử dụng và đặt điều kiện cận chiến/đánh xa rõ; không cho mọi GroundMelee một ranged fallback. AI dùng profile/config chung như GroundMelee, FlyingRanged và capability Boss, không behavior riêng theo tên loài.

FlyingBox của Ong ~6 × 3 u là phạm vi bay, không phải độ cao lơ lửng cố định. Ong phải tiếp cận vào vùng cận chiến theo chiều dọc khi giao tranh, không treo mãi ngoài tầm Kiếm; thời gian bay/tiếp cận thử tại PHY-01/ART-01. Một Linh Đạn dùng chung đổi scale/tint/speed/trail; không họ projectile hoặc animation projectile riêng theo loài. Pose ra đòn của actor chỉ bổ sung khi capability được chọn, theo accounting có điều kiện tại Art.

Stat normal tại level L (`round(x) = floor(x + 0.5)` cho x ≥ 0, kể cả EXP / final Damage): `HP = round(40 + 1.04*L*L*L)` khi L ≤ 5; L>5 đặt `x = L - 5`, `HP = round(170 + 50*x + 2.1*x*x)`; `DEF = round(2 + 0.8*L)`; `ATK = round(6 + 1.5*L + 0.06*L*L)`; `ACC = 60 + 4*L`; `EVA = 20 + 2*L`; `NormalEXP = round(10 + 2.5*L + 0.12*L*L)`; `GoldMin = 3 + 2*L`; `GoldMax = 6 + 3*L` (integer uniform inclusive). ATK mid/late tăng để bù một phần HP player tăng theo cấp;

HP mob giữ theo TTK probe. Một curve chung, chỉ evaluate ở bảy identity-levels đã author; hp / atk multipliers mặc định 1.0, chỉ tune có evidence sau playtest. Normal / variant slot respawn **25 s BASELINE / TUNABLE**, test 20 / 25 / 30 s, tính từ death; không timer variant riêng. Mục tiêu vòng bãi: clear A → nhặt → B / C / D → quay lại, không đứng nguyên một pocket đợi respawn.

TTK target cùng level + Common + 0: early 2–4 s, mid 3–6 s, late 4–8 s TEST, không guarantee mọi build. Đánh Thạch Lv 16 khi player15 trước đại chiêu còn là probe chậm; giữ HP curve, không tăng mọi HP chỉ để kéo giờ chơi. Các phép thử solo, 2–4 mục tiêu, Q8 Linh và đồ chậm hơn mốc cấp được quản lý tại Playtest & Balance.

**Mob attack contract:** đi qua aggro radius vẫn bị acquire / chase, body overlap không gây damage. Melee: Acquire → Chase → attack range → Face → Windup / lock facing → HitMoment / front hitbox → Recovery / reposition ngắn khi có chỗ hợp lệ → tiếp cận lại. Target chạy xuyên ra sau / nhảy ra khỏi vertical range / rời hitbox trước HitMoment thì MISS; không guaranteed damage vì animation đã start, không quay 180° giữa swing.

Ranged / Hybrid: Acquire → Aim / Windup → resolveMoment → authority logical target resolve → result → visual projectile; AnimationEvent chỉ visual. Mob normal attack power 1.0, CritChance 0 P0; formula Damage chung, exact hitbox / windup tại PHY-01 / ART-01.

<a id="melee-crowd"></a>

**Melee crowd — flow tái sử dụng:** `Approach → Contact hoặc Staging → Attack → Recovery/Reposition`. `Staging` là vị trí chờ gần tầm đánh: quái sau phải chờ/chỉnh bước có lý do đọc được, không trông như bị đồng đội chắn tường. **Occupied != blocked:** chỗ đã có quái không đồng nghĩa terrain wall. Không body blocking Player–Mob/Mob–Mob; chỉ separation nhẹ cho hình dễ đọc.

- Quái trước có thể nhường điểm tiếp xúc trong recovery; quái sau tiến/chỉnh vị trí hợp lệ. Không teleport, rear melee không tự thành ranged.
- Ưu tiên chỗ trái/phải trên WalkRegion, hòa dùng ID ổn định; deadband và thời gian hạn chế đổi phía 1,5 s là BASELINE/TUNABLE. Không đảo phía liên tục sau mỗi hit.
- Không formation cứng, bốn slot cố định, vòng tròn hoặc group attack token. Recovery/reposition dùng interval hiện có, không thêm cửa miễn đòn hoặc tăng HP/range/tốc độ để ép metric.
- Origin/facing lúc windup/hit không bị steering thay đổi; phase lệch nhau theo life. N-player probe phải kiểm nhiều target và cụm trong cùng camera.

**Mob / Linh Biến threat:** một `Threat[playerId]` và `Contribution[playerId]` riêng mỗi mob. Initial acquire nearest valid player hoặc attacker đầu tiên; direct / DoT cộng ActualHpLost (cap overkill, dedup), không raw damage. Sticky target: challenger có threat>0 và ≥ 1,25 × current mới đổi; current invalid / dead / disconnect / khác MapId / out-of-leash thì chọn highest valid threat, tie playerId; nếu không có threat chọn nearest valid trong aggro.

Báo động cụm chỉ wake, từng mob tự acquire / resolve. Return kết thúc encounter cũ, clear threat/contribution/status và hủy action chưa resolve; không giữ damage từ lượt kéo trước. Đích reset là HP đầy ở home; exact cách hồi/regen, grace và invulnerability khi về còn OPEN/TUNABLE, không coi “đầy ở home” là khóa hồi tức thì lúc bắt đầu Return. Linh Biến dùng cùng resolver, không nearest-only sau acquire.

**Linh Biến P0 — modifier trên normal slot:** dynamic roll `LinhBienChance = 0.05` (**5% TEST / TUNABLE**) chỉ tại spawn / respawn của mob **Lv 8+**, cap `MaxRandomActiveLinhBienPerMapId = 1`. Lv 1–7 không roll, không tiêu RNG rồi upgrade level; initial population dùng cùng arbitration. Return / root hide / reconnect không reroll. Khi chết, slot dùng deadline 25 s như normal; lần spawn sau mới xét variant.

| Modifier | BASELINE / TUNABLE |
| --- | --- |
| Stat | HP × 5; ATK × 1,3; DEF / ACC / EVA / MoveSpeed giữ base |
| Reward | EXP × 3; Gold × 3; physical loot theo Linh Biến profile các mục liên quan, không nhân EXP / Gold × 5 |
| Visual / count | Same sprite / animation / AI / projectile / threat; scale ~1,20–1,30, aura / name / HP bar; chiếm một normal slot, không thêm mob identity / rig |

<a id="q8-bounded-path"></a>

**Q8 force encounter — LOCKED no-softlock:** giữ `TA4.slot1` và một encounter shared. Random Linh cap vẫn áp cho random spawning; khi có requester hợp lệ, dành **một reservation Q8 chung cho map**, độc lập Linh unrelated đang giữ random cap. Đây là ngoại lệ quest cố định, không cap tăng theo N. Existing Linh ở target slot thì bind đúng life; Linh khác sống nguyên lifecycle, không demote/despawn/reset giữa combat. Suppress random promotions mới trong thời gian reservation hoạt động.

Tại target slot, chỉ promote normal idle/fullHP ở spawn hoặc next valid lifecycle boundary. Nếu target đang combat thì tiếp tục life đó, không ép reset; requester có thể kết thúc encounter hoặc chờ Return/respawn theo luật. Không phải chờ ai đó tình cờ giết Linh ở nơi khác. Reservation được xử lý tại mỗi due boundary, priority trước random roll; bounded scheduler/retry không phụ thuộc RNG. Exact timeout/promotion budget cần Q8-01 probe; guarantee là đường chủ động tiến triển, không hứa tự complete cho người AFK.

N requesters đăng ký idempotently theo character + active step; waiting order ổn định và có tuổi, không để người mới liên tục vượt người chưa credit. Cùng life có thể share encounter. Death snapshot dùng quest predicates riêng; đủ credit bỏ request, thiếu giữ vị trí ưu tiên cho valid lifecycle retry. Rời map bỏ hiện diện nhưng giữ progress/entitlement; reconnect không tạo force quyền mới. Không private entity hoặc world slot theo từng player.

**Economic guard:** force/retry không được là nút reroll Rare. Reservation có force entitlement bền theo active quest step và economic receipt; một budget Linh cố định được roll/commit một lần cho shared reservation, không roll reward lại do thiếu quest credit, Return, disconnect hoặc replay. Retry phục vụ credit, không tạo budget EXP/Gold/regular loot/Journey Linh mới. Không nhân pile theo N.

Binding vào random Linh đang sống dùng death/reward hiện hành của life đó, không cộng thêm quest pile. Exact entitlement grouping/recovery là Q8-01/G-D gate, không claim queue RAM tự bảo đảm crash-safe.

**Credit review:** bảng objectives giữ ngưỡng hiện tại tại [Quests & Narrative](quests-and-narrative.md#q8-credit-review). Tối đa năm người đạt ngưỡng trong một full-health life nên không hứa mọi N complete cùng death. Fair retries phải có probe đông người/outsider contention. Nếu còn starvation thì gate không pass; proposal credit participation riêng bên quest owner cần duyệt/đo trước thay threshold, không tự lấy 10%.



**Một World Boss chung:** Lv 20 Huyền Nham Cự Thú, HP **32.000**, ATK **160**, DEF **25**, ACC **140**, EVA **60** — BASELINE. Ngay trong Huyền Tích, không scene / story instance / cổng arena / gate Lv 20 riêng. Q11 hoàn thành mở toàn map; người đi ngang thấy trận đánh. Landmark có khoảng trống đọc vùng báo trước đòn, BossCombatArea radius 15 u TEST; không đặt quái thường trong vùng đánh Boss.

World mới Boss có sẵn; sống thì không spawn thêm; chết đặt nextSpawn = deathUtc + 15 phút; tới deadline chỉ spawn nếu không có entity. Reset khi wipe không coi là death, không roll loot / đổi deadline. Demo Boss có sẵn, respawn 60 s override rõ; release target 90–150 s trong benchmark đầu với hai endgame players, không auto-scale HP theo số người hoặc giới hạn world ở hai người.

| Pattern | Power | Telegraph | CD / interval | Shape |
| --- | ---: | ---: | ---: | --- |
| Basic | 1,00 × | — | 1,8 s | Melee |
| Nham Trảo | 1,20 × | 0,5 s | 2,5 s | Frontal cone |
| Địa Chấn | 1,50 × | 1,0 s | 6 s | Ground AoE, nhảy né |
| Nham Thạch Rơi | 1,80 × | 1,2 s | 8 s | Ba vùng đất, đặt quanh tối đa ba vị trí người chơi hợp lệ; mỗi người chỉ chịu tối đa một vùng trong một lần tung |

Boss chỉ bắt đầu **một action tại một thời điểm**: chọn kỹ năng đã hết hồi chiêu theo thứ tự Địa Chấn → Nham Thạch Rơi → Nham Trảo, nếu chưa có thì đánh thường. Hồi chiêu tính từ lúc bắt đầu action; vùng báo đòn đã phát không bị action mới chen vào. Ba vùng Nham Thạch Rơi không chồng hitbox gây double-hit; khi chỉ có một/hai mục tiêu, vẫn ba vùng tách nhau theo anchor quanh vị trí hợp lệ, không nhắm một người ba lần. Cuồng Mạch đổi nhịp action **sau** action hiện tại;

Băng Hàn chỉ giảm tốc đồng hồ chờ action tiếp theo. Độ rộng vùng, khoảng cách né và nhịp telegraph cần kiểm trên scene thật, không đổi quy tắc này khi chưa có evidence.

Boss threat = ActualHpLost, highest alive valid trong BossCombatArea; retarget invalid hoặc khoảng 1 s, không dùng sticky normal multiplier. Không ai alive trong area liên tục 10 s → reset HP / position / threat / contribution / status / phase. Corpse không ngăn reset. **Cuồng Mạch P0** một lần khi HP ≤ 30%: tint / glow / roar / local shake, future cadence × 0,8 TEST (CD / interval giảm 20%), không tăng ATK / cắt telegraph / reschedule action đang chạy. Linh Giáp / Vỡ Thế P1.

Boss eligibility / corpse / EXP / Gold / pile / Journey theo **các mục liên quan reward contract**; Q12 dùng active-step damage (các mục liên quan), không personal set. Damage clear khi reset. Sau chính tuyến: Huyền Tích / Linh → Rare / Epic → enhance → shared Boss / Dư Ảnh → build / PvP; không daily / dungeon mới.

> **Implementation:** [Technical — timer và Boss lifecycle](../02-technical/online-and-persistence.md#timers)

---


### Dependency thiết kế: input, density, terrain và quest

Tách chọn/thực thi cho phép chọn S2 trước rồi Execute theo ý định rõ, một intent mỗi lần bấm; không bắt chọn lại S2 mỗi đòn. Bỏ letter keys khỏi movement để thử Execute/Interact/dùng đồ/menu RPG; **phím chính xác vẫn OPEN**. AUTO tìm cục bộ theo kỹ năng/khả năng tới đích, click ghim đích, Tab đổi cục bộ theo thứ tự ổn định; vùng tìm/giữ/thực thi tách nhau. Chết hủy lệnh chờ nhưng giữ focus hợp lệ để xem trận cùng farm và HP thay đổi; focus đời cũ không tự gắn quái respawn.

**Phân tích mật độ — Tăng số pocket thay vì gom blob:**
- *Gameplay & Combat flow:* Tăng số bãi nhỏ độc lập (1–3 quái/cụm) trên các tuyến/thềm địa hình thay vì gom 8–10 quái thành một khối dồn cục (`blob`). Gom blob sẽ phá vỡ cơ chế xếp hàng tiếp cận (`crowd staging`), khiến quái chồng lấn khó đọc, đồng thời tạo ra lượng sát thương dồn tức thời quá lớn khiến người chơi không thể phản ứng. Bố trí nhiều cụm nhỏ giúp duy trì nhịp độ ra đòn nhanh của S1/S2 và tạo điều kiện cho Kiếm tận dụng sát thương quét 3 mục tiêu hoặc Cung tận dụng tầm xa/Spread để xử lý nhiều mục tiêu.
- *Online co-farm & Contention:* Trong môi trường trực tuyến N-người chơi, nhiều cụm độc lập (ví dụ Đồng Sương 8–10 cụm, Trúc Ảnh 9–11 cụm, Bạch Vân 8–10 cụm, Xích Nham 9–11 cụm, Huyền Tích 8–10 cụm) cho phép các nhóm người chơi chia nhau các khu vực farm mà không bị nghẽn (bottleneck). Đồng thời giảm nguy cơ quái bị hạ gục quá nhanh trước khi người chơi kịp đạt ngưỡng đóng góp 20% máu để nhận tín chỉ nhiệm vụ.
- *Thị giác camera tiêu chuẩn:* Bố trí theo tầng (dưới 1–2 quái, giữa 1–3 quái, trên 1–2 quái, nhánh phụ 1–2 quái) giúp một khung hình camera có thể bao quát 5–8+ quái cùng lúc, tạo cảm giác thế giới hoang sơ, nguy hiểm và đông đúc, nhưng aggro vẫn được giữ độc lập nhờ ranh giới `HomeRegion` và `WalkRegion` riêng biệt.
- *Tải CPU / Physics / Network:* Phân tán quái theo các `SpawnGroup` nhỏ độc lập giúp AI server dễ dàng đưa các cụm không có người chơi vào trạng thái ngủ (dormant). Do địa hình hoàn toàn trực giao, không có dốc (`no slope`) và không có leo trèo (`no climb/ladder`), quái chỉ tuần tra trên mặt phẳng ngang của vùng đi lại được chỉ định, triệt tiêu hoàn toàn chi phí tìm đường nhiều tầng (multi-floor pathfinding).
- *Boss Exclusion Rule:* Tại Huyền Tích, việc loại trừ tuyệt đối quái thường khỏi `BossCombatArea` bảo vệ tính toàn vẹn của cuộc chiến với Boss Huyền Nham Cự Thú, ngăn chặn các trường hợp quái thường quấy rối telegraph hoặc bị lợi dụng để farm hồi phục/tích nộ ngoài ý muốn.
- *Dữ liệu lịch sử vs Kế hoạch hiện hành:* Mốc 28 cụm / 66 slots và khoảng cách tâm 18–20 u là seed lịch sử để đối chiếu prototype; kế hoạch authoring hiện hành đưa ra các khoảng ngân sách mục tiêu (14–20 ở Đồng Sương, 20–28 ở Trúc Ảnh/Bạch Vân, 24–32 ở Xích Nham, 20–26 ở Huyền Tích). Các mã authored ID mới (`DS7+`, `TA7+`, `BV6+`, `XN7+`, `HT6+`) mở rộng số cụm mà không làm xáo trộn các Quest Anchor IDs gốc (`DS2`, `DS3–DS6`, `TA4`, `TA6`, `TA4.slot1`, `XN1–XN6`, `HT4–HT5`, `HT_BossLandmark`) cũng như các stable authored seed IDs (`TA5`, `BV1–BV5`, v.v.).

**Phân tích thiết kế địa hình trực giao (Orthogonal Terrain Rationale):**
- *Tính toán hình học chiến đấu tất định:* Việc khóa địa hình tự nhiên thành khối đặc dày (`Solid Mass`), chỉ gồm mặt phẳng ngang và mặt đứng trực giao (loại bỏ hoàn toàn dốc nghiêng `slope/ramp/triangle`) là yêu cầu cốt lõi để đảm bảo sự chuẩn xác của hệ thống chiến đấu 2D authoritative. Mọi hình dạng kiểm tra sát thương (Melee single 1,7 u, Arc 120°, Line 5,5 u rộng 0,6 u, Logical single/Spread 6,5 u, Explosion bán kính 2 u) đều tính toán theo trục tọa độ trực giao. Dốc nghiêng sẽ làm lệch góc xoay hitbox, dẫn đến việc đòn đánh bị trượt hoặc xuyên thấu kỳ dị giữa Client và Dedicated Server. Mặt phẳng ngang đảm bảo việc kiểm tra chồng lấp dọc (`vertical overlap`) và va chạm hitbox–hurtbox tại `HitMoment` luôn mang tính tất định (deterministic).
- *Bảo vệ tính toàn vẹn của AI quái vật:* Quái vật mặt đất (Sói, Nấm, Đạo Tặc, Thạch Linh, Cổ Vệ) không có logic leo trèo phức tạp. Nếu map có thang dây hay cơ chế leo (`ladder/climb`), người chơi chỉ cần đu trên thang hoặc đứng trên vách hẹp bắn tỉa quái bên dưới mà quái không thể phản ứng, biến toàn bộ bãi quái thành bia tập bắn vô dụng. Việc triệt tiêu thang leo và thay bằng các khối bậc nhảy trực giao (`stepped blocks`) bảo đảm mọi cao độ đều được quy về các phép kiểm tra tiếp cận (`reachability evaluation`) rõ ràng.
- *Quy tắc sàn One-way khắt khe:* Đất đá tự nhiên không bao giờ là one-way platform để tránh cảm giác phi lý (đất đá không thể nhảy xuyên từ dưới lên). Sàn one-way chỉ dành cho kết cấu mỏng nhân tạo có trụ/dầm/dây treo (ván gỗ, giàn catwalk, ban công, sàn treo). Người chơi bấm `↓` (`DropThrough`) chỉ xuyên qua một tầng sàn, không xâu chuỗi nhiều sàn khi giữ nút.
- *Vùng nước nông (Shallow Water):* Lòng suối cạn có đáy đất thật, người chơi lội qua bị giảm tốc nhẹ khi chân chạm nước, nhưng khi đi trên cầu gỗ hoặc nhảy trên không thì giữ nguyên tốc độ. Không bổ sung cơ chế bơi lội hay đuối nước để tránh phình to phạm vi animation, trạng thái và vật lý.

**Kiting hợp lệ vs Lỗi góc chết (Legitimate Kiting vs Safe Perch Exploit):**
- *Bản sắc class của Cung thủ:* Cung thủ sở hữu tầm đánh xa 6,5 u và độ cơ động cao. Việc liên tục di chuyển lùi bước, vừa chạy vừa bắn và nhảy qua lại giữa các thềm đá trên cùng một tuyến đường đi lại được (`reachable path`) để tránh né đòn đánh cận chiến của quái là kỹ năng thả diều hoàn toàn hợp lệ và là lợi thế tự nhiên của phái đánh xa (`class advantage`).
- *Xử lý lỗi góc chết bằng cơ chế Return đơn giản:* Nếu người chơi nhảy lên một mỏm đá cụt hoặc thềm cao mà quái vật không có đường tiếp cận hợp lệ (unreachable), hoặc người chơi chạy vượt quá giới hạn truy đuổi (`LeashRegion`), quái vật sẽ kích hoạt trạng thái `Return` rút về vị trí xuất phát (`HomeRegion`), đích reset tại home là full HP (cách hồi trong Return còn OPEN), kết thúc giao tranh. Cách xử lý này giải quyết triệt để vấn đề người chơi lợi dụng lỗi địa hình để farm quái an toàn mà không cần phải gượng ép bổ sung đòn đánh xa vô lý cho quái cận chiến (như cho Sói bắn đạn) hay teleport gian lận.

<a id="a07"></a>

## A07 — quyết định liên quan

**A07 Dummy** — HP 60/25 s: rẻ nhưng chờ; pool thêm/respawn nhanh: ít chờ; HP lớn/scaling: test dài hơn nhưng phức tạp hoặc hại Q3 · Một prefab/yard, tối thiểu 3 Dummy cùng lúc theo Q3; số thêm và respawn 3–5 s chỉ đề xuất, không DPS Meter/scaling P0 · P07; đổi HP/timer hoặc số thêm phải theo design owner

**A07** — ART-01 / QUEST-03 / SCOPE-01 · BASELINE HP60/25 s Q3; OPEN DEF/EVA, số điểm đứng thêm ngoài tối thiểu3 và timer nhanh. Q3/Q6 solo CURRENT; contention thử ở gate mạng.


<a id="a10"></a>

## A10 — quyết định liên quan

**A10 Tầng/AI/projectile mask** — Vùng đi được/bounds logic; LoS A không lọc vs B SolidWall; Hybrid 3/1/0 là ví dụ lịch sử · Không navigation nhiều tầng/full geometry LoS; exact A/B PROTOTYPE, số/identity Hybrid OPEN · P09; duyệt nội dung trước chọn roster Hybrid, không khóa 3/1/0

**A10** — PHY-01 / ART-01 · APPROVED logical lane/flying bounds; PROTOTYPE LoS A/B; Hybrid count/identity và exact Return/grace/regen/invuln/speed/targetability OPEN. Không tự khóa immune hoặc navigation; terrain/no-slope/no-climb đã LOCKED, không nằm trong options này.


Các hướng Đông/Tây/Bắc/Nam trong ý đồ blockout là nhãn quy hoạch/nhánh, không chỉ định nghiêng camera hoặc thêm depth traversal. Projection luôn theo [pure side-view Art lock](../03-art/art-and-visual-production.md#visual-perspective).
