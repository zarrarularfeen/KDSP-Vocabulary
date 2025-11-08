using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DropTarget : MonoBehaviour, IDropHandler
{
    public string word;

    public void OnDrop(PointerEventData eventData)
    {
        DraggableCard dragged  = eventData.pointerDrag.GetComponent<DraggableCard>();
        if (dragged == null)
        {
            return;
        }

        if (dragged.word == word)
        {
            Debug.Log("Correct match for word: " + word);
            Destroy(dragged.gameObject);
            VocabularyMatching.Instance.OnCorrectMatch();   
        }
        else
        {
            Debug.Log("Incorrect match for word: " + word);
            dragged.ResetPosition();
        }
    }
}
