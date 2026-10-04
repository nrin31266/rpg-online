#!/usr/bin/env python3
"""Check INDEX blobs, not working files. --all can be used by CI."""
import argparse,fnmatch,subprocess,sys
from pathlib import Path

def git(*args):return subprocess.check_output(['git',*args])
def main():
 parser=argparse.ArgumentParser();parser.add_argument('--all',action='store_true');args=parser.parse_args()
 root=Path(git('rev-parse','--show-toplevel').decode().strip())
 patterns=[x.strip() for x in (root/'.githooks/large-file-allowlist.txt').read_text().splitlines() if x.strip() and not x.lstrip().startswith('#')]
 raw=git('ls-files','-z') if args.all else git('diff','--cached','--name-only','--diff-filter=ACMR','-z')
 errors=[]
 for name in raw.decode().split('\0'):
  if not name:continue
  suffix=Path(name).suffix.lower()
  if suffix in {'.mp4','.docx','.log'}:errors.append(f'{name}: extension {suffix} is forbidden');continue
  size=int(git('cat-file','-s',':'+name))
  unity=name[name.index('Assets/'):] if 'Assets/' in name else name
  if size>500000 and not any(fnmatch.fnmatchcase(name,p) or fnmatch.fnmatchcase(unity,p) for p in patterns):errors.append(f'{name}: {size} bytes exceeds 500000 (not allowlisted)')
 for error in errors:print(error,file=sys.stderr)
 if errors:print('Move dumps/media outside the repo. Add only intentional asset paths to the size allowlist.',file=sys.stderr)
 return bool(errors)
if __name__=='__main__':sys.exit(main())
