#Region "Microsoft.VisualBasic::f0a36ca2893a3e065a7508a60275e47a, analysis\ProteinTools\ProteinTools.Interactions\SwissTCS\DataPreparations.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' Class DataPreparations
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: InferInteraction, (+2 Overloads) SequenceAssemble, Trim
    '     Class DipRecord
    ' 
    '         Properties: ConfidenceValues, IdInteractorA, IdInteractorB, InteractionDetectionMethods, InteractionIdentifiers
    '                     InteractionTypes, ProcessingStatus, PublicationIdentifiers, SourceDatabases, TaxidInteractorA
    '                     TaxidInteractorB
    ' 
    '         Function: Convert
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports RDotNet.Extensions.VisualBasic
Imports SMRUCC.genomics.Interops
Imports SMRUCC.genomics.Interops.ClustalOrg
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Programs
Imports SMRUCC.genomics.SequenceModel

Public Class DataPreparations

    Dim LocalBLAST As BLASTPlus
    Dim DipInteractions As DipRecord()
    Dim DipFsaSequence As FASTA.FastaFile
    Dim WorkDir As String
    Dim GenomicsProteins As FASTA.FastaFile
    Dim Clustal As Clustal

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="GenomicProteins">每一条序列对象仅包含有基因号</param>
    ''' <param name="DipDir"></param>
    ''' <param name="Blastbin"></param>
    ''' <remarks></remarks>
    Sub New(GenomicProteins As FASTA.FastaFile, DipDir As String, Blastbin As String, rBin As String, ClustalBin As String, Optional Temp As String = "")
        Call Console.WriteLine("===================================INITIALIZE_SESSION==========================================")

        LocalBLAST = New BLASTPlus(Blastbin)
        DipInteractions = String.Format("{0}/dip.csv", DipDir).LoadCsv(Of DipRecord)(False).ToArray
        DipFsaSequence = FASTA.FastaFile.Read(DipDir & "/dip.fsa")
        GenomicsProteins = GenomicProteins

        If Not String.IsNullOrEmpty(Temp) Then
            Call FileIO.FileSystem.CreateDirectory(Temp)
            WorkDir = Temp & "/"
        Else
            Temp = My.Computer.FileSystem.SpecialDirectories.Temp & "/"
        End If

        Call LocalBLAST.FormatDb(DipFsaSequence.FilePath, LocalBLAST.MolTypeProtein).Start(waitForExit:=True)
        Call TryInit(rBin)

        Clustal = New Clustal(ClustalBin)
        Call Console.WriteLine("===============================Initialization job done!======================================" & vbCrLf & vbCrLf)
    End Sub

    Public Function InferInteraction(GeneId As String) As IO.File
        Dim workDir As String = String.Format("{0}/{1}/", Me.WorkDir, GeneId)
        Dim TempFile As String = workDir & GeneId

        '1.保存目标基因的蛋白质序列数据
        Call GenomicsProteins.Select(Function(FsaObject As FASTA.FastaSeq) String.Equals(FsaObject.Headers.First, GeneId)).Save(TempFile)
        '2.blastp搜索dip数据库中的同源蛋白，作为原始计算数据
        Call Console.WriteLine("Searching the homologous protein in the dip database for object ""{0}""...", GeneId)
        Call LocalBLAST.Blastp(TempFile, DipFsaSequence.FilePath, String.Format("{0}/blastp_{1}_vs._dip.txt", workDir, GeneId), "1e-3").Start(waitForExit:=True)
        '3.导出所有的符合条件的besthit
        Dim Besthits = NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.TryParse(Me.LocalBLAST.LastBLASTOutputFilePath).ExportAllBestHist
        Dim Hits As BestHit() = Besthits
        Call Besthits.SaveTo(LocalBLAST.LastBLASTOutputFilePath & ".csv", False)
        '将besthit的蛋白质序列导出，留作多序列比对
        Dim MatchedIdCollection = (From item In Hits Where Not String.Equals(item.HitName, IBlastOutput.HITS_NOT_FOUND) Select item.HitName.Split.First Distinct).ToArray
#If DEBUG Then
        MatchedIdCollection = MatchedIdCollection.Take(5).ToArray
#End If

        Dim TargetHomologous = DipFsaSequence.Select(Function(FsaObject As FASTA.FastaSeq) Array.IndexOf(MatchedIdCollection, FsaObject.Headers.First.Split.First) > -1)
        '将目标基因的蛋白质序列加入到最后
        Call TargetHomologous.AddRange(FASTA.FastaFile.Read(TempFile))
        TempFile = TempFile & "_alignment.fsa"
        Call TargetHomologous.Save(TempFile)

        Dim InteractionPairs As List(Of KeyValuePair(Of String, String)) = New List(Of KeyValuePair(Of String, String))

        Call Console.WriteLine("Looking for the interaction pairs in the dip database...")
        '查找出partner
        Dim PartnersIdList As List(Of String) = New List(Of String)
        For Each id In MatchedIdCollection
            Dim PartnerIds = (From item In DipInteractions.AsParallel Where String.Equals(item.IdInteractorA, id) Select item.IdInteractorB).ToArray
            Call PartnersIdList.AddRange(PartnerIds)
            For Each partnerid As String In PartnerIds
                Call InteractionPairs.Add(New KeyValuePair(Of String, String)(id, partnerid))
            Next
            PartnerIds = (From item In DipInteractions.AsParallel Where String.Equals(item.IdInteractorB, id) Select item.IdInteractorA).ToArray
            Call PartnersIdList.AddRange(PartnerIds)
            For Each partnerid As String In PartnerIds
                Call InteractionPairs.Add(New KeyValuePair(Of String, String)(id, partnerid))
            Next
        Next
        PartnersIdList = (From id As String In PartnersIdList Select id Distinct Order By id Ascending).AsList
        Call Console.WriteLine("There are {0} partner records was found in the database!", PartnersIdList.Count)

        Dim TargetHomologousPartners = DipFsaSequence.Select(Function(FsaObject As FASTA.FastaSeq) PartnersIdList.IndexOf(FsaObject.Headers.First.Split.First) > -1)
        TempFile = workDir & GeneId & "_alignment_partners.fsa"
        Call TargetHomologousPartners.Save(TempFile)
        Call LocalBLAST.FormatDb(TempFile, LocalBLAST.MolTypeProtein).Start(waitForExit:=True)
        Call LocalBLAST.Blastp(GenomicsProteins.FilePath, TargetHomologousPartners.FilePath, workDir & "/blastp_genomics_proteins_vs.dip_partner.txt", "1e-3").Start(waitForExit:=True)

        Besthits = NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.TryParse(Me.LocalBLAST.LastBLASTOutputFilePath).ExportAllBestHist
        Hits = Besthits
        Call Besthits.SaveTo(LocalBLAST.LastBLASTOutputFilePath & ".csv", False)
        MatchedIdCollection = (From item In Hits Where Not String.Equals(IBlastOutput.HITS_NOT_FOUND, item.HitName) Select item.QueryName Distinct).ToArray
#If DEBUG Then
        MatchedIdCollection = MatchedIdCollection.Take(10).ToArray
#End If
        Dim PartnerTargetHomologous = GenomicsProteins.Select(Function(FsaObject As FASTA.FastaSeq) Array.IndexOf(MatchedIdCollection, FsaObject.Headers.First.Split.First) > -1)
        Call PartnerTargetHomologous.Save(workDir & "target_genomics_proteins_homologous_partners.fsa")
        Call TargetHomologousPartners.AddRange(PartnerTargetHomologous)
        Call TargetHomologousPartners.Save(TempFile)

        Console.WriteLine("Write dip interaction pairs data...")

        Dim pairsFile As IO.File = New IO.File
        Call pairsFile.Add(New String() {"proteinId", "partnerId"})
        '生成具备实验事实的互作关系
        Call pairsFile.AppendRange((From item In InteractionPairs Select New IO.RowObject From {item.Key, item.Value}).ToArray)
        Call pairsFile.Save(String.Format("{0}/interaction_pairs_partners_for_{1}.csv", workDir, GeneId), False)

        Dim pairsFilePretend As New IO.File
        Call pairsFilePretend.Add(New String() {"proteinId", "partnerId"})
        '生成假定具备互作关系的互作蛋白质对
        Call pairsFilePretend.AppendRange((From item In MatchedIdCollection Select New IO.RowObject From {GeneId, item}).ToArray)
        Call pairsFilePretend.Save(String.Format("{0}/interaction_pairs_partners_for_{1}_pretended.csv", workDir, GeneId), False)

        Call Console.WriteLine("Start multiple sequence alignment calling clustal...")
        '进行多序列比对
        TargetHomologous = Trim(Clustal.MultipleAlignment(TargetHomologous.FilePath))
        TargetHomologousPartners = Trim(Clustal.MultipleAlignment(TargetHomologousPartners.FilePath))
        Call Console.WriteLine("Alignment job done, saving temp data!")

        Call TargetHomologous.Save(workDir & "___target_aligned.fsa")
        Call TargetHomologousPartners.Save(workDir & "__target_partner_aligned.fsa")

        '建模操作
        Call Console.WriteLine("Start to modelling the protein interaction Bayesian network...")

        '利用所建立的模型求解具体的蛋白质互作问题
        Dim ResultInteractions As New IO.File

        SyncLock R
            With R
                Dim model As New BnlearnModelling(SequenceAssemble(TargetHomologous, TargetHomologousPartners, pairsFile))
                Dim StandardOutput As String() = .WriteLine(model)

                Call Console.WriteLine("Modelling job done!")
                Call ResultInteractions.Add(New String() {"GeneId", "InteractionPartner", "Possibilities"})
                Call Console.WriteLine("Predicting protein interactions...")

                For Each x In pairsFilePretend.Skip(1)
                    Dim seqPair = SequenceAssemble(TargetHomologous, TargetHomologousPartners, New KeyValuePair(Of String, String)(x(0), x(1)))
                    Dim stpwatch = Stopwatch.StartNew
                    StandardOutput = .WriteLine(New BnlearnInference(seqPair.Key, seqPair.Value)) '对未知的两个蛋白质之间的互作问题做出推测
                    Call x.Add(StandardOutput.First)
                    Call Console.WriteLine("{0} -> {1}  P(A|B):={2}, {3}ms", x(0), x(1), x(2), stpwatch.ElapsedMilliseconds)
                    Call ResultInteractions.Add(x)
                Next
            End With
        End SyncLock

        Call Console.WriteLine("Prediction job done!" & vbCrLf)
        Call Console.WriteLine("==================END_OF_PREDITION_OF ""{0}""=====================", GeneId)

        Return ResultInteractions
    End Function

    Protected Friend Shared Function Trim(AlignedData As FASTA.FastaFile) As FASTA.FastaFile
        Dim p As Integer = 0
        Dim Sequence As List(Of Char)() = (From fsa In AlignedData Select fsa.SequenceData.AsList).ToArray  '请注意，对象之间需要保持顺序
        Dim SequenceCountsCutOff = AlignedData.Count * 0.75

        Do While True
            Dim LQuery = (From seq In Sequence Where seq(p) = "-"c Select 1).ToArray.Sum
            If LQuery >= SequenceCountsCutOff Then
                For i As Integer = 0 To Sequence.Count - 1
                    Dim seq = Sequence(i)
                    Call seq.RemoveAt(p)
                Next

                p -= 1
            End If

            p += 1

            If p >= Sequence.First.Count Then
                Exit Do
            End If
        Loop

        For i As Integer = 0 To AlignedData.Count - 1
            AlignedData(i).SequenceData = Sequence(i).ToArray '请注意，对象之间需要保持顺序
        Next

        Return AlignedData
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="InteractorA">经过多序列比对后的结果序列文件</param>
    ''' <param name="InteractorB"></param>
    ''' <param name="InteractionPairs"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function SequenceAssemble(InteractorA As FASTA.FastaFile, InteractorB As FASTA.FastaFile, InteractionPairs As IO.File) As String()
        Dim List As List(Of String) = New List(Of String)
        For Each pair In InteractionPairs.Skip(1)
            Dim itA = pair(0), itB = pair(1)
            Dim itASeq = InteractorA.Select(Function(FsaObject As FASTA.FastaSeq) String.Equals(FsaObject.Headers.First.Split.First, itA)).First
            Dim itBSeq = InteractorB.Select(Function(FsaObject As FASTA.FastaSeq) String.Equals(FsaObject.Headers.First.Split.First, itB)).First
            Call List.Add(itASeq.SequenceData.ToUpper & itBSeq.SequenceData.ToUpper)
        Next

        Return List.ToArray
    End Function

    Private Shared Function SequenceAssemble(InteractorA As FASTA.FastaFile, InteractorB As FASTA.FastaFile, InteractionPairs As KeyValuePair(Of String, String)) As KeyValuePair(Of String, String)
        Dim itASeq = InteractorA.Select(Function(FsaObject As SMRUCC.genomics.SequenceModel.FASTA.FastaSeq) String.Equals(FsaObject.Headers.First, InteractionPairs.Key)).First
        Dim itBSeq = InteractorB.Select(Function(FsaObject As SMRUCC.genomics.SequenceModel.FASTA.FastaSeq) String.Equals(FsaObject.Headers.First, InteractionPairs.Value)).First
        Dim result = New KeyValuePair(Of String, String)(itASeq.SequenceData.ToUpper, itBSeq.SequenceData.ToUpper)

        Return result
    End Function

    Public Class DipRecord
        <Column("ID interactor A")> Public Property IdInteractorA As String
        <Column("ID interactor B")> Public Property IdInteractorB As String
        <Column("Interaction detection method(s)")> Public Property InteractionDetectionMethods As String
        <Column("Publication Identifier(s)")> Public Property PublicationIdentifiers As String
        <Column("Taxid interactor A")> Public Property TaxidInteractorA As String
        <Column("Taxid interactor B")> Public Property TaxidInteractorB As String
        <Column("Interaction type(s)")> Public Property InteractionTypes As String
        <Column("Source database(s)")> Public Property SourceDatabases As String
        <Column("Interaction identifier(s)")> Public Property InteractionIdentifiers As String
        <Column("Confidence value(s)")> Public Property ConfidenceValues As String
        <Column("Processing Status")> Public Property ProcessingStatus As String

        Public Shared Function Convert(DataSet As DipRecord()) As DipRecord()
            Dim ChunkBuffer As DipRecord() = New DipRecord(DataSet.Count - 1) {}
            For i As Integer = 0 To DataSet.Count - 1
                Dim itemObject = DataSet(i)
                itemObject.IdInteractorA = itemObject.IdInteractorA.Split(CChar("|")).First
                itemObject.IdInteractorB = itemObject.IdInteractorB.Split(CChar("|")).First

                ChunkBuffer(i) = itemObject
            Next
            Return ChunkBuffer
        End Function
    End Class
End Class
