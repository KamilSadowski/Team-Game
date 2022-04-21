Shader "Custom/Dissolve"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Enabled("Enable the dissolve effect",Range(0,1)) = 0
		[HDR]_EdgeColour("EdgeColour", Color) = (0, 0.1238661, 4, 0)
		_MyTime("MyTime", Float) = 0
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
		_Cutoff("Shadow alpha cutoff", Range(0,1)) = 0.5
	}

	CGINCLUDE
	void Remap(float3 In, float2 InMinMax, float2 OutMinMax, out float3 Out)
	{
		Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
	}

		void Remap(float In, float2 InMinMax, float2 OutMinMax, out float Out)
		{
			Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
		}

		void Remap(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
		{
			Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
		}

	void Checkboard(float2 UV, float3 ColorA, float3 ColorB, float2 Frequency, out float3 result)
	{
				UV = (UV.xy + 0.5) * Frequency;
				float4 derivatives = float4(ddx(UV), ddy(UV));
				float2 duv_length = sqrt(float2(dot(derivatives.xz, derivatives.xz), dot(derivatives.yw, derivatives.yw)));
				float  width = 1.0;
				float2 distance3 = 4.0 * abs(frac(UV + 0.25) - 0.5) - width;
				float2 scale = 0.35 / duv_length.xy;
				float  freqLimiter = sqrt(clamp(1.1f - max(duv_length.x, duv_length.y), 0.0, 1.0));
				float2 vector_alpha = clamp(distance3 * scale.xy, -1.0, 1.0);
				float  alpha = saturate(0.5f + 0.5f * vector_alpha.x * vector_alpha.y * freqLimiter);
				result = lerp(ColorA, ColorB, alpha.xxx);
	}

	float random(float2 uv)
	{
		return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453123);
	}

	ENDCG


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

		uniform float4 _MainTex_TexelSize;

		fixed _Cutoff;


		struct Input
		{
			float2 uv_MainTex;
			fixed4 color;
		};


		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
		// put more per-instance properties here

		float _MyTime;
		float _Enabled;
		fixed4 _EdgeColor;

		UNITY_INSTANCING_BUFFER_END(Props)


		void vert(inout appdata_full v, out Input o)
		{
			v.vertex = UnityFlipSprite(v.vertex, _Flip);

			#if defined(PIXELSNAP_ON)
				v.vertex = UnityPixelSnap(v.vertex);
			#endif

			UNITY_INITIALIZE_OUTPUT(Input, o);

				if (_Enabled)
				{
					// X manipulation
					{
						float t = _MyTime;
						Remap(t, float2(-1.f, 1.f), float2(1.f, 2.f), t);

						float y = v.vertex.y;
						Remap(y, float2(-1, 1), float2(1, 1.2), y);

						v.vertex.x *= y * t;
					}

					// Y manipulation
					{

						float t = _MyTime;
						Remap(t, float2(-1, 1), float2(0, .2), t);

						v.vertex.y += t;
					}

				}

			o.color = v.color * _Color * _RendererColor;
		}


		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 c = SampleSpriteTexture(IN.uv_MainTex) * IN.color;
			clip(c.a - _Cutoff);

			// Dissolve code
			if (_Enabled)
			{
				float2 UV        = IN.uv_MainTex;
				float3 ColorA    = { 1,1,1 };
				float3 ColorB    = { 0,0,0 };
				float2 Frequency = 8;

				float3 checkboardCol;

				Checkboard(UV, ColorA, ColorB, Frequency, checkboardCol);

				float t = _MyTime;

				Remap(t, float2(2,0 ), float2 (1, .5 ), t);

				checkboardCol *= t;

				c.a *= checkboardCol.r;

				c = lerp(c, _EdgeColor, t);
			}

			o.Albedo = c.rgb;
			o.Alpha  = c.a;
		}
		ENDCG
	}
	Fallback "Transparent/VertexLit"
}