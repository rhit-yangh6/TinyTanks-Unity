
Shader "Unlit/Water"
{
    Properties
    {
        _BottomColor ("Bottom color", Color) = (1,1,1,1)
        _TopColor ("Top color", Color) = (1,1,1,1)
        _WaveFill("Waves fill", Range(0, 10)) = 0.1
        _Shore("Shore", Range(0, 10)) = 0.1
        _Amplitude("Amplitude", Range(0, 10)) = 0.1
        _Length("Length", Range(0, 20)) = 0.1
        _NoiseStrength("NoiseStrength", Range(0, 20)) = 0.1
        _NoiseTexture ("Noise texture", 2D) = "" {}
        _WaveSpeed("Wave speed", float) = 0.1
        _NoiseSpeed("Noise speed", float) = 0.1
        _StartOffset("StartOffset", float) = 0
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma enable_d3d11_debug_symbols
            
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

            half4 _BottomColor;
            half4 _TopColor;
            float _WaveFill;
            float _Amplitude;
            float _Length;
            
            float _RotationAngle;
            float _StartOffset;
            float _WaveSpeed;
            float _NoiseSpeed;
            float _Shore;
            float _NoiseStrength;
            float2 _MinPoint;
            float2 _MaxPoint;
            float2 _Scale;
            float2 _Position;
            float2 _RectangleSize; // in uniform values 
            sampler2D _NoiseTexture;  

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

            float2 getWorldPosition(const float2 uv)
            {
                const float2 localPosition = (uv - 0.5) * _Scale;
                const float2 rotatedLocalPosition = rotate(localPosition, _RotationAngle);
                const float2 worldPosition = rotatedLocalPosition + _Position;

                return worldPosition;
            }

            float invLerp(float from, float to, float value)
            {
                return (value - from) / (to - from);
            }

            float getAlpha(float height)
            {
                return height < (_MaxPoint.y - _WaveFill);
            }
            
            float getOffset(float2 uv)
            {
                const float length = _Length * _RectangleSize.x;
                const float sinValue = sin(uv.x * length + _Time.x * _WaveSpeed) * _Amplitude;
                const float noise = tex2D(_NoiseTexture, _StartOffset + uv + _Time.x * _NoiseSpeed) * _NoiseStrength;
                
                return sinValue + noise;
            }

            half4 getResultColor(float2 worldPosition)
            {
                const float max = _MaxPoint.y - _WaveFill;
                const float min = max - _Shore;
                const float waveLerp = saturate(invLerp(min, max, worldPosition.y));
                
                half4 resultColor = lerp(_BottomColor, _TopColor, waveLerp);

                return resultColor;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float2 worldPosition = getWorldPosition(i.uv);
                worldPosition.y += getOffset(i.uv);

                half4 resultColor = getResultColor(worldPosition);
                resultColor.a = getAlpha(worldPosition.y);
                
                return resultColor;
            }
            ENDCG
        }
    }
}
