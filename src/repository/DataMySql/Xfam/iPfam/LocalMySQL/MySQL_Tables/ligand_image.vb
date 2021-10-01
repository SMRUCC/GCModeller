#Region "Microsoft.VisualBasic::6c6e3509d66117cbdc6258e9612c7fe9, DataMySql\Xfam\iPfam\LocalMySQL\MySQL_Tables\ligand_image.vb"

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

    ' Class ligand_image
    ' 
    '     Properties: ligand_id, size_120, size_270, size_400, size_600
    ' 
    '     Function: GetDeleteSQL, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  Microsoft VisualBasic MYSQL

' SqlDump= 


' 

Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace iPfam.LocalMySQL

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("ligand_image")>
Public Class ligand_image: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("ligand_id"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "3")> Public Property ligand_id As String
    <DatabaseField("size_120"), DataType(MySqlDbType.Blob)> Public Property size_120 As Byte()
    <DatabaseField("size_270"), DataType(MySqlDbType.Blob)> Public Property size_270 As Byte()
    <DatabaseField("size_400"), DataType(MySqlDbType.Blob)> Public Property size_400 As Byte()
    <DatabaseField("size_600"), DataType(MySqlDbType.Blob)> Public Property size_600 As Byte()
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `ligand_image` (`ligand_id`, `size_120`, `size_270`, `size_400`, `size_600`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `ligand_image` (`ligand_id`, `size_120`, `size_270`, `size_400`, `size_600`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `ligand_image` WHERE `ligand_id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `ligand_image` SET `ligand_id`='{0}', `size_120`='{1}', `size_270`='{2}', `size_400`='{3}', `size_600`='{4}' WHERE `ligand_id` = '{5}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, ligand_id)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, ligand_id, size_120, size_270, size_400, size_600)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, ligand_id, size_120, size_270, size_400, size_600)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, ligand_id, size_120, size_270, size_400, size_600, ligand_id)
    End Function
#End Region
End Class


End Namespace
