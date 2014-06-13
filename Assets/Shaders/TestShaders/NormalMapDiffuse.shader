Shader "Custom/Normal Map Diffuse" 
{
	Properties 
	{
		_Color ("Tint Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_MainTex ("Diffuse Texture", 2D) = "white" {}
		_BumpMap ("Normal Texture", 2D) = "bump" {}
		_BumpDepth("Bump Depth", Range(-2.0, 2.0)) = 1.0
	}

	SubShader
	{
		Pass
		{
			Tags { "LightMode" = "ForwardBase" }

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			uniform float4 _Color;
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform sampler2D _BumpMap;
			uniform float4 _BumpMap_ST;
			uniform float _BumpDepth;

			uniform float4 _LightColor0;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
				float4 tangent : TANGENT;
			};
			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 tex : TEXCOORD0;
				float4 posWorld : TEXCOORD1;
				float3 normalWorld : TEXCOORD2;
				float3 tangentWorld : TEXCOORD3;
				float3 binormalWorld : TEXCOORD4;
			};

			vertexOutput vert(vertexInput v)
			{
				vertexOutput o;

				o.normalWorld = normalize ( mul(float4(v.normal, 0.0), _World2Object).xyz);
				o.tangentWorld = normalize (mul ( _Object2World, float4(v.tangent.xyz, 0.0)).xyz);
				o.binormalWorld = normalize ( cross( o.normalWorld, o.tangentWorld) * v.tangent.w);

				o.posWorld = mul(_Object2World, v.vertex);
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.tex = v.texcoord;

				return o;
			}

			float4 frag(vertexOutput i) : COLOR
			{

				float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);

				float3 lightDirection;
				float atten;

				if (_WorldSpaceLightPos0.w == 0.0)
				{
					atten = 1.0;
					lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				}
				else
				{
					float3 fragmentToLightSource = _WorldSpaceLightPos0.xyz - i.posWorld.xyz;
					float distance = length(fragmentToLightSource) * 10;
					atten = 1.0/distance;
					lightDirection = normalize(fragmentToLightSource);
				}

				float4 tex = tex2D(_MainTex, i.tex.xy * _MainTex_ST.xy + _MainTex_ST.zw);
				float4 texN = tex2D(_BumpMap, i.tex.xy * _BumpMap_ST.xy + _BumpMap_ST.zw);

				float3 localCoords = float3(2.0 * texN.ag - float2(1.0, 1.0), 0.0);
				localCoords.z = _BumpDepth;

				float3x3 local2WorldTranspose = float3x3(i.tangentWorld,i.binormalWorld,i.normalWorld);

				float3 normalDirection = normalize(mul(localCoords, local2WorldTranspose));

				float3 diffuse = atten * _LightColor0.xyz *  saturate(dot (normalDirection, lightDirection));

				float3 lightFinal = UNITY_LIGHTMODEL_AMBIENT.xyz + diffuse;

				return float4(tex.xyz * lightFinal * _Color.xyz, 1.0);
			}
			ENDCG
		}

				Pass
		{
			Tags { "LightMode" = "ForwardAdd" }
			Blend One One

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			uniform float4 _Color;
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform sampler2D _BumpMap;
			uniform float4 _BumpMap_ST;
			uniform float _BumpDepth;

			uniform float4 _LightColor0;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
				float4 tangent : TANGENT;
			};
			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 tex : TEXCOORD0;
				float4 posWorld : TEXCOORD1;
				float3 normalWorld : TEXCOORD2;
				float3 tangentWorld : TEXCOORD3;
				float3 binormalWorld : TEXCOORD4;
			};

			vertexOutput vert(vertexInput v)
			{
				vertexOutput o;

				o.normalWorld = normalize ( mul(float4(v.normal, 0.0), _World2Object).xyz);
				o.tangentWorld = normalize (mul ( _Object2World, float4(v.tangent.xyz, 0.0)).xyz);
				o.binormalWorld = normalize ( cross( o.normalWorld, o.tangentWorld) * v.tangent.w);

				o.posWorld = mul(_Object2World, v.vertex);
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.tex = v.texcoord;

				return o;
			}

			float4 frag(vertexOutput i) : COLOR
			{

				float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);

				float3 lightDirection;
				float atten;

				if (_WorldSpaceLightPos0.w == 0.0)
				{
					atten = 1.0;
					lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				}
				else
				{
					float3 fragmentToLightSource = _WorldSpaceLightPos0.xyz - i.posWorld.xyz;
					float distance = length(fragmentToLightSource) * 10;
					atten = 1.0/distance;
					lightDirection = normalize(fragmentToLightSource);
				}

				float4 tex = tex2D(_MainTex, i.tex.xy * _MainTex_ST.xy + _MainTex_ST.zw);
				float4 texN = tex2D(_BumpMap, i.tex.xy * _BumpMap_ST.xy + _BumpMap_ST.zw);

				float3 localCoords = float3(2.0 * texN.ag - float2(1.0, 1.0), 0.0);
				localCoords.z = _BumpDepth;

				float3x3 local2WorldTranspose = float3x3(i.tangentWorld,i.binormalWorld,i.normalWorld);

				float3 normalDirection = normalize(mul(localCoords, local2WorldTranspose));

				float3 diffuse = atten * _LightColor0.xyz * saturate(dot (normalDirection, lightDirection));

				float3 lightFinal = UNITY_LIGHTMODEL_AMBIENT.xyz + diffuse;

				return float4(lightFinal * _Color.xyz, 1.0);
			}
			ENDCG
		}
	}
}
