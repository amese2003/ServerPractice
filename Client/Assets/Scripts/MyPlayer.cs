using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : Player
{
    NetworkManager _network;

    // Start is called before the first frame update
    void Start()
    {        
        _network = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        StartCoroutine(CoSendPacket());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator CoSendPacket()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.25f);

            C_Move movePacket = new C_Move();
            movePacket.posX = UnityEngine.Random.Range(-50, 50);
            movePacket.posY = 1;
            movePacket.posZ = UnityEngine.Random.Range(-50, 50);

            _network.Send(movePacket.Write());
        }
    }
}
