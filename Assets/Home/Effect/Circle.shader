Shader "Custom/Circle"
{
    Properties
    {
        _Color("Outside Color", Color) = (0,0,0,1)
        _Center("Center (0..1)", Vector) = (0.5,0.5,0,0)
        _Radius("Radius (0..1)", Range(0,1)) = 0.25
        _Feather("Feather (soft edge)", Range(0,0.5)) = 0.02
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appv {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _Color;
            float4 _Center;
            float _Radius;
            float _Feather;

            v2f vert(appv v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // UV assumed 0..1 across screen/fullscreen quad
                float2 uv = i.uv;
                float2 center = _Center.xy;
                float d = distance(uv, center);

                // smoothstep edge: inside circle -> 0 alpha, outside -> 1 alpha
                float edge0 = max(_Radius - _Feather, 0.0);
                float edge1 = _Radius;
                float outsideAlpha = smoothstep(edge0, edge1, d);

                // final color: outside colored, inside transparent
                fixed3 rgb = _Color.rgb;
                fixed a = _Color.a * outsideAlpha;

                return fixed4(rgb, a);
            }
            ENDHLSL
        }
    }

    FallBack Off
}
