﻿/*
    Copyright 2017 Katy Coe - http://www.hearthcode.org - http://www.djkaty.com

    All rights reserved.
*/

using System;
using System.Linq;
using System.Reflection;

namespace Il2CppInspector.Reflection
{
    public class MethodInfo : MethodBase
    {
        // IL2CPP-specific data
        public Il2CppMethodDefinition Definition { get; }
        public int Index { get; }
        public uint VirtualAddress { get; }
        public bool HasBody { get; }

        public override MemberTypes MemberType => MemberTypes.Method;

        // Info about the return parameter
        public ParameterInfo ReturnParameter { get; }

        // Return type of the method
        private readonly Il2CppType returnType;
        public TypeInfo ReturnType => Assembly.Model.GetType(returnType, MemberTypes.TypeInfo);

        // TODO: ReturnTypeCustomAttributes

        public MethodInfo(Il2CppInspector pkg, int methodIndex, TypeInfo declaringType) :
            base(declaringType) {
            Definition = pkg.Metadata.Methods[methodIndex];
            Index = methodIndex;
            if (Definition.methodIndex >= 0) {
                VirtualAddress = pkg.Binary.MethodPointers[Definition.methodIndex];
                HasBody = true;
            }
            Name = pkg.Strings[Definition.nameIndex];

            if ((Definition.flags & Il2CppConstants.METHOD_ATTRIBUTE_MEMBER_ACCESS_MASK) == Il2CppConstants.METHOD_ATTRIBUTE_PRIVATE)
                Attributes |= MethodAttributes.Private;
            if ((Definition.flags & Il2CppConstants.METHOD_ATTRIBUTE_MEMBER_ACCESS_MASK) == Il2CppConstants.METHOD_ATTRIBUTE_PUBLIC)
                Attributes |= MethodAttributes.Public;
            if ((Definition.flags & Il2CppConstants.METHOD_ATTRIBUTE_MEMBER_ACCESS_MASK) == Il2CppConstants.METHOD_ATTRIBUTE_FAM_AND_ASSEM)
                Attributes |= MethodAttributes.FamANDAssem;
            if ((Definition.flags & Il2CppConstants.METHOD_ATTRIBUTE_MEMBER_ACCESS_MASK) == Il2CppConstants.METHOD_ATTRIBUTE_ASSEM)
                Attributes |= MethodAttributes.Assembly;
            if ((Definition.flags & Il2CppConstants.METHOD_ATTRIBUTE_MEMBER_ACCESS_MASK) == Il2CppConstants.METHOD_ATTRIBUTE_FAMILY)
                Attributes |= MethodAttributes.Family;
            if ((Definition.flags & Il2CppConstants.METHOD_ATTRIBUTE_MEMBER_ACCESS_MASK) == Il2CppConstants.METHOD_ATTRIBUTE_FAM_OR_ASSEM)
                Attributes |= MethodAttributes.FamORAssem;
            if ((Definition.flags & Il2CppConstants.METHOD_ATTRIBUTE_VIRTUAL) != 0)
                Attributes |= MethodAttributes.Virtual;
            if ((Definition.flags & Il2CppConstants.METHOD_ATTRIBUTE_ABSTRACT) != 0)
                Attributes |= MethodAttributes.Abstract;
            if ((Definition.flags & Il2CppConstants.METHOD_ATTRIBUTE_STATIC) != 0)
                Attributes |= MethodAttributes.Static;
            if ((Definition.flags & Il2CppConstants.METHOD_ATTRIBUTE_FINAL) != 0)
                Attributes |= MethodAttributes.Final;
            if ((Definition.flags & Il2CppConstants.METHOD_ATTRIBUTE_HIDE_BY_SIG) != 0)
                Attributes |= MethodAttributes.HideBySig;
            if ((Definition.flags & Il2CppConstants.METHOD_ATTRIBUTE_VTABLE_LAYOUT_MASK) == Il2CppConstants.METHOD_ATTRIBUTE_NEW_SLOT)
                Attributes |= MethodAttributes.NewSlot;
            if ((Definition.flags & Il2CppConstants.METHOD_ATTRIBUTE_PINVOKE_IMPL) != 0)
                Attributes |= MethodAttributes.PinvokeImpl;
            if ((Definition.flags & Il2CppConstants.METHOD_ATTRIBUTE_SPECIAL_NAME) != 0)
                Attributes |= MethodAttributes.SpecialName;
            if ((Definition.flags & Il2CppConstants.METHOD_ATTRIBUTE_UNMANAGED_EXPORT) != 0)
                Attributes |= MethodAttributes.UnmanagedExport;

            // Add return parameter
            returnType = pkg.TypeUsages[Definition.returnType];
            ReturnParameter = new ParameterInfo(pkg, -1, this);

            // Add arguments
            for (var p = Definition.parameterStart; p < Definition.parameterStart + Definition.parameterCount; p++)
                DeclaredParameters.Add(new ParameterInfo(pkg, p, this));
        }

        public override string ToString() => ReturnType.Name + " " + Name + "(" + string.Join(", ", DeclaredParameters.Select(x => x.ParameterType.Name)) + ")";
    }
}