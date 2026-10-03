#!/usr/bin/env python3
"""Audit V6.2.1 docs and prototype relocation against the committed checkpoint.
Counts are computed from files, not estimated. Does not overwrite old runtime evidence.
"""
from pathlib import Path
import hashlib
import json
import random
import re
import subprocess

ROOT = Path(__file__).resolve().parents[3]
HERE = Path(__file__).resolve().parent
REV = '46006c4'
FILES = sorted((ROOT / 'docs/design').glob('*.md'))


def sha(data):
    return hashlib.sha256(data).hexdigest()


def metrics(data):
    text = data.decode(); lines = text.splitlines()
    return dict(lines=len(lines), pipes=sum(line.startswith('|') for line in lines),
                headings=len(re.findall(r'^#{1,6}\s', text, re.M)), sha=sha(data),
                critical=[text.count('53.100'), text.count('95 điểm'), text.count('32.000'),
                          len(re.findall(r'26[\s-]+(?:frames?|khung)', text)),
                          sum('1.800' in line and re.search(r'(?<!\d)200(?!\d)', line) is not None for line in lines)])


def blob(path):
    return subprocess.check_output(['git', 'show', f'{REV}:{path}'], cwd=ROOT)


def anchors(text):
    found = set(re.findall(r'<a id="([^"]+)"', text)); seen = {}
    for heading in re.findall(r'^#{1,6}\s+(.*)', text, re.M):
        slug = re.sub(r'[^\w\- ]', '', heading.lower()).replace(' ', '-')
        n = seen.get(slug, 0); seen[slug] = n + 1
        found.add(slug + ('-' + str(n) if n else ''))
    return found


errors, counts = [], []
for path in FILES:
    rel = path.relative_to(ROOT).as_posix(); old = blob(rel); new = path.read_bytes()
    before, after = metrics(old), metrics(new)
    counts.append(dict(file=rel, before=before, after=after))
    print(path.name, 'lines', before['lines'], '→', after['lines'], 'pipes', before['pipes'], '→', after['pipes'], 'headings', before['headings'], '→', after['headings'], 'critical', before['critical'], '→', after['critical'])
    if any(a < b for a, b in zip(after['critical'], before['critical'])):
        errors.append(rel + ': critical occurrence lost')
    if path.name.startswith('3_') and before['lines'] - after['lines'] >= 100:
        errors.append('STOP: Analysis lost at least 100 lines; user review required')
    # Mathematical tables stay byte-identical; early derived route rows are intentionally synchronized.
    if path.name.startswith('3_'):
        for line in old.decode().splitlines():
            if not line.startswith('|') or not re.search(r'\d', line) or line.startswith(('| A', '| D', '| F')):
                continue
            if line.startswith(('| 3 | Học Viện |', '| 4 | Đồng DS3–DS6 |', '| 5 | Đồng DS2(Q5)')):
                continue
            if line not in new.decode():
                errors.append('Analysis math row lost: ' + line[:100])
    if path.name.startswith('4_'):
        accounting = old.decode().split('<a id="production-accounting"></a>')[1].split('<a id="legacy-visual-flow"></a>')[0]
        # Placement wording changes are permitted; numeric production accounting remains intact.
        for line in accounting.splitlines():
            if line.startswith('|') and re.search(r'\d', line) and not line.startswith(('| Map content placement', '| P05 Hit không stun')) and line not in new.decode():
                errors.append('Art accounting row lost: ' + line[:100])

# All existing Unity metadata and historical runtime evidence retain exact bytes.
tracked = subprocess.check_output(['git', 'ls-tree', '-r', '--name-only', REV], cwd=ROOT, text=True).splitlines()
meta_count, evidence_count = 0, 0
for name in tracked:
    destination = None
    if name == 'game/Assets/HuyenLo.meta':
        destination = ROOT / 'game/Assets/_Prototype/VS1_EndToEnd.meta'
    elif name.startswith('game/Assets/HuyenLo/') and name.endswith('.meta'):
        destination = ROOT / name.replace('game/Assets/HuyenLo/', 'game/Assets/_Prototype/VS1_EndToEnd/', 1)
    if destination:
        meta_count += 1
        if not destination.exists() or destination.read_bytes() != blob(name):
            errors.append('Meta changed/missing: ' + name)
    if name.startswith('game/Validation/') and not name.endswith('verify_documents.py'):
        destination = ROOT / name.replace('game/Validation/', 'game/PrototypeEvidence/VS1_EndToEnd/', 1)
        evidence_count += 1
        if not destination.exists() or destination.read_bytes() != blob(name):
            errors.append('Historical evidence changed/missing: ' + name)

for path in [*FILES, ROOT / 'game/README.md', ROOT / 'research/README.md']:
    for _, url in re.findall(r'\[([^\]]*)\]\(([^\)]+)\)', path.read_text()):
        url = url.strip('<>')
        if re.match(r'[a-z]+:', url): continue
        part, _, fragment = url.partition('#')
        target = (path.parent / part).resolve() if part else path
        # This audit creates its own linked output after validation.
        if target == HERE / 'feedback-audit.json': continue
        if not target.exists(): errors.append(f'{path.name}: missing {url}')
        elif fragment and target.suffix == '.md' and fragment not in anchors(target.read_text()):
            errors.append(f'{path.name}: missing anchor {url}')

proofs = {
    'F01': ('1_HUYEN_LO_GDD.md', 'quests-story', 'Q4 — Chiến lợi phẩm đầu tiên / 3'),
    'F02': ('2_HUYEN_LO_TECHNICAL.md', 'maps', '**MapTransition contract:**'),
    'F03': ('2_HUYEN_LO_TECHNICAL.md', 'shared-combat-input', 'Release không cancel one-shot pending'),
    'F04': ('2_HUYEN_LO_TECHNICAL.md', 'movement-feel', 'Coyote time'),
    'F05': ('2_HUYEN_LO_TECHNICAL.md', 'combat-data', '**Melee separation/reposition prototype:**'),
    'F06': ('5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md', 'phase-gates', 'G-B: production base review'),
    'F07': ('1_HUYEN_LO_GDD.md', 'quests-story', 'Yard có ít nhất 3 Dummy cùng lúc'),
    'F08': ('4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md', 'icons-ui', '**UI usability gate P11:**'),
    'F09': ('3_HUYEN_LO_DESIGN_ANALYSIS.md', 'balance-baselines', 'LEGACY / SUPERSEDED — class Normal'),
}
# Persist the random draw so subsequent audits check the same review sample.
sample_path = HERE / 'feedback-sample.json'
if sample_path.exists(): sample = json.loads(sample_path.read_text())
else:
    sample = random.Random(20261003).sample(list(proofs), 8)
    sample_path.write_text(json.dumps(sample)+'\n')
snippets = []
for key in sample:
    filename, anchor, needle = proofs[key]
    text = (ROOT / 'docs/design' / filename).read_text()
    start = text.index('<a id="' + anchor + '">'); index = text.find(needle, start)
    if index < 0:
        errors.append(key + ': content absent at destination'); continue
    line_start = text.rfind('\n', 0, index) + 1; line_end = text.find('\n', index)
    snippet = text[line_start:line_end if line_end >= 0 else len(text)]
    snippets.append(dict(id=key, destination=filename+'#'+anchor, line=text[:index].count('\n')+1, snippet=snippet))
    print('SAMPLE', key, snippet)

# Targeted stale current-rule checks; historical comparison is retained explicitly.
for path in FILES:
    text = path.read_text()
    for pattern in (r'J-selected', r'ExecuteSelected \(J\)', r'Hold J/slot', r'S \+ Space', r'G-N ngay sau VS-1'):
        if re.search(pattern, text): errors.append(path.name + ': stale current rule ' + pattern)

result = dict(checkpoint=REV, date='2026-10-03', revision='V6.2.1', passed=not errors,
              critical_order=['53.100','95 điểm','32.000','26 frame/khung','1.800/200 same row'],
              comparisons=counts, meta_files_preserved=meta_count, historical_evidence_files_preserved=evidence_count,
              intentional_numeric_route_changes='Analysis early Lv3/4/5 route rows only; new Nấm values derive GDD, old simulations retained. Art P05 wording clarified to resolved-result/visual-only semantics, no numeric budget changed.',
              sampled_source_destination=snippets, errors=errors,
              scope='documentation and prototype relocation; not runtime acceptance of revised gameplay')
(HERE / 'feedback-audit.json').write_text(json.dumps(result,ensure_ascii=False,indent=2)+'\n')
print('AUDIT', 'PASS' if not errors else errors)
raise SystemExit(bool(errors))
