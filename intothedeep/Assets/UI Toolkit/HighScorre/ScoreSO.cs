using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ScoreSO", order = 1)]
public class ScoreSO : ScriptableObject
{
    public string Name;
    public float score;
}