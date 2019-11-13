#Region "Microsoft.VisualBasic::88ca26bd7c5da7fc1e7cb8a77501a451, data\ExternalDBSource\MetaCyc\MySQL\enzymaticreaction.vb"

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

    ' Class enzymaticreaction
    ' 
    '     Properties: ComplexWID, DataSetWID, ProteinWID, ReactionDirection, ReactionWID
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
''' DROP TABLE IF EXISTS `enzymaticreaction`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `enzymaticreaction` (
'''   `WID` bigint(20) NOT NULL,
'''   `ReactionWID` bigint(20) NOT NULL,
'''   `ProteinWID` bigint(20) NOT NULL,
'''   `ComplexWID` bigint(20) DEFAULT NULL,
'''   `ReactionDirection` varchar(30) DEFAULT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `ER_DATASETWID` (`DataSetWID`),
'''   KEY `FK_EnzymaticReaction1` (`ReactionWID`),
'''   KEY `FK_EnzymaticReaction2` (`ProteinWID`),
'''   KEY `FK_EnzymaticReaction3` (`ComplexWID`),
'''   CONSTRAINT `FK_EnzymaticReaction1` FOREIGN KEY (`ReactionWID`) REFERENCES `reaction` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_EnzymaticReaction2` FOREIGN KEY (`ProteinWID`) REFERENCES `protein` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_EnzymaticReaction3` FOREIGN KEY (`ComplexWID`) REFERENCES `protein` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_EnzymaticReaction4` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("enzymaticreaction", Database:="warehouse", SchemaSQL:="
CREATE TABLE `enzymaticreaction` (
  `WID` bigint(20) NOT NULL,
  `ReactionWID` bigint(20) NOT NULL,
  `ProteinWID` bigint(20) NOT NULL,
  `ComplexWID` bigint(20) DEFAULT NULL,
  `ReactionDirection` varchar(30) DEFAULT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `ER_DATASETWID` (`DataSetWID`),
  KEY `FK_EnzymaticReaction1` (`ReactionWID`),
  KEY `FK_EnzymaticReaction2` (`ProteinWID`),
  KEY `FK_EnzymaticReaction3` (`ComplexWID`),
  CONSTRAINT `FK_EnzymaticReaction1` FOREIGN KEY (`ReactionWID`) REFERENCES `reaction` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_EnzymaticReaction2` FOREIGN KEY (`ProteinWID`) REFERENCES `protein` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_EnzymaticReaction3` FOREIGN KEY (`ComplexWID`) REFERENCES `protein` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_EnzymaticReaction4` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class enzymaticreaction: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("ReactionWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="ReactionWID")> Public Property ReactionWID As Long
    <DatabaseField("ProteinWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="ProteinWID")> Public Property ProteinWID As Long
    <DatabaseField("ComplexWID"), DataType(MySqlDbType.Int64, "20"), Column(Name:="ComplexWID")> Public Property ComplexWID As Long
    <DatabaseField("ReactionDirection"), DataType(MySqlDbType.VarChar, "30"), Column(Name:="ReactionDirection")> Public Property ReactionDirection As String
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DataSetWID")> Public Property DataSetWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `enzymaticreaction` (`WID`, `ReactionWID`, `ProteinWID`, `ComplexWID`, `ReactionDirection`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `enzymaticreaction` (`WID`, `ReactionWID`, `ProteinWID`, `ComplexWID`, `ReactionDirection`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `enzymaticreaction` (`WID`, `ReactionWID`, `ProteinWID`, `ComplexWID`, `ReactionDirection`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `enzymaticreaction` (`WID`, `ReactionWID`, `ProteinWID`, `ComplexWID`, `ReactionDirection`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `enzymaticreaction` WHERE `WID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `enzymaticreaction` SET `WID`='{0}', `ReactionWID`='{1}', `ProteinWID`='{2}', `ComplexWID`='{3}', `ReactionDirection`='{4}', `DataSetWID`='{5}' WHERE `WID` = '{6}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `enzymaticreaction` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `enzymaticreaction` (`WID`, `ReactionWID`, `ProteinWID`, `ComplexWID`, `ReactionDirection`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, ReactionWID, ProteinWID, ComplexWID, ReactionDirection, DataSetWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `enzymaticreaction` (`WID`, `ReactionWID`, `ProteinWID`, `ComplexWID`, `ReactionDirection`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, WID, ReactionWID, ProteinWID, ComplexWID, ReactionDirection, DataSetWID)
        Else
        Return String.Format(INSERT_SQL, WID, ReactionWID, ProteinWID, ComplexWID, ReactionDirection, DataSetWID)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{WID}', '{ReactionWID}', '{ProteinWID}', '{ComplexWID}', '{ReactionDirection}', '{DataSetWID}')"
        Else
            Return $"('{WID}', '{ReactionWID}', '{ProteinWID}', '{ComplexWID}', '{ReactionDirection}', '{DataSetWID}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `enzymaticreaction` (`WID`, `ReactionWID`, `ProteinWID`, `ComplexWID`, `ReactionDirection`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, ReactionWID, ProteinWID, ComplexWID, ReactionDirection, DataSetWID)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `enzymaticreaction` (`WID`, `ReactionWID`, `ProteinWID`, `ComplexWID`, `ReactionDirection`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, WID, ReactionWID, ProteinWID, ComplexWID, ReactionDirection, DataSetWID)
        Else
        Return String.Format(REPLACE_SQL, WID, ReactionWID, ProteinWID, ComplexWID, ReactionDirection, DataSetWID)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `enzymaticreaction` SET `WID`='{0}', `ReactionWID`='{1}', `ProteinWID`='{2}', `ComplexWID`='{3}', `ReactionDirection`='{4}', `DataSetWID`='{5}' WHERE `WID` = '{6}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, ReactionWID, ProteinWID, ComplexWID, ReactionDirection, DataSetWID, WID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As enzymaticreaction
                         Return DirectCast(MyClass.MemberwiseClone, enzymaticreaction)
                     End Function
End Class


End Namespace
