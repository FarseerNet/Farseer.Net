//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 3.0.10
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace FS.MQ.RocketMQ.SDK
{
    public class OrderProducer : IDisposable
    {
        private HandleRef swigCPtr;
        protected bool swigCMemOwn;

        internal OrderProducer(IntPtr cPtr, bool cMemoryOwn)
        {
            swigCMemOwn = cMemoryOwn;
            swigCPtr = new HandleRef(this, cPtr);
        }

        internal static HandleRef getCPtr(OrderProducer obj) { return obj == null ? new HandleRef(null, IntPtr.Zero) : obj.swigCPtr; }

        ~OrderProducer() { Dispose(); }

        public virtual void Dispose()
        {
            lock (this)
            {
                if (swigCPtr.Handle != IntPtr.Zero)
                {
                    if (swigCMemOwn)
                    {
                        swigCMemOwn = false;
                        ONSClient4CPPPINVOKE.delete_OrderProducer(swigCPtr);
                    }
                    swigCPtr = new HandleRef(null, IntPtr.Zero);
                }
                GC.SuppressFinalize(this);
            }
        }

        public OrderProducer() : this(ONSClient4CPPPINVOKE.new_OrderProducer(), true) { SwigDirectorConnect(); }

        public virtual void start() { ONSClient4CPPPINVOKE.OrderProducer_start(swigCPtr); }

        public virtual void shutdown() { ONSClient4CPPPINVOKE.OrderProducer_shutdown(swigCPtr); }

        public virtual SendResultONS send(Message msg, string shardingKey)
        {
            var ret = new SendResultONS(ONSClient4CPPPINVOKE.OrderProducer_send(swigCPtr, Message.getCPtr(msg), shardingKey), true);
            if (ONSClient4CPPPINVOKE.SWIGPendingException.Pending) throw ONSClient4CPPPINVOKE.SWIGPendingException.Retrieve();
            return ret;
        }

        private void SwigDirectorConnect()
        {
            if (SwigDerivedClassHasMethod("start", swigMethodTypes0)) swigDelegate0 = SwigDirectorstart;
            if (SwigDerivedClassHasMethod("shutdown", swigMethodTypes1)) swigDelegate1 = SwigDirectorshutdown;
            if (SwigDerivedClassHasMethod("send", swigMethodTypes2)) swigDelegate2 = SwigDirectorsend;
            ONSClient4CPPPINVOKE.OrderProducer_director_connect(swigCPtr, swigDelegate0, swigDelegate1, swigDelegate2);
        }

        private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
        {
            var methodInfo = GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, methodTypes, null);
            var hasDerivedMethod = methodInfo.DeclaringType.IsSubclassOf(typeof(OrderProducer));
            return hasDerivedMethod;
        }

        private void SwigDirectorstart() { start(); }

        private void SwigDirectorshutdown() { shutdown(); }

        private IntPtr SwigDirectorsend(IntPtr msg, string shardingKey) { return SendResultONS.getCPtr(send(new Message(msg, false), shardingKey)).Handle; }

        public delegate void SwigDelegateOrderProducer_0();

        public delegate void SwigDelegateOrderProducer_1();

        public delegate IntPtr SwigDelegateOrderProducer_2(IntPtr msg, string shardingKey);

        private SwigDelegateOrderProducer_0 swigDelegate0;
        private SwigDelegateOrderProducer_1 swigDelegate1;
        private SwigDelegateOrderProducer_2 swigDelegate2;

        private static readonly Type[] swigMethodTypes0 = { };
        private static readonly Type[] swigMethodTypes1 = { };
        private static readonly Type[] swigMethodTypes2 = {typeof(Message), typeof(string)};
    }
}