Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.Metagenome.Kmers
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

<Package("kmers")>
Module KmersTool

    Sub Main()
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(SequenceHit()), AddressOf hitTable)
    End Sub

    <RGenericOverloads("as.data.frame")>
    Private Function hitTable(hits As SequenceHit(), args As list, env As Environment) As dataframe
        Dim df As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }

        Call df.add(NameOf(SequenceHit.reads_title), From h As SequenceHit In hits Select h.reads_title)
        Call df.add(NameOf(SequenceHit.id), From h As SequenceHit In hits Select h.id)
        Call df.add(NameOf(SequenceHit.ncbi_taxid), From h As SequenceHit In hits Select h.ncbi_taxid)
        Call df.add(NameOf(SequenceHit.accession_id), From h As SequenceHit In hits Select h.accession_id)
        Call df.add(NameOf(SequenceHit.name), From h As SequenceHit In hits Select h.name)
        Call df.add(NameOf(SequenceHit.identities), From h As SequenceHit In hits Select h.identities)
        Call df.add(NameOf(SequenceHit.ratio), From h As SequenceHit In hits Select h.ratio)
        Call df.add(NameOf(SequenceHit.total), From h As SequenceHit In hits Select h.total)
        Call df.add(NameOf(SequenceHit.score), From h As SequenceHit In hits Select h.score)

        Return df
    End Function
End Module
