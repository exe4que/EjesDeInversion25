using EjesDeInversion.Managers;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace EjesDeInversion
{
    public class SearchBarResultElementController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private Button _button;
        
        private PointerController _pointerController;
        private SearchBarController _searchBarController;

        public void Initialize(PointerController pointerController, SearchBarController searchBarController)
        {
            _pointerController = pointerController;
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(OnButtonClicked);
            _nameText.text = pointerController.Data.Name;
            _searchBarController = searchBarController;
        }

        private void OnButtonClicked()
        {
            _searchBarController.ClearInputField();
            _searchBarController.HideResults();
            CameraManager.GoToPointer(_pointerController);
        }
    }
}