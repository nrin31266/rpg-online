# Huyền Lộ — Art History

> LEGACY / SUPERSEDED
> Không dùng làm nguồn triển khai hiện hành.
> Chỉ giữ để truy vết lịch sử thiết kế.

Nguồn hiện hành: [documentation map](../README.md). Các con số, claim và đường triển khai bên dưới mô tả revision lịch sử.

> **SUPERSEDED bởi phase 2026-10-10:** two-button Execute/Interact, Q8 force/reservation, Q8/Q10 ordinal evidence, Q11 old quotas/mob fragments/three Xích seals và multi-independent regular non-boss loot trong trace cũ không còn authority. Đọc [PrimaryAction](../02-technical/gameplay-runtime.md#active-focus), [Quest RNG/Q11](../01-design/quests-and-narrative.md#quest-collection), [natural Linh](../01-design/world-and-content.md#linh-bien), [loot](../01-design/items-and-economy.md#regular-loot-outcome). Bảng/số lịch sử giữ nguyên để truy vết.

<a id="art-rationale"></a>

## Phụ lục A. Rationale và ghi chú thiết kế

Phần này giữ phát hiện và trade-off đã dẫn tới spec. **Bảng phát hiện ban đầu là HISTORICAL EVIDENCE**: flight/spawn lệch, class Normal, bốn SkillIds và bộ MP/CD cũ đã SUPERSEDED. Những option không đụng luật mới vẫn là phương án thử; trạng thái hiện hành đọc Analysis A01–A17, không lấy chữ “OPEN” trong trace làm quyền đổi luật.

<a id="art-initial-findings"></a>

### Những phát hiện ban đầu — reasoning được giữ

| Requirement hiện tại | Vấn đề khi production | Hướng đề xuất |
| --- | --- | --- |
| Một rig nam, 26 frame đồng bộ cho mọi part | Đúng tổng ô một profile, chưa chứng minh đủ hình cho cả chém kiếm và kéo cung; index đồng bộ không đồng nghĩa mỗi part cần một PNG mới ở mỗi ô | Giữ tám state và nhịp mẫu; tách profile Kiếm/Cung, bảng ánh xạ pose và hình thực. Việc diễn giải lại user-lock 26 frame phải được duyệt |
| 12 module = ba band × bốn loại | Chưa tính visual Mộc Kiếm và outfit trước khi nhận Áo/Quần; cũng chưa tính redraw tay/áo theo class | 13 module gear world, thêm hai fallback outfit; không nhân item × 26 |
| VFX accounting cũ: bốn active presets | Sáu profile Lv 5/10/17, normal, impact, status và telegraph có nhiệm vụ đọc gameplay khác nhau | Lịch sử bốn SkillIds; hiện sáu bindings độc lập, reuse preset family; thêm các primitive dùng chung, không sáu bộ VFX độc lập |
| Attack/Skill 12 FPS, action lock 0,26–0,40 s | Ba frame mất 0,25 s, bốn frame mất 0,333 s ở tốc đều; hit 0,10–0,18 s không luôn nằm đúng biên frame. Bow cần draw/release thật | Author thời lượng từng pose theo server timeline; FPS chỉ mốc preview, không clock damage |
| Multi-target và projectile Cung | Arc/line/nổ có thể cùng mốc; ba tên hiện spawn lệch 30 ms và có thời gian bay. Không thể tuyên bố mọi skill resolve đồng thời mà vẫn giữ luật này | Một cast đọc được như một nhịp; impact theo thời điểm server. OPEN riêng nếu muốn đổi ba tên thành resolve đồng thời |
| Mob Hit/Death chưa có bảng production | Damage thường dễ bị diễn thành stun; corpse dễ kéo dài hurtbox hoặc respawn | Hit feedback chồng lên action, Death terminal tách khỏi corpse và reward |
| MapRoot thiên về bốn Tilemap | Chưa đủ mô tả công trình đứng được, nước có lớp trước/sau, route của mob | Giữ physics đơn giản; thêm nhóm structural/overlay vào quy ước authoring, không xây hệ building lớn |
| Login/Character Select đã là P0 | UI kit, trạng thái chờ/lỗi và nguồn preview chưa nằm rõ trong scope art | Reuse rig và kit; không thêm account feature |


<a id="art-boots-rationale"></a>

### Tác động nếu bỏ Boots hoặc thêm footwear visual

**LEGACY / SUPERSEDED — đề xuất bỏ Boots:** bảng sau giữ đủ tác động và số cũ để đối chiếu, không dùng làm option hiện hành. GDD giữ sáu ô và nay thêm HP trên Boots; vì vậy phép trừ chỉ số dưới đây không là phép tính cho catalog revision mới.

| Dependency | Tác động nếu duyệt bỏ | Việc bắt buộc đo/sync |
| --- | --- | --- |
| Catalog / scope | 6→5 slots; 18→15 family; 21→18 mẫu thường, cộng Mộc Kiếm thành 19 gear definitions | Bỏ ba icon nhưng **không giảm world animation**, vì Boots vốn stat-only |
| Chỉ số | Mất DEF 2/4/6, EVA 4/8/12, flat +2 EVA/cấp, Tinh Hoa Giày; mất tốc chạy +1/2/3% | Build survival/accuracy, kite/chase/run-back và PvP phải tính lại; không tự chuyển tất cả stat sang Quần |
| Loot | Các nguồn 6 slots thành 5, nguồn Lv 2/4 từ 5 thành 4 | Nếu giữ chọn đều, mỗi non-weapon ở nguồn đủ slot tăng 1/6→1/5; xác suất một loại weapon khi gear roll thành công tăng 1/12→1/10. Đây là thay economy dù tổng gear roll giữ nguyên |
| Shop / sinks | Mất dòng mua/bán và đường enhance/transfer một slot | Tính lại vendor-all, nhu cầu Gold/Stone và giá trị loot; không giả sink thực giảm đúng 1/6 vì player không đầu tư đều |
| UI / progression | Character/equipment filters/tooltip/preview/QA đổi; Q7 Nhẫn vẫn giữ | Cập nhật item refs, evaluator, fixtures, bảng Analysis và acceptance; không rename slot rồi bỏ qua stat |

**Kết luận lịch sử A03:** bỏ Boots từng được cân nhắc để đơn giản gear, không tiết kiệm world art vì Boots vốn stat-only. Đề xuất này đã SUPERSEDED; hiện giữ sáu ô và footwear của LowerBody. Hình footwear đổi theo Boots sẽ tăng layer/overlap QA và chưa được duyệt P0.


<a id="art-historical-trace"></a>

## Phụ lục C. Historical trace và các giả định trước đây

**HISTORICAL / SUPERSEDED về gameplay:** pipeline/accounting và đề xuất sync sau đây là trace trước lock. Các mô tả flight/class Normal/four SkillIds/evolution không là contract hiện hành; giữ số và dependency để đối chiếu. Các nhãn §22.1/§26 và anchor cũ được giữ để tra cứu; thứ tự làm hiện tại thuộc Roadmap.

<a id="legacy-visual-flow"></a>

### C.1. Pipeline trước consolidation — trace được giữ

Bảng sau chuyển nguyên từ GDD §9 để bảo toàn trình tự production cũ. Chi tiết art do file này sở hữu; cách gắn runtime/physics do Technical sở hữu. Cụm `slice 26 frames` là shorthand lịch sử của contract; **không giải quyết A01 hoặc buộc mỗi part có 26 PNG**. Dùng §1/§22 và gate A01/A02 trước production.

| Pipeline | Thứ tự và ranh giới |
| --- | --- |
| Nhân vật | Source sprite → canvas 64×64 / PPU 32 → slice 26 frames → chung pivot chân → male BodyBase / HairHead / Pants / Armor / Weapon → đồng bộ state / frame → palette / accent ba gear bands → actor SortingGroup → status / VFX overlays. Hurtbox / collider độc lập visual, đổi gear / scale Linh không đổi physics. |
| Map | Forest / Mountain / Ancient → tileset / palette → background → Ground / one-way Platform → back props → landmark → foreground → anchors quái / NPC / MapExit/SpecialGate → colliders → camera bounds → kiểm contrast / telegraph / loot / chat. Không thêm lighting framework P0; dùng màu / VFX hiện có. |
| Gắn layout với art | Đồng thoáng / sparse; Trúc nhiều tầng / cầu; Bạch bậc thác; Xích hẻm núi / ba dấu ấn; Huyền phế tích / landmark Boss. Hub reuse props; safe strips và đường về phải đọc được. |

<a id="legacy-art-accounting"></a>

**Accounting trước review (giữ để đối chiếu, không budget đã duyệt):**

Art accounting: 3 bands × (Sword + Bow + Armor + Pants) = 12 visual modules trên chung 26-frame rig, không 18 full rigs; 21 regular template icons có thể reuse motif / palette, sáu manual icons dùng hai motif class + ba accents. Một base body / hair, aura / status overlays chung; actual slicing / pose reuse cần ART-01 đo, không nhân template count thành rig count.

Phép tính 12 đúng cho ba band × bốn loại, nhưng chưa bao gồm Mộc Kiếm/fallback outfit/pose theo class. Kịch bản bổ sung và mọi giả định nằm ở §23; số 13/15/33/264/278–290 chưa thay thế user-lock hay thành manifest sản xuất.

<a id="sync-history"></a>

### C.2. Trace đề xuất sync V6.1 và điều kiện còn lại

Bảng đề xuất sync trước consolidation được giữ nguyên dưới đây để không mất dependency/history. **Không phải mọi dòng đã áp dụng.** Các nguyên tắc/owner/routing đã sync theo §0; phần đổi gameplay/26-frame/hybrid/camera/Dummy/Boots vẫn theo trạng thái Analysis Axx. Di chuyển nội dung thực tế có bảng [SOURCE → DESTINATION](production-history.md#source-destination) và audit ở file 5. Các tham chiếu §10/§25 trong bảng lịch sử chỉ evidence/decision cũ, nay đã chuyển Analysis; GDD/Technical vẫn giữ baseline nếu option chưa duyệt.

| File / vị trí hiện có | Nội dung cụ thể cần cập nhật sau duyệt | Điều kiện/dependency |
| --- | --- | --- |
| GDD §0/§9/§10 | Làm rõ lock 26 là ô/profile hay tổng hình; class pose/dedup/default outfit; 13 gear world modules thay 12 | A01/A02/P01–P03; đồng bộ acceptance modular, không âm thầm bỏ user-lock hoặc thêm item |
| GDD §1/§6/§9 | Bảy weapon visuals kể cả Mộc; silhouette ba band; Armor/LowerBody+footwear; Ring/Necklace/Boots stat-only | Item/stat/rarity tách visual; không đổi catalog/range nếu chỉ duyệt art |
| GDD §2/§6/§10 nếu bỏ Boots | 5 slots/15 families/18 regular+Mộc; DEF/EVA/MoveSpeed/enhance/Tinh Hoa; slot pool/shop/sell/transfer/fixtures/DoD | Chỉ khi option bỏ A03 được duyệt; tính lại balance/economy, không tự chuyển stat |
| GDD §3 | Executor/speed/flight normal Cung còn thiếu; timeline riêng theo class nếu retune; quyền movement/jump lúc cast; sáu profile/bốn SkillIds; semantics multi-target | A05/A06/A14; giữ fallback SnapshotSpread/status dedup/Line falloff/Hàn không double-hit trừ khi luật mới được duyệt |
| GDD §3/§4/§9 | Damage reaction không stun, CC/Death cancel, normal/flying death/corpse, status/Boss telegraph dễ đọc | Presentation không đổi hit/CC; loot ground position hoặc mốc respawn đụng gameplay phải review riêng |
| GDD §4/§9 | Structural standables/surfaces, AI reachable/leash/flying approach, cue solid/one-way, nước nông giảm tốc theo GDD, camera readability | Giữ 8 roots/28 pockets/66 slots/gates; không thêm AI Jump/hazard/swimming; mask projectile cần chốt |
| GDD §5 Q3/Q6, §4 nếu cần | Bù Nhìn chung prefab/HP/DEF/EVA/life/Break; respawn/điểm đứng nếu đổi | A07/P07; giữ ba kills/20% quest credit và training yard; không QuestId/reward mới |
| GDD §8/§9 | Local anticipation/remote phase, UI pending/Common Kit, dependency Login/Select preview | Giữ authority và screen flow; không Create/Register/Party hoặc networking feature mới |
| Technical §2 | MapRoot có structural back/front/surface colliders/water overlay; route/spawn/loot ground points/occlusion | One-way theo actor như hiện hành; spike thuật toán navigation nếu phải đổi tầng |
| Technical §3/§4 | Visual snapshot weapon/profile/revision/clock; HitResult resolveTime/life generation; projectile phase/status snapshot/cancel/dedup | A11/A15; art/socket không authority; normal Bow contract sync GDD, không thêm client trust |
| Technical §5/§9 | Nguồn Select preview; read-only visual summary nếu chọn exact gear; views pending/error/connecting | Không dùng inventory/Gold client payload làm nguồn tin cậy, không thêm admin UI |
| Technical §6/§7 | Presentation terminal-pending; deathUtc t0/finalize/respawn deadline, corpse lifetime, loot publish sau ACK | Transaction/retry giữ semantics; corpse không chặn finalize/respawn, deadline không reset theo animation |
| Technical §8 | Pose manifest thay suy luận mỗi part cần 26 PNG; default/class profiles, weapon sockets/string/front-back; import/padding/atlas/sorting/flip/pool reset | A01/A02/P15; kiểm camera/import trên pipeline được pin, không thêm package tùy tiện |
| Technical §9/§10/§12 | Kit 21 primitives khác số ảnh; layout/icons từng view; thay estimate 12 modules; đo giờ variant/import/QA, thêm ca §24 | Không budget credit/PixelLab plan; QA 3–4+ là benchmark, không capacity claim |
| Analysis §1/§2/§4 | Cập nhật derived art counts; chạy lại TTK/sustain/Boss/PvP nếu đổi timeline/CD/projectile/Boots; ghi giả định và giờ slice | TTK cũ trước phụ stat gear là mốc tham khảo, không duyệt cảm giác; formula GDD giữ authority |
| Analysis §3/§4 | Dummy contention, layout mob-platform/loot access/run-back nếu timer/route/Boots đổi; vendor/slot pool/enhance sinks nếu bỏ Boots | Không suy economy giảm đúng 1/6 hoặc spawn rate chia theo N; quest credit và reward eligibility không gộp |
| Analysis §5 ART-01/PHY-01/BAL-02/CC-01/SCOPE-01/BOSS-02/TECH-01 | Gắn OPEN A01–A17 vào đúng gate, ghi option đã duyệt và evidence còn chưa chạy | Không mở lại PvP/SAVE user locks nếu chỉ đổi presentation; không biến mọi proposal thành baseline |

**Tự review sau consolidation:** bảo toàn S0/icons/mob/timing/options/dependencies; evidence timing và sổ Axx nay ở Analysis, data proposal ở Technical, lịch ở Roadmap. Art vẫn đủ detail cho pose/weapon/map/mob/NPC/UI/import/production. Các proposal đổi gameplay và counts chưa kiểm vẫn OPEN; prototype §24 chưa chạy. Corpse/status/VFX không quyết định damage/respawn/credit; local fixture không giả success bền vững. Số liệu đếm và kiểm destination nằm ở [audit](production-history.md#no-loss-audit).

<a id="cleanup-source-destination"></a>

### SOURCE → DESTINATION của lượt cleanup

Các khối dưới được MOVE đầy đủ trong cùng file; phần chính giữ summary/link. Bảng này chỉ ghi vị trí, không đổi trạng thái approval.

| Mã | SOURCE trước cleanup | DESTINATION hiện tại |
| --- | --- | --- |
| C01 | §0 — Những phát hiện ban đầu | [Phụ lục A — phát hiện](#art-initial-findings) |
| C02 | §1.2 — Ba technique và reasoning A01/A02 | [Phụ lục A — technique](../03-art/art-and-visual-production.md#art-technique-rationale) |
| C03 | §3.1 — Dependency/option Boots | [Phụ lục A — Boots](#art-boots-rationale) |
| C04 | §23.1 — S0/pose/export/module calculations | [Phụ lục B — S0](../03-art/art-and-visual-production.md#player-s0) |
| C05 | §23.2 — Family totals/VFX/cost/QA combinations | [Phụ lục B — accounting](../03-art/art-and-visual-production.md#family-scenarios) |
| C06 | §22.1 — Pipeline và accounting cũ | [Phụ lục C — pipeline](#legacy-visual-flow), [accounting](#legacy-art-accounting) |
| C07 | §26 — Sync proposals/dependencies/self-review | [Phụ lục C — sync trace](#sync-history) |


## Conditional mob budget trước content review

| Base rig / hành vi | Idle | Move / Flying | Melee | Ranged | Hit rảnh | Death | Tổng hình nếu các ô mới đều khác |
| --- | ---: | --- | ---: | ---: | ---: | ---: | ---: |
| Nấm Linh, melee chậm 1,2 u/s | 2 | 4 co/nẩy/thân đi/lấy lại thế | 4 | — | 1 | 3 | 14 |
| Sói Sương, chase 2,4 u/s | 2 | 6, hai chân × contact/passing/lift | 4 | — | 1 | 4 | 17 |
| Ong Giáp, ranged/flying 2,0 u/s | 4 hover/wing | Reuse 4 hover, offset/tilt lúc move; **0 hình thêm** | — | 4 | 1 | 4 | 13 |
| Đoạt Mạch Đạo Tặc, hybrid nhanh 2,2 u/s | 2 | 6 bước hai chân | 4 | 4 | 1 | 4 | 21 |
| Xích Thạch Linh, hybrid nặng 1,4 u/s | 2 | 4 đặt chân/chuyển trọng lượng | 4 | 4 | 1 | 4 | 19 |
| Cổ Môn Vệ Binh, hybrid 1,8 u/s | 2 | 6 bước và áo/giáp chuyển | 4 | 4 | 1 | 4 | 21 |
| Sói Trúc Ảnh | Reuse | Reuse | Reuse | — | Reuse | Reuse | **0 pose mới**, một palette identity |

**SUY RA có điều kiện ba Hybrid:** 14+17+13+21+19+21 = **105 hình rig**, giữ phép cộng lịch sử. Nếu không chọn ranged capability, hoặc recovery dùng idle/Hit chỉ flash, số ảnh thực giảm; phải kê pose map thật. Không gọi 105 là budget hiện hành hoặc tự giảm số mà vẫn tuyên bố mọi ô khác nhau.


## Export path của probe cũ

**HISTORICAL export path:** `ArtSource/Probes/Sword01/` → `prototypes/VS1_EndToEnd/Assets/_Prototype/VS1_EndToEnd/ArtProbe/Sword01/` từng là đề xuất. Giữ để tra lịch sử, VS-1 đã được xóa khỏi checkout; đường dẫn này không dùng để chạy probe revision mới; migration này không tạo/chạy sandbox hoặc asset.
