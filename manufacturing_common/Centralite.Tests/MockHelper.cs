using Moq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Centralite.Tests
{
    public static class MockHelper
    {
        public static DbSet<T> MockDbSet<T>(List<T> table) where T : class
        {
            var dbSet = new Mock<DbSet<T>>();
            dbSet.Setup(set => set.AsNoTracking()).Returns(dbSet.Object);
            dbSet.As<IQueryable<T>>().Setup(q => q.Provider).Returns(() => table.AsQueryable().Provider);
            dbSet.As<IQueryable<T>>().Setup(q => q.Expression).Returns(() => table.AsQueryable().Expression);
            dbSet.As<IQueryable<T>>().Setup(q => q.ElementType).Returns(() => table.AsQueryable().ElementType);
            dbSet.As<IQueryable<T>>().Setup(q => q.GetEnumerator()).Returns(() => table.AsQueryable().GetEnumerator());
            dbSet.Setup(set => set.Add(It.IsAny<T>())).Callback<T>(table.Add);
            dbSet.Setup(set => set.AddRange(It.IsAny<IEnumerable<T>>())).Callback<IEnumerable<T>>(table.AddRange);
            dbSet.Setup(set => set.Remove(It.IsAny<T>())).Callback<T>(t => table.Remove(t));
            dbSet.Setup(set => set.RemoveRange(It.IsAny<IEnumerable<T>>())).Callback<IEnumerable<T>>(ts =>
            {
                foreach (var t in ts) { table.Remove(t); }
            });
            return dbSet.Object;
        }
    }
}
