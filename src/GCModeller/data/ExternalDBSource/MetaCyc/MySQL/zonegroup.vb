#Region "Microsoft.VisualBasic::5129bd7d8314914ba1075eab67b5f58b, data\ExternalDBSource\MetaCyc\MySQL\zonegroup.vb"

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

    ' Class zonegroup
    ' 
    '     Properties: DataSetWID, PhysicalArrayDesign_ZoneGroups, SpacingsBetweenZonesX, SpacingsBetweenZonesY, WID
    '                 ZoneGroup_DistanceUnit, ZoneGroup_ZoneLayout, ZonesPerX, ZonesPerY
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @3/16/2018 10:40:19 PM


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
''' DROP TABLE IF EXISTS `zonegroup`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `zonegroup` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `PhysicalArrayDesign_ZoneGroups` bigint(20) DEFAULT NULL,
'''   `SpacingsBetweenZonesX` float DEFAULT NULL,
'''   `SpacingsBetweenZonesY` float DEFAULT NULL,
'''   `ZonesPerX` smallint(6) DEFAULT NULL,
'''   `ZonesPerY` smallint(6) DEFAULT NULL,
'''   `ZoneGroup_DistanceUnit` bigint(20) DEFAULT NULL,
'''   `ZoneGroup_ZoneLayout` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_ZoneGroup1` (`DataSetWID`),
'''   KEY `FK_ZoneGroup2` (`PhysicalArrayDesign_ZoneGroups`),
'''   KEY `FK_ZoneGroup3` (`ZoneGroup_DistanceUnit`),
'''   KEY `FK_ZoneGroup4` (`ZoneGroup_ZoneLayout`),
'''   CONSTRAINT `FK_ZoneGroup1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_ZoneGroup2` FOREIGN KEY (`PhysicalArrayDesign_ZoneGroups`) REFERENCES `arraydesign` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_ZoneGroup3` FOREIGN KEY (`ZoneGroup_DistanceUnit`) REFERENCES `unit` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_ZoneGroup4` FOREIGN KEY (`ZoneGroup_ZoneLayout`) REFERENCES `zonelayout` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("zonegroup", Database:="warehouse", SchemaSQL:="
CREATE TABLE `zonegroup` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `PhysicalArrayDesign_ZoneGroups` bigint(20) DEFAULT NULL,
  `SpacingsBetweenZonesX` float DEFAULT NULL,
  `SpacingsBetweenZonesY` float DEFAULT NULL,
  `ZonesPerX` smallint(6) DEFAULT NULL,
  `ZonesPerY` smallint(6) DEFAULT NULL,
  `ZoneGroup_DistanceUnit` bigint(20) DEFAULT NULL,
  `ZoneGroup_ZoneLayout` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_ZoneGroup1` (`DataSetWID`),
  KEY `FK_ZoneGroup2` (`PhysicalArrayDesign_ZoneGroups`),
  KEY `FK_ZoneGroup3` (`ZoneGroup_DistanceUnit`),
  KEY `FK_ZoneGroup4` (`ZoneGroup_ZoneLayout`),
  CONSTRAINT `FK_ZoneGroup1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ZoneGroup2` FOREIGN KEY (`PhysicalArrayDesign_ZoneGroups`) REFERENCES `arraydesign` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ZoneGroup3` FOREIGN KEY (`ZoneGroup_DistanceUnit`) REFERENCES `unit` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ZoneGroup4` FOREIGN KEY (`ZoneGroup_ZoneLayout`) REFERENCES `zonelayout` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class zonegroup: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DataSetWID")> Public Property DataSetWID As Long
    <DatabaseField("PhysicalArrayDesign_ZoneGroups"), DataType(MySqlDbType.Int64, "20"), Column(Name:="PhysicalArrayDesign_ZoneGroups")> Public Property PhysicalArrayDesign_ZoneGroups As Long
    <DatabaseField("SpacingsBetweenZonesX"), DataType(MySqlDbType.Double), Column(Name:="SpacingsBetweenZonesX")> Public Property SpacingsBetweenZonesX As Double
    <DatabaseField("SpacingsBetweenZonesY"), DataType(MySqlDbType.Double), Column(Name:="SpacingsBetweenZonesY")> Public Property SpacingsBetweenZonesY As Double
    <DatabaseField("ZonesPerX"), DataType(MySqlDbType.Int64, "6"), Column(Name:="ZonesPerX")> Public Property ZonesPerX As Long
    <DatabaseField("ZonesPerY"), DataType(MySqlDbType.Int64, "6"), Column(Name:="ZonesPerY")> Public Property ZonesPerY As Long
    <DatabaseField("ZoneGroup_DistanceUnit"), DataType(MySqlDbType.Int64, "20"), Column(Name:="ZoneGroup_DistanceUnit")> Public Property ZoneGroup_DistanceUnit As Long
    <DatabaseField("ZoneGroup_ZoneLayout"), DataType(MySqlDbType.Int64, "20"), Column(Name:="ZoneGroup_ZoneLayout")> Public Property ZoneGroup_ZoneLayout As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `zonegroup` (`WID`, `DataSetWID`, `PhysicalArrayDesign_ZoneGroups`, `SpacingsBetweenZonesX`, `SpacingsBetweenZonesY`, `ZonesPerX`, `ZonesPerY`, `ZoneGroup_DistanceUnit`, `ZoneGroup_ZoneLayout`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `zonegroup` (`WID`, `DataSetWID`, `PhysicalArrayDesign_ZoneGroups`, `SpacingsBetweenZonesX`, `SpacingsBetweenZonesY`, `ZonesPerX`, `ZonesPerY`, `ZoneGroup_DistanceUnit`, `ZoneGroup_ZoneLayout`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `zonegroup` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `zonegroup` SET `WID`='{0}', `DataSetWID`='{1}', `PhysicalArrayDesign_ZoneGroups`='{2}', `SpacingsBetweenZonesX`='{3}', `SpacingsBetweenZonesY`='{4}', `ZonesPerX`='{5}', `ZonesPerY`='{6}', `ZoneGroup_DistanceUnit`='{7}', `ZoneGroup_ZoneLayout`='{8}' WHERE `WID` = '{9}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `zonegroup` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `zonegroup` (`WID`, `DataSetWID`, `PhysicalArrayDesign_ZoneGroups`, `SpacingsBetweenZonesX`, `SpacingsBetweenZonesY`, `ZonesPerX`, `ZonesPerY`, `ZoneGroup_DistanceUnit`, `ZoneGroup_ZoneLayout`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, PhysicalArrayDesign_ZoneGroups, SpacingsBetweenZonesX, SpacingsBetweenZonesY, ZonesPerX, ZonesPerY, ZoneGroup_DistanceUnit, ZoneGroup_ZoneLayout)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{WID}', '{DataSetWID}', '{PhysicalArrayDesign_ZoneGroups}', '{SpacingsBetweenZonesX}', '{SpacingsBetweenZonesY}', '{ZonesPerX}', '{ZonesPerY}', '{ZoneGroup_DistanceUnit}', '{ZoneGroup_ZoneLayout}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `zonegroup` (`WID`, `DataSetWID`, `PhysicalArrayDesign_ZoneGroups`, `SpacingsBetweenZonesX`, `SpacingsBetweenZonesY`, `ZonesPerX`, `ZonesPerY`, `ZoneGroup_DistanceUnit`, `ZoneGroup_ZoneLayout`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, PhysicalArrayDesign_ZoneGroups, SpacingsBetweenZonesX, SpacingsBetweenZonesY, ZonesPerX, ZonesPerY, ZoneGroup_DistanceUnit, ZoneGroup_ZoneLayout)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `zonegroup` SET `WID`='{0}', `DataSetWID`='{1}', `PhysicalArrayDesign_ZoneGroups`='{2}', `SpacingsBetweenZonesX`='{3}', `SpacingsBetweenZonesY`='{4}', `ZonesPerX`='{5}', `ZonesPerY`='{6}', `ZoneGroup_DistanceUnit`='{7}', `ZoneGroup_ZoneLayout`='{8}' WHERE `WID` = '{9}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, PhysicalArrayDesign_ZoneGroups, SpacingsBetweenZonesX, SpacingsBetweenZonesY, ZonesPerX, ZonesPerY, ZoneGroup_DistanceUnit, ZoneGroup_ZoneLayout, WID)
    End Function
#End Region
Public Function Clone() As zonegroup
                  Return DirectCast(MyClass.MemberwiseClone, zonegroup)
              End Function
End Class


End Namespace
