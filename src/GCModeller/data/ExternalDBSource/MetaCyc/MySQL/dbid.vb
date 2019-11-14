#Region "Microsoft.VisualBasic::07afc8f9a62bb32944d10fd0db66a442, data\ExternalDBSource\MetaCyc\MySQL\dbid.vb"

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

    ' Class dbid
    ' 
    '     Properties: OtherWID, Type, Version, XID
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
''' DROP TABLE IF EXISTS `dbid`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `dbid` (
'''   `OtherWID` bigint(20) NOT NULL,
'''   `XID` varchar(150) NOT NULL,
'''   `Type` varchar(20) DEFAULT NULL,
'''   `Version` varchar(10) DEFAULT NULL,
'''   KEY `DBID_XID_OTHERWID` (`XID`,`OtherWID`),
'''   KEY `DBID_OTHERWID` (`OtherWID`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("dbid", Database:="warehouse", SchemaSQL:="
CREATE TABLE `dbid` (
  `OtherWID` bigint(20) NOT NULL,
  `XID` varchar(150) NOT NULL,
  `Type` varchar(20) DEFAULT NULL,
  `Version` varchar(10) DEFAULT NULL,
  KEY `DBID_XID_OTHERWID` (`XID`,`OtherWID`),
  KEY `DBID_OTHERWID` (`OtherWID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class dbid: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("OtherWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="OtherWID"), XmlAttribute> Public Property OtherWID As Long
    <DatabaseField("XID"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "150"), Column(Name:="XID"), XmlAttribute> Public Property XID As String
    <DatabaseField("Type"), DataType(MySqlDbType.VarChar, "20"), Column(Name:="Type")> Public Property Type As String
    <DatabaseField("Version"), DataType(MySqlDbType.VarChar, "10"), Column(Name:="Version")> Public Property Version As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `dbid` (`OtherWID`, `XID`, `Type`, `Version`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `dbid` (`OtherWID`, `XID`, `Type`, `Version`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `dbid` (`OtherWID`, `XID`, `Type`, `Version`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `dbid` (`OtherWID`, `XID`, `Type`, `Version`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `dbid` WHERE `XID`='{0}' and `OtherWID`='{1}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `dbid` SET `OtherWID`='{0}', `XID`='{1}', `Type`='{2}', `Version`='{3}' WHERE `XID`='{4}' and `OtherWID`='{5}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `dbid` WHERE `XID`='{0}' and `OtherWID`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, XID, OtherWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `dbid` (`OtherWID`, `XID`, `Type`, `Version`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, OtherWID, XID, Type, Version)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `dbid` (`OtherWID`, `XID`, `Type`, `Version`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, OtherWID, XID, Type, Version)
        Else
        Return String.Format(INSERT_SQL, OtherWID, XID, Type, Version)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{OtherWID}', '{XID}', '{Type}', '{Version}')"
        Else
            Return $"('{OtherWID}', '{XID}', '{Type}', '{Version}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `dbid` (`OtherWID`, `XID`, `Type`, `Version`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, OtherWID, XID, Type, Version)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `dbid` (`OtherWID`, `XID`, `Type`, `Version`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, OtherWID, XID, Type, Version)
        Else
        Return String.Format(REPLACE_SQL, OtherWID, XID, Type, Version)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `dbid` SET `OtherWID`='{0}', `XID`='{1}', `Type`='{2}', `Version`='{3}' WHERE `XID`='{4}' and `OtherWID`='{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, OtherWID, XID, Type, Version, XID, OtherWID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As dbid
                         Return DirectCast(MyClass.MemberwiseClone, dbid)
                     End Function
End Class


End Namespace
