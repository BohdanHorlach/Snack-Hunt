using UnityEngine;


[CreateAssetMenu(fileName = "DialogInfos", menuName = "Resources/UI/DialogInfo")]
public class DialogInfo : ScriptableObject
{
    public DialogPart[] Lines;
}