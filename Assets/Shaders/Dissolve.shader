Shader "Custom/Dissolve"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_NormalMap("Normal Map", 2D) = "bump" {}
		_NormalStrenght("Normal Strength", Range(0, 1.5)) = 0.5
		_Enabled("Enable the dissolve effect",Range(0,1)) = 0
		[HDR]_EdgeColour("EdgeColour", Color) = (0, 0.1238661, 4, 0)
		_MyTime("MyTime", Float) = 0
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[PerRendererData] _PrimaryColor("PrimaryColor", Color) = (1,1,1,1)
		[PerRendererData] _SecondaryColor("SecondaryColor", Color) = (0.9490196,0.7803922,0.6745098,1)
		[PerRendererData] _TertiaryColor("TertiaryColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
		 _DissolveMap("Dissolve Map", 2D) = "white" {}
		 _DissolveAmount("DissolveAmount", Range(0,1)) = 0
		 _DissolveColor("DissolveColor", Color) = (1,1,1,1)
		 _DissolveEmission("DissolveEmission", Range(0,1)) = 1
		 _DissolveWidth("DissolveWidth", Range(0,0.1)) = 0.05
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
#pragma target 3.0
		#include "UnitySprites.cginc"

		uniform float4 _MainTex_TexelSize;
		sampler2D _DissolveMap;
		sampler2D _NormalMap;

		fixed _Cutoff;


		struct Input
		{
			float2 uv_MainTex;
			float2 uv_DissolveMap;
			float2 uv_NormalMap;
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
		half _DissolveAmount;
		half _NormalStrenght;
		half _DissolveEmission;
		half _DissolveWidth;
		float4 _PrimaryColor;
		float4 _SecondaryColor;
		float4 _TertiaryColor;
		fixed4 _DissolveColor;

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
						Remap(t, float2(-1.f, 1.f), float2(.5, 2.f), t);

						float y = v.vertex.y;
						Remap(y, float2(-1, 1), float2(.5, 1.2), y);

						v.vertex.x *= y * t;
					}

					// Y manipulation
					{

						float t = _MyTime;
						Remap(t, float2(-1, 1), float2(0, 0.2), t);

						v.vertex.y += t;
					}
				}

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

			clip(c.a - _Cutoff);

			fixed4 mask = tex2D(_DissolveMap,IN.uv_DissolveMap);
			if (mask.r < _DissolveAmount)
				discard;

			o.Albedo = c.rgb / 2;

			if (mask.r < _DissolveAmount + _DissolveWidth) {
				o.Albedo = _DissolveColor * c.a;
				o.Emission = _DissolveColor * _DissolveEmission * c.a;
			}

			o.Alpha = c.a;
			o.Normal = UnpackScaleNormal(tex2D(_NormalMap,IN.uv_NormalMap), _NormalStrenght);
		}
		ENDCG
	}
	Fallback "Transparent/VertexLit"
}