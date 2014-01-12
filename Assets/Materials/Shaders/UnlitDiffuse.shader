Shader "Unlit/Diffuse" {
	Properties 
	{
		_Color ("Color", Color) = (0.5,0.5,0.5,0.5)
	}

SubShader {
	Tags { "Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="Opaque" }
	Blend SrcAlpha OneMinusSrcAlpha
	Cull Off Lighting Off ZTest LEqual ZWrite On Fog { Color (0,0,0,0) }
	
	pass {
	
            CGPROGRAM 
			#pragma vertex vert
            #pragma fragment frag 
            #pragma fragmentoption ARB_precision_hint_fastest 

            #include "UnityCG.cginc"    

			struct appdata {
    			float4 vertex : POSITION;
    		};

            struct v2f { 

               float4 pos : SV_POSITION;

            };
			
			v2f vert (appdata v) { 
				v2f o; 
				o.pos = mul( UNITY_MATRIX_MVP, v.vertex );
				return o; 
			}
			
			uniform float4 _Color;

            fixed4 frag (v2f i) : COLOR { 

				return fixed4( _Color );
				
            } 

            ENDCG 
            
		}
	} 
}