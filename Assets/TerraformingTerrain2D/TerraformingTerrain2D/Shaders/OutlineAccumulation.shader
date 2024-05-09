
Shader "OutlineAccumulation"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
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
            float2 _RectangleSize;
            float _RotationAngle;
            float4 _MouseData;
            float2 _MaxPoint;
            float2 _MinPoint;
            float2 _Position;
            float2 _Scale;
            int _CarveSign;

            float2 rotate(float2 localPosition, float angle)
            {
                const float cosTheta = cos(angle);
                const float sinTheta = sin(angle);
                
                return float2(localPosition.x * cosTheta - localPosition.y * sinTheta,localPosition.x * sinTheta + localPosition.y * cosTheta);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                const float2 localPosition = (i.uv - 0.5) * _RectangleSize;
                const float2 rotatedLocalPosition = rotate(localPosition, _RotationAngle);
                const float2 scaledPosition = float2(rotatedLocalPosition.x * _Scale.x, rotatedLocalPosition.y * _Scale.y);
                const float2 worldPosition = scaledPosition + _Position;

                const float2 mousePosition = float2(_MouseData.x, _MouseData.y);
                const float radius = _MouseData.z;
                
                const half4 distance = (length(worldPosition - mousePosition) <= radius) * _CarveSign;
                
                return saturate(tex2D(_Mask, i.uv) + distance);
            }
            ENDCG
        }
    }
}
