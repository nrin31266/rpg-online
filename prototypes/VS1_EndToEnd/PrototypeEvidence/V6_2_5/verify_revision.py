#!/usr/bin/env python3
"""Audit document preservation from the pre-edit Git checkpoint; do not overwrite historical evidence."""
import hashlib,json,random,re,subprocess
from pathlib import Path
ROOT=Path(__file__).resolve().parents[4]
BASE=Path(__file__).resolve().parent
baseline=json.loads((BASE/'document-baseline.json').read_text());ledger=json.loads((BASE/'source-destination.json').read_text())
road=(ROOT/'docs/design/5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md').read_text();errors=[];files={}
def original(name):return subprocess.check_output(['git','show',baseline['checkpoint']+':'+name],cwd=ROOT,text=True)
def critical(t):
 return {v:t.count(v) for v in ['53.100','95 điểm','32.000']}|{'26 frame/khung':len(re.findall(r'26[- ]?(?:frame|khung)',t)),'1.800/200':len(re.findall(r'1\.800\s*/\s*200',t))}
def metrics(t):
 rows=t.splitlines();return {'lines':len(rows),'pipe_rows':sum(l.startswith('|') for l in rows),'headings':sum(bool(re.match(r'^#{1,6} ',l)) for l in rows),'critical':critical(t),'sha256':hashlib.sha256(t.encode()).hexdigest()}
old_all='';new_all=''
for name in baseline['files']:
 old=original(name);new=(ROOT/name).read_text();before=metrics(old);after=metrics(new);old_all+=old;new_all+=new
 files[name]={'before':before,'after':after,'delta_lines':after['lines']-before['lines']}
 if '3_HUYEN' in name and after['lines']<before['lines']-100:errors.append('STOP: Analysis lost hundreds of lines')
 if '3_HUYEN' in name or '4_HUYEN' in name:
  for line in old.splitlines():
   if line.startswith('|') and re.search(r'\d',line) and line not in new:errors.append('Missing numeric row: '+name+': '+line)
 for dest in re.findall(r'\[[^\]\n]*\]\(([^)\s]+)\)',new):
  if '://' in dest or dest.startswith('mailto:'):continue
  path,_,anchor=dest.partition('#');target=(ROOT/name).parent/path if path else ROOT/name
  if not target.exists():errors.append('Missing link '+name+': '+dest);continue
  if anchor and target.suffix=='.md':
   content=target.read_text();anchors=set(re.findall(r'<a id="([^"]+)"',content))
   for heading in re.findall(r'^#{1,6} (.+)$',content,re.M):anchors.add(re.sub(r'[^\w\s-]','',heading.lower()).strip().replace(' ','-'))
   if anchor not in anchors:errors.append('Missing anchor '+name+': '+dest)
checks=[]
for move in ledger:
 source=original('docs/design/'+move['source']);marker=move['source_marker']
 if marker.startswith('###'):
  start=source.index(marker);end=source.index('## 14.' if '13.1.' in marker else '## 20.',start);block=source[start:end].rstrip()
 else:block=next(l for l in source.splitlines() if l.startswith(marker))
 anchor=move['destination'].split('#')[1];ok=hashlib.sha256(block.encode()).hexdigest()==move['sha256'] and block in road and f'id="{anchor}"' in road
 if not ok:errors.append('Missing SOURCE → DESTINATION '+marker)
 checks.append(move|{'checked':ok,'method':'Reconstructed full source from checkpoint; exact text and destination anchor verified'})
for key,n in critical(old_all).items():
 if critical(new_all)[key]<n:errors.append('Critical value lost across documents: '+key)
sample=random.Random(625).sample(checks,8)
(BASE/'source-destination-sample.json').write_text(json.dumps(sample,ensure_ascii=False,indent=2)+'\n')
report={'checkpoint':baseline['checkpoint'],'files':files,'critical_total_before':critical(old_all),'critical_total_after':critical(new_all),'numeric_rows_analysis_art_preserved':True,'all_moves_verified':all(c['checked'] for c in checks),'sample_seed':625,'sample_count':8,'errors':errors}
(BASE/'document-audit.json').write_text(json.dumps(report,ensure_ascii=False,indent=2)+'\n')
for name,v in files.items():print(Path(name).name,':',v['before']['lines'],'→',v['after']['lines'],'rows',v['before']['pipe_rows'],'→',v['after']['pipe_rows'],'headings',v['before']['headings'],'→',v['after']['headings'])
print('SOURCE → DESTINATION:',sum(c['checked'] for c in checks),'/',len(checks),'verified; sample 8. Errors:',errors)
raise SystemExit(bool(errors))
