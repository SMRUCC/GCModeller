#Region "Microsoft.VisualBasic::9b7eb176e4d47ae1daa5e0e64d856059, data\ExternalDBSource\MetaCyc\MySQL\transcriptionunitcomponent.vb"

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

    ' Class transcriptionunitcomponent
    ' 
    '     Properties: OtherWID, TranscriptionUnitWID, Type
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
''' DROP TABLE IF EXISTS `transcriptionunitcomponent`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `transcriptionunitcomponent` (
'''   `Type` varchar(100) NOT NULL,
'''   `TranscriptionUnitWID` bigint(20) NOT NULL,
'''   `OtherWID` bigint(20) NOT NULL,
'''   KEY `FK_TranscriptionUnitComponent1` (`TranscriptionUnitWID`),
'''   CONSTRAINT `FK_TranscriptionUnitComponent1` FOREIGN KEY (`TranscriptionUnitWID`) REFERENCES `transcriptionunit` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("transcriptionunitcomponent", Database:="warehouse", SchemaSQL:="
CREATE TABLE `transcriptionunitcomponent` (
  `Type` varchar(100) NOT NULL,
  `TranscriptionUnitWID` bigint(20) NOT NULL,
  `OtherWID` bigint(20) NOT NULL,
  KEY `FK_TranscriptionUnitComponent1` (`TranscriptionUnitWID`),
  CONSTRAINT `FK_TranscriptionUnitComponent1` FOREIGN KEY (`TranscriptionUnitWID`) REFERENCES `transcriptionunit` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class transcriptionunitcomponent: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("Type"), NotNull, DataType(MySqlDbType.VarChar, "100"), Column(Name:="Type")> Public Property Type As String
    <DatabaseField("TranscriptionUnitWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="TranscriptionUnitWID"), XmlAttribute> Public Property TranscriptionUnitWID As Long
    <DatabaseField("OtherWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="OtherWID")> Public Property OtherWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `transcriptionunitcomponent` (`Type`, `TranscriptionUnitWID`, `OtherWID`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `transcriptionunitcomponent` (`Type`, `TranscriptionUnitWID`, `OtherWID`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `transcriptionunitcomponent` (`Type`, `TranscriptionUnitWID`, `OtherWID`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `transcriptionunitcomponent` (`Type`, `TranscriptionUnitWID`, `OtherWID`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `transcriptionunitcomponent` WHERE `TranscriptionUnitWID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `transcriptionunitcomponent` SET `Type`='{0}', `TranscriptionUnitWID`='{1}', `OtherWID`='{2}' WHERE `TranscriptionUnitWID` = '{3}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `transcriptionunitcomponent` WHERE `TranscriptionUnitWID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, TranscriptionUnitWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `transcriptionunitcomponent` (`Type`, `TranscriptionUnitWID`, `OtherWID`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, Type, TranscriptionUnitWID, OtherWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `transcriptionunitcomponent` (`Type`, `TranscriptionUnitWID`, `OtherWID`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, Type, TranscriptionUnitWID, OtherWID)
        Else
        Return String.Format(INSERT_SQL, Type, TranscriptionUnitWID, OtherWID)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{Type}', '{TranscriptionUnitWID}', '{OtherWID}')"
        Else
            Return $"('{Type}', '{TranscriptionUnitWID}', '{OtherWID}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `transcriptionunitcomponent` (`Type`, `TranscriptionUnitWID`, `OtherWID`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, Type, TranscriptionUnitWID, OtherWID)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `transcriptionunitcomponent` (`Type`, `TranscriptionUnitWID`, `OtherWID`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, Type, TranscriptionUnitWID, OtherWID)
        Else
        Return String.Format(REPLACE_SQL, Type, TranscriptionUnitWID, OtherWID)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `transcriptionunitcomponent` SET `Type`='{0}', `TranscriptionUnitWID`='{1}', `OtherWID`='{2}' WHERE `TranscriptionUnitWID` = '{3}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, Type, TranscriptionUnitWID, OtherWID, TranscriptionUnitWID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As transcriptionunitcomponent
                         Return DirectCast(MyClass.MemberwiseClone, transcriptionunitcomponent)
                     End Function
End Class


End Namespace
