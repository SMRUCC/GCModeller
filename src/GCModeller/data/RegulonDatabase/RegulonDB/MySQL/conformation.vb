#Region "Microsoft.VisualBasic::aa00f340f6cd3ba72db778977f532ac1, GCModeller\data\RegulonDatabase\RegulonDB\MySQL\conformation.vb"

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

    '   Total Lines: 170
    '    Code Lines: 87
    ' Comment Lines: 61
    '   Blank Lines: 22
    '     File Size: 10.58 KB


    ' Class conformation
    ' 
    '     Properties: apo_holo_conformation, conformation_id, conformation_internal_comment, conformation_note, conformation_type
    '                 final_state, interaction_type, key_id_org, transcription_factor_id
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
''' DROP TABLE IF EXISTS `conformation`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `conformation` (
'''   `conformation_id` char(12) NOT NULL,
'''   `transcription_factor_id` char(12) NOT NULL,
'''   `final_state` varchar(2000) DEFAULT NULL,
'''   `conformation_note` varchar(2000) DEFAULT NULL,
'''   `interaction_type` varchar(250) DEFAULT NULL,
'''   `conformation_internal_comment` longtext,
'''   `key_id_org` varchar(5) NOT NULL,
'''   `conformation_type` varchar(20) DEFAULT NULL,
'''   `apo_holo_conformation` varchar(10) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("conformation", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `conformation` (
  `conformation_id` char(12) NOT NULL,
  `transcription_factor_id` char(12) NOT NULL,
  `final_state` varchar(2000) DEFAULT NULL,
  `conformation_note` varchar(2000) DEFAULT NULL,
  `interaction_type` varchar(250) DEFAULT NULL,
  `conformation_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL,
  `conformation_type` varchar(20) DEFAULT NULL,
  `apo_holo_conformation` varchar(10) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class conformation: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("conformation_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="conformation_id")> Public Property conformation_id As String
    <DatabaseField("transcription_factor_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="transcription_factor_id")> Public Property transcription_factor_id As String
    <DatabaseField("final_state"), DataType(MySqlDbType.VarChar, "2000"), Column(Name:="final_state")> Public Property final_state As String
    <DatabaseField("conformation_note"), DataType(MySqlDbType.VarChar, "2000"), Column(Name:="conformation_note")> Public Property conformation_note As String
    <DatabaseField("interaction_type"), DataType(MySqlDbType.VarChar, "250"), Column(Name:="interaction_type")> Public Property interaction_type As String
    <DatabaseField("conformation_internal_comment"), DataType(MySqlDbType.Text), Column(Name:="conformation_internal_comment")> Public Property conformation_internal_comment As String
    <DatabaseField("key_id_org"), NotNull, DataType(MySqlDbType.VarChar, "5"), Column(Name:="key_id_org")> Public Property key_id_org As String
    <DatabaseField("conformation_type"), DataType(MySqlDbType.VarChar, "20"), Column(Name:="conformation_type")> Public Property conformation_type As String
    <DatabaseField("apo_holo_conformation"), DataType(MySqlDbType.VarChar, "10"), Column(Name:="apo_holo_conformation")> Public Property apo_holo_conformation As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `conformation` (`conformation_id`, `transcription_factor_id`, `final_state`, `conformation_note`, `interaction_type`, `conformation_internal_comment`, `key_id_org`, `conformation_type`, `apo_holo_conformation`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `conformation` (`conformation_id`, `transcription_factor_id`, `final_state`, `conformation_note`, `interaction_type`, `conformation_internal_comment`, `key_id_org`, `conformation_type`, `apo_holo_conformation`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `conformation` (`conformation_id`, `transcription_factor_id`, `final_state`, `conformation_note`, `interaction_type`, `conformation_internal_comment`, `key_id_org`, `conformation_type`, `apo_holo_conformation`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `conformation` (`conformation_id`, `transcription_factor_id`, `final_state`, `conformation_note`, `interaction_type`, `conformation_internal_comment`, `key_id_org`, `conformation_type`, `apo_holo_conformation`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `conformation` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `conformation` SET `conformation_id`='{0}', `transcription_factor_id`='{1}', `final_state`='{2}', `conformation_note`='{3}', `interaction_type`='{4}', `conformation_internal_comment`='{5}', `key_id_org`='{6}', `conformation_type`='{7}', `apo_holo_conformation`='{8}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `conformation` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `conformation` (`conformation_id`, `transcription_factor_id`, `final_state`, `conformation_note`, `interaction_type`, `conformation_internal_comment`, `key_id_org`, `conformation_type`, `apo_holo_conformation`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, conformation_id, transcription_factor_id, final_state, conformation_note, interaction_type, conformation_internal_comment, key_id_org, conformation_type, apo_holo_conformation)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `conformation` (`conformation_id`, `transcription_factor_id`, `final_state`, `conformation_note`, `interaction_type`, `conformation_internal_comment`, `key_id_org`, `conformation_type`, `apo_holo_conformation`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, conformation_id, transcription_factor_id, final_state, conformation_note, interaction_type, conformation_internal_comment, key_id_org, conformation_type, apo_holo_conformation)
        Else
        Return String.Format(INSERT_SQL, conformation_id, transcription_factor_id, final_state, conformation_note, interaction_type, conformation_internal_comment, key_id_org, conformation_type, apo_holo_conformation)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{conformation_id}', '{transcription_factor_id}', '{final_state}', '{conformation_note}', '{interaction_type}', '{conformation_internal_comment}', '{key_id_org}', '{conformation_type}', '{apo_holo_conformation}')"
        Else
            Return $"('{conformation_id}', '{transcription_factor_id}', '{final_state}', '{conformation_note}', '{interaction_type}', '{conformation_internal_comment}', '{key_id_org}', '{conformation_type}', '{apo_holo_conformation}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `conformation` (`conformation_id`, `transcription_factor_id`, `final_state`, `conformation_note`, `interaction_type`, `conformation_internal_comment`, `key_id_org`, `conformation_type`, `apo_holo_conformation`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, conformation_id, transcription_factor_id, final_state, conformation_note, interaction_type, conformation_internal_comment, key_id_org, conformation_type, apo_holo_conformation)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `conformation` (`conformation_id`, `transcription_factor_id`, `final_state`, `conformation_note`, `interaction_type`, `conformation_internal_comment`, `key_id_org`, `conformation_type`, `apo_holo_conformation`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, conformation_id, transcription_factor_id, final_state, conformation_note, interaction_type, conformation_internal_comment, key_id_org, conformation_type, apo_holo_conformation)
        Else
        Return String.Format(REPLACE_SQL, conformation_id, transcription_factor_id, final_state, conformation_note, interaction_type, conformation_internal_comment, key_id_org, conformation_type, apo_holo_conformation)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `conformation` SET `conformation_id`='{0}', `transcription_factor_id`='{1}', `final_state`='{2}', `conformation_note`='{3}', `interaction_type`='{4}', `conformation_internal_comment`='{5}', `key_id_org`='{6}', `conformation_type`='{7}', `apo_holo_conformation`='{8}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As conformation
                         Return DirectCast(MyClass.MemberwiseClone, conformation)
                     End Function
End Class


End Namespace
