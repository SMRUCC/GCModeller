#Region "Microsoft.VisualBasic::9ec77b23e3103bd3a50471e073c8dfd8, ..\GCModeller\data\GO_gene-ontology\GeneOntology\MySQL\go\gene_product_property.vb"

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
''' DROP TABLE IF EXISTS `gene_product_property`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `gene_product_property` (
'''   `gene_product_id` int(11) NOT NULL,
'''   `property_key` varchar(64) NOT NULL,
'''   `property_val` varchar(255) DEFAULT NULL,
'''   UNIQUE KEY `gppu4` (`gene_product_id`,`property_key`,`property_val`),
'''   KEY `gpp1` (`gene_product_id`),
'''   KEY `gpp2` (`property_key`),
'''   KEY `gpp3` (`property_val`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("gene_product_property", Database:="go", SchemaSQL:="
CREATE TABLE `gene_product_property` (
  `gene_product_id` int(11) NOT NULL,
  `property_key` varchar(64) NOT NULL,
  `property_val` varchar(255) DEFAULT NULL,
  UNIQUE KEY `gppu4` (`gene_product_id`,`property_key`,`property_val`),
  KEY `gpp1` (`gene_product_id`),
  KEY `gpp2` (`property_key`),
  KEY `gpp3` (`property_val`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class gene_product_property: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("gene_product_id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property gene_product_id As Long
    <DatabaseField("property_key"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "64")> Public Property property_key As String
    <DatabaseField("property_val"), PrimaryKey, DataType(MySqlDbType.VarChar, "255")> Public Property property_val As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `gene_product_property` (`gene_product_id`, `property_key`, `property_val`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `gene_product_property` (`gene_product_id`, `property_key`, `property_val`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `gene_product_property` WHERE `gene_product_id`='{0}' and `property_key`='{1}' and `property_val`='{2}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `gene_product_property` SET `gene_product_id`='{0}', `property_key`='{1}', `property_val`='{2}' WHERE `gene_product_id`='{3}' and `property_key`='{4}' and `property_val`='{5}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `gene_product_property` WHERE `gene_product_id`='{0}' and `property_key`='{1}' and `property_val`='{2}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, gene_product_id, property_key, property_val)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `gene_product_property` (`gene_product_id`, `property_key`, `property_val`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, gene_product_id, property_key, property_val)
    End Function
''' <summary>
''' ```SQL
''' REPLACE INTO `gene_product_property` (`gene_product_id`, `property_key`, `property_val`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, gene_product_id, property_key, property_val)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `gene_product_property` SET `gene_product_id`='{0}', `property_key`='{1}', `property_val`='{2}' WHERE `gene_product_id`='{3}' and `property_key`='{4}' and `property_val`='{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, gene_product_id, property_key, property_val, gene_product_id, property_key, property_val)
    End Function
#End Region
End Class


End Namespace
