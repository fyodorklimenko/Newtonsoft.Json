#region License
// Copyright (c) 2007 James Newton-King
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
#if !(NET20 || NET35 || SILVERLIGHT || PORTABLE)
using System.Numerics;
#endif
using System.Text;
using Newtonsoft.Json.Converters;
#if !NETFX_CORE
using NUnit.Framework;
#else
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using TestFixture = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestClassAttribute;
using Test = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestMethodAttribute;
#endif
using Newtonsoft.Json.Linq;
using System.IO;
#if NET20
using Newtonsoft.Json.Utilities.LinqBridge;
#else
using System.Linq;
using Newtonsoft.Json.Utilities;
#endif

namespace Newtonsoft.Json.Tests.Linq
{
  [TestFixture]
  public class JTokenTests : TestFixtureBase
  {
    [Test]
    public void ReadFrom()
    {
      JObject o = (JObject)JToken.ReadFrom(new JsonTextReader(new StringReader("{'pie':true}")));
      Assert.AreEqual(true, (bool)o["pie"]);

      JArray a = (JArray)JToken.ReadFrom(new JsonTextReader(new StringReader("[1,2,3]")));
      Assert.AreEqual(1, (int)a[0]);
      Assert.AreEqual(2, (int)a[1]);
      Assert.AreEqual(3, (int)a[2]);

      JsonReader reader = new JsonTextReader(new StringReader("{'pie':true}"));
      reader.Read();
      reader.Read();

      JProperty p = (JProperty)JToken.ReadFrom(reader);
      Assert.AreEqual("pie", p.Name);
      Assert.AreEqual(true, (bool)p.Value);

      JConstructor c = (JConstructor)JToken.ReadFrom(new JsonTextReader(new StringReader("new Date(1)")));
      Assert.AreEqual("Date", c.Name);
      Assert.IsTrue(JToken.DeepEquals(new JValue(1), c.Values().ElementAt(0)));

      JValue v;

      v = (JValue)JToken.ReadFrom(new JsonTextReader(new StringReader(@"""stringvalue""")));
      Assert.AreEqual("stringvalue", (string)v);

      v = (JValue)JToken.ReadFrom(new JsonTextReader(new StringReader(@"1")));
      Assert.AreEqual(1, (int)v);

      v = (JValue)JToken.ReadFrom(new JsonTextReader(new StringReader(@"1.1")));
      Assert.AreEqual(1.1, (double)v);

#if !NET20
      v = (JValue)JToken.ReadFrom(new JsonTextReader(new StringReader(@"""1970-01-01T00:00:00+12:31"""))
        {
          DateParseHandling = DateParseHandling.DateTimeOffset
        });
      Assert.AreEqual(typeof(DateTimeOffset), v.Value.GetType());
      Assert.AreEqual(new DateTimeOffset(DateTimeUtils.InitialJavaScriptDateTicks, new TimeSpan(12, 31, 0)), v.Value);
#endif
    }

    [Test]
    public void Load()
    {
      JObject o = (JObject)JToken.Load(new JsonTextReader(new StringReader("{'pie':true}")));
      Assert.AreEqual(true, (bool)o["pie"]);
    }

    [Test]
    public void Parse()
    {
      JObject o = (JObject)JToken.Parse("{'pie':true}");
      Assert.AreEqual(true, (bool)o["pie"]);
    }

    [Test]
    public void Parent()
    {
      JArray v = new JArray(new JConstructor("TestConstructor"), new JValue(new DateTime(2000, 12, 20)));

      Assert.AreEqual(null, v.Parent);

      JObject o =
        new JObject(
          new JProperty("Test1", v),
          new JProperty("Test2", "Test2Value"),
          new JProperty("Test3", "Test3Value"),
          new JProperty("Test4", null)
        );

      Assert.AreEqual(o.Property("Test1"), v.Parent);

      JProperty p = new JProperty("NewProperty", v);

      // existing value should still have same parent
      Assert.AreEqual(o.Property("Test1"), v.Parent);

      // new value should be cloned
      Assert.AreNotSame(p.Value, v);

      Assert.AreEqual((DateTime)((JValue)p.Value[1]).Value, (DateTime)((JValue)v[1]).Value);

      Assert.AreEqual(v, o["Test1"]);

      Assert.AreEqual(null, o.Parent);
      JProperty o1 = new JProperty("O1", o);
      Assert.AreEqual(o, o1.Value);

      Assert.AreNotEqual(null, o.Parent);
      JProperty o2 = new JProperty("O2", o);

      Assert.AreNotSame(o1.Value, o2.Value);
      Assert.AreEqual(o1.Value.Children().Count(), o2.Value.Children().Count());
      Assert.AreEqual(false, JToken.DeepEquals(o1, o2));
      Assert.AreEqual(true, JToken.DeepEquals(o1.Value, o2.Value));
    }

    [Test]
    public void Next()
    {
      JArray a =
        new JArray(
          5,
          6,
          new JArray(7, 8),
          new JArray(9, 10)
        );

      JToken next = a[0].Next;
      Assert.AreEqual(6, (int)next);

      next = next.Next;
      Assert.IsTrue(JToken.DeepEquals(new JArray(7, 8), next));
 
      next = next.Next;
      Assert.IsTrue(JToken.DeepEquals(new JArray(9, 10), next));

      next = next.Next;
      Assert.IsNull(next);
    }

    [Test]
    public void Previous()
    {
      JArray a =
        new JArray(
          5,
          6,
          new JArray(7, 8),
          new JArray(9, 10)
        );

      JToken previous = a[3].Previous;
      Assert.IsTrue(JToken.DeepEquals(new JArray(7, 8), previous));

      previous = previous.Previous;
      Assert.AreEqual(6, (int)previous);

      previous = previous.Previous;
      Assert.AreEqual(5, (int)previous);

      previous = previous.Previous;
      Assert.IsNull(previous);
    }

    [Test]
    public void Children()
    {
      JArray a =
        new JArray(
          5,
          new JArray(1),
          new JArray(1, 2),
          new JArray(1, 2, 3)
        );

      Assert.AreEqual(4, a.Count());
      Assert.AreEqual(3, a.Children<JArray>().Count());
    }

    [Test]
    public void BeforeAfter()
    {
      JArray a =
        new JArray(
          5,
          new JArray(1, 2, 3),
          new JArray(1, 2, 3),
          new JArray(1, 2, 3)
        );

      Assert.AreEqual(5, (int)a[1].Previous);
      Assert.AreEqual(2, a[2].BeforeSelf().Count());
      //Assert.AreEqual(2, a[2].AfterSelf().Count());
    }

    [Test]
    public void Casting()
    {
      Assert.AreEqual(1L, (long)(new JValue(1)));
      Assert.AreEqual(2L, (long)new JArray(1, 2, 3)[1]);

      Assert.AreEqual(new DateTime(2000, 12, 20), (DateTime)new JValue(new DateTime(2000, 12, 20)));
#if !NET20
      Assert.AreEqual(new DateTimeOffset(2000, 12, 20, 0, 0, 0, TimeSpan.Zero), (DateTimeOffset)new JValue(new DateTime(2000, 12, 20, 0, 0, 0, DateTimeKind.Utc)));
      Assert.AreEqual(new DateTimeOffset(2000, 12, 20, 23, 50, 10, TimeSpan.Zero), (DateTimeOffset)new JValue(new DateTimeOffset(2000, 12, 20, 23, 50, 10, TimeSpan.Zero)));
      Assert.AreEqual(null, (DateTimeOffset?)new JValue((DateTimeOffset?)null));
      Assert.AreEqual(null, (DateTimeOffset?)(JValue)null);
#endif
      Assert.AreEqual(true, (bool)new JValue(true));
      Assert.AreEqual(true, (bool?)new JValue(true));
      Assert.AreEqual(null, (bool?)((JValue)null));
      Assert.AreEqual(null, (bool?)new JValue((object)null));
      Assert.AreEqual(10, (long)new JValue(10));
      Assert.AreEqual(null, (long?)new JValue((long?)null));
      Assert.AreEqual(null, (long?)(JValue)null);
      Assert.AreEqual(null, (int?)new JValue((int?)null));
      Assert.AreEqual(null, (int?)(JValue)null);
      Assert.AreEqual(null, (DateTime?)new JValue((DateTime?)null));
      Assert.AreEqual(null, (DateTime?)(JValue)null);
      Assert.AreEqual(null, (short?)new JValue((short?)null));
      Assert.AreEqual(null, (short?)(JValue)null);
      Assert.AreEqual(null, (float?)new JValue((float?)null));
      Assert.AreEqual(null, (float?)(JValue)null);
      Assert.AreEqual(null, (double?)new JValue((double?)null));
      Assert.AreEqual(null, (double?)(JValue)null);
      Assert.AreEqual(null, (decimal?)new JValue((decimal?)null));
      Assert.AreEqual(null, (decimal?)(JValue)null);
      Assert.AreEqual(null, (uint?)new JValue((uint?)null));
      Assert.AreEqual(null, (uint?)(JValue)null);
      Assert.AreEqual(null, (sbyte?)new JValue((sbyte?)null));
      Assert.AreEqual(null, (sbyte?)(JValue)null);
      Assert.AreEqual(null, (byte?)new JValue((byte?)null));
      Assert.AreEqual(null, (byte?)(JValue)null);
      Assert.AreEqual(null, (ulong?)new JValue((ulong?)null));
      Assert.AreEqual(null, (ulong?)(JValue)null);
      Assert.AreEqual(null, (uint?)new JValue((uint?)null));
      Assert.AreEqual(null, (uint?)(JValue)null);
      Assert.AreEqual(11.1f, (float)new JValue(11.1));
      Assert.AreEqual(float.MinValue, (float)new JValue(float.MinValue));
      Assert.AreEqual(1.1, (double)new JValue(1.1));
      Assert.AreEqual(uint.MaxValue, (uint)new JValue(uint.MaxValue));
      Assert.AreEqual(ulong.MaxValue, (ulong)new JValue(ulong.MaxValue));
      Assert.AreEqual(ulong.MaxValue, (ulong)new JProperty("Test", new JValue(ulong.MaxValue)));
      Assert.AreEqual(null, (string)new JValue((string)null));
      Assert.AreEqual(5m, (decimal)(new JValue(5L)));
      Assert.AreEqual(5m, (decimal?)(new JValue(5L)));
      Assert.AreEqual(5f, (float)(new JValue(5L)));
      Assert.AreEqual(5f, (float)(new JValue(5m)));
      Assert.AreEqual(5f, (float?)(new JValue(5m)));
      Assert.AreEqual(5, (byte)(new JValue(5)));

      Assert.AreEqual(null, (sbyte?)new JValue((object)null));

      Assert.AreEqual("1", (string)(new JValue(1)));
      Assert.AreEqual("1", (string)(new JValue(1.0)));
      Assert.AreEqual("1.0", (string)(new JValue(1.0m)));
      Assert.AreEqual("True", (string)(new JValue(true)));
      Assert.AreEqual(null, (string)(new JValue((object)null)));
      Assert.AreEqual(null, (string)(JValue)null);
      Assert.AreEqual("12/12/2000 12:12:12", (string)(new JValue(new DateTime(2000, 12, 12, 12, 12, 12, DateTimeKind.Utc))));
#if !NET20
      Assert.AreEqual("12/12/2000 12:12:12 +00:00", (string)(new JValue(new DateTimeOffset(2000, 12, 12, 12, 12, 12, TimeSpan.Zero))));
#endif
      Assert.AreEqual(true, (bool)(new JValue(1)));
      Assert.AreEqual(true, (bool)(new JValue(1.0)));
      Assert.AreEqual(true, (bool)(new JValue("true")));
      Assert.AreEqual(true, (bool)(new JValue(true)));
      Assert.AreEqual(1, (int)(new JValue(1)));
      Assert.AreEqual(1, (int)(new JValue(1.0)));
      Assert.AreEqual(1, (int)(new JValue("1")));
      Assert.AreEqual(1, (int)(new JValue(true)));
      Assert.AreEqual(1m, (decimal)(new JValue(1)));
      Assert.AreEqual(1m, (decimal)(new JValue(1.0)));
      Assert.AreEqual(1m, (decimal)(new JValue("1")));
      Assert.AreEqual(1m, (decimal)(new JValue(true)));
      Assert.AreEqual(TimeSpan.FromMinutes(1), (TimeSpan)(new JValue(TimeSpan.FromMinutes(1))));
      Assert.AreEqual("00:01:00", (string)(new JValue(TimeSpan.FromMinutes(1))));
      Assert.AreEqual(TimeSpan.FromMinutes(1), (TimeSpan)(new JValue("00:01:00")));
      Assert.AreEqual("46efe013-b56a-4e83-99e4-4dce7678a5bc", (string)(new JValue(new Guid("46EFE013-B56A-4E83-99E4-4DCE7678A5BC"))));
      Assert.AreEqual("http://www.google.com/", (string)(new JValue(new Uri("http://www.google.com"))));
      Assert.AreEqual(new Guid("46EFE013-B56A-4E83-99E4-4DCE7678A5BC"), (Guid)(new JValue("46EFE013-B56A-4E83-99E4-4DCE7678A5BC")));
      Assert.AreEqual(new Guid("46EFE013-B56A-4E83-99E4-4DCE7678A5BC"), (Guid)(new JValue(new Guid("46EFE013-B56A-4E83-99E4-4DCE7678A5BC"))));
      Assert.AreEqual(new Uri("http://www.google.com"), (Uri)(new JValue("http://www.google.com")));
      Assert.AreEqual(new Uri("http://www.google.com"), (Uri)(new JValue(new Uri("http://www.google.com"))));
      Assert.AreEqual(null, (Uri)(new JValue((object)null)));
      Assert.AreEqual(Convert.ToBase64String(Encoding.UTF8.GetBytes("hi")), (string)(new JValue(Encoding.UTF8.GetBytes("hi"))));
      CollectionAssert.AreEquivalent((byte[])Encoding.UTF8.GetBytes("hi"), (byte[])(new JValue(Convert.ToBase64String(Encoding.UTF8.GetBytes("hi")))));
      Assert.AreEqual(new Guid("46EFE013-B56A-4E83-99E4-4DCE7678A5BC"), (Guid)(new JValue(new Guid("46EFE013-B56A-4E83-99E4-4DCE7678A5BC").ToByteArray())));
      Assert.AreEqual(new Guid("46EFE013-B56A-4E83-99E4-4DCE7678A5BC"), (Guid?)(new JValue(new Guid("46EFE013-B56A-4E83-99E4-4DCE7678A5BC").ToByteArray())));
      Assert.AreEqual(1, (sbyte?)(new JValue((short?)1)));

      Assert.AreEqual(null, (Uri)(JValue)null);
      Assert.AreEqual(null, (int?)(JValue)null);
      Assert.AreEqual(null, (uint?)(JValue)null);
      Assert.AreEqual(null, (Guid?)(JValue)null);
      Assert.AreEqual(null, (TimeSpan?)(JValue)null);
      Assert.AreEqual(null, (byte[])(JValue)null);
      Assert.AreEqual(null, (bool?)(JValue)null);
      Assert.AreEqual(null, (char?)(JValue)null);
      Assert.AreEqual(null, (DateTime?)(JValue)null);
#if !NET20
      Assert.AreEqual(null, (DateTimeOffset?)(JValue)null);
#endif
      Assert.AreEqual(null, (short?)(JValue)null);
      Assert.AreEqual(null, (ushort?)(JValue)null);
      Assert.AreEqual(null, (byte?)(JValue)null);
      Assert.AreEqual(null, (byte?)(JValue)null);
      Assert.AreEqual(null, (sbyte?)(JValue)null);
      Assert.AreEqual(null, (sbyte?)(JValue)null);
      Assert.AreEqual(null, (long?)(JValue)null);
      Assert.AreEqual(null, (ulong?)(JValue)null);
      Assert.AreEqual(null, (double?)(JValue)null);
      Assert.AreEqual(null, (float?)(JValue)null);

      byte[] data = new byte[0];
      Assert.AreEqual(data, (byte[])(new JValue(data)));

      Assert.AreEqual(5, (int)(new JValue(StringComparison.OrdinalIgnoreCase)));

#if !(NET20 || NET35 || SILVERLIGHT || PORTABLE || PORTABLE40)
      string bigIntegerText = "1234567899999999999999999999999999999999999999999999999999999999999990";

      Assert.AreEqual(BigInteger.Parse(bigIntegerText), (new JValue(BigInteger.Parse(bigIntegerText))).Value);

      Assert.AreEqual(BigInteger.Parse(bigIntegerText), (new JValue(bigIntegerText)).ToObject<BigInteger>());
      Assert.AreEqual(new BigInteger(long.MaxValue), (new JValue(long.MaxValue)).ToObject<BigInteger>());
      Assert.AreEqual(new BigInteger(4.5d), (new JValue((4.5d))).ToObject<BigInteger>());
      Assert.AreEqual(new BigInteger(4.5f), (new JValue((4.5f))).ToObject<BigInteger>());
      Assert.AreEqual(new BigInteger(byte.MaxValue), (new JValue(byte.MaxValue)).ToObject<BigInteger>());
      Assert.AreEqual(new BigInteger(123), (new JValue(123)).ToObject<BigInteger>());
      Assert.AreEqual(new BigInteger(123), (new JValue(123)).ToObject<BigInteger?>());
      Assert.AreEqual(null, (new JValue((object)null)).ToObject<BigInteger?>());

      byte[] intData = BigInteger.Parse(bigIntegerText).ToByteArray();
      Assert.AreEqual(BigInteger.Parse(bigIntegerText), (new JValue(intData)).ToObject<BigInteger>());

      Assert.AreEqual(4.0d, (double)(new JValue(new BigInteger(4.5d))));
      Assert.AreEqual(true, (bool)(new JValue(new BigInteger(1))));
      Assert.AreEqual(long.MaxValue, (long)(new JValue(new BigInteger(long.MaxValue))));
      Assert.AreEqual(long.MaxValue, (long)(new JValue(new BigInteger(new byte[] { 255, 255, 255, 255, 255, 255, 255, 127 }))));
      Assert.AreEqual("9223372036854775807", (string)(new JValue(new BigInteger(long.MaxValue))));

      intData = (byte[]) (new JValue(new BigInteger(long.MaxValue)));
      CollectionAssert.AreEqual(new byte[] { 255, 255, 255, 255, 255, 255, 255, 127 }, intData);
#endif
    }

    [Test]
    public void FailedCasting()
    {
      ExceptionAssert.Throws<ArgumentException>("Can not convert Boolean to DateTime.", () => { var i = (DateTime)new JValue(true); });
      ExceptionAssert.Throws<ArgumentException>("Can not convert Integer to DateTime.", () => { var i = (DateTime)new JValue(1); });
      ExceptionAssert.Throws<ArgumentException>("Can not convert Float to DateTime.", () => { var i = (DateTime)new JValue(1.1); });
      ExceptionAssert.Throws<ArgumentException>("Can not convert Float to DateTime.", () => { var i = (DateTime)new JValue(1.1m); });
      ExceptionAssert.Throws<ArgumentException>("Can not convert TimeSpan to DateTime.", () => { var i = (DateTime)new JValue(TimeSpan.Zero); });
      ExceptionAssert.Throws<ArgumentException>("Can not convert Uri to DateTime.", () => { var i = (DateTime)new JValue(new Uri("http://www.google.com")); });
      ExceptionAssert.Throws<ArgumentException>("Can not convert Null to DateTime.", () => { var i = (DateTime)new JValue((object)null); });
      ExceptionAssert.Throws<ArgumentException>("Can not convert Guid to DateTime.", () => { var i = (DateTime)new JValue(Guid.NewGuid()); });

      ExceptionAssert.Throws<ArgumentException>("Can not convert Boolean to Uri.", () => { var i = (Uri)new JValue(true); });
      ExceptionAssert.Throws<ArgumentException>("Can not convert Integer to Uri.", () => { var i = (Uri)new JValue(1); });
      ExceptionAssert.Throws<ArgumentException>("Can not convert Float to Uri.", () => { var i = (Uri)new JValue(1.1); });
      ExceptionAssert.Throws<ArgumentException>("Can not convert Float to Uri.", () => { var i = (Uri)new JValue(1.1m); });
      ExceptionAssert.Throws<ArgumentException>("Can not convert TimeSpan to Uri.", () => { var i = (Uri)new JValue(TimeSpan.Zero); });
      ExceptionAssert.Throws<ArgumentException>("Can not convert Guid to Uri.", () => { var i = (Uri)new JValue(Guid.NewGuid()); });
      ExceptionAssert.Throws<ArgumentException>("Can not convert Date to Uri.", () => { var i = (Uri)new JValue(DateTime.Now); });
#if !NET20
      ExceptionAssert.Throws<ArgumentException>("Can not convert Date to Uri.", () => { var i = (Uri)new JValue(DateTimeOffset.Now); });
#endif

      ExceptionAssert.Throws<ArgumentException>("Can not convert Boolean to TimeSpan.", () => { var i = (TimeSpan)new JValue(true); });
      ExceptionAssert.Throws<ArgumentException>("Can not convert Integer to TimeSpan.", () => { var i = (TimeSpan)new JValue(1); });
      ExceptionAssert.Throws<ArgumentException>("Can not convert Float to TimeSpan.", () => { var i = (TimeSpan)new JValue(1.1); });
      ExceptionAssert.Throws<ArgumentException>("Can not convert Float to TimeSpan.", () => { var i = (TimeSpan)new JValue(1.1m); });
      ExceptionAssert.Throws<ArgumentException>("Can not convert Null to TimeSpan.", () => { var i = (TimeSpan)new JValue((object)null); });
      ExceptionAssert.Throws<ArgumentException>("Can not convert Guid to TimeSpan.", () => { var i = (TimeSpan)new JValue(Guid.NewGuid()); });
      ExceptionAssert.Throws<ArgumentException>("Can not convert Date to TimeSpan.", () => { var i = (TimeSpan)new JValue(DateTime.Now); });
#if !NET20
      ExceptionAssert.Throws<ArgumentException>("Can not convert Date to TimeSpan.", () => { var i = (TimeSpan)new JValue(DateTimeOffset.Now); });
#endif
      ExceptionAssert.Throws<ArgumentException>("Can not convert Uri to TimeSpan.", () => { var i = (TimeSpan)new JValue(new Uri("http://www.google.com")); });

      ExceptionAssert.Throws<ArgumentException>("Can not convert Boolean to Guid.", () => { var i = (Guid)new JValue(true); });
      ExceptionAssert.Throws<ArgumentException>("Can not convert Integer to Guid.", () => { var i = (Guid)new JValue(1); });
      ExceptionAssert.Throws<ArgumentException>("Can not convert Float to Guid.", () => { var i = (Guid)new JValue(1.1); });
      ExceptionAssert.Throws<ArgumentException>("Can not convert Float to Guid.", () => { var i = (Guid)new JValue(1.1m); });
      ExceptionAssert.Throws<ArgumentException>("Can not convert Null to Guid.", () => { var i = (Guid)new JValue((object)null); });
      ExceptionAssert.Throws<ArgumentException>("Can not convert Date to Guid.", () => { var i = (Guid)new JValue(DateTime.Now); });
#if !NET20
      ExceptionAssert.Throws<ArgumentException>("Can not convert Date to Guid.", () => { var i = (Guid)new JValue(DateTimeOffset.Now); });
#endif
      ExceptionAssert.Throws<ArgumentException>("Can not convert TimeSpan to Guid.", () => { var i = (Guid)new JValue(TimeSpan.FromMinutes(1)); });
      ExceptionAssert.Throws<ArgumentException>("Can not convert Uri to Guid.", () => { var i = (Guid)new JValue(new Uri("http://www.google.com")); });

#if !NET20
      ExceptionAssert.Throws<ArgumentException>("Can not convert Boolean to DateTimeOffset.", () => { var i = (DateTimeOffset)new JValue(true); });
#endif
      ExceptionAssert.Throws<ArgumentException>("Can not convert Boolean to Uri.", () => { var i = (Uri)new JValue(true); });

#if !(NET20 || NET35 || SILVERLIGHT || PORTABLE || PORTABLE40)
      ExceptionAssert.Throws<ArgumentException>("Can not convert Uri to BigInteger.", () => { var i = (new JValue(new Uri("http://www.google.com"))).ToObject<BigInteger>(); });
      ExceptionAssert.Throws<ArgumentException>("Can not convert Null to BigInteger.", () => { var i = (new JValue((object)null)).ToObject<BigInteger>(); });
      ExceptionAssert.Throws<ArgumentException>("Can not convert Guid to BigInteger.", () => { var i = (new JValue(Guid.NewGuid())).ToObject<BigInteger>(); });
      ExceptionAssert.Throws<ArgumentException>("Can not convert Guid to BigInteger.", () => { var i = (new JValue(Guid.NewGuid())).ToObject<BigInteger?>(); });
#endif

      ExceptionAssert.Throws<ArgumentException>("Can not convert Date to SByte.", () => { var i = (sbyte?)new JValue(DateTime.Now); });
      ExceptionAssert.Throws<ArgumentException>("Can not convert Date to SByte.", () => { var i = (sbyte)new JValue(DateTime.Now); });
    }

    [Test]
    public void ToObject()
    {
#if !(NET20 || NET35 || SILVERLIGHT || PORTABLE)
      Assert.AreEqual((BigInteger)1, (new JValue(1).ToObject(typeof(BigInteger))));
      Assert.AreEqual((BigInteger)1, (new JValue(1).ToObject(typeof(BigInteger?))));
      Assert.AreEqual((BigInteger?)null, (new JValue((object)null).ToObject(typeof(BigInteger?))));
#endif
      Assert.AreEqual((ushort)1, (new JValue(1).ToObject(typeof(ushort))));
      Assert.AreEqual((ushort)1, (new JValue(1).ToObject(typeof(ushort?))));
      Assert.AreEqual((uint)1L, (new JValue(1).ToObject(typeof(uint))));
      Assert.AreEqual((uint)1L, (new JValue(1).ToObject(typeof(uint?))));
      Assert.AreEqual((ulong)1L, (new JValue(1).ToObject(typeof(ulong))));
      Assert.AreEqual((ulong)1L, (new JValue(1).ToObject(typeof(ulong?))));
      Assert.AreEqual((sbyte)1L, (new JValue(1).ToObject(typeof(sbyte))));
      Assert.AreEqual((sbyte)1L, (new JValue(1).ToObject(typeof(sbyte?))));
      Assert.AreEqual((byte)1L, (new JValue(1).ToObject(typeof(byte))));
      Assert.AreEqual((byte)1L, (new JValue(1).ToObject(typeof(byte?))));
      Assert.AreEqual((short)1L, (new JValue(1).ToObject(typeof(short))));
      Assert.AreEqual((short)1L, (new JValue(1).ToObject(typeof(short?))));
      Assert.AreEqual(1, (new JValue(1).ToObject(typeof(int))));
      Assert.AreEqual(1, (new JValue(1).ToObject(typeof(int?))));
      Assert.AreEqual(1L, (new JValue(1).ToObject(typeof(long))));
      Assert.AreEqual(1L, (new JValue(1).ToObject(typeof(long?))));
      Assert.AreEqual((float)1, (new JValue(1.0).ToObject(typeof(float))));
      Assert.AreEqual((float)1, (new JValue(1.0).ToObject(typeof(float?))));
      Assert.AreEqual((double)1, (new JValue(1.0).ToObject(typeof(double))));
      Assert.AreEqual((double)1, (new JValue(1.0).ToObject(typeof(double?))));
      Assert.AreEqual(1m, (new JValue(1).ToObject(typeof(decimal))));
      Assert.AreEqual(1m, (new JValue(1).ToObject(typeof(decimal?))));
      Assert.AreEqual(true, (new JValue(true).ToObject(typeof(bool))));
      Assert.AreEqual(true, (new JValue(true).ToObject(typeof(bool?))));
      Assert.AreEqual('b', (new JValue('b').ToObject(typeof(char))));
      Assert.AreEqual('b', (new JValue('b').ToObject(typeof(char?))));
      Assert.AreEqual(TimeSpan.MaxValue, (new JValue(TimeSpan.MaxValue).ToObject(typeof(TimeSpan))));
      Assert.AreEqual(TimeSpan.MaxValue, (new JValue(TimeSpan.MaxValue).ToObject(typeof(TimeSpan?))));
      Assert.AreEqual(DateTime.MaxValue, (new JValue(DateTime.MaxValue).ToObject(typeof(DateTime))));
      Assert.AreEqual(DateTime.MaxValue, (new JValue(DateTime.MaxValue).ToObject(typeof(DateTime?))));
#if !NET20
      Assert.AreEqual(DateTimeOffset.MaxValue, (new JValue(DateTimeOffset.MaxValue).ToObject(typeof(DateTimeOffset))));
      Assert.AreEqual(DateTimeOffset.MaxValue, (new JValue(DateTimeOffset.MaxValue).ToObject(typeof(DateTimeOffset?))));
#endif
      Assert.AreEqual("b", (new JValue("b").ToObject(typeof(string))));
      Assert.AreEqual(new Guid("A34B2080-B5F0-488E-834D-45D44ECB9E5C"), (new JValue(new Guid("A34B2080-B5F0-488E-834D-45D44ECB9E5C")).ToObject(typeof(Guid))));
      Assert.AreEqual(new Guid("A34B2080-B5F0-488E-834D-45D44ECB9E5C"), (new JValue(new Guid("A34B2080-B5F0-488E-834D-45D44ECB9E5C")).ToObject(typeof(Guid?))));
      Assert.AreEqual(new Uri("http://www.google.com/"), (new JValue(new Uri("http://www.google.com/")).ToObject(typeof(Uri))));
    }

    [Test]
    public void ImplicitCastingTo()
    {
      Assert.IsTrue(JToken.DeepEquals(new JValue(new DateTime(2000, 12, 20)), (JValue)new DateTime(2000, 12, 20)));
#if !NET20
      Assert.IsTrue(JToken.DeepEquals(new JValue(new DateTimeOffset(2000, 12, 20, 23, 50, 10, TimeSpan.Zero)), (JValue)new DateTimeOffset(2000, 12, 20, 23, 50, 10, TimeSpan.Zero)));
      Assert.IsTrue(JToken.DeepEquals(new JValue((DateTimeOffset?)null), (JValue)(DateTimeOffset?)null));
#endif

#if !(NET20 || NET35 || SILVERLIGHT || PORTABLE || PORTABLE40)
      // had to remove implicit casting to avoid user reference to System.Numerics.dll
      Assert.IsTrue(JToken.DeepEquals(new JValue(new BigInteger(1)), new JValue(new BigInteger(1))));
      Assert.IsTrue(JToken.DeepEquals(new JValue((BigInteger?)null), new JValue((BigInteger?)null)));
#endif
      Assert.IsTrue(JToken.DeepEquals(new JValue(true), (JValue)true));
      Assert.IsTrue(JToken.DeepEquals(new JValue(true), (JValue)true));
      Assert.IsTrue(JToken.DeepEquals(new JValue(true), (JValue)(bool?)true));
      Assert.IsTrue(JToken.DeepEquals(new JValue((bool?)null), (JValue)(bool?)null));
      Assert.IsTrue(JToken.DeepEquals(new JValue(10), (JValue)10));
      Assert.IsTrue(JToken.DeepEquals(new JValue((long?)null), (JValue)(long?)null));
      Assert.IsTrue(JToken.DeepEquals(new JValue((DateTime?)null), (JValue)(DateTime?)null));
      Assert.IsTrue(JToken.DeepEquals(new JValue(long.MaxValue), (JValue)long.MaxValue));
      Assert.IsTrue(JToken.DeepEquals(new JValue((int?)null), (JValue)(int?)null));
      Assert.IsTrue(JToken.DeepEquals(new JValue((short?)null), (JValue)(short?)null));
      Assert.IsTrue(JToken.DeepEquals(new JValue((double?)null), (JValue)(double?)null));
      Assert.IsTrue(JToken.DeepEquals(new JValue((uint?)null), (JValue)(uint?)null));
      Assert.IsTrue(JToken.DeepEquals(new JValue((decimal?)null), (JValue)(decimal?)null));
      Assert.IsTrue(JToken.DeepEquals(new JValue((ulong?)null), (JValue)(ulong?)null));
      Assert.IsTrue(JToken.DeepEquals(new JValue((sbyte?)null), (JValue)(sbyte?)null));
      Assert.IsTrue(JToken.DeepEquals(new JValue((sbyte)1), (JValue)(sbyte)1));
      Assert.IsTrue(JToken.DeepEquals(new JValue((byte?)null), (JValue)(byte?)null));
      Assert.IsTrue(JToken.DeepEquals(new JValue((byte)1), (JValue)(byte)1));
      Assert.IsTrue(JToken.DeepEquals(new JValue((ushort?)null), (JValue)(ushort?)null));
      Assert.IsTrue(JToken.DeepEquals(new JValue(short.MaxValue), (JValue)short.MaxValue));
      Assert.IsTrue(JToken.DeepEquals(new JValue(ushort.MaxValue), (JValue)ushort.MaxValue));
      Assert.IsTrue(JToken.DeepEquals(new JValue(11.1f), (JValue)11.1f));
      Assert.IsTrue(JToken.DeepEquals(new JValue(float.MinValue), (JValue)float.MinValue));
      Assert.IsTrue(JToken.DeepEquals(new JValue(double.MinValue), (JValue)double.MinValue));
      Assert.IsTrue(JToken.DeepEquals(new JValue(uint.MaxValue), (JValue)uint.MaxValue));
      Assert.IsTrue(JToken.DeepEquals(new JValue(ulong.MaxValue), (JValue)ulong.MaxValue));
      Assert.IsTrue(JToken.DeepEquals(new JValue(ulong.MinValue), (JValue)ulong.MinValue));
      Assert.IsTrue(JToken.DeepEquals(new JValue((string)null), (JValue)(string)null));
      Assert.IsTrue(JToken.DeepEquals(new JValue((DateTime?)null), (JValue)(DateTime?)null));
      Assert.IsTrue(JToken.DeepEquals(new JValue(decimal.MaxValue), (JValue)decimal.MaxValue));
      Assert.IsTrue(JToken.DeepEquals(new JValue(decimal.MaxValue), (JValue)(decimal?)decimal.MaxValue));
      Assert.IsTrue(JToken.DeepEquals(new JValue(decimal.MinValue), (JValue)decimal.MinValue));
      Assert.IsTrue(JToken.DeepEquals(new JValue(float.MaxValue), (JValue)(float?)float.MaxValue));
      Assert.IsTrue(JToken.DeepEquals(new JValue(double.MaxValue), (JValue)(double?)double.MaxValue));
      Assert.IsTrue(JToken.DeepEquals(new JValue((object)null), (JValue)(double?)null));

      Assert.IsFalse(JToken.DeepEquals(new JValue(true), (JValue)(bool?)null));
      Assert.IsFalse(JToken.DeepEquals(new JValue((object)null), (JValue)(object)null));

      byte[] emptyData = new byte[0];
      Assert.IsTrue(JToken.DeepEquals(new JValue(emptyData), (JValue)emptyData));
      Assert.IsFalse(JToken.DeepEquals(new JValue(emptyData), (JValue)new byte[1]));
      Assert.IsTrue(JToken.DeepEquals(new JValue(Encoding.UTF8.GetBytes("Hi")), (JValue)Encoding.UTF8.GetBytes("Hi")));

      Assert.IsTrue(JToken.DeepEquals(new JValue(TimeSpan.FromMinutes(1)), (JValue)TimeSpan.FromMinutes(1)));
      Assert.IsTrue(JToken.DeepEquals(new JValue((object)null), (JValue)(TimeSpan?)null));
      Assert.IsTrue(JToken.DeepEquals(new JValue(TimeSpan.FromMinutes(1)), (JValue)(TimeSpan?)TimeSpan.FromMinutes(1)));
      Assert.IsTrue(JToken.DeepEquals(new JValue(new Guid("46EFE013-B56A-4E83-99E4-4DCE7678A5BC")), (JValue)new Guid("46EFE013-B56A-4E83-99E4-4DCE7678A5BC")));
      Assert.IsTrue(JToken.DeepEquals(new JValue(new Uri("http://www.google.com")), (JValue)new Uri("http://www.google.com")));
      Assert.IsTrue(JToken.DeepEquals(new JValue((object)null), (JValue)(Uri)null));
      Assert.IsTrue(JToken.DeepEquals(new JValue((object)null), (JValue)(Guid?)null));
    }

    [Test]
    public void Root()
    {
      JArray a =
        new JArray(
          5,
          6,
          new JArray(7, 8),
          new JArray(9, 10)
        );

      Assert.AreEqual(a, a.Root);
      Assert.AreEqual(a, a[0].Root);
      Assert.AreEqual(a, ((JArray)a[2])[0].Root);
    }

    [Test]
    public void Remove()
    {
      JToken t;
      JArray a =
        new JArray(
          5,
          6,
          new JArray(7, 8),
          new JArray(9, 10)
        );

      a[0].Remove();

      Assert.AreEqual(6, (int)a[0]);

      a[1].Remove();

      Assert.AreEqual(6, (int)a[0]);
      Assert.IsTrue(JToken.DeepEquals(new JArray(9, 10), a[1]));
      Assert.AreEqual(2, a.Count());

      t = a[1];
      t.Remove();
      Assert.AreEqual(6, (int)a[0]);
      Assert.IsNull(t.Next);
      Assert.IsNull(t.Previous);
      Assert.IsNull(t.Parent);

      t = a[0];
      t.Remove();
      Assert.AreEqual(0, a.Count());

      Assert.IsNull(t.Next);
      Assert.IsNull(t.Previous);
      Assert.IsNull(t.Parent);
    }

    [Test]
    public void AfterSelf()
    {
      JArray a =
        new JArray(
          5,
          new JArray(1),
          new JArray(1, 2),
          new JArray(1, 2, 3)
        );

      JToken t = a[1];
      List<JToken> afterTokens = t.AfterSelf().ToList();

      Assert.AreEqual(2, afterTokens.Count);
      Assert.IsTrue(JToken.DeepEquals(new JArray(1, 2), afterTokens[0]));
      Assert.IsTrue(JToken.DeepEquals(new JArray(1, 2, 3), afterTokens[1]));
    }

    [Test]
    public void BeforeSelf()
    {
      JArray a =
        new JArray(
          5,
          new JArray(1),
          new JArray(1, 2),
          new JArray(1, 2, 3)
        );

      JToken t = a[2];
      List<JToken> beforeTokens = t.BeforeSelf().ToList();

      Assert.AreEqual(2, beforeTokens.Count);
      Assert.IsTrue(JToken.DeepEquals(new JValue(5), beforeTokens[0]));
      Assert.IsTrue(JToken.DeepEquals(new JArray(1), beforeTokens[1]));
    }

    [Test]
    public void HasValues()
    {
      JArray a =
        new JArray(
          5,
          new JArray(1),
          new JArray(1, 2),
          new JArray(1, 2, 3)
        );

      Assert.IsTrue(a.HasValues);
    }

    [Test]
    public void Ancestors()
    {
      JArray a =
        new JArray(
          5,
          new JArray(1),
          new JArray(1, 2),
          new JArray(1, 2, 3)
        );

      JToken t = a[1][0];
      List<JToken> ancestors = t.Ancestors().ToList();
      Assert.AreEqual(2, ancestors.Count());
      Assert.AreEqual(a[1], ancestors[0]);
      Assert.AreEqual(a, ancestors[1]);
    }

    [Test]
    public void Descendants()
    {
      JArray a =
        new JArray(
          5,
          new JArray(1),
          new JArray(1, 2),
          new JArray(1, 2, 3)
        );

      List<JToken> descendants = a.Descendants().ToList();
      Assert.AreEqual(10, descendants.Count());
      Assert.AreEqual(5, (int)descendants[0]);
      Assert.IsTrue(JToken.DeepEquals(new JArray(1, 2, 3), descendants[descendants.Count - 4]));
      Assert.AreEqual(1, (int)descendants[descendants.Count - 3]);
      Assert.AreEqual(2, (int)descendants[descendants.Count - 2]);
      Assert.AreEqual(3, (int)descendants[descendants.Count - 1]);
    }

    [Test]
    public void CreateWriter()
    {
      JArray a =
        new JArray(
          5,
          new JArray(1),
          new JArray(1, 2),
          new JArray(1, 2, 3)
        );

      JsonWriter writer = a.CreateWriter();
      Assert.IsNotNull(writer);
      Assert.AreEqual(4, a.Count());

      writer.WriteValue("String");
      Assert.AreEqual(5, a.Count());
      Assert.AreEqual("String", (string)a[4]);

      writer.WriteStartObject();
      writer.WritePropertyName("Property");
      writer.WriteValue("PropertyValue");
      writer.WriteEnd();

      Assert.AreEqual(6, a.Count());
      Assert.IsTrue(JToken.DeepEquals(new JObject(new JProperty("Property", "PropertyValue")), a[5]));
    }

    [Test]
    public void AddFirst()
    {
      JArray a =
        new JArray(
          5,
          new JArray(1),
          new JArray(1, 2),
          new JArray(1, 2, 3)
        );

      a.AddFirst("First");

      Assert.AreEqual("First", (string)a[0]);
      Assert.AreEqual(a, a[0].Parent);
      Assert.AreEqual(a[1], a[0].Next);
      Assert.AreEqual(5, a.Count());

      a.AddFirst("NewFirst");
      Assert.AreEqual("NewFirst", (string)a[0]);
      Assert.AreEqual(a, a[0].Parent);
      Assert.AreEqual(a[1], a[0].Next);
      Assert.AreEqual(6, a.Count());

      Assert.AreEqual(a[0], a[0].Next.Previous);
    }

    [Test]
    public void RemoveAll()
    {
      JArray a =
        new JArray(
          5,
          new JArray(1),
          new JArray(1, 2),
          new JArray(1, 2, 3)
        );

      JToken first = a.First;
      Assert.AreEqual(5, (int)first);

      a.RemoveAll();
      Assert.AreEqual(0, a.Count());

      Assert.IsNull(first.Parent);
      Assert.IsNull(first.Next);
    }

    [Test]
    public void AddPropertyToArray()
    {
      ExceptionAssert.Throws<ArgumentException>("Can not add Newtonsoft.Json.Linq.JProperty to Newtonsoft.Json.Linq.JArray.",
      () =>
      {
        JArray a = new JArray();
        a.Add(new JProperty("PropertyName"));
      });
    }

    [Test]
    public void AddValueToObject()
    {
      ExceptionAssert.Throws<ArgumentException>(
        "Can not add Newtonsoft.Json.Linq.JValue to Newtonsoft.Json.Linq.JObject.",
        () =>
        {
          JObject o = new JObject();
          o.Add(5);
        });
    }

    [Test]
    public void Replace()
    {
      JArray a =
        new JArray(
          5,
          new JArray(1),
          new JArray(1, 2),
          new JArray(1, 2, 3)
        );

      a[0].Replace(new JValue(int.MaxValue));
      Assert.AreEqual(int.MaxValue, (int)a[0]);
      Assert.AreEqual(4, a.Count());

      a[1][0].Replace(new JValue("Test"));
      Assert.AreEqual("Test", (string)a[1][0]);

      a[2].Replace(new JValue(int.MaxValue));
      Assert.AreEqual(int.MaxValue, (int)a[2]);
      Assert.AreEqual(4, a.Count());

      Assert.IsTrue(JToken.DeepEquals(new JArray(int.MaxValue, new JArray("Test"), int.MaxValue, new JArray(1, 2, 3)), a));
    }

    [Test]
    public void ToStringWithConverters()
    {
      JArray a =
        new JArray(
          new JValue(new DateTime(2009, 2, 15, 0, 0, 0, DateTimeKind.Utc))
        );

      string json = a.ToString(Formatting.Indented, new IsoDateTimeConverter());

      Assert.AreEqual(@"[
  ""2009-02-15T00:00:00Z""
]", json);

      json = JsonConvert.SerializeObject(a, new IsoDateTimeConverter());

      Assert.AreEqual(@"[""2009-02-15T00:00:00Z""]", json);
    }

    [Test]
    public void ToStringWithNoIndenting()
    {
      JArray a =
        new JArray(
          new JValue(new DateTime(2009, 2, 15, 0, 0, 0, DateTimeKind.Utc))
        );

      string json = a.ToString(Formatting.None, new IsoDateTimeConverter());

      Assert.AreEqual(@"[""2009-02-15T00:00:00Z""]", json);
    }

    [Test]
    public void AddAfterSelf()
    {
      JArray a =
        new JArray(
          5,
          new JArray(1),
          new JArray(1, 2),
          new JArray(1, 2, 3)
        );

      a[1].AddAfterSelf("pie");

      Assert.AreEqual(5, (int)a[0]);
      Assert.AreEqual(1, a[1].Count());
      Assert.AreEqual("pie", (string)a[2]);
      Assert.AreEqual(5, a.Count());

      a[4].AddAfterSelf("lastpie");

      Assert.AreEqual("lastpie", (string)a[5]);
      Assert.AreEqual("lastpie", (string)a.Last);
    }

    [Test]
    public void AddBeforeSelf()
    {
      JArray a =
        new JArray(
          5,
          new JArray(1),
          new JArray(1, 2),
          new JArray(1, 2, 3)
        );

      a[1].AddBeforeSelf("pie");

      Assert.AreEqual(5, (int)a[0]);
      Assert.AreEqual("pie", (string)a[1]);
      Assert.AreEqual(a, a[1].Parent);
      Assert.AreEqual(a[2], a[1].Next);
      Assert.AreEqual(5, a.Count());

      a[0].AddBeforeSelf("firstpie");

      Assert.AreEqual("firstpie", (string)a[0]);
      Assert.AreEqual(5, (int)a[1]);
      Assert.AreEqual("pie", (string)a[2]);
      Assert.AreEqual(a, a[0].Parent);
      Assert.AreEqual(a[1], a[0].Next);
      Assert.AreEqual(6, a.Count());

      a.Last.AddBeforeSelf("secondlastpie");

      Assert.AreEqual("secondlastpie", (string)a[5]);
      Assert.AreEqual(7, a.Count());
    }

    [Test]
    public void DeepClone()
    {
      JArray a =
        new JArray(
          5,
          new JArray(1),
          new JArray(1, 2),
          new JArray(1, 2, 3),
          new JObject(
            new JProperty("First", new JValue(Encoding.UTF8.GetBytes("Hi"))),
            new JProperty("Second", 1),
            new JProperty("Third", null),
            new JProperty("Fourth", new JConstructor("Date", 12345)),
            new JProperty("Fifth", double.PositiveInfinity),
            new JProperty("Sixth", double.NaN)
            )
        );

      JArray a2 = (JArray)a.DeepClone();

      Console.WriteLine(a2.ToString(Formatting.Indented));

      Assert.IsTrue(a.DeepEquals(a2));
    }

#if !(SILVERLIGHT || NETFX_CORE || PORTABLE || PORTABLE40)
    [Test]
    public void Clone()
    {
      JArray a =
        new JArray(
          5,
          new JArray(1),
          new JArray(1, 2),
          new JArray(1, 2, 3),
          new JObject(
            new JProperty("First", new JValue(Encoding.UTF8.GetBytes("Hi"))),
            new JProperty("Second", 1),
            new JProperty("Third", null),
            new JProperty("Fourth", new JConstructor("Date", 12345)),
            new JProperty("Fifth", double.PositiveInfinity),
            new JProperty("Sixth", double.NaN)
            )
        );

      ICloneable c = a;

      JArray a2 = (JArray) c.Clone();

      Assert.IsTrue(a.DeepEquals(a2));
    }
#endif

    [Test]
    public void DoubleDeepEquals()
    {
      JArray a =
        new JArray(
          double.NaN,
          double.PositiveInfinity,
          double.NegativeInfinity
        );

      JArray a2 = (JArray)a.DeepClone();

      Assert.IsTrue(a.DeepEquals(a2));

      double d = 1 + 0.1 + 0.1 + 0.1;

      JValue v1 = new JValue(d);
      JValue v2 = new JValue(1.3);

      Assert.IsTrue(v1.DeepEquals(v2));
    }

    [Test]
    public void ParseAdditionalContent()
    {
      ExceptionAssert.Throws<JsonReaderException>("Additional text encountered after finished reading JSON content: ,. Path '', line 5, position 2.",
        () =>
        {
          string json = @"[
""Small"",
""Medium"",
""Large""
],";

          JToken.Parse(json);
        });
    }

    [Test]
    public void Path()
    {
      JObject o =
        new JObject(
          new JProperty("Test1", new JArray(1, 2, 3)),
          new JProperty("Test2", "Test2Value"),
          new JProperty("Test3", new JObject(new JProperty("Test1", new JArray(1, new JObject(new JProperty("Test1", 1)), 3)))),
          new JProperty("Test4", new JConstructor("Date", new JArray(1, 2, 3)))
          );

      JToken t = o.SelectToken("Test1[0]");
      Assert.AreEqual("Test1[0]", t.Path);

      t = o.SelectToken("Test2");
      Assert.AreEqual("Test2", t.Path);

      t = o.SelectToken("");
      Assert.AreEqual("", t.Path);

      t = o.SelectToken("Test4[0][0]");
      Assert.AreEqual("Test4[0][0]", t.Path);

      t = o.SelectToken("Test4[0]");
      Assert.AreEqual("Test4[0]", t.Path);

      t = t.DeepClone();
      Assert.AreEqual("", t.Path);

      t = o.SelectToken("Test3.Test1[1].Test1");
      Assert.AreEqual("Test3.Test1[1].Test1", t.Path);

      JArray a = new JArray(1);
      Assert.AreEqual("", a.Path);
      
      Assert.AreEqual("[0]", a[0].Path);
    }
  }
}