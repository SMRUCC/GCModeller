﻿#Region "Microsoft.VisualBasic::84a109f1d08c6fa9439d9c1f60556d7d, G:/GCModeller/src/GCModeller/analysis/SequenceToolkit/MotifScanner//Consensus/ModelLoader.vb"

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

    '   Total Lines: 69
    '    Code Lines: 47
    ' Comment Lines: 11
    '   Blank Lines: 11
    '     File Size: 2.69 KB


    ' Module ModelLoader
    ' 
    '     Function: GetUpstreams, LoadGenomic, LoadKEGGModels
    ' 
    ' Structure genomic
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ContextModel.Promoter
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.NCBI
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module ModelLoader

    ''' <summary>
    ''' Load kegg organism model
    ''' </summary>
    ''' <param name="repo"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function LoadKEGGModels(repo As String) As Dictionary(Of String, OrganismModel)
        Return (ls - l - r - "*.Xml" <= repo) _
            .Select(AddressOf LoadXml(Of OrganismModel)) _
            .ToDictionary(Function(org)

                              ' 2018-2-10 因为这两个物种都具有相同的Taxonomy编号，所以在这里就不适合使用taxon_id来作为唯一标识符了
                              '
                              ' TAX: 611301
                              ' Xanthomonas citri subsp. citri mf20
                              ' TAX: 611301
                              ' Xanthomonas citri subsp. citri MN10

                              Return org.GetGenbankSource
                          End Function)
    End Function

    Public Iterator Function LoadGenomic(assembly$, kegg$) As IEnumerable(Of genomic)
        Dim models = LoadKEGGModels(repo:=kegg)

        For Each genome As (name$, org As OrganismModel) In models.EnumerateTuples
            Dim name$ = genome.name
            Dim search$ = RepositoryExtensions.GetAssemblyPath(assembly, name)
            Dim gb As GBFF.File = RepositoryExtensions.GetGenomeData(gb:=search)
            Dim nt As FastaSeq = gb.Origin.ToFasta

            Yield New genomic With {
                .nt = nt,
                .organism = genome.org,
                .context = gb.GbffToPTT(ORF:=True)
            }
        Next
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function GetUpstreams(genome As genomic, length As PrefixLength) As Dictionary(Of String, FastaSeq)
        Return New PromoterRegionParser(genome.nt, PTT:=genome.context).GetRegionCollectionByLength(length)
    End Function
End Module

Public Structure genomic

    Dim nt As FastaSeq
    Dim organism As OrganismModel
    Dim context As PTT

    Public Overrides Function ToString() As String
        Return organism.ToString
    End Function
End Structure
