
using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nailpod.Http;
using Nailpod.Http.Endpoints.ErrorInfoEndpoint.Models;
using Nailpod.Services;

namespace NailpodManager.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
        }


        [TestMethod]
        public async Task ApiTestErrorInfoAsync()
        {
            var res = new ErrorInfoResponseObj();
            var req = new ErrorInfoRequestObj { MachineID = 0, ErrorCd = "abc", ErrorMsg = "abc", FixYn = "N" };
            NailApi Api = NailApi.GetDevelopmentInstance("");
            res = await Api.ErrorInfo.CreateErrorInfoAsync(req);
            Assert.IsNotNull(res);
        }

        public ISettingsService SettingsService { get; }

        [TestMethod]
        public async Task ValidateMariaConnectionAsync()
        {
            var result = await SettingsService.ValidateConnectionAsync("", DataProviderType.Maria);
            Assert.IsNotNull(result);
        }
    }
}
