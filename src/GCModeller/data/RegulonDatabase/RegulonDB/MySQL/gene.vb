#Region "Microsoft.VisualBasic::42898018b8abf2c912bfcb9ef6c88f9a, ..\GCModeller\data\RegulonDatabase\RegulonDB\MySQL\gene.vb"

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
''' DROP TABLE IF EXISTS `gene`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `gene` (
'''   `gene_id` char(12) NOT NULL,
'''   `gene_name` varchar(255) DEFAULT NULL,
'''   `gene_posleft` decimal(10,0) DEFAULT NULL,
'''   `gene_posright` decimal(10,0) DEFAULT NULL,
'''   `gene_strand` varchar(10) DEFAULT NULL,
'''   `gene_sequence` longtext,
'''   `gc_content` decimal(15,10) DEFAULT NULL,
'''   `cri_score` decimal(15,10) DEFAULT NULL,
'''   `gene_note` varchar(2000) DEFAULT NULL,
'''   `gene_internal_comment` longtext,
'''   `key_id_org` varchar(5) NOT NULL,
'''   `gene_type` varchar(100) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("gene", Database:="regulondb_7_5")>
Public Class gene: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("gene_id"), NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property gene_id As String
    <DatabaseField("gene_name"), DataType(MySqlDbType.VarChar, "255")> Public Property gene_name As String
    <DatabaseField("gene_posleft"), DataType(MySqlDbType.Decimal)> Public Property gene_posleft As Decimal
    <DatabaseField("gene_posright"), DataType(MySqlDbType.Decimal)> Public Property gene_posright As Decimal
    <DatabaseField("gene_strand"), DataType(MySqlDbType.VarChar, "10")> Public Property gene_strand As String
    <DatabaseField("gene_sequence"), DataType(MySqlDbType.Text)> Public Property gene_sequence As String
    <DatabaseField("gc_content"), DataType(MySqlDbType.Decimal)> Public Property gc_content As Decimal
    <DatabaseField("cri_score"), DataType(MySqlDbType.Decimal)> Public Property cri_score As Decimal
    <DatabaseField("gene_note"), DataType(MySqlDbType.VarChar, "2000")> Public Property gene_note As String
    <DatabaseField("gene_internal_comment"), DataType(MySqlDbType.Text)> Public Property gene_internal_comment As String
    <DatabaseField("key_id_org"), NotNull, DataType(MySqlDbType.VarChar, "5")> Public Property key_id_org As String
    <DatabaseField("gene_type"), DataType(MySqlDbType.VarChar, "100")> Public Property gene_type As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `gene` (`gene_id`, `gene_name`, `gene_posleft`, `gene_posright`, `gene_strand`, `gene_sequence`, `gc_content`, `cri_score`, `gene_note`, `gene_internal_comment`, `key_id_org`, `gene_type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `gene` (`gene_id`, `gene_name`, `gene_posleft`, `gene_posright`, `gene_strand`, `gene_sequence`, `gc_content`, `cri_score`, `gene_note`, `gene_internal_comment`, `key_id_org`, `gene_type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `gene` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `gene` SET `gene_id`='{0}', `gene_name`='{1}', `gene_posleft`='{2}', `gene_posright`='{3}', `gene_strand`='{4}', `gene_sequence`='{5}', `gc_content`='{6}', `cri_score`='{7}', `gene_note`='{8}', `gene_internal_comment`='{9}', `key_id_org`='{10}', `gene_type`='{11}' WHERE ;</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, gene_id, gene_name, gene_posleft, gene_posright, gene_strand, gene_sequence, gc_content, cri_score, gene_note, gene_internal_comment, key_id_org, gene_type)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, gene_id, gene_name, gene_posleft, gene_posright, gene_strand, gene_sequence, gc_content, cri_score, gene_note, gene_internal_comment, key_id_org, gene_type)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace
