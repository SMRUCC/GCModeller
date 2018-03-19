#Region "Microsoft.VisualBasic::7e19f030d9518ddbb1b62a41583fc7f7, data\ExternalDBSource\MetaCyc\bio_warehouse\featuredefect.vb"

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

    ' Class featuredefect
    ' 
    '     Properties: ArrayManufactureDeviation, DataSetWID, Feature, FeatureDefect_DefectType, FeatureDefect_PositionDelta
    '                 WID
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
''' DROP TABLE IF EXISTS `featuredefect`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `featuredefect` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `ArrayManufactureDeviation` bigint(20) DEFAULT NULL,
'''   `FeatureDefect_DefectType` bigint(20) DEFAULT NULL,
'''   `FeatureDefect_PositionDelta` bigint(20) DEFAULT NULL,
'''   `Feature` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_FeatureDefect1` (`DataSetWID`),
'''   KEY `FK_FeatureDefect2` (`ArrayManufactureDeviation`),
'''   KEY `FK_FeatureDefect3` (`FeatureDefect_DefectType`),
'''   KEY `FK_FeatureDefect4` (`FeatureDefect_PositionDelta`),
'''   KEY `FK_FeatureDefect5` (`Feature`),
'''   CONSTRAINT `FK_FeatureDefect1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_FeatureDefect2` FOREIGN KEY (`ArrayManufactureDeviation`) REFERENCES `arraymanufacturedeviation` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_FeatureDefect3` FOREIGN KEY (`FeatureDefect_DefectType`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_FeatureDefect4` FOREIGN KEY (`FeatureDefect_PositionDelta`) REFERENCES `positiondelta` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_FeatureDefect5` FOREIGN KEY (`Feature`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("featuredefect", Database:="warehouse")>
Public Class featuredefect: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DataSetWID As Long
    <DatabaseField("ArrayManufactureDeviation"), DataType(MySqlDbType.Int64, "20")> Public Property ArrayManufactureDeviation As Long
    <DatabaseField("FeatureDefect_DefectType"), DataType(MySqlDbType.Int64, "20")> Public Property FeatureDefect_DefectType As Long
    <DatabaseField("FeatureDefect_PositionDelta"), DataType(MySqlDbType.Int64, "20")> Public Property FeatureDefect_PositionDelta As Long
    <DatabaseField("Feature"), DataType(MySqlDbType.Int64, "20")> Public Property Feature As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `featuredefect` (`WID`, `DataSetWID`, `ArrayManufactureDeviation`, `FeatureDefect_DefectType`, `FeatureDefect_PositionDelta`, `Feature`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `featuredefect` (`WID`, `DataSetWID`, `ArrayManufactureDeviation`, `FeatureDefect_DefectType`, `FeatureDefect_PositionDelta`, `Feature`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `featuredefect` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `featuredefect` SET `WID`='{0}', `DataSetWID`='{1}', `ArrayManufactureDeviation`='{2}', `FeatureDefect_DefectType`='{3}', `FeatureDefect_PositionDelta`='{4}', `Feature`='{5}' WHERE `WID` = '{6}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, ArrayManufactureDeviation, FeatureDefect_DefectType, FeatureDefect_PositionDelta, Feature)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, ArrayManufactureDeviation, FeatureDefect_DefectType, FeatureDefect_PositionDelta, Feature)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, ArrayManufactureDeviation, FeatureDefect_DefectType, FeatureDefect_PositionDelta, Feature, WID)
    End Function
#End Region
End Class


End Namespace
