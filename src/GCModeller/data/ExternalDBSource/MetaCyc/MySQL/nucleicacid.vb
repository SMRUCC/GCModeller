#Region "Microsoft.VisualBasic::2ea288e0dcd46f44a6d25f896a6fca5d, data\ExternalDBSource\MetaCyc\MySQL\nucleicacid.vb"

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

    ' Class nucleicacid
    ' 
    '     Properties: [Class], BioSourceWID, CumulativeLength, CumulativeLengthApproximate, DataSetWID
    '                 Fragment, FullySequenced, GeneticCodeWID, MoleculeLength, MoleculeLengthApproximate
    '                 Name, SequenceDerivation, Strandedness, Topology, Type
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
''' DROP TABLE IF EXISTS `nucleicacid`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `nucleicacid` (
'''   `WID` bigint(20) NOT NULL,
'''   `Name` varchar(200) DEFAULT NULL,
'''   `Type` varchar(30) NOT NULL,
'''   `Class` varchar(30) DEFAULT NULL,
'''   `Topology` varchar(30) DEFAULT NULL,
'''   `Strandedness` varchar(30) DEFAULT NULL,
'''   `SequenceDerivation` varchar(30) DEFAULT NULL,
'''   `Fragment` char(1) DEFAULT NULL,
'''   `FullySequenced` char(1) DEFAULT NULL,
'''   `MoleculeLength` int(11) DEFAULT NULL,
'''   `MoleculeLengthApproximate` varchar(10) DEFAULT NULL,
'''   `CumulativeLength` int(11) DEFAULT NULL,
'''   `CumulativeLengthApproximate` varchar(10) DEFAULT NULL,
'''   `GeneticCodeWID` bigint(20) DEFAULT NULL,
'''   `BioSourceWID` bigint(20) DEFAULT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_NucleicAcid1` (`GeneticCodeWID`),
'''   KEY `FK_NucleicAcid2` (`BioSourceWID`),
'''   KEY `FK_NucleicAcid3` (`DataSetWID`),
'''   CONSTRAINT `FK_NucleicAcid1` FOREIGN KEY (`GeneticCodeWID`) REFERENCES `geneticcode` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_NucleicAcid2` FOREIGN KEY (`BioSourceWID`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_NucleicAcid3` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("nucleicacid", Database:="warehouse", SchemaSQL:="
CREATE TABLE `nucleicacid` (
  `WID` bigint(20) NOT NULL,
  `Name` varchar(200) DEFAULT NULL,
  `Type` varchar(30) NOT NULL,
  `Class` varchar(30) DEFAULT NULL,
  `Topology` varchar(30) DEFAULT NULL,
  `Strandedness` varchar(30) DEFAULT NULL,
  `SequenceDerivation` varchar(30) DEFAULT NULL,
  `Fragment` char(1) DEFAULT NULL,
  `FullySequenced` char(1) DEFAULT NULL,
  `MoleculeLength` int(11) DEFAULT NULL,
  `MoleculeLengthApproximate` varchar(10) DEFAULT NULL,
  `CumulativeLength` int(11) DEFAULT NULL,
  `CumulativeLengthApproximate` varchar(10) DEFAULT NULL,
  `GeneticCodeWID` bigint(20) DEFAULT NULL,
  `BioSourceWID` bigint(20) DEFAULT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_NucleicAcid1` (`GeneticCodeWID`),
  KEY `FK_NucleicAcid2` (`BioSourceWID`),
  KEY `FK_NucleicAcid3` (`DataSetWID`),
  CONSTRAINT `FK_NucleicAcid1` FOREIGN KEY (`GeneticCodeWID`) REFERENCES `geneticcode` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_NucleicAcid2` FOREIGN KEY (`BioSourceWID`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_NucleicAcid3` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class nucleicacid: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("Name"), DataType(MySqlDbType.VarChar, "200"), Column(Name:="Name")> Public Property Name As String
    <DatabaseField("Type"), NotNull, DataType(MySqlDbType.VarChar, "30"), Column(Name:="Type")> Public Property Type As String
    <DatabaseField("Class"), DataType(MySqlDbType.VarChar, "30"), Column(Name:="Class")> Public Property [Class] As String
    <DatabaseField("Topology"), DataType(MySqlDbType.VarChar, "30"), Column(Name:="Topology")> Public Property Topology As String
    <DatabaseField("Strandedness"), DataType(MySqlDbType.VarChar, "30"), Column(Name:="Strandedness")> Public Property Strandedness As String
    <DatabaseField("SequenceDerivation"), DataType(MySqlDbType.VarChar, "30"), Column(Name:="SequenceDerivation")> Public Property SequenceDerivation As String
    <DatabaseField("Fragment"), DataType(MySqlDbType.VarChar, "1"), Column(Name:="Fragment")> Public Property Fragment As String
    <DatabaseField("FullySequenced"), DataType(MySqlDbType.VarChar, "1"), Column(Name:="FullySequenced")> Public Property FullySequenced As String
    <DatabaseField("MoleculeLength"), DataType(MySqlDbType.Int64, "11"), Column(Name:="MoleculeLength")> Public Property MoleculeLength As Long
    <DatabaseField("MoleculeLengthApproximate"), DataType(MySqlDbType.VarChar, "10"), Column(Name:="MoleculeLengthApproximate")> Public Property MoleculeLengthApproximate As String
    <DatabaseField("CumulativeLength"), DataType(MySqlDbType.Int64, "11"), Column(Name:="CumulativeLength")> Public Property CumulativeLength As Long
    <DatabaseField("CumulativeLengthApproximate"), DataType(MySqlDbType.VarChar, "10"), Column(Name:="CumulativeLengthApproximate")> Public Property CumulativeLengthApproximate As String
    <DatabaseField("GeneticCodeWID"), DataType(MySqlDbType.Int64, "20"), Column(Name:="GeneticCodeWID")> Public Property GeneticCodeWID As Long
    <DatabaseField("BioSourceWID"), DataType(MySqlDbType.Int64, "20"), Column(Name:="BioSourceWID")> Public Property BioSourceWID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DataSetWID")> Public Property DataSetWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `nucleicacid` (`WID`, `Name`, `Type`, `Class`, `Topology`, `Strandedness`, `SequenceDerivation`, `Fragment`, `FullySequenced`, `MoleculeLength`, `MoleculeLengthApproximate`, `CumulativeLength`, `CumulativeLengthApproximate`, `GeneticCodeWID`, `BioSourceWID`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `nucleicacid` (`WID`, `Name`, `Type`, `Class`, `Topology`, `Strandedness`, `SequenceDerivation`, `Fragment`, `FullySequenced`, `MoleculeLength`, `MoleculeLengthApproximate`, `CumulativeLength`, `CumulativeLengthApproximate`, `GeneticCodeWID`, `BioSourceWID`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `nucleicacid` (`WID`, `Name`, `Type`, `Class`, `Topology`, `Strandedness`, `SequenceDerivation`, `Fragment`, `FullySequenced`, `MoleculeLength`, `MoleculeLengthApproximate`, `CumulativeLength`, `CumulativeLengthApproximate`, `GeneticCodeWID`, `BioSourceWID`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `nucleicacid` (`WID`, `Name`, `Type`, `Class`, `Topology`, `Strandedness`, `SequenceDerivation`, `Fragment`, `FullySequenced`, `MoleculeLength`, `MoleculeLengthApproximate`, `CumulativeLength`, `CumulativeLengthApproximate`, `GeneticCodeWID`, `BioSourceWID`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `nucleicacid` WHERE `WID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `nucleicacid` SET `WID`='{0}', `Name`='{1}', `Type`='{2}', `Class`='{3}', `Topology`='{4}', `Strandedness`='{5}', `SequenceDerivation`='{6}', `Fragment`='{7}', `FullySequenced`='{8}', `MoleculeLength`='{9}', `MoleculeLengthApproximate`='{10}', `CumulativeLength`='{11}', `CumulativeLengthApproximate`='{12}', `GeneticCodeWID`='{13}', `BioSourceWID`='{14}', `DataSetWID`='{15}' WHERE `WID` = '{16}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `nucleicacid` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `nucleicacid` (`WID`, `Name`, `Type`, `Class`, `Topology`, `Strandedness`, `SequenceDerivation`, `Fragment`, `FullySequenced`, `MoleculeLength`, `MoleculeLengthApproximate`, `CumulativeLength`, `CumulativeLengthApproximate`, `GeneticCodeWID`, `BioSourceWID`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, Name, Type, [Class], Topology, Strandedness, SequenceDerivation, Fragment, FullySequenced, MoleculeLength, MoleculeLengthApproximate, CumulativeLength, CumulativeLengthApproximate, GeneticCodeWID, BioSourceWID, DataSetWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `nucleicacid` (`WID`, `Name`, `Type`, `Class`, `Topology`, `Strandedness`, `SequenceDerivation`, `Fragment`, `FullySequenced`, `MoleculeLength`, `MoleculeLengthApproximate`, `CumulativeLength`, `CumulativeLengthApproximate`, `GeneticCodeWID`, `BioSourceWID`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, WID, Name, Type, [Class], Topology, Strandedness, SequenceDerivation, Fragment, FullySequenced, MoleculeLength, MoleculeLengthApproximate, CumulativeLength, CumulativeLengthApproximate, GeneticCodeWID, BioSourceWID, DataSetWID)
        Else
        Return String.Format(INSERT_SQL, WID, Name, Type, [Class], Topology, Strandedness, SequenceDerivation, Fragment, FullySequenced, MoleculeLength, MoleculeLengthApproximate, CumulativeLength, CumulativeLengthApproximate, GeneticCodeWID, BioSourceWID, DataSetWID)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{WID}', '{Name}', '{Type}', '{[Class]}', '{Topology}', '{Strandedness}', '{SequenceDerivation}', '{Fragment}', '{FullySequenced}', '{MoleculeLength}', '{MoleculeLengthApproximate}', '{CumulativeLength}', '{CumulativeLengthApproximate}', '{GeneticCodeWID}', '{BioSourceWID}', '{DataSetWID}')"
        Else
            Return $"('{WID}', '{Name}', '{Type}', '{[Class]}', '{Topology}', '{Strandedness}', '{SequenceDerivation}', '{Fragment}', '{FullySequenced}', '{MoleculeLength}', '{MoleculeLengthApproximate}', '{CumulativeLength}', '{CumulativeLengthApproximate}', '{GeneticCodeWID}', '{BioSourceWID}', '{DataSetWID}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `nucleicacid` (`WID`, `Name`, `Type`, `Class`, `Topology`, `Strandedness`, `SequenceDerivation`, `Fragment`, `FullySequenced`, `MoleculeLength`, `MoleculeLengthApproximate`, `CumulativeLength`, `CumulativeLengthApproximate`, `GeneticCodeWID`, `BioSourceWID`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, Name, Type, [Class], Topology, Strandedness, SequenceDerivation, Fragment, FullySequenced, MoleculeLength, MoleculeLengthApproximate, CumulativeLength, CumulativeLengthApproximate, GeneticCodeWID, BioSourceWID, DataSetWID)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `nucleicacid` (`WID`, `Name`, `Type`, `Class`, `Topology`, `Strandedness`, `SequenceDerivation`, `Fragment`, `FullySequenced`, `MoleculeLength`, `MoleculeLengthApproximate`, `CumulativeLength`, `CumulativeLengthApproximate`, `GeneticCodeWID`, `BioSourceWID`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, WID, Name, Type, [Class], Topology, Strandedness, SequenceDerivation, Fragment, FullySequenced, MoleculeLength, MoleculeLengthApproximate, CumulativeLength, CumulativeLengthApproximate, GeneticCodeWID, BioSourceWID, DataSetWID)
        Else
        Return String.Format(REPLACE_SQL, WID, Name, Type, [Class], Topology, Strandedness, SequenceDerivation, Fragment, FullySequenced, MoleculeLength, MoleculeLengthApproximate, CumulativeLength, CumulativeLengthApproximate, GeneticCodeWID, BioSourceWID, DataSetWID)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `nucleicacid` SET `WID`='{0}', `Name`='{1}', `Type`='{2}', `Class`='{3}', `Topology`='{4}', `Strandedness`='{5}', `SequenceDerivation`='{6}', `Fragment`='{7}', `FullySequenced`='{8}', `MoleculeLength`='{9}', `MoleculeLengthApproximate`='{10}', `CumulativeLength`='{11}', `CumulativeLengthApproximate`='{12}', `GeneticCodeWID`='{13}', `BioSourceWID`='{14}', `DataSetWID`='{15}' WHERE `WID` = '{16}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, Name, Type, [Class], Topology, Strandedness, SequenceDerivation, Fragment, FullySequenced, MoleculeLength, MoleculeLengthApproximate, CumulativeLength, CumulativeLengthApproximate, GeneticCodeWID, BioSourceWID, DataSetWID, WID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As nucleicacid
                         Return DirectCast(MyClass.MemberwiseClone, nucleicacid)
                     End Function
End Class


End Namespace
