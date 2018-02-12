#Region "Microsoft.VisualBasic::46e8a79eca0f5e40bc48467f198682f2, data\ExternalDBSource\MetaCyc\MySQL\positiondelta.vb"

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

    ' Class positiondelta
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
''' DROP TABLE IF EXISTS `positiondelta`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `positiondelta` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `DeltaX` float DEFAULT NULL,
'''   `DeltaY` float DEFAULT NULL,
'''   `PositionDelta_DistanceUnit` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_PositionDelta1` (`DataSetWID`),
'''   KEY `FK_PositionDelta2` (`PositionDelta_DistanceUnit`),
'''   CONSTRAINT `FK_PositionDelta1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_PositionDelta2` FOREIGN KEY (`PositionDelta_DistanceUnit`) REFERENCES `unit` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("positiondelta", Database:="warehouse", SchemaSQL:="
CREATE TABLE `positiondelta` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `DeltaX` float DEFAULT NULL,
  `DeltaY` float DEFAULT NULL,
  `PositionDelta_DistanceUnit` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_PositionDelta1` (`DataSetWID`),
  KEY `FK_PositionDelta2` (`PositionDelta_DistanceUnit`),
  CONSTRAINT `FK_PositionDelta1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_PositionDelta2` FOREIGN KEY (`PositionDelta_DistanceUnit`) REFERENCES `unit` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class positiondelta: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DataSetWID As Long
    <DatabaseField("DeltaX"), DataType(MySqlDbType.Double)> Public Property DeltaX As Double
    <DatabaseField("DeltaY"), DataType(MySqlDbType.Double)> Public Property DeltaY As Double
    <DatabaseField("PositionDelta_DistanceUnit"), DataType(MySqlDbType.Int64, "20")> Public Property PositionDelta_DistanceUnit As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `positiondelta` (`WID`, `DataSetWID`, `DeltaX`, `DeltaY`, `PositionDelta_DistanceUnit`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `positiondelta` (`WID`, `DataSetWID`, `DeltaX`, `DeltaY`, `PositionDelta_DistanceUnit`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `positiondelta` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `positiondelta` SET `WID`='{0}', `DataSetWID`='{1}', `DeltaX`='{2}', `DeltaY`='{3}', `PositionDelta_DistanceUnit`='{4}' WHERE `WID` = '{5}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `positiondelta` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `positiondelta` (`WID`, `DataSetWID`, `DeltaX`, `DeltaY`, `PositionDelta_DistanceUnit`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, DeltaX, DeltaY, PositionDelta_DistanceUnit)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{WID}', '{DataSetWID}', '{DeltaX}', '{DeltaY}', '{PositionDelta_DistanceUnit}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `positiondelta` (`WID`, `DataSetWID`, `DeltaX`, `DeltaY`, `PositionDelta_DistanceUnit`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, DeltaX, DeltaY, PositionDelta_DistanceUnit)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `positiondelta` SET `WID`='{0}', `DataSetWID`='{1}', `DeltaX`='{2}', `DeltaY`='{3}', `PositionDelta_DistanceUnit`='{4}' WHERE `WID` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, DeltaX, DeltaY, PositionDelta_DistanceUnit, WID)
    End Function
#End Region
End Class


End Namespace
