using GetOutAdminV2.Models;
using GetOutAdminV2.Providers;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace GetOutAdminV2.Managers
{
    public interface IReportManager
    {
        void GetAllReports();
        ReportUser GetReportById(long id);
        void AddReport(ReportUser report);
        void UpdateReport(ReportUser report);
        void DeleteReport(long id);
        ObservableCollection<ReportUser> ListOfReports { get; set; }
        ObservableCollection<ReportUser> GetReportPendingOrInvestigating();

        event Action ReportsUpdated;
    }

    public class ReportManager : IReportManager
    {
        readonly IReportProvider _reportProvider;
        public event Action ReportsUpdated;
        public ObservableCollection<ReportUser> ListOfReports { get; set; }

        public ReportManager(IReportProvider reportProvider)
        {
            _reportProvider = reportProvider;
            ListOfReports = new ObservableCollection<ReportUser>();
        }

        public void GetAllReports()
        {
            var reports = _reportProvider.GetReports().OrderBy(r => r.ReportedUserId);
            ListOfReports.Clear();
            foreach (var report in reports)
            {
                ListOfReports.Add(report);
            }
            ReportsUpdated?.Invoke();
        }

        public ObservableCollection<ReportUser> GetReportPendingOrInvestigating()
        {
            return _reportProvider.GetReportPendingOrInvestigating();
        }

        public ReportUser GetReportById(long id)
        {
            return _reportProvider.GetReportById(id);
        }

        public void AddReport(ReportUser report)
        {
            _reportProvider.AddReport(report);
            ReportsUpdated?.Invoke();
        }

        public void UpdateReport(ReportUser report)
        {
            _reportProvider.UpdateReport(report);
            ReportsUpdated?.Invoke();
        }

        public void DeleteReport(long id)
        {
            _reportProvider.DeleteReport(id);
            var reportToRemove = ListOfReports.FirstOrDefault(r => r.ReportedUserId == id);
            if (reportToRemove != null)
            {
                ListOfReports.Remove(reportToRemove);
            }
        }
    }
}
