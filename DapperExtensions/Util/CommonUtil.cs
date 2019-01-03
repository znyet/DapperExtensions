using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperExtensions
{
    internal class CommonUtil
    {
        /// <summary>
        /// 关键字处理[name] `name`
        /// 获取id,sex,name
        /// </summary>
        /// <param name="fieldList"></param>
        /// <param name="leftChar">左符号</param>
        /// <param name="rightChar">右符号</param>
        /// <returns></returns>
        public static string GetFieldsStr(IEnumerable<string> fieldList, string leftChar, string rightChar)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in fieldList)
            {
                sb.AppendFormat("{0}{1}{2}", leftChar, item, rightChar);

                if (item != fieldList.Last())
                {
                    sb.Append(",");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// //获取@id,@sex,@name
        /// </summary>
        /// <param name="fieldList"></param>
        /// <returns></returns>
        public static string GetFieldsAtStr(IEnumerable<string> fieldList, string symbol = "@") //oracle @换成 
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in fieldList)
            {
                sb.AppendFormat("{0}{1}", symbol, item);

                if (item != fieldList.Last())
                {
                    sb.Append(",");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 关键字处理[name] `name`
        /// 获取id=@id,name=@name
        /// </summary>
        /// <param name="fieldList"></param>
        /// <param name="leftChar">左符号</param>
        /// <param name="rightChar">右符号</param>
        /// <returns></returns>
        public static string GetFieldsEqStr(IEnumerable<string> fieldList, string leftChar, string rightChar, string symbol = "@") //oracle @换成 
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in fieldList)
            {
                sb.AppendFormat("{0}{1}{2}={3}{1}", leftChar, item, rightChar, symbol);

                if (item != fieldList.Last())
                {
                    sb.Append(",");
                }
            }
            return sb.ToString();
        }

        public static IEnumerable GetMultiExec(object param)
        {
            return (param is IEnumerable && !(param is string || param is IEnumerable<KeyValuePair<string, object>>)) ? (IEnumerable)param : null;
        }

        /// <summary>
        /// 判断输入参数是否有个数，用于in判断
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool ObjectIsEmpty(object param)
        {
            bool result = true;
            IEnumerable data = GetMultiExec(param);
            if (data != null)
            {
                foreach (var item in data)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        public static TableEntity CreateTableEntity(Type t)
        {
            TableAttribute table = t.GetCustomAttributes(false).FirstOrDefault(f => f is TableAttribute) as TableAttribute;
            if (table == null)
            {
                throw new Exception("Class " + t.Name + " is not labeled [TableAttribute], please label it first");
            }
            else
            {
                TableEntity model = new TableEntity();
                model.TableName = table.TableName;
                model.KeyName = table.KeyName;
                model.IsIdentity = table.IsIdentity;
                model.AllFieldList = new List<string>();
                model.ExceptKeyFieldList = new List<string>();


                var allproperties = t.GetProperties();

                foreach (var item in allproperties)
                {
                    var igore = item.GetCustomAttributes(false).FirstOrDefault(f => f is IgoreAttribute) as IgoreAttribute;
                    if (igore == null)
                    {
                        model.AllFieldList.Add(item.Name); //所有列

                        if (item.Name == model.KeyName)
                            model.KeyType = item.PropertyType;
                        else
                            model.ExceptKeyFieldList.Add(item.Name); //去除主键后的所有列
                    }
                }

                return model;
            }

        }

        [Obsolete]
        public static void InitTable(TableEntity table, string leftChar, string rightChar) //初始化增删改等语句
        {
            string Fields = GetFieldsStr(table.AllFieldList, leftChar, rightChar);
            string FieldsAt = GetFieldsAtStr(table.AllFieldList);
            string FieldsEq = GetFieldsEqStr(table.AllFieldList, leftChar, rightChar);

            string FieldsExtKey = GetFieldsStr(table.ExceptKeyFieldList, leftChar, rightChar);
            string FieldsAtExtKey = GetFieldsAtStr(table.ExceptKeyFieldList);
            string FieldsEqExtKey = GetFieldsEqStr(table.ExceptKeyFieldList, leftChar, rightChar);

            table.AllFields = Fields;
            table.AllFieldsAt = FieldsAt;
            table.AllFieldsAtEq = FieldsEq;

            table.AllFieldsExceptKey = FieldsExtKey;
            table.AllFieldsAtExceptKey = FieldsAtExtKey;
            table.AllFieldsAtEqExceptKey = FieldsEqExtKey;

            table.InsertSql = string.Format("INSERT INTO {3}{0}{4}({1})VALUES({2})", table.TableName, Fields, FieldsAt, leftChar, rightChar);
            if (!string.IsNullOrEmpty(table.KeyName)) //有主键
            {
                if (table.IsIdentity) //是自增
                {
                    table.InsertSql = string.Format("INSERT INTO {3}{0}{4}({1})VALUES({2})", table.TableName, FieldsExtKey, FieldsAtExtKey, leftChar, rightChar);
                }

                table.InsertIdentitySql = string.Format("INSERT INTO {3}{0}{4}({1})VALUES({2})", table.TableName, Fields, FieldsAt, leftChar, rightChar);

                table.DeleteByIdSql = string.Format("DELETE FROM {2}{0}{3} WHERE {2}{1}{3}=@id", table.TableName, table.KeyName, leftChar, rightChar);
                table.DeleteByIdsSql = string.Format("DELETE FROM {2}{0}{3} WHERE {2}{1}{3} IN @ids", table.TableName, table.KeyName, leftChar, rightChar);
                table.GetByIdSql = string.Format("SELECT {0} FROM {3}{1}{4} WHERE {3}{2}{4}=@id", Fields, table.TableName, table.KeyName, leftChar, rightChar);
                table.GetByIdsSql = string.Format("SELECT {0} FROM {3}{1}{4} WHERE {3}{2}{4} IN @ids", Fields, table.TableName, table.KeyName, leftChar, rightChar);
                table.UpdateSql = string.Format("UPDATE {3}{0}{4} SET {1} WHERE {3}{2}{4}=@{2}", table.TableName, FieldsEqExtKey, table.KeyName, leftChar, rightChar);
            }

            table.DeleteAllSql = string.Format("DELETE FROM {1}{0}{2} ", table.TableName, leftChar, rightChar);
            table.GetAllSql = string.Format("SELECT {0} FROM {2}{1}{3} ", Fields, table.TableName, leftChar, rightChar);
        }

        public static void InitTableForSqlServer(TableEntity table) //初始化增删改等语句sqlserver专用
        {
            string Fields = CommonUtil.GetFieldsStr(table.AllFieldList, "[", "]");
            string FieldsAt = CommonUtil.GetFieldsAtStr(table.AllFieldList);
            string FieldsEq = CommonUtil.GetFieldsEqStr(table.AllFieldList, "[", "]");

            string FieldsExtKey = CommonUtil.GetFieldsStr(table.ExceptKeyFieldList, "[", "]");
            string FieldsAtExtKey = CommonUtil.GetFieldsAtStr(table.ExceptKeyFieldList);
            string FieldsEqExtKey = CommonUtil.GetFieldsEqStr(table.ExceptKeyFieldList, "[", "]");

            table.AllFields = Fields;
            table.AllFieldsAt = FieldsAt;
            table.AllFieldsAtEq = FieldsEq;

            table.AllFieldsExceptKey = FieldsExtKey;
            table.AllFieldsAtExceptKey = FieldsAtExtKey;
            table.AllFieldsAtEqExceptKey = FieldsEqExtKey;

            table.InsertSql = string.Format("INSERT INTO [{0}]({1})VALUES({2})", table.TableName, Fields, FieldsAt);
            if (!string.IsNullOrEmpty(table.KeyName)) //有主键(table has primary key)
            {
                if (table.IsIdentity) //是自增 (and the parmary is identity)
                {
                    table.InsertSql = string.Format("INSERT INTO [{0}]({1})VALUES({2})", table.TableName, FieldsExtKey, FieldsAtExtKey);
                    table.InsertReturnIdSql = string.Format("INSERT INTO [{0}]({1})VALUES({2});SELECT @@IDENTITY;", table.TableName, FieldsExtKey, FieldsAtExtKey);
                }
                table.InsertIdentitySql = string.Format("SET IDENTITY_INSERT [{0}] ON;INSERT INTO [{0}]({1})VALUES({2});SET IDENTITY_INSERT [{0}] OFF", table.TableName, Fields, FieldsAt);
                table.DeleteByIdSql = string.Format("DELETE FROM [{0}] WHERE [{1}]=@id", table.TableName, table.KeyName);
                table.DeleteByIdsSql = string.Format("DELETE FROM [{0}] WHERE [{1}] IN @ids", table.TableName, table.KeyName);
                table.GetByIdSql = string.Format("SELECT {0} FROM [{1}] WITH(NOLOCK) WHERE [{2}]=@id", Fields, table.TableName, table.KeyName);
                table.GetByIdsSql = string.Format("SELECT {0} FROM [{1}] WITH(NOLOCK) WHERE [{2}] IN @ids", Fields, table.TableName, table.KeyName);
                table.UpdateSql = string.Format("UPDATE [{0}] SET {1} WHERE [{2}]=@{2}", table.TableName, FieldsEqExtKey, table.KeyName);
            }

            table.DeleteAllSql = string.Format("DELETE FROM [{0}] ", table.TableName);
            table.GetAllSql = string.Format("SELECT {0} FROM [{1}] WITH(NOLOCK) ", Fields, table.TableName);
        }

        public static void InitTableForMySql(TableEntity table) //初始化增删改等语句mysql专用
        {
            string Fields = GetFieldsStr(table.AllFieldList, "`", "`");
            string FieldsAt = GetFieldsAtStr(table.AllFieldList);
            string FieldsEq = GetFieldsEqStr(table.AllFieldList, "`", "`");

            string FieldsExtKey = GetFieldsStr(table.ExceptKeyFieldList, "`", "`");
            string FieldsAtExtKey = GetFieldsAtStr(table.ExceptKeyFieldList);
            string FieldsEqExtKey = GetFieldsEqStr(table.ExceptKeyFieldList, "`", "`");

            table.AllFields = Fields;
            table.AllFieldsAt = FieldsAt;
            table.AllFieldsAtEq = FieldsEq;

            table.AllFieldsExceptKey = FieldsExtKey;
            table.AllFieldsAtExceptKey = FieldsAtExtKey;
            table.AllFieldsAtEqExceptKey = FieldsEqExtKey;

            table.InsertSql = string.Format("INSERT INTO `{0}` ({1})VALUES({2})", table.TableName, Fields, FieldsAt);
            if (!string.IsNullOrEmpty(table.KeyName)) //有主键
            {
                if (table.IsIdentity) //是自增
                {
                    table.InsertSql = string.Format("INSERT INTO `{0}`({1})VALUES({2})", table.TableName, FieldsExtKey, FieldsAtExtKey);
                    table.InsertReturnIdSql = string.Format("INSERT INTO `{0}`({1})VALUES({2});SELECT @@IDENTITY;", table.TableName, FieldsExtKey, FieldsAtExtKey);
                }

                table.InsertIdentitySql = string.Format("INSERT INTO `{0}`({1})VALUES({2})", table.TableName, Fields, FieldsAt);

                table.DeleteByIdSql = string.Format("DELETE FROM `{0}` WHERE `{1}`=@id", table.TableName, table.KeyName);
                table.DeleteByIdsSql = string.Format("DELETE FROM `{0}` WHERE `{1}` IN @ids", table.TableName, table.KeyName);
                table.GetByIdSql = string.Format("SELECT {0} FROM `{1}` WHERE `{2}`=@id", Fields, table.TableName, table.KeyName);
                table.GetByIdsSql = string.Format("SELECT {0} FROM `{1}` WHERE `{2}` IN @ids", Fields, table.TableName, table.KeyName);
                table.UpdateSql = string.Format("UPDATE `{0}` SET {1} WHERE `{2}`=@{2}", table.TableName, FieldsEqExtKey, table.KeyName);
            }

            table.DeleteAllSql = string.Format("DELETE FROM `{0}` ", table.TableName);
            table.GetAllSql = string.Format("SELECT {0} FROM `{1}` ", Fields, table.TableName);
        }

        public static void InitTableForSqlite(TableEntity table) //初始化增删改等语句sqlite专用
        {
            string Fields = GetFieldsStr(table.AllFieldList, "[", "]");
            string FieldsAt = GetFieldsAtStr(table.AllFieldList);
            string FieldsEq = GetFieldsEqStr(table.AllFieldList, "[", "]");

            string FieldsExtKey = GetFieldsStr(table.ExceptKeyFieldList, "[", "]");
            string FieldsAtExtKey = GetFieldsAtStr(table.ExceptKeyFieldList);
            string FieldsEqExtKey = GetFieldsEqStr(table.ExceptKeyFieldList, "[", "]");

            table.AllFields = Fields;
            table.AllFieldsAt = FieldsAt;
            table.AllFieldsAtEq = FieldsEq;

            table.AllFieldsExceptKey = FieldsExtKey;
            table.AllFieldsAtExceptKey = FieldsAtExtKey;
            table.AllFieldsAtEqExceptKey = FieldsEqExtKey;

            table.InsertSql = string.Format("INSERT INTO [{0}]({1})VALUES({2})", table.TableName, Fields, FieldsAt);
            if (!string.IsNullOrEmpty(table.KeyName)) //有主键
            {
                if (table.IsIdentity) //是自增
                {
                    table.InsertSql = string.Format("INSERT INTO [{0}]({1})VALUES({2})", table.TableName, FieldsExtKey, FieldsAtExtKey);
                    table.InsertReturnIdSql = string.Format("INSERT INTO [{0}]({1})VALUES({2});SELECT last_insert_rowid();", table.TableName, FieldsExtKey, FieldsAtExtKey);
                }

                table.InsertIdentitySql = string.Format("INSERT INTO [{0}]({1})VALUES({2})", table.TableName, Fields, FieldsAt);

                table.DeleteByIdSql = string.Format("DELETE FROM [{0}] WHERE [{1}]=@id", table.TableName, table.KeyName);
                table.DeleteByIdsSql = string.Format("DELETE FROM [{0}] WHERE [{1}] IN @ids", table.TableName, table.KeyName);
                table.GetByIdSql = string.Format("SELECT {0} FROM [{1}] WHERE [{2}]=@id", Fields, table.TableName, table.KeyName);
                table.GetByIdsSql = string.Format("SELECT {0} FROM [{1}] WHERE [{2}] IN @ids", Fields, table.TableName, table.KeyName);
                table.UpdateSql = string.Format("UPDATE [{0}] SET {1} WHERE [{2}]=@{2}", table.TableName, FieldsEqExtKey, table.KeyName);
            }

            table.DeleteAllSql = string.Format("DELETE FROM [{0}] ", table.TableName);
            table.GetAllSql = string.Format("SELECT {0} FROM [{1}] ", Fields, table.TableName);
        }

        public static void InitTableForPostgresql(TableEntity table) //初始化增删改等语句postgresql专用
        {
            string Fields = GetFieldsStr(table.AllFieldList, "\"", "\"");
            string FieldsAt = GetFieldsAtStr(table.AllFieldList);
            string FieldsEq = GetFieldsEqStr(table.AllFieldList, "\"", "\"");

            string FieldsExtKey = GetFieldsStr(table.ExceptKeyFieldList, "\"", "\"");
            string FieldsAtExtKey = GetFieldsAtStr(table.ExceptKeyFieldList);
            string FieldsEqExtKey = GetFieldsEqStr(table.ExceptKeyFieldList, "\"", "\"");

            table.AllFields = Fields;
            table.AllFieldsAt = FieldsAt;
            table.AllFieldsAtEq = FieldsEq;

            table.AllFieldsExceptKey = FieldsExtKey;
            table.AllFieldsAtExceptKey = FieldsAtExtKey;
            table.AllFieldsAtEqExceptKey = FieldsEqExtKey;

            table.InsertSql = string.Format("INSERT INTO \"{0}\"({1})VALUES({2})", table.TableName, Fields, FieldsAt);
            if (!string.IsNullOrEmpty(table.KeyName)) //有主键
            {
                if (table.IsIdentity) //是自增
                {
                    table.InsertSql = string.Format("INSERT INTO \"{0}\"({1})VALUES({2})", table.TableName, FieldsExtKey, FieldsAtExtKey);
                    table.InsertReturnIdSql = string.Format("INSERT INTO \"{0}\"({1})VALUES({2})RETURNING \"{3}\"", table.TableName, FieldsExtKey, FieldsAtExtKey, table.KeyName);
                }

                table.InsertIdentitySql = string.Format("INSERT INTO \"{0}\"({1})VALUES({2})", table.TableName, Fields, FieldsAt);

                table.DeleteByIdSql = string.Format("DELETE FROM \"{0}\" WHERE \"{1}\"=@id", table.TableName, table.KeyName);
                table.DeleteByIdsSql = string.Format("DELETE FROM \"{0}\" WHERE \"{1}\"=ANY(@ids)", table.TableName, table.KeyName);
                table.GetByIdSql = string.Format("SELECT {0} FROM \"{1}\" WHERE \"{2}\"=@id", Fields, table.TableName, table.KeyName);
                table.GetByIdsSql = string.Format("SELECT {0} FROM \"{1}\" WHERE \"{2}\"=ANY(@ids)", Fields, table.TableName, table.KeyName);
                table.UpdateSql = string.Format("UPDATE \"{0}\" SET {1} WHERE \"{2}\"=@{2}", table.TableName, FieldsEqExtKey, table.KeyName);
            }

            table.DeleteAllSql = string.Format("DELETE FROM \"{0}\" ", table.TableName);
            table.GetAllSql = string.Format("SELECT {0} FROM \"{1}\" ", Fields, table.TableName);
        }

        public static void InitTableForOracle(TableEntity table) //初始化增删改等语句
        {
            string Fields = GetFieldsStr(table.AllFieldList, "\"", "\"");
            string FieldsAt = GetFieldsAtStr(table.AllFieldList, ":");
            string FieldsEq = GetFieldsEqStr(table.AllFieldList, "\"", "\"", ":");

            string FieldsExtKey = GetFieldsStr(table.ExceptKeyFieldList, "\"", "\"");
            string FieldsAtExtKey = GetFieldsAtStr(table.ExceptKeyFieldList, ":");
            string FieldsEqExtKey = GetFieldsEqStr(table.ExceptKeyFieldList, "\"", "\"", ":");

            table.AllFields = Fields;
            table.AllFieldsAt = FieldsAt;
            table.AllFieldsAtEq = FieldsEq;

            table.AllFieldsExceptKey = FieldsExtKey;
            table.AllFieldsAtExceptKey = FieldsAtExtKey;
            table.AllFieldsAtEqExceptKey = FieldsEqExtKey;

            table.InsertSql = string.Format("INSERT INTO \"{0}\"({1})VALUES({2})", table.TableName, Fields, FieldsAt);
            if (!string.IsNullOrEmpty(table.KeyName)) //有主键
            {
                table.InsertReturnIdSql = string.Format("INSERT INTO \"{0}\"({1})VALUES(```seq```.NEXTVAL,{2})", table.TableName, Fields, FieldsAtExtKey);
                if (table.IsIdentity) //是自增
                {
                    table.InsertSql = string.Format("INSERT INTO \"{0}\"({1})VALUES({2})", table.TableName, FieldsExtKey, FieldsAtExtKey);
                }

                table.InsertIdentitySql = string.Format("INSERT INTO \"{0}\"({1})VALUES({2})", table.TableName, Fields, FieldsAt);

                table.DeleteByIdSql = string.Format("DELETE FROM \"{0}\" WHERE \"{1}\"=:id", table.TableName, table.KeyName);
                table.DeleteByIdsSql = string.Format("DELETE FROM \"{0}\" WHERE \"{1}\" IN :ids", table.TableName, table.KeyName);
                table.GetByIdSql = string.Format("SELECT {0} FROM \"{1}\" WHERE \"{2}\"=:id", Fields, table.TableName, table.KeyName);
                table.GetByIdsSql = string.Format("SELECT {0} FROM \"{1}\" WHERE \"{2}\" IN :ids", Fields, table.TableName, table.KeyName);
                table.UpdateSql = string.Format("UPDATE \"{0}\" SET {1} WHERE \"{2}\"=:{2}", table.TableName, FieldsEqExtKey, table.KeyName);
            }

            table.DeleteAllSql = string.Format("DELETE FROM \"{0}\" ", table.TableName);
            table.GetAllSql = string.Format("SELECT {0} FROM \"{1}\" ", Fields, table.TableName);
        }

        public static void CheckTableKey(TableEntity table)
        {
            if (string.IsNullOrEmpty(table.KeyName))
            {
                string msg = "table [" + table.TableName + "] has no primary key";
                throw new Exception(msg);
            }

        }

        public static string CreateUpdateSql(TableEntity table, string updateFields, string leftChar, string rightChar, string symbol = "@") //oracle @换成 :
        {
            string updateList = GetFieldsEqStr(updateFields.Split(',').ToList(), leftChar, rightChar, symbol);
            return string.Format("UPDATE {0}{1}{2} SET {3} WHERE {0}{4}{2}={5}{4}", leftChar, table.TableName, rightChar, updateList, table.KeyName, symbol);
        }

        public static string CreateUpdateByWhereSql(TableEntity table, string where, string updateFields, string leftChar, string rightChar, string symbol = "@") //oracle @换成 
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("UPDATE {0}{1}{2} SET ", leftChar, table.TableName, rightChar);
            if (string.IsNullOrEmpty(updateFields)) //修改所有
            {
                if (!string.IsNullOrEmpty(table.KeyName)) //有主键
                    sb.AppendFormat(table.AllFieldsAtEqExceptKey);
                else
                    sb.AppendFormat(table.AllFieldsAtEq);
            }
            else
            {
                string updateList = GetFieldsEqStr(updateFields.Split(',').ToList(), leftChar, rightChar, symbol);
                sb.Append(updateList);
            }
            sb.Append(" ");
            sb.Append(where);

            return sb.ToString();
        }

    }
}
