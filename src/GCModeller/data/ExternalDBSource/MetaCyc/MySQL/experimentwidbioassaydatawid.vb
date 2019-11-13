#Region "Microsoft.VisualBasic::1860bd7adf9a1e52160a678274a4f71f, data\ExternalDBSource\MetaCyc\MySQL\experimentwidbioassaydatawid.vb"

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

    ' Class experimentwidbioassaydatawid
    ' 
    '     Properties: BioAssayDataWID, ExperimentWID
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
''' DROP TABLE IF EXISTS `experimentwidbioassaydatawid`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `experimentwidbioassaydatawid` (
'''   `ExperimentWID` bigint(20) NOT NULL,
'''   `BioAssayDataWID` bigint(20) NOT NULL,
'''   KEY `FK_ExperimentWIDBioAssayData1` (`ExperimentWID`),
'''   KEY `FK_ExperimentWIDBioAssayData2` (`BioAssayDataWID`),
'''   CONSTRAINT `FK_ExperimentWIDBioAssayData1` FOREIGN KEY (`ExperimentWID`) REFERENCES `experiment` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_ExperimentWIDBioAssayData2` FOREIGN KEY (`BioAssayDataWID`) REFERENCES `bioassaydata` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("experimentwidbioassaydatawid", Database:="warehouse", SchemaSQL:="
CREATE TABLE `experimentwidbioassaydatawid` (
  `ExperimentWID` bigint(20) NOT NULL,
  `BioAssayDataWID` bigint(20) NOT NULL,
  KEY `FK_ExperimentWIDBioAssayData1` (`ExperimentWID`),
  KEY `FK_ExperimentWIDBioAssayData2` (`BioAssayDataWID`),
  CONSTRAINT `FK_ExperimentWIDBioAssayData1` FOREIGN KEY (`ExperimentWID`) REFERENCES `experiment` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ExperimentWIDBioAssayData2` FOREIGN KEY (`BioAssayDataWID`) REFERENCES `bioassaydata` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class experimentwidbioassaydatawid: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("ExperimentWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="ExperimentWID"), XmlAttribute> Public Property ExperimentWID As Long
    <DatabaseField("BioAssayDataWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="BioAssayDataWID")> Public Property BioAssayDataWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `experimentwidbioassaydatawid` (`ExperimentWID`, `BioAssayDataWID`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `experimentwidbioassaydatawid` (`ExperimentWID`, `BioAssayDataWID`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `experimentwidbioassaydatawid` (`ExperimentWID`, `BioAssayDataWID`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `experimentwidbioassaydatawid` (`ExperimentWID`, `BioAssayDataWID`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `experimentwidbioassaydatawid` WHERE `ExperimentWID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `experimentwidbioassaydatawid` SET `ExperimentWID`='{0}', `BioAssayDataWID`='{1}' WHERE `ExperimentWID` = '{2}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `experimentwidbioassaydatawid` WHERE `ExperimentWID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, ExperimentWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `experimentwidbioassaydatawid` (`ExperimentWID`, `BioAssayDataWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, ExperimentWID, BioAssayDataWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `experimentwidbioassaydatawid` (`ExperimentWID`, `BioAssayDataWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, ExperimentWID, BioAssayDataWID)
        Else
        Return String.Format(INSERT_SQL, ExperimentWID, BioAssayDataWID)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{ExperimentWID}', '{BioAssayDataWID}')"
        Else
            Return $"('{ExperimentWID}', '{BioAssayDataWID}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `experimentwidbioassaydatawid` (`ExperimentWID`, `BioAssayDataWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, ExperimentWID, BioAssayDataWID)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `experimentwidbioassaydatawid` (`ExperimentWID`, `BioAssayDataWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, ExperimentWID, BioAssayDataWID)
        Else
        Return String.Format(REPLACE_SQL, ExperimentWID, BioAssayDataWID)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `experimentwidbioassaydatawid` SET `ExperimentWID`='{0}', `BioAssayDataWID`='{1}' WHERE `ExperimentWID` = '{2}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, ExperimentWID, BioAssayDataWID, ExperimentWID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As experimentwidbioassaydatawid
                         Return DirectCast(MyClass.MemberwiseClone, experimentwidbioassaydatawid)
                     End Function
End Class


End Namespace
