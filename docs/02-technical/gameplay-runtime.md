# Huyền Lộ — Gameplay Runtime

Các số BASELINE/TUNABLE chưa phải nghiệm thu runtime.

## Document owns

Cách triển khai clock/input/combat/quest/physics/map/UI/presentation; không sở hữu balance values.

## Document does not own

Luật ngoài domain thuộc owner trong [documentation map](../README.md); evidence thuộc [Playtest & Balance](../04-production/playtest-and-balance.md), thứ tự triển khai thuộc [Roadmap](../04-production/roadmap.md).

**Map Chat:** Enter mở input / gửi, tối đa 80 ký tự; rate 1 message / 2 s mỗi playerId do Game Server kiểm; bubble trên đầu tối đa hai dòng, 4 s rồi fade 0,5 s, cùng MapId. P0 không history lớn. System banner do Game Server phát cho Boss T−60, spawn / death và chapter completion; P1 có thể thêm history nhỏ.



<a id="maps"></a>

<a id="2-scene-và-map-trong-unity"></a>

## Scene và map trong Unity

Code/tọa độ/lỗi của prototype nằm tại [Roadmap — lịch sử](../90-archive/production-history.md#prototype-technical-history), không là contract production. Local production slice dự kiến vào World với profile dev và ba roots theo [VS-1](../04-production/roadmap.md#vs-1); session vẫn kiểm MapId/transition. Login không bắt buộc cho combat probe đầu; luồng online TARGET là Boot/Main Menu → Login → CharacterSelect → overlay kết nối → WorldOnline tại recovery map/SafeAnchor.

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

**Authoring gate và phân bổ bãi quái hiện hành:** Thử nghiệm trước trên một farm room đại diện trước khi nhân rộng toàn map; bảo toàn tuyệt đối các Quest Anchor IDs (`DS2`, `DS3–DS6`, `TA4`, `TA6`, `TA4.slot1`, `XN1–XN6`, `HT4–HT5`, `HT_BossLandmark`), giữ ổn định các stable authored seed IDs (`TA5`, `BV1–BV5`, v.v.), SafeAnchor, dải vào an toàn 6–8 u từ cửa map, tuyến rút lui về làng và đường tiếp cận bãi rơi đồ (loot).

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
- `SpecialGate` có logic kích hoạt xác thực riêng theo design owner (Huyền Môn Q11 cần tương tác kiểm đủ 3 Mảnh Ấn; Lôi Đài cần chấp nhận thách đấu), không dùng generic Interact cho mọi lối đi.
- **Cơ chế chống giật chuyển cảnh liên tục (Anti-Pingpong Transition):** Tọa độ xuất hiện (`spawnAnchor`) ở bản đồ đích luôn được đặt cách mép collider trigger chuyển cảnh tối thiểu 2,5–3 u về phía trong lòng map (`safe inner offset`), nằm hoàn toàn ngoài phạm vi trigger trả về. Hệ thống duy trì cờ `isTransitioning` và chỉ kích hoạt chuyển cảnh tiếp theo sau khi người chơi đã rời khỏi vùng an toàn hoặc qua thời gian ân hạn (`re-arm upon exit`).
- **Xử lý lỗi và đồng bộ:** Một pending transition cho mỗi actor, loại bỏ trùng lặp (dedup trigger/request). Nếu bị từ chối (Locked), hệ thống gửi thông báo lý do một lần; người chơi phải bước ra ngoài vùng exit rồi bước vào lại mới có thể thử lại. Commit checkpoint đích vào cơ sở dữ liệu trước khi gửi snapshot cho Client; nếu commit thất bại thì giữ nguyên vị trí ở bản đồ cũ, không gửi snapshot giả. Quá trình chuyển map dọn dẹp sạch sẽ target, threat, action pending và visuals của map cũ.

<a id="combat-data"></a>

<a id="3-dữ-liệu-và-luồng-combat"></a>

## Dữ liệu và luồng combat

| Definition / state | Trường cần có | Validation |
| --- | --- | --- |
| SkillDefinition / profile | Stable SkillId, class, UnlockLevel/ManualRequirement, slotIndex; power/MP/CD/shape/maxTargets/executor/timeline/status/VFX | SingleMelee/LogicalSingle/Arc/SnapshotSpread/Line/Explosion; không branch theo tên skill, SkillRank hoặc graph editor tổng quát |
| SkillAcquisitionState / PassiveDefinition | LearnedSkillIds; bốn passive IDs/class/unlockLevel/icon/tooltip/effect params | Active học từ manual; passive derive class+level theo owner design tương ứng, không thêm rank/points/duplicate flags; effect enum nhỏ |
| CombatRequest | Character binding, request correlation/sequence, requested SkillId, target identity/life/MapId, facing/aim intent | Authority kiểm binding/alive/map/capability/MP/CD/lock/range/target; Client không chọn actionId kết quả đáng tin |
| HitResult | Authoritative actionId, targetId/life generation, hitIndex, evade/crit/damage/remainingHP | Client chỉ trình diễn result; exact wire fields/time ở proposal bên dưới |
| MobDefinition | mobIdentityId/fixedLevel/baseRigId/paletteRef; stats curve, capability/profile, movement/range/timeline/hit shape/projectile presentation, loot/sourceProfileRef, linhBienEligible | Bảy fixed-level identities trên sáu rigs; palette reuse không cần AI/animation riêng; curve chỉ evaluate fixedLevel |
| LinhBienModifier | Stat/reward modifiers, visual preset, variant tag | Áp trên base runtime đúng một lần; không duplicate base identities/rigs |
| SpawnGroup / SpawnSlot | mapId/groupId/slotId, mobIdentityRef/cached level/spawnPosition; HomeRegion/HomeSpan/WalkRegion/SurfaceId/AggroRange/LeashRegion; deadline/generation/variantState/Q8 waiting set | Vùng đã author quyết đường đi/aggro; cache level phải bằng fixedLevel; respawn không đổi identity |
| ItemDefinition / Instance | templateID/instanceID/GearSlot/rarity/enhancement/count, stat lists; optional buyPrice/explicit sellValue, loot/material/binding refs | Không suy stat từ UI position hoặc fake buyPrice cho drop-only; tutorial-bound tách bản vendor |
| QuestDefinition | prerequisiteQuestIds/requiredLevel; ordered objectiveGroups/sourceIds/markerIds/counts/guaranteedEvidenceOrdinals; giver/turn-in requirements/rewards/unlockFlags | Nội dung theo [Quests & Narrative](../01-design/quests-and-narrative.md#quests-story), không bảng quest thứ hai |
| QuestProgress per QuestId | state/activeObjectiveGroup/counters/qualifyingKillOrdinal/virtualEvidence; activation/claimed/staged flags và selected mentor/ring khi cần | State riêng từng character/QuestId; backend commit theo revision/receipt các mục liên quan |

Production data phải author đủ **12 QuestDefinitions Q1–Q12 và toàn tuyến thật** ngay từ đầu; G-L chỉ thực thi slice Q1–Q6, không cho phép cắt definitions/quest runtime thành bản sáu nhiệm vụ. Bảng mô tả nhu cầu domain/state, **chưa khóa wire schema**. `actionID` trong request cũ chỉ là shorthand correlation; authority mới tạo/bind actionId sau validation. Client sequence/correlation khác ID của action/result/life. Local dùng commit adapter RAM; backend transaction/ACK dưới đây mô tả TARGET, không gọi RAM ACK là durable.

## Quest, vật phẩm và chỉ số

**Quest runtime:** `Dictionary<QuestId, QuestProgress>` là state authority đang dùng; PostgreSQL giữ bản đã commit. Q9 Available/InProgress đồng thời với Q10 Available/InProgress là hợp lệ; không dùng một `currentQuestId` làm nguồn duy nhất. UI có thể pin chính tuyến, nhưng nhánh optional vẫn lưu riêng. Chỉ active group nhận event, đúng source/MapId/filter từ design owner. Q6 chọn phái đồng thời activate nhóm equip/learn/confirm trước action tiếp theo. Q7 confirm kiểm đúng ring instance đang sở hữu và đã ≥+1; nếu đã mặc thì không ép re-equip.

**QuestState:** Locked → Available → InProgress → ReadyToTurnIn → Completed. EligibilityService kiểm đủ prerequisite IDs **và** requiredLevel theo graph design owner, không ngầm dùng Qn−1 cho mọi nhánh. Thiếu level thì trả Locked cùng reason/nextLevel cho NPC/HUD, không hiện marker Available.

QuestService nhận event sau khi hành động server thành công, kèm eventID/characterId/MapId/targetId và payload cần thiết: `NpcTalked`, `RegionVisited`, `ItemPickedUp`, `ItemEquipped`, `ItemSold`, `ConsumableUsed`, `MobKilled`, `VirtualEvidenceCredited`, `AttributeAllocated`, `ManualLearned`, `SkillUsed`, `EnhancementSucceeded`, `PvPCompleted`, `BossEligibilityAchieved`. Kiểm QuestId/active group/target, chặn replay rồi cập nhật counter/step.

Pickup/equip/sell/use/enhance thất bại không phát event; dùng bình khi đầy không có Used. Thay đổi inventory/character và quest step từ **cùng action** commit trong một transaction (các mục liên quan), với commandId/payload bất biến; publish durable success sau ACK. Riêng Potion phát accepted gameplay/ConsumableUsed từ server timeline, durable consume và Used progress cùng receipt theo [Potion durability](online-and-persistence.md#potion-durability).

Keydown/click hoặc UI tự báo thành công không cấp credit; không cần event bus lớn.

**NPC và quest:** giver/turn-in/service/MapId/anchor lấy từ definitions theo [NPC owner](../01-design/quests-and-narrative.md#npc-roster). Q3/Q4/Q7 thuộc Bách Luyện; Q1/Q2, mở Q6 và chính tuyến Q10–Q12 thuộc Lâm Bá. Yên Thảo bán thuốc/hồi sinh/Tẩy Mạch, Mộc An giữ Storage/Rest, Hạo Vũ giữ PvP. Q6 chọn phái và sau đó turn-in tại mentor đã chọn ở Học Viện. Server resolve mentor hợp lệ từ class + quest state (Phong Du/Diệp Lam), không tin turnInNpcId tùy ý từ Client.

Class/`ClassChosenLevel`/mentor binding/grant và active group commit cùng action chọn phái; replay không đổi mentor. Tạ Minh là LEGACY reference, không còn current route. Giữ stable internal IDs khi đổi display/ownership; NPC đặt vào khu chức năng, tọa độ còn OPEN, không thêm NPC thay thế.

TurnInRequest kiểm connection/character binding, ReadyToTurnIn, NPC đã resolve, sameMap/range/claimedFlag. Một transaction gồm reward, Completed, story/unlock/summary eligibility và cleanup evidence. Capacity preflight xét compatible stacks và số X slots thực cần; thiếu thì giữ Ready, trả reason+X, không thưởng một phần hoặc cleanup/unlock sớm. Retry trả receipt cũ. Đủ objective chỉ chuyển Ready, không teleport/auto-turn-in. Q12 chỉ Main Story/Chapter III Complete, vòng chơi vẫn tiếp tục.

**Staged grants:** receipt `(characterId,QuestId,grantId)` giữ quyền nhận một lần và pending khi túi đầy. Áo/sample Q4 là supply thật; ground hết hạn hoặc reconnect thì phục hồi entitlement chưa claim với cùng instanceId, không cấp reward lại. Áo bound tới equip, sample tới sell. Q6 grant weapon/manual trước learn/cast; confirm spent≥1 chấp nhận điểm đã cộng trước, không ép point mới khi pool0. MP Potion I reserved có receipt/đường cấp chắc chắn ở đúng step;

QuickMP ưu tiên món này, chỉ Used sau khi tiêu thật lúc thiếu MP. Q7 chỉ grant ring nếu thiếu, lưu selectedInstanceId/binding chống sell/drop tới confirm. Đá/Vàng reserved chỉ cấp khi cần +0→+1, không cấp thừa nếu đã ≥+1. Entitlement có pending/spawned/claimed/consumed; không dùng supply ngoài bước cho phép. Q12 Boss death credit và turn-in có receipts riêng; báo NPC không tạo pile nữa.

**Học manual:** sáu manuals ánh xạ tới sáu class SkillIds; class/level/S1 prerequisite cho S2/S3/boundCharacterId theo design owner. Kiểm ownership/alive/idle/no pending action/chưa learned. Consume book và thêm learned SkillId trong một command/receipt. S2 giữ S1, S3 không bắt S2. Cooldown dictionary theo SkillId; đổi slot/học mới không reset deadline cũ. RunningAction giữ profile snapshot; passive derive từ class+level.

SelectedSlot chỉ là UX, không cấp quyền cast hoặc tạo DB subsystem. **Manual không có cooldown:** learned/grant/learn receipts chặn lặp; chỉ skill có CD.

**Supply tutorial:** trước khi tạo grant/ground/entitlement, kiểm QuestId/InProgress/active step/source/generation/receipt. Ngoài đúng bước đó chỉ regular loot, không tạo entitlement cho bước tương lai. Retry quyền hợp lệ giữ itemInstanceId, không reroll hoặc hồi tố kill cũ. Mộc Kiếm Q3, thuốc Q6, ring/resources Q7 và reward Q12 không được gây softlock khi túi đầy/reconnect/replay.

**Bằng chứng ảo và credit:** evidence là virtual counters, không nằm trong bag hoặc có physical pickup RPC. Kiểm QuestId/InProgress/active group/source/count còn thiếu; threshold/recipient predicates theo owner design tương ứng. Khi gây ActualHpLost, ghi activeQuestId/objectiveGroup/targetGeneration vào quest-qualified ledger; damage trước accept hoặc sai step không hồi tố. Whole-life ledger vẫn phục vụ reward/threat.

Death chụp quest eligibility trước tăng step; counters/qualifying ordinal/evidence/receipt commit cùng character mutation. Dùng guaranteed ordinal của chính recipient, không world kill count/RNG, và clamp count. Level penalty không chặn quest/supply. Q11 activation từ ngoài Huyền Môn kiểm ba counters, commit flag idempotent; cleanup chỉ sau turn-in thành công. N players có progress riêng; Talk/Equip/Use/Enhance/Region/PvP không tự share và không phụ thuộc last hit.

**Thêm item vào inventory:** điền compatible stacks trước, overflow tạo stack tới capacity. Commit toàn amount hoặc giữ nguyên ground/pending. Compatibility gồm template/binding/instance flags; tutorial/manual-bound không merge với unbound. Bag Sort P1 không là điều kiện để auto-stack P0 chạy.

**Tính chỉ số trang bị:** ItemDefinition giữ `GearSlot`, `rarityPrimaryStatIds`, `enhanceStatRules`, `fixedSlotBonusDefinition`, `equipLevel` theo [Items & Economy](../01-design/items-and-economy.md#gear-economy). Data khai báo HP family Armor/Pants/Boots và MP family Weapon/Ring/Necklace, không suy từ bên trái/phải UI. HP/MP mới chịu rarity/enhance theo primary lists design owner; ACC/EVA, Crit/tốc chạy cố định, flat gains và Tinh Hoa dùng đúng thứ tự design owner.

Sáu class weapon templates riêng, không một weapon đổi stat theo người mặc. Quyền nhận drop/mua khác quyền equip; kiểm class/equipLevel tại equip/transfer preview và commit, không suy từ MapId/source. Giữ enhancement cap theo Items owner; derive Tinh Hoa theo cấp, tooltip cả khóa/mở và reject vượt cap. Retune HP/MP không đổi giá/source.

**Tính chỉ số character:** đọc owner design tương ứng, không copy balance table/formula. `ClassChosenLevel` ghi cấp thực lúc chọn phái cùng class transaction, giữ nguyên khi level-up/reset/reload; Tân Lữ chưa có datum này. Cung tăng HP chậm chỉ sau cấp đã chọn, nên chọn muộn không mất HP nền tại transaction; VIT có hiệu quả như nhau. Cộng nền/attributes/items sau rarity/enhance/flat/Tinh Hoa, rồi passive nền tảng đúng một lần.

Giữ số lẻ tới damage rounding/UI, không cộng stat trực tiếp mỗi equip/load. Level-up/class/equip/transfer/reset dùng cùng evaluator; sau ACK giữ currentHP/MP rồi clamp, không ratio-heal/revive. Phiên mới dùng checkpoint HP/MP rồi clamp; Food/status/CD chỉ giữ khi resume cùng phiên. Passive tinh thông chỉ tăng direct skill hit đúng cự ly, không basic Tân Lữ/Bỏng/proc chance; source chụp lúc cast, target/cự ly xét lúc resolve.

**UpgradeTransferCommand:** sourceInstanceId/targetInstanceId/commandId/expectedRevision; kiểm sender, ownership/bag, hai IDs khác nhau, slot/band/class/equipLevel/tutorial binding theo design owner. Chỉ cùng bậc hoặc lên đúng bậc kế, không I→III; preview/commit dùng cùng evaluator, no-gain reject trước mutation, không RNG. Plan tiêu source/cost + đổi enhancement đúng target instance; backend commit một receipt (các mục liên quan), không consume rồi spawn bản copy.

Retry trả target/result cũ dù source đã mất; hai commands tranh source chỉ một thắng. Failure giữ committed state, không spawn lại source để rollback. Equip riêng sau transfer, không auto-equip hoặc cần ô mới.

## Death, loot, spawn và action timeline

**Death/reward:** instanceID UUID duy nhất qua server restart, không GameObject.GetInstanceID/slotId; slotId authored giữ cố định. deathID=(instanceID,generation). Chụp RuntimeMaxHP/ActualHpLost ledger, contributor MapId/position/alive/connected/lastDamageAt và **level trước reward**, rồi mới sửa quest/EXP; sort stable characterId. DoT credit nguồn thật, không last hitter. Predicates reward/quest/Boss khác nhau theo design owner, không dùng một recipient list cho cả ba.

Không chia lại phần bị loại; TopDamage chọn whole-life ledger, không fallback né level suppression. Commit immutable roll/recipients/receipts trước publish; retry cùng deathID không reroll.

**Shared loot:** một roll/death theo catalog/rates design owner, exclusive gear buckets ở data. LootRecord giữ itemInstanceId/deathId/MapId/deathUtc/ownerId, contributorSnapshot gồm playerId/levelAtDeath và deadlines. Một OwnershipPhase(deadlines,now), pickup predicates theo mode. TopDamage eligibility100/0 quyết regular set trước roll, không RNG level gate/fallback. Fixed mob identity/level chọn band/slot pool, MapId chỉ chọn material theo design owner.

Contributor trong death ledger dùng level đã chụp cho pickup; người tới sau dùng current level. Recheck sameMap/alive/distance/capacity/window; không mất quyền vì chính kill làm level-up. Capacity fail giữ ground.

Claim/death transaction shapes và durability thuộc [Online & Persistence](online-and-persistence.md#persistence). Runtime tạo immutable death/claim payload và chỉ finalize durable rewards sau adapter success.

**Spawn arbitration:** một server-owned record/MapId giữ random activeSlotId/generation và Q8 reservation/waiting characterId set riêng, sửa trên simulation thread. Due slots serialize theo slotId; spawn đọc identity/fixedLevel/position/group, evaluate curve rồi Linh modifier đúng một lần. Random variant roll/cap theo World owner, cộng ngoại lệ reservation Q8 cố định; Lv1–7 không roll. Initial population cùng path; Return/root wake/reconnect không respawn/reroll.

Death đặt slot deadline25s BASELINE/TUNABLE; terminal-pending chưa finalize không respawn. Mốc capture deathUtc/release cap/reservation/due ordering còn [A15/TECH-01](../04-production/playtest-and-balance.md#pending-ordering-probes), không suy lethal đã cho phép release trước ACK.

**Q8 reservation:** server giữ random occupancy và một Q8 reservation chung tách biệt, theo [World policy](../01-design/world-and-content.md#q8-bounded-path). Key request là character/QuestId/active step, waiting set/age stable và bind target generation. Due arbitration ưu tiên reservation tại TA4.slot1; unrelated Linh không chặn admission. Không promote/reset actor đang combat.

Persist force entitlement/economic receipt qua adapter online để retries không reroll budget; snapshot credit riêng từng requester, không nhân reward theo waiting count. Rebuild request từ progress/entitlement sau restart, không phục hồi actor life cũ. Các bounds/schema cụ thể phải qua Q8-01.

Boss stat/scheduler/shape đọc [World & Content](../01-design/world-and-content.md#world-farm). Một action tại một thời điểm; ba vùng đá không double-hit, geometry/né/nhịp cần PlayMode. Q12 không spawn Boss; credit và loot/turn-in receipts tách nhau.

**Canonical combat:** target-based authoritative combat + logical geometry validation. Tại HitMoment, melee kiểm alive/MapId/life/generation/front/facing/vertical overlap/range/attack shape với hurtbox, rồi resolve evade/crit/damage. Sword sprite collision không quyết damage. Ranged resolve logic; tên hình ảnh qua target khác không đổi victim. Arc/Line/Spread/Explosion do authority chọn secondaries, không VFX collider.

**ActionTimeline:** authority kiểm requested SkillId/class/learned/level/weapon/MP/per-skill CD/common action lock/target/geometry. Accepted start tạo actionId/life/startClock, snapshot SkillId/profile/source stats/passive/origin/facing rồi commit MP/CD đúng một lần. RunningAction không đọc mutable selectedSlot. Một timeline cho basic Tân Lữ và các ExecuteSelected; sau class không zero-MP Normal Attack hoặc RepeatOnHold.

Nhịp S1/S2/S3 và MP mới là TUNABLE design owner, S2 có thể farm tần suất cao; không hard-code S1 là attack mặc định. Buffer/readiness theo [Combat & Character pending](../01-design/combat-and-character.md#pending-cast). Logical ranged resolve theo clock, không gameplay projectile. Spread ABC/ABA/AAA chụp start, ba hit cùng resolve moment từ profile/stable hitIndex; invalid index mất hit, không reacquire. Line sort intersections;

Hàn invalid primary không nổ, primary Evade vẫn nổ, secondary roll riêng/no primary double-hit. Dedup actionId+hitIndex+targetId+generation, revalidate life/MapId/shape và DEF/EVA khi resolve. Death/map/hard CC hủy unresolved action không refund; result đã resolve bất biến. Bỏ frame/AnimationEvent/packet trễ không phát damage lần nữa.

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

Physics masks tách body collision/hurtbox/solid/one-way. Combat geometry độc lập sprite/VFX colliders. LoS A/B và S2/S3 air permissions còn gate PHY-01/ART-01; không biến prototype mask thành production lock.

<a id="shared-combat-input"></a>

## Input, focus và interaction dùng chung

<a id="input-contract"></a>

Input adapter phát semantic actions `Move`, `Jump`, `DropThrough`, `SelectSkillSlot1`, `SelectSkillSlot2`, `SelectSkillSlot3`, `ExecuteSelected`, `CycleTarget(direction)`, `QuickHP`, `QuickMP`, `Food`, `Interact`, `Navigate`, `Confirm`, `Back`. Move ←/→, Jump ↑, DropThrough ↓ và select1/2/3 theo [Combat & Character](../01-design/combat-and-character.md#ux-art). Execute/Interact/Potion/Food/menu physical keys **OPEN**; candidate E/F/4–5/R/I chỉ PROPOSAL/TUNABLE DEFAULT cho usability. Không alias gameplay A/D/Space/S cũ hoặc tự gán C/Q.

Select hợp lệ chỉ đổi UX selectedSlot/SkillId, không cast/approach/MP/CD. Locked slot không đổi selection/action. `ExecuteSelected` mỗi physical press tạo tối đa một execution intent; release không hủy one-shot PendingCast, giữ không RepeatOnHold. Authority kiểm requested SkillId từ capability/definition, không tin selectedSlot hoặc secondary list Client. Chọn S2 rồi Execute nhiều lần là luồng bình thường, không special attack pipeline cho S1.

`CombatFocus` giữ mode NONE/AUTO/EXPLICIT và target ID/life/generation/MapId. Tách `search envelope` (vùng tìm), `retention range` (vùng giữ) và `execution range` (tầm thực thi). Lifecycle eligibility khác reachability để cast. NONE+Execute xét skill đã chọn: ưu tiên target local đánh được ngay, rồi target reachable bằng bounded horizontal approach. AUTO sticky, không bị target gần hơn chiếm.

Click tạo EXPLICIT, không đánh; xa/khác tầng/blocked vẫn giữ trong retention, Execute reject không swap. CycleTarget dùng local combat set; hướng ưu tiên cùng SpawnGroup → cùng WalkRegion/SurfaceId → nhóm lân cận nhìn thấy/cục bộ → local candidate khác. Order ổn định theo authored group/surface/IDs, không sort lại mỗi frame theo nearest; radius/order/vertical bounds **TUNABLE**.

Đổi/clear focus hủy pending/buffer trước khi bind target mới. Rời execution range/nhảy không clear focus còn trong retention. Target chết/despawn/new generation/life/WrongMap hoặc vượt retention thì clear; Esc/explicit replacement theo context, không tự cast/reacquire cho tới Execute mới. Interaction candidate riêng, `Interact` nhặt/dùng ngay không mutate CombatFocus.

**Player death và target HUD:** hủy PendingCast/BufferedIntent/approach, chặn combat input và cancel unresolved action theo timeline, **không clear CombatFocus chỉ vì player chết**. Retention/target-life validation không phụ thuộc owner alive; dead actor còn trong MapId vẫn nhận target state snapshots, marker/name/level/current-maxHP/bar cập nhật khi người khác đánh.

Target invalid mới clear theo các điều kiện trên; same SpawnSlot respawn có life/generation mới, không inherit focus cũ. Resume trong phiên còn sống giữ focus nếu vẫn hợp lệ; mất phiên không persist focus trong checkpoint. Corpse camera/loot/quest eligibility vẫn theo design owner, không từ HUD.

`PendingCast` giữ requested SkillId/target ID/life/generation/MapId/intentId/expiry/start/progress position/heldMovementMaskAtPress; chưa chụp source stats/action hoặc commit cost. `BufferedIntent` chỉ một execution intent mới nhất, có readiness/expiry theo authority clock. `RunningAction` đã accepted có snapshot riêng. Select đổi UX nhưng không mutate/cancel ba state này; **Execute mới** mới thay PendingCast/BufferedIntent, không sửa action đang chạy.

| Bước | Contract triển khai |
| --- | --- |
| Execute press | Chụp selected SkillId/target life/MapId và từng horizontal binding đang held; resolve/validate capability trước tạo pending/buffer |
| Input arbitration | Bỏ held mask cũ trong pending/buffer kể cả ngược hướng; KeyDown ngang mới/Jump/Drop/focus/UI/chat/Esc/death/map/invalid target hủy intent. Terminal trả quyền axis còn held |
| Buffered readiness | `max(remaining skill CD, remaining common lock)` phải sẵn trong window0,18s BASELINE/TUNABLE. Không approach trước ready, không FIFO/long-CD queue; expiry clear/reason |
| Approach path | Grounded/same reachable lane/wall/edge/progress và swept segment với exit; cắt EdgeExit thì Blocked trước movement/cost. Chỉ manual movement kích hoạt exit, không AssistAxis |
| Arrival validation | Target đúng life/generation/MapId/alive/notReturning; actor alive/no hard CC; learned/class/weapon/level/MP/CD/lock và range/shape tại origin thực. Fail clear/reason; success mới tạo actionId/snapshot/MP/CD đúng một lần |
| Cancel/reject | Blocked/no progress/timeout/invalid hoặc lifecycle/user cancel đều clear, không retry ngầm/chuyển target. Release Execute không cancel; hold không thêm intent |
| Esc dispatch | Consume đúng một tầng [Combat & Character](../01-design/combat-and-character.md#escape-priority); đóng UI/chat không rơi xuống cancel/clear focus cùng press |

Reject enums gồm PlayerDead/HardCc/NotLearned/WeaponRequired/InsufficientMp/Cooldown/ActionLocked/NoTarget/TargetMissing/TargetDead/TargetGeneration/TargetReturning/WrongMap/OutOfRange/Blocked/BufferExpired/UserCancelled. UI dịch reason thành text, không dùng text làm control flow.

Approach budget tính đoạn còn thiếu ngoài execution range của từng profile, không dùng một distance chung cho Kiếm/Cung. Chỉ chạy ngang với tốc chạy thường trên lane đi được; timeout/progress/budget cụ thể TUNABLE. Không tự jump/drop/dash, tìm đường nhiều tầng, nối pocket xa hoặc đổi target. Arc/Line kiểm victim từ origin dự kiến có thể tới trước assist, rồi kiểm lại từ origin thật trước commit; không tự đi tới secondary targets. Geometry vẫn có thể miss lúc resolve.

Gravity/momentum tiếp tục; air cast Tân Lữ/S1 cần probe, quyền S2/S3 trên không còn OPEN.

**Ranh giới UI:** keyboard/mouse dùng cùng action list/validator/command. Modal giữ selected action ID; renderer vẽ focus/lý do disabled rồi dispatch, không sửa progression. NPC mới mở ưu tiên quest action; consume input mở UI, không Confirm lần hai cùng frame. Khi list đổi, giữ selected ID nếu còn hợp lệ, nếu không thì clamp index; không gọi callback món/session cũ. Mở/đóng UI/transition/reset dọn pending gameplay và input capture; giữ phím không sinh Execute mới.

Enter là Confirm trong modal, Chat ở world. Tab/Shift+Tab điều hướng view trong UI, CycleTarget ngoài UI/chat, không dispatch cả hai. Bag filter theo GearSlot từ inventory hiện có; Equipment/Attributes/Derived Stats là views riêng cùng evaluator. Preview/world đọc cùng Art pose/socket contract. NPC text hiển thị kết quả command đã commit.

<a id="movement-feel"></a>

**Cảm giác di chuyển — PROBE/TUNABLE:** gameplay dùng phím mũi tên. Khi nhả Jump, cutoff điều chỉnh độ cao nhảy; coyote time tính từ lần đứng trên support cuối cùng. Jump buffer có hạn, chỉ consume một lần và một press tối đa một jump. Grounded kiểm chân/support và chiều chuyển động, không dùng sprite bounds. Độ cao nhảy thay đổi bằng cutoff/extra gravity khi đi lên; acceleration/deceleration riêng cho ground/air. Tốc trần dùng MoveSpeed evaluator gồm AGI/Giày/Slow/nước.

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

[Art](../03-art/art-and-visual-production.md#art-integration) sở hữu pose/frame mapping, module/catalog, sockets, sorting và import workflow. Technical giữ yêu cầu runtime, không có catalog hoặc sorting table thứ hai. `26 frames` là USER-LOCK; A01/A02 còn OPEN về raster/profile mapping, không tự đổi thành 33 frames. Rig baseline 64×64 px/PPU32/Bottom-Center/Point filter và state indices phải đối chiếu Art gate. `12 modules` cũ chỉ ba band×bốn loại, thiếu Mộc/fallback: LEGACY workload reference, không tổng asset hiện hành.

Một shared state/frame controller điều khiển modular parts; parts không chạy Animator clock riêng. Pose/phase/flip của front/back weapon và sockets đồng bộ. Physics root/hurtbox tách VisualRoot: thay gear/flip/scale không tự đổi collider. Collider baseline khoảng 0,60–0,65u×1,45u TUNABLE, không lấy toàn canvas. Attack origins author theo timeline, không sprite bounds. Không Climb state/animation. Module names/schema phải theo Art hiện hành, không giữ Pants/Lower hoặc Hair aliases thành hai slot.

**Player presentation — PROPOSAL:** Front/Side sprite-facing và carry Back/Hand đọc cùng pose controller, tách movement/action facing theo [Art](../03-art/art-and-visual-production.md#player-facing); đổi representation/order atomically để một equipped weapon không hiện hai lần. Carry timer chỉ cosmetic, không MP/CD hoặc delay Execute. Player dead state/terminal clock cùng identity/generation/MapId dẫn pop/fall visual-only → shared shadow; hide các outfit modules, giữ authority root/death anchor và HUD focus. Duplicate/stale không replay, late join bắt phase hiện tại; revive/map/life mới reset offset/visibility/timer. Không thêm physics knockback hay gameplay death states; schema/event cụ thể còn ở spike [presentation data](#presentation-data). Mob/Linh/Boss lifecycle giữ nguyên.

**Tích hợp map:** environment families Forest/Mountain/Ancient và hub props reuse theo Art. Tile/cell size derive asset pixels/PPU, không suy từ 64px rig. Background/decor/foreground không collider. Natural ground là solid mass; one-way chỉ mặt mỏng nhân tạo có support hợp lệ theo các mục liên quan. Validate movement/jump/drop với nhiều actor trước decorate. Foreground phải giữ telegraph/loot/nameplate/chat đọc được; không thêm lighting/streaming subsystem P0.

Stable MapId/NPC/region/slot/exit/gate/SafeAnchor và Boss exclusion được kiểm trên authored layout; seed manifest cũ không buộc current totals. Camera local follow/confiner/impulse đúng bounds và transition ACK. Kiểm hai hướng/cao độ/tint, wolf palette khác aura Linh. Ground mob route không trở thành route jump/drop chỉ vì asset có bậc. Nước chỉ shallow feet-contact visual, không swim/underwater physics.

**Feedback và pool:** renderer nhận Game Server event. AnimationEvent/FX không gây damage/stun hoặc pause simulation. Pool dùng chung configuration cho projectile/telegraph/status/hit/damage/NÉ/heal/upgrade/death/slash. Reuse reset tint/timer/owner/action/life, release hủy listeners/timers. Callback kiểm IDs/generation, không GameObject reference làm lifetime identity; pooled projectile chỉ presentation. Client bắt đúng phase còn hiệu lực, không replay gameplay khi packet trễ.

Hit animation không tự stun; Freeze Normal/Linh đọc rõ bằng overlay băng, Boss/PvP Slow bằng sắc lam/hạt lạnh nhẹ, Bỏng bằng tia lửa gọn. Overlay không đổi hitbox/collider. Corpse presentation phục vụ snapshot/camera/quest; player shadow/death anchor không pickup hoặc cast, vẫn quan sát focus hợp lệ theo [Art player death](../03-art/art-and-visual-production.md#player-shadow-death). Một SortingGroup và shared frame clock giữ nhiều actor/part không tách pose; layer order chi tiết từ Art.

Asset gate hiện hành nhập default/outfit I và Mộc/Kiếm vào **standalone disposable rig/art sandbox riêng**, ngoài VS-1 đã frozen. Fixture nhỏ kiểm mix-band/Tân Lữ/S1; thêm profile Kiếm khi cần kiểm reuse. Minimal Cung S1/S2/range/focus/kite/socket được probe trong Pha R; full production Cung vẫn theo G-C sau gates.

Chưa nhân ba families khi A01/A02/A17 hoặc grip/pivot/phase chưa kiểm. [Roadmap](../04-production/roadmap.md#production-release) sở hữu thứ tự; [legacy art gate](../90-archive/production-history.md#legacy-art-gate) giữ trace.


<a id="ui-notes"></a>

<a id="9-hợp-đồng-ui"></a>

## Hợp đồng UI

Một router/modal stack cho NPC/inventory/character/quest/PvP. InputActionAsset giữ Gameplay/UI contexts; chat/modal chặn gameplay actions và clear buffered movement/execution. Jump/DropThrough resolver ưu tiên Drop trên one-way nếu cùng frame, không emit cả hai. Chỉ owner local gắn input/camera/HUD; world online không cần PlayerInputManager couch join.

Semantic actions và quyền interaction theo [Combat & Character](../01-design/combat-and-character.md#ux-art); glyph đọc actual bindings, không hard-code candidate E/F/4–5/R/I hoặc alias cũ vào objective/tooltip.

**QuickConsumableAction:** server kiểm inventory/level/count/CD/state, chọn item theo design owner và consume một lần. MatchId còn kiểm quota theo design và cấm Hồi Sinh Phù; quota tăng tại cùng realtime acceptance với consume/cooldown/heal, không chờ DB commit, reject không tiêu item. Q6 QuickMP ưu tiên reserved Potion I ở đúng step, không bỏ món reserved để làm kẹt objective. Hiển thị reason khi reject.

Quest HUD dùng canonical state, không client tự chuyển Available/Ready. QuestDefinition giữ semantic enum/event/region/NPC/instance requirements; credit theo successful result, không keydown. Q3 author ít nhất ba Dummy placements cùng pool/lifecycle, không đổi farm respawn để chữa thời gian chờ tutorial. NPC focus ưu tiên context dịch vụ; combat execution riêng với interaction.

| View | Nội dung cần kiểm |
| --- | --- |
| HUD | HP/MP/EXP, selected skill và locked/learned/CD/MP state, Execution affordance, Food/Potion/quest/Boss timer; S2 chọn và farm thường xuyên, không trình bày S1 như default attack |
| Inventory/shop | Capacity/stack rules từ design owner, server transaction trước refresh |
| Upgrade/transfer | Preview cùng evaluator; source tiêu/target trước–sau/cost/Tinh Hoa/khóa–mở rõ; failure vẫn lưu cost |
| Skill panel | Ba active slot tích lũy/CD riêng, manual requirements, hai passive/class và mốc Lv5/13; passive hiện final stats, không skill points/ranks hoặc mới chọn là cast |
| Quest/chapter | State/nextLevel/resolved turnInNpcId server-owned; Ready vẫn chơi tiếp; Q6 mentor đúng class, Q10–Q12 Lâm Bá, Q12 Main Story Complete |
| WorldUI | HitResult/status/evidence theo recipient; combat focus marker + current/max HP/text/bar đồng bộ; owner chết vẫn thấy damage của người khác trên target còn hợp lệ; loot windows/quest cues/chat/Boss name riêng per-character |
| Death/PvP | Death choices khác PvPDefeated; stake/escrow pending/confirmed/phí/refund/quota, không xác nhận mutation khi chưa ACK |

Bag Sort/protection gear/quest arrows/history là P1, không điều kiện để P0 hoạt động. Tooltip làm tròn cho đọc nhưng evaluator giữ fractional enhancement. Không đưa RPC/saveAPI/microservice vào player flow.

<a id="dev-mode"></a>

<a id="91-dev-mode-để-kiểm-feature-và-phục-hồi-test"></a>

## Dev Mode để kiểm feature và phục hồi test

Dev Mode là tooling P0 cho người phát triển/tester trong development build, tách player UX/release. Không dùng dev shortcut làm lời giải cân bằng hoặc thay đường chơi production. Dev commands vẫn đi qua authority/evaluator/receipt khi thay state bền; không client tự sửa canonical snapshot. Dedicated cần quyền dev riêng trong cấu hình development và test account/storage riêng; không đưa credential/quyền/dev UI vào release client.

| Nhóm thao tác | Tooling cần có và giới hạn |
| --- | --- |
| Progression/class | SetLevel/EXP/attributes; Set/ClearClass với `ClassChosenLevel` hợp lệ; learn/unlearn skill; inspect derived stats/points/CD. Thay state invalidate pending action và derive lại evaluator, không cộng stat lần nữa hoặc đoán cấp chọn phái |
| Quest/inventory | SetQuestState/active step/prerequisite; give/remove item/Gold/manual; inspect/reset entitlement/counter/receipt test có chủ đích. Giữ stable IDs/bindings và phân biệt fixtures đã grant với reward chơi thật |
| World/combat | Teleport tới authored marker/SafeAnchor; spawn/reset group/life; force Linh chỉ cho probe; reset encounter/Boss/status; inspect target/focus/action/generation/clock/threat/contribution/loot deadlines. Không dùng force để thay production reservation hoặc autoroll |
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
