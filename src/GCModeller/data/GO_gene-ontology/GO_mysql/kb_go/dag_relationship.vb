#Region "Microsoft.VisualBasic::8588ca6072bbedd0e9329634422a30d8, data\GO_gene-ontology\GO_mysql\kb_go\dag_relationship.vb"

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

    ' Class dag_relationship
    ' 
    '     Properties: id, name, relationship, relationship_id, term_id
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
''' 由GO_term之间的相互关系所构成的有向无环图Directed Acyclic Graph（DAG）
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `dag_relationship`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `dag_relationship` (
'''   `id` int(10) unsigned NOT NULL COMMENT '当前的term编号',
'''   `relationship` varchar(45) DEFAULT NULL,
'''   `relationship_id` int(10) unsigned NOT NULL COMMENT '二者之间的关系编号，由于可能会存在多种互做类型，所以只使用id+term_id的结构来做主键会出现重复entry的问题，在这里将作用的类型也加入进来',
'''   `term_id` int(10) unsigned NOT NULL COMMENT '与当前的term发生互做关系的另外的一个partner term的编号',
'''   `name` varchar(45) DEFAULT NULL COMMENT '发生关系的term的名字',
'''   PRIMARY KEY (`id`,`term_id`,`relationship_id`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='由GO_term之间的相互关系所构成的有向无环图Directed Acyclic Graph（DAG）';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("dag_relationship", Database:="kb_go", SchemaSQL:="
CREATE TABLE `dag_relationship` (
  `id` int(10) unsigned NOT NULL COMMENT '当前的term编号',
  `relationship` varchar(45) DEFAULT NULL,
  `relationship_id` int(10) unsigned NOT NULL COMMENT '二者之间的关系编号，由于可能会存在多种互做类型，所以只使用id+term_id的结构来做主键会出现重复entry的问题，在这里将作用的类型也加入进来',
  `term_id` int(10) unsigned NOT NULL COMMENT '与当前的term发生互做关系的另外的一个partner term的编号',
  `name` varchar(45) DEFAULT NULL COMMENT '发生关系的term的名字',
  PRIMARY KEY (`id`,`term_id`,`relationship_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='由GO_term之间的相互关系所构成的有向无环图Directed Acyclic Graph（DAG）';")>
Public Class dag_relationship: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
''' <summary>
''' 当前的term编号
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="id"), XmlAttribute> Public Property id As Long
    <DatabaseField("relationship"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="relationship")> Public Property relationship As String
''' <summary>
''' 二者之间的关系编号，由于可能会存在多种互做类型，所以只使用id+term_id的结构来做主键会出现重复entry的问题，在这里将作用的类型也加入进来
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("relationship_id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="relationship_id"), XmlAttribute> Public Property relationship_id As Long
''' <summary>
''' 与当前的term发生互做关系的另外的一个partner term的编号
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("term_id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="term_id"), XmlAttribute> Public Property term_id As Long
''' <summary>
''' 发生关系的term的名字
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("name"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="name")> Public Property name As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `dag_relationship` (`id`, `relationship`, `relationship_id`, `term_id`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `dag_relationship` (`id`, `relationship`, `relationship_id`, `term_id`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `dag_relationship` (`id`, `relationship`, `relationship_id`, `term_id`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `dag_relationship` (`id`, `relationship`, `relationship_id`, `term_id`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `dag_relationship` WHERE `id`='{0}' and `term_id`='{1}' and `relationship_id`='{2}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `dag_relationship` SET `id`='{0}', `relationship`='{1}', `relationship_id`='{2}', `term_id`='{3}', `name`='{4}' WHERE `id`='{5}' and `term_id`='{6}' and `relationship_id`='{7}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `dag_relationship` WHERE `id`='{0}' and `term_id`='{1}' and `relationship_id`='{2}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id, term_id, relationship_id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `dag_relationship` (`id`, `relationship`, `relationship_id`, `term_id`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, id, relationship, relationship_id, term_id, name)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `dag_relationship` (`id`, `relationship`, `relationship_id`, `term_id`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, id, relationship, relationship_id, term_id, name)
        Else
        Return String.Format(INSERT_SQL, id, relationship, relationship_id, term_id, name)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{id}', '{relationship}', '{relationship_id}', '{term_id}', '{name}')"
        Else
            Return $"('{id}', '{relationship}', '{relationship_id}', '{term_id}', '{name}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `dag_relationship` (`id`, `relationship`, `relationship_id`, `term_id`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, id, relationship, relationship_id, term_id, name)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `dag_relationship` (`id`, `relationship`, `relationship_id`, `term_id`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, id, relationship, relationship_id, term_id, name)
        Else
        Return String.Format(REPLACE_SQL, id, relationship, relationship_id, term_id, name)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `dag_relationship` SET `id`='{0}', `relationship`='{1}', `relationship_id`='{2}', `term_id`='{3}', `name`='{4}' WHERE `id`='{5}' and `term_id`='{6}' and `relationship_id`='{7}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, relationship, relationship_id, term_id, name, id, term_id, relationship_id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As dag_relationship
                         Return DirectCast(MyClass.MemberwiseClone, dag_relationship)
                     End Function
End Class


End Namespace
