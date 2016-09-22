using UnityEngine;
using System.Collections;

public interface IGUIBehavior
{
    void Show(string item);
    void Hide(string item);
    void ShowAll();
    void HideAll();
    void EnableButtonInteraction(string item);
    void DisableButtonInteraction(string item);
    void EnableAllButtonInteraction();
    void DisableAllButtonInteraction();
}
