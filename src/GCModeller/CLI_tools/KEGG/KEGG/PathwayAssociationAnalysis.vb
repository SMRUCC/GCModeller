Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.DataMining.Framework.AprioriAlgorithm
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic

''' <summary>
''' 代谢途径功能关联分析
''' </summary>
''' <remarks></remarks>
Module PathwayAssociationAnalysis

    Private Function __tr(row As DocumentStream.RowObject, items As Char()) As String
        Dim values = (From col In row.Skip(2) Select CInt(Val(col))).ToArray
        Dim chars = (From i As Integer In values.Sequence Where values(i) = 1 Select items(i)).ToArray
        Return New String(chars)
    End Function

    ''' <summary>
    ''' 每一列都为一个事件，一行为一个Transaction
    ''' </summary>
    ''' <param name="Df"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Analysis(Df As DocumentStream.File) As Entities.Output()
        Dim Genes = (From col As String In Df.First.Skip(2) Select col).ToArray
        Dim ChrW As Char() = EncodingServices.GenerateCodes(Genes.Length)
        Dim items = (From i As Integer In Genes.Sequence Select ChrW(i + 1)).ToArray      '创建映射
        Dim Mappings = (From i As Integer In items.Sequence
                        Select code = items(i),
                            GeneName = Genes(i)).ToDictionary(Function(obj) obj.code)
        Dim Transactions = (From row In Df.Skip(1).AsParallel
                            Let Tr As String = __tr(row, items)
                            Select [Class] = row.First,
                                Transcation = Tr
                            Group By [Class] Into Group)    '生成事务
        Dim GetResultLQuery = (From [Class] In Transactions.AsParallel
                               Let trans = (From obj In [Class].Group.ToArray Select obj.Transcation).ToArray
                               Let Result = AlgorithmInvoker.CreateObject.AnalysisTransactions(trans, Items:=items)
                               Let StrongRules = Result.StrongRules.ToArray
                               Let FrequentItems = Result.FrequentItems.Values.ToArray
                               Let MappedResult = (From r As Entities.Rule
                                                   In StrongRules
                                                   Let XMappings = (From c In r.X Select Mappings(c).GeneName).ToArray
                                                   Let YMappings = (From c In r.Y Select Mappings(c).GeneName).ToArray
                                                   Let X = String.Join(" + ", XMappings)
                                                   Let Y = String.Join(" + ", YMappings)
                                                   Select r.Confidence, X, Y, XMappings, YMappings).ToArray
                               Select [Class],
                                   StrongRules,
                                   FrequentItems,
                                   MappedResult,
                                   Result,
                                   GeneIDs = (From s As String In (From item In MappedResult Select item.XMappings.JoinAsIterator(item.YMappings)).MatrixAsIterator
                                              Select s Distinct Order By s Ascending).ToArray).ToArray

        Call (From obj In GetResultLQuery Select (From rule In obj.MappedResult Select obj.Class.Class, rule.Confidence, rule.X, rule.Y).ToArray).ToArray.MatrixToVector.SaveTo(FileIO.FileSystem.GetParentPath(Df.FilePath) & "./Apriori/strong.rules.csv", False)
        Call (From obj In GetResultLQuery Select (From item In obj.FrequentItems Select item.Name, obj.Class, item.Support).ToArray).ToArray.MatrixToVector.SaveTo(FileIO.FileSystem.GetParentPath(Df.FilePath) & "./Apriori/FrequentItems.csv", False)

        Dim GeneIDls = (From obj In GetResultLQuery
                        Select obj.Class.Class,
                            ClsGeneIDs = String.Join(" + ", obj.GeneIDs)).ToArray
        Call GeneIDls.SaveTo(FileIO.FileSystem.GetParentPath(Df.FilePath) & "./Apriori/Class.GeneIDs.csv", False)

        Dim MAT = New DocumentStream.File
        Call MAT.Add({"Class"})
        Call MAT.Last.AddRange(Genes)

        For Each Line In GetResultLQuery
            Dim row As New List(Of String)
            Dim dist = (From Id As String In Genes
                        Let value = If(Array.IndexOf(Line.GeneIDs, Id) > -1, "1", "0")
                        Select value).ToArray

            Call row.Add(Line.Class.Class)
            Call row.AddRange(dist)
            Call MAT.Add(row)
        Next
        Call MAT.Save(FileIO.FileSystem.GetParentPath(Df.FilePath) & "./Apriori/MAT.csv", False)

        Return (From obj In GetResultLQuery Select obj.Result).ToArray
    End Function
End Module
