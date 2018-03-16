#Region "Microsoft.VisualBasic::38a4c72f301abb63929087b8e266c7ee, data\ExternalDBSource\MetaCyc\bio_warehouse\hardwarewidsoftwarewid.vb"

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

    ' Class hardwarewidsoftwarewid
    ' 
    '     Properties: HardwareWID, SoftwareWID
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
''' DROP TABLE IF EXISTS `hardwarewidsoftwarewid`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `hardwarewidsoftwarewid` (
'''   `HardwareWID` bigint(20) NOT NULL,
'''   `SoftwareWID` bigint(20) NOT NULL,
'''   KEY `FK_HardwareWIDSoftwareWID1` (`HardwareWID`),
'''   KEY `FK_HardwareWIDSoftwareWID2` (`SoftwareWID`),
'''   CONSTRAINT `FK_HardwareWIDSoftwareWID1` FOREIGN KEY (`HardwareWID`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_HardwareWIDSoftwareWID2` FOREIGN KEY (`SoftwareWID`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("hardwarewidsoftwarewid", Database:="warehouse")>
Public Class hardwarewidsoftwarewid: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("HardwareWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property HardwareWID As Long
    <DatabaseField("SoftwareWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property SoftwareWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `hardwarewidsoftwarewid` (`HardwareWID`, `SoftwareWID`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `hardwarewidsoftwarewid` (`HardwareWID`, `SoftwareWID`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `hardwarewidsoftwarewid` WHERE `HardwareWID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `hardwarewidsoftwarewid` SET `HardwareWID`='{0}', `SoftwareWID`='{1}' WHERE `HardwareWID` = '{2}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, HardwareWID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, HardwareWID, SoftwareWID)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, HardwareWID, SoftwareWID)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, HardwareWID, SoftwareWID, HardwareWID)
    End Function
#End Region
End Class


End Namespace
