﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FS.Utils.Component.WinSocket
{
    /// <summary>
    ///     服务端（监听）Socker
    /// </summary>
    public class ServerSocket
    {
        /// <summary>
        ///     接收客户端请求连接时执行
        /// </summary>
        private Action<StateObject> _actAccept;

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
        public ServerSocket(Socket socket)
        {
            _socket = socket;
            var server = (IPEndPoint)_socket.LocalEndPoint;
            Port = server.Port;
        }

        /// <summary>
        ///     服务端（监听）Socker
        /// </summary>
        /// <param name="port"> 端口 </param>
        /// <param name="listen"> 监听排队数 </param>
        /// <param name="bufferSize"> 缓存区大小 </param>
        public ServerSocket(int port, int listen, int bufferSize = 1024)
        {
            Port       = port;
            Listen     = listen;
            BufferSize = bufferSize;
        }

        /// <summary>
        ///     端口
        /// </summary>
        public int Port { get; }

        /// <summary>
        ///     监听排队数
        /// </summary>
        private int Listen { get; }

        /// <summary>
        ///     已连接的客户端列表
        /// </summary>
        public Dictionary<IPEndPoint, Socket> LstClient { get; set; }

        /// <summary>
        ///     当前运行总的连接人数
        /// </summary>
        public int AllNum { get; set; }

        /// <summary>
        ///     当前连接人数
        /// </summary>
        public int CurrentNum => LstClient.Count;

        /// <summary>
        ///     缓存区大小
        /// </summary>
        public int BufferSize { get; set; }

        /// <summary>
        ///     初始化Socket的监听、接收回调
        /// </summary>
        /// <param name="actAccept"> 监听请求连接时触发 </param>
        /// <param name="actReceive"> 接收信息时触发 </param>
        public void Init(Action<StateObject> actAccept, Action<StateObject> actReceive)
        {
            _actAccept  = actAccept;
            _actReceive = actReceive;
        }

        /// <summary>
        ///     开启服务
        /// </summary>
        public void Start()
        {
            LstClient = new Dictionary<IPEndPoint, Socket>();
            _socket   = new Socket(addressFamily: AddressFamily.InterNetwork, socketType: SocketType.Stream, protocolType: ProtocolType.Tcp);
            _socket.Bind(localEP: new IPEndPoint(address: IPAddress.Any, port: Port));
            _socket.Listen(backlog: Listen);
            _socket.BeginAccept(callback: AcceptEnd, state: new StateObject(bufferSize: BufferSize));
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

            foreach (var item in LstClient)
            {
                item.Value.Close();
                item.Value.Dispose();
            }

            LstClient.Clear();
        }

        /// <summary>
        ///     接收客户机连接的方法
        /// </summary>
        /// <param name="ar"> 回调参数 </param>
        private void AcceptEnd(IAsyncResult ar)
        {
            try
            {
                if (_socket == null) return;

                var state = ar.AsyncState as StateObject;
                // 获取客户端的Socket
                state.WorkSocket = _socket.EndAccept(asyncResult: ar);

                // 添加到客户端列表
                var ep = state.WorkSocket.RemoteEndPoint as IPEndPoint;
                LstClient[key: ep] = state.WorkSocket;

                AllNum++;

                // 执行传入的委托
                if (_actAccept != null) _actAccept(obj: state);

                // 客户端接受消息
                state.WorkSocket.BeginReceive(buffer: state.Buffer, offset: 0, size: state.BufferSize, socketFlags: 0, callback: ReceiveEnd, state: ar.AsyncState);


                // 继续接收下一个客户机连接
                _socket.BeginAccept(callback: AcceptEnd, state: new StateObject(bufferSize: BufferSize));
            }
            catch
            {
            }
        }

        /// <summary>
        ///     接收线程
        /// </summary>
        /// <param name="ar"> 回调参数 </param>
        private void ReceiveEnd(IAsyncResult ar)
        {
            var state = ar.AsyncState as StateObject;
            if (state == null) return;

            try
            {
                state.ByteCount = state.WorkSocket.EndReceive(asyncResult: ar);
            }
            catch
            {
                state.ByteCount = 0;
                state.Buffer    = new byte[0];
            }

            if (state.WorkSocket.Connected && state.ByteCount > 0)
                state.WorkSocket.BeginReceive(buffer: state.Buffer, offset: 0, size: state.BufferSize, socketFlags: 0, callback: ReceiveEnd, state: state);
            else // 客户端关闭
            {
                // 添加到客户端列表
                var ep = state.WorkSocket.RemoteEndPoint as IPEndPoint;
                LstClient.Remove(key: ep);
            }

            // 执行传入的委托
            if (_actReceive != null) _actReceive(obj: state);
        }

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="msg"> 要发送的消息 </param>
        /// <param name="sendCallBack"> 发送成功之后的回调 </param>
        public void Send(string msg, Action<StateObject> sendCallBack = null)
        {
            var byteData = Encoding.ASCII.GetBytes(s: msg);
            Send(msg: byteData, sendCallBack: sendCallBack);
        }

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="msg"> 要发送的消息 </param>
        /// <param name="sendCallBack"> 发送成功之后的回调 </param>
        public void Send(byte[] msg, Action<StateObject> sendCallBack = null)
        {
            _socket.BeginSend(buffer: msg, offset: 0, size: msg.Length, socketFlags: 0, callback: SendCallback, state: new StateObject(bufferSize: BufferSize, socket: _socket, sendCallBack: sendCallBack));
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
    }
}