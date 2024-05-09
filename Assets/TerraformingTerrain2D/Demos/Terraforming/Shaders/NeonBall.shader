
Shader "Dunno/NeonBall"
{
    Properties
    {
        _OutlineSize("Outline size", Range(0, 1)) = 0.5
        _Feather("Feather", float) = 0.5
        [HDR] _OutlineColor ("Outline color", Color) = (0.58, 0.91, 0.36, 0.745)
        _InnerColor ("Inner color", Color) = (0.58, 0.91, 0.36, 0.745) 
        _OuterColor ("Outer color", Color) = (0.58, 0.91, 0.36, 0.745) 
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

            float _OutlineSize;
            half4 _OutlineColor;
            half4 _InnerColor;
            half4 _OuterColor;
            half4 _Feather;

            vertexOutput vert (vertexInput v)
            {
                vertexOutput o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * 2 - 1;
                return o;
            }

            float4 invLerp(float4 from, float4 to, float4 value)
            {
                return (value - from) / (to - from);
            }
            
            half4 getInnerColor(float distance)
            {
                const float lerpValue = invLerp(0, 1 - _OutlineSize, distance);
                return lerp(_InnerColor, _OuterColor, lerpValue);
            }
            
            half4 frag (vertexOutput i) : SV_Target
            {
                const float distance = length(i.uv);
                const half4 baseColor = distance > 1 - _OutlineSize ? _OutlineColor : getInnerColor(distance);
                const int isCircle = step(distance, 1);
                
                return baseColor * isCircle;;
            }
            ENDCG
        }
    }
}