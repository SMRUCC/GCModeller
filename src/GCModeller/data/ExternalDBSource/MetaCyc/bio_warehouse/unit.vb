#Region "Microsoft.VisualBasic::5dd0982722ef02cf91f339ceee874a02, ExternalDBSource\MetaCyc\bio_warehouse\unit.vb"

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
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("unit", Database:="warehouse")>
Public Class unit: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DataSetWID As Long
    <DatabaseField("MAGEClass"), NotNull, DataType(MySqlDbType.VarChar, "100")> Public Property MAGEClass As String
    <DatabaseField("UnitName"), DataType(MySqlDbType.VarChar, "255")> Public Property UnitName As String
    <DatabaseField("UnitNameCV"), DataType(MySqlDbType.VarChar, "25")> Public Property UnitNameCV As String
    <DatabaseField("UnitNameCV2"), DataType(MySqlDbType.VarChar, "25")> Public Property UnitNameCV2 As String
    <DatabaseField("UnitNameCV3"), DataType(MySqlDbType.VarChar, "25")> Public Property UnitNameCV3 As String
    <DatabaseField("UnitNameCV4"), DataType(MySqlDbType.VarChar, "25")> Public Property UnitNameCV4 As String
    <DatabaseField("UnitNameCV5"), DataType(MySqlDbType.VarChar, "25")> Public Property UnitNameCV5 As String
    <DatabaseField("UnitNameCV6"), DataType(MySqlDbType.VarChar, "25")> Public Property UnitNameCV6 As String
    <DatabaseField("UnitNameCV7"), DataType(MySqlDbType.VarChar, "25")> Public Property UnitNameCV7 As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `unit` (`WID`, `DataSetWID`, `MAGEClass`, `UnitName`, `UnitNameCV`, `UnitNameCV2`, `UnitNameCV3`, `UnitNameCV4`, `UnitNameCV5`, `UnitNameCV6`, `UnitNameCV7`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `unit` (`WID`, `DataSetWID`, `MAGEClass`, `UnitName`, `UnitNameCV`, `UnitNameCV2`, `UnitNameCV3`, `UnitNameCV4`, `UnitNameCV5`, `UnitNameCV6`, `UnitNameCV7`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `unit` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `unit` SET `WID`='{0}', `DataSetWID`='{1}', `MAGEClass`='{2}', `UnitName`='{3}', `UnitNameCV`='{4}', `UnitNameCV2`='{5}', `UnitNameCV3`='{6}', `UnitNameCV4`='{7}', `UnitNameCV5`='{8}', `UnitNameCV6`='{9}', `UnitNameCV7`='{10}' WHERE `WID` = '{11}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, MAGEClass, UnitName, UnitNameCV, UnitNameCV2, UnitNameCV3, UnitNameCV4, UnitNameCV5, UnitNameCV6, UnitNameCV7)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, MAGEClass, UnitName, UnitNameCV, UnitNameCV2, UnitNameCV3, UnitNameCV4, UnitNameCV5, UnitNameCV6, UnitNameCV7)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, MAGEClass, UnitName, UnitNameCV, UnitNameCV2, UnitNameCV3, UnitNameCV4, UnitNameCV5, UnitNameCV6, UnitNameCV7, WID)
    End Function
#End Region
End Class


End Namespace
