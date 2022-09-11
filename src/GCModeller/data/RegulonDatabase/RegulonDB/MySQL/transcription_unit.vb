#Region "Microsoft.VisualBasic::781ab4119d6748e630f3ee8ea52f0dd2, GCModeller\data\RegulonDatabase\RegulonDB\MySQL\transcription_unit.vb"

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
    '     File Size: 9.08 KB


    ' Class transcription_unit
    ' 
    '     Properties: key_id_org, operon_id, promoter_id, transcription_unit_id, transcription_unit_name
    '                 transcription_unit_note, tu_internal_comment
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
''' DROP TABLE IF EXISTS `transcription_unit`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `transcription_unit` (
'''   `transcription_unit_id` char(12) NOT NULL,
'''   `promoter_id` char(12) DEFAULT NULL,
'''   `transcription_unit_name` varchar(255) DEFAULT NULL,
'''   `operon_id` char(12) DEFAULT NULL,
'''   `transcription_unit_note` varchar(4000) DEFAULT NULL,
'''   `tu_internal_comment` longtext,
'''   `key_id_org` varchar(5) NOT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("transcription_unit", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `transcription_unit` (
  `transcription_unit_id` char(12) NOT NULL,
  `promoter_id` char(12) DEFAULT NULL,
  `transcription_unit_name` varchar(255) DEFAULT NULL,
  `operon_id` char(12) DEFAULT NULL,
  `transcription_unit_note` varchar(4000) DEFAULT NULL,
  `tu_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class transcription_unit: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("transcription_unit_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="transcription_unit_id")> Public Property transcription_unit_id As String
    <DatabaseField("promoter_id"), DataType(MySqlDbType.VarChar, "12"), Column(Name:="promoter_id")> Public Property promoter_id As String
    <DatabaseField("transcription_unit_name"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="transcription_unit_name")> Public Property transcription_unit_name As String
    <DatabaseField("operon_id"), DataType(MySqlDbType.VarChar, "12"), Column(Name:="operon_id")> Public Property operon_id As String
    <DatabaseField("transcription_unit_note"), DataType(MySqlDbType.VarChar, "4000"), Column(Name:="transcription_unit_note")> Public Property transcription_unit_note As String
    <DatabaseField("tu_internal_comment"), DataType(MySqlDbType.Text), Column(Name:="tu_internal_comment")> Public Property tu_internal_comment As String
    <DatabaseField("key_id_org"), NotNull, DataType(MySqlDbType.VarChar, "5"), Column(Name:="key_id_org")> Public Property key_id_org As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `transcription_unit` (`transcription_unit_id`, `promoter_id`, `transcription_unit_name`, `operon_id`, `transcription_unit_note`, `tu_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `transcription_unit` (`transcription_unit_id`, `promoter_id`, `transcription_unit_name`, `operon_id`, `transcription_unit_note`, `tu_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `transcription_unit` (`transcription_unit_id`, `promoter_id`, `transcription_unit_name`, `operon_id`, `transcription_unit_note`, `tu_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `transcription_unit` (`transcription_unit_id`, `promoter_id`, `transcription_unit_name`, `operon_id`, `transcription_unit_note`, `tu_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `transcription_unit` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `transcription_unit` SET `transcription_unit_id`='{0}', `promoter_id`='{1}', `transcription_unit_name`='{2}', `operon_id`='{3}', `transcription_unit_note`='{4}', `tu_internal_comment`='{5}', `key_id_org`='{6}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `transcription_unit` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `transcription_unit` (`transcription_unit_id`, `promoter_id`, `transcription_unit_name`, `operon_id`, `transcription_unit_note`, `tu_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, transcription_unit_id, promoter_id, transcription_unit_name, operon_id, transcription_unit_note, tu_internal_comment, key_id_org)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `transcription_unit` (`transcription_unit_id`, `promoter_id`, `transcription_unit_name`, `operon_id`, `transcription_unit_note`, `tu_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, transcription_unit_id, promoter_id, transcription_unit_name, operon_id, transcription_unit_note, tu_internal_comment, key_id_org)
        Else
        Return String.Format(INSERT_SQL, transcription_unit_id, promoter_id, transcription_unit_name, operon_id, transcription_unit_note, tu_internal_comment, key_id_org)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{transcription_unit_id}', '{promoter_id}', '{transcription_unit_name}', '{operon_id}', '{transcription_unit_note}', '{tu_internal_comment}', '{key_id_org}')"
        Else
            Return $"('{transcription_unit_id}', '{promoter_id}', '{transcription_unit_name}', '{operon_id}', '{transcription_unit_note}', '{tu_internal_comment}', '{key_id_org}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `transcription_unit` (`transcription_unit_id`, `promoter_id`, `transcription_unit_name`, `operon_id`, `transcription_unit_note`, `tu_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, transcription_unit_id, promoter_id, transcription_unit_name, operon_id, transcription_unit_note, tu_internal_comment, key_id_org)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `transcription_unit` (`transcription_unit_id`, `promoter_id`, `transcription_unit_name`, `operon_id`, `transcription_unit_note`, `tu_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, transcription_unit_id, promoter_id, transcription_unit_name, operon_id, transcription_unit_note, tu_internal_comment, key_id_org)
        Else
        Return String.Format(REPLACE_SQL, transcription_unit_id, promoter_id, transcription_unit_name, operon_id, transcription_unit_note, tu_internal_comment, key_id_org)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `transcription_unit` SET `transcription_unit_id`='{0}', `promoter_id`='{1}', `transcription_unit_name`='{2}', `operon_id`='{3}', `transcription_unit_note`='{4}', `tu_internal_comment`='{5}', `key_id_org`='{6}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As transcription_unit
                         Return DirectCast(MyClass.MemberwiseClone, transcription_unit)
                     End Function
End Class


End Namespace
