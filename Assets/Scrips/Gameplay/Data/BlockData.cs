using UnityEngine;

// Scriptable object for blocks
[CreateAssetMenu(fileName = "NewBlockData", menuName = "TowerBuilder/Block Data", order = 1)]
public class BlockData : ScriptableObject
{
    [Header("General")]

    public string blockName = "Default Block";


    [Header("Visual")]

    [Tooltip("The 3D model to instantiate")]
    public GameObject blockModel;

    [Header("Physics")]

    [Tooltip("Weight")]
    public float mass = 1.0f;

    [Tooltip("Resistance to movement")]
    public float linearDamping = 1.0f;

    [Tooltip("Resistance to rotation")]
    public float angularDamping = 1.0f;


    [Header("Gameplay")]

    [Tooltip("Points for landing")]
    public int scoreValue = 10;

    [Tooltip("Multiplier for perfect landing")]
    public float perfectMultiplier = 1.0f;

    [Tooltip("Weighted spawn chance")]
    public int spawnWeight = 100;

    [Tooltip("Scale")]
    public Vector3 scale = new Vector3(1.0f,1.0f,1.0f);

}