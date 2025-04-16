Shader "Custom/ShadowOnly"
{
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Lighting On
        ZWrite On
        ColorMask 0 // Don't write to the color buffer
        Pass
        {
            Name "SHADOWCASTER"
            Tags { "LightMode" = "ShadowCaster" }
        }
    }
}