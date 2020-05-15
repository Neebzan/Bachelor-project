using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public static Dictionary<int, Projectile> Projectiles = new Dictionary<int, Projectile>();
    public static int idIndex = 0;

    public int id;

    // Start is called before the first frame update

    public void Init(Vector3 dir)
    {
        id = idIndex;
        idIndex++;
        Projectiles.Add(id, this);

        GetComponent<Rigidbody>().AddForce(dir * 300f);
        ServerPacketSender.SpawnProjectile(this);
        StartCoroutine(Remove());
    }

    public void Init(int id)
    {
        Projectiles.Add(id, this);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ServerPacketSender.ProjectilePosition(this);
    }

    public IEnumerator Remove()
    {
        yield return new WaitForSeconds(5f);
        ServerPacketSender.DespawnProjectile(this);
        Despawn();
    }

    public void Emulate(Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
    }

    public void Despawn()
    {
        Projectiles.Remove(id);
        Destroy(gameObject);
    }
}
