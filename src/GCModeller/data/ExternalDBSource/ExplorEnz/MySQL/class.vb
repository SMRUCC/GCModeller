#Region "Microsoft.VisualBasic::9e8471da9305009267a68a6ab482385c, ..\GCModeller\data\ExternalDBSource\ExplorEnz\MySQL\class.vb"

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

REM  Dump @3/29/2017 8:48:50 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace ExplorEnz.MySQL

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `class`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `class` (
'''   `id` int(11) NOT NULL AUTO_INCREMENT,
'''   `class` int(11) NOT NULL DEFAULT '0',
'''   `subclass` int(11) DEFAULT NULL,
'''   `subsubclass` int(11) DEFAULT NULL,
'''   `heading` varchar(255) DEFAULT NULL,
'''   `note` text,
'''   `last_change` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
'''   PRIMARY KEY (`id`)
''' ) ENGINE=MyISAM AUTO_INCREMENT=631 DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("class", Database:="enzymed", SchemaSQL:="
CREATE TABLE `class` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `class` int(11) NOT NULL DEFAULT '0',
  `subclass` int(11) DEFAULT NULL,
  `subsubclass` int(11) DEFAULT NULL,
  `heading` varchar(255) DEFAULT NULL,
  `note` text,
  `last_change` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=631 DEFAULT CHARSET=latin1;")>
Public Class [class]: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property id As Long
    <DatabaseField("class"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property [class] As Long
    <DatabaseField("subclass"), DataType(MySqlDbType.Int64, "11")> Public Property subclass As Long
    <DatabaseField("subsubclass"), DataType(MySqlDbType.Int64, "11")> Public Property subsubclass As Long
    <DatabaseField("heading"), DataType(MySqlDbType.VarChar, "255")> Public Property heading As String
    <DatabaseField("note"), DataType(MySqlDbType.Text)> Public Property note As String
    <DatabaseField("last_change"), NotNull, DataType(MySqlDbType.DateTime)> Public Property last_change As Date
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `class` (`class`, `subclass`, `subsubclass`, `heading`, `note`, `last_change`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `class` (`class`, `subclass`, `subsubclass`, `heading`, `note`, `last_change`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `class` WHERE `id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `class` SET `id`='{0}', `class`='{1}', `subclass`='{2}', `subsubclass`='{3}', `heading`='{4}', `note`='{5}', `last_change`='{6}' WHERE `id` = '{7}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `class` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `class` (`class`, `subclass`, `subsubclass`, `heading`, `note`, `last_change`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, [class], subclass, subsubclass, heading, note, DataType.ToMySqlDateTimeString(last_change))
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{[class]}', '{subclass}', '{subsubclass}', '{heading}', '{note}', '{last_change}', '{6}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `class` (`class`, `subclass`, `subsubclass`, `heading`, `note`, `last_change`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, [class], subclass, subsubclass, heading, note, DataType.ToMySqlDateTimeString(last_change))
    End Function
''' <summary>
''' ```SQL
''' UPDATE `class` SET `id`='{0}', `class`='{1}', `subclass`='{2}', `subsubclass`='{3}', `heading`='{4}', `note`='{5}', `last_change`='{6}' WHERE `id` = '{7}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, [class], subclass, subsubclass, heading, note, DataType.ToMySqlDateTimeString(last_change), id)
    End Function
#End Region
End Class


End Namespace
