#Region "Microsoft.VisualBasic::a1e220a76b93ff8bcb23237ffc0aed6c, GCModeller\data\MicrobesOnline\MySQL\genomics\locus2tree.vb"

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

    '   Total Lines: 77
    '    Code Lines: 42
    ' Comment Lines: 28
    '   Blank Lines: 7
    '     File Size: 4.38 KB


    ' Class locus2tree
    ' 
    '     Properties: [end], aaTree, begin, locusId, nAligned
    '                 scaffoldId, score, taxonomyId, treeId, version
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
''' DROP TABLE IF EXISTS `locus2tree`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `locus2tree` (
'''   `treeId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `locusId` bigint(10) unsigned NOT NULL DEFAULT '0',
'''   `version` int(2) unsigned NOT NULL DEFAULT '0',
'''   `aaTree` tinyint(1) NOT NULL DEFAULT '0',
'''   `begin` int(10) unsigned NOT NULL DEFAULT '0',
'''   `end` int(10) unsigned NOT NULL DEFAULT '0',
'''   `nAligned` int(10) unsigned NOT NULL DEFAULT '0',
'''   `score` decimal(5,1) unsigned DEFAULT NULL,
'''   `scaffoldId` bigint(10) unsigned DEFAULT NULL,
'''   `taxonomyId` bigint(20) DEFAULT NULL,
'''   KEY `locusId` (`locusId`),
'''   KEY `treeId` (`treeId`),
'''   KEY `treelocus` (`treeId`,`locusId`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("locus2tree")>
Public Class locus2tree: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("treeId"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property treeId As Long
    <DatabaseField("locusId"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property locusId As Long
    <DatabaseField("version"), NotNull, DataType(MySqlDbType.Int64, "2")> Public Property version As Long
    <DatabaseField("aaTree"), NotNull, DataType(MySqlDbType.Int64, "1")> Public Property aaTree As Long
    <DatabaseField("begin"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property begin As Long
    <DatabaseField("end"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property [end] As Long
    <DatabaseField("nAligned"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property nAligned As Long
    <DatabaseField("score"), DataType(MySqlDbType.Decimal)> Public Property score As Decimal
    <DatabaseField("scaffoldId"), DataType(MySqlDbType.Int64, "10")> Public Property scaffoldId As Long
    <DatabaseField("taxonomyId"), DataType(MySqlDbType.Int64, "20")> Public Property taxonomyId As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `locus2tree` (`treeId`, `locusId`, `version`, `aaTree`, `begin`, `end`, `nAligned`, `score`, `scaffoldId`, `taxonomyId`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `locus2tree` (`treeId`, `locusId`, `version`, `aaTree`, `begin`, `end`, `nAligned`, `score`, `scaffoldId`, `taxonomyId`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `locus2tree` WHERE `locusId` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `locus2tree` SET `treeId`='{0}', `locusId`='{1}', `version`='{2}', `aaTree`='{3}', `begin`='{4}', `end`='{5}', `nAligned`='{6}', `score`='{7}', `scaffoldId`='{8}', `taxonomyId`='{9}' WHERE `locusId` = '{10}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, locusId)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, treeId, locusId, version, aaTree, begin, [end], nAligned, score, scaffoldId, taxonomyId)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, treeId, locusId, version, aaTree, begin, [end], nAligned, score, scaffoldId, taxonomyId)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, treeId, locusId, version, aaTree, begin, [end], nAligned, score, scaffoldId, taxonomyId, locusId)
    End Function
#End Region
End Class


End Namespace
