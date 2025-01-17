Shader "Unlit/CharacterOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _EnableOutline ("EnableOutline",float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100
        
        Pass
        {
            Stencil{
                Ref 2
                Comp Always
                Pass Replace
            }
            ZTest Always
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 col : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 col : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _EnableOutline;
            v2f vert (appdata v)
            {
                v2f o;
                o.col = v.col;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col22 = tex2D(_MainTex, i.uv);
                fixed offset = 0.04;
                fixed4 col11 = tex2D(_MainTex, i.uv+(-offset,offset));
                fixed4 col12 = tex2D(_MainTex, i.uv+(0,offset));
                fixed4 col13 = tex2D(_MainTex, i.uv+(offset,offset));
                fixed4 col21 = tex2D(_MainTex, i.uv+(-offset,0));
                fixed4 col23 = tex2D(_MainTex, i.uv+(offset,0));
                fixed4 col31 = tex2D(_MainTex, i.uv+(-offset,-offset));
                fixed4 col32 = tex2D(_MainTex, i.uv+(0,-offset));
                fixed4 col33 = tex2D(_MainTex, i.uv+(offset,-offset));
                fixed4 bluredcol = 0.5*(col22*0.147 + (col12+col21+col23+col32)*0.118+(col11+col13+col31+col33)*0.094);
                bluredcol +=  bluredcol*clamp(sin(i.vertex.y*0.1+_Time.y),0,1);
                bluredcol = bluredcol * _EnableOutline;
                fixed4 col = pow(i.col,2.1) * col22;
                return col.a > 0.1 ? col : bluredcol;
            }
            ENDCG
        }
    }
}
