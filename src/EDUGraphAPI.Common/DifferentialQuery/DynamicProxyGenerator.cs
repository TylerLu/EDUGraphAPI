/*   
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.  
 *   * See LICENSE in the project root for license information.  
 */
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace EDUGraphAPI.DifferentialQuery
{
    static class DynamicProxyGenerator
    {
        private const string DynamicAssemblyName = "DynamicAssembly";
        private const string DynamicModuleName = "DynamicAssemblyModule";
        private const string ProxyClassNameFormater = "{0}Proxy";
        private const string ModifiedPropertyNamesPropertyName = "ModifiedPropertyNames";
        private const string IsDeletedPropertyName = "IsDeleted";

        private const MethodAttributes GetSetMethodAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.CheckAccessOnOverride | MethodAttributes.Virtual | MethodAttributes.HideBySig;

        public static Type CreateDeltaEntityProxyType<T>()
        {
            var type = typeof(T);

            var assemblyName = new AssemblyName(DynamicAssemblyName);
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);

            var moduleBuilder = assemblyBuilder.DefineDynamicModule(DynamicModuleName);
            var proxyClassName = string.Format(ProxyClassNameFormater, type.Name);

            var typeBuilder = moduleBuilder.DefineType(proxyClassName, TypeAttributes.Public, type);
            typeBuilder.AddInterfaceImplementation(typeof(IDeltaEntity));

            var modifiedPropertyNamesType = typeof(HashSet<string>);
            var modifiedPropertyNamesField = typeBuilder.DefineField("_" + ModifiedPropertyNamesPropertyName, typeof(HashSet<string>), FieldAttributes.Public);

            DefineConstructor(typeBuilder, modifiedPropertyNamesType, modifiedPropertyNamesField);

            var modifiedPropertyNamesBuilder = DefineGetProperty(typeBuilder, ModifiedPropertyNamesPropertyName, modifiedPropertyNamesField);
            typeBuilder.DefineMethodOverride(
                modifiedPropertyNamesBuilder.GetGetMethod(),
                typeof(IDeltaEntity).GetMethod("get_" + ModifiedPropertyNamesPropertyName, BindingFlags.Public | BindingFlags.Instance));

            var isDeletedBuilder = DefineProperty(typeBuilder, IsDeletedPropertyName, typeof(bool));

            foreach (var propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!propertyInfo.GetGetMethod().IsVirtual || !propertyInfo.GetSetMethod().IsVirtual) continue;
                OverrideProperty(typeBuilder, modifiedPropertyNamesField, propertyInfo.Name, propertyInfo.PropertyType);
            }

            return typeBuilder.CreateType();
        }

        private static void DefineConstructor(TypeBuilder typeBuilder, Type modifiedPropertyNamesType, FieldBuilder fbModifiedPropertyNames)
        {
            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, null);
            var ilgCtor = constructorBuilder.GetILGenerator();
            ilgCtor.Emit(OpCodes.Ldarg_0);
            ilgCtor.Emit(OpCodes.Newobj, modifiedPropertyNamesType.GetConstructor(new Type[0]));
            ilgCtor.Emit(OpCodes.Stfld, fbModifiedPropertyNames);
            ilgCtor.Emit(OpCodes.Ret);
        }

        private static PropertyBuilder DefineGetProperty(TypeBuilder typeBuilder, string propertyName, FieldBuilder fieldBuider)
        {
            var propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.None, fieldBuider.FieldType, Type.EmptyTypes);
            var methodGet = typeBuilder.DefineMethod("get_" + propertyName, GetSetMethodAttributes, fieldBuider.FieldType, Type.EmptyTypes);
            var ilGetMethod = methodGet.GetILGenerator();
            ilGetMethod.Emit(OpCodes.Ldarg_0);
            ilGetMethod.Emit(OpCodes.Ldfld, fieldBuider);
            ilGetMethod.Emit(OpCodes.Ret);
            propertyBuilder.SetGetMethod(methodGet);
            return propertyBuilder;
        }

        private static PropertyBuilder DefineProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
        {
            var fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);
            var propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.SpecialName, propertyType, null);

            var methodGet = typeBuilder.DefineMethod("get_" + propertyName, GetSetMethodAttributes, propertyType, Type.EmptyTypes);
            var ilGetMethod = methodGet.GetILGenerator();
            ilGetMethod.Emit(OpCodes.Ldarg_0);
            ilGetMethod.Emit(OpCodes.Ldfld, fieldBuilder);
            ilGetMethod.Emit(OpCodes.Ret);
            propertyBuilder.SetGetMethod(methodGet);

            var methodSet = typeBuilder.DefineMethod("set_" + propertyName, GetSetMethodAttributes, null, new Type[] { propertyType });
            var ilSetMethod = methodSet.GetILGenerator();
            ilSetMethod.Emit(OpCodes.Ldarg_0);
            ilSetMethod.Emit(OpCodes.Ldarg_1);
            ilSetMethod.Emit(OpCodes.Stfld, fieldBuilder);
            ilSetMethod.Emit(OpCodes.Ret);
            propertyBuilder.SetSetMethod(methodSet);

            return propertyBuilder;
        }

        private static void OverrideProperty(TypeBuilder typeBuilder, FieldBuilder modifiedPropertyNamesField, string propertyName, Type propertyType)
        {
            var fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);
            var propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.SpecialName, propertyType, null);

            var methodGet = typeBuilder.DefineMethod("get_" + propertyName, GetSetMethodAttributes, propertyType, Type.EmptyTypes);
            var ilGetMethod = methodGet.GetILGenerator();
            ilGetMethod.Emit(OpCodes.Ldarg_0);
            ilGetMethod.Emit(OpCodes.Ldfld, fieldBuilder);
            ilGetMethod.Emit(OpCodes.Ret);
            propertyBuilder.SetGetMethod(methodGet);

            var methodSet = typeBuilder.DefineMethod("set_" + propertyName, GetSetMethodAttributes, null, new Type[] { propertyType });
            var ilSetMethod = methodSet.GetILGenerator();
            ilSetMethod.Emit(OpCodes.Ldarg_0);
            ilSetMethod.Emit(OpCodes.Ldarg_1);
            ilSetMethod.Emit(OpCodes.Stfld, fieldBuilder);
            ilSetMethod.Emit(OpCodes.Ldarg_0);
            ilSetMethod.Emit(OpCodes.Ldfld, modifiedPropertyNamesField);
            ilSetMethod.Emit(OpCodes.Ldstr, propertyName);
            ilSetMethod.Emit(OpCodes.Callvirt, modifiedPropertyNamesField.FieldType.GetMethod("Add", new Type[] { typeof(string) }));
            ilSetMethod.Emit(OpCodes.Pop);
            ilSetMethod.Emit(OpCodes.Ret);
            propertyBuilder.SetSetMethod(methodSet);
        }
    }
}