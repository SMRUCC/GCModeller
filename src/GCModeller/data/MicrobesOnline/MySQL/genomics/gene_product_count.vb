#Region "Microsoft.VisualBasic::86cccc18abad760aa8b69abba0f64125, GCModeller\data\MicrobesOnline\MySQL\genomics\gene_product_count.vb"

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

    '   Total Lines: 70
    '    Code Lines: 37
    ' Comment Lines: 26
    '   Blank Lines: 7
    '     File Size: 3.46 KB


    ' Class gene_product_count
    ' 
    '     Properties: code, product_count, species_id, speciesdbname, term_id
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
''' DROP TABLE IF EXISTS `gene_product_count`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `gene_product_count` (
'''   `term_id` int(11) NOT NULL DEFAULT '0',
'''   `code` varchar(8) DEFAULT NULL,
'''   `speciesdbname` varchar(55) DEFAULT NULL,
'''   `species_id` int(11) DEFAULT NULL,
'''   `product_count` int(11) NOT NULL DEFAULT '0',
'''   KEY `species_id` (`species_id`),
'''   KEY `gpc1` (`term_id`),
'''   KEY `gpc2` (`code`),
'''   KEY `gpc3` (`speciesdbname`),
'''   KEY `gpc4` (`term_id`,`code`,`speciesdbname`),
'''   KEY `gpc5` (`term_id`,`species_id`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("gene_product_count")>
Public Class gene_product_count: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("term_id"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property term_id As Long
    <DatabaseField("code"), DataType(MySqlDbType.VarChar, "8")> Public Property code As String
    <DatabaseField("speciesdbname"), DataType(MySqlDbType.VarChar, "55")> Public Property speciesdbname As String
    <DatabaseField("species_id"), PrimaryKey, DataType(MySqlDbType.Int64, "11")> Public Property species_id As Long
    <DatabaseField("product_count"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property product_count As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `gene_product_count` (`term_id`, `code`, `speciesdbname`, `species_id`, `product_count`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `gene_product_count` (`term_id`, `code`, `speciesdbname`, `species_id`, `product_count`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `gene_product_count` WHERE `species_id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `gene_product_count` SET `term_id`='{0}', `code`='{1}', `speciesdbname`='{2}', `species_id`='{3}', `product_count`='{4}' WHERE `species_id` = '{5}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, species_id)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, term_id, code, speciesdbname, species_id, product_count)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, term_id, code, speciesdbname, species_id, product_count)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, term_id, code, speciesdbname, species_id, product_count, species_id)
    End Function
#End Region
End Class


End Namespace
