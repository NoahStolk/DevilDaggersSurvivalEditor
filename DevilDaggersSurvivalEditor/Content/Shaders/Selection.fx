sampler2D implicitInputSampler : register(S0);
sampler2D normalMapSampler : register(S1);

float diffuseIntensity : register(C0);
float ambientIntensity : register(C1);
float4 ambientColor : register(C2);
float3 lightDirection : register(C3);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 normal = (tex2D(normalMapSampler, uv) + float4(0.5, 0.5, 0.5, 0)) / 4;
    float4 light = float4(normalize(lightDirection), 1);
	
    float4 diffuseColor = tex2D(implicitInputSampler, uv);
    float4 diffuse = tex2D(normalMapSampler, uv).r > 0 ? diffuseIntensity * diffuseColor * dot(normal, light) : diffuseColor;

    return ambientIntensity * ambientColor + diffuse;
}