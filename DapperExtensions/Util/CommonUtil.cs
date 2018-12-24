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
        public static string GetFieldsAtStr(IEnumerable<string> fieldList)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in fieldList)
            {
                sb.AppendFormat("@{0}", item);

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
        public static string GetFieldsEqStr(IEnumerable<string> fieldList, string leftChar, string rightChar)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in fieldList)
            {
                sb.AppendFormat("{0}{1}{2}=@{1}", leftChar, item, rightChar);

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
                throw new Exception("类未标注TableAttribute,请先标注");
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

        public static string CreateUpdateSql(TableEntity model, string updateFields, string leftChar, string rightChar)
        {
            string updateList = GetFieldsEqStr(updateFields.Split(',').ToList(), leftChar, rightChar);
            string sql = string.Format("UPDATE {0}{1}{2} SET {3} WHERE {0}{4}{2}=@{4}", leftChar, model.TableName, rightChar, updateList, model.KeyName);
            return sql;
        }
        public static string CreateUpdateByWhereSql(TableEntity model, string where, string updateFields, string leftChar, string rightChar)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("UPDATE {0}{1}{2} SET ", leftChar, model.TableName, rightChar);
            if (string.IsNullOrEmpty(updateFields)) //修改所有
            {
                if (!string.IsNullOrEmpty(model.KeyName)) //有主键
                    sb.AppendFormat(model.AllFieldsAtEqExceptKey);
                else
                    sb.AppendFormat(model.AllFieldsAtEq);
            }
            else
            {
                string updateList = GetFieldsEqStr(updateFields.Split(',').ToList(), leftChar, rightChar);
                sb.Append(updateList);
            }
            sb.Append(" ");
            sb.Append(where);

            return sb.ToString();
        }
    }
}
