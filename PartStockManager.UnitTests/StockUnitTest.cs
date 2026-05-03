using PartStockManager.CoreLogic.Models;
using PartStockManager.Tests.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace PartStockManager.Tests
{
    public class StockUnitTest
    {
        private StubStockRepository _stubStockRepository = new();

        [Fact]
        public void Stock_Inventory()
        {
            var expected = new List<Part>() {
                new Part("MyFirstPart", "FirstPart", 30, 10),
                new Part("MySecondPart", "SecondPart", 10, 1),
                new Part("MyThirdPart", "ThirdPart", 1, 12)
            };

            _stubStockRepository.StubPartsList = new List<Part>() {
                new Part("MyFirstPart", "FirstPart", 5, 10),
                new Part("MySecondPart", "SecondPart", 2, 1),
                new Part("MyThirdPart", "ThirdPart", 50, 12)
            };

            Dictionary<string, int> inventoryRequest = new Dictionary<string, int>()
            {
                { expected[0].Reference, expected[0].StockQuantity },
                { expected[1].Reference, expected[1].StockQuantity },
                { expected[2].Reference, expected[2].StockQuantity }

            };

            _stubStockRepository.Inventory(inventoryRequest);

            var actual = _stubStockRepository.StubPartsList;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Stock_Adjustment_Fail()
        {
            _stubStockRepository.StubPart = new Part("My Stub Part", "STUB", 10, 1);
            int negativeQuantity = -30;

            var expected = "Quantity can't be lower than zero";
            var actual = Assert.Throws<ArgumentOutOfRangeException>(() => _stubStockRepository.StubPart.AdjustStock(negativeQuantity));

            Assert.Contains(expected, actual.Message);
        }

        [Fact]
        public void Stock_Exit_Record_Success()
        {
            var entity = new Part("MyFirstPart", "FirstPart", 5, 10);

            _stubStockRepository.StubPartsList = new List<Part>() {
                entity,
                new Part("MySecondPart", "SecondPart", 2, 1),
                new Part("MyThirdPart", "ThirdPart", 50, 12)
            };

            int quantityToRemove = 3;
            int expected = entity.StockQuantity - quantityToRemove;

            var result = _stubStockRepository.RecordStockExit(entity.Reference, quantityToRemove);

            Assert.True(result);
        }

        [Fact]
        public void Stock_Exit_Record_Failed_No_Part_Found()
        {
            _stubStockRepository.StubPartsList = new List<Part>() {
                new Part("MyFirstPart", "FirstPart", 5, 10),
                new Part("MySecondPart", "SecondPart", 2, 1),
                new Part("MyThirdPart", "ThirdPart", 50, 12)
            };

            var entity = _stubStockRepository.StubPartsList[0];

            var result = _stubStockRepository.RecordStockExit("FAILED", 3);

            Assert.False(result);
        }

        [Fact]
        public void Stock_Exit_Record_Failed_Negative_Quantity()
        {
            _stubStockRepository.StubPartsList = new List<Part>() {
                new Part("MyFirstPart", "FirstPart", 5, 10),
                new Part("MySecondPart", "SecondPart", 2, 1),
                new Part("MyThirdPart", "ThirdPart", 50, 12)
            };

            var entity = _stubStockRepository.StubPartsList[0];

            var result = _stubStockRepository.RecordStockExit(entity.Reference, -50);

            Assert.False(result);
        }

        [Fact]
        public void Stock_Exit_Record_Failed_Exit_Quantity_Too_High()
        {
            _stubStockRepository.StubPartsList = new List<Part>() {
                new Part("MyFirstPart", "FirstPart", 5, 10),
                new Part("MySecondPart", "SecondPart", 2, 1),
                new Part("MyThirdPart", "ThirdPart", 50, 12)
            };

            var entity = _stubStockRepository.StubPartsList[0];

            var result = _stubStockRepository.RecordStockExit(entity.Reference, 50);

            Assert.False(result);
        }

        [Fact]
        public void Stock_Entry_Record_Success()
        {
            var entity = new Part("MyFirstPart", "FirstPart", 5, 10);
            
            _stubStockRepository.StubPartsList = new List<Part>() {
                entity,
                new Part("MySecondPart", "SecondPart", 2, 1),
                new Part("MyThirdPart", "ThirdPart", 50, 12)
            };

            var quantityToAdd = 3;
            var expected = _stubStockRepository.StubPartsList[0].StockQuantity + quantityToAdd;

            var result = _stubStockRepository.RecordStockEntry(entity.Reference, 3);

            Assert.True(result);

            var actual = _stubStockRepository.StubPartsList[0].StockQuantity;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Stock_Entry_Failed_Record_No_Part_Found()
        {
            _stubStockRepository.StubPartsList = new List<Part>() {
                new Part("MyFirstPart", "FirstPart", 5, 10),
                new Part("MySecondPart", "SecondPart", 2, 1),
                new Part("MyThirdPart", "ThirdPart", 50, 12)
            };

            var entity = _stubStockRepository.StubPartsList[0];

            var result = _stubStockRepository.RecordStockEntry("FAILED", 3);

            Assert.False(result);
        }

        [Fact]
        public void Stock_Entry_Failed_Record_Negative_Quantity()
        {
            _stubStockRepository.StubPartsList = new List<Part>() {
                new Part("MyFirstPart", "FirstPart", 5, 10),
                new Part("MySecondPart", "SecondPart", 2, 1),
                new Part("MyThirdPart", "ThirdPart", 50, 12)
            };

            var entity = _stubStockRepository.StubPartsList[0];

            var result = _stubStockRepository.RecordStockEntry(entity.Reference, -50);

            Assert.False(result);
        }
    }
}
