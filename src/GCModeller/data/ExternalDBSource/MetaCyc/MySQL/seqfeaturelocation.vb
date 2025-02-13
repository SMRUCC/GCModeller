﻿#Region "Microsoft.VisualBasic::623967f0cd6e6add0d9d81b1e08a698c, data\ExternalDBSource\MetaCyc\MySQL\seqfeaturelocation.vb"

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
    '     Properties: DataSetWID, SeqFeature_Regions, SeqFeatureLocation_Coordinate, SeqFeatureLocation_Subregions, StrandType
    '                 WID
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class seqfeaturelocation: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DataSetWID")> Public Property DataSetWID As Long
    <DatabaseField("SeqFeature_Regions"), DataType(MySqlDbType.Int64, "20"), Column(Name:="SeqFeature_Regions")> Public Property SeqFeature_Regions As Long
    <DatabaseField("StrandType"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="StrandType")> Public Property StrandType As String
    <DatabaseField("SeqFeatureLocation_Subregions"), DataType(MySqlDbType.Int64, "20"), Column(Name:="SeqFeatureLocation_Subregions")> Public Property SeqFeatureLocation_Subregions As Long
    <DatabaseField("SeqFeatureLocation_Coordinate"), DataType(MySqlDbType.Int64, "20"), Column(Name:="SeqFeatureLocation_Coordinate")> Public Property SeqFeatureLocation_Coordinate As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `seqfeaturelocation` (`WID`, `DataSetWID`, `SeqFeature_Regions`, `StrandType`, `SeqFeatureLocation_Subregions`, `SeqFeatureLocation_Coordinate`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `seqfeaturelocation` (`WID`, `DataSetWID`, `SeqFeature_Regions`, `StrandType`, `SeqFeatureLocation_Subregions`, `SeqFeatureLocation_Coordinate`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `seqfeaturelocation` (`WID`, `DataSetWID`, `SeqFeature_Regions`, `StrandType`, `SeqFeatureLocation_Subregions`, `SeqFeatureLocation_Coordinate`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `seqfeaturelocation` (`WID`, `DataSetWID`, `SeqFeature_Regions`, `StrandType`, `SeqFeatureLocation_Subregions`, `SeqFeatureLocation_Coordinate`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `seqfeaturelocation` WHERE `WID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `seqfeaturelocation` SET `WID`='{0}', `DataSetWID`='{1}', `SeqFeature_Regions`='{2}', `StrandType`='{3}', `SeqFeatureLocation_Subregions`='{4}', `SeqFeatureLocation_Coordinate`='{5}' WHERE `WID` = '{6}';</SQL>

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
''' ```SQL
''' INSERT INTO `seqfeaturelocation` (`WID`, `DataSetWID`, `SeqFeature_Regions`, `StrandType`, `SeqFeatureLocation_Subregions`, `SeqFeatureLocation_Coordinate`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, WID, DataSetWID, SeqFeature_Regions, StrandType, SeqFeatureLocation_Subregions, SeqFeatureLocation_Coordinate)
        Else
        Return String.Format(INSERT_SQL, WID, DataSetWID, SeqFeature_Regions, StrandType, SeqFeatureLocation_Subregions, SeqFeatureLocation_Coordinate)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{WID}', '{DataSetWID}', '{SeqFeature_Regions}', '{StrandType}', '{SeqFeatureLocation_Subregions}', '{SeqFeatureLocation_Coordinate}')"
        Else
            Return $"('{WID}', '{DataSetWID}', '{SeqFeature_Regions}', '{StrandType}', '{SeqFeatureLocation_Subregions}', '{SeqFeatureLocation_Coordinate}')"
        End If
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
''' REPLACE INTO `seqfeaturelocation` (`WID`, `DataSetWID`, `SeqFeature_Regions`, `StrandType`, `SeqFeatureLocation_Subregions`, `SeqFeatureLocation_Coordinate`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, WID, DataSetWID, SeqFeature_Regions, StrandType, SeqFeatureLocation_Subregions, SeqFeatureLocation_Coordinate)
        Else
        Return String.Format(REPLACE_SQL, WID, DataSetWID, SeqFeature_Regions, StrandType, SeqFeatureLocation_Subregions, SeqFeatureLocation_Coordinate)
        End If
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

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As seqfeaturelocation
                         Return DirectCast(MyClass.MemberwiseClone, seqfeaturelocation)
                     End Function
End Class


End Namespace
