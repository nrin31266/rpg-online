# Hướng Dẫn Vận Hành Môi Trường Nghiên Cứu NSO (Ninja School Online)

Thư mục `research/` chứa mã nguồn máy chủ (Backend Java), cơ sở dữ liệu (MariaDB Docker), và client game (Unity PC 2.1.9) dùng để nghiên cứu cơ chế mạng, giao thức và gameplay.

---

## 1. Danh Sách Tài Khoản Thử Nghiệm (Account List)

Tất cả các tài khoản mặc định dưới đây đều đã được kích hoạt sẵn (`status = 1`, `activated = 1`) trong cơ sở dữ liệu `nsozace`:

| STT | Username | Mật khẩu | Lượng (Gold) | Nhân vật có sẵn | Ghi chú |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **1** | `admin` | `1` | ~50.000.000 | • `1314142dx` (Class 2, 9.9 tỷ xu, 5tr yên)<br>• `421sdcz`<br>• `421sdcz2` | Tài khoản Admin chính, full đồ/tiền |
| **2** | `admintest` | `1` | 50.000.000 | • `421sdczv` (Class 2, 105tr yên)<br>• `admin45` (Class 0, 100tr xu) | Tài khoản test admin |
| **3** | `test` | `1` | 100.000 | *(Chưa tạo nhân vật)* | Tài khoản sạch để tạo mới nhân vật |
| **4** | `1` | `1` | 0 | • `adminn` | Tài khoản phụ |

> **Lưu ý quan trọng**: Mật khẩu trong DB lưu dạng plain-text (hoặc so sánh trực tiếp). Khi đăng nhập trên client, chọn server nội bộ và điền đúng Username / Password ở bảng trên.

---

## 2. Hướng Dẫn Khởi Động Toàn Bộ Hệ Thống

### Cách 1: Khởi động tự động bằng script (Khuyên dùng)

Chỉ cần chạy một lệnh duy nhất:

```bash
cd /home/nguyenvanrin/PersonalProjects/rpg-online/research
./start_all.sh
```

Script sẽ tự động thực hiện:
1. Kiểm tra Docker daemon và khởi động container `nso-mariadb` (MariaDB 10.11 trên port `3306`).
2. Giải phóng port `14444` nếu đang bị chiếm dụng.
3. Chạy Backend Java (`target/Nso-jar-with-dependencies.jar`) ở chế độ background và đợi socket `14444` sẵn sàng.
4. Mở Client Unity PC 2.1.9 qua Wine (`Ninja School 2.1.9.exe`) hiển thị trên màn hình desktop.

---

### Cách 2: Khởi động từng phần thủ công (Manual)

Nếu cần debug hoặc xem log trực tiếp:

#### Bước 1: Khởi động Database MariaDB
```bash
cd /home/nguyenvanrin/PersonalProjects/rpg-online
docker compose -f research/docker-compose.yml --project-directory . up -d
```
Kiểm tra MariaDB sẵn sàng:
```bash
docker exec nso-mariadb mariadb-admin ping -h localhost
```

#### Bước 2: Khởi động Game Server (Java Backend)
```bash
cd "/home/nguyenvanrin/PersonalProjects/rpg-online/research/SRC NSOACE FIX"
# Giải phóng port 14444 nếu cần
fuser -k 14444/tcp 2>/dev/null || true

# Chạy server
java -server -jar -Dfile.encoding=UTF-8 -Xms512M -Xmx1G target/Nso-jar-with-dependencies.jar
```
*Server sẽ load dữ liệu NPC, Monster, Map, Skill, Task và lắng nghe tại port `14444`.*

#### Bước 3: Khởi động Game Client (Unity PC)
Mở một terminal khác:
```bash
cd /home/nguyenvanrin/PersonalProjects/rpg-online/research/Nso219Pc
wine "Ninja School 2.1.9.exe"
```

---

## 3. Tạo Tài Khoản Mới Trực Tiếp Vào Database

Game server không hỗ trợ đăng ký trực tiếp trong game (server gốc đăng ký qua web). Để tạo thêm tài khoản mới vào cơ sở dữ liệu:

```bash
docker exec -i nso-mariadb mariadb -u root nsozace <<EOF
INSERT INTO users (username, password, status, activated, luong) 
VALUES ('tentaikhoan', 'matkhau', 1, 1, 10000000);
EOF
```
*Điều kiện bắt buộc: `status = 1` (không bị khóa) và `activated = 1` (đã kích hoạt) thì server mới cho phép đăng nhập.*

---

## 4. Các Lệnh Tiện Ích Quản Lý

### Theo dõi Log
- **Xem log Backend:**
  ```bash
  tail -f /tmp/nso_server.log
  ```
- **Xem log Client Unity:**
  ```bash
  tail -f /tmp/nso_client.log
  ```
- **Xem log MariaDB Docker:**
  ```bash
  docker logs -f nso-mariadb
  ```

### Tắt Dịch Vụ
- **Tắt Client:**
  ```bash
  pkill -f "Ninja School 2.1.9.exe"
  ```
- **Tắt Backend Server:**
  ```bash
  fuser -k 14444/tcp
  ```
- **Tắt MariaDB:**
  ```bash
  docker stop nso-mariadb
  ```
- **Tắt toàn bộ:**
  ```bash
  pkill -f "Ninja School 2.1.9.exe" && fuser -k 14444/tcp && docker stop nso-mariadb
  ```

---

## 5. Xử Lý Sự Cố Thường Gặp (Troubleshooting)

1. **Lỗi kết nối Socket `localhost.saygame.us`:**
   Client Unity được cấu hình kết nối tới domain `localhost.saygame.us:14444`. Mặc dù domain này đã có DNS phân giải về `127.0.0.1`, để đảm bảo hoạt động cả khi offline, có thể thêm vào file `/etc/hosts`:
   ```text
   127.0.0.1 localhost.saygame.us
   ```

2. **Lỗi Docker daemon không phản hồi:**
   Khởi động Docker Desktop:
   ```bash
   systemctl --user start docker-desktop
   ```

3. **Port 14444 bị kẹt:**
   ```bash
   fuser -k 14444/tcp
   ```


## Evidence notes

Ba báo cáo NSO ở [notes/](notes/) chỉ là reference/historical recommendations; luật Huyền Lộ thuộc [GDD](../docs/design/1_HUYEN_LO_GDD.md). Các claim CPU/recipe line-count và recommendation chưa duyệt không phải evidence runtime.

- [Research gaps](notes/NSO_RESEARCH_GAPS_BEFORE_HUYEN_LO_LOCK.md)
- [Modernization](notes/NSO_TO_HUYEN_LO_MODERNIZATION_RESEARCH.md)
- [World/farm/targeting](notes/NSO_WORLD_FARM_TARGETING_RESEARCH.md)

Các liên kết `file://` trong ba báo cáo là provenance từ máy/phiên nghiên cứu cũ, không phải routing portable. Source Java hiện tại nằm dưới `SRC NSOACE FIX/src/main/java/`; không dùng các đường dẫn `src/com/` cũ để xác nhận source.
