#Region "Microsoft.VisualBasic::c49bb8df4271b7362687577ce496feea, data\ExternalDBSource\MetaCyc\MySQL\pathwaylink.vb"

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

    ' Class pathwaylink
    ' 
    '     Properties: ChemicalWID, Pathway1WID, Pathway2WID
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
''' DROP TABLE IF EXISTS `pathwaylink`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `pathwaylink` (
'''   `Pathway1WID` bigint(20) NOT NULL,
'''   `Pathway2WID` bigint(20) NOT NULL,
'''   `ChemicalWID` bigint(20) NOT NULL,
'''   KEY `FK_PathwayLink1` (`Pathway1WID`),
'''   KEY `FK_PathwayLink2` (`Pathway2WID`),
'''   KEY `FK_PathwayLink3` (`ChemicalWID`),
'''   CONSTRAINT `FK_PathwayLink1` FOREIGN KEY (`Pathway1WID`) REFERENCES `pathway` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_PathwayLink2` FOREIGN KEY (`Pathway2WID`) REFERENCES `pathway` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_PathwayLink3` FOREIGN KEY (`ChemicalWID`) REFERENCES `chemical` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("pathwaylink", Database:="warehouse", SchemaSQL:="
CREATE TABLE `pathwaylink` (
  `Pathway1WID` bigint(20) NOT NULL,
  `Pathway2WID` bigint(20) NOT NULL,
  `ChemicalWID` bigint(20) NOT NULL,
  KEY `FK_PathwayLink1` (`Pathway1WID`),
  KEY `FK_PathwayLink2` (`Pathway2WID`),
  KEY `FK_PathwayLink3` (`ChemicalWID`),
  CONSTRAINT `FK_PathwayLink1` FOREIGN KEY (`Pathway1WID`) REFERENCES `pathway` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_PathwayLink2` FOREIGN KEY (`Pathway2WID`) REFERENCES `pathway` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_PathwayLink3` FOREIGN KEY (`ChemicalWID`) REFERENCES `chemical` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class pathwaylink: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("Pathway1WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="Pathway1WID"), XmlAttribute> Public Property Pathway1WID As Long
    <DatabaseField("Pathway2WID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="Pathway2WID")> Public Property Pathway2WID As Long
    <DatabaseField("ChemicalWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="ChemicalWID")> Public Property ChemicalWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `pathwaylink` (`Pathway1WID`, `Pathway2WID`, `ChemicalWID`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `pathwaylink` (`Pathway1WID`, `Pathway2WID`, `ChemicalWID`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `pathwaylink` (`Pathway1WID`, `Pathway2WID`, `ChemicalWID`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `pathwaylink` (`Pathway1WID`, `Pathway2WID`, `ChemicalWID`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `pathwaylink` WHERE `Pathway1WID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `pathwaylink` SET `Pathway1WID`='{0}', `Pathway2WID`='{1}', `ChemicalWID`='{2}' WHERE `Pathway1WID` = '{3}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `pathwaylink` WHERE `Pathway1WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, Pathway1WID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `pathwaylink` (`Pathway1WID`, `Pathway2WID`, `ChemicalWID`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, Pathway1WID, Pathway2WID, ChemicalWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `pathwaylink` (`Pathway1WID`, `Pathway2WID`, `ChemicalWID`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, Pathway1WID, Pathway2WID, ChemicalWID)
        Else
        Return String.Format(INSERT_SQL, Pathway1WID, Pathway2WID, ChemicalWID)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{Pathway1WID}', '{Pathway2WID}', '{ChemicalWID}')"
        Else
            Return $"('{Pathway1WID}', '{Pathway2WID}', '{ChemicalWID}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `pathwaylink` (`Pathway1WID`, `Pathway2WID`, `ChemicalWID`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, Pathway1WID, Pathway2WID, ChemicalWID)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `pathwaylink` (`Pathway1WID`, `Pathway2WID`, `ChemicalWID`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, Pathway1WID, Pathway2WID, ChemicalWID)
        Else
        Return String.Format(REPLACE_SQL, Pathway1WID, Pathway2WID, ChemicalWID)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `pathwaylink` SET `Pathway1WID`='{0}', `Pathway2WID`='{1}', `ChemicalWID`='{2}' WHERE `Pathway1WID` = '{3}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, Pathway1WID, Pathway2WID, ChemicalWID, Pathway1WID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As pathwaylink
                         Return DirectCast(MyClass.MemberwiseClone, pathwaylink)
                     End Function
End Class


End Namespace
