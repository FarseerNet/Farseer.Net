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
    public class ONSFactoryProperty : IDisposable
    {
        protected bool swigCMemOwn;
        private HandleRef swigCPtr;

        internal ONSFactoryProperty(IntPtr cPtr, bool cMemoryOwn)
        {
            swigCMemOwn = cMemoryOwn;
            swigCPtr = new HandleRef(this, cPtr);
        }

        public ONSFactoryProperty() : this(ONSClient4CPPPINVOKE.new_ONSFactoryProperty(), true)
        {
        }

        public static string LogPath
        {
            set => ONSClient4CPPPINVOKE.ONSFactoryProperty_LogPath_set(value);
            get
            {
                var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_LogPath_get();
                return ret;
            }
        }

        public static string ProducerId
        {
            set => ONSClient4CPPPINVOKE.ONSFactoryProperty_ProducerId_set(value);
            get
            {
                var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_ProducerId_get();
                return ret;
            }
        }

        public static string ConsumerId
        {
            set => ONSClient4CPPPINVOKE.ONSFactoryProperty_ConsumerId_set(value);
            get
            {
                var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_ConsumerId_get();
                return ret;
            }
        }

        public static string PublishTopics
        {
            set => ONSClient4CPPPINVOKE.ONSFactoryProperty_PublishTopics_set(value);
            get
            {
                var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_PublishTopics_get();
                return ret;
            }
        }

        public static string MsgContent
        {
            set => ONSClient4CPPPINVOKE.ONSFactoryProperty_MsgContent_set(value);
            get
            {
                var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_MsgContent_get();
                return ret;
            }
        }

        public static string ONSAddr
        {
            set => ONSClient4CPPPINVOKE.ONSFactoryProperty_ONSAddr_set(value);
            get
            {
                var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_ONSAddr_get();
                return ret;
            }
        }

        public static string AccessKey
        {
            set => ONSClient4CPPPINVOKE.ONSFactoryProperty_AccessKey_set(value);
            get
            {
                var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_AccessKey_get();
                return ret;
            }
        }

        public static string SecretKey
        {
            set => ONSClient4CPPPINVOKE.ONSFactoryProperty_SecretKey_set(value);
            get
            {
                var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_SecretKey_get();
                return ret;
            }
        }

        public static string MessageModel
        {
            set => ONSClient4CPPPINVOKE.ONSFactoryProperty_MessageModel_set(value);
            get
            {
                var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_MessageModel_get();
                return ret;
            }
        }

        public static string BROADCASTING
        {
            set => ONSClient4CPPPINVOKE.ONSFactoryProperty_BROADCASTING_set(value);
            get
            {
                var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_BROADCASTING_get();
                return ret;
            }
        }

        public static string CLUSTERING
        {
            set => ONSClient4CPPPINVOKE.ONSFactoryProperty_CLUSTERING_set(value);
            get
            {
                var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_CLUSTERING_get();
                return ret;
            }
        }

        public static string SendMsgTimeoutMillis
        {
            set => ONSClient4CPPPINVOKE.ONSFactoryProperty_SendMsgTimeoutMillis_set(value);
            get
            {
                var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_SendMsgTimeoutMillis_get();
                return ret;
            }
        }

        public static string NAMESRV_ADDR
        {
            set => ONSClient4CPPPINVOKE.ONSFactoryProperty_NAMESRV_ADDR_set(value);
            get
            {
                var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_NAMESRV_ADDR_get();
                return ret;
            }
        }

        public static string ConsumeThreadNums
        {
            set => ONSClient4CPPPINVOKE.ONSFactoryProperty_ConsumeThreadNums_set(value);
            get
            {
                var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_ConsumeThreadNums_get();
                return ret;
            }
        }

        public static string OnsChannel
        {
            set => ONSClient4CPPPINVOKE.ONSFactoryProperty_OnsChannel_set(value);
            get
            {
                var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_OnsChannel_get();
                return ret;
            }
        }

        public static string MaxMsgCacheSize
        {
            set => ONSClient4CPPPINVOKE.ONSFactoryProperty_MaxMsgCacheSize_set(value);
            get
            {
                var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_MaxMsgCacheSize_get();
                return ret;
            }
        }

        public static string OnsTraceSwitch
        {
            set => ONSClient4CPPPINVOKE.ONSFactoryProperty_OnsTraceSwitch_set(value);
            get
            {
                var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_OnsTraceSwitch_get();
                return ret;
            }
        }

        public static string SendMsgRetryTimes
        {
            set => ONSClient4CPPPINVOKE.ONSFactoryProperty_SendMsgRetryTimes_set(value);
            get
            {
                var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_SendMsgRetryTimes_get();
                return ret;
            }
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
                        ONSClient4CPPPINVOKE.delete_ONSFactoryProperty(swigCPtr);
                    }
                    swigCPtr = new HandleRef(null, IntPtr.Zero);
                }
                GC.SuppressFinalize(this);
            }
        }

        internal static HandleRef getCPtr(ONSFactoryProperty obj)
        {
            return obj == null ? new HandleRef(null, IntPtr.Zero) : obj.swigCPtr;
        }

        ~ONSFactoryProperty()
        {
            Dispose();
        }

        public bool checkValidityOfFactoryProperties(string key, string value)
        {
            var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_checkValidityOfFactoryProperties(swigCPtr, key, value);
            if (ONSClient4CPPPINVOKE.SWIGPendingException.Pending)
                throw ONSClient4CPPPINVOKE.SWIGPendingException.Retrieve();
            return ret;
        }

        public string getLogPath()
        {
            var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_getLogPath(swigCPtr);
            return ret;
        }

        public void setSendMsgTimeout(int value)
        {
            ONSClient4CPPPINVOKE.ONSFactoryProperty_setSendMsgTimeout(swigCPtr, value);
        }

        public void setSendMsgRetryTimes(int value)
        {
            ONSClient4CPPPINVOKE.ONSFactoryProperty_setSendMsgRetryTimes(swigCPtr, value);
        }

        public void setMaxMsgCacheSize(int size)
        {
            ONSClient4CPPPINVOKE.ONSFactoryProperty_setMaxMsgCacheSize(swigCPtr, size);
        }

        public void setOnsTraceSwitch(bool onswitch)
        {
            ONSClient4CPPPINVOKE.ONSFactoryProperty_setOnsTraceSwitch(swigCPtr, onswitch);
        }

        public void setOnsChannel(ONSChannel onsChannel)
        {
            ONSClient4CPPPINVOKE.ONSFactoryProperty_setOnsChannel(swigCPtr, (int) onsChannel);
            if (ONSClient4CPPPINVOKE.SWIGPendingException.Pending)
                throw ONSClient4CPPPINVOKE.SWIGPendingException.Retrieve();
        }

        public void setFactoryProperty(string key, string value)
        {
            ONSClient4CPPPINVOKE.ONSFactoryProperty_setFactoryProperty(swigCPtr, key, value);
            if (ONSClient4CPPPINVOKE.SWIGPendingException.Pending)
                throw ONSClient4CPPPINVOKE.SWIGPendingException.Retrieve();
        }

        public void setFactoryProperties(SWIGTYPE_p_std__mapT_std__string_std__string_t factoryProperties)
        {
            ONSClient4CPPPINVOKE.ONSFactoryProperty_setFactoryProperties(swigCPtr,
                SWIGTYPE_p_std__mapT_std__string_std__string_t.getCPtr(factoryProperties));
            if (ONSClient4CPPPINVOKE.SWIGPendingException.Pending)
                throw ONSClient4CPPPINVOKE.SWIGPendingException.Retrieve();
        }

        public SWIGTYPE_p_std__mapT_std__string_std__string_t getFactoryProperties()
        {
            var ret = new SWIGTYPE_p_std__mapT_std__string_std__string_t(
                ONSClient4CPPPINVOKE.ONSFactoryProperty_getFactoryProperties(swigCPtr), true);
            return ret;
        }

        public string getProducerId()
        {
            var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_getProducerId(swigCPtr);
            return ret;
        }

        public string getConsumerId()
        {
            var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_getConsumerId(swigCPtr);
            return ret;
        }

        public string getPublishTopics()
        {
            var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_getPublishTopics(swigCPtr);
            return ret;
        }

        public string getMessageModel()
        {
            var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_getMessageModel(swigCPtr);
            return ret;
        }

        public int getSendMsgTimeout()
        {
            var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_getSendMsgTimeout(swigCPtr);
            return ret;
        }

        public int getSendMsgRetryTimes()
        {
            var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_getSendMsgRetryTimes(swigCPtr);
            return ret;
        }

        public int getConsumeThreadNums()
        {
            var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_getConsumeThreadNums(swigCPtr);
            return ret;
        }

        public int getMaxMsgCacheSize()
        {
            var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_getMaxMsgCacheSize(swigCPtr);
            return ret;
        }

        public ONSChannel getOnsChannel()
        {
            var ret = (ONSChannel) ONSClient4CPPPINVOKE.ONSFactoryProperty_getOnsChannel(swigCPtr);
            return ret;
        }

        public string getChannel()
        {
            var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_getChannel(swigCPtr);
            return ret;
        }

        public string getMessageContent()
        {
            var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_getMessageContent(swigCPtr);
            return ret;
        }

        public string getNameSrvAddr()
        {
            var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_getNameSrvAddr(swigCPtr);
            return ret;
        }

        public string getNameSrvDomain()
        {
            var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_getNameSrvDomain(swigCPtr);
            return ret;
        }

        public string getAccessKey()
        {
            var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_getAccessKey(swigCPtr);
            return ret;
        }

        public string getSecretKey()
        {
            var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_getSecretKey(swigCPtr);
            return ret;
        }

        public bool getOnsTraceSwitch()
        {
            var ret = ONSClient4CPPPINVOKE.ONSFactoryProperty_getOnsTraceSwitch(swigCPtr);
            return ret;
        }
    }
}