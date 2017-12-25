#Region "Microsoft.VisualBasic::8df9d077ae30c9497c87e999b15b1b87, ..\GCModeller\data\RegulonDatabase\RegulonDB\MySQL\evidence.vb"

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
''' DROP TABLE IF EXISTS `evidence`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `evidence` (
'''   `evidence_id` char(12) NOT NULL,
'''   `evidence_name` varchar(2000) NOT NULL,
'''   `type_object` varchar(200) DEFAULT NULL,
'''   `evidence_code` varchar(30) DEFAULT NULL,
'''   `evidence_note` varchar(2000) DEFAULT NULL,
'''   `evidence_internal_comment` longtext,
'''   `key_id_org` varchar(5) NOT NULL,
'''   `evidence_type` char(3) DEFAULT NULL,
'''   `evidence_category` varchar(200) DEFAULT NULL,
'''   `head` varchar(12) DEFAULT NULL,
'''   `example` varchar(500) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("evidence", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `evidence` (
  `evidence_id` char(12) NOT NULL,
  `evidence_name` varchar(2000) NOT NULL,
  `type_object` varchar(200) DEFAULT NULL,
  `evidence_code` varchar(30) DEFAULT NULL,
  `evidence_note` varchar(2000) DEFAULT NULL,
  `evidence_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL,
  `evidence_type` char(3) DEFAULT NULL,
  `evidence_category` varchar(200) DEFAULT NULL,
  `head` varchar(12) DEFAULT NULL,
  `example` varchar(500) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class evidence: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("evidence_id"), NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property evidence_id As String
    <DatabaseField("evidence_name"), NotNull, DataType(MySqlDbType.VarChar, "2000")> Public Property evidence_name As String
    <DatabaseField("type_object"), DataType(MySqlDbType.VarChar, "200")> Public Property type_object As String
    <DatabaseField("evidence_code"), DataType(MySqlDbType.VarChar, "30")> Public Property evidence_code As String
    <DatabaseField("evidence_note"), DataType(MySqlDbType.VarChar, "2000")> Public Property evidence_note As String
    <DatabaseField("evidence_internal_comment"), DataType(MySqlDbType.Text)> Public Property evidence_internal_comment As String
    <DatabaseField("key_id_org"), NotNull, DataType(MySqlDbType.VarChar, "5")> Public Property key_id_org As String
    <DatabaseField("evidence_type"), DataType(MySqlDbType.VarChar, "3")> Public Property evidence_type As String
    <DatabaseField("evidence_category"), DataType(MySqlDbType.VarChar, "200")> Public Property evidence_category As String
    <DatabaseField("head"), DataType(MySqlDbType.VarChar, "12")> Public Property head As String
    <DatabaseField("example"), DataType(MySqlDbType.VarChar, "500")> Public Property example As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `evidence` (`evidence_id`, `evidence_name`, `type_object`, `evidence_code`, `evidence_note`, `evidence_internal_comment`, `key_id_org`, `evidence_type`, `evidence_category`, `head`, `example`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `evidence` (`evidence_id`, `evidence_name`, `type_object`, `evidence_code`, `evidence_note`, `evidence_internal_comment`, `key_id_org`, `evidence_type`, `evidence_category`, `head`, `example`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `evidence` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `evidence` SET `evidence_id`='{0}', `evidence_name`='{1}', `type_object`='{2}', `evidence_code`='{3}', `evidence_note`='{4}', `evidence_internal_comment`='{5}', `key_id_org`='{6}', `evidence_type`='{7}', `evidence_category`='{8}', `head`='{9}', `example`='{10}' WHERE ;</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `evidence` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `evidence` (`evidence_id`, `evidence_name`, `type_object`, `evidence_code`, `evidence_note`, `evidence_internal_comment`, `key_id_org`, `evidence_type`, `evidence_category`, `head`, `example`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, evidence_id, evidence_name, type_object, evidence_code, evidence_note, evidence_internal_comment, key_id_org, evidence_type, evidence_category, head, example)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{evidence_id}', '{evidence_name}', '{type_object}', '{evidence_code}', '{evidence_note}', '{evidence_internal_comment}', '{key_id_org}', '{evidence_type}', '{evidence_category}', '{head}', '{example}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `evidence` (`evidence_id`, `evidence_name`, `type_object`, `evidence_code`, `evidence_note`, `evidence_internal_comment`, `key_id_org`, `evidence_type`, `evidence_category`, `head`, `example`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, evidence_id, evidence_name, type_object, evidence_code, evidence_note, evidence_internal_comment, key_id_org, evidence_type, evidence_category, head, example)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `evidence` SET `evidence_id`='{0}', `evidence_name`='{1}', `type_object`='{2}', `evidence_code`='{3}', `evidence_note`='{4}', `evidence_internal_comment`='{5}', `key_id_org`='{6}', `evidence_type`='{7}', `evidence_category`='{8}', `head`='{9}', `example`='{10}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace
