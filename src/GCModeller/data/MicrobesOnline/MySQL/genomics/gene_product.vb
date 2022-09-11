#Region "Microsoft.VisualBasic::cba1ea18806705699f3a901951e58cd1, GCModeller\data\MicrobesOnline\MySQL\genomics\gene_product.vb"

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


    ' Code Statistics:

    '   Total Lines: 77
    '    Code Lines: 38
    ' Comment Lines: 32
    '   Blank Lines: 7
    '     File Size: 3.62 KB


    ' Class gene_product
    ' 
    '     Properties: dbxref_id, full_name, id, species_id, symbol
    '                 type_id
    ' 
    '     Function: GetDeleteSQL, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:30:29 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MySQL.genomics

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `gene_product`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `gene_product` (
'''   `id` int(11) NOT NULL AUTO_INCREMENT,
'''   `symbol` varchar(128) NOT NULL DEFAULT '',
'''   `dbxref_id` int(11) NOT NULL DEFAULT '0',
'''   `species_id` int(11) DEFAULT NULL,
'''   `type_id` int(11) DEFAULT NULL,
'''   `full_name` text,
'''   PRIMARY KEY (`id`),
'''   UNIQUE KEY `dbxref_id` (`dbxref_id`),
'''   UNIQUE KEY `g0` (`id`),
'''   KEY `type_id` (`type_id`),
'''   KEY `g1` (`symbol`),
'''   KEY `g2` (`dbxref_id`),
'''   KEY `g3` (`species_id`),
'''   KEY `g4` (`id`,`species_id`),
'''   KEY `g5` (`dbxref_id`,`species_id`),
'''   KEY `g6` (`id`,`dbxref_id`),
'''   KEY `g7` (`id`,`species_id`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("gene_product")>
Public Class gene_product: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property id As Long
    <DatabaseField("symbol"), NotNull, DataType(MySqlDbType.VarChar, "128")> Public Property symbol As String
    <DatabaseField("dbxref_id"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property dbxref_id As Long
    <DatabaseField("species_id"), DataType(MySqlDbType.Int64, "11")> Public Property species_id As Long
    <DatabaseField("type_id"), DataType(MySqlDbType.Int64, "11")> Public Property type_id As Long
    <DatabaseField("full_name"), DataType(MySqlDbType.Text)> Public Property full_name As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `gene_product` (`symbol`, `dbxref_id`, `species_id`, `type_id`, `full_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `gene_product` (`symbol`, `dbxref_id`, `species_id`, `type_id`, `full_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `gene_product` WHERE `id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `gene_product` SET `id`='{0}', `symbol`='{1}', `dbxref_id`='{2}', `species_id`='{3}', `type_id`='{4}', `full_name`='{5}' WHERE `id` = '{6}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, symbol, dbxref_id, species_id, type_id, full_name)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, symbol, dbxref_id, species_id, type_id, full_name)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, symbol, dbxref_id, species_id, type_id, full_name, id)
    End Function
#End Region
End Class


End Namespace
