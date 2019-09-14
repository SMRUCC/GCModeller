#Region "Microsoft.VisualBasic::6ae99b7c7d60354084c5ac1805ed4579, ExternalDBSource\MetaCyc\bio_warehouse\chemicalbond.vb"

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

    ' Class chemicalbond
    ' 
    '     Properties: Atom1Index, Atom2Index, BondStereo, BondType, ChemicalWID
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
''' DROP TABLE IF EXISTS `chemicalbond`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `chemicalbond` (
'''   `ChemicalWID` bigint(20) NOT NULL,
'''   `Atom1Index` smallint(6) NOT NULL,
'''   `Atom2Index` smallint(6) NOT NULL,
'''   `BondType` smallint(6) NOT NULL,
'''   `BondStereo` decimal(10,0) DEFAULT NULL,
'''   KEY `FK_ChemicalBond` (`ChemicalWID`),
'''   CONSTRAINT `FK_ChemicalBond` FOREIGN KEY (`ChemicalWID`) REFERENCES `chemical` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("chemicalbond", Database:="warehouse")>
Public Class chemicalbond: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("ChemicalWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property ChemicalWID As Long
    <DatabaseField("Atom1Index"), NotNull, DataType(MySqlDbType.Int64, "6")> Public Property Atom1Index As Long
    <DatabaseField("Atom2Index"), NotNull, DataType(MySqlDbType.Int64, "6")> Public Property Atom2Index As Long
    <DatabaseField("BondType"), NotNull, DataType(MySqlDbType.Int64, "6")> Public Property BondType As Long
    <DatabaseField("BondStereo"), DataType(MySqlDbType.Decimal)> Public Property BondStereo As Decimal
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `chemicalbond` (`ChemicalWID`, `Atom1Index`, `Atom2Index`, `BondType`, `BondStereo`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `chemicalbond` (`ChemicalWID`, `Atom1Index`, `Atom2Index`, `BondType`, `BondStereo`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `chemicalbond` WHERE `ChemicalWID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `chemicalbond` SET `ChemicalWID`='{0}', `Atom1Index`='{1}', `Atom2Index`='{2}', `BondType`='{3}', `BondStereo`='{4}' WHERE `ChemicalWID` = '{5}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, ChemicalWID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, ChemicalWID, Atom1Index, Atom2Index, BondType, BondStereo)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, ChemicalWID, Atom1Index, Atom2Index, BondType, BondStereo)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, ChemicalWID, Atom1Index, Atom2Index, BondType, BondStereo, ChemicalWID)
    End Function
#End Region
End Class


End Namespace
