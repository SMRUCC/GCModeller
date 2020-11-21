Imports Microsoft.VisualBasic.CommandLine.Reflection
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

<Package("summary")>
Module summary

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
