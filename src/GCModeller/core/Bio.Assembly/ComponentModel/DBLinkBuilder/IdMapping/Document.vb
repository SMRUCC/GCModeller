Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text

Namespace ComponentModel.DBLinkBuilder

    <HideModuleName> Public Module Document

        <Extension>
        Public Function Save(maps As SecondaryIDSolver, path As String) As Boolean
            Using output As New StreamWriter(path.Open(, doClear:=True), Encodings.ASCII.CodePage) With {
                .NewLine = ASCII.LF
            }
                For Each map In maps.idMapping
                    Call output.WriteLine($"{map.Key} {map.Value.JoinBy(",")}")
                Next
            End Using

            Return True
        End Function

        Public Function LoadMappingText(handle As String, Optional skip2ndMaps As Boolean = False) As SecondaryIDSolver
            Return handle _
                .LineIterators _
                .DoCall(Function(mapLines)
                            Return SecondaryIDSolver.Create(
                                source:=mapLines.Select(Function(l) l.Split(" "c)),
                                mainID:=Function(a) a(Scan0),
                                secondaryID:=Function(a) a(1).Split(","c),
                                skip2ndMaps:=skip2ndMaps
                            )
                        End Function)
        End Function
    End Module
End Namespace