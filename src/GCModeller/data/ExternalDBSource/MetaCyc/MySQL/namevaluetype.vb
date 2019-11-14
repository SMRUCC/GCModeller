#Region "Microsoft.VisualBasic::f067f1c4c951c875d4f434afeadddf22, data\ExternalDBSource\MetaCyc\MySQL\namevaluetype.vb"

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

    ' Class namevaluetype
    ' 
    '     Properties: DataSetWID, Name, NameValueType_PropertySets, OtherWID, Type_
    '                 Value, WID
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
''' DROP TABLE IF EXISTS `namevaluetype`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `namevaluetype` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `Name` varchar(255) DEFAULT NULL,
'''   `Value` varchar(255) DEFAULT NULL,
'''   `Type_` varchar(255) DEFAULT NULL,
'''   `NameValueType_PropertySets` bigint(20) DEFAULT NULL,
'''   `OtherWID` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_NameValueType1` (`DataSetWID`),
'''   KEY `FK_NameValueType66` (`NameValueType_PropertySets`),
'''   CONSTRAINT `FK_NameValueType1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_NameValueType66` FOREIGN KEY (`NameValueType_PropertySets`) REFERENCES `namevaluetype` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("namevaluetype", Database:="warehouse", SchemaSQL:="
CREATE TABLE `namevaluetype` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `Value` varchar(255) DEFAULT NULL,
  `Type_` varchar(255) DEFAULT NULL,
  `NameValueType_PropertySets` bigint(20) DEFAULT NULL,
  `OtherWID` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_NameValueType1` (`DataSetWID`),
  KEY `FK_NameValueType66` (`NameValueType_PropertySets`),
  CONSTRAINT `FK_NameValueType1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_NameValueType66` FOREIGN KEY (`NameValueType_PropertySets`) REFERENCES `namevaluetype` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class namevaluetype: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DataSetWID")> Public Property DataSetWID As Long
    <DatabaseField("Name"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="Name")> Public Property Name As String
    <DatabaseField("Value"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="Value")> Public Property Value As String
    <DatabaseField("Type_"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="Type_")> Public Property Type_ As String
    <DatabaseField("NameValueType_PropertySets"), DataType(MySqlDbType.Int64, "20"), Column(Name:="NameValueType_PropertySets")> Public Property NameValueType_PropertySets As Long
    <DatabaseField("OtherWID"), DataType(MySqlDbType.Int64, "20"), Column(Name:="OtherWID")> Public Property OtherWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `namevaluetype` (`WID`, `DataSetWID`, `Name`, `Value`, `Type_`, `NameValueType_PropertySets`, `OtherWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `namevaluetype` (`WID`, `DataSetWID`, `Name`, `Value`, `Type_`, `NameValueType_PropertySets`, `OtherWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `namevaluetype` (`WID`, `DataSetWID`, `Name`, `Value`, `Type_`, `NameValueType_PropertySets`, `OtherWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `namevaluetype` (`WID`, `DataSetWID`, `Name`, `Value`, `Type_`, `NameValueType_PropertySets`, `OtherWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `namevaluetype` WHERE `WID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `namevaluetype` SET `WID`='{0}', `DataSetWID`='{1}', `Name`='{2}', `Value`='{3}', `Type_`='{4}', `NameValueType_PropertySets`='{5}', `OtherWID`='{6}' WHERE `WID` = '{7}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `namevaluetype` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `namevaluetype` (`WID`, `DataSetWID`, `Name`, `Value`, `Type_`, `NameValueType_PropertySets`, `OtherWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, Name, Value, Type_, NameValueType_PropertySets, OtherWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `namevaluetype` (`WID`, `DataSetWID`, `Name`, `Value`, `Type_`, `NameValueType_PropertySets`, `OtherWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, WID, DataSetWID, Name, Value, Type_, NameValueType_PropertySets, OtherWID)
        Else
        Return String.Format(INSERT_SQL, WID, DataSetWID, Name, Value, Type_, NameValueType_PropertySets, OtherWID)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{WID}', '{DataSetWID}', '{Name}', '{Value}', '{Type_}', '{NameValueType_PropertySets}', '{OtherWID}')"
        Else
            Return $"('{WID}', '{DataSetWID}', '{Name}', '{Value}', '{Type_}', '{NameValueType_PropertySets}', '{OtherWID}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `namevaluetype` (`WID`, `DataSetWID`, `Name`, `Value`, `Type_`, `NameValueType_PropertySets`, `OtherWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, Name, Value, Type_, NameValueType_PropertySets, OtherWID)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `namevaluetype` (`WID`, `DataSetWID`, `Name`, `Value`, `Type_`, `NameValueType_PropertySets`, `OtherWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, WID, DataSetWID, Name, Value, Type_, NameValueType_PropertySets, OtherWID)
        Else
        Return String.Format(REPLACE_SQL, WID, DataSetWID, Name, Value, Type_, NameValueType_PropertySets, OtherWID)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `namevaluetype` SET `WID`='{0}', `DataSetWID`='{1}', `Name`='{2}', `Value`='{3}', `Type_`='{4}', `NameValueType_PropertySets`='{5}', `OtherWID`='{6}' WHERE `WID` = '{7}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, Name, Value, Type_, NameValueType_PropertySets, OtherWID, WID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As namevaluetype
                         Return DirectCast(MyClass.MemberwiseClone, namevaluetype)
                     End Function
End Class


End Namespace
