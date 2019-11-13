#Region "Microsoft.VisualBasic::5cd6e96c104bdd6ad42777d9b590bfd2, data\ExternalDBSource\MetaCyc\bio_warehouse\experimentdata.vb"

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

    ' Class experimentdata
    ' 
    '     Properties: Data, DataSetWID, DateProduced, ExperimentWID, Kind
    '                 MageData, OtherWID, Role, WID
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
''' DROP TABLE IF EXISTS `experimentdata`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `experimentdata` (
'''   `WID` bigint(20) NOT NULL,
'''   `ExperimentWID` bigint(20) NOT NULL,
'''   `Data` longtext,
'''   `MageData` bigint(20) DEFAULT NULL,
'''   `Role` varchar(50) NOT NULL,
'''   `Kind` char(1) NOT NULL,
'''   `DateProduced` datetime DEFAULT NULL,
'''   `OtherWID` bigint(20) DEFAULT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_ExpData1` (`ExperimentWID`),
'''   KEY `FK_ExpDataMD` (`MageData`),
'''   KEY `FK_ExpData2` (`DataSetWID`),
'''   CONSTRAINT `FK_ExpData1` FOREIGN KEY (`ExperimentWID`) REFERENCES `experiment` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_ExpData2` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_ExpDataMD` FOREIGN KEY (`MageData`) REFERENCES `parametervalue` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("experimentdata", Database:="warehouse")>
Public Class experimentdata: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property WID As Long
    <DatabaseField("ExperimentWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property ExperimentWID As Long
    <DatabaseField("Data"), DataType(MySqlDbType.Text)> Public Property Data As String
    <DatabaseField("MageData"), DataType(MySqlDbType.Int64, "20")> Public Property MageData As Long
    <DatabaseField("Role"), NotNull, DataType(MySqlDbType.VarChar, "50")> Public Property Role As String
    <DatabaseField("Kind"), NotNull, DataType(MySqlDbType.VarChar, "1")> Public Property Kind As String
    <DatabaseField("DateProduced"), DataType(MySqlDbType.DateTime)> Public Property DateProduced As Date
    <DatabaseField("OtherWID"), DataType(MySqlDbType.Int64, "20")> Public Property OtherWID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DataSetWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `experimentdata` (`WID`, `ExperimentWID`, `Data`, `MageData`, `Role`, `Kind`, `DateProduced`, `OtherWID`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `experimentdata` (`WID`, `ExperimentWID`, `Data`, `MageData`, `Role`, `Kind`, `DateProduced`, `OtherWID`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `experimentdata` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `experimentdata` SET `WID`='{0}', `ExperimentWID`='{1}', `Data`='{2}', `MageData`='{3}', `Role`='{4}', `Kind`='{5}', `DateProduced`='{6}', `OtherWID`='{7}', `DataSetWID`='{8}' WHERE `WID` = '{9}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, ExperimentWID, Data, MageData, Role, Kind, DataType.ToMySqlDateTimeString(DateProduced), OtherWID, DataSetWID)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, ExperimentWID, Data, MageData, Role, Kind, DataType.ToMySqlDateTimeString(DateProduced), OtherWID, DataSetWID)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, ExperimentWID, Data, MageData, Role, Kind, DataType.ToMySqlDateTimeString(DateProduced), OtherWID, DataSetWID, WID)
    End Function
#End Region
End Class


End Namespace
