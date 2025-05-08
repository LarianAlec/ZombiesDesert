// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PBR Bloody Surface"
{
	Properties
	{
		_TopTexture1("Blood Splat", 2D) = "white" {}
		_Albedo("Albedo", 2D) = "white" {}
		_MetallicSmoothness("Metallic-Smoothness", 2D) = "white" {}
		_MetallicStrength("Metallic Strength", Range( 0 , 2)) = 1
		_SmoothnessStrength("Smoothness Strength", Range( 0 , 1)) = 1
		_NormalMap("Normal Map", 2D) = "bump" {}
		_AO("AO", 2D) = "white" {}
		_AOMultiplier("AO Multiplier", Range( 0 , 1)) = 0
		_BloodAmount("Blood Amount", Range( 0 , 1)) = 1
		_BloodAlbedo(" Blood Albedo", 2D) = "white" {}
		_BloodSmoothness("Blood Smoothness", Range( 0 , 1)) = 0.7
		[Normal]_BloodNormalMap("Blood Normal Map", 2D) = "bump" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform sampler2D _BloodNormalMap;
		uniform float4 _BloodNormalMap_ST;
		uniform sampler2D _TopTexture1;
		uniform float _BloodAmount;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform sampler2D _BloodAlbedo;
		uniform float _MetallicStrength;
		uniform sampler2D _MetallicSmoothness;
		uniform float4 _MetallicSmoothness_ST;
		uniform float _SmoothnessStrength;
		uniform float _BloodSmoothness;
		uniform sampler2D _AO;
		uniform float4 _AO_ST;
		uniform float _AOMultiplier;


		inline float4 TriplanarSamplingSF( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float tilling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= projNormal.x + projNormal.y + projNormal.z;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = ( tex2D( topTexMap, tilling * worldPos.zy * float2( nsign.x, 1.0 ) ) );
			yNorm = ( tex2D( topTexMap, tilling * worldPos.xz * float2( nsign.y, 1.0 ) ) );
			zNorm = ( tex2D( topTexMap, tilling * worldPos.xy * float2( -nsign.z, 1.0 ) ) );
			return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			float3 tex2DNode17 = UnpackNormal( tex2D( _NormalMap, uv_NormalMap ) );
			float2 uv_BloodNormalMap = i.uv_texcoord * _BloodNormalMap_ST.xy + _BloodNormalMap_ST.zw;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float4 triplanar130 = TriplanarSamplingSF( _TopTexture1, ase_worldPos, ase_worldNormal, 1.0, 1.0, 1.0, 0 );
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float temp_output_107_0 = ( exp( ( triplanar130.x * _BloodAmount ) ) * ( saturate( ase_vertex3Pos.y ) * _BloodAmount ) );
			float BloodMask74 = saturate( temp_output_107_0 );
			float3 lerpResult73 = lerp( tex2DNode17 , BlendNormals( UnpackNormal( tex2D( _BloodNormalMap, uv_BloodNormalMap ) ) , tex2DNode17 ) , BloodMask74);
			o.Normal = lerpResult73;
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 triplanar90 = TriplanarSamplingSF( _BloodAlbedo, ase_worldPos, ase_worldNormal, 1.0, 1.0, 1.0, 0 );
			float4 lerpResult57 = lerp( tex2D( _Albedo, uv_Albedo ) , triplanar90 , BloodMask74);
			o.Albedo = lerpResult57.xyz;
			float2 uv_MetallicSmoothness = i.uv_texcoord * _MetallicSmoothness_ST.xy + _MetallicSmoothness_ST.zw;
			float4 tex2DNode3 = tex2D( _MetallicSmoothness, uv_MetallicSmoothness );
			float3 appendResult10 = (float3(tex2DNode3.r , tex2DNode3.g , tex2DNode3.b));
			float3 lerpResult77 = lerp( ( _MetallicStrength * appendResult10 ) , float3( 0,0,0 ) , BloodMask74);
			o.Metallic = lerpResult77.x;
			float lerpResult79 = lerp( ( tex2DNode3.a * _SmoothnessStrength ) , _BloodSmoothness , BloodMask74);
			o.Smoothness = lerpResult79;
			float2 uv_AO = i.uv_texcoord * _AO_ST.xy + _AO_ST.zw;
			float4 temp_cast_3 = (( 1.0 - _AOMultiplier )).xxxx;
			float4 blendOpSrc152 = tex2D( _AO, uv_AO );
			float4 blendOpDest152 = temp_cast_3;
			o.Occlusion = ( saturate( ( blendOpSrc152 + blendOpDest152 - 1.0 ) )).r;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15800
7;29;1906;1004;1944.027;1398.748;2.070821;True;True
Node;AmplifyShaderEditor.CommentaryNode;29;-1197.06,-1053.292;Float;False;1411.623;632.874;Blood;21;87;31;90;91;74;100;83;84;69;70;107;130;137;138;139;140;141;142;143;144;145;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TriplanarNode;130;-852.5033,-1014.958;Float;True;Spherical;World;False;Blood Splat;_TopTexture1;white;0;Assets/PBR Melee Weapons Pack/Shaders/Splat.png;Mid Texture 1;_MidTexture1;white;-1;None;Bot Texture 1;_BotTexture1;white;-1;None;Blood Splat;False;9;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;8;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;84;-1183.542,-510.6085;Float;False;Property;_BloodAmount;Blood Amount;8;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;141;-501.7566,-936.4677;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;143;-551.809,-842.2302;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ExpOpNode;137;-509.3163,-828.3984;Float;False;1;0;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;145;-430.066,-760.0139;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;70;-1181.469,-996.4993;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;69;-991.2154,-935.8134;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;144;-592.4733,-730.9634;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;83;-1184.852,-751.7936;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;142;-924.9437,-839.0679;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;107;-959.7436,-737.0886;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;138;-388.5141,-736.8892;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;14;-737.8049,-114.5418;Float;False;846.7528;488.9396;Metallic + Smoothness;6;12;25;13;24;10;3;;1,1,1,1;0;0
Node;AmplifyShaderEditor.WireNode;140;-229.7354,-552.492;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;26;-751.6041,404.9932;Float;False;969.6501;347.7845;Ambient Occlusion  / needs better multiplier;4;23;16;19;152;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;28;-494.4572,-397.6068;Float;False;615.605;255.2513;Normal Map  / need to add a intensity multiplier;1;17;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;3;-710.9174,63.03497;Float;True;Property;_MetallicSmoothness;Metallic-Smoothness;2;0;Create;True;0;0;False;0;None;4d69115ddca993041aee0b9663cb1ecc;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;139;-423.4952,-517.1375;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-506.5188,-70.99948;Float;False;Property;_MetallicStrength;Metallic Strength;3;0;Create;True;0;0;False;0;1;1.057;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;74;-392.6135,-498.9058;Float;False;BloodMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;31;-145.4033,-626.0894;Float;True;Property;_BloodNormalMap;Blood Normal Map;11;1;[Normal];Create;True;0;0;False;0;3ebeb8125bc0c0d4f918324c9bd89a91;3ebeb8125bc0c0d4f918324c9bd89a91;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;91;-359.3882,-1002.349;Float;True;Property;_BloodAlbedo; Blood Albedo;9;0;Create;True;0;0;False;0;0aa4471809917df42b4d5fcbbf6035de;0aa4471809917df42b4d5fcbbf6035de;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SamplerNode;17;-460.4538,-344.1913;Float;True;Property;_NormalMap;Normal Map;5;0;Create;True;0;0;False;0;None;e76f437783afb124caa4849872410311;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;15;-148.8337,-1354.942;Float;False;342.3592;277.4363;Albedo;1;2;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-392.1243,249.3053;Float;False;Property;_SmoothnessStrength;Smoothness Strength;4;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;10;-315.9747,52.2675;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-733.1474,664.7343;Float;False;Property;_AOMultiplier;AO Multiplier;7;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;87;-130.8206,-726.8696;Float;False;Property;_BloodSmoothness;Blood Smoothness;10;0;Create;True;0;0;False;0;0.7;0.75;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;81;127.6224,-368.0043;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-103.9193,151.9651;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendNormalsNode;153;247.0004,-586.7177;Float;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;16;-740.1138,453.8961;Float;True;Property;_AO;AO;6;0;Create;True;0;0;False;0;None;c5c5cc6707ee66f409351171e7330f3f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;82;179.5975,111.2892;Float;False;74;BloodMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;23;-470.993,666.1022;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;75;288.9077,-682.1968;Float;False;74;BloodMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-115.5004,-1297.83;Float;True;Property;_Albedo;Albedo;1;0;Create;True;0;0;False;0;None;08a9f2070d25cbc45b00d7b575c0292c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-104.445,-65.442;Float;True;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TriplanarNode;90;-138.9878,-999.3483;Float;True;Spherical;World;False;Top Texture 0;_TopTexture0;white;0;Assets/PBR Melee Weapons Pack/Shaders/Blood.png;Mid Texture 0;_MidTexture0;white;-1;None;Bot Texture 0;_BotTexture0;white;-1;None;Triplanar Sampler;False;9;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;8;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;100;-725.2206,-646.5349;Float;True;5;0;FLOAT;0;False;1;FLOAT;0.07;False;2;FLOAT;0.9;False;3;FLOAT;-0.33;False;4;FLOAT;1.25;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;57;588.5378,-790.2691;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;79;483.031,141.9394;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;152;-204.5648,498.1871;Float;True;LinearBurn;True;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;73;591.9124,-596.8661;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;77;548.1017,-38.31907;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;865.3575,-3.473332;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;PBR Bloody Surface;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;141;0;130;1
WireConnection;141;1;84;0
WireConnection;143;0;141;0
WireConnection;137;0;143;0
WireConnection;145;0;137;0
WireConnection;69;0;70;2
WireConnection;144;0;145;0
WireConnection;83;0;69;0
WireConnection;83;1;84;0
WireConnection;142;0;144;0
WireConnection;107;0;142;0
WireConnection;107;1;83;0
WireConnection;138;0;107;0
WireConnection;140;0;138;0
WireConnection;139;0;140;0
WireConnection;74;0;139;0
WireConnection;10;0;3;1
WireConnection;10;1;3;2
WireConnection;10;2;3;3
WireConnection;81;0;17;0
WireConnection;24;0;3;4
WireConnection;24;1;25;0
WireConnection;153;0;31;0
WireConnection;153;1;17;0
WireConnection;23;0;19;0
WireConnection;13;0;12;0
WireConnection;13;1;10;0
WireConnection;90;0;91;0
WireConnection;100;0;107;0
WireConnection;57;0;2;0
WireConnection;57;1;90;0
WireConnection;57;2;75;0
WireConnection;79;0;24;0
WireConnection;79;1;87;0
WireConnection;79;2;82;0
WireConnection;152;0;16;0
WireConnection;152;1;23;0
WireConnection;73;0;81;0
WireConnection;73;1;153;0
WireConnection;73;2;75;0
WireConnection;77;0;13;0
WireConnection;77;2;82;0
WireConnection;0;0;57;0
WireConnection;0;1;73;0
WireConnection;0;3;77;0
WireConnection;0;4;79;0
WireConnection;0;5;152;0
ASEEND*/
//CHKSM=EFAA37E193EC2B17C4D8E6E09481D8C622ACBD4A