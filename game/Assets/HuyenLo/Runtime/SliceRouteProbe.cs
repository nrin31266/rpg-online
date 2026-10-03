#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HuyenLo.Domain;
using UnityEngine;

namespace HuyenLo.Runtime
{
    // Acceptance driver only: issues the same movement/interaction/command intents as the UI.
    // It never assigns quest, EXP, player position or HP; movement is Rigidbody2D integration.
    public sealed class SliceRouteProbe : MonoBehaviour
    {
        public string Failure {get;private set;}
        public bool Finished {get;private set;}
        private SliceHost host;
        private SliceSession S=>host.Session;
        private bool capture,cli;
        private string evidence;
        private int frame;
        private float lastCapture;
        private void Start() {
            var args=Environment.GetCommandLineArgs();cli=args.Contains("--verify-route");if(!cli)return;
            capture=args.Contains("--capture-route");int i=Array.IndexOf(args,"--evidence-path");evidence=i>=0&&i+1<args.Length?args[i+1]:Path.Combine(Application.persistentDataPath,"VS1-validation");
            Directory.CreateDirectory(evidence);if(capture)Directory.CreateDirectory(Path.Combine(evidence,"frames"));
            StartCoroutine(Cli());
        }
        private IEnumerator Cli() {
            yield return Run(GetComponent<SliceHost>());
            File.WriteAllLines(Path.Combine(evidence,"continuous-route.log"),S.Events);
            File.WriteAllText(Path.Combine(evidence,"route-result.txt"),$"{(Finished?"PASS":"FAIL")}\n{Failure}\nQuest={S.Quest}, School={S.Player.School}, Lv={S.Player.Level}, receipts={S.Receipts.Count}\nQ1..Q6 completed={string.Join(",",Enumerable.Range(1,6).Select(i=>S.Receipts.Contains($"Q{i}.completed")))}\nActual Rigidbody2D movement; no editor teleport/quest/EXP injection. Speed-up timeScale=4.\n");
            Debug.Log("[VS1-ROUTE] "+(Finished?"PASS":Failure));
            yield return new WaitForSecondsRealtime(1);Application.Quit(Finished?0:2);
        }
        private void Update() {
            if(!capture||!cli||host==null)return;
            if(Time.unscaledTime-lastCapture>=.1f){lastCapture=Time.unscaledTime;ScreenCapture.CaptureScreenshot(Path.Combine(evidence,"frames",$"{frame++:D5}.png"));}
        }
        public IEnumerator Run(SliceHost value) {
            host=value;host.ExternalInput=true;float old=Time.timeScale;Time.timeScale=4;
            var stack=new Stack<IEnumerator>();stack.Push(Route());
            while(stack.Count>0){
                object next=null;bool more=false;
                try { more=stack.Peek().MoveNext();if(more)next=stack.Peek().Current; }
                catch(Exception e){Failure=e.ToString();break;}
                if(!more){stack.Pop();continue;}
                if(next is IEnumerator nested){stack.Push(nested);continue;}
                yield return next;
            }
            Finished=Failure==null&&stack.Count==0;
            host.ExternalAxis=0;host.CombatRelease("J");host.CombatRelease("1");Time.timeScale=old;
        }
        private IEnumerator Move(double x) {
            host.Hud.Close();double started=S.Now;
            while(Math.Abs(host.Body.position.x-x)>.22){
                Check(S.Player.Alive,"player died during walking");Check(S.Now-started<35,"walk timed out "+x);
                host.ExternalAxis=host.Body.position.x<x?1:-1;HealIfNeeded();yield return new WaitForFixedUpdate();
            }
            host.ExternalAxis=0;yield return new WaitForFixedUpdate();
        }
        private void HealIfNeeded(){if(S.Player.Hp<S.Player.Stats.Hp*.45)S.Potion(true);}
        private void Check(bool ok,string message){if(!ok)throw new InvalidOperationException(message+"; "+S.Objective);}
        private IEnumerator Talk(string npc,bool accept=false,bool turnIn=false) {
            var a=SliceSession.Anchors.First(x=>x.Id==npc);Check(S.Player.Map==a.Map,"wrong map for "+npc);
            yield return Move(a.Position.X);Check(S.Interact(npc),"interact "+npc);host.Hud.OpenNpc(npc);
            if(accept)Check(S.AcceptQuest(),"accept "+npc);if(turnIn)Check(S.TurnIn(),"turn-in "+npc);host.Hud.Close();yield return new WaitForFixedUpdate();
        }
        private IEnumerator Portal(string id) {
            yield return Move(SliceSession.Anchors.First(x=>x.Id==id).Position.X);host.Interact();yield return new WaitForFixedUpdate();yield return new WaitForFixedUpdate();
        }
        private IEnumerator Fight(Mob mob) {
            yield return Move(mob.Position.X+(host.Body.position.x<mob.Position.X?-.8:.8));
            S.Combat.Explicit(mob);host.CombatPress("J");int life=mob.Generation;double start=S.Now;
            while(mob.Alive&&mob.Generation==life){Check(S.Player.Alive,"player died in combat");Check(S.Now-start<20,"combat stalled");HealIfNeeded();yield return new WaitForFixedUpdate();}
            host.CombatRelease("J");S.Combat.ClearFocus();while(S.Combat.Running!=null)yield return new WaitForFixedUpdate();
        }
        private IEnumerator WaitStage(int stage,double timeout=5) {
            double start=S.Now;while(S.Stage!=stage){Check(S.Now-start<timeout,"stage timed out "+stage);yield return new WaitForFixedUpdate();}
        }
        private IEnumerator Route() {
            yield return Talk("Lam",accept:true);yield return Talk("Yen");yield return Talk("Bach");yield return Talk("Moc");yield return Talk("Lam",turnIn:true);
            Check(S.Quest==2,"Q1 reward");yield return Talk("Lam",accept:true);yield return Portal("toAcademy");yield return Move(0);yield return WaitStage(1);
            yield return Move(8);host.ExternalJump=true;yield return new WaitForFixedUpdate();
            double jumpStart=S.Now;while(S.Stage!=2){Check(S.Now-jumpStart<5,"jump ledge not reached by physics");yield return new WaitForFixedUpdate();}
            host.ExternalDrop=true;yield return new WaitForFixedUpdate();yield return new WaitForFixedUpdate();Check(S.Receipts.Contains("Q2.dropped"),"drop input did not ignore platform; body="+host.Body.position+" grounded="+host.Grounded);yield return WaitStage(3);
            yield return Portal("toVillageA");yield return Talk("Lam",turnIn:true);
            yield return Portal("toAcademy");yield return Talk("Phong",accept:true);
            Check(S.Equip(S.Player.Inventory.Bag.First(x=>x.Id=="wood").Instance),"equip wood");yield return Move(22);yield return WaitStage(2);
            var dummy=S.Mobs.First(x=>x.Dummy);double dummyStart=S.Now;host.CombatPress("J");
            while(S.QuestState!=QuestState.Ready){Check(S.Now-dummyStart<100,"three Dummy lives timed out");yield return new WaitForFixedUpdate();}
            host.CombatRelease("J");yield return Talk("Phong",turnIn:true);
            Check(S.Equip(S.Player.Inventory.Bag.First(x=>x.Id=="pants1").Instance),"equip pants");
            yield return Portal("toVillageA");yield return Talk("Yen",accept:true);
            Check(S.Buy("food1","Yen")&&S.Buy("hp1","Yen")&&S.Buy("mp1","Yen"),"Q4 supply purchase");S.Buy("hp1","Yen");Check(S.UseFood(),"Food F");
            yield return Portal("toMist");yield return Move(50);yield return WaitStage(3);
            while(S.QuestState!=QuestState.Ready){
                var mob=S.Mobs.Where(x=>x.Map==Map.Mist&&x.Level==4&&x.Alive).OrderBy(x=>x.Position.Distance(S.Player.Position)).FirstOrDefault();
                if(mob==null){yield return new WaitForFixedUpdate();continue;}yield return Fight(mob);
            }
            yield return Portal("toVillageM");yield return Talk("Yen",turnIn:true);
            Check(S.Player.Level>=5&&S.Player.ResetAtFive&&S.Player.Unspent>=20,"Lv5 reset");yield return Talk("Bach",accept:true);yield return Portal("toMist");
            var mushroom=S.Mobs.First(x=>x.Slot=="DS2.slot1");while(!mushroom.Alive)yield return new WaitForFixedUpdate();yield return Fight(mushroom);yield return WaitStage(1);
            for(int i=0;i<8&&S.Stage==1;i++){host.Interact();yield return new WaitForFixedUpdate();}
            Check(S.Stage==2,"Q5 immediate E tutorial pickup");Check(S.Equip(S.Player.Inventory.Bag.First(x=>x.Id=="armor1"&&x.Binding=="Q5").Instance),"Q5 equip armor");
            yield return Portal("toVillageM");yield return Talk("Bach");Check(S.Sell(S.Player.Inventory.Bag.First(x=>x.Id=="mushroom"&&x.Binding=="Q5").Instance),"tutorial sell");yield return Talk("Bach",turnIn:true);
            yield return Talk("Ta",accept:true);yield return Portal("toAcademy");yield return Move(3);yield return WaitStage(1);
            Check(S.Interact("ClassHall"),"ClassHall E");host.Hud.OpenNpc("ClassHall");Check(S.ChooseSword(),"class transition");host.Hud.Close();
            Check(S.Equip(S.Player.Inventory.Bag.First(x=>x.Id=="sword1").Instance),"equip Sword");Check(S.Allocate("STR"),"allocate real point");Check(S.Learn(S.Player.Inventory.Bag.First(x=>x.Id=="manual1").Instance),"learn book");
            yield return Move(21.2);while(!dummy.Alive)yield return new WaitForFixedUpdate();S.Combat.ClearFocus();host.CombatPress("1",1);
            yield return WaitStage(4);Check(S.Potion(false),"Q6 reserved M potion actual consumption");host.CombatRelease("1");
            yield return Portal("toVillageA");yield return Talk("Ta",turnIn:true);
            Check(S.Complete&&S.Player.School==School.Sword,"continuous route did not complete");
            Check(Enumerable.Range(1,6).All(i=>S.Receipts.Contains($"Q{i}.completed")),"missing completion receipt");
            S.Emit("VS1 continuous Q1→Q6 PASS — physics walking, jumping, drop and real portals.");
        }
    }
}
#endif
