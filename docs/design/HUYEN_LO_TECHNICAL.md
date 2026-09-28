# Huyền Lộ — Hợp đồng kỹ thuật

**Đối chiếu:** GDD V5.1.2 · **Ngày:** 2026-09-29 · **Trạng thái:** specification trước triển khai.

[GDD](HUYEN_LO_GDD.md) quyết định gameplay; [Analysis](HUYEN_LO_DESIGN_ANALYSIS.md#open-decisions) giữ các gap phải chốt. Các tên component/API dưới đây là hướng triển khai, cần spike với phiên bản Unity/package thực dùng; chưa có Unity build hay runtime test trong vòng dọn tài liệu này.

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

<a id="maps"></a>

# 2. Unity Scene / Map Architecture

Boot → ProfileSelect → CharacterSelect → WorldOnline. Một world scene có **8 logical map roots**: Village, Academy, Arena và năm farm maps. Baseline đặt roots cách nhau khoảng 200u để tách physics; không phải cam kết chiều dài mỗi map. MapDefinition giữ MapId, spawn point, portal destination, bounds, SpawnGroups và environment family.

| Hợp đồng | P0 | P1 |
| --- | --- | --- |
| Map identity | Host giữ MapId; kiểm combat/chat/loot/portal | Không thay bằng scene visibility |
| Presentation | Camera và render filter chỉ root hiện hành | Streaming nếu profiler cho thấy cần |
| Network observers | Có thể còn replicate khác map; client không render | NetworkHide/NetworkShow sau correctness |
| Portal | Host kiểm nguồn, range, state, destination gate; đổi MapId+position | Loading polish |
| Empty root | AI pause; timer vẫn thuộc global manager | Interest optimization |

MapDefinition đọc requiredLevel/unlockFlag từ [GDD — map gates](HUYEN_LO_GDD.md#gdd-5); Host kiểm cờ **Completed**, không objective counter hay ReadyToTurnIn. Hai player có thể ở khác map. Portal từ chối không làm mất player hoặc tạo duplicate; chuyển map cập nhật target/threat/interaction và gửi snapshot root mới. Chỉ render filter không đủ bảo vệ gameplay.

<a id="combat-data"></a>

# 3. Combat Data Architecture

| Definition/state | Trường cần có | Validation |
| --- | --- | --- |
| SkillDefinition | ID, level/class, shape, range, power list, maxTargets, MP, CD, hit timings, status | Power theo GDD; không thêm SkillRank |
| CombatRequest | character binding, actionID, sequence, facing/aim intent | Host kiểm alive, map, cooldown, MP, range và target |
| HitResult | actionID, targetID, hitIndex, evade/crit, damage, remainingHP | Client chỉ hiển thị kết quả Host |
| MobDefinition | 6 archetype IDs, formula, AI type, range/interval, generic projectile config | Elite dùng baseDefinitionId+multiplier |
| SpawnGroup | mapID, groupID, slots, spawn positions, leash, absolute respawn | 18 groups; không respawn từ brain tick |
| ItemDefinition/Instance | templateID, instanceID, rarity, enhancement, count | Không sinh stat mới; instanceID duy nhất |
| QuestDefinition | prerequisiteQuestIds, requiredLevel, objectives, turnInNpcId, rewards, unlockFlags | Authoring theo [GDD §6](HUYEN_LO_GDD.md#gdd-6), không copy quest table |
| QuestState | state, objectiveCounters, claimedFlag; stagedGrantFlags cho tutorial | Host-owned; idempotent claim và save |

**QuestState model:** Locked → Available → InProgress → ReadyToTurnIn → Completed. EligibilityService kiểm đủ prerequisite quest IDs **và** requiredLevel; branch optional chỉ theo prerequisite graph GDD, không implicit Qn−1. Thiếu level trả Locked+reason/nextLevel cho NPC/HUD, không marker Available.

QuestService validate progress từ event Host hợp lệ; đủ objectives chỉ chuyển ReadyToTurnIn, không teleport hoặc gọi reward. TurnInRequest kiểm connection/character binding, state, turnInNpcId, sameMap, interaction range và claimedFlag. Một transaction commit reward+Completed+story/unlockFlags+summary eligibility, persist rồi publish; replay không cấp lần hai. Full-bag policy phải theo QUEST-02; nếu chưa commit được giữ ReadyToTurnIn, không mất reward/counters.

Tutorial staged grants (vũ khí/tiền/class unlock được GDD chỉ rõ) là giao dịch riêng có one-time flag, đủ để làm objectives trước turn-in; không nhầm với completion reward. Save sau staged class selection/reset và mỗi mutation critical; abort/reconnect không cấp lại 20 điểm hay weapon. Quest objective death dummy reset không loot/EXP event farm. Q12 boss eligibility event và quest turn-in là hai transactions có IDs khác: Boss loot một lần/event, story completion một lần/character; không duplicate Boss reward khi báo NPC.

ItemDefinition giữ rarityPrimaryStatIds, enhanceStatRules và fixedSlotBonusDefinition. Compute stat theo [GDD §7](HUYEN_LO_GDD.md#gdd-7); Tinh Hoa derive khi enhancement đạt mốc, không add-mutate mỗi equip/load. Bonus amounts/rarity interaction không data-freeze trước GEAR-01. **LootService chưa có final rarity policy trước LOOT-01**; chỉ dựng contract ownership/eligibility/idempotency, không ngầm code independent roll/supersede. Boss config stat/count/scheduler theo BOSS-02, availability theo BOSS-03; không lấy S07 assumptions làm asset values đã khóa.


Pipeline: validate request → chọn target hợp lệ cùng MapId → roll né/crit → damage theo GDD → cập nhật HP/status → emit result → death/reward transaction. ActionID+targetID+hitIndex ngăn damage lặp nhưng vẫn cho ba hit Liên Kích. Snapshot không reacquire; piercing theo thứ tự đường đạn; explosion loại primary. Cooldown/MP charge một lần cho action hợp lệ. Attack timing/animation không được biến mỗi visual frame thành hit mới.

Physics layer mask tách body collision, hurtbox và ground/platform; hit query/snapshot/pierce/explosion implement theo [GDD §4](HUYEN_LO_GDD.md#gdd-4). MobBrain reachable/blocked và projectile config theo [GDD §5](HUYEN_LO_GDD.md#gdd-5), một MobProjectile prefab đọc definition; ART-01/PHY-01 là gate test feel/vertical. Không duplicate luật roster/AI trong Technical.

<a id="network-authority"></a>

# 4. Network Authority

| Client gửi intent | Host quyết định | Client nhận |
| --- | --- | --- |
| Move/jump/drop, facing | Collision/map bounds và state hợp lệ | Position/state; interpolation/correction |
| Normal/skill, aim | Target/range/MP/CD/HP/status | Combat results và VFX trigger |
| Equip/sell/upgrade/pickup | Ownership, bag, level, cost, RNG | Inventory/stat snapshot |
| Quest/class/attributes | Prerequisite, điểm còn lại, one-time reward | Progression/summary |
| Portal/chat/PvP invite | Map/range/rate/match state | Transition/bubble/banner/match result |

P0 localhost/LAN/Direct IP cùng mạng; P1 Relay/VPN/tunnel. Không claim Internet-ready hay anti-cheat production. Validate request ở Host kể cả Host Player 1; client owner không được trực tiếp ghi HP/gold/profile. Co-op P0 phải đợi COOP-01 về EXP/normal loot; không nhầm personal Boss loot đã khóa với normal loot chưa chốt.

ChatService validate count/rate theo [GDD §9](HUYEN_LO_GDD.md#gdd-9), route MapId; global banner tách khỏi map chat, tên Boss lấy Q12 từng recipient. PvPService đọc restrictions/match rules từ GDD; edge cases PVP-01 là gate acceptance.

<a id="profile-authority"></a>

# 5. Character/Profile Authority

Host repository load bằng profileId/characterId. Client không upload level, gear, gold, EXP hay attributes như trusted state. Connection→profile→character binding là TECH-01: spike phải xác định ID tồn tại, quyền chọn, reconnect và duplicate-selection policy; không coi biết ID là authentication Internet.

Một active writer cho mỗi character; không cho hai connections cộng điểm/claim loot trên cùng save đồng thời. Runtime state và profile mutation cùng Host; kết quả transactions xác nhận sau khi cập nhật authoritative state. Disconnect gỡ player khỏi alive targets, chat và arena; xử lý contribution/loot theo connected-at-death rule GDD, không giữ eligibility bằng stale connection.

Invariant điểm: spent+unspent=5×(L−1); Lv1=0; Lv5 class reset tạo 20 unspent một lần, không nhận thêm 20 mỗi load. demo_class là ngoại lệ state onboarding hợp lệ: Lv5, Q5 Completed/Q6 Available, chưa class, 20 unspent. Template demo không phải nguồn ghi đè profile thật.

<a id="persistence"></a>

# 6. Persistence

| Save scope | Dữ liệu | Không trộn |
| --- | --- | --- |
| Character | schemaVersion/IDs/name, level/currentEXP/class, 4 attributes/unspent, gold/Journey, bag/storage/equipment, quests/reward flags/skills, map/position, HP/MP/status | Q12 không là world completion |
| World/session | nextBossSpawnUtc và trạng thái timer cần phục hồi | Không lưu player HP ở world record |
| Runtime transient | Active connections, threat/contribution, hit dedup | Không restore stale target references |

Item instance IDs phải unique trong bag/storage/equipment; equipped item không đồng thời nằm bag. Không dùng ví dụ JSON điểm cộng sai 95; validate EXP range, IDs, inventory count, enchant 0..5, class restrictions và điểm invariant khi load. Restore dead/Food/HP/MP/restart position nằm SAVE-01/CONS-01; chưa tự heal đầy hay offline regen.

**Safe save contract:** serialize snapshot → temp file cùng filesystem → flush → atomic replacement của bản chính → giữ backup hợp lệ → recovery validate schema/invariants. Có thể spike FileStream.Flush(true), File.Replace hoặc rename phù hợp platform; API cụ thể phụ thuộc Unity/.NET/filesystem, không bắt buộc trong GDD. Thử crash ở từng bước và first-save khi chưa có destination. Không overwrite corrupt main/backup bằng nhân vật mới im lặng; giữ bản hỏng để điều tra và báo recovery rõ.

Autosave 120s; critical save: logout/disconnect, class choice, Tẩy Mạch, quest/reward, enhance **success lẫn fail**, Boss loot/reward, Stage Summary. Upgrade fail vẫn tiêu tiền/đá nên phải persist. Serialize writes theo character; transactionID/claimed flags chống request retry cấp thưởng hai lần. Crash consistency inventory/currency/reward cùng transaction là acceptance, không chỉ chứng minh JSON parse được.

JSON P0; MariaDB P1 sau domain/repository ổn. Không bê schema/opcodes NSO hay triển khai SQL chỉ vì reference có 59 tables. Host crash/restart không đồng nghĩa mọi client có bản save đáng tin để upload.

<a id="timers"></a>

# 7. Timers & Inactive Maps

| Timer | Authority | Empty map/re-entry |
| --- | --- | --- |
| Mob/Elite respawnAtUtc | Global SpawnManager | Brain pause; nếu now≥deadline thì restore đúng một lần |
| Ground loot despawnAtUtc | LootManager | Expire dù root không active; lifetime theo GDD §7, không phụ thuộc active root |
| Boss nextSpawnUtc | Global WorldBossManager | Countdown/banner không phụ thuộc player ở Huyền Tích |
| Skill/Food/Potion | Host runtime clock | Local hit-stop không pause; offline policy chờ quyết định |

Dùng UTC deadlines cho lifecycle cần tồn tại qua inactive maps; runtime combat dùng clock nhất quán, không trust client timestamp. Re-entry resolve overdue timer idempotently trước gửi snapshot. Tránh catch-up sinh nhiều Boss hay pile-up cả chuỗi respawn khi map lâu rỗng. Restart semantics ngoài nextBossSpawnUtc cần gate SAVE-01.

WorldBossManager cập nhật threat/contribution từ damage đã validate; target resolver chỉ chọn alive/arena hợp lệ. Reset countdown hủy khi có alive player trở lại, deadline và threshold đọc [GDD §5](HUYEN_LO_GDD.md#gdd-5). Reset phải clear cả threat/contribution, không giữ bằng dead-body count. Một global entity; story display state không tạo thêm Boss.

BossDeathEvent chụp eligibility snapshot (connected/map/contribution, kể cả dead hợp lệ theo GDD) một lần rồi RollPersonalReward/SpawnOwnerLoot. Threshold derive `runtimeMaxHP×contributionFraction`, demo config đổi runtime HP thì threshold cũng đổi. Roll semantics chờ LOOT-01. Pickup verify owner/map/range/instance/count, idempotent trước consume ground object; bag full không mất item. Spawn/reward/despawn và banner đều có eventID/deadline, không lặp mỗi tick. Q12 accept/Ready không đổi nextSpawnUtc trước BOSS-03.

<a id="art-contract"></a>

# 8. Art / Animation Technical Contract

| Hạng mục | Contract | Kiểm import |
| --- | --- | --- |
| Male rig | 64×64px, PPU 32, body 44–48px, pivot Bottom-Center | Point filter, không texture blur; căn chân |
| State frames | Idle4 / Run6 / Jump2 / Fall2 / Attack3 / Skill4 / Hit2 / Death3=26 | Parts cùng state/index, đúng tổng frame đã khóa |
| Modular parts | BodyBase/HairHead/Armor/Pants/Weapon; ba visual families | Weapon Front/Back theo pose; không slot phụ vô cớ |
| Facing | Vẽ hướng phải, flipX trái | Sockets/attack origins flip đồng bộ |
| Collider | ~0,60–0,65u×1,45u, TUNABLE | Không lấy kích thước toàn canvas |
| Mob/Elite | 6 normal sets; Elite scale 1,25–1,30/aura | Same sprite/animation/skill/projectile |
| Background | Forest/Mountain/Ancient +hub props reuse | Telegraph, loot, chat không bị foreground che |

Feedback renderer nhận event Host; local hit-stop/flash không pause simulation. VFX/worldUI sorting phải giữ telegraph/loot/chat đọc được; content feedback do GDD §11 sở hữu.

Actor part order baseline: Shadow0 → WeaponBack5 → BodyBase10 → Pants14 → HairHead20 → Armor22 → WeaponFront30 → CombatFX40; đây là order kỹ thuật cần kiểm overlap, không thêm slot trang bị.

Sorting baseline: background → terrain/back props → actor parts → attack/telegraph effects → readable feedback/worldUI; weapon back/front nằm hai phía body theo pose. SortingGroup/part offsets là candidate implementation, phải thử overlap hai player và mob. Foreground alpha/occlusion được tune bằng readability, không tạo animation set mới để giải quyết layer.

Shared animator state/frame controller cho parts; damage timing từ combat event/data, không trust client AnimationEvent. Local hit-stop/flash/trails/floating damage chỉ presentation; player Hit state không tự gây stun. Frozen overlay generic chỉ normal/Elite, không cần sprite set riêng. Death tint/animation giữ corpse để camera/eligibility hoạt động.

Asset gate: làm một full rig 26 frames, một weapon/armor/pants family và một farm-room trước sản xuất đủ tiers. Kiểm outline/palette/pivot/frame sync khi chuyển gear. AI-generated bitmap nếu dùng vẫn cần slice/clean/import/QA; chưa tạo asset trong vòng docs này.

<a id="ui-notes"></a>

# 9. UI Technical Notes

Một router/modal stack cho NPC/inventory/character/quest/PvP. NPC services và bindings thuộc [GDD §10](HUYEN_LO_GDD.md#gdd-10); text/modal context chặn gameplay input và xử lý chord priority theo action map duy nhất.

**QuickConsumableAction:** bindings xem GDD §10; Host validates inventory/level/count/cooldown/state, chọn item theo GDD và consume một lần. UI hiển thị reject reason; không copy hotkey table hoặc tutorial content. Quest HUD dùng state snapshot, không chuyển Available bằng client-side objective flag.


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

**Gate đầu: spike ba ngày** — Host+Client với hai profiles độc lập; move/jump/drop-through; portal chuyển MapId; client khác map không hit/loot/chat được; Host load character ID; basic persistence crash experiment. Observer optimization không chặn gate này. Nếu movement correction/profile/save fail, xử lý trước mở content production.

| Tuần | Milestone | Gate |
| --- | --- | --- |
| 1 | Spike/runtime/movement/profile | Hai client, MapId, định nghĩa data |
| 2 | Combat slice/rig pipeline | Normal, skill Lv5, multi-target, no-contact, một modular family |
| 3 | Progression/items/consumables | EXP/attributes, class reset, Food/Potion/Death, inventory |
| 4 | Gear/shop/quest framework | Enhance sáu slots, safe transactions, Q1–Q8, review P1 |
| 5 | World groups/remaining skills | Năm farm roots, sáu mob, hai Elite, toàn bộ active/passive |
| 6 | Boss/co-op/persistence | Personal loot/reset/timers, Q11/Q12; chốt COOP/BOSS gaps |
| 7 | PvP/chat/story/demo | Q9 optional, match 120s, banners, summaries, profiles |
| 8 | Integration/QA/package | Regression, evidence hai client, build/scripts/video fallback |

Working target 160–200h, risk envelope 160–240h, tám tuần; không phải estimate đã chứng minh. Art worksheet cũ ≈164–210h riêng vượt tổng: phải đo slice rồi chọn reuse/giảm polish P1/P2. Không giảm sáu mob/hai class/26 frames user-lock để ép schedule. AI giảm boilerplate code/docs, không bỏ Editor/prefab/animator/tilemap/slicing/UI/network debugging/playtest. Milestone trượt thì cập nhật giờ và scope P1 minh bạch.

<a id="risks"></a>

# 11. Risk Register

| Risk | Mức | Evidence / gate | Xử lý |
| --- | --- | --- | --- |
| MP spam/INT value | CRITICAL | S04 deficit lớn khi INT 0 | Chốt dependency, test rotation thực |
| Co-op formula thiếu | CRITICAL | COOP-01 | Chốt trước reward code, không copy NSO |
| Profile binding/save corruption | HIGH | TECH-01 / SAVE-01 | Spike ID binding, crash recovery, một writer |
| Prediction/latency | HIGH | Chưa playable | Đo movement correction với hai client |
| Art/editor hours | HIGH | Art worksheet vượt budget | Một full family làm gate, reuse |
| Boss fairness/TTK | HIGH | S07 assumptions/BOSS-02 | Đo uptime, telegraph, target/reset |
| Quest full bag/onboarding | HIGH | QUEST-02 | Reward transaction và solo Q9 regression |
| Physics/target readability | HIGH | PHY-01/ART-01 | Test airborne/platform/target sau lưng |
| Observer performance | MEDIUM | P0 còn replicate | Profile rồi optimize P1; MapId correctness trước |
| P1 scope creep | HIGH | Nhiều audit proposals | Chọn sau gate P0; research không là DoD |

<a id="qa"></a>

# 12. Demo / QA / Acceptance Tests

**Tất cả ca dưới đây: CHƯA CHẠY.** Đây là checklist nghiệm thu tương lai; vòng hiện tại chỉ chạy doc validation và phép tính. Không đánh dấu playable/done bằng mô phỏng.

| Profile | State cố định | Demo |
| --- | --- | --- |
| demo_new | Lv1, Novice; kiếm chỉ cấp ở Q3 | Movement/Q1–Q4 |
| demo_class | Lv5, Q5 Completed/Q6 Available, 20 unspent, chưa class | Chọn class/reset/equip/skill Lv5 |
| demo_mid | Lv10 | Farm group/gear/Food/Elite |
| demo_end_host | Lv20, character riêng | Boss/PvP Host |
| demo_end_client | Lv20, character riêng | Boss/PvP Client |

DemoExpMultiplier 10, BossHP×0,40, EliteRespawn20s, BossRespawn60s phải là demo config rõ; release không dùng nhầm. DemoBossMaxHP 12.800 và contribution 640 cùng derived value, không chỉ UI. Quay class selection bằng demo_class, không demo_mid.

| Bộ nghiệm thu | Ca bắt buộc | Evidence |
| --- | --- | --- |
| Progression | Cumulative 53.100, carry EXP, catch-up remaining, reset Lv5 một lần, 95 điểm, không branch cap/rank | State trước/sau và reload |
| Combat | Ba normal intervals, mọi skill/hit count, snapshot/pierce/explosion, vertical/facing/no contact | Hit log/video hai client |
| CC/AI | Freeze normal/Elite, Boss/PvP chỉ Slow; Hybrid reachable melee, blocked ranged, Sói melee | Cases theo target/mode |
| World/timers | 18 groups, Return reset HP, deadlines map rỗng/re-entry, respawn idempotent | Clock/MapId logs |
| Boss | Threat/retarget, dead-body reset 10s, threshold release/demo, dead eligible, disconnect/leave ineligible, loot 90s | Contribution/reward logs |
| RPG | Sáu slots×năm tiers×rarities có gain; +0..5 strict gain, Tinh Hoa derive/reload không stack, reject undefined bonus/roll policy, fail persist/full bag | Transaction/inventory logs |
| Quest | State graph/level gate/đúngNPC, Ready không auto-turn-in, staged grant replay, Q9→Q10 solo, Q11 unlock chỉ Completed, Q12 Main Story Complete và continued play | Solo+online journey |
| PvP/chat | Invite/accept, level gap,120s/%HP timeout, chặn consumables, Food pause, MapChat/rate/80 ký tự | Hai client; chốt edge policy |
| Save/network | Reject forged state/replay, ID binding, disconnect save, crash từng bước, backup recovery | Corrupt/crash artifacts và log |
| Art/UI | 64×64/PPU 32/26 frames, pivot/frame alignment, gear flip, layer readability, input context | Import audit/video |

Chuẩn bị build đóng gói, README chạy demo trong bộ phân phối khi implementation tới gate, script mở Host/Client, reset demo profiles có chủ đích và video fallback. Không tạo thêm Markdown trong active design tree ở vòng này. Chọn rubric/trình tự trình diễn theo feature đã chạy; video fallback không thay evidence acceptance.
