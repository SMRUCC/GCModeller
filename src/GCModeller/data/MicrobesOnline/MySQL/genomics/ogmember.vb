#Region "Microsoft.VisualBasic::bf190fb21c18666c780e3620754b87f1, GCModeller\data\MicrobesOnline\MySQL\genomics\ogmember.vb"

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

    '   Total Lines: 79
    '    Code Lines: 43
    ' Comment Lines: 29
    '   Blank Lines: 7
    '     File Size: 4.74 KB


    ' Class ogmember
    ' 
    '     Properties: [end], aaLength, begin, locusId, nAligned
    '                 nMemberThisGenome, ogId, score, taxonomyId, treeId
    '                 version
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
''' DROP TABLE IF EXISTS `ogmember`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `ogmember` (
'''   `treeId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `ogId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `locusId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `version` int(2) unsigned NOT NULL DEFAULT '0',
'''   `begin` int(5) unsigned NOT NULL DEFAULT '0',
'''   `end` int(5) unsigned NOT NULL DEFAULT '0',
'''   `taxonomyId` int(10) NOT NULL DEFAULT '0',
'''   `nMemberThisGenome` int(10) unsigned NOT NULL DEFAULT '0',
'''   `aaLength` int(10) unsigned NOT NULL DEFAULT '0',
'''   `nAligned` int(10) unsigned DEFAULT NULL,
'''   `score` decimal(5,1) unsigned DEFAULT NULL,
'''   KEY `treeId` (`treeId`,`ogId`),
'''   KEY `locusId_2` (`locusId`,`version`),
'''   KEY `treelocus` (`treeId`,`locusId`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("ogmember")>
Public Class ogmember: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("treeId"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property treeId As Long
    <DatabaseField("ogId"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property ogId As Long
    <DatabaseField("locusId"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property locusId As Long
    <DatabaseField("version"), NotNull, DataType(MySqlDbType.Int64, "2")> Public Property version As Long
    <DatabaseField("begin"), NotNull, DataType(MySqlDbType.Int64, "5")> Public Property begin As Long
    <DatabaseField("end"), NotNull, DataType(MySqlDbType.Int64, "5")> Public Property [end] As Long
    <DatabaseField("taxonomyId"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property taxonomyId As Long
    <DatabaseField("nMemberThisGenome"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property nMemberThisGenome As Long
    <DatabaseField("aaLength"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property aaLength As Long
    <DatabaseField("nAligned"), DataType(MySqlDbType.Int64, "10")> Public Property nAligned As Long
    <DatabaseField("score"), DataType(MySqlDbType.Decimal)> Public Property score As Decimal
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `ogmember` (`treeId`, `ogId`, `locusId`, `version`, `begin`, `end`, `taxonomyId`, `nMemberThisGenome`, `aaLength`, `nAligned`, `score`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `ogmember` (`treeId`, `ogId`, `locusId`, `version`, `begin`, `end`, `taxonomyId`, `nMemberThisGenome`, `aaLength`, `nAligned`, `score`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `ogmember` WHERE `treeId`='{0}' and `ogId`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `ogmember` SET `treeId`='{0}', `ogId`='{1}', `locusId`='{2}', `version`='{3}', `begin`='{4}', `end`='{5}', `taxonomyId`='{6}', `nMemberThisGenome`='{7}', `aaLength`='{8}', `nAligned`='{9}', `score`='{10}' WHERE `treeId`='{11}' and `ogId`='{12}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, treeId, ogId)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, treeId, ogId, locusId, version, begin, [end], taxonomyId, nMemberThisGenome, aaLength, nAligned, score)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, treeId, ogId, locusId, version, begin, [end], taxonomyId, nMemberThisGenome, aaLength, nAligned, score)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, treeId, ogId, locusId, version, begin, [end], taxonomyId, nMemberThisGenome, aaLength, nAligned, score, treeId, ogId)
    End Function
#End Region
End Class


End Namespace
