#Region "Microsoft.VisualBasic::4b09073d9bcc74a2c060db64012c6201, data\ExternalDBSource\MetaCyc\MySQL\element.vb"

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
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @3/29/2017 8:48:56 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

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
''' 
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class element: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property WID As Long
    <DatabaseField("Name"), NotNull, DataType(MySqlDbType.VarChar, "15")> Public Property Name As String
    <DatabaseField("ElementSymbol"), NotNull, DataType(MySqlDbType.VarChar, "2")> Public Property ElementSymbol As String
    <DatabaseField("AtomicWeight"), NotNull, DataType(MySqlDbType.Double)> Public Property AtomicWeight As Double
    <DatabaseField("AtomicNumber"), NotNull, DataType(MySqlDbType.Int64, "6")> Public Property AtomicNumber As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `element` (`WID`, `Name`, `ElementSymbol`, `AtomicWeight`, `AtomicNumber`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `element` (`WID`, `Name`, `ElementSymbol`, `AtomicWeight`, `AtomicNumber`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `element` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `element` SET `WID`='{0}', `Name`='{1}', `ElementSymbol`='{2}', `AtomicWeight`='{3}', `AtomicNumber`='{4}' WHERE `WID` = '{5}';</SQL>
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
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{WID}', '{Name}', '{ElementSymbol}', '{AtomicWeight}', '{AtomicNumber}')"
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
''' UPDATE `element` SET `WID`='{0}', `Name`='{1}', `ElementSymbol`='{2}', `AtomicWeight`='{3}', `AtomicNumber`='{4}' WHERE `WID` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, Name, ElementSymbol, AtomicWeight, AtomicNumber, WID)
    End Function
#End Region
End Class


End Namespace
