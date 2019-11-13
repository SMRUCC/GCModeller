#Region "Microsoft.VisualBasic::29efe1dc4425eb1a54c0bfddba9ed256, data\ExternalDBSource\MetaCyc\MySQL\quanttypewidconfidenceindwid.vb"

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

    ' Class quanttypewidconfidenceindwid
    ' 
    '     Properties: ConfidenceIndicatorWID, QuantitationTypeWID
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
''' DROP TABLE IF EXISTS `quanttypewidconfidenceindwid`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `quanttypewidconfidenceindwid` (
'''   `QuantitationTypeWID` bigint(20) NOT NULL,
'''   `ConfidenceIndicatorWID` bigint(20) NOT NULL,
'''   KEY `FK_QuantTypeWIDConfidenceInd1` (`QuantitationTypeWID`),
'''   KEY `FK_QuantTypeWIDConfidenceInd2` (`ConfidenceIndicatorWID`),
'''   CONSTRAINT `FK_QuantTypeWIDConfidenceInd1` FOREIGN KEY (`QuantitationTypeWID`) REFERENCES `quantitationtype` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_QuantTypeWIDConfidenceInd2` FOREIGN KEY (`ConfidenceIndicatorWID`) REFERENCES `quantitationtype` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("quanttypewidconfidenceindwid", Database:="warehouse", SchemaSQL:="
CREATE TABLE `quanttypewidconfidenceindwid` (
  `QuantitationTypeWID` bigint(20) NOT NULL,
  `ConfidenceIndicatorWID` bigint(20) NOT NULL,
  KEY `FK_QuantTypeWIDConfidenceInd1` (`QuantitationTypeWID`),
  KEY `FK_QuantTypeWIDConfidenceInd2` (`ConfidenceIndicatorWID`),
  CONSTRAINT `FK_QuantTypeWIDConfidenceInd1` FOREIGN KEY (`QuantitationTypeWID`) REFERENCES `quantitationtype` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_QuantTypeWIDConfidenceInd2` FOREIGN KEY (`ConfidenceIndicatorWID`) REFERENCES `quantitationtype` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class quanttypewidconfidenceindwid: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("QuantitationTypeWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="QuantitationTypeWID"), XmlAttribute> Public Property QuantitationTypeWID As Long
    <DatabaseField("ConfidenceIndicatorWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="ConfidenceIndicatorWID")> Public Property ConfidenceIndicatorWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `quanttypewidconfidenceindwid` (`QuantitationTypeWID`, `ConfidenceIndicatorWID`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `quanttypewidconfidenceindwid` (`QuantitationTypeWID`, `ConfidenceIndicatorWID`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `quanttypewidconfidenceindwid` (`QuantitationTypeWID`, `ConfidenceIndicatorWID`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `quanttypewidconfidenceindwid` (`QuantitationTypeWID`, `ConfidenceIndicatorWID`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `quanttypewidconfidenceindwid` WHERE `QuantitationTypeWID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `quanttypewidconfidenceindwid` SET `QuantitationTypeWID`='{0}', `ConfidenceIndicatorWID`='{1}' WHERE `QuantitationTypeWID` = '{2}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `quanttypewidconfidenceindwid` WHERE `QuantitationTypeWID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, QuantitationTypeWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `quanttypewidconfidenceindwid` (`QuantitationTypeWID`, `ConfidenceIndicatorWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, QuantitationTypeWID, ConfidenceIndicatorWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `quanttypewidconfidenceindwid` (`QuantitationTypeWID`, `ConfidenceIndicatorWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, QuantitationTypeWID, ConfidenceIndicatorWID)
        Else
        Return String.Format(INSERT_SQL, QuantitationTypeWID, ConfidenceIndicatorWID)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{QuantitationTypeWID}', '{ConfidenceIndicatorWID}')"
        Else
            Return $"('{QuantitationTypeWID}', '{ConfidenceIndicatorWID}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `quanttypewidconfidenceindwid` (`QuantitationTypeWID`, `ConfidenceIndicatorWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, QuantitationTypeWID, ConfidenceIndicatorWID)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `quanttypewidconfidenceindwid` (`QuantitationTypeWID`, `ConfidenceIndicatorWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, QuantitationTypeWID, ConfidenceIndicatorWID)
        Else
        Return String.Format(REPLACE_SQL, QuantitationTypeWID, ConfidenceIndicatorWID)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `quanttypewidconfidenceindwid` SET `QuantitationTypeWID`='{0}', `ConfidenceIndicatorWID`='{1}' WHERE `QuantitationTypeWID` = '{2}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, QuantitationTypeWID, ConfidenceIndicatorWID, QuantitationTypeWID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As quanttypewidconfidenceindwid
                         Return DirectCast(MyClass.MemberwiseClone, quanttypewidconfidenceindwid)
                     End Function
End Class


End Namespace
