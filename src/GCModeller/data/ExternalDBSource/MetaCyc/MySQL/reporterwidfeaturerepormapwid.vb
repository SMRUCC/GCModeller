#Region "Microsoft.VisualBasic::1a24f4bf65ba6dd851337c4515ac5689, data\ExternalDBSource\MetaCyc\MySQL\reporterwidfeaturerepormapwid.vb"

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

    ' Class reporterwidfeaturerepormapwid
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

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
''' DROP TABLE IF EXISTS `reporterwidfeaturerepormapwid`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `reporterwidfeaturerepormapwid` (
'''   `ReporterWID` bigint(20) NOT NULL,
'''   `FeatureReporterMapWID` bigint(20) NOT NULL,
'''   KEY `FK_ReporterWIDFeatureReporMa1` (`ReporterWID`),
'''   KEY `FK_ReporterWIDFeatureReporMa2` (`FeatureReporterMapWID`),
'''   CONSTRAINT `FK_ReporterWIDFeatureReporMa1` FOREIGN KEY (`ReporterWID`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_ReporterWIDFeatureReporMa2` FOREIGN KEY (`FeatureReporterMapWID`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("reporterwidfeaturerepormapwid", Database:="warehouse", SchemaSQL:="
CREATE TABLE `reporterwidfeaturerepormapwid` (
  `ReporterWID` bigint(20) NOT NULL,
  `FeatureReporterMapWID` bigint(20) NOT NULL,
  KEY `FK_ReporterWIDFeatureReporMa1` (`ReporterWID`),
  KEY `FK_ReporterWIDFeatureReporMa2` (`FeatureReporterMapWID`),
  CONSTRAINT `FK_ReporterWIDFeatureReporMa1` FOREIGN KEY (`ReporterWID`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ReporterWIDFeatureReporMa2` FOREIGN KEY (`FeatureReporterMapWID`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class reporterwidfeaturerepormapwid: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("ReporterWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property ReporterWID As Long
    <DatabaseField("FeatureReporterMapWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property FeatureReporterMapWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `reporterwidfeaturerepormapwid` (`ReporterWID`, `FeatureReporterMapWID`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `reporterwidfeaturerepormapwid` (`ReporterWID`, `FeatureReporterMapWID`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `reporterwidfeaturerepormapwid` WHERE `ReporterWID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `reporterwidfeaturerepormapwid` SET `ReporterWID`='{0}', `FeatureReporterMapWID`='{1}' WHERE `ReporterWID` = '{2}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `reporterwidfeaturerepormapwid` WHERE `ReporterWID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, ReporterWID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `reporterwidfeaturerepormapwid` (`ReporterWID`, `FeatureReporterMapWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, ReporterWID, FeatureReporterMapWID)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{ReporterWID}', '{FeatureReporterMapWID}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `reporterwidfeaturerepormapwid` (`ReporterWID`, `FeatureReporterMapWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, ReporterWID, FeatureReporterMapWID)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `reporterwidfeaturerepormapwid` SET `ReporterWID`='{0}', `FeatureReporterMapWID`='{1}' WHERE `ReporterWID` = '{2}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, ReporterWID, FeatureReporterMapWID, ReporterWID)
    End Function
#End Region
End Class


End Namespace
