#Region "Microsoft.VisualBasic::99b391c8f47f540083e8c0795a774feb, data\ExternalDBSource\MetaCyc\MySQL\parametervalue.vb"

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

    ' Class parametervalue
    ' 
    '     Properties: DataSetWID, ParameterizableApplication, ParameterType, Value, WID
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
''' DROP TABLE IF EXISTS `parametervalue`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `parametervalue` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `Value` varchar(255) DEFAULT NULL,
'''   `ParameterType` bigint(20) DEFAULT NULL,
'''   `ParameterizableApplication` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_ParameterValue1` (`DataSetWID`),
'''   KEY `FK_ParameterValue2` (`ParameterType`),
'''   KEY `FK_ParameterValue3` (`ParameterizableApplication`),
'''   CONSTRAINT `FK_ParameterValue1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_ParameterValue2` FOREIGN KEY (`ParameterType`) REFERENCES `parameter` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_ParameterValue3` FOREIGN KEY (`ParameterizableApplication`) REFERENCES `parameterizableapplication` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("parametervalue", Database:="warehouse", SchemaSQL:="
CREATE TABLE `parametervalue` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `Value` varchar(255) DEFAULT NULL,
  `ParameterType` bigint(20) DEFAULT NULL,
  `ParameterizableApplication` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_ParameterValue1` (`DataSetWID`),
  KEY `FK_ParameterValue2` (`ParameterType`),
  KEY `FK_ParameterValue3` (`ParameterizableApplication`),
  CONSTRAINT `FK_ParameterValue1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ParameterValue2` FOREIGN KEY (`ParameterType`) REFERENCES `parameter` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ParameterValue3` FOREIGN KEY (`ParameterizableApplication`) REFERENCES `parameterizableapplication` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class parametervalue: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DataSetWID")> Public Property DataSetWID As Long
    <DatabaseField("Value"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="Value")> Public Property Value As String
    <DatabaseField("ParameterType"), DataType(MySqlDbType.Int64, "20"), Column(Name:="ParameterType")> Public Property ParameterType As Long
    <DatabaseField("ParameterizableApplication"), DataType(MySqlDbType.Int64, "20"), Column(Name:="ParameterizableApplication")> Public Property ParameterizableApplication As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `parametervalue` (`WID`, `DataSetWID`, `Value`, `ParameterType`, `ParameterizableApplication`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `parametervalue` (`WID`, `DataSetWID`, `Value`, `ParameterType`, `ParameterizableApplication`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `parametervalue` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `parametervalue` SET `WID`='{0}', `DataSetWID`='{1}', `Value`='{2}', `ParameterType`='{3}', `ParameterizableApplication`='{4}' WHERE `WID` = '{5}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `parametervalue` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `parametervalue` (`WID`, `DataSetWID`, `Value`, `ParameterType`, `ParameterizableApplication`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, Value, ParameterType, ParameterizableApplication)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{WID}', '{DataSetWID}', '{Value}', '{ParameterType}', '{ParameterizableApplication}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `parametervalue` (`WID`, `DataSetWID`, `Value`, `ParameterType`, `ParameterizableApplication`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, Value, ParameterType, ParameterizableApplication)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `parametervalue` SET `WID`='{0}', `DataSetWID`='{1}', `Value`='{2}', `ParameterType`='{3}', `ParameterizableApplication`='{4}' WHERE `WID` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, Value, ParameterType, ParameterizableApplication, WID)
    End Function
#End Region
Public Function Clone() As parametervalue
                  Return DirectCast(MyClass.MemberwiseClone, parametervalue)
              End Function
End Class


End Namespace

