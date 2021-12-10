/**
Genera un outline utilizando el operador Sobel.
El outline se calcula sobre la pantalla completa.
*/
Shader "Unlit/SobelOutlineScreen"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor("OutlineColor",Color) = (0,0,0,1)
        _Strength("Strength",Range(0,10)) = 1
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
            float4 _MainTex_TexelSize;
            fixed4 _OutlineColor;
            /** Textura con el depth buffer de la camara */
            sampler2D _CameraDepthTexture;
            int _Strength;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            /**
            Aplica una matriz de convolucion sobre los valores del depth buffer
            */
            fixed4 GetSobelDepth(sampler2D tex, float2 uv, float3x3 convolutionMat)
            {
                fixed4 color = fixed4(0,0,0,0);
                float offset = _MainTex_TexelSize.x * _Strength;

                color += tex2D(tex,float2(uv.x-offset, uv.y-offset)).r * convolutionMat[0][0];
                color += tex2D(tex,float2(uv.x, uv.y-offset)).r * convolutionMat[0][1];
                color += tex2D(tex,float2(uv.x+offset, uv.y+offset)).r * convolutionMat[0][2];
                
                color += tex2D(tex,float2(uv.x-offset, uv.y)).r * convolutionMat[1][0];
                color += tex2D(tex,uv).r * convolutionMat[1][1];
                color += tex2D(tex,float2(uv.x+offset, uv.y)).r * convolutionMat[1][2];
                
                color += tex2D(tex,float2(uv.x-offset, uv.y+offset)).r * convolutionMat[2][0];
                color += tex2D(tex,float2(uv.x, uv.y+offset)).r * convolutionMat[2][1];
                color += tex2D(tex,float2(uv.x+offset, uv.y+offset)).r * convolutionMat[2][2];
                
                color.a = 1;
                return color;
            }
            
            /**
            Retorna el color del Operador de sobel horizontal.
            */
            fixed4 GetSobelDepthHorizontal(sampler2D tex, float2 uv)
            {
                float3x3 convolutionMat = float3x3(
                    1,0,-1,
                    2,0,-2,
                    1,0,-1);
                return GetSobelDepth(tex,uv,convolutionMat);
            }

            /**
            Retorna el color del Operador de sobel horizontal.
            */
            fixed4 GetSobelDepthVertical(sampler2D tex, float2 uv)
            {
                float3x3 convolutionMat = float3x3(
                    1,2,1,
                    0,0,0,
                    -1,-2,-1);
                return GetSobelDepth(tex,uv,convolutionMat);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float depth = tex2D(_CameraDepthTexture,i.uv).r;
                // return depth;
                // return Linear01Depth(depth);
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 depthHor = GetSobelDepthHorizontal(_MainTex,i.uv);
                fixed4 depthVer = GetSobelDepthVertical(_MainTex,i.uv);
                float edge = sqrt(depthHor*depthHor + depthVer * depthVer);
                
                // return float4(edge,edge,edge,1);

                //interpolo entre el color original y el del outline para dibujar ambos.
                //se usa el valor de la convolucion como step para que pinte con el outline cuando este valor sea alto
                //(es decir, que haya un outline)
                return lerp(col,_OutlineColor,edge);
            }
            ENDCG
        }
    }
}
