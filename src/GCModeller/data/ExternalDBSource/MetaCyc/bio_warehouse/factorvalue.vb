#Region "Microsoft.VisualBasic::82592d39797795ee1074d5864ebd4cf6, data\ExternalDBSource\MetaCyc\bio_warehouse\factorvalue.vb"

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
    '     Function: GetDeleteSQL, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:02:47 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MetaCyc.MySQL

''' <summary>
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
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("factorvalue", Database:="warehouse")>
Public Class factorvalue: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DataSetWID As Long
    <DatabaseField("Identifier"), DataType(MySqlDbType.VarChar, "255")> Public Property Identifier As String
    <DatabaseField("Name"), DataType(MySqlDbType.VarChar, "255")> Public Property Name As String
    <DatabaseField("ExperimentalFactor"), DataType(MySqlDbType.Int64, "20")> Public Property ExperimentalFactor As Long
    <DatabaseField("ExperimentalFactor2"), DataType(MySqlDbType.Int64, "20")> Public Property ExperimentalFactor2 As Long
    <DatabaseField("FactorValue_Measurement"), DataType(MySqlDbType.Int64, "20")> Public Property FactorValue_Measurement As Long
    <DatabaseField("FactorValue_Value"), DataType(MySqlDbType.Int64, "20")> Public Property FactorValue_Value As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `factorvalue` (`WID`, `DataSetWID`, `Identifier`, `Name`, `ExperimentalFactor`, `ExperimentalFactor2`, `FactorValue_Measurement`, `FactorValue_Value`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `factorvalue` (`WID`, `DataSetWID`, `Identifier`, `Name`, `ExperimentalFactor`, `ExperimentalFactor2`, `FactorValue_Measurement`, `FactorValue_Value`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `factorvalue` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `factorvalue` SET `WID`='{0}', `DataSetWID`='{1}', `Identifier`='{2}', `Name`='{3}', `ExperimentalFactor`='{4}', `ExperimentalFactor2`='{5}', `FactorValue_Measurement`='{6}', `FactorValue_Value`='{7}' WHERE `WID` = '{8}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, Identifier, Name, ExperimentalFactor, ExperimentalFactor2, FactorValue_Measurement, FactorValue_Value)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, Identifier, Name, ExperimentalFactor, ExperimentalFactor2, FactorValue_Measurement, FactorValue_Value)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, Identifier, Name, ExperimentalFactor, ExperimentalFactor2, FactorValue_Measurement, FactorValue_Value, WID)
    End Function
#End Region
End Class


End Namespace
