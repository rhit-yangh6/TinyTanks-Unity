
Shader "Dunno/Stripes"
{
    Properties
    {
        _BaseColor ("Base color", Color) = (0.58, 0.91, 0.36, 0.745) 
        _Angle("Line rotation", Range(0, 180)) = 0
        _LineColor ("Line color", Color) = (0.58, 0.91, 0.36, 0.745) 
        _LineSize ("Line size", Range(0.5, 1)) = 0.5 
        _LineIntensity ("Line intensity", float) = 0.5 
        _LineOffset ("Line offset", float) = 0.5 
        _Tiling ("Tiling", float) = 0.5 
        _Speed("Speed", float) = 0.1
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct vertexInput
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct vertexOutput
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            half4 _BaseColor;
            half4 _LineColor;
            float _LineSize;
            float _Angle;
            float _LineIntensity;
            float _LineOffset;
            float _Tiling;
            float _Test;
            float _Speed;

            vertexOutput vert (vertexInput v)
            {
                vertexOutput o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed2 rotateUV(fixed2 uv, fixed2 mid)
            {
                const float angle = _Angle * UNITY_PI / 180;
                
                return fixed2(
                  cos(angle) * (uv.x - mid.x) + sin(angle) * (uv.y - mid.y) + mid.x,
                  cos(angle) * (uv.y - mid.y) - sin(angle) * (uv.x - mid.x) + mid.y
                );
            }

            half4 getLine(float2 uv)
            {
                uv = rotateUV(uv, fixed2(0.5, 0.5)) + float2(_LineOffset, _LineOffset);
                
                const float leftStep = 1 - smoothstep(0.5f, _LineSize, 1 - uv.x);
                const float rightStep = 1 - smoothstep(0.5f, _LineSize, uv.x);

                return leftStep * rightStep * _LineColor * _LineIntensity;
            }
            
            half4 frag (vertexOutput i) : SV_Target
            {
                float offset = _Time.x * _Speed;
                float2 uv = rotateUV(i.uv, fixed2(0.5, 0.5)) + float2(offset, offset);
                
                const float tiledUV = uv.x * _Tiling;
                const float uv_x = floor((frac(tiledUV) + _LineSize));
                const half4 lineColor = uv_x * _LineColor * _LineIntensity;
                
                return lerp(_BaseColor, lineColor, uv_x);
            }
            ENDCG
        }
    }
}