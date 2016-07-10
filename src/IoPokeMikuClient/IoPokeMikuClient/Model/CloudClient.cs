using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace IoPokeMikuClient.Model
{
    public struct ConnectionInfo
    {
        public string brokerHostName;
        public int brokerPort;
        public string userName;
        public string password;
    }

    public class CloudClientEventArgs : EventArgs
    {
        public PitchInfo PitchInfo { get; private set; }

        public CloudClientEventArgs(PitchInfo info)
        {
            PitchInfo = info;
        }
    }

    public class CloudClient
    {
        public delegate void CloudClientEventHandler(object sender, CloudClientEventArgs args);

        public event CloudClientEventHandler DataReceived;

        private MqttClient m_client;

        public CloudClient()
        {
        }

        public bool Connect()
        {
            ConnectionInfo info;
            LoadSetting(out info);
            Connect(info);
            return true;
        }

        private void LoadSetting(out ConnectionInfo info)
        {
            info.brokerHostName = "";
            info.userName = "";
            info.brokerPort = 0;
            info.password = "";
        }

        private void Connect(ConnectionInfo info)
        {
            // SSL使用時はtrue、CAを指定
            m_client = new MqttClient(info.brokerHostName, info.brokerPort, false, MqttSslProtocols.None);
            // clientidを生成
            string clientId = Guid.NewGuid().ToString();
            m_client.Connect(clientId, info.userName, info.password);
            //{AppID}/{データストア名}/{push|send|set|remove}
            Subscribe("catipxwt08x/test1/push");
        }

        private void OnReceive(object sender, MqttMsgPublishEventArgs e)
        {
            Debug.WriteLine(e.Topic);
            string json = Encoding.UTF8.GetString(e.Message);
            Debug.WriteLine(json);

            var handler = DataReceived;
            if (handler == null)
            {
                return;
            }

            RawData rd;
            try
            {
                rd = JsonConvert.DeserializeObject<RawData>(json);
            }
            catch (JsonSerializationException ex)
            {
                Debug.WriteLine("Parse failed" + ex.ToString());
                return;
            }

            var args = new CloudClientEventArgs(rd.pitch);
            handler(this, args);
        }

        private void Publish(string topic, string msg)
        {
            m_client.Publish(
                topic, Encoding.UTF8.GetBytes(msg),
                MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);
        }

        private void Subscribe(string topic)
        {
            // callbackを登録
            m_client.MqttMsgPublishReceived += this.OnReceive;
            m_client.Subscribe(new string[] { topic }, new byte[] {
                MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
        }
    }
}
