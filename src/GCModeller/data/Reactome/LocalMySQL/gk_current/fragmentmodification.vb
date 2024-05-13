#Region "Microsoft.VisualBasic::897023fce725a33e48db5ae293246005, data\Reactome\LocalMySQL\gk_current\fragmentmodification.vb"

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

    '   Total Lines: 158
    '    Code Lines: 78
    ' Comment Lines: 58
    '   Blank Lines: 22
    '     File Size: 7.12 KB


    ' Class fragmentmodification
    ' 
    '     Properties: DB_ID, endPositionInReferenceSequence, startPositionInReferenceSequence
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
''' DROP TABLE IF EXISTS `fragmentmodification`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `fragmentmodification` (
'''   `DB_ID` int(10) unsigned NOT NULL,
'''   `endPositionInReferenceSequence` int(10) DEFAULT NULL,
'''   `startPositionInReferenceSequence` int(10) DEFAULT NULL,
'''   PRIMARY KEY (`DB_ID`),
'''   KEY `endPositionInReferenceSequence` (`endPositionInReferenceSequence`),
'''   KEY `startPositionInReferenceSequence` (`startPositionInReferenceSequence`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("fragmentmodification", Database:="gk_current", SchemaSQL:="
CREATE TABLE `fragmentmodification` (
  `DB_ID` int(10) unsigned NOT NULL,
  `endPositionInReferenceSequence` int(10) DEFAULT NULL,
  `startPositionInReferenceSequence` int(10) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `endPositionInReferenceSequence` (`endPositionInReferenceSequence`),
  KEY `startPositionInReferenceSequence` (`startPositionInReferenceSequence`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class fragmentmodification: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="DB_ID"), XmlAttribute> Public Property DB_ID As Long
    <DatabaseField("endPositionInReferenceSequence"), DataType(MySqlDbType.Int64, "10"), Column(Name:="endPositionInReferenceSequence")> Public Property endPositionInReferenceSequence As Long
    <DatabaseField("startPositionInReferenceSequence"), DataType(MySqlDbType.Int64, "10"), Column(Name:="startPositionInReferenceSequence")> Public Property startPositionInReferenceSequence As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `fragmentmodification` (`DB_ID`, `endPositionInReferenceSequence`, `startPositionInReferenceSequence`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `fragmentmodification` (`DB_ID`, `endPositionInReferenceSequence`, `startPositionInReferenceSequence`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `fragmentmodification` (`DB_ID`, `endPositionInReferenceSequence`, `startPositionInReferenceSequence`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `fragmentmodification` (`DB_ID`, `endPositionInReferenceSequence`, `startPositionInReferenceSequence`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `fragmentmodification` WHERE `DB_ID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `fragmentmodification` SET `DB_ID`='{0}', `endPositionInReferenceSequence`='{1}', `startPositionInReferenceSequence`='{2}' WHERE `DB_ID` = '{3}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `fragmentmodification` WHERE `DB_ID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `fragmentmodification` (`DB_ID`, `endPositionInReferenceSequence`, `startPositionInReferenceSequence`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, endPositionInReferenceSequence, startPositionInReferenceSequence)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `fragmentmodification` (`DB_ID`, `endPositionInReferenceSequence`, `startPositionInReferenceSequence`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, DB_ID, endPositionInReferenceSequence, startPositionInReferenceSequence)
        Else
        Return String.Format(INSERT_SQL, DB_ID, endPositionInReferenceSequence, startPositionInReferenceSequence)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{DB_ID}', '{endPositionInReferenceSequence}', '{startPositionInReferenceSequence}')"
        Else
            Return $"('{DB_ID}', '{endPositionInReferenceSequence}', '{startPositionInReferenceSequence}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `fragmentmodification` (`DB_ID`, `endPositionInReferenceSequence`, `startPositionInReferenceSequence`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, endPositionInReferenceSequence, startPositionInReferenceSequence)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `fragmentmodification` (`DB_ID`, `endPositionInReferenceSequence`, `startPositionInReferenceSequence`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, DB_ID, endPositionInReferenceSequence, startPositionInReferenceSequence)
        Else
        Return String.Format(REPLACE_SQL, DB_ID, endPositionInReferenceSequence, startPositionInReferenceSequence)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `fragmentmodification` SET `DB_ID`='{0}', `endPositionInReferenceSequence`='{1}', `startPositionInReferenceSequence`='{2}' WHERE `DB_ID` = '{3}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, endPositionInReferenceSequence, startPositionInReferenceSequence, DB_ID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As fragmentmodification
                         Return DirectCast(MyClass.MemberwiseClone, fragmentmodification)
                     End Function
End Class


End Namespace
