#Region "Microsoft.VisualBasic::3f95b12b00a846da6271b1ffd5519928, data\GO_gene-ontology\GO_mysql\kb_go\xref.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:

    ' Class xref
    ' 
    '     Properties: comment, external_id, go_id, xref
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @3/16/2018 10:37:28 PM


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace kb_go

''' <summary>
''' ```SQL
''' GO_term与外部数据库之间的相互关联
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `xref`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `xref` (
'''   `go_id` int(10) unsigned NOT NULL,
'''   `xref` varchar(45) NOT NULL COMMENT '外部数据库名称',
'''   `external_id` varchar(45) NOT NULL COMMENT '外部数据库编号',
'''   `comment` longtext,
'''   PRIMARY KEY (`go_id`,`external_id`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='GO_term与外部数据库之间的相互关联';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' -- Dumping events for database 'kb_go'
''' --
''' 
''' --
''' -- Dumping routines for database 'kb_go'
''' --
''' /*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;
''' 
''' /*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
''' /*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
''' /*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
''' /*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
''' /*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
''' /*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
''' /*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
''' 
''' -- Dump completed on 2017-09-03  2:55:21
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("xref", Database:="kb_go", SchemaSQL:="
CREATE TABLE `xref` (
  `go_id` int(10) unsigned NOT NULL,
  `xref` varchar(45) NOT NULL COMMENT '外部数据库名称',
  `external_id` varchar(45) NOT NULL COMMENT '外部数据库编号',
  `comment` longtext,
  PRIMARY KEY (`go_id`,`external_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='GO_term与外部数据库之间的相互关联';")>
Public Class xref: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("go_id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="go_id"), XmlAttribute> Public Property go_id As Long
''' <summary>
''' 外部数据库名称
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("xref"), NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="xref")> Public Property xref As String
''' <summary>
''' 外部数据库编号
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("external_id"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="external_id"), XmlAttribute> Public Property external_id As String
    <DatabaseField("comment"), DataType(MySqlDbType.Text), Column(Name:="comment")> Public Property comment As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `xref` (`go_id`, `xref`, `external_id`, `comment`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `xref` (`go_id`, `xref`, `external_id`, `comment`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `xref` WHERE `go_id`='{0}' and `external_id`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `xref` SET `go_id`='{0}', `xref`='{1}', `external_id`='{2}', `comment`='{3}' WHERE `go_id`='{4}' and `external_id`='{5}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `xref` WHERE `go_id`='{0}' and `external_id`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, go_id, external_id)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `xref` (`go_id`, `xref`, `external_id`, `comment`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, go_id, xref, external_id, comment)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{go_id}', '{xref}', '{external_id}', '{comment}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `xref` (`go_id`, `xref`, `external_id`, `comment`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, go_id, xref, external_id, comment)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `xref` SET `go_id`='{0}', `xref`='{1}', `external_id`='{2}', `comment`='{3}' WHERE `go_id`='{4}' and `external_id`='{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, go_id, xref, external_id, comment, go_id, external_id)
    End Function
#End Region
Public Function Clone() As xref
                  Return DirectCast(MyClass.MemberwiseClone, xref)
              End Function
End Class


End Namespace
