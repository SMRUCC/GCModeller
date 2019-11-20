#Region "Microsoft.VisualBasic::208bfb2e00d96fbc760495e81a1e2a8f, data\KEGG\jp_kegg2\data_reactions.vb"

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

    ' Class data_reactions
    ' 
    '     Properties: comment, definition, EC, KEGG, name
    '                 products, substrates, uid
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

REM  Dump @2018/5/23 13:16:34


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace mysql

''' <summary>
''' ```SQL
''' KEGG之中的生物代谢反应的定义数据，这个表会包括非酶促反应过程和酶促反应过程
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `data_reactions`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `data_reactions` (
'''   `uid` int(11) NOT NULL,
'''   `KEGG` varchar(45) NOT NULL COMMENT 'rn:R.... KEGG reaction id',
'''   `EC` varchar(45) DEFAULT NULL,
'''   `name` varchar(45) DEFAULT NULL,
'''   `definition` varchar(45) DEFAULT NULL,
'''   `substrates` varchar(45) DEFAULT NULL COMMENT 'KEGG compounds uid list, in long array formats, like: 1, 2, 3, 4,   ``data_compounds.uid``',
'''   `products` varchar(45) DEFAULT NULL COMMENT 'KEGG compounds uid list, in long array formats, like: 1, 2, 3, 4',
'''   `comment` varchar(45) DEFAULT NULL,
'''   PRIMARY KEY (`uid`),
'''   UNIQUE KEY `uid_UNIQUE` (`uid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='KEGG之中的生物代谢反应的定义数据，这个表会包括非酶促反应过程和酶促反应过程';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("data_reactions", Database:="jp_kegg2", SchemaSQL:="
CREATE TABLE `data_reactions` (
  `uid` int(11) NOT NULL,
  `KEGG` varchar(45) NOT NULL COMMENT 'rn:R.... KEGG reaction id',
  `EC` varchar(45) DEFAULT NULL,
  `name` varchar(45) DEFAULT NULL,
  `definition` varchar(45) DEFAULT NULL,
  `substrates` varchar(45) DEFAULT NULL COMMENT 'KEGG compounds uid list, in long array formats, like: 1, 2, 3, 4,   ``data_compounds.uid``',
  `products` varchar(45) DEFAULT NULL COMMENT 'KEGG compounds uid list, in long array formats, like: 1, 2, 3, 4',
  `comment` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='KEGG之中的生物代谢反应的定义数据，这个表会包括非酶促反应过程和酶促反应过程';")>
Public Class data_reactions: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("uid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="uid"), XmlAttribute> Public Property uid As Long
''' <summary>
''' rn:R.... KEGG reaction id
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("KEGG"), NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="KEGG")> Public Property KEGG As String
    <DatabaseField("EC"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="EC")> Public Property EC As String
    <DatabaseField("name"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="name")> Public Property name As String
    <DatabaseField("definition"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="definition")> Public Property definition As String
''' <summary>
''' KEGG compounds uid list, in long array formats, like: 1, 2, 3, 4,   ``data_compounds.uid``
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("substrates"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="substrates")> Public Property substrates As String
''' <summary>
''' KEGG compounds uid list, in long array formats, like: 1, 2, 3, 4
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("products"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="products")> Public Property products As String
    <DatabaseField("comment"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="comment")> Public Property comment As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `data_reactions` (`uid`, `KEGG`, `EC`, `name`, `definition`, `substrates`, `products`, `comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `data_reactions` (`uid`, `KEGG`, `EC`, `name`, `definition`, `substrates`, `products`, `comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `data_reactions` (`uid`, `KEGG`, `EC`, `name`, `definition`, `substrates`, `products`, `comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `data_reactions` (`uid`, `KEGG`, `EC`, `name`, `definition`, `substrates`, `products`, `comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `data_reactions` WHERE `uid` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `data_reactions` SET `uid`='{0}', `KEGG`='{1}', `EC`='{2}', `name`='{3}', `definition`='{4}', `substrates`='{5}', `products`='{6}', `comment`='{7}' WHERE `uid` = '{8}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `data_reactions` WHERE `uid` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, uid)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `data_reactions` (`uid`, `KEGG`, `EC`, `name`, `definition`, `substrates`, `products`, `comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, uid, KEGG, EC, name, definition, substrates, products, comment)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `data_reactions` (`uid`, `KEGG`, `EC`, `name`, `definition`, `substrates`, `products`, `comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, uid, KEGG, EC, name, definition, substrates, products, comment)
        Else
        Return String.Format(INSERT_SQL, uid, KEGG, EC, name, definition, substrates, products, comment)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{uid}', '{KEGG}', '{EC}', '{name}', '{definition}', '{substrates}', '{products}', '{comment}')"
        Else
            Return $"('{uid}', '{KEGG}', '{EC}', '{name}', '{definition}', '{substrates}', '{products}', '{comment}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `data_reactions` (`uid`, `KEGG`, `EC`, `name`, `definition`, `substrates`, `products`, `comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, uid, KEGG, EC, name, definition, substrates, products, comment)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `data_reactions` (`uid`, `KEGG`, `EC`, `name`, `definition`, `substrates`, `products`, `comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, uid, KEGG, EC, name, definition, substrates, products, comment)
        Else
        Return String.Format(REPLACE_SQL, uid, KEGG, EC, name, definition, substrates, products, comment)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `data_reactions` SET `uid`='{0}', `KEGG`='{1}', `EC`='{2}', `name`='{3}', `definition`='{4}', `substrates`='{5}', `products`='{6}', `comment`='{7}' WHERE `uid` = '{8}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, uid, KEGG, EC, name, definition, substrates, products, comment, uid)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As data_reactions
                         Return DirectCast(MyClass.MemberwiseClone, data_reactions)
                     End Function
End Class


End Namespace
