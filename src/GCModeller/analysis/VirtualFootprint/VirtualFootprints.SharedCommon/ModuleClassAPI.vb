Imports Microsoft.VisualBasic.Linq
Imports LANS.SystemsBiology.Assembly.KEGG.DBGET
Imports LANS.SystemsBiology.ComponentModel
Imports System.Runtime.CompilerServices

Namespace DocumentFormat

    Public Module ModuleClassAPIExtension

        <Extension> Public Function Fill(Of T As RegulatesFootprints)(api As BriteHEntry.ModuleClassAPI, x As T) As T
            If Not api.Genehash.ContainsKey(x.ORF) Then
                Return x
            End If

            Dim mods As PathwayBrief() = api.Genehash(x.ORF)
            Dim A = __firstCount(mods, api.GetXType)
            Dim B = __firstCount(mods, api.GetXClass)
            Dim C = __firstCount(mods, api.GetXCategory)

            x.Category = C
            x.Class = B
            x.Type = A

            Return x
        End Function

        Private Function __firstCount(mods As PathwayBrief(), __getType As Func(Of PathwayBrief, String)) As String
            Dim cls = (From x In mods Select s = __getType(x) Group s By s Into Count)
            Dim orders = (From x In cls
                          Select x
                          Order By x.Count Descending).First.s
            Return orders
        End Function

        <Extension> Public Function Fill(Of T As RegulatesFootprints)(api As BriteHEntry.ModuleClassAPI, source As IEnumerable(Of T)) As T()
            Dim LQuery = (From x In source.AsParallel Select api.Fill(x)).ToArray
            Return LQuery
        End Function
    End Module
End Namespace