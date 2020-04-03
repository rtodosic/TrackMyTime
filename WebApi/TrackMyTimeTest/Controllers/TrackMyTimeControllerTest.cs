using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrackMyTime.Controllers;
using TrackMyTime.Models;
using TrackMyTime.Repositories;
using TrackMyTimeTest.Helpers;
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
            var controller = new TimeController(null, null);

            // Assert
            Assert.NotNull(controller);
        }

        [Fact]
        public async Task ShouldReturnItemsAsync()
        {
            // Arrange
            var mockRepo = new Mock<IMyTimeRepo>();
            mockRepo.Setup(r => r.GetTimesAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new List<TimeModel>
            {
                new TimeModel { Id = "id-1", UserId = "111", TimeGroup = "Test", Start = new DateTime(2020, 1, 1, 5, 10, 0), End = new DateTime(2020, 1, 1, 5, 12, 20), Notes="Test Note One" },
                new TimeModel { Id = "id-2", UserId = "111", TimeGroup = "Test", Start = new DateTime(2020, 1, 1, 6, 10, 0), End = new DateTime(2020, 1, 1, 7, 12, 20), Notes="Test Note Two" }
            });

            var logger = new SpyLogger<TimeController>();

            var controller = new TimeController(mockRepo.Object, logger);

            // Act
            var items = await controller.GetByGroupAsync("Test");

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
