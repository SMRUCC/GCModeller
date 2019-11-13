#Region "Microsoft.VisualBasic::5915d17eb50c7b931268089bc591faa6, data\ExternalDBSource\MetaCyc\MySQL\biomaterialmeasurement.vb"

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

    ' Class biomaterialmeasurement
    ' 
    '     Properties: BioAssayCreation, BioMaterial, DataSetWID, Measurement, Treatment
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
''' DROP TABLE IF EXISTS `biomaterialmeasurement`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `biomaterialmeasurement` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `BioMaterial` bigint(20) DEFAULT NULL,
'''   `Measurement` bigint(20) DEFAULT NULL,
'''   `Treatment` bigint(20) DEFAULT NULL,
'''   `BioAssayCreation` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_BioMaterialMeasurement1` (`DataSetWID`),
'''   KEY `FK_BioMaterialMeasurement2` (`BioMaterial`),
'''   KEY `FK_BioMaterialMeasurement3` (`Measurement`),
'''   KEY `FK_BioMaterialMeasurement4` (`Treatment`),
'''   KEY `FK_BioMaterialMeasurement5` (`BioAssayCreation`),
'''   CONSTRAINT `FK_BioMaterialMeasurement1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioMaterialMeasurement2` FOREIGN KEY (`BioMaterial`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioMaterialMeasurement3` FOREIGN KEY (`Measurement`) REFERENCES `measurement` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioMaterialMeasurement4` FOREIGN KEY (`Treatment`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioMaterialMeasurement5` FOREIGN KEY (`BioAssayCreation`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("biomaterialmeasurement", Database:="warehouse", SchemaSQL:="
CREATE TABLE `biomaterialmeasurement` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `BioMaterial` bigint(20) DEFAULT NULL,
  `Measurement` bigint(20) DEFAULT NULL,
  `Treatment` bigint(20) DEFAULT NULL,
  `BioAssayCreation` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_BioMaterialMeasurement1` (`DataSetWID`),
  KEY `FK_BioMaterialMeasurement2` (`BioMaterial`),
  KEY `FK_BioMaterialMeasurement3` (`Measurement`),
  KEY `FK_BioMaterialMeasurement4` (`Treatment`),
  KEY `FK_BioMaterialMeasurement5` (`BioAssayCreation`),
  CONSTRAINT `FK_BioMaterialMeasurement1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioMaterialMeasurement2` FOREIGN KEY (`BioMaterial`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioMaterialMeasurement3` FOREIGN KEY (`Measurement`) REFERENCES `measurement` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioMaterialMeasurement4` FOREIGN KEY (`Treatment`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioMaterialMeasurement5` FOREIGN KEY (`BioAssayCreation`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class biomaterialmeasurement: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DataSetWID")> Public Property DataSetWID As Long
    <DatabaseField("BioMaterial"), DataType(MySqlDbType.Int64, "20"), Column(Name:="BioMaterial")> Public Property BioMaterial As Long
    <DatabaseField("Measurement"), DataType(MySqlDbType.Int64, "20"), Column(Name:="Measurement")> Public Property Measurement As Long
    <DatabaseField("Treatment"), DataType(MySqlDbType.Int64, "20"), Column(Name:="Treatment")> Public Property Treatment As Long
    <DatabaseField("BioAssayCreation"), DataType(MySqlDbType.Int64, "20"), Column(Name:="BioAssayCreation")> Public Property BioAssayCreation As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `biomaterialmeasurement` (`WID`, `DataSetWID`, `BioMaterial`, `Measurement`, `Treatment`, `BioAssayCreation`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `biomaterialmeasurement` (`WID`, `DataSetWID`, `BioMaterial`, `Measurement`, `Treatment`, `BioAssayCreation`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `biomaterialmeasurement` (`WID`, `DataSetWID`, `BioMaterial`, `Measurement`, `Treatment`, `BioAssayCreation`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `biomaterialmeasurement` (`WID`, `DataSetWID`, `BioMaterial`, `Measurement`, `Treatment`, `BioAssayCreation`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `biomaterialmeasurement` WHERE `WID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `biomaterialmeasurement` SET `WID`='{0}', `DataSetWID`='{1}', `BioMaterial`='{2}', `Measurement`='{3}', `Treatment`='{4}', `BioAssayCreation`='{5}' WHERE `WID` = '{6}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `biomaterialmeasurement` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `biomaterialmeasurement` (`WID`, `DataSetWID`, `BioMaterial`, `Measurement`, `Treatment`, `BioAssayCreation`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, BioMaterial, Measurement, Treatment, BioAssayCreation)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `biomaterialmeasurement` (`WID`, `DataSetWID`, `BioMaterial`, `Measurement`, `Treatment`, `BioAssayCreation`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, WID, DataSetWID, BioMaterial, Measurement, Treatment, BioAssayCreation)
        Else
        Return String.Format(INSERT_SQL, WID, DataSetWID, BioMaterial, Measurement, Treatment, BioAssayCreation)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{WID}', '{DataSetWID}', '{BioMaterial}', '{Measurement}', '{Treatment}', '{BioAssayCreation}')"
        Else
            Return $"('{WID}', '{DataSetWID}', '{BioMaterial}', '{Measurement}', '{Treatment}', '{BioAssayCreation}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `biomaterialmeasurement` (`WID`, `DataSetWID`, `BioMaterial`, `Measurement`, `Treatment`, `BioAssayCreation`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, BioMaterial, Measurement, Treatment, BioAssayCreation)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `biomaterialmeasurement` (`WID`, `DataSetWID`, `BioMaterial`, `Measurement`, `Treatment`, `BioAssayCreation`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, WID, DataSetWID, BioMaterial, Measurement, Treatment, BioAssayCreation)
        Else
        Return String.Format(REPLACE_SQL, WID, DataSetWID, BioMaterial, Measurement, Treatment, BioAssayCreation)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `biomaterialmeasurement` SET `WID`='{0}', `DataSetWID`='{1}', `BioMaterial`='{2}', `Measurement`='{3}', `Treatment`='{4}', `BioAssayCreation`='{5}' WHERE `WID` = '{6}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, BioMaterial, Measurement, Treatment, BioAssayCreation, WID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As biomaterialmeasurement
                         Return DirectCast(MyClass.MemberwiseClone, biomaterialmeasurement)
                     End Function
End Class


End Namespace
