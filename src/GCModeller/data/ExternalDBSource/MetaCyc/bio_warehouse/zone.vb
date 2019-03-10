#Region "Microsoft.VisualBasic::81c49a9656338c8b59c0bef8cfaa74fa, data\ExternalDBSource\MetaCyc\bio_warehouse\zone.vb"

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

    ' Class zone
    ' 
    '     Properties: Column_, DataSetWID, Identifier, LowerRightX, LowerRightY
    '                 Name, Row_, UpperLeftX, UpperLeftY, WID
    '                 Zone_DistanceUnit, ZoneGroup_ZoneLocations
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
''' DROP TABLE IF EXISTS `zone`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `zone` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `Identifier` varchar(255) DEFAULT NULL,
'''   `Name` varchar(255) DEFAULT NULL,
'''   `Row_` smallint(6) DEFAULT NULL,
'''   `Column_` smallint(6) DEFAULT NULL,
'''   `UpperLeftX` float DEFAULT NULL,
'''   `UpperLeftY` float DEFAULT NULL,
'''   `LowerRightX` float DEFAULT NULL,
'''   `LowerRightY` float DEFAULT NULL,
'''   `Zone_DistanceUnit` bigint(20) DEFAULT NULL,
'''   `ZoneGroup_ZoneLocations` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_Zone1` (`DataSetWID`),
'''   KEY `FK_Zone3` (`Zone_DistanceUnit`),
'''   KEY `FK_Zone4` (`ZoneGroup_ZoneLocations`),
'''   CONSTRAINT `FK_Zone1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Zone3` FOREIGN KEY (`Zone_DistanceUnit`) REFERENCES `unit` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Zone4` FOREIGN KEY (`ZoneGroup_ZoneLocations`) REFERENCES `zonegroup` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("zone", Database:="warehouse")>
Public Class zone: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DataSetWID As Long
    <DatabaseField("Identifier"), DataType(MySqlDbType.VarChar, "255")> Public Property Identifier As String
    <DatabaseField("Name"), DataType(MySqlDbType.VarChar, "255")> Public Property Name As String
    <DatabaseField("Row_"), DataType(MySqlDbType.Int64, "6")> Public Property Row_ As Long
    <DatabaseField("Column_"), DataType(MySqlDbType.Int64, "6")> Public Property Column_ As Long
    <DatabaseField("UpperLeftX"), DataType(MySqlDbType.Double)> Public Property UpperLeftX As Double
    <DatabaseField("UpperLeftY"), DataType(MySqlDbType.Double)> Public Property UpperLeftY As Double
    <DatabaseField("LowerRightX"), DataType(MySqlDbType.Double)> Public Property LowerRightX As Double
    <DatabaseField("LowerRightY"), DataType(MySqlDbType.Double)> Public Property LowerRightY As Double
    <DatabaseField("Zone_DistanceUnit"), DataType(MySqlDbType.Int64, "20")> Public Property Zone_DistanceUnit As Long
    <DatabaseField("ZoneGroup_ZoneLocations"), DataType(MySqlDbType.Int64, "20")> Public Property ZoneGroup_ZoneLocations As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `zone` (`WID`, `DataSetWID`, `Identifier`, `Name`, `Row_`, `Column_`, `UpperLeftX`, `UpperLeftY`, `LowerRightX`, `LowerRightY`, `Zone_DistanceUnit`, `ZoneGroup_ZoneLocations`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `zone` (`WID`, `DataSetWID`, `Identifier`, `Name`, `Row_`, `Column_`, `UpperLeftX`, `UpperLeftY`, `LowerRightX`, `LowerRightY`, `Zone_DistanceUnit`, `ZoneGroup_ZoneLocations`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `zone` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `zone` SET `WID`='{0}', `DataSetWID`='{1}', `Identifier`='{2}', `Name`='{3}', `Row_`='{4}', `Column_`='{5}', `UpperLeftX`='{6}', `UpperLeftY`='{7}', `LowerRightX`='{8}', `LowerRightY`='{9}', `Zone_DistanceUnit`='{10}', `ZoneGroup_ZoneLocations`='{11}' WHERE `WID` = '{12}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, Identifier, Name, Row_, Column_, UpperLeftX, UpperLeftY, LowerRightX, LowerRightY, Zone_DistanceUnit, ZoneGroup_ZoneLocations)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, Identifier, Name, Row_, Column_, UpperLeftX, UpperLeftY, LowerRightX, LowerRightY, Zone_DistanceUnit, ZoneGroup_ZoneLocations)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, Identifier, Name, Row_, Column_, UpperLeftX, UpperLeftY, LowerRightX, LowerRightY, Zone_DistanceUnit, ZoneGroup_ZoneLocations, WID)
    End Function
#End Region
End Class


End Namespace
