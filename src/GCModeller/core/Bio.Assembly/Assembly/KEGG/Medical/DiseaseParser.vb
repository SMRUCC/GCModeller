
Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

Namespace Assembly.KEGG.Medical

    Public Module DiseaseParser

        Public Iterator Function ParseStream(path$) As IEnumerable(Of Disease)
            Dim lines$() = path.ReadAllLines
            Dim ref As Reference() = Nothing

            For Each pack As String() In lines.Split("///")
                Yield pack.ParseStream(ref).CreateDiseaseModel(ref)
            Next
        End Function

        <Extension>
        Public Function CreateDiseaseModel(getValue As Func(Of String, String()), ref As Reference()) As Disease
            Return New Disease With {
                .References = ref,
                .Entry = getValue("ENTRY").FirstOrDefault.Split.First,
                .Names = getValue("NAME") _
                    .Where(Function(s) Not s.StringEmpty) _
                    .ToArray,
                .Comments = getValue("COMMENT").JoinBy(" "),
                .Carcinogen = getValue("CARCINOGEN"),
                .Category = getValue("CATEGORY").FirstOrDefault,
                .DbLinks = getValue("DBLINKS") _
                    .Select(AddressOf DBLink.FromTagValue) _
                    .ToArray,
                .Description = getValue("DESCRIPTION").FirstOrDefault,
                .Drugs = getValue("DRUG"),
                .Env_factors = getValue("ENV_FACTOR"),
                .Genes = getValue("GENE"),
                .Markers = getValue("MARKER"),
                .Pathogens = getValue("PATHOGEN")
            }
        End Function
    End Module
End Namespace