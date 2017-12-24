#Region "Microsoft.VisualBasic::ff33cdd1a1b0f6a726bc1b0d2f7e2ffe, ..\GCModeller\data\RegulonDatabase\RegulonDB\MySQL\gensorunit.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
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

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @3/29/2017 11:24:24 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace RegulonDB.Tables

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `gensorunit`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `gensorunit` (
'''   `gu_id` char(12) NOT NULL,
'''   `gu_name` varchar(255) NOT NULL,
'''   `gu_description` varchar(2000) DEFAULT NULL,
'''   `gu_map_file` varchar(250) NOT NULL,
'''   `gu_status` varchar(50) DEFAULT NULL,
'''   `effector_feedback_paths` varchar(2000) DEFAULT NULL,
'''   `proteincomplex_regulated_by_tf` varchar(100) DEFAULT NULL,
'''   `total_enzymes_withconnectivity` varchar(100) DEFAULT NULL,
'''   `enzymes_withconnectivity` varchar(2000) DEFAULT NULL,
'''   `note` longtext
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("gensorunit", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `gensorunit` (
  `gu_id` char(12) NOT NULL,
  `gu_name` varchar(255) NOT NULL,
  `gu_description` varchar(2000) DEFAULT NULL,
  `gu_map_file` varchar(250) NOT NULL,
  `gu_status` varchar(50) DEFAULT NULL,
  `effector_feedback_paths` varchar(2000) DEFAULT NULL,
  `proteincomplex_regulated_by_tf` varchar(100) DEFAULT NULL,
  `total_enzymes_withconnectivity` varchar(100) DEFAULT NULL,
  `enzymes_withconnectivity` varchar(2000) DEFAULT NULL,
  `note` longtext
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class gensorunit: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("gu_id"), NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property gu_id As String
    <DatabaseField("gu_name"), NotNull, DataType(MySqlDbType.VarChar, "255")> Public Property gu_name As String
    <DatabaseField("gu_description"), DataType(MySqlDbType.VarChar, "2000")> Public Property gu_description As String
    <DatabaseField("gu_map_file"), NotNull, DataType(MySqlDbType.VarChar, "250")> Public Property gu_map_file As String
    <DatabaseField("gu_status"), DataType(MySqlDbType.VarChar, "50")> Public Property gu_status As String
    <DatabaseField("effector_feedback_paths"), DataType(MySqlDbType.VarChar, "2000")> Public Property effector_feedback_paths As String
    <DatabaseField("proteincomplex_regulated_by_tf"), DataType(MySqlDbType.VarChar, "100")> Public Property proteincomplex_regulated_by_tf As String
    <DatabaseField("total_enzymes_withconnectivity"), DataType(MySqlDbType.VarChar, "100")> Public Property total_enzymes_withconnectivity As String
    <DatabaseField("enzymes_withconnectivity"), DataType(MySqlDbType.VarChar, "2000")> Public Property enzymes_withconnectivity As String
    <DatabaseField("note"), DataType(MySqlDbType.Text)> Public Property note As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `gensorunit` (`gu_id`, `gu_name`, `gu_description`, `gu_map_file`, `gu_status`, `effector_feedback_paths`, `proteincomplex_regulated_by_tf`, `total_enzymes_withconnectivity`, `enzymes_withconnectivity`, `note`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `gensorunit` (`gu_id`, `gu_name`, `gu_description`, `gu_map_file`, `gu_status`, `effector_feedback_paths`, `proteincomplex_regulated_by_tf`, `total_enzymes_withconnectivity`, `enzymes_withconnectivity`, `note`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `gensorunit` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `gensorunit` SET `gu_id`='{0}', `gu_name`='{1}', `gu_description`='{2}', `gu_map_file`='{3}', `gu_status`='{4}', `effector_feedback_paths`='{5}', `proteincomplex_regulated_by_tf`='{6}', `total_enzymes_withconnectivity`='{7}', `enzymes_withconnectivity`='{8}', `note`='{9}' WHERE ;</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `gensorunit` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `gensorunit` (`gu_id`, `gu_name`, `gu_description`, `gu_map_file`, `gu_status`, `effector_feedback_paths`, `proteincomplex_regulated_by_tf`, `total_enzymes_withconnectivity`, `enzymes_withconnectivity`, `note`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, gu_id, gu_name, gu_description, gu_map_file, gu_status, effector_feedback_paths, proteincomplex_regulated_by_tf, total_enzymes_withconnectivity, enzymes_withconnectivity, note)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{gu_id}', '{gu_name}', '{gu_description}', '{gu_map_file}', '{gu_status}', '{effector_feedback_paths}', '{proteincomplex_regulated_by_tf}', '{total_enzymes_withconnectivity}', '{enzymes_withconnectivity}', '{note}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `gensorunit` (`gu_id`, `gu_name`, `gu_description`, `gu_map_file`, `gu_status`, `effector_feedback_paths`, `proteincomplex_regulated_by_tf`, `total_enzymes_withconnectivity`, `enzymes_withconnectivity`, `note`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, gu_id, gu_name, gu_description, gu_map_file, gu_status, effector_feedback_paths, proteincomplex_regulated_by_tf, total_enzymes_withconnectivity, enzymes_withconnectivity, note)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `gensorunit` SET `gu_id`='{0}', `gu_name`='{1}', `gu_description`='{2}', `gu_map_file`='{3}', `gu_status`='{4}', `effector_feedback_paths`='{5}', `proteincomplex_regulated_by_tf`='{6}', `total_enzymes_withconnectivity`='{7}', `enzymes_withconnectivity`='{8}', `note`='{9}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace
