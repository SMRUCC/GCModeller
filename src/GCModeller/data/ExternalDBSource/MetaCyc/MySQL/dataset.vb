#Region "Microsoft.VisualBasic::c543c27dfff8b96f49951af6531b4c0c, data\ExternalDBSource\MetaCyc\MySQL\dataset.vb"

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

    ' Class dataset
    ' 
    '     Properties: Application, ApplicationVersion, ChangeDate, HomeURL, LoadDate
    '                 LoadedBy, Name, QueryURL, ReleaseDate, Version
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
''' DROP TABLE IF EXISTS `dataset`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `dataset` (
'''   `WID` bigint(20) NOT NULL,
'''   `Name` varchar(255) NOT NULL,
'''   `Version` varchar(50) DEFAULT NULL,
'''   `ReleaseDate` datetime DEFAULT NULL,
'''   `LoadDate` datetime NOT NULL,
'''   `ChangeDate` datetime DEFAULT NULL,
'''   `HomeURL` varchar(255) DEFAULT NULL,
'''   `QueryURL` varchar(255) DEFAULT NULL,
'''   `LoadedBy` varchar(255) DEFAULT NULL,
'''   `Application` varchar(255) DEFAULT NULL,
'''   `ApplicationVersion` varchar(255) DEFAULT NULL,
'''   PRIMARY KEY (`WID`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("dataset", Database:="warehouse", SchemaSQL:="
CREATE TABLE `dataset` (
  `WID` bigint(20) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Version` varchar(50) DEFAULT NULL,
  `ReleaseDate` datetime DEFAULT NULL,
  `LoadDate` datetime NOT NULL,
  `ChangeDate` datetime DEFAULT NULL,
  `HomeURL` varchar(255) DEFAULT NULL,
  `QueryURL` varchar(255) DEFAULT NULL,
  `LoadedBy` varchar(255) DEFAULT NULL,
  `Application` varchar(255) DEFAULT NULL,
  `ApplicationVersion` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`WID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class dataset: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("Name"), NotNull, DataType(MySqlDbType.VarChar, "255"), Column(Name:="Name")> Public Property Name As String
    <DatabaseField("Version"), DataType(MySqlDbType.VarChar, "50"), Column(Name:="Version")> Public Property Version As String
    <DatabaseField("ReleaseDate"), DataType(MySqlDbType.DateTime), Column(Name:="ReleaseDate")> Public Property ReleaseDate As Date
    <DatabaseField("LoadDate"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="LoadDate")> Public Property LoadDate As Date
    <DatabaseField("ChangeDate"), DataType(MySqlDbType.DateTime), Column(Name:="ChangeDate")> Public Property ChangeDate As Date
    <DatabaseField("HomeURL"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="HomeURL")> Public Property HomeURL As String
    <DatabaseField("QueryURL"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="QueryURL")> Public Property QueryURL As String
    <DatabaseField("LoadedBy"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="LoadedBy")> Public Property LoadedBy As String
    <DatabaseField("Application"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="Application")> Public Property Application As String
    <DatabaseField("ApplicationVersion"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="ApplicationVersion")> Public Property ApplicationVersion As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `dataset` (`WID`, `Name`, `Version`, `ReleaseDate`, `LoadDate`, `ChangeDate`, `HomeURL`, `QueryURL`, `LoadedBy`, `Application`, `ApplicationVersion`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `dataset` (`WID`, `Name`, `Version`, `ReleaseDate`, `LoadDate`, `ChangeDate`, `HomeURL`, `QueryURL`, `LoadedBy`, `Application`, `ApplicationVersion`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `dataset` (`WID`, `Name`, `Version`, `ReleaseDate`, `LoadDate`, `ChangeDate`, `HomeURL`, `QueryURL`, `LoadedBy`, `Application`, `ApplicationVersion`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `dataset` (`WID`, `Name`, `Version`, `ReleaseDate`, `LoadDate`, `ChangeDate`, `HomeURL`, `QueryURL`, `LoadedBy`, `Application`, `ApplicationVersion`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `dataset` WHERE `WID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `dataset` SET `WID`='{0}', `Name`='{1}', `Version`='{2}', `ReleaseDate`='{3}', `LoadDate`='{4}', `ChangeDate`='{5}', `HomeURL`='{6}', `QueryURL`='{7}', `LoadedBy`='{8}', `Application`='{9}', `ApplicationVersion`='{10}' WHERE `WID` = '{11}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `dataset` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `dataset` (`WID`, `Name`, `Version`, `ReleaseDate`, `LoadDate`, `ChangeDate`, `HomeURL`, `QueryURL`, `LoadedBy`, `Application`, `ApplicationVersion`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, Name, Version, MySqlScript.ToMySqlDateTimeString(ReleaseDate), MySqlScript.ToMySqlDateTimeString(LoadDate), MySqlScript.ToMySqlDateTimeString(ChangeDate), HomeURL, QueryURL, LoadedBy, Application, ApplicationVersion)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `dataset` (`WID`, `Name`, `Version`, `ReleaseDate`, `LoadDate`, `ChangeDate`, `HomeURL`, `QueryURL`, `LoadedBy`, `Application`, `ApplicationVersion`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, WID, Name, Version, MySqlScript.ToMySqlDateTimeString(ReleaseDate), MySqlScript.ToMySqlDateTimeString(LoadDate), MySqlScript.ToMySqlDateTimeString(ChangeDate), HomeURL, QueryURL, LoadedBy, Application, ApplicationVersion)
        Else
        Return String.Format(INSERT_SQL, WID, Name, Version, MySqlScript.ToMySqlDateTimeString(ReleaseDate), MySqlScript.ToMySqlDateTimeString(LoadDate), MySqlScript.ToMySqlDateTimeString(ChangeDate), HomeURL, QueryURL, LoadedBy, Application, ApplicationVersion)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{WID}', '{Name}', '{Version}', '{ReleaseDate}', '{LoadDate}', '{ChangeDate}', '{HomeURL}', '{QueryURL}', '{LoadedBy}', '{Application}', '{ApplicationVersion}')"
        Else
            Return $"('{WID}', '{Name}', '{Version}', '{ReleaseDate}', '{LoadDate}', '{ChangeDate}', '{HomeURL}', '{QueryURL}', '{LoadedBy}', '{Application}', '{ApplicationVersion}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `dataset` (`WID`, `Name`, `Version`, `ReleaseDate`, `LoadDate`, `ChangeDate`, `HomeURL`, `QueryURL`, `LoadedBy`, `Application`, `ApplicationVersion`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, Name, Version, MySqlScript.ToMySqlDateTimeString(ReleaseDate), MySqlScript.ToMySqlDateTimeString(LoadDate), MySqlScript.ToMySqlDateTimeString(ChangeDate), HomeURL, QueryURL, LoadedBy, Application, ApplicationVersion)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `dataset` (`WID`, `Name`, `Version`, `ReleaseDate`, `LoadDate`, `ChangeDate`, `HomeURL`, `QueryURL`, `LoadedBy`, `Application`, `ApplicationVersion`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, WID, Name, Version, MySqlScript.ToMySqlDateTimeString(ReleaseDate), MySqlScript.ToMySqlDateTimeString(LoadDate), MySqlScript.ToMySqlDateTimeString(ChangeDate), HomeURL, QueryURL, LoadedBy, Application, ApplicationVersion)
        Else
        Return String.Format(REPLACE_SQL, WID, Name, Version, MySqlScript.ToMySqlDateTimeString(ReleaseDate), MySqlScript.ToMySqlDateTimeString(LoadDate), MySqlScript.ToMySqlDateTimeString(ChangeDate), HomeURL, QueryURL, LoadedBy, Application, ApplicationVersion)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `dataset` SET `WID`='{0}', `Name`='{1}', `Version`='{2}', `ReleaseDate`='{3}', `LoadDate`='{4}', `ChangeDate`='{5}', `HomeURL`='{6}', `QueryURL`='{7}', `LoadedBy`='{8}', `Application`='{9}', `ApplicationVersion`='{10}' WHERE `WID` = '{11}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, Name, Version, MySqlScript.ToMySqlDateTimeString(ReleaseDate), MySqlScript.ToMySqlDateTimeString(LoadDate), MySqlScript.ToMySqlDateTimeString(ChangeDate), HomeURL, QueryURL, LoadedBy, Application, ApplicationVersion, WID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As dataset
                         Return DirectCast(MyClass.MemberwiseClone, dataset)
                     End Function
End Class


End Namespace
