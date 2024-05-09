
Shader "Dunno/Star"
{
    Properties
    {
        _MainTex("Main texture", 2D) = "white" {}
        _Color ("Fill color", Color) = (0.58, 0.91, 0.36, 0.745) 
        _Fill ("Fill", Range(0, 1)) = 0.1 
        _Angle("Line rotation", Range(0, 180)) = 0
        _LineColor ("Line color", Color) = (0.58, 0.91, 0.36, 0.745) 
        _LineSize ("Line size", Range(0.5, 1)) = 0.5 
        _LineIntensity ("Line intensity", float) = 0.5 
        _LineOffset ("Line offset", float) = 0.5 
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
            
            half4 _Color;
            half4 _LineColor;
            sampler2D _MainTex;
            float _Fill;
            float _LineSize;
            float _Angle;
            float _LineIntensity;
            float _LineOffset;

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

            half3 getLine(float2 uv)
            {
                float offset = _Time.x * _LineOffset % 2 - 1;
                uv = rotateUV(uv, fixed2(0.5, 0.5)) + float2(offset, offset);
                const half4 lineColor = (1 - smoothstep(0.5f, _LineSize, 1 - uv.x)) * (1 - smoothstep(0.5f, _LineSize, uv.x)) * _LineColor;

                return lineColor * _LineIntensity;
            }
            
            half4 frag (vertexOutput i) : SV_Target
            {
                half4 textureColor = tex2D(_MainTex, i.uv);
                const half3 lineColor = getLine(i.uv);
                const half4 baseColor = half4(textureColor + lineColor, textureColor.a);
                
                half4 resultColor = i.uv.y > _Fill ? textureColor : _Color;
                resultColor.a = _Color.a * textureColor.a;
                
                return resultColor;
            }
            ENDCG
        }
    }
}