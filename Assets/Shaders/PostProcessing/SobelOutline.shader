Shader "Unlit/SobelOutline"
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


            fixed4 GetSobelDepth(sampler2D tex, float2 uv, float3x3 convolutionMat)
            {
                fixed4 color = fixed4(0,0,0,0);
                float offset = _MainTex_TexelSize.x * _Strength;

                color += Linear01Depth(tex2D(tex,float2(uv.x-offset, uv.y-offset)).r) * convolutionMat[0][0];
                color += Linear01Depth(tex2D(tex,float2(uv.x, uv.y-offset)).r) * convolutionMat[0][1];
                color += Linear01Depth(tex2D(tex,float2(uv.x+offset, uv.y+offset)).r) * convolutionMat[0][2];
                
                color += Linear01Depth(tex2D(tex,float2(uv.x-offset, uv.y)).r) * convolutionMat[1][0];
                color += Linear01Depth(tex2D(tex,uv).r) * convolutionMat[1][1];
                color += Linear01Depth(tex2D(tex,float2(uv.x+offset, uv.y)).r) * convolutionMat[1][2];
                
                color += Linear01Depth(tex2D(tex,float2(uv.x-offset, uv.y+offset)).r) * convolutionMat[2][0];
                color += Linear01Depth(tex2D(tex,float2(uv.x, uv.y+offset)).r) * convolutionMat[2][1];
                color += Linear01Depth(tex2D(tex,float2(uv.x+offset, uv.y+offset)).r) * convolutionMat[2][2];
                
                color.a = 1;
                return color;
            }
            
            fixed4 GetSobelDepthHorizontal(sampler2D tex, float2 uv)
            {
                float3x3 convolutionMat = float3x3(
                    1,0,-1,
                    2,0,-2,
                    1,0,-1);
                return GetSobelDepth(tex,uv,convolutionMat);
            }

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
                // return Linear01Depth(depth);
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 depthHor = GetSobelDepthHorizontal(_CameraDepthTexture,i.uv);
                fixed4 depthVer = GetSobelDepthVertical(_CameraDepthTexture,i.uv);
                float edge = sqrt(depthHor*depthHor + depthVer * depthVer);
                // return blend();
                return lerp(col,_OutlineColor,edge);
            }
            ENDCG
        }
    }
}
