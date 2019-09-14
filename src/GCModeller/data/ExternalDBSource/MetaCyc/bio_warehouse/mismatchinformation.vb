#Region "Microsoft.VisualBasic::318e1c15a08a9d218e941d7bf3987ee5, ExternalDBSource\MetaCyc\bio_warehouse\mismatchinformation.vb"

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

    ' Class mismatchinformation
    ' 
    '     Properties: CompositePosition, DataSetWID, FeatureInformation, NewSequence, ReplacedLength
    '                 ReporterPosition, StartCoord, WID
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
''' DROP TABLE IF EXISTS `mismatchinformation`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `mismatchinformation` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `CompositePosition` bigint(20) DEFAULT NULL,
'''   `FeatureInformation` bigint(20) DEFAULT NULL,
'''   `StartCoord` smallint(6) DEFAULT NULL,
'''   `NewSequence` varchar(255) DEFAULT NULL,
'''   `ReplacedLength` smallint(6) DEFAULT NULL,
'''   `ReporterPosition` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_MismatchInformation1` (`DataSetWID`),
'''   KEY `FK_MismatchInformation2` (`CompositePosition`),
'''   KEY `FK_MismatchInformation3` (`FeatureInformation`),
'''   KEY `FK_MismatchInformation4` (`ReporterPosition`),
'''   CONSTRAINT `FK_MismatchInformation1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_MismatchInformation2` FOREIGN KEY (`CompositePosition`) REFERENCES `sequenceposition` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_MismatchInformation3` FOREIGN KEY (`FeatureInformation`) REFERENCES `featureinformation` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_MismatchInformation4` FOREIGN KEY (`ReporterPosition`) REFERENCES `sequenceposition` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("mismatchinformation", Database:="warehouse")>
Public Class mismatchinformation: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DataSetWID As Long
    <DatabaseField("CompositePosition"), DataType(MySqlDbType.Int64, "20")> Public Property CompositePosition As Long
    <DatabaseField("FeatureInformation"), DataType(MySqlDbType.Int64, "20")> Public Property FeatureInformation As Long
    <DatabaseField("StartCoord"), DataType(MySqlDbType.Int64, "6")> Public Property StartCoord As Long
    <DatabaseField("NewSequence"), DataType(MySqlDbType.VarChar, "255")> Public Property NewSequence As String
    <DatabaseField("ReplacedLength"), DataType(MySqlDbType.Int64, "6")> Public Property ReplacedLength As Long
    <DatabaseField("ReporterPosition"), DataType(MySqlDbType.Int64, "20")> Public Property ReporterPosition As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `mismatchinformation` (`WID`, `DataSetWID`, `CompositePosition`, `FeatureInformation`, `StartCoord`, `NewSequence`, `ReplacedLength`, `ReporterPosition`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `mismatchinformation` (`WID`, `DataSetWID`, `CompositePosition`, `FeatureInformation`, `StartCoord`, `NewSequence`, `ReplacedLength`, `ReporterPosition`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `mismatchinformation` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `mismatchinformation` SET `WID`='{0}', `DataSetWID`='{1}', `CompositePosition`='{2}', `FeatureInformation`='{3}', `StartCoord`='{4}', `NewSequence`='{5}', `ReplacedLength`='{6}', `ReporterPosition`='{7}' WHERE `WID` = '{8}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, CompositePosition, FeatureInformation, StartCoord, NewSequence, ReplacedLength, ReporterPosition)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, CompositePosition, FeatureInformation, StartCoord, NewSequence, ReplacedLength, ReporterPosition)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, CompositePosition, FeatureInformation, StartCoord, NewSequence, ReplacedLength, ReporterPosition, WID)
    End Function
#End Region
End Class


End Namespace
