#Region "Microsoft.VisualBasic::b98ccaf43999cd480c9264c0118bab85, ..\GCModeller\data\ExternalDBSource\MetaCyc\bio_warehouse\bioassaywidchannelwid.vb"

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
''' DROP TABLE IF EXISTS `bioassaywidchannelwid`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `bioassaywidchannelwid` (
'''   `BioAssayWID` bigint(20) NOT NULL,
'''   `ChannelWID` bigint(20) NOT NULL,
'''   KEY `FK_BioAssayWIDChannelWID1` (`BioAssayWID`),
'''   KEY `FK_BioAssayWIDChannelWID2` (`ChannelWID`),
'''   CONSTRAINT `FK_BioAssayWIDChannelWID1` FOREIGN KEY (`BioAssayWID`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioAssayWIDChannelWID2` FOREIGN KEY (`ChannelWID`) REFERENCES `channel` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("bioassaywidchannelwid", Database:="warehouse")>
Public Class bioassaywidchannelwid: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("BioAssayWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property BioAssayWID As Long
    <DatabaseField("ChannelWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property ChannelWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `bioassaywidchannelwid` (`BioAssayWID`, `ChannelWID`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `bioassaywidchannelwid` (`BioAssayWID`, `ChannelWID`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `bioassaywidchannelwid` WHERE `BioAssayWID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `bioassaywidchannelwid` SET `BioAssayWID`='{0}', `ChannelWID`='{1}' WHERE `BioAssayWID` = '{2}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, BioAssayWID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, BioAssayWID, ChannelWID)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, BioAssayWID, ChannelWID)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, BioAssayWID, ChannelWID, BioAssayWID)
    End Function
#End Region
End Class


End Namespace
