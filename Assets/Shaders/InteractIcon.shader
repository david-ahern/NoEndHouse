Shader "HUD/InteractIcon"
{
	Properties
	{
		_MainTex ("Default Icon", 2D) = "white" {}
		_PickupTex("Pickup Icon", 2D) = "white" {}
		_DropTex("Drop Icon", 2D) = "white" {}
		_Blend("Blend", Range(0.0, 1.0)) = 0.5
	}

	SubShader
	{
		Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			uniform sampler2D _MainTex;
			uniform sampler2D _PickupTex;
			uniform sampler2D _DropTex;
			uniform float4 _MainTex_ST;
			uniform float _Blend;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 tex : TEXCOORD0;
			};

			vertexOutput vert(vertexInput v)
			{
				vertexOutput o;

				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.tex = v.texcoord;

				return o;
			}

			float4 frag(vertexOutput i) : COLOR
			{
				float4 tex = tex2D(_MainTex, i.tex.xy * _MainTex_ST.xy + _MainTex_ST.zw);
				if (tex.w < 0.9)
					tex.w = 0;
				float texW = (_Blend - 0.5) * 2;
				if (texW < 0)
					texW *= -1;
				texW = 1 - texW;

				float4 pickup = tex2D(_PickupTex, i.tex.xy * _MainTex_ST.xy + _MainTex_ST.zw);
				if (pickup.w < 0.8)
					pickup.w = 0;
				float pickupW = 1 - (_Blend * 2);
				if (pickupW < 0) 
					pickupW = 0;

				float4 drop = tex2D(_DropTex, i.tex.xy * _MainTex_ST.xy + _MainTex_ST.zw);
				if (drop.w < 0.8)
					drop.w = 0;
				float dropW = (_Blend - 0.5) * 2;
				if (dropW < 0)
					dropW = 0;

				float4 rtn = (tex * texW) + (pickup * pickupW) + (drop * dropW);

				return rtn;
			}
			ENDCG
		}
	}
}
