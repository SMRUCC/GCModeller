#Region "Microsoft.VisualBasic::97119483e29e22cca5f17f2999556762, data\ExternalDBSource\MetaCyc\MySQL\designelementtuple.vb"

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

    ' Class designelementtuple
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
''' DROP TABLE IF EXISTS `designelementtuple`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `designelementtuple` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `BioAssayTuple` bigint(20) DEFAULT NULL,
'''   `DesignElement` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_DesignElementTuple1` (`DataSetWID`),
'''   KEY `FK_DesignElementTuple2` (`BioAssayTuple`),
'''   KEY `FK_DesignElementTuple3` (`DesignElement`),
'''   CONSTRAINT `FK_DesignElementTuple1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_DesignElementTuple2` FOREIGN KEY (`BioAssayTuple`) REFERENCES `bioassaytuple` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_DesignElementTuple3` FOREIGN KEY (`DesignElement`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("designelementtuple", Database:="warehouse", SchemaSQL:="
CREATE TABLE `designelementtuple` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `BioAssayTuple` bigint(20) DEFAULT NULL,
  `DesignElement` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_DesignElementTuple1` (`DataSetWID`),
  KEY `FK_DesignElementTuple2` (`BioAssayTuple`),
  KEY `FK_DesignElementTuple3` (`DesignElement`),
  CONSTRAINT `FK_DesignElementTuple1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_DesignElementTuple2` FOREIGN KEY (`BioAssayTuple`) REFERENCES `bioassaytuple` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_DesignElementTuple3` FOREIGN KEY (`DesignElement`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class designelementtuple: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DataSetWID As Long
    <DatabaseField("BioAssayTuple"), DataType(MySqlDbType.Int64, "20")> Public Property BioAssayTuple As Long
    <DatabaseField("DesignElement"), DataType(MySqlDbType.Int64, "20")> Public Property DesignElement As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `designelementtuple` (`WID`, `DataSetWID`, `BioAssayTuple`, `DesignElement`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `designelementtuple` (`WID`, `DataSetWID`, `BioAssayTuple`, `DesignElement`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `designelementtuple` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `designelementtuple` SET `WID`='{0}', `DataSetWID`='{1}', `BioAssayTuple`='{2}', `DesignElement`='{3}' WHERE `WID` = '{4}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `designelementtuple` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `designelementtuple` (`WID`, `DataSetWID`, `BioAssayTuple`, `DesignElement`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, BioAssayTuple, DesignElement)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{WID}', '{DataSetWID}', '{BioAssayTuple}', '{DesignElement}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `designelementtuple` (`WID`, `DataSetWID`, `BioAssayTuple`, `DesignElement`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, BioAssayTuple, DesignElement)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `designelementtuple` SET `WID`='{0}', `DataSetWID`='{1}', `BioAssayTuple`='{2}', `DesignElement`='{3}' WHERE `WID` = '{4}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, BioAssayTuple, DesignElement, WID)
    End Function
#End Region
End Class


End Namespace
