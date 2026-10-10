# Huyền Lộ — Gameplay Runtime

Các số BASELINE/TUNABLE chưa phải nghiệm thu runtime.

## Document owns

Authority/dependency và Local → Dedicated; cách triển khai clock/input/combat/quest/physics/map/UI/presentation. Combat/quest/items/world owners giữ gameplay và balance; Online giữ durability/recovery.

## Document does not own

Luật ngoài domain thuộc owner trong [documentation map](../README.md); evidence thuộc [Playtest & Balance](../04-production/playtest-and-balance.md), thứ tự triển khai thuộc [Roadmap](../04-production/roadmap.md).

## Tra cứu trong tài liệu

[Architecture](#runtime) · [Local → Dedicated](#architecture-discipline) · [Responsibility matrix](#responsibility-matrix) · [Maps/physics](#maps) · [Combat data](#combat-data) · [ActiveFocus/input](#active-focus) · [Map Info](#map-info-runtime) · [Presentation](#presentation-data) · [UI](#ui-notes) · [Command flows](#item-transaction-flows).

## Architecture và ranh giới hệ thống

<a id="runtime"></a>

<a id="1-kiến-trúc-runtime"></a>

## Kiến trúc runtime

P0 chạy **một Unity Dedicated Game Server** headless, **một Spring Boot backend**, **một PostgreSQL** và N Unity 2D Clients. Hai Client là mức nghiệm thu tối thiểu, không phải `MaxPlayers = 2`. Không có player-host, Party service, shard, cloud orchestration hoặc engine combat trong Java.

| Thành phần | Sở hữu | Không sở hữu |
| --- | --- | --- |
| Unity Client | Input, camera, UI, animation, VFX, audio, interpolation; gọi login/character list/ticket qua backend và gửi intent gameplay tới Game Server | Damage, HP mục tiêu, EXP/Vàng, loot, quest/enhance/Boss/PvP result; không gửi character state đáng tin cậy |
| Unity Dedicated Game Server | Physics2D, movement/map validation, skill timeline, combat/status, HP/MP trong phiên, mob/Boss/PvP, contribution/threat, roll và phân phối kết quả gameplay; kiểm intent theo character binding | Account/password, SQL, bản lưu tiến trình dài hạn |
| Spring Boot | Admin tạo account, login, character list, session/ticket, character aggregate; giao dịch bền vững và idempotency cho progression/inventory/quest/loot/Gold/Journey | Physics/combat tick, AI, chọn mục tiêu hoặc roll lại kết quả gameplay |
| PostgreSQL | Account/password hash, tiến trình nhân vật, recovery checkpoint, PvP escrow/settlement receipts và ground loot còn hiệu lực | Static ScriptableObject definitions, projectile/AI/threat hoặc combat state chính xác trong phiên |

**Luồng kết nối:** Client ↔ Spring Boot để login, chọn nhân vật, lấy game ticket; Client ↔ Game Server qua NGO + Unity Transport cho realtime; Game Server ↔ Spring Boot qua internal HTTP API có service credential; Spring Boot ↔ PostgreSQL. Client không gọi backend để cộng thưởng hoặc hoàn thành quest. Backend không nằm trên đường mỗi frame/hit; kết quả làm thay đổi tiến trình bền vững chỉ được báo thành công sau khi commit (xác nhận thay đổi chính thức).

**Một nguồn luật:** Skill/Mob/Item/Quest/Map definitions là ScriptableObject với stable IDs, đóng gói cùng revision vào Game Server build. Client nhận phần cần hiển thị. Spring giữ định danh, definition revision và constraint dữ liệu/giao dịch tối thiểu; không chép damage/drop/enhance thành engine thứ hai trong Java. Riêng escrow/payout/fee/refund PvP do Spring tính theo owner design tương ứng và các mục liên quan bên dưới.

Game Server tính gameplay result từ definition; backend chỉ nhận lệnh từ service credential, kiểm session/IDs, expected character revision, idempotency key và cấu trúc giao dịch rồi commit atomic (toàn bộ cùng thành công hoặc cùng thất bại). Definition revision lệch thì từ chối join/mutation cho tới khi đồng bộ. PostgreSQL migrations giữ schema; JSON chỉ cho config/fixture/import-export dev, không là save authority.

Physics 50 Hz, network 20 Hz và render 60 FPS là BASELINE/TUNABLE, cần profiler trước khi hứa throughput. RPC kiểm sender/binding rồi gọi domain function; không custom transport adapter, DI/service bus hoặc distributed messaging. Session admission theo config, độc lập với gameplay; collections theo characterId/playerId hỗ trợ N người. PvP MatchId có đúng hai participant vì mode 1v1. Dedicated build dùng cùng gameplay assembly/definitions với Client; assembly/build target tách presentation, server bỏ camera/UI/audio và chạy headless.

**Unity/tooling:** pin Editor/ProjectVersion/manifest/lock khi dựng production base và kiểm package ở integration gate. Input System cho Client; NGO + Unity Transport là lựa chọn TARGET realtime, chưa có trong prototype. Multiplayer Play Mode (MPPM), Multiplayer Tools/Network Simulator và Unity Test Framework phục vụ dev/QA; ObjectPool chỉ quản lý presentation; Cinemachine 3 cho camera Client.

Local Session ở các mục liên quan phục vụ slice đầu, Dedicated phục vụ gate mạng/final online acceptance. MPPM giúp lặp với nhiều Client, không thay standalone acceptance. Tránh DOTS/ECS, Addressables, Relay, prediction/rollback và cloud/service framework nếu slice chưa chứng minh cần.

<a id="architecture-discipline"></a>

<a id="11-kỷ-luật-kiến-trúc-và-local--dedicated"></a>

## Kỷ luật kiến trúc và Local → Dedicated

**Ranh giới production base đã được chấp nhận; prototype classes/folders không là implementation authority.** Input/UI tạo intent; authority của phiên kiểm binding/state rồi gọi rules/resolver; resolver trả result/state change; presentation đọc trạng thái để vẽ. Chỉ một session giữ quyền thay đổi gameplay trong một lần chạy. UI/PlayerScript không tự sửa HP quái, inventory, quest hoặc EXP.

```text
Input / UI → Intent → Authority của phiên → Rules / Resolver
                                            ↓
                                   Result / State → Presentation
Local: intent gọi session trong process.
Dedicated: intent qua RPC đã kiểm sender/character/session.
```

Giữ ít abstraction: một điểm nhận intent, một clock gameplay, definitions có revision và một điểm commit progression/receipt. Đây là trách nhiệm cần tách, không buộc tên class/interface hay service framework. Local/Dedicated dùng cùng gameplay assembly cho combat/stat/quest/reward. Reuse code prototype phải review/test theo contract mới; không mang nguyên assembly cũ vào production. Authority chạy physics adapter;

MonoBehaviour có thể tích hợp physics, nhưng UI/animation không sở hữu luật. Resolver nhận state/definition/clock và trả kết quả dễ kiểm, không cần Text/Button/Animator/RPC/SQL để tính damage.

| Ranh giới | Local slice / fixture dev | Dedicated + backend TARGET |
| --- | --- | --- |
| Nhận intent | Gọi session trong process, bind actor fixture rõ | RPC kiểm sender/character/session rồi gọi cùng domain path; collections N-player |
| Simulation | Local Session tick physics/AI/timeline và sửa state | Dedicated tick headless; Client đọc state/interpolation, không chạy authority thứ hai |
| Definition/result | Stable IDs/revision, stat/quest/loot resolver, action/life IDs | Giữ cùng ý nghĩa; protocol serialization là adapter, không bản công thức riêng |
| Commit progression | Adapter RAM có receipt/revision; inject pending/reject/retry để thử luồng; mất khi đóng phiên | Spring/PostgreSQL theo các mục liên quan; chỉ ACK bền vững mới báo persistent success |
| Admission/recovery | Profile dev/reset rõ, chưa chứng minh login/save/reconnect | Login/Select/ticket/lease/checkpoint/escrow và outage theo các mục liên quan |

UI/modal/selection triển khai tại [UI](#ui-notes) và [shared item controllers](#shared-item-ui); NPC/class admission theo [Quest owner](../01-design/quests-and-narrative.md#npc-service-review).

**ID ổn định:** Skill/Item/Quest/Map/group/slot IDs thuộc definition, không đổi theo thứ tự Inspector/list hoặc display name. Runtime instanceID/actionID/generation phân biệt một đời instance và một action; không dùng GameObject instance hoặc sprite frame làm business identity. Callback từ đời cũ bị từ chối. Client sequence/correlation không thay authoritative result identity.

**Một clock gameplay:** hit/spawn, CD/action lock, status/tick, AI và due-slot dùng cùng timebase. Animator/UI không có timer quyết gameplay riêng; parts của actor đọc cùng state/phase. Deadline bền vững UTC theo các mục liên quan được quy đổi rõ với thời gian phiên; backend giữ timestamp giao dịch/reconciler riêng. AnimationEvent chỉ phát feedback cosmetic: bỏ frame/event không sinh, mất hoặc lặp damage. VFX collision/socket/render bounds không là hitbox authoritative. Hitstop/crit shake/material audio DEFERRED, không dừng clock gameplay.

**Đổi sang Dedicated:** thay adapter nhận intent, physics host/replication, admission/commit; giữ rules và ý nghĩa result. G-N kiểm ít nhất hai Client trước nhân content/art; fixture RAM không chứng minh durability (dữ liệu còn sau lỗi/crash). G-D dùng service credential/PostgreSQL thật và kiểm transaction/recovery trước nhận persistence done. Message fields ở [presentation proposal](#presentation-data) cần spike, không buộc custom bus từ local.

Task của coding agent phải nêu canonical section/revision, CURRENT/TARGET, input/result và gate liên quan. Thay số gameplay/timer/Boots/26-frame hoặc quyết định OPEN phải ghi giả định/evidence ở Playtest & Balance và đồng bộ owner. Không tự thêm movement lock/state/feature để làm art/count/test pass. Chọn phép kiểm có ý nghĩa cho luật/retry/life/physics; sửa visual nhẹ không cần test chỉ phản chiếu implementation.


<a id="responsibility-matrix"></a>

## Responsibility matrix — TARGET boundaries

Owner là nơi đặt code/trách nhiệm. Dedicated Server giữ realtime gameplay authority; Spring/PostgreSQL giữ durable commit theo contract hiện hành. Local dùng cùng domain path với RAM adapter. Không bắt mỗi row thành service/interface; Reused by không cho bypass validation/transaction.

| System / Component | Responsibility | Owner | Authority | Persistent state | Reads | Writes | Reused by | Must not depend on | Failure boundary | Idempotency requirement |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Client Inventory UI | Bag actions/pending | Client controller | Display/intent | Không | Bag snapshot/policy reasons | Selection/commands | Equipment picker | SQL/trusted mutations | Stale entry/reject | Correlate command; không double activate |
| Client ItemGrid | Cells/navigation/scroll | Client widget | Display | Không | Entry viewmodel | Local focus | Bag/Buy/Sell/Storage/picker | NPC/quest rules | Missing entry/layout | Stable entry selection |
| Client ItemDetails | Stats/quantity/requirements/reason | Widget/formatter | Display | Không | Selected entry/allowed actions | Local view | Item contexts | Damage/drop/price authority | Stale snapshot | Latest revision |
| Client Shop UI | Tabs/catalog/quantity/request | Client controller | Intent | Không | Catalog/bag/results | Buy/Sell intent | NPC service | Inventory write/trusted Gold | Timeout pending | Một command, retry cùng ID |
| Client NPC UI | Talk/quest/service/breadcrumb | Client controller | Display/intent | Không | NPC/quest/class result | Interaction intent | Quest/Shop/Storage | Grant từ thoại | Wrong NPC/range | Không UI credit/replay grant |
| Client Ground visual | Owner-filtered icon/pick cue | Client presenter | Display | Không | Rep ID/gen/map/owner | Cosmetic/pick intent | Regular/personal visual | ACL/claim logic | Late/stale/wrong map | Dedup/retire representation |
| Client Quest tracker | Action/possession feedback | Client presenter | Display | Không | Committed progress/bag/rights | Pin/local cue | NPC/HUD | Completion từ text | Pending vs committed | Latest revision; không event credit |
| Client Character presentation | Pose/parts/carry/shadow | Client presenter | Visual | Không | Facing/action/life/gear | VisualRoot | Preview/world | Physics damage/Inventory | Stale action/death | Shared clock/life reset |
| Client Movement input | Axis/press/semantic input | Input adapter | Intent | Không | Bindings/UI capture | Input sequence | Local/Dedicated receiver | Hold height/trusted position | Modal/stale press | Một accepted press/jump |
| Server Player Inventory | Plan add/remove/merge/capacity | Domain planner/session | Gameplay | Stack/location/quantity | Definitions/bag/revision | Inventory delta | Buy/Sell/Pickup/TurnIn/Storage | UI/RPC/SQL details | Capacity/overflow/stale | Atomic plan/commit receipt |
| Server Item ownership | Location/binding policy | Validator/data | Gameplay | Binding/location | Instance/session/quest policy | Validation result | Item handlers | Client visibility | Wrong owner/quest | Reject không mutation |
| Server Equipment | Equip/unequip/stat projection | Handler/evaluator | Gameplay | Slots/instance state | Bag/class/level | Equip/bag delta | Character/quest | Art pose/DB formulas | Full bag/off-class/policy | Atomic swap/command receipt |
| Server Loot eligibility | Recipient/window predicates | Domain validator | Gameplay | Eligibility snapshot | Ledger/level-at-death/source | Eligibility result | Regular/personal handlers | Visibility/last-hit shortcut | Wrong identity/special predicate/life | Snapshot một lần/death |
| Server Ground loot | Valid ground/lifetime/representation | Runtime manager/data | Gameplay | Regular pile/deadline; personal right riêng | Map/surface/entitlement | Generation/position/retire | Pickup/visual | Trusted client raycast | TTL/unreachable/stale gen | Một active personal rep/right |
| Server Personal quest entitlement | Bounded right/payload | Quest handler | Gameplay; durable via Spring | Pending/Claimed/Consumed/epoch/receipt | Active group/source outcome/eligibility | Entitlement delta | Death/placed/recovery | Regular owner budget/UI | Death rollback/restart | Unique death/recipient/objective/epoch hoặc staged grant; cả failure dedup |
| Server Pickup validation | Alive/map/range/ACL/capacity | Pickup handler | Gameplay | Claim receipt | Ground/right/bag/revision | Add+claim delta | Regular/personal pickup | Client claimed state | Race/full bag/stale rep | Add+claim atomic |
| Server Quest progress | History và derive possession | Evaluator/session | Gameplay | Per-Quest state/RNG outcomes/tutorial ordinal/receipts/placed flags | Definitions/bag/committed events | Progress/Ready cache | NPC/tracker/turn-in | UI counter/double collection authority | Wrong group/missing items | Event ID/source epoch |
| Server Shop transaction | Catalog/price/quantity/Gold plan | Buy/Sell handlers | Gameplay | Gold/bag/receipts | Catalog/NPC/bag | Purchase/sale delta | Shop commands | Client price/quest grant engine | Overflow/vendor/timeout | Immutable command payload |
| Server NPC interaction result | Resolve context/admission/NPC | Handler/data table | Gameplay | Q6 Talk/class/mentor receipts | NPC/quest/class | Talk/choice/result delta | Quest/service entry | Animation/ambient grant | Range/off-class | Talk gate/class receipt |
| Server Combat/mob credit | HpLost/active ledger/death snapshot | Combat resolver/session | Realtime | Death reward/progress receipts | Mob identity/variant/life/contribution/active quest | Immutable death result | EXP/loot/entitlement | Spring damage engine/VFX | Terminal pending/stale life | Death ID/recipient dedup |
| Spring Durable profile | Committed character aggregate | Backend application | Durable | Profile/bag/quest/class/checkpoint | Auth/DB/definitions | Validated durable delta | Join/commands | Physics/targets/RNG reroll | Revision/lease/credentials | Receipt trước revision |
| Spring Transaction/revision | Atomic shared-result commit | Backend transaction | Durable | Receipts/revisions/claims | Immutable plan/current rows | Related rows một lần | Buy/Sell/claim/turn-in/death N | Trusted client result | Rollback/unknown commit | Unique ID/payload; stable lock order |
| Spring Recovery/lease | Restore/one writer/fencing | Backend admission | Durable admission | Lease/generation/receipt/checkpoint | Lease/committed state | Ticket/lease/reconcile | Rejoin/restart/escrow | Client RAM save | Duplicate session/outage | One-time ticket/reject old writer |
| PostgreSQL | ACID constraints/storage | DB/schema owner | Durable storage | Committed records | Backend transactions | Rows/indexes/constraints | Persistence | Unity tick/UI | Startup/migration failure | Unique IDs/transaction constraints |

**Reuse:** inventory planner, item-policy validators, commit seam, UI widgets/formatter và valid-ground resolver. **Separate:** Buy/Sell/Equip/Storage/Pickup/TurnIn handlers vì admission và atomic deltas khác; regular pile/personal right khác ownership/expiry/recovery dù dùng chung visual. Collection evaluator đọc Inventory; action receipts giữ history, không event bus cho item counters. NPC chỉ cần data table/contextual handler. Spring dùng transaction/receipt boundary chung, không service cho mỗi item noun. Shared views nhận explicit viewmodel, tránh giant screen chứa business rules.


<a id="primary-action-responsibilities"></a>

## Responsibilities sau PrimaryAction / RNG / population migration

| Boundary | Responsibility | Không suy authority từ presentation |
| --- | --- | --- |
| Client | Sample input; một ActiveFocus/resolver/marker; PrimaryAction → concrete intent; NPC context/Map Info/HUD render | Không credit quest từ mở menu, không trusted target validity/RNG/counts/price |
| Game Server | Validate ExecuteSkill/PickupItem/OpenNpcContext/NpcAction/LandmarkInteract; eligibility, death RNG outcomes, one regular non-boss result, staged grant/placement plan; world population/revisions | Không một opaque network PrimaryAction; client selection không đủ rights |
| Durable backend | Trusted server results + lease/fencing/revisions/constraints; atomic receipts/Inventory/outcomes/entitlements/placed flags/unlock, recovery | Không reroll RNG, không realtime combat hoặc map count từ client |

Runtime owns [universal ActiveFocus](#active-focus); Combat owns combat branch/range/timeline. Q11 completion policy/material consume/level/maps còn OPEN tại Quest owner, không architecture lock bằng default schema. Inventory planners reuse cho Buy/Sell/Split/Merge/Discard/claim, handlers riêng admission; map population observer gắn lifecycle SpawnManager, snapshot/delta qua session/map replication, không subsystem persistence đếm renderer. [Online](online-and-persistence.md#identity-death-receipts) giữ durability boundary.

**Map Chat:** Enter mở input / gửi, tối đa 80 ký tự; rate 1 message / 2 s mỗi playerId do Game Server kiểm; bubble trên đầu tối đa hai dòng, 4 s rồi fade 0,5 s, cùng MapId. P0 không history lớn. System banner do Game Server phát cho Boss T−60, spawn / death và chapter completion; P1 có thể thêm history nhỏ.



<a id="maps"></a>

<a id="2-scene-và-map-trong-unity"></a>

## Scene và map trong Unity

Code/tọa độ/lỗi của prototype nằm tại [Roadmap — lịch sử](../90-archive/production-history.md#prototype-technical-history), không là contract production. Local production slice dự kiến vào World với profile dev và ba roots theo [phạm vi local slice](../04-production/roadmap.md#vs-1); session vẫn kiểm MapId/transition. Login không bắt buộc cho combat probe đầu; luồng online TARGET là Boot/Main Menu → Login → CharacterSelect → overlay kết nối → WorldOnline tại recovery map/SafeAnchor.

Một world scene có **8 logical map roots**: Village, Academy, Arena và năm farm maps. Root origins tính từ bounds thật và separation margin; ~200 u chỉ là mốc spike, không fixed spacing/width. Kiểm colliders/bounds không overlap khi đổi dimensions. Mỗi farm/combat map có một SafeAnchor cố định; Vân Khê/Học Viện dùng SafeAnchor fallback nếu checkpoint coordinate không hợp lệ. Arena dùng spawn theo MatchId, không là recovery map.

`MapDefinition` giữ MapId/bounds/spawn point/exit links/environment family/SpawnGroups/unlock data. World graph giữ các map nối nhau; internal map graph giữ nhánh, loop, tuyến cao/thấp, hốc/mỏm và đường jump/drop. Exit là vùng đã author ở nhánh phù hợp, không mặc định cuối bên phải. Asset/root đã tồn tại không chứng minh nhân vật được vào.

| Hợp đồng | P0 | P1 |
| --- | --- | --- |
| Map identity | Game Server giữ MapId và kiểm combat/chat/loot/transition | Không thay bằng scene visibility |
| Presentation | Camera/render filter chỉ root hiện hành | Streaming nếu profiler chứng minh cần |
| Network observers | Có thể còn replicate khác map; Client không render | NetworkHide/NetworkShow sau correctness |
| Map transition | EdgeExit overlap từ manual movement; SpecialGate activation riêng; authority kiểm nguồn/state/unlock rồi đổi MapId/position | Loading polish |
| Empty root | Có thể pause AI; timers vẫn thuộc global manager | Interest optimization |

### MapRoot, địa hình và collision

Mỗi `MapRoot` là một prefab/GameObject gốc đại diện cho một bản đồ logic trong World Scene, bao gồm hệ thống Tilemap phân tầng chuẩn tắc, structural visuals, các mặt collision vật lý, `SpawnPoints`, `SafeAnchor`, `MapExits/SpecialGates`, vùng nước nông và các mốc neo NPC (`NPC anchors`). Nước có thể có visual overlay riêng; tuyệt đối không để visual sprite tự động sinh collider ngẫu nhiên:

| Tầng Tilemap / Component | Layer & Phân loại vật lý | Cấu hình Collider & Quy chuẩn kỹ thuật |
| --- | --- | --- |
| **GroundTilemap** | Layer `SolidGround` (Đất/đá tự nhiên và khối kiến trúc đặc) | `TilemapCollider2D` + `CompositeCollider2D` ([Unity 6 Composite Operation = Merge](https://docs.unity3d.com/6000.3/Documentation/Manual/tilemaps/work-with-tilemaps/tilemap-collider-2d-reference.html)) + `Rigidbody2D Static`. Chỉ gồm các mặt phẳng ngang và mặt đứng trực giao. Cản trở hai chiều tuyệt đối; không có dốc xoay hay mặt chéo. |
| **PlatformTilemap** | Layer `OneWayPlatform` (Sàn mỏng nhân tạo: ván gỗ, giàn tre, ban công) | `TilemapCollider2D` + `PlatformEffector2D` (Bật `Use One Way`, góc bề mặt 0°); nếu dùng Composite thì `UsedByEffector` do CompositeCollider điều khiển. Cho phép nhảy từ dưới lên và bấm `↓` (`DropThrough`) để rơi xuống. |
| **WaterRegion** | Layer `Water` (Vùng nước nông ven suối/hồ) | `BoxCollider2D` thiết lập `Is Trigger`. Script `WaterSlowdownTrigger` kiểm tra va chạm của chân nhân vật (`feet contact` với GroundCheck); khi tiếp xúc sẽ áp dụng hệ số giảm tốc chạy nhẹ (TUNABLE); khi nhảy trên không hoặc đi trên cầu gỗ bắc ngang thì không kích hoạt trigger. |
| **BackgroundTilemap** | Sorting Layer `Background` / `MidBackground` | Hoàn toàn **không có Collider**. Độ tương phản và bão hòa màu thấp hơn các lớp chơi để tạo chiều sâu không gian. |
| **ForegroundTilemap** | Sorting Layer `Foreground` (Cành cây, mỏm đá viền mép màn hình) | Hoàn toàn **không có Collider**. Thiết lập độ che khuất (occlusion) cẩn thận, tuyệt đối không che khuất nameplate, target marker, thanh máu quái, telegraph đòn đánh hay bãi rơi đồ (loot). |

**Địa hình LOCKED:** mặt đi được chỉ nằm ngang; tường đứng và block/step trực giao. Không playable slope/ramp/triangle, collider đi được xoay hoặc diagonal surface. Đất/đá tự nhiên là solid mass có độ dày, mặt trên, mặt đứng và mép khép; đồi/núi bậc liên tục, không dải đất tự nhiên mỏng nổi. Mái/cành/background có thể vẽ chéo, nhưng route chơi trên mái phải author mặt ngang/bậc riêng.

Landmark/công trình chỉ dùng vài mặt collision sạch, không polygon collider theo toàn silhouette. **Không ladder/rope/vine/pole/wall climb; không Climb InputAction/state/animation.**

**Solid:** Ground baseline dùng TilemapCollider2D + CompositeCollider2D + Static Rigidbody2D; [Unity 6 Composite Operation = Merge](https://docs.unity3d.com/6000.3/Documentation/Manual/tilemaps/work-with-tilemaps/tilemap-collider-2d-reference.html), không checkbox UsedByComposite cũ. Natural earth/rock luôn solid.

**One-way:** chỉ cấu trúc mỏng hợp lý như ván/giàn/catwalk/ban công/sàn treo có support rõ, hiếm và khác hình solid. PlatformTilemap tách Ground, dùng [PlatformEffector2D Use One Way](https://docs.unity3d.com/6000.3/Documentation/Manual/2d-physics/effectors/platform-effector-2d-reference.html); nếu composite thì UsedByEffector do CompositeCollider điều khiển.

**DropThrough:** chỉ khi actor đứng trên one-way hợp lệ, tạm bỏ collision đúng cặp actor–support; restore khi actor xuống dưới hoặc timeout/cancel/death/transition. Không tắt effector/layer toàn world làm actor khác rơi; một press không chain qua các sàn kế tiếp. PHY-01 kiểm nhiều actor/nhiều tầng, không cần custom collision engine.

**Visual/data:** Background/Foreground không gameplay collider; solid/one-way/decor phải đọc khác nhau theo [Art](../03-art/art-and-visual-production.md#map-visual). Cell/tile size tính từ pixels/PPU32, không suy từ rig64. HazardTilemap P2. BossCombatArea là region trong Huyền Tích, không scene/root/portal hoặc Lv20 gate; bounds/normal-spawn exclusion đọc owner design tương ứng. Nước nông chỉ giảm nhẹ tốc chạy khi feet contact với vùng nước hợp lệ; trên cầu/trên không không giảm.

Slowdown/depth TUNABLE; flow/ripple cosmetic, không swim/drown/fluid subsystem.

**Camera:** Cinemachine chỉ follow character owner, chết vẫn nhìn death anchor theo vị trí authority; pop/fall VisualRoot không kéo camera. Transition đổi confiner theo root và snap/cancel damping qua offset; invalidation cache khi shape/lens đổi, không follow player từ xa.

**Authoring gate và phân bổ bãi quái hiện hành:** Thử nghiệm trước trên một farm room đại diện trước khi nhân rộng toàn map; bảo toàn tuyệt đối các Quest Anchor IDs (`DS2`, `DS3–DS6`, `TA4`, `TA6`, `XN1–XN6`, `HT4–HT5`, `HT_BossLandmark`), giữ ổn định các stable authored seed IDs (`TA5`, `BV1–BV5`, v.v.), SafeAnchor, dải vào an toàn 6–8 u từ cửa map, tuyến rút lui về làng và đường tiếp cận bãi rơi đồ (loot).

Mật độ bãi quái phải được author lại theo [Kế hoạch mật độ World & Content](../01-design/world-and-content.md#world-farm):
- **Nguyên tắc phân bổ:** Tăng số lượng bãi/cụm độc lập (`SpawnGroup`) trải trên các thềm đá, tầng cao/thấp và các tuyến nhánh; tuyệt đối không dồn thành một blob lớn 8–10 quái. Khung hình camera tiêu chuẩn có thể hiển thị 5–8+ quái thuộc 2–3 tầng khác nhau, nhưng mỗi cụm giữ AI/aggro độc lập, không báo động dây chuyền sang cụm bên cạnh.
- **Authoring source:** [World candidate manifest](../01-design/world-and-content.md) sở hữu ranges và quest anchors; runtime không giữ catalog thứ hai.
- **Boss Exclusion Rule:** Khu vực giao chiến Huyền Nham Cự Thú (`BossCombatArea`) tại trung tâm Huyền Tích cấm tuyệt đối việc sinh quái hoặc tuần tra của quái thường (`normal-spawn exclusion`), ngăn chặn việc quấy nhiễu trận Boss hoặc kéo quái thường vào bãi Boss.
- **Định danh ổn định:** Các ID cụm mới (`DS7+`, `TA7+`, `BV6+`, `XN7+`, `HT6+`) là mã authored ID ổn định trong `MapDefinition`, không dùng live instance index. Cấp độ trong `SpawnSlot.level` luôn phản ánh đúng `MobDefinition.fixedLevel`, không ngẫu nhiên hóa cấp trong cùng loài.
- **Dữ liệu lịch sử:** Con số 28 cụm / 66 slots là **LEGACY seed** cho bản thử nghiệm cũ; tổng dân số và số cụm cuối cùng vẫn là **OPEN / TUNABLE**, cần đo đạc traversal, run-back, contention, CPU và băng thông mạng trên scene thực tế.
- **Quy tắc cách ly:** Tách biệt vùng aggro và tuần tra bằng `WalkRegion` và khối địa hình thực tế; không áp dụng khoảng cách tâm cứng 18–20 u cũ vào mọi cụm. Scale visual của Linh Biến không tự scale hurtbox/aggro/leash; kích thước vật lý do definition và PHY-01 quyết định.

MapDefinition giữ `requiredLevel`, `unlockFlag` và required quest IDs; authority kiểm **Completed**, không ReadyToTurnIn hoặc counter. Locked exit trả lý do rõ, không invisible wall im lặng. Mỗi player có MapId riêng; root activity tính từ toàn player collection trên server, không từ map local của một Client. Render filter không disable simulation/NetworkObject của player khác.

**MapTransition và Cơ chế chống lặp (Anti-Pingpong):**
- `EdgeExit` giữ `targetMap`, `targetExit` và `spawnAnchor` đã author. Game Server kiểm tra thẩm quyền: nhân vật còn sống, đúng `MapId`/generation, chuyển động vào vùng exit là do người chơi chủ động điều khiển (`manual movement`), đích đến đã mở khóa (`requiredLevel`, `unlockFlag`, `quest Completed`); tuyệt đối không nhận tọa độ hay đích đến tùy ý từ Client.
- `SpecialGate` có logic kích hoạt xác thực riêng theo design owner (Huyền Môn kiểm committed restoration flags và access theo Q11 owner; exact final interaction OPEN; Lôi Đài cần chấp nhận thách đấu), không dùng generic Interact cho mọi lối đi.
- **Cơ chế chống giật chuyển cảnh liên tục (Anti-Pingpong Transition):** Tọa độ xuất hiện (`spawnAnchor`) ở bản đồ đích luôn được đặt cách mép collider trigger chuyển cảnh tối thiểu 2,5–3 u về phía trong lòng map (`safe inner offset`), nằm hoàn toàn ngoài phạm vi trigger trả về. Hệ thống duy trì cờ `isTransitioning` và chỉ kích hoạt chuyển cảnh tiếp theo sau khi người chơi đã rời khỏi vùng an toàn hoặc qua thời gian ân hạn (`re-arm upon exit`).
- **Xử lý lỗi và đồng bộ:** Một pending transition cho mỗi actor, loại bỏ trùng lặp (dedup trigger/request). Nếu bị từ chối (Locked), hệ thống gửi thông báo lý do một lần; người chơi phải bước ra ngoài vùng exit rồi bước vào lại mới có thể thử lại. Commit checkpoint đích vào cơ sở dữ liệu trước khi gửi snapshot cho Client; nếu commit thất bại thì giữ nguyên vị trí ở bản đồ cũ, không gửi snapshot giả. Quá trình chuyển map dọn dẹp sạch sẽ target, threat, action pending và visuals của map cũ.

<a id="combat-data"></a>

<a id="3-dữ-liệu-và-luồng-combat"></a>

## Dữ liệu và luồng combat

| Definition / state | Trường cần có | Validation |
| --- | --- | --- |
| SkillDefinition / profile | Stable SkillId, class, UnlockLevel/ManualRequirement, slotIndex; power/MP/CD/primaryEligibility/propagationPolicy/maxTargets/timeline/status/presentationProfileRef | SinglePrimary/PrimaryProximity/SnapshotSpread/PrimaryExplosion; không branch theo tên skill, SkillRank hoặc graph editor tổng quát |
| SkillAcquisitionState / PassiveDefinition | LearnedSkillIds; bốn passive IDs/class/unlockLevel/icon/tooltip/effect params | Active học từ manual; passive derive class+level theo owner design tương ứng, không thêm rank/points/duplicate flags; effect enum nhỏ |
| CombatRequest | Character binding, request correlation/sequence, requested SkillId, target identity/life/MapId, facing/aim intent | Authority kiểm binding/alive/map/capability/MP/CD/lock/range/target; Client không chọn actionId kết quả đáng tin |
| HitResult | Authoritative actionId, targetId/life generation, hitIndex, evade/crit/damage/remainingHP | Client chỉ trình diễn result; exact wire fields/time ở proposal bên dưới |
| MobDefinition | mobIdentityId/fixedLevel/baseRigId/paletteRef; stats curve, capability/profile, movement/range/timeline/hit shape/projectile presentation, loot/sourceProfileRef, linhBienEligible | Bảy fixed-level identities trên sáu rigs; palette reuse không cần AI/animation riêng; curve chỉ evaluate fixedLevel |
| LinhBienModifier | Stat/reward modifiers, visual preset, variant tag | Áp trên base runtime đúng một lần; không duplicate base identities/rigs |
| SpawnGroup / SpawnSlot | mapId/groupId/slotId, mobIdentityRef/cached level/spawnPosition; HomeRegion/HomeSpan/WalkRegion/SurfaceId/AggroRange/LeashRegion; deadline/generation/natural variantState/cap occupancy | Vùng đã author quyết đường đi/aggro; cache level phải bằng fixedLevel; respawn không đổi identity |
| ItemDefinition / Instance | templateID/instanceID/GearSlot/rarity/enhancement/count, stat lists; optional buyPrice/explicit sellValue, item policy/stack key/binding refs | Không suy stat từ UI position hoặc fake buyPrice cho drop-only; tutorial-bound tách bản vendor |
| QuestDefinition | prerequisiteQuestIds/requiredLevel; ordered typed objectiveGroups/MobIdentityRefs/special actor-or-variant predicates/markerIds/counts/collectionRngPolicyRefs/stagedGrantRefs/restorationTargetRefs (exact Q11 fields OPEN); giver/turn-in requirements/rewards/unlockFlags | Nội dung theo [Quests & Narrative](../01-design/quests-and-narrative.md#quests-story), không bảng quest thứ hai |
| QuestProgress per QuestId | state/activeObjectiveGroup/typed action counters/Q4 optional tutorialOrdinal/RNG outcome receipts/personal entitlements; restoration/claimed/staged flags và selected mentor/ring khi cần | State riêng từng character/QuestId; backend commit theo revision/receipt các mục liên quan |

Production data phải author đủ **12 QuestDefinitions Q1–Q12 và toàn tuyến thật** ngay từ đầu; G-L chỉ thực thi slice Q1–Q6, không cho phép cắt definitions/quest runtime thành bản sáu nhiệm vụ. Bảng mô tả nhu cầu domain/state, **chưa khóa wire schema**. `actionID` trong request cũ chỉ là shorthand correlation; authority mới tạo/bind actionId sau validation. Client sequence/correlation khác ID của action/result/life. Local dùng commit adapter RAM; backend transaction/ACK dưới đây mô tả TARGET, không gọi RAM ACK là durable.

## Quest, vật phẩm và chỉ số

**Quest runtime:** `Dictionary<QuestId, QuestProgress>` là cấu trúc TARGET đề nghị, chưa có production implementation; PostgreSQL giữ bản đã commit. Q9 Available/InProgress đồng thời với Q10 Available/InProgress là hợp lệ; không dùng một `currentQuestId` làm nguồn duy nhất. UI có thể pin chính tuyến, nhưng nhánh optional vẫn lưu riêng. Chỉ active objectiveGroup nhận event. Standard kill kiểm MobIdentity và sameMap player/mob tại death; landmark/placed/Boss/variant dùng typed special predicate theo Quest owner, không dùng sourceIds chung làm group whitelist. Q6 chọn phái đồng thời activate nhóm equip/learn/confirm trước action tiếp theo. Q7 confirm kiểm đúng ring instance đang sở hữu và đã ≥+1; nếu đã mặc thì không ép re-equip.

**QuestState:** Locked → Available → InProgress → ReadyToTurnIn → Completed. EligibilityService kiểm đủ prerequisite IDs **và** requiredLevel theo graph design owner, không ngầm dùng Qn−1 cho mọi nhánh. Thiếu level thì trả Locked cùng reason/nextLevel cho NPC/HUD, không hiện marker Available.

QuestService nhận event sau khi hành động server thành công, kèm eventID/characterId/MapId/targetId và payload cần thiết: `NpcTalked`, `RegionVisited`, `ItemPickedUp`, `ItemEquipped`, `ItemSold`, `ConsumableUsed`, `MobKilled`, `AttributeAllocated`, `ManualLearned`, `SkillUsed`, `EnhancementSucceeded`, `PvPCompleted`, `BossEligibilityAchieved`. Kiểm QuestId/active group/target, chặn replay rồi cập nhật counter/step.

Pickup/equip/sell/use/enhance thất bại không phát event; dùng bình khi đầy không có Used. Thay đổi inventory/character và quest step từ **cùng action** commit trong một transaction (các mục liên quan), với commandId/payload bất biến; publish durable success sau ACK. Riêng Potion phát accepted gameplay/ConsumableUsed từ server timeline, durable consume và Used progress cùng receipt theo [Potion durability](online-and-persistence.md#potion-durability).

Keydown/click hoặc UI tự báo thành công không cấp credit; không cần event bus lớn.

**NPC resolution:** definitions lấy [NPC/service owner](../01-design/quests-and-narrative.md#npc-service-review); server resolve mentor từ class + quest state, không tin turnInNpcId từ Client. Class/`ClassChosenLevel`/mentor/grant/active group commit cùng admission; replay không đổi mentor. Stable internal IDs giữ qua đổi display/ownership.

TurnInRequest kiểm connection/character binding, ReadyToTurnIn, NPC đã resolve, sameMap/range/claimedFlag. Một transaction gồm consume đúng collection items/binding, reward, Completed, story/unlock/summary eligibility và cleanup entitlement. Capacity preflight xét net bag sau consume + compatible merge rewards và số X slots thực cần; thiếu thì giữ Ready, trả reason+X, không thưởng một phần hoặc cleanup/unlock sớm. Retry trả receipt cũ. Đủ objective chỉ chuyển Ready, không teleport/auto-turn-in. Q12 chỉ Main Story/Chapter III Complete, vòng chơi vẫn tiếp tục.

**Staged grants:** receipt `(characterId,QuestId,grantId)` giữ quyền nhận một lần và pending khi túi đầy. Áo/sample Q4 là supply thật; ground hết hạn hoặc reconnect thì phục hồi entitlement chưa claim với cùng instanceId, không cấp reward lại. Áo bound tới equip, sample tới sell. Q6 grant weapon/manual trước learn/cast; confirm spent≥1 chấp nhận điểm đã cộng trước, không ép point mới khi pool0. MP Potion I reserved có receipt/đường cấp chắc chắn ở đúng step;

QuickMP ưu tiên món này, chỉ Used sau khi tiêu thật lúc thiếu MP. Q7 chỉ grant ring nếu thiếu, lưu selectedInstanceId/binding chống sell/drop tới confirm. Đá/Vàng reserved chỉ cấp khi cần +0→+1, không cấp thừa nếu đã ≥+1. Entitlement có pending/spawned/claimed/consumed; không dùng supply ngoài bước cho phép. Q12 Boss death credit và turn-in có receipts riêng; báo NPC không tạo pile nữa.

**Học manual:** sáu manuals ánh xạ tới sáu class SkillIds; class/level/S1 prerequisite cho S2/S3/boundCharacterId theo design owner. Kiểm ownership/alive/idle/no pending action/chưa learned. Consume book và thêm learned SkillId trong một command/receipt. S2 giữ S1, S3 không bắt S2. Cooldown dictionary theo SkillId; đổi slot/học mới không reset deadline cũ. RunningAction giữ profile snapshot; passive derive từ class+level.

SelectedSlot chỉ là UX, không cấp quyền cast hoặc tạo DB subsystem. **Manual không có cooldown:** learned/grant/learn receipts chặn lặp; chỉ skill có CD.

**Supply tutorial:** trước khi tạo grant/ground/entitlement, kiểm QuestId/InProgress/active step/source/generation/receipt. Ngoài đúng bước đó chỉ regular loot, không tạo entitlement cho bước tương lai. Retry quyền hợp lệ giữ itemInstanceId, không reroll hoặc hồi tố kill cũ. Mộc Kiếm Q3, thuốc Q6, ring/resources Q7 và reward Q12 không được gây softlock khi túi đầy/reconnect/replay.

**Collection items và credit:** item requirement derive từ committed bag theo [Quest owner](../01-design/quests-and-narrative.md#quest-collection), không collection counter thứ hai. Pending entitlement không credit possession; pickup phải qua server validation/commit. Kiểm QuestId/InProgress/active objectiveGroup/typed MobIdentity-or-special predicate/quyền còn thiếu; threshold/recipient predicates theo owner design tương ứng. Khi gây ActualHpLost, ghi activeQuestId/objectiveGroup/targetGeneration vào quest-qualified ledger; damage trước accept hoặc sai step không hồi tố. Whole-life ledger vẫn phục vụ reward/threat.

Death chụp active groups trước transition. Kill counters (Q3/Q4/Q5/Q12), Q4 tutorial ordinal, collection RNG outcome success/failure và bounded personal entitlements là typed paths, không generic guaranteed ordinal. Server roll một lần theo death/recipient/objective/epoch, persist immutable outcome cùng receipt/revision; level penalty không chặn quest. Serial cap tính bag+outstanding rights theo [Quest owner](../01-design/quests-and-narrative.md#quest-collection). N players riêng; Talk/Equip/Use/Enhance/Region/PvP không share/last-hit.

Q11 material Collect RNG → Lâm → Bách/gate/restored grant → three placements theo [direction/OPEN owner](../01-design/quests-and-narrative.md#q11-restoration). Grant receipt pin ba fragment identities/bindings và quyền Pending chưa nhận khi full bag; encoding/grant delivery exact cần spike, không tự direct-add giả pickup. Placement validator đúng target/fragment/quest epoch/map/range/active policy, consume corresponding fragment + durable flag + receipt atomic. Gate evaluate committed flags và approved completion policy, không bag possession sau consume; không tự choose endpoint/ordered sequence/level bằng runtime defaults.

**Thêm item vào inventory:** điền compatible stacks trước, overflow tạo stack tới capacity. Commit toàn amount hoặc giữ nguyên ground/pending. Compatibility gồm template/binding/instance flags; tutorial/manual-bound không merge với unbound. Sort-Merge/Split/Discard là explicit commands theo Items policy và roadmap priority; auto-stack không phụ thuộc Sort UI.

**Tính chỉ số trang bị:** ItemDefinition giữ `GearSlot`, `rarityPrimaryStatIds`, `enhanceStatRules`, `fixedSlotBonusDefinition`, `equipLevel` theo [Items & Economy](../01-design/items-and-economy.md#gear-economy). Data khai báo HP family Armor/Pants/Boots và MP family Weapon/Ring/Necklace, không suy từ bên trái/phải UI. HP/MP mới chịu rarity/enhance theo primary lists design owner; ACC/EVA, Crit/tốc chạy cố định, flat gains và Tinh Hoa dùng đúng thứ tự design owner.

Sáu class weapon templates riêng, không một weapon đổi stat theo người mặc. Quyền nhận drop/mua khác quyền equip; kiểm class/equipLevel tại equip/transfer preview và commit, không suy từ MapId/source. Giữ enhancement cap theo Items owner; derive Tinh Hoa theo cấp, tooltip cả khóa/mở và reject vượt cap. Retune HP/MP không đổi giá/source.

**Tính chỉ số character:** đọc owner design tương ứng, không copy balance table/formula. `ClassChosenLevel` ghi cấp thực lúc chọn phái cùng class transaction, giữ nguyên khi level-up/reset/reload; Tân Lữ chưa có datum này. Cung tăng HP chậm chỉ sau cấp đã chọn, nên chọn muộn không mất HP nền tại transaction; VIT có hiệu quả như nhau. Cộng nền/attributes/items sau rarity/enhance/flat/Tinh Hoa, rồi passive nền tảng đúng một lần.

Giữ số lẻ tới damage rounding/UI, không cộng stat trực tiếp mỗi equip/load. Level-up/class/equip/transfer/reset dùng cùng evaluator; sau ACK giữ currentHP/MP rồi clamp, không ratio-heal/revive. Phiên mới dùng checkpoint HP/MP rồi clamp; Food/status/CD chỉ giữ khi resume cùng phiên. Passive tinh thông chỉ tăng direct skill hit đúng cự ly, không basic Tân Lữ/Bỏng/proc chance; source chụp lúc cast, target/cự ly xét lúc resolve.

**UpgradeTransferCommand:** sourceInstanceId/targetInstanceId/commandId/expectedRevision; kiểm sender, ownership/bag, hai IDs khác nhau, slot/band/class/equipLevel/tutorial binding theo design owner. Chỉ cùng bậc hoặc lên đúng bậc kế, không I→III; preview/commit dùng cùng evaluator, no-gain reject trước mutation, không RNG. Plan tiêu source/cost + đổi enhancement đúng target instance; backend commit một receipt (các mục liên quan), không consume rồi spawn bản copy.

Retry trả target/result cũ dù source đã mất; hai commands tranh source chỉ một thắng. Failure giữ committed state, không spawn lại source để rollback. Equip riêng sau transfer, không auto-equip hoặc cần ô mới.

## Death, loot, spawn và action timeline

**Death/reward:** instanceID UUID duy nhất qua server restart, không GameObject.GetInstanceID/slotId; slotId authored giữ cố định. deathID=(instanceID,generation). Chụp RuntimeMaxHP/ActualHpLost ledger, contributor MapId/position/alive/connected/lastDamageAt và **level trước reward**, rồi mới sửa quest/EXP; sort stable characterId. DoT credit nguồn thật, không last hitter. Predicates reward/quest/Boss khác nhau theo design owner, không dùng một recipient list cho cả ba.

Không chia lại phần bị loại; TopDamage chọn whole-life ledger, không fallback né level suppression. Commit immutable roll/recipients/receipts trước publish; retry cùng deathID không reroll.

**Shared loot:** Normal/Linh một optional regular outcome/death từ weighted category pool theo Items owner; Boss giữ independent channels/exclusive gear. LootRecord giữ itemInstanceId/deathId/MapId/deathUtc/ownerId, contributorSnapshot gồm playerId/levelAtDeath và deadlines. Một OwnershipPhase(deadlines,now), pickup predicates theo mode. TopDamage eligibility100/0 quyết regular set trước roll, không RNG level gate/fallback. Fixed mob identity/level chọn band/slot pool, MapId chỉ chọn material theo design owner.

Contributor trong death ledger dùng level đã chụp cho pickup; người tới sau dùng current level. Recheck sameMap/alive/distance/capacity/window; không mất quyền vì chính kill làm level-up. Capacity fail giữ ground.

Claim/death transaction shapes và durability thuộc [Online & Persistence](online-and-persistence.md#persistence). Runtime tạo immutable death/claim payload và chỉ finalize durable rewards sau adapter success.

**Spawn arbitration:** server-owned cap occupancy/MapId giữ slotId/life/generation; due slots serialize stable slotId trên simulation writer. Spawn đọc identity/fixedLevel/position/group, curve rồi natural Linh modifier đúng một lần theo [World](../01-design/world-and-content.md#linh-bien), Lv1–7 không roll. Initial population cùng path; Return/root wake/reconnect không reroll life còn tồn tại. Không waiting set, Q8 reservation/force promotion hoặc quest retry machinery.

Death slot deadline25s BASELINE/TUNABLE; terminal-pending chưa finalize không respawn/release admission sớm. Exact deathUtc/cap release/due ordering còn [A15/TECH-01](../04-production/playtest-and-balance.md#pending-ordering-probes). Living Map Info count giảm tại server terminal state khác cap admission timing; không dùng count0 để cấp random Linh slot trước finalize. Hai views lấy cùng lifecycle record.

Boss stat/scheduler/shape đọc [World & Content](../01-design/world-and-content.md#world-farm). Một action tại một thời điểm; ba vùng đá không double-hit, geometry/né/nhịp cần PlayMode. Q12 không spawn Boss; credit và loot/turn-in receipts tách nhau.

**Canonical combat:** [Combat owner](../01-design/combat-and-character.md#target-propagation) tách acquisition/propagation/presentation. Primary identity của action là immutable; player damage kiểm eligibility/range theo profile, không hình chém. Proximity Kiếm quanh primary tại resolve; Spread start batch và Hàn valid-primary explosion là policies khác nhau. Mob melee/Boss ground geometry giữ đúng owner, không đổi theo correction player.

**ActionTimeline:** authority kiểm requested SkillId/class/learned/level/weapon/MP/per-skill CD/common action lock/primary eligibility/range. Accepted start tạo actionId/life/startClock, snapshot SkillId/profile/source stats/passive/origin/facing rồi commit MP/CD đúng một lần. RunningAction không đọc mutable selectedSlot. Một timeline cho basic Tân Lữ và combat intents từ PrimaryAction; sau class không zero-MP Normal Attack hoặc RepeatOnHold.

Nhịp S1/S2/S3 và MP mới là TUNABLE design owner, S2 có thể farm tần suất cao; không hard-code S1 là attack mặc định. Buffer/readiness theo [Combat & Character pending](../01-design/combat-and-character.md#pending-cast). Logical ranged resolve theo clock, không gameplay projectile. Spread ABC/ABA/AAA chụp start, ba hit cùng resolve moment từ profile/stable hitIndex; invalid index mất hit, không reacquire. Kiếm query secondary tại HitMoment một lần theo primary-first policy, freeze resolve set/indices trước apply;

Hàn invalid primary không nổ, primary Evade vẫn nổ, secondary roll riêng/no primary double-hit. Dedup actionId+hitIndex+targetId+generation, revalidate life/MapId/profile eligibility và DEF/EVA khi resolve. Death/map/hard CC hủy unresolved action không refund; result đã resolve bất biến. Bỏ frame/AnimationEvent/packet trễ không phát damage lần nữa.

**StatusController:** cache `(actionId,actualTargetId,effectId)` cả roll fail; A/A/A tối đa một application/unique landed target/cast, miss/invalid không roll. Chọn category trước roll: Normal/Linh chỉ Freeze; Boss/PvP chỉ Slow theo design owner. Đang Frozen hoặc trước `protectedUntil=thawAt+protectionDuration` skip/cached, không deferred proc. Freeze hủy unresolved action; visual đã phát không sinh gameplay sau cancel.

Bỏng giữ sourceCharacterId/ATK snapshot/generation/expiry/nextTickAt. Proc rates/duration/tick đọc design owner; reapply thay source/snapshot/expiry `now+burnDuration` nhưng **giữ nextTickAt**, tick đúng expiry chạy trước remove. Một tick/target, không N×damage; nguồn chết không xóa Bỏng, PvP immune. Target DEF xét ở tick; không Crit/random/INT, passive tinh thông không tăng tick/chance.

Freeze dùng frozenUntil/protectedUntil target-wide, không refresh. Slow một slowUntil theo category, chỉ đặt deadline `now+duration`, không stack magnitude. PvP Slow chỉ MoveSpeed, không interval/CD/animation/action đang cast. Boss Slow nhân tốc reposition theo design owner; clock chờ action kế tiếp giữ `remainingActionWait`, tick giảm `deltaTime×currentClockRate`, debuff hết lại rate1. Không reset full CD hoặc reschedule telegraph/hit/projectile đã start.

Cuồng Mạch chọn future base cadence trước, Slow chỉ tác động phần wait. N Cung share một debuff; generation chặn callback cũ kéo dài sai. Death/Return/Boss reset clear status; Client chỉ trình diễn status đã resolve.

**Passive hit:** Kiếm Thế/Xạ Tâm đo từ immutable actionOrigin tới actual target hurtbox center lúc hit/resolve theo design owner, cả secondary xét riêng. Bonus direct skill áp một lần trước DEF; không tăng basic/Bỏng/chance/range. Source capability chụp cast; target assignment không collision-swap/reacquire. Kiếm Tâm/Ưng Nhãn chỉ trong final evaluator, không per-hit proc cache.

## Mob capabilities, vùng hoạt động và crowd

`MobDefinition` cấu hình capability tái sử dụng như GroundMelee, GroundRanged/Hybrid, FlyingRanged và Boss special capability. Movement/range/timeline/hit shape/projectile/loot/rig/palette/Linh eligibility là data; không WolfAI.cs/MushroomAI.cs/BanditAI.cs theo species. Chỉ thêm code khi có capability tái sử dụng mới thật. Fixed-level identity giữ nguyên; Hybrid count/identity **OPEN**, candidate rows design owner không buộc ba ranged behaviors.

Ground mob giữ logical position trong `WalkRegion/SurfaceId` đã author, patrol ở home bounds/HomeSpan. Gặp edge không có ground continuation thì quay đầu, không rơi/Jump/DropThrough/pathfinding nhiều tầng. Hai terrace chỉ nối cho ground AI khi có route trực giao liên tục; route player phải jump/drop không tự là route mob. Flying dùng bounded2D engage band, Ong vẫn melee-accessible bằng Kiếm. Client chỉ interpolate, không full dynamic Rigidbody/nav graph/DropLink.

Passive aggro ưu tiên local/reachable context; hostile hit ngoài AggroRange vẫn wake và cộng threat. Group alert chỉ trong SpawnGroup, không sao chép threat hoặc recursively wake group khác. Threat/contribution theo design owner; sticky threshold baseline vẫn từ owner. Melee khóa origin/facing tại windup, revalidate front/vertical/MapId/alive ở HitMoment; player né thì miss, không guaranteed damage từ start. Ranged release resolve target logic, projectile cosmetic. Return/death/Freeze/map reset hủy unresolved theo action/life IDs; mob không Crit P0.

`HomeRegion` là vùng hoạt động gốc; `WalkRegion` là mặt/vùng đi được; `AggroRange` quyết passive acquire; `LeashRegion/bounds` neo vào home, không đuổi theo target position. Unreachable/outside leash sau grace thì Return theo policy; xét threat reachable khác trước. Return kết thúc encounter, clear threat/contribution/status/cancel action; đích reset HP đầy ở home, không giữ ledger lượt kéo trước.

Grace/Return speed/regen/invulnerability/targetability OPEN/TUNABLE; không khóa hồi đầy/miễn damage tức thì lúc bắt đầu Return hoặc vì một cú nhảy. Return không loot/reroll/life mới. CombatFocus có thể giữ Returning target. Execute validation theo policy targetability Return còn OPEN; không khóa mọi Return là immune hoặc luôn trả TargetReturning.

Cung kite liên tục trên route hợp lệ có thể no-hit pure melee; không chống lợi thế này bằng Wolf ranged fallback/teleport/jump tầng. Đứng một safe perch unreachable rồi spam mãi là geometry exploit: ưu tiên sửa map authoring và Return đơn giản. Không AI special-case class hoặc nhiều fallback chồng nhau.

**Melee crowd:** `Approach → Contact hoặc Staging → Attack → Recovery/Reposition`. Staging là điểm chờ gần tầm đánh. **Occupied != blocked:** actor đang chiếm chỗ không là terrain wall. Tắt Player–Mob/Mob–Mob body collision, tách hurtbox/query; front mob có thể nhường contact trong recovery, rear mob tiến/chỉnh bước hợp lệ. Không hard formation/four-slot/token/teleport hoặc biến rear melee thành ranged. Threat target không đổi vì điểm visual bị chiếm; mọi lựa chọn xét từng mob/target, không assume hai players.

Soft separation là PROBE/TUNABLE: desired X trên cùng WalkRegion, repulsion nhỏ có clamp/damping/deadband, stable IDs tránh jitter/đổi phía liên tục. Project vào home/walk bounds, không repel qua tường/tầng hoặc Rigidbody.AddForce. Windup/hit giữ origin/facing; steering chỉ approach/staging/recovery. Reposition/lùi trong recovery/interval hiện có, không tăng thời gian/DPS ngầm; terrain blocked thì bỏ chỉnh bước thay vì retry vô hạn.

Phase offset ổn định mỗi life không rút interval dưới definition. PHY-01 kiểm 1/2/4 melee ở tường/mép, player đứng/chạy/nhảy, AoE 2–4 targets và N players; không ép flying Ong dùng ground offsets.

Physics masks tách body collision/hurtbox/solid/one-way. Player eligibility/propagation và mob/Boss geometry độc lập sprite/VFX colliders. LoS A/B và S2/S3 air permissions còn gate PHY-01/ART-01; không biến prototype mask thành production lock.

<a id="shared-combat-input"></a>

## Input, focus và interaction dùng chung

<a id="input-contract"></a>

<a id="active-focus"></a>

### PrimaryAction / ActiveFocus — USER-APPROVED direction

Player có **một actionable ActiveFocus**: Enemy/PvP/GroundItem/NPC/Landmark/None. Enemy/PvP dùng CombatFocus NONE/AUTO/EXPLICIT + search/retention/execution/life machinery bên dưới. Non-combat focus thay combat selection; không marker enemy active cùng selected item. Candidate cache/internal combat memory không active authority thứ hai.

| Event/state | Resolver/dispatch |
| --- | --- |
| Valid combat focus | Sticky; item/NPC/landmark không tự cướp. Không re-sort nearest mỗi frame |
| Explicit click object | Choice thắng AUTO; bind ID/life/generation/MapId, click không action. Chuyển non-combat cancel PendingCast/BufferedIntent/approach trước bind; RunningAction đã accepted giữ snapshot riêng |
| Enemy invalid/dead (actor sống; dead observer policy OPEN ở dưới) | Invalidate rồi chọn combat candidate phù hợp ngay; không có thì eligible nearby GroundItem → NPC soft-focus trong support context → contextual landmark → None. Không auto cast/pickup; priority geometry/order exact TUNABLE |
| GroundItem active | Marker nhỏ; PrimaryAction snapshot **đúng item instance/generation** → PickupItem duy nhất; nearest thay không đổi payload, không ExecuteSkill |
| Item expired/claimed/no rights/out of range/despawn/wrong generation | Invalidate rồi resolver cho selection tiếp; press đã admit item không phát fallback cast/pickup khác trong cùng press |
| Capacity fail | Giữ valid item và reason thiếu ô; 60/60 compatible stack có thể success. Không coi bag-full là lost rights |
| Pickup success | Commit rồi clear claimed focus, resolver chọn candidate tiếp theo; một press không pickup chuỗi. Combat-first khi cần resolve; auto next-item policy/order TUNABLE, không stealing valid explicit focus |
| Tab/Shift+Tab world | Chỉ local combat set; đang item/NPC/landmark có thể chuyển combat nếu có candidate. Không cycle loot/NPC; không candidate giữ valid focus, không action |
| NPC PrimaryAction | OpenNpcContext → root menu quest/service/Talk; ưu tiên selected eligible quest action, **không NpcTalked/quest/grant/turn-in** chỉ vì mở menu |
| Landmark PrimaryAction | Một typed interaction đúng target/range/quest/state; scenery không tự thành mandatory objective |
| None + press | Resolve một candidate rồi chụp đúng một typed intent; None không target trả NoTarget. Nếu candidate invalid trong validation thì reason, không reroute press |
| Death/map/session | Death giữ valid combat observer/HUD nhưng khóa actions và cancel pending; auto acquire mới khi dead **OPEN**, recommendation giữ observer tới target invalid rồi None. Map transition/session loss clear stale focus/generation; same-session resume revalidate |

Client input adapter resolve rồi gửi concrete `ExecuteSkill(SkillId,target life)`, `PickupItem(itemId,generation)`, `OpenNpcContext(npcId)`, `NpcAction(actionId,npcId,...)`, `LandmarkInteract(targetId,...)`. Server validate từng command theo domain; không network `PrimaryAction()` mơ hồ. 1/2/3 select-only, không cost/CD/approach. PrimaryAction chưa chọn phím cứng; E là test default TUNABLE, không cần permanent F Interact.

Modal/chat capture press; press mở UI bị consume, không Confirm option cùng frame hoặc leak world cast. Talk chỉ option trong context và successful Talk mới phát NpcTalked. UI close/open/map/reset dọn pending/capture; key held không tạo press mới. NPC context phải revalidate NPC/range/state khi action commit; full bag/disabled quest/service reason đọc result hiện hành.

Valid explicit item không bị auto enemy steal; valid combat không bị auto loot steal. **OPEN** exact sticky/priority của auto non-combat focus khi có combat candidate mới; recommendation chỉ resolve lại khi invalid/explicit input để tránh flicker, trade-off Tab/click thêm khi enemy mới đến. Không mặc định rule đó thành gameplay lock.

Input adapter phát semantic actions `Move`, `Jump`, `DropThrough`, `SelectSkillSlot1`, `SelectSkillSlot2`, `SelectSkillSlot3`, `PrimaryAction`, `CycleTarget(direction)`, `QuickHP`, `QuickMP`, `Food`, `Navigate`, `Confirm`, `Back`. Move ←/→, Jump ↑, DropThrough ↓ và select1/2/3 theo [Combat & Character](../01-design/combat-and-character.md#ux-art). PrimaryAction/Potion/Food/menu physical keys **OPEN**; candidate E/4–5/R/I chỉ PROPOSAL/TUNABLE DEFAULT cho usability. Không alias gameplay A/D/Space/S cũ hoặc tự gán C/Q.

[CombatFocus/select/pending gameplay](../01-design/combat-and-character.md#focus-input) thuộc Combat; Runtime giữ dữ liệu triển khai: `CombatFocus(mode,targetId,life,generation,MapId)`, tách search/retention/execution. Không copy thứ tự acquisition/Tab/death observer ở đây. Universal non-combat resolver vẫn tại [ActiveFocus](#active-focus).

`PendingCast` giữ requested SkillId/target ID/life/generation/MapId/intentId/expiry/start/progress position/heldMovementMaskAtPress; chưa chụp source stats/action hoặc commit cost. `BufferedIntent` chỉ một execution intent mới nhất, có readiness/expiry theo authority clock. `RunningAction` đã accepted có snapshot riêng. Select đổi UX nhưng không mutate/cancel ba state này; **Execute mới** mới thay PendingCast/BufferedIntent, không sửa action đang chạy.

| Bước | Contract triển khai |
| --- | --- |
| Execute press | Chụp selected SkillId/target life/MapId và từng horizontal binding đang held; resolve/validate capability trước tạo pending/buffer |
| Input arbitration | Bỏ held mask cũ trong pending/buffer kể cả ngược hướng; KeyDown ngang mới/Jump/Drop/focus/UI/chat/Esc/death/map/invalid target hủy intent. Terminal trả quyền axis còn held |
| Buffered readiness | `max(remaining skill CD, remaining common lock)` phải sẵn trong window0,18s BASELINE/TUNABLE. Không approach trước ready, không FIFO/long-CD queue; expiry clear/reason |
| Approach path | Grounded/same reachable lane/wall/edge/progress và swept segment với exit; cắt EdgeExit thì Blocked trước movement/cost. Chỉ manual movement kích hoạt exit, không AssistAxis |
| Arrival validation | Target đúng life/generation/MapId/alive; actor alive/no hard CC; learned/class/weapon/level/MP/CD/lock và primary range/eligibility tại origin thực; Return targetability theo policy đang probe, không mặc định mọi Return bị cấm. Fail clear/reason; success mới tạo actionId/snapshot/MP/CD đúng một lần |
| Cancel/reject | Blocked/no progress/timeout/invalid hoặc lifecycle/user cancel đều clear, không retry ngầm/chuyển target. Release Execute không cancel; hold không thêm intent |
| Esc dispatch | Consume đúng một tầng [Combat & Character](../01-design/combat-and-character.md#escape-priority); đóng UI/chat không rơi xuống cancel/clear focus cùng press |

Reject enums gồm PlayerDead/HardCc/NotLearned/WeaponRequired/InsufficientMp/Cooldown/ActionLocked/NoTarget/TargetMissing/TargetDead/TargetGeneration/TargetReturning/WrongMap/OutOfRange/Blocked/BufferExpired/UserCancelled. UI dịch reason thành text, không dùng text làm control flow.

Approach budget tính đoạn còn thiếu ngoài execution range của từng profile, không dùng một distance chung cho Kiếm/Cung. Chỉ chạy ngang với tốc chạy thường trên lane đi được; timeout/progress/budget cụ thể TUNABLE. Không tự jump/drop/dash, tìm đường nhiều tầng, nối pocket xa hoặc đổi target. Kiểm primary eligibility từ origin dự kiến có thể tới trước assist rồi origin thực trước commit; không đi tới secondary để cứu cast. RunningAction resolve theo primary/batch policy đã chụp dù focus bị đổi sau start; pending bị đổi focus thì hủy. Range reject/invalid lúc resolve không retarget hoặc refund cost.

Gravity/momentum tiếp tục; air cast Tân Lữ/S1 cần probe, quyền S2/S3 trên không còn OPEN.

**Ranh giới UI:** keyboard/mouse dùng cùng action list/validator/command. Modal giữ selected action ID; renderer vẽ focus/lý do disabled rồi dispatch, không sửa progression. NPC mới mở ưu tiên quest action; consume input mở UI, không Confirm lần hai cùng frame. Khi list đổi, giữ selected ID nếu còn hợp lệ, nếu selected stack merge away thì map tới survivor ID; nếu mất thì clear/chọn lại có cue, không index-clamp sang món khác; không gọi callback món/session cũ. Mở/đóng UI/transition/reset dọn pending gameplay và input capture; giữ phím không sinh Execute mới.

Enter là Confirm trong modal, Chat ở world. Tab/Shift+Tab điều hướng view trong UI, CycleTarget ngoài UI/chat, không dispatch cả hai. Bag filter theo GearSlot từ inventory hiện có; Equipment/Attributes/Derived Stats là views riêng cùng evaluator. Preview/world đọc cùng Art pose/socket contract. NPC text hiển thị kết quả command đã commit.

<a id="map-info-runtime"></a>

### Map Info replication — ENGINEERING RECOMMENDATION

World population owner định nghĩa [counts](../01-design/world-and-content.md#map-population-info). Server emit current MapId + map/session generation + population revision + ordinary/Linh/NPC/Boss counts/alive state. Admission/reconnect gửi snapshot; spawn/new life/terminal death/despawn/NPC presence update revision/delta. Duplicate/stale revision ignore; gap/out-of-order request snapshot, không decrement hai lần hoặc underflow. Map change clear old panel, late old-map packet không overwrite; disconnect/loading hiện pending/unknown thay vì stale counts truth.

Client không count renderer/interest set; dormant mobs vẫn trong authoritative living total, due slots chưa sinh chưa tính. Natural cap admission timing và living count khác nghĩa theo spawn contract; schema/transport exact ở G-D. Panel không đưa entity IDs/positions/pockets để UI làm Linh ping; existing combat snapshots/visibility policy giữ. Integration phụ thuộc SpawnManager/population observer và map/session replication, không chỉ Art HUD widget.

<a id="movement-feel"></a>

**Normal Jump fixed height — direction đã xác nhận; feel TUNABLE:** gameplay dùng phím mũi tên. Mỗi accepted press khởi tạo cùng launch profile, không cutoff vận tốc hoặc đổi upward gravity khi nhả/giữ Jump. Coyote tính từ support cuối; buffer có hạn và consume một lần, một press tối đa một jump. Grounded kiểm chân/support/chiều chuyển động, không sprite bounds. Launch velocity/gravity/fall gravity/landing và acceleration/deceleration ground/air vẫn TUNABLE, deterministic theo authority fixed tick. Collision trần có thể chặn apex; test tap/hold phải cùng setup/no obstruction. Tốc ngang dùng MoveSpeed gồm AGI/Giày/Slow/nước, không suy Giày/AGI thay jump height.

Tuning tốc rơi không đổi pose clock. Duration/force chưa khóa; dùng authority clock/fixed physics, không phụ thuộc render FPS. Coyote/buffer không tạo double jump; dọn state khi death/map/UI/Freeze theo lifecycle.

DropThrough trên solid không có tác dụng và không chặn Jump. Trên one-way, Jump+Drop cùng frame thì Drop thắng, consume pending jump/coyote để không bật ngược. Chỉ bỏ collision actor–support hiện tại rồi restore; giữ Drop không xuyên sàn tiếp theo, cần press mới. Probe nhấn nhanh/giữ phím, jump sớm/muộn ở mép, tường/trần/landing, drop nhiều tầng, air cast, AGI/Slow và render 30/60/120 FPS. Review thủ công ở tốc độ thường quyết cảm giác; automation kiểm invariant.

Physics2D sở hữu collision/grounded/triggers/drop pairs của player/world; combat geometry riêng. Không thêm slope/climb state hoặc khóa movement để sửa lỗi art.

<a id="presentation-data"></a>

## Dữ liệu presentation cần thử ở spike — PROPOSAL

Đây là nhu cầu message/state cho presentation, **chưa khóa schema/transport**. [A11/A15](../03-art/art-and-visual-production.md#art-open-decisions) theo dõi validation. Local Session xuất cùng ý nghĩa result/state với Dedicated; Client không có quyền quyết result.

Fields ứng viên gồm actionId/request correlation, actor/life generation, MapId/profileId/startClock/timeline revision; visualId/equipment revision chụp lúc cast; visual index/origin/aim/travel duration; logical hitIndex/resolveClock; actualTarget/life/evade/crit/damage/remainingHP; status kind/source/expiry; terminal/cancel/lifecycle phase. Target HUD cần identity/generation/currentHP/maxHP snapshot dù owner đã chết.

Đây là fields authority gửi để hiển thị, không dùng Client echoes làm dữ liệu đáng tin. HitResult cũ chưa đủ timestamp/generation cho packet trễ hoặc pool reuse; exact message cần spike.

Network 20 Hz ≈ 50 ms/snapshot, physics 50 Hz = 20 ms/tick, render 60 FPS ≈ 16,7 ms/frame là BASELINE. Không stream từng sprite frame; clock/phase/profile chọn pose deterministic (cùng đầu vào cho cùng kết quả), correction theo action/state/result. Tick lệch không biến một hit batch thành truyền target A→B→C. Remote interpolation và local cosmetic anticipation giữ cùng ý nghĩa gameplay; độ trễ phải đo. Client căn packet trễ về phase còn hiệu lực, không chạy damage lại.


<a id="art-contract"></a>

<a id="8-hợp-đồng-tích-hợp-art-và-animation"></a>

## Hợp đồng tích hợp art và animation

[Art import/pose/socket owner](../03-art/art-and-visual-production.md#art-integration), [facing/carry](../03-art/art-and-visual-production.md#player-facing) và [player shadow](../03-art/art-and-visual-production.md#player-shadow-death) giữ visual constraints và classification. Runtime không có frame/module/sorting catalog thứ hai; adapter nhận pose/profile/socket refs từ manifest.

Một shared state/phase controller/actor dẫn mọi modular part. Physics root/hurtbox tách VisualRoot: swap/flip/scale/carry không đổi collider. Collider baseline khoảng 0,60–0,65u×1,45u **TUNABLE**, không lấy toàn canvas; attack origin là authority data, không socket hoặc sprite bounds. Atomic representation/order switch tránh double-render equipped weapon; cosmetic carry timer không delay Execute/MP/CD. Action-facing snapshot dùng xuyên release.

Terminal state/clock cùng actor identity/life/generation/MapId dẫn presenter vào phase hiện hành, không thêm gameplay death states hoặc physics knockback. Duplicate/stale không replay, late join bắt đúng phase; revive/map/life mới reset offset/visibility/timers và hide/show parts đúng state. Camera/death anchor/HUD bind authority root, không visual pop/fall offset. Schema/event vẫn [PROPOSAL](#presentation-data).

**Feedback và pool:** renderer nhận authority event. AnimationEvent/FX không gây damage/stun hoặc pause simulation. Pool reuse reset tint/timer/parent/owner/action/life; release hủy listeners/timers. Callback kiểm IDs/generation, không GameObject reference làm lifetime identity. Pooled projectile chỉ presentation; packet trễ bắt phase còn hiệu lực, không replay gameplay. Shared actor clock và SortingGroup đọc order từ Art.

Tích hợp layout/physics/camera theo [Maps](#maps), không lặp environment/terrain catalog tại đây. Import probe theo [Art First Probe](../03-art/art-and-visual-production.md#first-art-probe); [Roadmap production-release](../04-production/roadmap.md#production-release) giữ sandbox/family sequencing và gate. Legacy workload không là manifest hiện hành.


<a id="ui-notes"></a>

<a id="9-hợp-đồng-ui"></a>

## Hợp đồng UI

Một router/modal stack cho NPC/inventory/character/quest/PvP. InputActionAsset giữ Gameplay/UI contexts; chat/modal chặn gameplay actions và clear buffered movement/execution. Jump/DropThrough resolver ưu tiên Drop trên one-way nếu cùng frame, không emit cả hai. Chỉ owner local gắn input/camera/HUD; world online không cần PlayerInputManager couch join.

Semantic actions và quyền interaction theo [Combat & Character](../01-design/combat-and-character.md#ux-art); glyph đọc actual bindings, không hard-code candidate E/F/4–5/R/I hoặc alias cũ vào objective/tooltip.

**QuickConsumableAction:** server kiểm inventory/level/count/CD/state, chọn item theo design owner và consume một lần. MatchId còn kiểm quota theo design và cấm Hồi Sinh Phù; quota tăng tại cùng realtime acceptance với consume/cooldown/heal, không chờ DB commit, reject không tiêu item. Q6 QuickMP ưu tiên reserved Potion I ở đúng step, không bỏ món reserved để làm kẹt objective. Hiển thị reason khi reject.

Quest HUD dùng canonical state, không client tự chuyển Available/Ready. QuestDefinition giữ semantic enum/event/region/NPC/instance requirements; credit theo successful result, không keydown. Q3 author năm Dummy placements3+2 cùng pool/lifecycle theo World owner, không đổi farm respawn để chữa thời gian chờ tutorial. NPC focus ưu tiên context dịch vụ; combat execution riêng với interaction.

| View | Nội dung cần kiểm |
| --- | --- |
| HUD | HP/MP/EXP, selected skill và locked/learned/CD/MP state, Execution affordance, Food/Potion/quest/Boss timer; S2 chọn và farm thường xuyên, không trình bày S1 như default attack |
| Inventory/shop | Capacity/stack rules từ design owner, server transaction trước refresh |
| Upgrade/transfer | Preview cùng evaluator; source tiêu/target trước–sau/cost/Tinh Hoa/khóa–mở rõ; failure vẫn lưu cost |
| Skill panel | Ba active slot tích lũy/CD riêng, manual requirements, hai passive/class và mốc Lv5/13; passive hiện final stats, không skill points/ranks hoặc mới chọn là cast |
| Quest/chapter | State/nextLevel/resolved turnInNpcId server-owned; Ready vẫn chơi tiếp; Q6 mentor đúng class; Q10/Q12 Lâm Bá, Q11 Lâm→Bách và endpoint OPEN, Q12 Main Story Complete |
| WorldUI | HitResult/status/evidence theo recipient; combat focus marker + current/max HP/text/bar đồng bộ; owner chết vẫn thấy damage của người khác trên target còn hợp lệ; loot windows/quest cues/chat/Boss name riêng per-character |
| Death/PvP | Death choices khác PvPDefeated; stake/escrow pending/confirmed/phí/refund/quota, không xác nhận mutation khi chưa ACK |

Split/Sort-Merge/Discard delivery priority theo [Roadmap scope OPEN](../04-production/roadmap.md#map-info-work-package); gear protection mở rộng/quest arrows/history giữ P1. Tooltip làm tròn cho đọc nhưng evaluator giữ fractional enhancement. Không đưa RPC/saveAPI/microservice vào player flow.

<a id="dev-mode"></a>

<a id="91-dev-mode-để-kiểm-feature-và-phục-hồi-test"></a>

## Dev Mode để kiểm feature và phục hồi test

Dev Mode là tooling P0 cho người phát triển/tester trong development build, tách player UX/release. Không dùng dev shortcut làm lời giải cân bằng hoặc thay đường chơi production. Dev commands vẫn đi qua authority/evaluator/receipt khi thay state bền; không client tự sửa canonical snapshot. Dedicated cần quyền dev riêng trong cấu hình development và test account/storage riêng; không đưa credential/quyền/dev UI vào release client.

| Nhóm thao tác | Tooling cần có và giới hạn |
| --- | --- |
| Progression/class | SetLevel/EXP/attributes; Set/ClearClass với `ClassChosenLevel` hợp lệ; learn/unlearn skill; inspect derived stats/points/CD. Thay state invalidate pending action và derive lại evaluator, không cộng stat lần nữa hoặc đoán cấp chọn phái |
| Quest/inventory | SetQuestState/active step/prerequisite; give/remove item/Gold/manual; inspect/reset entitlement/counter/receipt test có chủ đích. Giữ stable IDs/bindings và phân biệt fixtures đã grant với reward chơi thật |
| World/combat | Teleport tới authored marker/SafeAnchor; spawn/reset group/life; force Linh chỉ cho probe; reset encounter/Boss/status; inspect target/focus/action/generation/clock/threat/contribution/loot deadlines. Dev Force không là production quest spawn hoặc acceptance fresh-run |
| Test lifecycle | ResetFreshCharacter/test profile, reload/checkpoint/reconnect probes và log before–after. Reset chỉ dữ liệu test đã chọn, không wipe database/character thật; clear hoặc đổi generation để callbacks cũ không tác động life mới |

Mỗi dev override có label, command log và cách quay về production defaults. Preset Lv20/fullgear/Boss yếu hữu ích cho feature probes; **không chứng minh pacing hoặc fresh journey Q1–Q12**. Nghiệm thu cần fresh character **cho từng phái Kiếm và Cung**, không skip, EXP multiplier1/current defaults, chơi đủ các bước và optional Q9 branch. Sau reset không để item/quest entitlement, receipt, focus, action hoặc profile stats cũ rò vào run mới.

Exact UI/command transport/authorization implementation còn PROPOSAL phải kiểm ở production base/G-D; ranh giới dev/release là contract.


<a id="potion-ordering"></a>

## Ordering Potion, damage và Food

Server xác định thứ tự semantic commands và combat events trên một timeline; không dùng thời điểm ACK để chèn heal. Accepted Potion consume/quota/cooldown/effect là một bước authority nguyên tử trong tick, rồi các hit/Food/death tiếp theo đọc HP/MP mới. Actor đã chết trước admission thì reject; đã accepted trước lethal hit thì hit dùng HP sau heal.

Client không dự đoán gameplay heal. Retry cùng intent/commandId không tạo event Used hoặc heal lần hai. ACK, reject hoặc timeout của persistence không chạy lại tick/action. Actor/life/generation cũ và callback VFX cũ không tác động phiên mới. Priority chính xác khi hit và command cùng tick còn POT-01 spike, phải deterministic và log để review.

Persistence ordering và crash quarantine thuộc [Online & Persistence](online-and-persistence.md#potion-durability), không thuộc renderer hoặc QuestService.

<a id="a11"></a>

## A11 — quyết định liên quan

**A11 Anticipation online** — Chỉ local pose: rẻ nhưng chờ projectile; projectile tạm: mượt hơn, cần xử lý ghost/dedup; rollback combat: tăng scope · Pose/âm/cast cosmetic là probe; projectile chính/result authoritative; tentative gameplay projectile và rollback DROP P0 · P12; không thêm prediction/rollback physics P0

**A11** — TECH-01 / ART-01 · BASELINE presentation chỉ đọc authority; OPEN immediate local pose/âm anticipation và schema. Tentative gameplay projectile/rollback DROP P0; không giữ branch projectile prediction trong backlog. Prototype chỉ reference; production base sau docs/feel/art probe, Dedicated/2-client gate sớm sau G-L revision mới.



**Đóng giao diện theo thao tác:** nhận/trả quest, nhập phái và nghỉ thành công đóng hội thoại để tiếp tục đi; câu xác nhận vẫn hiện trên NPC/tracker. Lỗi/reject giữ view và reason. Buy/Sell/Store/Take giữ view để làm nhiều lần; Esc lùi một submenu, ở root thì đóng. Equip/Unequip/Learn thành công trở về view chứa item/slot; tab switch đi trực tiếp tới view mới, không giữ submenu cũ; [Esc ưu tiên theo context](../01-design/combat-and-character.md#escape-priority). Intro hiện trước khi nhận quest; không bỏ narrative chỉ vì auto-close.

<a id="item-transaction-flows"></a>

## Item/quest command flows A–G — TARGET, chưa implementation

Handlers là trách nhiệm đề nghị, chưa phải classes có sẵn. Authority receiver kiểm sender→character/session/lease/life/MapId. UI gửi IDs/quantity/revision/correlation, không trusted price/reward/ownership. Buy/Sell/Pickup/TurnIn riêng dùng chung Inventory planner và commit seam; không generic ItemService/event bus. RAM adapter có receipt/revision để test pending/reject, chưa durable. Lookup committed receipt **trước** expected-revision check; cùng commandId khác payload reject. Khi chưa rõ kết quả, query/retry cùng ID trước lệnh dependent, không sửa payload dưới ID cũ.

| Flow | Admission/calculation | Atomic commit và publish | Failure/retry |
| --- | --- | --- | --- |
| A Buy | NPC catalog→details/quantity→request; kiểm alive/NPC/range/map/catalog revision/template/class/level/quantity/currency; checked price×quantity và simulate merge/capacity | Debit Gold + add bag + revisions + qualifying progress nếu có + receipt; publish result/snapshot sau ACK | Thiếu ô/Vàng/off-class không debit; timeout giữ pending/immutable plan; retry không mua hai lần. Catalog thay phải reconcile rồi request mới |
| B Sell | Grid **toàn bag**→instance/quantity; kiểm ownership/location/count/vendor/policy; quest-bound fail reason; Q4 sample chỉ đúng bound instance/step/Bách | Remove quantity + credit Gold + ItemSold receipt/progress + revisions cùng commit | Equipped/bound/stale fail; double-click cùng intent dedup; concurrent Use/Sell serialize writer/revision, chỉ một thắng |
| C Regular pickup | Snapshot windows/owner/contributor/level-at-death theo Items; alive/map/range/original deadline/unclaimed; simulate toàn payload | Ground claim + bag add + receipt/revisions; retire visibility sau ACK | Giữ 8/20/60 và 12/30/90. Hai người claim một pile chỉ một thắng; capacity fail giữ ground tới deadline gốc; regular expiry không recovery entitlement |
| D Personal quest | Lethal→snapshot active-group ledger/threshold→one RNG outcome/recipient/objective hoặc Q4 tutorial ordinal→bounded right; expose ground sau death finalize ACK. Pickup kiểm right/epoch/generation/lease/map/range/alive/quantity/capacity | **Death:** typed kill progress/Q4 ordinal + RNG success/failure + entitlements + optional regular outcome/rewards + death receipt atomic. **Pickup riêng:** add bag + Claim chuyển trạng thái + revisions/receipt atomic; re-evaluate possession/publish sau ACK | Không mark Claimed trước add commit; wrong-owner RPC reject dù client thấy/ẩn; stale generation reject. Already claimed trả receipt/reconcile; kill trước accept không hồi tố |
| E Full bag | Preflight compatible merge; reason thiếu X ô; player tự dọn bag | Không add/claim/collection credit; Pending right giữ bền, representation có TTL hữu hạn | TTL retire ground, không xóa quyền; re-offer ground theo recovery owner, không reserve/auto-add/vĩnh viễn |
| F Reconnect | Resume grace giữ session; phiên mới load bag/progress/receipts/rights, validate sourceMap/anchor; lookup uncertain command trước re-offer | Claimed item đã ở bag không respawn; Pending chỉ một generation mới tại sourceMap | Reconnect không regrant; lease/generation cũ không mutate. Restart không suy RAM/client visual thành committed grant |
| G Turn-in | Active/Ready cache, đúng resolved NPC/map/range/alive; revalidate action history và bound items/count; simulate consume rồi merge reward trên net bag | Consume remaining required inputs (Q11 fragments đã placed không consume lại) + reward/Gold/EXP + Completed theo approved endpoint + unlock/chapter/Journey + entitlement cleanup + receipt/revisions một commit | Reject giữ toàn inputs/Ready, không partial consume/reward/unlock. Timeout không báo Completed/cleanup; lost ACK lookup receipt, chỉ thưởng một lần |

**Migration:** chưa production DB/code để migrate trong lượt docs. Version QuestDefinitions thay old quotas/ordinals bằng mob Collect RNG và Q11 staged restoration targets; không chỉ đổi virtual counter sang item. Không suy fixture “counter đủ” thành item đã pickup: reset hoặc approved mapping một lần sang Pending recoverable rights theo source outcome/receipt (Q4 ordinal riêng); không credit lại regular kills/rewards. Action history khác collection possession; schema ở Online vẫn OPEN.

<a id="shared-item-ui"></a>

## Shared item UI/controller contract — STRONG DIRECTION UX

Shop dùng **tabs Buy/Sell trên, ItemGrid trái, SelectedItemDetails phải**. Buy chỉ NPC catalog; details có icon/name/type/description/requirements/stats/comparison khi phù hợp, unit price/quantity/total/Buy. Sell giữ bag order và hiện mọi món kể cả unsellable; disabled Sell có reason. SellableOnly là filter optional, không bắt Sort. Pending/reject không giả currency hoặc xóa món trước ACK. Chưa khóa columns/dimensions; 60 slots cần scroll/page responsive và selection luôn thấy được.

Reuse layout/navigation, slot/icon/quantity/rarity/binding glyph, stable-ID selection và detail/stat formatter. Buy dùng catalog entry ID; Sell/Bag/Storage dùng instance/stack ID, không index list làm authority. Controllers Buy/Sell/Bag/Storage/Equipment picker giữ source/actions/pending riêng; shared viewmodel chỉ display data + allowed actions/reasons. Đây là proposed boundaries, chưa classes/files có sẵn. Quantity picker prevalidate integer/available/currency, server vẫn kiểm. Keyboard/mouse dùng cùng semantic command; mở modal không confirm cùng frame, Esc/breadcrumb giữ selection hợp lệ. Shop không chứa Learn/Use/Equip; Inventory không tính trusted vendor price.

**NPC context:** receiver validate definition/map/range/alive và quest/service predicates. Q6 discovery receipt chỉ từ Talk đúng mentor/Q6 group; class admission kiểm đã nói cả hai, weapon slot trống và prerequisites. Việc unequip là action riêng có capacity check; class/grant/mentor commit cùng admission. Contextual reply đọc committed class/quest/learned skills. Marker/thoại không tự credit; unselected mentor vẫn Talk, off-class manual absent/disabled reason. Không relationship meter/cinematic/auto-face.


<a id="combat-boundary-review"></a>

<a id="combat-runtime-boundaries"></a>

## Combat foundation nhỏ và presentation profiles — TARGET, chưa classes hiện có

Kiếm S2/S3 dùng chung primary-centered query/order helpers với profiles/caps/powers khác; Cung S2 batch giữ executor riêng khi cần, không ép cùng executor. MobIdentity refs trong typed QuestDefinitions tại G-B trước author Q3–Q12; C0 chặn real combat credit/full VFX theo [Roadmap](../04-production/roadmap.md#combat-micro-slice).

Một authority receiver/clock và vài pure policies đủ P0; đây là các responsibilities có thể chung assembly, không yêu cầu chín services/interfaces. Focus/acquisition không quyết damage; eligibility dùng cùng helpers ở start/arrival/resolve, version profile ghi rõ phase. Secondary selection chỉ query server state, không nhận trusted list từ Client.

| Responsibility | Reads / outputs | Reuse / tách |
| --- | --- | --- |
| Focus/Primary acquisition | Mode/retention/local set→primary key/pending intent | Chung mọi skills; SpawnGroup chỉ navigation priority |
| Skill eligibility | Actor capability/target life/map/range/vertical/LoS policy→accept/reject | Chung checks; phase/profile khác nhau, không shared flag che Bow batch exceptions |
| Secondary selection | Snapshot action + server positions→ordered unique targets/indices | PrimaryProximity Kiếm; SnapshotSpread Bow; PrimaryExplosion Hàn, mỗi policy nhỏ; cùng bounded query/tie helper |
| Damage resolution | Immutable targets/source + target stats→per-index HitResult/ActualHpLost | Một formula/resolver; không đọc VFX hoặc SQL |
| Status application | Landed results/category + cache→status changes | Chung action/target/effect fail cache, unique applications; không mỗi arrow một controller |
| Result publication | Action/life/map/resolve clock/hit indices + status/terminal→authoritative results | Dedup/sequence; không phát duplicate damage khi emit VFX |
| Client presentation/VFX choice | Presentation profileId/action phase/socket/weapon snapshot→pose + một main execution VFX | Data profile theo SkillId; chỉ chọn hình và suppress-default policy, không skill system thứ hai |
| Per-target impact/status | Actual landed target/hitIndex/status expiry→small impact/status visual | Shared pool, unique keys; miss/invalid không wound/status, snapshot status không replay entry |

Presentation profile fields **PROPOSAL**: pose profileRef, mainVfxRef, defaultWeaponVfx policy Allow/Replace, Grip/Tip/Muzzle binding, release/retire visual duration, impactRef/status references và local/remote LOD. Gameplay timeline/target set từ server; visual durations không tăng lock. Một main VFX/action gồm ba arrow trails cho Spread là một composite execution, không ba draw actions. Keys dùng actionId/visualIndex và targetId/generation/hitIndex; pool checkout/release reset socket parent/tint/time/IDs/listeners. Reconnect/late join dựng phase/status hiện tại, không replay damage hoặc old impact. Sorting theo Art; không gameplay colliders trên skill VFX.

**Typed quest definitions tại G-B:** Kill(MobIdentityRef,requiredCount), VariantKill(MobIdentityRef,variantPredicate), BossCredit(BossId/life/area), Talk/Visit/Interact(marker/NPC), ItemRequirement(binding/count), SuccessfulAction(type) là discriminated data đề nghị. Một validator/evaluator theo objective kind đủ, không generic workflow engine. Source provenance phân biệt mob deathId/identity/variant/sourceMap/position/group/slot/active group/RNG outcome và NPC staged grantId/npcId; Q4 tutorialOrdinal optional, không required generic field; group/slot để audit/recovery, không standard credit whitelist. Death snapshot ghi active group trước update; chỉ những objective active ở snapshot nhận credit, không auto-credit group vừa mở. CollectRng(MobIdentityRef,policyRef), StagedGrant(grantRef), RestorationPlacement(targetRef,fragmentBinding,flagRef) nối Q11 direction: material→NPC gate/grant→placement committed flags; maps/order/level/endpoint OPEN theo Quest owner. Q11 không landmark-before-kill hoặc gate-possession graph cũ.

Inventory UI actions/policy theo [Items](../01-design/items-and-economy.md#inventory-ux-policy), controller/selection theo [shared item UI](#shared-item-ui), visual details theo [Art](../03-art/art-and-visual-production.md#icons-ui).
