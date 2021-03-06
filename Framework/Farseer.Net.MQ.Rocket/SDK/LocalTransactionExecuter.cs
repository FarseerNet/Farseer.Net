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
    public class LocalTransactionExecuter : IDisposable
    {
        public delegate int SwigDelegateLocalTransactionExecuter_0(IntPtr msg);

        private static readonly Type[] swigMethodTypes0 = {typeof(Message)};
        protected bool swigCMemOwn;
        private HandleRef swigCPtr;

        private SwigDelegateLocalTransactionExecuter_0 swigDelegate0;

        internal LocalTransactionExecuter(IntPtr cPtr, bool cMemoryOwn)
        {
            swigCMemOwn = cMemoryOwn;
            swigCPtr = new HandleRef(this, cPtr);
        }

        public LocalTransactionExecuter() : this(ONSClient4CPPPINVOKE.new_LocalTransactionExecuter(), true)
        {
            SwigDirectorConnect();
        }

        public virtual void Dispose()
        {
            lock (this)
            {
                if (swigCPtr.Handle != IntPtr.Zero)
                {
                    if (swigCMemOwn)
                    {
                        swigCMemOwn = false;
                        ONSClient4CPPPINVOKE.delete_LocalTransactionExecuter(swigCPtr);
                    }
                    swigCPtr = new HandleRef(null, IntPtr.Zero);
                }
                GC.SuppressFinalize(this);
            }
        }

        internal static HandleRef getCPtr(LocalTransactionExecuter obj)
        {
            return obj == null ? new HandleRef(null, IntPtr.Zero) : obj.swigCPtr;
        }

        ~LocalTransactionExecuter()
        {
            Dispose();
        }

        public virtual TransactionStatus execute(Message msg)
        {
            var ret = (TransactionStatus) ONSClient4CPPPINVOKE.LocalTransactionExecuter_execute(swigCPtr,
                Message.getCPtr(msg));
            if (ONSClient4CPPPINVOKE.SWIGPendingException.Pending)
                throw ONSClient4CPPPINVOKE.SWIGPendingException.Retrieve();
            return ret;
        }

        private void SwigDirectorConnect()
        {
            if (SwigDerivedClassHasMethod("execute", swigMethodTypes0))
                swigDelegate0 = SwigDirectorexecute;
            ONSClient4CPPPINVOKE.LocalTransactionExecuter_director_connect(swigCPtr, swigDelegate0);
        }

        private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
        {
            var methodInfo = GetType().GetMethod(methodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, methodTypes, null);
            var hasDerivedMethod = methodInfo.DeclaringType.IsSubclassOf(typeof(LocalTransactionExecuter));
            return hasDerivedMethod;
        }

        private int SwigDirectorexecute(IntPtr msg)
        {
            return (int) execute(new Message(msg, false));
        }
    }
}