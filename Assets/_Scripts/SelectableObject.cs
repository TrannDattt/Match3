// using Match3.Interfaces;
// using UnityEngine;
// using UnityEngine.Events;
// using UnityEngine.EventSystems;
// using static UnityEngine.EventSystems.PointerEventData;

// namespace Match3.Blocks
// {
//     public class SelectableObject : MonoBehaviour
//     {
//         private SpriteRenderer _renderer;
//         private Vector3 _originScale;

//         public bool IsSelect {get; private set;}
//         public UnityEvent<bool> OnSelect {get; private set;}

//         public void Select()
//         {
//             // Debug.Log($"Select: {gameObject.name}");
//             IsSelect = true;
//             _renderer.gameObject.transform.localScale = 1.2f * _originScale;
//             OnSelect?.Invoke(true);
//         }

//         public void Hover()
//         {
//             // Debug.Log($"Hover: {gameObject.name}");
//             IsSelect = false;
//             _renderer.gameObject.transform.localScale = 1.2f * _originScale;
//         }

//         public void Deselect()
//         {
//             // Debug.Log($"Deselect: {gameObject.name}");
//             _renderer.gameObject.transform.localScale = _originScale;
//             OnSelect?.Invoke(false);
//         }

//         void Start()
//         {
//             _renderer = GetComponentInChildren<SpriteRenderer>();
//             _originScale = _renderer.gameObject.transform.localScale;
//         }
//     }
// }
