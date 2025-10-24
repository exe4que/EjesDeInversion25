using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EjesDeInversion
{
    public class PdfLinkController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private Button _openButton;
        
        private string _url;
        
        private void OnEnable()
        {
            _openButton.onClick.AddListener(OpenPdf);
        }
        
        private void OnDisable()
        {
            _openButton.onClick.RemoveListener(OpenPdf);
        }
        
        public void SetData(string name, string url)
        {
            _nameText.text = name;
            _url = url;
        }
        
        private void OpenPdf()
        {
            if (!string.IsNullOrEmpty(_url))
            {
                Application.OpenURL(_url);
            }
            else
            {
                Debug.LogWarning("PDF URL is null or empty.");
            }
        }
    }
}