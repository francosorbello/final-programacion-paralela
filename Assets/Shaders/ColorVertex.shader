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
            float4 _Color;
            float _Quantity;
            int _Strength;
            float4x4 _ConvolutionMatrix;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float3x3 CreateBlurMatrix(){
                float3x3 BLUR_MATRIX;
                BLUR_MATRIX[0][0] = 0.0625;
                BLUR_MATRIX[0][1] = 0.125;
                BLUR_MATRIX[0][2] = 0.0625;
                
                BLUR_MATRIX[1][0] = 0.125;
                BLUR_MATRIX[1][1] = 0.25;
                BLUR_MATRIX[1][2] = 0.125;
                
                BLUR_MATRIX[2][0] = 0.0625;
                BLUR_MATRIX[2][1] = 0.125;
                BLUR_MATRIX[2][2] = 0.0625;

                return BLUR_MATRIX;
            }

            float3x3 CreateBoxBlur(){
                float3x3 BLUR_MATRIX;
                BLUR_MATRIX[0][0] = 1;
                BLUR_MATRIX[0][1] = 1;
                BLUR_MATRIX[0][2] = 1;
                
                BLUR_MATRIX[1][0] = 1;
                BLUR_MATRIX[1][1] = 1;
                BLUR_MATRIX[1][2] = 1;
                
                BLUR_MATRIX[2][0] = 1;
                BLUR_MATRIX[2][1] = 1;
                BLUR_MATRIX[2][2] = 1;

                return BLUR_MATRIX / 9;
            }

            float3x3 CreateOutilineMatrix(){
                float3x3 BLUR_MATRIX;
                BLUR_MATRIX[0][0] = -1;
                BLUR_MATRIX[0][1] = -1;
                BLUR_MATRIX[0][2] = -1;
                
                BLUR_MATRIX[1][0] = -1;
                BLUR_MATRIX[1][1] = 8;
                BLUR_MATRIX[1][2] = -1;
                
                BLUR_MATRIX[2][0] = -1;
                BLUR_MATRIX[2][1] = -1;
                BLUR_MATRIX[2][2] = -1;
                
                return BLUR_MATRIX;
            }

            float3x3 GetAdjacentValues(float2 uv)
            {
                float3x3 adjacent;
                float offset = _MainTex_TexelSize.x * _Strength;
                adjacent[0][0] = tex2D(_MainTex,float2(uv.x-offset, uv.y-offset));
                adjacent[0][1] = tex2D(_MainTex,float2(uv.x, uv.y-offset));
                adjacent[0][2] = tex2D(_MainTex,float2(uv.x+offset, uv.y+offset));
                
                adjacent[1][0] = tex2D(_MainTex,float2(uv.x-offset, uv.y));
                adjacent[1][1] = tex2D(_MainTex,float2(uv.x, uv.y));
                adjacent[1][2] = tex2D(_MainTex,float2(uv.x+offset, uv.y));
                
                adjacent[2][0] = tex2D(_MainTex,float2(uv.x-offset, uv.y+offset));
                adjacent[2][1] = tex2D(_MainTex,float2(uv.x, uv.y+offset));
                adjacent[2][2] = tex2D(_MainTex,float2(uv.x+offset, uv.y+offset));

                adjacent = adjacent * _Quantity;

                return adjacent;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3x3 blurMatrix = CreateBoxBlur();
                float3x3 adjacent = GetAdjacentValues(i.uv);
                // sample the texture
                // fixed4 col = tex2D(_MainTex, i.uv);
                // return tex2D(_MainTex,i.uv);
                fixed4 originalColor = tex2D(_MainTex,i.uv);
                fixed4 col = fixed4(0,0,0,0) + tex2D(_MainTex,i.uv) * (1-_Quantity);
                for(int i = 0; i < 3; i++)
                {
                    for(int j = 0; j < 3; j++)
                    {
                        col += adjacent[i][j] * _ConvolutionMatrix[i][j];
                    }
                }
                col.a = 1;
                col *= originalColor * _Color;

                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
