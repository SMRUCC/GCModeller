#Region "Microsoft.VisualBasic::bba8ad24e57677fb104f24119cf0506e, data\ExternalDBSource\MetaCyc\bio_warehouse\citation.vb"

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

    ' Class citation
    ' 
    '     Properties: Authors, Citation, DataSetWID, Editor, Issue
    '                 Pages, PMID, Publication, Publisher, Title
    '                 URI, Volume, WID, Year
    ' 
    '     Function: GetDeleteSQL, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:02:47 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MetaCyc.MySQL

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `citation`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `citation` (
'''   `WID` bigint(20) NOT NULL,
'''   `Citation` text,
'''   `PMID` decimal(10,0) DEFAULT NULL,
'''   `Title` varchar(255) DEFAULT NULL,
'''   `Authors` varchar(255) DEFAULT NULL,
'''   `Publication` varchar(255) DEFAULT NULL,
'''   `Publisher` varchar(255) DEFAULT NULL,
'''   `Editor` varchar(255) DEFAULT NULL,
'''   `Year` varchar(255) DEFAULT NULL,
'''   `Volume` varchar(255) DEFAULT NULL,
'''   `Issue` varchar(255) DEFAULT NULL,
'''   `Pages` varchar(255) DEFAULT NULL,
'''   `URI` varchar(255) DEFAULT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `CITATION_PMID` (`PMID`),
'''   KEY `CITATION_CITATION` (`Citation`(20)),
'''   KEY `FK_Citation` (`DataSetWID`),
'''   CONSTRAINT `FK_Citation` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("citation", Database:="warehouse")>
Public Class citation: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property WID As Long
    <DatabaseField("Citation"), DataType(MySqlDbType.Text)> Public Property Citation As String
    <DatabaseField("PMID"), DataType(MySqlDbType.Decimal)> Public Property PMID As Decimal
    <DatabaseField("Title"), DataType(MySqlDbType.VarChar, "255")> Public Property Title As String
    <DatabaseField("Authors"), DataType(MySqlDbType.VarChar, "255")> Public Property Authors As String
    <DatabaseField("Publication"), DataType(MySqlDbType.VarChar, "255")> Public Property Publication As String
    <DatabaseField("Publisher"), DataType(MySqlDbType.VarChar, "255")> Public Property Publisher As String
    <DatabaseField("Editor"), DataType(MySqlDbType.VarChar, "255")> Public Property Editor As String
    <DatabaseField("Year"), DataType(MySqlDbType.VarChar, "255")> Public Property Year As String
    <DatabaseField("Volume"), DataType(MySqlDbType.VarChar, "255")> Public Property Volume As String
    <DatabaseField("Issue"), DataType(MySqlDbType.VarChar, "255")> Public Property Issue As String
    <DatabaseField("Pages"), DataType(MySqlDbType.VarChar, "255")> Public Property Pages As String
    <DatabaseField("URI"), DataType(MySqlDbType.VarChar, "255")> Public Property URI As String
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DataSetWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `citation` (`WID`, `Citation`, `PMID`, `Title`, `Authors`, `Publication`, `Publisher`, `Editor`, `Year`, `Volume`, `Issue`, `Pages`, `URI`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `citation` (`WID`, `Citation`, `PMID`, `Title`, `Authors`, `Publication`, `Publisher`, `Editor`, `Year`, `Volume`, `Issue`, `Pages`, `URI`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `citation` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `citation` SET `WID`='{0}', `Citation`='{1}', `PMID`='{2}', `Title`='{3}', `Authors`='{4}', `Publication`='{5}', `Publisher`='{6}', `Editor`='{7}', `Year`='{8}', `Volume`='{9}', `Issue`='{10}', `Pages`='{11}', `URI`='{12}', `DataSetWID`='{13}' WHERE `WID` = '{14}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, Citation, PMID, Title, Authors, Publication, Publisher, Editor, Year, Volume, Issue, Pages, URI, DataSetWID)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, Citation, PMID, Title, Authors, Publication, Publisher, Editor, Year, Volume, Issue, Pages, URI, DataSetWID)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, Citation, PMID, Title, Authors, Publication, Publisher, Editor, Year, Volume, Issue, Pages, URI, DataSetWID, WID)
    End Function
#End Region
End Class


End Namespace
