#Region "Microsoft.VisualBasic::1059cca261fd221306984611c4879d15, data\ExternalDBSource\MetaCyc\MySQL\archive.vb"

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

    ' Class archive
    ' 
    '     Properties: Contents, DataSetWID, Format, OtherWID, ToolName
    '                 URL, WID
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
''' DROP TABLE IF EXISTS `archive`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `archive` (
'''   `WID` bigint(20) NOT NULL,
'''   `OtherWID` bigint(20) NOT NULL,
'''   `Format` varchar(10) NOT NULL,
'''   `Contents` longblob,
'''   `URL` text,
'''   `ToolName` varchar(50) DEFAULT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   PRIMARY KEY (`WID`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("archive", Database:="warehouse", SchemaSQL:="
CREATE TABLE `archive` (
  `WID` bigint(20) NOT NULL,
  `OtherWID` bigint(20) NOT NULL,
  `Format` varchar(10) NOT NULL,
  `Contents` longblob,
  `URL` text,
  `ToolName` varchar(50) DEFAULT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class archive: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("OtherWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="OtherWID")> Public Property OtherWID As Long
    <DatabaseField("Format"), NotNull, DataType(MySqlDbType.VarChar, "10"), Column(Name:="Format")> Public Property Format As String
    <DatabaseField("Contents"), DataType(MySqlDbType.Blob), Column(Name:="Contents")> Public Property Contents As Byte()
    <DatabaseField("URL"), DataType(MySqlDbType.Text), Column(Name:="URL")> Public Property URL As String
    <DatabaseField("ToolName"), DataType(MySqlDbType.VarChar, "50"), Column(Name:="ToolName")> Public Property ToolName As String
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DataSetWID")> Public Property DataSetWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `archive` (`WID`, `OtherWID`, `Format`, `Contents`, `URL`, `ToolName`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `archive` (`WID`, `OtherWID`, `Format`, `Contents`, `URL`, `ToolName`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `archive` (`WID`, `OtherWID`, `Format`, `Contents`, `URL`, `ToolName`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `archive` (`WID`, `OtherWID`, `Format`, `Contents`, `URL`, `ToolName`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `archive` WHERE `WID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `archive` SET `WID`='{0}', `OtherWID`='{1}', `Format`='{2}', `Contents`='{3}', `URL`='{4}', `ToolName`='{5}', `DataSetWID`='{6}' WHERE `WID` = '{7}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `archive` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `archive` (`WID`, `OtherWID`, `Format`, `Contents`, `URL`, `ToolName`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, OtherWID, Format, Contents, URL, ToolName, DataSetWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `archive` (`WID`, `OtherWID`, `Format`, `Contents`, `URL`, `ToolName`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, WID, OtherWID, Format, Contents, URL, ToolName, DataSetWID)
        Else
        Return String.Format(INSERT_SQL, WID, OtherWID, Format, Contents, URL, ToolName, DataSetWID)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{WID}', '{OtherWID}', '{Format}', '{Contents}', '{URL}', '{ToolName}', '{DataSetWID}')"
        Else
            Return $"('{WID}', '{OtherWID}', '{Format}', '{Contents}', '{URL}', '{ToolName}', '{DataSetWID}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `archive` (`WID`, `OtherWID`, `Format`, `Contents`, `URL`, `ToolName`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, OtherWID, Format, Contents, URL, ToolName, DataSetWID)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `archive` (`WID`, `OtherWID`, `Format`, `Contents`, `URL`, `ToolName`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, WID, OtherWID, Format, Contents, URL, ToolName, DataSetWID)
        Else
        Return String.Format(REPLACE_SQL, WID, OtherWID, Format, Contents, URL, ToolName, DataSetWID)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `archive` SET `WID`='{0}', `OtherWID`='{1}', `Format`='{2}', `Contents`='{3}', `URL`='{4}', `ToolName`='{5}', `DataSetWID`='{6}' WHERE `WID` = '{7}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, OtherWID, Format, Contents, URL, ToolName, DataSetWID, WID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As archive
                         Return DirectCast(MyClass.MemberwiseClone, archive)
                     End Function
End Class


End Namespace
