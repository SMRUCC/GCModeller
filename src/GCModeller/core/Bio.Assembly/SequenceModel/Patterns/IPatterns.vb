#Region "Microsoft.VisualBasic::acc435d8da0866f21d1af4d0d232bdaa, core\Bio.Assembly\SequenceModel\Patterns\IPatterns.vb"

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

    '   Total Lines: 113
    '    Code Lines: 78 (69.03%)
    ' Comment Lines: 13 (11.50%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 22 (19.47%)
    '     File Size: 4.16 KB


    '     Interface IPatternProvider
    ' 
    '         Properties: Site
    ' 
    '         Function: PWM
    ' 
    '     Interface IPatternSite
    ' 
    '         Properties: Probability
    ' 
    '         Function: EnumerateKeys, EnumerateValues
    ' 
    '     Structure SimpleSite
    ' 
    '         Properties: Address, Alphabets, IsConserved
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: EnumerateKeys, EnumerateValues, ToString
    ' 
    '         Sub: Assign
    ' 
    '     Structure PatternModel
    ' 
    '         Properties: Residues
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetVariation, PWM, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace SequenceModel.Patterns

    Public Interface IPatternProvider
        Default ReadOnly Property Site(i As Integer) As IPatternSite
        Function PWM() As IEnumerable(Of IPatternSite)
    End Interface

    Public Interface IPatternSite : Inherits IAddressOf

        Default ReadOnly Property Probability(c As Char) As Double

        ''' <summary>
        ''' get alphabets of current site
        ''' </summary>
        ''' <returns></returns>
        Function EnumerateKeys() As IEnumerable(Of Char)
        ''' <summary>
        ''' get the pwm data column
        ''' </summary>
        ''' <returns></returns>
        Function EnumerateValues() As IEnumerable(Of Double)
    End Interface

    Public Structure SimpleSite
        Implements IPatternSite

        Public ReadOnly Property Alphabets As Dictionary(Of Char, Double)

        ''' <summary>
        ''' Is this residue conserved in this motif?
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsConserved As Boolean
            Get
                For Each x As Double In Alphabets.Values
                    If x = 1.0R Then
                        Return True
                    End If
                Next

                Return False
            End Get
        End Property

        Default Public ReadOnly Property Probability(c As Char) As Double Implements IPatternSite.Probability
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return _Alphabets(c)
            End Get
        End Property

        Public Property Address As Integer Implements IAddressOf.Address

        Sub New(f As Dictionary(Of Char, Double), i As Integer)
            Alphabets = f
            Address = i
        End Sub

        Public Overrides Function ToString() As String
            Return Alphabets.GetJson
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function EnumerateKeys() As IEnumerable(Of Char) Implements IPatternSite.EnumerateKeys
            Return Alphabets.Keys
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function EnumerateValues() As IEnumerable(Of Double) Implements IPatternSite.EnumerateValues
            Return Alphabets.Values
        End Function

        Public Sub Assign(address As Integer) Implements IAddress(Of Integer).Assign
            Throw New NotImplementedException()
        End Sub
    End Structure

    ''' <summary>
    ''' 一个经过多序列比对对齐操作的序列集合之中所得到的残基的出现频率模型，可以用这个模型来计算突变率以及SNP位点
    ''' </summary>
    Public Structure PatternModel : Implements IPatternProvider, Enumeration(Of SimpleSite)

        Public ReadOnly Property Residues As SimpleSite()

        Default Public ReadOnly Property Site(i As Integer) As IPatternSite Implements IPatternProvider.Site
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Residues(i)
            End Get
        End Property

        Sub New(rs As IEnumerable(Of SimpleSite))
            Residues = rs.ToArray
        End Sub

        ''' <summary>
        ''' 通过比较参考序列得到每一个残基上面的突变率
        ''' </summary>
        ''' <param name="ref">序列的长度必须要和<see cref="Residues"/>的长度相等</param>
        ''' <param name="cutoff"></param>
        ''' <returns></returns>
        Public Function GetVariation(ref As FASTA.FastaSeq, cutoff As Double) As Double()
            Dim refs As Char() = ref.SequenceData.ToUpper.ToCharArray
            Return Residues.Select(Function(x, i) x.Variation(refs(i), cutoff)).ToArray
        End Function

        Public Iterator Function PWM() As IEnumerable(Of IPatternSite) Implements IPatternProvider.PWM
            For Each x As SimpleSite In Residues
                Yield x
            Next
        End Function

        Public Overrides Function ToString() As String
            Return GetJson
        End Function

        Public Iterator Function GenericEnumerator() As IEnumerator(Of SimpleSite) Implements Enumeration(Of SimpleSite).GenericEnumerator
            For Each x As SimpleSite In Residues
                Yield x
            Next
        End Function
    End Structure
End Namespace
