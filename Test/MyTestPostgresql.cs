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
    public class MyTestPostgresql  //DapperExtensions test
    {
        int Second = DateTime.Now.Second;

        [Test]
        public void GetDataTable()
        {
            string sql = "SELECT * FROM \"People\"";
            using (var conn = DbHelper.GetConn())
            {
                DataTable dt = conn.GetDataTable(sql);
                Assert.Pass(dt.Rows.Count.ToString());
            }

        }

        [Test]
        public void GetDataSet()
        {
            string sql = "SELECT * FROM \"People\";SELECT * FROM \"Student\";SELECT * FROM \"School\"";
            using (var conn = DbHelper.GetConn())
            {
                DataSet ds = conn.GetDataSet(sql);
                Assert.Pass(ds.Tables.Count.ToString());
            }
        }

        [Test]
        public void GetSchemaTable()
        {
            using (var conn = DbHelper.GetConn())
            {
                DataTable dt = conn.GetSchemaTable<PeopleTable>();
                Assert.Pass(dt.Columns[1].ColumnName);
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
                int effect = conn.Insert(people);

                StudentTable student = new StudentTable();
                student.Id = ObjectId.GenerateNewId().ToString();
                student.Name = "王五" + Second;
                student.Sex = Second;
                effect += conn.Insert(student);
                Assert.Pass(effect.ToString());

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
                var id = conn.InsertReturnId(people);
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
                int effect = conn.InsertIdentity(people);
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
                int effect = conn.Update(people);
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

                int effect = conn.UpdateByWhere(people, "WHERE \"Sex\"=@Sex", "Name");

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

                int effect = conn.InsertOrUpdate(people);
                //int effect = conn.InsertOrUpdate(people, "Name"); //only update Name
                effect += conn.InsertOrUpdate(p2);

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
                int effect = conn.InsertIdentityOrUpdate(people);

                PeopleTable p2 = new PeopleTable();
                p2.Id = 52;
                p2.Name = "小罗" + Second;
                p2.Sex = Second;
                effect += conn.InsertIdentityOrUpdate(p2);

                Assert.Pass(effect.ToString());
            }
        }

        [Test]
        public void Delete()
        {
            using (var conn = DbHelper.GetConn())
            {
                int effecf = conn.Delete<PeopleTable>(1);
                Assert.Pass(effecf.ToString());
            }
        }

        [Test]
        public void DeleteByIds()
        {
            using (var conn = DbHelper.GetConn())
            {
                //school
                int[] ids = new int[] { 15, 18, 19, 28 };

                //student
                string[] ids2 = new string[] 
                {
                    "5c2c5e54922fdc2cc86bb7b6",
                    "5c2c5e4f922fdc1af8cdfae9"
                };

                int effect = conn.DeleteByIds<SchoolTable>(ids);
                effect += conn.DeleteByIds<StudentTable>(ids2);
                Assert.Pass(effect.ToString());

            }


        }

        [Test]
        public void DeleteByWhere()
        {
            using (var conn = DbHelper.GetConn())
            {
                string where = "WHERE \"Name\"=@Name";
                int effect = conn.DeleteByWhere<SchoolTable>(where, new { Name = "2" });
                Assert.Pass(effect.ToString());
            }
        }

        [Test]
        public void DeleteAll()
        {
            using (var conn = DbHelper.GetConn())
            {
                int effect = conn.DeleteAll<SchoolTable>();
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
                long total = conn.GetTotal<PeopleTable>();
                total += conn.GetTotal<PeopleTable>("WHERE \"Id\"=@Id", new { id = 1 });
                Assert.Pass(total.ToString());
            }
        }

        [Test]
        public void GetAll()
        {
            using (var conn = DbHelper.GetConn())
            {
                IEnumerable<PeopleTable> data = conn.GetAll<PeopleTable>();
                //IEnumerable<PeopleTable> data = conn.GetAll<PeopleTable>("\"Name\""); //only return [name] field split with , "Name,Sex"
                //IEnumerable<PeopleTable> data = conn.GetAll<PeopleTable>(null,"ORDER BY \"Id\" DESC");
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetAllDynamic()
        {
            using (var conn = DbHelper.GetConn())
            {
                IEnumerable<dynamic> data = conn.GetAllDynamic<PeopleTable>("\"Id\",\"Name\"");
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetById()
        {
            using (var conn = DbHelper.GetConn())
            {
                PeopleTable people = conn.GetById<PeopleTable>(1);
                //PeopleTable people = conn.GetById<PeopleTable>(1,"\"Name\""); //only return [name] field
                string json = JsonConvert.SerializeObject(people);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetByIdDynamic()
        {
            using (var conn = DbHelper.GetConn())
            {
                dynamic people = conn.GetByIdDynamic<PeopleTable>(1);
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
                IEnumerable<PeopleTable> data = conn.GetByIds<PeopleTable>(ids); //select * from people where id in @ids
                //IEnumerable<PeopleTable> data = conn.GetByIds<PeopleTable>(ids, "\"Name\""); //select name from people where id in @ids
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
                IEnumerable<dynamic> data = conn.GetByIdsDynamic<PeopleTable>(ids);
                //IEnumerable<dynamic> data = conn.GetByIdsDynamic<PeopleTable>(ids, "\"Name\""); //only return [name] field
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
                IEnumerable<PeopleTable> data = conn.GetByIdsWithField<PeopleTable>(ids, "Sex"); //select * from people where Sex in @ids
                //IEnumerable<PeopleTable> data = conn.GetByIdsWithField<PeopleTable>(ids, "Sex","\"Name\""); //only return [name] field
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
                IEnumerable<dynamic> data = conn.GetByIdsWithFieldDynamic<PeopleTable>(ids, "Sex"); //select * from people where sex in @ids
                //IEnumerable<dynamic> data = conn.GetByIdsWithFieldDynamic<PeopleTable>(ids, "Sex","\"Name\",\"Sex\""); //only return [name] field
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetByWhere()
        {
            using (var conn = DbHelper.GetConn())
            {
                IEnumerable<PeopleTable> data = conn.GetByWhere<PeopleTable>("WHERE \"Id\"<@Id", new { Id = 3 });
                //IEnumerable<PeopleTable> data = conn.GetByWhere<PeopleTable>("WHERE \"Id\"<@Id", new { Id = 3 }, "\"Name\"");//only return [name] field
                //IEnumerable<PeopleTable> data = conn.GetByWhere<PeopleTable>("WHERE \"Id\"<@Id", new { Id = 3 }, null, "ORDER BY \"Id\" DESC"); // order by
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetByWhereDynamic()
        {
            using (var conn = DbHelper.GetConn())
            {
                IEnumerable<dynamic> data = conn.GetByWhereDynamic<PeopleTable>("WHERE \"Id\"<@Id", new { Id = 3 });
                //IEnumerable<dynamic> data = conn.GetByWhereDynamic<PeopleTable>("WHERE \"Id\"<@Id", new { Id = 3 }, "\"Name\",\"Sex\"");//only return [name] field
                //IEnumerable<dynamic> data = conn.GetByWhereDynamic<PeopleTable>("WHERE \"Id\"<@Id", new { Id = 3 }, null, "ORDER BY \"Id\" DESC"); // order by
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetByWhereFirst()
        {
            using (var conn = DbHelper.GetConn())
            {
                PeopleTable people = conn.GetByWhereFirst<PeopleTable>("WHERE \"Id\"<@Id", new { Id = 4 });
                //PeopleTable people = conn.GetByWhereFirst<PeopleTable>("WHERE \"Id\"<@Id", new { Id = 4 },"\"Name\""); //only return [Name] field
                string json = JsonConvert.SerializeObject(people);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetByWhereFirstDynamic()
        {
            using (var conn = DbHelper.GetConn())
            {
                dynamic people = conn.GetByWhereFirstDynamic<PeopleTable>("WHERE \"Id\"<@Id", new { Id = 4 });
                //dynamic people = conn.GetByWhereFirstDynamic<PeopleTable>("WHERE \"Id\"<@Id", new { Id = 4 },"\"Name\""); //only return [Name] field
                string json = JsonConvert.SerializeObject(people);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetBySkipTake()
        {
            using (var conn = DbHelper.GetConn())
            {
                IEnumerable<PeopleTable> data = conn.GetBySkipTake<PeopleTable>(0, 5);
                //IEnumerable<PeopleTable> data = conn.GetBySkipTake<PeopleTable>(0, 10, "WHERE \"Id\"<@Id", new { Id = 5 }); //where
                //IEnumerable<PeopleTable> data = conn.GetBySkipTake<PeopleTable>(0, 2,returnFields:"\"Name\""); //only return field [name]
                //IEnumerable<PeopleTable> data = conn.GetBySkipTake<PeopleTable>(0, 10, orderBy:"ORDER BY \"Id\" DESC"); //order by
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetBySkipTakeDynamic()
        {
            using (var conn = DbHelper.GetConn())
            {
                IEnumerable<dynamic> data = conn.GetBySkipTakeDynamic<PeopleTable>(0, 2);
                //IEnumerable<dynamic> data = conn.GetBySkipTakeDynamic<PeopleTable>(0, 10, "WHERE \"Id\"<@Id", new { Id = 5 }); //where
                //IEnumerable<dynamic> data = conn.GetBySkipTakeDynamic<PeopleTable>(0, 2, returnFields: "\"Name\""); //only return field [name]
                //IEnumerable<dynamic> data = conn.GetBySkipTakeDynamic<PeopleTable>(0, 10, orderBy: "ORDER BY \"Id\" DESC"); //order by
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetByPageIndex()
        {
            using (var conn = DbHelper.GetConn())
            {
                IEnumerable<PeopleTable> data = conn.GetByPageIndex<PeopleTable>(2, 2);
                //IEnumerable<PeopleTable> data = conn.GetByPageIndex<PeopleTable>(1, 10, "WHERE \"Id\"<@Id", new { Id = 5 }); //where
                //IEnumerable<PeopleTable> data = conn.GetByPageIndex<PeopleTable>(1, 2, returnFields: "\"Name\""); //only return field [name]
                //IEnumerable<PeopleTable> data = conn.GetByPageIndex<PeopleTable>(1, 10, orderBy: "ORDER BY \"Id\" DESC"); //order by
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetByPageIndexDynamic()
        {
            using (var conn = DbHelper.GetConn())
            {
                IEnumerable<dynamic> data = conn.GetByPageIndexDynamic<PeopleTable>(1, 2);
                //IEnumerable<dynamic> data = conn.GetByPageIndexDynamic<PeopleTable>(1, 10, "WHERE \"Id\"<@Id", new { Id = 5 }); //where
                //IEnumerable<dynamic> data = conn.GetByPageIndexDynamic<PeopleTable>(1, 2, returnFields: "\"Name\""); //only return field [name]
                //IEnumerable<dynamic> data = conn.GetByPageIndexDynamic<PeopleTable>(1, 10, orderBy: "ORDER BY \"Id\" DESC"); //order by
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetPage()
        {
            using (var conn = DbHelper.GetConn())
            {
                PageEntity<PeopleTable> data = conn.GetPage<PeopleTable>(1, 2);
                //PageEntity<PeopleTable> data = conn.GetPage<PeopleTable>(1, 10, "WHERE \"Id\"<@Id", new { Id = 5 }); //where
                //PageEntity<PeopleTable> data = conn.GetPage<PeopleTable>(1, 2, returnFields: "\"Name\""); //only return field [name]
                //PageEntity<PeopleTable> data = conn.GetPage<PeopleTable>(1, 10, orderBy: "ORDER BY \"Id\" DESC"); //order by
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetPageDynamic()
        {
            using (var conn = DbHelper.GetConn())
            {
                PageEntity<dynamic> data = conn.GetPageDynamic<PeopleTable>(1, 2);
                //PageEntity<dynamic> data = conn.GetPageDynamic<PeopleTable>(1, 10, "WHERE \"Id\"<@Id", new { Id = 5 }); //where
                //PageEntity<dynamic> data = conn.GetPageDynamic<PeopleTable>(1, 2, returnFields: "\"Name\""); //only return field [name]
                //PageEntity<dynamic> data = conn.GetPageDynamic<PeopleTable>(1, 10, orderBy: "ORDER BY \"Id\" DESC"); //order by
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

    }

}
