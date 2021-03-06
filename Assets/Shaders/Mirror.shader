// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Mirror" {
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _ReflectionTexture("Reflection Texture", 2D) = "white" {}
        _BTex("Output Render Texture", 2D) = "black" {}
        _Color("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
        _ReflectionStrength("Reflection strength", Range(0,1)) = 0.5
        _ColourStrength("Colour strength", Range(0,1)) = 0.5
        _Alpha("Alpha", Range(0,1)) = 0.5
        _Cutoff("Shadow alpha cutoff", Range(0,1)) = 0.5
    }

        SubShader
        {
            Tags
            {
                "Queue" = "Geometry"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
                "PreviewType" = "Plane"
                "CanUseSpriteAtlas" = "True"
            }

            LOD 200
            Cull Off
            ZWrite Off
            Blend One OneMinusSrcAlpha

            CGPROGRAM
            #pragma target 3.0
            #pragma surface surf Lambert addshadow fullforwardshadows vertex:vert nofog nolightmap nodynlightmap keepalpha noinstancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"

            fixed _ReflectionStrength;
            fixed _ColourStrength;

            fixed _Cutoff;

            struct Input
            {
                float2 uv_MainTex;
                fixed4 color;
                float4 screenPos;
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

            sampler2D _ReflectionTexture;
            float _Aspect;
            float _Alpha;

            void surf(Input IN, inout SurfaceOutput o)
            {
                fixed4 c = SampleSpriteTexture(IN.uv_MainTex) * IN.color * _ColourStrength;

                float screenAspect = _ScreenParams.x / _ScreenParams.y;
                fixed2 coords = IN.screenPos.xy / (IN.screenPos.w + 0.001f);

                fixed4 r = tex2D(_ReflectionTexture, UNITY_PROJ_COORD(coords));

                o.Albedo = c + r * _ReflectionStrength;
                o.Alpha = c.a * _Alpha;
                clip(o.Alpha - _Cutoff);
            }
            ENDCG
        }

        Fallback "Transparent/VertexLit"
}