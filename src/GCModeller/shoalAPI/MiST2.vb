#Region "Microsoft.VisualBasic::0a46873fbe6f6b8bc561752978f9ec3f, ..\GCModeller\shoalAPI\MiST2.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.Scripting.MetaData

<[PackageNamespace]("MiST2", Publisher:="xie.guigang@gmail.com", Category:=APICategories.ResearchTools)>
Public Module MiST2API

    <ExportAPI("Read.Xml.MiST2")>
    Public Function ReadMist2(path As String) As LANS.SystemsBiology.Assembly.MiST2.MiST2
        Return path.LoadXml(Of LANS.SystemsBiology.Assembly.MiST2.MiST2)()
    End Function

    <ExportAPI("Get.MiST2_OCS", Info:="Gets the One-Component System regulators from the MiST2 database.")>
    Public Function GetOCSId(mist2 As LANS.SystemsBiology.Assembly.MiST2.MiST2) As String()
        Return (From item In mist2.MajorModules.First.OneComponent Select item.Identifier Distinct).ToArray
    End Function

    <ExportAPI("MiST2.Download")>
    Public Function DownloadData(<MetaData.Parameter("Code.sp", "The bacterial species code for download the MiST2 database.")> code As String) As LANS.SystemsBiology.Assembly.MiST2.MiST2
        Return LANS.SystemsBiology.Assembly.MiST2.WebServices.DownloadData(code)
    End Function

    <Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.DeviceDriver.DriverHandles.IO_DeviceHandle(GetType(LANS.SystemsBiology.Assembly.MiST2.MiST2))>
    <ExportAPI("Write.Xml.MiST2")>
    Public Function WriteData(data As LANS.SystemsBiology.Assembly.MiST2.MiST2, saveto As String) As Boolean
        Return data.GetXml.SaveTo(saveto)
    End Function

    Sub New()
        If Not Settings.Initialized Then
            Call Settings.Initialize()
        End If
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="MiST2"></param>
    ''' <param name="export"></param>
    ''' <param name="grepScript">默认使用fasta标题之中的第一个单词为标识符</param>
    ''' <param name="FromFasta"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("MiST2.Download.ProtSequence",
               Info:="If the fasta sequence source is empty, then the function will trying download the sequence data from KEGG database. " &
                                              "the from_fasta sequence collection should be greped and only have the uniqueid property.")>
    Public Function DownloadMisT2Sequence(MiST2 As LANS.SystemsBiology.Assembly.MiST2.MiST2,
                                          export As String,
                                          <MetaData.Parameter("Script.Grep")> Optional grepScript As String = "tokens ' ' first",
                                          Optional FromFasta As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile = Nothing) _
        As Boolean

        Dim TCS_RR = (From item In {MiST2.MajorModules.First.TwoComponent.HRR, MiST2.MajorModules.First.TwoComponent.RR}.MatrixToVector Select item.Identifier Distinct).ToArray
        Dim TCS_HK = (From item In {MiST2.MajorModules.First.TwoComponent.HHK, MiST2.MajorModules.First.TwoComponent.HisK}.MatrixToVector Select item.Identifier Distinct).ToArray
        Dim OCS = (From item In MiST2.MajorModules.First.OneComponent Select item.Identifier).ToArray
        Dim MCPs = (From item In MiST2.MajorModules.First.Chemotaxis Select item.Identifier).ToArray
        Dim IDList As String() = {TCS_HK, TCS_RR, OCS, MCPs}.MatrixToVector
        Dim List As Object()

        If FromFasta.IsNullOrEmpty Then
            List = (From id As String In IDList Let get_Fasta = __getFasta(id) Select id, fasta = get_Fasta).ToArray
        Else
            Dim Script As Microsoft.VisualBasic.Text.TextGrepScriptEngine = Microsoft.VisualBasic.Text.TextGrepScriptEngine.Compile(grepScript)
            List = (From item In FromFasta Let id As String = Script.Grep(item.Title) Select id, fasta = item).ToArray
            Dim sourceId As String() = (From item In List Let strValue As String = item.id.ToString Select strValue).ToArray
            Dim nullList = (From id As String In IDList Where Array.IndexOf(sourceId, id) = -1 Select id).ToArray '这个是在源之中不存在的

            If Not nullList.IsNullOrEmpty Then
                Dim ChunkTemp = (From id As String In nullList Select id, fasta = __getFasta(id)).ToArray
                List = {List, ChunkTemp}.MatrixToVector
            End If
        End If

        Dim Dict As Dictionary(Of String, LANS.SystemsBiology.SequenceModel.FASTA.FastaToken) =
            List.ToDictionary(Function(obj) obj.id.ToString, elementSelector:=Function(obj) CType(obj.fasta, LANS.SystemsBiology.SequenceModel.FASTA.FastaToken))

        Dim ChunkBuffer As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken()

        ChunkBuffer = (From id As String In TCS_RR.AsParallel Select Dict(id)).ToArray
        Call CType(ChunkBuffer, LANS.SystemsBiology.SequenceModel.FASTA.FastaFile).Save(String.Format("{0}/TCS_RR.fasta", export))
        ChunkBuffer = (From id As String In TCS_HK.AsParallel Select Dict(id)).ToArray
        Call CType(ChunkBuffer, LANS.SystemsBiology.SequenceModel.FASTA.FastaFile).Save(String.Format("{0}/TCS_HisK.fasta", export))
        ChunkBuffer = (From id As String In OCS.AsParallel Select Dict(id)).ToArray
        Call CType(ChunkBuffer, LANS.SystemsBiology.SequenceModel.FASTA.FastaFile).Save(String.Format("{0}/OCS.fasta", export))
        ChunkBuffer = (From id As String In MCPs.AsParallel Select Dict(id)).ToArray
        Call CType(ChunkBuffer, LANS.SystemsBiology.SequenceModel.FASTA.FastaFile).Save(String.Format("{0}/MCPs.fasta", export))

        Return True
    End Function

    Private Function __getFasta(id As String) As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken
        Dim Path As String = String.Format("{0}/KEGG_FASTA____downloaded/{1}.fasta", Settings.DataCache, id)
        If FileIO.FileSystem.FileExists(Path) Then
            Dim FastaObject = LANS.SystemsBiology.SequenceModel.FASTA.FastaToken.Load(Path)

            If String.IsNullOrEmpty(FastaObject.SequenceData) Then
                Return LANS.SystemsBiology.Assembly.KEGG.WebServices.DownloadSequence(id)
            Else
                Return FastaObject
            End If
        Else
            Return LANS.SystemsBiology.Assembly.KEGG.WebServices.DownloadSequence(id)
        End If
    End Function
End Module
