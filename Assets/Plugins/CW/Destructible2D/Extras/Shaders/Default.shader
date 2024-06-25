Shader "Destructible 2D/Default"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0

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
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex SpriteVert_d2d
			#pragma fragment SpriteFrag_d2d
			#pragma target 2.0
			#pragma multi_compile_instancing
			#pragma multi_compile_local PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnitySprites.cginc"

			// D2D
			#pragma shader_feature_local _D2DOUTPUTRGB_ORIGINAL _D2DOUTPUTRGB_DESTRUCTIBLE _D2DOUTPUTRGB_COMBINED
			#pragma shader_feature_local _D2DOUTPUTALPHA_DESTRUCTIBLE _D2DOUTPUTALPHA_COMBINED
			#pragma shader_feature_local _ _D2D_OUTLINE
			#include "Destructible2D.cginc"

			struct v2f_d2d
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO

				// D2D
				float2 localPos : TEXCOORD1;
			};

			v2f_d2d SpriteVert_d2d(appdata_t IN)
			{
				v2f_d2d OUT;

				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

				OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
				OUT.vertex = UnityObjectToClipPos(OUT.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color * _RendererColor;

#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif

				// D2D
				OUT.localPos = IN.vertex.xy;

				return OUT;
			}

			fixed4 SpriteFrag_d2d(v2f_d2d IN) : SV_Target
			{
				float4 c = SampleSpriteTexture(IN.texcoord);

				// D2D
				D2dModifyColorAndAlpha(c.rgb, c.a, IN.localPos);

				c *= IN.color;

				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
}