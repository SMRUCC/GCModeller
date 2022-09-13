#Region "Microsoft.VisualBasic::fa84f67fb8d2f712d1f2b5379d916869, GCModeller\data\RegulonDatabase\RegulonDB\MySQL\effector.vb"

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
    '     File Size: 8.38 KB


    ' Class effector
    ' 
    '     Properties: category, effector_id, effector_internal_comment, effector_name, effector_note
    '                 effector_type, key_id_org
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
''' DROP TABLE IF EXISTS `effector`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `effector` (
'''   `effector_id` char(12) NOT NULL,
'''   `effector_name` varchar(255) NOT NULL,
'''   `category` varchar(10) DEFAULT NULL,
'''   `effector_type` varchar(100) DEFAULT NULL,
'''   `effector_note` varchar(2000) DEFAULT NULL,
'''   `effector_internal_comment` longtext,
'''   `key_id_org` varchar(5) NOT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("effector", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `effector` (
  `effector_id` char(12) NOT NULL,
  `effector_name` varchar(255) NOT NULL,
  `category` varchar(10) DEFAULT NULL,
  `effector_type` varchar(100) DEFAULT NULL,
  `effector_note` varchar(2000) DEFAULT NULL,
  `effector_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class effector: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("effector_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="effector_id")> Public Property effector_id As String
    <DatabaseField("effector_name"), NotNull, DataType(MySqlDbType.VarChar, "255"), Column(Name:="effector_name")> Public Property effector_name As String
    <DatabaseField("category"), DataType(MySqlDbType.VarChar, "10"), Column(Name:="category")> Public Property category As String
    <DatabaseField("effector_type"), DataType(MySqlDbType.VarChar, "100"), Column(Name:="effector_type")> Public Property effector_type As String
    <DatabaseField("effector_note"), DataType(MySqlDbType.VarChar, "2000"), Column(Name:="effector_note")> Public Property effector_note As String
    <DatabaseField("effector_internal_comment"), DataType(MySqlDbType.Text), Column(Name:="effector_internal_comment")> Public Property effector_internal_comment As String
    <DatabaseField("key_id_org"), NotNull, DataType(MySqlDbType.VarChar, "5"), Column(Name:="key_id_org")> Public Property key_id_org As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `effector` (`effector_id`, `effector_name`, `category`, `effector_type`, `effector_note`, `effector_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `effector` (`effector_id`, `effector_name`, `category`, `effector_type`, `effector_note`, `effector_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `effector` (`effector_id`, `effector_name`, `category`, `effector_type`, `effector_note`, `effector_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `effector` (`effector_id`, `effector_name`, `category`, `effector_type`, `effector_note`, `effector_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `effector` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `effector` SET `effector_id`='{0}', `effector_name`='{1}', `category`='{2}', `effector_type`='{3}', `effector_note`='{4}', `effector_internal_comment`='{5}', `key_id_org`='{6}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `effector` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `effector` (`effector_id`, `effector_name`, `category`, `effector_type`, `effector_note`, `effector_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, effector_id, effector_name, category, effector_type, effector_note, effector_internal_comment, key_id_org)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `effector` (`effector_id`, `effector_name`, `category`, `effector_type`, `effector_note`, `effector_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, effector_id, effector_name, category, effector_type, effector_note, effector_internal_comment, key_id_org)
        Else
        Return String.Format(INSERT_SQL, effector_id, effector_name, category, effector_type, effector_note, effector_internal_comment, key_id_org)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{effector_id}', '{effector_name}', '{category}', '{effector_type}', '{effector_note}', '{effector_internal_comment}', '{key_id_org}')"
        Else
            Return $"('{effector_id}', '{effector_name}', '{category}', '{effector_type}', '{effector_note}', '{effector_internal_comment}', '{key_id_org}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `effector` (`effector_id`, `effector_name`, `category`, `effector_type`, `effector_note`, `effector_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, effector_id, effector_name, category, effector_type, effector_note, effector_internal_comment, key_id_org)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `effector` (`effector_id`, `effector_name`, `category`, `effector_type`, `effector_note`, `effector_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, effector_id, effector_name, category, effector_type, effector_note, effector_internal_comment, key_id_org)
        Else
        Return String.Format(REPLACE_SQL, effector_id, effector_name, category, effector_type, effector_note, effector_internal_comment, key_id_org)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `effector` SET `effector_id`='{0}', `effector_name`='{1}', `category`='{2}', `effector_type`='{3}', `effector_note`='{4}', `effector_internal_comment`='{5}', `key_id_org`='{6}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As effector
                         Return DirectCast(MyClass.MemberwiseClone, effector)
                     End Function
End Class


End Namespace
