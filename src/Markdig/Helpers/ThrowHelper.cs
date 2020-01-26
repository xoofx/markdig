using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Markdig.Helpers
{
    /// <summary>
    /// Inspired by CoreLib, taken from https://github.com/MihaZupan/SharpCollections, cc @MihaZupan
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class ThrowHelper
    {
        public static void ThrowArgumentNullException(ExceptionArgument argument)
        {
            throw new ArgumentNullException(GetArgumentName(argument));
        }

        public static void ThrowArgumentException(ExceptionArgument argument, ExceptionReason reason)
        {
            throw new ArgumentException(GetArgumentName(argument), GetExceptionReason(reason));
        }

        public static void ThrowArgumentOutOfRangeException(ExceptionArgument argument, ExceptionReason reason)
        {
            throw new ArgumentOutOfRangeException(GetArgumentName(argument), GetExceptionReason(reason));
        }

        public static void ThrowIndexOutOfRangeException()
        {
            throw new IndexOutOfRangeException();
        }

        private static string GetArgumentName(ExceptionArgument argument)
        {
            switch (argument)
            {
                case ExceptionArgument.key:
                case ExceptionArgument.input:
                case ExceptionArgument.value:
                case ExceptionArgument.length:
                case ExceptionArgument.text:
                    return argument.ToString();

                case ExceptionArgument.offsetLength:
                    return "offset and length";

                default:
                    Debug.Assert(false, "The enum value is not defined, please check the ExceptionArgument Enum.");
                    return "";
            }
        }
        private static string GetExceptionReason(ExceptionReason reason)
        {
            switch (reason)
            {
                case ExceptionReason.String_Empty:
                    return "String must not be empty.";

                case ExceptionReason.SmallCapacity:
                    return "Capacity was less than the current size.";

                case ExceptionReason.InvalidOffsetLength:
                    return "Offset and length must refer to a position in the string.";

                case ExceptionReason.DuplicateKey:
                    return "The given key is already present in the dictionary.";

                default:
                    Debug.Assert(false, "The enum value is not defined, please check the ExceptionReason Enum.");
                    return "";
            }
        }
    }

    internal enum ExceptionArgument
    {
        key,
        input,
        value,
        length,
        offsetLength,
        text
    }

    internal enum ExceptionReason
    {
        String_Empty,
        SmallCapacity,
        InvalidOffsetLength,
        DuplicateKey,
    }
}
