#Region "Microsoft.VisualBasic::3fc63b1c580f89df4337867d5e9acbfb, analysis\RNA-Seq\Toolkits.RNA-Seq\Matrix\Experiment.vb"

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

    '     Structure Experiment
    ' 
    '         Properties: Experiment, Reference, Sample
    ' 
    '         Function: __sampleTable, GetSamples, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace dataExprMAT

    Public Structure Experiment

        Public Property Sample As String
        Public Property Experiment As String
        Public Property Reference As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        ''' <summary>
        ''' <see cref="Experiment"/>/<see cref="Reference"/>
        ''' </summary>
        ''' <param name="expr">&lt;a/b>|&lt;c/d>|&lt;e/f>|....</param>
        ''' <returns></returns>
        Public Shared Function GetSamples(expr As String) As Experiment()
            Dim pairs As String() = expr.Split("|"c)
            Dim samples As Experiment() = pairs.Select(AddressOf __sampleTable)
            Return samples
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="s">a/b</param>
        ''' <returns></returns>
        Private Shared Function __sampleTable(s As String) As Experiment
            Dim tokens As String() = s.Split("/"c)
            Return New Experiment With {
                .Experiment = tokens(0),
                .Reference = tokens(1),
                .Sample = s
            }
        End Function
    End Structure
End Namespace
