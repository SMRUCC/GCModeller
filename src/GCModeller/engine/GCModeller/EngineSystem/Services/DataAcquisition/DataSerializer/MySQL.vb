#Region "Microsoft.VisualBasic::433466c45545b1b25808c4a33101c290, engine\GCModeller\EngineSystem\Services\DataAcquisition\DataSerializer\MySQL.vb"

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

    '     Class MySQL
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: CommitData, PlusId
    ' 
    '         Sub: Close, CreateHandle, Initialize
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.STDIO
Imports Oracle.LinuxCompatibility.MySQL
Imports Oracle.LinuxCompatibility.MySQL.Uri

Namespace EngineSystem.Services.DataAcquisition.DataSerializer

    Public Class MySQL : Inherits DataSerializer

        Dim MySQL As New MySqli
        Dim Id As Long = 0L

        Public Sub New(Url As String)
            Call MyBase.New(Url)

            Dim MySQLConnection As ConnectionUri = Url
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
            Dim LQuery = From x As DataFlowF
                         In _DataFlows
                         Let SQL = x.InsertSQL(Id:=PlusId)
                         Select SQL.Replace("%s", TableName) '

            Dim t As New StringBuilder(10 * 1024)
            Dim exp As Exception = Nothing

            For Each INSERT_SQL As String In LQuery
                Call t.AppendLine(INSERT_SQL)
            Next

            Call _DataFlows.Clear()

            If Not MyBase._SuppressPeriodicMessage Then
                Call printf("   > Data size: %s byte.", t.Length)
            End If

            Dim r As Boolean = MySQL.CommitTransaction(t.ToString, exp)

            If r = False Then
                exp = New Exception(New String("-"c, 20) & "> " & TableName & " </SQL>" & vbCrLf & vbCrLf &
                    "    TRANSACTION_DUMP()::{" & vbCrLf & vbCrLf & t.ToString & vbCrLf & vbCrLf & "       }", exp)
                Call App.LogException(exp)

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
