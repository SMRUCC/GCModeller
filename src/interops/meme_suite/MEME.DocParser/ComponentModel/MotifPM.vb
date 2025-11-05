#Region "Microsoft.VisualBasic::dd6966fa69b6aabcb6230aca4aef7432, meme_suite\MEME.DocParser\ComponentModel\MotifPM.vb"

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

    '     Class MotifPM
    ' 
    '         Properties: A, Bits, C, G, MostProperly
    '                     T
    ' 
    '         Constructor: (+3 Overloads) Sub New
    ' 
    '         Function: ToString
    ' 
    '         Sub: __setValue
    '         Delegate Function
    ' 
    '             Properties: GetValueMethods, Index
    ' 
    '             Function: __createObject, CreateFromNtBase, CreateObject, EnumerateKeys, EnumerateValues
    '                       ToString
    ' 
    '             Sub: Assign
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.SequenceLogo
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.Patterns

Namespace ComponentModel

    ''' <summary>
    ''' Motif序列之中的一个位点
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MotifPM : Implements ILogoResidue

        Dim _innerTable As Dictionary(Of DNA, Double)

        ''' <summary>
        ''' <see cref="DNA.dAMP"></see>碱基在这个位点之上出现的概率
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property A As Double
            Get
                Return _innerTable(DNA.dAMP)
            End Get
            Set(value As Double)
                Call __setValue(DNA.dAMP, value)
            End Set
        End Property

        Private Sub __setValue(nt As DNA, value As Double)
            If _innerTable.ContainsKey(nt) Then
                Call _innerTable.Remove(nt)
            End If
            Call _innerTable.Add(nt, value)

            MostPossible = LinqAPI.DefaultFirst(Of KeyValuePair(Of DNA, Double)) <=
 _
                From t As KeyValuePair(Of DNA, Double)
                In _innerTable
                Where t.Value > 0
                Select t
                Order By t.Value Descending
        End Sub

        ''' <summary>
        ''' <see cref="DNA.dTMP"></see>碱基在这个位点之上出现的概率
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property T As Double
            Get
                Return _innerTable(DNA.dTMP)
            End Get
            Set(value As Double)
                Call __setValue(DNA.dTMP, value)
            End Set
        End Property

        ''' <summary>
        ''' <see cref="DNA.dGMP"></see>碱基在这个位点之上出现的概率
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property G As Double
            Get
                Return _innerTable(DNA.dGMP)
            End Get
            Set(value As Double)
                Call __setValue(DNA.dGMP, value)
            End Set
        End Property

        ''' <summary>
        ''' <see cref="DNA.dCMP"></see>碱基在这个位点之上出现的概率
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property C As Double
            Get
                Return _innerTable(DNA.dCMP)
            End Get
            Set(value As Double)
                Call __setValue(DNA.dCMP, value)
            End Set
        End Property

        Dim MostPossible As KeyValuePair(Of DNA, Double)

        <XmlAttribute> Public Property Bits As Double Implements ILogoResidue.Bits

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="s">MEME文本文件结果数据之中的概率表之中的一行文本</param>
        ''' <remarks></remarks>
        Friend Sub New(s As String)
            Dim Tokens As String() = LinqAPI.Exec(Of String) <=
 _
                From tt As String
                In s.Split
                Where Not String.IsNullOrEmpty(tt)
                Select tt

            Dim A = Val(Tokens(0))
            Dim T = Val(Tokens(3))
            Dim G = Val(Tokens(2))
            Dim C = Val(Tokens(1))

            _innerTable = New Dictionary(Of DNA, Double) From {
 _
                {DNA.dAMP, A},
                {DNA.dCMP, C},
                {DNA.dGMP, G},
                {DNA.dTMP, T}
            }

            MostPossible = LinqAPI.DefaultFirst(Of KeyValuePair(Of DNA, Double)) <=
                From x As KeyValuePair(Of DNA, Double)
                In _innerTable
                Where x.Value > 0
                Select x
                Order By x.Value Descending
        End Sub

        Sub New()
            _innerTable = New Dictionary(Of DNA, Double)
        End Sub

        Public Sub New(A As Double, T As Double, G As Double, C As Double)
            _innerTable = New Dictionary(Of DNA, Double) From {
 _
                {DNA.dAMP, A},
                {DNA.dCMP, C},
                {DNA.dGMP, G},
                {DNA.dTMP, T}
            }

            MostPossible = LinqAPI.DefaultFirst(Of KeyValuePair(Of DNA, Double)) <=
                From x
                In _innerTable
                Where x.Value > 0
                Select x
                Order By x.Value Descending
        End Sub

        ''' <summary>
        ''' 当前的位点之上最有可能出现的碱基及其概率，即可能出现的概率最高的碱基
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property MostProperly As KeyValuePair(Of DNA, Double)
            Get
                Return MostPossible
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{NucleicAcid.ToString(MostProperly.Key)}   bits:={Bits}"
        End Function

        Public Delegate Function GetValue(PM As MotifPM) As Double

        Public Shared ReadOnly Property GetValueMethods As Dictionary(Of DNA, GetValue) =
            New Dictionary(Of DNA, GetValue) From {
 _
            {DNA.dAMP, Function(pm As MotifPM) pm.A},
            {DNA.dTMP, Function(pm As MotifPM) pm.T},
            {DNA.dGMP, Function(pm As MotifPM) pm.G},
            {DNA.dCMP, Function(pm As MotifPM) pm.C}
        }

        Default Public ReadOnly Property Probability(c As Char) As Double Implements ILogoResidue.Probability
            Get
                Select Case c
                    Case "A"c, "a"c
                        Return A
                    Case "T"c, "t"c
                        Return T
                    Case "G"c, "g"c
                        Return G
                    Case "C"c, "c"c
                        Return Me.C
                    Case Else
                        Throw New Exception(c & " is not a valid residue!")
                End Select
            End Get
        End Property

        Public Property Index As Integer Implements IAddressOf.Address

        Private Sub Assign(address As Integer) Implements IAddress(Of Integer).Assign
            Me.Index = address
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="MAT">长度必须要相等</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateObject(MAT As MotifPM()()) As MotifPM()
            If MAT.Length = 1 Then
                Return MAT.First
            End If

            MAT = MAT.MatrixTransposeIgnoredDimensionAgreement

            Dim LQuery = (From nn In MAT Select __createObject(nn)).ToArray  '因为元素对象之间有先后顺序，所以这里不会使用并行化拓展
            Return LQuery
        End Function

        Private Shared Function __createObject(nn As MotifPM()) As MotifPM
            Dim A# = (From x As MotifPM In nn Select x._innerTable(DNA.dAMP)).Average
            Dim T# = (From x As MotifPM In nn Select x._innerTable(DNA.dTMP)).Average
            Dim G# = (From x As MotifPM In nn Select x._innerTable(DNA.dGMP)).Average
            Dim C# = (From x As MotifPM In nn Select x._innerTable(DNA.dCMP)).Average

            Return New MotifPM(A, T, G, C)
        End Function

        Public Overloads Shared Function ToString(Motifs As MotifPM()) As String
            Return NucleicAcid.ToString((From n As MotifPM In Motifs Select n.MostProperly.Key).ToArray)
        End Function

        Public Shared Function CreateFromNtBase(nt As DNA) As MotifPM
            Select Case nt
                Case DNA.dAMP
                    Return New MotifPM(A:=1, C:=0, G:=0, T:=0)
                Case DNA.dCMP
                    Return New MotifPM(C:=1, T:=0, G:=0, A:=0)
                Case DNA.dGMP
                    Return New MotifPM(G:=1, A:=0, T:=0, C:=0)
                Case DNA.dTMP
                    Return New MotifPM(T:=1, C:=0, A:=0, G:=0)
                Case Else
                    Return New MotifPM(0.25, 0.25, 0.25, 0.25)
            End Select
        End Function

        Public Function EnumerateKeys() As IEnumerable(Of Char) Implements IPatternSite.EnumerateKeys
            Return SequenceModel.NT
        End Function

        Public Function EnumerateValues() As IEnumerable(Of Double) Implements IPatternSite.EnumerateValues
            Return Me._innerTable.Values
        End Function
    End Class
End Namespace
