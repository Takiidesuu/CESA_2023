Shader "Custom/EmissionNoise"
{
    Properties{
       _EmissionMap("Emission", 2D) = "white" {}
       _NoiseTexture("Noise Texture", 2D) = "white" {}
       _NoiseStrength("Noise Strength", Range(0, 1)) = 0.5
    }

        SubShader{
           Tags {"Queue" = "Transparent" "RenderType" = "Opaque"}
           LOD 100

           CGPROGRAM
           #pragma surface surf Standard
           sampler2D _EmissionMap;
           sampler2D _NoiseTexture;
           float _NoiseStrength;

           struct Input {
              float2 uv_EmissionMap;
              float2 uv_NoiseTexture;
           };

           void surf(Input IN, inout SurfaceOutputStandard o) {
               // ノイズを生成
               float noise = tex2D(_NoiseTexture, IN.uv_NoiseTexture).r;

               // EmissionMapから色を取得
               float3 emissionColor = tex2D(_EmissionMap, IN.uv_EmissionMap).rgb;

               // ノイズの影響を加える
               emissionColor *= (1.0 - _NoiseStrength) + (_NoiseStrength * noise);

               // 結果を出力
               o.Emission = emissionColor;
            }
            ENDCG
       }
}