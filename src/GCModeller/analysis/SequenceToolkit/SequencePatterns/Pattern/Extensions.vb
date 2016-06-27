Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic
Imports System.Runtime.CompilerServices
Imports LANS.SystemsBiology.SequenceModel

Namespace Pattern

    Public Module Extensions

        <ExportAPI("Loci.Find.Location",
                   Info:="Found out all of the loci site on the target sequence.")>
        <Extension>
        Public Function FindLocation(Sequence As I_PolymerSequenceModel, Loci As String) As Integer()
            Return FindLocation(Sequence.SequenceData, Loci)
        End Function

        ''' <summary>
        ''' Found out all of the loci site on the target sequence.
        ''' </summary>
        ''' <param name="Sequence"></param>
        ''' <param name="Loci"></param>
        ''' <returns></returns>
        ''' <remarks>这个位置查找函数是OK的</remarks>
        <ExportAPI("Loci.Find.Location",
                   Info:="Found out all of the loci site on the target sequence.")>
        <Extension>
        Public Function FindLocation(Sequence As String, Loci As String) As Integer()
            Dim Locis = New List(Of Integer)
            Dim p As Integer = 1

            Do While True
                p = InStr(Start:=p, String1:=Sequence, String2:=Loci)
                If p > 0 Then
                    Call Locis.Add(p)
                    p += 1
                Else
                    Exit Do
                End If
            Loop

            Return Locis.ToArray
        End Function
    End Module
End Namespace