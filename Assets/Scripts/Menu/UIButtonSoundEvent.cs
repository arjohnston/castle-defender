 using UnityEngine;
 using UnityEngine.UI;
 using UnityEngine.EventSystems;
 using System.Collections;
 
 public class UIButtonSoundEvent : MonoBehaviour, IPointerEnterHandler {    
 
     public void OnPointerEnter( PointerEventData ped ) {
        SoundManager.Instance.Play(Sounds.MENU_BUTTON_HOVER);
     }
 }
