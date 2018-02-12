#Region "Microsoft.VisualBasic::b3f6401cef622d56d5e24ce032982e9d, data\ExternalDBSource\MetaCyc\MySQL\pathway.vb"

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

    ' Class pathway
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
''' DROP TABLE IF EXISTS `pathway`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `pathway` (
'''   `WID` bigint(20) NOT NULL,
'''   `Name` varchar(255) NOT NULL,
'''   `Type` char(1) NOT NULL,
'''   `BioSourceWID` bigint(20) DEFAULT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `PATHWAY_BSWID_WID_DWID` (`BioSourceWID`,`WID`,`DataSetWID`),
'''   KEY `PATHWAY_TYPE_WID_DWID` (`Type`,`WID`,`DataSetWID`),
'''   KEY `PATHWAY_DWID` (`DataSetWID`),
'''   CONSTRAINT `FK_Pathway1` FOREIGN KEY (`BioSourceWID`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Pathway2` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("pathway", Database:="warehouse", SchemaSQL:="
CREATE TABLE `pathway` (
  `WID` bigint(20) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Type` char(1) NOT NULL,
  `BioSourceWID` bigint(20) DEFAULT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `PATHWAY_BSWID_WID_DWID` (`BioSourceWID`,`WID`,`DataSetWID`),
  KEY `PATHWAY_TYPE_WID_DWID` (`Type`,`WID`,`DataSetWID`),
  KEY `PATHWAY_DWID` (`DataSetWID`),
  CONSTRAINT `FK_Pathway1` FOREIGN KEY (`BioSourceWID`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Pathway2` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class pathway: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property WID As Long
    <DatabaseField("Name"), NotNull, DataType(MySqlDbType.VarChar, "255")> Public Property Name As String
    <DatabaseField("Type"), NotNull, DataType(MySqlDbType.VarChar, "1")> Public Property Type As String
    <DatabaseField("BioSourceWID"), DataType(MySqlDbType.Int64, "20")> Public Property BioSourceWID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DataSetWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `pathway` (`WID`, `Name`, `Type`, `BioSourceWID`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `pathway` (`WID`, `Name`, `Type`, `BioSourceWID`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `pathway` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `pathway` SET `WID`='{0}', `Name`='{1}', `Type`='{2}', `BioSourceWID`='{3}', `DataSetWID`='{4}' WHERE `WID` = '{5}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `pathway` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `pathway` (`WID`, `Name`, `Type`, `BioSourceWID`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, Name, Type, BioSourceWID, DataSetWID)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{WID}', '{Name}', '{Type}', '{BioSourceWID}', '{DataSetWID}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `pathway` (`WID`, `Name`, `Type`, `BioSourceWID`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, Name, Type, BioSourceWID, DataSetWID)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `pathway` SET `WID`='{0}', `Name`='{1}', `Type`='{2}', `BioSourceWID`='{3}', `DataSetWID`='{4}' WHERE `WID` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, Name, Type, BioSourceWID, DataSetWID, WID)
    End Function
#End Region
End Class


End Namespace
