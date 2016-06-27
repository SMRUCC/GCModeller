Namespace GCModeller.FileSystem.Xfam

    Module Rfam

        Public ReadOnly Property Rfam As String
            Get
                Return RepositoryRoot & "/Xfam/Rfam"
            End Get
        End Property

        Public ReadOnly Property RfamFasta As String
            Get
                Return Rfam & "/Fasta/"
            End Get
        End Property
    End Module
End Namespace