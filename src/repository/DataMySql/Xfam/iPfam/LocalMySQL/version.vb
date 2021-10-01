#Region "Microsoft.VisualBasic::d36037724228232b4e75a606c9f30fdc, DataMySql\Xfam\iPfam\LocalMySQL\version.vb"

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

    ' Class version
    ' 
    '     Properties: pdb_date, pfam_version, release_date, release_version, sifts_date
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
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("version")>
Public Class version: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("release_version"), DataType(MySqlDbType.Text)> Public Property release_version As String
    <DatabaseField("release_date"), DataType(MySqlDbType.DateTime)> Public Property release_date As Date
    <DatabaseField("pdb_date"), DataType(MySqlDbType.DateTime)> Public Property pdb_date As Date
    <DatabaseField("sifts_date"), DataType(MySqlDbType.DateTime)> Public Property sifts_date As Date
    <DatabaseField("pfam_version"), DataType(MySqlDbType.VarChar, "45")> Public Property pfam_version As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `version` (`release_version`, `release_date`, `pdb_date`, `sifts_date`, `pfam_version`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `version` (`release_version`, `release_date`, `pdb_date`, `sifts_date`, `pfam_version`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `version` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `version` SET `release_version`='{0}', `release_date`='{1}', `pdb_date`='{2}', `sifts_date`='{3}', `pfam_version`='{4}' WHERE ;</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, release_version, DataType.ToMySqlDateTimeString(release_date), DataType.ToMySqlDateTimeString(pdb_date), DataType.ToMySqlDateTimeString(sifts_date), pfam_version)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, release_version, DataType.ToMySqlDateTimeString(release_date), DataType.ToMySqlDateTimeString(pdb_date), DataType.ToMySqlDateTimeString(sifts_date), pfam_version)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace
