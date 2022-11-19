#Region "Microsoft.VisualBasic::5fca408fe907a995d26ab5d73cebfb5f, GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\GenBank\GBK\Gene.vb"

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


    ' Code Statistics:

    '   Total Lines: 68
    '    Code Lines: 57
    ' Comment Lines: 0
    '   Blank Lines: 11
    '     File Size: 2.54 KB


    '     Class GeneObject
    ' 
    '         Properties: Features, Gene, LocusTag
    ' 
    '         Function: ToString
    ' 
    '     Module ExportGenes
    ' 
    '         Function: GetGene, GetGenes
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES

Namespace Assembly.NCBI.GenBank.GBFF

    Public Class GeneObject
        Implements INamedValue
        Implements IKeyValuePairObject(Of String, Feature())

        Public Property Gene As String
        Public Property LocusTag As String Implements INamedValue.Key, IKeyValuePairObject(Of String, Feature()).Key
        Public Property Features As Feature() Implements IKeyValuePairObject(Of String, Feature()).Value

        Public Overrides Function ToString() As String
            Return LocusTag
        End Function
    End Class

    <Package("Genbank.Export_Genes")>
    Public Module ExportGenes

        <ExportAPI("GET.Genes")>
        <Extension> Public Function GetGenes(gb As File) As GeneObject()
            Dim list As NamedValue(Of String)() = gb.GeneList
            Dim genes = LinqAPI.Exec(Of GeneObject) <=
 _
                From gene
                In list
                Let features As Feature() = (
                    From e
                    In gb.Features._innerList
                    Where String.Equals(e.Query(locus_tag), gene.Name)
                    Select e).ToArray
                Select New GeneObject With {
                    .LocusTag = gene.Name,
                    .Gene = gene.Value,
                    .Features = features
                }

            Return genes
        End Function

        Const locus_tag$ = NameOf(locus_tag)

        <ExportAPI("GET.Gene")>
        <Extension> Public Function GetGene(gb As File, LocusTag As String) As GeneObject
            Dim LQuery = From feature As Feature
                         In gb.Features._innerList
                         Where String.Equals(feature.Query(locus_tag), LocusTag)
                         Select feature '
            Dim List As New List(Of Feature)(LQuery)
            Dim Gene = (From e As Feature In List Where String.Equals(e.KeyName, "gene") Select e).First

            Call List.Remove(Gene)

            Return New GeneObject With {
                .LocusTag = LocusTag,
                .Gene = Gene.Query("gene"),
                .Features = List.ToArray
            }
        End Function
    End Module
End Namespace
