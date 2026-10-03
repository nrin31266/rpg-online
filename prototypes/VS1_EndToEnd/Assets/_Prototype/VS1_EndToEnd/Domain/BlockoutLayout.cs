using System.Collections.Generic;
namespace HuyenLo.Domain
{
    // Authoring data for this disposable mock. No Unity/presentation dependency.
    public readonly struct Surface
    {
        public readonly string Name;
        public readonly double X,Y,Width,Height;
        public readonly bool OneWay;
        public Surface(string name,double x,double y,double width,double height,bool oneWay=false)
        {Name=name;X=x;Y=y;Width=width;Height=height;OneWay=oneWay;}
    }
    public static class BlockoutLayout
    {
        public static double MinX(Map map)=>map==Map.Academy?-30:-10;
        public static double MaxX(Map map)=>map==Map.Mist?132:40;
        public static IEnumerable<Surface> Surfaces(Map map)
        {
            double min=MinX(map),max=MaxX(map);
            // Main walk route remains continuous; optional vertical routes have real supports.
            yield return new Surface("Main terrain",(min+max)/2,-1.5,max-min,3);
            if(map==Map.Village){
                yield return new Surface("Market awning",8,2.5,10,.3,true);
                yield return new Surface("Forge balcony",19,3,6,.3,true);
                yield return new Surface("Gate terrace",27,2,4,.3,true);
            }else if(map==Map.Academy){
                yield return new Surface("HV_JumpLedge",8,2.8,4,.4,true);
                yield return new Surface("Dojo gallery",1,4.8,6,.35,true);
                yield return new Surface("Practice balcony",20,3.6,9,.35,true);
                yield return new Surface("Step to gallery",-6,2.4,3,.3,true);
                yield return new Surface("West lookout",-18,4.5,11,.4,true);
            }else {
                yield return new Surface("DS2 log",20,1.2,4,.3,true);
                yield return new Surface("DS3 approach ledge",35,1.8,4,.3,true);
                yield return new Surface("DS upper path A",43,3.2,9,.4,true);
                yield return new Surface("PROBE8 upper lane",54,3.2,10,.4,true);
                yield return new Surface("DS4 descending ledge",63,1.8,4,.3,true);
                yield return new Surface("DS5 bridge",75,2.8,10,.4,true);
                yield return new Surface("DS6 lookout step",85,1.5,4,.3,true);
                yield return new Surface("DS6 lookout",93,3.5,8,.4,true);
                yield return new Surface("Eastern ridge",116,2.3,6,.4,true);
            }
        }
    }
}
