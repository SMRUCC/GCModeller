#Region "Microsoft.VisualBasic::573bb8991ac5fd1853b1a7b6740837a2, GCModeller\data\MicrobesOnline\MySQL\glamm\annotatedmap.vb"

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

    '   Total Lines: 65
    '    Code Lines: 37
    ' Comment Lines: 21
    '   Blank Lines: 7
    '     File Size: 2.97 KB


    ' Class annotatedmap
    ' 
    '     Properties: icon, id, mapId, svg, title
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
''' DROP TABLE IF EXISTS `annotatedmap`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `annotatedmap` (
'''   `id` bigint(20) NOT NULL AUTO_INCREMENT,
'''   `icon` varchar(255) NOT NULL,
'''   `mapId` varchar(255) NOT NULL,
'''   `svg` varchar(255) NOT NULL,
'''   `title` varchar(255) NOT NULL,
'''   PRIMARY KEY (`id`)
''' ) ENGINE=MyISAM AUTO_INCREMENT=4 DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("annotatedmap", Database:="glamm")>
Public Class annotatedmap: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property id As Long
    <DatabaseField("icon"), NotNull, DataType(MySqlDbType.VarChar, "255")> Public Property icon As String
    <DatabaseField("mapId"), NotNull, DataType(MySqlDbType.VarChar, "255")> Public Property mapId As String
    <DatabaseField("svg"), NotNull, DataType(MySqlDbType.VarChar, "255")> Public Property svg As String
    <DatabaseField("title"), NotNull, DataType(MySqlDbType.VarChar, "255")> Public Property title As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `annotatedmap` (`icon`, `mapId`, `svg`, `title`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `annotatedmap` (`icon`, `mapId`, `svg`, `title`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `annotatedmap` WHERE `id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `annotatedmap` SET `id`='{0}', `icon`='{1}', `mapId`='{2}', `svg`='{3}', `title`='{4}' WHERE `id` = '{5}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, icon, mapId, svg, title)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, icon, mapId, svg, title)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, icon, mapId, svg, title, id)
    End Function
#End Region
End Class


End Namespace
