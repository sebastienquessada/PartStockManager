using PartStockManager.CoreLogic.Models;
using PartStockManager.Tests.Repositories;

namespace PartStockManager.Tests
{
    public class PartUnitTest
    {
        private StubPartRepository _stubPartRepository = new();

        [Fact]
        public void Create_New_Part()
        {
            Part expected = new Part("My New Part", "MYREF", 2, 1);

            _stubPartRepository.StubPart = expected;

            Assert.Equal(expected, _stubPartRepository.StubPart);
        }

        [Fact]
        public void Create_Part_With_Wrong_Name()
        {
            Assert.Throws<ArgumentException>(() => new Part("My failed creation", "", 10, 5));
        }

        [Fact]
        public void Create_Part_With_Wrong_Reference()
        {
            Assert.Throws<ArgumentException>(() => new Part("", "FAILED", 10, 5));
        }

        [Fact]
        public void Create_Part_With_Existing_Reference()
        {
            _stubPartRepository.StubPartsList = new List<Part>() {
                new Part("My New Part", "MYREF", 2, 1)
            };

            Part newPart = new Part("FAILED", "MYREF", 10, 5);

            var actual = _stubPartRepository.CreatePart(newPart);

            Assert.False(actual);
        }

        [Fact]
        public void Create_Part_With_Wrong_Quantity()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Part("My failed creation", "FAILED", -10, 5));
        }
        
        [Fact]
        public void Create_Part_With_Wrong_Threshold()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Part("My failed creation", "FAILED", 10, -2000));
        }

        [Fact]
        public void Modify_Existing_Part()
        {
            Part expected = new Part("ModifiedName", "SecondPart", 2, 15);

            _stubPartRepository.StubPartsList = new List<Part>() {
                new Part("MyFirstPart", "FirstPart", 5, 10),
                new Part("MySecondPart", "SecondPart", 2, 1),
                new Part("MyThirdPart", "ThirdPart", 50, 12)
            };

            var actual = _stubPartRepository.StubPartsList[1];

            _stubPartRepository.ModifyPart(actual.Reference, expected);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Deleting_Existing_Part()
        {
            _stubPartRepository.StubPartsList = new List<Part>() {
                new Part("MyFirstPart", "FirstPart", 5, 10),
                new Part("MySecondPart", "SecondPart", 2, 1),
                new Part("MyThirdPart", "ThirdPart", 50, 12)
            };

            var partToDelete = _stubPartRepository.StubPartsList[1];

            var actual = _stubPartRepository.DeletePart(partToDelete.Reference);

            Assert.True(actual);
        }

        [Fact]
        public void Failed_Part_Deletion()
        {
            _stubPartRepository.StubPartsList = new List<Part>() {
                new Part("MyFirstPart", "FirstPart", 5, 10),
                new Part("MySecondPart", "SecondPart", 2, 1),
                new Part("MyThirdPart", "ThirdPart", 50, 12)
            };

            var partToDelete = _stubPartRepository.StubPartsList[1];

            var actual = _stubPartRepository.DeletePart("FAILED");

            Assert.False(actual);
        }

        [Fact]
        public void Research_Parts_By_Name()
        {
            var expected = new List<Part>() {
                new Part("MyThirdPart", "ThirdPart", 50, 12)
            };

            _stubPartRepository.StubPartsList = new List<Part>() {
                new Part("MyFirstPart", "FirstPart", 5, 10),
                new Part("MySecondPart", "SecondPart", 2, 1),
                new Part("MyThirdPart", "ThirdPart", 50, 12)
            };

            var actual = _stubPartRepository.GetParts("third", "");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Research_Parts_By_Reference()
        {
            var expected = new List<Part>() {
                new Part("MyThirdPart", "ThirdPart", 50, 12)
            };

            _stubPartRepository.StubPartsList = new List<Part>() {
                new Part("MyFirstPart", "FirstPart", 5, 10),
                new Part("MySecondPart", "SecondPart", 2, 1),
                new Part("MyThirdPart", "ThirdPart", 50, 12)
            };

            var actual = _stubPartRepository.GetParts("", "third");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Research_With_Empty_Result()
        {
            var expected = new List<Part>() {
                
            };

            _stubPartRepository.StubPartsList = new List<Part>() {
                new Part("MyFirstPart", "FirstPart", 5, 10),
                new Part("MySecondPart", "SecondPart", 2, 1),
                new Part("MyThirdPart", "ThirdPart", 50, 12)
            };

            var actual = _stubPartRepository.GetParts("Empty result", "second");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Get_Only_Parts_With_Reached_Threshold()
        {
            var expected = new List<Part>() {
                new Part("MyFirstPart", "FirstPart", 5, 10)
            };

            _stubPartRepository.StubPartsList = new List<Part>() {
                expected[0],
                new Part("MySecondPart", "SecondPart", 2, 1),
                new Part("MyThirdPart", "ThirdPart", 50, 12)
            };

            var actual = _stubPartRepository.GetPartsWithReachedThreshold();

            Assert.Equal(expected, actual);
        }
    }
}
