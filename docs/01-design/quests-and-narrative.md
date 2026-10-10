# Huyền Lộ — Quests & Narrative

Các số BASELINE/TUNABLE chưa phải nghiệm thu runtime.

Bảng Q1–Q12 xác định objectives, reward, marker và người nhận/trả. Q8 dùng [Linh Biến tự nhiên, shared world](world-and-content.md#q8-bounded-path). Q8/Q10 dùng Collect RNG, không quota kill cùng nguồn. Q11 là CURRENT DIRECTION; vị trí level gate là STRONG DIRECTION / NEEDS USER LOCK, không giả một graph production đã được duyệt.

## Document owns

Lore chi tiết, reveal order, bảy NPC, interaction/dialogue, Q1–Q12 và quest credit/recovery gameplay.

## Document does not own

Luật ngoài domain thuộc owner trong [documentation map](../README.md); evidence thuộc [Playtest & Balance](../04-production/playtest-and-balance.md), thứ tự triển khai thuộc [Roadmap](../04-production/roadmap.md).

<a id="quests-story"></a>
<a id="gdd-2"></a>
<a id="gdd-6"></a>

<a id="5-cốt-truyện-và-nhiệm-vụ"></a>

## Cốt truyện và nhiệm vụ

**STRONG DIRECTION bối cảnh:** vùng sơn cước Việt Nam tiền hiện đại giả tưởng, không khóa triều đại/năm lịch sử hoặc tái dựng lịch sử. Sắc thái đi từ dân dã → hiểm trở → huyền bí. Làng gỗ, mái ngói giản lược, tre, cầu gỗ, dược thảo, đèo đá và bia/trấn ấn tận dụng ba environment families, không thêm mechanic hoặc asset family.

Vân Khê nằm trên những Mạch Ấn ngầm, nơi linh khí nuôi rừng núi và giữ phế tích yên giấc. Gần đây, gió núi mang mùi tanh, nấm mọc khác thường, sói bỏ bãi cũ. Tân Lữ là người dự tuyển, lần theo những dấu nhỏ ấy trong lúc học cách tự giữ mình; không có lời tiên tri hay danh phận cứu thế. Lễ Nhập Lộ tại Lv 5 gắn lựa chọn kiếm / cung với việc chính thức bước vào đường tu luyện, bằng lời NPC và thao tác Q6, không cinematic hay quest mới.

| Trụ cột thế giới | Điều được hé lộ |
| --- | --- |
| Mạch Ấn và linh khí | Mạch Ấn bị can thiệp làm dòng linh khí lệch hướng, sinh trọc khí khiến sinh vật hung dữ. Ghi chú quest, tên vật phẩm và mô tả quái nối từng dấu vết. |
| Đoạt Mạch Đạo Tặc | Chúng đục phá Mạch Ấn để lấy linh thạch: lợi trước mắt của con người làm rối trật tự tự nhiên. Biết có đạo tặc chưa đủ kết luận nguồn gây nhiễu. |
| Cự Thú và Dư Ảnh | Huyền Nham Cự Thú là sinh linh thủ hộ cổ xưa bị trọc khí ăn mòn. Hạ nó giúp giải thoát Thủ Vệ; Dư Ảnh là tàn niệm linh lực còn đọng nơi cấm địa. |

Lời NPC ngắn, mộc mạc; không diễn giải hết bí ẩn. Lâm Bá kiệm lời, ấm áp, nhắc đường về; Bách Luyện cộc nhưng trọng người bền chí; Yên Thảo nghiêm về khí huyết và giữ mạng; Mộc An điềm đạm, nhắc nghỉ và giữ đồ; Phong Du gọn lời về thế kiếm, Diệp Lam rõ ràng về khoảng cách; Hạo Vũ sảng khoái, lấy tỷ thí làm lời chào. Những sắc thái này dùng text và nội dung hiện có, không thêm quest/NPC/asset chỉ để kể chuyện.

Chương/checkpoint và Main Story Complete thuộc [Game Design](game-design.md#chương-và-completion); quest owner giữ reveal order và objectives bên dưới.

Q12 **per character**: chưa complete hiển thị **Huyền Nham Cự Thú**, đã complete **Dư Ảnh Huyền Nham**, kể cả tracker / banner. Hai tên dùng một entity / sprite / AI / drop, không world story flag.

> **Đọc sâu:** [Playtest & Balance — review narrative](../04-production/playtest-and-balance.md#review-decisions)

## Q1–Q12 và nhiệm vụ

Q1 catch-up Lv 2, Q2 Lv 3, Q3 không EXP (giữ Lv 3), Q4 Nấm/loot catch-up Lv 4, Q5 Sói catch-up Lv 5, `CatchUp = max(0, TargetCumulativeEXP - CharacterTotalEXP)`; BaseEXP = 0 khi đã qua mốc. Q7–Q11 EXP hiện dùng 10% thanh cấp (half-up), Q3/Q6 = 0, Q12 = 0 ở cap; các số là BASELINE / TUNABLE cho QUEST-03. **Giữ 12 QuestId**: các bước ngắn đầu game phục vụ onboarding/recovery, Q9 là nhánh tùy chọn và không chặn Q10. Không thêm ID chỉ để lấp khoảng farm.

**State chung (Q11 endpoint mới là ngoại lệ OPEN, không tự ép NPC cuối):** LOCKED → AVAILABLE khi đủ prerequisite + level của graph đã duyệt (Q11 graph mới còn OPEN) → nhận → IN_PROGRESS → đủ active objectives → READY_TO_TURN_IN → đúng NPC/range → COMPLETED + reward/unlock. Ready không auto-turn-in/chuyển map; thiếu cấp thì NPC/HUD chỉ rõ mốc và bãi phù hợp. Lối cũ vẫn quay lại được.

**Objective theo hành động:** quest data lưu semantic actions như Talk/Visit/Interact/Pickup/Equip/UseFood/UseHpPotion/UseMpPotion, không lưu phím. Jump/DropThrough phục vụ traversal; Q2 không đếm input/action checklist làm objective. Tutorial/HUD lấy glyph từ binding hiện hành; key press đơn thuần không credit action thất bại.

**Marker/vùng nhiệm vụ:** mỗi ID dưới là khóa của trigger hoặc điểm tương tác trên map/NPC hiện có; Game Server kiểm MapId/vị trí. Visit radius 1,5 u TEST, Interact trong 2 u TEST (binding tại các mục liên quan). Huyền Môn có điểm đứng phía ngoài Xích Nham; có cần interaction cuối Q11 hay tự phản ứng vẫn OPEN. Dấu `+` trong objectives chỉ các mục tiêu cùng active group, không phải thêm QuestId.

| Quest / Lv | Story + người giao → trả | Objectives theo thứ tự | Reward | Unlock / next |
| --- | --- | --- | --- | --- |
| Q1 — Người mới đến Vân Khê / 1 | Lâm Bá: “Nhớ chỗ thuốc, lò rèn và đường về. Ra núi rồi, chẳng ai giữ hộ mạng mình.” Lâm Bá → Lâm Bá. | Đi tới khu dược nói chuyện Yên Thảo → lò rèn gặp Bách Luyện → nhà kho/nghỉ gặp Mộc An → nhìn lối đi ra/về làng rồi báo Lâm Bá. Các NPC ở khu chức năng riêng, không xếp cạnh nhau thành menu. | Catch-up Lv 2 + 50 Vàng | Q2 sau Q1 + Lv 2 |
| Q2 — Bước chân đầu tiên / 2 | Lâm Bá: “Đi thử một vòng. Chân vững rồi hãy cầm kiếm.” Lâm Bá → Lâm Bá. | **Direction đã xác nhận:** mini journey có mục đích tới Học Viện, lấy/xác nhận một vật thật rồi quay về; traversal tự nhiên, không objective bấm Jump/Drop. Checklist cũ `HV_Entrance → HV_JumpLedge → HV_DropLanding` **SUPERSEDED**; anchors giữ stable. Exact content/item ở [draft có typed requirements Q2](#q2-journey) là PROPOSAL, chưa được duyệt để author production. | Catch-up Lv 3 + 75 Vàng | Q3 sau Q2 + Lv 3 |
| Q3 — Vũ khí trong tay / 3 | Bách Luyện: “Cầm thử cây kiếm gỗ này. Ra sân tập cho quen tay, rồi trở lại đây.” Bách Luyện → Bách Luyện. | Nhận/mặc Mộc Kiếm → `HV_DummyYard` → hạ **4 Bù Nhìn**, mỗi life đóng góp ≥20% → báo Bách Luyện. Học Viện có **5 placements cùng loại: 3 sân chính + 2 khu phụ**, không yêu cầu bốn SpawnSlot khác nhau hoặc chờ respawn; HP 60, không đánh/trả thưởng; respawn 25 s TEST. | Mộc Kiếm cấp trước một lần; turn-in Quần Thanh Mộc I, 0 EXP | Q4 sau Q3 + Lv 3 |
| Q4 — Chiến lợi phẩm đầu tiên / 3 | Bách Luyện: “Thứ mặc được thì giữ. Thứ thừa đem bán, lấy đồng lộ phí.” Bách Luyện → Bách Luyện. | Theo gợi ý tuyến `DS2_MushroomPatch`, hạ **5 Nấm Linh Lv 2** đúng MobIdentity ở mọi bãi hợp lệ khi step active, đóng góp ≥20%/life → quyền nhận Áo Thanh Mộc + Nấm Sương tutorial một lần tại qualifying kill **#5 (authoring BASELINE)** → tự nhặt → mặc áo → bán sample cho Bách Luyện → báo Bách Luyện. | Áo I + sample cấp theo bước; turn-in catch-up Lv 4 | Q5 sau Q4 + Lv 4 |
| Q5 — Sinh tồn ngoài làng / 4 | Yên Thảo: “Ăn trước khi đi. Thuốc để dành lúc cần.” Yên Thảo → Yên Thảo. | Ứng 320 Vàng một lần → chuẩn bị Food I + HP Potion I (mua nếu chưa có; món hợp lệ đang có cũng tính) → dùng Food → `DS4_ExitTrail`, hạ **8 Sói Sương Lv 4** đúng MobIdentity ở mọi bãi hợp lệ (DS3–DS6 là tuyến gợi ý) → báo Yên Thảo. Bình Máu được giới thiệu, chỉ dùng khi thiếu HP, không objective tiêu lúc đầy; Bình MP dạy ở Q6. | Turn-in catch-up Lv 5 | Q6 sau Q5 + Lv 5 |
| Q6 — Lễ Nhập Lộ / 5 | Lâm Bá: “Qua Học Viện gặp hai người hướng dẫn. Kiếm hay cung, con tự chọn.” Lâm Bá → mentor đã chọn (Phong Du hoặc Diệp Lam). | `HV_ClassHall`: nói chuyện **cả Phong Du và Diệp Lam** để hiểu hai phái, thứ tự tùy player → tháo Mộc Kiếm vào túi → chọn Kiếm tại Phong Du hoặc Cung tại Diệp Lam (điểm đã được trả lại một lần khi lên Lv 5) → nhận weapon I + bí kíp nhập môn; mặc weapon + xác nhận đã cộng ≥1 điểm + học sách → cast S1 ở `HV_DummyYard` → nhận MP Potion I dự trữ, dùng khi thiếu MP → báo đúng mentor đã chọn tại Học Viện. Không quay qua NPC trung gian. Bảng skill hiện nội tại Lv 5 mở/Lv 13 khóa. | Class, weapon + bí kíp theo bước, MP Potion I; 0 EXP | Q7 sau Q6 + Lv 7; mở Trúc Ảnh |
| Q7 — Tinh Thạch đầu tiên / 7 | Bách Luyện: “Một nhát búa không thành đồ tốt. Cứ làm cho đều tay.” Bách Luyện → Bách Luyện. | Chọn nhẫn I đang sở hữu (thiếu mới cấp) → preview → nâng +0→+1 hoặc xác nhận nhẫn đã ≥+1 → mặc/xác nhận đúng nhẫn → báo Bách Luyện. | Nhẫn I nếu thiếu; 1 đá + 100 Vàng dự trữ chỉ khi cần nâng; turn-in 83 EXP | Q8 sau Q7 + Lv 8; hết Chương I |
| Q8 — Bóng sói trong Trúc Ảnh / 8 | Lâm Bá giải thích trọc khí và Sói Trúc Ảnh đôi khi hóa Linh Biến, có aura khác thường; người chơi tự đi tìm. Dấu đục của người trên trấn ấn nối sang điều tra Q10, không reveal nguồn nhiễu cuối. Lâm Bá → Lâm Bá. | **Collect Dấu Trọc Khí RNG** từ Sói Trúc Ảnh đúng MobIdentity khi group active → nhặt đủ **2 (BASELINE/TUNABLE)** vào bag → stage riêng **hạ 1 Sói Trúc Ảnh + variant Linh Biến**, ≥20% → báo Lâm Bá. Không quota kill normal, không ping tọa độ. BrokenSeal giữ lore môi trường, không hai checkbox tương tác. | 108 EXP + bí kíp tiến cảnh đúng class (dùng từ Lv 10) + 2 đá | Mở Bạch Vân; Xích Nham thêm Lv 12; Q9 tùy chọn Lv 12 / Q10 Lv 15 cần Q8 |
| Q9 — Khảo Chiến Đồng Môn / 12 | Hạo Vũ: “Có đồng môn thì thử vài đường. Chưa gặp ai, cứ đi tiếp.” Hạo Vũ → Hạo Vũ. | Nói chuyện Hạo Vũ → mời/chấp nhận cược 1v1 → hoàn thành một trận đấu thật có kết quả thắng/thua/hòa → báo Hạo Vũ. FORFEIT do ngắt kết nối và SYSTEM_ABORT không tính mục tiêu. | 270 EXP + 200 Vàng sau PvP thật | Optional; không chặn Q10, bỏ qua không thưởng |
| Q10 — Dấu chân Xích Nham / 15 | Lâm Bá: “Dấu đục trên đá không do thú rừng. Mang chứng cứ về, ta sẽ cùng xem.” Lâm Bá → Lâm Bá. | **Collect Vật Chứng RNG** từ Đoạt Mạch Đạo Tặc đúng MobIdentity ở mọi bãi hợp lệ, gồm Bạch Vân/Xích Nham → nhặt đủ **3 (BASELINE/TUNABLE)** → báo Lâm Bá. Không quota kill normal. ChiselMarks/SealScar giữ environmental clues; không buộc click cả hai. | 480 EXP + 3 đá | Q11 sau Q10 theo direction; level gate mới xem [Q11](#q11-restoration) |
| Q11 — Mở Lối Huyền Môn / gate OPEN | Lâm Bá dẫn truyện → Bách Luyện phục hồi ấn; NPC/điểm kết thúc OPEN. | **Collect RNG một quest material từ Xích Thạch Linh** → Lâm Bá → Bách Luyện → dialogue/level gate trước phục hồi → staged grant **3 restored fragments riêng** → **3 placements có ý nghĩa trên nhiều map** → Huyền Môn/Huyền Tích. Không quota kill, không mảnh rơi từ mob. [Flow/OPEN chi tiết](#q11-restoration). | Giữ 670 EXP + Rare III weapon + bí kíp đại chiêu đúng class; chưa retune | Q11 Completed vẫn là điều kiện access Huyền Tích; endpoint/gate interaction OPEN, Q12 thêm Lv 20; Chương II theo GDD |
| Q12 — Tiếng gọi từ Huyền Tích / 20 | Lâm Bá: “Giúp Thủ Vệ buông gánh cũ. Trở về rồi kể ta nghe.” Lâm Bá → Lâm Bá. | Theo gợi ý `HT4_GuardRoute`, hạ **10 Cổ Môn Vệ Binh Lv 20** đúng MobIdentity ở mọi bãi hợp lệ → tới `HT_BossLandmark` → đóng góp ≥10% HP trong **một life Boss** ở death khi đúng step → báo Lâm Bá. | 1.000 Vàng một lần; 0 EXP, không thêm Boss pile | Main Story/Chương III complete; Dư Ảnh và farm tiếp tục |

**Đồ tutorial chỉ cấp ở bước đang làm:** kill/grant chỉ tạo đồ hướng dẫn/ràng buộc nhiệm vụ và quyền nhận khi đúng QuestId, InProgress, bước đang active và eligibility theo loại objective dưới đây. Ngoài bước đó, quái chỉ roll đồ thường; không sinh áo/sample tutorial hoặc quyền nhận cho nhiệm vụ tương lai. Quyền đã tạo hợp lệ giữ cùng instance để thử nhận lại khi đồ trên đất hết hạn hoặc túi đầy; không dùng recovery này để hồi tố kill cũ.

**Recovery chung:** talk/visit/kill chỉ credit sau Game Server event hợp lệ; lưu step/counter sau commit, death/reconnect không xóa. Ngã ở Q2 thì thử lại, không giả credit chỉ vì bấm phím. Mộc Kiếm Q3 không bán/vứt, grant pending nếu túi đầy. Q5 không cấp lại 320 Vàng khi replay; dùng bình lúc đầy HP bị từ chối mà không tiêu thuốc. Q4 áo giữ binding tới lúc mặc, sample chỉ bán đúng step; hết hạn ground/full bag/reconnect giữ entitlement chưa claim với cùng itemInstanceId. Quest reward chỉ trao tại đúng NPC sau capacity preflight xét net Inventory sau consume yêu cầu và merge rewards.

| Quest cần xử lý riêng | Contract không lặp lại trong bảng objectives |
| --- | --- |
| Q6 | Reset Lv 5 đúng một lần kể cả chọn class muộn; giữ điểm lên cấp sau đó. Equip/learn/xác nhận điểm cùng active group; đã cộng ≥1 điểm chấp nhận, không ép cộng mới khi pool 0. Manual grant/learn và thuốc dự trữ có receipt; hướng dẫn QuickMP ngay sau accepted S1 khi có thiếu MP. Nếu Food đã hồi đầy thì giữ thuốc/Used requirement, reject không consume; không giao việc spam S1 để rút MP. Phương án dời tutorial thật ở review bên dưới chưa thay requirement hiện hành. |
| Q7 | Selected ring instance được giữ binding không bán/vứt tới khi xác nhận đã mặc; đang mặc +1 không phải tháo/mặc lại. Chỉ cấp đá/Vàng nếu thật cần +0→+1, không duplicate resource sau reconnect. |
| Q8 | Natural shared Linh, không quest spawn/reservation/retry budget. Có Dấu trong bag mới chuyển sang Linh group. Một death dùng snapshot group trước transition, không đồng thời credit Linh group vừa mở. Đủ quyền nhưng chưa nhặt vẫn thiếu possession. |
| Q9 | Không có đối thủ thì giữ optional và đi Q10; không bot/skip reward. Disconnect/abort không tự hoàn thành, kết quả theo PVP-01. |
| Q10 | Một RNG outcome/eligible death/recipient/active objective/epoch; cả success/failure dedup. SameMap là player và mob cùng map tại death, không whitelist quest map/group. Full bag/TTL phục hồi quyền đã tạo, không roll lại. |
| Q11 | Material mob RNG; ba restored fragments là deterministic staged/NPC grant sau gate, mỗi mảnh binding riêng. Mỗi placement consume đúng mảnh + commit restoration flag cùng receipt. Gate đọc ba flags đã commit, không đòi ba mảnh vẫn nằm trong bag. Consume material ở Lâm hay Bách và completion endpoint OPEN. |
| Q12 | Boss sống thì đánh, dead thì chờ shared deadline 15 phút; nhận quest không spawn Boss. Damage trước active step/reset không hồi tố; corpse còn trong BossCombatArea có thể credit, về làng/disconnect trước death thì không. Boss death credit và quest turn-in tách receipt; một shared pile. |

<a id="quest-collection"></a>

**Collection quest items — USER-APPROVED direction:** Q8 Dấu Trọc Khí, Q10 Vật Chứng và Q11 material mob là item thật: server eligibility → RNG success → entitlement cá nhân → owner-only ground → tự nhặt → Inventory chiếm ô → quest kiểm → consume đúng action. Q11 restored fragments theo staged grant, không mob RNG. Virtual collection counters SUPERSEDED; Kill/Talk/Equip/Use/Enhance/PvP/Boss vẫn là action progress.

**Single-axis:** cùng MobIdentity/source trong một stage chỉ một progress axis chính: Kill hoặc Collect RNG. Q8 Collect → Kill Linh và Q12 Kill Guards → Boss là stage khác thật sự. Q4 Kill5 → staged áo/sample #5 là tutorial supply, không generic Collect RNG; giữ đúng hai món tutorial, ngoại lệ của cap quest-RNG một món/death.

Chỉ `QuestId + InProgress + active objectiveGroup + typed eligibility + quantity còn thiếu` mới cho roll. Một eligible death/recipient/objective/quest epoch chỉ một outcome success/failure, không ordinal guaranteed. Không pity; thêm pity là **OPEN nếu có đề xuất/approval sau**, không bật trong current direction. Exact N/drop rate Q8/Q10/Q11 TUNABLE/OPEN như bảng, không tự chọn rate. RNG không dùng regular level penalty/TopDamage/last-hit làm gate.

**ENGINEERING RECOMMENDATION — bounded rights:** thiếu = required quantity − committed bag quantity − outstanding unconsumed rights cho cùng binding; claimed quantity đã vào bag không trừ lần hai. Serialize concurrent deaths theo character revision; reserve delta trước publish để full bag/kill spam không sinh quá N quyền. Pending không tính possession. Persist cả failure và success cùng eligibility snapshot; retry/reconnect/recovery không roll lại. Death snapshot group trước transition, không credit stage vừa activate. Khi đủ quyền dừng roll nhưng còn phải nhặt; consume/claim/history không được làm quyền cũ xuất hiện lại.

Q8/Q10 consume tại turn-in; Q11 material consumption boundary OPEN, restored fragment consume tại placement. Ba fragments có identities/bindings riêng, không merge thành “3 mảnh bất kỳ”; không nhầm trade material Mảnh Cổ Ấn. Committed placed flags là action history, không collection possession counter.

Normal/Linh credit giữ **≥20% RuntimeMobMaxHP ActualHpLost khi objective active**, connected/alive/sameMap/AssistRadius 8 u/participation≤10s tại death. Boss Q12 giữ 10% và corpse exception. Level penalty không chặn quest credit/supply; regular EXP/Gold/loot độc lập. N players có entitlement/counter riêng; cùng full-health life tối đa 5 người đạt 20%, không hứa tất cả N cùng được item. Không hứa availability/credit cho tất cả N trên cùng life; review natural hunt ở dưới không đổi threshold. Talk/Equip/Use/Enhance/Region/PvP không share tự động.

**Collection authority — recommendation BASELINE:** derive required possession từ Inventory committed với character/QuestId/objective binding, chỉ bag (không equipped/Storage/entitlement). Không lưu thêm collectionCounter làm authority thứ hai. Lưu source RNG outcome/action receipts riêng để chống farm trước và grant lại; Q4 tutorial giữ qualifying ordinal #5; `ItemPickedUp` chỉ trigger re-evaluation. Alternative explicit collectedProgress + itemRequirement giữ lịch sử tốt nhưng tạo hai state phải reconcile; chưa có nhu cầu Huyền Lộ để chịu chi phí đó. Ready là cached quest state; mỗi turn-in/interact quan trọng revalidate current items/action receipts, không tin cache. Q11 fragment đã consume kiểm placed flags, không đòi possession hoặc consume lần hai. Q4 sale sample dùng `ItemSold` receipt đã commit, không đòi vẫn có mẫu sau bán. Book Learn dùng consume+learn receipt, không collection turn-in.

**Failure chung:** 60 slots không reserve quest. Full bag báo thiếu ô, không claim/credit giả; dọn túi rồi nhặt. Ground hữu hạn; quyền nhận chưa claim bền vững và bounded theo required quantity/source outcome. [Recovery owner](../02-technical/online-and-persistence.md#personal-quest-recovery) giữ reconnect/death/map/restart/TTL/replay. Quest-bound không Sell/Trade/manual Drop/death loss/wrong Quest, consume đúng quest atomic. Q4 sample là normal quest-referenced có temporary restriction tới sale đúng bước, không collection quest-bound. ItemDefinition IDs mới/mapping exact là implementation proposal cần revision migration, không đổi Q1–Q12 IDs.

Turn-in preflight **net inventory** sau consume required collection + merge reward; full bag có thể vẫn turn-in nếu consumption tạo đủ chỗ. Thiếu capacity giữ Ready và tất cả inputs/rewards/state; không partial consume/reward/cleanup/unlock. Commit remaining required consume+reward+Completed theo approved endpoint+receipt+unlock/cleanup cùng transaction; Q11 placements đã consume được kiểm bằng flags, không trừ lại fragment. Death/reconnect giữ action history đã commit; không cấp lại staged reward ở turn-in. Cơ chế abandon chưa có P0: không thêm nút; nếu sau này có, cancel epoch/entitlement/bound items cùng mutation và không hoàn grant vô hạn.

Điểm Journey theo các mục liên quan; đủ điều kiện chương/truyện không tự hoàn thành nhiệm vụ khi Boss chết.

> **Đọc sâu:** [Playtest & Balance — progression và quyết định](../04-production/playtest-and-balance.md#quest-progression)

---


| Tracker state | Người chơi thấy |
| --- | --- |
| IN_PROGRESS | “Dấu Trọc Khí: owned / N” rồi “Sói Trúc Ảnh Linh Biến: 0 / 1”; pending có cue riêng, không giả owned |
| READY_TO_TURN_IN | “Quay về gặp Lâm Bá để báo cáo” |
| LEVEL GATE | “Tu luyện đến Lv.12”; NPC giải thích vùng phù hợp |
| AVAILABLE | NPC / quest panel cho nhận, chưa có progress trước khi nhận |
| OPTIONAL Q9 | Nhãn “Tùy chọn — Tỷ thí”, tách quest được pin; không chặn chính tuyến |

**Bảy NPC hiện hành.** Tạ Minh là LEGACY đã merge/remove khỏi roster player-facing: truyện/Huyền Môn sang Lâm Bá, Tẩy Mạch sang Mộc An, nhập phái/Q6 sang hai mentor. Không tạo NPC thay thế. Học Viện có hai khu mentor và Dummy Yard dễ tìm; không đặt Kiếm như lựa chọn mặc định trước Cung. Exact tọa độ còn OPEN cho blockout. Q1 phải dẫn qua các khu chức năng thật, giữ đường ra/về làng; dialogue/tracker nêu việc → khu vực/đường đi → NPC tiếp theo.

> **Implementation:** [Technical — UI](../02-technical/gameplay-runtime.md#ui-notes)

**Q6 contextual model — STRONG DIRECTION:** trước chọn phái, server ghi hai `NpcTalked` receipts cho Phong Du và Diệp Lam trong Q6 active discovery group; thứ tự không ép. Class admission chỉ mở sau cả hai, weapon slot trống và các prerequisites hiện hành. Mentor được chọn giữ primary binding; sau Q6 cả hai vẫn Talk được. Một data table chọn thoại/action theo `(npcId, questState/group, classId, learnedSkillIds)` với priority quest response → class guidance → service → ambient là PROPOSAL tối thiểu; không conversation engine/reputation. Unselected mentor phản hồi class hiện tại, không bán/grant manual off-class. Mentor selected không bán lại guaranteed quest book hay cấp resource bằng ambient talk.

**Thoại hướng dẫn:** Q1–Q6 dùng 1–3 câu khi nhận nhiệm vụ, phản hồi của NPC trung gian và một câu khi trả; tracker nêu việc → khu vực/đường đi → NPC tiếp theo. Không thêm QuestId, số kill, EXP hay reward từ việc mở rộng hội thoại. Q4 tutorial supply vẫn chỉ đúng active step; Q6 giữ gate tự tháo Mộc Kiếm trước nhập phái.


Q3 chuyển Bách Luyện để vũ khí tutorial không khiến người chơi hiểu Phong Du là lựa chọn class mặc định. Q6 Lâm Bá giới thiệu hai mentor, mentor đã chọn nhập phái/giao supply/trả quest; giảm vòng đi qua NPC trung gian. Lâm Bá giữ mạch truyện Q8/Q10–Q12; Yên Thảo giữ thuốc/Food; Mộc An giữ utility/Tẩy Mạch/Storage/Rest, Hạo Vũ giữ PvP. Bảy NPC ở khu chức năng giúp Q1 vẫn là tìm đường thật. Dev Mode cho test nhanh nhưng không cắt Q1–Q12 hay thay fresh-run evidence.

### Worldbuilding và naming audit — giữ nội dung mạnh, không nổ art scope

| Nhóm đã đối chiếu | Kết luận / lý do |
| --- | --- |
| Game title | Giữ Huyền Lộ, gợi hành trình và bí ẩn; không thêm thuật ngữ tôn/xianxia để phô trương. |
| Map display names | Giữ Vân Khê/Đồng Sương/Trúc Ảnh/Bạch Vân/Xích Nham/Huyền Tích, Học Viện/Lôi Đài; phù hợp dân dã→hiểm trở→huyền bí. |
| NPC | Giữ tên 7 NPC hiện hành (Lâm Bá, Yên Thảo, Bách Luyện, Mộc An, Phong Du, Diệp Lam, Hạo Vũ); Tạ Minh merge/remove là thay ownership, không chỉ display rename. |
| Quest titles | Giữ 12 tên trong design owner; thoại theo khu chức năng/người trả mới, Q12 khép Chương III chứ không Game Complete. |
| Skill / passive / manual | Giữ Phong Trảm/Linh Tiễn/Kiếm Khí/Hàn Tiễn, Kiếm Tâm/Ưng Nhãn/tiến cảnh/chân quyết; không thêm skill rank hay hệ tu luyện mới. |
| Gear families | Thanh Mộc/Vân Nham/Huyền Ấn giữ đường vật liệu và ba bậc; HP/MP retune không rename ItemId. |
| Materials/resources | Giữ Nấm Sương/Trúc Tâm/Vân Thạch/Khoáng Xích Nham/Mảnh Cổ Ấn/Tinh Thạch và Vàng; không crafting/gacha/currency mới. |
| Mob / Boss | Giữ Nấm Linh/Sói Sương/Sói Trúc Ảnh/Ong Giáp/Đoạt Mạch Đạo Tặc/Xích Thạch Linh/Cổ Môn Vệ Binh/Huyền Nham Cự Thú. Không đổi thành tên tầm thường chỉ để “Việt hơn”. |
| Status / landmarks | Giữ Linh Biến/Bỏng/Băng Hàn/Đóng Băng/Làm Chậm, trấn ấn/Linh Mạch/Huyền Môn/Dư Ảnh; dùng lại cơ chế đang có. |
| Dialogue / motifs | Lời mộc mạc, 1–3 câu, mỗi NPC giọng vừa đủ; núi rừng, gỗ/ngói/tre/cầu/dược/đèo/bia đá thay generic xianxia, không nhồi “ngươi/bổn tọa/linh căn”. |

Bối cảnh Việt Nam huyền sử tiền hiện đại không khóa triều đại hay tái dựng trang phục/công trình lịch sử. Bí ẩn đi từ trấn ấn nứt → mạch đất bất thường → Linh Biến → dấu người can thiệp → Huyền Môn/phế tích; không thêm danh phận cứu thế hoặc mechanic mới. Nếu đổi tên hiển thị sau này, giữ ID nội bộ ổn định; Technical/Art/thoại nhiệm vụ cùng theo design owner.

---

Giữ **Q1–Q12**, Q9 tùy chọn; Q3/Q4/Q5/Q12 giữ counts đã duyệt; Q8/Q10 normal quota và Q11 flow cũ đã SUPERSEDED trong lượt này. Không giữ count/source proposals cũ như luật. Q11 reward Rare III/đại chiêu chỉ sau completion, không tính chúng để chứng minh độ khó các bước Q11.

<a id="mob-identity-credit"></a>

## Kill credit theo MobIdentity — direction đã duyệt

Standard KillObjective dùng stable `MobIdentityRef`, không so display name, prefab/rig, level gần nhau hoặc SpawnGroup. Sói Sương và Sói Trúc Ảnh share rig nhưng **khác identity**; Thạch Linh và Cổ Vệ không thay nhau. Normal/Linh cùng base identity được tính cho standard identity objective nếu đủ predicates; variant requirement riêng chỉ khi objective ghi rõ. SameMap nghĩa **player và mob cùng MapId tại death**, không phải quest chỉ được làm ở map gợi ý. Q5 Sói Sương ở Trúc Ảnh, Q10 Đoạt Mạch ở Bạch Vân, Q11 material từ Thạch ở các bãi hợp lệ và Q12 Guards ở mọi hành lang đều tính; map gates vẫn kiểm quyền vào.

Giữ threshold/active-step ledger/connected/alive/AssistRadius/recency/life theo collection contract; không lấy regular EXP/loot eligibility để chặn quest muộn. Kills trước accept hoặc trước active step không hồi tố. Stable deathId dedup mỗi recipient/objective; cùng SpawnSlot hồi life mới thì qualifying kill mới hợp lệ. Không đếm unique slots, không đòi concurrent slots≥requiredCount. Revisit là farm tự nhiên; tutorial không bắt chờ respawn như một objective.

**Special predicates:** Talk/Visit/Interact đúng NPC/landmark/range/MapId; Q8 Linh đúng Sói Trúc Ảnh + variant trong stage riêng; Q11 placement đúng restored fragment + authored restoration target; Q12 đúng Boss/life/area/10%. Availability, eligibility, entitlement không chung source whitelist. Standard MobIdentity cũng dùng cho mob Collect RNG, không xét display/rig hoặc bãi gợi ý.

**Tutorial supply:** Q4 qualifying #5 của mỗi recipient giữ một áo + một sample; immutable source/death payload và recovery tại sourceMap thực. Full bag không fake Pickup; receipt dedup, không cấp ở cả năm deaths. Ordinal này là tutorial exception, không authority generic cho Q8/Q10/Q11.

<a id="q8-credit-review"></a>

## Q8 natural hunt — threshold giữ, pacing OPEN

≥20% normal/Linh và ≥10% Boss giữ nguyên. Natural shared Linh không có bounded completion guarantee: cap chung có thể đang bị Ong Linh giữ, chưa có Sói Linh; nhiều người cùng săn có thể không đủ 20%. Map Info chỉ cho biết tổng Linh trong map, không chứng minh đúng species hoặc eligibility. **OPEN pacing/availability:** đo thời gian chờ đúng identity, số lần gặp mà thiếu credit, ảnh hưởng người ngoài/gear và tail latency; recommendation nhỏ nhất là đo/giải thích rare hunt rõ trước tune rate/density, không giảm threshold, private spawn, queue hay pity. BrokenSeal/ChiselMarks/SealScar giữ scenery; thêm tối đa một investigation có state/narrative purpose ở Q8 hoặc Q10 chỉ là PROPOSAL, chưa thêm objective.

<a id="q11-restoration"></a>

## Q11 phục hồi ấn — CURRENT DIRECTION, level gate NEEDS USER LOCK

Flow: nhận sau Q10 → farm Xích Thạch Linh lấy **một loại quest material RNG** → mang về Lâm Bá (ông cho biết mảnh ấn cũ đã giao Bách Luyện phục hồi) → tới Bách Luyện → thoại và kiểm gate trước phục hồi/cấp mảnh → nhận ba restored fragments deterministic → đặt đúng ba trấn ấn trên nhiều map → Huyền Môn/Huyền Tích. Không thêm QuestId. Bắt đầu Q11 ngay sau Q10 và chuyển gate sang Bách là **STRONG DIRECTION / NEEDS USER LOCK**; Lv17 là reference cũ, chưa chọn level mới hoặc giả graph production đã khóa.

| Trường | Status / nội dung còn phải quyết |
| --- | --- |
| Material | OPEN tên/ItemDefinitionId, quantity N, drop rate; chỉ một loại từ Xích Thạch Linh, khác farm material |
| Material consume | OPEN tại Lâm hoặc Bách; recommendation Bách consume cùng restoration grant để không trừ hai lần; trade-off bag giữ lâu hơn |
| Level gate | OPEN exact level/điểm kiểm; direction trước cấp ba mảnh tại Bách, không ẩn gate khi accept |
| Restored grant | Deterministic staged/NPC entitlement; ba identities riêng, receipt một lần; full bag giữ bounded quyền chưa nhận, không partial progress giả |
| Placements | OPEN ba map/positions, ordered hay free; phải là nhiều map và **đều reachable trước Q11 Completed**; không cả ba bắt buộc Xích Nham hoặc Huyền Tích sau chính gate đó |
| Placement mutation | Consume fragment tương ứng + durable restoration flag atomic/idempotent; repeat trả receipt, không consume lần hai; kiểm quest epoch/binding/target/range |
| Endpoint | OPEN hoàn ở last seal, Huyền Môn hay báo Lâm; gate tự phản ứng hay cần PrimaryAction cuối cũng OPEN, không mặc định thêm interaction thứ tư |
| Gate / reward | Gate đọc ba flags committed + completion/access policy, không đọc possession ba mảnh đã consume. Access Q11Completed hiện hành giữ; NPC/commit action gắn Completed phải được duyệt cùng endpoint. Reward670/RareIII/S3 giữ, S3 dùng Lv17 không đổi |

Personal restoration/access không globally mở gate cho character khác. Exact shared visual cho trấn ấn khi mỗi character có state khác nhau **OPEN presentation**, recommendation overlay cá nhân reuse prop, trade-off cần view theo quest. Chương II reference Q11Completed+Lv17 tại GDD giữ riêng, không dùng nó khóa ngầm gate Bách; Q11 có thể kết thúc dưới17 nếu gate được chọn thấp hơn, lúc đó RareIII/S3 requirement vẫn giữ. QUEST-03 cần đo lại Lv15–20/farm gaps, chưa retune EXP/quest reward. Production Q11 content/fixtures phải ghi các placeholder chưa duyệt.

<a id="q2-journey"></a>

## Q2 mini journey — PROPOSAL nội dung, direction đã xác nhận

**Recommendation tốt nhất — PROPOSAL:** Lâm Bá nhờ lấy **Vải Bọc Chuôi** ở giá vật dụng Học Viện rồi mang về, để Bách Luyện chuẩn bị Mộc Kiếm tập võ cho Q3. Một việc nhỏ có lý do: người dự tuyển biết lối tới sân và mang vật liệu chuẩn bị về trước khi nhận vũ khí. Không crafting recipe hoặc gear component system; vật phẩm chỉ quest delivery. “Thẻ Nhập Sân” là draft cũ chưa khóa, được thay bằng recommendation này; **tên/display/ItemDefinitionId/placed-objectId exact vẫn cần duyệt**. Giữ Q2/title/catch-up Lv3/75 Vàng/prerequisite Q3/NPC Lâm Bá; không thêm NPC hoặc class gate.

Route draft: Vân Khê → EdgeExit tây → HV_Entrance → tuyến bậc solid nhìn được tới thềm/giá gần HV_JumpLedge → Interact nguồn placed → ground cá nhân ở mặt đứng cạnh giá → tự Pickup → đường vòng thấp qua HV_DropLanding/exit → trả Lâm Bá và consume vải cùng reward. Thấy sân tập/hai khu mentor trên tuyến để nối Q3/Q6. Khởi điểm 2–4 bước nhảy rộng chỉ là geometry PROPOSAL; có optional one-way return và đường thường, không checklist Jump/Drop hoặc precision landing. Fall chỉ thử lại, không nhận credit từ input.

**Definition-ready draft gated tại G-B:** ordered group `Visit/Interact(placed source)` → `ItemRequirement(character,Q2,delivery,quantity1)` → `TurnIn(Lâm Bá,consume delivery1)`. Marker dùng anchors stable hiện có; nguồn placed mới/mapping item là placeholder proposal, không giả đã tồn tại scene. Validator alive/sameMap/range/active group tạo tối đa một right/quest epoch; repeat re-offer Pending, không direct bag. Full bag/TTL/reconnect giữ right cùng payload; possession từ Inventory committed và net turn-in atomic. Blockout fixed Jump/camera/ground pickup/return phải thử trước production authoring; phê duyệt nội dung không tự chứng minh geometry pass.

### Q6 MP tutorial — recovery hiện hành và proposal dời thời điểm

Giữ Talk cả hai mentor, chọn đúng một phái, manual/weapon/point confirmation/cast S1, reserved MP Potion và Used receipt hiện hành; unselected mentor vẫn Talk. UI đưa glyph QuickMP ngay khi accepted S1 tạo thiếu MP, không nhắc cast vô nghĩa để drain pool. Full MP luôn reject không consume/Used; Food không bị pause chỉ để tutorial pass.

**Recommendation PROPOSAL cần duyệt trước đổi objective:** Q6 dạy chuẩn bị thuốc và cast; chuyển phần *bắt buộc consume* thành contextual tutorial trong lần thiếu MP tự nhiên đầu tiên sau nhập phái (farm thật Lv5–7), lưu tutorial receipt thuộc Q6 nhưng không chặn turn-in/Q6 map unlock. Không QuestId mới, không grant lần hai, không fake Used khi đầy. Lợi: tránh spam và chuyến quay mentor chỉ để uống bình; chi phí: một supplemental tutorial flag/receipt sau quest completion, phải tách khỏi active kill groups và giới hạn reserved supply. Alternative giữ Used blocking Q6 rẻ hơn nhưng Food tick có thể gây do dự/chờ. Chưa áp proposal như LOCKED; G-L phải ghi edge case này, không dùng fixture MP thấp để gọi fresh Q6 pass.

<a id="quest-gameplay-review"></a>

## Quest readiness và pacing review

[Q1–Q12 objectives/rewards](#q1q12-và-nhiệm-vụ), [collection/eligibility](#quest-collection), [Q11 direction/OPEN](#q11-restoration) và [regression Q-F01..12](../04-production/playtest-and-balance.md#quest-regression--q-f0112-toàn-bộ-chưa-chạy) là các owner sections; không duy trì thêm bảng catalog completeness. Recovery/idempotency tại [Online](../02-technical/online-and-persistence.md#personal-quest-recovery).

**Pacing review chưa chạy:** candidate World totals không cần bằng requiredCount. Đo clear/travel/revisit/loot/UI/25s respawn cho solo và2/3/4/>5 người, cả Kiếm/Cung; no-Food/gear-lag/novice pressure riêng. Nhiều placements giảm contention nhưng không bảo đảm N người đạt20% cùng life. RNG Collect thay số kills kỳ vọng/tail; loại quota không loại EXP/Gold thường từ kills. Gate Q11 mới thay farm gap và reward timing; QUEST-03 phải đo level trước/turn-in/farm gap, không tự retune NeedEXP hoặc rewards. Tutorial có route đủ targets để không bắt chờ; online wait/steal/fairness vẫn gate thực.

<a id="npc-roster"></a>
<a id="npc-service-review"></a>

## NPC service/post-quest review

| NPC | Narrative role | Services | Catalog ownership | Sell capability | Quest role | Post-quest interaction | Why this NPC owns this service |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Lâm Bá | Người dẫn truyện làng/mạch đất | Talk/quest/route guidance | Không shop mới | Không shop Sell | Q1/Q2; giới thiệu Q6; Q8/Q10–12 | Theo chương/quest, nhắc đường phù hợp | Giữ đầu mối truyện, không thêm vendor |
| Yên Thảo | Dược/thảo mộc, giữ sức | Food/HP/MP Potion, phục hồi thông thường | Food I–III/HP I–III/MP I–III theo Items | Sell tab chỉ catalog được phép; General Sell/Q4 ở Bách | Q5/Q1 visit | Chuẩn bị Food/thuốc và lời khuyên | Chuyên thuốc; Hồi Sinh/Tẩy Mạch đã chuyển sang Mộc theo chỉ đạo mới |
| Bách Luyện | Thợ rèn/trang bị | Buy/Sell, Enhance, Transfer | Common I/II, class-gated weapon, Tinh Thạch; không Crafting mới | Bag sellable theo policy; quest-bound disabled; Q4 sample đúng step | Q3/Q4/Q7/Q1 visit; Q11 restoration direction | Gear/service/preview, không grant lại supplies | Đồ/đá/chuyển cấp cùng nghề và recovery sample |
| Mộc An — nhà kho/nghỉ | Kho/nghỉ, người giữ đồ và bùa tiện ích | Storage40/Rest full/Utility Shop/Tẩy Mạch | Hồi Sinh Phù1000/Tẩy Mạch Phù1200, giữ effect/stock theo Items | Sell tab cho utility theo policy; General Sell vẫn Bách | Q1 visit | Store/Take/Rest/Buy utility/reset advice | Cùng điểm phục hồi/giữ đồ; không Crafting/seasonal hoặc utility mới |
| Phong Du — khu Kiếm Học Viện | Mentor Kiếm | Guidance/class admission/quest manual đúng class | Guaranteed manual theo quest; không off-class skill shop | Không general Sell | Q6 talk cả hai mentor, chọn Kiếm/turn-in | Kiếm: next skill; Cung: nhận biết phái/ambient | Chuyên môn Kiếm, không bias Q3 hoặc đổi phái |
| Diệp Lam — khu Cung Học Viện | Mentor Cung | Guidance/class admission/quest manual đúng class | Guaranteed manual theo quest; không off-class skill shop | Không general Sell | Q6 talk cả hai mentor, chọn Cung/turn-in | Cung: next skill; Kiếm: nhận biết phái/ambient | Chuyên môn Cung, ngang hàng trước chọn |
| Hạo Vũ | Thượng võ/tỷ thí | PvP Challenge/result | Không item shop mới | Không | Q9 optional | Invite/context Arena vẫn dùng | Một đầu mối PvP, không NPC combat mới |

**Naming audit:** giữ Bù Nhìn và cùng training identity; lượt này không rename Mộc Nhân hoặc thêm variant. Học Viện→Võ Viện hợp võ đường, nhưng lợi ích nhỏ so chi phí thoại/signage/acceptance; giữ Học Viện. Map/NPC/mob/gear/Quest names hiện hành đủ nhận diện Việt-inspired+cổ phong; không thêm Thiên/Đế máy móc. Nếu approve display rename, giữ MapId/QuestId/itemId/group/slot/marker/file/folder, chỉ migrate localization/reference text có kiểm. Vải Bọc Chuôi là recommendation draft Q2, không catalog item đã duyệt; Thẻ Nhập Sân chưa từng khóa.

**NPC menu ownership:** multi-service NPC root hiện Talk/Quest/Shop/Storage/Rest/Enhance/Transfer phù hợp definitions; Shop chỉ Buy/Sell. Một visual Shop/widget kit dùng lại với catalog/controller theo NPC; Storage/Rest/reset/enhance không biến thành Shop tabs. Items sở hữu catalog/sell policy, Runtime sở hữu commands/UI.

Mở NPC context bằng PrimaryAction chỉ mở menu, không tự NpcTalked/grant/turn-in; Talk là option. Input/menu owner: [Runtime](../02-technical/gameplay-runtime.md#active-focus).
