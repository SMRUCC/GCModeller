#Region "Microsoft.VisualBasic::ef4e9132451f09e950c52811d0f9f040, data\ExternalDBSource\MetaCyc\MySQL\parameterizableapplication.vb"

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

    ' Class parameterizableapplication
    ' 
    '     Properties: ActivityDate, ArrayDesign, ArrayManufacture, BioEvent_ProtocolApplications, DataSetWID
    '                 Hardware, MAGEClass, Protocol, ProtocolApplication, ProtocolApplication2
    '                 ReleaseDate, SerialNumber, Software, Version, WID
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
''' DROP TABLE IF EXISTS `parameterizableapplication`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `parameterizableapplication` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `MAGEClass` varchar(100) NOT NULL,
'''   `ArrayDesign` bigint(20) DEFAULT NULL,
'''   `ArrayManufacture` bigint(20) DEFAULT NULL,
'''   `BioEvent_ProtocolApplications` bigint(20) DEFAULT NULL,
'''   `SerialNumber` varchar(255) DEFAULT NULL,
'''   `Hardware` bigint(20) DEFAULT NULL,
'''   `ActivityDate` varchar(255) DEFAULT NULL,
'''   `ProtocolApplication` bigint(20) DEFAULT NULL,
'''   `ProtocolApplication2` bigint(20) DEFAULT NULL,
'''   `Protocol` bigint(20) DEFAULT NULL,
'''   `Version` varchar(255) DEFAULT NULL,
'''   `ReleaseDate` datetime DEFAULT NULL,
'''   `Software` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_ParameterizableApplicatio1` (`DataSetWID`),
'''   KEY `FK_ParameterizableApplicatio3` (`ArrayDesign`),
'''   KEY `FK_ParameterizableApplicatio4` (`ArrayManufacture`),
'''   KEY `FK_ParameterizableApplicatio5` (`BioEvent_ProtocolApplications`),
'''   KEY `FK_ParameterizableApplicatio6` (`Hardware`),
'''   KEY `FK_ParameterizableApplicatio7` (`ProtocolApplication`),
'''   KEY `FK_ParameterizableApplicatio8` (`ProtocolApplication2`),
'''   KEY `FK_ParameterizableApplicatio9` (`Protocol`),
'''   KEY `FK_ParameterizableApplicatio10` (`Software`),
'''   CONSTRAINT `FK_ParameterizableApplicatio1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_ParameterizableApplicatio10` FOREIGN KEY (`Software`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_ParameterizableApplicatio3` FOREIGN KEY (`ArrayDesign`) REFERENCES `arraydesign` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_ParameterizableApplicatio4` FOREIGN KEY (`ArrayManufacture`) REFERENCES `arraymanufacture` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_ParameterizableApplicatio5` FOREIGN KEY (`BioEvent_ProtocolApplications`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_ParameterizableApplicatio6` FOREIGN KEY (`Hardware`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_ParameterizableApplicatio7` FOREIGN KEY (`ProtocolApplication`) REFERENCES `parameterizableapplication` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_ParameterizableApplicatio8` FOREIGN KEY (`ProtocolApplication2`) REFERENCES `parameterizableapplication` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_ParameterizableApplicatio9` FOREIGN KEY (`Protocol`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("parameterizableapplication", Database:="warehouse", SchemaSQL:="
CREATE TABLE `parameterizableapplication` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `MAGEClass` varchar(100) NOT NULL,
  `ArrayDesign` bigint(20) DEFAULT NULL,
  `ArrayManufacture` bigint(20) DEFAULT NULL,
  `BioEvent_ProtocolApplications` bigint(20) DEFAULT NULL,
  `SerialNumber` varchar(255) DEFAULT NULL,
  `Hardware` bigint(20) DEFAULT NULL,
  `ActivityDate` varchar(255) DEFAULT NULL,
  `ProtocolApplication` bigint(20) DEFAULT NULL,
  `ProtocolApplication2` bigint(20) DEFAULT NULL,
  `Protocol` bigint(20) DEFAULT NULL,
  `Version` varchar(255) DEFAULT NULL,
  `ReleaseDate` datetime DEFAULT NULL,
  `Software` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_ParameterizableApplicatio1` (`DataSetWID`),
  KEY `FK_ParameterizableApplicatio3` (`ArrayDesign`),
  KEY `FK_ParameterizableApplicatio4` (`ArrayManufacture`),
  KEY `FK_ParameterizableApplicatio5` (`BioEvent_ProtocolApplications`),
  KEY `FK_ParameterizableApplicatio6` (`Hardware`),
  KEY `FK_ParameterizableApplicatio7` (`ProtocolApplication`),
  KEY `FK_ParameterizableApplicatio8` (`ProtocolApplication2`),
  KEY `FK_ParameterizableApplicatio9` (`Protocol`),
  KEY `FK_ParameterizableApplicatio10` (`Software`),
  CONSTRAINT `FK_ParameterizableApplicatio1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ParameterizableApplicatio10` FOREIGN KEY (`Software`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ParameterizableApplicatio3` FOREIGN KEY (`ArrayDesign`) REFERENCES `arraydesign` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ParameterizableApplicatio4` FOREIGN KEY (`ArrayManufacture`) REFERENCES `arraymanufacture` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ParameterizableApplicatio5` FOREIGN KEY (`BioEvent_ProtocolApplications`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ParameterizableApplicatio6` FOREIGN KEY (`Hardware`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ParameterizableApplicatio7` FOREIGN KEY (`ProtocolApplication`) REFERENCES `parameterizableapplication` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ParameterizableApplicatio8` FOREIGN KEY (`ProtocolApplication2`) REFERENCES `parameterizableapplication` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ParameterizableApplicatio9` FOREIGN KEY (`Protocol`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class parameterizableapplication: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DataSetWID")> Public Property DataSetWID As Long
    <DatabaseField("MAGEClass"), NotNull, DataType(MySqlDbType.VarChar, "100"), Column(Name:="MAGEClass")> Public Property MAGEClass As String
    <DatabaseField("ArrayDesign"), DataType(MySqlDbType.Int64, "20"), Column(Name:="ArrayDesign")> Public Property ArrayDesign As Long
    <DatabaseField("ArrayManufacture"), DataType(MySqlDbType.Int64, "20"), Column(Name:="ArrayManufacture")> Public Property ArrayManufacture As Long
    <DatabaseField("BioEvent_ProtocolApplications"), DataType(MySqlDbType.Int64, "20"), Column(Name:="BioEvent_ProtocolApplications")> Public Property BioEvent_ProtocolApplications As Long
    <DatabaseField("SerialNumber"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="SerialNumber")> Public Property SerialNumber As String
    <DatabaseField("Hardware"), DataType(MySqlDbType.Int64, "20"), Column(Name:="Hardware")> Public Property Hardware As Long
    <DatabaseField("ActivityDate"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="ActivityDate")> Public Property ActivityDate As String
    <DatabaseField("ProtocolApplication"), DataType(MySqlDbType.Int64, "20"), Column(Name:="ProtocolApplication")> Public Property ProtocolApplication As Long
    <DatabaseField("ProtocolApplication2"), DataType(MySqlDbType.Int64, "20"), Column(Name:="ProtocolApplication2")> Public Property ProtocolApplication2 As Long
    <DatabaseField("Protocol"), DataType(MySqlDbType.Int64, "20"), Column(Name:="Protocol")> Public Property Protocol As Long
    <DatabaseField("Version"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="Version")> Public Property Version As String
    <DatabaseField("ReleaseDate"), DataType(MySqlDbType.DateTime), Column(Name:="ReleaseDate")> Public Property ReleaseDate As Date
    <DatabaseField("Software"), DataType(MySqlDbType.Int64, "20"), Column(Name:="Software")> Public Property Software As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `parameterizableapplication` (`WID`, `DataSetWID`, `MAGEClass`, `ArrayDesign`, `ArrayManufacture`, `BioEvent_ProtocolApplications`, `SerialNumber`, `Hardware`, `ActivityDate`, `ProtocolApplication`, `ProtocolApplication2`, `Protocol`, `Version`, `ReleaseDate`, `Software`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `parameterizableapplication` (`WID`, `DataSetWID`, `MAGEClass`, `ArrayDesign`, `ArrayManufacture`, `BioEvent_ProtocolApplications`, `SerialNumber`, `Hardware`, `ActivityDate`, `ProtocolApplication`, `ProtocolApplication2`, `Protocol`, `Version`, `ReleaseDate`, `Software`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `parameterizableapplication` (`WID`, `DataSetWID`, `MAGEClass`, `ArrayDesign`, `ArrayManufacture`, `BioEvent_ProtocolApplications`, `SerialNumber`, `Hardware`, `ActivityDate`, `ProtocolApplication`, `ProtocolApplication2`, `Protocol`, `Version`, `ReleaseDate`, `Software`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `parameterizableapplication` (`WID`, `DataSetWID`, `MAGEClass`, `ArrayDesign`, `ArrayManufacture`, `BioEvent_ProtocolApplications`, `SerialNumber`, `Hardware`, `ActivityDate`, `ProtocolApplication`, `ProtocolApplication2`, `Protocol`, `Version`, `ReleaseDate`, `Software`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `parameterizableapplication` WHERE `WID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `parameterizableapplication` SET `WID`='{0}', `DataSetWID`='{1}', `MAGEClass`='{2}', `ArrayDesign`='{3}', `ArrayManufacture`='{4}', `BioEvent_ProtocolApplications`='{5}', `SerialNumber`='{6}', `Hardware`='{7}', `ActivityDate`='{8}', `ProtocolApplication`='{9}', `ProtocolApplication2`='{10}', `Protocol`='{11}', `Version`='{12}', `ReleaseDate`='{13}', `Software`='{14}' WHERE `WID` = '{15}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `parameterizableapplication` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `parameterizableapplication` (`WID`, `DataSetWID`, `MAGEClass`, `ArrayDesign`, `ArrayManufacture`, `BioEvent_ProtocolApplications`, `SerialNumber`, `Hardware`, `ActivityDate`, `ProtocolApplication`, `ProtocolApplication2`, `Protocol`, `Version`, `ReleaseDate`, `Software`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, MAGEClass, ArrayDesign, ArrayManufacture, BioEvent_ProtocolApplications, SerialNumber, Hardware, ActivityDate, ProtocolApplication, ProtocolApplication2, Protocol, Version, MySqlScript.ToMySqlDateTimeString(ReleaseDate), Software)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `parameterizableapplication` (`WID`, `DataSetWID`, `MAGEClass`, `ArrayDesign`, `ArrayManufacture`, `BioEvent_ProtocolApplications`, `SerialNumber`, `Hardware`, `ActivityDate`, `ProtocolApplication`, `ProtocolApplication2`, `Protocol`, `Version`, `ReleaseDate`, `Software`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, WID, DataSetWID, MAGEClass, ArrayDesign, ArrayManufacture, BioEvent_ProtocolApplications, SerialNumber, Hardware, ActivityDate, ProtocolApplication, ProtocolApplication2, Protocol, Version, MySqlScript.ToMySqlDateTimeString(ReleaseDate), Software)
        Else
        Return String.Format(INSERT_SQL, WID, DataSetWID, MAGEClass, ArrayDesign, ArrayManufacture, BioEvent_ProtocolApplications, SerialNumber, Hardware, ActivityDate, ProtocolApplication, ProtocolApplication2, Protocol, Version, MySqlScript.ToMySqlDateTimeString(ReleaseDate), Software)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{WID}', '{DataSetWID}', '{MAGEClass}', '{ArrayDesign}', '{ArrayManufacture}', '{BioEvent_ProtocolApplications}', '{SerialNumber}', '{Hardware}', '{ActivityDate}', '{ProtocolApplication}', '{ProtocolApplication2}', '{Protocol}', '{Version}', '{ReleaseDate}', '{Software}')"
        Else
            Return $"('{WID}', '{DataSetWID}', '{MAGEClass}', '{ArrayDesign}', '{ArrayManufacture}', '{BioEvent_ProtocolApplications}', '{SerialNumber}', '{Hardware}', '{ActivityDate}', '{ProtocolApplication}', '{ProtocolApplication2}', '{Protocol}', '{Version}', '{ReleaseDate}', '{Software}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `parameterizableapplication` (`WID`, `DataSetWID`, `MAGEClass`, `ArrayDesign`, `ArrayManufacture`, `BioEvent_ProtocolApplications`, `SerialNumber`, `Hardware`, `ActivityDate`, `ProtocolApplication`, `ProtocolApplication2`, `Protocol`, `Version`, `ReleaseDate`, `Software`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, MAGEClass, ArrayDesign, ArrayManufacture, BioEvent_ProtocolApplications, SerialNumber, Hardware, ActivityDate, ProtocolApplication, ProtocolApplication2, Protocol, Version, MySqlScript.ToMySqlDateTimeString(ReleaseDate), Software)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `parameterizableapplication` (`WID`, `DataSetWID`, `MAGEClass`, `ArrayDesign`, `ArrayManufacture`, `BioEvent_ProtocolApplications`, `SerialNumber`, `Hardware`, `ActivityDate`, `ProtocolApplication`, `ProtocolApplication2`, `Protocol`, `Version`, `ReleaseDate`, `Software`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, WID, DataSetWID, MAGEClass, ArrayDesign, ArrayManufacture, BioEvent_ProtocolApplications, SerialNumber, Hardware, ActivityDate, ProtocolApplication, ProtocolApplication2, Protocol, Version, MySqlScript.ToMySqlDateTimeString(ReleaseDate), Software)
        Else
        Return String.Format(REPLACE_SQL, WID, DataSetWID, MAGEClass, ArrayDesign, ArrayManufacture, BioEvent_ProtocolApplications, SerialNumber, Hardware, ActivityDate, ProtocolApplication, ProtocolApplication2, Protocol, Version, MySqlScript.ToMySqlDateTimeString(ReleaseDate), Software)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `parameterizableapplication` SET `WID`='{0}', `DataSetWID`='{1}', `MAGEClass`='{2}', `ArrayDesign`='{3}', `ArrayManufacture`='{4}', `BioEvent_ProtocolApplications`='{5}', `SerialNumber`='{6}', `Hardware`='{7}', `ActivityDate`='{8}', `ProtocolApplication`='{9}', `ProtocolApplication2`='{10}', `Protocol`='{11}', `Version`='{12}', `ReleaseDate`='{13}', `Software`='{14}' WHERE `WID` = '{15}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, MAGEClass, ArrayDesign, ArrayManufacture, BioEvent_ProtocolApplications, SerialNumber, Hardware, ActivityDate, ProtocolApplication, ProtocolApplication2, Protocol, Version, MySqlScript.ToMySqlDateTimeString(ReleaseDate), Software, WID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As parameterizableapplication
                         Return DirectCast(MyClass.MemberwiseClone, parameterizableapplication)
                     End Function
End Class


End Namespace
