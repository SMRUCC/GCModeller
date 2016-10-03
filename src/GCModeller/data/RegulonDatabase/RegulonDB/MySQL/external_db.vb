#Region "Microsoft.VisualBasic::e753825114c3b34eb09b5de72c60c682, ..\GCModeller\data\RegulonDatabase\RegulonDB\MySQL\external_db.vb"

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
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:08:18 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace RegulonDB.Tables

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `external_db`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `external_db` (
'''   `external_db_id` char(12) NOT NULL,
'''   `external_db_name` varchar(255) NOT NULL,
'''   `external_db_description` varchar(255) DEFAULT NULL,
'''   `url` varchar(255) NOT NULL,
'''   `external_db_note` varchar(2000) DEFAULT NULL,
'''   `ext_db_internal_comment` longtext
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("external_db", Database:="regulondb_7_5")>
Public Class external_db: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("external_db_id"), NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property external_db_id As String
    <DatabaseField("external_db_name"), NotNull, DataType(MySqlDbType.VarChar, "255")> Public Property external_db_name As String
    <DatabaseField("external_db_description"), DataType(MySqlDbType.VarChar, "255")> Public Property external_db_description As String
    <DatabaseField("url"), NotNull, DataType(MySqlDbType.VarChar, "255")> Public Property url As String
    <DatabaseField("external_db_note"), DataType(MySqlDbType.VarChar, "2000")> Public Property external_db_note As String
    <DatabaseField("ext_db_internal_comment"), DataType(MySqlDbType.Text)> Public Property ext_db_internal_comment As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `external_db` (`external_db_id`, `external_db_name`, `external_db_description`, `url`, `external_db_note`, `ext_db_internal_comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `external_db` (`external_db_id`, `external_db_name`, `external_db_description`, `url`, `external_db_note`, `ext_db_internal_comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `external_db` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `external_db` SET `external_db_id`='{0}', `external_db_name`='{1}', `external_db_description`='{2}', `url`='{3}', `external_db_note`='{4}', `ext_db_internal_comment`='{5}' WHERE ;</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, external_db_id, external_db_name, external_db_description, url, external_db_note, ext_db_internal_comment)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, external_db_id, external_db_name, external_db_description, url, external_db_note, ext_db_internal_comment)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace
