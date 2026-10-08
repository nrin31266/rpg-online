#!/usr/bin/env python3
"""Read-only checker for local links/anchors in all docs and the root README. --json writes to stdout."""
import argparse,json,re,sys,unicodedata
from pathlib import Path
from urllib.parse import unquote
ROOT=Path(__file__).resolve().parents[1]
def anchors(text):
 ids=set(re.findall(r'<(?:a|[^>]+)\s+[^>]*id=["\']([^"\']+)',text));seen={}
 for heading in re.findall(r'^#{1,6}\s+(.+)$',text,re.M):
  heading=re.sub(r'\[([^\]]*)\]\([^)]*\)',r'\1',heading);heading=re.sub(r'<[^>]*>','',heading)
  slug=''.join(c for c in heading.lower() if c in '-_ ' or unicodedata.category(c)[0] in 'LN').strip().replace(' ','-')
  n=seen.get(slug,0);seen[slug]=n+1;ids.add(slug+('-'+str(n) if n else ''))
 return ids

def scan():
 broken=[];links=0
 for source in sorted([*(ROOT/'docs').rglob('*.md'), ROOT/'README.md']):
  text=source.read_text()
  for m in re.finditer(r'\[[^\]\n]*\]\(([^)\s]+)\)',text):
   dest=unquote(m.group(1));line=text.count('\n',0,m.start())+1
   if re.match(r'^[a-zA-Z][a-zA-Z0-9+.-]*:',dest):continue
   links+=1;path,sep,anchor=dest.partition('#');target=source.parent/path if path else source
   if not target.exists():broken.append({'file':str(source.relative_to(ROOT)),'line':line,'target':dest,'kind':'missing_file'})
   elif sep and target.suffix=='.md' and anchor not in anchors(target.read_text()):broken.append({'file':str(source.relative_to(ROOT)),'line':line,'target':dest,'kind':'missing_anchor'})
 return {'links_checked':links,'broken_anchors':sum(x['kind']=='missing_anchor' for x in broken),'missing_files':sum(x['kind']=='missing_file' for x in broken),'errors':broken}
if __name__=='__main__':
 p=argparse.ArgumentParser();p.add_argument('--json',action='store_true');a=p.parse_args();r=scan()
 print(json.dumps(r,ensure_ascii=False,indent=2) if a.json else f"{r['links_checked']} local links; broken anchors={r['broken_anchors']}; missing files={r['missing_files']}"+'\n'.join('\n'+str(x) for x in r['errors']))
 sys.exit(bool(r['errors']))
