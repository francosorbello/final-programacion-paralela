/**
Realiza de convolución de matrices sobre un objeto.
*/
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

            /**
            Data que recibe el vertex shader
            */
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            /**
            Data que recibe el Fragment shader
            */
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            /**
            vector con las dimensiones normalizadas de la textura.
            float4(1 / width, 1 / height, width, height)
            */
            float4 _MainTex_TexelSize;
            float4 _Color;
            float _Quantity;
            int _Strength;
            float4x4 _ConvolutionMatrix;

            /**
            Vertex shader.
            */
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            /**
            Realiza la convolucion para un punto.
            
            @uv - el punto con el que se esta trabajando
            @tex - textura de la que se extraen los colores
            @convolutionMat - matriz de convolucion a aplicar
            */
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

            /**
            Fragment shader
            */
            fixed4 frag (v2f i) : SV_Target
            {
                return Convolute(i.uv,_MainTex,_ConvolutionMatrix);
            }
            ENDCG
        }
    }
}
