#Region "Microsoft.VisualBasic::aafd9460dbc82f8f5a69c3edfd7bba46, GCModeller\data\RegulonDatabase\RegulonDB\MySQL\publication.vb"

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
    '     File Size: 9.36 KB


    ' Class publication
    ' 
    '     Properties: author, external_db_id, publication_id, publication_internal_comment, publication_note
    '                 reference_id, source, title, years
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
''' DROP TABLE IF EXISTS `publication`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `publication` (
'''   `publication_id` char(12) NOT NULL,
'''   `reference_id` varchar(255) NOT NULL,
'''   `external_db_id` char(12) NOT NULL,
'''   `author` varchar(2000) DEFAULT NULL,
'''   `title` varchar(2000) DEFAULT NULL,
'''   `source` varchar(2000) DEFAULT NULL,
'''   `years` varchar(50) DEFAULT NULL,
'''   `publication_note` varchar(2000) DEFAULT NULL,
'''   `publication_internal_comment` longtext
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("publication", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `publication` (
  `publication_id` char(12) NOT NULL,
  `reference_id` varchar(255) NOT NULL,
  `external_db_id` char(12) NOT NULL,
  `author` varchar(2000) DEFAULT NULL,
  `title` varchar(2000) DEFAULT NULL,
  `source` varchar(2000) DEFAULT NULL,
  `years` varchar(50) DEFAULT NULL,
  `publication_note` varchar(2000) DEFAULT NULL,
  `publication_internal_comment` longtext
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class publication: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("publication_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="publication_id")> Public Property publication_id As String
    <DatabaseField("reference_id"), NotNull, DataType(MySqlDbType.VarChar, "255"), Column(Name:="reference_id")> Public Property reference_id As String
    <DatabaseField("external_db_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="external_db_id")> Public Property external_db_id As String
    <DatabaseField("author"), DataType(MySqlDbType.VarChar, "2000"), Column(Name:="author")> Public Property author As String
    <DatabaseField("title"), DataType(MySqlDbType.VarChar, "2000"), Column(Name:="title")> Public Property title As String
    <DatabaseField("source"), DataType(MySqlDbType.VarChar, "2000"), Column(Name:="source")> Public Property source As String
    <DatabaseField("years"), DataType(MySqlDbType.VarChar, "50"), Column(Name:="years")> Public Property years As String
    <DatabaseField("publication_note"), DataType(MySqlDbType.VarChar, "2000"), Column(Name:="publication_note")> Public Property publication_note As String
    <DatabaseField("publication_internal_comment"), DataType(MySqlDbType.Text), Column(Name:="publication_internal_comment")> Public Property publication_internal_comment As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `publication` (`publication_id`, `reference_id`, `external_db_id`, `author`, `title`, `source`, `years`, `publication_note`, `publication_internal_comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `publication` (`publication_id`, `reference_id`, `external_db_id`, `author`, `title`, `source`, `years`, `publication_note`, `publication_internal_comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `publication` (`publication_id`, `reference_id`, `external_db_id`, `author`, `title`, `source`, `years`, `publication_note`, `publication_internal_comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `publication` (`publication_id`, `reference_id`, `external_db_id`, `author`, `title`, `source`, `years`, `publication_note`, `publication_internal_comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `publication` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `publication` SET `publication_id`='{0}', `reference_id`='{1}', `external_db_id`='{2}', `author`='{3}', `title`='{4}', `source`='{5}', `years`='{6}', `publication_note`='{7}', `publication_internal_comment`='{8}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `publication` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `publication` (`publication_id`, `reference_id`, `external_db_id`, `author`, `title`, `source`, `years`, `publication_note`, `publication_internal_comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, publication_id, reference_id, external_db_id, author, title, source, years, publication_note, publication_internal_comment)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `publication` (`publication_id`, `reference_id`, `external_db_id`, `author`, `title`, `source`, `years`, `publication_note`, `publication_internal_comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, publication_id, reference_id, external_db_id, author, title, source, years, publication_note, publication_internal_comment)
        Else
        Return String.Format(INSERT_SQL, publication_id, reference_id, external_db_id, author, title, source, years, publication_note, publication_internal_comment)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{publication_id}', '{reference_id}', '{external_db_id}', '{author}', '{title}', '{source}', '{years}', '{publication_note}', '{publication_internal_comment}')"
        Else
            Return $"('{publication_id}', '{reference_id}', '{external_db_id}', '{author}', '{title}', '{source}', '{years}', '{publication_note}', '{publication_internal_comment}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `publication` (`publication_id`, `reference_id`, `external_db_id`, `author`, `title`, `source`, `years`, `publication_note`, `publication_internal_comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, publication_id, reference_id, external_db_id, author, title, source, years, publication_note, publication_internal_comment)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `publication` (`publication_id`, `reference_id`, `external_db_id`, `author`, `title`, `source`, `years`, `publication_note`, `publication_internal_comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, publication_id, reference_id, external_db_id, author, title, source, years, publication_note, publication_internal_comment)
        Else
        Return String.Format(REPLACE_SQL, publication_id, reference_id, external_db_id, author, title, source, years, publication_note, publication_internal_comment)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `publication` SET `publication_id`='{0}', `reference_id`='{1}', `external_db_id`='{2}', `author`='{3}', `title`='{4}', `source`='{5}', `years`='{6}', `publication_note`='{7}', `publication_internal_comment`='{8}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As publication
                         Return DirectCast(MyClass.MemberwiseClone, publication)
                     End Function
End Class


End Namespace
