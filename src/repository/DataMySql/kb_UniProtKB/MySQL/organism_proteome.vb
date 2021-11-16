#Region "Microsoft.VisualBasic::92d434e3d577731a30f59a292b89e078, DataMySql\kb_UniProtKB\MySQL\organism_proteome.vb"

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

    ' Class organism_proteome
    ' 
    '     Properties: component, gene_name, id_hashcode, org_id, proteomes_id
    '                 uniprot_id
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
''' 这个表之中列举出了某一个物种其基因组之中所拥有的蛋白质的集合
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `organism_proteome`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `organism_proteome` (
'''   `org_id` int(10) unsigned NOT NULL,
'''   `uniprot_id` varchar(45) DEFAULT NULL,
'''   `id_hashcode` int(10) unsigned NOT NULL,
'''   `gene_name` varchar(45) DEFAULT NULL,
'''   `proteomes_id` varchar(45) DEFAULT NULL COMMENT 'Proteomes蛋白组数据库之中的编号',
'''   `component` varchar(45) DEFAULT NULL COMMENT '染色体编号',
'''   PRIMARY KEY (`org_id`,`id_hashcode`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='这个表之中列举出了某一个物种其基因组之中所拥有的蛋白质的集合';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("organism_proteome", Database:="kb_uniprotkb", SchemaSQL:="
CREATE TABLE `organism_proteome` (
  `org_id` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) DEFAULT NULL,
  `id_hashcode` int(10) unsigned NOT NULL,
  `gene_name` varchar(45) DEFAULT NULL,
  `proteomes_id` varchar(45) DEFAULT NULL COMMENT 'Proteomes蛋白组数据库之中的编号',
  `component` varchar(45) DEFAULT NULL COMMENT '染色体编号',
  PRIMARY KEY (`org_id`,`id_hashcode`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='这个表之中列举出了某一个物种其基因组之中所拥有的蛋白质的集合';")>
Public Class organism_proteome: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("org_id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="org_id"), XmlAttribute> Public Property org_id As Long
    <DatabaseField("uniprot_id"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="uniprot_id")> Public Property uniprot_id As String
    <DatabaseField("id_hashcode"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="id_hashcode"), XmlAttribute> Public Property id_hashcode As Long
    <DatabaseField("gene_name"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="gene_name")> Public Property gene_name As String
''' <summary>
''' Proteomes蛋白组数据库之中的编号
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("proteomes_id"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="proteomes_id")> Public Property proteomes_id As String
''' <summary>
''' 染色体编号
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("component"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="component")> Public Property component As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `organism_proteome` (`org_id`, `uniprot_id`, `id_hashcode`, `gene_name`, `proteomes_id`, `component`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `organism_proteome` (`org_id`, `uniprot_id`, `id_hashcode`, `gene_name`, `proteomes_id`, `component`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `organism_proteome` (`org_id`, `uniprot_id`, `id_hashcode`, `gene_name`, `proteomes_id`, `component`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `organism_proteome` (`org_id`, `uniprot_id`, `id_hashcode`, `gene_name`, `proteomes_id`, `component`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `organism_proteome` WHERE `org_id`='{0}' and `id_hashcode`='{1}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `organism_proteome` SET `org_id`='{0}', `uniprot_id`='{1}', `id_hashcode`='{2}', `gene_name`='{3}', `proteomes_id`='{4}', `component`='{5}' WHERE `org_id`='{6}' and `id_hashcode`='{7}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `organism_proteome` WHERE `org_id`='{0}' and `id_hashcode`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, org_id, id_hashcode)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `organism_proteome` (`org_id`, `uniprot_id`, `id_hashcode`, `gene_name`, `proteomes_id`, `component`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, org_id, uniprot_id, id_hashcode, gene_name, proteomes_id, component)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `organism_proteome` (`org_id`, `uniprot_id`, `id_hashcode`, `gene_name`, `proteomes_id`, `component`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, org_id, uniprot_id, id_hashcode, gene_name, proteomes_id, component)
        Else
        Return String.Format(INSERT_SQL, org_id, uniprot_id, id_hashcode, gene_name, proteomes_id, component)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{org_id}', '{uniprot_id}', '{id_hashcode}', '{gene_name}', '{proteomes_id}', '{component}')"
        Else
            Return $"('{org_id}', '{uniprot_id}', '{id_hashcode}', '{gene_name}', '{proteomes_id}', '{component}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `organism_proteome` (`org_id`, `uniprot_id`, `id_hashcode`, `gene_name`, `proteomes_id`, `component`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, org_id, uniprot_id, id_hashcode, gene_name, proteomes_id, component)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `organism_proteome` (`org_id`, `uniprot_id`, `id_hashcode`, `gene_name`, `proteomes_id`, `component`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, org_id, uniprot_id, id_hashcode, gene_name, proteomes_id, component)
        Else
        Return String.Format(REPLACE_SQL, org_id, uniprot_id, id_hashcode, gene_name, proteomes_id, component)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `organism_proteome` SET `org_id`='{0}', `uniprot_id`='{1}', `id_hashcode`='{2}', `gene_name`='{3}', `proteomes_id`='{4}', `component`='{5}' WHERE `org_id`='{6}' and `id_hashcode`='{7}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, org_id, uniprot_id, id_hashcode, gene_name, proteomes_id, component, org_id, id_hashcode)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As organism_proteome
                         Return DirectCast(MyClass.MemberwiseClone, organism_proteome)
                     End Function
End Class


End Namespace
