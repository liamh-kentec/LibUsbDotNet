﻿// <copyright file="TypeKindExtensions.cs" company="Quamotion">
// Copyright (c) Quamotion. All rights reserved.
// </copyright>

using Core.Clang;
using System;

namespace LibUsbDotNet.Generator
{
    internal static class TypeKindExtensions
    {
        public static string ToClrType(this TypeInfo type)
        {
            var canonical = type.GetCanonicalType();

            switch (canonical.Kind)
            {
                case TypeKind.Bool:
                    return "bool";

                case TypeKind.UChar:
                case TypeKind.Char_U:
                    return "char";

                case TypeKind.SChar:
                case TypeKind.Char_S:
                    return "sbyte";

                case TypeKind.UShort:
                    return "ushort";

                case TypeKind.Short:
                    return "short";

                case TypeKind.Float:
                    return "float";

                case TypeKind.Double:
                    return "double";

                case TypeKind.Int:
                    return "int";

                case TypeKind.Enum:
                    var enumName = type.GetTypeDeclaration().GetDisplayName();
                    return NameConversions.ToClrName(enumName, NameConversion.Type);

                case TypeKind.UInt:
                    return "uint";

                case TypeKind.IncompleteArray:
                    return "IntPtr";

                case TypeKind.Long:
                    return "int";

                case TypeKind.ULong:
                    return "uint";

                case TypeKind.LongLong:
                    return "long";

                case TypeKind.ULongLong:
                    return "ulong";

                case TypeKind.Void:
                    return "void";

                case TypeKind.Pointer:
                    // int LIBUSB_CALL libusb_init(libusb_context **ctx);
                    // would map to Init(ref IntPtr),
                    // whereas 
                    // void LIBUSB_CALL libusb_exit(libusb_context * ctx);
                    // would map to Exit(IntPtr);
                    var pointee = type.GetPointeeType();
                    switch (pointee.Kind)
                    {
                        case TypeKind.Pointer:
                            return "ref IntPtr";

                        case TypeKind.Int:
                            return "ref int";

                        case TypeKind.Typedef:
                            var typeDefName = pointee.GetTypedefName();

                            if (pointee.GetCanonicalType().Kind == TypeKind.Int)
                            {
                                return "ref int";
                            }
                            else if (typeDefName == "uint8_t")
                            {
                                return "ref byte";
                            }
                            else
                            {
                                return $"Native{NameConversions.ToClrName(pointee.GetTypedefName(), NameConversion.Type)}";
                            }

                        case TypeKind.Void:
                            return "IntPtr";

                        case TypeKind.Elaborated:
                            var spelling = pointee.GetNamedType().GetSpelling();

                            if (spelling == "timeval")
                            {
                                return "ref UnixNativeTimeval";
                            }
                            else
                            {
                                return "IntPtr";
                            }

                        default:
                            return "IntPtr";
                    }

                default:
                    throw new NotSupportedException();
            }
        }
    }
}
