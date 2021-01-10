using DummyClient;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    ServerSession _serverSession = new ServerSession();

    public void Send(ArraySegment<byte> sendBuff)
    {
        _serverSession.Send(sendBuff);
    }

    // Start is called before the first frame update
    void Start()
    {
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

        Connector connector = new Connector();
        connector.Connecct(endPoint, () => { return _serverSession; }, 1);

        
    }

    // Update is called once per frame
    void Update()
    {
        List<IPacket> list = PacketQueue.Instance.PopAll();

        foreach(IPacket packet in list)
        {
            PacketManager.Instance.HandlePacket(_serverSession, packet);
        }
    }

    
}
