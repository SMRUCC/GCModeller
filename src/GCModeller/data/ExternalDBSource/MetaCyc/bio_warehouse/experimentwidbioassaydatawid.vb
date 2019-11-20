#Region "Microsoft.VisualBasic::d71a7aa2c23cc0311b09e5e9a64e37b9, data\ExternalDBSource\MetaCyc\bio_warehouse\experimentwidbioassaydatawid.vb"

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

    ' Class experimentwidbioassaydatawid
    ' 
    '     Properties: BioAssayDataWID, ExperimentWID
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
''' DROP TABLE IF EXISTS `experimentwidbioassaydatawid`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `experimentwidbioassaydatawid` (
'''   `ExperimentWID` bigint(20) NOT NULL,
'''   `BioAssayDataWID` bigint(20) NOT NULL,
'''   KEY `FK_ExperimentWIDBioAssayData1` (`ExperimentWID`),
'''   KEY `FK_ExperimentWIDBioAssayData2` (`BioAssayDataWID`),
'''   CONSTRAINT `FK_ExperimentWIDBioAssayData1` FOREIGN KEY (`ExperimentWID`) REFERENCES `experiment` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_ExperimentWIDBioAssayData2` FOREIGN KEY (`BioAssayDataWID`) REFERENCES `bioassaydata` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("experimentwidbioassaydatawid", Database:="warehouse")>
Public Class experimentwidbioassaydatawid: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("ExperimentWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property ExperimentWID As Long
    <DatabaseField("BioAssayDataWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property BioAssayDataWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `experimentwidbioassaydatawid` (`ExperimentWID`, `BioAssayDataWID`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `experimentwidbioassaydatawid` (`ExperimentWID`, `BioAssayDataWID`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `experimentwidbioassaydatawid` WHERE `ExperimentWID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `experimentwidbioassaydatawid` SET `ExperimentWID`='{0}', `BioAssayDataWID`='{1}' WHERE `ExperimentWID` = '{2}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, ExperimentWID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, ExperimentWID, BioAssayDataWID)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, ExperimentWID, BioAssayDataWID)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, ExperimentWID, BioAssayDataWID, ExperimentWID)
    End Function
#End Region
End Class


End Namespace
