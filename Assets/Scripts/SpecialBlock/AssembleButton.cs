using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AssembleButton : MonoBehaviour, IPointerClickHandler
{
    public ToolAssemblerManager toolAssemblerManager;

    public void OnPointerClick(PointerEventData eventData){
        toolAssemblerManager.TryAssembleTool();
    }
}