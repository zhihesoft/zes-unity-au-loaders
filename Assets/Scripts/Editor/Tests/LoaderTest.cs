using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Au.Loaders
{
    internal class LoaderTest
    {
        [Test]
        public async Task LoadText()
        {
            var loader = new LoaderForEditor(); // Loader.GetLoader();
            var goodpath = "http://www.baidu.com";
            var badpath = "http://www.baidu.com.ddd";

            var txt = await loader.LoadText(goodpath);
            Assert.IsFalse(string.IsNullOrEmpty(txt));
            txt = await loader.LoadText(badpath);
            UnityEngine.TestTools.LogAssert.Expect(UnityEngine.LogType.Error, new Regex("^*"));
            Assert.IsTrue(string.IsNullOrEmpty(txt));

        }
    }
}
