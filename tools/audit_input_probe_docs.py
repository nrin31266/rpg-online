#!/usr/bin/env python3
"""Read-only counts and independent SOURCE → DESTINATION sample for this probe task."""
import hashlib, json, random, re, subprocess
from pathlib import Path

ROOT=Path(__file__).resolve().parents[1]
EVIDENCE=ROOT/'prototypes/VS1_EndToEnd/PrototypeEvidence'
def digest(data):return hashlib.sha256(data).hexdigest()
def counts(path):
    raw=path.read_bytes();text=raw.decode()
    return {'lines':len(text.splitlines()),'pipe_rows':sum(x.startswith('|') for x in text.splitlines()),
            'headings':len(re.findall(r'^#{1,6}\s',text,re.M)),
            'critical':{v:text.count(v) for v in ['53.100','95 điểm','32.000','26 frame','26 khung','1.800 / 200']},'sha256':digest(raw)}
def audit():
    before=json.loads((EVIDENCE/'Baseline/preflight.json').read_text())
    migration=json.loads((EVIDENCE/'InputProbes/source-destination.json').read_text())
    changes=[]
    for name,old in before['docs_before'].items():
        now=counts(ROOT/name)
        if name in before['readonly_files']:assert now==old,('Protected document changed',name)
        if '/3_' in name:
            assert old['lines']-now['lines']<100,'STOP: Analysis lost hundreds of lines'
            assert now['critical']==old['critical'],'Analysis critical values changed'
        changes.append({'file':name,'before':old,'after':now})
    readonly=sorted(set(before['readonly_files']) | {x for x in subprocess.check_output(['git','ls-files']).decode().splitlines() if x.endswith('README.md')})
    for name in readonly:
        old=subprocess.check_output(['git','show',before['head']+':'+name])
        assert (ROOT/name).read_bytes()==old,('Read-only file modified',name)
    # Original §7–9 remains byte-identical after normalizing the explicitly allowed tag routing.
    import tarfile
    with tarfile.open(Path(before['outside_backup'])/'working-files.tar.gz') as archive:
        original=archive.extractfile('docs/design/5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md').read().decode()
    roadmap=(ROOT/'docs/design/5_HUYEN_LO_IMPLEMENTATION_ROADMAP.md').read_text()
    history=(ROOT/'prototypes/VS1_EndToEnd/CHANGELOG.md').read_text()
    normalized=roadmap
    for h,v in migration['tags'].items():
        normalized=normalized.replace('`'+v['tag']+'` (local-only archive, không có trên origin)','`'+h+'`')
    section=lambda x:x[x.index('# 7.'):x.index('<a id="prototype-visual-review">')]
    assert section(normalized)==section(original),'Roadmap §7–9 changed beyond hash routing'
    samples=[]
    eligible=[x for x in migration['blocks'] if len(x['source_text'])>=100]
    for block in random.Random(1004).sample(eligible,8):
        assert block['source_text'] in original,'Source ledger does not match original'
        assert block['destination_text'] in history,'Destination ledger missing actual content'
        assert digest(block['source_text'].encode())==block['source_sha256']
        assert digest(block['destination_text'].encode())==block['destination_sha256']
        # Check independent of the move tool: undo links/tag routing, then compare every word.
        strip_links=lambda x:re.sub(r'\[([^\]\n]*)\]\(([^)\s]+)\)',r'\1',x)
        dest=block['destination_text']
        for h,v in migration['tags'].items():dest=dest.replace('`'+v['tag']+'` (local-only archive, không có trên origin)','`'+h+'`')
        assert strip_links(dest)==strip_links(block['source_text']),'Content changed beyond routing'
        samples.append({'id':block['id'],'source':block['source'],'destination':block['destination'],
                        'excerpt':block['source_text'][:160],'actual_content_verified':True})
    # All blocks, not only samples, must actually exist at the destination.
    assert all(b['destination_text'] in history for b in migration['blocks'])
    critical=['53.100','95 điểm','32.000','26 frame','26 khung','1.800 / 200']
    migration_values={v:{'source':sum(b['source_text'].count(v) for b in migration['blocks']),
                         'destination':sum(b['destination_text'].count(v) for b in migration['blocks'])} for v in critical}
    assert all(x['source']==x['destination'] for x in migration_values.values())
    result=json.loads(subprocess.check_output(['python3',str(ROOT/'tools/check_doc_links.py'),'--json']))
    assert result['broken_anchors']==0 and result['missing_files']==0,result
    # The requested checker scans design docs; also verify the new destination's own links here.
    import importlib.util
    spec=importlib.util.spec_from_file_location('doc_links',ROOT/'tools/check_doc_links.py')
    module=importlib.util.module_from_spec(spec);spec.loader.exec_module(module)
    destination_links=0
    from urllib.parse import unquote
    for m in re.finditer(r'\[[^\]\n]*\]\(([^)\s]+)\)',history):
        dest=unquote(m.group(1))
        if re.match(r'^[a-zA-Z][a-zA-Z0-9+.-]*:',dest):continue
        path,sep,anchor=dest.partition('#')
        target=ROOT/'prototypes/VS1_EndToEnd'/path if path else ROOT/'prototypes/VS1_EndToEnd/CHANGELOG.md'
        assert target.exists(),('CHANGELOG file missing',dest)
        if sep and target.suffix=='.md':assert anchor in module.anchors(target.read_text()),('CHANGELOG anchor missing',dest)
        destination_links+=1
    return {'documents':changes,'source_destination_sample':samples,'blocks_preserved':len(migration['blocks']),
            'roadmap_7_to_9_preserved':True,'readonly_verified':readonly,'links':result,
            'critical_values_in_move':migration_values,'changelog_local_links_verified':destination_links}
if __name__=='__main__':print(json.dumps(audit(),ensure_ascii=False,indent=2))
