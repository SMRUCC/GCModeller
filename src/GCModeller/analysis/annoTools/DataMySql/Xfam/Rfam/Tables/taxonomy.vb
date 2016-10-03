#Region "Microsoft.VisualBasic::c803d0ad55140b9785ee89c5217b8d8f, ..\GCModeller\analysis\annoTools\DataMySql\Xfam\Rfam\Tables\taxonomy.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  Microsoft VisualBasic MYSQL

' SqlDump= 


' 

Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace Xfam.Rfam.MySQL.Tables

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("taxonomy")>
Public Class taxonomy: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("ncbi_id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property ncbi_id As Long
    <DatabaseField("species"), NotNull, DataType(MySqlDbType.VarChar, "100")> Public Property species As String
    <DatabaseField("tax_string"), DataType(MySqlDbType.Text)> Public Property tax_string As String
    <DatabaseField("tree_display_name"), DataType(MySqlDbType.VarChar, "100")> Public Property tree_display_name As String
    <DatabaseField("align_display_name"), DataType(MySqlDbType.VarChar, "50")> Public Property align_display_name As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `taxonomy` (`ncbi_id`, `species`, `tax_string`, `tree_display_name`, `align_display_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `taxonomy` (`ncbi_id`, `species`, `tax_string`, `tree_display_name`, `align_display_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `taxonomy` WHERE `ncbi_id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `taxonomy` SET `ncbi_id`='{0}', `species`='{1}', `tax_string`='{2}', `tree_display_name`='{3}', `align_display_name`='{4}' WHERE `ncbi_id` = '{5}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, ncbi_id)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, ncbi_id, species, tax_string, tree_display_name, align_display_name)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, ncbi_id, species, tax_string, tree_display_name, align_display_name)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, ncbi_id, species, tax_string, tree_display_name, align_display_name, ncbi_id)
    End Function
#End Region
End Class


End Namespace
