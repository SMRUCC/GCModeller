#Region "Microsoft.VisualBasic::af74fcdd7b96a543e9614eec3436de1c, engine\GCModeller\EngineSystem\Services\MySQL\DataModel\Record.vb"

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

    '     Class DataModelRecord
    ' 
    '         Properties: DataModel, DeleteSQL, GUID, InsertSQL, RegistryNumber
    '                     UpdateSQL
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace EngineSystem.Services.MySQL

    <TableName("GCModeller")> Public Class DataModelRecord

        ''' <summary>
        ''' The global unique identifier for each data model object in the GCModeller database.
        ''' (每一个数据模型对象在GCModeller数据库之中的唯一标识符)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <NotNULL> <Unique> <DataType(MySqlDbType.Text)> <DatabaseField>
        Public Property GUID As String

        ''' <summary>
        ''' The registry number for each datamodel object in its specific category table.
        ''' (每一个数据模型对象在其所属分类之下的数据表之中的登录号)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <PrimaryKey> <Unique> <DataType(MySqlDbType.BigInt)> <NotNULL> <AutoIncrement> <Unsigned>
        <DatabaseField>
        Public Property RegistryNumber As Long

        ''' <summary>
        ''' The model data of the datamodel, it is a XML model data.
        ''' (数据模型中的数据，本属性值为一个XML格式的数据模型，需要使用特定类型的对象进行反序列化操作方可以读取)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <NotNULL> <DataType(MySqlDbType.LongText)> <DatabaseField>
        Public Property DataModel As String

        Public Overrides Function ToString() As String
            Return String.Format("{0}, {1}, {2}", GUID, RegistryNumber, DataModel)
        End Function

        Shared Narrowing Operator CType(e As DataModelRecord) As String
            Return String.Format("{0}, {1}, {2}", e.GUID, e.RegistryNumber, e.DataModel)
        End Operator

        Const DELETE_SQL As String = "DELETE FROM `%s` WHERE `RegistryNumber`='{0}';"
        Const UPDATE_SQL As String = "UPDATE `%s` SET `registrynumber`='{0}', `guid`='{1}', `datamodel`='{2}' WHERE `registrynumber`='{3}';"
        Const INSERT_SQL As String = "INSERT INTO `%s` (`registrynumber`, `guid`, `datamodel`) VALUES ('{0}', '{1}', '{2}');"

        ''' <summary>
        ''' Get the delete sql text for this datamodel object instance, please notice that the table name is empty, 
        ''' so that you should replace the string "%s" with the table name that this sql text can be functionally.
        ''' (获取本数据模型对象实例的删除SQL命令，请注意，在获得的命令语句之中，表名属性为空，故在使用之前请将"%s"占位符替
        ''' 换为表名，本语句方能够起作用。)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property DeleteSQL As String
            Get
                Return String.Format(DELETE_SQL, RegistryNumber)
            End Get
        End Property

        ''' <summary>
        ''' Get the insert sql text for this datamodel object instance, please notice that the table name is empty, 
        ''' so that you should replace the string "%s" with the table name that this sql text can be functionally.
        ''' (获取本数据模型对象实例的插入SQL命令，请注意，在获得的命令语句之中，表名属性为空，故在使用之前请将"%s"占位符替
        ''' 换为表名，本语句方能够起作用。)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property InsertSQL As String
            Get
                Return String.Format(INSERT_SQL, RegistryNumber, GUID, DataModel)
            End Get
        End Property

        ''' <summary>
        ''' Get the update sql text for this datamodel object instance, please notice that the table name is empty, 
        ''' so that you should replace the string "%s" with the table name that this sql text can be functionally.
        ''' (获取本数据模型对象实例的更新SQL命令，请注意，在获得的命令语句之中，表名属性为空，故在使用之前请将"%s"占位符替
        ''' 换为表名，本语句方能够起作用。)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property UpdateSQL As String
            Get
                Return String.Format(UPDATE_SQL, RegistryNumber, GUID, DataModel, RegistryNumber)
            End Get
        End Property
    End Class
End Namespace
