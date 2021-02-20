Shader "Unlit/MapShader"
{
    Properties
    {
        _MainTex ("MainTexture", 2D) = "white" {}
        _HeightTex ("HeightTexture", 2D) = "white" {}
        _FogTex ("FogTexture", 2D) = "white" {}
        _ForestMaskTex ("ForestMaskTexture", 2D) = "white" {}
        _BackgroundTex ("BackgroundTexture", 2D) = "white" {}
        _ForestTex ("ForestTexture", 2D) = "white" {}
        _WaterLevel ("WaterLevel", Float) = 4.7
        _ShoreColor ("ShoreColor", Color) = (0, 0, 0, 0)
        _ShallowWaterColor ("ShallowWaterColor", Color) = (0, 0, 0, 0)
        _DeepWaterColor ("DeepWaterColor", Color) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _HeightTex;
            float4 _HeightTex_ST;
            
            sampler2D _FogTex;
            float4 _FogTex_ST;

            sampler2D _ForestMaskTex;
            float4 _ForestMaskTex_ST;

            sampler2D _BackgroundTex;
            float4 _BackgroundTex_ST;

            sampler2D _ForestTex;
            float4 _ForestTex_ST;

            float _WaterLevel;

            float4 _ShoreColor;
            float4 _ShallowWaterColor;
            float4 _DeepWaterColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 background = tex2D(_BackgroundTex, i.uv * 10);

                // sample the texture
                fixed4 forest = tex2D(_ForestTex, i.uv * 150);
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 forestMask = tex2D(_ForestMaskTex, i.uv);
                col = lerp(col, forest, forestMask.r * forest.a);

                fixed4 height = tex2D(_HeightTex, i.uv);
                float waterMask = step(height.r, _WaterLevel);

                float shoreMask = clamp((height.r - _WaterLevel) * 4, 0, 1);

                float shallowRamp = clamp((height.r - _WaterLevel + .2) * 4, 0, 1);
                float deepRamp = clamp((height.r - _WaterLevel + 1), 0, 1);

                // fixed3 explored = background.r * lerp(col.rgb, fixed3(0, 0, 1), waterMask);
                fixed3 explored = lerp(_ShoreColor.rgb, col.rgb, shoreMask);
                explored = lerp(_ShallowWaterColor, explored, shallowRamp);
                explored = lerp(_DeepWaterColor, explored, deepRamp);
                explored *= background.r;

                fixed4 fog = tex2D(_FogTex, i.uv);
                fixed3 final = lerp(explored, background, fog.r)


                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                // return shoreMask;
                return fixed4(final, 1.0);
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}
