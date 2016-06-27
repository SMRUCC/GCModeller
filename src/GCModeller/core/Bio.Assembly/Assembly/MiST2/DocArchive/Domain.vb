Namespace Assembly.MiST2

    ''' <summary>
    ''' The Microbial Signal Transduction database contains the signal transduction proteins 
    ''' for bacterial and archaeal genomes (2,457 complete and 5,181 draft). These are 
    ''' identified using various domain profiles that directly or indirectly implicate a 
    ''' particular protein in participating in signal transduction.
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure Domain
        Dim PfamId As String
        Dim Id As String
        Dim CommonName As String
        Dim Type As String
        Dim [Function] As String
        Dim Marker As Boolean

        Public Shared Function TryParse(strText As String) As Domain
            Dim Tokens As String() = strText.Split(CChar(","))
            Return New Domain With {
                .PfamId = Tokens(0),
                .Id = Tokens(1),
                .CommonName = Tokens(2),
                .Type = Tokens(3),
                .Function = Tokens(4),
                .Marker = String.Equals("-", Tokens(5))
            }
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("({0}) {1}", PfamId, CommonName)
        End Function

        ''' <summary>
        ''' 从内部的资源文件之中进行加载
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function Load() As Domain()
            Dim DbRecordText As String() = Strings.Split(My.Resources.MiST2, vbCrLf).Skip(1).ToArray
            Dim LQuery As Domain() = (From line As String
                                      In DbRecordText
                                      Select Domain.TryParse(line)).ToArray
            Return LQuery
        End Function
    End Structure
End Namespace