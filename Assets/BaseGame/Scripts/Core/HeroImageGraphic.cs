using Spine.Unity;
using TW.Utility.Extension;
using UnityEngine;
using UnityEngine.UI;

public class HeroImageGraphic : MonoBehaviour
{
    [field: SerializeField] public SkeletonGraphic SkeletonGraphic {get; private set;}
    private Image TempIcon { get; set; }
    private bool IsUsingTempIcon { get; set; }
    public void Init(HeroConfigData heroConfigData)
    {
        IsUsingTempIcon = heroConfigData.SkeletonDataAsset == null;
        TempIcon = SkeletonGraphic.transform.parent.FindChildOrCreate("TempIcon", true).GetComponent<Image>();
        if (!IsUsingTempIcon)
        {
            TempIcon.gameObject.SetActive(false);
            SkeletonGraphic.gameObject.SetActive(true);
            SkeletonGraphic.color = Color.white;
            SkeletonGraphic.skeletonDataAsset = heroConfigData.SkeletonDataAsset;
            SkeletonGraphic.Initialize(true);
        }
        else
        {
            SkeletonGraphic.gameObject.SetActive(false);
            TempIcon.gameObject.SetActive(true);
            TempIcon.sprite = heroConfigData.SpriteIcon;
        }
    }
    public void SetActive(bool isActive)
    {
        if (!IsUsingTempIcon)
        {
            SkeletonGraphic.gameObject.SetActive(isActive);
        }
        else
        {
            TempIcon.gameObject.SetActive(isActive);
        }
    }
    public void SetColor(Color color)
    {
        if (!IsUsingTempIcon)
        {
            SkeletonGraphic.color = color;
        }
        else
        {
            TempIcon.color = color;
        }
    }
}