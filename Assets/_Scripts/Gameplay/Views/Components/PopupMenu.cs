// using Match3.Controls;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Views.Components
{
    public abstract class PopupMenu : MonoBehaviour
    {
        protected RectTransform _parent;
        private Button _raycastBlocker;

        public virtual void Open()
        {
            _parent.gameObject.SetActive(true);
        }

        public virtual void Close()
        {
            _parent.gameObject.SetActive(false);
        }

        protected virtual void Awake()
        {
            _parent = GetComponent<RectTransform>();
            _raycastBlocker = GetComponentsInChildren<Button>().FirstOrDefault(b => b.gameObject.CompareTag("RaycastBlocker"));
            _raycastBlocker?.onClick.AddListener(Close);
        }

        protected virtual void Start()
        {
            Close();
        }
    }
}
