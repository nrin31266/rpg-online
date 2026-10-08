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
| Quest supply/routing hoặc Dev Mode rò vào release | HIGH | Bảy NPC, Q6 mentor, Q10–Q12 Lâm Bá; active-step virtual credit/full bag/replay. Fixtures không thay fresh journey; quyền/storage dev tách release |
| Observer/network cost | MEDIUM | Đo dead observer đúng MapId/N-player trước optimize P1; chưa công bố capacity |
| Scope P1 tăng ngoài gate | HIGH | Proposal/research không là DoD; chọn sau P0 gate, không framework hóa prototype |
