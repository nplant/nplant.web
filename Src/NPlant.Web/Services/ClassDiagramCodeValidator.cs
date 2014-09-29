using System;
using System.CodeDom.Compiler;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace NPlant.Web.Services
{
    public class ClassDiagramCodeValidator
    {
        public bool Validate(CompilerResults compilerResult, out string message)
        {
            message = null;

            var buffer = new StringBuilder();

            AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(compilerResult.PathToAssembly);

            foreach ( ModuleDefinition module in assembly.Modules)
            {
                foreach ( TypeDefinition type in module.Types)
                {
                    foreach ( MethodDefinition method in type.Methods )
                    {
                        if (method.Body != null)
                        {
                            foreach (Instruction instruction in method.Body.Instructions)
                            {
                                MethodReference methodReference = instruction.Operand as MethodReference;
                                
                                if (methodReference != null)
                                {
                                    string operand = instruction.Operand.ToString();

                                    if (!IsFriendly(operand))
                                    {
                                        buffer.AppendLine(operand);

                                        // A method operation is going to look like this:
                                        //
                                        // System.Void NPlant.ClassDiagram::.ctor()
                                        //
                                        // going with a quick and dirty solution:
                                        //  - split on ::
                                        //  - pull in the first part - i.e. System.Void NPlant.ClassDiagram
                                        //  - split on " "
                                        //  - confirm the second part is a call to NPlant and NPlant only

                                        var split = operand.Split(new[] {"::"}, StringSplitOptions.RemoveEmptyEntries);

                                        if (split.Length == 2)
                                        {
                                            string firstPart = split[0];

                                            var secondSplit = firstPart.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);

                                            if (secondSplit.Length == 2)
                                            {
                                                string secondPart = secondSplit[1];

                                                if (!secondPart.StartsWith("NPlant"))
                                                {
                                                    message = "Malicious code detected - what is this?  {0}".FormatWith(operand);
                                                    return false;
                                                }
                                            }
                                            else
                                            {
                                                message = "Failed to interpret the compiled code - Can't tell if this is friend or foe, so rejecting this until we can clearly tell the difference.";
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            message = "Failed to interpret the compiled code - Can't tell if this is friend or foe, so rejecting this until we can clearly tell the difference.";
                                            return false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        private bool IsFriendly(string operand)
        {
            switch (operand)
            {
                case "System.Void NPlant.ClassDiagram::.ctor()":
                case "System.Void System.Object::.ctor()":
                    return true;
                default:
                    return false;
            }
        }
    }
}