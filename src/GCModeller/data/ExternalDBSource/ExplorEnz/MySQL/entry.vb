#Region "Microsoft.VisualBasic::1e8ff08c45331baee3e1c9be1dc8b325, ..\GCModeller\data\ExternalDBSource\ExplorEnz\MySQL\entry.vb"

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

REM  Dump @12/3/2015 7:59:04 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace ExplorEnz.MySQL

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `entry`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `entry` (
'''   `ec_num` varchar(12) NOT NULL DEFAULT '',
'''   `accepted_name` varchar(300) DEFAULT NULL,
'''   `reaction` text,
'''   `other_names` text,
'''   `sys_name` text,
'''   `comments` text,
'''   `links` text,
'''   `class` int(1) DEFAULT NULL,
'''   `subclass` int(1) DEFAULT NULL,
'''   `subsubclass` int(1) DEFAULT NULL,
'''   `serial` int(1) DEFAULT NULL,
'''   `status` char(3) DEFAULT NULL,
'''   `diagram` varchar(255) DEFAULT NULL,
'''   `cas_num` varchar(100) DEFAULT NULL,
'''   `glossary` text,
'''   `last_change` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
'''   `id` int(11) NOT NULL AUTO_INCREMENT,
'''   UNIQUE KEY `id` (`id`),
'''   UNIQUE KEY `ec_num` (`ec_num`),
'''   FULLTEXT KEY `ec_num_2` (`ec_num`,`accepted_name`,`reaction`,`other_names`,`sys_name`,`comments`,`links`,`diagram`,`cas_num`,`glossary`)
''' ) ENGINE=MyISAM AUTO_INCREMENT=6616 DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("entry", Database:="enzymed")>
Public Class entry: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("ec_num"), NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property ec_num As String
    <DatabaseField("accepted_name"), DataType(MySqlDbType.VarChar, "300")> Public Property accepted_name As String
    <DatabaseField("reaction"), DataType(MySqlDbType.Text)> Public Property reaction As String
    <DatabaseField("other_names"), DataType(MySqlDbType.Text)> Public Property other_names As String
    <DatabaseField("sys_name"), DataType(MySqlDbType.Text)> Public Property sys_name As String
    <DatabaseField("comments"), DataType(MySqlDbType.Text)> Public Property comments As String
    <DatabaseField("links"), DataType(MySqlDbType.Text)> Public Property links As String
    <DatabaseField("class"), DataType(MySqlDbType.Int64, "1")> Public Property [class] As Long
    <DatabaseField("subclass"), DataType(MySqlDbType.Int64, "1")> Public Property subclass As Long
    <DatabaseField("subsubclass"), DataType(MySqlDbType.Int64, "1")> Public Property subsubclass As Long
    <DatabaseField("serial"), DataType(MySqlDbType.Int64, "1")> Public Property serial As Long
    <DatabaseField("status"), DataType(MySqlDbType.VarChar, "3")> Public Property status As String
    <DatabaseField("diagram"), DataType(MySqlDbType.VarChar, "255")> Public Property diagram As String
    <DatabaseField("cas_num"), DataType(MySqlDbType.VarChar, "100")> Public Property cas_num As String
    <DatabaseField("glossary"), DataType(MySqlDbType.Text)> Public Property glossary As String
    <DatabaseField("last_change"), NotNull, DataType(MySqlDbType.DateTime)> Public Property last_change As Date
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property id As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `entry` (`ec_num`, `accepted_name`, `reaction`, `other_names`, `sys_name`, `comments`, `links`, `class`, `subclass`, `subsubclass`, `serial`, `status`, `diagram`, `cas_num`, `glossary`, `last_change`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `entry` (`ec_num`, `accepted_name`, `reaction`, `other_names`, `sys_name`, `comments`, `links`, `class`, `subclass`, `subsubclass`, `serial`, `status`, `diagram`, `cas_num`, `glossary`, `last_change`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `entry` WHERE `id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `entry` SET `ec_num`='{0}', `accepted_name`='{1}', `reaction`='{2}', `other_names`='{3}', `sys_name`='{4}', `comments`='{5}', `links`='{6}', `class`='{7}', `subclass`='{8}', `subsubclass`='{9}', `serial`='{10}', `status`='{11}', `diagram`='{12}', `cas_num`='{13}', `glossary`='{14}', `last_change`='{15}', `id`='{16}' WHERE `id` = '{17}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, ec_num, accepted_name, reaction, other_names, sys_name, comments, links, [class], subclass, subsubclass, serial, status, diagram, cas_num, glossary, DataType.ToMySqlDateTimeString(last_change))
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, ec_num, accepted_name, reaction, other_names, sys_name, comments, links, [class], subclass, subsubclass, serial, status, diagram, cas_num, glossary, DataType.ToMySqlDateTimeString(last_change))
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, ec_num, accepted_name, reaction, other_names, sys_name, comments, links, [class], subclass, subsubclass, serial, status, diagram, cas_num, glossary, DataType.ToMySqlDateTimeString(last_change), id, id)
    End Function
#End Region
End Class


End Namespace
