Namespace Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES

    Public Delegate Function SearchMethod(data As FEATURES, keyword As String) As Feature

    ''' <summary>
    ''' 可以使用本模块内的方法搜索<see cref="NCBI.GenBank.GBFF.Keywords.FEATURES"></see>模块之中的内容
    ''' </summary>
    ''' <remarks></remarks>
    Public Class SearchInvoker

        Dim GBFF As File

#Region "Delegate Methods for Delegate Invoker"

        Public Shared ReadOnly Property SearchBy_GI As SearchMethod
            Get
                Return AddressOf SearchInvoker._SearchBy_GI
            End Get
        End Property

        Protected Shared Function _SearchBy_GI(data As FEATURES, keyword As String) As Feature
            Dim LQuery = (From b As Feature
                          In data._innerList
                          Let id As String = b.Query(FeatureQualifiers.db_xref)
                          Let gi = id.Replace("GI:", "")
                          Where String.Equals(gi, keyword)
                          Select b).FirstOrDefault
            Return LQuery
        End Function
#End Region

        ''' <summary>
        ''' Delegate invoker
        ''' </summary>
        ''' <param name="method"></param>
        ''' <param name="keyword"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Search(method As SearchMethod, keyword As String) As Feature
            Return method(GBFF.Features, keyword)
        End Function

        Sub New(gbk As File)
            GBFF = gbk
        End Sub

        Public Overrides Function ToString() As String
            Return GBFF.ToString
        End Function
    End Class
End Namespace