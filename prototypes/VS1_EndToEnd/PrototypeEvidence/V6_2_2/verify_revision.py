#!/usr/bin/env python3
"""Preserve docs, metadata and historical evidence while updating the disposable mock."""
from pathlib import Path
import hashlib,json,random,re,subprocess
ROOT=Path(__file__).resolve().parents[4]
HERE=Path(__file__).resolve().parent
REV='54350dd'
PROJECT=ROOT/'prototypes/VS1_EndToEnd'
errors=[]
def digest(data):return hashlib.sha256(data).hexdigest()
def metric(data):
 text=data.decode();lines=text.splitlines()
 return dict(lines=len(lines),pipes=sum(x.startswith('|') for x in lines),headings=len(re.findall(r'^#{1,6}\s',text,re.M)),sha256=digest(data),critical=[text.count('53.100'),text.count('95 điểm'),text.count('32.000'),len(re.findall(r'26[\s-]+(?:frames?|khung)',text)),sum('1.800' in x and re.search(r'(?<!\d)200(?!\d)',x)!=None for x in lines)])
def blob(name):return subprocess.check_output(['git','show',REV+':'+name],cwd=ROOT)
def mapped(text):return text.replace('../../game/','../../prototypes/VS1_EndToEnd/').replace('game/PrototypeEvidence/','prototypes/VS1_EndToEnd/PrototypeEvidence/')
def anchors(text):
 ids=set(re.findall(r'<a id="([^"]+)"',text));seen={}
 for name in re.findall(r'^#{1,6}\s+(.*)',text,re.M):
  slug=re.sub(r'[^\w\- ]','',name.lower()).replace(' ','-');n=seen.get(slug,0);seen[slug]=n+1;ids.add(slug+('-'+str(n) if n else ''))
 return ids
comparisons=[]
for path in sorted((ROOT/'docs/design').glob('*.md')):
 rel=path.relative_to(ROOT).as_posix();old=blob(rel);new=path.read_bytes();a,b=metric(old),metric(new)
 comparisons.append(dict(file=rel,before=a,after=b))
 print(path.name, 'lines',a['lines'],'→',b['lines'],'pipes',a['pipes'],'→',b['pipes'],'headings',a['headings'],'→',b['headings'])
 if any(n<o for n,o in zip(b['critical'],a['critical'])):errors.append('Critical value lost: '+rel)
 if path.name.startswith('3_') and a['lines']-b['lines']>=100:errors.append('STOP: Analysis lost hundreds of lines; user review required')
 if path.name.startswith(('3_','4_')):
  for line in old.decode().splitlines():
   if line.startswith('|') and re.search(r'\d',line) and mapped(line) not in new.decode():errors.append('Numeric/evidence table row lost: '+path.name+' '+line[:100])
tracked=subprocess.check_output(['git','ls-tree','-r','--name-only',REV],cwd=ROOT,text=True).splitlines();metas=history=0
for name in tracked:
 if name.startswith('game/Assets/') and name.endswith('.meta'):
  metas+=1;dest=ROOT/name.replace('game/','prototypes/VS1_EndToEnd/',1)
  if not dest.exists() or blob(name)!=dest.read_bytes():errors.append('GUID/meta changed: '+name)
 if name.startswith('game/PrototypeEvidence/'):
  history+=1;dest=ROOT/name.replace('game/','prototypes/VS1_EndToEnd/',1)
  if not dest.exists() or blob(name)!=dest.read_bytes():errors.append('Historical evidence changed: '+name)
for path in [*(ROOT/'docs/design').glob('*.md'),ROOT/'README.md',PROJECT/'README.md']:
 for url in re.findall(r'\[[^\]]*\]\(([^\)]+)\)',path.read_text()):
  url=url.strip('<>')
  if re.match(r'[a-z]+:',url):continue
  part,_,fragment=url.partition('#');target=(path.parent/part).resolve() if part else path
  if target in [HERE/'validation.json',HERE/'document-audit.json']:continue
  if not target.exists():errors.append(path.name+' missing link '+url)
  elif fragment and target.suffix=='.md' and fragment not in anchors(target.read_text()):errors.append(path.name+' missing anchor '+url)
proofs={
 'keyboard':('1_HUYEN_LO_GDD.md','**Menu bằng bàn phím:**'),
 'modules':('4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md','1. Chuẩn bị tám module:'),
 'lifecycle':('4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md','**Đường đi của file:**'),
 'manifest':('4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md','**Minimum pose manifest'),
 'sockets':('4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md','**Socket probe A02:**'),
 'dod':('4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md','- [ ] Canvas/PPU/pivot'),
 'style':('4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md','### 22.3. Visual language'),
 'provenance':('4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md','Một record cho mỗi nguồn/pack'),
 'sequence':('3_HUYEN_LO_DESIGN_ANALYSIS.md','CURRENT thu hoạch mock'),
 'supply':('1_HUYEN_LO_GDD.md','**Tutorial supply active-step only:**')}
sample_path=HERE/'source-destination-sample.json'
if sample_path.exists():sample=json.loads(sample_path.read_text())
else:sample=random.Random(20261004).sample(list(proofs),8);sample_path.write_text(json.dumps(sample)+'\n')
snippets=[]
for key in sample:
 name,needle=proofs[key];text=(ROOT/'docs/design'/name).read_text();line=next((x for x in text.splitlines() if needle in x),None)
 if line is None:errors.append('Missing source→destination '+key)
 else:snippets.append(dict(id=key,file=name,line=text[:text.index(needle)].count('\n')+1,snippet=line));print('SAMPLE',key,line)
check=subprocess.run(['git','diff','--check'],cwd=ROOT,capture_output=True,text=True)
if check.returncode:errors.append(check.stdout+check.stderr)
result=dict(revision='V6.2.2',date='2026-10-04',checkpoint=REV,passed=not errors,comparisons=comparisons,critical_order=['53.100','95 điểm','32.000','26 frame/khung','1.800/200 same row'],meta_files_preserved=metas,historical_files_preserved=history,sampled_source_destination=snippets,errors=errors,scope='documentation and disposable prototype; no production base/Dedicated/backend acceptance')
(HERE/'document-audit.json').write_text(json.dumps(result,ensure_ascii=False,indent=2)+'\n')
print('AUDIT', 'PASS' if not errors else errors)
raise SystemExit(bool(errors))
