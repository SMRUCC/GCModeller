
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools.WGCNA
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools.WGCNA.Network
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports any = Microsoft.VisualBasic.Scripting

<Package("WGCNA")>
Module WGCNA

    <ExportAPI("read.modules")>
    Public Function readModules(file As String) As Object
        Return WGCNAModules _
            .LoadModules(file) _
            .ToDictionary(Function(g) g.nodeName,
                          Function(g)
                              Return CObj(g.nodesPresent)
                          End Function) _
            .DoCall(Function(mods)
                        Return New list With {
                            .slots = mods
                        }
                    End Function)
    End Function

    <ExportAPI("read.weightMatrix")>
    Public Function readWeightMatrix(file As String, Optional threshold As Double = 0) As WGCNAWeight
        Return FastImports(path:=file, threshold:=threshold)
    End Function

    <ExportAPI("applyModuleColors")>
    Public Function applyModuleColors(g As NetworkGraph, modules As list) As Object
        For Each geneId As String In modules.getNames
            If Not g.GetElementByID(geneId) Is Nothing Then
                g.GetElementByID(geneId).data.color = any.ToString(modules(geneId)).GetBrush
            End If
        Next

        Return g
    End Function
End Module
