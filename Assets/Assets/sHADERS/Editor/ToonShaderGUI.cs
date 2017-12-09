using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

public class ToonShaderGUI : ShaderGUI 
{
	Material target;
	MaterialEditor editor;
	MaterialProperty[] properties;	
	bool shouldShowAlphaCutoff;
	
	static ColorPickerHDRConfig emissionConfig =
		new ColorPickerHDRConfig(0f, 99f, 1f / 99f, 3f);

	//-------------------------------------------
	// Enums
	enum SmoothnessSource 
	{
		Uniform, SpecularMap
	}

	enum RenderingMode 
	{
		Opaque, Cutout, Fade, Transparent
	}

	// This is to put the different transparency stuff 
	// in a different spot in the render queue
	struct RenderingSettings 
	{
		public RenderQueue queue;
		public string renderType;
		
		public BlendMode srcBlend, dstBlend;
		public bool zWrite;
		public static RenderingSettings[] modes = 
		{
			new RenderingSettings() 
			{
				queue = RenderQueue.Geometry,
				renderType = "",
				srcBlend = BlendMode.One,
				dstBlend = BlendMode.Zero,
				zWrite = true
			},
			new RenderingSettings() 
			{
				queue = RenderQueue.AlphaTest,
				renderType = "TransparentCutout",
				srcBlend = BlendMode.One,
				dstBlend = BlendMode.Zero,
				zWrite = true
			},
			new RenderingSettings() 
			{
				queue = RenderQueue.Transparent,
				renderType = "Transparent",
				srcBlend = BlendMode.SrcAlpha,
				dstBlend = BlendMode.OneMinusSrcAlpha,
				zWrite = true
			},
			new RenderingSettings() {
				queue = RenderQueue.Transparent,
				renderType = "Transparent",
				srcBlend = BlendMode.One,
				dstBlend = BlendMode.OneMinusSrcAlpha,
				zWrite = false
			}
		};
	}

	enum TransparencySource 
	{
		Uniform, SpecularMap
	}

	

	//-------------------------------------------
	// Methods that make me type less
	MaterialProperty FindProperty (string name) 
	{
		return FindProperty(name, properties);
	}

	static GUIContent staticLabel = new GUIContent();
	
	static GUIContent MakeLabel (
		string text, string tooltip = null) 
	{
		staticLabel.text = text;
		staticLabel.tooltip = tooltip;
		return staticLabel;
	}

	static GUIContent MakeLabel (
		MaterialProperty property, string tooltip = null) 
	{
		staticLabel.text = property.displayName;
		staticLabel.tooltip = tooltip;
		return staticLabel;
	}

	//void SetKeyword (string keyword, bool state) 
	//{
	//	if (state) 
	//	{
	//		target.EnableKeyword(keyword);
	//	}
	//	else 
	//	{
	//		target.DisableKeyword(keyword);
	//	}
	//}

	void SetKeyword (string keyword, bool state)
	{
		if (state) 
		{
			foreach (Material m in editor.targets) 
			{
				m.EnableKeyword(keyword);
			}
		}
		else 
		{
			foreach (Material m in editor.targets) 
			{
				m.DisableKeyword(keyword);
			}
		}
	}

	bool IsKeywordEnabled (string keyword) 
	{
		return target.IsKeywordEnabled(keyword);
	}
	
	// I just wanted to undo shit
	void RecordAction (string label) 
	{
		editor.RegisterPropertyChangeUndo(label);
	}

	//-------------------------------------------
	// The rest
	public override void OnGUI (
		MaterialEditor editor, MaterialProperty[] properties) 
	{
		this.target = editor.target as Material;
		this.editor = editor;
		this.properties = properties;
		DoMain();
		DoRim();
		DoSecondary();
		DoAdvanced();
		DoFun();
		
	}

	void DoRenderingMode () 
	{
		RenderingMode mode = RenderingMode.Opaque;
		shouldShowAlphaCutoff = false;
		if (IsKeywordEnabled("_RENDERING_CUTOUT")) 
		{
			mode = RenderingMode.Cutout;
			shouldShowAlphaCutoff = true;
		}
		else if (IsKeywordEnabled("_RENDERING_FADE")) 
		{
			mode = RenderingMode.Fade;
		}
		else if (IsKeywordEnabled("_RENDERING_TRANSPARENT")) 
		{
			mode = RenderingMode.Transparent;
		}


		EditorGUI.BeginChangeCheck();
		mode = (RenderingMode)EditorGUILayout.EnumPopup(
			MakeLabel("Rendering Mode"), mode
		);
		if (EditorGUI.EndChangeCheck()) 
		{
			RecordAction("Rendering Mode");
			SetKeyword("_RENDERING_CUTOUT", mode == RenderingMode.Cutout);
			SetKeyword("_RENDERING_FADE", mode == RenderingMode.Fade);
			SetKeyword("_RENDERING_TRANSPARENT", mode == RenderingMode.Transparent);

			RenderingSettings settings = RenderingSettings.modes[(int)mode];
			foreach (Material m in editor.targets) 
			{
				m.renderQueue = (int)settings.queue;
				m.SetOverrideTag("RenderType", settings.renderType);
				m.SetInt("_SrcBlend", (int)settings.srcBlend);
				m.SetInt("_DstBlend", (int)settings.dstBlend);
				m.SetInt("_ZWrite", settings.zWrite ? 1 : 0);
			}
				
		}
	}

	void DoMain() 
	{
		GUILayout.Label("Main Textures", EditorStyles.boldLabel);

		MaterialProperty mainTex = FindProperty("_MainTex");

		editor.TexturePropertySingleLine(
			MakeLabel(mainTex, "Albedo (RGB)"), mainTex, FindProperty("_Color")
		);

		DoRenderingMode();
		if (shouldShowAlphaCutoff) 
			DoAlphaCutoff();
		DoNormals();
		DoEmission();

		EditorGUI.indentLevel += 1;
		editor.TextureScaleOffsetProperty(mainTex);
		EditorGUI.indentLevel -= 1;

	}

	void DoAlphaCutoff () 
	{
		MaterialProperty slider = FindProperty("_AlphaCutoff");
		EditorGUI.indentLevel += 2;
		editor.ShaderProperty(slider, MakeLabel(slider));
		EditorGUI.indentLevel -= 2;
	}

	void DoNormals () 
	{
		MaterialProperty map = FindProperty("_BumpTex");
		Texture tex = map.textureValue;
		EditorGUI.BeginChangeCheck();
		editor.TexturePropertySingleLine(
			MakeLabel(map), map,
			tex ? FindProperty("_BumpScale") : null);
		
		if (EditorGUI.EndChangeCheck() && tex != map.textureValue) 
			SetKeyword("_NORMAL_MAP", map.textureValue);
	}

	void DoRim () 
	{
		GUILayout.Label("Rim Light", EditorStyles.boldLabel);

		MaterialProperty rimColor = FindProperty("_RimColor");
		MaterialProperty rimPower = FindProperty("_RimPower");

		EditorGUI.BeginChangeCheck();
		editor.ShaderProperty(rimColor, MakeLabel(rimColor, "Rim Color"), 2);
		editor.ShaderProperty(rimPower, MakeLabel(rimPower, "Rim Power"), 2);
		//editor.ColorProperty(
		//	MakeLabel(rimColor, "Rim Color"), rimColor, rimPower);
		
		if (EditorGUI.EndChangeCheck()) 
		{
			bool blank = false;
			float color = rimColor.colorValue.a *
			(rimColor.colorValue.r + rimColor.colorValue.g + rimColor.colorValue.b);
			if(color > 0.0f)
			{
				blank = true;
			}
			SetKeyword("_RIM_LIGHT", blank);
		}
	}

	void DoEmission () 
	{
		MaterialProperty map = FindProperty("_EmissionTex");
		EditorGUI.BeginChangeCheck();
		editor.TexturePropertyWithHDRColor(
			MakeLabel("Emission (RGB)"), map, FindProperty("_Emission"), 
			emissionConfig, false);
		
		if (EditorGUI.EndChangeCheck()) 
			SetKeyword("_EMISSION_MAP", map.textureValue);
	}

	void DoTransparency()
	{

	}

	void DoSmoothness () 
	{
		SmoothnessSource source = SmoothnessSource.Uniform;
		if (IsKeywordEnabled("_SMOOTHNESS_ALBEDO")) 
		{
			source = SmoothnessSource.SpecularMap;
		}
		
		MaterialProperty slider = FindProperty("_Smoothness");
		EditorGUI.indentLevel += 2;
		editor.ShaderProperty(slider, MakeLabel(slider));
		EditorGUI.indentLevel += 1;
		EditorGUI.BeginChangeCheck();
		source = (SmoothnessSource)EditorGUILayout.EnumPopup(
			MakeLabel("Source"), source);

		if (EditorGUI.EndChangeCheck()) 
		{
			SetKeyword("_SMOOTHNESS_ALBEDO", source == SmoothnessSource.SpecularMap);
		}
		EditorGUI.indentLevel -= 3;
	}

	void DoSecondary () 
	{
		GUILayout.Label("Secondary Maps", EditorStyles.boldLabel);

		MaterialProperty detailTex = FindProperty("_DetailTex");
		EditorGUI.BeginChangeCheck();

		editor.TexturePropertySingleLine(
			MakeLabel(detailTex, "Detail Albedo (RGB) multiplied by 2\n" +
			"The slider controls the amount of detail\n" +
			"0 is none, 1 is max"), 
			detailTex,
			detailTex.textureValue ? FindProperty("_DetailScale") : null);

		if (EditorGUI.EndChangeCheck()) 
			SetKeyword("_DETAIL_ALBEDO_MAP", detailTex.textureValue);

		DoSecondaryNormals();

		EditorGUI.indentLevel += 1;
		editor.TextureScaleOffsetProperty(detailTex);
		EditorGUI.indentLevel -= 1;

		
	}

	void DoSecondaryNormals () 
	{
		MaterialProperty map = FindProperty("_DetailBumpTex");
		EditorGUI.BeginChangeCheck();

		editor.TexturePropertySingleLine(
			MakeLabel(map), map,
			map.textureValue ? FindProperty("_DetailBumpScale") : null);

		if (EditorGUI.EndChangeCheck()) 
			SetKeyword("_DETAIL_NORMAL_MAP", map.textureValue);
	}

	void DoAdvanced () 
	{
		GUILayout.Label("Advanced Options", EditorStyles.boldLabel);

		editor.EnableInstancingField();
	}

	void DoFun()
	{
		MaterialProperty fun = new MaterialProperty();
		editor.RangeProperty(fun, "Fun: ");
	}
}