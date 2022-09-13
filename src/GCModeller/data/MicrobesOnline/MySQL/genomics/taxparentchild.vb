#Region "Microsoft.VisualBasic::f8f11a70327cdef64ca7482a74b3c44a, GCModeller\data\MicrobesOnline\MySQL\genomics\taxparentchild.vb"

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

    '   Total Lines: 63
    '    Code Lines: 35
    ' Comment Lines: 21
    '   Blank Lines: 7
    '     File Size: 2.87 KB


    ' Class taxparentchild
    ' 
    '     Properties: childId, nDistance, parentId
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
''' DROP TABLE IF EXISTS `taxparentchild`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `taxparentchild` (
'''   `parentId` int(10) NOT NULL DEFAULT '0',
'''   `childId` int(10) NOT NULL DEFAULT '0',
'''   `nDistance` int(10) unsigned NOT NULL DEFAULT '0',
'''   UNIQUE KEY `combined` (`parentId`,`childId`),
'''   KEY `parentId` (`parentId`),
'''   KEY `childId` (`childId`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("taxparentchild")>
Public Class taxparentchild: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("parentId"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property parentId As Long
    <DatabaseField("childId"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property childId As Long
    <DatabaseField("nDistance"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property nDistance As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `taxparentchild` (`parentId`, `childId`, `nDistance`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `taxparentchild` (`parentId`, `childId`, `nDistance`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `taxparentchild` WHERE `parentId`='{0}' and `childId`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `taxparentchild` SET `parentId`='{0}', `childId`='{1}', `nDistance`='{2}' WHERE `parentId`='{3}' and `childId`='{4}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, parentId, childId)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, parentId, childId, nDistance)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, parentId, childId, nDistance)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, parentId, childId, nDistance, parentId, childId)
    End Function
#End Region
End Class


End Namespace
