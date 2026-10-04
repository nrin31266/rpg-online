# V6.2.4 — kiểm chứng mock sau feedback

55 EditMode + 6 PlayMode PASS; standalone fresh Q1–Q6 PASS (physics/keyboard, timeScale4, không preset). Native keyboard/mouse review chạy 1× trên executable cuối; fixture Kiếm/Crowd có nhãn DEBUG, không thay nghiệm thu hành trình. G-L vẫn PARTIAL; production/network/backend chưa bắt đầu.

Lỗi equipment mouse: đổi actions giữa render sáu slot; kết thúc IMGUI event sau command đã sửa. Đã click slot trống/có đồ, bag filter, tabs, unequip và Esc; log không exception. Q1 intro trước nhận, accept auto-close giữ speech; tracker sai cổng đông được sửa theo step/current map.

Map: khối đất liền, bờ bậc, DS3 terrace và DS5 basin −2 u; cầu qua nước rộng, flow sau lane. Feet contact ×0,85; bridge/air dry. Native 4 Sói đều có lượt cắn, còn giao cắt ngắn; group bounds + return→patrol được kiểm logic, không thay combat range/interval.

Chưa có full fresh journey usability ở 1× hoặc human acceptance. Không dùng tests PASS để khóa spacing/camera/UI hoặc production art.

## Counts chạy bằng script trước/sau checkpoint

| File | Lines | Pipe rows | Headings |
| --- | ---: | ---: | ---: |
| README.md | 90 → 90 | 45 → 45 | 6 → 6 |
| 1_HUYEN_LO_GDD.md | 793 → 806 | 336 → 336 | 26 → 26 |
| 2_HUYEN_LO_TECHNICAL.md | 434 → 441 | 124 → 124 | 18 → 18 |
| 3_HUYEN_LO_DESIGN_ANALYSIS.md | 542 → 545 | 273 → 273 | 25 → 25 |
| 4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md | 941 → 965 | 401 → 401 | 52 → 52 |
| 5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md | 334 → 334 | 147 → 147 | 17 → 17 |

53.100 / 95 điểm / 32.000 / 26 frame và arithmetic 1.800/200 được giữ; numeric evidence rows Analysis/Art giữ nguyên. Chỉ wording water authority routing đổi có chủ đích theo user (water visual-only → shallow slowdown theo GDD). Existing .meta/history evidence giữ byte/GUID.

## SOURCE → DESTINATION — 8 mẫu ngẫu nhiên do root tự kiểm

Đây là mapping feedback/implementation, không khai MOVE giả hoặc xóa research. Script chọn 8/15 bằng seed cố định; root đã đọc đoạn đích và đối chiếu nội dung.

| SOURCE | DESTINATION | Kiểm |
| --- | --- | --- |
| pack returns then patrols | `prototypes/VS1_EndToEnd/Assets/_Prototype/VS1_EndToEnd/Domain/SliceSession.cs:349` | Có nội dung |
| mouse mutation ends render event | `prototypes/VS1_EndToEnd/Assets/_Prototype/VS1_EndToEnd/Runtime/SliceHud.cs:193` | Có nội dung |
| quest intro presentation | `prototypes/VS1_EndToEnd/Assets/_Prototype/VS1_EndToEnd/Runtime/SliceHud.cs:64` | Có nội dung |
| water feet contact, dry bridge | `prototypes/VS1_EndToEnd/Assets/_Prototype/VS1_EndToEnd/Domain/BlockoutLayout.cs:82` | Có nội dung |
| geometric shared rig | `prototypes/VS1_EndToEnd/Assets/_Prototype/VS1_EndToEnd/Runtime/GeometricRig.cs:19` | Có nội dung |
| skill slots | `prototypes/VS1_EndToEnd/Assets/_Prototype/VS1_EndToEnd/Runtime/SliceHud.cs:243` | Có nội dung |
| independent pack re-engage | `prototypes/VS1_EndToEnd/Assets/_Prototype/VS1_EndToEnd/Tests/EditMode/DomainTests.cs:230` | Có nội dung |
| single action vs batch menu close | `docs/design/1_HUYEN_LO_GDD.md:764` | Có nội dung |

## Cleanup

KEEP TRACKED: docs/source/scene/settings, existing meta và historical evidence. KEEP EVIDENCE: báo cáo/audit/script và bốn PNG có link trong Art. GITIGNORE: Library/Temp/Logs/UserSettings/Builds, InitTestScene*, raw XML/log/video. DELETE TEMP: obsolete isolated build và RPM downloads trong /tmp; không xóa tracked source/reference. Ignored research/start_all.sh và các nguồn ngoài scope giữ nguyên.

Chi tiết machine-readable ở [validation.json](validation.json), [audit](document-audit.json), [cleanup](cleanup-audit.json), [build](build-summary.txt), [fresh route](route-result.txt). Raw logs/captures nằm /tmp; không dump vào Git.
