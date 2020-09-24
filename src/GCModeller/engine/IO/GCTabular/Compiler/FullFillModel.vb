#Region "Microsoft.VisualBasic::658c25bb66f0668fa62238dbddef08b8, engine\IO\GCTabular\Compiler\FullFillModel.vb"

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

    ' Class FullFillModel
    ' 
    '     Properties: ModelLoader
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Sub: FullFillModel_Kegg, FullFillModel_Sabiork, WriteData
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports SMRUCC.genomics.Assembly.Expasy.AnnotationsTool
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.IO

Public Class FullFillModel

    Public Property ModelLoader As XmlresxLoader
    Dim _MetaCyc As DatabaseLoadder
    Dim _KEGGCompounds As bGetObject.Compound()
    Dim _KeggReactions As bGetObject.Reaction()

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="CellSystemXml"><see cref="FileStream.XmlFormat.CellSystemXmlModel"></see></param>
    ''' <remarks></remarks>
    Sub New(CellSystemXml As String, MetaCyc As String)
        Me._ModelLoader = New FileStream.IO.XmlresxLoader(CellSystemXml)
        Me._MetaCyc = DatabaseLoadder.CreateInstance(MetaCyc, False)
    End Sub

    Public Sub FullFillModel_Kegg(KEGGCompoundsCsv As String, KEGGReactionsCsv As String, CARMENCsv As String)
        Call New Compiler.Components.MergeKEGGCompounds(_ModelLoader, KEGGCompoundsCsv).InvokeMergeCompoundSpecies()
        Call New Compiler.Components.MergeKEGGReactions(_ModelLoader, KEGGReactionsCsv, KEGGCompoundsCsv, CARMENCsv).InvokeMethods()
        _KEGGCompounds = KEGGCompoundsCsv.LoadCsv(Of bGetObject.Compound)(False).ToArray
        _KeggReactions = KEGGReactionsCsv.LoadCsv(Of bGetObject.Reaction)(False).ToArray
    End Sub

    Public Sub FullFillModel_Sabiork(SabiorkCompoundsCsv As String, SabiorkKineticsCsv As String, EnzymeModifyKinetics As String, Expasy As T_EnzymeClass_BLAST_OUT())
        Call New Compiler.Components.MergeSabiork(_ModelLoader, SabiorkCompoundsCsv).InvokeMergeCompoundSpecies()
        Call New Compiler.Components.SabiorkKinetics(_ModelLoader, SabiorkKineticsCsv, EnzymeModifyKinetics, Expasy).InvokeMethod(MetaCyc:=_MetaCyc, KEGGCompounds:=_KEGGCompounds, KEGGReactions:=_KeggReactions)
    End Sub

    Public Sub WriteData()
        Call _ModelLoader.SaveTo(_ModelLoader.CellSystemModel.FilePath)
    End Sub
End Class
