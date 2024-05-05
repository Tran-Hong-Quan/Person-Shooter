using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TestDragButton : TestingDragUI, IBeginDragHandler, IEndDragHandler
{
    Button but; 
    private void Awake()
    {
        but= GetComponent<Button>();
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        but.interactable = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        but.interactable = true;
    }
}
