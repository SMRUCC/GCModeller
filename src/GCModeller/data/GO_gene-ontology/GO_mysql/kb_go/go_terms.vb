#Region "Microsoft.VisualBasic::039a88630be2ba58de2e701cc0bcf2f8, data\GO_gene-ontology\GO_mysql\kb_go\go_terms.vb"

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

    ' Class go_terms
    ' 
    '     Properties: [namespace], comment, def, id, is_obsolete
    '                 name, namespace_id, term
    ' 
    '     Function: Clone, GetDeleteSQL, GetDumpInsertValue, (+2 Overloads) GetInsertSQL, (+2 Overloads) GetReplaceSQL
    '               GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @2018/5/23 13:13:50


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace kb_go

''' <summary>
''' ```SQL
''' GO_term的具体的定义内容
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `go_terms`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `go_terms` (
'''   `id` int(10) unsigned NOT NULL COMMENT '其实就是将term编号之中的``GO:``前缀给删除了而得到的一个数字',
'''   `term` char(16) NOT NULL COMMENT 'GO id',
'''   `name` varchar(45) DEFAULT NULL,
'''   `namespace_id` int(10) unsigned NOT NULL,
'''   `namespace` varchar(45) NOT NULL,
'''   `def` longtext NOT NULL,
'''   `is_obsolete` tinyint(4) NOT NULL DEFAULT '0' COMMENT '0 为 False, 1 为 True',
'''   `comment` longtext,
'''   PRIMARY KEY (`id`),
'''   UNIQUE KEY `id_UNIQUE` (`id`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='GO_term的具体的定义内容';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("go_terms", Database:="kb_go", SchemaSQL:="
CREATE TABLE `go_terms` (
  `id` int(10) unsigned NOT NULL COMMENT '其实就是将term编号之中的``GO:``前缀给删除了而得到的一个数字',
  `term` char(16) NOT NULL COMMENT 'GO id',
  `name` varchar(45) DEFAULT NULL,
  `namespace_id` int(10) unsigned NOT NULL,
  `namespace` varchar(45) NOT NULL,
  `def` longtext NOT NULL,
  `is_obsolete` tinyint(4) NOT NULL DEFAULT '0' COMMENT '0 为 False, 1 为 True',
  `comment` longtext,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='GO_term的具体的定义内容';")>
Public Class go_terms: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
''' <summary>
''' 其实就是将term编号之中的``GO:``前缀给删除了而得到的一个数字
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="id"), XmlAttribute> Public Property id As Long
''' <summary>
''' GO id
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("term"), NotNull, DataType(MySqlDbType.VarChar, "16"), Column(Name:="term")> Public Property term As String
    <DatabaseField("name"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="name")> Public Property name As String
    <DatabaseField("namespace_id"), NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="namespace_id")> Public Property namespace_id As Long
    <DatabaseField("namespace"), NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="namespace")> Public Property [namespace] As String
    <DatabaseField("def"), NotNull, DataType(MySqlDbType.Text), Column(Name:="def")> Public Property def As String
''' <summary>
''' 0 为 False, 1 为 True
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("is_obsolete"), NotNull, DataType(MySqlDbType.Int32, "4"), Column(Name:="is_obsolete")> Public Property is_obsolete As Integer
    <DatabaseField("comment"), DataType(MySqlDbType.Text), Column(Name:="comment")> Public Property comment As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `go_terms` (`id`, `term`, `name`, `namespace_id`, `namespace`, `def`, `is_obsolete`, `comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `go_terms` (`id`, `term`, `name`, `namespace_id`, `namespace`, `def`, `is_obsolete`, `comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `go_terms` (`id`, `term`, `name`, `namespace_id`, `namespace`, `def`, `is_obsolete`, `comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `go_terms` (`id`, `term`, `name`, `namespace_id`, `namespace`, `def`, `is_obsolete`, `comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `go_terms` WHERE `id` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `go_terms` SET `id`='{0}', `term`='{1}', `name`='{2}', `namespace_id`='{3}', `namespace`='{4}', `def`='{5}', `is_obsolete`='{6}', `comment`='{7}' WHERE `id` = '{8}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `go_terms` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `go_terms` (`id`, `term`, `name`, `namespace_id`, `namespace`, `def`, `is_obsolete`, `comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, id, term, name, namespace_id, [namespace], def, is_obsolete, comment)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `go_terms` (`id`, `term`, `name`, `namespace_id`, `namespace`, `def`, `is_obsolete`, `comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, id, term, name, namespace_id, [namespace], def, is_obsolete, comment)
        Else
        Return String.Format(INSERT_SQL, id, term, name, namespace_id, [namespace], def, is_obsolete, comment)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{id}', '{term}', '{name}', '{namespace_id}', '{[namespace]}', '{def}', '{is_obsolete}', '{comment}')"
        Else
            Return $"('{id}', '{term}', '{name}', '{namespace_id}', '{[namespace]}', '{def}', '{is_obsolete}', '{comment}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `go_terms` (`id`, `term`, `name`, `namespace_id`, `namespace`, `def`, `is_obsolete`, `comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, id, term, name, namespace_id, [namespace], def, is_obsolete, comment)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `go_terms` (`id`, `term`, `name`, `namespace_id`, `namespace`, `def`, `is_obsolete`, `comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, id, term, name, namespace_id, [namespace], def, is_obsolete, comment)
        Else
        Return String.Format(REPLACE_SQL, id, term, name, namespace_id, [namespace], def, is_obsolete, comment)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `go_terms` SET `id`='{0}', `term`='{1}', `name`='{2}', `namespace_id`='{3}', `namespace`='{4}', `def`='{5}', `is_obsolete`='{6}', `comment`='{7}' WHERE `id` = '{8}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, term, name, namespace_id, [namespace], def, is_obsolete, comment, id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As go_terms
                         Return DirectCast(MyClass.MemberwiseClone, go_terms)
                     End Function
End Class


End Namespace
