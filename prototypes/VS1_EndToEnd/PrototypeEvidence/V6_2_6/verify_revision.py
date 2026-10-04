#!/usr/bin/env python3
"""Count docs by script and preserve canonical docs plus the prior relocation ledger."""
import hashlib,json,random,re,subprocess
from pathlib import Path
ROOT=Path(__file__).resolve().parents[4];BASE=Path(__file__).resolve().parent
baseline=json.loads((BASE/'document-baseline.json').read_text());errors=[];files={}
def original(checkpoint,name):return subprocess.check_output(['git','show',checkpoint+':'+name],cwd=ROOT,text=True)
def metrics(t):
 rows=t.splitlines();return {'lines':len(rows),'pipe_rows':sum(l.startswith('|') for l in rows),'headings':sum(bool(re.match(r'^#{1,6} ',l)) for l in rows),'critical':{v:t.count(v) for v in ['53.100','95 điểm','32.000','26 frame','26 khung','1.800 / 200']},'sha256':hashlib.sha256(t.encode()).hexdigest()}
for name in baseline['files']:
 old=original(baseline['checkpoint'],name);new=(ROOT/name).read_text();before=metrics(old);after=metrics(new);files[name]={'before':before,'after':after,'delta_lines':after['lines']-before['lines']}
 if before!=baseline['files'][name]:errors.append('Baseline inconsistent '+name)
 if not Path(name).name.startswith('5_') and before['sha256']!=after['sha256']:errors.append('Canonical docs 1–4 changed: '+name)
 if '3_HUYEN' in name and after['lines']<before['lines']-100:errors.append('STOP: Analysis lost hundreds of lines')
 for value,count in before['critical'].items():
  if after['critical'][value]<count:errors.append('Critical literal lost '+name+' '+value)
 for dest in re.findall(r'\[[^\]\n]*\]\(([^)\s]+)\)',new):
  if '://' in dest or dest.startswith('mailto:'):continue
  path,_,anchor=dest.partition('#');target=(ROOT/name).parent/path if path else ROOT/name
  if not target.exists():errors.append('Missing link '+name+': '+dest);continue
  if anchor and target.suffix=='.md':
   content=target.read_text();anchors=set(re.findall(r'<a id="([^"]+)"',content))
   for heading in re.findall(r'^#{1,6} (.+)$',content,re.M):anchors.add(re.sub(r'[^\w\s-]','',heading.lower()).strip().replace(' ','-'))
   if anchor not in anchors:errors.append('Missing anchor '+name+': '+dest)
# No moves this revision. Recheck the existing 17 SOURCE → DESTINATION records read-only.
prior=BASE.parent/'V6_2_5';ledger=json.loads((prior/'source-destination.json').read_text());old_checkpoint=json.loads((prior/'document-baseline.json').read_text())['checkpoint']
road=(ROOT/'docs/design/5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md').read_text();checks=[]
for move in ledger:
 source=original(old_checkpoint,'docs/design/'+move['source']);marker=move['source_marker']
 if marker.startswith('###'):
  start=source.index(marker);end=source.index('## 14.' if '13.1.' in marker else '## 20.',start);block=source[start:end].rstrip()
 else:block=next(l for l in source.splitlines() if l.startswith(marker))
 anchor=move['destination'].split('#')[1];ok=hashlib.sha256(block.encode()).hexdigest()==move['sha256'] and block in road and f'id="{anchor}"' in road
 if not ok:errors.append('Historical relocation lost '+marker)
 checks.append(move|{'checked':ok,'method':'Root script reconstructs exact source text from prior checkpoint and checks full destination block'})
sample=random.Random(626).sample(checks,8);(BASE/'source-destination-sample.json').write_text(json.dumps(sample,ensure_ascii=False,indent=2)+'\n')
report={'checkpoint':baseline['checkpoint'],'files':files,'docs_1_to_4_unchanged':all(v['before']['sha256']==v['after']['sha256'] for n,v in files.items() if not Path(n).name.startswith('5_')),'new_moves':0,'historical_moves_checked':len(checks),'all_historical_moves_preserved':all(c['checked'] for c in checks),'sample_seed':626,'sample_count':8,'errors':errors}
(BASE/'document-audit.json').write_text(json.dumps(report,ensure_ascii=False,indent=2)+'\n')
for n,v in files.items():print(Path(n).name,':',v['before']['lines'],'→',v['after']['lines'],'rows',v['before']['pipe_rows'],'→',v['after']['pipe_rows'],'headings',v['before']['headings'],'→',v['after']['headings'])
print('SOURCE → DESTINATION:',sum(c['checked'] for c in checks),'/',len(checks),'verified; sample 8. Errors:',errors)
raise SystemExit(bool(errors))
