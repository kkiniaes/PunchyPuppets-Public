Shader "Unlit/NewUnlitShader"
{
	Properties
	{
		_Color ("Main Color", Color) = (0.5, 0.5, 0.5, 1)
		_AmbientColor ("Ambient Color", Color) = (0, 0, 0, 1)
		// _LightingColor ("Light Color" , Color) = (0, 0, 0, 1)
		_MainTex ("Texture", 2D) = "white" {}
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_OutlineWidth("Outline Width", Range(1.0, 5.0)) = 1.01
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
			};

			float4 _OutlineColor;
			float _OutlineWidth;

			v2f vert (appdata v)
			{
				v.vertex.xyz += v.normal/10000 * _OutlineWidth * 10;

				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = _OutlineColor;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				return i.color;
			}


			ENDCG
		}

		Pass
		{
			ZWrite On

			Material
			{
				Diffuse[_Color]
				Ambient[_AmbientColor]
			}

			Lighting On

			SetTexture[_MainTex]
			{
				ConstantColor[_Color]
			}

			SetTexture[_MainTex]
			{
				Combine previous * primary DOUBLE
			}

			// SetTexture[_LightingColor]
			// {
			// 	// Combine previous lerp (ConstantColor[_LightingColor]) ConstantColor[_Color]
			// 	combine previous lerp (primary) texture
			// }
		}
	}
}
