
Shader "Unlit/PolygonSDF"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Offset("Offset", Vector) = (1,1,1,1)
        _Scale("Scale", float) = 1
        _Distance("Distance", float) = 1
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            StructuredBuffer<float2> _Vertices;
            int _VerticesCount;
            float4 _Offset;
            float4 _MinMax;
            float _Scale;
            float _SdfFactor;
            
            float sdPolygon(in float2 p)
            {
                float d = dot(p-_Vertices[0],p-_Vertices[0]);
                float s = 1.0;
                
                for(int i=0, j= _VerticesCount - 1; i< _VerticesCount; j = i, i++)
                {
                    float2 e = _Vertices[j] - _Vertices[i];
                    float2 w =    p - _Vertices[i];
                    float2 b = w - e*clamp( dot(w,e)/dot(e,e), 0.0, 1.0 );
                    d = min( d, dot(b,b) );
                    
                    bool3 cond = bool3( p.y>=_Vertices[i].y, 
                                        p.y <_Vertices[j].y, 
                                        e.x*w.y>e.y*w.x );
                    
                    if( all(cond) || all(!cond) ) s=-s;  
                }
                
                return s*sqrt(d);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                const float2 offset = -1 / float2(_SdfFactor, _SdfFactor);
                
                const float2 minPoint = float2(_MinMax.x, _MinMax.y) + offset;
                const float2 maxPoint = float2(_MinMax.z, _MinMax.w) - offset;
                const float2 magnitude = maxPoint - minPoint;
                
                const float2 uv = i.uv * magnitude + minPoint;
                const float distance = sdPolygon(uv);

                float3 col = distance > 0.0 ? float3(0.9,0.6,0.3) : float3(1,1,1);
	            col *= 1.0 - exp(-_SdfFactor * abs(distance));

                float resultColor = 1 - (distance * _SdfFactor);
                
                return half4(resultColor.xxxx);
            }
            ENDCG
        }
    }
}
