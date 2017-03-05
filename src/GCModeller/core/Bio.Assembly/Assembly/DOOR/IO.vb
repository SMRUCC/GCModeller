Namespace Assembly.DOOR

    ''' <summary>
    ''' Parser and writer
    ''' </summary>
    Public Module IO

        ''' <summary>
        ''' 解析已经读取的文本行为DOOR操纵子集合对象
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Function DocParser(data As String(), path As String) As DOOR
            Dim LQuery As OperonGene() = (From s_Line As String In data.Skip(1)
                                          Where Not String.IsNullOrEmpty(s_Line)
                                          Select OperonGene.TryParse(s_Line)).ToArray
            Dim DOOR As DOOR = New DOOR With {
                .Genes = LQuery,
                .FilePath = path
            }
            DOOR.DOOROperonView = Assembly.DOOR.CreateOperonView(DOOR)
            Return DOOR
        End Function
    End Module
End Namespace