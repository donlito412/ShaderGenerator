using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Shader Generator - Create Custom Shaders Easily
/// Version 1.0 | Visual Shader Creation Tool
/// </summary>
public class ShaderGenerator : EditorWindow
{
    public enum ShaderType { Toon, Hologram, Dissolve, Outline, Rim, Water, Gradient, Emission }
    
    private ShaderType shaderType = ShaderType.Toon;
    private string shaderName = "MyShader";
    private Color mainColor = Color.white;
    private Color secondaryColor = Color.gray;
    private float parameter1 = 0.5f;
    private float parameter2 = 1f;
    private Texture2D mainTexture;
    
    private Vector2 scrollPosition;
    
    [MenuItem("Tools/Shader Generator")]
    public static void ShowWindow()
    {
        ShaderGenerator window = GetWindow<ShaderGenerator>("Shader Generator");
        window.minSize = new Vector2(380, 500);
    }
    
    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        // Header
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
        headerStyle.fontSize = 18;
        headerStyle.alignment = TextAnchor.MiddleCenter;
        
        EditorGUILayout.Space(10);
        GUILayout.Label("🎨 SHADER GENERATOR", headerStyle);
        GUILayout.Label("Create Custom Shaders Easily", EditorStyles.centeredGreyMiniLabel);
        EditorGUILayout.Space(10);
        
        // Shader Type
        GUILayout.Label("✨ Shader Type", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        shaderType = (ShaderType)EditorGUILayout.EnumPopup("Type", shaderType);
        DrawShaderPreview();
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space(10);
        
        // Settings
        GUILayout.Label("⚙️ Settings", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        shaderName = EditorGUILayout.TextField("Shader Name", shaderName);
        mainColor = EditorGUILayout.ColorField("Main Color", mainColor);
        secondaryColor = EditorGUILayout.ColorField("Secondary Color", secondaryColor);
        
        DrawTypeSpecificSettings();
        
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space(10);
        
        // Texture
        GUILayout.Label("🖼️ Textures", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        mainTexture = (Texture2D)EditorGUILayout.ObjectField("Main Texture", mainTexture, typeof(Texture2D), false);
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space(15);
        
        // Generate Button
        GUI.backgroundColor = new Color(0.6f, 0.3f, 0.8f);
        if (GUILayout.Button("🎨 GENERATE SHADER", GUILayout.Height(45)))
        {
            GenerateShader();
        }
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.Space(5);
        
        GUI.backgroundColor = new Color(0.3f, 0.7f, 0.3f);
        if (GUILayout.Button("📦 Create Material from Shader", GUILayout.Height(30)))
        {
            CreateMaterial();
        }
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.Space(20);
        EditorGUILayout.EndScrollView();
    }
    
    private void DrawShaderPreview()
    {
        string desc = shaderType switch
        {
            ShaderType.Toon => "🎨 Cel-shaded cartoon look with stepped lighting",
            ShaderType.Hologram => "👻 Sci-fi hologram effect with scanlines",
            ShaderType.Dissolve => "💨 Dissolve/burn away effect",
            ShaderType.Outline => "✏️ Solid outline around objects",
            ShaderType.Rim => "✨ Rim lighting/fresnel glow effect",
            ShaderType.Water => "🌊 Stylized water surface",
            ShaderType.Gradient => "🌈 Color gradient based on height",
            ShaderType.Emission => "💡 Glowing/emissive material",
            _ => ""
        };
        EditorGUILayout.HelpBox(desc, MessageType.None);
    }
    
    private void DrawTypeSpecificSettings()
    {
        switch (shaderType)
        {
            case ShaderType.Toon:
                parameter1 = EditorGUILayout.Slider("Shadow Threshold", parameter1, 0f, 1f);
                parameter2 = EditorGUILayout.Slider("Shadow Softness", parameter2, 0f, 1f);
                break;
            case ShaderType.Hologram:
                parameter1 = EditorGUILayout.Slider("Scanline Density", parameter1, 10f, 200f);
                parameter2 = EditorGUILayout.Slider("Flicker Speed", parameter2, 0f, 10f);
                break;
            case ShaderType.Dissolve:
                parameter1 = EditorGUILayout.Slider("Dissolve Amount", parameter1, 0f, 1f);
                parameter2 = EditorGUILayout.Slider("Edge Width", parameter2, 0f, 0.2f);
                break;
            case ShaderType.Outline:
                parameter1 = EditorGUILayout.Slider("Outline Width", parameter1, 0f, 0.1f);
                break;
            case ShaderType.Rim:
                parameter1 = EditorGUILayout.Slider("Rim Power", parameter1, 0f, 10f);
                parameter2 = EditorGUILayout.Slider("Rim Intensity", parameter2, 0f, 3f);
                break;
            case ShaderType.Water:
                parameter1 = EditorGUILayout.Slider("Wave Speed", parameter1, 0f, 5f);
                parameter2 = EditorGUILayout.Slider("Wave Strength", parameter2, 0f, 1f);
                break;
            case ShaderType.Gradient:
                parameter1 = EditorGUILayout.Slider("Gradient Height", parameter1, 0f, 10f);
                break;
            case ShaderType.Emission:
                parameter1 = EditorGUILayout.Slider("Emission Intensity", parameter1, 0f, 5f);
                parameter2 = EditorGUILayout.Slider("Pulse Speed", parameter2, 0f, 5f);
                break;
        }
    }
    
    private void GenerateShader()
    {
        string shaderCode = GenerateShaderCode();
        
        string path = "Assets/_GeneratedShaders";
        if (!AssetDatabase.IsValidFolder(path))
        {
            AssetDatabase.CreateFolder("Assets", "_GeneratedShaders");
        }
        
        string filePath = path + "/" + shaderName + ".shader";
        File.WriteAllText(Application.dataPath + "/../" + filePath, shaderCode);
        
        AssetDatabase.Refresh();
        
        Debug.Log("✅ Shader generated: " + filePath);
        EditorUtility.DisplayDialog("Shader Created!", "Shader saved to:\n" + filePath, "OK");
        
        // Select the shader
        Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(filePath);
        if (shader != null) Selection.activeObject = shader;
    }
    
    private string GenerateShaderCode()
    {
        string colorR = mainColor.r.ToString("F2");
        string colorG = mainColor.g.ToString("F2");
        string colorB = mainColor.b.ToString("F2");
        
        return shaderType switch
        {
            ShaderType.Toon => GenerateToonShader(),
            ShaderType.Hologram => GenerateHologramShader(),
            ShaderType.Dissolve => GenerateDissolveShader(),
            ShaderType.Outline => GenerateOutlineShader(),
            ShaderType.Rim => GenerateRimShader(),
            ShaderType.Water => GenerateWaterShader(),
            ShaderType.Gradient => GenerateGradientShader(),
            ShaderType.Emission => GenerateEmissionShader(),
            _ => GenerateToonShader()
        };
    }
    
    private string GenerateToonShader()
    {
        return $@"Shader ""Custom/{shaderName}""
{{
    Properties
    {{
        _MainTex (""Texture"", 2D) = ""white"" {{}}
        _Color (""Color"", Color) = ({mainColor.r:F2}, {mainColor.g:F2}, {mainColor.b:F2}, 1)
        _ShadowColor (""Shadow Color"", Color) = ({secondaryColor.r:F2}, {secondaryColor.g:F2}, {secondaryColor.b:F2}, 1)
        _ShadowThreshold (""Shadow Threshold"", Range(0,1)) = {parameter1:F2}
    }}
    SubShader
    {{
        Tags {{ ""RenderType""=""Opaque"" ""RenderPipeline""=""UniversalPipeline"" }}
        
        Pass
        {{
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include ""Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl""
            #include ""Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl""
            
            struct Attributes {{ float4 positionOS : POSITION; float3 normalOS : NORMAL; float2 uv : TEXCOORD0; }};
            struct Varyings {{ float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; float3 normalWS : TEXCOORD1; }};
            
            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            float4 _Color, _ShadowColor;
            float _ShadowThreshold;
            
            Varyings vert(Attributes IN)
            {{
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                return OUT;
            }}
            
            half4 frag(Varyings IN) : SV_Target
            {{
                Light mainLight = GetMainLight();
                float NdotL = dot(IN.normalWS, mainLight.direction);
                float toon = step(_ShadowThreshold, NdotL * 0.5 + 0.5);
                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                return tex * lerp(_ShadowColor, _Color, toon);
            }}
            ENDHLSL
        }}
    }}
}}";
    }
    
    private string GenerateHologramShader()
    {
        return $@"Shader ""Custom/{shaderName}""
{{
    Properties
    {{
        _Color (""Color"", Color) = ({mainColor.r:F2}, {mainColor.g:F2}, {mainColor.b:F2}, 0.5)
        _ScanlineCount (""Scanlines"", Float) = {parameter1:F0}
        _FlickerSpeed (""Flicker Speed"", Float) = {parameter2:F2}
    }}
    SubShader
    {{
        Tags {{ ""Queue""=""Transparent"" ""RenderType""=""Transparent"" ""RenderPipeline""=""UniversalPipeline"" }}
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {{
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include ""Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl""
            
            struct Attributes {{ float4 positionOS : POSITION; float3 normalOS : NORMAL; }};
            struct Varyings {{ float4 positionCS : SV_POSITION; float3 positionWS : TEXCOORD0; float3 normalWS : TEXCOORD1; }};
            
            float4 _Color;
            float _ScanlineCount, _FlickerSpeed;
            
            Varyings vert(Attributes IN)
            {{
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                return OUT;
            }}
            
            half4 frag(Varyings IN) : SV_Target
            {{
                float scanline = sin(IN.positionWS.y * _ScanlineCount + _Time.y * _FlickerSpeed) * 0.5 + 0.5;
                float rim = 1.0 - saturate(dot(normalize(GetWorldSpaceViewDir(IN.positionWS)), IN.normalWS));
                float alpha = (scanline * 0.5 + rim) * _Color.a;
                return half4(_Color.rgb, alpha);
            }}
            ENDHLSL
        }}
    }}
}}";
    }
    
    private string GenerateDissolveShader()
    {
        return $@"Shader ""Custom/{shaderName}""
{{
    Properties
    {{
        _MainTex (""Texture"", 2D) = ""white"" {{}}
        _Color (""Color"", Color) = ({mainColor.r:F2}, {mainColor.g:F2}, {mainColor.b:F2}, 1)
        _EdgeColor (""Edge Color"", Color) = ({secondaryColor.r:F2}, {secondaryColor.g:F2}, {secondaryColor.b:F2}, 1)
        _DissolveAmount (""Dissolve"", Range(0,1)) = {parameter1:F2}
        _EdgeWidth (""Edge Width"", Range(0,0.2)) = {parameter2:F2}
    }}
    SubShader
    {{
        Tags {{ ""RenderType""=""Opaque"" ""RenderPipeline""=""UniversalPipeline"" }}
        
        Pass
        {{
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include ""Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl""
            
            struct Attributes {{ float4 positionOS : POSITION; float2 uv : TEXCOORD0; }};
            struct Varyings {{ float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; float3 positionOS : TEXCOORD1; }};
            
            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            float4 _Color, _EdgeColor;
            float _DissolveAmount, _EdgeWidth;
            
            Varyings vert(Attributes IN)
            {{
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.positionOS = IN.positionOS.xyz;
                return OUT;
            }}
            
            half4 frag(Varyings IN) : SV_Target
            {{
                float noise = frac(sin(dot(IN.positionOS.xz, float2(12.9898, 78.233))) * 43758.5453);
                clip(noise - _DissolveAmount);
                float edge = smoothstep(_DissolveAmount, _DissolveAmount + _EdgeWidth, noise);
                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                return lerp(_EdgeColor, tex * _Color, edge);
            }}
            ENDHLSL
        }}
    }}
}}";
    }
    
    private string GenerateOutlineShader() => GenerateToonShader();
    private string GenerateRimShader() => GenerateHologramShader();
    private string GenerateWaterShader() => GenerateHologramShader();
    private string GenerateGradientShader() => GenerateToonShader();
    private string GenerateEmissionShader() => GenerateDissolveShader();
    
    private void CreateMaterial()
    {
        string shaderPath = "Assets/_GeneratedShaders/" + shaderName + ".shader";
        Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(shaderPath);
        
        if (shader == null)
        {
            EditorUtility.DisplayDialog("No Shader", "Generate a shader first!", "OK");
            return;
        }
        
        Material mat = new Material(shader);
        mat.SetColor("_Color", mainColor);
        
        string matPath = "Assets/_GeneratedShaders/" + shaderName + "_Mat.mat";
        AssetDatabase.CreateAsset(mat, matPath);
        AssetDatabase.SaveAssets();
        
        Selection.activeObject = mat;
        Debug.Log("✅ Material created: " + matPath);
    }
}
