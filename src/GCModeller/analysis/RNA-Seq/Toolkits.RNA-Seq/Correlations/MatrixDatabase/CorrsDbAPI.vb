Imports System.Runtime.CompilerServices
Imports LANS.SystemsBiology.Toolkits.RNA_Seq.WGCNA
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' PCC/sPCC
''' </summary>
''' <param name="g1"></param>
''' <param name="g2"></param>
''' <returns></returns>
Public Delegate Function IsTrue(g1 As String, g2 As String) As Boolean

<PackageNamespace("GCModeller.Gene.Correlations",
                  Publisher:="xie.guigang@gcmodeller.org",
                  Category:=APICategories.ResearchTools)>
Public Module CorrsDbAPI

    <ExportAPI("IsTrue?")>
    <Extension>
    Public Function IsTrueFunc(corr As ICorrelations, Optional cut As Double = 0.65) As IsTrue
        Return AddressOf New __isTRUE With {
            .corrs = corr,
            .cut = cut
        }.IsTrue
    End Function

    Private Class __isTRUE

        Public corrs As ICorrelations
        Public cut As Double

        Public Function IsTrue(g1 As String, g2 As String) As Boolean
            Return corrs.IsTrue(g1, g2, cut)
        End Function
    End Class

    <ExportAPI("IsTrue?")>
    <Extension>
    Public Function IsTrue(corr As ICorrelations, g1 As String, g2 As String, Optional cut As Double = 0.65) As Boolean
        Dim pcc As Double = corr.GetPcc(g1, g2)
        Dim spcc As Double = corr.GetSPcc(g1, g2)
        Return Math.Abs(pcc) >= cut OrElse Math.Abs(spcc) >= cut
    End Function

    <ExportAPI("WGCNA.Fast.Imports")>
    Public Function FastImports(Path As String) As WGCNAWeight
        If Not Path.FileExists Then
            Call VBDebugger.Warning($"{Path.ToFileURL} is not exists on the file system!")
            Return New WGCNAWeight
        End If

        Dim Lines As String() = IO.File.ReadAllLines(Path)
        Dim Tokens = Lines.Skip(1).ToArray(Function(line) Strings.Split(line, vbTab), Parallel:=True)
        Dim weights As WGCNA.Weight() =
            Tokens.ToArray(
                Function(line) New Weight With {
                    .FromNode = line(Scan0),
                    .ToNode = line(1),
                    .Weight = Val(line(2))}, Parallel:=True)
        Return New WGCNAWeight With {.PairItems = weights}
    End Function
End Module
