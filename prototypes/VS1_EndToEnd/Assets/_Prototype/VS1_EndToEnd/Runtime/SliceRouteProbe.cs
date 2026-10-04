using System;
using System.Collections;
using System.Collections.Generic;
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
        private double releaseJumpAt;
        private void Start() {
            if(Environment.GetCommandLineArgs().Contains("--verify-route"))StartCoroutine(Cli());
        }
        private IEnumerator Cli() {
            yield return Run(GetComponent<SliceHost>());
            Debug.Log("[VS1-ROUTE] "+(Finished?"PASS Q1–Q6 fresh, actual Rigidbody2D and UI commands":Failure));
            yield return new WaitForSecondsRealtime(1);Application.Quit(Finished?0:2);
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
            host.ExternalAxis=0;host.CombatRelease("1");Time.timeScale=old;
        }


        private IEnumerator Move(double x) {
            host.Hud.Close();double started=S.Now;
            while(Math.Abs(host.Body.position.x-x)>.22){
                Check(S.Player.Alive,"player died during walking");Check(S.Now-started<35,"walk timed out "+x);
                host.ExternalAxis=host.Body.position.x<x?1:-1;JumpObstacle();HealIfNeeded();yield return new WaitForFixedUpdate();
            }
            host.ExternalAxis=0;host.ExternalJumpHeld=false;yield return new WaitForFixedUpdate();
            double landing=S.Now;while(!host.Grounded){Check(S.Now-landing<3,"landing timed out");yield return new WaitForFixedUpdate();}
        }
        private void JumpObstacle(){
            host.ExternalJumpHeld=S.Now<releaseJumpAt;
            if(!host.Grounded)return;
            var hit=Physics2D.Raycast(host.Body.position+new Vector2(0,-.6f),new Vector2(host.ExternalAxis,0),.95f,1<<6);
            if(hit.collider!=null&&hit.normal.y<.5f&&hit.collider.GetComponent<PlatformEffector2D>()==null){host.ExternalJump=true;host.ExternalJumpHeld=true;releaseJumpAt=S.Now+(hit.collider.bounds.max.y-(host.Body.position.y-.72f)>.7f?.65:.12);}
        }
        private void HealIfNeeded(){if(S.Player.Hp<S.Player.Stats.Hp*.45)S.Potion(true);}
        private void Check(bool ok,string message){if(!ok)throw new InvalidOperationException(message+"; "+S.Objective);}
        private void Ui(string id) {
            // The route navigates the same focus/actions as ↑/↓ and Enter; no mouse or domain bypass.
            if(id.StartsWith("buy.")&&host.Hud.Panel=="npc")Ui("service.buy");
            if(id.StartsWith("sell.")&&host.Hud.Panel=="npc")Ui("service.sell");
            if((id.StartsWith("equip.")||id.StartsWith("learn.")||id.StartsWith("use."))&&host.Hud.Panel=="bag")Ui("item."+id.Substring(id.IndexOf('.')+1));
            var actions=host.Hud.Actions;Check(actions.Any(x=>x.Id==id),"missing UI action "+id);
            for(int i=0;i<actions.Count&&host.Hud.SelectedActionId!=id;i++)host.HandleMenuKey("down");
            Check(host.Hud.SelectedActionId==id,"keyboard selection "+id);host.HandleMenuKey("activate");
        }
        private IEnumerator Talk(string npc,bool accept=false,bool turnIn=false) {
            var a=SliceSession.Anchors.First(x=>x.Id==npc);Check(S.Player.Map==a.Map,"wrong map for "+npc);
            yield return Move(a.Position.X);host.Interact();Check(host.Hud.Panel=="npc","open NPC "+npc);
            if(accept)Ui("quest.accept");if(turnIn)Ui("quest.turnin");host.HandleMenuKey("escape");yield return new WaitForFixedUpdate();
        }
        private IEnumerator Exit(string id) {
            host.Hud.Close();var exit=SliceSession.Anchors.First(x=>x.Id==id&&x.Map==S.Player.Map);var from=S.Player.Map;
            double start=S.Now;
            while(S.Player.Map==from){Check(S.Now-start<35,"auto exit stalled "+id);host.ExternalAxis=Math.Abs(host.Body.position.x-exit.Position.X)<.3?0:host.Body.position.x<exit.Position.X?1:-1;JumpObstacle();HealIfNeeded();yield return new WaitForFixedUpdate();}
            host.ExternalAxis=0;yield return new WaitForFixedUpdate();yield return new WaitForFixedUpdate();
        }
        private IEnumerator Fight(Mob mob) {
            // Authored test-driver route onto the DS5 shelf; not player auto-navigation.
            if(mob.Slot.StartsWith("DS5.")){
                yield return Move(53.4);releaseJumpAt=S.Now+.7;host.ExternalJump=true;host.ExternalJumpHeld=true;
            }
            yield return Move(mob.Position.X+(host.Body.position.x<mob.Position.X?-.8:.8));
            S.Combat.Explicit(mob);int life=mob.Generation;double start=S.Now;
            while(mob.Alive&&mob.Generation==life){
                Check(S.Player.Alive,"player died in combat");Check(S.Now-start<25,"combat stalled");HealIfNeeded();
                if(S.Combat.Running==null&&S.Combat.Remaining(S.Combat.Selected)<=0){host.CombatPress("1",1);host.CombatRelease("1");}
                yield return new WaitForFixedUpdate();
            }
            S.Combat.ClearFocus();while(S.Combat.Running!=null)yield return new WaitForFixedUpdate();
        }
        private void Equip(string id){host.Hud.Toggle("bag");Ui("equip."+S.Player.Inventory.Bag.First(x=>x.Id==id).Instance);host.Hud.Close();}
        private IEnumerator WaitStage(int stage,double timeout=5) {
            double start=S.Now;while(S.Stage!=stage){Check(S.Now-start<timeout,"stage timed out "+stage);yield return new WaitForFixedUpdate();}
        }
        private IEnumerator Route() {
            Check(S.DebugPreset==null,"acceptance must start fresh, not from a debug preset");
            yield return Talk("Lam",accept:true);yield return Talk("Yen");yield return Talk("Bach");yield return Talk("Moc");yield return Talk("Lam",turnIn:true);
            Check(S.Quest==2&&S.Player.Level==2,"Q1 reward");yield return Talk("Lam",accept:true);yield return Exit("toAcademy");yield return Move(31);yield return WaitStage(1);
            yield return Move(8);if(S.Stage<2){host.ExternalJumpHeld=true;host.ExternalJump=true;yield return new WaitForFixedUpdate();}
            double jumpStart=S.Now;while(S.Stage!=2){Check(S.Now-jumpStart<5,"jump ledge not reached by physics");yield return new WaitForFixedUpdate();}
            host.ExternalJumpHeld=false;host.ExternalDrop=true;yield return new WaitForFixedUpdate();yield return new WaitForFixedUpdate();Check(S.Receipts.Contains("Q2.dropped"),"drop input did not ignore platform");yield return WaitStage(3);
            yield return Exit("toVillageA");yield return Talk("Lam",turnIn:true);
            yield return Exit("toAcademy");yield return Talk("Phong",accept:true);Equip("wood");yield return Move(22);yield return WaitStage(2);
            foreach(var dummy in S.Mobs.Where(x=>x.Dummy).ToArray())yield return Fight(dummy);
            Check(S.QuestState==QuestState.Ready,"three concurrent Dummy lives");yield return Talk("Phong",turnIn:true);
            Check(S.Player.Level==3,"Q3 must not raise to Lv4");Equip("pants1");
            yield return Exit("toVillageA");yield return Talk("Bach",accept:true);yield return Exit("toMist");
            var mushroom=S.Mobs.First(x=>x.Slot=="DS2.slot1");while(!mushroom.Alive)yield return new WaitForFixedUpdate();yield return Fight(mushroom);yield return WaitStage(1);
            for(int i=0;i<8&&S.Stage==1;i++){host.Interact();yield return new WaitForFixedUpdate();}
            Check(S.Stage==2,"Q4 tutorial pickup");Equip("armor1");
            yield return Exit("toVillageM");yield return Talk("Bach");host.Hud.OpenNpc("Bach");Ui("sell."+S.Player.Inventory.Bag.First(x=>x.Id=="mushroom"&&x.Binding=="Q4").Instance);host.Hud.Close();yield return Talk("Bach",turnIn:true);
            Check(S.Player.Level==4,"Q4 catch-up Lv4");yield return Talk("Yen",accept:true);host.Hud.OpenNpc("Yen");Ui("buy.food1");Ui("buy.hp1");Ui("buy.hp1");Ui("buy.hp1");host.Hud.Close();Check(S.UseFood(),"Q5 Food");
            yield return Exit("toMist");yield return Move(50);yield return WaitStage(3);
            while(S.QuestState!=QuestState.Ready){
                var mob=S.Mobs.Where(x=>x.Map==Map.Mist&&x.Level==4&&x.Slot.StartsWith("DS")&&x.Alive).OrderBy(x=>x.Position.Distance(S.Player.Position)).FirstOrDefault();
                if(mob==null){yield return new WaitForFixedUpdate();continue;}yield return Fight(mob);
            }
            yield return Exit("toVillageM");yield return Talk("Yen",turnIn:true);
            Check(S.Player.Level==5&&S.Player.ResetAtFive&&S.Player.Unspent==20,"Q5 Lv5 reset");
            yield return Talk("Ta",accept:true);yield return Exit("toAcademy");yield return Move(3);yield return WaitStage(1);
            host.Hud.Toggle("equipment");Ui("slot.Weapon");Ui("unequip.Weapon");host.Hud.Close();
            yield return Move(SliceSession.Anchors.First(x=>x.Id=="Phong").Position.X);host.Interact();Ui("class.sword");host.Hud.Close();Equip("sword1");host.Hud.Toggle("attributes");Ui("allocate.STR");host.Hud.Close();
            host.Hud.Toggle("bag");Ui("learn."+S.Player.Inventory.Bag.First(x=>x.Id=="manual1").Instance);host.Hud.Close();
            yield return Move(21.2);while(!S.Mobs.First(x=>x.Dummy).Alive)yield return new WaitForFixedUpdate();S.Combat.ClearFocus();host.CombatPress("1",1);host.CombatRelease("1");
            yield return WaitStage(4);Check(S.Potion(false),"Q6 reserved MP actual consumption");
            yield return Exit("toVillageA");yield return Talk("Ta",turnIn:true);
            Check(S.Complete&&S.Player.School==School.Sword,"continuous route did not complete");
            Check(Enumerable.Range(1,6).All(i=>S.Receipts.Contains($"Q{i}.completed")),"missing completion receipt");
            S.Emit("VS1 Q1→Q6 PASS — physics, auto EdgeExit, keyboard menu adapter, one-press combat; no debug injection.");
        }
    }
}
