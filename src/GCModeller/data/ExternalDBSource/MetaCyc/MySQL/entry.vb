#Region "Microsoft.VisualBasic::5a1def8a81324864fa25d7f5ab1afe99, data\ExternalDBSource\MetaCyc\MySQL\entry.vb"

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

    ' Class entry
    ' 
    '     Properties: CreationDate, DatasetWID, ErrorMessage, InsertDate, LineNumber
    '                 LoadError, ModifiedDate, OtherWID
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
''' DROP TABLE IF EXISTS `entry`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `entry` (
'''   `OtherWID` bigint(20) NOT NULL,
'''   `InsertDate` datetime NOT NULL,
'''   `CreationDate` datetime DEFAULT NULL,
'''   `ModifiedDate` datetime DEFAULT NULL,
'''   `LoadError` char(1) NOT NULL,
'''   `LineNumber` int(11) DEFAULT NULL,
'''   `ErrorMessage` text,
'''   `DatasetWID` bigint(20) NOT NULL,
'''   PRIMARY KEY (`OtherWID`),
'''   KEY `FK_Entry` (`DatasetWID`),
'''   CONSTRAINT `FK_Entry` FOREIGN KEY (`DatasetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("entry", Database:="warehouse", SchemaSQL:="
CREATE TABLE `entry` (
  `OtherWID` bigint(20) NOT NULL,
  `InsertDate` datetime NOT NULL,
  `CreationDate` datetime DEFAULT NULL,
  `ModifiedDate` datetime DEFAULT NULL,
  `LoadError` char(1) NOT NULL,
  `LineNumber` int(11) DEFAULT NULL,
  `ErrorMessage` text,
  `DatasetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`OtherWID`),
  KEY `FK_Entry` (`DatasetWID`),
  CONSTRAINT `FK_Entry` FOREIGN KEY (`DatasetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class entry: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("OtherWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="OtherWID"), XmlAttribute> Public Property OtherWID As Long
    <DatabaseField("InsertDate"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="InsertDate")> Public Property InsertDate As Date
    <DatabaseField("CreationDate"), DataType(MySqlDbType.DateTime), Column(Name:="CreationDate")> Public Property CreationDate As Date
    <DatabaseField("ModifiedDate"), DataType(MySqlDbType.DateTime), Column(Name:="ModifiedDate")> Public Property ModifiedDate As Date
    <DatabaseField("LoadError"), NotNull, DataType(MySqlDbType.VarChar, "1"), Column(Name:="LoadError")> Public Property LoadError As String
    <DatabaseField("LineNumber"), DataType(MySqlDbType.Int64, "11"), Column(Name:="LineNumber")> Public Property LineNumber As Long
    <DatabaseField("ErrorMessage"), DataType(MySqlDbType.Text), Column(Name:="ErrorMessage")> Public Property ErrorMessage As String
    <DatabaseField("DatasetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DatasetWID")> Public Property DatasetWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `entry` (`OtherWID`, `InsertDate`, `CreationDate`, `ModifiedDate`, `LoadError`, `LineNumber`, `ErrorMessage`, `DatasetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `entry` (`OtherWID`, `InsertDate`, `CreationDate`, `ModifiedDate`, `LoadError`, `LineNumber`, `ErrorMessage`, `DatasetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `entry` (`OtherWID`, `InsertDate`, `CreationDate`, `ModifiedDate`, `LoadError`, `LineNumber`, `ErrorMessage`, `DatasetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `entry` (`OtherWID`, `InsertDate`, `CreationDate`, `ModifiedDate`, `LoadError`, `LineNumber`, `ErrorMessage`, `DatasetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `entry` WHERE `OtherWID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `entry` SET `OtherWID`='{0}', `InsertDate`='{1}', `CreationDate`='{2}', `ModifiedDate`='{3}', `LoadError`='{4}', `LineNumber`='{5}', `ErrorMessage`='{6}', `DatasetWID`='{7}' WHERE `OtherWID` = '{8}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `entry` WHERE `OtherWID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, OtherWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `entry` (`OtherWID`, `InsertDate`, `CreationDate`, `ModifiedDate`, `LoadError`, `LineNumber`, `ErrorMessage`, `DatasetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, OtherWID, MySqlScript.ToMySqlDateTimeString(InsertDate), MySqlScript.ToMySqlDateTimeString(CreationDate), MySqlScript.ToMySqlDateTimeString(ModifiedDate), LoadError, LineNumber, ErrorMessage, DatasetWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `entry` (`OtherWID`, `InsertDate`, `CreationDate`, `ModifiedDate`, `LoadError`, `LineNumber`, `ErrorMessage`, `DatasetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, OtherWID, MySqlScript.ToMySqlDateTimeString(InsertDate), MySqlScript.ToMySqlDateTimeString(CreationDate), MySqlScript.ToMySqlDateTimeString(ModifiedDate), LoadError, LineNumber, ErrorMessage, DatasetWID)
        Else
        Return String.Format(INSERT_SQL, OtherWID, MySqlScript.ToMySqlDateTimeString(InsertDate), MySqlScript.ToMySqlDateTimeString(CreationDate), MySqlScript.ToMySqlDateTimeString(ModifiedDate), LoadError, LineNumber, ErrorMessage, DatasetWID)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{OtherWID}', '{InsertDate}', '{CreationDate}', '{ModifiedDate}', '{LoadError}', '{LineNumber}', '{ErrorMessage}', '{DatasetWID}')"
        Else
            Return $"('{OtherWID}', '{InsertDate}', '{CreationDate}', '{ModifiedDate}', '{LoadError}', '{LineNumber}', '{ErrorMessage}', '{DatasetWID}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `entry` (`OtherWID`, `InsertDate`, `CreationDate`, `ModifiedDate`, `LoadError`, `LineNumber`, `ErrorMessage`, `DatasetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, OtherWID, MySqlScript.ToMySqlDateTimeString(InsertDate), MySqlScript.ToMySqlDateTimeString(CreationDate), MySqlScript.ToMySqlDateTimeString(ModifiedDate), LoadError, LineNumber, ErrorMessage, DatasetWID)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `entry` (`OtherWID`, `InsertDate`, `CreationDate`, `ModifiedDate`, `LoadError`, `LineNumber`, `ErrorMessage`, `DatasetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, OtherWID, MySqlScript.ToMySqlDateTimeString(InsertDate), MySqlScript.ToMySqlDateTimeString(CreationDate), MySqlScript.ToMySqlDateTimeString(ModifiedDate), LoadError, LineNumber, ErrorMessage, DatasetWID)
        Else
        Return String.Format(REPLACE_SQL, OtherWID, MySqlScript.ToMySqlDateTimeString(InsertDate), MySqlScript.ToMySqlDateTimeString(CreationDate), MySqlScript.ToMySqlDateTimeString(ModifiedDate), LoadError, LineNumber, ErrorMessage, DatasetWID)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `entry` SET `OtherWID`='{0}', `InsertDate`='{1}', `CreationDate`='{2}', `ModifiedDate`='{3}', `LoadError`='{4}', `LineNumber`='{5}', `ErrorMessage`='{6}', `DatasetWID`='{7}' WHERE `OtherWID` = '{8}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, OtherWID, MySqlScript.ToMySqlDateTimeString(InsertDate), MySqlScript.ToMySqlDateTimeString(CreationDate), MySqlScript.ToMySqlDateTimeString(ModifiedDate), LoadError, LineNumber, ErrorMessage, DatasetWID, OtherWID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As entry
                         Return DirectCast(MyClass.MemberwiseClone, entry)
                     End Function
End Class


End Namespace
