Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Diagnostics
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language

Namespace SourceMap

    Public Module mappingEncode

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="loci"></param>
        ''' <param name="file">target source file after encode</param>
        ''' <returns></returns>
        <Extension>
        Public Function encode(loci As IEnumerable(Of StackFrame), file$) As sourceMap
            Dim lines As New List(Of mappingLine())
            Dim symbols As New Index(Of String)
            Dim files = loci.GroupBy(Function(f) f.File).ToArray
            Dim sourceFile As IGrouping(Of Integer, StackFrame)()
            Dim mapLine As New List(Of mappingLine)

            For i As Integer = 0 To files.Length - 1
                sourceFile = files(i) _
                    .GroupBy(Function(a) Integer.Parse(a.Line)) _
                    .OrderBy(Function(a) a.Key) _
                    .ToArray

                For Each line In sourceFile
                    For Each col As StackFrame In line
                        If Not col.Method.Method Like symbols Then
                            Call symbols.Add(col.Method.Method)
                        End If

                        mapLine += New mappingLine With {
                            .fileIndex = i,
                            .nameIndex = symbols(col.Method.Method),
                            .sourceCol = 1,
                            .sourceLine = line.Key,
                            .targetCol = 1
                        }
                    Next

                    lines += mapLine.PopAll
                Next
            Next

            Return New sourceMap With {
                .version = 3,
                .file = file,
                .sourceRoot = "",
                .sources = files _
                    .Select(Function(filegroup) filegroup.Key) _
                    .ToArray,
                .mappings = lines.Select(Function(line) line.JoinBy(",")).JoinBy(";"),
                .names = symbols.Objects
            }
        End Function
    End Module
End Namespace