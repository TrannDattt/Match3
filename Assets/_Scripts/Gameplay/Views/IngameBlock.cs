using DG.Tweening;
using Match3.Interfaces;
using Match3.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Match3.Views
{
    public partial class IngameBlockView : MonoBehaviour, ISelectable
    {
        [Header("Initialize")]
        [SerializeField] private SpriteRenderer _renderer;
        // [SerializeField] private BlockControl _control;
        
        public BlockModel Block {get; private set;}
        public bool IsSelect {get; set;} = false;
        public UnityEvent OnSelect {get; set;} = new();

        private Vector3 _originScale;

        public void UpdateView(BlockModel block)
        {
            Block = block;
            gameObject.name = $"{block.Type}";
            _renderer.sprite = block.Sprite;

            IsSelect = false;
        }

        public void Move(Vector3 pos, float duration)
        {
            // await transform.DOMove(pos, duration).SetEase(Ease.InOutQuad).AsyncWaitForCompletion();
            transform.DOMove(pos, duration).SetEase(Ease.InOutQuad).WaitForCompletion();
        }

        private void Scale(float mult, float duration)
        {
            _renderer.transform.DOScale(mult * _originScale, duration).SetEase(Ease.InOutQuad);
        }

        #region Mouse Interaction
        public void Select()
        {
            IsSelect = true;
            Scale(1.2f, 0.1f);
        }

        public void Hover()
        {
            Scale(1.2f, 0.1f);
        }

        public void Deselect()
        {
            IsSelect = false;
            Scale(1f, 0.1f);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Hover();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // Debug.Log($"Quit hover {gameObject.name} with select state = {IsSelect}");
            if (IsSelect) Select();
            else Deselect();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // Debug.Log($"Click on: {gameObject.name}");
            OnSelect?.Invoke();
        }
        #endregion

        void Awake()
        {
            _originScale = _renderer.gameObject.transform.localScale;
        }
    }
}
