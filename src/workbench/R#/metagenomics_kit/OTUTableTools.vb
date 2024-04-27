#Region "Microsoft.VisualBasic::cecfa8fef07a6ff47476dc632a28b5c7, G:/GCModeller/src/workbench/R#/metagenomics_kit//OTUTableTools.vb"

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
    '    Code Lines: 41
    ' Comment Lines: 33
    '   Blank Lines: 6
    '     File Size: 3.09 KB


    ' Module OTUTableTools
    ' 
    '     Function: filter, relativeAbundance
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.Rsharp.Runtime.Interop

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
End Module
