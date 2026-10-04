#!/usr/bin/env python3
"""Counts and preservation audit run by the primary agent, not estimated by agents."""
from pathlib import Path
import hashlib,json,re,random,subprocess
EVIDENCE=Path(__file__).resolve().parent
ROOT=EVIDENCE.parents[3]
base=json.loads((EVIDENCE/'document-baseline.json').read_text())
keys=['53.100','95 điểm','32.000','26 frame','26 khung','1.800','200']
report={};errors=[]
for path,before in base['files'].items():
 p=ROOT/path;t=p.read_text();lines=t.splitlines()
 after={'lines':len(lines),'pipe_rows':sum(x.startswith('|') for x in lines),'headings':sum(bool(re.match(r'^#+\s',x)) for x in lines),'critical':{k:t.count(k) for k in keys},'sha256':hashlib.sha256(p.read_bytes()).hexdigest()}
 report[path]={'before':before,'after':after}
 print(p.name, 'lines',before['lines'],'→',after['lines'],'pipes',before['pipe_rows'],'→',after['pipe_rows'],'headings',before['headings'],'→',after['headings'])
 for k in keys:
  if k!='200' and after['critical'][k]<before['critical'][k]:errors.append('Lost '+k+' in '+path)
 if '3_HUYEN' in path and after['lines']<before['lines']-100:errors.append('STOP: Analysis lost >=100 lines')
 if '3_HUYEN' in path or '4_HUYEN' in path:
  old=subprocess.check_output(['git','show',base['checkpoint']+':'+path],cwd=ROOT,text=True)
  rows=[x.replace('V6.2.3','V6.2.4').replace('nước visual-only','nước nông giảm tốc theo GDD') for x in old.splitlines() if x.startswith('|') and re.search(r'\d',x)]
  missing=[x for x in rows if x not in t]
  if missing:errors.append('Numeric evidence rows changed '+path+': '+str(missing[:2]))
 for label,target in re.findall(r'\[([^\]]+)\]\(([^)]+)\)',t):
  if target.startswith(('http','app:','skill:')):continue
  target=target.strip('<>');file,sep,anchor=target.partition('#');dest=(p.parent/file).resolve() if file else p
  if not dest.exists():errors.append('Broken link '+path+': '+target)
  elif anchor and dest.suffix=='.md':
   dt=dest.read_text();slugs=set()
   for head in re.findall(r'^#+\s+(.+)$',dt,re.M):
    slugs.add(re.sub(r'[^\w\- ]','',head.lower()).replace(' ','-'))
   if anchor not in slugs and f'id="{anchor}"' not in dt:errors.append('Broken anchor '+path+': '+target)
# Inspect sampled real destination content; pairs are additive feedback mappings, not fictitious MOVEs.
pairs=[
 ('solid terrace grammar','docs/design/4_HUYEN_LO_ART_VISUAL_PRODUCTION_ANALYSIS.md','Main elevation đến từ terrace/step/hố nông'),
 ('rear wolf bug root cause','docs/design/2_HUYEN_LO_TECHNICAL.md','rank offset + peer clipping'),
 ('wolf telemetry','prototypes/VS1_EndToEnd/Assets/_Prototype/VS1_EndToEnd/Domain/SliceRules.cs','public int BiteAttempts;'),
 ('independent pack re-engage','prototypes/VS1_EndToEnd/Assets/_Prototype/VS1_EndToEnd/Tests/EditMode/DomainTests.cs','EveryWolfCanReengageWithoutFrontDeathOrTeleport'),
 ('geometric shared rig','prototypes/VS1_EndToEnd/Assets/_Prototype/VS1_EndToEnd/Runtime/GeometricRig.cs','BodyBase/torso'),
 ('RPG tab shell','docs/design/1_HUYEN_LO_GDD.md','Tab và Shift+Tab đổi view'),
 ('quest intro presentation','prototypes/VS1_EndToEnd/Assets/_Prototype/VS1_EndToEnd/Runtime/SliceHud.cs','public static string QuestIntro'),
 ('water cosmetic overlay; feet slowdown owned by layout','prototypes/VS1_EndToEnd/Assets/_Prototype/VS1_EndToEnd/Runtime/SliceHost.cs','WaterFrontOverlay cosmetic'),
 ('skill slots','prototypes/VS1_EndToEnd/Assets/_Prototype/VS1_EndToEnd/Runtime/SliceHud.cs','for(int i=1;i<=3;i++)'),
 ('probe remains partial','docs/design/5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md','G-L vẫn PARTIAL'),
 ('pack returns then patrols','prototypes/VS1_EndToEnd/Assets/_Prototype/VS1_EndToEnd/Domain/SliceSession.cs','private void Patrol(Mob m,double dt)'),
 ('mouse mutation ends render event','prototypes/VS1_EndToEnd/Assets/_Prototype/VS1_EndToEnd/Runtime/SliceHud.cs','Activate(action);GUIUtility.ExitGUI();'),
 ('water feet contact, dry bridge','prototypes/VS1_EndToEnd/Assets/_Prototype/VS1_EndToEnd/Domain/BlockoutLayout.cs','if(w.TouchesFeet(feet))factor='),
 ('single action vs batch menu close','docs/design/1_HUYEN_LO_GDD.md','Buy/Sell/Store/Take giữ view để làm nhiều lần'),
 ('walkable basin bridge movement fixture','prototypes/VS1_EndToEnd/Assets/_Prototype/VS1_EndToEnd/Tests/PlayMode/InputPhysicsTests.cs','WalkableBasinAndBridgeHaveDistinctMovementSpeeds')]

samples=[]
for source,path,proof in random.Random(62420261004).sample(pairs,8):
 t=(ROOT/path).read_text();ok=proof in t
 samples.append({'SOURCE':source,'DESTINATION':path,'proof':proof,'line':t[:t.find(proof)].count('\n')+1 if ok else None,'verified':ok})
 if not ok:errors.append('Mapping missing '+source)
 print('SAMPLE',source,'→',path,':',ok)
# Keep existing .meta GUIDs and the previous evidence immutable.
paths=subprocess.check_output(['git','ls-tree','-r','--name-only',base['checkpoint'],'prototypes/VS1_EndToEnd'],cwd=ROOT,text=True).splitlines()
for path in paths:
 if path.endswith('.meta') or '/PrototypeEvidence/' in path:
  old=subprocess.check_output(['git','show',base['checkpoint']+':'+path],cwd=ROOT)
  if not (ROOT/path).exists() or (ROOT/path).read_bytes()!=old:errors.append('Historical/meta changed '+path)
check=subprocess.run(['git','diff','--check'],cwd=ROOT,capture_output=True,text=True)
if check.returncode:errors.append(check.stdout)
(EVIDENCE/'source-destination-sample.json').write_text(json.dumps(samples,ensure_ascii=False,indent=2)+'\n')
(EVIDENCE/'document-audit.json').write_text(json.dumps({'files':report,'errors':errors,'status':'PASS' if not errors else 'FAIL'},ensure_ascii=False,indent=2)+'\n')
print('AUDIT', 'FAIL' if errors else 'PASS')
for e in errors:print(e)
raise SystemExit(bool(errors))
