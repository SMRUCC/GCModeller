#Region "Microsoft.VisualBasic::b7c92af103ad9385b98393e903372344, DataMySql\Interpro\Tables\entry_xref.vb"

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

    ' Class entry_xref
    ' 
    '     Properties: ac, dbcode, entry_ac, name
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

Namespace Interpro.Tables

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `entry_xref`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `entry_xref` (
'''   `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `dbcode` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `ac` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `name` varchar(70) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   PRIMARY KEY (`entry_ac`,`dbcode`,`ac`),
'''   KEY `fk_entry_xref$dbcode` (`dbcode`),
'''   CONSTRAINT `fk_entry_xref$dbcode` FOREIGN KEY (`dbcode`) REFERENCES `cv_database` (`dbcode`) ON DELETE NO ACTION ON UPDATE NO ACTION,
'''   CONSTRAINT `fk_entry_xref$entry_ac` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("entry_xref", Database:="interpro", SchemaSQL:="
CREATE TABLE `entry_xref` (
  `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `dbcode` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `ac` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `name` varchar(70) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  PRIMARY KEY (`entry_ac`,`dbcode`,`ac`),
  KEY `fk_entry_xref$dbcode` (`dbcode`),
  CONSTRAINT `fk_entry_xref$dbcode` FOREIGN KEY (`dbcode`) REFERENCES `cv_database` (`dbcode`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_entry_xref$entry_ac` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class entry_xref: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("entry_ac"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "9"), Column(Name:="entry_ac"), XmlAttribute> Public Property entry_ac As String
    <DatabaseField("dbcode"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "1"), Column(Name:="dbcode"), XmlAttribute> Public Property dbcode As String
    <DatabaseField("ac"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "30"), Column(Name:="ac"), XmlAttribute> Public Property ac As String
    <DatabaseField("name"), DataType(MySqlDbType.VarChar, "70"), Column(Name:="name")> Public Property name As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `entry_xref` (`entry_ac`, `dbcode`, `ac`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `entry_xref` (`entry_ac`, `dbcode`, `ac`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `entry_xref` (`entry_ac`, `dbcode`, `ac`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `entry_xref` (`entry_ac`, `dbcode`, `ac`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `entry_xref` WHERE `entry_ac`='{0}' and `dbcode`='{1}' and `ac`='{2}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `entry_xref` SET `entry_ac`='{0}', `dbcode`='{1}', `ac`='{2}', `name`='{3}' WHERE `entry_ac`='{4}' and `dbcode`='{5}' and `ac`='{6}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `entry_xref` WHERE `entry_ac`='{0}' and `dbcode`='{1}' and `ac`='{2}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, entry_ac, dbcode, ac)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `entry_xref` (`entry_ac`, `dbcode`, `ac`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, entry_ac, dbcode, ac, name)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `entry_xref` (`entry_ac`, `dbcode`, `ac`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, entry_ac, dbcode, ac, name)
        Else
        Return String.Format(INSERT_SQL, entry_ac, dbcode, ac, name)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{entry_ac}', '{dbcode}', '{ac}', '{name}')"
        Else
            Return $"('{entry_ac}', '{dbcode}', '{ac}', '{name}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `entry_xref` (`entry_ac`, `dbcode`, `ac`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, entry_ac, dbcode, ac, name)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `entry_xref` (`entry_ac`, `dbcode`, `ac`, `name`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, entry_ac, dbcode, ac, name)
        Else
        Return String.Format(REPLACE_SQL, entry_ac, dbcode, ac, name)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `entry_xref` SET `entry_ac`='{0}', `dbcode`='{1}', `ac`='{2}', `name`='{3}' WHERE `entry_ac`='{4}' and `dbcode`='{5}' and `ac`='{6}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, entry_ac, dbcode, ac, name, entry_ac, dbcode, ac)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As entry_xref
                         Return DirectCast(MyClass.MemberwiseClone, entry_xref)
                     End Function
End Class


End Namespace
