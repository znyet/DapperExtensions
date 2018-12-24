using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperExtensions
{
    internal class SqlServerCache
    {
        /// <summary>
        /// Cache
        /// </summary>
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, TableEntity> tableDict = new ConcurrentDictionary<RuntimeTypeHandle, TableEntity>();
        private static object _locker = new object();
        public static TableEntity GetTableEntity(Type t)
        {
            if (!tableDict.Keys.Contains(t.TypeHandle))
            {
                lock (_locker)
                {
                    if (!tableDict.Keys.Contains(t.TypeHandle))
                    {
                        TableEntity model = CommonUtil.CreateTableEntity(t);

                        string Fields = CommonUtil.GetFieldsStr(model.AllFieldList, "[", "]");
                        string FieldsAt = CommonUtil.GetFieldsAtStr(model.AllFieldList);
                        string FieldsEq = CommonUtil.GetFieldsEqStr(model.AllFieldList, "[", "]");

                        string FieldsExtKey = CommonUtil.GetFieldsStr(model.ExceptKeyFieldList, "[", "]");
                        string FieldsAtExtKey = CommonUtil.GetFieldsAtStr(model.ExceptKeyFieldList);
                        string FieldsEqExtKey = CommonUtil.GetFieldsEqStr(model.ExceptKeyFieldList, "[", "]");

                        model.AllFields = Fields;
                        model.AllFieldsAt = FieldsAt;
                        model.AllFieldsAtEq = FieldsEq;

                        model.AllFieldsExceptKey = FieldsExtKey;
                        model.AllFieldsAtExceptKey = FieldsAtExtKey;
                        model.AllFieldsAtEqExceptKey = FieldsEqExtKey;

                        if (!string.IsNullOrEmpty(model.KeyName) && model.IsIdentity) //有主键并且是自增
                        {
                            model.InsertSql = string.Format("INSERT INTO [{0}]({1})VALUES({2})", model.TableName, FieldsExtKey, FieldsAtExtKey);
                        }
                        else
                        {
                            model.InsertSql = string.Format("INSERT INTO [{0}]({1})VALUES({2})", model.TableName, Fields, FieldsAt);
                        }

                        model.InsertKeySql = string.Format("INSERT INTO [{0}]({1})VALUES({2})", model.TableName, Fields, FieldsAt);
                        //SET IDENTITY_INSERT [{0}] ON;
                        //SET IDENTITY_INSERT [{0}] OFF;

                        if (!string.IsNullOrEmpty(model.KeyName)) //含有主键
                        {
                            model.DeleteByIdSql = string.Format("DELETE FROM [{0}] WHERE [{1}]=@id", model.TableName, model.KeyName);
                            model.DeleteByIdsSql = string.Format("DELETE FROM [{0}] WHERE [{1}] IN @ids", model.TableName, model.KeyName);
                            model.GetByIdSql = string.Format("SELECT {0} FROM [{1}] WITH(NOLOCK) WHERE [{2}]=@id", Fields, model.TableName, model.KeyName);
                            model.GetByIdsSql = string.Format("SELECT {0} FROM [{1}] WITH(NOLOCK) WHERE [{2}] IN @ids", Fields, model.TableName, model.KeyName);
                            model.UpdateSql = string.Format("UPDATE [{0}] SET {1} WHERE [{2}]=@{2}", model.TableName, FieldsEqExtKey, model.KeyName);
                        }
                        model.DeleteAllSql = string.Format("DELETE FROM [{0}]", model.TableName);
                        model.GetAllSql = string.Format("SELECT {0} FROM [{1}] WITH(NOLOCK)", Fields, model.TableName);

                        tableDict[t.TypeHandle] = model;
                    }
                }
            }

            return tableDict[t.TypeHandle];
        }
    }
}
