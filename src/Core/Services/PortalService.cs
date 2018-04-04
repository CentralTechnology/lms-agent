using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Services
{
    using Abp;
    using Abp.Dependency;
    using Common.Managers;
    using Default;

    public class PortalService : LMSManagerBase, IPortalService, IShouldInitialize
    {
        private Container _container;

        public PortalService()
        {

        }

        public void Initialize()
        {
            ConfigureContainer();
        }

        private void ConfigureContainer()
        {
            _container = new Container(new Uri("https://api-v2.portal.ct.co.uk/odata"));            

            _container.ReceivingResponse += _container_ReceivingResponse;
            _container.SendingRequest2 += _container_SendingRequest2;
        }

        private void _container_SendingRequest2(object sender, Microsoft.OData.Client.SendingRequest2EventArgs e)
        {
            
        }

        private void _container_ReceivingResponse(object sender, Microsoft.OData.Client.ReceivingResponseEventArgs e)
        {

        }
    }
}
