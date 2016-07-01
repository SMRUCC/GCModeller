#Region "Microsoft.VisualBasic::6b62dd5f661b6c50b0a4198fad03d47f, ..\GCModeller\engine\GCModeller\EngineSystem\Services\DataAcquisition\DataSerializer\MySQL.vb"

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

Imports System.Text

Imports Microsoft.VisualBasic.Terminal.stdio
Imports Microsoft.VisualBasic.Extensions

Namespace EngineSystem.Services.DataAcquisition.DataSerializer

    Public Class MySQL : Inherits DataSerializer

        Dim MySQL As Oracle.LinuxCompatibility.MySQL.MySQL =
            New Oracle.LinuxCompatibility.MySQL.MySQL
        Dim Id As Long = 0L

        Public Sub New(Url As String)
            Call MyBase.New(Url)

            Dim MySQLConnection As Oracle.LinuxCompatibility.MySQL.ConnectionUri = Url
            Call MySQL.Connect(MySQLConnection:=Url, OnCreateSchema:=True)
        End Sub

        Public Overrides Sub Initialize(TableName As String)
            '清空原有的表中的数据
            Dim SqlDropTable As String = String.Format(DataFlowF.SQL_DROP_TABLE, TableName)
            Dim SqlCreateTable As String = String.Format(DataFlowF.CREATE_TABLE_SQL, TableName)

            Try
                Call MySQL.Execute(SqlDropTable)
            Catch ex As Exception
            End Try
            Call MySQL.Execute(SqlCreateTable)
        End Sub

        Public Overrides Function CommitData(Optional TableName As String = "") As Integer
            Dim Query = From e In _DataFlows Let SQL = e.InsertSQL(Id:=PlusId) Select SQL.Replace("%s", TableName) '
            Dim TransactionBuilder As StringBuilder = New StringBuilder(10 * 1024)

            For Each INSERT_SQL As String In Query.ToArray
                Call TransactionBuilder.AppendLine(INSERT_SQL)
            Next

            Call _DataFlows.Clear()
            If Not MyBase._SuppressPeriodicMessage Then Call Printf("   > Data size: %s byte.", TransactionBuilder.Length)
            Dim r As Boolean = MySQL.CommitTransaction(Transaction:=TransactionBuilder.ToString)

            If r = False Then
                MyBase._ErrMessage =
                    MySQL.GetErrMessageString & vbCrLf & New String("-"c, 20) & "> " & TableName & " </SQL>" & vbCrLf & vbCrLf &
                    "    TRANSACTION_DUMP()::{" & vbCrLf & vbCrLf & TransactionBuilder.ToString & vbCrLf & vbCrLf & "       }"
                Call FileIO.FileSystem.WriteAllText(Settings.TEMP & "/MySQL.log", MyBase._ErrMessage, append:=True)

                Return -1
            End If

            Return 0
        End Function

        Public Overrides Sub CreateHandle(List() As HandleF, Table As String)
            Dim TransactionBuilder As StringBuilder = New StringBuilder(10 * 1024)

            '清空原有的表中的数据
            Dim SqlDropTable As String = String.Format(HandleF.SQL_DROP_TABLE, Table)
            Dim SqlCreateTable As String = HandleF.CREATE_TABLE_SQL.Replace("%s", Table)

            Call MySQL.Execute(SqlDropTable)
            Call MySQL.Execute(SqlCreateTable)

            For Each Sql As HandleF In List
                Call TransactionBuilder.AppendLine(Sql.InsertSQL)
            Next
            Call TransactionBuilder.Replace("%s", Table)
            Call Me.MySQL.CommitTransaction(TransactionBuilder.ToString)
        End Sub

        Public Overrides Sub Close(Table As String)
            Call CommitData(TableName:=Table)
        End Sub

        Private Function PlusId() As Long
            Id += 1
            Return Id
        End Function
    End Class
End Namespace
