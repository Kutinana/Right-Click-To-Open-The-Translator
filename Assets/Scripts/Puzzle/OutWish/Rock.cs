using System.Collections;
using System.Collections.Generic;
using DataSystem;
using Localization;
using UI;
using UnityEngine;

namespace Outwish
{
    public class Rock : MonoBehaviour
    {
        public void ExecuteAtDoor()
        {
            if (GameProgressData.GetInventory().TryGetValue("bomb", out var value) && value >= 1)
            {
                var bomb = GameDesignData.GetObtainableObjectDataById("bomb");
                DialogBoxController.CallUp(LocalizationHelper.Get("Str_UseItemConfirm", LocalizationHelper.Get(bomb.Name)),
                    confirmCallback: () => { 
                        transform.parent.Find("Outwish_4-trigger1").gameObject.SetActive(true);
                        GameProgressData.Solve("OutWish");
                    });
            }
            else
            {
                ShortMessageController.CallUp(LocalizationHelper.Get("Str_RoadIsBlocked"));
            }
        }
    }

}