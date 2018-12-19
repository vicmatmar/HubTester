using Centralite.Common.Interfaces;
using Centralite.Database;
using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;

namespace Centralite.Services
{
    public class ManufacturingStoreRepository : IManufacturingStoreRepository
    {
        private ManufacturingStoreEntities dbContext;

        public ManufacturingStoreRepository() : this(new ManufacturingStoreEntities()) { }

        public ManufacturingStoreRepository(ManufacturingStoreEntities dbContext)
        {
            this.dbContext = dbContext;
        }

        #region IManufacturingStoreRepository
        public DbSet<ASNEntry> ASNEntries
        {
            get
            {
                return dbContext.ASNEntries;
            }
        }

        public DbSet<BoardRevision> BoardRevisions
        {
            get
            {
                return dbContext.BoardRevisions;
            }
        }

        public DbSet<Board> Boards
        {
            get
            {
                return dbContext.Boards;
            }
        }

        public DbSet<Bootloader> Bootloaders
        {
            get
            {
                return dbContext.Bootloaders;
            }
        }

        public DbSet<CalibrationResult> CalibrationResults
        {
            get
            {
                return dbContext.CalibrationResults;
            }
        }

        public DbSet<CalibrationValue> CalibrationValues
        {
            get
            {
                return dbContext.CalibrationValues;
            }
        }

        public DbSet<ChipType> ChipTypes
        {
            get
            {
                return dbContext.ChipTypes;
            }
        }

        public DbSet<CurrentDrawTest> CurrentDrawTests
        {
            get
            {
                return dbContext.CurrentDrawTests;
            }
        }

        public DbSet<C_dbSerialList> C_dbSerialList
        {
            get
            {
                return dbContext.C_dbSerialList;
            }
        }

        public DbSet<C_dbSerials> C_dbSerials
        {
            get
            {
                return dbContext.C_dbSerials;
            }
        }

        public DbSet<DeletedDevice> DeletedDevices
        {
            get
            {
                return dbContext.DeletedDevices;
            }
        }

        public DbSet<DoryRemoteTestResult> DoryRemoteTestResults
        {
            get
            {
                return dbContext.DoryRemoteTestResults;
            }
        }

        public DbSet<EEPROM> EEPROMs
        {
            get
            {
                return dbContext.EEPROMs;
            }
        }

        public DbSet<EuiList> EuiLists
        {
            get
            {
                return dbContext.EuiLists;
            }
        }

        public DbSet<Firmware> Firmwares
        {
            get
            {
                return dbContext.Firmwares;
            }
        }

        public DbSet<GarageDoorControllerTest> GarageDoorControllerTests
        {
            get
            {
                return dbContext.GarageDoorControllerTests;
            }
        }

        public DbSet<GarageDoorSensorTest> GarageDoorSensorTests
        {
            get
            {
                return dbContext.GarageDoorSensorTests;
            }
        }

        public DbSet<ImageType> ImageTypes
        {
            get
            {
                return dbContext.ImageTypes;
            }
        }

        public DbSet<InCircuitTest> InCircuitTests
        {
            get
            {
                return dbContext.InCircuitTests;
            }
        }

        public DbSet<InsightAdapter> InsightAdapters
        {
            get
            {
                return dbContext.InsightAdapters;
            }
        }

        public DbSet<JabilErrorView> JabilErrorViews
        {
            get
            {
                return dbContext.JabilErrorViews;
            }
        }

        public DbSet<LabelView> LabelViews
        {
            get
            {
                return dbContext.LabelViews;
            }
        }

        public DbSet<LoadTest> LoadTests
        {
            get
            {
                return dbContext.LoadTests;
            }
        }

        public DbSet<Migration> Migrations
        {
            get
            {
                return dbContext.Migrations;
            }
        }

        public DbSet<NetworkColor> NetworkColors
        {
            get
            {
                return dbContext.NetworkColors;
            }
        }

        public DbSet<PowerTest> PowerTests
        {
            get
            {
                return dbContext.PowerTests;
            }
        }

        public DbSet<ProductionSiteProductJoin> ProductionSiteProductJoins
        {
            get
            {
                return dbContext.ProductionSiteProductJoins;
            }
        }

        public DbSet<ProductionSite> ProductionSites
        {
            get
            {
                return dbContext.ProductionSites;
            }
        }

        public DbSet<Product> Products
        {
            get
            {
                return dbContext.Products;
            }
        }

        public DbSet<RangeTestChannel> RangeTestChannels
        {
            get
            {
                return dbContext.RangeTestChannels;
            }
        }

        public DbSet<RangeTestFirmware> RangeTestFirmwares
        {
            get
            {
                return dbContext.RangeTestFirmwares;
            }
        }

        public DbSet<RangeTest> RangeTests
        {
            get
            {
                return dbContext.RangeTests;
            }
        }

        public DbSet<RangeTestSetting> RangeTestSettings
        {
            get
            {
                return dbContext.RangeTestSettings;
            }
        }

        public DbSet<Result> Results
        {
            get
            {
                return dbContext.Results;
            }
        }

        public DbSet<SchemaVersion> SchemaVersions
        {
            get
            {
                return dbContext.SchemaVersions;
            }
        }

        public DbSet<SerialChange> SerialChanges
        {
            get
            {
                return dbContext.SerialChanges;
            }
        }

        public DbSet<SerialNumber> SerialNumbers
        {
            get
            {
                return dbContext.SerialNumbers;
            }
        }

        public DbSet<StationSite> StationSites
        {
            get
            {
                return dbContext.StationSites;
            }
        }

        public DbSet<Supervisor> Supervisors
        {
            get
            {
                return dbContext.Supervisors;
            }
        }

        public DbSet<sysdiagram> sysdiagrams
        {
            get
            {
                return dbContext.sysdiagrams;
            }
        }

        public DbSet<TargetDevice> TargetDevices
        {
            get
            {
                return dbContext.TargetDevices;
            }
        }

        public DbSet<TestError> TestErrors
        {
            get
            {
                return dbContext.TestErrors;
            }
        }

        public DbSet<Tester> Testers
        {
            get
            {
                return dbContext.Testers;
            }
        }

        public DbSet<Test> Tests
        {
            get
            {
                return dbContext.Tests;
            }
        }

        public DbSet<TestSession> TestSessions
        {
            get
            {
                return dbContext.TestSessions;
            }
        }

        public DbSet<TestStationMachine> TestStationMachines
        {
            get
            {
                return dbContext.TestStationMachines;
            }
        }

        public DbSet<TestType> TestTypes
        {
            get
            {
                return dbContext.TestTypes;
            }
        }

        public DbSet<UserSession> UserSessions
        {
            get
            {
                return dbContext.UserSessions;
            }
        }

        public IQueryable<iter_charlist_to_tbl_Result> iter_charlist_to_tbl(string list, string delimiter)
        {
            return dbContext.iter_charlist_to_tbl(list, delimiter);
        }

        public int CreateNewBoard(string name, int? chipId, string mfgTokens)
        {
            return dbContext.CreateNewBoard(name, chipId, mfgTokens);
        }

        public int RequestSerialNumbers(bool? overrideExisting, int? product, int? tester, string euiListString, int? supervisor)
        {
            return dbContext.RequestSerialNumbers(overrideExisting, product, tester, euiListString, supervisor);
        }

        public int sp_alterdiagram(string diagramname, int? owner_id, int? version, byte[] definition)
        {
            return dbContext.sp_alterdiagram(diagramname, owner_id, version, definition);
        }

        public int sp_CloseEvent(int? id)
        {
            return dbContext.sp_CloseEvent(id);
        }

        public int sp_CloseSession(int? id)
        {
            return dbContext.sp_CloseSession(id);
        }

        public int sp_CloseTest(int? id)
        {
            return dbContext.sp_CloseTest(id);
        }

        public int sp_creatediagram(string diagramname, int? owner_id, int? version, byte[] definition)
        {
            return dbContext.sp_creatediagram(diagramname, owner_id, version, definition);
        }

        public int sp_dropdiagram(string diagramname, int? owner_id)
        {
            return dbContext.sp_dropdiagram(diagramname, owner_id);
        }

        public ObjectResult<sp_helpdiagramdefinition_Result> sp_helpdiagramdefinition(string diagramname, int? owner_id)
        {
            return dbContext.sp_helpdiagramdefinition(diagramname, owner_id);
        }

        public ObjectResult<sp_helpdiagrams_Result> sp_helpdiagrams(string diagramname, int? owner_id)
        {
            return dbContext.sp_helpdiagrams(diagramname, owner_id);
        }

        public int sp_OpenEvent(int? test, int? @event, ObjectParameter id)
        {
            return dbContext.sp_OpenEvent(test, @event, id);
        }

        public int sp_OpenSession(int? tester, ObjectParameter id)
        {
            return dbContext.sp_OpenSession(tester, id);
        }

        public int sp_OpenTest(int? session, int? isa, int? product, int? revision, int? firmware, ObjectParameter id)
        {
            return dbContext.sp_OpenTest(session, isa, product, revision, firmware, id);
        }

        public int sp_renamediagram(string diagramname, int? owner_id, string new_diagramname)
        {
            return dbContext.sp_renamediagram(diagramname, owner_id, new_diagramname);
        }

        public int sp_upgraddiagrams()
        {
            return dbContext.sp_upgraddiagrams();
        }

        public int SaveChanges()
        {
            return dbContext.SaveChanges();
        }
        #endregion

        #region IDisposable
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    dbContext.Dispose();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
