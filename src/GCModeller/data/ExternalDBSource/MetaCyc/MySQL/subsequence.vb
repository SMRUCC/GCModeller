#Region "Microsoft.VisualBasic::96c9943b1156f1e32d25d17062f540a9, data\ExternalDBSource\MetaCyc\MySQL\subsequence.vb"

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

    ' Class subsequence
    ' 
    '     Properties: DataSetWID, FullSequence, Length, LengthApproximate, NucleicAcidWID
    '                 PercentGC, Sequence, Version, WID
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
''' DROP TABLE IF EXISTS `subsequence`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `subsequence` (
'''   `WID` bigint(20) NOT NULL,
'''   `NucleicAcidWID` bigint(20) NOT NULL,
'''   `FullSequence` char(1) DEFAULT NULL,
'''   `Sequence` longtext,
'''   `Length` int(11) DEFAULT NULL,
'''   `LengthApproximate` varchar(10) DEFAULT NULL,
'''   `PercentGC` float DEFAULT NULL,
'''   `Version` varchar(30) DEFAULT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_Subsequence1` (`NucleicAcidWID`),
'''   KEY `FK_Subsequence2` (`DataSetWID`),
'''   CONSTRAINT `FK_Subsequence1` FOREIGN KEY (`NucleicAcidWID`) REFERENCES `nucleicacid` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Subsequence2` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("subsequence", Database:="warehouse", SchemaSQL:="
CREATE TABLE `subsequence` (
  `WID` bigint(20) NOT NULL,
  `NucleicAcidWID` bigint(20) NOT NULL,
  `FullSequence` char(1) DEFAULT NULL,
  `Sequence` longtext,
  `Length` int(11) DEFAULT NULL,
  `LengthApproximate` varchar(10) DEFAULT NULL,
  `PercentGC` float DEFAULT NULL,
  `Version` varchar(30) DEFAULT NULL,
  `DataSetWID` bigint(20) NOT NULL,
  PRIMARY KEY (`WID`),
  KEY `FK_Subsequence1` (`NucleicAcidWID`),
  KEY `FK_Subsequence2` (`DataSetWID`),
  CONSTRAINT `FK_Subsequence1` FOREIGN KEY (`NucleicAcidWID`) REFERENCES `nucleicacid` (`WID`) ON DELETE CASCADE,
  CONSTRAINT `FK_Subsequence2` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class subsequence: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="WID"), XmlAttribute> Public Property WID As Long
    <DatabaseField("NucleicAcidWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="NucleicAcidWID")> Public Property NucleicAcidWID As Long
    <DatabaseField("FullSequence"), DataType(MySqlDbType.VarChar, "1"), Column(Name:="FullSequence")> Public Property FullSequence As String
    <DatabaseField("Sequence"), DataType(MySqlDbType.Text), Column(Name:="Sequence")> Public Property Sequence As String
    <DatabaseField("Length"), DataType(MySqlDbType.Int64, "11"), Column(Name:="Length")> Public Property Length As Long
    <DatabaseField("LengthApproximate"), DataType(MySqlDbType.VarChar, "10"), Column(Name:="LengthApproximate")> Public Property LengthApproximate As String
    <DatabaseField("PercentGC"), DataType(MySqlDbType.Double), Column(Name:="PercentGC")> Public Property PercentGC As Double
    <DatabaseField("Version"), DataType(MySqlDbType.VarChar, "30"), Column(Name:="Version")> Public Property Version As String
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20"), Column(Name:="DataSetWID")> Public Property DataSetWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `subsequence` (`WID`, `NucleicAcidWID`, `FullSequence`, `Sequence`, `Length`, `LengthApproximate`, `PercentGC`, `Version`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `subsequence` (`WID`, `NucleicAcidWID`, `FullSequence`, `Sequence`, `Length`, `LengthApproximate`, `PercentGC`, `Version`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `subsequence` (`WID`, `NucleicAcidWID`, `FullSequence`, `Sequence`, `Length`, `LengthApproximate`, `PercentGC`, `Version`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `subsequence` (`WID`, `NucleicAcidWID`, `FullSequence`, `Sequence`, `Length`, `LengthApproximate`, `PercentGC`, `Version`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `subsequence` WHERE `WID` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `subsequence` SET `WID`='{0}', `NucleicAcidWID`='{1}', `FullSequence`='{2}', `Sequence`='{3}', `Length`='{4}', `LengthApproximate`='{5}', `PercentGC`='{6}', `Version`='{7}', `DataSetWID`='{8}' WHERE `WID` = '{9}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `subsequence` WHERE `WID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `subsequence` (`WID`, `NucleicAcidWID`, `FullSequence`, `Sequence`, `Length`, `LengthApproximate`, `PercentGC`, `Version`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, NucleicAcidWID, FullSequence, Sequence, Length, LengthApproximate, PercentGC, Version, DataSetWID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `subsequence` (`WID`, `NucleicAcidWID`, `FullSequence`, `Sequence`, `Length`, `LengthApproximate`, `PercentGC`, `Version`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, WID, NucleicAcidWID, FullSequence, Sequence, Length, LengthApproximate, PercentGC, Version, DataSetWID)
        Else
        Return String.Format(INSERT_SQL, WID, NucleicAcidWID, FullSequence, Sequence, Length, LengthApproximate, PercentGC, Version, DataSetWID)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{WID}', '{NucleicAcidWID}', '{FullSequence}', '{Sequence}', '{Length}', '{LengthApproximate}', '{PercentGC}', '{Version}', '{DataSetWID}')"
        Else
            Return $"('{WID}', '{NucleicAcidWID}', '{FullSequence}', '{Sequence}', '{Length}', '{LengthApproximate}', '{PercentGC}', '{Version}', '{DataSetWID}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `subsequence` (`WID`, `NucleicAcidWID`, `FullSequence`, `Sequence`, `Length`, `LengthApproximate`, `PercentGC`, `Version`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, NucleicAcidWID, FullSequence, Sequence, Length, LengthApproximate, PercentGC, Version, DataSetWID)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `subsequence` (`WID`, `NucleicAcidWID`, `FullSequence`, `Sequence`, `Length`, `LengthApproximate`, `PercentGC`, `Version`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, WID, NucleicAcidWID, FullSequence, Sequence, Length, LengthApproximate, PercentGC, Version, DataSetWID)
        Else
        Return String.Format(REPLACE_SQL, WID, NucleicAcidWID, FullSequence, Sequence, Length, LengthApproximate, PercentGC, Version, DataSetWID)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `subsequence` SET `WID`='{0}', `NucleicAcidWID`='{1}', `FullSequence`='{2}', `Sequence`='{3}', `Length`='{4}', `LengthApproximate`='{5}', `PercentGC`='{6}', `Version`='{7}', `DataSetWID`='{8}' WHERE `WID` = '{9}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, NucleicAcidWID, FullSequence, Sequence, Length, LengthApproximate, PercentGC, Version, DataSetWID, WID)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As subsequence
                         Return DirectCast(MyClass.MemberwiseClone, subsequence)
                     End Function
End Class


End Namespace
