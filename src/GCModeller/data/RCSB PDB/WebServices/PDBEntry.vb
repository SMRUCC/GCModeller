Namespace WebServices

    Public Module PDBEntry

        Public Const PDBENTRY_LIST_FROM_LIGAND_COMPOUND As String = "http://www.ebi.ac.uk/pdbe-srv/pdbechem/PDBEntry/download/{0}"

        Public Function DownloadEntryList(CompoundCode As String) As String()
            Dim PageContent As String = String.Format(PDBENTRY_LIST_FROM_LIGAND_COMPOUND, CompoundCode).GET
            If String.IsNullOrEmpty(PageContent) Then
                Return New String() {}
            End If

            Dim ChunkTemp As String() = Strings.Split(PageContent, vbLf)
            ChunkTemp = ChunkTemp.Skip(2).ToArray
            Return ChunkTemp
        End Function

        Public Const PDBML_NOATOM As String = "http://www.ebi.ac.uk/pdbe/entry-files/download/{0}-noatom.xml"

        Public Function DownloadPDBFile(Entry As String, savedDir As String) As String
            Dim SavedFile As String = String.Format("{0}/{1}.xml", savedDir, Entry)
            Call String.Format(PDBML_NOATOM, Entry).DownloadFile(SavedFile)
            Return SavedFile
        End Function
    End Module
End Namespace