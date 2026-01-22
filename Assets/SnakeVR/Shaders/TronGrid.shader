Shader "SnakeVR/TronGrid"
{
    Properties
    {
        _GridColor ("Grid Color", Color) = (0, 1, 1, 1)
        _BackgroundColor ("Background Color", Color) = (0, 0, 0, 1)
        _LineThickness ("Line Thickness", Range(0.001, 0.1)) = 0.02
        _GridScale ("Grid Scale", Range(0.5, 10)) = 2.0
        _EmissionIntensity ("Emission Intensity", Range(0, 5)) = 1.5
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _GridColor;
                float4 _BackgroundColor;
                float _LineThickness;
                float _GridScale;
                float _EmissionIntensity;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Scale UVs
                float2 scaledUV = input.uv * _GridScale;

                // Get fractional part (position within each cell)
                float2 cellUV = frac(scaledUV);

                // Create grid lines using smoothstep for anti-aliasing
                float halfThickness = _LineThickness * 0.5;

                // Horizontal lines (at top and bottom of each cell)
                float hLine = smoothstep(0, halfThickness, cellUV.y) *
                              smoothstep(0, halfThickness, 1.0 - cellUV.y);
                hLine = 1.0 - hLine;

                // Vertical lines (at left and right of each cell)
                float vLine = smoothstep(0, halfThickness, cellUV.x) *
                              smoothstep(0, halfThickness, 1.0 - cellUV.x);
                vLine = 1.0 - vLine;

                // Combine lines
                float lines = saturate(hLine + vLine);

                // Final color with emission
                half3 baseColor = lerp(_BackgroundColor.rgb, _GridColor.rgb, lines);
                half3 emission = _GridColor.rgb * lines * _EmissionIntensity;

                return half4(baseColor + emission, 1.0);
            }
            ENDHLSL
        }

        // Depth and shadow passes for URP compatibility
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }

            ZWrite On
            ColorMask 0

            HLSLPROGRAM
            #pragma vertex DepthVert
            #pragma fragment DepthFrag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings DepthVert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }

            half4 DepthFrag(Varyings input) : SV_Target
            {
                return 0;
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Lit"
}
