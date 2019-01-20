Shader "Unlit/BrightSprite"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MultiplyColor  ("Multiply Color", Color) = (0.5, 0.5, 0.5, 1)
		[PreRendererData] _Color ("Tint", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags {
			"Queue"="Transparent"
			"RenderType"="Transparent"
			}
		LOD 100

		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			float4 _MultiplyColor;
			float4 _Color;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color * _Color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				
				return col * i.color*(fixed4(2,2,2,0)+i.color);
				
			}
			ENDCG
		}
	}
}
