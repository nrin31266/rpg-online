using System.Collections.Generic;
using HuyenLo.Domain;
using UnityEngine;
namespace HuyenLo.Runtime
{
    // One geometric module/pose description drives world sprites and the equipment paper doll.
    // Cosmetic only: no animation event, hitbox, damage or progression authority here.
    public readonly struct RigPart {
        public readonly string Name; public readonly Vector2 Center,Size;public readonly Color Color;public readonly int Layer;public readonly float Angle;
        public RigPart(string name,Vector2 center,Vector2 size,Color color,int layer,float angle=0){Name=name;Center=center;Size=size;Color=color;Layer=layer;Angle=angle;}
    }
    public static class GeometricRig {
        public static IEnumerable<RigPart> Pose(Player player,int facing,double now,float actionPhase,bool attacking,float velocity=0){
            var equipment=player.Inventory.Equipment;
            Color skin=new Color(.83f,.68f,.5f),hair=new Color(.15f,.19f,.23f);
            Color outfit=equipment.ContainsKey(GearSlot.Armor)?new Color(.24f,.64f,.5f):new Color(.51f,.53f,.57f);
            Color lower=equipment.ContainsKey(GearSlot.Pants)?new Color(.2f,.5f,.49f):new Color(.27f,.31f,.38f);
            Color lowerRear=new Color(lower.r*.82f,lower.g*.82f,lower.b*.82f);
            float lean=attacking?facing*.06f:0;
            float speed=Mathf.Min(1,Mathf.Abs(velocity)/4.5f);
            float cycle=Mathf.Sin((float)now*11f)*speed;
            float swing=cycle*14f*facing;
            float frontX=lean+facing*.11f+cycle*.06f*facing;
            float rearX=lean-facing*.11f-cycle*.06f*facing;
            const float legY=-.46f;
            yield return new RigPart("BodyBase/torso",new Vector2(lean,.02f),new Vector2(.43f,.55f),outfit,11);
            yield return new RigPart("BodyBase/neck",new Vector2(lean,.33f),new Vector2(.18f,.12f),skin,11);
            yield return new RigPart("BodyBase/collar",new Vector2(lean,.28f),new Vector2(.28f,.05f),outfit*1.15f,12);
            yield return new RigPart("BodyBase/head",new Vector2(lean,.54f),new Vector2(.34f,.35f),skin,12);
            yield return new RigPart("Hair/cap",new Vector2(lean,.71f),new Vector2(.39f,.15f),hair,13);
            yield return new RigPart("Hair/tail",new Vector2(lean-facing*.18f,.52f),new Vector2(.12f,.33f),hair,10);
            yield return new RigPart("LowerBody/left",new Vector2(facing==1?rearX:frontX,legY),new Vector2(.17f,.44f),facing==1?lowerRear:lower,facing==1?10:11,facing==1?-swing:swing);
            yield return new RigPart("LowerBody/right",new Vector2(facing==1?frontX:rearX,legY),new Vector2(.17f,.44f),facing==1?lower:lowerRear,facing==1?11:10,facing==1?swing:-swing);
            yield return new RigPart("Armor/belt",new Vector2(lean,-.22f),new Vector2(.48f,.08f),new Color(.44f,.32f,.23f),12);
            if(equipment.ContainsKey(GearSlot.Armor))yield return new RigPart("Armor/shoulder",new Vector2(lean,.21f),new Vector2(.59f,.12f),new Color(.35f,.74f,.57f),13);
            yield return new RigPart("BodyBase/rear arm",new Vector2(lean-facing*.26f,.04f),new Vector2(.13f,.37f),outfit,10);
            var hand=new Vector2(lean+facing*.28f,-.05f);
            yield return new RigPart("BodyBase/hand",hand,new Vector2(.16f,.16f),skin,14);
            if(equipment.TryGetValue(GearSlot.Weapon,out var weapon)){
                float angle=attacking?facing*Mathf.Sin(actionPhase*Mathf.PI)*-95:facing*-12;
                float radians=angle*Mathf.Deg2Rad;var along=new Vector2(-Mathf.Sin(radians),Mathf.Cos(radians));
                bool sword=weapon.Id=="sword1";
                yield return new RigPart("Weapon/blade",hand+along*.39f,new Vector2(sword?.10f:.14f,sword?.93f:.77f),sword?new Color(.75f,.87f,.86f):new Color(.58f,.37f,.2f),15,angle);
                yield return new RigPart("Weapon/guard",hand+along*.04f,new Vector2(.30f,.065f),new Color(.65f,.49f,.25f),16,angle);
            }
        }
    }
}
