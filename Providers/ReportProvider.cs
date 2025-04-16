using GetOutAdminV2.Enum;
using GetOutAdminV2.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetOutAdminV2.Providers
{
    public interface IReportProvider
    {
        ObservableCollection<ReportUser> GetReports();
        ReportUser GetReportById(long id);
        void AddReport(ReportUser report);
        void UpdateReport(ReportUser report);
        void DeleteReport(long id);
        ObservableCollection<ReportUser> GetReportPendingOrInvestigating();
    }

    public class ReportProvider : IReportProvider
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public ReportProvider(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public ObservableCollection<ReportUser> GetReports()
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                // Désactiver le chargement automatique pour éviter la récursion
                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                // Utiliser .AsNoTracking() pour éviter de suivre les entités
                var reports = context.ReportUsers
                                    .AsNoTracking()
                                    .ToList();

                return new ObservableCollection<ReportUser>(reports);
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la récupération des rapports", ex);
            }
        }

        public ObservableCollection<ReportUser> GetReportPendingOrInvestigating()
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var reports = context.ReportUsers.Where(r => r.Status != nameof(EReportStatus.resolved) || r.Status != nameof(EReportStatus.rejected)).ToList();
                return new ObservableCollection<ReportUser>(reports);
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la récupération des rapports non en attente", ex);
            }
        }

        public ReportUser GetReportById(long id)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                return context.ReportUsers.FirstOrDefault(r => r.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la récupération du rapport par ID", ex);
            }
        }

        public void AddReport(ReportUser report)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                context.ReportUsers.Add(report);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de l'ajout du rapport", ex);
            }
        }

        public void UpdateReport(ReportUser report)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();

                // Assurez-vous que le suivi est activé uniquement pour l'entité principale
                context.Entry(report).State = EntityState.Modified;

                // Détacher explicitement les entités liées pour éviter leur mise à jour
                if (report.TypeReport != null)
                    context.Entry(report.TypeReport).State = EntityState.Detached;
                if (report.Reporter != null)
                    context.Entry(report.Reporter).State = EntityState.Detached;
                if (report.ReportedUser != null)
                    context.Entry(report.ReportedUser).State = EntityState.Detached;
                if (report.ResolvedByNavigation != null)
                    context.Entry(report.ResolvedByNavigation).State = EntityState.Detached;

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la mise à jour du rapport", ex);
            }
        }

        public void DeleteReport(long id)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var reportToDelete = context.ReportUsers.FirstOrDefault(r => r.ReportedUserId == id);
                if (reportToDelete != null)
                {
                    context.ReportUsers.Remove(reportToDelete);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la suppression du rapport", ex);
            }
        }
    }
}
