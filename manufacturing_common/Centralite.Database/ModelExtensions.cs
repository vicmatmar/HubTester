using System.Linq;
using System.Windows.Media;
using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data;

namespace Centralite.Database
{
    public partial class ManufacturingStoreEntities
    {
        public virtual long GetNextMac(Nullable<long> StartBlock, Nullable<long> EndBlock)
        {
            var StartBlockParameter = StartBlock.HasValue ?
                new SqlParameter("StartBlock", StartBlock) :
                new SqlParameter("StartBlock", SqlDbType.BigInt);

            var EndBlockParameter = EndBlock.HasValue ?
                new SqlParameter("EndBlock", EndBlock) :
                new SqlParameter("EndBlock", SqlDbType.BigInt);
            
            var OutputParameter = new SqlParameter("NewMac", SqlDbType.BigInt);
            OutputParameter.Direction = ParameterDirection.Output;

            this.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction,
                "EXEC GetNextMac @StartBlock, @EndBlock, @NewMac OUTPUT",
                StartBlockParameter, EndBlockParameter, OutputParameter);

            return Convert.ToInt64(OutputParameter.Value);
        }

        public virtual int RequestSerialNumbers(Nullable<bool> overrideExisting, Nullable<int> product, Nullable<int> tester, string euiListString, Nullable<int> supervisor)
        {
            var overrideExistingParameter = overrideExisting.HasValue ?
                new SqlParameter("overrideExisting", overrideExisting) :
                new SqlParameter("overrideExisting", typeof(bool));

            var productParameter = product.HasValue ?
                new SqlParameter("product", product) :
                new SqlParameter("product", typeof(int));

            var testerParameter = tester.HasValue ?
                new SqlParameter("tester", tester) :
                new SqlParameter("tester", typeof(int));

            var euiListStringParameter = euiListString != null ?
                new SqlParameter("euiListString", euiListString) :
                new SqlParameter("euiListString", typeof(string));

            var supervisorParameter = supervisor.HasValue ?
                new SqlParameter("supervisor", supervisor) :
                new SqlParameter("supervisor", typeof(int?))
                {
                    SqlDbType = System.Data.SqlDbType.Int,
                    DbType = System.Data.DbType.Int32,
                    IsNullable = true,
                    Value = 0
                };

            return this.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction,
                "EXEC RequestSerialNumbers @overrideExisting, @product, @tester, @euiListString, @supervisor",
                overrideExistingParameter, productParameter, testerParameter, euiListStringParameter, supervisorParameter);
        }

        public bool IsAttached<T>(T entity) where T : class
        {
            return this.Set<T>().Local.Any(e => e == entity);
        }
    }

    public partial class EuiList
    {
        public override string ToString()
        {
            return this.EUI;
        }
    }

    partial class NetworkColor
    {
        public Color Color
        {
            get
            {
                return (Color)ColorConverter.ConvertFromString(Name);
            }
        }
    }

    partial class SerialNumber
    {
        public string PrettySerialNumber
        {
            get
            {
                return string.Format("{0}{1:D9}", Product.SerialNumberCode.ToUpper(), SerialNumber1);
            }
        }

        public override string ToString()
        {
            string output = "";
            output += base.ToString();
            output += "\n\tId: " + this.SerialNumberId;
            output += "\n\tSerial: " + this.SerialNumber1.ToString();
            output += "\n\tTester Id: " + this.TesterId.ToString();
            output += "\n\tProduct Id: " + this.ProductId.ToString();
            output += "\n\tDate created: " + this.CreateDate.ToString();
            return output;
        }
    }

    partial class BoardRevision
    {
        public char RevisionAsChar
        {
            get { return RevisionToChar(this.Revision); }
        }

        public static char RevisionToChar(int revision)
        {
            return (char)((revision - 1) + (int)'A');
        }

        public static int CharToRevision(char c)
        {
            return ((int)c + 1) - (int)'A';
        }

        public override string ToString()
        {
            char c = (char)(64 + this.Revision);
            return string.Format("Rev {0}", c);
        }
    }

    partial class Firmware
    {
        public string VersionString
        {
            get
            {
                return Firmware.FileVersionString(this.FileVersion);
            }
        }

        public static string FileVersionString(long FileVersion)
        {
            //format is 0xABCCDEFF
            //a = app major 
            //b = app minor
            //c = build
            //d = stack major
            //e = stack minor
            //f = stack build

            byte appMaj, appMin, appBuild;

            appMaj = (byte)((FileVersion >> 28) & 0x0F);
            appMin = (byte)((FileVersion >> 24) & 0x0F);
            appBuild = (byte)((FileVersion >> 16) & 0xFF);
            //stackMaj = (byte)((FileVersion >> 12) & 0x0F);
            //stackMin = (byte)((FileVersion >> 8) & 0x0F);
            //stackBuild = (byte)(FileVersion & 0xFF);

            return string.Format("v{0}.{1}.{2}", appMaj, appMin, appBuild);
        }
    }
}
