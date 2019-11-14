#Region "Microsoft.VisualBasic::31fc46d751ddf928827fa0bae4d217a7, data\ExternalDBSource\MetaCyc\MySQL\reaction.vb"

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

    ' Class reaction
    ' 
    '     Properties: DataSetWID, DeltaG, ECNumber, ECNumberProposed, Name
    '                 Spontaneous, WID
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
''' DROP TABLE IF EXISTS `reaction`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `reaction` (
'''   `WID` bigint(20) NOT NULL,
'''   `Name` varchar(250) DEFAULT NULL,
'''   `DeltaG` varchar(50) DEFAULT NULL,
'''   `ECNumber` varchar(50) DEFAULT NULL,
'''   `ECNumberProposed` varchar(50) DEFAULT NULL,
'''   `Spontaneous` char(1) DEFAULT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `REACTION_DWID` (`DataSetWID`),
'''   CONSTRAINT `FK_Reaction` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("reaction", Database:="warehouse", SchemaSQL:="
CREATE TABLE `reaction` (
  `WID` bigint(20) NOT NULL,
  `Name` varchar(250) DEFAULT NULL,
  `DeltaG` varchar(50) DEFAULT NULL,
  `ECNumber` varchar(50) DEFAULT NULL,
  `ECNumberProposed` varchar(50) DEFAULT NULL,
  `Spontaneous` char(1) DEFAULT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `REACTION_DWID` (`DataSetWID`),
  CONSTRAINT `FK_Reaction` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class reaction: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("Name"), DataType(MySqlDbType.VarChar, "250"), Column(Name:="Name")> Public Property Name As String
    <DatabaseField("DeltaG"), DataType(MySqlDbType.VarChar, "50"), Column(Name:="DeltaG")> Public Property DeltaG As String
    <DatabaseField("ECNumber"), DataType(MySqlDbType.VarChar, "50"), Column(Name:="ECNumber")> Public Property ECNumber As String
    <DatabaseField("ECNumberProposed"), DataType(MySqlDbType.VarChar, "50"), Column(Name:="ECNumberProposed")> Public Property ECNumberProposed As String
    <DatabaseField("Spontaneous"), DataType(MySqlDbType.VarChar, "1"), Column(Name:="Spontaneous")> Public Property Spontaneous As String
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DataSetWID")> Public Property DataSetWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `reaction` (`WID`, `Name`, `DeltaG`, `ECNumber`, `ECNumberProposed`, `Spontaneous`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `reaction` (`WID`, `Name`, `DeltaG`, `ECNumber`, `ECNumberProposed`, `Spontaneous`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `reaction` (`WID`, `Name`, `DeltaG`, `ECNumber`, `ECNumberProposed`, `Spontaneous`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `reaction` (`WID`, `Name`, `DeltaG`, `ECNumber`, `ECNumberProposed`, `Spontaneous`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `reaction` WHERE `WID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `reaction` SET `WID`='{0}', `Name`='{1}', `DeltaG`='{2}', `ECNumber`='{3}', `ECNumberProposed`='{4}', `Spontaneous`='{5}', `DataSetWID`='{6}' WHERE `WID` = '{7}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `reaction` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `reaction` (`WID`, `Name`, `DeltaG`, `ECNumber`, `ECNumberProposed`, `Spontaneous`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, Name, DeltaG, ECNumber, ECNumberProposed, Spontaneous, DataSetWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `reaction` (`WID`, `Name`, `DeltaG`, `ECNumber`, `ECNumberProposed`, `Spontaneous`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, WID, Name, DeltaG, ECNumber, ECNumberProposed, Spontaneous, DataSetWID)
        Else
        Return String.Format(INSERT_SQL, WID, Name, DeltaG, ECNumber, ECNumberProposed, Spontaneous, DataSetWID)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{WID}', '{Name}', '{DeltaG}', '{ECNumber}', '{ECNumberProposed}', '{Spontaneous}', '{DataSetWID}')"
        Else
            Return $"('{WID}', '{Name}', '{DeltaG}', '{ECNumber}', '{ECNumberProposed}', '{Spontaneous}', '{DataSetWID}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `reaction` (`WID`, `Name`, `DeltaG`, `ECNumber`, `ECNumberProposed`, `Spontaneous`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, Name, DeltaG, ECNumber, ECNumberProposed, Spontaneous, DataSetWID)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `reaction` (`WID`, `Name`, `DeltaG`, `ECNumber`, `ECNumberProposed`, `Spontaneous`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, WID, Name, DeltaG, ECNumber, ECNumberProposed, Spontaneous, DataSetWID)
        Else
        Return String.Format(REPLACE_SQL, WID, Name, DeltaG, ECNumber, ECNumberProposed, Spontaneous, DataSetWID)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `reaction` SET `WID`='{0}', `Name`='{1}', `DeltaG`='{2}', `ECNumber`='{3}', `ECNumberProposed`='{4}', `Spontaneous`='{5}', `DataSetWID`='{6}' WHERE `WID` = '{7}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, Name, DeltaG, ECNumber, ECNumberProposed, Spontaneous, DataSetWID, WID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As reaction
                         Return DirectCast(MyClass.MemberwiseClone, reaction)
                     End Function
End Class


End Namespace
