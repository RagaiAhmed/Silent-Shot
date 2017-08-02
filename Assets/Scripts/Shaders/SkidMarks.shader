Shader "VehicleSystem/SkidMarks" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { 
			"Queue"="Transparent" 
    		"RenderType"="Transparent"
		}
		ZWrite Off
		Blend OneMinusDstColor One
		LOD 200
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:fade
        #pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
            float4 colorr: COLOR; 
		};

		void surf (Input IN, inout SurfaceOutputStandard o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex) * IN.colorr;
			o.Albedo = c.rgb * IN.colorr.rgb;;
			o.Alpha = c.a * IN.colorr.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
