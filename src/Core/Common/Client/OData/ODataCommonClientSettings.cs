namespace Core.Common.Client.OData
{
    using System;
    using Abp;
    using Abp.Dependency;
    using Abp.Extensions;
    using Abp.Threading;
    using Administration;
    using Simple.OData.Client;

    public abstract class ODataCommonClientSettings : ODataClientSettings
    {
        protected static Guid DeviceId { get; set; }

        protected static string Token { get; set; }

        protected void ValidateDeviceId()
        {
            using (IDisposableDependencyObjectWrapper<ISettingsManager> settingManager = IocManager.Instance.ResolveAsDisposable<ISettingsManager>())
            {
                if (DeviceId == Guid.Empty)
                {
                    DeviceId = settingManager.Object.Read().DeviceId;

                    if (DeviceId == Guid.Empty)
                    {
                        throw new AbpException($"Cannot perform web request when device id is {Guid.Empty}");
                    }
                }
            }
        }

        protected void ValidateToken()
        {
            if (Token.IsNullOrEmpty())
            {
                using (IDisposableDependencyObjectWrapper<PortalClient> portalClient = IocManager.Instance.ResolveAsDisposable<PortalClient>())
                {
                    // ReSharper disable once AccessToDisposedClosure
                    Token = AsyncHelper.RunSync(() => portalClient.Object.GetTokenCookie());
                }
            }
        }
    }
}