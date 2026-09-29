# Huyền Lộ Design Documentation

## Đọc trước

1. [GDD](1_HUYEN_LO_GDD.md): game, luật, scope và DoD hiện hành.
2. [Technical](2_HUYEN_LO_TECHNICAL.md): hợp đồng triển khai, roadmap và QA.
3. [Design Analysis](3_HUYEN_LO_DESIGN_ANALYSIS.md): balance, evidence và quyết định mở; tra theo feature đang code.

## Authority

GDD = design authority; Analysis = simulation/quyết định mở; Technical = implementation contract.

Nếu Analysis/Technical mâu thuẫn GDD, GDD thắng. Deterministic defect đã xác minh phải sửa đồng bộ; proposal không tự đổi luật.

User lock và review đã thống nhất được ghi vào GDD hiện hành. Nguồn tham khảo không tự đổi design.

## Quick Routing

| Tôi đang làm | Đọc |
| --- | --- |
| Combat / attributes | [GDD §4](1_HUYEN_LO_GDD.md#gdd-4) + [Analysis §2](3_HUYEN_LO_DESIGN_ANALYSIS.md#combat-analysis) |
| Mob / Linh Biến / Boss | [GDD §5](1_HUYEN_LO_GDD.md#gdd-5) + [Technical §7](2_HUYEN_LO_TECHNICAL.md#timers) |
| EXP / economy / enhance | [GDD §3](1_HUYEN_LO_GDD.md#gdd-3), [§7](1_HUYEN_LO_GDD.md#gdd-7) + [Analysis §3](3_HUYEN_LO_DESIGN_ANALYSIS.md#economy-analysis) |
| Quest / decisions | [GDD §6](1_HUYEN_LO_GDD.md#gdd-6) + [Analysis §5](3_HUYEN_LO_DESIGN_ANALYSIS.md#open-decisions) |
| Network / profiles | [Technical §4](2_HUYEN_LO_TECHNICAL.md#network-authority), [§5](2_HUYEN_LO_TECHNICAL.md#profile-authority) |
| Save | [Technical §6](2_HUYEN_LO_TECHNICAL.md#persistence) |
| Art / UI | [GDD §11](1_HUYEN_LO_GDD.md#gdd-11) + [Technical §8](2_HUYEN_LO_TECHNICAL.md#art-contract), [§9](2_HUYEN_LO_TECHNICAL.md#ui-notes) |
| Scope / demo / QA | [GDD §12](1_HUYEN_LO_GDD.md#gdd-12) + [Technical §10](2_HUYEN_LO_TECHNICAL.md#roadmap), [§12](2_HUYEN_LO_TECHNICAL.md#qa) |

## Quy ước cập nhật

Giữ bốn file trong bộ active docs; tiền tố 1/2/3 biểu thị thứ tự đọc, README là điểm vào. Version chỉ nằm trong GDD; không tạo GDD versioned, audit, archive hay Open Questions riêng.

Trạng thái/version và DoD hiện hành xem GDD; bằng chứng nghiệm thu xem Technical.

Giữ số BASELINE khi chưa chốt; TUNABLE cần playtest; P1/P2 không thành requirement P0. Quyết định mở chỉ nằm trong một bảng ở Analysis.

Lịch sử thay đổi nằm trong Git.

Tra cứu chi tiết hơn qua Full Routing Index ở cuối GDD.
