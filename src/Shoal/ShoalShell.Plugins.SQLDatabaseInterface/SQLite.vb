Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.ShoalShell
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.HybridsScripting
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports System.Data.SQLite.Linq.DataMapping.Interface
Imports System.Data.SQLite.Linq

<LanguageEntryPoint("SQLite", "Language for query local configuration database.")>
<[PackageNamespace]("DBI.SQLite",
                    Publisher:="xie.guigang@gmail.com",
                    Category:=APICategories.SoftwareTools,
                    Url:="",
                    Description:="Language for query local configuration database.")>
Public Module SQLite

#Region "Shoal shell hybrid scripting interface API"

    Dim SQLiteEngine As SQLProcedure

    ''' <summary>
    ''' Gets or set the SQLite database file for the API.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataFrameColumn("DBI.SQLite.Url")>
    Public Property DataSource As String
        Get
            If SQLiteEngine Is Nothing Then
                Return ""
            Else
                Return SQLiteEngine.URL
            End If
        End Get
        Set(value As String)
            SQLite.SQLiteEngine = SQLProcedure.CreateSQLTransaction(value)
        End Set
    End Property

    <EntryInterface(InterfaceTypes.Evaluate)>
    <ExportAPI("Exec", Info:="If the SQL statement is a select statement, then the function will returns a dynamics datatable, orelse returns nothing!")>
    Public Function Exec(SQL As String) As Object
        Dim DbReader = SQLiteEngine.Execute(SQL)
        If DbReader Is Nothing Then
            Return Nothing
        Else
            Return DbReader.DataFrame
        End If
    End Function

    <EntryInterface(InterfaceTypes.SetValue)>
    <ExportAPI("Insert/Update",
               Info:="if target object instance is exists in the table, then function will update the record, or it will insert a new record into the table.")>
    Public Function InsertOrUpdate(Table As String, obj As Object) As Boolean
        If Reflector.RecordExists(SQLiteEngine, obj) Then
            Return Reflector.Update(SQLiteEngine, obj)
        Else
            Return Reflector.Insert(SQLiteEngine, obj)
        End If
    End Function
#End Region

    <ExportAPI("DBI.Connect")>
    Public Function Connect(url As String) As SQLProcedure
        SQLiteEngine = SQLProcedure.CreateSQLTransaction(url)
        Return SQLiteEngine
    End Function

    <ExportAPI("Exec")>
    Public Function Exec(DBI As SQLProcedure, SQL As String) As Object
        Return DBI.Execute(SQL)
    End Function

    <ExportAPI("Insert_Into")>
    Public Function Insert(DBI As SQLProcedure, obj As Object) As Boolean
        Return Reflector.Insert(DBI, obj:=obj)
    End Function

    <ExportAPI("Delete")>
    Public Function Delete(DBI As SQLProcedure, obj As Object) As Boolean
        Return Reflector.Delete(DBI, obj)
    End Function

    <ExportAPI("Table.Create")>
    Public Function CreateTable(DBI As SQLProcedure, Table As Type, Optional Delete_Exists As Boolean = False) As String
        If DBI.ExistsTable(Table) Then
            If Delete_Exists Then
                Call DBI.DeleteTable(Table)
            Else
                Return ""
            End If
        End If

        Return DBI.CreateTableFor(Table)
    End Function

    <ExportAPI("DBI.Get.TableName")>
    Public Function GetTableName(Schema As Type) As String
        Return DataMapping.Interface.GetTableName(type:=Schema)
    End Function

    <ExportAPI("Table.Is_Exists")>
    Public Function TableExists(DBI As SQLProcedure, Table As String) As Boolean
        Return DBI.ExistsTable(Table)
    End Function

    <ExportAPI("SQLite.Mapping_Query")>
    Public Function QueryTable(DBI As SQLProcedure, Schema As Type) As Object()
        Dim ChunkBuffer = Reflector.Load(DBI, Schema)
        Dim LQuery = (From row In ChunkBuffer Select s = row.ToString).ToArray
        Call Console.WriteLine(String.Join(vbCrLf, LQuery))
        Return ChunkBuffer
    End Function
End Module
