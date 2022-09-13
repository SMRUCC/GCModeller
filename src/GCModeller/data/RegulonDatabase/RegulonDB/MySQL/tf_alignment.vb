#Region "Microsoft.VisualBasic::052bf34bfb0e4e892180e150ff1f2cc6, GCModeller\data\RegulonDatabase\RegulonDB\MySQL\tf_alignment.vb"

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

    '   Total Lines: 164
    '    Code Lines: 83
    ' Comment Lines: 59
    '   Blank Lines: 22
    '     File Size: 9.13 KB


    ' Class tf_alignment
    ' 
    '     Properties: key_id_org, tf_alignment_id, tf_alignment_internal_comment, tf_alignment_name, tf_alignment_note
    '                 tf_matrix_id, transcription_factor_id
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
''' DROP TABLE IF EXISTS `tf_alignment`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `tf_alignment` (
'''   `tf_alignment_id` char(12) NOT NULL,
'''   `transcription_factor_id` char(12) NOT NULL,
'''   `tf_matrix_id` char(12) DEFAULT NULL,
'''   `tf_alignment_name` varchar(255) NOT NULL,
'''   `tf_alignment_note` varchar(2000) DEFAULT NULL,
'''   `tf_alignment_internal_comment` longtext,
'''   `key_id_org` varchar(5) NOT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("tf_alignment", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `tf_alignment` (
  `tf_alignment_id` char(12) NOT NULL,
  `transcription_factor_id` char(12) NOT NULL,
  `tf_matrix_id` char(12) DEFAULT NULL,
  `tf_alignment_name` varchar(255) NOT NULL,
  `tf_alignment_note` varchar(2000) DEFAULT NULL,
  `tf_alignment_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class tf_alignment: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("tf_alignment_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="tf_alignment_id")> Public Property tf_alignment_id As String
    <DatabaseField("transcription_factor_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="transcription_factor_id")> Public Property transcription_factor_id As String
    <DatabaseField("tf_matrix_id"), DataType(MySqlDbType.VarChar, "12"), Column(Name:="tf_matrix_id")> Public Property tf_matrix_id As String
    <DatabaseField("tf_alignment_name"), NotNull, DataType(MySqlDbType.VarChar, "255"), Column(Name:="tf_alignment_name")> Public Property tf_alignment_name As String
    <DatabaseField("tf_alignment_note"), DataType(MySqlDbType.VarChar, "2000"), Column(Name:="tf_alignment_note")> Public Property tf_alignment_note As String
    <DatabaseField("tf_alignment_internal_comment"), DataType(MySqlDbType.Text), Column(Name:="tf_alignment_internal_comment")> Public Property tf_alignment_internal_comment As String
    <DatabaseField("key_id_org"), NotNull, DataType(MySqlDbType.VarChar, "5"), Column(Name:="key_id_org")> Public Property key_id_org As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `tf_alignment` (`tf_alignment_id`, `transcription_factor_id`, `tf_matrix_id`, `tf_alignment_name`, `tf_alignment_note`, `tf_alignment_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `tf_alignment` (`tf_alignment_id`, `transcription_factor_id`, `tf_matrix_id`, `tf_alignment_name`, `tf_alignment_note`, `tf_alignment_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `tf_alignment` (`tf_alignment_id`, `transcription_factor_id`, `tf_matrix_id`, `tf_alignment_name`, `tf_alignment_note`, `tf_alignment_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `tf_alignment` (`tf_alignment_id`, `transcription_factor_id`, `tf_matrix_id`, `tf_alignment_name`, `tf_alignment_note`, `tf_alignment_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `tf_alignment` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `tf_alignment` SET `tf_alignment_id`='{0}', `transcription_factor_id`='{1}', `tf_matrix_id`='{2}', `tf_alignment_name`='{3}', `tf_alignment_note`='{4}', `tf_alignment_internal_comment`='{5}', `key_id_org`='{6}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `tf_alignment` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `tf_alignment` (`tf_alignment_id`, `transcription_factor_id`, `tf_matrix_id`, `tf_alignment_name`, `tf_alignment_note`, `tf_alignment_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, tf_alignment_id, transcription_factor_id, tf_matrix_id, tf_alignment_name, tf_alignment_note, tf_alignment_internal_comment, key_id_org)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `tf_alignment` (`tf_alignment_id`, `transcription_factor_id`, `tf_matrix_id`, `tf_alignment_name`, `tf_alignment_note`, `tf_alignment_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, tf_alignment_id, transcription_factor_id, tf_matrix_id, tf_alignment_name, tf_alignment_note, tf_alignment_internal_comment, key_id_org)
        Else
        Return String.Format(INSERT_SQL, tf_alignment_id, transcription_factor_id, tf_matrix_id, tf_alignment_name, tf_alignment_note, tf_alignment_internal_comment, key_id_org)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{tf_alignment_id}', '{transcription_factor_id}', '{tf_matrix_id}', '{tf_alignment_name}', '{tf_alignment_note}', '{tf_alignment_internal_comment}', '{key_id_org}')"
        Else
            Return $"('{tf_alignment_id}', '{transcription_factor_id}', '{tf_matrix_id}', '{tf_alignment_name}', '{tf_alignment_note}', '{tf_alignment_internal_comment}', '{key_id_org}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `tf_alignment` (`tf_alignment_id`, `transcription_factor_id`, `tf_matrix_id`, `tf_alignment_name`, `tf_alignment_note`, `tf_alignment_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, tf_alignment_id, transcription_factor_id, tf_matrix_id, tf_alignment_name, tf_alignment_note, tf_alignment_internal_comment, key_id_org)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `tf_alignment` (`tf_alignment_id`, `transcription_factor_id`, `tf_matrix_id`, `tf_alignment_name`, `tf_alignment_note`, `tf_alignment_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, tf_alignment_id, transcription_factor_id, tf_matrix_id, tf_alignment_name, tf_alignment_note, tf_alignment_internal_comment, key_id_org)
        Else
        Return String.Format(REPLACE_SQL, tf_alignment_id, transcription_factor_id, tf_matrix_id, tf_alignment_name, tf_alignment_note, tf_alignment_internal_comment, key_id_org)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `tf_alignment` SET `tf_alignment_id`='{0}', `transcription_factor_id`='{1}', `tf_matrix_id`='{2}', `tf_alignment_name`='{3}', `tf_alignment_note`='{4}', `tf_alignment_internal_comment`='{5}', `key_id_org`='{6}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As tf_alignment
                         Return DirectCast(MyClass.MemberwiseClone, tf_alignment)
                     End Function
End Class


End Namespace
