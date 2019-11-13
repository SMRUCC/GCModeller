#Region "Microsoft.VisualBasic::021316de4f926e606de3b5001f69fd15, data\ExternalDBSource\MetaCyc\MySQL\bioassay.vb"

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

    ' Class bioassay
    ' 
    '     Properties: BioAssayCreation, DataSetWID, DerivedBioAssay_Type, FeatureExtraction, Identifier
    '                 MAGEClass, Name, WID
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
''' DROP TABLE IF EXISTS `bioassay`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `bioassay` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `MAGEClass` varchar(100) NOT NULL,
'''   `Identifier` varchar(255) DEFAULT NULL,
'''   `Name` varchar(255) DEFAULT NULL,
'''   `DerivedBioAssay_Type` bigint(20) DEFAULT NULL,
'''   `FeatureExtraction` bigint(20) DEFAULT NULL,
'''   `BioAssayCreation` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_BioAssay1` (`DataSetWID`),
'''   KEY `FK_BioAssay3` (`DerivedBioAssay_Type`),
'''   KEY `FK_BioAssay4` (`FeatureExtraction`),
'''   KEY `FK_BioAssay5` (`BioAssayCreation`),
'''   CONSTRAINT `FK_BioAssay1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioAssay3` FOREIGN KEY (`DerivedBioAssay_Type`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioAssay4` FOREIGN KEY (`FeatureExtraction`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioAssay5` FOREIGN KEY (`BioAssayCreation`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("bioassay", Database:="warehouse", SchemaSQL:="
CREATE TABLE `bioassay` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `MAGEClass` varchar(100) NOT NULL,
  `Identifier` varchar(255) DEFAULT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `DerivedBioAssay_Type` bigint(20) DEFAULT NULL,
  `FeatureExtraction` bigint(20) DEFAULT NULL,
  `BioAssayCreation` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_BioAssay1` (`DataSetWID`),
  KEY `FK_BioAssay3` (`DerivedBioAssay_Type`),
  KEY `FK_BioAssay4` (`FeatureExtraction`),
  KEY `FK_BioAssay5` (`BioAssayCreation`),
  CONSTRAINT `FK_BioAssay1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioAssay3` FOREIGN KEY (`DerivedBioAssay_Type`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioAssay4` FOREIGN KEY (`FeatureExtraction`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioAssay5` FOREIGN KEY (`BioAssayCreation`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class bioassay: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DataSetWID")> Public Property DataSetWID As Long
    <DatabaseField("MAGEClass"), NotNull, DataType(MySqlDbType.VarChar, "100"), Column(Name:="MAGEClass")> Public Property MAGEClass As String
    <DatabaseField("Identifier"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="Identifier")> Public Property Identifier As String
    <DatabaseField("Name"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="Name")> Public Property Name As String
    <DatabaseField("DerivedBioAssay_Type"), DataType(MySqlDbType.Int64, "20"), Column(Name:="DerivedBioAssay_Type")> Public Property DerivedBioAssay_Type As Long
    <DatabaseField("FeatureExtraction"), DataType(MySqlDbType.Int64, "20"), Column(Name:="FeatureExtraction")> Public Property FeatureExtraction As Long
    <DatabaseField("BioAssayCreation"), DataType(MySqlDbType.Int64, "20"), Column(Name:="BioAssayCreation")> Public Property BioAssayCreation As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `bioassay` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `DerivedBioAssay_Type`, `FeatureExtraction`, `BioAssayCreation`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `bioassay` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `DerivedBioAssay_Type`, `FeatureExtraction`, `BioAssayCreation`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `bioassay` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `DerivedBioAssay_Type`, `FeatureExtraction`, `BioAssayCreation`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `bioassay` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `DerivedBioAssay_Type`, `FeatureExtraction`, `BioAssayCreation`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `bioassay` WHERE `WID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `bioassay` SET `WID`='{0}', `DataSetWID`='{1}', `MAGEClass`='{2}', `Identifier`='{3}', `Name`='{4}', `DerivedBioAssay_Type`='{5}', `FeatureExtraction`='{6}', `BioAssayCreation`='{7}' WHERE `WID` = '{8}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `bioassay` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `bioassay` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `DerivedBioAssay_Type`, `FeatureExtraction`, `BioAssayCreation`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, DerivedBioAssay_Type, FeatureExtraction, BioAssayCreation)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `bioassay` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `DerivedBioAssay_Type`, `FeatureExtraction`, `BioAssayCreation`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, DerivedBioAssay_Type, FeatureExtraction, BioAssayCreation)
        Else
        Return String.Format(INSERT_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, DerivedBioAssay_Type, FeatureExtraction, BioAssayCreation)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{WID}', '{DataSetWID}', '{MAGEClass}', '{Identifier}', '{Name}', '{DerivedBioAssay_Type}', '{FeatureExtraction}', '{BioAssayCreation}')"
        Else
            Return $"('{WID}', '{DataSetWID}', '{MAGEClass}', '{Identifier}', '{Name}', '{DerivedBioAssay_Type}', '{FeatureExtraction}', '{BioAssayCreation}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `bioassay` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `DerivedBioAssay_Type`, `FeatureExtraction`, `BioAssayCreation`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, DerivedBioAssay_Type, FeatureExtraction, BioAssayCreation)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `bioassay` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `DerivedBioAssay_Type`, `FeatureExtraction`, `BioAssayCreation`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, DerivedBioAssay_Type, FeatureExtraction, BioAssayCreation)
        Else
        Return String.Format(REPLACE_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, DerivedBioAssay_Type, FeatureExtraction, BioAssayCreation)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `bioassay` SET `WID`='{0}', `DataSetWID`='{1}', `MAGEClass`='{2}', `Identifier`='{3}', `Name`='{4}', `DerivedBioAssay_Type`='{5}', `FeatureExtraction`='{6}', `BioAssayCreation`='{7}' WHERE `WID` = '{8}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, DerivedBioAssay_Type, FeatureExtraction, BioAssayCreation, WID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As bioassay
                         Return DirectCast(MyClass.MemberwiseClone, bioassay)
                     End Function
End Class


End Namespace
