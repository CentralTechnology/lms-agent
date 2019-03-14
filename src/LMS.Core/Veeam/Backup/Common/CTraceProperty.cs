using System;

namespace LMS.Core.Veeam.Backup.Common
{
    [Serializable]
    public class CTraceProperty
    {
        public readonly string Key;
        public readonly string Value;

        public CTraceProperty(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", (object) this.Key, (object) this.Value);
        }
    }
}
