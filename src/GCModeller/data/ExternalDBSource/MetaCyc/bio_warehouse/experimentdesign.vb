﻿#Region "Microsoft.VisualBasic::13d03524a7dcf18b5706cbcdb2d8d7db, data\ExternalDBSource\MetaCyc\bio_warehouse\experimentdesign.vb"

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

    ' Class experimentdesign
    ' 
    '     Properties: DataSetWID, Experiment_ExperimentDesigns, NormalizationDescription, QualityControlDescription, ReplicateDescription
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
''' DROP TABLE IF EXISTS `experimentdesign`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `experimentdesign` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `Experiment_ExperimentDesigns` bigint(20) DEFAULT NULL,
'''   `QualityControlDescription` bigint(20) DEFAULT NULL,
'''   `NormalizationDescription` bigint(20) DEFAULT NULL,
'''   `ReplicateDescription` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_ExperimentDesign1` (`DataSetWID`),
'''   KEY `FK_ExperimentDesign3` (`Experiment_ExperimentDesigns`),
'''   CONSTRAINT `FK_ExperimentDesign1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_ExperimentDesign3` FOREIGN KEY (`Experiment_ExperimentDesigns`) REFERENCES `experiment` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("experimentdesign", Database:="warehouse")>
Public Class experimentdesign: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DataSetWID As Long
    <DatabaseField("Experiment_ExperimentDesigns"), DataType(MySqlDbType.Int64, "20")> Public Property Experiment_ExperimentDesigns As Long
    <DatabaseField("QualityControlDescription"), DataType(MySqlDbType.Int64, "20")> Public Property QualityControlDescription As Long
    <DatabaseField("NormalizationDescription"), DataType(MySqlDbType.Int64, "20")> Public Property NormalizationDescription As Long
    <DatabaseField("ReplicateDescription"), DataType(MySqlDbType.Int64, "20")> Public Property ReplicateDescription As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `experimentdesign` (`WID`, `DataSetWID`, `Experiment_ExperimentDesigns`, `QualityControlDescription`, `NormalizationDescription`, `ReplicateDescription`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `experimentdesign` (`WID`, `DataSetWID`, `Experiment_ExperimentDesigns`, `QualityControlDescription`, `NormalizationDescription`, `ReplicateDescription`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `experimentdesign` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `experimentdesign` SET `WID`='{0}', `DataSetWID`='{1}', `Experiment_ExperimentDesigns`='{2}', `QualityControlDescription`='{3}', `NormalizationDescription`='{4}', `ReplicateDescription`='{5}' WHERE `WID` = '{6}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, Experiment_ExperimentDesigns, QualityControlDescription, NormalizationDescription, ReplicateDescription)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, Experiment_ExperimentDesigns, QualityControlDescription, NormalizationDescription, ReplicateDescription)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, Experiment_ExperimentDesigns, QualityControlDescription, NormalizationDescription, ReplicateDescription, WID)
    End Function
#End Region
End Class


End Namespace
