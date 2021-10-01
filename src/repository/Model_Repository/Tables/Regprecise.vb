#Region "Microsoft.VisualBasic::3acac1765cd36b780beff510cc3de44d, Model_Repository\Tables\Regprecise.vb"

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

    ' Class Regprecise
    ' 
    '     Properties: COG, Family, GUID, Regulog, SpeciesCode
    '                 Tfbs
    ' 
    '     Function: CreateObject, DBPath, GetPath
    ' 
    ' Class Regulator
    ' 
    '     Properties: Definition, Family, KEGG, Regulog, SequenceData
    '                 Sites, Species, vimssId
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Data.Linq.Mapping
' Imports System.Data.SQLite.Linq.QueryBuilder
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Oracle.LinuxCompatibility.MySQL.Scripting
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.SequenceModel

''' <summary>
''' Regulator entry inforamtion for the regprecise regulators.
''' </summary>
''' <remarks></remarks>
<Table(Name:="regprecise")>
Public Class Regprecise : Inherits DbFileSystemObject : Implements DbFileSystemObject.ProteinDescriptionModel

    <Column(Name:="guid", DbType:="int", IsPrimaryKey:=True)> Public Property GUID As Integer

    ''' <summary>
    ''' Species code on KEGG.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Column(Name:="species_code", DbType:="varchar(128)")> Public Property SpeciesCode As String Implements DbFileSystemObject.ProteinDescriptionModel.OrganismSpecies
    <Column(Name:="regulog", DbType:="varchar(2048)")> Public Property Regulog As String
    <Column(Name:="tfbs", DbType:="varchar(4096)")> Public Property Tfbs As String
    <Column(Name:="tf_family", DbType:="varchar(512)")> Public Property Family As String

    ''' <summary>
    ''' 由于文件比较小，所以所有的序列都存放在一个fasta文件之中了
    ''' </summary>
    ''' <param name="RepositoryRoot"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function GetPath(RepositoryRoot As String) As String
        Dim Path As String = DBPath(RepositoryRoot)
        Return Path
    End Function

    Public Shared Function DBPath(RepositoryRoot As String) As String
        Dim Path As String = RepositoryRoot & "/Regprecise_TF_Proteins.fasta"
        Return Path
    End Function

    Public Shared Function CreateObject(Fasta As FastaReaders.Regulator, MD5 As String) As Regprecise
        Dim EntryInfo As Regprecise =
            New Regprecise With {
                .Family = Fasta.Family,
                .MD5Hash = MD5,
                .Definition = Fasta.Definition.MySqlEscaping,
                .LocusID = Fasta.LocusTag,
                .Regulog = Fasta.Regulog,
                .SpeciesCode = Fasta.SpeciesCode,
                .Tfbs = String.Join("; ", Fasta.Sites)
        }
        Return EntryInfo
    End Function

    Public Property COG As String Implements DbFileSystemObject.ProteinDescriptionModel.COG

End Class

''' <summary>
''' 调控因子的蛋白质序列
''' > xcb:XC_1184|Family|Regulates|Regulog|Definition
''' </summary>
Public Class Regulator
    Implements INamedValue, IPolymerSequenceModel

    ''' <summary>
    ''' &lt;(KEGG)species_code>:&lt;locusTag>
    ''' </summary>
    ''' <returns></returns>
    Public Property KEGG As String Implements INamedValue.Key
    Public Property Sites As String()
    Public Property Family As String
    Public Property Regulog As String
    Public Property Definition As String
    Public Property SequenceData As String Implements IPolymerSequenceModel.SequenceData
    Public Property Species As String
    ''' <summary>
    ''' identifier of regulator gene in MicrobesOnline database 
    ''' </summary>
    ''' <returns></returns>
    Public Property vimssId As Integer

    ''' <summary>
    ''' $"{<see cref="KEGG"/>}|{<see cref="Family"/>}|{<see cref="String.Join"/>(";", <see cref="Sites"/>)}|{<see cref="Regulog"/>}|{<see cref="Definition"/>}"
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function ToString() As String
        Return $"{KEGG}|{Family}|{String.Join(";", Sites)}|{Regulog}|{Definition}"
    End Function
End Class
