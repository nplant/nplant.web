using System;
using NPlant.Web.Services;
using NUnit.Framework;

namespace NPlant.Web.Test
{
    [TestFixture]
    public class CompilationServiceFixture
    {
        [Test]
        public void Empty_Diagram_Compiles_And_Runs_Successfully()
        {
            using (var factory = new CompilationServiceFactory())
            {
                var service = factory.Create();
                service.Compile("public class FooDiagram : NPlant.ClassDiagram {}");

                Assert.That(service.Successful, Is.True);

                service.Run();

                Assert.That(service.Successful, Is.True);
            }
        }

        [Test]
        public void Bunk_Diagram_Does_Not_Compile()
        {
            using (var factory = new CompilationServiceFactory())
            {
                var service = factory.Create();
                service.Compile("public classsssss FooDiagram : NPlant.ClassDiagram {}");

                Assert.That(service.Successful, Is.False);
            }
        }

        [Test]
        public void Code_Should_Always_Run_In_A_New_AppDomain_Which_Is_Unloaded()
        {
            int id = AppDomain.CurrentDomain.Id;

            int counter = 0;

            while (counter < 5)
            {
                using (var factory = new CompilationServiceFactory())
                {
                    var service = factory.Create();
                    service.Compile("public class FooDiagram : NPlant.ClassDiagram {}");

                    Assert.That(service.Successful, Is.True);

                    service.Run();

                    Assert.That(service.AppDomainId, Is.Not.EqualTo(id));

                    counter++;
                }

                Assert.That(AppDomain.CurrentDomain.Id, Is.EqualTo(id));
            }
        }
    }
}
