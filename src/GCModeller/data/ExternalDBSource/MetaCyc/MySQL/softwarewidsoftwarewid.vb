#Region "Microsoft.VisualBasic::53c39cf27279136ec2d05374aaad2a8b, data\ExternalDBSource\MetaCyc\MySQL\softwarewidsoftwarewid.vb"

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

    ' Class softwarewidsoftwarewid
    ' 
    '     Properties: SoftwareWID1, SoftwareWID2
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

REM  Dump @2018/5/23 13:13:40


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace MetaCyc.MySQL

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `softwarewidsoftwarewid`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `softwarewidsoftwarewid` (
'''   `SoftwareWID1` bigint(20) NOT NULL,
'''   `SoftwareWID2` bigint(20) NOT NULL,
'''   KEY `FK_SoftwareWIDSoftwareWID1` (`SoftwareWID1`),
'''   KEY `FK_SoftwareWIDSoftwareWID2` (`SoftwareWID2`),
'''   CONSTRAINT `FK_SoftwareWIDSoftwareWID1` FOREIGN KEY (`SoftwareWID1`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_SoftwareWIDSoftwareWID2` FOREIGN KEY (`SoftwareWID2`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("softwarewidsoftwarewid", Database:="warehouse", SchemaSQL:="
CREATE TABLE `softwarewidsoftwarewid` (
  `SoftwareWID1` bigint(20) NOT NULL,
  `SoftwareWID2` bigint(20) NOT NULL,
  KEY `FK_SoftwareWIDSoftwareWID1` (`SoftwareWID1`),
  KEY `FK_SoftwareWIDSoftwareWID2` (`SoftwareWID2`),
  CONSTRAINT `FK_SoftwareWIDSoftwareWID1` FOREIGN KEY (`SoftwareWID1`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_SoftwareWIDSoftwareWID2` FOREIGN KEY (`SoftwareWID2`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class softwarewidsoftwarewid: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("SoftwareWID1"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="SoftwareWID1"), XmlAttribute> Public Property SoftwareWID1 As Long
    <DatabaseField("SoftwareWID2"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="SoftwareWID2")> Public Property SoftwareWID2 As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `softwarewidsoftwarewid` (`SoftwareWID1`, `SoftwareWID2`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `softwarewidsoftwarewid` (`SoftwareWID1`, `SoftwareWID2`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `softwarewidsoftwarewid` (`SoftwareWID1`, `SoftwareWID2`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `softwarewidsoftwarewid` (`SoftwareWID1`, `SoftwareWID2`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `softwarewidsoftwarewid` WHERE `SoftwareWID1` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `softwarewidsoftwarewid` SET `SoftwareWID1`='{0}', `SoftwareWID2`='{1}' WHERE `SoftwareWID1` = '{2}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `softwarewidsoftwarewid` WHERE `SoftwareWID1` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, SoftwareWID1)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `softwarewidsoftwarewid` (`SoftwareWID1`, `SoftwareWID2`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, SoftwareWID1, SoftwareWID2)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `softwarewidsoftwarewid` (`SoftwareWID1`, `SoftwareWID2`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, SoftwareWID1, SoftwareWID2)
        Else
        Return String.Format(INSERT_SQL, SoftwareWID1, SoftwareWID2)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{SoftwareWID1}', '{SoftwareWID2}')"
        Else
            Return $"('{SoftwareWID1}', '{SoftwareWID2}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `softwarewidsoftwarewid` (`SoftwareWID1`, `SoftwareWID2`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, SoftwareWID1, SoftwareWID2)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `softwarewidsoftwarewid` (`SoftwareWID1`, `SoftwareWID2`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, SoftwareWID1, SoftwareWID2)
        Else
        Return String.Format(REPLACE_SQL, SoftwareWID1, SoftwareWID2)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `softwarewidsoftwarewid` SET `SoftwareWID1`='{0}', `SoftwareWID2`='{1}' WHERE `SoftwareWID1` = '{2}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, SoftwareWID1, SoftwareWID2, SoftwareWID1)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As softwarewidsoftwarewid
                         Return DirectCast(MyClass.MemberwiseClone, softwarewidsoftwarewid)
                     End Function
End Class


End Namespace
