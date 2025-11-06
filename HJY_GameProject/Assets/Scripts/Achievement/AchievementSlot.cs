using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementSlot : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public Text nameText;
    public Text descriptionText;
    public Text progressText;
    public Slider progressSlider;   

    public void SetAchievement(AchievementData achievement, float progress)
    {
        if (nameText != null)
            nameText.text = achievement.name;
        
        if(descriptionText != null)
            descriptionText.text = achievement.description;

        if (iconImage != null && achievement.icon != null)
            iconImage.sprite = achievement.icon;

        if (progressSlider != null)
            progressSlider.value = achievement.isUnlocked ? 1f : progress;

        if(progressText != null)
        {
            if(achievement.isUnlocked)
            {
                progressText.text = "¿Ï·á";
            }
            else
            {
                int current = Mathf.FloorToInt(progress * achievement.requiredAmount);
                progressText.text = current + "/" + achievement.requiredAmount;
            }
        }
    }
}
