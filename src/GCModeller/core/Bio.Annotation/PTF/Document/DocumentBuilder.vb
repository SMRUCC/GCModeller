Imports System.IO
Imports System.Runtime.CompilerServices

Namespace Ptf.Document

    Friend Module DocumentBuilder

        Public Sub writeTabular(ptf As PtfFile, output As TextWriter)
            For Each protein As ProteinAnnotation In ptf.proteins
                Call output.WriteLine(protein.asLineText)
            Next
        End Sub

        <Extension>
        Private Function asLineText(protein As ProteinAnnotation) As String
            Dim attrsToStr = protein.attributes _
                .Select(Function(a)
                            Return $"{a.Key}:{a.Value.JoinBy(",")}"
                        End Function) _
                .JoinBy("; ")

            Return $"{protein.geneId}{vbTab}{protein.description}{vbTab}{attrsToStr}"
        End Function
    End Module
End Namespace