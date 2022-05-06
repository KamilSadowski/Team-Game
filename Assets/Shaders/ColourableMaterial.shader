Shader "Custom/ColourableMaterial" {
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
        _Cutoff("Shadow alpha cutoff", Range(0,1)) = 0.5
        [PerRendererData] _PrimaryColor("PrimaryColor", Color) = (1,1,1,1)
        [PerRendererData] _SecondaryColor("SecondaryColor", Color) = (0.9490196,0.7803922,0.6745098,1)
        [PerRendererData] _TertiaryColor("TertiaryColor", Color) = (1,1,1,1)
    }

        SubShader
        {
            Tags
            {
                "Queue" = "Geometry"
                "IgnoreProjector" = "True"
                "RenderType" = "TransparentCutout"
                "PreviewType" = "Plane"
                "CanUseSpriteAtlas" = "True"
            }

            LOD 200
            Cull Off
            ZWrite Off
            Blend One OneMinusSrcAlpha

            CGPROGRAM
            #pragma surface surf Lambert addshadow fullforwardshadows vertex:vert nofog nolightmap nodynlightmap keepalpha noinstancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"

            fixed _Cutoff;
            float4 _PrimaryColor;
            float4 _SecondaryColor;
            float4 _TertiaryColor;

            struct Input
            {
                float2 uv_MainTex;
                fixed4 color;
            };

            void vert(inout appdata_full v, out Input o)
            {
                v.vertex = UnityFlipSprite(v.vertex, _Flip);

                #if defined(PIXELSNAP_ON)
                v.vertex = UnityPixelSnap(v.vertex);
                #endif

                UNITY_INITIALIZE_OUTPUT(Input, o);
                o.color = v.color * _Color * _RendererColor;
            }

            void surf(Input IN, inout SurfaceOutput o)
            {
                const float MIN_THRESHOLD = 0.1f;
                const float MAX_THRESHOLD = 0.3f;

                fixed4 c = SampleSpriteTexture(IN.uv_MainTex) * IN.color;
                if (c.r < MAX_THRESHOLD && c.g > MIN_THRESHOLD && c.b < MAX_THRESHOLD)
                {
                    c.r = c.g;
                    c.b = c.g;
                    c.rgb *= _PrimaryColor;
                }
                else if (c.r < MAX_THRESHOLD && c.g < MAX_THRESHOLD && c.b > MIN_THRESHOLD)
                {
                    c.r = c.b;
                    c.g = c.b;
                    c.rgb *= _SecondaryColor;
                }
                else if (c.r > MIN_THRESHOLD && c.g < MAX_THRESHOLD && c.b < MAX_THRESHOLD)
                {
                    c.g = c.r;
                    c.b = c.r;
                    c.rgb *= _TertiaryColor;

                }
                o.Albedo = c.rgb * c.a;
                o.Alpha = c.a;
                clip(o.Alpha - _Cutoff);
            }
            ENDCG
        }

            Fallback "Transparent/VertexLit"
}