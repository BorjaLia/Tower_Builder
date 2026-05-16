using UnityEngine;

// RequireComponent to force the components into the gameObject
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class BlockController : MonoBehaviour
{
    public BlockData currentData { get; private set; }
    private Rigidbody rb;
    private bool hasLanded = false;

    private float placedY;

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

    public void DropBlock(Vector3 dir)
    {
        rb.isKinematic = false;
        //rb.AddForce(dir*10,ForceMode.VelocityChange);
        rb.AddForce(rb.linearVelocity,ForceMode.VelocityChange);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathZone"))
        {
            if (!hasLanded)
            {
                HandleDeathZone();
            }
            else
            {
                if (transform.position.y < placedY - 0.5f)
                {
                    HandleDeathZone();
                }
                //else
                //{
                //    if (rb.isKinematic)
                //    {
                //        Destroy(this.gameObject);
                //    }
                //}
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasLanded) return;

        // Check base only if first block
        bool hitBaseValidly = collision.gameObject.CompareTag("Base") && lastPlacedBlock == null;
        bool hitBlock = collision.gameObject.CompareTag("Block");

        // Check if collision is valid
        if (hitBaseValidly || hitBlock)
        {
            hasLanded = true;

            placedY = transform.position.y;

            bool isPerfectPlacement = false;
            float tolerance = 0.15f;

            if (lastPlacedBlock != null)
            {
                float differenceX = Mathf.Abs(transform.position.x - lastPlacedBlock.position.x);
                if (differenceX <= tolerance)
                {
                    isPerfectPlacement = true;
                    transform.position = new Vector3(lastPlacedBlock.position.x, transform.position.y, transform.position.z);
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
            else
            {
                //First block is always perfect
                isPerfectPlacement = true;
            }

            lastPlacedBlock = this.transform;
            GameplayManager.s_instance.BlockLanded(this, isPerfectPlacement);

            // AudioManager.Instance.PlaySFX("BlockImpact");
        }
        else if (collision.gameObject.CompareTag("Base") && lastPlacedBlock != null)
        {
            HandleDeathZone();
        }
    }

    private void HandleDeathZone()
    {
        GameplayManager.s_instance.BlockFellFromTower(this);

        // TODO Particles
        Destroy(this.gameObject);
    }
}