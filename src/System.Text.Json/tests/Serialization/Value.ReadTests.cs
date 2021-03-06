﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public static partial class ValueTests
    {
        [Fact]
        public static void ReadPrimitives()
        {
            int i = JsonSerializer.Deserialize<int>(Encoding.UTF8.GetBytes(@"1"));
            Assert.Equal(1, i);

            int i2 = JsonSerializer.Deserialize<int>("2");
            Assert.Equal(2, i2);

            int? i3 = JsonSerializer.Deserialize<int?>("null");
            Assert.Null(i3);

            long l = JsonSerializer.Deserialize<long>(Encoding.UTF8.GetBytes(long.MaxValue.ToString()));
            Assert.Equal(long.MaxValue, l);

            long l2 = JsonSerializer.Deserialize<long>(long.MaxValue.ToString());
            Assert.Equal(long.MaxValue, l2);

            string s = JsonSerializer.Deserialize<string>(Encoding.UTF8.GetBytes(@"""Hello"""));
            Assert.Equal("Hello", s);

            string s2 = JsonSerializer.Deserialize<string>(@"""Hello""");
            Assert.Equal("Hello", s2);

            Uri u = JsonSerializer.Deserialize<Uri>(@"""""");
            Assert.Equal("", u.OriginalString);
        }

        [Fact]
        public static void ReadPrimitivesWithWhitespace()
        {
            int i = JsonSerializer.Deserialize<int>(Encoding.UTF8.GetBytes(@" 1 "));
            Assert.Equal(1, i);

            int i2 = JsonSerializer.Deserialize<int>("2\t");
            Assert.Equal(2, i2);

            int? i3 = JsonSerializer.Deserialize<int?>("\r\nnull");
            Assert.Null(i3);

            long l = JsonSerializer.Deserialize<long>(Encoding.UTF8.GetBytes("\t" + long.MaxValue.ToString()));
            Assert.Equal(long.MaxValue, l);

            long l2 = JsonSerializer.Deserialize<long>(long.MaxValue.ToString() + " \r\n");
            Assert.Equal(long.MaxValue, l2);

            string s = JsonSerializer.Deserialize<string>(Encoding.UTF8.GetBytes(@"""Hello"" "));
            Assert.Equal("Hello", s);

            string s2 = JsonSerializer.Deserialize<string>(@"  ""Hello"" ");
            Assert.Equal("Hello", s2);

            bool b = JsonSerializer.Deserialize<bool>(" \ttrue ");
            Assert.Equal(true, b);

            bool b2 = JsonSerializer.Deserialize<bool>(" false\n");
            Assert.Equal(false, b2);
        }

        [Fact]
        public static void ReadPrimitivesFail()
        {
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<int>(Encoding.UTF8.GetBytes(@"a")));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<int[]>(Encoding.UTF8.GetBytes(@"[1,a]")));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<int>(@"null"));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<int>(@""""""));
        }

        [Theory]
        [InlineData(typeof(bool))]
        [InlineData(typeof(byte))]
        [InlineData(typeof(char))]
        [InlineData(typeof(DateTime))]
        [InlineData(typeof(DateTimeOffset))]
        [InlineData(typeof(decimal))]
        [InlineData(typeof(double))]
        [InlineData(typeof(JsonTokenType))]
        [InlineData(typeof(Guid))]
        [InlineData(typeof(short))]
        [InlineData(typeof(int))]
        [InlineData(typeof(long))]
        [InlineData(typeof(sbyte))]
        [InlineData(typeof(float))]
        [InlineData(typeof(string))]
        [InlineData(typeof(ushort))]
        [InlineData(typeof(uint))]
        [InlineData(typeof(ulong))]
        [InlineData(typeof(Uri))]
        public static void PrimitivesShouldFailWithArrayOrObjectAssignment(Type primitiveType)
        {
            // This test lines up with the built in JsonConverters
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize(@"[]", primitiveType));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize(@"{}", primitiveType));
        }

        [Fact]
        public static void EmptyStringInput()
        {
            string obj = JsonSerializer.Deserialize<string>(@"""""");
            Assert.Equal(string.Empty, obj);
        }

        [Fact]
        public static void ReadPrimitiveExtraBytesFail()
        {
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<int[]>("[2] {3}"));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<int[]>(Encoding.UTF8.GetBytes(@"[2] {3}")));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<string>(@"""Hello"" 42"));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<string>(Encoding.UTF8.GetBytes(@"""Hello"" 42")));
        }

        [Fact]
        public static void RangeFail()
        {
            // These have custom code because the reader doesn't natively support:
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<byte>((byte.MinValue - 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<byte>((byte.MaxValue + 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<byte?>((byte.MinValue - 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<byte?>((byte.MaxValue + 1).ToString()));

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<sbyte>((sbyte.MinValue - 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<sbyte>((sbyte.MaxValue + 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<sbyte?>((sbyte.MinValue - 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<sbyte?>((sbyte.MaxValue + 1).ToString()));

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<short>((short.MinValue - 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<short>((short.MaxValue + 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<short?>((short.MinValue - 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<short?>((short.MaxValue + 1).ToString()));

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ushort>((ushort.MinValue - 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ushort>((ushort.MaxValue + 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ushort?>((ushort.MinValue - 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ushort?>((ushort.MaxValue + 1).ToString()));

            // These are natively supported by the reader:
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<int>(((long)int.MinValue - 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<int>(((long)int.MaxValue + 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<int?>(((long)int.MinValue - 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<int?>(((long)int.MaxValue + 1).ToString()));

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<uint>(((long)uint.MinValue - 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<uint>(((long)uint.MaxValue + 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<uint?>(((long)uint.MinValue - 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<uint?>(((long)uint.MaxValue + 1).ToString()));

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<long>(long.MinValue.ToString() + "0"));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<long>(long.MaxValue.ToString() + "0"));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<long?>(long.MinValue.ToString() + "0"));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<long?>(long.MaxValue.ToString() + "0"));

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ulong>(ulong.MinValue.ToString() + "0"));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ulong>(ulong.MaxValue.ToString() + "0"));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ulong?>(ulong.MinValue.ToString() + "0"));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ulong?>(ulong.MaxValue.ToString() + "0"));

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<decimal>(decimal.MinValue.ToString() + "0"));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<decimal>(decimal.MaxValue.ToString() + "0"));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<decimal?>(decimal.MinValue.ToString() + "0"));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<decimal?>(decimal.MaxValue.ToString() + "0"));
        }

        [Fact]
        public static void RangePass()
        {
            Assert.Equal(byte.MaxValue, JsonSerializer.Deserialize<byte>(byte.MaxValue.ToString()));
            Assert.Equal(byte.MaxValue, JsonSerializer.Deserialize<byte?>(byte.MaxValue.ToString()));

            Assert.Equal(sbyte.MaxValue, JsonSerializer.Deserialize<sbyte>(sbyte.MaxValue.ToString()));
            Assert.Equal(sbyte.MaxValue, JsonSerializer.Deserialize<sbyte?>(sbyte.MaxValue.ToString()));

            Assert.Equal(short.MaxValue, JsonSerializer.Deserialize<short>(short.MaxValue.ToString()));
            Assert.Equal(short.MaxValue, JsonSerializer.Deserialize<short?>(short.MaxValue.ToString()));

            Assert.Equal(ushort.MaxValue, JsonSerializer.Deserialize<ushort>(ushort.MaxValue.ToString()));
            Assert.Equal(ushort.MaxValue, JsonSerializer.Deserialize<ushort?>(ushort.MaxValue.ToString()));

            Assert.Equal(int.MaxValue, JsonSerializer.Deserialize<int>(int.MaxValue.ToString()));
            Assert.Equal(int.MaxValue, JsonSerializer.Deserialize<int?>(int.MaxValue.ToString()));

            Assert.Equal(uint.MaxValue, JsonSerializer.Deserialize<uint>(uint.MaxValue.ToString()));
            Assert.Equal(uint.MaxValue, JsonSerializer.Deserialize<uint?>(uint.MaxValue.ToString()));

            Assert.Equal(long.MaxValue, JsonSerializer.Deserialize<long>(long.MaxValue.ToString()));
            Assert.Equal(long.MaxValue, JsonSerializer.Deserialize<long?>(long.MaxValue.ToString()));

            Assert.Equal(ulong.MaxValue, JsonSerializer.Deserialize<ulong>(ulong.MaxValue.ToString()));
            Assert.Equal(ulong.MaxValue, JsonSerializer.Deserialize<ulong?>(ulong.MaxValue.ToString()));

            Assert.Equal(decimal.MaxValue, JsonSerializer.Deserialize<decimal>(decimal.MaxValue.ToString(CultureInfo.InvariantCulture)));
            Assert.Equal(decimal.MaxValue, JsonSerializer.Deserialize<decimal?>(decimal.MaxValue.ToString(CultureInfo.InvariantCulture)));
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Skipped since NETFX has different semantics and bugs with floating point.")]
        public static void RangePassFloatingPoint()
        {
            // Verify overflow\underflow.
            // On NETFX these throw.
            Assert.True(float.IsNegativeInfinity(JsonSerializer.Deserialize<float>(double.MinValue.ToString(CultureInfo.InvariantCulture))));
            Assert.True(float.IsPositiveInfinity(JsonSerializer.Deserialize<float>(double.MaxValue.ToString(CultureInfo.InvariantCulture))));
            Assert.True(float.IsNegativeInfinity(JsonSerializer.Deserialize<float?>(double.MinValue.ToString(CultureInfo.InvariantCulture)).Value));
            Assert.True(float.IsPositiveInfinity(JsonSerializer.Deserialize<float?>(double.MaxValue.ToString(CultureInfo.InvariantCulture)).Value));

            Assert.True(double.IsNegativeInfinity(JsonSerializer.Deserialize<double>(double.MinValue.ToString(CultureInfo.InvariantCulture) + "0")));
            Assert.True(double.IsPositiveInfinity(JsonSerializer.Deserialize<double>(double.MaxValue.ToString(CultureInfo.InvariantCulture) + "0")));
            Assert.True(double.IsNegativeInfinity(JsonSerializer.Deserialize<double?>(double.MinValue.ToString(CultureInfo.InvariantCulture) + "0").Value));
            Assert.True(double.IsPositiveInfinity(JsonSerializer.Deserialize<double?>(double.MaxValue.ToString(CultureInfo.InvariantCulture) + "0").Value));

            // Verify sign is correct.
            // On NETFX a value of -0 does not keep the sign.
            Assert.Equal(0x0000000000000000ul, (ulong)BitConverter.DoubleToInt64Bits(JsonSerializer.Deserialize<double>("0")));
            Assert.Equal(0x8000000000000000ul, (ulong)BitConverter.DoubleToInt64Bits(JsonSerializer.Deserialize<double>("-0")));
            Assert.Equal(0x8000000000000000ul, (ulong)BitConverter.DoubleToInt64Bits(JsonSerializer.Deserialize<double>("-0.0")));

#if BUILDING_INBOX_LIBRARY
            // Verify sign is correct; SingleToInt32Bits not available on netfx.
            Assert.Equal(0x00000000u, (uint)BitConverter.SingleToInt32Bits(JsonSerializer.Deserialize<float>("0")));
            Assert.Equal(0x80000000u, (uint)BitConverter.SingleToInt32Bits(JsonSerializer.Deserialize<float>("-0")));
            Assert.Equal(0x80000000u, (uint)BitConverter.SingleToInt32Bits(JsonSerializer.Deserialize<float>("-0.0")));
#endif

            // Verify Round-tripping.
            // On NETFX round tripping is not supported.
            Assert.Equal(float.MaxValue, JsonSerializer.Deserialize<float>(float.MaxValue.ToString(CultureInfo.InvariantCulture)));
            Assert.Equal(float.MaxValue, JsonSerializer.Deserialize<float?>(float.MaxValue.ToString(CultureInfo.InvariantCulture)));

            Assert.Equal(double.MaxValue, JsonSerializer.Deserialize<double>(double.MaxValue.ToString(CultureInfo.InvariantCulture)));
            Assert.Equal(double.MaxValue, JsonSerializer.Deserialize<double?>(double.MaxValue.ToString(CultureInfo.InvariantCulture)));
        }

        [Fact]
        public static void ValueFail()
        {
            string unexpectedString = @"""unexpected string""";

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<byte>(unexpectedString));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<byte?>(unexpectedString));

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<sbyte>(unexpectedString));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<sbyte?>(unexpectedString));

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<short>(unexpectedString));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<short?>(unexpectedString));

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ushort>(unexpectedString));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ushort?>(unexpectedString));

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<float>(unexpectedString));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<float?>(unexpectedString));

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<int>(unexpectedString));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<int?>(unexpectedString));

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<uint>(unexpectedString));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<uint?>(unexpectedString));

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<long>(unexpectedString));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<long?>(unexpectedString));

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ulong>(unexpectedString));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ulong?>(unexpectedString));

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<decimal>(unexpectedString));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<decimal?>(unexpectedString));

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<double>(unexpectedString));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<double?>(unexpectedString));

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<DateTime>(unexpectedString));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<DateTime?>(unexpectedString));

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<DateTimeOffset>(unexpectedString));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<DateTimeOffset?>(unexpectedString));

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<string>("1"));

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<char>("1"));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<char?>("1"));

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Enum>(unexpectedString));
        }

        [Fact]
        public static void ReadPrimitiveUri()
        {
            Uri uri = JsonSerializer.Deserialize<Uri>(@"""https://domain/path""");
            Assert.Equal("https:\u002f\u002fdomain\u002fpath", uri.ToString());

            uri = JsonSerializer.Deserialize<Uri>(@"""~/path""");
            Assert.Equal("~/path", uri.ToString());
        }
   }
}
