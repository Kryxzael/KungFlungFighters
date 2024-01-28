using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public PlayerManager owner;
    public Image image;

    public float lerpTime = 1f;

    private void Update()
    {
        image.fillAmount = Mathf.Lerp(image.fillAmount, owner.health / owner.maxHealth, lerpTime);
    }
}
