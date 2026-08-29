Imports Microsoft.VisualBasic.Text

Namespace Tabular.Tsv

    ''' <summary>
    ''' interaction types for protein links.
    ''' (从String-DB之中下载的蛋白质互作网络数据，例如：``9606.protein.actions.v10.txt``，
    ''' 这个对象只是存在注释数据的互作关系，只是所有的互作关系之中研究比较明白的网络部分，
    ''' 假若查看所有的网络数据在``9606.protein.links.v10.txt``文件之中)
    ''' </summary>
    Public Class LinkAction

        Public Property item_id_a As String
        Public Property item_id_b As String
        Public Property mode As String
        Public Property action As String
        Public Property a_is_acting As String
        Public Property score As String

        Public Shared Iterator Function LoadText(path As String) As IEnumerable(Of LinkAction)
            For Each line As String In path.IterateAllLines.Skip(1)
                Dim tokens As String() = line.Split(ASCII.TAB)

                Yield New LinkAction With {
                    .item_id_a = tokens(0),
                    .item_id_b = tokens(1),
                    .mode = tokens(2),
                    .action = tokens(3),
                    .a_is_acting = tokens(4),
                    .score = tokens(5)
                }
            Next
        End Function
    End Class
End Namespace