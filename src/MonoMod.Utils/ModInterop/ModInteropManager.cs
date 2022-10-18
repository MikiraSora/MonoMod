﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace MonoMod.ModInterop {
    public static class ModInteropManager {

        private static HashSet<Type> Registered = new HashSet<Type>();

        private static Dictionary<string, List<MethodInfo>> Methods = new Dictionary<string, List<MethodInfo>>();
        private static List<FieldInfo> Fields = new List<FieldInfo>();

        public static void ModInterop(this Type type) {
            if (Registered.Contains(type))
                return;
            Registered.Add(type);

            var prefix = type.Assembly.GetName().Name;
            foreach (ModExportNameAttribute attrib in type.GetCustomAttributes(typeof(ModExportNameAttribute), false)) {
                prefix = attrib.Name;
            }

            // Collect fields and methods in the type.
            foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Static)) {
                if (!typeof(Delegate).IsAssignableFrom(field.FieldType))
                    continue;
                Fields.Add(field);
            }
            foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Static)) {
                method.RegisterModExport();
                method.RegisterModExport(prefix);
            }

            // Refresh all existing fields and methods.
            foreach (FieldInfo field in Fields) {
                if (!Methods.TryGetValue(field.GetModImportName(), out var methods)) {
                    field.SetValue(null, null);
                    continue;
                }
                // Set the field to the first matching method, or null.
                bool matched = false;
                foreach (MethodInfo method in methods) {
                    try {
                        field.SetValue(null, Delegate.CreateDelegate(field.FieldType, null, method));
                        matched = true;
                        break;
                    } catch {
                        // Silently try the next method with the same name.
                    }
                }
                if (!matched)
                    field.SetValue(null, null);
            }
        }

        public static void RegisterModExport(this MethodInfo method, string? prefix = null) {
            if (!method.IsPublic || !method.IsStatic)
                throw new MemberAccessException("Utility must be public static");
            string name = method.Name;
            if (!string.IsNullOrEmpty(prefix))
                name = prefix + "." + name;

            if (!Methods.TryGetValue(name, out var methods))
                Methods[name] = methods = new List<MethodInfo>();

            if (!methods.Contains(method))
                methods.Add(method);
        }

        private static string GetModImportName(this FieldInfo field) {
            foreach (ModImportNameAttribute attrib in field.GetCustomAttributes(typeof(ModImportNameAttribute), false)) {
                return attrib.Name;
            }

            if (field.DeclaringType is not null) {
                foreach (ModImportNameAttribute attrib in field.DeclaringType.GetCustomAttributes(typeof(ModImportNameAttribute), false)) {
                    return attrib.Name + "." + field.Name;
                }
            }

            return field.Name;
        }

    }
}
