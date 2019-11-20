#Region "Microsoft.VisualBasic::1a22d0f2439eff957b12d8dcf66b6ecc, data\ExternalDBSource\ChEBI\Tables\comments.vb"

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

    ' Class comments
    ' 
    '     Properties: compound_id, created_on, datatype, datatype_id, id
    '                 text
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

REM  Dump @2018/5/23 13:13:39


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace ChEBI.Tables

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `comments`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `comments` (
'''   `id` int(11) NOT NULL,
'''   `compound_id` int(11) NOT NULL,
'''   `text` text NOT NULL,
'''   `created_on` datetime NOT NULL,
'''   `datatype` varchar(80) DEFAULT NULL,
'''   `datatype_id` int(11) NOT NULL,
'''   PRIMARY KEY (`id`),
'''   KEY `compound_id` (`compound_id`),
'''   CONSTRAINT `FK_COMMENTS_TO_COMPOUND` FOREIGN KEY (`compound_id`) REFERENCES `compounds` (`id`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("comments", Database:="chebi", SchemaSQL:="
CREATE TABLE `comments` (
  `id` int(11) NOT NULL,
  `compound_id` int(11) NOT NULL,
  `text` text NOT NULL,
  `created_on` datetime NOT NULL,
  `datatype` varchar(80) DEFAULT NULL,
  `datatype_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `compound_id` (`compound_id`),
  CONSTRAINT `FK_COMMENTS_TO_COMPOUND` FOREIGN KEY (`compound_id`) REFERENCES `compounds` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class comments: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="id"), XmlAttribute> Public Property id As Long
    <DatabaseField("compound_id"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="compound_id")> Public Property compound_id As Long
    <DatabaseField("text"), NotNull, DataType(MySqlDbType.Text), Column(Name:="text")> Public Property text As String
    <DatabaseField("created_on"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="created_on")> Public Property created_on As Date
    <DatabaseField("datatype"), DataType(MySqlDbType.VarChar, "80"), Column(Name:="datatype")> Public Property datatype As String
    <DatabaseField("datatype_id"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="datatype_id")> Public Property datatype_id As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `comments` (`id`, `compound_id`, `text`, `created_on`, `datatype`, `datatype_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `comments` (`id`, `compound_id`, `text`, `created_on`, `datatype`, `datatype_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `comments` (`id`, `compound_id`, `text`, `created_on`, `datatype`, `datatype_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `comments` (`id`, `compound_id`, `text`, `created_on`, `datatype`, `datatype_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `comments` WHERE `id` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `comments` SET `id`='{0}', `compound_id`='{1}', `text`='{2}', `created_on`='{3}', `datatype`='{4}', `datatype_id`='{5}' WHERE `id` = '{6}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `comments` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `comments` (`id`, `compound_id`, `text`, `created_on`, `datatype`, `datatype_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, id, compound_id, text, Oracle.LinuxCompatibility.MySQL.Scripting.Extensions.ToMySqlDateTimeString(created_on), datatype, datatype_id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `comments` (`id`, `compound_id`, `text`, `created_on`, `datatype`, `datatype_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, id, compound_id, text, Oracle.LinuxCompatibility.MySQL.Scripting.Extensions.ToMySqlDateTimeString(created_on), datatype, datatype_id)
        Else
        Return String.Format(INSERT_SQL, id, compound_id, text, Oracle.LinuxCompatibility.MySQL.Scripting.Extensions.ToMySqlDateTimeString(created_on), datatype, datatype_id)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{id}', '{compound_id}', '{text}', '{created_on}', '{datatype}', '{datatype_id}')"
        Else
            Return $"('{id}', '{compound_id}', '{text}', '{created_on}', '{datatype}', '{datatype_id}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `comments` (`id`, `compound_id`, `text`, `created_on`, `datatype`, `datatype_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, id, compound_id, text, Oracle.LinuxCompatibility.MySQL.Scripting.Extensions.ToMySqlDateTimeString(created_on), datatype, datatype_id)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `comments` (`id`, `compound_id`, `text`, `created_on`, `datatype`, `datatype_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, id, compound_id, text, Oracle.LinuxCompatibility.MySQL.Scripting.Extensions.ToMySqlDateTimeString(created_on), datatype, datatype_id)
        Else
        Return String.Format(REPLACE_SQL, id, compound_id, text, Oracle.LinuxCompatibility.MySQL.Scripting.Extensions.ToMySqlDateTimeString(created_on), datatype, datatype_id)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `comments` SET `id`='{0}', `compound_id`='{1}', `text`='{2}', `created_on`='{3}', `datatype`='{4}', `datatype_id`='{5}' WHERE `id` = '{6}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, compound_id, text, Oracle.LinuxCompatibility.MySQL.Scripting.Extensions.ToMySqlDateTimeString(created_on), datatype, datatype_id, id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As comments
                         Return DirectCast(MyClass.MemberwiseClone, comments)
                     End Function
End Class


End Namespace
