#Region "Microsoft.VisualBasic::cc01de1fd02f4d1f29da16556d9ac1ff, GCModeller\core\Bio.Assembly\Assembly\Expasy\EnzymeClass.vb"

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

    '   Total Lines: 69
    '    Code Lines: 29
    ' Comment Lines: 30
    '   Blank Lines: 10
    '     File Size: 2.49 KB


    '     Class EnzymeClass
    ' 
    '         Properties: Catalysts, EC_Class, ExpasyAnnotations, Hits, KEGG_ENTRIES
    '                     ProteinId
    ' 
    '         Function: ToString
    ' 
    '     Class T_ECPaired
    ' 
    '         Properties: ProteinId, uniprot
    ' 
    '         Function: ToString
    ' 
    '     Class T_EnzymeClass_BLAST_OUT
    ' 
    '         Properties: [Class], EValue, Identity
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Assembly.Expasy.AnnotationsTool

    ''' <summary>
    ''' 这个是最终的酶分类结果的呈现形式
    ''' </summary>
    ''' <remarks></remarks>
    Public Class EnzymeClass : Implements INamedValue

        ''' <summary>
        ''' 一种酶分子是可能同时具备有多个酶分类编号的
        ''' </summary>
        ''' <remarks></remarks>
        Public Property EC_Class As String()
        Public Property ProteinId As String Implements INamedValue.Key
        ''' <summary>
        ''' {[EC] Annotation}
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ExpasyAnnotations As String()
        Public Property Catalysts As String()
        Public Property Hits As String()

        ''' <summary>
        ''' KEGG数据库之中的Reaction的编号: {[KEGG_Reaction_Entry] Reaction}
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property KEGG_ENTRIES As String()

        Public Overrides Function ToString() As String
            Return String.Format("{0} -> {1}", ProteinId, String.Join(", ", EC_Class))
        End Function
    End Class

    Public MustInherit Class T_ECPaired : Implements INamedValue
        Implements IKeyValuePairObject(Of String, String)

        Public Property ProteinId As String Implements INamedValue.Key, IKeyValuePairObject(Of String, String).Key
        Public Property uniprot As String Implements IKeyValuePairObject(Of String, String).Value

        Public Overrides Function ToString() As String
            Return MyClass.GetJson
        End Function
    End Class

    ''' <summary>
    ''' The raw annotation data which was export from the blast output text.(蛋白酶的酶编号分类（这个数据结构是用来表示blast比对结果的）)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class T_EnzymeClass_BLAST_OUT : Inherits T_ECPaired

        ''' <summary>
        ''' 酶分类的EC编号
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property [Class] As String
        Public Property EValue As Double
        Public Property Identity As Double
    End Class
End Namespace
