#Region "Microsoft.VisualBasic::af60706934a3ecd3545bf60f7ef51c25, DataMySql\Interpro\Tables\entry_friends.vb"

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

    ' Class entry_friends
    ' 
    '     Properties: a1, a2, ab, entry1_ac, entry2_ac
    '                 p1, p2, pb, s
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
''' DROP TABLE IF EXISTS `entry_friends`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `entry_friends` (
'''   `entry1_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `entry2_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `s` int(3) NOT NULL,
'''   `p1` int(7) NOT NULL,
'''   `p2` int(7) NOT NULL,
'''   `pb` int(7) NOT NULL,
'''   `a1` int(5) NOT NULL,
'''   `a2` int(5) NOT NULL,
'''   `ab` int(5) NOT NULL,
'''   PRIMARY KEY (`entry1_ac`,`entry2_ac`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("entry_friends", Database:="interpro", SchemaSQL:="
CREATE TABLE `entry_friends` (
  `entry1_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `entry2_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `s` int(3) NOT NULL,
  `p1` int(7) NOT NULL,
  `p2` int(7) NOT NULL,
  `pb` int(7) NOT NULL,
  `a1` int(5) NOT NULL,
  `a2` int(5) NOT NULL,
  `ab` int(5) NOT NULL,
  PRIMARY KEY (`entry1_ac`,`entry2_ac`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class entry_friends: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("entry1_ac"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "9"), Column(Name:="entry1_ac"), XmlAttribute> Public Property entry1_ac As String
    <DatabaseField("entry2_ac"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "9"), Column(Name:="entry2_ac"), XmlAttribute> Public Property entry2_ac As String
    <DatabaseField("s"), NotNull, DataType(MySqlDbType.Int64, "3"), Column(Name:="s")> Public Property s As Long
    <DatabaseField("p1"), NotNull, DataType(MySqlDbType.Int64, "7"), Column(Name:="p1")> Public Property p1 As Long
    <DatabaseField("p2"), NotNull, DataType(MySqlDbType.Int64, "7"), Column(Name:="p2")> Public Property p2 As Long
    <DatabaseField("pb"), NotNull, DataType(MySqlDbType.Int64, "7"), Column(Name:="pb")> Public Property pb As Long
    <DatabaseField("a1"), NotNull, DataType(MySqlDbType.Int64, "5"), Column(Name:="a1")> Public Property a1 As Long
    <DatabaseField("a2"), NotNull, DataType(MySqlDbType.Int64, "5"), Column(Name:="a2")> Public Property a2 As Long
    <DatabaseField("ab"), NotNull, DataType(MySqlDbType.Int64, "5"), Column(Name:="ab")> Public Property ab As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `entry_friends` (`entry1_ac`, `entry2_ac`, `s`, `p1`, `p2`, `pb`, `a1`, `a2`, `ab`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `entry_friends` (`entry1_ac`, `entry2_ac`, `s`, `p1`, `p2`, `pb`, `a1`, `a2`, `ab`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `entry_friends` (`entry1_ac`, `entry2_ac`, `s`, `p1`, `p2`, `pb`, `a1`, `a2`, `ab`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `entry_friends` (`entry1_ac`, `entry2_ac`, `s`, `p1`, `p2`, `pb`, `a1`, `a2`, `ab`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `entry_friends` WHERE `entry1_ac`='{0}' and `entry2_ac`='{1}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `entry_friends` SET `entry1_ac`='{0}', `entry2_ac`='{1}', `s`='{2}', `p1`='{3}', `p2`='{4}', `pb`='{5}', `a1`='{6}', `a2`='{7}', `ab`='{8}' WHERE `entry1_ac`='{9}' and `entry2_ac`='{10}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `entry_friends` WHERE `entry1_ac`='{0}' and `entry2_ac`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, entry1_ac, entry2_ac)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `entry_friends` (`entry1_ac`, `entry2_ac`, `s`, `p1`, `p2`, `pb`, `a1`, `a2`, `ab`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, entry1_ac, entry2_ac, s, p1, p2, pb, a1, a2, ab)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `entry_friends` (`entry1_ac`, `entry2_ac`, `s`, `p1`, `p2`, `pb`, `a1`, `a2`, `ab`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, entry1_ac, entry2_ac, s, p1, p2, pb, a1, a2, ab)
        Else
        Return String.Format(INSERT_SQL, entry1_ac, entry2_ac, s, p1, p2, pb, a1, a2, ab)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{entry1_ac}', '{entry2_ac}', '{s}', '{p1}', '{p2}', '{pb}', '{a1}', '{a2}', '{ab}')"
        Else
            Return $"('{entry1_ac}', '{entry2_ac}', '{s}', '{p1}', '{p2}', '{pb}', '{a1}', '{a2}', '{ab}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `entry_friends` (`entry1_ac`, `entry2_ac`, `s`, `p1`, `p2`, `pb`, `a1`, `a2`, `ab`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, entry1_ac, entry2_ac, s, p1, p2, pb, a1, a2, ab)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `entry_friends` (`entry1_ac`, `entry2_ac`, `s`, `p1`, `p2`, `pb`, `a1`, `a2`, `ab`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, entry1_ac, entry2_ac, s, p1, p2, pb, a1, a2, ab)
        Else
        Return String.Format(REPLACE_SQL, entry1_ac, entry2_ac, s, p1, p2, pb, a1, a2, ab)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `entry_friends` SET `entry1_ac`='{0}', `entry2_ac`='{1}', `s`='{2}', `p1`='{3}', `p2`='{4}', `pb`='{5}', `a1`='{6}', `a2`='{7}', `ab`='{8}' WHERE `entry1_ac`='{9}' and `entry2_ac`='{10}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, entry1_ac, entry2_ac, s, p1, p2, pb, a1, a2, ab, entry1_ac, entry2_ac)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As entry_friends
                         Return DirectCast(MyClass.MemberwiseClone, entry_friends)
                     End Function
End Class


End Namespace
