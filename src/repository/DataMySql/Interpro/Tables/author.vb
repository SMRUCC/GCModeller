#Region "Microsoft.VisualBasic::6a2efb85ee601d0ae9d978df5e802b15, DataMySql\Interpro\Tables\author.vb"

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

    ' Class author
    ' 
    '     Properties: author_id, name, uppercase
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

REM  Dump @2018/5/23 13:13:37


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace Interpro.Tables

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `author`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `author` (
'''   `author_id` int(9) NOT NULL,
'''   `name` varchar(80) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `uppercase` varchar(80) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   PRIMARY KEY (`author_id`),
'''   UNIQUE KEY `ui_author$id$name` (`author_id`,`name`),
'''   UNIQUE KEY `uq_author$name` (`name`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("author", Database:="interpro", SchemaSQL:="
CREATE TABLE `author` (
  `author_id` int(9) NOT NULL,
  `name` varchar(80) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `uppercase` varchar(80) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  PRIMARY KEY (`author_id`),
  UNIQUE KEY `ui_author$id$name` (`author_id`,`name`),
  UNIQUE KEY `uq_author$name` (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class author: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("author_id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "9"), Column(Name:="author_id"), XmlAttribute> Public Property author_id As Long
    <DatabaseField("name"), NotNull, DataType(MySqlDbType.VarChar, "80"), Column(Name:="name")> Public Property name As String
    <DatabaseField("uppercase"), NotNull, DataType(MySqlDbType.VarChar, "80"), Column(Name:="uppercase")> Public Property uppercase As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `author` (`author_id`, `name`, `uppercase`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `author` (`author_id`, `name`, `uppercase`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `author` (`author_id`, `name`, `uppercase`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `author` (`author_id`, `name`, `uppercase`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `author` WHERE `author_id` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `author` SET `author_id`='{0}', `name`='{1}', `uppercase`='{2}' WHERE `author_id` = '{3}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `author` WHERE `author_id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, author_id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `author` (`author_id`, `name`, `uppercase`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, author_id, name, uppercase)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `author` (`author_id`, `name`, `uppercase`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, author_id, name, uppercase)
        Else
        Return String.Format(INSERT_SQL, author_id, name, uppercase)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{author_id}', '{name}', '{uppercase}')"
        Else
            Return $"('{author_id}', '{name}', '{uppercase}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `author` (`author_id`, `name`, `uppercase`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, author_id, name, uppercase)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `author` (`author_id`, `name`, `uppercase`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, author_id, name, uppercase)
        Else
        Return String.Format(REPLACE_SQL, author_id, name, uppercase)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `author` SET `author_id`='{0}', `name`='{1}', `uppercase`='{2}' WHERE `author_id` = '{3}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, author_id, name, uppercase, author_id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As author
                         Return DirectCast(MyClass.MemberwiseClone, author)
                     End Function
End Class


End Namespace
