#Region "Microsoft.VisualBasic::004f715075c5cf2d95b00d2fd8ae4987, data\ExternalDBSource\MetaCyc\MySQL\featurewidfeaturewid2.vb"

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

    ' Class featurewidfeaturewid2
    ' 
    '     Properties: FeatureWID1, FeatureWID2
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
''' DROP TABLE IF EXISTS `featurewidfeaturewid2`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `featurewidfeaturewid2` (
'''   `FeatureWID1` bigint(20) NOT NULL,
'''   `FeatureWID2` bigint(20) NOT NULL,
'''   KEY `FK_FeatureWIDFeatureWID21` (`FeatureWID1`),
'''   KEY `FK_FeatureWIDFeatureWID22` (`FeatureWID2`),
'''   CONSTRAINT `FK_FeatureWIDFeatureWID21` FOREIGN KEY (`FeatureWID1`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_FeatureWIDFeatureWID22` FOREIGN KEY (`FeatureWID2`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("featurewidfeaturewid2", Database:="warehouse", SchemaSQL:="
CREATE TABLE `featurewidfeaturewid2` (
  `FeatureWID1` bigint(20) NOT NULL,
  `FeatureWID2` bigint(20) NOT NULL,
  KEY `FK_FeatureWIDFeatureWID21` (`FeatureWID1`),
  KEY `FK_FeatureWIDFeatureWID22` (`FeatureWID2`),
  CONSTRAINT `FK_FeatureWIDFeatureWID21` FOREIGN KEY (`FeatureWID1`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_FeatureWIDFeatureWID22` FOREIGN KEY (`FeatureWID2`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class featurewidfeaturewid2: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("FeatureWID1"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="FeatureWID1"), XmlAttribute> Public Property FeatureWID1 As Long
    <DatabaseField("FeatureWID2"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="FeatureWID2")> Public Property FeatureWID2 As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `featurewidfeaturewid2` (`FeatureWID1`, `FeatureWID2`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `featurewidfeaturewid2` (`FeatureWID1`, `FeatureWID2`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `featurewidfeaturewid2` (`FeatureWID1`, `FeatureWID2`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `featurewidfeaturewid2` (`FeatureWID1`, `FeatureWID2`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `featurewidfeaturewid2` WHERE `FeatureWID1` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `featurewidfeaturewid2` SET `FeatureWID1`='{0}', `FeatureWID2`='{1}' WHERE `FeatureWID1` = '{2}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `featurewidfeaturewid2` WHERE `FeatureWID1` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, FeatureWID1)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `featurewidfeaturewid2` (`FeatureWID1`, `FeatureWID2`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, FeatureWID1, FeatureWID2)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `featurewidfeaturewid2` (`FeatureWID1`, `FeatureWID2`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, FeatureWID1, FeatureWID2)
        Else
        Return String.Format(INSERT_SQL, FeatureWID1, FeatureWID2)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{FeatureWID1}', '{FeatureWID2}')"
        Else
            Return $"('{FeatureWID1}', '{FeatureWID2}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `featurewidfeaturewid2` (`FeatureWID1`, `FeatureWID2`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, FeatureWID1, FeatureWID2)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `featurewidfeaturewid2` (`FeatureWID1`, `FeatureWID2`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, FeatureWID1, FeatureWID2)
        Else
        Return String.Format(REPLACE_SQL, FeatureWID1, FeatureWID2)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `featurewidfeaturewid2` SET `FeatureWID1`='{0}', `FeatureWID2`='{1}' WHERE `FeatureWID1` = '{2}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, FeatureWID1, FeatureWID2, FeatureWID1)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As featurewidfeaturewid2
                         Return DirectCast(MyClass.MemberwiseClone, featurewidfeaturewid2)
                     End Function
End Class


End Namespace
