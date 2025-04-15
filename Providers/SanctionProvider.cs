using GetOutAdminV2.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GetOutAdminV2.Providers
{
    public interface ISanctionProvider
    {
        ObservableCollection<SanctionsUser> GetSanctions();
        SanctionsUser GetSanctionById(long id);
        SanctionsUser GetActiveSanctionByUserId(long userId);
        void AddSanction(SanctionsUser sanction);
        void UpdateSanction(SanctionsUser sanction);
        void DeleteSanction(long id);
    }

    public class SanctionProvider : ISanctionProvider
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public SanctionProvider(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public ObservableCollection<SanctionsUser> GetSanctions()
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var sanctions = context.SanctionsUsers.Include(s => s.User).Include(s => s.TypeReportUsers).ToList();
                return new ObservableCollection<SanctionsUser>(sanctions);
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la récupération des sanctions", ex);
            }
        }

        public SanctionsUser GetSanctionById(long id)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                return context.SanctionsUsers
                    .Include(s => s.User)
                    .Include(s => s.TypeReportUsers)
                    .FirstOrDefault(s => s.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la récupération de la sanction par ID", ex);
            }
        }

        public SanctionsUser GetActiveSanctionByUserId(long userId)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var now = DateTime.Now;

                // Récupérer une sanction active (soit permanente, soit avec une date de fin dans le futur)
                return context.SanctionsUsers
                    .Include(s => s.User)
                    .Include(s => s.TypeReportUsers)
                    .Where(s => s.UserId == userId &&
                          (s.IsPermanent || s.EndAt > now) &&
                          s.Status == "active")
                    .OrderByDescending(s => s.CreatedAt)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la récupération des sanctions actives de l'utilisateur", ex);
            }
        }

        public void AddSanction(SanctionsUser sanction)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();

                // Définir les timestamps si non définis
                if (sanction.CreatedAt == null)
                    sanction.CreatedAt = DateTime.Now;

                if (sanction.UpdatedAt == null)
                    sanction.UpdatedAt = DateTime.Now;

                if (sanction.StartAt == null)
                    sanction.StartAt = DateTime.Now;

                context.SanctionsUsers.Add(sanction);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de l'ajout de la sanction", ex);
            }
        }

        public void UpdateSanction(SanctionsUser sanction)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();

                // Mettre à jour le timestamp
                sanction.UpdatedAt = DateTime.Now;

                context.Attach(sanction).State = EntityState.Modified;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la mise à jour de la sanction", ex);
            }
        }

        public void DeleteSanction(long id)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var sanctionToDelete = context.SanctionsUsers.FirstOrDefault(s => s.Id == id);
                if (sanctionToDelete != null)
                {
                    context.SanctionsUsers.Remove(sanctionToDelete);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la suppression de la sanction", ex);
            }
        }
    }
}