#Region "Microsoft.VisualBasic::8a85565c34f344d3e1d8b79d5aee853b, GCModeller\models\SBML\SBML\Export\Intersection.vb"

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

    '   Total Lines: 71
    '    Code Lines: 57
    ' Comment Lines: 7
    '   Blank Lines: 7
    '     File Size: 3.50 KB


    '     Module Intersection
    ' 
    '         Function: __setNOTE, KEGGReactions, LoadReactions
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.Utility
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.Model.SBML.Level2

Namespace ExportServices

    <Package("SBML.KEGG.Intersection")>
    Public Module Intersection

        ''' <summary>
        ''' 返回来的对象里面的Note的Text属性是相对应的KEGG里面的代谢过程的Id编号
        ''' </summary>
        ''' <param name="sbml"></param>
        ''' <param name="KEGG"></param>
        ''' <returns></returns>
        <ExportAPI("IntersectSets")>
        <Extension> Public Function KEGGReactions(sbml As XmlFile, KEGG As IEnumerable(Of bGetObject.Reaction)) As Elements.Reaction()
            Dim KEGGModels = KEGG.Select(Function(x) New KeyValuePairObject(Of bGetObject.Reaction, Equation)(x, x.ReactionModel)).ToArray
            Dim pendings = sbml.GetReactions.ToDictionary(Function(x) x, Function(x) x.ReactionModel)
            Dim LQuery = (From x As KeyValuePair(Of bGetObject.Reaction, Equation)
                          In pendings.AsParallel
                          Let kgRxn = (From xx In KEGGModels
                                       Where xx.Value.Equals(x.Value, False)
                                       Select xx).FirstOrDefault  ' 查找出KEGG数据库之中的可能的等价的代谢过程
                          Where Not kgRxn Is Nothing
                          Select x,
                              kgRxn.Key).ToArray
            Dim hash As Dictionary(Of String, Elements.Reaction) =
                sbml.Model.listOfReactions.ToDictionary(Function(x) x.id)
            Dim out As Elements.Reaction() = (From x In LQuery
                                              Let sbmlRxn As Elements.Reaction = hash(x.x.Key.ID)
                                              Let kgId As String = x.Key.ID
                                              Select __setNOTE(sbmlRxn, kgId)).ToArray
            Return out
        End Function

        Private Function __setNOTE(sbmlRxn As Elements.Reaction, note As String) As Elements.Reaction
            If sbmlRxn.Notes Is Nothing Then
                sbmlRxn.Notes = New Components.Notes
            End If
            If sbmlRxn.Notes.body Is Nothing Then
                sbmlRxn.Notes.body = New Components.Body
            End If
            ' Finally
            sbmlRxn.Notes.body.Text = note

            Return sbmlRxn
        End Function

        <ExportAPI("KEGGs.Load")>
        Public Function LoadReactions(repository As String) As bGetObject.Reaction()
            Using cbar As New CBusyIndicator(start:=True)
                Call Console.WriteLine("Loading KEGG reaction models from the repository:  ")
                Call Console.WriteLine(repository)

                Dim files = (From file As String
                             In FileIO.FileSystem.GetFiles(repository,
                                 FileIO.SearchOption.SearchAllSubDirectories,
                                 "*.xml").AsParallel
                             Select file.LoadXml(Of bGetObject.Reaction)).ToArray
                Return files
            End Using
        End Function
    End Module
End Namespace
