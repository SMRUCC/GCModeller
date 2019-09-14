#Region "Microsoft.VisualBasic::853836a73ffc5d8c58c685b32927e6f8, CLI_tools\c2\Workflows\RegulationNetwork\RegulationNetwork.vb"

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

    '     Class RegulationNetwork
    ' 
    '         Properties: DoorOperonView, Operons_150, Operons_200, Operons_250, Operons_300
    '                     Operons_350, Operons_400, Operons_450, Operons_500
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: ExtractPromoterRegion_KEGGModules, ExtractPromoterRegion_Pathway
    ' 
    '         Sub: (+2 Overloads) Dispose, ExportCoExpressionOperons, ExportOperonsPromoterRegions, InitalizeOperons, WholeGenomeRandomizeParsed
    '         Class PossibleRegulation
    ' 
    '             Properties: GeneId, Regulator, Weight
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text

Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions

Namespace Workflows.RegulationNetwork

    Public Class RegulationNetwork : Implements System.IDisposable

        Dim MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder

        Public Property DoorOperonView As LANS.SystemsBiology.Assembly.Door.OperonView

        Public Property Operons_150 As KeyValuePair(Of String, LANS.SystemsBiology.SequenceModel.FASTA.FastaToken)()
        Public Property Operons_200 As KeyValuePair(Of String, LANS.SystemsBiology.SequenceModel.FASTA.FastaToken)()
        Public Property Operons_250 As KeyValuePair(Of String, LANS.SystemsBiology.SequenceModel.FASTA.FastaToken)()
        Public Property Operons_300 As KeyValuePair(Of String, LANS.SystemsBiology.SequenceModel.FASTA.FastaToken)()
        Public Property Operons_350 As KeyValuePair(Of String, LANS.SystemsBiology.SequenceModel.FASTA.FastaToken)()
        Public Property Operons_400 As KeyValuePair(Of String, LANS.SystemsBiology.SequenceModel.FASTA.FastaToken)()
        Public Property Operons_450 As KeyValuePair(Of String, LANS.SystemsBiology.SequenceModel.FASTA.FastaToken)()
        Public Property Operons_500 As KeyValuePair(Of String, LANS.SystemsBiology.SequenceModel.FASTA.FastaToken)()

        Sub New(MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder, Door As String)
            Me.MetaCyc = MetaCyc
            Me.DoorOperonView = LANS.SystemsBiology.Assembly.Door.Load(Door).DoorOperonView
            Call InitalizeOperons(Door, MetaCyc.Database.FASTAFiles.OriginSourceFile)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="ExportedDir"></param>
        ''' <param name="splitCount"></param>
        ''' <param name="maxSplitCount"></param>
        ''' <remarks></remarks>
        Public Sub WholeGenomeRandomizeParsed(ExportedDir As String, Optional splitCount As Integer = 5000, Optional maxSplitCount As Integer = 25)
            Dim Operons = DoorOperonView.Operons
            Dim ok As Boolean() = New Boolean(5) {}

            Dim Action As System.Action(Of KeyValuePair(Of String, LANS.SystemsBiology.SequenceModel.FASTA.FastaToken)(), Integer, Integer) =
 _
                Sub(operonPromoters As KeyValuePair(Of String, LANS.SystemsBiology.SequenceModel.FASTA.FastaToken)(), length As Integer, okidx As Integer)
                    ok(okidx) = False

                    If Operons.Count > maxSplitCount Then
                        Dim stw = Stopwatch.StartNew

                        For countDown As Integer = 1 To splitCount
                            Dim chunkBuffer = Operons.TakeRandomly(maxSplitCount)
                            Dim Data As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken() = New LANS.SystemsBiology.SequenceModel.FASTA.FastaToken(chunkBuffer.Count - 1) {}
                            Dim Path As String = String.Format("{0}_{1}bp/whole_genome_rnd_split_{2}.fsa", ExportedDir, length, countDown)

                            If countDown Mod 237 = 0 Then
                                Console.WriteLine("THREAD__{0}:: Percentage {1}% on segment length:= {2}bp ..........{3}ms", okidx, 100 * countDown / splitCount, length, stw.ElapsedMilliseconds)
                            End If

                            For i As Integer = 0 To Data.Count - 1
                                Dim Operon = chunkBuffer(i)
                                Dim PromoterFsa As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken = (From promoter In operonPromoters.AsParallel Where String.Equals(Operon.Key, promoter.Key) Select promoter.Value).First
                                Data(i) = PromoterFsa
                            Next

                            Dim FASTAData = CType(Data, LANS.SystemsBiology.SequenceModel.FASTA.FastaFile)
                            Call FASTAData.Save(Path)
                        Next
                    Else
                        Dim Data As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken() = New LANS.SystemsBiology.SequenceModel.FASTA.FastaToken(Operons.Count - 1) {}
                        Dim Path As String = String.Format("{0}_{1}bp/whole_genome_operons.fsa", ExportedDir, length)

                        For i As Integer = 0 To Data.Count - 1
                            Dim Operon = Operons(i)
                            Dim PromoterFsa As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken = (From promoter In operonPromoters.AsParallel Where String.Equals(Operon.Key, promoter.Key) Select promoter.Value).First
                            Data(i) = PromoterFsa
                        Next

                        Call CType(Data, LANS.SystemsBiology.SequenceModel.FASTA.FastaFile).Save(Path)
                    End If

                    Console.WriteLine("Thread {0} job done and exit.", okidx)
                    ok(okidx) = True
                End Sub

            Call New Threading.Thread(Sub() Action(Me.Operons_150, 150, 0)).Start()
            Call New Threading.Thread(Sub() Action(Me.Operons_200, 200, 1)).Start()
            Call New Threading.Thread(Sub() Action(Me.Operons_250, 250, 2)).Start()
            Call New Threading.Thread(Sub() Action(Me.Operons_300, 300, 3)).Start()
            Call New Threading.Thread(Sub() Action(Me.Operons_350, 350, 4)).Start()
            Call New Threading.Thread(Sub() Action(Me.Operons_400, 400, 5)).Start()

            Do While (From item In ok Where item = True Select 1).Count = ok.Count
                Threading.Thread.Sleep(10000)
            Loop
            'Call Action(Me.Operons_450, 450)
            'Call Action(Me.Operons_500, 500)
        End Sub

        Private Sub InitalizeOperons(DoorFile As String, OS As String)
            Dim GenomeSeq As LANS.SystemsBiology.SequenceModel.NucleotideModels.SegmentReader =
                New LANS.SystemsBiology.SequenceModel.NucleotideModels.SegmentReader(LANS.SystemsBiology.SequenceModel.FASTA.FastaToken.LoadNucleotideData(OS), False)
            Dim Door = Me.DoorOperonView

            Dim CreateObject = Function(SegmentLength As Integer) As KeyValuePair(Of String, LANS.SystemsBiology.SequenceModel.FASTA.FastaToken)()
                                   Dim LQuery = (From i As Integer In Door.Operons.Sequence
                                                 Let Operon = Door.Operons(i)
                                                 Let FirstGene = Operon.FirstGene
                                                 Let GetFASTA =
                                                 Function() As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken
                                                     Dim PromoterFsa As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken =
                                                         New LANS.SystemsBiology.SequenceModel.FASTA.FastaToken With {
                                                             .Attributes = New String() {String.Format("lcl_{0} [AssociatedOperon={1}] [OperonPromoter={2}; {3}] [OperonGenes={4}]",
                                                                                                       i + 1, Operon.Key, FirstGene.Synonym, FirstGene.Location.ToString,
                                                                                                       LANS.SystemsBiology.Assembly.Door.OperonView.GenerateLstIdString(Operon))}}
                                                     Dim Location = FirstGene.Location

                                                     If Location.Strand = LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation.Strands.Forward Then
                                                         PromoterFsa.SequenceData = GenomeSeq.TryParse(Location.Left, SegmentLength, directionDownstream:=False)
                                                     Else
                                                         PromoterFsa.SequenceData = GenomeSeq.ReadComplement(Location.Right, SegmentLength, True)
                                                     End If

                                                     Return PromoterFsa
                                                 End Function Select New KeyValuePair(Of String, LANS.SystemsBiology.SequenceModel.FASTA.FastaToken)(Operon.Key, GetFASTA())).ToArray

                                   Return LQuery
                               End Function

            Operons_150 = CreateObject(150)
            Operons_200 = CreateObject(200)
            Operons_250 = CreateObject(250)
            Operons_300 = CreateObject(300)
            Operons_350 = CreateObject(350)
            Operons_400 = CreateObject(400)
            Operons_450 = CreateObject(450)
            Operons_500 = CreateObject(500)
        End Sub

        ''' <summary>
        ''' 代谢途径的
        ''' </summary>
        ''' <param name="ExportLocation"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ExtractPromoterRegion_Pathway(ExportLocation As String) As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
            Dim PwyFilters As PwyFilters = New PwyFilters(MetaCyc)
            Dim pathways = PwyFilters.Performance()
            Dim Parser As TryParsePromoter = New TryParsePromoter(MetaCyc, Me.DoorOperonView)

            Call Parser.Proceeding(pathways, String.Format("{0}/PathwayPromoter", ExportLocation), Me.DoorOperonView, SegmentLength:=New Integer() {150, 200, 250, 300, 350, 400, 450, 500})

            Dim PathwayOverviews = PwyFilters.GenerateReport(pathways, MetaCyc.GetGenes)
            Call PathwayOverviews.Save(ExportLocation & "\PathwayOverviews.csv", False)
            Return PathwayOverviews
        End Function

        Public Function ExtractPromoterRegion_KEGGModules(ExportLocation As String, KEGGModules As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File) As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
            Dim Modules = Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection.Reflector.Convert(Of LANS.SystemsBiology.Assembly.KEGG.DBGET.bGetObject.Module)(KEGGModules.DataFrame, False)
            Dim Operons = DoorOperonView

            Dim getfasta = Function(operonlist As LANS.SystemsBiology.Assembly.Door.Operon(),
                                    promoterRegions As KeyValuePair(Of String, LANS.SystemsBiology.SequenceModel.FASTA.FastaToken)()) As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile
                               Dim LQuery = (From item In operonlist Select (From itess In promoterRegions Where String.Equals(item.Key, itess.Key) Select itess).First.Value).ToArray
                               Return LQuery
                           End Function

            For Each [Module] In Modules
                Dim module_operons = Operons.Select([Module].GetPathwayGenes)
                Call getfasta(module_operons, Me.Operons_150).Save(String.Format("{0}/KEGGModules/_{1}bp/{2}.fsa", ExportLocation, 150, [Module].EntryId))
                Call getfasta(module_operons, Me.Operons_200).Save(String.Format("{0}/KEGGModules/_{1}bp/{2}.fsa", ExportLocation, 200, [Module].EntryId))
                Call getfasta(module_operons, Me.Operons_250).Save(String.Format("{0}/KEGGModules/_{1}bp/{2}.fsa", ExportLocation, 250, [Module].EntryId))
                Call getfasta(module_operons, Me.Operons_300).Save(String.Format("{0}/KEGGModules/_{1}bp/{2}.fsa", ExportLocation, 300, [Module].EntryId))
                Call getfasta(module_operons, Me.Operons_350).Save(String.Format("{0}/KEGGModules/_{1}bp/{2}.fsa", ExportLocation, 350, [Module].EntryId))
                Call getfasta(module_operons, Me.Operons_400).Save(String.Format("{0}/KEGGModules/_{1}bp/{2}.fsa", ExportLocation, 400, [Module].EntryId))
                Call getfasta(module_operons, Me.Operons_450).Save(String.Format("{0}/KEGGModules/_{1}bp/{2}.fsa", ExportLocation, 450, [Module].EntryId))
                Call getfasta(module_operons, Me.Operons_500).Save(String.Format("{0}/KEGGModules/_{1}bp/{2}.fsa", ExportLocation, 500, [Module].EntryId))
            Next

            Return Nothing
        End Function

        Public Sub ExportOperonsPromoterRegions(ExportedDir As String, operons As LANS.SystemsBiology.Assembly.Door.Operon(), Optional splitCount As Integer = 3, Optional maxSplitCount As Integer = 500)
            Dim Action As System.Action(Of KeyValuePair(Of String, LANS.SystemsBiology.SequenceModel.FASTA.FastaToken)(), Integer) =
                Sub(operonPromoters As KeyValuePair(Of String, LANS.SystemsBiology.SequenceModel.FASTA.FastaToken)(), length As Integer)

                    If operons.Count > maxSplitCount Then
                        For countDown As Integer = 1 To splitCount
                            Dim chunkBuffer = operons.TakeRandomly(maxSplitCount)
                            Dim Data As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken() = New LANS.SystemsBiology.SequenceModel.FASTA.FastaToken(chunkBuffer.Count - 1) {}
                            Dim Path As String = String.Format("{0}_{1}bp/regulator_rnd_split_{2}.fsa", ExportedDir, length, countDown)

                            For i As Integer = 0 To Data.Count - 1
                                Dim Operon = chunkBuffer(i)
                                Dim PromoterFsa As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken = (From promoter In operonPromoters.AsParallel Where String.Equals(Operon.Key, promoter.Key) Select promoter.Value).First
                                Data(i) = PromoterFsa
                            Next

                            Dim FASTAData = CType(Data, LANS.SystemsBiology.SequenceModel.FASTA.FastaFile)
                            Call FASTAData.Save(Path)
                        Next
                    Else
                        Dim Data As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken() = New LANS.SystemsBiology.SequenceModel.FASTA.FastaToken(operons.Count - 1) {}
                        Dim Path As String = String.Format("{0}_{1}bp/operons.fsa", ExportedDir, length)

                        For i As Integer = 0 To Data.Count - 1
                            Dim Operon = operons(i)
                            Dim PromoterFsa As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken = (From promoter In operonPromoters.AsParallel Where String.Equals(Operon.Key, promoter.Key) Select promoter.Value).First
                            Data(i) = PromoterFsa
                        Next

                        Call CType(Data, LANS.SystemsBiology.SequenceModel.FASTA.FastaFile).Save(Path)
                    End If
                End Sub

            Call Action(Me.Operons_150, 150)
            Call Action(Me.Operons_200, 200)
            Call Action(Me.Operons_250, 250)
            Call Action(Me.Operons_300, 300)
            Call Action(Me.Operons_350, 350)
            Call Action(Me.Operons_400, 400)
            Call Action(Me.Operons_450, 450)
            Call Action(Me.Operons_500, 500)
        End Sub

        ''' <summary>
        ''' New !!!!!!
        ''' </summary>
        ''' <param name="ExportedDir"></param>
        ''' <param name="splitCount"></param>
        ''' <param name="maxSplitCount"></param>
        ''' <remarks></remarks>
        Public Sub ExportCoExpressionOperons(ExportedDir As String, Regulations As KeyValuePair(Of String, PossibleRegulation())(), Optional splitCount As Integer = 3, Optional maxSplitCount As Integer = 500)
            Dim recordList As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File = New DocumentFormat.Csv.DocumentStream.File
            Call recordList.AppendLine(New String() {"OperonId", "OperonGenes", "OperonCounts", "PossibleCoExpressionOperons"})

            Dim Action As System.Action(Of LANS.SystemsBiology.Assembly.Door.Operon, LANS.SystemsBiology.Assembly.Door.Operon(),
                                        KeyValuePair(Of String, LANS.SystemsBiology.SequenceModel.FASTA.FastaToken)(), Integer, PossibleRegulation()) =
                Sub(Regulator As LANS.SystemsBiology.Assembly.Door.Operon, Operons As LANS.SystemsBiology.Assembly.Door.Operon(),
                             operonPromoters As KeyValuePair(Of String, LANS.SystemsBiology.SequenceModel.FASTA.FastaToken)(), length As Integer,
                             GeneList As PossibleRegulation())

                    If Operons.Count > maxSplitCount Then
                        For countDown As Integer = 1 To splitCount
                            Dim chunkBuffer = Operons.TakeRandomly(maxSplitCount)
                            Dim Data As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken() = New LANS.SystemsBiology.SequenceModel.FASTA.FastaToken(chunkBuffer.Count - 1) {}
                            Dim Path As String = String.Format("{0}_{1}bp/{2}_rnd_split_{3}.fsa", ExportedDir, length, Regulator.Key, countDown)
                            Dim sBuilder As StringBuilder = New StringBuilder(1024)

                            For i As Integer = 0 To Data.Count - 1
                                Dim Operon = chunkBuffer(i)
                                Dim PromoterFsa As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken = (From promoter In operonPromoters.AsParallel Where String.Equals(Operon.Key, promoter.Key) Select promoter.Value).First
                                Call sBuilder.Append(Operon.Key & ";")
                                Data(i) = PromoterFsa
                            Next
                            Call sBuilder.Remove(sBuilder.Length - 1, 1)
                            Call recordList.AppendLine(New String() {Regulator.Key, LANS.SystemsBiology.Assembly.Door.OperonView.GenerateLstIdString(Regulator), Operons.Count, sBuilder.ToString})

                            Dim FASTAData = CType(Data, LANS.SystemsBiology.SequenceModel.FASTA.FastaFile)
                            Call FASTAData.Save(Path)
                        Next
                    Else
                        Dim Data As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken() = New LANS.SystemsBiology.SequenceModel.FASTA.FastaToken(Operons.Count - 1) {}
                        Dim Path As String = String.Format("{0}_{1}bp/{2}.fsa", ExportedDir, length, Regulator.Key)
                        Dim sBuilder As StringBuilder = New StringBuilder(1024)

                        For i As Integer = 0 To Data.Count - 1
                            Dim Operon = Operons(i)
                            Dim PromoterFsa As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken = (From promoter In operonPromoters.AsParallel Where String.Equals(Operon.Key, promoter.Key) Select promoter.Value).First
                            Call sBuilder.Append(Operon.Key & ";")
                            Data(i) = PromoterFsa
                        Next
                        Call sBuilder.Remove(sBuilder.Length - 1, 1)
                        Call recordList.AppendLine(New String() {Regulator.Key, LANS.SystemsBiology.Assembly.Door.OperonView.GenerateLstIdString(Regulator), Operons.Count, sBuilder.ToString})

                        Call CType(Data, LANS.SystemsBiology.SequenceModel.FASTA.FastaFile).Save(Path)
                    End If

                    Call GeneList.SaveTo(String.Format("{0}/{1}.csv", FileIO.FileSystem.GetParentPath(ExportedDir), Regulator.Key))
                End Sub

            For Each item In Regulations
                Dim operons = Me.DoorOperonView.Select((From gene In item.Value Select gene.GeneId).ToArray) '得到所有的与Regulator相对应的基因号，再取出操纵子对象
                Dim TargetOperon = Me.DoorOperonView.Select(item.Key) 'targetoperon指的是regulator所在的operon，这个operon的序列数据并不放在operons之中

                If operons.IsNullOrEmpty OrElse operons.Count < 2 Then
                    Console.WriteLine("Unabled parse {0}", item.Key)
                    Call recordList.AppendLine(New String() {item.Key, LANS.SystemsBiology.Assembly.Door.OperonView.GenerateLstIdString(TargetOperon)})
                    Continue For
                End If

                Call Action(TargetOperon, operons, Me.Operons_150, 150, item.Value)
                Call Action(TargetOperon, operons, Me.Operons_200, 200, item.Value)
                Call Action(TargetOperon, operons, Me.Operons_250, 250, item.Value)
                Call Action(TargetOperon, operons, Me.Operons_300, 300, item.Value)
                Call Action(TargetOperon, operons, Me.Operons_350, 350, item.Value)
                Call Action(TargetOperon, operons, Me.Operons_400, 400, item.Value)
                Call Action(TargetOperon, operons, Me.Operons_450, 450, item.Value)
                Call Action(TargetOperon, operons, Me.Operons_500, 500, item.Value)
            Next

            Call recordList.Save(ExportedDir & "/gene_vs_coexpressionOperons_recordList.csv", False)
        End Sub

        Public Class PossibleRegulation
            Public Property Regulator As String
            Public Property GeneId As String
            <Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection.Column("weight")>
            Public Property Weight As Double
        End Class

        'Public Sub ExportCoExpressionOperons(ExportedDir As String, Optional pccCutoff As Double = 0.6, Optional splitCount As Integer = 3, Optional maxSplitCount As Integer = 25)
        '    Dim recordList As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File = New DocumentFormat.Csv.DocumentStream.File
        '    maxSplitCount = If(maxSplitCount <> 25, (From operon In Me.DoorOperonView.Operons Select operon.Value.Count).ToArray.Max, 25)

        '    Call recordList.AppendLine(New String() {"OperonId", "OperonGenes", "OperonCounts", "PossibleCoExpressionOperons"})

        '    Dim Action = Sub(TargetOperon As KeyValuePair(Of String, LANS.SystemsBiology.Assembly.Door.Gene()), Operons As KeyValuePair(Of String, LANS.SystemsBiology.Assembly.Door.Gene())(),
        '                     operonPromoters As KeyValuePair(Of String, LANS.SystemsBiology.SequenceModel.FASTA.FASTA)(), length As Integer)

        '                     If Operons.Count > maxSplitCount Then
        '                         For countDown As Integer = 1 To splitCount
        '                             Dim chunkBuffer = Operons.TakeRandomly(maxSplitCount)
        '                             Dim Data As LANS.SystemsBiology.SequenceModel.FASTA.FASTA() = New LANS.SystemsBiology.SequenceModel.FASTA.FASTA(chunkBuffer.Count - 1) {}
        '                             Dim Path As String = String.Format("{0}_{1}bp/{2}_rnd_split_{3}.fsa", ExportedDir, length, TargetOperon.Key, countDown)
        '                             Dim sBuilder As StringBuilder = New StringBuilder(1024)
        '                             Dim TargetOperonFASTA = (From promoter In operonPromoters.AsParallel Where String.Equals(TargetOperon.Key, promoter.Key) Select promoter.Value).First

        '                             For i As Integer = 0 To Data.Count - 1
        '                                 Dim Operon = chunkBuffer(i)
        '                                 Dim PromoterFsa As LANS.SystemsBiology.SequenceModel.FASTA.FASTA = (From promoter In operonPromoters.AsParallel Where String.Equals(Operon.Key, promoter.Key) Select promoter.Value).First
        '                                 Call sBuilder.Append(Operon.Key & ";")
        '                                 Data(i) = PromoterFsa
        '                             Next
        '                             Call sBuilder.Remove(sBuilder.Length - 1, 1)
        '                             Call recordList.AppendLine(New String() {TargetOperon.Key, LANS.SystemsBiology.Assembly.Door.OperonView.GenerateListIdString(TargetOperon), Operons.Count, sBuilder.ToString})

        '                             Dim FASTAData = CType(Data, LANS.SystemsBiology.SequenceModel.FASTA.File)
        '                             Call FASTAData.Remove(TargetOperonFASTA)
        '                             Call FASTAData.Insert(0, TargetOperonFASTA)
        '                             Call FASTAData.Save(Path)
        '                         Next
        '                     Else
        '                         Dim Data As LANS.SystemsBiology.SequenceModel.FASTA.FASTA() = New LANS.SystemsBiology.SequenceModel.FASTA.FASTA(Operons.Count - 1) {}
        '                         Dim Path As String = String.Format("{0}_{1}bp/{2}.fsa", ExportedDir, length, TargetOperon.Key)
        '                         Dim sBuilder As StringBuilder = New StringBuilder(1024)

        '                         For i As Integer = 0 To Data.Count - 1
        '                             Dim Operon = Operons(i)
        '                             Dim PromoterFsa As LANS.SystemsBiology.SequenceModel.FASTA.FASTA = (From promoter In operonPromoters.AsParallel Where String.Equals(Operon.Key, promoter.Key) Select promoter.Value).First
        '                             Call sBuilder.Append(Operon.Key & ";")
        '                             Data(i) = PromoterFsa
        '                         Next
        '                         Call sBuilder.Remove(sBuilder.Length - 1, 1)
        '                         Call recordList.AppendLine(New String() {TargetOperon.Key, LANS.SystemsBiology.Assembly.Door.OperonView.GenerateListIdString(TargetOperon), Operons.Count, sBuilder.ToString})

        '                         Call CType(Data, LANS.SystemsBiology.SequenceModel.FASTA.File).Save(Path)
        '                     End If
        '                 End Sub

        '    For Each TargetOperon In Me.DoorOperonView.Operons
        '        Dim GeneId = LANS.SystemsBiology.Assembly.Door.OperonView.GetFirstGene(TargetOperon).Synonym
        '        Dim Operons = CoExpression.GetPossibleCoExpression(GeneId, pccCutoff, 81, 64)

        '        If Operons.IsNullOrEmpty OrElse Operons.Count < 2 Then
        '            Console.WriteLine("Unabled parse {0}", GeneId)
        '            Call recordList.AppendLine(New String() {TargetOperon.Key, LANS.SystemsBiology.Assembly.Door.OperonView.GenerateListIdString(TargetOperon)})
        '            Continue For
        '        End If

        '        Call Action(TargetOperon, Operons, Me.Operons_150, 150)
        '        Call Action(TargetOperon, Operons, Me.Operons_200, 200)
        '        Call Action(TargetOperon, Operons, Me.Operons_250, 250)
        '    Next

        '    Call recordList.Save(ExportedDir & "/gene_vs_coexpressionOperons_recordList.csv")
        'End Sub

        'Public Sub Export2(GeneId As String, doorFile As String, ExportedDir As String, STDCutOff1 As Double, STDCutOff2 As Double, pccCutOff As Double)
        '    Dim Door = LANS.SystemsBiology.Assembly.Door.Load(doorFile).DoorOperonView
        '    Dim TargetOperon = Door.Select(GeneId)
        '    Dim Operons = CoExpression.GetPossibleCoExpression(LANS.SystemsBiology.Assembly.Door.OperonView.GetFirstGene(TargetOperon).Synonym, pccCutOff, STDCutOff1, STDCutOff2)
        '    Dim recordList As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File = New DocumentFormat.Csv.DocumentStream.File
        '    Call recordList.AppendLine(New String() {"OperonId", "OperonGenes", "PossibleCoExpressionOperon", "CoExpressionOperonGeneList"})

        '    If Operons.IsNullOrEmpty OrElse Operons.Count < 2 Then
        '        Console.WriteLine("Unabled parse {0}", GeneId)
        '        Call recordList.AppendLine(New String() {TargetOperon.Key & " - " & LANS.SystemsBiology.Assembly.Door.OperonView.GetFirstGene(TargetOperon).Synonym,
        '                                                 LANS.SystemsBiology.Assembly.Door.OperonView.GenerateListIdString(TargetOperon)})
        '        Call recordList.Save(ExportedDir & String.Format("/{0}_gene_vs_coexpressionOperons_recordList.csv", GeneId))
        '        Return
        '    End If

        '    For Each Operon In Operons
        '        Call recordList.AppendLine(New String() {TargetOperon.Key & " - " & LANS.SystemsBiology.Assembly.Door.OperonView.GetFirstGene(TargetOperon).Synonym,
        '                                                 LANS.SystemsBiology.Assembly.Door.OperonView.GenerateListIdString(TargetOperon),
        '                                                 Operon.Key & " - " & LANS.SystemsBiology.Assembly.Door.OperonView.GetFirstGene(Operon).Synonym,
        '                                                 LANS.SystemsBiology.Assembly.Door.OperonView.GenerateListIdString(Operon)})
        '    Next

        '    Call recordList.Save(ExportedDir & String.Format("/{0}_gene_vs_coexpressionOperons_recordList.csv", GeneId))

        '    Dim Length = 150
        '    For Each OperonPromoters In {Me.Operons_150, Me.Operons_200, Me.Operons_250}
        '        Dim Data As LANS.SystemsBiology.SequenceModel.FASTA.FASTA() = New LANS.SystemsBiology.SequenceModel.FASTA.FASTA(Operons.Count - 1) {}
        '        Dim Path As String = String.Format("{0}/{1}_{2}bp.fsa", ExportedDir, TargetOperon.Key, Length)

        '        For i As Integer = 0 To Data.Count - 1
        '            Dim Operon = Operons(i)
        '            Dim PromoterFsa As LANS.SystemsBiology.SequenceModel.FASTA.FASTA = (From promoter In OperonPromoters.AsParallel Where String.Equals(Operon.Key, promoter.Key) Select promoter.Value).First
        '            Data(i) = PromoterFsa
        '        Next
        '        Call CType(Data, LANS.SystemsBiology.SequenceModel.FASTA.File).Save(Path)
        '        Length += 50
        '    Next
        'End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' 检测冗余的调用

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO:  释放托管状态(托管对象)。
                End If

                ' TODO:  释放非托管资源(非托管对象)并重写下面的 Finalize()。
                ' TODO:  将大型字段设置为 null。
            End If
            Me.disposedValue = True
        End Sub

        ' TODO:  仅当上面的 Dispose(ByVal disposing As Boolean)具有释放非托管资源的代码时重写 Finalize()。
        'Protected Overrides Sub Finalize()
        '    ' 不要更改此代码。    请将清理代码放入上面的 Dispose(ByVal disposing As Boolean)中。
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' Visual Basic 添加此代码是为了正确实现可处置模式。
        Public Sub Dispose() Implements IDisposable.Dispose
            ' 不要更改此代码。    请将清理代码放入上面的 Dispose (disposing As Boolean)中。
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
