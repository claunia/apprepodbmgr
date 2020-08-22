// /***************************************************************************
// The Disc Image Chef
// ----------------------------------------------------------------------------
//
// Filename       : Checksum.cs
// Author(s)      : Natalia Portillo <claunia@claunia.com>
//
// Component      : Core methods.
//
// --[ Description ] ----------------------------------------------------------
//
//     Methods to checksum data.
//
// --[ License ] --------------------------------------------------------------
//
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as
//     published by the Free Software Foundation, either version 3 of the
//     License, or (at your option) any later version.
//
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
//
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// ----------------------------------------------------------------------------
// Copyright © 2011-2016 Natalia Portillo
// ****************************************************************************/

using System.Collections.Generic;
using System.Threading;
using DiscImageChef.Checksums;
using Schemas;

namespace apprepodbmgr.Core
{
    internal class Checksum
    {
        readonly Adler32Context   adler32ctx;
        adlerPacket               adlerPkt;
        Thread                    adlerThread;
        readonly Crc16Context     crc16ctx;
        crc16Packet               crc16Pkt;
        Thread                    crc16Thread;
        readonly Crc32Context     crc32ctx;
        crc32Packet               crc32Pkt;
        Thread                    crc32Thread;
        readonly Crc64Context     crc64ctx;
        crc64Packet               crc64Pkt;
        Thread                    crc64Thread;
        readonly Md5Context       md5ctx;
        md5Packet                 md5Pkt;
        Thread                    md5Thread;
        readonly Ripemd160Context ripemd160ctx;
        ripemd160Packet           ripemd160Pkt;
        Thread                    ripemd160Thread;
        readonly Sha1Context      sha1ctx;
        sha1Packet                sha1Pkt;
        Thread                    sha1Thread;
        readonly Sha256Context    sha256ctx;
        sha256Packet              sha256Pkt;
        Thread                    sha256Thread;
        readonly Sha384Context    sha384ctx;
        sha384Packet              sha384Pkt;
        Thread                    sha384Thread;
        readonly Sha512Context    sha512ctx;
        sha512Packet              sha512Pkt;
        Thread                    sha512Thread;
        spamsumPacket             spamsumPkt;
        Thread                    spamsumThread;
        readonly SpamSumContext   ssctx;

        internal Checksum()
        {
            adler32ctx   = new Adler32Context();
            crc16ctx     = new Crc16Context();
            crc32ctx     = new Crc32Context();
            crc64ctx     = new Crc64Context();
            md5ctx       = new Md5Context();
            ripemd160ctx = new Ripemd160Context();
            sha1ctx      = new Sha1Context();
            sha256ctx    = new Sha256Context();
            sha384ctx    = new Sha384Context();
            sha512ctx    = new Sha512Context();
            ssctx        = new SpamSumContext();

            adlerThread     = new Thread(updateAdler);
            crc16Thread     = new Thread(updateCRC16);
            crc32Thread     = new Thread(updateCRC32);
            crc64Thread     = new Thread(updateCRC64);
            md5Thread       = new Thread(updateMD5);
            ripemd160Thread = new Thread(updateRIPEMD160);
            sha1Thread      = new Thread(updateSHA1);
            sha256Thread    = new Thread(updateSHA256);
            sha384Thread    = new Thread(updateSHA384);
            sha512Thread    = new Thread(updateSHA512);
            spamsumThread   = new Thread(updateSpamSum);

            adlerPkt     = new adlerPacket();
            crc16Pkt     = new crc16Packet();
            crc32Pkt     = new crc32Packet();
            crc64Pkt     = new crc64Packet();
            md5Pkt       = new md5Packet();
            ripemd160Pkt = new ripemd160Packet();
            sha1Pkt      = new sha1Packet();
            sha256Pkt    = new sha256Packet();
            sha384Pkt    = new sha384Packet();
            sha512Pkt    = new sha512Packet();
            spamsumPkt   = new spamsumPacket();

            adlerPkt.context     = adler32ctx;
            crc16Pkt.context     = crc16ctx;
            crc32Pkt.context     = crc32ctx;
            crc64Pkt.context     = crc64ctx;
            md5Pkt.context       = md5ctx;
            ripemd160Pkt.context = ripemd160ctx;
            sha1Pkt.context      = sha1ctx;
            sha256Pkt.context    = sha256ctx;
            sha384Pkt.context    = sha384ctx;
            sha512Pkt.context    = sha512ctx;
            spamsumPkt.context   = ssctx;
        }

        internal void Update(byte[] data)
        {
            adlerPkt.data = data;
            adlerThread.Start(adlerPkt);
            crc16Pkt.data = data;
            crc16Thread.Start(crc16Pkt);
            crc32Pkt.data = data;
            crc32Thread.Start(crc32Pkt);
            crc64Pkt.data = data;
            crc64Thread.Start(crc64Pkt);
            md5Pkt.data = data;
            md5Thread.Start(md5Pkt);
            ripemd160Pkt.data = data;
            ripemd160Thread.Start(ripemd160Pkt);
            sha1Pkt.data = data;
            sha1Thread.Start(sha1Pkt);
            sha256Pkt.data = data;
            sha256Thread.Start(sha256Pkt);
            sha384Pkt.data = data;
            sha384Thread.Start(sha384Pkt);
            sha512Pkt.data = data;
            sha512Thread.Start(sha512Pkt);
            spamsumPkt.data = data;
            spamsumThread.Start(spamsumPkt);

            while(adlerThread.IsAlive     ||
                  crc16Thread.IsAlive     ||
                  crc32Thread.IsAlive     ||
                  crc64Thread.IsAlive     ||
                  md5Thread.IsAlive       ||
                  ripemd160Thread.IsAlive ||
                  sha1Thread.IsAlive      ||
                  sha256Thread.IsAlive    ||
                  sha384Thread.IsAlive    ||
                  sha512Thread.IsAlive    ||
                  spamsumThread.IsAlive) {}

            adlerThread     = new Thread(updateAdler);
            crc16Thread     = new Thread(updateCRC16);
            crc32Thread     = new Thread(updateCRC32);
            crc64Thread     = new Thread(updateCRC64);
            md5Thread       = new Thread(updateMD5);
            ripemd160Thread = new Thread(updateRIPEMD160);
            sha1Thread      = new Thread(updateSHA1);
            sha256Thread    = new Thread(updateSHA256);
            sha384Thread    = new Thread(updateSHA384);
            sha512Thread    = new Thread(updateSHA512);
            spamsumThread   = new Thread(updateSpamSum);
        }

        internal List<ChecksumType> End()
        {
            List<ChecksumType> chks = new List<ChecksumType>();

            var chk = new ChecksumType
            {
                type  = ChecksumTypeType.adler32,
                Value = adler32ctx.End()
            };

            chks.Add(chk);

            chk = new ChecksumType
            {
                type  = ChecksumTypeType.crc16,
                Value = crc16ctx.End()
            };

            chks.Add(chk);

            chk = new ChecksumType
            {
                type  = ChecksumTypeType.crc32,
                Value = crc32ctx.End()
            };

            chks.Add(chk);

            chk = new ChecksumType
            {
                type  = ChecksumTypeType.crc64,
                Value = crc64ctx.End()
            };

            chks.Add(chk);

            chk = new ChecksumType
            {
                type  = ChecksumTypeType.md5,
                Value = md5ctx.End()
            };

            chks.Add(chk);

            chk = new ChecksumType
            {
                type  = ChecksumTypeType.ripemd160,
                Value = ripemd160ctx.End()
            };

            chks.Add(chk);

            chk = new ChecksumType
            {
                type  = ChecksumTypeType.sha1,
                Value = sha1ctx.End()
            };

            chks.Add(chk);

            chk = new ChecksumType
            {
                type  = ChecksumTypeType.sha256,
                Value = sha256ctx.End()
            };

            chks.Add(chk);

            chk = new ChecksumType
            {
                type  = ChecksumTypeType.sha384,
                Value = sha384ctx.End()
            };

            chks.Add(chk);

            chk = new ChecksumType
            {
                type  = ChecksumTypeType.sha512,
                Value = sha512ctx.End()
            };

            chks.Add(chk);

            chk = new ChecksumType
            {
                type  = ChecksumTypeType.spamsum,
                Value = ssctx.End()
            };

            chks.Add(chk);

            return chks;
        }

        internal static List<ChecksumType> GetChecksums(byte[] data)
        {
            var adler32ctxData   = new Adler32Context();
            var crc16ctxData     = new Crc16Context();
            var crc32ctxData     = new Crc32Context();
            var crc64ctxData     = new Crc64Context();
            var md5ctxData       = new Md5Context();
            var ripemd160ctxData = new Ripemd160Context();
            var sha1ctxData      = new Sha1Context();
            var sha256ctxData    = new Sha256Context();
            var sha384ctxData    = new Sha384Context();
            var sha512ctxData    = new Sha512Context();
            var ssctxData        = new SpamSumContext();

            var adlerThreadData     = new Thread(updateAdler);
            var crc16ThreadData     = new Thread(updateCRC16);
            var crc32ThreadData     = new Thread(updateCRC32);
            var crc64ThreadData     = new Thread(updateCRC64);
            var md5ThreadData       = new Thread(updateMD5);
            var ripemd160ThreadData = new Thread(updateRIPEMD160);
            var sha1ThreadData      = new Thread(updateSHA1);
            var sha256ThreadData    = new Thread(updateSHA256);
            var sha384ThreadData    = new Thread(updateSHA384);
            var sha512ThreadData    = new Thread(updateSHA512);
            var spamsumThreadData   = new Thread(updateSpamSum);

            var adlerPktData     = new adlerPacket();
            var crc16PktData     = new crc16Packet();
            var crc32PktData     = new crc32Packet();
            var crc64PktData     = new crc64Packet();
            var md5PktData       = new md5Packet();
            var ripemd160PktData = new ripemd160Packet();
            var sha1PktData      = new sha1Packet();
            var sha256PktData    = new sha256Packet();
            var sha384PktData    = new sha384Packet();
            var sha512PktData    = new sha512Packet();
            var spamsumPktData   = new spamsumPacket();

            adlerPktData.context     = adler32ctxData;
            crc16PktData.context     = crc16ctxData;
            crc32PktData.context     = crc32ctxData;
            crc64PktData.context     = crc64ctxData;
            md5PktData.context       = md5ctxData;
            ripemd160PktData.context = ripemd160ctxData;
            sha1PktData.context      = sha1ctxData;
            sha256PktData.context    = sha256ctxData;
            sha384PktData.context    = sha384ctxData;
            sha512PktData.context    = sha512ctxData;
            spamsumPktData.context   = ssctxData;

            adlerPktData.data = data;
            adlerThreadData.Start(adlerPktData);
            crc16PktData.data = data;
            crc16ThreadData.Start(crc16PktData);
            crc32PktData.data = data;
            crc32ThreadData.Start(crc32PktData);
            crc64PktData.data = data;
            crc64ThreadData.Start(crc64PktData);
            md5PktData.data = data;
            md5ThreadData.Start(md5PktData);
            ripemd160PktData.data = data;
            ripemd160ThreadData.Start(ripemd160PktData);
            sha1PktData.data = data;
            sha1ThreadData.Start(sha1PktData);
            sha256PktData.data = data;
            sha256ThreadData.Start(sha256PktData);
            sha384PktData.data = data;
            sha384ThreadData.Start(sha384PktData);
            sha512PktData.data = data;
            sha512ThreadData.Start(sha512PktData);
            spamsumPktData.data = data;
            spamsumThreadData.Start(spamsumPktData);

            while(adlerThreadData.IsAlive     ||
                  crc16ThreadData.IsAlive     ||
                  crc32ThreadData.IsAlive     ||
                  crc64ThreadData.IsAlive     ||
                  md5ThreadData.IsAlive       ||
                  ripemd160ThreadData.IsAlive ||
                  sha1ThreadData.IsAlive      ||
                  sha256ThreadData.IsAlive    ||
                  sha384ThreadData.IsAlive    ||
                  sha512ThreadData.IsAlive    ||
                  spamsumThreadData.IsAlive) {}

            List<ChecksumType> dataChecksums = new List<ChecksumType>();

            var chk = new ChecksumType
            {
                type  = ChecksumTypeType.adler32,
                Value = adler32ctxData.End()
            };

            dataChecksums.Add(chk);

            chk = new ChecksumType
            {
                type  = ChecksumTypeType.crc16,
                Value = crc16ctxData.End()
            };

            dataChecksums.Add(chk);

            chk = new ChecksumType
            {
                type  = ChecksumTypeType.crc32,
                Value = crc32ctxData.End()
            };

            dataChecksums.Add(chk);

            chk = new ChecksumType
            {
                type  = ChecksumTypeType.crc64,
                Value = crc64ctxData.End()
            };

            dataChecksums.Add(chk);

            chk = new ChecksumType
            {
                type  = ChecksumTypeType.md5,
                Value = md5ctxData.End()
            };

            dataChecksums.Add(chk);

            chk = new ChecksumType
            {
                type  = ChecksumTypeType.ripemd160,
                Value = ripemd160ctxData.End()
            };

            dataChecksums.Add(chk);

            chk = new ChecksumType
            {
                type  = ChecksumTypeType.sha1,
                Value = sha1ctxData.End()
            };

            dataChecksums.Add(chk);

            chk = new ChecksumType
            {
                type  = ChecksumTypeType.sha256,
                Value = sha256ctxData.End()
            };

            dataChecksums.Add(chk);

            chk = new ChecksumType
            {
                type  = ChecksumTypeType.sha384,
                Value = sha384ctxData.End()
            };

            dataChecksums.Add(chk);

            chk = new ChecksumType
            {
                type  = ChecksumTypeType.sha512,
                Value = sha512ctxData.End()
            };

            dataChecksums.Add(chk);

            chk = new ChecksumType
            {
                type  = ChecksumTypeType.spamsum,
                Value = ssctxData.End()
            };

            dataChecksums.Add(chk);

            return dataChecksums;
        }

        #region Threading helpers
        struct adlerPacket
        {
            public Adler32Context context;
            public byte[]         data;
        }

        struct crc16Packet
        {
            public Crc16Context context;
            public byte[]       data;
        }

        struct crc32Packet
        {
            public Crc32Context context;
            public byte[]       data;
        }

        struct crc64Packet
        {
            public Crc64Context context;
            public byte[]       data;
        }

        struct md5Packet
        {
            public Md5Context context;
            public byte[]     data;
        }

        struct ripemd160Packet
        {
            public Ripemd160Context context;
            public byte[]           data;
        }

        struct sha1Packet
        {
            public Sha1Context context;
            public byte[]      data;
        }

        struct sha256Packet
        {
            public Sha256Context context;
            public byte[]        data;
        }

        struct sha384Packet
        {
            public Sha384Context context;
            public byte[]        data;
        }

        struct sha512Packet
        {
            public Sha512Context context;
            public byte[]        data;
        }

        struct spamsumPacket
        {
            public SpamSumContext context;
            public byte[]         data;
        }

        static void updateAdler(object packet) => ((adlerPacket)packet).context.Update(((adlerPacket)packet).data);

        static void updateCRC16(object packet) => ((crc16Packet)packet).context.Update(((crc16Packet)packet).data);

        static void updateCRC32(object packet) => ((crc32Packet)packet).context.Update(((crc32Packet)packet).data);

        static void updateCRC64(object packet) => ((crc64Packet)packet).context.Update(((crc64Packet)packet).data);

        static void updateMD5(object packet) => ((md5Packet)packet).context.Update(((md5Packet)packet).data);

        static void updateRIPEMD160(object packet) =>
            ((ripemd160Packet)packet).context.Update(((ripemd160Packet)packet).data);

        static void updateSHA1(object packet) => ((sha1Packet)packet).context.Update(((sha1Packet)packet).data);

        static void updateSHA256(object packet) => ((sha256Packet)packet).context.Update(((sha256Packet)packet).data);

        static void updateSHA384(object packet) => ((sha384Packet)packet).context.Update(((sha384Packet)packet).data);

        static void updateSHA512(object packet) => ((sha512Packet)packet).context.Update(((sha512Packet)packet).data);

        static void updateSpamSum(object packet) =>
            ((spamsumPacket)packet).context.Update(((spamsumPacket)packet).data);
        #endregion Threading helpers
    }
}