#Region "Microsoft.VisualBasic::1636117ff94a5899eb4002dff3f1e903, G:/GCModeller/src/GCModeller/data/Reactome//LocalMySQL/gk_current/reactioncoordinates.vb"

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


    ' Code Statistics:

    '   Total Lines: 184
    '    Code Lines: 94
    ' Comment Lines: 68
    '   Blank Lines: 22
    '     File Size: 9.83 KB


    ' Class reactioncoordinates
    ' 
    '     Properties: DB_ID, locatedEvent, locatedEvent_class, locationContext, locationContext_class
    '                 sourceX, sourceY, targetX, targetY
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

REM  Dump @2018/5/23 13:13:41


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace LocalMySQL.Tables.gk_current

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `reactioncoordinates`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `reactioncoordinates` (
'''   `DB_ID` int(10) unsigned NOT NULL,
'''   `locatedEvent` int(10) unsigned DEFAULT NULL,
'''   `locatedEvent_class` varchar(64) DEFAULT NULL,
'''   `locationContext` int(10) unsigned DEFAULT NULL,
'''   `locationContext_class` varchar(64) DEFAULT NULL,
'''   `sourceX` int(10) DEFAULT NULL,
'''   `sourceY` int(10) DEFAULT NULL,
'''   `targetX` int(10) DEFAULT NULL,
'''   `targetY` int(10) DEFAULT NULL,
'''   PRIMARY KEY (`DB_ID`),
'''   KEY `locatedEvent` (`locatedEvent`),
'''   KEY `locationContext` (`locationContext`),
'''   KEY `sourceX` (`sourceX`),
'''   KEY `sourceY` (`sourceY`),
'''   KEY `targetX` (`targetX`),
'''   KEY `targetY` (`targetY`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("reactioncoordinates", Database:="gk_current", SchemaSQL:="
CREATE TABLE `reactioncoordinates` (
  `DB_ID` int(10) unsigned NOT NULL,
  `locatedEvent` int(10) unsigned DEFAULT NULL,
  `locatedEvent_class` varchar(64) DEFAULT NULL,
  `locationContext` int(10) unsigned DEFAULT NULL,
  `locationContext_class` varchar(64) DEFAULT NULL,
  `sourceX` int(10) DEFAULT NULL,
  `sourceY` int(10) DEFAULT NULL,
  `targetX` int(10) DEFAULT NULL,
  `targetY` int(10) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `locatedEvent` (`locatedEvent`),
  KEY `locationContext` (`locationContext`),
  KEY `sourceX` (`sourceX`),
  KEY `sourceY` (`sourceY`),
  KEY `targetX` (`targetX`),
  KEY `targetY` (`targetY`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class reactioncoordinates: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="DB_ID"), XmlAttribute> Public Property DB_ID As Long
    <DatabaseField("locatedEvent"), DataType(MySqlDbType.Int64, "10"), Column(Name:="locatedEvent")> Public Property locatedEvent As Long
    <DatabaseField("locatedEvent_class"), DataType(MySqlDbType.VarChar, "64"), Column(Name:="locatedEvent_class")> Public Property locatedEvent_class As String
    <DatabaseField("locationContext"), DataType(MySqlDbType.Int64, "10"), Column(Name:="locationContext")> Public Property locationContext As Long
    <DatabaseField("locationContext_class"), DataType(MySqlDbType.VarChar, "64"), Column(Name:="locationContext_class")> Public Property locationContext_class As String
    <DatabaseField("sourceX"), DataType(MySqlDbType.Int64, "10"), Column(Name:="sourceX")> Public Property sourceX As Long
    <DatabaseField("sourceY"), DataType(MySqlDbType.Int64, "10"), Column(Name:="sourceY")> Public Property sourceY As Long
    <DatabaseField("targetX"), DataType(MySqlDbType.Int64, "10"), Column(Name:="targetX")> Public Property targetX As Long
    <DatabaseField("targetY"), DataType(MySqlDbType.Int64, "10"), Column(Name:="targetY")> Public Property targetY As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `reactioncoordinates` (`DB_ID`, `locatedEvent`, `locatedEvent_class`, `locationContext`, `locationContext_class`, `sourceX`, `sourceY`, `targetX`, `targetY`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `reactioncoordinates` (`DB_ID`, `locatedEvent`, `locatedEvent_class`, `locationContext`, `locationContext_class`, `sourceX`, `sourceY`, `targetX`, `targetY`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `reactioncoordinates` (`DB_ID`, `locatedEvent`, `locatedEvent_class`, `locationContext`, `locationContext_class`, `sourceX`, `sourceY`, `targetX`, `targetY`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `reactioncoordinates` (`DB_ID`, `locatedEvent`, `locatedEvent_class`, `locationContext`, `locationContext_class`, `sourceX`, `sourceY`, `targetX`, `targetY`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `reactioncoordinates` WHERE `DB_ID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `reactioncoordinates` SET `DB_ID`='{0}', `locatedEvent`='{1}', `locatedEvent_class`='{2}', `locationContext`='{3}', `locationContext_class`='{4}', `sourceX`='{5}', `sourceY`='{6}', `targetX`='{7}', `targetY`='{8}' WHERE `DB_ID` = '{9}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `reactioncoordinates` WHERE `DB_ID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `reactioncoordinates` (`DB_ID`, `locatedEvent`, `locatedEvent_class`, `locationContext`, `locationContext_class`, `sourceX`, `sourceY`, `targetX`, `targetY`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, locatedEvent, locatedEvent_class, locationContext, locationContext_class, sourceX, sourceY, targetX, targetY)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `reactioncoordinates` (`DB_ID`, `locatedEvent`, `locatedEvent_class`, `locationContext`, `locationContext_class`, `sourceX`, `sourceY`, `targetX`, `targetY`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, DB_ID, locatedEvent, locatedEvent_class, locationContext, locationContext_class, sourceX, sourceY, targetX, targetY)
        Else
        Return String.Format(INSERT_SQL, DB_ID, locatedEvent, locatedEvent_class, locationContext, locationContext_class, sourceX, sourceY, targetX, targetY)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{DB_ID}', '{locatedEvent}', '{locatedEvent_class}', '{locationContext}', '{locationContext_class}', '{sourceX}', '{sourceY}', '{targetX}', '{targetY}')"
        Else
            Return $"('{DB_ID}', '{locatedEvent}', '{locatedEvent_class}', '{locationContext}', '{locationContext_class}', '{sourceX}', '{sourceY}', '{targetX}', '{targetY}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `reactioncoordinates` (`DB_ID`, `locatedEvent`, `locatedEvent_class`, `locationContext`, `locationContext_class`, `sourceX`, `sourceY`, `targetX`, `targetY`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, locatedEvent, locatedEvent_class, locationContext, locationContext_class, sourceX, sourceY, targetX, targetY)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `reactioncoordinates` (`DB_ID`, `locatedEvent`, `locatedEvent_class`, `locationContext`, `locationContext_class`, `sourceX`, `sourceY`, `targetX`, `targetY`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, DB_ID, locatedEvent, locatedEvent_class, locationContext, locationContext_class, sourceX, sourceY, targetX, targetY)
        Else
        Return String.Format(REPLACE_SQL, DB_ID, locatedEvent, locatedEvent_class, locationContext, locationContext_class, sourceX, sourceY, targetX, targetY)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `reactioncoordinates` SET `DB_ID`='{0}', `locatedEvent`='{1}', `locatedEvent_class`='{2}', `locationContext`='{3}', `locationContext_class`='{4}', `sourceX`='{5}', `sourceY`='{6}', `targetX`='{7}', `targetY`='{8}' WHERE `DB_ID` = '{9}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, locatedEvent, locatedEvent_class, locationContext, locationContext_class, sourceX, sourceY, targetX, targetY, DB_ID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As reactioncoordinates
                         Return DirectCast(MyClass.MemberwiseClone, reactioncoordinates)
                     End Function
End Class


End Namespace
