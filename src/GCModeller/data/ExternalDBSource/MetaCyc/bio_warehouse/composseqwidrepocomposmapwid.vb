#Region "Microsoft.VisualBasic::d0b2d5f53a6c19eb6ee5683f3193ced3, ExternalDBSource\MetaCyc\bio_warehouse\composseqwidrepocomposmapwid.vb"

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

    ' Class composseqwidrepocomposmapwid
    ' 
    '     Properties: CompositeSequenceWID, ReporterCompositeMapWID
    ' 
    '     Function: GetDeleteSQL, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:02:47 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MetaCyc.MySQL

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `composseqwidrepocomposmapwid`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `composseqwidrepocomposmapwid` (
'''   `CompositeSequenceWID` bigint(20) NOT NULL,
'''   `ReporterCompositeMapWID` bigint(20) NOT NULL,
'''   KEY `FK_ComposSeqWIDRepoComposMap1` (`CompositeSequenceWID`),
'''   KEY `FK_ComposSeqWIDRepoComposMap2` (`ReporterCompositeMapWID`),
'''   CONSTRAINT `FK_ComposSeqWIDRepoComposMap1` FOREIGN KEY (`CompositeSequenceWID`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_ComposSeqWIDRepoComposMap2` FOREIGN KEY (`ReporterCompositeMapWID`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("composseqwidrepocomposmapwid", Database:="warehouse")>
Public Class composseqwidrepocomposmapwid: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("CompositeSequenceWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property CompositeSequenceWID As Long
    <DatabaseField("ReporterCompositeMapWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property ReporterCompositeMapWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `composseqwidrepocomposmapwid` (`CompositeSequenceWID`, `ReporterCompositeMapWID`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `composseqwidrepocomposmapwid` (`CompositeSequenceWID`, `ReporterCompositeMapWID`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `composseqwidrepocomposmapwid` WHERE `CompositeSequenceWID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `composseqwidrepocomposmapwid` SET `CompositeSequenceWID`='{0}', `ReporterCompositeMapWID`='{1}' WHERE `CompositeSequenceWID` = '{2}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, CompositeSequenceWID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, CompositeSequenceWID, ReporterCompositeMapWID)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, CompositeSequenceWID, ReporterCompositeMapWID)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, CompositeSequenceWID, ReporterCompositeMapWID, CompositeSequenceWID)
    End Function
#End Region
End Class


End Namespace
