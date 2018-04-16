#Region "Microsoft.VisualBasic::1abdadaa2d4fe1b70c2374256d764723, data\KEGG\LocalMySQL\data_enzyme.vb"

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

    ' Class data_enzyme
    ' 
    '     Properties: Comment, EC, name, Product, Reaction_IUBMB_
    '                 Reaction_KEGG_, Reaction_KEGG__uid, Substrate, sysname, uid
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @3/16/2018 10:40:15 PM


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
''' DROP TABLE IF EXISTS `data_enzyme`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `data_enzyme` (
'''   `uid` int(11) NOT NULL,
'''   `EC` varchar(45) DEFAULT NULL,
'''   `name` varchar(45) DEFAULT NULL,
'''   `sysname` varchar(45) DEFAULT NULL,
'''   `Reaction(KEGG)_uid` varchar(45) DEFAULT NULL,
'''   `Reaction(KEGG)` varchar(45) DEFAULT NULL,
'''   `Reaction(IUBMB)` varchar(45) DEFAULT NULL,
'''   `Substrate` varchar(45) DEFAULT NULL,
'''   `Product` varchar(45) DEFAULT NULL,
'''   `Comment` varchar(45) DEFAULT NULL,
'''   PRIMARY KEY (`uid`),
'''   UNIQUE KEY `uid_UNIQUE` (`uid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("data_enzyme", Database:="jp_kegg2", SchemaSQL:="
CREATE TABLE `data_enzyme` (
  `uid` int(11) NOT NULL,
  `EC` varchar(45) DEFAULT NULL,
  `name` varchar(45) DEFAULT NULL,
  `sysname` varchar(45) DEFAULT NULL,
  `Reaction(KEGG)_uid` varchar(45) DEFAULT NULL,
  `Reaction(KEGG)` varchar(45) DEFAULT NULL,
  `Reaction(IUBMB)` varchar(45) DEFAULT NULL,
  `Substrate` varchar(45) DEFAULT NULL,
  `Product` varchar(45) DEFAULT NULL,
  `Comment` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class data_enzyme: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("uid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="uid"), XmlAttribute> Public Property uid As Long
    <DatabaseField("EC"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="EC")> Public Property EC As String
    <DatabaseField("name"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="name")> Public Property name As String
    <DatabaseField("sysname"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="sysname")> Public Property sysname As String
    <DatabaseField("Reaction(KEGG)_uid"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="Reaction(KEGG)_uid")> Public Property Reaction_KEGG__uid As String
    <DatabaseField("Reaction(KEGG)"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="Reaction(KEGG)")> Public Property Reaction_KEGG_ As String
    <DatabaseField("Reaction(IUBMB)"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="Reaction(IUBMB)")> Public Property Reaction_IUBMB_ As String
    <DatabaseField("Substrate"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="Substrate")> Public Property Substrate As String
    <DatabaseField("Product"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="Product")> Public Property Product As String
    <DatabaseField("Comment"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="Comment")> Public Property Comment As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `data_enzyme` (`uid`, `EC`, `name`, `sysname`, `Reaction(KEGG)_uid`, `Reaction(KEGG)`, `Reaction(IUBMB)`, `Substrate`, `Product`, `Comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `data_enzyme` (`uid`, `EC`, `name`, `sysname`, `Reaction(KEGG)_uid`, `Reaction(KEGG)`, `Reaction(IUBMB)`, `Substrate`, `Product`, `Comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `data_enzyme` WHERE `uid` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `data_enzyme` SET `uid`='{0}', `EC`='{1}', `name`='{2}', `sysname`='{3}', `Reaction(KEGG)_uid`='{4}', `Reaction(KEGG)`='{5}', `Reaction(IUBMB)`='{6}', `Substrate`='{7}', `Product`='{8}', `Comment`='{9}' WHERE `uid` = '{10}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `data_enzyme` WHERE `uid` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, uid)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `data_enzyme` (`uid`, `EC`, `name`, `sysname`, `Reaction(KEGG)_uid`, `Reaction(KEGG)`, `Reaction(IUBMB)`, `Substrate`, `Product`, `Comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, uid, EC, name, sysname, Reaction_KEGG__uid, Reaction_KEGG_, Reaction_IUBMB_, Substrate, Product, Comment)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{uid}', '{EC}', '{name}', '{sysname}', '{Reaction_KEGG__uid}', '{Reaction_KEGG_}', '{Reaction_IUBMB_}', '{Substrate}', '{Product}', '{Comment}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `data_enzyme` (`uid`, `EC`, `name`, `sysname`, `Reaction(KEGG)_uid`, `Reaction(KEGG)`, `Reaction(IUBMB)`, `Substrate`, `Product`, `Comment`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, uid, EC, name, sysname, Reaction_KEGG__uid, Reaction_KEGG_, Reaction_IUBMB_, Substrate, Product, Comment)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `data_enzyme` SET `uid`='{0}', `EC`='{1}', `name`='{2}', `sysname`='{3}', `Reaction(KEGG)_uid`='{4}', `Reaction(KEGG)`='{5}', `Reaction(IUBMB)`='{6}', `Substrate`='{7}', `Product`='{8}', `Comment`='{9}' WHERE `uid` = '{10}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, uid, EC, name, sysname, Reaction_KEGG__uid, Reaction_KEGG_, Reaction_IUBMB_, Substrate, Product, Comment, uid)
    End Function
#End Region
Public Function Clone() As data_enzyme
                  Return DirectCast(MyClass.MemberwiseClone, data_enzyme)
              End Function
End Class


End Namespace
