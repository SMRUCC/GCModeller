#Region "Microsoft.VisualBasic::42be70a380b70e5d749ac177b26feb38, data\ExternalDBSource\MetaCyc\MySQL\product.vb"

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

    ' Class product
    ' 
    '     Properties: Coefficient, OtherWID, ReactionWID
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
''' DROP TABLE IF EXISTS `product`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `product` (
'''   `ReactionWID` bigint(20) NOT NULL,
'''   `OtherWID` bigint(20) NOT NULL,
'''   `Coefficient` smallint(6) NOT NULL,
'''   KEY `FK_Product` (`ReactionWID`),
'''   CONSTRAINT `FK_Product` FOREIGN KEY (`ReactionWID`) REFERENCES `reaction` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("product", Database:="warehouse", SchemaSQL:="
CREATE TABLE `product` (
  `ReactionWID` bigint(20) NOT NULL,
  `OtherWID` bigint(20) NOT NULL,
  `Coefficient` smallint(6) NOT NULL,
  KEY `FK_Product` (`ReactionWID`),
  CONSTRAINT `FK_Product` FOREIGN KEY (`ReactionWID`) REFERENCES `reaction` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class product: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("ReactionWID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="ReactionWID"), XmlAttribute> Public Property ReactionWID As Long
    <DatabaseField("OtherWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="OtherWID")> Public Property OtherWID As Long
    <DatabaseField("Coefficient"), NotNull, DataType(MySqlDbType.Int64, "6"), Column(Name:="Coefficient")> Public Property Coefficient As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `product` (`ReactionWID`, `OtherWID`, `Coefficient`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `product` (`ReactionWID`, `OtherWID`, `Coefficient`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `product` (`ReactionWID`, `OtherWID`, `Coefficient`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `product` (`ReactionWID`, `OtherWID`, `Coefficient`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `product` WHERE `ReactionWID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `product` SET `ReactionWID`='{0}', `OtherWID`='{1}', `Coefficient`='{2}' WHERE `ReactionWID` = '{3}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `product` WHERE `ReactionWID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, ReactionWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `product` (`ReactionWID`, `OtherWID`, `Coefficient`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, ReactionWID, OtherWID, Coefficient)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `product` (`ReactionWID`, `OtherWID`, `Coefficient`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, ReactionWID, OtherWID, Coefficient)
        Else
        Return String.Format(INSERT_SQL, ReactionWID, OtherWID, Coefficient)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{ReactionWID}', '{OtherWID}', '{Coefficient}')"
        Else
            Return $"('{ReactionWID}', '{OtherWID}', '{Coefficient}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `product` (`ReactionWID`, `OtherWID`, `Coefficient`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, ReactionWID, OtherWID, Coefficient)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `product` (`ReactionWID`, `OtherWID`, `Coefficient`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, ReactionWID, OtherWID, Coefficient)
        Else
        Return String.Format(REPLACE_SQL, ReactionWID, OtherWID, Coefficient)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `product` SET `ReactionWID`='{0}', `OtherWID`='{1}', `Coefficient`='{2}' WHERE `ReactionWID` = '{3}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, ReactionWID, OtherWID, Coefficient, ReactionWID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As product
                         Return DirectCast(MyClass.MemberwiseClone, product)
                     End Function
End Class


End Namespace
