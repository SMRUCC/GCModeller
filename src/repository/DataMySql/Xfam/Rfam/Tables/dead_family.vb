#Region "Microsoft.VisualBasic::140f14eab32094e80f2c42bf19e290fe, ..\repository\DataMySql\Xfam\Rfam\Tables\dead_family.vb"

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

REM  Dump @3/29/2017 11:55:32 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace Xfam.Rfam.MySQL.Tables

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `dead_family`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `dead_family` (
'''   `rfam_acc` varchar(7) NOT NULL DEFAULT '' COMMENT 'record the author???',
'''   `rfam_id` varchar(40) NOT NULL,
'''   `comment` mediumtext,
'''   `forward_to` varchar(7) DEFAULT NULL,
'''   `title` varchar(150) DEFAULT NULL COMMENT 'wikipedia page title\n',
'''   `user` tinytext NOT NULL,
'''   UNIQUE KEY `rfam_acc` (`rfam_acc`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("dead_family", Database:="rfam_12_2", SchemaSQL:="
CREATE TABLE `dead_family` (
  `rfam_acc` varchar(7) NOT NULL DEFAULT '' COMMENT 'record the author???',
  `rfam_id` varchar(40) NOT NULL,
  `comment` mediumtext,
  `forward_to` varchar(7) DEFAULT NULL,
  `title` varchar(150) DEFAULT NULL COMMENT 'wikipedia page title\n',
  `user` tinytext NOT NULL,
  UNIQUE KEY `rfam_acc` (`rfam_acc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class dead_family: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
''' <summary>
''' record the author???
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("rfam_acc"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "7")> Public Property rfam_acc As String
    <DatabaseField("rfam_id"), NotNull, DataType(MySqlDbType.VarChar, "40")> Public Property rfam_id As String
    <DatabaseField("comment"), DataType(MySqlDbType.Text)> Public Property comment As String
    <DatabaseField("forward_to"), DataType(MySqlDbType.VarChar, "7")> Public Property forward_to As String
''' <summary>
''' wikipedia page title\n
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("title"), DataType(MySqlDbType.VarChar, "150")> Public Property title As String
    <DatabaseField("user"), NotNull, DataType(MySqlDbType.Text)> Public Property user As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `dead_family` (`rfam_acc`, `rfam_id`, `comment`, `forward_to`, `title`, `user`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `dead_family` (`rfam_acc`, `rfam_id`, `comment`, `forward_to`, `title`, `user`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `dead_family` WHERE `rfam_acc` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `dead_family` SET `rfam_acc`='{0}', `rfam_id`='{1}', `comment`='{2}', `forward_to`='{3}', `title`='{4}', `user`='{5}' WHERE `rfam_acc` = '{6}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `dead_family` WHERE `rfam_acc` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, rfam_acc)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `dead_family` (`rfam_acc`, `rfam_id`, `comment`, `forward_to`, `title`, `user`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, rfam_acc, rfam_id, comment, forward_to, title, user)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{rfam_acc}', '{rfam_id}', '{comment}', '{forward_to}', '{title}', '{user}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `dead_family` (`rfam_acc`, `rfam_id`, `comment`, `forward_to`, `title`, `user`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, rfam_acc, rfam_id, comment, forward_to, title, user)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `dead_family` SET `rfam_acc`='{0}', `rfam_id`='{1}', `comment`='{2}', `forward_to`='{3}', `title`='{4}', `user`='{5}' WHERE `rfam_acc` = '{6}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, rfam_acc, rfam_id, comment, forward_to, title, user, rfam_acc)
    End Function
#End Region
End Class


End Namespace

