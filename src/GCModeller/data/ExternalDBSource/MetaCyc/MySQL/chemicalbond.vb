#Region "Microsoft.VisualBasic::123215985df29871a9a0af7c045c4113, data\ExternalDBSource\MetaCyc\MySQL\chemicalbond.vb"

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
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("chemicalbond", Database:="warehouse", SchemaSQL:="
CREATE TABLE `chemicalbond` (
  `ChemicalWID` bigint(20) NOT NULL,
  `Atom1Index` smallint(6) NOT NULL,
  `Atom2Index` smallint(6) NOT NULL,
  `BondType` smallint(6) NOT NULL,
  `BondStereo` decimal(10,0) DEFAULT NULL,
  KEY `FK_ChemicalBond` (`ChemicalWID`),
  CONSTRAINT `FK_ChemicalBond` FOREIGN KEY (`ChemicalWID`) REFERENCES `chemical` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class chemicalbond: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("ChemicalWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="ChemicalWID"), XmlAttribute> Public Property ChemicalWID As Long
    <DatabaseField("Atom1Index"), NotNull, DataType(MySqlDbType.Int64, "6"), Column(Name:="Atom1Index")> Public Property Atom1Index As Long
    <DatabaseField("Atom2Index"), NotNull, DataType(MySqlDbType.Int64, "6"), Column(Name:="Atom2Index")> Public Property Atom2Index As Long
    <DatabaseField("BondType"), NotNull, DataType(MySqlDbType.Int64, "6"), Column(Name:="BondType")> Public Property BondType As Long
    <DatabaseField("BondStereo"), DataType(MySqlDbType.Decimal), Column(Name:="BondStereo")> Public Property BondStereo As Decimal
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `chemicalbond` (`ChemicalWID`, `Atom1Index`, `Atom2Index`, `BondType`, `BondStereo`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `chemicalbond` (`ChemicalWID`, `Atom1Index`, `Atom2Index`, `BondType`, `BondStereo`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `chemicalbond` (`ChemicalWID`, `Atom1Index`, `Atom2Index`, `BondType`, `BondStereo`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `chemicalbond` (`ChemicalWID`, `Atom1Index`, `Atom2Index`, `BondType`, `BondStereo`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `chemicalbond` WHERE `ChemicalWID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `chemicalbond` SET `ChemicalWID`='{0}', `Atom1Index`='{1}', `Atom2Index`='{2}', `BondType`='{3}', `BondStereo`='{4}' WHERE `ChemicalWID` = '{5}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `chemicalbond` WHERE `ChemicalWID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, ChemicalWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `chemicalbond` (`ChemicalWID`, `Atom1Index`, `Atom2Index`, `BondType`, `BondStereo`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, ChemicalWID, Atom1Index, Atom2Index, BondType, BondStereo)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `chemicalbond` (`ChemicalWID`, `Atom1Index`, `Atom2Index`, `BondType`, `BondStereo`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, ChemicalWID, Atom1Index, Atom2Index, BondType, BondStereo)
        Else
        Return String.Format(INSERT_SQL, ChemicalWID, Atom1Index, Atom2Index, BondType, BondStereo)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{ChemicalWID}', '{Atom1Index}', '{Atom2Index}', '{BondType}', '{BondStereo}')"
        Else
            Return $"('{ChemicalWID}', '{Atom1Index}', '{Atom2Index}', '{BondType}', '{BondStereo}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `chemicalbond` (`ChemicalWID`, `Atom1Index`, `Atom2Index`, `BondType`, `BondStereo`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, ChemicalWID, Atom1Index, Atom2Index, BondType, BondStereo)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `chemicalbond` (`ChemicalWID`, `Atom1Index`, `Atom2Index`, `BondType`, `BondStereo`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, ChemicalWID, Atom1Index, Atom2Index, BondType, BondStereo)
        Else
        Return String.Format(REPLACE_SQL, ChemicalWID, Atom1Index, Atom2Index, BondType, BondStereo)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `chemicalbond` SET `ChemicalWID`='{0}', `Atom1Index`='{1}', `Atom2Index`='{2}', `BondType`='{3}', `BondStereo`='{4}' WHERE `ChemicalWID` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, ChemicalWID, Atom1Index, Atom2Index, BondType, BondStereo, ChemicalWID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As chemicalbond
                         Return DirectCast(MyClass.MemberwiseClone, chemicalbond)
                     End Function
End Class


End Namespace
