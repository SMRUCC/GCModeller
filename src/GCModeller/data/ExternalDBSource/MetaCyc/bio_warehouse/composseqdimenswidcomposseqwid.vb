#Region "Microsoft.VisualBasic::3fe7c2157d5d28928262ede3b9e442c5, ExternalDBSource\MetaCyc\bio_warehouse\composseqdimenswidcomposseqwid.vb"

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

    ' Class composseqdimenswidcomposseqwid
    ' 
    '     Properties: CompositeSequenceDimensionWID, CompositeSequenceWID
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
''' DROP TABLE IF EXISTS `composseqdimenswidcomposseqwid`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `composseqdimenswidcomposseqwid` (
'''   `CompositeSequenceDimensionWID` bigint(20) NOT NULL,
'''   `CompositeSequenceWID` bigint(20) NOT NULL,
'''   KEY `FK_ComposSeqDimensWIDComposS1` (`CompositeSequenceDimensionWID`),
'''   KEY `FK_ComposSeqDimensWIDComposS2` (`CompositeSequenceWID`),
'''   CONSTRAINT `FK_ComposSeqDimensWIDComposS1` FOREIGN KEY (`CompositeSequenceDimensionWID`) REFERENCES `designelementdimension` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_ComposSeqDimensWIDComposS2` FOREIGN KEY (`CompositeSequenceWID`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("composseqdimenswidcomposseqwid", Database:="warehouse")>
Public Class composseqdimenswidcomposseqwid: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("CompositeSequenceDimensionWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property CompositeSequenceDimensionWID As Long
    <DatabaseField("CompositeSequenceWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property CompositeSequenceWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `composseqdimenswidcomposseqwid` (`CompositeSequenceDimensionWID`, `CompositeSequenceWID`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `composseqdimenswidcomposseqwid` (`CompositeSequenceDimensionWID`, `CompositeSequenceWID`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `composseqdimenswidcomposseqwid` WHERE `CompositeSequenceDimensionWID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `composseqdimenswidcomposseqwid` SET `CompositeSequenceDimensionWID`='{0}', `CompositeSequenceWID`='{1}' WHERE `CompositeSequenceDimensionWID` = '{2}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, CompositeSequenceDimensionWID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, CompositeSequenceDimensionWID, CompositeSequenceWID)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, CompositeSequenceDimensionWID, CompositeSequenceWID)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, CompositeSequenceDimensionWID, CompositeSequenceWID, CompositeSequenceDimensionWID)
    End Function
#End Region
End Class


End Namespace
