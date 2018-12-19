using Centralite.Database;

namespace Centralite.Common.Interfaces
{
    public interface IUserSessionService
    {
        UserSession UserSession { get; }

        void CreateUserSession(int testerId);
        void CloseUserSession();

        void SaveOtaUpgrade(int ProductId, int FirmwareId, string Eui);
    }
}
