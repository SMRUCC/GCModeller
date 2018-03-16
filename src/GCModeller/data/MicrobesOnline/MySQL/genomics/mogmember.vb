#Region "Microsoft.VisualBasic::441c947d111688f3e612e1afb16cd5ea, data\MicrobesOnline\MySQL\genomics\mogmember.vb"

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

    ' Class mogmember
    ' 
    '     Properties: locusId, maxEnd, metric, minBegin, mogId
    '                 nAligned, ogId, taxonomyId, treeId, version
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
''' DROP TABLE IF EXISTS `mogmember`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `mogmember` (
'''   `mogId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `locusId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `version` int(2) unsigned NOT NULL DEFAULT '0',
'''   `minBegin` int(10) unsigned NOT NULL DEFAULT '0',
'''   `maxEnd` int(10) unsigned NOT NULL DEFAULT '0',
'''   `treeId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `ogId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `metric` float NOT NULL DEFAULT '0',
'''   `nAligned` int(10) unsigned DEFAULT NULL,
'''   `taxonomyId` int(10) DEFAULT NULL,
'''   UNIQUE KEY `locusId` (`locusId`,`version`),
'''   KEY `mogId` (`mogId`),
'''   KEY `taxonomyId` (`taxonomyId`),
'''   KEY `treeId` (`treeId`,`ogId`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("mogmember")>
Public Class mogmember: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("mogId"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property mogId As Long
    <DatabaseField("locusId"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property locusId As Long
    <DatabaseField("version"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "2")> Public Property version As Long
    <DatabaseField("minBegin"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property minBegin As Long
    <DatabaseField("maxEnd"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property maxEnd As Long
    <DatabaseField("treeId"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property treeId As Long
    <DatabaseField("ogId"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property ogId As Long
    <DatabaseField("metric"), NotNull, DataType(MySqlDbType.Double)> Public Property metric As Double
    <DatabaseField("nAligned"), DataType(MySqlDbType.Int64, "10")> Public Property nAligned As Long
    <DatabaseField("taxonomyId"), DataType(MySqlDbType.Int64, "10")> Public Property taxonomyId As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `mogmember` (`mogId`, `locusId`, `version`, `minBegin`, `maxEnd`, `treeId`, `ogId`, `metric`, `nAligned`, `taxonomyId`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `mogmember` (`mogId`, `locusId`, `version`, `minBegin`, `maxEnd`, `treeId`, `ogId`, `metric`, `nAligned`, `taxonomyId`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `mogmember` WHERE `locusId`='{0}' and `version`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `mogmember` SET `mogId`='{0}', `locusId`='{1}', `version`='{2}', `minBegin`='{3}', `maxEnd`='{4}', `treeId`='{5}', `ogId`='{6}', `metric`='{7}', `nAligned`='{8}', `taxonomyId`='{9}' WHERE `locusId`='{10}' and `version`='{11}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, locusId, version)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, mogId, locusId, version, minBegin, maxEnd, treeId, ogId, metric, nAligned, taxonomyId)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, mogId, locusId, version, minBegin, maxEnd, treeId, ogId, metric, nAligned, taxonomyId)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, mogId, locusId, version, minBegin, maxEnd, treeId, ogId, metric, nAligned, taxonomyId, locusId, version)
    End Function
#End Region
End Class


End Namespace
