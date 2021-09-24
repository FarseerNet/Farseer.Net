using System.Collections.Generic;
using FS.MQ.Rocket.SDK.Http.Util;

namespace FS.MQ.Rocket.SDK.Http.Model
{
    public class Message
    {
        public string Id { get; set; }

        public string ReceiptHandle { get; set; }

        public string Body { get; set; }

        public string BodyMD5 { get; set; }

        public string MessageTag { get; set; }

        public long PublishTime { get; set; }

        public long NextConsumeTime { get; set; }

        public long FirstConsumeTime { get; set; }

        public uint ConsumedTimes { get; set; }

        public IDictionary<string, string> Properties { get; } = new Dictionary<string, string>();

        public string MessageKey => Properties[key: Constants.MESSAGE_PROPERTIES_MSG_KEY];

        public long StartDeliverTime => Properties.ContainsKey(key: Constants.MESSAGE_PROPERTIES_TIMER_KEY)
                                            ? long.Parse(s: Properties[key: Constants.MESSAGE_PROPERTIES_TIMER_KEY])
                                            : 0;

        public uint TransCheckImmunityTime => Properties.ContainsKey(key: Constants.MESSAGE_PROPERTIES_TRANS_CHECK_KEY)
                                                  ? uint.Parse(s: Properties[key: Constants.MESSAGE_PROPERTIES_TRANS_CHECK_KEY])
                                                  : 0;

        // Check to see if Id property is set
        internal bool IsSetId() => Id != null;

        internal bool IsSetReceiptHandle() => ReceiptHandle != null;

        internal bool IsSetBody() => Body != null;

        internal bool IsSetBodyMD5() => BodyMD5 != null;

        public string GetProperty(string key) => Properties[key: key];

        public override string ToString() => string.Format(
                                                           format: "ID:{0}, PublishTime:{1}, NextConsumeTime:{2}, ConsumedTimes:{3}, " +
                                                                   "\nTag:{4}, BodyMD5:{5}, NextConsumeTime:{6}"                       +
                                                                   "\nBody:{7}"                                                        +
                                                                   "\nProperties:{8}"                                                  +
                                                                   "\nMessageKey:{9}",
                                                           Id, PublishTime, NextConsumeTime, ConsumedTimes,
                                                           MessageTag, BodyMD5, NextConsumeTime, Body, AliyunSDKUtils.DictToString(dict: Properties),
                                                           MessageKey
                                                          );
    }
}