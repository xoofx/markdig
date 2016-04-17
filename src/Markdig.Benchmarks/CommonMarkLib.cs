// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Testamina.Markdig.Benchmarks
{
    public static class CommonMarkLib
    {
        public static string ToHtml(string text)
        {
            unsafe
            {
                var textAsArray = Encoding.UTF8.GetBytes(text);

                fixed (void* ptext = textAsArray)
                {
                    var ptr = (byte*)cmark_markdown_to_html(new IntPtr(ptext), text.Length);
                    int length = 0;
                    while (ptr[length] != 0)
                    {
                        length++;
                    }
                    var buffer = new byte[length];
                    Marshal.Copy(new IntPtr(ptr), buffer, 0, length);
                    var result = Encoding.UTF8.GetString(buffer);
                    Marshal.FreeHGlobal(new IntPtr(ptr));
                    return result;
                }
            }
        }

        // char *cmark_markdown_to_html(const char *text, size_t len, int options);
        [DllImport("cmark", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr cmark_markdown_to_html(IntPtr charBuffer, int len, int options = 0);
    }
}