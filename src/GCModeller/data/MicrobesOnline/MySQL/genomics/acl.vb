#Region "Microsoft.VisualBasic::cf64d48c9924ca094597635156cdee57, GCModeller\data\MicrobesOnline\MySQL\genomics\acl.vb"

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

    '   Total Lines: 70
    '    Code Lines: 39
    ' Comment Lines: 24
    '   Blank Lines: 7
    '     File Size: 3.88 KB


    ' Class acl
    ' 
    '     Properties: admin, read, requesterId, requesterType, resourceId
    '                 resourceType, write
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
''' DROP TABLE IF EXISTS `acl`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `acl` (
'''   `requesterId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `requesterType` enum('user','group') NOT NULL DEFAULT 'user',
'''   `resourceId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `resourceType` enum('cart','job','uarray','scaffold','proteomic','interaction','taxonomyId') DEFAULT NULL,
'''   `read` tinyint(1) NOT NULL DEFAULT '0',
'''   `write` tinyint(1) NOT NULL DEFAULT '0',
'''   `admin` tinyint(1) NOT NULL DEFAULT '0',
'''   KEY `requesterId` (`requesterId`,`resourceId`),
'''   KEY `resourceId_key` (`resourceId`,`resourceType`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("acl")>
Public Class acl: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("requesterId"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property requesterId As Long
    <DatabaseField("requesterType"), NotNull, DataType(MySqlDbType.String)> Public Property requesterType As String
    <DatabaseField("resourceId"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property resourceId As Long
    <DatabaseField("resourceType"), DataType(MySqlDbType.String)> Public Property resourceType As String
    <DatabaseField("read"), NotNull, DataType(MySqlDbType.Int64, "1")> Public Property read As Long
    <DatabaseField("write"), NotNull, DataType(MySqlDbType.Int64, "1")> Public Property write As Long
    <DatabaseField("admin"), NotNull, DataType(MySqlDbType.Int64, "1")> Public Property admin As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `acl` (`requesterId`, `requesterType`, `resourceId`, `resourceType`, `read`, `write`, `admin`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `acl` (`requesterId`, `requesterType`, `resourceId`, `resourceType`, `read`, `write`, `admin`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `acl` WHERE `requesterId`='{0}' and `resourceId`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `acl` SET `requesterId`='{0}', `requesterType`='{1}', `resourceId`='{2}', `resourceType`='{3}', `read`='{4}', `write`='{5}', `admin`='{6}' WHERE `requesterId`='{7}' and `resourceId`='{8}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, requesterId, resourceId)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, requesterId, requesterType, resourceId, resourceType, read, write, admin)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, requesterId, requesterType, resourceId, resourceType, read, write, admin)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, requesterId, requesterType, resourceId, resourceType, read, write, admin, requesterId, resourceId)
    End Function
#End Region
End Class


End Namespace
