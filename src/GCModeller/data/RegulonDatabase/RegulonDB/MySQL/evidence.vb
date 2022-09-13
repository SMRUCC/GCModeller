#Region "Microsoft.VisualBasic::d5a9a3c04ed3fc99d957121a9ba02903, GCModeller\data\RegulonDatabase\RegulonDB\MySQL\evidence.vb"

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
    '     File Size: 10.63 KB


    ' Class evidence
    ' 
    '     Properties: evidence_category, evidence_code, evidence_id, evidence_internal_comment, evidence_name
    '                 evidence_note, evidence_type, example, head, key_id_org
    '                 type_object
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
''' DROP TABLE IF EXISTS `evidence`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `evidence` (
'''   `evidence_id` char(12) NOT NULL,
'''   `evidence_name` varchar(2000) NOT NULL,
'''   `type_object` varchar(200) DEFAULT NULL,
'''   `evidence_code` varchar(30) DEFAULT NULL,
'''   `evidence_note` varchar(2000) DEFAULT NULL,
'''   `evidence_internal_comment` longtext,
'''   `key_id_org` varchar(5) NOT NULL,
'''   `evidence_type` char(3) DEFAULT NULL,
'''   `evidence_category` varchar(200) DEFAULT NULL,
'''   `head` varchar(12) DEFAULT NULL,
'''   `example` varchar(500) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("evidence", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `evidence` (
  `evidence_id` char(12) NOT NULL,
  `evidence_name` varchar(2000) NOT NULL,
  `type_object` varchar(200) DEFAULT NULL,
  `evidence_code` varchar(30) DEFAULT NULL,
  `evidence_note` varchar(2000) DEFAULT NULL,
  `evidence_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL,
  `evidence_type` char(3) DEFAULT NULL,
  `evidence_category` varchar(200) DEFAULT NULL,
  `head` varchar(12) DEFAULT NULL,
  `example` varchar(500) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class evidence: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("evidence_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="evidence_id")> Public Property evidence_id As String
    <DatabaseField("evidence_name"), NotNull, DataType(MySqlDbType.VarChar, "2000"), Column(Name:="evidence_name")> Public Property evidence_name As String
    <DatabaseField("type_object"), DataType(MySqlDbType.VarChar, "200"), Column(Name:="type_object")> Public Property type_object As String
    <DatabaseField("evidence_code"), DataType(MySqlDbType.VarChar, "30"), Column(Name:="evidence_code")> Public Property evidence_code As String
    <DatabaseField("evidence_note"), DataType(MySqlDbType.VarChar, "2000"), Column(Name:="evidence_note")> Public Property evidence_note As String
    <DatabaseField("evidence_internal_comment"), DataType(MySqlDbType.Text), Column(Name:="evidence_internal_comment")> Public Property evidence_internal_comment As String
    <DatabaseField("key_id_org"), NotNull, DataType(MySqlDbType.VarChar, "5"), Column(Name:="key_id_org")> Public Property key_id_org As String
    <DatabaseField("evidence_type"), DataType(MySqlDbType.VarChar, "3"), Column(Name:="evidence_type")> Public Property evidence_type As String
    <DatabaseField("evidence_category"), DataType(MySqlDbType.VarChar, "200"), Column(Name:="evidence_category")> Public Property evidence_category As String
    <DatabaseField("head"), DataType(MySqlDbType.VarChar, "12"), Column(Name:="head")> Public Property head As String
    <DatabaseField("example"), DataType(MySqlDbType.VarChar, "500"), Column(Name:="example")> Public Property example As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `evidence` (`evidence_id`, `evidence_name`, `type_object`, `evidence_code`, `evidence_note`, `evidence_internal_comment`, `key_id_org`, `evidence_type`, `evidence_category`, `head`, `example`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `evidence` (`evidence_id`, `evidence_name`, `type_object`, `evidence_code`, `evidence_note`, `evidence_internal_comment`, `key_id_org`, `evidence_type`, `evidence_category`, `head`, `example`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `evidence` (`evidence_id`, `evidence_name`, `type_object`, `evidence_code`, `evidence_note`, `evidence_internal_comment`, `key_id_org`, `evidence_type`, `evidence_category`, `head`, `example`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `evidence` (`evidence_id`, `evidence_name`, `type_object`, `evidence_code`, `evidence_note`, `evidence_internal_comment`, `key_id_org`, `evidence_type`, `evidence_category`, `head`, `example`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `evidence` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `evidence` SET `evidence_id`='{0}', `evidence_name`='{1}', `type_object`='{2}', `evidence_code`='{3}', `evidence_note`='{4}', `evidence_internal_comment`='{5}', `key_id_org`='{6}', `evidence_type`='{7}', `evidence_category`='{8}', `head`='{9}', `example`='{10}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `evidence` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `evidence` (`evidence_id`, `evidence_name`, `type_object`, `evidence_code`, `evidence_note`, `evidence_internal_comment`, `key_id_org`, `evidence_type`, `evidence_category`, `head`, `example`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, evidence_id, evidence_name, type_object, evidence_code, evidence_note, evidence_internal_comment, key_id_org, evidence_type, evidence_category, head, example)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `evidence` (`evidence_id`, `evidence_name`, `type_object`, `evidence_code`, `evidence_note`, `evidence_internal_comment`, `key_id_org`, `evidence_type`, `evidence_category`, `head`, `example`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, evidence_id, evidence_name, type_object, evidence_code, evidence_note, evidence_internal_comment, key_id_org, evidence_type, evidence_category, head, example)
        Else
        Return String.Format(INSERT_SQL, evidence_id, evidence_name, type_object, evidence_code, evidence_note, evidence_internal_comment, key_id_org, evidence_type, evidence_category, head, example)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{evidence_id}', '{evidence_name}', '{type_object}', '{evidence_code}', '{evidence_note}', '{evidence_internal_comment}', '{key_id_org}', '{evidence_type}', '{evidence_category}', '{head}', '{example}')"
        Else
            Return $"('{evidence_id}', '{evidence_name}', '{type_object}', '{evidence_code}', '{evidence_note}', '{evidence_internal_comment}', '{key_id_org}', '{evidence_type}', '{evidence_category}', '{head}', '{example}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `evidence` (`evidence_id`, `evidence_name`, `type_object`, `evidence_code`, `evidence_note`, `evidence_internal_comment`, `key_id_org`, `evidence_type`, `evidence_category`, `head`, `example`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, evidence_id, evidence_name, type_object, evidence_code, evidence_note, evidence_internal_comment, key_id_org, evidence_type, evidence_category, head, example)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `evidence` (`evidence_id`, `evidence_name`, `type_object`, `evidence_code`, `evidence_note`, `evidence_internal_comment`, `key_id_org`, `evidence_type`, `evidence_category`, `head`, `example`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, evidence_id, evidence_name, type_object, evidence_code, evidence_note, evidence_internal_comment, key_id_org, evidence_type, evidence_category, head, example)
        Else
        Return String.Format(REPLACE_SQL, evidence_id, evidence_name, type_object, evidence_code, evidence_note, evidence_internal_comment, key_id_org, evidence_type, evidence_category, head, example)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `evidence` SET `evidence_id`='{0}', `evidence_name`='{1}', `type_object`='{2}', `evidence_code`='{3}', `evidence_note`='{4}', `evidence_internal_comment`='{5}', `key_id_org`='{6}', `evidence_type`='{7}', `evidence_category`='{8}', `head`='{9}', `example`='{10}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As evidence
                         Return DirectCast(MyClass.MemberwiseClone, evidence)
                     End Function
End Class


End Namespace
