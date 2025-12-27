using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public string word;
    private CanvasGroup canvasGroup;
    private Canvas canvas;
    private RectTransform rectTransform;
    private Vector2 startPosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvas = GetComponentInParent<Canvas>();
    }

    void Start()
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = rectTransform.anchoredPosition;
        Debug.Log("start position in OnBeginDrag function: " + startPosition);
        canvasGroup.blocksRaycasts = false; // Allow raycasts to pass through
    }
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true; // Restore raycast blocking
        Debug.Log("start position in OnEndDrag function before setting value: " + startPosition);
        rectTransform.anchoredPosition = startPosition; // Reset position if not dropped on a target
        Debug.Log("start position in OnEndDrag function after setting value: " + startPosition);    
    }
    
    public void ResetPosition()
    {
        rectTransform.anchoredPosition = startPosition;
    }

    
}
