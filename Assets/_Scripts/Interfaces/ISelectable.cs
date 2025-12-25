using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Match3.Interfaces
{
    public interface ISelectable : IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public bool IsSelect {get; set;}
        public UnityEvent OnSelect {get; set;}
    }
}
