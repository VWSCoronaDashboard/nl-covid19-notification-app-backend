﻿// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

// The current raw/unmanaged signature on the GAEN protobuf 'export.bin' file is described in
// https://developers.google.com/android/exposure-notifications/exposure-key-file-format as:
//
//     ......
//          signature_algorithm: "1.2.840.10045.4.3.2"
//     ......
//      message TEKSignature {
//        .......
//        //Signature in X9.62 format (ASN.1 SEQUENCE of two INTEGER fields)
//        optional bytes signature = 4;
//     ......
//
// As it stands:
//
//       cert.GetECDsaPrivateKey().SignData(content, HashAlgorithmName.SHA256);
//
// returns a 64 byte array; which contains two 32 byte (256 bits) integers
// concatenated back to back in big-endian/network order.
//
// Until the new System.Formats.Asn1 package and AsnWriter (or similar) is
// introdued; 'manually' create an X9.62 package. This format is defined in
// X9.62-1998, "Public Key Cryptography For The Financial Services Industry:
// The Elliptic Curve Digital Signature Algorithm (ECDSA)", January 7, 1999".
//
// Within the contect of an ECDSA and the OID of the signature algorithm;
// http://oid-info.com/get/1.2.840.10045.4.3.2 ()ecdsa-with-SHA256 and
// and most easily found in  https://www.ietf.org/rfc/rfc3279.txt in section
// 2.2.2 - the format meant is:
//
//       Dss-Sig-Value  ::=  SEQUENCE  {
//              r       INTEGER,
//              s       INTEGER
//      }
//
// In ASN1 a sequence is encoded a (http://luca.ntop.org/Teaching/Appunti/asn1.html,
// as 0x10 (table 1); and it is constructed (bit 6 set, 3.2) followed by its length:
//
//   0x30 <length>
//        integer, integer
//
//  Each integer is encoded as 0x2 (table 1); again followed by the length
//  and the 32 bytes of the integer in two compliment form, most significant digit
//  first and in its shortest representation. However GAEN (apparently) expect a BER/DER
//  encoding (which is industry standard) which means that its needst a 0 prefix
//  (table 3, section 5.7) if the top bit of the integer is set.
//
//  Resulting X9.62 DER encoded Dss-Sig-Value is:
//
//     0x30		 0x10 (sequence) + (1<<6) (contructed)
//     <len>		 Length of the next elements 2 x (2 + 32 + 0/1)
//        0x02           0x02 integer + (0<<6) primitive
//          <len>        Length of the next elements (32 + 0/1)
//          0 ?          0 prefix is the the top bit is set
//          32 bytes     integer r
//          <len>        Length of the next elements (32 + 0/1)
//          0 ?          0 prefix is the the top bit is set
//          32 bytes     integer s
//
// With the minimal len 70 = (2+2x(2+32)) and the worst case
// when both integers have their top bit set; 72 bytes.
//
// This does not quite comply with the spec; the BER/DER rule requires
// encoding with the minimum number of octets (except for 0, which is encoded
// as 0, length 1). So we should strip off any leading zero bytes. We do not.

using System;

namespace NL.Rijksoverheid.ExposureNotification.BackEnd.Components.Services.Signing.Signers
{
    /// <summary>
    /// Packaging fix over MS .NET implementation
    /// Converted from C provided by DWvG.
    /// </summary>
    public class X962PackagingFix
    {
        public byte[] Format(byte[] value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            var buffer = new byte[72];

            var r = new byte[32];
            var s = new byte[32];
            Array.Copy(value, r, 32);
            Array.Copy(value, 32, s, 0, 32);

            // Start of SEQUENCE
            buffer[0] = 0x30;

            // Length of sequence - including 0 prefixes when top bit of r/s are set.
            //
            buffer[1] = (byte)(0x44 + (r[0] >= 0x80 ? 1 : 0) + (s[0] >= 0x80 ? 1 : 0));

            // Integer header for an intger without a top bit set and when set.
            //
            var ia = new byte[] { 0x02, 0x20 };
            var ib = new byte[] { 0x02, 0x21, 0x0 };

            var index = 2;
            if (r[0] >= 0x80)
            {
                Array.Copy(ib, 0, buffer, index, ib.Length);
                index += ib.Length;
            }
            else
            {
                Array.Copy(ia, 0, buffer, index, ia.Length);
                index += ia.Length;
            }
            Array.Copy(r, 0, buffer, index, r.Length);
            index += r.Length;

            if (s[0] >= 0x80)
            {
                Array.Copy(ib, 0, buffer, index, ib.Length);
                index += ib.Length;
            }
            else
            {
                Array.Copy(ia, 0, buffer, index, ia.Length);
                index += ia.Length;
            }
            Array.Copy(s, 0, buffer, index, s.Length);
            index += s.Length;

            // Result length varies between 70 and 72.
            //
            var result = new byte[index];
            Array.Copy(buffer, result, index);

            return result;
        }
    }
}
