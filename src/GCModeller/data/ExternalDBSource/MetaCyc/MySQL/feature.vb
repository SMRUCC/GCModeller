#Region "Microsoft.VisualBasic::7b6377f42e473f5bee57e5bd12f74b79, data\ExternalDBSource\MetaCyc\MySQL\feature.vb"

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

    ' Class feature
    ' 
    '     Properties: [Class], [Variant], ComputationalSupport, DataSetWID, Description
    '                 EndPosition, EndPositionApproximate, ExperimentalSupport, PointType, RegionOrPoint
    '                 SequenceType, SequenceWID, StartPosition, StartPositionApproximate, Type
    '                 WID
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
''' DROP TABLE IF EXISTS `feature`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `feature` (
'''   `WID` bigint(20) NOT NULL,
'''   `Description` varchar(1300) DEFAULT NULL,
'''   `Type` varchar(50) DEFAULT NULL,
'''   `Class` varchar(50) DEFAULT NULL,
'''   `SequenceType` char(1) NOT NULL,
'''   `SequenceWID` bigint(20) DEFAULT NULL,
'''   `Variant` longtext,
'''   `RegionOrPoint` varchar(10) DEFAULT NULL,
'''   `PointType` varchar(10) DEFAULT NULL,
'''   `StartPosition` int(11) DEFAULT NULL,
'''   `EndPosition` int(11) DEFAULT NULL,
'''   `StartPositionApproximate` varchar(10) DEFAULT NULL,
'''   `EndPositionApproximate` varchar(10) DEFAULT NULL,
'''   `ExperimentalSupport` char(1) DEFAULT NULL,
'''   `ComputationalSupport` char(1) DEFAULT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_Feature` (`DataSetWID`),
'''   CONSTRAINT `FK_Feature` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("feature", Database:="warehouse", SchemaSQL:="
CREATE TABLE `feature` (
  `WID` bigint(20) NOT NULL,
  `Description` varchar(1300) DEFAULT NULL,
  `Type` varchar(50) DEFAULT NULL,
  `Class` varchar(50) DEFAULT NULL,
  `SequenceType` char(1) NOT NULL,
  `SequenceWID` bigint(20) DEFAULT NULL,
  `Variant` longtext,
  `RegionOrPoint` varchar(10) DEFAULT NULL,
  `PointType` varchar(10) DEFAULT NULL,
  `StartPosition` int(11) DEFAULT NULL,
  `EndPosition` int(11) DEFAULT NULL,
  `StartPositionApproximate` varchar(10) DEFAULT NULL,
  `EndPositionApproximate` varchar(10) DEFAULT NULL,
  `ExperimentalSupport` char(1) DEFAULT NULL,
  `ComputationalSupport` char(1) DEFAULT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_Feature` (`DataSetWID`),
  CONSTRAINT `FK_Feature` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class feature: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("Description"), DataType(MySqlDbType.VarChar, "1300"), Column(Name:="Description")> Public Property Description As String
    <DatabaseField("Type"), DataType(MySqlDbType.VarChar, "50"), Column(Name:="Type")> Public Property Type As String
    <DatabaseField("Class"), DataType(MySqlDbType.VarChar, "50"), Column(Name:="Class")> Public Property [Class] As String
    <DatabaseField("SequenceType"), NotNull, DataType(MySqlDbType.VarChar, "1"), Column(Name:="SequenceType")> Public Property SequenceType As String
    <DatabaseField("SequenceWID"), DataType(MySqlDbType.Int64, "20"), Column(Name:="SequenceWID")> Public Property SequenceWID As Long
    <DatabaseField("Variant"), DataType(MySqlDbType.Text), Column(Name:="Variant")> Public Property [Variant] As String
    <DatabaseField("RegionOrPoint"), DataType(MySqlDbType.VarChar, "10"), Column(Name:="RegionOrPoint")> Public Property RegionOrPoint As String
    <DatabaseField("PointType"), DataType(MySqlDbType.VarChar, "10"), Column(Name:="PointType")> Public Property PointType As String
    <DatabaseField("StartPosition"), DataType(MySqlDbType.Int64, "11"), Column(Name:="StartPosition")> Public Property StartPosition As Long
    <DatabaseField("EndPosition"), DataType(MySqlDbType.Int64, "11"), Column(Name:="EndPosition")> Public Property EndPosition As Long
    <DatabaseField("StartPositionApproximate"), DataType(MySqlDbType.VarChar, "10"), Column(Name:="StartPositionApproximate")> Public Property StartPositionApproximate As String
    <DatabaseField("EndPositionApproximate"), DataType(MySqlDbType.VarChar, "10"), Column(Name:="EndPositionApproximate")> Public Property EndPositionApproximate As String
    <DatabaseField("ExperimentalSupport"), DataType(MySqlDbType.VarChar, "1"), Column(Name:="ExperimentalSupport")> Public Property ExperimentalSupport As String
    <DatabaseField("ComputationalSupport"), DataType(MySqlDbType.VarChar, "1"), Column(Name:="ComputationalSupport")> Public Property ComputationalSupport As String
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DataSetWID")> Public Property DataSetWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `feature` (`WID`, `Description`, `Type`, `Class`, `SequenceType`, `SequenceWID`, `Variant`, `RegionOrPoint`, `PointType`, `StartPosition`, `EndPosition`, `StartPositionApproximate`, `EndPositionApproximate`, `ExperimentalSupport`, `ComputationalSupport`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `feature` (`WID`, `Description`, `Type`, `Class`, `SequenceType`, `SequenceWID`, `Variant`, `RegionOrPoint`, `PointType`, `StartPosition`, `EndPosition`, `StartPositionApproximate`, `EndPositionApproximate`, `ExperimentalSupport`, `ComputationalSupport`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `feature` (`WID`, `Description`, `Type`, `Class`, `SequenceType`, `SequenceWID`, `Variant`, `RegionOrPoint`, `PointType`, `StartPosition`, `EndPosition`, `StartPositionApproximate`, `EndPositionApproximate`, `ExperimentalSupport`, `ComputationalSupport`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `feature` (`WID`, `Description`, `Type`, `Class`, `SequenceType`, `SequenceWID`, `Variant`, `RegionOrPoint`, `PointType`, `StartPosition`, `EndPosition`, `StartPositionApproximate`, `EndPositionApproximate`, `ExperimentalSupport`, `ComputationalSupport`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `feature` WHERE `WID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `feature` SET `WID`='{0}', `Description`='{1}', `Type`='{2}', `Class`='{3}', `SequenceType`='{4}', `SequenceWID`='{5}', `Variant`='{6}', `RegionOrPoint`='{7}', `PointType`='{8}', `StartPosition`='{9}', `EndPosition`='{10}', `StartPositionApproximate`='{11}', `EndPositionApproximate`='{12}', `ExperimentalSupport`='{13}', `ComputationalSupport`='{14}', `DataSetWID`='{15}' WHERE `WID` = '{16}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `feature` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `feature` (`WID`, `Description`, `Type`, `Class`, `SequenceType`, `SequenceWID`, `Variant`, `RegionOrPoint`, `PointType`, `StartPosition`, `EndPosition`, `StartPositionApproximate`, `EndPositionApproximate`, `ExperimentalSupport`, `ComputationalSupport`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, Description, Type, [Class], SequenceType, SequenceWID, [Variant], RegionOrPoint, PointType, StartPosition, EndPosition, StartPositionApproximate, EndPositionApproximate, ExperimentalSupport, ComputationalSupport, DataSetWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `feature` (`WID`, `Description`, `Type`, `Class`, `SequenceType`, `SequenceWID`, `Variant`, `RegionOrPoint`, `PointType`, `StartPosition`, `EndPosition`, `StartPositionApproximate`, `EndPositionApproximate`, `ExperimentalSupport`, `ComputationalSupport`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, WID, Description, Type, [Class], SequenceType, SequenceWID, [Variant], RegionOrPoint, PointType, StartPosition, EndPosition, StartPositionApproximate, EndPositionApproximate, ExperimentalSupport, ComputationalSupport, DataSetWID)
        Else
        Return String.Format(INSERT_SQL, WID, Description, Type, [Class], SequenceType, SequenceWID, [Variant], RegionOrPoint, PointType, StartPosition, EndPosition, StartPositionApproximate, EndPositionApproximate, ExperimentalSupport, ComputationalSupport, DataSetWID)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{WID}', '{Description}', '{Type}', '{[Class]}', '{SequenceType}', '{SequenceWID}', '{[Variant]}', '{RegionOrPoint}', '{PointType}', '{StartPosition}', '{EndPosition}', '{StartPositionApproximate}', '{EndPositionApproximate}', '{ExperimentalSupport}', '{ComputationalSupport}', '{DataSetWID}')"
        Else
            Return $"('{WID}', '{Description}', '{Type}', '{[Class]}', '{SequenceType}', '{SequenceWID}', '{[Variant]}', '{RegionOrPoint}', '{PointType}', '{StartPosition}', '{EndPosition}', '{StartPositionApproximate}', '{EndPositionApproximate}', '{ExperimentalSupport}', '{ComputationalSupport}', '{DataSetWID}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `feature` (`WID`, `Description`, `Type`, `Class`, `SequenceType`, `SequenceWID`, `Variant`, `RegionOrPoint`, `PointType`, `StartPosition`, `EndPosition`, `StartPositionApproximate`, `EndPositionApproximate`, `ExperimentalSupport`, `ComputationalSupport`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, Description, Type, [Class], SequenceType, SequenceWID, [Variant], RegionOrPoint, PointType, StartPosition, EndPosition, StartPositionApproximate, EndPositionApproximate, ExperimentalSupport, ComputationalSupport, DataSetWID)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `feature` (`WID`, `Description`, `Type`, `Class`, `SequenceType`, `SequenceWID`, `Variant`, `RegionOrPoint`, `PointType`, `StartPosition`, `EndPosition`, `StartPositionApproximate`, `EndPositionApproximate`, `ExperimentalSupport`, `ComputationalSupport`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, WID, Description, Type, [Class], SequenceType, SequenceWID, [Variant], RegionOrPoint, PointType, StartPosition, EndPosition, StartPositionApproximate, EndPositionApproximate, ExperimentalSupport, ComputationalSupport, DataSetWID)
        Else
        Return String.Format(REPLACE_SQL, WID, Description, Type, [Class], SequenceType, SequenceWID, [Variant], RegionOrPoint, PointType, StartPosition, EndPosition, StartPositionApproximate, EndPositionApproximate, ExperimentalSupport, ComputationalSupport, DataSetWID)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `feature` SET `WID`='{0}', `Description`='{1}', `Type`='{2}', `Class`='{3}', `SequenceType`='{4}', `SequenceWID`='{5}', `Variant`='{6}', `RegionOrPoint`='{7}', `PointType`='{8}', `StartPosition`='{9}', `EndPosition`='{10}', `StartPositionApproximate`='{11}', `EndPositionApproximate`='{12}', `ExperimentalSupport`='{13}', `ComputationalSupport`='{14}', `DataSetWID`='{15}' WHERE `WID` = '{16}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, Description, Type, [Class], SequenceType, SequenceWID, [Variant], RegionOrPoint, PointType, StartPosition, EndPosition, StartPositionApproximate, EndPositionApproximate, ExperimentalSupport, ComputationalSupport, DataSetWID, WID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As feature
                         Return DirectCast(MyClass.MemberwiseClone, feature)
                     End Function
End Class


End Namespace
