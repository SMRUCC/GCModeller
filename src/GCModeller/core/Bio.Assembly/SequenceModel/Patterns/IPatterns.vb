Imports System.Runtime.CompilerServices
Imports System.Text
Imports LANS.SystemsBiology.SequenceModel.FASTA
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace SequenceModel.Patterns

    Public Interface IPatternProvider
        Default ReadOnly Property Site(i As Integer) As IPatternSite
        Function PWM() As IEnumerable(Of IPatternSite)
    End Interface

    Public Interface IPatternSite : Inherits IAddressHandle
        Default ReadOnly Property Probability(c As Char) As Double

        Function EnumerateKeys() As IEnumerable(Of Char)
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
            Get
                Return _Alphabets(c)
            End Get
        End Property

        Public Property Address As Integer Implements IAddressHandle.Address

        Sub New(f As Dictionary(Of Char, Double), i As Integer)
            Alphabets = f
            Address = i
        End Sub

        Public Overrides Function ToString() As String
            Return Alphabets.GetJson
        End Function

        Public Function EnumerateKeys() As IEnumerable(Of Char) Implements IPatternSite.EnumerateKeys
            Return Alphabets.Keys
        End Function

        Public Function EnumerateValues() As IEnumerable(Of Double) Implements IPatternSite.EnumerateValues
            Return Alphabets.Values
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
        End Sub
    End Structure

    ''' <summary>
    ''' 一个经过多序列比对对齐操作的序列集合之中所得到的残基的出现频率模型，可以用这个模型来计算突变率以及SNP位点
    ''' </summary>
    Public Structure PatternModel : Implements IPatternProvider

        Public ReadOnly Property Residues As SimpleSite()

        Default Public ReadOnly Property Site(i As Integer) As IPatternSite Implements IPatternProvider.Site
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
        Public Function GetVariation(ref As FASTA.FastaToken, cutoff As Double) As Double()
            Dim refs As Char() = ref.SequenceData.ToUpper.ToCharArray
            Return Residues.ToArray(Function(x, i) x.Variation(refs(i), cutoff))
        End Function

        Public Iterator Function PWM() As IEnumerable(Of IPatternSite) Implements IPatternProvider.PWM
            For Each x As SimpleSite In Residues
                Yield x
            Next
        End Function

        Public Overrides Function ToString() As String
            Return GetJson
        End Function
    End Structure
End Namespace