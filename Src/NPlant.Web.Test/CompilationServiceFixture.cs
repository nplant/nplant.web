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
            using (var guard = new DiagramRunGuard())
            {
                using (var scope = guard.CreateScope())
                {
                    scope.Compile("public class FooDiagram : NPlant.ClassDiagram {}");

                    Assert.That(scope.Successful, Is.True);

                    scope.Run();

                    Assert.That(scope.Successful, Is.True);
                }
            }
        }

        [Test]
        public void Bunk_Diagram_Does_Not_Compile()
        {
            using (var guard = new DiagramRunGuard())
            {
                using (var scope = guard.CreateScope())
                {
                    scope.Compile("public classsssss FooDiagram : NPlant.ClassDiagram {}");

                    Assert.That(scope.Successful, Is.False);
                }
            }
        }

        [Test]
        public void Code_Should_Always_Run_In_A_New_AppDomain_Which_Is_Unloaded()
        {
            int id = AppDomain.CurrentDomain.Id;

            int counter = 0;

            while (counter < 5)
            {
                using (var guard = new DiagramRunGuard())
                {
                    using (var scope = guard.CreateScope())
                    {
                        scope.Compile("public class FooDiagram : NPlant.ClassDiagram {}");

                        Assert.That(scope.Successful, Is.True);

                        scope.Run();

                        Assert.That(scope.AppDomainId, Is.Not.EqualTo(id));

                        counter++;
                    }

                    Assert.That(AppDomain.CurrentDomain.Id, Is.EqualTo(id));
                }
            }
        }
    }
}
