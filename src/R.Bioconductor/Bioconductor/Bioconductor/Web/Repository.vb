#Region "Microsoft.VisualBasic::fe7d44b373a9eb4c90138490f7e09fba, Bioconductor\Bioconductor\Web\Repository.vb"

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

    '     Class Repository
    ' 
    '         Properties: annotation, DefaultFile, experiment, softwares, version
    ' 
    '         Function: GetPackage, Load, LoadDefault, (+2 Overloads) Save, Search
    '                   ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.R.CRAN.Bioconductor.Web.Packages

Namespace Web

    Public Class Repository : Implements ISaveHandle

        Public Property version As Version
        Public Property softwares As Package()
            Get
                Return _softwares
            End Get
            Set(value As Package())
                _softwares = value
                If _softwares Is Nothing Then
                    __softwares = New Dictionary(Of Package)
                Else
                    __softwares = value.ToDictionary
                End If
            End Set
        End Property
        Public Property annotation As Package()
            Get
                Return _annotation
            End Get
            Set(value As Package())
                _annotation = value
                If _annotation Is Nothing Then
                    __annotation = New Dictionary(Of Package)
                Else
                    __annotation = value.ToDictionary
                End If
            End Set
        End Property
        Public Property experiment As Package()
            Get
                Return _experiment
            End Get
            Set(value As Package())
                _experiment = value
                If _experiment Is Nothing Then
                    __experiment = New Dictionary(Of Package)
                Else
                    __experiment = value.ToDictionary
                End If
            End Set
        End Property

        Dim _softwares, _annotation, _experiment As Package()
        Dim __softwares As Dictionary(Of Package)
        Dim __annotation As Dictionary(Of Package)
        Dim __experiment As Dictionary(Of Package)

        Public Shared ReadOnly Property DefaultFile As String =
            App.ProductSharedDIR & "/biocLite.json"

        Public Overrides Function ToString() As String
            Return version.ToString
        End Function

        Public Shared Function Load(file As String) As Repository
            Try
                Return LoadJsonFile(Of Repository)(file)
            Catch ex As Exception
                ex = New Exception(file, ex)

                Dim __new As Repository = New WebService().Repository
                Call __new.Save(file, Encodings.ASCII)
                Return __new
            End Try
        End Function

        Public Function GetPackage(name As String) As Package
            If __softwares.ContainsKey(name) Then
                Return __softwares(name)
            End If
            If __annotation.ContainsKey(name) Then
                Return __annotation(name)
            End If
            If __experiment.ContainsKey(name) Then
                Return __experiment(name)
            End If
            Return Nothing
        End Function

        Public Function Search(term As String, software As Boolean, experiments As Boolean, annotations As Boolean) As Package()
            Dim result As New List(Of Package)

            If software Then
                result += (From x As Package In __softwares.Values.AsParallel Where x.Match(term) Select x).ToArray
            End If
            If experiments Then
                result += (From x As Package In __experiment.Values.AsParallel Where x.Match(term) Select x).ToArray
            End If
            If annotations Then
                result += (From x As Package In __annotation.Values.AsParallel Where x.Match(term) Select x).ToArray
            End If

            Return result.ToArray
        End Function

        Public Shared Function LoadDefault() As Repository
            Return Load(DefaultFile)
        End Function

        Public Function Save(path As String, encoding As Encoding) As Boolean Implements ISaveHandle.Save
            Return Me.GetJson.SaveTo(path, encoding)
        End Function

        Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return Me.Save(path, encoding.CodePage)
        End Function
    End Class
End Namespace
