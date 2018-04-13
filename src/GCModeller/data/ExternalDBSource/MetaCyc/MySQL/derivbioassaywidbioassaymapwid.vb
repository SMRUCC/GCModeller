#Region "Microsoft.VisualBasic::d73d9da601b6514fb117fb2ab096abac, data\ExternalDBSource\MetaCyc\MySQL\derivbioassaywidbioassaymapwid.vb"

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

    ' Class derivbioassaywidbioassaymapwid
    ' 
    '     Properties: BioAssayMapWID, DerivedBioAssayWID
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @3/16/2018 10:40:19 PM


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
''' DROP TABLE IF EXISTS `derivbioassaywidbioassaymapwid`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `derivbioassaywidbioassaymapwid` (
'''   `DerivedBioAssayWID` bigint(20) NOT NULL,
'''   `BioAssayMapWID` bigint(20) NOT NULL,
'''   KEY `FK_DerivBioAssayWIDBioAssayM1` (`DerivedBioAssayWID`),
'''   KEY `FK_DerivBioAssayWIDBioAssayM2` (`BioAssayMapWID`),
'''   CONSTRAINT `FK_DerivBioAssayWIDBioAssayM1` FOREIGN KEY (`DerivedBioAssayWID`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_DerivBioAssayWIDBioAssayM2` FOREIGN KEY (`BioAssayMapWID`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("derivbioassaywidbioassaymapwid", Database:="warehouse", SchemaSQL:="
CREATE TABLE `derivbioassaywidbioassaymapwid` (
  `DerivedBioAssayWID` bigint(20) NOT NULL,
  `BioAssayMapWID` bigint(20) NOT NULL,
  KEY `FK_DerivBioAssayWIDBioAssayM1` (`DerivedBioAssayWID`),
  KEY `FK_DerivBioAssayWIDBioAssayM2` (`BioAssayMapWID`),
  CONSTRAINT `FK_DerivBioAssayWIDBioAssayM1` FOREIGN KEY (`DerivedBioAssayWID`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_DerivBioAssayWIDBioAssayM2` FOREIGN KEY (`BioAssayMapWID`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class derivbioassaywidbioassaymapwid: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DerivedBioAssayWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DerivedBioAssayWID"), XmlAttribute> Public Property DerivedBioAssayWID As Long
    <DatabaseField("BioAssayMapWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="BioAssayMapWID")> Public Property BioAssayMapWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `derivbioassaywidbioassaymapwid` (`DerivedBioAssayWID`, `BioAssayMapWID`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `derivbioassaywidbioassaymapwid` (`DerivedBioAssayWID`, `BioAssayMapWID`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `derivbioassaywidbioassaymapwid` WHERE `DerivedBioAssayWID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `derivbioassaywidbioassaymapwid` SET `DerivedBioAssayWID`='{0}', `BioAssayMapWID`='{1}' WHERE `DerivedBioAssayWID` = '{2}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `derivbioassaywidbioassaymapwid` WHERE `DerivedBioAssayWID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DerivedBioAssayWID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `derivbioassaywidbioassaymapwid` (`DerivedBioAssayWID`, `BioAssayMapWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DerivedBioAssayWID, BioAssayMapWID)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{DerivedBioAssayWID}', '{BioAssayMapWID}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `derivbioassaywidbioassaymapwid` (`DerivedBioAssayWID`, `BioAssayMapWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DerivedBioAssayWID, BioAssayMapWID)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `derivbioassaywidbioassaymapwid` SET `DerivedBioAssayWID`='{0}', `BioAssayMapWID`='{1}' WHERE `DerivedBioAssayWID` = '{2}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DerivedBioAssayWID, BioAssayMapWID, DerivedBioAssayWID)
    End Function
#End Region
Public Function Clone() As derivbioassaywidbioassaymapwid
                  Return DirectCast(MyClass.MemberwiseClone, derivbioassaywidbioassaymapwid)
              End Function
End Class


End Namespace
