#Region "Microsoft.VisualBasic::d3e8a9d4a03659d430dd86488f279513, ..\repository\DataMySql\Interpro\Tables\pub.vb"

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
''' 
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
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class pub: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("pub_id"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "11")> Public Property pub_id As String
    <DatabaseField("pub_type"), NotNull, DataType(MySqlDbType.VarChar, "1")> Public Property pub_type As String
    <DatabaseField("medline_id"), DataType(MySqlDbType.Int64, "9")> Public Property medline_id As Long
    <DatabaseField("issn"), DataType(MySqlDbType.VarChar, "9")> Public Property issn As String
    <DatabaseField("isbn"), DataType(MySqlDbType.VarChar, "10")> Public Property isbn As String
    <DatabaseField("volume"), DataType(MySqlDbType.VarChar, "5")> Public Property volume As String
    <DatabaseField("issue"), DataType(MySqlDbType.VarChar, "5")> Public Property issue As String
    <DatabaseField("firstpage"), DataType(MySqlDbType.Int64, "6")> Public Property firstpage As Long
    <DatabaseField("lastpage"), DataType(MySqlDbType.Int64, "6")> Public Property lastpage As Long
    <DatabaseField("year"), NotNull, DataType(MySqlDbType.Int64, "4")> Public Property year As Long
    <DatabaseField("title"), DataType(MySqlDbType.Text)> Public Property title As String
    <DatabaseField("url"), DataType(MySqlDbType.Text)> Public Property url As String
    <DatabaseField("rawpages"), DataType(MySqlDbType.VarChar, "30")> Public Property rawpages As String
    <DatabaseField("pubmed_id"), DataType(MySqlDbType.Int64, "10")> Public Property pubmed_id As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `pub` (`pub_id`, `pub_type`, `medline_id`, `issn`, `isbn`, `volume`, `issue`, `firstpage`, `lastpage`, `year`, `title`, `url`, `rawpages`, `pubmed_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `pub` (`pub_id`, `pub_type`, `medline_id`, `issn`, `isbn`, `volume`, `issue`, `firstpage`, `lastpage`, `year`, `title`, `url`, `rawpages`, `pubmed_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `pub` WHERE `pub_id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `pub` SET `pub_id`='{0}', `pub_type`='{1}', `medline_id`='{2}', `issn`='{3}', `isbn`='{4}', `volume`='{5}', `issue`='{6}', `firstpage`='{7}', `lastpage`='{8}', `year`='{9}', `title`='{10}', `url`='{11}', `rawpages`='{12}', `pubmed_id`='{13}' WHERE `pub_id` = '{14}';</SQL>
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
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{pub_id}', '{pub_type}', '{medline_id}', '{issn}', '{isbn}', '{volume}', '{issue}', '{firstpage}', '{lastpage}', '{year}', '{title}', '{url}', '{rawpages}', '{pubmed_id}')"
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
''' UPDATE `pub` SET `pub_id`='{0}', `pub_type`='{1}', `medline_id`='{2}', `issn`='{3}', `isbn`='{4}', `volume`='{5}', `issue`='{6}', `firstpage`='{7}', `lastpage`='{8}', `year`='{9}', `title`='{10}', `url`='{11}', `rawpages`='{12}', `pubmed_id`='{13}' WHERE `pub_id` = '{14}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, pub_id, pub_type, medline_id, issn, isbn, volume, issue, firstpage, lastpage, year, title, url, rawpages, pubmed_id, pub_id)
    End Function
#End Region
End Class


End Namespace

