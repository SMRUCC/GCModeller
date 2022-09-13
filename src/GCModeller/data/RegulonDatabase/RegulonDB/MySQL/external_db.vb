#Region "Microsoft.VisualBasic::acd080ffebf06323748d3200274ab403, GCModeller\data\RegulonDatabase\RegulonDB\MySQL\external_db.vb"

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

    '   Total Lines: 161
    '    Code Lines: 81
    ' Comment Lines: 58
    '   Blank Lines: 22
    '     File Size: 8.18 KB


    ' Class external_db
    ' 
    '     Properties: ext_db_internal_comment, external_db_description, external_db_id, external_db_name, external_db_note
    '                 url
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
''' DROP TABLE IF EXISTS `external_db`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `external_db` (
'''   `external_db_id` char(12) NOT NULL,
'''   `external_db_name` varchar(255) NOT NULL,
'''   `external_db_description` varchar(255) DEFAULT NULL,
'''   `url` varchar(255) NOT NULL,
'''   `external_db_note` varchar(2000) DEFAULT NULL,
'''   `ext_db_internal_comment` longtext
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("external_db", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `external_db` (
  `external_db_id` char(12) NOT NULL,
  `external_db_name` varchar(255) NOT NULL,
  `external_db_description` varchar(255) DEFAULT NULL,
  `url` varchar(255) NOT NULL,
  `external_db_note` varchar(2000) DEFAULT NULL,
  `ext_db_internal_comment` longtext
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class external_db: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("external_db_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="external_db_id")> Public Property external_db_id As String
    <DatabaseField("external_db_name"), NotNull, DataType(MySqlDbType.VarChar, "255"), Column(Name:="external_db_name")> Public Property external_db_name As String
    <DatabaseField("external_db_description"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="external_db_description")> Public Property external_db_description As String
    <DatabaseField("url"), NotNull, DataType(MySqlDbType.VarChar, "255"), Column(Name:="url")> Public Property url As String
    <DatabaseField("external_db_note"), DataType(MySqlDbType.VarChar, "2000"), Column(Name:="external_db_note")> Public Property external_db_note As String
    <DatabaseField("ext_db_internal_comment"), DataType(MySqlDbType.Text), Column(Name:="ext_db_internal_comment")> Public Property ext_db_internal_comment As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `external_db` (`external_db_id`, `external_db_name`, `external_db_description`, `url`, `external_db_note`, `ext_db_internal_comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `external_db` (`external_db_id`, `external_db_name`, `external_db_description`, `url`, `external_db_note`, `ext_db_internal_comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `external_db` (`external_db_id`, `external_db_name`, `external_db_description`, `url`, `external_db_note`, `ext_db_internal_comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `external_db` (`external_db_id`, `external_db_name`, `external_db_description`, `url`, `external_db_note`, `ext_db_internal_comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `external_db` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `external_db` SET `external_db_id`='{0}', `external_db_name`='{1}', `external_db_description`='{2}', `url`='{3}', `external_db_note`='{4}', `ext_db_internal_comment`='{5}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `external_db` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `external_db` (`external_db_id`, `external_db_name`, `external_db_description`, `url`, `external_db_note`, `ext_db_internal_comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, external_db_id, external_db_name, external_db_description, url, external_db_note, ext_db_internal_comment)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `external_db` (`external_db_id`, `external_db_name`, `external_db_description`, `url`, `external_db_note`, `ext_db_internal_comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, external_db_id, external_db_name, external_db_description, url, external_db_note, ext_db_internal_comment)
        Else
        Return String.Format(INSERT_SQL, external_db_id, external_db_name, external_db_description, url, external_db_note, ext_db_internal_comment)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{external_db_id}', '{external_db_name}', '{external_db_description}', '{url}', '{external_db_note}', '{ext_db_internal_comment}')"
        Else
            Return $"('{external_db_id}', '{external_db_name}', '{external_db_description}', '{url}', '{external_db_note}', '{ext_db_internal_comment}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `external_db` (`external_db_id`, `external_db_name`, `external_db_description`, `url`, `external_db_note`, `ext_db_internal_comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, external_db_id, external_db_name, external_db_description, url, external_db_note, ext_db_internal_comment)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `external_db` (`external_db_id`, `external_db_name`, `external_db_description`, `url`, `external_db_note`, `ext_db_internal_comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, external_db_id, external_db_name, external_db_description, url, external_db_note, ext_db_internal_comment)
        Else
        Return String.Format(REPLACE_SQL, external_db_id, external_db_name, external_db_description, url, external_db_note, ext_db_internal_comment)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `external_db` SET `external_db_id`='{0}', `external_db_name`='{1}', `external_db_description`='{2}', `url`='{3}', `external_db_note`='{4}', `ext_db_internal_comment`='{5}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As external_db
                         Return DirectCast(MyClass.MemberwiseClone, external_db)
                     End Function
End Class


End Namespace
