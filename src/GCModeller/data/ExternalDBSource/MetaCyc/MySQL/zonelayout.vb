#Region "Microsoft.VisualBasic::a43019038bde31f67cc94d07a1d178bc, data\ExternalDBSource\MetaCyc\MySQL\zonelayout.vb"

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

    ' Class zonelayout
    ' 
    '     Properties: DataSetWID, NumFeaturesPerCol, NumFeaturesPerRow, SpacingBetweenCols, SpacingBetweenRows
    '                 WID, ZoneLayout_DistanceUnit
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
''' DROP TABLE IF EXISTS `zonelayout`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `zonelayout` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `NumFeaturesPerRow` smallint(6) DEFAULT NULL,
'''   `NumFeaturesPerCol` smallint(6) DEFAULT NULL,
'''   `SpacingBetweenRows` float DEFAULT NULL,
'''   `SpacingBetweenCols` float DEFAULT NULL,
'''   `ZoneLayout_DistanceUnit` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_ZoneLayout1` (`DataSetWID`),
'''   KEY `FK_ZoneLayout2` (`ZoneLayout_DistanceUnit`),
'''   CONSTRAINT `FK_ZoneLayout1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_ZoneLayout2` FOREIGN KEY (`ZoneLayout_DistanceUnit`) REFERENCES `unit` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' /*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;
''' 
''' /*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
''' /*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
''' /*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
''' /*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
''' /*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
''' /*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
''' /*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
''' 
''' -- Dump completed on 2015-12-03 20:02:01
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("zonelayout", Database:="warehouse", SchemaSQL:="
CREATE TABLE `zonelayout` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `NumFeaturesPerRow` smallint(6) DEFAULT NULL,
  `NumFeaturesPerCol` smallint(6) DEFAULT NULL,
  `SpacingBetweenRows` float DEFAULT NULL,
  `SpacingBetweenCols` float DEFAULT NULL,
  `ZoneLayout_DistanceUnit` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_ZoneLayout1` (`DataSetWID`),
  KEY `FK_ZoneLayout2` (`ZoneLayout_DistanceUnit`),
  CONSTRAINT `FK_ZoneLayout1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ZoneLayout2` FOREIGN KEY (`ZoneLayout_DistanceUnit`) REFERENCES `unit` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class zonelayout: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DataSetWID")> Public Property DataSetWID As Long
    <DatabaseField("NumFeaturesPerRow"), DataType(MySqlDbType.Int64, "6"), Column(Name:="NumFeaturesPerRow")> Public Property NumFeaturesPerRow As Long
    <DatabaseField("NumFeaturesPerCol"), DataType(MySqlDbType.Int64, "6"), Column(Name:="NumFeaturesPerCol")> Public Property NumFeaturesPerCol As Long
    <DatabaseField("SpacingBetweenRows"), DataType(MySqlDbType.Double), Column(Name:="SpacingBetweenRows")> Public Property SpacingBetweenRows As Double
    <DatabaseField("SpacingBetweenCols"), DataType(MySqlDbType.Double), Column(Name:="SpacingBetweenCols")> Public Property SpacingBetweenCols As Double
    <DatabaseField("ZoneLayout_DistanceUnit"), DataType(MySqlDbType.Int64, "20"), Column(Name:="ZoneLayout_DistanceUnit")> Public Property ZoneLayout_DistanceUnit As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `zonelayout` (`WID`, `DataSetWID`, `NumFeaturesPerRow`, `NumFeaturesPerCol`, `SpacingBetweenRows`, `SpacingBetweenCols`, `ZoneLayout_DistanceUnit`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `zonelayout` (`WID`, `DataSetWID`, `NumFeaturesPerRow`, `NumFeaturesPerCol`, `SpacingBetweenRows`, `SpacingBetweenCols`, `ZoneLayout_DistanceUnit`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `zonelayout` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `zonelayout` SET `WID`='{0}', `DataSetWID`='{1}', `NumFeaturesPerRow`='{2}', `NumFeaturesPerCol`='{3}', `SpacingBetweenRows`='{4}', `SpacingBetweenCols`='{5}', `ZoneLayout_DistanceUnit`='{6}' WHERE `WID` = '{7}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `zonelayout` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `zonelayout` (`WID`, `DataSetWID`, `NumFeaturesPerRow`, `NumFeaturesPerCol`, `SpacingBetweenRows`, `SpacingBetweenCols`, `ZoneLayout_DistanceUnit`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, NumFeaturesPerRow, NumFeaturesPerCol, SpacingBetweenRows, SpacingBetweenCols, ZoneLayout_DistanceUnit)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{WID}', '{DataSetWID}', '{NumFeaturesPerRow}', '{NumFeaturesPerCol}', '{SpacingBetweenRows}', '{SpacingBetweenCols}', '{ZoneLayout_DistanceUnit}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `zonelayout` (`WID`, `DataSetWID`, `NumFeaturesPerRow`, `NumFeaturesPerCol`, `SpacingBetweenRows`, `SpacingBetweenCols`, `ZoneLayout_DistanceUnit`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, NumFeaturesPerRow, NumFeaturesPerCol, SpacingBetweenRows, SpacingBetweenCols, ZoneLayout_DistanceUnit)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `zonelayout` SET `WID`='{0}', `DataSetWID`='{1}', `NumFeaturesPerRow`='{2}', `NumFeaturesPerCol`='{3}', `SpacingBetweenRows`='{4}', `SpacingBetweenCols`='{5}', `ZoneLayout_DistanceUnit`='{6}' WHERE `WID` = '{7}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, NumFeaturesPerRow, NumFeaturesPerCol, SpacingBetweenRows, SpacingBetweenCols, ZoneLayout_DistanceUnit, WID)
    End Function
#End Region
Public Function Clone() As zonelayout
                  Return DirectCast(MyClass.MemberwiseClone, zonelayout)
              End Function
End Class


End Namespace

