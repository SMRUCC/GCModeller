#Region "Microsoft.VisualBasic::64f75cb3e736052469e6a37ec9a9d7df, RDotNet.Extensions.Bioinformatics\Wrappers\BnLearn\NetworkParameters.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    '     Class NetworkParameters
    ' 
    '         Function: __R_script, GetNetworkParameters
    ' 
    '         Sub: New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language
Imports RDotNET.Extensions.VisualBasic

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

            Call "library(bnlearn); 
pdag = iamb(learning.test); 
dag = set.arc(pdag, from = ""A"", to = ""B""); 
fit = bn.fit(dag, learning.test, method = ""bayes"");".__call

            Dim LQuery = LinqAPI.Exec(Of ConditionalProbability) <=
                From s As String
                In "fit".__call.AsCharacter.ToArray
                Let Data As String = s.Replace(vbCr, "").Replace(vbLf, "")
                Select ConditionalProbability.TryParse(Data, numberOfFactors)
            Return LQuery
        End Function

        Protected Overrides Function __R_script() As String
            Return BayesianNetworkObject
        End Function
    End Class
End Namespace
