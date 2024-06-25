sampler2D _D2dAlpha;
float     _D2dSharpness;
float2    _D2dScale;
float2    _D2dOffset;
float3    _D2dOutlineColor;
float     _D2dOutlineMin;
float     _D2dOutlineMax;
float     _D2dOutlinePower;

// This function modifies the final color and alpha values based on the specified local-space XY position.
// The modifications performed depend on the material settings.
void D2dModifyColorAndAlpha(inout float3 c, inout float a, float2 localXY)
{
	float4 d2d = tex2D(_D2dAlpha, (localXY - _D2dOffset) / _D2dScale);
	
#if _D2DOUTPUTRGB_ORIGINAL
	// Do nothing
#elif _D2DOUTPUTRGB_DESTRUCTIBLE
	c = d2d.rgb;
#elif _D2DOUTPUTRGB_COMBINED
	c *= d2d.rgb;
#endif
	
#if _D2DOUTPUTALPHA_DESTRUCTIBLE
	a = saturate(0.5f + (d2d.a - 0.5f) * _D2dSharpness); 
#elif _D2DOUTPUTALPHA_COMBINED
	a *= saturate(0.5f + (d2d.a - 0.5f) * _D2dSharpness);
#endif
	
#if _D2D_OUTLINE
	c = lerp(_D2dOutlineColor, c, pow(saturate((d2d.a - _D2dOutlineMin) / (_D2dOutlineMax - _D2dOutlineMin)), _D2dOutlinePower));
#endif
}