#Region "Microsoft.VisualBasic::bbfd92062b8d97c50053ecad69a78fcb, data\ExternalDBSource\MetaCyc\MySQL\chemicalatom.vb"

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

    ' Class chemicalatom
    ' 
    '     Properties: Atom, AtomIndex, Charge, ChemicalWID, StereoParity
    '                 X, Y, Z
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
''' DROP TABLE IF EXISTS `chemicalatom`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `chemicalatom` (
'''   `ChemicalWID` bigint(20) NOT NULL,
'''   `AtomIndex` smallint(6) NOT NULL,
'''   `Atom` varchar(2) NOT NULL,
'''   `Charge` smallint(6) NOT NULL,
'''   `X` decimal(10,0) DEFAULT NULL,
'''   `Y` decimal(10,0) DEFAULT NULL,
'''   `Z` decimal(10,0) DEFAULT NULL,
'''   `StereoParity` decimal(10,0) DEFAULT NULL,
'''   UNIQUE KEY `UN_ChemicalAtom` (`ChemicalWID`,`AtomIndex`),
'''   CONSTRAINT `FK_ChemicalAtom` FOREIGN KEY (`ChemicalWID`) REFERENCES `chemical` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("chemicalatom", Database:="warehouse", SchemaSQL:="
CREATE TABLE `chemicalatom` (
  `ChemicalWID` bigint(20) NOT NULL,
  `AtomIndex` smallint(6) NOT NULL,
  `Atom` varchar(2) NOT NULL,
  `Charge` smallint(6) NOT NULL,
  `X` decimal(10,0) DEFAULT NULL,
  `Y` decimal(10,0) DEFAULT NULL,
  `Z` decimal(10,0) DEFAULT NULL,
  `StereoParity` decimal(10,0) DEFAULT NULL,
  UNIQUE KEY `UN_ChemicalAtom` (`ChemicalWID`,`AtomIndex`),
  CONSTRAINT `FK_ChemicalAtom` FOREIGN KEY (`ChemicalWID`) REFERENCES `chemical` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class chemicalatom: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("ChemicalWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="ChemicalWID"), XmlAttribute> Public Property ChemicalWID As Long
    <DatabaseField("AtomIndex"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "6"), Column(Name:="AtomIndex"), XmlAttribute> Public Property AtomIndex As Long
    <DatabaseField("Atom"), NotNull, DataType(MySqlDbType.VarChar, "2"), Column(Name:="Atom")> Public Property Atom As String
    <DatabaseField("Charge"), NotNull, DataType(MySqlDbType.Int64, "6"), Column(Name:="Charge")> Public Property Charge As Long
    <DatabaseField("X"), DataType(MySqlDbType.Decimal), Column(Name:="X")> Public Property X As Decimal
    <DatabaseField("Y"), DataType(MySqlDbType.Decimal), Column(Name:="Y")> Public Property Y As Decimal
    <DatabaseField("Z"), DataType(MySqlDbType.Decimal), Column(Name:="Z")> Public Property Z As Decimal
    <DatabaseField("StereoParity"), DataType(MySqlDbType.Decimal), Column(Name:="StereoParity")> Public Property StereoParity As Decimal
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `chemicalatom` (`ChemicalWID`, `AtomIndex`, `Atom`, `Charge`, `X`, `Y`, `Z`, `StereoParity`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `chemicalatom` (`ChemicalWID`, `AtomIndex`, `Atom`, `Charge`, `X`, `Y`, `Z`, `StereoParity`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `chemicalatom` (`ChemicalWID`, `AtomIndex`, `Atom`, `Charge`, `X`, `Y`, `Z`, `StereoParity`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `chemicalatom` (`ChemicalWID`, `AtomIndex`, `Atom`, `Charge`, `X`, `Y`, `Z`, `StereoParity`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `chemicalatom` WHERE `ChemicalWID`='{0}' and `AtomIndex`='{1}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `chemicalatom` SET `ChemicalWID`='{0}', `AtomIndex`='{1}', `Atom`='{2}', `Charge`='{3}', `X`='{4}', `Y`='{5}', `Z`='{6}', `StereoParity`='{7}' WHERE `ChemicalWID`='{8}' and `AtomIndex`='{9}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `chemicalatom` WHERE `ChemicalWID`='{0}' and `AtomIndex`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, ChemicalWID, AtomIndex)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `chemicalatom` (`ChemicalWID`, `AtomIndex`, `Atom`, `Charge`, `X`, `Y`, `Z`, `StereoParity`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, ChemicalWID, AtomIndex, Atom, Charge, X, Y, Z, StereoParity)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `chemicalatom` (`ChemicalWID`, `AtomIndex`, `Atom`, `Charge`, `X`, `Y`, `Z`, `StereoParity`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, ChemicalWID, AtomIndex, Atom, Charge, X, Y, Z, StereoParity)
        Else
        Return String.Format(INSERT_SQL, ChemicalWID, AtomIndex, Atom, Charge, X, Y, Z, StereoParity)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{ChemicalWID}', '{AtomIndex}', '{Atom}', '{Charge}', '{X}', '{Y}', '{Z}', '{StereoParity}')"
        Else
            Return $"('{ChemicalWID}', '{AtomIndex}', '{Atom}', '{Charge}', '{X}', '{Y}', '{Z}', '{StereoParity}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `chemicalatom` (`ChemicalWID`, `AtomIndex`, `Atom`, `Charge`, `X`, `Y`, `Z`, `StereoParity`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, ChemicalWID, AtomIndex, Atom, Charge, X, Y, Z, StereoParity)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `chemicalatom` (`ChemicalWID`, `AtomIndex`, `Atom`, `Charge`, `X`, `Y`, `Z`, `StereoParity`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, ChemicalWID, AtomIndex, Atom, Charge, X, Y, Z, StereoParity)
        Else
        Return String.Format(REPLACE_SQL, ChemicalWID, AtomIndex, Atom, Charge, X, Y, Z, StereoParity)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `chemicalatom` SET `ChemicalWID`='{0}', `AtomIndex`='{1}', `Atom`='{2}', `Charge`='{3}', `X`='{4}', `Y`='{5}', `Z`='{6}', `StereoParity`='{7}' WHERE `ChemicalWID`='{8}' and `AtomIndex`='{9}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, ChemicalWID, AtomIndex, Atom, Charge, X, Y, Z, StereoParity, ChemicalWID, AtomIndex)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As chemicalatom
                         Return DirectCast(MyClass.MemberwiseClone, chemicalatom)
                     End Function
End Class


End Namespace
