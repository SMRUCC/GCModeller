#Region "Microsoft.VisualBasic::5582b9bfaffb37adb15673dc80a919c5, data\ExternalDBSource\MetaCyc\bio_warehouse\nodevalue.vb"

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

    ' Class nodevalue
    ' 
    '     Properties: DataSetWID, Name, Node_NodeValue, NodeValue_DataType, NodeValue_Scale
    '                 NodeValue_Type, Value, WID
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
''' DROP TABLE IF EXISTS `nodevalue`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `nodevalue` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `Node_NodeValue` bigint(20) DEFAULT NULL,
'''   `Name` varchar(255) DEFAULT NULL,
'''   `Value` varchar(255) DEFAULT NULL,
'''   `NodeValue_Type` bigint(20) DEFAULT NULL,
'''   `NodeValue_Scale` bigint(20) DEFAULT NULL,
'''   `NodeValue_DataType` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_NodeValue1` (`DataSetWID`),
'''   KEY `FK_NodeValue2` (`Node_NodeValue`),
'''   KEY `FK_NodeValue3` (`NodeValue_Type`),
'''   KEY `FK_NodeValue4` (`NodeValue_Scale`),
'''   KEY `FK_NodeValue5` (`NodeValue_DataType`),
'''   CONSTRAINT `FK_NodeValue1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_NodeValue2` FOREIGN KEY (`Node_NodeValue`) REFERENCES `node` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_NodeValue3` FOREIGN KEY (`NodeValue_Type`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_NodeValue4` FOREIGN KEY (`NodeValue_Scale`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_NodeValue5` FOREIGN KEY (`NodeValue_DataType`) REFERENCES `term` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("nodevalue", Database:="warehouse")>
Public Class nodevalue: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DataSetWID As Long
    <DatabaseField("Node_NodeValue"), DataType(MySqlDbType.Int64, "20")> Public Property Node_NodeValue As Long
    <DatabaseField("Name"), DataType(MySqlDbType.VarChar, "255")> Public Property Name As String
    <DatabaseField("Value"), DataType(MySqlDbType.VarChar, "255")> Public Property Value As String
    <DatabaseField("NodeValue_Type"), DataType(MySqlDbType.Int64, "20")> Public Property NodeValue_Type As Long
    <DatabaseField("NodeValue_Scale"), DataType(MySqlDbType.Int64, "20")> Public Property NodeValue_Scale As Long
    <DatabaseField("NodeValue_DataType"), DataType(MySqlDbType.Int64, "20")> Public Property NodeValue_DataType As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `nodevalue` (`WID`, `DataSetWID`, `Node_NodeValue`, `Name`, `Value`, `NodeValue_Type`, `NodeValue_Scale`, `NodeValue_DataType`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `nodevalue` (`WID`, `DataSetWID`, `Node_NodeValue`, `Name`, `Value`, `NodeValue_Type`, `NodeValue_Scale`, `NodeValue_DataType`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `nodevalue` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `nodevalue` SET `WID`='{0}', `DataSetWID`='{1}', `Node_NodeValue`='{2}', `Name`='{3}', `Value`='{4}', `NodeValue_Type`='{5}', `NodeValue_Scale`='{6}', `NodeValue_DataType`='{7}' WHERE `WID` = '{8}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, Node_NodeValue, Name, Value, NodeValue_Type, NodeValue_Scale, NodeValue_DataType)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, Node_NodeValue, Name, Value, NodeValue_Type, NodeValue_Scale, NodeValue_DataType)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, Node_NodeValue, Name, Value, NodeValue_Type, NodeValue_Scale, NodeValue_DataType, WID)
    End Function
#End Region
End Class


End Namespace
