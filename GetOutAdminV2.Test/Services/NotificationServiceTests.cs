using System;
using GetOutAdminV2.Services;
using Xunit;

namespace GetOutAdminV2.Tests.Services
{
    public class NotificationServiceTests
    {
        [Fact]
        public void Notify_WithMessageAndType_TriggersEvent()
        {
            // Arrange
            string capturedMessage = null;
            NotificationType capturedType = NotificationType.Info;
            int capturedDuration = 0;

            NotificationService.OnNotificationRequested += (message, type, duration) => {
                capturedMessage = message;
                capturedType = type;
                capturedDuration = duration;
            };

            // Act
            NotificationService.Notify("Test message", NotificationType.Error, 10);

            // Assert
            Assert.Equal("Test message", capturedMessage);
            Assert.Equal(NotificationType.Error, capturedType);
            Assert.Equal(10, capturedDuration);
        }
    }
}