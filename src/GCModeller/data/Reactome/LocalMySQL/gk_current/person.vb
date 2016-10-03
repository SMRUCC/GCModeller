#Region "Microsoft.VisualBasic::b159bf5d9a7c64169e310d0a04450732, ..\GCModeller\data\Reactome\LocalMySQL\gk_current\person.vb"

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

REM  Dump @12/3/2015 8:15:49 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace LocalMySQL.Tables.gk_current

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `person`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `person` (
'''   `DB_ID` int(10) unsigned NOT NULL,
'''   `eMailAddress` varchar(255) DEFAULT NULL,
'''   `firstname` text,
'''   `initial` varchar(10) DEFAULT NULL,
'''   `project` text,
'''   `surname` varchar(255) DEFAULT NULL,
'''   `url` text,
'''   PRIMARY KEY (`DB_ID`),
'''   KEY `eMailAddress` (`eMailAddress`),
'''   KEY `initial` (`initial`),
'''   KEY `surname` (`surname`),
'''   FULLTEXT KEY `eMailAddress_fulltext` (`eMailAddress`),
'''   FULLTEXT KEY `firstname` (`firstname`),
'''   FULLTEXT KEY `initial_fulltext` (`initial`),
'''   FULLTEXT KEY `project` (`project`),
'''   FULLTEXT KEY `surname_fulltext` (`surname`),
'''   FULLTEXT KEY `url` (`url`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("person", Database:="gk_current")>
Public Class person: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property DB_ID As Long
    <DatabaseField("eMailAddress"), DataType(MySqlDbType.VarChar, "255")> Public Property eMailAddress As String
    <DatabaseField("firstname"), DataType(MySqlDbType.Text)> Public Property firstname As String
    <DatabaseField("initial"), DataType(MySqlDbType.VarChar, "10")> Public Property initial As String
    <DatabaseField("project"), DataType(MySqlDbType.Text)> Public Property project As String
    <DatabaseField("surname"), DataType(MySqlDbType.VarChar, "255")> Public Property surname As String
    <DatabaseField("url"), DataType(MySqlDbType.Text)> Public Property url As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `person` (`DB_ID`, `eMailAddress`, `firstname`, `initial`, `project`, `surname`, `url`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `person` (`DB_ID`, `eMailAddress`, `firstname`, `initial`, `project`, `surname`, `url`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `person` WHERE `DB_ID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `person` SET `DB_ID`='{0}', `eMailAddress`='{1}', `firstname`='{2}', `initial`='{3}', `project`='{4}', `surname`='{5}', `url`='{6}' WHERE `DB_ID` = '{7}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, eMailAddress, firstname, initial, project, surname, url)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, eMailAddress, firstname, initial, project, surname, url)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, eMailAddress, firstname, initial, project, surname, url, DB_ID)
    End Function
#End Region
End Class


End Namespace
