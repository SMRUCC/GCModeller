Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language

Public Class phenotype

    Public Property labels As Integer()
    Public Property phnameA As String
    Public Property phnameB As String
    Public Property sample_num As Integer
    Public Property sample0 As Integer
    Public Property sample1 As Integer

    ''' <summary>
    ''' read_cls_file
    ''' </summary>
    ''' <param name="cls"></param>
    Sub New(cls As CLS)
        If cls.numOfClasses <> 2 Then
            Throw New Exception("Sorry, only two categorical phenotype labels are allowed.")
        ElseIf cls.sampleClass.Length <> cls.numOfSamples Then
            Throw New Exception("The total of phenotype labels doesn't match with the total of samples as you described at the first line. Please check this cls file.")
        End If

        Dim labels As New List(Of Integer)

        sample_num = cls.numOfSamples

        With cls.classNameMaps.ToList
            phnameA = .Item(0).Value
            phnameB = .Item(1).Value
        End With

        For Each label As String In cls.sampleClass
            If label = phnameA Then
                labels += 0
                sample0 += 1
            Else
                labels += 1
                sample1 += 1
            End If
        Next

        Me.labels = labels
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromClsFile(path As String) As phenotype
        Return New phenotype(CLS.ParseFile(path))
    End Function
End Class
