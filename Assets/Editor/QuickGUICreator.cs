using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public enum GUIType {
	PackedSprite,
	UIButton
}

public class QuickGUICreator : ScriptableWizard
{
	public GUIType type;
	public Material material;
	public bool useNewMaterial = true;
	public string newMaterialName;
	
	private List<Texture> textures = new List<Texture>();

	[UnityEditor.MenuItem("Tools/QuickGUICreator")]
	static void GUICreate()
	{
		ScriptableWizard.DisplayWizard("QuickGUICreate", typeof(QuickGUICreator));
	}
	
	
	void OnWizardCreate()
	{
		if (useNewMaterial)
		{
			if (newMaterialName != "")
			{
				material = new Material(Shader.Find("Sprite/Vertex Colored, Fast"));
				AssetDatabase.CreateAsset(material, "Assets/" + newMaterialName + ".mat");
			}
			else
			{
				Debug.Log("Don't have new material name.");
			}
		}
		Object[] o = Selection.GetFiltered(typeof(Texture), SelectionMode.Unfiltered);
		for(int i = 0; i < o.Length; i++) textures.Add((Texture) o[i]);
		foreach(Texture texture in textures)
		{
			GameObject obj = new GameObject(texture.name);
			if (type == GUIType.PackedSprite)
			{
				PackedSprite tmp = obj.AddComponent<PackedSprite>();
				tmp.gameObject.renderer.material = material;
				tmp.staticTexPath = AssetDatabase.GetAssetPath(texture);
				tmp.staticTexGUID = AssetDatabase.AssetPathToGUID(tmp.staticTexPath);			
				Vector2 pxSize = tmp.GetDefaultPixelSize(AssetDatabase.GUIDToAssetPath, AssetDatabase.LoadAssetAtPath);
				tmp.width = pxSize.x;
				tmp.height = pxSize.y;
			}
			else if(type == GUIType.UIButton)
			{
				UIButton tmp = obj.AddComponent<UIButton>();
				tmp.gameObject.renderer.material = material;
			}
		}
		BuildAtlases.doSpecificMaterials = true;
		List<Material> l = new List<Material>();
		l.Add(material);
		BuildAtlases.targetMaterials = l;
		ScriptableWizard.DisplayWizard("Build Atlas", typeof(BuildAtlases));
	}
}
