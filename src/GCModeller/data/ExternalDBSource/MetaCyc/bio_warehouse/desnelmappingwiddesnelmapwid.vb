#Region "Microsoft.VisualBasic::5c5996a1a5c53f1c2d9f87da5ece656d, ExternalDBSource\MetaCyc\bio_warehouse\desnelmappingwiddesnelmapwid.vb"

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

    ' Class desnelmappingwiddesnelmapwid
    ' 
    '     Properties: DesignElementMappingWID, DesignElementMapWID
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
''' DROP TABLE IF EXISTS `desnelmappingwiddesnelmapwid`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `desnelmappingwiddesnelmapwid` (
'''   `DesignElementMappingWID` bigint(20) NOT NULL,
'''   `DesignElementMapWID` bigint(20) NOT NULL,
'''   KEY `FK_DesnElMappingWIDDesnElMap1` (`DesignElementMappingWID`),
'''   KEY `FK_DesnElMappingWIDDesnElMap2` (`DesignElementMapWID`),
'''   CONSTRAINT `FK_DesnElMappingWIDDesnElMap1` FOREIGN KEY (`DesignElementMappingWID`) REFERENCES `designelementmapping` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_DesnElMappingWIDDesnElMap2` FOREIGN KEY (`DesignElementMapWID`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("desnelmappingwiddesnelmapwid", Database:="warehouse")>
Public Class desnelmappingwiddesnelmapwid: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DesignElementMappingWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DesignElementMappingWID As Long
    <DatabaseField("DesignElementMapWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DesignElementMapWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `desnelmappingwiddesnelmapwid` (`DesignElementMappingWID`, `DesignElementMapWID`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `desnelmappingwiddesnelmapwid` (`DesignElementMappingWID`, `DesignElementMapWID`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `desnelmappingwiddesnelmapwid` WHERE `DesignElementMappingWID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `desnelmappingwiddesnelmapwid` SET `DesignElementMappingWID`='{0}', `DesignElementMapWID`='{1}' WHERE `DesignElementMappingWID` = '{2}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DesignElementMappingWID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DesignElementMappingWID, DesignElementMapWID)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DesignElementMappingWID, DesignElementMapWID)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DesignElementMappingWID, DesignElementMapWID, DesignElementMappingWID)
    End Function
#End Region
End Class


End Namespace
