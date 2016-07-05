#Region "Microsoft.VisualBasic::30d86f761432e0418b664350d80fc882, ..\GCModeller\CLI_tools\KEGG\LocalMySQL\gene.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 7:34:47 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace LocalMySQL

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `gene`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `gene` (
'''   `locus_tag` char(45) NOT NULL,
'''   `gene_name` mediumtext,
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
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("gene", Database:="jp_kegg2")>
Public Class gene: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("locus_tag"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "45")> Public Property locus_tag As String
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
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `gene` (`locus_tag`, `gene_name`, `definition`, `aa_seq`, `nt_seq`, `ec`, `modules`, `diseases`, `organism`, `pathways`, `uniprot`, `ncbi_entry`, `kegg_sp`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `gene` (`locus_tag`, `gene_name`, `definition`, `aa_seq`, `nt_seq`, `ec`, `modules`, `diseases`, `organism`, `pathways`, `uniprot`, `ncbi_entry`, `kegg_sp`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `gene` WHERE `locus_tag` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `gene` SET `locus_tag`='{0}', `gene_name`='{1}', `definition`='{2}', `aa_seq`='{3}', `nt_seq`='{4}', `ec`='{5}', `modules`='{6}', `diseases`='{7}', `organism`='{8}', `pathways`='{9}', `uniprot`='{10}', `ncbi_entry`='{11}', `kegg_sp`='{12}' WHERE `locus_tag` = '{13}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, locus_tag)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, locus_tag, gene_name, definition, aa_seq, nt_seq, ec, modules, diseases, organism, pathways, uniprot, ncbi_entry, kegg_sp)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, locus_tag, gene_name, definition, aa_seq, nt_seq, ec, modules, diseases, organism, pathways, uniprot, ncbi_entry, kegg_sp)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, locus_tag, gene_name, definition, aa_seq, nt_seq, ec, modules, diseases, organism, pathways, uniprot, ncbi_entry, kegg_sp, locus_tag)
    End Function
#End Region
End Class


End Namespace

