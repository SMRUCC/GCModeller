#Region "Microsoft.VisualBasic::639585931d5b81f51f5e5c6e60a48b58, ..\GCModeller\data\GO_gene-ontology\GO_mysql\kb_go\term_synonym.vb"

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
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @2017/9/3 12:29:34


Imports System.Data.Linq.Mapping
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports System.Xml.Serialization

Namespace kb_go

''' <summary>
''' ```SQL
''' GO_term的同义词表
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `term_synonym`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `term_synonym` (
'''   `id` int(10) unsigned NOT NULL AUTO_INCREMENT COMMENT '自增编号',
'''   `term_id` int(10) unsigned NOT NULL COMMENT '当前的Go term的编号',
'''   `synonym` mediumtext NOT NULL COMMENT '同义名称',
'''   `type` varchar(45) DEFAULT NULL COMMENT 'EXACT []  表示完全一样\nRELATED [EC:3.1.27.3] 表示和xxxx有关联，其中EC编号为本表之中的object字段 ',
'''   `object` varchar(45) DEFAULT NULL COMMENT 'type所指向的类型，可以会为空',
'''   PRIMARY KEY (`id`),
'''   UNIQUE KEY `id_UNIQUE` (`id`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='GO_term的同义词表';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("term_synonym", Database:="kb_go", SchemaSQL:="
CREATE TABLE `term_synonym` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT COMMENT '自增编号',
  `term_id` int(10) unsigned NOT NULL COMMENT '当前的Go term的编号',
  `synonym` mediumtext NOT NULL COMMENT '同义名称',
  `type` varchar(45) DEFAULT NULL COMMENT 'EXACT []  表示完全一样\nRELATED [EC:3.1.27.3] 表示和xxxx有关联，其中EC编号为本表之中的object字段 ',
  `object` varchar(45) DEFAULT NULL COMMENT 'type所指向的类型，可以会为空',
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='GO_term的同义词表';")>
Public Class term_synonym: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
''' <summary>
''' 自增编号
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="id"), XmlAttribute> Public Property id As Long
''' <summary>
''' 当前的Go term的编号
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("term_id"), NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="term_id")> Public Property term_id As Long
''' <summary>
''' 同义名称
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("synonym"), NotNull, DataType(MySqlDbType.Text), Column(Name:="synonym")> Public Property synonym As String
''' <summary>
''' EXACT []  表示完全一样\nRELATED [EC:3.1.27.3] 表示和xxxx有关联，其中EC编号为本表之中的object字段 
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("type"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="type")> Public Property type As String
''' <summary>
''' type所指向的类型，可以会为空
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("object"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="object")> Public Property [object] As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `term_synonym` (`id`, `term_id`, `synonym`, `type`, `object`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `term_synonym` (`id`, `term_id`, `synonym`, `type`, `object`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `term_synonym` WHERE `id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `term_synonym` SET `id`='{0}', `term_id`='{1}', `synonym`='{2}', `type`='{3}', `object`='{4}' WHERE `id` = '{5}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `term_synonym` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `term_synonym` (`id`, `term_id`, `synonym`, `type`, `object`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, id, term_id, synonym, type, [object])
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{id}', '{term_id}', '{synonym}', '{type}', '{[object]}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `term_synonym` (`id`, `term_id`, `synonym`, `type`, `object`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, id, term_id, synonym, type, [object])
    End Function
''' <summary>
''' ```SQL
''' UPDATE `term_synonym` SET `id`='{0}', `term_id`='{1}', `synonym`='{2}', `type`='{3}', `object`='{4}' WHERE `id` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, term_id, synonym, type, [object], id)
    End Function
#End Region
Public Function Clone() As term_synonym
                  Return DirectCast(MyClass.MemberwiseClone, term_synonym)
              End Function
End Class


End Namespace
