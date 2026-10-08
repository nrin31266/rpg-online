# Huyền Lộ — Quests & Narrative

Các số BASELINE/TUNABLE chưa phải nghiệm thu runtime.

Bảng Q1–Q12 xác định objectives, reward, marker và người nhận/trả. Q8 dùng [reservation không softlock](world-and-content.md#q8-bounded-path). Proposal chưa duyệt không thay luật trong bảng.

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

**State:** LOCKED → AVAILABLE khi đủ prerequisite + level → nhận → IN_PROGRESS → đủ active objectives → READY_TO_TURN_IN → đúng NPC/range → COMPLETED + reward/unlock. Ready không auto-turn-in/chuyển map; thiếu cấp thì NPC/HUD chỉ rõ mốc và bãi phù hợp. Lối cũ vẫn quay lại được.

**Objective theo hành động:** quest data lưu Jump/DropThrough/UseFood/UseHpPotion/UseMpPotion/Interact/Pickup, không lưu phím. Tutorial/HUD lấy glyph từ binding hiện hành ở các mục liên quan; key press đơn thuần không credit action thất bại.

**Marker/vùng nhiệm vụ:** mỗi ID dưới là khóa của trigger hoặc điểm tương tác trên map/NPC hiện có; Game Server kiểm MapId/vị trí. Visit radius 1,5 u TEST, Interact trong 2 u TEST (binding tại các mục liên quan). Waypoint Huyền Môn đang khóa vẫn interact được từ phía ngoài Xích Nham ở Q11. Dấu `+` trong objectives chỉ các mục tiêu cùng active group, không phải thêm QuestId.

| Quest / Lv | Story + người giao → trả | Objectives theo thứ tự | Reward | Unlock / next |
| --- | --- | --- | --- | --- |
| Q1 — Người mới đến Vân Khê / 1 | Lâm Bá: “Nhớ chỗ thuốc, lò rèn và đường về. Ra núi rồi, chẳng ai giữ hộ mạng mình.” Lâm Bá → Lâm Bá. | Đi tới khu dược nói chuyện Yên Thảo → lò rèn gặp Bách Luyện → nhà kho/nghỉ gặp Mộc An → nhìn lối đi ra/về làng rồi báo Lâm Bá. Các NPC ở khu chức năng riêng, không xếp cạnh nhau thành menu. | Catch-up Lv 2 + 50 Vàng | Q2 sau Q1 + Lv 2 |
| Q2 — Bước chân đầu tiên / 2 | Lâm Bá: “Đi thử một vòng. Chân vững rồi hãy cầm kiếm.” Lâm Bá → Lâm Bá. | `HV_Entrance` → nhảy tới `HV_JumpLedge` → đi xuống xuyên sàn tới `HV_DropLanding` → đi qua MapExit về Vân Khê → báo Lâm Bá. | Catch-up Lv 3 + 75 Vàng | Q3 sau Q2 + Lv 3 |
| Q3 — Vũ khí trong tay / 3 | Bách Luyện: “Cầm thử cây kiếm gỗ này. Ra sân tập cho quen tay, rồi trở lại đây.” Bách Luyện → Bách Luyện. | Nhận/mặc Mộc Kiếm → `HV_DummyYard` → hạ 3 Bù Nhìn, mỗi life đóng góp ≥20% → báo Bách Luyện. Yard có ít nhất 3 Dummy cùng lúc; HP 60, không đánh/trả thưởng; respawn 25 s TEST. | Mộc Kiếm cấp trước một lần; turn-in Quần Thanh Mộc I, 0 EXP | Q4 sau Q3 + Lv 3 |
| Q4 — Chiến lợi phẩm đầu tiên / 3 | Bách Luyện: “Thứ mặc được thì giữ. Thứ thừa đem bán, lấy đồng lộ phí.” Bách Luyện → Bách Luyện. | `DS2_MushroomPatch`: hạ 1 Nấm Lv 2 khi đúng step, đóng góp ≥20% → nhặt Áo Thanh Mộc + Nấm Sương tutorial → mặc áo → bán sample cho Bách Luyện → báo Bách Luyện. | Áo I + sample cấp theo bước; turn-in catch-up Lv 4 | Q5 sau Q4 + Lv 4 |
| Q5 — Sinh tồn ngoài làng / 4 | Yên Thảo: “Ăn trước khi đi. Thuốc để dành lúc cần.” Yên Thảo → Yên Thảo. | Ứng 320 Vàng một lần → chuẩn bị Food I + HP Potion I (mua nếu chưa có; món hợp lệ đang có cũng tính) → dùng Food → `DS4_ExitTrail`, hạ **5 Sói Sương Lv 4 trong DS3–DS6** → báo Yên Thảo. Bình Máu được giới thiệu, chỉ dùng khi thiếu HP, không objective tiêu lúc đầy; Bình MP dạy ở Q6. | Turn-in catch-up Lv 5 | Q6 sau Q5 + Lv 5 |
| Q6 — Lễ Nhập Lộ / 5 | Lâm Bá: “Qua Học Viện gặp hai người hướng dẫn. Kiếm hay cung, con tự chọn.” Lâm Bá → mentor đã chọn (Phong Du hoặc Diệp Lam). | `HV_ClassHall`: tháo Mộc Kiếm vào túi → chọn Kiếm tại Phong Du hoặc Cung tại Diệp Lam (điểm đã được trả lại một lần khi lên Lv 5) → nhận weapon I + bí kíp nhập môn; mặc weapon + xác nhận đã cộng ≥1 điểm + học sách → cast S1 ở `HV_DummyYard` → nhận MP Potion I dự trữ, dùng khi thiếu MP → báo đúng mentor đã chọn tại Học Viện. Không quay qua NPC trung gian. Bảng skill hiện nội tại Lv 5 mở/Lv 13 khóa. | Class, weapon + bí kíp theo bước, MP Potion I; 0 EXP | Q7 sau Q6 + Lv 7; mở Trúc Ảnh |
| Q7 — Tinh Thạch đầu tiên / 7 | Bách Luyện: “Một nhát búa không thành đồ tốt. Cứ làm cho đều tay.” Bách Luyện → Bách Luyện. | Chọn nhẫn I đang sở hữu (thiếu mới cấp) → preview → nâng +0→+1 hoặc xác nhận nhẫn đã ≥+1 → mặc/xác nhận đúng nhẫn → báo Bách Luyện. | Nhẫn I nếu thiếu; 1 đá + 100 Vàng dự trữ chỉ khi cần nâng; turn-in 83 EXP | Q8 sau Q7 + Lv 8; hết Chương I |
| Q8 — Bóng sói trong Trúc Ảnh / 8 | Lâm Bá: “Vết cào này lạ. Xem dưới chân cầu có gì.” Mảnh ấn mang vết đục của người. Lâm Bá → Lâm Bá. | Tương tác `TA4_BrokenSeal` → hạ **4 Sói Trúc Ảnh Lv 8 tại TA4 + TA6**, nhận 2 Dấu Trọc Khí virtual ở qualifying kill #1/#2 → tương tác dấu ấn → hạ 1 Sói Trúc Ảnh **Linh Biến tại `TA4.slot1`** với ≥20% → báo Lâm Bá. | 108 EXP + bí kíp tiến cảnh đúng class (dùng từ Lv 10) + 2 đá | Mở Bạch Vân; Xích Nham thêm Lv 12; Q9 tùy chọn Lv 12 / Q10 Lv 15 cần Q8 |
| Q9 — Khảo Chiến Đồng Môn / 12 | Hạo Vũ: “Có đồng môn thì thử vài đường. Chưa gặp ai, cứ đi tiếp.” Hạo Vũ → Hạo Vũ. | Nói chuyện Hạo Vũ → mời/chấp nhận cược 1v1 → hoàn thành một trận đấu thật có kết quả thắng/thua/hòa → báo Hạo Vũ. FORFEIT do ngắt kết nối và SYSTEM_ABORT không tính mục tiêu. | 270 EXP + 200 Vàng sau PvP thật | Optional; không chặn Q10, bỏ qua không thưởng |
| Q10 — Dấu chân Xích Nham / 15 | Lâm Bá: “Dấu đục trên đá không do thú rừng. Mang chứng cứ về, ta sẽ cùng xem.” Lâm Bá → Lâm Bá. | Tương tác `XN1_ChiselMarks` → hạ **6 Đoạt Mạch Lv 13 tại XN1–XN3**, nhận 3 Vật Chứng virtual ở qualifying kill #2/#4/#6 → Tương tác `XN2_SealScar` → báo Lâm Bá. | 480 EXP + 3 đá | Q11 sau Q10 + Lv 17 |
| Q11 — Mở Lối Huyền Môn / 17 | Lâm Bá: “Nối lại từng dấu ấn. Chỉ chạm Huyền Môn khi mạch đất đã yên.” Lâm Bá → Lâm Bá. | Tương tác `XN4_SealA` + hạ **3 Thạch Lv 16 tại XN4** → Mảnh 1; Tương tác `XN5_SealB` + **3 tại XN5** → Mảnh 2; Tương tác `XN6_SealC` + **4 tại XN6** → Mảnh 3; Tương tác `XN_HuyenMon_Outer` kiểm đủ mảnh/kích hoạt → báo Lâm Bá. | 670 EXP + Rare III weapon + bí kíp đại chiêu đúng class | Chỉ **Completed** mới mở Huyền Tích; Q12 thêm Lv 20; hết Chương II |
| Q12 — Tiếng gọi từ Huyền Tích / 20 | Lâm Bá: “Giúp Thủ Vệ buông gánh cũ. Trở về rồi kể ta nghe.” Lâm Bá → Lâm Bá. | `HT4_GuardRoute`: hạ **6 Cổ Vệ Lv 20 tại HT4 + HT5** → tới `HT_BossLandmark` → đóng góp ≥10% HP trong **một life Boss** ở death khi đúng step → báo Lâm Bá. | 1.000 Vàng một lần; 0 EXP, không thêm Boss pile | Main Story/Chương III complete; Dư Ảnh và farm tiếp tục |

**Đồ tutorial chỉ cấp ở bước đang làm:** kill/grant chỉ tạo đồ hướng dẫn/ràng buộc nhiệm vụ và quyền nhận khi đúng QuestId, InProgress, bước đang active và nguồn. Ngoài bước đó, quái chỉ roll đồ thường; không sinh áo/sample tutorial hoặc quyền nhận cho nhiệm vụ tương lai. Quyền đã tạo hợp lệ giữ cùng instance để thử nhận lại khi đồ trên đất hết hạn hoặc túi đầy; không dùng recovery này để hồi tố kill cũ.

**Recovery chung:** talk/visit/kill chỉ credit sau Game Server event hợp lệ; lưu step/counter sau commit, death/reconnect không xóa. Ngã ở Q2 thì thử lại, không giả credit chỉ vì bấm phím. Mộc Kiếm Q3 không bán/vứt, grant pending nếu túi đầy. Q5 không cấp lại 320 Vàng khi replay; dùng bình lúc đầy HP bị từ chối mà không tiêu thuốc. Q4 áo giữ binding tới lúc mặc, sample chỉ bán đúng step; hết hạn ground/full bag/reconnect giữ entitlement chưa claim với cùng itemInstanceId. Quest reward chỉ trao tại đúng NPC sau capacity preflight.

| Quest cần xử lý riêng | Contract không lặp lại trong bảng objectives |
| --- | --- |
| Q6 | Reset Lv 5 đúng một lần kể cả chọn class muộn; giữ điểm lên cấp sau đó. Equip/learn/xác nhận điểm cùng active group; đã cộng ≥1 điểm chấp nhận, không ép cộng mới khi pool 0. Manual grant/learn và thuốc dự trữ có receipt; nếu Food hồi đầy MP, cast lại để dùng Bình Linh Lực thật, không fake consume. |
| Q7 | Selected ring instance được giữ binding không bán/vứt tới khi xác nhận đã mặc; đang mặc +1 không phải tháo/mặc lại. Chỉ cấp đá/Vàng nếu thật cần +0→+1, không duplicate resource sau reconnect. |
| Q8 | Chỉ step Linh Biến giữ force request; unrelated variant không chặn reservation; giữ nguyên life đang sống, không demote/despawn. N requesters chia cùng `TA4.slot1`; quest credit theo từng recipient, budget force/retry theo World policy, không reroll Rare do retry. |
| Q9 | Không có đối thủ thì giữ optional và đi Q10; không bot/skip reward. Disconnect/abort không tự hoàn thành, kết quả theo PVP-01. |
| Q10 | Evidence #2/#4/#6 tính theo qualifying kills **của từng recipient**; 6 kills đủ 3, không RNG hay farm lại vô hạn. Sai step/source không hồi tố. |
| Q11 | Mảnh 1/2/3 cấp tại kill đủ credit cuối mỗi khu 3/3/4; portal từ ngoài chỉ activation khi đủ ba counters; không consume mảnh trước turn-in, không mở map trước Completed. |
| Q12 | Boss sống thì đánh, dead thì chờ shared deadline 15 phút; nhận quest không spawn Boss. Damage trước active step/reset không hồi tố; corpse còn trong BossCombatArea có thể credit, về làng/disconnect trước death thì không. Boss death credit và quest turn-in tách receipt; một shared pile. |

**Virtual evidence — luật gameplay:** chỉ `QuestId + IN_PROGRESS + active step/group + đúng source + count còn thiếu` mới generate / credit, sau Game Server death / interaction đã xác thực. Không physical quest pickup, không bag slot, không hiển thị cho player không có objective. Item / icon “Vật Chứng +1” chỉ feedback cho recipient hợp lệ; farm trước / sai step / đủ count không sinh.

Guaranteed ordinal dùng qualifying kills **của chính recipient**, không raw world kills; progress / evidence cùng event receipt, cap required count. Turn-in thành công mới cleanup; failed turn-in giữ counters / activation.

Normal / Linh Biến kill / evidence credit cần **≥ 20% RuntimeMobMaxHP ActualHpLost trong lúc objective đang active**, connected / alive / sameMap / trong AssistRadius 8 u và participation ≤ 10 s tại death; Boss Q12 dùng 10% active-step damage và corpse exception các mục liên quan. Level penalty không chặn quest credit / supply / evidence; EXP / Gold / loot là pipeline riêng.

N players có counter / step riêng: không tự share Talk / Equip / Use / Enhance / Region / PvP, không last-hit dependency. Cùng một target không thể credit hơn 5 recipients ở 20% trên full-health life.

Áo / thuốc / vũ khí tutorial và Nấm Sương dùng dạy bán hàng là supply sử dụng được, không phải quest evidence; giữ delivery / entitlement của vật phẩm thật. Dấu Trọc Khí / Vật Chứng / Mảnh Ấn chính tuyến chỉ là virtual counters.

**Failure chung:** step / counter / staged grant / class / activation đã commit giữ qua death, rời map / reconnect; chưa commit event được retry cùng ID, không nhân credit. Tutorial supply giữ pending / cùng instance nếu full bag / ground expiry; không cấp lại lúc turn-in. Completion reward kiểm capacity sau merge stack, thiếu thì giữ Ready và báo X ô; không thưởng một phần / cleanup / unlock trước commit. Quest-specific item bindings chỉ phục vụ tutorial rồi gỡ đúng bước; không gear-lock UI mới.

Điểm Journey theo các mục liên quan; đủ điều kiện chương/truyện không tự hoàn thành nhiệm vụ khi Boss chết.

> **Đọc sâu:** [Playtest & Balance — progression và quyết định](../04-production/playtest-and-balance.md#quest-progression)

---


| Tracker state | Người chơi thấy |
| --- | --- |
| IN_PROGRESS | “Hạ Sói Trúc Ảnh: 2 / 4” (Q8; counts tại các mục liên quan) |
| READY_TO_TURN_IN | “Quay về gặp Lâm Bá để báo cáo” |
| LEVEL GATE | “Tu luyện đến Lv.12”; NPC giải thích vùng phù hợp |
| AVAILABLE | NPC / quest panel cho nhận, chưa có progress trước khi nhận |
| OPTIONAL Q9 | Nhãn “Tùy chọn — Tỷ thí”, tách quest được pin; không chặn chính tuyến |

<a id="npc-roster"></a>

| NPC / khu chức năng | Quest / service hiện hành |
| --- | --- |
| Lâm Bá — khu công cộng Vân Khê, dễ thấy | Q1/Q2, giới thiệu Q6; Q8/Q10/Q11/Q12 và truyện/Huyền Môn |
| Yên Thảo — khu dược/thảo mộc | Q5; Food/Potion/Hồi Sinh và Tẩy Mạch/reset stat |
| Bách Luyện — lò rèn/đe | Q3/Q4/Q7; vũ khí/gear/đá/bán/cường hóa/chuyển giao |
| Mộc An — nhà kho/nghỉ | Storage và nghỉ hồi đầy |
| Phong Du — khu Kiếm Học Viện | Mentor Kiếm; giao dịch nhập phái và trả Q6 nếu đã chọn Kiếm |
| Diệp Lam — khu Cung Học Viện | Mentor Cung; giao dịch nhập phái và trả Q6 nếu đã chọn Cung |
| Hạo Vũ — gần biển/lối Lôi Đài | Q9, PvP Challenge |

**Bảy NPC hiện hành.** Tạ Minh là LEGACY đã merge/remove khỏi roster player-facing: truyện/Huyền Môn sang Lâm Bá, Tẩy Mạch sang Yên Thảo, nhập phái/Q6 sang hai mentor. Không tạo NPC thay thế. Học Viện có hai khu mentor và Dummy Yard dễ tìm; không đặt Kiếm như lựa chọn mặc định trước Cung. Exact tọa độ còn OPEN cho blockout. Q1 phải dẫn qua các khu chức năng thật, giữ đường ra/về làng; dialogue/tracker nêu việc → khu vực/đường đi → NPC tiếp theo.

> **Implementation:** [Technical — UI](../02-technical/gameplay-runtime.md#ui-notes)

**Thoại hướng dẫn:** Q1–Q6 dùng 1–3 câu khi nhận nhiệm vụ, phản hồi của NPC trung gian và một câu khi trả; tracker nêu việc → khu vực/đường đi → NPC tiếp theo. Không thêm QuestId, số kill, EXP hay reward từ việc mở rộng hội thoại. Q4 tutorial supply vẫn chỉ đúng active step; Q6 giữ gate tự tháo Mộc Kiếm trước nhập phái.


Q3 chuyển Bách Luyện để vũ khí tutorial không khiến người chơi hiểu Phong Du là lựa chọn class mặc định. Q6 Lâm Bá giới thiệu hai mentor, mentor đã chọn nhập phái/giao supply/trả quest; giảm vòng đi qua NPC trung gian. Lâm Bá giữ mạch truyện Q8/Q10–Q12; Yên Thảo nhận Tẩy Mạch; Mộc An/Hạo Vũ giữ service. Bảy NPC ở khu chức năng giúp Q1 vẫn là tìm đường thật. Dev Mode cho test nhanh nhưng không cắt Q1–Q12 hay thay fresh-run evidence.

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

Giữ **Q1–Q12**, Q9 tùy chọn; không thêm nhiệm vụ để lấp khoảng farm. Đối chiếu đúng SpawnSlot: Q4 một Nấm DS2/loot/equip/sell; Q5 năm Sói DS3–DS6; Q8 bốn Sói TA4+TA6 rồi Linh Biến `TA4.slot1`; Q10 sáu Đạo Tặc XN1–XN3, vật chứng ở kill thứ 2/4/6; Q11 3/3/4 Thạch XN4/5/6; Q12 sáu Cổ Vệ HT4+HT5. Vật chứng nhiệm vụ theo từng người, không RNG vô hạn. Q11 hoàn thành mới cho vũ khí Rare III và đại chiêu; không dùng chúng để tính độ khó Q11.

<a id="q8-credit-review"></a>

## Q8 credit: baseline giữ, phương án cần review

**CURRENT BASELINE / TUNABLE:** giữ objective Linh Biến và ngưỡng đóng góp đã có trong bảng Q8. Quest credit tách regular loot eligibility; không dùng quyền nhặt để chứng minh đã làm nhiệm vụ. Reservation/retry không bị Linh khác giữ cap; [World owner](world-and-content.md#q8-bounded-path) giữ scheduling/economic guard.

**RATIONALE:** threshold theo phần MaxHP giới hạn số recipients mỗi life. Giảm xuống một tỷ lệ nhỏ hơn vẫn có giới hạn với N lớn; fair queue không tự chứng minh chống một outsider liên tục lấy hết damage. G-C phải kiểm đông requesters, chênh gear và interference, không coi “cứ retry” là bằng chứng no-starvation.

**PROPOSAL, chưa áp dụng:** riêng forced Q8 death credit bằng server-validated active participation: gây actual damage khi đúng step/life, còn alive/connected/sameMap/inAssistRadius và có hành động combat gần death. Zero damage/AFK/đến sau không credit. Giữ thresholds reward/loot/Journey riêng; không sửa chúng để nới quest. Exact damage/recency hoặc action-count predicate phải thử để vừa chống AFK vừa cho N requesters cùng làm; không lấy 10% làm mặc định. Nếu baseline không đạt Q8-01, cần chốt proposal này trước mở content production.
