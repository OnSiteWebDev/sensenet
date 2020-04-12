using Microsoft.VisualStudio.TestTools.UnitTesting;
using SenseNet.Search.Querying;
using SenseNet.Tests;
using System;

namespace SenseNet.Search.Tests
{
    [TestClass]
    public class SnTermTests : TestBase
    {
        [TestMethod, TestCategory("SnTerm")]
        public void SnTerm_Serialize_And_Parse()
        {
            void CheckSnTerm(SnTerm original)
            {
                var serialized = original.ToString();
                var deserialized = SnTerm.Parse(serialized);

                AssertSnTermsEqual(original, deserialized);
            }

            CheckSnTerm(new SnTerm("SNT1", "stringvalue"));
            //UNDONE: serializing an array SnTerm is not supported
            //CheckSnTerm(new SnTerm("SNT1", new[] {"a", "b", "c"}));
            CheckSnTerm(new SnTerm("SNT1", true));
            CheckSnTerm(new SnTerm("SNT1", 123));
            CheckSnTerm(new SnTerm("SNT1", 1234567890123));
            CheckSnTerm(new SnTerm("SNT1", 12.34));
            CheckSnTerm(new SnTerm("SNT1", new DateTime(2020, 04, 11, 10, 59, 57)));
        }

        [TestMethod, TestCategory("SnTerm")]
        public void SnTerm_Parse_Error()
        {
            void AssertError(string invalidTerm)
            {
                var thrown = false;

                try { SnTerm.Parse(invalidTerm); } 
                catch (ArgumentNullException) { thrown = true; }
                catch (InvalidOperationException) { thrown = true; }

                Assert.IsTrue(thrown, $"Exception NOT thrown for SnTerm: {invalidTerm}");
            }

            AssertError(null);
            AssertError(string.Empty);
            AssertError(":");
            AssertError("::");
            AssertError("A::");
            AssertError("A::B");
            AssertError("A:B:");
            AssertError("A:B:C:");
            AssertError(":A");
            AssertError(":A:B");
            AssertError(":A:B:C");
        }

        private static void AssertSnTermsEqual(SnTerm term1, SnTerm term2)
        {
            Assert.AreEqual(term1.Name, term2.Name);
            Assert.AreEqual(term1.StringValue, term2.StringValue);
            Assert.AreEqual(term1.BooleanValue, term2.BooleanValue);
            Assert.AreEqual(term1.IntegerValue, term2.IntegerValue);
            Assert.AreEqual(term1.LongValue, term2.LongValue);
            Assert.AreEqual(term1.SingleValue, term2.SingleValue);
            Assert.AreEqual(term1.DoubleValue, term2.DoubleValue);
            Assert.AreEqual(term1.DateTimeValue, term2.DateTimeValue);
            //Assert.AreEqual(term1.StringArrayValue, term2.StringArrayValue);
            Assert.AreEqual(term1.Type, term2.Type);
            Assert.AreEqual(term1.ValueAsString, term2.ValueAsString);
        }
    }
}
