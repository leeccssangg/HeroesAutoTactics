using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;

public class TestFunction : MonoBehaviour
{
    [Button]
    private void ChangeTextFont()
    {
        // Change the font of the text component in the scene.
        // This is a test function.
        Debug.Log("ChangeTextFont");
        foreach (TextMeshProUGUI textMeshProUGUI in GetComponentsInChildren<TextMeshProUGUI>())
        {
            // string guid = AssetDatabase.FindAssets("t: TMP_Settings")[0];
            // TMP_Settings tmpSettings = AssetDatabase.LoadAssetAtPath<TMP_Settings>(AssetDatabase.GUIDToAssetPath(guid));
            textMeshProUGUI.font = TMP_Settings.defaultFontAsset;
        }
    }
}