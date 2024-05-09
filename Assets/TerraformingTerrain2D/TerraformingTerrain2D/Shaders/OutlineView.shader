
Shader "OutlineView"
{
    Properties
    {
        _Mask ("Texture", 2D) = "black" {}
        _MainColor("Main color", Color) = (1,1,1,1)
        
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            Stencil
            {
                Ref 123
                Comp equal
            }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _Mask;
            half4 _MainColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half r = tex2D(_Mask, i.uv).r;
                
                return half4(r.xxxx) * _MainColor;
            }
            ENDCG
        }
    }
}
