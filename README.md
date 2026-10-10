# Huyền Lộ

[Documentation map](docs/README.md) là điểm vào bộ tài liệu hiện hành theo domain: design, technical, art và production/evidence. Prototype VS-1 đã được xóa khỏi checkout; [hồ sơ lịch sử và cách tra Git](docs/90-archive/production-history.md#prototype-source-retired) giữ trace của bản mẫu cũ.

```text
docs/                       Canonical docs theo domain + archive + old-path redirects
research/                    source/ghi chú tham khảo
```

Production chưa bắt đầu. Dự kiến `game/` là Unity project cho Client và Dedicated builds với gameplay chung; `backend/` là Spring Boot/PostgreSQL. Các folder/codebase đó chưa được scaffold. Architecture authority nằm trong [Gameplay Runtime](docs/02-technical/gameplay-runtime.md#architecture-discipline).
