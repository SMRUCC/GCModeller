#Region "Microsoft.VisualBasic::c808a217f3b1c0cf83f8f804c6482f2d, data\ExternalDBSource\MetaCyc\MySQL\tooladvice.vb"

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
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @3/16/2018 10:40:19 PM


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
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class tooladvice: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("OtherWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="OtherWID")> Public Property OtherWID As Long
    <DatabaseField("ToolName"), NotNull, DataType(MySqlDbType.VarChar, "50"), Column(Name:="ToolName")> Public Property ToolName As String
    <DatabaseField("Advice"), DataType(MySqlDbType.Text), Column(Name:="Advice")> Public Property Advice As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `tooladvice` (`OtherWID`, `ToolName`, `Advice`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `tooladvice` (`OtherWID`, `ToolName`, `Advice`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `tooladvice` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `tooladvice` SET `OtherWID`='{0}', `ToolName`='{1}', `Advice`='{2}' WHERE ;</SQL>
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
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{OtherWID}', '{ToolName}', '{Advice}')"
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
''' UPDATE `tooladvice` SET `OtherWID`='{0}', `ToolName`='{1}', `Advice`='{2}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
Public Function Clone() As tooladvice
                  Return DirectCast(MyClass.MemberwiseClone, tooladvice)
              End Function
End Class


End Namespace
