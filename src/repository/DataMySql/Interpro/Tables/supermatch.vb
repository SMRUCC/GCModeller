#Region "Microsoft.VisualBasic::612478982fe4c8702325e108e75d8f09, DataMySql\Interpro\Tables\supermatch.vb"

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

    ' Class supermatch
    ' 
    '     Properties: entry_ac, pos_from, pos_to, protein_ac
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
''' DROP TABLE IF EXISTS `supermatch`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `supermatch` (
'''   `protein_ac` varchar(6) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `pos_from` int(5) NOT NULL,
'''   `pos_to` int(5) NOT NULL,
'''   PRIMARY KEY (`protein_ac`,`entry_ac`,`pos_to`,`pos_from`),
'''   KEY `fkv_supermatch$entry_ac` (`entry_ac`),
'''   CONSTRAINT `fkv_supermatch$entry_ac` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION,
'''   CONSTRAINT `fkv_supermatch$protein_ac` FOREIGN KEY (`protein_ac`) REFERENCES `protein` (`protein_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("supermatch", Database:="interpro", SchemaSQL:="
CREATE TABLE `supermatch` (
  `protein_ac` varchar(6) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `pos_from` int(5) NOT NULL,
  `pos_to` int(5) NOT NULL,
  PRIMARY KEY (`protein_ac`,`entry_ac`,`pos_to`,`pos_from`),
  KEY `fkv_supermatch$entry_ac` (`entry_ac`),
  CONSTRAINT `fkv_supermatch$entry_ac` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `fkv_supermatch$protein_ac` FOREIGN KEY (`protein_ac`) REFERENCES `protein` (`protein_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class supermatch: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("protein_ac"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "6"), Column(Name:="protein_ac"), XmlAttribute> Public Property protein_ac As String
    <DatabaseField("entry_ac"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "9"), Column(Name:="entry_ac"), XmlAttribute> Public Property entry_ac As String
    <DatabaseField("pos_from"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "5"), Column(Name:="pos_from"), XmlAttribute> Public Property pos_from As Long
    <DatabaseField("pos_to"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "5"), Column(Name:="pos_to"), XmlAttribute> Public Property pos_to As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `supermatch` (`protein_ac`, `entry_ac`, `pos_from`, `pos_to`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `supermatch` (`protein_ac`, `entry_ac`, `pos_from`, `pos_to`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `supermatch` (`protein_ac`, `entry_ac`, `pos_from`, `pos_to`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `supermatch` (`protein_ac`, `entry_ac`, `pos_from`, `pos_to`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `supermatch` WHERE `protein_ac`='{0}' and `entry_ac`='{1}' and `pos_to`='{2}' and `pos_from`='{3}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `supermatch` SET `protein_ac`='{0}', `entry_ac`='{1}', `pos_from`='{2}', `pos_to`='{3}' WHERE `protein_ac`='{4}' and `entry_ac`='{5}' and `pos_to`='{6}' and `pos_from`='{7}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `supermatch` WHERE `protein_ac`='{0}' and `entry_ac`='{1}' and `pos_to`='{2}' and `pos_from`='{3}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, protein_ac, entry_ac, pos_to, pos_from)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `supermatch` (`protein_ac`, `entry_ac`, `pos_from`, `pos_to`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, protein_ac, entry_ac, pos_from, pos_to)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `supermatch` (`protein_ac`, `entry_ac`, `pos_from`, `pos_to`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, protein_ac, entry_ac, pos_from, pos_to)
        Else
        Return String.Format(INSERT_SQL, protein_ac, entry_ac, pos_from, pos_to)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{protein_ac}', '{entry_ac}', '{pos_from}', '{pos_to}')"
        Else
            Return $"('{protein_ac}', '{entry_ac}', '{pos_from}', '{pos_to}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `supermatch` (`protein_ac`, `entry_ac`, `pos_from`, `pos_to`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, protein_ac, entry_ac, pos_from, pos_to)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `supermatch` (`protein_ac`, `entry_ac`, `pos_from`, `pos_to`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, protein_ac, entry_ac, pos_from, pos_to)
        Else
        Return String.Format(REPLACE_SQL, protein_ac, entry_ac, pos_from, pos_to)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `supermatch` SET `protein_ac`='{0}', `entry_ac`='{1}', `pos_from`='{2}', `pos_to`='{3}' WHERE `protein_ac`='{4}' and `entry_ac`='{5}' and `pos_to`='{6}' and `pos_from`='{7}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, protein_ac, entry_ac, pos_from, pos_to, protein_ac, entry_ac, pos_to, pos_from)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As supermatch
                         Return DirectCast(MyClass.MemberwiseClone, supermatch)
                     End Function
End Class


End Namespace
