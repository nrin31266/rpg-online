#!/usr/bin/env python3
"""Verify the recorded pre-edit metrics, preserved evidence, links and sampled moves.
Run from any directory. Writes an audit result, never modifies canonical documents.
"""
import hashlib
import json
import re
import subprocess
from pathlib import Path

ROOT = Path(__file__).resolve().parents[3]
HERE = Path(__file__).resolve().parent
DESIGN = ROOT / 'docs/design'
BASELINE_REVISION = '4a57b80'


def metrics(path):
    text = path.read_text()
    lines = text.splitlines()
    return dict(file=str(path.relative_to(ROOT)), lines=len(lines),
                pipes=sum(line.startswith('|') for line in lines),
                headings=len(re.findall(r'^#{1,6}\s', text, re.M)),
                critical=[text.count('53.100'), text.count('95 điểm'), text.count('32.000'),
                          len(re.findall(r'26[\s-]+(?:frames?|khung)', text)),
                          sum('1.800' in line and re.search(r'(?<!\d)200(?!\d)', line) is not None for line in lines)],
                sha=hashlib.sha256(path.read_bytes()).hexdigest())


def anchors(path):
    text = path.read_text()
    found = set(re.findall(r'<a id="([^"]+)"', text))
    seen = {}
    for heading in re.findall(r'^#{1,6}\s+(.*)', text, re.M):
        slug = re.sub(r'[^\w\- ]', '', heading.lower()).replace(' ', '-')
        count = seen.get(slug, 0)
        seen[slug] = count + 1
        found.add(slug + ('-' + str(count) if count else ''))
    return found


before = json.loads((HERE / 'document-baseline.json').read_text())
errors, comparisons, moves = [], [], []
for old in before:
    path = ROOT / old['file']
    if old['file'].startswith('docs/design/'):
        new = metrics(path)
        comparisons.append(dict(before=old, after=new))
        print(f"{path.name}: lines {old['lines']}→{new['lines']}; pipe rows {old['pipes']}→{new['pipes']}; headings {old['headings']}→{new['headings']}; critical {old['critical']}→{new['critical']}")
        if path.name.startswith('3_') and old['lines'] - new['lines'] >= 100:
            errors.append('STOP: Analysis lost at least 100 lines; user review required.')
        if any(new['critical'][i] < old['critical'][i] for i in range(5)):
            errors.append(f'{path.name}: critical occurrence lost')
        original = subprocess.check_output(['git', 'show', f'{BASELINE_REVISION}:{old["file"]}'], cwd=ROOT)
        if hashlib.sha256(original).hexdigest() != old['sha']:
            errors.append(f'{path.name}: recorded baseline is not the expected Git blob')
        if path.name.startswith('3_'):
            for row in original.decode().splitlines():
                if row.startswith('|') and re.search(r'\d', row) and not row.startswith('| A') and row not in path.read_text():
                    errors.append('Analysis numeric row lost: ' + row[:100])
    elif path.name.startswith('NSO_'):
        destination = ROOT / 'research/notes' / path.name
        preserved = destination.exists() and hashlib.sha256(destination.read_bytes()).hexdigest() == old['sha']
        moves.append(dict(source=old['file'], destination=str(destination.relative_to(ROOT)), sha_preserved=preserved))
        if not preserved or path.exists():
            errors.append(f'{path.name}: move/hash verification failed')
    elif path.name in ('plan.md', 'HUYEN_LO_DESIGN_LOCK_INPUT.md') and path.exists():
        errors.append(f'temporary artifact still exists: {path.name}')

# Canonical links + research router. Historical reports contain pre-existing machine-specific file:// provenance.
for path in [*DESIGN.glob('*.md'), ROOT / 'research/README.md', ROOT / 'game/README.md']:
    if not path.exists():
        continue
    for _, url in re.findall(r'\[([^\]]*)\]\(([^\)]+)\)', path.read_text()):
        url = url.strip('<>')
        if re.match(r'[a-z]+:', url):
            continue
        part, _, frag = url.partition('#')
        target = (path.parent / part).resolve() if part else path
        if not target.exists():
            errors.append(f'{path.name}: missing {url}')
        elif frag and target.suffix == '.md' and frag not in anchors(target):
            errors.append(f'{path.name}: missing anchor {url}')

proofs = {
    'S01': ('1_HUYEN_LO_GDD.md', 'class-combat', 'logical hits'),
    'S02': ('1_HUYEN_LO_GDD.md', 'focus-input', 'EXPLICIT'),
    'S03': ('2_HUYEN_LO_TECHNICAL.md', 'shared-combat-input', 'press token'),
    'S04': ('2_HUYEN_LO_TECHNICAL.md', 'combat-data', 'sáu class SkillIds'),
    'S05': ('1_HUYEN_LO_GDD.md', 'character-power', 'kể cả Lv 5+'),
    'S06': ('4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md', 'player-visual', 'gravity'),
    'S07': ('3_HUYEN_LO_DESIGN_ANALYSIS.md', 'design-lock-rationale', '3/1/0'),
    'S08': ('4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md', 'icons-ui', 'current-max'),
    'S09': ('3_HUYEN_LO_DESIGN_ANALYSIS.md', 'design-lock-rationale', '22,76%'),
    'S10': ('4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md', 'player-visual', '33 pose'),
    'S11': ('2_HUYEN_LO_TECHNICAL.md', 'shared-combat-input', 'E act ngay'),
    'S12': ('5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md', 'vs-1', 'Q1–Q6'),
}
samples = []
for key in json.loads((HERE / 'source-destination-sample.json').read_text()):
    filename, anchor, needle = proofs[key]
    text = (DESIGN / filename).read_text()
    start = text.index('<a id="' + anchor + '">')
    section = text[start:start + 18000]
    if needle not in section:
        errors.append(f'{key}: destination content absent')
        continue
    pos = section.index(needle)
    snippet = section[max(0, pos-110):pos+180].replace('\n', ' ')
    samples.append(dict(id=key, destination=filename+'#'+anchor, content=snippet))
    print('SAMPLE', key, snippet)
result = dict(baseline_revision=BASELINE_REVISION, comparisons=comparisons, research_moves=moves,
              critical_order=['53.100', '95 điểm', '32.000', '26 frame/khung', 'payout 1.800 and 200 on same row'],
              sampled_source_destination=samples, errors=errors, passed=not errors)
(HERE / 'document-audit.json').write_text(json.dumps(result, ensure_ascii=False, indent=2)+'\n')
print('AUDIT', 'PASS' if not errors else errors)
raise SystemExit(bool(errors))
