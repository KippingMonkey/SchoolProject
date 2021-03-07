using Microsoft.VisualStudio.TestTools.UnitTesting;
using WPFStore;
using System;
using System.Collections.Generic;
using System.Text;

namespace WPFStore.Tests
{
    [TestClass()]
    public class MainWindowTests
    {
        [TestMethod()]
        public void TestTest()
        {
            int result = MainWindow.AddNumbers(4, 6);
            Assert.AreEqual(10, result);
        }
       
        [TestMethod()]
        public void UpdateTotalAddProduct()
        {
            MainWindow.CartObject productAdded = new MainWindow.CartObject("PS5", "Console", 6000, 1, 6000);
            decimal currentTotal = 0;
            decimal result = MainWindow.CalculateTotalCartPriceWhenAddingProduct(productAdded, currentTotal);
            Assert.AreEqual(6000, result);
        }

        [TestMethod()]
        public void CurrentTotal500()
        {
            MainWindow.CartObject productAdded = new MainWindow.CartObject("PS5", "Console", 6000, 1, 6000);
            decimal currentTotal = 500;
            decimal result = MainWindow.CalculateTotalCartPriceWhenAddingProduct(productAdded, currentTotal);
            Assert.AreEqual(6500, result);
        }
    }
}