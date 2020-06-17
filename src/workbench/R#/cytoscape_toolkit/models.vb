Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Visualize.Cytoscape.Tables
Imports SMRUCC.Rsharp.Runtime.Interop
Imports REnv = SMRUCC.Rsharp.Runtime
Imports stdNum = System.Math

''' <summary>
''' api for create network graph model for cytoscape
''' </summary>
<Package("models", Category:=APICategories.UtilityTools, Publisher:="xie.guigang@gcmodeller.org")>
Module models

    <ExportAPI("sif")>
    Public Function sif(<RRawVectorArgument> source As Object, <RRawVectorArgument> interaction As Object, <RRawVectorArgument> target As Object) As SIF()
        Dim U As String() = REnv.asVector(Of String)(source)
        Dim type As String() = REnv.asVector(Of String)(interaction)
        Dim V As String() = REnv.asVector(Of String)(target)

        Return Iterator Function() As IEnumerable(Of SIF)
                   Dim n As Integer = stdNum.Max(U.Length, V.Length)

                   For i As Integer = 0 To n - 1
                       Yield New SIF With {
                           .interaction = type.ElementAtOrDefault(i),
                           .source = U.ElementAtOrDefault(i),
                           .target = V.ElementAtOrDefault(i)
                       }
                   Next
               End Function().ToArray
    End Function
End Module
