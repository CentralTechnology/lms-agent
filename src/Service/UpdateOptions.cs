namespace LMS.Service
{
    using System;
    using System.Collections.Generic;
    using CommandLine;
    using CommandLine.Text;


        [Verb("update", HelpText = "Update settings")]
        public class UpdateOptions
        {
            [Option('p', "pdc-override", HelpText = "Allows the users monitor to run from a member server")]
            public bool? PdcOverride { get; set; }

            [Option("force-users", HelpText = "Enables the user monitoring to run even if the startup checks fail")]
            public bool? UsersOverride { get; set; }

            [Option("force-veeam", HelpText = "Enables the veeam monitoring to run even if the startup checks fail")]
            public bool? VeeamOverride { get; set; }

            [Option('a', "account", HelpText = "Autotask account id")]
            public int? AccountId { get; set; }

            [Option('d', "device", HelpText = "CentraStage device id")]
            public string DeviceId { get; set; }

            [Usage(ApplicationAlias = "lms.exe")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example("Update Autotask Account", new UpdateOptions {AccountId = 12345});
                    yield return new Example("Update CentraStage Device", new UpdateOptions {DeviceId = Guid.NewGuid().ToString()});
                    yield return new Example("Enable PDC override", new UpdateOptions {PdcOverride = true});
                    yield return new Example("Force user monitoring", new UpdateOptions {UsersOverride = true});
                    yield return new Example("Force veeam monitoring", new UpdateOptions {VeeamOverride = true});
                }
            }
        }
    
}