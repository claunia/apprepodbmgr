//
//  Author:
//    Natalia Portillo claunia@claunia.com
//
//  Copyright (c) 2017, © Claunia.com
//
//  All rights reserved.
//
//  Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//
//     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in
//       the documentation and/or other materials provided with the distribution.
//     * Neither the name of the [ORGANIZATION] nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
//  A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
//  CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
//  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//  PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
//  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
//  LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//

using apprepodbmgr.Core;
using Schemas;

namespace apprepodbmgr.Eto
{
    internal class DBEntryForEto
    {
        readonly DbEntry _item;

        public DBEntryForEto(DbEntry item) => _item = item;

        public long id
        {
            get => _item.Id;
            set {}
        }

        public string developer
        {
            get => _item.Developer;
            set {}
        }

        public string product
        {
            get => _item.Product;
            set {}
        }

        public string version
        {
            get => _item.Version;
            set {}
        }

        public string languages
        {
            get => _item.Languages;
            set {}
        }

        public string architecture
        {
            get => _item.Architecture;
            set {}
        }

        public string targetos
        {
            get => _item.TargetOs;
            set {}
        }

        public string format
        {
            get => _item.Format;
            set {}
        }

        public string description
        {
            get => _item.Description;
            set {}
        }

        public bool oem
        {
            get => _item.Oem;
            set {}
        }

        public bool upgrade
        {
            get => _item.Upgrade;
            set {}
        }

        public bool update
        {
            get => _item.Update;
            set {}
        }

        public bool source
        {
            get => _item.Source;
            set {}
        }

        public bool files
        {
            get => _item.Files;
            set {}
        }

        public bool Installer
        {
            get => _item.Installer;
            set {}
        }

        public byte[] xml
        {
            get => _item.Xml;
            set {}
        }

        public byte[] json
        {
            get => _item.Json;
            set {}
        }

        public string mdid
        {
            get => _item.Mdid;
            set {}
        }

        public DbEntry original
        {
            get => _item;
            set {}
        }
    }

    internal class StringEntry
    {
        public string str { get; set; }
    }

    internal class BarcodeEntry
    {
        public string          code { get; set; }
        public BarcodeTypeType type { get; set; }
    }

    internal class DiscEntry
    {
        public string          path { get; set; }
        public OpticalDiscType disc { get; set; }
    }

    internal class DiskEntry
    {
        public string         path { get; set; }
        public BlockMediaType disk { get; set; }
    }

    internal class TargetOsEntry
    {
        public string name    { get; set; }
        public string version { get; set; }
    }
}