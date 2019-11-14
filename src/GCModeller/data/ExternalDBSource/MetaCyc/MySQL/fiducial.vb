#Region "Microsoft.VisualBasic::1232a0f285779741a1ecc16004f5d05c, data\ExternalDBSource\MetaCyc\MySQL\fiducial.vb"

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

    ' Class fiducial
    ' 
    '     Properties: ArrayGroup_Fiducials, DataSetWID, Fiducial_DistanceUnit, Fiducial_FiducialType, Fiducial_Position
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
''' DROP TABLE IF EXISTS `fiducial`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `fiducial` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `ArrayGroup_Fiducials` bigint(20) DEFAULT NULL,
'''   `Fiducial_FiducialType` bigint(20) DEFAULT NULL,
'''   `Fiducial_DistanceUnit` bigint(20) DEFAULT NULL,
'''   `Fiducial_Position` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_Fiducial1` (`DataSetWID`),
'''   KEY `FK_Fiducial3` (`ArrayGroup_Fiducials`),
'''   KEY `FK_Fiducial4` (`Fiducial_FiducialType`),
'''   KEY `FK_Fiducial5` (`Fiducial_DistanceUnit`),
'''   KEY `FK_Fiducial6` (`Fiducial_Position`),
'''   CONSTRAINT `FK_Fiducial1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Fiducial3` FOREIGN KEY (`ArrayGroup_Fiducials`) REFERENCES `arraygroup` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Fiducial4` FOREIGN KEY (`Fiducial_FiducialType`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Fiducial5` FOREIGN KEY (`Fiducial_DistanceUnit`) REFERENCES `unit` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Fiducial6` FOREIGN KEY (`Fiducial_Position`) REFERENCES `position_` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("fiducial", Database:="warehouse", SchemaSQL:="
CREATE TABLE `fiducial` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `ArrayGroup_Fiducials` bigint(20) DEFAULT NULL,
  `Fiducial_FiducialType` bigint(20) DEFAULT NULL,
  `Fiducial_DistanceUnit` bigint(20) DEFAULT NULL,
  `Fiducial_Position` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_Fiducial1` (`DataSetWID`),
  KEY `FK_Fiducial3` (`ArrayGroup_Fiducials`),
  KEY `FK_Fiducial4` (`Fiducial_FiducialType`),
  KEY `FK_Fiducial5` (`Fiducial_DistanceUnit`),
  KEY `FK_Fiducial6` (`Fiducial_Position`),
  CONSTRAINT `FK_Fiducial1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Fiducial3` FOREIGN KEY (`ArrayGroup_Fiducials`) REFERENCES `arraygroup` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Fiducial4` FOREIGN KEY (`Fiducial_FiducialType`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Fiducial5` FOREIGN KEY (`Fiducial_DistanceUnit`) REFERENCES `unit` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Fiducial6` FOREIGN KEY (`Fiducial_Position`) REFERENCES `position_` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class fiducial: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DataSetWID")> Public Property DataSetWID As Long
    <DatabaseField("ArrayGroup_Fiducials"), DataType(MySqlDbType.Int64, "20"), Column(Name:="ArrayGroup_Fiducials")> Public Property ArrayGroup_Fiducials As Long
    <DatabaseField("Fiducial_FiducialType"), DataType(MySqlDbType.Int64, "20"), Column(Name:="Fiducial_FiducialType")> Public Property Fiducial_FiducialType As Long
    <DatabaseField("Fiducial_DistanceUnit"), DataType(MySqlDbType.Int64, "20"), Column(Name:="Fiducial_DistanceUnit")> Public Property Fiducial_DistanceUnit As Long
    <DatabaseField("Fiducial_Position"), DataType(MySqlDbType.Int64, "20"), Column(Name:="Fiducial_Position")> Public Property Fiducial_Position As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `fiducial` (`WID`, `DataSetWID`, `ArrayGroup_Fiducials`, `Fiducial_FiducialType`, `Fiducial_DistanceUnit`, `Fiducial_Position`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `fiducial` (`WID`, `DataSetWID`, `ArrayGroup_Fiducials`, `Fiducial_FiducialType`, `Fiducial_DistanceUnit`, `Fiducial_Position`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `fiducial` (`WID`, `DataSetWID`, `ArrayGroup_Fiducials`, `Fiducial_FiducialType`, `Fiducial_DistanceUnit`, `Fiducial_Position`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `fiducial` (`WID`, `DataSetWID`, `ArrayGroup_Fiducials`, `Fiducial_FiducialType`, `Fiducial_DistanceUnit`, `Fiducial_Position`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `fiducial` WHERE `WID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `fiducial` SET `WID`='{0}', `DataSetWID`='{1}', `ArrayGroup_Fiducials`='{2}', `Fiducial_FiducialType`='{3}', `Fiducial_DistanceUnit`='{4}', `Fiducial_Position`='{5}' WHERE `WID` = '{6}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `fiducial` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `fiducial` (`WID`, `DataSetWID`, `ArrayGroup_Fiducials`, `Fiducial_FiducialType`, `Fiducial_DistanceUnit`, `Fiducial_Position`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, ArrayGroup_Fiducials, Fiducial_FiducialType, Fiducial_DistanceUnit, Fiducial_Position)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `fiducial` (`WID`, `DataSetWID`, `ArrayGroup_Fiducials`, `Fiducial_FiducialType`, `Fiducial_DistanceUnit`, `Fiducial_Position`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, WID, DataSetWID, ArrayGroup_Fiducials, Fiducial_FiducialType, Fiducial_DistanceUnit, Fiducial_Position)
        Else
        Return String.Format(INSERT_SQL, WID, DataSetWID, ArrayGroup_Fiducials, Fiducial_FiducialType, Fiducial_DistanceUnit, Fiducial_Position)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{WID}', '{DataSetWID}', '{ArrayGroup_Fiducials}', '{Fiducial_FiducialType}', '{Fiducial_DistanceUnit}', '{Fiducial_Position}')"
        Else
            Return $"('{WID}', '{DataSetWID}', '{ArrayGroup_Fiducials}', '{Fiducial_FiducialType}', '{Fiducial_DistanceUnit}', '{Fiducial_Position}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `fiducial` (`WID`, `DataSetWID`, `ArrayGroup_Fiducials`, `Fiducial_FiducialType`, `Fiducial_DistanceUnit`, `Fiducial_Position`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, ArrayGroup_Fiducials, Fiducial_FiducialType, Fiducial_DistanceUnit, Fiducial_Position)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `fiducial` (`WID`, `DataSetWID`, `ArrayGroup_Fiducials`, `Fiducial_FiducialType`, `Fiducial_DistanceUnit`, `Fiducial_Position`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, WID, DataSetWID, ArrayGroup_Fiducials, Fiducial_FiducialType, Fiducial_DistanceUnit, Fiducial_Position)
        Else
        Return String.Format(REPLACE_SQL, WID, DataSetWID, ArrayGroup_Fiducials, Fiducial_FiducialType, Fiducial_DistanceUnit, Fiducial_Position)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `fiducial` SET `WID`='{0}', `DataSetWID`='{1}', `ArrayGroup_Fiducials`='{2}', `Fiducial_FiducialType`='{3}', `Fiducial_DistanceUnit`='{4}', `Fiducial_Position`='{5}' WHERE `WID` = '{6}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, ArrayGroup_Fiducials, Fiducial_FiducialType, Fiducial_DistanceUnit, Fiducial_Position, WID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As fiducial
                         Return DirectCast(MyClass.MemberwiseClone, fiducial)
                     End Function
End Class


End Namespace
