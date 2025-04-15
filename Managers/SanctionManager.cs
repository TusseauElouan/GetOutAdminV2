using GetOutAdminV2.Models;
using GetOutAdminV2.Providers;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace GetOutAdminV2.Managers
{
    public interface ISanctionManager
    {
        void GetAllSanctions();
        SanctionsUser GetSanctionById(long id);
        SanctionsUser GetActiveSanctionByUserId(long userId);
        void AddSanction(SanctionsUser sanction);
        void UpdateSanction(SanctionsUser sanction);
        void DeleteSanction(long id);
        ObservableCollection<SanctionsUser> ListOfSanctions { get; set; }

        event Action SanctionsUpdated;
    }

    public class SanctionManager : ISanctionManager
    {
        readonly ISanctionProvider _sanctionProvider;
        public event Action SanctionsUpdated;
        public ObservableCollection<SanctionsUser> ListOfSanctions { get; set; }

        public SanctionManager(ISanctionProvider sanctionProvider)
        {
            _sanctionProvider = sanctionProvider;
            ListOfSanctions = new ObservableCollection<SanctionsUser>();
        }

        public void GetAllSanctions()
        {
            var sanctions = _sanctionProvider.GetSanctions().OrderByDescending(s => s.CreatedAt);
            ListOfSanctions.Clear();
            foreach (var sanction in sanctions)
            {
                ListOfSanctions.Add(sanction);
            }
            SanctionsUpdated?.Invoke();
        }

        public SanctionsUser GetSanctionById(long id)
        {
            return _sanctionProvider.GetSanctionById(id);
        }

        public SanctionsUser GetActiveSanctionByUserId(long userId)
        {
            return _sanctionProvider.GetActiveSanctionByUserId(userId);
        }

        public void AddSanction(SanctionsUser sanction)
        {
            _sanctionProvider.AddSanction(sanction);
            SanctionsUpdated?.Invoke();
        }

        public void UpdateSanction(SanctionsUser sanction)
        {
            _sanctionProvider.UpdateSanction(sanction);
            SanctionsUpdated?.Invoke();
        }

        public void DeleteSanction(long id)
        {
            _sanctionProvider.DeleteSanction(id);
            var sanctionToRemove = ListOfSanctions.FirstOrDefault(s => s.Id == id);
            if (sanctionToRemove != null)
            {
                ListOfSanctions.Remove(sanctionToRemove);
            }
            SanctionsUpdated?.Invoke();
        }
    }
}