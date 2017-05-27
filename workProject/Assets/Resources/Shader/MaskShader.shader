// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/MaskShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_MaskTex ("Mask", 2D) = "white" {}
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	sampler2D _MainTex;
	sampler2D _MaskTex;
	
	struct appdata{
		float4 pos :POSITION;
		float2 texcoord: TEXCOORD;
		float4 col :COLOR;
	};
	struct v2f{
		float4 pos :POSITION;
		float2 texcoord: TEXCOORD;
		float4 col :COLOR;
	};

	v2f vert(appdata i){
		v2f o;
		o.pos = UnityObjectToClipPos (i.pos ); 
        o.texcoord = MultiplyUV( UNITY_MATRIX_TEXTURE0, i.texcoord ); 
		return o;
	}
	
	float4 frag(v2f i):COLOR{
		//use mask texture alpha instead
		float4 mcol = tex2D(_MaskTex, i.texcoord);
		float4 col  = tex2D(_MainTex, i.texcoord);
		col.a = mcol.a;
		return col;
	}
	ENDCG

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass {
//			Blend SrcAlpha One
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	} 

}
