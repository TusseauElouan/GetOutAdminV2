using GetOutAdminV2.Models;
using GetOutAdminV2.Providers;
using System;
using System.Collections.ObjectModel;

namespace GetOutAdminV2.Managers
{
    public interface ITypeReportManager
    {
        void GetAllTypeReports();
        TypeReportUser GetTypeReportById(long id);
        ObservableCollection<TypeReportUser> ListOfTypeReports { get; set; }

        event Action TypeReportsUpdated;
    }

    public class TypeReportManager : ITypeReportManager
    {
        readonly ITypeReportProvider _typeReportProvider;
        public event Action TypeReportsUpdated;
        public ObservableCollection<TypeReportUser> ListOfTypeReports { get; set; }

        public TypeReportManager(ITypeReportProvider typeReportProvider)
        {
            _typeReportProvider = typeReportProvider;
            ListOfTypeReports = new ObservableCollection<TypeReportUser>();
        }

        public void GetAllTypeReports()
        {
            var typeReports = _typeReportProvider.GetTypeReports().OrderBy(tr => tr.Id);
            ListOfTypeReports.Clear();
            foreach (var typeReport in typeReports)
            {
                ListOfTypeReports.Add(typeReport);
            }
            TypeReportsUpdated?.Invoke();
        }

        public TypeReportUser GetTypeReportById(long id)
        {
            return _typeReportProvider.GetTypeReportById(id);
        }
    }
}
