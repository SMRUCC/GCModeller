#Region "Microsoft.VisualBasic::d72393bbf6345ca1ca47f66f380d4f56, GCModeller\data\MicrobesOnline\MySQL\glamm\glammxref.vb"

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

    '   Total Lines: 74
    '    Code Lines: 40
    ' Comment Lines: 27
    '   Blank Lines: 7
    '     File Size: 4.00 KB


    ' Class glammxref
    ' 
    '     Properties: created, fromGuid, guid, priority, toXrefId
    '                 version, xrefDbName, xrefUrl
    ' 
    '     Function: GetDeleteSQL, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:22:12 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MySQL.glamm

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `glammxref`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `glammxref` (
'''   `guid` bigint(10) unsigned NOT NULL,
'''   `version` int(2) unsigned NOT NULL DEFAULT '1',
'''   `priority` tinyint(3) unsigned DEFAULT NULL,
'''   `created` date DEFAULT NULL,
'''   `fromGuid` bigint(10) unsigned NOT NULL,
'''   `toXrefId` varchar(50) NOT NULL DEFAULT '',
'''   `xrefDbName` varchar(50) NOT NULL DEFAULT '',
'''   `xrefUrl` text,
'''   PRIMARY KEY (`guid`),
'''   KEY `fromGuid` (`fromGuid`),
'''   KEY `toXrefId` (`toXrefId`),
'''   KEY `xrefDbName` (`xrefDbName`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("glammxref", Database:="glamm")>
Public Class glammxref: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("guid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property guid As Long
    <DatabaseField("version"), NotNull, DataType(MySqlDbType.Int64, "2")> Public Property version As Long
    <DatabaseField("priority"), DataType(MySqlDbType.Int64, "3")> Public Property priority As Long
    <DatabaseField("created"), DataType(MySqlDbType.DateTime)> Public Property created As Date
    <DatabaseField("fromGuid"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property fromGuid As Long
    <DatabaseField("toXrefId"), NotNull, DataType(MySqlDbType.VarChar, "50")> Public Property toXrefId As String
    <DatabaseField("xrefDbName"), NotNull, DataType(MySqlDbType.VarChar, "50")> Public Property xrefDbName As String
    <DatabaseField("xrefUrl"), DataType(MySqlDbType.Text)> Public Property xrefUrl As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `glammxref` (`guid`, `version`, `priority`, `created`, `fromGuid`, `toXrefId`, `xrefDbName`, `xrefUrl`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `glammxref` (`guid`, `version`, `priority`, `created`, `fromGuid`, `toXrefId`, `xrefDbName`, `xrefUrl`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `glammxref` WHERE `guid` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `glammxref` SET `guid`='{0}', `version`='{1}', `priority`='{2}', `created`='{3}', `fromGuid`='{4}', `toXrefId`='{5}', `xrefDbName`='{6}', `xrefUrl`='{7}' WHERE `guid` = '{8}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, guid)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, guid, version, priority, DataType.ToMySqlDateTimeString(created), fromGuid, toXrefId, xrefDbName, xrefUrl)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, guid, version, priority, DataType.ToMySqlDateTimeString(created), fromGuid, toXrefId, xrefDbName, xrefUrl)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, guid, version, priority, DataType.ToMySqlDateTimeString(created), fromGuid, toXrefId, xrefDbName, xrefUrl, guid)
    End Function
#End Region
End Class


End Namespace
