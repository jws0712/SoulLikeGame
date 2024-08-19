Shader "ShaderPractice/Default"
{
    Properties //프로퍼티
    {
        _BaseMap ("Texture", 2D) = "white" {}
        _Threshould("Threshould" , float) = 0
        _ShadowColor("ShadowColor", Color) = (1,1,1,1)
        _OutLineWidth("OutLineWidth", float) = 0
    }


    SubShader //셰이더 정의
    {
        Tags 
        { 
            
            "RenderPipeline" = "UniversalPipeline" // <-
            "UniversalMaterialType" = "Lit"
            "RenderType"="Opaque" 
            "Queue" = "Geometry+0" //3차원

            //주의 !! 이렇게 안나옴 테그 다 수정해야함
            //URP로 셰이더 쓰려면 Tag URP꼭 써야함
        }

        Pass
        {
            Name "ToonRendering"
            Tags
            {
                "LightMode" = "UniversalForward" //물체그리기
            }
            cull back       //뒷면 컬링
                            //cull front 앞면 컬링
            HLSLPROGRAM     //따로 제작

            //Receive Shadow

            //라이팅 전처리
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            
            #pragma vertex vert     //버택스 셰이더 함수 정의
            #pragma fragment frag   //픽셀 셰이더 함수 정의

            //인클루드
            //필요한 함수정의
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"

            CBUFFER_START(UnityPerMaterial)
            
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            half4 _BaseMap_ST;
            float _Threshould;
            float4 _ShadowColor;
            float _OutLineWidth;
            
            CBUFFER_END //최적화

            //버텍스 셰이더 구조체
            struct appdata
            {
                float4 vertex : POSITION;        //앞 이름 뒤 중요함 (모델스페이스 포지션)
                float2 uv : TEXCOORD0;           //UV좌표 가져옴
                float3 normal : NORMAL;          //버텍스 노말백터

                UNITY_VERTEX_INPUT_INSTANCE_ID   //URP맴버 최적화
            };

            //픽셀 셰이더 구조체
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                //URP문법
                
                o.vertex = TransformObjectToHClip(v.vertex.xyz);     //Model -> World -> View -> Projection 단계를 거침
                                                                     //반대 가능 (역행렬)
                o.uv = TRANSFORM_TEX(v.uv, _BaseMap);
                o.normal = TransformObjectToWorldNormal(v.normal);   

                return o;
            }

            
            half4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                Light mainLight = GetMainLight(float4(0,0,0,0));

                float4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);

                float NdotL = dot(mainLight.direction, i.normal);

                //NdotL = step(NdotL, _Threshould);

                NdotL = ceil(NdotL * 3) / 3;

                col.rgb = lerp(col.rgb, _ShadowColor.rgb, 1-NdotL);

                return half4(col.rgb, 1);
            }   


            //dot : 내적
            //dot(조명 방향, 원의 노말백터)
            //평행 1 (어두움)
            //마주봄 -1 (밝음)
            // -1 ~ 1
            //밝음 -> 어두움
            //x * 0.5f + 0.5f

            //step(x,y)
            //x < y = 1 , x > y = -1
            ENDHLSL
        }
        Pass
        {
            Name "OutLine"
            cull front       //앞면 컬링
                            //cull front 앞면 컬링
            HLSLPROGRAM     //따로 제작

            //Receive Shadow

            //라이팅 전처리
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            
            #pragma vertex vert     //버택스 셰이더 함수 정의
            #pragma fragment frag   //픽셀 셰이더 함수 정의

            //인클루드
            //필요한 함수정의
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"

            CBUFFER_START(UnityPerMaterial)
            
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            half4 _BaseMap_ST;
            float _Threshould;
            float4 _ShadowColor;
            float _OutLineWidth;
            
            CBUFFER_END //최적화

            //버텍스 셰이더 구조체
            struct appdata
            {
                float4 vertex : POSITION;        //앞 이름 뒤 중요함 (모델스페이스 포지션)
                float2 uv : TEXCOORD0;           //UV좌표 가져옴
                float3 normal : NORMAL;          //버텍스 노말백터

                UNITY_VERTEX_INPUT_INSTANCE_ID   //URP맴버 최적화
            };

            //픽셀 셰이더 구조체
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                //URP문법
                
                o.vertex = TransformObjectToHClip(v.vertex.xyz + v.normal * _OutLineWidth);     //Model -> World -> View -> Projection 단계를 거침
                                                                     //반대 가능 (역행렬)
                o.uv = TRANSFORM_TEX(v.uv, _BaseMap);
                o.normal = TransformObjectToWorldNormal(v.normal);   

                return o;
            }

            
            half4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                Light mainLight = GetMainLight(float4(0,0,0,0));

                float4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);

                return half4(0, 0, 0, 1);
            }   


            //dot : 내적
            //dot(조명 방향, 원의 노말백터)
            //평행 1 (어두움)
            //마주봄 -1 (밝음)
            // -1 ~ 1
            //밝음 -> 어두움
            //x * 0.5f + 0.5f

            //step(x,y)
            //x < y = 1 , x > y = -1
            ENDHLSL
        }
    }
}
