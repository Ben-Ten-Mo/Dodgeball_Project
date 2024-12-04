using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DodgeballController : MonoBehaviour
{

    private bool liveBall;
    private bool carriedBall;
    [HideInInspector]
    public DodgeballAgent thrownBy;
    [SerializeField] public float dropForwardForce, dropUpwardForce;
    
    public Rigidbody rb;
    public SphereCollider coll;

    public GameObject arena;

    public DodgeballEnvController envController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        envController = GetComponentInParent<DodgeballEnvController>();

        liveBall = false;
        coll.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEpisodeBegin() {
        // Debug.Log("Ball Episode Begin called");
        transform.SetParent(arena.transform);
        transform.localPosition = new Vector3(Random.Range(-4f, 4f), 0.5f, 0f);
        rb.isKinematic = false;
        rb.useGravity = true;
        liveBall = false;
        thrownBy = null;
    }


    public void pickUpBall(Transform playerTransform, DodgeballAgent thrower)
    {
        // Debug.Log("Ball pickUpBall called");
        if(!liveBall) {
            rb.isKinematic = true;
            coll.enabled = false;
            transform.SetParent(playerTransform);
            transform.localPosition = new Vector3(0, 0, 1);
            transform.localRotation = Quaternion.Euler(Vector3.zero);
            thrownBy = thrower;
        }
    }

    public void throwBall(Transform playerTransform, DodgeballAgent thrownBy)
    {
        // Debug.Log($"Ball throwBall called {thrownBy.teamID}");
        if(!liveBall) {
            liveBall = true;
            transform.SetParent(arena.transform);
            rb.isKinematic = false;
            rb.useGravity = false;
            coll.enabled = true;

            // Debug.Log(player.forward * dropForwardForce);
            rb.AddForce(playerTransform.forward * dropForwardForce, ForceMode.Impulse);
        }
    }


    public void OnCollisionEnter(Collision collision) {
        // Debug.Log("Ball collided with " + collision.gameObject.tag); 

        if (collision.gameObject.CompareTag("Floor")) {
            rb.useGravity = false;
            rb.linearVelocity = new Vector3(0, 0, 0);
            rb.isKinematic = true;
        }
        if (liveBall && collision.gameObject.CompareTag("Ceiling") ) {
            thrownBy.AddReward(-2f);
            rb.useGravity = true;
            thrownBy = null;
            liveBall = false;
        } else if (liveBall && collision.gameObject.CompareTag("Wall") ) { //|| collision.gameObject.CompareTag("Floor") ) ) {
            rb.useGravity = true;
            liveBall = false;
            thrownBy = null;
            // Debug.Log("Ball thrown into wall");
        } else if (liveBall && thrownBy != null && collision.gameObject.CompareTag("Player")) {
            liveBall = false;
            DodgeballAgent playerHit = collision.gameObject.GetComponent<DodgeballAgent>();
            // Debug.Log($"PlayerHit {playerHit} by {thrownBy}" );
            envController.PlayerHit(playerHit, thrownBy);
            rb.useGravity = true;
            thrownBy = null;
            // Debug.Log("Ball thrown into Player");
        } else if (liveBall && thrownBy != null && collision.gameObject.CompareTag("EnemyPlayer")) {
            liveBall = false;
            DodgeballAgent playerHit = collision.gameObject.GetComponent<DodgeballAgent>();
            // Debug.Log($"PlayerHit {playerHit} by {thrownBy}" );
            envController.PlayerHit(playerHit, thrownBy);
            rb.useGravity = true;
            thrownBy = null;
            // Debug.Log("Ball thrown into EnemyPlayer");
        } else {
            liveBall = false;
            thrownBy = null;
            Debug.Log("Ball hit: " + collision.gameObject);
            rb.useGravity = true;
        }
    }
}
