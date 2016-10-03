#Region "Microsoft.VisualBasic::7113cbaa8ac8d72caa82761f202651de, ..\GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\GenBank\TabularFormat\FeatureBriefs\GFF\FeatureKeys.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports System.Text
Imports SMRUCC.genomics.ComponentModel.Loci
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language

Namespace Assembly.NCBI.GenBank.TabularFormat

    Public Module FeatureKeys

        Public Const tRNA As String = "tRNA"
        Public Const CDS As String = "CDS"
        Public Const exon As String = "exon"
        Public Const gene As String = "gene"
        Public Const tmRNA As String = "tmRNA"
        Public Const rRNA As String = "rRNA"
        Public Const region As String = "region"

        Public Enum Features As Integer
            UnDefine = -1
            CDS
            gene
            tRNA
            exon
            tmRNA
            rRNA
            region
        End Enum

        <Extension>
        Public Function [GetFeatureType](x As Feature) As Features
            If String.IsNullOrEmpty(x.Feature) OrElse
                Not FeatureKeys.FeaturesHash.ContainsKey(x.Feature) Then
                Return Features.UnDefine
            Else
                Return FeatureKeys.FeaturesHash(x.Feature)
            End If
        End Function

        <Extension>
        Public Function GetsAllFeatures(source As IEnumerable(Of Feature), type As Features) As Feature()
            Return LinqAPI.Exec(Of Feature) <= From x As Feature
                                               In source
                                               Where type = x.GetFeatureType
                                               Select x
        End Function

        <Extension>
        Public Function GetsAllFeatures(gff As GFF, type As Features) As Feature()
            Return gff.Features.GetsAllFeatures(type)
        End Function

        Public ReadOnly Property FeaturesHash As IReadOnlyDictionary(Of String, Features) =
            New Dictionary(Of String, Features) From {
 _
            {FeatureKeys.CDS, Features.CDS},
            {FeatureKeys.exon, Features.exon},
            {FeatureKeys.gene, Features.gene},
            {FeatureKeys.region, Features.region},
            {FeatureKeys.rRNA, Features.rRNA},
            {FeatureKeys.tmRNA, Features.tmRNA},
            {FeatureKeys.tRNA, Features.tRNA}
        }
    End Module
End Namespace
