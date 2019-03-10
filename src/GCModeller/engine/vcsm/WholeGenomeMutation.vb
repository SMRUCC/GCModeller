#Region "Microsoft.VisualBasic::0c470cb74864197dea07b54465ba7e36, vcsm\WholeGenomeMutation.vb"

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

    ' Class WholeGenomeMutation
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Invoke
    ' 
    ' /********************************************************************************/

#End Region

Public Class WholeGenomeMutation

    Dim OriginalCommandl As String

    Sub New(CommandLine As String)
        Me.OriginalCommandl = CommandLine
    End Sub

    Public Function Invoke() As Integer
        Dim CmdlData = CommandLine.TryParse(OriginalCommandl)
        Dim ModelFilePath As String = CmdlData.Item("-i")
        Dim ModelDir As String = FileIO.FileSystem.GetParentPath(ModelFilePath)
        '    Dim GeneObjects = ModelFilePath.LoadXml(Of LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular.FileStream.XmlFormat.CellSystemXmlModel)().Transcript.GetFullPath(ModelDir).LoadCsv(Of LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular.FileStream.Transcript)(False)

        'For Each Gene In GeneObjects
        '    Dim CommandLine As String = String.Format("{0} -gene_mutations {1}|{2}", OriginalCommandl, Gene.Template, CmdlData.Item("-factor"))

        'Next

        Return 0
    End Function
End Class
