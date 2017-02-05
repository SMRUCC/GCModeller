#Region "Microsoft.VisualBasic::6b13a931fdf1b18c3d8db6f760666d3c, ..\GCModeller\data\GO_gene-ontology\GeneOntology\MySQL\go\species.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @9/5/2016 7:59:33 AM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MySQL.Tables

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `species`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `species` (
'''   `id` int(11) NOT NULL AUTO_INCREMENT,
'''   `ncbi_taxa_id` int(11) DEFAULT NULL,
'''   `common_name` varchar(255) DEFAULT NULL,
'''   `lineage_string` text,
'''   `genus` varchar(55) DEFAULT NULL,
'''   `species` varchar(255) DEFAULT NULL,
'''   `parent_id` int(11) DEFAULT NULL,
'''   `left_value` int(11) DEFAULT NULL,
'''   `right_value` int(11) DEFAULT NULL,
'''   `taxonomic_rank` varchar(255) DEFAULT NULL,
'''   PRIMARY KEY (`id`),
'''   UNIQUE KEY `sp0` (`id`),
'''   UNIQUE KEY `ncbi_taxa_id` (`ncbi_taxa_id`),
'''   KEY `sp1` (`ncbi_taxa_id`),
'''   KEY `sp2` (`common_name`),
'''   KEY `sp3` (`genus`),
'''   KEY `sp4` (`species`),
'''   KEY `sp5` (`genus`,`species`),
'''   KEY `sp6` (`id`,`ncbi_taxa_id`),
'''   KEY `sp7` (`id`,`ncbi_taxa_id`,`genus`,`species`),
'''   KEY `sp8` (`parent_id`),
'''   KEY `sp9` (`left_value`),
'''   KEY `sp10` (`right_value`),
'''   KEY `sp11` (`left_value`,`right_value`),
'''   KEY `sp12` (`id`,`left_value`),
'''   KEY `sp13` (`genus`,`left_value`,`right_value`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("species", Database:="go", SchemaSQL:="
CREATE TABLE `species` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `ncbi_taxa_id` int(11) DEFAULT NULL,
  `common_name` varchar(255) DEFAULT NULL,
  `lineage_string` text,
  `genus` varchar(55) DEFAULT NULL,
  `species` varchar(255) DEFAULT NULL,
  `parent_id` int(11) DEFAULT NULL,
  `left_value` int(11) DEFAULT NULL,
  `right_value` int(11) DEFAULT NULL,
  `taxonomic_rank` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `sp0` (`id`),
  UNIQUE KEY `ncbi_taxa_id` (`ncbi_taxa_id`),
  KEY `sp1` (`ncbi_taxa_id`),
  KEY `sp2` (`common_name`),
  KEY `sp3` (`genus`),
  KEY `sp4` (`species`),
  KEY `sp5` (`genus`,`species`),
  KEY `sp6` (`id`,`ncbi_taxa_id`),
  KEY `sp7` (`id`,`ncbi_taxa_id`,`genus`,`species`),
  KEY `sp8` (`parent_id`),
  KEY `sp9` (`left_value`),
  KEY `sp10` (`right_value`),
  KEY `sp11` (`left_value`,`right_value`),
  KEY `sp12` (`id`,`left_value`),
  KEY `sp13` (`genus`,`left_value`,`right_value`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class species: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property id As Long
    <DatabaseField("ncbi_taxa_id"), DataType(MySqlDbType.Int64, "11")> Public Property ncbi_taxa_id As Long
    <DatabaseField("common_name"), DataType(MySqlDbType.VarChar, "255")> Public Property common_name As String
    <DatabaseField("lineage_string"), DataType(MySqlDbType.Text)> Public Property lineage_string As String
    <DatabaseField("genus"), DataType(MySqlDbType.VarChar, "55")> Public Property genus As String
    <DatabaseField("species"), DataType(MySqlDbType.VarChar, "255")> Public Property species As String
    <DatabaseField("parent_id"), DataType(MySqlDbType.Int64, "11")> Public Property parent_id As Long
    <DatabaseField("left_value"), DataType(MySqlDbType.Int64, "11")> Public Property left_value As Long
    <DatabaseField("right_value"), DataType(MySqlDbType.Int64, "11")> Public Property right_value As Long
    <DatabaseField("taxonomic_rank"), DataType(MySqlDbType.VarChar, "255")> Public Property taxonomic_rank As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `species` (`ncbi_taxa_id`, `common_name`, `lineage_string`, `genus`, `species`, `parent_id`, `left_value`, `right_value`, `taxonomic_rank`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `species` (`ncbi_taxa_id`, `common_name`, `lineage_string`, `genus`, `species`, `parent_id`, `left_value`, `right_value`, `taxonomic_rank`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `species` WHERE `id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `species` SET `id`='{0}', `ncbi_taxa_id`='{1}', `common_name`='{2}', `lineage_string`='{3}', `genus`='{4}', `species`='{5}', `parent_id`='{6}', `left_value`='{7}', `right_value`='{8}', `taxonomic_rank`='{9}' WHERE `id` = '{10}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `species` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `species` (`ncbi_taxa_id`, `common_name`, `lineage_string`, `genus`, `species`, `parent_id`, `left_value`, `right_value`, `taxonomic_rank`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, ncbi_taxa_id, common_name, lineage_string, genus, species, parent_id, left_value, right_value, taxonomic_rank)
    End Function
''' <summary>
''' ```SQL
''' REPLACE INTO `species` (`ncbi_taxa_id`, `common_name`, `lineage_string`, `genus`, `species`, `parent_id`, `left_value`, `right_value`, `taxonomic_rank`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, ncbi_taxa_id, common_name, lineage_string, genus, species, parent_id, left_value, right_value, taxonomic_rank)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `species` SET `id`='{0}', `ncbi_taxa_id`='{1}', `common_name`='{2}', `lineage_string`='{3}', `genus`='{4}', `species`='{5}', `parent_id`='{6}', `left_value`='{7}', `right_value`='{8}', `taxonomic_rank`='{9}' WHERE `id` = '{10}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, ncbi_taxa_id, common_name, lineage_string, genus, species, parent_id, left_value, right_value, taxonomic_rank, id)
    End Function
#End Region
End Class


End Namespace
