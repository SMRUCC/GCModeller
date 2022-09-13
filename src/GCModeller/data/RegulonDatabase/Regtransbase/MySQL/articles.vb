#Region "Microsoft.VisualBasic::af38303887f38a1dccae60ff733ac482, GCModeller\data\RegulonDatabase\Regtransbase\MySQL\articles.vb"

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

    '   Total Lines: 196
    '    Code Lines: 103
    ' Comment Lines: 71
    '   Blank Lines: 22
    '     File Size: 12.46 KB


    ' Class articles
    ' 
    '     Properties: art_abstruct, art_guid, art_issue, art_journal, art_month
    '                 art_pages, art_state, art_volume, art_year, author
    '                 exp_num, last_update, pkg_guid, pmid, title
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

REM  Dump @2018/5/23 13:13:38


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace Regtransbase.MySQL

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `articles`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `articles` (
'''   `art_guid` int(11) NOT NULL DEFAULT '0',
'''   `pkg_guid` int(11) NOT NULL DEFAULT '0',
'''   `title` varchar(255) DEFAULT NULL,
'''   `author` varchar(255) DEFAULT NULL,
'''   `pmid` varchar(20) DEFAULT NULL,
'''   `art_journal` varchar(50) DEFAULT NULL,
'''   `art_year` varchar(10) DEFAULT NULL,
'''   `art_month` varchar(10) DEFAULT NULL,
'''   `art_volume` varchar(10) DEFAULT NULL,
'''   `art_issue` varchar(10) DEFAULT NULL,
'''   `art_pages` varchar(20) DEFAULT NULL,
'''   `art_abstruct` blob,
'''   `exp_num` int(11) DEFAULT NULL,
'''   `art_state` int(11) DEFAULT '0',
'''   `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
'''   PRIMARY KEY (`art_guid`),
'''   UNIQUE KEY `pmid_unique` (`pmid`),
'''   KEY `FK_articles-pkg_guid` (`pkg_guid`),
'''   CONSTRAINT `FK_articles-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("articles", Database:="dbregulation_update", SchemaSQL:="
CREATE TABLE `articles` (
  `art_guid` int(11) NOT NULL DEFAULT '0',
  `pkg_guid` int(11) NOT NULL DEFAULT '0',
  `title` varchar(255) DEFAULT NULL,
  `author` varchar(255) DEFAULT NULL,
  `pmid` varchar(20) DEFAULT NULL,
  `art_journal` varchar(50) DEFAULT NULL,
  `art_year` varchar(10) DEFAULT NULL,
  `art_month` varchar(10) DEFAULT NULL,
  `art_volume` varchar(10) DEFAULT NULL,
  `art_issue` varchar(10) DEFAULT NULL,
  `art_pages` varchar(20) DEFAULT NULL,
  `art_abstruct` blob,
  `exp_num` int(11) DEFAULT NULL,
  `art_state` int(11) DEFAULT '0',
  `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`art_guid`),
  UNIQUE KEY `pmid_unique` (`pmid`),
  KEY `FK_articles-pkg_guid` (`pkg_guid`),
  CONSTRAINT `FK_articles-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class articles: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("art_guid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="art_guid"), XmlAttribute> Public Property art_guid As Long
    <DatabaseField("pkg_guid"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="pkg_guid")> Public Property pkg_guid As Long
    <DatabaseField("title"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="title")> Public Property title As String
    <DatabaseField("author"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="author")> Public Property author As String
    <DatabaseField("pmid"), DataType(MySqlDbType.VarChar, "20"), Column(Name:="pmid")> Public Property pmid As String
    <DatabaseField("art_journal"), DataType(MySqlDbType.VarChar, "50"), Column(Name:="art_journal")> Public Property art_journal As String
    <DatabaseField("art_year"), DataType(MySqlDbType.VarChar, "10"), Column(Name:="art_year")> Public Property art_year As String
    <DatabaseField("art_month"), DataType(MySqlDbType.VarChar, "10"), Column(Name:="art_month")> Public Property art_month As String
    <DatabaseField("art_volume"), DataType(MySqlDbType.VarChar, "10"), Column(Name:="art_volume")> Public Property art_volume As String
    <DatabaseField("art_issue"), DataType(MySqlDbType.VarChar, "10"), Column(Name:="art_issue")> Public Property art_issue As String
    <DatabaseField("art_pages"), DataType(MySqlDbType.VarChar, "20"), Column(Name:="art_pages")> Public Property art_pages As String
    <DatabaseField("art_abstruct"), DataType(MySqlDbType.Blob), Column(Name:="art_abstruct")> Public Property art_abstruct As Byte()
    <DatabaseField("exp_num"), DataType(MySqlDbType.Int64, "11"), Column(Name:="exp_num")> Public Property exp_num As Long
    <DatabaseField("art_state"), DataType(MySqlDbType.Int64, "11"), Column(Name:="art_state")> Public Property art_state As Long
    <DatabaseField("last_update"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="last_update")> Public Property last_update As Date
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `articles` (`art_guid`, `pkg_guid`, `title`, `author`, `pmid`, `art_journal`, `art_year`, `art_month`, `art_volume`, `art_issue`, `art_pages`, `art_abstruct`, `exp_num`, `art_state`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `articles` (`art_guid`, `pkg_guid`, `title`, `author`, `pmid`, `art_journal`, `art_year`, `art_month`, `art_volume`, `art_issue`, `art_pages`, `art_abstruct`, `exp_num`, `art_state`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `articles` (`art_guid`, `pkg_guid`, `title`, `author`, `pmid`, `art_journal`, `art_year`, `art_month`, `art_volume`, `art_issue`, `art_pages`, `art_abstruct`, `exp_num`, `art_state`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `articles` (`art_guid`, `pkg_guid`, `title`, `author`, `pmid`, `art_journal`, `art_year`, `art_month`, `art_volume`, `art_issue`, `art_pages`, `art_abstruct`, `exp_num`, `art_state`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `articles` WHERE `art_guid` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `articles` SET `art_guid`='{0}', `pkg_guid`='{1}', `title`='{2}', `author`='{3}', `pmid`='{4}', `art_journal`='{5}', `art_year`='{6}', `art_month`='{7}', `art_volume`='{8}', `art_issue`='{9}', `art_pages`='{10}', `art_abstruct`='{11}', `exp_num`='{12}', `art_state`='{13}', `last_update`='{14}' WHERE `art_guid` = '{15}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `articles` WHERE `art_guid` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, art_guid)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `articles` (`art_guid`, `pkg_guid`, `title`, `author`, `pmid`, `art_journal`, `art_year`, `art_month`, `art_volume`, `art_issue`, `art_pages`, `art_abstruct`, `exp_num`, `art_state`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, art_guid, pkg_guid, title, author, pmid, art_journal, art_year, art_month, art_volume, art_issue, art_pages, art_abstruct, exp_num, art_state, MySqlScript.ToMySqlDateTimeString(last_update))
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `articles` (`art_guid`, `pkg_guid`, `title`, `author`, `pmid`, `art_journal`, `art_year`, `art_month`, `art_volume`, `art_issue`, `art_pages`, `art_abstruct`, `exp_num`, `art_state`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, art_guid, pkg_guid, title, author, pmid, art_journal, art_year, art_month, art_volume, art_issue, art_pages, art_abstruct, exp_num, art_state, MySqlScript.ToMySqlDateTimeString(last_update))
        Else
        Return String.Format(INSERT_SQL, art_guid, pkg_guid, title, author, pmid, art_journal, art_year, art_month, art_volume, art_issue, art_pages, art_abstruct, exp_num, art_state, MySqlScript.ToMySqlDateTimeString(last_update))
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{art_guid}', '{pkg_guid}', '{title}', '{author}', '{pmid}', '{art_journal}', '{art_year}', '{art_month}', '{art_volume}', '{art_issue}', '{art_pages}', '{art_abstruct}', '{exp_num}', '{art_state}', '{last_update}')"
        Else
            Return $"('{art_guid}', '{pkg_guid}', '{title}', '{author}', '{pmid}', '{art_journal}', '{art_year}', '{art_month}', '{art_volume}', '{art_issue}', '{art_pages}', '{art_abstruct}', '{exp_num}', '{art_state}', '{last_update}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `articles` (`art_guid`, `pkg_guid`, `title`, `author`, `pmid`, `art_journal`, `art_year`, `art_month`, `art_volume`, `art_issue`, `art_pages`, `art_abstruct`, `exp_num`, `art_state`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, art_guid, pkg_guid, title, author, pmid, art_journal, art_year, art_month, art_volume, art_issue, art_pages, art_abstruct, exp_num, art_state, MySqlScript.ToMySqlDateTimeString(last_update))
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `articles` (`art_guid`, `pkg_guid`, `title`, `author`, `pmid`, `art_journal`, `art_year`, `art_month`, `art_volume`, `art_issue`, `art_pages`, `art_abstruct`, `exp_num`, `art_state`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, art_guid, pkg_guid, title, author, pmid, art_journal, art_year, art_month, art_volume, art_issue, art_pages, art_abstruct, exp_num, art_state, MySqlScript.ToMySqlDateTimeString(last_update))
        Else
        Return String.Format(REPLACE_SQL, art_guid, pkg_guid, title, author, pmid, art_journal, art_year, art_month, art_volume, art_issue, art_pages, art_abstruct, exp_num, art_state, MySqlScript.ToMySqlDateTimeString(last_update))
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `articles` SET `art_guid`='{0}', `pkg_guid`='{1}', `title`='{2}', `author`='{3}', `pmid`='{4}', `art_journal`='{5}', `art_year`='{6}', `art_month`='{7}', `art_volume`='{8}', `art_issue`='{9}', `art_pages`='{10}', `art_abstruct`='{11}', `exp_num`='{12}', `art_state`='{13}', `last_update`='{14}' WHERE `art_guid` = '{15}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, art_guid, pkg_guid, title, author, pmid, art_journal, art_year, art_month, art_volume, art_issue, art_pages, art_abstruct, exp_num, art_state, MySqlScript.ToMySqlDateTimeString(last_update), art_guid)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As articles
                         Return DirectCast(MyClass.MemberwiseClone, articles)
                     End Function
End Class


End Namespace
