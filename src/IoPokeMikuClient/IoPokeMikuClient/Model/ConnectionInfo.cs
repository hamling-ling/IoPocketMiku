using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoPokeMikuClient.Model
{
    public class ConnectionInfo
    {
        public string AppId { get; set; }
        public string DataStore { get; set; }
        public string BrokerHostName { get; set; }
        public int BrokerPort { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string TopicName { get; set; }
        public bool IsEmpty { get; set; }

        public ConnectionInfo()
        {
            BrokerHostName = "";
            UserName = "";
            BrokerPort = 0;
            Password = "";
            TopicName = "";
            IsEmpty = true;
        }

        public ConnectionInfo(string appId, string dataStore)
        {
            AppId = appId;
            DataStore = dataStore;
            BrokerHostName = appId + ".mlkcca.com";
            UserName = "sdammy";
            BrokerPort = 1883;
            Password = appId;
            TopicName = appId + "/" + dataStore + "/push";
            IsEmpty = false;
        }
    }
}
