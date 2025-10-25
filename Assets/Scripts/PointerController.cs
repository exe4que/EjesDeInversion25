using System;
using EjesDeInversion.Data;
using TMPro;
using UnityEngine;

namespace EjesDeInversion
{
    public class PointerController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        
        private PointerData _data;
        
        public void Initialize(PointerData data)
        {
            _data = data;
            _text.text = $"<b>{data.Name}</b>\n{data.ShortDescription}";
        }

        private void LateUpdate()
        {
            Vector3 targetPosition = _data.Position;
            // world to screen
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetPosition);
            screenPosition.z = 0;
            // follow the screen position
            transform.position = screenPosition;
        }
    }
}