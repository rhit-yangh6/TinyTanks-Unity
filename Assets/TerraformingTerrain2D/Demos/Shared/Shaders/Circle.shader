
Shader "Dunno/Circle"
{
    Properties
    {
        _Color ("Color", Color) = (0.58, 0.91, 0.36, 0.745) 
        _Feather ("Feather", Range(0, 0.1)) = 0.02
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
            
            static float _Radius = 0.25f;
            static float _CentreX = 0.5f;
            static float _CentreY = 0.5f;
            float _Feather;
            half4 _Color;

            vertexOutput vert (vertexInput v)
            {
                vertexOutput o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float drawCircle(float2 uv)
            {
                const float distance = pow(uv.x - _CentreX, 2) + pow(uv.y - _CentreY, 2);
                
                return smoothstep(_Radius, _Radius - _Feather, distance);
            }
            
            half4 frag (vertexOutput i) : SV_Target
            {
                const half4 transparent = half4(_Color.x, _Color.y, _Color.z, 0);
                
                half4 final_color = lerp(transparent, _Color, drawCircle(i.uv));
                
                return final_color;
            }
            ENDCG
        }
    }
}