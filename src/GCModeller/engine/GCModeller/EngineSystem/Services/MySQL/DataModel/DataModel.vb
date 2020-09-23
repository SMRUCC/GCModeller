#Region "Microsoft.VisualBasic::758d9c0de118af1ee834ca0a91fab4c4, engine\GCModeller\EngineSystem\Services\MySQL\DataModel\DataModel.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:

    '     Class DataModel
    ' 
    '         Properties: Table
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: __model, Commit, GetMaxHandle, Load, ToString
    ' 
    '         Sub: Dispose
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic

Namespace EngineSystem.Services.MySQL

    ''' <summary>
    ''' The data exchange service for the data model read from and write into the mysql database.
    ''' (数据模型对象与数据库服务器之间的数据交换服务)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DataModel : Inherits Service
        Implements System.IDisposable

        Const SELECT_ALL As String = "SELECT * FROM {0};"
        Const SELECT_COUNTS As String = "SELECT * FROM {0} LIMIT {1},{2};"

        Dim lPointer As New Hashtable
        Dim Transaction As StringBuilder = New StringBuilder(4096)

        ''' <summary>
        ''' The data table that will be operated.
        ''' (所进行操作的数据表)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Table As String

        Private Shared Function __model(row As DataRow) As MySQL.DataModelRecord
            Dim Model = New DataModelRecord

            Model.GUID = row.Item("guid")
            Model.RegistryNumber = row.Item("RegistryNumber")
            Model.DataModel = row.Item("DataModel")

            Return Model
        End Function

        ''' <summary>
        ''' Load the data from database server. Please notice that: every time you call this function, the transaction will be commit to the database server.
        ''' (从数据库服务器之中加载数据，请注意：每一次加载数据都会将先前的所积累下来的事务提交至数据库服务器之上)
        ''' </summary>
        ''' <param name="Counts">
        ''' The count of the record that will be read from the server. Notice: Zero or negative is stands for 
        ''' load all records in the database.
        ''' (从数据库中读取的记录数目。请注意：值0和负数值都表示加载数据库的表中的所有数据)
        ''' </param>
        ''' <remarks></remarks>
        Public Function Load(Table As String, Optional Counts As Integer = 100) As List(Of DataModelRecord)
            Dim SQL As String, p As Integer
            Dim DataSet As List(Of MySQL.DataModelRecord)

            If Counts <= 0 Then  'Load all data when count is zero or negative.
                SQL = String.Format(SELECT_ALL, Table)
            Else
                p = lPointer(Table)
                SQL = String.Format(SELECT_COUNTS, Table, p, Counts)
            End If

            Dim Query As Generic.IEnumerable(Of MySQL.DataModelRecord) = From row As DataRow
                                                                         In MYSQL.Fetch(SQL).Tables(0).Rows.AsParallel
                                                                         Select __model(row) 'Create new datamodel from database
            DataSet = Query.AsList
            If Counts <= 0 Then
                lPointer(Table) = DataSet.Count
            Else
                lPointer(Table) = p + Counts
            End If

            Return DataSet
        End Function

        ''' <summary>
        ''' Commit the transaction to the database server.
        ''' (将事务集提交至数据库服务器之中)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Commit() As Boolean
            Dim s As String = Transaction.ToString
            Call Transaction.Clear()
            Return MYSQL.CommitTransaction(Transaction:=s)
        End Function

        ''' <summary>
        ''' 查询出数据库之中的某一个表的最大句柄值
        ''' </summary>
        ''' <remarks></remarks>
        Const MAX_HANDLE_SQL As String = "SELECT MAX(`registrynumber`) FROM '{0}';"

        ''' <summary>
        ''' 获取目标数据表之中的最大的句柄值
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetMaxHandle(Table As String) As Long
            Dim SQL As String = String.Format(MAX_HANDLE_SQL, Table)
            Dim r = MYSQL.Fetch(SQL).Tables(0).Rows.Item(0)

            Return r.Item(0)
        End Function

        Public Overrides Function ToString() As String
            Return MYSQL.ToString
        End Function

        ''' <summary>
        ''' </summary>
        ''' <param name="ConnectionString">The connection string of the target mysql database.(至目标数据库的连接字符串)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Shadows Widening Operator CType(ConnectionString As String) As DataModel
            Return New DataModel With {.MYSQL = ConnectionString}
        End Operator

        Private Sub New()

        End Sub

        Protected Overrides Sub Dispose(disposing As Boolean)
            Call Me.Commit()   'Commit the remaining data to the database in order to keep away from the risk of data lost.
            MyBase.Dispose(disposing)
        End Sub
    End Class
End Namespace
