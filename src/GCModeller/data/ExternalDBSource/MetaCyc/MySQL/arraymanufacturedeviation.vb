#Region "Microsoft.VisualBasic::386318fd0b5fc4ef36759df2447d5a03, data\ExternalDBSource\MetaCyc\MySQL\arraymanufacturedeviation.vb"

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

    ' Class arraymanufacturedeviation
    ' 
    '     Properties: Array_, DataSetWID, WID
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
''' DROP TABLE IF EXISTS `arraymanufacturedeviation`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `arraymanufacturedeviation` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `Array_` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_ArrayManufactureDeviation1` (`DataSetWID`),
'''   KEY `FK_ArrayManufactureDeviation2` (`Array_`),
'''   CONSTRAINT `FK_ArrayManufactureDeviation1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_ArrayManufactureDeviation2` FOREIGN KEY (`Array_`) REFERENCES `array_` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("arraymanufacturedeviation", Database:="warehouse", SchemaSQL:="
CREATE TABLE `arraymanufacturedeviation` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `Array_` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_ArrayManufactureDeviation1` (`DataSetWID`),
  KEY `FK_ArrayManufactureDeviation2` (`Array_`),
  CONSTRAINT `FK_ArrayManufactureDeviation1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ArrayManufactureDeviation2` FOREIGN KEY (`Array_`) REFERENCES `array_` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class arraymanufacturedeviation: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DataSetWID")> Public Property DataSetWID As Long
    <DatabaseField("Array_"), DataType(MySqlDbType.Int64, "20"), Column(Name:="Array_")> Public Property Array_ As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `arraymanufacturedeviation` (`WID`, `DataSetWID`, `Array_`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `arraymanufacturedeviation` (`WID`, `DataSetWID`, `Array_`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `arraymanufacturedeviation` (`WID`, `DataSetWID`, `Array_`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `arraymanufacturedeviation` (`WID`, `DataSetWID`, `Array_`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `arraymanufacturedeviation` WHERE `WID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `arraymanufacturedeviation` SET `WID`='{0}', `DataSetWID`='{1}', `Array_`='{2}' WHERE `WID` = '{3}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `arraymanufacturedeviation` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `arraymanufacturedeviation` (`WID`, `DataSetWID`, `Array_`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, Array_)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `arraymanufacturedeviation` (`WID`, `DataSetWID`, `Array_`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, WID, DataSetWID, Array_)
        Else
        Return String.Format(INSERT_SQL, WID, DataSetWID, Array_)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{WID}', '{DataSetWID}', '{Array_}')"
        Else
            Return $"('{WID}', '{DataSetWID}', '{Array_}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `arraymanufacturedeviation` (`WID`, `DataSetWID`, `Array_`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, Array_)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `arraymanufacturedeviation` (`WID`, `DataSetWID`, `Array_`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, WID, DataSetWID, Array_)
        Else
        Return String.Format(REPLACE_SQL, WID, DataSetWID, Array_)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `arraymanufacturedeviation` SET `WID`='{0}', `DataSetWID`='{1}', `Array_`='{2}' WHERE `WID` = '{3}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, Array_, WID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As arraymanufacturedeviation
                         Return DirectCast(MyClass.MemberwiseClone, arraymanufacturedeviation)
                     End Function
End Class


End Namespace
