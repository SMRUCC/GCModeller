#Region "Microsoft.VisualBasic::f052b8a546ccc2b382fa6743fb8be71c, core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\ReactionClass.vb"

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

    '     Class ReactionClass
    ' 
    '         Properties: category, definition, entryId, enzymes, orthology
    '                     pathways, reactantPairs, reactions
    ' 
    '         Function: ScanRepository
    ' 
    '     Class ReactionCompoundTransform
    ' 
    '         Properties: [to], from
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace Assembly.KEGG.DBGET.bGetObject

    Public Class ReactionClass : Inherits XmlDataModel

        <XmlAttribute>
        Public Property entryId As String
        Public Property definition As String
        Public Property reactantPairs As ReactionCompoundTransform()
        Public Property reactions As NamedValue()
        Public Property enzymes As NamedValue()
        Public Property pathways As NamedValue()
        Public Property orthology As NamedValue()
        Public Property category As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="repository">a directory which contains reaction class xml model files</param>
        ''' <param name="loadsAll">load all models files or distinct(ignores duplicated in different class category)</param>
        ''' <returns></returns>
        Public Shared Iterator Function ScanRepository(repository As String, Optional loadsAll As Boolean = False) As IEnumerable(Of ReactionClass)
            Dim busy As New SwayBar
            Dim message$
            Dim [class] As ReactionClass
            Dim loaded As New Index(Of String)

            repository = repository.GetDirectoryFullPath

            For Each xml As String In ls - l - r - "*.xml" <= repository
                [class] = xml.LoadXml(Of ReactionClass)
                [class].category = xml.GetFullPath.ParentPath.Replace(repository, "").Trim("\"c, "/"c, " ")
                message = [class].definition

                Call busy.Step(message)

                If Not loadsAll Then
                    If Not [class].entryId Like loaded Then
                        loaded.Add([class].entryId)

                        ' return current file data
                        Yield [class]
                    End If
                Else
                    ' return current file data
                    Yield [class]
                End If
            Next
        End Function
    End Class

    ''' <summary>
    ''' 反应左端代谢物在经过了代谢反应之后结果上的转换变化的结果（反应的右端）
    ''' </summary>
    Public Class ReactionCompoundTransform

        ''' <summary>
        ''' the kegg compound id
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Overridable Property from As String

        ''' <summary>
        ''' the kegg compound id
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Overridable Property [to] As String

        Public Overrides Function ToString() As String
            Return $"{from}->{[to]}"
        End Function
    End Class
End Namespace
