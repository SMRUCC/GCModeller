#Region "Microsoft.VisualBasic::1c4ddfe728b8a6d2168a2075049b7933, CLI_tools\c2\Reconstruction\Genome\Promoter\Promoters.vb"

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

    '     Class Promoters
    ' 
    '         Properties: GetReconstructedPromoters, ReconstructList, Workspace
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Performance, Reconstruction
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ConsoleDevice.STDIO
Imports Microsoft.VisualBasic.Extensions

Namespace Reconstruction

    ''' <summary>
    ''' 使用BLASTN获取可能的同源的启动子序列
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Promoters : Inherits Operation

        Dim ReconstructedPromoters As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile
        Dim SubjectPromoterExports As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile
        Dim Dir As String

        Public Property ReconstructList As Operation.ReconstructedList(Of LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Promoter)

        Protected Friend Overrides ReadOnly Property Workspace As String
            Get
                Call Randomize()
                Dim n As Double = RandomDouble()
                Return String.Format("{0}\{1}\", MyBase.Workspace, n)
            End Get
        End Property

        Public ReadOnly Property GetReconstructedPromoters As List(Of LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Promoter)
            Get
                Return Me.ReconstructList.ToTable
            End Get
        End Property

        Sub New(Session As OperationSession)
            Call MyBase.New(Session)

            Me.ReconstructedPromoters = New PromoterFinder(Session).GetSegments
            Dim Export As SubjectPromoter = New SubjectPromoter(Session)
            Call Export.Performance()
            Me.SubjectPromoterExports = Export.GetExported
        End Sub

        Public Overrides Function Performance() As Integer
            Dim Dir As String = Me.Workspace

            Call FileIO.FileSystem.CreateDirectory(Dir)
            Call ReconstructedPromoters.Save(String.Format("{0}\___reconstructedPromoters.fsa", Dir))
            Call SubjectPromoterExports.Save(String.Format("{0}\_subjectPromoterExports__.fsa", Dir))

            'Call MyBase.MotifSampler.CreateBackgroundModel.Invoke(ReconstructedPromoters.SourceFile, String.Format("{0}\Query.txt", Dir)).Start(WaitForExit:=True)
            'Dim Query = MyBase.MotifSampler.CreateBackgroundModel.LastWork
            'Call MyBase.MotifSampler.CreateBackgroundModel.Invoke(SubjectPromoterExports.SourceFile, String.Format("{0}\Subject.txt", Dir)).Start(WaitForExit:=True)
            'Dim Subject = MyBase.MotifSampler.CreateBackgroundModel.LastWork

            'MotifSampler.MotifSampler.Output = String.Format("{0}\Query_MotifSampler.txt", Dir)
            'Call MyBase.MotifSampler.MotifSampler.Invoke(ReconstructedPromoters.SourceFile, Query).Start(WaitForExit:=True)
            'MotifSampler.MotifSampler.Output = String.Format("{0}\Subject_MotifSampler.txt", Dir)
            'Call MyBase.MotifSampler.MotifSampler.Invoke(SubjectPromoterExports.SourceFile, Subject).Start(WaitForExit:=True)

            Call MyBase.LocalBLAST.FormatDb(ReconstructedPromoters.SourceFile, LocalBLAST.MolTypeNucleotide).Start(WaitForExit:=True)
            Call MyBase.LocalBLAST.FormatDb(SubjectPromoterExports.SourceFile, LocalBLAST.MolTypeNucleotide).Start(WaitForExit:=True)

            Call MyBase.LocalBLAST.Blastn(ReconstructedPromoters.SourceFile, SubjectPromoterExports.SourceFile, Dir & "\BLASTN.xml", "1").Start(WaitForExit:=True)
            Call Printf("LOCALBLAST::BLASTN job done!\n\nStart to parsing promoters in the reconstructed database...")

            Dim Log = LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.BLASTOutput.XmlFile.BlastOutput.LoadFromFile(LocalBLAST.LastBLASTOutputFilePath)
            Dim QueryScript As Microsoft.VisualBasic.Text.TextGrepScriptEngine = Microsoft.VisualBasic.Text.TextGrepScriptEngine.Compile("tokens ' ' 0")
            Dim HitScript As Microsoft.VisualBasic.Text.TextGrepScriptEngine = Microsoft.VisualBasic.Text.TextGrepScriptEngine.Compile("tokens | 1")
            Call Log.Grep(AddressOf QueryScript.Grep, AddressOf HitScript.Grep)
            Call Log.Save()

            Dim LQuery = From QueryItem As LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.BLASTOutput.XmlFile.Iteration In Log.Iterations
                         Select Reconstruction(QueryItem) '
            Me.ReconstructList = New Operation.ReconstructedList(Of LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Promoter)
            Dim n = LQuery.ToArray.Count

            Call MyBase.Reconstructed.GetPromoters()
            Call Me.Reconstructed.Database.Promoters.AddRange(Me.ReconstructList.ToTable)
            Call Me.Reconstructed.Database.Promoters.Indexing()

            Return n
        End Function

        Private Function Reconstruction(Query As LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.BLASTOutput.XmlFile.Iteration) As Integer
            If Query.Hits.IsNullOrEmpty Then Return -1

            Dim Promoter As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Promoter =
                New LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Promoter
            Dim LQuery = From Hit In Query.Hits Select Hit Order By (From hsp In Hit.Hsps Select Val(hsp.BitScore)).Sum Descending '
            Dim ID As String = LQuery.First.Id

            Promoter.Identifier = "PM-" & Query.QueryDef
            Promoter.AbbrevName = Query.QueryDef
            Promoter.Comment = String.Format("{0} -> {1}", ID, Query.QueryDef)
            Promoter.ComponentOf = New List(Of String) From {Query.QueryDef}

            Call Me.ReconstructList.Add(ID, Promoter)
            Return 0
        End Function
    End Class
End Namespace
