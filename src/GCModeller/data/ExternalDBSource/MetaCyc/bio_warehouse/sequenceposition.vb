#Region "Microsoft.VisualBasic::fbb6a5cdcce34e5a9eda5dc584326538, data\ExternalDBSource\MetaCyc\bio_warehouse\sequenceposition.vb"

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

    ' Class sequenceposition
    ' 
    '     Properties: [End], Composite, CompositeCompositeMap, DataSetWID, MAGEClass
    '                 Reporter, ReporterCompositeMap, Start_, WID
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
''' DROP TABLE IF EXISTS `sequenceposition`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `sequenceposition` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `MAGEClass` varchar(100) NOT NULL,
'''   `Start_` smallint(6) DEFAULT NULL,
'''   `End` smallint(6) DEFAULT NULL,
'''   `CompositeCompositeMap` bigint(20) DEFAULT NULL,
'''   `Composite` bigint(20) DEFAULT NULL,
'''   `ReporterCompositeMap` bigint(20) DEFAULT NULL,
'''   `Reporter` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_SequencePosition1` (`DataSetWID`),
'''   KEY `FK_SequencePosition2` (`CompositeCompositeMap`),
'''   KEY `FK_SequencePosition3` (`Composite`),
'''   KEY `FK_SequencePosition4` (`ReporterCompositeMap`),
'''   KEY `FK_SequencePosition5` (`Reporter`),
'''   CONSTRAINT `FK_SequencePosition1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_SequencePosition2` FOREIGN KEY (`CompositeCompositeMap`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_SequencePosition3` FOREIGN KEY (`Composite`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_SequencePosition4` FOREIGN KEY (`ReporterCompositeMap`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_SequencePosition5` FOREIGN KEY (`Reporter`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("sequenceposition", Database:="warehouse")>
Public Class sequenceposition: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DataSetWID As Long
    <DatabaseField("MAGEClass"), NotNull, DataType(MySqlDbType.VarChar, "100")> Public Property MAGEClass As String
    <DatabaseField("Start_"), DataType(MySqlDbType.Int64, "6")> Public Property Start_ As Long
    <DatabaseField("End"), DataType(MySqlDbType.Int64, "6")> Public Property [End] As Long
    <DatabaseField("CompositeCompositeMap"), DataType(MySqlDbType.Int64, "20")> Public Property CompositeCompositeMap As Long
    <DatabaseField("Composite"), DataType(MySqlDbType.Int64, "20")> Public Property Composite As Long
    <DatabaseField("ReporterCompositeMap"), DataType(MySqlDbType.Int64, "20")> Public Property ReporterCompositeMap As Long
    <DatabaseField("Reporter"), DataType(MySqlDbType.Int64, "20")> Public Property Reporter As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `sequenceposition` (`WID`, `DataSetWID`, `MAGEClass`, `Start_`, `End`, `CompositeCompositeMap`, `Composite`, `ReporterCompositeMap`, `Reporter`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `sequenceposition` (`WID`, `DataSetWID`, `MAGEClass`, `Start_`, `End`, `CompositeCompositeMap`, `Composite`, `ReporterCompositeMap`, `Reporter`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `sequenceposition` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `sequenceposition` SET `WID`='{0}', `DataSetWID`='{1}', `MAGEClass`='{2}', `Start_`='{3}', `End`='{4}', `CompositeCompositeMap`='{5}', `Composite`='{6}', `ReporterCompositeMap`='{7}', `Reporter`='{8}' WHERE `WID` = '{9}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, MAGEClass, Start_, [End], CompositeCompositeMap, Composite, ReporterCompositeMap, Reporter)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, MAGEClass, Start_, [End], CompositeCompositeMap, Composite, ReporterCompositeMap, Reporter)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, MAGEClass, Start_, [End], CompositeCompositeMap, Composite, ReporterCompositeMap, Reporter, WID)
    End Function
#End Region
End Class


End Namespace
