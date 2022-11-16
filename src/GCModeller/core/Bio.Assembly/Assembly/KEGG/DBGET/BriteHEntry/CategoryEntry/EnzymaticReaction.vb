#Region "Microsoft.VisualBasic::6a754adf102b900a13d9bbbc6a7db53b, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\BriteHEntry\CategoryEntry\EnzymaticReaction.vb"

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

    '   Total Lines: 182
    '    Code Lines: 126
    ' Comment Lines: 29
    '   Blank Lines: 27
    '     File Size: 7.21 KB


    '     Class EnzymaticReaction
    ' 
    '         Properties: [Class], Category, EC, Entry, SubCategory
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: Build, doLongFileNameTrim, DownloadReactions, getCategorySaveDirectory, KEGGrxns
    '                   LoadFile, LoadFromResource, loadSource, ToString
    ' 
    '         Sub: downloaderInternal
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

Namespace Assembly.KEGG.DBGET.BriteHEntry

    ''' <summary>
    ''' Extract data from the <see cref="htext"/> entry model
    ''' </summary>
    Public Class EnzymaticReaction

        ''' <summary>
        ''' level: D
        ''' </summary>
        ''' <returns></returns>
        Public Property EC As String
        ''' <summary>
        ''' level: A
        ''' </summary>
        ''' <returns></returns>
        Public Property [Class] As String
        ''' <summary>
        ''' level: B
        ''' </summary>
        ''' <returns></returns>
        Public Property Category As String
        ''' <summary>
        ''' level: C
        ''' </summary>
        ''' <returns></returns>
        Public Property SubCategory As String
        Public Property Entry As KeyValuePair

        ''' <summary>
        ''' br08201
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function LoadFromResource() As EnzymaticReaction()
            Dim model As BriteHText = BriteHTextParser.Load(My.Resources.br08201)
            Return Build(model)
        End Function

        Protected Shared Function Build(Model As BriteHText) As EnzymaticReaction()
            Dim out As New List(Of EnzymaticReaction)

            For Each [class] As BriteHText In Model.categoryItems
                For Each category As BriteHText In [class].categoryItems
                    For Each subCategory As BriteHText In category.categoryItems

                        If subCategory.categoryItems.IsNullOrEmpty Then
                            Continue For
                        End If

                        For Each EC As BriteHText In subCategory.categoryItems
                            If Not EC.categoryItems.IsNullOrEmpty Then
                                out += KEGGrxns(EC, [class], category, subCategory)
                            End If
                        Next
                    Next
                Next
            Next

            Return out.ToArray
        End Function

        Private Shared Function KEGGrxns(EC As BriteHText, [class] As BriteHText, category As BriteHText, subCat As BriteHText) As EnzymaticReaction()
            Dim LQuery = LinqAPI.Exec(Of EnzymaticReaction) <=
 _
                From rxn As BriteHText
                In EC.categoryItems
                Let erxn As EnzymaticReaction = New EnzymaticReaction With {
                    .EC = EC.classLabel,
                    .Category = category.classLabel,
                    .Class = [class].classLabel,
                    .SubCategory = subCat.classLabel,
                    .Entry = New KeyValuePair With {
                        .Key = rxn.entryID,
                        .Value = rxn.description
                    }
                }
                Select erxn

            Return LQuery
        End Function

        Public Shared Function LoadFile(path As String) As EnzymaticReaction()
            Return Build(BriteHTextParser.Load(FileIO.FileSystem.ReadAllText(path)))
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("[{0}: {1}] {2}", String.Join("/", [Class], Category, SubCategory), EC, Entry.ToString)
        End Function

        Private Shared Function loadSource(path$) As EnzymaticReaction()
            If Not path.FileLength > 0 Then
                Return LoadFromResource()
            Else
                Return LoadFile(path)
            End If
        End Function

    End Class
End Namespace
