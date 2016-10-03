#Region "Microsoft.VisualBasic::0a7a4823adcfa4f52cb2197e1b3e475c, ..\GCModeller\data\RegulonDatabase\RegulonDB\MySQL\regulatory_interaction.vb"

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
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:08:18 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace RegulonDB.Tables

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `regulatory_interaction`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `regulatory_interaction` (
'''   `regulatory_interaction_id` char(12) NOT NULL,
'''   `conformation_id` char(12) NOT NULL,
'''   `promoter_id` char(12) DEFAULT NULL,
'''   `site_id` char(12) NOT NULL,
'''   `ri_function` varchar(9) DEFAULT NULL,
'''   `center_position` decimal(20,2) DEFAULT NULL,
'''   `ri_dist_first_gene` decimal(20,2) DEFAULT NULL,
'''   `ri_first_gene_id` char(12) DEFAULT NULL,
'''   `affinity_exp` decimal(20,5) DEFAULT NULL,
'''   `regulatory_interaction_note` varchar(2000) DEFAULT NULL,
'''   `ri_internal_comment` longtext,
'''   `key_id_org` varchar(5) NOT NULL,
'''   `ri_sequence` varchar(100) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("regulatory_interaction", Database:="regulondb_7_5")>
Public Class regulatory_interaction: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("regulatory_interaction_id"), NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property regulatory_interaction_id As String
    <DatabaseField("conformation_id"), NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property conformation_id As String
    <DatabaseField("promoter_id"), DataType(MySqlDbType.VarChar, "12")> Public Property promoter_id As String
    <DatabaseField("site_id"), NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property site_id As String
    <DatabaseField("ri_function"), DataType(MySqlDbType.VarChar, "9")> Public Property ri_function As String
    <DatabaseField("center_position"), DataType(MySqlDbType.Decimal)> Public Property center_position As Decimal
    <DatabaseField("ri_dist_first_gene"), DataType(MySqlDbType.Decimal)> Public Property ri_dist_first_gene As Decimal
    <DatabaseField("ri_first_gene_id"), DataType(MySqlDbType.VarChar, "12")> Public Property ri_first_gene_id As String
    <DatabaseField("affinity_exp"), DataType(MySqlDbType.Decimal)> Public Property affinity_exp As Decimal
    <DatabaseField("regulatory_interaction_note"), DataType(MySqlDbType.VarChar, "2000")> Public Property regulatory_interaction_note As String
    <DatabaseField("ri_internal_comment"), DataType(MySqlDbType.Text)> Public Property ri_internal_comment As String
    <DatabaseField("key_id_org"), NotNull, DataType(MySqlDbType.VarChar, "5")> Public Property key_id_org As String
    <DatabaseField("ri_sequence"), DataType(MySqlDbType.VarChar, "100")> Public Property ri_sequence As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `regulatory_interaction` (`regulatory_interaction_id`, `conformation_id`, `promoter_id`, `site_id`, `ri_function`, `center_position`, `ri_dist_first_gene`, `ri_first_gene_id`, `affinity_exp`, `regulatory_interaction_note`, `ri_internal_comment`, `key_id_org`, `ri_sequence`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `regulatory_interaction` (`regulatory_interaction_id`, `conformation_id`, `promoter_id`, `site_id`, `ri_function`, `center_position`, `ri_dist_first_gene`, `ri_first_gene_id`, `affinity_exp`, `regulatory_interaction_note`, `ri_internal_comment`, `key_id_org`, `ri_sequence`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `regulatory_interaction` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `regulatory_interaction` SET `regulatory_interaction_id`='{0}', `conformation_id`='{1}', `promoter_id`='{2}', `site_id`='{3}', `ri_function`='{4}', `center_position`='{5}', `ri_dist_first_gene`='{6}', `ri_first_gene_id`='{7}', `affinity_exp`='{8}', `regulatory_interaction_note`='{9}', `ri_internal_comment`='{10}', `key_id_org`='{11}', `ri_sequence`='{12}' WHERE ;</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, regulatory_interaction_id, conformation_id, promoter_id, site_id, ri_function, center_position, ri_dist_first_gene, ri_first_gene_id, affinity_exp, regulatory_interaction_note, ri_internal_comment, key_id_org, ri_sequence)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, regulatory_interaction_id, conformation_id, promoter_id, site_id, ri_function, center_position, ri_dist_first_gene, ri_first_gene_id, affinity_exp, regulatory_interaction_note, ri_internal_comment, key_id_org, ri_sequence)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace
