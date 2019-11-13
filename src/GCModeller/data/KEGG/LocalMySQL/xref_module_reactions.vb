#Region "Microsoft.VisualBasic::cf735b312d6e33d81c73324977e45426, data\KEGG\LocalMySQL\xref_module_reactions.vb"

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

    ' Class xref_module_reactions
    ' 
    '     Properties: [module], KEGG, reaction
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

REM  Dump @2018/5/23 13:13:37


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace LocalMySQL

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `xref_module_reactions`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `xref_module_reactions` (
'''   `module` int(11) NOT NULL,
'''   `reaction` varchar(45) DEFAULT NULL,
'''   `KEGG` varchar(45) DEFAULT NULL,
'''   PRIMARY KEY (`module`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("xref_module_reactions", Database:="jp_kegg2", SchemaSQL:="
CREATE TABLE `xref_module_reactions` (
  `module` int(11) NOT NULL,
  `reaction` varchar(45) DEFAULT NULL,
  `KEGG` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`module`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class xref_module_reactions: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("module"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="module"), XmlAttribute> Public Property [module] As Long
    <DatabaseField("reaction"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="reaction")> Public Property reaction As String
    <DatabaseField("KEGG"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="KEGG")> Public Property KEGG As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `xref_module_reactions` (`module`, `reaction`, `KEGG`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `xref_module_reactions` (`module`, `reaction`, `KEGG`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `xref_module_reactions` (`module`, `reaction`, `KEGG`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `xref_module_reactions` (`module`, `reaction`, `KEGG`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `xref_module_reactions` WHERE `module` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `xref_module_reactions` SET `module`='{0}', `reaction`='{1}', `KEGG`='{2}' WHERE `module` = '{3}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `xref_module_reactions` WHERE `module` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, [module])
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `xref_module_reactions` (`module`, `reaction`, `KEGG`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, [module], reaction, KEGG)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `xref_module_reactions` (`module`, `reaction`, `KEGG`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, [module], reaction, KEGG)
        Else
        Return String.Format(INSERT_SQL, [module], reaction, KEGG)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{[module]}', '{reaction}', '{KEGG}')"
        Else
            Return $"('{[module]}', '{reaction}', '{KEGG}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `xref_module_reactions` (`module`, `reaction`, `KEGG`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, [module], reaction, KEGG)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `xref_module_reactions` (`module`, `reaction`, `KEGG`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, [module], reaction, KEGG)
        Else
        Return String.Format(REPLACE_SQL, [module], reaction, KEGG)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `xref_module_reactions` SET `module`='{0}', `reaction`='{1}', `KEGG`='{2}' WHERE `module` = '{3}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, [module], reaction, KEGG, [module])
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As xref_module_reactions
                         Return DirectCast(MyClass.MemberwiseClone, xref_module_reactions)
                     End Function
End Class


End Namespace
