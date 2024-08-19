Shader "ShaderPractice/Default"
{
    Properties //������Ƽ
    {
        _BaseMap ("Texture", 2D) = "white" {}
        _Threshould("Threshould" , float) = 0
        _ShadowColor("ShadowColor", Color) = (1,1,1,1)
        _OutLineWidth("OutLineWidth", float) = 0
    }


    SubShader //���̴� ����
    {
        Tags 
        { 
            
            "RenderPipeline" = "UniversalPipeline" // <-
            "UniversalMaterialType" = "Lit"
            "RenderType"="Opaque" 
            "Queue" = "Geometry+0" //3����

            //���� !! �̷��� �ȳ��� �ױ� �� �����ؾ���
            //URP�� ���̴� ������ Tag URP�� �����
        }

        Pass
        {
            Name "ToonRendering"
            Tags
            {
                "LightMode" = "UniversalForward" //��ü�׸���
            }
            cull back       //�޸� �ø�
                            //cull front �ո� �ø�
            HLSLPROGRAM     //���� ����

            //Receive Shadow

            //������ ��ó��
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            
            #pragma vertex vert     //���ý� ���̴� �Լ� ����
            #pragma fragment frag   //�ȼ� ���̴� �Լ� ����

            //��Ŭ���
            //�ʿ��� �Լ�����
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
            
            CBUFFER_END //����ȭ

            //���ؽ� ���̴� ����ü
            struct appdata
            {
                float4 vertex : POSITION;        //�� �̸� �� �߿��� (�𵨽����̽� ������)
                float2 uv : TEXCOORD0;           //UV��ǥ ������
                float3 normal : NORMAL;          //���ؽ� �븻����

                UNITY_VERTEX_INPUT_INSTANCE_ID   //URP�ɹ� ����ȭ
            };

            //�ȼ� ���̴� ����ü
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

                //URP����
                
                o.vertex = TransformObjectToHClip(v.vertex.xyz);     //Model -> World -> View -> Projection �ܰ踦 ��ħ
                                                                     //�ݴ� ���� (�����)
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


            //dot : ����
            //dot(���� ����, ���� �븻����)
            //���� 1 (��ο�)
            //���ֺ� -1 (����)
            // -1 ~ 1
            //���� -> ��ο�
            //x * 0.5f + 0.5f

            //step(x,y)
            //x < y = 1 , x > y = -1
            ENDHLSL
        }
        Pass
        {
            Name "OutLine"
            cull front       //�ո� �ø�
                            //cull front �ո� �ø�
            HLSLPROGRAM     //���� ����

            //Receive Shadow

            //������ ��ó��
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            
            #pragma vertex vert     //���ý� ���̴� �Լ� ����
            #pragma fragment frag   //�ȼ� ���̴� �Լ� ����

            //��Ŭ���
            //�ʿ��� �Լ�����
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
            
            CBUFFER_END //����ȭ

            //���ؽ� ���̴� ����ü
            struct appdata
            {
                float4 vertex : POSITION;        //�� �̸� �� �߿��� (�𵨽����̽� ������)
                float2 uv : TEXCOORD0;           //UV��ǥ ������
                float3 normal : NORMAL;          //���ؽ� �븻����

                UNITY_VERTEX_INPUT_INSTANCE_ID   //URP�ɹ� ����ȭ
            };

            //�ȼ� ���̴� ����ü
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

                //URP����
                
                o.vertex = TransformObjectToHClip(v.vertex.xyz + v.normal * _OutLineWidth);     //Model -> World -> View -> Projection �ܰ踦 ��ħ
                                                                     //�ݴ� ���� (�����)
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


            //dot : ����
            //dot(���� ����, ���� �븻����)
            //���� 1 (��ο�)
            //���ֺ� -1 (����)
            // -1 ~ 1
            //���� -> ��ο�
            //x * 0.5f + 0.5f

            //step(x,y)
            //x < y = 1 , x > y = -1
            ENDHLSL
        }
    }
}
