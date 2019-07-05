Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput

Namespace Pipeline.COG.Whog

    Public Module Assigner

        ''' <summary>
        ''' Do COG category assign based on the whog data
        ''' </summary>
        ''' <param name="repo"></param>
        ''' <param name="prot"></param>
        ''' <returns></returns>
        <Extension>
        Public Function DoAssign(repo As WhogRepository, prot As MyvaCOG) As MyvaCOG
            If String.IsNullOrEmpty(prot.MyvaCOG) OrElse String.Equals(prot.MyvaCOG, IBlastOutput.HITS_NOT_FOUND) Then
                ' 没有可以分类的数据
                Return prot
            End If

            Dim Cog = (From entry As Category
                       In repo.Categories
                       Where entry.ContainsGene(prot.MyvaCOG)
                       Select entry).FirstOrDefault

            If Cog Is Nothing Then
                Call $"Could Not found the COG category id for myva cog {prot.QueryName} <-> {prot.MyvaCOG}....".Warning
                Return prot
            End If

            prot.COG = Cog.COG_id
            prot.Category = Cog.category
            prot.Description = Cog.description

            Return prot
        End Function
    End Module
End Namespace