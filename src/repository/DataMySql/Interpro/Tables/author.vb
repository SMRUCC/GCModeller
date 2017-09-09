#Region "Microsoft.VisualBasic::6c64e269c1916a46e55183a163edd555, ..\repository\DataMySql\Interpro\Tables\author.vb"

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
''' DROP TABLE IF EXISTS `author`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `author` (
'''   `author_id` int(9) NOT NULL,
'''   `name` varchar(80) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `uppercase` varchar(80) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   PRIMARY KEY (`author_id`),
'''   UNIQUE KEY `ui_author$id$name` (`author_id`,`name`),
'''   UNIQUE KEY `uq_author$name` (`name`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("author", Database:="interpro", SchemaSQL:="
CREATE TABLE `author` (
  `author_id` int(9) NOT NULL,
  `name` varchar(80) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `uppercase` varchar(80) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  PRIMARY KEY (`author_id`),
  UNIQUE KEY `ui_author$id$name` (`author_id`,`name`),
  UNIQUE KEY `uq_author$name` (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class author: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("author_id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "9")> Public Property author_id As Long
    <DatabaseField("name"), NotNull, DataType(MySqlDbType.VarChar, "80")> Public Property name As String
    <DatabaseField("uppercase"), NotNull, DataType(MySqlDbType.VarChar, "80")> Public Property uppercase As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `author` (`author_id`, `name`, `uppercase`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `author` (`author_id`, `name`, `uppercase`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `author` WHERE `author_id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `author` SET `author_id`='{0}', `name`='{1}', `uppercase`='{2}' WHERE `author_id` = '{3}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `author` WHERE `author_id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, author_id)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `author` (`author_id`, `name`, `uppercase`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, author_id, name, uppercase)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{author_id}', '{name}', '{uppercase}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `author` (`author_id`, `name`, `uppercase`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, author_id, name, uppercase)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `author` SET `author_id`='{0}', `name`='{1}', `uppercase`='{2}' WHERE `author_id` = '{3}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, author_id, name, uppercase, author_id)
    End Function
#End Region
End Class


End Namespace

