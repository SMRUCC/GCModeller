Imports System.Runtime.CompilerServices
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports Oracle.LinuxCompatibility.MySQL.Reflection.Schema

Public Module DataClient

    ''' <summary>
    ''' 从数据库之中加载所有的数据到程序的内存之中，只推荐表的数据量比较小的使用，
    ''' 使用这个函数加载完数据到内存之中后，进行内存之中的查询操作，会很明显提升应用程序的执行性能
    ''' 
    ''' ```SQL
    ''' SELECT * FROM `{table.Database}`.`{table.TableName}`;
    ''' ```
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="mysql"></param>
    ''' <returns></returns>
    <Extension>
    Public Function SelectALL(Of T As SQLTable)(mysql As MySQL) As T()
        Dim table As TableName = GetType(T).GetAttribute(Of TableName)
        Dim SQL$ = $"SELECT * FROM `{table.Database}`.`{table.Name}`;"
        Return mysql.Query(Of T)(SQL)
    End Function
End Module
