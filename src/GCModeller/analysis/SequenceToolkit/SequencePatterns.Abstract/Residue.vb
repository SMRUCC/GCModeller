Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Public Structure Residue

    Public Property frequency As Dictionary(Of Char, Double)
    Public Property index As Integer

    Public ReadOnly Property topChar As Char
        Get
            Return Max(Me)
        End Get
    End Property

    Default Public ReadOnly Property getFrequency(base As Char) As Double
        Get
            Return _frequency(base)
        End Get
    End Property

    Public ReadOnly Property isEmpty As Boolean
        Get
            If frequency.IsNullOrEmpty Then
                Return True
            ElseIf frequency.Values.All(Function(p) p = 0.0) Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Dim max As Double = -99999
        Dim maxChar As Char?

        For Each b In frequency
            If b.Value > max Then
                max = b.Value
                maxChar = b.Key
            End If
        Next

        If maxChar Is Nothing Then
            Return "-"
        ElseIf max >= 0.5 Then
            Return Char.ToUpper(maxChar)
        Else
            Return Char.ToLower(maxChar)
        End If
    End Function

    Public Shared Function GetEmpty() As Residue
        Return New Residue With {
            .frequency = New Dictionary(Of Char, Double),
            .index = -1
        }
    End Function

    Public Shared Function Max(r As Residue) As Char
        With r.frequency.ToArray
            If .Values.All(Function(p) p = 0R) Then
                Return "-"c
            Else
                Return .ByRef(Which.Max(.Values)) _
                       .Key
            End If
        End With
    End Function
End Structure