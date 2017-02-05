#Region "Microsoft.VisualBasic::783d4c83912504b826d46bb824f74737, ..\GCModeller\data\GO_gene-ontology\GeneOntology\MySQL\go\gene_product_homolset.vb"

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
''' DROP TABLE IF EXISTS `gene_product_homolset`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `gene_product_homolset` (
'''   `id` int(11) NOT NULL AUTO_INCREMENT,
'''   `gene_product_id` int(11) NOT NULL,
'''   `homolset_id` int(11) NOT NULL,
'''   PRIMARY KEY (`id`),
'''   KEY `gene_product_id` (`gene_product_id`),
'''   KEY `homolset_id` (`homolset_id`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("gene_product_homolset", Database:="go", SchemaSQL:="
CREATE TABLE `gene_product_homolset` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `gene_product_id` int(11) NOT NULL,
  `homolset_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `gene_product_id` (`gene_product_id`),
  KEY `homolset_id` (`homolset_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class gene_product_homolset: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property id As Long
    <DatabaseField("gene_product_id"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property gene_product_id As Long
    <DatabaseField("homolset_id"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property homolset_id As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `gene_product_homolset` (`gene_product_id`, `homolset_id`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `gene_product_homolset` (`gene_product_id`, `homolset_id`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `gene_product_homolset` WHERE `id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `gene_product_homolset` SET `id`='{0}', `gene_product_id`='{1}', `homolset_id`='{2}' WHERE `id` = '{3}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `gene_product_homolset` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `gene_product_homolset` (`gene_product_id`, `homolset_id`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, gene_product_id, homolset_id)
    End Function
''' <summary>
''' ```SQL
''' REPLACE INTO `gene_product_homolset` (`gene_product_id`, `homolset_id`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, gene_product_id, homolset_id)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `gene_product_homolset` SET `id`='{0}', `gene_product_id`='{1}', `homolset_id`='{2}' WHERE `id` = '{3}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, gene_product_id, homolset_id, id)
    End Function
#End Region
End Class


End Namespace
