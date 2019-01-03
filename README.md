# DapperExtensions
open source and zero config
##### 1、IDbConnection
```c#
public static IDbConnection GetConn()
{
      return new SqlConnection("server=127.0.0.1;uid=sa;pwd=123456;database=test;timeout=1");
      //return new MySqlConnection("server=127.0.0.1;uid=root;pwd=123456;database=test;charset=utf8");
      //return new SQLiteConnection(@"Data Source=C:\Users\Administrator\Desktop\1.db;Pooling=true;FailIfMissing=false");
      //return new NpgsqlConnection("server=127.0.0.1;uid=postgres;pwd=123456;database=test");
      //return new OracleConnection("User ID=test;Password=123456;Data Source=(DESCRIPTION =(ADDRESS_LIST =(ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521)))(CONNECT_DATA =(SERVICE_NAME = XE)))");
}
```
##### 2、Table Class
```c#
using System;
using DapperExtensions;

[Table(TableName = "People", KeyName = "Id", IsIdentity = true)]
public class PeopleTable
{

    public int Id { get; set; }

    public string Name { get; set; }

    public int Sex { get; set; }

    //[Igore]
    //public dynamic OtherData = new ExpandoObject();
    
}
```
##### 2、Ready to fly
```c#
using DapperExtensions;

using (var conn = GetConn()) //IDbConnection (sqlserver、mysql、oracle、postgresql、sqlite)
{
      PeopleTable people = new PeopleTable();
      people.Name = "Jone";
      people.Sex = 1;

      //Insert
      int effect = conn.Insert(people);
      var id = conn.InsertReturnId(people); //return the insertId

      int effect = conn.InsertIdentity(people); //insert Identity

      //oracle insert
      people.Id = conn.GetSequenceNext<int>("seq_my"); //get the sequence
      int effect = conn.Insert(people);

      var id = conn.InsertReturnIdForOracle(people,"seq_my");//insert and return id
      
      var seq = conn.GetSequenceNext<decimal>("seq_my"); //oracle seq
      var seq = conn.GetSequenceCurrent<decimal>("seq_my");
      
      //Update
      int effect = conn.Update(people);
      int effect = conn.Update(people, "Name"); //update people set Name=@Name where Id=@Id (update field split by ,)

      //UpdateByWhere
      PeopleTable people = new PeopleTable();
      people.Name = "lili";
      people.Sex = 1;
      int effect = conn.UpdateByWhere(people, "WHERE Sex=@Sex", "Name"); //update people set Name=@Name WHERE Sex=@Sex

      //InsertOrUpdate
      int effect = conn.InsertOrUpdate(people); //if exists update by id else insert

      //InsertIdentityOrUpdate
      int effect = conn.InsertIdentityOrUpdate(people); //if exists update by id else insertidentity

      //Delete
      int effecf = conn.Delete<PeopleTable>(1); //object id

      //DeleteByIds 
      int[] ids = new int[] { 15, 18, 19, 28 };//school
   
      string[] ids2 = new string[]   //student
      {
          "5c2c5e54922fdc2cc86bb7b6",
          "5c2c5e4f922fdc1af8cdfae9"
      };
      int effect = conn.DeleteByIds<SchoolTable>(ids);
      effect += conn.DeleteByIds<StudentTable>(ids2);
      
      //DeleteByWhere
      string where = "WHERE Name=@Name";
      int effect = conn.DeleteByWhere<SchoolTable>(where, new { Name = "2" });
      
      //DeleteAll
      int effect = conn.DeleteAll<SchoolTable>();
      
      //GetTotal
      long total = conn.GetTotal<PeopleTable>();
      long total = conn.GetTotal<PeopleTable>("WHERE Id=@Id", new { id = 1 }); 
      
      //GetAll
      IEnumerable<PeopleTable> data = conn.GetAll<PeopleTable>();
      IEnumerable<PeopleTable> data = conn.GetAll<PeopleTable>("Id,Name"); //only return Id,Name
      
      //GetAllDynamic
      IEnumerable<dynamic> data = conn.GetAllDynamic<PeopleTable>();
      
      //GetById
      PeopleTable people = conn.GetById<PeopleTable>(1);
      PeopleTable people = conn.GetById<PeopleTable>(1,"Id,Name"); ////only return Id,Name
      
      //GetByIdDynamic
      dynamic people = conn.GetByIdDynamic<PeopleTable>(1);
      
      //GetByIds
      List<int> ids = new List<int>() { 1, 2, 3 };
      IEnumerable<PeopleTable> data = conn.GetByIds<PeopleTable>(ids); //select * from people where id in @ids
      IEnumerable<PeopleTable> data = conn.GetByIds<PeopleTable>(ids, "Name"); //select name from people where id in @ids
      
      //GetByIdsDynamic
      int[] ids = new int[] { 1, 2, 3 };
      IEnumerable<dynamic> data = conn.GetByIdsDynamic<PeopleTable>(ids);
      IEnumerable<dynamic> data = conn.GetByIdsDynamic<PeopleTable>(ids, "Name"); //only return [name] field
      
      //GetByIdsWithField
      int[] ids = new int[] { 18, 1, 19 };
      IEnumerable<PeopleTable> data = conn.GetByIdsWithField<PeopleTable>(ids, "Sex"); //select * from people where Sex in @ids
      IEnumerable<PeopleTable> data = conn.GetByIdsWithField<PeopleTable>(ids, "Sex","Name"); //only return [name] field
      
      //GetByIdsWithFieldDynamic
      int[] ids = new int[] { 18, 1, 19 };
      IEnumerable<dynamic> data = conn.GetByIdsWithFieldDynamic<PeopleTable>(ids, "Sex"); //select * from people where sex in @ids
      IEnumerable<dynamic> data = conn.GetByIdsWithFieldDynamic<PeopleTable>(ids, "Sex","Name,Sex"); //only return [name] field
      
      //GetByWhere
      IEnumerable<PeopleTable> data = conn.GetByWhere<PeopleTable>("WHERE Id<@Id", new { Id = 3 });
      IEnumerable<PeopleTable> data = conn.GetByWhere<PeopleTable>("WHERE Id<@Id", new { Id = 3 }, "Name");//only return [name] field
      IEnumerable<PeopleTable> data = conn.GetByWhere<PeopleTable>("WHERE Id<@Id", new { Id = 3 }, null, "ORDER BY Id DESC"); // order by
      
      //GetByWhereDynamic
      IEnumerable<dynamic> data = conn.GetByWhereDynamic<PeopleTable>("WHERE Id<@Id", new { Id = 3 });
      IEnumerable<dynamic> data = conn.GetByWhereDynamic<PeopleTable>("WHERE Id<@Id", new { Id = 3 }, "Name,Sex");//only return [name] field
      IEnumerable<dynamic> data = conn.GetByWhereDynamic<PeopleTable>("WHERE Id<@Id", new { Id = 3 }, null, "ORDER BY Id DESC"); // order by
      
      //GetByWhereFirst
      PeopleTable people = conn.GetByWhereFirst<PeopleTable>("WHERE Id<@Id", new { Id = 4 });
      PeopleTable people = conn.GetByWhereFirst<PeopleTable>("WHERE Id<@Id", new { Id = 4 },"Name"); //only return [Name] field
      
      //GetByWhereFirstDynamic
      dynamic people = conn.GetByWhereFirstDynamic<PeopleTable>("WHERE Id<@Id", new { Id = 4 });
      dynamic people = conn.GetByWhereFirstDynamic<PeopleTable>("WHERE Id<@Id", new { Id = 4 },"Name"); //only return [Name] field
      
      //GetBySkipTake
      IEnumerable<PeopleTable> data = conn.GetBySkipTake<PeopleTable>(0, 5);
      IEnumerable<PeopleTable> data = conn.GetBySkipTake<PeopleTable>(0, 10, "WHERE Id<@Id", new { Id = 5 }); //where
      IEnumerable<PeopleTable> data = conn.GetBySkipTake<PeopleTable>(0, 2,returnFields:"Name"); //only return field [name]
      IEnumerable<PeopleTable> data = conn.GetBySkipTake<PeopleTable>(0, 10, orderBy:"ORDER BY Id DESC"); //order by
      
      //GetBySkipTakeDynamic
      IEnumerable<dynamic> data = conn.GetBySkipTakeDynamic<PeopleTable>(0, 2);
      IEnumerable<dynamic> data = conn.GetBySkipTakeDynamic<PeopleTable>(0, 10, "WHERE Id<@Id", new { Id = 5 }); //where
      IEnumerable<dynamic> data = conn.GetBySkipTakeDynamic<PeopleTable>(0, 2, returnFields: "Name"); //only return field [name]
      IEnumerable<dynamic> data = conn.GetBySkipTakeDynamic<PeopleTable>(0, 10, orderBy: "ORDER BY Id DESC"); //order by
      
      //GetByPageIndex
      IEnumerable<PeopleTable> data = conn.GetByPageIndex<PeopleTable>(2, 2);
      IEnumerable<PeopleTable> data = conn.GetByPageIndex<PeopleTable>(1, 10, "WHERE Id<@Id", new { Id = 5 }); //where
      IEnumerable<PeopleTable> data = conn.GetByPageIndex<PeopleTable>(1, 2, returnFields: "Name"); //only return field [name]
      IEnumerable<PeopleTable> data = conn.GetByPageIndex<PeopleTable>(1, 10, orderBy: "ORDER BY Id DESC"); //order by
      
      //GetByPageIndexDynamic
      IEnumerable<dynamic> data = conn.GetByPageIndexDynamic<PeopleTable>(1, 2);
      IEnumerable<dynamic> data = conn.GetByPageIndexDynamic<PeopleTable>(1, 10, "WHERE Id<@Id", new { Id = 5 }); //where
      IEnumerable<dynamic> data = conn.GetByPageIndexDynamic<PeopleTable>(1, 2, returnFields: "Name"); //only return field [name]
      IEnumerable<dynamic> data = conn.GetByPageIndexDynamic<PeopleTable>(1, 10, orderBy: "ORDER BY Id DESC"); //order by
      
      //GetPage
      PageEntity<PeopleTable> data = conn.GetPage<PeopleTable>(1, 2);
      PageEntity<PeopleTable> data = conn.GetPage<PeopleTable>(1, 10, "WHERE Id<@Id", new { Id = 5 }); //where
      PageEntity<PeopleTable> data = conn.GetPage<PeopleTable>(1, 2, returnFields: "Name"); //only return field [name]
      PageEntity<PeopleTable> data = conn.GetPage<PeopleTable>(1, 10, orderBy: "ORDER BY Id DESC"); //order by
      
      //GetPageDynamic
      PageEntity<dynamic> data = conn.GetPageDynamic<PeopleTable>(1, 2);
      PageEntity<dynamic> data = conn.GetPageDynamic<PeopleTable>(1, 10, "WHERE Id<@Id", new { Id = 5 }); //where
      PageEntity<dynamic> data = conn.GetPageDynamic<PeopleTable>(1, 2, returnFields: "Name"); //only return field [name]
      PageEntity<dynamic> data = conn.GetPageDynamic<PeopleTable>(1, 10, orderBy: "ORDER BY Id DESC"); //order by
      
      //for oracle GetPageForOracle
      PageEntity<PeopleTableOracle> data = conn.GetPageForOracle<PeopleTableOracle>(1, 2);
      PageEntity<PeopleTableOracle> data = conn.GetPageForOracle<PeopleTableOracle>(1, 10, "WHERE \"Id\"<:Id", new { Id = 5 }); //where
      PageEntity<PeopleTableOracle> data = conn.GetPageForOracle<PeopleTableOracle>(1, 2, returnFields: "\"Name\""); //only return field [name]
      PageEntity<PeopleTableOracle> data = conn.GetPageForOracle<PeopleTableOracle>(1, 10, orderBy: "ORDER BY \"Id\" DESC"); //order by
    
      //GetDataTable
      string sql = "SELECT * FROM People";
      DataTable dt = conn.GetDataTable(sql);
      
      //GetDataSet
      string sql = "SELECT * FROM People;SELECT * FROM Student;SELECT * FROM School";
      DataSet ds = conn.GetDataSet(sql);
      
      //GetSchemaTable
      DataTable dt = conn.GetSchemaTable<PeopleTable>();
      
      //SqlBulkCopy
      string msg = conn.BulkCopy(dt, "School", null);
      
      //SqlBulkUpdate
      string msg = conn.BulkUpdate(dt, "School");
      
}
```
# CodeGenerator
<img src="https://github.com/znyet/img/blob/master/code/1.png"  /><br>
<img src="https://github.com/znyet/img/blob/master/code/2.png"  /><br>
<img src="https://github.com/znyet/img/blob/master/code/3.png"  /><br>
<img src="https://github.com/znyet/img/blob/master/code/4.png"  /><br>
<img src="https://github.com/znyet/img/blob/master/code/5.png"  /><br>

also java getter setter
<img src="https://github.com/znyet/img/blob/master/code/6.png"  /><br>
<img src="https://github.com/znyet/img/blob/master/code/7.png"  /><br>

If you think it's very helpful to you, you can buy me a cup of coffee. Thank you. <br>  <br>
<img src="https://github.com/znyet/img/blob/master/wx.jpg?raw=true"  /><br>  
<img src="https://github.com/znyet/img/blob/master/zfb.jpg?raw=true" />
