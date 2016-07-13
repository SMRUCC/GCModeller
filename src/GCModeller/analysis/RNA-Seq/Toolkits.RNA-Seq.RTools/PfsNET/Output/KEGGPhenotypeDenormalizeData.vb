Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat

Namespace PfsNET.TabularArchives

    Public Class KEGGPhenotypeDenormalizeData

        Public Property [Class] As String
        Public Property Category As String
        ''' <summary>
        ''' 使用结果文件名来表示
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property PhenotypePair As String
        Public Property UniqueId As String
        Public Property PathwayDescription As String
        Public Property Statistics As Double
        Public Property PValue As Double
        Public Property GeneId As String
        Public Property GeneFunction As String

        Public Shared Function Denormalize(phenotypeData As IEnumerable(Of KEGGPhenotypes), PTT_DIR As String) As KEGGPhenotypeDenormalizeData()
            Dim bufs As New List(Of KEGGPhenotypeDenormalizeData)
            Dim PTT As New PTTDbLoader(PTT_DIR)

            For Each phen As KEGGPhenotypes In phenotypeData
                Dim funcs As KEGGPhenotypeDenormalizeData() =
                    LinqAPI.Exec(Of KEGGPhenotypeDenormalizeData) <=
 _
                        From locus As String
                        In phen.SignificantGeneObjects
                        Let gFuncs As KEGGPhenotypeDenormalizeData = Denormalize(PTT, locus, phen)
                        Select gFuncs

                bufs += funcs
            Next

            Return bufs.ToArray
        End Function

        Private Shared Function Denormalize(PTT As PTTDbLoader, locus As String, phen As KEGGPhenotypes) As KEGGPhenotypeDenormalizeData
            Dim dnData As New KEGGPhenotypeDenormalizeData
            Dim PttGene = PTT(locus)

            dnData.GeneId = locus
            dnData.GeneFunction = PttGene.Product
            dnData.Category = phen.Category
            dnData.Class = phen.Class
            dnData.PathwayDescription = phen.Description
            dnData.PhenotypePair = phen.PhenotypePair
            dnData.PValue = phen.PValue
            dnData.Statistics = phen.Statistics
            dnData.UniqueId = phen.UniqueId

            Return dnData
        End Function
    End Class
End Namespace