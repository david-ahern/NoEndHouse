Shader "HUD/InteractIcon"
{
	Properties
	{
		_MainTex ("MainTex", 2D) = "white" {}
		_BlendTex("BlendTex", 2D) = "white" {}
		_ExtraTex("ExtraTex", 2D) = "white" {}
		_Blend("Blend", Range(0.0, 1.0)) = 1.0
	}

	SubShader
	{
		Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
		LOD 100

		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			uniform sampler2D _MainTex;
			uniform sampler2D _BlendTex;
			uniform sampler2D _ExtraTex;
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
				if (tex.w < 0.8)
					tex.w = 0;
				float texW = 1 - (_Blend * 2);
				if (texW < 0) 
					texW = 0;

				float4 bln = tex2D(_BlendTex, i.tex.xy * _MainTex_ST.xy + _MainTex_ST.zw);
				if (bln.w < 0.8)
					bln.w = 0;
				float blnW = (_Blend - 0.5) * 2;
				if (blnW < 0)
					blnW = 0;

				float4 ext = tex2D(_ExtraTex, i.tex.xy * _MainTex_ST.xy + _MainTex_ST.zw);
				if (ext.w < 0.8)
					ext.w = 0;
				float extW = (_Blend - 0.5) * 2;
				if (extW < 0)
					extW *= -1;
				extW = 1 - extW;

				float4 rtn = (tex * texW) + (bln * blnW) + (ext * extW);

				return rtn;
			}
			ENDCG
		}
	}
}
