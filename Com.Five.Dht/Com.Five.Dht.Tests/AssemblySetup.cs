namespace Com.Five.Dht.Tests
{
    using log4net.Config;
    using NUnit.Framework;

    [SetUpFixture]
    public class AssemblySetup
    {
        [OneTimeSetUp]
        public void Setup()
        {
            XmlConfigurator.Configure();
        }
    }
}
