﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Centralite.Database
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class ManufacturingStoreEntities : DbContext
    {
        public ManufacturingStoreEntities()
            : base("name=ManufacturingStoreEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ASNEntry> ASNEntries { get; set; }
        public virtual DbSet<Board> Boards { get; set; }
        public virtual DbSet<BoardRevision> BoardRevisions { get; set; }
        public virtual DbSet<Bootloader> Bootloaders { get; set; }
        public virtual DbSet<CalibrationResult> CalibrationResults { get; set; }
        public virtual DbSet<CalibrationValue> CalibrationValues { get; set; }
        public virtual DbSet<ChipType> ChipTypes { get; set; }
        public virtual DbSet<CurrentDrawTest> CurrentDrawTests { get; set; }
        public virtual DbSet<DeletedDevice> DeletedDevices { get; set; }
        public virtual DbSet<DoryRemoteTestResult> DoryRemoteTestResults { get; set; }
        public virtual DbSet<EEPROM> EEPROMs { get; set; }
        public virtual DbSet<EuiList> EuiLists { get; set; }
        public virtual DbSet<Firmware> Firmwares { get; set; }
        public virtual DbSet<GarageDoorControllerTest> GarageDoorControllerTests { get; set; }
        public virtual DbSet<GarageDoorSensorTest> GarageDoorSensorTests { get; set; }
        public virtual DbSet<ImageType> ImageTypes { get; set; }
        public virtual DbSet<InCircuitTest> InCircuitTests { get; set; }
        public virtual DbSet<InsightAdapter> InsightAdapters { get; set; }
        public virtual DbSet<LoadTest> LoadTests { get; set; }
        public virtual DbSet<Migration> Migrations { get; set; }
        public virtual DbSet<NetworkColor> NetworkColors { get; set; }
        public virtual DbSet<PowerTest> PowerTests { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductionSite> ProductionSites { get; set; }
        public virtual DbSet<ProductionSiteProductJoin> ProductionSiteProductJoins { get; set; }
        public virtual DbSet<RangeTest> RangeTests { get; set; }
        public virtual DbSet<RangeTestChannel> RangeTestChannels { get; set; }
        public virtual DbSet<RangeTestFirmware> RangeTestFirmwares { get; set; }
        public virtual DbSet<RangeTestSetting> RangeTestSettings { get; set; }
        public virtual DbSet<Result> Results { get; set; }
        public virtual DbSet<SchemaVersion> SchemaVersions { get; set; }
        public virtual DbSet<SerialChange> SerialChanges { get; set; }
        public virtual DbSet<SerialNumber> SerialNumbers { get; set; }
        public virtual DbSet<StationSite> StationSites { get; set; }
        public virtual DbSet<Supervisor> Supervisors { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<TargetDevice> TargetDevices { get; set; }
        public virtual DbSet<Test> Tests { get; set; }
        public virtual DbSet<Tester> Testers { get; set; }
        public virtual DbSet<TestError> TestErrors { get; set; }
        public virtual DbSet<TestSession> TestSessions { get; set; }
        public virtual DbSet<TestStationMachine> TestStationMachines { get; set; }
        public virtual DbSet<TestType> TestTypes { get; set; }
        public virtual DbSet<UserSession> UserSessions { get; set; }
        public virtual DbSet<C_dbSerialList> C_dbSerialList { get; set; }
        public virtual DbSet<C_dbSerials> C_dbSerials { get; set; }
        public virtual DbSet<JabilErrorView> JabilErrorViews { get; set; }
        public virtual DbSet<LabelView> LabelViews { get; set; }
        public virtual DbSet<MacAddress> MacAddresses { get; set; }
    
        [DbFunction("ManufacturingStoreEntities", "iter_charlist_to_tbl")]
        public virtual IQueryable<iter_charlist_to_tbl_Result> iter_charlist_to_tbl(string list, string delimiter)
        {
            var listParameter = list != null ?
                new ObjectParameter("list", list) :
                new ObjectParameter("list", typeof(string));
    
            var delimiterParameter = delimiter != null ?
                new ObjectParameter("delimiter", delimiter) :
                new ObjectParameter("delimiter", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<iter_charlist_to_tbl_Result>("[ManufacturingStoreEntities].[iter_charlist_to_tbl](@list, @delimiter)", listParameter, delimiterParameter);
        }
    
        public virtual int CreateNewBoard(string name, Nullable<int> chipId, string mfgTokens)
        {
            var nameParameter = name != null ?
                new ObjectParameter("name", name) :
                new ObjectParameter("name", typeof(string));
    
            var chipIdParameter = chipId.HasValue ?
                new ObjectParameter("chipId", chipId) :
                new ObjectParameter("chipId", typeof(int));
    
            var mfgTokensParameter = mfgTokens != null ?
                new ObjectParameter("mfgTokens", mfgTokens) :
                new ObjectParameter("mfgTokens", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("CreateNewBoard", nameParameter, chipIdParameter, mfgTokensParameter);
        }
    
        public virtual int sp_alterdiagram(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var versionParameter = version.HasValue ?
                new ObjectParameter("version", version) :
                new ObjectParameter("version", typeof(int));
    
            var definitionParameter = definition != null ?
                new ObjectParameter("definition", definition) :
                new ObjectParameter("definition", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_alterdiagram", diagramnameParameter, owner_idParameter, versionParameter, definitionParameter);
        }
    
        public virtual int sp_CloseEvent(Nullable<int> id)
        {
            var idParameter = id.HasValue ?
                new ObjectParameter("id", id) :
                new ObjectParameter("id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_CloseEvent", idParameter);
        }
    
        public virtual int sp_CloseSession(Nullable<int> id)
        {
            var idParameter = id.HasValue ?
                new ObjectParameter("id", id) :
                new ObjectParameter("id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_CloseSession", idParameter);
        }
    
        public virtual int sp_CloseTest(Nullable<int> id)
        {
            var idParameter = id.HasValue ?
                new ObjectParameter("id", id) :
                new ObjectParameter("id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_CloseTest", idParameter);
        }
    
        public virtual int sp_creatediagram(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var versionParameter = version.HasValue ?
                new ObjectParameter("version", version) :
                new ObjectParameter("version", typeof(int));
    
            var definitionParameter = definition != null ?
                new ObjectParameter("definition", definition) :
                new ObjectParameter("definition", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_creatediagram", diagramnameParameter, owner_idParameter, versionParameter, definitionParameter);
        }
    
        public virtual int sp_dropdiagram(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_dropdiagram", diagramnameParameter, owner_idParameter);
        }
    
        public virtual ObjectResult<sp_helpdiagramdefinition_Result> sp_helpdiagramdefinition(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_helpdiagramdefinition_Result>("sp_helpdiagramdefinition", diagramnameParameter, owner_idParameter);
        }
    
        public virtual ObjectResult<sp_helpdiagrams_Result> sp_helpdiagrams(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_helpdiagrams_Result>("sp_helpdiagrams", diagramnameParameter, owner_idParameter);
        }
    
        public virtual int sp_OpenEvent(Nullable<int> test, Nullable<int> @event, ObjectParameter id)
        {
            var testParameter = test.HasValue ?
                new ObjectParameter("test", test) :
                new ObjectParameter("test", typeof(int));
    
            var eventParameter = @event.HasValue ?
                new ObjectParameter("event", @event) :
                new ObjectParameter("event", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_OpenEvent", testParameter, eventParameter, id);
        }
    
        public virtual int sp_OpenSession(Nullable<int> tester, ObjectParameter id)
        {
            var testerParameter = tester.HasValue ?
                new ObjectParameter("tester", tester) :
                new ObjectParameter("tester", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_OpenSession", testerParameter, id);
        }
    
        public virtual int sp_OpenTest(Nullable<int> session, Nullable<int> isa, Nullable<int> product, Nullable<int> revision, Nullable<int> firmware, ObjectParameter id)
        {
            var sessionParameter = session.HasValue ?
                new ObjectParameter("session", session) :
                new ObjectParameter("session", typeof(int));
    
            var isaParameter = isa.HasValue ?
                new ObjectParameter("isa", isa) :
                new ObjectParameter("isa", typeof(int));
    
            var productParameter = product.HasValue ?
                new ObjectParameter("product", product) :
                new ObjectParameter("product", typeof(int));
    
            var revisionParameter = revision.HasValue ?
                new ObjectParameter("revision", revision) :
                new ObjectParameter("revision", typeof(int));
    
            var firmwareParameter = firmware.HasValue ?
                new ObjectParameter("firmware", firmware) :
                new ObjectParameter("firmware", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_OpenTest", sessionParameter, isaParameter, productParameter, revisionParameter, firmwareParameter, id);
        }
    
        public virtual int sp_renamediagram(string diagramname, Nullable<int> owner_id, string new_diagramname)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var new_diagramnameParameter = new_diagramname != null ?
                new ObjectParameter("new_diagramname", new_diagramname) :
                new ObjectParameter("new_diagramname", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_renamediagram", diagramnameParameter, owner_idParameter, new_diagramnameParameter);
        }
    
        public virtual int sp_upgraddiagrams()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_upgraddiagrams");
        }
    }
}