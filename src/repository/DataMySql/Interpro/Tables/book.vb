#Region "Microsoft.VisualBasic::2368442e96809056191e11ee8bce4e87, DataMySql\Interpro\Tables\book.vb"

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

    ' Class book
    ' 
    '     Properties: edition, isbn, publisher, pubplace, title
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
''' DROP TABLE IF EXISTS `book`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `book` (
'''   `isbn` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `title` mediumtext CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `edition` int(3) DEFAULT NULL,
'''   `publisher` varchar(200) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   `pubplace` varchar(50) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   PRIMARY KEY (`isbn`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("book", Database:="interpro", SchemaSQL:="
CREATE TABLE `book` (
  `isbn` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `title` mediumtext CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `edition` int(3) DEFAULT NULL,
  `publisher` varchar(200) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `pubplace` varchar(50) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  PRIMARY KEY (`isbn`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class book: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("isbn"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "10"), Column(Name:="isbn"), XmlAttribute> Public Property isbn As String
    <DatabaseField("title"), NotNull, DataType(MySqlDbType.Text), Column(Name:="title")> Public Property title As String
    <DatabaseField("edition"), DataType(MySqlDbType.Int64, "3"), Column(Name:="edition")> Public Property edition As Long
    <DatabaseField("publisher"), DataType(MySqlDbType.VarChar, "200"), Column(Name:="publisher")> Public Property publisher As String
    <DatabaseField("pubplace"), DataType(MySqlDbType.VarChar, "50"), Column(Name:="pubplace")> Public Property pubplace As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `book` (`isbn`, `title`, `edition`, `publisher`, `pubplace`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `book` (`isbn`, `title`, `edition`, `publisher`, `pubplace`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `book` (`isbn`, `title`, `edition`, `publisher`, `pubplace`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `book` (`isbn`, `title`, `edition`, `publisher`, `pubplace`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `book` WHERE `isbn` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `book` SET `isbn`='{0}', `title`='{1}', `edition`='{2}', `publisher`='{3}', `pubplace`='{4}' WHERE `isbn` = '{5}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `book` WHERE `isbn` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, isbn)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `book` (`isbn`, `title`, `edition`, `publisher`, `pubplace`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, isbn, title, edition, publisher, pubplace)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `book` (`isbn`, `title`, `edition`, `publisher`, `pubplace`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, isbn, title, edition, publisher, pubplace)
        Else
        Return String.Format(INSERT_SQL, isbn, title, edition, publisher, pubplace)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{isbn}', '{title}', '{edition}', '{publisher}', '{pubplace}')"
        Else
            Return $"('{isbn}', '{title}', '{edition}', '{publisher}', '{pubplace}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `book` (`isbn`, `title`, `edition`, `publisher`, `pubplace`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, isbn, title, edition, publisher, pubplace)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `book` (`isbn`, `title`, `edition`, `publisher`, `pubplace`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, isbn, title, edition, publisher, pubplace)
        Else
        Return String.Format(REPLACE_SQL, isbn, title, edition, publisher, pubplace)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `book` SET `isbn`='{0}', `title`='{1}', `edition`='{2}', `publisher`='{3}', `pubplace`='{4}' WHERE `isbn` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, isbn, title, edition, publisher, pubplace, isbn)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As book
                         Return DirectCast(MyClass.MemberwiseClone, book)
                     End Function
End Class


End Namespace
