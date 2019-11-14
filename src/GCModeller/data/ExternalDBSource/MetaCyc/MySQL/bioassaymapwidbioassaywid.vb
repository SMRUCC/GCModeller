#Region "Microsoft.VisualBasic::43ab37cd7727e65c2513a9f3211d4e44, data\ExternalDBSource\MetaCyc\MySQL\bioassaymapwidbioassaywid.vb"

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

    ' Class bioassaymapwidbioassaywid
    ' 
    '     Properties: BioAssayMapWID, BioAssayWID
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
''' DROP TABLE IF EXISTS `bioassaymapwidbioassaywid`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `bioassaymapwidbioassaywid` (
'''   `BioAssayMapWID` bigint(20) NOT NULL,
'''   `BioAssayWID` bigint(20) NOT NULL,
'''   KEY `FK_BioAssayMapWIDBioAssayWID1` (`BioAssayMapWID`),
'''   KEY `FK_BioAssayMapWIDBioAssayWID2` (`BioAssayWID`),
'''   CONSTRAINT `FK_BioAssayMapWIDBioAssayWID1` FOREIGN KEY (`BioAssayMapWID`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_BioAssayMapWIDBioAssayWID2` FOREIGN KEY (`BioAssayWID`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("bioassaymapwidbioassaywid", Database:="warehouse", SchemaSQL:="
CREATE TABLE `bioassaymapwidbioassaywid` (
  `BioAssayMapWID` bigint(20) NOT NULL,
  `BioAssayWID` bigint(20) NOT NULL,
  KEY `FK_BioAssayMapWIDBioAssayWID1` (`BioAssayMapWID`),
  KEY `FK_BioAssayMapWIDBioAssayWID2` (`BioAssayWID`),
  CONSTRAINT `FK_BioAssayMapWIDBioAssayWID1` FOREIGN KEY (`BioAssayMapWID`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_BioAssayMapWIDBioAssayWID2` FOREIGN KEY (`BioAssayWID`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class bioassaymapwidbioassaywid: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("BioAssayMapWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="BioAssayMapWID"), XmlAttribute> Public Property BioAssayMapWID As Long
    <DatabaseField("BioAssayWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="BioAssayWID")> Public Property BioAssayWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `bioassaymapwidbioassaywid` (`BioAssayMapWID`, `BioAssayWID`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `bioassaymapwidbioassaywid` (`BioAssayMapWID`, `BioAssayWID`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `bioassaymapwidbioassaywid` (`BioAssayMapWID`, `BioAssayWID`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `bioassaymapwidbioassaywid` (`BioAssayMapWID`, `BioAssayWID`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `bioassaymapwidbioassaywid` WHERE `BioAssayMapWID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `bioassaymapwidbioassaywid` SET `BioAssayMapWID`='{0}', `BioAssayWID`='{1}' WHERE `BioAssayMapWID` = '{2}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `bioassaymapwidbioassaywid` WHERE `BioAssayMapWID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, BioAssayMapWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `bioassaymapwidbioassaywid` (`BioAssayMapWID`, `BioAssayWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, BioAssayMapWID, BioAssayWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `bioassaymapwidbioassaywid` (`BioAssayMapWID`, `BioAssayWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, BioAssayMapWID, BioAssayWID)
        Else
        Return String.Format(INSERT_SQL, BioAssayMapWID, BioAssayWID)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{BioAssayMapWID}', '{BioAssayWID}')"
        Else
            Return $"('{BioAssayMapWID}', '{BioAssayWID}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `bioassaymapwidbioassaywid` (`BioAssayMapWID`, `BioAssayWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, BioAssayMapWID, BioAssayWID)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `bioassaymapwidbioassaywid` (`BioAssayMapWID`, `BioAssayWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, BioAssayMapWID, BioAssayWID)
        Else
        Return String.Format(REPLACE_SQL, BioAssayMapWID, BioAssayWID)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `bioassaymapwidbioassaywid` SET `BioAssayMapWID`='{0}', `BioAssayWID`='{1}' WHERE `BioAssayMapWID` = '{2}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, BioAssayMapWID, BioAssayWID, BioAssayMapWID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As bioassaymapwidbioassaywid
                         Return DirectCast(MyClass.MemberwiseClone, bioassaymapwidbioassaywid)
                     End Function
End Class


End Namespace
