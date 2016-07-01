Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic
Imports SMRUCC.genomics.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.DatabaseServices.SabiorkKineticLaws.TabularDump

Namespace Compiler.Components

    <[Namespace]("Sabio-RK")>
    Public Class MergeSabiork

        Public Class MatchedSabiorkEnzyme : Inherits EnzymeCatalystKineticLaw

            Public Property MatchedId As String

            Public Overrides Function ToString() As String
                Return String.Format("{0} <==> {1}", MyBase.Uniprot, MatchedId)
            End Function

            Public Shared Function CreateObject(UniprotBesthit As BestHit, SabiorkEnzyme As EnzymeCatalystKineticLaw) As MatchedSabiorkEnzyme
                Dim EnzymeData As MatchedSabiorkEnzyme = SabiorkEnzyme.Copy(Of MatchedSabiorkEnzyme)()
                EnzymeData.MatchedId = UniprotBesthit.QueryName
                Return EnzymeData
            End Function
        End Class

        <ExportAPI("read.besthitcsv")>
        Public Shared Function LoadUniprotBesthits(path As String) As SMRUCC.genomics.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit()
            Return path.LoadCsv(Of SMRUCC.genomics.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit)(False).ToArray
        End Function

        <ExportAPI("read.enzymes")>
        Public Shared Function LoadSabiorkEnzyme(path As String) As EnzymeCatalystKineticLaw()
            Return path.LoadCsv(Of EnzymeCatalystKineticLaw)(False).ToArray
        End Function

        ''' <summary>
        ''' Match enzyme between the target genome and uniprot
        ''' </summary>
        ''' <param name="UniprotBesthits"></param>
        ''' <param name="SabiorkEnzyme"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("-match_enzyme")>
        Public Shared Function Matches(UniprotBesthits As BestHit(), SabiorkEnzyme As EnzymeCatalystKineticLaw()) As MatchedSabiorkEnzyme()

            Dim LQuery = (From Enzyme In SabiorkEnzyme.AsParallel
                          Select New KeyValuePair(Of EnzymeCatalystKineticLaw, BestHit())(Enzyme, GetItems(UniprotBesthits, Enzyme.Uniprot))).ToArray
            Dim List As List(Of MatchedSabiorkEnzyme) = New List(Of MatchedSabiorkEnzyme)
            For Each Line In LQuery
                Dim ChunkBuffer As MatchedSabiorkEnzyme() = (From item In Line.Value Select MatchedSabiorkEnzyme.CreateObject(item, Line.Key)).ToArray
                Call List.AddRange(ChunkBuffer)
            Next
            Return List.ToArray
        End Function

        <ExportAPI("write.matched_enzyme")>
        Public Shared Function WriteResult(data As MatchedSabiorkEnzyme(), savefile As String) As Boolean
            Try
                Call data.SaveTo(savefile, False)
            Catch ex As Exception
                Call Console.WriteLine(ex.ToString)
                Return False
            End Try
            Return True
        End Function

        Private Shared Function GetItems(UniprotBesthits As SMRUCC.genomics.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit(), UniprotId As String) _
            As SMRUCC.genomics.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit()

            Dim LQuery = (From item In UniprotBesthits Where String.Equals(item.HitName, UniprotId, StringComparison.OrdinalIgnoreCase) Select item).ToArray
            Return LQuery
        End Function

        Dim _ModelLoader As FileStream.IO.XmlresxLoader
        Dim _SabiorkCompoundsCSV As String
        Dim _EntryViews As EntryViews

        Sub New(ModelLoader As FileStream.IO.XmlresxLoader, SabiorkCompoundsCsv As String)
            Me._ModelLoader = ModelLoader
            Me._SabiorkCompoundsCSV = SabiorkCompoundsCsv
        End Sub

        Public Sub InvokeMergeCompoundSpecies()
            Dim UpdateCounts As Integer = 0
            Dim InsertCounts As Integer = 0
            Dim scpd = LoadCsv(Of CompoundSpecie)(Path:=Me._SabiorkCompoundsCSV, explicit:=False)

            Call Console.WriteLine(Me._ModelLoader.ToString)

            Me._EntryViews = New EntryViews(_ModelLoader.MetabolitesModel.Values.ToList)

            For Each item In scpd
                If item.GetDBLinkManager Is Nothing OrElse item.GetDBLinkManager.IsEmpty Then
                    Continue For
                End If
                Dim cpd As FileStream.Metabolite = GetCompound(item)
                If cpd Is Nothing Then '没有则进行添加
                    Call Me._EntryViews.AddEntry(item)
                    InsertCounts = InsertCounts + 1
                Else
                    Call Me._EntryViews.Update(cpd, item)   '存在，则尝试更新DBLink属性
                    UpdateCounts = UpdateCounts + 1
                End If
            Next

            Call Console.WriteLine("Job done, {0} was updated dblinks and {1} was insert into the compiled model!", UpdateCounts, InsertCounts)
        End Sub

        Private Function GetCompound(SabiorkCompound As CompoundSpecie) As FileStream.Metabolite
            Dim cpd = _EntryViews.GetByCheBIEntry(SabiorkCompound.ICompoundObjectCHEBI_values)
            If Not cpd Is Nothing Then
                Return cpd
            End If
            cpd = _EntryViews.GetByKeggEntry(SabiorkCompound.KEGG_Compound)
            If Not cpd Is Nothing Then
                Return cpd
            End If
            cpd = _EntryViews.GetByPubChemEntry(SabiorkCompound.PUBCHEM)
            Return cpd
        End Function
    End Class
End Namespace