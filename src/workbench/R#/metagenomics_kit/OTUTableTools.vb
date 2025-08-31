﻿#Region "Microsoft.VisualBasic::cecfa8fef07a6ff47476dc632a28b5c7, R#\metagenomics_kit\OTUTableTools.vb"

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

'   Total Lines: 80
'    Code Lines: 41 (51.25%)
' Comment Lines: 33 (41.25%)
'    - Xml Docs: 87.88%
' 
'   Blank Lines: 6 (7.50%)
'     File Size: 3.09 KB


' Module OTUTableTools
' 
'     Function: filter, relativeAbundance
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.Metagenomics
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports Matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports rdataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports Taxonomy = SMRUCC.genomics.Metagenomics.Taxonomy
Imports Vector = Microsoft.VisualBasic.Math.LinearAlgebra.Vector

''' <summary>
''' Tools for handling OTU table data
''' </summary>
''' <remarks>
''' ### Operational taxonomic unit (OTU)
''' 
''' OTU's are used to categorize bacteria based on sequence similarity.
''' 
''' In 16S metagenomics approaches, OTUs are cluster of similar sequence variants of the 
''' 16S rDNA marker gene sequence. Each of these cluster is intended to represent a 
''' taxonomic unit of a bacteria species or genus depending on the sequence similarity 
''' threshold. Typically, OTU cluster are defined by a 97% identity threshold of the 16S 
''' gene sequences to distinguish bacteria at the genus level.
'''
''' Species separation requires a higher threshold Of 98% Or 99% sequence identity, Or 
''' even better the use Of exact amplicon sequence variants (ASV) instead Of OTU sequence 
''' clusters.
''' </remarks>
<Package("OTU_table")>
<RTypeExport("OTU_table", GetType(OTUTable))>
Module OTUTableTools

    ''' <summary>
    ''' Transform abundance data in an otu_table to relative abundance, sample-by-sample. 
    ''' 
    ''' Transform abundance data into relative abundance, i.e. proportional data. This is 
    ''' an alternative method of normalization and may not be appropriate for all datasets,
    ''' particularly if your sequencing depth varies between samples.
    ''' </summary>
    ''' <param name="x"></param>
    ''' <returns></returns>
    <ExportAPI("relative_abundance")>
    <RApiReturn(GetType(OTUTable))>
    Public Function relativeAbundance(x As OTUTable()) As Object
        Dim sample_ids As String() = x _
            .Select(Function(otu) otu.Properties.Keys) _
            .IteratesALL _
            .Distinct _
            .ToArray
        Dim v As Vector

        For Each name As String In sample_ids
            v = x.Select(Function(otu) otu(name)).AsVector
            v = v / v.Sum

            For i As Integer = 0 To x.Length - 1
                x(i)(name) = v(i)
            Next
        Next

        Return x
    End Function

    ''' <summary>
    ''' filter the otu data which has relative abundance greater than the given threshold
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="relative_abundance"></param>
    ''' <returns></returns>
    <ExportAPI("filter")>
    <RApiReturn(GetType(OTUTable))>
    Public Function filter(x As OTUTable(), relative_abundance As Double) As Object
        Return x _
            .Where(Function(otu)
                       Return otu.Properties _
                          .Values _
                          .Any(Function(xi)
                                   Return xi > relative_abundance
                               End Function)
                   End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' read 16s OTU table
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="sumDuplicated"></param>
    ''' <returns></returns>
    <ExportAPI("read.OTUtable")>
    Public Function readOTUTable(file As String,
                                 Optional sumDuplicated As Boolean = True,
                                 Optional OTUTaxonAnalysis As Boolean = False) As OTUTable()
        Dim otus As OTUTable()

        If OTUTaxonAnalysis Then
            otus = OTU _
                .LoadOTUTaxonAnalysis(file, tsv:=Not file.ExtensionSuffix("csv")) _
                .ToArray
        Else
            otus = file.LoadCsv(Of OTUTable)(mute:=True).ToArray
        End If

        If sumDuplicated Then
            Return OTUTable.SumDuplicatedOTU(otus).ToArray
        Else
            Return otus
        End If
    End Function

    ''' <summary>
    ''' cast the expression matrix to the otu data
    ''' </summary>
    ''' <param name="x">
    ''' an expression matrix which use the biom taxonomy string as feature unique id reference.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("otu_from_matrix")>
    Public Function fromMatrix(x As Matrix) As OTUData(Of Double)()
        Return x.FromExpressionMatrix.ToArray
    End Function

    ''' <summary>
    ''' Create expression matrix data from a given otu table
    ''' </summary>
    ''' <param name="otu_table"></param>
    ''' <returns></returns>
    <ExportAPI("as.hts_matrix")>
    <RApiReturn(GetType(Matrix))>
    Public Function cast_matrix(otu_table As OTUTable()) As Object
        Return otu_table.CastMatrix
    End Function

    ''' <summary>
    ''' convert the mothur rank tree as the OTU table
    ''' </summary>
    ''' <param name="x"></param>
    ''' <returns></returns>
    <ExportAPI("as.OTU_table")>
    <RApiReturn(GetType(OTUTable))>
    Public Function asOTUTable(x As Object,
                               Optional id As String = "OTU_num",
                               Optional taxonomy As String = "taxonomy",
                               Optional env As Environment = Nothing) As Object

        If x Is Nothing Then
            Return Nothing
        End If

        If TypeOf x Is MothurRankTree Then
            Return DirectCast(x, MothurRankTree).GetOTUTable
        ElseIf TypeOf x Is rdataframe Then
            Return asOTUTable(DirectCast(x, rdataframe), id, taxonomy)
        Else
            Return Message.InCompatibleType(GetType(MothurRankTree), x.GetType, env)
        End If
    End Function

    ''' <summary>
    ''' convert the dataframe object to OTU table
    ''' </summary>
    ''' <param name="table"></param>
    ''' <param name="id"></param>
    ''' <param name="taxonomy"></param>
    ''' <returns></returns>
    Public Function asOTUTable(table As rdataframe,
                               Optional id As String = "OTU_num",
                               Optional taxonomy As String = "taxonomy") As OTUTable()

        Dim unique_id As String() = CLRVector.asCharacter(table.getColumnVector(id))
        Dim taxonomyStr As Taxonomy() = CLRVector.asCharacter(table.getColumnVector(taxonomy)) _
            .Select(Function(str) New Taxonomy(BIOMTaxonomy.TaxonomyParser(str))) _
            .ToArray

        Call table.columns.Remove(id)
        Call table.columns.Remove(taxonomy)

        Dim samples = table.forEachRow.ToArray
        Dim sample_ids As String() = table.colnames

        Return unique_id _
            .Select(Function(tax, i)
                        Return New OTUTable With {
                            .ID = tax,
                            .taxonomy = taxonomyStr(i),
                            .Properties = samples(i) _
                                .Select(Function(sample, j) (sample, sample_ids(j))) _
                                .ToDictionary(Function(a) a.Item2,
                                              Function(a)
                                                  Return Val(a.Item1)
                                              End Function)
                        }
                    End Function) _
            .ToArray
    End Function
End Module
