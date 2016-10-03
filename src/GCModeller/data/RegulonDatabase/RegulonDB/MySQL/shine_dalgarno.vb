#Region "Microsoft.VisualBasic::19c708352af57dd5862e3d44603adb2b, ..\GCModeller\data\RegulonDatabase\RegulonDB\MySQL\shine_dalgarno.vb"

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
''' DROP TABLE IF EXISTS `shine_dalgarno`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `shine_dalgarno` (
'''   `shine_dalgarno_id` char(12) NOT NULL,
'''   `gene_id` char(12) NOT NULL,
'''   `shine_dalgarno_dist_gene` decimal(10,0) NOT NULL,
'''   `shine_dalgarno_posleft` decimal(10,0) DEFAULT NULL,
'''   `shine_dalgarno_posright` decimal(10,0) DEFAULT NULL,
'''   `shine_dalgarno_sequence` varchar(200) DEFAULT NULL,
'''   `shine_dalgarno_note` varchar(2000) DEFAULT NULL,
'''   `sd_internal_comment` longtext,
'''   `key_id_org` varchar(5) NOT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("shine_dalgarno", Database:="regulondb_7_5")>
Public Class shine_dalgarno: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("shine_dalgarno_id"), NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property shine_dalgarno_id As String
    <DatabaseField("gene_id"), NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property gene_id As String
    <DatabaseField("shine_dalgarno_dist_gene"), NotNull, DataType(MySqlDbType.Decimal)> Public Property shine_dalgarno_dist_gene As Decimal
    <DatabaseField("shine_dalgarno_posleft"), DataType(MySqlDbType.Decimal)> Public Property shine_dalgarno_posleft As Decimal
    <DatabaseField("shine_dalgarno_posright"), DataType(MySqlDbType.Decimal)> Public Property shine_dalgarno_posright As Decimal
    <DatabaseField("shine_dalgarno_sequence"), DataType(MySqlDbType.VarChar, "200")> Public Property shine_dalgarno_sequence As String
    <DatabaseField("shine_dalgarno_note"), DataType(MySqlDbType.VarChar, "2000")> Public Property shine_dalgarno_note As String
    <DatabaseField("sd_internal_comment"), DataType(MySqlDbType.Text)> Public Property sd_internal_comment As String
    <DatabaseField("key_id_org"), NotNull, DataType(MySqlDbType.VarChar, "5")> Public Property key_id_org As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `shine_dalgarno` (`shine_dalgarno_id`, `gene_id`, `shine_dalgarno_dist_gene`, `shine_dalgarno_posleft`, `shine_dalgarno_posright`, `shine_dalgarno_sequence`, `shine_dalgarno_note`, `sd_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `shine_dalgarno` (`shine_dalgarno_id`, `gene_id`, `shine_dalgarno_dist_gene`, `shine_dalgarno_posleft`, `shine_dalgarno_posright`, `shine_dalgarno_sequence`, `shine_dalgarno_note`, `sd_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `shine_dalgarno` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `shine_dalgarno` SET `shine_dalgarno_id`='{0}', `gene_id`='{1}', `shine_dalgarno_dist_gene`='{2}', `shine_dalgarno_posleft`='{3}', `shine_dalgarno_posright`='{4}', `shine_dalgarno_sequence`='{5}', `shine_dalgarno_note`='{6}', `sd_internal_comment`='{7}', `key_id_org`='{8}' WHERE ;</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, shine_dalgarno_id, gene_id, shine_dalgarno_dist_gene, shine_dalgarno_posleft, shine_dalgarno_posright, shine_dalgarno_sequence, shine_dalgarno_note, sd_internal_comment, key_id_org)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, shine_dalgarno_id, gene_id, shine_dalgarno_dist_gene, shine_dalgarno_posleft, shine_dalgarno_posright, shine_dalgarno_sequence, shine_dalgarno_note, sd_internal_comment, key_id_org)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace
