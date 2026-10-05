# Huyền Lộ — Hợp đồng kỹ thuật

## Tóm tắt

Technical giữ cách triển khai TARGET: một authority (phía có quyền quyết định kết quả) trong mỗi phiên, definitions/IDs và đồng hồ gameplay chung. UI gửi intent (ý định thao tác) và đọc kết quả, không tự sửa gameplay. Local fixture chỉ kiểm luồng trong RAM; Dedicated và dữ liệu bền vững cần bằng chứng riêng. Luật và số cân bằng đọc GDD, chi tiết hình ảnh đọc Art.

## Tìm gì ở đâu

- [Kỷ luật kiến trúc](#architecture-discipline), [definitions và timeline combat](#combat-data).
- [Input / PendingCast / CombatFocus](#input-contract), [UI](#ui-notes), [Dev Mode](#dev-mode).
- [Save / SafeAnchor](#profile-authority), [ca kiểm pending/clock](#pending-ordering-probes), [QA](#qa).

**Đối chiếu:** [GDD hiện hành](1_HUYEN_LO_GDD.md) · **Ngày:** 2026-10-06 · **Trạng thái:** specification TARGET; VS-1 là disposable/reference prototype lịch sử, G-L cũ PARTIAL; G-L hiện hành CHƯA CHẠY và production base chưa bắt đầu.

[GDD](1_HUYEN_LO_GDD.md) sở hữu luật gameplay; [Analysis](3_HUYEN_LO_DESIGN_ANALYSIS.md#open-decisions) giữ phép tính và quyết định mở; [Art](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md) giữ chi tiết hình ảnh; [Roadmap](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md) giữ thứ tự làm. P0 bên dưới là TARGET. [Prototype hiện có](../../prototypes/VS1_EndToEnd/README.md) đã chạy build/tests cho revision cũ; đó là bằng chứng lịch sử, không nghiệm thu input/quest/map mới và không quyết định kiến trúc production. CURRENT là đồng bộ docs, thử cảm giác chơi/UI/art, rồi dựng production base trước gate mạng sớm.

**Trạng thái quyết định:** LOCKED là luật đã khóa; STRONG DIRECTION là hướng rõ nhưng cách thực hiện còn phải kiểm; BASELINE/TUNABLE là mốc dùng để thử; OPEN là quyết định chưa chốt; LEGACY/SUPERSEDED là nội dung lịch sử đã bị thay thế. Technical không nâng một phương án thử thành luật gameplay.

<a id="runtime"></a>

# 1. Kiến trúc runtime

P0 chạy **một Unity Dedicated Game Server** headless, **một Spring Boot backend**, **một PostgreSQL** và N Unity 2D Clients. Hai Client là mức nghiệm thu tối thiểu, không phải `MaxPlayers = 2`. Không có player-host, Party service, shard, cloud orchestration hoặc engine combat trong Java.

| Thành phần | Sở hữu | Không sở hữu |
| --- | --- | --- |
| Unity Client | Input, camera, UI, animation, VFX, audio, interpolation; gọi login/character list/ticket qua backend và gửi intent gameplay tới Game Server | Damage, HP mục tiêu, EXP/Vàng, loot, quest/enhance/Boss/PvP result; không gửi character state đáng tin cậy |
| Unity Dedicated Game Server | Physics2D, movement/map validation, skill timeline, combat/status, HP/MP trong phiên, mob/Boss/PvP, contribution/threat, roll và phân phối kết quả gameplay; kiểm intent theo character binding | Account/password, SQL, bản lưu tiến trình dài hạn |
| Spring Boot | Admin tạo account, login, character list, session/ticket, character aggregate; giao dịch bền vững và idempotency cho progression/inventory/quest/loot/Gold/Journey | Physics/combat tick, AI, chọn mục tiêu hoặc roll lại kết quả gameplay |
| PostgreSQL | Account/password hash, tiến trình nhân vật, recovery checkpoint, PvP escrow/settlement receipts và ground loot còn hiệu lực | Static ScriptableObject definitions, projectile/AI/threat hoặc combat state chính xác trong phiên |

**Luồng kết nối:** Client ↔ Spring Boot để login, chọn nhân vật, lấy game ticket; Client ↔ Game Server qua NGO + Unity Transport cho realtime; Game Server ↔ Spring Boot qua internal HTTP API có service credential; Spring Boot ↔ PostgreSQL. Client không gọi backend để cộng thưởng hoặc hoàn thành quest. Backend không nằm trên đường mỗi frame/hit; kết quả làm thay đổi tiến trình bền vững chỉ được báo thành công sau khi commit (xác nhận thay đổi chính thức).

**Một nguồn luật:** Skill/Mob/Item/Quest/Map definitions là ScriptableObject với stable IDs, đóng gói cùng revision vào Game Server build. Client nhận phần cần hiển thị. Spring giữ định danh, definition revision và constraint dữ liệu/giao dịch tối thiểu; không chép damage/drop/enhance thành engine thứ hai trong Java. Riêng escrow/payout/fee/refund PvP do Spring tính theo GDD §8 và §6 bên dưới. Game Server tính gameplay result từ definition; backend chỉ nhận lệnh từ service credential, kiểm session/IDs, expected character revision, idempotency key và cấu trúc giao dịch rồi commit atomic (toàn bộ cùng thành công hoặc cùng thất bại). Definition revision lệch thì từ chối join/mutation cho tới khi đồng bộ. PostgreSQL migrations giữ schema; JSON chỉ cho config/fixture/import-export dev, không là save authority.

Physics 50 Hz, network 20 Hz và render 60 FPS là BASELINE/TUNABLE, cần profiler trước khi hứa throughput. RPC kiểm sender/binding rồi gọi domain function; không custom transport adapter, DI/service bus hoặc distributed messaging. Session admission theo config, độc lập với gameplay; collections theo characterId/playerId hỗ trợ N người. PvP MatchId có đúng hai participant vì mode 1v1. Dedicated build dùng cùng gameplay assembly/definitions với Client; assembly/build target tách presentation, server bỏ camera/UI/audio và chạy headless.

**Unity/tooling:** pin Editor/ProjectVersion/manifest/lock khi dựng production base và kiểm package ở integration gate. Input System cho Client; NGO + Unity Transport là lựa chọn TARGET realtime, chưa có trong prototype. Multiplayer Play Mode (MPPM), Multiplayer Tools/Network Simulator và Unity Test Framework phục vụ dev/QA; ObjectPool chỉ quản lý presentation; Cinemachine 3 cho camera Client. Local Session ở §1.1 phục vụ slice đầu, Dedicated phục vụ gate mạng/final online acceptance. MPPM giúp lặp với nhiều Client, không thay standalone acceptance. Tránh DOTS/ECS, Addressables, Relay, prediction/rollback và cloud/service framework nếu slice chưa chứng minh cần.

<a id="architecture-discipline"></a>

## 1.1. Kỷ luật kiến trúc và Local → Dedicated

**Ranh giới production base đã được chấp nhận; prototype classes/folders không là implementation authority.** Input/UI tạo intent; authority của phiên kiểm binding/state rồi gọi rules/resolver; resolver trả result/state change; presentation đọc trạng thái để vẽ. Chỉ một session giữ quyền thay đổi gameplay trong một lần chạy. UI/PlayerScript không tự sửa HP quái, inventory, quest hoặc EXP.

```text
Input / UI → Intent → Authority của phiên → Rules / Resolver
                                            ↓
                                   Result / State → Presentation
Local: intent gọi session trong process.
Dedicated: intent qua RPC đã kiểm sender/character/session.
```

Giữ ít abstraction: một điểm nhận intent, một clock gameplay, definitions có revision và một điểm commit progression/receipt. Đây là trách nhiệm cần tách, không buộc tên class/interface hay service framework. Local/Dedicated dùng cùng gameplay assembly cho combat/stat/quest/reward. Reuse code prototype phải review/test theo contract mới; không mang nguyên assembly cũ vào production. Authority chạy physics adapter; MonoBehaviour có thể tích hợp physics, nhưng UI/animation không sở hữu luật. Resolver nhận state/definition/clock và trả kết quả dễ kiểm, không cần Text/Button/Animator/RPC/SQL để tính damage.

| Ranh giới | Local slice / fixture dev | Dedicated + backend TARGET |
| --- | --- | --- |
| Nhận intent | Gọi session trong process, bind actor fixture rõ | RPC kiểm sender/character/session rồi gọi cùng domain path; collections N-player |
| Simulation | Local Session tick physics/AI/timeline và sửa state | Dedicated tick headless; Client đọc state/interpolation, không chạy authority thứ hai |
| Definition/result | Stable IDs/revision, stat/quest/loot resolver, action/life IDs | Giữ cùng ý nghĩa; protocol serialization là adapter, không bản công thức riêng |
| Commit progression | Adapter RAM có receipt/revision; inject pending/reject/retry để thử luồng; mất khi đóng phiên | Spring/PostgreSQL theo §6; chỉ ACK bền vững mới báo persistent success |
| Admission/recovery | Profile dev/reset rõ, chưa chứng minh login/save/reconnect | Login/Select/ticket/lease/checkpoint/escrow và outage theo §5–7 |

**UI focus/submenu:** modal giữ đường quay lại (breadcrumb) và selected action/itemInstanceId; không giữ callback tới món đã consume/reset. NPC root chỉ hiện quest/service; service view lấy đúng danh sách. Back theo [GDD — Esc](1_HUYEN_LO_GDD.md#escape-priority); menu shell và physical menu keys còn PROPOSAL/OPEN. Bag grid điều hướng hàng/cột kể cả ô rỗng, detail dùng cùng validators; equipment có cue selected/empty/locked. Markers đọc quest state, dialogue không tự cấp reward. Class admission kiểm ô Vũ khí trống trước staged grant; tháo/cất là commands riêng.

**ID ổn định:** Skill/Item/Quest/Map/group/slot IDs thuộc definition, không đổi theo thứ tự Inspector/list hoặc display name. Runtime instanceID/actionID/generation phân biệt một đời instance và một action; không dùng GameObject instance hoặc sprite frame làm business identity. Callback từ đời cũ bị từ chối. Client sequence/correlation không thay authoritative result identity.

**Một clock gameplay:** hit/spawn, CD/action lock, status/tick, AI và due-slot dùng cùng timebase. Animator/UI không có timer quyết gameplay riêng; parts của actor đọc cùng state/phase. Deadline bền vững UTC theo §6–7 được quy đổi rõ với thời gian phiên; backend giữ timestamp giao dịch/reconciler riêng. AnimationEvent chỉ phát feedback cosmetic: bỏ frame/event không sinh, mất hoặc lặp damage. VFX collision/socket/render bounds không là hitbox authoritative. Hitstop/crit shake/material audio DEFERRED, không dừng clock gameplay.

**Đổi sang Dedicated:** thay adapter nhận intent, physics host/replication, admission/commit; giữ rules và ý nghĩa result. G-N kiểm ít nhất hai Client trước nhân content/art; fixture RAM không chứng minh durability (dữ liệu còn sau lỗi/crash). G-D dùng service credential/PostgreSQL thật và kiểm transaction/recovery trước nhận persistence done. Message fields ở [presentation proposal](#presentation-data) cần spike, không buộc custom bus từ local.

Task của coding agent phải nêu canonical section/revision, CURRENT/TARGET, input/result và gate liên quan. Thay số gameplay/timer/Boots/26-frame hoặc quyết định OPEN phải ghi giả định/evidence ở Analysis và đồng bộ owner. Không tự thêm movement lock/state/feature để làm art/count/test pass. Chọn phép kiểm có ý nghĩa cho luật/retry/life/physics; sửa visual nhẹ không cần test chỉ phản chiếu implementation.

<a id="maps"></a>

# 2. Scene và map trong Unity

Code/tọa độ/lỗi của prototype nằm tại [Roadmap — lịch sử](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#prototype-technical-history), không là contract production. Local production slice dự kiến vào World với profile dev và ba roots theo [VS-1](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#vs-1); session vẫn kiểm MapId/transition. Login không bắt buộc cho combat probe đầu; luồng online TARGET là Boot/Main Menu → Login → CharacterSelect → overlay kết nối → WorldOnline tại recovery map/SafeAnchor.

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

Mỗi MapRoot gồm BackgroundTilemap/GroundTilemap/PlatformTilemap/ForegroundTilemap, structural back/front visuals, các mặt collision, SpawnPoints, MapExits/SpecialGates và NPC anchors. Nước có thể có overlay riêng; visual không tự tạo collider.

**Địa hình LOCKED:** mặt đứng được chỉ ngang; tường đứng và block/step trực giao. Không playable slope/ramp/triangle, collider đi được xoay hoặc diagonal surface. Đất/đá tự nhiên là solid mass có độ dày, mặt trên, mặt đứng và mép khép; đồi/núi bậc liên tục, không dải đất tự nhiên mỏng nổi. Mái/cành/background có thể vẽ chéo, nhưng route chơi trên mái phải author mặt ngang/bậc riêng. Landmark/công trình chỉ dùng vài mặt collision sạch, không polygon collider theo toàn silhouette. **Không ladder/rope/vine/pole/wall climb; không Climb InputAction/state/animation.**

**Solid:** Ground baseline dùng TilemapCollider2D + CompositeCollider2D + Static Rigidbody2D; [Unity 6 Composite Operation = Merge](https://docs.unity3d.com/6000.3/Documentation/Manual/tilemaps/work-with-tilemaps/tilemap-collider-2d-reference.html), không checkbox UsedByComposite cũ. Natural earth/rock luôn solid.

**One-way:** chỉ cấu trúc mỏng hợp lý như ván/giàn/catwalk/ban công/sàn treo có support rõ, hiếm và khác hình solid. PlatformTilemap tách Ground, dùng [PlatformEffector2D Use One Way](https://docs.unity3d.com/6000.3/Documentation/Manual/2d-physics/effectors/platform-effector-2d-reference.html); nếu composite thì UsedByEffector do CompositeCollider điều khiển.

**DropThrough:** chỉ khi actor đứng trên one-way hợp lệ, tạm bỏ collision đúng cặp actor–support; restore khi actor xuống dưới hoặc timeout/cancel/death/transition. Không tắt effector/layer toàn world làm actor khác rơi; một press không chain qua các sàn kế tiếp. PHY-01 kiểm nhiều actor/nhiều tầng, không cần custom collision engine.

**Visual/data:** Background/Foreground không gameplay collider; solid/one-way/decor phải đọc khác nhau theo [Art](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#map-visual). Cell/tile size tính từ pixels/PPU32, không suy từ rig64. HazardTilemap P2. BossCombatArea là region trong Huyền Tích, không scene/root/portal hoặc Lv20 gate; bounds/normal-spawn exclusion đọc GDD §4. Nước nông chỉ giảm nhẹ tốc chạy khi feet contact với vùng nước hợp lệ; trên cầu/trên không không giảm. Slowdown/depth TUNABLE; flow/ripple cosmetic, không swim/drown/fluid subsystem.

**Camera:** Cinemachine chỉ follow character owner, chết vẫn nhìn corpse. Transition đổi confiner theo root và snap/cancel damping qua offset; invalidation cache khi shape/lens đổi, không follow player từ xa.

**Authoring gate:** thử một farm room trước nhân pockets; giữ source IDs quest, SafeAnchor/entrance/route về làng và đường tới loot. Mật độ phải author lại theo [GDD §4](1_HUYEN_LO_GDD.md#world-farm): nhiều SpawnGroup độc lập trong cùng camera, không một blob lớn. **28 groups/66 slots là LEGACY seed**, không budget final; population/pocket totals OPEN/TUNABLE. Tách aggro bằng WalkRegion/topology/vùng địa hình thật, không áp tâm18–20 u cũ vào mọi group. Đo traversal/run-back/contention/CPU/network. Scale visual Linh Biến không tự scale hurtbox/aggro/leash; physics bounds do definition/PHY-01 quyết.

MapDefinition giữ `requiredLevel`, `unlockFlag` và required quest IDs; authority kiểm **Completed**, không ReadyToTurnIn hoặc counter. Locked exit trả lý do rõ, không invisible wall im lặng. Mỗi player có MapId riêng; root activity tính từ toàn player collection trên server, không từ map local của một Client. Render filter không disable simulation/NetworkObject của player khác.

**MapTransition:** EdgeExit giữ targetMap/targetExit/spawn anchor đã author. Authority kiểm actor alive, MapId/generation, overlap đúng exit do manual movement, connected destination và unlock; không nhận arbitrary destination từ Client. SpecialGate có activation riêng theo GDD (Huyền Môn/Arena), không generic Interact cho mọi exit. Cả hai đi qua cùng MapId/checkpoint/cancel pipeline. Một pending transition/actor, dedup trigger/request; destination spawn ngoài return trigger, re-arm sau khi rời exit. Reject báo lý do một lần, phải rời rồi vào lại mới retry; actor khác vẫn dùng được exit. Commit destination checkpoint trước publish (§6); fail giữ nguồn, không gửi destination snapshot giả. Transition dọn target/threat/interaction/visuals map cũ. Collider/safe spawn/camera/pending ACK cần kiểm riêng, không lấy trigger local cũ nghiệm thu online.

<a id="combat-data"></a>

# 3. Dữ liệu và luồng combat

| Definition / state | Trường cần có | Validation |
| --- | --- | --- |
| SkillDefinition / profile | Stable SkillId, class, UnlockLevel/ManualRequirement, slotIndex; power/MP/CD/shape/maxTargets/executor/timeline/status/VFX | SingleMelee/LogicalSingle/Arc/SnapshotSpread/Line/Explosion; không branch theo tên skill, SkillRank hoặc graph editor tổng quát |
| SkillAcquisitionState / PassiveDefinition | LearnedSkillIds; bốn passive IDs/class/unlockLevel/icon/tooltip/effect params | Active học từ manual; passive derive class+level theo GDD §3, không thêm rank/points/duplicate flags; effect enum nhỏ |
| CombatRequest | Character binding, request correlation/sequence, requested SkillId, target identity/life/MapId, facing/aim intent | Authority kiểm binding/alive/map/capability/MP/CD/lock/range/target; Client không chọn actionId kết quả đáng tin |
| HitResult | Authoritative actionId, targetId/life generation, hitIndex, evade/crit/damage/remainingHP | Client chỉ trình diễn result; exact wire fields/time ở proposal bên dưới |
| MobDefinition | mobIdentityId/fixedLevel/baseRigId/paletteRef; stats curve, capability/profile, movement/range/timeline/hit shape/projectile presentation, loot/sourceProfileRef, linhBienEligible | Bảy fixed-level identities trên sáu rigs; palette reuse không cần AI/animation riêng; curve chỉ evaluate fixedLevel |
| LinhBienModifier | Stat/reward modifiers, visual preset, variant tag | Áp trên base runtime đúng một lần; không duplicate base identities/rigs |
| SpawnGroup / SpawnSlot | mapId/groupId/slotId, mobIdentityRef/cached level/spawnPosition; HomeRegion/HomeSpan/WalkRegion/SurfaceId/AggroRange/LeashRegion; deadline/generation/variantState/Q8 waiting set | Vùng đã author quyết đường đi/aggro; cache level phải bằng fixedLevel; respawn không đổi identity |
| ItemDefinition / Instance | templateID/instanceID/GearSlot/rarity/enhancement/count, stat lists; optional buyPrice/explicit sellValue, loot/material/binding refs | Không suy stat từ UI position hoặc fake buyPrice cho drop-only; tutorial-bound tách bản vendor |
| QuestDefinition | prerequisiteQuestIds/requiredLevel; ordered objectiveGroups/sourceIds/markerIds/counts/guaranteedEvidenceOrdinals; giver/turn-in requirements/rewards/unlockFlags | Nội dung theo [GDD §5](1_HUYEN_LO_GDD.md#quests-story), không bảng quest thứ hai |
| QuestProgress per QuestId | state/activeObjectiveGroup/counters/qualifyingKillOrdinal/virtualEvidence; activation/claimed/staged flags và selected mentor/ring khi cần | State riêng từng character/QuestId; backend commit theo revision/receipt §6 |

Production data phải author đủ **12 QuestDefinitions Q1–Q12 và toàn tuyến thật** ngay từ đầu; G-L chỉ thực thi slice Q1–Q6, không cho phép cắt definitions/quest runtime thành bản sáu nhiệm vụ. Bảng mô tả nhu cầu domain/state, **chưa khóa wire schema**. `actionID` trong request cũ chỉ là shorthand correlation; authority mới tạo/bind actionId sau validation. Client sequence/correlation khác ID của action/result/life. Local dùng commit adapter RAM; backend transaction/ACK dưới đây mô tả TARGET, không gọi RAM ACK là durable.

## Quest, vật phẩm và chỉ số

**Quest runtime:** `Dictionary<QuestId, QuestProgress>` là state authority đang dùng; PostgreSQL giữ bản đã commit. Q9 Available/InProgress đồng thời với Q10 Available/InProgress là hợp lệ; không dùng một `currentQuestId` làm nguồn duy nhất. UI có thể pin chính tuyến, nhưng nhánh optional vẫn lưu riêng. Chỉ active group nhận event, đúng source/MapId/filter từ GDD. Q6 chọn phái đồng thời activate nhóm equip/learn/confirm trước action tiếp theo. Q7 confirm kiểm đúng ring instance đang sở hữu và đã ≥+1; nếu đã mặc thì không ép re-equip.

**QuestState:** Locked → Available → InProgress → ReadyToTurnIn → Completed. EligibilityService kiểm đủ prerequisite IDs **và** requiredLevel theo graph GDD, không ngầm dùng Qn−1 cho mọi nhánh. Thiếu level thì trả Locked cùng reason/nextLevel cho NPC/HUD, không hiện marker Available.

QuestService nhận event sau khi hành động server thành công, kèm eventID/characterId/MapId/targetId và payload cần thiết: `NpcTalked`, `RegionVisited`, `ItemPickedUp`, `ItemEquipped`, `ItemSold`, `ConsumableUsed`, `MobKilled`, `VirtualEvidenceCredited`, `AttributeAllocated`, `ManualLearned`, `SkillUsed`, `EnhancementSucceeded`, `PvPCompleted`, `BossEligibilityAchieved`. Kiểm QuestId/active group/target, chặn replay rồi cập nhật counter/step. Pickup/equip/sell/use/enhance thất bại không phát event; dùng bình khi đầy không có Used. Thay đổi inventory/character và quest step từ **cùng action** commit trong một transaction (§6), với commandId/payload bất biến; publish sau ACK. Keydown/click hoặc UI tự báo thành công không cấp credit; không cần event bus lớn.

**NPC và quest:** giver/turn-in/service/MapId/anchor lấy từ definitions theo [GDD NPC](1_HUYEN_LO_GDD.md#ux-art). Q3/Q4/Q7 thuộc Bách Luyện; Q1/Q2, mở Q6 và chính tuyến Q10–Q12 thuộc Lâm Bá. Yên Thảo bán thuốc/hồi sinh/Tẩy Mạch, Mộc An giữ Storage/Rest, Hạo Vũ giữ PvP. Q6 chọn phái và sau đó turn-in tại mentor đã chọn ở Học Viện. Server resolve mentor hợp lệ từ class + quest state (Phong Du/Diệp Lam), không tin turnInNpcId tùy ý từ Client. Class/`ClassChosenLevel`/mentor binding/grant và active group commit cùng action chọn phái; replay không đổi mentor. Tạ Minh là LEGACY reference, không còn current route. Giữ stable internal IDs khi đổi display/ownership; NPC đặt vào khu chức năng, tọa độ còn OPEN, không thêm NPC thay thế.

TurnInRequest kiểm connection/character binding, ReadyToTurnIn, NPC đã resolve, sameMap/range/claimedFlag. Một transaction gồm reward, Completed, story/unlock/summary eligibility và cleanup evidence. Capacity preflight xét compatible stacks và số X slots thực cần; thiếu thì giữ Ready, trả reason+X, không thưởng một phần hoặc cleanup/unlock sớm. Retry trả receipt cũ. Đủ objective chỉ chuyển Ready, không teleport/auto-turn-in. Q12 chỉ Main Story/Chapter III Complete, vòng chơi vẫn tiếp tục.

**Staged grants:** receipt `(characterId,QuestId,grantId)` giữ quyền nhận một lần và pending khi túi đầy. Áo/sample Q4 là supply thật; ground hết hạn hoặc reconnect thì phục hồi entitlement chưa claim với cùng instanceId, không cấp reward lại. Áo bound tới equip, sample tới sell. Q6 grant weapon/manual trước learn/cast; confirm spent≥1 chấp nhận điểm đã cộng trước, không ép point mới khi pool0. MP Potion I reserved có receipt/đường cấp chắc chắn ở đúng step; QuickMP ưu tiên món này, chỉ Used sau khi tiêu thật lúc thiếu MP. Q7 chỉ grant ring nếu thiếu, lưu selectedInstanceId/binding chống sell/drop tới confirm. Đá/Vàng reserved chỉ cấp khi cần +0→+1, không cấp thừa nếu đã ≥+1. Entitlement có pending/spawned/claimed/consumed; không dùng supply ngoài bước cho phép. Q12 Boss death credit và turn-in có receipts riêng; báo NPC không tạo pile nữa.

**Học manual:** sáu manuals ánh xạ tới sáu class SkillIds; class/level/S1 prerequisite cho S2/S3/boundCharacterId theo GDD. Kiểm ownership/alive/idle/no pending action/chưa learned. Consume book và thêm learned SkillId trong một command/receipt. S2 giữ S1, S3 không bắt S2. Cooldown dictionary theo SkillId; đổi slot/học mới không reset deadline cũ. RunningAction giữ profile snapshot; passive derive từ class+level. SelectedSlot chỉ là UX, không cấp quyền cast hoặc tạo DB subsystem. **Manual không có cooldown:** learned/grant/learn receipts chặn lặp; chỉ skill có CD.

**Supply tutorial:** trước khi tạo grant/ground/entitlement, kiểm QuestId/InProgress/active step/source/generation/receipt. Ngoài đúng bước đó chỉ regular loot, không tạo entitlement cho bước tương lai. Retry quyền hợp lệ giữ itemInstanceId, không reroll hoặc hồi tố kill cũ. Mộc Kiếm Q3, thuốc Q6, ring/resources Q7 và reward Q12 không được gây softlock khi túi đầy/reconnect/replay.

**Bằng chứng ảo và credit:** evidence là virtual counters, không nằm trong bag hoặc có physical pickup RPC. Kiểm QuestId/InProgress/active group/source/count còn thiếu; threshold/recipient predicates theo GDD §5–6. Khi gây ActualHpLost, ghi activeQuestId/objectiveGroup/targetGeneration vào quest-qualified ledger; damage trước accept hoặc sai step không hồi tố. Whole-life ledger vẫn phục vụ reward/threat. Death chụp quest eligibility trước tăng step; counters/qualifying ordinal/evidence/receipt commit cùng character mutation. Dùng guaranteed ordinal của chính recipient, không world kill count/RNG, và clamp count. Level penalty không chặn quest/supply. Q11 activation từ ngoài Huyền Môn kiểm ba counters, commit flag idempotent; cleanup chỉ sau turn-in thành công. N players có progress riêng; Talk/Equip/Use/Enhance/Region/PvP không tự share và không phụ thuộc last hit.

**Thêm item vào inventory:** điền compatible stacks trước, overflow tạo stack tới capacity. Commit toàn amount hoặc giữ nguyên ground/pending. Compatibility gồm template/binding/instance flags; tutorial/manual-bound không merge với unbound. Bag Sort P1 không là điều kiện để auto-stack P0 chạy.

**Tính chỉ số trang bị:** ItemDefinition giữ `GearSlot`, `rarityPrimaryStatIds`, `enhanceStatRules`, `fixedSlotBonusDefinition`, `equipLevel` theo [GDD §6](1_HUYEN_LO_GDD.md#gear-economy). Data khai báo HP family Armor/Pants/Boots và MP family Weapon/Ring/Necklace, không suy từ bên trái/phải UI. HP/MP mới chịu rarity/enhance theo primary lists GDD; ACC/EVA, Crit/tốc chạy cố định, flat gains và Tinh Hoa dùng đúng thứ tự GDD. Sáu class weapon templates riêng, không một weapon đổi stat theo người mặc. Quyền nhận drop/mua khác quyền equip; kiểm class/equipLevel tại equip/transfer preview và commit, không suy từ MapId/source. Giữ trần I+4/II+6/III+8; derive Tinh Hoa theo cấp, tooltip cả khóa/mở và reject vượt cap. Retune HP/MP không đổi giá/source.

**Tính chỉ số character:** đọc GDD §2/§3/§6, không copy balance table/formula. `ClassChosenLevel` ghi cấp thực lúc chọn phái cùng class transaction, giữ nguyên khi level-up/reset/reload; Tân Lữ chưa có datum này. Cung tăng HP chậm chỉ sau cấp đã chọn, nên chọn muộn không mất HP nền tại transaction; VIT có hiệu quả như nhau. Cộng nền/attributes/items sau rarity/enhance/flat/Tinh Hoa, rồi passive nền tảng đúng một lần. Giữ số lẻ tới damage rounding/UI, không cộng stat trực tiếp mỗi equip/load. Level-up/class/equip/transfer/reset dùng cùng evaluator; sau ACK giữ currentHP/MP rồi clamp, không ratio-heal/revive. Phiên mới dùng checkpoint HP/MP rồi clamp; Food/status/CD chỉ giữ khi resume cùng phiên. Passive tinh thông chỉ tăng direct skill hit đúng cự ly, không basic Tân Lữ/Bỏng/proc chance; source chụp lúc cast, target/cự ly xét lúc resolve.

**UpgradeTransferCommand:** sourceInstanceId/targetInstanceId/commandId/expectedRevision; kiểm sender, ownership/bag, hai IDs khác nhau, slot/band/class/equipLevel/tutorial binding theo GDD. Chỉ cùng bậc hoặc lên đúng bậc kế, không I→III; preview/commit dùng cùng evaluator, no-gain reject trước mutation, không RNG. Plan tiêu source/cost + đổi enhancement đúng target instance; backend commit một receipt (§6), không consume rồi spawn bản copy. Retry trả target/result cũ dù source đã mất; hai commands tranh source chỉ một thắng. Failure giữ committed state, không spawn lại source để rollback. Equip riêng sau transfer, không auto-equip hoặc cần ô mới.

## Death, loot, spawn và action timeline

**Death/reward:** instanceID UUID duy nhất qua server restart, không GameObject.GetInstanceID/slotId; slotId authored giữ cố định. deathID=(instanceID,generation). Chụp RuntimeMaxHP/ActualHpLost ledger, contributor MapId/position/alive/connected/lastDamageAt và **level trước reward**, rồi mới sửa quest/EXP; sort stable characterId. DoT credit nguồn thật, không last hitter. Predicates reward/quest/Boss khác nhau theo GDD, không dùng một recipient list cho cả ba. Không chia lại phần bị loại; TopDamage chọn whole-life ledger, không fallback né level suppression. Commit immutable roll/recipients/receipts trước publish; retry cùng deathID không reroll.

**Shared loot:** một roll/death theo catalog/rates GDD, exclusive gear buckets ở data. LootRecord giữ itemInstanceId/deathId/MapId/deathUtc/ownerId, contributorSnapshot gồm playerId/levelAtDeath và deadlines. Một OwnershipPhase(deadlines,now), pickup predicates theo mode. TopDamage eligibility100/0 quyết regular set trước roll, không RNG level gate/fallback. Fixed mob identity/level chọn band/slot pool, MapId chỉ chọn material theo GDD. Contributor trong death ledger dùng level đã chụp cho pickup; người tới sau dùng current level. Recheck sameMap/alive/distance/capacity/window; không mất quyền vì chính kill làm level-up. Capacity fail giữ ground.

Claim ground→inventory + claim receipt trong **một PostgreSQL transaction**. Serialize hai claims, loser AlreadyClaimed, không bản thứ hai. Death transaction cùng commit EXP/Gold/quest/Journey/loot/death receipt cho N recipients trước publish. Không two-phase commit/event bus; receipts/deadlines đủ replay/crash safety, cleanup khi death dispatch/loot terminal, không log mọi combat packet vĩnh viễn.

**Spawn arbitration:** một server-owned record/MapId giữ activeSlotId/generation/reservation/waiting characterId set, sửa trên simulation thread. Due slots serialize theo slotId; spawn đọc identity/fixedLevel/position/group, evaluate curve rồi Linh modifier đúng một lần. Lv8+ roll5%/cap1 theo GDD; Lv1–7 không roll. Initial population cùng path; Return/root wake/reconnect không respawn/reroll. Death đặt slot deadline25s BASELINE/TUNABLE; terminal-pending chưa finalize không respawn. Mốc capture deathUtc/release cap/reservation/due ordering còn [A15/TECH-01](#pending-ordering-probes), không suy lethal đã cho phép release trước ACK.

**Q8 reservation:** derive requests từ characterId/QuestId/active step của người hiện diện Trúc Ảnh. Existing `TA4.slot1` Sói Linh hợp lệ thì bind generation; variant khác còn sống thì chờ cap, không demote/despawn/ép Return. Chỉ promote wolf idle/fullHP tại spawn hoặc due respawn. N requesters share waiting set/encounter, không entity per player. Death snapshot xét từng requester theo threshold riêng; đủ credit bỏ request, thiếu retry lifecycle. Force không đổi Linh reward profile. HUD nêu cap/slot đang chờ; replay không nhân request/roll. Restart khởi tạo life mới và rebuild request từ quest progress, không phục hồi variant cũ.

Boss stat/scheduler/shape đọc [GDD §4](1_HUYEN_LO_GDD.md#world-farm). Một action tại một thời điểm; ba vùng đá không double-hit, geometry/né/nhịp cần PlayMode. Q12 không spawn Boss; credit và loot/turn-in receipts tách nhau.

**Canonical combat:** target-based authoritative combat + logical geometry validation. Tại HitMoment, melee kiểm alive/MapId/life/generation/front/facing/vertical overlap/range/attack shape với hurtbox, rồi resolve evade/crit/damage. Sword sprite collision không quyết damage. Ranged resolve logic; tên hình ảnh qua target khác không đổi victim. Arc/Line/Spread/Explosion do authority chọn secondaries, không VFX collider.

**ActionTimeline:** authority kiểm requested SkillId/class/learned/level/weapon/MP/per-skill CD/common action lock/target/geometry. Accepted start tạo actionId/life/startClock, snapshot SkillId/profile/source stats/passive/origin/facing rồi commit MP/CD đúng một lần. RunningAction không đọc mutable selectedSlot. Một timeline cho basic Tân Lữ và các ExecuteSelected; sau class không zero-MP Normal Attack hoặc RepeatOnHold. Nhịp S1/S2/S3 và MP mới là TUNABLE GDD, S2 có thể farm tần suất cao; không hard-code S1 là attack mặc định. Buffer/readiness theo [GDD pending](1_HUYEN_LO_GDD.md#pending-cast). Logical ranged resolve theo clock, không gameplay projectile. Spread ABC/ABA/AAA chụp start, ba hit cùng +0,12s/stable hitIndex; invalid index mất hit, không reacquire. Line sort intersections; Hàn invalid primary không nổ, primary Evade vẫn nổ, secondary roll riêng/no primary double-hit. Dedup actionId+hitIndex+targetId+generation, revalidate life/MapId/shape và DEF/EVA khi resolve. Death/map/hard CC hủy unresolved action không refund; result đã resolve bất biến. Bỏ frame/AnimationEvent/packet trễ không phát damage lần nữa.

**StatusController:** cache `(actionId,actualTargetId,effectId)` cả roll fail; A/A/A tối đa một application/unique landed target/cast, miss/invalid không roll. Chọn category trước roll: Normal/Linh chỉ Freeze; Boss/PvP chỉ Slow theo GDD. Đang Frozen hoặc trước `protectedUntil=thawAt+3s` skip/cached, không deferred proc. Freeze hủy unresolved action; visual đã phát không sinh gameplay sau cancel.

Bỏng giữ sourceCharacterId/ATK snapshot/generation/expiry/nextTickAt. Proc rates/duration/tick đọc GDD; reapply thay source/snapshot/expiry `now+6s` nhưng **giữ nextTickAt**, tick đúng expiry chạy trước remove. Một tick/target, không N×damage; nguồn chết không xóa Bỏng, PvP immune. Target DEF xét ở tick; không Crit/random/INT, passive tinh thông không tăng tick/chance.

Freeze dùng frozenUntil/protectedUntil target-wide, không refresh. Slow một slowUntil theo category, chỉ đặt deadline `now+duration`, không stack magnitude. PvP Slow chỉ MoveSpeed, không interval/CD/animation/action đang cast. Boss Slow nhân tốc reposition theo GDD; clock chờ action kế tiếp giữ `remainingActionWait`, tick giảm `deltaTime×currentClockRate`, debuff hết lại rate1. Không reset full CD hoặc reschedule telegraph/hit/projectile đã start. Cuồng Mạch chọn future base cadence trước, Slow chỉ tác động phần wait. N Cung share một debuff; generation chặn callback cũ kéo dài sai. Death/Return/Boss reset clear status; Client chỉ trình diễn status đã resolve.

**Passive hit:** Kiếm Thế/Xạ Tâm đo từ immutable actionOrigin tới actual target hurtbox center lúc hit/resolve theo GDD, cả secondary xét riêng. Bonus direct skill áp một lần trước DEF; không tăng basic/Bỏng/chance/range. Source capability chụp cast; target assignment không collision-swap/reacquire. Kiếm Tâm/Ưng Nhãn chỉ trong final evaluator, không per-hit proc cache.

## Mob capabilities, vùng hoạt động và crowd

`MobDefinition` cấu hình capability tái sử dụng như GroundMelee, GroundRanged/Hybrid, FlyingRanged và Boss special capability. Movement/range/timeline/hit shape/projectile/loot/rig/palette/Linh eligibility là data; không WolfAI.cs/MushroomAI.cs/BanditAI.cs theo species. Chỉ thêm code khi có capability tái sử dụng mới thật. Fixed-level identity giữ nguyên; Hybrid count/identity **OPEN**, candidate rows GDD không buộc ba ranged behaviors.

Ground mob giữ logical position trong `WalkRegion/SurfaceId` đã author, patrol ở home bounds/HomeSpan. Gặp edge không có ground continuation thì quay đầu, không rơi/Jump/DropThrough/pathfinding nhiều tầng. Hai terrace chỉ nối cho ground AI khi có route trực giao liên tục; route player phải jump/drop không tự là route mob. Flying dùng bounded2D engage band, Ong vẫn melee-accessible bằng Kiếm. Client chỉ interpolate, không full dynamic Rigidbody/nav graph/DropLink.

Passive aggro ưu tiên local/reachable context; hostile hit ngoài AggroRange vẫn wake và cộng threat. Group alert chỉ trong SpawnGroup, không sao chép threat hoặc recursively wake group khác. Threat/contribution theo GDD; sticky threshold baseline vẫn từ owner. Melee khóa origin/facing tại windup, revalidate front/vertical/MapId/alive ở HitMoment; player né thì miss, không guaranteed damage từ start. Ranged release resolve target logic, projectile cosmetic. Return/death/Freeze/map reset hủy unresolved theo action/life IDs; mob không Crit P0.

`HomeRegion` là vùng hoạt động gốc; `WalkRegion` là mặt/vùng đi được; `AggroRange` quyết passive acquire; `LeashRegion/bounds` neo vào home, không đuổi theo target position. Unreachable/outside leash sau grace thì Return theo policy; xét threat reachable khác trước. Return kết thúc encounter, clear threat/contribution/status/cancel action; đích reset HP đầy ở home, không giữ ledger lượt kéo trước. Grace/Return speed/regen/invulnerability/targetability OPEN/TUNABLE; không khóa hồi đầy/miễn damage tức thì lúc bắt đầu Return hoặc vì một cú nhảy. Return không loot/reroll/life mới. CombatFocus có thể giữ Returning target, Execute trả TargetReturning.

Cung kite liên tục trên route hợp lệ có thể no-hit pure melee; không chống lợi thế này bằng Wolf ranged fallback/teleport/jump tầng. Đứng một safe perch unreachable rồi spam mãi là geometry exploit: ưu tiên sửa map authoring và Return đơn giản. Không AI special-case class hoặc nhiều fallback chồng nhau.

**Melee crowd:** `Approach → Contact hoặc Staging → Attack → Recovery/Reposition`. Staging là điểm chờ gần tầm đánh. **Occupied != blocked:** actor đang chiếm chỗ không là terrain wall. Tắt Player–Mob/Mob–Mob body collision, tách hurtbox/query; front mob có thể nhường contact trong recovery, rear mob tiến/chỉnh bước hợp lệ. Không hard formation/four-slot/token/teleport hoặc biến rear melee thành ranged. Threat target không đổi vì điểm visual bị chiếm; mọi lựa chọn xét từng mob/target, không assume hai players.

Soft separation là PROBE/TUNABLE: desired X trên cùng WalkRegion, repulsion nhỏ có clamp/damping/deadband, stable IDs tránh jitter/đổi phía liên tục. Project vào home/walk bounds, không repel qua tường/tầng hoặc Rigidbody.AddForce. Windup/hit giữ origin/facing; steering chỉ approach/staging/recovery. Reposition/lùi trong recovery/interval hiện có, không tăng thời gian/DPS ngầm; terrain blocked thì bỏ chỉnh bước thay vì retry vô hạn. Phase offset ổn định mỗi life không rút interval dưới definition. PHY-01 kiểm 1/2/4 melee ở tường/mép, player đứng/chạy/nhảy, AoE 2–4 targets và N players; không ép flying Ong dùng ground offsets.

Physics masks tách body collision/hurtbox/solid/one-way. Combat geometry độc lập sprite/VFX colliders. LoS A/B và S2/S3 air permissions còn gate PHY-01/ART-01; không biến prototype mask thành production lock.

<a id="shared-combat-input"></a>

## Input, focus và interaction dùng chung

<a id="input-contract"></a>

Input adapter phát semantic actions `Move`, `Jump`, `DropThrough`, `SelectSkillSlot1`, `SelectSkillSlot2`, `SelectSkillSlot3`, `ExecuteSelected`, `CycleTarget(direction)`, `QuickHP`, `QuickMP`, `Food`, `Interact`, `Navigate`, `Confirm`, `Back`. Move ←/→, Jump ↑, DropThrough ↓ và select1/2/3 theo [GDD §9](1_HUYEN_LO_GDD.md#ux-art). Execute/Interact/Potion/Food/menu physical keys **OPEN**; candidate E/F/4–5/R/I chỉ PROPOSAL/TUNABLE DEFAULT cho usability. Không alias gameplay A/D/Space/S cũ hoặc tự gán C/Q.

Select hợp lệ chỉ đổi UX selectedSlot/SkillId, không cast/approach/MP/CD. Locked slot không đổi selection/action. `ExecuteSelected` mỗi physical press tạo tối đa một execution intent; release không hủy one-shot PendingCast, giữ không RepeatOnHold. Authority kiểm requested SkillId từ capability/definition, không tin selectedSlot hoặc secondary list Client. Chọn S2 rồi Execute nhiều lần là luồng bình thường, không special attack pipeline cho S1.

`CombatFocus` giữ mode NONE/AUTO/EXPLICIT và target ID/life/generation/MapId. Tách `search envelope` (vùng tìm), `retention range` (vùng giữ) và `execution range` (tầm thực thi). Lifecycle eligibility khác reachability để cast. NONE+Execute xét skill đã chọn: ưu tiên target local đánh được ngay, rồi target reachable bằng bounded horizontal approach. AUTO sticky, không bị target gần hơn chiếm. Click tạo EXPLICIT, không đánh; xa/khác tầng/blocked vẫn giữ trong retention, Execute reject không swap. CycleTarget dùng local combat set; hướng ưu tiên cùng SpawnGroup → cùng WalkRegion/SurfaceId → nhóm lân cận nhìn thấy/cục bộ → local candidate khác. Order ổn định theo authored group/surface/IDs, không sort lại mỗi frame theo nearest; radius/order/vertical bounds **TUNABLE**.

Đổi/clear focus hủy pending/buffer trước khi bind target mới. Rời execution range/nhảy không clear focus còn trong retention. Target chết/despawn/new generation/life/WrongMap hoặc vượt retention thì clear; Esc/explicit replacement theo context, không tự cast/reacquire cho tới Execute mới. Interaction candidate riêng, `Interact` nhặt/dùng ngay không mutate CombatFocus.

**Player death và target HUD:** hủy PendingCast/BufferedIntent/approach, chặn combat input và cancel unresolved action theo timeline, **không clear CombatFocus chỉ vì player chết**. Retention/target-life validation không phụ thuộc owner alive; dead actor còn trong MapId vẫn nhận target state snapshots, marker/name/level/current-maxHP/bar cập nhật khi người khác đánh. Target invalid mới clear theo các điều kiện trên; same SpawnSlot respawn có life/generation mới, không inherit focus cũ. Resume trong phiên còn sống giữ focus nếu vẫn hợp lệ; mất phiên không persist focus trong checkpoint. Corpse camera/loot/quest eligibility vẫn theo GDD, không từ HUD.

`PendingCast` giữ requested SkillId/target ID/life/generation/MapId/intentId/expiry/start/progress position/heldMovementMaskAtPress; chưa chụp source stats/action hoặc commit cost. `BufferedIntent` chỉ một execution intent mới nhất, có readiness/expiry theo authority clock. `RunningAction` đã accepted có snapshot riêng. Select đổi UX nhưng không mutate/cancel ba state này; **Execute mới** mới thay PendingCast/BufferedIntent, không sửa action đang chạy.

| Bước | Contract triển khai |
| --- | --- |
| Execute press | Chụp selected SkillId/target life/MapId và từng horizontal binding đang held; resolve/validate capability trước tạo pending/buffer |
| Input arbitration | Bỏ held mask cũ trong pending/buffer kể cả ngược hướng; KeyDown ngang mới/Jump/Drop/focus/UI/chat/Esc/death/map/invalid target hủy intent. Terminal trả quyền axis còn held |
| Buffered readiness | `max(remaining skill CD, remaining common lock)` phải sẵn trong window0,18s BASELINE/TUNABLE. Không approach trước ready, không FIFO/long-CD queue; expiry clear/reason |
| Approach path | Grounded/same reachable lane/wall/edge/progress và swept segment với exit; cắt EdgeExit thì Blocked trước movement/cost. Chỉ manual movement kích hoạt exit, không AssistAxis |
| Arrival validation | Target đúng life/generation/MapId/alive/notReturning; actor alive/no hard CC; learned/class/weapon/level/MP/CD/lock và range/shape tại origin thực. Fail clear/reason; success mới tạo actionId/snapshot/MP/CD đúng một lần |
| Cancel/reject | Blocked/no progress/timeout/invalid hoặc lifecycle/user cancel đều clear, không retry ngầm/chuyển target. Release Execute không cancel; hold không thêm intent |
| Esc dispatch | Consume đúng một tầng [GDD](1_HUYEN_LO_GDD.md#escape-priority); đóng UI/chat không rơi xuống cancel/clear focus cùng press |

Reject enums gồm PlayerDead/HardCc/NotLearned/WeaponRequired/InsufficientMp/Cooldown/ActionLocked/NoTarget/TargetMissing/TargetDead/TargetGeneration/TargetReturning/WrongMap/OutOfRange/Blocked/BufferExpired/UserCancelled. UI dịch reason thành text, không dùng text làm control flow.

Approach budget tính đoạn còn thiếu ngoài execution range của từng profile, không dùng một distance chung cho Kiếm/Cung. Chỉ chạy ngang với tốc chạy thường trên lane đi được; timeout/progress/budget cụ thể TUNABLE. Không tự jump/drop/dash, tìm đường nhiều tầng, nối pocket xa hoặc đổi target. Arc/Line kiểm victim từ origin dự kiến có thể tới trước assist, rồi kiểm lại từ origin thật trước commit; không tự đi tới secondary targets. Geometry vẫn có thể miss lúc resolve. Gravity/momentum tiếp tục; air cast Tân Lữ/S1 cần probe, quyền S2/S3 trên không còn OPEN.

**Ranh giới UI:** keyboard/mouse dùng cùng action list/validator/command. Modal giữ selected action ID; renderer vẽ focus/lý do disabled rồi dispatch, không sửa progression. NPC mới mở ưu tiên quest action; consume input mở UI, không Confirm lần hai cùng frame. Khi list đổi, giữ selected ID nếu còn hợp lệ, nếu không thì clamp index; không gọi callback món/session cũ. Mở/đóng UI/transition/reset dọn pending gameplay và input capture; giữ phím không sinh Execute mới. Enter là Confirm trong modal, Chat ở world. Tab/Shift+Tab điều hướng view trong UI, CycleTarget ngoài UI/chat, không dispatch cả hai. Bag filter theo GearSlot từ inventory hiện có; Equipment/Attributes/Derived Stats là views riêng cùng evaluator. Preview/world đọc cùng Art pose/socket contract. NPC text hiển thị kết quả command đã commit.

<a id="movement-feel"></a>

**Cảm giác di chuyển — PROBE/TUNABLE:** gameplay dùng phím mũi tên. Khi nhả Jump, cutoff điều chỉnh độ cao nhảy; coyote time tính từ lần đứng trên support cuối cùng. Jump buffer có hạn, chỉ consume một lần và một press tối đa một jump. Grounded kiểm chân/support và chiều chuyển động, không dùng sprite bounds. Độ cao nhảy thay đổi bằng cutoff/extra gravity khi đi lên; acceleration/deceleration riêng cho ground/air. Tốc trần dùng MoveSpeed evaluator gồm AGI/Giày/Slow/nước. Tuning tốc rơi không đổi pose clock. Duration/force chưa khóa; dùng authority clock/fixed physics, không phụ thuộc render FPS. Coyote/buffer không tạo double jump; dọn state khi death/map/UI/Freeze theo lifecycle.

DropThrough trên solid không có tác dụng và không chặn Jump. Trên one-way, Jump+Drop cùng frame thì Drop thắng, consume pending jump/coyote để không bật ngược. Chỉ bỏ collision actor–support hiện tại rồi restore; giữ Drop không xuyên sàn tiếp theo, cần press mới. Probe nhấn nhanh/giữ phím, jump sớm/muộn ở mép, tường/trần/landing, drop nhiều tầng, air cast, AGI/Slow và render 30/60/120 FPS. Review thủ công ở tốc độ thường quyết cảm giác; automation kiểm invariant. Physics2D sở hữu collision/grounded/triggers/drop pairs của player/world; combat geometry riêng. Không thêm slope/climb state hoặc khóa movement để sửa lỗi art.

<a id="presentation-data"></a>

## Dữ liệu presentation cần thử ở spike — PROPOSAL

Chuyển từ Art §20; đây là nhu cầu message/state, **chưa khóa schema/transport**. [A11/A15](3_HUYEN_LO_DESIGN_ANALYSIS.md#art-open-decisions) theo dõi validation. Local Session xuất cùng ý nghĩa result/state với Dedicated; Client không có quyền quyết result.

Fields ứng viên gồm actionId/request correlation, actor/life generation, MapId/profileId/startClock/timeline revision; visualId/equipment revision chụp lúc cast; visual index/origin/aim/travel duration; logical hitIndex/resolveClock; actualTarget/life/evade/crit/damage/remainingHP; status kind/source/expiry; terminal/cancel/lifecycle phase. Target HUD cần identity/generation/currentHP/maxHP snapshot dù owner đã chết. Đây là fields authority gửi để hiển thị, không dùng Client echoes làm dữ liệu đáng tin. HitResult cũ chưa đủ timestamp/generation cho packet trễ hoặc pool reuse; exact message cần spike.

Network 20 Hz ≈ 50 ms/snapshot, physics 50 Hz = 20 ms/tick, render 60 FPS ≈ 16,7 ms/frame là BASELINE. Không stream từng sprite frame; clock/phase/profile chọn pose deterministic (cùng đầu vào cho cùng kết quả), correction theo action/state/result. Tick lệch không biến một hit batch thành truyền target A→B→C. Remote interpolation và local cosmetic anticipation giữ cùng ý nghĩa gameplay; độ trễ phải đo. Client căn packet trễ về phase còn hiệu lực, không chạy damage lại.

<a id="network-authority"></a>

# 4. Quyền quyết định qua mạng

| Client gửi intent | Game Server quyết định | Backend khi cần dữ liệu bền |
| --- | --- | --- |
| Move/Jump/Drop, Execute requested SkillId/target hint, facing/aim | Physics/target/range/MP/CD/hit/damage/status; broadcast state/result | Không gọi mỗi tick/hit |
| Pickup/equip/sell/shop/upgrade/transfer | Kiểm MapId/NPC/range/mode/bag ownership; tính roll/preview từ definitions | Inventory/Gold/equipment atomic, unique IDs/receipt/revision; trả canonical snapshot |
| Quest/class/attributes/manual | Kiểm event/NPC/step/level/entitlement; resolve mentor/ClassChosenLevel và outcome | Counters/reward/unlock/class/ClassChosenLevel/learned/points cùng transaction |
| Map transition/chat/PvP | Map/state/rate/match; realtime outcome và Journey entitlement | Escrow trước MatchId; payout/phí/refund/Journey/receipt theo MatchId |

Game Server PlayerRegistry theo characterId, connection binding/MapId và threat/contribution/recipients đều collections N-player. ConnectionId chỉ là handle tạm; sender lấy từ transport, không playerId trong payload. Direct Client API P0 chỉ account/list/ticket, không internal mutation. Combat không gọi backend trừ mutation bền như reward/tiêu item/quest. Client có thể dự đoán hình ảnh, không damage. Actor chết vẫn nhận world/target snapshots hợp lệ, không vì observer filter alive mà mất HUD.

**PvP escrow:** Game Server giữ invite/MatchId đúng hai participants, Countdown/Active, HP/MP/Food/CD/quota bình và realtime result. Stake từ tập GDD, accept khớp exact stake. Spring khóa hai character rows theo stable ID, kiểm đủ tiền/lease và tạm giữ stake mỗi người cùng **một transaction inviteId**, đồng thời ghi pre-Arena checkpoints. Bất kỳ phần nào fail thì rollback (hoàn tác) cả hai, không MatchId. Sau ACK HELD mới tạo/bind MatchId, countdown; backend mark ACTIVE phải ACK trước combat/clock120s. Không trừ tiền lại khi kết thúc.

**Outcome/settlement:** Game Server gửi WIN/LOSE/DRAW/FORFEIT/SYSTEM_ABORT + MatchId. HP0 thắng; disconnect sau ACTIVE xử thua ngay, không reconnect grace trong trận; trước ACTIVE cancel/refund100%. Hai disconnect cùng tick/không chứng minh thứ tự hoặc lỗi server/backend rớt cả hai thì SYSTEM_ABORT, không dựa callback order để chọn forfeit. Hết120s còn sống là DRAW, không so HP. Spring tính pot/fee/refund theo GDD, commit Gold/Journey/settlement receipt một transaction, không chạy combat lại. WIN/FORFEIT winner90%pot; DRAW mỗi người90%stake; pre-active cancel/SYSTEM_ABORT100%. Retry MatchId trả receipt cũ. Backend lỗi chờ refund khi phục hồi; crash orphan reconciler100% chỉ nếu chưa settled, ACK mất sau settlement không refund thêm. Không dùng PvE receipt cho PvP.

**Trong Arena:** Food tick/hết hạn theo thời gian thật; bình thật/CD riêng HP–MP theo GDD. Quota3+3 mỗi người chỉ tăng sau consume commit, reject quota không tiêu item; cấm Hồi Sinh Phù. Entry đầy HP/MP; RAM chụp pre-match để người còn phiên về Vân Khê với HP/MP cũ/clamp, Food không hoàn thời gian, bình không hoàn. Mất phiên load checkpoint trước Arena, không tọa độ Lôi Đài. Q9 credit trận thật WIN/LOSE/DRAW, không FORFEIT/abort; forfeit winner vẫn Journey theo GDD.

P0 local/LAN: backend/server bind địa chỉ config; localhost chỉ khi cùng máy, Client khác dùng server reachable. Internal API chỉ Game Server qua HTTPS hoặc LAN cô lập có service credential; credential không trong Client/public build. Không claim Internet-ready hoặc commercial anti-cheat.

<a id="profile-authority"></a>

# 5. Tài khoản, nhân vật và phiên chơi

**Tài khoản do admin tạo:** không có Register cho player. Admin dùng seed script/CLI hoặc endpoint admin được bảo vệ riêng để tạo username duy nhất, password hash bằng BCrypt/Argon2 của Spring Security qua package đã pin, trạng thái enabled và character test. Không lưu plaintext password trong repo/log. Character Select vẫn hỗ trợ nhiều character/account; P0 chưa cần Character Create UI khi admin cấp sẵn. `demo_new`, `demo_class`, `demo_mid`, `demo_end_sword`, `demo_end_bow` chỉ là tên fixtures, không phải authority của save.

**Luồng màn hình:** Boot/Main Menu → Login → Character Select → connecting overlay → World theo recovery checkpoint; character mới bắt đầu tại Vân Khê. Không thêm email/OAuth/forgot password, server browser hoặc Loading Scene riêng. Spring login trả session token ngắn hạn cho API client. Character Select đọc `characterId, name, level, class` thuộc account, không dùng inventory/Gold do client gửi làm dữ liệu đáng tin.

Chọn character gọi Spring lấy **one-time game ticket**, TTL 60 s, ràng `accountId, characterId, sessionId, expiry, nonce`. Client gửi ticket tại connection approval. Game Server dùng service credential đổi ticket qua internal API; backend kiểm login/ownership/expiry/nonce, chưa consumed và chưa có active lease. Consume ticket và cấp lease trong **cùng transaction**. Ticket replay/hết hạn bị từ chối; client tự gửi characterId không chứng minh quyền. Token không vào chat/log; P0 dùng TLS hoặc LAN cô lập.

**Một writer cho mỗi character:** Spring giữ lease `(characterId, gameServerInstanceId, sessionGeneration, expiresAt)` và revision. Game Server heartbeat mỗi 30 s; lease hết hạn sau 90 s không heartbeat. Sau join, Game Server cấp resume token ngẫu nhiên, một lần dùng, ràng characterId/sessionGeneration. Transport rớt thì giữ actor tối đa 15 s trong world, vẫn chịu AI/damage/timer nhưng không nhận input. Resume đúng server trong grace sẽ rotate token và gắn connection mới vào cùng runtime position/HP/MP/Food/CD/status; không tạo actor thứ hai.

PvP ACTIVE là ngoại lệ: disconnect xử FORFEIT ngay, không resume vào trận; trước ACTIVE cancel/hoàn cược. Hết grace hoặc phiên bị hủy thì checkpoint cuối nếu backend còn hoạt động, despawn và release lease. Ticket mới bị chặn khi lease cũ còn sống; phiên mới chỉ nạp PostgreSQL sau grace/lease expiry. Crash/outage có thể chỉ giữ checkpoint đã commit gần nhất. Mutation/checkpoint từ generation cũ bị từ chối.

**Khôi phục phiên mới:** Game Server nạp canonical progression/checkpoint, derive MaxHP/MaxMP rồi clamp HP/MP lưu sẵn. Farm/combat dùng SafeAnchor của checkpoint MapId. Vân Khê/Học Viện có thể dùng exact coordinate đã checkpoint nếu còn walkable, trong bounds và ngoài trigger exit/gate; sai thì SafeAnchor map đó. Lôi Đài không là recovery map: dùng checkpoint pre-Arena về Vân Khê/pre-match state. HP=0 vẫn dead, corpse ở SafeAnchor và giữ lựa chọn về làng/dùng phù; reconnect không hồi sinh miễn phí. Character mới chưa checkpoint bắt đầu Vân Khê đầy HP/MP. MapId mất hiệu lực sau đổi content thì fallback Vân Khê nhưng giữ HP/MP đã clamp.

Mất phiên xóa Food/status/CD/action/threat và focus; resume ngắn giữ runtime phiên còn sống. Death trong phiên khác session loss: focus hợp lệ vẫn quan sát được theo [input contract](#input-contract). Runtime PlayerId ánh xạ characterId, không phải NGO clientId.

Điểm vẫn tuân `spent + unspent = 5 × (L−1)`; reset nhập môn một lần, late class không set tổng điểm về 20. `ClassChosenLevel` phải được ghi cùng class transaction, giữ nguyên khi level-up/reset/reload; evaluator dùng [GDD](1_HUYEN_LO_GDD.md#class-combat) để tránh mất HP khi chọn Cung muộn. Fixture đã có class phải author datum này rõ ràng. Dữ liệu class cũ thiếu datum cần migration/import có nguồn xác nhận; không đoán bằng current level hoặc silently đổi HP nền. Demo import chỉ vào account/character test riêng.

<a id="persistence"></a>

# 6. Lưu dữ liệu — Spring Boot + PostgreSQL

**Dữ liệu bền theo character:** level/EXP/class/`ClassChosenLevel`/attributes/points/reset flag, Gold/Journey, inventory/storage/equipment instances, learned SkillIds, per-QuestId progress/entitlements, story/unlock/summary flags, receipts và checkpoint tối thiểu `(MapId, HP, MP, safePosition nếu ở khu an toàn, checkpointSeq, sessionGeneration, updatedAt)`. `ClassChosenLevel` nullable chỉ khi chưa class; chọn phái commit class và datum này atomically. Quest supply bindings/selectedInstanceId/mentor route cần đủ dữ liệu để retry đúng instance và đúng mentor, không client chọn tùy ý.

Checkpoint không giữ tọa độ farm/combat chính xác, Arena, Food tick, CD, pending cast, status, projectile, threat/contribution hoặc AI/Boss HP. `isDead` derive HP=0. Restore clamp HP/MP khi Max stat đổi, không tự heal. PostgreSQL là authority của state đã commit; RAM Game Server là authority realtime trong phiên.

**Schema tối thiểu:** `accounts(id, username UNIQUE, password_hash, enabled)`; `characters(id, account_id FK, name, level, exp, class_id, class_chosen_level nullable, gold, journey, reset_used, revision, lease_generation, ...)`; `character_attributes(character_id PK/FK, str, vit, int, agi, unspent)`; `item_instances(id PK, character_id FK, location/slot, template_id, rarity, enhancement, quantity, binding)`; `character_skills(character_id, profile_id PK pair)`; `quest_progress(character_id, quest_id PK pair, state, active_group, counters/ordinals, entitlement_state)`; `character_unlocks(character_id, flag_id PK pair)`; `recovery_checkpoints(character_id PK/FK, map_id, hp, mp, safe_x nullable, safe_y nullable, checkpoint_seq, session_generation, updated_at)`; `mutation_receipts(command_id PK, result, created_at)`; `world_loot(item_instance_id PK, death_id, map_id, payload, ownership_snapshot, deadlines, claimed_by nullable)`; `game_tickets`/`character_leases`; `pvp_escrows(invite_id PK, two_character_ids, stake_each, match_id UNIQUE nullable, phase, server_instance_id, active_at, outcome, settled_at)` với unique settlement receipt theo MatchId. Quest counters có thể là versioned JSONB data. Không save bằng file hoặc lưu static Item/Mob/SkillDefinitions trùng ScriptableObject.

**Checkpoint cadence — BASELINE:** mỗi 30 s cho actor ngoài Arena; không ghi mỗi hit/Food tick. Ghi critical checkpoint tại map transition (commit destination MapId trước chuyển), orderly logout, hết reconnect grace, death, revive/về làng và trước/sau PvP. Ngoài Arena, Potion consume và checkpoint HP/MP sau heal cùng transaction. Trong Arena, chỉ consume item + receipt; không đổi pre-Arena HP/MP/map/position. Pre-Arena checkpoints của hai người commit cùng escrow; kết quả/abort đưa người còn phiên về Vân Khê với HP/MP pre-match/clamp.

`sessionGeneration + checkpointSeq` tăng đơn điệu; backend từ chối packet cũ đến sau transition/death/escrow. State lấy từ Game Server. Periodic lỗi retry cùng ID; crash tải checkpoint cuối đã commit, có thể trễ khoảng một chu kỳ trừ mốc critical. Inventory/progression đã commit không mất theo checkpoint. Logout chờ critical ACK; backend lỗi kéo dài thì đóng phiên có thông báo, lần sau dùng state bền cuối.

**Ranh giới transaction:** Game Server tính gameplay result một lần từ definitions và snapshots: deathID, contribution, quest-qualified credit, rolls, enhancement RNG, inventory preview. Spring không chạy lại combat/drop formula. Internal service kiểm credential/lease/generation/commandId/expected revisions/IDs/capacity/ownership, commit deltas và canonical result. Kiểm receipt **trước revision mới**; duplicate command trả receipt đã commit, command mới phải đúng lease/revision.

Một death với N recipients và shared pile commit rewards/quest credit/loot/death receipt chung hoặc rollback toàn bộ. Khóa character rows theo stable ID. Trong terminal-pending, mob không nhận hit mới, không respawn, chưa publish reward; finalize sau ACK. Claim ground→inventory cùng claim receipt trong một transaction, hai claims chỉ một thắng. Turn-in preflight capacity rồi commit reward/Completed/unlock/cleanup chung. Enhance fail vẫn commit cost/receipt; transfer tiêu source/cost và sửa target chung. Action chọn phái Q6 commit class/`ClassChosenLevel`/mentor binding/grant/active group chung. Turn-in là action riêng sau khi đủ objective, được kiểm tại cùng mentor và có receipt riêng.

**Retry và lỗi backend:** mutation bền commit ngay, chỉ publish success sau ACK. Potion effect chỉ áp sau consume + checkpoint commit; giữ thứ tự hit/action, không spam hoặc miễn damage. Serialization với hit/Food tick xen giữa và definite reject còn [pending probes](#pending-ordering-probes); snapshot đang chờ ACK không được đè HP mới. Logout chờ command/escrow/checkpoint ACK rồi release lease; không snapshot toàn world.

Timeout chưa biết commit thì giữ immutable payload/commandId trong RAM và retry có backoff. Backend/DB lỗi thì pause mutations/reward-generating actions/PvP, báo gián đoạn. PvP không tiếp tục khi backend không thể settle: SYSTEM_ABORT, refund khi backend phục hồi. Hết lease đóng phiên, chặn generation cũ. Chưa commit thì chưa được công nhận; đã commit mất ACK thì receipt trả đúng result, không roll/grant lần hai. Definite reject cần reconcile theo probe, không retry cùng lỗi vô hạn. Không Kafka/event sourcing/distributed transaction.

**Escrow recovery:** `inviteId` có trước MatchId. HELD transaction khóa hai rows, kiểm stake/tiền, trừ mỗi người `W`, lưu pre-Arena checkpoints; retry cùng inviteId không trừ lại. ACK HELD mới tạo/bind MatchId; countdown rồi mark ACTIVE phải ACK trước combat. Invite chưa accept hết hạn không trừ tiền. HELD/BIND lỗi, disconnect pre-ACTIVE hoặc server crash refund100% idempotently.

Game Server gửi WIN/FORFEIT với winnerId/loserId thuộc đúng hai participants. Spring tính từ W: winner `2W × 0,90`; DRAW mỗi người `W × 0,90`; SYSTEM_ABORT mỗi người `W`. Phí và Journey +200 cho winner cùng settlement receipt trong một transaction khóa hai rows. Receipt kiểm trước revision; terminal outcome đã commit chặn outcome khác. Reconciler hoàn HELD chưa bind quá 60 s hoặc match mồ côi khi server heartbeat mất quá 90 s, **chỉ nếu chưa settlement receipt**; race kết quả được serialize bằng lock/unique state. Outage thì refund pending tới khi phục hồi; UI không báo đã trả trước ACK. Potion đã consume hợp lệ không hoàn theo tiền cược. P0 không cần payout service riêng.

**World recovery:** `world_loot` đã commit được restore với deadline UTC gốc nếu chưa hết hạn; cleanup idempotent, không reroll. Normal slot generation/variant, Boss HP/phase/threat/respawn deadline chỉ ở RAM. Restart tạo normal lives mới và **một Boss Alive đầy HP**, không phát credit/loot encounter cũ. Boss có thể xuất hiện sớm sau restart: hạn chế restart chủ đích trong demo, ghi scope P0 vào QA; chỉ thêm persisted deadline nếu evidence exploit cần. Q8 waiting set rebuild từ progress của người hiện diện, không variant cũ. Crash PvP refund100% khi backend phục hồi, không Journey; match đã settled giữ receipt, không đổi abort. DB corrupt/migration fail chặn gameplay startup, không khởi tạo world như chưa có dữ liệu.

<a id="timers"></a>

# 7. Đồng hồ và map rỗng

| Timer/state | Authority | Map rỗng / restart |
| --- | --- | --- |
| Normal/Linh slot deadline, Q8 reservation | Game Server RAM, global SpawnManager | Map rỗng có thể pause AI; due slot resolve một lần khi vào lại; restart tạo slot life mới và roll theo spawn mới |
| Ground loot deadline/claim | Game Server chọn cửa nhặt, Spring/PostgreSQL lưu pile/claim | UTC tiếp tục khi map rỗng; restart chỉ restore pile chưa hết hạn |
| Boss spawn/death/HP/phase | Game Server RAM | Countdown không cần người ở map; restart một Boss Alive full HP, bỏ encounter/threat/contribution cũ |
| Skill/Food/Potion/status và exact HP/MP/position | Game Server session clock/RAM | Resume ngắn giữ runtime; session loss nạp checkpoint, bỏ Food/CD/status và dùng SafeAnchor |

Runtime combat dùng một server clock, không trust client timestamp. Timer quá hạn xử lý idempotently, không catch-up tạo nhiều Boss/pile. WorldBossManager theo dõi actual-loss direct/DoT, clear khi wipe; scheduler/Slow/Cuồng Mạch theo GDD. Boss death commit shared pile/per-recipient credit qua §6 trước broadcast; Q12 không sinh Boss mới. P0 không restore giữa trận. MapId correctness độc lập presentation hide/show.

<a id="art-contract"></a>

# 8. Hợp đồng tích hợp art và animation

[Art](4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md#art-integration) sở hữu pose/frame mapping, module/catalog, sockets, sorting và import workflow. Technical giữ yêu cầu runtime, không có catalog hoặc sorting table thứ hai. `26 frames` là USER-LOCK; A01/A02 còn OPEN về raster/profile mapping, không tự đổi thành 33 frames. Rig baseline 64×64 px/PPU32/Bottom-Center/Point filter và state indices phải đối chiếu Art gate. `12 modules` cũ chỉ ba band×bốn loại, thiếu Mộc/fallback: LEGACY workload reference, không tổng asset hiện hành.

Một shared state/frame controller điều khiển modular parts; parts không chạy Animator clock riêng. Pose/phase/flip của front/back weapon và sockets đồng bộ. Physics root/hurtbox tách VisualRoot: thay gear/flip/scale không tự đổi collider. Collider baseline khoảng 0,60–0,65u×1,45u TUNABLE, không lấy toàn canvas. Attack origins author theo timeline, không sprite bounds. Không Climb state/animation. Module names/schema phải theo Art hiện hành, không giữ Pants/Lower hoặc Hair aliases thành hai slot.

**Tích hợp map:** environment families Forest/Mountain/Ancient và hub props reuse theo Art. Tile/cell size derive asset pixels/PPU, không suy từ 64px rig. Background/decor/foreground không collider. Natural ground là solid mass; one-way chỉ mặt mỏng nhân tạo có support hợp lệ theo §2. Validate movement/jump/drop với nhiều actor trước decorate. Foreground phải giữ telegraph/loot/nameplate/chat đọc được; không thêm lighting/streaming subsystem P0.

Stable MapId/NPC/region/slot/exit/gate/SafeAnchor và Boss exclusion được kiểm trên authored layout; seed manifest cũ không buộc current totals. Camera local follow/confiner/impulse đúng bounds và transition ACK. Kiểm hai hướng/cao độ/tint, wolf palette khác aura Linh. Ground mob route không trở thành route jump/drop chỉ vì asset có bậc. Nước chỉ shallow feet-contact visual, không swim/underwater physics.

**Feedback và pool:** renderer nhận Game Server event. AnimationEvent/FX không gây damage/stun hoặc pause simulation. Pool dùng chung configuration cho projectile/telegraph/status/hit/damage/NÉ/heal/upgrade/death/slash. Reuse reset tint/timer/owner/action/life, release hủy listeners/timers. Callback kiểm IDs/generation, không GameObject reference làm lifetime identity; pooled projectile chỉ presentation. Client bắt đúng phase còn hiệu lực, không replay gameplay khi packet trễ.

Hit animation không tự stun; Freeze Normal/Linh đọc rõ bằng overlay băng, Boss/PvP Slow bằng sắc lam/hạt lạnh nhẹ, Bỏng bằng tia lửa gọn. Overlay không đổi hitbox/collider. Corpse presentation phục vụ snapshot/camera/quest; corpse player không pickup hoặc cast, vẫn quan sát focus hợp lệ. Một SortingGroup và shared frame clock giữ nhiều actor/part không tách pose; layer order chi tiết từ Art.

Asset gate hiện hành nhập default/outfit I và Mộc/Kiếm vào **standalone disposable rig/art sandbox riêng**, ngoài VS-1 đã frozen. Fixture nhỏ kiểm mix-band/Tân Lữ/S1; thêm profile Kiếm khi cần kiểm reuse. Cung giữ analysis, probe trước production branch sau G-N. Chưa nhân ba families khi A01/A02/A17 hoặc grip/pivot/phase chưa kiểm. [Roadmap](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#production-release) sở hữu thứ tự; [legacy art gate](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#legacy-art-gate) giữ trace. Vòng này chỉ docs, chưa tạo/import art runtime.

<a id="ui-notes"></a>

# 9. Hợp đồng UI

Một router/modal stack cho NPC/inventory/character/quest/PvP. InputActionAsset giữ Gameplay/UI contexts; chat/modal chặn gameplay actions và clear buffered movement/execution. Jump/DropThrough resolver ưu tiên Drop trên one-way nếu cùng frame, không emit cả hai. Chỉ owner local gắn input/camera/HUD; world online không cần PlayerInputManager couch join. Semantic actions và quyền interaction theo [GDD](1_HUYEN_LO_GDD.md#ux-art); glyph đọc actual bindings, không hard-code candidate E/F/4–5/R/I hoặc alias cũ vào objective/tooltip.

**QuickConsumableAction:** server kiểm inventory/level/count/CD/state, chọn item theo GDD và consume một lần. MatchId còn kiểm quotaHP3/MP3 và cấm Hồi Sinh Phù; quota tăng sau DB commit, reject không tiêu item. Q6 QuickMP ưu tiên reserved Potion I ở đúng step, không bỏ món reserved để làm kẹt objective. Hiển thị reason khi reject.

Quest HUD dùng canonical state, không client tự chuyển Available/Ready. QuestDefinition giữ semantic enum/event/region/NPC/instance requirements; credit theo successful result, không keydown. Q3 author ít nhất ba Dummy placements cùng pool/lifecycle, không đổi farm respawn để chữa thời gian chờ tutorial. NPC focus ưu tiên context dịch vụ; combat execution riêng với interaction.

| View | Nội dung cần kiểm |
| --- | --- |
| HUD | HP/MP/EXP, selected skill và locked/learned/CD/MP state, Execution affordance, Food/Potion/quest/Boss timer; S2 chọn và farm thường xuyên, không trình bày S1 như default attack |
| Inventory/shop | Capacity/stack rules từ GDD, server transaction trước refresh |
| Upgrade/transfer | Preview cùng evaluator; source tiêu/target trước–sau/cost/Tinh Hoa/khóa–mở rõ; failure vẫn lưu cost |
| Skill panel | Ba active slot tích lũy/CD riêng, manual requirements, hai passive/class và mốc Lv5/13; passive hiện final stats, không skill points/ranks hoặc mới chọn là cast |
| Quest/chapter | State/nextLevel/resolved turnInNpcId server-owned; Ready vẫn chơi tiếp; Q6 mentor đúng class, Q10–Q12 Lâm Bá, Q12 Main Story Complete |
| WorldUI | HitResult/status/evidence theo recipient; combat focus marker + current/max HP/text/bar đồng bộ; owner chết vẫn thấy damage của người khác trên target còn hợp lệ; loot windows/quest cues/chat/Boss name riêng per-character |
| Death/PvP | Death choices khác PvPDefeated; stake/escrow pending/confirmed/phí/refund/quota, không xác nhận mutation khi chưa ACK |

Bag Sort/protection gear/quest arrows/history là P1, không điều kiện để P0 hoạt động. Tooltip làm tròn cho đọc nhưng evaluator giữ fractional enhancement. Không đưa RPC/saveAPI/microservice vào player flow.

<a id="dev-mode"></a>

## 9.1. Dev Mode để kiểm feature và phục hồi test

Dev Mode là tooling P0 cho người phát triển/tester trong development build, tách player UX/release. Không dùng dev shortcut làm lời giải cân bằng hoặc thay đường chơi production. Dev commands vẫn đi qua authority/evaluator/receipt khi thay state bền; không client tự sửa canonical snapshot. Dedicated cần quyền dev riêng trong cấu hình development và test account/storage riêng; không đưa credential/quyền/dev UI vào release client.

| Nhóm thao tác | Tooling cần có và giới hạn |
| --- | --- |
| Progression/class | SetLevel/EXP/attributes; Set/ClearClass với `ClassChosenLevel` hợp lệ; learn/unlearn skill; inspect derived stats/points/CD. Thay state invalidate pending action và derive lại evaluator, không cộng stat lần nữa hoặc đoán cấp chọn phái |
| Quest/inventory | SetQuestState/active step/prerequisite; give/remove item/Gold/manual; inspect/reset entitlement/counter/receipt test có chủ đích. Giữ stable IDs/bindings và phân biệt fixtures đã grant với reward chơi thật |
| World/combat | Teleport tới authored marker/SafeAnchor; spawn/reset group/life; force Linh chỉ cho probe; reset encounter/Boss/status; inspect target/focus/action/generation/clock/threat/contribution/loot deadlines. Không dùng force để thay production reservation hoặc autoroll |
| Test lifecycle | ResetFreshCharacter/test profile, reload/checkpoint/reconnect probes và log before–after. Reset chỉ dữ liệu test đã chọn, không wipe database/character thật; clear hoặc đổi generation để callbacks cũ không tác động life mới |

Mỗi dev override có label, command log và cách quay về production defaults. Preset Lv20/fullgear/Boss yếu hữu ích cho feature probes; **không chứng minh pacing hoặc fresh journey Q1–Q12**. Nghiệm thu cần fresh character **cho từng phái Kiếm và Cung**, không skip, EXP multiplier1/current defaults, chơi đủ các bước và optional Q9 branch. Sau reset không để item/quest entitlement, receipt, focus, action hoặc profile stats cũ rò vào run mới. Exact UI/command transport/authorization implementation còn PROPOSAL phải kiểm ở production base/G-D; ranh giới dev/release là contract.

<a id="roadmap"></a>
<a id="technical-gates"></a>

# 10. Gate kỹ thuật và nghiệm thu

[Roadmap](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#phase-gates) sở hữu thứ tự làm: harvest → feel/art/UI probes → production base → G-L mới → G-N sớm → G-D. Technical giữ cách kiểm; anchor `roadmap` bảo toàn link cũ. Lịch/workload/cut ladder cũ ở [trace](5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#legacy-roadmap), không là evidence đã pass.

**G-L — local production slice:** sau base review, revision mới chạy liên tục Q1–Q6/Tân Lữ→Kiếm trên ba map. Kiểm di chuyển chỉ bằng mũi tên; chọn 1/2/3 không cast, approach hoặc tốn MP/CD; Execute riêng; nhả phím giữ one-shot và giữ phím không repeat. Đổi selection lúc pending chỉ đổi UX; Execute mới thay intent chờ, action đang chạy giữ snapshot. Kiểm cancel thủ công/buffer/arrival validation. Nhịp S2 và MP từ GDD hiện hành được thử bằng fixture hẹp riêng nếu chưa mở trong route Q1–Q6; fixture không mở rộng story scope G-L hoặc ký pass fresh route. AUTO sticky, EXPLICIT không swap, Tab dùng local set ổn định; thử target trên không/sau lưng/khác tầng, no-target/no-cost và dead-owner focus HUD. Physical Execute/interaction candidates đi qua usability probe, chưa khóa.

G-L còn kiểm terrain trực giao, không climb, natural solid và one-way có support; movement ở tốc độ thường; khu chức năng NPC, Q3 tại Bách Luyện và Q6 mentor route. Glyph phải theo context. EdgeExit/SpecialGate/MapId/checkpoint ordering, supply/full bag/death/retry đi cùng revision. Cần video rig/pivot/socket và giờ art/editor/QA/rework thật. RAM adapter không chứng minh persistence; prototype cũ không pass gate mới.

**G-N — Dedicated spike sớm:** Dedicated headless + hai standalone Clients cùng content revision, slice đã đủ ổn. Profile/admission RAM dev và receipt fixtures có thể đo authority/physics/recipients, chưa là final login/save. Không đưa service credential giả vào client. Kiểm sender binding, replay/forged sequence/duplicate connection, map transition, shared kill/quest credit và hai claims tranh một item. Packet action/status cũ không gắn vào respawn life, UI không nhân effect/result. Owner chết vẫn nhận current/max HP của focus từ hit của người khác. Collections/AI/crowd/loot xử lý N người, không fixed pair. Thử delay/loss, ghi latency/correction; thêm 3–4 Clients để tìm assumptions, chưa claim capacity. Sửa boundary/headless physics trước mở rộng content.

**G-D — backend thật:** PostgreSQL + Spring Boot login/character/ticket + Dedicated + hai Clients. Kiểm ticket một lần, lease/duplicate connection, SafeAnchor/checkpoint/resume/map transition và một death với N recipients/claim idempotent. Q6 commit class/`ClassChosenLevel`/mentor binding chung; chọn Cung muộn rồi reload không giảm HP nền. Retry/outage/crash trước–sau commit/ACK theo §6; đo backend latency, sửa ticket/transaction/clock/checkpoint/headless physics trước integration done. MPPM phục vụ iterate; acceptance dùng standalone. G-N RAM pass không thay G-D.

**Setup đích:** Docker PostgreSQL với volume test riêng → Spring migrations/admin-created accounts/characters → Dedicated build lấy backend URL/service credential từ environment/world config → hai Client builds login/chọn character riêng → logs cả bốn tiến trình. Restart server kiểm checkpoint MapId/HP/MP/SafeAnchor, Boss reset/loot receipts và PvP orphan refund100%; tắt Spring/DB để kiểm pause/retry. Không commit password thật hoặc service credential vào client. Headless target là artifact riêng, cùng content revision/gameplay assembly; chỉ server chạy authority.

G-C/F/P/T mở rộng progression/gear/quest/Linh/Boss/co-op/recovery/PvP/chat/regression/package theo §12, gồm **full fresh Q1–Q12**, late class và optional Q9 song song Q10. G-L slice Q1–Q6 không thay toàn journey. Các gate TARGET vẫn CHƯA CHẠY; ghi revision/build/packages/machine/log/video/time/result. Mục tiêu quản lý hai tháng/bốn người ở Roadmap chỉ được ước lượng lại từ giờ integration/art/editor/QA thật; estimate 160–240h player-host/JSON cũ là LEGACY.

<a id="pending-ordering-probes"></a>

## Probe pending/clock — TECH-01, SAVE-01, A15

Commit/retry invariants đã có; barrier/serialization/time anchoring/definite-reject recovery còn **OPEN, chưa runtime evidence**. G-D ghi quyết định và test trước reliability done. Bảng là probe thuộc gate, không schema mới đã khóa.

| Chuỗi phải thử | Kết quả cần chứng minh / quyết định còn mở |
| --- | --- |
| Replay/correlation trùng, packet life cũ | Authority giữ action/result IDs; life mới không nhận result/status cũ, recipient không đổi chéo; ghi network/delay/loss/seed |
| Hai claims, một ACK trễ | Một inventory nhận item, loser AlreadyClaimed, retry receipt cũ; RAM G-N không chứng minh crash durability |
| LethalHP0 → terminal-pending → ACK sau respawn deadline | Chưa finalize không reward/respawn; chốt `deathUtc`, release cap/reservation/due ordering. Nếu neo t0HP0 thì giữ t0, không đặt lại từ ACK/corpse. Art t0 chỉ giả định A15 |
| Potion → hit/Food tick → consume/checkpoint ACK | Không overwrite HP mới, heal/consume hai lần, mất bình mà bỏ heal đã commit, hoặc miễn damage. Chốt mutation barrier/replay clock; không pause UI/Animator/gameplay clock để chữa |
| Timeout unknown / commit mất ACK / definite reject | Unknown retry cùng immutable payload/key; receipt trả committed result. Reject không reroll/regrant/retry vô hạn; chốt canonical reconcile/close/pending recovery theo lease và thông báo |
| Periodic đến sau transition/death; restart/clock jump | Sequence/generation chặn stale; chốt session↔UTC anchor. Loot giữ UTC gốc; normal/Boss restart semantics §6–7; UI countdown không authority |

Ghi state/IDs/revisions trước–sau/expected outcome; latency/correction kèm cảm giác chơi là số đo, chưa có ngưỡng performance/capacity approved. G-N correctness với fixtures; G-D PostgreSQL thật/lỗi trước–sau commit. TECH-01/SAVE-01/A15 ở Analysis giữ OPEN cho đến evidence, không pass vì có checklist.

<a id="risks"></a>

# 11. Rủi ro kỹ thuật

| Rủi ro | Mức | Evidence/gate và xử lý |
| --- | --- | --- |
| Sustain MP, S2 farm thường xuyên, INT/Potion | CRITICAL | Nhịp/MP/power và HP/MP gear mới thay mô hình cũ. Dùng [current balance probe](3_HUYEN_LO_DESIGN_ANALYSIS.md#current-balance-probe), đo rotation/Food/Potion/zero-INT; sustain sheets cũ là LEGACY |
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
| Quest supply/routing hoặc Dev Mode rò vào release | HIGH | Bảy NPC, Q6 mentor, Q10–Q12 Lâm Bá; active-step virtual credit/full bag/replay. Fixtures không thay fresh journey; quyền/storage dev tách release |
| Observer/network cost | MEDIUM | Đo dead observer đúng MapId/N-player trước optimize P1; chưa công bố capacity |
| Scope P1 tăng ngoài gate | HIGH | Proposal/research không là DoD; chọn sau P0 gate, không framework hóa prototype |

<a id="qa"></a>

# 12. Demo, QA và acceptance

P0 cần ít nhất hai concurrent Unity Clients trên một Dedicated Game Server, Spring/PostgreSQL phục vụ cả hai. Fixtures không giới hạn registry/spawn. QA với 3–4+ players ghi machine/build/packages/CPU/frame time/bytes/messages/latency trước capacity claim; không cần Party/Channel.

EditMode kiểm EXP/points/evaluator/`ClassChosenLevel`, reward/quest result construction, PvP quota và checkpoint ordering. PlayMode kiểm multi-actor drop, terrain/transition/camera/MapId/SafeAnchor, action timeline và pool reuse. Spring integration dùng PostgreSQL thật để kiểm ticket/lease/migrations/item/quest/death/claim/class/escrow, replay/outage/settlement/reconciler. Escrow hai người cùng thành công hoặc cùng rollback. Multiplayer iterate bằng MPPM; acceptance là standalone Dedicated + Clients. Evidence thủ công về feel/input/rig/UI ở tốc độ thường vẫn cần.

**Mọi TARGET Dedicated/backend dưới đây CHƯA CHẠY.** Prototype evidence chỉ có giá trị theo revision trong prototype README; fixture local không ký pass target. GDD sở hữu số balance/catalog/quest; Technical kiểm invariant và source mapping, luôn dùng current definitions khi retune.

| Fixture | State đã author | Dùng để kiểm |
| --- | --- | --- |
| demo_new | Lv1 Tân Lữ; kiếm chỉ grant tại Q3 | Movement/Q1–Q4; fresh run phải tiếp tục đủ Q1–Q12 trên character mới |
| demo_class | Lv5, Q5 Completed/Q6 Available, 20 unspent; chưa class/`ClassChosenLevel` | Chọn class/mentor, turn-in, reset/equip/manual/passive; thêm biến thể chọn muộn Lv6+ |
| demo_mid | Lv10, chọn class tại C=5, Family I +0, Q1–Q8 Completed; có S2 manual chưa learned | Học S2 giữ S1/CD; group/gear/Food/Linh; Band II giữ trong bag trước Lv11, không equip |
| demo_end_sword | Lv20 Kiếm C=5, đủ active manuals; Rare III Weapon+6, Armor/Pants Common III+0, ba ô còn lại II+0; test Gold12000 và ≥3 bình mỗi loại | Boss/PvP Client A, không giả định full+8 |
| demo_end_bow | Lv20 Cung C=5, cùng gear/learned setup, characterId riêng | Boss/PvP Client B; thêm fixture chọn Cung muộn với C được author rõ |

DebugExpMultiplier mặc định1; ×10 chỉ opt-in dev có label, không pacing acceptance. BossHP×0,40/respawn60s là **demo override**, Boss có sẵn; label/revert rõ, không dùng làm release defaults. Contribution threshold derive từ MaxHP override cùng evaluator (ví dụ fixture12.800→1.280), không chỉ đổi UI. Quay class selection bằng demo_class, không fixture đã class. Preset feature không thay fresh journey.

| Bộ nghiệm thu | Ca bắt buộc | Evidence |
| --- | --- | --- |
| Input/focus | Arrows-only; chọn1/2/3 chỉ selection, Execute riêng/one-shot/không repeat. Selection pending chỉ UX, Execute mới thay intent; manual cancel/arrival validation/buffer0,18s TUNABLE. AUTO ưu tiên executable rồi reachable và sticky; EXPLICIT không swap, Tab local order ổn định. Thử trên không/sau lưng/khác tầng, no-target/no-cost. Owner chết giữ marker/current-max HP theo hit của Client B nhưng bị chặn combat | Local/Dedicated input/action/life/focus logs, video tốc độ thường và glyph usability |
| Progression/stats | EXP carry/catch-up, cumulative53100/95points; reset một lần. Q6 chọn muộn giữ total points/Mộc Kiếm/reserved MP. Cung chọn tại C=5/6+/cấp cao dùng C thật, HP nền không giảm tại choose, gain chậm chỉ sau C, VIT như Kiếm. Reload/level/equip/reset derive một lần, clamp không heal; passive source/target range đúng | Canonical state trước–sau, evaluator và reload |
| Skill/status | Nhịp/MP/power S1/S2/S3 hiện hành từ GDD; probe farm S2 thường xuyên, CD từng SkillId/common lock, học S2 giữ S1 và CD cũ, không Normal Attack sau class. Spread ABC/ABA/AAA cùng resolve/stable indices, invalid mất hit/không reacquire; Line/Hàn đúng geometry. Roll mỗi unique target kể cả fail; Bỏng refresh source/expiry giữ nextTick; Freeze protectedUntil; Boss remaining wait không reset action; PvP Slow chỉ movement. Một/hai/bốn Cung không nhân debuff | Một clock, action/target/generation/status logs; không AnimationEvent |
| Mob/AI/crowd | Capability tái sử dụng, Hybrid OPEN; home/walk/edge không Jump/Drop. Hostile hit wake ngoài passive aggro, group alert không lan cả map, sticky threat. Một/hai/bốn melee thử occupied≠blocked, contact/staging/recovery, tường/mép/N-player/AoE. Cung kite hợp lệ; perch dùng Return, clear ledger/status/action và full HP tại home, phase parameters OPEN; Ong vẫn melee-accessible | Authored region/path/phase/threat/action logs và video |
| World/terrain/timers | Playable surfaces trực giao, không slope/climb; natural solid mass, shallow water và one-way nhân tạo có support. Drop per actor, press mới mỗi tầng và đúng priority. Hai/ba/bốn players farm, local groups dày nhưng độc lập, safe exit strip, không chain whole-map. Seed28/66 LEGACY, current totals OPEN; root occupancy/deadlines/map rỗng/re-entry/respawn idempotent đúng MapId | Layout audit, clock/MapId/run-back/contention logs |
| Boss | Boot/restart đúng một Alive Boss. Stats/nhịp/respawn từ GDD, demo override tách; Q12 không spawn. Một action, ba vùng đá không double-hit; Cuồng Mạch chỉ future cadence, Slow phần chờ còn lại. Threshold10% derive MaxHP, corpse eligibility, một pile, không direct EXP/Vàng; loot windows12–30–90s | Lifecycle/clock/credit/claim logs với hai/bốn players |
| Reward/loot | Phần contribution mỗi người floor, không chia lại; snapshot trước reward/level-up. Level suppression không gây quest softlock. TopDamage whole-life không đủ level thì không regular set/fallback. Shared windows8–20–60s; claim race/full bag/late join/level sau kill, một item chỉ một claim và crash receipt | N-recipient payload/revision/transaction/deadline logs |
| Catalog/equip/upgrade | 21 stable templateIDs/names/equip gates/pools/source theo GDD. HP ở Armor/Pants/Boots, MP ở Weapon/Ring/Necklace; rarity/enhance/flat/Tinh Hoa đúng primary lists/thứ tự. Fixed ACC/EVA/Crit/speed không rarity multiply; sáu class weapons. Transfer cùng/next band, cùng slot/class/equip level; no-gain reject, source consumed/target ID giữ; receipt replay/race source, failure cost/cap | Definitions/preview/runtime/reload tại+0/+4/+8, ownership và DB failure |
| Linh/Q8 | Lv8+/5%/cap1/25s TUNABLE, fixed identity/level/cache/life; due slots đồng thời được serialize. Q8`TA4.slot1` chờ live variant, không demote, share waiting set, promote idle generation và giữ rewards. Terminal-pending không respawn; A15 death anchor OPEN; restart rebuild requests | Seed/slot/cap/reservation/generation logs và throughput |
| Quest Q1–Q12 | Fresh no-skip toàn route, late class, solo/online/full bag/reconnect. Q3 Bách Luyện; Q6 Lâm Bá→mentor với class/C/grant/active group atomic rồi turn-in sau objective tại cùng mentor; Q10–Q12 Lâm Bá, Yên Thảo Tẩy Mạch, bảy NPC. Giữ Q4 DS2; Q5 DS3–DS6; Q8 TA4+TA6/`TA4.slot1`; Q10 XN1–XN3/evidence#2/#4/#6; Q11 XN4/5/6; Q12 HT4+HT5. Virtual credit đúng active QuestId/group/generation, không pre-farm/RNG sai step/overcount. Staged grant/manual/bindings/replay/full bag; optional Q9 song song Q10, Q11 activate ngoài gate, Q12 death receipt riêng turn-in receipt và game tiếp tục | Per-Quest snapshots/counters/ordinals/grant/turn-in receipts, full-journey video và negative mentor tests |
| PvP/chat | Mười stakes, accept exact, hai participants, escrow cả hai hoặc không ai/pre-Arena checkpoint. Countdown/ACTIVE ACK;120s DRAW không so HP;HP0 WIN/disconnect ACTIVE FORFEIT/pre-ACTIVE cancel/hai disconnect cùng tick SYSTEM_ABORT. Food real clock, quotaHP3/MP3/item commit/CD và Hồi Sinh Phù reject; PvP scale/Slow/chat80charMapId/rate từ GDD | W1000:WIN1800/fee200,DRAW900 mỗi người,abort1000 mỗi người;Journey200 một lần,Q9 không FORFEIT/abort; match/receipt hai Clients |
| Recovery/backend/network | N-player collections/sender/ticket/replay/lease một writer. Resume15s bằng token một lần vẫn chịu hit/giữ exact runtime; session loss checkpoint/SafeAnchor/safe coordinate hợp lệ/Arena excluded/HP0 dead/focus clear. Periodic30s/critical ACK/stale seq, PvE Potion item+heal checkpoint/PvP item-only, class/C atomic. HELD/BIND/ACTIVE/SETTLED crash/mất ACK chỉ refund khi chưa settle; death/claim/enhance receipt/durable loot UTC/Boss RAM reset; schema fail chặn startup | PostgreSQL transaction/failure tests, hai/ba/bốn Client delay/loss/crash logs và pending probe outcomes |
| Art/UI/Dev Mode | Art import theo current26frames/rig/pivot/phase/socket/flip/foreground/overlay, không Climb. Glyph semantic select/Execute/NPC focus/dead-target HP. Dev permission/test storage/labels/reset fresh/clear callbacks life cũ; release không dev, preset không thay fresh journey | Import audit, video tốc độ thường, build config và reset logs |

Đóng gói hướng dẫn PostgreSQL → Spring → Dedicated → hai Clients, seed/reset test có chủ đích và video fallback. Build/packages/content revision, kết quả từng ca và vấn đề OPEN phải rõ. Video fallback không thay acceptance; vòng migration này không chạy prototype/build hoặc tạo planning/manifest docs mới.
