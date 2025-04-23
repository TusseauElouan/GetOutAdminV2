using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GetOutAdminV2.Enum;
using GetOutAdminV2.Managers;
using GetOutAdminV2.Models;
using GetOutAdminV2.Providers;
using Moq;
using Xunit;

namespace GetOutAdminV2.Tests.Managers
{
    public class ReportManagerTests
    {
        [Fact]
        public void GetReportPendingOrInvestigating_ReturnsCorrectReports()
        {
            // Arrange
            var pendingReports = new ObservableCollection<ReportUser>
            {
                new ReportUser { Id = 1, Status = EReportStatus.pending.ToString() },
                new ReportUser { Id = 2, Status = EReportStatus.investigating.ToString() },
                new ReportUser { Id = 3, Status = EReportStatus.resolved.ToString() }
            };

            var mockReportProvider = new Mock<IReportProvider>();
            mockReportProvider.Setup(p => p.GetReportPendingOrInvestigating())
                .Returns(new ObservableCollection<ReportUser>(pendingReports.Where(r =>
                    r.Status == EReportStatus.pending.ToString() ||
                    r.Status == EReportStatus.investigating.ToString())));

            var reportManager = new ReportManager(mockReportProvider.Object);

            // Act
            var result = reportManager.GetReportPendingOrInvestigating();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.Id == 1);
            Assert.Contains(result, r => r.Id == 2);
            Assert.DoesNotContain(result, r => r.Id == 3);
        }

        [Fact]
        public void UpdateReport_ShouldCallProviderAndRaiseEvent()
        {
            // Arrange
            var report = new ReportUser { Id = 1, Status = EReportStatus.pending.ToString() };
            var eventRaised = false;

            var mockReportProvider = new Mock<IReportProvider>();
            mockReportProvider.Setup(p => p.UpdateReport(It.IsAny<ReportUser>())).Verifiable();

            var reportManager = new ReportManager(mockReportProvider.Object);
            reportManager.ReportsUpdated += () => eventRaised = true;

            // Act
            reportManager.UpdateReport(report);

            // Assert
            mockReportProvider.Verify(p => p.UpdateReport(report), Times.Once);
            Assert.True(eventRaised);
        }
    }
}