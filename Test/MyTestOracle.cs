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
    public class MyTestOracle  //DapperExtensions test
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
        public void GetSchemaTable()
        {
            using (var conn = DbHelper.GetConn())
            {
                DataTable dt = conn.GetSchemaTable<PeopleTableOracle>();
                Assert.Pass(dt.Columns[1].ColumnName);
            }
        }

        [Test]
        public void Insert()
        {
            using (var conn = DbHelper.GetConn())
            {
                PeopleTableOracle people = new PeopleTableOracle();
                people.Id = conn.GetSequenceNext<int>("seq_my");
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
                PeopleTableOracle people = new PeopleTableOracle();
                people.Name = "李四" + Second;
                people.Sex = Second;
                var id = conn.InsertReturnIdForOracle(people,"seq_my");
                Assert.Pass(id.ToString());
            }

        }

        [Test]
        public void InsertIdentity()
        {
            using (var conn = DbHelper.GetConn())
            {
                PeopleTableOracle people = new PeopleTableOracle();
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
                PeopleTableOracle people = new PeopleTableOracle();
                people.Id = 1;
                people.Name = "李四" + Second;
                people.Sex = Second;
                //int effect = conn.Update(people);
                int effect = conn.Update(people, "Name"); //update people set Name=@Name where Id=@Id
                Assert.Pass(effect.ToString());
            }
        }

        [Test]
        public void UpdateByWhere()
        {
            using (var conn = DbHelper.GetConn())
            {
                PeopleTableOracle people = new PeopleTableOracle();
                people.Id = 1;
                people.Name = "钱七" + Second;
                people.Sex = 47;

                int effect = conn.UpdateByWhere(people, "WHERE \"Sex\"=:Sex", "Name");

                Assert.Pass(effect.ToString());
            }

        }

        [Test]
        public void InsertOrUpdate()
        {
            using (var conn = DbHelper.GetConn())
            {
                PeopleTableOracle people = new PeopleTableOracle();
                people.Id = 1;
                people.Name = "王五" + Second;
                people.Sex = Second;

                PeopleTableOracle p2 = new PeopleTableOracle();
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
                PeopleTableOracle people = new PeopleTableOracle();
                people.Id = 1;
                people.Name = "王五" + Second;
                people.Sex = Second;
                int effect = conn.InsertIdentityOrUpdate(people);

                PeopleTableOracle p2 = new PeopleTableOracle();
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
                int effecf = conn.Delete<PeopleTableOracle>(1);
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
                string where = "WHERE \"Name\"=:Name";
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
        public void GetSequenceCurrent() 
        {
            using (var conn = DbHelper.GetConn())
            {
               var seq = conn.GetSequenceCurrent<decimal>("seq_my");
            }
        }

        [Test]
        public void GetSequenceNext() 
        {
            using (var conn = DbHelper.GetConn())
            {
                var seq = conn.GetSequenceNext<decimal>("seq_my");
            }
        }

        [Test]
        public void GetTotal()
        {
            using (var conn = DbHelper.GetConn())
            {
                long total = conn.GetTotal<PeopleTableOracle>();
                total += conn.GetTotal<PeopleTableOracle>("WHERE \"Id\"=:Id", new { id = 1 });
                Assert.Pass(total.ToString());
            }
        }

        [Test]
        public void GetAll()
        {
            using (var conn = DbHelper.GetConn())
            {
                IEnumerable<PeopleTableOracle> data = conn.GetAll<PeopleTableOracle>();
                //IEnumerable<PeopleTableOracle> data = conn.GetAll<PeopleTableOracle>("\"Name\""); //only return [name] field split with , "Name,Sex"
                //IEnumerable<PeopleTableOracle> data = conn.GetAll<PeopleTableOracle>(null,"ORDER BY \"Id\" DESC");
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetAllDynamic()
        {
            using (var conn = DbHelper.GetConn())
            {
                IEnumerable<dynamic> data = conn.GetAllDynamic<PeopleTableOracle>("\"Id\",\"Name\"");
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetById()
        {
            using (var conn = DbHelper.GetConn())
            {
                PeopleTableOracle people = conn.GetById<PeopleTableOracle>(1);
                //PeopleTableOracle people = conn.GetById<PeopleTableOracle>(1,"\"Name\""); //only return [name] field
                string json = JsonConvert.SerializeObject(people);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetByIdDynamic()
        {
            using (var conn = DbHelper.GetConn())
            {
                dynamic people = conn.GetByIdDynamic<PeopleTableOracle>(1);
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
                IEnumerable<PeopleTableOracle> data = conn.GetByIds<PeopleTableOracle>(ids); //select * from people where id in @ids
                //IEnumerable<PeopleTableOracle> data = conn.GetByIds<PeopleTableOracle>(ids, "\"Name\""); //select name from people where id in @ids
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
                IEnumerable<dynamic> data = conn.GetByIdsDynamic<PeopleTableOracle>(ids);
                //IEnumerable<dynamic> data = conn.GetByIdsDynamic<PeopleTableOracle>(ids, "\"Name\""); //only return [name] field
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
                IEnumerable<PeopleTableOracle> data = conn.GetByIdsWithField<PeopleTableOracle>(ids, "Sex"); //select * from people where Sex in @ids
                //IEnumerable<PeopleTableOracle> data = conn.GetByIdsWithField<PeopleTableOracle>(ids, "Sex","\"Name\""); //only return [name] field
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
                IEnumerable<dynamic> data = conn.GetByIdsWithFieldDynamic<PeopleTableOracle>(ids, "Sex"); //select * from people where sex in @ids
                //IEnumerable<dynamic> data = conn.GetByIdsWithFieldDynamic<PeopleTableOracle>(ids, "Sex","\"Name\",\"Sex\""); //only return [name] field
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetByWhere()
        {
            using (var conn = DbHelper.GetConn())
            {
                IEnumerable<PeopleTableOracle> data = conn.GetByWhere<PeopleTableOracle>("WHERE \"Id\"<:Id", new { Id = 3 });
                //IEnumerable<PeopleTableOracle> data = conn.GetByWhere<PeopleTableOracle>("WHERE \"Id\"<:Id", new { Id = 3 }, "\"Name\"");//only return [name] field
                //IEnumerable<PeopleTableOracle> data = conn.GetByWhere<PeopleTableOracle>("WHERE \"Id\"<:Id", new { Id = 3 }, null, "ORDER BY \"Id\" DESC"); // order by
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetByWhereDynamic()
        {
            using (var conn = DbHelper.GetConn())
            {
                IEnumerable<dynamic> data = conn.GetByWhereDynamic<PeopleTableOracle>("WHERE \"Id\"<:Id", new { Id = 3 });
                //IEnumerable<dynamic> data = conn.GetByWhereDynamic<PeopleTableOracle>("WHERE \"Id\"<:Id", new { Id = 3 }, "\"Name\",\"Sex\"");//only return [name] field
                //IEnumerable<dynamic> data = conn.GetByWhereDynamic<PeopleTableOracle>("WHERE \"Id\"<:Id", new { Id = 3 }, null, "ORDER BY \"Id\" DESC"); // order by
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetByWhereFirst()
        {
            using (var conn = DbHelper.GetConn())
            {
                PeopleTableOracle people = conn.GetByWhereFirst<PeopleTableOracle>("WHERE \"Id\"<:Id", new { Id = 4 });
                //PeopleTableOracle people = conn.GetByWhereFirst<PeopleTableOracle>("WHERE \"Id\"<:Id", new { Id = 4 },"\"Name\""); //only return [Name] field
                string json = JsonConvert.SerializeObject(people);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetByWhereFirstDynamic()
        {
            using (var conn = DbHelper.GetConn())
            {
                dynamic people = conn.GetByWhereFirstDynamic<PeopleTableOracle>("WHERE \"Id\"<:Id", new { Id = 4 });
                //dynamic people = conn.GetByWhereFirstDynamic<PeopleTableOracle>("WHERE \"Id\"<:Id", new { Id = 4 },"\"Name\""); //only return [Name] field
                string json = JsonConvert.SerializeObject(people);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetBySkipTake()
        {
            using (var conn = DbHelper.GetConn())
            {
                IEnumerable<PeopleTableOracle> data = conn.GetBySkipTake<PeopleTableOracle>(2, 5);
                //IEnumerable<PeopleTableOracle> data = conn.GetBySkipTake<PeopleTableOracle>(0, 10, "WHERE \"Id\"<:Id", new { Id = 5 }); //where
                //IEnumerable<PeopleTableOracle> data = conn.GetBySkipTake<PeopleTableOracle>(0, 2,returnFields:"\"Name\""); //only return field [name]
                //IEnumerable<PeopleTableOracle> data = conn.GetBySkipTake<PeopleTableOracle>(0, 10, orderBy:"ORDER BY \"Id\" DESC"); //order by
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetBySkipTakeDynamic()
        {
            using (var conn = DbHelper.GetConn())
            {
                //IEnumerable<dynamic> data = conn.GetBySkipTakeDynamic<PeopleTableOracle>(0, 2);
                //IEnumerable<dynamic> data = conn.GetBySkipTakeDynamic<PeopleTableOracle>(0, 10, "WHERE \"Id\"<:Id", new { Id = 5 }); //where
                //IEnumerable<dynamic> data = conn.GetBySkipTakeDynamic<PeopleTableOracle>(0, 2, returnFields: "\"Name\""); //only return field [name]
                IEnumerable<dynamic> data = conn.GetBySkipTakeDynamic<PeopleTableOracle>(0, 10, orderBy: "ORDER BY \"Id\" DESC"); //order by
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetByPageIndex()
        {
            using (var conn = DbHelper.GetConn())
            {
                IEnumerable<PeopleTableOracle> data = conn.GetByPageIndex<PeopleTableOracle>(2, 2);
                //IEnumerable<PeopleTableOracle> data = conn.GetByPageIndex<PeopleTableOracle>(1, 10, "WHERE \"Id\"<:Id", new { Id = 5 }); //where
                //IEnumerable<PeopleTableOracle> data = conn.GetByPageIndex<PeopleTableOracle>(1, 2, returnFields: "\"Name\""); //only return field [name]
                //IEnumerable<PeopleTableOracle> data = conn.GetByPageIndex<PeopleTableOracle>(1, 10, orderBy: "ORDER BY \"Id\" DESC"); //order by
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetByPageIndexDynamic()
        {
            using (var conn = DbHelper.GetConn())
            {
                IEnumerable<dynamic> data = conn.GetByPageIndexDynamic<PeopleTableOracle>(1, 2);
                //IEnumerable<dynamic> data = conn.GetByPageIndexDynamic<PeopleTableOracle>(1, 10, "WHERE \"Id\"<:Id", new { Id = 5 }); //where
                //IEnumerable<dynamic> data = conn.GetByPageIndexDynamic<PeopleTableOracle>(1, 2, returnFields: "\"Name\""); //only return field [name]
                //IEnumerable<dynamic> data = conn.GetByPageIndexDynamic<PeopleTableOracle>(1, 10, orderBy: "ORDER BY \"Id\" DESC"); //order by
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetPage()
        {
            using (var conn = DbHelper.GetConn())
            {
                PageEntity<PeopleTableOracle> data = conn.GetPageForOracle<PeopleTableOracle>(1, 2);
                //PageEntity<PeopleTableOracle> data = conn.GetPageForOracle<PeopleTableOracle>(1, 10, "WHERE \"Id\"<:Id", new { Id = 5 }); //where
                //PageEntity<PeopleTableOracle> data = conn.GetPageForOracle<PeopleTableOracle>(1, 2, returnFields: "\"Name\""); //only return field [name]
                //PageEntity<PeopleTableOracle> data = conn.GetPageForOracle<PeopleTableOracle>(1, 10, orderBy: "ORDER BY \"Id\" DESC"); //order by
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

        [Test]
        public void GetPageDynamic()
        {
            using (var conn = DbHelper.GetConn())
            {
                PageEntity<dynamic> data = conn.GetPageForOracleDynamic<PeopleTableOracle>(1, 2);
                //PageEntity<dynamic> data = conn.GetPageDynamicOracle<PeopleTableOracle>(1, 10, "WHERE \"Id\"<:Id", new { Id = 5 }); //where
                //PageEntity<dynamic> data = conn.GetPageDynamicOracle<PeopleTableOracle>(1, 2, returnFields: "\"Name\""); //only return field [name]
                //PageEntity<dynamic> data = conn.GetPageDynamicOracle<PeopleTableOracle>(1, 10, orderBy: "ORDER BY \"Id\" DESC"); //order by
                string json = JsonConvert.SerializeObject(data);
                Assert.Pass(json);
            }
        }

    }

}
