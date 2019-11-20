#Region "Microsoft.VisualBasic::8b3482cc01818bd0eb1951225d1e4f87, data\ExternalDBSource\MetaCyc\bio_warehouse\namevaluetype.vb"

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
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("namevaluetype", Database:="warehouse")>
Public Class namevaluetype: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DataSetWID As Long
    <DatabaseField("Name"), DataType(MySqlDbType.VarChar, "255")> Public Property Name As String
    <DatabaseField("Value"), DataType(MySqlDbType.VarChar, "255")> Public Property Value As String
    <DatabaseField("Type_"), DataType(MySqlDbType.VarChar, "255")> Public Property Type_ As String
    <DatabaseField("NameValueType_PropertySets"), DataType(MySqlDbType.Int64, "20")> Public Property NameValueType_PropertySets As Long
    <DatabaseField("OtherWID"), DataType(MySqlDbType.Int64, "20")> Public Property OtherWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `namevaluetype` (`WID`, `DataSetWID`, `Name`, `Value`, `Type_`, `NameValueType_PropertySets`, `OtherWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `namevaluetype` (`WID`, `DataSetWID`, `Name`, `Value`, `Type_`, `NameValueType_PropertySets`, `OtherWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `namevaluetype` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `namevaluetype` SET `WID`='{0}', `DataSetWID`='{1}', `Name`='{2}', `Value`='{3}', `Type_`='{4}', `NameValueType_PropertySets`='{5}', `OtherWID`='{6}' WHERE `WID` = '{7}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, Name, Value, Type_, NameValueType_PropertySets, OtherWID)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, Name, Value, Type_, NameValueType_PropertySets, OtherWID)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, Name, Value, Type_, NameValueType_PropertySets, OtherWID, WID)
    End Function
#End Region
End Class


End Namespace
