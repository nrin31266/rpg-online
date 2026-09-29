# Huyền Lộ — Hợp đồng kỹ thuật

**Đối chiếu:** [GDD hiện hành](1_HUYEN_LO_GDD.md) · **Ngày:** 2026-09-29 · **Trạng thái:** specification trước triển khai.

[GDD](1_HUYEN_LO_GDD.md) quyết định gameplay; [Analysis](3_HUYEN_LO_DESIGN_ANALYSIS.md#open-decisions) giữ các gap phải chốt. Các tên component/API dưới đây là hướng triển khai, cần spike với phiên bản Unity/package thực dùng; chưa có Unity build hay runtime test trong vòng dọn tài liệu này.

<a id="runtime"></a>

# 1. Runtime Architecture

| Layer | Trách nhiệm | Biên giới |
| --- | --- | --- |
| Definition | ScriptableObject: class, skill, mob, item, quest, map | ID ổn định; không giữ state mutable trong asset |
| Host simulation | Movement validation, combat, AI, loot, quest, economy, timers | Authority duy nhất cho kết quả gameplay |
| Session state | Player state, Mob/Boss instances, contribution/threat | Không coi client cache là save |
| Persistence | Character/profile repository và world timer | Schema/version/migration, transaction và recovery |
| Presentation | Sprite parts, VFX, HUD, chat, sound | Render feedback không thay server clock |

**Một nguồn definition:** SkillDefinition/MobDefinition/ItemDefinition/QuestDefinition/MapDefinition là ScriptableObject static authoritative; không song song cùng stat trong `.asset` và `.json`. JSON chỉ runtime save/snapshot persisted. DemoConfig dùng một ScriptableObject riêng chọn ở boot, không override cùng value từ JSON thứ hai; demo profiles là runtime JSON fixtures, không chứa bản copy definitions.

Baseline fixed physics 50Hz, network 20Hz, render target 60FPS; tune qua profiler, không hứa đạt trước benchmark. NGO + Unity Transport là hướng baseline; giữ adapter để không để gameplay phụ thuộc tên RPC cụ thể. Offline practice dùng cùng Host simulation, không tạo engine damage thứ hai. Không dựng dedicated production server P0.

**Multiplayer state:** World support N players. PlayerRegistry, connection→profile/character binding, MapId membership, Threat/Contribution, quest assist, chat rate limits và loot eligible recipients dùng dictionaries/sets theo playerId; spawn từ mỗi connection hợp lệ, không hai fixed slots hoặc MaxPlayers=2. Network clientId là handle của connection, không save characterId; request dùng sender thực do network cấp để resolve binding, không trust playerId từ payload. Boss resolver duyệt toàn bộ alive candidates; reward snapshot duyệt toàn bộ eligible players. Config capacity ở session admission tách khỏi gameplay, chỉ công bố sau benchmark 3–4+.

**Unity/tooling baseline — adopt theo mục đích, chưa cài package trong vòng docs:** Khi tạo project chọn supported Unity LTS và package tương thích, pin Editor patch + manifest/lock sau spike; không auto-upgrade hoặc lấy bản mới nhất. Unity6.3 LTS là mốc documentation đã kiểm, không ép migration project đang chạy ổn.

| Kỹ thuật | Áp dụng cho Huyền Lộ / biên giới |
| --- | --- |
| [Input System — actions](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.20/manual/index.html) | Adopt một InputActionAsset với Gameplay/UI contexts; owner local duy nhất đọc input, chuyển intent sang Host. Giữ bindings GDD, không thêm hotbar/rebind UI. |
| [NGO](https://docs.unity3d.com/6000.3/Documentation/Manual/com.unity.netcode.gameobjects.html) + [Unity Transport](https://docs.unity3d.com/6000.3/Documentation/Manual/com.unity.transport.html) | Giữ Host-authoritative baseline; không viết transport/RPC layer thứ hai. Chọn package version đã xác minh release/Editor compatibility ở spike; lưu ý release channel bên dưới. |
| [Multiplayer Play Mode](https://docs.unity3d.com/Packages/com.unity.multiplayer.playmode@2.0/manual/index.html) | Adopt dev workflow Host+Clients trên cùng máy; tối đa4 Editor Players là giới hạn tool, không world. Acceptance cuối vẫn có standalone builds, test3–4+ đo capacity riêng. |
| [Multiplayer Tools / Network Simulator](https://docs.unity3d.com/Packages/com.unity.multiplayer.tools@2.2/manual/network-simulator.html) | Adopt profiling và giả lập delay/jitter/loss/disconnect khi dev; ghi preset và logs, tránh double simulation ở cả tool và transport. Không ship simulator bật trong release. |
| [Unity Test Framework](https://docs.unity3d.com/Packages/com.unity.test-framework@1.6/manual/index.html) | Adopt EditMode tests cho domain/repository, PlayMode cho physics/lifecycle; dùng batch runner của package tương thích, không xây test framework riêng. |
| [UnityEngine.Pool.ObjectPool](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Pool.ObjectPool_1.html) | Adopt cho local presentation VFX/damage text; callback reset/release, collection check ở dev. NetworkObject pooling chưa cần P0; không dùng pool thay network spawn/despawn. |
| [Cinemachine3 Position Composer](https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachinePositionComposer.html) / [Confiner2D](https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineConfiner2D.html) / [Impulse](https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineImpulse.html) | Adopt local orthographic follow/bounds/shake, giảm custom camera code; không camera network authority hoặc cutscene system mới. |

Input/PlayMode/Tools/Cinemachine có released entries trong Unity6.3 catalog; [Test Framework là core package](https://docs.unity3d.com/6000.3/Documentation/Manual/com.unity.test-framework.html). Catalog NGO đang đánh dấu pre-release dù có upstream release tags: giữ lựa chọn framework, xác minh exact package/channel trước dependency freeze ở gate hiện có, không tự gọi latest là stable. Không chuyển topology để giải quyết metadata. DOTS/Entities/ECS, Addressables/streaming, cloud/dedicated, prediction/rollback phức tạp và service/event framework lớn chưa có lợi ích đo được cho slice; giữ kiến trúc hiện hành.

<a id="maps"></a>

# 2. Unity Scene / Map Architecture

Boot → ProfileSelect → CharacterSelect → WorldOnline. Một world scene có **8 logical map roots**: Village, Academy, Arena và năm farm maps. Root origins derive từ actual world bounds + separation margin; ~200u chỉ điểm bắt đầu của spike, không fixed spacing/width. Kiểm bounds/colliders không overlap sau khi dimensions đổi, không để map dài hơn offset nối physics hai roots. MapDefinition giữ MapId, spawn point, portal destination, bounds, SpawnGroups và environment family.

| Hợp đồng | P0 | P1 |
| --- | --- | --- |
| Map identity | Host giữ MapId; kiểm combat/chat/loot/portal | Không thay bằng scene visibility |
| Presentation | Camera và render filter chỉ root hiện hành | Streaming nếu profiler cho thấy cần |
| Network observers | Có thể còn replicate khác map; client không render | NetworkHide/NetworkShow sau correctness |
| Portal | Host kiểm nguồn, range, state, destination gate; đổi MapId+position | Loading polish |
| Empty root | AI pause; timer vẫn thuộc global manager | Interest optimization |

Mỗi MapRoot gồm BackgroundTilemap/GroundTilemap/PlatformTilemap/ForegroundTilemap, SpawnPoints, Portals, NPC anchors. Ground baseline: TilemapCollider2D + CompositeCollider2D + Static Rigidbody2D; [Unity6 Composite Operation = Merge](https://docs.unity3d.com/6000.3/Documentation/Manual/tilemaps/work-with-tilemaps/tilemap-collider-2d-reference.html), không hướng dẫn checkbox UsedByComposite cũ. PlatformTilemap tách ground, [PlatformEffector2D Use One Way](https://docs.unity3d.com/6000.3/Documentation/Manual/2d-physics/effectors/platform-effector-2d-reference.html); nếu composite thì UsedByEffector do CompositeCollider điều khiển. S+Space drop-through chỉ tạm bỏ collision cặp actor–platform đang đứng, restore sau khi actor xuống dưới hoặc cancel/death/portal; không toggle effector/layer collision toàn world làm player khác rơi. PHY-01 spike kiểm nhiều actors và nhiều tầng; không cần custom collision engine. Background/Foreground không gameplay collider. Tile dimensions/world cell derive từ asset contract PPU32, tile size không suy ra từ kích thước rig. Không HazardTilemap P0 (Hazard P2). MapDefinition vẫn owns bounds/spawns/portals/environment/unlock data; arena sub-region/access rule phải đợi BOSS-03, không thêm scene/root tùy tiện. Local Cinemachine target chỉ character owner (dead vẫn corpse); portal đổi confiner bounds theo root và snap/cancel damping qua offset giữa roots, invalidation cache khi đổi shape/lens, không follow player từ xa.

**Map authoring gate:** layout/pockets/safe strips/spacing/traversal targets theo [GDD §5](1_HUYEN_LO_GDD.md#gdd-5). Author one farm room trước 25 groups; centers/terrain phải isolate group aggro. Measure traversal và Village run-back riêng; scale SpriteRenderer Linh Biến không scale hurtbox/aggro/leash tự động. Collider/hitbox do data/PHY-01 quyết định, không variant transform kéo physics bounds vô tình.

MapDefinition đọc requiredLevel/unlockFlag từ [GDD — map gates](1_HUYEN_LO_GDD.md#gdd-5); Host kiểm cờ **Completed**, không objective counter hay ReadyToTurnIn. Mọi player có MapId riêng, có thể ở khác map. Root activity tính từ collection player trên Host, không từ map local của một Client; presentation filter không disable simulation/NetworkObject của player khác. Portal từ chối không làm mất player hoặc tạo duplicate; chuyển map cập nhật target/threat/interaction và gửi snapshot root mới. Chỉ render filter không đủ bảo vệ gameplay.

<a id="combat-data"></a>

# 3. Combat Data Architecture

| Definition/state | Trường cần có | Validation |
| --- | --- | --- |
| SkillDefinition | ID, level/class, shape, range, power list, maxTargets, MP, CD, hit timings, status | Power theo GDD; không thêm SkillRank |
| CombatRequest | character binding, actionID, sequence, facing/aim intent | Host kiểm alive, map, cooldown, MP, range và target |
| HitResult | actionID, targetID, hitIndex, evade/crit, damage, remainingHP | Client chỉ hiển thị kết quả Host |
| MobDefinition | archetypeId (6), base formula, AI/movement, attackTimeline/front hitbox, projectile, lootProfileRef, linhBienEligible; hp/atk/def/exp multipliers | Default multipliers1.0; level thuộc SpawnSlot, một definition mỗi base archetype |
| LinhBienModifier | stat/reward modifiers, visual preset, variant tag | Config apply/derive trên base runtime; không duplicate sáu definitions |
| SpawnGroup / SpawnSlot | mapId/groupId/slotId, archetype, level, spawnPosition, respawnAt, spawnGeneration, variantState, forcedVariantEntitlements, variantOrigin | 25 groups TEST/TUNABLE theo GDD; normal slots, không EliteAnchor/EliteSlot |
| ItemDefinition/Instance | templateID, instanceID, rarity, enhancement, count; optional buyPrice, explicit sellValue, materialId/loot refs/binding | Không fake buyPrice cho drop-only; instanceID duy nhất; tutorial-bound không merge bản vendor |
| QuestDefinition | prerequisiteQuestIds, requiredLevel, objectives, turnInNpcId, rewards, unlockFlags | Authoring theo [GDD §6](1_HUYEN_LO_GDD.md#gdd-6), không copy quest table |
| QuestProgress per QuestId | state, currentStep/activeObjectiveGroup, objectiveCounters, claimedFlag, stagedGrantFlags | Host-owned runtime dictionary; save qua DTO §6, idempotent claim/save |

**Quest runtime:** `Dictionary<QuestId, QuestProgress>` là source of truth theo từng quest ID. Q9 Available/InProgress cùng Q10 Available/InProgress là hợp lệ; `currentQuestId` toàn cục không được làm nguồn duy nhất. UI có thể pin một chính tuyến, còn nhánh optional vẫn persist. Definition giữ ordered steps/objective groups, target IDs, counts, event filters và supply/evidence policy; chỉ group active được cập nhật.

**QuestState model:** Locked → Available → InProgress → ReadyToTurnIn → Completed. EligibilityService kiểm đủ prerequisite quest IDs **và** requiredLevel; branch optional chỉ theo prerequisite graph GDD, không implicit Qn−1. Thiếu level trả Locked+reason/nextLevel cho NPC/HUD, không marker Available.

QuestService consume event từ hành động Host thành công, có eventID/characterId/MapId/targetId cùng payload cần thiết: `NpcTalked`, `RegionVisited`, `ItemPickedUp`, `ItemEquipped`, `ItemSold`, `ConsumableUsed`, `MobKilled`, `QuestItemCollected`, `AttributeAllocated`, `SkillUsed`, `EnhancementSucceeded`, `PvPCompleted`, `BossEligibilityAchieved`. Khớp QuestId/active objective/group/target, chặn replay rồi persist counter/step trước publish. Pickup/equip/sell/use/upgrade chỉ emit sau mutation thành công; consumable reject khi đầy không emit Used. Mutation character và progress/dedup cùng critical save transaction; external Mob/Boss death giữ eventID+eligible snapshot để retry không mất/nhân credit nếu save gián đoạn. Click UI hoặc client tự báo “đã làm” không tăng tiến độ. Không yêu cầu framework event bus lớn.

QuestService validate progress từ event Host hợp lệ; đủ objectives chỉ chuyển ReadyToTurnIn, không teleport hoặc gọi reward. TurnInRequest kiểm connection/character binding, state, turnInNpcId, sameMap, interaction range và claimedFlag. Một transaction commit reward+Completed+story/unlockFlags+summary eligibility, persist rồi publish; replay không cấp lần hai. Preflight reward insertion tính compatible stacks và X slots còn cần theo GDD. Thiếu capacity trả reason+X, không mutate reward/evidence/flags, giữ ReadyToTurnIn; retry idempotent. Completion transaction gồm cleanup evidence. Không phát gold/EXP một phần rồi fail physical item.

Tutorial staged grants/drop (supply do GDD chỉ rõ) là giao dịch riêng có one-time flag, đủ để làm objectives trước turn-in; không nhầm với completion reward. Save sau staged class selection/reset và mỗi mutation critical; abort/reconnect không cấp lại 20 điểm hay weapon. Grant ledger keyed characterId+QuestId+grantId, trạng thái pending/spawned/claimed/consumed; staged Q5 gear/trade sample, Q6 Potion và Q7 gear/reserved resources không cấp lại lúc turn-in. Reserved tutorial resources không dùng cho giao dịch khác trước đúng bước. Q5 áo không sell/drop trước equip; sample Trade Material có tutorial entitlement flag, tách compatible stack, chỉ sell khi Sell objective active rồi gỡ flag; không thêm player gear-lock UI P0. Q7 nhẫn giữ tới enhancement/equip. Tutorial ground object mất/expire/reconnect thì restore cùng entitlement/instanceID pending, không new reward; chỉ hết entitlement sau receipt/consume đã persist. Bag full để pending và retry; replay kill/accept không nhân supply. Tutorial sample không tính regular loot roll/economy rate. Quest objective death dummy reset không loot/EXP event farm. Q12 boss eligibility event và quest turn-in là hai transactions có IDs khác: Boss loot một lần/event, story completion một lần/character; không duplicate Boss reward khi báo NPC.

**Evidence contract — QUEST-02:** Definition chọn virtual counter (ưu tiên) hay physical quest-bound item; không ngầm dùng Trade Material. Virtual evidence chỉ đếm Host event khi đúng active step, icon/name tracker lấy definition. Với physical: separate itemID, character-bound, cannot sell/drop/trade; count/spawn chỉ active objective; persist entitlement+instance, bag-full giữ pending, không xóa nguồn trước receipt, không count pre-farmed goods; cleanup chỉ sau successful turn-in. Q11 altar xác thực đủ evidence và commit activation một lần; bằng chứng không mất khi failed turn-in/reconnect. Exact representation/collection rates cần QUEST-02 trước authoring.

Inventory insert: fill compatible stacks trước, overflow tạo stack tới capacity; plan insertion rồi commit toàn amount hoặc giữ ground/pending intact theo GDD. Compatibility gồm template/binding và instance flags, không merge quest evidence với trade goods. Bag Sort P1 không là điều kiện của auto-stack P0.

ItemDefinition giữ rarityPrimaryStatIds, enhanceStatRules và fixedSlotBonusDefinition. Compute stat theo [GDD §7](1_HUYEN_LO_GDD.md#gdd-7), gồm flat gain hiện hành; Tinh Hoa derive theo enhancement, không add-mutate mỗi equip/load. Bonus amounts/rarity interaction không freeze trước GEAR-01.

**LootService contract:** implement các channels/quantity/map-tier/slot/potion/vendor baseline GDD, không tự chọn independent rarity/category. Một integer roll trong 1000 buckets cho gear: Normal40/10/1/949; Linh Biến300/60/5/635; Boss400/80/520, tối đa một gear/effective reward profile. Roll rarity trước, slot uniform 6, weapon 50/50, create item +0; dùng discrete quantities uniform inclusive. Gold ledger auto-credit, cosmetic coin VFX local không NetworkObject/pickup. Death eventID + eligibility/rolled payload snapshot persist/retry như Boss dispatch; không reroll do pickup/reconnect, mỗi character receipt cùng transaction. Tutorial/evidence channels riêng, không lấy vật liệu vendor làm evidence. **COOP-01 gate trước recipient/EXP/Gold distribution code**, không assume one full roll/player hoặc killer-only. LOOT-01 chỉ tune reward/prices, không chặn semantics spike.

**Linh Biến arbitration:** một Host-owned record/MapId (`activeSlotId`, `generation`, `reservation`, pending characterId set), mutate trên một simulation thread. SpawnManager serialize due slots theo slotId: check cap + reserve + roll + spawn commit cùng action, chỉ increment generation khi new spawn. Initial population dùng cùng arbitration; Return/AI wake không roll. LinhBienModifier derive từ base mỗi spawn/restore, không multiply nhiều lần; death clear active record và schedule chính slot đó 12s. Không dedicated Elite timer/anchor hoặc variant subclass graph.

**Q8 reservation:** pending entitlement keyed characterId+QuestId+step, không connectionId; tạo từ active Host quest state, restore từ saved progress. Áp bảng trạng thái Q8 tại GDD §5: variant khác idle ở spawn/full HP/no pending action được demote ngay; nếu đang combat/chưa về spawn thì chờ death hoặc Return hoàn tất. Không despawn engaged mob hoặc bỏ qua respawn deadline. `variantOrigin` Dynamic/ForcedQ8 giữ reward source; bind dynamic Sói vào Q8 không downgrade rewards của recipients khác. Nếu không reachable idle/full-HP wolf thì chọn next eligible respawn; random promotion bị suppress trong reservation. Một runtime generation dùng chung waiting set, không entity per quest/player; attach late join vào current eligible encounter. Disconnect/portal gỡ session binding, giữ pending quest; map rỗng không spawn thêm để phục vụ absent player. Random rule chỉ bị reserve khi có pending player trong map; current variant vẫn giữ cap qua AI pause. Death snapshot mọi eligible quest recipients **trước** increment objective step, receipt/dedup như tutorial ledger; death không credit thì entitlement pending và retry đúng slot.

Forced bonus receipt tách quest-progress receipt: unique key theo entitlement, pending được reserve như committed trong Host state để hai death events khi IO chưa xong không chọn bonus profile hai lần; serialize character transaction/retry cùng key. Một eligible pending character chỉ nhận Linh Biến bonus profile lần đầu, kể cả quest credit retry sau death/reconnect; recipient không có entitlement hoặc đã bonus thì Normal profile theo GDD. Persist selected effective profile (EXP/Gold/loot/Journey), reward values và receipt trước publish; không cộng Normal+Linh Biến profiles, không thay COOP distribution. Save world slot/generation/variant/roll outcome và character pending/bonus receipt, rebuild registry idempotently để restart không reroll hoặc nhân force request; exact non-Boss world restore acceptance vẫn SAVE-01. Không bảo đảm zero wait nếu mob đang fight; đảm bảo không phụ thuộc RNG, HUD target hiện hành rõ.

Boss config stat/count/scheduler theo BOSS-02, availability theo BOSS-03; không lấy S07 assumptions làm asset values đã khóa.

**ActionTimeline:** SkillDefinition/action config giữ castStart, windup, hitMoments[], projectileSpawnMoments[], recovery và totalLockDuration (giá trị exact tại ART-01/combat playtest). Host validate, charge MP/CD một lần, tạo actionID/startClock rồi schedule authoritative hits/spawns. Liên Kích snapshot target khi action bắt đầu, resolve ba hitIndex vào ba hitMoments, bỏ target invalid, không reacquire. Bow spawn projectile đúng moment, Host resolve impact khi collision hợp lệ; Boss telegraph báo trước impact được Host schedule, không cho animation client quyết định damage. Client đồng bộ visual theo action clock; render frame bị bỏ không làm mất hoặc thêm hit.

Pipeline: validate request → chọn target hợp lệ cùng MapId → roll né/crit → damage theo GDD → cập nhật HP/status → emit result → death/reward transaction. ActionID+targetID+hitIndex ngăn damage lặp nhưng vẫn cho ba hit Liên Kích. Snapshot không reacquire; piercing theo thứ tự đường đạn; explosion loại primary. Cooldown/MP charge một lần cho action hợp lệ. Attack timing/animation không được biến mỗi visual frame thành hit mới. AnimationEvent chỉ SFX/slash trail/muzzle flash/camera shake/cosmetic VFX; không damage/crit/MP/CD/kill/quest progress.

**Mob action timeline:** target resolver scan N same-MapId valid players; candidate nearest hợp lệ, tie bằng playerId, không Player1/2. Melee windup locks facing/attack origin intent, không locks guaranteed damage: tại HitMoment Host revalidate same map/alive/range/front vertical hurtbox cho target đó, player cross-behind/out-of-band thì miss. Single-target normal attack không cleave thêm players. Ranged aim snapshot tại windup, projectile spawn theo Host clock, collision query hurtbox/terrain+MapId; không homing/reaim giữa projectile flight. Return/death/Freeze/map reset cancel unresolved action bằng instanceID+generation, không delayed hit sau despawn; Freeze hủy pending windup/hit/spawn, không thu hồi projectile đã spawn hoặc hoàn cooldown. Không mob Crit P0 (CritChance0 baseline GDD), Damage dùng GDD formula với mobATK/power.

**Monster separation candidate:** disable body collisions Player–Monster và Monster–Monster; hurtbox triggers/query vẫn hoạt động. Host steering trên reachable cùng-platform neighbors trong radius nhỏ: aggregate repulsion vector, clamp tốc độ/offset, damp/dead-zone, project lên đường đi hợp lệ trong leash; không Rigidbody.AddForce. Flying neighbors cùng movement band; không repel qua tường/khác tầng. During windup/hit/Return không steering đổi facing/hit origin; omit correction nếu blocked, không jitter teleport. Spatial query nhỏ theo MapId, stable neighbor order/instance IDs, replicated positions thay Client tự steer. PHY-01 kiểm crowd/near-wall/portal/leash và 2/3/4 players. Ong attack approach/hover band phải reachable bằng melee arc/jump Kiếm; clamp theo accessible platform, không dùng roam box height làm altitude cố định.

Physics layer mask tách body collision, hurtbox và ground/platform; hit query/snapshot/pierce/explosion implement theo [GDD §4](1_HUYEN_LO_GDD.md#gdd-4). MobBrain reachable/blocked và projectile config theo [GDD §5](1_HUYEN_LO_GDD.md#gdd-5), một MobProjectile prefab đọc definition; ART-01/PHY-01 là gate test feel/vertical. Không duplicate luật roster/AI trong Technical.

<a id="network-authority"></a>

# 4. Network Authority

| Client gửi intent | Host quyết định | Client nhận |
| --- | --- | --- |
| Move/jump/drop, facing | Collision/map bounds và state hợp lệ | Position/state; interpolation/correction |
| Normal/skill, aim | Target/range/MP/CD/HP/status | Combat results và VFX trigger |
| Equip/sell/upgrade/pickup | Ownership, bag, level, cost, RNG | Inventory/stat snapshot |
| Quest/class/attributes | Prerequisite, điểm còn lại, one-time reward | Progression/summary |
| Portal/chat/PvP invite | Map/range/rate/match state | Transition/bubble/banner/match result |

P0 localhost/LAN/Direct IP cùng mạng; P1 Relay/VPN/tunnel. Không claim Internet-ready hay anti-cheat production. Validate request ở Host kể cả player chạy trên Host; client owner không được trực tiếp ghi HP/gold/profile. Co-op P0 phải đợi COOP-01 cho ba policy độc lập: quest kill assist, EXP, normal loot. Policy và eligibility candidate xem [Analysis S14](3_HUYEN_LO_DESIGN_ANALYSIS.md#loot-consumable-analysis); không nhầm personal Boss loot đã khóa với normal loot chưa chốt.

PvP match state giữ MatchId + đúng hai participantIds vì mode1v1; damage/result/invite kiểm membership+match phase bên cạnh MapId, người ngoài không hit hoặc nhận result của match. World registry vẫn N players; concurrent arena matches/timeout/disconnect policy còn PVP-01, không tự thêm matchmaking.

ChatService validate count/rate theo [GDD §9](1_HUYEN_LO_GDD.md#gdd-9), route MapId; global banner tách khỏi map chat, tên Boss lấy Q12 từng recipient. PvPService đọc restrictions/match rules từ GDD; edge cases PVP-01 là gate acceptance.

<a id="profile-authority"></a>

# 5. Character/Profile Authority

Host repository load bằng profileId/characterId. Client không upload level, gear, gold, EXP hay attributes như trusted state. Connection→profile→character binding là TECH-01: spike phải xác định ID tồn tại, quyền chọn, reconnect và duplicate-selection policy; không coi biết ID là authentication Internet.

Binding/player spawn dùng collection theo connectionId/playerId, cùng một prefab/config cho mọi approved connection; Host player cũng đi qua validation tương đương. ClientId không là index vào hai slot/save fixtures; reconnect cập nhật binding, invalidate stale callbacks của connection cũ (TECH-01). Một active writer cho mỗi character; không cho hai connections cộng điểm/claim loot trên cùng save đồng thời. Runtime state và profile mutation cùng Host; kết quả transactions xác nhận sau khi cập nhật authoritative state. Disconnect gỡ player khỏi alive targets, chat và arena; xử lý contribution/loot theo connected-at-death rule GDD, không giữ eligibility bằng stale connection.

Invariant điểm: spent+unspent=5×(L−1); Lv1=0; Reset nhập môn khi đạt Lv5 tạo20 unspent một lần; Q6 nhận muộn không set pool về20, level-up sauLv5 vẫn cộng5 nên total=5×(L−1). Không nhận thêm20 mỗi load/class selection. Normal Tân Lữ còn dùng trước chọn class kể cảLv5+. Demo fixtures tại §12 không phải nguồn ghi đè profile thật.

<a id="persistence"></a>

# 6. Persistence

| Save scope | Dữ liệu | Không trộn |
| --- | --- | --- |
| Character | schemaVersion/IDs/name, level/currentEXP/class, 4 attributes/unspent, gold/Journey, bag/storage/equipment, quests/reward flags/skills (gồm Q8 pending/forced-bonus receipt), map/position, HP/MP/status | Q12 không là world completion |
| World/session | nextBossSpawnUtc, pending death/reward dispatch (eventID, recipients, rolled payloads/deadlines), Linh Biến slot/generation/roll/reservation và timer cần phục hồi | Không lưu player HP ở world record |
| Runtime transient | Active connections, threat/contribution, hit dedup | Không restore stale target references |

**Save DTO / serializer:** Runtime dictionaries (players/quests/flags) không serialize trực tiếp bằng [JsonUtility](https://docs.unity3d.com/6000.3/Documentation/Manual/json-serialization.html), vì Unity chỉ serialize fields và không hỗ trợ Dictionary. P0 dùng plain `[Serializable]` DTO với `List<QuestSaveEntry>` chứa questId+progress và list entries cho map khác nếu cần; rebuild dictionary sau validate unique IDs. Save DTO không giữ ScriptableObject/Unity object references, chỉ definition IDs; schemaVersion/required fields/counts kiểm trước apply để missing fields không bị hiểu thành nhân vật mới hợp lệ. Round-trip phải giữ Q9 và Q10 active đồng thời, counters/claimed/staged flags. Không cần thêm serializer dependency khi DTO đủ.

Item instance IDs phải unique trong bag/storage/equipment; equipped item không đồng thời nằm bag. Không dùng ví dụ JSON điểm cộng sai 95; validate EXP range, IDs, inventory count, enchant 0..5, class restrictions và điểm invariant khi load. Restore dead/Food/HP/MP/restart position nằm SAVE-01/CONS-01; chưa tự heal đầy hay offline regen.

**Safe save contract:** capture immutable plain DTO ở Host simulation boundary → serialize snapshot → temp file cùng filesystem → flush → atomic replacement của bản chính → giữ backup hợp lệ → recovery validate schema/invariants. Có thể spike FileStream.Flush(true), File.Replace hoặc rename phù hợp platform; API cụ thể phụ thuộc Unity/.NET/filesystem, không bắt buộc trong GDD. Thử crash ở từng bước và first-save khi chưa có destination. Write/replace lỗi: giữ last-valid file, giữ transaction pending để retry, không báo durable success trước commit. Nếu làm background IO chỉ xử lý immutable DTO/bytes, không đọc/mutate Unity objects; single writer giữ thứ tự revisions. Không overwrite corrupt main/backup bằng nhân vật mới im lặng; giữ bản hỏng để điều tra và báo recovery rõ.

Autosave 120s; critical save: logout/disconnect, class choice, Tẩy Mạch, quest/reward, enhance **success lẫn fail**, Boss loot/reward, Stage Summary. Upgrade fail vẫn tiêu tiền/đá nên phải persist. Serialize writes theo character; transactionID/claimed flags chống request retry cấp thưởng hai lần. Crash consistency inventory/currency/reward cùng transaction là acceptance, không chỉ chứng minh JSON parse được.

JSON P0; MariaDB P1 sau domain/repository ổn. Host crash/restart không đồng nghĩa mọi client có bản save đáng tin để upload.

<a id="timers"></a>

# 7. Timers & Inactive Maps

| Timer | Authority | Empty map/re-entry |
| --- | --- | --- |
| Normal/Linh Biến slot respawnAtUtc | Global SpawnManager | Brain pause; nếu now≥deadline thì restore đúng một lần |
| Ground loot despawnAtUtc | LootManager | Expire dù root không active; lifetime theo GDD §7, không phụ thuộc active root |
| Boss nextSpawnUtc | Global WorldBossManager | Countdown/banner không phụ thuộc player ở Huyền Tích |
| Skill/Food/Potion | Host runtime clock | Local hit-stop không pause; offline policy chờ quyết định |

Dùng UTC deadlines cho lifecycle cần tồn tại qua inactive maps; runtime combat dùng clock nhất quán, không trust client timestamp. Re-entry resolve overdue timer idempotently trước gửi snapshot. Tránh catch-up sinh nhiều Boss hay pile-up cả chuỗi respawn khi map lâu rỗng. Restart semantics ngoài nextBossSpawnUtc cần gate SAVE-01.

WorldBossManager cập nhật threat/contribution từ actual HP lost sau validate (cap overkill), dedup action/target/hitIndex; target resolver chỉ chọn alive/arena hợp lệ. Reset countdown hủy khi có alive player trở lại, deadline và threshold đọc [GDD §5](1_HUYEN_LO_GDD.md#gdd-5). Reset phải clear cả threat/contribution, không giữ bằng dead-body count. Một global entity; story display state không tạo thêm Boss. Cuồng Mạch flag một lần khi HP≤30%, phase reset cùng encounter; future cadence timers đọc multiplier GDD, giữ telegraph/windup và không reschedule hit/projectile đang pending. Một action chỉ emit phase roar/Impulse local một lần; Nham Thạch pressure count/overlap author sau BOSS-02, không thêm attack set.

BossDeathEvent chụp eligibility snapshot (connected/map/contribution, kể cả dead hợp lệ theo GDD) một lần rồi RollPersonalReward/SpawnOwnerLoot. Threshold derive `runtimeMaxHP * contributionFraction`, demo config đổi runtime HP thì threshold cũng đổi. Exclusive gear roll và auto-credit Gold theo GDD §7. Pickup verify owner/map/range/instance/count, idempotent trước consume ground object; bag full không mất item. Boss death có nhiều recipients: persist eventID+eligible snapshot+rolled payloads/instanceIDs+nextSpawnUtc trước dispatch, rồi commit riêng mỗi character với reward receipt cùng transaction. Recovery retry recipients chưa commit, không reroll hoặc giả định ghi atomically N character files; giữ nguyên loot expiry deadline, không reset90s mỗi reconnect. Claimed/expired receipt ngăn spawn lại; ground-state restore details còn SAVE-01. Spawn/reward/despawn và banner đều có eventID/deadline, không lặp mỗi tick. Q12 accept/Ready không đổi nextSpawnUtc trước BOSS-03.

<a id="art-contract"></a>

# 8. Art / Animation Technical Contract

| Hạng mục | Contract | Kiểm import |
| --- | --- | --- |
| Male rig | 64×64px, PPU 32, body 44–48px, pivot Bottom-Center | Point filter, không texture blur; căn chân |
| State frames | Idle4 / Run6 / Jump2 / Fall2 / Attack3 / Skill4 / Hit2 / Death3=26 | Parts cùng state/index, đúng tổng frame đã khóa |
| Modular parts | BodyBase/HairHead/Armor/Pants/Weapon; ba visual families | Weapon Front/Back theo pose; không slot phụ vô cớ |
| Facing | Vẽ hướng phải, flipX trái | Sockets/attack origins flip đồng bộ |
| Collider | ~0,60–0,65u×1,45u, TUNABLE | Không lấy kích thước toàn canvas |
| Mob/Linh Biến | 6 normal sets, một shared modifier scale/aura/tint/name/HP bar | Exact same sprite/animation/AI/projectile; không dedicated variant set |
| Background | Forest/Mountain/Ancient +hub props reuse | Telegraph, loot, chat không bị foreground che |

Feedback renderer nhận event Host; damage/hit/status dùng combat timeline, không trust client AnimationEvent. Local hit-stop/flash/trails không pause simulation hoặc thêm stun. Content feedback thuộc GDD §11.

| Presentation contract | Quy tắc |
| --- | --- |
| Actor parts | Một SortingGroup; Shadow0 → WeaponBack5 → BodyBase10 → Pants14 → HairHead20 → Armor22 → WeaponFront30 → CombatFX40. Shared state/frame controller; VisualRoot/sockets flip đồng bộ. Kiểm overlap ở cả hai hướng và nhiều actors. |
| World sorting | Background → terrain/back props → actors → attack/telegraph → readable feedback/worldUI. Foreground không che telegraph/loot/chat; palette/accent theo ba families GDD, không thêm slot/frame set. |
| Pooled VFX | Một pool/config chung cho projectile/telegraph/Freeze presentation và hit/damage/NÉ/heal/upgrade/death/slash feedback. Reuse reset tint/timer/owner/action; release hủy timers/listeners. |
| Lifetime | Authoritative projectile/action có instanceID/generation riêng; callbacks reject instance đã despawn/reuse, không dùng GameObject reference làm lifetime ID. ObjectPool chỉ quản lý presentation. |
| Hit/death | Hit state không tự stun; Freeze dùng overlay chung cho normal/Linh Biến. Death animation/tint giữ corpse cho camera/eligibility. |

Asset gate: làm một full rig 26 frames, một weapon/armor/pants family và một farm-room trước sản xuất đủ tiers. Kiểm outline/palette/pivot/frame sync khi chuyển gear. AI-generated bitmap nếu dùng vẫn cần slice/clean/import/QA; chưa tạo asset trong vòng docs này.

<a id="ui-notes"></a>

# 9. UI Technical Notes

Một router/modal stack cho NPC/inventory/character/quest/PvP. InputActionAsset giữ Gameplay/UI contexts; khi chat/modal active tắt gameplay actions và clear buffered movement/attack. S+Space resolver ưu tiên DropThrough, không cùng frame emit Jump. Chỉ owner local gắn input/camera/HUD; PlayerInputManager cho local couch join không cần cho world online. NPC services và bindings thuộc [GDD §10](1_HUYEN_LO_GDD.md#gdd-10).

**QuickConsumableAction:** bindings xem GDD §10; Host validates inventory/level/count/cooldown/state, chọn item theo GDD và consume một lần; bước M tutorial Q6 ưu tiên reserved Potion I, không để highest-tier rule tiêu món khác mà bỏ kẹt objective. UI hiển thị reject reason; không copy hotkey table hoặc tutorial content. Quest HUD dùng state snapshot, không chuyển Available bằng client-side objective flag.


| View | Nội dung / validation |
| --- | --- |
| HUD | HP/MP/EXP, skillCD, Food/Potion, quest, Boss timer; server state |
| Inventory/shop | Capacity/stack rules từ GDD §7; Host transaction trước refresh view |
| Upgrade | Preview dùng cùng stat evaluator với runtime; Tinh Hoa concept có label, amounts chưa duyệt không quảng cáo số; fail loss persist |
| Quest/chapter | State/nextLevel/turnInNpcId từ Host; Ready cho tiếp tục chơi; Completed summary theo GDD; Q12 dùng Main Story Complete |
| WorldUI | Render HitResult; chat lifetime/line limit theo GDD §9; Boss display per character |
| Death/PvP | Death choices khác PvPDefeated; consumable restrictions đúng mode |

Bag Sort/protection gear/quest arrows/history P1; P0 validation không phụ thuộc QoL. Tooltip làm tròn để đọc nhưng internal fractional enhance không mất gain. Không hiện microservice/RPC/saveAPI trong player flow.

<a id="roadmap"></a>

# 10. Technical Spike & Roadmap

**Gate đầu: spike ba ngày** — Host + ít nhất 1 Client với profiles độc lập (P0 acceptance: tối thiểu 2 concurrent players); move/jump/drop-through; portal chuyển MapId; client khác map không hit/loot/chat được; Host load character ID; basic persistence crash experiment. MPPM hỗ trợ iterate; chạy thêm 3–4 concurrent players để bắt fixed-pair assumptions, không công bố capacity chỉ từ ca pass. Observer optimization không chặn gate này. Nếu movement correction/profile/save fail, xử lý trước mở content production.

| Tuần | Milestone | Gate |
| --- | --- | --- |
| 1 | Spike/runtime/movement/profile | Tối thiểu 2 concurrent players, MapId, định nghĩa data |
| 2 | Combat slice/rig pipeline | Normal, skill Lv5, multi-target, separation/melee miss/projectile, một modular family |
| 3 | Progression/items/consumables | EXP/attributes, class reset, Food/Potion/Death, inventory/loot channels+sellValue; freeze COOP reward recipients |
| 4 | Gear/shop/quest framework | Enhance sáu slots, safe transactions, Q1–Q8/forced variant, recipient policy/quest assist đã rõ, scope checkpoint |
| 5 | World groups/remaining skills | Năm farm roots/25 groups TEST, sáu mob/Linh Biến cap1, toàn bộ active/passive |
| 6 | Boss/co-op/persistence | Personal loot/reset/Cuồng Mạch, Q11/Q12; COOP/BOSS policies phải chốt trước dependent code |
| 7 | PvP/chat/story/demo | Q9 optional, match 120s, banners, summaries, profiles |
| 8 | Integration/QA/package | Regression, evidence tối thiểu 2 concurrent players, build/scripts/video fallback |

Working target 160–200h, risk envelope 160–240h, tám tuần; không phải estimate đã chứng minh. Art worksheet tham khảo ≈164–210h riêng vượt tổng: phải đo slice rồi chọn reuse/giảm polish P1/P2. Không giảm sáu mob/hai class/26 frames user-lock để ép schedule. AI giảm boilerplate code/docs, không bỏ Editor/prefab/animator/tilemap/slicing/UI/network debugging/playtest. Milestone trượt thì cập nhật giờ và scope P1 minh bạch.

**Contingency/cut ladder:** review sau Art Vertical Slice, cuối Week 2 và Week 4 scope checkpoint, bằng actual spent/remaining hours. Cắt P1/P2 trước; sau đó giảm cosmetic polish/additional sound/VFX variations, elaborate Journey summary presentation (giữ scores/summary core), Storage presentation depth (giữ 40 slots/basic deposit-withdraw), extra UI polish/QoL. Không tự cut hai classes, sáu archetypes, 26-frame rig, Lv1→20 hoặc multiplayer core; không demote PvP/Q9/MapChat nếu chưa user approve. Nếu vẫn vượt 160–240h, đổi schedule hoặc trình concrete scope tradeoff để user quyết, không gọi nghiệm thu phần thiếu là done.

Linh Biến tái dùng normal sprites nhưng vẫn cần shared aura và integration/QA; không suy ra giảm giờ vẽ hai bộ Elite riêng vì baseline vốn không có các bộ đó. Đo full rig/family/farm-room ở Week 2 và actual placement/route/QA cho 25 groups trước cam kết budget; bỏ anchors/timers cũ không chứng minh tổng giờ giảm.

<a id="risks"></a>

# 11. Risk Register

| Risk | Mức | Evidence / gate | Xử lý |
| --- | --- | --- | --- |
| MP spam/INT value | CRITICAL | S04 deficit lớn khi INT 0 | Chốt dependency, test rotation thực |
| Co-op formula thiếu | CRITICAL | COOP-01 | Chốt policy trước reward code |
| Profile binding/save corruption | HIGH | TECH-01 / SAVE-01 | Spike ID binding, crash recovery, một writer |
| Prediction/latency | HIGH | Chưa playable | Đo movement correction với tối thiểu 2 concurrent players |
| Art/editor hours | HIGH | Art worksheet vượt budget | Một full family làm gate, reuse |
| Boss fairness/TTK | HIGH | S07 assumptions/BOSS-02 | Đo uptime, telegraph, target/reset |
| Quest full bag/onboarding | HIGH | QUEST-02 | Reward transaction và solo Q9 regression |
| Physics/target readability | HIGH | PHY-01/ART-01 | Test airborne/platform/target sau lưng |
| Observer performance | MEDIUM | P0 còn replicate | Profile rồi optimize P1; MapId correctness trước |
| World farm/economy | HIGH | SCOPE-01/S16/25 groups TEST | Measure 2/3/4-player starvation/flow; tune rate/reward/layout, không cut mechanic từ toy model |
| P1 scope creep | HIGH | Nhiều audit proposals | Chọn sau gate P0; research không là DoD |

<a id="qa"></a>

# 12. Demo / QA / Acceptance Tests

P0 acceptance: **tối thiểu 2 concurrent players**, Host + ít nhất 1 Client; profile demo Host/Client là fixtures, không giới hạn registry/spawn. QA mở rộng 3–4+ phải ghi machine/build/package versions, CPU/frame time, bytes/messages và latency trước capacity claim. Không cần Party/Channel/dedicated server.

EditMode: EXP/attributes/equipment evaluator, inventory preflight+idempotency, per-ID quest DTO round-trip, corrupted/partial save và write failure. PlayMode: multi-actor platform drop-through, portal/camera/MapId, authoritative timeline, pool reuse. Multiplayer: MPPM khi dev + standalone Host/Client acceptance; Network Simulator kiểm delay/jitter/loss/reconnect, forged sender binding/retry không nhân reward. Các tests giữ domain invariants, không chỉ mirror implementation; manual build evidence vẫn cần cho game feel.

**Tất cả ca dưới đây: CHƯA CHẠY.** Đây là checklist nghiệm thu tương lai; vòng hiện tại chỉ chạy doc validation và phép tính. Không đánh dấu playable/done bằng mô phỏng.

| Profile | State cố định | Demo |
| --- | --- | --- |
| demo_new | Lv1, Novice; kiếm chỉ cấp ở Q3 | Movement/Q1–Q4 |
| demo_class | Lv5, Q5 Completed/Q6 Available, 20 unspent, chưa class | Chọn class/reset/equip/skill Lv5 |
| demo_mid | Lv10 | Farm group/gear/Food/Linh Biến |
| demo_end_host | Lv20, character riêng | Boss/PvP Host |
| demo_end_client | Lv20, character riêng | Boss/PvP Client |

DemoExpMultiplier 10, BossHP×0,40, BossRespawn60s phải là demo config rõ; release không dùng nhầm. DemoBossMaxHP 12.800 và contribution 640 cùng derived value, không chỉ UI. Quay class selection bằng demo_class, không demo_mid.

| Bộ nghiệm thu | Ca bắt buộc | Evidence |
| --- | --- | --- |
| Progression | Cumulative 53.100, carry EXP, catch-up remaining, reset Lv5 một lần, trì hoãn Q6 tới Lv6+ giữ total points/normal Mộc Kiếm và M reserved potion, 95 điểm, không branch cap/rank | State trước/sau và reload |
| Combat | Timeline hit/spawn đúng clock khi client drop frame; AnimationEvent không mutate gameplay; ba normal intervals, mọi skill/hit count, snapshot/pierce/explosion, vertical/facing/no contact | Hit log/video Host + Client |
| CC/AI | Cross-behind/vertical miss tại HitMoment, no contact/shoving, stable separation/leash, Host projectile, Ong melee-accessible; Freeze normal/Linh Biến, Boss/PvP chỉ Slow; Hybrid reachable melee, blocked ranged, Sói melee | Cases theo target/mode |
| World/timers | 2/3/4 players same-map farm, no whole-map starvation, safe portal strip, no chain aggro cả map, traversal/run-back logs; N-player root occupancy; drop-through một actor không ảnh hưởng actor khác; 25 groups TEST/TUNABLE, Return reset HP, deadlines map rỗng/re-entry, respawn idempotent | Clock/MapId logs |
| Boss | Cuồng Mạch≤30% once/reset, cadence future-only và không mất telegraph, 2/4 players (8 nếu performance cho phép), Threat/retarget, dead-body reset 10s, threshold release/demo, dead eligible, disconnect/leave ineligible, loot 90s | Contribution/reward logs |
| RPG | Gold auto-credit/recipient ledger, one exclusive gear max, map-tier/six slots/weapon50–50, Potion level-tier/HP–MP50–50, sellValue khi buyPrice null; sáu slots×năm tiers×rarities có gain; +0..5 strict gain, Tinh Hoa derive/reload không stack, reject undefined bonus/roll policy, fail persist/full bag | Transaction/inventory logs |
| Linh Biến/economy | 10% TEST roll, max1/MapId under simultaneous respawn, normal slot/count/lifecycle, no reconnect/reroll race; Q8 reservation đúng Sói Lv8 khi variant khác idle/full HP hoặc đang combat/Return, giữ respawn deadline; no bonus/Journey replay; measured Stone/gear/Gold/material-hour vs sinks | Seed/slot/generation/recipient logs, throughput không capacity claim |
| Quest | State graph/level gate/đúngNPC; Q5/Q7 guaranteed supply không RNG, Q6 M thật; replay/despawn/reconnect/full bag không mất/nhân reward; per-ID Q9 song song Q10 và skip solo; evidence pre-farm reject/cleanup; Q11 unlock chỉ Completed; Q12 Main Story Complete/continued play | Solo+online journey |
| PvP/chat | Invite/accept, level gap,120s/%HP timeout, chặn consumables, Food pause, MapChat/rate/80 ký tự | Host + Client; chốt edge policy |
| Save/network | N-player registry/recipient sets; Q9+Q10 DTO round-trip; write failure không ack durable success; reject forged state/replay, ID binding, disconnect save, crash từng bước, backup recovery | Corrupt/crash artifacts và log |
| Art/UI | 64×64/PPU 32/26 frames, pivot/frame alignment, gear flip, layer readability, input context | Import audit/video |

Chuẩn bị build đóng gói, README chạy demo trong bộ phân phối khi implementation tới gate, script mở Host/Client, reset demo profiles có chủ đích và video fallback. Không tạo thêm Markdown trong active design tree ở vòng này. Chọn rubric/trình tự trình diễn theo feature đã chạy; video fallback không thay evidence acceptance.
