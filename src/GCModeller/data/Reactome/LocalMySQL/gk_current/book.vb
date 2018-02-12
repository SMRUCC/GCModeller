#Region "Microsoft.VisualBasic::98d7c314fbbcb286f01c3077caee3447, data\Reactome\LocalMySQL\gk_current\book.vb"

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
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @3/29/2017 9:40:27 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace LocalMySQL.Tables.gk_current

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `book`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `book` (
'''   `DB_ID` int(10) unsigned NOT NULL,
'''   `ISBN` text,
'''   `chapterTitle` text,
'''   `pages` text,
'''   `publisher` int(10) unsigned DEFAULT NULL,
'''   `publisher_class` varchar(64) DEFAULT NULL,
'''   `year` int(10) DEFAULT NULL,
'''   PRIMARY KEY (`DB_ID`),
'''   KEY `publisher` (`publisher`),
'''   KEY `year` (`year`),
'''   FULLTEXT KEY `ISBN` (`ISBN`),
'''   FULLTEXT KEY `chapterTitle` (`chapterTitle`),
'''   FULLTEXT KEY `pages` (`pages`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("book", Database:="gk_current", SchemaSQL:="
CREATE TABLE `book` (
  `DB_ID` int(10) unsigned NOT NULL,
  `ISBN` text,
  `chapterTitle` text,
  `pages` text,
  `publisher` int(10) unsigned DEFAULT NULL,
  `publisher_class` varchar(64) DEFAULT NULL,
  `year` int(10) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `publisher` (`publisher`),
  KEY `year` (`year`),
  FULLTEXT KEY `ISBN` (`ISBN`),
  FULLTEXT KEY `chapterTitle` (`chapterTitle`),
  FULLTEXT KEY `pages` (`pages`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class book: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property DB_ID As Long
    <DatabaseField("ISBN"), DataType(MySqlDbType.Text)> Public Property ISBN As String
    <DatabaseField("chapterTitle"), DataType(MySqlDbType.Text)> Public Property chapterTitle As String
    <DatabaseField("pages"), DataType(MySqlDbType.Text)> Public Property pages As String
    <DatabaseField("publisher"), DataType(MySqlDbType.Int64, "10")> Public Property publisher As Long
    <DatabaseField("publisher_class"), DataType(MySqlDbType.VarChar, "64")> Public Property publisher_class As String
    <DatabaseField("year"), DataType(MySqlDbType.Int64, "10")> Public Property year As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `book` (`DB_ID`, `ISBN`, `chapterTitle`, `pages`, `publisher`, `publisher_class`, `year`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `book` (`DB_ID`, `ISBN`, `chapterTitle`, `pages`, `publisher`, `publisher_class`, `year`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `book` WHERE `DB_ID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `book` SET `DB_ID`='{0}', `ISBN`='{1}', `chapterTitle`='{2}', `pages`='{3}', `publisher`='{4}', `publisher_class`='{5}', `year`='{6}' WHERE `DB_ID` = '{7}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `book` WHERE `DB_ID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `book` (`DB_ID`, `ISBN`, `chapterTitle`, `pages`, `publisher`, `publisher_class`, `year`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, ISBN, chapterTitle, pages, publisher, publisher_class, year)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{DB_ID}', '{ISBN}', '{chapterTitle}', '{pages}', '{publisher}', '{publisher_class}', '{year}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `book` (`DB_ID`, `ISBN`, `chapterTitle`, `pages`, `publisher`, `publisher_class`, `year`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, ISBN, chapterTitle, pages, publisher, publisher_class, year)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `book` SET `DB_ID`='{0}', `ISBN`='{1}', `chapterTitle`='{2}', `pages`='{3}', `publisher`='{4}', `publisher_class`='{5}', `year`='{6}' WHERE `DB_ID` = '{7}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, ISBN, chapterTitle, pages, publisher, publisher_class, year, DB_ID)
    End Function
#End Region
End Class


End Namespace
