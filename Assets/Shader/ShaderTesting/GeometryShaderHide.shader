Shader "Custom/GeometryShaderHide"
{
    Properties
    {
        [IntRange] _StencilID ("Stencil ID", Range(0, 255)) = 2
        _BaseColor ("Color", Color) = (1, 0, 0, 1)
    }

    SubShader
    {
        Tags { 
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry+2"  // Draws LAST at 2002, after the wall
        }

        Pass
        {
            ZWrite On
            ZTest Always  // Ignore depth — renders even behind the wall

            Stencil
            {
                Ref [_StencilID]
                Comp NotEqual   // Only render where the mask painted the stencil
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings   { float4 positionHCS : SV_POSITION; };

            float4 _BaseColor;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                return _BaseColor;
            }
            ENDHLSL
        }
    
    }
}
