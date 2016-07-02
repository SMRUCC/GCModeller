Imports System.Data.SQLite.Linq.DataMapping.Interface
Imports System.Data.SQLite.Linq.DataMapping.Interface.Reflector
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.ShoalShell
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.HybridsScripting
Imports Microsoft.VisualBasic.Win32.WindowsServices
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Data.Model_Repository.SQLEngines
Imports SMRUCC.genomics.Data.Model_Repository.Tables

<LanguageEntryPoint("GCModeller.RQL",
                    "The linq like language for query the GCModeller repository database.")>
<[PackageNamespace]("GCModeller.Workbench.Model_Repository",
                    Publisher:="xie.guigang@gcmodeller.org")>
Public Module API

    Dim SQLiteEngine As SQLiteIndex

    Public ReadOnly Property RepositoryEngine As SQLiteIndex
        Get
            Return SQLiteEngine
        End Get
    End Property

    Sub New()
        Call Settings.Initialize()
        SQLiteEngine = New SQLiteIndex(Settings.SettingsFile.RepositoryRoot)
    End Sub

    <ExportAPI("DB.init()",
               Info:="Warning, this operation will loose all of the previous data in this repository.")>
    Public Function DBInit() As Boolean
        If SQLiteEngine Is Nothing Then
            Call Initialize()
        End If

        Return Model_Repository.DBInit.InvokeOperation(SQLiteEngine)
    End Function

    <EntryInterface(InterfaceTypes.Evaluate)>
    Public Function SQLQuery(sql As String) As Object
        Dim Reader As System.Data.Common.DbDataReader = SQLiteEngine.SQLiteEngine.Execute(sql)
        Dim TableName As String = Regex.Match(sql, "from '.+?'", RegexOptions.IgnoreCase).Value
        TableName = Regex.Replace(TableName, "from\s+", "", RegexOptions.IgnoreCase)
        TableName = Mid(TableName, 2, Len(TableName) - 2)

        If String.Equals(TableName, System.Data.SQLite.Linq.DataMapping.Interface.Reflector.GetTableName(Of GenbankEntryInfo)) Then
            Dim ChunkBuffer = Reader.Load(TypeInfo:=GetType(GenbankEntryInfo))
            Dim LQuery = (From item In ChunkBuffer Select DirectCast(item, GenbankEntryInfo)).ToArray
            Return LQuery
        Else '通用表
            Dim df = Reader.DataFrame
            Return New DocumentStream.File(df)
        End If
    End Function

    ''' <summary>
    ''' 更新或者插入新的数据对象<paramref name="obj"></paramref>到目标数据表<paramref name="tableName"></paramref>之中
    ''' </summary>
    ''' <param name="tableName"></param>
    ''' <param name="obj"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <EntryInterface(InterfaceTypes.SetValue)>
    Public Function SQLImports(tableName As String, obj As Object) As Boolean
        Return SQLiteEngine.Imports(DirectCast(obj, GBFF.File))
    End Function

    <ExportAPI("SqlDump.Create.GenbankEntry")>
    Public Function CreateGenbankEntryDump() As String
        Return SQLiteEngine.SQLiteEngine.CreateSQLDump(Of GenbankEntryInfo)()
    End Function

    <ExportAPI("Set.RepositoryRoot")>
    Public Function SetRepositoryRoot(dir As String) As String
        Settings.SettingsFile.RepositoryRoot = dir
        Call FileIO.FileSystem.CreateDirectory(dir)
        SQLiteEngine = New SQLiteIndex(dir)
        Call Settings.Save()
        Return dir
    End Function

    <ExportAPI("Genbank.Query.Def", Info:="The function returns the file path list of the target keyword query result.")>
    Public Function QueryGBKByDefinition(keyword As String, Optional disp_out_result As Boolean = True) As String()
        Dim EntryList = SQLiteEngine.QueryGenbankEntryByDefinition(keyword)
        Dim LQuery = (From item In EntryList Select item.GetPath(Settings.SettingsFile.RepositoryRoot)).ToArray
        If disp_out_result Then Call Console.WriteLine(String.Join(vbCrLf, LQuery))
        Return LQuery
    End Function

    <ExportAPI("Genbank.Query.Def", Info:="The function returns the file path list of the target keyword query result.")>
    Public Function QueryGBKByDefinition(keywords As String()) As String()
        Dim LQuery = (From keyword As String In keywords Select QueryGBKByDefinition(keyword, disp_out_result:=False)).ToArray.MatrixToList.Distinct.ToArray
        Call Console.WriteLine(String.Join(vbCrLf, LQuery))
        Return LQuery
    End Function

    <ExportAPI("Genbank.Query.Locus_Id", Info:="The function returns the file path list of the target keyword query result.")>
    Public Function QueryGBKByLocusID(id_list As String()) As String()
        Dim TableName As String = GetTableName(Of GenbankEntryInfo)()
        Dim SQL As String() = (From id As String
                               In id_list
                               Select QueryBuilder.QueryByStringCompares("locus_id", value:=id, TableName:=TableName, ComparedMethod:=StringCompareMethods.EqualsWithCaseInsensitive)).ToArray
        Dim LQuery As GenbankEntryInfo() = (From Query As String
                                            In SQL
                                            Select API.SQLiteEngine.SQLiteEngine.Load(Of GenbankEntryInfo)(SQL:=Query)).ToArray.MatrixToVector
        Dim PathSource As String() = (From ResultItem In LQuery Select ResultItem.GetPath(API.SQLiteEngine.RepositoryRoot)).ToArray
        Call Console.WriteLine(String.Join(vbCrLf, PathSource))
        Return PathSource
    End Function

    <ExportAPI("Genbank.Query.Locus_Id", Info:="The function returns the file path list of the target keyword query result.")>
    Public Function QueryGBKByLocusID(id As String) As String()
        Dim TableName As String = GetTableName(Of GenbankEntryInfo)()
        Dim SQL As String = QueryBuilder.QueryByStringCompares("locus_id", value:=id, TableName:=TableName, ComparedMethod:=StringCompareMethods.EqualsWithCaseInsensitive)
        Dim LQuery As GenbankEntryInfo() = API.SQLiteEngine.SQLiteEngine.Load(Of GenbankEntryInfo)(SQL)
        Dim PathSource As String() = (From ResultItem In LQuery Select ResultItem.GetPath(API.SQLiteEngine.RepositoryRoot)).ToArray
        Call Console.WriteLine(String.Join(vbCrLf, PathSource))
        Return PathSource
    End Function

    ''' <summary>
    ''' 当<paramref name="path"></paramref>参数为一个文件夹的时候，会扫描该文件夹之中的所有文件
    ''' </summary>
    ''' <param name="path">If the path parameter is specific to a file, then it will imports the target GenBank file, orelse it will try imports the path as a folder in batch mode!</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("Gbk.Imports", Info:="If the path parameter is specific to a file, then it will imports the target GenBank file, orelse it will try imports the path as a folder in batch mode!")>
    Public Function ImportsGBK(path As String) As Boolean
        If FileIO.FileSystem.FileExists(path) Then
            Return SQLiteEngine.Imports(GBFF.File.Read(path))
        Else
            Dim ImportsLQuery = (From FilePath As String
                                 In FileIO.FileSystem.GetFiles(path, FileIO.SearchOption.SearchAllSubDirectories, "*.gbk")
                                 Select SQLiteEngine.Imports(GBFF.File.Read(FilePath))).ToArray
            Return Not ImportsLQuery.IsNullOrEmpty
        End If
    End Function

    <ExportAPI("Get.SecurityTableInfo")>
    Public Function GetSecurityTableInfo() As Type
        Return GetType(UserSecurity)
    End Function

    <ExportAPI("User.Create")>
    Public Function CreateUser(user As String, pwd As String) As UserSecurity
        Return New UserSecurity With {
            .UserName = user,
            .Group = -1,
            .Password = New SecurityString.SHA256("GCModeller", "12589634").EncryptData(pwd)
        }
    End Function

    ''' <summary>
    ''' 这个函数仅仅是在修改了数据结构之后，需要更新表结构的时候才需要使用的
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <ExportAPI("table.schema.update()", Info:="This function is only for the developer.")>
    Public Function UpdateGenbankTableSchema() As Boolean
        Return GenbankEntryInfo.UpdateTableSchema(API.SQLiteEngine)
    End Function
End Module