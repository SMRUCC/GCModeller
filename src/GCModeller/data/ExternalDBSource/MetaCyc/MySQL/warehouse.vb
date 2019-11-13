#Region "Microsoft.VisualBasic::940cfed691609bf3e15ecda7633d0363, data\ExternalDBSource\MetaCyc\MySQL\warehouse.vb"

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

    ' Class warehouse
    ' 
    '     Properties: Description, LoadDate, MaxReservedWID, MaxSpecialWID, Version
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
''' DROP TABLE IF EXISTS `warehouse`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `warehouse` (
'''   `Version` decimal(6,3) NOT NULL,
'''   `LoadDate` datetime NOT NULL,
'''   `MaxSpecialWID` bigint(20) NOT NULL,
'''   `MaxReservedWID` bigint(20) NOT NULL,
'''   `Description` text
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("warehouse", Database:="warehouse", SchemaSQL:="
CREATE TABLE `warehouse` (
  `Version` decimal(6,3) NOT NULL,
  `LoadDate` datetime NOT NULL,
  `MaxSpecialWID` bigint(20) NOT NULL,
  `MaxReservedWID` bigint(20) NOT NULL,
  `Description` text
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class warehouse: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("Version"), NotNull, DataType(MySqlDbType.Decimal), Column(Name:="Version")> Public Property Version As Decimal
    <DatabaseField("LoadDate"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="LoadDate")> Public Property LoadDate As Date
    <DatabaseField("MaxSpecialWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="MaxSpecialWID")> Public Property MaxSpecialWID As Long
    <DatabaseField("MaxReservedWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="MaxReservedWID")> Public Property MaxReservedWID As Long
    <DatabaseField("Description"), DataType(MySqlDbType.Text), Column(Name:="Description")> Public Property Description As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `warehouse` (`Version`, `LoadDate`, `MaxSpecialWID`, `MaxReservedWID`, `Description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `warehouse` (`Version`, `LoadDate`, `MaxSpecialWID`, `MaxReservedWID`, `Description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `warehouse` (`Version`, `LoadDate`, `MaxSpecialWID`, `MaxReservedWID`, `Description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `warehouse` (`Version`, `LoadDate`, `MaxSpecialWID`, `MaxReservedWID`, `Description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `warehouse` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `warehouse` SET `Version`='{0}', `LoadDate`='{1}', `MaxSpecialWID`='{2}', `MaxReservedWID`='{3}', `Description`='{4}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `warehouse` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `warehouse` (`Version`, `LoadDate`, `MaxSpecialWID`, `MaxReservedWID`, `Description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, Version, MySqlScript.ToMySqlDateTimeString(LoadDate), MaxSpecialWID, MaxReservedWID, Description)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `warehouse` (`Version`, `LoadDate`, `MaxSpecialWID`, `MaxReservedWID`, `Description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, Version, MySqlScript.ToMySqlDateTimeString(LoadDate), MaxSpecialWID, MaxReservedWID, Description)
        Else
        Return String.Format(INSERT_SQL, Version, MySqlScript.ToMySqlDateTimeString(LoadDate), MaxSpecialWID, MaxReservedWID, Description)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{Version}', '{LoadDate}', '{MaxSpecialWID}', '{MaxReservedWID}', '{Description}')"
        Else
            Return $"('{Version}', '{LoadDate}', '{MaxSpecialWID}', '{MaxReservedWID}', '{Description}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `warehouse` (`Version`, `LoadDate`, `MaxSpecialWID`, `MaxReservedWID`, `Description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, Version, MySqlScript.ToMySqlDateTimeString(LoadDate), MaxSpecialWID, MaxReservedWID, Description)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `warehouse` (`Version`, `LoadDate`, `MaxSpecialWID`, `MaxReservedWID`, `Description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, Version, MySqlScript.ToMySqlDateTimeString(LoadDate), MaxSpecialWID, MaxReservedWID, Description)
        Else
        Return String.Format(REPLACE_SQL, Version, MySqlScript.ToMySqlDateTimeString(LoadDate), MaxSpecialWID, MaxReservedWID, Description)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `warehouse` SET `Version`='{0}', `LoadDate`='{1}', `MaxSpecialWID`='{2}', `MaxReservedWID`='{3}', `Description`='{4}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As warehouse
                         Return DirectCast(MyClass.MemberwiseClone, warehouse)
                     End Function
End Class


End Namespace
