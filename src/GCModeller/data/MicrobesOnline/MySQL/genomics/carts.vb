#Region "Microsoft.VisualBasic::6659c4fea2da6130031da981e7b62147, GCModeller\data\MicrobesOnline\MySQL\genomics\carts.vb"

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
    '     File Size: 3.52 KB


    ' Class carts
    ' 
    '     Properties: active, cartId, name, seqCount, seqData
    '                 time, userId
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
''' DROP TABLE IF EXISTS `carts`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `carts` (
'''   `cartId` int(10) unsigned NOT NULL AUTO_INCREMENT,
'''   `userId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `name` varchar(32) NOT NULL DEFAULT '',
'''   `seqData` longtext NOT NULL,
'''   `seqCount` int(10) unsigned NOT NULL DEFAULT '0',
'''   `time` int(10) unsigned NOT NULL DEFAULT '0',
'''   `active` int(1) unsigned NOT NULL DEFAULT '0',
'''   PRIMARY KEY (`cartId`),
'''   KEY `userId` (`userId`)
''' ) ENGINE=MyISAM AUTO_INCREMENT=9354 DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("carts")>
Public Class carts: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("cartId"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property cartId As Long
    <DatabaseField("userId"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property userId As Long
    <DatabaseField("name"), NotNull, DataType(MySqlDbType.VarChar, "32")> Public Property name As String
    <DatabaseField("seqData"), NotNull, DataType(MySqlDbType.Text)> Public Property seqData As String
    <DatabaseField("seqCount"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property seqCount As Long
    <DatabaseField("time"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property time As Long
    <DatabaseField("active"), NotNull, DataType(MySqlDbType.Int64, "1")> Public Property active As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `carts` (`userId`, `name`, `seqData`, `seqCount`, `time`, `active`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `carts` (`userId`, `name`, `seqData`, `seqCount`, `time`, `active`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `carts` WHERE `cartId` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `carts` SET `cartId`='{0}', `userId`='{1}', `name`='{2}', `seqData`='{3}', `seqCount`='{4}', `time`='{5}', `active`='{6}' WHERE `cartId` = '{7}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, cartId)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, userId, name, seqData, seqCount, time, active)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, userId, name, seqData, seqCount, time, active)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, cartId, userId, name, seqData, seqCount, time, active, cartId)
    End Function
#End Region
End Class


End Namespace
