Shader "Destructible 2D/Diffuse"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		
		// D2D
		[PerRendererData] _D2dAlpha("D2D Alpha", 2D) = "white" {}
		[PerRendererData] _D2dScale("D2D Scale", Vector) = (1,1,0,0)
		[PerRendererData] _D2dOffset("D2D Offset", Vector) = (0,0,0,0)
		[PerRendererData] _D2dSharpness("D2D Sharpness", Float) = 1.0
		[Header(DESTRUCTIBLE 2D)]
		[KeywordEnum(Original, Destructible, Combined)] _D2dOutputRgb("	Output RGB", Float) = 0
		[KeywordEnum(Destructible, Combined)] _D2dOutputAlpha("	Output Alpha", Float) = 0
		[Toggle(_D2D_OUTLINE)] _D2dOutline("	Outline", Float) = 0
		_D2dOutlineColor("	Outline Color", Color) = (0,0,0,1)
		_D2dOutlineMin("	Outline Min", Float) = 0.0
		_D2dOutlineMax("	Outline Max", Float) = 1.0
		_D2dOutlinePower("	Outline Power", Float) = 10.0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		CGPROGRAM
		#pragma surface surf Lambert vertex:vert nofog keepalpha
		#pragma multi_compile_local PIXELSNAP_ON

		// D2D
		#pragma shader_feature_local _D2DOUTPUTRGB_ORIGINAL _D2DOUTPUTRGB_DESTRUCTIBLE _D2DOUTPUTRGB_COMBINED
		#pragma shader_feature_local _D2DOUTPUTALPHA_DESTRUCTIBLE _D2DOUTPUTALPHA_COMBINED
		#pragma shader_feature_local _ _D2D_OUTLINE
		#include "Destructible2D.cginc"

		sampler2D _MainTex;
		float4    _Color;

		struct Input
		{
			float2 uv_MainTex;
			fixed4 color;
			// D2D
			float2 localPos;
		};
		
		void vert (inout appdata_full v, out Input o)
		{
			#if defined(PIXELSNAP_ON) && !defined(SHADER_API_FLASH)
			v.vertex = UnityPixelSnap (v.vertex);
			#endif
			v.normal = float3(0,0,-1);
			
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.color = v.color * _Color;
			
			// D2D
			o.localPos = v.vertex.xy;
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			float4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;

			// D2D
			D2dModifyColorAndAlpha(c.rgb, c.a, IN.localPos);

			o.Albedo = c.rgb * c.a;
			o.Alpha = c.a;
		}
		ENDCG
	}
	
	Fallback "Transparent/VertexLit"
}