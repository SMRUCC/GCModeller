#Region "Microsoft.VisualBasic::ab9eea3e20a39f1a368c050181664831, R#\gseakit\KEGG.vb"

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

    '   Total Lines: 40
    '    Code Lines: 23 (57.50%)
    ' Comment Lines: 10 (25.00%)
    '    - Xml Docs: 90.00%
    ' 
    '   Blank Lines: 7 (17.50%)
    '     File Size: 1.33 KB


    ' Module KEGG
    ' 
    '     Function: compoundSet
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.XML
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' the kegg background helper
''' </summary>
''' 
<Package("kegg")>
Module KEGG

    ''' <summary>
    ''' gt kegg compound set from a kegg pathway map collection
    ''' </summary>
    ''' <param name="maps"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("compound_set")>
    Public Function compoundSet(<RRawVectorArgument> maps As Object, Optional env As Environment = Nothing) As Object
        Dim coll = pipeline.TryCreatePipeline(Of Map)(maps, env)

        If coll.isError Then
            Return coll.getError
        End If

        Dim cpd_set As list = list.empty

        For Each map As Map In Tqdm.Wrap(coll.populates(Of Map)(env).ToArray)
            Call cpd_set.add(map.EntryId, map.GetCompoundSet.ToDictionary(Function(a) a.Name, Function(a) a.Value))
        Next

        Return cpd_set
    End Function

End Module

