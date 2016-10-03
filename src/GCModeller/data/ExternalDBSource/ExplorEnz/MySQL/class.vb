#Region "Microsoft.VisualBasic::54cd127b42c21d5f3f2edc32199b5f0a, ..\GCModeller\data\ExternalDBSource\ExplorEnz\MySQL\class.vb"

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
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("class", Database:="enzymed")>
Public Class [class]: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
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
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `class` (`class`, `subclass`, `subsubclass`, `heading`, `note`, `last_change`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `class` (`class`, `subclass`, `subsubclass`, `heading`, `note`, `last_change`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `class` WHERE `id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `class` SET `id`='{0}', `class`='{1}', `subclass`='{2}', `subsubclass`='{3}', `heading`='{4}', `note`='{5}', `last_change`='{6}' WHERE `id` = '{7}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, [class], subclass, subsubclass, heading, note, DataType.ToMySqlDateTimeString(last_change))
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, [class], subclass, subsubclass, heading, note, DataType.ToMySqlDateTimeString(last_change))
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, [class], subclass, subsubclass, heading, note, DataType.ToMySqlDateTimeString(last_change), id)
    End Function
#End Region
End Class


End Namespace
