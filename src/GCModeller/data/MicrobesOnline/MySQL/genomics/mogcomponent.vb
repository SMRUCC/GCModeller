#Region "Microsoft.VisualBasic::71a9e69cb3b8e04202b406b6f6872cfe, GCModeller\data\MicrobesOnline\MySQL\genomics\mogcomponent.vb"

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

    '   Total Lines: 68
    '    Code Lines: 38
    ' Comment Lines: 23
    '   Blank Lines: 7
    '     File Size: 3.42 KB


    ' Class mogcomponent
    ' 
    '     Properties: metric, mogId, nMembers, nMembersBest, ogId
    '                 treeId
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
''' DROP TABLE IF EXISTS `mogcomponent`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `mogcomponent` (
'''   `mogId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `treeId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `ogId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `metric` float NOT NULL DEFAULT '0',
'''   `nMembers` int(10) unsigned NOT NULL DEFAULT '0',
'''   `nMembersBest` int(10) unsigned NOT NULL DEFAULT '0',
'''   KEY `mogId` (`mogId`),
'''   KEY `treeId` (`treeId`,`ogId`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("mogcomponent")>
Public Class mogcomponent: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("mogId"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property mogId As Long
    <DatabaseField("treeId"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property treeId As Long
    <DatabaseField("ogId"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property ogId As Long
    <DatabaseField("metric"), NotNull, DataType(MySqlDbType.Double)> Public Property metric As Double
    <DatabaseField("nMembers"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property nMembers As Long
    <DatabaseField("nMembersBest"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property nMembersBest As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `mogcomponent` (`mogId`, `treeId`, `ogId`, `metric`, `nMembers`, `nMembersBest`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `mogcomponent` (`mogId`, `treeId`, `ogId`, `metric`, `nMembers`, `nMembersBest`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `mogcomponent` WHERE `mogId` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `mogcomponent` SET `mogId`='{0}', `treeId`='{1}', `ogId`='{2}', `metric`='{3}', `nMembers`='{4}', `nMembersBest`='{5}' WHERE `mogId` = '{6}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, mogId)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, mogId, treeId, ogId, metric, nMembers, nMembersBest)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, mogId, treeId, ogId, metric, nMembers, nMembersBest)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, mogId, treeId, ogId, metric, nMembers, nMembersBest, mogId)
    End Function
#End Region
End Class


End Namespace
