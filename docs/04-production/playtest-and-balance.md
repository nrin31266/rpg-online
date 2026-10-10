# Huyền Lộ — Playtest & Balance

Các số BASELINE/TUNABLE chưa phải nghiệm thu runtime.

**EVIDENCE STATUS:** các bảng DERIVED/SIMULATION 2026-10-06 không phải nghiệm thu runtime; giả định target geometry/population/pacing đã SUPERSEDED bởi recovery 2026-10-09. Script tạm không còn artifact/version cố định nên simulation chưa reproducible dài hạn; không dùng để pass gate. Trạng thái triển khai và gate xem tại [Roadmap](roadmap.md).

## Document owns

Evidence/fixtures, probes/KPI, fresh-run và yêu cầu harness tương lai; không quyết định luật gameplay.

## Document does not own

Luật ngoài domain thuộc owner trong [documentation map](../README.md); evidence thuộc [Playtest & Balance](../04-production/playtest-and-balance.md), thứ tự triển khai thuộc [Roadmap](../04-production/roadmap.md).

<a id="acceptance-routing"></a>
<a id="gdd-13"></a>
<a id="gdd-14"></a>

<a id="10-nghiệm-thu-và-hướng-dẫn-tra-cứu"></a>

## Nghiệm thu và hướng dẫn tra cứu

**Dev Mode chỉ là tooling:** cho test nhanh bằng preset/lệnh dev không thay canonical Q1–Q12, không tạo tiến trình hợp lệ trong production. Fresh-run acceptance phải chạy route thật từ đầu; [Technical](../02-technical/gameplay-runtime.md#dev-mode) giữ cách cô lập tooling, [protocol](#dev-speed-acceptance) phân biệt DEV SPEED và ACCEPTANCE EVIDENCE.

**VS-1 đã được xóa khỏi checkout; source đã commit và evidence cũ chỉ là lịch sử tham khảo, không nghiệm thu TARGET.** G-L theo revision mới cần evidence mới; test/video cũ chỉ ghi behavior của revision cũ. Chi tiết CURRENT/DEFERRED nằm ở Roadmap; không ký hoàn thành P0 khi chỉ Kiếm/offline chạy được.

**Chưa nghiệm thu**: cần playable build / evidence hai Client kết nối Dedicated Game Server (tối thiểu 2 concurrent players), không thay bằng simulation hoặc diễn giải thành capacity tối đa.

| Nhóm | Tiêu chuẩn |
| --- | --- |
| Player | Movement ←/→, Jump ↑, DropThrough ↓; Lv 1–20; reset 20 điểm Lv 5; hai class; 95 điểm Lv 20; Tẩy Mạch không mất dữ liệu |
| Combat | Basic Tân Lữ trước class; ba active tích lũy + hai nội tại / class, manual Lv 5 / 10 / 17, CD riêng/common lock; 1–3 chỉ select, PrimaryAction (combat branch) tạo one-shot approach/cast, giữ không RepeatOnHold; chết giữ valid focus/HUD; primary eligibility / propagation / maxTargets / falloff; snapshot / proximity / explosion không double-hit; Evade / Crit; Bỏng / Băng Hàn đúng target branch, post-thaw protection target-wide |
| World | Năm farm maps, ba support zones; SpawnGroup độc lập, HomeRegion/WalkRegion/Return/respawn; mật độ re-author, solid trực giao/one-way cấu trúc, không slope/climb; đúng bảy fixed-level identities / sáu rigs, natural Linh shared cap/rate theo World owner; Map Info authoritative, một Boss với telegraph / target / reset / Cuồng Mạch |
| Story | Q1–Q12 có setup / objectives / turn-in; thiếu level không auto-chain; READY_TO_TURN_IN không auto trả; Q9 không chặn Q10; Q11 complete mới mở vùng; Q12 per character / Main Story Complete; vòng chơi tiếp tục |
| RPG / art | Food / Potion / Death; túi / kho / shop; 6 ô / 18 dòng / 21 mẫu thường / phẩm chất / giới hạn I+4, II+6, III+8 / chuyển giao cùng bậc hoặc lên bậc kế; modular 64 × 64 / PPU 32 / 26 frames |
| Online | N-player collections; P0 acceptance tối thiểu 2 concurrent players qua LAN; combat / MapExit/SpecialGate / chat / loot MapId validation; co-op; PvP cược 1v1, escrow trước trận, timeout 120 s hòa; Boss contribution 10% và shared pile 90 s |
| Reliability | Login/character binding qua Spring Boot; PostgreSQL lưu tiến trình, recovery checkpoint và escrow/settlement receipt; reconnect ngắn resume phiên còn sống, phiên đã mất dùng SafeAnchor; hai Client + Game Server + backend + database, demo fixtures và script/build/video fallback |

Mọi thay đổi tiến trình lưu ngay qua backend, **enhance fail cũng lưu chi phí**; checkpoint MapId/HP/MP theo chu kỳ và chuyển trạng thái quan trọng, không save từng hit hay toàn world. Persistence contract, demo fixtures và acceptance đầy đủ tại Technical.

> **Implementation:** [Technical — demo / QA](#qa)

<a id="roadmap"></a>
<a id="technical-gates"></a>

<a id="10-gate-kỹ-thuật-và-nghiệm-thu"></a>

## Gate kỹ thuật và nghiệm thu

[Roadmap](roadmap.md#phase-gates) sở hữu thứ tự làm: harvest → feel/art/UI probes → production base → G-L mới → G-N sớm → G-D. Technical giữ cách kiểm; anchor `roadmap` bảo toàn link cũ. Lịch/workload/cut ladder cũ ở [trace](../90-archive/production-history.md#legacy-roadmap), không là evidence đã pass.

**G-L — local production slice:** sau base review, revision mới chạy liên tục Q1–Q6/Tân Lữ→Kiếm trên ba map. Kiểm di chuyển chỉ bằng mũi tên; chọn 1/2/3 không cast, approach hoặc tốn MP/CD; Execute riêng; nhả phím giữ one-shot và giữ phím không repeat. Đổi selection lúc pending chỉ đổi UX; Execute mới thay intent chờ, action đang chạy giữ snapshot. Kiểm cancel thủ công/buffer/arrival validation.

Nhịp S2 và MP từ design owner hiện hành được thử bằng fixture hẹp riêng nếu chưa mở trong route Q1–Q6; fixture không mở rộng story scope G-L hoặc ký pass fresh route. AUTO sticky, EXPLICIT không swap, Tab dùng local set ổn định; thử target trên không/sau lưng/khác tầng, no-target/no-cost và dead-owner focus HUD. PrimaryAction physical binding và non-combat focus đi qua usability probe, chưa khóa.

G-L còn kiểm terrain trực giao, không climb, natural solid và one-way có support; movement ở tốc độ thường; khu chức năng NPC, Q3 tại Bách Luyện và Q6 mentor route. Glyph phải theo context. EdgeExit/SpecialGate/MapId/checkpoint ordering, supply/full bag/death/retry đi cùng revision. Cần video rig/pivot/socket và giờ art/editor/QA/rework thật. RAM adapter không chứng minh persistence; prototype cũ không pass gate mới.

**G-N — Dedicated spike sớm:** Dedicated headless + hai standalone Clients cùng content revision, slice đã đủ ổn. Profile/admission RAM dev và receipt fixtures có thể đo authority/physics/recipients, chưa là final login/save. Không đưa service credential giả vào client. Kiểm sender binding, replay/forged sequence/duplicate connection, map transition, shared kill/quest credit và hai claims tranh một item.

Packet action/status cũ không gắn vào respawn life, UI không nhân effect/result. Owner chết vẫn nhận current/max HP của focus từ hit của người khác. Collections/AI/crowd/loot xử lý N người, không fixed pair. Thử delay/loss, ghi latency/correction; thêm 3–4 Clients để tìm assumptions, chưa claim capacity. Sửa boundary/headless physics trước mở rộng content.

**G-D — backend thật:** PostgreSQL + Spring Boot login/character/ticket + Dedicated + hai Clients. Kiểm ticket một lần, lease/duplicate connection, SafeAnchor/checkpoint/resume/map transition và một death với N recipients/claim idempotent. Q6 commit class/`ClassChosenLevel`/mentor binding chung; chọn Cung muộn rồi reload không giảm HP nền. Retry/outage/crash trước–sau commit/ACK theo các mục liên quan; đo backend latency, sửa ticket/transaction/clock/checkpoint/headless physics trước integration done. MPPM phục vụ iterate; acceptance dùng standalone. G-N RAM pass không thay G-D.

**Setup đích:** Docker PostgreSQL với volume test riêng → Spring migrations/admin-created accounts/characters → Dedicated build lấy backend URL/service credential từ environment/world config → hai Client builds login/chọn character riêng → logs cả bốn tiến trình. Restart server kiểm checkpoint MapId/HP/MP/SafeAnchor, Boss reset/loot receipts và PvP orphan refund100%; tắt Spring/DB để kiểm pause/retry. Không commit password thật hoặc service credential vào client.

Headless target là artifact riêng, cùng content revision/gameplay assembly; chỉ server chạy authority.

G-C/F/P/T mở rộng progression/gear/quest/Linh/Boss/co-op/recovery/PvP/chat/regression/package theo các mục liên quan, gồm **full fresh Q1–Q12**, late class và optional Q9 song song Q10. G-L slice Q1–Q6 không thay toàn journey. Các gate TARGET vẫn CHƯA CHẠY; ghi revision/build/packages/machine/log/video/time/result. Mục tiêu quản lý hai tháng/bốn người ở Roadmap chỉ được ước lượng lại từ giờ integration/art/editor/QA thật; estimate 160–240h player-host/JSON cũ là LEGACY.

<a id="risks"></a>

<a id="11-rủi-ro-kỹ-thuật"></a>

## Rủi ro kỹ thuật

| Rủi ro | Mức | Evidence/gate và xử lý |
| --- | --- | --- |
| Sustain MP, S2 farm thường xuyên, INT/Potion | CRITICAL | Nhịp/MP/power và HP/MP gear mới thay mô hình cũ. Dùng [current balance probe](#current-balance-probe), đo rotation/Food/Potion/zero-INT; sustain sheets cũ là LEGACY |
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
| Quest supply/routing hoặc Dev Mode rò vào release | HIGH | [Quest NPC/Q11 endpoint owner](../01-design/quests-and-narrative.md#npc-service-review); active-step action credit/physical collection/personal entitlement/full bag/replay. Fixtures không thay fresh journey; quyền/storage dev tách release |
| Observer/network cost | MEDIUM | Đo dead observer đúng MapId/N-player trước optimize P1; chưa công bố capacity |
| Scope P1 tăng ngoài gate | HIGH | Proposal/research không là DoD; chọn sau P0 gate, không framework hóa prototype |



<a id="pending-ordering-probes"></a>

## Probe pending/clock — TECH-01, SAVE-01, A15

Commit/retry invariants đã có; barrier/serialization/time anchoring/definite-reject recovery còn **OPEN, chưa runtime evidence**. G-D ghi quyết định và test trước reliability done. Bảng là probe thuộc gate, không schema mới đã khóa.

| Chuỗi phải thử | Kết quả cần chứng minh / quyết định còn mở |
| --- | --- |
| Replay/correlation trùng, packet life cũ | Authority giữ action/result IDs; life mới không nhận result/status cũ, recipient không đổi chéo; ghi network/delay/loss/seed |
| Hai claims, một ACK trễ | Một inventory nhận item, loser AlreadyClaimed, retry receipt cũ; RAM G-N không chứng minh crash durability |
| LethalHP0 → terminal-pending → ACK sau respawn deadline | Chưa finalize không reward/respawn; chốt `deathUtc`, release cap/due ordering. Nếu neo t0HP0 thì giữ t0, không đặt lại từ ACK/corpse. Art t0 chỉ giả định A15 |
| Potion ordering | Realtime acceptance không đợi ACK; replay/crash/outage theo [POT-01](#potion-acceptance) |
| Timeout unknown / commit mất ACK / definite reject | Unknown retry cùng immutable payload/key; receipt trả committed result. Reject không reroll/regrant/retry vô hạn; chốt canonical reconcile/close/pending recovery theo lease và thông báo |
| Periodic đến sau transition/death; restart/clock jump | Sequence/generation chặn stale; chốt session↔UTC anchor. Loot giữ UTC gốc; normal/Boss restart semantics các mục liên quan; UI countdown không authority |

Ghi state/IDs/revisions trước–sau/expected outcome; latency/correction kèm cảm giác chơi là số đo, chưa có ngưỡng performance/capacity approved. G-N correctness với fixtures; G-D PostgreSQL thật/lỗi trước–sau commit. TECH-01/SAVE-01/A15 ở [Online & Persistence](../02-technical/online-and-persistence.md) giữ OPEN cho đến evidence, không pass vì có checklist.


<a id="qa"></a>

<a id="12-demo-qa-và-acceptance"></a>

## Demo, QA và acceptance

P0 cần ít nhất hai concurrent Unity Clients trên một Dedicated Game Server, Spring/PostgreSQL phục vụ cả hai. Fixtures không giới hạn registry/spawn. QA với 3–4+ players ghi machine/build/packages/CPU/frame time/bytes/messages/latency trước capacity claim; không cần Party/Channel.

EditMode kiểm EXP/points/evaluator/`ClassChosenLevel`, reward/quest result construction, PvP quota và checkpoint ordering. PlayMode kiểm multi-actor drop, terrain/transition/camera/MapId/SafeAnchor, action timeline và pool reuse. Spring integration dùng PostgreSQL thật để kiểm ticket/lease/migrations/item/quest/death/claim/class/escrow, replay/outage/settlement/reconciler. Escrow hai người cùng thành công hoặc cùng rollback.

Multiplayer iterate bằng MPPM; acceptance là standalone Dedicated + Clients. Evidence thủ công về feel/input/rig/UI ở tốc độ thường vẫn cần.

**Mọi TARGET Dedicated/backend dưới đây CHƯA CHẠY.** Prototype evidence chỉ có giá trị theo revision trong prototype README; fixture local không ký pass target. design owner sở hữu số balance/catalog/quest; Technical kiểm invariant và source mapping, luôn dùng current definitions khi retune.

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
| Skill/status | Nhịp/MP/power S1/S2/S3 hiện hành từ design owner; probe farm S2 thường xuyên, CD từng SkillId/common lock, học S2 giữ S1 và CD cũ, không Normal Attack sau class. Spread ABC/ABA/AAA cùng resolve/stable indices, invalid mất hit/không reacquire; Kiếm primary/proximity và Hàn radius đúng profile owner. Roll mỗi unique target kể cả fail; Bỏng refresh source/expiry giữ nextTick; Freeze protectedUntil; Boss remaining wait không reset action; PvP Slow chỉ movement. Một/hai/bốn Cung không nhân debuff | Một clock, action/target/generation/status logs; không AnimationEvent |
| Mob/AI/crowd | Capability tái sử dụng, Hybrid OPEN; home/walk/edge không Jump/Drop. Hostile hit wake ngoài passive aggro, group alert không lan cả map, sticky threat. Một/hai/bốn melee thử occupied≠blocked, contact/staging/recovery, tường/mép/N-player/AoE. Cung kite hợp lệ; perch dùng Return, clear ledger/status/action và full HP tại home, phase parameters OPEN; Ong vẫn melee-accessible | Authored region/path/phase/threat/action logs và video |
| World/terrain/timers | Playable surfaces trực giao, không slope/climb; natural solid mass, shallow water và one-way nhân tạo có support. Drop per actor, press mới mỗi tầng và đúng priority. Hai/ba/bốn players farm, local groups dày nhưng độc lập, safe exit strip, không chain whole-map. Seed28/66 LEGACY, current totals OPEN; root occupancy/deadlines/map rỗng/re-entry/respawn idempotent đúng MapId | Layout audit, clock/MapId/run-back/contention logs |
| Boss | Boot/restart đúng một Alive Boss. Stats/nhịp/respawn từ design owner, demo override tách; Q12 không spawn. Một action, ba vùng đá không double-hit; Cuồng Mạch chỉ future cadence, Slow phần chờ còn lại. Threshold10% derive MaxHP, corpse eligibility, một pile, không direct EXP/Vàng; loot windows12–30–90s | Lifecycle/clock/credit/claim logs với hai/bốn players |
| Reward/loot | Phần contribution mỗi người floor, không chia lại; snapshot trước reward/level-up. Level suppression không gây quest softlock. TopDamage whole-life không đủ level thì không regular set/fallback. Shared windows8–20–60s; claim race/full bag/late join/level sau kill, một item chỉ một claim và crash receipt | N-recipient payload/revision/transaction/deadline logs |
| Catalog/equip/upgrade | 21 stable templateIDs/names/equip gates/pools/source theo design owner. HP ở Armor/Pants/Boots, MP ở Weapon/Ring/Necklace; rarity/enhance/flat/Tinh Hoa đúng primary lists/thứ tự. Fixed ACC/EVA/Crit/speed không rarity multiply; sáu class weapons. Transfer cùng/next band, cùng slot/class/equip level; no-gain reject, source consumed/target ID giữ; receipt replay/race source, failure cost/cap | Definitions/preview/runtime/reload tại+0/+4/+8, ownership và DB failure |
| Linh/Q8 | Natural shared spawn/respawn; không quest-specific promotion/guarantee; đúng identity+variant credit≥20%, Map Info0→1→0 không tiết lộ vị trí; đo scarcity/outsider/contention | Q8-01 logs; CHƯA CHẠY |
| Quest Q1–Q12 | Q3=4/5placements; Q4=5/tutorial#5; Q5=8; Q8 CollectDấu→Kill1Linh, Q10 CollectVậtChứng, Q11 materialRNG→Lâm→Bách/gate/grant→3placements nhiều map OPEN; Q12=10+Boss10%. Same identity/sameMap/active snapshots; personal rights/bag/consume/placement flags; Q6 bothTalk/Q9optional giữ | Q-F01..12, outcomes/grant/placement/turn-in receipts, full-journey video; CHƯA CHẠY |
| PvP/chat | Mười stakes, accept exact, hai participants, escrow cả hai hoặc không ai/pre-Arena checkpoint. Countdown/ACTIVE ACK;120s DRAW không so HP;HP0 WIN/disconnect ACTIVE FORFEIT/pre-ACTIVE cancel/hai disconnect cùng tick SYSTEM_ABORT. Food real clock, quota theo design tăng tại realtime acceptance, item durability riêng và Hồi Sinh Phù reject; PvP scale/Slow/chat80charMapId/rate từ design owner | W1000:WIN1800/fee200,DRAW900 mỗi người,abort1000 mỗi người;Journey200 một lần,Q9 không FORFEIT/abort; match/receipt hai Clients |
| Potion ordering | Realtime acceptance không đợi ACK; replay/crash/outage theo [POT-01](#potion-acceptance) | G-D/G-T: chưa chạy |
| Art/UI/Dev Mode | Art import theo current26frames/rig/pivot/phase/socket/flip/foreground/overlay, không Climb. Glyph semantic select/Execute/NPC focus/dead-target HP. Dev permission/test storage/labels/reset fresh/clear callbacks life cũ; release không dev, preset không thay fresh journey | Import audit, video tốc độ thường, build config và reset logs |

Đóng gói hướng dẫn PostgreSQL → Spring → Dedicated → hai Clients, seed/reset test có chủ đích và video fallback. Build/packages/content revision, kết quả từng ca và vấn đề OPEN phải rõ. Video fallback không thay acceptance.

<a id="balance-baselines"></a>

<a id="1-phương-pháp-đối-chiếu-và-giới-hạn"></a>

## Phương pháp, đối chiếu và giới hạn

Power/MP/CD và HP-MP trang bị đã đổi sang baseline để thử trong design owner. Các bảng lịch sử bên dưới dùng cấu hình cũ; chỉ [probe hiện hành](#current-balance-probe) và bảng cường hóa được tính lại cho revision này, không thay thế design owner. **TÍNH TỪ LUẬT** là phép tính xác định; **MÔ PHỎNG** phụ thuộc giả định; **ĐỀ XUẤT** chưa là luật.

PvP có Food/Potion và recovery checkpoint mới nên mô hình PvP cũ chỉ là đối chiếu sát thương trực tiếp, không dự báo thắng/hòa; farm scheduler cũ cũng cần rerun vì combat/resource đổi. Các mô hình dưới đây không nghiệm thu runtime; kết quả bản mẫu nằm tại [Production history](../90-archive/production-history.md#prototype-runtime-history).

**Nguồn đầu vào:** [nhân vật](../01-design/combat-and-character.md#character-power), [kỹ năng/trạng thái](../01-design/combat-and-character.md#class-combat), [quái và bãi](../01-design/world-and-content.md#world-farm), [trang bị/thưởng](../01-design/items-and-economy.md#gear-economy), [Food/Bình](../01-design/items-and-economy.md#consumables-death). Playtest & Balance không giữ catalog thứ hai.


<a id="character-evidence"></a>
<a id="combat-analysis"></a>

<a id="2-nhân-vật-chiến-đấu-và-hiệu-ứng"></a>

## Nhân vật, chiến đấu và hiệu ứng

<a id="current-balance-probe"></a>

## Probe 2026-10-06 — DERIVED / SIMULATION lịch sử, chưa nghiệm thu

**Giả định → phép tính → kết luận:** dùng baseline owner design tương ứng; nhập phái ở Lv 5, trừ ca chọn muộn riêng. Phân điểm cân bằng như bảng dưới, đủ bộ Common +0 theo cấp (I ở Lv 5/10, II ở Lv 13, III ở Lv 17/20). Bộ đồ chỉ là fixture kiểm chỉ số, không phải inventory được cấp cho người chơi. S1/S2/S3 có CD độc lập, chi phí mới và chung thời gian khóa action; không có đòn Normal sau nhập phái. Giá/nguồn, Food/Bình, HP quái và thưởng không đổi. Các số chỉnh lại đều **BASELINE/TUNABLE**, chưa LOCKED.

### HP Cung và HP/MP của gear

Cung tăng HP chậm hơn **sau cấp nhập phái thực `ClassChosenLevel`**, giữ HP nền đã tích lũy; Kiếm/Tân Lữ tiếp tục tăng +10. Chọn Cung Lv 5 có hệ số probe +8/cấp về sau, không khóa đề xuất +7. Chọn muộn Lv 10 giữ HP nền 210 tại giao dịch, Lv 11 mới lên 218; nếu cứ trừ theo Lv 5 sẽ mất 10 HP ngay lúc chọn, nên không dùng công thức đó. VIT +8/điểm không đổi, HP gear vẫn là lựa chọn build; Technical lưu cấp nhập phái cùng class receipt. Cộng/reset/equip chỉ clamp current HP/MP, không hồi theo tỷ lệ.

| Lv / điểm STR–VIT–INT–AGI | HP nền Kiếm / Cung (chọn Lv 5) | MaxHP Kiếm / Cung | MaxMP Kiếm / Cung |
| --- | --- | --- | --- |
| 5 / 5–5–5–5 | 160 / 160 | 312,4 / 284 | 143 / 157,3 |
| 10 / 11–12–11–11 | 210 / 200 | 429 / 380 | 193 / 212,3 |
| 17 / 20–20–20–20 | 280 / 256 | 776,6 / 682 | 338 / 371,8 |
| 20 / 24–24–24–23 | 310 / 280 | 844,8 / 738 | 370 / 407 |

Lv 5 không có chênh HP nền ở class choice. Lv 20 cân bằng Cung thấp hơn Kiếm 106,8 HP, gồm tăng HP chậm hơn và nội tại Kiếm Tâm; Cung vẫn có range/kite và Ưng Nhãn. Không cố cân từng encounter 50/50 hoặc giảm hệ số VIT riêng của Cung.

| Gear contribution Common +0 | I trước → mới | II trước → mới | III trước → mới |
| --- | --- | --- | --- |
| HP từ Armor/Pants/Boots | 65 → 84 | 145 → 188 | 205 → 266 |
| MP từ Weapon/Ring/Necklace | 20 → 42 | 40 → 76 | 60 → 114 |

HP tăng khoảng 29–30% **trên phần gear**; Lv 20 MaxHP Kiếm cân bằng tăng 777,7→844,8 (+8,6%). MP gear tăng mạnh theo tỷ lệ vì có hai dòng mới từ nền nhỏ; MaxMP Kiếm Lv 20 316→370 (+17,1%), Cung 347,6→407 (+17,1%), không tăng gấp đôi tài nguyên nhân vật. Armor giữ HP mạnh, Pants vừa, Boots nhẹ; Necklace MP mạnh hơn Weapon/Ring. Không suy stat từ bên trái/phải layout.

**Cực đoan Lv 20 Common III+0 — cùng 95 điểm:**

| Build | MaxHP Kiếm / Cung | MaxMP Kiếm / Cung | Ý nghĩa |
| --- | --- | --- | --- |
| Dồn STR | 633,6 / 546 | 250 / 275 | Sát thương cao, vẫn phải quản lý MP/đòn nhận |
| Dồn VIT | 1469,6 / 1306 | 250 / 275 | VIT giữ hiệu quả bằng nhau, không khóa progression |
| Dồn INT | 633,6 / 546 | 725 / 797,5 | MP và skill bonus cao, không tự tăng HP |
| Dồn AGI | 633,6 / 546 | 250 / 275 | Tăng hit/evasion, không tăng HP/MP qua AGI |
| Không VIT | 633,6 / 546 | 410 / 451 | HP gear giúp nhưng không cam kết chịu overlap |
| Không INT | 915,2 / 802 | 250 / 275 | Gear MP/Food hỗ trợ S2; weave vẫn tốn bình |

### Rarity / enhancement / Tinh Hoa — tính lại đủ biên

HP/MP mới thuộc primary list, cùng thứ tự nhân rarity → enhance → cộng flat/Tinh Hoa → cộng nền/điểm → nội tại một lần. Không nhân Crit/tốc chạy; ACC/EVA chỉ chịu rarity và flat hiện hành. Trần I+4/II+6/III+8, giá, tỷ lệ và milestone không đổi. Ví dụ Rare III Áo +8: HP 138×1,16×1,59+10 = 264,5272; DEF 15×1,16×1,59+2 = 29,666. Weapon/Ring có MP chịu cùng hệ số; Boots có HP chịu cùng hệ số, không sinh Tinh Hoa mới.

| Full III, Lv 20 cân bằng | MaxHP Kiếm / Cung | MaxMP Kiếm / Cung | ATK |
| --- | --- | --- | ---: |
| Common +0 | 844,8 / 738 | 370 / 407 | 91,6 |
| Common +4 | 923,1 / 809,18 | 406,22 / 446,84 | 100,8 |
| Common +8 | 1044,93 / 919,94 | 447,26 / 491,99 | 115,2 |
| Rare +4 | 980,68 / 861,53 | 428,66 / 471,52 | 108,67 |
| Rare +6 | 1038,38 / 913,98 | 451,14 / 496,25 | 116,56 |
| Rare +8 | 1119,37 / 987,61 | 476,26 / 523,89 | 125,38 |
| Epic +8 | 1161,24 / 1025,68 | 492,58 / 541,83 | 131,1 |

Đủ bộ Rare/Epic +8 là biên sau truyện, không phải điều kiện Q12. [Bảng tra Common từng món](#gear-upgrade-values) được tính lại toàn bộ; không dùng số HP/MP cũ của các mô hình lịch sử để nghiệm thu evaluator mới. ATK không đổi nên không có lý do tăng giá, HP quái hoặc drop chỉ vì thêm MP/HP.

### Cadence, animation và vai trò S1/S2/S3

CD probe 0,60/0,90/6 s nằm trong hướng S1 0,5–0,7; S2 0,7–1,0; S3 5–7, **không khóa những khoảng brainstorm**. Cost S1 giữ 2; S2 giảm 4→3 để kiểm farm thường xuyên; S3 giữ 16. Cung S2 giảm tổng power 2,4→1,8 qua ba indices 0,70/0,60/0,50, bù một phần tăng tần suất, tránh dùng nguyên burst cũ ở CD ngắn. Kiếm S2 giữ 1,35 mỗi victim, max3 để giữ lợi thế cluster.

| Profile trên một victim | Power/cast | Power/MP | Power/CD (trước INT/DEF/crit) | Action lock/CD |
| --- | ---: | ---: | ---: | ---: |
| Kiếm S1 | 1,20 | 0,60 | 2,00 | 0,30/0,60 = 50% |
| Kiếm S2 | 1,35 | 0,45 | 1,50 | 0,30/0,90 = 33,33% |
| Cung S1 | 1,15 | 0,575 | 1,9167 | 0,30/0,60 = 50% |
| Cung S2 AAA | 1,80 | 0,60 | 2,00 | 0,34/0,90 = 37,78% |
| S3 hai phái | Theo shape design owner | Theo số victim | CD 6 s; không full DPS độc lập | 0,40/6 = 6,67% |

Cung AAA hơn S1 khoảng 4,35% power/MP và power/CD; S1 vẫn commit 2 MP, CD ngắn hơn và ít overkill. Kiếm S1 tốt cho đơn, S2 tốt cho cụm (ba victim: tổng power 4,05/0,90 = 4,50/s trước giảm trừ). Đây là role direction, chưa chứng minh không có nút bị lép vế. S2 phải được đầu tư pose/release/impact và VFX sạch vì có thể bấm rất thường xuyên, không chỉ dành art cho S3.

Cộng thời gian chiếm lock lý tưởng của ba kỹ năng được 90% với Kiếm/94,44% với Cung, nhưng CD có thể trùng lúc và chỉ một action chạy. Không cộng power/CD để gọi là DPS luân phiên thực. HitMoment/resolve/action lock giữ baseline trước; Attack 3 hình ở 12 FPS dài 0,25 s, Skill 4 hình ở 12 FPS dài 0,333 s. Art phải đặt pose, đoạn giữ và hồi động tác theo đồng hồ/action đã nhận, không tăng lock chỉ để chạy hết clip; lock S2 Cung 0,34 s không phù hợp động tác kéo cung dài 1 s. Giữ trọng lực/quán tính; quyền S2/S3 trên không còn OPEN.

### MP/s, Food/Potion và sustain

Food hồi trung bình MaxMP × 0,75%/1%/1,25% mỗi giây ở bậc I/II/III; tick thực mỗi 2 s. Demand = cost/CD là trần nhu cầu khi bấm đủ nhịp thuận lợi, chưa trừ đi đường, hụt hình đòn, chỉnh vị trí hoặc CD trùng lúc. Dấu âm là Food hồi dư; dương cần MP có sẵn/bình/nghỉ. Chỉ S1 ở Lv 5, S2 từ Lv 10, S3 sau Q11.

| Lv / kiểu bấm | MaxMP Kiếm / Cung | Food MP/s Kiếm / Cung | Demand MP/s | Thiếu MP/s Kiếm / Cung |
| --- | --- | --- | ---: | --- |
| 5 / S1 | 143 / 157,3 | 1,0725 / 1,1798 | 3,3333 | 2,2608 / 2,1536 |
| 10 / S1 | 193 / 212,3 | 1,93 / 2,123 | 3,3333 | 1,4033 / 1,2103 |
| 10 / S2 | 193 / 212,3 | 1,93 / 2,123 | 3,3333 | 1,4033 / 1,2103 |
| 17 / S1 | 338 / 371,8 | 4,225 / 4,6475 | 3,3333 | -0,8917 / -1,3142 |
| 17 / S2 | 338 / 371,8 | 4,225 / 4,6475 | 3,3333 | -0,8917 / -1,3142 |
| 17 / S2+S3 | 338 / 371,8 | 4,225 / 4,6475 | 6 | 1,775 / 1,3525 |
| 17 / luân phiên cả ba | 338 / 371,8 | 4,225 / 4,6475 | 9,3333 | 5,1083 / 4,6858 |
| 20 / S1 | 370 / 407 | 4,625 / 5,0875 | 3,3333 | -1,2917 / -1,7542 |
| 20 / S2 | 370 / 407 | 4,625 / 5,0875 | 3,3333 | -1,2917 / -1,7542 |
| 20 / S2+S3 | 370 / 407 | 4,625 / 5,0875 | 6 | 1,375 / 0,9125 |
| 20 / luân phiên cả ba | 370 / 407 | 4,625 / 5,0875 | 9,3333 | 4,7083 / 4,2458 |

Lv 20 không INT: MP 250/275; Food III hồi 3,125/3,4375 MP/s. Chỉ S2 cần 3,3333 MP/s: Kiếm thiếu 0,2083, Cung dư 0,1042; thêm S3 đưa trần lên 6,0, thiếu 2,875/2,5625. Trang bị hỗ trợ farm; INT vẫn có ích khi dùng đòn mạnh/luân phiên. Không Food thì không tự hồi; hồ sơ cân bằng chỉ S2 tiêu pool 370/407 MP trong khoảng 111/122 s nếu bỏ mọi hồi phục.

Bình III hồi 222/244,2 MP; hồi chiêu MP 8 s tách hồi chiêu HP 8 s. Hồ sơ cân bằng Lv 20 bấm S2+S3 đủ nhịp thiếu 1,375/0,9125 MP/s → một bình khoảng mỗi 161/268 s khi đã ổn định, chưa tính pool đầu. Luân phiên cả ba ở trần thiếu 4,7083/4,2458 MP/s → khoảng 47/58 s/bình, tốn xấp xỉ 36.649/30.044 Vàng/h **nếu dùng hết lượng hồi**, cộng Food III 4.200 Vàng/h. Đây không phải cam kết đủ Vàng: mô hình Vàng/h cũ mất hiệu lực khi mật độ/nhịp đòn đổi. Ít thời gian đánh thực hơn sẽ giảm nhu cầu; không tự tăng Vàng/drop để bảo đảm luân phiên liên tục.

[TTK, Boss sensitivity và PvP arithmetic 2026-10-06](../90-archive/design-history.md#ttk-và-group-clear--mô-phỏng-lịch-sử-24-seeds-superseded-cho-targetingpacing) giữ nguyên tại history; chưa rerun targeting/pacing.

<a id="farm-progression"></a>
<a id="world-economy-analysis"></a>

<a id="3-đường-farm-và-tranh-chấp-bãi"></a>

## Đường farm và tranh chấp bãi

## Lv 1 → 20 — lộ trình tính từ luật

Mỗi loại quái có một level cố định. HP/EXP/Gold derive từ design owner; band dưới là **loot source**, không đòi full set. Tier Food/Potion dùng theo player level có thể khác Potion rơi từ source. Ở Lv 20, EXP = 0 dù Cổ Vệ có base EXP 108. Map gates/quest markers tại design owner.

| Player Lv | Map / cụm nên farm | Mob Lv | HP / EXP / Gold | Gear drop / Potion-Food dùng | Lý do chuyển bãi |
| --- | --- | --- | --- | --- | --- |
| 1 | Vân Khê | Talk | — | — / I | Q1 nhớ NPC / đường về |
| 2 | Học Viện | Movement | — | — / I | Q2 mini journey; exact content PROPOSAL |
| 3 | Học Viện → Đồng DS2 | Dummy → Nấm Lv 2 | Dummy 60 / 0 / 0; Nấm 48 / 15 / 7–12 | Mộc + Quần I; supply Áo I / I | Q3 không EXP; Q4 Nấm/loot/equip/sell catch-up Lv 4 |
| 4 | Đồng DS3–DS6 | Sói Sương Lv 4 | 107 / 22 / 11–18 | I, không Weapon / I | Q5 Food/thuốc + 8 Sói, turn-in catch-up Lv 5 |
| 5 | Học Viện Q6 → Trúc TA1–3 | Sói Sương Lv 4 | 107 / 22 / 11–18 | I, không Weapon / I | Q6 class/manual; không quay lại Nấm làm tutorial mới |
| 6 | Trúc TA1–3 hoặc TA4 / 6 | Sói Sương Lv 4 | 107 / 22 / 11–18 | I, không Weapon / I | Sói Lv 4 còn thưởng; Sói Lv 8 khó hơn nếu chọn |
| 7 | Trúc TA4 / TA6 | Sói Trúc Lv 8 | 339 / 38 / 19–30 | I / I | Q7 nhẫn +1; chuyển Sói Lv 8 trước mốc Lv 8 |
| 8 | Trúc TA4 / 6 → Bạch BV1 / 2 | Sói Trúc Lv 8 | 339 / 38 / 19–30 | I / I | Q8 natural Linh / book / gates, chưa auto tiến cảnh |
| 9 | Bạch BV1 / 2 | Ong Lv 10 | 473 / 47 / 23–36 | II chưa mặc / I | Luyện trước tiến cảnh, Sói Lv 8 vẫn hợp lệ |
| 10 | Bạch BV1 / 2 → BV3–5 | Ong Lv 10 | 473 / 47 / 23–36 | II chưa mặc / II | Học tiến cảnh; Đoạt Lv 13 là lựa chọn khó hơn |
| 11 | Bạch BV1 / 2 / BV3–5 | Ong Lv 10 | 473 / 47 / 23–36 | II mặc được / II | Mốc gear riêng sau tiến cảnh Lv 10; chọn nâng I hay thay II |
| 12 | Bạch BV3–5 / Xích XN1–3 | Đoạt Lv 13 | 704 / 63 / 29–45 | II / II | Giữ/mua thêm II; ngoại vi Xích, Q9 optional |
| 13 | Xích XN1–3 | Đoạt Lv 13 | 704 / 63 / 29–45 | II / II | Nội tại II tự mở; không active Lv 13 |
| 14 | Xích XN1–3 hoặc XN4–6 | Đoạt Lv 13 | 704 / 63 / 29–45 | II / II | Đoạt Lv 13 gần cấp; Thạch Lv 16 nếu đủ sức |
| 15 | Xích XN1–XN3(Q10) → XN4–6 | Thạch Lv 16 | 974 / 81 / 35–54 | II / III | Q10 CollectRNG Đoạt Lv13; Food III |
| 16 | Xích XN4–6 | Thạch Lv 16 | 974 / 81 / 35–54 | II / III | Core / Bỏng / position, chưa big |
| 17 | Xích XN4–6(Q11) → Huyền HT1 | Thạch Lv 16 | 974 / 81 / 35–54 | II; Q11 Rare III Weapon / III | Material farm ThạchLv16; restored placements nhiều map OPEN; big reward timing theo endpoint OPEN |
| 18 | Huyền HT1 hoặc HT2–5 | Cổ Lv 20 | 1393 / 108 / 43–66 | III / III | Cổ Lv 20 khó hơn; HT1 Thạch Lv 16 vẫn full reward |
| 19 | Huyền HT2–5 | Cổ Lv 20 | 1393 / 108 / 43–66 | III / III | Big farm; III từ Cổ, Thạch Lv 16 vẫn trong 3 cấp |
| 20 | Huyền HT2–5 / Boss | Cổ Lv 20 | 1393 / 0 / 43–66 | III / III | Cap: 0 EXP; Thạch Lv 16 không thưởng farm; Q12 / endgame |

Lv 5–7 có thể chọn Sói Lv 8 trong gap 3, nhưng không đảm bảo survival; Lv 8 không còn thưởng từ Sói Lv 4. Ong Lv 10 bắt đầu rơi Band II: Lv 10 có thể giữ trong bag, Lv 11 mặc được; mốc mở ngoại vi Xích Nham vẫn là Lv 12. Lv 17 đánh Thạch Lv 16 vẫn gear II; weapon Q11 và Cổ Vệ dẫn sang III. Quest muộn quay về quái thấp vẫn lấy objective/evidence, không lấy regular budget ngoài khoảng.


<a id="economy-analysis"></a>
<a id="loot-consumable-analysis"></a>
<a id="quest-progression"></a>
<a id="boss-availability"></a>

<a id="4-trang-bị-kinh-tế-nhiệm-vụ-và-hành-trình"></a>

## Trang bị, kinh tế, nhiệm vụ và hành trình

## Trang bị: sensitivity và evaluator evidence

Input dùng [Items catalog/enhance/transfer](../01-design/items-and-economy.md#gear-economy); baseline classification theo owner, không bảng luật thứ hai.

**Kiểm biên Common +0:** Với hồ sơ cân bằng tại Lv 5/13/20 và quái thường cùng cấp (EVA theo design owner), Chí mạng Kiếm mới tăng kỳ vọng sát thương trực tiếp khoảng 0,24/0,48/0,73%; Chính xác Cung giảm tỷ lệ bị né khoảng 0,29/0,27/0,27 điểm %. Đây là khác biệt nhỏ trước mô phỏng kỹ năng/di chuyển; Giày tăng tốc chạy tuyệt đối 1/2/3 điểm % nhưng không đổi nhịp đòn hoặc hồi chiêu. Không dùng phép kiểm này để khẳng định TTK mới.


<a id="gear-upgrade-values"></a>

## Bảng tra đủ chỉ số 21 mẫu trang bị thường + Mộc Kiếm

Bảng này **tính từ luật owner design tương ứng**, chỉ cho phẩm chất **Common**; không phải catalog hay luật thứ hai. Mỗi ô là chỉ số **của riêng món đồ**, trước khi cộng nền nhân vật, điểm thuộc tính và nội tại phái. Kiếm cùng bậc thêm Chí mạng cố định, Cung thêm Chính xác chịu phẩm chất nhưng không chịu hệ số cường hóa; Giày thêm tốc chạy cố định. Tinh Hoa cộng sau các chỉ số này; Chí mạng và tốc chạy hiển thị theo điểm phần trăm. Số lẻ là giá trị nội bộ; UI mới làm tròn.

Tinh Hoa I đã cộng tại +4; Tinh Hoa II chỉ có ở bậc III +8. Dấu **—** là cấp vượt trần hoặc không được nâng; các phẩm chất khác dùng đúng công thức design owner.

| Bậc | Món | +0 | +1 | +2 | +3 | +4 | +5 | +6 | +7 | +8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Ngoại lệ | Mộc Kiếm Q3 | ATK 10 | — | — | — | — | — | — | — | — |
| I — Thanh Mộc | Thanh Mộc Kiếm | ATK 15 · MP 10 · Crit 0,5% | ATK 15,75 · MP 10,5 · Crit 0,5% | ATK 16,5 · MP 11 · Crit 0,5% | ATK 17,4 · MP 11,6 · Crit 0,5% | ATK 18,45 · MP 12,3 · Crit 1% | — | — | — | — |
| I — Thanh Mộc | Thanh Mộc Cung | ATK 15 · MP 10 · ACC 10 | ATK 15,75 · MP 10,5 · ACC 10 | ATK 16,5 · MP 11 · ACC 10 | ATK 17,4 · MP 11,6 · ACC 10 | ATK 18,45 · MP 12,3 · ACC 10 · Crit 0,5% | — | — | — | — |
| I — Thanh Mộc | Áo Thanh Mộc | HP 44 · DEF 4 | HP 46,2 · DEF 4,2 | HP 48,4 · DEF 4,4 | HP 51,04 · DEF 4,64 | HP 64,12 · DEF 4,92 | — | — | — | — |
| I — Thanh Mộc | Quần Thanh Mộc | HP 28 · DEF 3 | HP 29,4 · DEF 3,15 | HP 30,8 · DEF 3,3 | HP 32,48 · DEF 3,48 | HP 34,44 · DEF 4,69 | — | — | — | — |
| I — Thanh Mộc | Giày Thanh Mộc | HP 12 · DEF 2 · EVA 4 · Tốc chạy +1% | HP 12,6 · DEF 2,1 · EVA 6 · Tốc chạy +1% | HP 13,2 · DEF 2,2 · EVA 8 · Tốc chạy +1% | HP 13,92 · DEF 2,32 · EVA 10 · Tốc chạy +1% | HP 14,76 · DEF 2,46 · EVA 16 · Tốc chạy +1% | — | — | — | — |
| I — Thanh Mộc | Nhẫn Thanh Mộc | MP 8 · ACC 6 · Crit 1% | MP 8,4 · ACC 8 · Crit 1,2% | MP 8,8 · ACC 10 · Crit 1,4% | MP 9,28 · ACC 12 · Crit 1,6% | MP 9,84 · ACC 18 · Crit 1,8% | — | — | — | — |
| I — Thanh Mộc | Dây chuyền Thanh Mộc | MP 24 · EVA 5 | MP 25,2 · EVA 7 | MP 26,4 · EVA 9 | MP 27,84 · EVA 11 | MP 39,52 · EVA 13 | — | — | — | — |
| II — Vân Nham | Vân Nham Kiếm | ATK 28 · MP 18 · Crit 1% | ATK 29,4 · MP 18,9 · Crit 1% | ATK 30,8 · MP 19,8 · Crit 1% | ATK 32,48 · MP 20,88 · Crit 1% | ATK 34,44 · MP 22,14 · Crit 1,5% | ATK 36,96 · MP 23,76 · Crit 1,5% | ATK 39,2 · MP 25,2 · Crit 1,5% | — | — |
| II — Vân Nham | Vân Nham Cung | ATK 28 · MP 18 · ACC 20 | ATK 29,4 · MP 18,9 · ACC 20 | ATK 30,8 · MP 19,8 · ACC 20 | ATK 32,48 · MP 20,88 · ACC 20 | ATK 34,44 · MP 22,14 · ACC 20 · Crit 0,5% | ATK 36,96 · MP 23,76 · ACC 20 · Crit 0,5% | ATK 39,2 · MP 25,2 · ACC 20 · Crit 0,5% | — | — |
| II — Vân Nham | Áo Vân Nham | HP 99 · DEF 9 | HP 103,95 · DEF 9,45 | HP 108,9 · DEF 9,9 | HP 114,84 · DEF 10,44 | HP 131,77 · DEF 11,07 | HP 140,68 · DEF 11,88 | HP 148,6 · DEF 12,6 | — | — |
| II — Vân Nham | Quần Vân Nham | HP 61 · DEF 6 | HP 64,05 · DEF 6,3 | HP 67,1 · DEF 6,6 | HP 70,76 · DEF 6,96 | HP 75,03 · DEF 8,38 | HP 80,52 · DEF 8,92 | HP 85,4 · DEF 9,4 | — | — |
| II — Vân Nham | Giày Vân Nham | HP 28 · DEF 4 · EVA 8 · Tốc chạy +2% | HP 29,4 · DEF 4,2 · EVA 10 · Tốc chạy +2% | HP 30,8 · DEF 4,4 · EVA 12 · Tốc chạy +2% | HP 32,48 · DEF 4,64 · EVA 14 · Tốc chạy +2% | HP 34,44 · DEF 4,92 · EVA 20 · Tốc chạy +2% | HP 36,96 · DEF 5,28 · EVA 22 · Tốc chạy +2% | HP 39,2 · DEF 5,6 · EVA 24 · Tốc chạy +2% | — | — |
| II — Vân Nham | Nhẫn Vân Nham | MP 14 · ACC 10 · Crit 1,5% | MP 14,7 · ACC 12 · Crit 1,7% | MP 15,4 · ACC 14 · Crit 1,9% | MP 16,24 · ACC 16 · Crit 2,1% | MP 17,22 · ACC 22 · Crit 2,3% | MP 18,48 · ACC 24 · Crit 2,5% | MP 19,6 · ACC 26 · Crit 2,7% | — | — |
| II — Vân Nham | Dây chuyền Vân Nham | MP 44 · EVA 8 | MP 46,2 · EVA 10 | MP 48,4 · EVA 12 | MP 51,04 · EVA 14 | MP 64,12 · EVA 16 | MP 68,08 · EVA 18 | MP 71,6 · EVA 20 | — | — |
| III — Huyền Ấn | Huyền Ấn Kiếm | ATK 40 · MP 26 · Crit 1,5% | ATK 42 · MP 27,3 · Crit 1,5% | ATK 44 · MP 28,6 · Crit 1,5% | ATK 46,4 · MP 30,16 · Crit 1,5% | ATK 49,2 · MP 31,98 · Crit 2% | ATK 52,8 · MP 34,32 · Crit 2% | ATK 56 · MP 36,4 · Crit 2% | ATK 59,6 · MP 38,74 · Crit 2% | ATK 63,6 · MP 41,34 · Crit 2% · ACC 6 |
| III — Huyền Ấn | Huyền Ấn Cung | ATK 40 · MP 26 · ACC 30 | ATK 42 · MP 27,3 · ACC 30 | ATK 44 · MP 28,6 · ACC 30 | ATK 46,4 · MP 30,16 · ACC 30 | ATK 49,2 · MP 31,98 · ACC 30 · Crit 0,5% | ATK 52,8 · MP 34,32 · ACC 30 · Crit 0,5% | ATK 56 · MP 36,4 · ACC 30 · Crit 0,5% | ATK 59,6 · MP 38,74 · ACC 30 · Crit 0,5% | ATK 63,6 · MP 41,34 · ACC 36 · Crit 0,5% |
| III — Huyền Ấn | Áo Huyền Ấn | HP 138 · DEF 15 | HP 144,9 · DEF 15,75 | HP 151,8 · DEF 16,5 | HP 160,08 · DEF 17,4 | HP 179,74 · DEF 18,45 | HP 192,16 · DEF 19,8 | HP 203,2 · DEF 21 | HP 215,62 · DEF 22,35 | HP 229,42 · DEF 25,85 |
| III — Huyền Ấn | Quần Huyền Ấn | HP 88 · DEF 9 | HP 92,4 · DEF 9,45 | HP 96,8 · DEF 9,9 | HP 102,08 · DEF 10,44 | HP 108,24 · DEF 12,07 | HP 116,16 · DEF 12,88 | HP 123,2 · DEF 13,6 | HP 131,12 · DEF 14,41 | HP 154,92 · DEF 15,31 |
| III — Huyền Ấn | Giày Huyền Ấn | HP 40 · DEF 6 · EVA 12 · Tốc chạy +3% | HP 42 · DEF 6,3 · EVA 14 · Tốc chạy +3% | HP 44 · DEF 6,6 · EVA 16 · Tốc chạy +3% | HP 46,4 · DEF 6,96 · EVA 18 · Tốc chạy +3% | HP 49,2 · DEF 7,38 · EVA 24 · Tốc chạy +3% | HP 52,8 · DEF 7,92 · EVA 26 · Tốc chạy +3% | HP 56 · DEF 8,4 · EVA 28 · Tốc chạy +3% | HP 59,6 · DEF 8,94 · EVA 30 · Tốc chạy +3% | HP 63,6 · DEF 10,54 · EVA 32 · Tốc chạy +3% |
| III — Huyền Ấn | Nhẫn Huyền Ấn | MP 22 · ACC 15 · Crit 2% | MP 23,1 · ACC 17 · Crit 2,2% | MP 24,2 · ACC 19 · Crit 2,4% | MP 25,52 · ACC 21 · Crit 2,6% | MP 27,06 · ACC 27 · Crit 2,8% | MP 29,04 · ACC 29 · Crit 3% | MP 30,8 · ACC 31 · Crit 3,2% | MP 32,78 · ACC 33 · Crit 3,4% | MP 34,98 · ACC 35 · Crit 4,1% |
| III — Huyền Ấn | Dây chuyền Huyền Ấn | MP 66 · EVA 12 | MP 69,3 · EVA 14 | MP 72,6 · EVA 16 | MP 76,56 · EVA 18 | MP 91,18 · EVA 20 | MP 97,12 · EVA 22 | MP 102,4 · EVA 24 | MP 108,34 · EVA 26 | MP 114,94 · EVA 32 |

**Kỳ vọng lũy kế từ +0** (mỗi bước hình học độc lập; thất bại tiêu chi phí nhưng giữ cấp). Cột Vàng quy đổi dùng giả định mua toàn bộ đá thiếu ở shop 800 Vàng/viên; không cộng đồng thời với Vàng thuần. Số thập phân là kỳ vọng, không bảo đảm số lần cụ thể.

| Đạt cấp | Tổng lần thử kỳ vọng | Vàng thuần kỳ vọng | Tinh Thạch kỳ vọng | Vàng nếu mua toàn bộ đá |
| --- | ---: | ---: | ---: | ---: |
| +1 | 1,00 | 100 | 1,00 | 900 |
| +2 | 2,11 | 322 | 2,11 | 2.011 |
| +3 | 3,36 | 760 | 4,61 | 4.449 |
| +4 — Tinh Hoa I | 4,90 | 1.606 | 9,23 | 8.987 |
| +5 | 7,12 | 3.495 | 20,34 | 19.765 |
| +6 — trần II | 9,98 | 7.209 | 40,34 | 39.479 |
| +7 | 13,98 | 15.209 | 80,34 | 79.479 |
| +8 — Tinh Hoa II | 20,65 | 36.542 | 173,67 | 175.479 |

Một vũ khí I +4 cần bình quân 9,23 đá; II +6 từ +0 cần 40,34 đá, nhưng **I +4 → II +4** chỉ cần kỳ vọng thêm 31,11 đá cho hai bước +5/+6 và chi phí chuyển 2 đá. III +8 cần thêm 133,33 đá từ +6: mục tiêu sau truyện, không bắt để đánh Q12. **SUPERSEDED loot-model arithmetic:** drop đá thường8% cũ = 12,5 kill/đá nếu chỉ tính quái thường; 640 kill cho kỳ vọng 51,2 đá, thêm khoảng 23 Linh Biến cho ~74 đá sinh ra trước nhặt/tiêu. Mua đá 800 Vàng cho phép bù thiếu nhưng +8 một món từ +0 vượt ngân sách Gold/đá chính tuyến.

Boss có 5–8 đá trong một pile chung, **không** nhân theo số người tham gia. Không cần tăng drop hay sửa giá đá chỉ để biến +8 thành điều kiện story.

**Một phép so cùng ô vũ khí** (chỉ số của món, làm tròn hai chữ số; chưa cộng nền nhân vật/nội tại):

| Món | ATK | Chí mạng Kiếm / Chính xác Cung (riêng vũ khí) | Tinh Hoa / ý nghĩa |
| --- | ---: | ---: | --- |
| Rare II +6 | 45,47 | 1,5% / 23,2 | Tinh Hoa I +0,5 điểm % chí mạng; đáng giữ tới III |
| Common III +0 | 40,00 | 1,5% / 30 | Món mới chưa vượt món II đã đầu tư |
| Rare III +0 | 46,40 | 1,5% / 34,8 | Nhỉnh hơn Rare II +6 về ATK, chưa có Tinh Hoa |
| Common III +4 | 49,20 | 2% / 30 | Vượt Rare II +6 sau đầu tư +4 |
| Rare III +4 | 57,07 | 2% / 34,8 | Giữ phẩm chất Rare và mở Tinh Hoa I; chuyển từ Rare II +6 giữ +6 còn mạnh hơn |
| Rare III +8 | 73,78 | 2% / 40,8 | Thêm Tinh Hoa II +6 ACC; trần săn sau truyện |
| Epic III +8 | 79,50 | 2% / 43,5 | Cao hơn Rare III +8 khoảng 7,8% ATK món, không nhân cả nhân vật |

Rare II +6 Áo mới là **170,776 HP / 14,616 DEF**, so Common III +0 **138 / 15** và Common III +4 **179,74 / 18,45**. Nhẫn Rare II +6 có **27,6 ACC / 2,7% chí mạng**, còn Dây chuyền Rare II +6 **81,456 MP / 21,28 EVA**; vì thế không tự động thay toàn bộ II bằng Common III +0. Trang bị phòng thủ/phụ kiện vẫn có giá trị: bớt bình máu, tăng ổn định trúng/né và Linh lực, dù vũ khí quyết định phần lớn tốc độ hạ quái.


**Chuyển giao giữ nguyên cấp, không nhân đồ:** cùng bậc tốn 800 Vàng, không đá; lên đúng một bậc tốn 500 Vàng +2 đá. Đồ nguồn mất, đồ đích giữ template/phẩm chất/instance; từ chối nếu đích không tăng. Ví dụ Rare II +6 → Epic II +0 thành Epic II +6, tiết kiệm việc đập lại +0→+6 nhưng mất giá bán Rare II (vũ khí Rare II 324 Vàng); opportunity tối thiểu **1.124 Vàng**. II +6 → III +0 tốn 500+2 đá và mất giá bán nguồn, opportunity **2.424 Vàng** nếu đá mua 800; rẻ hơn tự đập III +0→+6 ước 39.479 Vàng gồm đá mua. So sánh chỉ hợp khi đã sở hữu món nguồn; sunk cost cường hóa nguồn không được bỏ qua để gọi chuyển giao là nguồn tạo cấp miễn phí. I→III trực tiếp bị cấm. Không có vòng lặp bán/transfer: mỗi lệnh tiêu một source, target không sinh bản sao, giá bán không cộng tiền cường hóa, no-gain bị chặn; receipt chống replay.

**Đường đầu tư kỳ vọng:** Lv 5 vũ khí I +0; Lv 8 +2; Lv 10 +3; Lv 11 chọn vũ khí II hoặc giữ I +4 tới lúc đủ đá; Lv 13 vũ khí II +4 và nâng Áo nếu chịu đòn nhiều; Lv 15 II +4; Lv 17 trước Q11 II +4/+5, sau Q11 chuyển sang vũ khí Rare III rồi học đại chiêu; Lv 20 chính tuyến thường chỉ vũ khí III khoảng +4..+6, các món khác +0..+2. Đó là hồ sơ kiểm, không là requirement. Band I +4 đáng làm nếu chơi lâu trước II vì Tinh Hoa I và chuyển nguyên +4; II +6 đáng làm nếu sở hữu món tốt và muốn sang III +6. Affordability của route cũ chưa chứng minh supply model mới; không coi đủ Stone/+8 sau quota removal. Xem [EV transition](#loot-model-transition).

## Ngân sách farm và ví dụ kiểm chứng khi nhiều người đánh

**HISTORICAL / SUPERSEDED sau one-outcome loot:** cận trên bán toàn bộ đồ rơi giả định bán trang bị/nguyên liệu/bình/đá trước khi dùng hoặc nhặt hụt; không đồng thời bán và dùng. Food I/II/III tốn **900/2.400/4.200 Vàng/h** nếu hiệu lực liên tục. Theo quái cố định Lv 2/4/8/10/13/16/20, Vàng trực tiếp + cận trên bán đồ khoảng **29,24/34,54/45,16/55,38/63,78/73,88/95,53 Vàng/quái thường**. Ngoài abs level gap 3, regular EXP/Gold/loot/Journey bằng 0. Quest/supply/evidence active step vẫn hoạt động ở level thấp.

Necklace III sell tăng **200 → 225 Vàng**, ngang Boots/Ring III; tổng sell sáu Common III tăng **1.725 → 1.750**. Giá mua II/sell II/ATK và transfer cost không đổi. **HISTORICAL loot rates:** normal Lv20 vendor-all upper từng tăng0,24V/kill vì sell necklace, không estimate current one-outcome pool; xem [EV transition](#loot-model-transition).

**DERIVED — giá/drop không đổi:** Boss expected sell-all pile: **1.000** Thỏi + **303,33** gear + **1.300** Stone + **150** Potion + **75** phù = **2.828,33 Vàng/world death** trước pickup/use; direct Gold/EXP = 0. Respawn 15 phút + fight 90–150 s tạo Thỏi khoảng 3.429–3.636 Vàng/h/world. Q12 turn-in 1.000 riêng từng character, không nhân vật phẩm world theo N người.

**Ví dụ kiểm chứng đóng góp:** mob Lv 4 HP107/EXP22/Gold15; Lv 20 gây 90, Lv 5 gây17. Người cao lệch level ≥4 nhận 0; người thấp nhận floor(22×17/107)=3 EXP, floor(15×17/107)=2 Gold, không nhận cả pool. TopDamage quá gap không tạo regular loot set. Người thấp 15,9% chưa đạt quest credit threshold 20%. Cùng level 70/37 damage nhận 14/7 EXP và 9/5 Gold; lẻ không redistribute.

AFK damage0 không credit; cùng MapId / bán kính 8u / còn sống / đã gây sát thương trong 10s vẫn kiểm từng recipient. Quest chỉ active-step; Boss ≥10% và corpse area là predicate riêng. Không có Party. FFA chỉ nhặt, không sinh EXP/Gold/quest. Level lúc mob chết là bản chụp cho người có đóng góp, nên lên cấp ngay sau kill không làm mất quyền pickup; người tới sau xét level hiện tại.

## Nhiệm vụ, hành trình và Boss

<a id="review-decisions"></a>
<a id="open-decisions"></a>

<a id="5-quyết-định-đã-chốt-và-cổng-kiểm-khi-triển-khai"></a>

## Quyết định đã chốt và cổng kiểm khi triển khai

Decision classification theo [README index](../README.md#open-decision-index) và design/technical owners; bảng dưới giữ câu hỏi đo, evidence và điều kiện review. Không lấy baseline lặp tại Playtest để khóa gameplay.

| ID / trạng thái | Điều cần đo hoặc làm | Baseline hiện tại | Điều kiện xem lại |
| --- | --- | --- | --- |
| BAL-01 — PLAYTEST | Giá trị bốn thuộc tính và build cực đoan | [Combat stats](../01-design/combat-and-character.md#character-power) | Một build làm Q10/Q11 không thể qua dù dùng cơ chế bình thường, hoặc PvP có kết quả lệch quá xa |
| BAL-02 — PLAYTEST | Hồi phục và chi phí Food/Bình | [Items consumables](../01-design/items-and-economy.md#consumables-death) | Trận farm phải đứng chờ Linh lực liên tục hoặc Vàng âm sau route hợp lệ |
| PHY-01 — PLAYTEST | Collider, platform, nhiều người | Bounds design owner, drop-through theo từng actor | Trúng đòn/đi xuyên sàn sai hoặc người khác làm đổi collision |
| CC-01 — PLAYTEST | Đóng Băng/Làm Chậm/Bỏng khi nhiều người | [Combat status](../01-design/combat-and-character.md#combat-status) | Boss mất khả năng ra đòn, người chơi không đọc được hiệu ứng, hoặc overlap gây unfair hit |
| SCOPE-01 — STRONG DIRECTION / TUNABLE | Re-author nhiều pocket độc lập, đường nhánh/cao độ và contention | [World spawn/population](../01-design/world-and-content.md#world-farm), [natural Q8](../01-design/world-and-content.md#q8-bounded-path) | 2 người thiếu quái rõ hoặc 3–4 người chờ nhiều; benchmark trước claim capacity |
| GEAR-01 — LOCKED SEMANTICS / TUNABLE STATS | Chạy lại TTK, hit/crit, di chuyển giữa cụm, hành trình và Boss với Chí mạng Kiếm / Chính xác Cung / tốc chạy Giày; đo cảm giác +4/+6/+8 và preview | owner design tương ứng, bảng kinh tế các mục liên quan, hai Client + Dedicated Server playtest | Gear II/III, sức mạnh hai phái hoặc Tinh Hoa làm Boss/PvP/kinh tế lệch khi chơi thật |
| CONS-01 — BASELINE ĐÃ CHỐT | Thử nhầm phím/bình/refresh Food | HP và MP hồi chiêu riêng; tier đủ bù nhỏ nhất, Food thay cũ | Người chơi thường xuyên phí bình hoặc tutorial Q6 kẹt |
| Potion ordering | Realtime acceptance không đợi ACK; replay/crash/outage theo [POT-01](#potion-acceptance) | G-D/G-T: chưa chạy | G-D/G-T: chưa chạy |
| PVP-01 — ĐÃ CHỐT | Escrow hai người trước MatchId; 10 mức cược; Food tick/Bình quota; outcome và payout idempotent | owner design tương ứng, owner kỹ thuật tương ứng, bảng arithmetic ở trên | HELD/ACTIVE mắc kẹt tiền; hết 120 s vẫn so HP; FORFEIT sai thời điểm; result retry trả Vàng hai lần |
| BOSS-02 — BASELINE ĐÃ CHỐT | Độ rộng vùng/nhịp báo trước đòn | [World Boss](../01-design/world-and-content.md#world-boss) | Không thể né bằng kỹ năng di chuyển thường, overlap khó đọc, thời gian 2 người lệch xa 90–150 s |
| ART-01 — PLAYTEST | Đường đạn, aim, thời điểm hit, hình nhân vật | design owner/Technical art contract | Collider/hình lệch hoặc cảm giác chém/bắn khó đọc |
| LOOT-01 — PLAYTEST | Đá/trang bị sinh ra so lượng nhặt và sink | [Items current one-outcome model](../01-design/items-and-economy.md#regular-loot-outcome), [EV transition](#loot-model-transition) | Người chơi hợp level không đủ nguồn cho +4, hoặc +8 quá dễ trước story |
| QUEST-03 — PLAYTEST | Full Q1–Q12, NPC mới, hồi phục và Dev Mode cách ly acceptance | Mô hình 115–131 phút là LEGACY; mục tiêu 150–240 phút chưa nghiệm thu | Playable journey vẫn quá nhanh/chậm sau tính đi lại, chết, UI và multiplayer |
| TECH-01 — SPIKE | Login → Character Select → one-time ticket → dedicated join; lease/duplicate session, internal service credential và backend outage | owner kỹ thuật tương ứng: Spring xác minh account/character; Game Server xác minh ticket, một writer/character | Spike hai Client + backend/DB thật, thử duplicate join, consumed ticket, timeout trước/sau commit và server crash |
| INPUT-01 — LUẬT ĐÃ DUYỆT / FEEL CÒN MỞ | Input/focus/pending được chủ dự án duyệt; [Combat & Character](../01-design/combat-and-character.md#focus-input) là owner | Contract [Technical — input](../02-technical/gameplay-runtime.md#input-contract); kiểm hành vi một luồng, không flags/A/B | Rollover bàn phím thật, cảm giác ở tốc độ thường và envelope còn cần review |
| CMB-01 — STRONG DIRECTION / TUNABLE | Basic Tân Lữ giữ; S1/S2 nhanh, S3 signature theo probe mới | [Onboarding evidence](#novice-onboarding-evidence), số fixture cũ không thay fresh route | TTK Q3–Q5 và feel còn giới hạn; không tune EXP/HP để ép số |
| MOBAI-01 — NGUYÊN TẮC ĐÃ DUYỆT | [World & Content — melee crowd](../01-design/world-and-content.md#melee-crowd) | [Balance finding](#novice-onboarding-evidence); không formation/token/shoving hoặc range/speed/interval/HP mới | Readability/deadband/offset cần xem bằng người thật; không chỉnh damage để khớp một ngưỡng metric |

<a id="input-combat-mob-proposals"></a>
<a id="novice-onboarding-evidence"></a>

## Kiểm chứng onboarding và rủi ro cân bằng

**DERIVED:** nhịp basic Tân Lữ đã duyệt cho occupancy lock 0,32 / 0,70 và phần còn lại 54,29% chu kỳ, so với 74% ở control cũ 1,00 / 0,26. Phần còn lại không đồng nghĩa mất điều khiển: locomotion/gravity vẫn hoạt động. Đây là phép tính, không bằng chứng cảm giác tay.

**EXISTING FIXTURE, không đo lại:** mẫu trước đã ghi Lv 3/ba Dummy 9,78 → 6,42 s (fixture lịch sử; Q3 hiện hành là bốn kills/năm placements) và Lv 4/một Sói 4,96 → 3,98 s khi bật nhóm thay đổi. Nguồn và điều kiện nằm trong CHANGELOG tại Git, mục `phase-e--kết-quả-ab-và-giới-hạn`; xem [cách tra source đã xóa](../90-archive/production-history.md#prototype-source-retired). Những số này giúp đánh giá Q3 và Q5, nhưng gộp input/AI/cadence nên không chứng minh riêng tác động CD, không là TTK của build một-hành-vi mới. Q4 Nấm/loot/equip/sell chưa có thời gian fresh tương ứng;

Q3–Q5 cần route chức năng mới. Không lấy mốc Q4 Sói của mô hình cũ thay Q4 Nấm hiện hành; reward/count đọc [Quests & Narrative](../01-design/quests-and-narrative.md#quests-story).

**BALANCE FINDING:** cùng mẫu cũ ba Sói, mất 131 → 166 HP/10 s (+26,7%) khi bật toàn bộ nhóm; riêng AI là 142 HP (+8,4%), riêng input 122 HP (−6,9%). Phụ thuộc target/life/RNG và nhiều thay đổi gộp, chưa thể gán chênh lệch cho một luật chọn cánh. Ghi rủi ro áp lực nhận damage; không đổi HP/range/speed/interval, không thêm safe window để ép kết quả. Mô phỏng Python ở đầu các mục liên quan là loại bằng chứng riêng, không thay quan sát runtime.

**Các câu hỏi thật còn mở:**

- A07 / SCOPE-01: xác nhận wording và authoring của Dummy training actor ngoài “bảy identity/sáu rig”; không tự cộng một combat identity hay rig mới.
- A07 / SAVE-01: Học Viện là khu an toàn nhưng có sân Dummy không gây damage; cần định nghĩa nhất quán safe-zone/checkpoint với training yard trước production.
- QUEST-03 / BAL-02: chi phí chạy về làng/turn-in/mua đồ khi chưa có fast travel; không thêm teleport tiện ích từ ước lượng hành trình.
- INPUT-01: latest buffer giữ 0,18 s BASELINE/TUNABLE; readiness phải nằm trong cửa sổ đang dùng. Tick/latency, duration và giới hạn approach phải kiểm trước production, không tự gọi 0,18 là exact lock.
- Feel/rollover thực trên desktop/laptop, mũi tên + Select/Execute và đề xuất phím dùng đồ chưa được người thật nghiệm thu. Functional route/test không thay kết luận này.

Luật hiện hành ở design owner; các phương án keyboard B/C và số A/B cũ chỉ giữ trong [CHANGELOG trong Git](../90-archive/production-history.md#prototype-source-retired), mục `single-behavior-history-3`. Cấu trúc input đã LOCKED; physical PrimaryAction/items/menu, shell, envelopes và baseline cadence vẫn OPEN/TUNABLE, không giữ điều khiển cũ làm requirement.

TECH-01/SAVE-01 và A15 còn cần chốt thứ tự Potion–hit–checkpoint, definite reject khác timeout, deathUtc/cap release ở terminal-pending và neo clock phiên↔UTC. [Các chuỗi probe cụ thể](#pending-ordering-probes) bổ sung acceptance của spike, chưa bằng chứng runtime hoặc approval schema. Các gap này không chặn local Kiếm bằng RAM fixture, nhưng phải giải trước G-D/reliability.

**Rủi ro kiến trúc :** auth/ticket/lease, checkpoint ordering/validation, reconnect grace, escrow orphan recovery, transaction N recipients, backend downtime và dedicated build/headless physics cần integration tests. Mốc giờ player-host/JSON cũ không còn dùng để cam kết lịch; spike đầu đo integration/latency, sau đó Technical mới đặt lại ngân sách. Quest rewards/nguồn, EXP, giá/drop và Boss giữ; HP/MP trang bị/cadence/HP Cung dùng probe mới, world density chưa chốt totals; các luật PvP/recovery trên là user lock, không Open Decision.

[Review Art](../03-art/art-and-visual-production.md#art-review) tách nguyên tắc đủ dùng và con số chưa duyệt; thứ tự probe/mở production được quản lý tại [Roadmap](roadmap.md#phase-gates).

Các dòng đã có baseline không chặn việc bắt đầu code; ca PLAYTEST/SPIKE ghi rõ phải đo gì. Không tăng NeedEXP, HP quái hoặc tạo hệ thống mới từ mô hình thiếu cảnh thật.


### Quyết định còn OPEN / TUNABLE

| Owner gate | Trạng thái và việc phải chốt |
| --- | --- |
| INPUT-01 | PrimaryAction/QuickHP/QuickMP/Food/menu physical keys và RPG shell còn OPEN; E/4/5/R/I chỉ proposal. Search/retention/vertical/cycle ordering/approach budget và buffer duration TUNABLE. |
| CMB-01 / BAL-02 | CD0,60/0,90/6, MP2/3/16, power S2 Cung và animation timing là PROBE BASELINE/TUNABLE; test S2 repeated và manual weave, MP thiếu/no Food/gear-lag. |
| BAL-01 / GEAR-01 | Cung+8 sau ClassChosenLevel, exact HP/MP trang bị còn TUNABLE; VIT chung/6slot đã giữ. Kiểm late class/evaluator/rarity/+4/+8/PvP/Boss. |
| MOBAI-01 / SCOPE-01 | Hybrid count/identity OPEN; grace/Return regen/invuln/speed/targetability, crowd offsets và Home/Walk bounds còn TUNABLE/OPEN. Không khóa 3/1/0 hoặc dùng quái anti-Bow. |
| SCOPE-01 / PHY-01 | Population/pocket totals từng map còn OPEN; 28/66 là lịch sử; validate nhiều tầng/nhiều group độc lập và quest anchors. Solid trực giao/no natural one-way/no ladder đã LOCKED. |
| ART-01 / QUEST-03 | NPC tọa độ, camera/tile/module/VFX counts và 26 logical frames giữ LOCKED, exact raster mapping OPEN; roster bảy NPC/quest ownership mới đã xác định. Dev Mode không thay fresh-run Q1–Q12 hai phái. |


<a id="keyboard-prototype-review"></a>

## Keyboard UX và giới hạn art validation

Lịch sử build, debug fixtures và kết quả kiểm VS-1 nằm tại [Production history](../90-archive/production-history.md#prototype-runtime-history); chúng không thay thế validation balance hoặc G-L production.

[Art & Visual Production](../03-art/art-and-visual-production.md) sở hữu First Art Probe, file lifecycle, pose/socket minimum schema, asset DoD, visual style sample và provenance. Không chốt nghĩa 26 frame, technique/socket/camera hoặc sản xuất full catalog. Các bảng mô phỏng/accounting giữ để đối chiếu, không chuyển thành acceptance mới.

<a id="prototype-feedback-review"></a>

## Onboarding, control và giới hạn kiểm chứng

Luật và counts Q1–Q12 ở [Quests & Narrative](../01-design/quests-and-narrative.md#quests-story); input/pending ở [Combat & Character](../01-design/combat-and-character.md#focus-input). Các quyết định F01–F08 và review theo phiên đã chuyển nguyên văn sang CHANGELOG, mục `single-behavior-history-3`, nay tra tại [commit lưu trữ](../90-archive/production-history.md#prototype-source-retired). Giữ các alias trên để tra evidence cũ, không giữ luật gameplay trùng tại Playtest & Balance.

26 logical frames LOCKED; exact raster mapping/technique/camera/file count OPEN; LoS/Hybrid/Return/airborneS2-S3 giữ OPEN. Input/NPC/terrain/cadence/HP-MP đọc theo design owner; bảng lịch sử giữ số và nhãn để truy vết, không nghiệm thu runtime.


<a id="art-validation"></a>

<a id="24-prototypevalidation-trước-production-hàng-loạt"></a>

## Prototype/validation trước production hàng loạt

Đây là **ma trận kiểm hình ảnh**, thứ tự làm ở Roadmap, cách thử mẫu ở các mục liên quan. Mọi ca **CHƯA CHẠY**; tài liệu không nghiệm thu Unity/art. Dùng placeholder/fixture có nhãn trong sandbox hoặc slice phù hợp. Các dấu hiệu đủ giúp quyết option đang thử; P12 chỉ thử anticipation A11 nếu chọn, không khóa pose xuất hiện ngay. G-N cần result/life/phase đúng, không buộc thêm projectile tạm hoặc rollback.

**Ưu tiên kiểm:**

- Trước base: P01 minimal player 3/4 Idle Left/Right/side/carry/shadow và swap overlay nhỏ; P02 Kiếm/default/I với S1/S2 A/B và P03 minimal Cung S1/S2/socket/draw-release ngang ưu tiên, P05 Nấm/Sói, P06 ground, P07 solo Q3/Q6, P08 room, P09 geometry nhỏ, P11 kit và P15 công thực tế. P04 primary-proximity/S3/Spread/Hàn dùng fixture hẹp trong C0 trước credit quest thật/full VFX.
- Sau base và G-L mới: P07 contention, P12 Dedicated. RAM pass không thay backend ACK/terminal-pending ở P06/P12.
- Sau core gate: full Bow bands/profile/P03, Boss/P14, thác/full gear và stress full content. Bảng dưới giữ toàn TARGET, không buộc chạy hết trước slice.

**Asset Definition of Done — áp cho module nhập vào probe:**

- [ ] Canvas/PPU/pivot/mốc chân đúng; alpha/palette sạch, không crop lệch cell hoặc bleed.
- [ ] PoseKey/ref/duration có manifest; các layer khớp, không hở Body/Armor/Lower.
- [ ] Hai hướng đúng grip/socket/front-back; không mirror text/physics hoặc mirror hai lần.
- [ ] Import/slice/platform/atlas refs đúng; gear swap runtime giữ phase, Run không trượt chân.
- [ ] Camera thực đọc silhouette/cue/UI; AnimationEvent/visual collision không gây damage.
- [ ] Source/export/meta/revision/provenance đầy đủ; ghi pass/fail, công sửa và output bị loại.

DoD là chất lượng asset probe. “Có PNG” hoặc “build chạy” chưa nghiệm thu art/UX toàn game.

| Ca | Nội dung phải thử | Dấu hiệu đủ để ra quyết định |
| --- | --- | --- |
| P01 Modular player | Early subset theo [Art probe](../03-art/art-and-visual-production.md#first-art-probe): 3/4 Idle Left/Right/side Run/Jump/Fall, action/recovery, carry, player shadow, một overlay swap nhỏ. Full default +I/II/III/mix-band/unequip ở phase rộng | Không hở thân/grip sai/nhảy pivot/jitter; thử stop ngay/delay, focus-only/repeated S2; shadow hide gear và reset đúng life. Manifest unique/reuse/socket/VFX/optional, kiểm26 logical samples và unique raster mapping và delta 3/4-facing/class/airborne |
| P02 Hai Sword cùng action | Mộc và Huyền Ấn dùng cùng track Attack; hai Kiếm class khác band dùng cùng Skill; thử swap giữa action hợp lệ | Đúng cây ở cả Back/Hand/release, không duplicate/ghost hoặc sprite Common baked; socket/góc raster đủ; visual snapshot không morph sai; không cấp skill class cho Tân Lữ chỉ để test Mộc |
| P03 Bow | Ít nhất hai band rest/draw/release/Skill, S1 và triple | Back silhouette/canvas đọc được, tay/string/arrow nock khớp; spawn đúng timeline, ba tên một cast; quyết có partial-draw thêm và logical hit/visual timing contract |
| P04 Multi-target | Primary-proximity Kiếm3/5, Hàn1+4, spreadA/B/C–A/B/A–A/A/A, no-target/invalid; C1/C2/LoS A/B | Impact cùng phase resolve của profile, spread ba tên một cast, không chain giả; không double status/impact/projectile |
| P05 Hit không stun | Mỗi rig bị hit Idle/Move/Windup/Attack/Ranged; Freeze trước/sau release; lethal | Main action không restart/hit trễ vì flash; CC server cancel unresolved đúng life; resolved result bất biến, visual travel không gameplay callback, Death thắng stale Hit |
| P06 Death/corpse | Ground/Ong/Boss; chết ở ledge, late viewer, ACK chậm/retry, normal respawn 25 s | Corpse không target/collider/AI, loot đứng được, timer từ death không từ clip/ACK, đời mới không nhận effect cũ |
| P07 Dummy online | Q3 bốn kills/năm placements3+2, repeated respawned slot; Q6 thiếu MP/full-MP reject, không spam S1; hai Lv3, Lv3+20, 3–4 requesters | Không mandatory wait/unique slots, HP60/timer25s/credit≥20% đúng life; một Dummy identity; đo contention trước đổi timer |
| P08 Terrain room | Bốn grammar: đồi bậc solid, khối đá/hốc/khe, công trình/cầu và one-way có support; decor cùng chất liệu | Người chưa biết collider đoán đúng; không slope/climb/natural one-way/floating bars; drop một actor không làm người kia rơi |
| P09 Công trình | Hai surface/routes, HomeRegion/WalkRegion, spawn/Ong/chase/leash/Return/Kiếm/Cung; Hybrid chỉ fixture khi cần | Không stuck/spawn trên không/perch spam vĩnh viễn; ground quay ở mép, không jump/drop; kite hợp lệ; mask LoS A/B còn probe |
| P10 Nước | Puddle base/front, thác seam, actor dưới nước/đi trên cầu, cả facing | Front chỉ che chân, không telegraph; bridge không splash, không thay physics; quyết sprite/particle reuse |
| P11 UI kit | Inventory/Storage/Shop cùng kit, sáu RPG views, text Việt; Select→Execute, pending đổi selection, Q1 khu NPC/Q3 Bách/Q6 mentor | Select không cast/cost/approach, S2 selected/execution cue rõ; modal không lọt input, glyph không khóa proposal key; rarity/band/+level và error khác RNG fail |
| P12 Online presentation | Hai Client Dedicated tối thiểu; thêm 4+, RTT 0/100/200 ms/jitter/loss; player chết khi người khác đánh target | Thử anticipation A11 nếu chọn; một effect/result, HP focus hợp lệ tiếp tục cập nhật, same-slot respawn xóa life cũ, map/reconnect không replay reward |
| P13 Combat/balance | Intervals/timing control; Lv 5/10/17, MP zero-INT/cân bằng, TTK solo/cụm và PvP Food/quota | Ghi số Novice/S1/S2/S3 casts, thời gian chờ MP, proc/TTK/visual travel; đổi một nhóm số mỗi lượt, chạy lại model sau khi timeline thay |
| P14 Boss/camera | Telegraph 0,5/1,0/1,2 s, ba landing zones, Cuồng dưới 30%, Slow, nhiều VFX/người | Telegraph không bị che/cắt hoặc time-stretch, đọc/né bằng movement hiện có; camera/zoom đọc được range |
| P15 Chi phí slice | Một outfit family, Kiếm/Cung, mob theo capability được chọn và room/UI nhập hoàn chỉnh | Kê ảnh mới/reuse/variant/setup và giờ thật; tính lại công, Hybrid chỉ khi chọn; không nhân frame count chưa kiểm |

Ghi build/Editor/packages, camera scale, máy và seed/fixture; một ca chạy mượt chưa chứng minh capacity. Pose khó đọc thì sửa art/timing hình ảnh; đổi hit/spawn/CD/collider/loot position/timer phải đi đúng owner và kiểm lại ca phụ thuộc. Các phép thử không tự khóa số đề xuất trong file. Dev Mode phục vụ test nhanh, không là UI production hoặc evidence fresh-run; route canonical Q1–Q12 vẫn thật theo design owner/Technical/Roadmap.


<a id="revision-validation"></a>

<a id="31-các-phép-kiểm-bắt-buộc-sau-migration"></a>

## Các phép kiểm bắt buộc

Đây là những dependency để đóng gate triển khai, không phải sổ quyết định mới. Kết quả cũ chỉ chứng minh revision cũ; tất cả dòng dưới **chưa có bằng chứng runtime theo revision mới**. Các số TUNABLE và phần OPEN phải được ghi cùng setup để người review biết đang thử giả định nào.

| Nội dung cần kiểm | Kết quả đủ để review | Gate phụ thuộc |
| --- | --- | --- |
| Controls và PrimaryAction (combat branch) | Phím mũi tên điều khiển movement. 1/2/3 chỉ chọn, không tiếp cận/cast/tiêu MP/đặt CD. Execute riêng tạo lệnh một lần; đổi slot không sửa snapshot đã có. Kiểm giữ phím, pending/buffer, hủy lệnh và lúc tới tầm; log/video ghi bindings cùng review bàn phím/chuột. Đề xuất E PrimaryAction/4–5 Potion/R Food/I menu còn TUNABLE, exact keys OPEN | G-B/G-L; G-N kiểm intent qua mạng |
| Focus khi chết và HP observer | Người chơi chết hủy lệnh combat nhưng giữ focus hợp lệ, marker và HP hiện tại/tối đa; HP vẫn cập nhật khi người khác đánh target. Target chết, despawn, sai life/map hoặc hết điều kiện giữ phải xóa focus đúng. Respawn cùng slot không kế thừa focus đời cũ | G-L; G-N với ≥2 client |
| Cadence và vai trò S1/S2/S3 | Baseline TUNABLE: S1 0,60 s/2 MP, S2 0,90 s/3 MP, S3 6 s/16 MP; Cung S2 0,70/0,60/0,50 power. Kiểm S2 dùng thường xuyên để farm, thời gian khóa hành động, lúc resolve, đánh nhóm/trạng thái, mức tiêu và hồi MP, TTK. Không thêm đòn thường 0 MP sau chọn phái | G-L cho phần Kiếm đã có; G-C/F cho skill và hai phái đầy đủ |
| HP và gear mới | So hai phái ở cùng cấp/trang bị/điểm, chọn phái đúng Lv5 và chọn muộn. Cung tăng +8 HP/level sau ClassChosenLevel thực là baseline TUNABLE; HP không tụt khi chọn phái, VIT vẫn +8. Hướng ba slot HP/ba slot MP cần tính lại qua rarity/enhance/chuyển giao với các giá trị TUNABLE. Kết quả sustain/Boss cũ không pass giá trị mới | G-B/D kiểm dữ liệu và transaction; G-C/F kiểm balance |
| Density / topology | Author theo [World candidate manifest](../01-design/world-and-content.md); giữ quest anchors và Boss exclusion. Đo nhiều tầng, independent aggro/crowd/Return và contention trước capacity claim | G-L/G-N/G-C; chưa chạy |
| Địa hình và kiểm thử 8 map | Đất/đá tự nhiên là khối solid dày, mặt đi ngang/mặt đứng trực giao; không mặt đi dốc/ramp/tam giác, đất one-way hay cơ chế leo. One-way hiếm chỉ ở kết cấu mỏng nhân tạo có chống đỡ rõ ràng. Kiểm thử nhảy/drop-through/EdgeExit chống giật lặp chuyển cảnh (anti-pingpong transition); kiểm thử nước nông làm chậm nhẹ khi chân tiếp xúc và không làm chậm khi đi trên cầu gỗ/trên không; kiểm tra 8 MapRoots không chồng lấn collider; kiểm tra BossCombatArea cấm tuyệt đối quái thường; GroundMelee không rơi/nhảy/drop giữa tầng | G-L trước G-N; G-C kiểm phần map mở thêm |
| NPC và tuyến chọn phái | Bảy NPC ở khu chức năng (Lâm Bá, Yên Thảo, Bách Luyện, Mộc An, Phong Du, Diệp Lam, Hạo Vũ). Q3 tại Bách Luyện; Q6 Lâm Bá giới thiệu, talk cả hai mentor rồi nhập phái và trả tại mentor phái đã chọn; Q10–Q12 Lâm Bá, Tẩy Mạch tại Mộc An. Người chơi tự tháo/mặc/học/dùng đồ; kiểm từ chối, retry và túi đầy, không dùng preset để bỏ bước | G-B review định nghĩa; G-L Q1–Q6; G-C/G-T hai route |
| Full quest definitions và fresh-run | Đủ Q1–Q12 definitions và tuyến production thật; giữ IDs/anchors/nguồn credit ổn định. Q9 tùy chọn có ca online riêng. Mỗi phái có route fresh hợp lệ với hành động, thưởng và mở khóa thật; không chỉ đặt tracker hoặc dùng dev tool hoàn thành prerequisite | G-B về schema/definition; G-C/G-T về tuyến đầy đủ |
| Rig/UI và độ đọc theo luật mới | Slot được chọn đọc rõ và độc lập hành động đang chạy; S2 có feedback đủ đọc ở nhịp mới; HUD vẫn hiện focus khi chết. Kiểm pose/socket/terrain/crowd trong room thật; chưa giải A01/A02/A12/A14 thì chưa nhân family dựa trên giả định | G-L với mẫu nhỏ; G-C/F trước mở rộng hình ảnh |

<a id="dev-speed-acceptance"></a>

<a id="32-dev-speed-và-acceptance-evidence"></a>

## DEV SPEED và ACCEPTANCE EVIDENCE

[Dev Mode trong Technical](../02-technical/gameplay-runtime.md#dev-mode) là tooling dev-only để rút thời gian thử: đặt level/quest/class, hoàn thành prerequisite, cấp item/skill, teleport tới marker đã author, reset encounter/group/Boss hoặc force Linh Biến. Nó không thuộc UI production cho người chơi, không lưu như tiến trình hợp lệ và không bypass authority release. Kết quả có preset phải ghi là fixture/probe.

**DEV SPEED** cho phép đi thẳng tới tình huống để tìm lỗi và so phương án. **ACCEPTANCE EVIDENCE** chứng minh người chơi thực hiện được hành trình bằng các hành động hợp lệ. Ca dùng preset có thể pass kiểm kỹ thuật hẹp, nhưng không pass fresh-run route.

G-L cần fresh-run Q1–Q6 của Kiếm từ trạng thái đầu hợp lệ. G-C/G-T cần fresh-run **toàn Q1–Q12 cho từng phái Kiếm và Cung**, từ tạo/chọn nhân vật tới chọn phái, nhận/trả quest, gear/skill/supply và Boss/credit đúng tuyến. Q9 optional không chặn truyện chính; phải có ca PvP/Q9 riêng để nghiệm thu hệ đó. Không dùng SetQuestState/GiveItem/CompletePrerequisite/teleport dev để thay bước trong bằng chứng route. Kiểm retry/recovery có setup riêng và phải phân biệt với video hành trình fresh.

Mỗi hồ sơ cần ghi revision/build, phái, trạng thái khởi đầu, bindings/setup, log kết quả authority và video/phần review ở tốc độ thường. Ghi rõ bước nào đã quan sát và gate nào còn thiếu. Automation bổ trợ kiểm logic; cảm giác điều khiển, layout và usability vẫn cần review của người chơi.


<a id="early-two-class-probe"></a>

## Early minimal Kiếm và Cung — trước production rộng

**Chưa chạy.** Minimal player 3/4 Idle Left/Right/side/carry/shadow và overlay swap theo [Art owner](../03-art/art-and-visual-production.md#first-art-probe), trước khi vẽ đủ ba bands; P01 local fixture không thay P12 duplicate/late/stale/reconnect/death authority evidence mạng. Dùng cùng scene scale/movement/collider và fixture revision cho hai phái. Kiếm: movement/select/Execute/S1 + **S2 A/B ngang ưu tiên Cung S2**, primary-proximity3, S3 fixture và socket/timing. Cung: movement/select/Execute/S1 + S2 Spread hiện hành, range/focus/kite và bow socket/draw-release.

Đo acceleration/deceleration, jump/coyote/buffer, air policy, target switch và pending, animation khớp action clock, MP sustain/Food. Chạy 1/3/4 targets, có damage **gây và nhận**, no-Food/expiry/gear-lag và quay đầu/đổi tầng/Return. S2/S3 air permission giữ OPEN, ghi rõ thử policy nào. Capture build/revision, bindings, fixture, logs và video tốc độ thường; người thật review rollover/feel/usability. Automation không pass cảm giác tay.

<a id="potion-acceptance"></a>

## POT-01 — realtime, durability và outage acceptance

Inject backend latency khi một hit sắp lethal: heal timing phải theo server acceptance, không theo ACK. Log command/tick/effect/HP-before-after/consume/quota/checkpoint sequences. Cùng-tick hit/Potion cần deterministic ordering được review.

Replay/lost ACK, double input, Food tick xen giữa, death sau heal, periodic checkpoint đến trễ, logout/transition, resume và stale generation: chỉ một consume/effect/quota/Used credit. Arena không ghi combat HP vào pre-Arena checkpoint.

Process crash **sau accepted heal trước durable handoff**, restart/rejoin lặp lại và timeout trước/sau commit là ca bắt buộc. Nếu debit mất/hoàn bình lặp vô hạn thì fail G-D/G-T; queue RAM không đủ. Thử pending budget/lease expiry/definite reject, đảm bảo fail-closed admission mới và không rollback tick đã accepted. Exact recovery mechanism còn OPEN tại [online owner](../02-technical/online-and-persistence.md#potion-durability).

## Q8-01 — natural hunt / contribution / population, CHƯA CHẠY

Test initial/new life Lv8+ roll5% TEST/cap1/map; Normal slot/Return/rootwake/reconnect cùng life không reroll. Không quest force/private/reserved life; TA4 chỉ normal pocket. Giữ Ong Linh sống để đo thiếu Sói Linh, không kỳ vọng quest admission guarantee hoặc demote. Dev Force fixture riêng không pass natural availability.

Q8 CollectDấu RNG trong group trước, owned đủ mới mở Kill1SóiTrúcLinh; wrongidentity/normalvariant/prestep damage không credit, một death snapshot không credit stage vừa activate. N=1/2/3/4/>5 và gear-lag/outsider: đo qualifying eligible deaths, encounter/wait/credit distributions/tail, không giảm20% để demo pass. Một shared mob, regular loot≤1+Gold budget, personal right mỗi eligible recipient riêng. Map Info ordinary/Linh subset chính xác0→1→0, không coords/pocket. Snapshot/mapchange/reconnect/late delta kiểm tại MI cases.

Record life/deathId, identity/variant/map, cap occupancy/due times, active group/ActualHpLost, RNG outcome success/failure, entitlement and receipts; lostACK/TTL/fullbag/rejoin/restart giữ committed result. Không queue age/quest retry spawn hoặc extra reward budget. Nếu natural hunt pacing không đạt, report OPEN rate/density/lifetime/progression dependency và evidence; không tự chọn fix gameplay.

## Balance harness — yêu cầu tương lai, chưa triển khai

Harness phải có deterministic seeds, version/build/definition revision, command/setup/fixtures và artifact output lưu bền trong repo khi được phép triển khai sau. Dùng shared gameplay formulas hoặc parity test với runtime, không thêm formula authority thứ hai. Tách DERIVED, SIMULATION và runtime observations; archive kết quả sai revision.

Ma trận: Sword/Bow, balanced/extreme/no-INT/no-VIT/gear-lag/late class và rarity/enhancement; 1/3/4 mobs, Linh, Boss và PvP khi phù hợp. Báo TTK, damage dealt/received, MP used/remaining, Food/Potion sustain, EXP/hr, Gold/hr, wait/contention và death/run-back. Ghi positioning, hitbox-validity, travel/reaction/latency, uptime và pool-start assumptions. Chưa có AI phản công thì không suy survivability; chưa có geometry thì không gọi TTK là actual gameplay.

Seed hoặc script tạm trong /tmp không phải reproducible evidence dài hạn. Phép tính đã có được giữ để truy vết, không được biến thành pass mới.


<a id="consolidation-acceptance"></a>

## Consolidation acceptance mới — toàn bộ CHƯA CHẠY runtime

Lượt consolidation 2026-10-09 chỉ chạy docs/link/scope validation. Source/test names và DERIVED tables không pass runtime gate. [Slice plan](roadmap.md#consolidation-slices) chọn tests theo phase; Stable counts Q3/Q4/Q5/Q12 và MobIdentity giữ; Q8/Q10 normal quotas/Q11 ordinal flow SUPERSEDED; RNG outcomes thay generic ordinals, Q2/schema/TTL/26 mapping và bounds/LoS giữ PROPOSAL/TUNABLE/OPEN tại owner.

| Probe | Setup/kết quả phải quan sát | Gate |
| --- | --- | --- |
| Inventory 60/stack | Reject món distinct thứ 61; full bag compatible merge; wrong binding/unique gear; numeric bound/checked cost/tampered quantity/split IDs; không reserve quest | G-B/L; G-D API/DB parity |
| Shared Shop UI | Catalog Buy/toàn bag Sell, disabled bound+reason, quantity/total, keyboard/mouse cùng command; viewport/focus 60 ô, Q4 sample Sell đúng Bách/step | P11/G-L; G-N snapshots |
| Fixed Jump/Drop | Cùng setup không ceiling: tap/hold cùng apex; coyote/buffer/no double jump/Jump+Drop priority; hai actor một sàn, một Drop; restore death/map/disconnect; không natural one-way | PHY-01/G-L/N |
| Physical collection | Q8/Q10/Q11 material/restored grants Pending không possession; owner-only visual/RPC ACL; finite TTL/sourceMap re-offer không kill lại; compatible merge nhưng Q11 ba identities riêng | G-L fixture; G-C route/G-D durability |
| Full bag/net turn-in | Compatible claim vẫn fit ở 60/60; nếu không fit giữ Pending. Consume tạo chỗ cho reward; thiếu net capacity giữ toàn inputs/state, không partial | G-L/D |
| Reconnect/crash/replay | Crash trước/sau death/claim/turn-in commit; lost ACK/receipt query; old lease/generation, map/death/TTL/new session, N eligible/outsider; không duplicate grant/pile | G-N network; G-D real DB |
| Approved counts / identity credit | Matrix Q-F01..12; farm đúng identity ở group khác/map-of-quest khác khi player-mob sameMap. Wrong identity/reused rig/variant/special fail đúng predicate. Không whitelist DS/TA/XN/HT cho standard kill. Counts4/5/8/10+Boss giữ; Q8 Collect→Kill1Linh/Q10Collect/Q11restoration direction. Novice sustain và 2/3/4/>5 requesters contention/20%/ground readability | C0/G-L rồi QUEST-03/G-C/T; fixtures không pass route |
| Q2 journey | Approved placed-item definition; reason/destination/return rõ, fixed-jump terrain, Drop optional; full bag/TTL retry/consume thật; nối Q3/Q6 | G-L; Q2 content approval |
| Q6/NPC context | Both-talk mọi thứ tự, class/mentor/grant atomic; selected/unselected mentor biết class sau Q6, không off-class manual; Tẩy Mạch ở Mộc An | G-L Sword/G-C Cung/G-D receipts |
| Facing/26/shadow | Stop giữ Left/Right, Idle 3/4 đọc mặt/áo; Run side, Jump/Fall riêng, class upper action; socket/mirror/asymmetry; 26 giữ/mapping OPEN; current player death, mob death giữ | P01/P03/G-L/N; A01/A02/A12 |

Archive virtual/front/variable-jump wording vẫn là historical evidence, không sửa source/CHANGELOG hoặc dùng tests cũ nghiệm thu luật mới. Các bảng timing lịch sử không được tái dùng cho pacing counts mới; chưa có fresh-run hoặc simulation mới sau recovery. G-T vẫn cần fresh Q1–Q12 từng phái và ≥2 clients/backend thật.


<a id="recovery-combat-acceptance"></a>

## Recovery combat acceptance — 15 ca, toàn bộ CHƯA CHẠY

L = direction/values hiện hành; B = executor recommendation; O = policy/numeric OPEN. C0 trước real quest death credit; rerun G-N/G-D/G-C theo dependency. Ghi origin/primary/target lives, start/resolve set, index/power, profile/bounds/LoS, Evade/Crit/proc-cache, damage clock và VFX event IDs; không dùng screenshot effect làm bằng chứng target-set.

| ID / ca | Expected outcome và điểm còn mở | Gate / trạng thái |
| --- | --- | --- |
| C01 Một target | L S1 single; Kiếm S2/S3 một primary, Hàn không double primary, Bow S2 AAA ba indices một unique status roll | C0/G-C — CHƯA CHẠY |
| C02 Nhiều target quanh primary | L caps3/5/3indices/1+4, không thêm damage. B Kiếm/Hàn primary-first rồi near-primary/ID; C1/C2 biên và caster envelope phải log | C0/P04 — CHƯA CHẠY |
| C03 Sau lưng primary | L eligible secondary có thể hit dù ngoài slash. B query near-primary, impact đúng actual result | C0/P04 — CHƯA CHẠY |
| C04 Sau lưng player | O khuyến nghị no secondary front-filter C1/C2; test trong/ngoài bounds, ghi profile. Chưa kết luận policy cuối | C0/feel — CHƯA CHẠY |
| C05 Khác tầng | L không PlatformID gate; O trong/ngoài vertical1/1,3/3 và C1/C2; same surface ID không tự cho hit | C0/PHY-01 — CHƯA CHẠY |
| C06 Qua SolidWall | O A control vs B khuyến nghị chặn origin→primary/primary→secondary; Spread origin→each. One-way không chặn B; không collider VFX | C0/PHY-01 — CHƯA CHẠY |
| C07 Cùng identity khác group | L group không chặn damage; valid death có credit khi quest active/predicate/threshold, không giới hạn map-of-quest | C0→Slice5/6/G-N — CHƯA CHẠY |
| C08 Primary chết trước HitMoment | B Kiếm/S1 mất hit, Kiếm không lan; L Hàn không nổ; L Spread A indices mất, B/C hợp lệ vẫn hit; không refund/reacquire | C0/G-N — CHƯA CHẠY |
| C09 Secondary chết/đổi life | Không stale hit/status, không refill index/cap; respawned life không thay snapshot. B query Kiếm resolve chỉ lấy live eligible set trước apply | C0/G-N — CHƯA CHẠY |
| C10 Primary Evade | B Kiếm vẫn lan; L Hàn vẫn nổ, Spread mỗi hit roll; primary NÉ không wound/status, cache fail status trên AAA | C0/P04/G-C — CHƯA CHẠY |
| C11 Đổi focus/slot/gear | L pending focus change cancel; select-only không sửa intent đã chụp; accepted action giữ primary/SkillId/source/visual và timing | C0/G-L/N — CHƯA CHẠY |
| C12 Chạy tiếp khi pending | L suppress held-axis cũ, new movement/Jump/Drop cancel; arrival revalidate trước cost. No-target/out-of-range không credit/cost giả | C0/G-L/N — CHƯA CHẠY |
| C13 Nhiều player cùng hit | L server serializes life/death, mỗi ledger/receipt đúng recipient; ≥20% normal/≥10% Boss. No fixed-pair, >5 Q8 cohunters contribution/scarcity còn probe | C0 fixture→G-N/D — CHƯA CHẠY |
| C14 Boss sprite lớn và eligibility | O authored logical center metric/profiles; đổi sprite bounds không tăng reach; actual hurtbox data khác phải explicit. Boss telegraph/mob dodge geometry giữ | C0/P14/G-F — CHƯA CHẠY |
| C15 VFX tan sau damage | L damage/result clock không đợi fade/arrival callback; một main/action, impacts actual, status thật, retire/reconnect không replay | C0 minimal/P04/P12 — CHƯA CHẠY |

### Quest regression — Q-F01..12, toàn bộ CHƯA CHẠY

| Ca | Setup / expected theo matrix quest owner |
| --- | --- |
| Q-F01 | Q1 ba NPC khu dược/rèn/kho theo thứ tự; Intro/Accept/return/action receipt, replay không thưởng hai lần |
| Q-F02 | Q2 draft journey có purpose/placed-item/return; fixed Jump, optional Drop; full bag/TTL/re-offer/net consume. Chờ duyệt exact content, không production PASS |
| Q-F03 | Q3 bốn qualifying life kills, năm slots3+2; kill lại respawned slot tính, không chờ bắt buộc/unique ID. Lv3+20 contribution, no loot/EXP |
| Q-F04 | Nấm5 từ bãi bất kỳ, gồm DS1/DS7/DS8; áo+sample entitlement một lần #5 baseline; wrong identity/step no credit, full bag/TTL/reconnect giữ payload; equip áo rồi Sell sample đúng Bách atomic |
| Q-F05 | Food/supply receipt một lần, SóiSương8 cả bãi Trúc khi sameMap; HP full reject không consume, không bắt potion vào lúc đầy |
| Q-F06 | Both-talk hai thứ tự, Sword/Bow, manual unequip/class/C/mentor commit; unselected mentor Talk và đúng manual. Food đầy MP sau cast: thuốc còn, Used chưa credit, không spam; proposal reschedule chưa áp dụng |
| Q-F07 | Ring0→1 hoặc owned≥1; thiếu mới grant, equip binding/preview/cost/RNG+receipt; full bag/reconnect không duplicate |
| Q-F08 | Collect Dấu RNG: success/fail/no eligible không tạo sai right; baselineN2 TUNABLE, không normalKillquota; owned đủ → riêng Kill1SóiTrúcLinh≥20%. Natural shared, no private/force; BrokenSeal lore, no filler. One death không credit future group. Fullbag/TTL/reconnect/durable outcome; Q8-01 contention |
| Q-F09 | Q9 optional khi Q10 active; genuine WIN/LOSE/DRAW tính, FORFEIT/ABORT không; escrow/outage/replay đúng policy |
| Q-F10 | Collect VậtChứng RNG baselineN3 TUNABLE từ ĐoạtMạch đúng identity kể cả BV/XN sameMap; khôngKillquota/groupwhitelist, ChiselMarks/SealScar lore. Outcome success/fail dedup; physical pickup→turn-in consume/reward atomic, sourceMap/fullbag/TTL/reconnect |
| Q-F11 | MaterialRNG Thạch→Lâm→Bách; gate before deterministic3fragment grant theo OPEN fixture labels; fullbag/batch retry no duplicate. Three distinct placements trên nhiều map accessible trướcQ11; consume corresponding fragment+durableflag atomic, replay/reconnect; gate reads flags, no mobfragment/oldquota/bag3. Completion/level/maps/order còn OPEN, fixture không author production hoặc pass content |
| Q-F12 | Guard10 bất kỳ bãi same identity; specific Boss một life≥10%, corpse trong area tính/return-disconnect fail; shared15min/no quest spawn/no normal trong BossArea; một death receipt và một turn-in reward |

### NPC/Shop regression — CHƯA CHẠY

Yên chỉ Food/HP/MP và healing thường; Bách gear/stone/GeneralSell/Enhance/Transfer/Q4 sample; Mộc Storage40/Rest/HồiSinh1000/TẩyMạch1200. So catalog/prices/effects trước-sau, utility không còn ở Yên, không duplicate Hoán Chuyển/Crafting/seasonal/bag expansion. NPC root service riêng; Shop Buy/Sell top, grid trái/detail phải, full bag Sell có disabled reasons/quantity; wrong vendor/step/quantity/Gold/range/map/stale revision reject, UI reuse không thay business handler. Runtime/DB evidence theo Slice4/G-N/D, chưa có PASS.

<a id="loot-model-transition"></a>

## Loot Before → After — DERIVED equations, chưa simulation/runtime

Đếm **stack/outcome**, không item units: model cũ Normal E=0.30+0.04+0.08+0.051=**0.471** regular outcomes/death; P(any)=1−0.70×0.96×0.92×0.949=**0.41329024**. Linh E=1 material stack+1stone+0.365gear=**2.365**, material quantitymean1.5 cho **2.865 units/death**. Không dùng tỷ lệ này làm active weights. Gold/EXP không nằm phép đếm; quest/tutorial/Boss tách riêng.

Model mới variant v: E(outcomes)=q_v≤1, E(quantity category c)=q_v×w_vc×E(Q_vc), Σw=1 conditional success; E(sell value)=q_v×Σ(w_vc×E(Q_vc×unitValue_vc)). Gear rarity conditional riêng. q_L>q_N và rarity tốt hơn chưa chứng minh Linh totalvalue premium nếu category/quantityweights sai. Không thể giữ E_L2.365 stacks dưới cap1; phải tune targets có evidence. q/rates/weights/quantities **TUNABLE**, không đề xuất numbers mới.

So trước/sau material units/hr, potion usable tier/hr, stone/hr (enhance/transfer sink), gear/rarity frequency, sell income/netFoodPotionGold, bag/storage distinct occupancy và Linh/Normal premium. Density/life throughput/level eligibility/claim expiry cùng model, không nhân per-player budget. Stone8%/640kill/Gold-perkill tables trước đây là HISTORICAL; shop800/enhance costs giữ, chưa chứng minh affordability revision mới. Boss EV và source/price arithmetic không đổi; không rerun balance bằng model cũ gọi pass.

Collect iid eligible deaths: E(kills)=N/p, Var=N(1−p)/p²; player eligibility/world supply/competition làm wall-clock khác, N/rate chưa chọn. Q8/Q10 quota removal và Q11 gate mới đổi EXP/Gold farm/tail và Lv15–20 timing; QUEST-03 phải fresh-route từng phái, không auto retune NeedEXP/reward/top-up.

<a id="primary-action-acceptance"></a>

## PrimaryAction / Inventory / Map Info — toàn bộ CHƯA CHẠY

| ID | Setup / expected | Gate |
| --- | --- | --- |
| F01 | AUTO/EXPLICIT enemy sticky, closer item/NPC không steal;1/2/3 select-only; onePress→onecombatintent, hold không repeat | C0/G-L/N |
| F02 | Enemy invalid/dead → next valid combat ngay, no auto cast; no combat → eligible item/NPC soft/landmark. Respawn same slot newlife không inherit | C0/5/8 |
| F03 | Clickitem khi enemy/pending/buffer tồn tại → single item marker, pending cancel; PrimaryAction exact item/gen only, RunningAction accepted snapshot không bị sửa | C0/5/N |
| F04 | Item expired/claimed/rights/range/gen fail → reason/invalidate/resolver, **không samepress cast/fallback**; capacityfail validfocus giữ |5/N/D|
| F05 | Successpickup→candidate next, không pickupchain/holdrepeat; Tab fromitem/NPC chỉ combat, no candidate giữvalid focus |5/8/G-L|
| F06 | NPC PrimaryAction rootmenu/quest preselected; no NpcTalked/grant/turnin onopen, Talk option explicit; modal openingpress consumed, no worldleak |8/G-L/N|
| F07 | Death giữvalidcombatobserver+HPupdates, actionslocked; invalid clears; newdeadacquisitionOPEN fixturelabel. Mapchange/oldgeneration/sessionclear không stale cast |C0/G-N|
| L01 | Normal/Linh0..1regular weightedoutcome; gold/EXP riêng, qL>qN/quality profile fixture; questRNG optionalcoexist, Q4two stagedsupply exception, Bosschannels unchanged |5/6/G-D|
| R01 | Eligible death fail/success persists once; replay/lostACK/restart không reroll; concurrentdeaths+fullbag capbag+pending≤N; stopsafterrightscomplete |6/G-D|
| R02 | Nrecipient ownrights no shared claim; sourceMap BV recovery, TTL không loss, oneactivegen; kill trướcgroup/noeligible nooutcomegrant; placement/grantQ11 durable flags |6/7/G-N/D|
| I01 | Buy/Sellquantity/total/partialstack/serverprice/overflow/revision/capacityreject atomic;60/60compatiblepickupworks |1/4/G-D|
| I02 | Splitk1..Q−1 emptyslot/newstackID/quantityconserved/noclonesourceR; compatiblemergeonly; replay/conflict toànstate |1/3/G-D|
| I03 | Manual Sort/Merge noautopickupsort; uniquegearstate khôngmerge, selectedID map survivor khôngmóndifferent |1/3/G-L|
| I04 | Discardconfirm destroy/noGround; protected/quest/tutorial/manual/equipped rejects+reason; ordinary canDiscard defaultOPEN không infer từpalette |1/3/G-L/D|
| MI01 | Spawn/terminaldeath/despawn/Linh0→1→0 counts, ordinaryincludesLinhsubset, dormantincluded/Bossseparate, no rendercount |5/world/G-N|
| MI02 | Initial/dueadmission/reconnect snapshot; late/duplicate/gap/outoforder revision; mapchange clearsoldpanel, oldmapgenerationignore; unknownpendingcue/no coords |5/8/G-N|

Logs phải có focus type/id/gen/press/typed command/reject/maprevision, server state trước-sau và receipt; video usability không thay DB evidence, automation không pass cảm giác. Art cue/quest overlay/compactrequirements và 26logical giữLOCKED cần manual review.
