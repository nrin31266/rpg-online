# Huyền Lộ — Documentation Map

Entry point duy nhất cho documentation hiện hành, tổ chức theo semantic owner. Trạng thái triển khai hiện hành xem tại [Roadmap](04-production/roadmap.md). VS-1 là prototype/reference; kết quả của prototype không tự chứng minh current acceptance.

## Documentation map

```text
docs/
├── README.md
├── 01-design/
│   ├── game-design.md
│   ├── combat-and-character.md
│   ├── world-and-content.md
│   ├── quests-and-narrative.md
│   └── items-and-economy.md
├── 02-technical/
│   ├── architecture.md
│   ├── gameplay-runtime.md
│   └── online-and-persistence.md
├── 03-art/
│   └── art-and-visual-production.md
├── 04-production/
│   ├── roadmap.md
│   └── playtest-and-balance.md
└── 90-archive/
    ├── design-history.md
    ├── art-history.md
    └── production-history.md
```

| Document | Document owns |
| --- | --- |
| [Game Design](01-design/game-design.md) | Identity, pillars, loop, scope, mục tiêu trải nghiệm, chương và ý nghĩa completion |
| [Combat & Character](01-design/combat-and-character.md) | Stats/class/skill/input/focus/pending/damage/status và combat modifiers |
| [World & Content](01-design/world-and-content.md) | Map/gate/topology/terrain, mob/AI gameplay, density/spawn, Linh Biến/Boss |
| [Quests & Narrative](01-design/quests-and-narrative.md) | Lore/NPC/dialogue, canonical objectives và quest credit/gameplay recovery |
| [Items & Economy](01-design/items-and-economy.md) | Gear/Food/Potion, inventory/shop/loot, Gold/Journey và PvP stake economy |
| [Architecture](02-technical/architecture.md) | Authority, dependencies, boundaries và shared rules Local → Dedicated |
| [Gameplay Runtime](02-technical/gameplay-runtime.md) | Triển khai clock/input/combat/quest/physics/map/UI/presentation |
| [Online & Persistence](02-technical/online-and-persistence.md) | Auth/lease/reconnect/checkpoint, transactions/receipts, durability/outage/escrow |
| [Art & Visual Production](03-art/art-and-visual-production.md) | Perspective/rig/pose/socket, visual language, map Bible/UI, workflow/accounting |
| [Roadmap](04-production/roadmap.md) | CURRENT/TARGET/DEFERRED, phase/gate, phụ thuộc, lịch quản lý và readiness |
| [Playtest & Balance](04-production/playtest-and-balance.md) | Evidence/fixtures/probes/KPI, harness requirements và fresh-run protocol |

## Authority

Một current fact có một canonical owner. Design quyết định WHAT; Technical quyết định HOW trong luật design; Art quyết định hình ảnh; Roadmap quyết định WHEN; Playtest ghi evidence. Secondary documents chỉ dẫn nguồn, không tạo balance catalog hoặc authority thứ hai.

LOCKED là quyết định đã duyệt. STRONG DIRECTION là hướng rõ cần kiểm cách làm. BASELINE/TUNABLE là mốc để thử. OPEN là câu hỏi chưa chốt. PROPOSAL chưa phải luật. LEGACY/SUPERSEDED chỉ truy vết lịch sử; không dùng làm implementation requirement. P0/P1/P2 mô tả scope, không tự chứng minh hệ đã được triển khai.

Evidence trái design phải thành finding để owner quyết định, không silently sửa luật. Reference/prototype/external review không tự thành authority.

## Reading by role

| Vai trò | Đường đọc |
| --- | --- |
| Gameplay developer | README → design domain → Gameplay Runtime → Playtest khi cần evidence |
| Backend/network | README → Architecture → Online & Persistence → design owner liên quan |
| Level designer | README → World & Content → Quests & Narrative → Art |
| Artist | README → Art & Visual Production → relevant design constraints |
| QA | README → Roadmap → Playtest & Balance → design/technical owner của ca kiểm |

## Coding-agent reading protocol

1. Luôn đọc `docs/README.md` trước; đọc [Game Design](01-design/game-design.md) khi cần context gameplay tổng quan.
2. Đọc canonical DESIGN OWNER của feature; với task implementation, đọc TECHNICAL OWNER tương ứng theo routing bên dưới.
3. Đọc [Playtest & Balance](04-production/playtest-and-balance.md) khi cần evidence, tuning, acceptance hoặc probe; đọc [Art](03-art/art-and-visual-production.md) khi liên quan asset, presentation, UI visual, animation, socket hoặc terrain visual.
4. Không dùng `90-archive/` làm requirement hiện hành, trừ khi task explicitly yêu cầu historical context. Không tự promote OPEN/TUNABLE/PROPOSAL thành LOCKED.
5. Khi docs có vẻ mâu thuẫn, semantic owner thắng; nếu vẫn chưa rõ, report ambiguity, không invent rule.
6. Không cần đọc toàn bộ docs cho mọi task; follow routing/owner để giữ context focused.

## Quick routing

| Cần tra | Owner |
| --- | --- |
| Skill/MP/CD, target, input/pending | [Combat](01-design/combat-and-character.md) |
| Density/Hybrid/Linh/Boss (WHAT) | [World](01-design/world-and-content.md) |
| Return gameplay policy (WHAT) | [World](01-design/world-and-content.md#melee-crowd) |
| Return runtime/state machine (HOW) | [Runtime](02-technical/gameplay-runtime.md#mob-capabilities-vùng-hoạt-động-và-crowd) |
| Q8 gameplay guarantee/reservation policy (WHAT) | [World](01-design/world-and-content.md#q8-bounded-path) |
| Q8 quest credit (WHAT) | [Quests](01-design/quests-and-narrative.md#q8-credit-review) |
| Q8 runtime scheduling/lifecycle (HOW) | [Runtime](02-technical/gameplay-runtime.md#death-loot-spawn-và-action-timeline) |
| Q8 entitlement/reconnect/durable receipt (HOW) | [Online](02-technical/online-and-persistence.md#lưu-dữ-liệu--spring-boot--postgresql) |
| Objectives/NPC/evidence/credit | [Quests](01-design/quests-and-narrative.md) |
| Food/loot/gear/Gold (WHAT) | [Items](01-design/items-and-economy.md) |
| Potion gameplay/resource rule (WHAT) | [Items](01-design/items-and-economy.md#consumables-death) |
| Potion realtime application ordering (HOW) | [Runtime](02-technical/gameplay-runtime.md#potion-ordering) |
| Potion durability/crash/reconnect (HOW) | [Online](02-technical/online-and-persistence.md#potion-durability) |
| Perspective/26-frame interpretation/socket/accounting | [Art](03-art/art-and-visual-production.md) |
| Early Kiếm/Cung và production readiness | [Roadmap](04-production/roadmap.md), [protocol](04-production/playtest-and-balance.md#early-two-class-probe) |
| Old tables/prototype history | [Design history](90-archive/design-history.md), [Art history](90-archive/art-history.md), [Production history](90-archive/production-history.md) |

## Glossary

| Thuật ngữ | Nghĩa dùng trong docs |
| --- | --- |
| Authority | Thành phần có quyền quyết định một loại state/result |
| Snapshot | Bản chụp bất biến của data tại một thời điểm |
| Commit / rollback | Ghi thành công toàn transaction / hoàn tác transaction thất bại |
| Idempotent | Retry cùng ID không tạo effect/reward/consume lần nữa |
| Generation | Phiên/đời dùng để loại callback hoặc write cũ |
| CombatFocus | Target quan sát/ý định combat; không tự là một cast |
| Execution range | Tầm có thể thực thi skill tại validation/resolve |
| Retention range | Vùng còn giữ focus; khác tầm thực thi |
| PendingCast | Intent chờ tiếp cận/readiness, chưa là RunningAction |
| SpawnGroup / SpawnSlot | Cụm author độc lập / điểm spawn có identity ổn định |
| HomeRegion / WalkRegion | Vùng gốc của mob / địa hình mob được đi |
| Leash | Giới hạn truy đuổi neo vào home |
| One-way / solid terrain | Sàn cấu trúc có thể xuyên đúng thao tác / khối đặc không xuyên |
| Hitbox / hurtbox | Hình truy vấn đòn / vùng có thể nhận đòn |
| Action lock | Cửa đang chạy action, khác cooldown và độ dài VFX |
| Receipt / ACK | Kết quả durable để retry / xác nhận đã nhận hoặc commit theo contract |
| Entitlement | Quyền nhận/force theo event hợp lệ, giữ qua retry |
| Fresh-run | Hành trình từ trạng thái đầu hợp lệ, không dùng preset bỏ bước |

<a id="open-decision-index"></a>

## Open Decision Index

Index chỉ dẫn nơi quyết định; không giữ options hoặc gameplay values.

| ID | Status | Question | Owner | Resolve at |
| --- | --- | --- | --- | --- |
| INPUT-01 | OPEN/TUNABLE feel | Physical keys, shell, envelopes và approach/buffer feel? | [Combat](01-design/combat-and-character.md) | Pha R, G-L/G-N |
| CMB-01 / BAL-01 | TUNABLE | Cadence, class growth/build và gear-lag có hợp lý? | [Combat](01-design/combat-and-character.md) | Early probes, G-C/F |
| BAL-02 / GEAR-01 / LOOT-01 | TUNABLE | Food sustain, gear power và economic sinks? | [Items](01-design/items-and-economy.md) | Playtest/G-C/F |
| MOBAI-01 / SCOPE-01 | OPEN/TUNABLE | Hybrid, Return và final population authoring? | [World](01-design/world-and-content.md) | PHY-01/G-C/F |
| Q8-01 | LOCKED guarantee / OPEN mechanism | Bounds, fairness/credit và force entitlement recovery? | [World policy](01-design/world-and-content.md#q8-bounded-path), [credit](01-design/quests-and-narrative.md#q8-credit-review) | G-C/G-D, contention probe |
| QUEST-03 | PLAYTEST | Fresh route, narrative/usability và duration có đạt? | [Quest](01-design/quests-and-narrative.md), [evidence](04-production/playtest-and-balance.md) | G-L/C/T |
| POT-01 | LOCKED realtime / OPEN durability | Accepted Potion survive crash/outage thế nào? | [Online](02-technical/online-and-persistence.md#potion-durability) | G-D/G-T bắt buộc |
| TECH-01 / SAVE-01 | SPIKE | Auth/lease/checkpoint/reconnect/recovery có nhất quán? | [Online](02-technical/online-and-persistence.md) | G-D |
| A01 / A02 / A17 | OPEN | Raster/frame meaning, technique và real accounting? | [Art](03-art/art-and-visual-production.md#art-open-decisions) | Early probes/P15 |
| A03 | BASELINE | Stat-only visual binding? | [Items](01-design/items-and-economy.md#a03) | Gear/UI integration |
| A04 / A08 / A09 / A12 / A16 | OPEN visual details | Motif/corpse/kit/camera/Boss pose? | [Art](03-art/art-and-visual-production.md#art-open-decisions) | Pha R/P01–P15 |
| A05 / A06 / A14 | OPEN/TUNABLE details | Ranged timing, role và air policy? | [Combat](01-design/combat-and-character.md#a05) | Pha R/G-C |
| A07 / A10 | OPEN details | Dummy/content/LoS/Return authoring? | [World](01-design/world-and-content.md#a07) | PHY-01/G-C |
| A11 | OPEN presentation | Cosmetic anticipation/schema? | [Runtime](02-technical/gameplay-runtime.md#a11) | G-N |
| A13 / A15 | OPEN details | Select preview và terminal/persistence ordering? | [Online](02-technical/online-and-persistence.md#a13) | G-D/P12 |
