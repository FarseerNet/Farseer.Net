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
using System.Runtime.InteropServices;

namespace FS.MQ.RocketMQ.SDK
{
    public class ONSClientException : IDisposable
    {
        private HandleRef swigCPtr;
        protected bool swigCMemOwn;

        internal ONSClientException(IntPtr cPtr, bool cMemoryOwn)
        {
            swigCMemOwn = cMemoryOwn;
            swigCPtr = new HandleRef(this, cPtr);
        }

        internal static HandleRef getCPtr(ONSClientException obj) { return obj == null ? new HandleRef(null, IntPtr.Zero) : obj.swigCPtr; }

        ~ONSClientException() { Dispose(); }

        public virtual void Dispose()
        {
            lock (this)
            {
                if (swigCPtr.Handle != IntPtr.Zero)
                {
                    if (swigCMemOwn)
                    {
                        swigCMemOwn = false;
                        ONSClient4CPPPINVOKE.delete_ONSClientException(swigCPtr);
                    }
                    swigCPtr = new HandleRef(null, IntPtr.Zero);
                }
                GC.SuppressFinalize(this);
            }
        }

        public ONSClientException() : this(ONSClient4CPPPINVOKE.new_ONSClientException__SWIG_0(), true) { }

        public ONSClientException(string msg, int error) : this(ONSClient4CPPPINVOKE.new_ONSClientException__SWIG_1(msg, error), true)
        {
            if (ONSClient4CPPPINVOKE.SWIGPendingException.Pending) throw ONSClient4CPPPINVOKE.SWIGPendingException.Retrieve();
        }

        public string GetMsg()
        {
            var ret = ONSClient4CPPPINVOKE.ONSClientException_GetMsg(swigCPtr);
            return ret;
        }

        public string what()
        {
            var ret = ONSClient4CPPPINVOKE.ONSClientException_what(swigCPtr);
            return ret;
        }

        public int GetError()
        {
            var ret = ONSClient4CPPPINVOKE.ONSClientException_GetError(swigCPtr);
            return ret;
        }
    }
}