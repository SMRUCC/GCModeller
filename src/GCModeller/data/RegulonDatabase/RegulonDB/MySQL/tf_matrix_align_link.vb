#Region "Microsoft.VisualBasic::d2a8e6efc2613375aad6326e8bedb65c, GCModeller\data\RegulonDatabase\RegulonDB\MySQL\tf_matrix_align_link.vb"

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

    '   Total Lines: 176
    '    Code Lines: 91
    ' Comment Lines: 63
    '   Blank Lines: 22
    '     File Size: 11.13 KB


    ' Class tf_matrix_align_link
    ' 
    '     Properties: key_id_org, media, score_high, score_low, standar_desv
    '                 tf_alignment_id, tf_matrix_id, tf_matrix_internal_comment, tf_matrix_name, tf_matrix_note
    '                 transcription_factor_id
    ' 
    '     Function: Clone, GetDeleteSQL, GetDumpInsertValue, (+2 Overloads) GetInsertSQL, (+2 Overloads) GetReplaceSQL
    '               GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @2018/5/23 13:13:36


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace RegulonDB.Tables

''' <summary>
''' ```SQL
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
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("tf_matrix_align_link", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `tf_matrix_align_link` (
  `tf_matrix_id` char(12) NOT NULL,
  `tf_alignment_id` char(12) DEFAULT NULL,
  `transcription_factor_id` char(12) NOT NULL,
  `tf_matrix_name` varchar(255) DEFAULT NULL,
  `media` decimal(5,2) NOT NULL,
  `standar_desv` decimal(8,3) NOT NULL,
  `score_low` decimal(5,2) NOT NULL,
  `score_high` decimal(5,2) DEFAULT NULL,
  `tf_matrix_note` varchar(2000) DEFAULT NULL,
  `tf_matrix_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class tf_matrix_align_link: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("tf_matrix_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="tf_matrix_id")> Public Property tf_matrix_id As String
    <DatabaseField("tf_alignment_id"), DataType(MySqlDbType.VarChar, "12"), Column(Name:="tf_alignment_id")> Public Property tf_alignment_id As String
    <DatabaseField("transcription_factor_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="transcription_factor_id")> Public Property transcription_factor_id As String
    <DatabaseField("tf_matrix_name"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="tf_matrix_name")> Public Property tf_matrix_name As String
    <DatabaseField("media"), NotNull, DataType(MySqlDbType.Decimal), Column(Name:="media")> Public Property media As Decimal
    <DatabaseField("standar_desv"), NotNull, DataType(MySqlDbType.Decimal), Column(Name:="standar_desv")> Public Property standar_desv As Decimal
    <DatabaseField("score_low"), NotNull, DataType(MySqlDbType.Decimal), Column(Name:="score_low")> Public Property score_low As Decimal
    <DatabaseField("score_high"), DataType(MySqlDbType.Decimal), Column(Name:="score_high")> Public Property score_high As Decimal
    <DatabaseField("tf_matrix_note"), DataType(MySqlDbType.VarChar, "2000"), Column(Name:="tf_matrix_note")> Public Property tf_matrix_note As String
    <DatabaseField("tf_matrix_internal_comment"), DataType(MySqlDbType.Text), Column(Name:="tf_matrix_internal_comment")> Public Property tf_matrix_internal_comment As String
    <DatabaseField("key_id_org"), NotNull, DataType(MySqlDbType.VarChar, "5"), Column(Name:="key_id_org")> Public Property key_id_org As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `tf_matrix_align_link` (`tf_matrix_id`, `tf_alignment_id`, `transcription_factor_id`, `tf_matrix_name`, `media`, `standar_desv`, `score_low`, `score_high`, `tf_matrix_note`, `tf_matrix_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `tf_matrix_align_link` (`tf_matrix_id`, `tf_alignment_id`, `transcription_factor_id`, `tf_matrix_name`, `media`, `standar_desv`, `score_low`, `score_high`, `tf_matrix_note`, `tf_matrix_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `tf_matrix_align_link` (`tf_matrix_id`, `tf_alignment_id`, `transcription_factor_id`, `tf_matrix_name`, `media`, `standar_desv`, `score_low`, `score_high`, `tf_matrix_note`, `tf_matrix_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `tf_matrix_align_link` (`tf_matrix_id`, `tf_alignment_id`, `transcription_factor_id`, `tf_matrix_name`, `media`, `standar_desv`, `score_low`, `score_high`, `tf_matrix_note`, `tf_matrix_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `tf_matrix_align_link` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `tf_matrix_align_link` SET `tf_matrix_id`='{0}', `tf_alignment_id`='{1}', `transcription_factor_id`='{2}', `tf_matrix_name`='{3}', `media`='{4}', `standar_desv`='{5}', `score_low`='{6}', `score_high`='{7}', `tf_matrix_note`='{8}', `tf_matrix_internal_comment`='{9}', `key_id_org`='{10}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `tf_matrix_align_link` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `tf_matrix_align_link` (`tf_matrix_id`, `tf_alignment_id`, `transcription_factor_id`, `tf_matrix_name`, `media`, `standar_desv`, `score_low`, `score_high`, `tf_matrix_note`, `tf_matrix_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, tf_matrix_id, tf_alignment_id, transcription_factor_id, tf_matrix_name, media, standar_desv, score_low, score_high, tf_matrix_note, tf_matrix_internal_comment, key_id_org)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `tf_matrix_align_link` (`tf_matrix_id`, `tf_alignment_id`, `transcription_factor_id`, `tf_matrix_name`, `media`, `standar_desv`, `score_low`, `score_high`, `tf_matrix_note`, `tf_matrix_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, tf_matrix_id, tf_alignment_id, transcription_factor_id, tf_matrix_name, media, standar_desv, score_low, score_high, tf_matrix_note, tf_matrix_internal_comment, key_id_org)
        Else
        Return String.Format(INSERT_SQL, tf_matrix_id, tf_alignment_id, transcription_factor_id, tf_matrix_name, media, standar_desv, score_low, score_high, tf_matrix_note, tf_matrix_internal_comment, key_id_org)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{tf_matrix_id}', '{tf_alignment_id}', '{transcription_factor_id}', '{tf_matrix_name}', '{media}', '{standar_desv}', '{score_low}', '{score_high}', '{tf_matrix_note}', '{tf_matrix_internal_comment}', '{key_id_org}')"
        Else
            Return $"('{tf_matrix_id}', '{tf_alignment_id}', '{transcription_factor_id}', '{tf_matrix_name}', '{media}', '{standar_desv}', '{score_low}', '{score_high}', '{tf_matrix_note}', '{tf_matrix_internal_comment}', '{key_id_org}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `tf_matrix_align_link` (`tf_matrix_id`, `tf_alignment_id`, `transcription_factor_id`, `tf_matrix_name`, `media`, `standar_desv`, `score_low`, `score_high`, `tf_matrix_note`, `tf_matrix_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, tf_matrix_id, tf_alignment_id, transcription_factor_id, tf_matrix_name, media, standar_desv, score_low, score_high, tf_matrix_note, tf_matrix_internal_comment, key_id_org)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `tf_matrix_align_link` (`tf_matrix_id`, `tf_alignment_id`, `transcription_factor_id`, `tf_matrix_name`, `media`, `standar_desv`, `score_low`, `score_high`, `tf_matrix_note`, `tf_matrix_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, tf_matrix_id, tf_alignment_id, transcription_factor_id, tf_matrix_name, media, standar_desv, score_low, score_high, tf_matrix_note, tf_matrix_internal_comment, key_id_org)
        Else
        Return String.Format(REPLACE_SQL, tf_matrix_id, tf_alignment_id, transcription_factor_id, tf_matrix_name, media, standar_desv, score_low, score_high, tf_matrix_note, tf_matrix_internal_comment, key_id_org)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `tf_matrix_align_link` SET `tf_matrix_id`='{0}', `tf_alignment_id`='{1}', `transcription_factor_id`='{2}', `tf_matrix_name`='{3}', `media`='{4}', `standar_desv`='{5}', `score_low`='{6}', `score_high`='{7}', `tf_matrix_note`='{8}', `tf_matrix_internal_comment`='{9}', `key_id_org`='{10}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As tf_matrix_align_link
                         Return DirectCast(MyClass.MemberwiseClone, tf_matrix_align_link)
                     End Function
End Class


End Namespace
