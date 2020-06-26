Imports System.IO
Imports Microsoft.VisualBasic.Text

Namespace Ptf.Document

    Friend Module PtfParser

        ''' <summary>
        ''' 读取小文件
        ''' </summary>
        ''' <param name="file"></param>
        ''' <returns></returns>
        Public Function ParseDocument(file As String) As PtfFile
            Dim lines As String() = file.IterateAllLines.ToArray
            Dim headers As String() = lines.TakeWhile(Function(a) a.StartsWith("#")).ToArray
            Dim attributes As Dictionary(Of String, String) = headers _
                .Select(Function(line)
                            Return line.Trim("#"c, " "c).GetTagValue("=")
                        End Function) _
                .ToDictionary(Function(a) a.Name,
                              Function(a)
                                  Return a.Value
                              End Function)

            Return New PtfFile With {
                .attributes = attributes,
                .proteins = lines _
                    .Skip(headers.Length) _
                    .Select(AddressOf ParseAnnotation) _
                    .ToArray
            }
        End Function

        ''' <summary>
        ''' 这个函数主要是用于读取大文件
        ''' </summary>
        ''' <returns></returns>
        Public Iterator Function IterateAnnotations(file As Stream) As IEnumerable(Of ProteinAnnotation)
            Using reader As New StreamReader(file)
                For Each line As String In reader.ReadLine
                    If line.StartsWith("#") Then
                        ' 跳过文件头
                    Else
                        Yield ParseAnnotation(line)
                    End If
                Next
            End Using
        End Function

        Public Function ParseAnnotation(line As String) As ProteinAnnotation
            Dim tokens As String() = line.Split(ASCII.TAB)
            Dim attrs As Dictionary(Of String, String()) = tokens(2) _
                .StringSplit(";\s+") _
                .Select(Function(t) t.GetTagValue(":", trim:=True)) _
                .ToDictionary(Function(a) a.Name,
                              Function(a)
                                  Return a.Value.Split(","c)
                              End Function)

            Return New ProteinAnnotation With {
                .geneId = tokens(Scan0),
                .description = tokens(1),
                .attributes = attrs
            }
        End Function
    End Module
End Namespace