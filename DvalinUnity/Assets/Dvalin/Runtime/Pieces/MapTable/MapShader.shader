Shader "Custom/MapShader"
{
    Properties
    {
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Occlusion ("Occlusion", Range(0,1)) = 0.5
        _Fog ("Fog", Range(0,1)) = 1.0
        _Color ("Color", Color) = (0, 0, 0, 0)

        _MainTex ("MainTexture", 2D) = "white" {}
        _HeightTex ("HeightTexture", 2D) = "white" {}
        _FogTex ("FogTexture", 2D) = "white" {}
        _ForestMaskTex ("ForestMaskTexture", 2D) = "white" {}
        _BackgroundTex ("BackgroundTexture", 2D) = "white" {}
        _ForestTex ("ForestTexture", 2D) = "white" {}
        _WaterLevel ("WaterLevel", Float) = 0.58
        _ShoreColor ("ShoreColor", Color) = (0, 0, 0, 0)
        _ShallowWaterColor ("ShallowWaterColor", Color) = (0, 0, 0, 0)
        _DeepWaterColor ("DeepWaterColor", Color) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        float _Glossiness;
        float _Metallic;
        float _Occlusion;
        float _Fog;
        float4 _Color;

        sampler2D _MainTex;
        sampler2D _HeightTex;
        sampler2D _FogTex;
        sampler2D _ForestMaskTex;
        sampler2D _BackgroundTex;
        sampler2D _ForestTex;
        float _WaterLevel;
        float4 _ShoreColor;
        float4 _ShallowWaterColor;
        float4 _DeepWaterColor;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 background = tex2D(_BackgroundTex, IN.uv_MainTex * 1);

            fixed4 forest = tex2D(_ForestTex, IN.uv_MainTex * 100);
            fixed4 col = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 forestMask = tex2D(_ForestMaskTex, IN.uv_MainTex);
            col = lerp(col, forest, forestMask.r * forest.a);

            fixed4 height = tex2D(_HeightTex, IN.uv_MainTex);// * 12.5; // adjusted height values for dev
            float waterMask = step(height.r, _WaterLevel);

            float shoreMask = clamp((height.r - _WaterLevel) * 0.5, 0, 1);

            float shallowRamp = clamp((height.r - _WaterLevel + .2 * 12.5) * 0.5, 0, 1);
            float deepRamp = clamp((height.r - _WaterLevel + 1 * 12.5) * 0.1, 0, 1);

            fixed3 explored = lerp(_ShoreColor.rgb, col.rgb, shoreMask);
            explored = lerp(_ShallowWaterColor, explored, shallowRamp);
            explored = lerp(_DeepWaterColor, explored, deepRamp);
            explored *= background.r;

            fixed4 fog = tex2D(_FogTex, IN.uv_MainTex);
            fixed3 final = lerp(explored, background, fog.r * _Fog);

            o.Albedo = final * _Color;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = 1;
            o.Occlusion = _Occlusion;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
