#Region "Microsoft.VisualBasic::071cf4e6de17c3f4a4d6d225e227738f, ExternalDBSource\MetaCyc\bio_warehouse\dbid.vb"

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

    ' Class dbid
    ' 
    '     Properties: OtherWID, Type, Version, XID
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
''' DROP TABLE IF EXISTS `dbid`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `dbid` (
'''   `OtherWID` bigint(20) NOT NULL,
'''   `XID` varchar(150) NOT NULL,
'''   `Type` varchar(20) DEFAULT NULL,
'''   `Version` varchar(10) DEFAULT NULL,
'''   KEY `DBID_XID_OTHERWID` (`XID`,`OtherWID`),
'''   KEY `DBID_OTHERWID` (`OtherWID`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("dbid", Database:="warehouse")>
Public Class dbid: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("OtherWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property OtherWID As Long
    <DatabaseField("XID"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "150")> Public Property XID As String
    <DatabaseField("Type"), DataType(MySqlDbType.VarChar, "20")> Public Property Type As String
    <DatabaseField("Version"), DataType(MySqlDbType.VarChar, "10")> Public Property Version As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `dbid` (`OtherWID`, `XID`, `Type`, `Version`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `dbid` (`OtherWID`, `XID`, `Type`, `Version`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `dbid` WHERE `XID`='{0}' and `OtherWID`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `dbid` SET `OtherWID`='{0}', `XID`='{1}', `Type`='{2}', `Version`='{3}' WHERE `XID`='{4}' and `OtherWID`='{5}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, XID, OtherWID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, OtherWID, XID, Type, Version)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, OtherWID, XID, Type, Version)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, OtherWID, XID, Type, Version, XID, OtherWID)
    End Function
#End Region
End Class


End Namespace
