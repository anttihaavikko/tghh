using UnityEngine;

public class Country : MonoBehaviour
{
    [SerializeField] private string capitalName;
    [SerializeField] private Transform capital;

    public string CapitalName => capitalName;
}