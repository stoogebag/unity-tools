#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldCreatorTesting : MonoBehaviour
{
    [SerializeField] private FieldCreator _fieldCreator;


    #region FieldCreator
	[SerializeField]private VerticalLayoutGroup _dialogueCanvas;
[SerializeField]private Transform _blocks;
[SerializeField]private Button _Next;

    #endregion
    
    public int testInt;
}
#endif