# Huyền Lộ — Architecture

Các số BASELINE/TUNABLE chưa phải nghiệm thu runtime.

**RULE:** Game Server quyết định realtime combat; backend quyết định dữ liệu bền. Potion đã được server chấp nhận hồi trên timeline combat, không đợi PostgreSQL ACK. [Online & Persistence](online-and-persistence.md#potion-durability) sở hữu durability/outage; [Gameplay Runtime](gameplay-runtime.md#potion-ordering) sở hữu thứ tự tick.

## Document owns

Ranh giới hệ thống, authority/dependency, Local → Dedicated, shared definitions và kỷ luật kiến trúc.

## Document does not own

Luật ngoài domain thuộc owner trong [documentation map](../README.md); evidence thuộc [Playtest & Balance](../04-production/playtest-and-balance.md), thứ tự triển khai thuộc [Roadmap](../04-production/roadmap.md).

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

**UI focus/submenu:** modal giữ đường quay lại (breadcrumb) và selected action/itemInstanceId; không giữ callback tới món đã consume/reset. NPC root chỉ hiện quest/service; service view lấy đúng danh sách. Back theo [Combat & Character — Esc](../01-design/combat-and-character.md#escape-priority); menu shell và physical menu keys còn PROPOSAL/OPEN. Bag grid điều hướng hàng/cột kể cả ô rỗng, detail dùng cùng validators; equipment có cue selected/empty/locked.

Markers đọc quest state, dialogue không tự cấp reward. Class admission kiểm ô Vũ khí trống trước staged grant; tháo/cất là commands riêng.

**ID ổn định:** Skill/Item/Quest/Map/group/slot IDs thuộc definition, không đổi theo thứ tự Inspector/list hoặc display name. Runtime instanceID/actionID/generation phân biệt một đời instance và một action; không dùng GameObject instance hoặc sprite frame làm business identity. Callback từ đời cũ bị từ chối. Client sequence/correlation không thay authoritative result identity.

**Một clock gameplay:** hit/spawn, CD/action lock, status/tick, AI và due-slot dùng cùng timebase. Animator/UI không có timer quyết gameplay riêng; parts của actor đọc cùng state/phase. Deadline bền vững UTC theo các mục liên quan được quy đổi rõ với thời gian phiên; backend giữ timestamp giao dịch/reconciler riêng. AnimationEvent chỉ phát feedback cosmetic: bỏ frame/event không sinh, mất hoặc lặp damage. VFX collision/socket/render bounds không là hitbox authoritative. Hitstop/crit shake/material audio DEFERRED, không dừng clock gameplay.

**Đổi sang Dedicated:** thay adapter nhận intent, physics host/replication, admission/commit; giữ rules và ý nghĩa result. G-N kiểm ít nhất hai Client trước nhân content/art; fixture RAM không chứng minh durability (dữ liệu còn sau lỗi/crash). G-D dùng service credential/PostgreSQL thật và kiểm transaction/recovery trước nhận persistence done. Message fields ở [presentation proposal](gameplay-runtime.md#presentation-data) cần spike, không buộc custom bus từ local.

Task của coding agent phải nêu canonical section/revision, CURRENT/TARGET, input/result và gate liên quan. Thay số gameplay/timer/Boots/26-frame hoặc quyết định OPEN phải ghi giả định/evidence ở Playtest & Balance và đồng bộ owner. Không tự thêm movement lock/state/feature để làm art/count/test pass. Chọn phép kiểm có ý nghĩa cho luật/retry/life/physics; sửa visual nhẹ không cần test chỉ phản chiếu implementation.


<a id="risks"></a>

<a id="11-rủi-ro-kỹ-thuật"></a>

## Rủi ro kỹ thuật

| Rủi ro | Mức | Evidence/gate và xử lý |
| --- | --- | --- |
| Sustain MP, S2 farm thường xuyên, INT/Potion | CRITICAL | Nhịp/MP/power và HP/MP gear mới thay mô hình cũ. Dùng [current balance probe](../04-production/playtest-and-balance.md#current-balance-probe), đo rotation/Food/Potion/zero-INT; sustain sheets cũ là LEGACY |
| Cung chọn phái muộn | HIGH | Persist `ClassChosenLevel` cùng class; thử chọn tại Lv5/Lv6+/cấp cao, reload/equip/reset/clamp, không mất HP nền hoặc cộng passive hai lần |
| Contribution và tranh loot | HIGH | Snapshot level trước reward, không chia lại phần bị loại hoặc fallback TopDamage; N-recipient transaction/claim/crash theo COOP-01 |
| Ticket/lease/DB/outage | CRITICAL | TECH-01/SAVE-01: replay, một writer, schema/startup, terminal-pending, ACK mất và definite reject |
| Escrow settle hai lần hoặc mồ côi | CRITICAL | inviteId/MatchId receipt, khóa hai rows, reconciler; crash HELD/BIND/ACTIVE/SETTLED và đủ mười stakes |
| Checkpoint cũ hoặc heal đè HP mới | HIGH | Generation/sequence/critical ACK/SafeAnchor/HP0; Potion–hit–Food race và pre-Arena boundary |
| Input/focus/latency khó đọc | HIGH | Select-only/Execute keys OPEN, immutable pending, manual cancel/dead observer; AUTO/EXPLICIT/Tab/vertical thresholds TUNABLE. Review tốc độ thường với Dedicated delay/loss |
| Terrain/crowd/Return | HIGH | Natural solid/one-way hiếm/no-climb; occupied≠blocked, authored home/walk, không fallback chống Cung. Full HP tại home; grace/speed/regen/invulnerability/targetability của Return còn OPEN |
| Hybrid và mật độ hiện hành | HIGH | Identity/count OPEN, seed28/66 LEGACY. Probe capability rồi author layout, đo 2/3/4-player contention; toy model không quyết gameplay |
| Boss restart/fairness/TTK | MEDIUM/HIGH | RAM restart có thể tạo Boss sớm. Ghi hạn chế P0 và đo nhịp/telegraph/target/status/reset với balance mới, không kế thừa TTK cũ |
| Dedicated lệch content/headless | HIGH | Pin revision, reject mismatch; kiểm Physics2D/network/N-player trước mở rộng |
| Art/import/editor/QA effort | HIGH | A01/A02/A17 OPEN; import một family, kiểm socket/phase/nhiều actor và đo giờ thật trước nhân families |
| Quest supply/routing hoặc Dev Mode rò vào release | HIGH | Bảy NPC, Q6 mentor, Q10–Q12 Lâm Bá; active-step action credit/physical collection/personal entitlement/full bag/replay. Fixtures không thay fresh journey; quyền/storage dev tách release |
| Observer/network cost | MEDIUM | Đo dead observer đúng MapId/N-player trước optimize P1; chưa công bố capacity |
| Scope P1 tăng ngoài gate | HIGH | Proposal/research không là DoD; chọn sau P0 gate, không framework hóa prototype |


<a id="source-audit-20261009"></a>

## Source audit 2026-10-09 — baseline 229cc61, read-only

Production `game/` và `backend/` chưa tồn tại. Source gameplay hiện có là VS-1 frozen reference; đã đọc cả 16 C# files, README, package manifest và ProjectVersion. Không sửa source/build/scene/assets. Prototype dùng Unity 6000.5.9f1, Input System 1.20.0/Test Framework 1.4.6; Multiplayer Center trong manifest chưa phải NGO/Transport gameplay implementation. Không suy production dependencies hoặc gates đã đạt từ prototype.

Paths dưới thuộc `prototypes/VS1_EndToEnd/Assets/_Prototype/VS1_EndToEnd/`. Class/method là **actual**; matrix ghi **proposed responsibilities**, chưa phải classes đã tạo.

| Subsystem | Actual files/classes/methods | Observed behavior | Trạng thái so TARGET / reuse decision |
| --- | --- | --- | --- |
| Item definitions/instances | [SliceRules.cs](../../prototypes/VS1_EndToEnd/Assets/_Prototype/VS1_EndToEnd/Domain/SliceRules.cs): `ItemDef`, `Catalog`, `Item` | 15 catalog entries; long Instance/int Count/string Binding/double Quality; `ItemDef.Stack=Gear/Manual?1:99`, chưa đủ enhancement/options | PROTOTYPE-ONLY/PARTIAL; tham khảo data, không copy cap 99 hoặc dùng string Binding thay toàn policy |
| Inventory/Storage | `Inventory` trong `SliceRules.cs`: `Capacity=30`, `Merge/Fits/Add/Consume`; `Player.Storage Capacity40` | Bag List + equipment Dictionary; compatibility Id/Binding/Quality; copy/preflight; split vượt 99 reuse incoming Instance ID, chưa checked arithmetic/revision | PROTOTYPE-ONLY; cần planner/IDs/quantity validation/revision production |
| Equipment/stat | `SliceSession.Equip/Unequip/Allocate/Learn/Store`; `Player.Stats` | Local gear swap, một class Sword, manual consume; thiếu current HP/MP bands/ClassChosenLevel/Transfer/Enhancement đầy đủ | PARTIAL prototype; tests mới theo current owner |
| Shop | [SliceHud.cs](../../prototypes/VS1_EndToEnd/Assets/_Prototype/VS1_EndToEnd/Runtime/SliceHud.cs) `ShopBuy/ShopSell`, `SliceSession.Buy/Sell` | Buy 1 item/lần; lists riêng; Sell chỉ value>0 và Bách, Q4 binding exception; chưa quantity/grid shop | PROTOTYPE-ONLY/PARTIAL; không port giant Refresh branching |
| Inventory/UI/context | `SliceHud.DrawInventory/Description/NavigateGrid/ItemActions/Storage` | 6 columns×30 slots, detail/viewport nhỏ; tăng 60 đơn thuần sẽ tràn. Keyboard/mouse chung callbacks nhưng gọi RAM session trực tiếp | PROTOTYPE-ONLY; reuse grid/detail/selection, tách context controllers |
| Quest definitions/progress/events | [SliceSession.cs](../../prototypes/VS1_EndToEnd/Assets/_Prototype/VS1_EndToEnd/Domain/SliceSession.cs) `Quest/Stage/Kills/Objective/AcceptQuest/TurnIn/ObservePosition/Die` | Hardcoded Q1–Q6, một current quest, arrays/ifs/event strings/direct updates; không typed QuestDefinitions hay Q9 concurrent | PROTOTYPE-ONLY; full 12 definitions/runtime NOT IMPLEMENTED |
| Tutorial grant/ground/recovery | `SliceSession.Grant/TutorialSupply/PickUp/Tick`, `Loot.Eligible` trong `SliceRules.cs` | RAM pendingGrants/Receipts; Q4 áo/sample Tutorial flag, TTL 60 s re-offer DS2Home, owner=1; full bag không claim ground | PARTIAL; durable entitlement/RPC ACL/N recipients NOT IMPLEMENTED |
| Regular loot/mob credit | `SliceSession.Landed/Die/Drop`; `Mob.QuestTag/QuestDamage/Generation` | Single-player 20% quest; random drops/death receipt RAM, owner=1; 20/60 windows thiếu contributor phase 8 s/N ledger/atomic durable pile; chưa ground snap | PROTOTYPE-ONLY/PARTIAL; admission/credit/snap/transactions cần production implementation |
| NPC routing/Dev Mode | `SliceSession.Anchors/QuestNpc/Interact/ChooseSword`, `SliceHud.Npc/Dialogue/Marker`; `PrototypePresets.Create`, `SliceHost.ResetPrototype` | Q3 Phong/Q6 Tạ Minh cũ; Diệp Talk nhưng Bow disabled; chưa both-talk gate. F8 preset reset RAM/local pause, chưa durable dev roles | PROTOTYPE-ONLY; rebuild theo Quest owner; presets không thay fresh acceptance |
| Movement/Jump/Drop | [SliceHost.cs](../../prototypes/VS1_EndToEnd/Assets/_Prototype/VS1_EndToEnd/Runtime/SliceHost.cs) `Update/FixedUpdate/IsGrounded/BuildMap` | JumpSpeed 12/gravity 2; release clamp vy>4→4; jumpHeld; fall extra .35, coyote .10/buffer .12. Ignored pairs restore crossing/timeout .65; map clear chưa explicit restore trước destroy | PROTOTYPE-ONLY/PARTIAL; bỏ release branch, kiểm buffer/coyote và per-actor reset |
| Terrain/SpawnGroups | `BlockoutLayout.Surface/Surfaces/MobSupport`, `SliceSession` mob constructor | Ba map; DS1–6 và PROBE DS7/8, strings group/lane/home/activity; natural RearEarth one-way trái current terrain | PROTOTYPE-ONLY; không port old geometry; sources/counts lấy World candidate, chưa eight production roots |
| Facing/art/death | [CombatController.cs](../../prototypes/VS1_EndToEnd/Assets/_Prototype/VS1_EndToEnd/Domain/CombatController.cs) `Facing=1/TryStart`; [GeometricRig.cs](../../prototypes/VS1_EndToEnd/Assets/_Prototype/VS1_EndToEnd/Runtime/GeometricRig.cs) `Pose`; `SliceHost.RenderPlayer` | Facing giữ qua Idle; torso gần static/weapon rotate; dead-gray full outfit, mob view inactive ngay; chưa 3/4 poses/shadow/current corpse presentation | PROTOTYPE-ONLY; reuse facing concept, presentation mới DOC-ONLY |
| Player combat executors | `CombatController.InRange/Witness/TryStart/Resolve/Hit`, `Rules` skill profiles | Arc front cone, Line width0,6/caster-order; snapshot targets start; old Bow spread powers0,9/0,8/0,7 và Sword-only weapon admission; `Hit` chưa StatusController | Frozen reference, **không current implementation**. Primary/propagation C0 phải dựng/test riêng; không copy shape hoặc old powers |
| Input/combat tests | `CombatController.Press/Release/Tick`, `DomainTests/InputBehaviorTests`, `InputPhysicsTests`, `ContinuousRouteTests`, `SliceRouteProbe` | 1/2/3 vừa select vừa execute; HurtPlayer clear focus; variable-jump test đòi hold apex>tap+.5; route releaseJumpAt/Q2.dropped/Tạ Minh | Historical tests, không current acceptance; cần select-only/dead-focus/fixed-jump/new-route tests |
| Persistence/online | Không Java/SQL/backend handler/production RPC; `WorldRevision` chỉ map render | RAM HashSet receipts/pending Dictionary; chưa inventory revision, serializer, login/ticket/lease/DB save | **NOT IMPLEMENTED / DOC-ONLY**; RAM PASS không chứng minh G-N/G-D |

**Recovery re-audit 2026-10-09:** source hiện tại không đổi so snapshot consolidation; đọc lại CombatController/SliceRules/SliceSession/SliceHost và đối chiếu hash toàn source. Không có production handler để refactor ngay. Coding task dựng base nhỏ sau G-B/Pha R, dùng paths trên làm reference và review trước port; [vertical slices](../04-production/roadmap.md#consolidation-slices) không invent existing filenames.

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
| Server Personal quest entitlement | Bounded right/payload | Quest handler | Gameplay; durable via Spring | Pending/Claimed/Consumed/epoch/receipt | Active group/ordinal/eligibility | Entitlement delta | Death/placed/recovery | Regular owner budget/UI | Death rollback/restart | Unique source ordinal; không reroll |
| Server Pickup validation | Alive/map/range/ACL/capacity | Pickup handler | Gameplay | Claim receipt | Ground/right/bag/revision | Add+claim delta | Regular/personal pickup | Client claimed state | Race/full bag/stale rep | Add+claim atomic |
| Server Quest progress | History và derive possession | Evaluator/session | Gameplay | Per-Quest state/ordinal/receipts/activation | Definitions/bag/committed events | Progress/Ready cache | NPC/tracker/turn-in | UI counter/double collection authority | Wrong group/missing items | Event ID/source epoch |
| Server Shop transaction | Catalog/price/quantity/Gold plan | Buy/Sell handlers | Gameplay | Gold/bag/receipts | Catalog/NPC/bag | Purchase/sale delta | Shop commands | Client price/quest grant engine | Overflow/vendor/timeout | Immutable command payload |
| Server NPC interaction result | Resolve context/admission/NPC | Handler/data table | Gameplay | Q6 Talk/class/mentor receipts | NPC/quest/class | Talk/choice/result delta | Quest/service entry | Animation/ambient grant | Range/off-class | Talk gate/class receipt |
| Server Combat/mob credit | HpLost/active ledger/death snapshot | Combat resolver/session | Realtime | Death reward/progress receipts | Mob identity/variant/life/contribution/active quest | Immutable death result | EXP/loot/entitlement | Spring damage engine/VFX | Terminal pending/stale life | Death ID/recipient dedup |
| Spring Durable profile | Committed character aggregate | Backend application | Durable | Profile/bag/quest/class/checkpoint | Auth/DB/definitions | Validated durable delta | Join/commands | Physics/targets/RNG reroll | Revision/lease/credentials | Receipt trước revision |
| Spring Transaction/revision | Atomic shared-result commit | Backend transaction | Durable | Receipts/revisions/claims | Immutable plan/current rows | Related rows một lần | Buy/Sell/claim/turn-in/death N | Trusted client result | Rollback/unknown commit | Unique ID/payload; stable lock order |
| Spring Recovery/lease | Restore/one writer/fencing | Backend admission | Durable admission | Lease/generation/receipt/checkpoint | Lease/committed state | Ticket/lease/reconcile | Rejoin/restart/escrow | Client RAM save | Duplicate session/outage | One-time ticket/reject old writer |
| PostgreSQL | ACID constraints/storage | DB/schema owner | Durable storage | Committed records | Backend transactions | Rows/indexes/constraints | Persistence | Unity tick/UI | Startup/migration failure | Unique IDs/transaction constraints |

**Reuse:** inventory planner, item-policy validators, commit seam, UI widgets/formatter và valid-ground resolver. **Separate:** Buy/Sell/Equip/Storage/Pickup/TurnIn handlers vì admission và atomic deltas khác; regular pile/personal right khác ownership/expiry/recovery dù dùng chung visual. Collection evaluator đọc Inventory; action receipts giữ history, không event bus cho item counters. NPC chỉ cần data table/contextual handler. Spring dùng transaction/receipt boundary chung, không service cho mỗi item noun. Shared views nhận explicit viewmodel, tránh giant screen chứa business rules.


<a id="combat-boundary-review"></a>

## Combat architecture review sau recovery

Reuse một focus/intent foundation, một eligibility library, clock/source snapshots, damage/status resolver và result publisher; chọn ba policy nhỏ Proximity/SnapshotSpread/PrimaryExplosion theo [Runtime boundaries](gameplay-runtime.md#combat-runtime-boundaries). Kiếm S2/S3 share primary-centered query và ordering nhưng profiles/caps/powers khác; Cung S2 snapshot three indices không ép dùng same executor. Hàn có primary-gated explosion. Không service mỗi skill, controller mỗi mob identity, generic graph/event bus hoặc gameplay VFX engine thứ hai.

Client presentation profiles reuse pose/socket/timing/pool/LOD và suppress-default policy; VFX choice và landed impact là render responsibilities khác damage. Server không dependency SpriteRenderer/Animator, renderer không chọn actual victim. Contribution/death result downstream chỉ ActualHpLost và typed objective eligibility; Spring giữ durable boundary, không realtime targeting.

MobIdentity refs vào typed QuestDefinition ngay G-B, trước author Q3–Q12; group/slot giữ placement/query optimization/audit, không credit rule. [C0 micro-slice](../04-production/roadmap.md#combat-micro-slice) chặn real combat-driven quest credit và full VFX production; Inventory/shared UI fixtures vẫn độc lập để bắt đầu code. Không coi all-docs completion là gameplay gate PASS.
