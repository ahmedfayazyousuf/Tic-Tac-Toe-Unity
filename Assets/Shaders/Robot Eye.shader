Shader "Unlit/Robot Eye"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Main Color", Color) = (0.043, 0, 1, 1)
        _GlowColor("Glow Color", Color) = (0.29, 0, 1, 1)
        _GlowPower("Glow Power", Float) = 40
        _Radius ("Radius", Range(0, 1)) = 0.128
        _Height("Height", Range(0, 1)) = 0.404
        _Blend("Blend", Range(0, 1)) = 0.01
        _Thickness("Thickness", Range(0, 1)) = 0.016
        _Pos("Position", Vector) = (-0.4, 0.2, 0, 0)
        _Ratio ("Ratio", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _Color, _GlowColor;
            float _GlowPower;
            float _Radius, _Height;
            float _Thickness, _Blend;
            float4 _Pos;
            float _Ratio;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float LineDist(float2 p, float2 p1, float2 p2)
            {
                float t = clamp(dot(p2 - p1, p - p1) / dot(p2 - p1, p2 - p1), 0, 1);
                return length(p - ((1 - t) * p1 + p2 * t));
            }

            float sdEllipse(float2 p, float2 ab)
            {
                if( ab.x==ab.y ) return length(p)-ab.x;

                p = abs(p);
                if (p.x > p.y) { p = p.yx; ab = ab.yx; }

                float l = ab.y * ab.y - ab.x * ab.x;

                float m = ab.x * p.x / l;
                float n = ab.y * p.y / l;
                float m2 = m * m;
                float n2 = n * n;

                float c = (m2 + n2 - 1.0) / 3.0;
                float c3 = c * c * c;

                float d = c3 + m2 * n2;
                float q = d + m2 * n2;
                float g = m + m * n2;

                float co;

                if (d < 0.0)
                {
                    float h = acos(q / c3) / 3.0;
                    float s = cos(h) + 2.0;
                    float t = sin(h) * sqrt(3.0);
                    float rx = sqrt(m2 - c * (s + t));
                    float ry = sqrt(m2 - c * (s - t));
                    co = ry + sign(l) * rx + abs(g) / (rx * ry);
                }
                else
                {
                    float h = 2.0 * m * n * sqrt(d);
                    float s = sign(q + h) * pow(abs(q + h), 1.0 / 3.0);
                    float t = sign(q - h) * pow(abs(q - h), 1.0 / 3.0);
                    float rx = -(s + t) - c * 4.0 + 2.0 * m2;
                    float ry = (s - t) * sqrt(3.0);
                    float rm = sqrt(rx * rx + ry * ry);
                    co = ry / sqrt(rm - rx) + 2.0 * g / rm;
                }
                co = (co - m) / 2.0;

                float si = sqrt(max(1.0 - co * co, 0.0));

                float2 r = ab * float2(co, si);

                return length(r - p) * sign(p.y - r.y);
            }

            fixed4 RobotEye(float2 uv)
            {
                fixed4 col = 0;

                float r = _Radius;
                float h = _Height;
                float glow = _Blend;
                float thick = _Thickness;
                float ed1 = 0, ed2 = 0;

                float minimumHeight = r - 0.01;
                h = _Height * (1 + minimumHeight) - minimumHeight;

                float h1 = +max(0, h);
                float h2 = r + min(0, h);

                float d1 = LineDist(uv, float2(0, -h1 * 0.5), float2(0, h1 * 0.5)) - r;
                ed1 = d1;
                d1 = smoothstep(thick, thick - glow, abs(d1));

                float d2 = sdEllipse(uv, float2(r, h2));
                ed2 = d2;
                d2 = smoothstep(thick, thick - glow, abs(d2));

                float d = h >= 0 ? d1 : d2;
                float emission = h >= 0 ? ed1 : ed2;

                col = pow(1 - abs(emission), _GlowPower) * _GlowColor;
                col = lerp(col, _Color, d);

                return col;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv - 0.5;
                uv.x *= _Ratio;

                fixed4 col;

                uv.x = abs(uv.x);
                col = RobotEye(uv * _Pos.z - _Pos.xy);

                return col;
            }
            ENDCG
        }
    }
}