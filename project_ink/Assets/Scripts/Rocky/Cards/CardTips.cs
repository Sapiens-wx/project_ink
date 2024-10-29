using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardTips : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI cardName, damage, description;
    public void ShowTip(Card card){
        cardName.text=card.type.ToString();
        damage.text=card.damage.ToString();
        description.text=card.description;
        panel.SetActive(true);
    }
    public void HideTip(){
        panel.SetActive(false);
    }
}
