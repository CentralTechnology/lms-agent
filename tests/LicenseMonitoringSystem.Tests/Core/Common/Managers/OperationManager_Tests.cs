using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseMonitoringSystem.Tests.Core.Common.Managers
{
    using LMS.Common.Managers;
    using Shouldly;
    using Xunit;

    public class OperationManager_Tests : LicenseMonitoringSystemTestBase
    {
        private readonly OperationManager _operationManager;
        public OperationManager_Tests()
        {
            _operationManager = LocalIocManager.Resolve<OperationManager>();
        }

        [Fact]
        public void Add_ShouldCreateList_ShouldAddValue()
        {
            // arrange
            var minute = 5;

            // act 
            _operationManager.MinutesHistory.ShouldBe(null);
            _operationManager.Add(minute);

            // assert
            _operationManager.MinutesHistory.Count.ShouldBe(1);
            _operationManager.MinutesHistory.First().ShouldBe(minute);
        }

        [Fact]
        public void AddMultiple_ShouldDiscardTheFirstValue()
        {
            // act
            _operationManager.Add(10);
            _operationManager.Add(9);
            _operationManager.Add(8);
            _operationManager.Add(7);
            _operationManager.Add(6);
            _operationManager.Add(5);

            // assert
            _operationManager.MinutesHistory.Count.ShouldBe(5);
        }

        [Fact]
        public void Get_ShouldReturnCorrectAverage()
        {
            // arrange
            _operationManager.Add(10);
            _operationManager.Add(9);
            _operationManager.Add(8);
            _operationManager.Add(7);
            _operationManager.Add(6);
            _operationManager.Add(5);

            // assert
            _operationManager.Get().ShouldBe(7);
        }
    }
}
