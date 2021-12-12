using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuperCipher;

namespace SuperCipherTest
{
    /// <summary>
    /// Summary description for CFBTest
    /// </summary>
    [TestClass]
    public class CFBTest
    {
        public CFBTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void StringTest()
        {
            //
            // TODO: Add test logic here
            //
            byte[] plain = Encoding.ASCII.GetBytes("kripto.grafi");
            CFB cfb = new CFB(plain,null,"123456789","098765432");
            cfb.encrypt();
            CollectionAssert.AreEqual(plain, cfb.decrypt());
        }

        [TestMethod]
        public void StringTest2()
        {
            //
            // TODO: Add test logic here
            //
            byte[] plain = Encoding.ASCII.GetBytes("kripto.grafi");
            CFB cfb = new CFB(plain, null, "1234567890123", "0987654321098");
            cfb.encrypt();
            CollectionAssert.AreEqual(plain, cfb.decrypt());
        }
    }
}
