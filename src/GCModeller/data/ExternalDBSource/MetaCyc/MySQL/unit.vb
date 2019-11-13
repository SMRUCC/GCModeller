#Region "Microsoft.VisualBasic::6a077a57698dd706d9c7ea0052eab2af, data\ExternalDBSource\MetaCyc\MySQL\unit.vb"

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

    ' Class unit
    ' 
    '     Properties: DataSetWID, MAGEClass, UnitName, UnitNameCV, UnitNameCV2
    '                 UnitNameCV3, UnitNameCV4, UnitNameCV5, UnitNameCV6, UnitNameCV7
    '                 WID
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
''' DROP TABLE IF EXISTS `unit`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `unit` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `MAGEClass` varchar(100) NOT NULL,
'''   `UnitName` varchar(255) DEFAULT NULL,
'''   `UnitNameCV` varchar(25) DEFAULT NULL,
'''   `UnitNameCV2` varchar(25) DEFAULT NULL,
'''   `UnitNameCV3` varchar(25) DEFAULT NULL,
'''   `UnitNameCV4` varchar(25) DEFAULT NULL,
'''   `UnitNameCV5` varchar(25) DEFAULT NULL,
'''   `UnitNameCV6` varchar(25) DEFAULT NULL,
'''   `UnitNameCV7` varchar(25) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_Unit1` (`DataSetWID`),
'''   CONSTRAINT `FK_Unit1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("unit", Database:="warehouse", SchemaSQL:="
CREATE TABLE `unit` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `MAGEClass` varchar(100) NOT NULL,
  `UnitName` varchar(255) DEFAULT NULL,
  `UnitNameCV` varchar(25) DEFAULT NULL,
  `UnitNameCV2` varchar(25) DEFAULT NULL,
  `UnitNameCV3` varchar(25) DEFAULT NULL,
  `UnitNameCV4` varchar(25) DEFAULT NULL,
  `UnitNameCV5` varchar(25) DEFAULT NULL,
  `UnitNameCV6` varchar(25) DEFAULT NULL,
  `UnitNameCV7` varchar(25) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_Unit1` (`DataSetWID`),
  CONSTRAINT `FK_Unit1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class unit: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DataSetWID")> Public Property DataSetWID As Long
    <DatabaseField("MAGEClass"), NotNull, DataType(MySqlDbType.VarChar, "100"), Column(Name:="MAGEClass")> Public Property MAGEClass As String
    <DatabaseField("UnitName"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="UnitName")> Public Property UnitName As String
    <DatabaseField("UnitNameCV"), DataType(MySqlDbType.VarChar, "25"), Column(Name:="UnitNameCV")> Public Property UnitNameCV As String
    <DatabaseField("UnitNameCV2"), DataType(MySqlDbType.VarChar, "25"), Column(Name:="UnitNameCV2")> Public Property UnitNameCV2 As String
    <DatabaseField("UnitNameCV3"), DataType(MySqlDbType.VarChar, "25"), Column(Name:="UnitNameCV3")> Public Property UnitNameCV3 As String
    <DatabaseField("UnitNameCV4"), DataType(MySqlDbType.VarChar, "25"), Column(Name:="UnitNameCV4")> Public Property UnitNameCV4 As String
    <DatabaseField("UnitNameCV5"), DataType(MySqlDbType.VarChar, "25"), Column(Name:="UnitNameCV5")> Public Property UnitNameCV5 As String
    <DatabaseField("UnitNameCV6"), DataType(MySqlDbType.VarChar, "25"), Column(Name:="UnitNameCV6")> Public Property UnitNameCV6 As String
    <DatabaseField("UnitNameCV7"), DataType(MySqlDbType.VarChar, "25"), Column(Name:="UnitNameCV7")> Public Property UnitNameCV7 As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `unit` (`WID`, `DataSetWID`, `MAGEClass`, `UnitName`, `UnitNameCV`, `UnitNameCV2`, `UnitNameCV3`, `UnitNameCV4`, `UnitNameCV5`, `UnitNameCV6`, `UnitNameCV7`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `unit` (`WID`, `DataSetWID`, `MAGEClass`, `UnitName`, `UnitNameCV`, `UnitNameCV2`, `UnitNameCV3`, `UnitNameCV4`, `UnitNameCV5`, `UnitNameCV6`, `UnitNameCV7`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `unit` (`WID`, `DataSetWID`, `MAGEClass`, `UnitName`, `UnitNameCV`, `UnitNameCV2`, `UnitNameCV3`, `UnitNameCV4`, `UnitNameCV5`, `UnitNameCV6`, `UnitNameCV7`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `unit` (`WID`, `DataSetWID`, `MAGEClass`, `UnitName`, `UnitNameCV`, `UnitNameCV2`, `UnitNameCV3`, `UnitNameCV4`, `UnitNameCV5`, `UnitNameCV6`, `UnitNameCV7`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `unit` WHERE `WID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `unit` SET `WID`='{0}', `DataSetWID`='{1}', `MAGEClass`='{2}', `UnitName`='{3}', `UnitNameCV`='{4}', `UnitNameCV2`='{5}', `UnitNameCV3`='{6}', `UnitNameCV4`='{7}', `UnitNameCV5`='{8}', `UnitNameCV6`='{9}', `UnitNameCV7`='{10}' WHERE `WID` = '{11}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `unit` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `unit` (`WID`, `DataSetWID`, `MAGEClass`, `UnitName`, `UnitNameCV`, `UnitNameCV2`, `UnitNameCV3`, `UnitNameCV4`, `UnitNameCV5`, `UnitNameCV6`, `UnitNameCV7`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, MAGEClass, UnitName, UnitNameCV, UnitNameCV2, UnitNameCV3, UnitNameCV4, UnitNameCV5, UnitNameCV6, UnitNameCV7)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `unit` (`WID`, `DataSetWID`, `MAGEClass`, `UnitName`, `UnitNameCV`, `UnitNameCV2`, `UnitNameCV3`, `UnitNameCV4`, `UnitNameCV5`, `UnitNameCV6`, `UnitNameCV7`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, WID, DataSetWID, MAGEClass, UnitName, UnitNameCV, UnitNameCV2, UnitNameCV3, UnitNameCV4, UnitNameCV5, UnitNameCV6, UnitNameCV7)
        Else
        Return String.Format(INSERT_SQL, WID, DataSetWID, MAGEClass, UnitName, UnitNameCV, UnitNameCV2, UnitNameCV3, UnitNameCV4, UnitNameCV5, UnitNameCV6, UnitNameCV7)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{WID}', '{DataSetWID}', '{MAGEClass}', '{UnitName}', '{UnitNameCV}', '{UnitNameCV2}', '{UnitNameCV3}', '{UnitNameCV4}', '{UnitNameCV5}', '{UnitNameCV6}', '{UnitNameCV7}')"
        Else
            Return $"('{WID}', '{DataSetWID}', '{MAGEClass}', '{UnitName}', '{UnitNameCV}', '{UnitNameCV2}', '{UnitNameCV3}', '{UnitNameCV4}', '{UnitNameCV5}', '{UnitNameCV6}', '{UnitNameCV7}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `unit` (`WID`, `DataSetWID`, `MAGEClass`, `UnitName`, `UnitNameCV`, `UnitNameCV2`, `UnitNameCV3`, `UnitNameCV4`, `UnitNameCV5`, `UnitNameCV6`, `UnitNameCV7`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, MAGEClass, UnitName, UnitNameCV, UnitNameCV2, UnitNameCV3, UnitNameCV4, UnitNameCV5, UnitNameCV6, UnitNameCV7)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `unit` (`WID`, `DataSetWID`, `MAGEClass`, `UnitName`, `UnitNameCV`, `UnitNameCV2`, `UnitNameCV3`, `UnitNameCV4`, `UnitNameCV5`, `UnitNameCV6`, `UnitNameCV7`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, WID, DataSetWID, MAGEClass, UnitName, UnitNameCV, UnitNameCV2, UnitNameCV3, UnitNameCV4, UnitNameCV5, UnitNameCV6, UnitNameCV7)
        Else
        Return String.Format(REPLACE_SQL, WID, DataSetWID, MAGEClass, UnitName, UnitNameCV, UnitNameCV2, UnitNameCV3, UnitNameCV4, UnitNameCV5, UnitNameCV6, UnitNameCV7)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `unit` SET `WID`='{0}', `DataSetWID`='{1}', `MAGEClass`='{2}', `UnitName`='{3}', `UnitNameCV`='{4}', `UnitNameCV2`='{5}', `UnitNameCV3`='{6}', `UnitNameCV4`='{7}', `UnitNameCV5`='{8}', `UnitNameCV6`='{9}', `UnitNameCV7`='{10}' WHERE `WID` = '{11}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, MAGEClass, UnitName, UnitNameCV, UnitNameCV2, UnitNameCV3, UnitNameCV4, UnitNameCV5, UnitNameCV6, UnitNameCV7, WID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As unit
                         Return DirectCast(MyClass.MemberwiseClone, unit)
                     End Function
End Class


End Namespace
