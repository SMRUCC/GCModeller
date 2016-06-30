Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic
Imports RDotNet.Extensions.VisualBasic

Namespace bnlearn

    Public Class NetworkParameters : Inherits bnlearn

        Dim BayesianNetworkObject As String

        ''' <summary>
        ''' 贝叶斯网络对象的变量名
        ''' </summary>
        ''' <param name="ObjectName"></param>
        ''' <remarks></remarks>
        Sub New(ObjectName As String)
            BayesianNetworkObject = ObjectName
        End Sub

        ''' <summary>
        ''' Get Bayesian network parameters
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        Public Function GetNetworkParameters(numberOfFactors As Integer) As ConditionalProbability()
            Call RServer.WriteLine("library(bnlearn); pdag = iamb(learning.test) ; dag = set.arc(pdag, from = ""A"", to = ""B"") ; fit = bn.fit(dag, learning.test, method = ""bayes"") ;")
            Dim DataChunk As String() = RServer.WriteLine("fit")
            Dim LQuery = (From strData As String In DataChunk Select ConditionalProbability.TryParse(strData.Replace(vbCr, "").Replace(vbLf, ""), numberOfFactors)).ToArray
            Return LQuery
        End Function

        Protected Overrides Function __R_script() As String
            Return BayesianNetworkObject
        End Function
    End Class
End Namespace