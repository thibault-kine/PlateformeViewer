using UnityEngine.EventSystems;
using UnityEngine;

/// <summary>
/// Attach to a UI Image. IsHeld is true while the finger/pointer is down on this element.
/// </summary>
public class HoldButton : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public bool IsHeld { get; private set; }

    public void OnPointerDown(PointerEventData e) => IsHeld = true;
    public void OnPointerUp(PointerEventData e)   => IsHeld = false;
    public void OnPointerExit(PointerEventData e)  => IsHeld = false; // finger slid off
}
