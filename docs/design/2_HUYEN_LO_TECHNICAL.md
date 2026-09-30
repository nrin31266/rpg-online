# Huyền Lộ — Hợp đồng kỹ thuật

**Đối chiếu:** [GDD hiện hành](1_HUYEN_LO_GDD.md) · **Ngày:** 2026-09-30 · **Trạng thái:** specification trước triển khai.

[GDD](1_HUYEN_LO_GDD.md) quyết định gameplay; [Analysis](3_HUYEN_LO_DESIGN_ANALYSIS.md#open-decisions) giữ các gap phải chốt. Các tên component / API dưới đây là hướng triển khai, cần spike với phiên bản Unity / package thực dùng; chưa có Unity build hay runtime test trong vòng dọn tài liệu này.

<a id="runtime"></a>

# 1. Runtime Architecture

| Layer | Trách nhiệm | Biên giới |
| --- | --- | --- |
| Definition | ScriptableObject: class, skill, mob, item, quest, map | ID ổn định; không giữ state mutable trong asset |
| Host simulation | Movement validation, combat, AI, loot, quest, economy, timers | Authority duy nhất cho kết quả gameplay |
| Session state | Player state, Mob / Boss instances, per-instance contribution / threat / status | Không coi client cache là save |
| Persistence | Character / profile repository và world timer | Schema / version / migration, transaction và recovery |
| Presentation | Sprite parts, VFX, HUD, chat, sound | Render feedback không thay server clock |

**Một nguồn definition:** SkillDefinition / MobDefinition / ItemDefinition / QuestDefinition / MapDefinition là ScriptableObject static authoritative; không song song cùng stat trong `.asset` và `.json`. JSON chỉ runtime save / snapshot persisted. DemoConfig dùng một ScriptableObject riêng chọn ở boot, không override cùng value từ JSON thứ hai; demo profiles là runtime JSON fixtures, không chứa bản copy definitions.

Baseline fixed physics 50 Hz, network 20 Hz, render target 60 FPS; tune qua profiler, không hứa đạt trước benchmark. NGO + Unity Transport là hướng baseline; RPC handlers chỉ validate sender / intent rồi gọi domain functions; không custom transport adapter, DI / service bus hoặc repository framework. Offline practice dùng cùng Host simulation, không tạo engine damage thứ hai. Không dựng dedicated production server P0.

**Multiplayer state:** World support N players. P0 không Party registry/membership, invitation/leader/UI/HP bars/chat/bonus/quest-sharing/raid; cùng đánh một target đi thẳng qua per-character ledgers, không membership prerequisite. PvP invite chỉ thuộc mode 1v1. Cụm quái (`SpawnGroup`) là authoring/layout, không nhóm người chơi. PlayerRegistry, connection → profile / character binding, MapId membership, Threat / Contribution, quest assist, chat rate limits và loot eligible recipients dùng dictionaries / sets theo playerId; spawn từ mỗi connection hợp lệ, không hai fixed slots hoặc MaxPlayers = 2. Network clientId là handle của connection, không save characterId; request dùng sender thực do network cấp để resolve binding, không trust playerId từ payload. Mob / Boss resolver duyệt N valid candidates; death snapshot duyệt toàn bộ contribution ledger, một physical set / pile và per-character EXP / Gold / quest receipts. Config capacity ở session admission tách khỏi gameplay, chỉ công bố sau benchmark 3–4+.

**Unity / tooling baseline — adopt theo mục đích, chưa cài package trong vòng docs:** Khi tạo project chọn supported Unity LTS và package tương thích, pin Editor patch + manifest / lock sau spike; không auto-upgrade hoặc lấy bản mới nhất. Unity 6.3 LTS là mốc documentation đã kiểm, không ép migration project đang chạy ổn.

| Kỹ thuật | Áp dụng cho Huyền Lộ / biên giới |
| --- | --- |
| [Input System — actions](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.20/manual/index.html) | Adopt một InputActionAsset với Gameplay / UI contexts; owner local duy nhất đọc input, chuyển intent sang Host. Giữ bindings GDD, không thêm hotbar / rebind UI. |
| [NGO](https://docs.unity3d.com/6000.3/Documentation/Manual/com.unity.netcode.gameobjects.html) + [Unity Transport](https://docs.unity3d.com/6000.3/Documentation/Manual/com.unity.transport.html) | Giữ Host-authoritative baseline; không viết transport / RPC layer thứ hai. Chọn package version đã xác minh release / Editor compatibility ở spike; lưu ý release channel bên dưới. |
| [Multiplayer Play Mode](https://docs.unity3d.com/Packages/com.unity.multiplayer.playmode@2.0/manual/index.html) | Adopt dev workflow Host + Clients trên cùng máy; tối đa 4 Editor Players là giới hạn tool, không world. Acceptance cuối vẫn có standalone builds, test 3–4+ đo capacity riêng. |
| [Multiplayer Tools / Network Simulator](https://docs.unity3d.com/Packages/com.unity.multiplayer.tools@2.2/manual/network-simulator.html) | Adopt profiling và giả lập delay / jitter / loss / disconnect khi dev; ghi preset và logs, tránh double simulation ở cả tool và transport. Không ship simulator bật trong release. |
| [Unity Test Framework](https://docs.unity3d.com/Packages/com.unity.test-framework@1.6/manual/index.html) | Adopt EditMode tests cho domain / repository, PlayMode cho physics / lifecycle; dùng batch runner của package tương thích, không xây test framework riêng. |
| [UnityEngine.Pool.ObjectPool](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Pool.ObjectPool_1.html) | Adopt cho local presentation VFX / damage text; callback reset / release, collection check ở dev. NetworkObject pooling chưa cần P0; không dùng pool thay network spawn / despawn. |
| [Cinemachine 3 Position Composer](https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachinePositionComposer.html) / [Confiner2D](https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineConfiner2D.html) / [Impulse](https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineImpulse.html) | Adopt local orthographic follow / bounds / shake, giảm custom camera code; không camera network authority hoặc cutscene system mới. |

Input / PlayMode / Tools / Cinemachine có released entries trong Unity 6.3 catalog; [Test Framework là core package](https://docs.unity3d.com/6000.3/Documentation/Manual/com.unity.test-framework.html). Catalog NGO đang đánh dấu pre-release dù có upstream release tags: giữ lựa chọn framework, xác minh exact package / channel trước dependency freeze ở gate hiện có, không tự gọi latest là stable. Không chuyển topology để giải quyết metadata. DOTS / Entities / ECS, Addressables / streaming, cloud / dedicated, prediction / rollback phức tạp và service / event framework lớn chưa có lợi ích đo được cho slice; giữ kiến trúc hiện hành.

<a id="maps"></a>

# 2. Unity Scene / Map Architecture

Boot → ProfileSelect → CharacterSelect → WorldOnline. Một world scene có **8 logical map roots**: Village, Academy, Arena và năm farm maps. Root origins derive từ actual world bounds + separation margin; ~200 u chỉ điểm bắt đầu của spike, không fixed spacing / width. Kiểm bounds / colliders không overlap sau khi dimensions đổi, không để map dài hơn offset nối physics hai roots. MapDefinition giữ MapId, spawn point, portal destination, bounds, SpawnGroups và environment family.

| Hợp đồng | P0 | P1 |
| --- | --- | --- |
| Map identity | Host giữ MapId; kiểm combat / chat / loot / portal | Không thay bằng scene visibility |
| Presentation | Camera và render filter chỉ root hiện hành | Streaming nếu profiler cho thấy cần |
| Network observers | Có thể còn replicate khác map; client không render | NetworkHide / NetworkShow sau correctness |
| Portal | Host kiểm nguồn, range, state, destination gate; đổi MapId + position | Loading polish |
| Empty root | AI pause; timer vẫn thuộc global manager | Interest optimization |

Mỗi MapRoot gồm BackgroundTilemap / GroundTilemap / PlatformTilemap / ForegroundTilemap, SpawnPoints, Portals, NPC anchors. Ground baseline: TilemapCollider2D + CompositeCollider2D + Static Rigidbody2D; [Unity 6 Composite Operation = Merge](https://docs.unity3d.com/6000.3/Documentation/Manual/tilemaps/work-with-tilemaps/tilemap-collider-2d-reference.html), không hướng dẫn checkbox UsedByComposite cũ. PlatformTilemap tách ground, [PlatformEffector2D Use One Way](https://docs.unity3d.com/6000.3/Documentation/Manual/2d-physics/effectors/platform-effector-2d-reference.html); nếu composite thì UsedByEffector do CompositeCollider điều khiển. S + Space drop-through chỉ tạm bỏ collision cặp actor–platform đang đứng, restore sau khi actor xuống dưới hoặc cancel / death / portal; không toggle effector / layer collision toàn world làm player khác rơi. PHY-01 spike kiểm nhiều actors và nhiều tầng; không cần custom collision engine. Background / Foreground không gameplay collider. Tile dimensions / world cell derive từ asset contract PPU 32, tile size không suy ra từ kích thước rig. Không HazardTilemap P0 (Hazard P2). MapDefinition vẫn owns bounds / spawns / portals / environment / unlock data; BossCombatArea là spatial region trong Huyền Tích, không scene / root / portal / Lv 20 gate; bounds / normal-spawn exclusion theo GDD §4. Local Cinemachine target chỉ character owner (dead vẫn corpse); portal đổi confiner bounds theo root và snap / cancel damping qua offset giữa roots, invalidation cache khi đổi shape / lens, không follow player từ xa.

**Map authoring gate:** layout / pockets / safe strips / spacing / traversal targets theo [GDD §4](1_HUYEN_LO_GDD.md#gdd-5). Author one farm room trước toàn bộ pockets; centers / terrain phải isolate group aggro. Measure traversal và Village run-back riêng; scale SpriteRenderer Linh Biến không scale hurtbox / aggro / leash tự động. Collider / hitbox do data / PHY-01 quyết định, không variant transform kéo physics bounds vô tình.

MapDefinition đọc requiredLevel / unlockFlag từ [GDD — map gates](1_HUYEN_LO_GDD.md#gdd-5); Host kiểm cờ **Completed**, không objective counter hay ReadyToTurnIn. Mọi player có MapId riêng, có thể ở khác map. Root activity tính từ collection player trên Host, không từ map local của một Client; presentation filter không disable simulation / NetworkObject của player khác. Portal từ chối không làm mất player hoặc tạo duplicate; chuyển map cập nhật target / threat / interaction và gửi snapshot root mới. Chỉ render filter không đủ bảo vệ gameplay.

<a id="combat-data"></a>

# 3. Combat Data Architecture

| Definition / state | Trường cần có | Validation |
| --- | --- | --- |
| SkillDefinition / profile | Stable SkillId, class, UnlockLevel / ManualRequirement, EvolutionProfileId / level, power / MP / CD / shape / maxTargets / executor / timeline / status / VFX | SingleMelee / SingleProjectile / Arc / SnapshotSpread / Line / Explosion; không named-skill branch, SkillRank hoặc general graph editor |
| SkillAcquisitionState / PassiveDefinition | LearnedProfileIds; bốn passive IDs, class / unlockLevel / icon / tooltip / effect params | Active learned từ manual; nội tại derive class+level theo GDD §3, không persist duplicate flags / rank / points; effect enum nhỏ, không proc graph |
| CombatRequest | character binding, actionID, sequence, facing / aim intent | Host kiểm alive, map, cooldown, MP, range và target |
| HitResult | actionID, targetID, hitIndex, evade / crit, damage, remainingHP | Client chỉ hiển thị kết quả Host |
| MobDefinition | mobIdentityId (7), fixedLevel, baseRigId (6), paletteRef; base formula, AI / timeline / hitbox / projectile / sourceProfileRef, linhBienEligible | Level thuộc identity; Sói Trúc reuse rig / AI Sói Sương, palette khác. Curve evaluate fixedLevel; không duplicate animation / AI cho palette variant |
| LinhBienModifier | stat / reward modifiers, visual preset, variant tag | Config apply / derive trên base runtime; không duplicate bảy base identities |
| SpawnGroup / SpawnSlot | mapId / groupId / slotId, mobIdentityRef / cached level / spawnPosition, respawnAtUtc, generation, variantState, Q8 waiting set | Author pockets GDD; SpawnSlot.level phải bằng MobDefinition.fixedLevel; respawn giữ identity, chance 5% chỉ Lv 8+, normal slot lifecycle 25 s |
| ItemDefinition / Instance | templateID, instanceID, rarity, enhancement, count; optional buyPrice, explicit sellValue, materialId / loot refs / binding | Không fake buyPrice cho drop-only; instanceID duy nhất; tutorial-bound không merge bản vendor |
| QuestDefinition | prerequisiteQuestIds, requiredLevel, ordered objectiveGroups / sourceIds / markerIds / counts / guaranteedEvidenceOrdinals, turnInNpcId, rewards / unlockFlags | Authoring theo [GDD §5](1_HUYEN_LO_GDD.md#gdd-6), không copy quest table |
| QuestProgress per QuestId | state, activeObjectiveGroup, counters / qualifyingKillOrdinal / virtualEvidence, activation / claimed / staged flags | Host-owned runtime dictionary; save qua DTO §6, idempotent claim / save |

**Quest runtime:** `Dictionary<QuestId, QuestProgress>` là source of truth theo từng quest ID. Q9 Available / InProgress cùng Q10 Available / InProgress là hợp lệ; `currentQuestId` toàn cục không được làm nguồn duy nhất. UI có thể pin một chính tuyến, còn nhánh optional vẫn persist. Definition giữ ordered objective groups và event / source filters theo GDD; chỉ active group cập nhật. Q6 class transaction đồng thời activate equip / learn / confirm group trước actions tiếp theo; Q7 confirm kiểm đúng selected owned ring instance ≥ +1, không đòi re-equip nếu đã mặc.

**QuestState model:** Locked → Available → InProgress → ReadyToTurnIn → Completed. EligibilityService kiểm đủ prerequisite quest IDs **và** requiredLevel; branch optional chỉ theo prerequisite graph GDD, không implicit Qn−1. Thiếu level trả Locked + reason / nextLevel cho NPC / HUD, không marker Available.

QuestService consume event từ hành động Host thành công, có eventID / characterId / MapId / targetId cùng payload cần thiết: `NpcTalked`, `RegionVisited`, `ItemPickedUp`, `ItemEquipped`, `ItemSold`, `ConsumableUsed`, `MobKilled`, `VirtualEvidenceCredited`, `AttributeAllocated`, `ManualLearned`, `SkillUsed`, `EnhancementSucceeded`, `PvPCompleted`, `BossEligibilityAchieved`. Khớp QuestId / active objective / group / target, chặn replay rồi commit counter / step trước publish. Pickup / equip / sell / use / upgrade chỉ emit sau mutation thành công; consumable reject khi đầy không emit Used. Mutation character / progress và death snapshot cùng HostSave command; retry dùng payload / receipt đã chụp. Click UI hoặc client tự báo “đã làm” không tăng tiến độ. Không yêu cầu framework event bus lớn.

QuestService validate progress từ event Host hợp lệ; đủ objectives chỉ chuyển ReadyToTurnIn, không teleport hoặc gọi reward. TurnInRequest kiểm connection / character binding, state, turnInNpcId, sameMap, interaction range và claimedFlag. Một transaction commit reward + Completed + story / unlockFlags + summary eligibility, persist rồi publish; replay không cấp lần hai. Preflight reward insertion tính compatible stacks và X slots còn cần theo GDD. Thiếu capacity trả reason + X, không mutate reward / evidence / flags, giữ ReadyToTurnIn; retry idempotent. Completion transaction gồm cleanup evidence. Không phát gold / EXP một phần rồi fail physical item.

Tutorial staged grants là receipt `(characterId,QuestId,grantId)`; one-time / pending giữ khi full bag. Q5 gear / trade sample là supply thật, không evidence; ground expiry / reconnect restore entitlement chưa claimed với cùng instanceId, không cấp lại reward. Bind áo tới equip / sample tới sell để không mất đường tutorial. Q6 chọn class + grant weapon / manual nhập môn trước learn / cast objective; confirm spent ≥ 1 chấp nhận đã cộng trước, không ép point mới nếu unspent 0. Q7 select owned ring, grant chỉ nếu thiếu; persist selectedInstanceId và giữ binding không sell / drop tới confirm equip thành công rồi gỡ. Reserved stone / Gold chỉ khi cần +0 → +1, đã ≥ +1 confirm không cấp resources thừa. Entitlement có pending / spawned / claimed / consumed state; reserved supply không dùng ngoài objective trước bước cho phép. Q12 Boss death credit và quest turn-in là hai receipts riêng: báo NPC không tạo thêm Boss pile. Completion và staged grants có IDs riêng, không grant lần nữa khi turn-in.

**Bí kíp không có cooldown hoặc timer chống học lại.** Learned flag, grant receipt và learn command ID chặn duplicate; chỉ skill có CD, giữ deadline qua evolution.

**Manual learn command:** ItemDefinition categoryManual giữ class, requiredLevel, prerequisiteProfileId, unlockProfileId, boundCharacterId; sáu definitions, không book economy / skill tree. Host kiểm sender / ownership / alive / idle / no pending cast, đúng class / level / prerequisite và chưa learned; preflight consume + learned flag trong cùng save command. Owned storage cho phép cất sách; learn từ bag. Invalid / too early / duplicate không consume. Reconnect restore learned IDs, không derive active từ level đơn thuần; mất / mâu thuẫn manual / quest grant receipt là save validation / recovery, không silent regrant. Nội tại derive class+level Lv 5/Lv 13; chọn class muộn mở mọi mốc đã đủ, không re-grant points/active. Evolution giữ SkillId / CD: nextAllowed = max(old deadline, lastCast + newProfileCD), không reset CD / MP hoặc pending projectiles; action đã bắt đầu giữ immutable profile snapshot. UI đọc learned profile / required quest, hotkey 1 đổi profile, hotkey 2 big.

**Virtual evidence / quest participation:** QuestDefinition chỉ virtual counters, không physical quest item / bag / pickup RPC. Host kiểm QuestId / InProgress / active group / source và count<required; qualifying kill threshold 20% RuntimeMobMaxHP, Boss 10% RuntimeBossMaxHP và mode-specific eligibility từ GDD. Snapshot quest eligibility trước increment step; quest / evidence counters, ordinal và receipt cùng character mutation. Hit actual-loss đồng thời tag activeQuestId / objectiveGroup / targetGeneration và cộng questQualifiedHpLost; damage trước accept / sai step không hồi tố. Reward / threat vẫn dùng entire-life ledger, không mất regular share vì step đổi. Death dùng quest-qualified ledger cho threshold, không total damage rồi kiểm mỗi state-at-death. Guaranteed ordinals author từ GDD, không RNG; count luôn clamp. Level eligibility không áp quest / supply. Tracker text / icon riêng recipient, không network ground object. Q11 activation validate 3 virtual counters từ phía ngoài portal, commit flag idempotently; cleanup chỉ successful turn-in.

Inventory insert: fill compatible stacks trước, overflow tạo stack tới capacity; plan insertion rồi commit toàn amount hoặc giữ ground / pending intact theo GDD. Compatibility gồm template / binding và instance flags, manual / tutorial binding tách khỏi compatible unbound stack. Bag Sort P1 không là điều kiện của auto-stack P0.

ItemDefinition giữ rarityPrimaryStatIds, enhanceStatRules, fixedSlotBonusDefinition và equipLevel theo [GDD §6](1_HUYEN_LO_GDD.md#gdd-7). Sáu weapon templates có templateID/tên/phái riêng: Kiếm có ATK + Chí mạng cố định, Cung có ATK + ACC chịu rarity nhưng không chịu hệ số enhance; không dùng một weapon definition chung rồi đổi stat theo class người mặc. Ba mẫu Giày có tốc chạy +1/2/3% cố định, cộng với phần AGI trước khi áp Làm Chậm PvP ×0,75; rarity/enhance không nhân tốc chạy. Band I non-weapon Lv 1, Weapon Lv 5 + đúng class; mọi Band II Lv 11, mọi Band III Lv 17. Drop/shop ownership không đồng nghĩa được mặc: item II từ Ong Lv 10 có thể ở bag trước Lv 11; Host kiểm equipLevel ở equip/transfer preview và commit, không suy level từ MapId/source. Compute stat gồm flat gain hiện hành; Tinh Hoa derive theo enhancement, không add-mutate mỗi equip / load. Áp trần theo item band I+4 / II+6 / III+8; cùng cấp dùng cùng bảng GDD. Tinh Hoa I/II là fixed bonus sau multiplier và flat, luôn có tooltip kể cả khi khóa. Không chấp nhận enhance command vượt cap.

**Character/stat evaluator:** đọc công thức GDD §2 và catalog §6. BaseHP/BaseMP tăng theo level; cộng thuộc tính + item đã rarity/enhance/flat/Tinh Hoa derive rồi **mới** áp nội tại nền tảng đúng một lần: Kiếm MaxHP ×1,10/DEF ×1,08 hoặc Cung MaxMP ×1,10/ACC ×1,08. Không còn class multiplier ẩn hoặc increment trực tiếp stat khi equip/load. Giữ fractional values tới damage rounding/UI. Level-up, chọn class, equip, chuyển giao và tẩy điểm cùng dùng evaluator; khi MaxHP/MaxMP đổi, giữ current rồi clamp trong transaction, không ratio-heal/revive. Restore HP/MP/Food theo §6; không hồi miễn phí khi tải lại. Hai nội tại tinh thông là bonus 1,12 cho direct skill hit đúng cự ly; không nhân normal/Bỏng hoặc status chance. Snapshot class/passive/source stat ở cast; mục tiêu và khoảng cách thực xét tại hit/impact. Không cộng nền và nội tại hai lần sau reload.

**UpgradeTransferCommand — P0:** sourceInstanceId/targetInstanceId/commandId và expectedRevision; resolve sender binding, ownership/bag, distinct IDs, slots/bands/class/equip-level/tutorial binding theo GDD §6. Cho cùng bậc hoặc lên đúng một bậc, không I→III; đích = max(cấp nguồn, cấp đích), không vượt trần đích. Cùng bậc trừ 800 Vàng, lên bậc trừ 500 Vàng + 2 đá. Plan tiêu nguồn + chi phí + cập nhật cấp instance đích; từ chối không tăng cấp trước mutation (không RNG). Một immutable result/receipt trong save queue §6, không consume rồi tạo bản copy đích. Preview và commit dùng cùng evaluator; publish inventory/stat sau durable commit. Replay trả cùng target/result, không charge lại dù nguồn đã biến mất; hai commands tranh nguồn chỉ một thắng. Failure giữ committed state; không rollback bằng spawn lại source. Equip là action sau transfer, không auto-equip hoặc cần thêm ô mới.

**Death / reward pipeline:** runtime instanceID mới / unique qua cả Host restart (GUID, không GameObject.GetInstanceID hay slotId); authored slotId giữ cố định. Một deathID = (instanceID, generation), snapshot RuntimeMaxHP, actual-loss ledger, contributor positions / MapId / alive / connected / lastDamageAt và **levels trước reward**. Snapshot trước mutate quest / EXP, sort stable characterId; source DoT attribution không dựa last hitter. GDD §6 quyết định recipients / factors / floor: không normalize share người bị loại. TopDamage chọn trên whole life ledger, không fallback để né level-gap suppression. Quest 20% / Boss 10% là predicate riêng EXP / Gold>0; không copy một eligibility list cho cả ba hệ thống. Persist rolled payload và snapshots trước dispatch, retry cùng deathID không reroll.

**Shared physical loot:** LootService dùng một roll / death theo catalog / rates GDD, exclusive integer buckets author trong data; không copy percentages ở đây. LootRecord giữ itemInstanceId, deathId, MapId, deathUtc, ownerId, contributorSnapshot **gồm playerId và levelAtDeath**, expiry (field data, không object dump). Một hàm OwnershipPhase(deadlines, now) và pickup predicates riêng mode. TopDamage eligibility100/0 cả normal/Linh set trước rolls, không RNG level gate/fallback; fixed mob level/content chọn band/slot pool, MapId chỉ chọn material theo source table GDD. Pickup dùng death-snapshot level cho character đã có trong death ledger, current level cho người đến sau, theo GDD §6; không dùng owner factor cho mọi người hoặc loại người vừa level-up từ kill. Vẫn recheck MapId/alive/distance/capacity và window predicates. Client thấy pile và prompt, Host serialize claims. Capacity preflight trước mutation, không fit thì ground giữ nguyên.

Claim thay ground → inventory +claim receipt trong **một HostSave transaction (§6)**. Hai requests tranh item được resolve nối tiếp; loser nhận AlreadyClaimed, không spawn bản thứ hai. Death commit rolled payload + recipient EXP / Gold / quest / Journey + loot cùng envelope trước publish; retry giữ deathID / payload, không reroll. Không two-phase commit / cross-file claim protocol. Receipt / deadlines đủ replay / crash safety; bounded cleanup khi death dispatch / loot đều terminal, không log mọi combat packet vĩnh viễn.

**Spawn / variant arbitration:** một Host-owned record / MapId (activeSlotId, generation, reservation, waiting characterId set), mutate trên simulation thread. Due slots serialize theo slotId; new spawn resolve authored mobIdentity/fixedLevel/position/group, apply base HP/ATK curve GDD rồi Linh modifier đúng một lần, không multiply nhiều lần. Dynamic eligibility Lv 8+, roll 5% và check cap / reservation cùng commit; Lv 1–7 không roll. Initial population cùng path; Return / root wake / reconnect không respawn / reroll. Death clear active record và đặt chính slot deadline 25 s.

**Q8 reservation:** rebuild pending requests từ active quest state / characterId + QuestId + step. Chỉ requester hiện diện Trúc Ảnh mới giữ reservation; existing Sói `TA4.slot1` Lv 8 Linh thì bind cùng generation. Variant khác sống thì chờ cap, **không demote / despawn hoặc ép Return**; chỉ promote wolf idle / full HP ở spawn hoặc due respawn. Requests shared waiting set, không spawn / player. Death snapshot credit 20% riêng từng requester; đủ credit remove pending, không đủ thì retry lifecycle. Entitlement không đổi reward: một Linh profile, cùng shared set / contribution semantics. HUD nêu cap / slot đang chờ. Restore generation / roll / request idempotently; normal restart theo §6–7, không duplicate variant đang sống.

Boss HP 32.000, ATK 160, DEF 25, ACC 140, EVA 60; ba vùng Nham Thạch Rơi và lịch một action tại một thời điểm theo GDD §4. Các vùng không double-hit cùng người. Shared lifecycle theo GDD §4; đo kích thước vùng, né và nhịp bằng PlayMode.

Client trình diễn theo actionId / startClock từ Host; bỏ lỡ frame / AnimationEvent không làm mất authoritative hit. Late packet reconcile presentation về phase hiện tại, không chạy damage lại.

**ActionTimeline:** config mốc theo GDD §3, exact feel ART-01. Host validate learned profile / level / MP / CD / action lock, charge một lần, tạo actionId/startClock/profile và source stat snapshot (ATK/SkillDamageBonus/ACC/Crit/passive capability), actionOrigin rồi schedule hits/spawns; gameplay không AnimationEvent. SnapshotSpread chọn A / B / C → A / B / A → A / A / A ở cast start; từng arrow giữ power / aim index, invalid target không reacquire. Host projectile resolve first allowed collision; big explosion dedup primary, line sort intersections. `actionId+projectileIndex+actualTargetId` dedup damage; Tại impact kiểm target alive / mode / MapId gốc và projectile lifetime / range từ spawn, không range từ vị trí caster mới; caster đã chết / portal không thu hồi projectile đã spawn. Pending caster action cancel death / portal, không thu hồi projectile đã bay nếu chưa terminal; source character attribution vẫn giữ.

**Shared StatusController:** cache `(actionId,actualTargetId,effectId)` cho Bỏng/Băng Hàn kể cả roll fail; A/A/A tối đa một application/unique actual target/cast, miss/invalid không roll. Resolve target category trước roll: Normal/Linh → Đóng Băng 1,5 s; Boss → Làm Chậm 3 s; PvP → Làm Chậm 1,5 s. Đang Frozen hoặc trước `protectedUntil = thawAt + 3 s` thì skip application/cached; không deferred proc. Freeze hủy unresolved attack/windup/spawn, không thu hồi projectile đã bay. Boss/PvP không nhận Freeze; Slow không tác động normal/Linh.

Bỏng giữ sourceCharacterId/ATK snapshot/generation/expiry/nextTickAt; Phong Trảm 4%, Kiếm Khí 70%, cùng 0,06 ATK/tick mỗi 1 s trong 6 s, không phụ thuộc nội tại Lv 13. Proc lại thay source/snapshot/expiry `now + 6 s` nhưng **giữ nextTickAt hiện có**; callback generation cũ bỏ, tick đúng expiry chạy trước remove. Một tick/target, không N × damage. Source death không xóa Bỏng; PvP immune. DEF target xét ở tick, không Crit/random/INT.

Đóng Băng dùng `frozenUntil/protectedUntil` target-wide, một duration cho quái thường và Linh Biến, không refresh. Làm Chậm dùng một `slowUntil` theo category, reapply chỉ đặt lại deadline `now + duration`, magnitude không stack. PvP nhân MoveSpeed ×0,75, không đổi attack interval, skill CD, animation hoặc action đang cast. Boss nhân tốc chạy khi reposition ×0,85 và **tốc đồng hồ chờ action kế tiếp ×0,75**; giữ `remainingActionWait`, mỗi Host tick giảm `deltaTime × currentClockRate`, khi debuff hết lại giảm `deltaTime`. Không reset về full CD, không reschedule telegraph/hit/projectile của action đã start; Cuồng Mạch chọn base future cadence trước, slow chỉ đi qua phần wait. N Cung share một debuff; timer có generation để callback cũ không kéo dài sai. Death/Return/Boss reset clear status. Client chỉ diễn status đã resolve.

**Passive hit integration:** Kiếm Thế đo từ immutable actionOrigin đến actual target hurtbox center lúc hit ≤1,2 u; Xạ Tâm đo từ immutable projectileOrigin đến actual target center tại impact ≥4 u, cả target nổ xét riêng. Bonus 1,12 chỉ trên direct skill raw, áp một lần/target trước DEF; không thêm Burn chance/tick hoặc range. Nếu collision đổi target thì xét actual target, không snapshot target cũ. Source passive capability snapshot ở cast; death/portal sau khi bắn không biến projectile thành đòn mới. Kiếm Tâm/Ưng Nhãn chỉ nằm trong final stat evaluator, không cần per-hit proc cache.

**Mob action timeline:** initial acquire nearest valid / first attacker; mỗi mob dùng ThreatResolver table riêng theo GDD sticky 1,25 ×, actual-loss ledger riêng. Challenger threat>0, không zero-vs-zero oscillation. Retarget invalid / highest valid, tie playerId; group alert chỉ wake, không copy target / threat. Return clear threat / contribution / status khi về full HP. Scan N same-MapId / leash candidates, không Player 1 / 2. Melee windup locks facing / attack origin intent, không locks guaranteed damage: tại HitMoment Host revalidate same map / alive / range / front vertical hurtbox cho target đó, player cross-behind / out-of-band thì miss. Single-target normal attack không cleave thêm players. Ranged aim snapshot tại windup, projectile spawn theo Host clock, collision query hurtbox / terrain + MapId; không homing / reaim giữa projectile flight. Return / death / Freeze / map reset cancel unresolved action bằng instanceID + generation, không delayed hit sau despawn; Freeze hủy pending windup / hit / spawn, không thu hồi projectile đã spawn hoặc hoàn cooldown; Slow / Freeze / Burn magnitude / lifetime đọc shared status data GDD §3. Không mob Crit P0 (CritChance 0 baseline GDD), Damage dùng GDD formula với mobATK / power.

**Monster separation candidate:** disable body collisions Player–Monster và Monster–Monster; hurtbox triggers / query vẫn hoạt động. Host steering trên reachable cùng-platform neighbors trong radius nhỏ: aggregate repulsion vector, clamp tốc độ / offset, damp / dead-zone, project lên đường đi hợp lệ trong leash; không Rigidbody.AddForce. Flying neighbors cùng movement band; không repel qua tường / khác tầng. During windup / hit / Return không steering đổi facing / hit origin; omit correction nếu blocked, không jitter teleport. Spatial query nhỏ theo MapId, stable neighbor order / instance IDs, replicated positions thay Client tự steer. PHY-01 kiểm crowd / near-wall / portal / leash và 2 / 3 / 4 players. Ong attack approach / hover band phải reachable bằng melee arc / jump Kiếm; clamp theo accessible platform, không dùng roam box height làm altitude cố định.

Physics layer mask tách body collision, hurtbox và ground / platform; hit query / snapshot / pierce / explosion implement theo [GDD §3](1_HUYEN_LO_GDD.md#gdd-4). MobBrain reachable / blocked và projectile config theo [GDD §4](1_HUYEN_LO_GDD.md#gdd-5), một MobProjectile prefab đọc definition; ART-01 / PHY-01 là gate test feel / vertical. Không duplicate luật roster / AI trong Technical.

<a id="network-authority"></a>

# 4. Network Authority

| Client gửi intent | Host quyết định | Client nhận |
| --- | --- | --- |
| Move / jump / drop, facing | Collision / map bounds và state hợp lệ | Position / state; interpolation / correction |
| Normal / skill, aim | Target / range / MP / CD / HP / status | Combat results và VFX trigger |
| Equip / sell / upgrade / transfer / pickup | Ownership, bag, level, cost, RNG | Inventory / stat snapshot |
| Quest / class / attributes | Prerequisite, điểm còn lại, one-time reward | Progression / summary |
| Portal / chat / PvP invite | Map / range / rate / match state | Transition / bubble / banner / match result |

P0 localhost / LAN / Direct IP cùng mạng; P1 Relay / VPN / tunnel. Không claim Internet-ready hay anti-cheat production. Validate request ở Host kể cả player chạy trên Host; client owner không được trực tiếp ghi HP / gold / profile. Co-op formula / quest participation / shared loot policy đã chốt tại GDD §5–6; implementation giữ ba predicates riêng, mọi N recipients snapshot per character. Analysis contribution examples giữ ví dụ chống powerlevel, không một policy authority khác.

PvP match state giữ MatchId + đúng hai participantIds vì mode 1v1; damage / result / invite kiểm membership + match phase bên cạnh MapId, người ngoài không hit hoặc nhận result của match. World registry vẫn N players; concurrent arena matches tách bằng MatchId. Bắt đầu trận ghi **cùng một giao dịch** hai bản chụp HP/MP/Food/vị trí trước trận và trạng thái active trước khi dịch chuyển; kết quả, 200 Journey và restore cũng commit một lần theo MatchId. Boot gặp active thiếu result thì hủy/restore hai người, không thưởng; timeout/hòa/ngắt kết nối theo GDD §8, không tự thêm matchmaking.

ChatService validate count / rate theo [GDD §8](1_HUYEN_LO_GDD.md#gdd-9), route MapId; global banner tách khỏi map chat, tên Boss lấy Q12 từng recipient. PvPService đọc restrictions/match rules và raw-damage scalar từ GDD §8; DamageResolver chỉ apply scalar sau xác thực đúng MatchId/phase/membership, trước DEF/rounding, không dùng cho world PvE; MatchId receipt cho kết quả/200 Journey, timeout/hòa/ngắt kết nối/restore là ca nghiệm thu; không trả thưởng lần hai sau reconnect.

<a id="profile-authority"></a>

# 5. Character / Profile Authority

Host repository load bằng profileId / characterId. Client không upload level, gear, gold, EXP hay attributes như trusted state. Connection → profile → character binding là TECH-01: spike phải xác định ID tồn tại, quyền chọn, reconnect và duplicate-selection policy; không coi biết ID là authentication Internet.

Binding / player spawn dùng collection theo connectionId / playerId, cùng một prefab / config cho mọi approved connection; Host player cũng đi qua validation tương đương. ClientId không là index vào hai slot / save fixtures; reconnect cập nhật binding, invalidate stale callbacks của connection cũ (TECH-01). Gameplay threat / contribution dùng playerId ổn định gắn characterId, không key bằng transient ClientId; reconnect thay connection binding, không tạo thêm ledger entry cho cùng character trong một life. Một active writer cho mỗi character; không cho hai connections cộng điểm / claim loot trên cùng save đồng thời. Runtime state và profile mutation cùng Host; kết quả transactions xác nhận sau khi cập nhật authoritative state. Disconnect gỡ player khỏi alive targets, chat và arena; xử lý contribution / loot theo connected-at-death rule GDD, không giữ eligibility bằng stale connection.

Invariant điểm: spent + unspent = 5 × (L−1); Lv 1 = 0; Reset nhập môn khi đạt Lv 5 tạo 20 unspent một lần; Q6 nhận muộn không set pool về 20, level-up sau Lv 5 vẫn cộng 5 nên total = 5 × (L−1). Không nhận thêm 20 mỗi load / class selection. Normal Tân Lữ còn dùng trước chọn class kể cả Lv 5+. Demo fixtures tại §12 không phải nguồn ghi đè profile thật.

<a id="persistence"></a>

# 6. Persistence

**P0 một HostSaveEnvelope JSON** chứa profile index / character records và world records trong cùng atomic file; characterId lookup là in-memory dictionary, không mỗi character một file / transaction coordinator. Demo JSON fixtures chỉ import / reset có chủ đích, không authority thứ hai. MariaDB P1 tách repository khi thật cần.

| Persist trong DTO | Runtime / derive, không save |
| --- | --- |
| Envelope schemaVersion / revision; profileId / characterId / name và danh sách character thuộc profile | Connection / clientId binding và quyền session |
| Level / EXP / class, spent / unspent attributes, currency, bag / storage / equipment với stable instanceIds | Final stats, passive unlock, Unity object references |
| MapId / position và safe return destination; HP / MP / isDead; Food và bình còn hiệu lực, deadline hồi chiêu bình/kỹ năng | Chỉ số cuối derive; không hồi hoặc hồi sinh trong thời gian Host tắt |
| Per-QuestId progress, virtual evidence / ordinals, story / unlock / summary flags; Journey score và one-time reward receipts; learned profile IDs; PvP MatchId / bản chụp trước trận / kết quả receipt khi còn active | Threat / contribution / action dedup giữa trận, short combat statuses |
| Boss life / dead deadline; deathId / rolled payload / recipient receipts; ground items / expiry và claim receipts; trạng thái/generation/deadline từng normal SpawnSlot | Mid-fight Boss HP / phase / status; AI/threat normal; boot Alive full HP / clear encounter |
| Tutorial grant / entitlement receipts, selected tutorial item instance, committed Q8 step; pending immutable command nếu có | Q8 waiting set derive từ present characters; live AI không restore object references |

DTO plain serializable fields / lists, stable IDs không Unity references; [JsonUtility](https://docs.unity3d.com/6000.3/Documentation/Manual/json-serialization.html) không serialize dictionaries nên rebuild unique maps sau validate. Check schema / definition IDs, spent + unspent, cap EXP, enhancement trong trần bậc I 0–4 / II 0–6 / III 0–8, item-instance uniqueness bag / storage / equipment, learned profile prerequisites / class / level và manual receipts; không coi missing fields là valid new character. Tải save giữ HP/MP/dead, MapId và vị trí hợp lệ; nếu tọa độ bị invalid do đổi map data thì dùng safe return destination đã lưu và giữ HP/dead. Food, bình và hồi chiêu dùng deadline UTC: thời gian offline vẫn trôi, hết hạn thì tắt; không tick hồi phục Food khi chết/offline. Ngoại lệ Food đang tạm dừng trong PvP: giữ số giây còn lại đã chụp, đặt deadline mới khi restore; hồi chiêu kỹ năng/bình ngoài trận vẫn trôi theo UTC. SpawnSlot lưu generation/variant: trạng thái Alive sau restart trở lại đầy HP **cùng variant, không reroll**; trạng thái Dead theo deadline, quá hạn hồi đúng một lần và roll variant đúng một lần. Không restore AI/threat/HP giữa trận. Boss Alive khởi động lại đầy HP theo lifecycle dưới đây. Match PvP active không có result receipt khi boot thì hủy, khôi phục hai người từ bản chụp trước trận ở Vân Khê, không thưởng; result đã commit chỉ replay receipt, không cấp lại. Không auto-heal, revive hoặc nhân loot/quest khi load.

**Serialized single-writer / save queue:** một FIFO command queue do Host sở hữu cho mọi critical mutation, autosave và disconnect save. P0 có thể drain đồng bộ; không cần worker / background framework. Mỗi command có commandId, expectedRevision và immutable payload; lấy latest committed revision khi tới lượt, validate lại rồi mới plan. Không để hai working copies cùng revision lần lượt overwrite kết quả của nhau. Command pending write / retry giữ thứ tự; chưa commit thì không publish success hoặc xử lý command phụ thuộc state đó.

**Command transaction:** validate + plan trên working copy → snapshot resulting envelope với revision = committedRevision + 1 → ghi temp cùng filesystem → flush durable → atomic replace + last-valid backup → adopt committed revision / apply / publish. Claim inventory + ground removal, learn consume + flag, turn-in reward + Completed, enhance fail cost, transfer source/cost/target, Boss death + deadline + pile + N-recipient credit đều cùng snapshot. Write fail giữ last-valid state và pending payload / ID; retry không roll lại. Sau crash chỉ công nhận snapshot đã validate; request lặp đọc receipt trong snapshot đó rồi trả kết quả đã commit.

**Atomic replace chống file ghi dở; receipts chống nhân reward.** Bắt buộc deathID, claim receipt `(itemInstanceId,claimId)`, quest grant / turn-in receipt `(characterId,QuestId,grantId)` và commandId / revision cho học / nâng / chuyển giao / bán; transfer receipt giữ source/target/result dù source consumed. Không xóa receipts chỉ vì dùng một JSON. Receipt cleanup chỉ khi event / item đã terminal và request cũ không thể được nhận lại qua revision / session validation; không giữ log mọi combat packet.

Implementation spike kiểm `FileStream.Flush(true)` và atomic replace / rename trên filesystem đích; first-save chưa có destination là nhánh riêng. Giữ corrupt artifacts để điều tra; validate main / backup, recover hoặc báo lỗi, không overwrite bằng new world. Benchmark bytes / snapshot, queue depth, write latency p50 / p95 và main-thread stall ở 2 / 3 / 4+ players, có fixture history lớn. Chỉ chọn immutable background snapshot nếu kết quả đo cần; vẫn giữ một writer và revision order.

Autosave 120 s; critical commands: class / reset, learn manual, quest / evidence / reward, enhancement success / fail, upgrade transfer, loot claim, GoldBar sell, PvP start/result/restore, death dispatch, logout / disconnect / summary. Bounded receipts và one-save atomicity thay distributed transaction protocol; không event sourcing / WAL / db / async layering bắt buộc. Queue drain / disconnect / shutdown phải chờ pending durable command hoặc báo failure rõ; không ghi snapshot cũ đè revision mới. Capacity / performance của save toàn envelope cần benchmark N players; không claim production auth hoặc unlimited scale.

<a id="timers"></a>

# 7. Timers & Inactive Maps

| Timer | Authority | Empty map / re-entry |
| --- | --- | --- |
| Normal / Linh Biến slot respawnAtUtc | Global SpawnManager | Brain pause; nếu now ≥ deadline thì restore đúng một lần |
| Ground loot despawnAtUtc | LootManager | Expire dù root không active; lifetime theo GDD §6, không phụ thuộc active root |
| Boss nextSpawnUtc | Global WorldBossManager | Countdown / banner không phụ thuộc player ở Huyền Tích |
| Skill / Food / Potion | Host runtime clock + UTC deadline đã lưu cho hồi chiêu kỹ năng, Food và bình | Local hit-stop không pause; offline không hồi Food, thời hạn vẫn trôi |

Dùng UTC deadlines cho lifecycle cần tồn tại qua inactive maps; runtime combat dùng clock nhất quán, không trust client timestamp. Re-entry resolve overdue timer idempotently trước gửi snapshot. Tránh catch-up sinh nhiều Boss hay pile-up cả chuỗi respawn khi map lâu rỗng. Normal / Linh restart theo deadline đã lưu; chỉ recover committed drops / receipts, không restore AI / threat; Q8 arbitration derive sau world boot.

WorldBossManager cập nhật threat / contribution từ actual-loss direct / DoT (cap overkill, dedup), target highest alive valid BossCombatArea; clear ledger / status / phase khi wipe reset 10 s. Cuồng Mạch chọn base future cadence từ GDD; Băng Hàn chỉ nhân tốc đếm `remainingActionWait` 0,75 khi active, không reset wait hay đổi pending hit / telegraph. Ba vùng không double-hit; scheduler một action, thứ tự ưu tiên và hồi chiêu theo GDD §4. Logical region không gate scene / level / quest; actual MapId / spatial checks vẫn bắt buộc.

**Boss boot / lifecycle:** explicit new world hoặc persisted Alive → tạo đúng một Boss nếu registry chưa có; restart Alive bắt đầu full HP / clear transient encounter, không restore stale threat. Persisted Dead với future nextSpawnUtc → chờ deadline; overdue → spawn once, không catch-up nhiều lifetimes. World save thiếu / hỏng phải recover / báo lỗi, không coi corrupt file là new world để farm reset timer. Accept / Ready / Completed Q12 không đổi lifecycle. Death commit Dead + nextSpawnUtc + deathID+**một rolled shared pile**+recipient credit trong envelope trước publish; no EXP / direct Gold, item Thỏi Vàng chỉ tạo currency qua Sell transaction. Gear / pile / windows / threshold 10% derive GDD (demo threshold derive runtimeHP). Shared loot recovery giữ original deadlines; Q12 / Journey credit theo qualifying snapshot, không replay sau boot. Banner eventID chống lặp tick.

<a id="art-contract"></a>

# 8. Art / Animation Technical Contract

| Hạng mục | Contract | Kiểm import |
| --- | --- | --- |
| Male rig | 64 × 64 px, PPU 32, body 44–48 px, pivot Bottom-Center | Point filter, không texture blur; căn chân |
| State frames | Idle 4 / Run 6 / Jump 2 / Fall 2 / Attack 3 / Skill 4 / Hit 2 / Death 3 = 26 | Parts cùng state / index, đúng tổng frame đã khóa |
| Modular parts | BodyBase / HairHead / Armor / Pants / Weapon; ba gear visual families,12 modules / 21 regular item icons | Weapon Front / Back theo pose; không slot phụ vô cớ |
| Facing | Vẽ hướng phải, flipX trái | Sockets / attack origins flip đồng bộ |
| Collider | ~0,60–0,65 u × 1,45 u, TUNABLE | Không lấy kích thước toàn canvas |
| Mob / Linh Biến | 6 base sets + palette Sói Trúc (7 fixed-level identities), một shared modifier scale / aura / tint / name / HP bar | Linh reuse sprite identity / AI / projectile; aura độc lập wolf palette, không dedicated Linh sprite set |
| Background | Forest / Mountain / Ancient +hub props reuse | Telegraph, loot, chat không bị foreground che |

**Character asset pipeline:** source sheets → canvas64×64/PPU 32/Point filter → slice từng state/frame → pivot Bottom-Center đồng nhất → gán male BodyBase/HairHead/Pants/Armor/Weapon → shared state/frame controller → gear family palette/accent → SortingGroup/sockets → overlays. Không để Animator mỗi part chạy clock riêng. Hurtbox/movement collider nằm dưới physics root, không VisualRoot; đổi gear/flip/scale không tự scale collider. Attack origins author riêng theo timeline, không lấy bounds ảnh làm hitbox.

**Map asset pipeline:**

| Bước | Import / authoring và kiểm |
| --- | --- |
| Environment / tileset | Forest cho Đồng / Trúc, Mountain cho Bạch / Xích, Ancient cho Huyền; hub reuse props. Tiles / cell size derive asset pixels / PPU, không lấy64px rig làm tile size. |
| Background / terrain / platform | Background không collider; Ground static composite; Platform tách one-way workflow §2. Test nhảy / drop nhiều actors trước decorate. |
| Back props / landmark / foreground | Landmark đặt theo region IDs quest / Boss, không cản đường đọc. Foreground chỉ mỹ thuật, không che telegraph / loot / nameplate / chat; không lighting / streaming subsystem P0. |
| Anchors | Đặt từng fixed identity slot theo manifest, NPC / portal / quest region stable IDs; validate MapId / bounds / safe entrance / Boss-spawn exclusion. |
| Camera / readability | Bounds theo root, Cinemachine local follow / confiner / impulse; kiểm hai hướng, cao độ, tint / contrast và palette sói vs aura Linh. Gameplay layout Đồng sparse, Trúc vertical, Bạch terraces, Xích canyon, Huyền ruin tương ứng GDD §4 / §9. |

Feedback renderer nhận event Host; damage / hit / status dùng combat timeline, không trust client AnimationEvent. Local hit-stop / flash / trails không pause simulation hoặc thêm stun. Content feedback thuộc GDD §9.

| Presentation contract | Quy tắc |
| --- | --- |
| Actor parts | Một SortingGroup; Shadow 0 → WeaponBack 5 → BodyBase 10 → Pants 14 → HairHead 20 → Armor 22 → WeaponFront 30 → CombatFX 40. Shared state / frame controller; VisualRoot / sockets flip đồng bộ. Kiểm overlap ở cả hai hướng và nhiều actors. |
| World sorting | Background → terrain / back props → actors → attack / telegraph → readable feedback / worldUI. Foreground không che telegraph / loot / chat; palette / accent theo ba families GDD, không thêm slot / frame set. |
| Pooled VFX | Một pool / config chung cho projectile / telegraph / Bỏng, Đóng Băng, Boss/PvP Làm Chậm presentation và hit / damage / NÉ / heal / upgrade / death / slash feedback. Reuse reset tint / timer / owner / action; release hủy timers / listeners. |
| Lifetime | Authoritative projectile / action có instanceID / generation riêng; callbacks reject instance đã despawn / reuse, không dùng GameObject reference làm lifetime ID. ObjectPool chỉ quản lý presentation. |
| Hit / death | Hit state không tự stun; Đóng Băng dùng overlay băng rõ trên normal/Linh Biến; Boss/PvP Làm Chậm dùng phủ lam mờ/hạt lạnh, không hard-ice. Bỏng dùng tia lửa gọn; overlay không đổi collider/hitbox. Death animation / tint giữ corpse cho camera / Boss quest / contributor snapshot; không cho corpse pickup. |

Asset gate: làm một full rig 26 frames, một Sword / Bow / Armor / Pants family và một farm-room trước sản xuất đủ ba gear bands. Kiểm outline / palette / pivot / frame sync khi chuyển gear. AI-generated bitmap nếu dùng vẫn cần slice / clean / import / QA; chưa tạo asset trong vòng docs này.

<a id="ui-notes"></a>

# 9. UI Technical Notes

Một router / modal stack cho NPC / inventory / character / quest / PvP. InputActionAsset giữ Gameplay / UI contexts; khi chat / modal active tắt gameplay actions và clear buffered movement / attack. S + Space resolver ưu tiên DropThrough, không cùng frame emit Jump. Chỉ owner local gắn input / camera / HUD; PlayerInputManager cho local couch join không cần cho world online. NPC services và bindings thuộc [GDD §9](1_HUYEN_LO_GDD.md#gdd-10).

**QuickConsumableAction:** bindings xem GDD §9; Host validates inventory / level / count / cooldown / state, chọn item theo GDD và consume một lần; bước M tutorial Q6 ưu tiên reserved Potion I, không để lựa chọn bình thường tiêu món khác mà bỏ kẹt objective. UI hiển thị reject reason; không copy hotkey table hoặc tutorial content. Quest HUD dùng state snapshot, không chuyển Available bằng client-side objective flag.


| View | Nội dung / validation |
| --- | --- |
| HUD | HP / MP / EXP, skill CD, Food / Potion, quest, Boss timer; server state |
| Inventory / shop | Capacity / stack rules từ GDD §6; Host transaction trước refresh view |
| Upgrade / transfer | Preview dùng cùng stat evaluator với runtime; transfer hiện source consumed, target trước / sau, cost / Tinh Hoa; hiển thị rõ số của Tinh Hoa I/II tại +4/+8 và trạng thái khóa/mở; thất bại vẫn lưu chi phí |
| Skill panel | Hai active / shared evolution slot, hai nội tại/class; tên/icon/tooltip/khóa Lv 5/13; Kiếm Tâm +HP/DEF, Ưng Nhãn +MP/ACC hiển thị ở stat panel; auto-open từ Host class+level, không point/rank/hotkey |
| Quest / chapter | State / nextLevel / turnInNpcId từ Host; Ready cho tiếp tục chơi; Completed summary theo GDD; Q12 dùng Main Story Complete |
| WorldUI | HitResult / status / virtual evidence recipient; two-state nameplate theo abs level gap / quest marker độc lập; loot phase prompt; chat GDD §8, Boss name per character |
| Death / PvP | Death choices khác PvPDefeated; consumable restrictions đúng mode |

Bag Sort / protection gear / quest arrows / history P1; P0 validation không phụ thuộc QoL. Tooltip làm tròn để đọc nhưng internal fractional enhance không mất gain. Không hiện microservice / RPC / saveAPI trong player flow.

<a id="roadmap"></a>

# 10. Technical Spike & Roadmap

**Gate đầu: spike ba ngày** — Host + ít nhất 1 Client với profiles độc lập (P0 acceptance: tối thiểu 2 concurrent players); move / jump / drop-through; portal chuyển MapId; client khác map không hit / loot / chat được; Host load character ID; basic persistence crash experiment. MPPM hỗ trợ iterate; chạy thêm 3–4 concurrent players để bắt fixed-pair assumptions, không công bố capacity chỉ từ ca pass. Observer optimization không chặn gate này. Đo thêm thời gian transfer preview/transaction/crash QA, bốn passives và palette sói trong workload; không tự coi reuse là zero giờ. Nếu movement correction / profile / save fail, xử lý trước mở content production.

| Tuần | Milestone | Gate |
| --- | --- | --- |
| 1 | Spike / runtime / movement / profile | Tối thiểu 2 concurrent players, MapId, định nghĩa data |
| 2 | Combat slice / rig pipeline | Normal, Lv 5 single +Lv 10 evolution fixtures, target fallback, separation / melee miss / projectile, một modular family |
| 3 | Progression / items / consumables | EXP / attributes, class reset, Food / Potion / Death, inventory / loot channels + sellValue; contribution / factor / shared-window predicates đã rõ |
| 4 | Gear / shop / quest framework | Enhance / chuyển giao sáu slots, safe transactions, Q1–Q8 / manual grants / forced variant, recipient policy / quest assist đã rõ, scope checkpoint |
| 5 | Cụm quái / remaining skills | Năm farm roots / density matrix TEST, bảy fixed identities / sáu rigs / Linh Biến cap1, hai active +hai nội tại / class / evolution / manuals |
| 6 | Boss / co-op / persistence | Shared pile / claim / reset / Cuồng Mạch, Q11 / Q12; shared pile / death dispatch đã rõ; kiểm lịch Boss đã chốt trên scene |
| 7 | PvP / chat / story / demo | Q9 optional, match 120 s, banners, summaries, profiles |
| 8 | Integration / QA / package | Regression, evidence tối thiểu 2 concurrent players, build / scripts / video fallback |

Working target 160–200 h, risk envelope 160–240 h, tám tuần; không phải estimate đã chứng minh. Chưa có estimate tin cậy cho 12 visual modules / 21 regular icons / manual motifs: đo slice trước, dùng accounting theo catalog mới. Không giảm sáu base rigs / hai class / 26 frames user-lock để ép schedule. AI giảm boilerplate code / docs, không bỏ Editor / prefab / animator / tilemap / slicing / UI / network debugging / playtest. Milestone trượt thì cập nhật giờ và scope P1 minh bạch.

| Workload cần đo tại slice | Unit / reuse | Evidence phải ghi |
| --- | --- | --- |
| Character rig | Một male rig, 26 frames; chung pivots / frame controller | Giờ clean / slice / import, lỗi flip / socket / pose |
| Gear visuals | 12 modules = 3 bands × Sword / Bow / Armor / Pants | Giờ trên module đầu, phần reuse so redraw; không nhân 21 templates thành rigs |
| Icons / data | 21 regular gear + Mộc Kiếm; 6 manuals dùng 2 motifs × 3 accents; consumable / material riêng | Template count, icon mới / reuse (thêm bốn passive icons từ hai motifs), validation bindings / stat evaluator |
| World | 5 farm rooms + 3 support roots; pocket / slot budget theo GDD | Giờ placement / route / aggro / vertical / portal QA trên room đầu rồi extrapolate |
| Mobs / Boss | 6 base sets +1 wolf palette, 7 identities, shared Linh modifier, 1 Boss | AI / config reuse, telegraph / collision / scheduler integration |
| UI / network / save | HUD / modal, ownership claims, profile binding, save queue | Editor / prefab hours và latency / crash / standalone test, không chỉ script hours |

**Contingency / cut ladder:** review sau Art Vertical Slice, cuối Week 2 và Week 4 scope checkpoint, bằng actual spent / remaining hours. Cắt P1 / P2 trước; sau đó giảm cosmetic polish / additional sound / VFX variations, elaborate Journey summary presentation (giữ scores / summary core), Storage presentation depth (giữ 40 slots / basic deposit-withdraw), extra UI polish / QoL. Không tự cut hai classes, sáu base rigs/bảy identities, 26-frame rig, Lv 1 → 20 hoặc multiplayer core; không demote PvP / Q9 / MapChat nếu chưa user approve. Nếu vẫn vượt 160–240 h, đổi schedule hoặc trình concrete scope tradeoff để user quyết, không gọi nghiệm thu phần thiếu là done.

Linh Biến tái dùng normal sprites nhưng vẫn cần shared aura và integration / QA; không suy ra giảm giờ vẽ hai bộ Elite riêng vì baseline vốn không có các bộ đó. Đo full rig / family / farm-room ở Week 2 và actual placement / route / QA cho layout mới trước cam kết budget; bỏ anchors / timers cũ không chứng minh tổng giờ giảm.

<a id="risks"></a>

# 11. Risk Register

| Risk | Mức | Evidence / gate | Xử lý |
| --- | --- | --- | --- |
| MP spam / INT value | CRITICAL | Analysis sustain: Food MP retune, zero-INT Potion dependency | Chốt dependency, test rotation thực |
| Contribution / loot race | HIGH | Contribution cases, COOP-01 closed | Snapshot trước reward, no redistribution / fallback; serialized claim và crash recovery |
| Profile binding / save corruption | HIGH | TECH-01 / restore baseline §6 | Spike ID binding, crash recovery, một writer |
| Prediction / latency | HIGH | Chưa playable | Đo movement correction với tối thiểu 2 concurrent players |
| Art / editor hours | HIGH | Module / import time chưa đo | Một full family làm gate, reuse |
| Boss fairness / TTK | HIGH | Boss sensitivity / scheduler baseline | Đo thời gian ra đòn hữu hiệu, vùng báo trước, mục tiêu và reset |
| Quest onboarding / recovery | HIGH | QUEST-02 closed | Virtual deterministic evidence, staged supply và solo / late-quest / Q9 regression |
| Physics / target readability | HIGH | PHY-01 / ART-01 | Test airborne / platform / target sau lưng |
| Observer performance | MEDIUM | P0 còn replicate | Profile rồi optimize P1; MapId correctness trước |
| World farm / economy | HIGH | world density model | Measure 2 / 3 / 4-player starvation / flow; tune rate / reward / layout, không cut mechanic từ toy model |
| P1 scope creep | HIGH | Nhiều audit proposals | Chọn sau gate P0; research không là DoD |

<a id="qa"></a>

# 12. Demo / QA / Acceptance Tests

P0 acceptance: **tối thiểu 2 concurrent players**, Host + ít nhất 1 Client; profile demo Host / Client là fixtures, không giới hạn registry / spawn. QA mở rộng 3–4+ phải ghi machine / build / package versions, CPU / frame time, bytes / messages và latency trước capacity claim. Không cần Party / Channel / dedicated server.

EditMode: EXP / attributes / equipment evaluator, inventory preflight + idempotency, per-ID quest DTO round-trip, corrupted / partial save và write failure. PlayMode: multi-actor platform drop-through, portal / camera / MapId, authoritative timeline, pool reuse. Multiplayer: MPPM khi dev + standalone Host / Client acceptance; Network Simulator kiểm delay / jitter / loss / reconnect, forged sender binding / retry không nhân reward. Các tests giữ domain invariants, không chỉ mirror implementation; manual build evidence vẫn cần cho game feel.

**Tất cả ca dưới đây: CHƯA CHẠY.** Đây là checklist nghiệm thu tương lai; vòng hiện tại chỉ chạy doc validation và phép tính. Không đánh dấu playable / done bằng mô phỏng.

| Profile | State cố định | Demo |
| --- | --- | --- |
| demo_new | Lv 1, Novice; kiếm chỉ cấp ở Q3 | Movement / Q1–Q4 |
| demo_class | Lv 5, Q5 Completed / Q6 Available, 20 unspent, chưa class | Chọn class / reset / equip / skill Lv 5 + nội tại nền tảng, nội tại tinh thông khóa Lv 13 |
| demo_mid | Lv 10, class / Family I gear + 0, Q8 evolution manual chưa học, Q1–Q8 Completed | Học tiến cảnh → farm group / gear / Food / Linh Biến; nhặt/mua Band II trước Lv 11 chỉ giữ trong bag |
| demo_end_host | Lv 20 Kiếm cân bằng, đã học nhập môn / tiến cảnh / đại chiêu; Rare III vũ khí +6, Áo/Quần Common III +0, ba ô còn lại II +0, character riêng | Boss / PvP Host, không giả định full +8 |
| demo_end_client | Lv 20 Cung cân bằng, cùng mức học/trang bị với Host và characterId riêng | Boss / PvP Client, so hai người chính tuyến |

Demo mặc định DebugExpMultiplier = 1, switch profiles từng phase; × 10 chỉ dev / debug opt-in có label, không nghiệm thu pacing. BossHP × 0,40 / BossRespawn 60 s là demo override rõ, Boss có sẵn ngay; release không dùng nhầm. DemoBossMaxHP 12.800 và contribution 1.280 cùng derived value, không chỉ UI. Quay class selection bằng demo_class, không demo_mid.

| Bộ nghiệm thu | Ca bắt buộc | Evidence |
| --- | --- | --- |
| Progression | Cumulative 53.100, carry EXP, catch-up remaining, reset Lv 5 một lần, trì hoãn Q6 tới Lv 6+ giữ total points / normal Mộc Kiếm và M reserved potion, 95 điểm, không branch cap / rank | State trước / sau và reload |
| Combat / status | BaseHP/MP theo cấp; nội tại Kiếm Tâm/Ưng Nhãn nhân final stat một lần, Kiếm Thế/Xạ Tâm +12% direct skill đúng cự ly; nhịp thường Tân Lữ 1,00 / Kiếm 0,80 / Cung 0,90 s, Lv 5 active vẫn single, Lv 10 mới đánh lan, Lv 17 big; A/B/C fallback/no reacquire, per-unique-target status roll cached kể cả fail; evolution giữ CD; Bỏng 4%/70%, 6 s/1 s/6% ATK, refresh source + expiry nhưng không reset tick; Freeze 1,5 s Normal/Linh + miễn 3 s, test 1/2/4 Cung; Boss Slow 3 s/75% action wait **không reset countdown/action đã start**; PvP Slow 25% move 1,5 s, không attack/CD slow; không nhân status khi nhiều caster | Host clock / stat evaluator / status logs, không AnimationEvent |
| CC / AI | Sticky per-mob threat 1,25 ×, group wake no copied threat; dead / disconnect / portal / leash retarget, Return clears status / ledger; front / vertical miss, no contact / shoving, projectile snapshot, Ong melee-accessible | N-player target / status tests |
| World / timers | 2 / 3 / 4 players same-map farm, no whole-map starvation, safe portal strip, no chain aggro cả map, traversal / run-back logs; N-player root occupancy; drop-through một actor không ảnh hưởng actor khác; density theo GDD matrix TEST / TUNABLE, Return reset HP, deadlines map rỗng / re-entry, respawn idempotent | Clock / MapId logs |
| Boss | New world/Alive boot đúng một con; HP 32.000, ATK 160, DEF 25, ACC 140, EVA 60; chết hồi sau 15 phút / demo 60 s, không spawn khi nhận Q12; một action theo thứ tự GDD, ba vùng đá không double-hit; Cuồng Mạch chỉ đổi nhịp về sau, Làm Chậm chỉ giảm tốc phần chờ còn lại; ngưỡng 10% = 3.200/demo 1.280, corpse đủ điều kiện, không EXP/Vàng trực tiếp, một pile/Thỏi/cửa nhặt 12–30–90 s | Lifecycle/clock/credit/claim logs, 2/4 người |
| RPG — thưởng/nhặt | Mỗi người nhận phần EXP/Vàng theo đóng góp, làm tròn xuống, không chia lại; lệch cấp ≤3 nhận đủ, ≥4 không có thưởng farm. TopDamage không đủ level thì không roll set thường, không fallback. Shared pickup 8/20/60 s; một item chỉ một claim. Level tăng từ kill không tước quyền nhặt đã chụp; người tới sau dùng level hiện tại. | Race claim, túi đầy, deadline và crash logs |
| RPG — catalog | 18 dòng / 21 mẫu thường có tên riêng; Kiếm thêm Chí mạng 0,5/1/1,5 điểm %, Cung thêm ACC 10/20/30, Giày thêm tốc chạy 1/2/3%. Quái Lv 2/4 chỉ rơi năm ô không vũ khí ở mọi map. Mặc I không vũ khí Lv 1, vũ khí I Lv 5, II Lv 11, III Lv 17. Ong Lv 10 rơi II nhưng chưa mặc/chuyển vào đích II trước Lv 11. Dây chuyền III bán 225, Thỏi bán 250; cường hóa không tăng giá bán. | 21 templateID/tên duy nhất; sai phái không mặc/chuyển; Chí mạng/ACC/tốc chạy tại +0/+4/+8, preview/load khớp; nguồn rơi, shop và cấp mặc |
| RPG — cường hóa/chuyển giao | Trần I+4/II+6/III+8; Tinh Hoa I +4, II +8; thất bại giữ cấp nhưng tiêu chi phí. Chuyển cùng bậc 800 Vàng hoặc lên đúng bậc kế 500 Vàng +2 đá; cùng ô/đúng loại vũ khí, đủ cấp, hai instance khác nhau trong túi. Từ chối nếu đích không tăng; nguồn bị tiêu, đích giữ ID/phẩm chất; receipt replay không trừ/cấp lại. | +0→+8, vượt trần, khác phái, no-gain, hai lệnh tranh nguồn, save failure |
| Linh Biến / economy | Chance 5% only Lv 8+, HP × 5 / EXP-Gold × 3, cap 1 under concurrent respawn, fixed mobIdentity / fixedLevel validate slot cache, respawn25 s; Q8 reservation waits live variant / no demote, same Linh rewards, no force replay; Stone / gear / Gold-hour and crowd contention | Seed / slot / generation logs and measured throughput |
| Quest | Một bảng Q1–Q12 GDD là authority; Q4 5 Sói DS3–DS6, Q8 4 Sói TA4+TA6 rồi force `TA4.slot1`, Q10 6 Đoạt XN1–XN3/evidence #2/#4/#6, Q11 3/3/4 Thạch theo XN4/5/6, Q12 6 Cổ HT4+HT5; active virtual evidence, no pre-farm/overcount; damage trước accept/sai step không hồi tố, late quest không bị level penalty softlock; Q5–Q7 supply/manual/full bag/replay, Q9 optional, Q11 activation ngoài portal | Solo/late/online/reconnect/full-bag journey |
| PvP / chat | Mời/nhận, lệch cấp, 120 s so tỷ lệ HP, hòa/ngắt kết nối, không dùng bình, Food tạm dừng; hệ số sát thương PvP 0,20 và các build khác nhau; Làm Chậm ×0,75 tốc chạy trong 1,5 s, không đổi action/CD; không áp hệ số sang PvE. Chat theo MapId, 80 ký tự và giới hạn gửi. | Host + Client; snapshot trước trận, crash/restore và thưởng MatchId một lần |
| Save / network | N-player registry / recipient sets; Q9 + Q10 DTO round-trip; FIFO queue / revision ordering, hai claims / enhance / transfer / disconnect / auto-save tranh lượt; deathID / claim / quest grant receipts vẫn tồn tại qua crash, replay sau commit không nhân reward; write failure không ack durable success; reject forged state / replay, ID binding, disconnect save, crash từng bước, backup recovery | Corrupt / crash artifacts và log |
| Art / UI | 64 × 64 / PPU 32 / 26 frames, pivot/frame alignment, gear flip, map anchors; phân biệt Bỏng/tia lửa, quái Đóng Băng/băng vỡ, Boss/PvP Làm Chậm/phủ lam mờ; input context | Import audit / video |

Chuẩn bị build đóng gói, README chạy demo trong bộ phân phối khi implementation tới gate, script mở Host / Client, reset demo profiles có chủ đích và video fallback. Không tạo thêm Markdown trong active design tree ở vòng này. Chọn rubric / trình tự trình diễn theo feature đã chạy; video fallback không thay evidence acceptance.
