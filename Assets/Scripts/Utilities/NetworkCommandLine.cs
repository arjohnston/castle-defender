using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/*
    To open the game via command prompt:
    - Build the project (ctrl-B)
    - Navigate to the project in the command prompt
    - Run the project: "Castle Defender.exe" -mlapi <protocol>
        - Where protocol: host, server, client

    Note: the host is both the server and client.
*/

public class NetworkCommandLine : MonoBehaviour
{
   private NetworkManager netManager;

   void Start()
   {
       netManager = GetComponentInParent<NetworkManager>();

    //    if (Application.isEditor) {
    //        Logger.Instance.LogInfo("Detected Unity Editor mode. Starting networking as Client.");
    //        netManager.StartClient();
    //        return;
    //     };

       var args = GetCommandlineArgs();

       if (args.TryGetValue("-mlapi", out string mlapiValue))
       {
           switch (mlapiValue)
           {
                case "server":
                    netManager.StartServer();
                    break;
                case "host":
                    netManager.StartHost();
                    break;
                case "client":
                    netManager.StartClient();
                    break;
           }
       }
   }

   private Dictionary<string, string> GetCommandlineArgs()
   {
       Dictionary<string, string> argDictionary = new Dictionary<string, string>();

       var args = System.Environment.GetCommandLineArgs();

       for (int i = 0; i < args.Length; ++i)
       {
           var arg = args[i].ToLower();
           if (arg.StartsWith("-"))
           {
               var value = i < args.Length - 1 ? args[i + 1].ToLower() : null;
               value = (value?.StartsWith("-") ?? false) ? null : value;

               argDictionary.Add(arg, value);
           }
       }
       return argDictionary;
   }
}
