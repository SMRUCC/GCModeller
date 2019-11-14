#Region "Microsoft.VisualBasic::6215ddda8dc1b6b37c5027b006f19588, data\ExternalDBSource\MetaCyc\MySQL\position_.vb"

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

    ' Class position_
    ' 
    '     Properties: DataSetWID, Position_DistanceUnit, WID, X, Y
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
''' DROP TABLE IF EXISTS `position_`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `position_` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `X` float DEFAULT NULL,
'''   `Y` float DEFAULT NULL,
'''   `Position_DistanceUnit` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_Position_1` (`DataSetWID`),
'''   KEY `FK_Position_2` (`Position_DistanceUnit`),
'''   CONSTRAINT `FK_Position_1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Position_2` FOREIGN KEY (`Position_DistanceUnit`) REFERENCES `unit` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("position_", Database:="warehouse", SchemaSQL:="
CREATE TABLE `position_` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `X` float DEFAULT NULL,
  `Y` float DEFAULT NULL,
  `Position_DistanceUnit` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_Position_1` (`DataSetWID`),
  KEY `FK_Position_2` (`Position_DistanceUnit`),
  CONSTRAINT `FK_Position_1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Position_2` FOREIGN KEY (`Position_DistanceUnit`) REFERENCES `unit` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class position_: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DataSetWID")> Public Property DataSetWID As Long
    <DatabaseField("X"), DataType(MySqlDbType.Double), Column(Name:="X")> Public Property X As Double
    <DatabaseField("Y"), DataType(MySqlDbType.Double), Column(Name:="Y")> Public Property Y As Double
    <DatabaseField("Position_DistanceUnit"), DataType(MySqlDbType.Int64, "20"), Column(Name:="Position_DistanceUnit")> Public Property Position_DistanceUnit As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `position_` (`WID`, `DataSetWID`, `X`, `Y`, `Position_DistanceUnit`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `position_` (`WID`, `DataSetWID`, `X`, `Y`, `Position_DistanceUnit`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `position_` (`WID`, `DataSetWID`, `X`, `Y`, `Position_DistanceUnit`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `position_` (`WID`, `DataSetWID`, `X`, `Y`, `Position_DistanceUnit`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `position_` WHERE `WID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `position_` SET `WID`='{0}', `DataSetWID`='{1}', `X`='{2}', `Y`='{3}', `Position_DistanceUnit`='{4}' WHERE `WID` = '{5}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `position_` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `position_` (`WID`, `DataSetWID`, `X`, `Y`, `Position_DistanceUnit`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, X, Y, Position_DistanceUnit)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `position_` (`WID`, `DataSetWID`, `X`, `Y`, `Position_DistanceUnit`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, WID, DataSetWID, X, Y, Position_DistanceUnit)
        Else
        Return String.Format(INSERT_SQL, WID, DataSetWID, X, Y, Position_DistanceUnit)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{WID}', '{DataSetWID}', '{X}', '{Y}', '{Position_DistanceUnit}')"
        Else
            Return $"('{WID}', '{DataSetWID}', '{X}', '{Y}', '{Position_DistanceUnit}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `position_` (`WID`, `DataSetWID`, `X`, `Y`, `Position_DistanceUnit`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, X, Y, Position_DistanceUnit)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `position_` (`WID`, `DataSetWID`, `X`, `Y`, `Position_DistanceUnit`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, WID, DataSetWID, X, Y, Position_DistanceUnit)
        Else
        Return String.Format(REPLACE_SQL, WID, DataSetWID, X, Y, Position_DistanceUnit)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `position_` SET `WID`='{0}', `DataSetWID`='{1}', `X`='{2}', `Y`='{3}', `Position_DistanceUnit`='{4}' WHERE `WID` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, X, Y, Position_DistanceUnit, WID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As position_
                         Return DirectCast(MyClass.MemberwiseClone, position_)
                     End Function
End Class


End Namespace
