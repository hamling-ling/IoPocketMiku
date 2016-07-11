using System;
using System.Diagnostics;

namespace IoPokeMikuClient.Model
{
    interface ILogOutput
    {
        void Write(string msg);
    }

    class DataLogger
    {
        private static readonly Lazy<DataLogger> _instance = new Lazy<DataLogger>(() => new DataLogger());

        private ILogOutput m_output;

        private DataLogger()
        { }

        public static DataLogger Instance { get { return _instance.Value; } }

        public static void Write(string msg)
        {
            if (msg == null)
            {
                return;
            }
            if(Instance.m_output == null)
            {
                Debug.WriteLine("output is not initialized");
                return;
            }

            string timedMsg = DateTime.Now.ToString("[yyyy/MM/dd hh:mm:ss:fff] ") + msg;
            Instance.m_output.Write(timedMsg);
        }

        public void SetOutput(ILogOutput output)
        {
            Instance.m_output = output;
        }
    }
}
