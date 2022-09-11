#Region "Microsoft.VisualBasic::e20f2184f474265f288796c9138a5e7d, GCModeller\data\MicrobesOnline\MySQL\genomics\keggconf.vb"

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

    '   Total Lines: 67
    '    Code Lines: 37
    ' Comment Lines: 23
    '   Blank Lines: 7
    '     File Size: 3.19 KB


    ' Class keggconf
    ' 
    '     Properties: [object], coord, mapId, type, url
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
''' DROP TABLE IF EXISTS `keggconf`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `keggconf` (
'''   `mapId` varchar(20) DEFAULT NULL,
'''   `object` varchar(20) NOT NULL DEFAULT '',
'''   `type` int(1) NOT NULL DEFAULT '0',
'''   `url` varchar(255) NOT NULL DEFAULT '',
'''   `coord` varchar(100) DEFAULT NULL,
'''   UNIQUE KEY `combo` (`mapId`,`object`,`coord`),
'''   KEY `mapId` (`mapId`),
'''   KEY `object` (`object`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("keggconf")>
Public Class keggconf: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("mapId"), PrimaryKey, DataType(MySqlDbType.VarChar, "20")> Public Property mapId As String
    <DatabaseField("object"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "20")> Public Property [object] As String
    <DatabaseField("type"), NotNull, DataType(MySqlDbType.Int64, "1")> Public Property type As Long
    <DatabaseField("url"), NotNull, DataType(MySqlDbType.VarChar, "255")> Public Property url As String
    <DatabaseField("coord"), PrimaryKey, DataType(MySqlDbType.VarChar, "100")> Public Property coord As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `keggconf` (`mapId`, `object`, `type`, `url`, `coord`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `keggconf` (`mapId`, `object`, `type`, `url`, `coord`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `keggconf` WHERE `mapId`='{0}' and `object`='{1}' and `coord`='{2}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `keggconf` SET `mapId`='{0}', `object`='{1}', `type`='{2}', `url`='{3}', `coord`='{4}' WHERE `mapId`='{5}' and `object`='{6}' and `coord`='{7}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, mapId, [object], coord)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, mapId, [object], type, url, coord)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, mapId, [object], type, url, coord)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, mapId, [object], type, url, coord, mapId, [object], coord)
    End Function
#End Region
End Class


End Namespace
