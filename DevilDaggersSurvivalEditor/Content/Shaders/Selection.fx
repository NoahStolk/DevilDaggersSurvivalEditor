sampler2D implicitInputSampler : register(S0);
sampler2D normalMapSampler : register(S1);

float flashIntensity : register(C0);
float4 highlightColor : register(C1);
float highlightRadiusSquared : register(C2);
float2 mousePosition : register(C3);

float4 invertRGB(float4 value)
{
    return float4(1 - value.r, 1 - value.g, 1 - value.b, value.a);
}

bool isEqualTo(float4 left, float4 right)
{
    return left.x == right.x
		&& left.y == right.y
		&& left.z == right.z
		&& left.w == right.w;
}

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 diffuseColor = tex2D(implicitInputSampler, uv);
    if (!isEqualTo(diffuseColor, float4(0, 0, 0, 1)))
    {
        float distanceSquared = pow(uv.x - mousePosition.x, 2) + pow(uv.y - mousePosition.y, 2);
        if (distanceSquared < highlightRadiusSquared)
        {
            float highlightPercentage = saturate(1 - (distanceSquared / highlightRadiusSquared)) * highlightColor.w;
            diffuseColor += highlightColor * highlightPercentage;
        }
    }

    float4 normalMapColor = tex2D(normalMapSampler, uv);
    if (normalMapColor.r == 0)
        return diffuseColor;
	
    float4 normalLeft = tex2D(normalMapSampler, uv - float2(1 / 408.0, 0));
    float4 normalUp = tex2D(normalMapSampler, uv - float2(0, 1 / 408.0));
    float4 normalRight = tex2D(normalMapSampler, uv + float2(1 / 408.0, 0));
    float4 normalDown = tex2D(normalMapSampler, uv + float2(0, 1 / 408.0));

    bool isBorder = isEqualTo(normalLeft, float4(0, 0, 0, 1))
				 || isEqualTo(normalUp, float4(0, 0, 0, 1))
				 || isEqualTo(normalRight, float4(0, 0, 0, 1))
				 || isEqualTo(normalDown, float4(0, 0, 0, 1));
	
    return (isBorder ? invertRGB(diffuseColor) : diffuseColor + float4(0.2, 0.2, 0.2, 0)) * (flashIntensity + 1);
}