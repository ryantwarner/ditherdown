Shader "Hidden/Shader/DitherDown"
{
        HLSLINCLUDE

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
    
    struct Attributes
    {
        uint vertexID : SV_VertexID;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float2 texcoord   : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    Varyings Vertex(Attributes input)
    {
        Varyings output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
        output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
        return output;
    }

    TEXTURE2D(_DitherTexture);
    float _Dithering;
    float _Downsampling;
    float _Levels;

    TEXTURE2D_X(_InputTexture);

	half quantize(half c)
	{
        int levels = _Levels;
		int val = c*255.0;
		val = (((val * levels + 127) / 255) * 255 + levels / 2) / levels;
		return val / 255.0;
	} 

    float4 Fragment(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        // Input sample
        const uint2 pss = (uint2)(input.texcoord * _ScreenSize.xy) / _Downsampling;
        float4 col = LOAD_TEXTURE2D_X(_InputTexture, pss * _Downsampling);
        
        // Linear -> sRGB
        col.rgb = LinearToSRGB(col.rgb);
        uint tw, th;
        _DitherTexture.GetDimensions(tw, th);
        float dither = LOAD_TEXTURE2D(_DitherTexture, pss % uint2(tw, th)).x;
        col.rgb += dither * _Dithering;
        col.r = quantize(col.r);
        col.g = quantize(col.g);
        col.b = quantize(col.b);

        // sRGB -> Linear
        col.rgb = SRGBToLinear(col.rgb);

        return col;
    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Cull Off ZWrite Off ZTest Always
            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            ENDHLSL
        }
    }
    Fallback Off
}
