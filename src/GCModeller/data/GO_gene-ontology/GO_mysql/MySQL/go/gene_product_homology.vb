#Region "Microsoft.VisualBasic::435a783083e8ae0b83d79a8ab2bd34d1, ..\GCModeller\data\GO_gene-ontology\GeneOntology\MySQL\go\gene_product_homology.vb"

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
''' DROP TABLE IF EXISTS `gene_product_homology`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `gene_product_homology` (
'''   `gene_product1_id` int(11) NOT NULL,
'''   `gene_product2_id` int(11) NOT NULL,
'''   `relationship_type_id` int(11) NOT NULL,
'''   KEY `gene_product1_id` (`gene_product1_id`),
'''   KEY `gene_product2_id` (`gene_product2_id`),
'''   KEY `relationship_type_id` (`relationship_type_id`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("gene_product_homology", Database:="go", SchemaSQL:="
CREATE TABLE `gene_product_homology` (
  `gene_product1_id` int(11) NOT NULL,
  `gene_product2_id` int(11) NOT NULL,
  `relationship_type_id` int(11) NOT NULL,
  KEY `gene_product1_id` (`gene_product1_id`),
  KEY `gene_product2_id` (`gene_product2_id`),
  KEY `relationship_type_id` (`relationship_type_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class gene_product_homology: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("gene_product1_id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property gene_product1_id As Long
    <DatabaseField("gene_product2_id"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property gene_product2_id As Long
    <DatabaseField("relationship_type_id"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property relationship_type_id As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `gene_product_homology` (`gene_product1_id`, `gene_product2_id`, `relationship_type_id`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `gene_product_homology` (`gene_product1_id`, `gene_product2_id`, `relationship_type_id`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `gene_product_homology` WHERE `gene_product1_id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `gene_product_homology` SET `gene_product1_id`='{0}', `gene_product2_id`='{1}', `relationship_type_id`='{2}' WHERE `gene_product1_id` = '{3}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `gene_product_homology` WHERE `gene_product1_id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, gene_product1_id)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `gene_product_homology` (`gene_product1_id`, `gene_product2_id`, `relationship_type_id`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, gene_product1_id, gene_product2_id, relationship_type_id)
    End Function
''' <summary>
''' ```SQL
''' REPLACE INTO `gene_product_homology` (`gene_product1_id`, `gene_product2_id`, `relationship_type_id`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, gene_product1_id, gene_product2_id, relationship_type_id)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `gene_product_homology` SET `gene_product1_id`='{0}', `gene_product2_id`='{1}', `relationship_type_id`='{2}' WHERE `gene_product1_id` = '{3}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, gene_product1_id, gene_product2_id, relationship_type_id, gene_product1_id)
    End Function
#End Region
End Class


End Namespace
