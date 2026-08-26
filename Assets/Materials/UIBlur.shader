Shader "UI/BackgroundBlur"
{
    Properties
    {
        [HideInInspector] _MainTex ("Sprite Texture", 2D) = "white" {}
        _TintColor ("Tint Color", Color) = (0, 0, 0, 0.45)
        _Size ("Blur Size", Range(0, 15)) = 3.5
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        GrabPass { "_BackgroundTexture" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float4 uvgrab : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            sampler2D _MainTex;
            fixed4 _TintColor;
            float _Size;
            sampler2D _BackgroundTexture;
            float4 _BackgroundTexture_TexelSize;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                #if UNITY_UV_STARTS_AT_TOP
                float scale = -1.0;
                #else
                float scale = 1.0;
                #endif
                o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y * scale) + o.vertex.w) * 0.5;
                o.uvgrab.zw = o.vertex.zw;
                o.uv = v.texcoord;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uvgrab.xy / i.uvgrab.w;
                fixed4 col = fixed4(0, 0, 0, 0);
                float stepU = _BackgroundTexture_TexelSize.x * _Size;
                float stepV = _BackgroundTexture_TexelSize.y * _Size;

                col += tex2D(_BackgroundTexture, uv + float2(-stepU, -stepV)) * 0.077;
                col += tex2D(_BackgroundTexture, uv + float2( 0.0,   -stepV)) * 0.123;
                col += tex2D(_BackgroundTexture, uv + float2( stepU, -stepV)) * 0.077;
                col += tex2D(_BackgroundTexture, uv + float2(-stepU,  0.0  )) * 0.123;
                col += tex2D(_BackgroundTexture, uv + float2( 0.0,    0.0  )) * 0.200;
                col += tex2D(_BackgroundTexture, uv + float2( stepU,  0.0  )) * 0.123;
                col += tex2D(_BackgroundTexture, uv + float2(-stepU,  stepV)) * 0.077;
                col += tex2D(_BackgroundTexture, uv + float2( 0.0,    stepV)) * 0.123;
                col += tex2D(_BackgroundTexture, uv + float2( stepU,  stepV)) * 0.077;

                return lerp(col, _TintColor, _TintColor.a);
            }
            ENDCG
        }
    }
}