#Region "Microsoft.VisualBasic::8f9b46ad008bc374c1a46dd6334f5fb6, GCModeller\data\MicrobesOnline\MySQL\genomics\locuscount.vb"

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

    '   Total Lines: 62
    '    Code Lines: 35
    ' Comment Lines: 20
    '   Blank Lines: 7
    '     File Size: 2.76 KB


    ' Class locuscount
    ' 
    '     Properties: nGenes, scaffoldId, taxonomyId
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
''' DROP TABLE IF EXISTS `locuscount`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `locuscount` (
'''   `scaffoldId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `taxonomyId` int(10) NOT NULL DEFAULT '0',
'''   `nGenes` int(10) unsigned NOT NULL DEFAULT '0',
'''   PRIMARY KEY (`scaffoldId`),
'''   KEY `taxonomyId` (`taxonomyId`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("locuscount")>
Public Class locuscount: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("scaffoldId"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property scaffoldId As Long
    <DatabaseField("taxonomyId"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property taxonomyId As Long
    <DatabaseField("nGenes"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property nGenes As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `locuscount` (`scaffoldId`, `taxonomyId`, `nGenes`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `locuscount` (`scaffoldId`, `taxonomyId`, `nGenes`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `locuscount` WHERE `scaffoldId` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `locuscount` SET `scaffoldId`='{0}', `taxonomyId`='{1}', `nGenes`='{2}' WHERE `scaffoldId` = '{3}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, scaffoldId)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, scaffoldId, taxonomyId, nGenes)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, scaffoldId, taxonomyId, nGenes)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, scaffoldId, taxonomyId, nGenes, scaffoldId)
    End Function
#End Region
End Class


End Namespace
