Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq

Namespace ComponentModel.Chemical

    ''' <summary>
    ''' The formula composition data model
    ''' </summary>
    Public Structure FormulaData

        Dim elements As Dictionary(Of String, Integer)

        Public Shared ReadOnly Property Empty As FormulaData
            Get
                Return New FormulaData(New Dictionary(Of String, Integer))
            End Get
        End Property

        Sub New(C%, H%, O%, N%, Optional S% = 0)
            elements = New Dictionary(Of String, Integer)
            elements!C = C
            elements!H = H
            elements!O = O
            elements!N = N
            elements!S = S
        End Sub

        Sub New(count As Dictionary(Of String, Integer))
            elements = count
        End Sub

        Public Function Add(atom As String, Optional n As Integer = 1) As FormulaData
            If elements Is Nothing Then
                elements = New Dictionary(Of String, Integer) From {{atom, n}}
            Else
                elements(atom) = n
            End If

            Return Me
        End Function

        ''' <summary>
        ''' return formula string
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder

            For Each atom As KeyValuePair(Of String, Integer) In elements
                If atom.Value = 1 Then
                    sb.Append(atom.Key)
                ElseIf atom.Value <= 0 Then
                    ' do nothing
                Else
                    sb.Append(atom.Key & atom.Value)
                End If
            Next

            Return sb.ToString
        End Function

        Public Shared ReadOnly H2O As New FormulaData(0, H:=2, O:=1, 0)

        Public Shared Operator +(a As FormulaData, b As FormulaData) As FormulaData
            Dim sum = a.elements.JoinIterates(b.elements) _
                .GroupBy(Function(atom) atom.Key) _
                .ToDictionary(Function(atom) atom.Key,
                              Function(atom)
                                  Return atom.Values.Sum
                              End Function)

            Return New FormulaData(sum)
        End Operator

        Public Shared Operator *(a As FormulaData, n As Integer) As FormulaData
            Dim count As Dictionary(Of String, Integer) = a.elements _
                .ToDictionary(Function(atom) atom.Key,
                              Function(atom)
                                  Return atom.Value * n
                              End Function)

            Return New FormulaData(count)
        End Operator

        Public Shared Operator -(a As FormulaData, b As FormulaData) As FormulaData
            Dim count As New Dictionary(Of String, Integer)(a.elements)

            For Each atom In b.elements
                If count.ContainsKey(atom.Key) Then
                    count(atom.Key) -= atom.Value

                    If count(atom.Key) <= 0 Then
                        count.Remove(atom.Key)
                    End If
                End If
            Next

            Return New FormulaData(count)
        End Operator

        Public Shared Operator /(a As FormulaData, n As Integer) As FormulaData
            Dim count As Dictionary(Of String, Integer) = a.elements _
                .ToDictionary(Function(atom) atom.Key,
                              Function(atom)
                                  Return CInt(atom.Value / n)
                              End Function)

            For Each atom As String In count.Keys.ToArray
                If count(atom) <= 0 Then
                    Call count.Remove(atom)
                End If
            Next

            Return New FormulaData(count)
        End Operator

    End Structure

End Namespace