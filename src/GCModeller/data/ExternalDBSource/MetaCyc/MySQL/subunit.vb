#Region "Microsoft.VisualBasic::2c990ba362d8e5b44ff0e8ac3a16c90f, data\ExternalDBSource\MetaCyc\MySQL\subunit.vb"

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

    ' Class subunit
    ' 
    '     Properties: Coefficient, ComplexWID, SubunitWID
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
''' DROP TABLE IF EXISTS `subunit`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `subunit` (
'''   `ComplexWID` bigint(20) NOT NULL,
'''   `SubunitWID` bigint(20) NOT NULL,
'''   `Coefficient` smallint(6) DEFAULT NULL,
'''   KEY `FK_Subunit1` (`ComplexWID`),
'''   KEY `FK_Subunit2` (`SubunitWID`),
'''   CONSTRAINT `FK_Subunit1` FOREIGN KEY (`ComplexWID`) REFERENCES `protein` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Subunit2` FOREIGN KEY (`SubunitWID`) REFERENCES `protein` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("subunit", Database:="warehouse", SchemaSQL:="
CREATE TABLE `subunit` (
  `ComplexWID` bigint(20) NOT NULL,
  `SubunitWID` bigint(20) NOT NULL,
  `Coefficient` smallint(6) DEFAULT NULL,
  KEY `FK_Subunit1` (`ComplexWID`),
  KEY `FK_Subunit2` (`SubunitWID`),
  CONSTRAINT `FK_Subunit1` FOREIGN KEY (`ComplexWID`) REFERENCES `protein` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Subunit2` FOREIGN KEY (`SubunitWID`) REFERENCES `protein` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class subunit: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("ComplexWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="ComplexWID"), XmlAttribute> Public Property ComplexWID As Long
    <DatabaseField("SubunitWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="SubunitWID")> Public Property SubunitWID As Long
    <DatabaseField("Coefficient"), DataType(MySqlDbType.Int64, "6"), Column(Name:="Coefficient")> Public Property Coefficient As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `subunit` (`ComplexWID`, `SubunitWID`, `Coefficient`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `subunit` (`ComplexWID`, `SubunitWID`, `Coefficient`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `subunit` (`ComplexWID`, `SubunitWID`, `Coefficient`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `subunit` (`ComplexWID`, `SubunitWID`, `Coefficient`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `subunit` WHERE `ComplexWID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `subunit` SET `ComplexWID`='{0}', `SubunitWID`='{1}', `Coefficient`='{2}' WHERE `ComplexWID` = '{3}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `subunit` WHERE `ComplexWID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, ComplexWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `subunit` (`ComplexWID`, `SubunitWID`, `Coefficient`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, ComplexWID, SubunitWID, Coefficient)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `subunit` (`ComplexWID`, `SubunitWID`, `Coefficient`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, ComplexWID, SubunitWID, Coefficient)
        Else
        Return String.Format(INSERT_SQL, ComplexWID, SubunitWID, Coefficient)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{ComplexWID}', '{SubunitWID}', '{Coefficient}')"
        Else
            Return $"('{ComplexWID}', '{SubunitWID}', '{Coefficient}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `subunit` (`ComplexWID`, `SubunitWID`, `Coefficient`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, ComplexWID, SubunitWID, Coefficient)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `subunit` (`ComplexWID`, `SubunitWID`, `Coefficient`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, ComplexWID, SubunitWID, Coefficient)
        Else
        Return String.Format(REPLACE_SQL, ComplexWID, SubunitWID, Coefficient)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `subunit` SET `ComplexWID`='{0}', `SubunitWID`='{1}', `Coefficient`='{2}' WHERE `ComplexWID` = '{3}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, ComplexWID, SubunitWID, Coefficient, ComplexWID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As subunit
                         Return DirectCast(MyClass.MemberwiseClone, subunit)
                     End Function
End Class


End Namespace
