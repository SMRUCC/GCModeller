#Region "Microsoft.VisualBasic::d5995bb0106e03a7e29349b506e14671, data\ExternalDBSource\MetaCyc\MySQL\factorvalue.vb"

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

    ' Class factorvalue
    ' 
    '     Properties: DataSetWID, ExperimentalFactor, ExperimentalFactor2, FactorValue_Measurement, FactorValue_Value
    '                 Identifier, Name, WID
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
''' DROP TABLE IF EXISTS `factorvalue`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `factorvalue` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `Identifier` varchar(255) DEFAULT NULL,
'''   `Name` varchar(255) DEFAULT NULL,
'''   `ExperimentalFactor` bigint(20) DEFAULT NULL,
'''   `ExperimentalFactor2` bigint(20) DEFAULT NULL,
'''   `FactorValue_Measurement` bigint(20) DEFAULT NULL,
'''   `FactorValue_Value` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_FactorValue1` (`DataSetWID`),
'''   KEY `FK_FactorValue3` (`ExperimentalFactor`),
'''   KEY `FK_FactorValue4` (`ExperimentalFactor2`),
'''   KEY `FK_FactorValue5` (`FactorValue_Measurement`),
'''   KEY `FK_FactorValue6` (`FactorValue_Value`),
'''   CONSTRAINT `FK_FactorValue1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_FactorValue3` FOREIGN KEY (`ExperimentalFactor`) REFERENCES `experimentalfactor` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_FactorValue4` FOREIGN KEY (`ExperimentalFactor2`) REFERENCES `experimentalfactor` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_FactorValue5` FOREIGN KEY (`FactorValue_Measurement`) REFERENCES `measurement` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_FactorValue6` FOREIGN KEY (`FactorValue_Value`) REFERENCES `term` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("factorvalue", Database:="warehouse", SchemaSQL:="
CREATE TABLE `factorvalue` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `Identifier` varchar(255) DEFAULT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `ExperimentalFactor` bigint(20) DEFAULT NULL,
  `ExperimentalFactor2` bigint(20) DEFAULT NULL,
  `FactorValue_Measurement` bigint(20) DEFAULT NULL,
  `FactorValue_Value` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_FactorValue1` (`DataSetWID`),
  KEY `FK_FactorValue3` (`ExperimentalFactor`),
  KEY `FK_FactorValue4` (`ExperimentalFactor2`),
  KEY `FK_FactorValue5` (`FactorValue_Measurement`),
  KEY `FK_FactorValue6` (`FactorValue_Value`),
  CONSTRAINT `FK_FactorValue1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_FactorValue3` FOREIGN KEY (`ExperimentalFactor`) REFERENCES `experimentalfactor` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_FactorValue4` FOREIGN KEY (`ExperimentalFactor2`) REFERENCES `experimentalfactor` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_FactorValue5` FOREIGN KEY (`FactorValue_Measurement`) REFERENCES `measurement` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_FactorValue6` FOREIGN KEY (`FactorValue_Value`) REFERENCES `term` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class factorvalue: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DataSetWID")> Public Property DataSetWID As Long
    <DatabaseField("Identifier"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="Identifier")> Public Property Identifier As String
    <DatabaseField("Name"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="Name")> Public Property Name As String
    <DatabaseField("ExperimentalFactor"), DataType(MySqlDbType.Int64, "20"), Column(Name:="ExperimentalFactor")> Public Property ExperimentalFactor As Long
    <DatabaseField("ExperimentalFactor2"), DataType(MySqlDbType.Int64, "20"), Column(Name:="ExperimentalFactor2")> Public Property ExperimentalFactor2 As Long
    <DatabaseField("FactorValue_Measurement"), DataType(MySqlDbType.Int64, "20"), Column(Name:="FactorValue_Measurement")> Public Property FactorValue_Measurement As Long
    <DatabaseField("FactorValue_Value"), DataType(MySqlDbType.Int64, "20"), Column(Name:="FactorValue_Value")> Public Property FactorValue_Value As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `factorvalue` (`WID`, `DataSetWID`, `Identifier`, `Name`, `ExperimentalFactor`, `ExperimentalFactor2`, `FactorValue_Measurement`, `FactorValue_Value`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `factorvalue` (`WID`, `DataSetWID`, `Identifier`, `Name`, `ExperimentalFactor`, `ExperimentalFactor2`, `FactorValue_Measurement`, `FactorValue_Value`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `factorvalue` (`WID`, `DataSetWID`, `Identifier`, `Name`, `ExperimentalFactor`, `ExperimentalFactor2`, `FactorValue_Measurement`, `FactorValue_Value`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `factorvalue` (`WID`, `DataSetWID`, `Identifier`, `Name`, `ExperimentalFactor`, `ExperimentalFactor2`, `FactorValue_Measurement`, `FactorValue_Value`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `factorvalue` WHERE `WID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `factorvalue` SET `WID`='{0}', `DataSetWID`='{1}', `Identifier`='{2}', `Name`='{3}', `ExperimentalFactor`='{4}', `ExperimentalFactor2`='{5}', `FactorValue_Measurement`='{6}', `FactorValue_Value`='{7}' WHERE `WID` = '{8}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `factorvalue` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `factorvalue` (`WID`, `DataSetWID`, `Identifier`, `Name`, `ExperimentalFactor`, `ExperimentalFactor2`, `FactorValue_Measurement`, `FactorValue_Value`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, Identifier, Name, ExperimentalFactor, ExperimentalFactor2, FactorValue_Measurement, FactorValue_Value)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `factorvalue` (`WID`, `DataSetWID`, `Identifier`, `Name`, `ExperimentalFactor`, `ExperimentalFactor2`, `FactorValue_Measurement`, `FactorValue_Value`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, WID, DataSetWID, Identifier, Name, ExperimentalFactor, ExperimentalFactor2, FactorValue_Measurement, FactorValue_Value)
        Else
        Return String.Format(INSERT_SQL, WID, DataSetWID, Identifier, Name, ExperimentalFactor, ExperimentalFactor2, FactorValue_Measurement, FactorValue_Value)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{WID}', '{DataSetWID}', '{Identifier}', '{Name}', '{ExperimentalFactor}', '{ExperimentalFactor2}', '{FactorValue_Measurement}', '{FactorValue_Value}')"
        Else
            Return $"('{WID}', '{DataSetWID}', '{Identifier}', '{Name}', '{ExperimentalFactor}', '{ExperimentalFactor2}', '{FactorValue_Measurement}', '{FactorValue_Value}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `factorvalue` (`WID`, `DataSetWID`, `Identifier`, `Name`, `ExperimentalFactor`, `ExperimentalFactor2`, `FactorValue_Measurement`, `FactorValue_Value`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, Identifier, Name, ExperimentalFactor, ExperimentalFactor2, FactorValue_Measurement, FactorValue_Value)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `factorvalue` (`WID`, `DataSetWID`, `Identifier`, `Name`, `ExperimentalFactor`, `ExperimentalFactor2`, `FactorValue_Measurement`, `FactorValue_Value`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, WID, DataSetWID, Identifier, Name, ExperimentalFactor, ExperimentalFactor2, FactorValue_Measurement, FactorValue_Value)
        Else
        Return String.Format(REPLACE_SQL, WID, DataSetWID, Identifier, Name, ExperimentalFactor, ExperimentalFactor2, FactorValue_Measurement, FactorValue_Value)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `factorvalue` SET `WID`='{0}', `DataSetWID`='{1}', `Identifier`='{2}', `Name`='{3}', `ExperimentalFactor`='{4}', `ExperimentalFactor2`='{5}', `FactorValue_Measurement`='{6}', `FactorValue_Value`='{7}' WHERE `WID` = '{8}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, Identifier, Name, ExperimentalFactor, ExperimentalFactor2, FactorValue_Measurement, FactorValue_Value, WID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As factorvalue
                         Return DirectCast(MyClass.MemberwiseClone, factorvalue)
                     End Function
End Class


End Namespace
