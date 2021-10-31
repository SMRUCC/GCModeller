#Region "Microsoft.VisualBasic::4e1c12d578888c2d3c84906e32fd7afe, DataMySql\kb_UniProtKB\MySQL\protein_go.vb"

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

    ' Class protein_go
    ' 
    '     Properties: [namespace], go_id, GO_term, hash_code, namespace_id
    '                 term_name, uniprot_id
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

REM  Dump @2018/5/23 13:13:51


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace kb_UniProtKB.mysql

''' <summary>
''' ```SQL
''' 对蛋白质的GO功能注释的信息关联表
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `protein_go`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `protein_go` (
'''   `hash_code` int(10) unsigned NOT NULL,
'''   `uniprot_id` varchar(45) NOT NULL,
'''   `go_id` int(10) unsigned NOT NULL,
'''   `GO_term` varchar(45) NOT NULL COMMENT 'GO编号',
'''   `term_name` tinytext,
'''   `namespace_id` int(10) unsigned NOT NULL,
'''   `namespace` char(32) DEFAULT NULL,
'''   PRIMARY KEY (`hash_code`,`go_id`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='对蛋白质的GO功能注释的信息关联表';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("protein_go", Database:="kb_uniprotkb", SchemaSQL:="
CREATE TABLE `protein_go` (
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) NOT NULL,
  `go_id` int(10) unsigned NOT NULL,
  `GO_term` varchar(45) NOT NULL COMMENT 'GO编号',
  `term_name` tinytext,
  `namespace_id` int(10) unsigned NOT NULL,
  `namespace` char(32) DEFAULT NULL,
  PRIMARY KEY (`hash_code`,`go_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='对蛋白质的GO功能注释的信息关联表';")>
Public Class protein_go: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("hash_code"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="hash_code"), XmlAttribute> Public Property hash_code As Long
    <DatabaseField("uniprot_id"), NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="uniprot_id")> Public Property uniprot_id As String
    <DatabaseField("go_id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="go_id"), XmlAttribute> Public Property go_id As Long
''' <summary>
''' GO编号
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("GO_term"), NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="GO_term")> Public Property GO_term As String
    <DatabaseField("term_name"), DataType(MySqlDbType.Text), Column(Name:="term_name")> Public Property term_name As String
    <DatabaseField("namespace_id"), NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="namespace_id")> Public Property namespace_id As Long
    <DatabaseField("namespace"), DataType(MySqlDbType.VarChar, "32"), Column(Name:="namespace")> Public Property [namespace] As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `protein_go` (`hash_code`, `uniprot_id`, `go_id`, `GO_term`, `term_name`, `namespace_id`, `namespace`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `protein_go` (`hash_code`, `uniprot_id`, `go_id`, `GO_term`, `term_name`, `namespace_id`, `namespace`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `protein_go` (`hash_code`, `uniprot_id`, `go_id`, `GO_term`, `term_name`, `namespace_id`, `namespace`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `protein_go` (`hash_code`, `uniprot_id`, `go_id`, `GO_term`, `term_name`, `namespace_id`, `namespace`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `protein_go` WHERE `hash_code`='{0}' and `go_id`='{1}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `protein_go` SET `hash_code`='{0}', `uniprot_id`='{1}', `go_id`='{2}', `GO_term`='{3}', `term_name`='{4}', `namespace_id`='{5}', `namespace`='{6}' WHERE `hash_code`='{7}' and `go_id`='{8}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `protein_go` WHERE `hash_code`='{0}' and `go_id`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, hash_code, go_id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `protein_go` (`hash_code`, `uniprot_id`, `go_id`, `GO_term`, `term_name`, `namespace_id`, `namespace`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, hash_code, uniprot_id, go_id, GO_term, term_name, namespace_id, [namespace])
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `protein_go` (`hash_code`, `uniprot_id`, `go_id`, `GO_term`, `term_name`, `namespace_id`, `namespace`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, hash_code, uniprot_id, go_id, GO_term, term_name, namespace_id, [namespace])
        Else
        Return String.Format(INSERT_SQL, hash_code, uniprot_id, go_id, GO_term, term_name, namespace_id, [namespace])
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{hash_code}', '{uniprot_id}', '{go_id}', '{GO_term}', '{term_name}', '{namespace_id}', '{[namespace]}')"
        Else
            Return $"('{hash_code}', '{uniprot_id}', '{go_id}', '{GO_term}', '{term_name}', '{namespace_id}', '{[namespace]}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `protein_go` (`hash_code`, `uniprot_id`, `go_id`, `GO_term`, `term_name`, `namespace_id`, `namespace`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, hash_code, uniprot_id, go_id, GO_term, term_name, namespace_id, [namespace])
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `protein_go` (`hash_code`, `uniprot_id`, `go_id`, `GO_term`, `term_name`, `namespace_id`, `namespace`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, hash_code, uniprot_id, go_id, GO_term, term_name, namespace_id, [namespace])
        Else
        Return String.Format(REPLACE_SQL, hash_code, uniprot_id, go_id, GO_term, term_name, namespace_id, [namespace])
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `protein_go` SET `hash_code`='{0}', `uniprot_id`='{1}', `go_id`='{2}', `GO_term`='{3}', `term_name`='{4}', `namespace_id`='{5}', `namespace`='{6}' WHERE `hash_code`='{7}' and `go_id`='{8}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, hash_code, uniprot_id, go_id, GO_term, term_name, namespace_id, [namespace], hash_code, go_id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As protein_go
                         Return DirectCast(MyClass.MemberwiseClone, protein_go)
                     End Function
End Class


End Namespace
