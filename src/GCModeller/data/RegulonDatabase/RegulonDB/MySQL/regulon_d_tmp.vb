#Region "Microsoft.VisualBasic::5e0efda76a68ee61e85b4665aac8c48e, ..\GCModeller\data\RegulonDatabase\RegulonDB\MySQL\regulon_d_tmp.vb"

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
''' DROP TABLE IF EXISTS `regulon_d_tmp`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `regulon_d_tmp` (
'''   `re_id` decimal(10,0) NOT NULL,
'''   `regulon_id` char(12) NOT NULL,
'''   `regulon_name` varchar(500) DEFAULT NULL,
'''   `regulon_key_id_org` char(5) NOT NULL,
'''   `regulon_tf_group` decimal(10,0) NOT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("regulon_d_tmp", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `regulon_d_tmp` (
  `re_id` decimal(10,0) NOT NULL,
  `regulon_id` char(12) NOT NULL,
  `regulon_name` varchar(500) DEFAULT NULL,
  `regulon_key_id_org` char(5) NOT NULL,
  `regulon_tf_group` decimal(10,0) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class regulon_d_tmp: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("re_id"), NotNull, DataType(MySqlDbType.Decimal)> Public Property re_id As Decimal
    <DatabaseField("regulon_id"), NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property regulon_id As String
    <DatabaseField("regulon_name"), DataType(MySqlDbType.VarChar, "500")> Public Property regulon_name As String
    <DatabaseField("regulon_key_id_org"), NotNull, DataType(MySqlDbType.VarChar, "5")> Public Property regulon_key_id_org As String
    <DatabaseField("regulon_tf_group"), NotNull, DataType(MySqlDbType.Decimal)> Public Property regulon_tf_group As Decimal
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `regulon_d_tmp` (`re_id`, `regulon_id`, `regulon_name`, `regulon_key_id_org`, `regulon_tf_group`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `regulon_d_tmp` (`re_id`, `regulon_id`, `regulon_name`, `regulon_key_id_org`, `regulon_tf_group`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `regulon_d_tmp` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `regulon_d_tmp` SET `re_id`='{0}', `regulon_id`='{1}', `regulon_name`='{2}', `regulon_key_id_org`='{3}', `regulon_tf_group`='{4}' WHERE ;</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `regulon_d_tmp` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `regulon_d_tmp` (`re_id`, `regulon_id`, `regulon_name`, `regulon_key_id_org`, `regulon_tf_group`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, re_id, regulon_id, regulon_name, regulon_key_id_org, regulon_tf_group)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{re_id}', '{regulon_id}', '{regulon_name}', '{regulon_key_id_org}', '{regulon_tf_group}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `regulon_d_tmp` (`re_id`, `regulon_id`, `regulon_name`, `regulon_key_id_org`, `regulon_tf_group`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, re_id, regulon_id, regulon_name, regulon_key_id_org, regulon_tf_group)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `regulon_d_tmp` SET `re_id`='{0}', `regulon_id`='{1}', `regulon_name`='{2}', `regulon_key_id_org`='{3}', `regulon_tf_group`='{4}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace
