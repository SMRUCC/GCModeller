#Region "Microsoft.VisualBasic::40f2f721e20d52c31c0cbd541c59325b, GCModeller\data\MicrobesOnline\MySQL\genomics\locus2ipr.vb"

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
    '    Code Lines: 35
    ' Comment Lines: 22
    '   Blank Lines: 7
    '     File Size: 2.80 KB


    ' Class locus2ipr
    ' 
    '     Properties: iprId, locusId, taxonomyId
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
''' DROP TABLE IF EXISTS `locus2ipr`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `locus2ipr` (
'''   `locusId` int(10) unsigned DEFAULT NULL,
'''   `iprId` varchar(9) NOT NULL DEFAULT '',
'''   `taxonomyId` int(10) DEFAULT NULL,
'''   UNIQUE KEY `orf2ipr` (`locusId`,`iprId`),
'''   KEY `orfId` (`locusId`),
'''   KEY `iprId` (`iprId`),
'''   KEY `taxonomyId` (`taxonomyId`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("locus2ipr")>
Public Class locus2ipr: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("locusId"), PrimaryKey, DataType(MySqlDbType.Int64, "10")> Public Property locusId As Long
    <DatabaseField("iprId"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "9")> Public Property iprId As String
    <DatabaseField("taxonomyId"), DataType(MySqlDbType.Int64, "10")> Public Property taxonomyId As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `locus2ipr` (`locusId`, `iprId`, `taxonomyId`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `locus2ipr` (`locusId`, `iprId`, `taxonomyId`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `locus2ipr` WHERE `locusId`='{0}' and `iprId`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `locus2ipr` SET `locusId`='{0}', `iprId`='{1}', `taxonomyId`='{2}' WHERE `locusId`='{3}' and `iprId`='{4}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, locusId, iprId)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, locusId, iprId, taxonomyId)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, locusId, iprId, taxonomyId)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, locusId, iprId, taxonomyId, locusId, iprId)
    End Function
#End Region
End Class


End Namespace
