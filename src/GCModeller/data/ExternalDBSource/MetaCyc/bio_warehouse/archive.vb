#Region "Microsoft.VisualBasic::4ebd402f4c410e68acc1598ee9a08a86, ..\GCModeller\data\ExternalDBSource\MetaCyc\bio_warehouse\archive.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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
''' DROP TABLE IF EXISTS `archive`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `archive` (
'''   `WID` bigint(20) NOT NULL,
'''   `OtherWID` bigint(20) NOT NULL,
'''   `Format` varchar(10) NOT NULL,
'''   `Contents` longblob,
'''   `URL` text,
'''   `ToolName` varchar(50) DEFAULT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   PRIMARY KEY (`WID`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("archive", Database:="warehouse")>
Public Class archive: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property WID As Long
    <DatabaseField("OtherWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property OtherWID As Long
    <DatabaseField("Format"), NotNull, DataType(MySqlDbType.VarChar, "10")> Public Property Format As String
    <DatabaseField("Contents"), DataType(MySqlDbType.Blob)> Public Property Contents As Byte()
    <DatabaseField("URL"), DataType(MySqlDbType.Text)> Public Property URL As String
    <DatabaseField("ToolName"), DataType(MySqlDbType.VarChar, "50")> Public Property ToolName As String
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DataSetWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `archive` (`WID`, `OtherWID`, `Format`, `Contents`, `URL`, `ToolName`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `archive` (`WID`, `OtherWID`, `Format`, `Contents`, `URL`, `ToolName`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `archive` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `archive` SET `WID`='{0}', `OtherWID`='{1}', `Format`='{2}', `Contents`='{3}', `URL`='{4}', `ToolName`='{5}', `DataSetWID`='{6}' WHERE `WID` = '{7}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, OtherWID, Format, Contents, URL, ToolName, DataSetWID)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, OtherWID, Format, Contents, URL, ToolName, DataSetWID)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, OtherWID, Format, Contents, URL, ToolName, DataSetWID, WID)
    End Function
#End Region
End Class


End Namespace
