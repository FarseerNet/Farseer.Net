using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace FS.Utils.Component.WinSocket
{
    /// <summary>
    ///     客户端套接字
    /// </summary>
    public class ClientSocket : IDisposable
    {
        /// <summary>
        ///     套接字
        /// </summary>
        private Socket _socket;

        /// <summary>
        ///     服务器端口
        /// </summary>
        private readonly IPEndPoint _server;

        /// <summary>
        ///     接收信息时执行
        /// </summary>
        private Action<StateObject> _actReceive;

        /// <summary>
        ///     服务端（监听）Socker
        /// </summary>
        /// <param name="socket">套接字</param>
        public ClientSocket(Socket socket)
        {
            _socket = socket;
            _server = (IPEndPoint) _socket.RemoteEndPoint;
        }

        /// <summary>
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port">端口</param>
        /// <param name="addressFamily"></param>
        /// <param name="socketType"></param>
        /// <param name="protocolType"></param>
        public ClientSocket(string ip, int port, AddressFamily addressFamily = AddressFamily.InterNetwork, SocketType socketType = SocketType.Stream, ProtocolType protocolType = ProtocolType.Tcp)
        {
            _socket = new Socket(addressFamily, socketType, protocolType);
            _server = new IPEndPoint(IPAddress.Parse(ip), port);
        }

        /// <summary>
        ///     视情况决定是否需要绑定本地
        /// </summary>
        /// <param name="ip"></param>
        public void Bind(IPEndPoint ip)
        {
            _socket.Bind(ip);
        }

        /// <summary>
        ///     初始化Socket的接收回调
        /// </summary>
        /// <param name="actReceive">接收信息时触发</param>
        public void Init(Action<StateObject> actReceive)
        {
            _actReceive = actReceive;
        }

        /// <summary>
        ///     开启服务（需要先执行Init方法）
        /// </summary>
        public void Start()
        {
            _socket.Connect(_server);
            var state = new StateObject(1024, _socket);
            _socket.BeginReceive(state.Buffer, 0, state.BufferSize, 0, ReceiveCallback, state);
        }

        /// <summary>
        ///     停止服务
        /// </summary>
        public void Stop()
        {
            if (_socket != null)
            {
                if (_socket.Connected) { _socket.Shutdown(SocketShutdown.Both); }
                _socket.Close();
                _socket.Dispose();
                _socket = null;
            }
        }

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="bufferSize"></param>
        /// <param name="sendCallBack">发送成功之后的回调</param>
        public void Send(string msg, int bufferSize = 1024, Action<StateObject> sendCallBack = null)
        {
            var byteData = Encoding.ASCII.GetBytes(msg);
            Send(byteData, bufferSize, sendCallBack);
        }

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="bufferSize"></param>
        /// <param name="sendCallBack">发送成功之后的回调</param>
        public void Send(byte[] buffer, int bufferSize = 1024, Action<StateObject> sendCallBack = null)
        {
            _socket.BeginSend(buffer, 0, buffer.Length, 0, SendCallback, new StateObject(bufferSize, _socket, sendCallBack));
        }

        /// <summary>
        ///     发送后，马上返回接收的信息
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port">端口</param>
        /// <param name="msg"></param>
        /// <param name="bufferSize"></param>
        /// <param name="sendCallBack">发送成功之后的回调</param>
        /// <param name="addressFamily"></param>
        /// <param name="socketType"></param>
        /// <param name="protocolType"></param>
        public static string SendToReceive(string ip, int port, string msg, int bufferSize = 1024, Action<StateObject> sendCallBack = null, AddressFamily addressFamily = AddressFamily.InterNetwork, SocketType socketType = SocketType.Stream, ProtocolType protocolType = ProtocolType.Tcp)
        {
            using (var socket = new ClientSocket(ip, port, addressFamily, socketType, protocolType))
            {
                socket.Start();
                return socket.SendToReceive(msg, bufferSize);
            }
        }

        /// <summary>
        ///     发送后，马上返回接收的信息
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port">端口</param>
        /// <param name="msg"></param>
        /// <param name="bufferSize"></param>
        /// <param name="sendCallBack">发送成功之后的回调</param>
        /// <param name="addressFamily"></param>
        /// <param name="socketType"></param>
        /// <param name="protocolType"></param>
        public static byte[] SendToReceive(string ip, int port, byte[] msg, int bufferSize = 1024, Action<StateObject> sendCallBack = null, AddressFamily addressFamily = AddressFamily.InterNetwork, SocketType socketType = SocketType.Stream, ProtocolType protocolType = ProtocolType.Tcp)
        {
            using (var socket = new ClientSocket(ip, port, addressFamily, socketType, protocolType))
            {
                socket.Start();
                return socket.SendToReceive(msg, bufferSize);
            }
        }

        /// <summary>
        ///     发送后，马上返回接收的信息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="bufferSize"></param>
        /// <param name="sendCallBack">发送成功之后的回调</param>
        public byte[] SendToReceive(byte[] msg, int bufferSize = 1024, Action<StateObject> sendCallBack = null)
        {
            byte[] result = null;
            this.Init(o => { result = o.Buffer; });
            this.Send(msg, bufferSize, sendCallBack);
            var resetCount = 0;
            while (result == null && ++resetCount < 30) { Thread.Sleep(100); }
            return result;
        }

        /// <summary>
        ///     发送后，马上返回接收的信息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="bufferSize"></param>
        /// <param name="sendCallBack">发送成功之后的回调</param>
        public string SendToReceive(string msg, int bufferSize = 1024, Action<StateObject> sendCallBack = null)
        {
            var result = string.Empty;
            this.Init(o => { result = o.Msg; });
            this.Send(msg, bufferSize, sendCallBack);
            var resetCount = 0;
            while (string.IsNullOrWhiteSpace(result) && ++resetCount < 30) { Thread.Sleep(100); }
            return result;
        }

        /// <summary>
        ///     发送成功之后的回调
        /// </summary>
        /// <param name="ar"></param>
        private void SendCallback(IAsyncResult ar)
        {
            var state = ar.AsyncState as StateObject;
            try { state.ByteCount = state.WorkSocket.EndSend(ar); }
            catch
            {
                state.ByteCount = 0;
                state.Buffer = new byte[0];
            }

            state.SendCallBack();
        }

        /// <summary>
        ///     接收信息时的回调
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveCallback(IAsyncResult ar)
        {
            var state = ar.AsyncState as StateObject;
            try { state.ByteCount = state.WorkSocket.EndReceive(ar); }
            catch
            {
                state.ByteCount = 0;
                state.Buffer = new byte[0];
            }

            // 接收到信息时，客户端的回调
            if (_actReceive != null) { _actReceive(state); }
        }

        #region IDisposable 成员

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Stop();
        }

        #endregion
    }
}
