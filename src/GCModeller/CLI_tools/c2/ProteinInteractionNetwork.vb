Imports System.Text

Public Class ProteinInteractionNetwork

    Dim LocalBlast As LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.InteropService.InteropService,
        Pfam As LANS.SystemsBiology.Assembly.NCBI.CDD.DbFile,
        DOMINE As LANS.SystemsBiology.Assembly.DOMINE.Database

    Sub New(LocalBlast As LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.InteropService.InteropService, CDDDir As String, DOMINE As String)
        Me.LocalBlast = LocalBlast
        Me.DOMINE = DOMINE.LoadXml(Of LANS.SystemsBiology.Assembly.DOMINE.Database)()

        Using CDD As LANS.SystemsBiology.Assembly.NCBI.CDD.DomainInfo.CDDLoader = New LANS.SystemsBiology.Assembly.NCBI.CDD.DomainInfo.CDDLoader(CDDDir)
            Me.Pfam = CDD.GetPfam
            Call LocalBlast.FormatDb(Pfam.FastaUrl, LocalBlast.MolTypeProtein).Start(WaitForExit:=True)
        End Using
    End Sub

    ''' <summary>
    ''' 获取蛋白质集合可能的相互作用关系
    ''' </summary>
    ''' <param name="Proteins">目标待构建蛋白质互作网络的蛋白质组，FASTA序列文件格式</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function InvokeAction(Proteins As String) As LANS.SystemsBiology.InteractionModel.ProteinInteractionNetwork.Interaction()
        Call LocalBlast.FormatDb(Proteins, LocalBlast.MolTypeProtein).Start(WaitForExit:=True)
        Call LocalBlast.Blastp(Proteins, Pfam.FastaUrl, Settings.TEMP & "/BLAST-Pfam.xml") '.Start(WaitForExit:=True)

        Dim Log = LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.BLASTOutput.XmlFile.BlastOutput.LoadFromFile(LocalBlast.LastBLASTOutputFilePath)
        Dim Script = Microsoft.VisualBasic.Text.TextGrepScriptEngine.Compile("tokens | 4")
        Call Log.Grep(AddressOf Script.Grep, Nothing)

        Dim FsaData = LANS.SystemsBiology.SequenceModel.FASTA.FastaFile.Read(Pfam.FastaUrl)
        Dim ProteinDAs = (From Query In Log.Iterations Select LANS.SystemsBiology.AnalysisTools.ProteinTools.SMART.CompileDomains.CreateProteinDescription(Query, Nothing, FsaData, Pfam)).ToArray

        Return LANS.SystemsBiology.InteractionModel.ProteinInteractionNetwork.BuildInteraction(ProteinDAs, DOMINE)
    End Function
End Class
