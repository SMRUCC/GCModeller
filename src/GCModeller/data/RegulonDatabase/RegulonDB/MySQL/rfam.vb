#Region "Microsoft.VisualBasic::e2059a59e337d7001c4b86e2ab1b3703, ..\GCModeller\data\RegulonDatabase\RegulonDB\MySQL\rfam.vb"

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
''' DROP TABLE IF EXISTS `rfam`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `rfam` (
'''   `rfam_id` varchar(12) NOT NULL,
'''   `gene_id` char(12) DEFAULT NULL,
'''   `rfam_type` varchar(100) DEFAULT NULL,
'''   `rfam_description` varchar(2000) DEFAULT NULL,
'''   `rfam_score` decimal(10,5) DEFAULT NULL,
'''   `rfam_strand` varchar(12) DEFAULT NULL,
'''   `rfam_posleft` decimal(10,0) DEFAULT NULL,
'''   `rfam_posright` decimal(10,0) DEFAULT NULL,
'''   `rfam_sequence` varchar(1000) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("rfam", Database:="regulondb_7_5")>
Public Class rfam: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("rfam_id"), NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property rfam_id As String
    <DatabaseField("gene_id"), DataType(MySqlDbType.VarChar, "12")> Public Property gene_id As String
    <DatabaseField("rfam_type"), DataType(MySqlDbType.VarChar, "100")> Public Property rfam_type As String
    <DatabaseField("rfam_description"), DataType(MySqlDbType.VarChar, "2000")> Public Property rfam_description As String
    <DatabaseField("rfam_score"), DataType(MySqlDbType.Decimal)> Public Property rfam_score As Decimal
    <DatabaseField("rfam_strand"), DataType(MySqlDbType.VarChar, "12")> Public Property rfam_strand As String
    <DatabaseField("rfam_posleft"), DataType(MySqlDbType.Decimal)> Public Property rfam_posleft As Decimal
    <DatabaseField("rfam_posright"), DataType(MySqlDbType.Decimal)> Public Property rfam_posright As Decimal
    <DatabaseField("rfam_sequence"), DataType(MySqlDbType.VarChar, "1000")> Public Property rfam_sequence As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `rfam` (`rfam_id`, `gene_id`, `rfam_type`, `rfam_description`, `rfam_score`, `rfam_strand`, `rfam_posleft`, `rfam_posright`, `rfam_sequence`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `rfam` (`rfam_id`, `gene_id`, `rfam_type`, `rfam_description`, `rfam_score`, `rfam_strand`, `rfam_posleft`, `rfam_posright`, `rfam_sequence`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `rfam` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `rfam` SET `rfam_id`='{0}', `gene_id`='{1}', `rfam_type`='{2}', `rfam_description`='{3}', `rfam_score`='{4}', `rfam_strand`='{5}', `rfam_posleft`='{6}', `rfam_posright`='{7}', `rfam_sequence`='{8}' WHERE ;</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, rfam_id, gene_id, rfam_type, rfam_description, rfam_score, rfam_strand, rfam_posleft, rfam_posright, rfam_sequence)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, rfam_id, gene_id, rfam_type, rfam_description, rfam_score, rfam_strand, rfam_posleft, rfam_posright, rfam_sequence)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace
