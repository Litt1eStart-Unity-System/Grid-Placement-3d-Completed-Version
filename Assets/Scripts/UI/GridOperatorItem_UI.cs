using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridOperatorItem_UI : MonoBehaviour
{
    [SerializeField] private GridOperator gridOperator; 

    public void OnItemClicked()
    {
        GameManager.Instance.currentOperator = gridOperator;
    }
}
