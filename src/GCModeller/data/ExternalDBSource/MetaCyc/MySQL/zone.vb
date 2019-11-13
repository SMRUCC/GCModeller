#Region "Microsoft.VisualBasic::f0956b28d65c24208485317fe0fe3cfb, data\ExternalDBSource\MetaCyc\MySQL\zone.vb"

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
    '     Function: Clone, GetDeleteSQL, GetDumpInsertValue, (+2 Overloads) GetInsertSQL, (+2 Overloads) GetReplaceSQL
    '               GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @2018/5/23 13:13:40


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace MetaCyc.MySQL

''' <summary>
''' ```SQL
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
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("zone", Database:="warehouse", SchemaSQL:="
CREATE TABLE `zone` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `Identifier` varchar(255) DEFAULT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `Row_` smallint(6) DEFAULT NULL,
  `Column_` smallint(6) DEFAULT NULL,
  `UpperLeftX` float DEFAULT NULL,
  `UpperLeftY` float DEFAULT NULL,
  `LowerRightX` float DEFAULT NULL,
  `LowerRightY` float DEFAULT NULL,
  `Zone_DistanceUnit` bigint(20) DEFAULT NULL,
  `ZoneGroup_ZoneLocations` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_Zone1` (`DataSetWID`),
  KEY `FK_Zone3` (`Zone_DistanceUnit`),
  KEY `FK_Zone4` (`ZoneGroup_ZoneLocations`),
  CONSTRAINT `FK_Zone1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Zone3` FOREIGN KEY (`Zone_DistanceUnit`) REFERENCES `unit` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Zone4` FOREIGN KEY (`ZoneGroup_ZoneLocations`) REFERENCES `zonegroup` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class zone: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DataSetWID")> Public Property DataSetWID As Long
    <DatabaseField("Identifier"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="Identifier")> Public Property Identifier As String
    <DatabaseField("Name"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="Name")> Public Property Name As String
    <DatabaseField("Row_"), DataType(MySqlDbType.Int64, "6"), Column(Name:="Row_")> Public Property Row_ As Long
    <DatabaseField("Column_"), DataType(MySqlDbType.Int64, "6"), Column(Name:="Column_")> Public Property Column_ As Long
    <DatabaseField("UpperLeftX"), DataType(MySqlDbType.Double), Column(Name:="UpperLeftX")> Public Property UpperLeftX As Double
    <DatabaseField("UpperLeftY"), DataType(MySqlDbType.Double), Column(Name:="UpperLeftY")> Public Property UpperLeftY As Double
    <DatabaseField("LowerRightX"), DataType(MySqlDbType.Double), Column(Name:="LowerRightX")> Public Property LowerRightX As Double
    <DatabaseField("LowerRightY"), DataType(MySqlDbType.Double), Column(Name:="LowerRightY")> Public Property LowerRightY As Double
    <DatabaseField("Zone_DistanceUnit"), DataType(MySqlDbType.Int64, "20"), Column(Name:="Zone_DistanceUnit")> Public Property Zone_DistanceUnit As Long
    <DatabaseField("ZoneGroup_ZoneLocations"), DataType(MySqlDbType.Int64, "20"), Column(Name:="ZoneGroup_ZoneLocations")> Public Property ZoneGroup_ZoneLocations As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `zone` (`WID`, `DataSetWID`, `Identifier`, `Name`, `Row_`, `Column_`, `UpperLeftX`, `UpperLeftY`, `LowerRightX`, `LowerRightY`, `Zone_DistanceUnit`, `ZoneGroup_ZoneLocations`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `zone` (`WID`, `DataSetWID`, `Identifier`, `Name`, `Row_`, `Column_`, `UpperLeftX`, `UpperLeftY`, `LowerRightX`, `LowerRightY`, `Zone_DistanceUnit`, `ZoneGroup_ZoneLocations`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `zone` (`WID`, `DataSetWID`, `Identifier`, `Name`, `Row_`, `Column_`, `UpperLeftX`, `UpperLeftY`, `LowerRightX`, `LowerRightY`, `Zone_DistanceUnit`, `ZoneGroup_ZoneLocations`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `zone` (`WID`, `DataSetWID`, `Identifier`, `Name`, `Row_`, `Column_`, `UpperLeftX`, `UpperLeftY`, `LowerRightX`, `LowerRightY`, `Zone_DistanceUnit`, `ZoneGroup_ZoneLocations`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `zone` WHERE `WID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `zone` SET `WID`='{0}', `DataSetWID`='{1}', `Identifier`='{2}', `Name`='{3}', `Row_`='{4}', `Column_`='{5}', `UpperLeftX`='{6}', `UpperLeftY`='{7}', `LowerRightX`='{8}', `LowerRightY`='{9}', `Zone_DistanceUnit`='{10}', `ZoneGroup_ZoneLocations`='{11}' WHERE `WID` = '{12}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `zone` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `zone` (`WID`, `DataSetWID`, `Identifier`, `Name`, `Row_`, `Column_`, `UpperLeftX`, `UpperLeftY`, `LowerRightX`, `LowerRightY`, `Zone_DistanceUnit`, `ZoneGroup_ZoneLocations`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, Identifier, Name, Row_, Column_, UpperLeftX, UpperLeftY, LowerRightX, LowerRightY, Zone_DistanceUnit, ZoneGroup_ZoneLocations)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `zone` (`WID`, `DataSetWID`, `Identifier`, `Name`, `Row_`, `Column_`, `UpperLeftX`, `UpperLeftY`, `LowerRightX`, `LowerRightY`, `Zone_DistanceUnit`, `ZoneGroup_ZoneLocations`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, WID, DataSetWID, Identifier, Name, Row_, Column_, UpperLeftX, UpperLeftY, LowerRightX, LowerRightY, Zone_DistanceUnit, ZoneGroup_ZoneLocations)
        Else
        Return String.Format(INSERT_SQL, WID, DataSetWID, Identifier, Name, Row_, Column_, UpperLeftX, UpperLeftY, LowerRightX, LowerRightY, Zone_DistanceUnit, ZoneGroup_ZoneLocations)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{WID}', '{DataSetWID}', '{Identifier}', '{Name}', '{Row_}', '{Column_}', '{UpperLeftX}', '{UpperLeftY}', '{LowerRightX}', '{LowerRightY}', '{Zone_DistanceUnit}', '{ZoneGroup_ZoneLocations}')"
        Else
            Return $"('{WID}', '{DataSetWID}', '{Identifier}', '{Name}', '{Row_}', '{Column_}', '{UpperLeftX}', '{UpperLeftY}', '{LowerRightX}', '{LowerRightY}', '{Zone_DistanceUnit}', '{ZoneGroup_ZoneLocations}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `zone` (`WID`, `DataSetWID`, `Identifier`, `Name`, `Row_`, `Column_`, `UpperLeftX`, `UpperLeftY`, `LowerRightX`, `LowerRightY`, `Zone_DistanceUnit`, `ZoneGroup_ZoneLocations`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, Identifier, Name, Row_, Column_, UpperLeftX, UpperLeftY, LowerRightX, LowerRightY, Zone_DistanceUnit, ZoneGroup_ZoneLocations)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `zone` (`WID`, `DataSetWID`, `Identifier`, `Name`, `Row_`, `Column_`, `UpperLeftX`, `UpperLeftY`, `LowerRightX`, `LowerRightY`, `Zone_DistanceUnit`, `ZoneGroup_ZoneLocations`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, WID, DataSetWID, Identifier, Name, Row_, Column_, UpperLeftX, UpperLeftY, LowerRightX, LowerRightY, Zone_DistanceUnit, ZoneGroup_ZoneLocations)
        Else
        Return String.Format(REPLACE_SQL, WID, DataSetWID, Identifier, Name, Row_, Column_, UpperLeftX, UpperLeftY, LowerRightX, LowerRightY, Zone_DistanceUnit, ZoneGroup_ZoneLocations)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `zone` SET `WID`='{0}', `DataSetWID`='{1}', `Identifier`='{2}', `Name`='{3}', `Row_`='{4}', `Column_`='{5}', `UpperLeftX`='{6}', `UpperLeftY`='{7}', `LowerRightX`='{8}', `LowerRightY`='{9}', `Zone_DistanceUnit`='{10}', `ZoneGroup_ZoneLocations`='{11}' WHERE `WID` = '{12}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, Identifier, Name, Row_, Column_, UpperLeftX, UpperLeftY, LowerRightX, LowerRightY, Zone_DistanceUnit, ZoneGroup_ZoneLocations, WID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As zone
                         Return DirectCast(MyClass.MemberwiseClone, zone)
                     End Function
End Class


End Namespace
