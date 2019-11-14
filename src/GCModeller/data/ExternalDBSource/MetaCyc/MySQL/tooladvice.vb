#Region "Microsoft.VisualBasic::c522d93fb7c0733a4689d9e3be3def00, data\ExternalDBSource\MetaCyc\MySQL\tooladvice.vb"

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

    ' Class tooladvice
    ' 
    '     Properties: Advice, OtherWID, ToolName
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
''' DROP TABLE IF EXISTS `tooladvice`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `tooladvice` (
'''   `OtherWID` bigint(20) NOT NULL,
'''   `ToolName` varchar(50) NOT NULL,
'''   `Advice` longtext
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("tooladvice", Database:="warehouse", SchemaSQL:="
CREATE TABLE `tooladvice` (
  `OtherWID` bigint(20) NOT NULL,
  `ToolName` varchar(50) NOT NULL,
  `Advice` longtext
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class tooladvice: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("OtherWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="OtherWID")> Public Property OtherWID As Long
    <DatabaseField("ToolName"), NotNull, DataType(MySqlDbType.VarChar, "50"), Column(Name:="ToolName")> Public Property ToolName As String
    <DatabaseField("Advice"), DataType(MySqlDbType.Text), Column(Name:="Advice")> Public Property Advice As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `tooladvice` (`OtherWID`, `ToolName`, `Advice`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `tooladvice` (`OtherWID`, `ToolName`, `Advice`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `tooladvice` (`OtherWID`, `ToolName`, `Advice`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `tooladvice` (`OtherWID`, `ToolName`, `Advice`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `tooladvice` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `tooladvice` SET `OtherWID`='{0}', `ToolName`='{1}', `Advice`='{2}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `tooladvice` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `tooladvice` (`OtherWID`, `ToolName`, `Advice`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, OtherWID, ToolName, Advice)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `tooladvice` (`OtherWID`, `ToolName`, `Advice`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, OtherWID, ToolName, Advice)
        Else
        Return String.Format(INSERT_SQL, OtherWID, ToolName, Advice)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{OtherWID}', '{ToolName}', '{Advice}')"
        Else
            Return $"('{OtherWID}', '{ToolName}', '{Advice}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `tooladvice` (`OtherWID`, `ToolName`, `Advice`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, OtherWID, ToolName, Advice)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `tooladvice` (`OtherWID`, `ToolName`, `Advice`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, OtherWID, ToolName, Advice)
        Else
        Return String.Format(REPLACE_SQL, OtherWID, ToolName, Advice)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `tooladvice` SET `OtherWID`='{0}', `ToolName`='{1}', `Advice`='{2}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As tooladvice
                         Return DirectCast(MyClass.MemberwiseClone, tooladvice)
                     End Function
End Class


End Namespace
