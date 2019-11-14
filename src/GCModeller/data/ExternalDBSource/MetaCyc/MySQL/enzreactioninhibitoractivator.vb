#Region "Microsoft.VisualBasic::bf477f0cc1dd47baad39680ddcffd691, data\ExternalDBSource\MetaCyc\MySQL\enzreactioninhibitoractivator.vb"

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

    ' Class enzreactioninhibitoractivator
    ' 
    '     Properties: CompoundWID, EnzymaticReactionWID, InhibitOrActivate, Mechanism, PhysioRelevant
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
''' DROP TABLE IF EXISTS `enzreactioninhibitoractivator`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `enzreactioninhibitoractivator` (
'''   `EnzymaticReactionWID` bigint(20) NOT NULL,
'''   `CompoundWID` bigint(20) NOT NULL,
'''   `InhibitOrActivate` char(1) DEFAULT NULL,
'''   `Mechanism` char(1) DEFAULT NULL,
'''   `PhysioRelevant` char(1) DEFAULT NULL,
'''   KEY `FK_EnzReactionIA1` (`EnzymaticReactionWID`),
'''   CONSTRAINT `FK_EnzReactionIA1` FOREIGN KEY (`EnzymaticReactionWID`) REFERENCES `enzymaticreaction` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("enzreactioninhibitoractivator", Database:="warehouse", SchemaSQL:="
CREATE TABLE `enzreactioninhibitoractivator` (
  `EnzymaticReactionWID` bigint(20) NOT NULL,
  `CompoundWID` bigint(20) NOT NULL,
  `InhibitOrActivate` char(1) DEFAULT NULL,
  `Mechanism` char(1) DEFAULT NULL,
  `PhysioRelevant` char(1) DEFAULT NULL,
  KEY `FK_EnzReactionIA1` (`EnzymaticReactionWID`),
  CONSTRAINT `FK_EnzReactionIA1` FOREIGN KEY (`EnzymaticReactionWID`) REFERENCES `enzymaticreaction` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class enzreactioninhibitoractivator: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("EnzymaticReactionWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="EnzymaticReactionWID"), XmlAttribute> Public Property EnzymaticReactionWID As Long
    <DatabaseField("CompoundWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="CompoundWID")> Public Property CompoundWID As Long
    <DatabaseField("InhibitOrActivate"), DataType(MySqlDbType.VarChar, "1"), Column(Name:="InhibitOrActivate")> Public Property InhibitOrActivate As String
    <DatabaseField("Mechanism"), DataType(MySqlDbType.VarChar, "1"), Column(Name:="Mechanism")> Public Property Mechanism As String
    <DatabaseField("PhysioRelevant"), DataType(MySqlDbType.VarChar, "1"), Column(Name:="PhysioRelevant")> Public Property PhysioRelevant As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `enzreactioninhibitoractivator` (`EnzymaticReactionWID`, `CompoundWID`, `InhibitOrActivate`, `Mechanism`, `PhysioRelevant`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `enzreactioninhibitoractivator` (`EnzymaticReactionWID`, `CompoundWID`, `InhibitOrActivate`, `Mechanism`, `PhysioRelevant`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `enzreactioninhibitoractivator` (`EnzymaticReactionWID`, `CompoundWID`, `InhibitOrActivate`, `Mechanism`, `PhysioRelevant`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `enzreactioninhibitoractivator` (`EnzymaticReactionWID`, `CompoundWID`, `InhibitOrActivate`, `Mechanism`, `PhysioRelevant`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `enzreactioninhibitoractivator` WHERE `EnzymaticReactionWID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `enzreactioninhibitoractivator` SET `EnzymaticReactionWID`='{0}', `CompoundWID`='{1}', `InhibitOrActivate`='{2}', `Mechanism`='{3}', `PhysioRelevant`='{4}' WHERE `EnzymaticReactionWID` = '{5}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `enzreactioninhibitoractivator` WHERE `EnzymaticReactionWID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, EnzymaticReactionWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `enzreactioninhibitoractivator` (`EnzymaticReactionWID`, `CompoundWID`, `InhibitOrActivate`, `Mechanism`, `PhysioRelevant`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, EnzymaticReactionWID, CompoundWID, InhibitOrActivate, Mechanism, PhysioRelevant)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `enzreactioninhibitoractivator` (`EnzymaticReactionWID`, `CompoundWID`, `InhibitOrActivate`, `Mechanism`, `PhysioRelevant`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, EnzymaticReactionWID, CompoundWID, InhibitOrActivate, Mechanism, PhysioRelevant)
        Else
        Return String.Format(INSERT_SQL, EnzymaticReactionWID, CompoundWID, InhibitOrActivate, Mechanism, PhysioRelevant)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{EnzymaticReactionWID}', '{CompoundWID}', '{InhibitOrActivate}', '{Mechanism}', '{PhysioRelevant}')"
        Else
            Return $"('{EnzymaticReactionWID}', '{CompoundWID}', '{InhibitOrActivate}', '{Mechanism}', '{PhysioRelevant}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `enzreactioninhibitoractivator` (`EnzymaticReactionWID`, `CompoundWID`, `InhibitOrActivate`, `Mechanism`, `PhysioRelevant`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, EnzymaticReactionWID, CompoundWID, InhibitOrActivate, Mechanism, PhysioRelevant)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `enzreactioninhibitoractivator` (`EnzymaticReactionWID`, `CompoundWID`, `InhibitOrActivate`, `Mechanism`, `PhysioRelevant`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, EnzymaticReactionWID, CompoundWID, InhibitOrActivate, Mechanism, PhysioRelevant)
        Else
        Return String.Format(REPLACE_SQL, EnzymaticReactionWID, CompoundWID, InhibitOrActivate, Mechanism, PhysioRelevant)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `enzreactioninhibitoractivator` SET `EnzymaticReactionWID`='{0}', `CompoundWID`='{1}', `InhibitOrActivate`='{2}', `Mechanism`='{3}', `PhysioRelevant`='{4}' WHERE `EnzymaticReactionWID` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, EnzymaticReactionWID, CompoundWID, InhibitOrActivate, Mechanism, PhysioRelevant, EnzymaticReactionWID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As enzreactioninhibitoractivator
                         Return DirectCast(MyClass.MemberwiseClone, enzreactioninhibitoractivator)
                     End Function
End Class


End Namespace
