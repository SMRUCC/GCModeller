#Region "Microsoft.VisualBasic::bc80665520d1220457c9a178b41165dd, data\KEGG\LocalMySQL\xref_pathway_genes.vb"

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

    ' Class xref_pathway_genes
    ' 
    '     Properties: gene, gene_KO, gene_name, locus_tag, pathway
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

REM  Dump @2018/5/23 13:13:37


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace LocalMySQL

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `xref_pathway_genes`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `xref_pathway_genes` (
'''   `pathway` int(11) NOT NULL,
'''   `gene` int(11) NOT NULL,
'''   `gene_KO` varchar(45) DEFAULT NULL,
'''   `locus_tag` varchar(45) DEFAULT NULL,
'''   `gene_name` varchar(45) DEFAULT NULL,
'''   PRIMARY KEY (`pathway`,`gene`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("xref_pathway_genes", Database:="jp_kegg2", SchemaSQL:="
CREATE TABLE `xref_pathway_genes` (
  `pathway` int(11) NOT NULL,
  `gene` int(11) NOT NULL,
  `gene_KO` varchar(45) DEFAULT NULL,
  `locus_tag` varchar(45) DEFAULT NULL,
  `gene_name` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`pathway`,`gene`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class xref_pathway_genes: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("pathway"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="pathway"), XmlAttribute> Public Property pathway As Long
    <DatabaseField("gene"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="gene"), XmlAttribute> Public Property gene As Long
    <DatabaseField("gene_KO"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="gene_KO")> Public Property gene_KO As String
    <DatabaseField("locus_tag"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="locus_tag")> Public Property locus_tag As String
    <DatabaseField("gene_name"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="gene_name")> Public Property gene_name As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `xref_pathway_genes` (`pathway`, `gene`, `gene_KO`, `locus_tag`, `gene_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `xref_pathway_genes` (`pathway`, `gene`, `gene_KO`, `locus_tag`, `gene_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `xref_pathway_genes` (`pathway`, `gene`, `gene_KO`, `locus_tag`, `gene_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `xref_pathway_genes` (`pathway`, `gene`, `gene_KO`, `locus_tag`, `gene_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `xref_pathway_genes` WHERE `pathway`='{0}' and `gene`='{1}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `xref_pathway_genes` SET `pathway`='{0}', `gene`='{1}', `gene_KO`='{2}', `locus_tag`='{3}', `gene_name`='{4}' WHERE `pathway`='{5}' and `gene`='{6}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `xref_pathway_genes` WHERE `pathway`='{0}' and `gene`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, pathway, gene)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `xref_pathway_genes` (`pathway`, `gene`, `gene_KO`, `locus_tag`, `gene_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, pathway, gene, gene_KO, locus_tag, gene_name)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `xref_pathway_genes` (`pathway`, `gene`, `gene_KO`, `locus_tag`, `gene_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, pathway, gene, gene_KO, locus_tag, gene_name)
        Else
        Return String.Format(INSERT_SQL, pathway, gene, gene_KO, locus_tag, gene_name)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{pathway}', '{gene}', '{gene_KO}', '{locus_tag}', '{gene_name}')"
        Else
            Return $"('{pathway}', '{gene}', '{gene_KO}', '{locus_tag}', '{gene_name}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `xref_pathway_genes` (`pathway`, `gene`, `gene_KO`, `locus_tag`, `gene_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, pathway, gene, gene_KO, locus_tag, gene_name)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `xref_pathway_genes` (`pathway`, `gene`, `gene_KO`, `locus_tag`, `gene_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, pathway, gene, gene_KO, locus_tag, gene_name)
        Else
        Return String.Format(REPLACE_SQL, pathway, gene, gene_KO, locus_tag, gene_name)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `xref_pathway_genes` SET `pathway`='{0}', `gene`='{1}', `gene_KO`='{2}', `locus_tag`='{3}', `gene_name`='{4}' WHERE `pathway`='{5}' and `gene`='{6}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, pathway, gene, gene_KO, locus_tag, gene_name, pathway, gene)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As xref_pathway_genes
                         Return DirectCast(MyClass.MemberwiseClone, xref_pathway_genes)
                     End Function
End Class


End Namespace
