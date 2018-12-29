1、@Model.ClassName     -----------> c# or java class name
2、@Model.NameSpace     -----------> c# namespace or java package
3、@Model.Table         -----------> TableEntity
4、@Model.ColumnList    -----------> List<ColumnEntity>
5、@Raw                 -----------> special tag like <  > you must use @Raw

public class TableEntity
{
    public string Name { get; set; } //tableName
    public string NameUpper { get; set; } //TableName
    public string NameLower { get; set; } //tableName
    public string Comment { get; set; } //Descript
    public string KeyName { get; set; } //primary key name
    public string IsIdentity { get; set; } //true false
}

public class ColumnEntity
{
    public string Name { get; set; } //name
    public string NameUpper { get; set; } //Name
    public string NameLower { get; set; } //name
    public string CsType { get; set; } //c# type(string  int long double...)
    public string JavaType { get; set; } //java type(String Date...)
    public string Comment { get; set; } //Descript
    public string DbType { get; set; } //(int varchar(20) text...)
    public string AllowNull { get; set; }
    public string DefaultValue { get; set; }
}

=========================================================================================

DbTypeMap.xml
you can config DbType change to c# or java type


=========================================================================================
see more razor grammar,you can go to
https://github.com/Antaris/RazorEngine