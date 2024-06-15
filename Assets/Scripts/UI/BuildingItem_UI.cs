using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingItem_UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buildingSizeTxt;
    [SerializeField] private BuildingSO buildingData;
    private Button button;
    private void Start()
    {
        buildingSizeTxt.text = buildingData.Size.x + " x " + buildingData.Size.y;
    }

    public void OnButtonClick()
    {
        GameManager.Instance.ChangeBuldingData(buildingData);
    }
}
