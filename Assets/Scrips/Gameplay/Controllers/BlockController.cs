using UnityEngine;

// RequireComponent to force the components into the gameObject
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class BlockController : MonoBehaviour
{
    public BlockData currentData { get; private set; }
    public Rigidbody rb { get; private set; }
    private bool hasReportedLanding = false;

    public static Transform lastPlacedBlock;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // kinematic to have it stuck to the crane 
        rb.isKinematic = true;
    }

    public void Initialize(BlockData data)
    {
        currentData = data;

        this.transform.localScale = data.scale;

        GetComponent<MeshFilter>().mesh = data.blockMesh;
        GetComponent<MeshRenderer>().material = data.blockMaterial;

        rb.mass = data.mass;
        rb.linearDamping = data.linearDamping;
        rb.angularDamping = data.angularDamping;

        this.gameObject.tag = "Block";
    }

    public void DropBlock(Vector3 inheritedVelocity)
    {
        rb.isKinematic = false;
        rb.AddForce(inheritedVelocity, ForceMode.VelocityChange);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathZone"))
        {
            GameplayManager.s_instance.OnBlockHitDeathZone(this);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Base"))
        {
            GameplayManager.s_instance.OnBlockHitBase(this);
            return;
        }

        if (hasReportedLanding) return;

        if (collision.gameObject.CompareTag("Block"))
        {
            hasReportedLanding = true;
            GameplayManager.s_instance.OnBlockLanded(this, collision.gameObject.tag);
        }
    }
}