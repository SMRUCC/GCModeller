#Region "Microsoft.VisualBasic::89541dddc3c5be2d993b4a9c2f7f9e5f, data\ExternalDBSource\MetaCyc\MySQL\sequenceposition.vb"

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

    ' Class sequenceposition
    ' 
    '     Properties: [End], Composite, CompositeCompositeMap, DataSetWID, MAGEClass
    '                 Reporter, ReporterCompositeMap, Start_, WID
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
''' DROP TABLE IF EXISTS `sequenceposition`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `sequenceposition` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `MAGEClass` varchar(100) NOT NULL,
'''   `Start_` smallint(6) DEFAULT NULL,
'''   `End` smallint(6) DEFAULT NULL,
'''   `CompositeCompositeMap` bigint(20) DEFAULT NULL,
'''   `Composite` bigint(20) DEFAULT NULL,
'''   `ReporterCompositeMap` bigint(20) DEFAULT NULL,
'''   `Reporter` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_SequencePosition1` (`DataSetWID`),
'''   KEY `FK_SequencePosition2` (`CompositeCompositeMap`),
'''   KEY `FK_SequencePosition3` (`Composite`),
'''   KEY `FK_SequencePosition4` (`ReporterCompositeMap`),
'''   KEY `FK_SequencePosition5` (`Reporter`),
'''   CONSTRAINT `FK_SequencePosition1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_SequencePosition2` FOREIGN KEY (`CompositeCompositeMap`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_SequencePosition3` FOREIGN KEY (`Composite`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_SequencePosition4` FOREIGN KEY (`ReporterCompositeMap`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_SequencePosition5` FOREIGN KEY (`Reporter`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("sequenceposition", Database:="warehouse", SchemaSQL:="
CREATE TABLE `sequenceposition` (
  `WID` bigint(20) NOT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  `MAGEClass` varchar(100) NOT NULL,
  `Start_` smallint(6) DEFAULT NULL,
  `End` smallint(6) DEFAULT NULL,
  `CompositeCompositeMap` bigint(20) DEFAULT NULL,
  `Composite` bigint(20) DEFAULT NULL,
  `ReporterCompositeMap` bigint(20) DEFAULT NULL,
  `Reporter` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_SequencePosition1` (`DataSetWID`),
  KEY `FK_SequencePosition2` (`CompositeCompositeMap`),
  KEY `FK_SequencePosition3` (`Composite`),
  KEY `FK_SequencePosition4` (`ReporterCompositeMap`),
  KEY `FK_SequencePosition5` (`Reporter`),
  CONSTRAINT `FK_SequencePosition1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_SequencePosition2` FOREIGN KEY (`CompositeCompositeMap`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_SequencePosition3` FOREIGN KEY (`Composite`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_SequencePosition4` FOREIGN KEY (`ReporterCompositeMap`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_SequencePosition5` FOREIGN KEY (`Reporter`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class sequenceposition: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DataSetWID")> Public Property DataSetWID As Long
    <DatabaseField("MAGEClass"), NotNull, DataType(MySqlDbType.VarChar, "100"), Column(Name:="MAGEClass")> Public Property MAGEClass As String
    <DatabaseField("Start_"), DataType(MySqlDbType.Int64, "6"), Column(Name:="Start_")> Public Property Start_ As Long
    <DatabaseField("End"), DataType(MySqlDbType.Int64, "6"), Column(Name:="End")> Public Property [End] As Long
    <DatabaseField("CompositeCompositeMap"), DataType(MySqlDbType.Int64, "20"), Column(Name:="CompositeCompositeMap")> Public Property CompositeCompositeMap As Long
    <DatabaseField("Composite"), DataType(MySqlDbType.Int64, "20"), Column(Name:="Composite")> Public Property Composite As Long
    <DatabaseField("ReporterCompositeMap"), DataType(MySqlDbType.Int64, "20"), Column(Name:="ReporterCompositeMap")> Public Property ReporterCompositeMap As Long
    <DatabaseField("Reporter"), DataType(MySqlDbType.Int64, "20"), Column(Name:="Reporter")> Public Property Reporter As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `sequenceposition` (`WID`, `DataSetWID`, `MAGEClass`, `Start_`, `End`, `CompositeCompositeMap`, `Composite`, `ReporterCompositeMap`, `Reporter`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `sequenceposition` (`WID`, `DataSetWID`, `MAGEClass`, `Start_`, `End`, `CompositeCompositeMap`, `Composite`, `ReporterCompositeMap`, `Reporter`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `sequenceposition` (`WID`, `DataSetWID`, `MAGEClass`, `Start_`, `End`, `CompositeCompositeMap`, `Composite`, `ReporterCompositeMap`, `Reporter`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `sequenceposition` (`WID`, `DataSetWID`, `MAGEClass`, `Start_`, `End`, `CompositeCompositeMap`, `Composite`, `ReporterCompositeMap`, `Reporter`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `sequenceposition` WHERE `WID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `sequenceposition` SET `WID`='{0}', `DataSetWID`='{1}', `MAGEClass`='{2}', `Start_`='{3}', `End`='{4}', `CompositeCompositeMap`='{5}', `Composite`='{6}', `ReporterCompositeMap`='{7}', `Reporter`='{8}' WHERE `WID` = '{9}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `sequenceposition` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `sequenceposition` (`WID`, `DataSetWID`, `MAGEClass`, `Start_`, `End`, `CompositeCompositeMap`, `Composite`, `ReporterCompositeMap`, `Reporter`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, MAGEClass, Start_, [End], CompositeCompositeMap, Composite, ReporterCompositeMap, Reporter)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `sequenceposition` (`WID`, `DataSetWID`, `MAGEClass`, `Start_`, `End`, `CompositeCompositeMap`, `Composite`, `ReporterCompositeMap`, `Reporter`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, WID, DataSetWID, MAGEClass, Start_, [End], CompositeCompositeMap, Composite, ReporterCompositeMap, Reporter)
        Else
        Return String.Format(INSERT_SQL, WID, DataSetWID, MAGEClass, Start_, [End], CompositeCompositeMap, Composite, ReporterCompositeMap, Reporter)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{WID}', '{DataSetWID}', '{MAGEClass}', '{Start_}', '{[End]}', '{CompositeCompositeMap}', '{Composite}', '{ReporterCompositeMap}', '{Reporter}')"
        Else
            Return $"('{WID}', '{DataSetWID}', '{MAGEClass}', '{Start_}', '{[End]}', '{CompositeCompositeMap}', '{Composite}', '{ReporterCompositeMap}', '{Reporter}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `sequenceposition` (`WID`, `DataSetWID`, `MAGEClass`, `Start_`, `End`, `CompositeCompositeMap`, `Composite`, `ReporterCompositeMap`, `Reporter`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, MAGEClass, Start_, [End], CompositeCompositeMap, Composite, ReporterCompositeMap, Reporter)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `sequenceposition` (`WID`, `DataSetWID`, `MAGEClass`, `Start_`, `End`, `CompositeCompositeMap`, `Composite`, `ReporterCompositeMap`, `Reporter`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, WID, DataSetWID, MAGEClass, Start_, [End], CompositeCompositeMap, Composite, ReporterCompositeMap, Reporter)
        Else
        Return String.Format(REPLACE_SQL, WID, DataSetWID, MAGEClass, Start_, [End], CompositeCompositeMap, Composite, ReporterCompositeMap, Reporter)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `sequenceposition` SET `WID`='{0}', `DataSetWID`='{1}', `MAGEClass`='{2}', `Start_`='{3}', `End`='{4}', `CompositeCompositeMap`='{5}', `Composite`='{6}', `ReporterCompositeMap`='{7}', `Reporter`='{8}' WHERE `WID` = '{9}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, MAGEClass, Start_, [End], CompositeCompositeMap, Composite, ReporterCompositeMap, Reporter, WID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As sequenceposition
                         Return DirectCast(MyClass.MemberwiseClone, sequenceposition)
                     End Function
End Class


End Namespace
