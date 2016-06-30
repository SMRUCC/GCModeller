#Region "Microsoft.VisualBasic::4975f55fc6f2eab6a7715bd967a614e7, ..\SQLite_Interface\SQLProcedure.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports System.Data.Common
Imports System.Text
Imports System.Data.Linq.Mapping
Imports System.Data.Entity.Core
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.FileIO.FileSystem
Imports System.Reflection

''' <summary>
''' The API interface wrapper of the SQLite.(SQLite的存储引擎的接口)
''' </summary>
''' <remarks></remarks>
Public Class SQLProcedure : Implements System.IDisposable

    Dim URLConnection As DbConnection

    ''' <summary>
    ''' Get the filename of the connected SQLite database file.(返回数据库文件的文件位置)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property URL As String

    Protected Sub New()
    End Sub

    ''' <summary>
    ''' Create a table in current database file for the specific table schema <para>T</para> . 
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateTableFor(Of T As Class)() As String
        Return CreateTableFor(TypeInfo:=GetType(T))
    End Function

    ''' <summary>
    ''' Create a table in current database file for the specific table schema <paramref name="TypeInfo"></paramref> . 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateTableFor(TypeInfo As System.Type) As String
        Dim TableSchema = New TableSchema(TypeInfo)
        Dim SQL As String = SchemaCache.CreateTableSQL(TableSchema.DatabaseFields, TableSchema.TableName)
        Call Me.Execute(SQL)

        Dim p As Integer = Me.Load(Of TableDump).Count + 1
        Dim dumpInfo As TableDump() =
            LinqAPI.Exec(Of TableDump) <=
            From Field As SchemaCache
            In TableSchema
            Let ipk As Integer = If(Field.FieldEntryPoint.IsPrimaryKey, 1, 0)
            Select New TableDump With {
                .Guid = p.MoveNext,
                .DbType = Field.DbType,
                .FieldName = Field.DbFieldName,
                .IsPrimaryKey = ipk,
                .TableName = TableSchema.TableName
            } ' 由于需要生成递增的Guid，故而这里不能再使用并行拓展了

        TableSchema = New TableSchema(GetType(TableDump))

        For Each field As TableDump In dumpInfo
            Call Me.Insert(TableSchema, field)
        Next

        Return SQL
    End Function

    ''' <summary>
    ''' Get a value to knows that wether the target table is exists in the database or not.
    ''' (判断某一个数据表是否存在)
    ''' </summary>
    ''' <param name="TableName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ExistsTable(TableName As String) As Boolean
        Dim SQL As String = "SELECT COUNT(*) FROM sqlite_master where type='table' and name='" & TableName & "';"

        Using EXECommand As IDbCommand = Me.URLConnection.CreateCommand

            EXECommand.Connection = Me.URLConnection
            EXECommand.CommandText = SQL
            Return CType(EXECommand.ExecuteScalar, Integer) > 0
        End Using
    End Function

    ''' <summary>
    ''' Delete the target table.
    ''' </summary>
    ''' <param name="SchemaInfo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteTable(SchemaInfo As Type) As Boolean
        Dim TbName As String = Reflector.GetTableName(SchemaInfo)
        Call Execute("DROP TABLE '" & TbName & "';")
        Return True
    End Function

    ''' <summary>
    ''' Delete the target table.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteTable(TableName As String) As Boolean
        Call Execute("DROP TABLE '" & TableName & "';")
        Return True
    End Function

    ''' <summary>
    ''' Get a value to knows that wether the target table is exists in the database or not.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ExistsTable(Schema As Type) As Boolean
        Dim TbName As String = Reflector.GetTableName(Schema)
        Return ExistsTable(TbName)
    End Function

    Public Function ExistsTableForType(Of T As Class)() As Boolean
        Dim TbName As String = Reflector.GetTableName(Of T)()
        Return ExistsTable(TbName)
    End Function

    Const FILEIO_EXCEPTION As String = "Maybe we have a wrong file place for ""file:///{0}"", shuch as no sufficient privilege or a readonly place."

    ''' <summary>
    ''' SQLite 连接字符串
    ''' </summary>
    Const SQLiteCnn As String = "Data Source=""{0}"";Pooling=true;FailIfMissing=false"

    ''' <summary>
    ''' Establishing the protocol of the SQLite connection between you program and the database file "<paramref name="url"></paramref>".
    ''' </summary>
    ''' <param name="url">The path of the SQLite database file.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CreateSQLTransaction(url As String) As SQLProcedure
        url = FileIO.FileSystem.GetFileInfo(url).FullName.Replace("\", "/")

        Try

            If Not url.FileExists Then
                Call FileIO.FileSystem.CreateDirectory(url.ParentPath)
                Call SQLiteConnection.CreateFile(databaseFileName:=url)
            End If
        Catch ex As Exception
            ex = New Exception(String.Format(FILEIO_EXCEPTION, url), ex)
            Throw ex
        End Try

        Dim URLConnection As DbConnection
        Dim cnn As String = String.Format(SQLiteCnn, url)
        URLConnection = New SQLiteConnection(cnn)
        URLConnection.Open()

        Dim DBI As SQLProcedure = New SQLProcedure With {._URL = url, .URLConnection = URLConnection}
        Dim DumpInfoSchema As Type = GetType(TableDump)

        If Not DBI.ExistsTable(DumpInfoSchema) Then
            Dim TableSchema = New TableSchema(DumpInfoSchema)
            Dim SQL As String = SchemaCache.CreateTableSQL(TableSchema.DatabaseFields, TableSchema.TableName)
            Call DBI.Execute(SQL)
        End If

        Return DBI
    End Function

    Public Overrides Function ToString() As String
        Return "file:///" & _URL
    End Function

    ''' <summary>
    ''' Batch execute a SQL collection as a SQL transaction. 
    ''' </summary>
    ''' <param name="SQL"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ExecuteTransaction(ParamArray SQL As String()) As Boolean
        Dim sb As New StringBuilder(1024)

        Call sb.AppendLine("BEGIN IMMEDIATE")

        For Each Line As String In SQL
            Call sb.AppendLine(Line)
        Next

        Call sb.AppendLine("COMMIT")

        Try
            Call Execute(sb.ToString)
        Catch ex As Exception
            Call App.LogException(ex)
            Return False
        End Try

        Return True
    End Function

    ''' <summary>
    ''' If the SQL is a SELECT statement, then this function returns a table object, if not then it returns nothing.
    ''' </summary>
    ''' <param name="SQL"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Execute(SQL As String) As DbDataReader
        Using execRun As IDbCommand = URLConnection.CreateCommand

            Dim type As String = SQL.Trim.Split.First.ToLower

            execRun.Connection = URLConnection
            execRun.CommandText = SQL

            Try
                If InStr(ModifyTokens, type, CompareMethod.Text) > 0 Then
                    Dim i As Integer = execRun.ExecuteNonQuery()

                    If i = 0 Then
                        Throw New Exception("No data row was effected!")
                    End If
                ElseIf InStr("drop, create", type, CompareMethod.Text) > 0 Then
                    Call execRun.ExecuteNonQuery()
                Else
                    Return execRun.ExecuteReader()
                End If
            Catch ex As Exception
                Dim msg As String = SQL_EXECUTE_ERROR & SQL
                Dim trace As String =
                    MethodBase.GetCurrentMethod.GetFullName

                ex = New EntitySqlException(msg, ex)
                Call App.LogException(ex, trace)

                Throw ex
            End Try
        End Using

        Return Nothing
    End Function

    Const ModifyTokens As String = "insert, delete, update"

    Const SQL_EXECUTE_ERROR As String =
 _
        "Error occurred while trying to execute sql:  " & vbCrLf &
        "      -----> "

    ''' <summary>
    ''' If the SQL is a SELECT statement, then this function returns a table object, if not then it returns nothing.
    ''' </summary>
    ''' <param name="SQL"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Execute(SQL As String, ParamArray argvs As String()) As DbDataReader
        Return Me.Execute(String.Format(SQL, argvs))
    End Function

    Public Sub CloseTransaction()
        Call Me.URLConnection.Close()
        Call Me.URLConnection.Dispose()
    End Sub

    ''' <summary>
    ''' Export the data dump in the format of a INSERT INTO SQL statement for transfer the data in this database into another database.
    ''' </summary>
    ''' <typeparam name="Table">The table schema of the target table which will be transfer.</typeparam>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateSQLDump(Of Table As Class)() As String
        Dim sb As StringBuilder = New StringBuilder(2048)
        Dim SchemaCache As SchemaCache() = Reflector.__getSchemaCache(Of Table)()
        Dim TableName As String = Reflector.GetTableName(Of Table)()
        Dim SQL As String =
            [Interface].SchemaCache.CreateTableSQL(SchemaCache, TableName)

        Call sb.AppendLine("/* CREATE_TABLE_SCHEMA_INFORMATION */")
        Call sb.AppendLine(SQL)
        Call sb.AppendLine()
        Call sb.AppendLine("/* DATA_STORAGES */")

        Dim LQuery As String() =
            LinqAPI.Exec(Of String) <= From ItemRowObject As Table
                                       In Me.Load(Of Table)()
                                       Select [Interface].SchemaCache.CreateInsertSQL(
                                           SchemaCache,
                                           ItemRowObject,
                                           TableName)

        For Each Line As String In LQuery
            Call sb.AppendLine(Line)
        Next

        Call sb.AppendLine()
        Call sb.AppendLine("/* END_OF_SQL_DUMP */")

        Return sb.ToString
    End Function

    ''' <summary>
    ''' 转储整个数据库
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DbDump(DumpFile As String) As Boolean
        Dim Tables = (From dump As TableDump
                      In Me.Load(Of TableDump)()
                      Select dump
                      Group By dump.TableName Into Group).ToArray

        Call FileIO.FileSystem.CreateDirectory(DumpFile.ParentPath)

        For Each Table In Tables
            Dim SQLDump As String = ___SQLDump(Table.Group.ToArray)
            Call WriteAllText(DumpFile, SQLDump, append:=True)
        Next

        Return True
    End Function

    ''' <summary>
    ''' Export the data dump in the format of a INSERT INTO SQL statement for transfer the data in this database into another database.
    ''' </summary>
    ''' <param name="Table">The table schema of the target table which will be transfer.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ___SQLDump(Table As TableDump()) As String
        Dim TableName As String = Table(Scan0).TableName
        Dim SQL As String = [Interface].SchemaCache.CreateTableSQL(Table)
        Dim sb As New StringBuilder(2048)

        Call sb.AppendLine("/* CREATE_TABLE_SCHEMA_INFORMATION */")
        Call sb.AppendLine(SQL)
        Call sb.AppendLine()
        Call sb.AppendLine($"/* DATA_STORAGES  ""{TableName}"" */")

        Dim DbReader = Me.Execute("SELECT * FROM '{0}';", TableName)
        Dim SchemaCache = (From tField As TableDump
                           In Table
                           Select Field = tField,
                               p = DbReader.GetOrdinal(tField.FieldName)).ToArray
        Dim array As String() =
            LinqAPI.Exec(Of String) <= From p
                                       In SchemaCache
                                       Select p.Field.FieldName
        Dim values As String
        Dim columns As String = String.Join(", ", array)

        Do While DbReader.Read
            array =
                LinqAPI.Exec(Of String) <= From p In SchemaCache
                                           Let value As Object = DbReader.GetValue(p.p)
                                           Let s As String = Scripting.ToString(value)
                                           Select $"'{s}'"
            values = String.Join(", ", array)

            Dim InsertSQL As String = $"INSERT INTO '{TableName}' ({columns}) VALUES ({values}) ;"
            Call sb.AppendLine(InsertSQL)
        Loop

        Call sb.AppendLine()
        Call sb.AppendLine($"/* END_OF_SQL_DUMP  ""{TableName}"" */")

        Return sb.ToString
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                Call Me.CloseTransaction()
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose( disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose( disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class
