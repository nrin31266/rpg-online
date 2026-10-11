# Movement + Camera correction probe — 2026-10-12

**Revision note:** phần A–I dưới đây lưu initial correction pass để giữ audit/history. Feedback feel tiếp theo đã supersede early-ascent/fast-double-tap timing; contract/probe hiện tại ở [Follow-up](#ascent-followup), không dùng các số initial bên dưới để nghiệm thu revision mới.

Execution/audit record, không gameplay owner hoặc production tuning lock. Contract hiện hành thuộc [Combat & Character](../01-design/combat-and-character.md#movement-direction); acceptance thuộc [Playtest](playtest-and-balance.md#movement-camera-acceptance). Chưa commit/push.

## A. Files changed và baseline được giữ

- `apps/client-unity/Assets/_Sandbox/Scripts/PlayerTestMovement.cs`: mở rộng prototype đang sửa, giữ arrow/legacy input, fall-gravity experiment và GroundTilemap support check.
- `apps/client-unity/Assets/_Sandbox/Scripts/CameraFollowTest.cs`: mở rộng file untracked đã tồn tại, giữ target/autowire/X follow; thay Y policy.
- [Combat & Character](../01-design/combat-and-character.md#movement-direction): semantic owner + camera acceptance direction.
- [Gameplay Runtime](../02-technical/gameplay-runtime.md#movement-feel): authority/input/lifecycle/camera integration, link owner.
- [Art & Visual Production](../03-art/art-and-visual-production.md): reconcile presentation wording, giữ Jump2/Fall2/26 samples/PPU32.
- [Playtest & Balance](playtest-and-balance.md#movement-camera-acceptance): JUMP-01..07/CAM-01..05, evidence boundaries và exact probe values.
- [Roadmap](roadmap.md#consolidation-slices): Slice 2 và routing/dependencies đổi sang Two-Stage Jump / Drop.
- [World & Content](../01-design/world-and-content.md) và [Quests & Narrative](../01-design/quests-and-narrative.md#q2-journey): chỉ bỏ traversal wording cũ, giữ route/quest/topology.
- `apps/client-unity/Assets/_Sandbox/Scenes/ArtLab.unity`: final scene đã serialize probe fields mới; delta so với snapshot ban đầu được ghi rõ dưới đây.
- `tools/probes/movement_camera_probe.cs`: repeatable disposable numerical fixture; [JSON results](probes/2026-10-12-movement-camera.json) + báo cáo này.

Trước sửa, working tree đã có ArtLab scene, PlayerTestMovement, AI Assistant Settings sửa và CameraFollowTest + meta untracked. Agent không gọi save scene hoặc sửa AI settings. Tuy nhiên scene trên đĩa được lưu trong lúc pass đang chạy: đối chiếu snapshot đầu pass chỉ thấy serialized movement/camera probe fields mới và Pixel Perfect reference resolution đổi 384×215 → 360×220; PPU32 và các phần scene khác giữ nguyên. Không đủ bằng chứng quy kết nguồn save; giữ delta đang có, không revert. ArtLab SHA256 đầu pass `a9f829ebe2272b4e7dc4ca7d35622ce8ded7e3c8aa4069c9be04edf3bdc540bc`, cuối pass `c65aed874e101a541bda1f292418e76401ff01646aa313a5808362fc0e413ba7`. AI settings SHA256 trước/sau `721d60e4fef797f8c46010eb874f873ef47bb9ace2625fa4316cf6b579a90dad`. Camera meta được giữ. Docs README không có old jump rule nên không đổi. Archives/history giữ nguyên.

## B. Stale-reference audit

Line numbers dưới đây là **baseline trước pass**, không current line links. Các dòng rule đang có hiệu lực hoặc routing dựa rule cũ được reconcile; không sửa lịch sử để xóa quyết định cũ.

| Baseline file / line | Reference cũ | Reconciliation |
| --- | --- | --- |
| Combat & Character 292 | Feel gate fixed height, hold/release không đổi apex | Link owner mới; coyote/buffer chỉ Stage1; feel/timings TUNABLE |
| Combat & Character 353 | LOCKED fixed profile, hold/release không đổi upward/apex | Thay bằng user-approved two-stage semantic + supersession note |
| Gameplay Runtime 439 | Normal Jump fixed height / cùng launch profile | Link semantic owner; input continuity, jump instance, authority/lifecycle; không duplicate gameplay rule |
| Art 985 | Tap/hold cùngapex | Stage1/extension presentation; giữ Jump2/Fall2; physics stage khác logical visual samples |
| Roadmap 168 | Inventory → fixed Jump; trước Q2 để không dựa hold apex | Inventory → Two-Stage Jump / Drop, trước Q2 để kiểm cả hai envelopes; dependency ordering giữ |
| Roadmap 212 | Slice 2 Fixed normal Jump | Rename Two-Stage Jump / Drop và camera vertical probe |
| Roadmap 217 | Một press/profile như contract duy nhất | Current-instance shared one-shot extension; coyote/buffer chỉ Stage1 |
| Roadmap 219 | Migration bỏ hold/release apex tests | Thay bằng JUMP/CAM cases mới; giữ bỏ Q2.dropped/natural one-way |
| Roadmap 225 | Rigidbody tap/hold cùng apex/time | Tap base, hold/second-press extension, invalid triggers/reset + camera cases |
| Roadmap 228 | DoD fixed profile | Semantic invariants + physics logs + manual feel/camera/pixel review |
| Roadmap 368 | Fixed Jump là slice tiếp theo | Two-Stage Jump / Drop trước Q2, ordering giữ |
| Playtest 648 | Fixed Jump/Drop và same-apex acceptance | Two-Stage Jump / Drop / Camera, link JUMP/CAM matrix |
| Playtest 653 | Q2 fixed-jump terrain | Stage1/optional Stage2 traversal, Drop optional, quest purpose giữ |
| Playtest 689 | Q-F02 fixed Jump/optional Drop | Two-stage traversal wording; item/rights/consume/reward không đổi |
| World & Content 78 | Route bậc solid + jump cố định | Link Stage1/optional Stage2 owner; return/one-way optional giữ |
| World & Content 373 | Fixed Jump tới hai Bù Nhìn | Two-stage reachability theo movement owner; placement/quest giữ |
| Quests & Narrative 196 | Blockout fixed Jump/camera | Stage1/optional Stage2 blockout/camera/pickup/return validation |

Other matched wording được phân loại: Jump2/Fall2 logical samples là **visual contract**; historical release-clamp/variable-jump source/tests trong archives và Actual reference là **PROTOTYPE-ONLY/HISTORICAL**; forces/windows/soft zones/smoothing/pixel technique là **OPEN/TUNABLE**. Current acceptance nói hai trigger ở khác ascent times có thể có apex hơi khác; đây không giữ old same-apex rule.

## C. Semantic mới đã viết trong owner

> Một grounded jump có optional Stage-2 extension một lần. Stage 2 được kích hoạt bằng deliberate short hold qua threshold hoặc fast second press trong early ascent; đây là extension của cùng cú nhảy, không phải second airborne jump.

Owner ghi đủ Grounded → Stage1Ascending → optional Stage2ExtendedAscending → Falling → Grounded; Stage1 tap ngắn đủ; hold liên tục phải qua minimum threshold từ accepted jump start; second tap phải trong đúng current jump/window và ascending. Cả hai consume cùng flag; không cộng lần ba, không full velocity reset, auto jump, continuous/infinite hold scaling, air reset hoặc extension khi falling/ceiling/window hết. Landing reset. Coyote/buffer chỉ Stage1; timing/forces không lock.

## D. Sandbox code

Movement thêm jump start clock, jump-in-progress/one-shot/continuous-hold state và read-only VerticalVelocity; giữ IsGrounded. Input route chung `ProcessJumpInput` dùng bởi Update; hold không extend ngay frame đầu. Stage2 cộng `jumpForce * stage2VelocityFraction`, không gán full base launch. Enter/Stay ground contact dùng upward normal hiện hành; remaining takeoff contact khi vy còn dương không reset jump state. Landing clear extension. Fall multiplier2.5 giữ nguyên; không thêm coyote/buffer/DropThrough/New Input System production controller.

Camera đọc player Y/vy, upper/lower zones và capped look-ahead; target Y cập nhật khi airborne. Smoothing bất đối xứng; landing settle. Safety catch dựa current orthographic view cho long/fast fall; không sửa map confiner/bounds. Optional final-position pixel quantization mặc định **off**, PPU32; smoothing state giữ subpixel riêng. Đây là A/B probe, không accepted pixel fix.

## E. Camera trước → sau

| Trước | Sau / evidence |
| --- | --- |
| Grounded recenter là nhánh chính; airborne chỉ theo khi quá xa bằng một limit đối xứng | Position/vy + asymmetric zones cập nhật Y khi đang ascending/falling |
| Short hop giữ Y nhưng exception không phân trajectory | Stage1 numerical max Y drift 0.00253u trong fixture |
| High ascent có thể đợi landing/large distance | Stage2 fixture bắt đầu reveal khi còn vy>0; max camera drift 0.58161u |
| Long fall cùng threshold với lên | Lower zone/look-ahead + slower smoothing; follow trước landing, root cách camera tối đa4.5u ở half-height5u/margin0.5u |
| Chuyển tầng chủ yếu sau grounded | Synthetic low→middle→high→middle→low bắt đầu cả bốn transitions trước landing |

## F. Exact PROBE/TUNABLE values

| Movement | Giá trị chạy probe |
| --- | --- |
| Base launch / horizontal speed | 7 / 5 u/s trong scene |
| Base gravity / gravityScale / falling multiplier | −9.81 u/s² / 1 / 2.5 |
| Hold threshold / Stage2 window / double-tap window | 0.14 / 0.26 / 0.22 s |
| Additive extension | 18% base velocity = +1.26 u/s |
| Fixed dt | 0.0199999921 s |

| Camera | Giá trị chạy probe |
| --- | --- |
| Vertical offset / upper/lower zones | 0.8 / 1.85 / 2.3 u |
| Up/down look-ahead / cap | 0.08 / 0.06 s / 0.35 u |
| Up/down/landing smooth time | 0.18 / 0.28 / 0.2 s |
| View half-height / safe margin | 5 / 0.5 u |
| PPU / optional quantization | 32 / off |

Source jumpForce default12 là edit đã có trước pass; actual serialized scene vẫn7, không bị override. Camera zones hiện được probe cho scene7; tạo component mới/default12 hoặc đổi lens cần retune, không suy acceptance chung cho mọi setup. Threshold0.14 được quan sát ở tick0.16 vì sample dt; double tap cùng tick tạo cùng boost/apex. Probe không canonicalize exact numbers.

## G. Cases và execution

[Acceptance matrix](playtest-and-balance.md#movement-camera-acceptance) thêm đủ JUMP-01..07/CAM-01..05. [Raw JSON](probes/2026-10-12-movement-camera.json) ghi source hashes/values/fixture notes. Unity6000.5.9f1 numerical probe: **12 checks, 0 failures**.

- JUMP-01/04/05: no extension, apex rise2.42788u; falling press không đổi vy.
- JUMP-02/03/06: một extension, rise3.19343u; hold và double tap **cùng trigger tick** có cùng policy/result; tăng31.53% so base. Trigger khác thời điểm có thể đổi apex một chút, không phải shared-policy failure.
- JUMP-07: hai jumps/hai landings, mỗi jump được extend một lần; holding không auto-start.
- CAM-01..03: numerical short-hop/reveal/real long-fall invariants verified.
- CAM-04: numerical synthetic four-leg trajectory verified; chưa physical staircase route/landing destination acceptance.
- CAM-05-grid-only: optional quantization grid verified, **CAM-05 visual acceptance CHƯA CHẠY**; chưa có animated real-art shimmer review, snapping giữ off.

Fixture là real isolated PhysicsScene2D + box floor đặt tên GroundTilemap, dùng real sandbox contact callbacks/resolver/camera; không authored tilemap/capsule/map topology. Press/held booleans inject vào resolver, không keyboard hardware. Numerical pass không nghiệm thu feel, actual keys, representative art/resolution, render30/60/120 FPS, ceiling/terminal constraints hoặc G-L/G-N.

Chạy lại từ repository root, Editor đang Play:

```sh
unity command eval_file --file "$PWD/tools/probes/movement_camera_probe.cs" --timeout 20000 --json
```

Report runtime tạm ở `/tmp/huyenlo-movement-camera-results.json`; archive JSON trong repo là evidence của run này. Fixture scenes cleanup; chỉ ArtLab còn mở. Editor trở lại Play như trạng thái lúc bắt đầu; agent không gọi save dirty scene. Final readback: ArtLab, camera half-height5u, Pixel Perfect reference360×220, PPU32.

## H. Unresolved interactions

- **Coyote/buffer:** concepts giữ, chỉ Stage1; sandbox chưa implement. Production boundary tests/durations/held buffer handling cần gate.
- **DropThrough:** riêng ↓; giữ one-way Drop priority/per-actor ignore/restore lifecycle; sandbox hiện chưa resolver. Không nhận numerical run này làm Drop acceptance.
- **Pixel Perfect:** PPU32 giữ; optional camera grid test không chứng minh không shimmer. Real motion/resolution/art/pipeline A/B và human review pending.
- **Network authority:** docs yêu cầu authority tick/window/one-shot/current actor life/map/jump identity, stale/replay rejection; message representation/latency/correction cần G-N, chưa có ≥2-client evidence.
- **Camera route/feel:** CAM-02 destination readability/CAM-04 physical staircase và short-hop feel vẫn manual pending.

## I. Validation

- Current canonical search reconcile toàn references trong B; archive visual/history giữ nguyên. Remaining same-apex wording trong acceptance là explicit rejection của invariant cũ cho khác trigger time; audit report này chỉ historical trace.
- `python3 tools/check_doc_links.py --json`: 630 local links, 0 broken anchors, 0 missing files.
- Scoped `git diff --check -- docs apps/client-unity/Assets/_Sandbox/Scripts tools`: clean. Added files kiểm whitespace riêng.
- Full `git diff --check`: hai trailing-whitespace lines ArtLab271/297; cùng hai serialized `m_Name` whitespace đã có ở baseline271/288, line thứ hai dịch sau fields mới. Scoped files sạch; không sửa whitespace scene ngoài scope. Scene snapshot comparison ghi delta chính xác trong A.
- Unity recompile completed, failed=false, errors=[]; Console ground truth0 errors/0 warnings và không error entries mới sau cursor74. Historical captured errors trước pass không phải current compile result.
- Không sửa combat/PrimaryAction/skills/equipment/26 samples/PPU32/Q8-Q10-Q11/economy/persistence/mobs/topology/authority model/input migration hoặc production map tooling. Không commit/push.


<a id="ascent-followup"></a>

## Follow-up — relaxed ascent second press và deliberate hold

**Feedback/approved direction:** khó chủ động jump thấp với hold0.14s; second press không cần quá nhanh, miễn actor chưa sát chuyển qua falling. [Current owner](../01-design/combat-and-character.md#movement-direction) thay early-ascent/fast-double-tap contract; một jump/optional shared one-shot/landing reset vẫn giữ.

- Hold threshold **0.40s PROBE**, liên tục từ accepted Stage1; release trước đó chỉ Stage1.
- Bỏ `doubleTapWindow` và timed `stage2Window`. Second press hợp lệ khi upward velocity còn **>0.50u/s PROBE**, chưa consume, đúng current jump; ở gravity9.81/base7 tương ứng khoảng0.05s trước apex. Không hardcode timeout theo launch force.
- Window đóng vĩnh viễn trong current jump khi vy xuống cutoff; bounce/external upward force không mở lại. Stage1 mới reset window. Cộng cùng +1.26u/s, không full launch/reset.
- Camera code/values, base force, fall experiment, input, Pixel Perfect và unrelated files giữ nguyên trong follow-up.

**Files trong follow-up:** PlayerTestMovement, ArtLab serialized jump settings, Combat & Character, Gameplay Runtime, Art & Visual Production, Roadmap, Playtest & Balance, fixture hiện hành, báo cáo này và [new raw JSON](probes/2026-10-12-movement-camera-ascent-followup.json). Scene được agent save lần này sau compile để persist hai probe settings mới. So snapshot ngay trước follow-up: chỉ thay hold0.14→0.40, bỏ hai timeout và thêm ascent cutoff0.50; không có scene delta khác. Initial JSON giữ nguyên như bằng chứng revision đầu; source hashes của fixture/script mới nằm trong new JSON.

**Execution:** Unity6000.5.9f1, fixed dt~0.02s, **15 numerical checks / 0 failures**:

| Case | Kết quả mới |
| --- | --- |
| Single tap + short hold~0.25s (release tick0.26s) | No extension, rise2.42788u |
| Deliberate hold | Threshold0.40s, observed tick0.42s; one extension, rise2.86583u (+18.04%) |
| Second press cùng tick hold0.42s | One extension, cùng rise2.86583u |
| Relaxed second press0.50s | One extension, rise2.76503u; hợp lệ ngoài cả timeout cũ0.22/0.26s |
| Near-apex press0.68s / falling press0.90s | No extension, same Stage1 baseline |
| Hold + another press / landing reset | One per jump; hai jump lives/hai extensions/hai landings |
| Closed window + upward velocity restored | Cutoff press bị từ chối; window không mở lại |
| Camera checks | Short-hop drift0.00253u; hold reveal trước apex, drift0.25386u; long fall safe/root distance≤4.5u; synthetic staircase/grid invariants giữ |

Acceptance cập nhật JUMP-03 relaxed press, JUMP-04 near-apex rejection; thêm JUMP-08 short hold và JUMP-09 latched cutoff. Numerical results không chứng minh đã đạt cảm giác chơi; bạn cần thử timing0.40s với keyboard thật. Physical staircase/pixel shimmer/network/coyote/buffer/DropThrough giới hạn vẫn như H; chưa tuyên bố manual acceptance.

**Validation:** recompile completed, failed=false, errors=[]; Console ground truth0 errors/0 warnings, compiling=false. Current canonical không còn fast-second-press/early-ascent timeout rule; initial report/JSON được label historical. Scoped diff sạch; hai pre-existing ArtLab m_Name trailing-whitespace lines271/297 giữ. Link checker sau cập nhật:633 links,0 broken anchors,0 missing files; source hashes mới khớp. Editor trở lại Play ở ArtLab. Không commit/push.


### Height tuning tiếp theo — cú đầu thấp nhẹ, extension rõ hơn

Theo feedback tiếp theo, ArtLab đổi **base launch7→6.85u/s**, **extension fraction0.18→0.28** (+1.918u/s). Hold0.40s/ascent cutoff0.50u/s giữ. Đây là serialized scene probe; source defaults/component mới chưa retune, không production-lock.

[Raw height-tune evidence](probes/2026-10-12-movement-camera-height-tune.json):15 numerical checks,0 failures. Single tap/short hold rise **2.32322u** (trước2.42788u, giảm4.31%); hold extension rise **3.02552u** (trước2.86583u, tăng5.58%). Chênh giữa base và hold extension **0.70230u**, khoảng22.47world pixels ở PPU32; visibility/feel vẫn cần người chơi thử. Relaxed second press0.50s rise2.87208u; thời điểm trigger khác cho apex khác.

Scene save diff chỉ hai scalar jumpForce/stage2VelocityFraction; camera/input/gravity/scripts giữ. Actual source hashes nằm trong raw JSON mới; initial/ascent JSON giữ history. Editor đang Play để người chơi thử; không commit/push.
