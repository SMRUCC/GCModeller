#Region "Microsoft.VisualBasic::38fc1f7192d5cc7e95b11d62e8581f4d, data\ExternalDBSource\MetaCyc\MySQL\spotidmethod.vb"

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

    ' Class spotidmethod
    ' 
    '     Properties: DatasetWID, MethodAbbrev, MethodDesc, MethodName, WID
    ' 
    '     Function: Clone, GetDeleteSQL, GetDumpInsertValue, (+2 Overloads) GetInsertSQL, (+2 Overloads) GetReplaceSQL
    '               GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @2018/5/23 13:13:40


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace MetaCyc.MySQL

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `spotidmethod`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `spotidmethod` (
'''   `WID` bigint(20) NOT NULL,
'''   `MethodName` varchar(100) NOT NULL,
'''   `MethodDesc` varchar(500) DEFAULT NULL,
'''   `MethodAbbrev` varchar(10) DEFAULT NULL,
'''   `DatasetWID` bigint(20) NOT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_SpotIdMethDataset` (`DatasetWID`),
'''   CONSTRAINT `FK_SpotIdMethDataset` FOREIGN KEY (`DatasetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("spotidmethod", Database:="warehouse", SchemaSQL:="
CREATE TABLE `spotidmethod` (
  `WID` bigint(20) NOT NULL,
  `MethodName` varchar(100) NOT NULL,
  `MethodDesc` varchar(500) DEFAULT NULL,
  `MethodAbbrev` varchar(10) DEFAULT NULL,
  `DatasetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_SpotIdMethDataset` (`DatasetWID`),
  CONSTRAINT `FK_SpotIdMethDataset` FOREIGN KEY (`DatasetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class spotidmethod: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("MethodName"), NotNull, DataType(MySqlDbType.VarChar, "100"), Column(Name:="MethodName")> Public Property MethodName As String
    <DatabaseField("MethodDesc"), DataType(MySqlDbType.VarChar, "500"), Column(Name:="MethodDesc")> Public Property MethodDesc As String
    <DatabaseField("MethodAbbrev"), DataType(MySqlDbType.VarChar, "10"), Column(Name:="MethodAbbrev")> Public Property MethodAbbrev As String
    <DatabaseField("DatasetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DatasetWID")> Public Property DatasetWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `spotidmethod` (`WID`, `MethodName`, `MethodDesc`, `MethodAbbrev`, `DatasetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `spotidmethod` (`WID`, `MethodName`, `MethodDesc`, `MethodAbbrev`, `DatasetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `spotidmethod` (`WID`, `MethodName`, `MethodDesc`, `MethodAbbrev`, `DatasetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `spotidmethod` (`WID`, `MethodName`, `MethodDesc`, `MethodAbbrev`, `DatasetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `spotidmethod` WHERE `WID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `spotidmethod` SET `WID`='{0}', `MethodName`='{1}', `MethodDesc`='{2}', `MethodAbbrev`='{3}', `DatasetWID`='{4}' WHERE `WID` = '{5}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `spotidmethod` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `spotidmethod` (`WID`, `MethodName`, `MethodDesc`, `MethodAbbrev`, `DatasetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, MethodName, MethodDesc, MethodAbbrev, DatasetWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `spotidmethod` (`WID`, `MethodName`, `MethodDesc`, `MethodAbbrev`, `DatasetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, WID, MethodName, MethodDesc, MethodAbbrev, DatasetWID)
        Else
        Return String.Format(INSERT_SQL, WID, MethodName, MethodDesc, MethodAbbrev, DatasetWID)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{WID}', '{MethodName}', '{MethodDesc}', '{MethodAbbrev}', '{DatasetWID}')"
        Else
            Return $"('{WID}', '{MethodName}', '{MethodDesc}', '{MethodAbbrev}', '{DatasetWID}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `spotidmethod` (`WID`, `MethodName`, `MethodDesc`, `MethodAbbrev`, `DatasetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, MethodName, MethodDesc, MethodAbbrev, DatasetWID)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `spotidmethod` (`WID`, `MethodName`, `MethodDesc`, `MethodAbbrev`, `DatasetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, WID, MethodName, MethodDesc, MethodAbbrev, DatasetWID)
        Else
        Return String.Format(REPLACE_SQL, WID, MethodName, MethodDesc, MethodAbbrev, DatasetWID)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `spotidmethod` SET `WID`='{0}', `MethodName`='{1}', `MethodDesc`='{2}', `MethodAbbrev`='{3}', `DatasetWID`='{4}' WHERE `WID` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, MethodName, MethodDesc, MethodAbbrev, DatasetWID, WID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As spotidmethod
                         Return DirectCast(MyClass.MemberwiseClone, spotidmethod)
                     End Function
End Class


End Namespace
