#Region "Microsoft.VisualBasic::6f0d1cf9494c4a07b78d4785fba8754b, data\ExternalDBSource\MetaCyc\MySQL\flowcytometrysample.vb"

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

    ' Class flowcytometrysample
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @3/29/2017 8:48:56 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MetaCyc.MySQL

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `flowcytometrysample`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `flowcytometrysample` (
'''   `WID` bigint(20) NOT NULL,
'''   `BioSourceWID` bigint(20) DEFAULT NULL,
'''   `FlowCytometryProbeWID` bigint(20) DEFAULT NULL,
'''   `MeasurementWID` bigint(20) DEFAULT NULL,
'''   `ManufacturerWID` bigint(20) DEFAULT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FlowCytometrySample_DWID` (`DataSetWID`),
'''   KEY `FK_FlowCytometrySample1` (`BioSourceWID`),
'''   KEY `FK_FlowCytometrySample2` (`FlowCytometryProbeWID`),
'''   KEY `FK_FlowCytometrySample3` (`MeasurementWID`),
'''   KEY `FK_FlowCytometrySample4` (`ManufacturerWID`),
'''   CONSTRAINT `FK_FlowCytometrySample1` FOREIGN KEY (`BioSourceWID`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_FlowCytometrySample2` FOREIGN KEY (`FlowCytometryProbeWID`) REFERENCES `flowcytometryprobe` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_FlowCytometrySample3` FOREIGN KEY (`MeasurementWID`) REFERENCES `measurement` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_FlowCytometrySample4` FOREIGN KEY (`ManufacturerWID`) REFERENCES `contact` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_FlowCytometrySampleDS` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("flowcytometrysample", Database:="warehouse", SchemaSQL:="
CREATE TABLE `flowcytometrysample` (
  `WID` bigint(20) NOT NULL,
  `BioSourceWID` bigint(20) DEFAULT NULL,
  `FlowCytometryProbeWID` bigint(20) DEFAULT NULL,
  `MeasurementWID` bigint(20) DEFAULT NULL,
  `ManufacturerWID` bigint(20) DEFAULT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `FlowCytometrySample_DWID` (`DataSetWID`),
  KEY `FK_FlowCytometrySample1` (`BioSourceWID`),
  KEY `FK_FlowCytometrySample2` (`FlowCytometryProbeWID`),
  KEY `FK_FlowCytometrySample3` (`MeasurementWID`),
  KEY `FK_FlowCytometrySample4` (`ManufacturerWID`),
  CONSTRAINT `FK_FlowCytometrySample1` FOREIGN KEY (`BioSourceWID`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_FlowCytometrySample2` FOREIGN KEY (`FlowCytometryProbeWID`) REFERENCES `flowcytometryprobe` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_FlowCytometrySample3` FOREIGN KEY (`MeasurementWID`) REFERENCES `measurement` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_FlowCytometrySample4` FOREIGN KEY (`ManufacturerWID`) REFERENCES `contact` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_FlowCytometrySampleDS` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class flowcytometrysample: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property WID As Long
    <DatabaseField("BioSourceWID"), DataType(MySqlDbType.Int64, "20")> Public Property BioSourceWID As Long
    <DatabaseField("FlowCytometryProbeWID"), DataType(MySqlDbType.Int64, "20")> Public Property FlowCytometryProbeWID As Long
    <DatabaseField("MeasurementWID"), DataType(MySqlDbType.Int64, "20")> Public Property MeasurementWID As Long
    <DatabaseField("ManufacturerWID"), DataType(MySqlDbType.Int64, "20")> Public Property ManufacturerWID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DataSetWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `flowcytometrysample` (`WID`, `BioSourceWID`, `FlowCytometryProbeWID`, `MeasurementWID`, `ManufacturerWID`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `flowcytometrysample` (`WID`, `BioSourceWID`, `FlowCytometryProbeWID`, `MeasurementWID`, `ManufacturerWID`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `flowcytometrysample` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `flowcytometrysample` SET `WID`='{0}', `BioSourceWID`='{1}', `FlowCytometryProbeWID`='{2}', `MeasurementWID`='{3}', `ManufacturerWID`='{4}', `DataSetWID`='{5}' WHERE `WID` = '{6}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `flowcytometrysample` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `flowcytometrysample` (`WID`, `BioSourceWID`, `FlowCytometryProbeWID`, `MeasurementWID`, `ManufacturerWID`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, BioSourceWID, FlowCytometryProbeWID, MeasurementWID, ManufacturerWID, DataSetWID)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{WID}', '{BioSourceWID}', '{FlowCytometryProbeWID}', '{MeasurementWID}', '{ManufacturerWID}', '{DataSetWID}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `flowcytometrysample` (`WID`, `BioSourceWID`, `FlowCytometryProbeWID`, `MeasurementWID`, `ManufacturerWID`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, BioSourceWID, FlowCytometryProbeWID, MeasurementWID, ManufacturerWID, DataSetWID)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `flowcytometrysample` SET `WID`='{0}', `BioSourceWID`='{1}', `FlowCytometryProbeWID`='{2}', `MeasurementWID`='{3}', `ManufacturerWID`='{4}', `DataSetWID`='{5}' WHERE `WID` = '{6}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, BioSourceWID, FlowCytometryProbeWID, MeasurementWID, ManufacturerWID, DataSetWID, WID)
    End Function
#End Region
End Class


End Namespace
