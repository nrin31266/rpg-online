# Huyền Lộ — Online & Persistence

Các số BASELINE/TUNABLE chưa phải nghiệm thu runtime.

**RULE:** realtime Potion không chờ DB ACK. Durable operations khác vẫn dùng transaction/receipt. Cơ chế write-behind/crash recovery cụ thể còn OPEN và chặn G-D/G-T nếu chưa chứng minh được invariant, không được gọi queue RAM là crash-safe.

## Document owns

Auth/ticket/lease, reconnect, checkpoint, transaction/receipts, Potion durability, escrow và crash/outage.

## Document does not own

Luật ngoài domain thuộc owner trong [documentation map](../README.md); evidence thuộc [Playtest & Balance](../04-production/playtest-and-balance.md), thứ tự triển khai thuộc [Roadmap](../04-production/roadmap.md).

<a id="network-authority"></a>

<a id="4-quyền-quyết-định-qua-mạng"></a>

## Quyền quyết định qua mạng

| Client gửi intent | Game Server quyết định | Backend khi cần dữ liệu bền |
| --- | --- | --- |
| Move/Jump/Drop, Execute requested SkillId/target hint, facing/aim | Physics/target/range/MP/CD/hit/damage/status; broadcast state/result | Không gọi mỗi tick/hit |
| Pickup/equip/sell/shop/upgrade/transfer | Kiểm MapId/NPC/range/mode/bag ownership; tính roll/preview từ definitions | Inventory/Gold/equipment atomic, unique IDs/receipt/revision; trả canonical snapshot |
| Quest/class/attributes/manual | Kiểm event/NPC/step/level/entitlement; resolve mentor/ClassChosenLevel và outcome | Counters/reward/unlock/class/ClassChosenLevel/learned/points cùng transaction |
| Map transition/chat/PvP | Map/state/rate/match; realtime outcome và Journey entitlement | Escrow trước MatchId; payout/phí/refund/Journey/receipt theo MatchId |

Game Server PlayerRegistry theo characterId, connection binding/MapId và threat/contribution/recipients đều collections N-player. ConnectionId chỉ là handle tạm; sender lấy từ transport, không playerId trong payload. Direct Client API P0 chỉ account/list/ticket, không internal mutation. Combat không gọi backend trừ mutation bền như reward/tiêu item/quest. Client có thể dự đoán hình ảnh, không damage. Actor chết vẫn nhận world/target snapshots hợp lệ, không vì observer filter alive mà mất HUD.

**PvP escrow:** Game Server giữ invite/MatchId đúng hai participants, Countdown/Active, HP/MP/Food/CD/quota bình và realtime result. Stake từ tập design owner, accept khớp exact stake. Spring khóa hai character rows theo stable ID, kiểm đủ tiền/lease và tạm giữ stake mỗi người cùng **một transaction inviteId**, đồng thời ghi pre-Arena checkpoints. Bất kỳ phần nào fail thì rollback (hoàn tác) cả hai, không MatchId. Sau ACK HELD mới tạo/bind MatchId, countdown; backend mark ACTIVE phải ACK trước combat/clock120s. Không trừ tiền lại khi kết thúc.

**Outcome/settlement:** Game Server gửi WIN/LOSE/DRAW/FORFEIT/SYSTEM_ABORT + MatchId. HP0 thắng; disconnect sau ACTIVE xử thua ngay, không reconnect grace trong trận; trước ACTIVE cancel/refund100%. Hai disconnect cùng tick/không chứng minh thứ tự hoặc lỗi server/backend rớt cả hai thì SYSTEM_ABORT, không dựa callback order để chọn forfeit. Hết120s còn sống là DRAW, không so HP.

Spring tính pot/fee/refund theo design owner, commit Gold/Journey/settlement receipt một transaction, không chạy combat lại. WIN/FORFEIT winner90%pot; DRAW mỗi người90%stake; pre-active cancel/SYSTEM_ABORT100%. Retry MatchId trả receipt cũ. Backend lỗi chờ refund khi phục hồi; crash orphan reconciler100% chỉ nếu chưa settled, ACK mất sau settlement không refund thêm. Không dùng PvE receipt cho PvP.

**Trong Arena:** Food tick/hết hạn theo thời gian thật; bình thật/CD riêng HP–MP theo design owner. Quota theo design tăng ngay tại realtime acceptance của Game Server, cùng consume/cooldown/effect, reject quota không tiêu item; cấm Hồi Sinh Phù. Entry đầy HP/MP; RAM chụp pre-match để người còn phiên về Vân Khê với HP/MP cũ/clamp, Food không hoàn thời gian, bình không hoàn. Mất phiên load checkpoint trước Arena, không tọa độ Lôi Đài. Q9 credit trận thật WIN/LOSE/DRAW, không FORFEIT/abort; forfeit winner vẫn Journey theo design owner.

P0 local/LAN: backend/server bind địa chỉ config; localhost chỉ khi cùng máy, Client khác dùng server reachable. Internal API chỉ Game Server qua HTTPS hoặc LAN cô lập có service credential; credential không trong Client/public build. Không claim Internet-ready hoặc commercial anti-cheat.

<a id="profile-authority"></a>

<a id="5-tài-khoản-nhân-vật-và-phiên-chơi"></a>

## Tài khoản, nhân vật và phiên chơi

**Tài khoản do admin tạo:** không có Register cho player. Admin dùng seed script/CLI hoặc endpoint admin được bảo vệ riêng để tạo username duy nhất, password hash bằng BCrypt/Argon2 của Spring Security qua package đã pin, trạng thái enabled và character test. Không lưu plaintext password trong repo/log. Character Select vẫn hỗ trợ nhiều character/account; P0 chưa cần Character Create UI khi admin cấp sẵn. `demo_new`, `demo_class`, `demo_mid`, `demo_end_sword`, `demo_end_bow` chỉ là tên fixtures, không phải authority của save.

**Luồng màn hình:** Boot/Main Menu → Login → Character Select → connecting overlay → World theo recovery checkpoint; character mới bắt đầu tại Vân Khê. Không thêm email/OAuth/forgot password, server browser hoặc Loading Scene riêng. Spring login trả session token ngắn hạn cho API client. Character Select đọc `characterId, name, level, class` thuộc account, không dùng inventory/Gold do client gửi làm dữ liệu đáng tin.

Chọn character gọi Spring lấy **one-time game ticket**, TTL 60 s, ràng `accountId, characterId, sessionId, expiry, nonce`. Client gửi ticket tại connection approval. Game Server dùng service credential đổi ticket qua internal API; backend kiểm login/ownership/expiry/nonce, chưa consumed và chưa có active lease. Consume ticket và cấp lease trong **cùng transaction**. Ticket replay/hết hạn bị từ chối; client tự gửi characterId không chứng minh quyền. Token không vào chat/log; P0 dùng TLS hoặc LAN cô lập.

**Một writer cho mỗi character:** Spring giữ lease `(characterId, gameServerInstanceId, sessionGeneration, expiresAt)` và revision. Game Server heartbeat mỗi 30 s; lease hết hạn sau 90 s không heartbeat. Sau join, Game Server cấp resume token ngẫu nhiên, một lần dùng, ràng characterId/sessionGeneration. Transport rớt thì giữ actor tối đa 15 s trong world, vẫn chịu AI/damage/timer nhưng không nhận input. Resume đúng server trong grace sẽ rotate token và gắn connection mới vào cùng runtime position/HP/MP/Food/CD/status; không tạo actor thứ hai.

PvP ACTIVE là ngoại lệ: disconnect xử FORFEIT ngay, không resume vào trận; trước ACTIVE cancel/hoàn cược. Hết grace hoặc phiên bị hủy thì checkpoint cuối nếu backend còn hoạt động, despawn và release lease. Ticket mới bị chặn khi lease cũ còn sống; phiên mới chỉ nạp PostgreSQL sau grace/lease expiry. Crash/outage có thể chỉ giữ checkpoint đã commit gần nhất. Mutation/checkpoint từ generation cũ bị từ chối.

**Khôi phục phiên mới:** Game Server nạp canonical progression/checkpoint, derive MaxHP/MaxMP rồi clamp HP/MP lưu sẵn. Farm/combat dùng SafeAnchor của checkpoint MapId. Vân Khê/Học Viện có thể dùng exact coordinate đã checkpoint nếu còn walkable, trong bounds và ngoài trigger exit/gate; sai thì SafeAnchor map đó. Lôi Đài không là recovery map: dùng checkpoint pre-Arena về Vân Khê/pre-match state.

HP=0 vẫn dead, corpse ở SafeAnchor và giữ lựa chọn về làng/dùng phù; reconnect không hồi sinh miễn phí. Character mới chưa checkpoint bắt đầu Vân Khê đầy HP/MP. MapId mất hiệu lực sau đổi content thì fallback Vân Khê nhưng giữ HP/MP đã clamp.

Mất phiên xóa Food/status/CD/action/threat và focus; resume ngắn giữ runtime phiên còn sống. Death trong phiên khác session loss: focus hợp lệ vẫn quan sát được theo [input contract](gameplay-runtime.md#input-contract). Runtime PlayerId ánh xạ characterId, không phải NGO clientId.

Điểm vẫn tuân `spent + unspent = 5 × (L−1)`; reset nhập môn một lần, late class không set tổng điểm về 20. `ClassChosenLevel` phải được ghi cùng class transaction, giữ nguyên khi level-up/reset/reload; evaluator dùng [Combat & Character](../01-design/combat-and-character.md#class-combat) để tránh mất HP khi chọn Cung muộn. Fixture đã có class phải author datum này rõ ràng. Dữ liệu class cũ thiếu datum cần migration/import có nguồn xác nhận; không đoán bằng current level hoặc silently đổi HP nền. Demo import chỉ vào account/character test riêng.

<a id="persistence"></a>

<a id="6-lưu-dữ-liệu--spring-boot--postgresql"></a>

## Lưu dữ liệu — Spring Boot + PostgreSQL

**Dữ liệu bền theo character:** level/EXP/class/`ClassChosenLevel`/attributes/points/reset flag, Gold/Journey, inventory/storage/equipment instances, learned SkillIds, per-QuestId progress/entitlements, onboarding `MpPotionTutorialArmed` / `MpPotionTutorialCompleted`, story/unlock/summary flags, receipts và checkpoint tối thiểu `(MapId, HP, MP, safePosition nếu ở khu an toàn, checkpointSeq, sessionGeneration, updatedAt)`. `ClassChosenLevel` nullable chỉ khi chưa class; chọn phái commit class và datum này atomically.

Quest supply bindings/selectedInstanceId/mentor route cần đủ dữ liệu để retry đúng instance và đúng mentor, không client chọn tùy ý.

**Onboarding Bình MP — gameplay đã duyệt, encoding G-D:** hai flags/receipt là state per-character độc lập `quest_progress`; không thêm QuestId hoặc bắt một table/service mới. Q6 class admission arm tutorial cùng class/mentor transaction; thuốc dự trữ vẫn có grant receipt riêng theo bước hiện hành. Khi Armed và chưa Completed, accepted MP Potion tạo Complete delta cùng ordered consume command; HP Potion/reject/full MP không Complete. Consume + onboarding Complete + receipt commit atomic; runtime có thể giữ accepted Complete pending để không nhắc/consume lặp trong lúc chờ ACK, nhưng không gọi state đó đã durable. Q6 turn-in/unlock không đọc Completed này. Reload dùng flags đã commit và reconcile pending Potion theo POT-01, không suy Complete từ Q6Completed hay thiếu thuốc; không regrant hoặc reset tutorial khi death/map/session đổi. Schema/import policy có version cần kiểm ở G-D, không migration SQL trong lượt docs.

Checkpoint không giữ tọa độ farm/combat chính xác, Arena, Food tick, CD, pending cast, status, projectile, threat/contribution hoặc AI/Boss HP. `isDead` derive HP=0. Restore clamp HP/MP khi Max stat đổi, không tự heal. PostgreSQL là authority của state đã commit; RAM Game Server là authority realtime trong phiên.

**Schema design candidate — PROPOSAL/OPEN, DOC-ONLY:** chưa có backend/SQL implementation trong repo; tên bảng dưới là đề nghị, không schema đã khóa. G-D quyết encoding/constraints/migration sau integration spike. `accounts(id, username UNIQUE, password_hash, enabled)`; `characters(id, account_id FK, name, level, exp, class_id, class_chosen_level nullable, gold, journey, reset_used, revision, lease_generation, ...)`; `character_attributes(character_id PK/FK, str, vit, int, agi, unspent)`; `item_instances(id PK, character_id FK, location/slot, template_id, rarity, enhancement, quantity, binding)`; `character_skills(character_id, profile_id PK pair)`; `quest_progress(character_id, quest_id PK pair, state, active_group, typed counters/Q4 tutorialOrdinal/RNG outcomes/placed flags, entitlement_state)`; `character_unlocks(character_id, flag_id PK pair)`; `recovery_checkpoints(character_id PK/FK, map_id, hp, mp, safe_x nullable, safe_y nullable, checkpoint_seq, session_generation, updated_at)`; `mutation_receipts(command_id PK, result, created_at)`; `world_loot(item_instance_id PK, death_id, map_id, payload, ownership_snapshot, deadlines, claimed_by nullable)`; `game_tickets`/`character_leases`; `pvp_escrows(invite_id PK, two_character_ids, stake_each, match_id UNIQUE nullable, phase, server_instance_id, active_at, outcome, settled_at)` với unique settlement receipt theo MatchId.

Quest counters có thể là versioned JSONB data. Không save bằng file hoặc lưu static Item/Mob/SkillDefinitions trùng ScriptableObject.

**Checkpoint cadence — BASELINE:** mỗi 30 s cho actor ngoài Arena; không ghi mỗi hit/Food tick. Ghi critical checkpoint tại map transition (commit destination MapId trước chuyển), orderly logout, hết reconnect grace, death, revive/về làng và trước/sau PvP. Potion persistence đi theo ordered command stream bên dưới; không đặt consume/checkpoint ACK trước realtime heal.

Trong Arena, lưu consume + receipt/quota cần recovery, không dùng HP/MP trong trận để ghi đè checkpoint pre-Arena. Pre-Arena checkpoints của hai người commit cùng escrow; kết quả/abort đưa người còn phiên về Vân Khê với HP/MP pre-match/clamp.

`sessionGeneration + checkpointSeq` tăng đơn điệu; backend từ chối packet cũ đến sau transition/death/escrow. State lấy từ Game Server. Periodic lỗi retry cùng ID; crash tải checkpoint cuối đã commit, có thể trễ khoảng một chu kỳ trừ mốc critical. Inventory/progression đã commit không mất theo checkpoint. Logout chờ critical ACK; backend lỗi kéo dài thì đóng phiên có thông báo, lần sau dùng state bền cuối.

**Ranh giới transaction:** Game Server tính gameplay result một lần từ definitions và snapshots: deathID, contribution, quest-qualified credit, rolls, enhancement RNG, inventory preview. Spring không chạy lại combat/drop formula. Internal service kiểm credential/lease/generation/commandId/expected revisions/IDs/capacity/ownership, commit deltas và canonical result. Kiểm receipt **trước revision mới**; duplicate command trả receipt đã commit, command mới phải đúng lease/revision.

Một death với N recipients và shared pile commit rewards/quest credit/loot/death receipt chung hoặc rollback toàn bộ. Khóa character rows theo stable ID. Trong terminal-pending, mob không nhận hit mới, không respawn, chưa publish reward; finalize sau ACK. Claim ground→inventory cùng claim receipt trong một transaction, hai claims chỉ một thắng. Turn-in simulate consume remaining required bound items (Q11 fragments đã placed chỉ revalidate flags, không consume lại) rồi merge rewards để kiểm net capacity; commit consume/reward/Completed/unlock/entitlement cleanup chung, không partial consume.

Enhance fail vẫn commit cost/receipt; transfer tiêu source/cost và sửa target chung. Action chọn phái Q6 kiểm cả hai discovery talk receipts rồi commit class/`ClassChosenLevel`/mentor binding/grant/active group và `MpPotionTutorialArmed` chung. Turn-in là action riêng sau khi đủ core objectives, được kiểm tại cùng mentor và có receipt riêng; không đợi `MpPotionTutorialCompleted`.

**Retry và lỗi backend:** durable reward, claim, quest turn-in, gear và escrow chỉ publish success sau ACK. Potion là ngoại lệ realtime đã duyệt; admission/consume/effect do server timeline quyết định theo contract bên dưới. ACK chỉ xác nhận durability, không áp lại heal hoặc rollback một combat tick đã xảy ra. Logout chờ các command/checkpoint/escrow còn pending rồi release lease.

Timeout chưa biết commit thì giữ immutable payload/commandId trong RAM và retry có backoff. Backend/DB lỗi thì pause mutations/reward-generating actions/PvP, báo gián đoạn. PvP không tiếp tục khi backend không thể settle: SYSTEM_ABORT, refund khi backend phục hồi. Hết lease đóng phiên, chặn generation cũ. Durable success chưa commit thì chưa được công nhận; Potion realtime accepted được xử lý theo contract riêng; đã commit mất ACK thì receipt trả đúng result, không roll/grant lần hai. Definite reject cần reconcile theo probe, không retry cùng lỗi vô hạn. Không Kafka/event sourcing/distributed transaction.

**Escrow recovery:** `inviteId` có trước MatchId. HELD transaction khóa hai rows, kiểm stake/tiền, trừ mỗi người `W`, lưu pre-Arena checkpoints; retry cùng inviteId không trừ lại. ACK HELD mới tạo/bind MatchId; countdown rồi mark ACTIVE phải ACK trước combat. Invite chưa accept hết hạn không trừ tiền. HELD/BIND lỗi, disconnect pre-ACTIVE hoặc server crash refund100% idempotently.

Game Server gửi WIN/FORFEIT với winnerId/loserId thuộc đúng hai participants. Spring tính từ W: winner `2W × 0,90`; DRAW mỗi người `W × 0,90`; SYSTEM_ABORT mỗi người `W`. Phí và Journey +200 cho winner cùng settlement receipt trong một transaction khóa hai rows. Receipt kiểm trước revision; terminal outcome đã commit chặn outcome khác.

Reconciler hoàn HELD chưa bind quá 60 s hoặc match mồ côi khi server heartbeat mất quá 90 s, **chỉ nếu chưa settlement receipt**; race kết quả được serialize bằng lock/unique state. Outage thì refund pending tới khi phục hồi; UI không báo đã trả trước ACK. Potion đã consume hợp lệ không hoàn theo tiền cược. P0 không cần payout service riêng.

**World recovery:** `world_loot` đã commit được restore với deadline UTC gốc nếu chưa hết hạn; cleanup idempotent, không reroll. Normal slot generation/variant, Boss HP/phase/threat/respawn deadline chỉ ở RAM. Restart tạo normal lives mới và **một Boss Alive đầy HP**, không phát credit/loot encounter cũ. Boss có thể xuất hiện sớm sau restart: hạn chế restart chủ đích trong demo, ghi scope P0 vào QA; chỉ thêm persisted deadline nếu evidence exploit cần.

Crash PvP refund100% khi backend phục hồi, không Journey; match đã settled giữ receipt, không đổi abort. DB corrupt/migration fail chặn gameplay startup, không khởi tạo world như chưa có dữ liệu.

<a id="timers"></a>

<a id="7-đồng-hồ-và-map-rỗng"></a>

## Đồng hồ và map rỗng

| Timer/state | Authority | Map rỗng / restart |
| --- | --- | --- |
| Normal/Linh slot deadline, natural cap occupancy | Game Server RAM, global SpawnManager | Map rỗng có thể pause AI; due slot resolve một lần khi vào lại; restart tạo slot life mới và roll theo spawn mới |
| Ground loot deadline/claim | Game Server chọn cửa nhặt, Spring/PostgreSQL lưu pile/claim | UTC tiếp tục khi map rỗng; restart chỉ restore pile chưa hết hạn |
| Boss spawn/death/HP/phase | Game Server RAM | Countdown không cần người ở map; restart một Boss Alive full HP, bỏ encounter/threat/contribution cũ |
| Skill/Food/Potion/status và exact HP/MP/position | Game Server session clock/RAM | Resume ngắn giữ runtime; session loss nạp checkpoint, bỏ Food/CD/status; vị trí theo recovery policy của [profile owner](#profile-authority), gồm SafeAnchor và ngoại lệ safe-map/pre-Arena |

Runtime combat dùng một server clock, không trust client timestamp. Timer quá hạn xử lý idempotently, không catch-up tạo nhiều Boss/pile. WorldBossManager theo dõi actual-loss direct/DoT, clear khi wipe; scheduler/Slow/Cuồng Mạch theo design owner. Boss death commit shared pile/per-recipient credit qua các mục liên quan trước broadcast; Q12 không sinh Boss mới. P0 không restore giữa trận. MapId correctness độc lập presentation hide/show.


<a id="potion-durability"></a>

## Potion realtime và dữ liệu bền

**LOCKED invariant:** server kiểm session generation, commandId, inventory khả dụng, cấp, alive, phần HP/MP thiếu, cooldown và PvP quota tại simulation tick. Khi chấp nhận, consume một bình trong RAM authoritative, tăng quota/cooldown, áp heal và phát accepted result đúng một lần. Không chờ backend để cứu mạng; request trùng trả kết quả cũ.

**Minimum architecture direction:** một writer/character và ordered mutation stream với commandId, generation, sequence, immutable inventory delta và result receipt. Bình đã debit RAM không thể bán/chuyển kho/dùng lại trong pending command khác. Durable consume và qualifying action/onboarding progress của cùng action commit chung; Bình MP tutorial ghi `MpPotionTutorialCompleted`, không `UsedMpPotion` objective blocking Q6. ACK không replay effect. Checkpoint có thứ tự theo stream, không ghi snapshot HP lúc nhấn bình đè lên damage/Food/death mới hơn.

Resume phiên còn sống dùng đúng RAM state và pending IDs, không nạp lại inventory cũ. Phiên mới không được mở trong khi writer cũ/pending reconciliation còn quyền ghi. Generation fencing, receipt-before-revision và atomic inventory debit vẫn bắt buộc. UI có thể phân biệt accepted gameplay với durability pending; reward/quest completion bền vẫn đợi commit.

**OPEN — POT-01:** chọn cơ chế durable handoff tối thiểu và chính sách crash với accepted command chưa vào PostgreSQL. Queue RAM đơn thuần sẽ mất debit sau process crash: **chưa đạt invariant**, không được phát hành như đã giải. Spike so serial write-behind với recovery record/reservation nhỏ nếu cần; không mặc định một journal nhiều tầng, event sourcing hay Kafka. Phải chứng minh retry không double consume và crash/rejoin không cho hoàn bình lặp vô hạn trước G-D/G-T.

**Candidate ưu tiên để spike, chưa LOCKED:** minimal local durable recovery record cho Potion, ghi/flush payload immutable trước apply heal rồi async commit Spring/PostgreSQL; restart reconcile unresolved records theo DB receipt trước writer mới. Phải đo latency flush trong acceptance path, pin generation/sequence và include qualifying onboarding delta; backend reservation là đối chiếu nhưng có thể đặt network ACK trên đường heal. Local journal không tự chứng minh machine/disk-loss hoặc multi-server failover; quyết supported failure scope/replay/fencing tại G-D. Không dùng candidate này để gọi realtime/durability gate đã pass.

**Bounded outage:** admission có giới hạn pending count/age và lease health, số cụ thể OPEN/TUNABLE. Hết budget thì ngừng nhận **command mới**, báo gián đoạn và đóng/reconcile phiên theo policy đã đo; effect đã accepted không bị rút lại. Không cho farm/reward-generating actions vô hạn khi backend chết. Definite reject sau realtime acceptance là lỗi reconciliation cần cô lập writer, không trừ item lần hai hoặc tự heal lại. Không tự reset quota vì ACK timeout.

**Gate:** [POT-01 probes](../04-production/playtest-and-balance.md#potion-acceptance) phải giải cả process crash trước persistence; chỉ test lost ACK sau commit là chưa đủ.

<a id="a13"></a>

## A13 — quyết định liên quan

**A13 Select preview** — Default/class preview: không thêm data; exact gear: reuse rig nhưng cần canonical visual summary · Default preview/name/level/class; exact gear chỉ nếu duyệt dependency read-only · Design/Technical + P11/P12; không account redesign

**A13** — TECH-01 / ART-01 · OPEN preview default hoặc exact gear/read-only summary; Login/Select TARGET P0, DEFERRED khỏi local slice đầu.


<a id="a15"></a>

## A15 — quyết định liên quan

**A15 Death terminal-pending** — Chờ ACK mới Death: nhất quán nhưng trễ; terminal state → Death ngay: đọc tốt nhưng cần event/state riêng · Diễn theo server terminal, success/reward sau ACK; deathUtc/deadline không tự đổi · Technical + P06/P12; không local đoán chết

**A15** — TECH-01 / SAVE-01 / ART-01 · BASELINE terminal-pending không reward/respawn trước commit; OPEN event presentation trước ACK và deathUtc t0 contract chi tiết.

<a id="personal-quest-recovery"></a>

## Personal quest entitlement/recovery — TARGET recommendation, schema OPEN

**Forensics — snapshot VS-1 trước khi xóa, xem [Git history](../90-archive/production-history.md#prototype-source-retired):** chỉ RAM `Receipts`, `pendingGrants`, `Loot.Tutorial` và `Tick` re-offer sau 60 s; chưa serializer/save/Spring/PostgreSQL/RPC. `WorldRevision` chỉ map rebuild, chưa Inventory revision. Không có existing production tables để migrate. Character durable state phải giữ quyền nhận/claim, network visibility không là ACL.

| Viable design | Cost/DB | Exploit/recovery | UX | Kết luận |
| --- | --- | --- | --- | --- |
| **A Persisted entitlement + recreated ground** | Bounded record/quyền trong quest progress payload hoặc relation tương đương; writes ở source/death, claim, completion; không write mỗi visual tick/TTL | Claim+bag atomic; epoch/source outcome/receipt chống replay; one-writer lease/fencing chống reconnect race. TTL chỉ retire visual, reload quyền chưa claim | Về sourceMap, dọn bag rồi tự nhặt; tracker/ground cue phải rõ | **Recommendation BASELINE architecture**; giữ gameplay confirmed, encoding/schema/TTL OPEN |
| B Recoverable pending grant + durable ground record | Pending grant + ground row TTL; có thể reuse world_loot storage candidate, thêm re-offer/retirement writes | Claim+bag atomic; pending payload sống qua TTL; hai records phải đồng bộ grant/ground/claim và generations, personal row không regular FFA | “Lấy lại” tại source marker hiện ground rồi tự nhặt; thêm thao tác so A nhưng không direct-to-bag/NPC mới | **Viable alternative**, state/DB/recovery-command cost cao hơn A |

A đề nghị logical record `(characterId, QuestId, questEpoch, objective/source, sourceEventId/sourceKind, optional tutorialOrdinal, immutable entitlementId, item payload/binding/qty, sourceMap/sourceAnchor, Pending|Claimed|Consumed|Cancelled, receiptRefs)`. Tuple không phải locked wire/SQL schema. Ưu tiên versioned quest-progress payload nếu G-D chứng minh bounded queries/atomic mutation đủ dùng; chưa bắt buộc table/microservice riêng. Q11 restored ba identities + một material mob type; Q8/Q10 compatible quantities merge nhưng claim receipts độc lập.

**Ordering:** persist eligibility/RNG outcome (cả failure)/entitlement đúng active group trong source/death transaction trước expose ground; rollback không có grant giả. Payload immutable, không reroll. Pickup add Inventory + Claimed + receipt/revisions cùng transaction; uncertain ACK query receipt, không cấp lại. Stack merge ghi mapping/delta vào receipt, split không duplicate incoming ID. Turn-in consume + reward + Completed + Consumed/cleanup atomic. Spring validate credentials/lease/ownership/revisions/constraints rồi commit authoritative result từ Game Server; không Java realtime combat/quest engine.

**Bounds:** chỉ quantity objective cần, bag+outstanding rights serialize theo revision; kills sau cap không thêm quyền. Ground TTL hữu hạn, exact TUNABLE; 60 s có thể làm PROBE vì regular pipeline dùng 60 s, chưa lock. Mỗi right có một active representation trên current writer/session; epoch/generation loại pickup cũ. Cosmetic TTL/offset/phase không bắt buộc durable row mỗi lần re-offer nếu restore/fencing chứng minh an toàn. Pending không hết quyền vì TTL/rời map; cleanup sau consume/completion/cancel commit, receipt retention chống replay chốt tại G-D. Không ground vĩnh viễn hoặc pending vô hạn do kill spam.

**Recovery UX — PROPOSAL:** owner sống và về **sourceMap** thì re-offer tại nguồn cũ nếu standable/reachable; điểm invalid do content/death location dùng authored anchor gần source trong cùng map. Tracker ghi món chưa nhặt/vùng lấy lại. Không teleport vào bag, không random current location/làng, không NPC service mới. PrimaryAction trên source/marker hoặc vào recovery zone có thể hiện ground; exact trigger/anchor/glyph là PROBE. Server author/physics validate reachability/ground/bounds, offset tách regular/personal piles; client không chọn trusted position. Người khác không thấy/claim; nhiều món cùng owner có separation và deterministic selection, không hidden trong solid/ngoài map/click-blocked.

| Lifecycle/failure | Kết quả cần giữ |
| --- | --- |
| Full bag/revision conflict | Không add/claim/collection credit; Pending giữ. Conflict lookup receipt/reload rồi request mới, không đổi payload dưới ID cũ |
| TTL/map change | Retire representation, giữ right; về sourceMap re-offer, không reroll/kill lại |
| Death/revive | Quest-bound không loss; Pending giữ, dead không pickup; shadow presentation không sở hữu item delivery |
| Resume≤grace | Giữ session/ground nếu chưa TTL; không actor/representation thứ hai |
| New session/server restart | Load committed aggregate; Claimed chỉ ở bag, Pending generation mới tại sourceMap. Old lease/RPC reject; death chưa commit không infer grant từ client |
| Duplicate/concurrent pickup | Cùng command trả receipt; commands khác claim cùng right serialize, một thành công; whole mutation cùng revisions/ownership constraints |
| Quest Completed | Consume+cleanup atomic; không re-offer/grant bước tương lai, old callback reject epoch/state |
| Abandon | Chưa P0; nếu thêm cần cancel epoch+items+rights atomic, không reaccept để farm grant/vendor vô hạn |
| Backend outage/unknown commit | Pause durable mutation theo policy; receipt query/retry, không success UI trước ACK, không fallback RAM unlock/progress |

Acceptance G-D cần crash trước/sau source/death/claim/turn-in commit, mất ACK, hai writers, old generation, TTL/resume/death/map/content-anchor change, 60/60 có/không compatible stack, N eligible/outsider, Completed/abandon-future replay. RAM chỉ kiểm transitions, không chứng minh durability. POT-01 và A15/death clock/cap release vẫn OPEN riêng; chưa giải Potion crash window hoặc đổi mob respawn ordering bằng recommendation này.


<a id="identity-death-receipts"></a>

## MobIdentity credit/source provenance — TARGET engineering contract

Game Server death result pin `deathId/mobIdentityRef/variant/life/MapId` và từng recipient's active QuestId/objectiveGroup trước reward/step transition. Standard kill eligibility theo [Quest owner](../01-design/quests-and-narrative.md#mob-identity-credit), không SpawnGroup whitelist hoặc quest-map gate; special marker/variant/Boss giữ typed predicate. Spring không query mobs hoặc chạy damage; validate trusted server payload/lease/definition revision/constraints rồi commit result.

Group/slot/sourceMap/death position giữ provenance/recovery, không credit whitelist. Mob collection Q8/Q10/Q11 material lưu actual deathMap/anchor, gồm Q10 ở Bạch Vân; fallback cùng sourceMap. **RNG outcome record** pin `(deathId,characterId,questEpoch,objectiveGroup,objectiveId,definitionRevision)` + eligible snapshot + success/failure + immutable payload/entitlement refs. Cả fail và success committed dedup; retry/death replay không roll lại, full bag không đổi outcome. Source identity cho NPC grant là `(characterId,QuestId,questEpoch,grantId)` + npc/map provenance, không bắt qualifyingOrdinal/deathId. Q4 tutorial #5 ordinal riêng vẫn cấp đúng một áo/một sample.

Bounded pending rights xét owned bag+unconsumed outstanding rights, serialize concurrent deaths; claim quantity không double count. No rights after N, không dùng TTL hoặc consume để reset RNG budget. Committed failure/outcome retention phải bounded theo quest epoch + death receipt retention tại G-D; không DB row vô hạn mỗi kill sau objective cap.

Q11 restoration grant pin ba distinct fragment payloads; Pending/fullbag giữ đúng các quyền chưa claim, không duplicate batch sau partial claim/reconnect. Exact delivery encoding là engineering spike, không chọn material consume boundary còn OPEN. Placement consume đúng fragment + placed flag + receipt/revision atomic; replay trả old result, flags không mất khi vật đã consume. Gate đọc committed flags + approved Completed/access policy; không đánh dấu Completed hoặc mở toàn world theo UI/missing fragment. Endpoint/ordered policy/level theo [Quest OPEN](../01-design/quests-and-narrative.md#q11-restoration). Q12 specific Boss/life/10%/corpse/reset giữ.

**OPEN crash boundary:** immutable result trong RAM chưa là durable outcome. Không publish ground/reward/collection success trước ACK; committed failures/successes được replay, backend recovery không reroll. Process crash trước handoff/commit vẫn cần TECH-01 chứng minh lethal/death retry giữ original outcome hoặc abort uncommitted event theo policy đã duyệt; không tuyên bố persisted death guarantee từ queue RAM, không restore actor cũ rồi roll cùng deathId lại. Regular outcome cùng boundary, không riêng quest path.

**P0 candidate cho G-D, chưa chọn policy:** commit-before-reward; process crash thì reconcile receipt để phân biệt committed-but-ACK-lost với thật sự uncommitted. Receipt chưa thấy hoặc query timeout chưa đủ kết luận abort: phải fence/reconcile request từ writer cũ còn in-flight, giữ unknown-commit policy. Khi xác nhận event không commit và không thể commit muộn, abort death event cũ và tạo normal lives mới theo World recovery; không replay/reroll deathId đó. Nếu đã commit, khôi phục đúng receipt/result/pile deadline, không grant thêm. Cách này không cần cứu RNG chưa commit bằng journal riêng, nhưng có thể mất reward của death vừa thấy trước crash. G-D cần test trước/sau commit/ACK, atomic N recipients và A15 terminal clock/cap ordering trước durability acceptance.

Definition revision **cần đổi trong implementation sau**, không migration SQL trong lượt docs. Fixtures cũ counts/group whitelist/virtual collection không tự đủ reward: reset dev hoặc approved import mapping preserving committed receipts/rights, không hồi tố pre-step kills hoặc nhân ordinary rewards. Không có production saves để migrate hôm nay; exact encoding/unique constraints vẫn G-D. Tests: wrong identity cùng rig, same identity khác group/map-of-quest, player/mob khácMap, repeated slot newlife, future step, lostACK/fullbag/TTL/restart, N recipients atomic rollback.

Split/Sort-Merge/Discard: commit quantity/location deltas + receipt/revision atomic; split sinh stack ID mới, không clone source receipt; merge receipt map survivor, Discard không world_loot insert. Same ID/different payload reject; uncertain ACK lookup trước dependent mutation. Map Info là replicated runtime population, không checkpoint authority hoặc durable counts tự cộng từ client.

Q11 NPC batch grant và material consume cần preflight theo đúng boundary được duyệt: nếu consume ở Bách, xét net capacity sau consume, không đòi ô thừa vô ích; nếu ở Lâm, receipt/epoch phải nối sang Bách để không tiêu lần hai. Pending fragments đã claim không regrant, từng placement đúng bound identity. Exact full-batch vs partial delivery UX OPEN engineering spike, không báo restoration complete chỉ vì đã tạo Pending rights.
