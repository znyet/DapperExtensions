using System;
using System.Collections.Concurrent;

namespace DapperExtensions
{
    internal class SqlServerCache
    {
        /// <summary>
        /// Cache
        /// </summary>
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, TableEntity> tableDict = new ConcurrentDictionary<RuntimeTypeHandle, TableEntity>();
        private static readonly object _locker = new object();
        public static TableEntity GetTableEntity<T>()
        {
            Type t = typeof(T);
            RuntimeTypeHandle typeHandle = t.TypeHandle;
            if (!tableDict.Keys.Contains(typeHandle))
            {
                lock (_locker)
                {
                    if (!tableDict.Keys.Contains(typeHandle))
                    {
                        TableEntity table = CommonUtil.CreateTableEntity(t);
                        CommonUtil.InitTableForSqlServer(table);
                        tableDict[typeHandle] = table;
                    }
                }
            }

            return tableDict[typeHandle];
        }
    }
}
