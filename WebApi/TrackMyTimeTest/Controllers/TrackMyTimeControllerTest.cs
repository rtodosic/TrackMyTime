using Moq;
using System;
using System.Collections.Generic;
using TrackMyTime.Controllers;
using TrackMyTime.Models;
using TrackMyTime.Services;
using Xunit;

namespace TrackMyTimeTest.Controllers
{
    public class TrackMyTimeControllerTest
    {
        [Fact]
        public void ShouldBeAbleToCreate()
        {
            // Arrange

            // Act
            var controller = new TrackMyTimeController(null);

            // Assert
            Assert.NotNull(controller);
        }

        [Fact]
        public void ShouldReturnItems()
        {
            // Arrange
            var mockRepo = new Mock<IMyTimeRepo>();
            mockRepo.Setup(r => r.GetItems(It.IsAny<string>(), It.IsAny<string>())).Returns(new List<TimeModel>
            {
                new TimeModel { Id = "id-1", UserId = "111", TimeGroup = "Test", Start = new DateTime(2020, 1, 1, 5, 10, 0), End = new DateTime(2020, 1, 1, 5, 12, 20), Notes="Test Note One" },
                new TimeModel { Id = "id-2", UserId = "111", TimeGroup = "Test", Start = new DateTime(2020, 1, 1, 6, 10, 0), End = new DateTime(2020, 1, 1, 7, 12, 20), Notes="Test Note Two" }
            });
            var controller = new TrackMyTimeController(mockRepo.Object);

            // Act
            var items = controller.GetByGroup("Test");

            // Assert
            Assert.NotNull(items);

            var enumerator = items.GetEnumerator();

            Assert.True(enumerator.MoveNext());
            Assert.Equal("id-1", enumerator.Current.Id);
            Assert.Equal("111", enumerator.Current.UserId);
            Assert.Equal("Test", enumerator.Current.TimeGroup);
            Assert.Equal(new DateTime(2020, 1, 1, 5, 10, 0), enumerator.Current.Start);
            Assert.Equal(new DateTime(2020, 1, 1, 5, 12, 20), enumerator.Current.End);
            Assert.Equal("Test Note One", enumerator.Current.Notes);

            Assert.True(enumerator.MoveNext());
            Assert.Equal("id-2", enumerator.Current.Id);
            Assert.Equal("111", enumerator.Current.UserId);
            Assert.Equal("Test", enumerator.Current.TimeGroup);
            Assert.Equal(new DateTime(2020, 1, 1, 6, 10, 0), enumerator.Current.Start);
            Assert.Equal(new DateTime(2020, 1, 1, 7, 12, 20), enumerator.Current.End);
            Assert.Equal("Test Note Two", enumerator.Current.Notes);
        }
    }
}
