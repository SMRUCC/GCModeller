Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports LANS.SystemsBiology.SequenceModel
Imports LANS.SystemsBiology.SequenceModel.NucleotideModels
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.TokenIcer

Namespace Motif.Patterns

    ''' <summary>
    ''' 使用正则表达式来表示序列的模式
    ''' </summary>
    Public Class PatternExpression
        Implements sIdEnumerable

        Public Property RangeExpr As Token(Of Tokens)()

        Public Property Motif As Residue()
            Get
                Return __motif
            End Get
            Set(value As Residue())
                __motif = value
                __regex = New Regex(String.Join("", value.ToArray(Function(x) x.Regex)))
                __rc = New Regex(String.Join("", value.Reverse.ToArray(Function(x) x.GetComplement.Regex)))
            End Set
        End Property

        Public Property Identifier As String Implements sIdEnumerable.Identifier

        Dim __motif As Residue()
        Dim __regex As Regex
        ''' <summary>
        ''' Regex complement reversed.
        ''' </summary>
        Dim __rc As Regex

        Public Function Match(seq As I_PolymerSequenceModel) As SimpleSegment()
            Dim nt As String = seq.SequenceData.ToUpper
            Dim matches = __regex.Matches(nt).ToArray

            Throw New NotImplementedException
        End Function

        Public Overrides Function ToString() As String
            Return String.Join("", Motif.ToArray(Function(x) x.Raw))
        End Function
    End Class
End Namespace