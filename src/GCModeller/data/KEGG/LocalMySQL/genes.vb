#Region "Microsoft.VisualBasic::e16e613b596dedfe243326d453c8ec9a, data\KEGG\LocalMySQL\genes.vb"

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

    ' Class genes
    ' 
    '     Properties: aa_seq, definition, diseases, ec, gene_name
    '                 kegg_sp, locus_tag, modules, ncbi_entry, nt_seq
    '                 organism, pathways, uniprot
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @3/29/2017 10:06:32 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace LocalMySQL

''' <summary>
''' ```SQL
''' 基因的概览表，主要的数据包括蛋白功能以及序列数据和一些dbxref的简单的数量统计
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `genes`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `genes` (
'''   `locus_tag` char(45) NOT NULL COMMENT '基因号',
'''   `gene_name` mediumtext COMMENT '基因名',
'''   `definition` mediumtext,
'''   `aa_seq` longtext,
'''   `nt_seq` longtext,
'''   `ec` tinytext,
'''   `modules` mediumtext,
'''   `diseases` mediumtext,
'''   `organism` varchar(45) DEFAULT NULL,
'''   `pathways` varchar(45) DEFAULT NULL,
'''   `uniprot` varchar(45) DEFAULT NULL COMMENT 'uniprot entry for this protein',
'''   `ncbi_entry` varchar(45) DEFAULT NULL,
'''   `kegg_sp` varchar(45) DEFAULT NULL COMMENT 'kegg species organism brief code',
'''   PRIMARY KEY (`locus_tag`),
'''   UNIQUE KEY `entry_UNIQUE` (`locus_tag`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='基因的概览表，主要的数据包括蛋白功能以及序列数据和一些dbxref的简单的数量统计';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("genes", Database:="jp_kegg2", SchemaSQL:="
CREATE TABLE `genes` (
  `locus_tag` char(45) NOT NULL COMMENT '基因号',
  `gene_name` mediumtext COMMENT '基因名',
  `definition` mediumtext,
  `aa_seq` longtext,
  `nt_seq` longtext,
  `ec` tinytext,
  `modules` mediumtext,
  `diseases` mediumtext,
  `organism` varchar(45) DEFAULT NULL,
  `pathways` varchar(45) DEFAULT NULL,
  `uniprot` varchar(45) DEFAULT NULL COMMENT 'uniprot entry for this protein',
  `ncbi_entry` varchar(45) DEFAULT NULL,
  `kegg_sp` varchar(45) DEFAULT NULL COMMENT 'kegg species organism brief code',
  PRIMARY KEY (`locus_tag`),
  UNIQUE KEY `entry_UNIQUE` (`locus_tag`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='基因的概览表，主要的数据包括蛋白功能以及序列数据和一些dbxref的简单的数量统计';")>
Public Class genes: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
''' <summary>
''' 基因号
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("locus_tag"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "45")> Public Property locus_tag As String
''' <summary>
''' 基因名
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("gene_name"), DataType(MySqlDbType.Text)> Public Property gene_name As String
    <DatabaseField("definition"), DataType(MySqlDbType.Text)> Public Property definition As String
    <DatabaseField("aa_seq"), DataType(MySqlDbType.Text)> Public Property aa_seq As String
    <DatabaseField("nt_seq"), DataType(MySqlDbType.Text)> Public Property nt_seq As String
    <DatabaseField("ec"), DataType(MySqlDbType.Text)> Public Property ec As String
    <DatabaseField("modules"), DataType(MySqlDbType.Text)> Public Property modules As String
    <DatabaseField("diseases"), DataType(MySqlDbType.Text)> Public Property diseases As String
    <DatabaseField("organism"), DataType(MySqlDbType.VarChar, "45")> Public Property organism As String
    <DatabaseField("pathways"), DataType(MySqlDbType.VarChar, "45")> Public Property pathways As String
''' <summary>
''' uniprot entry for this protein
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("uniprot"), DataType(MySqlDbType.VarChar, "45")> Public Property uniprot As String
    <DatabaseField("ncbi_entry"), DataType(MySqlDbType.VarChar, "45")> Public Property ncbi_entry As String
''' <summary>
''' kegg species organism brief code
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("kegg_sp"), DataType(MySqlDbType.VarChar, "45")> Public Property kegg_sp As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `genes` (`locus_tag`, `gene_name`, `definition`, `aa_seq`, `nt_seq`, `ec`, `modules`, `diseases`, `organism`, `pathways`, `uniprot`, `ncbi_entry`, `kegg_sp`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `genes` (`locus_tag`, `gene_name`, `definition`, `aa_seq`, `nt_seq`, `ec`, `modules`, `diseases`, `organism`, `pathways`, `uniprot`, `ncbi_entry`, `kegg_sp`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `genes` WHERE `locus_tag` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `genes` SET `locus_tag`='{0}', `gene_name`='{1}', `definition`='{2}', `aa_seq`='{3}', `nt_seq`='{4}', `ec`='{5}', `modules`='{6}', `diseases`='{7}', `organism`='{8}', `pathways`='{9}', `uniprot`='{10}', `ncbi_entry`='{11}', `kegg_sp`='{12}' WHERE `locus_tag` = '{13}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `genes` WHERE `locus_tag` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, locus_tag)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `genes` (`locus_tag`, `gene_name`, `definition`, `aa_seq`, `nt_seq`, `ec`, `modules`, `diseases`, `organism`, `pathways`, `uniprot`, `ncbi_entry`, `kegg_sp`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, locus_tag, gene_name, definition, aa_seq, nt_seq, ec, modules, diseases, organism, pathways, uniprot, ncbi_entry, kegg_sp)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{locus_tag}', '{gene_name}', '{definition}', '{aa_seq}', '{nt_seq}', '{ec}', '{modules}', '{diseases}', '{organism}', '{pathways}', '{uniprot}', '{ncbi_entry}', '{kegg_sp}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `genes` (`locus_tag`, `gene_name`, `definition`, `aa_seq`, `nt_seq`, `ec`, `modules`, `diseases`, `organism`, `pathways`, `uniprot`, `ncbi_entry`, `kegg_sp`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, locus_tag, gene_name, definition, aa_seq, nt_seq, ec, modules, diseases, organism, pathways, uniprot, ncbi_entry, kegg_sp)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `genes` SET `locus_tag`='{0}', `gene_name`='{1}', `definition`='{2}', `aa_seq`='{3}', `nt_seq`='{4}', `ec`='{5}', `modules`='{6}', `diseases`='{7}', `organism`='{8}', `pathways`='{9}', `uniprot`='{10}', `ncbi_entry`='{11}', `kegg_sp`='{12}' WHERE `locus_tag` = '{13}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, locus_tag, gene_name, definition, aa_seq, nt_seq, ec, modules, diseases, organism, pathways, uniprot, ncbi_entry, kegg_sp, locus_tag)
    End Function
#End Region
End Class


End Namespace
