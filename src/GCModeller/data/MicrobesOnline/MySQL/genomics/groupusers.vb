#Region "Microsoft.VisualBasic::eae0279439a90d3e3745e7bf8ba7af3b, GCModeller\data\MicrobesOnline\MySQL\genomics\groupusers.vb"

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

    '   Total Lines: 64
    '    Code Lines: 36
    ' Comment Lines: 21
    '   Blank Lines: 7
    '     File Size: 2.90 KB


    ' Class groupusers
    ' 
    '     Properties: active, groupId, time, userId
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
''' DROP TABLE IF EXISTS `groupusers`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `groupusers` (
'''   `groupId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `userId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `active` tinyint(1) unsigned NOT NULL DEFAULT '0',
'''   `time` int(10) unsigned NOT NULL DEFAULT '0',
'''   KEY `groupId` (`groupId`),
'''   KEY `userId` (`userId`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("groupusers")>
Public Class groupusers: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("groupId"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property groupId As Long
    <DatabaseField("userId"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property userId As Long
    <DatabaseField("active"), NotNull, DataType(MySqlDbType.Int64, "1")> Public Property active As Long
    <DatabaseField("time"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property time As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `groupusers` (`groupId`, `userId`, `active`, `time`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `groupusers` (`groupId`, `userId`, `active`, `time`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `groupusers` WHERE `groupId` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `groupusers` SET `groupId`='{0}', `userId`='{1}', `active`='{2}', `time`='{3}' WHERE `groupId` = '{4}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, groupId)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, groupId, userId, active, time)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, groupId, userId, active, time)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, groupId, userId, active, time, groupId)
    End Function
#End Region
End Class


End Namespace
