Imports System.Text
Imports System.Xml.Serialization
Imports LANS.SystemsBiology.SequenceModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic

<XmlRoot("ProtInDb", Namespace:="http://protindb.cs.iastate.edu/Publications.py")>
Public Class ProtInDb

    <XmlElement> Public Property ProteinChainsInfo As ProteinChain()

    Public Function ExportDatabase(ExportDir As String) As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile
        Dim LQuery = (From item As ProteinChain In Me.ProteinChainsInfo.AsParallel
                      Let SurfaceBuilder = Function() As String
                                               Dim sBuilder As StringBuilder = New StringBuilder(1024)
                                               Dim Surface = item.Surface
                                               For Each n In Surface
                                                   Call sBuilder.Append(n)
                                               Next

                                               Return sBuilder.ToString
                                           End Function Select New LANS.SystemsBiology.SequenceModel.FASTA.FastaToken With {
                                               .SequenceData = item.SequenceData,
                                               .Attributes = New String() {item.PdbId & "-" & item.ChainId & " " & SurfaceBuilder()}}).ToArray

        Call FileIO.FileSystem.CreateDirectory(ExportDir)

        Dim FastaObject = CType(LQuery, LANS.SystemsBiology.SequenceModel.FASTA.FastaFile)
        Call FastaObject.Save(String.Format("{0}/ProtInDb.fsa", ExportDir))

        Dim CsvData As StringBuilder = New StringBuilder(1024 * 1024)
        Call CsvData.AppendLine("PdbId,ChainId,itrChainId,Interface")
        For Each item In Me.ProteinChainsInfo
            For Each itrId In item.InterfaceOnSurface
                Dim sBuilder As StringBuilder = New StringBuilder(1024)
                For Each n In itrId.Value
                    Call sBuilder.Append(n)
                Next
                Call CsvData.AppendLine(String.Format("{0},{1},{2},{3}", item.PdbId, item.ChainId, itrId.Key, sBuilder.ToString))
            Next
        Next

        Call CsvData.ToString.SaveTo(String.Format("{0}/ProtInDb-assembly.csv", ExportDir))

        Return FastaObject
    End Function

    Public Function ExtractInteractions(PdbId As String, ChainId As String) As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile
        Dim LQuery = (From item In Me.ProteinChainsInfo.AsParallel Where String.Equals(item.PdbId, PdbId) Select item).ToArray
        Dim Target = (From item In LQuery Where String.Equals(item.ChainId, ChainId) Select item).First
        Dim InteractionChains = (From item In Target.InterfaceOnSurface Select item.Key).ToArray
        LQuery = (From item In LQuery Where Array.IndexOf(InteractionChains, item.ChainId) > -1 Select item).ToArray
        Dim FastaData = (From item In LQuery Select item.ToFASTA).ToArray
        Return FastaData
    End Function

    Public Sub ExportInteractions(ExportedDir As String)
        Dim LQuery = (From item In Me.ProteinChainsInfo.AsParallel
                      Let Interactions = Function() As String
                                             Dim sBuilder As StringBuilder = New StringBuilder(64)
                                             For Each itr In item.InterfaceOnSurface
                                                 Call sBuilder.Append(itr.Key & ", ")
                                             Next
                                             Call sBuilder.Remove(sBuilder.Length - 2, 2)
                                             Return sBuilder.ToString
                                         End Function Let row As String = String.Format("{0},{1},""{2}""", item.PdbId, item.ChainId, Interactions())
                      Select row
                      Order By row Ascending).ToList
        Call LQuery.Insert(0, "PdbId,ChainId,InteractionChainId")
        Call IO.File.WriteAllLines(String.Format("{0}/ProtInDb_Complexes-Assembly.csv", ExportedDir), LQuery.ToArray)
    End Sub

    Public Shared Function Load(InfoDir As String) As ProtInDb
        Dim Files = (From Path As String In FileIO.FileSystem.GetFiles(InfoDir, FileIO.SearchOption.SearchTopLevelOnly, "*.txt").AsParallel Select ProteinChain.TryParse(Path)).ToArray
        Dim PdbIdList = (From File In Files Select File.PdbId Distinct).ToArray
        Return New ProtInDb With {.ProteinChainsInfo = Files}
    End Function
End Class
