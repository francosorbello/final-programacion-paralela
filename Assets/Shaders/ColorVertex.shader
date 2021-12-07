Shader "Unlit/Color Vertex"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Quantity("Quantity",Range(0,1)) = 0
        _Strength("Strength",Range(1,4)) = 1
        _Color("Color",Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma enable_d3d11_debug_symbols


            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float4 _Color;
            float _Quantity;
            int _Strength;
            float4x4 _ConvolutionMatrix;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 Convolute(float2 uv,sampler2D tex, float4x4 convolutionMat)
            {
                fixed4 color = fixed4(0,0,0,0);
                float offset = _MainTex_TexelSize.x * _Strength;

                color += tex2D(tex,float2(uv.x-offset, uv.y-offset)) * convolutionMat[0][0];
                color += tex2D(tex,float2(uv.x, uv.y-offset)) * convolutionMat[0][1];
                color += tex2D(tex,float2(uv.x+offset, uv.y+offset)) * convolutionMat[0][2];
                
                color += tex2D(tex,float2(uv.x-offset, uv.y)) * convolutionMat[1][0];
                color += tex2D(tex,uv) * convolutionMat[1][1];
                color += tex2D(tex,float2(uv.x+offset, uv.y)) * convolutionMat[1][2];
                
                color += tex2D(tex,float2(uv.x-offset, uv.y+offset)) * convolutionMat[2][0];
                color += tex2D(tex,float2(uv.x, uv.y+offset)) * convolutionMat[2][1];
                color += tex2D(tex,float2(uv.x+offset, uv.y+offset)) * convolutionMat[2][2];
                
                color.a = 1;

                return color;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return Convolute(i.uv,_MainTex,_ConvolutionMatrix);
            }
            ENDCG
        }
    }
}
