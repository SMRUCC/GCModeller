#Region "Microsoft.VisualBasic::2eabfd51aece6a70cdd5070c3b0e3c83, G:/GCModeller/src/GCModeller/data/Reactome//LocalMySQL/gk_current/event.vb"

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
    '     File Size: 10.44 KB


    ' Class [event]
    ' 
    '     Properties: _doRelease, DB_ID, definition, evidenceType, evidenceType_class
    '                 goBiologicalProcess, goBiologicalProcess_class, releaseDate, releaseStatus
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
''' DROP TABLE IF EXISTS `event`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `event` (
'''   `DB_ID` int(10) unsigned NOT NULL,
'''   `_doRelease` enum('TRUE','FALSE') DEFAULT NULL,
'''   `definition` text,
'''   `evidenceType` int(10) unsigned DEFAULT NULL,
'''   `evidenceType_class` varchar(64) DEFAULT NULL,
'''   `goBiologicalProcess` int(10) unsigned DEFAULT NULL,
'''   `goBiologicalProcess_class` varchar(64) DEFAULT NULL,
'''   `releaseDate` date DEFAULT NULL,
'''   `releaseStatus` text,
'''   PRIMARY KEY (`DB_ID`),
'''   KEY `_doRelease` (`_doRelease`),
'''   KEY `evidenceType` (`evidenceType`),
'''   KEY `goBiologicalProcess` (`goBiologicalProcess`),
'''   KEY `releaseDate` (`releaseDate`),
'''   FULLTEXT KEY `definition` (`definition`),
'''   FULLTEXT KEY `releaseStatus` (`releaseStatus`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("event", Database:="gk_current", SchemaSQL:="
CREATE TABLE `event` (
  `DB_ID` int(10) unsigned NOT NULL,
  `_doRelease` enum('TRUE','FALSE') DEFAULT NULL,
  `definition` text,
  `evidenceType` int(10) unsigned DEFAULT NULL,
  `evidenceType_class` varchar(64) DEFAULT NULL,
  `goBiologicalProcess` int(10) unsigned DEFAULT NULL,
  `goBiologicalProcess_class` varchar(64) DEFAULT NULL,
  `releaseDate` date DEFAULT NULL,
  `releaseStatus` text,
  PRIMARY KEY (`DB_ID`),
  KEY `_doRelease` (`_doRelease`),
  KEY `evidenceType` (`evidenceType`),
  KEY `goBiologicalProcess` (`goBiologicalProcess`),
  KEY `releaseDate` (`releaseDate`),
  FULLTEXT KEY `definition` (`definition`),
  FULLTEXT KEY `releaseStatus` (`releaseStatus`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class [event]: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="DB_ID"), XmlAttribute> Public Property DB_ID As Long
    <DatabaseField("_doRelease"), DataType(MySqlDbType.String), Column(Name:="_doRelease")> Public Property _doRelease As String
    <DatabaseField("definition"), DataType(MySqlDbType.Text), Column(Name:="definition")> Public Property definition As String
    <DatabaseField("evidenceType"), DataType(MySqlDbType.Int64, "10"), Column(Name:="evidenceType")> Public Property evidenceType As Long
    <DatabaseField("evidenceType_class"), DataType(MySqlDbType.VarChar, "64"), Column(Name:="evidenceType_class")> Public Property evidenceType_class As String
    <DatabaseField("goBiologicalProcess"), DataType(MySqlDbType.Int64, "10"), Column(Name:="goBiologicalProcess")> Public Property goBiologicalProcess As Long
    <DatabaseField("goBiologicalProcess_class"), DataType(MySqlDbType.VarChar, "64"), Column(Name:="goBiologicalProcess_class")> Public Property goBiologicalProcess_class As String
    <DatabaseField("releaseDate"), DataType(MySqlDbType.DateTime), Column(Name:="releaseDate")> Public Property releaseDate As Date
    <DatabaseField("releaseStatus"), DataType(MySqlDbType.Text), Column(Name:="releaseStatus")> Public Property releaseStatus As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `event` (`DB_ID`, `_doRelease`, `definition`, `evidenceType`, `evidenceType_class`, `goBiologicalProcess`, `goBiologicalProcess_class`, `releaseDate`, `releaseStatus`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `event` (`DB_ID`, `_doRelease`, `definition`, `evidenceType`, `evidenceType_class`, `goBiologicalProcess`, `goBiologicalProcess_class`, `releaseDate`, `releaseStatus`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `event` (`DB_ID`, `_doRelease`, `definition`, `evidenceType`, `evidenceType_class`, `goBiologicalProcess`, `goBiologicalProcess_class`, `releaseDate`, `releaseStatus`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `event` (`DB_ID`, `_doRelease`, `definition`, `evidenceType`, `evidenceType_class`, `goBiologicalProcess`, `goBiologicalProcess_class`, `releaseDate`, `releaseStatus`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `event` WHERE `DB_ID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `event` SET `DB_ID`='{0}', `_doRelease`='{1}', `definition`='{2}', `evidenceType`='{3}', `evidenceType_class`='{4}', `goBiologicalProcess`='{5}', `goBiologicalProcess_class`='{6}', `releaseDate`='{7}', `releaseStatus`='{8}' WHERE `DB_ID` = '{9}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `event` WHERE `DB_ID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `event` (`DB_ID`, `_doRelease`, `definition`, `evidenceType`, `evidenceType_class`, `goBiologicalProcess`, `goBiologicalProcess_class`, `releaseDate`, `releaseStatus`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, _doRelease, definition, evidenceType, evidenceType_class, goBiologicalProcess, goBiologicalProcess_class, MySqlScript.ToMySqlDateTimeString(releaseDate), releaseStatus)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `event` (`DB_ID`, `_doRelease`, `definition`, `evidenceType`, `evidenceType_class`, `goBiologicalProcess`, `goBiologicalProcess_class`, `releaseDate`, `releaseStatus`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, DB_ID, _doRelease, definition, evidenceType, evidenceType_class, goBiologicalProcess, goBiologicalProcess_class, MySqlScript.ToMySqlDateTimeString(releaseDate), releaseStatus)
        Else
        Return String.Format(INSERT_SQL, DB_ID, _doRelease, definition, evidenceType, evidenceType_class, goBiologicalProcess, goBiologicalProcess_class, MySqlScript.ToMySqlDateTimeString(releaseDate), releaseStatus)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{DB_ID}', '{_doRelease}', '{definition}', '{evidenceType}', '{evidenceType_class}', '{goBiologicalProcess}', '{goBiologicalProcess_class}', '{releaseDate}', '{releaseStatus}')"
        Else
            Return $"('{DB_ID}', '{_doRelease}', '{definition}', '{evidenceType}', '{evidenceType_class}', '{goBiologicalProcess}', '{goBiologicalProcess_class}', '{releaseDate}', '{releaseStatus}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `event` (`DB_ID`, `_doRelease`, `definition`, `evidenceType`, `evidenceType_class`, `goBiologicalProcess`, `goBiologicalProcess_class`, `releaseDate`, `releaseStatus`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, _doRelease, definition, evidenceType, evidenceType_class, goBiologicalProcess, goBiologicalProcess_class, MySqlScript.ToMySqlDateTimeString(releaseDate), releaseStatus)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `event` (`DB_ID`, `_doRelease`, `definition`, `evidenceType`, `evidenceType_class`, `goBiologicalProcess`, `goBiologicalProcess_class`, `releaseDate`, `releaseStatus`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, DB_ID, _doRelease, definition, evidenceType, evidenceType_class, goBiologicalProcess, goBiologicalProcess_class, MySqlScript.ToMySqlDateTimeString(releaseDate), releaseStatus)
        Else
        Return String.Format(REPLACE_SQL, DB_ID, _doRelease, definition, evidenceType, evidenceType_class, goBiologicalProcess, goBiologicalProcess_class, MySqlScript.ToMySqlDateTimeString(releaseDate), releaseStatus)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `event` SET `DB_ID`='{0}', `_doRelease`='{1}', `definition`='{2}', `evidenceType`='{3}', `evidenceType_class`='{4}', `goBiologicalProcess`='{5}', `goBiologicalProcess_class`='{6}', `releaseDate`='{7}', `releaseStatus`='{8}' WHERE `DB_ID` = '{9}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, _doRelease, definition, evidenceType, evidenceType_class, goBiologicalProcess, goBiologicalProcess_class, MySqlScript.ToMySqlDateTimeString(releaseDate), releaseStatus, DB_ID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As [event]
                         Return DirectCast(MyClass.MemberwiseClone, [event])
                     End Function
End Class


End Namespace
