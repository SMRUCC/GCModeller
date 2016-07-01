#Region "Microsoft.VisualBasic::8c430e5f3ba99232a43798e8028f5706, ..\GCModeller\data\Reactome\LocalMySQL\gk_current\literaturereference.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

REM  Dump @12/3/2015 8:15:49 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace LocalMySQL.Tables.gk_current

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `literaturereference`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `literaturereference` (
'''   `DB_ID` int(10) unsigned NOT NULL,
'''   `journal` varchar(255) DEFAULT NULL,
'''   `pages` text,
'''   `pubMedIdentifier` int(10) DEFAULT NULL,
'''   `volume` int(10) DEFAULT NULL,
'''   `year` int(10) DEFAULT NULL,
'''   PRIMARY KEY (`DB_ID`),
'''   KEY `journal` (`journal`),
'''   KEY `pubMedIdentifier` (`pubMedIdentifier`),
'''   KEY `volume` (`volume`),
'''   KEY `year` (`year`),
'''   FULLTEXT KEY `journal_fulltext` (`journal`),
'''   FULLTEXT KEY `pages` (`pages`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("literaturereference", Database:="gk_current")>
Public Class literaturereference: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property DB_ID As Long
    <DatabaseField("journal"), DataType(MySqlDbType.VarChar, "255")> Public Property journal As String
    <DatabaseField("pages"), DataType(MySqlDbType.Text)> Public Property pages As String
    <DatabaseField("pubMedIdentifier"), DataType(MySqlDbType.Int64, "10")> Public Property pubMedIdentifier As Long
    <DatabaseField("volume"), DataType(MySqlDbType.Int64, "10")> Public Property volume As Long
    <DatabaseField("year"), DataType(MySqlDbType.Int64, "10")> Public Property year As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `literaturereference` (`DB_ID`, `journal`, `pages`, `pubMedIdentifier`, `volume`, `year`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `literaturereference` (`DB_ID`, `journal`, `pages`, `pubMedIdentifier`, `volume`, `year`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `literaturereference` WHERE `DB_ID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `literaturereference` SET `DB_ID`='{0}', `journal`='{1}', `pages`='{2}', `pubMedIdentifier`='{3}', `volume`='{4}', `year`='{5}' WHERE `DB_ID` = '{6}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, journal, pages, pubMedIdentifier, volume, year)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, journal, pages, pubMedIdentifier, volume, year)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, journal, pages, pubMedIdentifier, volume, year, DB_ID)
    End Function
#End Region
End Class


End Namespace

