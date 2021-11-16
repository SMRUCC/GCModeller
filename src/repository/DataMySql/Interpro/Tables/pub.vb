#Region "Microsoft.VisualBasic::9bbb6bd56bf36b5c19dd70cc0421fb9c, DataMySql\Interpro\Tables\pub.vb"

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

    ' Class pub
    ' 
    '     Properties: firstpage, isbn, issn, issue, lastpage
    '                 medline_id, pub_id, pub_type, pubmed_id, rawpages
    '                 title, url, volume, year
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
''' DROP TABLE IF EXISTS `pub`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `pub` (
'''   `pub_id` varchar(11) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `pub_type` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `medline_id` int(9) DEFAULT NULL,
'''   `issn` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   `isbn` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   `volume` varchar(5) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   `issue` varchar(5) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   `firstpage` int(6) DEFAULT NULL,
'''   `lastpage` int(6) DEFAULT NULL,
'''   `year` int(4) NOT NULL,
'''   `title` mediumtext CHARACTER SET latin1 COLLATE latin1_bin,
'''   `url` mediumtext CHARACTER SET latin1 COLLATE latin1_bin,
'''   `rawpages` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   `pubmed_id` bigint(10) DEFAULT NULL,
'''   PRIMARY KEY (`pub_id`),
'''   KEY `fk_pub$issn` (`issn`),
'''   CONSTRAINT `fk_pub$issn` FOREIGN KEY (`issn`) REFERENCES `journal` (`issn`) ON DELETE CASCADE ON UPDATE NO ACTION
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("pub", Database:="interpro", SchemaSQL:="
CREATE TABLE `pub` (
  `pub_id` varchar(11) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `pub_type` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `medline_id` int(9) DEFAULT NULL,
  `issn` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `isbn` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `volume` varchar(5) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `issue` varchar(5) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `firstpage` int(6) DEFAULT NULL,
  `lastpage` int(6) DEFAULT NULL,
  `year` int(4) NOT NULL,
  `title` mediumtext CHARACTER SET latin1 COLLATE latin1_bin,
  `url` mediumtext CHARACTER SET latin1 COLLATE latin1_bin,
  `rawpages` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `pubmed_id` bigint(10) DEFAULT NULL,
  PRIMARY KEY (`pub_id`),
  KEY `fk_pub$issn` (`issn`),
  CONSTRAINT `fk_pub$issn` FOREIGN KEY (`issn`) REFERENCES `journal` (`issn`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class pub: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("pub_id"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "11"), Column(Name:="pub_id"), XmlAttribute> Public Property pub_id As String
    <DatabaseField("pub_type"), NotNull, DataType(MySqlDbType.VarChar, "1"), Column(Name:="pub_type")> Public Property pub_type As String
    <DatabaseField("medline_id"), DataType(MySqlDbType.Int64, "9"), Column(Name:="medline_id")> Public Property medline_id As Long
    <DatabaseField("issn"), DataType(MySqlDbType.VarChar, "9"), Column(Name:="issn")> Public Property issn As String
    <DatabaseField("isbn"), DataType(MySqlDbType.VarChar, "10"), Column(Name:="isbn")> Public Property isbn As String
    <DatabaseField("volume"), DataType(MySqlDbType.VarChar, "5"), Column(Name:="volume")> Public Property volume As String
    <DatabaseField("issue"), DataType(MySqlDbType.VarChar, "5"), Column(Name:="issue")> Public Property issue As String
    <DatabaseField("firstpage"), DataType(MySqlDbType.Int64, "6"), Column(Name:="firstpage")> Public Property firstpage As Long
    <DatabaseField("lastpage"), DataType(MySqlDbType.Int64, "6"), Column(Name:="lastpage")> Public Property lastpage As Long
    <DatabaseField("year"), NotNull, DataType(MySqlDbType.Int64, "4"), Column(Name:="year")> Public Property year As Long
    <DatabaseField("title"), DataType(MySqlDbType.Text), Column(Name:="title")> Public Property title As String
    <DatabaseField("url"), DataType(MySqlDbType.Text), Column(Name:="url")> Public Property url As String
    <DatabaseField("rawpages"), DataType(MySqlDbType.VarChar, "30"), Column(Name:="rawpages")> Public Property rawpages As String
    <DatabaseField("pubmed_id"), DataType(MySqlDbType.Int64, "10"), Column(Name:="pubmed_id")> Public Property pubmed_id As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `pub` (`pub_id`, `pub_type`, `medline_id`, `issn`, `isbn`, `volume`, `issue`, `firstpage`, `lastpage`, `year`, `title`, `url`, `rawpages`, `pubmed_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `pub` (`pub_id`, `pub_type`, `medline_id`, `issn`, `isbn`, `volume`, `issue`, `firstpage`, `lastpage`, `year`, `title`, `url`, `rawpages`, `pubmed_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `pub` (`pub_id`, `pub_type`, `medline_id`, `issn`, `isbn`, `volume`, `issue`, `firstpage`, `lastpage`, `year`, `title`, `url`, `rawpages`, `pubmed_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `pub` (`pub_id`, `pub_type`, `medline_id`, `issn`, `isbn`, `volume`, `issue`, `firstpage`, `lastpage`, `year`, `title`, `url`, `rawpages`, `pubmed_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `pub` WHERE `pub_id` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `pub` SET `pub_id`='{0}', `pub_type`='{1}', `medline_id`='{2}', `issn`='{3}', `isbn`='{4}', `volume`='{5}', `issue`='{6}', `firstpage`='{7}', `lastpage`='{8}', `year`='{9}', `title`='{10}', `url`='{11}', `rawpages`='{12}', `pubmed_id`='{13}' WHERE `pub_id` = '{14}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `pub` WHERE `pub_id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, pub_id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `pub` (`pub_id`, `pub_type`, `medline_id`, `issn`, `isbn`, `volume`, `issue`, `firstpage`, `lastpage`, `year`, `title`, `url`, `rawpages`, `pubmed_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, pub_id, pub_type, medline_id, issn, isbn, volume, issue, firstpage, lastpage, year, title, url, rawpages, pubmed_id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `pub` (`pub_id`, `pub_type`, `medline_id`, `issn`, `isbn`, `volume`, `issue`, `firstpage`, `lastpage`, `year`, `title`, `url`, `rawpages`, `pubmed_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, pub_id, pub_type, medline_id, issn, isbn, volume, issue, firstpage, lastpage, year, title, url, rawpages, pubmed_id)
        Else
        Return String.Format(INSERT_SQL, pub_id, pub_type, medline_id, issn, isbn, volume, issue, firstpage, lastpage, year, title, url, rawpages, pubmed_id)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{pub_id}', '{pub_type}', '{medline_id}', '{issn}', '{isbn}', '{volume}', '{issue}', '{firstpage}', '{lastpage}', '{year}', '{title}', '{url}', '{rawpages}', '{pubmed_id}')"
        Else
            Return $"('{pub_id}', '{pub_type}', '{medline_id}', '{issn}', '{isbn}', '{volume}', '{issue}', '{firstpage}', '{lastpage}', '{year}', '{title}', '{url}', '{rawpages}', '{pubmed_id}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `pub` (`pub_id`, `pub_type`, `medline_id`, `issn`, `isbn`, `volume`, `issue`, `firstpage`, `lastpage`, `year`, `title`, `url`, `rawpages`, `pubmed_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, pub_id, pub_type, medline_id, issn, isbn, volume, issue, firstpage, lastpage, year, title, url, rawpages, pubmed_id)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `pub` (`pub_id`, `pub_type`, `medline_id`, `issn`, `isbn`, `volume`, `issue`, `firstpage`, `lastpage`, `year`, `title`, `url`, `rawpages`, `pubmed_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, pub_id, pub_type, medline_id, issn, isbn, volume, issue, firstpage, lastpage, year, title, url, rawpages, pubmed_id)
        Else
        Return String.Format(REPLACE_SQL, pub_id, pub_type, medline_id, issn, isbn, volume, issue, firstpage, lastpage, year, title, url, rawpages, pubmed_id)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `pub` SET `pub_id`='{0}', `pub_type`='{1}', `medline_id`='{2}', `issn`='{3}', `isbn`='{4}', `volume`='{5}', `issue`='{6}', `firstpage`='{7}', `lastpage`='{8}', `year`='{9}', `title`='{10}', `url`='{11}', `rawpages`='{12}', `pubmed_id`='{13}' WHERE `pub_id` = '{14}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, pub_id, pub_type, medline_id, issn, isbn, volume, issue, firstpage, lastpage, year, title, url, rawpages, pubmed_id, pub_id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As pub
                         Return DirectCast(MyClass.MemberwiseClone, pub)
                     End Function
End Class


End Namespace
