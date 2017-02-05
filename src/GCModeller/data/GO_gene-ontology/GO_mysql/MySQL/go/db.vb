#Region "Microsoft.VisualBasic::6d1dafdc52a8d6776b85ba2074764b3a, ..\GCModeller\data\GO_gene-ontology\GeneOntology\MySQL\go\db.vb"

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
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @9/5/2016 7:59:33 AM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MySQL.Tables

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `db`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `db` (
'''   `id` int(11) NOT NULL AUTO_INCREMENT,
'''   `name` varchar(55) DEFAULT NULL,
'''   `fullname` varchar(255) DEFAULT NULL,
'''   `datatype` varchar(255) DEFAULT NULL,
'''   `generic_url` varchar(255) DEFAULT NULL,
'''   `url_syntax` varchar(255) DEFAULT NULL,
'''   `url_example` varchar(255) DEFAULT NULL,
'''   `uri_prefix` varchar(255) DEFAULT NULL,
'''   PRIMARY KEY (`id`),
'''   UNIQUE KEY `db0` (`id`),
'''   UNIQUE KEY `name` (`name`),
'''   KEY `db1` (`name`),
'''   KEY `db2` (`fullname`),
'''   KEY `db3` (`datatype`)
''' ) ENGINE=MyISAM AUTO_INCREMENT=262 DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("db", Database:="go", SchemaSQL:="
CREATE TABLE `db` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(55) DEFAULT NULL,
  `fullname` varchar(255) DEFAULT NULL,
  `datatype` varchar(255) DEFAULT NULL,
  `generic_url` varchar(255) DEFAULT NULL,
  `url_syntax` varchar(255) DEFAULT NULL,
  `url_example` varchar(255) DEFAULT NULL,
  `uri_prefix` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `db0` (`id`),
  UNIQUE KEY `name` (`name`),
  KEY `db1` (`name`),
  KEY `db2` (`fullname`),
  KEY `db3` (`datatype`)
) ENGINE=MyISAM AUTO_INCREMENT=262 DEFAULT CHARSET=latin1;")>
Public Class db: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property id As Long
    <DatabaseField("name"), DataType(MySqlDbType.VarChar, "55")> Public Property name As String
    <DatabaseField("fullname"), DataType(MySqlDbType.VarChar, "255")> Public Property fullname As String
    <DatabaseField("datatype"), DataType(MySqlDbType.VarChar, "255")> Public Property datatype As String
    <DatabaseField("generic_url"), DataType(MySqlDbType.VarChar, "255")> Public Property generic_url As String
    <DatabaseField("url_syntax"), DataType(MySqlDbType.VarChar, "255")> Public Property url_syntax As String
    <DatabaseField("url_example"), DataType(MySqlDbType.VarChar, "255")> Public Property url_example As String
    <DatabaseField("uri_prefix"), DataType(MySqlDbType.VarChar, "255")> Public Property uri_prefix As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `db` (`name`, `fullname`, `datatype`, `generic_url`, `url_syntax`, `url_example`, `uri_prefix`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `db` (`name`, `fullname`, `datatype`, `generic_url`, `url_syntax`, `url_example`, `uri_prefix`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `db` WHERE `id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `db` SET `id`='{0}', `name`='{1}', `fullname`='{2}', `datatype`='{3}', `generic_url`='{4}', `url_syntax`='{5}', `url_example`='{6}', `uri_prefix`='{7}' WHERE `id` = '{8}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `db` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `db` (`name`, `fullname`, `datatype`, `generic_url`, `url_syntax`, `url_example`, `uri_prefix`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, name, fullname, datatype, generic_url, url_syntax, url_example, uri_prefix)
    End Function
''' <summary>
''' ```SQL
''' REPLACE INTO `db` (`name`, `fullname`, `datatype`, `generic_url`, `url_syntax`, `url_example`, `uri_prefix`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, name, fullname, datatype, generic_url, url_syntax, url_example, uri_prefix)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `db` SET `id`='{0}', `name`='{1}', `fullname`='{2}', `datatype`='{3}', `generic_url`='{4}', `url_syntax`='{5}', `url_example`='{6}', `uri_prefix`='{7}' WHERE `id` = '{8}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, name, fullname, datatype, generic_url, url_syntax, url_example, uri_prefix, id)
    End Function
#End Region
End Class


End Namespace
