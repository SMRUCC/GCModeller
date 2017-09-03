#Region "Microsoft.VisualBasic::5541995841edb889782286822900b31c, ..\repository\DataMySql\Interpro\Tables\entry.vb"

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

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @3/29/2017 10:21:21 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace Interpro.Tables

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `entry`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `entry` (
'''   `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `entry_type` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `name` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   `created` datetime NOT NULL,
'''   `short_name` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   PRIMARY KEY (`entry_ac`),
'''   KEY `i_fk_entry$entry_type` (`entry_type`),
'''   KEY `fk_entry$entry_type` (`entry_type`),
'''   CONSTRAINT `fk_entry$entry_type` FOREIGN KEY (`entry_type`) REFERENCES `cv_entry_type` (`code`) ON DELETE NO ACTION ON UPDATE NO ACTION
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("entry", Database:="interpro", SchemaSQL:="
CREATE TABLE `entry` (
  `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `entry_type` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `name` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `created` datetime NOT NULL,
  `short_name` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  PRIMARY KEY (`entry_ac`),
  KEY `i_fk_entry$entry_type` (`entry_type`),
  KEY `fk_entry$entry_type` (`entry_type`),
  CONSTRAINT `fk_entry$entry_type` FOREIGN KEY (`entry_type`) REFERENCES `cv_entry_type` (`code`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class entry: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("entry_ac"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "9")> Public Property entry_ac As String
    <DatabaseField("entry_type"), NotNull, DataType(MySqlDbType.VarChar, "1")> Public Property entry_type As String
    <DatabaseField("name"), DataType(MySqlDbType.VarChar, "100")> Public Property name As String
    <DatabaseField("created"), NotNull, DataType(MySqlDbType.DateTime)> Public Property created As Date
    <DatabaseField("short_name"), DataType(MySqlDbType.VarChar, "30")> Public Property short_name As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `entry` (`entry_ac`, `entry_type`, `name`, `created`, `short_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `entry` (`entry_ac`, `entry_type`, `name`, `created`, `short_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `entry` WHERE `entry_ac` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `entry` SET `entry_ac`='{0}', `entry_type`='{1}', `name`='{2}', `created`='{3}', `short_name`='{4}' WHERE `entry_ac` = '{5}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `entry` WHERE `entry_ac` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, entry_ac)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `entry` (`entry_ac`, `entry_type`, `name`, `created`, `short_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, entry_ac, entry_type, name, DataType.ToMySqlDateTimeString(created), short_name)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{entry_ac}', '{entry_type}', '{name}', '{created}', '{short_name}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `entry` (`entry_ac`, `entry_type`, `name`, `created`, `short_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, entry_ac, entry_type, name, DataType.ToMySqlDateTimeString(created), short_name)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `entry` SET `entry_ac`='{0}', `entry_type`='{1}', `name`='{2}', `created`='{3}', `short_name`='{4}' WHERE `entry_ac` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, entry_ac, entry_type, name, DataType.ToMySqlDateTimeString(created), short_name, entry_ac)
    End Function
#End Region
End Class


End Namespace

