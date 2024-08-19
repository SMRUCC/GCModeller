﻿#Region "Microsoft.VisualBasic::6e6ccd7f26ca68266f8f19ed51c60fae, data\SABIO-RK\SBML\SbmlDocument.vb"

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


' Code Statistics:

'   Total Lines: 61
'    Code Lines: 48 (78.69%)
' Comment Lines: 1 (1.64%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 12 (19.67%)
'     File Size: 2.45 KB


'     Class SbmlDocument
' 
'         Properties: mathML, sbml
' 
'         Function: getLambda, LoadDocument
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.MIME.application.xml
Imports Microsoft.VisualBasic.MIME.application.xml.MathML
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Model.SBML.Level3

Namespace SBML

    Public Class SbmlDocument

        Public Property sbml As XmlFile(Of SBMLReaction)
        Public Property mathML As NamedValue(Of LambdaExpression)()

        Public Shared Function LoadXml(xml As String, <Out> Optional ByRef text As String = Nothing) As XmlFile(Of SBMLReaction)
            text = xml.SolveStream

            If text.StringEmpty(, True) OrElse
                (text.Trim(" "c, ASCII.TAB, ASCII.CR, ASCII.LF) = "No results found for query") OrElse
                (text = vbCrLf) Then

                Return Nothing
            Else
                Return text.LoadFromXml(Of XmlFile(Of SBMLReaction))
            End If
        End Function

        Public Shared Function LoadDocument(path As String) As SbmlDocument
            Dim text As String = Nothing
            Dim sbml As XmlFile(Of SBMLReaction) = LoadXml(path, text)
            Dim math = MathMLParser.ParseMathML(sbmlText:=text).ToArray
            Dim formulas As NamedValue(Of LambdaExpression)() = Math _
                .Select(Function(a)
                            Return New NamedValue(Of LambdaExpression) With {
                                .Name = a.Name,
                                .Description = a.Description,
                                .Value = getLambda(a.Value)
                            }
                        End Function) _
                .ToArray

            Return New SbmlDocument With {
                .sbml = sbml,
                .mathML = formulas
            }
        End Function

        Shared ReadOnly defaultParameters As Index(Of String) = {"Km", "kcat", "E"}

        Private Shared Function getLambda(xml As XmlElement) As LambdaExpression
            Dim lambda = LambdaExpression.FromMathML(xml)

            If lambda.lambda Is Nothing Then
                If lambda.parameters.Length = 3 AndAlso
                    lambda.parameters.Any(Function(a) a = "Km") AndAlso
                    lambda.parameters.Any(Function(a) a = "kcat") AndAlso
                    lambda.parameters.Any(Function(a) a = "E") Then

                    Return DefaultKineticis()
                Else
                    ' Throw New NotImplementedException(lambda.parameters.JoinBy(" | "))
                    Return lambda
                End If
            End If

            Return lambda
        End Function
    End Class
End Namespace
