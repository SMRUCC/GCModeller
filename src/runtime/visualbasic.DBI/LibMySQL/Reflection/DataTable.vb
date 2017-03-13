#Region "Microsoft.VisualBasic::fb96062edc809ea9903c1ee55ded9ab5, ..\LibMySQL\Reflection\DataTable.vb"

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

Imports System.Reflection
Imports System.Text
Imports Microsoft.VisualBasic
Imports MySql.Data.MySqlClient
Imports Oracle.LinuxCompatibility.MySQL.Reflection.Schema
Imports Oracle.LinuxCompatibility.MySQL.Reflection.SQL

Namespace Reflection

    ''' <summary>
    ''' A table object of a specific table schema that mapping a table object in the mysql database.
    ''' (一个映射到MYSQL数据库中的指定的表之上的表对象)
    ''' </summary>
    ''' <typeparam name="Schema">
    ''' The table shcema which define on the custom attribut of a class.
    ''' (定义于一个类之中的自定义属性的表结构)
    ''' </typeparam>
    ''' <remarks></remarks>
    Public Class DataTable(Of Schema) : Implements IDisposable

        ''' <summary>
        ''' 'DELETE' sql text generator of a record that type of schema.
        ''' </summary>
        ''' <remarks></remarks>
        Dim DeleteSQL As SQL.Delete(Of Schema)
        ''' <summary>
        ''' 'INSERT' sql text generator of a record that type of schema.
        ''' </summary>
        ''' <remarks></remarks>
        Dim InsertSQL As SQL.Insert(Of Schema)
        ''' <summary>
        ''' 'UPDATE' sql text generator of a record that type of schema.
        ''' </summary>
        ''' <remarks></remarks>
        Dim UpdateSQL As SQL.Update(Of Schema)

        ''' <summary>
        ''' The sql transaction that will be commit to the mysql database.
        ''' (将要被提交至MYSQL数据库之中的SQL事务集)
        ''' </summary>
        ''' <remarks></remarks>
        Dim Transaction As StringBuilder = New StringBuilder(2048)
        Dim WithEvents MySQL As MySQL

        ''' <summary>
        ''' The structure definition information which was parsed from the custom attribut on a class object.
        ''' (从一个类对象上面的自定义属性之中解析出来的表结构信息)
        ''' </summary>
        ''' <remarks></remarks>
        Dim TableSchema As Reflection.Schema.Table
        Dim p As Long

        ''' <summary>
        ''' DataSet of the table in the database.
        ''' (数据库的表之中的数据集)
        ''' </summary>
        ''' <remarks></remarks>
        Friend _listData As New List(Of Schema)(capacity:=1024)

        ''' <summary>
        ''' The error information that come from MYSQL database server.
        ''' (来自于MYSQL数据库服务器的错误信息)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ErrorMessage As String

        ''' <summary>
        ''' DataSet of the table in the database. Do not edit the data directly from this property...
        ''' (数据库的表之中的数据集，请不要直接在这个属性之上修改数据)
        ''' </summary>
        ''' <remarks></remarks>
        ReadOnly Property ListData As List(Of Schema)
            Get
                Return _listData
            End Get
        End Property

        Friend Sub New()
            TableSchema = GetType(Schema) 'Start reflection and parsing the table structure information.

            DeleteSQL = TableSchema  'Initialize the sql generator using the table structure information that parse from the custom attribut of a class object.
            InsertSQL = TableSchema
            UpdateSQL = TableSchema
        End Sub

        ''' <summary>
        ''' Execute ``CREATE TABLE`` sql.
        ''' </summary>
        ''' <returns></returns>
        Public Function Create() As Boolean
            Dim SQL As String = CreateTableSQL.FromSchema(TableSchema)
#If DEBUG Then
            Console.WriteLine(SQL)
#End If
            Return MySQL.Execute(SQL)
        End Function

        ''' <summary>
        ''' Get a specific record in the dataset by compaired the UNIQUE_INDEX field value.
        ''' (通过值唯一的索引字段来获取一个特定的数据记录)
        ''' </summary>
        ''' <param name="Record"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetHandle(Record As Schema) As Schema
            Dim [String] As String = TableSchema.IndexProperty.GetValue(Record, Nothing).ToString 'Get the Index field value
            Dim LQuery As IEnumerable(Of Schema) = From schema As Schema
                                                   In _listData
                                                   Let str As String = TableSchema.IndexProperty.GetValue(schema, Nothing).ToString
                                                   Where String.Equals([String], str)
                                                   Select schema ' Use LINQ and index value find out the target item 
            Return LQuery.First  'return the item handle
        End Function

        ''' <summary>
        ''' Delete a record in the table. Please notice that, in order to decrease the usage of CPU and networking traffic, the 
        ''' change is not directly affect on the database server, it will be store as a delete sql in the memory and accumulated 
        ''' as a transaction, the change of the database will not happen until you call the commit method to make this change 
        ''' permanently in the database.
        ''' (删除表中的一条记录。请注意：为了减少服务器的计算资源和网络流量的消耗，在使用本模块对数据库作出修改的时候，更改并不会直接提
        ''' 交至数据库之中的，而是将修改作为一条SQL语句存储下来并对这些命令进行积累作为一个事务存在，即数据库不会受到修改的影响直到你将
        ''' 本事务提交至数据库服务器之上)
        ''' </summary>
        ''' <param name="Record"></param>
        ''' <remarks></remarks>
        Public Sub Delete(Record As Schema)
            Dim SQL As String = DeleteSQL.Generate(Record)

            Call _listData.Remove(Record)
            Call Transaction.AppendLine(SQL)
        End Sub

        ''' <summary>
        ''' Insert a record in the table. Please notice that, in order to decrease the usage of CPU and networking traffic, the 
        ''' change is not directly affect on the database server, it will be store as a insert sql in the memory and accumulated 
        ''' as a transaction, the change of the database will not happen until you call the commit method to make this change 
        ''' permanently in the database.
        ''' (向表中插入一条新记录。请注意：为了减少服务器的计算资源和网络流量的消耗，在使用本模块对数据库作出修改的时候，更改并不会直接提
        ''' 交至数据库之中的，而是将修改作为一条SQL语句存储下来并对这些命令进行积累作为一个事务存在，即数据库不会受到修改的影响直到你将
        ''' 本事务提交至数据库服务器之上)
        ''' </summary>
        ''' <param name="Record"></param>
        ''' <remarks></remarks>
        Public Overloads Sub Insert(Record As Schema)
            Dim SQL As String = InsertSQL.Generate(Record)

            Call _listData.Add(Record)
            Call Transaction.AppendLine(SQL)
        End Sub

        ''' <summary>
        ''' Update a record in the table. Please notice that, in order to decrease the usage of CPU and networking traffic, the 
        ''' change is not directly affect on the database server, it will be store as a update sql in the memory and accumulated 
        ''' as a transaction, the change of the database will not happen until you call the commit method to make this change 
        ''' permanently in the database.
        ''' (修改表中的一条记录。请注意：为了减少服务器的计算资源和网络流量的消耗，在使用本模块对数据库作出修改的时候，更改并不会直接提
        ''' 交至数据库之中的，而是将修改作为一条SQL语句存储下来并对这些命令进行积累作为一个事务存在，即数据库不会受到修改的影响直到你将
        ''' 本事务提交至数据库服务器之上)
        ''' </summary>
        ''' <param name="Record"></param>
        ''' <remarks></remarks>
        Public Sub Update(Record As Schema)
            Dim SQL As String = UpdateSQL.Generate(Record)
            Dim OldRecord As Schema = GetHandle(Record)
            Dim Handle As Integer = _listData.IndexOf(OldRecord)

            _listData(Handle) = Record

            Call Transaction.AppendLine(SQL)
        End Sub

        ''' <summary>
        ''' Load the data from database server. Please notice that: every time you call this function, the transaction will be commit to the database server in.
        ''' (从数据库服务器之中加载数据，请注意：每一次加载数据都会将先前的所积累下来的事务提交至数据库服务器之上)
        ''' </summary>
        ''' <param name="Count">
        ''' The count of the record that will be read from the server. Notice: Zero or negative is stands for 
        ''' load all records in the database.
        ''' (从数据库中读取的记录数目。请注意：值0和负数值都表示加载数据库的表中的所有数据)
        ''' </param>
        ''' <remarks></remarks>
        Public Sub Fetch(Optional Count As Integer = -1)
            Call Me.Commit()  '

            If Count <= 0 Then  'Load all data when count is zero or negative.
                _listData = Query($"SELECT * FROM {TableSchema.TableName};")
                p = _listData.Count
            Else
                Dim NewData As List(Of Schema)
                NewData = Query($"SELECT * FROM {TableSchema.TableName} LIMIT {p},{Count};")
                _listData.AddRange(NewData)
                p += Count  'pointer move next block.
            End If
        End Sub

        ''' <summary>
        ''' Query a data table using Reflection.(使用反射机制来查询一个数据表)
        ''' </summary>
        ''' <param name="SQL">Sql 'SELECT' query statement.(Sql 'SELECT' 查询语句)</param>
        ''' <returns>The target data table.(目标数据表)</returns>
        ''' <remarks></remarks>
        Public Function Query(SQL As String) As List(Of Schema)
            Dim MySql As MySqlConnection = New MySqlConnection(Me.MySQL.UriMySQL.GetConnectionString) '[ConnectionString] is a compiled mysql connection string from our class constructor.
            Dim MySqlCommand As MySqlCommand = New MySqlCommand(SQL, MySql)
            Dim Reader As Global.MySql.Data.MySqlClient.MySqlDataReader = Nothing
            Dim NewTable As New List(Of Schema)

            Try
                MySql.Open()
                Reader = MySqlCommand.ExecuteReader(CommandBehavior.CloseConnection)

                Dim Ordinals = (From Field As Field In TableSchema.Fields 'This table schema is created from the previous reflection operations
                                Let Ordinal As Integer = Reader.GetOrdinal(Field.FieldName)
                                Where Ordinal >= 0
                                Select Ordinal, Field, TypeCast = Function(value As Object) Field.DataType.TypeCasting(value)).ToArray

                While Reader.Read 'When we call this function, the pointer will move to next line in the table automatically.  
                    Dim FillObject = Activator.CreateInstance(TableSchema.SchemaType) 'Create a instance of specific type: our record schema. 

                    For Each Field In Ordinals  'Scan all of the fields in the field list and get the field value.
                        Call Field.Field.PropertyInfo.SetValue(FillObject, Field.TypeCast(Reader.GetValue(Field.Ordinal)), Nothing)
                    Next

                    NewTable.Add(FillObject)
                End While
                Return NewTable 'Return the new table that we get
            Catch ex As Exception
                Me.ErrorMessage = ex.ToString
                Call Console.WriteLine(ex.ToString)
            Finally
                If Not Reader Is Nothing Then Reader.Close()
                If Not MySqlCommand Is Nothing Then MySqlCommand.Dispose()
                If Not MySql Is Nothing Then MySql.Dispose()
            End Try
            Return Nothing
        End Function

        ''' <summary>
        ''' Commit the transaction to the database server to make the change permanently.
        ''' (将事务集提交至数据库服务器之上以永久的修改数据库) 
        ''' </summary>
        ''' <returns>The transaction commit is successfully or not.(事务集是否被成功提交)</returns>
        ''' <remarks></remarks>
        Public Function Commit() As Boolean
            If Transaction.Length = 0 Then Return True 'No transaction will be commit to database server.

            Dim ex As Exception = Nothing

            If Not MySQL.CommitTransaction(Transaction.ToString, ex) Then 'the transaction commit failure.
                ErrorMessage = ex.ToString
                Return False
            End If
            Call Transaction.Clear()  'Clean the previous transaction when the transaction commit is successfully. 
            Return True
        End Function

        ''' <summary>
        ''' Convert the mapping object to a dataset
        ''' </summary>
        ''' <param name="schema"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Narrowing Operator CType(schema As DataTable(Of Schema)) As List(Of Schema)
            Return schema.ListData
        End Operator

        ''' <summary>
        ''' Initialize the mapping from a connection object
        ''' </summary>
        ''' <param name="uri"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Widening Operator CType(uri As ConnectionUri) As DataTable(Of Schema)
            Return New DataTable(Of Schema) With {.MySQL = uri}
        End Operator

        ''' <summary>
        ''' Initialize the mapping from a connection string
        ''' </summary>
        ''' <param name="uri"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Widening Operator CType(uri As String) As DataTable(Of Schema)
            Return New DataTable(Of Schema) With {.MySQL = uri}
        End Operator

        ''' <summary>
        ''' Initialize the mapping from a connection string
        ''' </summary>
        ''' <param name="xml"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Widening Operator CType(xml As Xml.Linq.XElement) As DataTable(Of Schema)
            Return New DataTable(Of Schema) With {.MySQL = CType(xml, ConnectionUri)}
        End Operator

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    Call Me.Commit()
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

        Public Event ThrowException(ex As Exception, SQL As String)

        Private Sub MySQL_ThrowException(ex As Exception, SQL As String) Handles MySQL.ThrowException
            RaiseEvent ThrowException(ex, SQL)
        End Sub
    End Class
End Namespace


