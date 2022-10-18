﻿using System.Reflection;
using System.Reflection.Emit;

namespace MonoMod.Utils {
    internal interface IDMDGenerator {
        MethodInfo Generate(DynamicMethodDefinition dmd, object? context);
    }
    /// <summary>
    /// A DynamicMethodDefinition "generator", responsible for generating a runtime MethodInfo from a DMD MethodDefinition.
    /// </summary>
    /// <typeparam name="TSelf"></typeparam>
    public abstract class DMDGenerator<TSelf> : IDMDGenerator where TSelf : DMDGenerator<TSelf>, new() {

        private static TSelf? _Instance;

        protected abstract MethodInfo GenerateCore(DynamicMethodDefinition dmd, object? context);

        MethodInfo IDMDGenerator.Generate(DynamicMethodDefinition dmd, object? context) {
            return Postbuild(GenerateCore(dmd, context));
        }

        public static MethodInfo Generate(DynamicMethodDefinition dmd, object? context = null)
            => Postbuild((_Instance ??= new TSelf()).GenerateCore(dmd, context));

        internal static unsafe MethodInfo Postbuild(MethodInfo mi) {

            if (PlatformDetection.Runtime is RuntimeKind.Mono) {
                // Luckily we're guaranteed to be safe from DynamicMethod -> RuntimeMethodInfo conversions.
                if (mi is not DynamicMethod && mi.DeclaringType != null) {
                    // get_Assembly is virtual in some versions of Mono (notably older ones and the infamous Unity fork).
                    // ?. results in a call instead of callvirt to skip a redundant nullcheck, which breaks this on ^...
                    var module = mi.Module;
                    if (module is null)
                        return mi;
                    Assembly asm = module.Assembly; // Let's hope that this doesn't get optimized into a call.
                    var asmType = asm.GetType();
                    if (asmType is null)
                        return mi;

                    asm.SetMonoCorlibInternal(true);
                }
            }

            return mi;
        }

    }
}
