#Region "Microsoft.VisualBasic::b0c27a9bbc6e46bd5d2a09ca6ba222d2, data\KEGG\LocalMySQL\class_ko00001_orthology.vb"

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

    ' Class class_ko00001_orthology
    ' 
    '     Properties: [function], KEGG, level_A, level_B, level_C
    '                 name, Orthology
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
''' DROP TABLE IF EXISTS `class_ko00001_orthology`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `class_ko00001_orthology` (
'''   `Orthology` int(11) NOT NULL,
'''   `KEGG` varchar(45) DEFAULT NULL,
'''   `name` varchar(45) DEFAULT NULL,
'''   `function` varchar(45) DEFAULT NULL,
'''   `level_A` varchar(45) DEFAULT NULL,
'''   `level_B` varchar(45) DEFAULT NULL,
'''   `level_C` varchar(45) DEFAULT NULL COMMENT 'KEGG pathway',
'''   PRIMARY KEY (`Orthology`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("class_ko00001_orthology", Database:="jp_kegg2", SchemaSQL:="
CREATE TABLE `class_ko00001_orthology` (
  `Orthology` int(11) NOT NULL,
  `KEGG` varchar(45) DEFAULT NULL,
  `name` varchar(45) DEFAULT NULL,
  `function` varchar(45) DEFAULT NULL,
  `level_A` varchar(45) DEFAULT NULL,
  `level_B` varchar(45) DEFAULT NULL,
  `level_C` varchar(45) DEFAULT NULL COMMENT 'KEGG pathway',
  PRIMARY KEY (`Orthology`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class class_ko00001_orthology: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("Orthology"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="Orthology"), XmlAttribute> Public Property Orthology As Long
    <DatabaseField("KEGG"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="KEGG")> Public Property KEGG As String
    <DatabaseField("name"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="name")> Public Property name As String
    <DatabaseField("function"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="function")> Public Property [function] As String
    <DatabaseField("level_A"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="level_A")> Public Property level_A As String
    <DatabaseField("level_B"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="level_B")> Public Property level_B As String
''' <summary>
''' KEGG pathway
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("level_C"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="level_C")> Public Property level_C As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `class_ko00001_orthology` (`Orthology`, `KEGG`, `name`, `function`, `level_A`, `level_B`, `level_C`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `class_ko00001_orthology` (`Orthology`, `KEGG`, `name`, `function`, `level_A`, `level_B`, `level_C`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `class_ko00001_orthology` (`Orthology`, `KEGG`, `name`, `function`, `level_A`, `level_B`, `level_C`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `class_ko00001_orthology` (`Orthology`, `KEGG`, `name`, `function`, `level_A`, `level_B`, `level_C`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `class_ko00001_orthology` WHERE `Orthology` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `class_ko00001_orthology` SET `Orthology`='{0}', `KEGG`='{1}', `name`='{2}', `function`='{3}', `level_A`='{4}', `level_B`='{5}', `level_C`='{6}' WHERE `Orthology` = '{7}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `class_ko00001_orthology` WHERE `Orthology` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, Orthology)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `class_ko00001_orthology` (`Orthology`, `KEGG`, `name`, `function`, `level_A`, `level_B`, `level_C`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, Orthology, KEGG, name, [function], level_A, level_B, level_C)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `class_ko00001_orthology` (`Orthology`, `KEGG`, `name`, `function`, `level_A`, `level_B`, `level_C`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, Orthology, KEGG, name, [function], level_A, level_B, level_C)
        Else
        Return String.Format(INSERT_SQL, Orthology, KEGG, name, [function], level_A, level_B, level_C)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{Orthology}', '{KEGG}', '{name}', '{[function]}', '{level_A}', '{level_B}', '{level_C}')"
        Else
            Return $"('{Orthology}', '{KEGG}', '{name}', '{[function]}', '{level_A}', '{level_B}', '{level_C}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `class_ko00001_orthology` (`Orthology`, `KEGG`, `name`, `function`, `level_A`, `level_B`, `level_C`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, Orthology, KEGG, name, [function], level_A, level_B, level_C)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `class_ko00001_orthology` (`Orthology`, `KEGG`, `name`, `function`, `level_A`, `level_B`, `level_C`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, Orthology, KEGG, name, [function], level_A, level_B, level_C)
        Else
        Return String.Format(REPLACE_SQL, Orthology, KEGG, name, [function], level_A, level_B, level_C)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `class_ko00001_orthology` SET `Orthology`='{0}', `KEGG`='{1}', `name`='{2}', `function`='{3}', `level_A`='{4}', `level_B`='{5}', `level_C`='{6}' WHERE `Orthology` = '{7}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, Orthology, KEGG, name, [function], level_A, level_B, level_C, Orthology)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As class_ko00001_orthology
                         Return DirectCast(MyClass.MemberwiseClone, class_ko00001_orthology)
                     End Function
End Class


End Namespace
