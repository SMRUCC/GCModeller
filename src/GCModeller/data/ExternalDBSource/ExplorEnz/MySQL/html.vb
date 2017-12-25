#Region "Microsoft.VisualBasic::db6a0f62bd3ecacba68a55133ce7608f, ..\GCModeller\data\ExternalDBSource\ExplorEnz\MySQL\html.vb"

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
''' DROP TABLE IF EXISTS `html`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `html` (
'''   `ec_num` varchar(12) NOT NULL DEFAULT '',
'''   `accepted_name` text,
'''   `reaction` text,
'''   `other_names` text,
'''   `sys_name` text,
'''   `comments` text,
'''   `links` text,
'''   `glossary` text,
'''   `last_change` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
'''   PRIMARY KEY (`ec_num`),
'''   UNIQUE KEY `ec_num` (`ec_num`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("html", Database:="enzymed", SchemaSQL:="
CREATE TABLE `html` (
  `ec_num` varchar(12) NOT NULL DEFAULT '',
  `accepted_name` text,
  `reaction` text,
  `other_names` text,
  `sys_name` text,
  `comments` text,
  `links` text,
  `glossary` text,
  `last_change` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`ec_num`),
  UNIQUE KEY `ec_num` (`ec_num`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class html: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("ec_num"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property ec_num As String
    <DatabaseField("accepted_name"), DataType(MySqlDbType.Text)> Public Property accepted_name As String
    <DatabaseField("reaction"), DataType(MySqlDbType.Text)> Public Property reaction As String
    <DatabaseField("other_names"), DataType(MySqlDbType.Text)> Public Property other_names As String
    <DatabaseField("sys_name"), DataType(MySqlDbType.Text)> Public Property sys_name As String
    <DatabaseField("comments"), DataType(MySqlDbType.Text)> Public Property comments As String
    <DatabaseField("links"), DataType(MySqlDbType.Text)> Public Property links As String
    <DatabaseField("glossary"), DataType(MySqlDbType.Text)> Public Property glossary As String
    <DatabaseField("last_change"), NotNull, DataType(MySqlDbType.DateTime)> Public Property last_change As Date
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `html` (`ec_num`, `accepted_name`, `reaction`, `other_names`, `sys_name`, `comments`, `links`, `glossary`, `last_change`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `html` (`ec_num`, `accepted_name`, `reaction`, `other_names`, `sys_name`, `comments`, `links`, `glossary`, `last_change`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `html` WHERE `ec_num` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `html` SET `ec_num`='{0}', `accepted_name`='{1}', `reaction`='{2}', `other_names`='{3}', `sys_name`='{4}', `comments`='{5}', `links`='{6}', `glossary`='{7}', `last_change`='{8}' WHERE `ec_num` = '{9}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `html` WHERE `ec_num` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, ec_num)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `html` (`ec_num`, `accepted_name`, `reaction`, `other_names`, `sys_name`, `comments`, `links`, `glossary`, `last_change`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, ec_num, accepted_name, reaction, other_names, sys_name, comments, links, glossary, DataType.ToMySqlDateTimeString(last_change))
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{ec_num}', '{accepted_name}', '{reaction}', '{other_names}', '{sys_name}', '{comments}', '{links}', '{glossary}', '{last_change}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `html` (`ec_num`, `accepted_name`, `reaction`, `other_names`, `sys_name`, `comments`, `links`, `glossary`, `last_change`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, ec_num, accepted_name, reaction, other_names, sys_name, comments, links, glossary, DataType.ToMySqlDateTimeString(last_change))
    End Function
''' <summary>
''' ```SQL
''' UPDATE `html` SET `ec_num`='{0}', `accepted_name`='{1}', `reaction`='{2}', `other_names`='{3}', `sys_name`='{4}', `comments`='{5}', `links`='{6}', `glossary`='{7}', `last_change`='{8}' WHERE `ec_num` = '{9}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, ec_num, accepted_name, reaction, other_names, sys_name, comments, links, glossary, DataType.ToMySqlDateTimeString(last_change), ec_num)
    End Function
#End Region
End Class


End Namespace
