using System.Collections.Generic;
namespace HuyenLo.Domain
{
    // Disposable blockout authoring. Solid terrace profiles are distinct from wooden one-way decks.
    public readonly struct Surface
    {
        public readonly string Name;
        public readonly double X,Y,Width,Height;
        public readonly bool OneWay;
        public Surface(string name,double x,double y,double width,double height,bool oneWay=false)
        {Name=name;X=x;Y=y;Width=width;Height=height;OneWay=oneWay;}
    }
    public readonly struct WaterRegion
    {
        public readonly double Left,Right,Bottom,Level,SpeedFactor;
        public WaterRegion(double left,double right,double bottom,double level,double speedFactor=.85)
        {Left=left;Right=right;Bottom=bottom;Level=level;SpeedFactor=speedFactor;}
        public bool TouchesFeet(Point feet)=>feet.X>Left&&feet.X<Right&&feet.Y>=Bottom-.08&&feet.Y<Level;
    }
    public static class BlockoutLayout
    {
        public static double MinX(Map map)=>map==Map.Academy?-30:-10;
        public static double MaxX(Map map)=>map==Map.Mist?132:40;
        private static Surface Earth(string name,double left,double right,double top)=>new Surface(name,(left+right)/2,(top-8)/2,right-left,top+8);
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
                yield return Earth("Ritual courtyard",18,32,0);
                yield return Earth("East lookout",32,40,1);
                yield return new Surface("Wooden market gallery",11,3,6,.25,true);
            }else if(map==Map.Academy){
                yield return Earth("West dojo terrace",-30,-24,4);
                yield return Earth("Dojo upper stair",-24,-22,3);
                yield return Earth("Dojo middle stair",-22,-20,2);
                yield return Earth("Dojo low stair",-20,-18,1);
                yield return Earth("Class courtyard",-18,12,0);
                yield return Earth("Practice approach step",12,14,.4);
                yield return Earth("Practice terrace",14,17,.8);
                yield return Earth("Practice descent",17,19,.4);
                yield return Earth("Dummy yard and gate",19,40,0);
                yield return new Surface("HV_JumpLedge wooden deck",8,2.8,4,.4,true);
                yield return new Surface("West dojo gallery",-27,6.1,5,.25,true);
            }else {
                yield return Earth("Mist entrance and mushrooms",-10,18,0);
                yield return Earth("DS3 low stair",18,20,.6);
                yield return Earth("DS3 upper stair",20,22,1.2);
                yield return Earth("DS3 solid terrace",22,38,1.8);
                yield return Earth("DS4 upper descent",38,41,1.2);
                yield return Earth("DS4 low descent",41,44,.6);
                yield return Earth("DS4 valley",44,60,0);
                yield return Earth("Stream upper bank",60,62,-.5);
                yield return Earth("Stream low bank",62,64,-1.25);
                yield return Earth("DS5 walkable basin",64,80,-2);
                yield return Earth("DS6 lower stair",80,82,-1.25);
                yield return Earth("DS6 upper stair",82,86,-.5);
                yield return Earth("DS6 clearing",86,100,0);
                yield return Earth("Eastern ridge step",100,104,.4);
                yield return Earth("PROBE7 ridge",104,122,.8);
                yield return Earth("Eastern road",122,132,0);
                yield return new Surface("PROBE8 wooden upper bridge",54,3.2,10,.4,true);
                yield return new Surface("Valley wooden footbridge",73,-.025,22,.25,true);
            }
        }
        // Feet contact, not XY overlap: bridge/air actors must remain dry. No water physics.
        public static IEnumerable<WaterRegion> Waters(Map map){
            if(map==Map.Village)yield return new WaterRegion(2.4,4,-.35,0);
            if(map==Map.Mist){
                yield return new WaterRegion(64,69,-2,-1.55);
                yield return new WaterRegion(71,80,-2,-1.45);
            }
        }
        public static double WaterSpeed(Map map,Point feet){
            double factor=1;foreach(var w in Waters(map))if(w.TouchesFeet(feet))factor=System.Math.Min(factor,w.SpeedFactor);return factor;
        }
        public static double GroundTop(Map map,double x){
            double top=-4;
            foreach(var s in Surfaces(map))if(!s.OneWay&&x>=s.X-s.Width/2&&x<=s.X+s.Width/2)top=System.Math.Max(top,s.Y+s.Height/2);
            return top;
        }
    }
}
