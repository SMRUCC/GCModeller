﻿#Region "Microsoft.VisualBasic::9ad21c7a4e0556987688b722158da9cb, analysis\Microarray\Enrichment\KOBAS\EnrichmentTerm.vb"

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

    '     Class EnrichmentTerm
    ' 
    '         Properties: Backgrounds, CorrectedPvalue, Database, ID, Input
    '                     link, number, ORF, Pvalue, Term
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace KOBAS

    Public Class EnrichmentTerm
        Implements IGoTerm
        Implements IGoTermEnrichment
        Implements IKEGGTerm
        Implements INamedValue

        ''' <summary>
        ''' #Term
        ''' </summary>
        ''' <returns></returns>
        <Column("#Term")>
        Public Property Term As String Implements IKEGGTerm.Term
        Public Property Database As String

        ''' <summary>
        ''' <see cref="INamedValue.Key"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property ID As String Implements IGoTerm.Go_ID, IKEGGTerm.ID, INamedValue.Key

        ''' <summary>
        ''' Input number
        ''' </summary>
        ''' <returns></returns>
        <Column("Input number")> Public Property number As Integer

        ''' <summary>
        ''' Background number
        ''' </summary>
        ''' <returns></returns>
        <Column("Background number")> Public Property Backgrounds As Integer

        ''' <summary>
        ''' P-Value
        ''' </summary>
        ''' <returns></returns>
        <Column("P-Value")> Public Property Pvalue As Double Implements IGoTermEnrichment.Pvalue, IKEGGTerm.Pvalue

        ''' <summary>
        ''' Corrected P-Value
        ''' </summary>
        ''' <returns></returns>
        <Column("Corrected P-Value")> Public Property CorrectedPvalue As Double Implements IGoTermEnrichment.CorrectedPvalue

        ''' <summary>
        ''' The group of this input gene id list
        ''' </summary>
        ''' <returns></returns>
        Public Property Input As String
        Public Property ORF As String() Implements IKEGGTerm.ORF

        ''' <summary>
        ''' 用于一些可视化的超链接url
        ''' </summary>
        ''' <returns></returns>
        <Column("Hyperlink")>
        Public Property link As String Implements IKEGGTerm.Link

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
