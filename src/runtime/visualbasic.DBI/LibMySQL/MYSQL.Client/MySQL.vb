#Region "Microsoft.VisualBasic::f4767ff3e107e7318d5e28534ddd55ec, ..\LibMySQL\MYSQL.Client\MySQL.vb"

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

Imports System.Threading
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Serialization.JSON
Imports MySql.Data.MySqlClient

''' <summary>
''' MySql database server connection module.
''' (与MySql数据库服务器之间的通信操作的封装模块)
''' </summary>
''' <remarks></remarks>
Public Class MySQL : Implements IDisposable

    ''' <summary>
    ''' A error occurred during the execution of a sql command or transaction.
    ''' (在执行SQL命令或者提交一个事务的时候发生了错误) 
    ''' </summary>
    ''' <param name="Ex">
    ''' The detail information of the occurred error.
    ''' (所发生的错误的详细信息)
    ''' </param>
    ''' <remarks></remarks>
    Public Event ThrowException(Ex As Exception, SQL As String)

    Dim _reflector As Reflection.DbReflector

    ''' <summary>
    ''' A Formatted connection string using for the connection established to the database server. 
    ''' </summary>
    ''' <remarks></remarks>
    Public ReadOnly Property UriMySQL As ConnectionUri

    Public Overrides Function ToString() As String
        Return UriMySQL.GetJson
    End Function

    ''' <summary>
    ''' Connect to the database server using a assigned mysql connection helper object. The function returns the ping value of the client to the MYSQL database server.
    ''' (使用一个由用户所指定参数的连接字符串生成器来打开一个对服务器的连接，之后返回客户端对数据库服务器的ping值) 
    ''' </summary>
    ''' <param name="MySQLConnection"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Connect(MySQLConnection As ConnectionUri, Optional OnCreateSchema As Boolean = False) As Double
        Dim TempName As String = MySQLConnection.Database

        If OnCreateSchema Then
            MySQLConnection.Database = ""
            _UriMySQL = MySQLConnection
            Call CreateSchema(Name:=MySQLConnection.Database)
            MySQLConnection.Database = TempName
        End If

        Return Connect(ConnectionString:=MySQLConnection.GetConnectionString)
    End Function

    Public Sub New(strUri As String)
        Dim uri As ConnectionUri = strUri
        Call Connect(uri)
    End Sub

    Public Sub New()
    End Sub

    Sub New(uri As ConnectionUri)
        Call Connect(uri)
    End Sub

    Private Function CreateSchema(Name As String) As Boolean
        Const CREATE_SCHEMA As String = "CREATE DATABASE /*!32312 IF NOT EXISTS*/ {0};"
        Return Me.Execute(String.Format(CREATE_SCHEMA, Name)) = 0
    End Function

    ''' <summary>
    ''' Connect to the database server using a assigned mysql connection string.
    ''' (使用一个由用户所制定的连接字符串连接MySql数据库服务器) 
    ''' </summary>
    ''' <param name="ConnectionString"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Connect(ConnectionString As String) As Double
        _UriMySQL = ConnectionString
        _reflector = New Reflection.DbReflector(_UriMySQL.GetConnectionString)

        Return Ping()
    End Function

    ''' <summary>
    ''' Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
    ''' </summary>
    ''' <returns></returns>
    ''' <param name="SQL">请手工添加 limit 1 限定</param>
    Public Function ExecuteScalar(Of T As Class)(SQL As String) As T
        Using MySQL As MySqlConnection = New MySqlConnection(_UriMySQL)
            Dim MySqlCommand As MySqlCommand = New MySqlCommand(SQL)

            MySqlCommand.Connection = MySQL
            Try
                Call MySQL.Open()

                Dim Result = Fetch(SQL)
                Dim Reader = Result.CreateDataReader
                Dim value As T = Reflection.DbReflector.ReadFirst(Of T)(Reader)

                Return value
            Catch ex As Exception
                ex = __throwExceptionHelper(ex, SQL, False)
                Call ex.PrintException
                Call App.LogException(ex)

                Return Nothing
            Finally
                Call MySQL.Close()
            End Try
        End Using
    End Function

    ''' <summary>
    ''' 执行聚合函数并返回值
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="SQL"></param>
    ''' <returns></returns>
    Public Function ExecuteAggregate(Of T)(SQL As String) As T
        Try
            Return __executeAggregate(Of T)(SQL)
        Catch ex As Exception
            ex = __throwExceptionHelper(ex, SQL, False)
            Call ex.PrintException
            Call App.LogException(ex)

            Return Nothing
        Finally

        End Try
    End Function

    Private Function __executeAggregate(Of T)(SQL As String) As T
        Dim Result As DataSet = Fetch(SQL)
        Dim Reader As DataTableReader = Result.CreateDataReader

        Call Reader.Read()

        Dim ObjValue As Object = Reader.GetValue(Scan0)
        Dim value As T = CType(ObjValue, T)

        Return value
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="[where]">只需要给出条件WHERE表达式即可，函数会自动生成SQL查询语句</param>
    ''' <returns></returns>
    Public Function ExecuteScalarAuto(Of T As Class)([where] As String) As T
        Dim Tbl As Reflection.DbAttributes.TableName =
            Reflection.DbAttributes.TableName.GetTableName(Of T)
        Dim SQL As String = $"SELECT * FROM `{Tbl.Database}`.`{Tbl.Name}` WHERE {where} LIMIT 1;"
        Return ExecuteScalar(Of T)(SQL)
    End Function

    Public Function ExecuteDataset(SQL As String) As MySqlDataReader
        Using MySQL As MySqlConnection = New MySqlConnection(_UriMySQL)
            Dim MySqlCommand As MySqlCommand = New MySqlCommand(SQL)

            MySqlCommand.Connection = MySQL
            Try
                Call MySQL.Open()
                Return MySqlCommand.ExecuteReader

            Catch ex As Exception
                ex = __throwExceptionHelper(ex, SQL, False)
                Call ex.PrintException
                Call App.LogException(ex)

                Return Nothing
            End Try

            MySQL.Close()
        End Using
    End Function

    ''' <summary>
    ''' Execute a DML/DDL sql command and then return the row number that the row was affected 
    ''' by this command, and you should open a connection to a database server before you call 
    ''' this function. 
    ''' (执行一个DML/DDL命令并且返回受此命令的执行所影响的行数，你应该在打开一个数据库服务器的连接之
    ''' 后调用本函数，执行SQL语句发生错误时会返回负数)
    ''' </summary>
    ''' <param name="SQL">DML/DDL sql command, not a SELECT command(DML/DDL 命令，而非一个SELECT语句)</param>
    ''' <returns>
    ''' Return the row number that was affected by the DML/DDL command, if the databse 
    ''' server connection is interrupt or errors occurred during the executes, this 
    ''' function will return a negative number.
    ''' (返回受DML/DDL命令所影响的行数，假若数据库服务器断开连接或者在命令执行的期间发生错误，
    ''' 则这个函数会返回一个负数)
    ''' </returns>
    ''' <remarks></remarks>
    Public Function Execute(SQL As String, Optional throwExp As Boolean = False) As Integer
        Using MySQL As New MySqlConnection(_UriMySQL)
            Dim MySqlCommand As New MySqlCommand(SQL) With {
                .Connection = MySQL
            }

            Try
                MySQL.Open()
                Return MySqlCommand.ExecuteNonQuery
            Catch ex As Exception
                If throwExp Then
                    __throwExceptionHelper(ex, SQL, True)
                Else
                    ex = __throwExceptionHelper(ex, SQL, False)
                    Call ex.PrintException
                    Call App.LogException(ex)
                End If
                Return -1
            Finally
                MySQL.Close()
            End Try
        End Using
    End Function

    Public Function ForEach(Of T)(SQL As String, Invoke As Action(Of T)) As String
        Dim Err As String = ""
        Call _reflector.ForEach(Of T)(SQL, Invoke, Err)
        Return Err
    End Function

    ''' <summary>
    ''' Execute a 'SELECT' query command and then returns the query result of this sql command.
    ''' (执行一个'SELECT'查询命令之后返回本查询命令的查询结果) 
    ''' </summary>
    ''' <param name="SQL"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Fetch(SQL As String) As DataSet
        Dim MySql As MySqlConnection = New MySqlConnection(_UriMySQL)
        Dim MySqlCommand As MySqlCommand = New MySqlCommand(SQL)
        Dim Adapter As New MySqlDataAdapter()
        Dim DataSet As DataSet = New DataSet

        MySqlCommand.Connection = MySql
        Adapter.SelectCommand = MySqlCommand
        Try
            Adapter.Fill(DataSet)
        Catch ex As Exception
            ex = __throwExceptionHelper(ex, SQL, False)
            Call ex.PrintException
            Call App.LogException(ex)
            Return Nothing
        End Try

        Return DataSet
    End Function

    Public Function Query(Of T As Class)(SQL As String, Optional Parallel As Boolean = False, Optional throwExp As Boolean = True) As T()
        Dim Err As String = ""
        Dim Table As T() =
            If(Parallel,
            _reflector.ParallelQuery(Of T)(SQL, GetErr:=Err),
            _reflector.Query(Of T)(SQL, GetErr:=Err))

        If Table Is Nothing Then
            If throwExp Then
                Dim ex As New Exception(SQL)
                ex = New Exception(Err, ex)
                Throw ex
            Else
                Return Nothing
            End If
        Else
            Return Table.ToArray
        End If
    End Function

    Public Function CreateQuery(SQL As String) As MySqlDataReader
        Dim MySql As MySqlConnection = New MySqlConnection(_UriMySQL) '[ConnectionString] is a compiled mysql connection string from our class constructor.
        Dim MySqlCommand As MySqlCommand = New MySqlCommand(SQL, MySql)

        Try
            MySql.Open()
            Return MySqlCommand.ExecuteReader(CommandBehavior.CloseConnection)
        Catch ex As Exception
            ex = __throwExceptionHelper(ex, SQL, False)
            Call App.LogException(ex)
            Call ex.PrintException
            Return Nothing
        Finally

        End Try
    End Function

#Region ""
    Public Function ExecUpdate(SQL As Oracle.LinuxCompatibility.MySQL.SQLTable, Optional throwExp As Boolean = False) As Boolean
        Dim s_SQL As String = SQL.GetUpdateSQL
        Return Execute(s_SQL, throwExp) > 0
    End Function

    Public Function ExecInsert(SQL As Oracle.LinuxCompatibility.MySQL.SQLTable, Optional throwExp As Boolean = False) As Boolean
        Dim s_SQL As String = SQL.GetInsertSQL
#If DEBUG Then
        Call s_SQL.__DEBUG_ECHO
#End If
        Dim success As Boolean = Execute(s_SQL, throwExp) > 0
        Return success
    End Function

    Public Function ExecDelete(SQL As Oracle.LinuxCompatibility.MySQL.SQLTable, Optional throwExp As Boolean = False) As Boolean
        Dim s_SQL As String = SQL.GetDeleteSQL
        Return Execute(s_SQL, throwExp) > 0
    End Function

    Public Function ExecReplace(SQL As SQLTable, Optional throwExp As Boolean = False) As Boolean
        Return Execute(SQL.GetReplaceSQL, throwExp) > 0
    End Function
#End Region

    Public Function CommitInserts(Transaction As IEnumerable(Of SQLTable), Optional ByRef ex As Exception = Nothing) As Boolean
        Dim SQL As String = Transaction.ToArray(Function(x) x.GetInsertSQL).JoinBy(vbLf)
        Return CommitTransaction(SQL, ex)
    End Function

    ''' <summary>
    ''' Commit a transaction command collection to the database server and then return the 
    ''' result that this transaction is commit successfully or not. 
    ''' (向数据库服务器提交一个事务之后返回本事务是否被成功提交)
    ''' </summary>
    ''' <param name="Transaction"></param>
    ''' <returns>
    ''' Return the result that this transaction is commit succeedor not.
    ''' (返回本事务是否被成功提交至数据库服务器)
    ''' </returns>
    ''' <remarks></remarks>
    Public Function CommitTransaction(Transaction As String, Optional ByRef excep As Exception = Nothing) As Boolean
        Using MyConnection As New MySqlConnection(_UriMySQL)
            MyConnection.Open()

            Dim MyCommand As MySqlCommand = MyConnection.CreateCommand()
            Dim MyTrans As MySqlTransaction

            ' Start a local transaction
            MyTrans = MyConnection.BeginTransaction()

            ' Must assign both transaction object and connection
            ' to Command object for a pending local transaction

            MyCommand.Connection = MyConnection
            MyCommand.Transaction = MyTrans

            Try
                MyCommand.CommandText = Transaction
                MyCommand.ExecuteNonQuery()
                MyTrans.Commit()

                Return True
            Catch e As Exception
                Try
                    MyTrans.Rollback()
                Catch ex As MySqlException
                    e = New Exception(__throwExceptionHelper(ex, Transaction, False).ToString, e)
                End Try
                excep = e
                Return False
            Finally
                MyConnection.Close()
            End Try
        End Using
    End Function

    Private Function __throwExceptionHelper(Ex As Exception, SQL As String, throwExp As Boolean) As Exception
        Dim url As New ConnectionUri(UriMySQL)
        url.Password = "********"
        Ex = New Exception(url.GetJson, Ex)
        Ex = New Exception(SQL, Ex)

        If throwExp Then
            Throw Ex
        Else
            Return Ex
        End If
    End Function

    ''' <summary>
    ''' Test the connection of the client to the mysql database server and then 
    ''' return the communication delay time between the client and the server. 
    ''' This function should be call after you connection to a database server.
    ''' (测试客户端和MySql数据库服务器之间的通信连接并且返回二者之间的通信延时。
    ''' 这个函数应该在你连接上一个数据库服务器之后进行调用，-1值表示客户端与服务器之间通信失败.)
    ''' </summary>
    ''' <returns>当函数返回一个负数的时候，表明Ping操作失败，即无数据库服务器连接</returns>
    ''' <remarks></remarks>
    Public Function Ping() As Double
        Dim Flag As Boolean
        Dim DelayTime As Double

        Using MySql = New MySqlConnection(_UriMySQL)
            Try
                MySql.Open()
            Catch ex As Exception
                ex = __throwExceptionHelper(ex, "null", False)
                Call ex.PrintException
                Call App.LogException(ex)
                Return -1
            End Try

            Dim DelayTimer = Stopwatch.StartNew
            Flag = MySql.Ping
            DelayTime = DelayTimer.ElapsedMilliseconds

            MySql.Close()
        End Using

        If Flag Then
            Return DelayTime
        Else
            Return -1
        End If
    End Function

    Public ReadOnly Property Version As String
        Get
            Using MySQL As MySqlConnection = New MySqlConnection(_UriMySQL)
                Return MySQL.ServerVersion
            End Using
        End Get
    End Property

    ''' <summary>
    ''' Open a mysql connection using a specific connection string
    ''' </summary>
    ''' <param name="strUri">The mysql connection string</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Widening Operator CType(strUri As String) As MySQL
        Dim uri As ConnectionUri = strUri
        Dim DBIClient As MySQL = New MySQL With {
            ._UriMySQL = uri,
            ._reflector = New Reflection.DbReflector(uri.GetConnectionString)
        }
        Return DBIClient
    End Operator

    ''' <summary>
    ''' Open a  mysql  connection using a connection helper object
    ''' </summary>
    ''' <param name="uri_obj">The connection helper object</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Widening Operator CType(uri_obj As ConnectionUri) As MySQL
        Return New MySQL With {
            ._UriMySQL = uri_obj,
            ._reflector = New Reflection.DbReflector(uri_obj.GetConnectionString)
        }
    End Operator

    ''' <summary>
    ''' ``mysql.Connect(cnn)``
    ''' </summary>
    ''' <param name="mysql"></param>
    ''' <param name="cnn"></param>
    ''' <returns></returns>
    Public Shared Operator <=(mysql As MySQL, cnn As ConnectionUri) As Double
        Return mysql.Connect(cnn)
    End Operator

    Public Shared Operator >=(mysql As MySQL, cnn As ConnectionUri) As Double
        Throw New NotSupportedException
    End Operator

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
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

