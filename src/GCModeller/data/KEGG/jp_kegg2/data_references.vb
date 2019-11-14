#Region "Microsoft.VisualBasic::64c9b18631749f94a0d81457cf5e30fb, data\KEGG\jp_kegg2\data_references.vb"

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

    ' Class data_references
    ' 
    '     Properties: authors, journal, pmid, title, uid
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

REM  Dump @2018/5/23 13:16:34


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace mysql

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `data_references`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `data_references` (
'''   `uid` int(11) NOT NULL AUTO_INCREMENT,
'''   `pmid` int(11) NOT NULL,
'''   `journal` varchar(45) DEFAULT NULL,
'''   `title` varchar(45) NOT NULL,
'''   `authors` varchar(45) DEFAULT NULL,
'''   PRIMARY KEY (`uid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("data_references", Database:="jp_kegg2", SchemaSQL:="
CREATE TABLE `data_references` (
  `uid` int(11) NOT NULL AUTO_INCREMENT,
  `pmid` int(11) NOT NULL,
  `journal` varchar(45) DEFAULT NULL,
  `title` varchar(45) NOT NULL,
  `authors` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class data_references: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("uid"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="uid"), XmlAttribute> Public Property uid As Long
    <DatabaseField("pmid"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="pmid")> Public Property pmid As Long
    <DatabaseField("journal"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="journal")> Public Property journal As String
    <DatabaseField("title"), NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="title")> Public Property title As String
    <DatabaseField("authors"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="authors")> Public Property authors As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `data_references` (`pmid`, `journal`, `title`, `authors`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `data_references` (`uid`, `pmid`, `journal`, `title`, `authors`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `data_references` (`pmid`, `journal`, `title`, `authors`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `data_references` (`uid`, `pmid`, `journal`, `title`, `authors`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `data_references` WHERE `uid` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `data_references` SET `uid`='{0}', `pmid`='{1}', `journal`='{2}', `title`='{3}', `authors`='{4}' WHERE `uid` = '{5}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `data_references` WHERE `uid` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, uid)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `data_references` (`uid`, `pmid`, `journal`, `title`, `authors`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, pmid, journal, title, authors)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `data_references` (`uid`, `pmid`, `journal`, `title`, `authors`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, uid, pmid, journal, title, authors)
        Else
        Return String.Format(INSERT_SQL, pmid, journal, title, authors)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{uid}', '{pmid}', '{journal}', '{title}', '{authors}')"
        Else
            Return $"('{pmid}', '{journal}', '{title}', '{authors}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `data_references` (`uid`, `pmid`, `journal`, `title`, `authors`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, pmid, journal, title, authors)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `data_references` (`uid`, `pmid`, `journal`, `title`, `authors`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, uid, pmid, journal, title, authors)
        Else
        Return String.Format(REPLACE_SQL, pmid, journal, title, authors)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `data_references` SET `uid`='{0}', `pmid`='{1}', `journal`='{2}', `title`='{3}', `authors`='{4}' WHERE `uid` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, uid, pmid, journal, title, authors, uid)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As data_references
                         Return DirectCast(MyClass.MemberwiseClone, data_references)
                     End Function
End Class


End Namespace
