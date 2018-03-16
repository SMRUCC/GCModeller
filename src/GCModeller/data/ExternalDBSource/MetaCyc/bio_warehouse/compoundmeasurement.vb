#Region "Microsoft.VisualBasic::948c3c8035c7d1990f405daa973f0ecd, data\ExternalDBSource\MetaCyc\bio_warehouse\compoundmeasurement.vb"

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

    ' Class compoundmeasurement
    ' 
    '     Properties: Compound, Compound_ComponentCompounds, DataSetWID, Measurement, Treatment_CompoundMeasurements
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
''' DROP TABLE IF EXISTS `compoundmeasurement`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `compoundmeasurement` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `Compound_ComponentCompounds` bigint(20) DEFAULT NULL,
'''   `Compound` bigint(20) DEFAULT NULL,
'''   `Measurement` bigint(20) DEFAULT NULL,
'''   `Treatment_CompoundMeasurements` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_CompoundMeasurement1` (`DataSetWID`),
'''   KEY `FK_CompoundMeasurement2` (`Compound_ComponentCompounds`),
'''   KEY `FK_CompoundMeasurement3` (`Compound`),
'''   KEY `FK_CompoundMeasurement4` (`Measurement`),
'''   KEY `FK_CompoundMeasurement5` (`Treatment_CompoundMeasurements`),
'''   CONSTRAINT `FK_CompoundMeasurement1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_CompoundMeasurement2` FOREIGN KEY (`Compound_ComponentCompounds`) REFERENCES `chemical` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_CompoundMeasurement3` FOREIGN KEY (`Compound`) REFERENCES `chemical` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_CompoundMeasurement4` FOREIGN KEY (`Measurement`) REFERENCES `measurement` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_CompoundMeasurement5` FOREIGN KEY (`Treatment_CompoundMeasurements`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("compoundmeasurement", Database:="warehouse")>
Public Class compoundmeasurement: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DataSetWID As Long
    <DatabaseField("Compound_ComponentCompounds"), DataType(MySqlDbType.Int64, "20")> Public Property Compound_ComponentCompounds As Long
    <DatabaseField("Compound"), DataType(MySqlDbType.Int64, "20")> Public Property Compound As Long
    <DatabaseField("Measurement"), DataType(MySqlDbType.Int64, "20")> Public Property Measurement As Long
    <DatabaseField("Treatment_CompoundMeasurements"), DataType(MySqlDbType.Int64, "20")> Public Property Treatment_CompoundMeasurements As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `compoundmeasurement` (`WID`, `DataSetWID`, `Compound_ComponentCompounds`, `Compound`, `Measurement`, `Treatment_CompoundMeasurements`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `compoundmeasurement` (`WID`, `DataSetWID`, `Compound_ComponentCompounds`, `Compound`, `Measurement`, `Treatment_CompoundMeasurements`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `compoundmeasurement` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `compoundmeasurement` SET `WID`='{0}', `DataSetWID`='{1}', `Compound_ComponentCompounds`='{2}', `Compound`='{3}', `Measurement`='{4}', `Treatment_CompoundMeasurements`='{5}' WHERE `WID` = '{6}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, Compound_ComponentCompounds, Compound, Measurement, Treatment_CompoundMeasurements)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, Compound_ComponentCompounds, Compound, Measurement, Treatment_CompoundMeasurements)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, Compound_ComponentCompounds, Compound, Measurement, Treatment_CompoundMeasurements, WID)
    End Function
#End Region
End Class


End Namespace
