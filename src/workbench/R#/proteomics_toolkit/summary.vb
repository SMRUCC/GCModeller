Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Annotation
Imports SMRUCC.genomics.Annotation.Ptf
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Data.GeneOntology
Imports SMRUCC.genomics.Data.GeneOntology.DAG
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports csvfile = Microsoft.VisualBasic.Data.csv.IO.File
Imports RDataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe

<Package("summary")>
Module summary

    Sub New()
        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(CatalogProfiles), AddressOf asProfileTable)
    End Sub

    Private Function asProfileTable(profiles As CatalogProfiles, args As list, env As Environment) As RDataframe
        Dim type As String = args.getValue("type", env, [default]:="go")
        Dim table As New RDataframe With {.columns = New Dictionary(Of String, Array)}
        Dim file As New csvfile

        Select Case type
            Case "go"
                Dim i As i32 = Scan0

                For Each k As NamedValue(Of CatalogProfile) In profiles.GetProfiles()
                    For Each term As NamedValue(Of Double) In k.Value.AsEnumerable
                        ' {"namespace", "id", "name", "counts"} 
                        Call file.AppendLine(New String() {k.Name, term.Name, term.Description, term.Value})
                    Next
                Next

                table.columns.Add("namespace", file.Column(++i).ToArray)
                table.columns.Add("id", file.Column(++i).ToArray)
                table.columns.Add("name", file.Column(++i).ToArray)
                table.columns.Add("counts", file.Column(++i).Select(AddressOf Val).ToArray)
                table.rownames = table.columns!id

            Case "ko"

                Throw New NotImplementedException

            Case Else
                Throw New NotImplementedException
        End Select

        Return table
    End Function

    <ExportAPI("proteins.GO")>
    <RApiReturn(GetType(CatalogProfiles))>
    Public Function proteinsGOprofiles(<RRawVectorArgument> annotations As Object, goDb As GO_OBO, Optional level% = -1, Optional env As Environment = Nothing) As Object
        Dim ptf As pipeline = pipeline.TryCreatePipeline(Of AnnotationTable)(annotations, env, suppress:=True)

        If ptf.isError Then
            ptf = pipeline.TryCreatePipeline(Of ProteinAnnotation)(annotations, env)

            If ptf.isError Then
                Return ptf.getError
            End If

            ptf = ptf.populates(Of ProteinAnnotation)(env) _
                .Select(AddressOf AnnotationTable.FromUnifyPtf) _
                .DoCall(AddressOf pipeline.CreateFromPopulator)
        End If

        Dim goInfo As AnnotationTable() = ptf _
            .populates(Of AnnotationTable)(env) _
            .Where(Function(prot) Not prot.GO.IsNullOrEmpty) _
            .ToArray
        Dim goTerms As Dictionary(Of String, Term) = goDb.terms.ToDictionary(Function(x) x.id)
        Dim DAG As New Graph(goTerms.Values)
        Dim data = goInfo.CountStat(Function(prot) prot.GO, goTerms)

        If level > 0 Then
            data = data.LevelGOTerms(level, DAG)
        End If

        Return New CatalogProfiles(data)
    End Function
End Module
