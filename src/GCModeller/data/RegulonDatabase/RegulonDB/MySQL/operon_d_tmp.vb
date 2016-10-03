#Region "Microsoft.VisualBasic::926c9fc22574cedd3f29dd60ac6f5208, ..\GCModeller\data\RegulonDatabase\RegulonDB\MySQL\operon_d_tmp.vb"

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
''' DROP TABLE IF EXISTS `operon_d_tmp`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `operon_d_tmp` (
'''   `op_id` decimal(10,0) NOT NULL,
'''   `operon_id` char(12) DEFAULT NULL,
'''   `operon_name` varchar(255) DEFAULT NULL,
'''   `operon_tu_group` decimal(10,0) DEFAULT NULL,
'''   `operon_gene_group` decimal(10,0) DEFAULT NULL,
'''   `operon_sf_group` decimal(10,0) DEFAULT NULL,
'''   `operon_site_group` decimal(10,0) DEFAULT NULL,
'''   `operon_promoter_group` decimal(10,0) DEFAULT NULL,
'''   `operon_tf_group` decimal(10,0) DEFAULT NULL,
'''   `operon_terminator_group` decimal(10,0) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("operon_d_tmp", Database:="regulondb_7_5")>
Public Class operon_d_tmp: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("op_id"), NotNull, DataType(MySqlDbType.Decimal)> Public Property op_id As Decimal
    <DatabaseField("operon_id"), DataType(MySqlDbType.VarChar, "12")> Public Property operon_id As String
    <DatabaseField("operon_name"), DataType(MySqlDbType.VarChar, "255")> Public Property operon_name As String
    <DatabaseField("operon_tu_group"), DataType(MySqlDbType.Decimal)> Public Property operon_tu_group As Decimal
    <DatabaseField("operon_gene_group"), DataType(MySqlDbType.Decimal)> Public Property operon_gene_group As Decimal
    <DatabaseField("operon_sf_group"), DataType(MySqlDbType.Decimal)> Public Property operon_sf_group As Decimal
    <DatabaseField("operon_site_group"), DataType(MySqlDbType.Decimal)> Public Property operon_site_group As Decimal
    <DatabaseField("operon_promoter_group"), DataType(MySqlDbType.Decimal)> Public Property operon_promoter_group As Decimal
    <DatabaseField("operon_tf_group"), DataType(MySqlDbType.Decimal)> Public Property operon_tf_group As Decimal
    <DatabaseField("operon_terminator_group"), DataType(MySqlDbType.Decimal)> Public Property operon_terminator_group As Decimal
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `operon_d_tmp` (`op_id`, `operon_id`, `operon_name`, `operon_tu_group`, `operon_gene_group`, `operon_sf_group`, `operon_site_group`, `operon_promoter_group`, `operon_tf_group`, `operon_terminator_group`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `operon_d_tmp` (`op_id`, `operon_id`, `operon_name`, `operon_tu_group`, `operon_gene_group`, `operon_sf_group`, `operon_site_group`, `operon_promoter_group`, `operon_tf_group`, `operon_terminator_group`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `operon_d_tmp` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `operon_d_tmp` SET `op_id`='{0}', `operon_id`='{1}', `operon_name`='{2}', `operon_tu_group`='{3}', `operon_gene_group`='{4}', `operon_sf_group`='{5}', `operon_site_group`='{6}', `operon_promoter_group`='{7}', `operon_tf_group`='{8}', `operon_terminator_group`='{9}' WHERE ;</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, op_id, operon_id, operon_name, operon_tu_group, operon_gene_group, operon_sf_group, operon_site_group, operon_promoter_group, operon_tf_group, operon_terminator_group)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, op_id, operon_id, operon_name, operon_tu_group, operon_gene_group, operon_sf_group, operon_site_group, operon_promoter_group, operon_tf_group, operon_terminator_group)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace
