#Region "Microsoft.VisualBasic::2c2023f3faba725ccb4dc97d7ec76608, ..\LibMySQL\Reflection\DbReflector.vb"

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

Imports MySql.Data.MySqlClient
Imports Microsoft.VisualBasic
Imports System.Reflection
Imports System.Text.RegularExpressions
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports Microsoft.VisualBasic.Linq

Namespace Reflection

    Public Class DbReflector

        Dim ConnectionString As String

        ''' <summary>
        ''' 假若目标数据表不存在数据记录，则会返回空值
        ''' </summary>
        ''' <typeparam name="ItemType"></typeparam>
        ''' <param name="Reader"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function ReadFirst(Of ItemType As Class)(Reader As DataTableReader) As ItemType

            If Not Reader.HasRows Then
                Return Nothing
            Else
                Call Reader.Read()
            End If

            Dim Type As Type = GetType(ItemType)
            Dim DbFieldAttr As DatabaseField
            Dim ItemTypeProperty As PropertyInfo() = Type.GetProperties
            Dim [Property] As PropertyInfo
            Dim FillObject As Object = Activator.CreateInstance(Type) 'Create a instance of specific type: our record schema. 

            For i As Integer = 0 To ItemTypeProperty.Length - 1  'Using the reflection to get the fields in the table schema only once.
                [Property] = ItemTypeProperty(i)
                DbFieldAttr = [Property].GetAttribute(Of DatabaseField)()

                If DbFieldAttr Is Nothing Then Continue For
                If Len(DbFieldAttr.Name) = 0 Then
                    DbFieldAttr.Name = [Property].Name
                End If

                Dim Ordinal As Integer = Reader.GetOrdinal(DbFieldAttr.Name)
                If Ordinal >= 0 Then
                    Dim ObjValue = Reader.GetValue(Ordinal)
                    If Not IsDBNull(ObjValue) Then
                        Call [Property].SetValue(FillObject, ObjValue, Nothing)
                    End If
                End If
            Next

            Return DirectCast(FillObject, ItemType)
        End Function

        Private Class __readFirst(Of T)

            Public ReadOnly Property value As T

            Public Function __getFirst(obj As T) As Boolean
                _value = obj
                Return True
            End Function
        End Class

        ''' <summary>
        ''' 查询并行化
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="source"></param>
        ''' <param name="type"></param>
        ''' <param name="FieldList"></param>
        ''' <returns></returns>
        Private Function __getObject(Of T)(source As Object, type As Type, FieldList As SeqValue(Of PropertyInfo)()) As T
            Dim FillObject As Object = Activator.CreateInstance(type) 'Create a instance of specific type: our record schema. 

            For FieldPointer As Integer = 0 To FieldList.Length - 1  'Scan all of the fields in the field list and get the field value.
                Dim prop As SeqValue(Of PropertyInfo) = FieldList(FieldPointer)
                Dim Ordinal As Integer = prop.i
                Dim ObjValue As Object = source.GetValue(Ordinal)

                If Not IsDBNull(ObjValue) Then
                    Call prop.obj.SetValue(FillObject, ObjValue, Nothing)
                End If
            Next

            Return DirectCast(FillObject, T)
        End Function

        Private Shared Function __getObject(Of T)(reader As MySqlDataReader,
                                                  type As Type,
                                                  FieldList As SeqValue(Of PropertyInfo)()) As T
            Dim FillObject As Object = Activator.CreateInstance(type) 'Create a instance of specific type: our record schema. 
            Dim FieldPointer As Integer

            Try
                For FieldPointer = 0 To FieldList.Length - 1  'Scan all of the fields in the field list and get the field value.
                    Dim prop As SeqValue(Of PropertyInfo) = FieldList(FieldPointer)
                    Dim Ordinal As Integer = prop.i
                    Dim ObjValue As Object = reader.GetValue(Ordinal)
                    If Not IsDBNull(ObjValue) Then
                        Call prop.obj.SetValue(FillObject, ObjValue, Nothing)
                    End If
                Next
            Catch ex As Exception
                Dim ErrorField = FieldList(FieldPointer)
                ex = New Exception($"[{ErrorField.i}] => {ErrorField.obj.ToString}", ex)
                Throw ex
            End Try

            Return DirectCast(FillObject, T)
        End Function

        Private Shared Function __queryInitSchema(Reader As MySqlDataReader, type As Type) As SeqValue(Of PropertyInfo)()
            Dim DbFieldAttr As DatabaseField
            Dim ItemTypeProperty = type.GetProperties
            Dim fields As New List(Of SeqValue(Of PropertyInfo))
            Dim schema = (From r As DataRow
                          In Reader.GetSchemaTable
                          Select r).ToArray(Function(x) x.Item("ColumnName").ToString)  ' 获取当前表之中可用的列名称列表

            For Each [property] As PropertyInfo In ItemTypeProperty    'Using the reflection to get the fields in the table schema only once.
                DbFieldAttr = [property].GetAttribute(Of DatabaseField)()

                If DbFieldAttr Is Nothing Then Continue For
                If String.IsNullOrEmpty(DbFieldAttr.Name) Then
                    DbFieldAttr.Name = [property].Name
                End If
                If Array.IndexOf(schema, DbFieldAttr.Name) = -1 Then
                    Continue For  ' 反射操作的时候只对包含有的列属性进行赋值
                End If

                Dim Ordinal As Integer = Reader.GetOrdinal(DbFieldAttr.Name)

                If Ordinal >= 0 Then
                    fields += New SeqValue(Of PropertyInfo) With {
                        .i = Ordinal,
                        .obj = [property]
                    }
                End If
            Next

            Return fields
        End Function

        Private Delegate Function QueryInvoke(Of T)(Reader As MySqlDataReader, type As Type, FieldList As SeqValue(Of PropertyInfo)()) As IEnumerable(Of T)

        Private Function __queryParallelInvoke(Of T)(Reader As MySqlDataReader,
                                                     type As Type,
                                                     FieldList As SeqValue(Of PropertyInfo)()) As T()
            Dim LQuery As T() = (From x As T In __linqToMySQL(Of T)(Reader, type, FieldList).AsParallel Select x).ToArray
            Return LQuery
        End Function

        Private Function __linqToMySQL(Of T)(Reader As MySqlDataReader,
                                             type As Type,
                                             FieldList As SeqValue(Of PropertyInfo)()) As IEnumerable(Of T)
            Dim LQuery As IEnumerable(Of T) = From row As Object
                                              In Reader
                                              Select __getObject(Of T)(row, type, FieldList)
            Return LQuery
        End Function

        ''' <summary>
        ''' Linq to MySQL.(Linq to MySQL对象实体数据映射)
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="SQL"></param>
        ''' <returns></returns>
        Public Function AsQuery(Of T)(SQL As String, ByRef getError As String) As IEnumerable(Of T)
            Dim query As IEnumerable(Of T) =
                __queryEngine(Of T)(SQL, AddressOf __linqToMySQL(Of T), getError)
            Return query
        End Function

        Private Shared Function __queryInvoke(Of T)(Reader As MySqlDataReader,
                                             type As Type,
                                             FieldList As SeqValue(Of PropertyInfo)()) As T()
            Dim Table As New List(Of T)

            Do While Reader.Read
                Call Table.Add(__getObject(Of T)(Reader, type, FieldList))
            Loop

            Return Table.ToArray
        End Function

        ''' <summary>
        ''' 执行具体的数据映射操作
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="SQL"></param>
        ''' <param name="queryEngine"></param>
        ''' <param name="getError"></param>
        ''' <returns></returns>
        Private Function __queryEngine(Of T)(SQL As String, queryEngine As QueryInvoke(Of T), ByRef getError As String) As IEnumerable(Of T)
            Dim Type As Type = GetType(T)
            Dim MySql As MySqlConnection = New MySqlConnection(ConnectionString) '[ConnectionString] is a compiled mysql connection string from our class constructor.
            Dim MySqlCommand As MySqlCommand = New MySqlCommand(SQL, MySql)
            Dim Reader As MySqlDataReader = Nothing

            Try
                Call MySql.Open()

                Reader = MySqlCommand.ExecuteReader(CommandBehavior.CloseConnection)

                If Not Reader.HasRows Then ' 没有数据返回则返回一个空表
                    Return New T() {}
                End If

                Dim FieldList As SeqValue(Of PropertyInfo)() = __queryInitSchema(Reader, Type)
                Dim Table As IEnumerable(Of T) = queryEngine(Reader, Type, FieldList)
                Return Table
            Catch ex As Exception
                Dim log As String = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) & "/MYSQL_REFLECTOR_ERROR.log"

                ex = New Exception(Type.FullName, ex)
                getError = App.BugsFormatter(New Exception(SQL, ex), MethodBase.GetCurrentMethod.GetFullName)

                Call FileIO.FileSystem.WriteAllText(log, getError & vbCrLf & vbCrLf, append:=True)
            Finally
                If Not Reader Is Nothing Then Call Reader.Close()
                If Not MySqlCommand Is Nothing Then Call MySqlCommand.Dispose()
                If Not MySql Is Nothing Then Call MySql.Dispose()
            End Try

            Return Nothing
        End Function

        Private Class Linq_2MySQL(Of T)

            ReadOnly __invoke As Func(Of T, Boolean)
            ReadOnly __invokeAction As Action(Of T)

            Sub New(__invoke As Action(Of T))
                Me.__invokeAction = __invoke
                Me.__invoke = AddressOf ___invoke
            End Sub

            Sub New(__invoke As Func(Of T, Boolean))
                Me.__invoke = __invoke
            End Sub

            Private Function ___invoke(obj As T) As Boolean
                Call __invokeAction(obj)
                Return False
            End Function

            Public Function __queryEngine(Reader As MySqlDataReader,
                                          type As Type,
                                          lstField As SeqValue(Of PropertyInfo)()) As T()
                Call __forEach(Of T)(Reader, type, lstField, Me.__invoke)
                Return Nothing
            End Function

            ''' <summary>
            ''' LINQ to MySQL engine
            ''' </summary>
            ''' <typeparam name="TEntity"></typeparam>
            ''' <param name="reader"></param>
            ''' <param name="type"></param>
            ''' <param name="lstFields"></param>
            ''' <param name="__invoke"></param>
            Private Shared Sub __forEach(Of TEntity)(reader As MySqlDataReader,
                                                     type As Type,
                                                     lstFields As SeqValue(Of PropertyInfo)(),
                                                     __invoke As Func(Of TEntity, Boolean))
                Do While reader.Read
                    Dim obj As TEntity = __getObject(Of TEntity)(reader, type, lstFields)
                    Dim _exit As Boolean = __invoke(obj)

                    If _exit Then
                        Exit Do
                    End If
                Loop
            End Sub
        End Class

        ''' <summary>
        ''' Optimization for the large dataset parallel query.(数据集合非常大的时候，可以尝试使用这个并行查询函数)
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="SQL"></param>
        ''' <param name="GetErr"></param>
        ''' <returns></returns>
        Public Function ParallelQuery(Of T)(SQL As String, Optional ByRef GetErr As String = "") As T()
            Dim Table As T() = __queryEngine(Of T)(SQL, AddressOf __queryParallelInvoke(Of T), GetErr)
            Return Table
        End Function

        ''' <summary>
        ''' LINQ to MySQL interactor.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="SQL"></param>
        ''' <param name="invoke"></param>
        ''' <param name="getErr"></param>
        Public Sub ForEach(Of T)(SQL As String, invoke As Action(Of T), Optional ByRef getErr As String = "")
            Call __queryEngine(Of T)(SQL, AddressOf New Linq_2MySQL(Of T)(invoke).__queryEngine, getErr)
        End Sub

        ''' <summary>
        ''' LINQ to MySQL interactor.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="SQL"></param>
        ''' <param name="invoke"></param>
        ''' <param name="getErr"></param>
        Public Sub ForEach(Of T)(SQL As String, invoke As Func(Of T, Boolean), Optional ByRef getErr As String = "")
            Call __queryEngine(Of T)(SQL, AddressOf New Linq_2MySQL(Of T)(invoke).__queryEngine, getErr)
        End Sub

        ''' <summary>
        ''' Query a data table using Reflection.(使用反射机制来查询一个数据表，请注意，假若返回的是Nothing，则说明发生了错误)
        ''' </summary>
        ''' <typeparam name="ItemType">Mapping schema to our data table.(对我们的数据表的映射类型)</typeparam>
        ''' <param name="SQL">Sql 'SELECT' query statement.(Sql 'SELECT' 查询语句)</param>
        ''' <returns>The target data table.(目标数据表)</returns>
        ''' <remarks></remarks>
        Public Function Query(Of ItemType)(SQL As String, Optional ByRef GetErr As String = "") As ItemType()
            Dim Table = __queryEngine(Of ItemType)(SQL, AddressOf __queryInvoke(Of ItemType), GetErr)
            Return Table
        End Function

#Region "Constructor"

        Protected Friend Sub New()
        End Sub

        Public Sub New(ConnectionString As String)
            Me.ConnectionString = ConnectionString
        End Sub

        Public Sub New(uri As ConnectionUri)
            Me.ConnectionString = uri.GetConnectionString
        End Sub

        Public Overrides Function ToString() As String
            Return ConnectionString
        End Function

        Public Shared Widening Operator CType(uri As ConnectionUri) As DbReflector
            Return New DbReflector With {
                .ConnectionString = uri
            }
        End Operator

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="s_cnn">MySql connection string.(MySql连接字符串)</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Example: 
        ''' http://localhost:8080/client?user=username%password=password%database=database
        ''' </remarks>
        Public Shared Widening Operator CType(s_cnn As String) As DbReflector
            Dim NewConnection As ConnectionUri = s_cnn
            Return CType(NewConnection, DbReflector)
        End Operator

        Public Shared Widening Operator CType(cnn As Xml.Linq.XElement) As DbReflector
            Return CType(cnn.Value, ConnectionUri)
        End Operator
#End Region
    End Class
End Namespace

