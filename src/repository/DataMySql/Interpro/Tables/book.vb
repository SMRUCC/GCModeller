#Region "Microsoft.VisualBasic::9af7fbc2f4d2f19581c0ac4f3735164b, ..\repository\DataMySql\Interpro\Tables\book.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @3/29/2017 10:21:21 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

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
''' 
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
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class book: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("isbn"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "10")> Public Property isbn As String
    <DatabaseField("title"), NotNull, DataType(MySqlDbType.Text)> Public Property title As String
    <DatabaseField("edition"), DataType(MySqlDbType.Int64, "3")> Public Property edition As Long
    <DatabaseField("publisher"), DataType(MySqlDbType.VarChar, "200")> Public Property publisher As String
    <DatabaseField("pubplace"), DataType(MySqlDbType.VarChar, "50")> Public Property pubplace As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `book` (`isbn`, `title`, `edition`, `publisher`, `pubplace`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `book` (`isbn`, `title`, `edition`, `publisher`, `pubplace`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `book` WHERE `isbn` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `book` SET `isbn`='{0}', `title`='{1}', `edition`='{2}', `publisher`='{3}', `pubplace`='{4}' WHERE `isbn` = '{5}';</SQL>
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
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{isbn}', '{title}', '{edition}', '{publisher}', '{pubplace}')"
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
''' UPDATE `book` SET `isbn`='{0}', `title`='{1}', `edition`='{2}', `publisher`='{3}', `pubplace`='{4}' WHERE `isbn` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, isbn, title, edition, publisher, pubplace, isbn)
    End Function
#End Region
End Class


End Namespace

