#Region "Microsoft.VisualBasic::15c2fd9dbea720c453487c47c6985059, data\ExternalDBSource\MetaCyc\MySQL\enzreactioncofactor.vb"

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

    ' Class enzreactioncofactor
    ' 
    '     Properties: ChemicalWID, EnzymaticReactionWID, Prosthetic
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
''' DROP TABLE IF EXISTS `enzreactioncofactor`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `enzreactioncofactor` (
'''   `EnzymaticReactionWID` bigint(20) NOT NULL,
'''   `ChemicalWID` bigint(20) NOT NULL,
'''   `Prosthetic` char(1) DEFAULT NULL,
'''   KEY `FK_EnzReactionCofactor1` (`EnzymaticReactionWID`),
'''   KEY `FK_EnzReactionCofactor2` (`ChemicalWID`),
'''   CONSTRAINT `FK_EnzReactionCofactor1` FOREIGN KEY (`EnzymaticReactionWID`) REFERENCES `enzymaticreaction` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_EnzReactionCofactor2` FOREIGN KEY (`ChemicalWID`) REFERENCES `chemical` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("enzreactioncofactor", Database:="warehouse", SchemaSQL:="
CREATE TABLE `enzreactioncofactor` (
  `EnzymaticReactionWID` bigint(20) NOT NULL,
  `ChemicalWID` bigint(20) NOT NULL,
  `Prosthetic` char(1) DEFAULT NULL,
  KEY `FK_EnzReactionCofactor1` (`EnzymaticReactionWID`),
  KEY `FK_EnzReactionCofactor2` (`ChemicalWID`),
  CONSTRAINT `FK_EnzReactionCofactor1` FOREIGN KEY (`EnzymaticReactionWID`) REFERENCES `enzymaticreaction` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_EnzReactionCofactor2` FOREIGN KEY (`ChemicalWID`) REFERENCES `chemical` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class enzreactioncofactor: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("EnzymaticReactionWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="EnzymaticReactionWID"), XmlAttribute> Public Property EnzymaticReactionWID As Long
    <DatabaseField("ChemicalWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="ChemicalWID")> Public Property ChemicalWID As Long
    <DatabaseField("Prosthetic"), DataType(MySqlDbType.VarChar, "1"), Column(Name:="Prosthetic")> Public Property Prosthetic As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `enzreactioncofactor` (`EnzymaticReactionWID`, `ChemicalWID`, `Prosthetic`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `enzreactioncofactor` (`EnzymaticReactionWID`, `ChemicalWID`, `Prosthetic`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `enzreactioncofactor` (`EnzymaticReactionWID`, `ChemicalWID`, `Prosthetic`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `enzreactioncofactor` (`EnzymaticReactionWID`, `ChemicalWID`, `Prosthetic`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `enzreactioncofactor` WHERE `EnzymaticReactionWID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `enzreactioncofactor` SET `EnzymaticReactionWID`='{0}', `ChemicalWID`='{1}', `Prosthetic`='{2}' WHERE `EnzymaticReactionWID` = '{3}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `enzreactioncofactor` WHERE `EnzymaticReactionWID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, EnzymaticReactionWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `enzreactioncofactor` (`EnzymaticReactionWID`, `ChemicalWID`, `Prosthetic`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, EnzymaticReactionWID, ChemicalWID, Prosthetic)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `enzreactioncofactor` (`EnzymaticReactionWID`, `ChemicalWID`, `Prosthetic`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, EnzymaticReactionWID, ChemicalWID, Prosthetic)
        Else
        Return String.Format(INSERT_SQL, EnzymaticReactionWID, ChemicalWID, Prosthetic)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{EnzymaticReactionWID}', '{ChemicalWID}', '{Prosthetic}')"
        Else
            Return $"('{EnzymaticReactionWID}', '{ChemicalWID}', '{Prosthetic}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `enzreactioncofactor` (`EnzymaticReactionWID`, `ChemicalWID`, `Prosthetic`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, EnzymaticReactionWID, ChemicalWID, Prosthetic)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `enzreactioncofactor` (`EnzymaticReactionWID`, `ChemicalWID`, `Prosthetic`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, EnzymaticReactionWID, ChemicalWID, Prosthetic)
        Else
        Return String.Format(REPLACE_SQL, EnzymaticReactionWID, ChemicalWID, Prosthetic)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `enzreactioncofactor` SET `EnzymaticReactionWID`='{0}', `ChemicalWID`='{1}', `Prosthetic`='{2}' WHERE `EnzymaticReactionWID` = '{3}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, EnzymaticReactionWID, ChemicalWID, Prosthetic, EnzymaticReactionWID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As enzreactioncofactor
                         Return DirectCast(MyClass.MemberwiseClone, enzreactioncofactor)
                     End Function
End Class


End Namespace
