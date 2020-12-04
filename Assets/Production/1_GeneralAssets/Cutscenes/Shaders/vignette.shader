Shader "Custom/Vignette" {
	Properties {
		_MainTex ("Base (RGBA)", 2D) = "white" {}
		_VignettePower ("VignettePower", Range(0.0,6.0)) = 1
    _TintColor("Color Tint", Color) = (0,0,0,1)
	}
	SubShader {
    Tags {"Queue"="Transparent" "RenderType"="Transparent"}

    Blend SrcAlpha OneMinusSrcAlpha

		Pass {

      CGPROGRAM
      #pragma vertex vert_img
      #pragma fragment frag
      #pragma fragmentoption ARB_precision_hint_fastest
      #include "UnityCG.cginc"

      uniform sampler2D _MainTex;
      uniform float _VignettePower;
      uniform fixed4 _TintColor;

      struct v2f {
        float2 texcoord	: TEXCOORD0;
      };
      
      float4 frag(v2f_img i) : COLOR {
        float4 renderTex = _TintColor * tex2D(_MainTex, i.uv);
        float2 dist = (i.uv - 0.5f) * 1.25f;
        dist.x = dot(dist, dist)  * _VignettePower;
        renderTex.a *= dist.x;
        return renderTex;
      }

      ENDCG
		} 
	}
}  