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
        ///     服务器端口
        /// </summary>
        private readonly IPEndPoint _server;

        /// <summary>
        ///     接收信息时执行
        /// </summary>
        private Action<StateObject> _actReceive;

        /// <summary>
        ///     套接字
        /// </summary>
        private Socket _socket;

        /// <summary>
        ///     服务端（监听）Socker
        /// </summary>
        /// <param name="socket"> 套接字 </param>
        public ClientSocket(Socket socket)
        {
            _socket = socket;
            _server = (IPEndPoint)_socket.RemoteEndPoint;
        }

        /// <summary>
        /// </summary>
        /// <param name="ip"> </param>
        /// <param name="port"> 端口 </param>
        /// <param name="addressFamily"> </param>
        /// <param name="socketType"> </param>
        /// <param name="protocolType"> </param>
        public ClientSocket(string ip, int port, AddressFamily addressFamily = AddressFamily.InterNetwork, SocketType socketType = SocketType.Stream, ProtocolType protocolType = ProtocolType.Tcp)
        {
            _socket = new Socket(addressFamily: addressFamily, socketType: socketType, protocolType: protocolType);
            _server = new IPEndPoint(address: IPAddress.Parse(ipString: ip), port: port);
        }

        #region IDisposable 成员

        /// <summary>
        /// </summary>
        public void Dispose()
        {
            Stop();
        }

        #endregion

        /// <summary>
        ///     视情况决定是否需要绑定本地
        /// </summary>
        /// <param name="ip"> </param>
        public void Bind(IPEndPoint ip)
        {
            _socket.Bind(localEP: ip);
        }

        /// <summary>
        ///     初始化Socket的接收回调
        /// </summary>
        /// <param name="actReceive"> 接收信息时触发 </param>
        public void Init(Action<StateObject> actReceive)
        {
            _actReceive = actReceive;
        }

        /// <summary>
        ///     开启服务（需要先执行Init方法）
        /// </summary>
        public void Start()
        {
            _socket.Connect(remoteEP: _server);
            var state = new StateObject(bufferSize: 1024, socket: _socket);
            _socket.BeginReceive(buffer: state.Buffer, offset: 0, size: state.BufferSize, socketFlags: 0, callback: ReceiveCallback, state: state);
        }

        /// <summary>
        ///     停止服务
        /// </summary>
        public void Stop()
        {
            if (_socket != null)
            {
                if (_socket.Connected) _socket.Shutdown(how: SocketShutdown.Both);
                _socket.Close();
                _socket.Dispose();
                _socket = null;
            }
        }

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="msg"> </param>
        /// <param name="bufferSize"> </param>
        /// <param name="sendCallBack"> 发送成功之后的回调 </param>
        public void Send(string msg, int bufferSize = 1024, Action<StateObject> sendCallBack = null)
        {
            var byteData = Encoding.ASCII.GetBytes(s: msg);
            Send(buffer: byteData, bufferSize: bufferSize, sendCallBack: sendCallBack);
        }

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="buffer"> </param>
        /// <param name="bufferSize"> </param>
        /// <param name="sendCallBack"> 发送成功之后的回调 </param>
        public void Send(byte[] buffer, int bufferSize = 1024, Action<StateObject> sendCallBack = null)
        {
            _socket.BeginSend(buffer: buffer, offset: 0, size: buffer.Length, socketFlags: 0, callback: SendCallback, state: new StateObject(bufferSize: bufferSize, socket: _socket, sendCallBack: sendCallBack));
        }

        /// <summary>
        ///     发送后，马上返回接收的信息
        /// </summary>
        /// <param name="ip"> </param>
        /// <param name="port"> 端口 </param>
        /// <param name="msg"> </param>
        /// <param name="bufferSize"> </param>
        /// <param name="sendCallBack"> 发送成功之后的回调 </param>
        /// <param name="addressFamily"> </param>
        /// <param name="socketType"> </param>
        /// <param name="protocolType"> </param>
        public static string SendToReceive(string ip, int port, string msg, int bufferSize = 1024, Action<StateObject> sendCallBack = null, AddressFamily addressFamily = AddressFamily.InterNetwork, SocketType socketType = SocketType.Stream, ProtocolType protocolType = ProtocolType.Tcp)
        {
            using (var socket = new ClientSocket(ip: ip, port: port, addressFamily: addressFamily, socketType: socketType, protocolType: protocolType))
            {
                socket.Start();
                return socket.SendToReceive(msg: msg, bufferSize: bufferSize);
            }
        }

        /// <summary>
        ///     发送后，马上返回接收的信息
        /// </summary>
        /// <param name="ip"> </param>
        /// <param name="port"> 端口 </param>
        /// <param name="msg"> </param>
        /// <param name="bufferSize"> </param>
        /// <param name="sendCallBack"> 发送成功之后的回调 </param>
        /// <param name="addressFamily"> </param>
        /// <param name="socketType"> </param>
        /// <param name="protocolType"> </param>
        public static byte[] SendToReceive(string ip, int port, byte[] msg, int bufferSize = 1024, Action<StateObject> sendCallBack = null, AddressFamily addressFamily = AddressFamily.InterNetwork, SocketType socketType = SocketType.Stream, ProtocolType protocolType = ProtocolType.Tcp)
        {
            using (var socket = new ClientSocket(ip: ip, port: port, addressFamily: addressFamily, socketType: socketType, protocolType: protocolType))
            {
                socket.Start();
                return socket.SendToReceive(msg: msg, bufferSize: bufferSize);
            }
        }

        /// <summary>
        ///     发送后，马上返回接收的信息
        /// </summary>
        /// <param name="msg"> </param>
        /// <param name="bufferSize"> </param>
        /// <param name="sendCallBack"> 发送成功之后的回调 </param>
        public byte[] SendToReceive(byte[] msg, int bufferSize = 1024, Action<StateObject> sendCallBack = null)
        {
            byte[] result = null;
            Init(actReceive: o =>
            {
                result = o.Buffer;
            });
            Send(buffer: msg, bufferSize: bufferSize, sendCallBack: sendCallBack);
            var resetCount = 0;
            while (result == null && ++resetCount < 30) Thread.Sleep(millisecondsTimeout: 100);
            return result;
        }

        /// <summary>
        ///     发送后，马上返回接收的信息
        /// </summary>
        /// <param name="msg"> </param>
        /// <param name="bufferSize"> </param>
        /// <param name="sendCallBack"> 发送成功之后的回调 </param>
        public string SendToReceive(string msg, int bufferSize = 1024, Action<StateObject> sendCallBack = null)
        {
            var result = string.Empty;
            Init(actReceive: o =>
            {
                result = o.Msg;
            });
            Send(msg: msg, bufferSize: bufferSize, sendCallBack: sendCallBack);
            var resetCount = 0;
            while (string.IsNullOrWhiteSpace(value: result) && ++resetCount < 30) Thread.Sleep(millisecondsTimeout: 100);
            return result;
        }

        /// <summary>
        ///     发送成功之后的回调
        /// </summary>
        /// <param name="ar"> </param>
        private void SendCallback(IAsyncResult ar)
        {
            var state = ar.AsyncState as StateObject;
            try
            {
                state.ByteCount = state.WorkSocket.EndSend(asyncResult: ar);
            }
            catch
            {
                state.ByteCount = 0;
                state.Buffer    = new byte[0];
            }

            state.SendCallBack();
        }

        /// <summary>
        ///     接收信息时的回调
        /// </summary>
        /// <param name="ar"> </param>
        private void ReceiveCallback(IAsyncResult ar)
        {
            var state = ar.AsyncState as StateObject;
            try
            {
                state.ByteCount = state.WorkSocket.EndReceive(asyncResult: ar);
            }
            catch
            {
                state.ByteCount = 0;
                state.Buffer    = new byte[0];
            }

            // 接收到信息时，客户端的回调
            if (_actReceive != null) _actReceive(obj: state);
        }
    }
}