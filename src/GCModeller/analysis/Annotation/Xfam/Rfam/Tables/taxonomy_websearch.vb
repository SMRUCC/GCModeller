#Region "Microsoft.VisualBasic::6c5094f06ffd2219b162b7b337c7b556, ..\GCModeller\analysis\Annotation\Xfam\Rfam\Tables\taxonomy_websearch.vb"

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
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("taxonomy_websearch")>
Public Class taxonomy_websearch: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("ncbi_id"), DataType(MySqlDbType.Int64, "10")> Public Property ncbi_id As Long
    <DatabaseField("species"), DataType(MySqlDbType.VarChar, "100")> Public Property species As String
    <DatabaseField("taxonomy"), DataType(MySqlDbType.Text)> Public Property taxonomy As String
    <DatabaseField("lft"), PrimaryKey, DataType(MySqlDbType.Int64, "10")> Public Property lft As Long
    <DatabaseField("rgt"), DataType(MySqlDbType.Int64, "10")> Public Property rgt As Long
    <DatabaseField("parent"), DataType(MySqlDbType.Int64, "10")> Public Property parent As Long
    <DatabaseField("level"), DataType(MySqlDbType.VarChar, "200")> Public Property level As String
    <DatabaseField("minimal"), NotNull, DataType(MySqlDbType.Int64, "1")> Public Property minimal As Long
    <DatabaseField("rank"), DataType(MySqlDbType.VarChar, "100")> Public Property rank As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `taxonomy_websearch` (`ncbi_id`, `species`, `taxonomy`, `lft`, `rgt`, `parent`, `level`, `minimal`, `rank`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `taxonomy_websearch` (`ncbi_id`, `species`, `taxonomy`, `lft`, `rgt`, `parent`, `level`, `minimal`, `rank`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `taxonomy_websearch` WHERE `lft` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `taxonomy_websearch` SET `ncbi_id`='{0}', `species`='{1}', `taxonomy`='{2}', `lft`='{3}', `rgt`='{4}', `parent`='{5}', `level`='{6}', `minimal`='{7}', `rank`='{8}' WHERE `lft` = '{9}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, lft)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, ncbi_id, species, taxonomy, lft, rgt, parent, level, minimal, rank)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, ncbi_id, species, taxonomy, lft, rgt, parent, level, minimal, rank)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, ncbi_id, species, taxonomy, lft, rgt, parent, level, minimal, rank, lft)
    End Function
#End Region
End Class


End Namespace

