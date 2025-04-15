using GetOutAdminV2.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GetOutAdminV2.Providers
{
    public interface ITypeReportProvider
    {
        ObservableCollection<TypeReportUser> GetTypeReports();
        TypeReportUser GetTypeReportById(long id);
    }

    public class TypeReportProvider : ITypeReportProvider
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public TypeReportProvider(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public ObservableCollection<TypeReportUser> GetTypeReports()
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var typeReports = context.TypeReportUsers.ToList();
                return new ObservableCollection<TypeReportUser>(typeReports);
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la récupération des types de rapports", ex);
            }
        }

        public TypeReportUser GetTypeReportById(long id)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                return context.TypeReportUsers.FirstOrDefault(tr => tr.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la récupération du type de rapport par ID", ex);
            }
        }
    }
}
