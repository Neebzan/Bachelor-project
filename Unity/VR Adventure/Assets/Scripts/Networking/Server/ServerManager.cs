using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    public static ServerManager instance;

    public GameObject playerPrefab;
    public GameObject vrPlayerPrefab;
    public GameObject projectilePrefab;
    public GameObject FireballPrefab;

    public Dictionary<int, Fireball> Fireballs = new Dictionary<int, Fireball>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Debug.Log("Instance already exists!");
            Destroy(this);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        Server.Start(10, 27000);
    }

    private void FixedUpdate()
    {
        ServerPacketSender.UpdateFireballsCollection();

        ServerPacketSender.PlayerPositions();
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    public Projectile SpawnProjectile(Vector3 pos)
    {
        return Instantiate(projectilePrefab, pos, Quaternion.identity).GetComponent<Projectile>();
    }

    public Player SpawnPlayer()
    {
        return Instantiate(vrPlayerPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity).GetComponent<Player>();
        //return Instantiate(playerPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity).GetComponent<Player>();
    }

    public Fireball SpawnFireball () {
        Fireball fireball = Instantiate(FireballPrefab).GetComponent<Fireball>();
        Fireballs.Add(fireball.ID, fireball);
        return fireball;
    }
}
