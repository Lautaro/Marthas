using UnityEngine;
using System.Collections;

public class PlatformPickup : MonoBehaviour
{
    public bool isDead;

    // Use this for initialization
    void Start()
    {
        var newX = Random.Range(-15, 15);
        var newY = Random.Range(3, 11);
        transform.position = new Vector2(newX, newY);
        var forceX = Random.Range(-20f, 20f);
        var forceY = Random.Range(5f, 25f);
        GetComponent<Rigidbody2D>().AddForce(new Vector2(forceX, forceY));
        Invoke("DieOfAge", 10);
    }
    

    public void Die()
    {
        isDead = true;
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        gameObject.GetComponent<Collider2D>().enabled = false;
        GetComponent<ParticleSystem>().Stop(true);
        Destroy(gameObject, 2f);
    }

    void DieOfAge()
    {
        SFXMan.sfx_Shot.Play();
        Die();
    }
}
