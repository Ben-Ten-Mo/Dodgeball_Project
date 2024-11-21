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
        Debug.Log("Ball Episode Begin called");
        transform.position = new Vector3(Random.Range(-4f, 4f), 0.5f, 0f);
        transform.SetParent(null);
        liveBall = false;
        thrownBy = null;
    }


    public void pickUpBall(Transform playerTransform, DodgeballAgent thrower)
    {
        Debug.Log("Ball pickUpBall called");
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
        Debug.Log("Ball throwBall called");
        if(!liveBall && thrownBy != null) {
            liveBall = true;
            transform.SetParent(null);
            rb.isKinematic = false;
            rb.useGravity = false;
            coll.enabled = true;

            // Debug.Log(player.forward * dropForwardForce);
            rb.AddForce(playerTransform.forward * dropForwardForce, ForceMode.Impulse);
        }
    }

    public void OnCollisionEnter(Collision collision) {
        Debug.Log("Ball collided with " + collision.gameObject.tag);
        if (liveBall && collision.gameObject.CompareTag("Wall")) {
            rb.useGravity = true;
            liveBall = false;
            thrownBy = null;
            Debug.Log("Ball thrown into wall");
        } else if (liveBall && collision.gameObject.CompareTag("Player")) {
            DodgeballAgent playerHit = collision.gameObject.GetComponent<DodgeballAgent>();
            envController.PlayerHit(playerHit, thrownBy);
            rb.useGravity = true;
            liveBall = false;
            thrownBy = null;
            Debug.Log("Ball thrown into Player");
        } else if (liveBall && collision.gameObject.CompareTag("EnemyPlayer")) {
            DodgeballAgent playerHit = collision.gameObject.GetComponent<DodgeballAgent>();
            envController.PlayerHit(playerHit, thrownBy);
            rb.useGravity = true;
            liveBall = false;
            thrownBy = null;
            Debug.Log("Ball thrown into EnemyPlayer");
        }
    }
}
