using System;
using System.IO;
using System.Text;
using NPlant.Web.Services;
using NUnit.Framework;

namespace NPlant.Web.Test
{
    [TestFixture]
    public class CompilationServiceSecurityFixture
    {
        [Test]
        public void Complex_Diagram_Playing_Nice_Should_Succeed()
        {
            using (var guard = new DiagramRunGuard())
            {
                using (var scope = guard.CreateScope())
                {
                    string code = CreateComplexDiagram();
                    scope.Compile(code);

                    Assert.That(scope.Successful, Is.True);

                    scope.Run();

                    Assert.That(scope.Successful, Is.True);
                }
            }
        }
        
        [Test]
        public void Complex_Diagram_Not_Playing_Nice_Should_Fail()
        {
            using (var guard = new DiagramRunGuard())
            {
                using (var scope = guard.CreateScope())
                {
                    string code = CreateComplexDiagram("System.IO.File.WriteAllText(\"C:\\\\Temp\\\\Bad.stuff\", \"smelly!!!\");");
                    scope.Compile(code);

                    Assert.That(scope.Successful, Is.False);
                }
            }
        }

        private static string CreateComplexDiagram(string extra = null)
        {
            var buffer = new StringBuilder();
            buffer.AppendLine("using System;");
            buffer.AppendLine("");
            buffer.AppendLine("namespace NPlant.Samples.FullMonty");
            buffer.AppendLine("{");
            buffer.AppendLine("    public class FullMontyClassDiagram : ClassDiagram");
            buffer.AppendLine("    {");
            buffer.AppendLine("        public FullMontyClassDiagram()");
            buffer.AppendLine("        {");
            buffer.AppendLine("            this.GenerationOptions.ShowMethods();");
            buffer.AppendLine("{0}".FormatWith(extra));
            buffer.AppendLine("            AddClass<Foo>();");
            buffer.AppendLine("        }");
            buffer.AppendLine("    }");
            buffer.AppendLine("");
            buffer.AppendLine("    public class Foo");
            buffer.AppendLine("    {");
            buffer.AppendLine("        public string SomeString;");
            buffer.AppendLine("        public Bar TheBar;");
            buffer.AppendLine("        public Baz<Arg1, Arg2> TheBaz;");
            buffer.AppendLine("        public Baz2<Arg1, Arg2> TheBaz2;");
            buffer.AppendLine("");
            buffer.AppendLine("        public void DoSomethingOnFoo() { }");
            buffer.AppendLine("        public void DoSomethingOnFoo(string parm1) { }");
            buffer.AppendLine("        public void DoSomethingOnFoo(string parm1, DateTime? parm2, Bar parm3) { }");
            buffer.AppendLine("    }");
            buffer.AppendLine("");
            buffer.AppendLine("    public class Bar");
            buffer.AppendLine("    {");
            buffer.AppendLine("        public DateTime? SomeDate;");
            buffer.AppendLine("");
            buffer.AppendLine("        public void DoSomethingOnBar() { }");
            buffer.AppendLine("        public void DoSomethingOnBar(string parm1) { }");
            buffer.AppendLine("        public void DoSomethingOnBar(string parm1, DateTime? parm2, Baz<Arg1, Arg2> parm3) { }");
            buffer.AppendLine("    }");
            buffer.AppendLine("");
            buffer.AppendLine("    public class Baz<T1, T2>");
            buffer.AppendLine("    {");
            buffer.AppendLine("        public Foo TheFoo;");
            buffer.AppendLine("");
            buffer.AppendLine("        public T1 Arg1;");
            buffer.AppendLine("");
            buffer.AppendLine("        public T2 Arg2;");
            buffer.AppendLine("    }");
            buffer.AppendLine("");
            buffer.AppendLine("    public class Baz2<T1, T2>");
            buffer.AppendLine("    {");
            buffer.AppendLine("        public Baz2()");
            buffer.AppendLine("        {");
            buffer.AppendLine("        }");
            buffer.AppendLine("");
            buffer.AppendLine("        public string Whatever;");
            buffer.AppendLine("    }");
            buffer.AppendLine("");
            buffer.AppendLine("    public class Arg1 { }");
            buffer.AppendLine("    public class Arg2{}");
            buffer.AppendLine("}");

            return buffer.ToString();
        }
    }
}
