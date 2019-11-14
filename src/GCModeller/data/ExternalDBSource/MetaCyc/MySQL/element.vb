#Region "Microsoft.VisualBasic::59deeaa0e44778c8dc7ff5977e8f73c3, data\ExternalDBSource\MetaCyc\MySQL\element.vb"

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

    ' Class element
    ' 
    '     Properties: AtomicNumber, AtomicWeight, ElementSymbol, Name, WID
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
''' DROP TABLE IF EXISTS `element`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `element` (
'''   `WID` bigint(20) NOT NULL,
'''   `Name` varchar(15) NOT NULL,
'''   `ElementSymbol` varchar(2) NOT NULL,
'''   `AtomicWeight` float NOT NULL,
'''   `AtomicNumber` smallint(6) NOT NULL,
'''   PRIMARY KEY (`WID`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("element", Database:="warehouse", SchemaSQL:="
CREATE TABLE `element` (
  `WID` bigint(20) NOT NULL,
  `Name` varchar(15) NOT NULL,
  `ElementSymbol` varchar(2) NOT NULL,
  `AtomicWeight` float NOT NULL,
  `AtomicNumber` smallint(6) NOT NULL,
  PRIMARY KEY (`WID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class element: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("Name"), NotNull, DataType(MySqlDbType.VarChar, "15"), Column(Name:="Name")> Public Property Name As String
    <DatabaseField("ElementSymbol"), NotNull, DataType(MySqlDbType.VarChar, "2"), Column(Name:="ElementSymbol")> Public Property ElementSymbol As String
    <DatabaseField("AtomicWeight"), NotNull, DataType(MySqlDbType.Double), Column(Name:="AtomicWeight")> Public Property AtomicWeight As Double
    <DatabaseField("AtomicNumber"), NotNull, DataType(MySqlDbType.Int64, "6"), Column(Name:="AtomicNumber")> Public Property AtomicNumber As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `element` (`WID`, `Name`, `ElementSymbol`, `AtomicWeight`, `AtomicNumber`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `element` (`WID`, `Name`, `ElementSymbol`, `AtomicWeight`, `AtomicNumber`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `element` (`WID`, `Name`, `ElementSymbol`, `AtomicWeight`, `AtomicNumber`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `element` (`WID`, `Name`, `ElementSymbol`, `AtomicWeight`, `AtomicNumber`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `element` WHERE `WID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `element` SET `WID`='{0}', `Name`='{1}', `ElementSymbol`='{2}', `AtomicWeight`='{3}', `AtomicNumber`='{4}' WHERE `WID` = '{5}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `element` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `element` (`WID`, `Name`, `ElementSymbol`, `AtomicWeight`, `AtomicNumber`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, Name, ElementSymbol, AtomicWeight, AtomicNumber)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `element` (`WID`, `Name`, `ElementSymbol`, `AtomicWeight`, `AtomicNumber`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, WID, Name, ElementSymbol, AtomicWeight, AtomicNumber)
        Else
        Return String.Format(INSERT_SQL, WID, Name, ElementSymbol, AtomicWeight, AtomicNumber)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{WID}', '{Name}', '{ElementSymbol}', '{AtomicWeight}', '{AtomicNumber}')"
        Else
            Return $"('{WID}', '{Name}', '{ElementSymbol}', '{AtomicWeight}', '{AtomicNumber}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `element` (`WID`, `Name`, `ElementSymbol`, `AtomicWeight`, `AtomicNumber`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, Name, ElementSymbol, AtomicWeight, AtomicNumber)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `element` (`WID`, `Name`, `ElementSymbol`, `AtomicWeight`, `AtomicNumber`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, WID, Name, ElementSymbol, AtomicWeight, AtomicNumber)
        Else
        Return String.Format(REPLACE_SQL, WID, Name, ElementSymbol, AtomicWeight, AtomicNumber)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `element` SET `WID`='{0}', `Name`='{1}', `ElementSymbol`='{2}', `AtomicWeight`='{3}', `AtomicNumber`='{4}' WHERE `WID` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, Name, ElementSymbol, AtomicWeight, AtomicNumber, WID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As element
                         Return DirectCast(MyClass.MemberwiseClone, element)
                     End Function
End Class


End Namespace
