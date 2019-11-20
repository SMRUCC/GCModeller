#Region "Microsoft.VisualBasic::a0f9944c326e214ee9cd51876f3e8ed4, data\ExternalDBSource\MetaCyc\MySQL\reportergroupwidreporterwid.vb"

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

    ' Class reportergroupwidreporterwid
    ' 
    '     Properties: ReporterGroupWID, ReporterWID
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
''' DROP TABLE IF EXISTS `reportergroupwidreporterwid`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `reportergroupwidreporterwid` (
'''   `ReporterGroupWID` bigint(20) NOT NULL,
'''   `ReporterWID` bigint(20) NOT NULL,
'''   KEY `FK_ReporterGroupWIDReporterW1` (`ReporterGroupWID`),
'''   KEY `FK_ReporterGroupWIDReporterW2` (`ReporterWID`),
'''   CONSTRAINT `FK_ReporterGroupWIDReporterW1` FOREIGN KEY (`ReporterGroupWID`) REFERENCES `designelementgroup` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_ReporterGroupWIDReporterW2` FOREIGN KEY (`ReporterWID`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("reportergroupwidreporterwid", Database:="warehouse", SchemaSQL:="
CREATE TABLE `reportergroupwidreporterwid` (
  `ReporterGroupWID` bigint(20) NOT NULL,
  `ReporterWID` bigint(20) NOT NULL,
  KEY `FK_ReporterGroupWIDReporterW1` (`ReporterGroupWID`),
  KEY `FK_ReporterGroupWIDReporterW2` (`ReporterWID`),
  CONSTRAINT `FK_ReporterGroupWIDReporterW1` FOREIGN KEY (`ReporterGroupWID`) REFERENCES `designelementgroup` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ReporterGroupWIDReporterW2` FOREIGN KEY (`ReporterWID`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class reportergroupwidreporterwid: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("ReporterGroupWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="ReporterGroupWID"), XmlAttribute> Public Property ReporterGroupWID As Long
    <DatabaseField("ReporterWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="ReporterWID")> Public Property ReporterWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `reportergroupwidreporterwid` (`ReporterGroupWID`, `ReporterWID`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `reportergroupwidreporterwid` (`ReporterGroupWID`, `ReporterWID`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `reportergroupwidreporterwid` (`ReporterGroupWID`, `ReporterWID`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `reportergroupwidreporterwid` (`ReporterGroupWID`, `ReporterWID`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `reportergroupwidreporterwid` WHERE `ReporterGroupWID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `reportergroupwidreporterwid` SET `ReporterGroupWID`='{0}', `ReporterWID`='{1}' WHERE `ReporterGroupWID` = '{2}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `reportergroupwidreporterwid` WHERE `ReporterGroupWID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, ReporterGroupWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `reportergroupwidreporterwid` (`ReporterGroupWID`, `ReporterWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, ReporterGroupWID, ReporterWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `reportergroupwidreporterwid` (`ReporterGroupWID`, `ReporterWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, ReporterGroupWID, ReporterWID)
        Else
        Return String.Format(INSERT_SQL, ReporterGroupWID, ReporterWID)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{ReporterGroupWID}', '{ReporterWID}')"
        Else
            Return $"('{ReporterGroupWID}', '{ReporterWID}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `reportergroupwidreporterwid` (`ReporterGroupWID`, `ReporterWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, ReporterGroupWID, ReporterWID)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `reportergroupwidreporterwid` (`ReporterGroupWID`, `ReporterWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, ReporterGroupWID, ReporterWID)
        Else
        Return String.Format(REPLACE_SQL, ReporterGroupWID, ReporterWID)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `reportergroupwidreporterwid` SET `ReporterGroupWID`='{0}', `ReporterWID`='{1}' WHERE `ReporterGroupWID` = '{2}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, ReporterGroupWID, ReporterWID, ReporterGroupWID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As reportergroupwidreporterwid
                         Return DirectCast(MyClass.MemberwiseClone, reportergroupwidreporterwid)
                     End Function
End Class


End Namespace
