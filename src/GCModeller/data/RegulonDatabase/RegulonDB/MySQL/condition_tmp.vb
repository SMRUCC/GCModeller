#Region "Microsoft.VisualBasic::b6e1dc5c4b4357a75d7d7b4b374612ce, ..\GCModeller\data\RegulonDatabase\RegulonDB\MySQL\condition_tmp.vb"

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
''' DROP TABLE IF EXISTS `condition_tmp`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `condition_tmp` (
'''   `condition_id` char(12) DEFAULT NULL,
'''   `cond_effect_link_id` char(12) DEFAULT NULL,
'''   `condition_gene_name` varchar(200) DEFAULT NULL,
'''   `condition_gene_id` varchar(12) DEFAULT NULL,
'''   `condition_effect` varchar(10) DEFAULT NULL,
'''   `condition_promoter_name` varchar(200) DEFAULT NULL,
'''   `condition_promoter_id` varchar(12) DEFAULT NULL,
'''   `condition_final_state` varchar(200) DEFAULT NULL,
'''   `condition_conformation_id` varchar(12) DEFAULT NULL,
'''   `condition_site` varchar(200) DEFAULT NULL,
'''   `condition_evidence` varchar(200) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("condition_tmp", Database:="regulondb_7_5")>
Public Class condition_tmp: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("condition_id"), DataType(MySqlDbType.VarChar, "12")> Public Property condition_id As String
    <DatabaseField("cond_effect_link_id"), DataType(MySqlDbType.VarChar, "12")> Public Property cond_effect_link_id As String
    <DatabaseField("condition_gene_name"), DataType(MySqlDbType.VarChar, "200")> Public Property condition_gene_name As String
    <DatabaseField("condition_gene_id"), DataType(MySqlDbType.VarChar, "12")> Public Property condition_gene_id As String
    <DatabaseField("condition_effect"), DataType(MySqlDbType.VarChar, "10")> Public Property condition_effect As String
    <DatabaseField("condition_promoter_name"), DataType(MySqlDbType.VarChar, "200")> Public Property condition_promoter_name As String
    <DatabaseField("condition_promoter_id"), DataType(MySqlDbType.VarChar, "12")> Public Property condition_promoter_id As String
    <DatabaseField("condition_final_state"), DataType(MySqlDbType.VarChar, "200")> Public Property condition_final_state As String
    <DatabaseField("condition_conformation_id"), DataType(MySqlDbType.VarChar, "12")> Public Property condition_conformation_id As String
    <DatabaseField("condition_site"), DataType(MySqlDbType.VarChar, "200")> Public Property condition_site As String
    <DatabaseField("condition_evidence"), DataType(MySqlDbType.VarChar, "200")> Public Property condition_evidence As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `condition_tmp` (`condition_id`, `cond_effect_link_id`, `condition_gene_name`, `condition_gene_id`, `condition_effect`, `condition_promoter_name`, `condition_promoter_id`, `condition_final_state`, `condition_conformation_id`, `condition_site`, `condition_evidence`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `condition_tmp` (`condition_id`, `cond_effect_link_id`, `condition_gene_name`, `condition_gene_id`, `condition_effect`, `condition_promoter_name`, `condition_promoter_id`, `condition_final_state`, `condition_conformation_id`, `condition_site`, `condition_evidence`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `condition_tmp` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `condition_tmp` SET `condition_id`='{0}', `cond_effect_link_id`='{1}', `condition_gene_name`='{2}', `condition_gene_id`='{3}', `condition_effect`='{4}', `condition_promoter_name`='{5}', `condition_promoter_id`='{6}', `condition_final_state`='{7}', `condition_conformation_id`='{8}', `condition_site`='{9}', `condition_evidence`='{10}' WHERE ;</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, condition_id, cond_effect_link_id, condition_gene_name, condition_gene_id, condition_effect, condition_promoter_name, condition_promoter_id, condition_final_state, condition_conformation_id, condition_site, condition_evidence)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, condition_id, cond_effect_link_id, condition_gene_name, condition_gene_id, condition_effect, condition_promoter_name, condition_promoter_id, condition_final_state, condition_conformation_id, condition_site, condition_evidence)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace
