#Region "Microsoft.VisualBasic::28fcceae236c9ed4dc4a41f2364326ec, data\ExternalDBSource\MetaCyc\MySQL\measurement.vb"

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

    ' Class measurement
    ' 
    '     Properties: DataSetWID, KindCV, Measurement_Unit, OtherKind, Type_
    '                 Value, WID
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @3/16/2018 10:40:19 PM


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
''' DROP TABLE IF EXISTS `measurement`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `measurement` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `Type_` varchar(25) DEFAULT NULL,
'''   `Value` varchar(255) DEFAULT NULL,
'''   `KindCV` varchar(25) DEFAULT NULL,
'''   `OtherKind` varchar(255) DEFAULT NULL,
'''   `Measurement_Unit` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_Measurement1` (`DataSetWID`),
'''   KEY `FK_Measurement2` (`Measurement_Unit`),
'''   CONSTRAINT `FK_Measurement1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Measurement2` FOREIGN KEY (`Measurement_Unit`) REFERENCES `unit` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("measurement", Database:="warehouse", SchemaSQL:="
CREATE TABLE `measurement` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `Type_` varchar(25) DEFAULT NULL,
  `Value` varchar(255) DEFAULT NULL,
  `KindCV` varchar(25) DEFAULT NULL,
  `OtherKind` varchar(255) DEFAULT NULL,
  `Measurement_Unit` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_Measurement1` (`DataSetWID`),
  KEY `FK_Measurement2` (`Measurement_Unit`),
  CONSTRAINT `FK_Measurement1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Measurement2` FOREIGN KEY (`Measurement_Unit`) REFERENCES `unit` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class measurement: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DataSetWID")> Public Property DataSetWID As Long
    <DatabaseField("Type_"), DataType(MySqlDbType.VarChar, "25"), Column(Name:="Type_")> Public Property Type_ As String
    <DatabaseField("Value"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="Value")> Public Property Value As String
    <DatabaseField("KindCV"), DataType(MySqlDbType.VarChar, "25"), Column(Name:="KindCV")> Public Property KindCV As String
    <DatabaseField("OtherKind"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="OtherKind")> Public Property OtherKind As String
    <DatabaseField("Measurement_Unit"), DataType(MySqlDbType.Int64, "20"), Column(Name:="Measurement_Unit")> Public Property Measurement_Unit As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `measurement` (`WID`, `DataSetWID`, `Type_`, `Value`, `KindCV`, `OtherKind`, `Measurement_Unit`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `measurement` (`WID`, `DataSetWID`, `Type_`, `Value`, `KindCV`, `OtherKind`, `Measurement_Unit`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `measurement` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `measurement` SET `WID`='{0}', `DataSetWID`='{1}', `Type_`='{2}', `Value`='{3}', `KindCV`='{4}', `OtherKind`='{5}', `Measurement_Unit`='{6}' WHERE `WID` = '{7}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `measurement` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `measurement` (`WID`, `DataSetWID`, `Type_`, `Value`, `KindCV`, `OtherKind`, `Measurement_Unit`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, Type_, Value, KindCV, OtherKind, Measurement_Unit)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{WID}', '{DataSetWID}', '{Type_}', '{Value}', '{KindCV}', '{OtherKind}', '{Measurement_Unit}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `measurement` (`WID`, `DataSetWID`, `Type_`, `Value`, `KindCV`, `OtherKind`, `Measurement_Unit`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, Type_, Value, KindCV, OtherKind, Measurement_Unit)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `measurement` SET `WID`='{0}', `DataSetWID`='{1}', `Type_`='{2}', `Value`='{3}', `KindCV`='{4}', `OtherKind`='{5}', `Measurement_Unit`='{6}' WHERE `WID` = '{7}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, Type_, Value, KindCV, OtherKind, Measurement_Unit, WID)
    End Function
#End Region
Public Function Clone() As measurement
                  Return DirectCast(MyClass.MemberwiseClone, measurement)
              End Function
End Class


End Namespace

