using EjesDeInversion.Data;
using EjesDeInversion.Utilities;
using UnityEngine;

namespace EjesDeInversion
{
    public class DataManager : Singleton<DataManager>
    {
        [SerializeField] private MainBarData _mainBarData;
        
        public static MainBarData MainBarData => Instance._mainBarData;
    }
}