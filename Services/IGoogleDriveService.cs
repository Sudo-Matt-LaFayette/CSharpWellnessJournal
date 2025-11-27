using System.Threading.Tasks;

namespace DailyCheckInJournal.Services
{
    public interface IGoogleDriveService
    {
        Task<bool> AuthenticateAsync();
        Task<bool> SyncToDriveAsync();
        Task<bool> SyncFromDriveAsync();
        bool IsAuthenticated { get; }
    }
}

