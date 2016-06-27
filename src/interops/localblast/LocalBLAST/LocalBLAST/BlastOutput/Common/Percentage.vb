Imports System.Text.RegularExpressions
Imports System.Xml.Serialization

Namespace LocalBLAST.BLASTOutput.ComponentModel

    ''' <summary>
    ''' 分数，百分比
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure Percentage

        ''' <summary>
        ''' 分子
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Numerator As Double
        ''' <summary>
        ''' 分母
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Denominator As Double

        ''' <summary>
        ''' <see cref="Numerator"></see>/<see cref="Denominator"></see>
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Value As Double
            Get
                Return Numerator / Denominator
            End Get
        End Property

        Public ReadOnly Property FractionExpr As String
            Get
                Return $"{Numerator}/{Denominator}"
            End Get
        End Property

        Sub New(n As Double, d As Double)
            Numerator = n
            Denominator = d
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("{0}/{1} ({2}%)", Numerator, Denominator, Value)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Text">\d+[/]\d+ \(\d+[%]\)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function TryParse(Text As String) As Percentage
            If String.IsNullOrEmpty(Text) Then Return ZERO

            Dim Matchs As String() = Regex.Matches(Text, "\d+").ToArray
            Return New Percentage With {
                .Numerator = Matchs(0).RegexParseDouble,
                .Denominator = Matchs(1).RegexParseDouble
            }
        End Function

        Public Shared ReadOnly Property ZERO As Percentage
            Get
                Return New Percentage(0, 1)
            End Get
        End Property

        Public Shared Narrowing Operator CType(value As Percentage) As Double
            Return value.Value
        End Operator

        Public Shared Operator >(value As Percentage, n As Double) As Boolean
            Return value.Value > n
        End Operator

        Public Shared Operator <(value As Percentage, n As Double) As Boolean
            Return value.Value < n
        End Operator
    End Structure
End Namespace