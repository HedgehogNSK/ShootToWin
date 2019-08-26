using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter
{
    public enum ConnectionStatus
    {
        Server,
        Client,
        Host,
    }
    static public class ConnectionSettings
    {
        static public void SetConnectionStatus(ConnectionStatus conn)
        {
            PlayerPrefs.SetInt("ConnectionStatus", (int)conn);
            PlayerPrefs.Save();
        }
        static public ConnectionStatus GetConnectionStatus()
        {
            return (ConnectionStatus)PlayerPrefs.GetInt("ConnectionStatus", -1);
        }
        
        static public void SetMatchIPAdress(string adress)
        {
            PlayerPrefs.SetString("IP", adress);
            PlayerPrefs.Save();
        }

        static public string GetIPMatchAdress()
        {
            return PlayerPrefs.GetString("IP", "0.0.0.0");
        }
        
    }
}
