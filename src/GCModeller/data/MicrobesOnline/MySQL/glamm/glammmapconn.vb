#Region "Microsoft.VisualBasic::4bde54542bae325f5a7dc5cf6423b53d, data\MicrobesOnline\MySQL\glamm\glammmapconn.vb"

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

    ' Class glammmapconn
    ' 
    '     Properties: cpd0ExtId, cpd0SvgId, cpd1ExtId, cpd1SvgId, created
    '                 guid, mapTitle, priority, rxnExtId, rxnSvgId
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

REM  Dump @12/3/2015 8:22:12 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MySQL.glamm

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `glammmapconn`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `glammmapconn` (
'''   `guid` bigint(10) unsigned NOT NULL,
'''   `version` int(2) unsigned NOT NULL DEFAULT '1',
'''   `priority` tinyint(3) unsigned DEFAULT NULL,
'''   `created` date DEFAULT NULL,
'''   `mapTitle` text NOT NULL,
'''   `cpd0ExtId` varchar(50) NOT NULL DEFAULT '',
'''   `cpd0SvgId` varchar(50) NOT NULL DEFAULT '',
'''   `cpd1ExtId` varchar(50) NOT NULL DEFAULT '',
'''   `cpd1SvgId` varchar(50) NOT NULL DEFAULT '',
'''   `rxnExtId` varchar(50) NOT NULL DEFAULT '',
'''   `rxnSvgId` varchar(50) NOT NULL DEFAULT '',
'''   PRIMARY KEY (`guid`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("glammmapconn", Database:="glamm")>
Public Class glammmapconn: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("guid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property guid As Long
    <DatabaseField("version"), NotNull, DataType(MySqlDbType.Int64, "2")> Public Property version As Long
    <DatabaseField("priority"), DataType(MySqlDbType.Int64, "3")> Public Property priority As Long
    <DatabaseField("created"), DataType(MySqlDbType.DateTime)> Public Property created As Date
    <DatabaseField("mapTitle"), NotNull, DataType(MySqlDbType.Text)> Public Property mapTitle As String
    <DatabaseField("cpd0ExtId"), NotNull, DataType(MySqlDbType.VarChar, "50")> Public Property cpd0ExtId As String
    <DatabaseField("cpd0SvgId"), NotNull, DataType(MySqlDbType.VarChar, "50")> Public Property cpd0SvgId As String
    <DatabaseField("cpd1ExtId"), NotNull, DataType(MySqlDbType.VarChar, "50")> Public Property cpd1ExtId As String
    <DatabaseField("cpd1SvgId"), NotNull, DataType(MySqlDbType.VarChar, "50")> Public Property cpd1SvgId As String
    <DatabaseField("rxnExtId"), NotNull, DataType(MySqlDbType.VarChar, "50")> Public Property rxnExtId As String
    <DatabaseField("rxnSvgId"), NotNull, DataType(MySqlDbType.VarChar, "50")> Public Property rxnSvgId As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `glammmapconn` (`guid`, `version`, `priority`, `created`, `mapTitle`, `cpd0ExtId`, `cpd0SvgId`, `cpd1ExtId`, `cpd1SvgId`, `rxnExtId`, `rxnSvgId`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `glammmapconn` (`guid`, `version`, `priority`, `created`, `mapTitle`, `cpd0ExtId`, `cpd0SvgId`, `cpd1ExtId`, `cpd1SvgId`, `rxnExtId`, `rxnSvgId`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `glammmapconn` WHERE `guid` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `glammmapconn` SET `guid`='{0}', `version`='{1}', `priority`='{2}', `created`='{3}', `mapTitle`='{4}', `cpd0ExtId`='{5}', `cpd0SvgId`='{6}', `cpd1ExtId`='{7}', `cpd1SvgId`='{8}', `rxnExtId`='{9}', `rxnSvgId`='{10}' WHERE `guid` = '{11}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, guid)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, guid, version, priority, DataType.ToMySqlDateTimeString(created), mapTitle, cpd0ExtId, cpd0SvgId, cpd1ExtId, cpd1SvgId, rxnExtId, rxnSvgId)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, guid, version, priority, DataType.ToMySqlDateTimeString(created), mapTitle, cpd0ExtId, cpd0SvgId, cpd1ExtId, cpd1SvgId, rxnExtId, rxnSvgId)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, guid, version, priority, DataType.ToMySqlDateTimeString(created), mapTitle, cpd0ExtId, cpd0SvgId, cpd1ExtId, cpd1SvgId, rxnExtId, rxnSvgId, guid)
    End Function
#End Region
End Class


End Namespace
