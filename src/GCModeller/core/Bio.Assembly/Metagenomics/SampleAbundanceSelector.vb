#Region "Microsoft.VisualBasic::7877b5001c474ec7d191ad72738c029f, core\Bio.Assembly\Metagenomics\SampleAbundanceSelector.vb"

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

    '     Class SampleAbundanceSelector
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: SelectSamples
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Namespace Metagenomics

    Public Class SampleAbundanceSelector(Of T)

        Dim taxonomyAbundance As (taxonomy As Taxonomy, T)()

        Default Public ReadOnly Property Abundances(taxonomy As Taxonomy()) As KeyValuePair(Of Taxonomy, T())()
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return SelectSamples(taxonomy)
            End Get
        End Property

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Sub New(abundance As IEnumerable(Of (taxonomy As Taxonomy, T)))
            taxonomyAbundance = abundance.ToArray
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Sub New(abundance As IEnumerable(Of KeyValuePair(Of Taxonomy, T)))
            taxonomyAbundance = abundance _
                .Select(Function(tax) (tax.Key, tax.Value)) _
                .ToArray
        End Sub

        ''' <summary>
        ''' Select samples' taxonomy abundance by a given list of taxonomy.
        ''' </summary>
        ''' <param name="taxs"></param>
        ''' <returns>
        ''' 返回来的结果的长度以及物种信息是和函数的参数长度一致的
        ''' </returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function SelectSamples(taxs As IEnumerable(Of Taxonomy)) As KeyValuePair(Of Taxonomy, T())()
            Return taxs _
                .Select(Function(tax As Taxonomy)
                            Dim list = taxonomyAbundance _
                                .SelectByTaxonomyRange(tax, Function(m) m.taxonomy, Function(m) m) _
                                .Select(Function(s) s.Item2) _
                                .ToArray

                            Return New KeyValuePair(Of Taxonomy, T())(tax, list)
                        End Function) _
                .ToArray
        End Function
    End Class
End Namespace
