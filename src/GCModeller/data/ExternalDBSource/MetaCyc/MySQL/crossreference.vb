#Region "Microsoft.VisualBasic::a0504b1208df00576895c58a06344c57, data\ExternalDBSource\MetaCyc\MySQL\crossreference.vb"

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

    ' Class crossreference
    ' 
    '     Properties: CrossWID, DatabaseName, OtherWID, Relationship, Type
    '                 Version, XID
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
''' DROP TABLE IF EXISTS `crossreference`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `crossreference` (
'''   `OtherWID` bigint(20) NOT NULL,
'''   `CrossWID` bigint(20) DEFAULT NULL,
'''   `XID` varchar(50) DEFAULT NULL,
'''   `Type` varchar(20) DEFAULT NULL,
'''   `Version` varchar(10) DEFAULT NULL,
'''   `Relationship` varchar(50) DEFAULT NULL,
'''   `DatabaseName` varchar(255) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("crossreference", Database:="warehouse", SchemaSQL:="
CREATE TABLE `crossreference` (
  `OtherWID` bigint(20) NOT NULL,
  `CrossWID` bigint(20) DEFAULT NULL,
  `XID` varchar(50) DEFAULT NULL,
  `Type` varchar(20) DEFAULT NULL,
  `Version` varchar(10) DEFAULT NULL,
  `Relationship` varchar(50) DEFAULT NULL,
  `DatabaseName` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class crossreference: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("OtherWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="OtherWID")> Public Property OtherWID As Long
    <DatabaseField("CrossWID"), DataType(MySqlDbType.Int64, "20"), Column(Name:="CrossWID")> Public Property CrossWID As Long
    <DatabaseField("XID"), DataType(MySqlDbType.VarChar, "50"), Column(Name:="XID")> Public Property XID As String
    <DatabaseField("Type"), DataType(MySqlDbType.VarChar, "20"), Column(Name:="Type")> Public Property Type As String
    <DatabaseField("Version"), DataType(MySqlDbType.VarChar, "10"), Column(Name:="Version")> Public Property Version As String
    <DatabaseField("Relationship"), DataType(MySqlDbType.VarChar, "50"), Column(Name:="Relationship")> Public Property Relationship As String
    <DatabaseField("DatabaseName"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="DatabaseName")> Public Property DatabaseName As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `crossreference` (`OtherWID`, `CrossWID`, `XID`, `Type`, `Version`, `Relationship`, `DatabaseName`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `crossreference` (`OtherWID`, `CrossWID`, `XID`, `Type`, `Version`, `Relationship`, `DatabaseName`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `crossreference` (`OtherWID`, `CrossWID`, `XID`, `Type`, `Version`, `Relationship`, `DatabaseName`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `crossreference` (`OtherWID`, `CrossWID`, `XID`, `Type`, `Version`, `Relationship`, `DatabaseName`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `crossreference` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `crossreference` SET `OtherWID`='{0}', `CrossWID`='{1}', `XID`='{2}', `Type`='{3}', `Version`='{4}', `Relationship`='{5}', `DatabaseName`='{6}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `crossreference` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `crossreference` (`OtherWID`, `CrossWID`, `XID`, `Type`, `Version`, `Relationship`, `DatabaseName`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, OtherWID, CrossWID, XID, Type, Version, Relationship, DatabaseName)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `crossreference` (`OtherWID`, `CrossWID`, `XID`, `Type`, `Version`, `Relationship`, `DatabaseName`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, OtherWID, CrossWID, XID, Type, Version, Relationship, DatabaseName)
        Else
        Return String.Format(INSERT_SQL, OtherWID, CrossWID, XID, Type, Version, Relationship, DatabaseName)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{OtherWID}', '{CrossWID}', '{XID}', '{Type}', '{Version}', '{Relationship}', '{DatabaseName}')"
        Else
            Return $"('{OtherWID}', '{CrossWID}', '{XID}', '{Type}', '{Version}', '{Relationship}', '{DatabaseName}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `crossreference` (`OtherWID`, `CrossWID`, `XID`, `Type`, `Version`, `Relationship`, `DatabaseName`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, OtherWID, CrossWID, XID, Type, Version, Relationship, DatabaseName)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `crossreference` (`OtherWID`, `CrossWID`, `XID`, `Type`, `Version`, `Relationship`, `DatabaseName`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, OtherWID, CrossWID, XID, Type, Version, Relationship, DatabaseName)
        Else
        Return String.Format(REPLACE_SQL, OtherWID, CrossWID, XID, Type, Version, Relationship, DatabaseName)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `crossreference` SET `OtherWID`='{0}', `CrossWID`='{1}', `XID`='{2}', `Type`='{3}', `Version`='{4}', `Relationship`='{5}', `DatabaseName`='{6}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As crossreference
                         Return DirectCast(MyClass.MemberwiseClone, crossreference)
                     End Function
End Class


End Namespace
