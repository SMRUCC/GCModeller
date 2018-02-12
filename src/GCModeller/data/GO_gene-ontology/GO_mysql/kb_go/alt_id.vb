#Region "Microsoft.VisualBasic::32905df63829f6d82683fee301c9a33f, data\GO_gene-ontology\GO_mysql\kb_go\alt_id.vb"

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

    ' Class alt_id
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @2017/9/3 12:29:34


Imports System.Data.Linq.Mapping
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports System.Xml.Serialization

Namespace kb_go

''' <summary>
''' ```SQL
''' GO_term的主编号和次级编号之间的关系
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `alt_id`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `alt_id` (
'''   `id` int(10) unsigned NOT NULL,
'''   `alt_id` int(10) unsigned NOT NULL,
'''   `name` mediumtext COMMENT 'The name field in the go_term',
'''   PRIMARY KEY (`id`,`alt_id`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='GO_term的主编号和次级编号之间的关系';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("alt_id", Database:="kb_go", SchemaSQL:="
CREATE TABLE `alt_id` (
  `id` int(10) unsigned NOT NULL,
  `alt_id` int(10) unsigned NOT NULL,
  `name` mediumtext COMMENT 'The name field in the go_term',
  PRIMARY KEY (`id`,`alt_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='GO_term的主编号和次级编号之间的关系';")>
Public Class alt_id: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="id"), XmlAttribute> Public Property id As Long
    <DatabaseField("alt_id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="alt_id"), XmlAttribute> Public Property alt_id As Long
''' <summary>
''' The name field in the go_term
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("name"), DataType(MySqlDbType.Text), Column(Name:="name")> Public Property name As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `alt_id` (`id`, `alt_id`, `name`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `alt_id` (`id`, `alt_id`, `name`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `alt_id` WHERE `id`='{0}' and `alt_id`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `alt_id` SET `id`='{0}', `alt_id`='{1}', `name`='{2}' WHERE `id`='{3}' and `alt_id`='{4}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `alt_id` WHERE `id`='{0}' and `alt_id`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id, alt_id)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `alt_id` (`id`, `alt_id`, `name`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, id, alt_id, name)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{id}', '{alt_id}', '{name}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `alt_id` (`id`, `alt_id`, `name`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, id, alt_id, name)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `alt_id` SET `id`='{0}', `alt_id`='{1}', `name`='{2}' WHERE `id`='{3}' and `alt_id`='{4}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, alt_id, name, id, alt_id)
    End Function
#End Region
Public Function Clone() As alt_id
                  Return DirectCast(MyClass.MemberwiseClone, alt_id)
              End Function
End Class


End Namespace
