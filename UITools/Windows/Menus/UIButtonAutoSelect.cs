using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


    [RequireComponent(typeof(Selectable))]
    public class UIButtonAutoSelect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDeselectHandler
    {
        private Selectable selectable;
        private bool isPointerOver = false;

        void Awake()
        {
            selectable = GetComponent<Selectable>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isPointerOver = true;
            
            if (selectable != null && selectable.interactable)
            {
                if (EventSystem.current != null)
                {
                    EventSystem.current.SetSelectedGameObject(gameObject);
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isPointerOver = false;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (isPointerOver && EventSystem.current.currentSelectedGameObject != gameObject)
            {
                if (selectable != null && selectable.interactable)
                {
                    if (EventSystem.current != null)
                    {
                        EventSystem.current.SetSelectedGameObject(gameObject);
                    }
                }
            }
        }

        void OnDisable()
        {
            isPointerOver = false;
        }
    }
