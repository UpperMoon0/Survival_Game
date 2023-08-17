using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class RightClickHandler : MonoBehaviour, IPointerClickHandler
{
    public event Action OnRightClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick?.Invoke();
        }
    }
}