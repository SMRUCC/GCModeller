#Region "Microsoft.VisualBasic::6ccf062915e60932f56972eaa7535f50, data\ExternalDBSource\MetaCyc\MySQL\featureinformation.vb"

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

    ' Class featureinformation
    ' 
    '     Properties: DataSetWID, Feature, FeatureReporterMap, WID
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
''' DROP TABLE IF EXISTS `featureinformation`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `featureinformation` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `Feature` bigint(20) DEFAULT NULL,
'''   `FeatureReporterMap` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_FeatureInformation1` (`DataSetWID`),
'''   KEY `FK_FeatureInformation2` (`Feature`),
'''   KEY `FK_FeatureInformation3` (`FeatureReporterMap`),
'''   CONSTRAINT `FK_FeatureInformation1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_FeatureInformation2` FOREIGN KEY (`Feature`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_FeatureInformation3` FOREIGN KEY (`FeatureReporterMap`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("featureinformation", Database:="warehouse", SchemaSQL:="
CREATE TABLE `featureinformation` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `Feature` bigint(20) DEFAULT NULL,
  `FeatureReporterMap` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_FeatureInformation1` (`DataSetWID`),
  KEY `FK_FeatureInformation2` (`Feature`),
  KEY `FK_FeatureInformation3` (`FeatureReporterMap`),
  CONSTRAINT `FK_FeatureInformation1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_FeatureInformation2` FOREIGN KEY (`Feature`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_FeatureInformation3` FOREIGN KEY (`FeatureReporterMap`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class featureinformation: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DataSetWID")> Public Property DataSetWID As Long
    <DatabaseField("Feature"), DataType(MySqlDbType.Int64, "20"), Column(Name:="Feature")> Public Property Feature As Long
    <DatabaseField("FeatureReporterMap"), DataType(MySqlDbType.Int64, "20"), Column(Name:="FeatureReporterMap")> Public Property FeatureReporterMap As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `featureinformation` (`WID`, `DataSetWID`, `Feature`, `FeatureReporterMap`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `featureinformation` (`WID`, `DataSetWID`, `Feature`, `FeatureReporterMap`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `featureinformation` (`WID`, `DataSetWID`, `Feature`, `FeatureReporterMap`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `featureinformation` (`WID`, `DataSetWID`, `Feature`, `FeatureReporterMap`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `featureinformation` WHERE `WID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `featureinformation` SET `WID`='{0}', `DataSetWID`='{1}', `Feature`='{2}', `FeatureReporterMap`='{3}' WHERE `WID` = '{4}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `featureinformation` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `featureinformation` (`WID`, `DataSetWID`, `Feature`, `FeatureReporterMap`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, Feature, FeatureReporterMap)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `featureinformation` (`WID`, `DataSetWID`, `Feature`, `FeatureReporterMap`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, WID, DataSetWID, Feature, FeatureReporterMap)
        Else
        Return String.Format(INSERT_SQL, WID, DataSetWID, Feature, FeatureReporterMap)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{WID}', '{DataSetWID}', '{Feature}', '{FeatureReporterMap}')"
        Else
            Return $"('{WID}', '{DataSetWID}', '{Feature}', '{FeatureReporterMap}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `featureinformation` (`WID`, `DataSetWID`, `Feature`, `FeatureReporterMap`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, Feature, FeatureReporterMap)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `featureinformation` (`WID`, `DataSetWID`, `Feature`, `FeatureReporterMap`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, WID, DataSetWID, Feature, FeatureReporterMap)
        Else
        Return String.Format(REPLACE_SQL, WID, DataSetWID, Feature, FeatureReporterMap)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `featureinformation` SET `WID`='{0}', `DataSetWID`='{1}', `Feature`='{2}', `FeatureReporterMap`='{3}' WHERE `WID` = '{4}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, Feature, FeatureReporterMap, WID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As featureinformation
                         Return DirectCast(MyClass.MemberwiseClone, featureinformation)
                     End Function
End Class


End Namespace
