using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Windows.Storage;

namespace IoPokeMikuClient.Model
{
    public class CloudClient
    {
        public delegate void CloudClientEventHandler(object sender, CloudClientEventArgs args);

        public event CloudClientEventHandler DataReceived;

        private MqttClient m_client;

        public CloudClient()
        {
        }

        public async Task<bool> Connect()
        {
            ConnectionInfo info = await LoadSetting();
            Connect(info);

            return true;
        }

        public async Task<bool> SaveSetting(ConnectionInfo info)
        {
            bool ret = false;
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = null;
            try
            {
                file = await folder.CreateFileAsync("setting.json", CreationCollisionOption.ReplaceExisting);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("setting file not found. " + Environment.NewLine + ex.ToString());
            }

            try
            {
                string json = JsonConvert.SerializeObject(info);
                await FileIO.WriteTextAsync(file, json);
                ret = true;
            }
            catch (FileNotFoundException ex)
            {
                Debug.WriteLine("file not found. " + ex.ToString());
            }
            catch (JsonSerializationException ex)
            {
                Debug.WriteLine("Parse failed" + ex.ToString());
            }
            return ret;
        }

        public async Task<ConnectionInfo> LoadSetting()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            var info = new ConnectionInfo();
            StorageFile file = null;
            try
            {
                file = await folder.GetFileAsync("setting.json");
            } catch(Exception ex)
            {
                Debug.WriteLine("setting file not found. " + Environment.NewLine + ex.ToString());
            }
            if(file == null)
            {
                return info;
            }

            try
            {
                string json = await FileIO.ReadTextAsync(file);
                info = JsonConvert.DeserializeObject<ConnectionInfo>(json);
            }
            catch(FileNotFoundException ex)
            {
                Debug.WriteLine("file not found. " + ex.ToString());
            }
            catch (JsonSerializationException ex)
            {
                Debug.WriteLine("Parse failed" + ex.ToString());
            }
            return info;
        }

        private void Connect(ConnectionInfo info)
        {
            // SSL使用時はtrue、CAを指定
            m_client = new MqttClient(info.BrokerHostName, info.BrokerPort, false, MqttSslProtocols.None);
            // clientidを生成
            string clientId = Guid.NewGuid().ToString();
            m_client.Connect(clientId, info.UserName, info.Password);
            //{AppID}/{データストア名}/{push|send|set|remove}
            Subscribe(info.TopicName);
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
