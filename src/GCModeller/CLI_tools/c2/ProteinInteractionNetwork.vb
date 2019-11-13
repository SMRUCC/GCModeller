#Region "Microsoft.VisualBasic::5d2bdd327ef2db965c2a861876cebd92, CLI_tools\c2\ProteinInteractionNetwork.vb"

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

    ' Class ProteinInteractionNetwork
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: InvokeAction
    ' 
    ' /********************************************************************************/

#End Region

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
