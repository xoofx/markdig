// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Markdig.Extensions.MediaLinks
{
    /// <summary>
    /// Options for the <see cref="MediaLinkExtension"/>.
    /// </summary>
    public class MediaOptions
    {
        public MediaOptions()
        {
            Width = "500";
            Height = "281";
            ExtensionToMimeType = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {".3gp", "video/3gpp"},
                {".3g2", "video/3gpp2"},
                {".avi", "video/x-msvideo"},
                {".uvh", "video/vnd.dece.hd"},
                {".uvm", "video/vnd.dece.mobile"},
                {".uvu", "video/vnd.uvvu.mp4"},
                {".uvp", "video/vnd.dece.pd"},
                {".uvs", "video/vnd.dece.sd"},
                {".uvv", "video/vnd.dece.video"},
                {".fvt", "video/vnd.fvt"},
                {".f4v", "video/x-f4v"},
                {".flv", "video/x-flv"},
                {".fli", "video/x-fli"},
                {".h261", "video/h261"},
                {".h263", "video/h263"},
                {".h264", "video/h264"},
                {".jpm", "video/jpm"},
                {".jpgv", "video/jpeg"},
                {".m4v", "video/x-m4v"},
                {".asf", "video/x-ms-asf"},
                {".pyv", "video/vnd.ms-playready.media.pyv"},
                {".wm", "video/x-ms-wm"},
                {".wmx", "video/x-ms-wmx"},
                {".wmv", "video/x-ms-wmv"},
                {".wvx", "video/x-ms-wvx"},
                {".mj2", "video/mj2"},
                {".mxu", "video/vnd.mpegurl"},
                {".mpeg", "video/mpeg"},
                {".mp4", "video/mp4"},
                {".ogv", "video/ogg"},
                {".webm", "video/webm"},
                {".qt", "video/quicktime"},
                {".movie", "video/x-sgi-movie"},
                {".viv", "video/vnd.vivo"},

                {".adp", "audio/adpcm"},
                {".aac", "audio/x-aac"},
                {".aif", "audio/x-aiff"},
                {".uva", "audio/vnd.dece.audio"},
                {".eol", "audio/vnd.digital-winds"},
                {".dra", "audio/vnd.dra"},
                {".dts", "audio/vnd.dts"},
                {".dtshd", "audio/vnd.dts.hd"},
                {".rip", "audio/vnd.rip"},
                {".lvp", "audio/vnd.lucent.voice"},
                {".m3u", "audio/x-mpegurl"},
                {".pya", "audio/vnd.ms-playready.media.pya"},
                {".wma", "audio/x-ms-wma"},
                {".wax", "audio/x-ms-wax"},
                {".mid", "audio/midi"},
                {".mpga", "audio/mpeg"},
                {".mp4a", "audio/mp4"},
                {".ecelp4800", "audio/vnd.nuera.ecelp4800"},
                {".ecelp7470", "audio/vnd.nuera.ecelp7470"},
                {".ecelp9600", "audio/vnd.nuera.ecelp9600"},
                {".oga", "audio/ogg"},
                {".weba", "audio/webm"},
                {".ram", "audio/x-pn-realaudio"},
                {".rmp", "audio/x-pn-realaudio-plugin"},
                {".au", "audio/basic"},
                {".wav", "audio/x-wav"},
            };
        }

        public string Width { get; set; }

        public string Height { get; set; }

        public Dictionary<string, string> ExtensionToMimeType { get; }
    }
}