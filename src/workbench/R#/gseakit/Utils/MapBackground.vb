Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Annotation.Ptf
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports Rdataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe

Module MapBackground

    ''' <summary>
    ''' try to map any terms to KO
    ''' </summary>
    ''' <param name="genes"></param>
    ''' <param name="geneId$"></param>
    ''' <param name="KO$"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    Public Function KOMaps(genes As Object, geneId$, KO$, env As Environment) As Object
        If TypeOf genes Is list Then
            If KO.StringEmpty OrElse geneId.StringEmpty Then
                Return DirectCast(genes, list).slots _
                    .Select(Function(t)
                                Return New NamedValue(Of String) With {
                                    .Name = t.Key,
                                    .Value = Scripting.ToString([single](t.Value))
                                }
                            End Function) _
                    .ToArray
            Else
                Return DirectCast(genes, list).slots.Values _
                    .Select(Function(map)
                                Dim id As String = DirectCast(map, list).getValue(Of String)(geneId, env)
                                Dim koId As String = DirectCast(map, list).getValue(Of String)(KO, env)

                                Return New NamedValue(Of String)(id, koId)
                            End Function) _
                    .ToArray
            End If
        ElseIf TypeOf genes Is Rdataframe Then
            Dim idVec As String() = DirectCast(genes, Rdataframe).columns(geneId)
            Dim koVec As String() = DirectCast(genes, Rdataframe).columns(KO)

            Return idVec _
                .Select(Function(id, i)
                            Return New NamedValue(Of String) With {
                                .Name = id,
                                .Value = koVec(i)
                            }
                        End Function) _
                .ToArray
        ElseIf TypeOf genes Is EntityObject() Then
            Return DirectCast(genes, EntityObject()) _
                .Select(Function(row)
                            Return New NamedValue(Of String) With {
                                .Name = row(geneId),
                                .Value = row(KO)
                            }
                        End Function) _
                .ToArray
        ElseIf TypeOf genes Is PtfFile OrElse
               TypeOf genes Is ProteinAnnotation() OrElse
              (TypeOf genes Is pipeline AndAlso DirectCast(genes, pipeline).elementType Like GetType(ProteinAnnotation)) Then

            Dim prot As ProteinAnnotation()

            If TypeOf genes Is PtfFile Then
                prot = DirectCast(genes, PtfFile).proteins
            ElseIf TypeOf genes Is pipeline Then
                prot = DirectCast(genes, pipeline).populates(Of ProteinAnnotation)(env).ToArray
            Else
                prot = DirectCast(genes, ProteinAnnotation())
            End If

            Return prot.Where(Function(p) p.has("ko")) _
                .Select(Function(protein)
                            Return protein.attributes("ko") _
                                .Select(Function(koid)
                                            Return New NamedValue(Of String) With {
                                                .Name = protein.geneId,
                                                .Value = koid,
                                                .Description = protein.description
                                            }
                                        End Function)
                        End Function) _
                .IteratesALL _
                .ToArray
        ElseIf TypeOf genes Is pipeline AndAlso DirectCast(genes, pipeline).elementType Like GetType(entry) Then
            Dim entrylist As entry() = DirectCast(genes, pipeline).populates(Of entry)(env).ToArray
            Dim maps As NamedValue(Of String)() = entrylist _
                .Where(Function(i)
                           Return Not i.KO Is Nothing
                       End Function) _
                .Select(Function(i)
                            Return New NamedValue(Of String) With {
                                .Name = i.accessions(Scan0),
                                .Value = i.KO.id,
                                .Description = i.name
                            }
                        End Function) _
                .ToArray

            Return maps
        Else
            Return Internal.debug.stop(New InvalidProgramException(genes.GetType.FullName), env)
        End If
    End Function
End Module
