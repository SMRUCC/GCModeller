#Region "Microsoft.VisualBasic::d243469743eac4cfea2bf9f1555c860a, G:/GCModeller/src/GCModeller/data/Reactome//LocalMySQL/gk_current/_attributevaluebeforechange.vb"

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

    '   Total Lines: 153
    '    Code Lines: 75
    ' Comment Lines: 56
    '   Blank Lines: 22
    '     File Size: 5.61 KB


    ' Class _attributevaluebeforechange
    ' 
    '     Properties: changedAttribute, DB_ID
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
''' DROP TABLE IF EXISTS `_attributevaluebeforechange`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `_attributevaluebeforechange` (
'''   `DB_ID` int(10) unsigned NOT NULL,
'''   `changedAttribute` text,
'''   PRIMARY KEY (`DB_ID`),
'''   FULLTEXT KEY `changedAttribute` (`changedAttribute`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("_attributevaluebeforechange", Database:="gk_current", SchemaSQL:="
CREATE TABLE `_attributevaluebeforechange` (
  `DB_ID` int(10) unsigned NOT NULL,
  `changedAttribute` text,
  PRIMARY KEY (`DB_ID`),
  FULLTEXT KEY `changedAttribute` (`changedAttribute`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class _attributevaluebeforechange: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="DB_ID"), XmlAttribute> Public Property DB_ID As Long
    <DatabaseField("changedAttribute"), DataType(MySqlDbType.Text), Column(Name:="changedAttribute")> Public Property changedAttribute As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `_attributevaluebeforechange` (`DB_ID`, `changedAttribute`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `_attributevaluebeforechange` (`DB_ID`, `changedAttribute`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `_attributevaluebeforechange` (`DB_ID`, `changedAttribute`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `_attributevaluebeforechange` (`DB_ID`, `changedAttribute`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `_attributevaluebeforechange` WHERE `DB_ID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `_attributevaluebeforechange` SET `DB_ID`='{0}', `changedAttribute`='{1}' WHERE `DB_ID` = '{2}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `_attributevaluebeforechange` WHERE `DB_ID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `_attributevaluebeforechange` (`DB_ID`, `changedAttribute`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, changedAttribute)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `_attributevaluebeforechange` (`DB_ID`, `changedAttribute`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, DB_ID, changedAttribute)
        Else
        Return String.Format(INSERT_SQL, DB_ID, changedAttribute)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{DB_ID}', '{changedAttribute}')"
        Else
            Return $"('{DB_ID}', '{changedAttribute}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `_attributevaluebeforechange` (`DB_ID`, `changedAttribute`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, changedAttribute)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `_attributevaluebeforechange` (`DB_ID`, `changedAttribute`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, DB_ID, changedAttribute)
        Else
        Return String.Format(REPLACE_SQL, DB_ID, changedAttribute)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `_attributevaluebeforechange` SET `DB_ID`='{0}', `changedAttribute`='{1}' WHERE `DB_ID` = '{2}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, changedAttribute, DB_ID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As _attributevaluebeforechange
                         Return DirectCast(MyClass.MemberwiseClone, _attributevaluebeforechange)
                     End Function
End Class


End Namespace
