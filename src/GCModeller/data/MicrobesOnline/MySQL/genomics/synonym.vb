#Region "Microsoft.VisualBasic::958977a487cf9dfc21d5119bf598df2f, GCModeller\data\MicrobesOnline\MySQL\genomics\synonym.vb"

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

    '   Total Lines: 71
    '    Code Lines: 38
    ' Comment Lines: 26
    '   Blank Lines: 7
    '     File Size: 3.45 KB


    ' Class synonym
    ' 
    '     Properties: created, locusId, name, source, type
    '                 version
    ' 
    '     Function: GetDeleteSQL, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:30:29 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MySQL.genomics

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `synonym`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `synonym` (
'''   `name` varchar(100) DEFAULT NULL,
'''   `locusId` int(10) unsigned DEFAULT NULL,
'''   `version` int(2) unsigned DEFAULT '1',
'''   `type` int(2) unsigned DEFAULT NULL,
'''   `created` date DEFAULT NULL,
'''   `source` varchar(50) DEFAULT NULL,
'''   KEY `orfId` (`locusId`),
'''   KEY `orfId_version` (`locusId`,`version`),
'''   KEY `name` (`name`),
'''   KEY `type` (`type`),
'''   KEY `locusId_type_version` (`locusId`,`type`,`version`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("synonym")>
Public Class synonym: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("name"), DataType(MySqlDbType.VarChar, "100")> Public Property name As String
    <DatabaseField("locusId"), PrimaryKey, DataType(MySqlDbType.Int64, "10")> Public Property locusId As Long
    <DatabaseField("version"), DataType(MySqlDbType.Int64, "2")> Public Property version As Long
    <DatabaseField("type"), DataType(MySqlDbType.Int64, "2")> Public Property type As Long
    <DatabaseField("created"), DataType(MySqlDbType.DateTime)> Public Property created As Date
    <DatabaseField("source"), DataType(MySqlDbType.VarChar, "50")> Public Property source As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `synonym` (`name`, `locusId`, `version`, `type`, `created`, `source`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `synonym` (`name`, `locusId`, `version`, `type`, `created`, `source`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `synonym` WHERE `locusId` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `synonym` SET `name`='{0}', `locusId`='{1}', `version`='{2}', `type`='{3}', `created`='{4}', `source`='{5}' WHERE `locusId` = '{6}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, locusId)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, name, locusId, version, type, DataType.ToMySqlDateTimeString(created), source)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, name, locusId, version, type, DataType.ToMySqlDateTimeString(created), source)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, name, locusId, version, type, DataType.ToMySqlDateTimeString(created), source, locusId)
    End Function
#End Region
End Class


End Namespace
