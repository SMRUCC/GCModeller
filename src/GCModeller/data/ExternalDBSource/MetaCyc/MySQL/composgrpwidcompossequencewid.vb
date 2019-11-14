#Region "Microsoft.VisualBasic::6ca24a4c5fe70dd94ea04e3707873dff, data\ExternalDBSource\MetaCyc\MySQL\composgrpwidcompossequencewid.vb"

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

    ' Class composgrpwidcompossequencewid
    ' 
    '     Properties: CompositeGroupWID, CompositeSequenceWID
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
''' DROP TABLE IF EXISTS `composgrpwidcompossequencewid`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `composgrpwidcompossequencewid` (
'''   `CompositeGroupWID` bigint(20) NOT NULL,
'''   `CompositeSequenceWID` bigint(20) NOT NULL,
'''   KEY `FK_ComposGrpWIDComposSequenc1` (`CompositeGroupWID`),
'''   KEY `FK_ComposGrpWIDComposSequenc2` (`CompositeSequenceWID`),
'''   CONSTRAINT `FK_ComposGrpWIDComposSequenc1` FOREIGN KEY (`CompositeGroupWID`) REFERENCES `designelementgroup` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_ComposGrpWIDComposSequenc2` FOREIGN KEY (`CompositeSequenceWID`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("composgrpwidcompossequencewid", Database:="warehouse", SchemaSQL:="
CREATE TABLE `composgrpwidcompossequencewid` (
  `CompositeGroupWID` bigint(20) NOT NULL,
  `CompositeSequenceWID` bigint(20) NOT NULL,
  KEY `FK_ComposGrpWIDComposSequenc1` (`CompositeGroupWID`),
  KEY `FK_ComposGrpWIDComposSequenc2` (`CompositeSequenceWID`),
  CONSTRAINT `FK_ComposGrpWIDComposSequenc1` FOREIGN KEY (`CompositeGroupWID`) REFERENCES `designelementgroup` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_ComposGrpWIDComposSequenc2` FOREIGN KEY (`CompositeSequenceWID`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class composgrpwidcompossequencewid: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("CompositeGroupWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="CompositeGroupWID"), XmlAttribute> Public Property CompositeGroupWID As Long
    <DatabaseField("CompositeSequenceWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="CompositeSequenceWID")> Public Property CompositeSequenceWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `composgrpwidcompossequencewid` (`CompositeGroupWID`, `CompositeSequenceWID`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `composgrpwidcompossequencewid` (`CompositeGroupWID`, `CompositeSequenceWID`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `composgrpwidcompossequencewid` (`CompositeGroupWID`, `CompositeSequenceWID`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `composgrpwidcompossequencewid` (`CompositeGroupWID`, `CompositeSequenceWID`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `composgrpwidcompossequencewid` WHERE `CompositeGroupWID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `composgrpwidcompossequencewid` SET `CompositeGroupWID`='{0}', `CompositeSequenceWID`='{1}' WHERE `CompositeGroupWID` = '{2}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `composgrpwidcompossequencewid` WHERE `CompositeGroupWID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, CompositeGroupWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `composgrpwidcompossequencewid` (`CompositeGroupWID`, `CompositeSequenceWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, CompositeGroupWID, CompositeSequenceWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `composgrpwidcompossequencewid` (`CompositeGroupWID`, `CompositeSequenceWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, CompositeGroupWID, CompositeSequenceWID)
        Else
        Return String.Format(INSERT_SQL, CompositeGroupWID, CompositeSequenceWID)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{CompositeGroupWID}', '{CompositeSequenceWID}')"
        Else
            Return $"('{CompositeGroupWID}', '{CompositeSequenceWID}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `composgrpwidcompossequencewid` (`CompositeGroupWID`, `CompositeSequenceWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, CompositeGroupWID, CompositeSequenceWID)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `composgrpwidcompossequencewid` (`CompositeGroupWID`, `CompositeSequenceWID`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, CompositeGroupWID, CompositeSequenceWID)
        Else
        Return String.Format(REPLACE_SQL, CompositeGroupWID, CompositeSequenceWID)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `composgrpwidcompossequencewid` SET `CompositeGroupWID`='{0}', `CompositeSequenceWID`='{1}' WHERE `CompositeGroupWID` = '{2}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, CompositeGroupWID, CompositeSequenceWID, CompositeGroupWID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As composgrpwidcompossequencewid
                         Return DirectCast(MyClass.MemberwiseClone, composgrpwidcompossequencewid)
                     End Function
End Class


End Namespace
