Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.PubMed
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

<Package("pubmed")>
Module pubmed

    <ExportAPI("parse")>
    Public Function ParsePubmed(<RRawVectorArgument> text As Object)
        Return CLRVector.asCharacter(text) _
            .Select(Function(si) PubMedServicesExtensions.ParseArticles(si)) _
            .IteratesALL _
            .ToArray
    End Function
End Module
