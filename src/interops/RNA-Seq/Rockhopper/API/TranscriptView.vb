#Region "Microsoft.VisualBasic::d91fd06830c06eb1b6ab15392350d16b, RNA-Seq\Rockhopper\API\TranscriptView.vb"

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

    ' Class TranscriptView
    ' 
    '     Properties: ATGDistance, Category, COG, Coverage, First
    '                 GeneId, gpStart, gpStop, gStrand, Product
    '                 pStart, pStop, Replicon, Strand, Subcategory
    '                 Target, TSS_id, Type
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection.Reflector

Public Class TranscriptView
#Region "TSS INFORMATION"

    Public Property TSS_id As String
#Region "Position"
    Public Property pStart As Integer
    Public Property pStop As Integer
#End Region
    Public Property Strand As String
    Public Property Replicon As String

    Public Property Category As String 'Rockhopper.AnalysisAPI.Transcripts.Categories
    ''' <summary>
    ''' Subcategory Of asRNA
    ''' </summary>
    ''' <returns></returns>
    <Column("Subcategory of asRNA")> Public Property Subcategory As String
    ''' <summary>
    ''' First 3 nt
    ''' </summary>
    ''' <returns></returns>
    <Column("First 3 nt")> Public Property First As String
    ''' <summary>
    ''' Distance To start codon
    ''' </summary>
    ''' <returns></returns>
    <Column("Distance to start codon")> Public Property ATGDistance As String
    Public Property Coverage As String
#End Region
#Region "ASSOCIATED GENE"
    <Column("Gene id")> Public Property GeneId As String
    ''' <summary>
    ''' Type Of gene	
    ''' </summary>
    ''' <returns></returns>
    <Column("Type of gene")> Public Property Type As String
#Region "Position"
    Public Property gpStart As Integer
    Public Property gpStop As Integer
#End Region
    ''' <summary>
    ''' Gene product (protein-coding genes)
    ''' </summary>
    ''' <returns></returns>
    Public Property Product As String
    Public Property COG As String
    ''' <summary>
    ''' Target gene Of asRNA
    ''' </summary>
    ''' <returns></returns>
    <Column("Target gene of asRNA")> Public Property Target As String

    Public Property gStrand As String
#End Region
End Class
