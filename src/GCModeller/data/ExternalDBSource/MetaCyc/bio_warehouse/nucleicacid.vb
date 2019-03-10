#Region "Microsoft.VisualBasic::a85d2dc8ddbc3812629e3ac7c1915417, data\ExternalDBSource\MetaCyc\bio_warehouse\nucleicacid.vb"

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
    '     Function: GetDeleteSQL, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:02:47 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MetaCyc.MySQL

''' <summary>
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
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("nucleicacid", Database:="warehouse")>
Public Class nucleicacid: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property WID As Long
    <DatabaseField("Name"), DataType(MySqlDbType.VarChar, "200")> Public Property Name As String
    <DatabaseField("Type"), NotNull, DataType(MySqlDbType.VarChar, "30")> Public Property Type As String
    <DatabaseField("Class"), DataType(MySqlDbType.VarChar, "30")> Public Property [Class] As String
    <DatabaseField("Topology"), DataType(MySqlDbType.VarChar, "30")> Public Property Topology As String
    <DatabaseField("Strandedness"), DataType(MySqlDbType.VarChar, "30")> Public Property Strandedness As String
    <DatabaseField("SequenceDerivation"), DataType(MySqlDbType.VarChar, "30")> Public Property SequenceDerivation As String
    <DatabaseField("Fragment"), DataType(MySqlDbType.VarChar, "1")> Public Property Fragment As String
    <DatabaseField("FullySequenced"), DataType(MySqlDbType.VarChar, "1")> Public Property FullySequenced As String
    <DatabaseField("MoleculeLength"), DataType(MySqlDbType.Int64, "11")> Public Property MoleculeLength As Long
    <DatabaseField("MoleculeLengthApproximate"), DataType(MySqlDbType.VarChar, "10")> Public Property MoleculeLengthApproximate As String
    <DatabaseField("CumulativeLength"), DataType(MySqlDbType.Int64, "11")> Public Property CumulativeLength As Long
    <DatabaseField("CumulativeLengthApproximate"), DataType(MySqlDbType.VarChar, "10")> Public Property CumulativeLengthApproximate As String
    <DatabaseField("GeneticCodeWID"), DataType(MySqlDbType.Int64, "20")> Public Property GeneticCodeWID As Long
    <DatabaseField("BioSourceWID"), DataType(MySqlDbType.Int64, "20")> Public Property BioSourceWID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DataSetWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `nucleicacid` (`WID`, `Name`, `Type`, `Class`, `Topology`, `Strandedness`, `SequenceDerivation`, `Fragment`, `FullySequenced`, `MoleculeLength`, `MoleculeLengthApproximate`, `CumulativeLength`, `CumulativeLengthApproximate`, `GeneticCodeWID`, `BioSourceWID`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `nucleicacid` (`WID`, `Name`, `Type`, `Class`, `Topology`, `Strandedness`, `SequenceDerivation`, `Fragment`, `FullySequenced`, `MoleculeLength`, `MoleculeLengthApproximate`, `CumulativeLength`, `CumulativeLengthApproximate`, `GeneticCodeWID`, `BioSourceWID`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `nucleicacid` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `nucleicacid` SET `WID`='{0}', `Name`='{1}', `Type`='{2}', `Class`='{3}', `Topology`='{4}', `Strandedness`='{5}', `SequenceDerivation`='{6}', `Fragment`='{7}', `FullySequenced`='{8}', `MoleculeLength`='{9}', `MoleculeLengthApproximate`='{10}', `CumulativeLength`='{11}', `CumulativeLengthApproximate`='{12}', `GeneticCodeWID`='{13}', `BioSourceWID`='{14}', `DataSetWID`='{15}' WHERE `WID` = '{16}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, Name, Type, [Class], Topology, Strandedness, SequenceDerivation, Fragment, FullySequenced, MoleculeLength, MoleculeLengthApproximate, CumulativeLength, CumulativeLengthApproximate, GeneticCodeWID, BioSourceWID, DataSetWID)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, Name, Type, [Class], Topology, Strandedness, SequenceDerivation, Fragment, FullySequenced, MoleculeLength, MoleculeLengthApproximate, CumulativeLength, CumulativeLengthApproximate, GeneticCodeWID, BioSourceWID, DataSetWID)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, Name, Type, [Class], Topology, Strandedness, SequenceDerivation, Fragment, FullySequenced, MoleculeLength, MoleculeLengthApproximate, CumulativeLength, CumulativeLengthApproximate, GeneticCodeWID, BioSourceWID, DataSetWID, WID)
    End Function
#End Region
End Class


End Namespace
