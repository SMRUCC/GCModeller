#Region "Microsoft.VisualBasic::385ffff5aff3d3ca44b9a13b52ce0f14, ..\GCModeller\data\RegulonDatabase\RegulonDB\MySQL\tf_matrix_align_link.vb"

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
''' DROP TABLE IF EXISTS `tf_matrix_align_link`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `tf_matrix_align_link` (
'''   `tf_matrix_id` char(12) NOT NULL,
'''   `tf_alignment_id` char(12) DEFAULT NULL,
'''   `transcription_factor_id` char(12) NOT NULL,
'''   `tf_matrix_name` varchar(255) DEFAULT NULL,
'''   `media` decimal(5,2) NOT NULL,
'''   `standar_desv` decimal(8,3) NOT NULL,
'''   `score_low` decimal(5,2) NOT NULL,
'''   `score_high` decimal(5,2) DEFAULT NULL,
'''   `tf_matrix_note` varchar(2000) DEFAULT NULL,
'''   `tf_matrix_internal_comment` longtext,
'''   `key_id_org` varchar(5) NOT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("tf_matrix_align_link", Database:="regulondb_7_5")>
Public Class tf_matrix_align_link: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("tf_matrix_id"), NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property tf_matrix_id As String
    <DatabaseField("tf_alignment_id"), DataType(MySqlDbType.VarChar, "12")> Public Property tf_alignment_id As String
    <DatabaseField("transcription_factor_id"), NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property transcription_factor_id As String
    <DatabaseField("tf_matrix_name"), DataType(MySqlDbType.VarChar, "255")> Public Property tf_matrix_name As String
    <DatabaseField("media"), NotNull, DataType(MySqlDbType.Decimal)> Public Property media As Decimal
    <DatabaseField("standar_desv"), NotNull, DataType(MySqlDbType.Decimal)> Public Property standar_desv As Decimal
    <DatabaseField("score_low"), NotNull, DataType(MySqlDbType.Decimal)> Public Property score_low As Decimal
    <DatabaseField("score_high"), DataType(MySqlDbType.Decimal)> Public Property score_high As Decimal
    <DatabaseField("tf_matrix_note"), DataType(MySqlDbType.VarChar, "2000")> Public Property tf_matrix_note As String
    <DatabaseField("tf_matrix_internal_comment"), DataType(MySqlDbType.Text)> Public Property tf_matrix_internal_comment As String
    <DatabaseField("key_id_org"), NotNull, DataType(MySqlDbType.VarChar, "5")> Public Property key_id_org As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `tf_matrix_align_link` (`tf_matrix_id`, `tf_alignment_id`, `transcription_factor_id`, `tf_matrix_name`, `media`, `standar_desv`, `score_low`, `score_high`, `tf_matrix_note`, `tf_matrix_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `tf_matrix_align_link` (`tf_matrix_id`, `tf_alignment_id`, `transcription_factor_id`, `tf_matrix_name`, `media`, `standar_desv`, `score_low`, `score_high`, `tf_matrix_note`, `tf_matrix_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `tf_matrix_align_link` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `tf_matrix_align_link` SET `tf_matrix_id`='{0}', `tf_alignment_id`='{1}', `transcription_factor_id`='{2}', `tf_matrix_name`='{3}', `media`='{4}', `standar_desv`='{5}', `score_low`='{6}', `score_high`='{7}', `tf_matrix_note`='{8}', `tf_matrix_internal_comment`='{9}', `key_id_org`='{10}' WHERE ;</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, tf_matrix_id, tf_alignment_id, transcription_factor_id, tf_matrix_name, media, standar_desv, score_low, score_high, tf_matrix_note, tf_matrix_internal_comment, key_id_org)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, tf_matrix_id, tf_alignment_id, transcription_factor_id, tf_matrix_name, media, standar_desv, score_low, score_high, tf_matrix_note, tf_matrix_internal_comment, key_id_org)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace
