# Huyền Lộ — prototype tham khảo, chưa có production codebase

**DESIGN + PROTOTYPE VALIDATION.** VS-1 này là **disposable/reference integration prototype V6.2.0**, checkpoint `46006c4`. Nó phục vụ harvest findings, không là production architecture và không phải base để gắn network. **Production codebase chưa bắt đầu.** Folder/class/asmdef names prototype không là authority; contract hiện hành thuộc [GDD V6.2.1](../docs/design/1_HUYEN_LO_GDD.md), [Technical](../docs/design/2_HUYEN_LO_TECHNICAL.md) và [Roadmap gates](../docs/design/5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md#phase-gates).

```text
Assets/_Prototype/VS1_EndToEnd/   domain/runtime/editor/tests/scenes/resources cũ
PrototypeEvidence/VS1_EndToEnd/  build/test/route evidence V6.2.0 + audit feedback riêng
```

Đã move cùng `.meta`, giữ GUID và sửa scene/build/font paths; production folder `Assets/HuyenLo/` chưa tồn tại. Unity project thực 6000.5.9f1 vẫn mở được qua Hub tại `game/`; scene [VS1.unity](Assets/_Prototype/VS1_EndToEnd/Scenes/VS1.unity). Session trong RAM, đóng game mất tiến trình. Source NSO/research chỉ reference, không gameplay authority.

## Prototype cũ khác contract mới

| Hạng mục | Behavior có trong prototype V6.2.0 | Docs V6.2.1 cho probe/base tiếp theo |
| --- | --- | --- |
| Combat | J-selected, slot select+cast, hold repeat/approach | Không J/RepeatOnHold; 1–3 one-press bounded approach/cast, release giữ pending, manual/UI/Esc/focus/map cancel |
| Onboarding | Q3→Lv4, Q4 Sói→Lv5, Q5 Nấm/loot | Q3 Lv3, Q4 Nấm/loot/equip/sell→Lv4, Q5 Food/Bình Máu + Sói→Lv5 |
| Di chuyển map | E ở portal anchors | EdgeExit auto ở mép có arrow/tên đích; SpecialGate riêng |
| Jump/drop | Space, S+Space | Space/↑ Jump; S/↓ Drop trên one-way, có Movement Feel prototype gate |
| Panels | B/C/K/L | I Inventory, C Character + skill tab, Q Quest là default để kiểm usability |
| Art/AI/UI | Primitive/text UI, melee dễ pile, jump basic | Art/rig + movement + separation/reposition + manual usability probe trước base |

**Vòng này ưu tiên sửa docs và phân loại prototype; chưa rewrite gameplay prototype thành revision mới.** Không dùng old tests/video PASS để ký G-L mới. Muốn thử V6.2.1 phải implement probe theo docs rồi ghi evidence revision riêng. Reuse domain code chỉ sau review/test contract; không mặc định copy nguyên mono-flow/local receipts/asmdefs vào production.

Để tái hiện behavior cũ: A/D hoặc arrows đi; Space nhảy, S+Space drop; J/1 đánh; E NPC/portal/pickup; F/H/M consumables; B túi, C điểm, K skill, L quest; click EXPLICIT focus, Esc clear/modal. Q1–Q6 và Tân Lữ→Kiếm ở ba maps; S2/S3/Cung chỉ fixture definitions, không production content/status. Probe cũ dùng speed 5/jump velocity12/gravity2, Dummy DEF/EVA0, 150ms buffer, unreachable2s→Return untargetable/reset: **không production lock**.

## Source và chạy lại reference

[SliceRules](Assets/_Prototype/VS1_EndToEnd/Domain/SliceRules.cs), [CombatController](Assets/_Prototype/VS1_EndToEnd/Domain/CombatController.cs), [SliceSession](Assets/_Prototype/VS1_EndToEnd/Domain/SliceSession.cs) là prototype domain; [SliceHost](Assets/_Prototype/VS1_EndToEnd/Runtime/SliceHost.cs)/[SliceHud](Assets/_Prototype/VS1_EndToEnd/Runtime/SliceHud.cs) là adapters/presentation; [SliceBuild](Assets/_Prototype/VS1_EndToEnd/Editor/SliceBuild.cs) tạo scene/build reference. Font [DejaVu Sans](Assets/_Prototype/VS1_EndToEnd/Resources/Fonts/DejaVuSans.ttf) có [license](Assets/_Prototype/VS1_EndToEnd/Resources/Fonts/LICENSE.txt).

Từ root repo:

```sh
HUYEN_LO_EDITOR=/home/nguyenvanrin/Unity/Hub/Editor/6000.5.9f1/Editor/Unity
"$HUYEN_LO_EDITOR" -batchmode -nographics -projectPath "$PWD/game" -runTests -testPlatform EditMode -testResults /tmp/huyenlo-reference-edit.xml -logFile /tmp/huyenlo-reference-edit.log
"$HUYEN_LO_EDITOR" -batchmode -nographics -projectPath "$PWD/game" -runTests -testPlatform PlayMode -testResults /tmp/huyenlo-reference-play.xml -logFile /tmp/huyenlo-reference-play.log
"$HUYEN_LO_EDITOR" -batchmode -nographics -quit -projectPath "$PWD/game" -executeMethod HuyenLo.Editor.SliceBuild.Linux -logFile /tmp/huyenlo-reference-build.log
python3 game/PrototypeEvidence/VS1_EndToEnd/verify_feedback.py
```

Build reference mới dùng `Builds/Prototype/VS1_EndToEnd/HuyenLo.x86_64`; output ignored Git. Build Linux trước checkpoint còn tại `Builds/Linux/HuyenLo.x86_64`, là artifact cũ. Development-only [SliceRouteProbe](Assets/_Prototype/VS1_EndToEnd/Runtime/SliceRouteProbe.cs) vẫn tái hiện **route cũ**, không skip quest/EXP/HP/position; nó chạy physics/portal thật nhưng driver tự động, thời gian ×4, không manual feel review:

```sh
game/Builds/Prototype/VS1_EndToEnd/HuyenLo.x86_64 -screen-fullscreen 0 -screen-width 1280 -screen-height 720 -logFile /tmp/huyenlo-reference-player.log --verify-route --capture-route --evidence-path /tmp/huyenlo-reference-route
```

## Evidence và điểm dừng

[validation.json](PrototypeEvidence/VS1_EndToEnd/validation.json), [audit sync cũ](PrototypeEvidence/VS1_EndToEnd/document-audit.json), [route log cũ](PrototypeEvidence/VS1_EndToEnd/continuous-route.log), [video cũ](PrototypeEvidence/VS1_EndToEnd/continuous-route.mp4) giữ nguyên để trace V6.2.0. Paths/source hashes/Git status trong các record này là **historical snapshot trước migration**, không live status. `verify_documents.py` là verifier của lượt sync cũ: đừng chạy lại lên canonical đã thay để overwrite historical evidence. [Audit feedback mới](PrototypeEvidence/VS1_EndToEnd/feedback-audit.json) ghi counts/links/source→destination/meta migration riêng; không chứng minh gameplay mới.

[Migration verification](PrototypeEvidence/VS1_EndToEnd/migration-validation.json): 34 EditMode pass và build Linux reference pass sau move; chỉ chứng minh GUID/build paths/source cũ còn chạy, không behavior V6.2.1.

**G-L PARTIAL; chưa bắt đầu production base hoặc G-N.** Next: harvest/docs → Movement Feel + UI/rig/art probes → design/build production base (G-B) → G-L revision mới → network gate sớm. Các open art/LoS/hybrid/unreachable decisions vẫn ở Analysis; không kéo full Cung/Boss/backend vào vòng feedback này.
