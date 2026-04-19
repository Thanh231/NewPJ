
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.TextCore.LowLevel;

public partial class BuildLocalizationState_main
{
    #region core

	public class GenerateTextMeshProFontOutput
	{
		public int pointSize;
		public Texture2D fontAtlas;
		public FaceInfo faceInfo;
		public List<TMP_Character> characterTable = new List<TMP_Character>();
		public List<Glyph> glyphTable = new List<Glyph>();
		public List<GlyphRect> freeGlyphRect = new List<GlyphRect>();
		public List<GlyphRect> usedGlyphRect = new List<GlyphRect>();

		public GenerateTextMeshProFontOutput(PackGlyphsOutput packGlyphsOutput)
		{
			pointSize = packGlyphsOutput.pointSize;
			freeGlyphRect = packGlyphsOutput.freeGlyphRect;
			usedGlyphRect = packGlyphsOutput.usedGlyphRect;
		}
	}

	public class PackGlyphsOutput
	{
		public int pointSize;
		public List<GlyphRect> freeGlyphRect = new List<GlyphRect>();
		public List<GlyphRect> usedGlyphRect = new List<GlyphRect>();
		public List<Glyph> glyphPacked = new List<Glyph>();
		public List<Glyph> glyphToPack = new List<Glyph>();
	}

	private const GlyphLoadFlags glyphLoadFlags = GlyphLoadFlags.LOAD_RENDER | GlyphLoadFlags.LOAD_NO_HINTING;
	private const int packingModifier = 1;
	private const int maxPackGlyphIteration = 15;
	private const GlyphPackingMode glyphPackingMode = GlyphPackingMode.ContactPointRule;
	private const GlyphRenderMode glyphRenderMode = GlyphRenderMode.SDFAA;
	private const string fontOutputPath = "Assets/_game/localization-data/font/{0}.asset";

	private static Type FontEngineEditorUtilitiesType => StaticUtils.GetType(
		"UnityEditor.TextCore.LowLevel", "FontEngineEditorUtilities",
		"UnityEditor.TextCoreFontEngineModule", "0.0.0.0");

	public static void BuildTextMeshProFont(Dictionary<LanguageGroupName, string> charSet)
	{
		foreach (var i in charSet)
		{
			var characterSetInfo = GameFrameworkConfig.instance.GetLanguageGroupInfo(i.Key);
			var output = GenerateTextMeshProFont(characterSetInfo, i.Value);
			SaveTextMeshProFont(characterSetInfo, i.Value, output);
		}
	}
	
	#endregion

	#region generate font

	private static GenerateTextMeshProFontOutput GenerateTextMeshProFont(LanguageGroupInfo characterSetInfo, string charSet)
	{
		// Initialize font engine
		var initializeFontEngineResult = FontEngine.InitializeFontEngine();
		if (initializeFontEngineResult != FontEngineError.Success)
		{
			throw new Exception($"Initialize FontEngine fail code={initializeFontEngineResult}");
		}

		// load font
		var loadFontResult = FontEngine.LoadFontFace(characterSetInfo.font);
		if (loadFontResult != FontEngineError.Success)
		{
			throw new Exception($"Load font {characterSetInfo.font.name} fail code={loadFontResult}");
		}

		var glyphLookupMap = StringToListGlyphIndex(charSet);
		var listGlyphCode = new List<uint>(glyphLookupMap.Keys);
		var listGlyph = new List<Glyph>();

		var packGlyphOutput = PackGlyphs(listGlyphCode, characterSetInfo);

		var output = new GenerateTextMeshProFontOutput(packGlyphOutput);

		foreach (Glyph glyph in packGlyphOutput.glyphPacked)
		{
			if (glyph.glyphRect.width > 0 && glyph.glyphRect.height > 0)
			{
				listGlyph.Add(glyph);
			}
			output.glyphTable.Add(glyph);
			foreach (var unicode in glyphLookupMap[glyph.index])
			{
				output.characterTable.Add(new TMP_Character(unicode, glyph));
			}
		}

		output.faceInfo = FontEngine.GetFaceInfo();
		output.fontAtlas = RenderGlyph(listGlyph, characterSetInfo);

		return output;
	}

	private static Dictionary<uint, List<uint>> StringToListGlyphIndex(string charSet)
	{
		//2 unicode can have the same glyph
		//for example space and non-breaking space
		var result = new Dictionary<uint, List<uint>>();

		for (int i = 0; i < charSet.Length; i++)
		{
			uint unicode = charSet[i];

			// Handle surrogate pairs
			if (i < charSet.Length - 1 && char.IsHighSurrogate((char)unicode) && char.IsLowSurrogate(charSet[i + 1]))
			{
				unicode = (uint)char.ConvertToUtf32(charSet[i], charSet[i + 1]);
				i += 1;
			}

			if (FontEngine.TryGetGlyphIndex(unicode, out uint glyphIndex))
			{
				if (!result.ContainsKey(glyphIndex))
				{
					result.Add(glyphIndex, new List<uint>());
				}
				result[glyphIndex].Add(unicode);
			}
		}

		return result;
	}

	private static PackGlyphsOutput PackGlyphs(List<uint> glyphSet, LanguageGroupInfo characterSetInfo)
	{
		var output = new PackGlyphsOutput();
		var optimumPointSizeFound = false;

		//calculate begin point size
		var glyphArea = characterSetInfo.atlasSize * characterSetInfo.atlasSize / glyphSet.Count;
		var minPointSize = 0;
		var maxPointSize = (int)Mathf.Sqrt(glyphArea) * 3;
		output.pointSize = (maxPointSize + minPointSize) / 2;

		//iterate
		for (var iteration = 0; iteration < maxPackGlyphIteration && optimumPointSizeFound == false; iteration++)
		{
			FontEngine.SetFaceSize(output.pointSize);

			output.glyphToPack.Clear();
			output.glyphPacked.Clear();
			output.freeGlyphRect.Clear();
			output.usedGlyphRect.Clear();

			output.freeGlyphRect.Add(new GlyphRect(0, 0,
				characterSetInfo.atlasSize - packingModifier, characterSetInfo.atlasSize - packingModifier));

			foreach (var glyphId in glyphSet)
			{
				if (FontEngine.TryGetGlyphWithIndexValue(glyphId, glyphLoadFlags, out Glyph glyph))
				{
					if (glyph.glyphRect.width > 0 && glyph.glyphRect.height > 0)
					{
						output.glyphToPack.Add(glyph);
					}
					else
					{
						output.glyphPacked.Add(glyph);
					}
				}
			}

			StaticUtils.CallStaticFunction(typeof(FontEngine), "TryPackGlyphsInAtlas", new object[] {
				output.glyphToPack,
				output.glyphPacked,
				characterSetInfo.padding,
				glyphPackingMode,
				glyphRenderMode,
				characterSetInfo.atlasSize,
				characterSetInfo.atlasSize,
				output.freeGlyphRect,
				output.usedGlyphRect });

			if (output.glyphToPack.Count > 0)
			{
				if (output.pointSize > minPointSize)
				{
					maxPointSize = output.pointSize;
					output.pointSize = (output.pointSize + minPointSize) / 2;
				}
			}
			else
			{
				if (maxPointSize - minPointSize > 1 && output.pointSize < maxPointSize)
				{
					minPointSize = output.pointSize;
					output.pointSize = (output.pointSize + maxPointSize) / 2;
				}
				else
				{
					optimumPointSizeFound = true;
				}
			}
		}

		return output;
	}

	private static Texture2D RenderGlyph(List<Glyph> glyphs, LanguageGroupInfo characterSetInfo)
	{
		var byteBuffer = new byte[characterSetInfo.atlasSize * characterSetInfo.atlasSize];
		StaticUtils.CallStaticFunction(typeof(FontEngine), "RenderGlyphsToTexture", new object[] {
			glyphs,
			characterSetInfo.padding,
			glyphRenderMode,
			byteBuffer,
			characterSetInfo.atlasSize,
			characterSetInfo.atlasSize });

		var colorBuffer = new Color32[characterSetInfo.atlasSize * characterSetInfo.atlasSize];
		for (int i = 0; i < colorBuffer.Length; i++)
		{
			byte c = byteBuffer[i];
			colorBuffer[i] = new Color32(c, c, c, c);
		}

		var fontAtlas = new Texture2D(characterSetInfo.atlasSize, characterSetInfo.atlasSize,
			TextureFormat.Alpha8, false, true);
		fontAtlas.SetPixels32(colorBuffer, 0);
		fontAtlas.Apply(false, false);

		return fontAtlas;
	}

	#endregion

	#region save font

	private static void SaveTextMeshProFont(
		LanguageGroupInfo characterSetInfo, string charSet, GenerateTextMeshProFontOutput genOutput)
	{
		var fontPath = string.Format(fontOutputPath, characterSetInfo.languageGroupName);
		StaticUtilsEditor.ModifyAsset<TMP_FontAsset>(fontPath, fontAsset =>
		{
			var isNewlyCreated = false;
			if (fontAsset == null)
			{
				fontAsset = SaveNewTextMeshProFont(characterSetInfo, genOutput, fontPath);
				isNewlyCreated = true;
			}
			else
			{
				SaveExistingTextMeshProFont(characterSetInfo, fontAsset, genOutput);
			}

			ConfigureFontAsset(fontAsset, isNewlyCreated, characterSetInfo, charSet, genOutput);

			StaticUtils.CallStaticFunction(FontEngineEditorUtilitiesType, "SetAtlasTextureIsReadable",
				new object[] { fontAsset.atlasTexture, false });
			StaticUtils.CallInstanceFunction(fontAsset, "SortAllTables", null);
			fontAsset.ReadFontAssetDefinition();

			// NEED TO GENERATE AN EVENT TO FORCE A REDRAW OF ANY TEXTMESHPRO INSTANCES THAT MIGHT BE USING THIS FONT ASSET
			TMPro_EventManager.ON_FONT_PROPERTY_CHANGED(true, fontAsset);
		});
	}

	private static TMP_FontAsset SaveNewTextMeshProFont(
		LanguageGroupInfo characterSetInfo, GenerateTextMeshProFontOutput genOutput, string fontPath)
	{
		var fontAssetName = characterSetInfo.languageGroupName.ToString();

		//create font asset
		var fontAsset = ScriptableObject.CreateInstance<TMP_FontAsset>();
		var fullFontPath = $"{Application.dataPath}/../{fontPath}";
		StaticUtils.CreateFolder(Path.GetDirectoryName(fullFontPath), true);
		AssetDatabase.CreateAsset(fontAsset, fontPath);

		//add atlas to font
		fontAsset.atlasTextures = new Texture2D[] { genOutput.fontAtlas };
		genOutput.fontAtlas.name = fontAssetName + " Atlas";
		AssetDatabase.AddObjectToAsset(genOutput.fontAtlas, fontAsset);

		//add material to font
		Shader shader = Shader.Find("TextMeshPro/Distance Field");
		int spread = characterSetInfo.padding + 1;

		Material mat = new Material(shader);
		mat.name = fontAssetName + " Material";
		mat.SetTexture(ShaderUtilities.ID_MainTex, genOutput.fontAtlas);
		mat.SetFloat(ShaderUtilities.ID_TextureWidth, genOutput.fontAtlas.width);
		mat.SetFloat(ShaderUtilities.ID_TextureHeight, genOutput.fontAtlas.height);
		mat.SetFloat(ShaderUtilities.ID_GradientScale, spread); // Spread = Padding for Brute Force SDF.
		mat.SetFloat(ShaderUtilities.ID_WeightNormal, fontAsset.normalStyle);
		mat.SetFloat(ShaderUtilities.ID_WeightBold, fontAsset.boldStyle);
		fontAsset.material = mat;

		AssetDatabase.AddObjectToAsset(mat, fontAsset);

		return fontAsset;
	}

	private static void SaveExistingTextMeshProFont(
		LanguageGroupInfo characterSetInfo, TMP_FontAsset fontAsset, GenerateTextMeshProFontOutput genOutput)
	{
		var fontAssetName = characterSetInfo.languageGroupName.ToString();

		//destroy old font atlas
		if (fontAsset.atlasTextures != null && fontAsset.atlasTextures.Length > 0)
		{
			for (int i = 1; i < fontAsset.atlasTextures.Length; i++)
				UnityEngine.Object.DestroyImmediate(fontAsset.atlasTextures[i], true);
		}

		//update font atlas
		Texture2D tex = fontAsset.atlasTextures[0];
		tex.name = fontAssetName + " Atlas";

		bool isReadableState = tex.isReadable;
		if (isReadableState == false)
		{
			StaticUtils.CallStaticFunction(FontEngineEditorUtilitiesType, "SetAtlasTextureIsReadable",
				new object[] { tex, true });
		}

		if (tex.width != characterSetInfo.atlasSize || tex.height != characterSetInfo.atlasSize)
		{
			tex.Reinitialize(characterSetInfo.atlasSize, characterSetInfo.atlasSize);
			tex.Apply(false);
		}

		Graphics.CopyTexture(genOutput.fontAtlas, tex);
		tex.Apply(false);

		//update material
		Material[] mats = (Material[])StaticUtils.CallStaticFunction(typeof(TMP_EditorUtility),
			"FindMaterialReferences", new object[] { fontAsset });
		for (int i = 0; i < mats.Length; i++)
		{
			int spread = characterSetInfo.padding + 1;

			mats[i].SetFloat(ShaderUtilities.ID_TextureWidth, tex.width);
			mats[i].SetFloat(ShaderUtilities.ID_TextureHeight, tex.height);
			mats[i].SetFloat(ShaderUtilities.ID_GradientScale, spread);
			mats[i].SetFloat(ShaderUtilities.ID_WeightNormal, fontAsset.normalStyle);
			mats[i].SetFloat(ShaderUtilities.ID_WeightBold, fontAsset.boldStyle);
		}
	}

	private static void ConfigureFontAsset(TMP_FontAsset fontAsset, bool isNewlyCreated,
		LanguageGroupInfo characterSetInfo, string charSet, GenerateTextMeshProFontOutput genOutput)
	{
		if (isNewlyCreated)
		{
			StaticUtils.SetProperty(fontAsset, "version", "1.1.0");
			StaticUtils.SetProperty(fontAsset, "atlasRenderMode", glyphRenderMode);

			var font = characterSetInfo.font;
			var fontGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(font));
			StaticUtils.SetField(fontAsset, "m_SourceFontFile_EditorRef", font);
			StaticUtils.SetField(fontAsset, "m_SourceFontFileGUID", fontGUID);
		}

		StaticUtils.SetProperty(fontAsset, "freeGlyphRects", genOutput.freeGlyphRect);
		StaticUtils.SetProperty(fontAsset, "usedGlyphRects", genOutput.usedGlyphRect);
		StaticUtils.SetProperty(fontAsset, "glyphTable", genOutput.glyphTable);
		StaticUtils.SetProperty(fontAsset, "characterTable", genOutput.characterTable);
		StaticUtils.SetProperty(fontAsset, "atlasWidth", characterSetInfo.atlasSize);
		StaticUtils.SetProperty(fontAsset, "atlasHeight", characterSetInfo.atlasSize);
		StaticUtils.SetProperty(fontAsset, "atlasPadding", characterSetInfo.padding);

		fontAsset.faceInfo = genOutput.faceInfo;
		fontAsset.creationSettings = CreateFontAssetCreationSettings(characterSetInfo, charSet, genOutput);
	}

	private static FontAssetCreationSettings CreateFontAssetCreationSettings(
		LanguageGroupInfo characterSetInfo, string charSet, GenerateTextMeshProFontOutput genOutput)
	{
		var font = characterSetInfo.font;
		var fontGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(font));
		return new FontAssetCreationSettings()
		{
			sourceFontFileGUID = fontGUID,
			pointSizeSamplingMode = 0,// Auto-Sizing Point Size Mode
			pointSize = genOutput.pointSize,
			padding = characterSetInfo.padding,
			packingMode = (int)glyphPackingMode,
			atlasWidth = characterSetInfo.atlasSize,
			atlasHeight = characterSetInfo.atlasSize,
			characterSetSelectionMode = 7,//custom characters
			characterSequence = charSet,
			referencedFontAssetGUID = null,
			referencedTextAssetGUID = null,
			renderMode = (int)glyphRenderMode,
			includeFontFeatures = false,
		};
	}

	#endregion
}
