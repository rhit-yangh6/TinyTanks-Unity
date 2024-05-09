
Shader "Dunno/SwampyWater"
{
    Properties
    {
        _WaterTexture("Water texture", 2D) = "white" {}
        _Color ("Color", Color) = (0.58, 0.91, 0.36, 0.745) 
        _Feather ("Feather", Range(0, 1)) = 0.02
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
            
            sampler2D _WaterTexture;
            float _Feather;
            half4 _Color;

            vertexOutput vert (vertexInput v)
            {
                vertexOutput o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            half4 frag (vertexOutput i) : SV_Target
            {
                const half4 alpha = tex2D(_WaterTexture, i.uv);
                half4 resultColor = _Color;
                resultColor.a = (1 - step(alpha, _Feather)) * _Color.a;
                
                return resultColor; 
            }
            ENDCG
        }
    }
}