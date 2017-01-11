Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.Uniprot.Web

Public Module ProteinGroups

    Public Function GetProteinIds(path$, Optional column$ = "Protein IDs") As String()
        Dim idlist$() = path _
            .LoadCsv _
            .GetColumnObjects(column, Function(s) s.Split(";"c)) _
            .Unlist _
            .Distinct _
            .ToArray
        Return idlist
    End Function

    Public Sub GetProteinDefs(path$, Optional save$ = Nothing, Optional column$ = "Protein IDs")
        Dim idData$() = GetProteinIds(path, column)
        Dim gz$ = If(save.IsBlank, path.TrimSuffix & ".xml.gz", save)

        Call Retrieve_IDmapping.Mapping(idData, IdTypes.NF90, IdTypes.ACC, gz)
        Call idData.SaveTo(gz.TrimSuffix.TrimSuffix & "-proteins.txt")
    End Sub
End Module
