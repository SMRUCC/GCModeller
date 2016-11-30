
Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language

Namespace ComponentModel.Loci

    Public Module Extensions

        ''' <summary>
        ''' 这个函数返回来的位点里面的<see cref="MotifSite.Type"/>信息可以使用``+``分割
        ''' </summary>
        ''' <param name="sites"></param>
        ''' <param name="gapOffset%">默认是不允许跳过gap间隙的</param>
        ''' <returns></returns>
        <Extension>
        Public Function Assemble(sites As IEnumerable(Of IMotifSite), Optional gapOffset% = 0) As MotifSite()
            Dim locations As New List(Of Location)
            Dim sitesData As IMotifSite() = sites.ToArray

            Const motif As String = NameOf(motif)

            For Each x As IMotifSite In sitesData
                x.Site.Extension = New ExtendedProps
                x.Site.Extension.DynamicHash(motif) = x
                locations.Add(x.Site)
            Next

            Dim assm As Location() = locations _
                .OrderBy(Function(x) x.Left) _
                .FragmentAssembly(gapOffset)

            Dim out As New List(Of MotifSite)

            For Each x As Location In assm
                Dim o As IMotifSite = DirectCast(x.Extension.DynamicHash(motif), IMotifSite)

                Call x.Extension _
                    .DynamicHash _
                    .Properties _
                    .Remove(motif)
                out += New MotifSite With {
                    .Name = o.Name,
                    .Type = {
                        o.Type
                    } _
                    .Join(x.Extension _
                           .DynamicHash _
                           .Properties _
                           .Values _
                           .Select(Function(s) DirectCast(DirectCast(s, Location) _
                           .Extension _
                           .DynamicHash _
                           .Properties(motif), IMotifSite).Type)) _
                    .JoinBy("+")  ' 这里不进行Distinct了，因为这些重复的类型可能还有别的用途，例如数量上面的统计之类的
                }
            Next

            Return out.ToArray
        End Function
    End Module
End Namespace