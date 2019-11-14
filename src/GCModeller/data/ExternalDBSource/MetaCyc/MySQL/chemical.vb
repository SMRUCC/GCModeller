#Region "Microsoft.VisualBasic::3a1f84146a60477dca4e4e67bbe03d5e, data\ExternalDBSource\MetaCyc\MySQL\chemical.vb"

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

    ' Class chemical
    ' 
    '     Properties: [Class], BeilsteinName, CAS, Charge, DataSetWID
    '                 EmpiricalFormula, MolecularWeightCalc, MolecularWeightExp, Name, OctH2OPartitionCoeff
    '                 PKA1, PKA2, PKA3, Smiles, SystematicName
    '                 WaterSolubility, WID
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
''' DROP TABLE IF EXISTS `chemical`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `chemical` (
'''   `WID` bigint(20) NOT NULL,
'''   `Name` varchar(255) NOT NULL,
'''   `Class` char(1) DEFAULT NULL,
'''   `BeilsteinName` varchar(50) DEFAULT NULL,
'''   `SystematicName` varchar(255) DEFAULT NULL,
'''   `CAS` varchar(50) DEFAULT NULL,
'''   `Charge` smallint(6) DEFAULT NULL,
'''   `EmpiricalFormula` varchar(50) DEFAULT NULL,
'''   `MolecularWeightCalc` float DEFAULT NULL,
'''   `MolecularWeightExp` float DEFAULT NULL,
'''   `OctH2OPartitionCoeff` varchar(50) DEFAULT NULL,
'''   `PKA1` float DEFAULT NULL,
'''   `PKA2` float DEFAULT NULL,
'''   `PKA3` float DEFAULT NULL,
'''   `WaterSolubility` char(1) DEFAULT NULL,
'''   `Smiles` varchar(255) DEFAULT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `CHEMICAL_DWID_NAME` (`DataSetWID`,`Name`),
'''   KEY `CHEMICAL_NAME` (`Name`),
'''   CONSTRAINT `FK_Chemical1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("chemical", Database:="warehouse", SchemaSQL:="
CREATE TABLE `chemical` (
  `WID` bigint(20) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Class` char(1) DEFAULT NULL,
  `BeilsteinName` varchar(50) DEFAULT NULL,
  `SystematicName` varchar(255) DEFAULT NULL,
  `CAS` varchar(50) DEFAULT NULL,
  `Charge` smallint(6) DEFAULT NULL,
  `EmpiricalFormula` varchar(50) DEFAULT NULL,
  `MolecularWeightCalc` float DEFAULT NULL,
  `MolecularWeightExp` float DEFAULT NULL,
  `OctH2OPartitionCoeff` varchar(50) DEFAULT NULL,
  `PKA1` float DEFAULT NULL,
  `PKA2` float DEFAULT NULL,
  `PKA3` float DEFAULT NULL,
  `WaterSolubility` char(1) DEFAULT NULL,
  `Smiles` varchar(255) DEFAULT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `CHEMICAL_DWID_NAME` (`DataSetWID`,`Name`),
  KEY `CHEMICAL_NAME` (`Name`),
  CONSTRAINT `FK_Chemical1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class chemical: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("Name"), NotNull, DataType(MySqlDbType.VarChar, "255"), Column(Name:="Name")> Public Property Name As String
    <DatabaseField("Class"), DataType(MySqlDbType.VarChar, "1"), Column(Name:="Class")> Public Property [Class] As String
    <DatabaseField("BeilsteinName"), DataType(MySqlDbType.VarChar, "50"), Column(Name:="BeilsteinName")> Public Property BeilsteinName As String
    <DatabaseField("SystematicName"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="SystematicName")> Public Property SystematicName As String
    <DatabaseField("CAS"), DataType(MySqlDbType.VarChar, "50"), Column(Name:="CAS")> Public Property CAS As String
    <DatabaseField("Charge"), DataType(MySqlDbType.Int64, "6"), Column(Name:="Charge")> Public Property Charge As Long
    <DatabaseField("EmpiricalFormula"), DataType(MySqlDbType.VarChar, "50"), Column(Name:="EmpiricalFormula")> Public Property EmpiricalFormula As String
    <DatabaseField("MolecularWeightCalc"), DataType(MySqlDbType.Double), Column(Name:="MolecularWeightCalc")> Public Property MolecularWeightCalc As Double
    <DatabaseField("MolecularWeightExp"), DataType(MySqlDbType.Double), Column(Name:="MolecularWeightExp")> Public Property MolecularWeightExp As Double
    <DatabaseField("OctH2OPartitionCoeff"), DataType(MySqlDbType.VarChar, "50"), Column(Name:="OctH2OPartitionCoeff")> Public Property OctH2OPartitionCoeff As String
    <DatabaseField("PKA1"), DataType(MySqlDbType.Double), Column(Name:="PKA1")> Public Property PKA1 As Double
    <DatabaseField("PKA2"), DataType(MySqlDbType.Double), Column(Name:="PKA2")> Public Property PKA2 As Double
    <DatabaseField("PKA3"), DataType(MySqlDbType.Double), Column(Name:="PKA3")> Public Property PKA3 As Double
    <DatabaseField("WaterSolubility"), DataType(MySqlDbType.VarChar, "1"), Column(Name:="WaterSolubility")> Public Property WaterSolubility As String
    <DatabaseField("Smiles"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="Smiles")> Public Property Smiles As String
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DataSetWID")> Public Property DataSetWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `chemical` (`WID`, `Name`, `Class`, `BeilsteinName`, `SystematicName`, `CAS`, `Charge`, `EmpiricalFormula`, `MolecularWeightCalc`, `MolecularWeightExp`, `OctH2OPartitionCoeff`, `PKA1`, `PKA2`, `PKA3`, `WaterSolubility`, `Smiles`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `chemical` (`WID`, `Name`, `Class`, `BeilsteinName`, `SystematicName`, `CAS`, `Charge`, `EmpiricalFormula`, `MolecularWeightCalc`, `MolecularWeightExp`, `OctH2OPartitionCoeff`, `PKA1`, `PKA2`, `PKA3`, `WaterSolubility`, `Smiles`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `chemical` (`WID`, `Name`, `Class`, `BeilsteinName`, `SystematicName`, `CAS`, `Charge`, `EmpiricalFormula`, `MolecularWeightCalc`, `MolecularWeightExp`, `OctH2OPartitionCoeff`, `PKA1`, `PKA2`, `PKA3`, `WaterSolubility`, `Smiles`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `chemical` (`WID`, `Name`, `Class`, `BeilsteinName`, `SystematicName`, `CAS`, `Charge`, `EmpiricalFormula`, `MolecularWeightCalc`, `MolecularWeightExp`, `OctH2OPartitionCoeff`, `PKA1`, `PKA2`, `PKA3`, `WaterSolubility`, `Smiles`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `chemical` WHERE `WID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `chemical` SET `WID`='{0}', `Name`='{1}', `Class`='{2}', `BeilsteinName`='{3}', `SystematicName`='{4}', `CAS`='{5}', `Charge`='{6}', `EmpiricalFormula`='{7}', `MolecularWeightCalc`='{8}', `MolecularWeightExp`='{9}', `OctH2OPartitionCoeff`='{10}', `PKA1`='{11}', `PKA2`='{12}', `PKA3`='{13}', `WaterSolubility`='{14}', `Smiles`='{15}', `DataSetWID`='{16}' WHERE `WID` = '{17}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `chemical` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `chemical` (`WID`, `Name`, `Class`, `BeilsteinName`, `SystematicName`, `CAS`, `Charge`, `EmpiricalFormula`, `MolecularWeightCalc`, `MolecularWeightExp`, `OctH2OPartitionCoeff`, `PKA1`, `PKA2`, `PKA3`, `WaterSolubility`, `Smiles`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, Name, [Class], BeilsteinName, SystematicName, CAS, Charge, EmpiricalFormula, MolecularWeightCalc, MolecularWeightExp, OctH2OPartitionCoeff, PKA1, PKA2, PKA3, WaterSolubility, Smiles, DataSetWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `chemical` (`WID`, `Name`, `Class`, `BeilsteinName`, `SystematicName`, `CAS`, `Charge`, `EmpiricalFormula`, `MolecularWeightCalc`, `MolecularWeightExp`, `OctH2OPartitionCoeff`, `PKA1`, `PKA2`, `PKA3`, `WaterSolubility`, `Smiles`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, WID, Name, [Class], BeilsteinName, SystematicName, CAS, Charge, EmpiricalFormula, MolecularWeightCalc, MolecularWeightExp, OctH2OPartitionCoeff, PKA1, PKA2, PKA3, WaterSolubility, Smiles, DataSetWID)
        Else
        Return String.Format(INSERT_SQL, WID, Name, [Class], BeilsteinName, SystematicName, CAS, Charge, EmpiricalFormula, MolecularWeightCalc, MolecularWeightExp, OctH2OPartitionCoeff, PKA1, PKA2, PKA3, WaterSolubility, Smiles, DataSetWID)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{WID}', '{Name}', '{[Class]}', '{BeilsteinName}', '{SystematicName}', '{CAS}', '{Charge}', '{EmpiricalFormula}', '{MolecularWeightCalc}', '{MolecularWeightExp}', '{OctH2OPartitionCoeff}', '{PKA1}', '{PKA2}', '{PKA3}', '{WaterSolubility}', '{Smiles}', '{DataSetWID}')"
        Else
            Return $"('{WID}', '{Name}', '{[Class]}', '{BeilsteinName}', '{SystematicName}', '{CAS}', '{Charge}', '{EmpiricalFormula}', '{MolecularWeightCalc}', '{MolecularWeightExp}', '{OctH2OPartitionCoeff}', '{PKA1}', '{PKA2}', '{PKA3}', '{WaterSolubility}', '{Smiles}', '{DataSetWID}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `chemical` (`WID`, `Name`, `Class`, `BeilsteinName`, `SystematicName`, `CAS`, `Charge`, `EmpiricalFormula`, `MolecularWeightCalc`, `MolecularWeightExp`, `OctH2OPartitionCoeff`, `PKA1`, `PKA2`, `PKA3`, `WaterSolubility`, `Smiles`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, Name, [Class], BeilsteinName, SystematicName, CAS, Charge, EmpiricalFormula, MolecularWeightCalc, MolecularWeightExp, OctH2OPartitionCoeff, PKA1, PKA2, PKA3, WaterSolubility, Smiles, DataSetWID)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `chemical` (`WID`, `Name`, `Class`, `BeilsteinName`, `SystematicName`, `CAS`, `Charge`, `EmpiricalFormula`, `MolecularWeightCalc`, `MolecularWeightExp`, `OctH2OPartitionCoeff`, `PKA1`, `PKA2`, `PKA3`, `WaterSolubility`, `Smiles`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, WID, Name, [Class], BeilsteinName, SystematicName, CAS, Charge, EmpiricalFormula, MolecularWeightCalc, MolecularWeightExp, OctH2OPartitionCoeff, PKA1, PKA2, PKA3, WaterSolubility, Smiles, DataSetWID)
        Else
        Return String.Format(REPLACE_SQL, WID, Name, [Class], BeilsteinName, SystematicName, CAS, Charge, EmpiricalFormula, MolecularWeightCalc, MolecularWeightExp, OctH2OPartitionCoeff, PKA1, PKA2, PKA3, WaterSolubility, Smiles, DataSetWID)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `chemical` SET `WID`='{0}', `Name`='{1}', `Class`='{2}', `BeilsteinName`='{3}', `SystematicName`='{4}', `CAS`='{5}', `Charge`='{6}', `EmpiricalFormula`='{7}', `MolecularWeightCalc`='{8}', `MolecularWeightExp`='{9}', `OctH2OPartitionCoeff`='{10}', `PKA1`='{11}', `PKA2`='{12}', `PKA3`='{13}', `WaterSolubility`='{14}', `Smiles`='{15}', `DataSetWID`='{16}' WHERE `WID` = '{17}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, Name, [Class], BeilsteinName, SystematicName, CAS, Charge, EmpiricalFormula, MolecularWeightCalc, MolecularWeightExp, OctH2OPartitionCoeff, PKA1, PKA2, PKA3, WaterSolubility, Smiles, DataSetWID, WID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As chemical
                         Return DirectCast(MyClass.MemberwiseClone, chemical)
                     End Function
End Class


End Namespace
