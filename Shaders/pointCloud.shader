Shader "Unlit/pointCloud"
{
    Properties
    {
        _Size ("Size", Range(0, 3)) = 0.03 //patch size
        _Theta ("Theta", Range(0, 3.1415)) = 0 //orientation
		    _Gamma ("Gamma", Range(0, 6)) =2.41 //squareness
		    _SigmaX ("SigmaX", Range(-3, 3)) = 0.10 //x falloff
		    _SigmaY ("SigmaY", Range(-3, 3)) = 0.10 //y falloff
		    _Alpha ("Alpha", Range(0,10)) = 0.015 //amplitude

    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
			
        Cull Off // render both back and front faces
        Blend Off 

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
            };

            struct v2g
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
            };


            struct g2f
            {
                float4 pos : SV_POSITION;
                float2 tex0 : TEXCOORD0;
                float4 color : COLOR;
            };

            float _Size;


            v2g vert (appdata v)
            {
                v2g o;
                o.pos = v.vertex;
                o.color = v.color;

               
                return o;
            }

            [maxvertexcount(4)]
            void geom(point v2g p[1], inout TriangleStream<g2f> triStream)
            {
                
                            
                float3 up =UNITY_MATRIX_IT_MV[1].xyz;
                float3 right = UNITY_MATRIX_IT_MV[0].xyz;
                up = normalize(up);
                right = normalize(right);

                float halfS = 0.5f * _Size;

                
                        
                float4 v[4];
                v[0] = float4(p[0].pos + halfS * right - halfS * up, 1.0f);
                v[1] = float4(p[0].pos + halfS * right + halfS * up, 1.0f);
                v[2] = float4(p[0].pos - halfS * right - halfS * up, 1.0f);
                v[3] = float4(p[0].pos - halfS * right + halfS * up, 1.0f);

                //float4 vp = UnityObjectToClipPos(unity_WorldToObject);
                g2f pIn;
                pIn.pos = UnityObjectToClipPos(v[0]);
                pIn.tex0 = float2(1, 0);
                pIn.color = p[0].color;
                triStream.Append(pIn);

                pIn.pos = UnityObjectToClipPos(v[1]);
                pIn.tex0 = float2(1, 1);
                pIn.color = p[0].color;
                triStream.Append(pIn);

                pIn.pos = UnityObjectToClipPos(v[2]);
                pIn.tex0 = float2(0, 0);
                pIn.color = p[0].color;
                triStream.Append(pIn);

                pIn.pos = UnityObjectToClipPos(v[3]);
                pIn.tex0 = float2(0, 1);
                pIn.color = p[0].color;
                triStream.Append(pIn);
                
            }


            float _Dev;
				float _Gamma;
				float _SigmaX;
				float _SigmaY;
				float _Alpha;
				float _Theta;

				float gaussianTheta(float x, float x0, float y,float y0, float a, float sigmax, float sigmay, float gamma,float theta){
				

					float x2 = cos(theta)*(x-x0)-sin(theta)*(y-y0)+x0;
					float y2 = sin(theta)*(x-x0)+cos(theta)*(y-y0)+y0;
					float z2 = a*exp(-0.5*( pow(pow((x2-x0)/sigmax,2),gamma/2)))*exp(-0.5*( pow(pow((y2-y0)/sigmay,2),gamma/2)));
					return z2;
				}


            fixed4 frag (g2f i) : SV_Target
            {
                // sample the texture
                // Comment this bit out if you don't want the fancy extra stuff like change orientation
                float alpha =gaussianTheta(i.tex0.x,0.5,i.tex0.y,0.5,_Alpha,_SigmaX,_SigmaY,_Gamma,_Theta);
                
                alpha = alpha>1? 1:alpha;
                alpha = alpha < 0.05? 0:alpha;
                float4 t = float4(1.0f,1.0f,1.0f,alpha);
                if(t.a ==0 )
                    discard;
                t = t*i.color;
                // Comment out up to here


                return i.color;
            }
            ENDCG
        }
    }
}
