using System.Collections.Generic;
namespace HuyenLo.Domain
{
    // Disposable blockout authoring. Foreground solid earth, rear earth with a one-way top, and wooden decks are distinct.
    public readonly struct Surface
    {
        public readonly string Name;
        public readonly double X,Y,Width,Height,Rise;
        public readonly bool OneWay,Overhead,Wood,RearEarth;
        public Surface(string name,double x,double y,double width,double height,bool oneWay=false,bool overhead=false,bool wood=false,bool rearEarth=false,double rise=0)
        {Name=name;X=x;Y=y;Width=width;Height=height;Rise=rise;OneWay=oneWay;Overhead=overhead;RearEarth=rearEarth;Wood=wood||(oneWay&&!rearEarth);}
        public double TopAt(double x)=>Y+Height/2-System.Math.Max(0,Rise)+Rise*(x-(X-Width/2))/Width;
    }
    public readonly struct WaterRegion
    {
        public readonly double Left,Right,Bottom,Level,SpeedFactor;
        public readonly bool DecorativeOnly;
        public WaterRegion(double left,double right,double bottom,double level,double speedFactor=.85,bool decorativeOnly=false)
        {Left=left;Right=right;Bottom=bottom;Level=level;SpeedFactor=speedFactor;DecorativeOnly=decorativeOnly;}
        public bool TouchesFeet(Point feet)=>!DecorativeOnly&&feet.X>Left&&feet.X<Right&&feet.Y>=Bottom-.08&&feet.Y<Level;
    }
    public static class BlockoutLayout
    {
        public static double MinX(Map map)=>map==Map.Academy?-30:-10;
        public static double MaxX(Map map)=>map==Map.Mist?132:40;
        private static Surface Earth(string name,double left,double right,double top)=>new Surface(name,(left+right)/2,(top-8)/2,right-left,top+8);
        private static Surface RearEarth(string name,double left,double right,double top)=>new Surface(name,(left+right)/2,(top-8)/2,right-left,top+8,true,true,false,true);
        public static IEnumerable<Surface> Surfaces(Map map)
        {
            if(map==Map.Village){
                yield return Earth("West garden terrace",-10,-8,2);
                yield return Earth("Garden descent",-8,-7,1);
                yield return Earth("Garden low step",-7,-6,.5);
                yield return Earth("Village square west",-6,2.4,0);
                yield return Earth("Village shallow basin",2.4,4,-.35);
                yield return Earth("Village square east",4,8,0);
                yield return Earth("Market step",8,9,.3);
                yield return Earth("Forge terrace",9,17,.6);
                yield return Earth("Inn step",17,18,.3);
                yield return Earth("Inn courtyard",18,21,0);
                yield return Earth("Ritual foothill",21,22,1.2);
                yield return Earth("Ritual upper step",22,23,2.4);
                yield return Earth("Ritual hill",23,27,3.6);
                yield return Earth("Ritual descent high",27,28,2.4);
                yield return Earth("Ritual descent low",28,29,1.2);
                yield return Earth("Eastern gate road",29,32,0);
                yield return Earth("East lookout",32,40,1);
                yield return new Surface("Wooden forge gallery",13,3,5,.25,true);
                yield return new Surface("Elder house upper floor",0,2.875,5,.25,true);
            }else if(map==Map.Academy){
                yield return Earth("West dojo terrace",-30,-24,4);
                yield return Earth("Dojo upper shoulder",-24,-22,4.4);
                yield return Earth("Dojo ridge step",-22,-20,5.4);
                yield return Earth("Dojo ridge",-20,-18,6.4);
                yield return Earth("Class courtyard",-18,-8,0);
                yield return Earth("Class entrance low step",-6,-5,1.4);
                yield return Earth("Class entrance middle step",-7,-6,2.8);
                yield return Earth("Class entrance high step",-8,-7,4.2);
                yield return Earth("Class main hall route",-5,12,0);
                yield return RearEarth("Bow hill walkable cap",-18,-10,6.4);
                yield return Earth("Practice approach step",12,14,.4);
                yield return Earth("Practice terrace",14,17,.8);
                yield return Earth("Practice descent",17,19,.4);
                yield return Earth("Dummy yard and gate",19,40,0);
                yield return new Surface("HV_JumpLedge wooden deck",8,2.8,4,.4,true);
                yield return new Surface("West dojo gallery",-27,6.1,5,.25,true);
                yield return new Surface("Bow hall upper floor",-12,9.075,6,.25,true);
            }else {
                yield return Earth("Mist entrance and mushrooms",-10,18,0);
                yield return Earth("DS3 low stair",18,20,1.2);
                yield return Earth("DS3 upper stair",20,22,2.4);
                yield return Earth("DS3 solid hill",22,38,3.6);
                yield return Earth("DS4 upper descent",38,41,2.4);
                yield return Earth("DS4 low descent",41,44,1.2);
                yield return Earth("DS4 valley entry",44,46,0);
                yield return Earth("DS4 jumping approach",46,48,1.2);
                yield return Earth("DS4 takeoff plateau",48,55,2.4);
                yield return Earth("DS5 lane descent",55,57,1.8);
                yield return Earth("DS5 lower passage",57,66,1.2);
                yield return RearEarth("DS5 walkable earth overhang",54,64,4.6);
                yield return RearEarth("Stream rear cliff",64,66,3.4);
                // Wide water is inaccessible scenery: a solid wooden bridge is the only route.
                yield return new Surface("Valley solid wooden bridge",74,1.05,16,.3,false,false,true);
                yield return new Surface("DS6 sloped bridge approach",84,-3.4,4,9.2,false,false,false,false,-1.2);
                yield return Earth("DS6 clearing",86,100,0);
                yield return Earth("Eastern ridge step",100,104,.4);
                yield return Earth("PROBE7 ridge",104,122,.8);
                yield return new Surface("PROBE8 timber climbing step",107,2.4,2,.2,true);
                yield return RearEarth("PROBE8 earth lookout",109,119,4.6);
                yield return RearEarth("PROBE8 climbing ledge",119,122,3.2);
                yield return Earth("Eastern climbing shoulder",122,124,2);
                yield return Earth("Eastern road",124,132,0);
            }
        }
        // Feet contact, not XY overlap: bridge/air actors must remain dry. No water physics.
        public static IEnumerable<WaterRegion> Waters(Map map){
            if(map==Map.Village)yield return new WaterRegion(2.4,4,-.35,0);
            if(map==Map.Mist){
                yield return new WaterRegion(66,82,-8,-1.2,1,true);
            }
        }
        public static double WaterSpeed(Map map,Point feet){
            double factor=1;foreach(var w in Waters(map))if(w.TouchesFeet(feet))factor=System.Math.Min(factor,w.SpeedFactor);return factor;
        }
        public static double BaseGroundTop(Map map,double x){
            double top=-4;foreach(var s in Surfaces(map))if(!s.OneWay&&!s.Overhead&&x>=s.X-s.Width/2&&x<=s.X+s.Width/2)top=System.Math.Max(top,s.TopAt(x));return top;
        }
        public static Surface MobSupport(string slot,double x){
            string roof=slot.StartsWith("DS5.")?"DS5 walkable earth overhang":slot.StartsWith("PROBE8.")?"PROBE8 earth lookout":null;
            Surface best=default;double top=-100;
            foreach(var s in Surfaces(Map.Mist))if((!s.OneWay||s.RearEarth)&&x>s.X-s.Width/2&&x<s.X+s.Width/2&&(roof!=null?s.Name==roof:!s.Overhead)&&s.Y+s.Height/2>top){best=s;top=s.Y+s.Height/2;}
            if(top==-100)throw new System.InvalidOperationException("Missing authored support for "+slot);return best;
        }
        public static double GroundTop(Map map,double x){
            double top=-4;
            foreach(var s in Surfaces(map))if((!s.OneWay||s.RearEarth)&&x>=s.X-s.Width/2&&x<=s.X+s.Width/2)top=System.Math.Max(top,s.TopAt(x));
            return top;
        }
    }
}
