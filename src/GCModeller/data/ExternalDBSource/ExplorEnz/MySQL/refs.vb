#Region "Microsoft.VisualBasic::a8072d27d8fc8742a3a4a410e13b8a15, data\ExternalDBSource\ExplorEnz\MySQL\refs.vb"

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

    ' Class refs
    ' 
    '     Properties: address, author, booktitle, cite_key, edition
    '                 editor, entry_title, erratum, first_page, journal
    '                 language, last_change, last_page, link, patent_yr
    '                 publisher, pubmed_id, title, type, volume
    '                 year
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

Namespace ExplorEnz.MySQL

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `refs`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `refs` (
'''   `cite_key` varchar(48) NOT NULL DEFAULT '',
'''   `type` varchar(7) DEFAULT NULL,
'''   `author` text,
'''   `title` text,
'''   `journal` varchar(72) DEFAULT NULL,
'''   `volume` varchar(20) DEFAULT NULL,
'''   `year` int(11) DEFAULT NULL,
'''   `first_page` varchar(12) DEFAULT NULL,
'''   `last_page` varchar(11) DEFAULT NULL,
'''   `pubmed_id` varchar(8) DEFAULT NULL,
'''   `language` varchar(127) DEFAULT NULL,
'''   `booktitle` varchar(255) DEFAULT NULL,
'''   `editor` varchar(128) DEFAULT NULL,
'''   `edition` char(3) DEFAULT NULL,
'''   `publisher` varchar(65) DEFAULT NULL,
'''   `address` varchar(65) DEFAULT NULL,
'''   `erratum` text,
'''   `entry_title` text,
'''   `patent_yr` int(11) DEFAULT NULL,
'''   `link` char(1) DEFAULT NULL,
'''   `last_change` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
'''   PRIMARY KEY (`cite_key`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' /*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;
''' 
''' /*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
''' /*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
''' /*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
''' /*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
''' /*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
''' /*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
''' /*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
''' 
''' -- Dump completed on 2015-12-03 19:58:29
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("refs", Database:="enzymed", SchemaSQL:="
CREATE TABLE `refs` (
  `cite_key` varchar(48) NOT NULL DEFAULT '',
  `type` varchar(7) DEFAULT NULL,
  `author` text,
  `title` text,
  `journal` varchar(72) DEFAULT NULL,
  `volume` varchar(20) DEFAULT NULL,
  `year` int(11) DEFAULT NULL,
  `first_page` varchar(12) DEFAULT NULL,
  `last_page` varchar(11) DEFAULT NULL,
  `pubmed_id` varchar(8) DEFAULT NULL,
  `language` varchar(127) DEFAULT NULL,
  `booktitle` varchar(255) DEFAULT NULL,
  `editor` varchar(128) DEFAULT NULL,
  `edition` char(3) DEFAULT NULL,
  `publisher` varchar(65) DEFAULT NULL,
  `address` varchar(65) DEFAULT NULL,
  `erratum` text,
  `entry_title` text,
  `patent_yr` int(11) DEFAULT NULL,
  `link` char(1) DEFAULT NULL,
  `last_change` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`cite_key`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class refs: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("cite_key"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "48"), Column(Name:="cite_key"), XmlAttribute> Public Property cite_key As String
    <DatabaseField("type"), DataType(MySqlDbType.VarChar, "7"), Column(Name:="type")> Public Property type As String
    <DatabaseField("author"), DataType(MySqlDbType.Text), Column(Name:="author")> Public Property author As String
    <DatabaseField("title"), DataType(MySqlDbType.Text), Column(Name:="title")> Public Property title As String
    <DatabaseField("journal"), DataType(MySqlDbType.VarChar, "72"), Column(Name:="journal")> Public Property journal As String
    <DatabaseField("volume"), DataType(MySqlDbType.VarChar, "20"), Column(Name:="volume")> Public Property volume As String
    <DatabaseField("year"), DataType(MySqlDbType.Int64, "11"), Column(Name:="year")> Public Property year As Long
    <DatabaseField("first_page"), DataType(MySqlDbType.VarChar, "12"), Column(Name:="first_page")> Public Property first_page As String
    <DatabaseField("last_page"), DataType(MySqlDbType.VarChar, "11"), Column(Name:="last_page")> Public Property last_page As String
    <DatabaseField("pubmed_id"), DataType(MySqlDbType.VarChar, "8"), Column(Name:="pubmed_id")> Public Property pubmed_id As String
    <DatabaseField("language"), DataType(MySqlDbType.VarChar, "127"), Column(Name:="language")> Public Property language As String
    <DatabaseField("booktitle"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="booktitle")> Public Property booktitle As String
    <DatabaseField("editor"), DataType(MySqlDbType.VarChar, "128"), Column(Name:="editor")> Public Property editor As String
    <DatabaseField("edition"), DataType(MySqlDbType.VarChar, "3"), Column(Name:="edition")> Public Property edition As String
    <DatabaseField("publisher"), DataType(MySqlDbType.VarChar, "65"), Column(Name:="publisher")> Public Property publisher As String
    <DatabaseField("address"), DataType(MySqlDbType.VarChar, "65"), Column(Name:="address")> Public Property address As String
    <DatabaseField("erratum"), DataType(MySqlDbType.Text), Column(Name:="erratum")> Public Property erratum As String
    <DatabaseField("entry_title"), DataType(MySqlDbType.Text), Column(Name:="entry_title")> Public Property entry_title As String
    <DatabaseField("patent_yr"), DataType(MySqlDbType.Int64, "11"), Column(Name:="patent_yr")> Public Property patent_yr As Long
    <DatabaseField("link"), DataType(MySqlDbType.VarChar, "1"), Column(Name:="link")> Public Property link As String
    <DatabaseField("last_change"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="last_change")> Public Property last_change As Date
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `refs` (`cite_key`, `type`, `author`, `title`, `journal`, `volume`, `year`, `first_page`, `last_page`, `pubmed_id`, `language`, `booktitle`, `editor`, `edition`, `publisher`, `address`, `erratum`, `entry_title`, `patent_yr`, `link`, `last_change`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `refs` (`cite_key`, `type`, `author`, `title`, `journal`, `volume`, `year`, `first_page`, `last_page`, `pubmed_id`, `language`, `booktitle`, `editor`, `edition`, `publisher`, `address`, `erratum`, `entry_title`, `patent_yr`, `link`, `last_change`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `refs` (`cite_key`, `type`, `author`, `title`, `journal`, `volume`, `year`, `first_page`, `last_page`, `pubmed_id`, `language`, `booktitle`, `editor`, `edition`, `publisher`, `address`, `erratum`, `entry_title`, `patent_yr`, `link`, `last_change`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `refs` (`cite_key`, `type`, `author`, `title`, `journal`, `volume`, `year`, `first_page`, `last_page`, `pubmed_id`, `language`, `booktitle`, `editor`, `edition`, `publisher`, `address`, `erratum`, `entry_title`, `patent_yr`, `link`, `last_change`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `refs` WHERE `cite_key` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `refs` SET `cite_key`='{0}', `type`='{1}', `author`='{2}', `title`='{3}', `journal`='{4}', `volume`='{5}', `year`='{6}', `first_page`='{7}', `last_page`='{8}', `pubmed_id`='{9}', `language`='{10}', `booktitle`='{11}', `editor`='{12}', `edition`='{13}', `publisher`='{14}', `address`='{15}', `erratum`='{16}', `entry_title`='{17}', `patent_yr`='{18}', `link`='{19}', `last_change`='{20}' WHERE `cite_key` = '{21}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `refs` WHERE `cite_key` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, cite_key)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `refs` (`cite_key`, `type`, `author`, `title`, `journal`, `volume`, `year`, `first_page`, `last_page`, `pubmed_id`, `language`, `booktitle`, `editor`, `edition`, `publisher`, `address`, `erratum`, `entry_title`, `patent_yr`, `link`, `last_change`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, cite_key, type, author, title, journal, volume, year, first_page, last_page, pubmed_id, language, booktitle, editor, edition, publisher, address, erratum, entry_title, patent_yr, link, MySqlScript.ToMySqlDateTimeString(last_change))
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `refs` (`cite_key`, `type`, `author`, `title`, `journal`, `volume`, `year`, `first_page`, `last_page`, `pubmed_id`, `language`, `booktitle`, `editor`, `edition`, `publisher`, `address`, `erratum`, `entry_title`, `patent_yr`, `link`, `last_change`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, cite_key, type, author, title, journal, volume, year, first_page, last_page, pubmed_id, language, booktitle, editor, edition, publisher, address, erratum, entry_title, patent_yr, link, MySqlScript.ToMySqlDateTimeString(last_change))
        Else
        Return String.Format(INSERT_SQL, cite_key, type, author, title, journal, volume, year, first_page, last_page, pubmed_id, language, booktitle, editor, edition, publisher, address, erratum, entry_title, patent_yr, link, MySqlScript.ToMySqlDateTimeString(last_change))
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{cite_key}', '{type}', '{author}', '{title}', '{journal}', '{volume}', '{year}', '{first_page}', '{last_page}', '{pubmed_id}', '{language}', '{booktitle}', '{editor}', '{edition}', '{publisher}', '{address}', '{erratum}', '{entry_title}', '{patent_yr}', '{link}', '{last_change}')"
        Else
            Return $"('{cite_key}', '{type}', '{author}', '{title}', '{journal}', '{volume}', '{year}', '{first_page}', '{last_page}', '{pubmed_id}', '{language}', '{booktitle}', '{editor}', '{edition}', '{publisher}', '{address}', '{erratum}', '{entry_title}', '{patent_yr}', '{link}', '{last_change}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `refs` (`cite_key`, `type`, `author`, `title`, `journal`, `volume`, `year`, `first_page`, `last_page`, `pubmed_id`, `language`, `booktitle`, `editor`, `edition`, `publisher`, `address`, `erratum`, `entry_title`, `patent_yr`, `link`, `last_change`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, cite_key, type, author, title, journal, volume, year, first_page, last_page, pubmed_id, language, booktitle, editor, edition, publisher, address, erratum, entry_title, patent_yr, link, MySqlScript.ToMySqlDateTimeString(last_change))
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `refs` (`cite_key`, `type`, `author`, `title`, `journal`, `volume`, `year`, `first_page`, `last_page`, `pubmed_id`, `language`, `booktitle`, `editor`, `edition`, `publisher`, `address`, `erratum`, `entry_title`, `patent_yr`, `link`, `last_change`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, cite_key, type, author, title, journal, volume, year, first_page, last_page, pubmed_id, language, booktitle, editor, edition, publisher, address, erratum, entry_title, patent_yr, link, MySqlScript.ToMySqlDateTimeString(last_change))
        Else
        Return String.Format(REPLACE_SQL, cite_key, type, author, title, journal, volume, year, first_page, last_page, pubmed_id, language, booktitle, editor, edition, publisher, address, erratum, entry_title, patent_yr, link, MySqlScript.ToMySqlDateTimeString(last_change))
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `refs` SET `cite_key`='{0}', `type`='{1}', `author`='{2}', `title`='{3}', `journal`='{4}', `volume`='{5}', `year`='{6}', `first_page`='{7}', `last_page`='{8}', `pubmed_id`='{9}', `language`='{10}', `booktitle`='{11}', `editor`='{12}', `edition`='{13}', `publisher`='{14}', `address`='{15}', `erratum`='{16}', `entry_title`='{17}', `patent_yr`='{18}', `link`='{19}', `last_change`='{20}' WHERE `cite_key` = '{21}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, cite_key, type, author, title, journal, volume, year, first_page, last_page, pubmed_id, language, booktitle, editor, edition, publisher, address, erratum, entry_title, patent_yr, link, MySqlScript.ToMySqlDateTimeString(last_change), cite_key)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As refs
                         Return DirectCast(MyClass.MemberwiseClone, refs)
                     End Function
End Class


End Namespace
