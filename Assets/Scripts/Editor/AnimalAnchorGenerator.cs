using UnityEngine;
using UnityEditor;
using System.IO;

[InitializeOnLoad]
public static class AnimalAnchorGenerator
{
    private const string ANIMAL_PREFAB_DIR = "Assets/Prefabs/Animals";
    private const string BODY_PART_LAYER = "BodyPart";

    [InitializeOnLoadMethod]
    private static void OnProjectLoadedInEditor()
    {
        EditorApplication.delayCall += () =>
        {
            if (!File.Exists($"{ANIMAL_PREFAB_DIR}/Dog.prefab") ||
                !File.Exists($"{ANIMAL_PREFAB_DIR}/Chicken.prefab") ||
                !File.Exists($"{ANIMAL_PREFAB_DIR}/Goat.prefab"))
            {
                GenerateAll();
            }
        };
    }

    [MenuItem("Tools/Generate Animal Anchors (Chicken, Dog, Goat)")]
    public static void GenerateAll()
    {
        EnsureLayerExists(BODY_PART_LAYER);
        EnsureDirectoryExists(ANIMAL_PREFAB_DIR);

        GenerateDog();
        GenerateChicken();
        GenerateGoat();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("<color=green>[AnimalAnchorGenerator]</color> Successfully generated and saved Chicken, Dog, Goat prefabs with Anchors & Colliders to " + ANIMAL_PREFAB_DIR);
    }

    private static void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            AssetDatabase.Refresh();
        }
    }

    private static void EnsureLayerExists(string layerName)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layersProp = tagManager.FindProperty("layers");

        bool layerFound = false;
        for (int i = 8; i < layersProp.arraySize; i++)
        {
            SerializedProperty sp = layersProp.GetArrayElementAtIndex(i);
            if (sp.stringValue == layerName)
            {
                layerFound = true;
                break;
            }
        }

        if (!layerFound)
        {
            for (int i = 8; i < layersProp.arraySize; i++)
            {
                SerializedProperty sp = layersProp.GetArrayElementAtIndex(i);
                if (string.IsNullOrEmpty(sp.stringValue))
                {
                    sp.stringValue = layerName;
                    tagManager.ApplyModifiedProperties();
                    Debug.Log($"[AnimalAnchorGenerator] Added layer '{layerName}' at index {i}");
                    break;
                }
            }
        }
    }

    private static GameObject CreateAnchor(GameObject parent, string anchorName, Vector3 localPos, Vector3 colliderSize, Vector3 colliderCenter = default)
    {
        Transform existing = parent.transform.Find(anchorName);
        GameObject anchorObj;
        if (existing != null)
        {
            anchorObj = existing.gameObject;
        }
        else
        {
            anchorObj = new GameObject(anchorName);
            anchorObj.transform.SetParent(parent.transform, false);
        }

        anchorObj.transform.localPosition = localPos;
        anchorObj.transform.localRotation = Quaternion.identity;
        anchorObj.transform.localScale = Vector3.one;

        int layer = LayerMask.NameToLayer(BODY_PART_LAYER);
        if (layer != -1)
        {
            anchorObj.layer = layer;
        }

        BoxCollider col = anchorObj.GetComponent<BoxCollider>();
        if (col == null)
        {
            col = anchorObj.AddComponent<BoxCollider>();
        }
        col.isTrigger = true;
        col.size = colliderSize;
        col.center = colliderCenter;

        return anchorObj;
    }

    private static void GenerateDog()
    {
        string sourcePrefabPath = "Assets/ithappy/Animals_FREE/Prefabs/Dog_001.prefab";
        GameObject source = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePrefabPath);
        if (source == null)
        {
            Debug.LogError($"[AnimalAnchorGenerator] Source Dog prefab not found at {sourcePrefabPath}");
            return;
        }

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(source);
        instance.name = "Dog";
        instance.transform.position = Vector3.zero;
        instance.transform.rotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one;

        // Add Anchors according to workflow conventions
        // 1. Head
        CreateAnchor(instance, "Anchor_Head", new Vector3(0f, 0.48f, 0.38f), new Vector3(0.24f, 0.26f, 0.30f));

        // 2. Body
        CreateAnchor(instance, "Anchor_Body", new Vector3(0f, 0.38f, 0.02f), new Vector3(0.26f, 0.30f, 0.50f));

        // 3. Tail
        CreateAnchor(instance, "Anchor_Tail", new Vector3(0f, 0.44f, -0.32f), new Vector3(0.12f, 0.22f, 0.20f));

        // 4. Ears
        CreateAnchor(instance, "Anchor_Ear_Left", new Vector3(-0.10f, 0.58f, 0.34f), new Vector3(0.10f, 0.14f, 0.12f));
        CreateAnchor(instance, "Anchor_Ear_Right", new Vector3(0.10f, 0.58f, 0.34f), new Vector3(0.10f, 0.14f, 0.12f));

        // 5. Legs (4 legs)
        CreateAnchor(instance, "Anchor_LegFrontLeft", new Vector3(-0.11f, 0.18f, 0.18f), new Vector3(0.12f, 0.36f, 0.14f));
        CreateAnchor(instance, "Anchor_LegFrontRight", new Vector3(0.11f, 0.18f, 0.18f), new Vector3(0.12f, 0.36f, 0.14f));
        CreateAnchor(instance, "Anchor_LegBackLeft", new Vector3(-0.11f, 0.18f, -0.18f), new Vector3(0.12f, 0.36f, 0.14f));
        CreateAnchor(instance, "Anchor_LegBackRight", new Vector3(0.11f, 0.18f, -0.18f), new Vector3(0.12f, 0.36f, 0.14f));

        string savePath = $"{ANIMAL_PREFAB_DIR}/Dog.prefab";
        PrefabUtility.SaveAsPrefabAsset(instance, savePath);
        Object.DestroyImmediate(instance);
        Debug.Log($"[AnimalAnchorGenerator] Saved {savePath}");
    }

    private static void GenerateChicken()
    {
        string sourcePrefabPath = "Assets/ithappy/Animals_FREE/Prefabs/Chicken_001.prefab";
        GameObject source = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePrefabPath);
        if (source == null)
        {
            Debug.LogError($"[AnimalAnchorGenerator] Source Chicken prefab not found at {sourcePrefabPath}");
            return;
        }

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(source);
        instance.name = "Chicken";
        instance.transform.position = Vector3.zero;
        instance.transform.rotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one;

        // Add Anchors according to workflow conventions
        // 1. Head (Head + Beak + Comb)
        CreateAnchor(instance, "Anchor_Head", new Vector3(0f, 0.44f, 0.12f), new Vector3(0.18f, 0.22f, 0.22f));

        // 2. Body
        CreateAnchor(instance, "Anchor_Body", new Vector3(0f, 0.28f, -0.05f), new Vector3(0.24f, 0.24f, 0.32f));

        // 3. Tail
        CreateAnchor(instance, "Anchor_Tail", new Vector3(0f, 0.35f, -0.24f), new Vector3(0.14f, 0.22f, 0.18f));

        // 4. Wings
        CreateAnchor(instance, "Anchor_Wing_Left", new Vector3(-0.14f, 0.28f, -0.03f), new Vector3(0.10f, 0.18f, 0.22f));
        CreateAnchor(instance, "Anchor_Wing_Right", new Vector3(0.14f, 0.28f, -0.03f), new Vector3(0.10f, 0.18f, 0.22f));

        // 5. Legs
        CreateAnchor(instance, "Anchor_LegFrontLeft", new Vector3(-0.07f, 0.10f, -0.02f), new Vector3(0.10f, 0.20f, 0.14f));
        CreateAnchor(instance, "Anchor_LegFrontRight", new Vector3(0.07f, 0.10f, -0.02f), new Vector3(0.10f, 0.20f, 0.14f));

        string savePath = $"{ANIMAL_PREFAB_DIR}/Chicken.prefab";
        PrefabUtility.SaveAsPrefabAsset(instance, savePath);
        Object.DestroyImmediate(instance);
        Debug.Log($"[AnimalAnchorGenerator] Saved {savePath}");
    }

    private static void GenerateGoat()
    {
        string sourcePrefabPath = "Assets/UrsaAnimation/LOW POLY CUBIC - Goat and Sheep Pack/Prefabs_URP/SK_Goat_white.prefab";
        if (!File.Exists(sourcePrefabPath))
        {
            sourcePrefabPath = "Assets/UrsaAnimation/LOW POLY CUBIC - Goat and Sheep Pack/Prefabs_Standard/SK_Goat_white.prefab";
        }

        GameObject source = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePrefabPath);
        if (source == null)
        {
            Debug.LogError($"[AnimalAnchorGenerator] Source Goat prefab not found at {sourcePrefabPath}");
            return;
        }

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(source);
        instance.name = "Goat";
        instance.transform.position = Vector3.zero;
        instance.transform.rotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one;

        // Add Anchors according to workflow conventions
        // 1. Head (Head + Horns)
        CreateAnchor(instance, "Anchor_Head", new Vector3(0f, 0.72f, 0.38f), new Vector3(0.28f, 0.32f, 0.34f));

        // 2. Body
        CreateAnchor(instance, "Anchor_Body", new Vector3(0f, 0.54f, 0f), new Vector3(0.34f, 0.38f, 0.60f));

        // 3. Tail
        CreateAnchor(instance, "Anchor_Tail", new Vector3(0f, 0.58f, -0.38f), new Vector3(0.14f, 0.18f, 0.16f));

        // 4. Ears
        CreateAnchor(instance, "Anchor_Ear_Left", new Vector3(-0.14f, 0.82f, 0.32f), new Vector3(0.12f, 0.18f, 0.14f));
        CreateAnchor(instance, "Anchor_Ear_Right", new Vector3(0.14f, 0.82f, 0.32f), new Vector3(0.12f, 0.18f, 0.14f));

        // 5. Legs (4 legs)
        CreateAnchor(instance, "Anchor_LegFrontLeft", new Vector3(-0.14f, 0.25f, 0.22f), new Vector3(0.14f, 0.50f, 0.16f));
        CreateAnchor(instance, "Anchor_LegFrontRight", new Vector3(0.14f, 0.25f, 0.22f), new Vector3(0.14f, 0.50f, 0.16f));
        CreateAnchor(instance, "Anchor_LegBackLeft", new Vector3(-0.14f, 0.25f, -0.22f), new Vector3(0.14f, 0.50f, 0.16f));
        CreateAnchor(instance, "Anchor_LegBackRight", new Vector3(0.14f, 0.25f, -0.22f), new Vector3(0.14f, 0.50f, 0.16f));

        string savePath = $"{ANIMAL_PREFAB_DIR}/Goat.prefab";
        PrefabUtility.SaveAsPrefabAsset(instance, savePath);
        Object.DestroyImmediate(instance);
        Debug.Log($"[AnimalAnchorGenerator] Saved {savePath}");
    }
}
