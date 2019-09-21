#Region "Microsoft.VisualBasic::ec56e1fc9b388a7e561e703ffe8e3231, ExternalDBSource\MetaCyc\bio_warehouse\parameter.vb"

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

    ' Class parameter
    ' 
    '     Properties: DataSetWID, Identifier, Name, Parameter_DataType, Parameter_DefaultValue
    '                 Parameterizable_ParameterTypes, WID
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
''' DROP TABLE IF EXISTS `parameter`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `parameter` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `Identifier` varchar(255) DEFAULT NULL,
'''   `Name` varchar(255) DEFAULT NULL,
'''   `Parameter_DefaultValue` bigint(20) DEFAULT NULL,
'''   `Parameter_DataType` bigint(20) DEFAULT NULL,
'''   `Parameterizable_ParameterTypes` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_Parameter1` (`DataSetWID`),
'''   KEY `FK_Parameter3` (`Parameter_DefaultValue`),
'''   KEY `FK_Parameter4` (`Parameter_DataType`),
'''   KEY `FK_Parameter5` (`Parameterizable_ParameterTypes`),
'''   CONSTRAINT `FK_Parameter1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Parameter3` FOREIGN KEY (`Parameter_DefaultValue`) REFERENCES `measurement` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Parameter4` FOREIGN KEY (`Parameter_DataType`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Parameter5` FOREIGN KEY (`Parameterizable_ParameterTypes`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("parameter", Database:="warehouse")>
Public Class parameter: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DataSetWID As Long
    <DatabaseField("Identifier"), DataType(MySqlDbType.VarChar, "255")> Public Property Identifier As String
    <DatabaseField("Name"), DataType(MySqlDbType.VarChar, "255")> Public Property Name As String
    <DatabaseField("Parameter_DefaultValue"), DataType(MySqlDbType.Int64, "20")> Public Property Parameter_DefaultValue As Long
    <DatabaseField("Parameter_DataType"), DataType(MySqlDbType.Int64, "20")> Public Property Parameter_DataType As Long
    <DatabaseField("Parameterizable_ParameterTypes"), DataType(MySqlDbType.Int64, "20")> Public Property Parameterizable_ParameterTypes As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `parameter` (`WID`, `DataSetWID`, `Identifier`, `Name`, `Parameter_DefaultValue`, `Parameter_DataType`, `Parameterizable_ParameterTypes`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `parameter` (`WID`, `DataSetWID`, `Identifier`, `Name`, `Parameter_DefaultValue`, `Parameter_DataType`, `Parameterizable_ParameterTypes`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `parameter` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `parameter` SET `WID`='{0}', `DataSetWID`='{1}', `Identifier`='{2}', `Name`='{3}', `Parameter_DefaultValue`='{4}', `Parameter_DataType`='{5}', `Parameterizable_ParameterTypes`='{6}' WHERE `WID` = '{7}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, Identifier, Name, Parameter_DefaultValue, Parameter_DataType, Parameterizable_ParameterTypes)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, Identifier, Name, Parameter_DefaultValue, Parameter_DataType, Parameterizable_ParameterTypes)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, Identifier, Name, Parameter_DefaultValue, Parameter_DataType, Parameterizable_ParameterTypes, WID)
    End Function
#End Region
End Class


End Namespace
