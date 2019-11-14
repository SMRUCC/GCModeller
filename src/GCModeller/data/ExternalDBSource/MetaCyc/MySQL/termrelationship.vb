#Region "Microsoft.VisualBasic::a7ca8546f67bdf939623664cddaeae13, data\ExternalDBSource\MetaCyc\MySQL\termrelationship.vb"

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

    ' Class termrelationship
    ' 
    '     Properties: RelatedTermWID, Relationship, TermWID
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
''' DROP TABLE IF EXISTS `termrelationship`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `termrelationship` (
'''   `TermWID` bigint(20) NOT NULL,
'''   `RelatedTermWID` bigint(20) NOT NULL,
'''   `Relationship` varchar(10) NOT NULL,
'''   KEY `FK_TermRelationship1` (`TermWID`),
'''   KEY `FK_TermRelationship2` (`RelatedTermWID`),
'''   CONSTRAINT `FK_TermRelationship1` FOREIGN KEY (`TermWID`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_TermRelationship2` FOREIGN KEY (`RelatedTermWID`) REFERENCES `term` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("termrelationship", Database:="warehouse", SchemaSQL:="
CREATE TABLE `termrelationship` (
  `TermWID` bigint(20) NOT NULL,
  `RelatedTermWID` bigint(20) NOT NULL,
  `Relationship` varchar(10) NOT NULL,
  KEY `FK_TermRelationship1` (`TermWID`),
  KEY `FK_TermRelationship2` (`RelatedTermWID`),
  CONSTRAINT `FK_TermRelationship1` FOREIGN KEY (`TermWID`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_TermRelationship2` FOREIGN KEY (`RelatedTermWID`) REFERENCES `term` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class termrelationship: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("TermWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="TermWID"), XmlAttribute> Public Property TermWID As Long
    <DatabaseField("RelatedTermWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="RelatedTermWID")> Public Property RelatedTermWID As Long
    <DatabaseField("Relationship"), NotNull, DataType(MySqlDbType.VarChar, "10"), Column(Name:="Relationship")> Public Property Relationship As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `termrelationship` (`TermWID`, `RelatedTermWID`, `Relationship`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `termrelationship` (`TermWID`, `RelatedTermWID`, `Relationship`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `termrelationship` (`TermWID`, `RelatedTermWID`, `Relationship`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `termrelationship` (`TermWID`, `RelatedTermWID`, `Relationship`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `termrelationship` WHERE `TermWID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `termrelationship` SET `TermWID`='{0}', `RelatedTermWID`='{1}', `Relationship`='{2}' WHERE `TermWID` = '{3}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `termrelationship` WHERE `TermWID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, TermWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `termrelationship` (`TermWID`, `RelatedTermWID`, `Relationship`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, TermWID, RelatedTermWID, Relationship)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `termrelationship` (`TermWID`, `RelatedTermWID`, `Relationship`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, TermWID, RelatedTermWID, Relationship)
        Else
        Return String.Format(INSERT_SQL, TermWID, RelatedTermWID, Relationship)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{TermWID}', '{RelatedTermWID}', '{Relationship}')"
        Else
            Return $"('{TermWID}', '{RelatedTermWID}', '{Relationship}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `termrelationship` (`TermWID`, `RelatedTermWID`, `Relationship`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, TermWID, RelatedTermWID, Relationship)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `termrelationship` (`TermWID`, `RelatedTermWID`, `Relationship`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, TermWID, RelatedTermWID, Relationship)
        Else
        Return String.Format(REPLACE_SQL, TermWID, RelatedTermWID, Relationship)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `termrelationship` SET `TermWID`='{0}', `RelatedTermWID`='{1}', `Relationship`='{2}' WHERE `TermWID` = '{3}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, TermWID, RelatedTermWID, Relationship, TermWID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As termrelationship
                         Return DirectCast(MyClass.MemberwiseClone, termrelationship)
                     End Function
End Class


End Namespace
