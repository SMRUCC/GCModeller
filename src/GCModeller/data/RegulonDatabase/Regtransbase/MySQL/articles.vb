#Region "Microsoft.VisualBasic::9041f1d552d96c3a9a4ec680a3fca536, ..\GCModeller\data\RegulonDatabase\Regtransbase\MySQL\articles.vb"

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

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:07:30 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace Regtransbase.MySQL

''' <summary>
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
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("articles", Database:="dbregulation_update")>
Public Class articles: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("art_guid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property art_guid As Long
    <DatabaseField("pkg_guid"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property pkg_guid As Long
    <DatabaseField("title"), DataType(MySqlDbType.VarChar, "255")> Public Property title As String
    <DatabaseField("author"), DataType(MySqlDbType.VarChar, "255")> Public Property author As String
    <DatabaseField("pmid"), DataType(MySqlDbType.VarChar, "20")> Public Property pmid As String
    <DatabaseField("art_journal"), DataType(MySqlDbType.VarChar, "50")> Public Property art_journal As String
    <DatabaseField("art_year"), DataType(MySqlDbType.VarChar, "10")> Public Property art_year As String
    <DatabaseField("art_month"), DataType(MySqlDbType.VarChar, "10")> Public Property art_month As String
    <DatabaseField("art_volume"), DataType(MySqlDbType.VarChar, "10")> Public Property art_volume As String
    <DatabaseField("art_issue"), DataType(MySqlDbType.VarChar, "10")> Public Property art_issue As String
    <DatabaseField("art_pages"), DataType(MySqlDbType.VarChar, "20")> Public Property art_pages As String
    <DatabaseField("art_abstruct"), DataType(MySqlDbType.Blob)> Public Property art_abstruct As Byte()
    <DatabaseField("exp_num"), DataType(MySqlDbType.Int64, "11")> Public Property exp_num As Long
    <DatabaseField("art_state"), DataType(MySqlDbType.Int64, "11")> Public Property art_state As Long
    <DatabaseField("last_update"), NotNull, DataType(MySqlDbType.DateTime)> Public Property last_update As Date
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `articles` (`art_guid`, `pkg_guid`, `title`, `author`, `pmid`, `art_journal`, `art_year`, `art_month`, `art_volume`, `art_issue`, `art_pages`, `art_abstruct`, `exp_num`, `art_state`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `articles` (`art_guid`, `pkg_guid`, `title`, `author`, `pmid`, `art_journal`, `art_year`, `art_month`, `art_volume`, `art_issue`, `art_pages`, `art_abstruct`, `exp_num`, `art_state`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `articles` WHERE `art_guid` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `articles` SET `art_guid`='{0}', `pkg_guid`='{1}', `title`='{2}', `author`='{3}', `pmid`='{4}', `art_journal`='{5}', `art_year`='{6}', `art_month`='{7}', `art_volume`='{8}', `art_issue`='{9}', `art_pages`='{10}', `art_abstruct`='{11}', `exp_num`='{12}', `art_state`='{13}', `last_update`='{14}' WHERE `art_guid` = '{15}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, art_guid)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, art_guid, pkg_guid, title, author, pmid, art_journal, art_year, art_month, art_volume, art_issue, art_pages, art_abstruct, exp_num, art_state, DataType.ToMySqlDateTimeString(last_update))
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, art_guid, pkg_guid, title, author, pmid, art_journal, art_year, art_month, art_volume, art_issue, art_pages, art_abstruct, exp_num, art_state, DataType.ToMySqlDateTimeString(last_update))
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, art_guid, pkg_guid, title, author, pmid, art_journal, art_year, art_month, art_volume, art_issue, art_pages, art_abstruct, exp_num, art_state, DataType.ToMySqlDateTimeString(last_update), art_guid)
    End Function
#End Region
End Class


End Namespace
