﻿#Region "Microsoft.VisualBasic::c52db9b3cf05028897f57548217b8b05, data\ExternalDBSource\MetaCyc\bio_warehouse\chemical.vb"

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
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("chemical", Database:="warehouse")>
Public Class chemical: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property WID As Long
    <DatabaseField("Name"), NotNull, DataType(MySqlDbType.VarChar, "255")> Public Property Name As String
    <DatabaseField("Class"), DataType(MySqlDbType.VarChar, "1")> Public Property [Class] As String
    <DatabaseField("BeilsteinName"), DataType(MySqlDbType.VarChar, "50")> Public Property BeilsteinName As String
    <DatabaseField("SystematicName"), DataType(MySqlDbType.VarChar, "255")> Public Property SystematicName As String
    <DatabaseField("CAS"), DataType(MySqlDbType.VarChar, "50")> Public Property CAS As String
    <DatabaseField("Charge"), DataType(MySqlDbType.Int64, "6")> Public Property Charge As Long
    <DatabaseField("EmpiricalFormula"), DataType(MySqlDbType.VarChar, "50")> Public Property EmpiricalFormula As String
    <DatabaseField("MolecularWeightCalc"), DataType(MySqlDbType.Double)> Public Property MolecularWeightCalc As Double
    <DatabaseField("MolecularWeightExp"), DataType(MySqlDbType.Double)> Public Property MolecularWeightExp As Double
    <DatabaseField("OctH2OPartitionCoeff"), DataType(MySqlDbType.VarChar, "50")> Public Property OctH2OPartitionCoeff As String
    <DatabaseField("PKA1"), DataType(MySqlDbType.Double)> Public Property PKA1 As Double
    <DatabaseField("PKA2"), DataType(MySqlDbType.Double)> Public Property PKA2 As Double
    <DatabaseField("PKA3"), DataType(MySqlDbType.Double)> Public Property PKA3 As Double
    <DatabaseField("WaterSolubility"), DataType(MySqlDbType.VarChar, "1")> Public Property WaterSolubility As String
    <DatabaseField("Smiles"), DataType(MySqlDbType.VarChar, "255")> Public Property Smiles As String
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DataSetWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `chemical` (`WID`, `Name`, `Class`, `BeilsteinName`, `SystematicName`, `CAS`, `Charge`, `EmpiricalFormula`, `MolecularWeightCalc`, `MolecularWeightExp`, `OctH2OPartitionCoeff`, `PKA1`, `PKA2`, `PKA3`, `WaterSolubility`, `Smiles`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `chemical` (`WID`, `Name`, `Class`, `BeilsteinName`, `SystematicName`, `CAS`, `Charge`, `EmpiricalFormula`, `MolecularWeightCalc`, `MolecularWeightExp`, `OctH2OPartitionCoeff`, `PKA1`, `PKA2`, `PKA3`, `WaterSolubility`, `Smiles`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `chemical` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `chemical` SET `WID`='{0}', `Name`='{1}', `Class`='{2}', `BeilsteinName`='{3}', `SystematicName`='{4}', `CAS`='{5}', `Charge`='{6}', `EmpiricalFormula`='{7}', `MolecularWeightCalc`='{8}', `MolecularWeightExp`='{9}', `OctH2OPartitionCoeff`='{10}', `PKA1`='{11}', `PKA2`='{12}', `PKA3`='{13}', `WaterSolubility`='{14}', `Smiles`='{15}', `DataSetWID`='{16}' WHERE `WID` = '{17}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, Name, [Class], BeilsteinName, SystematicName, CAS, Charge, EmpiricalFormula, MolecularWeightCalc, MolecularWeightExp, OctH2OPartitionCoeff, PKA1, PKA2, PKA3, WaterSolubility, Smiles, DataSetWID)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, Name, [Class], BeilsteinName, SystematicName, CAS, Charge, EmpiricalFormula, MolecularWeightCalc, MolecularWeightExp, OctH2OPartitionCoeff, PKA1, PKA2, PKA3, WaterSolubility, Smiles, DataSetWID)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, Name, [Class], BeilsteinName, SystematicName, CAS, Charge, EmpiricalFormula, MolecularWeightCalc, MolecularWeightExp, OctH2OPartitionCoeff, PKA1, PKA2, PKA3, WaterSolubility, Smiles, DataSetWID, WID)
    End Function
#End Region
End Class


End Namespace
