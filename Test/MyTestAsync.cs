using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using NUnit.Framework;
using DapperExtensions;
using Newtonsoft.Json;

namespace Test
{
    [TestFixture]
    public class MyTestAsync
    {
            
        int Second = DateTime.Now.Second;

        [Test]
        public void GetDataTable()
        {
            string sql = "SELECT * FROM People";
            using (var conn = DbHelper.GetConn())
            {
                DataTable dt = conn.GetDataTableAsync(sql).Result;
                Assert.Pass(dt.Rows.Count.ToString());
            }

        }

        [Test]
        public void GetDataSet()
        {
            string sql = "SELECT * FROM People;SELECT * FROM Student;SELECT * FROM School";
            using (var conn = DbHelper.GetConn())
            {
                DataSet ds = conn.GetDataSetAsync(sql).Result;
                Assert.Pass(ds.Tables.Count.ToString());
            }
        }

        [Test]
        public void GetSchemaTable()
        {
            using (var conn = DbHelper.GetConn())
            {
                DataTable dt = conn.GetSchemaTableAsync<PeopleTable>().Result;
                Assert.Pass(dt.Columns[1].ColumnName);
            }
        }

        [Test]
        public void BulkCopy()
        {
            using (var conn = DbHelper.GetConn())
            {
                string sql = "SELECT Top 1 * FROM School";
                DataTable dt = conn.GetDataTableAsync(sql).Result;
                foreach (DataRow row in dt.Rows)
                {
                    row["Name"] = "钱七" + Second;
                }
                string msg = conn.BulkCopyAsync(dt, "School", null).Result;
                Assert.Pass(msg);
            }
        }

        [Test]
        public void BulkCopy_T()
        {
            using (var conn = DbHelper.GetConn())
            {
                string sql = "SELECT Top 1 * FROM School";
                DataTable dt = conn.GetDataTableAsync(sql).Result;
                foreach (DataRow row in dt.Rows)
                {
                    row["Name"] = "钱七" + Second;
                }
                string msg = conn.BulkCopyAsync<SchoolTable>(dt).Result;
                Assert.Pass(msg);

            }
        }

        [Test]
        public void BulkUpdate()
        {
            using (var conn = DbHelper.GetConn())
            {
                string sql = "SELECT * FROM School";
                DataTable dt = conn.GetDataTableAsync(sql).Result;
                foreach (DataRow row in dt.Rows)
                {
                    row["Name"] = "钱七" + Second;
                }
                string msg = conn.BulkUpdateAsync(dt, "School").Result;
                Assert.Pass(msg);
            }
        }

        [Test]
        public void BulkUpdate_T()
        {
            using (var conn = DbHelper.GetConn())
            {
                string sql = "SELECT * FROM School";
                DataTable dt = conn.GetDataTableAsync(sql).Result;
                foreach (DataRow row in dt.Rows)
                {
                    row["Name"] = "钱七" + Second;
                }
                string msg = conn.BulkUpdateAsync<SchoolTable>(dt).Result;
                Assert.Pass(msg);
            }
        }

        [Test]
        public void Insert()
        {
            using (var conn = DbHelper.GetConn())
            {
                PeopleTable people = new PeopleTable();
                people.Name = "李四" + Second;
                people.Sex = Second;
                int effect = conn.InsertAsync(people).Result;

                StudentTable student = new StudentTable();
                student.Id = ObjectId.GenerateNewId().ToString();
                student.Name = "王五" + Second;
                student.Sex = Second;
                int effect2 = conn.InsertAsync(student).Result;

                string msg = string.Format("\n effect:{0} \n effsct2:{1}", effect, effect2);
                Assert.Pass(msg);

            }
        }

        [Test]
        public void InsertReturnId()
        {
            using (var conn = DbHelper.GetConn())
            {
                PeopleTable people = new PeopleTable();
                people.Name = "李四" + Second;
                people.Sex = Second;
                var id = conn.InsertReturnIdAsync(people).Result;
                Assert.Pass(id.ToString());
            }

        }

        [Test]
        public void InsertIdentity()
        {
            using (var conn = DbHelper.GetConn())
            {
                PeopleTable people = new PeopleTable();
                people.Id = 1;
                people.Name = "李四" + Second;
                people.Sex = Second;
                int effect = conn.InsertIdentityAsync(people).Result;
                Assert.Pass(effect.ToString());
            }
        }

        [Test]
        public void Update()
        {
            using (var conn = DbHelper.GetConn())
            {
                PeopleTable people = new PeopleTable();
                people.Id = 1;
                people.Name = "李四" + Second;
                people.Sex = Second;
                int effect = conn.UpdateAsync(people).Result;
                //int effect = conn.Update(people, "Name"); //update people set Name=@Name where Id=@Id
                Assert.Pass(effect.ToString());
            }
        }

        [Test]
        public void UpdateByWhere()
        {
            using (var conn = DbHelper.GetConn())
            {
                PeopleTable people = new PeopleTable();
                people.Id = 1;
                people.Name = "钱七" + Second;
                people.Sex = 47;

                int effect = conn.UpdateByWhereAsync(people, "WHERE Sex=@Sex", "Name").Result; //update people set Name=@Name WHERE Sex=@Sex
                Assert.Pass(effect.ToString());
            }

        }

        [Test]
        public void InsertOrUpdate()
        {
            using (var conn = DbHelper.GetConn())
            {
                PeopleTable people = new PeopleTable();
                people.Id = 1;
                people.Name = "王五" + Second;
                people.Sex = Second;

                PeopleTable p2 = new PeopleTable();
                p2.Id = 2;
                p2.Name = "王五" + Second;
                p2.Sex = Second;

                int effect = conn.InsertOrUpdateAsync(people).Result;
                //int effect = conn.InsertOrUpdateAsync(people, "Name").Result; //only update Name
                effect += conn.InsertOrUpdateAsync(p2).Result;

                Assert.Pass(effect.ToString());
            }
        }

        [Test]
        public void InsertIdentityOrUpdate()
        {
            using (var conn = DbHelper.GetConn())
            {
                PeopleTable people = new PeopleTable();
                people.Id = 1;
                people.Name = "王五" + Second;
                people.Sex = Second;
                int effect = conn.InsertIdentityOrUpdateAsync(people).Result;

                PeopleTable p2 = new PeopleTable();
                p2.Id = 52;
                p2.Name = "小罗" + Second;
                p2.Sex = Second;
                effect += conn.InsertIdentityOrUpdateAsync(p2).Result;

                Assert.Pass(effect.ToString());
            }
        }

        [Test]
        public void Delete()
        {
            using (var conn = DbHelper.GetConn())
            {
                int effecf = conn.DeleteAsync<PeopleTable>(1).Result;
                Assert.Pass(effecf.ToString());
            }
        }

        [Test]
        public void DeleteByIds()
        {
            using (var conn = DbHelper.GetConn())
            {
                List<int> ids = new List<int>();
                ids.Add(15);
                ids.Add(18);
                ids.Add(19);
                ids.Add(28);
                int effect = conn.DeleteByIdsAsync<SchoolTable>(ids).Result;

                List<string> ids2 = new List<string>() 
                {
                    "5c2c5e54922fdc2cc86bb7b6",
                    "5c2c5e4f922fdc1af8cdfae9"
                };

                effect += conn.DeleteByIdsAsync<StudentTable>(ids2).Result;

                Assert.Pass(effect.ToString());
            }


        }

        [Test]
        public void DeleteByWhere()
        {
            using (var conn = DbHelper.GetConn())
            {
                string where = "WHERE Name=@Name";
                int effect = conn.DeleteByWhereAsync<SchoolTable>(where, new { Name = "2" }).Result;
                Assert.Pass(effect.ToString());
            }
        }

        [Test]
        public void DeleteAll()
        {
            using (var conn = DbHelper.GetConn())
            {
                int effect = conn.DeleteAllAsync<SchoolTable>().Result;
                Assert.Pass(effect.ToString());
            }
        }

        [Test]
        public void GetSequenceCurrent() { }

        [Test]
        public void GetSequenceNext() { }

        [Test]
        public void GetTotal()
        {
            using (var conn = DbHelper.GetConn())
            {
                long total = conn.GetTotalAsync<PeopleTable>().Result;
                total += conn.GetTotalAsync<PeopleTable>("WHERE Id=@Id", new { id = 1 }).Result;
                Assert.Pass(total.ToString());
            }
        }

        [Test]
        public void GetAll()
        {
            using (var conn = DbHelper.GetConn())
            {
                IEnumerable<PeopleTable> data = conn.GetAllAsync<PeopleTable>().Result;
                //IEnumerable<PeopleTable> data = conn.GetAll<PeopleTable>("Name"); //only return [name] field split with , "Name,Sex"
                //IEnumerable<PeopleTable> data = conn.GetAll<PeopleTable>(null,"ORDER BY Id DESC");
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetAllDynamic()
        {
            using (var conn = DbHelper.GetConn())
            {
                IEnumerable<dynamic> data = conn.GetAllDynamicAsync<PeopleTable>("Id,Name").Result;
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetById()
        {
            using (var conn = DbHelper.GetConn())
            {
                PeopleTable people = conn.GetByIdAsync<PeopleTable>(1).Result;
                //PeopleTable people = conn.GetById<PeopleTable>(1,"Name"); //only return [name] field
                string json = JsonConvert.SerializeObject(people);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetByIdDynamic()
        {
            using (var conn = DbHelper.GetConn())
            {
                dynamic people = conn.GetByIdDynamicAsync<PeopleTable>(1).Result;
                string json = JsonConvert.SerializeObject(people);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetByIds()
        {

            using (var conn = DbHelper.GetConn())
            {
                List<int> ids = new List<int>() { 1, 2, 3 };
                IEnumerable<PeopleTable> data = conn.GetByIdsAsync<PeopleTable>(ids).Result; //select * from people where id in @ids
                //IEnumerable<PeopleTable> data = conn.GetByIds<PeopleTable>(ids, "Name"); //select name from people where id in @ids
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetByIdsDynamic()
        {
            using (var conn = DbHelper.GetConn())
            {
                int[] ids = new int[] { 1, 2, 3 };
                IEnumerable<dynamic> data = conn.GetByIdsDynamicAsync<PeopleTable>(ids).Result;
                //IEnumerable<dynamic> data = conn.GetByIdsDynamic<PeopleTable>(ids, "Name"); //only return [name] field
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetByIdsWithField()
        {
            using (var conn = DbHelper.GetConn())
            {
                int[] ids = new int[] { 18, 1, 19 };
                IEnumerable<PeopleTable> data = conn.GetByIdsWithFieldAsync<PeopleTable>(ids, "Sex").Result; //select * from people where Sex in @ids
                //IEnumerable<PeopleTable> data = conn.GetByIdsWithField<PeopleTable>(ids, "Sex","Name"); //only return [name] field
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetByIdsWithFieldDynamic()
        {
            using (var conn = DbHelper.GetConn())
            {
                int[] ids = new int[] { 18, 1, 19 };
                IEnumerable<dynamic> data = conn.GetByIdsWithFieldDynamicAsync<PeopleTable>(ids, "Sex").Result; //select * from people where sex in @ids
                //IEnumerable<dynamic> data = conn.GetByIdsWithFieldDynamic<PeopleTable>(ids, "Sex","Name,Sex"); //only return [name] field
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetByWhere()
        {
            using (var conn = DbHelper.GetConn())
            {
                IEnumerable<PeopleTable> data = conn.GetByWhereAsync<PeopleTable>("WHERE Id<@Id", new { Id = 3 }).Result;
                //IEnumerable<PeopleTable> data = conn.GetByWhere<PeopleTable>("WHERE Id<@Id", new { Id = 3 }, "Name");//only return [name] field
                //IEnumerable<PeopleTable> data = conn.GetByWhere<PeopleTable>("WHERE Id<@Id", new { Id = 3 }, null,"ORDER BY Id DESC"); // order by
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetByWhereDynamic()
        {
            using (var conn = DbHelper.GetConn())
            {
                IEnumerable<dynamic> data = conn.GetByWhereDynamicAsync<PeopleTable>("WHERE Id<@Id", new { Id = 3 }).Result;
                //IEnumerable<dynamic> data = conn.GetByWhereDynamic<PeopleTable>("WHERE Id<@Id", new { Id = 3 }, "Name,Sex");//only return [name] field
                //IEnumerable<dynamic> data = conn.GetByWhereDynamic<PeopleTable>("WHERE Id<@Id", new { Id = 3 }, null, "ORDER BY Id DESC"); // order by
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetByWhereFirst()
        {
            using (var conn = DbHelper.GetConn())
            {
                PeopleTable people = conn.GetByWhereFirstAsync<PeopleTable>("WHERE Id<@Id", new { Id = 4 }).Result;
                //PeopleTable people = conn.GetByWhereFirst<PeopleTable>("WHERE Id<@Id", new { Id = 4 },"Name"); //only return [Name] field
                string json = JsonConvert.SerializeObject(people);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetByWhereFirstDynamic()
        {
            using (var conn = DbHelper.GetConn())
            {
                dynamic people = conn.GetByWhereFirstDynamicAsync<PeopleTable>("WHERE Id<@Id", new { Id = 4 }).Result;
                //dynamic people = conn.GetByWhereFirstDynamic<PeopleTable>("WHERE Id<@Id", new { Id = 4 },"Name"); //only return [Name] field
                string json = JsonConvert.SerializeObject(people);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetBySkipTake()
        {
            using (var conn = DbHelper.GetConn())
            {
                IEnumerable<PeopleTable> data = conn.GetBySkipTakeAsync<PeopleTable>(0, 5).Result;
                //IEnumerable<PeopleTable> data = conn.GetBySkipTake<PeopleTable>(0, 10, "WHERE Id<@Id", new { Id = 5 }); //where
                //IEnumerable<PeopleTable> data = conn.GetBySkipTake<PeopleTable>(0, 2,returnFields:"Name"); //only return field [name]
                //IEnumerable<PeopleTable> data = conn.GetBySkipTake<PeopleTable>(0, 10, orderBy:"ORDER BY Id DESC"); //order by
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetBySkipTakeDynamic()
        {
            using (var conn = DbHelper.GetConn())
            {
                IEnumerable<dynamic> data = conn.GetBySkipTakeDynamicAsync<PeopleTable>(0, 2).Result;
                //IEnumerable<dynamic> data = conn.GetBySkipTakeDynamic<PeopleTable>(0, 10, "WHERE Id<@Id", new { Id = 5 }); //where
                //IEnumerable<dynamic> data = conn.GetBySkipTakeDynamic<PeopleTable>(0, 2, returnFields: "Name"); //only return field [name]
                //IEnumerable<dynamic> data = conn.GetBySkipTakeDynamic<PeopleTable>(0, 10, orderBy: "ORDER BY Id DESC"); //order by
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetByPageIndex()
        {
            using (var conn = DbHelper.GetConn())
            {
                IEnumerable<PeopleTable> data = conn.GetByPageIndexAsync<PeopleTable>(2, 2).Result;
                //IEnumerable<PeopleTable> data = conn.GetByPageIndex<PeopleTable>(1, 10, "WHERE Id<@Id", new { Id = 5 }); //where
                //IEnumerable<PeopleTable> data = conn.GetByPageIndex<PeopleTable>(1, 2, returnFields: "Name"); //only return field [name]
                //IEnumerable<PeopleTable> data = conn.GetByPageIndex<PeopleTable>(1, 10, orderBy: "ORDER BY Id DESC"); //order by
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetByPageIndexDynamic()
        {
            using (var conn = DbHelper.GetConn())
            {
                IEnumerable<dynamic> data = conn.GetByPageIndexDynamicAsync<PeopleTable>(1, 2).Result;
                //IEnumerable<dynamic> data = conn.GetByPageIndexDynamic<PeopleTable>(1, 10, "WHERE Id<@Id", new { Id = 5 }); //where
                //IEnumerable<dynamic> data = conn.GetByPageIndexDynamic<PeopleTable>(1, 2, returnFields: "Name"); //only return field [name]
                //IEnumerable<dynamic> data = conn.GetByPageIndexDynamic<PeopleTable>(1, 10, orderBy: "ORDER BY Id DESC"); //order by
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetPage()
        {
            using (var conn = DbHelper.GetConn())
            {
                PageEntity<PeopleTable> data = conn.GetPageAsync<PeopleTable>(5, 2).Result;
                //PageEntity<PeopleTable> data = conn.GetPage<PeopleTable>(1, 10, "WHERE Id<@Id", new { Id = 5 }); //where
                //PageEntity<PeopleTable> data = conn.GetPage<PeopleTable>(1, 2, returnFields: "Name"); //only return field [name]
                //PageEntity<PeopleTable> data = conn.GetPage<PeopleTable>(1, 10, orderBy: "ORDER BY Id DESC"); //order by
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetPageDynamic()
        {
            using (var conn = DbHelper.GetConn())
            {
                PageEntity<dynamic> data = conn.GetPageDynamicAsync<PeopleTable>(1, 2).Result;
                //PageEntity<dynamic> data = conn.GetPageDynamic<PeopleTable>(1, 10, "WHERE Id<@Id", new { Id = 5 }); //where
                //PageEntity<dynamic> data = conn.GetPageDynamic<PeopleTable>(1, 2, returnFields: "Name"); //only return field [name]
                //PageEntity<dynamic> data = conn.GetPageDynamic<PeopleTable>(1, 10, orderBy: "ORDER BY Id DESC"); //order by
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }
    }
}
