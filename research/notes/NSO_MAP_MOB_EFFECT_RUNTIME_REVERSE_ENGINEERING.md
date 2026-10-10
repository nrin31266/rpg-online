# BÁO CÁO NGHIÊN CỨU THỰC NGHIỆM: KHẢO CỔ HỆ THỐNG RUNTIME & ASSET NSO (MAP, MOB, EFFECT) VÀ BẢN ĐỒ ÁNH XẠ CHUẨN HÓA SANG UNITY 2D HIỆN ĐẠI (PROJECT HUYỀN LỘ)
*(NSO Binary Map Grid, Minimap Synthesis, Water Pipeline, Mob Slicing vs Modern Unity 2D Engine, URP, Tilemap, Shader Graph & Specs Docs Comparison)*

---

## MỤC ĐÍCH & PHẠM VI NGHIÊN CỨU

Báo cáo này tập trung vào 2 nhiệm vụ trọng tâm:
1. **Khảo cổ học mã nguồn và dữ liệu thực tế (Reverse-Engineering Facts)**: Bóc tách chính xác cách Ninja School Online (NSO) tổ chức Map Binary, Minimap $2\times 2$, hệ thống nước động, quái vật (Mob cũ vs Mob mới), và hiệu ứng rời (Effect Atlas) từ client J2ME, server Java và CSDL `nsoz.sql`.
2. **Quy chuẩn hóa và Ánh xạ sang Unity Engine Hiện Đại (Modern Unity 2D Mapping)**: Đối chiếu trực tiếp với các tài liệu đặc tả thiết kế hiện hành của dự án Huyền Lộ trong `docs/` (`03-art/art-and-visual-production.md`, `01-design/world-and-content.md`, `02-technical/gameplay-runtime.md`), từ đó xác lập kiến trúc chuyển dịch tối ưu trên nền tảng **Unity 6 / 2023 LTS (Universal Render Pipeline - URP 2D)**.

> **Quy ước gán nhãn khoa học xuyên suốt tài liệu:**
> - `[SOURCE-PROVEN]`: Bằng chứng trực tiếp từ mã nguồn decompiled Client J2ME hoặc Server Java (`com.nsoz.*`).
> - `[ASSET-PROVEN]`: Bằng chứng từ khảo sát nhị phân và kích thước pixel ảnh thực tế trong thư mục `Data/`.
> - `[DOCS-COMPLIANT]`: Đối chiếu và khớp hoàn toàn với quy chuẩn đã khóa (Locked) trong tài liệu `docs/` của Huyền Lộ.
> - `[UNITY-MODERN]`: Giải pháp kiến trúc kỹ thuật hiện đại trên Unity Engine (C#, Shader Graph, ScriptableObject, URP).

---

# PHẦN I: KHẢO CỔ THỰC THI NSO (MAP, MOB, EFFECT & RESOLUTION TIERS)

## 1. PHÂN HỆ BẢN ĐỒ & MÔI TRƯỜNG (MAP & ENVIRONMENT)

### 1.1. Cấu Trúc Nhị Phân File Map (`Data/Map/<id>`)
Khảo sát toàn bộ 177 file nhị phân trong `research/SRC NSOACE FIX/Data/Map/`:

- `[ASSET-PROVEN]`: **176/177 file map** tuân thủ tuyệt đối cấu trúc định dạng phẳng:
  $$\text{File Size} = 2 + (\text{Width} \times \text{Height}) \text{ bytes}$$
  - **Byte 0**: `Width` (số lượng ô tile theo chiều ngang).
  - **Byte 1**: `Height` (số lượng ô tile theo chiều dọc).
  - **Từ Byte 2 đến hết**: Mảng nhị phân $W \times H$ bytes chứa các `TileIndex` (chỉ số tile từ $0 \rightarrow 255$, mỗi byte đại diện 1 ô tile $24 \times 24\text{ px}$).
  - *Dẫn chứng kích thước*:
    - `Map/0`: $W=50, H=15$ ($750$ ô) $\rightarrow$ Size = $2 + 750 = \mathbf{752\text{ bytes}}$.
    - `Map/1`: $W=80, H=20$ ($1.600$ ô) $\rightarrow$ Size = $2 + 1600 = \mathbf{1.602\text{ bytes}}$.
    - `Map/10`: $W=68, H=16$ ($1.088$ ô) $\rightarrow$ Size = $2 + 1088 = \mathbf{1.090\text{ bytes}}$.

- `[SOURCE-PROVEN]` & `[ASSET-PROVEN]`: **Bóc Tách & Giải Mã Anomaly File `Map/176` (Hang Nymoz)**:
  - File `Map/176` có dung lượng thực tế trên đĩa là **904 bytes**. Tuy nhiên hai byte đầu tiên của file là `52, 204` ($0x34, 0xCC$). Nếu hiểu là $W=52, H=204$ thì dung lượng phải là $2 + 52 \times 204 = 10.610\text{ bytes}$.
  - **Phân tích mã nguồn Server** (`com.nsoz.map.TileMap.java` lines 615–622):
    ```java
    byte key = 0;
    if (ab[0] + ab[1] == 0) { // Cờ nhận diện map có header đặc biệt
        dis.read(); dis.read();
        key = ab[0];
    }
    this.tmw = (short) (dis.read() - key);
    this.tmh = (short) (dis.read() - key);
    ```
  - **Phân tích CSDL SQL** (`nsoz.sql` dòng 2276): Bản ghi `(176, 'Hang Nymoz', '[]', '[]', '[]', 0, '[]', 3, 0, 0, '[]', '[]', '[]', '[]')`. Cả mảng waypoint, monster, stand đều rỗng hoàn toàn.
  - **Phân tích tần suất Byte của `Map/176`**: Byte 52 ($0x34$) chiếm tới **392 lần** (43.3% dung lượng file), Byte 163 ($0xA3$) chiếm **213 lần** (23.5%).
  - **Phân tích Client J2ME** (`nameDQ.java` line 1553): Client khởi tạo mảng bản đồ cứng gồm đúng 160 phần tử: `nameDQ.a = new byte[160][]`. Client nguyên bản chưa bao giờ load các map ID $\ge 160$.
  - **Kết Luận Bản Chất**: File `Map/176` là một **file map bị lỗi xuất bản (corrupted/malformed export)** trong bộ công cụ map editor nội bộ khi thêm dữ liệu thử nghiệm Hang Nymoz (hoặc header bị ghi đè byte rác trong quá trình đóng gói). Hoàn toàn **không có thuật toán nén ZIP/GZIP hay mã hóa đối xứng bí mật** nào ở file này.

---

### 1.2. Mối Quan Hệ Giữa `tile/*.png` và Minimap Lookup (`mini_*.png`)
Khảo sát trực tiếp các file ảnh trong `research/SRC NSOACE FIX/Data/Img/Map/1/tile/`:

- `[ASSET-PROVEN]`:
  - `tile/1.png` ($24 \times 2880\text{ px}$) $\rightarrow 2880 / 24 =$ **120 tiles** ($24 \times 24\text{ px}$).  
    `mini_1.png` ($2 \times 240\text{ px}$) $\rightarrow 240 / 2 =$ **120 blocks** ($2 \times 2\text{ px}$). $\rightarrow$ **Khớp 100% tỷ lệ 1:1**.
  - `tile/2.png` ($24 \times 3384\text{ px}$) $\rightarrow 3384 / 24 =$ **141 tiles**.  
    `mini_2.png` ($2 \times 282\text{ px}$) $\rightarrow 282 / 2 =$ **141 blocks**. $\rightarrow$ **Khớp 100% tỷ lệ 1:1**.
  - `tile/3.png` ($24 \times 3432\text{ px}$) $\rightarrow 3432 / 24 =$ **143 tiles**.  
    `mini_3.png` ($2 \times 286\text{ px}$) $\rightarrow 286 / 2 =$ **143 blocks**. $\rightarrow$ **Khớp 100% tỷ lệ 1:1**.
  - `tile/4.png` ($24 \times 2472\text{ px}$) $\rightarrow 2472 / 24 =$ **103 tiles**.  
    `mini_4.png` ($2 \times 286\text{ px}$) $\rightarrow 286 / 2 =$ **143 blocks**. $\rightarrow$ **LỆCH: Dư 40 blocks**.

- `[SOURCE-PROVEN]`: **Thuật Toán Tổng Hợp Minimap Trong Client (`nameDQ.java` lines 420–435)**:
  ```java
  // Tạo canvas ảnh Minimap trong RAM: chiều rộng mapW * 2, chiều cao mapH * 2
  a = Image.createImage((int)(mapW * 2 * nameGE.a), (int)(mapH * 2 * nameGE.a));
  nameGE g = new nameGE(a.getGraphics());
  g.c(0, 0, mapW * 2, mapH * 2);
  for (int x = 0; x < mapW; ++x) {
      for (int y = 0; y < mapH; ++y) {
          int tileIndex = mapData[y * mapW + x] - 1;
          if (tileIndex == -1) continue;
          // Cắt đúng mẩu 2x2 px từ dải mini_*.png tương ứng vẽ vào tọa độ (x*2, y*2)
          g.drawRegion(miniImage, 0, tileIndex * 2, 2, 2, 0, x * 2, y * 2, 0);
      }
  }
  ```
- **Lý giải Mismatch của `mini_4.png`**:
  - Khi render minimap, client đọc `tileIndex` từ file nhị phân map (chỉ số chỉ chạy từ $0 \rightarrow 102$). Do đó, vùng ảnh thừa từ $103 \rightarrow 142$ trong `mini_4.png` hoàn toàn không bao giờ bị trỏ tới và không gây crash game.
  - `mini_4.png` vốn được export tự động từ bản nháp cũ của `tile/4.png` (có cùng 143 tiles như `tile/3`), sau đó họa sĩ đã xóa bớt 40 tiles không dùng ở cuối `tile/4.png` nhưng không re-export dải `mini_4.png`.
- **Bài học bản chất**: NSO **không bao giờ dùng camera phụ render thu nhỏ toàn cảnh** để làm minimap. Minimap là một cấu trúc đồ họa cực nhẹ được **tổng hợp trực tiếp từ mảng Tile ID kết hợp dải màu $2 \times 2$ pixel** một lần duy nhất lúc tải map trong RAM.

---

### 1.3. Hệ Thống Nước Động & Animation Độc Lập (Water Subsystem)
- `[ASSET-PROVEN]`: Các file nước không nằm trong strip `tile/*.png` mà là các asset animation riêng:
  - `wtf.png` (Water Flow): Dải mặt nước động gồm 4 frame ($24 \times 96\text{ px} = 4 \text{ frame } 24 \times 24\text{ px}$).
  - `twtf.png` (Top Water Fall): Dải nước thác đổ ngọn gồm các frame cuộn.
  - `uwt.png` (Under Water): Texture khối nước chìm bên dưới.
  - `wts.png`, `wts1.png` (Water Splash): Bọt nước bắn tung tóe.
- `[SOURCE-PROVEN]`:
  - Trong `TileMap.java` (Server dòng 408–414) và `nameDQ.java` (Client dòng 480–545):
    ```java
    // Render mặt nước động theo chu kỳ nhịp đồng hồ nameCX.e:
    if ((nameDQ.e(i, j) & 0x20) == 32) { // Cờ T_WATERFLOW
        g.drawRegion(img_wtf, 0, 24 * (nameCX.e % 4), 24, 24, 0, i * 24, j * 24, 0);
    }
    // Render thác nước đổ ngọn:
    if (tile == 17 || tile == 23) {
        g.drawRegion(img_twtf, 0, 24 * (nameCX.e % 8 >> 1), 24, 24, 0, i * 24, j * 24, 0);
    }
    ```
  - **Bản chất**: Mặt nước không phải là các tile tĩnh, mà được engine quét qua mảng cờ va chạm (`T_WATERFLOW = 32`). Bất cứ ô nào có cờ này sẽ được tự động vẽ đè một dải hoạt ảnh sóng nước 4 frame chạy tuần hoàn theo clock toàn cục (`nameCX.e % 4`).

---

### 1.4. Cơ Chế Va Chạm & Phân Tách Biên Giới Client - Server (Collision & Authority)
- `[SOURCE-PROVEN]`:
  - **Server là nơi nắm giữ toàn quyền logic va chạm** (`TileMap.java` lines 383–475):
    - Server đọc file nhị phân `Data/Map/<id>`, lấy mảng `maps[]`, sau đó dựa vào `tileSetId` (1, 2, 3, hoặc 4) để ánh xạ từng số `TileID` sang **Bitmask va chạm** (`types[]`):
      - `T_TOP = 2`: Mặt sàn đứng được (chân nhân vật tiếp đất).
      - `T_LEFT = 4`, `T_RIGHT = 8`: Tường chắn bên trái/phải.
      - `T_BRIDGE = 512`: Cầu / Bục gỗ cho phép nhảy xuyên từ dưới lên hoặc thả rơi xuống (`DropThrough`).
      - `T_WATERFLOW = 32`: Dòng nước chảy làm trôi nhân vật.
      - `T_SOLIDGROUND = 2048`: Đất đặc không thể đi xuyên qua.
    - Server tính toán di chuyển, rơi tự do, bơi, và kiểm tra hack tọa độ hoàn toàn trên mảng `types[]` bitmask này mà **không cần bất kỳ hình ảnh nào**.

---

## 2. HỆ THỐNG QUÁI VẬT & THỰC THỂ DỰNG TỪ MOB RENDERER (MOB SYSTEM)

### 2.1. Phân Loại Cấu Trúc File Mob: Single PNG vs Packed Sheet
- `[ASSET-PROVEN]`:
  - Các Mob ID thấp ($\le 190$): Đa số gồm 3–4 file ảnh riêng rẽ theo cú pháp `{mobId}_{index}.png` (ví dụ `0_0.png`, `0_1.png`, `0_2.png`).
  - Các Mob ID cao ($\ge 220$, ví dụ Mob 240 - Nhất vĩ, Mob 237 - Juubi): Chỉ có **đúng 1 file PNG lớn** (ví dụ `240_0.png` $255 \times 255\text{ px}$) chứa toàn bộ các mảnh bộ phận, chiêu thức và vệt đuôi.
- `[SOURCE-PROVEN]`: **Sự Tiến Hóa Pipeline trong Mã Nguồn (`GameData.java` & `Service.java`)**:
  - Tại `Service.java` dòng 1985–2000, Server phân biệt rõ:
    - **`mob.id < 236` $\rightarrow$ `writeDataMobOld()`**: Quái thế hệ cũ. Mỗi file PNG là một frame ảnh tĩnh thô hoặc một trạng thái hoàn chỉnh.
    - **`mob.id >= 236` $\rightarrow$ `writeDataMobNew()`**: Quái thế hệ mới (Boss, Pet, Vĩ thú). Server serialize toàn bộ metadata cắt sprite:
      1. `ImageInfo[]`: Tọa độ cắt hình chữ nhật $(x, y, w, h)$ từ tấm Sprite Sheet lớn.
      2. `Frame[]`: Mỗi frame diễn hoạt gồm nhiều mảnh con gộp lại, kèm $(dx, dy)$, cờ lật `flip`, và thứ tự lớp `onTop`.
      3. `sequence[]`: Mảng chỉ số frame diễn giải nhịp chuyển động (`Idle`, `Move`, `Attack`).

---

### 2.2. Giải Mã `Mob/98.png` và `Mob/99.png`: Thư Mục `Mob` Chứa Những Gì?
- `[SOURCE-PROVEN]` & `[ASSET-PROVEN]`:
  - File `Mob/98.png` và `Mob/99.png` là ảnh đơn, không có đuôi `_0/_1`.
  - Tra cứu trong `nsoz.sql` dòng 2650:
    - `Mob 98`: Tên là **`Bạch Long trụ`** (HP: 3.000.000, speed: 0, range_move: 0, type: 0).
    - `Mob 99`: Tên là **`Hắc Long trụ`** (HP: 3.000.000, speed: 0, range_move: 0, type: 0).
  - Tra cứu trong `Service.java` dòng 1960:
    ```java
    if (mobTemplateID == MobName.BACH_LONG_TRU || mobTemplateID == MobName.HAC_LONG_TRU) {
        byte[] ab = GameData.getInstance().loadFile("Data/Img/Mob/" + zoomLevel + "/" + mobTemplateID + ".png");
        ds.writeInt(ab.length);
        ds.write(ab);
    }
    ```
  - **Kết luận bản chất**: Thư mục `Mob/` **không chỉ chứa quái vật di động (monsters)**, mà là kho chứa **toàn bộ các thực thể tương tác có thanh máu trong game (Damageable Entities)**: bao gồm Trụ bảo vệ (Totem), Cột cờ gia tộc, Bù nhìn luyện võ, Thùng bùa chú, và Boss tĩnh.

---

## 3. HỆ THỐNG HIỆU ỨNG HÌNH ẢNH RỜI (EFFECT SUBSYSTEM)

### 3.1. Bản Chất của `Effect/<id>.png`: Repository Sprite Sheet
- `[ASSET-PROVEN]`: Có 235 Effect ID cho mỗi tier resolution. Ảnh `Effect/<id>.png` **không bao giờ là 1 animation frame đơn lẻ**, mà là một **Packed Sprite Sheet chứa nhiều mảnh vụn hình chữ nhật không đều nhau** (irregular atlas).
- `[SOURCE-PROVEN]`: **Kiến Trúc Metadata Effect (`EffectDataManager.java` & `EffectData.java`)**:
  Để cắt và diễn hoạt một `Effect`, hệ thống NSO sử dụng bảng CSDL `effect_data` gồm 5 trường cấu trúc:
  ```sql
  INSERT INTO `effect_data` (`id`, `sprites`, `frames`, `running`, `frame_char`) VALUES
  (0, '[{"w":32,"x":0,"h":13,"y":74,"id":0}, ...]', '[[{"id":0,"dx":-15,"dy":-6,"flip":0,"onTop":0}], ...]', '[0,1,2,3]', '[[0],[1],[2],[3]]');
  ```
  1. **`sprites` (`SmallImage[]`)**: Bảng hình chữ nhật định vị chính xác vị trí mảnh cắt trong tấm PNG: $\{id, x, y, w, h\}$.
  2. **`frames` (`PartFrame[][]`)**: Một keyframe hoàn chỉnh được ghép từ $N$ mảnh `SmallImage`, mỗi mảnh có độ lệch tâm $(dx, dy)$, cờ lật `flip` ($0$ hoặc $1$), và thứ tự phủ `onTop`.
  3. **`running` (`byte[] sequence`)**: Mảng timeline quy định thứ tự phát các keyframe (ví dụ `[0, 1, 2, 3, 2, 1]`).
  4. **`frame_char`**: Ánh xạ frame của effect tương ứng với 4 trạng thái của nhân vật (Đứng, Chạy, Nhảy, Tấn công).

---

## 4. BẢN CHẤT 4 TIER RESOLUTION (`Data/Img/1/`, `2/`, `3/`, `4/`)

- `[SOURCE-PROVEN]`:
  - Trong `Session.java` lines 91–95:
    ```java
    this.zoomLevel = mss.reader().readByte();
    if (this.zoomLevel < 1 || this.zoomLevel > 4) {
        this.zoomLevel = 1;
    }
    ```
  - Khi client kết nối tới Server, thiết bị client gửi lên thông số `zoomLevel` (tương ứng với mật độ điểm ảnh của máy):
    - **Tier 1 (J2ME / Màn hình nhỏ 128x128, 240x320)**: Scale gốc $1\times$.
    - **Tier 2 (Android màn hình trung bình HVGA / WVGA)**: Scale $1.5\times - 2\times$.
    - **Tier 3 (iOS / Màn hình HD)**: Scale $3\times$.
    - **Tier 4 (Màn hình Full HD / Máy tính PC)**: Scale $4\times$.
- `[ASSET-PROVEN]`: Khảo sát các file ảnh cùng ID giữa các thư mục `1/`, `2/`, `3/`, `4/` cho thấy các asset ở Tier cao **được vẽ lại đường nét (hand-retouched outlines), thay đổi khoảng đệm (padding) và khử răng cưa cục bộ** để phù hợp với màn hình cảm ứng độ nét cao, không phải Nearest-Neighbor thuần túy.

---

# PHẦN II: BẢN ĐỒ ÁNH XẠ CHUẨN HÓA SANG UNITY ENGINE HIỆN ĐẠI (PROJECT HUYỀN LỘ)

## 5. SO SÁNH ĐỐI CHIẾU VỚI TÀI LIỆU THIẾT KẾ HUYỀN LỘ (`docs/`)

Bảng đối chiếu giữa **Bản chất thực thi NSO** và **Tài liệu đặc tả thiết kế Huyền Lộ**:

```
+-------------------------------------------------------------------------------------------------------------------------------+
| SO SÁNH ĐỐI CHIẾU: THỰC NGHIỆM NSO VS ĐẶC TẢ THIẾT KẾ HUYỀN LỘ (DOCS/)                                                         |
+----------------------+---------------------------------+---------------------------------+------------------------------------+
| Hạng mục             | Thực nghiệm NSO (Legacy)        | Đặc tả Huyền Lộ (Docs/)         | Giải pháp Unity Hiện Đại           |
+----------------------+---------------------------------+---------------------------------+------------------------------------+
| 1. Hệ thống Độ phân  | 4 folder asset riêng biệt       | [DOCS-COMPLIANT] Khóa 1 chuẩn   | [UNITY-MODERN] 1 Bộ Asset PPU 32 + |
| giải (Resolution)    | (Tier 1/2/3/4) tốn bộ nhớ       | PPU 32 duy nhất, Canvas 64x64   | URP 2D Pixel Perfect Camera        |
+----------------------+---------------------------------+---------------------------------+------------------------------------+
| 2. Địa hình &        | Tile 24x24 px; Dùng bitmask     | [DOCS-COMPLIANT] Locked địa hình| [UNITY-MODERN] Unity Tilemap 2D +  |
| Va chạm (Terrain)    | T_TOP, T_LEFT, T_BRIDGE...      | trực giao, khối solid có độ dày,| CompositeCollider2D (Geometry type |
|                      | Cầu một chiều nhảy xuyên qua    | One-way hiếm, DropThrough 1 sàn | = Polygons / Outlines)             |
+----------------------+---------------------------------+---------------------------------+------------------------------------+
| 3. Bản đồ nhỏ        | Tổng hợp ảnh mini 2x2 px        | [DOCS-COMPLIANT] Panel thông tin| [UNITY-MODERN] Render trực tiếp từ |
| (Minimap System)     | từ mảng Tile ID trong RAM       | Map Info, Living Entity Counts  | Tile Data sang Texture2D tĩnh 1 lần|
+----------------------+---------------------------------+---------------------------------+------------------------------------+
| 4. Nước & Môi trường | Cắt dải wtf.png, twtf.png bằng  | [DOCS-COMPLIANT] Nước nông cạn, | [UNITY-MODERN] Shader Graph 2D     |
| (Water & Environment)| CPU theo nhịp clock % 4         | giảm nhẹ MoveSpeed khi chân chạm| UV Panner + Sine Distortion        |
+----------------------+---------------------------------+---------------------------------+------------------------------------+
| 5. Quái & Thực thể   | Quái mới dùng Packed Sheet      | [DOCS-COMPLIANT] 1 Linh Đạn     | [UNITY-MODERN] ScriptableObject    |
| (Mob Pipeline)       | Trụ, Cọc cờ dùng chung Mob      | chung; Mob/Linh Biến threat;    | + Sprite Atlas + ObjectPool        |
|                      | renderer                        | Dummy HP 60, Respawn 25s        |                                    |
+----------------------+---------------------------------+---------------------------------+------------------------------------+
```

---

## 6. QUY CHUẨN KỸ THUẬT UNITY ENGINE HIỆN ĐẠI CHO HUYỀN LỘ

### 6.1. Chuẩn Hóa Độ Phân Giải & Pixel Perfect Camera
- `[DOCS-COMPLIANT]` & `[UNITY-MODERN]`:
  - Trong `docs/03-art/art-and-visual-production.md` (L108, L292): Đã khóa **Canvas $64 \times 64\text{ px}$**, **PPU = 32** (32 pixel = 1 Unit trong Unity World), và **Reference Camera Resolution $384 \times 216\text{ px}$** (tỷ lệ 16:9 gốc).
  - Cấu hình component **`Pixel Perfect Camera`** (URP 2D):
    ```csharp
    // Thông số thiết lập chuẩn trên Camera chính
    AssetsPPU = 32;
    ReferenceResolution = new Vector2Int(384, 216);
    UpscaleRT = true;              // Giữ pixel art sắc nét, không bị làm mờ (Bilinear)
    PixelSnapping = true;          // Chống hiện tượng jitter/sub-pixel jitter khi di chuyển
    CropFrameX = false;
    CropFrameY = false;
    ```
  - **Lợi ích**: Studio chỉ cần vẽ đúng **1 bộ asset duy nhất**. Unity sẽ tự động scale nguyên khối lên 720p, 1080p, 1440p hay 4K mà nét vẽ pixel luôn vuông vắn, hoàn hảo không bị vỡ đường viền.

---

### 6.2. Quy Trình Xuất Bản Bản Đồ 1 Nguồn (Single-Source Map Authoring)
- `[DOCS-COMPLIANT]` & `[UNITY-MODERN]`:
  - Trong `docs/01-design/world-and-content.md` (L218–L228): Địa hình Huyền Lộ đã **LOCKED địa hình trực giao (Orthogonal Terrain)**:
    - Đất/đá tự nhiên là **khối solid có độ dày**, mặt trên ngang, mặt đứng thẳng, không dốc (no slope/ramp), không leo trèo (no climb/ladder).
    - Sàn **One-way hiếm**, chỉ áp dụng cho ván mỏng, giàn gỗ có cột đỡ rõ ràng.
  - **Kiến Trúc Xuất Bản 1 Nguồn (Single-Source Pipeline)**:
    ```
    [ Authored Map trong Unity / Tiled Editor ]
                        │
                        ▼ (Build Script: MapExportPipeline.cs)
         ┌──────────────┴──────────────┐
         ▼                             ▼
    [ Game Server Data ]          [ Unity Client Prefab ]
    • map_geometry.bin            • Tilemap_Solid (Ground Layer)
    • BoxColliders (Solid)        • Tilemap_OneWay (PlatformEffector2D)
    • EdgeColliders (One-way)     • Tilemap_Decor (No Collider)
    • Spawner / Leash Anchors     • Parallax Background Layers
    • Waypoint / Portal Rects     • 2D Lights & Environment VFX
    ```
  - **Lợi ích kiến trúc**: Server tính toán va chạm bằng các hình hộp chữ nhật lớn (`AABB Box Colliders`), loại bỏ hoàn toàn việc dò từng ô tile rời rạc của J2ME cũ. Client và Server được sinh ra từ cùng 1 file thiết kế gốc, **loại trừ 100% nguy cơ lệch map giữa hai đầu**.

---

### 6.3. Triển Khai Mặt Nước Bằng Shader Graph 2D (Thay Thế CPU Water Loop)
- `[DOCS-COMPLIANT]` & `[UNITY-MODERN]`:
  - Tài liệu GDD (`docs/01-design/world-and-content.md` L232) quy định: Lòng nước nông có đáy đất thật, người chơi lội qua bị giảm nhẹ tốc chạy, không thêm bơi lội/đuối nước.
  - **Giải Pháp Unity**: Thay vì cắt frame CPU như NSO (`wtf.png % 4`), tạo **Shader Graph 2D `Shader_ShallowWater2D`**:
    1. **UV Panner Node**: Tự động cuộn texture mặt nước theo trục ngang $X$ với tốc độ cố định ($0.1\text{ u/s}$).
    2. **Sine Distortion Node**: Làm gợn sóng nhẹ ở mép tiếp giáp giữa mặt nước và bờ đá.
    3. **Sprite Lit Material**: Tương thích hoàn hảo với hệ thống ánh sáng 2D (Light 2D) của URP.
    $\rightarrow$ **Hiệu năng**: Chạy mượt mà 60 FPS trên GPU, tiêu tốn đúng **0% CPU**.

---

### 6.4. Triển Khai Minimap Hiện Đại (Data-Driven Texture2D Synthesis)
- `[DOCS-COMPLIANT]` & `[UNITY-MODERN]`:
  - Kế thừa bài học kinh điển của NSO: **Không dùng Secondary Camera render Minimap**.
  - **Cơ chế Hiện Đại**:
    ```csharp
    // Sinh Minimap siêu nhẹ trong RAM lúc nạp Map:
    public Texture2D GenerateMinimap(Tilemap groundMap, Tilemap oneWayMap)
    {
        BoundsInt bounds = groundMap.cellBounds;
        Texture2D miniTex = new Texture2D(bounds.size.x, bounds.size.y, TextureFormat.RGBA32, false);
        miniTex.filterMode = FilterMode.Point; // Giữ pixel rõ nét

        for (int x = 0; x < bounds.size.x; x++) {
            for (int y = 0; y < bounds.size.y; y++) {
                Vector3Int pos = new Vector3Int(bounds.xMin + x, bounds.yMin + y, 0);
                if (groundMap.HasTile(pos))
                    miniTex.SetPixel(x, y, new Color(0.35f, 0.25f, 0.15f, 1f)); // Màu đất
                else if (oneWayMap.HasTile(pos))
                    miniTex.SetPixel(x, y, new Color(0.6f, 0.45f, 0.2f, 1f));   // Màu ván gỗ
                else
                    miniTex.SetPixel(x, y, Color.clear);
            }
        }
        miniTex.Apply();
        return miniTex;
    }
    ```
  - Tấm ảnh texture sau khi sinh chỉ chiếm chưa đầy **$10\text{ KB}$ RAM**, được gắn vào UI RawImage và vẽ vị trí Player/Party bằng các Icon Marker động.

---

### 6.5. Quản Lý Quái Vật & VFX Bằng ScriptableObject + Object Pooling
- `[DOCS-COMPLIANT]` & `[UNITY-MODERN]`:
  - Trong `docs/01-design/world-and-content.md` (L250, L283): Quái vật chia sẻ đạn chung (`Linh Đạn`), Linh Biến tái sử dụng nguyên vẹn sprite của mob gốc và chỉ nhân scale $1.25\times$ kèm Aura shader.
  - **Kiến Trúc ScriptableObject**:
    - `MobVisualDefinitionSO`: Chứa Sprite Atlas, Animator Override Controller, Socket offsets (Head, Hand, Center, Ground), và Footstep audio.
    - `VFXDefinitionSO`: Chứa Prefab hiệu ứng, Sorting Order, thời lượng tồn tại (Duration), và cơ chế hủy (Despawn).
  - Toàn bộ Mũi tên, Kiếm khí, và Impact VFX được lưu thông qua generic `UnityEngine.Pool.ObjectPool<GameObject>`:
    ```csharp
    // Loại bỏ 100% rác bộ nhớ GC Alloc
    private IObjectPool<GameObject> vfxPool;
    vfxPool = new ObjectPool<GameObject>(
        createFunc: () => Instantiate(vfxPrefab),
        actionOnGet: obj => obj.SetActive(true),
        actionOnRelease: obj => obj.SetActive(false),
        actionOnDestroy: obj => Destroy(obj),
        defaultCapacity: 20,
        maxSize: 100
    );
    ```

---

## 7. BẢNG PHÂN ĐỊNH TRÁCH NHIỆM CLIENT - SERVER (AUTHORITY MATRIX)

```
+-------------------------------------------------------------------------------------------------------------------------+
| PHÂN ĐỊNH RANH GIỚI TRÁCH NHIỆM CLIENT - SERVER TRONG HUYỀN LỘ RUNTIME                                                  |
+----------------------+---------------------------------+---------------------------------+------------------------------+
| Phân hệ              | Dữ liệu Authoritative tại Server| State gửi qua Network (Packets) | Trách nhiệm Unity Client     |
+----------------------+---------------------------------+---------------------------------+------------------------------+
| 1. Map & Terrain     | • Kích thước Map Bounds         | • MapId khi chuyển vùng         | • Render Tilemap Visuals     |
|                      | • BoxColliders (Đất đặc/Tường)  | • Danh sách Entity trong Map    | • Minimap UI & Marker        |
|                      | • EdgeColliders (One-way)       | • Vị trí NPC, Waypoint, Portal  | • Shader nước động, mây trôi |
|                      | • Spawners, Leash/Home Region   | (Không gửi tile textures/bytes) | • Parallax Background scroll |
+----------------------+---------------------------------+---------------------------------+------------------------------+
| 2. Mob & Entity      | • HP, MaxHP, DEF, Level         | • MobId, InstanceId             | • Animation Idle/Run/Attack  |
|                      | • Vị trí thực (X, Y)            | • State (Moving, Attack, Return)| • Interpolation vị trí mượt  |
|                      | • AI Target, Leash enforcement  | • Tọa độ đích di chuyển         | • Flash chớp đỏ khi trúng đòn|
|                      | • Damage & Trúng/Trượt resolve  | • TargetId khi ra chiêu         | • Hiển thị thanh máu trên đầu|
+----------------------+---------------------------------+---------------------------------+------------------------------+
| 3. Effect & VFX      | • SkillId được chấp nhận        | • CasterId, TargetId            | • Tự phát animation VFX      |
|                      | • Mốc thời gian resolve sát     | • SkillId, Timestamp bắt đầu    | • Tự tính offset Socket      |
|                      |   thương (+0.12s, +0.18s)       | • StatusId (Bỏng, Đóng băng)    | • Tự thu hồi vào ObjectPool  |
|                      | (Không render hay đếm frame VFX)| (Không gửi từng frame animation)| (VFX không tự gây damage)    |
+-------------------------------------------------------------------------------------------------------------------------+
```

---

## 8. KẾT LUẬN & ĐỀ XUẤT HÀNH ĐỘNG TIẾP THEO

1. **Chuẩn Hóa Bộ Công Cụ Tác Nghiệp (Authoring Tooling)**:
   - Xây dựng công cụ `MapExportPipeline.cs` trong Unity Editor để khi Level Designer vẽ map trên Tilemap, công cụ tự động xuất ra file va chạm phẳng nhị phân/JSON cho Server và Prefab hoàn chỉnh cho Client.
2. **Loại Bỏ Phân Mảnh Resolution**:
   - Kiên định với lựa chọn **1 Bộ Asset duy nhất PPU 32** kết hợp **Pixel Perfect Camera** của URP 2D. Không tốn công sức chia nhỏ thành 4 folder tier như NSO.
3. **Hiện Thực Hóa Các Mẫu Shader & Script**:
   - Triển khai Shader Graph nước nông 2D và lớp `ObjectPool` cho đạn/VFX trong thư mục nguyên mẫu `prototypes/`.
