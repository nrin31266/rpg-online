# Huyền Lộ — Documentation Map

Entry point cho **11 canonical docs** (kể cả index), tổ chức theo semantic owner; architecture và runtime dùng chung file với section ownership rõ. Trạng thái triển khai hiện hành xem tại [Roadmap](04-production/roadmap.md). Prototype VS-1 đã được xóa khỏi checkout; [lịch sử và source đã commit](90-archive/production-history.md#prototype-source-retired) vẫn tra được qua Git. Kết quả cũ không tự chứng minh current acceptance.

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
| [Combat & Character](01-design/combat-and-character.md) | Stats/class/skill/combat focus branch/pending/damage/status và combat modifiers |
| [World & Content](01-design/world-and-content.md) | Map/gate/topology/terrain, mob/AI gameplay, density/spawn, Linh Biến/Boss |
| [Quests & Narrative](01-design/quests-and-narrative.md) | Lore/NPC/dialogue, canonical objectives và quest credit/gameplay recovery |
| [Items & Economy](01-design/items-and-economy.md) | Gear/Food/Potion, inventory/shop/loot, Gold/Journey và PvP stake economy |
| [Gameplay Runtime](02-technical/gameplay-runtime.md) | Architecture/authority/Local → Dedicated; PrimaryAction/ActiveFocus/input/UI; clock/combat/quest/physics/map/presentation implementation |
| [Online & Persistence](02-technical/online-and-persistence.md) | Auth/lease/reconnect/checkpoint, transactions/receipts, durability/outage/escrow |
| [Art & Visual Production](03-art/art-and-visual-production.md) | Master Pose/visual-part authoring/composition, perspective/rig/pose/socket, visual language, map Bible/UI, workflow/accounting |
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
| Backend/network | README → Runtime architecture/boundaries → Online & Persistence → design owner liên quan |
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
| Architecture/authority/Local → Dedicated | [Runtime architecture](02-technical/gameplay-runtime.md#runtime), [discipline](02-technical/gameplay-runtime.md#architecture-discipline) |
| Code responsibilities / reuse / failure boundaries | [Runtime matrix](02-technical/gameplay-runtime.md#responsibility-matrix) |
| PrimaryAction / universal ActiveFocus / NPC menu | [Runtime](02-technical/gameplay-runtime.md#active-focus) |
| Quest RNG / physical collection | [Quest](01-design/quests-and-narrative.md#quest-collection) |
| Q11 restoration / level and endpoint OPEN | [Quest](01-design/quests-and-narrative.md#q11-restoration) |
| One regular physical outcome | [Items](01-design/items-and-economy.md#regular-loot-outcome) |
| Inventory Split/Sort-Merge/Discard policy | [Items](01-design/items-and-economy.md#inventory-ux-policy) |
| Map Info population / replication | [World](01-design/world-and-content.md#map-population-info), [Runtime](02-technical/gameplay-runtime.md#map-info-runtime) |
| Skill/MP/CD, combat target/range/pending | [Combat](01-design/combat-and-character.md) |
| Density/Hybrid/Linh/Boss (WHAT) | [World](01-design/world-and-content.md) |
| Return gameplay policy (WHAT) | [World](01-design/world-and-content.md#melee-crowd) |
| Return runtime/state machine (HOW) | [Runtime](02-technical/gameplay-runtime.md#mob-capabilities-vùng-hoạt-động-và-crowd) |
| Q8 natural hunt/spawn availability (WHAT) | [World](01-design/world-and-content.md#q8-bounded-path) |
| Q8 quest credit (WHAT) | [Quests](01-design/quests-and-narrative.md#q8-credit-review) |
| Natural Linh runtime lifecycle (HOW) | [Runtime](02-technical/gameplay-runtime.md#death-loot-spawn-và-action-timeline) |
| Q8 entitlement/reconnect/durable receipt (HOW) | [Online](02-technical/online-and-persistence.md#lưu-dữ-liệu--spring-boot--postgresql) |
| Objectives/NPC/evidence/credit | [Quests](01-design/quests-and-narrative.md) |
| Food/loot/gear/Gold (WHAT) | [Items](01-design/items-and-economy.md) |
| Potion gameplay/resource rule (WHAT) | [Items](01-design/items-and-economy.md#consumables-death) |
| Potion realtime application ordering (HOW) | [Runtime](02-technical/gameplay-runtime.md#potion-ordering) |
| Potion durability/crash/reconnect (HOW) | [Online](02-technical/online-and-persistence.md#potion-durability) |
| Master Pose / modular parts / 26 logical mapping / socket / accounting | [Art](03-art/art-and-visual-production.md#master-pose), [runtime integration](02-technical/gameplay-runtime.md#art-contract), [probe acceptance](04-production/playtest-and-balance.md#modular-character-acceptance) |
| Early Kiếm/Cung và production readiness | [Roadmap](04-production/roadmap.md), [protocol](04-production/playtest-and-balance.md#early-two-class-probe) |
| Old tables/prototype history | [Design history](90-archive/design-history.md), [Art history](90-archive/art-history.md), [Production history](90-archive/production-history.md) |

## Glossary

| Thuật ngữ | Nghĩa dùng trong docs |
| --- | --- |
| Authority | Thành phần có quyền quyết định một loại state/result |
| Master Pose Template / Schema | Reference authoring và contract dữ liệu pose/anchors dùng chung; không body render layer |
| Pose-indexed visual set | Head/Hair, Upper/Armor, Lower/Pants hoặc Weapon được chọn/căn theo logical pose; default Upper/Lower dùng khi unequip |
| Logical pose / raster | Mẫu trên timeline / ảnh pixel xuất; 26 logical poses LOCKED không đồng nghĩa 26 unique rasters mỗi item |
| UI icon / character visual | Hai asset roles riêng: biểu diễn item trong UI / fragments ghép actor trong world |
| Snapshot | Bản chụp bất biến của data tại một thời điểm |
| Commit / rollback | Ghi thành công toàn transaction / hoàn tác transaction thất bại |
| Idempotent | Retry cùng ID không tạo effect/reward/consume lần nữa |
| Generation | Phiên/đời dùng để loại callback hoặc write cũ |
| PrimaryAction | Một player-facing action dispatch theo ActiveFocus thành concrete server command |
| ActiveFocus | Một object/type actionable; combat branch có CombatFocus machinery, không double selected marker |
| Quest RNG outcome | Success/failure immutable theo eligible death/recipient/objective/epoch; recovery không reroll |
| Restoration flag | Action history đã commit sau consume đúng fragment, không possession counter |
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
| Entitlement | Quyền nhận item theo event/grant hợp lệ, giữ qua retry; không quest force-spawn |
| Fresh-run | Hành trình từ trạng thái đầu hợp lệ, không dùng preset bỏ bước |

<a id="open-decision-index"></a>

## Open Decision Index

Index chỉ dẫn nơi quyết định; không giữ options hoặc gameplay values.

| ID | Status | Question | Owner | Resolve at |
| --- | --- | --- | --- | --- |
| INPUT-01 | OPEN/TUNABLE feel | Physical keys, shell, envelopes và approach/buffer feel? | [Combat](01-design/combat-and-character.md), [Runtime](02-technical/gameplay-runtime.md#active-focus) | Pha R, G-L/G-N |
| CMB-01 / BAL-01 | TUNABLE | Cadence, class growth/build và gear-lag có hợp lý? | [Combat](01-design/combat-and-character.md) | Early probes, G-C/F |
| BAL-02 / GEAR-01 / LOOT-01 | TUNABLE | Food sustain, gear power và economic sinks? | [Items](01-design/items-and-economy.md) | Playtest/G-C/F |
| MOBAI-01 / SCOPE-01 | OPEN/TUNABLE | Hybrid, Return và final population authoring? | [World](01-design/world-and-content.md) | PHY-01/G-C/F |
| Q8-01 | CURRENT natural / TUNABLE pacing | Rare identity availability, shared cap và contribution contention? | [World](01-design/world-and-content.md#q8-bounded-path), [Quest](01-design/quests-and-narrative.md#q8-credit-review) | G-C/QUEST-03 |
| QUEST-03 | PLAYTEST | Fresh route, narrative/usability và duration có đạt? | [Quest](01-design/quests-and-narrative.md), [evidence](04-production/playtest-and-balance.md) | G-L/C/T |
| POT-01 | LOCKED realtime / OPEN durability | Accepted Potion survive crash/outage thế nào? | [Online](02-technical/online-and-persistence.md#potion-durability) | G-D/G-T bắt buộc |
| TECH-01 / SAVE-01 | SPIKE | Auth/lease/checkpoint/reconnect/recovery có nhất quán? | [Online](02-technical/online-and-persistence.md) | G-D |
| A01 / A02 / A17 | OPEN | Raster mapping trong26 logical frames, technique và real accounting? | [Art](03-art/art-and-visual-production.md#art-open-decisions) | Early probes/P15 |
| A03 | BASELINE | Stat-only visual binding? | [Items](01-design/items-and-economy.md#a03) | Gear/UI integration |
| A04 / A08 / A09 / A12 / A16 | OPEN visual details | Motif/corpse/kit/camera/Boss pose? | [Art](03-art/art-and-visual-production.md#art-open-decisions) | Pha R/P01–P15 |
| A05 / A06 / A14 | OPEN/TUNABLE details | Ranged timing, role và air policy? | [Combat](01-design/combat-and-character.md#a05) | Pha R/G-C |
| A07 / A10 | OPEN details | Dummy/content/LoS/Return authoring? | [World](01-design/world-and-content.md#a07) | PHY-01/G-C |
| A11 | OPEN presentation | Cosmetic anticipation/schema? | [Runtime](02-technical/gameplay-runtime.md#a11) | G-N |
| A13 / A15 | OPEN details | Select preview và terminal/persistence ordering? | [Online](02-technical/online-and-persistence.md#a13) | G-D/P12 |
| FOCUS-02 | OPEN | Auto non-combat sticky/priority và dead new acquisition? | [Runtime](02-technical/gameplay-runtime.md#active-focus) | C0/G-L/N |
| QUEST-RNG | TUNABLE / OPEN | Collection quantities/rates và natural-hunt pacing? | [Quest](01-design/quests-and-narrative.md#quest-collection), [Q8 probe](04-production/playtest-and-balance.md#q8-01--natural-hunt--contribution--population-chưa-chạy) | QUEST-03/G-C/T |
| Q11 | CURRENT direction / NEEDS USER LOCK | Start/gate, material/consume, level, maps/positions/order, endpoint/final gate action? | [Quest](01-design/quests-and-narrative.md#q11-restoration) | G-B/content review trước authoring |
| LOOT-02 | TUNABLE / OPEN | One-outcome weights/income và Linh Gold rounding? | [Items](01-design/items-and-economy.md#regular-loot-outcome), [EV](04-production/playtest-and-balance.md#loot-model-transition) | G-C/F |
| INV-UX | CURRENT planned / OPEN scope | Ordinary canDiscard và Split/Sort/Discard shipment P0/P1? | [Items](01-design/items-and-economy.md#inventory-ux-policy), [Roadmap](04-production/roadmap.md#map-info-work-package) | Slice1/3/4; scope review |
| MAP-INFO | CURRENT direction / ENGINEERING recommendation / OPEN visual | Count encoding/lifecycle/replication và labels/layout? | [World](01-design/world-and-content.md#map-population-info), [Runtime](02-technical/gameplay-runtime.md#map-info-runtime), [Art](03-art/art-and-visual-production.md#activefocus--map-info--inventory-visual-sync--current-direction) | Slice5/8/G-N |
| RNG-DURABILITY | OPEN engineering spike | Schema/retention/TTL, pre-handoff crash và staged batch delivery? | [Online](02-technical/online-and-persistence.md#identity-death-receipts) | G-D |
| Q2 / Q6 | PROPOSAL chưa áp dụng | Exact journey content và MP tutorial reschedule? | [Q2](01-design/quests-and-narrative.md#q2-journey), [Q6](01-design/quests-and-narrative.md#q6-mp-tutorial--recovery-hiện-hành-và-proposal-dời-thời-điểm) | Content approval/G-L |

## History

[Combat decision trace](90-archive/design-history.md#combat-decision-trace), [source audit 2026-10-09](90-archive/production-history.md#source-audit-20261009) và [prototype history](90-archive/production-history.md#prototype-runtime-history) chỉ dùng truy vết. CURRENT/gate status xem [Roadmap](04-production/roadmap.md#target-current-deferred); chưa có runtime evidence mới từ lượt docs.
