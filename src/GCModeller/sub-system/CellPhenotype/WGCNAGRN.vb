Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.genomics.Analysis.BNLearn.Core

''' <summary>
''' Gene regulatory network based on the WGCNA correlation network and bnlearn network
''' </summary>
Public Module WGCNAGRN

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="wgcna">WGCNA co-expression network</param>
    ''' <param name="TF">a list of the gene id(network vectex node label) which has been annotated as TF</param>
    ''' <returns></returns>
    Public Function BuildBNNetwork(wgcna As NetworkGraph, TF As String()) As BNLearnWorkflow

    End Function

End Module
