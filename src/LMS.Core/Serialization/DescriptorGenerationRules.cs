namespace LMS.Core.Serialization
{
    using System;

    [Flags]
    public enum DescriptorGenerationRules
    {
        None = 0,
        Fields = 1,
        Properties = 2,
    }
}
