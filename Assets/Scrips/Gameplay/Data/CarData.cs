using UnityEngine;

// Scriptable object for cars
[CreateAssetMenu(fileName = "NewCarData", menuName = "TowerBuilder/Car Data", order = 1)]
public class CarData : ScriptableObject
{
    [Header("General")]
    public string carName = "Default Car";
    
    [Header("Visual")]
    public GameObject carModel;

    [Header("Gameplay")]
    public Vector2 speedRange;
}