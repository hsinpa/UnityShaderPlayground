using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Hsinpa.Algorithm
{
    public class QuadTreeUti
    {
    
        public struct QuadRect {
            public float x;
            public float y;
            public float2 extends;

            public float2 size => extends * 2;
            public float2 minPoints => new float2(x - extends.x, y - extends.y);
            public float2 maxPoints => new float2(x + extends.x, y + extends.y);

            public QuadRect(float p_x, float p_y, float2 p_extends) {
                this.x = p_x;
                this.y = p_y;
                this.extends = p_extends;
            }

            public bool intersect() {
                return false;
            }
        }

    }
}