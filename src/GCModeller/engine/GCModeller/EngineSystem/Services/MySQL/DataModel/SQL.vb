#Region "Microsoft.VisualBasic::186c47c877e788b32784d64c71e0eb23, engine\GCModeller\EngineSystem\Services\MySQL\DataModel\SQL.vb"

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

    '  
    ' 
    '     Sub: CreateTable, Delete, Flush, FlushAll, Insert
    '          Update
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace EngineSystem.Services.MySQL : Partial Class DataModel

        ''' <summary>
        ''' Insert a line of data into the database table.
        ''' (向数据表之中插入一行新数据)
        ''' </summary>
        ''' <param name="row">The row data that will be insert into the table.(将要插入表中的数据行)</param>
        ''' <param name="PendingTransaction">
        ''' Does this operation was pending to the sql transaction to save networking usage or not, default is not pending.
        ''' (是否将本次操作排队至事务集之中以节约网络通信消耗，默认不排队)
        ''' </param>
        ''' <remarks></remarks>
        Public Sub Insert(row As MySQL.DataModelRecord, Optional PendingTransaction As Boolean = False)
            Dim SQL As String = row.InsertSQL.Replace("%s", Table)

#If DEBUG Then
            Console.WriteLine(SQL)
#End If

            If PendingTransaction Then
                Transaction.AppendLine(SQL)
            Else
                MYSQL.Execute(SQL)
            End If
        End Sub

        ''' <summary>
        ''' Update a line of data into the database table using its unique registry number.
        ''' (使用数据对象的登录号更新数据表之中的一行已存在的数据)
        ''' </summary>
        ''' <param name="row">The row data that will be update into the table.(将要对表中的数据行进行更新的数据对象)</param>
        ''' <param name="PendingTransaction">
        ''' Does this operation was pending to the sql transaction to save networking usage or not, default is not pending.
        ''' (是否将本次操作排队至事务集之中以节约网络通信消耗，默认不排队)
        ''' </param>
        ''' <remarks></remarks>
        Public Sub Update(row As MySQL.DataModelRecord, Optional PendingTransaction As Boolean = False)
            Dim SQL As String = row.UpdateSQL.Replace("%s", Table)

            If PendingTransaction Then
                Transaction.AppendLine(SQL)
            Else
                MYSQL.Execute(SQL)
            End If
        End Sub

        ''' <summary>
        ''' Delete a line of data into the database table using its unique registry number.
        ''' (使用数据对象的登录号删除数据表之中的一行已存在的数据)
        ''' </summary>
        ''' <param name="row">The row data that will be deleted in the table.(表中将要删除的一行数据对象)</param>
        ''' <param name="PendingTransaction">
        ''' Does this operation was pending to the sql transaction to save networking usage or not, default is not pending.
        ''' (是否将本次操作排队至事务集之中以节约网络通信消耗，默认不排队)
        ''' </param>
        ''' <remarks></remarks>
        Public Sub Delete(row As MySQL.DataModelRecord, Optional PendingTransaction As Boolean = False)
            Dim SQL As String = row.DeleteSQL.Replace("%s", Table)

            If PendingTransaction Then
                Transaction.AppendLine(SQL)
            Else
                MYSQL.Execute(SQL)
            End If
        End Sub

        ReadOnly CREATE_TABLE_SQL As String =
            <SQL>
                CREATE  TABLE `{0}` (
                 `guid` VARCHAR(64) NOT NULL ,
                 `registrynumber` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT ,
                 `datamodel` LONGTEXT NOT NULL ,
                UNIQUE INDEX `guid_UNIQUE` (`guid` ASC) ,
                UNIQUE INDEX `registrynumber_UNIQUE` (`registrynumber` ASC) ,
                PRIMARY KEY (`registrynumber`) );
            </SQL>

        ''' <summary>
        ''' Create a table to store that model data in the database with the specifci name.
        ''' (在数据库之中创建一个具有特定名称类型的用于存储模型数据的数据表)
        ''' </summary>
        ''' <param name="Table">Table name.(表名)</param>
        ''' <remarks>
        ''' Please notice that the storage format of every datamodel is in the same type, but in order to make 
        ''' the sql query faster, we split this models stored into several table with the same schema but the 
        ''' name is different. So the difference between these data table just only on the part of the table 
        ''' namming. 
        ''' (请注意，模型数据在数据库中的存储格式在各个对象之间是一模一样的，但是为了加快查询速度，故将模型数据按照分
        ''' 类分别存放于若干个不同的数据表之中。故在这些表之中，各个表之间的格式是一致的，但是不同之处仅在于表的名称
        ''' 不同而已)
        ''' </remarks>
        Public Sub CreateTable(Table As String)
            Dim SQL As String = String.Format(CREATE_TABLE_SQL, Table)
            Call MYSQL.Execute(SQL)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Table">
        ''' The name of the table that all of its model data will be delete. If the value of this parameter is '*', 
        ''' then the client will clear the whole model data in the database 'GCModeller'.
        ''' (将要清除所有模型数据的表的名称，假若本参数的值为'*'，则将数据库‘GCModeller’中所有的数据表的数据清空)
        ''' </param>
        ''' <remarks></remarks>
        Public Sub Flush(Table As String)
            If String.Equals(Table, "*") Then
                Call FlushAll()
            Else
                Dim SQL As String = String.Format("DELETE FROM `{0}` WHERE `RegistryNumber`>'0';", Table)
                Call MYSQL.Execute(SQL)
            End If
        End Sub

        ''' <summary>
        ''' Clear all of the model data in the database.
        ''' (清除数据库之中的所有模型数据)
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub FlushAll()
            Const FLUSH_SQL As String = "DELETE FROM `{0}` WHERE `RegistryNumber`>'0';"
            Dim SQL As String

            For Each Table As String In {"Gene", "polypeptide", "reactions", "compounds", "proteincomplexes", "proteinfeature"}
                SQL = String.Format(FLUSH_SQL, Table)
                Call MYSQL.Execute(SQL)
            Next
        End Sub
    End Class
End Namespace
