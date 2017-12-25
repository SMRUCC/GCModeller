#Region "Microsoft.VisualBasic::ad944d0b6b67982744c191652de8f205, ..\GCModeller\data\ExternalDBSource\MetaCyc\MySQL\measbassaywidmeasbassaydatawid.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
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

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @3/29/2017 8:48:56 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MetaCyc.MySQL

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `measbassaywidmeasbassaydatawid`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `measbassaywidmeasbassaydatawid` (
'''   `MeasuredBioAssayWID` bigint(20) NOT NULL,
'''   `MeasuredBioAssayDataWID` bigint(20) NOT NULL,
'''   KEY `FK_MeasBAssayWIDMeasBAssayDa1` (`MeasuredBioAssayWID`),
'''   KEY `FK_MeasBAssayWIDMeasBAssayDa2` (`MeasuredBioAssayDataWID`),
'''   CONSTRAINT `FK_MeasBAssayWIDMeasBAssayDa1` FOREIGN KEY (`MeasuredBioAssayWID`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_MeasBAssayWIDMeasBAssayDa2` FOREIGN KEY (`MeasuredBioAssayDataWID`) REFERENCES `bioassaydata` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("measbassaywidmeasbassaydatawid", Database:="warehouse", SchemaSQL:="
CREATE TABLE `measbassaywidmeasbassaydatawid` (
  `MeasuredBioAssayWID` bigint(20) NOT NULL,
  `MeasuredBioAssayDataWID` bigint(20) NOT NULL,
  KEY `FK_MeasBAssayWIDMeasBAssayDa1` (`MeasuredBioAssayWID`),
  KEY `FK_MeasBAssayWIDMeasBAssayDa2` (`MeasuredBioAssayDataWID`),
  CONSTRAINT `FK_MeasBAssayWIDMeasBAssayDa1` FOREIGN KEY (`MeasuredBioAssayWID`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_MeasBAssayWIDMeasBAssayDa2` FOREIGN KEY (`MeasuredBioAssayDataWID`) REFERENCES `bioassaydata` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class measbassaywidmeasbassaydatawid: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("MeasuredBioAssayWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property MeasuredBioAssayWID As Long
    <DatabaseField("MeasuredBioAssayDataWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property MeasuredBioAssayDataWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `measbassaywidmeasbassaydatawid` (`MeasuredBioAssayWID`, `MeasuredBioAssayDataWID`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `measbassaywidmeasbassaydatawid` (`MeasuredBioAssayWID`, `MeasuredBioAssayDataWID`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `measbassaywidmeasbassaydatawid` WHERE `MeasuredBioAssayWID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `measbassaywidmeasbassaydatawid` SET `MeasuredBioAssayWID`='{0}', `MeasuredBioAssayDataWID`='{1}' WHERE `MeasuredBioAssayWID` = '{2}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `measbassaywidmeasbassaydatawid` WHERE `MeasuredBioAssayWID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, MeasuredBioAssayWID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `measbassaywidmeasbassaydatawid` (`MeasuredBioAssayWID`, `MeasuredBioAssayDataWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, MeasuredBioAssayWID, MeasuredBioAssayDataWID)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{MeasuredBioAssayWID}', '{MeasuredBioAssayDataWID}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `measbassaywidmeasbassaydatawid` (`MeasuredBioAssayWID`, `MeasuredBioAssayDataWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, MeasuredBioAssayWID, MeasuredBioAssayDataWID)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `measbassaywidmeasbassaydatawid` SET `MeasuredBioAssayWID`='{0}', `MeasuredBioAssayDataWID`='{1}' WHERE `MeasuredBioAssayWID` = '{2}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, MeasuredBioAssayWID, MeasuredBioAssayDataWID, MeasuredBioAssayWID)
    End Function
#End Region
End Class


End Namespace
