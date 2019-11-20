#Region "Microsoft.VisualBasic::b7adc6666325eb8ece48db893c9311a5, data\ExternalDBSource\MetaCyc\MySQL\bioevent.vb"

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

    ' Class bioevent
    ' 
    '     Properties: Array_, BioAssayMapTarget, BioMaterial_Treatments, CompositeSequence, CompositeSequence2
    '                 DataSetWID, DerivedBioAssayDataTarget, DesignElementMapping, Identifier, MAGEClass
    '                 MeasuredBioAssayTarget, Name, Order_, PhysicalBioAssay, PhysicalBioAssay2
    '                 PhysicalBioAssaySource, PhysicalBioAssayTarget, QuantitationTypeMapping, Reporter, Target
    '                 TargetQuantitationType, Transformation_BioAssayMapping, Treatment_Action, Treatment_ActionMeasurement, WID
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
''' DROP TABLE IF EXISTS `bioevent`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `bioevent` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `MAGEClass` varchar(100) NOT NULL,
'''   `Identifier` varchar(255) DEFAULT NULL,
'''   `Name` varchar(255) DEFAULT NULL,
'''   `CompositeSequence` bigint(20) DEFAULT NULL,
'''   `Reporter` bigint(20) DEFAULT NULL,
'''   `CompositeSequence2` bigint(20) DEFAULT NULL,
'''   `BioAssayMapTarget` bigint(20) DEFAULT NULL,
'''   `TargetQuantitationType` bigint(20) DEFAULT NULL,
'''   `DerivedBioAssayDataTarget` bigint(20) DEFAULT NULL,
'''   `QuantitationTypeMapping` bigint(20) DEFAULT NULL,
'''   `DesignElementMapping` bigint(20) DEFAULT NULL,
'''   `Transformation_BioAssayMapping` bigint(20) DEFAULT NULL,
'''   `BioMaterial_Treatments` bigint(20) DEFAULT NULL,
'''   `Order_` smallint(6) DEFAULT NULL,
'''   `Treatment_Action` bigint(20) DEFAULT NULL,
'''   `Treatment_ActionMeasurement` bigint(20) DEFAULT NULL,
'''   `Array_` bigint(20) DEFAULT NULL,
'''   `PhysicalBioAssayTarget` bigint(20) DEFAULT NULL,
'''   `PhysicalBioAssay` bigint(20) DEFAULT NULL,
'''   `Target` bigint(20) DEFAULT NULL,
'''   `PhysicalBioAssaySource` bigint(20) DEFAULT NULL,
'''   `MeasuredBioAssayTarget` bigint(20) DEFAULT NULL,
'''   `PhysicalBioAssay2` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_BioEvent1` (`DataSetWID`),
'''   KEY `FK_BioEvent3` (`CompositeSequence`),
'''   KEY `FK_BioEvent4` (`Reporter`),
'''   KEY `FK_BioEvent5` (`CompositeSequence2`),
'''   KEY `FK_BioEvent6` (`BioAssayMapTarget`),
'''   KEY `FK_BioEvent7` (`TargetQuantitationType`),
'''   KEY `FK_BioEvent8` (`DerivedBioAssayDataTarget`),
'''   KEY `FK_BioEvent9` (`QuantitationTypeMapping`),
'''   KEY `FK_BioEvent10` (`DesignElementMapping`),
'''   KEY `FK_BioEvent11` (`Transformation_BioAssayMapping`),
'''   KEY `FK_BioEvent12` (`BioMaterial_Treatments`),
'''   KEY `FK_BioEvent13` (`Treatment_Action`),
'''   KEY `FK_BioEvent14` (`Treatment_ActionMeasurement`),
'''   KEY `FK_BioEvent15` (`Array_`),
'''   KEY `FK_BioEvent16` (`PhysicalBioAssayTarget`),
'''   KEY `FK_BioEvent17` (`PhysicalBioAssay`),
'''   KEY `FK_BioEvent18` (`Target`),
'''   KEY `FK_BioEvent19` (`PhysicalBioAssaySource`),
'''   KEY `FK_BioEvent20` (`MeasuredBioAssayTarget`),
'''   KEY `FK_BioEvent21` (`PhysicalBioAssay2`),
'''   CONSTRAINT `FK_BioEvent1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioEvent10` FOREIGN KEY (`DesignElementMapping`) REFERENCES `designelementmapping` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioEvent11` FOREIGN KEY (`Transformation_BioAssayMapping`) REFERENCES `bioassaymapping` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioEvent12` FOREIGN KEY (`BioMaterial_Treatments`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioEvent13` FOREIGN KEY (`Treatment_Action`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioEvent14` FOREIGN KEY (`Treatment_ActionMeasurement`) REFERENCES `measurement` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioEvent15` FOREIGN KEY (`Array_`) REFERENCES `array_` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioEvent16` FOREIGN KEY (`PhysicalBioAssayTarget`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioEvent17` FOREIGN KEY (`PhysicalBioAssay`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioEvent18` FOREIGN KEY (`Target`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioEvent19` FOREIGN KEY (`PhysicalBioAssaySource`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioEvent20` FOREIGN KEY (`MeasuredBioAssayTarget`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioEvent21` FOREIGN KEY (`PhysicalBioAssay2`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioEvent3` FOREIGN KEY (`CompositeSequence`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioEvent4` FOREIGN KEY (`Reporter`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioEvent5` FOREIGN KEY (`CompositeSequence2`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioEvent6` FOREIGN KEY (`BioAssayMapTarget`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioEvent7` FOREIGN KEY (`TargetQuantitationType`) REFERENCES `quantitationtype` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioEvent8` FOREIGN KEY (`DerivedBioAssayDataTarget`) REFERENCES `bioassaydata` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioEvent9` FOREIGN KEY (`QuantitationTypeMapping`) REFERENCES `quantitationtypemapping` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("bioevent", Database:="warehouse", SchemaSQL:="
CREATE TABLE `bioevent` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `MAGEClass` varchar(100) NOT NULL,
  `Identifier` varchar(255) DEFAULT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `CompositeSequence` bigint(20) DEFAULT NULL,
  `Reporter` bigint(20) DEFAULT NULL,
  `CompositeSequence2` bigint(20) DEFAULT NULL,
  `BioAssayMapTarget` bigint(20) DEFAULT NULL,
  `TargetQuantitationType` bigint(20) DEFAULT NULL,
  `DerivedBioAssayDataTarget` bigint(20) DEFAULT NULL,
  `QuantitationTypeMapping` bigint(20) DEFAULT NULL,
  `DesignElementMapping` bigint(20) DEFAULT NULL,
  `Transformation_BioAssayMapping` bigint(20) DEFAULT NULL,
  `BioMaterial_Treatments` bigint(20) DEFAULT NULL,
  `Order_` smallint(6) DEFAULT NULL,
  `Treatment_Action` bigint(20) DEFAULT NULL,
  `Treatment_ActionMeasurement` bigint(20) DEFAULT NULL,
  `Array_` bigint(20) DEFAULT NULL,
  `PhysicalBioAssayTarget` bigint(20) DEFAULT NULL,
  `PhysicalBioAssay` bigint(20) DEFAULT NULL,
  `Target` bigint(20) DEFAULT NULL,
  `PhysicalBioAssaySource` bigint(20) DEFAULT NULL,
  `MeasuredBioAssayTarget` bigint(20) DEFAULT NULL,
  `PhysicalBioAssay2` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_BioEvent1` (`DataSetWID`),
  KEY `FK_BioEvent3` (`CompositeSequence`),
  KEY `FK_BioEvent4` (`Reporter`),
  KEY `FK_BioEvent5` (`CompositeSequence2`),
  KEY `FK_BioEvent6` (`BioAssayMapTarget`),
  KEY `FK_BioEvent7` (`TargetQuantitationType`),
  KEY `FK_BioEvent8` (`DerivedBioAssayDataTarget`),
  KEY `FK_BioEvent9` (`QuantitationTypeMapping`),
  KEY `FK_BioEvent10` (`DesignElementMapping`),
  KEY `FK_BioEvent11` (`Transformation_BioAssayMapping`),
  KEY `FK_BioEvent12` (`BioMaterial_Treatments`),
  KEY `FK_BioEvent13` (`Treatment_Action`),
  KEY `FK_BioEvent14` (`Treatment_ActionMeasurement`),
  KEY `FK_BioEvent15` (`Array_`),
  KEY `FK_BioEvent16` (`PhysicalBioAssayTarget`),
  KEY `FK_BioEvent17` (`PhysicalBioAssay`),
  KEY `FK_BioEvent18` (`Target`),
  KEY `FK_BioEvent19` (`PhysicalBioAssaySource`),
  KEY `FK_BioEvent20` (`MeasuredBioAssayTarget`),
  KEY `FK_BioEvent21` (`PhysicalBioAssay2`),
  CONSTRAINT `FK_BioEvent1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent10` FOREIGN KEY (`DesignElementMapping`) REFERENCES `designelementmapping` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent11` FOREIGN KEY (`Transformation_BioAssayMapping`) REFERENCES `bioassaymapping` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent12` FOREIGN KEY (`BioMaterial_Treatments`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent13` FOREIGN KEY (`Treatment_Action`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent14` FOREIGN KEY (`Treatment_ActionMeasurement`) REFERENCES `measurement` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent15` FOREIGN KEY (`Array_`) REFERENCES `array_` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent16` FOREIGN KEY (`PhysicalBioAssayTarget`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent17` FOREIGN KEY (`PhysicalBioAssay`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent18` FOREIGN KEY (`Target`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent19` FOREIGN KEY (`PhysicalBioAssaySource`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent20` FOREIGN KEY (`MeasuredBioAssayTarget`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent21` FOREIGN KEY (`PhysicalBioAssay2`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent3` FOREIGN KEY (`CompositeSequence`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent4` FOREIGN KEY (`Reporter`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent5` FOREIGN KEY (`CompositeSequence2`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent6` FOREIGN KEY (`BioAssayMapTarget`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent7` FOREIGN KEY (`TargetQuantitationType`) REFERENCES `quantitationtype` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent8` FOREIGN KEY (`DerivedBioAssayDataTarget`) REFERENCES `bioassaydata` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioEvent9` FOREIGN KEY (`QuantitationTypeMapping`) REFERENCES `quantitationtypemapping` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class bioevent: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DataSetWID")> Public Property DataSetWID As Long
    <DatabaseField("MAGEClass"), NotNull, DataType(MySqlDbType.VarChar, "100"), Column(Name:="MAGEClass")> Public Property MAGEClass As String
    <DatabaseField("Identifier"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="Identifier")> Public Property Identifier As String
    <DatabaseField("Name"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="Name")> Public Property Name As String
    <DatabaseField("CompositeSequence"), DataType(MySqlDbType.Int64, "20"), Column(Name:="CompositeSequence")> Public Property CompositeSequence As Long
    <DatabaseField("Reporter"), DataType(MySqlDbType.Int64, "20"), Column(Name:="Reporter")> Public Property Reporter As Long
    <DatabaseField("CompositeSequence2"), DataType(MySqlDbType.Int64, "20"), Column(Name:="CompositeSequence2")> Public Property CompositeSequence2 As Long
    <DatabaseField("BioAssayMapTarget"), DataType(MySqlDbType.Int64, "20"), Column(Name:="BioAssayMapTarget")> Public Property BioAssayMapTarget As Long
    <DatabaseField("TargetQuantitationType"), DataType(MySqlDbType.Int64, "20"), Column(Name:="TargetQuantitationType")> Public Property TargetQuantitationType As Long
    <DatabaseField("DerivedBioAssayDataTarget"), DataType(MySqlDbType.Int64, "20"), Column(Name:="DerivedBioAssayDataTarget")> Public Property DerivedBioAssayDataTarget As Long
    <DatabaseField("QuantitationTypeMapping"), DataType(MySqlDbType.Int64, "20"), Column(Name:="QuantitationTypeMapping")> Public Property QuantitationTypeMapping As Long
    <DatabaseField("DesignElementMapping"), DataType(MySqlDbType.Int64, "20"), Column(Name:="DesignElementMapping")> Public Property DesignElementMapping As Long
    <DatabaseField("Transformation_BioAssayMapping"), DataType(MySqlDbType.Int64, "20"), Column(Name:="Transformation_BioAssayMapping")> Public Property Transformation_BioAssayMapping As Long
    <DatabaseField("BioMaterial_Treatments"), DataType(MySqlDbType.Int64, "20"), Column(Name:="BioMaterial_Treatments")> Public Property BioMaterial_Treatments As Long
    <DatabaseField("Order_"), DataType(MySqlDbType.Int64, "6"), Column(Name:="Order_")> Public Property Order_ As Long
    <DatabaseField("Treatment_Action"), DataType(MySqlDbType.Int64, "20"), Column(Name:="Treatment_Action")> Public Property Treatment_Action As Long
    <DatabaseField("Treatment_ActionMeasurement"), DataType(MySqlDbType.Int64, "20"), Column(Name:="Treatment_ActionMeasurement")> Public Property Treatment_ActionMeasurement As Long
    <DatabaseField("Array_"), DataType(MySqlDbType.Int64, "20"), Column(Name:="Array_")> Public Property Array_ As Long
    <DatabaseField("PhysicalBioAssayTarget"), DataType(MySqlDbType.Int64, "20"), Column(Name:="PhysicalBioAssayTarget")> Public Property PhysicalBioAssayTarget As Long
    <DatabaseField("PhysicalBioAssay"), DataType(MySqlDbType.Int64, "20"), Column(Name:="PhysicalBioAssay")> Public Property PhysicalBioAssay As Long
    <DatabaseField("Target"), DataType(MySqlDbType.Int64, "20"), Column(Name:="Target")> Public Property Target As Long
    <DatabaseField("PhysicalBioAssaySource"), DataType(MySqlDbType.Int64, "20"), Column(Name:="PhysicalBioAssaySource")> Public Property PhysicalBioAssaySource As Long
    <DatabaseField("MeasuredBioAssayTarget"), DataType(MySqlDbType.Int64, "20"), Column(Name:="MeasuredBioAssayTarget")> Public Property MeasuredBioAssayTarget As Long
    <DatabaseField("PhysicalBioAssay2"), DataType(MySqlDbType.Int64, "20"), Column(Name:="PhysicalBioAssay2")> Public Property PhysicalBioAssay2 As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `bioevent` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `CompositeSequence`, `Reporter`, `CompositeSequence2`, `BioAssayMapTarget`, `TargetQuantitationType`, `DerivedBioAssayDataTarget`, `QuantitationTypeMapping`, `DesignElementMapping`, `Transformation_BioAssayMapping`, `BioMaterial_Treatments`, `Order_`, `Treatment_Action`, `Treatment_ActionMeasurement`, `Array_`, `PhysicalBioAssayTarget`, `PhysicalBioAssay`, `Target`, `PhysicalBioAssaySource`, `MeasuredBioAssayTarget`, `PhysicalBioAssay2`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `bioevent` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `CompositeSequence`, `Reporter`, `CompositeSequence2`, `BioAssayMapTarget`, `TargetQuantitationType`, `DerivedBioAssayDataTarget`, `QuantitationTypeMapping`, `DesignElementMapping`, `Transformation_BioAssayMapping`, `BioMaterial_Treatments`, `Order_`, `Treatment_Action`, `Treatment_ActionMeasurement`, `Array_`, `PhysicalBioAssayTarget`, `PhysicalBioAssay`, `Target`, `PhysicalBioAssaySource`, `MeasuredBioAssayTarget`, `PhysicalBioAssay2`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `bioevent` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `CompositeSequence`, `Reporter`, `CompositeSequence2`, `BioAssayMapTarget`, `TargetQuantitationType`, `DerivedBioAssayDataTarget`, `QuantitationTypeMapping`, `DesignElementMapping`, `Transformation_BioAssayMapping`, `BioMaterial_Treatments`, `Order_`, `Treatment_Action`, `Treatment_ActionMeasurement`, `Array_`, `PhysicalBioAssayTarget`, `PhysicalBioAssay`, `Target`, `PhysicalBioAssaySource`, `MeasuredBioAssayTarget`, `PhysicalBioAssay2`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `bioevent` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `CompositeSequence`, `Reporter`, `CompositeSequence2`, `BioAssayMapTarget`, `TargetQuantitationType`, `DerivedBioAssayDataTarget`, `QuantitationTypeMapping`, `DesignElementMapping`, `Transformation_BioAssayMapping`, `BioMaterial_Treatments`, `Order_`, `Treatment_Action`, `Treatment_ActionMeasurement`, `Array_`, `PhysicalBioAssayTarget`, `PhysicalBioAssay`, `Target`, `PhysicalBioAssaySource`, `MeasuredBioAssayTarget`, `PhysicalBioAssay2`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `bioevent` WHERE `WID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `bioevent` SET `WID`='{0}', `DataSetWID`='{1}', `MAGEClass`='{2}', `Identifier`='{3}', `Name`='{4}', `CompositeSequence`='{5}', `Reporter`='{6}', `CompositeSequence2`='{7}', `BioAssayMapTarget`='{8}', `TargetQuantitationType`='{9}', `DerivedBioAssayDataTarget`='{10}', `QuantitationTypeMapping`='{11}', `DesignElementMapping`='{12}', `Transformation_BioAssayMapping`='{13}', `BioMaterial_Treatments`='{14}', `Order_`='{15}', `Treatment_Action`='{16}', `Treatment_ActionMeasurement`='{17}', `Array_`='{18}', `PhysicalBioAssayTarget`='{19}', `PhysicalBioAssay`='{20}', `Target`='{21}', `PhysicalBioAssaySource`='{22}', `MeasuredBioAssayTarget`='{23}', `PhysicalBioAssay2`='{24}' WHERE `WID` = '{25}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `bioevent` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `bioevent` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `CompositeSequence`, `Reporter`, `CompositeSequence2`, `BioAssayMapTarget`, `TargetQuantitationType`, `DerivedBioAssayDataTarget`, `QuantitationTypeMapping`, `DesignElementMapping`, `Transformation_BioAssayMapping`, `BioMaterial_Treatments`, `Order_`, `Treatment_Action`, `Treatment_ActionMeasurement`, `Array_`, `PhysicalBioAssayTarget`, `PhysicalBioAssay`, `Target`, `PhysicalBioAssaySource`, `MeasuredBioAssayTarget`, `PhysicalBioAssay2`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, CompositeSequence, Reporter, CompositeSequence2, BioAssayMapTarget, TargetQuantitationType, DerivedBioAssayDataTarget, QuantitationTypeMapping, DesignElementMapping, Transformation_BioAssayMapping, BioMaterial_Treatments, Order_, Treatment_Action, Treatment_ActionMeasurement, Array_, PhysicalBioAssayTarget, PhysicalBioAssay, Target, PhysicalBioAssaySource, MeasuredBioAssayTarget, PhysicalBioAssay2)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `bioevent` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `CompositeSequence`, `Reporter`, `CompositeSequence2`, `BioAssayMapTarget`, `TargetQuantitationType`, `DerivedBioAssayDataTarget`, `QuantitationTypeMapping`, `DesignElementMapping`, `Transformation_BioAssayMapping`, `BioMaterial_Treatments`, `Order_`, `Treatment_Action`, `Treatment_ActionMeasurement`, `Array_`, `PhysicalBioAssayTarget`, `PhysicalBioAssay`, `Target`, `PhysicalBioAssaySource`, `MeasuredBioAssayTarget`, `PhysicalBioAssay2`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, CompositeSequence, Reporter, CompositeSequence2, BioAssayMapTarget, TargetQuantitationType, DerivedBioAssayDataTarget, QuantitationTypeMapping, DesignElementMapping, Transformation_BioAssayMapping, BioMaterial_Treatments, Order_, Treatment_Action, Treatment_ActionMeasurement, Array_, PhysicalBioAssayTarget, PhysicalBioAssay, Target, PhysicalBioAssaySource, MeasuredBioAssayTarget, PhysicalBioAssay2)
        Else
        Return String.Format(INSERT_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, CompositeSequence, Reporter, CompositeSequence2, BioAssayMapTarget, TargetQuantitationType, DerivedBioAssayDataTarget, QuantitationTypeMapping, DesignElementMapping, Transformation_BioAssayMapping, BioMaterial_Treatments, Order_, Treatment_Action, Treatment_ActionMeasurement, Array_, PhysicalBioAssayTarget, PhysicalBioAssay, Target, PhysicalBioAssaySource, MeasuredBioAssayTarget, PhysicalBioAssay2)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{WID}', '{DataSetWID}', '{MAGEClass}', '{Identifier}', '{Name}', '{CompositeSequence}', '{Reporter}', '{CompositeSequence2}', '{BioAssayMapTarget}', '{TargetQuantitationType}', '{DerivedBioAssayDataTarget}', '{QuantitationTypeMapping}', '{DesignElementMapping}', '{Transformation_BioAssayMapping}', '{BioMaterial_Treatments}', '{Order_}', '{Treatment_Action}', '{Treatment_ActionMeasurement}', '{Array_}', '{PhysicalBioAssayTarget}', '{PhysicalBioAssay}', '{Target}', '{PhysicalBioAssaySource}', '{MeasuredBioAssayTarget}', '{PhysicalBioAssay2}')"
        Else
            Return $"('{WID}', '{DataSetWID}', '{MAGEClass}', '{Identifier}', '{Name}', '{CompositeSequence}', '{Reporter}', '{CompositeSequence2}', '{BioAssayMapTarget}', '{TargetQuantitationType}', '{DerivedBioAssayDataTarget}', '{QuantitationTypeMapping}', '{DesignElementMapping}', '{Transformation_BioAssayMapping}', '{BioMaterial_Treatments}', '{Order_}', '{Treatment_Action}', '{Treatment_ActionMeasurement}', '{Array_}', '{PhysicalBioAssayTarget}', '{PhysicalBioAssay}', '{Target}', '{PhysicalBioAssaySource}', '{MeasuredBioAssayTarget}', '{PhysicalBioAssay2}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `bioevent` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `CompositeSequence`, `Reporter`, `CompositeSequence2`, `BioAssayMapTarget`, `TargetQuantitationType`, `DerivedBioAssayDataTarget`, `QuantitationTypeMapping`, `DesignElementMapping`, `Transformation_BioAssayMapping`, `BioMaterial_Treatments`, `Order_`, `Treatment_Action`, `Treatment_ActionMeasurement`, `Array_`, `PhysicalBioAssayTarget`, `PhysicalBioAssay`, `Target`, `PhysicalBioAssaySource`, `MeasuredBioAssayTarget`, `PhysicalBioAssay2`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, CompositeSequence, Reporter, CompositeSequence2, BioAssayMapTarget, TargetQuantitationType, DerivedBioAssayDataTarget, QuantitationTypeMapping, DesignElementMapping, Transformation_BioAssayMapping, BioMaterial_Treatments, Order_, Treatment_Action, Treatment_ActionMeasurement, Array_, PhysicalBioAssayTarget, PhysicalBioAssay, Target, PhysicalBioAssaySource, MeasuredBioAssayTarget, PhysicalBioAssay2)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `bioevent` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `CompositeSequence`, `Reporter`, `CompositeSequence2`, `BioAssayMapTarget`, `TargetQuantitationType`, `DerivedBioAssayDataTarget`, `QuantitationTypeMapping`, `DesignElementMapping`, `Transformation_BioAssayMapping`, `BioMaterial_Treatments`, `Order_`, `Treatment_Action`, `Treatment_ActionMeasurement`, `Array_`, `PhysicalBioAssayTarget`, `PhysicalBioAssay`, `Target`, `PhysicalBioAssaySource`, `MeasuredBioAssayTarget`, `PhysicalBioAssay2`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, CompositeSequence, Reporter, CompositeSequence2, BioAssayMapTarget, TargetQuantitationType, DerivedBioAssayDataTarget, QuantitationTypeMapping, DesignElementMapping, Transformation_BioAssayMapping, BioMaterial_Treatments, Order_, Treatment_Action, Treatment_ActionMeasurement, Array_, PhysicalBioAssayTarget, PhysicalBioAssay, Target, PhysicalBioAssaySource, MeasuredBioAssayTarget, PhysicalBioAssay2)
        Else
        Return String.Format(REPLACE_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, CompositeSequence, Reporter, CompositeSequence2, BioAssayMapTarget, TargetQuantitationType, DerivedBioAssayDataTarget, QuantitationTypeMapping, DesignElementMapping, Transformation_BioAssayMapping, BioMaterial_Treatments, Order_, Treatment_Action, Treatment_ActionMeasurement, Array_, PhysicalBioAssayTarget, PhysicalBioAssay, Target, PhysicalBioAssaySource, MeasuredBioAssayTarget, PhysicalBioAssay2)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `bioevent` SET `WID`='{0}', `DataSetWID`='{1}', `MAGEClass`='{2}', `Identifier`='{3}', `Name`='{4}', `CompositeSequence`='{5}', `Reporter`='{6}', `CompositeSequence2`='{7}', `BioAssayMapTarget`='{8}', `TargetQuantitationType`='{9}', `DerivedBioAssayDataTarget`='{10}', `QuantitationTypeMapping`='{11}', `DesignElementMapping`='{12}', `Transformation_BioAssayMapping`='{13}', `BioMaterial_Treatments`='{14}', `Order_`='{15}', `Treatment_Action`='{16}', `Treatment_ActionMeasurement`='{17}', `Array_`='{18}', `PhysicalBioAssayTarget`='{19}', `PhysicalBioAssay`='{20}', `Target`='{21}', `PhysicalBioAssaySource`='{22}', `MeasuredBioAssayTarget`='{23}', `PhysicalBioAssay2`='{24}' WHERE `WID` = '{25}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, CompositeSequence, Reporter, CompositeSequence2, BioAssayMapTarget, TargetQuantitationType, DerivedBioAssayDataTarget, QuantitationTypeMapping, DesignElementMapping, Transformation_BioAssayMapping, BioMaterial_Treatments, Order_, Treatment_Action, Treatment_ActionMeasurement, Array_, PhysicalBioAssayTarget, PhysicalBioAssay, Target, PhysicalBioAssaySource, MeasuredBioAssayTarget, PhysicalBioAssay2, WID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As bioevent
                         Return DirectCast(MyClass.MemberwiseClone, bioevent)
                     End Function
End Class


End Namespace
