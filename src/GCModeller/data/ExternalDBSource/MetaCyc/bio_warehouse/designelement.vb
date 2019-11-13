#Region "Microsoft.VisualBasic::0f90fe205234471cbd8b82d8dde40cec, data\ExternalDBSource\MetaCyc\bio_warehouse\designelement.vb"

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

    ' Class designelement
    ' 
    '     Properties: DataSetWID, DesignElement_ControlType, Feature_FeatureLocation, Feature_Position, FeatureGroup
    '                 FeatureGroup_Features, Identifier, MAGEClass, Name, Reporter_WarningType
    '                 WID, Zone
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
''' DROP TABLE IF EXISTS `designelement`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `designelement` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `MAGEClass` varchar(100) NOT NULL,
'''   `Identifier` varchar(255) DEFAULT NULL,
'''   `Name` varchar(255) DEFAULT NULL,
'''   `FeatureGroup_Features` bigint(20) DEFAULT NULL,
'''   `DesignElement_ControlType` bigint(20) DEFAULT NULL,
'''   `Feature_Position` bigint(20) DEFAULT NULL,
'''   `Zone` bigint(20) DEFAULT NULL,
'''   `Feature_FeatureLocation` bigint(20) DEFAULT NULL,
'''   `FeatureGroup` bigint(20) DEFAULT NULL,
'''   `Reporter_WarningType` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_DesignElement1` (`DataSetWID`),
'''   KEY `FK_DesignElement3` (`FeatureGroup_Features`),
'''   KEY `FK_DesignElement4` (`DesignElement_ControlType`),
'''   KEY `FK_DesignElement5` (`Feature_Position`),
'''   KEY `FK_DesignElement6` (`Zone`),
'''   KEY `FK_DesignElement7` (`Feature_FeatureLocation`),
'''   KEY `FK_DesignElement8` (`FeatureGroup`),
'''   KEY `FK_DesignElement9` (`Reporter_WarningType`),
'''   CONSTRAINT `FK_DesignElement1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_DesignElement3` FOREIGN KEY (`FeatureGroup_Features`) REFERENCES `designelementgroup` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_DesignElement4` FOREIGN KEY (`DesignElement_ControlType`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_DesignElement5` FOREIGN KEY (`Feature_Position`) REFERENCES `position_` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_DesignElement6` FOREIGN KEY (`Zone`) REFERENCES `zone` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_DesignElement7` FOREIGN KEY (`Feature_FeatureLocation`) REFERENCES `featurelocation` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_DesignElement8` FOREIGN KEY (`FeatureGroup`) REFERENCES `designelementgroup` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_DesignElement9` FOREIGN KEY (`Reporter_WarningType`) REFERENCES `term` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("designelement", Database:="warehouse")>
Public Class designelement: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DataSetWID As Long
    <DatabaseField("MAGEClass"), NotNull, DataType(MySqlDbType.VarChar, "100")> Public Property MAGEClass As String
    <DatabaseField("Identifier"), DataType(MySqlDbType.VarChar, "255")> Public Property Identifier As String
    <DatabaseField("Name"), DataType(MySqlDbType.VarChar, "255")> Public Property Name As String
    <DatabaseField("FeatureGroup_Features"), DataType(MySqlDbType.Int64, "20")> Public Property FeatureGroup_Features As Long
    <DatabaseField("DesignElement_ControlType"), DataType(MySqlDbType.Int64, "20")> Public Property DesignElement_ControlType As Long
    <DatabaseField("Feature_Position"), DataType(MySqlDbType.Int64, "20")> Public Property Feature_Position As Long
    <DatabaseField("Zone"), DataType(MySqlDbType.Int64, "20")> Public Property Zone As Long
    <DatabaseField("Feature_FeatureLocation"), DataType(MySqlDbType.Int64, "20")> Public Property Feature_FeatureLocation As Long
    <DatabaseField("FeatureGroup"), DataType(MySqlDbType.Int64, "20")> Public Property FeatureGroup As Long
    <DatabaseField("Reporter_WarningType"), DataType(MySqlDbType.Int64, "20")> Public Property Reporter_WarningType As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `designelement` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `FeatureGroup_Features`, `DesignElement_ControlType`, `Feature_Position`, `Zone`, `Feature_FeatureLocation`, `FeatureGroup`, `Reporter_WarningType`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `designelement` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `FeatureGroup_Features`, `DesignElement_ControlType`, `Feature_Position`, `Zone`, `Feature_FeatureLocation`, `FeatureGroup`, `Reporter_WarningType`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `designelement` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `designelement` SET `WID`='{0}', `DataSetWID`='{1}', `MAGEClass`='{2}', `Identifier`='{3}', `Name`='{4}', `FeatureGroup_Features`='{5}', `DesignElement_ControlType`='{6}', `Feature_Position`='{7}', `Zone`='{8}', `Feature_FeatureLocation`='{9}', `FeatureGroup`='{10}', `Reporter_WarningType`='{11}' WHERE `WID` = '{12}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, FeatureGroup_Features, DesignElement_ControlType, Feature_Position, Zone, Feature_FeatureLocation, FeatureGroup, Reporter_WarningType)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, FeatureGroup_Features, DesignElement_ControlType, Feature_Position, Zone, Feature_FeatureLocation, FeatureGroup, Reporter_WarningType)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, FeatureGroup_Features, DesignElement_ControlType, Feature_Position, Zone, Feature_FeatureLocation, FeatureGroup, Reporter_WarningType, WID)
    End Function
#End Region
End Class


End Namespace
