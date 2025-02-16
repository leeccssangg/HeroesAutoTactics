using Spine.Unity;
using TW.Utility.Extension;
using UnityEngine;

public class HeroSpriteGraphic : MonoBehaviour
{
    [field: SerializeField] public SkeletonAnimation SkeletonAnimation {get; private set;}
    public void Init(HeroConfigData heroConfigData)
    {
        if (heroConfigData.SkeletonDataAsset != null) 
        {
            SkeletonAnimation.skeletonDataAsset = heroConfigData.SkeletonDataAsset;
        }
        else if (heroConfigData.SpriteIcon != null)
        {
            SkeletonAnimation.gameObject.SetActive(false);
            SpriteRenderer temp = SkeletonAnimation.transform.parent.FindChildOrCreate("TempSprite", true).GetComponent<SpriteRenderer>();
            temp.sprite = heroConfigData.SpriteIcon;
            temp.gameObject.SetActive(true);
        }
    }
}