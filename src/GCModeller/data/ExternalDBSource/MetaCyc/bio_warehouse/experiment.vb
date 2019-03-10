#Region "Microsoft.VisualBasic::1c2d2d39ab211728464ba908d69983a2, data\ExternalDBSource\MetaCyc\bio_warehouse\experiment.vb"

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

    ' Class experiment
    ' 
    '     Properties: ArchiveWID, BioSourceWID, ContactWID, DataSetWID, Description
    '                 EndDate, GroupIndex, GroupSize, GroupType, GroupWID
    '                 StartDate, TimePoint, TimeUnit, Type, WID
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
''' DROP TABLE IF EXISTS `experiment`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `experiment` (
'''   `WID` bigint(20) NOT NULL,
'''   `Type` varchar(50) NOT NULL,
'''   `ContactWID` bigint(20) DEFAULT NULL,
'''   `ArchiveWID` bigint(20) DEFAULT NULL,
'''   `StartDate` datetime DEFAULT NULL,
'''   `EndDate` datetime DEFAULT NULL,
'''   `Description` text,
'''   `GroupWID` bigint(20) DEFAULT NULL,
'''   `GroupType` varchar(50) DEFAULT NULL,
'''   `GroupSize` int(11) NOT NULL,
'''   `GroupIndex` int(11) DEFAULT NULL,
'''   `TimePoint` int(11) DEFAULT NULL,
'''   `TimeUnit` varchar(20) DEFAULT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `BioSourceWID` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_Experiment3` (`ContactWID`),
'''   KEY `FK_Experiment4` (`ArchiveWID`),
'''   KEY `FK_Experiment2` (`GroupWID`),
'''   KEY `FK_Experiment5` (`DataSetWID`),
'''   KEY `FK_Experiment6` (`BioSourceWID`),
'''   CONSTRAINT `FK_Experiment2` FOREIGN KEY (`GroupWID`) REFERENCES `experiment` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Experiment3` FOREIGN KEY (`ContactWID`) REFERENCES `contact` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Experiment4` FOREIGN KEY (`ArchiveWID`) REFERENCES `archive` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Experiment5` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Experiment6` FOREIGN KEY (`BioSourceWID`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("experiment", Database:="warehouse")>
Public Class experiment: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property WID As Long
    <DatabaseField("Type"), NotNull, DataType(MySqlDbType.VarChar, "50")> Public Property Type As String
    <DatabaseField("ContactWID"), DataType(MySqlDbType.Int64, "20")> Public Property ContactWID As Long
    <DatabaseField("ArchiveWID"), DataType(MySqlDbType.Int64, "20")> Public Property ArchiveWID As Long
    <DatabaseField("StartDate"), DataType(MySqlDbType.DateTime)> Public Property StartDate As Date
    <DatabaseField("EndDate"), DataType(MySqlDbType.DateTime)> Public Property EndDate As Date
    <DatabaseField("Description"), DataType(MySqlDbType.Text)> Public Property Description As String
    <DatabaseField("GroupWID"), DataType(MySqlDbType.Int64, "20")> Public Property GroupWID As Long
    <DatabaseField("GroupType"), DataType(MySqlDbType.VarChar, "50")> Public Property GroupType As String
    <DatabaseField("GroupSize"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property GroupSize As Long
    <DatabaseField("GroupIndex"), DataType(MySqlDbType.Int64, "11")> Public Property GroupIndex As Long
    <DatabaseField("TimePoint"), DataType(MySqlDbType.Int64, "11")> Public Property TimePoint As Long
    <DatabaseField("TimeUnit"), DataType(MySqlDbType.VarChar, "20")> Public Property TimeUnit As String
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DataSetWID As Long
    <DatabaseField("BioSourceWID"), DataType(MySqlDbType.Int64, "20")> Public Property BioSourceWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `experiment` (`WID`, `Type`, `ContactWID`, `ArchiveWID`, `StartDate`, `EndDate`, `Description`, `GroupWID`, `GroupType`, `GroupSize`, `GroupIndex`, `TimePoint`, `TimeUnit`, `DataSetWID`, `BioSourceWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `experiment` (`WID`, `Type`, `ContactWID`, `ArchiveWID`, `StartDate`, `EndDate`, `Description`, `GroupWID`, `GroupType`, `GroupSize`, `GroupIndex`, `TimePoint`, `TimeUnit`, `DataSetWID`, `BioSourceWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `experiment` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `experiment` SET `WID`='{0}', `Type`='{1}', `ContactWID`='{2}', `ArchiveWID`='{3}', `StartDate`='{4}', `EndDate`='{5}', `Description`='{6}', `GroupWID`='{7}', `GroupType`='{8}', `GroupSize`='{9}', `GroupIndex`='{10}', `TimePoint`='{11}', `TimeUnit`='{12}', `DataSetWID`='{13}', `BioSourceWID`='{14}' WHERE `WID` = '{15}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, Type, ContactWID, ArchiveWID, DataType.ToMySqlDateTimeString(StartDate), DataType.ToMySqlDateTimeString(EndDate), Description, GroupWID, GroupType, GroupSize, GroupIndex, TimePoint, TimeUnit, DataSetWID, BioSourceWID)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, Type, ContactWID, ArchiveWID, DataType.ToMySqlDateTimeString(StartDate), DataType.ToMySqlDateTimeString(EndDate), Description, GroupWID, GroupType, GroupSize, GroupIndex, TimePoint, TimeUnit, DataSetWID, BioSourceWID)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, Type, ContactWID, ArchiveWID, DataType.ToMySqlDateTimeString(StartDate), DataType.ToMySqlDateTimeString(EndDate), Description, GroupWID, GroupType, GroupSize, GroupIndex, TimePoint, TimeUnit, DataSetWID, BioSourceWID, WID)
    End Function
#End Region
End Class


End Namespace
