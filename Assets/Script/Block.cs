using UnityEngine;

public class Block : MonoBehaviour
{
    private DistanceJoint2D joint;
    private GameController gameController;
    private Rigidbody2D rb;
    public int blockId;

    public bool isCurrent = false;
    
    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        gameController = FindObjectOfType<GameController>();
    }

    public void SetupBlock(Rigidbody2D hookRb, int blockId){
        joint = GetComponent<DistanceJoint2D>();
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.mass = 1f;
        joint.connectedBody = hookRb;
        joint.enabled = true;

        this.blockId = blockId;
    }   

    public void Release(){
        if (joint != null)
        {
            joint.enabled = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (!isCurrent) return;

        if (collision.gameObject.CompareTag("DeathZone")) {
            gameController.onMissedLanding();
            return;
        }
        gameController.StartStabilityCheck(this);
    }
}