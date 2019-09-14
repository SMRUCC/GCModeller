#Region "Microsoft.VisualBasic::5237c20a2a73cd9f74049d32c2e74c97, ExternalDBSource\MetaCyc\bio_warehouse\zonedefect.vb"

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

    ' Class zonedefect
    ' 
    '     Properties: ArrayManufactureDeviation, DataSetWID, WID, Zone, ZoneDefect_DefectType
    '                 ZoneDefect_PositionDelta
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
''' DROP TABLE IF EXISTS `zonedefect`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `zonedefect` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `ArrayManufactureDeviation` bigint(20) DEFAULT NULL,
'''   `ZoneDefect_DefectType` bigint(20) DEFAULT NULL,
'''   `ZoneDefect_PositionDelta` bigint(20) DEFAULT NULL,
'''   `Zone` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_ZoneDefect1` (`DataSetWID`),
'''   KEY `FK_ZoneDefect2` (`ArrayManufactureDeviation`),
'''   KEY `FK_ZoneDefect3` (`ZoneDefect_DefectType`),
'''   KEY `FK_ZoneDefect4` (`ZoneDefect_PositionDelta`),
'''   KEY `FK_ZoneDefect5` (`Zone`),
'''   CONSTRAINT `FK_ZoneDefect1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_ZoneDefect2` FOREIGN KEY (`ArrayManufactureDeviation`) REFERENCES `arraymanufacturedeviation` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_ZoneDefect3` FOREIGN KEY (`ZoneDefect_DefectType`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_ZoneDefect4` FOREIGN KEY (`ZoneDefect_PositionDelta`) REFERENCES `positiondelta` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_ZoneDefect5` FOREIGN KEY (`Zone`) REFERENCES `zone` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("zonedefect", Database:="warehouse")>
Public Class zonedefect: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DataSetWID As Long
    <DatabaseField("ArrayManufactureDeviation"), DataType(MySqlDbType.Int64, "20")> Public Property ArrayManufactureDeviation As Long
    <DatabaseField("ZoneDefect_DefectType"), DataType(MySqlDbType.Int64, "20")> Public Property ZoneDefect_DefectType As Long
    <DatabaseField("ZoneDefect_PositionDelta"), DataType(MySqlDbType.Int64, "20")> Public Property ZoneDefect_PositionDelta As Long
    <DatabaseField("Zone"), DataType(MySqlDbType.Int64, "20")> Public Property Zone As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `zonedefect` (`WID`, `DataSetWID`, `ArrayManufactureDeviation`, `ZoneDefect_DefectType`, `ZoneDefect_PositionDelta`, `Zone`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `zonedefect` (`WID`, `DataSetWID`, `ArrayManufactureDeviation`, `ZoneDefect_DefectType`, `ZoneDefect_PositionDelta`, `Zone`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `zonedefect` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `zonedefect` SET `WID`='{0}', `DataSetWID`='{1}', `ArrayManufactureDeviation`='{2}', `ZoneDefect_DefectType`='{3}', `ZoneDefect_PositionDelta`='{4}', `Zone`='{5}' WHERE `WID` = '{6}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, ArrayManufactureDeviation, ZoneDefect_DefectType, ZoneDefect_PositionDelta, Zone)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, ArrayManufactureDeviation, ZoneDefect_DefectType, ZoneDefect_PositionDelta, Zone)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, ArrayManufactureDeviation, ZoneDefect_DefectType, ZoneDefect_PositionDelta, Zone, WID)
    End Function
#End Region
End Class


End Namespace
