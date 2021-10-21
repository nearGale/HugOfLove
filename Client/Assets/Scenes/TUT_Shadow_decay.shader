// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "TUT/DynamicFlatShadow_d" {
	Properties {
		_MainTex ("Main Texture", 2D) = "white" {}
		_IconTex ("Icon", 2D) = "white" {}
		_OffsetSize ("Offset", Range(0,0.05)) = 0.01
		_ShadowColor ("Shadow Color", color) = (0,0,0,0)
		_rot ("Rotation", Range(0,360)) = 1.0
		_decay ("Decay", Range(0.6,1)) = 1.0
	}
	SubShader {
		CGINCLUDE
		
		#include "UnityCG.cginc"
		
		sampler2D _MainTex;  
		uniform half4 _MainTex_ST;
		uniform half4 _IconTex_ST;
		sampler2D _IconTex;  
		half4 _IconTex_TexelSize;
		float _OffsetSize;
		float4 _ShadowColor;
		fixed _rot; 
		fixed _decay; 

		struct v2f {
			float4 pos : SV_POSITION;
			half2 uv[15]: TEXCOORD0;
		};
		
		
		v2f vert(appdata_img v) {
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			_rot=fmod(_rot,360);
			_rot=radians(_rot);
			half2 dir=	(half2(sin(_rot),cos(_rot)));//normalize
			half2 uv = v.texcoord;
			half2 offset= half2(-1*dir.x, dir.y)*_OffsetSize;
			for (int it = 0; it < 15; it++) {
				o.uv[it]=uv +  offset*it;
			}						 
			return o;
		}
		
		

		
		fixed4 frag(v2f i) : SV_Target {
			fixed iconshadow=0;
			fixed maxit=14;
			for (int it =14; it >=0; it--) {
				fixed shadowA = tex2D(_IconTex, i.uv[it]*_IconTex_ST-_IconTex_ST.zw).a ;
				if(iconshadow<shadowA){
					iconshadow=shadowA;
				}else{
					iconshadow*=_decay;
				}
			}			
			
			fixed4 sum = tex2D(_MainTex, i.uv[0]*_MainTex_ST-_MainTex_ST.zw).rgba;
			if(iconshadow>0){
				float3 shadowColor=sum*lerp(1,_ShadowColor.rgb,_ShadowColor.a);				
				float shadowst=lerp(1,1-_ShadowColor.a,iconshadow);
				sum.rgb=lerp(shadowColor,sum,shadowst);
			}
			fixed4 icon = tex2D(_IconTex, i.uv[0]*_IconTex_ST.xy-_IconTex_ST.zw).rgba ;
			sum.xyz = lerp (sum.xyz, icon.xyz, icon.www);
			return sum;
		}
	
	

		ENDCG
	
		
		Pass {
				
			//ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			  
			#pragma vertex vert  
			#pragma fragment frag
			  
			ENDCG  
		}
		

		
	} 
	FallBack "Diffuse"
}