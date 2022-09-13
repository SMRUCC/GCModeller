#Region "Microsoft.VisualBasic::8f5989a6b3d2ca0d4891c25d89137f61, GCModeller\data\MicrobesOnline\MySQL\genomics\position.vb"

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
    '     File Size: 3.40 KB


    ' Class position
    ' 
    '     Properties: [end], begin, objectId, posId, scaffoldId
    '                 strand
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
''' DROP TABLE IF EXISTS `position`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `position` (
'''   `posId` int(10) unsigned NOT NULL AUTO_INCREMENT,
'''   `strand` enum('+','-') NOT NULL DEFAULT '+',
'''   `begin` int(10) unsigned DEFAULT NULL,
'''   `end` int(10) unsigned DEFAULT NULL,
'''   `scaffoldId` int(10) unsigned DEFAULT NULL,
'''   `objectId` int(2) unsigned DEFAULT '1',
'''   PRIMARY KEY (`posId`),
'''   KEY `Indx_Position_scaffoldId` (`scaffoldId`),
'''   KEY `objectId` (`objectId`),
'''   KEY `start` (`begin`),
'''   KEY `end` (`end`)
''' ) ENGINE=MyISAM AUTO_INCREMENT=44218375 DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("position")>
Public Class position: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("posId"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property posId As Long
    <DatabaseField("strand"), NotNull, DataType(MySqlDbType.String)> Public Property strand As String
    <DatabaseField("begin"), DataType(MySqlDbType.Int64, "10")> Public Property begin As Long
    <DatabaseField("end"), DataType(MySqlDbType.Int64, "10")> Public Property [end] As Long
    <DatabaseField("scaffoldId"), DataType(MySqlDbType.Int64, "10")> Public Property scaffoldId As Long
    <DatabaseField("objectId"), DataType(MySqlDbType.Int64, "2")> Public Property objectId As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `position` (`strand`, `begin`, `end`, `scaffoldId`, `objectId`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `position` (`strand`, `begin`, `end`, `scaffoldId`, `objectId`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `position` WHERE `posId` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `position` SET `posId`='{0}', `strand`='{1}', `begin`='{2}', `end`='{3}', `scaffoldId`='{4}', `objectId`='{5}' WHERE `posId` = '{6}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, posId)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, strand, begin, [end], scaffoldId, objectId)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, strand, begin, [end], scaffoldId, objectId)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, posId, strand, begin, [end], scaffoldId, objectId, posId)
    End Function
#End Region
End Class


End Namespace
