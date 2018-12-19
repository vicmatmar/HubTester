using Centralite.Database;
using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;

namespace Centralite.Common.Interfaces
{
    public interface IManufacturingStoreRepository : IDisposable
    {
        DbSet<ASNEntry> ASNEntries { get; }
        DbSet<Board> Boards { get; }
        DbSet<BoardRevision> BoardRevisions { get; }
        DbSet<Bootloader> Bootloaders { get; }
        DbSet<CalibrationResult> CalibrationResults { get; }
        DbSet<CalibrationValue> CalibrationValues { get; }
        DbSet<ChipType> ChipTypes { get; }
        DbSet<CurrentDrawTest> CurrentDrawTests { get; }
        DbSet<DeletedDevice> DeletedDevices { get; }
        DbSet<DoryRemoteTestResult> DoryRemoteTestResults { get; }
        DbSet<EEPROM> EEPROMs { get; }
        DbSet<EuiList> EuiLists { get; }
        DbSet<Firmware> Firmwares { get; }
        DbSet<GarageDoorControllerTest> GarageDoorControllerTests { get; }
        DbSet<GarageDoorSensorTest> GarageDoorSensorTests { get; }
        DbSet<ImageType> ImageTypes { get; }
        DbSet<InCircuitTest> InCircuitTests { get; }
        DbSet<InsightAdapter> InsightAdapters { get; }
        DbSet<LoadTest> LoadTests { get; }
        DbSet<Migration> Migrations { get; }
        DbSet<NetworkColor> NetworkColors { get; }
        DbSet<PowerTest> PowerTests { get; }
        DbSet<Product> Products { get; }
        DbSet<ProductionSite> ProductionSites { get; }
        DbSet<ProductionSiteProductJoin> ProductionSiteProductJoins { get; }
        DbSet<RangeTest> RangeTests { get; }
        DbSet<RangeTestChannel> RangeTestChannels { get; }
        DbSet<RangeTestFirmware> RangeTestFirmwares { get; }
        DbSet<RangeTestSetting> RangeTestSettings { get; }
        DbSet<Result> Results { get; }
        DbSet<SchemaVersion> SchemaVersions { get; }
        DbSet<SerialChange> SerialChanges { get; }
        DbSet<SerialNumber> SerialNumbers { get; }
        DbSet<StationSite> StationSites { get; }
        DbSet<Supervisor> Supervisors { get; }
        DbSet<sysdiagram> sysdiagrams { get; }
        DbSet<TargetDevice> TargetDevices { get; }
        DbSet<Test> Tests { get; }
        DbSet<Tester> Testers { get; }
        DbSet<TestError> TestErrors { get; }
        DbSet<TestSession> TestSessions { get; }
        DbSet<TestStationMachine> TestStationMachines { get; }
        DbSet<TestType> TestTypes { get; }
        DbSet<UserSession> UserSessions { get; }
        DbSet<C_dbSerialList> C_dbSerialList { get; }
        DbSet<C_dbSerials> C_dbSerials { get; }
        DbSet<JabilErrorView> JabilErrorViews { get; }
        DbSet<LabelView> LabelViews { get; }

        [DbFunction("ManufacturingStoreEntities", "iter_charlist_to_tbl")]
        IQueryable<iter_charlist_to_tbl_Result> iter_charlist_to_tbl(string list, string delimiter);

        int CreateNewBoard(string name, Nullable<int> chipId, string mfgTokens);

        int RequestSerialNumbers(Nullable<bool> overrideExisting, Nullable<int> product, Nullable<int> tester, string euiListString, Nullable<int> supervisor);

        int sp_alterdiagram(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition);

        int sp_CloseEvent(Nullable<int> id);

        int sp_CloseSession(Nullable<int> id);

        int sp_CloseTest(Nullable<int> id);

        int sp_creatediagram(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition);

        int sp_dropdiagram(string diagramname, Nullable<int> owner_id);

        ObjectResult<sp_helpdiagramdefinition_Result> sp_helpdiagramdefinition(string diagramname, Nullable<int> owner_id);

        ObjectResult<sp_helpdiagrams_Result> sp_helpdiagrams(string diagramname, Nullable<int> owner_id);

        int sp_OpenEvent(Nullable<int> test, Nullable<int> @event, ObjectParameter id);

        int sp_OpenSession(Nullable<int> tester, ObjectParameter id);

        int sp_OpenTest(Nullable<int> session, Nullable<int> isa, Nullable<int> product, Nullable<int> revision, Nullable<int> firmware, ObjectParameter id);

        int sp_renamediagram(string diagramname, Nullable<int> owner_id, string new_diagramname);

        int sp_upgraddiagrams();

        int SaveChanges();
    }
}
