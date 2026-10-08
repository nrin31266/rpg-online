# Huyền Lộ — Production History

> LEGACY / SUPERSEDED
> Không dùng làm nguồn triển khai hiện hành.
> Chỉ giữ để truy vết lịch sử thiết kế.

Nguồn hiện hành: [documentation map](../README.md). Các con số, claim và đường triển khai bên dưới mô tả revision lịch sử.

<a id="legacy-roadmap"></a>

<a id="roadmap-history-appendix"></a>

## Phụ lục — lịch sử roadmap và hồ sơ prototype

§7–10 giữ các bảng, số đo và routing lịch sử, kèm nhãn để phân biệt với thiết kế hiện hành. Riêng §8 có thêm bảng routing migration 2026-10-06. Các chữ CURRENT, mặc định OFF, flags/A/B và version trong phần lịch sử mô tả thời điểm cũ. Controls, cadence, NPC/quest, mật độ và terrain đã được thay ở revision tài liệu 2026-10-06; bằng chứng cũ không pass gate mới. Luật đọc GDD; thứ tự và trạng thái hiện hành đọc §1–6.

<a id="7-trace-roadmap-cũ--dữ-liệu-giữ-để-đối-chiếu"></a>

## Trace roadmap cũ — dữ liệu giữ để đối chiếu

Chuyển nguyên từ Technical §10/asset gate trước consolidation. **Không phải lịch đang điều hành.** Thứ tự backend-first và Sword/Bow art cùng lúc đã được thay bằng VS-1 → G-N → G-D. Gates/giờ cần đo/cut rationale bên dưới vẫn có giá trị; mốc Week/160–240 h/12 modules là giả định lịch sử, không thông số mới được duyệt. Chi tiết đầy đủ art accounting hiện dùng để probe nằm ở Art §23, trạng thái ở Analysis A01/A17.

<a id="legacy-tech-spike"></a>

## Gate backend/dedicated cũ

**Gate đầu: architecture spike** — PostgreSQL + Spring Boot login/character/ticket + Unity Dedicated build + hai Clients; kiểm ticket một lần, duplicate connection/lease, SafeAnchor/checkpoint + resume ngắn, movement/portal/MapId, một kill reward và một loot claim commit idempotent. Chạy thêm 3–4 Clients để tìm fixed-pair assumptions, đo backend latency, không công bố capacity từ ca pass. MPPM dùng khi iterate, acceptance vẫn cần standalone server/clients. Đo giờ build/integration thật trước cam kết lịch; nếu ticket, DB commit, checkpoint hoặc headless physics sai thì sửa trước mở content.

<a id="legacy-calendar"></a>

## Bảng lịch tám tuần cũ và giới hạn estimate

| Tuần | Milestone | Gate |
| --- | --- | --- |
| 1 | Architecture spike: DB/backend/login/ticket/dedicated build | Hai Clients, MapId, một character load và một reward/claim commit idempotent |
| 2 | Combat slice / rig pipeline | Normal, Lv 5 single +Lv 10 evolution fixtures, target fallback, separation / melee miss / projectile, một modular family |
| 3 | Progression / persistent items | EXP / attributes, class reset, Food / Potion / Death, PostgreSQL inventory/loot/sell; contribution và cửa nhặt đã rõ |
| 4 | Gear / shop / quest framework | Enhance / chuyển giao sáu slots, backend transactions, Q1–Q8 / manual grants / forced variant, recipient policy / quest assist; scope checkpoint |
| 5 | Cụm quái / remaining skills | Năm farm roots / density matrix TEST, bảy fixed identities / sáu rigs / Linh Biến cap1, hai active +hai nội tại / class / evolution / manuals |
| 6 | Boss / co-op / recovery | Shared pile / claim / reset / Cuồng Mạch, Q11 / Q12; backend outage/retry và restart semantics, lịch Boss trên scene |
| 7 | PvP / chat / story / demo | Q9 optional, escrow/settlement idempotent, potion quota, match 120 s, banners/summaries, account/character fixtures |
| 8 | Integration / QA / package | Regression, evidence tối thiểu 2 concurrent players, build / scripts / video fallback |

Lịch tám tuần phía trên là **thứ tự gate lịch sử, không phải lịch hiện hành hoặc cam kết thời gian**. Mốc 160–240 h cũ được tính cho player-host/JSON nên không còn là estimate hợp lệ sau khi thêm Spring Boot, PostgreSQL, auth và dedicated build. Đo riêng giờ backend/schema/migration, Unity server build, integration/recovery và editor/art tại slice đầu rồi lập lại ngân sách; báo trượt milestone hoặc scope P1 minh bạch. Không giảm sáu base rigs / hai class / 26 frames user-lock để ép lịch.

<a id="legacy-workload"></a>

## Workload, cut ladder và reuse rationale cũ

| Workload cần đo tại slice | Unit / reuse | Evidence phải ghi |
| --- | --- | --- |
| Character rig | Một male rig, 26 frames; chung pivots / frame controller | Giờ clean / slice / import, lỗi flip / socket / pose |
| Gear visuals | 12 modules = 3 bands × Sword / Bow / Armor / Pants | Giờ trên module đầu, phần reuse so redraw; không nhân 21 templates thành rigs |
| Icons / data | 21 regular gear + Mộc Kiếm; 6 manuals dùng 2 motifs × 3 accents; consumable / material riêng | Template count, icon mới / reuse (thêm bốn passive icons từ hai motifs), validation bindings / stat evaluator |
| World | 5 farm rooms + 3 support roots; pocket / slot budget theo GDD | Giờ placement / route / aggro / vertical / portal QA trên room đầu rồi extrapolate |
| Mobs / Boss | 6 base sets +1 wolf palette, 7 identities, shared Linh modifier, 1 Boss | AI / config reuse, telegraph / collision / scheduler integration |
| UI / network / backend | HUD/modal, ticket/lease, ownership claims, DB transactions, dedicated build | Editor/prefab/backend integration hours, latency/crash/standalone test |

**Contingency / cut ladder:** review sau Art Vertical Slice, cuối Week 2 và Week 4 scope checkpoint, bằng actual spent / remaining hours. Cắt P1 / P2 trước; sau đó giảm cosmetic polish / additional sound / VFX variations, elaborate Journey summary presentation (giữ scores / summary core), Storage presentation depth (giữ 40 slots / basic deposit-withdraw), extra UI polish / QoL. Không tự cut hai classes, sáu base rigs/bảy identities, 26-frame rig, Lv 1 → 20 hoặc multiplayer core; không demote PvP / Q9 / MapChat nếu chưa user approve. Nếu vẫn vượt 160–240 h, đổi schedule hoặc trình concrete scope tradeoff để user quyết, không gọi nghiệm thu phần thiếu là done.

Linh Biến tái dùng normal sprites nhưng vẫn cần shared aura và integration / QA; không suy ra giảm giờ vẽ hai bộ Elite riêng vì baseline vốn không có các bộ đó. Đo full rig / family / farm-room ở Week 2 và actual placement / route / QA cho layout mới trước cam kết budget; bỏ anchors / timers cũ không chứng minh tổng giờ giảm.

**Chú thích trace:** câu “Nếu vẫn vượt 160–240 h” và các Week ở đoạn cũ trên chỉ phản ánh điều kiện lịch sử. Ngân sách hiện hành phải lập từ giờ khả dụng của nhóm bốn người trong hai tháng và số đo slice; không dùng 160–240 h làm trần hay tổng person-hours.

<a id="legacy-art-gate"></a>

## Asset gate cũ — trace của yêu cầu thử cả Kiếm/Cung

Asset gate: làm một full rig 26 frames, một Sword / Bow / Armor / Pants family và một farm-room trước sản xuất đủ ba gear bands. Kiểm outline / palette / pivot / frame sync khi chuyển gear. AI-generated bitmap nếu dùng vẫn cần slice / clean / import / QA; chưa tạo asset trong vòng docs này.

Gate art CURRENT đã tách theo class ở §2–3: Kiếm thử trước, Cung thử trước production branch Cung. Câu gate cũ ở trên được giữ để không mất history, không dùng làm điều kiện chặn VS-1. Tương tự, Week 2/4 cũ được map sang các checkpoint quản lý §4.

<a id="source-destination"></a>

<a id="8-source--destination--bản-đồ-bảo-toàn-nội-dung"></a>

## SOURCE → DESTINATION — bản đồ bảo toàn nội dung

<a id="canonical-migration-2026-10-06"></a>

## Migration canonical 2026-10-06

Đây là routing của lượt migration hiện hành từ master prompt và các quyết định owner đã chốt. Nội dung luật, số tính lại và contract nằm tại các đích canonical; bảng này chỉ ghi cách xử lý để review, không tự chứng minh gate đã pass. Những bảng consolidation cũ phía sau tiếp tục giữ làm lịch sử.

| SOURCE cần thay / đồng bộ | DESTINATION sở hữu nội dung hiện hành | Cách giữ và trạng thái kiểm |
| --- | --- | --- |
| Controls chọn+cast, aliases và focus cũ | [GDD input](../01-design/combat-and-character.md#focus-input), [Technical contract](../02-technical/gameplay-runtime.md#input-contract), [gate revision](../04-production/playtest-and-balance.md#revision-validation) | Thay bằng chọn/thực thi riêng, giữ focus hợp lệ khi chết; controls cũ ở §8/§10 có nhãn lịch sử |
| Cadence S1/S2/S3, HP Cung và gear HP/MP cũ | [GDD combat](../01-design/combat-and-character.md#class-combat), [gear](../01-design/items-and-economy.md#gear-economy), [Analysis probe mới](../04-production/playtest-and-balance.md#current-balance-probe) | Baseline mới TUNABLE; số cũ giữ tại Analysis với nhãn LEGACY. Tính lại không phải runtime acceptance |
| Density 28/66, crowd và capability quái | [GDD world/crowd](../01-design/world-and-content.md#world-farm), [Analysis farm](../04-production/playtest-and-balance.md#farm-progression), [Technical combat](../02-technical/gameplay-runtime.md#combat-data) | Giữ seed IDs/quest sources; re-author mật độ, tổng và Hybrid count OPEN; kiểm Home/Walk/Return dùng chung |
| Dốc, đất one-way và nhu cầu climb cũ | [GDD terrain](../01-design/world-and-content.md#terrain-rules), [Art map](../03-art/art-and-visual-production.md#map-visual), [Technical maps](../02-technical/gameplay-runtime.md#maps) | Solid tự nhiên trực giao, one-way kết cấu hiếm, không climb; hình/layout VS-1 cũ chỉ là reference |
| Tám NPC, Q3/Q6 và NPC cuối truyện cũ | [GDD quest](../01-design/quests-and-narrative.md#quests-story), [Art NPC](../03-art/art-and-visual-production.md#npc-visual), [slice/gates](../04-production/roadmap.md#vs-1) | Bảy NPC theo khu chức năng; mentor phái giữ class transaction/turn-in Q6. Quest IDs, thưởng/credit/anchors được giữ |
| Scope slice bị hiểu thành scope quest / preset debug | [Technical Dev Mode](../02-technical/gameplay-runtime.md#dev-mode), [DEV SPEED / acceptance](../04-production/playtest-and-balance.md#dev-speed-acceptance), [phase gates](../04-production/roadmap.md#phase-gates) | Đủ 12 definitions/tuyến production; Q1–Q6 chỉ là slice đầu. Full fresh-run từng phái không dùng preset |
| CURRENT/evidence/prototype bị hiểu thành production | [TARGET/CURRENT](../04-production/roadmap.md#target-current-deferred), [hồ sơ lịch sử](#prototype-visual-review), [README routing](../README.md) | Docs sync 2026-10-06, không bump prototype; old evidence không pass revision mới. Local-first → G-N ≥2 sớm, giữ TARGET N người |

## Trace consolidation trước cleanup

Bảng dưới ghi lượt consolidation trước cleanup. Một số detail Art nay nằm trong phụ lục; anchor đích cũ vẫn giữ. Vị trí cleanup cụ thể ở [Art — bản đồ MOVE](art-history.md#cleanup-source-destination).

Mọi MOVE đã ghi đích đầy đủ trước khi rút nguồn; nguồn còn summary/link. GIỮ nghĩa là không xóa khi không có đích tốt hơn. Các bảng luật/evidence được giữ nguyên; khác biệt authority/context được ghi để tránh hai nơi cùng sửa số. Bảng lịch cũ/sync proposal cũ vẫn có nhãn trace; không thành lịch hoặc quyết định mới.

| Mã | SOURCE | DESTINATION | Cách giữ / merge / summary + reference |
| --- | --- | --- | --- |
| T01 | Technical §10: gate backend-first cũ | [5 — legacy-tech-spike](#legacy-tech-spike) | MOVE đầy đủ, đánh dấu trace; G-D hiện hành giữ kiểm thật |
| T02 | Technical §10: bảng tám tuần + estimate | [5 — legacy-calendar](#legacy-calendar) | MOVE nguyên bảng/giới hạn; lịch CURRENT mới ở §4 |
| T03 | Technical §10: workload/cut ladder/reuse | [5 — legacy-workload](#legacy-workload) | MOVE đầy đủ; 12 modules/giờ cũ giữ nhãn lịch sử |
| T04 | Technical §8: asset gate Sword/Bow cũ | [5 — legacy-art-gate](#legacy-art-gate) | MOVE nguyên gate; CURRENT thử Kiếm trước |
| T05 | Art §10: timing/probes/occupancy/normal | [3 — art-combat-timing-evidence](design-history.md#art-combat-timing-evidence) | MOVE đầy đủ bảng/phép tính/giả định; Art giữ kết luận+link |
| T06 | Art §10.1: sustain/TTK probe assumptions | [3 — art-sustain-evidence](design-history.md#art-sustain-evidence) | MOVE đầy đủ bảng và giới hạn, không retune |
| T07 | Art §10.2: proc cadence/feedback | [3 — art-proc-evidence](design-history.md#art-proc-evidence) | MOVE đầy đủ phép tính/reasoning; tần suất không thành uptime |
| T08 | Art §25: A01–A17 options/trade-off | [3 — art-open-decisions](../03-art/art-and-visual-production.md#art-open-decisions) | MOVE nguyên bảng; thêm trạng thái/alias gate, Art chỉ link |
| T09 | Art §20: field/timebase proposal | [2 — presentation-data](../02-technical/gameplay-runtime.md#presentation-data) | MOVE nguyên nhu cầu/schema đề xuất; còn OPEN |
| T10 | GDD §9: Visual production flow table | [4 — legacy-visual-flow](art-history.md#legacy-visual-flow) | MOVE nguyên bảng; GDD giữ yêu cầu nhìn thấy+link |
| T11 | GDD §9: art accounting 12 modules | [4 — legacy-art-accounting](art-history.md#legacy-art-accounting) | MOVE nguyên phép đếm cũ; phân biệt thiếu Mộc/fallback với S0 |
| K01 | GDD §2/§6: EXP/stat/catalog/enhance/loot tables | [1 — character-power](../01-design/combat-and-character.md#character-power) | GIỮ nguyên mọi bảng số; không đổi gameplay |
| K02 | Analysis §2: stat/TTK/MP/damage/status/PvP tables | [3 — character-evidence](../04-production/playtest-and-balance.md#character-evidence) | GIỮ nguyên mọi bảng; ghi rõ simulation cũ/giả định |
| K03 | Analysis §3: farm ladder/density/respawn tables | [3 — farm-progression](../04-production/playtest-and-balance.md#farm-progression) | GIỮ nguyên mọi bảng/giới hạn map budget |
| K04 | Analysis §4: gear/upgrade/economy/journey/Boss tables | [3 — economy-analysis](../04-production/playtest-and-balance.md#economy-analysis) | GIỮ nguyên mọi bảng và reasoning, chưa chạy runtime |
| K05 | Art §1–3: pose/weapon/Lower/Boots reasoning | [4 — player-visual](../03-art/art-and-visual-production.md#player-visual) | GIỮ đủ bảng/trade-off; chỉ làm rõ A01 là proposal |
| K06 | Art §6–9: mob/Hit/Death/Dummy tables | [4 — mob-visual](../03-art/art-and-visual-production.md#mob-visual) | GIỮ nguyên bảng và phép đếm/timing probe |
| K07 | Art §11–19: map/terrain/structure/environment/NPC/VFX/icons/UI | [4 — map-visual](../03-art/art-and-visual-production.md#map-visual) | GIỮ đầy đủ các matrix/counts/reasoning |
| K08 | Art §22–24: contract/S0/family cost/prototype matrix | [4 — production-accounting](../03-art/art-and-visual-production.md#production-accounting) | GIỮ nguyên các bảng/kịch bản; thêm Free workflow và thứ tự probe |
| K09 | Art §26: sync proposal history/dependencies | [4 — sync-history](art-history.md#sync-history) | GIỮ nguyên bảng cũ, đánh dấu trace chưa duyệt toàn bộ |
| K10 | Technical §11–12: risk/fixtures/acceptance tables | [2 — qa](../04-production/playtest-and-balance.md#qa) | GIỮ nguyên bảng; final acceptance vẫn Dedicated/backend/DB |

**Các chỉnh sửa routing/context:** GDD §0/§1/§10 thêm vai trò năm file và TARGET/CURRENT; Technical §1.1 thêm Local→Dedicated/kỷ luật kiến trúc và §10 giữ gate/setup thay calendar; Art §0 phân loại nhóm và §24 ghi thứ tự probe; README mở đường đọc 1–5. Đây là phần bổ sung/diễn đạt lại, không xóa catalog/simulation/decision history.

<a id="design-lock-sync-audit"></a>

## DESIGN LOCK SYNC — SOURCE → DESTINATION và lịch sử sync

Các replacement sau ghi review được user duyệt 2026-10-03. Đây là lịch sử sync; các mô tả select+cast, aliases, E interact và Hybrid 3/1/0 dưới đây không còn là contract hiện hành. Không giữ artifact tạm như authority thứ sáu. Lịch sử simulation/art/accounting trong Analysis/Art/§7–9 vẫn đủ số, chưa nghiệm thu runtime.

| Mã | SOURCE review | DESTINATION canonical | Cách xử lý |
| --- | --- | --- | --- |
| S01 | D01/D12, logical ranged batch | [GDD §3](../01-design/combat-and-character.md#class-combat), [Technical timeline](../02-technical/gameplay-runtime.md#combat-data) | REPLACE flight/collision damage; giữ geometry/power |
| S02 | D02/D03, focus independent facing | [GDD focus](../01-design/combat-and-character.md#focus-input), [Technical shared input](../02-technical/gameplay-runtime.md#shared-combat-input) | AUTO context-sticky; EXPLICIT pinned; range khác acquire |
| S03 | D04/D09/D28, control V6.2.0 superseded bằng F03 one-press | [Technical shared input](../02-technical/gameplay-runtime.md#shared-combat-input) | Một pipeline/lock/buffer; pending one-shot, mọi skill không repeat |
| S04 | D27/D29, accumulated slots/IDs | [GDD §3](../01-design/combat-and-character.md#class-combat), [Technical §3](../02-technical/gameplay-runtime.md#combat-data) | Sáu IDs/CD riêng, sáu books/bốn passives giữ |
| S05 | D05, Q5 Novice compatibility | [GDD §2](../01-design/combat-and-character.md#character-power) / [§3](../01-design/combat-and-character.md#class-combat) | Novice trước class kể cả Lv5; không class fourth action |
| S06 | D06/D08/D10, reaction/air | [GDD focus/input](../01-design/combat-and-character.md#focus-input), [Art §1](../03-art/art-and-visual-production.md#player-visual) | Gravity/momentum; no normal Hurt/knockback/recovery cancel |
| S07 | D14/D15R/D17, prototype boundaries | [Analysis rationale](../01-design/combat-and-character.md#design-lock-rationale) | Hybrid 3/1/0, exact unreachable/LoS A-B chưa khóa |
| S08 | D22, minimal target and slot UI | [Art §19](../03-art/art-and-visual-production.md#icons-ui), [GDD §9](../01-design/combat-and-character.md#ux-art) | World marker/mini HP; screen name/level/current-max; ba slot |
| S09 | D30, Bow AAA/resource audit | [Analysis role audit](../01-design/combat-and-character.md#design-lock-rationale) | Giữ TUNABLE numbers, rerun legacy models |
| S10 | D24, art frame/accounting | [Art §1](../03-art/art-and-visual-production.md#player-visual) / [§23](../03-art/art-and-visual-production.md#production-accounting) | Giữ 26 frame OPEN/count scenarios, không duyệt 33 pose |
| S11 | Interaction consistency mới | [GDD focus](../01-design/combat-and-character.md#focus-input), [Technical input](../02-technical/gameplay-runtime.md#shared-combat-input) | E act candidate ngay, loot không steal focus |
| S12 | D26, CURRENT stop gate | [VS-1](../04-production/roadmap.md#vs-1), [phase gates](../04-production/roadmap.md#phase-gates) | Chỉ Q1–Q6/ba map/Tân Lữ→Kiếm local, không tự mở G-N |

**LEGACY / SUPERSEDED — control acceptance V6.2.1:** A/D+arrows OR; Space/↑ jump và S/↓ drop; 1–3 select/one-press approach+cast, no J/no-repeat mọi skill, locked slots2/3 trong route. One-action/snapshot/latest buffer, AUTO/EXPLICIT, pending không đổi target; release giữ one-shot, manual/focus/UI/Esc/map cancel. E candidate độc lập; EdgeExit auto không E. Manual feel/usability và revision-matched evidence bắt buộc. S2/S3/Cung production DEFERRED. Không tăng số balance để làm button đẹp; giữ role/sustain gate.

<a id="editorial-source-destination"></a>

## Keyboard prototype và operational art — 2026-10-04

Mock project ở `prototypes/VS1_EndToEnd/`, tương lai `game/` là Unity production Client/Dedicated, `backend/` là Spring/PostgreSQL; chưa dựng các codebase đó. Prototype tiếp tục dùng để thử nhanh, không mở G-B/G-N chỉ vì build mới chạy. Debug mốc/reset chỉ cho dev, route acceptance bắt đầu fresh và không dùng preset. Manual keyboard usability/feel và rig/art import vẫn cần người chơi review ở tốc độ thường.

| SOURCE | DESTINATION | Nội dung thực |
| --- | --- | --- |
| Review §2/§4 | [First Art Probe](../03-art/art-and-visual-production.md#first-art-probe) | Tám module/pose probe, source→export→import→runtime, pass/fail |
| Review §3/§5/§6 | [Art setup](../03-art/art-and-visual-production.md#first-art-probe), [DoD](../04-production/playtest-and-balance.md#art-validation) | Minimum manifest, socket A/B, asset QA |
| Review §7/§8 | [Art setup](../03-art/art-and-visual-production.md#first-art-probe) | Mini style sample và provenance |
| Review §9/§10/§15/§16 | [Art](../03-art/art-and-visual-production.md#working-spec-end), [Analysis](../04-production/playtest-and-balance.md#keyboard-prototype-review) | Version hiện hành, base trước mạng, numbering và history marker |
| Review §12 | [GDD quest](../01-design/quests-and-narrative.md#quests-story), [Technical quest](../02-technical/gameplay-runtime.md#combat-data) | Active-step-only tutorial supply, không future entitlement |
| User keyboard/debug/mock | [GDD UX](../01-design/combat-and-character.md#ux-art), [Technical input](../02-technical/gameplay-runtime.md#shared-combat-input), [prototype](../../prototypes/VS1_EndToEnd/README.md) | Menu keyboard/mouse cùng command, debug mốc/reset, project tách biệt |

<a id="feedback-source-destination"></a>

## Feedback → canonical và migration prototype

**Trace feedback 2026-10-03; những mapping/input trong bảng có thể đã bị thay bởi thiết kế 2026-10-06.** Mốc trước sửa `archive/checkpoint-46006c4` (local-only archive, không có trên origin); request feedback 2026-10-03 cho phép sửa tạm và tự xử lý inconsistency. Các đích dưới là nội dung thực, không copy feedback thành authority thứ sáu. Các quyết định numeric/feel chưa có evidence giữ TUNABLE/PROTOTYPE ở Analysis.

| Mã | SOURCE feedback | DESTINATION | Xử lý |
| --- | --- | --- | --- |
| F01 | Nấm trước Sói/catch-up | [GDD quest](../01-design/quests-and-narrative.md#quests-story), [Analysis matrix](../04-production/playtest-and-balance.md#farm-progression) | Q3 Lv3, Q4 Nấm→Lv4, Q5 Sói→Lv5; QIds/supply/recovery đồng bộ |
| F02 | Portal thường bị dùng rộng | [GDD world](../01-design/world-and-content.md#world-farm), [Technical world](../02-technical/gameplay-runtime.md#maps) | EdgeExit auto + SpecialGate, validation/checkpoint/dedup/ping-pong |
| F03 | Bỏ J/hold, tap thông minh | [GDD input](../01-design/combat-and-character.md#focus-input), [Technical input](../02-technical/gameplay-runtime.md#shared-combat-input) | One-shot pending/approach, no-repeat/no-cost/cancel/expiry/revalidate |
| F04 | Jump/drop mapping/feel | [GDD UX](../01-design/combat-and-character.md#ux-art), [Technical movement](../02-technical/gameplay-runtime.md#movement-feel) | OR aliases; coyote/buffer/variable-height/acceleration chưa khóa số |
| F05 | Melee pile/reposition | [GDD mob](../01-design/world-and-content.md#world-farm), [Technical AI](../02-technical/gameplay-runtime.md#combat-data) | Cluster đọc được, soft separation/recovery offsets; không lock ring slots |
| F06 | Prototype không codebase | [Technical discipline](../02-technical/architecture.md#architecture-discipline), [Roadmap gates](../04-production/roadmap.md#phase-gates), [prototype README](../../prototypes/VS1_EndToEnd/README.md) | Tách folder giữ meta; evidence cũ không pass revision mới; G-B trước G-N |
| F07 | Q3 waiting | [GDD quest](../01-design/quests-and-narrative.md#quests-story), [Art Dummy](../03-art/art-and-visual-production.md#mob-visual) | ≥3 placements cùng pool; HP60/25s giữ, contention OPEN |
| F08 | UI keys/usability/navigation | [Art UI](../03-art/art-and-visual-production.md#icons-ui), [GDD UX](../01-design/combat-and-character.md#ux-art) | I/C/Q default, edge arrow/name + NPC marker P0, manual UX review |
| F09 | Class Normal reasoning stale | [Analysis feedback](../04-production/playtest-and-balance.md#prototype-feedback-review), [legacy timing](../04-production/playtest-and-balance.md#balance-baselines) | Giữ bảng số, gắn LEGACY/SUPERSEDED trực tiếp reasoning; không restore Normal |

Script đếm trước/sau lines/pipe rows/headings/critical values và hash ở `prototypes/VS1_EndToEnd/PrototypeEvidence/VS1_EndToEnd/verify_feedback.py`; baseline từ commit checkpoint, audit là documentation evidence của lượt này, tách build/test/video cũ. Lấy mẫu ngẫu nhiên 8 mục và kiểm snippet đích thực; không dùng agent đếm. Không xóa bảng combat/gear/journey/Boss/art scenarios để “cleanup”.

<a id="no-loss-audit"></a>

<a id="9-no-loss-audit-của-lượt-consolidation"></a>

## No-loss audit của lượt consolidation

**Snapshot lịch sử:** các số trước/sau và mẫu kiểm dưới đây ghi riêng lượt consolidation, không phải số đo sau cleanup. Giữ nguyên để đối chiếu; audit cleanup dùng snapshot riêng và kiểm các đích MOVE hiện tại.

Mốc Git trước sửa: `archive/checkpoint-52d7a4a` (local-only archive, không có trên origin) (`docs: snapshot art analysis before consolidation`). Bản gốc và script/snapshots/metrics/migration proofs lưu tại `/tmp/huyenlo-consolidation/`; thư mục tạm không thuộc deliverable repo và có thể mất sau phiên. Các kết quả cần review được giữ ngay trong mục này, không tạo audit/plan/manifest riêng trong active docs.

Đếm bằng Python 3: dòng dùng `splitlines()`, bảng là **số dòng bắt đầu bằng `|`** (gồm header/separator), heading dùng `^#{1,6}\s`. Theo dõi literal `53.100`, `95 điểm`, `32.000`; frame dùng regex `26` + khoảng trắng/gạch nối + `frame/frames/khung`; payout là dòng có `1.800` và số nguyên `200` (nhận cả bảng hai cột). Đây là occurrences, không số lượng gameplay. File mới có baseline 0.

<!-- AUDIT_COUNTS_START -->
Số **trước → sau** (giá trị sau bao gồm chính bảng báo cáo này):

| File | Dòng | Dòng bảng | Heading |
| --- | ---: | ---: | ---: |
| 1 | 752 → 750 | 341 → 336 | 25 → 25 |
| 2 | 337 → 385 | 127 → 124 | 13 → 16 |
| 3 | 382 → 479 | 195 → 246 | 18 → 22 |
| 4 | 725 → 766 | 397 → 393 | 38 → 39 |
| 5 | 0 → 256 | 0 → 112 | 0 → 14 |
| README | 86 → 90 | 45 → 45 | 6 → 6 |

Occurrences số quan trọng **trước → sau**; thay đổi vị trí do MOVE hoặc thêm câu giải thích/audit, không đổi giá trị gameplay:

| File | 53.100 | 95 điểm | 32.000 | 26 frame/khung | 1.800 cùng 200 |
| --- | ---: | ---: | ---: | ---: | ---: |
| 1 | 1 → 1 | 3 → 3 | 1 → 1 | 5 → 3 | 1 → 1 |
| 2 | 1 → 1 | 1 → 1 | 2 → 2 | 5 → 3 | 1 → 1 |
| 3 | 1 → 1 | 4 → 4 | 2 → 2 | 1 → 3 | 1 → 1 |
| 4 | 0 → 0 | 0 → 0 | 0 → 0 | 6 → 9 | 0 → 0 |
| 5 | 0 → 2 | 0 → 2 | 0 → 2 | 0 → 7 | 0 → 2 |
| README | 0 → 0 | 0 → 0 | 0 → 0 | 0 → 0 | 0 → 0 |

EXP/điểm/Boss/payout của các file gameplay/evidence gốc giữ nguyên. Occurrences frame giảm ở GDD/Technical vì pipeline/accounting/calendar/asset gate đã chuyển sang Art/Roadmap; các khối này đã được kiểm nguyên văn. Giá trị 26 chưa được diễn giải lại thành lock mới.
<!-- AUDIT_COUNTS_END -->

Ngoài đếm, script đã kiểm **110 khối bảng gốc của file 1–4** còn nguyên trong bộ đích và 11 block MOVE đầy đủ ở file chỉ định. README giữ toàn bộ row cũ trừ ô nghĩa P0 được cập nhật từ “bản đầu” sang TARGET để VS-1 không bị hiểu là full P0; đây là thay thuật ngữ theo feedback, không mất bảng số. Chỉ sửa ô ấy trong bảng thuật ngữ. Script đếm không giao agent. Analysis mất từ 100 dòng trở lên phải dừng và hỏi; lượt này Analysis tăng vì nhận evidence/decision từ Art. Link nội bộ/anchor và `git diff --stat` được kiểm sau edit.

<!-- AUDIT_SAMPLE_START -->
Lấy mẫu bằng `secrets.SystemRandom().sample(..., 8)` từ 21 mục, lưu selection trước khi đọc: **T04, T05, T02, K02, K04, T09, T08, K05**. Root đã tự mở/đọc nội dung và ngữ cảnh tại đích, ngoài kiểm full-block tự động; không giao agent xác nhận mẫu thay mình.

| Mục ngẫu nhiên | Đích đã tự kiểm | Nội dung thực có / kết quả |
| --- | --- | --- |
| T04 | [5 — legacy-art-gate](#legacy-art-gate) | PASS — Gate 26 frames/Sword–Bow/farm-room còn nguyên, nằm trong trace và có chú thích CURRENT Kiếm trước. |
| T05 | [3 — art-combat-timing-evidence](design-history.md#art-combat-timing-evidence) | PASS — Bảng normal/core/big, occupancy 20%/5,7% và timing reasoning đủ ở Analysis; không retune. |
| T02 | [5 — legacy-calendar](#legacy-calendar) | PASS — Bảng lịch tám tuần và đoạn estimate 160–240 h được giữ nguyên dưới nhãn lịch cũ. |
| K02 | [3 — character-evidence](../04-production/playtest-and-balance.md#character-evidence) | PASS — Hàng Common III +0 35,35/26,32/20,87 và giả định PvP một chiều vẫn ở Analysis §2. |
| K04 | [3 — economy-analysis](../04-production/playtest-and-balance.md#economy-analysis) | PASS — Bảng +8 20,65 lần thử/36.542 Vàng/173,67 đá/175.479 quy đổi cùng giả định còn ở §4. |
| T09 | [2 — presentation-data](../02-technical/gameplay-runtime.md#presentation-data) | PASS — Có đủ correlation/generation/resolveClock/flight/status fields và 20Hz–50Hz–60FPS, nhãn PROPOSAL. |
| T08 | [3 — art-open-decisions](../03-art/art-and-visual-production.md#art-open-decisions) | PASS — Bảng A01–A17 options/recommendations giữ nguyên; trạng thái mới gắn gate, không LOCKED proposal. |
| K05 | [4 — player-visual](../03-art/art-and-visual-production.md#player-visual) | PASS — Bảng Kiếm hybrid 1–4 hình và Cung 3 shapes giữ nguyên; 13–25 còn kịch bản OPEN. |

Reader Testing độc lập theo skill doc-coauthoring kiểm scope/gates, authority/ACK và Art OPEN; các ambiguity immediate pose, t0 death, speed Cung, request/action IDs, room/ba map và legacy budget đã được sửa. Reader không đếm file. Chi tiết Potion ordering/cap release/time capture được giữ làm ca SPIKE/OPEN, không tự duyệt cơ chế mới.
<!-- AUDIT_SAMPLE_END -->

**Giới hạn:** kiểm đếm/khối bảng/migration/link là audit tài liệu, không nghiệm thu Unity, art đã xuất, performance, balance hoặc backend transactions. Tất cả gate runtime và mẫu art/% dùng được vẫn CHƯA CHẠY/CHƯA ĐO.


<a id="prototype-visual-review"></a>

<a id="10-hồ-sơ-bản-mẫu--không-phải-luật-production"></a>

## Hồ sơ bản mẫu — không phải luật production

**Snapshot lịch sử V6.2.7; G-L của bản mẫu cũ PARTIAL.** Layout, tọa độ mock, lịch sử V6.2.4–V6.2.7 và bằng chứng đã chuyển sang [CHANGELOG prototype](../../prototypes/VS1_EndToEnd/CHANGELOG.md#prototype-visual-review). Bản nháp V6.2.8 trong hồ sơ chưa được xác minh. Metadata docs 2026-10-06 không phải prototype version bump; không xác nhận bản nháp hoặc gate mới đã pass. Toàn bộ chi tiết được bảo toàn, gồm chỉnh sửa cảnh quan chưa commit có trước task này; tests cũ không là visual/feel acceptance.

**Snapshot vận hành trước migration:** bản chạy thử cũ dùng một hành vi đã duyệt, không ProbeConfig/flag/metric harness; F8/debug chọn giai đoạn và reset đã được khôi phục, chỉ cho prototype local. Chỉ thị khi đó là dừng A/B, evidence work, hook/Git cleanup và Phase H. Đây là lịch sử, không mở hoặc đóng gate revision mới; budget release và TARGET vẫn theo tài liệu canonical hiện hành.

| Revision | Tóm tắt lịch sử |
| --- | --- |
| V6.2.4 | User từ chối layout khối/nước; giữ làm trace. |
| V6.2.5 | User từ chối hiểu “núi” thành ngoại cảnh. |
| V6.2.6 | User từ chối slab solid, bridge clearance và cách đọc tầng đất. |
| V6.2.7 | Snapshot bản mẫu được ghi trong hồ sơ cũ; visual/feel chưa nghiệm thu, không production base. |

**Trace INPUT-01/CMB-01/MOBAI-01 của revision cũ:** A/B lúc đó mặc định OFF, [báo cáo probe](../../prototypes/VS1_EndToEnd/CHANGELOG.md#phase-e--kết-quả-ab-và-giới-hạn). Cung gameplay vẫn DEFERRED thuộc TARGET P0; chỉ có ranged fixture. Không nối network/backend vào lớp throwaway. G-B nhận findings sau review, không lấy test pass để mở G-L.

Các anchor dưới đây giữ routing lịch sử cho tài liệu read-only; nội dung thực ở CHANGELOG:

<a id="prototype-technical-history"></a>

[Technical — lịch sử probe](../../prototypes/VS1_EndToEnd/CHANGELOG.md#prototype-technical-history).

<a id="prototype-feedback-history"></a>

[Analysis — lịch sử feedback](../../prototypes/VS1_EndToEnd/CHANGELOG.md#prototype-feedback-history).

<a id="prototype-map-history"></a>

[Art — layout mock](../../prototypes/VS1_EndToEnd/CHANGELOG.md#prototype-map-history).

<a id="prototype-ui-history"></a>

[Art — UI/rig mock](../../prototypes/VS1_EndToEnd/CHANGELOG.md#prototype-ui-history).

<a id="prototype-water-history"></a>

[GDD — lịch sử tuning nước](../../prototypes/VS1_EndToEnd/CHANGELOG.md#prototype-water-history).

<a id="prototype-tooling-history"></a>

[Technical — tooling mock](../../prototypes/VS1_EndToEnd/CHANGELOG.md#prototype-tooling-history).

<a id="prototype-runtime-history"></a>

[Analysis — runtime và sequencing](../../prototypes/VS1_EndToEnd/CHANGELOG.md#prototype-runtime-history).

**LEGACY / SUPERSEDED — feedback prototype 2026-10-05 về địa hình/focus/actor:** vũng Vân Khê chữ nhật dài hơn, có đường đất one-way phía trên để chọn đi khô/lội nước; cầu giữ đường đi solid và nâng mặt nước hình ảnh, không thêm collision dưới nước. Thân đất liền, mặt cỏ/đá lát nông khác nhau; bỏ bờ tam giác và bậc vụn ở quảng trường. Dùng lại rig hình học từ `50f05ed`, cache renderer thay vì dựng mỗi frame; NPC/Sói/Dummy đặt chân đúng support. Hướng dẫn phím world chỉ một panel. Tab/click search thử ±12 u ngang/±6 u dọc; retention ±20/±10, độc lập range/vertical cast nên nhảy không mất focus. Quái có thể crossing ngắn khi recovery, nhưng không bắt đầu windup khi peer quá sát; không slot/token/formation. Đây là mô tả disposable probe của revision cũ, chưa nghiệm thu feel hoặc G-L production. Đất one-way và các vùng số thử ±12/±6, ±20/±10 không phải luật hiện hành; VS-1 giữ nguyên trong migration. Chi tiết/tọa độ lịch sử không chuyển sang docs 1–4.
