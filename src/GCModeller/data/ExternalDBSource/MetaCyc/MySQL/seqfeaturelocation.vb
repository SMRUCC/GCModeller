#Region "Microsoft.VisualBasic::881ae8c94135e1a2b46f775f62bb37ec, data\ExternalDBSource\MetaCyc\MySQL\seqfeaturelocation.vb"

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

    ' Class seqfeaturelocation
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
''' DROP TABLE IF EXISTS `seqfeaturelocation`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `seqfeaturelocation` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `SeqFeature_Regions` bigint(20) DEFAULT NULL,
'''   `StrandType` varchar(255) DEFAULT NULL,
'''   `SeqFeatureLocation_Subregions` bigint(20) DEFAULT NULL,
'''   `SeqFeatureLocation_Coordinate` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_SeqFeatureLocation1` (`DataSetWID`),
'''   KEY `FK_SeqFeatureLocation2` (`SeqFeature_Regions`),
'''   KEY `FK_SeqFeatureLocation3` (`SeqFeatureLocation_Subregions`),
'''   KEY `FK_SeqFeatureLocation4` (`SeqFeatureLocation_Coordinate`),
'''   CONSTRAINT `FK_SeqFeatureLocation1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_SeqFeatureLocation2` FOREIGN KEY (`SeqFeature_Regions`) REFERENCES `feature` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_SeqFeatureLocation3` FOREIGN KEY (`SeqFeatureLocation_Subregions`) REFERENCES `seqfeaturelocation` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_SeqFeatureLocation4` FOREIGN KEY (`SeqFeatureLocation_Coordinate`) REFERENCES `sequenceposition` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("seqfeaturelocation", Database:="warehouse", SchemaSQL:="
CREATE TABLE `seqfeaturelocation` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `SeqFeature_Regions` bigint(20) DEFAULT NULL,
  `StrandType` varchar(255) DEFAULT NULL,
  `SeqFeatureLocation_Subregions` bigint(20) DEFAULT NULL,
  `SeqFeatureLocation_Coordinate` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_SeqFeatureLocation1` (`DataSetWID`),
  KEY `FK_SeqFeatureLocation2` (`SeqFeature_Regions`),
  KEY `FK_SeqFeatureLocation3` (`SeqFeatureLocation_Subregions`),
  KEY `FK_SeqFeatureLocation4` (`SeqFeatureLocation_Coordinate`),
  CONSTRAINT `FK_SeqFeatureLocation1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_SeqFeatureLocation2` FOREIGN KEY (`SeqFeature_Regions`) REFERENCES `feature` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_SeqFeatureLocation3` FOREIGN KEY (`SeqFeatureLocation_Subregions`) REFERENCES `seqfeaturelocation` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_SeqFeatureLocation4` FOREIGN KEY (`SeqFeatureLocation_Coordinate`) REFERENCES `sequenceposition` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class seqfeaturelocation: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DataSetWID As Long
    <DatabaseField("SeqFeature_Regions"), DataType(MySqlDbType.Int64, "20")> Public Property SeqFeature_Regions As Long
    <DatabaseField("StrandType"), DataType(MySqlDbType.VarChar, "255")> Public Property StrandType As String
    <DatabaseField("SeqFeatureLocation_Subregions"), DataType(MySqlDbType.Int64, "20")> Public Property SeqFeatureLocation_Subregions As Long
    <DatabaseField("SeqFeatureLocation_Coordinate"), DataType(MySqlDbType.Int64, "20")> Public Property SeqFeatureLocation_Coordinate As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `seqfeaturelocation` (`WID`, `DataSetWID`, `SeqFeature_Regions`, `StrandType`, `SeqFeatureLocation_Subregions`, `SeqFeatureLocation_Coordinate`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `seqfeaturelocation` (`WID`, `DataSetWID`, `SeqFeature_Regions`, `StrandType`, `SeqFeatureLocation_Subregions`, `SeqFeatureLocation_Coordinate`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `seqfeaturelocation` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `seqfeaturelocation` SET `WID`='{0}', `DataSetWID`='{1}', `SeqFeature_Regions`='{2}', `StrandType`='{3}', `SeqFeatureLocation_Subregions`='{4}', `SeqFeatureLocation_Coordinate`='{5}' WHERE `WID` = '{6}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `seqfeaturelocation` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `seqfeaturelocation` (`WID`, `DataSetWID`, `SeqFeature_Regions`, `StrandType`, `SeqFeatureLocation_Subregions`, `SeqFeatureLocation_Coordinate`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, SeqFeature_Regions, StrandType, SeqFeatureLocation_Subregions, SeqFeatureLocation_Coordinate)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{WID}', '{DataSetWID}', '{SeqFeature_Regions}', '{StrandType}', '{SeqFeatureLocation_Subregions}', '{SeqFeatureLocation_Coordinate}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `seqfeaturelocation` (`WID`, `DataSetWID`, `SeqFeature_Regions`, `StrandType`, `SeqFeatureLocation_Subregions`, `SeqFeatureLocation_Coordinate`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, SeqFeature_Regions, StrandType, SeqFeatureLocation_Subregions, SeqFeatureLocation_Coordinate)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `seqfeaturelocation` SET `WID`='{0}', `DataSetWID`='{1}', `SeqFeature_Regions`='{2}', `StrandType`='{3}', `SeqFeatureLocation_Subregions`='{4}', `SeqFeatureLocation_Coordinate`='{5}' WHERE `WID` = '{6}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, SeqFeature_Regions, StrandType, SeqFeatureLocation_Subregions, SeqFeatureLocation_Coordinate, WID)
    End Function
#End Region
End Class


End Namespace
