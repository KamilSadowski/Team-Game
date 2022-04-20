
void SubstituteColor(float4 Col, float _Cutoff, float4 _PrimaryColor, float4 _SecondaryColor, float4 _TertiaryColor, out float4 Out)
{
    float4 c = Col;
    if (c.r < 0.3f && c.g > 0.25f && c.b < 0.3f)
    {
        c.r = c.g;
        c.b = c.g;
        c.rgb *= _PrimaryColor;
    }
    else if (c.r < 0.3f && c.g < 0.3f && c.b > 0.25f)
    {
        c.r = c.b;
        c.g = c.b;
        c.rgb *= _SecondaryColor;
    }
    else if (c.r > 0.25f && c.g < 0.3f && c.b < 0.3f)
    {
        c.g = c.r;
        c.b = c.r;
        c.rgb *= _TertiaryColor;

    }
    clip(c.w - _Cutoff);
    Out = c;
}