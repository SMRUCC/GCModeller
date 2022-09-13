#Region "Microsoft.VisualBasic::0d0f2f361f21bf1b6038e04dfd9fdb5f, GCModeller\data\MicrobesOnline\MySQL\genomics\tigrinfo.vb"

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

    '   Total Lines: 72
    '    Code Lines: 39
    ' Comment Lines: 26
    '   Blank Lines: 7
    '     File Size: 3.48 KB


    ' Class tigrinfo
    ' 
    '     Properties: definition, ec, geneSymbol, id, roleId
    '                 tigrId, type
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
''' DROP TABLE IF EXISTS `tigrinfo`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `tigrinfo` (
'''   `id` varchar(16) DEFAULT NULL,
'''   `tigrId` varchar(30) NOT NULL DEFAULT '',
'''   `type` varchar(32) DEFAULT NULL,
'''   `roleId` int(5) DEFAULT NULL,
'''   `geneSymbol` varchar(10) DEFAULT NULL,
'''   `ec` varchar(16) DEFAULT NULL,
'''   `definition` longtext,
'''   PRIMARY KEY (`tigrId`),
'''   KEY `type` (`type`),
'''   KEY `roleId` (`roleId`),
'''   KEY `ec` (`ec`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("tigrinfo")>
Public Class tigrinfo: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), DataType(MySqlDbType.VarChar, "16")> Public Property id As String
    <DatabaseField("tigrId"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "30")> Public Property tigrId As String
    <DatabaseField("type"), DataType(MySqlDbType.VarChar, "32")> Public Property type As String
    <DatabaseField("roleId"), DataType(MySqlDbType.Int64, "5")> Public Property roleId As Long
    <DatabaseField("geneSymbol"), DataType(MySqlDbType.VarChar, "10")> Public Property geneSymbol As String
    <DatabaseField("ec"), DataType(MySqlDbType.VarChar, "16")> Public Property ec As String
    <DatabaseField("definition"), DataType(MySqlDbType.Text)> Public Property definition As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `tigrinfo` (`id`, `tigrId`, `type`, `roleId`, `geneSymbol`, `ec`, `definition`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `tigrinfo` (`id`, `tigrId`, `type`, `roleId`, `geneSymbol`, `ec`, `definition`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `tigrinfo` WHERE `tigrId` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `tigrinfo` SET `id`='{0}', `tigrId`='{1}', `type`='{2}', `roleId`='{3}', `geneSymbol`='{4}', `ec`='{5}', `definition`='{6}' WHERE `tigrId` = '{7}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, tigrId)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, id, tigrId, type, roleId, geneSymbol, ec, definition)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, id, tigrId, type, roleId, geneSymbol, ec, definition)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, tigrId, type, roleId, geneSymbol, ec, definition, tigrId)
    End Function
#End Region
End Class


End Namespace
