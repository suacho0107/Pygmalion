Shader "UI/SlantedFill"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)

        _FillAmount("Fill Amount", Range(0,1)) = 1

        // 기울기 강도 (0 = 수직, 값 클수록 더 눕는다)
        _Slope("Slope", Range(0.01, 1)) = 0.05
    }

        SubShader
        {
            Tags
            {
                "Queue" = "Transparent"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
                "PreviewType" = "Plane"
                "CanUseSpriteAtlas" = "True"
            }

            Cull Off
            Lighting Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    fixed4 color : COLOR;
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    fixed4 color : COLOR;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;

                fixed4 _Color;

                float _FillAmount;
                float _Slope;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    o.color = v.color * _Color;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.uv) * i.color;

                    // 사선 마스크 계산
                    float mask = i.uv.x - i.uv.y * _Slope;
                    
                    // mask의 실제 범위
                    float minMask = -_Slope;
                    float maxMask = 1.0;
                    
                    // 0~1로 정규화
                    float normalized = (mask - minMask) / (maxMask - minMask);

                    //FillAmount와 비교
                    if (normalized > _FillAmount)
                    {
                        discard;
                    }

                    return col;
                }
                ENDCG
            }
        }
}